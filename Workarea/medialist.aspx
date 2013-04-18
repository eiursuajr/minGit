<%@ Page Language="C#" AutoEventWireup="true" Inherits="medialist" CodeFile="medialist.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
  <title></title>
  <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
	
  </head>
  <body class="PopupLibrary" runat="server" id="uxBodyTag">

  <script type="text/javascript">
	    <!--      //--><![CDATA[//><!--
      //setTimeout('initHdnVals()', 100);
      function initHdnVals() {
          var enhancedmetaselect = '<%=m_strEnhancedMetaSelect%>';
          var metadataformtagid = '<%=m_strMetadataFormTagId%>';
          var separator = '<%=m_strSeparator%>';
          var currentselectionids = '<%=m_selectids%>';
          var currentselectiontitles = '<%=m_selecttitles%>';

          if (enhancedmetaselect != '') {
              hdnObj = document.getElementById('enhancedmetaselect');
              if (hdnObj) {
                  hdnObj.value = enhancedmetaselect;
              }

              hdnObj = document.getElementById('metadataformtagid');
              if (hdnObj) {
                  hdnObj.value = metadataformtagid;
              }

              hdnObj = document.getElementById('separator');
              if (hdnObj) {
                  hdnObj.value = separator;
              }

              hdnObj = document.getElementById('currentselectionids');
              if (hdnObj) {
                  hdnObj.value = currentselectionids;
              }

              hdnObj = document.getElementById('currentselectiontitles');
              if (hdnObj) {
                  hdnObj.value = currentselectiontitles;
              }
          }
      }
      //--><!]]>
	</script>

	<% if(m_bAjaxTree) {%>
		<div id="TreeOutput" style="position:block;top:0;left:0;width:100%;height:100%;overflow:auto"> </div>
		<input type="hidden" id="folderName" name="folderName"/>
		<input type="hidden" id="selected_folder_id" name="selected_folder_id" value="0"/>
		<script type="text/javascript">
		<!--//--><![CDATA[//><!--
		//////////
		//
		// override the default images for the tree
		//

		//////////
		//
		// override the default images for the tree
		//

		//TreeDisplayUtil.plusclosefolder  = "Tree/images/xp/plusclosefolder.gif";
		//TreeDisplayUtil.plusopenfolder   = "Tree/images/xp/plusopenfolder.gif";
		//TreeDisplayUtil.minusclosefolder = "Tree/images/xp/minusclosefolder.gif";
		//TreeDisplayUtil.minusopenfolder  = "Tree/images/xp/minusopenfolder.gif";
		//TreeDisplayUtil.folder = "Tree/images/xp/folder.gif";
		// 1 - normal folders
		// 2 - blogs
		// 3 - domains
		// 4 - discussion board
		// 5 - discussion forum

		var pcfarray = new Array(10);
		pcfarray[0]  = "images/ui/icons/tree/folderCollapsed.png";
		pcfarray[1]  = "images/ui/icons/tree/folderBlogCollapsed.png";
		pcfarray[2]  = "images/ui/icons/tree/folderSiteCollapsed.png";
		pcfarray[3]  = "images/ui/icons/tree/folderBoardCollapsed.png";
		pcfarray[4]  = "images/ui/icons/tree/folderBoardCollapsed.png";
		pcfarray[5]  = "images/ui/icons/tree/home.png";
		pcfarray[6]  = "images/ui/icons/tree/folderCommunityCollapsed.png";
		pcfarray[7]  = "images/ui/icons/tree/folderFilmCollapsed.png";
		pcfarray[8]  = "images/ui/icons/tree/folderCalendarCollapsed.png"; // calendar
        pcfarray[9]  = "images/ui/icons/tree/folderGreenCollapsed.png";
		TreeDisplayUtil.plusclosefolders = pcfarray;
		var mcfarray = new Array(10);
		mcfarray[0]  = "images/ui/icons/tree/folderExpanded.png";
		mcfarray[1]  = "images/ui/icons/tree/folderBlogExpanded.png";
		mcfarray[2]  = "images/ui/icons/tree/folderSiteExpanded.png";
		mcfarray[3]  = "images/ui/icons/tree/folderBoardExpanded.png";
		mcfarray[4]  = "images/ui/icons/tree/folderBoardExpanded.png";
		mcfarray[5]  = "images/ui/icons/tree/home.png";
		mcfarray[6]  = "images/ui/icons/tree/folderCommunityExpanded.png";
		mcfarray[7]  = "images/ui/icons/tree/folderFilmExpanded.png";
		mcfarray[8]  = "images/ui/icons/tree/folderCalendarExpanded.png"; // calendar
        mcfarray[9]  = "images/ui/icons/tree/folderGreenExpanded.png";
		TreeDisplayUtil.minusclosefolders = mcfarray;
		var farray = new Array(10);
		farray[0]  = "images/ui/icons/tree/folder.png";
		farray[1]  = "images/ui/icons/tree/folderBlog.png";
		farray[2]  = "images/ui/icons/tree/folderSite.png";
		farray[3]  = "images/ui/icons/tree/folderBoard.png";
		farray[4]  = "images/ui/icons/tree/folderBoard.png";
		farray[5]  = "images/ui/icons/tree/home.png";
		farray[6]  = "images/ui/icons/tree/folderCommunity.png";
		farray[7]  = "images/ui/icons/tree/folderFilm.png";
		farray[8]  = "images/ui/icons/tree/folderCalendar.png"; // calendar
        farray[9]  = "images/ui/icons/tree/folderGreen.png";
		TreeDisplayUtil.folders = farray;

		//TreeDisplayUtil.plusclosefolder  = "Tree/images/xp/plusclosefolder.gif";
		//TreeDisplayUtil.plusopenfolder   = "Tree/images/xp/plusopenfolder.gif";
		//TreeDisplayUtil.minusclosefolder = "Tree/images/xp/minusclosefolder.gif";
		//TreeDisplayUtil.minusopenfolder  = "Tree/images/xp/minusopenfolder.gif";
		//TreeDisplayUtil.folder = "Tree/images/xp/folder.gif";

		var clickedElementPrevious = null;
		var clickedIdPrevious = null;
		var callback_function=''
		var hdnObj;

		function onContextMenuHandler( id, clickedElement ) { return false; }

		function onFolderNodeClick( id, clickedElement )
		{	
		    clickedElementPrevious = clickedElement;
			clickedIdPrevious = id;

			var folderName = clickedElement.innerText;
			var folderId   = id;

			document.getElementById( "folderName" ).value = folderName;
			document.getElementById( "selected_folder_id" ).value = id;
            var autonavfolder = getQuerystring("autonavfolder",0,parent["medialist"].location.href);
			parent["mediainsert"].location.href=ContentUrl+id+"&autonavfolder="+autonavfolder;
            returnValue = new Folder( folderName, folderId );
		}
		
		//Returns the Key/Value pairs 
        function getQuerystring(key, default_,url)
        {
          if (default_==null) default_="";
          key = key.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
          var regex = new RegExp("[\\?&]"+key+"=([^&#]*)");
          var qs = regex.exec(url);
          if(qs == null)
            return default_;
          else
            return qs[1];
        }
		function Folder( name, id )
		{
			this.name = name;
			this.id   = id;
		}

		function onToggleClick( id, callback, args )
		{
			toolkit.getChildFolders( id, -1, callback, args );
			if(callback_function==''){
				callback_function=callback;
			}
		}

		function displayTree()
		{
		    switch (FrameName) {
			    case "SmartDesktop":
			    case "Module":
			    case "Admin":{
				    var html='';
				    TreeOutput.innerHTML=html;
				    break;
			    }
			    default:{
				    toolkit.getRootFolder( function( folderRoot ) {
				    document.body.style.cursor = "default";
				    var ExpandIds = [<%=m_AutoNavFolderIDs%>];
				    if( vFolderName != null ) {
					    treeRoot = new Tree( vFolderName, 0, null, folderRoot, 0 );
					    TreeDisplayUtil.showSelf( treeRoot );
					    //TreeDisplayUtil.toggleTree( treeRoot.node.id );
					    TreeDisplayUtil.expandTreeSet( ExpandIds );	//[   ServerSideCall(Path)   ]
				    } else {
					    var element = document.getElementById( "TreeOutput" );
					    element.style["padding"] = "10pt";
					    var debugInfo = "<b>Cannot connect to the CMS server</b> "
					    element.innerHTML = debugInfo;
				    }
				    Explorer.onLoadExplorePanel();
				    }, 0 );
			    }
		    }
		}
		Ektron.ready(function(){     
            $ektron(window.parent.document).keydown(function(event) 
            {
                event = (event ? event : window.event);
                if (/*escape*/ 27 == event.keyCode && window.parent.radWindow)
                {
                    $ektron("span.RadERadWindowButtonClose", window.parent.radWindow.ContentWrapperTable).click();
                    event.stopImmediatePropagation();
                    return false;
                }
            });
        });
		//--><!]]>

		</script>
		<% } %>
	<form id="Form1" method="post" runat="server">
		<input type="hidden" id="enhancedmetaselect" name="enhancedmetaselect" value="" />
		<input type="hidden" id="metadataformtagid" name="metadataformtagid" value="" />
		<input type="hidden" id="separator" name="separator" value="" />
		<input type="hidden" id="currentselectionids" name="currentselectionids" value="" />
		<input type="hidden" id="currentselectiontitles" name="currentselectiontitles" value="" />

		<%if(!m_bAjaxTree) {%>
		<script type="text/javascript" src="java/ekfoldercontrol.js">
</script>

		<script type="text/javascript">
		<!--//--><![CDATA[//><!--
		// save a reference to the original ClearFolderInfo
		var __oldClearFolderInfo = ClearFolderInfo;
		// replace __doPostBack with another function
		ClearFolderInfo = FireBeforeClearFolderInfo;
		function FireBeforeClearFolderInfo () {
			top.SetSelectedLibraryItem("", "", "", "", "");
			// finally, let the original ClearFolderInfo do its work
			return __oldClearFolderInfo ();
		}
		<asp:Literal ID="ltr_ClientScript" Runat=server/>
		//--><!]]>
		</script>

		<script type="text/javascript">
		    <!--//--><![CDATA[//><!--
			InitializeFolderControl();
			//--><!]]>
		</script>
		<% } %>
		<asp:Literal ID="Msg" Runat="server"/>
    </form>
  </body>
</html>
