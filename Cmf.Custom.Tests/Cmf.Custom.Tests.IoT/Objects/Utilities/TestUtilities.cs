using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.OutputObjects;
using Cmf.Foundation.Security;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.DispatchManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.LaborManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.OrderManagement.InputObjects;
using Cmf.TestScenarios;
using AMSOsramEIAutomaticTests.Objects.Extensions;
using AMSOsramEIAutomaticTests.Objects.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using TestScenariosUtil = Cmf.TestScenarios.Others;
using Cmf.Custom.TestUtilities;

namespace AMSOsramEIAutomaticTests.Objects.TestUtilities
{
	public static class TestUtilities
    {
        #region Production Order

        //public static ProductionOrder CreateProductionOrder(string name = null, decimal? quantity = null,
        //    int priority = 1, string facilityName = null, string productName = null, DateTime? startTime = null,
        //    DateTime? endTime = null, TestTeardownManager tearDownManager = null, bool MesOnly = false,
        //    bool releasePO = false, string flowPath = null)
        //{
        //    string defaultPOName = "TestPO_" + TestScenariosUtil.Utilities.NewGuid();
        //    const string defaultPOFacility = Constants.DefaultFacility;
        //    const string defaultPOProduct = Constants.DefaultProduct;
        //    const decimal defaultQuantity = 100;

        //    string prodOrderName = string.IsNullOrEmpty(name) ? defaultPOName : name;
        //    string prodOrderFacility = string.IsNullOrEmpty(facilityName) ? defaultPOFacility : facilityName;
        //    string prodOrderProduct = string.IsNullOrEmpty(productName) ? defaultPOProduct : productName;
        //    decimal prodOrderQuantity = quantity == null ? defaultQuantity : (decimal)quantity;
        //    string ProductERPIdentifier = string.Empty;

           

        //    Facility facility = SystemUtilities.GetObjectByName<Facility>(prodOrderFacility);
        //    Product product = SystemUtilities.GetObjectByName<Product>(prodOrderProduct);


        //    ProductionOrder prodOrder = new ProductionOrder
        //    {
        //    //    Name = prodOrderName,
        //    //    Type = Constants.DefaultTypeStandard,
        //    //    OrderNumber = TestScenariosUtil.Utilities.NewGuid(),
        //    //    Quantity = prodOrderQuantity,
        //    //    Units = Constants.DefaultUnits,
        //    //    Product = product,
        //    //    OverDeliveryTolerance = 0,
        //    //    PlannedStartDate = startTime ?? DateTime.Now,
        //    //    PlannedEndDate = endTime ?? DateTime.Now.AddDays(1),
        //    //    DueDate = endTime ?? DateTime.Now.AddDays(1),
        //    //    Priority = priority,
        //    //    Facility = facility,
        //    //    SystemState = (MesOnly && !releasePO) ? ProductionOrderSystemState.Created : ProductionOrderSystemState.Released,
        //    //    ValidateMaterialProducts = false,
        //    //    RestrictOnComplete = false,
        //    //    Attributes = (!MesOnly) ? new AttributeCollection() : null,
        //    //    FlowPath = flowPath ?? product.FlowPath,
        //    };

        //    SystemUtilities.CreateObject<ProductionOrder>(prodOrder);

        //    if (tearDownManager != null)
        //        tearDownManager.Push(prodOrder);

        //    return prodOrder;
        //}

        #endregion



        public static void LoadMaterials(ProductionOrder po)
        {
            new LoadMaterialsForProductionOrdersInput
            {
                ProductionOrders = new ProductionOrderCollection { po }
            }.LoadMaterialsForProductionOrdersSync().ProductionOrders.First();
        }


        #region Resource
        /// <summary>
        /// Get a Resource that can be used to dispatch a material
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static Resource GetResourceForDispatch(Material instance, string preferredResourceName = "")
        {
            ResourceCollection resources = null;
            Resource resource = null;

            resources = GetDispatchList(instance);

            if (resources != null)
            {
                resource.Load();
                if (!string.IsNullOrEmpty(preferredResourceName))
                {
                    resource = resources.FirstOrDefault(R => R.Name.Equals(preferredResourceName));
                }

                if (resource == null)
                    resource = resources.FirstOrDefault();
            }
            return resource;
        }


