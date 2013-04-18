<%@ Control Language="C#" AutoEventWireup="true" CodeFile="taxonomytree.ascx.cs"
    Inherits="taxonomytree" %>
<script type="text/javascript">
  var taxonomytreemode="";var ____ek_appPath2="";
  var __EkFolderId="-1";
  var __TaxonomyOverrideId=0;
</script>

<script type="text/javascript">
    function LoadLanguage(inVal) {
		if(inVal=='0') { return false ; }
		document.location = 'taxonomy.aspx?action=viewtree&taxonomyid=' + "<%=TaxonomyId%>" + '&LangType=' + inVal;
	}

	function TranslateTaxonomy(TaxonomyId, ParentId, LanguageId) {
		document.location = 'taxonomy.aspx?action=add&taxonomyid=' + TaxonomyId + '&LangType='+LanguageId+'&parentid=' + ParentId;
	}
	function DeleteNode(pid){
	    if(pid>0){
	        if(confirm('<asp:Literal id="ltr_confrmDelTax" runat="server" />')){
                document.getElementById("submittedaction").value="deletenode";
                document.forms[0].submit();
            }
	    }
	    else
	    {
            if(confirm('<asp:Literal id="ltr_alrtDelTax" runat="server" />')){
                document.getElementById("submittedaction").value="deletenode";
                document.forms[0].submit();
            }
        }
        return false;
    }
    function EnableDisableNode(action)
    {
       if(action=="enable")
          document.getElementById("submittedaction").value= "enable";
       else
          document.getElementById("submittedaction").value = "disable";
       document.forms[0].submit();
       return false;
    }
</script>

<div id="FrameContainer" style="position: absolute; top: 25px; left: 2px; width: 1px;
    height: 1px; display: none; z-index: 1000;">
    <iframe id="ChildPage" frameborder="1" marginheight="0" marginwidth="0" width="100%"
        height="100%" scrolling="auto" style="background-color: white;">
    </iframe>
</div>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <table>
        <tr>
            <td style="position: relative;">
                <div style="position: absolute;" id='TreeOutput'>
                </div>
            </td>
        </tr>
</table>
</div>

<script type="text/javascript">

//////////
//
// The click handlers for the trees. These should be placed
// in an external file, I'm just throwing them in here for now,
// since we're only using the tree in once place.
//

var clickedElementPrevious = null;
var clickedIdPrevious = null;

function onDragEnterHandler( id, element )
{
	folderID = id;

	// todo: create a 'highlight node' function
	if( clickedElementPrevious != null ) {
		clickedElementPrevious.style["background"] = "#ffffff";
		clickedElementPrevious.style["color"] = "#000000";
	}

	element.style["background"] = "#3366CC";
	element.style["color"] = "#ffffff";
}

function onMouseOverHandler( id, element )
{
	element.style["background"] = "#ffffff";
	element.style["color"] = "#000000";
}

function onDragLeaveHandler( id, element )
{
	element.style["background"] = "#ffffff";
	element.style["color"] = "#000000";
}

function onFolderClick( id, clickedElement )
{
    var tree = null;
    var visible = "";
    if (id > 0)
	   {
		 tree = TreeUtil.getTreeById(id);
	   }
	if (tree && tree.node && tree.node.data)
	   {
		 visible = tree.node.data.visible;
	   }
	// todo: create a 'highlight node' function
	if( clickedElementPrevious != null ) {
		//if( clickedIdPrevious == id ) {
		//	return;
		//}
		var previousTree = null;
		var previousVisible = "";
		 if (clickedElementPrevious.id > 0)
	   		previousTree = TreeUtil.getTreeById(clickedElementPrevious.id);
	    if (previousTree && previousTree.node && previousTree.node.data)
	       previousVisible = previousTree.node.data.visible;
	    if(previousVisible != "false"){
		    clickedElementPrevious.style["background"] = "#ffffff";
		    clickedElementPrevious.style["color"] = "#000000";
		    }
		    else{
	            clickedElementPrevious.style["background"] = "#808080";
	           clickedElementPrevious.style["color"] = "#000000";
	       }
	}
    if(visible != "false"){
	    clickedElement.style["background"] = "#3366CC";
	    clickedElement.style["color"] = "#ffffff";
	}
	else{
	   clickedElement.style["background"] = "#808080";
	   clickedElement.style["color"] = "#000000";
	}
	clickedElementPrevious = clickedElement;
	clickedIdPrevious = id;

	var name = clickedElement.innerText;
	var folder = new Asset();
	folder.set( "name", name );
	folder.set( "id", id );
}

