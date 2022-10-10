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
}
