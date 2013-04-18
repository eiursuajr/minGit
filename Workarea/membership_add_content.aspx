<%@ Page Language="C#" AutoEventWireup="true" CodeFile="membership_add_content.aspx.cs"
    Inherits="membership_add_content" ValidateRequest="false" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="controls/Editor/ContentDesignerWithValidator.ascx" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <title>Add Content</title>
     <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        var inPublishProcess = false;
        //--><!]]>
    </script>
    <asp:literal id="ltr_css" runat="server"></asp:literal>
    <script type="text/javascript">
    <!--//--><![CDATA[//><!--
    var click= false;
    function Close()
    {
    
        if (parent != null && parent != self && typeof parent.ektb_remove == 'function')
        {
            parent.ektb_remove();
        } else {
            self.close();
        }
    }
    function RefreshPage(taxonomyOverriderId, bDynamicBox)
    {
 
        if (bDynamicBox) {
            parent.ektb_remove();
		    parent.location.reload(true);
        } else {
		    if (top.opener.location.href.indexOf("#") != -1)
		    {
		        var loc = top.opener.location.href.split('#');
		        if (taxonomyOverriderId != '') {
		            var tempBuffer = new String( loc[0] );
		            if (tempBuffer.indexOf("__taxonomyid=") > -1)
		            {
		                var startindex = tempBuffer.indexOf("__taxonomyid=");
		                var endindex = tempBuffer.indexOf("&", startindex);
    		            
		                if (endindex == -1)
		                {
		                    endindex = tempBuffer.length;
		                    startindex--;
		                }
		                else
		                    endindex++;
    		            
		                var replaceTerm = tempBuffer.substring(startindex, endindex);
		                tempBuffer = tempBuffer.replace(replaceTerm, "");
		            }
    		        
		            if (tempBuffer.indexOf("?") > -1)
			            top.opener.location.href = tempBuffer + "&__taxonomyid=" +taxonomyOverriderId;
			        else
			            top.opener.location.href = tempBuffer + "?__taxonomyid=" + taxonomyOverriderId;
			            
		           } else {
		            top.opener.location.href = loc[0];
		        }
		    } else {
		        top.opener.location.reload(true);
		    }
		}
       
        
        
    }
    //--><!]]>
    </script>
