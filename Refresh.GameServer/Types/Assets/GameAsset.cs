using System.Security.Cryptography;
using Bunkum.Core.Storage;
using Realms;
using Refresh.GameServer.Authentication;
using Refresh.GameServer.Database;
using Refresh.GameServer.Importing;
using Refresh.GameServer.Importing.Mip;
using Refresh.GameServer.Resources;
using Refresh.GameServer.Types.UserData;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Refresh.GameServer.Types.Assets;

public partial class GameAsset : IRealmObject
{
    [PrimaryKey] public string AssetHash { get; set; } = string.Empty;
    public GameUser? OriginalUploader { get; set; }
    public DateTimeOffset UploadDate { get; set; }
    public bool IsPSP { get; set; }
    public int SizeInBytes { get; set; }
    [Ignored] public GameAssetType AssetType
    {
        get => (GameAssetType)this._AssetType;
        set => this._AssetType = (int)value;
    }

    // ReSharper disable once InconsistentNaming
    internal int _AssetType { get; set; }

    public IList<string> Dependencies { get; } = null!;

    [Ignored] public AssetSafetyLevel SafetyLevel => AssetSafetyLevelExtensions.FromAssetType(this.AssetType);

    public string? AsMainlineIconHash { get; set; }
    public string? AsMipIconHash { get; set; }
    
    //NOTE: there's no "as MIP photo hash" because theres no way to browse photos on LBP PSP.
    public string? AsMainlinePhotoHash { get; set; }

    /// <summary>
    /// Transforms an input image into a photo-friendly format
    /// </summary>
    /// <param name="image">The input photo</param>
    /// <returns>The transformed image</returns>
    private Image<Rgba32>? PhotoTransformation(Image image)
    {
        //No transformation needs to be done for photos.
        return null;
    }
    
    /// <summary>
    /// Automatically crops and resizes an image into its corresponding icon form.
    /// </summary>
    /// <param name="image">The source image</param>
    /// <returns>The cropped and resized image, or null if its already fine</returns>
    private Image<Rgba32>? CropToIcon(Image image)
    {
        const int maxWidth = 256;
        
        //If the image is already square, and already small enough for our uses, then we can just return it as-is
        if (image.Width == image.Height && image.Width <= maxWidth) return null;

        Image<Rgba32> copy = image.CloneAs<Rgba32>();

        //If the image is already square, just resize it.
        if (image.Width == image.Height)
        {
            copy.Mutate(ctx => ctx.Resize(maxWidth, maxWidth));

            return copy;
        }
        
        Rectangle cropRectangle;
        
        //If the image is wider than it is tall
        cropRectangle = image.Width > image.Height ? new Rectangle(image.Width / 2 - image.Height / 2, 0, image.Height, image.Height) :
            //If the image is taller than it is wide
            new Rectangle(0, image.Height / 2 - image.Width / 2, image.Width, image.Width);

        int targetWidth = Math.Clamp(cropRectangle.Width, 16, maxWidth);
        
        //Round to the nearest multiple of 16, this is to make PSP happy
        targetWidth = 
            (int)Math.Round(
                targetWidth / (double)16,
                MidpointRounding.AwayFromZero
            ) * 16;
        
        copy.Mutate(ctx => ctx.Crop(cropRectangle).Resize(targetWidth, targetWidth));
        
        return copy;
    }

