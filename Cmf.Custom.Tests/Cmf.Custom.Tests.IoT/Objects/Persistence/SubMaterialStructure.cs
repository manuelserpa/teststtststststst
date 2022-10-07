namespace amsOSRAMEIAutomaticTests.Objects.Persistence
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
        Processed,
        Skipped
    }

}
