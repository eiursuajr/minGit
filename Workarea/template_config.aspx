<%@ Page Language="C#" AutoEventWireup="true" CodeFile="template_config.aspx.cs"
    Inherits="template_config" %>

<%@ Register Src="pagebuilder/foldertree.ascx" TagName="FolderTree" TagPrefix="CMS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Select Template to Add</title>
    <!-- Styles -->
    <link type="text/css" rel="stylesheet" href="java/plugins/modal/ektron.modal.css" />
    <link type="text/css" rel="stylesheet" href="java/plugins/treeview/ektron.treeview.css" />
    <style type="text/css"  >
        .deviceheader
        {
            color: #2E6E9E;
            font-weight:bold;
            padding-left: 4px;
            padding-right: 4px;
          
        }
        .devicelabel
        {
           width:146px;
        }
        
        .widget {}
    </style>
    <!-- Scripts -->
    <script language="javascript" type="text/javascript" src="java/plugins/modal/ektron.modal.js"></script>
    <script language="javascript" type="text/javascript" src="java/plugins/treeview/ektron.treeview.js"></script>
   
    <script language="javascript" type="text/javascript">


        function disableEnterKey(event) {
            if (event.keyCode == 13) {
                return false;
            }
            return true;
        }
        function ConfirmUpdate() {
            if (confirm('Are you sure you wish to update the active list of templates to this configuration?')) {
                document.forms[0].submit();
                return true;
            }
            else {
                return false;
            }
        }

        function CancelAddTemplate() {
            //debugger;
            var notSupportIFrame = "0";
            if (notSupportIFrame == "1") {
                top.close();
            }
            else {
                if (parent.CloseChildPage == 'undefined' || parent.CloseChildPage == null) {
                    top.close();
                }
                else {
                    parent.CloseChildPage();
                }
            }
            return false;
        }

        function ConfirmNotEmpty(fieldName, deviceFieldNames)
        {
            if (deviceFieldNames != "")
            {
                var deviceField;
                var splitResult = deviceFieldNames.split(",");
                for (var i = 0; i < splitResult.length; i++)
                {
                    if ($ektron("#cbDeviceTemplate_" + splitResult[i]).attr('checked'))
                    {
                        if ($ektron("#updateDeviceTemplate_" + splitResult[i]).val() == "")
                        {
                            alert('Please provide a device template for checked items');
                            return;
                        }
                        deviceField = $ektron("#updateDeviceTemplate_" + splitResult[i]).val();

                        if ((deviceField.indexOf("\\") >= 0) || (deviceField.indexOf(":") >= 0) || (deviceField.indexOf("*") >= 0) || (deviceField.indexOf("\"") >= 0) || (deviceField.indexOf("<") >= 0) || (deviceField.indexOf(">") >= 0) || (deviceField.indexOf("|") >= 0) || (deviceField.indexOf("\'") >= 0))
                        {
                            alert("Device template name can't include ('\\', ':', '*', ' \" ', '<', '>', '|', '\'').");
                            return;
                        }
                    }
                }
            }

            var field = $ektron("#" + fieldName);
            if (!field.hasClass("masterlayout"))
            {
                if (Trim(field.val()) != '' && !field.hasClass("masterlayout"))
                {
                    field = field.val();
                    if ((field.indexOf("\\") >= 0) || (field.indexOf(":") >= 0) || (field.indexOf("*") >= 0) || (field.indexOf("\"") >= 0) || (field.indexOf("<") >= 0) || (field.indexOf(">") >= 0) || (field.indexOf("|") >= 0) || (field.indexOf("\'") >= 0))
                    {
                        alert("Template name can't include ('\\', ':', '*', ' \" ', '<', '>', '|', '\'').");
                    }
                    else
                    {
                        document.forms[0].submit();
                    }
                }
                else
                {
                    alert('<asp:Literal ID="ltrTemplateMessage" runat="server" />');
                }
            } else
            {
                document.forms[0].submit();
            }
        }

        function AddTemplateEntry(template_id, template_name) {
            var parentdoc = parent.document;
            var list = parentdoc.getElementById('addTemplate');
            if (list == null) {
                // see if it's in opener (older non-modal code)
                list = opener.document.getElementById('addTemplate');
                parentdoc = opener.document;
            }
            if (list != null) {
                var new_entry = parentdoc.createElement('option');

                new_entry.appendChild(parentdoc.createTextNode(template_name));
                new_entry.value = template_id;
                list.appendChild(new_entry);

                // select what was added so user can add it by clicking the add button
                for (var i = 0; i < list.options.length; i++) {
                    if (list.options[i].value == template_id) {
                        list.selectedIndex = i;
                        break;
                    }
                }

                if (parent.CloseChildPage == 'undefined' || parent.CloseChildPage == null) {
                    top.close();
                }
                else {
                    parent.CloseChildPage();
                }
            }
            else {
                parent.ReloadPage();
                top.close();
            }
        }

        function ReloadPage()
        {
            top.opener.document.location.reload(true);
        }
        function OpenAddDialog()
        {
            window.open('template_config.aspx?view=add', '', 'width=650,height=600,resizable=0');
        }

        function AddToEditFolderList(template_id, template_name) {
            var t_list = window.opener.document.getElementById('addTemplate');
            var selectList = document.getElementById(element_id);
            var newOption = document.createElement('option');
            newOption.value = xml_id;
            newOption.appendChild(document.createTextNode(xml_name));
            selectList.appendChild(newOption);

        }

        function ShowBrowseDialog() {

        }

        function OnPageBuilderCheckboxChanged(checked) {
            $ektron("#widgetDisplay").css("display", checked ? "block" : "none");
        }

        function SelectAllWidgets() {
            var widgets = $ektron(".widget");
            widgets.each(function(i) {
                var widget = $ektron(widgets[i]);
                var checkbox = widget.find("input");
                if (!checkbox.is(":checked")) {
                    widget.addClass("selected");
                    ToggleCheckbox(checkbox);
                }
            });
        }

        function SelectWidgets(widgetIds) {
            for (var i in widgetIds) {
                var id = widgetIds[i];
                SelectWidget(id);
            }
        }

        function SelectWidget(id) {
            var checkbox = $ektron(".widget input#widget" + id);
            var widget = checkbox.parent(".widget");
            if (!checkbox.is(":checked")) {
                widget.addClass("selected");
                ToggleCheckbox(checkbox);
            }
        }

        function UnselectAllWidgets() {
            var widgets = $ektron(".widget");
            widgets.each(function(i) {
                var widget = $ektron(widgets[i]);
                var checkbox = widget.find("input");
                if (checkbox.is(":checked")) {
                    widget.removeClass("selected");
                    ToggleCheckbox(checkbox);
                }
            });
        }

        function OnBrowseButtonClicked() {
            // show folder selection dialog
        }

        function ToggleCheckbox(checkbox) {
            if (checkbox.is(":checked")) {
                checkbox.removeAttr("checked");
            }
            else {
                checkbox.attr("checked", "checked");
            }
        }

        function PrintObject(obj) {
            var str = "";
            var count = 0;
            for (var i in obj) {
                str += i + ":" + obj[i] + "; ";

                if (++count == 10) {
                    count = 0;
                    str += "\n";
                }
            }
            alert(str);
        }

        function OnFileClicked(path)
        {
            var idBnt = $ektron("#browsebtnclicked").val();
            var input;
            if (idBnt == "")
            {
                input = $ektron("input#updateTemplate");
                if (input.length == 0)
                    input = $ektron("input#addTemplate");
                if (input.length == 0)
                    return;
            }
            else
            {
                input = $ektron("input#" + idBnt);
            }
            $ektron("#browsebtnclicked").val("");
            input.attr("value", path.replace(/\\/g, "/"));
            $ektron("#dlgBrowse").modal().modalHide();
        }

        Ektron.ready(function () {
        	// add ektronPageHeader since baseClass doesn't add it for us
        	// ...base class adds ektronPageHeader now
            // $ektron("table.baseClassToolbar").wrap("<div class='ektronPageHeader'></div>");

            // Initialize browse dialog
            $ektron("#dlgBrowse").modal({ modal: true, onShow: function (h) {
                $ektron("#dlgBrowse").css("margin-top", -1 * Math.round($ektron("#dlgBrowse").outerHeight() / 2)); h.w.show();
            }
            });

            // Initialize PageBuilder checkbox
            var checkbox = $ektron(".pageBuilderCheckbox input");

            OnPageBuilderCheckboxChanged(checkbox.is(':checked'));

            checkbox.click(function () {
                OnPageBuilderCheckboxChanged($ektron(".pageBuilderCheckbox input").is(':checked'));
            });

            // Initialize widgetType display

            var widgets = $ektron(".widget");
            widgets.each(function (i) {
                var widget = $(widgets[i]);
                if (widget.find("input").is(":checked")) {
                    widget.addClass("selected");
                }

                widget.click(function () {

                    var widgetCheckbox = widget.find("input");

                    ToggleCheckbox(widgetCheckbox);
                    if (widgetCheckbox.is(":checked")) {
                        widget.addClass("selected");
                    }
                    else {
                        widget.removeClass("selected");
                    }
                });
            });
        });

        function SetBtnClicked(id)
        {
            $ektron("#browsebtnclicked").val("updateDeviceTemplate_" + id);
        }
    </script>

