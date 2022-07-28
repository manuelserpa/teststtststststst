using System;
using System.Collections.Generic;
using System.Linq;
using ArtistInterface;
using AutoShellMessaging;
using GefasoftEngineering.CimProcessing;
using log4net;

namespace Cmf.Custom.OntoFDC
{
    public class FDC_API_Onto
    {
        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILog Log = CimProcLogger.CimPrcessingLogger;

        /// <summary>
        /// The FDC Server
        /// </summary>
        private readonly AshlServerLite myServer;

        private readonly string toolname;

        public bool FDCActive { get; private set; } = false;
        public bool FDCMandatory { get; private set; } = false;

        // private bool RegisterHandler = false;

        public FDC_API_Onto(bool fdcActive, bool fdcMandatory, string fdcServer, string fdcPort, string toolCode = "TEST")
        {
            Log.Debug($"FDC_API_Onto Constructor.");
            Log.Debug($"FDC_API_Onto FDCActive={fdcActive}");
            Log.Debug($"FDC_API_Onto FDCMandatory={fdcMandatory}");
            Log.Debug($"FDC_API_Onto FDCServer={fdcServer}:{fdcPort}");

            if (FDCActive)
            {
                if (string.IsNullOrEmpty(toolCode))
                {
                    toolname = "TEST";
                    Log.Error($"Toolcode is empty! Toolname { toolname } is used instead.");
                }
                else
                {
                    toolname = toolCode;    // e.g. 2FSPC4
                }

                MessagingSettings messagingSettings = new MessagingSettings
                {
                    AciConf = $"{ fdcServer }:{ fdcPort }",
                    CheckDuplicateRegistration = true,
                    UseDashForBackwardCompatibleAciNames = true,
                    UseInterfaceSelectionMethod = InterfaceSelectionMethod.DISCOVER,
                    InterfaceName = toolname,
                    KeepAliveIntervalMilliseconds = 30000,  // 30 seconds
                    // Any unique identifier must be used as Name. The first example uses local hostname, the second uses the toolname with the suffix EQC attached
                    //    messagingSettings.Name = System.Net.Dns.GetHostName();
                    Name = toolname + "_EQC"
                };
                Log.Debug($"FDC_API_Onto Constructor Toolcode: {toolname} connecting to: {messagingSettings.AciConf}.");

                // Now the connection the the FDC server is established.
                // This is an expensive operation (can take several seconds!)  The server instance is automatically reused by the AshlMessage class.    
                try
                {
                    myServer = AshlServerLite.getInstanceUsingParameters(messagingSettings);
                    if (!myServer.isActive())
                    {
                        Log.Error($"Error establishing connection to FDC server: ServerConnecion is not active.");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error establishing connection to FDC server: {ex.Message}");
                }
            }
        }



        /// <summary>
        /// This function shall be called to determine, if FDC is still available.
        /// </summary>
        public bool SendTalkConnectCheck()
        {
            // This method sends a "talkConnectCheck" command to the real-time server and reports back the results.
            // In the reply, the Artist database will return its product version information and build date.
            // Typically the application developer has no need for this information, but will simply use this as a periodic verification that communiations are working and the real-time server is receiving messages.

            TalkConnectCheck talkConnectCheck = new TalkConnectCheck();
            try
            {
                Log.Debug($"FDC start talkConnectCheck for toolname={toolname}");
                talkConnectCheck.send("rtSrv" + toolname, 10);

                Log.Debug($"FDC talkConnectCheck: {talkConnectCheck.MajorVer}|{talkConnectCheck.MinorVer}|{talkConnectCheck.Builder}|{talkConnectCheck.BuildDate}");
                return true;
            }
            catch (BoundMessageEncodeException ex)
            {
                Log.Debug($"BoundMessageEncodeException: {ex.Message}");
                return false;
            }
            catch (BoundMessageSendException ex)
            {
                Log.Debug($"BoundMessageSendException: {ex.Message}");
                return false;
            }
            catch (BoundMessageReplySyntaxException ex)
            {
                Log.Debug($"BoundMessageSendException: {ex.Message}");
                return false;
            }
        }



        /// <summary>
        /// Call this function, when the EQC is shutting down.
        /// </summary>
        public void DeRegisterFDC()
        {
            Log.Debug($"Deregister FDC_API_Onto.");
            // De-Register FDC:
            if (myServer.isActive())
            {
                try
                {
                    myServer.requestStopAndJoin();
                    Log.Debug($"De-registration successful.");
                }
                catch (Exception ex)
                {
                    Log.Error($"De-registration not successful: {ex.Message}");
                }
            } 
            else
            {
                Log.Debug($"AshlServerLite is not active, it will not de-register.");
            }
        }



        /// <summary>
        /// Calculates the Epoch Time from the current UTC Time
        /// </summary>
        private long CurrentTimeEpoch()
        {
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)timeSpan.TotalSeconds;
        }



