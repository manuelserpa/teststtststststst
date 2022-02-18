using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMSOsramEIAutomaticTests.Objects.Utilities
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
