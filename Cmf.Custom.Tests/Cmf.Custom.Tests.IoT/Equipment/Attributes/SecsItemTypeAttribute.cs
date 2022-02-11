using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cmf.SECS.Driver;

namespace AMSOsramEIAutomaticTests.IoT.Common.Attributes
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
