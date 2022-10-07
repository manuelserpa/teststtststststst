using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Cmf.Custom.amsOSRAM.Actions.Tibco
{
    public class CustomSendEventMessage : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     
            /// Action Groups:
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            bool canExecute = false;

            if (Input.TryGetValueAs("ActionGroupName", out string actionGroupName) && !string.IsNullOrWhiteSpace(actionGroupName))
            {
                Dictionary<string, CustomTransactionTypes> associatedTransactions = new Dictionary<string, CustomTransactionTypes>()
                {
                    { "BusinessObjects.MaterialCollection.Create.Post", CustomTransactionTypes.MaterialCreate },
                    { "BusinessObjects.MaterialCollection.Terminate.Post", CustomTransactionTypes.MaterialTerminate },
                    { "BusinessObjects.MaterialCollection.Dispatch.Post", CustomTransactionTypes.MaterialDispatch },
                    { "BusinessObjects.MaterialCollection.TrackIn.Post", CustomTransactionTypes.MaterialTrackIn },
                    { "BusinessObjects.MaterialCollection.TrackOut.Post", CustomTransactionTypes.MaterialTrackOut },
                    { "BusinessObjects.MaterialCollection.MoveToNextStep.Post", CustomTransactionTypes.MaterialMoveNext },
                    { "BusinessObjects.MaterialCollection.Split.Post", CustomTransactionTypes.MaterialSplit },
                    { "BusinessObjects.MaterialCollection.RecordLoss.Post", CustomTransactionTypes.MaterialLoss },
                    { "BusinessObjects.MaterialCollection.RecordBonus.Post", CustomTransactionTypes.MaterialBonus },
                    { "BusinessObjects.MaterialCollection.Hold.Post", CustomTransactionTypes.MaterialHold },
                    { "BusinessObjects.Material.Merge.Post", CustomTransactionTypes.MaterialMerge },
                    { "BusinessObjects.Material.Release.Post", CustomTransactionTypes.MaterialRelease },
                    { "BusinessObjects.Container.AssociateMaterials.Post", CustomTransactionTypes.ContainerAssociation }
                };

                if (associatedTransactions.ContainsKey(actionGroupName))
                {
                    CustomTransactionTypes transactionToExecute = associatedTransactions[actionGroupName];

                    GenericTable customTransactionsToTibcoGT = new GenericTable() { Name = amsOSRAMConstants.GenericTableCustomTransactionsToTibco };
                    customTransactionsToTibcoGT.Load();

                    customTransactionsToTibcoGT.LoadData(new Foundation.BusinessObjects.QueryObject.FilterCollection()
                    {
                        new Foundation.BusinessObjects.QueryObject.Filter()
                        {
                            Name = amsOSRAMConstants.GenericTableCustomTransactionsToTibcoTransactionProperty,
                            Operator = FieldOperator.IsEqualTo,
                            LogicalOperator = LogicalOperator.AND,
                            Value = transactionToExecute
                        },
                        new Foundation.BusinessObjects.QueryObject.Filter()
                        {
                            Name = amsOSRAMConstants.GenericTableCustomTransactionsToTibcoIsEnabledProperty,
                            Operator = FieldOperator.IsEqualTo,
                            LogicalOperator = LogicalOperator.AND,
                            Value = true
                        }
                    });

                    if (customTransactionsToTibcoGT.HasData)
                    {
                        canExecute = true;

                        DeeContextHelper.SetContextParameter("TransactionToExecute", transactionToExecute);
                    }
                }
            }

            return canExecute;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //Foundation
            UseReference("", "Cmf.Foundation.Common.Exceptions");

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IMaterialCollection materialCollection = entityFactory.CreateCollection<IMaterialCollection>();
            string messageSubject = string.Empty;
            string messageXml = string.Empty;

            CustomTransactionTypes transactionToExecute = (CustomTransactionTypes)DeeContextHelper.GetContextParameter("TransactionToExecute");
            switch (transactionToExecute)
            {
                case CustomTransactionTypes.MaterialCreate:
                case CustomTransactionTypes.MaterialTerminate:
                case CustomTransactionTypes.MaterialDispatch:
                case CustomTransactionTypes.MaterialTrackIn:
                case CustomTransactionTypes.MaterialTrackOut:
                case CustomTransactionTypes.MaterialMoveNext:
                case CustomTransactionTypes.MaterialLoss:
                case CustomTransactionTypes.MaterialBonus:
                case CustomTransactionTypes.MaterialHold:
                    {
                        materialCollection = DeeActionHelper.GetInputItem<IMaterialCollection>(Input, Navigo.Common.Constants.MaterialCollection);
                        messageSubject = amsOSRAMConstants.CustomLotChange;
                    }
                    break;
                case CustomTransactionTypes.MaterialRelease:
                case CustomTransactionTypes.MaterialMerge:
                    {
                        IMaterial material = DeeActionHelper.GetInputItem<IMaterial>(Input, Navigo.Common.Constants.Material);
                        materialCollection.Add(material);
                        messageSubject = amsOSRAMConstants.CustomLotChange;
                    }
                    break;
                case CustomTransactionTypes.MaterialSplit:
                    {
                        Dictionary<IMaterial, ISplitInputParametersCollection> splittedMaterials = DeeActionHelper.GetInputItem<Dictionary<IMaterial, ISplitInputParametersCollection>>(Input, "ChildMaterialsInformation");
                        materialCollection.AddRange(splittedMaterials.Keys);
                        messageSubject = amsOSRAMConstants.CustomLotChange;
                    }
                    break;
                case CustomTransactionTypes.ContainerAssociation:
                    {
                        messageSubject = amsOSRAMConstants.CustomEquipmentStatusChange;
                    }
                    break;
            }

            if (!string.IsNullOrWhiteSpace(messageSubject))
            {
                foreach (IMaterial material in materialCollection)
                {
                    // Publish message on Message Bus
                    Utilities.PublishTransactionalMessage(messageSubject,
                                                          JsonConvert.SerializeObject(new
                                                          {
                                                              Header = GetMessageHeader(material, transactionToExecute.ToString()),
                                                              Message = GetMaterialCollectionXml(material, transactionToExecute.ToString())
                                                          }));
                }
            }

            string GetMaterialCollectionXml(IMaterial material, string actionName)
            {
                IMaterialCollection materialCollection = entityFactory.CreateCollection<IMaterialCollection>();
                materialCollection.Add(material);

                // List of ElementNames to discard on output XML
                List<string> xmlElementsToDiscard = new List<string>()
                {
                    "CurrentBOMAssemblyType",
                    "CurrentBOMVersion",
                    "LineFlowVersion",
                    "LineValidationMode",
                    "LineAssemblyMode",
                    "InhibitMoveFromStep",
                    "OverrideProductBlock",
                    "InhibitShip",
                    "InTransitFromState",
                    "InTransitType",
                    "IsInNonSequentialBlock",
                    "LocationAltitude",
                    "LocationLatitude",
                    "LocationLongitude",
                    "ResourceAssociationType",
                    "SplitMergeRestrictionType",
                    "NotificationCount",
                    "TimeConstraintsCount",
                    "CurrentSamplingPattern",
                    "SamplingSequence",
                    "RequiredFutureAction",
                    "MaterialTransferFromFacility",
                    "MaterialTransferCostCenter",
                    "MaximumAssembleDate",
                    "CurrentBOMTrackOutLossesMode",
                    "MaintenanceHoldCount",
                    "AccountsToProductionOrderQuantity",
                    "ExcludeFromScheduling",
                    "HasFromDependencies",
                    "HasToDependencies",
                    "CurrentBOMWeighAndDispenseMode",
                    "ExperimentMaterialGroupName",
                    "ExperimentSubMaterialNumber",
                    "TargetMaterialQuantity",
                    "TargetMaterialUnits",
                    "CurrentBOMUnits",
                    "PickListItemCount",
                    "IsInTransferOrderItem",
                    "MoistureSensitivityLevel",
                    "FloorLifeOpenDate",
                    "FloorLifeCounterState",
                    "FloorLifeRemainingHours",
                    "FloorLifeSealed",
                    "ManufacturerPartNumber",
                    "ManufacturerLotNumber",
                    "DateCode",
                    "CapacityClass",
                    "IsRoHSCompliant",
                    "IsApproved",
                    "RequiredResource",
                    "OpenInspectionOrderCount",
                    "OpenInspectionOrderStepSampleCount",
                    "PendingLineReworkReturn",
                    "InTransitToFacility",
                    "IsDispatchable",
                    "Priority",
                    "RequiredService",
                    "ShippingLabel",
                    "SynchronizeMapUnits",
                    "MasterMap",
                    "IsTemplate",
                    "RelationCollection",
                    "DocumentationURL",
                    "Image",
                    "DataGroupName"
                };

                // List of AttributeNames associated to the Elements to discard on output XML
                List<string> xmlAttributesToDiscard = new List<string>()
                {
                    "type",
                    "actualtype",
                    "ExportId",
                    "RefId"
                };

                // Load exported XML object
                XDocument xDocument = XDocument.Parse(materialCollection.ExportToString());

                #region Get & Replace Attributes associated to the Materials

                Dictionary<string, XElement> materialsAttributes = ExportMaterialsAttributesToXml(materialCollection);

                if (materialsAttributes is not null && materialsAttributes.Any())
                {
                    foreach (KeyValuePair<string, XElement> materialAttributes in materialsAttributes)
                    {
                        // Material Element filtered by MaterialName
                        XElement materialElement = xDocument.Root.Descendants("Object")?.Where(obj => obj.Attribute("type").Value.StartsWith("Cmf.Navigo.BusinessObjects.Material", StringComparison.InvariantCultureIgnoreCase))?
                                                                                        .Where(e => e.Element("Name").Attribute("value").Value.Equals(materialAttributes.Key, StringComparison.InvariantCultureIgnoreCase))?
                                                                                        .Single();

                        if (materialElement is not null && materialAttributes.Value is not null)
                        {
                            // Replace the Attributes element associated to the Material
                            materialElement.Element("Attributes").ReplaceWith(materialAttributes.Value);
                        }
                    }
                }

                #endregion Get & Replace Attributes associated to the Materials

                #region Remove discarded Elements & Attributes from XML

                // Remove Material discarded Elements
                xDocument.Root.Descendants().Where(e => xmlElementsToDiscard.Contains(e.Name.LocalName)).Remove();

                // Remove Material discarded Attributes
                xDocument.Root.Descendants().Attributes().Where(a => xmlAttributesToDiscard.Contains(a.Name.LocalName)).Remove();

                #endregion Remove discarded Elements & Attributes from XML

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xDocument.ToString());

                return xmlDocument.InnerXml;
            }

            Dictionary<string, XElement> ExportMaterialsAttributesToXml(IMaterialCollection materials, Dictionary<string, XElement> materialsAttributes = null)
            {
                Dictionary<string, XElement> outputMaterialsAttributes = new Dictionary<string, XElement>();

                if (materialsAttributes is not null && materialsAttributes.Any())
                {
                    outputMaterialsAttributes = materialsAttributes;
                }

                foreach (IMaterial material in materials)
                {
                    material.LoadAttributes();

                    if (material.Attributes.Count > 0)
                    {
                        XElement materialAttributesElement = new XElement("Attributes");

                        List<XElement> attributes = new List<XElement>();

                        foreach (KeyValuePair<string, object> attribute in material.Attributes)
                        {
                            XElement attributeElement = new XElement(attribute.Key, new XAttribute("value", attribute.Value.ToString()));

                            attributes.Add(attributeElement);
                        }

                        materialAttributesElement.Add(attributes);

                        outputMaterialsAttributes.Add(material.Name, materialAttributesElement);
                    }

                    if (material.SubMaterialCount > 0)
                    {
                        material.LoadChildren();

                        ExportMaterialsAttributesToXml(material.SubMaterials, outputMaterialsAttributes);
                    }
                }

                return outputMaterialsAttributes;
            }

            object GetMessageHeader(IMaterial material, string action)
            {
                string lotName = string.Empty;
                string pathFrom = string.Empty;
                string pathTo = string.Empty;
                string sAPproductType = string.Empty;

                lotName = material.Name;

                string facilityCode = string.Empty;
                if (material.Facility.HasAttribute(amsOSRAMConstants.CustomFacilityCodeAttribute, true))
                {
                    facilityCode = material.Facility.GetAttributeValue(amsOSRAMConstants.CustomSiteCodeAttribute) as string;
                }

                material.Facility.Site.Load();
                string siteCode = string.Empty;
                if (material.Facility.Site.HasAttribute(amsOSRAMConstants.CustomSiteCodeAttribute, true))
                {
                    siteCode = material.Facility.Site.GetAttributeValue(amsOSRAMConstants.CustomSiteCodeAttribute) as string;
                }

                string stepLogicalName = string.Empty;
                if (material.Step.ContainsLogicalNames)
                {
                    material.Flow.LoadRelations(Navigo.Common.Constants.FlowStep);
                    IFlowStep flowStep = entityFactory.Create<IFlowStep>();
                    IStep step = entityFactory.Create<IStep>();

                    material.Flow.GetFlowAndStepFromFlowpath(material.FlowPath, ref step, ref flowStep);
                    stepLogicalName = flowStep.LogicalName;
                }

                pathFrom = pathTo = $"{siteCode}.{facilityCode}.{stepLogicalName}";

                if (material.Product.HasRelatedAttribute(amsOSRAMConstants.ProductAttributeSAPProductType, true))
                {
                    sAPproductType = material.Product.GetRelatedAttributeValue(amsOSRAMConstants.ProductAttributeSAPProductType) as string;
                }

                return new
                {
                    stdObjectName = lotName,
                    stdFrom = pathFrom,
                    stdTo = pathTo,
                    stdProductType = sAPproductType,
                    stdDataOrigin = Environment.MachineName,
                    stdTransaction = action
                };
            }

            //---End DEE Code---

            return Input;
        }
    }
}
