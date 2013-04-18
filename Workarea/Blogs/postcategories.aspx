<%@ Page Language="C#" AutoEventWireup="true" CodeFile="postcategories.aspx.cs"
    Inherits="blog_postcategories" ValidateRequest="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <title>Post Categories</title>
             <style type="text/css">
            <!--/*--><![CDATA[/*><!--*/
                ul.ektree {position:static !important;}		        
                div#FrameContainer
                {
                    width: 80%;
                    height: 60%;
                    margin: -250px 0 0 -500px;
                }
            /*]]>*/-->
        </style>
        <script language="javascript" type="text/javascript">
            function Close() {
                if (parent != null && parent != self && typeof parent.ektb_remove == 'function') {
                    parent.ektb_remove();
                } else {
                    self.close();
                }
            }
            function Save(uniqueId, selectedValues, selectedNames) {
                $ektron('#blogposttaxid_' + uniqueId, window.opener.document).val(selectedValues);
                $ektron('#ekblogtax' + uniqueId, window.opener.document).html(GetList(selectedNames));
                Close();
            }
            function GetList(selectedNames) {
                var listHtml = '';
                if (selectedNames.length > 0) {
                    listHtml = '<ul>';
                    for (var i = 0; i < selectedNames.length; i++)
                        listHtml += '<li>' + selectedNames[i] + '</li>';
                    listHtml += '</ul>';
                } else {
                    listHtml += '<asp:literal id="ltrNoCatSelected" runat="Server" />';
                }
                return listHtml;
            }
        </script>
</head>
<body>
    <form id="form1" name="form1" runat="server">
        <asp:Literal runat="server" ID="EditTaxonomyHtml" />
        <div style="clear: both;"> </div>
        <br />
        <div id="wamm_float_menu_block_menunode" class="Menu" onmouseout="wamm_float_menu_block_mouseout(this)" onmouseover="wamm_float_menu_block_mouseover(this)" style="position: absolute; left: 203px; top: 311px; z-index: 3200; display: none;">
            <input type="hidden" name="LastClickedParent" id="Hidden1" value="" />
            <input type="hidden" name="ClickRootCategory" id="Hidden2" value="false" />
            <ul>
                <li class="MenuItem add">
                    <a title="Route Action" href="#" onclick="routeAction(true, 'add');">
                            <asp:literal ID="lit_add_string" runat="server"/>
                    </a>
                </li>
            </ul>
        </div>

        <input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
        <input type="hidden" name="LastClickedOnChecked" id="LastClickedOnChecked" value="false" />
        <input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value="" runat="server" />
    </form>

</body>
</html>