function onToggleClick( id, callback, args )
{
	toolkit.getAllSubCategory( id, -99, callback, args );
}

function makeElementEditable( element )
{
	element.contentEditable = true;
	element.focus();
	element.style.background = "#fff";
	element.style.color = "#000";
}

//////////
//
// Define the default images for the tree
//

var baseUrl = URLUtil.getAppRoot(document.location) + "images/ui/icons/tree/";
TreeDisplayUtil.plusclosefolder  = baseUrl + "taxonomyCollapsed.png";
TreeDisplayUtil.plusopenfolder   = baseUrl + "taxonomyCollapsed.png";
TreeDisplayUtil.minusclosefolder = baseUrl + "taxonomyExpanded.png";
TreeDisplayUtil.minusopenfolder  = baseUrl + "taxonomyExpanded.png";
TreeDisplayUtil.folder = baseUrl + "taxonomy.png";

var g_menu_id = "";
function displayCategory( categoryRoot )
{
	document.body.style.cursor = "default";
	var taxonomyTitle = null;
	try {
		taxonomyTitle = categoryRoot.title;
		g_menu_id = categoryRoot.id;
	} catch( e ) {
		;
	}

	if( taxonomyTitle != null ) {
		// Start with the root of the menu, via the AncestorTaxonomyId:
		treeRoot = new Tree( taxonomyTitle, <%=AncestorTaxonomyId%>, null, categoryRoot );
		TreeDisplayUtil.showSelf( treeRoot, document.getElementById( "TreeOutput" ) );
		if(treeRoot.node.data.visible == "false")
		{
		  markDisabledMenuObject(true, treeRoot.node.id);
		}
		TreeDisplayUtil.toggleTree( treeRoot.node.id );
	} else {
		var element = document.getElementById( "TreeOutput" );
		var debugInfo = "<b>Cannot connect to the service</b>";
		element.innerHTML = debugInfo;
	}
}
var toolkit = new EktronToolkit();
// Start with the root of the menu, via the AncestorTaxonomyId:
toolkit.getTaxonomy( <%=AncestorTaxonomyId%>, -99, displayCategory, 0 );

function reloadTreeRoot( id )
{
	// clear out existing tree
	TREES = {};
	toolkit.getTaxonomy( id, -99, displayCategory, 0 );
}

var g_selectedFolderList = "<%=m_selectedTaxonomyList%>";
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
			bSuccessFlag = TreeDisplayUtil.expandTreeSet( folderList );
		}

		if (!bSuccessFlag)
		{
			g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
		}
	}
}

</script>

