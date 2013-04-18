<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dictionaryconfigurator.aspx.cs" Inherits="Ektron.ContentDesigner.DictionaryConfigurator.DictionaryConfigurator" %>
<%@ register src="Controls/Default.ascx" tagprefix="uc1" tagname="default" %>
<%@ register assembly="Ektron.RadSpell" namespace="Ektron.Telerik.WebControls" tagprefix="radS" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" 
  "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html>
<head id="Head1" runat="server">
    
</head>
<body class="BODY">
    <!-- **********************************************************************     -->
    <!--  FOR SECURITY REASONS, DO NOT ALLOW ACCESS TO THIS FOLDER                  -->
    <!--  To enable this dictionay configurator, ASPNet user needs to have full     -->
    <!--  permission to read from write to the dictionary TDF folder                -->
    <font size=4 color="red" face="Arial, Helvetica">You can only edit eWebEdit400 spell check dictionaries if the asp.net user is granted full permission to Workarea\Foundation\RadControls\Spell\TDF\ folder.  Access to the server's file system should only be allowed within a secured, password protected environment.</font>

    <!--  **********************************************************************    -->
    <form id="mainForm" runat="server" method="post" style="width: 100%">
        <uc1:default runat="server" id="NavigationControl" />
        <asp:placeholder id="configuratorContents" runat="server"></asp:placeholder>

        <script runat="server" language="C#">
        protected override void OnLoad(EventArgs args)
        {
	        string page = "Import";
	        if (Request.QueryString["Page"] != null)
		        page = Request.QueryString["Page"];

	        Control contents = LoadControl("Controls/" + page + ".ascx");
	        configuratorContents.Controls.Add(contents);
        }
        </script>
    </form>
</body>
</html>
