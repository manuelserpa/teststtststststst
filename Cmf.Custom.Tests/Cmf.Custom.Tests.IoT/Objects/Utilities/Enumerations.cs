using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMSOsramEIAutomaticTests.Objects.Utilities
{
   public enum LoadPortStateModelStateEnum
    {
        Available,
        Occupied,
        ReadyToUnload
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