        /// <summary>
        /// Sends the logistic data to Onto FDC system for a lot start (in) event.
        /// </summary>
        /// <param name="batchName">If BatchName is empty, it will be filled with the LotName parameter</param>
        /// <param name="lotName">The LotName of the wafer</param>
        /// <param name="recipe">The machine recipe</param>
        /// <param name="operation">The Operation (from MES) (Default="DummyOperation")</param>
        /// <param name="sps">The SPS (from MES) (Default="")</param>
        /// <param name="ProductName">The ProductName (from MES) (Default="")</param>
        /// <param name="ProductRoute">The ProductRoute (from MES) (Default="")</param>
        /// <param name="NumberOfWafersInBatch">Number of all wafers in the batch (from MES) (Default=0)</param>
        /// <param name="eventId">CEID the LotIn was started by.</param>
        /// <param name="remark">free text to transfer to FDC</param>
        /// /// <param name="chamber">The chamber of the tool, the wafer is going to be processed (default="")</param>
        /// <param name="eqChamberConnectString">The character which links the toolname with the chamber name (2FICP7_PM1 -> "_") (default = "-")</param>
        /// <param name="additionalLogisticTerms">Additional logistic terms can be defined as key-values pairs (Key = Logistic Term, Value = the value) (default = null)</param>
        public void SendFdcLotStart(string batchName, 
            string lotName, 
            string recipe, 
            string operation = "DummyOperation", 
            string sps = "", 
            string ProductName = "", 
            string ProductRoute = "", 
            int NumberOfWafersInBatch = 0, 
            long eventId = 0, 
            string remark = "",
            string chamber = "",
            string eqChamberConnectString = "-",
            Dictionary<string, string> additionalLogisticTerms = null,
            string owner = "")
        {
            // A startLot message must be sent before lot processing begins.
            // For single lot processing, notify that a lot is starting and send context information such as lot number, layer, device ID....

            if (!FDCActive) return;

            //string lLayer;
            string lBatchName;
            string lOperation;
            string lRecipe;

            // Mandatory input parameter checks:
            // lotName:
            if (string.IsNullOrEmpty(lotName))
            {
                Log.Error($"FDC logistic parameter lotName is empty. Function will not be called!");
                return;
            }

            // BatchName:
            if (string.IsNullOrEmpty(batchName))
            {
                lBatchName = lotName;
                Log.Debug($"batchName is empty -> lotName is sent for batchName instead.");
            }
            else
            {
                lBatchName = batchName;
            }

            // Operation:
            if (string.IsNullOrEmpty(operation))
            {
                lOperation = "DummyOperation";
            }
            else
            {
                lOperation = operation;
            }

            // Recipe:
            if (string.IsNullOrEmpty(recipe))
            {
                lRecipe = "UnknownRecipe";
            }
            else
            {
                lRecipe = recipe;
            }

            var toolnameWithChamber = GenerateToolnameWithChamber(chamber, eqChamberConnectString);

            Log.Info($"Send LotIn for tool={toolnameWithChamber}, CEID={eventId}");
            Log.Info($"                     LOT={lotName}");
            Log.Info($"                     LOGPT={lBatchName}");
            Log.Info($"                     OPN={lOperation}");
            Log.Info($"                     RECIPE={lRecipe}");
            if (!string.IsNullOrEmpty(sps))
                Log.Info($"                     SUBOPN={sps}");
            if (!string.IsNullOrEmpty(ProductName))
                Log.Info($"                     TYPE={ProductName}");
            if (!string.IsNullOrEmpty(ProductRoute))
                Log.Info($"                     ROUTE={ProductRoute}");
            if (NumberOfWafersInBatch != 0)
                Log.Info($"                     NUMSLICE={NumberOfWafersInBatch}");
            if (!string.IsNullOrEmpty(owner))
                Log.Info($"                     OWNER={owner}");

            if (additionalLogisticTerms != null)
            {
                foreach (KeyValuePair<string, string> additionalLogisticTerm in additionalLogisticTerms)
                {
                    if (!String.IsNullOrEmpty(additionalLogisticTerm.Value))
                    {
                        Log.Info($"                     additionalLogisticTerm:  Key = {additionalLogisticTerm.Key.ToUpper()}, Value = {additionalLogisticTerm.Value}");
                    } else {
                        Log.Warn($"                     additionalLogisticTerm: Value is null for Key = {additionalLogisticTerm.Key.ToUpper()}");
                    }
                }
            }

            DoAction doAction = new DoAction(toolnameWithChamber, CurrentTimeEpoch(), "LotIn");
            doAction.addTaggedParameter("LOT", lotName);                            // mandatory
            doAction.addTaggedParameter("LOGPT", lBatchName);                       // mandatory
            doAction.addTaggedParameter("OPN", lOperation);                         // mandatory
            if (!string.IsNullOrEmpty(sps))
                doAction.addTaggedParameter("SUBOPN", sps);                         // optional
            doAction.addTaggedParameter("RECIPE", lRecipe);                         // optional
            if (!string.IsNullOrEmpty(ProductName))
                doAction.addTaggedParameter("TYPE", ProductName);                   // optional
            if (!string.IsNullOrEmpty(ProductRoute))
                doAction.addTaggedParameter("ROUTE", ProductRoute);                 // optional
            if (NumberOfWafersInBatch != 0)
                doAction.addTaggedParameter("NUMSLICE", NumberOfWafersInBatch);     // optional
            if (!string.IsNullOrEmpty(owner))
                doAction.addTaggedParameter("OWNER", owner);                        // optional

            if (additionalLogisticTerms != null)
            {
                foreach (KeyValuePair<string, string> additionalLogisticTerm in additionalLogisticTerms)
                {
                    if (!String.IsNullOrEmpty(additionalLogisticTerm.Value))
                    {
                        doAction.addTaggedParameter(additionalLogisticTerm.Key.ToUpper(), additionalLogisticTerm.Value);     // optional
                    }
                }
            }

            doAction.setActionComment("LotIn event (CEID " + eventId + ") for " + lotName + " [" + remark + "]");
            try
            {
                doAction.send("rtSrv" + toolnameWithChamber);
            }
            catch (BoundMessageEncodeException ex)
            {
                Log.Error($"Message could not be build: required data was missing. {ex.Message}");
                return;
            }
            catch (BoundMessageSendException ex)
            {
                Log.Error($"Message could not be sent; check the Artist server If logging is enabled, look at 'verbose.log' for details about the problem. {ex.Message}");
                return;
            }

            Log.Info($"LotIn was sent successful for lot: {lotName}");
        }



        /// <summary>
        /// Sends the logistic data to Onto FDC system for a lot end (out) event.
        /// </summary>
        /// <param name="batchName">If BatchName is empty, it will be filled with the LotName parameter</param>
        /// <param name="lotName">The LotName of the wafer</param>
        /// <param name="operation">The Operation (from MES) (Default="DummyOperation")</param>
        /// <param name="statusOfBatch">Here you can provice the BatchState the batch will get in MES (e.g. FINISHED/ABORTED) (optional)</param>
        /// <param name="eventId">CEID the LotOut was started by.</param>
        /// <param name="remark">free text to transfer to FDC</param>
        /// <param name="chamber">The chamber of the tool, the wafer is going to be processed (default="")</param>
        /// <param name="eqChamberConnectString">The character which links the toolname with the chamber name (2FICP7_PM1 -> "_") (default = "-")</param>
        public void SendFdcLotEnd(string batchName, 
            string lotName, 
            string operation = "DummyOperation", 
            string statusOfBatch = "", 
            long eventId = 0, 
            string remark = "",
            string chamber = "",
            string eqChamberConnectString = "-")
        {
            // For single-lot processing, mark the end of a lot.
            // Lot information tags(lot, logpt and opn) in the endLot message must exactly those from the corresponding startLot message.

            if (!FDCActive) return;

            string lBatchName;
            string lOperation;

            // Mandatory input parameter checks:
            // lotName:
            if (string.IsNullOrEmpty(lotName))
            {
                Log.Error($"FDC logistic parameter lotName is empty. Function will not be called!");
                return;
            }

            // BatchName:
            if (string.IsNullOrEmpty(batchName))
            {
                lBatchName = lotName;
                Log.Debug($"batchName is empty -> lotName is sent for batchName instead.");
            }
            else
            {
                lBatchName = batchName;
            }

            // Operation:
            if (string.IsNullOrEmpty(operation))
            {
                lOperation = "DummyOperation";
            }
            else
            {
                lOperation = operation;
            }


            var toolnameWithChamber = GenerateToolnameWithChamber(chamber, eqChamberConnectString);

            Log.Info($"Send LotOut for tool={toolnameWithChamber}, CEID={eventId}");
            Log.Info($"                     LOT={lotName}");
            Log.Info($"                     LOGPT={lBatchName}");
            Log.Info($"                     OPN={lOperation}");
            Log.Info($"                     Status={statusOfBatch}");


            DoAction doAction = new DoAction(toolnameWithChamber, CurrentTimeEpoch(), "LotOut");
            doAction.addTaggedParameter("LOT", lotName);                // mandatory
            doAction.addTaggedParameter("LOGPT", lBatchName);           // mandatory
            doAction.addTaggedParameter("OPN", lOperation);             // mandatory

            doAction.setActionComment("LotOut event (CEID " + eventId + ") (" + statusOfBatch + ") for " + lotName + " [" + remark + "]");
            try
            {
                doAction.send("rtSrv" + toolnameWithChamber);
            }
            catch (BoundMessageEncodeException ex)
            {
                Log.Debug($"Message could not be build: required data was missing. {ex.Message}");
                return;
            }
            catch (BoundMessageSendException ex)
            {
                Log.Debug($"Message could not be sent; check the Artist server. If logging is enabled, look at 'verbose.log' for details about the problem. {ex.Message}");
                return;
            }

            Log.Info($"LotOut was sent successful for lot: {lotName}.");
        }



