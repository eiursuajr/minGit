<%@ Page Language="C#" AutoEventWireup="true" Inherits="QuickLinkSelect" CodeFile="QuickLinkSelect.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>QuickLinkSelect</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        function SetContentChoice(cTitle,cID,cLID,QLink,fName,TitleName,useQLC, setBState,cStatus)
        {
            window.opener.document.forms[0][TitleName].value = cTitle ;
	        window.opener.document.forms[0].frm_content_id.value = cID ;
	        window.opener.document.forms[0].frm_content_langid.value = cLID ;
	        window.opener.document.forms[0].frm_qlink.value = QLink ;
        	if((window.opener.document.forms[0]) && (window.opener.document.forms[0].frm_content_status!= 'undefined') && (window.opener.document.forms[0].frm_content_status!=null)){
        	    window.opener.document.forms[0].frm_content_status.value=cStatus;
        	}
	        if(useQLC=='1') {
		        if(window.opener.document.forms[0] && window.opener.document.forms[0].frm_use_qlink) {
			        window.opener.document.forms[0].frm_use_qlink.checked = true ;
		        }
	        }

	        if(setBState=='1')
	        {
	            if ("function" == typeof window.opener.SetBrowserState || "object" == typeof window.opener.SetBrowserState)
	            {
		            window.opener.SetBrowserState();
		        }
	        }
	        window.close();
        }
        function SetQLinkChoice(cTitle,cID,cLID,QLink,fName,TitleName,useQLC, setBState,cStatus)
        {
            if(parent.document.getElementById(TitleName))
            {
                parent.document.getElementById(TitleName).value=cTitle;
            }
            if (parent.document.getElementById('frm_content_id'))
            {
                parent.document.getElementById('frm_content_id').value=cID;
            }
            if (parent.document.getElementById('frm_content_langid'))
            {
                parent.document.getElementById('frm_content_langid').value=cLID;
            }
            if (parent.document.getElementById('frm_qlink'))
            {
                parent.document.getElementById('frm_qlink').value=QLink;
            }

	        if(setBState=='1')
	        {
		        parent.SetBrowserState();
             }
        }
        //--><!]]>
    </script>
    <style type="text/css">
    <!--/*--><![CDATA[/*><!--*/

    	span.filePath {display: inline-block; color:#e17009; margin-left: .5em; padding: .25em; border: solid 1px #ccc; cursor: default; background-color: #eee;}
    /*]]>*/-->
    </style>
</head>
<body>
    <form id="form_qlink" method="post" runat="server">
        <div id="dhtmltooltip"></div>

        <%=m_refStyle.GetClientScript()%>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
		    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
		</div>
        <div class="ektronPageContainer ektronPageInfo">
            <asp:DataGrid ID="QLinkGrid"
                runat="server"
                AutoGenerateColumns="False"
                Width="100%"
                OnItemDataBound="QLinkGrid_ItemDataBound"
                GridLines="None">
            </asp:DataGrid>
			<p class="pageLinks">
								<asp:Label ToolTip="Page" runat="server" id="PageLabel" >Page</asp:Label>
								<asp:Label id="CurrentPage" CssClass="pageLinks" runat="server" />
								<asp:Label ToolTip="of" runat="server" id="OfLabel" >of</asp:Label>
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
        </div>
    </form>
</body>
</html>

