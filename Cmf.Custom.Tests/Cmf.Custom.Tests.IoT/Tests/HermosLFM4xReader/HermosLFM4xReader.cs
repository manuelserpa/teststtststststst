using cmConnect.TestFramework.Environment;
using cmConnect.TestFramework.EquipmentSimulator.Drivers;
using Cmf.Navigo.BusinessObjects;
using Cmf.SECS.Driver;
using AMSOsramEIAutomaticTests.Objects.Extensions;
using System;
using System.Net;
using AMSOsramEIAutomaticTests;
using System.Collections.Generic;
using cmConnect.TestFramework.Common.Utilities;
using Cmf.Foundation.BusinessObjects;
using cmConnect.TestFramework.ConnectIoT.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Cmf.Custom.Tests.IoT.Tests.HermosLFM4xReader
{
    public class HermosLFM4xReader : CommonTests
	{
        public Dictionary<String, String> targetIdRFID = new Dictionary<string, string>();
        public string ResourceName = "";

        public void TestInit(string resourceName, AutomationScenario scenario)
         {
            targetIdRFID = new Dictionary<string, string>();
            ResourceName = resourceName;
            Equipment = scenario.GetEquipment(resourceName) as SecsGemEquipment;
            base.Equipment.RegisterOnMessage("S18F9", OnS18F9);

            try
            {
                cmConnect.TestFramework.Common.Utilities.TestUtilities.WaitFor(120, $"Driver never connected", () =>
                {
                    var instance = cmConnect.TestFramework.SystemRest.Utilities.SystemUtilities.GetObjectById<AutomationDriverInstance>(((IoTEquipment)Equipment.BaseImplementation).EntityInstance.Id);
                    return (instance.CommunicationState == AutomationCommunicationState.Communicating);
                });
            }
            catch
            {
                Assert.Inconclusive("Test could not connect to driver");
            }
         }

        protected bool OnS18F9(SecsMessage request, SecsMessage reply)
        {
            Log(String.Format("{0}: [S] Trying to read RFID Target ID {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), request.Item.ASCII));

            var targetId = request.Item.ASCII;

            string mid = targetIdRFID.FirstOrDefault(f => f.Key == targetId).Value;
            Log(String.Format("{0}: [S] Trying to read RFID Target ID {1} value {2}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), targetId, mid));


            reply.Item.Clear();

            reply.Item.SetTypeToList();
            var replyList = reply.Item;
            replyList.Add(new SecsItem() { ASCII = targetId }); //adds target id

            if(String.IsNullOrEmpty(mid) || mid.Length == 2) 
            {
                replyList.Add(new SecsItem() { ASCII = (mid.Length == 2 ? mid : "EE") }); //SSACK code
            }
            else
            {
                replyList.Add(new SecsItem() { ASCII = "NO" }); // SSACK code
            }

            replyList.Add(new SecsItem() { ASCII = mid }); // Material Id read (actual container id on MES)

            var statusList = new SecsItem();
            statusList.SetTypeToList();
            replyList.Add(statusList);
            return true;
        }
    }
}
