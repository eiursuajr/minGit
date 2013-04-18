<%@ Control Language="C#" AutoEventWireup="true" Inherits="viewcontent" CodeFile="viewcontent.ascx.cs" %>
<%@ Register TagPrefix="ucEktron" TagName="Items" Src="../../Commerce/CatalogEntry/Items/Items.ascx" %>
<%@ Register TagPrefix="ucEktron" TagName="Media" Src="../../Commerce/CatalogEntry/Media/Media.ascx" %>

<script type="text/javascript" language="javascript" >
     Ektron.ready(function(){
     $("#"+"<%=divContentHtml.ClientID%>"+" a").click(function() {
                      var chkalert = "<%=showAlert%>";
                     if (chkalert == "True"){
                        alert('<%=m_refMsg.GetMessage("js err links disabled")%>');
                     }
                     return false;
                });
            });
</script>
<script type="text/javascript">
    <!--//--><![CDATA[//><!--
    $ektron.addLoadEvent(function(){
        if (document.getElementById('content_DefaultTab').value != 'dvProperties'){  
            var tabValue = getQueryStringValue("tab");
            if(tabValue == 'properties')
                $ektron("a[href='#dvProperties']").click();
            else
                $ektron("a[href='#dvContent']").click();
        }
    });
	function LoadContent(FormName,opt){
		var checkedFolderID = "<asp:literal id="jsFolderId" runat="server"/>";
		var jsToolId ="<asp:literal id="jsToolId" runat="server"/>";
		var jsToolAction="<asp:literal id="jsToolAction" runat="server"/>";
		var jsBackStr="<asp:literal id="jsBackStr" runat="server"/>";
		var jsIsForm = "<asp:Literal id="jsIsForm" runat="server" />";
		var xml_id = '<%=xml_id%>';
		var allowHtml = "<%=allowHtml %>";
		var jsLangID = "<asp:Literal id="jsLangId" runat="server"></asp:Literal>";

		if(opt=='VIEW'){
			var num=document.forms[0].viewcontent.selectedIndex;
			var lang=document.forms[0].viewcontent.options[num].value;
			top.notifyLanguageSwitch(lang, checkedFolderID);
			document.forms[0].action="content.aspx?folder_id="+checkedFolderID+"&id="+jsToolId+"&action="+jsToolAction+"&LangType="+lang;
			document.forms[0].submit();
		}else{
			var num=document.forms[0].addcontent.selectedIndex;
			var lang=document.forms[0].addcontent.options[num].value;
			if(lang!=0)
			{
			    top.notifyLanguageSwitch(lang, checkedFolderID);
			    if (jsIsForm == '2')
			    {
		            document.forms[0].__VIEWSTATE.name = 'NOVIEWSTATE';
        			document.forms[0].action="cmsform.aspx?back_LangType="+jsContentLanguage+"&form_id="+jsToolId+"&LangType="+lang+"&action=Addform&folder_id="+checkedFolderID+"&callbackpage=cmsform.aspx&parm1=action&value1=ViewAllFormsByFolderID&parm2=folder_id&value2="+checkedFolderID;
			        document.forms[0].submit();
			    }
			    else if (jsIsForm == '3333')
			    {
			        top.document.getElementById('ek_main').src = 'commerce/CatalogEntry.aspx?close=false&ContType=3333&LangType=' + lang + '&content_id=' + jsToolId + '&type=addlang&createtask=1&id=' + checkedFolderID + '&xid=' + xml_id + '&back_file=content.aspx&back_action=ViewContentByCategory&back_id=' + checkedFolderID + '&back_LangType=' + jsContentLanguage;
			    }
			    else
			    {
			        top.document.getElementById('ek_main').src = 'edit.aspx?close=false&LangType=' + lang + '&content_id=' + jsToolId + '&folderid=' + checkedFolderID+ '&con_lang_id=' + jsLangID + '&type=add&createtask=1&id=' + checkedFolderID + '&' + jsBackStr + allowHtml + '&translate=true';
			        if( xml_id > 0 ) {
			            top.document.getElementById('ek_main').src += "&xid=" + xml_id
			        }
			    }
			}

		return false;
		}
	}
	// Adjusts the navigation-tree frame (if function exists; ie workarea).
	// (True Shows the nav-tree, False hides it)
	function ResizeFrame(val) {
		if ((typeof(top.ResizeFrame) == "function") && top != self) {
			top.ResizeFrame(val);
		}
	}
	function IsBrowserIE_Email()
	{

	    return (document.all ? true : false);

    }
	function LoadChildPage(pageURL)
	{
		if (IsBrowserIE_Email())
		{
			var frameObj = document.getElementById("ChildPage");
			frameObj.src = "blankredirect.aspx?" + pageURL;

			var pageObj = document.getElementById("FrameContainer");
			pageObj.style.display = "";
			pageObj.style.width = "80%";
			pageObj.style.height = "80%";
		}
		else
		{
			PopUpWindow(pageURL, 490,500,1,1);
		}
	}

	function SetAction(sAction,sAppPath,pubAsHtml){
		// determine the re-direct path based on the action requested.
		var sHtmlAction = "";
		if ("workoffline" == sAction)
		{
			sHtmlAction = "content.aspx?LangType="+jsContentLanguage+"&action=WorkOffline&id=<%=m_intId%>&folder_id=<%=m_intFolderId%>";
		}
		else if ("checkin" == sAction)
		{
			sHtmlAction = "content.aspx?LangType="+jsContentLanguage+"&action=CheckIn&id=<%=m_intId%>&fldid=<%=m_intFolderId%>&page=workarea";
		}
		else if ("savelocalcopy" != sAction)
		{
			// something not expected
			return true;
		}

		// for the events coming from OnAssetUserRequestEvent and the icon clicks.
		if ("object" == typeof g_AssetHandler)
		{
			// DMS Function calls need to wait for DMS return events
			// before CMS can change the status of the document at SetAction.
			var sContentTitle = "<asp:literal id="lblContentTitle" runat=server/>";
			var basehosturl = location.protocol + '//' + location.host;
			g_AssetHandler.SetPostInfo(basehosturl + sAppPath + 'ProcessUpload.aspx', jsContentLanguage,
				 <%=m_intFolderId%>, pubAsHtml,'update',<%=m_intId%>,1);

			if (!g_AssetHandler.SetAction(sAction, sContentTitle)){
				// if SetAction is not success.  return and do not redirect.
				return false;
			}
			// after DMS returns events, it is redirected to contentaction page
			// to complete the CMS side of administration. acts like in edit.asp after the post.
			// extra variables need in the contentaction.asp
			if ("checkin" == sAction)
			{
			    sHtmlAction = "content.aspx?LangType="+jsContentLanguage+"&action=View&id=<%=m_intId%>&fldid=<%=m_intFolderId%>&page=workarea";
			}
			sHtmlAction += "&asset_assetfilename=" + escape( document.forms[g_AssetHandler.formName].elements["asset_assetfilename"].value);
		}
		// re-directed to contentaction page
		DisplayHoldMsg(true);
		document.location.href = sHtmlAction;
	}

	function CheckTitle()
	{
	    var bret = true;
		var objForm =  document.getElementById("<%=content_title.ClientID %>");
		if (objForm != null && objForm.value != "")
		{
		    if((objForm.value.indexOf('\\') > -1) || (objForm.value.indexOf('*') > -1) || (objForm.value.indexOf('>') > -1)||(objForm.value.indexOf('<') > -1)||(objForm.value.indexOf('|') > -1)||(objForm.value.indexOf('\"') > -1) || (objForm.value.indexOf('/') > -1 ) )
            {
                alert("The title cannot contain '\\', '/', '*','>','<','|','\"'.");
                bret = false;
            }

	    }
	    return bret;
    }
    //--><!]]>
