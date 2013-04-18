<%@ Page Language="C#" AutoEventWireup="true" Inherits="selectusergroup" ValidateRequest="false"
    CodeFile="selectusergroup.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>selectusergroup</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
    <meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE"/>
    <meta content="JavaScript" name="vs_defaultClientScript"/>
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    <asp:literal id="StyleSheetJS" runat="server"/>

    <script type="text/javascript">
	<!--//--><![CDATA[//><!--
		var m_bAllUsers = true;
		var m_bAllGroups = true;
		// Configure eWebEditPro:

		function IsBrowserIE() {
			// document.all is an IE only property
			return (document.all ? true : false);
		}

		function selectUser(Group, ID, Name){
			top.opener.selectUser(Group, ID, Name, 1);
			top.close();
		}

		function submit_form(buttonName) {
			//eWebEditPro.save();
			strUserList  = "";
			var strPage = '<asp:Literal id="ltr_rptStatus" runat="server" />';
			if (document.getElementById("frmEmailAddr") != null) {
			    // validate required email sender address
			    var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
                if (!emailPattern.test(document.getElementById("frmEmailAddr").value)) {
                    document.getElementById("frmEmailAddr").focus();
                    alert('<asp:Literal id="ltr_emailFromError" runat="server" />');
                    return;
                }
			}

			if (buttonName != 'send') {

					if ("siteupdateactivity" == strPage)
					{
						if (parent.document.getElementById("siteRptHtml") != null)
						{
							document.getElementById("attRptHtml").value = parent.document.getElementById("siteRptHtml").value;
						}
						else if (top.opener.document.getElementById("siteRptHtml") != null)
						{
							document.getElementById("attRptHtml").value = top.opener.document.getElementById("siteRptHtml").value;
						}
					}
					else
					{
						if (parent.document.getElementById("rptHtml") != null)
						{
							document.getElementById("attRptHtml").value = parent.document.getElementById("rptHtml").value;
						}
						else if (top.opener.document.getElementById("rptHtml") != null)
						{
						    document.getElementById("attRptHtml").value = top.opener.document.getElementById("rptHtml").value;
						}
					}
					//document.getElementById("setRptTitle").value = parent.document.getElementById("rptTitle").value;
					if (parent.document.getElementById("WorkAreaTitleBar") != null)
					{
				    document.getElementById("setRptTitle").value=parent.document.getElementById("WorkAreaTitleBar").outerText;//not for FF
					}
					else if (top.opener.document.getElementById("WorkareaTitlebar") != null)
					{
					document.getElementById("setRptTitle").value = top.opener.document.getElementById("WorkareaTitlebar").innerHTML;
					}

					haveTargets = false;
					strUserList = ProcessUserList();
					if (strUserList.length > 0)
					{
						haveTargets = true;
					}
					strGrpList = ProcessUserGroupList();
					if (strGrpList.length > 0)
					{
						haveTargets = true;
					}
					if (haveTargets) {
						document.getElementById("frm_email_list").value = strGrpList + ":" + strUserList;
						if (!(document.forms[0].frmEmailAddr == null)) {
							if ((document.forms[0].frmEmailAddr.value != "") || (document.forms[0].frmEmailAddr.value != undefined)) {
								document.forms[0].action = "selectusergroup.aspx?action=report&sndrEmail=" + document.forms[0].frmEmailAddr.value + "&rptStatus=" + strPage;
							}
						}
						else {
							document.forms[0].action = "selectusergroup.aspx?action=report&rptStatus=" + strPage;
						}
						document.forms[0].submit();
					}
					else
					{
						alert('<asp:Literal id="ltr_emailError" runat="server" />');
					}
				}
				else
			{ // false == if (buttonName != 'send'):
				var fname = document.getElementById("fname").value;
				var lname = document.getElementById("lname").value;
				var lnname = document.getElementById("loginname").value;
				var grpname = document.getElementById("grpname").value;
				document.forms[0].action = "selectusergroup.aspx?action=searchuser&fname=" + fname + "&lname=" + lname + "&loginname=" + lnname + "&groupname=" + grpname;
				document.forms[0].submit();
			}
		}

		function ProcessUserList()
		{
			var prefix = "emailUsercheckbox_";
			var qtyElements = document.forms[0].elements.length;
			var strUserList = "";
			for(idx = 0; idx < qtyElements; idx++ )
			{
				if (document.forms[0].elements[idx].type == "checkbox")
				{
					name = document.forms[0].elements[idx].name;
					if (name.indexOf("emailUsercheckbox_") >= 0)
					{
						if (document.forms[0].elements[idx].checked == true)
						{
							if ((document.getElementById("usraddr" + name.substring(prefix.length)) != null) && (document.getElementById("usraddr" + name.substring(prefix.length)).value != ""))
							{
								strUserList = strUserList + document.getElementById("usraddr" + name.substring(prefix.length)).value + ",";
							}
							else
							{
								strUserList = strUserList + name.substring(prefix.length) + ",";
							}
						}
						else
						{
							m_bAllUsers = false;
						}
					}
				}
			}
			return strUserList;
		}

		function ProcessUserGroupList()
		{
			var strGrpList = "";
			var prefix = "emailGrpcheckbox_";
			var qtyElements = document.forms[0].elements.length;
			for(idx = 0; idx < qtyElements; idx++ )
			{
				if (document.forms[0].elements[idx].type == "checkbox")
				{
					name = document.forms[0].elements[idx].name;
					if (name.indexOf("emailGrpcheckbox_") >= 0)
					{
						if (document.forms[0].elements[idx].checked == true)
						{
							strGrpList = strGrpList + name.substring(prefix.length) + ",";
						}
						else
						{
							m_bAllGroups = false;
						}
					}
				}
			}
			return strGrpList;
		}

		function SaveSelUserList()
		{
			<asp:literal id="JSUserListArray" runat="server"/>
			var sUserNames = "";
			var sGroupNames = "";
			var sNewLink = "<a href=\"javascript://\" id=\"selExclUser\" onclick=\"javascript:LoadUserListChildPage('siteupdateactivity_siteRpt')\">[None]</a>";

			haveTargets = false;
			strUserList = ProcessUserList();
			strUserList = strUserList.substr(0, strUserList.length - 1);
			if (strUserList.length > 0)
			{
				haveTargets = true;
			}
			strGrpList = ProcessUserGroupList();
			strGrpList = strGrpList.substr(0, strGrpList.length - 1);
			if (strGrpList.length > 0)
			{
				haveTargets = true;
			}
			var parentdoc = opener.document;
			var sUserList = "";
			if (haveTargets)
			{
				if (parentdoc.getElementById("excludeUserIds") != null)
				{
					parentdoc.getElementById("excludeUserIds").value = strUserList;
					parentdoc.getElementById("excludeUserGroups").value = strGrpList;
					if (parentdoc.getElementById("excludeUserList") != null)
					{
						if (0 == strUserList.length)
						{
							sUserNames = "None";
						}
						else
						{
							sUserNames = ConvertListFromArray(strUserList, JSUserIdArray, JSUserListArray);
						}
						if (0 == strGrpList.length)
						{
							sGroupNames = "None";
						}
						else
						{
							sGroupNames = ConvertListFromArray(strGrpList, JSGroupIdArray, JSGroupListArray);
						}
						sUserList = "User(" + sUserNames + ") : User Group(" + sGroupNames + ")";

						sNewLink = "<a href=\"javascript://\" id=\"selExclUser\" onclick=\"javascript:LoadUserListChildPage('siteupdateactivity_siteRpt')\">" + sUserList + "</a>";

						if (m_bAllUsers || m_bAllGroups)
						{
							parentdoc.getElementById("excludeAllUsers").value = "true";
						}
						else
						{
							parentdoc.getElementById("excludeAllUsers").value = "false";
						}
					}
				}
			}
			else
			{
				if (parentdoc.getElementById("excludeUserIds") != null)
				{
					parentdoc.getElementById("excludeUserIds").value = "";
					parentdoc.getElementById("excludeUserGroups").value = "";
					parentdoc.getElementById("excludeAllUsers").value = "false";
				}
			}

			parentdoc.getElementById("excludeUserList").innerHTML = sUserList.replace(/,/g, ", ").replace(" : User Group(", "<br />User Group (").replace("User(", "User ("); //sNewLink;
            top.close();
			return false;
		}

		function ConvertListFromArray(sSelectedIds, aFullId, aFullList)
		{
			var sName = "";
			if (0 == sSelectedIds.length) return sSelectedIds;
			if (0 == aFullId.length) return sSelectedIds;
			if (0 == aFullList.length) return sSelectedIds;
			var aSelectedIds = sSelectedIds.split(",");

			for (var i = 0; i < aSelectedIds.length; i++)
			{
				for (var j = 0; j < aFullId.length; j++)
				{
					if (aSelectedIds[i] == aFullId[j])
					{
						if (sName.length > 0)
						{
							sName = sName + ",";
						}
						sName = sName + aFullList[j];
					}
				}
			}
			return sName;
		}

		function ToggleCheckboxes ()
		{
			var idx, prefix, name;
			g_emailChecked = !g_emailChecked;
			for(idx = 0; idx < document.forms[0].elements.length; idx++ ) {
				if ((document.forms[0].elements[idx].type == "checkbox") && (document.forms[0].elements[idx].disabled==false)) {
					name = document.forms[0].elements[idx].name;
					if ((name.indexOf("emailGrpcheckbox_") != -1) || (name.indexOf("emailUsercheckbox_") != -1)){
						document.forms[0].elements[idx].checked = g_emailChecked;
					}
				}
			}
		}

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
	//--><!]]>
    </script>
    <script type="text/javascript">
        $ektron(document).ready(function(){
            $ektron(".ektronPageContainer").css({
                backgroundColor: 'transparent'
            });
        });

    </script>

</head>
<body>
    <form id="Form1" method="post" runat="server">
        <input id="frm_email_list" type="hidden" name="frm_email_list" />
        <input id="attRptHtml" type="hidden" name="attRptHtml" />
        <input id="setRptTitle" type="hidden" name="setRptTitle" />

        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" title="Select User or Group For Task" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div title="User or Group Name" class="ektronPageContainer ektronPageInfo">
            <div class="exception" id="TD_msg" runat="server"></div>
            <asp:Literal ID="EmailData" runat="server" />
            <div>
                <asp:DataGrid ID="UserGroupGrid"
                    runat="server"
                    AutoGenerateColumns="False"
                    Width="100%"
                    CssClass="ektronGrid"
                    HeaderStyle-CssClass="title-header"
                    GridLines="None">
                </asp:DataGrid>
            </div>
        </div>
    </form>
</body>
</html>

