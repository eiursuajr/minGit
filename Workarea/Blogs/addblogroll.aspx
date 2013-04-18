<%@ Page Language="C#" AutoEventWireup="true" CodeFile="addblogroll.aspx.cs" Inherits="blogs_addblogroll"
    ValidateRequest="false" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Blog Roll</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="ltr_js" runat="server" />
    <div class="ektronPageContainer ektronPageInfo">
        <table width="100%" class="ektronGrid ektronBorder">
            <tr>
                <td class="label" title="Link Name">
                    <asp:Literal ID="ltr_linkname" runat="server" />:
                </td>
                <td>
                    <input title="Enter Link Name here" name="editfolder_linkname" type="text" value=""
                        size="55" id="editfolder_linkname" />
                </td>
            </tr>
            <tr>
                <td class="label" title="URL">
                    <asp:Literal ID="ltr_url" runat="server" />:
                </td>
                <td>
                    <input title="Enter URL here" name="editfolder_url" type="text" value="http://" size="55"
                        id="editfolder_url" />
                </td>
            </tr>
            <tr>
                <td class="label" title="Short Description">
                    <asp:Literal ID="ltr_shortdesc" runat="server" />:
                </td>
                <td>
                    <input title="Enter Short Description here" name="editfolder_short" type="text" value=""
                        size="55" id="editfolder_short" />
                </td>
            </tr>
            <tr>
                <td class="label" title="Relationship">
                    <asp:Literal ID="ltr_rel" runat="server" />:
                </td>
                <td>
                    <input title="Enter Relationship Text here" name="editfolder_rel" type="text" value=""
                        size="45" id="editfolder_rel" />&nbsp; <a title="Edit" class="button buttonInline blueHover buttonEdit"
                            style="padding-top: .25em; padding-bottom: .25em;" href="#" onclick="window.open('xfnbuilder.aspx?field=editfolder_rel&id=0','XFNBuilder','location=0,status=0,scrollbars=0,width=625,height=350');">
                            <asp:Literal ID="ltr_edit" runat="server" />
                        </a>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