<script type="text/javascript">
function LoadChildPage(Action, TaxonomyId, ParentId) {
	var frameObj = document.getElementById("ChildPage");
	var lastClickedOn = document.getElementById("LastClickedOn");
	lastClickedOn.value= TaxonomyId;
	document.getElementById("LastClickedParent").value=ParentId;
	document.getElementById("LastClickedOnItemCount").value=document.getElementById("TIC"+TaxonomyId).innerHTML;
	if(parseInt(ParentId)==0){document.getElementById("ClickRootCategory").value="true";}
	switch (Action){
		case "add":
		    if (TaxonomyId == "") {
		        alert("Please select a taxonomy.");
		        return false;
	        }
			frameObj.src = "blankredirect.aspx?taxonomy.aspx?iframe=true&action=add&parentid="+TaxonomyId ;
			break;
		case "view":
			frameObj.src = "blankredirect.aspx?taxonomy.aspx?iframe=true&action=view&view=item&taxonomyid=" + TaxonomyId + '&parentid='+ParentId ;
			break;
		case "edit":
			frameObj.src = "blankredirect.aspx?taxonomy.aspx?iframe=true&action=edit&taxonomyid=" + TaxonomyId + '&parentid='+ParentId ;
			break;
		case "delete":
		    DeleteNode(ParentId);
			break;
		case "additem":
			frameObj.src = "blankredirect.aspx?taxonomy.aspx?iframe=true&action=additem&folderid=0&taxonomyid=" + TaxonomyId + '&parentid='+ParentId ;
			break;
        case "addfolder":
			frameObj.src = "blankredirect.aspx?taxonomy.aspx?iframe=true&action=addfolder&folderid=0&taxonomyid=" + TaxonomyId + '&parentid='+ParentId ;
			break;
		case "reorder":
			frameObj.src = "blankredirect.aspx?taxonomy.aspx?iframe=true&action=reorder&reorder=category&folderid=0&taxonomyid=" + TaxonomyId + '&parentid='+ParentId ;
			break;
		case "enable":
		  $ektron('#divEnable').modal({
                trigger: '',
                modal: true
	        });
	      $ektron("#divEnable").modalShow();
		    break;
		case "disable":
		$ektron('#divDisable').modal({
                trigger: '',
                modal: true
	        });
	      $ektron("#divDisable").modalShow();
		    break;
		default :
			break;
	}
	if(Action!="delete"){
	    DisplayIframe();
	}
}
function DisplayIframe(){
    var pageObj = document.getElementById("FrameContainer");
	pageObj.style.display = "";
    if(navigator.userAgent.indexOf("MSIE 6.0") > -1){
        pageObj.style.width = "100%";
		pageObj.style.height = "520px";
    }
    else{
        pageObj.style.width = "95%";
        pageObj.style.height = "95%";
    }
}
function CancelIframe(){
	var pageObj = document.getElementById("FrameContainer");
	pageObj.style.display = "none";
	pageObj.style.width = "1px";
	pageObj.style.height = "1px";
}
function CloseChildPage(){
    CancelIframe();
	var ClickRootCategory = document.getElementById("ClickRootCategory");
	if (ClickRootCategory.value == "true"){
	    window.location.reload(true);
	}
	else
	{
	    var lastClickedOn = document.getElementById("LastClickedOn");
	    var clickType = document.getElementById("ClickType");
	    TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
	    var node = document.getElementById("T" + lastClickedOn.value );
        if(node!=null){
	        for (var i=0;i<node.childNodes.length;i++){
			    if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) < 9)
			    {
				    if(node.childNodes(i).nodeName=='LI' || node.childNodes(i).nodeName=='UL'){
					    var parent = node.childNodes(i).parentElement;
					    parent.removeChild( node.childNodes(i));
				    }
			    }
			    else
			    {
				    if(node.childNodes[i].nodeName=='LI' || node.childNodes[i].nodeName=='UL'){
					    var parent = node.childNodes[i].parentNode;
					    parent.removeChild( node.childNodes[i]);
				    }
			    }
	        }
	        TREES["T" + lastClickedOn.value].children = [];
	        TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
	        onToggleClick(lastClickedOn.value,TreeUtil.addChildren,lastClickedOn.value);
        }
        window.setTimeout(ValidateCount,1000);
    }
}
function ValidateCount(){
    try{
        var vTaxonomyId=document.getElementById("LastClickedOn").value;
        var vParentId=document.getElementById("LastClickedParent").value;
        var vLastItemCount=document.getElementById("LastClickedOnItemCount").value;
        var vCurrentItemCount=document.getElementById("TIC"+vTaxonomyId).innerHTML;
        var differ=parseInt(vCurrentItemCount)-parseInt(vLastItemCount)
        if(differ!=0){
            document.getElementById("TIC"+vParentId).innerHTML=parseInt(document.getElementById("TIC"+vParentId).innerHTML)+parseInt(differ);
            var treeid=vParentId;
            var node=document.getElementById("T"+vParentId).parentNode;
            var tid="T"+"<%=AncestorTaxonomyId%>";
            var cid="C"+"<%=AncestorTaxonomyId%>";
            while(node.id!=null && node.id!="undefined")
            {
                var currentid=node.parentNode.id;
                var tid1="T"+vParentId;
                var cid1="C"+vParentId;
                if(currentid==tid1 || currentid==cid1){
                node=node.parentNode;
                }
                else{
                vParentId=currentid.replace("T","");vParentId=vParentId.replace("C","");
                document.getElementById("TIC"+vParentId).innerHTML=parseInt(document.getElementById("TIC"+vParentId).innerHTML)+parseInt(differ);
                node=node.parentNode;
                }
                if(currentid==tid || currentid==cid){break;}
            }
        }
    }
    catch(e){}
}
</script>

<script language="javascript" type="text/javascript">
var g_delayedHideTimer = null;
var g_delayedHideTime = 1000;
var g_wamm_float_menu_treeid = -1;
var g_isIeInit = false;
var g_isIeFlag = false;