</script>

<asp:literal id="EnhancedMetadataScript" runat="server" />
<asp:Literal ID="EnhancedMetadataArea" runat="server" />

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div style="border-right: 1px; border-top: 1px; display:none; border-left: 1px;
    width: 100%; border-bottom: 1px; position: absolute; top: 48px; height: 1px;
    background-color: white" id="dvHoldMessage">
    <table border="1" width="100%">
        <tr>
            <td valign="top" align="center">
                <h3 style="color: red"><asp:Literal ID="HoldMomentMsg" runat="server" /></h3>
            </td>
        </tr>
    </table>
</div>

<input type="hidden" name="DefaultTab" value="" id="DefaultTab" runat="server" />

<div class="ektronPageContainer ektronPageTabbed">
    <div class="tabContainerWrapper">
        <div class="tabContainer">
            <ul>
                <li>
                    <a title="<%=m_refMsg.GetMessage("properties text")%>" href="#dvProperties">
                        <%=m_refMsg.GetMessage("properties text")%>
                    </a>
                </li>
                <li>
                    <a title="<%=m_refMsg.GetMessage("content text")%>" href="#dvContent">
                        <%=m_refMsg.GetMessage("content text")%>
                    </a>
                </li>
                <li>
                    <a title="<%= m_refMsg.GetMessage("summary text")%>" href="#dvSummary">
                        <%=m_refMsg.GetMessage("Summary text")%>
                    </a>
                </li>
                <asp:PlaceHolder ID="phCommerce" runat="server">
                    <li>
                        <a title="<%=m_refMsg.GetMessage("lbl pricing")%>" href="#dvPricing">
                            <%=m_refMsg.GetMessage("lbl pricing")%>
                        </a>
                    </li>
                    <asp:PlaceHolder ID="phAttributes" runat="server">
                        <li>
                            <a title="<%=m_refMsg.GetMessage("lbl attributes")%>" href="#dvAttributes">
                                <%=m_refMsg.GetMessage("lbl attributes")%>
                            </a>
                        </li>
                    </asp:PlaceHolder>
                    <li>
                        <a title="<%=m_refMsg.GetMessage("lbl media")%>" href="#dvMedia">
                            <%=m_refMsg.GetMessage("lbl media")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phItems" runat="server">
                    <li>
                        <a title="<%=m_refMsg.GetMessage("generic items")%>" href="#dvItems">
                            <%=m_refMsg.GetMessage("generic items")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <li>
                    <a title="<%=m_refMsg.GetMessage("metadata text")%>" href="#dvMetadata">
                        <%=m_refMsg.GetMessage("metadata text")%>
                    </a>
                </li>
                <asp:PlaceHolder ID="phAliases" runat="server">
                    <li>
                        <a title="<%=m_refMsg.GetMessage("lbl alias")%>" href="#dvAliases">
                            <%=m_refMsg.GetMessage("lbl alias")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <li>
                    <a title="<%=m_refMsg.GetMessage("comment text")%>" href="#dvComment">
                        <%=m_refMsg.GetMessage("comment text")%>
                    </a>
                </li>
                <li>
                    <a title="<%=m_refMsg.GetMessage("tasks text")%>" href="#dvTasks">
                        <%=m_refMsg.GetMessage("tasks text")%>
                    </a>
                </li>
                <asp:PlaceHolder ID="phCategories" runat="server">
                    <li>
                        <a title="<%=m_refMsg.GetMessage("viewtaxonomytabtitle")%>" href="#dvCategories">
                            <%=m_refMsg.GetMessage("viewtaxonomytabtitle")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phWebAlerts" runat="server">
                    <li>
                        <a title="<%=m_refMsg.GetMessage("lbl web alert tab")%>" href="#dvWebAlerts">
                            <%=m_refMsg.GetMessage("lbl web alert tab")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phLocalization" Visible="false" runat="server">
                    <li>
                        <a title="<%= m_refMsg.GetMessage("view localization tab title")%>" href="#dvLocalization">
                            <%= m_refMsg.GetMessage("view localization tab title")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
            </ul>
            <div id="dvProperties">
                <div class="ektronPageInfo">
                    <table class="ektronGrid">
                        <tbody>
                            <asp:Literal ID="litPropertyRows" runat="server" />
                        </tbody>
                    </table>
                    <asp:DataGrid ID="PropertiesGrid" CssClass="ektronGrid" runat="server" AutoGenerateColumns="false"></asp:DataGrid>
                </div>
            </div>
            <div id="dvContent">
                <!-- TODO: Ross - Update class -->
                <div class="info-header" id="tdcontlbl" runat="server"></div>
                <div id="divContentHtml" runat="server"></div>
                <asp:Literal runat="server" ID="litBlogComment" Visible="false" />
            </div>
            <div id="dvSummary">
                <div id="tdsummarytext" runat="server"></div>
            </div>
            <asp:PlaceHolder ID="phCategories2" runat="server">
                <div id="dvCategories">
                    <%=TaxonomyList%>
                </div>
            </asp:PlaceHolder>
            <div id="dvTasks">
                <asp:Literal ID="TaskTypeJS" runat="server" />
                <table class="ektronGrid">
                    <tr>
                        <td class="label" title="<%=m_refMsg.GetMessage("lbl show task type")%>"><%=m_refMsg.GetMessage("lbl show task type")%></td>
                        <td class="value">
                            <select name="show_task_type" id="show_task_type" onchange="javascript:RefreshTasksWithTaskType();">
                            </select>
                        </td>
                    </tr>
                </table>
                <div class="ektronTopSpaceSmall"></div>
                <div class="ektronBorder">
                    <div class="ektronPageInfo">
                        <asp:DataGrid ID="TaskDataGrid"
                            runat="server"
                            CssClass="ektronGrid"
                            AutoGenerateColumns="False"
                            OnItemDataBound="TaskDataGrid_ItemDataBound">
                            <HeaderStyle CssClass="title-header" />
                        </asp:DataGrid>
                    </div>
                </div>
                <script type="text/javascript">
