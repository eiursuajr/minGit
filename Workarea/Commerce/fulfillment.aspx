<%@ Page Language="C#" AutoEventWireup="true" CodeFile="fulfillment.aspx.cs" Inherits="Commerce_fulfillment" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    <title>Fulfillment</title>
    <asp:literal id="ltr_js" runat="server" />
    <script type="text/javascript">

        var allowOpen = false;

        function resetCPostback()
	    {
            document.forms["form1"].isCPostData.value = "";
        }
        function resetPostback()
        {
            document.forms[0].isPostData.value = "";
        }
        function CaptureOrder()
        {
            if (confirm('<asp:Literal id="ltr_captOrder" runat="server" />'))
            {
                document.getElementById('hdn_code').value = 2;
                document.forms[0].submit();
            }
        }

        function CaptureOrderAndSettle() {
            if (confirm('<asp:Literal id="ltr_msgconfirmCaptureSettle" runat="server" />')) {
                document.getElementById('hdn_code').value = 7;
                document.forms[0].submit();
            }
        }
        function SettlePayment() {
            if (confirm('<asp:Literal id="ltr_msgconfirmSettle" runat="server" />')) {
                document.getElementById('hdn_code').value = 8;
                document.forms[0].submit();
            }
        }

        function EditNotes()
        {
             if(document.getElementById('txt_ordernotes').value.indexOf('>')>-1 || document.getElementById('txt_ordernotes').value.indexOf('<')>-1)
             {
                 var errMessage=document.getElementById('hdn_errOrderNotesMessage').value ;
                 alert (errMessage);
                 return false;
             }
            if (confirm('<asp:Literal id="ltr_editNotes" runat="server" />'))
            {
                document.getElementById('hdn_code').value = 6;
                document.forms[0].submit();
            }
        }

        function MarkAsFraud()
        {
            if (confirm('<asp:Literal id="ltr_markFraud" runat="server" />'))
            {
                document.getElementById('hdn_code').value = 3;
                document.forms[0].submit();
            }
        }

        function AddTrackingNumber()
        {
            //ektb_show('','#EkTB_inline?height=200&width=500&inlineId=AddTrackingNumber&modal=true', null);return false;
            ektb_show('<asp:Literal id="ltr_addTrackNumb" runat="server" />', 'fulfillment.aspx?id=<asp:Literal id="ltr_mIID" runat="server" />&action=trackingnumber&EkTB_iframe=true&width=500&height=200&scrolling=true&modal=true', false);
        }

        function ShowOrderIdSearch()
        {
            ektb_show('<asp:Literal id="ltr_showidsearch" runat="server" />', '#EkTB_inline?height=200&width=500&inlineId=SearchOrderId&modal=true', false, ''); return false;
        }

        function SearchByOrderId()
        {

            var orderId = document.getElementById('orderId').value;

            if (orderId > 0)
            {

                document.getElementById('orderId').value = '';
                window.location='fulfillment.aspx?action=vieworder&id=' + orderId;
                ektb_remove();

            }
            else
                alert('<asp:Literal id="ltr_searchid" runat="server" />');
            return false;

        }

        function SubmitTrackingNumber()
        {
            var errorMessage="Tracking Number "+ document.getElementById('hdn_errorMessage').value;
            if(document.getElementById('txt_trackingnumber').value.indexOf('<')>-1 ||document.getElementById('txt_trackingnumber').value.indexOf('>')>-1)
            {alert(errorMessage);return false;}
            document.getElementById('hdn_code').value = 3;
            document.forms[0].submit();
        }

        function EktronUiDialogInit(task, id, id2) 
        {
            switch (task.toLowerCase())
            {
                case "notes":
                    setTimeout( function() 
                    {
                        $ektron(document).find(".uxEditNotesIframe").attr("src", "fulfillment.aspx?action=editnotes&id=" + id + "&width=350&height=250&scrolling=true&modal=true");
                    }, 0);
                    break;
                case "showpayment":
                    setTimeout( function() 
                    {
                        $ektron(document).find(".uxPaymentIframe").attr("src", "fulfillment.aspx?action=showpayment&orderid=" + id + "&id=" + id2 + "&width=500&height=300&scrolling=true&modal=true");
                    }, 0);
                    break;
                case "email":
                    setTimeout( function() 
                    {
                        $ektron(document).find(".uxEmailIframe").attr("src", "../email.aspx?userarray=" + id + "&fromModal=true&width=500&height=600&scrolling=true&modal=true");
                    }, 0);
                    break;
                case "coupon":
                    setTimeout( function() 
                    {
                        $ektron(document).find(".uxCouponIframe").attr("src", "Coupons/Properties/properties.aspx?fromModal=true&couponId=" + id + "&width=500&height=250&scrolling=true&modal=true");
                    }, 0);
                    break;
                default:
                    break;
            }
        }

        function EktronUiDialogClose(task)
        {
            switch (task.toLowerCase())
            {
                case "email":
                    var jsEmailDlgId = "<asp:literal id="jsUxDialogSelectorTxt" runat="server"/>";
                    $ektron(jsEmailDlgId).dialog('close'); 
                    break;
                case "coupon":
                    var jsCouponDlgId = "<asp:literal id="jsUxCouponDlgSelectorTxt" runat="server"/>";
                    $ektron(jsCouponDlgId).dialog('close'); 
                    break;
                default:
                    break;
            }
            return false;
        }
    </script>

    <style type="text/css">
        .btnOk { line-height: 16px !important;padding-top: .2em !important; padding-bottom: .2em !important;}
        img.workflowimage { border: 0; }
        a.btnEdit { padding-top: .2em !important; padding-bottom: .2em !important;line-height: 16pt !important; display:inline-block; text-decoration: none !important; }
        div.workflowimgwrapper{ max-height:600px; height:600px; height:auto !important; overflow:auto; }
        label.paymentLabel { color: #1d5987; font-weight: bold; text-align: left; white-space: nowrap; width: 10%; }
        label.couponLabel { color: #1d5987; font-weight: bold; text-align: left; white-space: nowrap; width: 10%; }
        div#pnl_notes > textarea {margin: 0.5em !important;}
        /*div.ektronPageHeader {padding-top: 0 !important;}*/
    </style>
    <!--[if IE 6]>
    <style type="text/css">
        #wfImgIframe {width: 98%;}
    </style>
    <![endif]-->
</head>
<body class="UiMain" onclick="MenuUtil.hide()" onload="setTimeout('allowOpen = true;', 5000);" >
    <form id="form1" runat="server">
        <!--Workflow image modal-->
        <div style="visibility:hidden; position:absolute">
            <div id="dvHoldMessage">
                <table class="wrapper">
                    <tr>
                        <td>
                            <h3><strong><asp:Literal ID="ltr_holdmsg" runat="server" /></strong></h3>
                        </td>
                    </tr>
                </table>
            </div>
         </div>
         <div id="SearchOrderId" style="visibility:hidden; position:absolute">
             <table class="ektronGrid">
                 <tr class="stripe">
                     <td>
                        <div class="ektronTopSpace">
                         <asp:TextBox ID="orderId" runat="server" />
                         <a title="Ok" class="button buttonInline greenHover buttonOk btnOk" onclick="return SearchByOrderId();">
                             <asp:Literal ID="lbl_ok" runat="server" />
                         </a>
                         </div>
                     </td>
                 </tr>
             </table>
         </div>
        <div id="htmToolBar" runat="server"></div>
        <ektronUI:Dialog ID="uxCouponDialog" CssClass="EktronCouponUI" Width="500" Height="600" Modal="true" Title="" runat="server">
            <ContentTemplate>
            <iframe class="uxCouponIframe" frameborder="0" border="0"  ID="uxCouponIframe" scrolling="auto" runat="server" height="100%" width="100%"></iframe>	
            </ContentTemplate>
        </ektronUI:Dialog>
        <ektronUI:Dialog ID="uxEmailDialog" CssClass="EktronEmailUI" Width="500" Height="600" Modal="true" Title="" runat="server">
            <ContentTemplate>
            <iframe class="uxEmailIframe" frameborder="0" border="0"  ID="uxEmailIframe" scrolling="no" runat="server" height="100%" width="100%"></iframe>	
            </ContentTemplate>
        </ektronUI:Dialog>
        <ektronUI:Dialog ID="uxNotesDialog" CssClass="EktronEditNotesUI" Width="450" Height="350" Modal="true" Title="" runat="server">
            <ContentTemplate>
            <iframe class="uxEditNotesIframe" frameborder="0" border="0"  ID="uxEditNotesIframe" scrolling="no" runat="server" height="100%" width="100%"></iframe>	
            </ContentTemplate>
        </ektronUI:Dialog>
        <ektronUI:Dialog ID="uxPaymentDialog" CssClass="EktronPaymentUI" Width="500" Height="300" Modal="true" Title="" runat="server">
            <ContentTemplate>
            <iframe class="uxPaymentIframe" frameborder="0" border="0"  ID="uxPaymentIframe" scrolling="no" runat="server" height="100%" width="100%"></iframe>	
            </ContentTemplate>
        </ektronUI:Dialog>
        <div class="ektronPageContainer ektronPageTabbed">
            <asp:Panel ID="pnl_notes" runat="server" Visible="false" CssClass="">
                <asp:HiddenField ID="hdn_errOrderNotesMessage" runat="server" />
                <asp:TextBox ID="txt_ordernotes" runat="server" TextMode="MultiLine" Rows="8" Columns="50" /><br />
            </asp:Panel>
            <asp:Panel ID="pnl_payment" runat="server" Visible="false">
                <div id="dvPaymentStatus"></div>
                <div id="Div1">
                    <table class="ektronGrid" runat="server">
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_transactionId_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_transactionId" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_gateway_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_gateway" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_type_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_type" runat="server" /></td>
                        </tr>
                        <tr id="tr_last4" runat="server">
                            <td class="label">
                                <asp:Literal ID="ltr_last4_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_last4" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_amount_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_amount" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_paymentdate_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_paymentdate" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_authorizeddate_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_authorizeddate" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_captureddate_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_captureddate" runat="server" /></td>
                        </tr>
                        <tr id="tr_settled" runat="server" visible="false">
                            <td class="label">
                                <asp:Literal ID="ltr_settleddate_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_settleddate" runat="server" /></td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnl_trackingnumber" runat="server" Visible="false">
                <div id="AddTrackingNumber">
                    <asp:HiddenField ID="hdn_errorMessage" runat="server" />
                    <table class="ektronGrid" runat="server">
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_trackingnumber" runat="server" />:</td>
                            <td>
                                <asp:TextBox ID="txt_trackingnumber" runat="server" Columns="30" MaxLength="50" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_markasshipped" runat="server" />:</td>
                            <td>
                                <asp:CheckBox ToolTip="Mark/Unmark Shipped" ID="chk_markasshipped" runat="server" /></td>
                        </tr>
                        <tr id="tr_orderpart" runat="server" visible="false">
                            <td class="label">
                                <asp:Literal ID="Literal2" runat="server" />:</td>
                            <td>
                                <asp:TextBox ID="TextBox1" runat="server" /></td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnl_view" runat="Server" Visible="false">
                <div class="tabContainerWrapper">
                    <div class="tabContainer">
                        <ul>
                            <li>
                                <a title="Summary" href="#dvSummary">
                                    <asp:Literal ID="ltr_summary" runat="server" />
                                </a>
                            </li>
                            <li>
                                <a title="Status" href="#dvStatus">
                                    <asp:Literal ID="ltr_dvstatus" runat="server" />
                                </a>
                            </li>
                            <li>
                                <a title="Payment" href="#dvPayment">
                                    <asp:Literal ID="ltr_payment" runat="server" />
                                </a>
                            </li>
                            <li>
                                <a title="Coupon" href="#dvCoupon">
                                    <asp:Literal ID="ltr_coupon" runat="server" />
                                </a>
                            </li>
                            <li>
                                <a title="Addresses" href="#dvAddresses">
                                    <asp:Literal ID="ltr_addresses" runat="server" />
                                </a>
                            </li>
                            <li>
                                <a title="Description" href="#dvDescription">
                                    <asp:Literal ID="ltr_description" runat="server" />
                                </a>
                            </li>
                            <li>
                                <a title="Workflow" href="#dvWorkflow">
                                    <asp:Literal ID="ltr_workflow" runat="server" />
                                </a>
                            </li>
                        </ul>
                        <div id="dvSummary">
                            <table class="ektronGrid">
                                <tr>
                                    <td class="label left">
                                        <asp:Literal ID="ltr_orderid" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <table width="100%" class="ektronGrid">
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="ltr_customername" runat="server" /></td>
                                                <td class="label right">
                                                    <asp:Literal ID="ltr_customerorders_lbl" runat="server" />:</td>
                                                <td class="right">
                                                    <asp:Literal ID="ltr_customerorders" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td class="label">
                                                </td>
                                                <td class="label right">
                                                    <asp:Literal ID="ltr_customertotal_lbl" runat="server" />:</td>
                                                <td class="right">
                                                    <asp:Literal ID="ltr_customertotal" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td class="label">
                                                </td>
                                                <td class="label right">
                                                    <asp:Literal ID="ltr_customeravg_lbl" runat="server" />:</td>
                                                <td class="right">
                                                    <asp:Literal ID="ltr_customeravg" runat="server" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table class="ektronGrid">
                                <tr>
                                    <td class="label left">
                                        <asp:Literal ID="ltr_notes_lbl" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td>
                                        <table width="100%" class="ektronGrid">
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="ltr_notes" runat="server" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="dvStatus">
                            <table class="ektronGrid">
                                <tr>
                                    <td>
                                        <asp:Literal ID="ltr_status" runat="server" />
                                        <div class="ektronTopSpace">
                                        </div>
                                        <table width="100%" class="ektronGrid">
                                            <tr>
                                                <td class="label">
                                                    <asp:Literal ID="ltr_created_lbl" runat="server" />:</td>
                                                <td>
                                                    <asp:Literal ID="ltr_created" runat="server" /></td>
                                            </tr>
                                            <tr runat="server" id="tr_authorized">
                                                <td class="label">
                                                    <asp:Literal ID="ltr_authorized_lbl" runat="server" />:</td>
                                                <td>
                                                    <asp:Literal ID="ltr_authorized" runat="server" /></td>
                                            </tr>
                                            <tr runat="server" id="tr_captured">
                                                <td class="label">
                                                    <asp:Literal ID="ltr_captured_lbl" runat="server" />:</td>
                                                <td>
                                                    <asp:Literal ID="ltr_captured" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td class="label">
                                                    <asp:Literal ID="ltr_required_lbl" runat="server" />:</td>
                                                <td>
                                                    <asp:Literal ID="ltr_required" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td class="label">
                                                    <asp:Literal ID="ltr_completed_lbl" runat="server" />:</td>
                                                <td>
                                                    <asp:Literal ID="ltr_completed" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td class="label">
                                                    <asp:Literal ID="ltr_shipped_lbl" runat="server" />:</td>
                                                <td>
                                                    <asp:Literal ID="ltr_shipped" runat="server" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="dvPayment">
                            <asp:Literal ID="ltr_payments" runat="server" />
                            <asp:DataGrid ID="dg_payments"
                                runat="server"
                                AutoGenerateColumns="false"
                                ShowHeader="false"
                                CssClass="ektronGrid"
                                GridLines="None">
                                <HeaderStyle CssClass="title-header" />
                                <Columns>
                                    <asp:TemplateColumn>
                                        <ItemTemplate>
                                            <%#Util_ShowIcon(Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "AuthorizedOn")), Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "CapturedOn"))) %>
                                            &#160;<a title="Payment Date" href="#" onclick="if (allowOpen) { $ektron('<%#uxPaymentDialog.Selector%>').dialog('open');EktronUiDialogInit('showpayment', <%#m_iID%>, <%#DataBinder.Eval(Container.DataItem, "Id")%>); } return false;"><%#DataBinder.Eval(Container.DataItem, "PaymentDate")%></a></ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <a title="Show Payment" href="#" onclick="if (allowOpen) { $ektron('<%#uxPaymentDialog.Selector%>').dialog('open');EktronUiDialogInit('showpayment', <%#m_iID%>, <%#DataBinder.Eval(Container.DataItem, "Id")%>); } return false;">
                                                <%#Util_ShowPrice(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "PaymentTotal")), Convert.ToInt64(DataBinder.Eval(Container.DataItem, "Currency.Id")))%>
                                            </a>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                            </asp:DataGrid>
                        </div>
                        <div id="dvCoupon">
                            <asp:Literal ID="ltr_coupons" runat="server" />
                            <asp:DataGrid ID="dg_coupons"
                                runat="server"
                                AutoGenerateColumns="false"
                                ShowHeader="false"
                                CssClass="ektronGrid"
                                GridLines="None">
                                <HeaderStyle CssClass="title-header" />
                                <Columns>
                                    <asp:TemplateColumn>
                                        <ItemTemplate>
                                            <a title="Id" href="#" onclick="if (allowOpen) { $ektron('<%#uxCouponDialog.Selector%>').dialog('open');EktronUiDialogInit('coupon', <%#DataBinder.Eval(Container.DataItem, "Id")%>); } return false;"><%#DataBinder.Eval(Container.DataItem, "Id")%></a></ItemTemplate>
                                        </asp:TemplateColumn>
                                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <a title="Code" href="#" onclick="if (allowOpen) { $ektron('<%#uxCouponDialog.Selector%>').dialog('open');EktronUiDialogInit('coupon', <%#DataBinder.Eval(Container.DataItem, "Id")%>); } return false;">
                                                <%#DataBinder.Eval(Container.DataItem, "Code")%>
                                            </a>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <a title="Discount Type" href="#" onclick="if (allowOpen) { $ektron('<%#uxCouponDialog.Selector%>').dialog('open');EktronUiDialogInit('coupon', <%#DataBinder.Eval(Container.DataItem, "Id")%>); } return false;">
                                                <%#Util_ShowCouponInfo((Ektron.Cms.Common.EkEnumeration.CouponDiscountType)DataBinder.Eval(Container.DataItem, "DiscountType"),Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "DiscountValue")),Convert.ToInt64(DataBinder.Eval(Container.DataItem, "CurrencyId")))%>
                                            </a>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                            </asp:DataGrid>
                        </div>
                        <div id="dvWorkflow">
                            <asp:Literal ID="ltr_workflow_image" runat="server" />
                        </div>
                        <div id="dvAddresses">
                            <table class="ektronGrid">
                                <tr class="title-header">
                                    <td class="label left">
                                        <asp:Literal ID="ltr_order_billing_lbl" runat="server" /></td>
                                    <td class="label left">
                                        <asp:Literal ID="ltr_order_shipping_lbl" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td style="width: 50%">
                                        <table width="100%" class="ektronGrid">
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="ltr_order_billing" runat="server" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width: 50%">
                                        <table width="100%" class="ektronGrid">
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="ltr_order_shipping" runat="server" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="dvDescription">
                            <table class="ektronGrid">
                                <tr class="title-header">
                                    <td class="label left">
                                        <asp:Literal ID="ltr_desc_lbl" runat="server" /></td>
                                    <td class="label left">
                                        <asp:Literal ID="ltr_saleprice_lbl" runat="server" /></td>
                                    <td class="label left">
                                        <asp:Literal ID="ltr_qty_lbl" runat="server" /></td>
                                    <td class="label left">
                                        <asp:Literal ID="ltr_total_lbl" runat="server" /></td>
                                </tr>
                                <asp:Literal ID="ltr_lineitems" runat="server" />
                                <tr>
                                    <td class="right" colspan="3">
                                        <asp:Literal ID="ltr_subtotal_lbl" runat="server" /></td>
                                    <td class="right">
                                        <asp:Literal ID="ltr_subtotal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="right" colspan="3">
                                        <asp:Literal ID="ltr_coupontotal_lbl" runat="server" /></td>
                                    <td class="right">
                                        <asp:Literal ID="ltr_coupontotal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="right" colspan="3">
                                        <asp:Literal ID="ltr_taxtotal_lbl" runat="server" /></td>
                                    <td class="right">
                                        <asp:Literal ID="ltr_taxtotal" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="right" colspan="3">
                                        <asp:Literal ID="ltr_shippingtotal_lbl" runat="server" /></td>
                                    <td class="right">
                                        <asp:Literal ID="ltr_shippingtotal" runat="server" /></td>
                                </tr>
								 <tr>
                                    <td class="right" colspan="3">
                                        <asp:Literal ID="ltr_shippingtotal_tax_lbl" runat="server" /></td>
                                    <td class="right">
                                        <asp:Literal ID="ltr_shippingtotal_tax_total" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="right" colspan="3">
                                        <asp:Literal ID="ltr_ordertotal_lbl" runat="server" /></td>
                                    <td class="right">
                                        <asp:Literal ID="ltr_ordertotal" runat="server" /></td>
                                </tr>
                            </table>
                        </div>
                        <asp:Literal ID="ltr_orders" runat="server" />
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnl_viewaddress" runat="Server" Visible="false">
                    <table class="ektronGrid">
                        <tr id="tr_address_id" runat="server">
                            <td class="label">
                                <asp:Literal ID="ltr_address_id_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_address_id" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_address_name" runat="server" />:</td>
                            <td>
                                <asp:TextBox ID="txt_address_name" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_address_company" runat="server" />:</td>
                            <td>
                                <asp:TextBox ID="txt_address_company" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_address_line1" runat="server" />:</td>
                            <td>
                                <asp:TextBox ID="txt_address_line1" runat="server" />
                                <br />
                                <asp:TextBox ID="txt_address_line2" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_address_city_lbl" runat="server" />:</td>
                            <td>
                                <asp:TextBox ID="txt_address_city" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_address_postal" runat="server" />:</td>
                            <td>
                                <asp:TextBox ID="txt_address_postal" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_address_country" runat="server" />:</td>
                            <td>
                                <asp:DropDownList AutoPostBack="true" ID="drp_address_country" runat="server" OnSelectedIndexChanged="drp_address_country_ServerChange" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_address_region" runat="server" />:</td>
                            <td>
                                <asp:DropDownList ID="drp_address_region" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_address_phone" runat="server" />:</td>
                            <td>
                                <asp:TextBox ID="txt_address_phone" runat="server" /></td>
                        </tr>
                    </table>
                </asp:Panel>
            <asp:Panel CssClass="ektronPageGrid" ID="pnl_viewall" runat="Server">
                <asp:DataGrid
                    ID="dg_orders"
                    runat="server"
                    AutoGenerateColumns="false"
                    Width="100%"
                    CssClass="ektronGrid"
                    GridLines="None"
                >
                    <HeaderStyle CssClass="title-header" />
                    <Columns>
                        <asp:HyperLinkColumn DataTextField="Id" ItemStyle-Font-Underline="true" HeaderText="Id" DataNavigateUrlField="Id"
                            DataNavigateUrlFormatString="fulfillment.aspx?action=vieworder&id={0}"></asp:HyperLinkColumn>
                        <asp:HyperLinkColumn DataTextField="DateCreated" ItemStyle-Font-Underline="true" HeaderText="Date" DataNavigateUrlField="Id"
                            DataNavigateUrlFormatString="fulfillment.aspx?action=vieworder&id={0}"></asp:HyperLinkColumn>
                        <asp:TemplateColumn>
                            <HeaderTemplate>
                                <a title="Toggle Site" href="javascript: void(0);" onclick="ToggleDiv('dvSites');">Site</a><br />
                                <div id="dvSites" style="position: absolute; visibility: hidden; border: 1px solid black;
                                    background-color: white; padding: 4px;">
                                    <table>
                                        <asp:Literal ID="ltr_sites" runat="server" />
                                    </table>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Util_AddSite((string)DataBinder.Eval(Container.DataItem, "httphost"))%>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Status">
                            <ItemTemplate>
                                <%#Util_ShowStatus((Ektron.Cms.Common.EkEnumeration.OrderStatus)DataBinder.Eval(Container.DataItem, "Status"))%>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Customer">
                            <ItemTemplate>
                                <%#Util_ShowCustomer((Ektron.Cms.Commerce.CustomerData)DataBinder.Eval(Container.DataItem, "Customer"))%>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Order Value" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <%#DataBinder.Eval(Container.DataItem, "Currency.AlphaIsoCode")%>
                        <%#Ektron.Cms.Common.EkFunctions.FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "OrderTotal")),DataBinder.Eval(Container.DataItem, "Currency.CultureCode").ToString())%>
                    </ItemTemplate>
                </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
                <p class="pageLinks">
                    <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                    <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                    <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
                    <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
                </p>
                <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                    OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
                <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="lnkBtnPreviousPage" Text="[Previous Page]"
                    OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
                <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                    OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
                <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                    OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
            </asp:Panel>
            <asp:HiddenField ID="hdn_event" runat="server" />
            <asp:HiddenField ID="hdn_code" runat="server" />
            <div>
                <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
                <input type="hidden" runat="server" id="isDeleted" value="" name="isDeleted" />
                <input type="hidden" runat="server" id="isSearchPostData" value="" name="isSearchPostData" />
                <asp:Literal ID="literal1" runat="server" />
            </div>
        </div>
        <input type="hidden" runat="server" id="isCPostData" value="false" />
        <input type="hidden" runat="server" id="Hidden1" value="true" name="isPostData" />
    </form>
</body>
</html>

