<%@ Page Language="C#" AutoEventWireup="true" CodeFile="urlAutoAliasSourceSelector.aspx.cs" Inherits="Workarea_urlAutoAliasSourceSelector" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Select Source</title>
	<script  type="text/javascript">
		var appPath = "<asp:Literal runat='server' id='litAppPath' />";
		var folder_id = "<asp:Literal runat='server' id='litFolderID' />";
        var SourceTaxonomyPath = '';
		var folder_path = "<asp:Literal runat='server' id='litFolderPath' />";
		var languageID = "<asp:Literal runat='server' id='litContLanguage' />";
		if(parent.document.getElementById("taxonomy_hdn_parentTaxonomyPath") !== null)
		{
		    SourceTaxonomyPath = parent.document.getElementById("taxonomy_hdn_parentTaxonomyPath").value;
		}
		// Move Copy Taxonomy related code starts
		
		if(parent.document.getElementById("taxonomy_hdnSourceId") !== null)
		{
		    var sourceId = parent.document.getElementById("taxonomy_hdnSourceId").value;
		}
		var destinationId = folder_id;
		var jsSelDiffTax = "<asp:Literal runat='server' id='litSelDiffTax'/>";
		var sourceTaxName = "<asp:Literal runat='server' id='litSourceTaxName'/>";
		var destinationTaxName = "<asp:Literal runat='server' id='litDestinationTaxName'/>";
		var newTaxID = "<asp:Literal runat='server' id='litNewTaxId' />";
		
		function moveCopyTaxonomy(movecopy,taxonomyType)
		{
		    if ((sourceId == destinationId) || (SourceTaxonomyPath == folder_path))
		    {
		        alert(jsSelDiffTax);
		        return false;
		    }
		    else
		    {
//                if(taxonomyType == "Locale")
//                {
//                    location.href = 'urlAutoAliasSourceSelector.aspx?CopyType=Locale&action=' + movecopy + '&Sourceid=' + sourceId + '&destinationid=' + destinationId + '&LangType=' + languageID + '';
//                    top.refreshTaxonomyAccordion(languageID);	
//                    parent.location.href = 'Localization/LocaleTaxonomy.aspx?action=view&view=locale&taxonomyid=' + destinationId + '&treeViewId=-1&LangType=' + languageID + '';
//                }
//                else
//                {
                    $ektron.ajax(
                    {
                        type: "POST",
                        url: appPath + "MoveCopyTaxonomy.ashx?action=" + movecopy + "&Sourceid=" + sourceId + "&destinationid=" + destinationId + "&LangType=" + languageID,
                        data:"",
                        dataType: "text",
                        success: function(data)
                        {
                            if(data.toString().indexOf("error") !== -1)
                            {
                                parent.location.href = "reterror.aspx?info=" + data.replace("error: " ,'') + "&LandType=" + languageID;
                                parent.$ektron('#TaxonomySelect').modalHide();
                                return false;
                            }
                            else if(taxonomyType == "Locale")
                            {
                                top.refreshTaxonomyAccordion(languageID);	
                                parent.location.href = 'Localization/LocaleTaxonomy.aspx?action=view&view=locale&taxonomyid=' + destinationId + '&treeViewId=-1&LangType=' + languageID + '';
                            }
                            else
                            {
                                top.refreshTaxonomyAccordion(languageID);	
                                parent.location.href = 'taxonomy.aspx?action=view&view=item&reloadtrees=Tax&taxonomyid=' + destinationId + '&treeViewId=-1&LangType=' + languageID + '';    
                            }
                        },
                        
                        error: function(XMLHttpRequest, textStatus, errorThrown)
                        {
                            return false;
                        }
                    });
                //}
            }
		}
		
		// Move Copy Taxonomy related code ends

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
				obj = document.getElementById("main_body");
				if (obj)
				{
					obj.style.backgroundColor = "#ffffe1";
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
			var ListFoldersFor = "<asp:Literal runat='server' id='litListFoldersFor' />";
			var bTargetFolderIsXml = parseInt(<asp:Literal runat="server" id="litTargetFolderIsXml" />);
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
			    location.href='urlmanualaliasmaint.aspx?LangType=<asp:Literal runat="server" id="litLangID" />&type=add&id=<asp:Literal runat="server" id="ltrFolderID" />';
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
				    if (mainWindow.ReturnChildValue)
				    {
				        mainWindow.ReturnChildValue(folder_id, folder_path, thirdParm);
				    }
				    if ("undefined" !== typeof(CloseSelContent))
				    {
				        CloseSelContent();
				    }
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
					    lnk = "Approval.aspx?action=" + RedirectUrl + "&fldid="
				    }
				    else {
						    lnk = "reports.aspx?action=" + RedirectUrl + "&language=" + languageID + "&filtertype=path&filterid=" ;
				    }
				    if (RedirectUrl != "siteupdateactivity" ) {
					    lnk = lnk + ParentFolderId;
				    }
				    mainWindow.document.forms[0].action = lnk;
				    mainWindow.document.forms[0].__VIEWSTATE.name = 'NOVIEWSTATE';
				    mainWindow.document.forms[0].submit();
				    CloseSelContent();
			    }
			    return false;
			}
		}

		function CancelSelContent()
		{
			return CloseSelContent();
		}

		function SetFolderChoice(folderId, folderPath)
		{
			folder_id = folderId;
			folder_path = folderPath;
		}

		function RecursiveSubmit(parentfolderid,noblogfolders,fromPage, pageAction,mode, pagerCmd)
		{
			var ExtraQuery = "<asp:Literal runat='server' id='litExtraQuery' />";
			var aliasOrTax = "<asp:Literal runat='server' id='ltraliasOrTax' />";
            $ektron("#aliasOrTax")[0].value = aliasOrTax;
            
			if (fromPage == 'undefined' || fromPage == null){
				fromPage = ""
			}
			if (pageAction == 'undefined' || pageAction == null){
				pageAction = ""
			}
			if (pagerCmd == 'undefined' || pagerCmd == null){
				pagerCmd = ""
			}


			var allSubfoldersChecked = false;
			if (document.forms[0].allsubfolders)
			{
				allSubfoldersChecked = document.forms[0].allsubfolders.checked;
			}

			document.forms[0].action="urlAutoAliasSourceSelector.aspx?FolderID=" + parentfolderid + ExtraQuery + "&from_page=" + fromPage + "&action=" + pageAction + "&noblogfolders=" + noblogfolders + "&subfolderchk=" + allSubfoldersChecked +"&mode=" + mode + "&pager=" + pagerCmd + "&aliasortax=" + aliasOrTax;

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
			var sNewLink = "<a href=\"javascript://\" id=\"hselFolder\" onclick=\"LoadFolderChildPage('siteupdateactivity','" + languageID + "');return true;\">\\</a>";
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
						sNewLink = "<a href=\"javascript://\" id=\"hselFolder\" onclick =\"LoadFolderChildPage('siteupdateactivity','" + languageID + "');return true;\">" + sFolderList + "</a>";
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

			CloseSelContent()
			return false;
		}

	function resetPostback(){
        document.forms[0].form1_isPostData.value = "";
	}

		</script>
		<script  type="text/javascript" src="java/toolbar_roll.js">
