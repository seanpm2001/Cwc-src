﻿
using cwc.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace cwc
{
	public class UpdateCwc{


		internal static void fLauchUpdate(string _sModuleFolder, string _sToFolder)
		{
			  LauchTool _oUpd =  new LauchTool();
			_oUpd.bOutput = false;
			_oUpd.UseShellExecute = true;
			_oUpd.fLauchExe( _sModuleFolder + "cwc.exe", " --self_update \"" + _sToFolder + "\"", "","",true);
		//		Console.WriteLine("Update to ver "  +_sVersion );
			while(!_oUpd.bExeLauched && Base.bAlive) {
				Thread.Sleep(1);
			}
			Thread.Sleep(1000);//Give time to show if we have info diplayed
			SysAPI.fQuit(true);
		}

		internal static void fRestart(string sCurrFolder)
		{
			  LauchTool _oUpd =  new LauchTool();
			_oUpd.bOutput = false;
			_oUpd.UseShellExecute = true;

           //  Output.Trace("\f0AVersion " + Data.sVersion + "\fs \n" );
		//	_oUpd.fLauchExe( sCurrFolder + "cwc.exe", "Updated ", "","",true);
		//	_oUpd.fLauchExe( sCurrFolder + "cwc.exe", "--message \"\f2ACwc succefully updated to\f2B v" + Data.sVersion + "\fs\"", "","",true);
			_oUpd.fLauchExe( sCurrFolder + "cwc.exe", "--updated \"" + Data.sVersion +"\"", "","",true);


			//_process.StartInfo.Arguments = "Updated " + _sVersion + " " + Data.sWorkDir + " " + Data.sResendArg;	
			while(!_oUpd.bExeLauched  && Base.bAlive) {
				Thread.Sleep(1);
			}	
			Thread.Sleep(1500);//Give time to show Msgs
			SysAPI.fQuit(true);
		}


		public static void fUpdateFiles(string _sSource) {
				Base.bAlive = true;
//Thread.Sleep(10000);
	
			  Output.TraceGood("Update Cwc to ver: " + Data.sUpdateVer);
				
			string _sBaseSrc = PathHelper.GetExeDirectory();
			string _sBaseDest = _sSource;


			// Output.TraceGood("Copy Tools: " +_sBaseSrc  + "Tools/"+   "    "+_sBaseDest + "Tools/");
            string _sUtilsErr = "";

			try {
				_sUtilsErr = FileUtils.CopyFolderContents(_sBaseSrc + "Utils/", _sBaseDest + "Utils/"); //TODO on run pass only?
			}catch(Exception e) {//Not used?
				Output.TraceError(e.Message);
                _sUtilsErr = e.Message;
			}

			Output.Trace("--- Copy Cwc ----");
			//Thread.Sleep(10);

			//Use Retry
			int _nCount = 16;
			int _nRetryCount = _nCount;
			string _sErrror = "";
			while(_nRetryCount > 0) {
				try {
					_sErrror = "";
					File.Copy(_sBaseSrc + "cwc.exe",_sBaseDest + "cwc.exe", true);
					Output.TraceGood("[cwc.exe] copied");
					//_nRetryCount  =0;
					break;
				}catch(Exception e) {
					if(_nRetryCount == _nCount - 1) {
						Output.TraceWarning(e.Message);
						Output.TraceWarning("Retrying...");
					}else {
						if(_nRetryCount != _nCount) {
							Output.TraceWarning(_nRetryCount.ToString());
						}
					}
					_nRetryCount--;
					Thread.Sleep(1000);
					//Output.TraceError(e.GetType().Name);//IOException
					
				    
					_sErrror = e.Message;
				}
			}
			if(_sErrror != "" || _sUtilsErr != "") {
                if(_sUtilsErr != "") {
                    Output.TraceError("Can't update Utils folder, please retry later...");
                }
                if(_sErrror != "") {
				    Output.TraceError("Can't update CWC, please retry later...");
                }
				Thread.Sleep(4000);
			}else {

				Output.TraceGood("--- Done ----");
                //Thread.Sleep(500);
                fRestart(_sBaseDest);
			}
			
			
	
		
			
				
				//Console.WriteLine("--- Done ----"); 
		}

		internal static void fUpdated(string _param){	
   
			if(_param ==  Data.sVersion) {
				 Output.Trace("\f2ACwc succefully updated to\f2B v" + Data.sVersion + "\fs\"");
				//CleanFolder
				Thread.Sleep(500); //Wait for update close //TODO add retry ??
				FileUtils.DeleteDirectory(PathHelper.GetExeDirectory() + "Upd_Cwc",true);
				FileUtils.DeleteDirectory(PathHelper.GetExeDirectory() + "Upd_CwcUtils",true);
			}else {
				Output.TraceError("Fail updating to: " + _param + ", current version: " + Data.sVersion);
				Output.TraceWarning("Current " + Data.sVersion);
			}

		}
	}
}