</head>
<body>
    <form id="form1" method="post" enctype="multipart/form-data" class="newTemplateForm"
    runat="server">
    <div id="dlgBrowse" class="ektronWindow ektronModalStandard">
        <div class="ektronModalHeader">
            <h3>
                <asp:Literal ID="selectTemplate" runat="server" />
                <asp:HyperLink ToolTip="Close" ID="closeDialogLink" CssClass="ektronModalClose" runat="server" />
            </h3>
        </div>
        <div class="ektronModalBody">
            <div class="folderTree">
                <CMS:FolderTree ID="folderTree" runat="server" Filter="[.]aspx$" />
            </div>
        </div>
    </div>
    <asp:Literal ID="MainBody" runat="server" />
    <div class="formBody">
        <asp:Panel ID="pnlPageBuilder" runat="server" Visible="False">
            <div class="ektronPageContainer ektronPageInfo">
                <table>
                    <tbody>
                        <tr>
                            <td class="label">
                                <label for="addTemplate" title="<%= lblTemplateFile.Text%>: <%= m_refContentApi.SitePath%>">
                                    <asp:Literal ID="lblTemplateFile" runat="server" />:</label>
                            </td>
                            <asp:Literal ID="sitePathValue" runat="server" />
                            <td class="value" id="browsebuttontd" runat="server">
                                <input title="Browse" type="button" value="..." class="ektronModal browseButton"
                                    onclick="OnBrowseButtonClicked()" />
                            </td>
                        </tr>
                        <asp:Literal ID="ltrTemplateDetails" runat="server"></asp:Literal>
                        <tr>
                            <td class="label" colspan="2">
                                <asp:CheckBox ToolTip="Page Builder Wireframe" ID="cbPageBuilderTemplate" runat="server"
                                    CssClass="pageBuilderCheckbox clearfix" />
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div id="pageBuilderInfo" runat="server">
                    <div id="widgetDisplay" class="templateConfig" style="display: none">
                        <fieldset>
                            <legend>
                                <asp:Literal ID="lblSelectWidgets" runat="server" /></legend>
                            <div class="widgetsHeader ui-helper-clearfix">
                                <h4>
                                    <asp:Literal ID="widgetTitle" runat="server" /></h4>
                                <ul id="widgetActions" class="buttonWrapper clearfix">
                                    <li>
                                        <asp:LinkButton ID="btnSelectNone" runat="server" CssClass="redHover button selectNoneButton buttonLeft"
                                            OnClientClick="UnselectAllWidgets();return false;" /></li>
                                    <li>
                                        <asp:LinkButton ID="btnSelectAll" runat="server" CssClass="greenHover button selectAllButton buttonRight"
                                            OnClientClick="SelectAllWidgets();return false;" /></li>
                                </ul>
                            </div>
                            <div id="widgets">
                                <ul id="widgetList">
                                    <asp:Repeater ID="repWidgetTypes" runat="server">
                                        <ItemTemplate>
                                            <li>
                                                <div class="widget">
                                                    <input type="checkbox" name="widget<%# ((Ektron.Cms.Widget.WidgetTypeData)Container.DataItem).ID %>"
                                                        id="widget<%#((Ektron.Cms.Widget.WidgetTypeData)Container.DataItem).ID %>" /><img
                                                            src="<%#m_siteApi.SitePath + "widgets/" + ((Ektron.Cms.Widget.WidgetTypeData)Container.DataItem).ControlURL + ".jpg"%>"
                                                            alt="<%#((Ektron.Cms.Widget.WidgetTypeData)Container.DataItem).Title%>" title="<%#((Ektron.Cms.Widget.WidgetTypeData)Container.DataItem).Title%>" /><div
                                                                class="widgetTitle" title="<%#((Ektron.Cms.Widget.WidgetTypeData)Container.DataItem).Title%>">
                                                                <%#((Ektron.Cms.Widget.WidgetTypeData)Container.DataItem).Title%></div>
                                                </div>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </div>
                        </fieldset>
                    </div>
                    <asp:Label ID="lbStatus" runat="server" />
                </div>
                <table>
                      <tbody>
                            <asp:Literal ID="ltrDeviceConfigurations" runat="server" ></asp:Literal>
                      </tbody>
                </table>
            </div>
        </asp:Panel>
    </div>
    <input type="hidden" name="browsebtnclicked" id="browsebtnclicked" value="" />
    </form>
</body>
</html>