        /// <summary>
        /// Sends the logistic data to Onto FDC system for a wafer start (in) event.
        /// </summary>
        /// <param name="batchName">If BatchName is empty, it will be filled with the LotName instead</param>
        /// <param name="lotName">The LotName of the wafer (mandatory)</param>
        /// <param name="fdcWaferInfo">The wafer structure (with at least waferName - which is mandatory) which you want to find in FDC</param>
        /// <param name="chamber">The chamber of the tool, the wafer is going to be processed (default="")</param>
        /// <param name="eqChamberConnectString">The character which links the toolname with the chamber name (2FICP7_PM1 -> "_") (default = "-")</param>
        /// <param name="chamberRecipe">The chamber recipe, if different from the machine recipe (optional)</param>
        /// <param name="eventId">CEID the WaferIn was started by.</param>
        /// <param name="remark">free text to transfer to FDC</param>
        public void SendFdcWaferIn(string batchName, 
            string lotName, 
            FdcWaferInfo fdcWaferInfo, 
            string chamber = "", 
            string eqChamberConnectString = "-", 
            string chamberRecipe = "", 
            long eventId = 0, 
            string remark = "")
        {
            // A sendWaferIn message must be sent before wafer processing begins in the specified chamber.
            // The basetime of the WaferIn message should be at least 1 second after the previous startLot

            if (!FDCActive) return;

            string lBatchName;
            //string lEqChamberConnectString;

            // Input parameter checks:
            // lotName (mandatory):
            if (string.IsNullOrEmpty(lotName))
            {
                Log.Error($"FDC logistic parameter lotName is empty. Function will not be called!");
                return;
            }

            // waferId (mandatory):
            if (string.IsNullOrEmpty(fdcWaferInfo.WaferName))
            {
                Log.Error($"FDC logistic parameter waferId is empty. Function will not be called!");
                return;
            }

            // BatchName:
            if (string.IsNullOrEmpty(batchName))
            {
                lBatchName = lotName;
                Log.Debug($"batchName is empty -> lotName is sent for batchName instead.");
            }
            else
            {
                lBatchName = batchName;
            }

            string toolnameChamber = GenerateToolnameWithChamber(chamber, eqChamberConnectString);

            /*
            if (string.IsNullOrEmpty(eqChamberConnectString))
            {
                lEqChamberConnectString = "-";
            }
            else
            {
                lEqChamberConnectString = eqChamberConnectString;
            }

            string toolnameChamber = toolname;
            // Assign logistic data to subequipment:
            if (!string.IsNullOrEmpty(chamber))
            {
                toolnameChamber += lEqChamberConnectString + chamber;
            }
            */

            Log.Info($"Send WaferIn for tool={toolnameChamber}, CEID={eventId}");
            Log.Info($"                     LOGPT={lBatchName}");
            Log.Info($"                     PROC_LOT={lotName}");
            Log.Info($"                     PROC_WAFERID={fdcWaferInfo.WaferName}");
            if (fdcWaferInfo.SlotPos != null && fdcWaferInfo.SlotPos != 0)
                Log.Info($"                     PROC_SLOT={fdcWaferInfo.SlotPos}");
            if (!string.IsNullOrEmpty(chamber))
                Log.Info($"                     CHAMBER={chamber}");
            if (!string.IsNullOrEmpty(chamberRecipe))
                Log.Info($"                     CHAMBERRECIPE={chamberRecipe}");
            if (fdcWaferInfo.LotPos != null && fdcWaferInfo.LotPos != 0)
                Log.Info($"                     LOTPOS={fdcWaferInfo.LotPos}");
            if (!string.IsNullOrEmpty(fdcWaferInfo.CarrierGravure))
                Log.Info($"                     CARRIERGRAVURE={fdcWaferInfo.CarrierGravure}");
            if (!string.IsNullOrEmpty(fdcWaferInfo.Gravure))
                Log.Info($"                     GRAVURE={fdcWaferInfo.Gravure}");
            if (!string.IsNullOrEmpty(fdcWaferInfo.VendorName))
                Log.Info($"                     VENDORNAME={fdcWaferInfo.VendorName}");
            if (!string.IsNullOrEmpty(fdcWaferInfo.Frame))
                Log.Info($"                     FRAME={fdcWaferInfo.Frame}");
            if (!string.IsNullOrEmpty(fdcWaferInfo.WaferSize))
                Log.Info($"                     WAFERSIZE={fdcWaferInfo.WaferSize}");
            if (fdcWaferInfo.QtyIn != null && fdcWaferInfo.QtyIn != 0)
                Log.Info($"                     QTY_IN={fdcWaferInfo.QtyIn}");
            if (!string.IsNullOrEmpty(fdcWaferInfo.WaferRecipe))
                Log.Info($"                     WAFER_RECIPE={fdcWaferInfo.WaferRecipe}");
            if (!string.IsNullOrEmpty(fdcWaferInfo.BondingBoat))
                Log.Info($"                     BONDING_BOAT={fdcWaferInfo.BondingBoat}");
            if (fdcWaferInfo.IsDummy)
                Log.Info($"                     IS_DUMMY={fdcWaferInfo.IsDummy}");
            if (!string.IsNullOrEmpty(remark))
                Log.Info($"                     Remark={remark}");


            foreach (KeyValuePair<string, string> additionalLogisticTerm in fdcWaferInfo.AdditionalLogisticTerms)
            {
                if (!String.IsNullOrEmpty(additionalLogisticTerm.Value))
                {
                    Log.Info($"                     additionalLogisticTerm:  Key = {additionalLogisticTerm.Key.ToUpper()}, Value = {additionalLogisticTerm.Value}");
                } else {
                    Log.Warn($"                     additionalLogisticTerm: Value is null for Key = {additionalLogisticTerm.Key.ToUpper()}");
                }
            }


            DoAction doAction = new DoAction(toolnameChamber, CurrentTimeEpoch(), "WaferIn");
            doAction.addTaggedParameter("LOGPT", lBatchName);                               // mandatory input parameter in FDC
            doAction.addTaggedParameter("PROC_LOT", lotName);                               // mandatory input parameter in FDC
            doAction.addTaggedParameter("PROC_WAFERID", fdcWaferInfo.WaferName);            // mandatory input parameter in FDC
            if (fdcWaferInfo.SlotPos != null && fdcWaferInfo.SlotPos != 0)
                doAction.addTaggedParameter("PROC_SLOT", fdcWaferInfo.SlotPos);             // optional
            if (!string.IsNullOrEmpty(chamber))
                doAction.addTaggedParameter("CHAMBER", chamber);                            // optional
            if (!string.IsNullOrEmpty(chamberRecipe))
                doAction.addTaggedParameter("CHAMBERRECIPE", chamberRecipe);                // optional
            if (fdcWaferInfo.LotPos != null && fdcWaferInfo.LotPos != 0)
                doAction.addTaggedParameter("LOTPOS", fdcWaferInfo.LotPos);                 // optional
            if (!string.IsNullOrEmpty(fdcWaferInfo.CarrierGravure))
                doAction.addTaggedParameter("CARRIERGRAVURE", fdcWaferInfo.CarrierGravure); // optional
            if (!string.IsNullOrEmpty(fdcWaferInfo.Gravure))
                doAction.addTaggedParameter("GRAVURE", fdcWaferInfo.Gravure);               // optional
            if (!string.IsNullOrEmpty(fdcWaferInfo.VendorName))
                doAction.addTaggedParameter("VENDORNAME", fdcWaferInfo.VendorName);         // optional
            if (!string.IsNullOrEmpty(fdcWaferInfo.Frame))
                doAction.addTaggedParameter("FRAME", fdcWaferInfo.Frame);                   // optional
            if (!string.IsNullOrEmpty(fdcWaferInfo.WaferSize))
                doAction.addTaggedParameter("WAFERSIZE", fdcWaferInfo.WaferSize);           // optional
            if (fdcWaferInfo.QtyIn != null && fdcWaferInfo.QtyIn != 0)
                doAction.addTaggedParameter("QTY_IN", fdcWaferInfo.QtyIn);                  // optional
            if (!string.IsNullOrEmpty(fdcWaferInfo.WaferRecipe))
                doAction.addTaggedParameter("WAFER_RECIPE", fdcWaferInfo.WaferRecipe);      // optional
            if (!string.IsNullOrEmpty(fdcWaferInfo.BondingBoat))
                doAction.addTaggedParameter("BONDING_BOAT", fdcWaferInfo.BondingBoat);      // optional
            if (fdcWaferInfo.IsDummy)
                doAction.addTaggedParameter("IS_DUMMY", fdcWaferInfo.IsDummy);              // optional

            foreach (KeyValuePair<string, string> additionalLogisticTerm in fdcWaferInfo.AdditionalLogisticTerms)
            {
                if (!String.IsNullOrEmpty(additionalLogisticTerm.Value))
                {
                    doAction.addTaggedParameter(additionalLogisticTerm.Key.ToUpper(), additionalLogisticTerm.Value);     // optional
                }
            }

            doAction.setActionComment("WaferIn event (CEID " + eventId + ") for wafer " + fdcWaferInfo.WaferName + " of lot " + lotName + " [" + remark + "]");
            try
            {
                //Log.Debug($"{doAction.ToString()}");
                doAction.send("rtSrv" + toolnameChamber);
            }
            catch (BoundMessageEncodeException ex)
            {
                Log.Error($"Message could not be build: required data was missing. {ex.Message}");
                return;
            }
            catch (BoundMessageSendException ex)
            {
                Log.Error($"Message could not be sent; check the Artist server. If logging is enabled, look at 'verbose.log' for details about the problem. {ex.Message}");
                return;
            }

            Log.Info($"WaferIn was sent successful for lot: {lotName} - Wafer: {fdcWaferInfo.WaferName}.");

        }