        /// <summary>
        /// Retrieves list of possible resources for dispatch of current material instance
        /// </summary>
        /// <param name="instance">Material instance for which resource dispatch list will be provided...</param>
        /// <param name="dispatchType">Type of dispatch to be made. Defaults to ProcessingType = Process</param>
        /// <returns>Resource Collection of possible resources for dispatch</returns>
        public static ResourceCollection GetDispatchList(Material instance, ProcessingType dispatchType = ProcessingType.Process)
        {
            ResourceCollection possibleResources = null;

            // only proceed if instance is set.
            if (instance != null)
            {
                // invoke service and assign result to return variable
                possibleResources = new GetDispatchListForMaterialInput()
                {
                    Material = instance
                    ,
                    DispatchType = ProcessingType.Process
                }.GetDispatchListForMaterialSync().Resources;

            }

            return possibleResources;
        }

        /// <summary>
        /// Get user logged in into the system
        /// </summary>
        /// <returns></returns>
        public static Employee GetEmployeeLoggedIn()
        {
            User user = TestScenariosUtil.SecurityScenario.GetUser(Constants.UserName);

            Employee employee = new GetEmployeeByUserAccountInput()
            {
                UserAccount = user.UserAccount
            }.GetEmployeeByUserAccountSync().Employee;

            return employee;
        }

        /// <summary>
        /// Get resource dispatched and in process list
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>MaterialCollection</returns>
        public static MaterialCollection GetResourceDispatchedAndInProcessList(string resourceName)
        {
            MaterialCollection materials = new MaterialCollection();
            QueryObject query = new QueryObject();
            query.Description = "";
            query.EntityTypeName = "Resource";
            query.Name = "sdgdsg";
            query.Query = new Query();
            query.Query.Distinct = false;
            query.Query.Filters = new FilterCollection() {
                new Filter()
                {
                    Name = "Name",
                    ObjectName = "Resource",
                    ObjectAlias = "Resource_1",
                    Operator = Cmf.Foundation.Common.FieldOperator.IsEqualTo,
                    Value = resourceName,
                    LogicalOperator = Cmf.Foundation.Common.LogicalOperator.AND,
                    FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
                },
                new Filter()
                {
                    ObjectName = "Material",
                    ObjectAlias = "Resource_MaterialResource_SourceEntity_3",
                    Value = null,
                    LogicalOperator = Cmf.Foundation.Common.LogicalOperator.AND,
                    FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.AlwaysTrue,
                },
                new Filter()
                {
                    LogicalOperator = Cmf.Foundation.Common.LogicalOperator.AND,
                    FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.AlwaysTrue,
                    InnerFilter = new FilterCollection() {
                        new Filter()
                        {
                            Name = "SystemState",
                            ObjectName = "Material",
                            ObjectAlias = "Resource_MaterialResource_SourceEntity_3",
                            Operator = Cmf.Foundation.Common.FieldOperator.IsEqualTo,
                            Value = 1,
                            LogicalOperator = Cmf.Foundation.Common.LogicalOperator.OR,
                            FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
                        },
                        new Filter()
                        {
                            Name = "SystemState",
                            ObjectName = "Material",
                            ObjectAlias = "Resource_MaterialResource_SourceEntity_3",
                            Operator = Cmf.Foundation.Common.FieldOperator.IsEqualTo,
                            Value = 2,
                            LogicalOperator = Cmf.Foundation.Common.LogicalOperator.Nothing,
                            FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
                        }
                    }
                }
            };
            query.Query.Fields = new FieldCollection() {
                new Field()
                {
                    Alias = "MaterialId",
                    ObjectName = "Material",
                    ObjectAlias = "Resource_MaterialResource_SourceEntity_3",
                    IsUserAttribute = false,
                    Name = "Id",
                    Position = 3,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort,
                    DisplayStyleName = "MaterialId"
                },
                new Field()
                {
                    Alias = "MaterialName",
                    ObjectName = "Material",
                    ObjectAlias = "Resource_MaterialResource_SourceEntity_3",
                    IsUserAttribute = false,
                    Name = "Name",
                    Position = 4,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort,
                    DisplayStyleName = "MaterialName"
                },
                new Field()
                {
                    Alias = "MaterialResourceOrder",
                    ObjectName = "MaterialResource",
                    ObjectAlias = "Resource_MaterialResource_2",
                    IsUserAttribute = false,
                    Name = "Order",
                    Position = 2,
                    Sort = Cmf.Foundation.Common.FieldSort.Ascending
                },
                new Field()
                {
                    Alias = "Id",
                    ObjectName = "Resource",
                    ObjectAlias = "Resource_1",
                    IsUserAttribute = false,
                    Name = "Id",
                    Position = 0,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort
                },
                new Field()
                {
                    Alias = "Name",
                    ObjectName = "Resource",
                    ObjectAlias = "Resource_1",
                    IsUserAttribute = false,
                    Name = "Name",
                    Position = 1,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort
                }
            };
            query.Query.Relations = new RelationCollection() {
                new Relation()
                {
                    Alias = "Resource_MaterialResource_2",
                    IsRelation = true,
                    Name = "MaterialResource",
                    SourceEntity = "Material",
                    SourceEntityAlias = "Resource_MaterialResource_SourceEntity_3",
                    SourceJoinType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
                    SourceProperty = "Id",
                    TargetEntity = "Resource",
                    TargetEntityAlias = "Resource_1",
                    TargetJoinType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
                    TargetProperty = "Id"
                }
            };

            var executeInput = new ExecuteQueryInput();
            executeInput.QueryObject = query;
            ExecuteQueryOutput executeOutput = executeInput.ExecuteQuerySync();

            DataSet ds = TestScenariosUtil.Utilities.ToDataSet(executeOutput.NgpDataSet);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    long id = (long)ds.Tables[0].Rows[i]["MaterialId"];
                    string name = (string)ds.Tables[0].Rows[i]["MaterialName"];

                    Material material = new Material()
                    {
                        Id = id,
                        Name = name
                    };

                    materials.Add(material);
                }
            }

