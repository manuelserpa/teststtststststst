using System.Runtime.Serialization;

namespace Cmf.Custom.Tests.Biz.Common.Tibco
{
    public enum CustomSendEventMessageTopics
    {
        [EnumMember(Value = "CustomLotChange")]
        CustomLotChange,

        [EnumMember(Value = "CustomEquipmentStatusChange")]
        CustomEquipmentStatusChange
    }
}
