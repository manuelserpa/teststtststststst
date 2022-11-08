using cmConnect.TestFramework.Environment;
using cmConnect.TestFramework.EquipmentSimulator.Drivers;
using Cmf.SECS.Driver;
using System;
using amsOSRAMEIAutomaticTests;
using System.Collections.Generic;
using Cmf.Foundation.BusinessObjects;
using cmConnect.TestFramework.ConnectIoT.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Cmf.Custom.TestUtilities;

namespace Cmf.Custom.Tests.IoT.Tests.HermosLFM4xReader
{
    public class HermosLFM4xReader : CommonTests
    {
        public Dictionary<String, String> targetIdRFID = new Dictionary<string, string>();
        public string ResourceName = "";
        private bool recievedS18F9 = false;

        private Dictionary<string, bool> recievedOnS18F9 = new Dictionary<string, bool>();
        private Dictionary<string, string> statusOnS18F9 = new Dictionary<string, string>();

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
                    AutomationDriverInstance driverInstance = new AutomationDriverInstance();
                    driverInstance.Load(((IoTEquipment)Equipment.BaseImplementation).EntityInstance.Id);

                    return (driverInstance.CommunicationState == AutomationCommunicationState.Communicating);
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

            recievedS18F9 = true;
            recievedOnS18F9.Add(targetId, true);

            string mid = targetIdRFID.FirstOrDefault(f => int.Parse(f.Key) == int.Parse(targetId)).Value;
            Log(String.Format("{0}: [S] Trying to read RFID Target ID {1} value {2}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), targetId, mid));


            reply.Item.Clear();

            reply.Item.SetTypeToList();
            var replyList = reply.Item;
            replyList.Add(new SecsItem() { ASCII = targetId }); //adds target id

            //SSACK code
            if (mid is null) //According to customer empty string is valid
            {
                replyList.Add(new SecsItem() { ASCII = "EE" });
            }
            else if (mid.Length == 2) {
                replyList.Add(new SecsItem() { ASCII = mid });
            }
            else {
                replyList.Add(new SecsItem() { ASCII = "NO" }); 
            }

            statusOnS18F9.Add(targetId, replyList[1].ASCII);


            replyList.Add(new SecsItem() { ASCII = mid }); // Material Id read (actual container id on MES)

            var statusList = new SecsItem();
            statusList.SetTypeToList();
            replyList.Add(statusList);
            return true;
        }

        public bool RecievedS18F9(string targetId = null) {

            if (targetId is not null && recievedOnS18F9.ContainsKey(targetId)) {
                return recievedOnS18F9[targetId];
            }

            return this.recievedS18F9;
        }

        public string TargetIdStatusS18F9(string targetId)
        {
            if (targetId is not null && statusOnS18F9.Keys.Contains(targetId))
            {
                return statusOnS18F9[targetId];
            }

            return null;
        }

        public List<string> GetRecievedTargetIds() {
            return recievedOnS18F9.Keys.ToList();
        }

        public void ClearFlags(string targetId = null) {
            if (targetId is not null)
            {
                if (recievedOnS18F9.ContainsKey(targetId))
                {
                    recievedOnS18F9.Remove(targetId);
                }

                if (statusOnS18F9.ContainsKey(targetId))
                {
                    statusOnS18F9.Remove(targetId);
                }
            }
            else {
                recievedOnS18F9.Clear();
                statusOnS18F9.Clear();
            }

            recievedS18F9 = false;
        }
    }
}
