<%@ Page Language="C#" AutoEventWireup="true" CodeFile="urlManualAliasMaint.aspx.cs"
    Inherits="Workarea_urlManualAliasMaint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>

    <asp:literal id="StyleSheetJS" runat="server" />

    <script type="text/javascript">
		<!--//--><![CDATA[//><!--
	function SubmitForm(FormName, Validate) {
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
	 function isEmpty(strData) {
	    return ((strData == null) || (strData.length == 0));
    }
	function isWhitespace(strData) {
        var whitespace = " \t\n\r";
	    var iCtr,
		    cTemp;

        if (isEmpty(strData))
		    return true;

        for (iCtr = 0; iCtr < strData.length; iCtr++) {
            var cTemp = strData.charAt(iCtr);
		    if (whitespace.indexOf(cTemp) == -1)
			    return false;
        }

        return true;
    }
	function VerifyAddAlias() {
		var es = '' ;
		if((document.forms.form1.txtAliasName.value=='') ||(isWhitespace(document.forms.form1.txtAliasName.value))) {
			es+= '<asp:Literal id="ltr_noAliasEntered" runat="server" />' ;
		}
		if (document.forms.form1.txtContentTitle.value=='') {
			es += '<asp:Literal id="ltr_noContBlck" runat="server" />' ;
		}

//		if (document.forms.form1.frm_qlink.value=='') {
//			es += '<asp:Literal id="ltr_noLink" runat="server" />' ;
//		}

		if(es!='') {
			alert('<asp:Literal id="ltr_follErr" runat="server" />' + es) ; return false;
		} else {
			return true ;
		}
	}

	function SetBrowserState()
	{
	     $ektron('#QuickLink').modalHide();
	     $ektron.ajax({
              url: "AJAXbase.aspx?action=getcontenttemplates&id=" + document.getElementById('frm_content_id').value + "&LangType=" + document.getElementById('frm_content_langid').value,
              cache: false,
              success: function(html){

                    $ektron('#templateList option').remove();
                    $ektron('#templateList').append(html);
                    return true;
              }});
	    return false;
	}

    Ektron.ready(function() {
        $ektron('#QuickLink').modal({
            trigger: '.viewQuickLink',
            modal: true,
            toTop: true,
            onShow: function(hash)
            {
                hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");
                hash.o.fadeTo("fast", 0.5, function()
                    {
	                    hash.w.fadeIn("fast");
                    }
                );
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
                    }
                );
            }
        });
    });
        //--><!]]>
    </script>
    <style type="text/css" >
        <!--/*--><![CDATA[/*><!--*/
            .selectContent { background-image: url('Images/ui/icons/check.png'); background-repeat: no-repeat; background-position:.5em center; padding-bottom: .3em !important; padding-top: .3em !important; line-height: 16px !important;}
            div.modalIframeWrapper {border: solid 1px #ccc;}
	        div.modalIframeWrapper iframe.modalIframe {height: 30em; width: 100%;}
         /*]]>*/-->
	</style>
</head>
<body>
    <form id="form1" runat="server">
        <input type="hidden" value="<%= contID %>" name="frm_content_id" id="frm_content_id" />
        <input type="hidden" value="<%= contLangID %>" name="frm_content_langid" id="frm_content_langid" />

        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronWindow ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all" id="QuickLink">
            <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix propertiesModalHeader">
                <span class="ui-dialog-title header" title="QuickLink Select"><%=msgHelper.GetMessage("lbl quickLink select")%></span>
                <a href="#" class="ui-dialog-titlebar-close ui-corner-all ektronModalClose"><span class="ui-icon ui-icon-closethick">&nbsp;</span></a>
            </div>
            <div class="ui-dialog-content ui-widget-content">
                <div class="modalIframeWrapper">
                    <iframe class="modalIframe" frameborder="0" scrolling="auto" src="QuickLinkSelect.aspx?folderid=<%=siteid %>&formName=frm_urlalias&titleFormElem=txtContentTitle&useQLinkCheck=0&SetBrowserState=1&forcetemplate=1&disAllowAddContent=1" id="iQuickLinkSelect"></iframe>
                </div>
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <table class="ektronGrid" width="100%">
                <tr>
                    <td class="label"><asp:Label ID="lblActive" runat="server" />:</td>
                    <td class="value"><asp:CheckBox ToolTip="Active Option" ID="activeChkBox" runat="server" Checked="true" /></td>
                </tr>
            </table>

            <div class="ektronTopSpace"></div>
            <fieldset id="urldef">
                <legend title="URL Definition"><%=msgHelper.GetMessage("lbl urldef")%></legend>
                <table class="ektronGrid" width="100%">
                    <tr>
                        <td class="label"><asp:Label ID="lblPrimary" Text="" runat="server" />:</td>
                        <td class="value"><asp:CheckBox ToolTip="Primary Option" ID="primaryChkBox" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label"><asp:Label ID="lblaliasname" runat="server" />:</td>
                        <td class="value" title="<%=_refContentApi.SitePath%>">
                            <%=_refContentApi.SitePath%>
                            <asp:TextBox ToolTip="Enter Alias Name here" ID="txtAliasName" CssClass="ektronTextMedium" runat="server" />
                            <asp:DropDownList ToolTip="Select Alias Extension from Drop Down Menu" ID="listAliasExtension" runat="server" /></td>
                    </tr>
                </table>
            </fieldset>

            <div class="ektronTopSpace"></div>
            <fieldset id="aliascontent">
                <legend title="<%=lblContentBlk%>"><%=lblContentBlk%></legend>
                <table class="ektronGrid" width="100%">
                    <tr>
                        <td class="label"><asp:Label ID="lblTitle" runat="server" />:</td>
                        <td class="value">
                            <asp:TextBox ToolTip="Enter Title here" Enabled="false" ID="txtContentTitle" runat="server" />
                            <asp:Label ID="quickLinkSelect" runat="server">
                            <a title="Select" href="#" class="button buttonInline greenHover selectContent viewQuickLink"><%=msgHelper.GetMessage("generic select")%></a>
                            </asp:Label>
                        </td>
                    </tr>
                    <tr id="tr_links" runat="server">
                        <td class="label"><asp:Label ID="lblLink" runat="server" /></td>
                        <td class="value">
                            <asp:TextBox ToolTip="Enter Quick Link here" Enabled="false" ID="frm_qlinkdis" runat="server" />
                            <input type="hidden" id="frm_qlink" value="" name="frm_qlink" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label"><asp:Label ID="lblTemplates" Text="Quick Links" runat="server" /></td>
                        <td class="value">
                            <asp:Label ID="lblTemplateList" runat="server">
                                <select title="Select QuickLink from the Drop Down Menu" id="templateList" name="templateList" runat="server">
                                </select>
                            </asp:Label>
                        </td>
                    </tr>
                </table>
            </fieldset>

            <div class="ektronTopSpace"></div>
            <table class="ektronGrid" width="100%">
                <tr>
                    <td class="label"><asp:Label ID="lblQueryStringAction" runat="server" />:</td>
                    <td class="value">
                        <asp:DropDownList ToolTip="Select QueryString Action from Drop Down Menu" ID="ddlQueryStringAction"  runat="server" >
                        <asp:ListItem Text="None" Value="0" />
                           <asp:ListItem Text="Replace All Parameters within Alias" Value="1" />
                           <asp:ListItem Text="Append Parameters to Alias" Value="2" />
                           <asp:ListItem Text="Resolve Matched Parameters within Alias" Value="3" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="label"><asp:Label ID="lblAddVar" Text="" runat="server" />:</td>
                    <td class="value">
                        <asp:TextBox ToolTip="Enter Additional Variables here" ID="txtAddVar" runat="server" />
                        <div class="ektronCaption" title="(Optional ex:Param1=Value1&Param2=Value2)"><%=msgHelper.GetMessage("lbl opturlparam")%></div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>