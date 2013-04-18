<%@ Page Language="C#" AutoEventWireup="true" Inherits="selectcreatecontent" validateRequest="false" CodeFile="selectcreatecontent.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title></title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
		var content_id = "";
		var content_title = "";
		var Content_QLink = "";  
		var objTeaser = "";
		var folderId = '<asp:Literal id="ltr_folderId" runat="server" />';
		var languageID = '<asp:Literal id="ltr_ContentLanguage" runat="server" />';
		var pleaseSelectMsg = '<asp:Literal id="ltr_pleaseSelectMsg" runat="server" />';
		var ItemID = '<asp:Literal id="ltr_ItemID" runat="server" />';
		var callerPage = '<asp:Literal id="ltr_callerPage" runat="server" />';
		var folderType = '<asp:Literal id="ltr_folderType" runat="server" />';

		function SaveSelCreateContent(RedirectUrl, ParentFolderId) {
		    if(!($('.urlAliasInputHidden').val() == "")){
            Content_QLink = $('.urlAliasInputHidden').val();
            }
		    var idx,  qtyElements, lnk, strTemp;
		    var notSupportIFrame = '<asp:Literal id="ltr_notSupportIFrame" runat="server" />';
		    if (RedirectUrl == undefined || RedirectUrl == "") {
		       if (content_id === "")
			    {
				    alert(pleaseSelectMsg);
				    return false;
			    }
			    if(notSupportIFrame == "1" && !(document.getElementById))
			    {
				    if(top.opener != null && !(top.opener.closed) && (typeof(top.opener.ReturnChildValue) == 'function'))
				    {
				        switch (callerPage){
				            case "suggestedResults":
				                top.opener.ReturnChildValue(content_id,content_title,Content_QLink,objTeaser,folderId,languageID);
				                break;
				            default:
				                top.opener.ReturnChildValue(content_id, content_title, Content_QLink, folderId, languageID);
					    }
					    
					    top.close();
				    }
				    else
				    {
					    alert('Unable to save changes. The work area page has been closed.');
					    top.close();
				    }
			    }
			    else
			    {
				    if( parent.ReturnChildValue == 'undefined' || parent.ReturnChildValue == null ) {
				        switch (callerPage){
				            case "suggestedResults":
				                top.opener.ReturnChildValue(content_id,content_title,Content_QLink,objTeaser,folderId,languageID);
				                break;
				            default:
				                top.opener.ReturnChildValue(content_id, content_title, Content_QLink, folderId, languageID);
					    }
				        //top.opener.ReturnChildValue(content_id,content_title,Content_QLink,folderId,languageID);
				        top.close();
				    }
				    else {
				        switch (callerPage){
				            case "suggestedResults":
				                parent.ReturnChildValue(content_id,content_title,Content_QLink,objTeaser,folderId,languageID);
				                break;
				            default:
				                parent.ReturnChildValue(content_id, content_title, Content_QLink, folderId, languageID);
					    }
				    }
		        }
		    }
		    else {
			    if(notSupportIFrame == "1"){
				    qtyElements = document.forms[0].elements.length;
				    // redirect to approval.aspx page if action = viewApprovalList else go to report with specified action
				    // Site activity has a link to select a folder and display its name in the page, needs to be treated different than other reports
				    if (RedirectUrl == "viewapprovallist") {
					    lnk = "Approval.aspx?action=" + RedirectUrl + "&fldid="
				    }
				    else {
						    lnk = "reports.aspx?action=" + RedirectUrl + "&language=" + languageID + "&filtertype=path&filterid=" ;
				    }
				    var isChecked = false;
				    for(idx = 0; idx < qtyElements; idx++ ) {
					    if (document.forms[0].elements[idx].type == "radio"){
						    if (document.forms[0].elements[idx].checked == true) {
							    isChecked = true;
							    if (RedirectUrl == "siteupdateactivity") {
								    myString = new String(document.forms[0].elements[idx].value);
								    if (myString.indexOf(":") > -1) {
									    //lnk = lnk + myString.substring(0,myString.indexOf(":")) + "&FldrName=" + myString.substring(myString.indexOf(":")+1,myString.length);
									    top.opener.document.getElementById("hselFolder").innerHTML = "<div id=\"div3\">" + myString.substring(myString.indexOf(":")+1,myString.length) +  "</div>";
									    top.opener.document.getElementById("fId").value = myString.substring(0,myString.indexOf(":"));
									    top.opener.document.getElementById("rptFolder").value = myString.substring(myString.indexOf(":")+1,myString.length);
									    top.close();
									    return false;
								    }
							    }
						    else {
								    lnk = lnk + document.forms[0].elements[idx].value;
						    }
						    }
					    }
				    }
				    if (!isChecked) {
					    lnk = lnk + ParentFolderId;
					    //lnk = lnk + "0";
					    if (RedirectUrl == "siteupdateactivity") {
							    strTemp = document.forms[0].outerHTML;
							    strTemp = strTemp.substring (strTemp.indexOf("Path:"), strTemp.length);
							    strTemp = strTemp.substring(5,strTemp.indexOf("</TD>"));

							    parent.document.getElementById("hselFolder").innerHTML = "<div id=\"div3\">" + strTemp +  "</div>";
							    parent.document.getElementById("fId").value = ParentFolderId;
							    parent.document.getElementById("rptFolder").value = strTemp;
							    parent.CloseChildPage();
							    return false;
						    }
				    }
				    top.opener.document.forms[0].action = lnk;
				    top.opener.document.forms[0].__VIEWSTATE.name = 'NOVIEWSTATE';
				    top.opener.document.forms[0].submit();
				    top.close();
			    }
			    else
			    {
					    qtyElements = document.forms[0].elements.length;
					    // redirect to approval.aspx page if action = viewApprovalList else go to report with specified action
					    // Site activity has a link to select a folder and display its name in the page, needs to be treated different than other reports
					    if (RedirectUrl == "viewapprovallist") {
						    lnk = "Approval.aspx?action=" + RedirectUrl + "&fldid="
					    }
					    else {
							    lnk = "reports.aspx?action=" + RedirectUrl + "&language=" + languageID + "&filtertype=path&filterid=" ;
					    }
					    var isChecked = false;
					    for(idx = 0; idx < qtyElements; idx++ ) {
						    if (document.forms[0].elements[idx].type == "radio"){
							    if (document.forms[0].elements[idx].checked == true) {
								    isChecked = true;
								    if (RedirectUrl == "siteupdateactivity") {
									    myString = new String(document.forms[0].elements[idx].value);
									    if (myString.indexOf(":") > -1) {
										    //lnk = lnk + myString.substring(0,myString.indexOf(":")) + "&FldrName=" + myString.substring(myString.indexOf(":")+1,myString.length);
										    parent.document.getElementById("hselFolder").innerHTML = "<div id=\"div3\">" + myString.substring(myString.indexOf(":")+1,myString.length) +  "</div>";
										    parent.document.getElementById("fId").value = myString.substring(0,myString.indexOf(":"));
										    parent.document.getElementById("rptFolder").value = myString.substring(myString.indexOf(":")+1,myString.length);
										    parent.CloseChildPage();
										    return false;
									    }
								    }
							    else {
									    lnk = lnk + document.forms[0].elements[idx].value;
							    }
							    }
						    }
					    }
					    if (!isChecked) {
						    lnk = lnk + ParentFolderId;
						    //lnk = lnk + "0";
						    if (RedirectUrl == "siteupdateactivity") {
							    strTemp = document.forms[0].outerHTML;
							    strTemp = strTemp.substring (strTemp.indexOf("Path:"), strTemp.length);
							    strTemp = strTemp.substring(5,strTemp.indexOf("</TD>"));

							    parent.document.getElementById("hselFolder").innerHTML = "<div id=\"div3\">" + strTemp +  "</div>";
							    parent.document.getElementById("fId").value = ParentFolderId;
							    parent.document.getElementById("rptFolder").value = strTemp;
							    parent.CloseChildPage();
							    return false;
						    }
					    }
					    parent.document.forms[0].action = lnk;
					    parent.document.forms[0].__VIEWSTATE.name = 'NOVIEWSTATE';
					    parent.document.forms[0].submit();
					    parent.CloseChildPage();
			    }
		    }
		    return false;
		}

		function CancelSelContent()
		{
		var notSupportIFrame = '<asp:Literal id="ltr_notSupportIFrameCancel" runat="server" />';
		if(notSupportIFrame == "1")
		{
			top.close();
		}
		else {
			if( parent.CloseChildPage == 'undefined' || parent.CloseChildPage == null ) {
			    top.close();
			}
			else {
			    parent.CloseChildPage();
			}
		}
		return false;
		}

		function SetContentChoice(cID, cTitle, QLink, Teaser) {
		    content_id = cID;
		    content_title = cTitle;
            if (folderType == "Domain") {
		        if($('.urlAliasInputHidden').val() == "") {
		            showSelAliasdialog(cID, languageID);
                    Content_QLink = $('.urlAliasInputHidden').val();
                }
            }else{
		    Content_QLink = QLink; }
		    objTeaser = document.getElementById(Teaser);
		}
		//--><!]]>
	</script>	
	<asp:literal id="StyleSheetJS" runat="server"/>
	<style type="text/css">	   
	    .addPadding {padding: 0 .5em}
	    .ektronPageGrid table tr td img {margin-right: .5em;}
	    .ektronPageGrid table tr td {padding-top: .25em}
	    span.selectedContent {display: inline; color:#e17009; margin-left: .5em; padding: .25em; border: solid 1px #ccc; cursor: default; background-color: #eee;}
		.folderOutput img {margin-right: .5em;}	   
	</style>
  </head>
  <body onload="ScrollParentToTop();">
    <form id="Form1" method="post" runat="server">
    <div class="uxAliasradiobox"></div>
    <input type="hidden" name="urlAliasInputHidden" class="urlAliasInputHidden" />
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" title="Select Content" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer addPadding">
            <div class="ektronPageGrid">
                <asp:DataGrid ID="ContentGrid"
                    Runat="server"
                    OnItemDataBound="Grid_ItemDataBound"
                    AutoGenerateColumns="False"
                    AllowPaging="False"
                    AllowCustomPaging="True"
                    PageSize="10"
                    PagerStyle-Visible="False"
                    EnableViewState="False"
                    GridLines="None"
                    CssClass="folderOutput" />
                <p class="pageLinks">
                    <asp:Label ToolTip="Page" runat="server" id="PageLabel" >Page</asp:Label>
                    <asp:Label id="CurrentPage" CssClass="pageLinks" runat="server" />
                    <asp:Label ToolTip="of" runat="server" id="OfLabel" >of</asp:Label>
                    <asp:Label id="TotalPages" CssClass="pageLinks" runat="server" />
                </p>
                <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks"
                id="FirstPage" Text="[First Page]"
                OnCommand="NavigationLink_Click" CommandName="First" />
                <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks"
                id="lnkBtnPreviousPage" Text="[Previous Page]"
                OnCommand="NavigationLink_Click" CommandName="Prev" />
                <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks"
                id="NextPage" Text="[Next Page]"
                OnCommand="NavigationLink_Click" CommandName="Next" />
                <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks"
                id="LastPage" Text="[Last Page]"
            OnCommand="NavigationLink_Click" CommandName="Last" />
		    </div>
		    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData"/>
		</div>
        
    </form>
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        function RecursiveSubmit(parentfolderid,actionname, fromPage, pageAction)
		{
			if (fromPage == 'undefined' || fromPage == null){
				fromPage = ""
			}
			if (pageAction == 'undefined' || pageAction == null){
				pageAction = ""
			}

			var cType = '<asp:Literal id="ltr_overrideContentEnum" runat="server" />';
			var startingFolderId = '<asp:Literal id="ltr_StartingFolderID" runat="server" />';
			var ContentLanguage = '<asp:Literal id="ltr_ContentLanguageRecursive" runat="server" />';

            if('<%= blnForTasks%>'.toLowerCase() == 'true')
            {
			   document.forms[0].action="SelectCreateContent.aspx?StartingFolderID=" + startingFolderId + "&FolderID=" +parentfolderid +"&actionName=" + actionname + "&LangType=" + ContentLanguage + "&from_page=" + fromPage + "&action=" + pageAction + "&for_tasks=1" + "&ty=" + callerPage;
            }
            else
            {
              document.forms[0].action="SelectCreateContent.aspx?StartingFolderID=" + startingFolderId + "&FolderID=" +parentfolderid +"&actionName=" + actionname + "&LangType=" + ContentLanguage + "&from_page=" + fromPage + "&action=" + pageAction + "&ty=" + callerPage;
            }
			if( cType != 0 ) {
			    document.forms[0].action += "&overrideType=" + cType;
			}

			document.forms[0].submit();
		}
		function ScrollParentToTop()
		{
		//parent.scrollTo(0,0);
		}
		function resetPostback()
		{
			document.forms[0].isPostData.value = "";
}
function showSelAliasdialog(contId,contLanguage) {
    var retval = "";
    $ektron.ajax({
        url: "urlaliasdialogHandler.ashx?action=getaliaslist&contID=" + contId + "&LangType=" + contLanguage,
        cache: false,
        success: function (html) {
            retval = html;
            $('.uxAliasradiobox')[0].innerHTML = html;
            var aliasSelect = $('.uxAliasradiobox').find('#aliasSelect')
             if (html.indexOf("<error>") == -1) {
                if (html.indexOf("<aliascount>") != -1) {
                    if (html.indexOf("<linkmanage>") != -1) {


                    }
                    else {
                        //                        var IsLockedVar = document.getElementsByName('aliasSelect');
                        //                        for (var x = 0; x < IsLockedVar.length; x++) {
                        //                            if (IsLockedVar[x].checked) {
                        //                                QLink = IsLockedVar[x].value;
                        //                            }
                        //                        }

                        $('.urlAliasInputHidden').val(aliasSelect.val());
                    }
                }
                else {

                }
            } else {

            }
        }
    });
    return retval;
}
		//--><!]]>
    </script>
  </body>
</html>

