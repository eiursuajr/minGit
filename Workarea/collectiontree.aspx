<%@ Page Language="C#" AutoEventWireup="true" CodeFile="collectiontree.aspx.cs" Inherits="Workarea_collectiontree" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <meta http-equiv="pragma" content="no-cache"/>
    <title>
        <%=AppUI.AppName + " " + "Collections"%>
    </title>

    <script type="text/javascript" language="JavaScript" src="java/toolbar_roll.js"></script>
    <link rel="stylesheet" type="text/css" href="csslib/ektron.fixedPositionToolbar.css"/>
    <script type="text/javascript">
<!--

        /***********************************************
        * Contractible Headers script- Â© Dynamic Drive (www.dynamicdrive.com)
        * This notice must stay intact for legal use. Last updated Oct 21st, 2003.
        * Visit http://www.dynamicdrive.com/ for full source code
        ***********************************************/

        var enablepersist = "off"; //Enable saving state of content structure using session cookies? (on/off)
        var collapseprevious = "no"; //Collapse previously open content when opening present? (yes/no)


        if (document.getElementById)
        {
            document.write('<style type="text/css">')
            document.write('.switchcontent{display:none;}')
            document.write('</style>')
        }

        function getElementbyClass(classname)
        {
            ccollect = new Array()
            var inc = 0
            var alltags = document.all ? document.all : document.getElementsByTagName("*")
            for (i = 0; i < alltags.length; i++)
            {
                if (alltags[i].className == classname)
                    ccollect[inc++] = alltags[i]
            }
        }

        function contractcontent(omit)
        {
            var inc = 0
            while (ccollect[inc])
            {
                if (ccollect[inc].id != omit)
                    ccollect[inc].style.display = "none"
                inc++
            }
        }

        function expandcontent(cid)
        {
            if (typeof ccollect != "undefined")
            {
                if (collapseprevious == "yes")
                    contractcontent(cid)
                document.getElementById(cid).style.display = (document.getElementById(cid).style.display != "block") ? "block" : "none"
            }
        }

        function revivecontent()
        {
            contractcontent("omitnothing")
            selectedItem = getselectedItem()
            selectedComponents = selectedItem.split("|")
            for (i = 0; i < selectedComponents.length - 1; i++)
                document.getElementById(selectedComponents[i]).style.display = "block"
        }

        function get_cookie(Name)
        {
            var search = Name + "="
            var returnvalue = "";
            if (document.cookie.length > 0)
            {
                offset = document.cookie.indexOf(search)
                if (offset != -1)
                {
                    offset += search.length
                    end = document.cookie.indexOf(";", offset);
                    if (end == -1) end = document.cookie.length;
                    returnvalue = unescape(document.cookie.substring(offset, end))
                }
            }
            return returnvalue;
        }

        function getselectedItem()
        {
            if (get_cookie(window.location.pathname) != "")
            {
                selectedItem = get_cookie(window.location.pathname)
                return selectedItem
            }
            else
                return ""
        }

        function saveswitchstate()
        {
            var inc = 0, selectedItem = ""
            while (ccollect[inc])
            {
                if (ccollect[inc].style.display == "block")
                    selectedItem += ccollect[inc].id + "|"
                inc++
            }

            document.cookie = window.location.pathname + "=" + selectedItem
        }

        function do_onload()
        {
            getElementbyClass("switchcontent")
            if (enablepersist == "on" && typeof ccollect != "undefined")
                revivecontent()
        }


        if (window.addEventListener)
            window.addEventListener("load", do_onload, false)
        else if (window.attachEvent)
            window.attachEvent("onload", do_onload)
        else if (document.getElementById)
            window.onload = do_onload

        if (enablepersist == "on" && document.getElementById)
            window.onunload = saveswitchstate
-->
    </script>

    <script type="text/javascript" language="JavaScript">
