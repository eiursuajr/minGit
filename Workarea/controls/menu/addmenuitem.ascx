<%@ Control Language="C#" AutoEventWireup="true" CodeFile="addmenuitem.ascx.cs" Inherits="Workarea_controls_menu_addmenuitem" %>

<script language="javascript" type="text/javascript">
    function test_new_cb() {
        document.AddMenuItem.submit();
    }
    $ektron.addLoadEvent(function () {
        if (window.location.href.indexOf("noworkarea") == -1) {
            $ektron("input.newContentItemType")[0].style.display = "none";
        }
    });
    $ektron(document).ready(function(){
         $ektron(".ektronPageContainer").css({
            backgroundColor: 'transparent'
         });
    });

    </script>
</script>

<div class="ektronPageHeader">
    <div class="ektronTitlebar" title="Add New Item">
        <asp:Literal ID="litAddMenuTitle" runat="server"></asp:Literal>
    </div>
    <div class="ektronToolbar">
        <table>
            <tr>
                <asp:Literal ID="litButtons" runat="server"></asp:Literal>
                <td>
                    <asp:Literal ID="litHelp" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
    </div>
</div>
<form name="AddMenuItem" action="collections.aspx?LangType=<%=ContentLanguage %>&action=pAddMenuItem&nId=<%=MenuId %>&folderid=<%=FolderId %>&parentid=<%=mpID %>&ancestorid=<%=maID %>&iframe=<%=Request.QueryString["iframe"] %>&back=<%=Server.UrlEncode(Request.QueryString["back"]) %><%=noWorkAreaString%>" method="post">
<input type="hidden" name="frm_back" id="frm_back" runat="server" />
<div class="ektronPageContainer ektronPageInfo">
    <div class="heightFix">
        <div>
             <input title="Content Item" type="radio" name="ItemType" checked="checked"  value="content" /><asp:Literal ID="content" runat="server"></asp:Literal>
        </div>
        <div>
             <input title="New Content Block" type="radio" visible="false" class="newContentItemType" name="ItemType" value="newcontent" /><asp:Literal Visible="false"  ID="ContentBlock" runat="server"></asp:Literal>
        <div>
       <div>
             <input title="Library Asset" type="radio" name="ItemType" value="library" /><asp:Literal ID="Library" runat="server"></asp:Literal>
       </div>
        <div>
            <input title="External Hyperlink" type="radio" name="ItemType" value="link" /><asp:Literal ID="Hyperlink" runat="server"></asp:Literal>
       </div> 
        
       <div>
            <input title="Sub Menu" type="radio" name="ItemType" value="submenu" /><asp:Literal ID="subMenu" runat="server"></asp:Literal>
       </div>
        
        <div class="ektronTopSpace">
        </div>
        <input title="Next" name="next" type="button" value="Next..." onclick="test_new_cb();" />
    </div>
</div>
</form>
