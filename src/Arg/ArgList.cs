﻿using cwc.Update;
using cwc.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cwc {

    class ArgStruct {
        public delegate void dfunc(string str);
        public dfunc func;
        public string arg_short = "";
        public string arg_long = "";
        public string description = "";
        public string param = "";
         
        public ArgStruct(string _short, string _long, string _param , dfunc _func, string _description) {
            arg_short = _short;
            arg_long = _long;
            func = _func;
            param = _param;
            description = _description;
        }

    }

    class ArgList {
        public static ArgStruct[] aList = new ArgStruct[] {
            new ArgStruct("-h", "--help",          "",                          getHelp     , "Get command list"),
            new ArgStruct("-v", "--version",       "",                          getVersion  , "Get current version"),
            new ArgStruct("-m", "--module",        "",                          getModule   , "Get installed module list"),
            new ArgStruct("-r", "--release",       "",                          getRelease  , "Get module release list"),
            new ArgStruct("-u", "--update",		   "[Autor/Name/(Version)]",    updateModule, "Update modules"),
            new ArgStruct("-a", "--args",		   "[Arg List]",                getRelease	, "Pass argument to the lauching app"),
            new ArgStruct("",   "--message",       "[String]",                  message  , "Message to print at start"),
            new ArgStruct("",   "--self_update",   "[Dir]",                     self_update  , "Copy itself to dir and reload"),
            new ArgStruct("",   "--updated",       "[Version]",                 updated  , "Validate update"),
			new ArgStruct("",	"--GUI",			"",						GUI  ,		"Start in GUI mode"),
			new ArgStruct("-p", "--pipe",		   "[Name/(Server)/(TimeoutMs)]",namedpipe  , "Create a named pipe"),
       
        };

        public static bool bReceiveMSG = false;

        public static void updated(string _param) {
            bReceiveMSG = true;

            UpdateCwc.fUpdated(_param);
        
        }
		public static void GUI(string _param) {
			if(Data.bConsoleMode == false) {
				 Output.TraceWarning("Already in GUI mode" );
			}else {
				 Output.TraceAction("Lauch GUI mode");//Impossible?
			}
			 
		}

	    public static void namedpipe(string _param) {
			string _server = "localhost";
			string _name = _param;
			if(_name == "") {
				_name = "cwc_pipe";
			}
            Output.TraceAction("Named Pipe [" + _server + "] "  + _name + " " );
			new NamedPipes(_server, _name);
        }

		

       public static void message(string _param) {
            bReceiveMSG = true;
             Output.Trace(_param);
        }

        public static string alignTo(string _in, int _to) {
            for(int i = _in.Length; i < _to; i++) {
                _in+=" ";
            }
            return _in;
        }
        
       public static void ProcessArg(string _fullarg) {
			_fullarg = _fullarg.Trim();     
			if(_fullarg == "") {
				Debug.fTrace("No Args");
				return;
			}
            Output.TraceAction( _fullarg );

            CppCmd _oTemp =  new CppCmd(null,"");
			//Debug.fTrace("Args: " + _fullarg);

            string _sCmdArg = _oTemp.fExtractSpaceMultiVals(_fullarg, ' ' );
            string _cmd = _oTemp.sRet_ExtractSpaceMultiValsCmd;
			if(_cmd =="") {
				Debug.fTrace("No cmd Args");
				return;
			}
            foreach(ArgStruct _o in ArgList.aList) {
                if( _o.arg_short == _cmd || _o.arg_long == _cmd) {
                    _o.func(_sCmdArg);
                    return;
                }
            }
            Output.TraceWarningLite( "Unknow command[\f0C" + _cmd + "\f0E], try --help" );
        }


       public static void getVersion(string _param) {
          //   Output.TraceAction("getVersion!");

        }

       public static void getHelp(string _param) {

            foreach(ArgStruct _o in ArgList.aList) {
                string _line = "    ";
                _line =   alignTo(_line  + _o.arg_short,        8);
                _line =   alignTo(_line  + _o.arg_long ,       25); 
			    _line =   alignTo(_line  + _o.param,       50); 
                _line += "(" + _o.description + ")";

                Output.TraceReturn(_line);
            }
        }


       public static string[] getDirList(string _path) {
             string[] _aDir = Directory.GetDirectories(_path);
			Array.Sort(_aDir, StringComparer.InvariantCultureIgnoreCase);
			Array.Reverse(_aDir);
            return _aDir;
        }

            public static bool bSelfUpdate = false;
       public static void  self_update(string _toDir) {
            bReceiveMSG = true;
            bSelfUpdate = true;
            Output.TraceAction("Self_UPDATE to: " +_toDir );
            UpdateCwc.fUpdateFiles(_toDir);
            /*
            while(true) {
                Thread.Sleep(2);
            }*/
        }

		public static void updateModule(string _param) {
			if(_param == "") {
				Output.TraceReturn("-= Update Your Modules =-");
				Output.TraceReturn("examples:");
				Output.TraceReturn("-u cwc");
				Output.TraceReturn("-u cwc v0.0.95.9");
				Output.TraceReturn("-u VlianceTool/LibRT");
			}
			_param = _param.ToLower();
			if(_param == "cwc") {
				Output.TraceAction("Update CWC");
			}else {
                return;
            }

            //Cleanup
			FileUtils.DeleteDirectory(PathHelper.GetExeDirectory() + "Upd_Cwc",true);
			FileUtils.DeleteDirectory(PathHelper.GetExeDirectory() + "Upd_CwcUtils",true);

			ModuleData _oModule = ModuleData.fGetModule("VLiance/Cwc", true);

                Thread winThread = new Thread(new ThreadStart(() =>  {  
                       _oModule.fGetLocalVersions();
                     _oModule.fReadHttpModuleTags();
                
                        //Wait to finish
                        while(ModuleData.nRequestTag > 0) {
                            Thread.CurrentThread.Join(1);
                        }

                    List<ModuleLink> _aLink = new List<ModuleLink>();
                    if( _oModule.aLinkList.Count > 0) {
                        foreach(string _sKeyLink  in _oModule.aLinkList) {
                            // Output.TraceWarning( "Recommended version:");
                            Output.TraceReturn( " Tag:" + _oModule.sName + " : " + _sKeyLink );
                            _aLink.Add(_oModule.aLink[_sKeyLink]);
                           break;
                        }
                    }else {
                            Output.TraceError( "Not found:" + _oModule.sName  );
                    }

                
				    Output.TraceWarning( "Starting Download ... ");
				    foreach(ModuleLink _oLink in _aLink) { //Only one
						    _oLink.fDownload();
						    while(_oLink.bDl_InProgress) {Thread.CurrentThread.Join(1); }
                     
						    _oLink.fExtract();
						    while(_oLink.oModule.bExtact_InProgress) {Thread.CurrentThread.Join(1); }
                        
				    }
					    Output.Trace("");
				    Output.TraceGood( "---------------- All Required Module Completed ------------------");
				    foreach(ModuleLink _oLink in _aLink) { //only one TODO
						    Output.TraceAction(_oLink.oModule.sCurrFolder);
                         UpdateCwc.fLauchUpdate( _oLink.oModule.sCurrFolder, PathHelper.GetCurrentDirectory());
                        break;
				    }


                }));  
		        winThread.Start();
		}

        public static void getRelease(string _param) {
            Output.TraceAction("PAram: " + _param);

               //ModuleData _oModule = ModuleData.fGetModule("VLianceTool/LibRT", true);
               ModuleData _oModule = ModuleData.fGetModule("VLiance/Cwc", true);
			   _oModule.fGetLocalVersions();
             _oModule.fReadHttpModuleTags();
                
                  
                //Wait to finish
                while(ModuleData.nRequestTag > 0) {
                    Thread.CurrentThread.Join(1);
                }
              
                List<ModuleLink> _aLink = new List<ModuleLink>();
          
                if( _oModule.aLinkList.Count > 0) {
                    foreach(string _sKeyLink  in _oModule.aLinkList) {
                        // Output.TraceWarning( "Recommended version:");
                        Output.TraceReturn( " Tag:" + _oModule.sName + " : " + _sKeyLink );
                        _aLink.Add(_oModule.aLink[_sKeyLink]);
                       
                    }
                }else {
                        Output.TraceError( "Not found:" + _oModule.sName  );
                    // _bFound = false;
                        
                }
				//GuiConsole.sFormCmd = "GoEnd_Force";
                
        }

        public static void getModule(string _param) {

			 // fGetModule

            string[] _aDir = getDirList( PathHelper.GetExeDirectory() + "Toolchain/");
            foreach(string _sServer in _aDir) {
                Output.TraceReturn(_sServer);
                string[] _aSubDir = getDirList( _sServer);
                foreach(string _sDir in _aSubDir) {
                    Output.TraceReturn("    " + _sDir);
                    string[] _aVersion = getDirList( _sDir);
                    foreach(string _sVersion in _aVersion) {
                        Output.TraceReturn("        " + _sVersion);


                    }
                }
            }


                    
                    ModuleData _oModule = ModuleData.fGetModule("VLianceTool/LibRT", true);
			        _oModule.fGetLocalVersions();
                   // _oModule.fUpdateStatus();

                    foreach(string _sVersion in _oModule.aLocalVersion) { if(_sVersion != null) {
                          Output.TraceReturn("Module: " + _oModule.sName + " version " + _sVersion);
                    } }
                    
                  
                    
			       // _oModule.fReadHttpModuleTags();
                


                  /*
                //Wait to finish
                while(ModuleData.nRequestTag > 0) {
                    Thread.CurrentThread.Join(1);
                }
              
                List<ModuleLink> _aLink = new List<ModuleLink>();
                foreach(string _sModule in Data.aRequiredModule) {
                    ModuleData _oModule = ModuleData.fFindModule(_sModule);
                    if( _oModule.aLinkList.Count > 0) {
                        foreach(string _sKeyLink  in _oModule.aLinkList) {
                          // Output.TraceWarning( "Recommended version:");
                            Output.TraceAction( "Recommended version:" + _oModule.sName + " : " + _sKeyLink );
                            _aLink.Add(_oModule.aLink[_sKeyLink]);
                            break;
                        }
                    }else {
                         Output.TraceError( "Not found:" + _sModule  );
                       // _bFound = false;
                        
                    }
                }*/


        }


    }
}
