using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using cmConnect.TestFramework.Common.Drivers.SpecialVariables;
using cmConnect.TestFramework.EquipmentSimulator.Drivers;
using Cmf.SECS.Driver;
using AMSOsramEIAutomaticTests.IoT.Common.Attributes;

namespace AMSOsramEIAutomaticTests.IoT.Common
{
    public class ComplexList<T> : ICustomVariable
    {
		private SecsGemEquipment m_Driver;

		public T ComplexListObject { get; set; }

		public ComplexList(SecsGemEquipment driver)
		{
			m_Driver = driver;
		}

		public void SetValue(SecsItem secsItem2Set, SecsItem.ItemType referenceItemType)
		{
			secsItem2Set.SetTypeToList();

			foreach (PropertyInfo prop in typeof(T).GetProperties())
			{
				var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

				var attributes = prop.GetCustomAttributes();
				var attributeCasted = attributes.ToList().First() as SecsItemTypeAttribute;

				AddVariableValue(secsItem2Set, "", prop.GetValue(ComplexListObject), attributeCasted.Type);
			}

		}

		private SecsItem AddVariableValue(SecsItem list, string abstractVariable, object dummyValue = null, SecsItem.ItemType dummyValueType = SecsItem.ItemType.Error)
		{
			SecsItem varValue = new SecsItem();


			if (dummyValueType == SecsItem.ItemType.List)
			{
				varValue.SetTypeToList();

				foreach (PropertyInfo prop in dummyValue.GetType().GetProperties())
				{
					var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

					var attributes = prop.GetCustomAttributes();
					var attributeCasted = attributes.ToList().First() as SecsItemTypeAttribute;

					AddVariableValue(varValue, "", prop.GetValue(dummyValue), attributeCasted.Type);
				}
			}
			else if (dummyValueType == SecsItem.ItemType.Variant)
			{
				varValue.SetTypeToList();

				if (dummyValue is IList)
				{
					var genericType = dummyValue.GetType().GenericTypeArguments[0];

					var attributes = genericType.GetCustomAttributes();
					var attributeCasted = attributes.ToList().First() as SecsItemTypeAttribute;

					foreach (var itemValue in (dummyValue as IList))
					{
						SecsItem varSubValue = new SecsItem();
						AddVariableValue(varValue, "", itemValue, attributeCasted.Type);
					}
				}

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
