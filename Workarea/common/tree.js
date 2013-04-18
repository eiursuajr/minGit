var taxonomytreemode="editor";

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
  if( clickedElementPrevious != null ) {
  clickedElementPrevious.style["background"] = "#ffffff";
  clickedElementPrevious.style["color"] = "#000000";
  }

  clickedElement.style["background"] = "#3366CC";
  clickedElement.style["color"] = "#ffffff";
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
  var baseUrl =  "images/ui/icons/tree/";
  try{if(URLUtil.ek_appPath2 !=null)  baseUrl=URLUtil.ek_appPath2 + baseUrl;}catch(e){}
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
