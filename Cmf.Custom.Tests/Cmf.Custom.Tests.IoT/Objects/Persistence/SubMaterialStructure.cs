using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMSOsramEIAutomaticTests.Objects.Persistence
{
    public class SubMaterialStructure
    {
        public string MaterialId;
        public string MaterialName;
        public SubMaterialStateEnum MaterialState;
        public long Slot;       
    }

    public enum SubMaterialStateEnum
    {
        Queued,
        InProcess,
        Process,
        Skipped
    }

}
