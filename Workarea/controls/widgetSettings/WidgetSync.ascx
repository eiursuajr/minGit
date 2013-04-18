<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WidgetSync.ascx.cs" Inherits="Workarea_controls_widgetSettings_WidgetSync" %>

<script type="text/javascript">
    function SyncWidgets()
    {
        if (confirm('<%=String.Format((string) (m_refMsg.GetMessage("js confirm sync widgets")), m_refContentApi.RequestInformationRef.WidgetsPath)%>'))
        {
            document.forms[0].submit();
        }

        return false;
    }
    function showEditWidget(id){
        $ektron('#WidgetEditIframe')[0].src='widgetsettings.aspx?action=widgetedit&widgetid=' + id;
        $ektron('#editWidget').modalShow();
        $ektron('.ektronModalStandard div.ektronModalBody').css({
            "margin" : "0px",
            "padding" : "0px"
        });
    }
</script>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <asp:DataGrid ID="grdWidgets"
        runat="server"
        Width="100%"
        EnableViewState="False"
        AutoGenerateColumns="False"
        CssClass="ektronGrid"
        GridLines="None">
        <Columns>
            <asp:TemplateColumn HeaderText="Widget">
                <ItemTemplate>
                    <a class="widgetedit" title="Edit <%#(Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).ControlURL%>" href="#" onclick="showEditWidget('<%#(Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).ID%>');">&nbsp;</a>
                    <asp:Label ID="lblControlURL" ToolTip='<%# DataBinder.Eval(Container.DataItem, "ControlURL")%>'
                       Text='<%# DataBinder.Eval(Container.DataItem, "ControlURL")%>'
                       runat="server"/>


                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
        <HeaderStyle CssClass="title-header" />
    </asp:DataGrid>
    <asp:Label ID="lblNoWidgets" runat="server" Visible="false" />
</div>
<div id="editWidget" class="ektronWindow ektronModalStandard">
    <div class="ektronModalHeader"><h3><span title="Editing Widget">Editing Widget</span><a title="Close" href="#" class="ektronModalClose">Close</a></h3></div>
    <div class="ektronModalBody">
        <iframe style="z-index:-1; width:100%; height:400px; border:0px;" id="WidgetEditIframe" src="">
        </iframe>
    </div>
</div>

<script type="text/javascript">
    Ektron.ready(function(){

        // EDIT WIDGET MODAL
        $ektron("#editWidget").modal(
        {
            trigger: '',
            modal: true,
            toTop: true,
            onShow: function(hash)
            {
                hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");
                hash.o.fadeIn();
                hash.w.fadeIn();
            },
            onHide: function(hash)
            {
                hash.w.fadeOut("fast");
                hash.o.fadeOut("fast", function()
                {
                    if (hash.o)
                    {
                        hash.o.remove();
                    }
                });
            }
        });
    });
</script>

