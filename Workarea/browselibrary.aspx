<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BrowseLibrary.aspx.cs" Inherits="Workarea_BrowseLibrary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <%=AppUI.AppName + " " + MsgHelper.GetMessage("library page html title") + " " + (Ektron.Cms.CommonApi.GetEcmCookie()["username"])%>
    </title>

    <script type="text/javascript">
    <!--    //--><![CDATA[//><!--
    var inserter = false;

    function SetLoadStatus(Page) {
        if (Page.toLowerCase() == "inserter") {
            inserter = true;
        }
    }

    function GetLoadStatus() {
        if (inserter == true) {
            return true;
        }
        return false;
    }
    //--><!]]>

    </script>

</head>
<frameset cols="300,*" border="5" class="library">
        <frame name="medialist" id="frmMediaList" runat="server" marginheight="2" scrolling="auto" frameborder="1"></frame> 	
		<frame name="libraryinsert" id="frmLibraryInsert"  runat="server" marginwidth="2" marginheight="2" scrolling="auto" frameborder="1"></frame> 	
	</frameset>
</html>