        /// <summary>
        /// Sends the logistic data to Onto FDC system for a wafer end (out) event.
        /// </summary>
        /// <param name="batchName">If BatchName is empty, it will be filled with the LotName instead</param>
        /// <param name="lotName">The LotName of the wafer (mandatory)</param>
        /// <param name="fdcWaferInfo">The wafer structure (with at least waferName - which is mandatory) which you want to find in FDC</param>
        /// <param name="chamber">The chamber of the tool, the wafer is going to be processed (default="")</param>
        /// <param name="eqChamberConnectString">The character which links the toolname with the chamber name (2FICP7_PM1 -> "_") (default = "-")</param>
        /// <param name="eventId">CEID the WaferOut was started by.</param>
        /// <param name="remark">free text to transfer to FDC</param>
        public void SendFdcWaferOut(string batchName, 
            string lotName, 
            FdcWaferInfo fdcWaferInfo, 
            string chamber = "", 
            string eqChamberConnectString = "-", 
            long eventId = 0, 
            string remark = "")
        {
            // A sendWaferOut message must be sent after wafer processing and in the specified chamber.
            // The the basetime of the WaferOut message should be at least 1 second before the next WaferIn or endLot.

            if (!FDCActive) return;

            string lBatchName;
            //string lEqChamberConnectString;

            // Input parameter checks:
            // lotName (mandatory):
            if (string.IsNullOrEmpty(lotName))
            {
                Log.Error($"FDC logistic parameter lotName is empty. Function will not be called!");
                return;
            }

            // waferId (mandatory):
            if (string.IsNullOrEmpty(fdcWaferInfo.WaferName))
            {
                Log.Error($"FDC logistic parameter waferId is empty. Function will not be called!");
                return;
            }

            // BatchName:
            if (string.IsNullOrEmpty(batchName))
            {
                lBatchName = lotName;
                Log.Debug($"batchName is empty -> lotName is sent for batchName instead.");
            }
            else
            {
                lBatchName = batchName;
            }


            string toolnameChamber = GenerateToolnameWithChamber(chamber, eqChamberConnectString);

            /*
            if (string.IsNullOrEmpty(eqChamberConnectString))
            {
                lEqChamberConnectString = "-";
            }
            else
            {
                lEqChamberConnectString = eqChamberConnectString;
            }

            string toolnameChamber = toolname;
            // Assign logistic data to subequipment:
            if (!string.IsNullOrEmpty(chamber))
            {
                toolnameChamber += lEqChamberConnectString + chamber;
            }
            */


            Log.Info($"Send WaferOut for tool={toolnameChamber}, CEID={eventId}");
            Log.Info($"                     LOGPT={lBatchName}");
            Log.Info($"                     PROC_LOT={lotName}");
            Log.Info($"                     PROC_WAFERID={fdcWaferInfo.WaferName}");
            if (!string.IsNullOrEmpty(chamber))
                Log.Info($"                     CHAMBER={chamber}");
            if (fdcWaferInfo.ReadQuality != null && fdcWaferInfo.ReadQuality != -1)
                Log.Info($"                     READQUALITY={fdcWaferInfo.ReadQuality}");
            if (fdcWaferInfo.Processed)
                Log.Info($"                     PROCESSED={fdcWaferInfo.Processed}");
            if (!string.IsNullOrEmpty(fdcWaferInfo.WaferState))
                Log.Info($"                     WAFERSTATE={fdcWaferInfo.WaferState}");
            if (!string.IsNullOrEmpty(remark))
                Log.Info($"                     Remark={remark}");

            foreach (KeyValuePair<string, string> additionalLogisticTerm in fdcWaferInfo.AdditionalLogisticTerms)
            {
                if (!String.IsNullOrEmpty(additionalLogisticTerm.Value))
                {
                    Log.Info($"                     additionalLogisticTerm:  Key = {additionalLogisticTerm.Key.ToUpper()}, Value = {additionalLogisticTerm.Value}");
                }
                else
                {
                    Log.Warn($"                     additionalLogisticTerm: Value is null for Key = {additionalLogisticTerm.Key.ToUpper()}");
                }
            }

            DoAction doAction = new DoAction(toolnameChamber, CurrentTimeEpoch(), "WaferOut");
            doAction.addTaggedParameter("LOGPT", lBatchName);                           // mandatory input parameter in FDC
            doAction.addTaggedParameter("PROC_LOT", lotName);                           // mandatory input parameter in FDC
            doAction.addTaggedParameter("PROC_WAFERID", fdcWaferInfo.WaferName);        // mandatory input parameter in FDC
            if (!string.IsNullOrEmpty(chamber))
                doAction.addTaggedParameter("CHAMBER", chamber);                        // optional
            if (fdcWaferInfo.ReadQuality != null && fdcWaferInfo.ReadQuality != -1)
                doAction.addTaggedParameter("READQUALITY", fdcWaferInfo.ReadQuality);   // optional
            if (fdcWaferInfo.Processed)
                doAction.addTaggedParameter("PROCESSED", fdcWaferInfo.Processed);       // optional
            if (!string.IsNullOrEmpty(fdcWaferInfo.WaferState))
                doAction.addTaggedParameter("WAFERSTATE", fdcWaferInfo.WaferState);       // optional

            foreach (KeyValuePair<string, string> additionalLogisticTerm in fdcWaferInfo.AdditionalLogisticTerms)
            {
                if (!String.IsNullOrEmpty(additionalLogisticTerm.Value))
                {
                    doAction.addTaggedParameter(additionalLogisticTerm.Key.ToUpper(), additionalLogisticTerm.Value);     // optional
                }
            }

            doAction.setActionComment("WaferOut event (CEID " + eventId + ") for wafer " + fdcWaferInfo.WaferName + " of lot " + lotName + " [" + remark + "]");
            try
            {
                //Log.Debug($"{doAction.ToString()}");
                doAction.send("rtSrv" + toolnameChamber);
            }
            catch (BoundMessageEncodeException ex)
            {
                Log.Error($"Message could not be build: required data was missing. {ex.Message}");
                return;
            }
            catch (BoundMessageSendException ex)
            {
                Log.Error($"Message could not be sent; check the Artist server. If logging is enabled, look at 'verbose.log' for details about the problem. {ex.Message}");
                return;
            }

            Log.Info($"WaferOut was sent successful for lot: {lotName} - Wafer: {fdcWaferInfo.WaferName}.");

        }



