using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using amsOSRAMEIAutomaticTests.IoT.Common.Attributes;
using cmConnect.TestFramework.Common.Drivers.SpecialVariables;
using cmConnect.TestFramework.EquipmentSimulator.Drivers;
using Cmf.SECS.Driver;


namespace Cmf.Custom.Tests.IoT.Tests.Common
{

	class SubstHistoryList : ICustomVariable
	{
		private SecsGemEquipment m_Driver;

		public SubstHistoryInternalList SubstHistoryInternalList { get; set; }

		public SubstHistoryList(SecsGemEquipment driver)
		{

			m_Driver = driver;
		}

		public void SetValue(SecsItem secsItem2Set, SecsItem.ItemType referenceItemType)
		{
			secsItem2Set.SetTypeToList();

			SubstHistoryInternalList.SetValue(secsItem2Set);
		}
	}


	public class SubstHistoryInternalList : ICustomVariable
	{
		[SecsItemType(SecsItem.ItemType.Variant)]
		public List<SubstHistoryEntry> SubstHistoryEntryList { get; set; }

		public void SetValue(SecsItem secsList, SecsItem.ItemType referenceItemType = SecsItem.ItemType.List) {
			secsList.SetTypeToList();
			foreach (var entry in SubstHistoryEntryList){
				var secsEntry = new SecsItem();
				secsEntry.SetTypeToList();

				secsEntry.Add(new SecsItem { ASCII = entry.Location });
				secsEntry.Add(new SecsItem { ASCII = entry.TimeIn	});
				secsEntry.Add(new SecsItem { ASCII = entry.TimeOut	});

				secsList.Add(secsEntry);
			}
		}
    }

	[SecsItemType(SecsItem.ItemType.List)]
	public class SubstHistoryEntry
	{
		[SecsItemType(SecsItem.ItemType.Ascii)]
		public string Location { get; set; }
		[SecsItemType(SecsItem.ItemType.Ascii)]
		public string TimeIn { get; set; }
		[SecsItemType(SecsItem.ItemType.Ascii)]
		public string TimeOut{ get; set; }
	}
}
