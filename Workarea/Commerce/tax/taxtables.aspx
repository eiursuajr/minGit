<%@ Page Language="C#" AutoEventWireup="true" CodeFile="taxtables.aspx.cs" Inherits="Commerce_tax_taxtables" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
        <title>Tax Tables</title>
        <asp:literal id="ltr_js" runat="server"/>
        <script type="text/javascript">
			    //hide drag drop uploader frame/////
			    top.HideDragDropWindow();
			    ////////////////////////////////////
			    /***********************************************
			    * Contractible Headers script- Â© Dynamic Drive (www.dynamicdrive.com)
			    * This notice must stay intact for legal use. Last updated Oct 21st, 2003.
			    * Visit http://www.dynamicdrive.com/ for full source code
			    ***********************************************/
			    var enablepersist="off" //Enable saving state of content structure using session cookies? (on/off)
			    var collapseprevious="no" //Collapse previously open content when opening present? (yes/no)


			    if (document.getElementById){
				    document.write('<style type="text/css">');
				    document.write('.switchcontent{display:none;}');
				    document.write('</style>');
			    }

			    function PopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
				    var cToolBar = "toolbar=0,location=0,directories=0,status=" + nResize + ",menubar=0,scrollbars=" + nScroll + ",resizable=" + nResize + ",width=" + nWidth + ",height=" + nHeight;
				    var popupwin = window.open(url, hWind, cToolBar);
				    return popupwin;
			    }

			    function getElementbyClass(classname){
				    ccollect=new Array();
				    var inc=0;
				    var alltags=document.all? document.all : document.getElementsByTagName("*");
				    for (i=0; i<alltags.length; i++){
					    if (alltags[i].className==classname)
						    ccollect[inc++]=alltags[i];
				    }
			    }

			    function contractcontent(omit){
				    var inc=0;
				    while (ccollect[inc]){
					    if (ccollect[inc].id!=omit)
						    ccollect[inc].style.display="none";
					    inc++;
				    }
			    }

			    function expandcontent(cid){
				    if (typeof ccollect!="undefined"){
					    if (collapseprevious=="yes")
						    contractcontent(cid);
					    document.getElementById(cid).style.display=(document.getElementById(cid).style.display!="block")? "block" : "none";
				    }
			    }

			    function revivecontent(){
				    contractcontent("omitnothing");
				    selectedItem=getselectedItem();
				    selectedComponents=selectedItem.split("|");
				    for (i=0; i<selectedComponents.length-1; i++)
					    document.getElementById(selectedComponents[i]).style.display="block";
			    }

			    function get_cookie(Name) {
				    var search = Name + "=";
				    var returnvalue = "";
				    if (document.cookie.length > 0) {
					    offset = document.cookie.indexOf(search);
					    if (offset != -1) {
						    offset += search.length;
						    end = document.cookie.indexOf(";", offset);
						    if (end == -1) {
							    end = document.cookie.length;
						    }
						    returnvalue=unescape(document.cookie.substring(offset, end));
					    }
				    }
				    return returnvalue;
			    }

			    function getselectedItem(){
				    if (get_cookie(window.location.pathname) != ""){
					    selectedItem=get_cookie(window.location.pathname);
					    return selectedItem;
				    }
				    else
					    return "";
			    }

			    function saveswitchstate(){
				    var inc=0, selectedItem="";
				    while (ccollect[inc]){
					    if (ccollect[inc].style.display=="block") {
						    selectedItem+=ccollect[inc].id+"|";
					    }
					    inc++;
				    }
				    document.cookie=window.location.pathname+"="+selectedItem;
			    }

			    function do_onload(){
				    getElementbyClass("switchcontent");
				    if (enablepersist=="on" && typeof ccollect!="undefined")
					    revivecontent();
			    }


			    if (window.addEventListener)
				    window.addEventListener("load", do_onload, false);
			    else if (window.attachEvent)
				    window.attachEvent("onload", do_onload);
			    else if (document.getElementById)
				    window.onload=do_onload;

			    if (enablepersist=="on" && document.getElementById) {
				    window.onunload=saveswitchstate;
			    }

			    function CheckApproveSelect()
			    {
			        var _countChecked = 0;
	                var err = 1;
	                var bret = true;

	                var ins = document.getElementsByTagName('input');
                    for (i = 0; i < ins.length; i++)
                    {
                        if (ins[i].getAttribute('type') == 'radio' && ins[i].name.indexOf("cr_") > -1 && ins[i].checked == true) { err = 0; }
                    }

	                if (err == 1) { bret = false; alert('Please select as least one review to update.'); }
	                return bret;
			    }
			    function resetPostback()
                {
                    document.forms[0].isPostData.value = "";
                }
        </script>
    </head>
    <body>
        <form id="form1" runat="server">
            <div class="ektronPageContainer">
                <asp:Panel CssClass="ektronPageGrid" ID="pnl_viewall" runat="Server">
                    <asp:DataGrid ID="dg_viewall"
                        EnableViewState="False"
                        AllowPaging="true"
                        AllowCustomPaging="True"
                        PagerStyle-Visible="False"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="ektronGrid"
                        GridLines="None">
                        <HeaderStyle CssClass="title-header" />
                    </asp:DataGrid>
                </asp:Panel>
                <asp:Panel ID="pnl_view" runat="Server" Visible="false">
                    <div class="ektronPageInfo">
                        <div class="tabContainerWrapper">
                            <div class="tabContainer">
                                <ul>
                                    <li><a title="Tax Codes" href="#TaxCodes"><%=this.GetMessage("lbl Tax Codes")%></a></li>
                                    <li><a title="Tax Rates" href="#TaxRates"><%=this.GetMessage("lbl tax rates")%></a></li>
                                </ul>
                                <div id="TaxCodes">
                                    <table id="tblmain" runat="server" class="ektronGrid">
                                        <tr>
                                            <td title="Region Name" class="label"><asp:Literal ID="ltr_name" runat="server"/>:</td>
                                            <td><asp:TextBox ID="txt_name" runat="server" MaxLength="25"/></td>
                                        </tr>
                                        <tr id="tr_id" runat="server">
                                            <td title="Region ID" class="label"><asp:Literal ID="ltr_id" runat="server"/>:</td>
                                            <td class="readOnlyValue"><asp:Label ID="lbl_id" runat="server"/></td>
                                        </tr>
                                        <tr>
                                            <td title="Enable - Disable Tax Rate" class="label"><asp:Literal ID="ltr_enabled" runat="server"/>:</td>
                                            <td><asp:CheckBox ID="chk_enabled" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td title="Code" class="label"><asp:Literal ID="ltr_code" runat="server"/>:</td>
                                            <td><asp:TextBox ID="txt_code" runat="server" MaxLength="25"/></td>
                                        </tr>
                                        <tr>
                                            <td title="Country" class="label"><asp:Literal ID="ltr_country" runat="server"/>:</td>
                                            <td><asp:DropDownList ID="drp_country" runat="server"/></td>
                                        </tr>
                                    </table>
                                </div>
                                <div id="TaxRates">
                                    <asp:Literal ID="ltr_txtClass" runat="server"/>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <p class="pageLinks">
                    <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                    <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                    <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
                    <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
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
                <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
            </div>
        </form>
    </body>
</html>

