<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ucContentFolderTree.ascx.cs" Inherits="Ektron.ContentDesigner.Dialogs.ContentFolderTree" %>
<div class="CBfoldercontainer">
    <span class="folder" tabindex="0" data-ektron-folid="0" id="rootLabel" runat="server">Root</span>
    <ul class="EktronFolderTree EktronFiletree" data-ektron-folid="0"></ul>
</div>

<script language="javascript" type="text/javascript">
<!--
    function ContentDirectoryToHtml(directory)
    {
        var html = "";
        for(var i in directory.subdirectories)
        {
            html += "<li class=\"closed";
            if(i == directory.subdirectories.length - 1)
            {
                html += " last";
            }
            html += "\"><span class=\"folder";
            switch (directory.subdirectories[i].type) 
            {
                case 1:
                    html += " blog";
                    break;
            }
            html += "\" tabindex=\"0\" data-ektron-folid=\"" + directory.subdirectories[i].id + "\" data-ektron-foltype=\""+ directory.subdirectories[i].type +"\" >" +
                directory.subdirectories[i].name + "</span>";
                if(directory.subdirectories[i].haschildren)
                {
                    html += "<ul data-ektron-folid=\"" + directory.subdirectories[i].id + "\" data-ektron-foltype=\""+ directory.subdirectories[i].type +"\"></ul>";
                }
                html += "</li>";
        }
        return html;
    }
    
    function ConfigContentFolderTree(el, filterby, selectorType, startingFolder)
    {
         $ektron.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: webserviceURL,
            data: {"request" : createRequestObj("getchildfolders", 0, "", "folder", startingFolder, filterby) },
            success: function(msg)
            {
                var directory = eval("(" + msg + ")");
                var foldertree = ContentDirectoryToHtml(directory);
                if (foldertree.length > 0)
                {
                    $ektron("ul.EktronFolderTree").html(foldertree);
                $ektron("ul.EktronFolderTree").treeview(
                {
                    toggle : function(index, element)
                    {
                        var $element = $ektron(element);

                        if($element.html() === "")
                        {
                            $ektron.ajax(
                            {
                                type: "POST",
                                cache: false,
                                async: false,
                                url: webserviceURL,
                                data: {"request" : createRequestObj("getchildfolders", startingFolder, "", "folder", $element.attr("data-ektron-folid"), filterby) },
                                success: function(msg)
                                {
                                    var directory = eval("(" + msg + ")");
                                        var subfoldertree = ContentDirectoryToHtml(directory);
                                        if (subfoldertree.length > 0)
                                        {
                                            var el = $ektron(subfoldertree);
                                    $element.append(el);
                                    $ektron("ul.EktronFolderTree").treeview({add: el});
                                    configClickAction(filterby, selectorType);
                                }
                                    }
                            });
                        }
                    }
                });
                }
                configClickAction(filterby, selectorType);
                openToSelectedContent();
            }
        });
    }
    
    function configClickAction(filterby, selectorType)
    {
        $ektron("div.CBfoldercontainer span[data-ektron-folid]").unbind("click").click(function(){
            onFolderEventHandler(this, filterby, selectorType);
        });
        $ektron("div.CBfoldercontainer span[data-ektron-folid]").unbind("keydown").keydown(function(e){
            switch (e.keyCode) 
            {
                case 32: // select folder on space bar key
                    onFolderEventHandler(this, filterby, selectorType);
                    if ("string" == typeof selectorType && "content" == electorType)
                    {
                        openToSelectedContent();
                    }
                    e.stopImmediatePropagation();
                    return false;
                    break;
                case 13: // expand folder on Enter key
                    var fid = $ektron(this).attr("data-ektron-folid");
                    var clicktarget = $ektron("ul.EktronFolderTree span.folder[data-ektron-folid='" + fid + "']").parent().children("div.expandable-hitarea");
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
    
    function onFolderEventHandler(node, filterby, selectorType)
    {
        var $node = $ektron(node);
        $ektron("div.CBfoldercontainer .selected").removeClass("selected");
        $node.addClass("selected");
        // select folder
        $ektron("div.CBfoldercontainer .folderselected").removeClass("folderselected");
        $node.addClass("folderselected");
        // select content
        if ("content" == selectorType)
        {
            var objectID = $node.attr("data-ektron-folid");
            var FolderType = $node.attr("data-ektron-foltype");
            $ektron("#hdnFolderId").val(objectID);
            var pageNum = 0;
            var action = "getfoldercontent";
            var objecttype = "folder";
//            var localparent = $ektron(this).parents("div.CBWidget");
//            Ektron.PFWidgets.ContentBlock.parentID = localparent.attr("id");
            
//            if(localparent.find("select#CBTypeFilter")[0].value.toLowerCase() == "forms")
//            {
//              localparent.find(".CBAdd").hide();
//              localparent.find(".CBAddForm").show();
//            }
//            else
//            {
//              localparent.find(".CBAddForm").hide();
//              localparent.find(".CBAdd").show();
//            }
//            if(FolderType == "1" || FolderType =="3" || FolderType == "4")
//            {
//                localparent.find(".CBAdd").hide();
//            }
            getResults(action, objectID, pageNum, objecttype, "", node, filterby);
        }
    }
     
    function openToSelectedContent()
    {
        var fid = $ektron(".HiddenTBFolderPath"); //a list of folders (folder path) to reload the previous selection
        if (fid.length === 0)
        {
            return true;
        }
        fid = fid.val().split(',');
        if (fid.length === 0)
        {
            return true;
        }
        //now use fid to open all the folders
        for (var i = fid.length - 1; i >= 0; i--)
        {
            var clicktarget = null;
            if (i !== 0)
            {
                clicktarget = $ektron("ul.EktronFolderTree span.folder[data-ektron-folid='" + fid[i] + "']").parent().children("div.expandable-hitarea");
            }
            else
            {
                clicktarget = $ektron("ul.EktronFolderTree span.folder[data-ektron-folid='" + fid[i] + "']");
            }

            if(clicktarget.length > 0)
            {
                clicktarget.click();
            }
        }
        //now scroll to the folder
        $ektron("#Pageview1").scrollTo("ul.EktronFolderTree span.folder[data-ektron-folid='" + fid[0] + "']");

        highlightSelectedContent();
    } 
    
    function highlightSelectedContent()
    {
        var cid = $ektron(".HiddenTBData");
        if (cid.length === 0)
        {
            return true;
        }
        cid = cid.val();
        if (isNaN(cid)) return;
        //now select the content and scroll to it
        var citem = $ektron(".CBEdit #CBResults .CBresult span.contentid:contains('" + cid + "')").parent();
        if (citem.length > 0)
        {
            citem.click();
            $ektron(".CBEdit #CBResults").scrollTo(citem);
        }
        else
        {
        //#53210: the original structure are from function ContentToHtml() in resourceselectorpopup.aspx
            var $ResultBlock = $ektron(".CBEdit #CBResults .CBresult");
            if ($ResultBlock.length > 0)
            {
                var iconImgTag = "<img alt=\"" + m_idType + "\" src=\"" + m_waPath + "images/UI/Icons/contentHtml.png\" width=\"16\" height=\"16\" border=\"0\" />";
                var $MatchedRows = $ektron("span.idtype", $ResultBlock);
                if ($MatchedRows.length > 0)
                {
                    for ( var i = 0; i < $MatchedRows.length; i++)
                    {
                        if (m_idType == $MatchedRows.eq(i).text())
                        {
                            iconImgTag = $MatchedRows.eq(i).siblings(".icon").html();
                            break;
                        }
                    }
                }
                var newRow = ["<div class=\"CBresult odd selected\" tabindex=\"0\" title=\"",  m_resourceTitle,  "\">", 
                        "<span class=\"icon\">", iconImgTag, "</span>", 
                        "<span class=\"contentid\">", cid, "</span>",
                        "<span class=\"title\">", m_resourceTitle, "</span>",
                        "<span style=\"display: none\" class=\"idtype\">", m_idType, "</span>",
                        "<br class=\"clearall\" />",
                        "</div>"].join("");
                $ResultBlock.eq(0).before(newRow);
            }
        }
    } 
    
    function ConfigContentFolderTreeView(filterby, selectortype, startingfolder)
    {
        ConfigContentFolderTree($ektron("ul.EktronFolderTree"), (filterby || "content"), (selectortype || "content"), (startingfolder || 0));
    }
//-->
</script>