        /// <summary>
        /// Sends the logistic data to Onto FDC system for wafer start (in) event with multiple wafers starting at the same time.
        /// </summary>
        /// <param name="batchName">If BatchName is empty, it will be filled with the LotName instead</param>
        /// <param name="lotName">The LotName of the wafer (mandatory)</param>
        /// <param name="fdcWaferInfoList">A list of wafer structure (with at least waferName - which is mandatory) which you want to find in FDC</param>
        /// <param name="chamber">The chamber of the tool, the wafer is going to be processed (default="")</param>
        /// <param name="eqChamberConnectString">The character which links the toolname with the chamber name (2FICP7_PM1 -> "_") (default = "-")</param>
        /// <param name="chamberRecipe">The chamber recipe, if different from the machine recipe (optional)</param>
        /// <param name="eventId">CEID the WaferInMulti was started by.</param>
        /// <param name="remark">free text to transfer to FDC</param>
        public void SendFdcWaferInMulti(string batchName, 
            string lotName, 
            List<FdcWaferInfo> fdcWaferInfoList, 
            string chamber = "", 
            string eqChamberConnectString = "-", 
            string chamberRecipe = "", 
            long eventId = 0, 
            string remark = "")
        {
            // A sendWaferIn message must be sent before wafer processing begins in the specified chamber.
            // The basetime of the WaferIn message should be at least 1 second after the previous startLot

            if (!FDCActive) return;

            string lBatchName;
            string lEqChamberConnectString;

            // Input parameter checks:
            // lotName (mandatory):
            if (string.IsNullOrEmpty(lotName))
            {
                Log.Error($"FDC logistic parameter lotName is empty. Function will not be called!");
                return;
            }

            // waferId (mandatory):
            if (fdcWaferInfoList == null || fdcWaferInfoList.Count == 0)
            {
                Log.Error($"FDC logistic parameter fdcWaferInfoList is empty. Function will not be called!");
                return;
            }

            // BatchName:
            if (string.IsNullOrEmpty(batchName))
            {
                lBatchName = lotName;
                Log.Debug($"batchName is empty -> lotName is sent for batchName instead.");
            }
            else
            {
                lBatchName = batchName;
            }

            // Generate comma separated lists for use in API call:
            string lWaferNameList = String.Join(",", fdcWaferInfoList.Select(m => m.WaferName).ToArray());
            string lSlotPosList = String.Join(",", fdcWaferInfoList.Select(m => m.SlotPos).ToArray());
            string lLotPosList = String.Join(",", fdcWaferInfoList.Select(m => m.LotPos).ToArray());
            string lCarrierGravureList = String.Join(",", fdcWaferInfoList.Select(m => m.CarrierGravure).ToArray());
            string lGravureList = String.Join(",", fdcWaferInfoList.Select(m => m.Gravure).ToArray());
            string lVendorNameList = String.Join(",", fdcWaferInfoList.Select(m => m.VendorName).ToArray());
            string lFrameList = String.Join(",", fdcWaferInfoList.Select(m => m.Frame).ToArray());
            string lWaferSizeList = String.Join(",", fdcWaferInfoList.Select(m => m.WaferSize).ToArray());
            string lQtyinList = String.Join(",", fdcWaferInfoList.Select(m => m.QtyIn).ToArray());
            string lWaferRecipeList = String.Join(",", fdcWaferInfoList.Select(m => m.WaferRecipe).ToArray());
            string lBondingBoatList = String.Join(",", fdcWaferInfoList.Select(m => m.BondingBoat).ToArray());
            string lIsDummyList = String.Join(",", fdcWaferInfoList.Select(m => m.IsDummy).ToArray());


            if (string.IsNullOrEmpty(eqChamberConnectString))
            {
                lEqChamberConnectString = "-";
            }
            else
            {
                lEqChamberConnectString = eqChamberConnectString;
            }

            string toolnameChamber = toolname;
            // Assign logistic data to subequipment:
            if (!string.IsNullOrEmpty(chamber))
            {
                toolnameChamber += lEqChamberConnectString + chamber;
            }


            Log.Info($"Send WaferInMulti for tool={toolnameChamber}, CEID={eventId}");
            Log.Info($"                     LOGPT={lBatchName}");
            Log.Info($"                     PROC_LOT={lotName}");
            Log.Info($"                     PROC_WAFERID={lWaferNameList}");
            if (!string.IsNullOrEmpty(lSlotPosList.Replace(",", "").Replace("0", "").Trim()))
                Log.Info($"                     PROC_SLOT={lSlotPosList}");
            if (!string.IsNullOrEmpty(chamber))
                Log.Info($"                     CHAMBER={chamber}");
            if (!string.IsNullOrEmpty(chamberRecipe))
                Log.Info($"                     CHAMBERRECIPE={chamberRecipe}");
            if (!string.IsNullOrEmpty(lLotPosList.Replace(",", "").Replace("0", "").Trim()))
                Log.Info($"                     LOTPOS={lLotPosList}");
            if (!string.IsNullOrEmpty(lCarrierGravureList.Replace(",", "").Trim()))
                Log.Info($"                     CARRIERGRAVURE={lCarrierGravureList}");
            if (!string.IsNullOrEmpty(lGravureList.Replace(",", "").Trim()))
                Log.Info($"                     GRAVURE={lGravureList}");
            if (!string.IsNullOrEmpty(lVendorNameList.Replace(",", "").Trim()))
                Log.Info($"                     VENDORNAME={lVendorNameList}");
            if (!string.IsNullOrEmpty(lFrameList.Replace(",", "").Trim()))
                Log.Info($"                     FRAME={lFrameList}");
            if (!string.IsNullOrEmpty(lWaferSizeList.Replace(",", "").Trim()))
                Log.Info($"                     WAFERSIZE={lWaferSizeList}");
            if (!string.IsNullOrEmpty(lQtyinList.Replace(",", "").Replace("0", "").Trim()))
                Log.Info($"                     QTY_IN={lQtyinList}");
            if (!string.IsNullOrEmpty(lWaferRecipeList.Replace(",", "").Trim()))
                Log.Info($"                     WAFER_RECIPE={lWaferRecipeList}");
            if (!string.IsNullOrEmpty(lBondingBoatList.Replace(",", "").Trim()))
                Log.Info($"                     BONDING_BOAT={lBondingBoatList}");
            if (!string.IsNullOrEmpty(lIsDummyList.Replace(",", "").Replace("False", "").Trim())) 
                Log.Info($"                     IS_DUMMY={lIsDummyList}");
            if (!string.IsNullOrEmpty(remark))
                Log.Info($"                     Remark={remark}");


            DoAction doAction = new DoAction(toolnameChamber, CurrentTimeEpoch(), "WaferInMulti");
            doAction.addTaggedParameter("LOGPT", lBatchName);                               // mandatory input parameter in FDC
            doAction.addTaggedParameter("PROC_LOT", lotName);                               // mandatory input parameter in FDC
            doAction.addTaggedParameter("PROC_WAFERID", lWaferNameList);                    // mandatory input parameter in FDC
            if (!string.IsNullOrEmpty(lSlotPosList.Replace(",", "").Replace("0", "").Trim()))
                doAction.addTaggedParameter("PROC_SLOT", lSlotPosList);                     // optional
            if (!string.IsNullOrEmpty(chamber))
                doAction.addTaggedParameter("CHAMBER", chamber);                            // optional
            if (!string.IsNullOrEmpty(chamberRecipe))
                doAction.addTaggedParameter("CHAMBERRECIPE", chamberRecipe);                // optional
            if (!string.IsNullOrEmpty(lLotPosList.Replace(",", "").Replace("0", "").Trim()))
                doAction.addTaggedParameter("LOTPOS", lLotPosList);                         // optional
            if (!string.IsNullOrEmpty(lCarrierGravureList.Replace(",", "").Trim()))
                doAction.addTaggedParameter("CARRIERGRAVURE", lCarrierGravureList);         // optional
            if (!string.IsNullOrEmpty(lGravureList.Replace(",", "").Trim()))
                doAction.addTaggedParameter("GRAVURE", lGravureList);                       // optional
            if (!string.IsNullOrEmpty(lVendorNameList.Replace(",", "").Trim()))
                doAction.addTaggedParameter("VENDORNAME", lVendorNameList);                 // optional
            if (!string.IsNullOrEmpty(lFrameList.Replace(",", "").Trim()))
                doAction.addTaggedParameter("FRAME", lFrameList);                           // optional
            if (!string.IsNullOrEmpty(lWaferSizeList.Replace(",", "").Trim()))
                doAction.addTaggedParameter("WAFERSIZE", lWaferSizeList);                   // optional
            if (!string.IsNullOrEmpty(lQtyinList.Replace(",", "").Replace("0", "").Trim()))
                doAction.addTaggedParameter("QTY_IN", lQtyinList);                          // optional
            if (!string.IsNullOrEmpty(lWaferRecipeList.Replace(",", "").Trim()))
                doAction.addTaggedParameter("WAFER_RECIPE", lWaferRecipeList);              // optional
            if (!string.IsNullOrEmpty(lBondingBoatList.Replace(",", "").Trim()))
                doAction.addTaggedParameter("BONDING_BOAT", lBondingBoatList);              // optional
            if (!string.IsNullOrEmpty(lIsDummyList.Replace(",", "").Replace("False", "").Trim()))
                doAction.addTaggedParameter("IS_DUMMY", lIsDummyList);                      // optional

            doAction.setActionComment("WaferInMulti event (CEID " + eventId + ") for wafers " + lWaferNameList + " of lot " + lotName + " [" + remark + "]");
            try
            {
                //Log.Debug($"{doAction.ToString()}");
                doAction.send("rtSrv" + toolnameChamber);
            }
            catch (BoundMessageEncodeException ex)
            {
                Log.Error($"Message could not be build: required data was missing. {ex.Message}");
                return;
            }
            catch (BoundMessageSendException ex)
            {
                Log.Error($"Message could not be sent; check the Artist server. If logging is enabled, look at 'verbose.log' for details about the problem. {ex.Message}");
                return;
            }

            Log.Info($"WaferInMulti was sent successful for lot: {lotName} - Wafer: {lWaferNameList}.");

        }



