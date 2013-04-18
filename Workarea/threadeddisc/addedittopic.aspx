<%@ Page Language="C#" AutoEventWireup="true" CodeFile="addedittopic.aspx.cs" Inherits="threadeddisc_addedittopic" ValidateRequest="false" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="../controls/Editor/ContentDesignerWithValidator.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <style type="text/css">
        div#TreeOutput { position:relative; background-color: white; }
    </style>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
			    function CanNavigate() {
				    // Block navigation while this page loaded (called from top window-object):
				    return false;
			    }
                function CanShowNavTree() {
				    // Block displaying the navigation tree while this page loaded (called from top window-object):
				    return false;
			    }
              //--><!]]>
        </script>
</head>
<body>
    <asp:Literal ID="ltr_js" runat="server"/>
    <form id="frmMain" runat="server">
<div class="ektronPageContainer ektronPageTabbed">
    <div class="tabContainerWrapper">
        <div class="tabContainer">
            <ul>
                <li>
                    <a title="Properties" href="#dvProp">
                        <asp:literal ID="litTabProp" runat="server" />
                    </a>
                </li>
                <li>
                    <a title="Summary" href="#dvSumm">
                        <asp:literal ID="litTabSumm" runat="server" />
                    </a>
                </li>
                <li>
                    <a title="Categories" href="#dvCategories">
                        <asp:literal ID="litTabCategories" runat="server" />
                    </a>
                </li>
            </ul>
        <div id="dvProp">
            <table class="ektronGrid">
                <tr>
                    <td class="label" title="Subject"><asp:Literal ID="ltr_title" runat="server"/></td>
                    <td class="value"><asp:TextBox ToolTip="Enter Subject here" ID="txt_topic_title" runat="server" MaxLength="200" /></td>
                </tr>
                <tr id="tr_createddate" visible="false" runat="server">
                    <td class="label" title="Date Created"><asp:Literal ID="ltr_created" runat="server"/></td>
                    <td class="readOnlyValue"><asp:Literal ID="ltr_created_data" runat="server"/></td>
                </tr>
                <tr id="tr_createdby" visible="false" runat="server">
                    <td class="label" title="Created By"><asp:Literal ID="ltr_created_by" runat="server"/></td>
                    <td class="readOnlyValue"><asp:Literal ID="ltr_created_by_data" runat="server"/></td>
                </tr>
                <tr id="tr_priority" runat="server">
                    <td class="label" valign="top"><asp:Literal ID="ltr_priority" runat="server"/></td>
                    <td class="value">
                        <asp:RadioButton ToolTip="Announcement" ID="rb_announcement" runat="server" GroupName="grp_priority" />
                        <br />
                        <asp:RadioButton ToolTip="Sticky" ID="rb_sticky" runat="server" GroupName="grp_priority" />
                        <br />
                        <asp:RadioButton ToolTip="Normal" ID="rb_normal" runat="server" GroupName="grp_priority" />
                    </td>
                </tr>
                <tr id="tr_lock" runat="server">
                    <td class="label" title="Lock Topic"><asp:Literal ID="ltr_adt_lock" runat="server"/>:</td>
                    <td class="value"><asp:CheckBox ToolTip="Lock Topic Option" ID="chk_adt_lock" runat="server" Checked="False" /></td>
                </tr>
                <tr id="tr_desc" runat="server" visible="false">
                    <td class="label" title="Message"><asp:Literal ID="ltr_desc" runat="server"/></td>
                    <td class="value"><asp:Panel ID="pnl_message_editor" runat="server"/></td>
                </tr>
            </table>
        </div>
        <div id="dvSumm">
            <table class="ektronGrid">
                <tr>
                    <td class="label" title="Summary"><asp:Literal ID="ltr_summ" runat="server"/>:</td>
                    <td class="value"><asp:TextBox ToolTip="Enter Summary here" ID="txt_summ" runat="server" TextMode="MultiLine"/></td>
                </tr>
            </table>
        </div>
        <div id="dvCategories">
            <asp:Literal runat="server" ID="EditTaxonomyHtml"/>
            <div id="TreeOutput"></div>
            <input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
            <input type="hidden" name="LastClickedOnChecked" id="LastClickedOnChecked" value="false" />
            <input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value="" runat="server" />
            <input type="hidden" id="data" name="data" value="" runat="server" />
            <input type="hidden" id="hdn_uniqueId" name="uniqueId" value="" runat="server" />
        </div>
        <asp:HiddenField ID="hdn_action" runat="server" />
        <asp:HiddenField ID="hdn_folderid" runat="server" />
        <asp:HiddenField ID="hdn_topicid" runat="server" />
        <asp:HiddenField ID="hdn_prior" runat="server" Value="0" />
        </div>
    </div>
</div>
    </form>
</body>
</html>

