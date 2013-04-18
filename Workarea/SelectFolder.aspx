<%@ Page Language="C#" AutoEventWireup="true" Inherits="SelectFolder"  validateRequest="false" CodeFile="SelectFolder.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>SelectFolder</title>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
        <asp:literal id="StyleSheetJS" runat="server" />		
		<script type="text/javascript">
		<!--//--><![CDATA[//><!--
		var folder_id = "<asp:literal id="jsFolderId" runat="server" />";
		var folder_path = "<asp:literal id="jsFPath" runat="server" />";
		var languageID = "<asp:literal id="jsContentLanguage" runat="server" />";
	    var taxonomy = "<asp:literal id="taxonomyString" runat="server" />";
		
		function doInitialize()
		{
			var parentWindow = window.parent;
			if (parentWindow 
				&& parentWindow.isMetaSelectContainer
				&& parentWindow.isMetaSelectContainer())
			{
				var obj = document.getElementById("select_folder_cancel_btn_container");
				if (obj)
				{
					obj.style.display = "none";
				}
				obj = document.getElementById("select_folder_save_btn_container");
				if (obj)
				{
					obj.style.display = "none";
				}
				obj = document.getElementById("select_folder_insert_btn_container");
				if (obj)
				{
					obj.style.display = "";
				}
			}
		}
		
		function insidePopUpWindow()
		{
			return (window.parent == window.top);
		}

		function insideContainer()
		{
			return (window.parent 
				&& window.parent.isMetaSelectContainer
				&& window.parent.isMetaSelectContainer());
		}

		function getMainWindow()
		{
			var mainWindow = null;

			if (insidePopUpWindow())
			{
				// we're inside a popup window.
				if (top.opener 
					&& !top.opener.closed) 
				{
					mainWindow = top.opener;
				}
			}
			else
			{
				// we're in an iframe.
				if (window.parent)
				{
					mainWindow = window.parent;
				}
			}
			return mainWindow;
		}

		function SaveSelCreateContent(RedirectUrl, ParentFolderId)
		{
			var idx, qtyElements, lnk, strTemp;
			var ListFoldersFor = "<asp:literal id="jsListFoldersFor" runat="server" />";
			var bTargetFolderIsXml = parseInt(<asp:literal id="jsTargetFolderIsXml" runat="server" />);
			var frmFormTagId = document.getElementById("frmFormTagId").value;
			var mainWindow; 
			if (window.parent 
				&& ((window.parent.isMetaSelectContainer
				&& window.parent.isMetaSelectContainer()) || (window.parent.isWikiSelectContainer && window.parent.isWikiSelectContainer())))
			{
				mainWindow = window.parent;
			}
			else
			{
				mainWindow = getMainWindow();
			}
			
			if (ListFoldersFor == "listsummary")
			{
			    location.href='editarea.aspx?LangType=' + languageID + '&type=add&id=' + folder_id + taxonomy;
			    return true;
			}
			else 
			{

			    if ((undefined == RedirectUrl) 
				    || ("" == RedirectUrl)) 
			    {
					if ((null == mainWindow)
						|| (("function" != typeof mainWindow.ReturnChildValue) && ("object" != typeof mainWindow.ReturnChildValue)))
					{
						alert("Unable to save changes: Cannot access mainWindow.ReturnChildValue().\n"
							+ "The Work-Area page may have been closed.");
						return false;
					}
			    
				    if (folder_id == "")
				    {
					    alert("Please select folder");
					    return false;
				    }
    				
				    var thirdParm = null;
				    if (frmFormTagId && frmFormTagId.length)
				    {
					    thirdParm = frmFormTagId;
				    }
				    else
				    {
					    thirdParm = bTargetFolderIsXml
				    }
				    // finally, let's return the data:
				    mainWindow.ReturnChildValue(folder_id, folder_path, thirdParm);				    
				    CloseSelContent();
				    return false;
			    }
			    else
			    {
				    var offset, search_string;
				    if (insidePopUpWindow())
				    {
					    offset = 5;
					    search_string = "Path:";
				    }
				    else
				    {
					    offset = 16;
					    search_string = "Selected Folder:";
				    }
    				
				    qtyElements = document.forms[0].elements.length;
				    // redirect to approval.aspx page if action = viewApprovalList else go to report with specified action
				    // Site activity has a link to select a folder and display its name in the page, needs to be treated different than other reports
				    if (RedirectUrl == "viewapprovallist") {
					    lnk = "Approval.aspx?action=" + RedirectUrl + "&fldid=";
				    }
				    else {
						    lnk = "reports.aspx?action=" + RedirectUrl + "&language=" + languageID + "&filtertype=path&filterid=" ;
				    }
				    if (RedirectUrl != "siteupdateactivity" ) {
					    lnk = lnk + ParentFolderId;
				    }
				    mainWindow.location = lnk;
				    CloseSelContent();
			    }
			    return false;
			}
		}
		
		function CancelSelContent()
		{
			return CloseSelContent();
		}
	
		function CloseSelContent()
		{
			if (!insideContainer())
			{
                			
				if(insidePopUpWindow() && !(window.parent.isWikiSelectContainer && window.parent.isWikiSelectContainer()) )
			    {
				    top.close();
			    }
			    else
			    {
				    if (parent.CloseChildPage)
				    {
					    parent.CloseChildPage();
				    }
			    }
			}
			return false;
		}

		function SetFolderChoice(folderId, folderPath)
		{
			folder_id = folderId;
			folder_path = folderPath;
		}
				
		function RecursiveSubmit(parentfolderid,noblogfolders,fromPage, pageAction)
		{
			var ExtraQuery = "<asp:literal id="jsExtraQuery" runat="server" />";		
			
			if (fromPage == 'undefined' || fromPage == null){
				fromPage = ""
			}
			if (pageAction == 'undefined' || pageAction == null){
				pageAction = ""
			}	
			

			var allSubfoldersChecked = false;
			if (document.forms[0].allsubfolders)
			{
				allSubfoldersChecked = document.forms[0].allsubfolders.checked;
			}
			
			document.forms[0].action="SelectFolder.aspx?FolderID=" + parentfolderid + ExtraQuery + "&from_page=" + fromPage + "&action=" + pageAction + "&noblogfolders=" + noblogfolders + "&subfolderchk=" + allSubfoldersChecked + taxonomy;
			
			document.forms[0].submit();
		}
		
		function SelectCatalog(folderid,folderpath)
		{
		    if (!insideContainer())
			{
                			
				if(insidePopUpWindow() && !(window.parent.isWikiSelectContainer && window.parent.isWikiSelectContainer()) )
			    {
				    var destFolder = window.opener.document.getElementById("move_folder_id");
		            destFolder.value = folderpath;
			        document.forms[0].submit();
			        top.close();   
			    }
			    else
			    {
				    if (parent.CloseChildPage)
				    {
					    var destFolder = parent.document.getElementById("move_folder_id");
		                destFolder.value = folderpath;
			            document.forms[0].submit();
		                CancelSelContent();   
				    }
			    }
			}
		}
		
		function checkAll()
		{
			var checked = document.getElementById("selectall").checked;
			var bFound = false;
			for(idx = 0; idx < document.forms[0].selectedfolder.length; idx++ ) 
			{
				bFound = true;
				if (checked)
				{
					document.forms[0].selectedfolder[idx].checked = true;
				}
				else
				{
					document.forms[0].selectedfolder[idx].checked = false;
				}
			}
			if (!bFound)
			{
				if (checked)
				{
					document.getElementById("selectedfolder").checked = true;
				}
				else
				{
					document.getElementById("selectedfolder").checked = false;
				}
			}
		}
		
		function SaveSelFolderList()
		{
			var sFolderId = "";
			var sFolderList = "";
			var sNewLink = "<a href=\"#\" id=\"hselFolder\" onclick=\"LoadFolderChildPage('siteupdateactivity','" + languageID + "');return true;\">\\</a>";
			var mainWindow = getMainWindow();
			var bFound = false;
			
			for(idx = 0; idx < document.forms[0].selectedfolder.length; idx++ ) 
			{
				bFound = true;
				if (true == document.forms[0].selectedfolder[idx].checked)
				{
					if (sFolderId.length > 0)
					{
						sFolderId = sFolderId + ",";
						sFolderList = sFolderList + ",";
					}
					sFolderId = sFolderId + document.forms[0].selectedfolder[idx].value;
					sFolderList = sFolderList + document.getElementById("selfolder" + document.forms[0].selectedfolder[idx].value).value;
				}
			}
			if (!bFound)
			{
				if (true == document.getElementById("selectedfolder").checked)
				{
					sFolderId = sFolderId + document.getElementById("selectedfolder").value;
					sFolderList = sFolderList + document.getElementById("selfolder" + document.getElementById("selectedfolder").value).value;
				}
			}

			if (mainWindow)
			{ 
				if (sFolderId.length > 0)
				{
					if (mainWindow.document.getElementById("fId") != null)
					{
						mainWindow.document.getElementById("fId").value = sFolderId;
					}
					if (mainWindow.document.getElementById("subfldInclude") != null) // allsubfolders
					{
						mainWindow.document.getElementById("subfldInclude").value = document.forms[0].allsubfolders.checked;
						if (mainWindow.document.getElementById("subfldIncludetxt") != null)
						{
							if (document.forms[0].allsubfolders.checked)
							{
								mainWindow.document.getElementById("subfldIncludetxt").style.display = "inline";
							}
							else
							{
								mainWindow.document.getElementById("subfldIncludetxt").style.display = "none";
							}
						}
					}
					if (mainWindow.document.getElementById("rootFolder") != null)
					{
						mainWindow.document.getElementById("rootFolder").value = document.getElementById("rootFolder").value;
					}
					
					if (mainWindow.document.getElementById("selectedFolderList") != null)
					{
						sNewLink = "<a href=\"#\" id=\"hselFolder\" onclick =\"LoadFolderChildPage('siteupdateactivity','" + languageID + "');return true;\">" + sFolderList + "</a>";
					}
				}
				else
				{
					mainWindow.document.getElementById("fId").value = "0";
					mainWindow.document.getElementById("rootFolder").value = "0";
					mainWindow.document.getElementById("subfldInclude").value = false;
					mainWindow.document.getElementById("subfldIncludetxt").style.display = "none";
				}
				mainWindow.document.getElementById("selectedFolderList").innerHTML = sNewLink;
			}
			
			CloseSelContent();
			return false;
		}
		
		function ShowTransString(Text) 
		{
		    var ObjId = "WorkareaTitlebar";
		    var ObjShow = document.getElementById('_' + ObjId);
		    var ObjHide = document.getElementById(ObjId);
		    if ((typeof ObjShow != "undefined") && (ObjShow != null)) 
		    {
			    ObjShow.innerHTML = Text;
			    ObjShow.style.display = "inline";
			    if ((typeof ObjHide != "undefined") && (ObjHide != null)) 
			    {
				    ObjHide.style.display = "none";
			    }
		    }
	    }
	    
	    function HideTransString() {
		    var ObjId = "WorkareaTitlebar";
		    var ObjShow = document.getElementById(ObjId);
		    var ObjHide = document.getElementById('_' + ObjId);

		    if ((typeof ObjShow != "undefined") && (ObjShow != null)) {
			    ObjShow.style.display = "inline";
			    if ((typeof ObjHide != "undefined") && (ObjHide != null)) {
				    ObjHide.style.display = "none";
			    }
		    }
	    }
		//--><!]]>
		</script>
		<style type="text/css">
		    span.selectedFolder {display: inline-block; color:#000000; margin-left: .5em; padding: .25em; border: solid 1px #ccc; cursor: default; background-color: #eee;}
		    .folderOutput img {margin-right: .5em;}
		</style>
	</head>
	<body id="main_body" onload="doInitialize();">
		<form id="Form1" method="post" runat="server">
			<div id="dhtmltooltip"></div>									
			<div class="ektronPageHeader">
			    <div class="ektronTitlebar" title="Select Folder" id="divTitleBar" runat="server"></div>
			    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
			</div>
            <div class="ektronPageContainer ektronPageInfo">
                <asp:DataGrid ID="ContentGrid"
                    Runat="server"
                    OnItemDataBound="Grid_ItemDataBound"
			        BorderStyle="None"
			        AutoGenerateColumns="False"
			        EnableViewState="False"
			        BorderWidth="0"
			        GridLines="None"
			        Height="100%" 
			        CssClass="folderOutput"
			        />
			</div>
            <asp:HiddenField ID="frmFormTagId" runat="server" Value="" />
		</form>
	</body>
</html>