        /// <summary>
        /// Sends the logistic data to Onto FDC system for a wafer end (out) event with multiple wafers finished at the same time.
        /// </summary>
        /// <param name="batchName">If BatchName is empty, it will be filled with the LotName instead</param>
        /// <param name="lotName">The LotName of the wafer (mandatory)</param>
        /// <param name="fdcWaferInfoList">A list of wafer structure (with at least waferName - which is mandatory) which you want to find in FDC</param>
        /// <param name="chamber">The chamber of the tool, the wafer is going to be processed (default="")</param>
        /// <param name="eqChamberConnectString">The character which links the toolname with the chamber name (2FICP7_PM1 -> "_") (default = "-")</param>
        /// <param name="eventId">CEID the WaferOutMulti was started by.</param>
        /// <param name="remark">free text to transfer to FDC</param>
        public void SendFdcWaferOutMulti(string batchName, 
            string lotName, 
            List<FdcWaferInfo> fdcWaferInfoList, 
            string chamber = "", 
            string eqChamberConnectString = "-", 
            long eventId = 0, 
            string remark = "")
        {
            // A sendWaferOut message must be sent after wafer processing and in the specified chamber.
            // The the basetime of the WaferOut message should be at least 1 second before the next WaferIn or endLot.

            if (!FDCActive) return;

            string lBatchName;
            string lEqChamberConnectString;

            // Input parameter checks:
            // lotName (mandatory):
            if (string.IsNullOrEmpty(lotName))
            {
                Log.Error($"FDC logistic parameter lotName is empty. Function will not be called!");
                return;
            }

            // waferId (mandatory):
            if (fdcWaferInfoList == null || fdcWaferInfoList.Count == 0)
            {
                Log.Error($"FDC logistic parameter waferNameList is empty. Function will not be called!");
                return;
            }

            // BatchName:
            if (string.IsNullOrEmpty(batchName))
            {
                lBatchName = lotName;
                Log.Debug($"batchName is empty -> lotName is sent for batchName instead.");
            }
            else
            {
                lBatchName = batchName;
            }


            // Generate comma separated lists for use in API call:
            string lWaferNameList = String.Join(",", fdcWaferInfoList.Select(m => m.WaferName).ToArray());
            string lReadQualityList = String.Join(",", fdcWaferInfoList.Select(m => m.ReadQuality).ToArray());
            string lProcessedList = String.Join(",", fdcWaferInfoList.Select(m => m.Processed).ToArray());
            string lWaferStateList = String.Join(",", fdcWaferInfoList.Select(m => m.WaferState).ToArray());

            if (string.IsNullOrEmpty(eqChamberConnectString))
            {
                lEqChamberConnectString = "-";
            }
            else
            {
                lEqChamberConnectString = eqChamberConnectString;
            }

            string toolnameChamber = toolname;
            // Assign logistic data to subequipment:
            if (!string.IsNullOrEmpty(chamber))
            {
                toolnameChamber += lEqChamberConnectString + chamber;
            }


            Log.Info($"Send WaferOutMulti for tool={toolnameChamber}, CEID={eventId}");
            Log.Info($"                     LOGPT={lBatchName}");
            Log.Info($"                     PROC_LOT={lotName}");
            Log.Info($"                     PROC_WAFERID={lWaferNameList}");
            if (!string.IsNullOrEmpty(chamber))
                Log.Info($"                     CHAMBER={chamber}");
            if (!string.IsNullOrEmpty(lReadQualityList.Replace(",", "").Replace("-1", "").Trim()))
                Log.Info($"                     READQUALITY={lReadQualityList}");
            if (!string.IsNullOrEmpty(lProcessedList.Replace(",", "").Replace("False", "").Trim()))
                Log.Info($"                     PROCESSED={lProcessedList}");
            if (!string.IsNullOrEmpty(lWaferStateList.Replace(",", "").Trim()))
                Log.Info($"                     WAFERSTATE={lWaferStateList}");
            if (!string.IsNullOrEmpty(remark))
                Log.Info($"                     Remark={remark}");


            foreach (FdcWaferInfo fdcWaferInfo in fdcWaferInfoList)
            {
                foreach (KeyValuePair<string, string> additionalLogisticTerm in fdcWaferInfo.AdditionalLogisticTerms)
                {
                    Log.Warn($"                     additionalLogisticTerm will be ignored by SendFdcWaferInMulti:  Key = {additionalLogisticTerm.Key.ToUpper()}, Value = {additionalLogisticTerm.Value}");
                }
            }


            DoAction doAction = new DoAction(toolnameChamber, CurrentTimeEpoch(), "WaferOutMulti");
            doAction.addTaggedParameter("LOGPT", lBatchName);                               // mandatory input parameter in FDC
            doAction.addTaggedParameter("PROC_LOT", lotName);                               // mandatory input parameter in FDC
            doAction.addTaggedParameter("PROC_WAFERID", lWaferNameList);                    // mandatory input parameter in FDC
            if (!string.IsNullOrEmpty(chamber))
                doAction.addTaggedParameter("CHAMBER", chamber);                            // optional
            if (!string.IsNullOrEmpty(lReadQualityList.Replace(",", "").Replace("-1", "").Trim()))
                doAction.addTaggedParameter("READQUALITY", lReadQualityList);               // optional
            if (!string.IsNullOrEmpty(lProcessedList.Replace(",", "").Replace("False", "").Trim()))
                doAction.addTaggedParameter("PROCESSED", lProcessedList);                   // optional
            if (!string.IsNullOrEmpty(lWaferStateList.Replace(",", "").Trim()))
                doAction.addTaggedParameter("WAFERSTATE", lWaferStateList);                 // optional


            doAction.setActionComment("WaferOutMulti event (CEID " + eventId + ") for wafers " + lWaferNameList + " of lot " + lotName + " [" + remark + "]");
            try
            {
                //Log.Debug($"{doAction.ToString()}");
                doAction.send("rtSrv" + toolnameChamber);
            }
            catch (BoundMessageEncodeException ex)
            {
                Log.Error($"Message could not be build: required data was missing. {ex.Message}");
                return;
            }
            catch (BoundMessageSendException ex)
            {
                Log.Error($"Message could not be sent; check the Artist server. If logging is enabled, look at 'verbose.log' for details about the problem. {ex.Message}");
                return;
            }

            Log.Info($"WaferOutMulti was sent successful for lot: {lotName} - Wafer: {lWaferNameList}.");

        }

