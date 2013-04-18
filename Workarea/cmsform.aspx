<%@ Page Language="C#" AutoEventWireup="true" Inherits="cmsform" CodeFile="cmsform.aspx.cs"
    ValidateRequest="false" %>

<%@ Reference Control="controls/forms/viewform.ascx" %>
<%@ Reference Control="controls/forms/newformwizard.ascx" %>
<%@ Reference Control="controls/forms/editform.ascx" %>
<%@ Reference Control="sync/SyncResources.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><asp:Literal runat="server" id="ltr_title"/></title>
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />

<style type="text/css">
    a.selusers
    {
	    text-decoration:underline;
    }
</style>

    <asp:PlaceHolder ID="sync_jsResourcesPlaceholder" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder id="phJSWithCodeBlocks" runat="Server">
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
		        var UniqueID="<asp:literal id="UniqueLiteral" runat="server"/>_";
		        var jsContentLanguage="<asp:literal id="jsContentLanguage" runat="server"/>";
		        var jsDefaultContentLanguage="<asp:literal id="txtDefaultContentLanguage" runat="Server"/>";
		        var jsEnableMultilingual="<asp:literal id="txtEnableMultilingual" runat="Server"/>";
		        var jsAction="<asp:literal id="jsAction" runat="server"/>";
		        var jsFormId="<asp:literal id="jsFormId" runat="server"/>"
		        var vFolderId="<asp:literal id="vFolderId" runat="server"/>";
	        //--><!]]>
        </script>
        <% if(Request.QueryString["FromEE"] != null) { %>
	    <% if(!((Request.QueryString["FromEE"].ToString().Trim()) == "1")) {%>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
            if(typeof top.HideDragDropWindow != "undefined")
            {
	            top.HideDragDropWindow();
	        }
	        //--><!]]>
        </script>
        <% } %>
        <% } %>
        <%= m_strStyleSheetJS %>

        <script type="text/javascript" src="java/jfunct.js">
</script>

        <script type="text/javascript" src="java/toolbar_roll.js">
</script>

        <script type="text/javascript" src="java/platforminfo.js">
</script>

        <script type="text/javascript" src="java/designformentry.js">
