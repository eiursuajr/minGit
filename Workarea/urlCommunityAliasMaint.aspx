<%@ Page Language="C#" AutoEventWireup="true" CodeFile="urlCommunityAliasMaint.aspx.cs"
    Inherits="Workarea_urlCommunityAliasMaint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Community Alias</title>
    <asp:literal id="StyleSheetJS" runat="server" />

    <script type="text/javascript">
        <!--//--><![CDATA[//><!--

            function SubmitForm(FormName, Validate)
             {
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
             
            function resetCPostback()
            {
               document.forms["frm_communityalias"].isCPostData.value = "";
            }
            
             function VerifyAddAlias() {
	              var es = '' ;
                  if (document.forms.frm_communityalias.ddlExt.value==''){
	                es += '<asp:Literal id="ltr_noExtSel" runat="server" />' ;
	                document.forms["frm_communityalias"].isCPostData.value = "false";
	               }
                   if (document.forms.frm_communityalias.tbAliasPath.value=='')
                   {
                        if($ektron("#ddltype")[0].value.toLowerCase() == "user")
                        {
                            $ektron("#txtExample")[0].value = "[DisplayName]" + $ektron("#ddlExt")[0].value;
                            $ektron("#hdntxtExample")[0].value = $ektron("#txtExample")[0].value;
                        }
                        else
                        {
                            $ektron("#txtExample")[0].value = "[GroupName]"  + $ektron("#ddlExt")[0].value;
                            $ektron("#hdntxtExample")[0].value = $ektron("#txtExample")[0].value;
                        }
                   }
                   else if($ektron("#tbAliasPath")[0].value == "[Site Root]")
                   {
                        $ektron("#hdntxtExample")[0].value = $ektron("#txtExample")[0].value;
                   }
                   else
                   {
                        $ektron("#hdntxtExample")[0].value = $ektron("#txtExample")[0].value;
                   }
	             ///es += '<asp:Literal id="ltr_noAliasSel" runat="server" />' ;
	             //document.forms["frm_communityalias"].isCPostData.value = "false";
	            
                if(es!='') {
		            alert('<asp:Literal id="ltr_msgFollErr" runat="server" />' + es) ; return false;
		            document.forms["frm_communityalias"].isCPostData.value = "false";
	            }
	            else {
		            return true;
	            }
            }
            SetPreviewLinkTextBox = function(obj)
            {
                var srcType = $ektron("#ddltype")[0].value;
                var previewLink = "";
                if(srcType.toLowerCase() == "user")
                {
                    previewLink = "[DisplayName]";
                }
                else
                {
                    previewLink = "[GroupName]";
                }
                if(obj.value == '')
                {
                    $ektron("#txtExample")[0].value = obj.value + previewLink + $ektron("#ddlExt")[0].value;
                }
                else
                {
                    $ektron("#txtExample")[0].value = obj.value + "/" + previewLink + $ektron("#ddlExt")[0].value;
                }
                $ektron("#hdntxtExample")[0].value = $ektron("#txtExample")[0].value;
            }
            ToggleExamplePath = function(obj)
            {
                if(obj.selectedIndex == 0)
                {
                    if($ektron("#tbAliasPath")[0].value == '')
                    {
                        $ektron("#txtExample")[0].value = $ektron("#tbAliasPath")[0].value + "[DisplayName]" + $ektron("#ddlExt")[0].value;
                    }
                    else
                    {
                        $ektron("#txtExample")[0].value = $ektron("#tbAliasPath")[0].value + "/[DisplayName]" + $ektron("#ddlExt")[0].value;
                    }
                }
                else
                {
                    if($ektron("#tbAliasPath")[0].value == '')
                    {
                        $ektron("#txtExample")[0].value = $ektron("#tbAliasPath")[0].value + "[GroupName]" + $ektron("#ddlExt")[0].value;
                    }
                    else
                    {
                        $ektron("#txtExample")[0].value = $ektron("#tbAliasPath")[0].value + "/[GroupName]" + $ektron("#ddlExt")[0].value;
                    }
                }
                $ektron("#hdntxtExample")[0].value = $ektron("#txtExample")[0].value;
            }
            ToggleExample = function(obj)
            {
                var aliasVal = $ektron("#tbAliasPath")[0].value;
                
                if(obj.selectedIndex == 0)
                {
                    if($ektron("#ddltype")[0].selectedIndex == 0)
                    {
                        if(aliasVal == '')
                        {
                            $ektron("#txtExample")[0].value =  $ektron("#tbAliasPath")[0].value + "[DisplayName]" + $ektron("#ddlExt")[0].value;
                        }
                        else
                        {
                            $ektron("#txtExample")[0].value =  $ektron("#tbAliasPath")[0].value + "/[DisplayName]" + $ektron("#ddlExt")[0].value;                            
                        }
                    }
                    else
                    {
                        if(aliasVal == '')
                        {
                            $ektron("#txtExample")[0].value =  $ektron("#tbAliasPath")[0].value + "[GroupName]" + $ektron("#ddlExt")[0].value;
                        }
                        else
                        {
                            $ektron("#txtExample")[0].value =  $ektron("#tbAliasPath")[0].value + "/[GroupName]" + $ektron("#ddlExt")[0].value;
                        }
                    }
                }
                else
                {
                    if($ektron("#ddltype")[0].selectedIndex == 0)
                    {
                        if(aliasVal == '')
                        {
                            $ektron("#txtExample")[0].value = $ektron("#tbAliasPath")[0].value +  "[DisplayName]" + $ektron("#ddlExt")[0].value;
                        }
                        else
                        {
                            $ektron("#txtExample")[0].value = $ektron("#tbAliasPath")[0].value +  "/[DisplayName]" + $ektron("#ddlExt")[0].value;
                        }
                    }
                    else
                    {
                        if(aliasVal == '')
                        {
                            $ektron("#txtExample")[0].value =  $ektron("#tbAliasPath")[0].value + "[GroupName]" + $ektron("#ddlExt")[0].value;
                        }
                        else
                        {
                            $ektron("#txtExample")[0].value =  $ektron("#tbAliasPath")[0].value + "/[GroupName]" + $ektron("#ddlExt")[0].value;
                        }
                    }
                }
                $ektron("#hdntxtExample")[0].value = $ektron("#txtExample")[0].value;
            }
             //--><!]]>
    </script>

</head>
<body>
    <form id="frm_communityalias" runat="server">
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server">
            </div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <fieldset id="Fieldset1">
                <asp:CheckBox ToolTip="Active Alias Option" ID="activeChkBox" runat="server" Checked="true" />
                <asp:Label ID="lblActive" runat="server" />
            </fieldset>
            <div class="ektronTopSpace">
            </div>
            <fieldset id="urldef">
                <legend title="<%=msgHelper.GetMessage("lbl urldef")%>">
                    <%=msgHelper.GetMessage("lbl urldef")%>
                </legend>
                <table class="ektronGrid" width="100%">
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblPrimary" Text="" runat="server" />:</td>
                        <td class="value">
                            <asp:CheckBox ToolTip="Primary Option" ID="primaryChkBox" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblType" runat="server" />:</td>
                        <td>
                            <asp:DropDownList ToolTip="Select Source Type from the Drop Down Menu" ID="ddltype" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblAliasPath" runat="server" />:</td>
                        <td class="value" title="<%=_refContentApi.SitePath%>">
                            <%=_refContentApi.SitePath%>
                            &nbsp;<asp:TextBox ToolTip="Enter remaining Alias Path here" ID="tbAliasPath" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblExt" runat="server" />:</td>
                        <td>
                            <asp:DropDownList ToolTip="Select Extension from the Drop Down Menu" ID="ddlExt" AutoPostBack="false" OnSelectedIndexChanged="ShowLinkExample"
                                runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ToolTip="Enter Replacement Character here" ID="lblReplaceChar" runat="server" />:</td>
                        <td class="value">
                            <asp:TextBox ID="txtReplaceChar" Text="_" MaxLength="1" Width="13" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label"><asp:Label id="lblQueryStringParam" runat="server" /></td>
                        <td><asp:TextBox ToolTip="Enter Source Query String Parameters here" ID="txtQueryStringParam" runat="server" /></td>
                        </tr>
                </table>
            </fieldset>
            <div class="ektronTopSpace">
            </div>
            <fieldset id="previewAlias">
                <legend><strong title="<%=msgHelper.GetMessage("lbl preview alias")%>">
                    <%=msgHelper.GetMessage("lbl preview alias")%>
                </strong></legend>
                <table class="ektronGrid">
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblExample" runat="server" />:</td>
                        <td title="<%=_refContentApi.SitePath%>">
                            <%=_refContentApi.SitePath%>
                            &nbsp;<input title="Enter remaining Link Sample Preview here" type="text" id="txtExample" name="txtExample" runat="server" disabled="disabled" />
                            <input type="hidden" runat="server" id="hdntxtExample" name="hdntxtExample" /></td>
                    </tr>
                </table>
            </fieldset>
        </div>
        <input type="hidden" runat="server" id="isCPostData" value="false" />
    </form>
</body>
</html>