    /// <summary>
    /// Transforms an image to be consumed by a particular game
    /// </summary>
    /// <param name="game">The game which will consume the resulting asset</param>
    /// <param name="dataStore">The data store</param>
    /// <param name="decodeImage">The function used to decode an image from the data store</param>
    /// <param name="transformImage">The transformation function</param>
    /// <returns>The hash of the transformed asset</returns>
    /// <exception cref="NotImplementedException">That conversion step is unimplemented at the moment</exception>
    /// <exception cref="ArgumentOutOfRangeException">Invalid TokenGame</exception>
    private string TransformImage(TokenGame game, IDataStore dataStore, Func<string, Image<Rgba32>> decodeImage, Func<Image, Image<Rgba32>?> transformImage)
    {
        string dataStorePath = this.IsPSP ? $"psp/{this.AssetHash}" : this.AssetHash;

        bool mainlineDoesntNeedConversion = this.AssetType is GameAssetType.Png or GameAssetType.Texture or GameAssetType.GameDataTexture;
        
        switch (game)
        {
            case TokenGame.Website:
            case TokenGame.LittleBigPlanet1: 
            case TokenGame.LittleBigPlanet2:
            case TokenGame.LittleBigPlanet3:
            case TokenGame.LittleBigPlanetVita: {
                Image sourceImage = decodeImage(dataStorePath);
                
                //Load the image from the data store and transform it
                Image? image = transformImage(sourceImage);
                //If its null, then no transformation was needed
                if (image == null)
                {
                    if (mainlineDoesntNeedConversion)
                    {
                        //Return the existing asset hash
                        return this.AssetHash;
                    }

                    //Set the image to use to the source image
                    image = sourceImage;
                }

                //Save the image as a PNG file in a byte array in memory
                MemoryStream memory = new();
                image.SaveAsPng(memory);
                byte[] data = memory.ToArray();

                //Get the hash of the converted asset
                string convertedHash = AssetImporter.BytesToHexString(SHA1.HashData(data));

                //Write the data to the store
                dataStore.WriteToStore(convertedHash, data);

                //Return the new icon hash
                return convertedHash;
            }
            case TokenGame.LittleBigPlanetPSP: {
                Image<Rgba32> sourceImage = decodeImage(dataStorePath);

                //Transform the image, if no transformation is needed, use the source image
                Image<Rgba32> image = transformImage(sourceImage) ?? sourceImage;

                MemoryStream memory = new();
                new MipEncoder().Encode(image, memory);
                //Get the used chunk of the underlying buffer
                Span<byte> data = memory.GetBuffer().AsSpan()[..((int)memory.Length)];
                //Encrypt the data
                byte[] encryptedData = ResourceHelper.PspEncrypt(data, Importer.PSPKey.Value);

                //Get the hash
                string convertedHash = AssetImporter.BytesToHexString(SHA1.HashData(encryptedData));

                dataStore.WriteToStore($"psp/{convertedHash}", encryptedData);

                return convertedHash;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(game), game, null);
        }
    }

    public string GetAsPhoto(TokenGame game, GameDatabaseContext database, IDataStore dataStore)
    {
        return this.GetAsGeneric(
            game,
            database,
            dataStore,
            this.PhotoTransformation,
            () => this.AsMainlinePhotoHash,
            hash => database.SetAsMainlinePhotoHash(this, hash),
            () => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        ); 
    }

    public string GetAsIcon(TokenGame game, GameDatabaseContext database, IDataStore dataStore)
    {
        return this.GetAsGeneric(
            game,
            database,
            dataStore,
            this.CropToIcon,
            () => this.AsMainlineIconHash,
            hash => database.SetAsMainlineIconHash(this, hash),
            () => this.AsMipIconHash,
            hash => database.SetAsMipIconHash(this, hash)
        );
    }

