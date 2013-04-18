<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Notificationmessages.aspx.cs"
    Inherits="Workarea_Notificationmessages" %>

<%@ Register TagPrefix="ektron" TagName="ContentDesigner" Src="../controls/Editor/ContentDesignerWithValidator.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Notification Message Page</title>
    <asp:Literal ID="StyleSheetJS" runat="server" />

    <script type="text/javascript">

        function SubmitForm(FormName, Validate) {

            resetCPostback();
            if (Validate.length > 0) {
                if (eval(Validate)) {
                    document.forms[FormName].submit();
                    return false;
                }
                else {
                    return false;
                }
            }
            else {
                document.forms[FormName].submit();
                return false;
            }
        }
        function resetCPostback() {
            document.forms["form1"].isCPostData.value = "";
        }
        function VerifyAddNotificationMsg() {
            var es = '';
            if (document.forms.form1.txtTitle.value == '') {
                es += '<asp:Literal id="ltr_titleErr" runat="server" />\n';
            }
            if (document.forms.form1.txtSubject.value == '') {
                es += '<asp:Literal id="ltr_subErr" runat="server" />\n';
            }
            if (document.forms.form1.txtPlainText.value == '' && Ektron.ContentDesigner.instances["txtTextAddEdit"].getContent("text") == '') {
                es += '<asp:Literal id="ltr_bodyErr" runat="server" />\n';
            }
            if (es != '') {
                alert('<asp:Literal id="ltr_follErr" runat="server" />' + es); return false;
            }
            else {
                return true;
            }
        }
        function ConfirmDelete() {

            return (confirm('<asp:literal id="delSubScriptionMsg" runat="server"/>'));
        }

    </script>

    <asp:Literal ID="jsEditorScripts" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="dhtmltooltip">
    </div>
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" id="divTitleBar" runat="server">
        </div>
        <div class="ektronToolbar" id="divToolBar" runat="server">
        </div>
    </div>
    <div class="ektronPageContainer">
        <div id="AddNewMessage" runat="server">
            <div class="ektronPageInfo">
                <table class="ektronGrid">
                    <tbody>
                        <tr>
                            <td class="label" title="Title">
                                <%=msgHelper.GetMessage("generic Title")%>:
                            </td>
                            <td class="value">
                                <asp:TextBox ToolTip="Enter Title here" ID="txtTitle" runat="server" MaxLength="255" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Label ToolTip="Id" ID="lblId" runat="server" />
                            </td>
                            <td class="readOnlyValue">
                                <asp:Literal ID="ltrViewID" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" title="Type">
                                <%=msgHelper.GetMessage("generic Type")%>:
                            </td>
                            <td class="value">
                                <asp:DropDownList ToolTip="Select Type from the Drop Down Menu" ID="ddlType" runat="server"
                                    AutoPostBack="true" OnSelectedIndexChanged="LoadSubTypeList">
                                    <asp:ListItem Text="User" Value="UserActivity" />
                                    <asp:ListItem Text="Community Group" Value="GroupActivity" />
                                    <asp:ListItem Text="General Notifications" Value="Notifications" />
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" title="Sub Type">
                                <%=msgHelper.GetMessage("generic subtype")%>:
                            </td>
                            <td class="value">
                                <asp:DropDownList ToolTip="Select SubType from the Drop Down Menu" ID="ddlSubType"
                                    runat="server" AutoPostBack="true" OnSelectedIndexChanged="LoadAllTokenList" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" title="Tokens">
                                <%=msgHelper.GetMessage("lbl tokens")%>:
                            </td>
                            <td class="value">
                                <p id="paraTokenList" style="overflow: auto; height: 55px; width: 250px;">
                                    <%=strtokenList%></p>
                            </td>
                        </tr>
                        <tr id="tr_defaultView" runat="server">
                            <td class="label" title="Default View">
                                <asp:Literal ID="ltrDefault" Text="" runat="server" />:
                            </td>
                            <td class="value">
                                <asp:CheckBox ToolTip="Enable/Disable Default View" ID="chkDefault" runat="server" />
                            </td>
                        </tr>
                        <tr id="rowSubjectView" runat="server">
                            <td class="label" title="Subject">
                                <%=msgHelper.GetMessage("generic subject label")%>
                            </td>
                            <td class="value">
                                <asp:TextBox ToolTip="Enter Subject here" ID="txtSubject" runat="server" MaxLength="255"
                                    Rows="50" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" title="Text">
                                <%=msgHelper.GetMessage("lbl Text")%>
                            </td>
                            <td class="value" valign="top">
                                <input type="hidden" name="ephox" id="ephox" value="false" />
                                <asp:PlaceHolder ID="phContentDesigner" runat="server" />
                                <br />
                                <asp:Literal ID="viewContentHTML" runat="server" />
                            </td>
                        </tr>
                        <tr runat="server">
                            <td class="label" title="Plain Text">
                                <%=msgHelper.GetMessage("generic plaintext")%>:
                            </td>
                            <td class="value">
                                <asp:TextBox ToolTip="Enter Plain Text here" ID="txtPlainText" runat="server" MaxLength="160"
                                    Rows="50" Width="300px" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div id="ViewAllMessageGrid" runat="server">
            <div>
                <asp:GridView ID="ViewMessageGrid" runat="server" AutoGenerateColumns="False" CssClass="ektronGrid" ViewStateMode="Disabled">
                    <HeaderStyle CssClass="title-header" />
                </asp:GridView>
                <p class="pageLinks">
                    <asp:Label runat="server" ID="PageLabel" ToolTip="Page"><%=msgHelper.GetMessage("page lbl")%></asp:Label>
                    <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                    <asp:Label runat="server" ID="OfLabel" ToolTip="of"> <%=msgHelper.GetMessage("lbl of")%></asp:Label>
                    <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
                    <input type="hidden" runat="server" name="hdnUnit" value="hidden" id="hdnUnit" />
                    <input type="hidden" runat="server" name="hdnCurrentPage" value="hidden" id="hdnCurrentPage" />
                </p>
                <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage"
                    Text="[First Page]" OnCommand="NavigationLink_Click" CommandName="First" />
                <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="PreviousPage1"
                    Text="[Previous Page]" OnCommand="NavigationLink_Click" CommandName="Prev" />
                <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage"
                    Text="[Next Page]" OnCommand="NavigationLink_Click" CommandName="Next" />
                <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage"
                    Text="[Last Page]" OnCommand="NavigationLink_Click" CommandName="Last" />
                <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
            </div>
        </div>
        <input type="hidden" runat="server" id="isCPostData" value="false" />
    </div>
    </form>
</body>
</html>
