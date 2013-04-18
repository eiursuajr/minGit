<%@ Control Language="C#" AutoEventWireup="true" CodeFile="assigntaxonomy.ascx.cs"
    Inherits="assigntaxonomy" %>

<script type="text/javascript">
    function Validate(){
        if(!IsSelected('itemlist')){
                alert('<%=m_refMsg.GetMessage("js select one or more items and then click update button to add this items")%>');
                return false;
        }
        $ektron("#txtSearch").clearInputLabel();
        document.forms[0].submit();
    }
    function ChangeView() {
        var iViewType = document.getElementById('typelist').value;
        if (iViewType == 21000) {
            window.location.href = '<%=getURL()%>&type=author';
        } else if (iViewType == 22000) {
            window.location.href = '<%=getURL()%>&type=member';
        } else if (iViewType == 19) {
            window.location.href = '<%=getURL()%>&type=cgroup';
        } else {
        if(document.getElementById('contenttype') != null){
          var iContentType = document.getElementById('contenttype').value;
          if(iContentType=="activecontent")
            window.location.href = '<%=getURL()%>&contFetchType=activecontent';
          else if(iContentType=="library")
            window.location.href = '<%=getURL()%>&contFetchType=library';
          else if(iContentType=="archivedcontent")
            window.location.href = '<%=getURL()%>&contFetchType=archivedcontent';
          else
            window.location.href = '<%=getURL()%>';
          }
         else{
            window.location.href = '<%=getURL()%>'
          }
       }
    }
    function searchuser(){
	    if(document.getElementById('txtSearch').value.indexOf('\"') != -1){
	        alert('<%=m_refMsg.GetMessage("js remove all quotes then click search")%>');
	        return false;
	    }
	    $ektron("#txtSearch").clearInputLabel();
	    document.forms[0].submit();
	    return true;
	}
	function CheckForReturn(e)
	{
	    var keynum;
        var keychar;

        if(window.event) // IE
        {
            keynum = e.keyCode
        }
        else if(e.which) // Netscape/Firefox/Opera
        {
            keynum = e.which
        }

        if( keynum == 13 ) {
            document.getElementById('btnSearch').focus();
        }
	}
    function OnFolderCheck(v,action){
        var e=document.getElementById('_dv'+v);
        if(action.checked && e!=null){

            var newElt2 = document.createElement('span');
            newElt2.setAttribute("id","spacechk");
            newElt2.innerHTML="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

            e.appendChild(newElt2);
            var newElt = document.createElement('input');
            newElt.setAttribute("id","recursiveidlist");
            newElt.setAttribute("name","recursiveidlist");
            newElt.setAttribute("type","checkbox");
            newElt.setAttribute("value",v);
            newElt.setAttribute("title","Check here to include all its subfolder.");
            e.appendChild(newElt);
            var newElt1 = document.createElement('span');
            newElt1.setAttribute("id","spanchk");
            newElt1.innerHTML="Include subfolder(s).";
            e.appendChild(newElt1);
            e.style.display="block";
        }
        else
        {
            while (e.firstChild) {
                e.removeChild(e.firstChild);
            }
        }
    }
	function resetPostback(){
        document.forms[0].taxonomy_isPostData.value = "";
	}
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <div class="ektronPageGrid">
        <asp:GridView ID="TaxonomyItemList"
            runat="server"
            AutoGenerateColumns="False"
            Width="100%"
            EnableViewState="False"
            GridLines="None" />
    </div>
    <p class="pageLinks">
        <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
        <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
        <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
        <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
    </p>
    <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
        OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
    <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="PreviousPage" Text="[Previous Page]"
        OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
    <asp:LinkButton ToolTip="Next" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
        OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
    <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
        OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
  <%  if(m_strPageAction == "addfolder")
        {%>
        <%=m_refMsg.GetMessage("assigntaxonomyfolderlabel")%>
        <div class="ektronTopSpace"></div>
        <div id="TreeOutput" style="position: absolute;"></div>
        <%}%>
