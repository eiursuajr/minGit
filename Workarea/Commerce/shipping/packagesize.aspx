<%@ Page Language="C#" AutoEventWireup="true" CodeFile="packagesize.aspx.cs" Inherits="Commerce_shipping_packagesize" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
        <title>Packages</title>
        <asp:literal id="ltr_js" runat="server" />
        <script type="text/javascript" language="javascript">
        function resetPostback()
        {
            document.forms[0].isPostData.value = "";
        }
        </script>
        <style type="text/css">
            form {position:relative;margin-top:-1px;}
        </style>
    </head>
    <body>
        <form id="form1" runat="server">
		<div class="ektronPageContainer">
            <div id="packagesize_page">
            <asp:Panel CssClass="ektronPageGrid" ID="pnl_viewall" runat="Server">
                <asp:DataGrid ID="dg_package"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="ektronGrid"
                    GridLines="None">
                    <HeaderStyle CssClass="title-header" />
                </asp:DataGrid>
            </asp:Panel>
            <asp:Panel ID="pnl_viewaddress" runat="Server" Visible="false">
                <div class="ektronPageInfo">
                    <table class="ektronGrid">
                        <tr>
                            <td class="label"><asp:Literal ID="ltr_package_name" runat="server" />:</td>
                            <td><asp:TextBox ID="txt_package_name" runat="server" /></td>
                        </tr>
                        <tr id="ltr_package_id_tr" runat="server">
                            <td class="label"><asp:Literal ID="ltr_package_id" runat="server" />:</td>
                            <td><asp:Label ID="lbl_package_id" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Literal ID="ltr_package_length" runat="server" />:</td>
                            <td>
                                <asp:TextBox ID="txt_package_length" runat="server" MaxLength="8" class="filterfield" />
                                <span style="padding-left:.5em;">
                                    <asp:Literal ID="ltr_length_unit" runat="server" />
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Literal ID="ltr_package_height" runat="server" />:</td>
                            <td>
                                <asp:TextBox ID="txt_package_height" runat="server" MaxLength="8" class="filterfield" />
                                <span style="padding-left:.5em;">
                                    <asp:Literal ID="ltr_height_unit" runat="server" />
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Literal ID="ltr_package_width" runat="server" />:</td>
                            <td>
                                <asp:TextBox ID="txt_package_width" runat="server" MaxLength="8" class="filterfield"  />
                                <span style="padding-left:.5em;">
                                    <asp:Literal ID="ltr_width_unit" runat="server" />
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Literal ID="ltr_package_weight" runat="server" />:</td>
                            <td>
                                <asp:TextBox ID="txt_package_weight" runat="server" MaxLength="8" class="filterfield"  />
                                <span style="padding-left:.5em;">
                                    <asp:Literal ID="ltr_weight_unit" runat="server" />
                                </span>
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <p class="pageLinks">
                <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                <asp:Label ToolTip="Of" runat="server" ID="OfLabel">of</asp:Label>
                <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
                <input type="hidden" runat="server" name="hdnUnit" value="hidden" id="hdnUnit" />
                <input type="hidden" runat="server" name="hdnCurrentPage" value="hidden" id="hdnCurrentPage" />
            </p>
            <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                OnCommand="NavigationLink_Click" CommandName="First" />
            <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="lnkBtnPreviousPage" Text="[Previous Page]"
                OnCommand="NavigationLink_Click" CommandName="Prev" />
            <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                OnCommand="NavigationLink_Click" CommandName="Next" />
            <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                OnCommand="NavigationLink_Click" CommandName="Last" />
        </div>
            <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
            <script type="text/javascript" language="javascript">

	    function CheckDelete()
	    {
		    return confirm(deletePackageMsg);
	    }

	    function SubmitForm()
	    {
		    var objtitle = document.getElementById("txt_package_name");
		    var uUnit = document.getElementById("ltr_weight_unit");
		    var mwUnit = document.getElementById("ltr_weight_unit");

		    if (Trim(objtitle.value).length > 0)
		    {
		        if (!CheckForillegalChar(objtitle.value)) {
		            objtitle.focus();
		        }
		        else if (ValidateNumberFieldForSubmit('txt_package_length')
		            && ValidateNumberFieldForSubmit('txt_package_height')
		            && ValidateNumberFieldForSubmit('txt_package_width')
		            && ValidateNumberFieldForSubmit('txt_package_weight') ) {
		            document.forms[0].submit();
		        }
		        else {
    		        alert(badPackageDimensionMsg);
		            return false;
		        }
		    }
		    else
		    {
		        alert(emptyPackageMsg);
		        objtitle.focus();
		    }
		    return false;
	    }

	    function CheckForillegalChar(txtName) {
		    var val = txtName;
		    if ((val.indexOf("\\") > -1) || (val.indexOf("/") > -1) || (val.indexOf(":") > -1)||(val.indexOf("*") > -1) || (val.indexOf("?") > -1)|| (val.indexOf("\"") > -1) || (val.indexOf("<") > -1)|| (val.indexOf(">") > -1) || (val.indexOf("|") > -1) || (val.indexOf("&") > -1) || (val.indexOf("\'") > -1))
		    {
		        alert(badPackageNameMsg);
		        return false;
		    }
		    return true;
	    }

        </script>
            <script type="text/javascript" language="javascript">
            Ektron.ready(InitializePageUI);

            function InitializePageUI(){
                $ektron("#packagesize_page input.filterfield").bind("keypress", FilterInput);
            }

            function FilterInput(eventObject){
                return (FilterNonNumerics(eventObject, this.value));
            }

	        ///////////////////////////////////
	        // When called like onkeypress='FilterNonNumerics(event)' will
	        // only allow numbers to be entered into input field.
	        // Note: For editing purposes, does allow control codes (e.g. backspace,
	        // left-arrow, delete, etc.) to be passed through for editing:
	        function FilterNonNumerics(eventObj, currentText){
                if (eventObj){
                    var charKey = 0;
                    var ctrl = 0;
                    var ctrlKey = false;
                    var newChar = "";

                    // IE doesn't pass control-codes, only standard keys (seems to handle internally):
                    if ("undefined" == typeof eventObj.charCode
                        && "undefined" != typeof eventObj.keyCode)
                        { charKey = eventObj.keyCode; }

                    // FireFox passes normal chars in charCode, control chars in keyCode:
                    if ("undefined" != typeof eventObj.charCode
                        && "undefined" != typeof eventObj.keyCode)
                        {
                            ctrl = eventObj.keyCode;
                            charKey = eventObj.charCode;
                        }

                    if ("undefined" != typeof eventObj.ctrlKey)
                        { ctrlKey = eventObj.ctrlKey }

                    if (!ctrlKey && charKey > 0)
                        newChar = String.fromCharCode(charKey);

                    if (8 == ctrl || 9 == ctrl)
                        return (true); // pass 'backspace' or 'tab'.

                    if ((charKey > 47 && charKey < 58) // numbers
                        || (ctrlKey && 118 == charKey) // <ctrl>-v (paste)
                        || (46 == charKey) // period
                        )
                    { return (ValidateFormat5_2(currentText + newChar)); } // if validates with existing and new char, then accept keypress.
                }
                return (false); // eat keypress.
            }

            // format 5.2:
            // zero-to-five digits, optionally followed
            // by a decimal and zero-to-two digits:
            function ValidateFormat5_2(text){
	            var regEx = /^\d{0,5}(\.\d{0,2})?$/;
	            return (regEx.test(text));
            }

            function ValidateNumberFieldForSubmit(id){
                obj = document.getElementById(id);
                if (obj
                    && (obj.value != "")
                    && !isNaN(obj.value)
                    && ValidateFormat5_2(obj.value)){
                    return (true);
                } else {
                    obj.focus();
                    return (false);
                }
            }

        </script>
		</div>
        </form>
    </body>
</html>

