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
    class HermosLFM4xReader : CommonTests
	{
         public Dictionary<String, String> PortId = new Dictionary<string, string>();

	     public void TestInit()
         {
            base.Equipment.RegisterOnMessage("S18F9", OnS18F9);

         }

        protected bool OnS18F9(SecsMessage request, SecsMessage reply)
        {
            var readerId = request.Item.GetChildList()[0];

            reply.Item.Clear();
            
            return true;
        }
    }
}
