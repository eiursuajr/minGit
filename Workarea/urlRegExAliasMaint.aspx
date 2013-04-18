<%@ Page Language="C#" AutoEventWireup="true" CodeFile="urlRegExAliasMaint.aspx.cs"
    Inherits="Workarea_urlRegExAliasMaint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>

    <asp:literal id="StyleSheetJS" runat="server" />

    <script type="text/javascript" language="javascript">
        Ektron.ready(function() {
            $ektron('#selExpDialog').modal({
                trigger: '.viewSelectExpTrigger',
                modal: true,
                onShow: function(hash) {
                    hash.o.fadeTo("fast", 0.5, function() {
                        hash.w.fadeIn("fast");
                    });
                },
                onHide: function(hash) {
                    hash.w.fadeOut("fast");
                        hash.o.fadeOut("fast", function(){
                        if (hash.o)
                            hash.o.remove();
                        });
                    }
                });
        });

        function selectExpMap(expressionMap,expression,reqURL)
        {

          $ektron('#selExpDialog').modalHide();
          var obj = document.getElementById ("txtExpressionMap") ;
          obj.value= expressionMap;
          var obj1 = document.getElementById ("txtExpression") ;
          obj1.value=expression;
          var obj2 = document.getElementById ("txtRequestedUrl") ;
          obj2.value=reqURL;
          var obj3 = document.getElementById ("txtExampleURL");
          obj3.value=reqURL;

        }
        function SubmitForm(FormName, Validate) {
		if (Validate.length > 0) {
			if (eval(Validate)) {
			    resetCPostback("submit");
				document.forms[FormName].submit();
				return false;
			}
			else {
				return false;
			}
		}
		else {
		    resetCPostback("submit");
			document.forms[FormName].submit();
			return false;
		}
	}
        function resetCPostback(sample)
        {
        if(sample=="test")
        {
         document.forms["form1"].isCPostData.value = "";
         }
         else
         {
          document.forms["form1"].isCPostData.value = "false";
          }
          return false;
        }
        function testRegExpression()
        {

         resetCPostback("test");
         if(document.forms.form1.txtRequestedUrl.value=='')
          {
            alert('<asp:Literal id="ltr_valURL" runat="server" />');
             return false;
          }
         document.forms["form1"].submit();
         return false;
        }

        function VerifyAddAlias() {
		var es = '' ;
		if(document.forms.form1.txtExpressionName.value=='') {
			es+= '<asp:Literal id="ltr_enterRegexName" runat="server" />' ;
		}
		if(document.forms.form1.txtExpression.value=='') {
			es+= '<asp:Literal id="ltr_enterRegex" runat="server" />' ;
		}
		if (document.forms.form1.txtExpressionMap.value==''){
			es += '<asp:Literal id="ltr_enterRegExMap" runat="server" />' ;
		}
	    if(es!='') {
			alert('<asp:Literal id="ltr_follErr" runat="server" />' + es) ; return false;
		}
		else {
			return true ;
		}
	}
    </script>
