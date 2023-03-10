using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.BusinessObjects.Abstractions;
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
            ///   DEE Action used to publish Lot event messages to MessageBus based on Material action. E.g.: Material.TrackIn, Material.TrackOut, Material.MoveNext.
            /// Action Groups:
            ///   BusinessObjects.MaterialCollection.Create.Post
            ///   BusinessObjects.MaterialCollection.Dispatch.Post
            ///   BusinessObjects.MaterialCollection.TrackIn.Post
            ///   BusinessObjects.MaterialCollection.TrackOut.Post
            ///   BusinessObjects.MaterialCollection.MoveToNextStep.Pre
            ///   BusinessObjects.MaterialCollection.MoveToNextStep.Post
            ///   BusinessObjects.MaterialCollection.Split.Post
            ///   BusinessObjects.MaterialCollection.RecordLoss.Post
            ///   BusinessObjects.MaterialCollection.RecordBonus.Post
            ///   BusinessObjects.MaterialCollection.Hold.Post
            ///   BusinessObjects.MaterialCollection.Terminate.Post
            ///   BusinessObjects.Material.Release.Post
            ///   BusinessObjects.Material.Merge.Post
            ///   BusinessObjects.Container.AssociateMaterials.Post
            ///   BusinessObjects.MaterialCollection.Ship.Post
            ///   BusinessObjects.MaterialCollection.Receive.Pre
            ///   BusinessObjects.MaterialCollection.Receive.Post
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            bool canExecute = false;

            if (Input.TryGetValueAs("ActionGroupName", out string actionGroupName) && !string.IsNullOrWhiteSpace(actionGroupName))
            {
                if (actionGroupName == "BusinessObjects.MaterialCollection.MoveToNextStep.Pre" ||
                    actionGroupName == "BusinessObjects.MaterialCollection.Receive.Pre")
                {
                    Dictionary<string, string> materialsSourcePath = new Dictionary<string, string>();

                    IMaterialCollection materialCollection = DeeActionHelper.GetInputItem<IMaterialCollection>(Input, Navigo.Common.Constants.MaterialCollection);
                    foreach (IMaterial material in materialCollection)
                    {
                        materialsSourcePath.Add(material.Name, amsOSRAMUtilities.GetMaterialSourcePath(material));
                    }

                    DeeContextHelper.SetContextParameter("MaterialsPreSourcePath", materialsSourcePath);
                    return canExecute;
                }

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
                    { "BusinessObjects.Container.AssociateMaterials.Post", CustomTransactionTypes.ContainerAssociation },
                    { "BusinessObjects.MaterialCollection.Ship.Post", CustomTransactionTypes.MaterialShip },
                    { "BusinessObjects.MaterialCollection.Receive.Post", CustomTransactionTypes.MaterialReceive }
                };

                if (associatedTransactions.ContainsKey(actionGroupName))
                {
                    CustomTransactionTypes transactionToExecute = associatedTransactions[actionGroupName];

                    IGenericTable customTransactionsToTibcoGT = entityFactory.Create<IGenericTable>();
                    customTransactionsToTibcoGT.Load(amsOSRAMConstants.GenericTableCustomTransactionsToTibco);

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
                            LogicalOperator = LogicalOperator.Nothing,
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

            // System
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json");
            UseReference("%MicrosoftNetPath%System.Private.Xml.dll", "System.Xml");
            UseReference("%MicrosoftNetPath%System.Private.Xml.Linq.dll", "System.Xml.Linq");

            // Foundation
            UseReference("", "Cmf.Foundation.Common.Exceptions");

            // Common
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("", "Cmf.Custom.amsOSRAM.Common.DataStructures");
            UseReference("", "Cmf.Custom.amsOSRAM.Common.Extensions");

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
                case CustomTransactionTypes.MaterialShip:
                case CustomTransactionTypes.MaterialReceive:
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
                        IMaterialContainerCollection materialContainerCollection = DeeActionHelper.GetInputItem<IMaterialContainerCollection>(Input, "MaterialRelations");
                        foreach (IMaterialContainer materialContainer in materialContainerCollection)
                        {
                            materialCollection.Add(materialContainer.SourceEntity);
                        }
                        messageSubject = amsOSRAMConstants.CustomLotChange;
                    }
                    break;
            }

            if (!string.IsNullOrWhiteSpace(messageSubject) && materialCollection.Any())
            {
                foreach (IMaterial material in materialCollection)
                {
                    // Publish message on Message Bus
                    Utilities.PublishTransactionalMessage(messageSubject,
                                                          JsonConvert.SerializeObject(new
                                                          {
                                                              Header = GetMessageHeader(material, transactionToExecute),
                                                              Message = GetMaterialXml(material)
                                                          }));
                }
            }

            #region Auxiliar Methods

            /// <summary>
            /// Get Message Header for Tibco
            /// </summary>
            /// <param name="material">Material</param>
            /// <param name="transactionToExecute">Transaction type to exectute</param>
            /// <returns>Json object with Header</returns>
            object GetMessageHeader(IMaterial material, CustomTransactionTypes transactionToExecute)
            {
                // Get stdObjectName key header message value
                string lotName = material.Name;

                // Get stdFrom key header message value
                string pathFrom = amsOSRAMUtilities.GetMaterialSourcePath(material);

                // Get stdTo key header message value
                string pathTo = pathFrom;

                // Get the InTransit Facility when Material is shipped
                if (material.InTransitToFacility is not null)
                {
                    pathTo = amsOSRAMUtilities.GetMaterialSourcePath(material, material.InTransitToFacility);
                }

                // When the transaction is MoveNext or Receive the pathTo is obtained from the Material in the Post action
                // but the pathFrom must be retrieved from the Material on Pre action (when its possible to get the previous Step name).
                if (transactionToExecute == CustomTransactionTypes.MaterialMoveNext ||
                    transactionToExecute == CustomTransactionTypes.MaterialReceive)
                {
                    Dictionary<string, string> materialsSourcePath = DeeContextHelper.GetContextParameter("MaterialsPreSourcePath") as Dictionary<string, string>;
                    pathFrom = materialsSourcePath.GetValueOrDefault(material.Name);
                }

                // Get stdProductType key header message value
                string sapProductType = string.Empty;
                if (material.Product.HasRelatedAttribute(amsOSRAMConstants.ProductAttributeSAPProductType, true))
                {
                    sapProductType = material.Product.GetRelatedAttributeValue(amsOSRAMConstants.ProductAttributeSAPProductType) as string;
                }

                // Build Header object
                return new
                {
                    stdObjectName = lotName,
                    stdFrom = pathFrom,
                    stdTo = pathTo,
                    stdProductType = sapProductType,
                    stdDataOrigin = Environment.MachineName,
                    stdTransaction = transactionToExecute.ToString()
                };
            }

            string GetMaterialXml(IMaterial material)
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
                    "ExportId"
                };

                materialCollection.LoadChildren();
                materialCollection.LoadMaterialOffFlows();

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

            #endregion Auxiliar Methods

            //---End DEE Code---

            return Input;
        }
    }
}
