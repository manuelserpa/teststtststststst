using Cmf.Foundation.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cmf.Custom.AMSOsram.Common.Extensions
{
    /// <summary>
    /// Extensions to extend entity functionalities
    /// </summary>
	public static class EntityExtensions
	{
        /// <summary>
        /// Creates an entity with just the Id and Name filled
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
		public static T GenerateLightEntity<T>(this T original) where T: Entity, new()
		{
			T lightEntity = new T();
			PropertyInfo propertyInfo = lightEntity.GetType().GetProperty("Id");
			propertyInfo.SetValue(lightEntity, Convert.ChangeType(original.Id, propertyInfo.PropertyType), null);
			lightEntity.Name = original.Name;

			return lightEntity;			
		}

        /// <summary>
        /// Get state model transitions from the EntityInstance current state to the given state
        /// </summary>
        /// <param name="entityInstance"></param>
        /// <param name="toStateName"></param>
        /// <returns>bool</returns>
        public static StateModelTransition GetStateModelTransitionForState(this EntityInstance entityInstance, string toStateName)
        {
            StateModelTransition transition = null;

            if (entityInstance?.CurrentMainState != null)
            {
                entityInstance.CurrentMainState.StateModel.Load();

                var transitionsForState = entityInstance.CurrentMainState.StateModel.GetPossibleTransitionsForState(entityInstance.CurrentMainState.CurrentState);

                transition = transitionsForState.FirstOrDefault(t => t.ToState.Name.Equals(toStateName, StringComparison.InvariantCultureIgnoreCase));
            }

            return transition;
        }
    }
}