</script>
		<asp:literal id="StyleSheetJS" runat="server" />
		<style type="text/css">
		    span.selectedContent {display: inline-block; color:#000000; margin-left: .5em; padding: .25em; border: solid 1px #ccc; cursor: default; background-color: #eee;}
		    .contentOutput img {margin-right: .5em;}
		</style>
</head>
<body id="main_body" onload="doInitialize();">
    <form id="form1" runat="server" class="modal-content">
	    <div id="dhtmltooltip"></div>
		<div class="ektronPageHeader">
		    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <asp:DataGrid  ID="ContentGrid"
                Runat="server"
                OnItemDataBound="Grid_ItemDataBound"
	            AutoGenerateColumns="False"
                EnableViewState="False"
                GridLines="None"
                Height="100%"
                ShowHeader="false"
                CssClass="contentOutput"
                />
		    <asp:HiddenField ID="frmFormTagId" runat="server" Value="" />
    <p class="pageLinks">
        <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
        <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
        <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
        <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
    </p>
    <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
        CommandName="First" OnClientClick="resetPostback()" />
    <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="PreviousPage1" Text="[Previous Page]"
        CommandName="Prev" OnClientClick="resetPostback()" />
    <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
        CommandName="Next" OnClientClick="resetPostback()" />
    <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
         CommandName="Last" OnClientClick="resetPostback()" />
		</div>
    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
    <input type="hidden" runat="server" id="aliasOrTax" value="true" name="aliasOrTax" />
    </form>
</body>
</html>

