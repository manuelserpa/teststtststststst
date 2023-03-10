using cmConnect.TestFramework.Common.Drivers.SpecialVariables;
using cmConnect.TestFramework.EquipmentSimulator.Drivers;
using Cmf.SECS.Driver;

namespace Cmf.Custom.Tests.IoT.Tests.Common
{
    class DummyList : ICustomVariable
    {
        private SecsGemEquipment m_Driver;

        public int[] LoadPortNumber { get; set; }

        public DummyList(SecsGemEquipment driver)
        {

            /*
				< L 					
					<I1 1>
                    <I1 2>
                    ...
				>				
                
             * */

            m_Driver = driver;
        }

        public void SetValue(SecsItem secsItem2Set, SecsItem.ItemType referenceItemType)
        {
            secsItem2Set.SetTypeToList();

            for (int i = 0; i < LoadPortNumber.Length; i++)
            {
                //SecsItem child = AddVariableValue(secsItem2Set, "", null, SecsItem.ItemType.List);                
                AddVariableValue(secsItem2Set, "", LoadPortNumber[i], SecsItem.ItemType.I1);
            }
        }

        private SecsItem AddVariableValue(SecsItem list, string abstractVariable, object dummyValue = null, SecsItem.ItemType dummyValueType = SecsItem.ItemType.Error)
        {
            SecsItem varValue = new SecsItem();

            if (dummyValueType == SecsItem.ItemType.List)
            {
                varValue.SetTypeToList();
            }
            else
            {
                if (m_Driver.EquipmentVariables.ContainsKey(abstractVariable))
                {
                    var variable = m_Driver.EquipmentVariables[abstractVariable];
                    m_Driver.SetValue(varValue, m_Driver.FromEquipmentDataType(variable.EquipmentDataType), m_Driver.Variables[abstractVariable]);
                }
                else
                    m_Driver.SetValue(varValue, dummyValueType, dummyValue);
            }

            list.Add(varValue);

            return (varValue);
        }
    }
}
