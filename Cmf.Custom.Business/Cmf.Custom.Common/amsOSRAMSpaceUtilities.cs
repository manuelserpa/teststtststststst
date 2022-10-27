using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.amsOSRAM.Common.DataStructures.CustomReportEDCToSpaceDto;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessObjects.Abstractions;
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
                employee = entityFactory.Create<IEmployee>();
                employee.Name = Utilities.DomainUserName;
            }

            if (employee.ObjectExists())
            {
                employee.Load();

                if (employee.Workgroup != null && employee.Workgroup.Name == null)
                {
                    employee.Workgroup.Load();

                    if (employee.Workgroup.ShiftPlan != null && employee.Workgroup.ShiftPlan.Name == null)
                    {
                        employee.Workgroup.ShiftPlan.Load();
                    }
                }
            }

            lot.Product.LoadAttribute(amsOSRAMConstants.ProductAttributeSAPProductType);

            string sapOwner = "-";
            if (lot.Product.Attributes.ContainsKey(amsOSRAMConstants.ProductAttributeSAPProductType))
            {
                lot.Product.Attributes.TryGetValueAs(amsOSRAMConstants.ProductAttributeSAPProductType, out sapOwner);
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
                    Value = lot.LastRecipe?.Name ?? "-"
                },
                new Key()
                {
                    Name = "MEASUREMENT EQUIPMENT",
                    Value = currentLotResource?.Name ?? "-"
                },
                new Key()
                {
                    Name = "MEASUREMENT RECIPE",
                    Value = !String.IsNullOrWhiteSpace(lot.CurrentRecipeInstance?.ParentEntity?.Name) ? lot.CurrentRecipeInstance.ParentEntity.Name : "-"
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
                    Value = employee.EmployeeNumber ?? "-"
                },
                new Key()
                {
                    Name = "SHIFT",
                    Value = employee.Workgroup?.ShiftPlan?.Name ?? "-"
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

            string ldCode = String.Empty;

            // TODO: We need this fallback?
            if (String.IsNullOrEmpty(ldCode) && lot.Facility.Site != null)
            {
                // Load Site
                ISite site = lot.Facility.Site;
                if (String.IsNullOrEmpty(site.Name))
                {
                    site.Load();
                }

                // Get SiteCode attribute value
                ldCode = site.GetAttributeValue(amsOSRAMConstants.CustomSiteCodeAttribute, true).ToString();
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

            IParameterCollection eligbleParameters = entityFactory.CreateCollection<IParameterCollection>();
            eligbleParameters.AddRange(parameters.Where(s =>
                s.HasAttribute(amsOSRAMConstants.CustomParameterSendToSpaceAttribute) &&
                s.GetAttributeValueOrDefault<bool>(amsOSRAMConstants.CustomParameterSendToSpaceAttribute, false) &&
                (s.DataType == ParameterDataType.Decimal || s.DataType == ParameterDataType.Long)));

            List<Sample> samples = new List<Sample>();

            foreach (IParameter parameter in eligbleParameters)
            {
                // Get the DC Points for the specific parameter
                IDataCollectionPointCollection points = entityFactory.CreateCollection<IDataCollectionPointCollection>();
                points.AddRange(dataCollectionInstance.DataCollectionPoints.Where(p => p.TargetEntity.Name.Equals(parameter.Name)));

                IDataCollectionPoint point = points.FirstOrDefault(f => f.TargetEntity.Name.Equals(parameter.Name));

                SpecificationLimits limits = new SpecificationLimits();

                if (limitSet.DataCollectionParameterLimits.Any(ls => ls.TargetEntity.Name.Equals(parameter.Name)))
                {
                    IDataCollectionParameterLimit parameterLimit = limitSet.DataCollectionParameterLimits.FirstOrDefault(ls => ls.TargetEntity.Name.Equals(parameter.Name));

                    if (parameterLimit != null)
                    {
                        if (parameterLimit.LowerErrorLimit != null && parameterLimit.UpperErrorLimit != null)
                        {
                            limits.Upper = parameterLimit.UpperErrorLimit.ToString();
                            limits.Lower = parameterLimit.LowerErrorLimit.ToString();
                        }

                        if (parameterLimit.LowerWarningLimit != null && parameterLimit.UpperWarningLimit != null)
                        {
                            limits.Upper = parameterLimit.UpperWarningLimit.ToString();
                            limits.Lower = parameterLimit.LowerWarningLimit.ToString();
                        }
                    }
                }

                IDataCollectionParameter dataCollectionParameter = dc.DataCollectionParameters.FirstOrDefault(f => f.TargetEntity.Name == parameter.Name);
                bool isSampleTypeMaterialId = dataCollectionParameter.SampleKey == DataCollectionParameterSampleKey.MaterialId;

                IMaterial logicalWafer = isSampleTypeMaterialId ? lot.SubMaterials.FirstOrDefault(f => f.Name == point.SampleId) : null;
                IMaterialContainer waferContainer = null;

                if (logicalWafer != null)
                {
                    logicalWafer.LoadRelations(Cmf.Navigo.Common.Constants.MaterialContainer);
                    waferContainer = logicalWafer.MaterialContainer?.FirstOrDefault(f => f.SourceEntity.Name == lot.Name);

                    logicalWafer.LoadChildren();
                }

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
                            Value = lot.Flow.Name
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
                            Value = logicalWafer.SystemState == MaterialSystemState.Processed ? logicalWafer?.LastProcessedResource?.Name ?? "-" : "-"
                        },
                        new Key()
                        {
                            Name = "WILDCARD EX1",
                            Value = !isSampleTypeMaterialId ? point.SampleId : "-"
                        },
                        new Key()
                        {
                            Name = "WILDCARD EX2",
                            Value = "-"
                        },
                        new Key()
                        {
                            Name = "CRYSTAL",
                            Value = logicalWafer?.SubMaterials.FirstOrDefault(f => f.Form == "Wafer" && f.Type == "Crystal")?.Name ?? "-"
                        },
                        new Key()
                        {
                            Name = "SUBSTRATE",
                            Value = logicalWafer?.SubMaterials.FirstOrDefault(f => f.Form == "Wafer" && f.Type == "Substrate")?.Name ?? "-"
                        },
                        new Key()
                        {
                            Name = "CARRIER",
                            Value = logicalWafer?.SubMaterials.FirstOrDefault(f => f.Form == "Wafer" && f.Type == "Carrier")?.Name ?? "-"
                        },
                        new Key()
                        {
                            Name = "VENDOR",
                            Value = logicalWafer?.Manufacturer != null && logicalWafer.Manufacturer.IsSupplier ? lot.Manufacturer.Name : "-"
                        },
                    },
                    Raws = new Raws
                    {
                        StoreRaws = "True",
                        RawCollection = points.Select(s =>
                            new Raw
                            {
                                RawValue = Convert.ToDecimal(s.Value)
                            }).ToList()
                    }
                });
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
                       Id = !String.IsNullOrWhiteSpace(ldCode) ? ldCode : String.Empty
                    }
                },
                Samples = samples
            };
        }

        #endregion Space
    }
}