<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ucTaxonomyTree.ascx.cs" Inherits="Ektron.ContentDesigner.Dialogs.TaxonomyTree" %>
<asp:Label ID="noTaxonomies" runat="server" />
<asp:Repeater ID="taxonomies" runat="server">
    <ItemTemplate>
        <div class="treecontainer">
        <span class="folder" tabindex="0" data-ektron-resid="<%#DataBinder.Eval(Container.DataItem, "TaxonomyId")%>">
        <%--<input type="checkbox" class="categoryCheck">--%>
        <%#DataBinder.Eval(Container.DataItem, "TaxonomyName")%></span>
        <ul class="EktronTaxonomyTree" data-ektron-resid="<%#DataBinder.Eval(Container.DataItem, "TaxonomyId")%>"></ul>
        </div>
    </ItemTemplate>
</asp:Repeater>
<span style="display:none;"><asp:TextBox ID="txtselectedTaxonomyNodes" runat="server"></asp:TextBox></span>
<span id="taxRequired" class="TaxRequiredBool" runat="server" style="display:none;"></span>
<script type="text/javascript">
<!--
function configTaxonomyTreeview(filterby)
{
    configTreeview($ektron("ul.EktronTaxonomyTree"), filterby);
}

function TaxonomyToHtml(directory)
{
    var html = "";
    for(var subdirectory in directory.subdirectories)
    {
        html += "<li class=\"closed";
        if(subdirectory == directory.subdirectories.length -1) html += " last";
        html += "\"><span tabindex=\"0\" class=\"folder\" data-ektron-resid=\"" + directory.subdirectories[subdirectory].tid + "\">" +
//            "<input type=\"checkbox\" class=\"categoryCheck\">" +
			directory.subdirectories[subdirectory].name + "</span>";
            if(directory.subdirectories[subdirectory].haschildren)
                html += "<ul data-ektron-resid=\"" + directory.subdirectories[subdirectory].tid + "\"></ul>";
            html += "</li>";
    }

    return html;
}

function configTreeview(el, filterby){
    $ektron(el).treeview({
        toggle : function(index, element) {
            var $element = $ektron(element);
            if($element.html() == ""){
                $ektron.ajax(
                {
                    type: "POST",
                    cache: false,
                    async: false,
                    url: "<%= new Ektron.Cms.SiteAPI().ApplicationPath %>ContentDesigner/dialogs/taxonomytree.ashx",
                    data: {"resid" : $element.attr("data-ektron-resid")},
                    success: function(msg)
                    {
                        var directory = eval("(" + msg + ")");
                        var thisel = $ektron(TaxonomyToHtml(directory));
                        $element.append(thisel);
                        $ektron(el).treeview({add: thisel});
						configTaxonomyClickAction(filterby);
//                        configCheckBoxes();
                        //are any ancestors checked? if so, disable me
/*                        var checked = $ektron("input.categoryCheck:checked[disabled='']");
                        for(var i=0; i<checked.length; i++){
                            var checkedid = $ektron(checked[i]).parent().attr("data-ektron-taxid");
                            if(thisel.parents("ul.EktronTaxonomyTree[data-ektron-taxid='"+checkedid+"']").length > 0){
                                thisel.find("input.categoryCheck").attr("checked", "true").attr("disabled", "disabled");
                            }
                        }
*/                    }
                });
            }
        }
    });
//    configCheckBoxes();
    if ("taxonomy" == filterby)
    {
        openToSelectedTaxonomy();
    }
}

function  openToSelectedTaxonomy()
{
    var tid = $ektron(".HiddenTBTaxonomyPath"); //a list of taxonomy id (taxonomy path) to reload the previous selection
    if (tid.length === 0)
    {
        return true;
    }
    var tid = tid.val().split(",");
    if (tid.length === 0)
    {
        return true;
    }
    //now use tid to open all the folders 
    for (var i = 0; i < tid.length; i++)
    {
        var clicktarget = null;
        if (i != tid.length - 1)
        {
            clicktarget = $ektron("div.treecontainer span.folder[data-ektron-resid='" + tid[i] + "']").parent().children("div.expandable-hitarea");
        }
        else
        {
            clicktarget = $ektron("div.treecontainer span.folder[data-ektron-resid='" + tid[i] + "']");
        }

        if(clicktarget.length > 0)
        {
            clicktarget.click();
        }
    }
    //now scroll to the folder
    $ektron("#Pageview2").scrollTo("div.treecontainer span.folder[data-ektron-resid='" + tid[tid.length - 1] + "']");
}

//function configCheckBoxes(){
//    $ektron("input.categoryCheck").unbind('click').bind('click', function(e) {
//        e.stopPropagation();

//        $ektron(this).parent().parent(".expandable").children("div").click();

