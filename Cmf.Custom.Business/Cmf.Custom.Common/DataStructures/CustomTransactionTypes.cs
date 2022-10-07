using System.Runtime.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    [DataContract]
    public enum CustomTransactionTypes
    {
        [EnumMember]
        MaterialCreate,

        [EnumMember]
        MaterialTerminate,

        [EnumMember]
        MaterialDispatch,

        [EnumMember]
        MaterialTrackIn,

        [EnumMember]
        MaterialTrackOut,

        [EnumMember]
        MaterialMoveNext,

        [EnumMember]
        MaterialSplit,

        [EnumMember]
        MaterialMerge,

        [EnumMember]
        MaterialLoss,

        [EnumMember]
        MaterialBonus,

        [EnumMember]
        MaterialHold,

        [EnumMember]
        MaterialRelease,

        [EnumMember]
        ContainerAssociation
    }
}
