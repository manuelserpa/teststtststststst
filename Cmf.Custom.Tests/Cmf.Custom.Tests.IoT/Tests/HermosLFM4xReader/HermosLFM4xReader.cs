using cmConnect.TestFramework.Environment;
using cmConnect.TestFramework.EquipmentSimulator.Drivers;
using Cmf.Navigo.BusinessObjects;
using Cmf.SECS.Driver;
using AMSOsramEIAutomaticTests.Objects.Extensions;
using System;
using System.Net;
using AMSOsramEIAutomaticTests;
using System.Collections.Generic;

namespace Cmf.Custom.Tests.IoT.Tests.HermosLFM4xReader
{
    public class HermosLFM4xReader : CommonTests
	{
        public Dictionary<String, String> targetIdRFID = new Dictionary<string, string>();
        public string ResourceName = "";

        public void TestInit(string resourceName, AutomationScenario scenario)
         {
            ResourceName = resourceName;
            Equipment = scenario.GetEquipment(resourceName) as SecsGemEquipment;
            base.Equipment.RegisterOnMessage("S18F9", OnS18F9);
         }

        protected bool OnS18F9(SecsMessage request, SecsMessage reply)
        {
            var targetId = request.Item.ASCII;

            string mid = targetIdRFID.GetValueOrDefault(targetId, "");
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
