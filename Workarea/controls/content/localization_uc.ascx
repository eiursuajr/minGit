<%@ Control Language="C#" AutoEventWireup="false" Inherits="localization_uc" CodeFile="localization_uc.ascx.cs" %>

<script language="JavaScript" type="text/javascript">
<!--
	function LoadContent(FormName,opt){
		var checkedFolderID = "<asp:literal id="jsFolderId" runat="server"/>";
		var jsToolId ="<asp:literal id="jsToolId" runat="server"/>";
		var jsToolAction="<asp:literal id="jsToolAction" runat="server"/>";
		var jsBackStr="<asp:literal id="jsBackStr" runat="server"/>";
		var jsIsForm = "<asp:Literal id="jsIsForm" runat="server"></asp:Literal>";
		if(opt=='VIEW'){
			var num=document.forms[0].localization.selectedIndex;
			document.forms[0].action="content.aspx?folder_id="+checkedFolderID+"&id="+jsToolId+"&action="+jsToolAction+"&LangType="+document.forms[0].localization.options[num].value;
			document.forms[0].submit();
		}else{
			var num=document.forms[0].addcontent.selectedIndex;
			if(document.forms[0].addcontent.options[num].value!=0)
			{
			    if (jsIsForm == '2')
			    {
		            document.forms[0].__VIEWSTATE.name = 'NOVIEWSTATE';
        			document.forms[0].action="cmsform.aspx?back_LangType="+jsContentLanguage+"&form_id="+jsToolId+"&LangType="+document.forms[0].addcontent.options[num].value+"&action=Addform&folder_id="+checkedFolderID+"&callbackpage=cmsform.aspx&parm1=action&value1=ViewAllFormsByFolderID&parm2=folder_id&value2="+checkedFolderID;
			        document.forms[0].submit();
			    }
			    else
			    {
			        top.document.getElementById('ek_main').src = 'edit.aspx?close=false&LangType=' + document.forms[0].addcontent.options[num].value + '&content_id=' + jsToolId + '&type=add&createtask=1&id=' + checkedFolderID + '&' + jsBackStr;
			    }
			}

		return false;
		}
	}
	function SetAction(sAction){
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
		else
		{
			// something not expected
			return true;
		}

		// re-directed to contentaction page
		DisplayHoldMsg(true);
		document.location.href = sHtmlAction;
	}

	function validate()
	{
		var valid = true;
		var numSelected = 0;
		var strLanguages = "";
		var objForm = document.forms[0];
		var objElem = null;
		var nDefaultLanguage = <asp:literal id="jsDefaultLanguage" runat="server" />;

		if (valid)
		{
			//objElem = objForm.elements["frm_langID"];
			objElem = objForm.elements['<asp:literal id="jsSourceLanguageListID" runat="server"/>'];
			if (objElem && objElem.value != nDefaultLanguage)
			{
				var msg = "Most often the default language of the CMS is the source language, click OK if you are sure you want to continue.";
				valid = confirm(msg);
			}
		}

		strLanguages = "";
		if (valid)
		{
			for (var iCount = 0; ; iCount++)
			{
				objElem = objForm.elements[iCount];
				if (null == objElem) break;
				if (objElem.name && objElem.name.indexOf("ExportLang") != -1)
				{
					if (objElem.checked)
					{
						if (strLanguages.length > 0)
						{
							strLanguages += ",";
						}
						strLanguages += objElem.value;
						numSelected++;
					}
				}
			}
			if (0 == numSelected)
			{
				alert("Please select at least one target language.");
				valid = false;
			}
		}
		objForm.elements["TargetLanguages"].value = strLanguages;
		objForm.elements["action"].value = "LocalizeExport";
		if (!valid)
		{
			DisplayHoldMsg(false);
		}
		return valid;
	}

	function onCheckAll(objThis)
	{
		var objForm = document.forms[0];
		var objElem = null;
		for (var iCount = 0; ; iCount++)
		{
			objElem = objForm.elements[iCount];
			if (null == objElem) break;
			if (objElem.name && objElem.name.indexOf("ExportLang") != -1)
			{
				objElem.checked = objThis.checked;
			}
		}
	}
	
	function DisplayXLIFFPanel(bShow) {	
		var obj = document.getElementById("<%=pnlForm.ClientID%>");
		if (obj != null) { 
			if (bShow == false) {obj.style.display = "none";}
			else {obj.style.display = "block";}
		}
		var obj = document.getElementById("<%=pnlXLIFFData.ClientID%>");
		if (obj != null) { 
			if (bShow == false) {obj.style.display = "none";}
			else {obj.style.display = "block";}
		}		
		// don't return false or true;
	}
// -->
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>

<asp:Panel ID="pnlOuterForm" Cssclass="ektronPageContainer ektronPageInfo" runat="server">
    <div class="ektronPageContainer ektronPageInfo" style="display:none" id="dvHoldMessage">
        <table class="ektronForm">
            <tr>
                <td valign="top" align="center">
                    <h3 style="color: red"><asp:Literal ID="HoldMomentMsg" runat="server" /></h3>
                </td>
            </tr>
        </table>
    </div>

  <asp:Panel ID="pnlForm" runat="server">
	<asp:CheckBox ToolTip="Include subfolders" ID="chkRecursive" runat="server" Checked="True" Text="Include subfolders" />
	<div runat="server" class="ui-state-highlight" id="MenuWarning" visible="false"></div>
	<div class="ektronTopSpaceSmall"></div>
	
	<table class="ektronForm">
	    <tr>
	        <td class="label" title="Source Language"><%=GetMessage("lbl source language")%></td>
	        <td class="value"><asp:DropDownList ToolTip="Source Language" ID="ddlSourceLanguage" runat="server" AutoPostBack="true"/></td>
	    </tr>
	</table>

	<asp:Panel ID="pnlTargetLocales" runat="server">
		<fieldset>
			<legend title="Target Language"><%= GetMessage("lbl target language")%></legend>
			<div class="ektronPageGrid">
				<asp:GridView ID="LanguageGrid" runat="server"
					EnableViewState="true"
					AllowSorting="false"
					AutoGenerateColumns="false"
					ShowHeader="true"
					Width="100%"
					GridLines="None">
					<HeaderStyle CssClass="title-header" />
				</asp:GridView>
			</div>
		</fieldset>
	</asp:Panel>
  </asp:Panel>

  <asp:Panel ID="pnlXLIFFData" runat="server">
    <div class="ektronTopSpace"></div>
    <div class="ektronHeader" title="History"><%= GetMessage("lbl generic history")%></div>
    <div class="ektronBorder">
        <iframe src="Localization/localizationjobs.aspx" width="96%" height="360"></iframe>
    </div>

    <input type="hidden" name="TargetLanguages" value="" />
    <input type="hidden" name="action" value="" />
  </asp:Panel>
</asp:Panel>
