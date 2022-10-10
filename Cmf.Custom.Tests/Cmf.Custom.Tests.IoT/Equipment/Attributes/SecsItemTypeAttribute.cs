using System;
using Cmf.SECS.Driver;

namespace amsOSRAMEIAutomaticTests.IoT.Common.Attributes
{
	public class SecsItemTypeAttribute : Attribute
	{
		public SecsItemTypeAttribute(SecsItem.ItemType type)
		{
			Type = type;
		}
		public SecsItem.ItemType Type { get; set; }
	}
}
