﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cwc.Compiler {
   public  class ConfigType {

       // public Dictionary<string, Node> aNode = new Dictionary<string, Node>();
        public Node oMainNode = new Node(null, "");
        public Node oCurrNode ;

        CompilerData oParent;
        ConfigType oDefault;

        public string sName = "";
        public ConfigType(CompilerData _oParent, string _sName, ConfigType _oGblConfigType) {
            oParent = _oParent;
            oDefault = _oGblConfigType;
            sName =  _sName;

            oCurrNode = oMainNode;
        }

        public string sExtention = "";
        public string[] aExtention = new string[]{ };





       
        public string sExe_CWift = "";
        public string sExe_Cpp = "";
        public string sExe_C = "";


        public string sExe_Compiler = "";
  public string fGet_Exe_Compiler() {
                 if (sExe_Compiler == "") {
    return oDefault.sExe_Compiler;
            }return sExe_Compiler;}

		public string sExe_Link_Static = "";

        public string sExe_Link_Dynamic = "";


        public string sExe_RC = "";
        public string sExe_Linker = "";



		public string sPath = "";
		public string sCompiler= "";
		public string sCompilerLinker= "";
		public string sFinally_CopyFolder= "";
        public string sCpp = "";
        public string sCWayv = "";
        public string sC = "";
		public string sLinker = "";
        public string sLink_Static = "";
        public string sLink_Dynamic = "";
        public string sRC = "";
        public string sDebug = "";
    


   public string sLinkerD = "";
   public string sLinkerS = "";

        public string sWebsite = "";

        public string sArgCpp = "";
        public string sArgC = "";
        public string sArgLinkerS = "";
        public string sArgLinkerD = "";
        public string sArgRC = "";

        public string sHArgCpp = "";
        public string sHArgC = "";
        public string sHArgLinker = "";
        public string sHArgLinkerS = "";
        public string sHArgLinkerD = "";
        public string sHArgRC = "";
		public string sLinkFinalAddLib = "";





        public void fSetExtentions(string _sValue) {

            sExtention = _sValue;
           aExtention = sExtention.Split(',');
              
        }

        internal Node fAddNode(string _sName) { //Just for master default goblal ConfigType --> else we copy nodes 
               oCurrNode = oCurrNode.fAddNode(_sName);
            return oCurrNode;
        }

        internal void fRemoveNode(string _sName) { //Just for master default goblal ConfigType --> else we copy nodes 
              oCurrNode = oCurrNode.oParent;//TODO TEST IF IS SAME NAME
            if (oCurrNode == null) { //TODO POSSIBLE?
                oCurrNode = oMainNode;
            }
                 
            //Console.WriteLine("fRemoveNode " + _sName);
        }

        internal void fSetValue(string _sValue,string _sType) {
               //    Console.WriteLine("-- fSetValue  "  + oCurrNode.sName + " | "+ _sValue);
           oCurrNode.fSetValue( _sValue, _sType);
        }

        internal string fGetNode(ConfigType _oAddCompilerConfig, string[] _aValue, string _sType) {
           //TODO _oAddCompilerConfig

            bool  _bCombineMode = true;
            Node _oNode = oMainNode;
   
            if (_aValue[0] == "Exe") { //Only one exe at time
                _bCombineMode = false;
            }

            foreach (string _sStr in _aValue) {
                if(_oNode.aVal.Count == 0){
                  
                    if(_oNode.aNode.ContainsKey(_sStr)){
                        _oNode = _oNode.aNode[_sStr];
                      
                    } else {
                 
                    }
                }
                
                 if(_oNode.aVal.Count != 0){
                    if(_sType == "" && _oNode.aVal.ContainsKey("")){
                        return _oNode.aVal[""].sValue;
                    }
                    if (_oNode.aVal.ContainsKey(_sType)) {
                        if(_bCombineMode){
                             return _oNode.aVal[""].sValue + " " + _oNode.aVal[_sType].sValue;
                        } else {
                            return  _oNode.aVal[_sType].sValue;
                        }
                    }
                    if( _oNode.aVal.ContainsKey("") ) {
                        return _oNode.aVal[""].sValue;
                    }else {
                        return "";
                    }
                }
            }
               
            return "";
        }


        /*
        internal void fCopyNodes(Node _oLeafNode) { //Just for master default goblal ConfigType --> else we copy nodes 
           Node _oCurrNode = _oLeafNode;
           List<Node> _aListNode = new List<Node>();
            while (_oCurrNode != null) {
                _aListNode.Insert(0, _oCurrNode);
                _oCurrNode = _oCurrNode.oParent;
            }

            Node _oCurrCheck= oMainNode;

            foreach (Node _oNode in _aListNode) {
                if (_oCurrCheck.aNode.ContainsKey(_oNode.sName)) {

                }
            } 

            //oCurrNode = oCurrNode.fAddNode(_sName);
        }*/

    }
}