FillInShowTaskType();</script>
            </div>
            <div id="dvMetadata">
                <asp:Literal ID="MetaDataValue" runat="server" />
            </div>
            <asp:PlaceHolder ID="phAliases2" runat="server">
                <div id="dvAliases">
                    <div class="ektronHeader" title="<%=m_refMsg.GetMessage("lbl tree url manual aliasing")%>"><%=m_refMsg.GetMessage("lbl tree url manual aliasing")%></div>
                    <table class="ektronGrid">
                        <tr>
                            <td class="label"><%=m_refMsg.GetMessage("lbl primary") + " " + m_refMsg.GetMessage("lbl alias name")%>:</td>
                            <td class="readOnlyValue" id="tdAliasPageName" runat="server"></td>
                        </tr>
                    </table>
                    <div class="ektronTopSpace"></div>
                    <div class="autoAlias" style="width: auto; height: auto; overflow: auto;" id="autoAliasList" runat="server" ></div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phCommerce2" runat="server">
                <div id="dvPricing">
                    <asp:Literal ID="ltr_pricing" runat="server" />
                </div>
                <asp:PlaceHolder ID="phAttributes2" runat="server">
                    <div id="dvAttributes">
                        <asp:Literal ID="ltr_attrib" runat="server" />
                    </div>
                </asp:PlaceHolder>
                <div id="dvMedia">
                    <ucEktron:Media ID="ucMedia" runat="server" />
                </div>
            </asp:PlaceHolder>
            <div id="dvItems">
                <ucEktron:Items ID="ucItems" runat="server" />
            </div>
            <div id="dvComment">
                <div id="tdcommenttext" runat="server"></div>
            </div>
            <asp:PlaceHolder ID="phWebAlerts2" runat="server">
                <div id="dvWebAlerts">
                    <asp:Literal id="tdsubscriptiontext" runat="server"/>
                </div>
            </asp:PlaceHolder>
            <div id="dvLocalization">
                <asp:PlaceHolder ID="phLocalization2" runat="server" Visible="false"></asp:PlaceHolder>
            </div>
        </div>
    </div>
