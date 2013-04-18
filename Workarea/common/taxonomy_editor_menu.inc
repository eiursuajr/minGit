<%if (TaxonomyRoleExists){ %>
    <script language="javascript" type="text/javascript">
    var taxonomytreemenu=true;
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
		        el.style.display = "none";
			    if (visible!="false")
					markMenuObject(false, g_wamm_float_menu_treeid);
			    if (null != event)
			    {
			        var hoverElement = $ektron("#" + treeId);
                    var offset = hoverElement.offset();
                    var hoverElementHeight = parseInt(hoverElement.height(), 10);
                    var hoverElementWidth = parseInt(hoverElement.width(), 10)
                    
                    var fixedPositionToolbarFix = 0;
                    if ($ektron("form#LibraryItem").length > 0) {
						fixedPositionToolbarFix = 44;
                    }
                    
                    el.style.top = (parseInt(offset.top, 10) + hoverElementHeight - 5 - fixedPositionToolbarFix) + "px";
	                el.style.left = (parseInt(offset.left, 10) + hoverElementWidth - 5) + "px";
				      
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
	    if (g_delayedHideTimer){
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
	    if (g_wamm_float_menu_treeid > 0){
		    tree = TreeUtil.getTreeById(g_wamm_float_menu_treeid);
	    }

	    if (tree && tree.node && tree.node.data){
		    var TaxonomyId = tree.node.data.id;
		    var ParentId = tree.node.pid;
		    if(ParentId==null || ParentId=='undefined'){
		        ParentId=0;
		    }

		    showWammFloatMenuForMenuNode(false, false, null, -1);
		    LoadChildPage(op,TaxonomyId,ParentId);
	    }
    }
    function LoadChildPage(Action, TaxonomyId, ParentId) {
	    var frameObj = document.getElementById("ChildPage");
	    var lastClickedOn = document.getElementById("LastClickedOn");
	    lastClickedOn.value= TaxonomyId;
	    document.getElementById("LastClickedParent").value=ParentId;
	    if(parseInt(ParentId)==0){document.getElementById("ClickRootCategory").value="true";}
	    else{document.getElementById("ClickRootCategory").value="false";}
	    switch (Action){
		    case "add":
		        if (TaxonomyId == "") {
		            alert("Please select a taxonomy.");
		            return false;
	            }
			    frameObj.src = "blankredirect.aspx?taxonomy.aspx?iframe=true&action=add&parentid="+TaxonomyId ;
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
			pageObj.style.height = "500px";
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
	    var lastClickedOn = document.getElementById("LastClickedOn");
	    var clickType = document.getElementById("ClickType");
		if (ClickRootCategory.value == "true")
		{
            __EkFolderId="<%=m_intTaxFolderId%>";
		}
        else{
            __EkFolderId=-1;
            TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
        }
        var node = document.getElementById("T" + lastClickedOn.value );
        if(node!=null){
            for (var i=0;i<node.childNodes.length;i++){
		        if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) < 9)
		        {
			        if(node.childNodes(i).nodeName == 'LI' || node.childNodes(i).nodeName == 'UL'){
				        var parent = node.childNodes(i).parentElement;
				        parent.removeChild( node.childNodes(i));
			        }
		        }
		        else
		        {
			        if(node.childNodes[i].nodeName == 'LI' || node.childNodes[i].nodeName == 'UL'){
				        var parent = node.childNodes[i].parentNode;
				        parent.removeChild( node.childNodes[i]);
			        }
		        }
            }
            TREES["T" + lastClickedOn.value].children = [];
            TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
            onToggleClick(lastClickedOn.value,TreeUtil.addChildren,lastClickedOn.value);
        }
    }
    </script>
	<% } %>
	<% if ((Page.Request.Url.AbsoluteUri.IndexOf("membership_add_content.aspx") == -1 && Page.Request.Url.ToString().IndexOf("forum=1") == -1)){ %>
    <div id="wamm_float_menu_block_menunode" class="Menu" style="position:absolute; left:10px; top:10px;
        display:none; z-index:3200;" onmouseover="wamm_float_menu_block_mouseover(this)"
        onmouseout="wamm_float_menu_block_mouseout(this)">
        <input type="hidden" name="LastClickedParent" id="LastClickedParent" value="" />
        <input type="hidden" name="ClickRootCategory" id="ClickRootCategory" value="false" />
        <ul>
            <li class="MenuItem add">
                <a href="#" onclick="routeAction(true, 'add');"><%=(m_refMsg.GetMessage("generic add title"))%></a>
            </li>
        </ul>
    </div>
    <% } %>
    <% else { %>
    <script type="text/javascript" >
        var taxonomytreemenu=false;
    </script>
    <% } %>