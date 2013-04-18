<%@ Page Language="C#" AutoEventWireup="true" Inherits="editdesign" validateRequest="false" CodeFile="editdesign.aspx.cs" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="controls/Editor/ContentDesignerWithValidator.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>Edit Design</title>
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
		<asp:literal id="StyleSheetJS" runat="server"/>
		<script type="text/javascript">
		    <!--//--><![CDATA[//><!--
			    var sSitePath = "<asp:literal id="jsSitePath" runat="server" />";
			    var AutoNav = "<asp:literal id="jsPath" runat="server" />";
				var ResourceText = 
				{
					sContentInvalid: "<asp:literal id="sContentInvalid" runat="server"/>"
				,	sDesignIncompatible: "<asp:literal id="sDesignIncompatible" runat="server"/>"
				,	sContinue: "<asp:literal id="sContinue" runat="server"/>"
                ,   sWarnOnUnload: "<asp:literal id="sWarnOnUnload" runat="server"/>"
				};
			    var defaultFolderId = 0;
			    var buttonPressed = false;
			    var _b_cms_lnReady= false;
			    var editorNotLoadedMsg = "<asp:literal id="jsEditorMsg" runat="server" />";
    			
			    function CheckKeyValue(item, keys) {
				    var keyArray = keys.split(",");
				    for (var i = 0; i < keyArray.length; i++) {
					    if ((document.layers) || ((!document.all) && (document.getElementById))) {
						    if (item.which == keyArray[i]) {
							    return false;
						    }
					    }
					    else {
						    if (event.keyCode == keyArray[i]) {
							    return false;
						    }
					    }
				    }
			    }

			    function CheckTitle () {
				    //there is no title to check here so return true
				    return true;
			    }

    			
    			
			    function SetStyleSheetFromInput()
			    {
				    var sheet = document.forms.frmMain.stylesheet.value;
				    if ("object" == typeof eWebEditPro)
				    {
					    var objInstance = eWebEditPro.instances["content_html"];
					    if (objInstance && objInstance.isEditor())
					    {
						    objInstance.editor.setProperty("StyleSheet",sheet);
					    }
				    }
				    return true;
			    }
			    function SetStyleSheet()
			    {
				    var path = document.forms.frmMain.stylesheetoptions.value;
				    //alert(path);
				    if ("object" == typeof eWebEditPro)
				    {
					    var objInstance = eWebEditPro.instances["content_html"];
					    if (objInstance && objInstance.isEditor())
					    {
						    objInstance.editor.setProperty("StyleSheet",path);
					    }
				    }
				    document.forms.frmMain.stylesheet.value = document.forms.frmMain.stylesheetoptions.value;
				    return true;
			    }
    			
			    function SetAction(Button) {
				    if (buttonPressed != false) {
					    return false;
				    }
				    buttonPressed = true;
				    var objInstance = null;
				    var objContentDesignerInstance = null;
				    if ("object" == typeof eWebEditPro)
				    {   
					    objInstance = eWebEditPro.instances["content_html"];
					    if (objInstance && (!objInstance.isEditor()))
					    {
						    objInstance = null; // if not eWebEditPro ActiveX, then forget about it!
					    }
					    if (objInstance && _b_cms_lnReady == false)
				        {
				           alert(editorNotLoadedMsg);
				           buttonPressed = false;
				           return false;
				        }
				    }
				    if (typeof(objInstance) === 'undefined' || objInstance == null)
				    {
				        if ("object" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances)
				        {
				            objContentDesignerInstance = Ektron.ContentDesigner.instances["content_html"];
				        }
				    }
				    
				    if (Button == "cancel") {
					    document.forms.frmMain.editaction.value = Button;
					    ShutdownImageEditor();
					    if (typeof(objInstance) !== 'undefined' && objInstance !== null) 
					        objInstance.editor.Clear();
    					
					    return true;
				    }
				    if (CheckTitle()) {
					    var SavePosition;
					    var SaveContentLength;
					    var SaveSearchLength;
					    var HowMuchToSave;
					    var iLoop;
					    var dataindex_value;
					    var datafieldlist_value;
					    var myContent;
					    var index_cms;
    					
					    if (objInstance)
					    {
				            var saveContentObj = new Object();
				            saveContentObj.value = "";
				            iLoop = 1;
				            if (!objInstance.save(saveContentObj)) {
					            buttonPressed = false;
					            return false;
				            }		
				            myContent =  saveContentObj.value;
				            dataindex_value = objInstance.editor.GetContent("dataindex");
				            datafieldlist_value = objInstance.editor.GetContent("datafieldlist");
				            dataindex_value = dataindex_value.replace('<?xml version="1.0"?>',"");
				            document.forms.frmMain.dataindex_value.value = dataindex_value;
				            datafieldlist_value = datafieldlist_value.replace('<?xml version="1.0"?>',"");
				            document.forms.frmMain.datafieldlist_value.value = datafieldlist_value;
                            SaveContentLength = myContent.length;
				            for (iLoop = 1; iLoop <= document.forms.frmMain.numberoffields.value; iLoop++) {
					            eval("document.forms.frmMain.hiddencontent" + iLoop + ".value = ''");
				            }
				            iLoop = 1;
				            for(SavePosition = 0; SavePosition < SaveContentLength; SavePosition += 65000) {
					            if ((SaveContentLength - SavePosition) < 65000) {
						            HowMuchToSave = (SaveContentLength - SavePosition);
					            }
					            else {
						            HowMuchToSave = 65000;
					            }
					            eval("document.forms.frmMain.hiddencontent" + iLoop + ".value = myContent.substring(" + SavePosition + "," + (SavePosition + HowMuchToSave) + ");");
					            iLoop += 1;
				            }
				            document.forms.frmMain.hiddencontentsize.value = SaveContentLength;
				            objInstance.editor.Clear();
					    }
				        else if (typeof(objContentDesignerInstance) !== 'undefined' && objContentDesignerInstance !== null)
				        {
							var bCancelAction = false;
							var errMessage = objContentDesignerInstance.validateContent();
							if (errMessage != null && errMessage != "") 
							{
								if ("object" == typeof errMessage && "undefined" == typeof errMessage.code) 
								{
									errMessage = errMessage.join("\n\n\n");
								}
								else if ("object" == typeof errMessage && "string" == typeof errMessage.msg) 
								{
									errMessage = errMessage.msg;
								}
								if ("string" == typeof errMessage && errMessage.length > 0) 
								{
									alert(ResourceText.sContentInvalid + "\n\n" + errMessage);
									bCancelAction = true;
								}
							}
							else
							{
								var report = objContentDesignerInstance.checkCompatibility();
								if (report != null) 
								{
									var reportMsg = "";
                                    if (report.join)
                                    {
                                        reportMsg = report.join("\n\n\n") + "\n\n";
                                    }
                                    if (!confirm(ResourceText.sDesignIncompatible + "\n\n" + reportMsg + ResourceText.sContinue))
									{
										bCancelAction = true;
									}  
								}
							}
							if (bCancelAction)
							{
								buttonPressed = false;
								return false;
							}
				            datafieldlist_value = objContentDesignerInstance.getContent("datafieldlist");
				            document.forms.frmMain.datafieldlist_value.value = datafieldlist_value;
				        }

				        if (window.navigator.userAgent.search("MSIE") == -1 && window.navigator.userAgent.search(/4\.7/) > 0)
                        {
					        document.forms.frmMain.netscape.value = "";
				        }
					    document.forms.frmMain.editaction.value = Button; //this is going to be update
					    
					    if ("function" == typeof WebForm_DoPostBackWithOptions)
					    {
						    WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("", "", false, "", "", false, true));
					    }
					    else if ("function" == typeof __doPostBack)
					    {
						    __doPostBack("","");
					    }
					    else
					    {
					    document.forms.frmMain.submit();
					    }
					    return false;
				    }
				    buttonPressed = false;
				    return false;
			    }
    			
			    function ShutdownImageEditor() {
				    // now we need to check for the image editor and close it before saving
				    // this will cause the present edits on the image to save
				    if ("object" === typeof eWebEditPro && eWebEditPro && eWebEditPro.isInstalled) 
				    {
					    var objInstance = eWebEditPro.instances['content_html'];
					    if (objInstance && objInstance.isEditor())
					    {
					    var objImageEdit = objInstance.editor.ImageEditor();
						    if (objImageEdit && objImageEdit.IsPresent()) 
						    {
							    if (objImageEdit.IsVisible()) 
							    {
							    objImageEdit.ExecCommand("cmdexit", "", 0);
						    }
					    }
				    }
			    }
			    }

			    function Trim (string) {
				    if (string.length > 0) {
					    string = RemoveLeadingSpaces (string);
				    }
				    if (string.length > 0) {
					    string = RemoveTrailingSpaces(string);
				    }
				    return string;
			    }
    		
			    function RemoveLeadingSpaces(string) {
				    while(string.substring(0, 1) == " ") {
					    string = string.substring(1, string.length);
				    }
				    return string;
			    }

			    function RemoveTrailingSpaces(string) {
				    while(string.substring((string.length - 1), string.length) == " ") {
					    string = string.substring(0, (string.length - 1));
				    }
				    return string;
			    }
		    //--><!]]>
        </script>
		<asp:literal id="EditorJS" runat="server"/>
		<script type="text/javascript">
		    <!--//--><![CDATA[//><!--
			    if ("object" == typeof eWebEditPro)
			    {
			        var sAppeWebPath = "<asp:literal id="jsAppeWebPath" runat="server"/>";
			        var sAppLocaleString = "<asp:literal id="jsAppLocaleString" runat="server"/>";
			        eWebEditPro.actionOnUnload = EWEP_ONUNLOAD_NOSAVE;
			        eWebEditPro.parameters.locale = sAppeWebPath + "locale" + sAppLocaleString + "b.xml";
			        eWebEditPro.parameters.baseURL = sSitePath;		
			        editorEstimateContentSize = "body";
			    }
		    //--><!]]>
        </script>
        <style type="text/css">	  
			<!--/*--><![CDATA[/*><!--*/
                .maxHeight { height: 700px !important; }	
            /*]]>*/-->
        </style>
	</head>
	<body onunload="EDUnLoad();" onload="EDLoad();">
		<form id="frmMain" method="post" runat="server">
            <div id="dhtmltooltip"></div>
		    <div class="ektronPageHeader">
		        <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
		        <div class="ektronToolbar" id="divToolBar" runat="server"></div>
		    </div>
		    <div class="ektronPageContainer ektronPageInfo">			
				<table class="ektronGrid">
					<tr>
						<td class="label" title="Title"><asp:literal id="jsTitle" runat="server"/></td>
						<td class="readOnlyValue" id="TD_ColTitle" runat="server"></td>
					</tr>
			    </table>
				<div id="SelectStyleCaption" class="ektronCaption" runat="server">
					<asp:literal id="jsXmlStyle" runat="server" />
				</div>
				<div id="SelectStyleControl" runat="server">
					<asp:DropDownList ToolTip="Select Stylesheet Options from the Drop Down Menu" ID="stylesheetoptions" Runat="server"/>--<asp:literal id="jsInputTmpStyleSheet" runat="server" />
				</div>		
				<div class="ektronTopSpace"></div>		
			    <input type="hidden" name="editaction" id="editaction" value="" />
			    <asp:literal id="jshdnContentLanguage" runat="server" />
			    <input type="hidden" name="hiddencontentsize" id="hiddencontentsize" value="" />
			    <asp:literal id="jshdnMaxContLength" runat="server" />
			    <asp:literal id="jshdnXml_id" runat="server" />
			    <asp:literal id="jshdnIndex_cms" runat="server" /> 
			    <input type="hidden" name="dataindex_value" id="dataindex_value" value="" />
			    <input type="hidden" name="datafieldlist_value" id="datafieldlist_value" value="" />
			    <asp:Literal ID="hiddenfields" runat="server" />
			    <asp:literal id="jshdniSegment" runat="server" />								
			    <script language="javascript" type="text/javascript">
			        <!--//--><![CDATA[//><!--
				        if ("object" == typeof eWebEditPro)
				        {
				        eWebEditPro.parameters.config = eWebEditProPath + "cms_config.aspx?mode=datadesign&nosrc=1";
				        eWebEditPro.parameters.baseURL = sSitePath;		
				        eWebEditPro.parameters.editorGetMethod = "getDocument";
				        eWebEditPro.parameters.maxContentSize = <asp:literal id="jsMaxContLength" runat="server" />;
				        eWebEditPro.parameters.styleSheet = "<asp:literal id="jsTmpStyleSheet" runat="server" />";  
				        } 
        															
				        <asp:literal id="loadSegmentsFn" runat="server"/>
        				
				        function DisableUpload(sEditorName)
				        {
					        if ("object" == typeof eWebEditPro)
					        {
						        var objInstance = eWebEditPro.instances[sEditorName];
						        if (objInstance && objInstance.isEditor())
				                {
							        var objMedia = objInstance.editor.MediaFile();
							        if (objMedia != null)
							        {
					                    var objAutoUpload = objMedia.AutomaticUpload();
								        if (objAutoUpload != null)
					                    {
						                    objAutoUpload.setProperty("TransferMethod", "none");
								            var objMenu = objInstance.editor.Toolbars();
									        if (objMenu != null)
						                    {
							                    var objCommand = objMenu.CommandItem("cmdmfuuploadall");
									            if (objCommand != null)
							                    {
								                    objCommand.setProperty("CmdGray", true);
							                    }
						                    }
					                    }
				                    }
				                }
					        }
				        }
				        Ektron.ready( function() { 
                            $ektron("#content_html_ContentDesigner_wrapper").addClass('maxHeight');

                            window.onbeforeunload = confirmExit;
                            function confirmExit()
                            {
                                if (false == buttonPressed) return ResourceText.sWarnOnUnload;
                            }
				        });
			        //--><!]]>
                </script>
			    <asp:Literal ID="EditorScripts" Runat="server"/>
			    <asp:PlaceHolder ID="phEditContent" Runat="server" />
			    <script type="text/javascript">
			        <!--//--><![CDATA[//><!--
				        if ("object" == typeof eWebEditPro)
				        {
				            eWebEditPro.addEventHandler("onready", "initTransferMethod(eWebEditPro.event.srcName, 'mediamanager.aspx', 'autoupload.aspx')");
				        }
				        function initTransferMethod(sEditor, strURL, strAutoURL)
				        {
					        var objInstance = eWebEditPro.instances[sEditor];
					        if (objInstance && objInstance.isEditor()) 
					        {
					            _b_cms_lnReady = true;
						        loadSegments();
					            // The GUI Selection method:
						        var objMedia = objInstance.editor.MediaFile();
						        if(objMedia != null)
						        {
							        var sMediaURL = AppendURLParam(strURL, "autonav", AutoNav);
							        objMedia.setProperty("TransferMethod", sMediaURL);
							        // The Automatic Accept method:
							        var objAutoUpload = objMedia.AutomaticUpload();
							        if(objAutoUpload != null)
							        {
								        objAutoUpload.setProperty("TransferMethod", strAutoURL);
								        objAutoUpload.SetFieldValue("folder_id", defaultFolderId);
							        }
						        }
						        <asp:literal id="DisabledUpload" runat="server"/>
					        }
				        }
				        function AppendURLParam(strURL, strParam, strAddValue)
				        {
					        var strValue = " ";
					        if ("number" == typeof strAddValue)
					        {
						        strValue = strAddValue.toString();
					        }
					        else
					        {
						        strValue = strAddValue;
					        }
					        if (strValue.length > 0)
					        {
						        var sDelim = "?";
						        var sAssign = "=";
						        if (strURL.indexOf("?") > 0)
						        {
							        sDelim = "&";
						        }
						        if (strParam.indexOf("=") > 0)
						        {
							        sAssign = "";	
						        }
						        strURL += sDelim + strParam + sAssign + escape(strValue);
					        }
					        return(strURL);
				        }

				        var timerId = 0;
        				
				        function EDLoad() {
					        if (timerId) {
						        clearTimeout(timerId);
					        }
					        timerId = setTimeout("DelayedResizeHideFunc()", 1500);
				        }

				        function DelayedResizeHideFunc() {
					        if ((typeof(top.ResizeFrame) == "function")) {
						        top.ResizeFrame(0);
					        }
				        }

				        function EDUnLoad() {
					        if ((typeof(top.ResizeFrame) == "function")) {
						        top.ResizeFrame(1);
					        }
				        }

				        function CanNavigate() {
						    // for now, hardwire to disable navigating away:
						    return (false);
				        }
			            function CanShowNavTree() {
				            // Block displaying the navigation tree while this page is loaded (called from top window-object):
				            return false;
				        }
        				
			            // thanks to http://www.mediacollege.com/internet/javascript/form/disable-return.html
			            function stopRKey(evt) {
	                        var evt  = (evt) ? evt : ((event) ? event : null);
	                        var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
	                        if ((evt.keyCode == 13) && (node.type=="text")) { return false; }
                        }
                        
                        var stylesheetObj = document.getElementById('stylesheet');
                        if (stylesheetObj)
                        {
                            stylesheetObj.onkeypress = stopRKey;
                        }
			        //--><!]]>
                </script>
			</div>	
		</form>
	</body>
</html>