function IsBrowserIE(){
	if (!g_isIeInit)
	{
		var ua = window.navigator.userAgent.toLowerCase();
		g_isIeFlag = (ua.indexOf('msie') > -1) && (!(ua.indexOf('opera') > -1));
		g_isIeInit = true;
	}
	return (g_isIeFlag);
}

function markDisabledMenuObject(markFlag, id) {
	if (id && (id > 0)) {
		var obj = document.getElementById(id);
		if (obj && obj.className) {
			if (markFlag) {
				if (obj.className.indexOf("disabledLinkStyle_selected") < 0) {
					obj.className += " disabledLinkStyle_selected";
				}
			}
			else {
				if (obj.className.indexOf("disabledLinkStyle_selected") >= 0) {
					obj.className = "linkStyle";
				}
			}
		}
	}
}
function markMenuObject(markFlag, id) {
	if (id && (id > 0)) {
		var obj = document.getElementById(id);
		if (obj && obj.className) {
			if (markFlag) {
				if (obj.className.indexOf("linkStyle_selected") < 0) {
					obj.className += " linkStyle_selected";
				}
			}
			else {
				if (obj.className.indexOf("linkStyle_selected") >= 0) {
					obj.className = "linkStyle";
				}
			}
		}
	}
}
function showWammFloatMenuForMenuNode(show, delay, event, treeId){
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
		    var enable_node = document.getElementById("li_enable");
		    var disable_node = document.getElementById("li_disable");
		    if (visible == "true")
		    {
		   	    enable_node.style.display = "none";
		   	    disable_node.style.display="";
		    }
		    else if (visible == "false")
		    {
		        disable_node.style.display = "none";
		        enable_node.style.display = "";
		    }
			el.style.display = "none";
			if (visible!="false")
			    markMenuObject(false, g_wamm_float_menu_treeid);
			if (null != event)
			{
				el.style.left = (20 + getShiftedEventX(event)) + "px"
				if (IsBrowserIE())
				{
					el.style.top = (48 + getEventY(event)) + "px"
				}
				else
				{
					el.style.top = (getEventY(event)) + "px"
				}
				el.style.display = "";
				if (treeId && (treeId > 0))
				{
					g_wamm_float_menu_treeid = treeId;
					if (visible!="false")
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
				if (visible!="false")
			        markMenuObject(false, g_wamm_float_menu_treeid);
			}
		}
	}
}

function getEventX(event){
	var xVal;
	if (IsBrowserIE())
	{
		xVal = event.x;
	}
	else
	{
		xVal = event.pageX;
	}
	return(xVal)
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
	if (event.srcElement && event.srcElement.offsetLeft){
		srcLeft = event.srcElement.offsetLeft;
	}
	else if (event.target && event.target.offsetLeft){
		srcLeft = event.target.offsetLeft;
	}

	if (event.srcElement) {
		if (event.srcElement.offsetWidth) {
			xVal = srcLeft + event.srcElement.offsetWidth;
		}
		else if (event.srcElement.scrollWidth) {
			xVal = srcLeft + event.srcElement.scrollWidth;
		}
	}
	else if (event.target && event.target.offsetLeft){
		if (event.target.offsetWidth) {
			xVal = srcLeft + event.target.offsetWidth;
		}
		else if (event.target.scrollWidth) {
			xVal = srcLeft + event.target.scrollWidth;
		}
	}

	return(xVal)
}


function getEventY(event){
	var yVal;
	if (IsBrowserIE())
	{
		yVal = event.y;
	}
	else
	{
		yVal = event.pageY;
	}
	return(yVal)
}

 function wamm_float_menu_block_mouseover(obj) {
	if (g_delayedHideTimer)
	{
		clearTimeout(g_delayedHideTimer);
		g_delayedHideTimer = null;
	}
 }

 function wamm_float_menu_block_mouseout(obj) {
	if (null != obj){
		g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuNode(false, false, null, -1)", g_delayedHideTime);

	}
 }

function routeAction(containerFlag, op){
	var tree = null;
	if (g_wamm_float_menu_treeid > 0)
	{
		tree = TreeUtil.getTreeById(g_wamm_float_menu_treeid);
	}

	if (tree && tree.node && tree.node.data)
	{
		var TaxonomyId = tree.node.data.id;
		var ParentId = tree.node.pid;
		if(ParentId==null || ParentId=='undefined'){
		    ParentId=0;
		}

		showWammFloatMenuForMenuNode(false, false, null, -1);
		LoadChildPage(op,TaxonomyId,ParentId);
	}
}