</head>
<body>
    <form id="form1" name="form1" runat="server">
    <div id="ActionButtons" style="position:absolute; text-align:right; width:100%;top:10px; margin-right:20px;">
        <asp:Button ToolTip="Publish" ID="dialog_publish_top" runat="server" Text="Publish"  OnClick="dialog_publish_top_Click"/>
        <input title="Cancel" id="btnClose" type="button" value="Cancel" onclick="Close()" />
        <input id="hdnSearchText" name="hdnSearchText" type="hidden" /> 
    </div>
        <div class="wrap">
            <input type="hidden" name="content_id" value="0" id="content_id" runat="server" />
            <input type="hidden" runat="server" id="submitasstagingview" name="submitasstagingview"
                value="" />
            <input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
            <input type="hidden" name="LastClickedOnChecked" id="LastClickedOnChecked" value="false" />
            <input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value=""
                runat="server" />
            <asp:Literal ID="ltr_js" runat="Server"></asp:Literal>
            <asp:Label ToolTip="Title" ID="title_label" runat="server" Text="Title: " Font-Bold="True"></asp:Label>
            <asp:TextBox ID="title_value" ToolTip="Enter Title here" runat="server" Width="300px">[Enter Title]</asp:TextBox>
            <asp:CheckBox ToolTip="Auto Generate New Summary" ID="auto_generate_summary" runat="server" Text="Auto Generate New Summary" />
            <asp:Literal ID="help_button" runat="server" />
            <asp:Label ID="change_notification" runat="server" Font-Overline="False" ForeColor="Red"></asp:Label><br />
            <br />
            <div class="selected_editor" id="d_dvContent">
                 <asp:Literal ID="ftb_control" runat="server"></asp:Literal><br />
                 <ektron:ContentDesigner ID="cdContent_teaser" runat="server" AllowScripts="true" Height="500" Width="100%" Visible="true"
                  ShowHtmlMode="false" />
            </div>
            <div class="unselected_editor" id="d_dvCategory">
                <asp:Literal runat="server" ID="EditTaxonomyHtml"></asp:Literal>
                <div style="clear: both;">
                </div>
                <br />
            </div>
            <div id="dvWaitImage">
           
            </div>
            <table width="100%">
                <tr id="tr_pub" runat="server">
                    <td>
                        <asp:Button ToolTip="Click here to Publish" ID="dialog_publish" runat="server" Text="Publish"  OnClick="Button1_Click"/>
                        <input title="Cancel" id="dialog_cancel" type="button" value="Cancel" onclick="Close()" />                   
                    </td>                    
                </tr>                   
                <tr id="tr_asset" runat="server">
                    <td>
                        <asp:Button ToolTip="Publish" ID="dialog_publish_asset" runat="server" Text="Publish"  OnClick="dialog_publish_asset_Click"/>
                        <asp:Literal ID="content_block_view" runat="server" Visible="false" />
                        <CMS:AssetControl ID="content_block_overwrite" runat="server" Visible="false" UploadType="Update" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:Literal ID="UpdateFieldJS" runat="server"></asp:Literal>
        <div id="FrameContainer" style="position: absolute; top: 0px; left: 0px; width: 1px;
            height: 1px; display: none; z-index: 1000;">
            <iframe id="ChildPage" src="javascript:false;" frameborder="1" marginheight="0" marginwidth="0" width="100%"
                height="100%" scrolling="auto" style="background-color: white;">
            </iframe>
        </div>
    </form>

    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
    
        <asp:literal id="js_taxon" runat="server" />
    // var taxonomytreearr="".split(",");
    // var taxonomytreedisablearr="".split(",");
    function fetchtaxonomyid(pid){
        for(var i=0;i<taxonomytreearr.length;i++){
            if(taxonomytreearr[i]==pid){
                return true;
                break;
            }
        }
        return false;
    }
     function fetchdisabletaxonomyid(pid){
        for(var i=0;i<taxonomytreedisablearr.length;i++){
            if(taxonomytreedisablearr[i]==pid){
                return true;
                break;
            }
        }
        return false;
    }
    function updatetreearr(pid,op){
        if(op=="remove"){
            for(var i=0;i<taxonomytreearr.length;i++){
                if(taxonomytreearr[i]==pid){
                    taxonomytreearr.splice(i,1);break;
                }
            }
        }
        else{
            taxonomytreearr.splice(0,0,pid);
        }
        
        document.getElementById("taxonomyselectedtree").value="";
        for(var i=0;i<taxonomytreearr.length;i++){
            if(document.getElementById("taxonomyselectedtree").value==""){
                document.getElementById("taxonomyselectedtree").value=taxonomytreearr[i];
            }else{
                document.getElementById("taxonomyselectedtree").value=document.getElementById("taxonomyselectedtree").value+","+taxonomytreearr[i];
            }
        }
    }
