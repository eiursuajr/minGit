<%@ Page Language="C#" AutoEventWireup="true" CodeFile="addcatalogimage.aspx.cs"
    Inherits="Commerce_addcatalogimage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <script type="text/javascript" language="javascript">
        	
		function IsExtensionValid() {
		            var filename = document.getElementById('ul_image').value;
		            var ExtensionList = "<%=lib_settings_data.ImageExtensions %>;"
		            var strRet = '';
					
					if (ExtensionList.length > 0) {
						var ExtensionArray = ExtensionList.split(",");
						var FileExtension = filename.split(".");
						for (var i = 0; i < ExtensionArray.length; i++) {
							if (FileExtension[FileExtension.length - 1].toLowerCase() == Trim(ExtensionArray[i].toLowerCase())) {
								strRet='true';
							}
						}
						if( strRet != 'true')
						{
						    alert('<%=m_refContentApi.EkMsgRef.GetMessage("invalid file extension")%>');
						    return false;
						}
					}
				}
		
		
    </script>
</head>
<body>
    <form id="form1" runat="server">
        
        	<asp:Literal ID="ltr_topjs" runat="server"></asp:Literal>
        
        <div id="dvPage" runat="server">
            <table>
                <tr>
                    <td align="right">
                        <img alt="" title="<% =GetMessage("alt media title") %>" src="../images/application/Commerce/about.gif" />&nbsp;
                        <label id="lblTitle" runat="server">
                        </label>
                    </td>
                    <td>
                        <input runat="server" id="txtTitle" type="text" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <img alt="" title="<% =GetMessage("alt media alt")%>" src="../images/application/Commerce/about.gif" />&nbsp;
                        <label id="lblaltTitle" runat="server">
                            </label>
                    </td>
                    <td>
                        <input runat="server" id="altTitle" type="text" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <img alt="" title="<% =GetMessage("alt media file upload") %>"Browse to the image to be uploaded." src="../images/application/Commerce/about.gif" />&nbsp;
                        <label id="lblFullSize" runat="server">
                            :</label>
                    </td>
                    <td>
                        <asp:FileUpload ID="ul_image" runat="server" />
                    </td>
                </tr>
            </table>
            <asp:Literal runat="server" ID="ltr_pixel" />
            <br />
            <div>
                <asp:Button runat="server" ID="btnUpload"  OnClick="btnUpload_Click"/>
            </div>
            <div style="visibility: hidden;" id="dvHoldMessage">
		        <strong><asp:Literal ID="ltr_uploadmsg" runat="Server"></asp:Literal></strong>
	        </div>
	        <div style="visibility: hidden;" id="dvErrorMessage">
		        <span class="important"><strong><asp:Literal ID="ltr_error" runat="Server"></asp:Literal></strong></span>
	        </div>
        </div>

        <script language="javaScript" type="text/javascript">
            <asp:Literal ID="ltr_bottomjs" runat="server"></asp:Literal>
            <asp:Literal ID="ltrAddMediaJS" runat="server"></asp:Literal>
        </script>

    </form>
</body>
</html>

