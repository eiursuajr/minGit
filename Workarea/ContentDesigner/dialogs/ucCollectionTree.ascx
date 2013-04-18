<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ucCollectionTree.ascx.cs" Inherits="Ektron.ContentDesigner.Dialogs.CollectionTree" %>
<asp:Repeater ID="collections" runat="server">
    <ItemTemplate>
        <div class="treecontainer">
        <span class="folder" tabindex="0" data-ektron-resid="<%#DataBinder.Eval(Container.DataItem, "Id")%>">
        <%#DataBinder.Eval(Container.DataItem, "Title")%></span>
        </div>
    </ItemTemplate>
</asp:Repeater>
<script type="text/javascript">
<!--

function  openToSelectedCollection(tid)
{
    //now use tid to select the collection 
    var clicktarget = $ektron("div.treecontainer span.folder[data-ektron-resid='" + tid + "']");
    if(clicktarget.length > 0)
    {
        clicktarget.click();
    }
    //now scroll to the folder
    $ektron("#Pageview2").scrollTo("div.treecontainer span.folder[data-ektron-resid='" + tid + "']");
}

function configCollectionClickAction()
{
    $ektron("div.treecontainer span[data-ektron-resid]").unbind("click").click(function(){
        onNodeEventHandler(this);
    });
    $ektron("div.treecontainer span[data-ektron-resid]").unbind("keydown").keydown(function(e){
        switch (e.keyCode) 
        {
            case 32: // select folder on space bar key
                onNodeEventHandler(this);
                e.stopImmediatePropagation();
                return false;
                break;
        }
    });
}

function onNodeEventHandler(node)
{
    var $node = $ektron(node);
    $ektron("div.treecontainer .selected").removeClass("selected");
    $node.addClass("selected");
    var objectID = $node.attr("data-ektron-resid");
    var pageNum = 0;
    var action = "getcollectioncontent";
    var objecttype = "collection";
    getResults(action, objectID, pageNum, objecttype, "", node, "collection");
}

Ektron.ready(function(event, eventName)
{
    var iconSrc = "<%= new Ektron.Cms.SiteAPI().ApplicationPath %>images/UI/Icons/collection.png";
    var eIcon = $ektron("<img />").attr("style", "padding: 0 2px 0 0; vertical-align: bottom;").attr("src", iconSrc).attr("alt", ResourceText.CollectionItems);
    $ektron("div#PageViewCollection div.treecontainer").prepend(eIcon);
});
//-->
</script>