<%@ Page Language="C#" AutoEventWireup="true" Inherits="edit" ValidateRequest="false" CodeFile="edit.aspx.cs" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Reference Control="controls/forms/newformwizard.ascx" %>
<%@ Reference Control="controls/media/commonparams.ascx" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="controls/Editor/ContentDesignerWithValidator.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
        <title>Edit</title>
        <meta http-equiv="Pragma" content="no-cache" />
        <script type="text/javascript" src="java/ActiveXActivate.js"></script>
        <script type="text/javascript" src="java/RunActiveContent.js"></script>
        <script type="text/javascript" src="java/internCalendarDisplayFuncs.js"></script>
        <script type="text/javascript" src="java/eweputil.js"></script>
        <script type="text/javascript" src="java/jfunct.js"></script>
        <script type="text/javascript" src="java/searchfuncsupport.js"></script>
        <script type="text/javascript" src="java/cmsedit.js"></script>
        <script type="text/javascript" src="java/determineoffice.js"></script>
        <link type="text/css" rel="stylesheet" href="Tree/css/com.ektron.ui.tree.css" />
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
              var isReadyEditor = false;
              Ektron.ready(function(){
                    var iframelist = Array();
                    if ($ektron(".confirmationURL").length > 0 && $ektron("#NextUsing").length > 0)
	                    $ektron(".confirmationURL").attr("value", $ektron("#NextUsing").val());
                    $ektron("form").css("visibility", "visible");
					setTimeout(function(){
						$ektron("body").css("background-image", "none");
					}, 100);
					if ($ektron.browser.mozilla)
					{
					    $ektron(window).bind("onReady_content_html", function()
                        {
                            isReadyEditor = true;
                        });
                        $ektron(window).bind("ektron_editor_ff_iframe_add", function()
                        {
                            $ektron('#pleaseWait').modalShow();
                            iframelist.push(true);
                        });

                        $ektron(window).bind("ektron_editor_ff_iframe_remove", function()
                        {
                            //iframelist.pop();

                            //debugger;
                            pollLoadingFF();
                        });
					}
					else
					{
					    isReadyEditor = true;
                    }
                });
			    var elx1 = null;
			    var elx2 = null;

                function pollLoadingFF() {
                    var items = $ektron("#RadEContentIframecontent_html_ContentDesigner").contents().find('.design_loading_indicator');
                    if(items.length == 0) {
                        $ektron('#pleaseWait').modalHide();
                    }else{
                        setTimeout("pollLoadingFF()", 500); // poll in a half second
                    }
                }

			    function IsBrowserIE()
                {
                    var ua = window.navigator.userAgent.toLowerCase();
                    return((ua.indexOf("msie ") > -1) && (!(ua.indexOf("opera") > -1)));
			    }
			    function IsFireFox()
			    {
			        if (/Firefox[\/\s](\d+\.\d+)/.test(navigator.userAgent)) { //test for Firefox/x.x or Firefox x.x (ignoring remaining digits);
			            return true;
			        }
			        else {
			            return false;
			        }
			    }
			    function GetFireFoxVersion()
			    {
			        if (/Firefox[\/\s](\d+\.\d+)/.test(navigator.userAgent)) { //test for Firefox/x.x or Firefox x.x (ignoring remaining digits);
                        var ffversion=new Number(RegExp.$1) // capture x.x portion and store as a number
                        if (ffversion>=3)
                            return 3;
                        else if (ffversion>=2)
                            return 2;
                        else if (ffversion>=1)
                            return 1;
                    }
			    }
                
			    //hide the drag and drop uploader
			    if (typeof top.HideDragDropWindow != "undefined")
			    {
				    top.HideDragDropWindow();
			    }
			 
			    var jsContentLanguage="<asp:literal id="jsContentLanguage" runat="server"/>";
			    var jsId="<asp:literal id="jsId" runat="server"/>";
			    var jsDefaultContentLanguage="<asp:literal id="jsDefaultContentLanguage" runat="server"/>";
			    var jsType="<asp:literal id="jsType" runat="server"/>";
			    var jsIsMac="<asp:literal id="jsIsMac" runat="server"/>";
			    var jsSelectedDivStyleClass="<asp:literal id="jsSelectedDivStyleClass" runat="server"/>";
			    var jsUnSelectedDivStyleClass="<asp:literal id="jsUnSelectedDivStyleClass" runat="server"/>";
			    var ewebchildwin;
			    var m_initializedOffsets = false;
			    var m_fullScreenView = false;
			    var m_mainTblOffset = 78;
			    var m_stdVertOffset = 105;
			    var m_altVertOffset = 25;
			    var buttonaction;
			    var g_initialPaneToShow = 'dvContent';
			    var g_visiblePane = "";
			    var g_aryFieldList = new Array();
			    var g_contentEditor = true;
			    var bEnableTabs = false;

			    function IsContentEditorSelected()
			    {
				    return (g_contentEditor);
			    }

			    function GetEphoxEditor()
			    {
				    if (IsContentEditorSelected())
				    {
					    return (elx1);
				    }
				    else
				    {
					    return (elx2);
				    }
			    }
			    function RemoveContentImage(path) {
			        var elem = null;
			        var elemThumb = null;
			        elem = document.getElementById( 'content_image' );
			        if (elem != null)
			        {
			            elem.value = '';
			        }
			        elemThumb = document.getElementById( 'content_image_thumb' );
			        if ( elemThumb != null )
			        {
			            elemThumb.src = path;
			        }
			    }
			    function checktoclosechildwind() {
				    if (ewebchildwin && ewebchildwin.open && !ewebchildwin.closed) ewebchildwin.close()
			    }
			    function CanNavigate() {
				    // Block navigation while this page loaded (called from top window-object):
				    return false;
			    }
			    function CanShowNavTree() {
				    // Block displaying the navigation tree while this page loaded (called from top window-object):
				    return false;
			    }
			    // Adjusts the navigation-tree frame (if function exists; ie workarea).
			    // (True Shows the nav-tree, False hides it)
			    function ResizeFrame(val) {
				    if ((typeof(top.ResizeFrame) == "function") && top != self) {
					    top.ResizeFrame(val);
				    }
			    }

			    var m_isMac = false;
			    var m_isMacInit = false;
			    function IsPlatformMac() {
				    if (m_isMacInit) {
					    return (m_isMac);
				    } else {
					    var posn;
					    var sUsrAgent = new String(navigator.userAgent);
					    sUsrAgent = sUsrAgent.toLowerCase();
					    posn = parseInt(sUsrAgent.indexOf('mac'));
					    m_isMac = (0 <= posn);
					    m_isMacInit = true;
					    return (m_isMac);
				    }
			    }

			    var m_isSafari = false;
			    var m_isSafariInit = false;
			    function IsBrowserSafari() {
				    if (m_isSafariInit) {
					    return (m_isSafari);
				    } else {
					    var posn;
					    var sUsrAgent = new String(navigator.userAgent);
					    sUsrAgent = sUsrAgent.toLowerCase();
					    posn = parseInt(sUsrAgent.indexOf('safari'));
					    m_isSafari = (0 <= posn);
					    m_isSafariInit = true;
					    return (m_isSafari);
				    }
			    }


			    function SetObjVisible(itemId, flag) {
				    var obj;
				    if (('string' == typeof(itemId)) && (0 < itemId.length)) {
					    obj = document.getElementById(itemId);
					    if ((null != obj) && ('undefined' != typeof(obj.style)) && ('undefined' != typeof(obj.style.visibility))) {
						    if (flag) {
							    obj.style.visibility = "visible";
						    } else {
							    obj.style.visibility = "hidden";
						    }
					    }
				    }
			    }
			    function SetObjAltOffset(itemId, flag) {
				    var obj, offset;
				    if (flag) {
					    offset = m_altVertOffset;
				    } else {
					    offset = m_stdVertOffset;
				    }
				    if (('string' == typeof(itemId)) && (0 < itemId.length)) {
					    obj = document.getElementById(itemId);
					    if ((null != obj) && ('undefined' != typeof(obj.style))) {
						    // Note: handled differently for IE & NN/FF in an attempt to prevent browser errors:
						    if (IsBrowserIE()) {
							    if ('undefined' != typeof(obj.style.pixelTop)) {
								    obj.style.pixelTop = offset;
								    }
						    } else {
							    if ('undefined' != typeof(obj.style.top)) {
								    obj.style.top = Trim(offset.toString()) + "px";
							    }
						    }
					    }
				    }
			    }

			    function ShowPane(tabID)
			    {
				    if (false == bEnableTabs){
					    return false;
				    }

				    // Needed to determine if midia-insert (library) items should
				    // be sent to the content Ephox Editor or the summarry one:
				    g_contentEditor = ("dvContent" == tabID);

				    // For Netscape/FireFox: Objects appear to get destroyed when "display" is set to "none" and re-created
				    // when "display" is set to "block." Instead will use the appropriate style-sheet
				    // class to move the unselected items to a position where they are not visible.
				    // For IE: If the ActiveX control is display="none" programmatically rather than by user click,
				    // the ActiveX control seems to uninitialize, for example, the DHTML Edit Control (DEC) is gone.
				    var aryTabs = ["dvContent", "dvPollWizard", "dvSummary", "dvAlias", "dvMetadata", "dvSchedule", "dvComment", "dvSubscription", "dvTemplates","dvTaxonomy"];

				    for (var i = 0; i < aryTabs.length; i++)
				    {
					    SetPaneVisible(aryTabs[i], false);
					    SetPaneVisible(aryTabs[i], (tabID == aryTabs[i]));
				    }
				    g_initialPaneToShow = tabID; // remember which tab is selected if editor is reloaded
			    }

			    function SetPaneVisible(tabID, bVisible)
			    {
				    var objElem = null;
				    objElem = document.getElementById(tabID);
				    if (objElem != null)
				    {
					    if (!bVisible) {
					        $ektron("#" + tabID).addClass("ui-tabs-hide");
					    } else {
					        $ektron("#" + tabID).removeClass("ui-tabs-hide");
					    }
				    }
				    objElem = document.getElementById("_" + tabID);
				    if (objElem != null)
				    {
					    // For Safari on the Mac (to fix Ephox Editor issues),
					    // the actual class names are overridden in the code behind
					    // (uses special classes when Safari on the Mac is detected):
					    objElem.className = (bVisible ? jsSelectedDivStyleClass : jsUnSelectedDivStyleClass);
				    }
				    if (bVisible)
				    {
					    g_visiblePane = tabID;
				    }
				    else if (!bVisible && g_visiblePane == tabID)
				    {
					    g_visiblePane = "";
				    }

				    if ("dvSummary" == tabID && bVisible && "2" == document.forms.frmMain.content_type.value)
				    {
					    updateMergeFieldList("content_html", "content_teaser");
				    }
				    $ektron(document).trigger("wizardPanelShown");
			    }

			    // This is a hack, but time is of the essence.
			    var bFormEditorReady = false;
			    var bResponseEditorReady = false;
			    var bContentEditorReady = false;
			    var bTeaserEditorReady = false;
			    var bPageClosing = false;
			    function updateMergeFieldList(sFormEditor, sResponseEditor)
			    {
				    /*
				    sFieldList is the list of fields defined in the form editor as a JavaScript literal array.
				    Each item in the array is an object with these properties:
					    name:			tag name of the field
					    displayName:	human-readable name of the field
					    datatype:		type of the data for ektdesignns_content attribute, typically XSD datatype, but not always
					    content:		type of the data for ektdesignns_content attribute
					    xpath:			XPath locating the field, starting with "/root/"

				    The XML structure of the form data submitted is:
					    <SubmittedData>
						    <FormTitle>This is the Title</FormTitle>
						    <FormDescription>This is the description.</FormDescription>
						    <Date value="2005-07-11T17:24:58">7/11/2005 5:24:58 PM</Date>
						    <Data>
							    <Field1>Value of Field 1</Field1>
							    <Field2>Value of Field 2</Field2>
							    <Field3>Value of Field 3</Field3>
							    :
						    </Data>
					    </SubmittedData>

				    Notes:
				     * The <FormDescription> is currently plain text, but could be rich text in the future.
				     * The <Date> is not ISO 8601 format and therefore is treated as a string. The value
				     * attribute is ISO format.
				    */
				    var objFormInstance = null;
				    var objRespInstance = null;
				    var cdForm = null; //cd = content designer
				    var cdResponse = null;
				    var sEditorContent = "";
				    if (sFormEditor != "" && sFormEditor != null)
				    {
					    if (typeof eWebEditPro != "undefined" && eWebEditPro)
					    {
					        objFormInstance = eWebEditPro.instances[sFormEditor];
					    }
					    if (!objFormInstance)
					    {
					        cdForm = Ektron.ContentDesigner.instances[sFormEditor];
					    }
					    if (bPageClosing) return;
                        var sFieldList = "";
					    if (objFormInstance)
					    {
					        // eWebEditPro only
					        if (!objFormInstance.isEditor() || !bFormEditorReady || !bResponseEditorReady) return;
					        if ("undefined" == typeof g_aryFormFieldList || null == g_aryFormFieldList || objFormInstance.isChanged())
					        {
						        sFieldList = objFormInstance.editor.GetContent('datafieldlistjs');
					        }
                        }
					    else if (cdForm)
				        {
				            sEditorContent = cdForm.getContent();
				            sFieldList = cdForm.getContent("datafieldlistjs");
				        }

					    if ("" == sFieldList || sFieldList.indexOf("{fields:[") < 0)
					    {
						    sFieldList = "{fields:[],datalists:[]}";
					    }

					    eval("g_aryFormFieldList = " + sFieldList);

					    if ("undefined" == typeof g_aryFormFieldList || null == g_aryFormFieldList) return;
				   
					    for (var counter = 0; counter < g_aryFormFieldList.fields.length; counter++)
				        {
					        // Patch XPath path from form editor to match the XML data to be transformed.
					        // Be careful not to replace xpaths for items added in updateMergeFieldList().
					        g_aryFormFieldList.fields[counter].xpath = g_aryFormFieldList.fields[counter].xpath.replace("/root/", "/*/Data/");
	                    }
					    g_aryFormFieldList.fields[g_aryFormFieldList.fields.length] = {
							    name:			"FormTitle"
							    , displayName:	"[Form Title]"
							    , datatype:		"string"
							    , content:		"text"
							    , xpath:		"/*/FormTitle"
					    };
					    g_aryFormFieldList.fields[g_aryFormFieldList.fields.length] = {
							    name:			"FormDescription"
							    , displayName:	"[Form Description]"
							    , datatype:		"string"
							    , content:		"text"
							    , xpath:		"/*/FormDescription"
					    };
					    g_aryFormFieldList.fields[g_aryFormFieldList.fields.length] = {
							    name:			"date"
							    , displayName:	"[Date Submitted]"
							    , datatype:		"string"
							    , content:		"text"
							    , xpath:		"/*/Date"
					    };
				    }

                    if (typeof eWebEditPro != "undefined" && eWebEditPro)
                    {
                        // eWebEditPro Only
                        objRespInstance = eWebEditPro.instances[sResponseEditor];
				        if (!objRespInstance || !objRespInstance.isEditor() || !bFormEditorReady || !bResponseEditorReady || bPageClosing) return;

				        var objMenu = objRespInstance.editor.Toolbars();
				        if (!objMenu) return;

				        var objCommand = objMenu.CommandItem("jsfieldlist");
				        if (!objCommand || !objCommand.IsValid())
				        {
					        objCommand = objMenu.CommandAdd('jsfieldlist', 'Field List', 'Field List', '', 0, 5, 'designbar', 0, 0);
					        if (!objCommand) return;
				        }

				        objCommand.Clear(); // remove any items which may be in the drop-down list

				        // Always show "(Insert Field)" at the top of the list.
				        objCommand.AddItem('(Insert Field)', 0, '');

				        if ("undefined" == typeof g_aryFormFieldList || null == g_aryFormFieldList) return;

				        for (var counter = 0; counter < g_aryFormFieldList.fields.length; counter++)
				        {
					        // Add field to drop-down list
					        objCommand.AddItem(g_aryFormFieldList.fields[counter].displayName, counter + 1, 'jsfieldlist');
				        }
				        if (objFormInstance && objFormInstance.isEditor())
				        {
					        var sXMLList = objFormInstance.editor.GetContent("datafieldlist");
					        var sXMLData = objFormInstance.editor.GetContent("datadocumentxml");
					        var sFormTitle = "[Form Title]";
					        if ("object" == typeof document.forms[0].content_title && document.forms[0].content_title)
					        {
						        sFormTitle = document.forms[0].content_title.value;
						        sFormTitle = eWebEditProUtil.HTMLEncode(sFormTitle);
					        }
					        var sDate;
					        var dNow = new Date();
					        if (dNow.toLocaleString)
					        {
						        sDate = dNow.toLocaleString();
					        }
					        else
					        {
						        sDate = dNow.toString();
					        }
					        sDate = eWebEditProUtil.HTMLEncode(sDate);
					        var sXMLDoc = "<SubmittedData><FormTitle>" + sFormTitle + "</FormTitle>";
					        sXMLDoc += "<FormDescription>" + "[Form Description]" + "</FormDescription>";
					        sXMLDoc += "<Date>" + sDate + "</Date>";
					        sXMLDoc += "<Data>";
					        // Remove <root> tag
					        sXMLDoc += sXMLData.replace("<root>","").replace("</root>","");
					        sXMLDoc += "</Data>";
					        sXMLDoc += "</SubmittedData>";
					        objRespInstance.editor.SetContent("datafieldlist", sXMLList, "");
					        objRespInstance.editor.SetContent("datadocumentxml", sXMLDoc, "");
				        }
				    }
				    else if ("object" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances)
				    {
				        if (bPageClosing) return;
				        cdResponse = Ektron.ContentDesigner.instances[sResponseEditor];
				        if (cdResponse)
				        {
				            cdResponse.fieldListArray = g_aryFormFieldList;
				            cdResponse.formContent = sEditorContent;
				        }
				    }
			    }

			    function SetFormActionToTemplate(obj)
			    {
			        var new_xid = obj.value;
			        var current_xid = document.getElementById('SelectedXid').value = new_xid;
			    }

			    function DisableTemplateSelect(bCheck)
			    {
			        if (bCheck)
			        {
			            document.getElementById("templateSelect").setAttribute("disabled", "disabled");
			        } else {
			            document.getElementById("templateSelect").removeAttribute("disabled");
			        }
			    }

			    function MultipleUploadView()
                {
                    if(document.getElementById("idMultipleView") != null)
                    {
                        document.all("idMultipleView").style.display="inline";
                    }
                }


                var folderRequireManualAlias = '<asp:literal id="jsfolderRequireManualAlias" runat="server"/>';
                var folderRequireManualAlias2010 = '<asp:literal id="jsfolderRequireManualAlias2010" runat="server"/>';
	            var ManualAliasAlert='<asp:literal id="jsManualAliasAlert" runat="server"/>' ;
                function DocumentUpload()
                {
                    if(folderRequireManualAlias=='true')
                        alert(ManualAliasAlert);
                    if(document.getElementById("idUploadCtl") != null)
                    {
                        document.getElementById("idUploadCtl").MultipleUpload();
                    }
                    return false;
                }
                function DocumentUploadOffice2010()
                {
                    var btn=document.getElementById('btnMupload');
                    btn.setAttribute('disabled','disabled');
                    btn.value='Done';

                    if(folderRequireManualAlias2010=='true')
                        alert(ManualAliasAlert);

                    return DocumentUpload();
                }
                function UpdateContentWidget(id,buttonId)
                {

                    parent.opener.$ektron(".HiddenTBData").val(id);
                    //make sure any cluetips get lost
                    parent.opener.$ektron("#cluetip, #cluetip-waitimage").remove();
                    var parentId = parent.opener.$ektron("#" + buttonId).parents("div.CBWidget").attr("id");
                    var btnName = parent.opener.$ektron("#" + parentId ).find(".CBSave").attr("name");
                    parent.opener.__doPostBack(btnName,"");
                    top.close();

                }




            //--><!]]>
        </script>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
			    var eWebEditProLangType = jsContentLanguage;
			    var eWebEditProDefaultContentLanguage = jsDefaultContentLanguage;
			    var ewebchildwin;

			    function WaitOnLoadAction()
			    {
				    setTimeout('loadPage();', 100);
			    }
			    function loadPage()
			    {
				    if(window.top.opener && window.top.opener.closed)
				    {
					    top.close();
				    }
				    else
				    {
					    top.opener.location.href = (top.opener.location.href).replace(top.opener.location.hash,"");
					    if(navigator.userAgent.indexOf('win') != -1)
					    {
					        top.close();
					    }
				    }
			    }

		        function makeNewElement (elementTag) {
                    try {
                        newElement = document.createElement("<" + elementTag + "/>");
                    }
                    catch(e) {
                        newElement = document.createElement(elementTag);
                    }
                    return newElement;
                }

                function escapeHTML (str) {
                   var div = makeNewElement('div');
                   var text = document.createTextNode(str.replace("'", "''"));
                   div.appendChild(text);
                   return div.innerHTML;
                }

                function validateContentTitle() {
                    var contentTitleTXTBox = document.getElementById("content_title");
                    if (contentTitleTXTBox != null) {
                        var strTitle = contentTitleTXTBox.value;
                        var escapedTitle = escapeHTML(strTitle);
                        var jsEscapedContentTitle = escape(strTitle);
                        if (escapedTitle.length > 200 || jsEscapedContentTitle.length > 200) {
                            alert('<asp:literal id="jsMaxLengthMsg" runat="server"/>');
                            $ektron('#pleaseWait').modalHide();
                            return false;
                        }
                        else {
                            return true;
                        }
                    }
                    return true;
                }
                function ektronFormSubmit()
                {
				    if ("function" == typeof WebForm_DoPostBackWithOptions)
				    {
					    // eventTarget, eventArgument, validation, validationGroup, actionUrl, trackFocus, clientSubmit
					    // validation has already been performed
					    WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("", "", false, "", "", false, true));
				    }
				    else if ("function" == typeof __doPostBack)
				    {
					    __doPostBack("","");
				    }
				    else
				    {
					    document.forms[0].submit();
				    }
                }
			    // Macintosh FF does not fire off onload event.  Therefore, the left pane is not collapsed and the tabs are not enabled.
			    if (jsIsMac)
			    {
			        Ektron.ready(function()  {
			            WaitOnLoadAction();
			        });
			    }
			    Ektron.ready(function() {
			        $ektron("a[href='#dvSummary']").click(function(e) {
			            updateMergeFieldList("content_html", "content_teaser");
			        });
			    });
            //--><!]]>
        </script>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
                Ektron.ready( function()
                    {
                        //TABS
                        var tabsContainers = $ektron(".tabContainer");
                        tabsContainers.tabs();

                        // PLEASE WAIT MODAL
                        $ektron("#pleaseWait").modal(
                            {
                                trigger: '',
                                modal: true,
                                toTop: true,
                                onShow: function(hash)
                                {
                                    hash.o.fadeIn();
                                    hash.w.fadeIn();
                                },
                                onHide: function(hash)
                                {
                                    hash.w.fadeOut("fast");
			                        hash.o.fadeOut("fast", function()
			                            {
				                            if (hash.o)
				                            {
					                            hash.o.remove();
			                                }
			                            }
			                        );
                                }
                            }
                        );
                        //Tag Modal
                        $ektron("#newTagNameDiv").modal(
                            {
                                trigger: '',
                                modal: true,
                                toTop: true,
                                onShow: function(hash)
                                {
                                    hash.o.fadeIn();
                                    hash.w.fadeIn();
                                },
                                onHide: function(hash)
                                {
                                    hash.w.fadeOut("fast");
			                        hash.o.fadeOut("fast", function()
			                            {
				                            if (hash.o)
				                            {
					                            hash.o.remove();
			                                }
			                            }
			                        );
                                }
                            }
                        );
                    }
                );
            //--><!]]>
        </script>
        <style type="text/css">
              <!--/*--><![CDATA[/*><!--*/
                body {background-image:url("images/ui/loading_big.gif");background-repeat:no-repeat;background-position:50% 50%;}
                form {visibility:hidden;}

                #DragDropContainer {
                    background-color:silver;
                    border: solid thin black;
                }
                #ReplaceMsg{
                    padding:2px;
                    border-bottom: solid thin black;
                }
               div#newTagNameDiv
               {
                    height: 95px;
                    width:350px;
                    margin: 10em 0 0 -15em;
                    border: solid 1px #aaaaaa;
                    z-index: 10;
                    background-color: white;
               }
               div#pleaseWait
                {
                    width: 128px;
                    height: 128px;
                    margin: -64px 0 0 -64px;
                    background-color: #fff;
                    background-image: url("images/ui/loading_big.gif");
                    background-repeat: no-repeat;
                    text-indent: -10000px;
                    border: none;
                    padding: 0;
                    top: 50%;
                }
                div.selected_editor{height:95%;position:absolute;display:block;width:95%;}
                div.unselected_editor{height:95%;display:none;position:absolute;width:95%;}
                div img.reportImgHidden {visibility:hidden;}
                li.inline { display:inline;}
                a.inlineBlock {display:inline block!important;}
                .selectContent { background-image: url('Images/ui/icons/check.png');background-repeat: no-repeat;background-position:.5em center; }
                .useCurrent{ background-image: url('Images/ui/icons/shape_square.png'); background-repeat: no-repeat; background-position:.5em center; }

                table.pseudoGrid
                {
                	border-spacing:0;
	                border-collapse:collapse;
	                background-color:White;
                }

                table.pseudoGrid tr td {
	                padding: .25em .5em;
	                vertical-align: middle;
	                text-align:left;
                }

                table.pseudoGrid .label
                {
                	width: 1%;
                	color:#1d5987;
                    white-space:nowrap;
	                font-weight:bold;
	            }
            /*]]>*/-->
        </style>
    </head>
    <body class="UiMain" onload="WaitOnLoadAction();">
        <form name="frmMain" id="frmMain" action="edit.aspx" method="post" runat="server">
            <asp:Literal ID="StyleSheetJS" runat="server" />
            <asp:Literal ID="jsEditorScripts" runat="server" />
            <asp:Literal ID="EnhancedMetadataScript" runat="server" />
            <asp:Literal ID="EnhancedMetadataArea" runat="server" />
            <asp:Literal ID="ClosePanel" runat="server" />
            <script type="text/javascript" src="java/empjsfunc.js">
