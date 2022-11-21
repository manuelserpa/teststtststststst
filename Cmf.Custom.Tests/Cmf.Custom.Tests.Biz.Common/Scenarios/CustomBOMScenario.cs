using Cmf.Custom.amsOSRAM.BusinessObjects;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.BusinessOrchestration.ChangeSetManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.ChangeSetManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.LocalizationManagement.InputObjects;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.Common.Scenarios
{
    public class CustomBOMScenario : CustomBaseScenario
    {
        #region Properties

        public BOMCollection GeneratedBOMs = new BOMCollection();

        public CustomSorterJobDefinitionCollection GeneratedCustomSorterJobDefinitions = new CustomSorterJobDefinitionCollection();

        #endregion Properties

        /// <summary>
        /// CustomBOMScenario Constructor
        /// </summary>
        public CustomBOMScenario()
        {
        }

        /// <summary>
        /// CustomBOMScenario Setup
        /// </summary>
        public override void Setup()
        {
        }

        public override void CompleteCleanUp()
        {
            TearDownManager.TearDownSequentially();
        }

        #region Private Methods

        public Tuple<BOM, RequestChangeSetApprovalOutput, CustomSorterJobDefinition> GenerateClonedBOM(string bomToCloneName,
                                                           string bomName = null,
                                                           string bomType = null,
                                                           string bomUnits = amsOSRAMConstants.UnitWafers,
                                                           string startingCarrierType = null,
                                                           bool? isForLotCompose = null,
                                                           BOMScope bomScope = BOMScope.Materials,
                                                           BOMProductCollection bomProducts = null,
                                                           ChangeSet changeSet = null)
        {
            if (changeSet == null)
            {
                changeSet = new ChangeSet
                {
                    Name = Guid.NewGuid().ToString(),
                    Type = "General",
                    MakeEffectiveOnApproval = true
                };
                changeSet.Create();
            }

            BOM bom = new BOM() { Name = bomToCloneName };
            bom.Load();
            bom.LoadRelations(new Collection<string>() { "BOMProduct" });

            BOM targetBom = new BOM()
            {
                Name = bomName ?? "TestBOM_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff"),
                Type = bomType ?? bom.Type,
                Units = bomUnits,
                Scope = bomScope,
                ChangeSet = changeSet,
                BomProducts = bomProducts ?? bom.BomProducts
            };

            CloneObjectOutput cloneOutput = new CloneObjectInput()
            {
                ChangeSet = changeSet,
                Source = bom,
                Targets = new Collection<object>() { targetBom }
            }.CloneObjectSync();

            BOM newBom = cloneOutput.Targets.FirstOrDefault() as BOM;

            TearDownManager.Push(newBom);
            GeneratedBOMs.Add(newBom);

            AttributeCollection attributes = new AttributeCollection();

            if (!String.IsNullOrWhiteSpace(startingCarrierType))
            {
                attributes.Add("StartingCarrierType", startingCarrierType);
            }

            if (isForLotCompose.HasValue)
            {
                attributes.Add("IsForLotCompose", isForLotCompose);
            }

            if (attributes.Any())
            {
                newBom.SaveRelatedAttributes(attributes);
            }

            newBom.LoadRelations(new Collection<string>() { "BOMProduct" });

            RequestChangeSetApprovalOutput requestChangeSetApprovalOutput = new RequestChangeSetApprovalInput()
            {
                ChangeSet = changeSet,
                IgnoreLastServiceId = true
            }.RequestChangeSetApprovalSync();

            CustomSorterJobDefinition customSorterJobDefinition = new CustomSorterJobDefinition() { Name = GetCustomSorterJobBasedOnBOM(newBom) };

            if (customSorterJobDefinition.ObjectExists())
            {
                customSorterJobDefinition.Load();
                TearDownManager.Push(customSorterJobDefinition);
                GeneratedCustomSorterJobDefinitions.Add(customSorterJobDefinition);
            }

            return new Tuple<BOM, RequestChangeSetApprovalOutput, CustomSorterJobDefinition>(newBom, requestChangeSetApprovalOutput, customSorterJobDefinition);
        }

        public Tuple<BOM, RequestChangeSetApprovalOutput, CustomSorterJobDefinition> CreateNewBOMVersion(BOM bom, ChangeSet changeSet = null)
        {
            if (changeSet == null)
            {
                changeSet = new ChangeSet
                {
                    Name = Guid.NewGuid().ToString(),
                    Type = "General",
                    MakeEffectiveOnApproval = true
                };
                changeSet.Create();
            }

            bom.Load();
            bom.LoadRelations(new Collection<string>() { "BOMProduct" });
            bom.ChangeSet = changeSet;

            CreateObjectVersionInput createObjectVersionInput = new CreateObjectVersionInput();
            createObjectVersionInput.Object = bom;
            createObjectVersionInput.OperationTarget = EntityTypeSource.Version;
            bom = createObjectVersionInput.CreateObjectVersionSync().Object as BOM;

            RequestChangeSetApprovalOutput requestChangeSetApprovalOutput = new RequestChangeSetApprovalInput()
            {
                ChangeSet = changeSet,
                IgnoreLastServiceId = true
            }.RequestChangeSetApprovalSync();

            TearDownManager.Push(bom);
            GeneratedBOMs.Add(bom);

            CustomSorterJobDefinition customSorterJobDefinition = new CustomSorterJobDefinition() { Name = GetCustomSorterJobBasedOnBOM(bom) };

            if (customSorterJobDefinition.ObjectExists())
            {
                customSorterJobDefinition.Load();
                TearDownManager.Push(customSorterJobDefinition);
                GeneratedCustomSorterJobDefinitions.Add(customSorterJobDefinition);
            }

            return new Tuple<BOM, RequestChangeSetApprovalOutput, CustomSorterJobDefinition>(bom, requestChangeSetApprovalOutput, customSorterJobDefinition);
        }

        public string GetCustomSorterJobBasedOnBOM(BOM bom)
        {
            return String.Format("{0}.{1}.{2}", bom.Name, bom.Revision, bom.Version);
        }

        #endregion Private Methods
    }
}
