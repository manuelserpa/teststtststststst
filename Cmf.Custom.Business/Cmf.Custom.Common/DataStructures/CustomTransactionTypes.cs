using System.Runtime.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    /// <summary>
    /// An enum that represents the types of possible actions to Send Message to Tibco
    /// </summary>
    [DataContract]
    public enum CustomTransactionTypes
    {
        /// <summary>
        /// Type that represents the MaterialCreate action
        /// </summary>
        [EnumMember]
        MaterialCreate,

        /// <summary>
        /// Type that represents the MaterialTerminate action
        /// </summary>
        [EnumMember]
        MaterialTerminate,

        /// <summary>
        /// Type that represents the MaterialDispatch action
        /// </summary>
        [EnumMember]
        MaterialDispatch,

        /// <summary>
        /// Type that represents the MaterialTrackIn action
        /// </summary>
        [EnumMember]
        MaterialTrackIn,

        /// <summary>
        /// Type that represents the MaterialTrackOut action
        /// </summary>
        [EnumMember]
        MaterialTrackOut,

        /// <summary>
        /// Type that represents the MaterialMoveNextPost action
        /// </summary>
        [EnumMember]
        MaterialMoveNextPost,

        /// <summary>
        /// Type that represents the MaterialSplit action
        /// </summary>
        [EnumMember]
        MaterialSplit,

        /// <summary>
        /// Type that represents the MaterialMerge action
        /// </summary>
        [EnumMember]
        MaterialMerge,

        /// <summary>
        /// Type that represents the MaterialLoss action
        /// </summary>
        [EnumMember]
        MaterialLoss,

        /// <summary>
        /// Type that represents the MaterialBonus action
        /// </summary>
        [EnumMember]
        MaterialBonus,

        /// <summary>
        /// Type that represents the MaterialHold action
        /// </summary>
        [EnumMember]
        MaterialHold,

        /// <summary>
        /// Type that represents the MaterialRelease action
        /// </summary>
        [EnumMember]
        MaterialRelease,

        /// <summary>
        /// Type that represents the ContainerAssociation action
        /// </summary>
        [EnumMember]
        ContainerAssociation,

        /// <summary>
        /// Type that represents the MaterialMoveNextPre action
        /// </summary>
        [EnumMember]
        MaterialMoveNextPre
    }
}