</div>
<input type="hidden" value="" name="item_id_list" id="item_id_list" />
<input type="hidden" value="" name="item_language_list" id="item_language_list" />
<input type="hidden" value="" name="item_folder_list" id="item_folder_list" />
<% if (m_strPageAction == "addfolder")
   {%>
<input type="hidden" id="folderName" name="folderName" />
<input type="hidden" id="selectedfolder" name="selectedfolder" value="<%=m_selectedFolderList%>"/>
<input type="hidden" id="selected_folder_id" name="selected_folder_id" value="0" />

<script type="text/javascript">
    var vFolderName='Folder';
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

    var clickedElementPrevious = null;
    var clickedIdPrevious = null;
    var selectedfolderarr="<%=m_selectedFolderList%>".split(",");
    function IsAlreadySelected(fid){
        for(var i=0;i<selectedfolderarr.length;i++){
            if(selectedfolderarr[i]==fid){
                return true;
                break;
            }
        }
        return false;
    }
    function UpdateSelectedValue(control){
        var fid=control.value;
        if(control.checked){
            selectedfolderarr.splice(0,0,fid);
        }else{
            for(var i=0;i<selectedfolderarr.length;i++){
                if(selectedfolderarr[i]==fid){
                    selectedfolderarr.splice(i,1);break;
                }
            }
        }
        document.getElementById("selectedfolder").value="";
        for(var i=0;i<selectedfolderarr.length;i++){
            if(document.getElementById("selectedfolder").value==""){
                document.getElementById("selectedfolder").value=selectedfolderarr[i];
            }else{
                document.getElementById("selectedfolder").value=document.getElementById("selectedfolder").value+","+selectedfolderarr[i];
            }
        }
    }

    function onContextMenuHandler( id, clickedElement ) { return false; }

    function onFolderClick( id, clickedElement )
    {
	    return (onFolderClickEx( id, clickedElement, true ));
    }

    function onFolderClickEx( id, clickedElement, openMainPage )
    {
	    if( clickedElementPrevious != null ) {
		    clickedElementPrevious.style["background"] = "#ffffff";
		    clickedElementPrevious.style["color"] = "#000000";
	    }

	    clickedElement.style["background"] = "#3366CC";
	    clickedElement.style["color"] = "#ffffff";
	    clickedElementPrevious = clickedElement;
	    clickedIdPrevious = id;

	    var folderName = clickedElement.innerText;
	    var folderId   = id;

	    document.getElementById( "folderName" ).value = folderName;
	    document.getElementById( "selected_folder_id" ).value = id;

	    	    returnValue = new Folder( folderName, folderId );
    }

    function Folder( name, id )
    {
	    this.name = name;
	    this.id   = id;
    }

    function onToggleClick( id, callback, args )
    {
        //var callbackstr = callback.toString();
	    toolkit.getChildFolders( id, -1, callback, args );
    }

    var g_selectedFolderList = "<%=m_selectedFolderList%>";
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
		    var tree = TreeUtil.getTreeById( 0 );
		    if (tree)
		    {
			    var lastId = 0;
			    var folderList = g_selectedFolderList.split(",");
			    bSuccessFlag = TreeDisplayUtil.expandTreeSet( folderList );
		    }

		    if (!bSuccessFlag)
		    {
			    g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
		    }
		    else
		    {
			    var idValStr = folderList[folderList.length-1];
			    var idVal = parseInt(idValStr,10);
			    var obj = document.getElementById(idValStr);
			    if (obj)
			    {
				    onFolderClickEx(idVal, obj, false);
			    }
			    else
			    {
				    g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
			    }
		    }
	    }
    }

    function displayTreeFolderSelect()
    {
			    toolkit.getRootFolder( function( folderRoot ) {
			    document.body.style.cursor = "default";
			    if( vFolderName != null ) {
				    treeRoot = new Tree( vFolderName, 0, null, folderRoot );
				    TreeDisplayUtil.showSelf( treeRoot );
				    TreeDisplayUtil.toggleTree( treeRoot.node.id );

			    } else {
				    var element = document.getElementById( "TreeOutput" );
				    element.style["padding"] = "10pt";
				    var debugInfo = "<b>Cannot connect to the CMS server</b> "
				    element.innerHTML = debugInfo;
			    }
			    Explorer.onLoadExplorePanel();
			    }, 0 );

    }

    function isBrowserFireFox()
    {
	    return (top.IsBrowserFF && top.IsBrowserFF());
    }

    function setupClassNames()
    {
	    var navContainer = document.getElementById("TreeOutput");

	    if (isBrowserFireFox()) {
	    // Use CSS for FF:
		    navContainer.setAttribute("class", "NavTreeFF");
		    document.body.setAttribute("class", "BodyForFFNavTree");
	    }
	    else {
	    // Use CSS for IE/everyone else:
		    navContainer.setAttribute("className", "NavTreeIE");
		    document.body.setAttribute("className", "BodyForIENavTree");
	    }
    }
</script>

<% } %>