        public void SendFdcRecipeStepStart(string batchName,
            string lotName,
            string waferName,
            string chamber = "",
            string eqChamberConnectString = "-",
            string recipeStepNr = "",
            string recipeStepName = "",
            long eventId = 0,
            string remark = "")
        {
            // A RecipeStepStart message must be sent before step processing begins in the specified chamber.

            if (!FDCActive) return;

            string lBatchName;

            // Input parameter checks:
            // lotName (mandatory):
            if (string.IsNullOrEmpty(lotName))
            {
                Log.Error($"FDC logistic parameter lotName is empty. Function will not be called!");
                return;
            }

            // waferId (mandatory):
            if (string.IsNullOrEmpty(waferName))
            {
                Log.Error($"FDC logistic parameter waferId is empty. Function will not be called!");
                return;
            }

            // BatchName:
            if (string.IsNullOrEmpty(batchName))
            {
                lBatchName = lotName;
                Log.Debug($"batchName is empty -> lotName is sent for batchName instead.");
            }
            else
            {
                lBatchName = batchName;
            }

            // RecipeStepNr (mandatory):
            if (string.IsNullOrEmpty(recipeStepNr))
            {
                Log.Error($"FDC logistic parameter RecipeStepNr is empty. Function will not be called!");
                return;
            }

            string toolnameChamber = GenerateToolnameWithChamber(chamber, eqChamberConnectString);


            Log.Info($"Send RecipeStepStart for tool={toolnameChamber}, CEID={eventId}");
            Log.Info($"                     LOGPT={lBatchName}");
            Log.Info($"                     PROC_LOT={lotName}");
            Log.Info($"                     PROC_WAFERID={waferName}");
            Log.Info($"                     RECIPESTEPNR={recipeStepNr}");
            if (!string.IsNullOrEmpty(recipeStepName))
                Log.Info($"                     RECIPESTEPNAME={recipeStepName}");
            if (!string.IsNullOrEmpty(remark))
                Log.Info($"                     Remark={remark}");




            DoAction doAction = new DoAction(toolnameChamber, CurrentTimeEpoch(), "RecipeStepStart");
            doAction.addTaggedParameter("LOGPT", lBatchName);                               // mandatory input parameter in FDC
            doAction.addTaggedParameter("PROC_LOT", lotName);                               // mandatory input parameter in FDC
            doAction.addTaggedParameter("PROC_WAFERID", waferName);                         // mandatory input parameter in FDC
            doAction.addTaggedParameter("RECIPESTEPNR", recipeStepNr);                 // mandatory input parameter in FDC

            if (!string.IsNullOrEmpty(recipeStepName))
                doAction.addTaggedParameter("RECIPESTEPNAME", recipeStepName);             // optional


            doAction.setActionComment("RecipeStepStart event (CEID " + eventId + ") for wafer " + waferName + " of lot " + lotName + " [" + remark + "] with RECIPESTEPNR " + recipeStepNr);
            try
            {
                //Log.Debug($"{doAction.ToString()}");
                doAction.send("rtSrv" + toolnameChamber);
            }
            catch (BoundMessageEncodeException ex)
            {
                Log.Error($"Message could not be build: required data was missing. {ex.Message}");
                return;
            }
            catch (BoundMessageSendException ex)
            {
                Log.Error($"Message could not be sent; check the Artist server. If logging is enabled, look at 'verbose.log' for details about the problem. {ex.Message}");
                return;
            }

            Log.Info($"RecipeStepStart was sent successful for lot: {lotName} - Wafer: {waferName} + recipeStepNr {recipeStepNr}.");

        }

