using System;
using System.Collections.Generic;
using System.Text;
using cmConnect.TestFramework.Common.Drivers.SpecialVariables;
using cmConnect.TestFramework.EquipmentSimulator.Drivers;
using Cmf.SECS.Driver;

namespace Cmf.Custom.Tests.IoT.Tests.EvatecClusterline200II
{
    public class SlotMapVariable : ICustomVariable
    {
        private SecsGemEquipment m_Driver;

        public int[] Presence { get; set; }

        public SlotMapVariable(SecsGemEquipment driver)
        {

            /*
				< L 
					<A '00'>
					<U1 1>
				>
				< L 
					<A '01'>
					<U1 0>
				>
				< L 
					<A '02'>
					<U1 1>
				>
                ...
                
             * */

            m_Driver = driver;
        }
        /// <summary>Set the value of a secs item</summary>
        /// <param name="secsItem2Set">Item to change</param>
        /// <param name="referenceItemType">Type of value</param>
        public void SetValue(Cmf.SECS.Driver.SecsItem secsItem2Set, Cmf.SECS.Driver.SecsItem.ItemType referenceItemType)
        {
            secsItem2Set.SetTypeToList();

            for (int i = 0; i < Presence.Length; i++)
            {
                //SecsItem child = AddVariableValue(secsItem2Set, "", null, SecsItem.ItemType.List);                
                AddVariableValue(secsItem2Set, "", Presence[i], SecsItem.ItemType.U1);
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
