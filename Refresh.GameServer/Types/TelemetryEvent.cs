﻿namespace Refresh.GameServer.Types;

public enum TelemetryEvent : uint
{
    Header,
    Start,
    End,
    TestInt,
    TestV2,
    TestM44,
    TestChar,
    EnterLevel,
    CdnVector,
    ConnectionQuality,
    LeaveLevel,
    LeaveCreateLevel,
    LevelHearted,
    LevelRated,
    AccessoriesConnected,
    DebugNameLookup,
    CostumesWorn,
    DlcOwned,
    UpdatePosition,
    DeathPosition,
    SuicidePosition,
    RestartPosition,
    QuitPosition,
    SwitchToEasyPosition,
    OffScreenPosition,
    PhotoPosition,
    GameMessage,
    PrizeBubblePosition,
    LostAllLivesPosition,
    StickerPosition,
    PoppetState,
    PodComputerState,
    ExpressionState,
    LevelPublish,
    PadDisconnectPosition,
    AiDeathPosition,
    ExceptionHandlePosition,
    UserExperienceMetrics,
    OffscreenDeathPosition,
    InventoryItemClick,
    OpenPsid,
    Is50hzTv,
    IsStandardDefTv,
    UsingImportedLbp1Profile,
    MergeProfile,
    ImportProfile,
    ExportProfile,
    ExportLevel,
    ImportLevel,
    MoveTutorial,
    MoveCalibration,
    GameVersion,
    ModalOverlayState,
    Friend,
    Challenge,
    LiveArea,
    StoreAction,
    StoreCheckout,
    TakePhoto,
    Message,
    GameProgression,
    ChallengePlanetInteraction,
    PlayerChallengeEntered,
    PlayerChallengePlayedEnd,
    PlayerChallengeCreationEnd,
    PlayerChallengePublish,
    ChangingRoomAction,
    MainPlayerCostume,
    StoryChallengeEntered,
    StoryChallengePlayedEnd,
    DcdsAction,
}