<style type="text/css" >
    div.ektronWindow h3 {
        background-color:#3163BD;
        background-image:url(images/application/darkblue_gradiant-nm.gif);
        background-position:0pt -2px;
        background-repeat:repeat-x;
        color:#FFFFFF;
        font-size:1em;
        margin:0pt;
        padding:0.6em 0.25em;
        position:relative;
    }
    div.ektronWindow h3 a.ektronModalClose {
        background-color:transparent;
        background-image:url(images/application/closeButton.gif);
        background-position:0px -23px;
        text-indent:-10000px;
        background-repeat:no-repeat;
        display:block;
        height:21px;
        overflow:hidden;
        position:absolute;
        right:0.25em;
        text-decoration:none;
        top:0.25em;
        width:21px;
        color:#FFFFFF;
}
</style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="ektronWindow" style="overflow: auto;" id="selExpDialog">
            <div class="ektronModalHeader">
                <h3>
                    <asp:Literal ID="lblExpressionLib" runat="server" />
                    <asp:HyperLink ID="closeDialogLink" CssClass="ektronModalClose" runat="server" />
                </h3>
            </div>
            <div style="width: 100%; height: auto; float: left; overflow: auto;">
                <div class="ektronPageGrid">
                        <asp:GridView ID="regExPicker"
                        runat="server"
                        AutoGenerateColumns="False"
                        Width="10%"
                        EnableViewState="False"
                        GridLines="None">
                        <HeaderStyle CssClass="title-header" />
                    </asp:GridView>
                </div>
            </div>
        </div>

        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageInfo">
            <table class="ektronGrid">
                <tr>
                    <td class="label"><asp:Label ID="lblActive" runat="server" />:</td>
                    <td class="value"><asp:CheckBox ToolTip="Active Option" ID="activeChkBox" runat="server" Checked="true" /></td>
                </tr>
            </table>
                        
            <div class="ektronTopSpace"></div>
            <fieldset>
                <legend title="<%=msgHelper.GetMessage("lbl pattern")%>"><%=msgHelper.GetMessage("lbl pattern")%></legend>
                <table class="ektronForm">
                    <tr>
                        <td class="label"><asp:Label ID="lblExpressionName" runat="server" />:</td>
                        <td class="value"><asp:TextBox ToolTip="Enter Expression Name here" Enabled="true" ID="txtExpressionName" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label"><asp:Label ID="lblExpression" runat="server" />:</td>
                        <td class="value"><asp:TextBox ToolTip="Enter Expression here" Enabled="true" ID="txtExpression" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label"><asp:Label ID="lblExpressionMap" runat="server" />:</td>
                        <td class="value">
                            <asp:TextBox ToolTip="Enter Expression Map here" ID="txtExpressionMap" runat="server" />
                            <asp:Label ID="quickLinkSelect" runat="server"><a href="#" class="viewSelectExpTrigger" title="<%=msgHelper.GetMessage("lbl select exp")%>"><%=msgHelper.GetMessage("lbl select exp")%></a></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="label"><asp:Label ID="lblExampleURL" runat="server" />:</td>
                        <td class="value"><asp:TextBox ToolTip="Enter Example URL here" Enabled="true" ID="txtExampleURL" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label"><asp:Label ID="lblSort" Text="Sort Order" runat="server" />:</td>
                        <td class="value"><asp:DropDownList ToolTip="Select Priority from the Drop Down Menu" ID="ddlSort" runat="server" /></td>
                    </tr>
                </table>
            </fieldset>

            <div id="aliasTest" runat="server">
                <div class="ektronTopSpace"></div>
                <fieldset>
                    <legend title="<%=msgHelper.GetMessage("lbl expression testing")%>"><%=msgHelper.GetMessage("lbl expression testing")%></legend>
                    <table class="ektronForm">
                        <tr>
                            <td class="label"><asp:Label ID="lblRequestedUrl" runat="server" ToolTip="<%=_refContentApi.SitePath%>" />:&nbsp;<%=_refContentApi.SitePath%></td>
                            <td class="value">
                                <asp:TextBox ToolTip="Enter remaining Requested URL here" Enabled="true" ID="txtRequestedUrl" runat="server" />
                                <asp:Label ID="lbltestRegEx" runat="server">
                                    <input type="button" title="Transform" name="evalExp" value="<%=msgHelper.GetMessage("btn transform")%>" onclick="testRegExpression();return false;" />
                                </asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Label ID="lblTransformedUrl" runat="server" />:</td>
                            <td class="value"><asp:TextBox ToolTip="Enter Resulting URL here" Enabled="false" ID="txtTransformedUrl" runat="server" /></td>
                        </tr>
                    </table>
                </fieldset>
            </div>

            <input type="hidden" runat="server" id="isCPostData" value="false" />
        </div>
    </form>
</body>
</html>