//    function selecttaxonomy(control){
//        var pid=control.value;
//        if(control.checked){
//            updatetreearr(pid,"add");
//        }else{
//            updatetreearr(pid,"remove");
//        }
//        var currval=eval(document.getElementById("chkTree_T"+pid).value);
//        var node = document.getElementById( "T" + pid );
//        var newvalue=!currval;
//        document.getElementById("chkTree_T"+pid).value=eval(newvalue);
//        Traverse(node,newvalue);
//    }
   function selecttaxonomy(control){
        var pid=control.value;
        if(control.checked)
        {
            updatetreearr(pid,"add");
        }
        else
        {
            updatetreearr(pid,"remove");
        }
        var currval=eval(document.getElementById("chkTree_T"+pid).value);
        var node = document.getElementById( "T" + pid );
        var newvalue=!currval;
        document.getElementById("chkTree_T"+pid).value=eval(newvalue);
        if(control.checked)
          {
            Traverse(node,true);
          }
        else
          {
            Traverse(node,false);
            var hasSibling = false;
            if (taxonomytreearr != "")
              { for(var i = 0 ;i<taxonomytreearr.length;i++)
                    {
                      if(taxonomytreearr[i] != "")
                        {
                          var newnode = document.getElementById( "T" + taxonomytreearr[i]);
                            if(newnode != null && newnode.parentNode == node.parentNode)
                               {Traverse(node,true);hasSibling=true;break;}
                        }
                    }
              }
            if(hasSibling == false)
            { 
                checkParent(node);
            }  
          }
    }
   
    function checkParent(node)
    { if(node!= null)
        {
              var subnode = node.parentNode;
              if(subnode!=null && subnode.id!="T0" &&  subnode.id!="")
              {
                for(var j=0;j<subnode.childNodes.length;j++)
                    {
                           var pid=subnode.childNodes[j].id;
                           if(document.getElementById("chkTree_"+pid).value == true || document.getElementById("chkTree_"+pid).value == "true")
                              {Traverse(subnode.childNodes[j],true);return;}
                          }
               checkParent(subnode.parentNode);
              }
        }
    }
    function Traverse(node,newvalue){
        if(node!=null){
            subnode=node.parentElement;
            if(subnode!=null && subnode.id!="T0" &&  subnode.id!=""){
                for(var j=0;j<subnode.childNodes.length;j++){
                    var n=subnode.childNodes[j]
                    if(n.nodeName=="INPUT" && n.attributes["type"].value=="checkbox"){
                        var pid=subnode.id;
                        updatetreearr(pid.replace("T",""),"remove");
                        document.getElementById("chkTree_"+pid).value=eval(newvalue);
                        n.setAttribute("checked",eval(newvalue));
                        n.setAttribute("disabled",eval(newvalue));
                        
                    }
                }
                if(HasChildren(subnode) && subnode.getAttribute("checked")){
                       subnode.setAttribute("checked",true);
                        subnode.setAttribute("disabled",true);  
                }
                Traverse(subnode,newvalue);
            }
        }
    }
    function HasChildren(subnode)
    {
        if(subnode!=null){
            for(var j=0;j<subnode.childNodes.length;j++)
            {
                for(var j=0;j<subnode.childNodes.length;j++){
                    var n=subnode.childNodes[j]
                    if(n.nodeName=="INPUT" && n.attributes["type"].value=="checkbox"){
                        var pid=subnode.id;
                        var v=document.getElementById("chkTree_"+pid).value;
                        if(v==true || v=="true"){
                        return true;break;
                        }
                    }
                }
            }
        }
        return false;
    }
    
    function SaveCategory()
    {
        var selected_nodes = document.getElementById('taxonomyselectedtree');
        var target = parent.document.getElementById('ekcategoryselection');
        if( target != null ) {
            target.value = selected_nodes.value;
        }
        parent.CloseCategorySelect(false);
    }
    
    function Wait(bool)
    {    
        if (bool)
        {
            ShowPane('dvWaitImage');
            document.getElementById("dvWaitImage").innerHTML = '<img src="images/application/loading_big.gif" alt="Please wait..." />';
            document.getElementById("dialog_publish").style.visibility = 'hidden';
            document.getElementById("dialog_cancel").style.visibility = 'hidden';
            
        } else {
            document.getElementById("dvWaitImage").innerHTML = '';
            document.getElementById("dialog_publish").style.visibility = 'visible';
            document.getElementById("dialog_cancel").style.visibility = 'visible';
            ShowPane('dvContent');
        }
        inPublishProcess = bool;
    }
    

        var IsForum = <%=IsForum.ToString().ToLower()%>;
        if( IsForum ) {
            var dvContent = document.getElementById('dvContent');
            dvContent.style.display = 'none';
            ShowPane('dvCategory');
            
            var cancel_button = document.getElementById('dialog_cancel');
            if( cancel_button != null ) {
                cancel_button.style.display = 'none';
            }
            
            var dvCategory = document.getElementById('dvCategory');
            if( dvCategory != null ) {
                dvCategory.style.display = 'none';
            }
            
            var ekcategoryselection = top.document.getElementById('ekcategoryselection');
            if( ekcategoryselection != null ) { 
                ekcategoryselection.value = taxonomytreearr;
            }
            $ektron(".ektronPageHeader").css("display", "none");
        }
        //--><!]]>
    </script>
    
     <%if(TaxonomyRoleExists) {%>
    <script language="javascript" type="text/javascript">
        var taxonomytreemenu = true;
        var g_delayedHideTimer = null;
        var g_delayedHideTime = 1000;
        var g_wamm_float_menu_treeid = -1;
        var g_isIeInit = false;
        var g_isIeFlag = false;

        function IsBrowserIE() {
            if (!g_isIeInit) {
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

        function showWammFloatMenuForMenuNode(show, delay, event, treeId) {
            var el = document.getElementById("wamm_float_menu_block_menunode");
            var visible = "";
            if (el) {
                if (g_delayedHideTimer) {
                    clearTimeout(g_delayedHideTimer);
                    g_delayedHideTimer = null;
                }
                var tree = null;
                if (treeId > 0) {
                    tree = TreeUtil.getTreeById(treeId);
                }
                if (tree && tree.node && tree.node.data) {
                    visible = tree.node.data.visible;
                }
                if (show) {
                    el.style.display = "none";
                    if (visible != "false")
                        markMenuObject(false, g_wamm_float_menu_treeid);
                    if (null != event) {
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
                        if (treeId && (treeId > 0)) {
                            g_wamm_float_menu_treeid = treeId;
                            if (visible != "false")
                                markMenuObject(true, treeId);
                        }
                        else {
                            g_wamm_float_menu_treeid = -1;
                        }
                    }
                }
                else {
                    if (delay) {
                        g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuNode(false, false, null, -1)", g_delayedHideTime);
                    }
                    else {
                        el.style.display = "none";
                        if (visible != "false")
                            markMenuObject(false, g_wamm_float_menu_treeid);
                    }
                }
            }
        }

        function getEventX(event) {
            var xVal;
            if (IsBrowserIE()) {
                xVal = event.x;
            }
            else {
                xVal = event.pageX;
            }
            return (xVal)
        }

        function getShiftedEventX(event) {
            var srcLeft;
            var xVal;
            if (IsBrowserIE()) {
                xVal = event.x;
            }
            else {
                xVal = event.pageX;
            }

            // attempt to shift div-tag to the right of the menu items:
            srcLeft = xVal;
            if (event.srcElement && event.srcElement.offsetLeft) {
                srcLeft = event.srcElement.offsetLeft;
            }
            else if (event.target && event.target.offsetLeft) {
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
            else if (event.target && event.target.offsetLeft) {
                if (event.target.offsetWidth) {
                    xVal = srcLeft + event.target.offsetWidth;
                }
                else if (event.target.scrollWidth) {
                    xVal = srcLeft + event.target.scrollWidth;
                }
            }

            return (xVal)
        }


        function getEventY(event) {
            var yVal;
            if (IsBrowserIE()) {
                yVal = event.y;
            }
            else {
                yVal = event.pageY;
            }
            return (yVal)
        }

        function wamm_float_menu_block_mouseover(obj) {
            if (g_delayedHideTimer) {
                clearTimeout(g_delayedHideTimer);
                g_delayedHideTimer = null;
            }
        }

        function wamm_float_menu_block_mouseout(obj) {
            if (null != obj) {
                g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuNode(false, false, null, -1)", g_delayedHideTime);
            }
        }

        function routeAction(containerFlag, op) {
            var tree = null;
            if (g_wamm_float_menu_treeid > 0) {
                tree = TreeUtil.getTreeById(g_wamm_float_menu_treeid);
            }

            if (tree && tree.node && tree.node.data) {
                var TaxonomyId = tree.node.data.id;
                var ParentId = tree.node.pid;
                if (ParentId == null || ParentId == 'undefined') {
                    ParentId = 0;
                }

                showWammFloatMenuForMenuNode(false, false, null, -1);
                LoadChildPage(op, TaxonomyId, ParentId);
            }
        }
        function LoadChildPage(Action, TaxonomyId, ParentId) {
            var frameObj = document.getElementById("ChildPage");
            var lastClickedOn = document.getElementById("LastClickedOn");
            lastClickedOn.value = TaxonomyId;
            document.getElementById("LastClickedParent").value = ParentId;
            if (parseInt(ParentId) == 0) { document.getElementById("ClickRootCategory").value = "true"; }
            else { document.getElementById("ClickRootCategory").value = "false"; }
            switch (Action) {
                case "add":
                    if (TaxonomyId == "") {
                        alert("Please select a taxonomy.");
                        return false;
                    }
                    frameObj.src = "taxonomy.aspx?iframe=true&action=add&parentid=" + TaxonomyId;
                    break;
                default:
                    break;
            }
            if (Action != "delete") {
                DisplayIframe();
            }
        }
        function DisplayIframe() {
            var pageObj = document.getElementById("FrameContainer");
            pageObj.style.display = "";
            if (navigator.userAgent.indexOf("MSIE 6.0") > -1) {
                pageObj.style.width = "100%";
                pageObj.style.height = "500px";
            }
            else {
                pageObj.style.width = "95%";
                pageObj.style.height = "95%";
            }
        }
        function CancelIframe() {
            var pageObj = document.getElementById("FrameContainer");
            pageObj.style.display = "none";
            pageObj.style.width = "1px";
            pageObj.style.height = "1px";
        }
        function CloseChildPage() {
            CancelIframe();
            var ClickRootCategory = document.getElementById("ClickRootCategory");
            var lastClickedOn = document.getElementById("LastClickedOn");
            var clickType = document.getElementById("ClickType");
            if (ClickRootCategory.value == "true")
                __EkFolderId = "<%=m_intTaxFolderId%>";
            else {
                __EkFolderId = -1;
                TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
            }
            var node = document.getElementById("T" + lastClickedOn.value);
            if (node != null) {
                for (var i = 0; i < node.childNodes.length; i++) {
                    if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) < 9) {
                        if (node.childNodes(i).nodeName == 'LI' || node.childNodes(i).nodeName == 'UL') {
                            var parent = node.childNodes(i).parentElement;
                            parent.removeChild(node.childNodes(i));
                        }
                    }
                    else {
                        if (node.childNodes[i].nodeName == 'LI' || node.childNodes[i].nodeName == 'UL') {
                            var parent = node.childNodes[i].parentNode;
                            parent.removeChild(node.childNodes[i]);
                        }
                    }
                }
                TREES["T" + lastClickedOn.value].children = [];
                TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
                onToggleClick(lastClickedOn.value, TreeUtil.addChildren, lastClickedOn.value);
            }
        }
    </script>
	        <% if (Page.Request.Url.AbsoluteUri.IndexOf("membership_add_content.aspx") == -1 && Page.Request.Url.ToString().IndexOf("forum=1") == -1) { %>
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
             <% }else {%>
                <script type="text/javascript" >
                    var taxonomytreemenu = false;
                </script>
             <%}%>
      <%}%>
    <!--#include file="common/taxonomy_editor_menu.inc" -->
    <!--#include file="common/treejs.inc" -->
</body>
</html>