function enableDisableAction(languageFlag, op)
{
   if(languageFlag == true)
     document.getElementById('<%=alllanguages.ClientID%>').value = "true";
   if(op=="enable")
        EnableDisableNode("enable");
   else if (op =="disable")
        EnableDisableNode("disable");
}
</script>

<div id="wamm_float_menu_block_menunode" class="Menu" style="position:absolute; left:10px; top:10px;
    display:none; z-index:3200;" onmouseover="wamm_float_menu_block_mouseover(this)"
    onmouseout="wamm_float_menu_block_mouseout(this)">
    <input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
    <input type="hidden" name="LastClickedParent" id="LastClickedParent" value="" />
    <input type="hidden" name="LastClickedOnItemCount" id="LastClickedOnItemCount" value="" />
    <input type="hidden" name="ClickRootCategory" id="ClickRootCategory" value="false" />    
    <input type="hidden" id="alllanguages" value="" runat="server"  />    
    <ul>
        <li class="MenuItem add">
            <a title="Add Title" href="#" onclick="routeAction(true, 'add');"><%=m_refMsg.GetMessage("generic add title")%></a>
        </li>
        <li class="MenuItem view">
            <a title="View/Edit Items" href="#" onclick="routeAction(true, 'view');"><%=m_refMsg.GetMessage("lbl tree view/edit items")%></a>
        </li>
        <li class="MenuItem reorder">
            <a title="Reorder Items" href="#" onclick="routeAction(true, 'reorder');"><%=m_refMsg.GetMessage("btn reorder")%></a>
        </li>
        <li class="MenuItem editProperties">
            <a title="Edit Properties" href="#" onclick="routeAction(true, 'edit');"><%=m_refMsg.GetMessage("lbl tree edit")%></a>
        </li>
        <li class="MenuItem delete">
            <a title="Delete Title" href="#" onclick="routeAction(true, 'delete');"><%=m_refMsg.GetMessage("generic delete title")%></a>
        </li>
        <li class="MenuItem assignItems">
            <a title="Assign Items" href="#" onclick="routeAction(true, 'additem');"><%=m_refMsg.GetMessage("lbl assign items")%></a>
        </li>
        <li class="MenuItem assignFolders">
            <a title="Assign Folders" href="#" onclick="routeAction(true, 'addfolder');"><%=m_refMsg.GetMessage("lbl assign folders")%></a>
        </li>
        <li class="MenuItem enable" id="li_enable">
            <a title="Enable" href="#" onclick="routeAction(true, 'enable');"><%=m_refMsg.GetMessage("lbl enable")%></a>
        </li>
        <li class="MenuItem disable" id="li_disable">
            <a title="Disable" href="#" onclick="routeAction(true, 'disable');"><%=m_refMsg.GetMessage("lbl disable")%></a>
        </li>
    </ul>
</div>

<div id="divEnable" class="ektronWindow">
   <div class="ektronModalBody" id="bodyEnable">
      <asp:Literal ID="litEnable" runat="server" />
      <ul class="ektronModalButtonWrapper ektronSyncButtons clearFix">
          <li><asp:LinkButton ToolTip="Ok" ID="btnEnYes" Text="OK" OnClientClick="enableDisableAction(true,'enable')" runat="server" CssClass="greenHover button  buttonRight"  /></li>
          <li><asp:LinkButton ToolTip="Cancel" ID="btnEnNo" Text="Cancel" OnClientClick ="enableDisableAction(false,'enable')" runat="server" CssClass="greenHover button  buttonRight" /></li>
      </ul>
  </div>
</div>

<div id="divDisable" class="ektronWindow">
    <div class="ektronModalBody" id="bodyDisable">
        <asp:Literal ID="litDisable" runat="server" />
        <ul class="ektronModalButtonWrapper ektronSyncButtons clearFix">
            <li><asp:LinkButton ToolTip="Ok" ID="btnDisYes" Text="OK" OnClientClick="enableDisableAction(true,'disable')" runat="server" CssClass="greenHover button  buttonRight"  /></li>
            <li><asp:LinkButton ToolTip="Cancel" ID="btnDisNo" Text="Cancel" OnClientClick ="enableDisableAction(false,'disable')" runat="server" CssClass="greenHover button  buttonRight" /></li>
        </ul>
    </div>
</div>