<!--

	function ConfirmNavDelete() {
		return confirm("<%=(MsgHelper.GetMessage("js: confirm collection deletion msg"))%>");
	}	
	
	function SubmitForm(FormName, Validate) {
		if (Validate.length > 0) {
			if (eval(Validate)) {
				document.forms[FormName].submit();
				return false;
			}
			else {
				return false;
			}
		}
		else {
			document.forms[FormName].submit();
			return false;
		}
	}

	function Move(sDir, objList, objOrder) {
		if (objList.selectedIndex != null) {
			nSelIndex = objList.selectedIndex;
			sSelValue = objList[nSelIndex].value;
			sSelText = objList[nSelIndex].text;
			if (sDir == "up" && nSelIndex > 0) {
				sSwitchValue = objList[nSelIndex -1].value;
				sSwitchText = objList[nSelIndex - 1].text;
				objList[nSelIndex].value = sSwitchValue;
				objList[nSelIndex].text = sSwitchText;
				objList[nSelIndex - 1].value = sSelValue;
				objList[nSelIndex - 1].text = sSelText;
				objList[nSelIndex - 1].selected = true;
			}
			else if (sDir == "dn" && nSelIndex < (objList.length - 1)) {
				sSwitchValue = objList[nSelIndex + 1].value;
				sSwitchText = objList[nSelIndex +  1].text;
				objList[nSelIndex].value = sSwitchValue;
				objList[nSelIndex].text = sSwitchText;
				objList[nSelIndex + 1].value = sSelValue;
				objList[nSelIndex + 1].text = sSelText;
				objList[nSelIndex + 1].selected = true;
			}
		}
		objOrder.value = "";
		for (i = 0; i < objList.length; i++) {
			objOrder.value = objOrder.value + objList[i].value;
			if (i < (objList.length - 1)) {
				objOrder.value = objOrder.value + ",";
			}
		}
	}
	
	function SelectAll() {
		var lLoop = 0;
		for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
			if ((document.forms.selections[lLoop].type.toLowerCase() == "checkbox")
					&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_check") != -1)) {
				document.forms.selections[lLoop].checked = true;
			}
		}
		var cArray = Collections.split(",");
		for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
			if ((document.forms.selections[lLoop].type.toLowerCase() == "hidden") 
							&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_hidden") != -1)) {
				var cIndex = document.forms.selections[lLoop].name.toLowerCase().replace("frm_hidden", "");
				document.forms.selections[lLoop].value = cArray[cIndex];
			}
		}
	}

	function ClearAll() {
		var lLoop = 0;
		for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
			if ((document.forms.selections[lLoop].type.toLowerCase() == "checkbox")
					&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_check") != -1)) {
				document.forms.selections[lLoop].checked = false;
			}
		}
		for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
			if ((document.forms.selections[lLoop].type.toLowerCase() == "hidden") 
							&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_hidden") != -1)) {
				document.forms.selections[lLoop].value = 0;
			}
		}
	}

	function GetIDs() {
		var lLoop = 0;
		document.forms.selections.frm_content_ids.value = "";
		document.forms.selections.frm_folder_ids.value = "";
		for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
			if ((document.forms.selections[lLoop].type.toLowerCase() == "hidden")
					&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_hidden") != -1)
					&& (document.forms.selections[lLoop].value != 0)) {
				document.forms.selections.frm_content_ids.value = document.forms.selections.frm_content_ids.value + "," + document.forms.selections[lLoop].value;
				document.forms.selections.frm_folder_ids.value = document.forms.selections.frm_folder_ids.value + "," + document.forms.selections[lLoop].fid;
			}
		}
		document.forms.selections.frm_content_ids.value = document.forms.selections.frm_content_ids.value.substring(1, document.forms.selections.frm_content_ids.value.length);
		document.forms.selections.frm_folder_ids.value = document.forms.selections.frm_folder_ids.value.substring(1, document.forms.selections.frm_folder_ids.value.length);
		if (document.forms.selections.frm_content_ids.value.length == 0) {
			alert("<%=(MsgHelper.GetMessage("js:no items selected"))%>");
			return false;
		}
		return true;
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

	function VerifyCollectionForm () {
		regexp1 = /"/gi; //Leave this comment "
		document.forms.nav.frm_nav_title.value = Trim(document.forms.nav.frm_nav_title.value);
		document.forms.nav.frm_nav_title.value = document.forms.nav.frm_nav_title.value.replace(regexp1, "'");

		document.forms.nav.frm_nav_description.value = Trim(document.forms.nav.frm_nav_description.value);
		document.forms.nav.frm_nav_description.value = document.forms.nav.frm_nav_description.value.replace(regexp1, "'");
		if (document.forms.nav.frm_nav_title.value == "")
		{
			alert ("<%=(MsgHelper.GetMessage("js: collection title required msg"))%>");
			document.forms.nav.frm_nav_title.focus();
			return false;
		}
		return true;
	}
	
//-->
    </script>
</head>
<body>
   
        <asp:Panel ID="pnlError" Visible="false" runat="server" >
                <div class="ektronToolbarHeader">
                        <table width="100%" class="ektronForm">
                            <tr>
                                <td class="ektronTitlebar">
                                    Select Folder - Error
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td class="titlebar-error">
                                    <%=ErrorString%>
                                </td>
                            </tr>
                        </table>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlPage" runat="server" >
              <form name="selections" method="post" action="Collections.aspx?folderid=<%=(folderId)%>&nid=<%=(nID)%>&parentid=<%=mpID%>&ancestorid=<%=maID%>&SelTaxonomyID=<%=selTaxID%>">
               
                 <div class="ektronToolbarHeader">
                    <table width="100%" >
                        <tr>
                            <td class="ektronTitlebar">
                                Select Folder</td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td class="label">
                                Selected
                                <%=(MsgHelper.GetMessage("foldername label"))%>
                            </td>
                            <td>
                                <%=("\"" + FolderName + "\"")%>
                            </td>
                        </tr>
                    </table>
                </div>
                <asp:Literal ID="ltrPage" runat="server" ></asp:Literal>
              
                <script type="text/javascript" language="javascript">
                    Collections = "<%=(cLinkArray)%>";
                    Folders = "<%=(fLinkArray)%>";
                </script>
                 <input type="hidden" name="frm_content_ids" value=""/>
                 <input type="hidden" name="frm_folder_ids" value=""/>
                 <input type="hidden" name="CollectionID" value="<%=(nID)%>"/>
              </form> 
        </asp:Panel>

</body>
</html>