</script>
            <script type="text/javascript" src="java/toolbar_roll.js">
</script>
            <script type="text/javascript">
                <!--//--><![CDATA[//><!--
				    var timerId = 0;
				    // Give the editors time to load, return after delay to hide the navigation tree.
				    function WaitOnLoadAction()
				    {
					    if (timerId)
					    {
						    clearTimeout(timerId);
					    }
					    var wait = 0; //ContentDesigner does not need extra time
					    if (typeof eWebEditPro != "undefined" && eWebEditPro)
					    {
					        wait = 1500;
					    }
					    timerId = setTimeout("DelayedResizeHideFunc()", wait);
					    setTimeout("if (!bContentEditorReady) initTransferMethod('content_html', 'mediamanager.aspx', 'autoupload.aspx')", 2000);
					    setTimeout("if (!bTeaserEditorReady) initTransferMethod('content_teaser', 'mediamanager.aspx', 'autoupload.aspx')", 2000);
                    }

    			    function DelayedResizeHideFunc()
				    {
					    ResizeFrame(0); // Hide the navigation-tree frame.
					    bEnableTabs = true;
					    if (g_visiblePane != g_initialPaneToShow)
					    {
						    ShowPane(g_initialPaneToShow);
					    }

				        if ("object" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances)
				        {
					        var objContentDesigner = Ektron.ContentDesigner.instances["content_teaser"];
					        if (objContentDesigner != null)
					        {
				                setResponseActionContentDesigner(initialSummaryPane);
					        }
					    }
				    }

				    var AutoNav = "<asp:literal id="AutoNav" runat="server"/>";
				    var invalidFormatMsg = "<asp:literal id="invalidFormatMsg" runat="server"/>";
				    var invalidYearMsg = "<asp:literal id="invalidYearMsg" runat="server"/>";
				    var invalidMonthMsg = "<asp:literal id="invalidMonthMsg" runat="server"/>";
				    var invalidDayMsg = "<asp:literal id="invalidDayMsg" runat="server"/>";
				    var invalidTimeMsg = "<asp:literal id="invalidTimeMsg" runat="server"/>";
				    var initialSummaryPane = "<asp:literal id="initialSummaryPane" runat="server"/>";
				    var buttonPressed = false;

				    var ecmMetaComplete = "<asp:literal id="ecmMetaComplete" runat="server"/>"; // used for both IE and NS
				    ecmMonths = "<asp:literal id="ecmMonths" runat="server"/>"; // Both IE and NS

				    function CheckKeyValue(item, keys)
				    {
					    var keyArray = keys.split(",");
					    for (var i = 0; i < keyArray.length; i++)
					    {
						    if ((document.layers) || ((!document.all) && (document.getElementById)))
						    {
							    if (item.which == keyArray[i])
							    {
								    return false;
							    }
						    }
						    else {
							    if (event.keyCode == keyArray[i])
							    {
								    return false;
							    }
						    }
					    }
				    }

				    function CheckTitle (Button)
				    {
					    
					    var objForm = document.forms[0];
					    if (objForm.content_title != null)
					    {
				            objForm.content_title.value = Trim(objForm.content_title.value);
				            if (objForm.content_title.value == "" && "" == "<%= Request.QueryString["multi"] %>")
				            {
					            alert("<asp:literal id="jsNullContent" runat="server"/>");
					            objForm.content_title.focus();
					            return false;
				            }
				            else if(objForm.content_title.className == "masterlayout")
				            {
				                var field = objForm.content_title.value;
                                if ((field.indexOf("\\") >= 0) || (field.indexOf(":") >= 0)||(field.indexOf("*") >= 0) || (field.indexOf("?") >= 0)|| (field.indexOf("\"") >= 0) || (field.indexOf("<") >= 0)|| (field.indexOf(">") >= 0) || (field.indexOf("|") >= 0) || (field.indexOf("&") >= 0) || (field.indexOf("\'") >= 0))
	                            {
		                            alert("The title of a master layout can't include ('\\', ':', '*', '?', ' \" ', '<', '>', '|', '&', '\'').");
		                            return false;
	                            }
				            }
				            else if((objForm.content_title.value.indexOf('*') > -1) || (objForm.content_title.value.indexOf('>') > -1)||(objForm.content_title.value.indexOf('<') > -1)||(objForm.content_title.value.indexOf('|') > -1)||(objForm.content_title.value.indexOf('\"') > -1))
                            {
                                alert("The title cannot contain '*','>','<','|','\"'.");
                                return false;
                            }
                          
					    }

				       if(document.getElementById("prev_frm_manalias_name") != null && document.getElementById("prev_frm_manalias_ext") != null)
				       {
				           var prevAliasName = document.getElementById("prev_frm_manalias_name").value + document.getElementById("prev_frm_manalias_ext").value ;
				           var currAliasName = document.getElementById("frm_manalias").value + document.getElementById("frm_manaliasExt").value ;

				           if( prevAliasName != currAliasName)
				           {
				               if(CheckAliasName()==false)
				               {
				                     return false;
				               }
                            }
                         }
					    objForm.go_live.value = Trim(objForm.go_live.value);
					    objForm.end_date.value = Trim(objForm.end_date.value);
					    if (objForm.end_date.value != "" && objForm.go_live.value != "")
					    {
						    if (!EkDTCompareDates(objForm.go_live, objForm.end_date))
						    {
							    var msg = "<asp:literal id="jsEDWarning" runat="server"/>";
							    if (!confirm(msg))
							    {
								    return false;
							    }
						    }
					    }

					    <asp:literal id="ValidateContentPanel" visible="False" runat="server"/>
					    return true;

				    }

				    function CheckAliasName()
	                {
	                     var returnValue = false;
	                     var contLang = "<%=m_intContentLanguage %>";
	                     var folderId = "<%=m_intContentFolder%>";
	                     var aliasName = "";
	                     aliasName = document.getElementById('frm_manalias').value;
	                     if(aliasName.indexOf("+") != -1)
	                     {

	                        aliasName = aliasName.replace(/\+/g,"%2B");
	                     }
	                    $ektron.ajax({
                              url: "urlaliasdialoghandler.ashx?action=checkaliasname&aliasname=" + aliasName + "&fileextension=" + document.getElementById("frm_manaliasExt").value + "&langtype=" + contLang + "&folderid=" + folderId,
                              cache: false,
                              async: false,
                              success: function(html){
                                     if (html.indexOf("<aliasname>") != -1)
                                     {
                                       returnValue = true;
                                       return true;
                                     }
                                     else
                                     {
                                        alert(html);
                                        returnValue = false;
                                        return false;
                                     }

                              }});
                              return returnValue;
	                   }


				    // cmsedit.js :: function eWebEditor_save(objValueDestination,objNotify,fnNotify,bValidateContent) {}

				    // cmsedit.js :: function CheckContentSize()

				    function DisplayMetaIncomplete() {
					    alert("<asp:literal id="jsMetaCompleteWarning" runat="server"/>");
				    }

				    <asp:literal id="jsSetActionFunction" runat="server"/>

				    // cmsedit.js :: function ShutdownImageEditor() {}

				    // cmsedit.js :: function replaceAll(inStr, searchStr, replaceStr) {}

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
				    function DisplayHoldMsg_Local(flag) {
					    var tabArray = new Array("_dvContent", "_dvPollWizard", "_dvSummary", "_dvMetadata", "_dvSchedule", "_dvComment","_dvSubscription", "_dvTemplates","_dvTaxonomy");

					    // ensure in normal viewing mode:
					    SetFullScreenView(false);

					    // Now make room for the message:
					    SetObjVisible("upperTable", !flag);
					    for (var i=0; i < tabArray.length; i++) {
						    SetPaneVisible(tabArray[i].substr(1), !flag);
					    }

					    // call the standard function:
					    DisplayHoldMsg(flag);
				    }
				    Ektron.ready(function() {
			            window.onbeforeunload = confirmExit;
			            function confirmExit() 
			            {
                            if ("" == document.forms[0].editaction.value) return "<%=m_refMsg.GetMessage("js: alert confirm close no save")%>";
			            }
			        });
                //--><!]]>
            </script>
            <script type="text/javascript">
                <!--//--><![CDATA[//><!--
	                // ServerControl :: eWebEditPro.actionOnUnload = EWEP_ONUNLOAD_NOSAVE;
	                // ServerControl :: eWebEditPro.parameters.locale = "<asp:literal id="jsEditProLocale" runat="server"/>";
	                // ServerControl :: eWebEditPro.parameters.baseURL = "<asp:literal id="jsSitePath" runat="server"/>";
	                // ServerControl :: editorEstimateContentSize = "body";
                //--><!]]>
            </script>
            <asp:Literal ID="PostBackPage" runat="server" />
            <div class="ektronWindow" id="pleaseWait">
                <h3><asp:Literal ID="LoadingImg" runat="server" /></h3>
            </div>
            <div id="dhtmltooltip"></div>
            <div class="ektronPageHeader" id="editContentToolbar">
                <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
                <div class="ektronToolbar" id="htmToolBar" runat="server">
		            <asp:RegularExpressionValidator ID="ContentValidator" runat="server" />
                    <asp:RegularExpressionValidator ID="SummaryValidator" runat="server" />
                    <asp:Button ToolTip="Include Page Client" ID="Include_Page_ClientValidate" style="display:none" runat="server" />
                </div>
            </div>
            <div class="ektronPageContainer ektronPageInfo" id="editContentPageContainer">
                <div class="ektronTabUpperContainer">
                    <table cellspacing="0" class="pseudoGrid">
                        <tr>
                            <td class="label" title="Title"><asp:Literal ID="lbl_GenericTitleLabel" runat="server" /></td>
                            <td class="value">
                                <input type="text" title="Enter Title here" size="50" maxlength="200" name="content_title" id="content_title" onkeypress="javascript:return CheckKeyValue(event, '34,13');" runat="server" />
                                <asp:Literal ID="MultiupLoadTitleMsg" runat="server" />
                                <asp:Literal ID="lblLangName" runat="server" />
                                <asp:Literal ID="lblNotificationStatus" runat="server" />
                            </td>
                        </tr>
                    </table>
					<asp:Label ID="WarningMessage" runat="server" Visible="false"></asp:Label>
                </div>
                <asp:Panel id="TR_Properties" runat="server">
                    <asp:Literal ID="QLink_Search" runat="server" />
                </asp:Panel>
                <input type="hidden" name="netscape" id="netscape" runat="server" /><!--Conditional Enable -->
                <input type="hidden" name="content_id" id="content_id" runat="server" />
                <input type="hidden" name="eType" id="eType" runat="server" />
                <input type="hidden" name="mycollection" id="mycollection" runat="server" />
                <input type="hidden" name="addto" id="addto" runat="server" />
                <input type="hidden" name="content_folder" id="content_folder" runat="server" />
                <input type="hidden" name="editaction" id="editaction" runat="server" />
                <input type="hidden" name="type"  id="type" runat="server" />
                <input type="hidden" name="content_language" id="content_language" runat="server" />
                <%--<input type="hidden" name="contentteaser" id="contentteaser" runat="server" />--%>
                <input type="hidden" name="hiddencontentsize" id="hiddencontentsize" runat="server" />
                <input type="hidden" name="hiddensearchsize" id="hiddensearchsize" runat="server" />
                <input type="hidden" name="maxcontentsize" id="maxcontentsize" runat="server" />
                <input type="hidden" name="Ver4Editor" id="Ver4Editor" runat="server" />
                <input type="hidden" name="createtask" id="createtask" runat="server" />
                <input type="hidden" name="content_subtype" id="content_subtype" runat="server" />
                <input type="hidden" name="content_type" id="content_type" runat="server" />
                <input type="hidden" name="report_type" id="report_type" runat="server" />
                <input type="hidden" name="report_display_type" id="report_display_type" runat="server" />
                <input type="hidden" name="ast_frm_manaliasExt" id="ast_frm_manaliasExt" runat="server" />
                <input type="hidden" name="filename" id="filename" runat="server" />
                <input type="hidden" name="oldfilename" id="oldfilename" runat="server" />
                <input type="hidden" name="validTypes" id="validTypes" runat="server" />
                <input type="hidden" name="Cmd" value="Save" />
                <input type="hidden" name="NextUsing" id="NextUsing" runat ="server" />
                <input type="hidden" value="New" />
                <input type="hidden" name="putopts" id="putopts" runat="server" value="true" />
                <input type="hidden" name="destination" id="destination"  runat ="server" />
                <input type="hidden" name="Confirmation-URL" class="confirmationURL""  />
                <input type="hidden" name="PostURL"  id="PostURL"  runat ="server" />
                <input type="hidden" name="VTI-GROUP" value="0" />
                <input type="hidden" name="isOfficeDoc" id="isOfficeDoc" runat="server" />
                <input type="hidden" name="FromEE" id="FromEE" value="false" />
                <input type="hidden" runat="server" id="submitasstagingview" name="submitasstagingview" value="" />
                <input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
                <input type="hidden" name="LastClickedOnChecked" id="LastClickedOnChecked" value="false" />
                <input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value="" runat="server" />
                <input type="hidden" name="translate" id="translate" runat="server" />
                <asp:Literal ID="MultiTemplateID" runat="server" />
                <asp:Literal ID="AssetHidden" runat="server" />
                <asp:Literal ID="EnumeratedHiddenFields" runat="server" />
                <asp:Literal ID="eWebEditProJS" runat="server" />
                <script type="text/javascript" src="java/OptionTransfer.js">
