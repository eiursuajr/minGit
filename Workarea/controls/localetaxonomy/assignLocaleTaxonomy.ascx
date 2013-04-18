<%@ Control Language="C#" AutoEventWireup="true" CodeFile="assignLocaleTaxonomy.ascx.cs" 
Inherits="assignLocaleTaxonomy" %>

<script type="text/javascript">
<!--
    function Validate(){
        if(!IsSelected('itemlist')){
                alert('<%= m_refMsg.GetMessage("js select one or more items and then click update button to add this items")%>');
                return false;
        }
        $ektron("#txtSearch").clearInputLabel();
        document.forms[0].submit();
    }
    function ChangeView() {
       // debugger;
        var iViewType = document.getElementById('typelist').value;
        if (iViewType == 21000) {
            window.location.href = '<%= getURL() %>&type=author';
        } else if (iViewType == 22000) {
            window.location.href = '<%= getURL() %>&type=member';
        } else if (iViewType == 19) {
            window.location.href = '<%= getURL() %>&type=cgroup';
        }
        //iViewType == 9 for locale taxonomy type
        else if (iViewType == 9) {
        window.location.href = '<%= getURL() %>&type=locales';
        }
        else {
            if (document.getElementById('contenttype') != null) {
                var iContentType = document.getElementById('contenttype').value;
                if (iContentType == "activecontent")
                    window.location.href = '<%= getURL() %>&contFetchType=activecontent';
                else if (iContentType == "archivedcontent")
                    window.location.href = '<%= getURL() %>&contFetchType=archivedcontent';
                else
                    window.location.href = '<%= getURL() %>';
            }
            else {
                window.location.href = '<%= getURL() %>'
            }
        }
    }
    function searchuser(){
	    if(document.getElementById('txtSearch').value.indexOf('\"') != -1){
	        alert('<%= m_refMsg.GetMessage("js remove all quotes then click search")%>');
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
    // -->
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
        <asp:Label runat="server" ID="PageLabel" ToolTip="Page">Page</asp:Label>
        <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
        <asp:Label runat="server" ID="OfLabel" ToolTip="of">of</asp:Label>
        <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
    </p>
    <asp:LinkButton runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
        OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
    <asp:LinkButton runat="server" CssClass="pageLinks" ID="PreviousPage" Text="[Previous Page]"
        OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
    <asp:LinkButton runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
        OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
    <asp:LinkButton runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
        OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
    <div runat="server" id="localization_folder_chkRecursive_div" visible="false">
      <input type="checkbox" checked="checked" class="localization_folder_chkRecursive" name="foldercheckRecursive" onclick="updateSubfoldersOnRecursiveChange(this)" id="localization_folder_chkRecursive" /> <label for="localization_folder_chkRecursive">Include Subfolders</label><br />
  </div>
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
<input type="hidden" id="ApplicationPathLocale" name="ApplicationPathLocale" runat="server" class="ApplicationPathLocale"  />
<input type="hidden" id="folderName" name="folderName" />
<input type="hidden" id="newFolderDescendantsCSV" name="newFolderDescendantsCSV" value="" />
<input type="hidden" id="newFolderChildrenCSV" name="newFolderChildrenCSV" value="" />
<input type="hidden" id="selected_folder_id" name="selected_folder_id" value="0" />

<script type="text/javascript">
<!--
    var vFolderName = 'Folder';
    var ApplicationPath = $ektron(".ApplicationPathLocale");
    var ApplicationPathVar = "";
    // debugger;
    if (ApplicationPath.length > 0) {
        ApplicationPathVar = ApplicationPath[0].value;
    }
    var pcfarray = new Array(10);
    pcfarray[0] = ApplicationPathVar + "images/ui/icons/tree/folderCollapsed.png";
    pcfarray[1] = ApplicationPathVar + "images/ui/icons/tree/folderBlogCollapsed.png";
    pcfarray[2] = ApplicationPathVar + "images/ui/icons/tree/folderSiteCollapsed.png";
    pcfarray[3] = ApplicationPathVar + "images/ui/icons/tree/folderBoardCollapsed.png";
    pcfarray[4] = ApplicationPathVar + "images/ui/icons/tree/folderBoardCollapsed.png";
    pcfarray[5] = ApplicationPathVar + "images/ui/icons/tree/home.png";
    pcfarray[6] = ApplicationPathVar + "images/ui/icons/tree/folderCommunityCollapsed.png";
    pcfarray[7] = ApplicationPathVar + "images/ui/icons/tree/folderFilmCollapsed.png";
    pcfarray[8] = ApplicationPathVar + "images/ui/icons/tree/folderCalendarCollapsed.png"; // calendar
    pcfarray[9] = ApplicationPathVar + "images/ui/icons/tree/folderGreenCollapsed.png";
    TreeDisplayUtil.plusclosefolders = pcfarray;
    var mcfarray = new Array(10);
    mcfarray[0] = ApplicationPathVar + "images/ui/icons/tree/folderExpanded.png";
    mcfarray[1] = ApplicationPathVar + "images/ui/icons/tree/folderBlogExpanded.png";
    mcfarray[2] = ApplicationPathVar + "images/ui/icons/tree/folderSiteExpanded.png";
    mcfarray[3] = ApplicationPathVar + "images/ui/icons/tree/folderBoardExpanded.png";
    mcfarray[4] = ApplicationPathVar + "images/ui/icons/tree/folderBoardExpanded.png";
    mcfarray[5] = ApplicationPathVar + "images/ui/icons/tree/home.png";
    mcfarray[6] = ApplicationPathVar + "images/ui/icons/tree/folderCommunityExpanded.png";
    mcfarray[7] = ApplicationPathVar + "images/ui/icons/tree/folderFilmExpanded.png";
    mcfarray[8] = ApplicationPathVar + "images/ui/icons/tree/folderCalendarExpanded.png"; // calendar
    mcfarray[9] = ApplicationPathVar + "images/ui/icons/tree/folderGreenExpanded.png";
    TreeDisplayUtil.minusclosefolders = mcfarray;
    var farray = new Array(10);
    farray[0] = ApplicationPathVar + "images/ui/icons/tree/folder.png";
    farray[1] = ApplicationPathVar + "images/ui/icons/tree/folderBlog.png";
    farray[2] = ApplicationPathVar + "images/ui/icons/tree/folderSite.png";
    farray[3] = ApplicationPathVar + "images/ui/icons/tree/folderBoard.png";
    farray[4] = ApplicationPathVar + "images/ui/icons/tree/folderBoard.png";
    farray[5] = ApplicationPathVar + "images/ui/icons/tree/home.png";
    farray[6] = ApplicationPathVar + "images/ui/icons/tree/folderCommunity.png";
    farray[7] = ApplicationPathVar + "images/ui/icons/tree/folderFilm.png";
    farray[8] = ApplicationPathVar + "images/ui/icons/tree/folderCalendar.png"; // calendar
    farray[9] = ApplicationPathVar + "images/ui/icons/tree/folderGreen.png";
    TreeDisplayUtil.folders = farray;

    var clickedElementPrevious = null;
    var clickedIdPrevious = null;

    var g_selectedFolders = [];
    var g_prevFolderDescendants = [];
    var g_prevFolderChildren = [];
    var g_newFolderDescendants = [];
    var g_newFolderChildren = [];

    var g_selectedFoldersCSV = "<%=m_selectedFoldersCSV%>";
    var g_prevFolderDescendantsCSV = "<%=m_prevFolderDescendantsCSV%>";
    var g_prevFolderChildrenCSV = "<%=m_prevFolderChildrenCSV%>";
    
    if (g_selectedFoldersCSV.length > 0)
    {
        g_selectedFolders = g_selectedFoldersCSV.split(",");
    }
    
    if (g_prevFolderDescendantsCSV.length > 0)
    {
        g_prevFolderDescendants = g_prevFolderDescendantsCSV.split(",");
    }

    if (g_prevFolderChildrenCSV.length > 0)
    {
        g_prevFolderChildren = g_prevFolderChildrenCSV.split(",");
    }

    // extension method for array
    function Array_removeFolder(folderId)
    {
        for (var i = 0; i < this.length; i++)
        {
            if (this[i] == folderId)
            {
                this.splice(i, 1);
                return true;
            }
        }

        return false;
    }

    g_selectedFolders.removeFolder = Array_removeFolder;
    g_newFolderDescendants.removeFolder = Array_removeFolder;
    g_newFolderChildren.removeFolder = Array_removeFolder;

    function updateSubfolderCheckbox(checkbox, isParentChecked)
    {
        var childId = checkbox.value;
        if (isParentChecked)
        {
            if (g_newFolderDescendants.removeFolder(childId))
            {
                g_selectedFolders.removeFolder(childId);
            }

            if (g_newFolderChildren.removeFolder(childId))
            {
                g_selectedFolders.removeFolder(childId);
            }
            
            checkbox.checked = true;
            checkbox.disabled = true;
        }
        else if (TreeDisplayUtil.isFolderEnabled(childId))
        {
            checkbox.checked = false;
            checkbox.disabled = false;
        }
    }

    // also in taxonomy/assigntaxonomy.ascx
    function IsAlreadySelected(folderId) 
    {
        for (var i = 0; i < g_selectedFolders.length; i++) 
        {
            if (g_selectedFolders[i] == folderId) 
            {
                return true;
            }
        }
        
        return false;
    }

    // onclick handler for checkboxes, see com.ektron.ui.assignfolder.js
    function UpdateSelectedValue(checkbox) 
    {
        var folderId = checkbox.value;
        var isFolderChecked = checkbox.checked;
        if (isFolderChecked) 
        {
            var folderCheckRecursive = $ektron("input.localization_folder_chkRecursive:checked");
            var isIncludeSubfoldersChecked = (folderCheckRecursive.length > 0);
            if (isIncludeSubfoldersChecked)
            {
                g_newFolderDescendants.push(folderId);
            }
            else
            {
                g_newFolderChildren.push(folderId);
            }
            
            g_selectedFolders.push(folderId);
        }
        else
        {
            g_newFolderDescendants.removeFolder(folderId);
            g_newFolderChildren.removeFolder(folderId);
            g_selectedFolders.removeFolder(folderId);
        }

        document.getElementById("newFolderDescendantsCSV").value = g_newFolderDescendants.join(",");
        document.getElementById("newFolderChildrenCSV").value = g_newFolderChildren.join(",");

        updateSelectedSubfolders(checkbox, isFolderChecked)
    }

    // part of onclick handler for checkboxes, see com.ektron.ui.assignfolder.js
    function updateSelectedSubfolders(checkbox, isFolderChecked)
    {
        var folderId = checkbox.value;
        var foldertree = TreeUtil.getTreeById(folderId);

        //get if the Include subfolders checkbox is checked or not.
        var folderCheckRecursive = $ektron("input.localization_folder_chkRecursive:checked");
        var isIncludeSubfoldersChecked = (folderCheckRecursive.length > 0);

        //if include subfolders checkbox is checked.
        if (isIncludeSubfoldersChecked)
        {
            //expand the tree just to check Checkboxes and we will hide them after all the checkboxes for the subfolders are checked.
            if ((foldertree.node.data.hasChildren == "true") || (foldertree.node.id == 0))
            {
                var numberChildrenFolders = foldertree.children.length;
                //loop through all the subfolders of the Checked Folder and check them.
                for (var i = 0; i < numberChildrenFolders; i++)
                {
                    //get the checkbox with Child Id Value.
                    var descendantCheckboxes = $ektron("#T" + foldertree.children[i].node.id + " :checkbox");
                    descendantCheckboxes.each(function()
                    {
                        updateSubfolderCheckbox(this, isFolderChecked);
                    });
                }
            }
        }
    }

    //added by Rama krishna Ila..
    function updateSubfoldersOnRecursiveChange(checkbox)
    {
        var newSelectedFolderCheckboxes = $ektron("#T0 :checkbox:checked:enabled");
        if (newSelectedFolderCheckboxes.length > 0)
        {
            newSelectedFolderCheckboxes.each(function()
            {
                // if sender.checked, check and disable all the descendants
                // otherwise, enable all the descendants and uncheck all the descendants that have not been selected individually
                var folderId = this.value;
                var descendantCheckboxes = $ektron("#C" + folderId + " :checkbox");
                descendantCheckboxes.each(function()
                {
                    updateSubfolderCheckbox(this, checkbox.checked);
                });

                if (checkbox.checked)
                {
                    // Change from FolderChildren to FolderDescendants
                    if (g_newFolderChildren.removeFolder(folderId))
                    {
                        g_newFolderDescendants.push(folderId);
                    }
                }
                else
                {
                    // Change from FolderDescendants to FolderChildren
                    if (g_newFolderDescendants.removeFolder(folderId))
                    {
                        g_newFolderChildren.push(folderId);
                    }
                }
            });

            document.getElementById("newFolderDescendantsCSV").value = g_newFolderDescendants.join(",");
            document.getElementById("newFolderChildrenCSV").value = g_newFolderChildren.join(",");
        }
    }

    function onContextMenuHandler(id, clickedElement) { return false; }

    function onFolderClickEx(id, clickedElement) {
        if (clickedElementPrevious != null) {
            clickedElementPrevious.style["background"] = "#ffffff";
            clickedElementPrevious.style["color"] = "#000000";
        }

        clickedElement.style["background"] = "#3366CC";
        clickedElement.style["color"] = "#ffffff";
        clickedElementPrevious = clickedElement;
        clickedIdPrevious = id;

        var folderName = clickedElement.innerText;
        var folderId = id;

        document.getElementById("folderName").value = folderName;
        document.getElementById("selected_folder_id").value = id;

        returnValue = new Folder(folderName, folderId);
    }

    function Folder(name, id) {
        this.name = name;
        this.id = id;
    }

    function onToggleClick(id, callback, args) {
        //var callbackstr = callback.toString();
        toolkit.getChildFolders(id, -1, callback, args);
    }

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

        if (g_selectedFolders.length > 0) 
        {
            var tree = TreeUtil.getTreeById(0);
            if (tree) 
            {
                var lastId = 0;
                bSuccessFlag = TreeDisplayUtil.expandTreeSet(g_selectedFolders);
            }

            if (!bSuccessFlag) 
            {
                g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
            }
            else 
            {
                var idValStr = g_selectedFolders[g_selectedFolders.length - 1];
                var idVal = parseInt(idValStr, 10);
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

    function displayTreeFolderSelect() {
        toolkit.getRootFolder(function (folderRoot) {
            document.body.style.cursor = "default";
            if (vFolderName != null) {
                treeRoot = new Tree(vFolderName, 0, null, folderRoot);
                TreeDisplayUtil.showSelf(treeRoot);
                TreeDisplayUtil.toggleTree(treeRoot.node.id);

            } else {
                var element = document.getElementById("TreeOutput");
                element.style["padding"] = "10pt";
                var debugInfo = "<b>Cannot connect to the CMS server</b> "
                element.innerHTML = debugInfo;
            }
            Explorer.onLoadExplorePanel();
        }, 0);

    }

    function isBrowserFireFox() {
        return (top.IsBrowserFF && top.IsBrowserFF());
    }

    function setupClassNames() {
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
    // -->
</script>

<% } %>