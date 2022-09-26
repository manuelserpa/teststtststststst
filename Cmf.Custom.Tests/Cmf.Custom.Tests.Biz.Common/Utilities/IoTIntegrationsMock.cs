using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessOrchestration.DataPlatform.Domain;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.Common.Utilities
{
    public static class IoTIntegrationsMock
    {
        public static void SmartLoadPortInput(
            string resourceName = "SLP0100", 
            int loadPortNumber = 2,
            string containerName = "")
        {
            // Change State to Occupied
            new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput
            {
                Action = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.GetActionByNameInput
                {
                    Name = "CustomAutomationAdjustLoadPortState"
                }.GetActionByNameSync().Action,
                Input = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "ResourceName", resourceName },
                        { "LoadPortNumber", loadPortNumber.ToString() },
                        { "StateName", "Occupied" }
                    }
            }.ExecuteActionSync();

            // Dock Container
            new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput
            {
                Action = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.GetActionByNameInput
                {
                    Name = "CustomDockStoreIoT"
                }.GetActionByNameSync().Action,
                Input = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "CarrierId", containerName },
                        { "ResourceId", resourceName },
                        { "LoadPortNumber", loadPortNumber.ToString() },
                        { "FromOnlineLoadPort", true }
                    }
            }.ExecuteActionSync();

            // Change State to Ready to Unload
            new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput
            {
                Action = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.GetActionByNameInput
                {
                    Name = "CustomAutomationAdjustLoadPortState"
                }.GetActionByNameSync().Action,
                Input = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "ResourceName", resourceName },
                        { "LoadPortNumber", loadPortNumber.ToString() },
                        { "StateName", "ReadyToUnload" },
                        { "CarrierID", containerName }
                    }
            }.ExecuteActionSync();

        }

        public static void LiftContainerFromLoadPort(Container container)
        {
            Resource currentLocation = null;
            container.LoadRelations(new Collection<string>() { "ContainerResource" });

            if (container.RelationCollection != null &&
                container.RelationCollection.ContainsKey("ContainerResource") &&
                container.RelationCollection["ContainerResource"].Count > 0)
            {
                currentLocation = (container.RelationCollection["ContainerResource"].FirstOrDefault() as ContainerResource).TargetEntity;
                currentLocation.Load();
            }
            // Get Parent Resource
            var getAscendentResourcesOutput = new Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects.GetAscendentResourcesInput
            {
                Depth = 1,
                Resource = currentLocation
            }.GetAscendentResourcesSync();
            string resourceName = getAscendentResourcesOutput.AscendentResources.FirstOrDefault().ParentResource.Name;

            // Change State to Available
            new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput
            {
                Action = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.GetActionByNameInput
                {
                    Name = "CustomAutomationAdjustLoadPortState"
                }.GetActionByNameSync().Action,
                Input = new System.Collections.Generic.Dictionary<string, object>
                {
                    { "ResourceName", resourceName },
                    { "LoadPortNumber", currentLocation.DisplayOrder.ToString() },
                    { "StateName", "Available" },
                    { "CarrierID", container.Name }
                }
            }.ExecuteActionSync();
        }

        public static void MCSTransport(IoTEvent ioTEvent)
        {
            // Try to read information from ioTEvent
        }

        public static void MCSTransport(Container container, string destination)
        {
            Resource currentLocation = null;
            container.LoadRelations(new Collection<string>() { "ContainerResource" });

            if (container.RelationCollection != null &&
                container.RelationCollection.ContainsKey("ContainerResource") &&
                container.RelationCollection["ContainerResource"].Count > 0)
            {
                currentLocation = (container.RelationCollection["ContainerResource"].FirstOrDefault() as ContainerResource).TargetEntity;
                currentLocation.Load();
            }

            // Pick Container
            if (currentLocation != null)
            {
                // if current location is a load port
                // and automated
                if (currentLocation.ProcessingType == ProcessingType.LoadPort &&
                    currentLocation.AutomationMode == ResourceAutomationMode.Online)
                {
                    // Get Parent Resource
                    var getAscendentResourcesOutput = new Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects.GetAscendentResourcesInput
                    {
                        Depth = 1,
                        Resource = currentLocation
                    }.GetAscendentResourcesSync();
                    string resourceName = getAscendentResourcesOutput.AscendentResources.FirstOrDefault().ParentResource.Name;
                    // Change State to Available
                    new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput
                    {
                        Action = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.GetActionByNameInput
                        {
                            Name = "CustomAutomationAdjustLoadPortState"
                        }.GetActionByNameSync().Action,
                        Input = new System.Collections.Generic.Dictionary<string, object>
                        {
                            { "ResourceName", resourceName },
                            { "LoadPortNumber", currentLocation.DisplayOrder.ToString() },
                            { "StateName", "Available" },
                            { "CarrierID", container.Name }
                        }
                    }.ExecuteActionSync();
                }
                else
                {
                    // Retrieve/Umdowck Container
                    new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput
                    {
                        Action = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.GetActionByNameInput
                        {
                            Name = "CustomUndockRetrieveIoT"
                        }.GetActionByNameSync().Action,
                        Input = new System.Collections.Generic.Dictionary<string, object>
                        {
                            { "CarrierId", container.Name }
                        }
                    }.ExecuteActionSync();
                }
            }

            Resource targetLocation = new Resource();
            targetLocation.Load(destination);

            if (targetLocation.ProcessingType == ProcessingType.LoadPort &&
                targetLocation.AutomationMode == ResourceAutomationMode.Online)
            {
                // Get Parent Resource
                var getAscendentResourcesOutput = new Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects.GetAscendentResourcesInput
                {
                    Depth = 1,
                    Resource = targetLocation
                }.GetAscendentResourcesSync();
                string resourceName = getAscendentResourcesOutput.AscendentResources.FirstOrDefault().ParentResource.Name;

                // Change State to Occupied
                new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput
                {
                    Action = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.GetActionByNameInput
                    {
                        Name = "CustomAutomationAdjustLoadPortState"
                    }.GetActionByNameSync().Action,
                    Input = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "ResourceName", resourceName },
                        { "LoadPortNumber", targetLocation.DisplayOrder.ToString() },
                        { "StateName", "Occupied" }
                    }
                }.ExecuteActionSync();

                // Dock Container
                new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput
                {
                    Action = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.GetActionByNameInput
                    {
                        Name = "CustomDockStoreIoT"
                    }.GetActionByNameSync().Action,
                    Input = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "CarrierId", container.Name },
                        { "ResourceId", resourceName },
                        { "LoadPortNumber", targetLocation.DisplayOrder.ToString() },
                        { "FromOnlineLoadPort", true }
                    }
                }.ExecuteActionSync();
            }
            else 
            {
                // Store Container
                new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput
                {
                    Action = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.GetActionByNameInput
                    {
                        Name = "CustomDockStoreIoT"
                    }.GetActionByNameSync().Action,
                    Input = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "CarrierId", container.Name },
                        { "ResourceId", targetLocation.Name },
                    }
                }.ExecuteActionSync();
            }


        }

        public static List<IoTEvent> GetGeneratedIoTEvents(long fromId = 0)
        {
            var eventList = new Cmf.Foundation.BusinessOrchestration.DataPlatform.InputObjects.GetIoTEventQueueInfoInput
            {
                IoTEventTimeframeFilter = Foundation.BusinessOrchestration.DataPlatform.Domain.Enumerations.IoTEventTimeframe.LastHour,

            }.GetIoTEventQueueInfoSync().IotEvents;
            return eventList.Where(E => long.Parse(E.EventId) > fromId).ToList();
        }

        public static long ValidateNumberOfIoTEvents(long maxEventId = 0, int maxNumberOfEvents = 1, string assertComment = "")
        {
            var iotEvents = IoTIntegrationsMock.GetGeneratedIoTEvents(maxEventId);
            Assert.AreEqual(maxNumberOfEvents, iotEvents.Count(), "IoT Event Triggered " + assertComment);
            return long.Parse(iotEvents.Max(E => E.EventId));
        }
    }
}
