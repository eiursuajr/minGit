<%@ Page Language="C#" AutoEventWireup="true" Inherits="libraryinsert" validateRequest="False" CodeFile="libraryinsert.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title><asp:literal id="litTitle" runat="server" /></title>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<meta http-equiv="pragma" content="no-cache"/>
		<asp:literal id="StyleSheetJS" runat="server"></asp:literal>
		<link href="java/plugins/modal/ektron.modal.css" rel="stylesheet" type="text/css" />
		
		<style type="text/css" >
		div.ektronWindow a.ektronModalClose
		{
		    background-color:transparent;
            background-image:url(images/application/close_red_sm.JPG);
            text-indent:-10000px;
            background-repeat:no-repeat;
            display:block;
            height:21px;
            overflow:hidden;
            position:absolute;
            right:0.5em;
            text-decoration:none;
            top:0.90em;
            width:21px;
            color:#FFFFFF;

	    }
	    div.ektronWindow h6
	     {
            background-color:#3163BD;
            background-image:url(images/application/darkblue_gradiant-nm.gif);
            background-position:0pt -2px;
            background-repeat:repeat-x;
            color:#FFFFFF;
            font-size:1em;
            margin:0pt;
            padding:0.6em 0.25em;
            position:relative;
          }
          .ektronWindow
          {
            margin-left:-15em;
            width:28em;
          }
          .ektronPageHeader{
            padding-top: 0px;
          }
		</style>
		
	</head>
	<body onload="javascript:top.SetLoadStatus('inserter');">
		<form id="Form1" method="post" runat="server">
		 <div class="ektronWindow" id="selAliasDialog">
		<div class="ektronModalHeader" >
		<h6><strong title="Insert QuickLink"><%=m_refContentApi.EkMsgRef.GetMessage("btn insert")%>  <%=m_refContentApi.EkMsgRef.GetMessage("lbl quicklink")%></strong></h6>
		<a href="#" class="ektronModalClose"></a>
		</div>
        <div class="divAliasList" id="divAliasList" runat="server" >
        </div>
        <div class="divOk" id="divOk" runat="server">
         <input type="submit" title="Ok" name="aliasSubmit" id="aliasSubmit"  size="20" value="<%=m_refContentApi.EkMsgRef.GetMessage("lbl ok")%>"  onclick="getRadioValue(0);" />
         </div>
        </div>
            <div id="dhtmltooltip"></div>
			<div class="ektronPageHeader">
			    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
			    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
		    </div>
		    <div class="ektronPageContainer ektronPageGrid">
		        <asp:DataGrid id="MediaListGrid" 
	                OnItemDataBound="MediaListGrid_ItemBound" 
	                AutoGenerateColumns="False" 
	                Runat="server" 
	                Width="100%"
			        AllowPaging="False" 
			        AllowCustomPaging="True" 
			        PageSize="10" 
			        EnableViewState="False" 
			        PagerStyle-Visible="False"
			        GridLines="None">
                    <HeaderStyle CssClass="title-header" />       			    
			    </asp:DataGrid>
		        <p class="pageLinks">
			        <asp:Label ToolTip="Page" runat="server" id="PageLabel">Page</asp:Label>
			        <asp:Label id="CurrentPage" CssClass="pageLinks" runat="server" />
			        <asp:Label ToolTip="of" runat="server" id="OfLabel">of</asp:Label>
			        <asp:Label id="TotalPages" CssClass="pageLinks" runat="server" />
		        </p>
		        <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" id="FirstPage" Text="[First Page]" OnCommand="NavigationLink_Click"
			        CommandName="First" />
		        <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" id="lnkBtnPreviousPage" Text="[Previous Page]" OnCommand="NavigationLink_Click"
			        CommandName="Prev" />
		        <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" id="NextPage" Text="[Next Page]" OnCommand="NavigationLink_Click"
			        CommandName="Next" />
		        <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" id="LastPage" Text="[Last Page]" OnCommand="NavigationLink_Click"
			        CommandName="Last" />
    							    
			    <input type="hidden" name="preview_type" value="<%=LibType%>" /> <input type="hidden" name="preview_filename"/>
			<input type="hidden" name="contentID" id="contentID" />
			<input type="hidden" name="noAlias" id="noAlias" />
		    </div>		    
		</form>
		<script type="text/javascript">
	    <!--//--><![CDATA[//><!--
	       var m_LibID, m_Folder, m_Title, m_FileName, m_Type, m_PreviewThumbnail;
		function Insert(libraryid, folder, title, filename, type,contentid){ 
		    document.forms[0].contentID.value = contentid;
		     m_Folder = folder;
		     m_Title = title;
		     m_LibID = libraryid;
		     m_FileName = filename;
		     m_Type = type;
		    var retField = '<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["RetField"])%>';
			top.opener.selectLibraryItem(libraryid, folder, title, filename, type, retField, '<%=SitePath%>');

			var szPreviewFormType = typeof(document.forms[0]);
			if (szPreviewFormType.toLowerCase() != "undefined") {
				document.forms[0].preview_filename.value = filename;
			}
		}
		function SubmitInsert() {
		    var alias = document.forms[0].noAlias.value;
		    if(alias != ""){
		          var retField1 = '<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["RetField"])%>';
		          top.opener.selectLibraryItem(m_LibID, m_Folder, m_Title, m_FileName, m_Type, retField1, '<%=SitePath%>');
		          top.close();
		    }
		    else{
		        top.close();
		        if (document.forms[0].preview_filename.value == "") {
				alert("<%= m_refMsg.GetMessage("js: alert double click lib name") %>");
				return false;
			}
			
		   }
			
		}
		$ektron().ready(function() 
		{
		    $ektron("#selAliasDialog").modal({
                modal: true,
                overlay: 0,
                trigger: ""
                });
		});
		//Alias Code Starts here...
		function showSelAliasdialog()
		{
		  $ektron.ajax({
              url: "urlaliasdialogHandler.ashx?action=getaliaslist&contID=" + document.forms[0].contentID.value + "&LangType=1033",
              cache: false,
              success: function(html){
                    if (html.indexOf("<error>") == -1)
                    {   
                        $ektron("#divAliasList").empty();
                        $ektron("#divAliasList").append("<p>" + html + "</p>");
                     
                        if(html.indexOf("<aliascount>") != -1)
                        {
                          if(html.indexOf("<linkmanage>")!=-1)
                          {
                                document.forms[0].noAlias.value = true;
                                $ektron("#selAliasDialog").modalShow();
                                return false;
                          }
                          else
                          {
                                document.forms[0].noAlias.value = true;
                                getRadioValue(1);
                               return false;
                          }
                        }
                        else
                        {
                          
                          $ektron("#selAliasDialog").modalShow();
                           document.forms[0].noAlias.value = true;
                          return false;
                        }
                    }
                    else
                    {
                        SubmitInsert();
                    }
              }
            });
		 
		 }
		
		 function SaveAlias(selradio)
		 {
		   
		    document.forms[0].preview_filename.value= selradio;
		    m_FileName = document.forms[0].preview_filename.value;
		    SubmitInsert();
		       
		    
		 }
		 function getRadioValue(count) 
		 {
		   
		    if(count ==1 )
		    {
		      var radioValue = document.forms[0].aliasSelect.value;
			  SaveAlias(radioValue);
			  return false;
		    }
		    else
		    {
		        var index;
		        for(index=0; index<document.forms[0].aliasSelect.length; index++)
		        {
    		      
			        if (document.forms[0].aliasSelect[index].checked) 
				        {
					        var radioValue = document.forms[0].aliasSelect[index].value;
					        SaveAlias(radioValue);
					        break;
				        }
    				 
			      }	
    		   }
    	       
	       }
		   
		    function previewItem(scope) {
			    var regexp1 = / /gi;
			    var filename = document.forms[0].preview_filename.value;		
			    //check if there is a value
			    if (filename == "") {
				    alert("<%= m_refMsg.GetMessage("js: alert single click lib name")%>");				
			    } else {
				    //create the popup 
				    popup = window.open('', 'imagePreview', 'width=600,height=450,left=100,top=75,screenX=100,screenY=75,scrollbars,location,menubar,status,toolbar,resizable=1');
				    //start writing in the html code
				    popup.document.writeln("<html><body bgcolor='#FFFFFF'>");
				    //get the extension of the file to see if it has one of the image extensions					
				    if (scope == "images") {					
					    popup.document.writeln("<img src='" + filename + "'>");
				    }
				    else if(scope == "hyperlinks") {
					    var tempString = document.forms[0].preview_filename.value.toLowerCase();
					    var tempHREF;
					    if ((tempString.substring(0,7) == "http://")
							    || (tempString.substring(0,8) == "https://")) {
						    tempHREF = document.forms[0].preview_filename.value;						
					    }
					    else {
						    tempHREF = "http://" + document.forms[0].preview_filename.value;
					    }
					    popup.document.writeln("<a href='" + tempHREF.replace(regexp1, "%20") + "'>" + tempHREF + "</a>");
    					
				    }
				    else {
					    //if not extension fron list above write URL to file 
					    popup.document.writeln("<a href='" + filename + "'>" + filename + "</a>");
				    }
				    popup.document.writeln("</body></html>");
				    popup.document.close();
				    popup.focus();
			    }
		    }
		    function PreviewFunct(oldURL){		
			    var regexp1 = / /gi;
			    if (document.forms[0].preview_filename.value == "") {
				    alert("<%= m_refMsg.GetMessage("js: alert single click lib name") %>");
				    return false;
			    }
			    else {
				    if ((document.forms[0].preview_type.value == "quicklinks") || (document.forms[0].preview_type.value == "forms")) {
					    if (document.forms[0].preview_filename.value.indexOf("?") != -1) {
						    var tempHREF = document.forms[0].preview_filename.value.replace(regexp1, "%20") + "&Preview=True";
					    }
					    else {
						    var tempHREF = document.forms[0].preview_filename.value.replace(regexp1, "%20") + "?Preview=True";
					    }
					    if ((document.forms[0].preview_type.value == "forms")) {
						    if (document.forms[0].preview_filename.value.indexOf("<%= SitePath %>") == -1) {
							    tempHREF = "<%= SitePath %>" + tempHREF;
						    }
					    }
				    }
				    else if ((document.forms[0].preview_type.value == "files") 
						    || (document.forms[0].preview_type.value == "images")) {
					    var tempHREF = document.forms[0].preview_filename.value.replace(regexp1, "%20");
				    }
				    else if (document.forms[0].preview_type.value == "hyperlinks") {
					    var tempString = document.forms[0].preview_filename.value.toLowerCase();
					    if ((tempString.substring(0,7) == "http://")
							    || (tempString.substring(0,8) == "https://")) {
						    var tempHREF = document.forms[0].preview_filename.value.replace(regexp1, "%20");
					    }
					    else {
						    var tempHREF = "http://" + document.forms[0].preview_filename.value.replace(regexp1, "%20");
					    }			
				    }
				    for (var i = 0; i < document.links.length; i++) {
					    if (document.links[i].href == oldURL) {
						    break;
					    }
				    }
				    document.links[i].href = tempHREF;
				    return true;
			    }
		    }
    		
		    var lastSelected = null;
		    var lastSelectedColor;
    		
		    function Blink(CellName, Color) {
			    if (lastSelected != CellName) {
				    if (lastSelected != null) {
					    if (document.all) {
						    document.all[lastSelected].style.backgroundColor = lastSelectedColor;
						    document.all[lastSelected + "_0"].style.backgroundColor = lastSelectedColor;
						    document.all[lastSelected + "_1"].style.backgroundColor = lastSelectedColor;
					    }
					    else if (document.getElementById) {
						    var MyObj = document.getElementById(lastSelected);
						    MyObj.style.background = lastSelectedColor;
						    var MyObj = document.getElementById(lastSelected + "_0");
						    MyObj.style.background = lastSelectedColor;
						    var MyObj = document.getElementById(lastSelected + "_1");
						    MyObj.style.background = lastSelectedColor;
					    }
					    else {
						    var layername = "layer" + lastSelected;
						    var NsObj = document.layers[layername];
						    NsObj.bgColor = lastSelectedColor;
						    var NsObj = document.layers[layername + "_0"];
						    NsObj.bgColor = lastSelectedColor;
						    var NsObj = document.layers[layername + "_1"];
						    NsObj.bgColor = lastSelectedColor;
					    }
				    }
				    lastSelected = CellName;
				    if (document.all) {
					    lastSelectedColor = document.all[CellName].style.backgroundColor;
				    }
				    else if (document.getElementById) {
					    var MyObj = document.getElementById(CellName);
					    lastSelectedColor = MyObj.style.background;
				    }
				    else {
					    var layername = "layer" + CellName;
					    var NsObj = document.layers[layername];
					    lastSelectedColor = NsObj.bgColor;
				    }
			    }
			    lastSelected = CellName;
			    if (document.all) {
				    document.all[CellName].style.backgroundColor = Color;
				    document.all[CellName + "_0"].style.backgroundColor = Color;
				    document.all[CellName + "_1"].style.backgroundColor = Color;
			    }
			    else if (document.getElementById) {
				    var MyObj = document.getElementById(CellName);
				    MyObj.style.background = Color;
				    var MyObj = document.getElementById(CellName + "_0");
				    MyObj.style.background = Color;
				    var MyObj = document.getElementById(CellName + "_1");
				    MyObj.style.background = Color;
			    }
			    else {
				    var layername = "layer" + CellName;
				    var NsObj = document.layers[layername];
				    NsObj.bgColor = Color;
				    var NsObj = document.layers[layername + "_0"];
				    NsObj.bgColor = Color;
				    var NsObj = document.layers[layername + "_1"];
				    NsObj.bgColor = Color;
			    }
		    }
    		
		    function updateFolders(Folder, folderType, imagepermission, filepermission, overwritepermission, libid){
			    if ((imagepermission != 0) && (imagepermission != 2)) {
				    imagepermission = 1;
			    }
			    if ((filepermission != 0) && ((filepermission != 2))) {
				    filepermission = 1;
			    }
			    if ((overwritepermission != 0) && ((overwritepermission != 2))) {
				    overwritepermission = 1;
			    }			
		    }
    		
		    function ClearFolderInfo() {
			    return true;
		    }
    					
	    //--><!]]>
		</script>
	</body>
</html>