</div>
<input type="hidden" id="media_display_html" name="media_display_html" runat="server" />
<input type="hidden" id="media_html" name="media_html" runat="server" />
<input type="hidden" id="content_title" name="content_title" runat="server" />
<asp:Literal ID="AssetHidden" runat="server" />
<table id="ApprovalScript" runat="server">
    <tr>
        <td>
            <div id="FrameContainer" style="display:none; z-index: 2; left: 55px; width: 1px; position: absolute; top: 48px; height: 1px; background-color: white">
                <iframe id="ChildPage" marginheight="0" marginwidth="0" width="100%" scrolling="auto"></iframe>
            </div>
            <script type="text/javascript">
                <!--//--><![CDATA[//><!--
				function PopUp_ApprovalWindow(url, hWind, nWidth, nHeight, nScroll, nResize) {
					var cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + nWidth + ',height=' + nHeight;
					var popupwin = window.open(url, hWind, cToolBar);
					return popupwin;
				}

				function LoadChildPage(str) {
						if (IsBrowserIE()) {
							var frameObj = document.getElementById("ChildPage");
							var height = document.parentWindow.screen.height/100 *30;
							var width = document.parentWindow.screen.height/100 *45 ;
							frameObj.src = "AddTaskComment.aspx?" + str + "&height=" + height + "&width=" + width;
							var pageObj = document.getElementById("FrameContainer");
							pageObj.style.display = "";
							pageObj.style.width = width;
							pageObj.style.height = height;
						}
						else {
							PopUp_ApprovalWindow("blankredirect.aspx?AddTaskComment.aspx?" + str + "&height=490&width=500","Approval",490,500,1,1);
						}
				}

				function ReturnChildValue(str) {
					// take value, store it, write to display
					var pageObj = document.getElementById("FrameContainer");
					pageObj.style.display = "none";
					pageObj.style.width = "1px";
					pageObj.style.height = "1px";
					document.location.href = "content.aspx?"+str
				}

				function IsChildWaiting() {
					var pageObj = document.getElementById("FrameContainer");
					if (pageObj == null) {
						return (false);
					}
					if (pageObj.style.display == "") {
						return (true);
					}
					else {
						return (false);
					}
				}
                //--><!]]>
            </script>
        </td>
    </tr>
