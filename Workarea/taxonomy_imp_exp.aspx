<%@ Page Language="C#" AutoEventWireup="true" CodeFile="taxonomy_imp_exp.aspx.cs"
    ValidateRequest="false" Inherits="taxonomy_imp_exp" %>

<%  if (m_strPageAction != "export") {%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Taxonomy import.</title>
    <asp:Literal ID="displaystylesheet" runat="server" />

    <script type="text/javascript">
        // Begin: Initialize the ResourceText Strings

        var ResourceText = {
            jsTitleIsRequiredField: '<asp:Literal id="ltr_titleRequired" runat="server" />',
            jsFileOrXMLSelectionIsRequired: '<asp:Literal id="ltr_fileOrXMLRequired" runat="server" />',
            jsInvalidInputUseFileOrXMLAsYourImportOption: '<asp:Literal id="ltr_invalidInputOrXML" runat="server" />',
            jsInvalidFileSpecifiedPleaseImportAFileWithXMLExtension: '<asp:Literal id="ltr_invalidExtension" runat="server" />',
            jsPleaseEnterTheValidFilePath: '<asp:Literal id="ltr_enterValidFilePath" runat="server" />'
        }

        // End: Initializing the ResourceText Strings
        function LoadLanguage(num) {
            document.location.href = "taxonomy_imp_exp.aspx?LangType=" + num;
        }
        function Import() {
            if (document.getElementById('txttitle').value == '') { alert(ResourceText.jsTitleIsRequiredField); }
            else {
                if (document.getElementById('fileimport').value == '' && document.getElementById('textimport').value == '') {
                    alert(ResourceText.jsFileOrXMLSelectionIsRequired);
                }
                else if (document.getElementById('fileimport').value != '' && document.getElementById('textimport').value != '') {
                    alert(ResourceText.jsInvalidInputUseFileOrXMLAsYourImportOption);
                }
                else {
                    if (document.getElementById('fileimport').value != '') {
                        var FileExtension = (document.getElementById('fileimport').value).split(".");
                        if (FileExtension[FileExtension.length - 1] != "xml") {
                            alert(ResourceText.jsInvalidFileSpecifiedPleaseImportAFileWithXMLExtension);
                        }
                        else {
                            try {
                                document.forms[0].submit();
                            } catch (e) {
                                alert(ResourceText.jsPleaseEnterTheValidFilePath);
                                window.location.reload(true);
                                return false;
                            }
                            return false;
                        }
                    }
                    else { document.forms[0].submit(); return false; }
                }
            }
        }
        function ClearErr() {
            document.getElementById('errmsg').innerHTML = '';
        }
    </script>

</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
    <div>
        <div id="dhtmltooltip">
        </div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server">
            </div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <table class="ektronForm">
                <tr>
                    <td class="label" title="Title">
                        <%=m_refMsg.GetMessage("generic title")%>:
                    </td>
                    <td class="value">
                        <asp:TextBox ToolTip="Enter Title here" ID="txttitle" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="label" title="Files">
                        <%=m_refMsg.GetMessage("generic files")%>:
                    </td>
                    <td>
                        <asp:FileUpload ToolTip="Select File to Import" ID="fileimport" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td title="or">
                        <%=m_refMsg.GetMessage("lbl or")%>
                    </td>
                </tr>
                <tr>
                    <td class="label" title="XML">
                        <%=m_refMsg.GetMessage("lbl XML")%>:
                    </td>
                    <td>
                        <asp:TextBox ToolTip="Enter XML Text here" ID="textimport" runat="server" TextMode="MultiLine" />
                    </td>
                </tr>
                <tr id="tr_config" runat="server">
                    <td class="label" title="Configuration">
                        <%=m_refMsg.GetMessage("config page html title")%>:
                    </td>
                    <td>
                        <asp:CheckBox ToolTip="Content" ID="chkConfigContent" Checked="true" runat="server"
                            Text="Content" />
                        <br />
                        <asp:CheckBox ToolTip="User" ID="chkConfigUser" runat="server" Text="User" />
                        <br />
                        <asp:CheckBox ToolTip="Group" ID="chkConfigGroup" runat="server" Text="Group" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
<% }%>