</script>
                <div id="editContentContainer" class="ektronPageTabbed">
                <div id="editContentTabContainer" class="tabContainer">
                    <ul id="editContentTabs">
                        <asp:PlaceHolder ID="phContent" runat="server">
                            <li id="liContent">
                                <a title="Content" href="#dvContent">
                                    <asp:Literal ID="divContentText" runat="server" />
                                </a>
                            </li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phSummary" runat="server">
                            <li id="liSummary"><a title="Summary" href="#dvSummary">
                                <asp:Literal ID="divSummaryText" runat="server" />
                            </a></li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phMetadata" Visible="false" runat="server">
                            <li id="liMetadata">
                                <a title="MetaData" href="#dvMetadata">
                                    <%=m_refMsg.GetMessage("metadata text")%>
                                </a>
                            </li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phAlias" runat="server">
                            <li id="liAlias">
                                <a title="Alias" href="#dvAlias">
                                    <%=m_refMsg.GetMessage("lbl alias")%>
                                </a>
                            </li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="PhSchedule" runat="server">
                        <li id="liSchedule">
                            <a title="Schedule" href="#dvSchedule">
                                <%=m_refMsg.GetMessage("schedule text")%>
                            </a>
                        </li>
                         </asp:PlaceHolder>
                        <asp:PlaceHolder ID="PhComment" runat="server">
                        <li id="liComment">
                            <a title="Comments" href="#dvComment">
                                <%=m_refMsg.GetMessage("comment text")%>
                            </a>
                        </li>
                         </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phSubscription" runat="server">
                            <li id="liSubscription">
                                <a title="Subscription" href="#dvSubscription">
                                    <%=m_refMsg.GetMessage("lbl web alert tab")%>
                                </a>
                            </li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phTemplates" runat="server">
                            <li id="liTemplates">
                                <a title="Templates" href="#dvTemplates">
                                    <%=m_refMsg.GetMessage("generic templates lbl")%>
                                </a>
                            </li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phTaxonomy" runat="server">
                            <li id="liTaxonomy">
                                <a title="Taxonomy" href="#dvTaxonomy">
                                    <%=m_refMsg.GetMessage("viewtaxonomytabtitle")%>
                                </a>
                            </li>
                        </asp:PlaceHolder>
                    </ul>
                    <asp:Literal ID="EditMetadataHtml" runat="server" />
                    <asp:Literal ID="EditAliasHtml" runat="server" />
                    <asp:Literal ID="EditCommentHtml" runat="server" />
                    <asp:Literal ID="EditSubscriptionHtml" runat="server" />
                    <asp:Literal ID="EditScheduleHtml" runat="server" />
                    <asp:Literal ID="EditTemplateHtml" runat="server" />
                    <asp:Literal ID="EditTaxonomyHtml" runat="server" />
                    <asp:PlaceHolder ID="phEditContent" runat="server" />
                    <asp:PlaceHolder ID="phEditSummary" runat="server" />
                    <asp:Literal ID="Summary_Meta_Win" runat="server" />
                </div>
            </div>
                <script type="text/javascript">
                <!--//--><![CDATA[//><!--
                    var nLimit;
                    var temp;
                    var temp = "<asp:literal id="jsValidCounter" runat="server"/>";
                    if (temp == "")
                    {
                        nLimit = 0;
                    }
                    else
                    {
                        nLimit = parseInt(temp);
                        if (isNaN(nLimit))
                        {
                            nLimit = 0;
                        }
                    }
                    if (nLimit)
                    {
                        if (document.forms.frmMain.frm_text_1)
                        {
                            try
                            {
	                            if (document.forms.frmMain.frm_text_1.type  != "hidden")
	                            {
	                            document.forms.frmMain.frm_text_1.focus();
                            }
                            }
                            catch(er)
                            {
                            }
                        }
                    }
                //--><!]]>
            </script>
                <script type="text/javascript">
                <!--//--><![CDATA[//><!--
                    <asp:literal id="jsActionOnUnload" runat="server"/>
                //--><!]]>
            </script>
                <script type="text/javascript">
                <!--//--><![CDATA[//><!--
                    if (typeof eWebEditPro == "object")
                    {
                        var objInstance = eWebEditPro.instances["content_html"];
                        if (objInstance && "activex" == objInstance.type)
                        {
	                        // Wait for onready when editor is ActiveX control
	                        b_cms_lnReady = false;
	                        b_cms_lnEnable = false;
                        }
                        else
                        {
	                        bContentEditorReady = true;
	                        bTeaserEditorReady = true;
	                        bFormEditorReady = true;
	                        bResponseEditorReady = true;
                        }
                        eWebEditPro.addEventHandler("onready", "initTransferMethod(eWebEditPro.event.srcName, 'mediamanager.aspx', 'autoupload.aspx')");
                        eWebEditPro.addEventHandler("ontoolbarreset", "initTransferMethod(eWebEditPro.event.srcName, 'mediamanager.aspx', 'autoupload.aspx')");
                        
                        //TODO : Add jsfieldlist to the list of custom commands in the SC
                        eWebEditProExecCommandHandlers["jsfieldlist"] = InsertMergeField;
                    }
                    else
                    {
                        bContentEditorReady = true;
                        bTeaserEditorReady = true;
                        bFormEditorReady = true;
                        bResponseEditorReady = true;
                    }
                    function InsertMergeField(sEditorName, strCmdName, strTextData, lData)
                    {
                        var objInstance = eWebEditPro.instances[sEditorName];
                        if (objInstance && objInstance.isEditor())
                        {
	                        var iSelectedIndex;
	                        if (lData <= 0) return;
	                        iSelectedIndex = lData - 1;
	                        var objField = g_aryFormFieldList.fields[iSelectedIndex];
	                        var sTagName = "ektdesignns_mergefield";
	                        var sName = objField.name;
	                        var sDataType = objField.datatype.toLowerCase();
	                        if ("selection" == sDataType)
	                        {
		                        var strText = "ektdesignns_name=\"" + objField.name + "\"";
		                        strText += serializeAttribute("title", objField.displayName);
		                        strText += serializeAttribute("ektdesignns_datatype", objField.datatype);
		                        strText += serializeOptionalAttribute("ektdesignns_basetype", objField.basetype);
		                        strText += serializeOptionalAttribute("ektdesignns_content", objField.content);
		                        strText += serializeDatalistAttributes(objField);
		                        strText += serializeAttribute("ektdesignns_bind", objField.xpath);
		                        objInstance.editor.ExecCommand("cmddsgmergelist", strText, 0);
	                        }
	                        else
	                        {
		                        if ("date" == sDataType.substr(0,4))
		                        {
			                        sTagName = "ektdesignns_calendar";
			                        //sName = sName + "_iso";
		                        }
		                        var strHtml = "<" + sTagName;
		                        strHtml += " ektdesignns_name=\"" + sName + "\"";
		                        strHtml += serializeAttribute("ektdesignns_datatype", objField.datatype);
		                        strHtml += serializeOptionalAttribute("ektdesignns_basetype", objField.basetype);
		                        strHtml += serializeOptionalAttribute("ektdesignns_content", objField.content);
		                        strHtml += serializeDatalistAttributes(objField);
		                        strHtml += serializeAttribute("ektdesignns_bind", objField.xpath);
		                        strHtml += ">&#171;";
		                        strHtml += objField.displayName;
		                        strHtml +=  "&#187;</" + sTagName + ">";
		                        objInstance.editor.pasteHTML(strHtml);
	                        }
	                        var objMenu = objInstance.editor.Toolbars();
	                        if (objMenu)
	                        {
		                        var objCommand = objMenu.CommandItem("jsfieldlist");
		                        if (objCommand)
		                        {
			                        // Set value in drop-down list back to the first item, ie, "(Insert Field)"
			                        objCommand.setProperty("CmdIndex",0);
		                        }
	                        }
                        }
                    }

                    function getDatalistByName(sDataList)
                    {
                        for (var i = 0; i < g_aryFormFieldList.datalists.length; i++)
                        {
	                        if (sDataList == g_aryFormFieldList.datalists[i].name)
	                        {
		                        return g_aryFormFieldList.datalists[i];
	                        }
                        }
                        return null;
                    }

                    function serializeDatalistAttributes(oField)
                    {
                        if (!oField) return "";
                        var strHtml = "";
                        strHtml += serializeOptionalAttribute("ektdesignns_datalist", oField.datalist);
                        if (strHtml.length > 0)
                        {
	                        var dl = getDatalistByName(oField.datalist);
	                        strHtml += serializeOptionalAttribute("ektdesignns_datasrc", dl.datasrc);
	                        strHtml += serializeOptionalAttribute("ektdesignns_dataselect", dl.dataselect);
	                        strHtml += serializeOptionalAttribute("ektdesignns_captionxpath", dl.captionxpath);
	                        strHtml += serializeOptionalAttribute("ektdesignns_valuexpath", dl.valuexpath);
	                        strHtml += serializeOptionalAttribute("ektdesignns_datanamespaces", dl.datanamespaces);
                        }
                        return strHtml;
                    }

                    function serializeAttribute(name, value)
                    {
                        return " " + name + "=\"" + eWebEditProUtil.HTMLEncode(value) + "\"";
                    }

                    function serializeOptionalAttribute(name, value)
                    {
                        if ("string" == typeof value && value.length > 0)
                        {
	                        return serializeAttribute(name, value);
                        }
                        return "";
                    }
                 //--><!]]>
            </script>
                <script type="text/javascript">
                <!--//--><![CDATA[//><!--
                    var g_prevResponseAction = (initialSummaryPane || "");
                    var g_bResettingToolbar = false;

                    function showReportOptions () {
                        var strTemp1 = '';
                        var strTemp2 = '';


                        if (document.getElementById("rptType").disabled)
                        {
                            document.getElementById("rptType").disabled = false;
                            document.getElementById("rptDisplayType").disabled = false;
                        }
                        setResponseAction("report");
                        return false;
                    }

                    function setReportOptions(rptOption)
                    {
                        if (rptOption == "rptType") {
                            document.getElementById("rptType").value = document.getElementById("rptType")[document.getElementById("rptType").selectedIndex].value;
                            setResponseAction("report");
                        }
                        else if (rptOption == "rptDisplayType") {
                            document.getElementById("rptDisplayType").value = document.getElementById("rptDisplayType")[document.getElementById("rptDisplayType").selectedIndex].value;
                            setResponseAction("report");
                        }
                        return false;
                    }
                    function setResponseActionContentDesigner(action)
                    {
                        frmDesDisableItem("rptDisplayType");
                        frmDesDisableItem("rptType");

                        var objElemStandard = document.getElementById("_dvSummaryStandard");
                        var objElemRedirect = document.getElementById("_dvSummaryRedirect");
                        var objElemTransfer = document.getElementById("_dvSummaryTransfer");
                        var objElemReport = document.getElementById("_dvSummaryReport");
                        var objElemReportImg = document.getElementById("_imgFormSummaryReport");

                        if(objElemStandard == null || objElemRedirect == null || objElemTransfer == null || objElemReport == null)
                            return;

                        g_prevResponseAction = action;

                        switch (action)
                        {
                            case "message":
                                objElemStandard.className = jsSelectedDivStyleClass;
                                objElemRedirect.className = jsUnSelectedDivStyleClass;
                                objElemTransfer.className = jsUnSelectedDivStyleClass;
                                objElemReport.className = jsUnSelectedDivStyleClass;
                                objElemReportImg.className = "reportImgHidden";
                                break;
                            case "redirect":
                                objElemStandard.className = jsUnSelectedDivStyleClass;
                                objElemRedirect.className = jsSelectedDivStyleClass;
                                objElemTransfer.className = jsUnSelectedDivStyleClass;
                                objElemReport.className = jsUnSelectedDivStyleClass;
                                objElemReportImg.className = "reportImgHidden";
                                break;
                            case "transfer":
                                objElemStandard.className = jsUnSelectedDivStyleClass;
                                objElemRedirect.className = jsUnSelectedDivStyleClass;
                                objElemTransfer.className = jsSelectedDivStyleClass;
                                objElemReport.className = jsUnSelectedDivStyleClass;
                                objElemReportImg.className = "reportImgHidden";
                                break;
                            case "report":
                                frmDesEnableItem("rptDisplayType");
                                frmDesEnableItem("rptType");
                                objElemStandard.className = jsUnSelectedDivStyleClass;
                                objElemRedirect.className = jsUnSelectedDivStyleClass;
                                objElemTransfer.className = jsUnSelectedDivStyleClass;
                                objElemReport.className = jsSelectedDivStyleClass;
                                objElemReportImg.className = "reportImgVisible";
                                break;
                            default:
                                return;
                        }
                    }


                    function setResponseAction(action)
                    {
                        if(action == g_prevResponseAction)
                        {
                            return;
                        }

                        var objInstance = null;
                        if (typeof eWebEditPro != "undefined" && eWebEditPro)
                        {
                            objInstance = eWebEditPro.instances["content_teaser"];
                        }
                        if (!objInstance || !objInstance.isEditor())
                        {
                            var objContentDesigner = Ektron.ContentDesigner.instances["content_teaser"];
                            if (objContentDesigner != null)
                            {
                                return setResponseActionContentDesigner(action);
                            }
                            return;
                        }

                        //Below is eWebEditPro Only
                        var objEditor = objInstance.editor;
                        var strFieldName = "";

                        // These will be enabled if report is specified.
                        frmDesDisableItem("rptDisplayType");
                        frmDesDisableItem("rptType");

                        // The action 'message' is an action that will require the resetting
                        // of the toolbar when leaving.
                        if("message" == g_prevResponseAction)
                        {
                            if(
                                ("transfer" == action) ||
                                ("redirect" == action) ||
                                ("report" == action)
                              )
                            {
                                frmDesDisableFormDesignDirection();
                            }
                        }
                        else if ("message" == action)
                        {
                            frmDesDisableFormDesignDirection();
                        }


                        if ("transfer" == action)
                        {
                            strFieldName = "transfer_page";
                            changeFieldSetType("redirection_page", "");
                            changeFieldSetType("report_page", "");
                        }
                        else if ("redirect" == action)
                        {
                            strFieldName = "redirection_page";
                            changeFieldSetType("transfer_page", "");
                            changeFieldSetType("report_page", "");
                        }
                        else if ("report" == action)
                        {
                            frmDesEnableItem("rptDisplayType");
                            frmDesEnableItem("rptType");

                            strFieldName = "report_page";
                            changeFieldSetType("redirection_page", "");
                            changeFieldSetType("transfer_page", "");
                        }
                        changeFieldSetType(strFieldName, "dataentryxslt");

                        if ("message" != action && "message" != g_prevResponseAction)
                        {
                            // do not need to switch editor but need to change the xslt
                            var objElem = objInstance.linkedElement(strFieldName);
                            if (objElem != null)
                            {
	                            var strContent;
	                            if ("report" != g_prevResponseAction)
	                            {
		                            // A GetContent will make content 'not changed', but only for a short time in data entry mode.
		                            strContent = objEditor.GetContent("datadocumentxml");
	                            }
	                            else
	                            {
		                            // report mode would not have any content in the datadocumentxml in the editor
		                            // need to get the datadocumentxml content from its hidden field.
		                            var objData = objInstance.linkedElement("redirection_data");
		                            if (objData != null)
		                            {
			                            strContent = objData.value;
		                            }
	                            }
	                            /* IMPORTANT:
		                            Get content then change the XSLT. Lastly reload the content.
		                            The content must be set AFTER the XSLT is set, otherwise
		                            the content will be transformed by the existing XSLT and changing
		                            the XSLT after that will have no effect.
	                            */
	                            objEditor.SetContent("dataentryxslt", objElem.value, "");
	                            objEditor.SetContent("datadocumentxml", strContent, "");
                            }
                            g_prevResponseAction = action;
                        }
                        else
                        {
                            // need to reload toolbar.
                            if (action != "message")
                            {
	                            // do not save the content on message mode.
	                            // Otherwise it will overwrite the content_teaser hidden field.
	                            changeFieldName("content_teaser", "redirection_data"); // will save in separate hidden fields
	                            objInstance.save(undefined, undefined, undefined, false); // non-validating save
                            }
                            g_prevResponseAction = action;
                            var strConfigURL = eWebEditProPath;
                            switch (action)
                            {
                            case "message":
	                            strConfigURL += "cms_config.aspx?mode=xsltdesign";
	                            // mask fields to run in Presentation Design mode
	                            changeEditorNameOfFields("content_teaser", "not_redirect");
	                            break;
                            case "redirect":
                            case "transfer":
	                            strConfigURL += "cms_config.aspx?mode=dataentry&InterfaceName=none";
	                            // unmask to run in Data Entry mode
	                            changeEditorNameOfFields("not_redirect", "content_teaser");
	                            changeFieldGetType("redirection_data", "datadocumentxml");
	                            break;
                            case "report":
	                            strConfigURL += "cms_config.aspx?mode=dataentry&InterfaceName=none";
	                            // unmask to run in Data Entry mode
	                            changeEditorNameOfFields("not_redirect", "content_teaser");
	                            changeFieldGetType("redirection_data", "");
	                            break;
                            default:
	                            return;
                            }
                            objEditor.setProperty("Config", strConfigURL);
                            g_bResettingToolbar = true;
                            eWebEditPro.ontoolbarreset = reloadContentTeaser;
                            objEditor.ExecCommand("toolbarreset", "", 0);
                        }
                    }

                    function frmDesDisableFormDesignDirection()
                    {
                        frmDesDisableItem("response_message");
                        frmDesDisableItem("response_redirect");
                        frmDesDisableItem("response_transfer");
                        frmDesDisableItem("response_report");
                    }

                    function frmDesEnableFormDesignDirection()
                    {
                        frmDesEnableItem("response_message");
                        frmDesEnableItem("response_redirect");
                        frmDesEnableItem("response_transfer");
                        frmDesEnableItem("response_report");
                    }

                    function frmDesDisableItem(strItemName)
                    {
                        var objField = document.getElementById(strItemName);
                        if(objField)
                        {
                            objField.setAttribute("disabled", "disabled");
                        }
                        objField = document.getElementById("lbl_" + strItemName);
                        if(objField)
                        {
                            objField.setAttribute("disabled", "disabled");
                        }
                    }

                    function frmDesEnableItem(strItemName)
                    {
                        var objField = document.getElementById(strItemName);
                        if(objField)
                        {
                            objField.removeAttribute("disabled");
                        }
                        objField = document.getElementById("lbl_" + strItemName);
                        if(objField)
                        {
                            objField.removeAttribute("disabled");
                        }
                    }

                    function reloadContentTeaser()
                    {
                        // eWebEditPro only
                        if (eWebEditPro.event.srcName != "content_teaser") return;
                        eWebEditPro.ontoolbarreset = undefined;
                        var objInstance = eWebEditPro.instances["content_teaser"];
                        if (!objInstance || !objInstance.isEditor()) return;
                        objInstance.load();
                        changeFieldName("redirection_data", "content_teaser"); // will save to the main hidden field
                        updateMergeFieldList("", "content_teaser");
                        g_bResettingToolbar = false;
                    }

                    function changeEditorNameOfFields(from_name, to_name)
                    {
                        //eWebEditPro Only
                        var aryFields = eWebEditPro.fields;
                        if (aryFields)
                        {
                            for (var i = 0; i < aryFields.length; i++)
                            {
	                            var fld = aryFields[i];
	                            if (fld.editorName == from_name)
	                            {
		                            fld.editorName = to_name;
	                            }
                            } // for
                        }
                    }

                    function changeFieldName(from_name, to_name)
                    {
                        //eWebEditPro Only
                        var aryFields = eWebEditPro.fields;
                        if (aryFields)
                        {
                            for (var i = 0; i < aryFields.length; i++)
                            {
	                            var fld = aryFields[i];
	                            if (fld.name == from_name)
	                            {
		                            fld.name = to_name;
	                            }
                            } // for
                        }
                    }
                    function changeFieldSetType(name, setType)
                    {
                        //eWebEditPro Only
                        var aryFields = eWebEditPro.fields;
                        if (aryFields)
                        {
                            for (var i = 0; i < aryFields.length; i++)
                            {
	                            var fld = aryFields[i];
	                            if (fld.name == name)
	                            {
		                            fld.setType = setType;
	                            }
                            } // for
                        }
                    }
                    function changeFieldGetType(name, getType)
                    {
                        //eWebEditPro Only
                        var aryFields = eWebEditPro.fields;
                        if (aryFields)
                        {
                            for (var i = 0; i < aryFields.length; i++)
                            {
	                            var fld = aryFields[i];
	                            if (fld.name == name)
	                            {
		                            fld.getType = getType;
	                            }
                            } // for
                        }
                    }
                //--><!]]>
            </script>
                <script type="text/javascript">
                        <!--//--><![CDATA[//><!--
                            if ("object" == typeof eWebEditPro)
                            {
	                            var objInstance = eWebEditPro.instances["content_teaser"];
	                            if (objInstance && "activex" == objInstance.type)
	                            {
		                            // Wait for onready when editor is ActiveX control
		                            b_cms_lnReady = false;
		                            b_cms_lnEnable = false;
	                            }
	                            else
	                            {
		                            bTeaserEditorReady = true;
	                            }
                            }
                            else
                            {
	                            bTeaserEditorReady = true;
                            }

                            if (bTeaserEditorReady && "function" == typeof frmDesEnableFormDesignDirection)
                            {
	                            frmDesEnableFormDesignDirection();
                            }
                            CommentPopUpPage="commentpopup.aspx?ref_type=C&id="+jsId+"&LangType="+jsContentLanguage;
                            ValidationPopUpPage="validation_main.aspx?editor_name=content_html&id="+jsId+"&LangType="+jsContentLanguage;
                            CommentSaveType="<%=Request.QueryString["type"]%>";
                            docid=jsId;
                            var defaultFolderId="<asp:literal id="defaultFolderId" runat="server"/>";
                            folderId = defaultFolderId;
                        //--><!]]>
                    </script>
                <asp:PlaceHolder ID="phNewFormWizard" runat="server" />
                <asp:Literal ID="PollPaneHtml" runat="server" />
                <asp:Literal ID="UpdateFieldJS" runat="server" />
                <br />

                <%if(TaxonomyRoleExists) {%>

                <div id="FrameContainer" style="position: absolute; top: 0px; left: 0px; width: 1px; height: 1px; display: none; z-index: 1000;">
                    <iframe id="ChildPage" src="javascript:false;" frameborder="1" marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="auto" style="background-color: white;">
                    </iframe>
                </div>

                <%}%>
            </div>
        </form>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
	            function design_makeUnique()
	            {
		            // extract from ewebeditpro project design.js
		            // correct uniqueness for all id attributes and assoc labels.
		            // correct uniqueness for list item names.
		            var strUniqueSuffix = Math.floor(Math.random() * 1679616).toString(8); //7 digit alphanum
		            return strUniqueSuffix;
	            }
	            if ("object" == typeof oProgressSteps && oProgressSteps != null)
	            {
	                var objInstance = null;
	                if (typeof eWebEditPro != "undefined" && eWebEditPro)
	                {
		                objInstance = eWebEditPro.instances["content_html"];
		            }
		            var sFormType = '<%=Request.QueryString["form_type"]%>';
		            oProgressSteps.onselect = function(stepNumber)
		            {
			            switch (this.getStep(stepNumber).id)
			            {
			            case "response":
				            if ("poll" == sFormType)
				            {
					            ShowPane('dvPollWizard');
				            }
				            else
				            {
					            ShowPane('dvSummary');
				            }
				            break;
			            case "metadata":
				            ShowPane('dvMetadata');
				            break;
			            case "schedule":
				            ShowPane('dvSchedule');
				            break;
			            case "done":
				            if ("poll" == sFormType)
				            {
					            SetPaneVisible('dvPollWizard', false);
				            }
				            else
				            {
					            SetPaneVisible('dvSummary', false);
				            }
				            break;
			            default:
				            break;
			            }
		            }
		            oProgressSteps.ondone = function()
		            {
			            if ("poll" == sFormType)
			            {
				            //construct poll form from the question and choices
				            var sQuestion = document.getElementById("frm_Question").value;
				            var aChoice = new Array();
				            var aValue = new Array();
				            var bHasChoice = false;
				            var numPollChoices = document.getElementById("numPollChoices").value;
				            if (sQuestion.length > 1000)
				            {
					            sQuestion = sQuestion.substr(0,1000);
				            }
				            for (var i = 1; i <= numPollChoices; i++)
				            {
					            aChoice[i] = document.getElementById("frm_Choice" + i).value;
					            if (aChoice[i].length > 50)
					            {
						            aChoice[i] = aChoice[i].substr(0,50);
					            }
					            aValue[i] = aChoice[i].replace(/,/g, "_");
					            aValue[i] = aValue[i] + "_" + i;
					            if (aChoice[i].length > 0)
					            {
						            bHasChoice = true;
					            }
				            }
				            var randId;
				            var sFieldName = "ektpoll";
				            var dNow = new Date();
				            var sNow = dNow.getTime().toString();
				            sFieldName += sNow + "";
				            var sPollHtml = "";
				            if (sQuestion.length > 0)
				            {
					            sPollHtml += eWebEditProUtil.HTMLEncode(sQuestion) + "<br/>";
				            }
				            if (bHasChoice)
				            {
					            sFieldName = eWebEditProUtil.HTMLEncode(sFieldName);
					            sQuestion = eWebEditProUtil.HTMLEncode(sQuestion);
					            if (objInstance)
					            {
					                sPollHtml += "<ektdesignns_choices";
					            }
					            else
					            {
					                sPollHtml += "<div class=\"ektdesignns_choices\"";
					            }
					            sPollHtml += " id=\"" + sFieldName + "\" ektdesignns_name=\"" + sFieldName + "\" name=\"" + sFieldName + "\" ektdesignns_caption=\"" + sQuestion + "\" title=\"" + sQuestion + "\" ektdesignns_nodetype=\"element\">";
					            sPollHtml += "<ol class=\"design_list_vertical\" title=\"" + sQuestion + "\" contenteditable=\"false\" unselectable=\"on\" ektdesignns_minoccurs=\"1\" ektdesignns_maxoccurs=\"1\" ektdesignns_validation=\"choice-req\""
					            sPollHtml += " onblur=\"design_validate_choice(1, -1, this, 'Options are required.')\" onclick=\"design_validate_choice(1, -1, this, 'Options are required.')\"  onkeypress=\"design_validate_choice(1, -1, this, 'Options are required.')\">";
					            for (var i = 1; i <= numPollChoices; i++)
					            {
						            if (aChoice[i].length > 0)
						            {
							            randId = design_makeUnique();
							            sPollHtml += "<li><input id=\"ID" + randId.toString() + "\" title=\"" + eWebEditProUtil.HTMLEncode(aChoice[i]) + "\" type=\"radio\" value=\"" + eWebEditProUtil.HTMLEncode(aValue[i]) + "\" name=\"" + sFieldName + "\" ektdesignns_nodetype=\"item\" /><label contenteditable=\"true\" for=\"ID" + randId.toString() + "\" unselectable=\"off\">" + eWebEditProUtil.HTMLEncode(aChoice[i]) + "</label></li>";
						            }
					            }
					            sPollHtml += "</ol>";
					            if (objInstance)
					            {
					                sPollHtml += "</ektdesignns_choices>";
					            }
					            else
					            {
					                sPollHtml += "</div>";
					            }
				            }
				            sPollHtml += "<input type=\"submit\" value=\"Vote\" />";

				            //replace content in content_html
				            if (objInstance)
				            {
				                var objData = objInstance.linkedElement("content_html");
				                if (objData != null)
				                {
					                objData.value = sPollHtml;
					                objInstance.load();
				                }
				            }
				            else
				            {
				                var objContentDesigner = Ektron.ContentDesigner.instances["content_html"];
				                if (objContentDesigner != null)
				                {
					                objContentDesigner.setContent("designpage", sPollHtml);
				                }
				            }
			            }
			            ShowPane('dvContent');
		            }
		            oProgressSteps.oncancel = function()
		            {
			            //These tests is needed before the page is closed and cancel the action.
			            if (true == bFormEditorReady && true == bResponseEditorReady)
			            {
				            bPageClosing = true;
				            this.close();
				            var bReturn = SetAction('cancel');
				            if ("poll" == sFormType && false == bReturn)
				            {
				                this.done();
				            }
			            }
		            }
		            if ("survey" == sFormType)
		            {
			            // Survey
			            oProgressSteps.define([
                            { id:"",	title:"This step is complete.",	disabled:true }
                            , { id:"",	title:"This step is complete.",	disabled:true }
                            , { id:"response",	title:"<%= (new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("lbl define survey")%>",	description:"<%= (new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("alt msg form data submitted")%>" }
                            , { id:"done",		title:"<%= (new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("lbl setup complete")%>",		description:"<%= (new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("alt msg view n edit form")%>" }
                            ]);
			            g_initialPaneToShow = 'dvSummary';
		            }
		            else if ("poll" == sFormType)
		            {
			            // Poll
			            oProgressSteps.define([
                            { id:"",	title:"This step is complete.",	disabled:true }
                            , { id:"",	title:"This step is complete.",	disabled:true }
                            , { id:"response",	title:"<%= (new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("lbl define poll")%>",	description:"<%= (new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("alt msg ques response")%>" }
                            , { id:"done",		title:"<%= (new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("lbl setup complete")%>",		description:"<%= (new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("alt msg view n edit form")%>" }
                            ]);
			            g_initialPaneToShow = 'dvPollWizard';
		            }
		            else
		            {
			            // stardard 5 steps form
			            oProgressSteps.define([
                            { id:"",	title:"This step is complete.",	disabled:true }
                            , { id:"",	title:"This step is complete.",	disabled:true }
                            , { id:"",	title:"This step is complete.",	disabled:true }
                            , { id:"response",	title:"<%= (new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("lbl response")%>", 	description:"<%= (new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("alt msg form data submitted")%>" }
                            , { id:"done",		title:"<%= (new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("lbl setup complete")%>", description:"<%= (new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("alt msg view n edit form")%>" }
                            ]);
			            g_initialPaneToShow = 'dvSummary';
		            }
	            }
            //--><!]]>
        </script>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
	            function ContentTitleSetFocus()
	            {
		            var objContentTitle = document.getElementById('content_title');
		            if ((null != objContentTitle) && ('undefined' != typeof(objContentTitle)))
		            {
			            try
			            {
				            objContentTitle.focus();
			            }
			            catch (e)
			            {
			            }
		            }
	            }
	            var str_url;
	            str_url=window.location.href;
	            if (str_url.indexOf('type=add')>0)
	            {
		            setTimeout('ContentTitleSetFocus();', 1000); // 1 second is best
	            }

	            function PreviewTemplate(sEditorName,sitepath,content_id,width,height)
                {
                    var objInstance = eWebEditPro.instances[sEditorName];

                    var templar = document.getElementById("templateSelect")
                    if (templar.value != 0) {
	                    window.open(sitepath + templar.options[templar.selectedIndex].text + '?id=' + content_id,'','toolbar,width=' + width + ',height=' + height);
                    } else {
	                    alert('<%=(new Ektron.Cms.ContentAPI()).EkMsgRef.GetMessage("lbl please select a valid template")%>');
                    }
	            }
            //--><!]]>
        </script>
        <% if(DisplayTab){%>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
            var taxonomytreearr="<%=TaxonomyTreeIdList%>".split(",");
            var taxonomytreedisablearr="<%=TaxonomyTreeParentIdList%>".split(",");
            var __EkFolderId="<%=m_intTaxFolderId%>";
            var __TaxonomyOverrideId="<%=TaxonomyOverrideId%>";
            function fetchtaxonomyid(pid){
                for(var i=0;i<taxonomytreearr.length;i++){
                    if(taxonomytreearr[i]==pid){
                        return true;
                        break;
                    }
                }
                return false;
            }
             function fetchdisabletaxonomyid(pid){
                for(var i=0;i<taxonomytreedisablearr.length;i++){
                    if(taxonomytreedisablearr[i]==pid){
                        return true;
                        break;
                    }
                }
                return false;
            }
            function updatetreearr(pid,op){
                if(op=="remove"){
                    for(var i=0;i<taxonomytreearr.length;i++){
                        if(taxonomytreearr[i]==pid){
                            taxonomytreearr.splice(i,1);break;
                        }
                    }
                }
                else{
                    taxonomytreearr.splice(0,0,pid);
                }
                document.getElementById("taxonomyselectedtree").value="";
                for(var i=0;i<taxonomytreearr.length;i++){
                    if(document.getElementById("taxonomyselectedtree").value==""){
                        document.getElementById("taxonomyselectedtree").value=taxonomytreearr[i];
                    }else{
                        document.getElementById("taxonomyselectedtree").value=document.getElementById("taxonomyselectedtree").value+","+taxonomytreearr[i];
                    }
                }
            }
          
            function selecttaxonomy(control) {                
                var pid=control.value;
                if(control.checked)
                {
                    updatetreearr(pid,"add");
                }
                else
                {
                    updatetreearr(pid,"remove");
                }
                var currval=eval(document.getElementById("chkTree_T"+pid).value);
                var node = document.getElementById( "T" + pid );
                var newvalue=!currval;
                document.getElementById("chkTree_T"+pid).value=eval(newvalue);
                if(control.checked)
                  {
                    Traverse(node,true);
                  }
                else
                  {
                    Traverse(node,false);
                    var hasSibling = false;
                    if (taxonomytreearr != "")
                      { for(var i = 0 ;i<taxonomytreearr.length;i++)
                            {
                              if(taxonomytreearr[i] != "")
                                {
                                  var newnode = document.getElementById( "T" + taxonomytreearr[i]);
                                    if(newnode != null && newnode.parentNode == node.parentNode)
                                       {Traverse(node,true);hasSibling=true;break;}
                                }
                            }
                      }
                    if(hasSibling == false)
                    {
                     checkParent(node);
                    }
                  }
            }

            function checkParent(node)
            { if(node!= null)
                {
                      var subnode = node.parentNode;
                      if(subnode!=null && subnode.id!="T0" &&  subnode.id!="")
                      {
                                for(var j=0;j<subnode.childNodes.length;j++)
                                  {var pid=subnode.childNodes[j].id;
                                   if(document.getElementById("chkTree_"+pid).value == true || document.getElementById("chkTree_"+pid).value == "true")
                                      {Traverse(subnode.childNodes[j],true);return;}
                                  }
                       checkParent(subnode.parentNode);
                      }
                }
            }

            function Traverse(node,newvalue){
                if(node!=null){
                    subnode=node.parentNode;
                     if(subnode!=null && subnode.id!="T0" &&  subnode.id!="")
                     {
                        for(var j=0;j<subnode.childNodes.length;j++)
                          {
                                var n=subnode.childNodes[j]
                                if(n.nodeName=="INPUT" && n.attributes["type"].value=="checkbox")
                                {
                                    var pid=subnode.id;
                                    updatetreearr(pid.replace("T",""),"remove");
                                    document.getElementById("chkTree_"+pid).value=eval(newvalue);
                                    if (navigator.userAgent.indexOf("Firefox") > -1 ||
                                        navigator.userAgent.indexOf("MSIE 8.0") > -1) {
                                          n.checked = eval(newvalue);
                                          n.disabled = eval(newvalue);
                                     }
                                    else{
                                         n.setAttribute("checked",eval(newvalue));
                                         n.setAttribute("disabled",eval(newvalue));
                                    }
                                }
                          }
                        if(HasChildren(subnode) && subnode.getAttribute("checked")){
                               subnode.setAttribute("checked",true);
                                subnode.setAttribute("disabled",true);
                     }
                        Traverse(subnode,newvalue);
                    }
                }
            }
            function HasChildren(subnode)
            {
                if(subnode!=null){
                    for(var j=0;j<subnode.childNodes.length;j++)
                    {
                        for(var j=0;j<subnode.childNodes.length;j++){
                            var n=subnode.childNodes[j]
                            if(n.nodeName=="INPUT" && n.attributes["type"].value=="checkbox"){
                                var pid=subnode.id;
                                var v=document.getElementById("chkTree_"+pid).value;
                                if(v==true || v=="true"){
                                return true;break;
                                }
                            }
                        }
                    }
                }
                return false;
            }
           
            //--><!]]>
        </script>
         <!------------------------>
               <%if (TaxonomyRoleExists == true)
      {%>
    <script language="javascript" type="text/javascript">
        var taxonomytreemenu = true;
        var g_delayedHideTimer = null;
        var g_delayedHideTime = 1000;
        var g_wamm_float_menu_treeid = -1;
        var g_isIeInit = false;
        var g_isIeFlag = false;

        function IsBrowserIE()
        {
            if (!g_isIeInit)
            {
                var ua = window.navigator.userAgent.toLowerCase();
                g_isIeFlag = (ua.indexOf('msie') > -1) && (!(ua.indexOf('opera') > -1));
                g_isIeInit = true;
            }
            return (g_isIeFlag);
        }

        function markMenuObject(markFlag, id)
        {
            if (id && (id > 0))
            {
                var obj = document.getElementById(id);
                if (obj && obj.className)
                {
                    if (markFlag)
                    {
                        if (obj.className.indexOf("linkStyle_selected") < 0)
                        {
                            obj.className += " linkStyle_selected";
                        }
                    }
                    else
                    {
                        if (obj.className.indexOf("linkStyle_selected") >= 0)
                        {
                            obj.className = "linkStyle";
                        }
                    }
                }
            }
        }

        function showWammFloatMenuForMenuNode(show, delay, event, treeId)
        {
            var el = document.getElementById("wamm_float_menu_block_menunode");
            var visible = "";
            if (el)
            {
                if (g_delayedHideTimer)
                {
                    clearTimeout(g_delayedHideTimer);
                    g_delayedHideTimer = null;
                }
                var tree = null;
                if (treeId > 0)
                {
                    tree = TreeUtil.getTreeById(treeId);
                }
                if (tree && tree.node && tree.node.data)
                {
                    visible = tree.node.data.visible;
                }
                if (show)
                {
                    el.style.display = "none";
                    if (visible != "false")
                        markMenuObject(false, g_wamm_float_menu_treeid);
                    if (null != event)
                    {
                        var hoverElement = $ektron("#" + treeId);
                        var offset = hoverElement.offset(hoverElement[0].selector);
                        var hoverElementHeight = parseInt(hoverElement.height(), 10);
                        var hoverElementWidth = parseInt(hoverElement.width(), 10)

                        var fixedPositionToolbarFix = 0;
                        //if ($ektron("form#LibraryItem").length > 0)
                        //{
                            fixedPositionToolbarFix = 44;
                        //}

                        el.style.top = (parseInt(offset.top, 10) + hoverElementHeight - 120 - fixedPositionToolbarFix) + "px";
                        el.style.left = (parseInt(offset.left, 10) + hoverElementWidth - 8) + "px";

                        el.style.display = "";
                        if (treeId && (treeId > 0))
                        {
                            g_wamm_float_menu_treeid = treeId;
                            if (visible != "false")
                                markMenuObject(true, treeId);
                        }
                        else
                        {
                            g_wamm_float_menu_treeid = -1;
                        }
                    }
                }
                else
                {
                    if (delay)
                    {
                        g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuNode(false, false, null, -1)", g_delayedHideTime);
                    }
                    else
                    {
                        el.style.display = "none";
                        if (visible != "false")
                            markMenuObject(false, g_wamm_float_menu_treeid);
                    }
                }
            }
        }

        function getEventX(event)
        {
            var xVal;
            if (IsBrowserIE())
            {
                xVal = event.x;
            }
            else
            {
                xVal = event.pageX;
            }
            return (xVal)
        }

        function getShiftedEventX(event)
        {
            var srcLeft;
            var xVal;
            if (IsBrowserIE())
            {
                xVal = event.x;
            }
            else
            {
                xVal = event.pageX;
            }

            // attempt to shift div-tag to the right of the menu items:
            srcLeft = xVal;
            if (event.srcElement && event.srcElement.offsetLeft)
            {
                srcLeft = event.srcElement.offsetLeft;
            }
            else if (event.target && event.target.offsetLeft)
            {
                srcLeft = event.target.offsetLeft;
            }

            if (event.srcElement)
            {
                if (event.srcElement.offsetWidth)
                {
                    xVal = srcLeft + event.srcElement.offsetWidth;
                }
                else if (event.srcElement.scrollWidth)
                {
                    xVal = srcLeft + event.srcElement.scrollWidth;
                }
            }
            else if (event.target && event.target.offsetLeft)
            {
                if (event.target.offsetWidth)
                {
                    xVal = srcLeft + event.target.offsetWidth;
                }
                else if (event.target.scrollWidth)
                {
                    xVal = srcLeft + event.target.scrollWidth;
                }
            }

            return (xVal)
        }


        function getEventY(event)
        {
            var yVal;
            if (IsBrowserIE())
            {
                yVal = event.y;
            }
            else
            {
                yVal = event.pageY;
            }
            return (yVal)
        }

        function wamm_float_menu_block_mouseover(obj)
        {
            if (g_delayedHideTimer)
            {
                clearTimeout(g_delayedHideTimer);
                g_delayedHideTimer = null;
            }
        }

        function wamm_float_menu_block_mouseout(obj)
        {
            if (null != obj)
            {
                g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuNode(false, false, null, -1)", g_delayedHideTime);
            }
        }

        function routeAction(containerFlag, op)
        {
            var tree = null;
            if (g_wamm_float_menu_treeid > 0)
            {
                tree = TreeUtil.getTreeById(g_wamm_float_menu_treeid);
            }

            if (tree && tree.node && tree.node.data)
            {
                var TaxonomyId = tree.node.data.id;
                var ParentId = tree.node.pid;
                if (ParentId == null || ParentId == 'undefined')
                {
                    ParentId = 0;
                }

                showWammFloatMenuForMenuNode(false, false, null, -1);
                LoadChildPage(op, TaxonomyId, ParentId);
            }
        }
        function LoadChildPage(Action, TaxonomyId, ParentId)
        {
            var frameObj = document.getElementById("ChildPage");
            var lastClickedOn = document.getElementById("LastClickedOn");
            lastClickedOn.value = TaxonomyId;
            document.getElementById("LastClickedParent").value = ParentId;
            if (parseInt(ParentId) == 0) { document.getElementById("ClickRootCategory").value = "true"; }
            else { document.getElementById("ClickRootCategory").value = "false"; }
            switch (Action)
            {
                case "add":
                    if (TaxonomyId == "")
                    {
                        alert("Please select a taxonomy.");
                        return false;
                    }
                    frameObj.src = "taxonomy.aspx?iframe=true&action=add&parentid=" + TaxonomyId;
                    break;
                default:
                    break;
            }
            if (Action != "delete")
            {
                DisplayIframe();
            }
        }
        function DisplayIframe()
        {
            var pageObj = document.getElementById("FrameContainer");
            pageObj.style.display = "";
            if (navigator.userAgent.indexOf("MSIE 6.0") > -1)
            {
                pageObj.style.width = "100%";
                pageObj.style.height = "500px";
            }
            else
            {
                pageObj.style.width = "95%";
                pageObj.style.height = "95%";
            }
        }
        function CancelIframe()
        {
            var pageObj = document.getElementById("FrameContainer");
            pageObj.style.display = "none";
            pageObj.style.width = "1px";
            pageObj.style.height = "1px";
        }
        function CloseChildPage()
        {
            CancelIframe();
            var ClickRootCategory = document.getElementById("ClickRootCategory");
            var lastClickedOn = document.getElementById("LastClickedOn");
            var clickType = document.getElementById("ClickType");
            if (ClickRootCategory.value == "true")
                __EkFolderId = "<%=m_intTaxFolderId%>";
            else
            {
                __EkFolderId = -1;
                TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
            }
            var node = document.getElementById("T" + lastClickedOn.value);
            if (node != null)
            {
                for (var i = 0; i < node.childNodes.length; i++)
                {
                    if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) < 9)
                    {
                        if (node.childNodes(i).nodeName == 'LI' || node.childNodes(i).nodeName == 'UL')
                        {
                            var parent = node.childNodes(i).parentElement;
                            parent.removeChild(node.childNodes(i));
                        }
                    }
                    else
                    {
                        if (node.childNodes[i].nodeName == 'LI' || node.childNodes[i].nodeName == 'UL')
                        {
                            var parent = node.childNodes[i].parentNode;
                            parent.removeChild(node.childNodes[i]);
                        }
                    }
                }
                TREES["T" + lastClickedOn.value].children = [];
                TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
                onToggleClick(lastClickedOn.value, TreeUtil.addChildren, lastClickedOn.value);
            }
        }
    </script>
	<% if(Page.Request.Url.AbsoluteUri.IndexOf("membership_add_content.aspx")== -1 && Page.Request.Url.ToString().IndexOf("forum=1") == -1){ %>
    <div id="Div1" class="Menu" style="position:absolute; left:10px; top:10px;
        display:none; z-index:3200;" onmouseover="wamm_float_menu_block_mouseover(this)"
        onmouseout="wamm_float_menu_block_mouseout(this)">
        <input type="hidden" name="LastClickedParent" id="Hidden1" value="" />
        <input type="hidden" name="ClickRootCategory" id="Hidden2" value="false" />
        <ul>
            <li class="MenuItem add">
                <a href="#" onclick="routeAction(true, 'add');"><%=(m_refMsg.GetMessage("generic add title"))%></a>
            </li>
        </ul>
    </div>
    <% } %>
     <% } %>
    <%else{%>
    <script type="text/javascript" >
        var taxonomytreemenu = false;
    </script>
    <% } %>   