            return materials;
        }

            #endregion

            #region Material

        //    /// <summary>
        //    /// Create consumable materials and attach to resource
        //    /// </summary>
        //    /// <param name="consumableFeedFlowPath"></param>
        //    /// <param name="resource"></param>
        //    /// <param name="isToCreateMaterial"></param>
        //    /// <param name="materialCollection"></param>
        //    /// <param name="subResourceName"></param>
        //    /// <param name="quantity"></param>
        //    /// <param name="productName"></param>
        //    /// <param name="consumableType"></param>
        //    /// <returns></returns>
        //    public static MaterialCollection CreateAndAttachConsumables(string consumableFeedFlowPath = null,
        //                                                            Resource resource = null,
        //                                                            bool isToCreateMaterial = true,
        //                                                            MaterialCollection materialCollection = null,
        //                                                            string subResourceName = Constants.SubResourceMainFeeder,
        //                                                            int quantity = 100,
        //                                                            string productName = Constants.ProductPlainBoardConsumableFeed,
        //                                                            string consumableType = "Good",
        //                                                            string units = null)
        //{
        //    if (isToCreateMaterial && materialCollection == null)
        //    {
        //        if (consumableFeedFlowPath == null)
        //            consumableFeedFlowPath = Constants.InFeederFlow;

        //        materialCollection = new MaterialCollection
        //        {
        //            TestUtilities.CreateMaterial(name: "TestAttachedMaterial_" + TestScenariosUtil.Utilities.NewGuid(),
        //                                            flowPath: consumableFeedFlowPath,
        //                                            form: Constants.DefaultMaterialFormPallet,
        //                                            productName: productName,
        //                                            primaryQuantity:quantity,
        //                                            secondaryQuantity: 0,
        //                                            type: consumableType,
        //                                            primaryUnits: units)
        //        };
        //    }

        //    // Find Sub Resource
        //    Resource subResource = new Resource();
        //    subResource.Load(subResourceName);

        //    // Consumable feed
        //    Dictionary<Resource, MaterialCollection> dictionary = new Dictionary<Resource, MaterialCollection>
        //    {
        //        { subResource, materialCollection }
        //    };

        //    // Attach Consumable feed
        //    ManageResourceConsumableFeedsInput manageResourceConsumableFeedsInput = new ManageResourceConsumableFeedsInput()
        //    {
        //        ConsumablesToAttach = dictionary,
        //        Resource = resource
        //    };
        //    manageResourceConsumableFeedsInput.ManageResourceConsumableFeedsSync();

        //    return materialCollection;
        //}

        /// <summary>
        /// Create New Material
        /// </summary>
        /// <param name="name">Material Name</param>
        /// <param name="type">Material Type</param>
        /// <param name="form">Material Form</param>
        /// <param name="facilityName">Facility Name</param>
        /// <param name="productName">Product Name</param>
        /// <param name="flowPath">Flow Path</param>
        /// <param name="primaryQuantity">Primary Quantity</param>
        /// <param name="withDataGroup">With Data Group?</param>
        /// <param name="prodOrder">Production Order</param>
        /// <param name="tearDownManager">Test Tear Down Manager</param>
        /// <param name="expiraDate">Expiration Date</param>
        /// <returns>New Material</returns>
        public static Material CreateMaterial(string name = null, string type = null, string form = null, string facilityName = null,
            string productName = null, string flowPath = null, decimal? primaryQuantity = null, bool withDataGroup = false, ProductionOrder prodOrder = null,
            Cmf.TestScenarios.TestTeardownManager tearDownManager = null, DateTime? expiraDate = null, string primaryUnits = null, decimal? secondaryQuantity = null, string secondaryUnits = null, Resource resource = null)
        {
            //string defaultMaterialName = "TestMaterial_" + TestScenariosUtil.Utilities.NewGuid();
    
            //const decimal defaultPrimaryQuantity = 1;
            //const decimal defaultSecondaryQuantity = 1;

            //string matName = string.IsNullOrEmpty(name) ? defaultMaterialName : name;
            //string matType = string.IsNullOrEmpty(type) ? defaultMaterialType : type;
            //string matForm = string.IsNullOrEmpty(form) ? defaultMaterialForm : form;
            //string matFacility = string.IsNullOrEmpty(facilityName) ? defaultMaterialFacility : facilityName;
            //string matProduct = string.IsNullOrEmpty(productName) ? defaultMaterialProduct : productName;
            //string matFlowPath = string.IsNullOrEmpty(flowPath) ? defaultMaterialFlowPath : flowPath;
            //decimal matPrimaryQuantity = primaryQuantity == null ? defaultPrimaryQuantity : (decimal)primaryQuantity;
            //decimal matSecondaryQuantity = secondaryQuantity == null ? defaultSecondaryQuantity : (decimal)secondaryQuantity;
            //string matPrimaryUnits = string.IsNullOrEmpty(primaryUnits) ? defaultMaterialPrimaryUnits : primaryUnits;
            //string matSecondaryUnits = string.IsNullOrEmpty(secondaryUnits) ? defaultMaterialSecondaryUnits : secondaryUnits;

            //Facility facility = TestScenariosUtil.GenericGetsScenario.GetObjectByName<Facility>(matFacility);
            //Product product = TestScenariosUtil.GenericGetsScenario.GetObjectByName<Product>(matProduct);

            Material material = new Material
            {
                //Name = matName,
                //Type = matType,
                //Form = matForm,
                //Facility = facility,
                //Product = product,
                //FlowPath = matFlowPath,
                //PrimaryQuantity = matPrimaryQuantity,
                //ProductionOrder = prodOrder,
                //ExpirationDate = expiraDate,
                //PrimaryUnits = matPrimaryUnits,
                //SecondaryQuantity = matSecondaryQuantity,
                //SecondaryUnits = matSecondaryUnits
            };

            material.Create();

            if (tearDownManager != null)
                tearDownManager.Push(material);

            return material;
        }

        public static void TerminateMaterialsOnResource(string resourceName)
        {
           // Resource resource = new Resource() { Name = resourceName };
            MaterialCollection materials = GetResourceDispatchedAndInProcessList(resourceName);
            if (materials.Count > 0)
            {
                foreach (Material mat in materials)
                {
                    mat.Load();
                    mat.Terminate();
                }
            }

        }

        /// <summary>
        /// Validate if a material is set up correctly
        /// </summary>
        /// <param name="materials">collection of materials to validate</param>
        /// <returns>returns a dictionary with the material and a boolean that says if the material is set up correctly</returns>
