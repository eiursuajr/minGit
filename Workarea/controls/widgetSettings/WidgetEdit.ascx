<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WidgetEdit.ascx.cs" Inherits="Workarea_controls_widgetSettings_WidgetEdit" %>

<script type="text/javascript" language="javascript">
    function closeParentWin(){
        window.parent.$ektron('.ektronWindow').modalHide();
        return false;
    }
</script>
<style type="text/css">
    .buttonG {display: block; background-color: #f5f5f5; border: 1px solid #dedede; border-top:1px solid #eee; border-left:1px solid #eee; line-height: 100%; text-decoration: none; color: #565656; cursor: pointer; padding: .25em 1em .25em 2.25em; margin: 0 0 0 .75em; background-repeat: no-repeat;}
    .buttonInlineG {display:inline;}
    .greenHoverG:hover {background-color:#E6EFC2;border:1px solid #C6D880;color:#529214;}
    .buttonUpdateG { background-image: url(images/ui/icons/save.png); background-position:.25em center;}
    .buttonClearG {background-image: url(images/ui/icons/cancel.png); background-position:.25em center;}
    .redHoverG:hover {background-color:#fbe3e4;border:1px solid #fbc2c4;color:#d12f19;}    
</style>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
<div class="ektronPageInfo">

<asp:MultiView ID="viewset" runat="server">
    <asp:View ID="viewError" runat="server">
        <asp:Label ID="lblError" runat="server" Visible="true"></asp:Label><br />
    </asp:View>
    <asp:View ID="viewSettings" runat="server">
        <script type="text/javascript" language="javascript">
            Ektron.ready(function(){
                var text = $ektron("input.freetext");
                var numerics = $ektron("input.integer");
                var doubles = $ektron("input.double");
                var dates = $ektron("input.datetime");
                
                $ektron("input:text").addClass("ektronTextSmall");
                
                //block chars for standard input
                text.bind("keypress",function(event){
                    var k = event.keyCode ? event.keyCode : event.charCode ? event.charCode : event.which;
                    if(String.fromCharCode(k) == ">" || String.fromCharCode(k) == "<" || k == 13) {
                        return false;
                    }
                    
                });
                
                //block chars for numerics
                numerics.bind("keypress",function(event){
                    var k = event.keyCode ? event.keyCode : event.charCode ? event.charCode : event.which;
                    if(isNaN(parseInt(String.fromCharCode(k), 10)) &&
                        !(k==8 || k==37 || k==39 || k==16)) {
                        return false;
                    }
                });
                
                //block chars for doubles
                doubles.bind("keypress",function(event){
                    var k = event.keyCode ? event.keyCode : event.charCode ? event.charCode : event.which;
                    if($ektron(this).val().indexOf(".") > -1 && String.fromCharCode(k) == ".") return false;
                    if(isNaN(parseInt(String.fromCharCode(k), 10)) && !(String.fromCharCode(k) == "." ||
                        k==8 || k==37 || k==39 || k==16)) {
                        return false;
                    }
                });

                //set up datetimes
                for(var i=0; i<dates.length; i++){
                    
                    var dateel = $ektron(dates[i]);
                    var hdnfield = $ektron("input#" + dateel.attr("hiddenfield"));
                    var displayfield = dateel.parent().parent().find(".displayParsedDate");
                    dateel.keyup(function(e){
                        evaluateDate(dateel, hdnfield, displayfield);
                    });
                    evaluateDate(dateel, hdnfield, displayfield);
                }
            });
            function evaluateDate(input, hiddenfield, displayfield){
		        if (input.val().length > 0) {
			        var date = Date.parse(input.val());
			        if (date !== null) {
				        input.removeClass("accept").removeClass("validate_error");
				        displayfield.removeClass("error").addClass("accept").text(date.toString("ddd, MMM d yyyy h:mm tt"));
				        hiddenfield.val(date.toString("ddd, MMM d yyyy h:mm tt"));
			        } else {
				        input.removeClass("accept").addClass("validate_error");
				        displayfield.removeClass("accept").addClass("error").text("I can't parse that");
				        hiddenfield.val("1/1/0001 12:00:00 AM");
			        }
		        }
            }            
        </script>
        <table class="ektronGrid">
            <tr>
                <td class="label"><%=m_refMsg.GetMessage("lbl Widget Type ID")%>:</td>
                <td><asp:Label ID="lblID" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="label"><%=m_refMsg.GetMessage("lbl Widget File Name")%>:</td>
                <td><asp:Label ID="lblFilename" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="label"><%=m_refMsg.GetMessage("lbl Widget Title")%>:</td>
                <td><asp:TextBox ID="txtTitle" CssClass="ektronTextSmall" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="label"><%=m_refMsg.GetMessage("lbl Widget Label")%>:</td>
                <td><asp:TextBox ID="txtLabel" CssClass="ektronTextSmall" runat="server"></asp:TextBox></td>
            </tr>
            <tr id="GlobalSettingsRow" runat="server">
                <td colspan="2">
                    <fieldset>
                        <legend><%=m_refMsg.GetMessage("lbl Global Settings")%></legend>
                        <table>
                            <asp:Repeater ID="globalProps" runat="server" OnItemDataBound="Properties_OnItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td class="label" title="Property"><%=m_refMsg.GetMessage("generic Property")%>:</td>
                                        <td class="value padLeft"><asp:Label ID="lblPropertyName" runat="server"></asp:Label></td>
                                        <td class="label " title="Type"><%=m_refMsg.GetMessage("generic type")%>:</td>
                                        <td class="padLeft"><asp:Label ID="lblType" runat="server"></asp:Label></td>    
                                    </tr>
                                    <tr>
                                        <td class="label">Value:</td>
                                        <td class="padLeft"><asp:PlaceHolder ID="phValue" runat="server"></asp:PlaceHolder></td>
                                        <td colspan="2"><asp:PlaceHolder ID="phExtraOutput" runat="server"></asp:PlaceHolder></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>                            
                        </table>
                    </fieldset>
                </td>
            </tr>
        </table>
        <div class="ektronTopSpace">
            <ul class="buttonWrapper floatRight" style="width:100%">
                <li>
                    <button title="Cancel" class="buttonG buttonInlineG redHoverG buttonClearG" onclick="return closeParentWin();"><%=m_refMsg.GetMessage("btn cancel")%></button>
                </li>
                <li>
                    <asp:Button ToolTip="Save" CssClass="buttonG buttonInlineG greenHoverG buttonUpdateG" ID="Save" runat="server" OnClick="SaveProperties" Text="Save" />
                </li>                
            </ul>
        </div>
    </asp:View>
    <asp:View ID="viewSuccess" runat="server">
        <asp:Label ID="successmessage" runat="server" Visible="true"></asp:Label><br />
    </asp:View>
</asp:MultiView>

</div>
</div>
