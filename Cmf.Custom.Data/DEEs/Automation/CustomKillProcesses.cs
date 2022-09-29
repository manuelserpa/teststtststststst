using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cmf.Custom.amsOSRAM.Actions.Automation
{
    class CustomKillProcesses : DeeDevBase
	{
		/// <summary>
		/// DEE Action Code
		/// </summary>
		/// <param name="Input"></param>
		/// /// <param name="Input['ProcessName']">The name of the process we want to kill</param>
		/// /// <param name="Input['ExecutionPath']">The execution path where this process is running from</param>
		/// <returns>Input Message</returns>
		public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
		{
			//---Start DEE Code---

			UseReference("%MicrosoftNetPath%System.Diagnostics.Process.dll", "System.Diagnostics");
			UseReference("%MicrosoftNetPath%System.ComponentModel.Primitives.dll", "");

			try
			{
				string processName = string.Empty;
				string executionPath = string.Empty;

				if (!Input.ContainsKey("ProcessName"))
				{
					processName = "Node";
				}
				else
				{
					processName = Input["ProcessName"] as string;
				}

				if (!Input.ContainsKey("ExecutionPath"))
				{
					executionPath = @"C:\IoTTestsAutomationManagers";
				}
				else
				{
					executionPath = Input["ExecutionPath"] as string;
				}

				int numberOfProcessesKilled = 0;
				// Fetch all node processes running
				Process[] processes = Process.GetProcessesByName(processName);

				if (processes != null)
				{
					foreach (Process process in processes)
					{
						// retrieve the path where the process is running
						string processPath = process.MainModule.FileName;
						if (!string.IsNullOrWhiteSpace(processPath) && processPath.Contains(@executionPath))
						{
							process.Kill();
							numberOfProcessesKilled++;
						}
					}
				}

				Input["Result"] = string.Format("Number of node processes killed: {0}{1}", numberOfProcessesKilled, Environment.NewLine);
			}
			catch (Exception ex)
			{
				// we don't want to stop the execution of the IoT tests.
				// If it fails, it fails.
				Input["Result"] = ex.Message;
			}

			//---End DEE Code---
			return Input;
		}

		/// <summary>
		/// DEE Test Condition
		/// </summary>
		/// <param name="Input"></param>
		/// <returns></returns>
		public override bool DeeTestCondition(Dictionary<string, object> Input)
		{
			//---Start DEE Condition Code---

			#region Info

			/* Description:
             *     Dee action is triggered by IoT Tests to kill zombie node instances            
             *  
             * Action Groups:
             *      None
             *     
            */

			#endregion

			return true;

			//---End DEE Condition Code---
		}
	}
}