    public string GetAsGeneric(TokenGame game, GameDatabaseContext database, IDataStore dataStore, Func<Image, Image<Rgba32>?> transformImage, Func<string?> getMainline, Action<string> setMainline, Func<string?> getMip, Action<string> setMip)
    {
        switch (this.AssetType)
        {
            case GameAssetType.Tga:
            case GameAssetType.Jpeg:
            case GameAssetType.Png:
                switch (game)
                {
                    case TokenGame.Website:
                    case TokenGame.LittleBigPlanet1: 
                    case TokenGame.LittleBigPlanet2:
                    case TokenGame.LittleBigPlanet3:
                    case TokenGame.LittleBigPlanetVita: {
                        //If the cached icon hash is already set, early return it.
                        if (getMainline() != null) return getMainline()!;

                        string convertedHash = this.TransformImage(game, dataStore, path => Image.Load<Rgba32>(dataStore.GetStreamFromStore(path)), transformImage);

                        setMainline(convertedHash);
                        
                        //Return the new icon hash
                        return getMainline()!;
                    }
                    case TokenGame.LittleBigPlanetPSP: {
                        if (getMip() != null) return getMip()!;

                        string convertedHash = this.TransformImage(game, dataStore, path => Image.Load<Rgba32>(dataStore.GetStreamFromStore(path)), transformImage);

                        setMip(convertedHash);
                        
                        return getMip()!;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(game), game, null);
                }
            case GameAssetType.Texture:
                switch (game)
                {
                    case TokenGame.Website:
                    case TokenGame.LittleBigPlanet1: 
                    case TokenGame.LittleBigPlanet2:
                    case TokenGame.LittleBigPlanet3:
                    case TokenGame.LittleBigPlanetVita: {
                        //If the cached icon hash is already set, early return it.
                        if (getMainline() != null) return getMainline()!;

                        string convertedHash = this.TransformImage(game, dataStore, path => ImageImporter.LoadTex(dataStore.GetStreamFromStore(path)), transformImage);

                        setMainline(convertedHash);
                        
                        //Return the new icon hash
                        return getMainline()!;
                    }
                    case TokenGame.LittleBigPlanetPSP: {
                        if (getMip() != null) return getMip()!;

                        string convertedHash = this.TransformImage(game, dataStore, path => ImageImporter.LoadTex(dataStore.GetStreamFromStore(path)), transformImage);
 
                        setMip(convertedHash);
                        
                        return getMip()!;
                    };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(game), game, null);
                }
            case GameAssetType.GameDataTexture:
                switch (game)
                {
                    case TokenGame.Website:
                    case TokenGame.LittleBigPlanet1:
                    case TokenGame.LittleBigPlanet2:
                    case TokenGame.LittleBigPlanet3:
                    case TokenGame.LittleBigPlanetVita: {
                        //If the cached icon hash is already set, early return it.
                        if (getMainline() != null) return getMainline()!;
                        
                        string convertedHash = this.TransformImage(game, dataStore, path => ImageImporter.LoadGtf(dataStore.GetStreamFromStore(path)), transformImage);

                        setMainline(convertedHash);
                        
                        //Return the new icon hash
                        return getMainline()!;
                    }
                    case TokenGame.LittleBigPlanetPSP: {
                        if (getMip() != null) return getMip()!;

                        string convertedHash = this.TransformImage(game, dataStore, path => ImageImporter.LoadGtf(dataStore.GetStreamFromStore(path)), transformImage);
 
                        setMip(convertedHash);
                        
                        return getMip()!;
                    };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(game), game, null);
                }
            case GameAssetType.Mip:
                switch (game)
                {
                    //LBP1, LBP2, LBP3, and LBP Vita are unable to handle MIP files.
                    //The Website technically can utilize them after import,
                    //but using PNGs for the site will cause less load on the server, so lets do that!
                    case TokenGame.Website:
                    case TokenGame.LittleBigPlanet1:
                    case TokenGame.LittleBigPlanet2:
                    case TokenGame.LittleBigPlanet3:
                    case TokenGame.LittleBigPlanetVita: {
                        //If the cached icon hash is already set, early return it.
                        if (getMainline() != null) return getMainline()!;
                        
                        string convertedHash = this.TransformImage(game, dataStore, path =>
                        {
                            //Load the data from the data store
                            byte[] rawData = dataStore.GetDataFromStore(path);
                            //Decrypt it
                            byte[] sourceData = ResourceHelper.PspDecrypt(rawData, Importer.PSPKey.Value);

                            //Create a memory stream from the decrypted asset data
                            using MemoryStream sourceDataStream = new(sourceData);
                        
                            //Load the mip file
                            return ImageImporter.LoadMip(sourceDataStream);
                        }, transformImage);

                        setMainline(convertedHash);
                        
                        //Return the new icon hash
                        return getMainline()!;
                    }
                    case TokenGame.LittleBigPlanetPSP: {
                        //If the cached icon hash is already set, early return it.
                        if (getMip() != null) return getMip()!;

                        string convertedHash = this.TransformImage(game, dataStore, path =>
                        {
                            //Load the data from the data store
                            byte[] rawData = dataStore.GetDataFromStore(path);
                            //Decrypt it
                            byte[] sourceData = ResourceHelper.PspDecrypt(rawData, Importer.PSPKey.Value);

                            //Create a memory stream from the decrypted asset data
                            using MemoryStream sourceDataStream = new(sourceData);

                            //Load the mip file
                            return ImageImporter.LoadMip(sourceDataStream);
                        }, transformImage);

                        setMip(convertedHash);
                        
                        return getMip()!;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(game), game, null);
                }
            case GameAssetType.Level:
            case GameAssetType.Painting:
            case GameAssetType.Plan:
            case GameAssetType.Material:
            case GameAssetType.Mesh:
            case GameAssetType.Palette:
            case GameAssetType.Script:
            case GameAssetType.MoveRecording:
            case GameAssetType.VoiceRecording:
            case GameAssetType.SyncedProfile:
            case GameAssetType.Unknown:
            default:
                throw new InvalidOperationException($"Format '{this.AssetType}' is not a valid image.");
        }
    }
}