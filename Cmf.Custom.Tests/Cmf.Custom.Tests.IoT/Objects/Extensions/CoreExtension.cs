using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.SecurityManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.SecurityManagement.OutputObjects;
using Cmf.Foundation.Common.Base;
using Cmf.Foundation.Security;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.FacilityManagement.FlowManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects;
using Cmf.TestScenarios.Others;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace amsOSRAMEIAutomaticTests.Objects.Extensions
{
    /// <summary>
    /// Extension Methods Class that adapts generic services as Business Objects extension methods
    /// </summary>
    public static partial class Extensions
    {
        // holds properties that are related with service history control..
        static Collection<string> _ServiceProperties = new Collection<string>() { "LastServiceHistoryId", "LastOperationHistorySeq", "ModifiedBy", "ModifiedOn" };

        #region Load

        #region Private Methods

        #region Singular

        /// <summary>
        /// Performs a load for the current object
        /// </summary>
        /// <typeparam name="T">Type of the object to be loaded</typeparam>
        /// <param name="instance">object instance to be loaded</param>
        /// <returns></returns>
        private static T InternalLoad<T>(this T instance) where T : EntityBase
        {
            return InternalLoad(instance, 0);
        }

        /// <summary>
        /// Peforms a load of a given instance, based on its Id or Name
        /// </summary>
        /// <typeparam name="T">Type of the instance to be loaded</typeparam>
        /// <param name="instance">instance to be loaded</param>
        /// <param name="levelsToLoad">levels to load</param>
        /// <returns>loaded instance</returns>
        private static T InternalLoad<T>(this T instance, Int32 levelsToLoad) where T : EntityBase
        {
            if (instance != null)
            {
                if (instance.Id > 0)
                    InternalLoad(instance, instance.Id, levelsToLoad);
                else if (!String.IsNullOrEmpty(instance.Name))
                    InternalLoad(instance, instance.Name, levelsToLoad);
            }
            return instance;
        }

        /// <summary>
        /// Loads an instance based on its Id
        /// </summary>
        /// <typeparam name="T">type of the instance to be loaded</typeparam>
        /// <param name="instance">instance to be loaded</param>
        /// <param name="Id">Id of the instance to load</param>
        /// <param name="levelsToLoad">Number of levels to load for that instance</param>
        /// <returns>loaded instance</returns>
        private static T InternalLoad<T>(this T instance, Int64 Id, Int32 levelsToLoad = 0) where T : CoreBase
        {

            T outObject = (T)new GetObjectByIdInput() { Id = Id, Type = instance, LevelsToLoad = 0 }.GetObjectByIdSync().Instance;
            if (outObject != null)
            {
                instance.CopyFrom(outObject);
            }

            return instance;
        }

        /// <summary>
        /// Loads an instance based on its name
        /// </summary>
        /// <typeparam name="T">type of the instance to be loaded</typeparam>
        /// <param name="instance">instance to be loaded</param>
        /// <param name="Name">Name of the instance to load</param>
        /// <param name="levelsToLoad">Number of levels to load for that instance</param>
        /// <returns>loaded instance</returns>
        private static T InternalLoad<T>(this T instance, String Name, Int32 levelsToLoad = 0) where T : EntityBase
        {
            T outObject = (T)new GetObjectByNameInput() { Name = Name, Type = instance, LevelsToLoad = 0 }.GetObjectByNameSync().Instance;
            if (outObject != null)
            {
                instance.CopyFrom(outObject);
            }
            return instance;
        }

        #endregion Singular

        #region Collection

        /// <summary>
        /// Loads and retrieves a collection of objects based on their IDs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceSample">Sample object for type determination</param>
        /// <param name="instanceIds">Ids of the objects to be loaded</param>
        /// <param name="levelsToLoad">Levels to load</param>
        /// <returns></returns>
        private static Collection<T> InternalCollectionLoad<T>(this T instanceSample, Collection<Int64> instanceIds, Int32 levelsToLoad = 0) where T : EntityBase
        {
            Collection<T> returnObject = null;

            if (instanceIds != null && instanceIds.Any())
            {
                // gather only distinct Ids
                Collection<Int64> internalIDsList = new Collection<Int64>(instanceIds.Distinct().ToList());

                // set up return object
                returnObject = new Collection<T>();

                // if only one entry, use singular...
                if (internalIDsList.Count == 1)
                {
                    T obj = Activator.CreateInstance<T>();
                    returnObject.Add(InternalLoad(obj, internalIDsList.First(), levelsToLoad));
                }
                else
                {
                    // prepare filters
                    FilterCollection filtersToUse = new FilterCollection()
                            {
                                new Filter()
                                {
                                    Name = "Id"
                                    , Operator = Cmf.Foundation.Common.FieldOperator.In
                                    , Value = internalIDsList.ToArray()
                                }
                            };

                    // call service
                    Collection<object> returnedObjects = new GetObjectsByFilterInput() { Filter = filtersToUse, Type = instanceSample, LevelsToLoad = levelsToLoad }.GetObjectsByFilterSync().Instance;


                    // Convert to type collection...
                    returnObject = new Collection<T>(returnedObjects.Select(E => (T)E).ToList());

                }
            }

            return returnObject;
        }

        /// <summary>
        /// Internal collection load method by filters
        /// </summary>
        /// <typeparam name="T">Base type of the objects to be loaded</typeparam>
        /// <param name="instanceSample">instance sample from where type is extracted by the service</param>
        /// <param name="filtersToApply">Filters to be applied</param>
        /// <param name="levelsToLoad">Levels to load. defaults to 0</param>
        /// <returns>Collection of the objects matching filter criteria</returns>
        private static Collection<T> InternalCollectionLoadByFilters<T>(this T instanceSample, FilterCollection filtersToApply, Int32 levelsToLoad = 0) where T : EntityBase
        {
            Collection<T> returnObject = null;

            // call service
            Collection<object> returnedObjects = new GetObjectsByFilterInput() { Filter = filtersToApply, Type = instanceSample, LevelsToLoad = levelsToLoad }.GetObjectsByFilterSync().Instance;

            // Convert to type collection...
            returnObject = new Collection<T>(returnedObjects.Select(E => (T)E).ToList());

            return null;
        }

        #endregion Collection

        #endregion Private Methods

        #region Public Methods

        #region Singular

        /// <summary>
        /// Tries to load a object of a given type by its Id
        /// </summary>
        /// <typeparam name="T">Type of the object to be loaded</typeparam>
        /// <param name="id">Id of the object to be loaded</param>
        /// <param name="levelsToLoad">Load depth level</param>
        /// <returns>loaded object</returns>
        public static T GetEntity<T>(long id, int levelsToLoad = 0)
        {
            return (T)new Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects.GetObjectByIdInput()
            {
                Id = id
                 ,
                Type = Activator.CreateInstance<T>()
                 ,
                LevelsToLoad = levelsToLoad
            }.GetObjectByIdSync().Instance;
        }

        /// <summary>
        /// Tries to load a object of a given type by its name
        /// </summary>
        /// <typeparam name="T">Type of the object to be loaded</typeparam>
        /// <param name="name">Name of the object to be loaded</param>
        /// <param name="levelsToLoad">Load depth level</param>
        /// <returns>loaded object</returns>
        public static T GetEntity<T>(string name, int levelsToLoad = 0)
        {
            return (T)new Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects.GetObjectByNameInput()
            {
                Name = name
                 ,
                Type = Activator.CreateInstance<T>()
                 ,
                LevelsToLoad = levelsToLoad
            }.GetObjectByNameSync().Instance;
        }

        /// <summary>
        /// Performs a load for the current object
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static EntityBase Load(this EntityBase instance)
        {
            return InternalLoad(instance);
        }

        /// <summary>
        /// Loads an instance based on its name
        /// </summary>
        /// <param name="instance">instance to be loaded</param>
        /// <param name="Name">Name of the instance to load</param>
        /// <param name="levelsToLoad">Number of levels to load for that instance</param>
        /// <returns>loaded instance</returns>
        public static EntityBase Load(this EntityBase instance, String Name, Int32 levelsToLoad = 0)
        {
            return InternalLoad(instance, Name, levelsToLoad);
        }

        /// <summary>
        /// Peforms a load of a given instance, based on its Id or Name
        /// </summary>
        /// <param name="instance">instance to be loaded</param>
        /// <param name="levelsToLoad">levels to load</param>
        /// <returns>loaded instance</returns>
        public static EntityBase Load(this EntityBase instance, Int32 levelsToLoad = 0)
        {
            return InternalLoad(instance, levelsToLoad);
        }

        /// <summary>
        /// Loads an instance based on its Id
        /// </summary>
        /// <param name="instance">instance to be loaded</param>
        /// <param name="Id">Id of the instance to load</param>
        /// <param name="levelsToLoad">Number of levels to load for that instance</param>
        /// <returns>loaded instance</returns>
        public static CoreBase Load<T>(this CoreBase instance, Int64 Id, Int32 levelsToLoad = 0)
        {
            return InternalLoad(instance, Id, levelsToLoad);
        }

        /// <summary>
        /// Performs load if either Name or Id ( less than or equal 0) are not set, but at least one of them is..
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="levelsToLoad"></param>
        /// <returns></returns>
        public static T SpecialLoad<T>(this T instance, Int32 levelsToLoad = 1) where T : EntityBase
        {
            if ((String.IsNullOrEmpty(instance.Name) || instance.Id <= 0) && (!String.IsNullOrEmpty(instance.Name) || instance.Id > 0))
            {
                instance.Load(levelsToLoad);
            }

            return instance;
        }

        /// <summary>
        /// Loads a given role by its name or Id
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Role Load(this Role instance)
        {
            if (instance != null)
            {
                if (instance.Id > 0)
                    instance.CopyFrom(GetEntity<Role>(instance.Id));
                else
                    instance.CopyFrom(GetEntity<Role>(instance.Name));
            }
            return instance;
        }

        /// <summary>
        /// Tries to obtain all objects created since a given timestamp and optionally can set a name filter
        /// </summary>
        /// <typeparam name="T">Type of object to be retrieved</typeparam>
        /// <param name="startingTS">Timestamp since which objects must have been created</param>
        /// <param name="nameFilter">optional name filter to be applied (%[nameFilter]%)</param>
        /// <returns></returns>
        public static Collection<T> GetObjectsCreatedSince<T>(DateTime startingTS, string nameFilter = null) where T : CoreBase
        {
            // set up return object
            Collection<T> returnObj = new Collection<T>();

            // prepare the get objects by filter
            GetObjectsByFilterInput input = new GetObjectsByFilterInput()
            {
                Type = Activator.CreateInstance<T>()
                ,
                Filter = new FilterCollection()
                {
                    new Filter() { Name = "CreatedOn", Operator = Cmf.Foundation.Common.FieldOperator.GreaterThanOrEqualTo, Value = startingTS }
                }
            };

            if (!String.IsNullOrWhiteSpace(nameFilter))
                input.Filter.Add(new Filter() { Name = "Name", Operator = Cmf.Foundation.Common.FieldOperator.Contains, Value = nameFilter });

            // invoke get objects by filter
            Collection<object> result = input.GetObjectsByFilterSync().Instance;

            if (result != null)
            {
                result.ToList().ForEach(E => returnObj.Add(E as T));
            }

            return returnObj;
        }

        #endregion Singular

        #region Collection

        /// <summary>
        /// Tries to load a given set of objects based on a filter collection
        /// </summary>
        /// <typeparam name="T">Type of the objects to be loaded</typeparam>
        /// <param name="filter">Filters to be applied</param>
        /// <param name="levelsToLoad">Load depth level</param>
        /// <returns>Collection of found objects of provided type and criteria</returns>
        public static Collection<T> GetEntities<T>(FilterCollection filter, int levelsToLoad = 0)
        {
            Collection<T> returnObject = null;

            Collection<object> res = new Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects.GetObjectsByFilterInput()
            {
                Filter = filter
                ,
                Type = Activator.CreateInstance<T>()
                ,
                LevelsToLoad = levelsToLoad
            }.GetObjectsByFilterSync().Instance;

            if (res != null)
            {
                returnObject = new Collection<T>();
                res.ToList().ForEach(E => returnObject.Add((T)E));
            }

            return returnObject;
        }

        /// <summary>
        /// Loads given set of objects in collection
        /// </summary>
        /// <typeparam name="T">type of the base instance to be retrieved.</typeparam>
        /// <param name="instances"></param>
        /// <param name="filters">Filter collection to be applied</param>
        /// <param name="levelsToLoad">Levels to load. Defaults to 0</param>
        public static void Load<T>(this List<T> instances, FilterCollection filters, Int32 levelsToLoad = 0) where T : EntityBase
        {
            if (instances != null && filters != null && filters.Count > 0)
            {
                // hold dictionaries for elements by their names and Ids if defined...
                Dictionary<Int64, T> incomingById = instances.Where(E => E.Id > 0).GroupBy(E => E.Id).ToDictionary(E => E.Key, E => E.First());
                Dictionary<String, T> incomingByName = instances.Where(E => !String.IsNullOrEmpty(E.Name)).GroupBy(E => E.Name).ToDictionary(E => E.Key, E => E.First());

                // get all objects
                T instance = instances.FirstOrDefault() ?? Activator.CreateInstance<T>();
                Collection<T> returnObjects = InternalCollectionLoadByFilters(instance, filters, levelsToLoad);

                // if any results retrieved, copy them to the original collection
                if (returnObjects != null && returnObjects.Count > 0)
                {
                    foreach (T returnObject in returnObjects)
                    {
                        if (incomingById.ContainsKey(returnObject.Id))
                            incomingById[returnObject.Id].CopyFrom(returnObject);
                        else if (incomingByName.ContainsKey(returnObject.Name))
                            incomingByName[returnObject.Name].CopyFrom(returnObject);
                        else instances.Add(returnObject);
                    }
                }
            }
        }

        /// <summary>
        /// Loads given set of objects in collection
        /// </summary>
        /// <param name="instances"></param>
        /// <param name="levelsToLoad">Levels to load. Defaults to 0</param>
        public static List<T> Load<T>(this List<T> instances, Int32 levelsToLoad = 0) where T : EntityBase
        {
            if (instances != null && instances.Count > 0)
            {
                // get distinct Ids, in case collection comes with different IDs
                Collection<Int64> objectIds = new Collection<Int64>(instances.Select(E => E.Id).Distinct().ToArray());

                // collect each index for Id (for the case there's more than one occurrence)
                Dictionary<Int64, Collection<Int32>> idIndexes = objectIds.ToDictionary(E => E, E => new Collection<Int32>());

                // ...collection each object Id per index... 
                Dictionary<Int32, Int64> indexId = instances.Select((o, i) => new { Index = i, Id = o.Id }).ToDictionary(E => E.Index, E => E.Id);
                foreach (var kvp in indexId)
                {
                    idIndexes[kvp.Value].Add(kvp.Key);
                }

                // get all objects
                var returnObjects = InternalCollectionLoad(instances.First(), objectIds, levelsToLoad);

                // if any results retrieved, copy them to the original collection
                if (returnObjects != null && returnObjects.Count > 0)
                {
                    foreach (var returnObject in returnObjects)
                    {
                        Int64 objId = returnObject.Id;
                        foreach (Int32 originalIndex in idIndexes[objId])
                        {
                            instances[originalIndex].CopyFrom(returnObject);
                        }
                    }
                }
            }

            return instances;
        }

        /// <summary>
        /// Loads given set of objects in collection
        /// </summary>
        /// <param name="instances"></param>
        /// <param name="idsToLoad"></param>
        /// <param name="levelsToLoad">Levels to load. Defaults to 0</param>
        public static List<T> Load<T>(this List<T> instances, Collection<Int64> idsToLoad, Int32 levelsToLoad = 0) where T : EntityBase
        {
            if (instances != null && idsToLoad.Count > 0)
            {
                // clear any existing entries
                instances.Clear();

                // get distinct Ids, in case collection comes with different IDs
                Collection<Int64> objectIds = new Collection<Int64>(idsToLoad.Distinct().ToArray());

                // get all objects
                T instance = instances.FirstOrDefault() ?? Activator.CreateInstance<T>();
                var returnObjects = InternalCollectionLoad(instance, objectIds, levelsToLoad);
                instances.AddRange(returnObjects);
            }

            return instances;
        }

        #endregion Collection

        #endregion Public Methods

        #endregion Load

        #region Attributes

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="attributeNames"></param>
        ///// <returns></returns>
        //public static T LoadAttributes<T>(this T instance, Collection<String> attributeNames) where T : EntityBase
        //{
        //    instance.CopyFrom(GenericGetsScenario.LoadAttributes<T>(instance, attributeNames));
        //    return instance;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="attributeNames"></param>
        ///// <returns></returns>
        //public static T LoadAttributes<T>(this T instance, params String[] attributeNames) where T : EntityBase
        //{
        //    Collection<String> attributes = new Collection<String>(attributeNames);
        //    return instance.LoadAttributes(attributes);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="attributeName"></param>
        ///// <returns></returns>
        //public static T LoadAttribute<T>(this T instance, String attributeName) where T : EntityBase
        //{
        //    instance.CopyFrom(GenericGetsScenario.LoadAttributes<T>(instance, new Collection<String>() { attributeName }));
        //    return instance;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <returns></returns>
        //public static T LoadAttributes<T>(this T instance) where T : EntityBase
        //{
        //    instance.CopyFrom(GenericGetsScenario.LoadAttributes<T>(instance));
        //    return instance;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="attributes"></param>
        ///// <returns></returns>
        //public static T SaveAttributes<T>(this T instance, AttributeCollection attributes) where T : EntityBase
        //{
        //    instance.CopyFrom(new UpdateObjectAttributesInput() { Object = instance, AttributeCollection = attributes }.UpdateObjectAttributesSync().Object as T);
        //    return instance;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="attributeName"></param>
        ///// <param name="attributeValue"></param>
        ///// <returns></returns>
        //public static T SaveAttribute<T>(this T instance, String attributeName, Object attributeValue) where T : EntityBase
        //{
        //    instance.SaveAttributes(new AttributeCollection() { { attributeName, attributeValue } });
        //    return instance;
        //}


        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static T SaveRelatedAttributes<T>(this T instance, AttributeCollection attributes) where T : EntityVersion
        {
            instance.CopyFrom(GenericServices.UpdateObjectAttributes<T>(instance, null, attributes));
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="attributeNames"></param>
        /// <returns></returns>
        public static T RemoveAttributes<T>(this T instance, Collection<String> attributeNames) where T : Entity
        {
            instance.CopyFrom(GenericServices.RemoveObjectAttributes<T>((T)instance, attributeNames, null));
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="attributeNames"></param>
        /// <returns></returns>
        public static T RemoveAttribute<T>(this T instance, String attributeNames) where T : Entity
        {
            RemoveAttributes<T>(instance, new Collection<String>() { attributeNames });
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="relatedAttributeNames"></param>
        /// <returns></returns>
        public static T RemoveRelatedAttributes<T>(this T instance, Collection<String> relatedAttributeNames = null) where T : EntityVersion
        {
            instance.CopyFrom(GenericServices.RemoveObjectAttributes<T>(instance, null, relatedAttributeNames));
            return instance;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="Y"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="attributeName"></param>
        ///// <param name="defaultValue"></param>
        ///// <returns></returns>
        //public static Y GetAttributeValue<T, Y>(this T instance, String attributeName, Y defaultValue) where T : EntityBase
        //{
        //    if (instance.HasAttributeDefined(attributeName))
        //    {
        //        //Object readValue = instance.Attributes[attributeName];
        //        //if (readValue is Y)
        //        //    return (Y)readValue;
        //        return (Y)instance.Attributes[attributeName];
        //    }

        //    return defaultValue;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="Y"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="attributeName"></param>
        ///// <param name="defaultValue"></param>
        ///// <returns></returns>
        //public static Y GetRelatedAttributeValue<T, Y>(this T instance, String attributeName, Y defaultValue) where T : EntityVersion
        //{
        //    if (instance.HasRelatedAttributeDefined(attributeName))
        //    {
        //        return (Y)instance.RelatedAttributes[attributeName];
        //    }

        //    //if (instance.RelatedAttributes != null && instance.RelatedAttributes.ContainsKey(attributeName))
        //    //{
        //    //    Object readValue = instance.RelatedAttributes[attributeName];
        //    //    if (readValue is Y)
        //    //        return (Y)readValue;
        //    //}

        //    return defaultValue;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Y"></typeparam>
        /// <param name="instance"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static Y GetAttributeValue<T, Y>(this T instance, String attributeName) where T : EntityBase
        {
            if (instance.Attributes != null && instance.Attributes.ContainsKey(attributeName))
            {
                Object readValue = instance.Attributes[attributeName];
                if (readValue is Y)
                    return (Y)readValue;
            }

            return default(Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Y"></typeparam>
        /// <param name="instance"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static Boolean AttributeEquals<T, Y>(this T instance, String attributeName, Y attributeValue) where T : EntityBase
        {
            if (instance.Attributes != null
                    && instance.Attributes.ContainsKey(attributeName)
                    && instance.Attributes[attributeName] is Y
            )
            {
                Y attValue = (Y)instance.Attributes[attributeName];
                return Object.Equals((Object)attValue, (Object)attributeValue);
            }

            try
            {
                return (attributeValue == null);
            }
            catch { }

            return false;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="Y"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="attributeName"></param>
        ///// <param name="attributeValue"></param>
        ///// <returns></returns>
        //public static Boolean RelatedAttributeEquals<T, Y>(this T instance, String attributeName, Y attributeValue) where T : EntityVersion
        //{
        //    if (instance.HasRelatedAttributeDefined(attributeName))
        //    {
        //        Y attValue = (Y)instance.RelatedAttributes[attributeName];
        //        return Object.Equals((Object)attValue, (Object)attributeValue);
        //    }

        //    try
        //    {
        //        return (attributeValue == null);
        //    }
        //    catch { }
        //    return false;
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="collectionInstance"></param>
        ///// <param name="attributeNames"></param>
        //public static void LoadCollectionAttributes<T>(this List<T> collectionInstance, params String[] attributeNames) where T : EntityBase
        //{
        //    Collection<String> attributesToLoad = new Collection<String>(attributeNames);
        //    collectionInstance.LoadCollectionAttributes(attributesToLoad);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="collectionInstance"></param>
        ///// <param name="attributeNames"></param>
        //public static void LoadCollectionAttributes<T>(this List<T> collectionInstance, Collection<String> attributeNames) where T : EntityBase
        //{
        //    Collection<Object> objectsToLoad = new Collection<Object>();
        //    foreach (T instance in collectionInstance)
        //    {
        //        instance.LoadAttributes(attributeNames);
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="attributeName"></param>
        ///// <returns></returns>
        //public static Boolean HasAttributeDefined<T>(this T instance, String attributeName) where T : EntityBase
        //{
        //    if (instance.Attributes == null || !instance.Attributes.ContainsKey(attributeName))
        //        instance.LoadAttribute(attributeName);

        //    if (instance.Attributes != null)
        //        return instance.Attributes.ContainsKey(attributeName);

        //    return false;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="attributeName"></param>
        ///// <returns></returns>
        //public static Boolean HasRelatedAttributeDefined<T>(this T instance, String attributeName) where T : EntityVersion
        //{
        //    if (instance.RelatedAttributes == null || !instance.RelatedAttributes.ContainsKey(attributeName))
        //        instance.LoadAttribute(attributeName);

        //    if (instance.RelatedAttributes != null)
        //        return instance.RelatedAttributes.ContainsKey(attributeName);

        //    return false;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="attributeNames"></param>
        ///// <returns></returns>
        //public static Boolean HasRelatedAttributesDefined<T>(this T instance, params String[] attributeNames) where T : EntityVersion
        //{
        //    Int32 attCount = attributeNames == null ? 0 : attributeNames.Count();
        //    if (attCount > 0)
        //    {
        //        if (instance.RelatedAttributes == null || instance.RelatedAttributes.Select(E => E.Key).Intersect(attributeNames).Count() != attCount)
        //        {
        //            instance.LoadAttributes(attributeNames);
        //        }

        //        if (instance.RelatedAttributes != null && instance.RelatedAttributes.Select(E => E.Key).Intersect(attributeNames).Count() == attCount)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instances"></param>
        ///// <param name="attributeName"></param>
        ///// <param name="attributeValue"></param>
        //public static void SaveCollectionAttribute<T>(this List<T> instances, string attributeName, object attributeValue) where T : EntityInstance
        //{
        //    foreach (T instance in instances)
        //    {
        //        instance.SaveAttribute(attributeName, attributeValue);
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instances"></param>
        ///// <param name="attributeCollection"></param>
        //public static void SaveCollectionAttributes<T>(this List<T> instances, AttributeCollection attributeCollection) where T : EntityInstance
        //{
        //    foreach (T instance in instances)
        //    {
        //        instance.SaveAttributes(attributeCollection);
        //    }
        //}

        #endregion

        #region (Un)Terminate

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        //public static void Terminate<T>(this T instance, String serviceComments = null, Reason reasonToUse = null, LossReasonClassification classifications = null) where T : EntityBase
        //{
        //    if (instance is Material)
        //    {
        //        (instance as Material).TerminateMaterial(reasonToUse, classifications, serviceComments);
        //    }
        //    else
        //    {
        //        instance.CopyFrom(new TerminateObjectInput() { Object = instance, ServiceComments = serviceComments }.TerminateObjectSync().Object as T);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="serviceComments"></param>
        /// <param name="reasonToUse"></param>
        /// <param name="classifications"></param>
        public static void SpecialTerminate<T>(this T instance, String serviceComments = null, Reason reasonToUse = null, LossReasonClassification classifications = null) where T : EntityBase
        {
            if (instance != null && instance.Exists(true) && instance.UniversalState != UniversalState.Terminated)
                instance.CopyFrom(new TerminateObjectInput() { Object = instance, ServiceComments = serviceComments }.TerminateObjectSync().Object as T);
        }

        /// <summary>
        /// Unterminate an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="serviceComments"></param>
        public static void Unterminate<T>(this T instance, String serviceComments = null) where T : EntityBase
        {
            instance.CopyFrom(new UnterminateObjectInput() { Object = instance, ServiceComments = serviceComments }.UnterminateObjectSync().Object as T);
        }

        #endregion

        #region Relations

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="levelsToLoad"></param>
        //public static void LoadRelations<T>(this T instance, Int32 levelsToLoad = 0) where T : EntityBase
        //{
        //    instance.CopyFrom(GenericGetsScenario.LoadRelations<T>(instance, null, levelsToLoad));
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="relationNames"></param>
        ///// <param name="levelsToLoad"></param>
        //public static void LoadRelations<T>(this T instance, Collection<String> relationNames, Int32 levelsToLoad = 0) where T : EntityBase
        //{
        //    instance.CopyFrom(GenericGetsScenario.LoadRelations<T>(instance, relationNames, levelsToLoad));
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="relationName"></param>
        ///// <param name="levelsToLoad"></param>
        //public static void LoadRelation<T>(this T instance, String relationName, Int32 levelsToLoad = 0) where T : EntityBase
        //{
        //    instance.LoadRelations<T>(new Collection<String>() { relationName }, levelsToLoad);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="relationName"></param>
        ///// <returns></returns>
        //public static Boolean HasRelation<T>(this T instance, String relationName) where T : Entity
        //{
        //    Boolean returnVeridict = false;

        //    if (instance.RelationCollection == null)
        //        instance.LoadRelation(relationName);

        //    returnVeridict = (instance.RelationCollection != null
        //                        && instance.RelationCollection.ContainsKey(relationName)
        //                        && instance.RelationCollection[relationName].Count > 0);

        //    return returnVeridict;
        //}

        /// <summary>
        /// Adds relations to an instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="relationsToAdd"></param>
        public static void AddRelations<T>(this T instance, EntityRelationCollection relationsToAdd) where T : Entity
        {
            if (instance != null)
            {
                instance.CopyFrom(GenericServices.AddObjectRelations<T>(instance, relationsToAdd, null));
            }
        }

        /// <summary>
        /// Removes relations from an object instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="relationsToRemove"></param>
        public static void RemoveRelations<T>(this T instance, EntityRelationCollection relationsToRemove) where T : Entity
        {
            if (instance != null)
            {
                instance.CopyFrom(GenericServices.RemoveObjectRelations<T>(instance, relationsToRemove, null));
            }
        }

        #endregion

        #region Save

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        ///// <param name="attributesToAddOrUpdate"></param>
        ///// <param name="attributesToRemove"></param>
        ///// <param name="ignoreLastServiceHistoryId"></param>
        //public static void Save<T>(this T instance, AttributeCollection attributesToAddOrUpdate = null, Collection<String> attributesToRemove = null, bool ignoreLastServiceHistoryId = false) where T : Entity
        //{
        //    if (instance is Step)
        //    {
        //        (instance as Step).FullUpdate(attributesToAddOrUpdate: attributesToAddOrUpdate, attributesToRemove: attributesToRemove, ignoreLastServiceHistoryId: ignoreLastServiceHistoryId);
        //    }
        //    else
        //    {

        //        instance.CopyFrom<T, T>(new FullUpdateObjectInput
        //        {
        //            Object = instance
        //            ,
        //            FullUpdateParameters = new Cmf.Foundation.BusinessOrchestration.FullUpdateParameters
        //            {
        //                AttributesToAddOrUpdate = attributesToAddOrUpdate
        //                ,
        //                AttributesNamesToRemove = attributesToRemove
        //            }
        //            ,
        //            IgnoreLastServiceId = ignoreLastServiceHistoryId
        //        }.FullUpdateObjectSync().Object as T);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="attributesToAddOrUpdate"></param>
        /// <param name="attributesToRemove"></param>
        /// <param name="relationsToAdd"></param>
        /// <param name="relationsToRemove"></param>
        /// <param name="relationsToUpdate"></param>
        /// <param name="ignoreLastServiceHistoryId"></param>
        public static void FullUpdate(this Step instance, AttributeCollection attributesToAddOrUpdate = null, Collection<String> attributesToRemove = null
                , EntityRelationCollection relationsToAdd = null, EntityRelationCollection relationsToUpdate = null, EntityRelationCollection relationsToRemove = null
                , bool ignoreLastServiceHistoryId = false)
        {
            instance.CopyFrom(new FullUpdateStepInput()
            {
                Step = instance
                ,
                AttributesToAddOrUpdate = attributesToAddOrUpdate
                ,
                AttributesNamesToRemove = attributesToRemove
                ,
                RelationsToAdd = relationsToAdd
                ,
                RelationsToUpdate = relationsToUpdate
                ,
                RelationsToRemove = relationsToRemove
                ,
                IgnoreLastServiceId = ignoreLastServiceHistoryId
            }.FullUpdateStepSync().Step);
        }

        #endregion

        #region Create

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        //public static void Create<T>(this T instance) where T : Entity
        //{
        //    if (instance is Resource)
        //    {
        //        instance.CopyFrom(new Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects.CreateResourceInput() { Resource = instance as Resource }.CreateResourceSync().Resource);
        //    }
        //    else if (instance is Material)
        //    {
        //        instance.CopyFrom(new CreateMaterialInput() { Material = instance as Material }.CreateMaterialSync().Material);
        //    }
        //    else
        //    {
        //        instance.CopyFrom<T, T>(GenericServices.CreateObject<T>(instance));
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance"></param>
        //public static void CreateVersion<T>(this T instance, bool createNewChangeSet = true, bool approveChangeSet = true, bool makeEffectiveOnApproval = true) where T : EntityVersion
        //{
        //    if (instance != null)
        //    {
        //        bool isNewDefinition = !instance.Exists();

        //        if (instance.ChangeSet == null && createNewChangeSet)
        //        {
        //            instance.ChangeSet = new ChangeSet();
        //            instance.ChangeSet.CreateChangeSet(makeEffectiveOnApproval);
        //        }

        //        instance.CopyFrom<T, T>(GenericServices.CreateObjectVersion<T>(instance, isNewDefinition, null));

        //        if (approveChangeSet)
        //        {
        //            instance.ChangeSet.RequestApproval();
        //        }

        //        instance.Load();
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="services"></param>
        /// <param name="serviceComments"></param>
        public static void AddServices(this Resource instance, ServiceCollection services, String serviceComments = null)
        {
            if (services != null && services.Count > 0)
            {
                instance.SpecialLoad();
                foreach (Service service in services)
                {
                    service.SpecialLoad();
                }

                ResourceServiceCollection relationsToAdd = new ResourceServiceCollection();
                relationsToAdd.AddRange(
                        services.Select(E => new ResourceService()
                        {
                            IsEnabled = true
                            ,
                            TargetEntity = E
                            ,
                            SourceEntity = instance
                            ,
                            IsSource = true
                        }));

                instance.CopyFrom(new AddResourceServicesInput()
                {
                    Resource = instance
                    ,
                    ResourceServices = relationsToAdd
                    ,
                    ServiceComments = serviceComments
                }.AddResourceServicesSync().Resource);
            }
        }

        /// <summary>
        /// Creates n objects of the same type in collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionInstance"></param>
        /// <param name="levelsToLoad"></param>
        public static void CollectionCreate<T>(this List<T> collectionInstance, int levelsToLoad = 0) where T : EntityBase
        {
            if (collectionInstance != null && collectionInstance.Count > 0)
            {
                Collection<object> createdObjects = new CreateObjectsInput() { Objects = new Collection<object>(collectionInstance.ToList<object>()) }.CreateObjectsSync().Objects;
                if (createdObjects != null && createdObjects.Count == collectionInstance.Count)
                {
                    for (int iPos = 0; iPos < createdObjects.Count; iPos++)
                    {
                        collectionInstance[iPos].CopyFrom((T)createdObjects[iPos]);
                    }
                }
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Checks if object exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool Exists<T>(this T instance, bool loadIfExisting = false) where T : EntityBase
        {
            bool returnVeridict = false;

            if (instance != null)
            {
                try
                {
                    T res;
                    if (instance.Id > 0)
                        res = GetEntity<T>(instance.Id);
                    else
                        res = GetEntity<T>(instance.Name);

                    if (loadIfExisting)
                        instance.CopyFrom(res);

                    returnVeridict = true;
                }
                catch { }
            }

            return returnVeridict;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="instance"></param>
        ///// <returns></returns>
        //public static void CreateChangeSet(this ChangeSet instance, bool makeEffectiveOnApproval = true)
        //{
        //    if (instance != null)
        //    {
        //        instance.Name = String.IsNullOrEmpty(instance.Name) ? $"ChangeSet {TestUtilities.GenerateHexGuid()}" : instance.Name;
        //        instance.Type = String.IsNullOrEmpty(instance.Type) ? $"General" : instance.Type;
        //        instance.Description = String.IsNullOrEmpty(instance.Description) ? "Changeset generated in Generalization Utilities" : instance.Description;
        //        instance.MakeEffectiveOnApproval = makeEffectiveOnApproval;

        //        instance.CopyFrom(new CreateChangeSetInput
        //        {
        //            ChangeSet = instance
        //        }.CreateChangeSetSync().ChangeSet);
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="instance"></param>
        //public static RequestChangeSetApprovalOutput RequestApproval(this ChangeSet instance)
        //{
        //    RequestChangeSetApprovalOutput output = null;

        //    if (instance != null)
        //    {
        //        instance.Load();

        //        output = new RequestChangeSetApprovalInput()
        //        {
        //            ChangeSet = instance
        //        }.RequestChangeSetApprovalSync();

        //        instance.CopyFrom(output.ChangeSet);
        //    }

        //    return output;
        //}

        ///// <summary>
        ///// Set Object Version Effective
        ///// </summary>
        ///// <typeparam name="T">EntityVersion</typeparam>
        ///// <param name="instance"></param>
        //public static SetObjectVersionEffectiveOutput SetObjectVersionEffective<T>(this T instance) where T : EntityVersion
        //{
        //    SetObjectVersionEffectiveOutput output = null;

        //    if (instance != null)
        //    {
        //        output = new SetObjectVersionEffectiveInput()
        //        {
        //            Object = instance
        //        }.SetObjectVersionEffectiveSync();

        //        instance.CopyFrom<T, T>((T)output.Object);

        //        instance.Load();
        //    }

        //    return output;
        //}

        #endregion

        #region Reflection Utilities

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetInstance"></param>
        /// <param name="sourceInstance"></param>
        /// <param name="excludeServiceProperties"></param>
        public static void CopyFrom<T, S>(this T targetInstance, S sourceInstance, bool excludeServiceProperties = false) where T : CoreBase
        {
            if (targetInstance != null && sourceInstance != null && targetInstance is S)
            {
                foreach (PropertyInfo piInfo in targetInstance.GetType().GetRuntimeProperties())
                {
                    if (piInfo != null && piInfo.CanWrite)
                    {
                        if (!(excludeServiceProperties && _ServiceProperties.Contains(piInfo.Name)))
                        {
                            piInfo.SetValue(targetInstance, piInfo.GetValue(sourceInstance));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates and returns a copy for a given object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static T GetCopy<T>(this T instance) where T : CoreBase
        {
            T instanceCopy = null;
            if (instance != null)
            {
                instanceCopy = Activator.CreateInstance(instance.GetType()) as T;
                instanceCopy.CopyFrom(instance);
            }

            return instanceCopy;
        }

        #endregion

        #region Framework

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparisonValue"></param>
        /// <returns></returns>
        public static bool InvariantEquals(this string value, string comparisonValue)
        {
            return String.Equals(value, comparisonValue, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Custom


        #endregion

        #region Roles

        /// <summary>
        /// Update Role
        /// </summary>
        /// <param name="role">Role to be updated</param>
        public static void Update(this Role role)
        {
            UpdateRoleAccessModeToReportsAndDataGroupsOutput updateRoleInput = new UpdateRoleAccessModeToReportsAndDataGroupsInput()
            {
                Role = role
            }.UpdateRoleAccessModeToReportsAndDataGroupsSync();
        }

        #endregion

        #region Object Exists

        /// <summary>
        /// Checks if a object already exists with the same name in Database.
        /// </summary>
        /// <param name="instance">EntityBase Object</param>
        /// <returns>True if the object already exists</returns>
        public static bool ObjectExistsByName<T>(this T instance) where T : EntityBase
        {
            bool entityExists = false;

            string type = typeof(T).Name;

            QueryObject query = new QueryObject();
            query.Description = "";
            query.EntityTypeName = type;
            query.Name = "ObjectExists";
            query.Query = new Query();
            query.Query.Distinct = false;
            query.Query.Filters = new FilterCollection() {
                new Filter()
                {
                    Name = "Name",
                    ObjectName = type,
                    ObjectAlias = type + "_1",
                    Operator = Cmf.Foundation.Common.FieldOperator.IsEqualTo,
                    Value = instance.Name,
                    LogicalOperator = Cmf.Foundation.Common.LogicalOperator.Nothing,
                    FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
                }
            };

            query.Query.Fields = new FieldCollection() {
                new Field()
                {
                    Alias = "Id",
                    ObjectName = type,
                    ObjectAlias = type + "_1",
                    IsUserAttribute = false,
                    Name = "Id",
                    Position = 0,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort
                },
                new Field()
                {
                    Alias = "Name",
                    ObjectName = type,
                    ObjectAlias = type + "_1",
                    IsUserAttribute = false,
                    Name = "Name",
                    Position = 1,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort
                }
            };

            query.Query.Relations = new RelationCollection();
            var executeInput = new ExecuteQueryInput();
            executeInput.QueryObject = query;
            ExecuteQueryOutput executeOutput = executeInput.ExecuteQuerySync();

            DataSet ds = Cmf.TestScenarios.Others.Utilities.ToDataSet(executeOutput.NgpDataSet);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                entityExists = true;
            }

            return entityExists;
        }

        #endregion

    }
}
