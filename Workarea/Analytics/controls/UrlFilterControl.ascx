<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UrlFilterControl.ascx.cs" Inherits="UrlFilterControl" %>
<style type="text/css">
    .UrlFilterControl{position: relative; top: 0; left: 0;}
    .UrlFilterControl .UrlFilterContainer {position: relative; top: 0; left: 0; margin-right: 0.5em;}
    .UrlFilterControl .UrlFilters {position: absolute; top: 2.0em; right: -0.25em; border: solid 1px #D7E0E7; padding: 0.5em; background-color: #E7F0F7; text-align: left;}
    .UrlFilterControl .UrlFilters label {margin-left: 0.5em;}
    .UrlFilterControl .SelectedUrlFilter {width: 15em;}
    .UrlFilterControl .AddUrlFilter {margin-top: 1em;}
    .UrlFilterControl .AddUrlFilter .AddUrlFilterInput {width: 15em;}
    .UrlFilterControl .UrlFilteringButtonContainer img {cursor: pointer;}
    .UrlFilterControl .UrlFilterLabel {margin-right: 1em;}
    .UrlFilterControl .UrlFilterUtilityButtons {margin: 0.5em;}
    .UrlFilterControl .UrlFilterUtilityButtons .btnUrlFilterRemoveAll {margin-left: 1em;}
</style>

<script type="text/javascript">
<!--
    $(document).ready(function() {
            UrlFilterControl_UpdateUi();

            // hook ASP.NET Ajax update panel callback begin and end events as needed:
        if ("undefined" != typeof Sys.WebForms.PageRequestManager.getInstance) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(UrlFilterControl_EndCallbackHandler);
            }
        });
        
        function UrlFilterControl_EndCallbackHandler() {
            UrlFilterControl_UpdateUi();
        }

        function UrlFilterControl_UpdateUi() {
        $(".UrlFilteringButtonContainer img").click(function() {
            $(".UrlFilters").toggle();
            $(".UrlFilteringButtonContainer img").toggle();
            });
        }
// -->
</script>

<div class="UrlFilterControl">
    <asp:UpdatePanel ID="upUrlFilterContainer" runat="server" >
        <ContentTemplate>
            <div class="UrlFilterContainer">
                <span class="UrlFilterLabel"><asp:literal ID="litUrlFilterLabel" runat="server" /></span>
                <asp:TextBox ID="tbUrlFilteringStatus" runat="server" CssClass="SelectedUrlFilter"  ReadOnly="true" />
                <span class="UrlFilteringButtonContainer">
                    <img title="Down" alt="Down" class="UrlFilteringShow" src="../images/UI/Icons/arrowHeadDown.png" />
                    <img title="Up" alt="Up" class="UrlFilteringHide" src="../images/UI/Icons/arrowHeadUp.png" style="display: none;" />
                </span>
                <div class="UrlFilters" style="display: none;">
                    <h3 class="UrlFilterHeading" title="Limit results to the following URLs">
                        <asp:Literal ID="litUrlFilterHeading" runat="server" />
                    </h3>
                    <asp:CheckBoxList ToolTip="Click here to Limit results to the following URLs" ID="cbl1" runat="server" AutoPostBack="true" />
                    <div class="UrlFilterUtilityButtons">
                        <asp:LinkButton ID="btnUrlFilterAddAll" runat="server" CssClass="btnUrlFilterAddAll" />
                        <asp:LinkButton ID="btnUrlFilterRemoveAll" runat="server" CssClass="btnUrlFilterRemoveAll" />
                    </div>
                    <div class="AddUrlFilter">
                        <asp:TextBox ID="tbAddUrlFilter" runat="server" CssClass="AddUrlFilterInput" />
                        <asp:Button ID="btnAddUrlFilter" runat="server" Text="Add" CssClass="AddUrlFilterButton" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>