//        if (this.checked) {
//            //if checked now, uncheck and disable all children nodes
//            var parentuls = $ektron(this).parents("ul[data-ektron-taxid]");
//            for (var i = 0; i < parentuls.length; i++) {
//                //check parent box
//                var parentid = $ektron(parentuls[i]).attr("data-ektron-taxid");
//                var box = $ektron("span[data-ektron-taxid='" + parentid + "'] > input");
//                box.attr("checked", "true");
//                box.attr("disabled", "disabled");
//            }
//        } else {
//            //if unchecked now, enable all parent nodes except those above another checked node
//            var disabled = $ektron("input.categoryCheck:checked[disabled]");
//            disabled.attr("checked", "");
//            disabled.attr("disabled", "");

//            var disabled = $ektron("input.categoryCheck:checked");
//            for (var j = 0; j < disabled.length; j++) {
//                var parentuls = $ektron(disabled[j]).parents("ul[data-ektron-taxid]");
//                for (var i = 0; i < parentuls.length; i++) {
//                    //check parent box
//                    var parentid = $ektron(parentuls[i]).attr("data-ektron-taxid");
//                    var box = $ektron("span[data-ektron-taxid='" + parentid + "'] > input");
//                    box.attr("checked", "true");
//                    box.attr("disabled", "disabled");
//                }
//            }
//        }
//        //get all checked nodes, concatenate, store in textbox
//        var checked = $ektron("input.categoryCheck:checked[disabled='']");
//        var string = "";
//        for (var i = 0; i < checked.length; i++) {
//                string += $ektron(checked[i]).parent().attr("data-ektron-taxid") + ",";
//        }
//        $ektron("input#<%= txtselectedTaxonomyNodes.ClientID %>")[0].value = string.substring(0, string.length - 1);

//    });
//}

function configTaxonomyClickAction(filterby)
{
    $ektron("div.treecontainer span[data-ektron-resid]").unbind("click").click(function(){
        onTaxNodeHandler(this, filterby);    
    });
    $ektron("div.treecontainer span[data-ektron-resid]").unbind("keydown").keydown(function(e){
        switch (e.keyCode) 
        {
            case 32: // select folder on space bar key
                onTaxNodeHandler(this, filterby);
                e.stopImmediatePropagation();
                return false;
                break;
            case 13: // expand folder on Enter key
                var tid = $ektron(this).attr("data-ektron-resid");
                var clicktarget = $ektron("div.treecontainer span.folder[data-ektron-resid='" + tid + "']").parent().children("div.expandable-hitarea");
                if(clicktarget.length > 0)
                {
                    clicktarget.click();
                }
                e.stopImmediatePropagation();
                return false;
                break;
        }
    });
}

function onTaxNodeHandler(node, filterby)
{
    $ektron("div.treecontainer .selected").removeClass("selected");
    var eCurrent = $ektron(node);
    eCurrent.addClass("selected");
    var objectID = eCurrent.attr("data-ektron-resid");
    if (filterby != "taxonomy")
    {
        var pageNum = 0;
        var action = "gettaxonomycontent";
        var objecttype = "taxonomy";
        getResults(action, objectID, pageNum, objecttype, "", node, filterby);
    }
    if ("taxonomy" == m_idType)
    {
        var path = eCurrent.text();
        if (eCurrent.parent() && !eCurrent.parent().hasClass("treecontainer"))
        {
            for (var i = 0; i < eCurrent.parents("ul").length; i++)
            {
                var resid = eCurrent.parents("ul").eq(i).attr("data-ektron-resid");
                if (resid.length > 0)
                {
                    var pathName = $ektron("span[data-ektron-resid=" + resid + "]").text();
                    if (pathName.length > 0)
                    {
                        path = pathName + " > " + path;
                    }
                }
            }
        }
        $ektron("div#taxPath").html("<i>" + path + "</i>");
    }
}

function ConfigTaxonomyTreeView(filterby)
{
    var els = $ektron("ul.EktronTaxonomyTree");
    for (var i = 0; i < els.length; i++)
    {
        $ektron.ajax(
        {
            type: "POST",
            cache: false,
            async: false,
            url: "<%= new Ektron.Cms.SiteAPI().ApplicationPath %>ContentDesigner/dialogs/taxonomytree.ashx",
            data: { "resid": $ektron(els[i]).attr("data-ektron-resid") },
            success: function(msg)
            {
                try
                {
	                var directory = eval("(" + msg + ")");
	                $ektron(els[i]).html(TaxonomyToHtml(directory));
                }
                catch (ex)
                {
	                $ektron(els[i]).html(ex.message + " (" + msg + ")");
                }
                configTreeview(els[i], filterby);
                configTaxonomyClickAction(filterby);
            }
        });
    }
}

Ektron.ready(function(event, eventName)
{
    var iconSrc = "<%= new Ektron.Cms.SiteAPI().ApplicationPath %>images/UI/Icons/taxonomy.png";
    var eIcon = $ektron("<img />").attr("style", "padding: 0 2px 0 0; vertical-align: bottom;").attr("src", iconSrc).attr("alt", ResourceText.TaxonomyItems);
    $ektron("div#PageViewTaxonomy div.treecontainer").prepend(eIcon);
});
//-->
</script>