<script type="text/javascript">
    var taxonomytreemode = "editor"; var ____ek_appPath2 = "";
</script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.url.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.explorer.init.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.explorer.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.explorer.config.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.explorer.windows.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.cms.types.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.cms.parser.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.cms.toolkit.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.cms.api.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.ui.contextmenu.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.ui.iconlist.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.ui.tabs.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.ui.explore.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.ui.taxonomytree.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.net.http.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.lang.exception.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.form.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.log.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.dom.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.debug.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.string.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.cookie.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.querystring.js"></script>
<script type="text/javascript">
    var clickedElementPrevious = null;
    var clickedIdPrevious = null;

    function onDragEnterHandler(id, element)
    {
        folderID = id;
        if (clickedElementPrevious != null)
        {
            clickedElementPrevious.style["background"] = "#ffffff";
            clickedElementPrevious.style["color"] = "#000000";
        }
        element.style["background"] = "#3366CC";
        element.style["color"] = "#ffffff";
    }

    function onMouseOverHandler(id, element)
    {
        element.style["background"] = "#ffffff";
        element.style["color"] = "#000000";
    }

    function onDragLeaveHandler(id, element)
    {
        element.style["background"] = "#ffffff";
        element.style["color"] = "#000000";
    }

    function onFolderClick(id, clickedElement)
    {
        var tree = null;
        var visible = "";
        if (id > 0)
        {
            tree = TreeUtil.getTreeById(id);
        }
        if (tree)
        {
            if (tree.node)
            {
                if (tree.node.data)
                {
                    visible = tree.node.data.visible;
                }
            }
        }
        if (clickedElementPrevious != null)
        {
            var previousTree = null;
            var previousVisible = "";
            if (clickedElementPrevious.id > 0)
                previousTree = TreeUtil.getTreeById(clickedElementPrevious.id);
            if (previousTree)
            {
                if (previousTree.node)
                {
                    if (previousTree.node.data)
                    {
                        previousVisible = previousTree.node.data.visible;
                    }
                }
            }
            if (previousVisible != "false")
            {
                clickedElementPrevious.style["background"] = "#ffffff";
                clickedElementPrevious.style["color"] = "#000000";
            }
            else
            {
                clickedElementPrevious.style["background"] = "#808080";
                clickedElementPrevious.style["color"] = "#000000";
            }
        }
        if (visible != "false")
        {
            clickedElement.style["background"] = "#3366CC";
            clickedElement.style["color"] = "#ffffff";
        }
        else
        {
            clickedElement.style["background"] = "#808080";
            clickedElement.style["color"] = "#ffffff";
        }
        clickedElementPrevious = clickedElement;
        clickedIdPrevious = id;

        var name = clickedElement.innerText;
        var folder = new Asset();
        folder.set("name", name);
        folder.set("id", id);
        folder.set("folderid", __EkFolderId);
        __EkFolderId = -1;
    }

    function onToggleClick(id, callback, args)
    {
        toolkit.getAllSubCategory(id, -99, callback, args);
    }

    function makeElementEditable(element)
    {
        element.contentEditable = true;
        element.focus();
        element.style.background = "#fff";
        element.style.color = "#000";
    }

    var baseUrl = URLUtil.getAppRoot(document.location) + "images/ui/icons/tree/";
    TreeDisplayUtil.plusclosefolder = baseUrl + "taxonomyCollapsed.png";
    TreeDisplayUtil.plusopenfolder = baseUrl + "taxonomyCollapsed.png";
    TreeDisplayUtil.minusclosefolder = baseUrl + "taxonomyExpanded.png";
    TreeDisplayUtil.minusopenfolder = baseUrl + "taxonomyExpanded.png";
    TreeDisplayUtil.folder = baseUrl + "taxonomy.png";

    var g_menu_id = "";
    function displayCategory(categoryRoot)
    {
        document.body.style.cursor = "default";
        var taxonomyTitle = null;
        try
        {
            taxonomyTitle = categoryRoot.title;
            g_menu_id = categoryRoot.id;
        } catch (e)
        {
            ;
        }

        if (taxonomyTitle != null)
        {
            treeRoot = new Tree(taxonomyTitle, __TaxonomyOverrideId, null, categoryRoot, 0);
            TreeDisplayUtil.showSelf(treeRoot, document.getElementById("TreeOutput"));
            TreeDisplayUtil.toggleTree(treeRoot.node.id);
        } else
        {
            var element = document.getElementById("TreeOutput");
            var debugInfo = "<b>Cannot connect to the service</b>";
            element.innerHTML = debugInfo;
        }
    }

    var toolkit = new EktronToolkit();
    toolkit.getTaxonomy(__TaxonomyOverrideId, -99, displayCategory, __TaxonomyOverrideId);

    function reloadTreeRoot(id)
    {
        TREES = {};
        toolkit.getTaxonomy(id, -99, displayCategory, __TaxonomyOverrideId);
    }

    var g_selectedFolderList = "0";
    var g_timerForFolderTreeDisplay;
    function showSelectedFolderTree()
    {
        if (g_timerForFolderTreeDisplay)
        {
            window.clearTimeout(g_timerForFolderTreeDisplay);
        }
        g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
    }

    function showSelectedFolderTree_delayed()
    {
        var bSuccessFlag = false;
        if (g_timerForFolderTreeDisplay)
        {
            window.clearTimeout(g_timerForFolderTreeDisplay);
        }

        if (g_selectedFolderList.length > 0)
        {
            var tree = TreeUtil.getTreeById(g_menu_id);
            if (tree)
            {
                var lastId = 0;
                var folderList = g_selectedFolderList.split(",");
                bSuccessFlag = TreeDisplayUtil.expandTreeSet(folderList);
            }

            if (!bSuccessFlag)
            {
                g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
            }
        }
    }