</script>

       <%-- <% if ((Request.QueryString["ShowTStatusMsg"].ToString().Trim()) == "1") { %>--%>
        <% if(Request.QueryString["ShowTStatusMsg"] != null) { %>
        <% if(Request.QueryString["ShowTStatusMsg"].ToString() == "1") { %>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
		    alert("<%= m_refMsg.GetMessage("alert msg waiting for completion")%>");
	        //--><!]]>
        </script>
        <%}%>
        <%}%>

        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
		    /***********************************************
		    * Contractible Headers script- Â© Dynamic Drive (www.dynamicdrive.com)
		    * This notice must stay intact for legal use. Last updated Oct 21st, 2003.
		    * Visit http://www.dynamicdrive.com/ for full source code
		    ***********************************************/
		    var enablepersist="off" //Enable saving state of content structure using session cookies? (on/off)
		    var collapseprevious="no" //Collapse previously open content when opening present? (yes/no)
            
		    <% if (m_bAjaxTree) { %>
		        <% if(Request.QueryString["FromEE"] != null && !((Request.QueryString["FromEE"].ToString().Trim()) == "1")) { %>
		             <% if (Request.QueryString["buttonid"].ToString() == "") { %> //For Content block widget
		            if (top["ek_nav_bottom"])
		            {
                        if('undefined' != typeof(top["ek_nav_bottom"]["NavIframeContainer"]) && 'undefined' != typeof(top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"])){
		                        if('undefined' != typeof(top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"]["FormsTree"])){
			                        var treeobj=top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"]["FormsTree"];
			                        if(treeobj.document.getElementById("selected_folder_id")!=null){
				                        var SelectedTreeId=treeobj.document.getElementById("selected_folder_id").value;
				                        var CurrentFolderId="<%=Request.QueryString["folder_id"]%>";
				                        if(CurrentFolderId==0 && SelectedTreeId!=0) {
					                        var stylenode = treeobj.document.getElementById( SelectedTreeId );
					                        if(stylenode!=null){
						                        stylenode.style["background"] = "#ffffff";
						                        stylenode.style["color"] = "#000000";
						                        var stylenode = treeobj.document.getElementById( CurrentFolderId );
						                        if (stylenode != null)
						                        {
							                        stylenode.style["background"] = "#3366CC";
							                        stylenode.style["color"] = "#ffffff";
						                        }
					                        }
				                        }
				                    }
			                    }
			                }
			            }
			            <% } %>
		     <% } %>
		    <% } %>

		    if (document.getElementById){
			    document.write('<style type="text/css">')
			    document.write('.switchcontent{display:none;}')
			    document.write('</style>')
		    }

		    function getElementbyClass(classname){
			    ccollect=new Array()
			    var inc=0
			    var alltags=document.all? document.all : document.getElementsByTagName("*")
			    for (i=0; i<alltags.length; i++){
				    if (alltags[i].className==classname)
					    ccollect[inc++]=alltags[i]
				    }
		    }

		    function contractcontent(omit){
			    var inc=0
			    while (ccollect[inc]){
				    if (ccollect[inc].id!=omit)
					    ccollect[inc].style.display="none";
					    collapseprevious = "no";
				    inc++
			    }
		    }

		    function expandcontent(cid){
			    if (typeof ccollect!="undefined"){
				    if (collapseprevious=="yes")
					    contractcontent(cid);
					    document.getElementById(cid).style.display=(document.getElementById(cid).style.display!="block")? "block" : "none";
			    }
		    }

		    function revivecontent(){
			    contractcontent("omitnothing")
			    selectedItem=getselectedItem()
			    selectedComponents=selectedItem.split("|")
			    for (i=0; i<selectedComponents.length-1; i++)
				    document.getElementById(selectedComponents[i]).style.display="block"
		    }

		    function get_cookie(Name) {
			    var search = Name + "="
			    var returnvalue = "";
			    if (document.cookie.length > 0) {
				    offset = document.cookie.indexOf(search)
				    if (offset != -1) {
					    offset += search.length
					    end = document.cookie.indexOf(";", offset);
					    if (end == -1) end = document.cookie.length;
						    returnvalue=unescape(document.cookie.substring(offset, end))
				    }
			    }
			    return returnvalue;
		    }

		    function getselectedItem(){
			    if (get_cookie(window.location.pathname) != ""){
				    selectedItem=get_cookie(window.location.pathname)
				    return selectedItem
			    }
			    else
				    return ""
		    }

		    function saveswitchstate(){
			    var inc=0, selectedItem=""
			    while (ccollect[inc]){
				    if (ccollect[inc].style.display=="block")
				    selectedItem+=ccollect[inc].id+"|"
				    inc++
			    }
			    document.cookie=window.location.pathname+"="+selectedItem
		    }

		    function do_onload(){
			    getElementbyClass("switchcontent")
			    if (enablepersist=="on" && typeof ccollect!="undefined")
				    revivecontent()
		    }

		    if (window.addEventListener)
			    window.addEventListener("load", do_onload, false)
		    else if (window.attachEvent)
			    window.attachEvent("onload", do_onload)
		    else if (document.getElementById)
			    window.onload=do_onload

		    if (enablepersist=="on" && document.getElementById)
		    {
			    window.onunload=saveswitchstate;
		    }
	        //--><!]]>
        </script>

        <script type="text/javascript">
	        <!--//--><![CDATA[//><!--
	        
	        $ektron.addLoadEvent(function(){
                Ektron.Workarea.Sync.Relationships.Init();
            });
	        
		    function PopEditWindow(URLInfo, Name, a, b, c, d) {
			    EditHandle = PopUpWindow(URLInfo, Name, a, b, c, d);
			    top.SetEditor(EditHandle);
		    }

		    function NavigateFolder(actionItem,FID){
			    var tmpHref =  'cmsform.aspx?action=' + actionItem;
			    //alert('The action Item is ->' + actionItem);
			    // WARNING: the URL with subject and preamble could exceed the 2K limit of URLs, but can't see where this function is called at all.
			    tmpHref = tmpHref + '&folderid=' + FID; // WARNING: probably should be folder_id, but can't see where this function is called at all.
			    tmpHref = tmpHref + '&form_title=' + escape(document.getElementById(UniqueID+"frm_form_title").value);
			    tmpHref = tmpHref + '&form_description=' + escape(document.getElementById(UniqueID+"frm_form_description").value);
			    tmpHref = tmpHref + '&mail_to=' + escape(document.getElementById(UniqueID+"frm_form_mailto").value);
			    tmpHref = tmpHref + '&mail_from=' + escape(document.getElementById(UniqueID+"frm_form_mailfrom").value);
			    tmpHref = tmpHref + '&mail_cc=' + escape(document.getElementById(UniqueID+"frm_form_mailcc").value);
			    tmpHref = tmpHref + '&mail_subject=' + escape(document.getElementById(UniqueID+"frm_form_mailsubject").value);
			    tmpHref = tmpHref + '&mail_preamble=' + escape(document.getElementById(UniqueID+"frm_form_mailpreamble").value);
			    self.location.href = tmpHref;
			    return false;
		    }
		    function SetContentChoice(cTitle,cId,Qlink){
			    document.forms.myform.frm_content_title.value = cTitle;
			    document.forms.myform.frm_content_id.value = cId;
			    return false;
		    }

		    function ConfirmFormDelete() {
			    return confirm("<%=m_refMsg.GetMessage("alert msg del form item")%>");
		    }

		    function SubmitForm(FormName, Validate) {
			    if (Validate.length > 0) {
				    if (eval(Validate)) {
					    document.forms[0].submit();
					    return false;
				    }
				    else {
					    return false;
				    }
			    }
			    else {
				    document.forms[0].submit();
				    return false;
			    }
		    }

		    function Trim (string) {
			    if (string.length > 0) {
				    string = RemoveLeadingSpaces (string);
			    }
			    if (string.length > 0) {
				    string = RemoveTrailingSpaces(string);
			    }
			    return string;
		    }

		    function RemoveLeadingSpaces(string) {
			    while(string.substring(0, 1) == " ") {
				    string = string.substring(1, string.length);
			    }
			    return string;
		    }

		    function RemoveTrailingSpaces(string) {
			    while(string.substring((string.length - 1), string.length) == " ") {
				    string = string.substring(0, (string.length - 1));
			    }
			    return string;
		    }

		    function CheckKeyValue(item, keys) {
			    var keyArray = keys.split(",");
			    for (var i = 0; i < keyArray.length; i++) {
				    if ((document.layers) || ((!document.all) && (document.getElementById))) {
					    if (item.which == keyArray[i]) {
						    return false;
					    }
				    }
				    else {
					    if (event.keyCode == keyArray[i]) {
						    return false;
					    }
				    }
			    }
		    }

		    function AddNewForm(parms) {
			    var bContinue = true;
			    var contentLang = parseInt(jsContentLanguage, 10);
			    var multiSupport = true;//jsEnableMultilingual;
			    if ((contentLang < 1) && multiSupport)
			    {
				    bContinue = confirm("<%=m_refMsg.GetMessage("alert msg add content lang")%>");// TODO: BCB: Remove hard-coded string.
			    }
			    if (bContinue)
			    {
				    // force language to default:
				    if ("string" == typeof parms)
				    {
					    parms = parms.replace(/LangType\=(\-1|0)\&/, "LangType=" + jsDefaultContentLanguage + "&");
				    }
				    top.document.getElementById('ek_main').src = 'cmsform.aspx?action=Addform&' + parms;
			    }v
		    }
	        //--><!]]>
        </script>

        <!-- copied from tasks.aspx -->

        <script type="text/javascript">
	        <!--//--><![CDATA[//><!--
		    function selectUser(userType, id, name,showSelect) {
		    showSelect = false;
		    var NavObjUser;
		    var NavObjNoUser;
		    var NS4;
		    //debugger
		    if (document.layers){
			    NS4 = true;
			    document.forms[0].elements["form$nsfourname"].value = name;
		    }
		    else if (document.all){
			    NavObjUser = document.all["user"];
			    NavObjNoUser = document.all["nouser"];
		    }
		    else {
			    NavObjUser = document.getElementById("user");
			    NavObjNoUser = document.getElementById("nouser");
		    }
		    if (!NS4){
			    NavObjNoUser.style.display = "none";
		    }
		    if (userType == "0"){
			    /* TODO: CL need to connect the form_id to task for all authors to get its folder permission.
			    if ("all authors" == name.toLowerCase())
			    {
				   document.forms[0].elements["form:assigned_to_user_id"].value = "";
				   document.forms[0].elements["form:assigned_to_usergroup_id"].value = "0";
			    }
			    else //(unassigned)
			    {*/
				   document.forms[0].elements["form$assigned_to_user_id"].value = "";
				   document.forms[0].elements["form$assigned_to_usergroup_id"].value = "";
			    //}
			    if (!NS4){
				    if(showSelect ==  "1")
				    {
					    NavObjUser.innerHTML = name + "&nbsp;&nbsp;<a href='#' onclick='ShowUsers()' class='selusers'>Select User or Group</a>";
				    }
				    else
				    {
					    NavObjUser.innerHTML = name;
				    }
			    }
		    }
		    else if (userType == "1"){
			   document.forms[0].elements["form$assigned_to_user_id"].value = id;
			   document.forms[0].elements["form$assigned_to_usergroup_id"].value = "";
			   document.forms[0].elements["form$assigned_by_user_id"].value =document.forms[0].elements["form$current_user_id"].value;
			    if (!NS4){
				    if(showSelect ==  "1")
				    {
					    NavObjUser.innerHTML = "<img src='<%=(AppPath)%>images/UI/Icons/user.png' align='absbottom'>" + name + "&nbsp;&nbsp;<a href='#' onclick='ShowUsers()' class='selusers'>Select User or Group</a>";
				    }
				    else
				    {
					    NavObjUser.innerHTML = "<img src='<%=(AppPath)%>images/UI/Icons/user.png' align='absbottom'>" + name;
				    }

			    }
		    }
		    else if (userType == "2"){
			   document.forms[0].elements["form$assigned_to_user_id"].value = "";
			   document.forms[0].elements["form$assigned_to_usergroup_id"].value = id;
			   document.forms[0].elements["form$assigned_by_user_id"].value =document.forms[0].elements["form$current_user_id"].value;
			    if (!NS4){
				    if(showSelect ==  "1")
				    {
					    NavObjUser.innerHTML = "<img src='<%=(AppPath)%>images/UI/Icons/users.png' align='absbottom'>" + name + "&nbsp;&nbsp;<a href='#' onclick='ShowUsers()' class='selusers'>Select User or Group</a>";
				    }
				    else
				    {
					    NavObjUser.innerHTML = "<img src='<%=(AppPath)%>images/UI/Icons/users.png' align='absbottom'>" + name;
				    }
			    }
		    }
		    if (!NS4){
			    NavObjUser.style.display = "inline"; //"block";
			    document.getElementById("idUnassignTask").style.display = "inline";
		    }
	    }

	    function ShowUsers()
	    {
		    var formId = document.getElementById("form_content_id").value;
		    if (0 == formId)
		    {
			    //need to pass along the folder id for user permission list when creating new form
			    PopUpWindow("selectusergroup.aspx?id=" + formId + "&LangType=" + document.getElementById("form_current_language").value + "&folder_id=" + vFolderId + "&TaskType=9" , "SelectUser", 400, 300, 1, 1);
		    }
		    else
		    {
			    PopUpWindow("selectusergroup.aspx?id=" + formId + "&LangType=" + document.getElementById("form_current_language").value + "&TaskType=9" , "SelectUser", 400, 300, 1, 1);
		    }
	    }

	        //--><!]]>
        </script>

        <script type="text/javascript">
	    <!--//--><![CDATA[//><!--
	    function unassignTask()
	    {
		    selectUser("0", "", "<%= m_refMsg.GetMessage("lbl unassigned")%>", "1");
		    document.getElementById("idUnassignTask").style.display = "none";
	    }
	    //--><!]]>
        </script>

        <%=m_strReloadJS%>
    </asp:PlaceHolder>
</head>
<body>
        <!-- Modal Dialog: Confirm -->
        <div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all" id="ConfirmDialog" style="display: none;">
            <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix  ektronModalHeader">
                <h3 class="ui-dialog-title header">
                    <span class="headerText"></span>
                    <asp:HyperLink ToolTip="Close" ID="closeDialogLink" CssClass="ui-dialog-titlebar-close ui-corner-all ektronModalClose" runat="server" />
                </h3>
            </div>
            <div class="ektronModalBody">
                <div class="ui-dialog-content ui-widget-content ektronPageInfo">
                    <p class="messages"></p>
                </div>
                <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                    <li><asp:HyperLink ToolTip="Cancel" ID="btnConfirmCancel" runat="server"  CssClass="redHover button cancelButton buttonRight" /></li>
                    <li><asp:HyperLink ToolTip="Ok" ID="btnConfirmOk" runat="server" CssClass="greenHover button okButton buttonRight" /></li>
                </ul>
            </div>
        </div>
        <!-- Modal Dialog: Sync Status -->
        <div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all" id="SyncStatusModal" style="display: none;">
            <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix">
                <h3 class="ui-dialog-title header">
                    <span class="headerText"><asp:Literal ID="lblSyncStatus" runat="server" /></span>
                    <asp:HyperLink ToolTip="Close" ID="closeDialogLink2" CssClass="ui-dialog-titlebar-close ui-corner-all ektronModalClose" runat="server" />
                </h3>
            </div>
            <div class="ektronModalBody">
                <div class="ui-dialog-content ui-widget-content ektronPageInfo">
                    <p class="messages"></p>
                    <div class="syncStatusMessages"></div>
                </div>
                <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                    <li><asp:HyperLink ToolTip="Close Sync" ID="btnCloseSyncStatus" runat="server"  CssClass="redHover button buttonNoIcon buttonRight" /></li>
                </ul>
            </div>
        </div>
        <!-- Modal Dialog: Show Sync Configuration Modal -->
        <div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all" id="ShowSyncConfigModal" style="display: none;">
            <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix">
                <h3 class="ui-dialog-title header">
                    <span class="headerText"></span>
                    <asp:HyperLink ToolTip="Close" ID="closeDialogLink3" CssClass="ui-dialog-titlebar-close ui-corner-all ektronModalClose" runat="server" />
                </h3>
            </div>
            <div class="ektronModalBody">
                <div class="ui-dialog-content ui-widget-content">
                    <p class="messages"></p>
                    <ul class="server" id="configurations"></ul>
                    <select id="selectConfigs" size="7" ></select>
                </div>
                <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                    <li><asp:HyperLink ToolTip="Start Sync" ID="btnStartSync" runat="server" CssClass="greenHover button performSyncButton buttonRight" onclick="Ektron.Workarea.Sync.Relationships.StartContentFolderSync(); return false;" /></li>
                </ul>
            </div>
        </div>
    <form id="myform" name="myform" method="post" runat="server">
        <input onkeypress="return CheckKeyValue(event,'34');" type="hidden" name="netscape" />
        <input type="hidden" id="folder_id" name="folder_id" value="-1" runat="server" />
        <asp:PlaceHolder ID="DataHolder" runat="server"></asp:PlaceHolder>
    </form>
</body>
</html>

