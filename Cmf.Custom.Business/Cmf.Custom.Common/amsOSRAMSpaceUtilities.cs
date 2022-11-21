using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.amsOSRAM.Common.DataStructures.CustomReportEDCToSpaceDto;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Foundation.Security.Abstractions;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessOrchestration.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Cmf.Custom.amsOSRAM.Common
{
    /// <summary>
    /// Support class to encapsulate methods to support the development for the business layer regarding Space
    /// </summary>
    public static class amsOSRAMSpaceUtilities
    {
        #region Space

        /// <summary>
        /// Method to create xml message with Wafer and Data Collection Info to be sent to Space system
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="dataCollectionInstance"></param>
        /// <param name="limitSet"></param>
        /// <returns></returns>
        public static CustomReportEDCToSpace CreateSpaceInfoWaferValues(IMaterial lot, IDataCollectionInstance dataCollectionInstance, IDataCollectionLimitSet limitSet)
        {
            // Get services provider information
            IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();
            ILaborOrchestration laborOrchestration = serviceProvider.GetService<ILaborOrchestration>();

            lot.Load(1);
            lot.LoadChildren();
            lot.LoadRelations(Cmf.Navigo.Common.Constants.MaterialResource);

            IResource currentLotResource = lot.LastProcessedResource;

            if (currentLotResource != null)
            {
                currentLotResource.LoadRelations(Cmf.Navigo.Common.Constants.ResourceEmployee);
            }

            IEmployee employee = currentLotResource?.ResourceEmployees != null && currentLotResource.ResourceEmployees.Any()
                ? currentLotResource.ResourceEmployees.FirstOrDefault().TargetEntity
                : null;

            if (employee is null)
            {
                IUser user = entityFactory.Create<IUser>();
                user.Name = Utilities.DomainUserName;
                user.Load();

                employee = entityFactory.Create<IEmployee>();
                employee.Name = user.UserName;
            }

            IShiftDefinitionShift shiftDefinitionShift = null;

            if (employee.ObjectExists())
            {
                employee.Load();
                employee.Calendar.Load();
                shiftDefinitionShift = amsOSRAMUtilities.GetShiftDefinitionShiftByCalendar(employee.Calendar);
            }

            string sapOwner = "-";
            if (lot.Form == amsOSRAMConstants.MaterialLotForm && lot.HasAttribute(amsOSRAMConstants.MaterialAttributeSAPOwner, true))
            {
                sapOwner = (string)lot.GetAttributeValue(amsOSRAMConstants.MaterialAttributeSAPOwner);
            }

            lot.LoadAttribute(amsOSRAMConstants.MaterialLastProcessRecipeAttribute);

            string lastProcessRecipeName = "-";
            if (lot.Attributes.ContainsKey(amsOSRAMConstants.MaterialLastProcessRecipeAttribute))
            {
                lot.Attributes.TryGetValueAs(amsOSRAMConstants.MaterialLastProcessRecipeAttribute, out lastProcessRecipeName);
            }

            List<Key> Keys = new List<Key>()
            {
                new Key()
                {
                    Name = "LOT",
                    Value = lot.Name
                },
                new Key()
                {
                    Name = "BATCH",
                    Value = lot.ParentMaterial?.Name != null && lot.ParentMaterial.Form == "Batch" ? lot.ParentMaterial?.Name : "-"
                },
                new Key()
                {
                    Name = "LOT TYPE",
                    Value = sapOwner
                },
                new Key()
                {
                    Name = "PROCESS EQUIPMENT 1",
                    Value = lot.LastProcessStepResource?.Name ?? "-"
                },
                new Key()
                {
                    Name = "EQUIPMENT PLATFORM",
                    Value = lot.LastProcessStepResource?.Model ?? "-"
                },
                new Key()
                {
                    Name = "PROCESS RECIPE 1",
                    Value = lastProcessRecipeName ?? "-"
                },
                new Key()
                {
                    Name = "MEASUREMENT EQUIPMENT",
                    Value = currentLotResource?.Name ?? "-"
                },
                new Key()
                {
                    Name = "MEASUREMENT RECIPE",
                    Value = lot.LastRecipe?.Name ?? "-"
                },
                new Key()
                {
                    Name = "QUANTITY",
                    Value = lot.SubMaterialCount.ToString()
                },
                new Key()
                {
                    Name = "ACCESSORY",
                    Value = "-"
                },
                new Key()
                {
                    Name = "OPERATOR",
                    Value = employee?.EmployeeNumber ?? "-"
                },
                new Key()
                {
                    Name = "SHIFT",
                    Value = shiftDefinitionShift?.Name ?? "-"
                },
                new Key()
                {
                    Name = "SENDER",
                    Value = "CMF"
                },
                new Key()
                {
                    Name = "AREA",
                    Value = lot.Facility.Name
                },
                new Key()
                {
                    Name = "WILDCARD DA1",
                    Value = "-"
                },
                new Key()
                {
                    Name = "WILDCARD DA2",
                    Value = "-"
                },
            };

            IArea area = lot.GetArea();
            string ldsId = String.Empty;

            if (area != null)
            {
                area.LoadAttribute(amsOSRAMConstants.AreaLdsIdAttribute);
                area.Attributes.TryGetValueAs(amsOSRAMConstants.AreaLdsIdAttribute, out ldsId);
            }

            // Get StepLogicalName to skip load on logical wafer if they match
            string stepLogicalName = lot.Step.Name;

            if (lot.Step.ContainsLogicalNames)
            {
                lot.Flow.LoadRelations(Navigo.Common.Constants.FlowStep);
                IFlowStep flowStep = entityFactory.Create<IFlowStep>();
                IStep step = entityFactory.Create<IStep>();

                lot.Flow.GetFlowAndStepFromFlowpath(lot.FlowPath, ref step, ref flowStep);
                stepLogicalName = flowStep.LogicalName;
            }

            // get distinct parameters
            IParameterCollection parameters = entityFactory.CreateCollection<IParameterCollection>();
            dataCollectionInstance.LoadRelations();

            IDataCollection dc = dataCollectionInstance.DataCollection;
            dc.LoadRelations(Navigo.Common.Constants.DataCollectionParameter);

            parameters.AddRange(dc.DataCollectionParameters.Select(p => p.TargetEntity));
            parameters.Load();
            parameters.LoadAttributes(new Collection<string> { amsOSRAMConstants.CustomParameterSendToSpaceAttribute });

            IParameterCollection eligibleParameters = entityFactory.CreateCollection<IParameterCollection>();
            eligibleParameters.AddRange(parameters.Where(s =>
                s.HasAttribute(amsOSRAMConstants.CustomParameterSendToSpaceAttribute) &&
                (bool)s.GetAttributeValue(amsOSRAMConstants.CustomParameterSendToSpaceAttribute) &&
                (s.DataType == ParameterDataType.Decimal || s.DataType == ParameterDataType.Long)));

            List<Sample> samples = new List<Sample>();

            foreach (IParameter parameter in eligibleParameters)
            {
                SpecificationLimits limits = new SpecificationLimits();

                if (limitSet?.DataCollectionParameterLimits != null && limitSet.DataCollectionParameterLimits.Any(ls => ls.TargetEntity.Name.Equals(parameter.Name)))
                {
                    IDataCollectionParameterLimit parameterLimit = limitSet.DataCollectionParameterLimits.FirstOrDefault(ls => ls.TargetEntity.Name.Equals(parameter.Name));

                    if (parameterLimit != null)
                    {
                        if (parameterLimit.LowerErrorLimit != null && parameterLimit.UpperErrorLimit != null)
                        {
                            limits.Upper = Decimal.Truncate(parameterLimit.UpperErrorLimit.Value).ToString();
                            limits.Lower = Decimal.Truncate(parameterLimit.LowerErrorLimit.Value).ToString();
                        }

                        if (parameterLimit.LowerWarningLimit != null && parameterLimit.UpperWarningLimit != null)
                        {
                            limits.Upper = Decimal.Truncate(parameterLimit.UpperWarningLimit.Value).ToString();
                            limits.Lower = Decimal.Truncate(parameterLimit.LowerWarningLimit.Value).ToString();
                        }
                    }
                }

                // Get the DC Points for the specific parameter
                IDataCollectionPointCollection points = entityFactory.CreateCollection<IDataCollectionPointCollection>();
                points.AddRange(dataCollectionInstance.DataCollectionPoints.Where(p => p.TargetEntity.Name == parameter.Name));

                // Get Points by sample
                Dictionary<string, List<decimal>> mapSampleDCPoints = new Dictionary<string, List<decimal>>();
                
                foreach(IDataCollectionPoint point in points)
                {
                    if (mapSampleDCPoints.ContainsKey(point.SampleId))
                    {
                        mapSampleDCPoints[point.SampleId].Add(Convert.ToDecimal(point.Value));
                    }
                    else
                    {
                        mapSampleDCPoints.Add(point.SampleId, new List<decimal> { Convert.ToDecimal(point.Value) });
                    }
                }

                IDataCollectionParameter dataCollectionParameter = dc.DataCollectionParameters.FirstOrDefault(f => f.TargetEntity.Name == parameter.Name);
                bool isSampleTypeMaterialId = dataCollectionParameter.SampleKey == DataCollectionParameterSampleKey.MaterialId;

                IMaterialCollection logicalWafers = entityFactory.CreateCollection<IMaterialCollection>();
                
                if (isSampleTypeMaterialId)
                {
                    logicalWafers.AddRange(mapSampleDCPoints.Keys.Select(s =>
                    {
                        IMaterial logicalWafer = entityFactory.Create<IMaterial>();
                        logicalWafer.Name = s;
                        return logicalWafer;
                    }));

                    logicalWafers.Load();
                    logicalWafers.LoadRelations(Cmf.Navigo.Common.Constants.MaterialContainer);
                    logicalWafers.LoadChildren();
                }

                foreach (KeyValuePair<string, List<decimal>> mapSampleDCPoint in mapSampleDCPoints)
                {
                    IMaterial logicalWafer = isSampleTypeMaterialId ? logicalWafers.FirstOrDefault(f => f.Name == mapSampleDCPoint.Key) : null;
                    IMaterialContainer waferContainer = logicalWafer?.MaterialContainer?.FirstOrDefault(f => f.SourceEntity.Id == logicalWafer.Id);
                    IMaterial substrate = logicalWafer?.SubMaterials?.FirstOrDefault(f => f.Form == "Wafer" && f.Type == "Substrate");

                    samples.Add(new Sample
                    {
                        ParameterName = parameter.Name,
                        ParameterUnit = parameter.DataUnit,
                        SpecificationLimits = limits,
                        Keys = new List<Key>(Keys)
                        {
                            new Key()
                            {
                                Name = "WAFER",
                                Value = logicalWafer?.Name ?? "-"
                            },
                            new Key()
                            {
                                Name = "PRODUCT",
                                Value = logicalWafer?.Product?.Name ?? lot.Product.Name
                            },
                            new Key()
                            {
                                Name = "PRODUCT VERSION",
                                Value = logicalWafer?.Product?.Version.ToString() ?? lot.Product.Version.ToString()
                            },
                            new Key()
                            {
                                Name = "PRODUCT TECHNOLOGY",
                                Value = logicalWafer?.Product?.ProductGroup?.Name ?? lot.Product.ProductGroup?.Name ?? "-"
                            },
                            new Key()
                            {
                                Name = "POSITION 1",
                                Value = waferContainer?.Position.Value.ToString() ?? "-"
                            },
                            new Key()
                            {
                                Name = "POSITION 2",
                                Value = "-"
                            },
                            new Key()
                            {
                                Name = "FLOW",
                                Value = logicalWafer?.Flow?.Name ?? lot.Flow.Name
                            },
                            new Key()
                            {
                                Name = "SINGLE PROCESS",
                                Value = stepLogicalName
                            },
                            new Key()
                            {
                                Name = "PROCESS EQUIPMENT CHAMBER",
                                Value = logicalWafer?.LastProcessStepResource?.Name ?? "-"
                            },
                            new Key()
                            {
                                Name = "MEASUREMENT EQUIPMENT CHAMBER",
                                Value = logicalWafer?.SystemState == MaterialSystemState.Processed ? logicalWafer?.LastProcessedResource?.Name ?? "-" : "-"
                            },
                            new Key()
                            {
                                Name = "WILDCARD EX1",
                                Value = logicalWafer == null ? mapSampleDCPoint.Key : "-"
                            },
                            new Key()
                            {
                                Name = "WILDCARD EX2",
                                Value = "-"
                            },
                            new Key()
                            {
                                Name = "CRYSTAL",
                                Value = logicalWafer?.SubMaterials?.FirstOrDefault(f => f.Form == "Wafer" && f.Type == "Crystal")?.Name ?? "-"
                            },
                            new Key()
                            {
                                Name = "SUBSTRATE",
                                Value = substrate?.Name ?? "-"
                            },
                            new Key()
                            {
                                Name = "CARRIER",
                                Value = logicalWafer?.SubMaterials?.FirstOrDefault(f => f.Form == "Wafer" && f.Type == "Carrier")?.Name ?? "-"
                            },
                            new Key()
                            {
                                Name = "VENDOR",
                                Value = substrate?.Supplier != null ? substrate.Supplier.Name : "-"
                            },
                        },
                        Raws = new Raws
                        {
                            StoreRaws = "True",
                            RawCollection = mapSampleDCPoint.Value.Select(s =>
                                new Raw
                                {
                                    RawValue = Decimal.Truncate(s)
                                }).ToList()
                        }
                    });
                }
            }

            return new CustomReportEDCToSpace()
            {
                SampleDate = DateTime.UtcNow.ToString("o"),
                Sender = new Sender()
                {
                    Value = Environment.MachineName
                },
                Lds = new List<Ld>()
                {
                    new Ld()
                    {
                       Id = ldsId ?? String.Empty
                    }
                },
                Samples = samples
            };
        }

        #endregion Space
    }
}