</table>
<script type="text/javascript">
    <!--//--><![CDATA[//><!--
	// This function is in viewform.ascx, viewcontent.ascx
	function disableFormElements(containingElement)
	{
		var oFormElem = null;
		if ("object" == typeof containingElement && containingElement != null)
		{
			oFormElem = containingElement;
		}
		else if ("string" == typeof containingElement && containingElement.length > 0)
		{
			if (typeof document.getElementById != "undefined")
			{
				oFormElem = document.getElementById(containingElement);
			}
		}
		if (!oFormElem) return;
		if ("undefined" == typeof oFormElem.getElementsByTagName) return;

		var aryTagNames = ["input", "select", "textarea"];
		var aryElems;
		for (var iTagName = 0; iTagName < aryTagNames.length; iTagName++)
		{
			aryElems = oFormElem.getElementsByTagName(aryTagNames[iTagName]);
			for (var i = 0; i < aryElems.length; i++)
			{
				aryElems[i].disabled = true;
			}
		}
		aryTagNames = ["label", "legend"];
		for (var iTagName = 0; iTagName < aryTagNames.length; iTagName++)
		{
			aryElems = oFormElem.getElementsByTagName(aryTagNames[iTagName]);
			for (var i = 0; i < aryElems.length; i++)
			{
				aryElems[i].contentEditable = false;
			}
		}
	}

	setTimeout("disableFormElements('_dvContent')", 100);
	ResizeFrame(1);
	var SelectedPane = document.getElementById('content_DefaultTab');
	if ((SelectedPane !== null) && (SelectedPane.value == 'dvProperties'))
	{
	    $ektron("#dvContent").attr("class", "tab_disabled");
	    $ektron(".menuContent").css("display", "none");
	    $ektron("#dvProperties").attr("class", "tab_actived");
	    $ektron("#_dvProperties").css("display", "block");
	}
    //--><!]]>
</script>