//        public static Dictionary<Material, bool> IsOrderSetup(MaterialCollection materials)
//        {
//            Dictionary<Material, bool> dictionaryToReturn = new Dictionary<Material, bool>();
//​
//            foreach (var material in materials)
//            {
                
//                material.Load();
//                bool correctlySetup = true;
//                if (material.GetNativeValue<long>("CurrentChecklistInstance") > 0 && material.CurrentChecklistInstance.UniversalState == UniversalState.Active)
//                {
//                    correctlySetup = false;
//                }
//                else
//                {
//                    CustomSetupConsumptionDetailCollection customSetupDetails = SetupConsumptionUtility(material);
//                    correctlySetup = !customSetupDetails.Where(x => x.ExpectedProduct == null || x.ExpectedProduct.Id != x.CurrentlyAttachedProduct?.Id).Any();
//                }
//​
//                dictionaryToReturn.Add(material, correctlySetup);
//​
//            }
//            return dictionaryToReturn;
//        }

        /// <summary>
        /// RetrieveS mBOM and dBOM Actual vs Planned Products
        /// </summary>
        /// <param name="material">Material to be produced (exemple MO)</param>
        /// <returns></returns>
        //public static CustomSetupConsumptionDetailCollection SetupConsumptionUtility(Material material)
        //{
        //    CustomSetupConsumptionDetailCollection setupConsumptions = new CustomSetupConsumptionDetailCollection();
        //    BOM mBOM = new BOM();
        //    BOM dBOM = new BOM();
        //    CustomSetupConsumptionDetail consumptionDetail = null;
        //    // Get Line
        //    Resource resource = new Resource();
        //    material.Load();
        //    material.LoadRelations("MaterialResource");
        //    resource = material.MaterialResourceRelations.FirstOrDefault()?.TargetEntity;
        //    resource.GetDescendentResources(-1);
        //    #region Resolve and validate mBOM
        //    // All Consumable Feeds from Line
        //    Dictionary<int, Resource> orderedConsumableFeedResources = new Dictionary<int, Resource>();
        //    int orderC = 1;
        //    orderedConsumableFeedResources = resource.GetResourceSubResources(ref orderC, ProcessingType.ConsumableFeed.ToString());
           
        //    resource.LoadRelations();
            
        //    foreach(SubResource sub in resource.RelationCollection["SubResource"].ToList())
        //    {
        //        sub.Load(1);
        //        if(sub.TargetEntity.ProcessingType.Equals(ProcessingType.ConsumableFeed))
        //        {
        //            orderedConsumableFeedResources.Add(sub.Order, sub.TargetEntity);

        //        }
        //    }
        //    orderedConsumableFeedResources
        //    if (material.CurrentBOMVersion == null)
        //    {
        //        var bomDataInput = new BomDataInput
        //        {
        //            BomLevelsToLoad = 1,
        //            BOMProductLevelsToLoad = 1,
        //            Material = material,
        //            MaterialLevelsToLoad = 0,
        //            Operation = GetDataForTrackInOperation.TrackIn,
        //            Resource = resource,
        //            ResourceLevelsToLoad = 0,
        //            TopMostMaterialLevelsToLoad = 0,
        //        };
        //        ResolveBomContextsResult bomContext = material.Product.ResolveBomContexts(material, bomDataInput.OperationAttributes);
        //        if (bomContext != null && bomContext.Bom != null && bomContext.Bom.UniversalState == UniversalState.Effective)
        //            mBOM = bomContext.Bom;
        //    }
        //    else
        //    {
        //        mBOM = material.CurrentBOMVersion;
        //    }
        //    if (mBOM?.Name != null)
        //    {
        //        // Get mBOM Products
        //        BOMProductCollection mbOMProducts = mBOM.GetBOMProducts();
        //        if (orderedConsumableFeedResources.Count >= mbOMProducts.Count)
        //        {
        //            foreach (var mbomProduct in mbOMProducts)
        //            {
        //                int order = mbomProduct.Order == null ? 0 : (int)mbomProduct.Order;
        //                Resource consumableFeed = orderedConsumableFeedResources[order];
        //                // Get materials attached to the sub resource
        //                Material mats = consumableFeed.GetAttachedMaterials().FirstOrDefault();
        //                consumptionDetail = new CustomSetupConsumptionDetail()
        //                {
        //                    BOM = mBOM,
        //                    ExpectedProduct = mbomProduct.TargetEntity,
        //                    ConsumableFeed = consumableFeed,
        //                    CurrentlyAttachedProduct = mats == null ? null : mats.Product,
        //                    Position = order
        //                };
        //                setupConsumptions.Add(consumptionDetail);
        //            }
        //        }
        //        else
        //        {
        //            int order = 1;
        //            Resource consumableFeed = orderedConsumableFeedResources[order];
        //            // Get materials attached to the sub resource
        //            Material mats = consumableFeed.GetAttachedMaterials().FirstOrDefault();
        //            consumptionDetail = new CustomSetupConsumptionDetail()
        //            {
        //                BOM = mBOM,
        //                ExpectedProduct = mbOMProducts.FirstOrDefault().TargetEntity,
        //                ConsumableFeed = consumableFeed,
        //                CurrentlyAttachedProduct = mats == null ? null : mats.Product,
        //                Position = order
        //            };
        //            setupConsumptions.Add(consumptionDetail);
        //        }
        //    }
        //    #endregion

        //    return setupConsumptions;
        //}

        /// <summary>
        /// Get all materials attached to a resource
        /// </summary>
        /// <param name="resource">Resource where it is wanted to search for attached materials</param>
        /// <param name="loadMaterials">If it is to load the materials information</param>
        /// <returns>return a material collection with all the materials attached to the consumable feed</returns>
        public static MaterialCollection GetAttachedMaterials(this Resource resource)
        {
            ResourceCollection resources = new ResourceCollection()
            {
                resource
            };
            return resources.GetAttachedMaterials();
        }
        /// <summary>
        /// Get all materials attached to all the resources
        /// </summary>
        /// <param name="resources">Resource Collection where it is wanted to search for attached materials</param>
        /// <param name="loadMaterials">If it is to load the materials information</param>
        /// <returns>Return a material collection with all the materials attached to the consumable feed</returns>
        public static MaterialCollection GetAttachedMaterials(this ResourceCollection resources)
        {
            // resources
            foreach (var resource in resources)
            {
                resource.Load();
                resource.LoadRelations();
            }

            MaterialCollection materials = new MaterialCollection();
            Collection<long> materialIds = new Collection<long>(resources.Where(r => !(r.ResourceMaterials == null || r.ResourceMaterials.Count == 0))
                .SelectMany(r => r.ResourceMaterials.Select(rm => rm.SourceEntity.Id)).ToList());
            materials.Load(materialIds);
            return materials;
        }




        #endregion

        public static DataCollectionPointCollection DataCollectionPointCollection_ForTrackOut(decimal quantity = 50)
        {
            Parameter parameter = new Parameter()
            {
                Name = "MOCompletedQuantity"
            };
            parameter.Load();
            DataCollectionPoint dataCollectionPoint = new DataCollectionPoint()
            {
                TargetEntity = parameter,
                Value = quantity,
                ReadingNumber = 1,
                SampleId = "Sample 1"
            };
            DataCollectionPointCollection dataCollectionPoints = new DataCollectionPointCollection()
            {
                dataCollectionPoint
            };

            return dataCollectionPoints;
        }

        /// <summary>
        /// Generartes HEX String based on current time stamp
        /// </summary>
        /// <returns></returns>
        public static String GenerateHexGuid()
        {
            System.Threading.Thread.Sleep(4);

            DateTime currentDate = TestUtilities.GetServerTimeUTC();
            String hexGUid = Int64.Parse(String.Format("{0}{1}{2}{3}{4}{5}{6}", currentDate.Year.ToString().Substring(2, 2)
                                    , currentDate.Month.ToString().PadLeft(2, '0')
                                    , currentDate.Day.ToString().PadLeft(2, '0')
                                    , currentDate.Hour.ToString().PadLeft(2, '0')
                                    , currentDate.Minute.ToString().PadLeft(2, '0')
                                    , currentDate.Second.ToString().PadLeft(2, '0')
                                    , currentDate.Millisecond.ToString().PadLeft(3, '0')
                                    )).ToString("X").PadLeft(12, '0');

            return hexGUid;
        }

        /// <summary>
        /// Retrieves Application Server Current Time (UTC)
        /// </summary>
        /// <param name="setKindToLocal">Indicates if timestamp kind should be set to local</param>
        /// <returns></returns>
        public static DateTime GetServerTimeUTC(bool setKindToLocal = false)
        {
            // not sure if query object uses milliseconds when filtering... waiting one second and one millisecond just in case it doesn't
            System.Threading.Thread.Sleep(1001);
            DateTime returnVal = DateTime.Now.ToUniversalTime(); // new GetServerTimeInput().GetServerTimeSync().ServerTime;
            if (setKindToLocal)
                returnVal = DateTime.SpecifyKind(returnVal, DateTimeKind.Local);

            return returnVal;
        }

        public static void ResourceChangeAutomationMode(String resourceName , ResourceAutomationMode automationMode)
        {
            Resource resource = new Resource() { Name = resourceName };
            resource.Load();

            resource.AutomationMode = automationMode;
            resource.Save();
        }
    }
}
