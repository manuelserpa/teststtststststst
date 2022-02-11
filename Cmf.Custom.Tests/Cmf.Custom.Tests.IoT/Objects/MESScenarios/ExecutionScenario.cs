using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Navigo.BusinessObjects;
using Cmf.TestScenarios.Others;
using AMSOsramEIAutomaticTests.Objects.TestUtilities;
using System.Collections.Generic;
using System.Linq;
using cmConnect.TestFramework.SystemRest.Utilities;
using Cmf.Navigo.BusinessOrchestration.DispatchManagement.InputObjects;
using AMSOsramEIAutomaticTests.Objects.Utilities;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMSOsramEIAutomaticTests.Objects;
using AMSOsramEIAutomaticTests.Objects.Extensions;
using Cmf.Foundation.Common.Base;
using System.Collections.ObjectModel;
using Cmf.Custom.TestUtilities;

namespace AMSOsramEIAutomaticTests.MESObjects
{
    public class ExecutionScenario : BaseCustomScenario
    {
       
        public Material Material;

        /// <summary>
        /// The Primary Quantity for the Material
        /// </summary>
        public int PrimaryQuantity { get; set; } = 100;

        public string FacilityName { get; set; } = Constants.DefaultFacility;

        public Resource Resource;

        /// <summary>
        /// Product Name
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Product Name
        /// </summary>
        public string ProductName { get; set; } = Constants.DefaultProduct;

        public string FlowName = Constants.DefaultFlow;

        public string StepName = Constants.DefaultStep;

        /// <summary>
        /// Set FlowPath
        /// </summary>
        public string FlowPath { get; set; } = Constants.DefaultFlowPath;
        

        public ExecutionScenario()
        {
        }

        public override void Setup()
        {        
            Resource.Load();
            Facility facility = new Facility() { Name = FacilityName };

            if (Product == null)
            {
                Product = new Product() { Name = ProductName };
            }


            Material = new Material()
            {
                Name = "EITest_Material_" + DateTime.UtcNow.ToString("MMddyyyyhhmmsstt"),
                Facility = facility,
                Product = Product,
                Form = Constants.LotForm,
                Type = Constants.ProductionType,
                Flow = new Flow() { Name = FlowName },
                Step = new Step() { Name = StepName },
                FlowPath = FlowPath,
                PrimaryQuantity = PrimaryQuantity,
                SecondaryQuantity = 0,
            };
            Material.Create();

            Material.Load();
            Material.Expand(Constants.WaferForm);

            if (Resource == null)
                Resource = TestUtilities.GetResourceForDispatch(Material);
            Resource.Load();
            Resource.SetResourceDispachable();

            if (Resource != null && Resource.EnableCheckIn && Resource.RequireCheckInForMaterialOperations)
            {
                Resource.Load();
                Resource.LoadRelation("ResourceEmployee");

                if (Resource.HasRelation("ResourceEmployee"))
                {
                    var user = TestUtilities.GetEmployeeLoggedIn();
                    var resourceEmployee = this.Resource.RelationCollection["ResourceEmployee"].Cast<ResourceEmployee>().FirstOrDefault(x => x.TargetEntity.Id == user.Id);

                    if (resourceEmployee == null)
                    {
                        Resource.CheckIn(userName: Constants.UserName);
                    }
                }
                else
                {
                    Resource.CheckIn(userName: Constants.UserName);
                }
                Resource.Load();
            }

        }

        public override void CompleteCleanUp()
        {



            try
            {
                Material.TerminateAllMaterialRelations();
                Material.TerminateMaterial();
            }
            catch
            {

            }

            try { 
                 if (Resource != null)
                 {
                     Resource.Load();
                     if (Resource.EnableCheckIn && Resource.RequireCheckInForMaterialOperations)
                         Resource.CheckOut(userName: Constants.UserName);
                 }

               }
               catch (Exception e)
               {
                   throw new Exception("Tear down failed cleaning up the attached materials: " + e);
               }

        }

    }
}
