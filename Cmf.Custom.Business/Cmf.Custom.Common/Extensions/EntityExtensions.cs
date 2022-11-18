using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.Abstractions;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Cmf.Custom.amsOSRAM.Common.Extensions
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
		public static T GenerateLightEntity<T>(this T original) where T : IEntity, new()
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
        public static IStateModelTransition GetStateModelTransitionForState(this IEntityInstance entityInstance, string toStateName)
        {
            IStateModelTransition transition = null;

            if (entityInstance?.CurrentMainState != null)
            {
                entityInstance.CurrentMainState.StateModel.Load();

                IStateModelTransitionCollection transitionsForState = entityInstance.CurrentMainState.StateModel.GetPossibleTransitionsForState(entityInstance.CurrentMainState.CurrentState);

                transition = transitionsForState.FirstOrDefault(t => t.ToState.Name.Equals(toStateName, StringComparison.InvariantCultureIgnoreCase)) as IStateModelTransition;
            }

            return transition;
        }

        #region Collection Utilities

        /// <summary>
        /// Loads a given set of IDs of the same base type in collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instances">Collection instance. Cleared during load and populated with loaded items</param>
        /// <param name="idsToLoad">Collection of IDs to be loaded</param>
        public static void LoadByIDs<T>(this IEntityCollection<T> instances, Collection<long> idsToLoad) where T : IEntity
        {
            instances.LoadByIDs(idsToLoad.ToList());
        }

        /// <summary>
        /// Loads a given set of IDs of the same base type in collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instances">Collection instance. Cleared during load and populated with loaded items</param>
        /// <param name="idsToLoad">Collection of IDs to be loaded</param>
        public static void LoadByIDs<T>(this ICoreBaseCollection<T> instances, List<long> idsToLoad) where T : ICoreBase
        {
            if (instances != null && idsToLoad != null)
            {
                instances.Clear();

                List<long> validIds = idsToLoad.Distinct().Where(E => E > 0).ToList();
                if (validIds.Count > 0)
                {
                    // Get services provider information
                    IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
                    IGenericServiceOrchestration genericServiceOrchestration = serviceProvider.GetService<IGenericServiceOrchestration>();

                    Collection<object> foundObjects = genericServiceOrchestration.GetObjectsByFilter(new GetObjectsByFilterInput()
                    {
                        Filter = new FilterCollection()
                         {
                             new Filter()
                             {
                                 Name = "Id"
                                 , Operator = FieldOperator.In
                                 , Value = validIds
                             }
                         }
                         ,
                        Type = serviceProvider.GetService<T>()
                    }).Instance;

                    if (foundObjects != null && foundObjects.Count > 0)
                    {
                        foreach (object loadedObject in foundObjects)
                        {
                            instances.Add((T)loadedObject);
                        }
                    }
                }
            }
        }

        #endregion Collection Utilities
    }
}