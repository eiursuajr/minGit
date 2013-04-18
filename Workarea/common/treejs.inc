<link   type='text/css' rel='stylesheet' href='Tree/css/com.ektron.ui.tree.css' />
<script type="text/javascript">
	var taxonomytreemode="editor";var ____ek_appPath2="";
</script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.url.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.explorer.init.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.explorer.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.explorer.config.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.explorer.windows.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.cms.types.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.cms.parser.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.cms.toolkit.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.cms.api.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.ui.contextmenu.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.ui.iconlist.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.ui.tabs.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.ui.explore.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.ui.taxonomytree.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.net.http.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.lang.exception.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.form.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.log.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.dom.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.debug.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.string.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.cookie.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.querystring.js"></script>
<script type="text/javascript">
var clickedElementPrevious = null;
var clickedIdPrevious = null;

function onDragEnterHandler( id, element ){
  folderID = id;
  if( clickedElementPrevious != null ) {
    clickedElementPrevious.style["background"] = "#ffffff";
    clickedElementPrevious.style["color"] = "#000000";
  }
  element.style["background"] = "#3366CC";
  element.style["color"] = "#ffffff";
}

function onMouseOverHandler( id, element ){
  element.style["background"] = "#ffffff";
  element.style["color"] = "#000000";
}

function onDragLeaveHandler( id, element ) {
  element.style["background"] = "#ffffff";
  element.style["color"] = "#000000";
}

function onFolderClick( id, clickedElement ){
  var tree = null;
  var visible = "";
  if (id > 0)
  {
    tree = TreeUtil.getTreeById(id);
  }
  if (tree)
  {
    if(tree.node){
      if(tree.node.data){
        visible = tree.node.data.visible;
      }
    }
  }
  if( clickedElementPrevious != null ) {
  var previousTree = null;
  var previousVisible = "";
  if (clickedElementPrevious.id > 0)
    previousTree = TreeUtil.getTreeById(clickedElementPrevious.id);
  if (previousTree){
    if(previousTree.node){
      if( previousTree.node.data){
        previousVisible = previousTree.node.data.visible;}
      }
    }
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
    clickedElement.style["color"] = "#ffffff";
  }
  clickedElementPrevious = clickedElement;
  clickedIdPrevious = id;

  var name = clickedElement.innerText;
  var folder = new Asset();
  folder.set( "name", name );
  folder.set( "id", id );
  folder.set("folderid",__EkFolderId);
  __EkFolderId=-1;
}

function onToggleClick( id, callback, args ){
  toolkit.getAllSubCategory( id, -99, callback, args );
}

function makeElementEditable( element ) {
  element.contentEditable = true;
  element.focus();
  element.style.background = "#fff";
  element.style.color = "#000";
}

var baseUrl = URLUtil.getAppRoot(document.location) + "images/ui/icons/tree/";
TreeDisplayUtil.plusclosefolder  = baseUrl + "taxonomyCollapsed.png";
TreeDisplayUtil.plusopenfolder   = baseUrl + "taxonomyCollapsed.png";
TreeDisplayUtil.minusclosefolder = baseUrl + "taxonomyExpanded.png";
TreeDisplayUtil.minusopenfolder  = baseUrl + "taxonomyExpanded.png";
TreeDisplayUtil.folder = baseUrl + "taxonomy.png";

var g_menu_id = "";
function displayCategory( categoryRoot ) {
  document.body.style.cursor = "default";
  var taxonomyTitle = null;
  try {
    taxonomyTitle = categoryRoot.title;
    g_menu_id = categoryRoot.id;
  } catch( e ) {
  ;
  }

  if( taxonomyTitle != null ) {
    treeRoot = new Tree( taxonomyTitle, __TaxonomyOverrideId, null, categoryRoot, 0 );
    TreeDisplayUtil.showSelf( treeRoot, document.getElementById( "TreeOutput" ) );
    TreeDisplayUtil.toggleTree( treeRoot.node.id );
  } else {
    var element = document.getElementById( "TreeOutput" );
    var debugInfo = "<b>Cannot connect to the service</b>";
    element.innerHTML = debugInfo;
  }
}

var toolkit = new EktronToolkit();
toolkit.getTaxonomy( __TaxonomyOverrideId, -99, displayCategory, __TaxonomyOverrideId );

function reloadTreeRoot( id ){
  TREES = {};
  toolkit.getTaxonomy( id, -99, displayCategory, __TaxonomyOverrideId );
}

var g_selectedFolderList = "0";
var g_timerForFolderTreeDisplay;
function showSelectedFolderTree(){
  if (g_timerForFolderTreeDisplay){
    window.clearTimeout(g_timerForFolderTreeDisplay);
  }
  g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
}

function showSelectedFolderTree_delayed() {
  var bSuccessFlag = false;
  if (g_timerForFolderTreeDisplay){
    window.clearTimeout(g_timerForFolderTreeDisplay);
  }

  if (g_selectedFolderList.length > 0){
    var tree = TreeUtil.getTreeById(g_menu_id);
    if (tree){
      var lastId = 0;
      var folderList = g_selectedFolderList.split(",");
      bSuccessFlag = TreeDisplayUtil.expandTreeSet( folderList );
    }

    if (!bSuccessFlag){
      g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
    }
  }
}
</script>