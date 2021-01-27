﻿using cwc.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cwc {
    class Data {


          public static string sVersion = "0.0.95.26";

        		internal static bool bUpdateMode = false;
		internal static string sUpdateModeSrc = "";
		internal static string sUpdateVer;
		public static bool bNothingToBuild = false;


      

        public static string sTRUE = "1";
        public static string sFALSE = "0";
        public static string fGetStrBool(bool _bCond){
            if (_bCond) {
                return sTRUE;
            } else {
                return sFALSE;
            }
        }
       public static bool fIsDataTrue(string _sName) {
            if (aVarGlobal.ContainsKey(_sName)) {
               return aVarGlobal[_sName] == Data.sTRUE;
            }
            if (aOption.ContainsKey(_sName)) {
               return aOption[_sName] == Data.sTRUE;
            }
            return false;
        }


        public static bool bModeIDE = false;

           public static string[] CmdArgs;




        public static  List<ArgumentManager> aAll_ArgumentManager = new List<ArgumentManager>();
         public static List<string> aAllInclude = new List<string>();

 

        public static string sArg = "";
        public static string sArgExpand = "";

        public static string sCmd = "";

                public static int nDontKillId = 0;
       public static ArgumentManager oArg;
        public static ArgumentManager oCompilerArg = new ArgumentManager();

        		//public static SelectForm oSelectForm;

		public static GuiConsole oGuiConsole = null;

        
		public static Dictionary<string,  string> aVarGlobal = new Dictionary<string, string>();
		public static Dictionary<string,  string> aOption= new Dictionary<string, string>(); //Same as varGlobal but its saved settings
	

        		
		public static List<String> aBrowser = new List<String>() ;
		public static List<String> aBrowserVersion = new List<String>() ;

        public static bool bNowBuilding = false;
	 public static bool bDontExecute = false;

        public  static Process MainProcess =  Process.GetCurrentProcess();
        public  static bool bConsoleMode = false;
       public static ConfigMng oConfigMng = null;
        

        internal static void fCreateConfigMng() {
           	oConfigMng = new ConfigMng();
			oConfigMng.LoadConfig();
        }

        public  static bool bInGUI = false;


   //     public  static string sCurrFolder =  Directory.GetCurrentDirectory();
      //  public  static MainForm oMainForm = null;

        public  static Empty oMsgForm = null;
		public  static LauchProject oLauchProject = new LauchProject();

      public static Dictionary<string, ModuleData> aCompilerData = new Dictionary<string, ModuleData>();

		public static Form oMsgBox = null;
		public static Form oMainUpdateForm = null;


		public static bool bWaitGUI = false;
		internal static bool bStartWithMessage;
		internal static bool bNonBuildCommand = false;

		public static bool bShowLibRT { get; internal set; }

		public static string sShowModule = "";

	//	public static  List<ModuleData> aModuleUsed  = new List<ModuleData>();

		public static  List<string> aRequiredModule  = new List<string>();
		public static  List<string> aModuleList  = new List<string>();
		public static bool bGUI = true;
		internal static bool bColor = true;

		public static ModeIDE oModeIDE = null;
		public static int nCloseOnId = 0;
        public static bool bModuleIsRequired = false;
        public static bool bToolchainDefined = false;

		public static ModuleData fAddRequiredModule(string _sName, bool _bIsCompiler = false)  {
    

			ModuleData _oModule = ModuleData.fGetModule(_sName,_bIsCompiler);
			  
			string _sLastVersion = _oModule.fGetLastVersions();
		//	string _sFolder = _oModuleLibRT.sOutFolder + _oModuleLibRT.sPrefixFolder + _sLastVersion;

			//Debug.fTrace("Last: " + _sFolder);

		//	if( !Directory.Exists(_oModuleLibRT.sFolder) ) {//Double verification?
			if(aCompilerData.ContainsKey(_sName)){ //Bug if already exist
				aCompilerData.Remove(_sName);
			}
			aCompilerData.Add(_sName, _oModule);
       

			if( _oModule.sCurrFolder == "" ) { //Not exist, we required downlaod

				//Is exist?
				////////////
         
                bModuleIsRequired = true;
				bStartWithMessage = true;
				//bDontExecute = true;

				foreach(string _sModule in aRequiredModule) {
					if(_sName == _sModule) {
						return _oModule;
					}
				}
			
				if(_bIsCompiler){
					 Output.Trace("\f4CRequired compiler: " +  _sName);
				}else{
					Output.Trace("\f4CRequired lib: " +  _sName);
				}
				//		 Output.Trace("\f1B --- End --- \f13 " +   _nSeconde + "." + _nDotSeconde + " sec" );
				aRequiredModule.Add(_sName);

			}
			return _oModule;
		}


		public static string sCurrViewIn = "";
		internal static void fAddBrowser(string _sBrowser, string _sVersion)	{
			if(aBrowser.Count == 0){
				sCurrViewIn = _sBrowser;
			}
			aBrowser.Add(_sBrowser);


            /*
			if(oMainForm != null){
				oMainForm.fUpdateBrowser();
			}*/
		}
			
		internal static string fGetViewIn()	{
          string _sVar =  Data.fGetGlobalVar("_sViewIn");
            if(_sVar != "") {
                return _sVar;
            }

			return sCurrViewIn;
		}

        
         public static void fClearData(){
            Data.aVarGlobal.Clear();
             Data.fSetDefaultVar();
            if(Data.oGuiConsole != null) {
                Data.oGuiConsole.fLoadData();
            }


            ModuleData.aMData.Clear();

             CppCompiler.aOutput.Clear();
             ArgumentManager.aDependance.Clear();
        }

              public static string sToLauch = "";
        internal static bool bForceTestNextCmd;
        internal static bool bIWantGoToEnd = false;

        internal static void fCheckUpdate() {
           
////////////////// UPDATER ///////////////////
			if(CmdArgs != null && CmdArgs.Length > 2 && CmdArgs[0] == "Updated") {
			
				bDontExecute = true;
				if( CmdArgs[1].Trim() == sVersion) {
					MessageBox.Show("Update Success: " + CmdArgs[1] );
				}else {
					MessageBox.Show("Update Fail: " + CmdArgs[1] + " Current Version: " + sVersion );
				}
				if (Directory.Exists(PathHelper.Updater)){
					 FileUtils.DeleteDirectory(PathHelper.Updater);
				}
				PathHelper.ExeWorkDir = CmdArgs[2];
				CmdArgs = CmdArgs.Skip(3).ToArray(); //shift
			}	
///////////////////////////////////
        }

        internal static void fGetMainArg() {
               Data.sArg = Environment.CommandLine.Trim() ;

             if(sArg[0] == '\"'){ //Remove current file arg when loaded from file
                int _nFindEndQuote =  sArg.IndexOf("\"",1)+1;
                sArg = sArg.Substring( _nFindEndQuote ,sArg.Length-_nFindEndQuote).Trim();
            }
				
				//////////// Process ARGS ///////////////
				string _stdArg =  Data.sArg;
				//remvoe all before first "-"
				int _first_arg = _stdArg.IndexOf("-");
				if(_first_arg == -1) {
					 _stdArg = "";
				}else {
					Data.sArg = _stdArg.Substring(0,_first_arg);
					_stdArg = _stdArg.Substring(_first_arg);
				}
				ArgList.ProcessArg(_stdArg);
				/////////////////////////////////////////

				//////////////////Remove itself//////////////////
				Data.sArg = Data.sArg.Trim();
				string _sArgL = Data.sArg.ToLower();
				int _idx_cwc = _sArgL.IndexOf("cwc.exe");
				if (_idx_cwc != -1) {
					Data.sArg = "";
				}
				 _idx_cwc = _sArgL.LastIndexOf("cwc");
				if (_idx_cwc == _sArgL.Length - "cwc".Length) {
					Data.sArg = "";
				}
				//////////////////////////////////////////////////////

			 /*
                //Remove current name exe cwc
                if (sArg.Length >= 3 && sArg[0] == 'c' && sArg[1] == 'w'  && sArg[2] == 'c') {
                      sArg = sArg.Substring(3).Trim();

                 //   Data.fSetWorkingDir(_sFullValue);
                }
			*/

			/*
			//First arg is current file?
			if(Sys.oParentProcess.ProcessName == "cmd"){ //Remove escape sequence
			Data.sArg =  Data.sArg.Replace("\"|\"", "|");
			}*/


			//Output.TraceAction("ARG: " +  Data.sArg );
		
        }


        
		public static string  fGetGlobalVar(string _sVar, bool _bWeak = false) {
//Output.TraceWarning("fGetGlobalVar " + _sVar);
            /*
            if(_sVar == "lPThread") {
            Output.TraceWarning("fGetGlobalVar " + _sVar);
             }*/
			if (Data.aVarGlobal.ContainsKey(_sVar)){
				return   CppCmd.fExtractVar(Data.aVarGlobal[_sVar], null );
			}
            if(_bWeak) {
                return "{" + _sVar + "}";//Keep original
            }
			return "";
		}
		public static void  fSetGlobalVar(string _sVar, string _sValue, bool _bAssingOnEmpty = false) {
            /*
            var _sResult = "";
            //Remove quotes ... or not??!?
            for(int i =0; i < _sValue.Length; i++){}
			_sValue = _sValue.Replace("\'","" ).Replace("\"","" ).Trim();
            */

            var _sResult = CppCompiler.fExtracQuote(_sValue, false).Trim();

			if (!Data.aVarGlobal.ContainsKey(_sVar)){
				  Data.aVarGlobal.Add( _sVar, _sResult);
			//	return;
			}else {
                if(_bAssingOnEmpty) {
                    if( aVarGlobal[_sVar] == "") {
                         aVarGlobal[_sVar] = _sResult;
                    }
                }else { 
			        aVarGlobal[_sVar] = _sResult;
                }
            }
           // if(_sVar == "_sPlatform" && _sValue == "") {Output.TraceError("Here ");} //For debugging
			//Output.TraceGood("Set " + _sVar + ":" + _sValue);
		}



        public static void  fSetDefaultVar(string[] _sArgs) {
            Data.CmdArgs = _sArgs;
            fSetDefaultVar();
        }

         public static void  fSetDefaultVar() {

        	Data.fSetGlobalVar("_wToolchain", "Unknow" );

			Data.fSetGlobalVar("wArch", "x32" );
			Data.fSetGlobalVar("wArchPC", "x86" );
	
			Data.fSetGlobalVar("_pProject", PathHelper.ExeWorkDir);
			Data.fSetGlobalVar("_sOpt", "Debug");

			Data.fSetGlobalVar("wBuildAnd","Run");

		}






    }
}