        public void SendFdcRecipeStepEnd(string batchName,
            string lotName,
            string waferName,
            string chamber = "",
            string eqChamberConnectString = "-",
            string recipeStepNr = "",
            long eventId = 0,
            string remark = "")
        {
            // A sendWaferOut message must be sent after wafer processing and in the specified chamber.
            // The the basetime of the WaferOut message should be at least 1 second before the next WaferIn or endLot.

            if (!FDCActive) return;

            string lBatchName;
            //string lEqChamberConnectString;

            // Input parameter checks:
            // lotName (mandatory):
            if (string.IsNullOrEmpty(lotName))
            {
                Log.Error($"FDC logistic parameter lotName is empty. Function will not be called!");
                return;
            }

            // waferId (mandatory):
            if (string.IsNullOrEmpty(waferName))
            {
                Log.Error($"FDC logistic parameter waferId is empty. Function will not be called!");
                return;
            }

            // BatchName:
            if (string.IsNullOrEmpty(batchName))
            {
                lBatchName = lotName;
                Log.Debug($"batchName is empty -> lotName is sent for batchName instead.");
            }
            else
            {
                lBatchName = batchName;
            }

            // RecipeStepNr (mandatory):
            if (string.IsNullOrEmpty(recipeStepNr))
            {
                Log.Error($"FDC logistic parameter RecipeStepNr is empty. Function will not be called!");
                return;
            }


            string toolnameChamber = GenerateToolnameWithChamber(chamber, eqChamberConnectString);

         


            Log.Info($"Send RecipeStepEnd for tool={toolnameChamber}, CEID={eventId}");
            Log.Info($"                     LOGPT={lBatchName}");
            Log.Info($"                     PROC_LOT={lotName}");
            Log.Info($"                     PROC_WAFERID={waferName}");
            Log.Info($"                     RECIPESTEPNR={recipeStepNr}");
            if (!string.IsNullOrEmpty(remark))
                Log.Info($"                     Remark={remark}");

            DoAction doAction = new DoAction(toolnameChamber, CurrentTimeEpoch(), "RecipeStepEnd");
            doAction.addTaggedParameter("LOGPT", lBatchName);                           // mandatory input parameter in FDC
            doAction.addTaggedParameter("PROC_LOT", lotName);                           // mandatory input parameter in FDC
            doAction.addTaggedParameter("PROC_WAFERID", waferName);        // mandatory input parameter in FDC
            doAction.addTaggedParameter("RECIPESTEPNR", recipeStepNr);                 // mandatory input parameter in FDC


            doAction.setActionComment("RecipeStepEnd event (CEID " + eventId + ") for wafer " + waferName + " of lot " + lotName + " [" + remark + "] with recipeStepNr " + recipeStepNr);
            try
            {
                //Log.Debug($"{doAction.ToString()}");
                doAction.send("rtSrv" + toolnameChamber);
            }
            catch (BoundMessageEncodeException ex)
            {
                Log.Error($"Message could not be build: required data was missing. {ex.Message}");
                return;
            }
            catch (BoundMessageSendException ex)
            {
                Log.Error($"Message could not be sent; check the Artist server. If logging is enabled, look at 'verbose.log' for details about the problem. {ex.Message}");
                return;
            }

            Log.Info($"RecipeStepEnd was sent successful for lot: {lotName} - Wafer: {waferName} - recipeStepNr: {recipeStepNr} .");

        }


        private string GenerateToolnameWithChamber(
            string chamber,
            string eqChamberConnectString)
        {
            var lEqChamberConnectString = "";

            if (string.IsNullOrEmpty(eqChamberConnectString))
            {
                lEqChamberConnectString = "-";
            }
            else
            {
                lEqChamberConnectString = eqChamberConnectString;
            }

            string toolnameChamber = toolname;
            // Assign logistic data to subequipment:
            if (!string.IsNullOrEmpty(chamber))
            {
                toolnameChamber += lEqChamberConnectString + chamber;
            }

            return toolnameChamber;
        }


        ///// <summary>
        ///// Transmit trace signal data points to Onto FDC system.
        ///// </summary>
        ///// <param name="chamber">The chamber of the tool, the wafer is going to be processed (default="")</param>
        ///// <param name="eqChamberConnectString">The character which links the toolname with the chamber name (2FICP7_PM1 -> "_") (default = "-")</param>
        //private void SendFdcPutData(string chamber = "",
        //    string eqChamberConnectString = "-")
        //{
        //    // Transmit trace signal data points to EquipmentSentinel.
        //    // Multiple signals are typically included in a single putdata message.

        //    if (string.IsNullOrEmpty(eqChamberConnectString))
        //    {
        //        lEqChamberConnectString = "-";
        //    }
        //    else
        //    {
        //        lEqChamberConnectString = eqChamberConnectString;
        //    }

        //    string toolnameChamber = toolname;
        //    // Assign logistic data to subequipment:
        //    if (!string.IsNullOrEmpty(chamber))
        //    {
        //        toolnameChamber += eqChamberConnectString + chamber;
        //    }


        //    Random rnd = new Random();
        //    PutData dataMsg = new PutData(toolname, CurrentTimeEpoch());

        //    DataSet dataSet = new DataSet("ChamberPressureA", DataType.NUMBER);
        //    dataSet.addDataSample(new DataSample(0, rnd.NextDouble() * 50 + 70, DataType.NUMBER));
        //    dataMsg.addDataSet(dataSet);

        //    dataSet = new DataSet("ChamberPressureB", DataType.NUMBER);
        //    dataSet.addDataSample(new DataSample(0, rnd.NextDouble() * 10 + 20, DataType.NUMBER));
        //    dataMsg.addDataSet(dataSet);

        //    dataSet = new DataSet("ChamberTemperature", DataType.NUMBER);
        //    dataSet.addDataSample(new DataSample(0, rnd.NextDouble() * 400 + 450, DataType.NUMBER));
        //    dataMsg.addDataSet(dataSet);

        //    try
        //    {
        //        //Log.Debug($"{dataMsg.ToString()}");
        //        dataMsg.send("rtSrv" + toolnameChamber);

        //    }
        //    catch (BoundMessageEncodeException ex)
        //    {
        //        Log.Debug($"Message could not be build: required data was missing. {ex.Message}");
        //    }
        //    catch (BoundMessageSendException ex)
        //    {
        //        Log.Debug($"Message could not be sent; check the Artist server. If logging is enabled, look at 'verbose.log' for details about the problem. {ex.Message}");
        //    }

        //    Log.Info($"FdcPutData was sent successful.");
        //}



        //// Notify FDC of a tool event or alarm; or, any other abnormal condition that should be reported.
        //private void SendFdcDoAction(object sender, EventArgs e)
        //{
        //    DoAction doAction = new DoAction(toolname, CurrentTimeEpoch(), "ExternalEvent");
        //    doAction.addTaggedParameter("LOT", "Lot001");
        //    doAction.setActionComment("tool fired an event");
        //    try
        //    {
        //        doAction.send("rtSrv" + toolname);

        //    }
        //    catch (BoundMessageEncodeException ex)
        //    {
        //        Log.Debug($"Message could not be build: required data was missing. {ex.Message}");
        //    }
        //    catch (BoundMessageSendException ex)
        //    {
        //        Log.Debug($"Message could not be sent; check the Artist server. If logging is enabled, look at 'verbose.log' for details about the problem. {ex.Message}");
        //    }
        //}



    }
}
