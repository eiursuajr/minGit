<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ContentStatistics.aspx.cs"
    Inherits="ContentStatistics" EnableEventValidation="false" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <script type="text/javascript">
    <!--//--><![CDATA[//><!--
        var initialTab = '<asp:Literal id="SelectedTab" runat="server" />';

        function dateUpdatedEvent()
        {
            var d_s = document.getElementById('start_date_span').innerHTML;
            var d_e = document.getElementById('end_date_span').innerHTML;
            var d_s2 = document.getElementById('start_date2_span').innerHTML;
            var d_e2 = document.getElementById('end_date2_span').innerHTML;

            if (d_s != "[None]" && d_e != "[None]")
            {
                if (Date.parse(d_s) > Date.parse(d_e))
                {
                    alert('The end date needs to be after the start date.');
                    ClearDate('end_date');
                }
            }
            if (d_s2 != "[None]" && d_e2 != "[None]")
            {
                if (Date.parse(d_s2) > Date.parse(d_e2))
                {
                    alert('The end date needs to be after the start date.');
                    ClearDate('end_date2');
                }
            }
        }

        function ClearDate(flag)
        {
            if (flag == 'start_date' || flag == 'end_date' || flag == 'start_date2' || flag == 'end_date2')
            {
                document.getElementById(flag + '_iso').value = '';
                document.getElementById(flag + '_dow').value = '';
                document.getElementById(flag + '_dow').value = '';
                document.getElementById(flag + '_monum').value = '';
                document.getElementById(flag + '_yrnum').value = '';
                document.getElementById(flag + '_hr').value = '';
                document.getElementById(flag + '_mi').value = '';
                document.getElementById(flag + '_span').innerHTML = '[None]';

            }
        }

	    // Client-side control of folder-level flagging options:
	    function IsBrowserIE()
	    {
		    // document.all is an IE only property
		    return (document.all ? true : false);
	    }

        function ShowPane(tabID)
	    {
		    var arTab = new Array( "dvRating","dvMessages","dvFlagging");

		    if ((tabID == arTab[0]) && !IsBrowserIE())
		    {
			    window.location.reload(false);
		    }
		    else
		    {
			    var dvShow; //tab
			    var _dvShow; //pane
			    var dvHide;
			    var _dvHide;

			    for (var i=0; i < arTab.length; i++) {
				    if (tabID == arTab[i]) {
					    dvShow = eval('document.getElementById("' + arTab[i] + '");');
					    _dvShow = eval('document.getElementById("_' + arTab[i] + '");');
				    } else {

					    dvHide = eval('document.getElementById("' + arTab[i] + '");');
					    if (dvHide != null) {
						    dvHide.className = "tab_disabled";
					    }
					    _dvHide = eval('document.getElementById("_' + arTab[i] + '");');
					    if (_dvHide != null) {
						    _dvHide.style.display = "none";
					    }
				    }
			    }
			    if (_dvShow != null){
			        _dvShow.style.display = "block";
			    }
			    dvShow.className = "tab_actived";
		    }
	    }

	    // prompts to confirm if user really wants to delete the content rating
	    function deleteContentRatingPrompt(settings)
	    {
	        var s = {
	            ratingId: -1,
	            contentId: -1,
	            user: "",
	            date: "",
	            comment: ""
	        };
	        var deleteCommentMessage = "<asp:Literal ID="confirmDeleteCommentMessage" runat="server" />";
	        var deleteCommentDialog = $ektron("#ConfirmDialog");
	        var messageContainer = deleteCommentDialog.find("p.messages");
	        var okButton = deleteCommentDialog.find(".buttonOk");
	        // modify properties for insertion
	        $ektron(document).extend(s, settings);
	        s.user = "<span class='deleteCommentUser'>" + s.user + "<\/span>";
	        s.date = "<span class='deleteCommentDate'>" + s.date + "<\/span>";
	        if ($ektron.trim(s.comment.length) > 0)
	        {
	            s.comment = "<blockquote class='deleteCommentComment'><span class='bqStart'>&#8220;<\/span>" + s.comment + "<span class='bqEnd'>&#8221;<\/span><\/blockquote>";
	        }
	        

	        messageContainer.html("" + $ektron.formatString(deleteCommentMessage, s.user, s.date, s.comment));
	        // add data attribute with necessary values to Ok Button
	        okButton.attr("data-ektron-ratingid", s.ratingId);
	        okButton.attr("data-ektron-contentid", s.contentId);
	        deleteCommentDialog.modalShow();
	    }

        Ektron.ready(function()
        {
            var tabsContainers = $ektron(".tabContainer");
            tabsContainers.tabs();
            if (initialTab == 'dvFlagging')
            {
                $('.tabContainer').tabs('option', 'selected', 2)
            }

            // initialize deleteContentRating prompt dialog
            var confirmDialog = $ektron("#ConfirmDialog");
            confirmDialog.modal(
            {
                modal: true,
                overlay: 0,
                trigger: "",
                onShow: function(hash) {
                    hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");
		            hash.o.fadeTo("fast", 0.5, function() {
			            hash.w.fadeIn("fast");
		            });
                },
                onHide: function(hash) {
                    hash.w.fadeOut("fast");
		            hash.o.fadeOut("fast", function() {
			            if (hash.o)
			            {
				            hash.o.remove();
		                }
		            });
                }
            });

            // provide hover effects for dialog close buttons
            $ektron(".ui-widget-header .ektronModalClose").hover(
                function(){
                    $ektron(this).addClass("ui-state-hover");
                },
                function(){
                    $ektron(this).removeClass("ui-state-hover");
                }
            );

            // bind the deleteComment action to the "ok" button of the dialog
            confirmDialog.find(".buttonOk").one("click", function()
            {
                var okButton = $ektron(this);
                var appPath = "<asp:Literal id="jsAppPath" runat="server" />";
                var ratingId = okButton.attr("data-ektron-ratingid");
                var contentId = okButton.attr("data-ektron-contentid");
                var queryString = $ektron.formatString("?action=delete&id={0}&cid={1}&page=workarea&redirectUrl=", ratingId, contentId);

                if (window.location.href.indexOf("showReviews") !== -1)
                {
                    queryString += encodeURIComponent(window.location.href);
                }
                else
                {
                    queryString += encodeURIComponent(window.location.href + "&showReviews=true");
                }
                window.location = appPath + "addeditcontentreview.aspx" + queryString;
            });
        });
	//--><!]]>
    </script>

    <style type="text/css">
    <!--/*--><![CDATA[/*><!--*/

    div.ekMessagePaging {display: block;margin: 0em; width:auto; position: static; padding-bottom:15px;}
    div.ekMessagePaging h4 {display: block;margin: 0em ; font-size: 1em; text-align: left;}
    div.ekMessagePaging div.ekPageControl {display: block; margin: 0em; position: relative; text-align: left; width: auto; padding: 0em;}
    div.ekMessagePaging div.ekPageControl span {display: inline; width: auto; height: auto; overflow: auto; float: none;}

    div.ekMessagePaging div.ekPageControl span.but_first_disable {margin: 0em;text-indent: 0px; background-image:none; background-position: 0px 0px;}
    div.ekMessagePaging div.ekPageControl span.but_first{margin: 0em;background-image:none; background-position: 0px 0px;}
    div.ekMessagePaging div.ekPageControl span.but_first a{margin: 0em;display: inline; text-indent: 0px; height: 0px;}

    div.ekMessagePaging div.ekPageControl span.but_previous_disable{text-indent: 0px; background-image:none; background-position: 0px 0px; }
    div.ekMessagePaging div.ekPageControl span.but_previous{background-image:none; background-position: 0px 0px;}
    div.ekMessagePaging div.ekPageControl span.but_previous a{display: inline; text-indent: 0px; height: 0px;}

    div.ekMessagePaging div.ekPageControl span.but_next_disable {text-indent: 0px; background-image:none; background-position: 0px 0px;}
    div.ekMessagePaging div.ekPageControl span.but_next{background-image:none; background-position: 0px 0px;}
    div.ekMessagePaging div.ekPageControl span.but_next a{display: inline; text-indent: 0px; height: 0px;}

    div.ekMessagePaging div.ekPageControl span.but_last_disable {text-indent: 0px; background-image:none; background-position: 0px 0px;}
    div.ekMessagePaging div.ekPageControl span.but_last{background-image:none; background-position: 0px 0px;}
    div.ekMessagePaging div.ekPageControl span.but_last a{display: inline; text-indent: 0px; height: 0px;}

    a.buttonDelete { background-image: url(images/ui/icons/delete.png) !important; background-position: 0 0 !important;}
    td.approve a { display:block; background-repeat:no-repeat; width:16px; height:16px; background-image: url(Images/ui/icons/approvals.png); background-position: 0 0; margin:0 auto; text-indent:-10000px; overflow:hidden; }

    span.ratingStars {display: block; margin: 0; padding: 0; width: 80px; }
    span.commandLinks {display: block; margin: 0; padding: 0; width: 38px;}
    span.commandLinks a {float: left;}
    span.commandLinks a.ui-icon-ektron-contentEdit {margin-right: 6px;}
    span.deleteCommentUser, span.deleteCommentDate {color: #336699; font-weight: bold;}

    .ektronConfirmDialogModal .ektronModalBody blockquote
    {
        display: block;
	    font-style:italic;
	    margin: 1em;
	    padding: 0;
	    overflow: hidden;
    }
    
    .ektronConfirmDialogModal .ektronModalBody blockquote span.bqStart, .ektronConfirmDialogModal .ektronModalBody blockquote span.bqEnd
    {
        display: block;
	    float: left;
        height: 45px;
        font-size: 700%;
	    font-family: Times New Roman, Serif;
	    color: #aeaeae;
        margin: -37px 6px 0px -10px;
        *margin: -37px -10px -50px -10px;
    }
    .ektronConfirmDialogModal .ektronModalBody blockquote span.bqEnd
    {
	    float: right;
        height: 45px;
        margin: -37px 0 0 -10px;
        *margin: -37px -15px 0 -10px;
        *line-height: 1.25em;
        *height: .5em;
    }
    table.ektronGrid#messageBoardTable {display: table !important;}
    /*]]>*/-->
    </style>
</head>
<body>
    <!-- Modal Dialog: Confirm -->
    <div class="ektronWindow ektronConfirmDialogModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all"
        id="ConfirmDialog">
        <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix  ektronModalHeader">
            <h3 class="ui-dialog-title header">
                <span class="headerText">
                    <asp:Literal ID="confirmDialogHeader" runat="server" /></span>
                <asp:HyperLink ID="closeDialogLink" CssClass="ui-dialog-titlebar-close ui-corner-all ektronModalClose"
                    runat="server" Style="cursor: pointer" /></h3>
        </div>
        <div class="ektronModalBody">
            <div class="ui-dialog-content ui-widget-content ektronPageInfo">
                <p class="messages">
                </p>
            </div>
            <ul class="buttonWrapper ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                <li>
                    <asp:HyperLink ToolTip="Cancel" ID="dialogCancelButtonText" runat="server" CssClass="redHover button buttonCancel buttonRight ektronModalClose" /></li>
                <li>
                    <asp:HyperLink ToolTip="Ok" ID="dialogOkButtonText" runat="server" CssClass="greenHover button buttonOk buttonRight" /></li>
            </ul>
        </div>
    </div>
    <form id="form1" name="form1" runat="server">
        <div id="dhtmltooltip">
        </div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="txtTitleBar" runat="server">
            </div>
            <div class="ektronToolbar" id="htmToolBar" runat="server">
            </div>
        </div>
        <div class="ektronPageContainer ektronPageTabbed">
            <div class="tabContainerWrapper">
                <div class="tabContainer">
                    <ul>
                        <li><a title="Rating" href="#dvRating">
                            <%=_MessageHelper.GetMessage("viewratingtabtitle")%>
                        </a></li>
                        <li><a title="Messages" href="#dvMessages">
                            <%=_MessageHelper.GetMessage("viewmessagetabtitle")%>
                        </a></li>
                        <li><a title="Flagging" href="#dvFlagging" id="tabFlagging">
                            <%=_MessageHelper.GetMessage("viewflaggingtabtitle")%>
                        </a></li>
                    </ul>
                    <div id="dvRating">
                        <asp:Literal ID="Literal1" runat="server" EnableViewState="False" />&nbsp;&nbsp;<br />
                        <asp:Button ToolTip="Get Reviews" ID="getResultBtn" runat="server" Text="Get Reviews"
                            OnClick="getResultBtn_Click" />
                        <asp:Button ToolTip="Purge Reviews" ID="Button2" runat="server" Text="Purge Reviews"
                            OnClick="Button2_Click" />
                        <div class="ektronTopSpace">
                        </div>
                        <div class="ektronBorder">
                            <asp:Label ID="resultGraph" runat="server" />
                        </div>
                        <asp:Label ID="totalResults" runat="server" CssClass="ektronTopSpace" />
                        <asp:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound"
                            AllowPaging="True" AllowSorting="True" PageSize="50" Width="100%" AutoGenerateColumns="false"
                            CssClass="ektronGrid ektronTopSpace" GridLines="None" OnSorting="GridView1_Sorting"
                            OnPageIndexChanging="GridView1_PageIndexChanging">
                            <HeaderStyle CssClass="title-header" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <span class="commandLinks ui-helper-clearfix">
                                            <asp:LinkButton ToolTip="Edit Comment Link" ID="editCommentLink" CssClass="ui-icon ui-icon-ektron-contentEdit"
                                                runat="server"></asp:LinkButton><asp:LinkButton ToolTip="Deletes Comment Link" ID="deleteCommentLink"
                                                    CssClass="ui-icon ui-icon-ektron-delete" runat="server"></asp:LinkButton>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:LinkButton ID="userNameHeader" CommandArgument="Username" CommandName="Sort"
                                            runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Literal ID="r_uname" runat="Server" Text='<%# GetUserName(DataBinder.Eval(Container, "DataItem.Username")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date" SortExpression="RatingDate">
                                    <HeaderTemplate>
                                        <asp:LinkButton ID="dateHeader" CommandArgument="RatingDate" CommandName="Sort" runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Literal ID="r_datelink" runat="Server" Text='<%#GetEditURL(Convert.ToInt64(DataBinder.Eval(Container, "DataItem.ContentRatingID")), Convert.ToString(DataBinder.Eval(Container, "DataItem.RatingDate"))) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <span class="ratingStars">
                                            <%#GenerateStars(Convert.ToInt32(DataBinder.Eval(Container, "DataItem.Rating")))%>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Comments" SortExpression="Comments">
                                    <ItemTemplate>
                                        <asp:Literal ID="r_comments" runat="Server" Text='<%# HttpContext.Current.Server.UrlDecode(DisplayComments(Convert.ToString(DataBinder.Eval(Container, "DataItem.Comments")))) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status" SortExpression="RatingState">
                                    <ItemTemplate>
                                        <asp:Literal ID="r_status" runat="Server" Text='<%# DisplayRatingStatus(Convert.ToInt32(DataBinder.Eval(Container, "DataItem.RatingState"))) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:Literal ID="no_results_lbl" runat="server" />
                        <br />
                        <asp:Button ToolTip="Export to Excel" ID="Button1" runat="server" Text="Export to Excel"
                            OnClick="Button1_Click" />
                        <table>
                            <tr>
                                <td id="td_ecp_search" runat="server">
                                </td>
                            </tr>
                        </table>
                        <input type="hidden" id="start_date" name="start_date" />
                        <input type="hidden" id="end_date" name="end_date" />
                    </div>
                    <div id="dvMessages">
                        <CMS:MessageBoard ID="MessageBoard1" runat="server" MarkupLanguage="community/messageboardworkarea.ekml"
                            EnablePaging="true" Moderate="true" DynamicObjectParameter="id" />
                    </div>
                    <div id="dvFlagging">
                        <div id="divFlaggingHtml" runat="server">
                            <asp:Literal ID="Literal2" runat="server" EnableViewState="False" />
                            <div class="ektronTopSpace">
                            </div>
                            <asp:Button ToolTip="Get Flags" ID="getFlagBtn" runat="server" Text="Get Flags" OnClick="getFlagBtn_Click" />
                            <asp:Button ToolTip="Purge Flags" ID="cmdFlags" runat="server" Text="Purge Flags"
                                OnClick="cmdFlags_Click" />
                            <div class="ektronTopSpace">
                            </div>
                            <asp:DataGrid ID="dg_flag" runat="server" Width="100%" AutoGenerateColumns="false"
                                EnableViewState="False" CssClass="ektronGrid" GridLines="None">
                                <HeaderStyle CssClass="title-header" />
                            </asp:DataGrid>
                            <asp:Label ID="no_results_lbl2" runat="server" />
                            <input type="hidden" id="start_date2" name="start_date" />
                            <input type="hidden" id="end_date2" name="end_date" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