</script>
 
         <!------------------------>
        <%}%>
       <%-- <script type="text/javascript">
            <!--//--><![CDATA[//><!--
                var isOffice = document.getElementById("isOfficeDoc");

                if ((isOffice != null) && (isOffice.value == "true") && (ShowMultipleUpload() && IsBrowserIE()))
                {
                    g_initialPaneToShow = 'dvSummary';
                    var contentTabHeader = document.getElementById("dvContent");
                    var contentTabContent = document.getElementById("_dvContent");
                    if (contentTabHeader != null)
                       contentTabHeader.style.display="none";
                    if (contentTabContent != null)
                       contentTabContent.style.display="none";
                }
            //--><!]]>
        </script> --%>
        <script type="text/javascript">
            <!--            //--><![CDATA[//><!--
            var WebEventContTab = '<asp:literal id="WebEventCont" runat="server"/>';
             
            if ((WebEventContTab != null) && (WebEventContTab == "true"))
             {            
                g_initialPaneToShow = 'dvSummary';
                var contentTabHeader = document.getElementById("dvContent");
                var contentTabContent = document.getElementById("_dvContent");
                if (contentTabHeader != null)
                    contentTabHeader.style.display = "none";
                if (contentTabContent != null)
                    contentTabContent.style.display = "none";
            }
            //--><!]]>
        </script>
        <script type="text/javascript">            <!--
                function ShowAddPersonalTagArea(){
                    $ektron("#newTagName")[0].value = "";
                    $ektron("#newTagNameDiv").modalShow();
                }

                this.customPTagCnt = 0;
                function SaveNewPersonalTag(){
	                // add new tag:
	                var objTagName = document.getElementById("newTagName");
	                var objTagLanguage = document.getElementById("TagLanguage");
	                var objLanguageFlag = document.getElementById("flag_" + objTagLanguage.value);
	                var divObj = document.getElementById("newAddedTagNamesDiv");

	                if(!CheckForillegalChar(objTagName.value)){
	                    return;
	                }

	                if (objTagName && (objTagName.value.length > 0) && divObj){
		                // save previous checkbox values to work around FF3 bug where it doesn't read out the checkbox state when appending to a DIV's innerHTML
		                var oldcbstate = new Array();
		                for (var i = 0; i < divObj.childNodes.length; i++)
		                {
		                    if (divObj.childNodes[i].tagName && (divObj.childNodes[i].tagName.toString().toLowerCase() == "input")) {
		    	                oldcbstate[divObj.childNodes[i].id] = divObj.childNodes[i].checked;
		                    }
		                }

		                ++this.customPTagCnt;
		                divObj.innerHTML += "<input type='checkbox' checked='checked' onclick='ToggleCustomPTagsCbx(this, \"" + objTagName.value + "\");' id='userCustomPTagsCbx_" + this.customPTagCnt + "' name='userCustomPTagsCbx_" + this.customPTagCnt + "' />&#160;"

		                if(objLanguageFlag != null){
		                    divObj.innerHTML += "<img src='" + objLanguageFlag.value + "' border=\"0\" />"
		                }

		                divObj.innerHTML +="&#160;" + objTagName.value + "<br />"

		                // restore checkbox state to work around FF3 bug
		                for (i = 0; i < divObj.childNodes.length; i++)
		                {
		                    if (divObj.childNodes[i].tagName && (divObj.childNodes[i].tagName.toString().toLowerCase() == "input") &&
		    	                (oldcbstate[divObj.childNodes[i].id] != undefined)) {
		    	                divObj.childNodes[i].checked = oldcbstate[divObj.childNodes[i].id];
		                    }
		                }

		                AddHdnTagNames(objTagName.value + '~' + objTagLanguage.value);
	                }

	                // now close window:
	                CancelSaveNewPersonalTag();
                }

                function CancelSaveNewPersonalTag(){
                    $ektron("#newTagNameDiv").modalHide();
                }

                function AddHdnTagNames(newTagName){
	                objHdn = document.getElementById("newTagNameHdn");
	                if (objHdn){
		                var vals = objHdn.value.split(";");
		                var matchFound = false;
		                for (var idx = 0; idx < vals.length; idx++){
			                if (vals[idx] == newTagName){
				                matchFound = true;
				                break;
			                }
		                }
		                if (!matchFound){
			                if (objHdn.value.length > 0){
				                objHdn.value += ";";
			                }
			                objHdn.value += newTagName;
		                }
	                }
                }

                function RemoveHdnTagNames(oldTagName){
	                objHdn = document.getElementById("newTagNameHdn");
	                if (objHdn && (objHdn.value.length > 0)){
		                var vals = objHdn.value.split(";");
		                objHdn.value = "";
		                for (var idx = 0; idx < vals.length; idx++){
			                if (vals[idx] != oldTagName){
				                if (objHdn.value.length > 0){
					                objHdn.value += ";";
				                }
				                objHdn.value += vals[idx];
			                }
		                }
	                }
                }

                function ToggleCustomPTagsCbx(btnObj, tagName){
	                if (btnObj.checked){
		                AddHdnTagNames(tagName);
		                btnObj.checked = true;
	                }
	                else{
		                RemoveHdnTagNames(tagName);
		                btnObj.checked = false; // otherwise re-checks when adding new custom tag.
	                }
                }

                function CheckForillegalChar(tag) {
                   if (Trim(tag) == '')
                   {
                       alert('<asp:Literal ID="error_TagsCantBeBlank" Text="Please enter a name for the Tag." runat="server"/>');
                       return false;
                   } else {

                        //alphanumeric plus _ -
                        var tagRegEx = /[!"#$%&'()*+,.\/:;<=>?@[\\\]^`{|}~ ]+/;
                        if(tagRegEx.test(tag)==true) {
                          alert('<asp:Literal ID="error_InvalidChars" Text="Tag Text can only include alphanumeric characters." runat="server"/>');
                        return false;
                        }

                   }
                   return true;
                }
            //-->
        </script>

        <input type="hidden" id="buttonId" runat="server" />
    </body>
</html>

