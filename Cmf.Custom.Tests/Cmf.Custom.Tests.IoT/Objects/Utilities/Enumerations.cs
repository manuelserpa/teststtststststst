namespace amsOSRAMEIAutomaticTests.Objects.Utilities
{
    public enum LoadPortStateModelStateEnum
    {
        ReadyToLoad,
        TransferBlocked,
        ReadyToUnload,
        Reserved,
        OutOfService

    }

    public enum MaterialStateModelStateEnum
    {
        Setup,
        InProcess,
        InProgress,
        Complete,
        Processed,
        Abort,
        Queued
    }

    public enum EquipmentControlStateEnum
    {
        EquipmentOffline = 1,
        AttemptOnline = 2,
        HostOffline = 3,
        OnlineLocal = 4,
        OnlineRemote = 5
    }
}
