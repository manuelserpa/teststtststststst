using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;

namespace Cmf.Custom.AMSOsram.Actions.Materials
{
    public class CustomSendHoldInformationToIoT : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.Cultures");
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.DataStructures");
            UseReference("", "System.Threading");
            //Please start code here

            MaterialCollection materials = new MaterialCollection();

            if (Input.ContainsKey("MaterialCollection") && Input["MaterialCollection"] != null)
            {
                materials.AddRange(Input["MaterialCollection"] as MaterialCollection);
            }

            foreach (Material material in materials)
            {
                List<MaterialData> materialDataToIot = new List<MaterialData>();

                if (material.SystemState != MaterialSystemState.InProcess &&
                    material.SubMaterialCount > 0)
                {
                    material.LoadChildren();
                    Material subMaterial = material.SubMaterials.FirstOrDefault();

                    if (subMaterial != null)
                    {
                        subMaterial.Load();
                        subMaterial.LoadRelations("MaterialContainer");

                        if (subMaterial.MaterialContainer != null &&
                            subMaterial.MaterialContainer.Count > 0)
                        {
                            MaterialContainer containerMaterial = subMaterial.MaterialContainer.FirstOrDefault();

                            if (containerMaterial != null)
                            {
                                Container currentContainer = containerMaterial.TargetEntity;
                                currentContainer.LoadRelations("ContainerResource");

                                if (currentContainer.ContainerResourceRelations != null &&
                                    currentContainer.ContainerResourceRelations.Count > 0)
                                {
                                    ContainerResource containerResource = currentContainer.ContainerResourceRelations.FirstOrDefault();

                                    if (containerResource != null)
                                    {
                                        Resource loadPort = containerResource.TargetEntity;
                                        Resource mainResource = loadPort.GetTopMostResource();

                                        if (mainResource.AutomationMode == ResourceAutomationMode.Online)
                                        {
                                            MaterialData materialData = new MaterialData();
                                            materialData.MaterialId = material.Id.ToString();
                                            materialData.MaterialName = material.Name;
                                            materialData.ContainerId = currentContainer.Id.ToString();
                                            materialData.ContainerName = currentContainer.Name;
                                            materialData.LoadPortPosition = loadPort.DisplayOrder.ToString();
                                            materialData.LoadPortName = loadPort.Name;
                                            materialDataToIot.Add(materialData);

                                            AutomationControllerInstance controllerInstance = mainResource.GetAutomationControllerInstance();

                                            if (controllerInstance != null)
                                            {
                                                // Send Asynchronous request to automation Hold the Material
                                                controllerInstance.Publish("Hold", materialDataToIot.ToJsonString());
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            //---End DEE Code---

            return Input;
        }

        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---
            return true;

            //---End DEE Condition Code---
        }
    }
}
