<%@ Control Language="C#" AutoEventWireup="true" CodeFile="foldertree.ascx.cs" Inherits="Workarea_pagebuilder_foldertree" %>

<ul id="<%= ClientID %>" class="EktronFiletree">
</ul>

<script type="text/javascript">
<!--

function DirectoryToHtml(directory)
{
    var html = "";
    for(var subdirectory in directory.subdirectories)
    {
        html += "<li class=\"closed\"><span class=\"folder\">" +
            directory.subdirectories[subdirectory].substring(directory.subdirectories[subdirectory].lastIndexOf("\\")+1) +
            "</span><ul data-ektron-path=\"" + directory.subdirectories[subdirectory] + "\"></ul></li>";
    }

    for(var file in directory.files)
    {
        html += "<li><span class=\"file\" onclick=\"if(OnFileClicked != null) OnFileClicked('" + directory.files[file].replace(/\\/g, "\\\\") + "')\">" +
            directory.files[file].substring(directory.files[file].lastIndexOf("\\")+1) +
            "</span></li>";
    }

    return html;
}


$ektron.ajax(
{
    type: "POST",
    cache: false,
    async: false,
    url: "<%= new Ektron.Cms.SiteAPI().ApplicationPath %>pagebuilder/foldertree.ashx",
    data: {"path" : "\\"
           <%= (Filter != "" ? ", \"filter\" : \"" + Filter + "\"" : "") %>},
    success: function(msg)
    {
        var directory = eval("(" + msg + ")");
        $ektron("#<%= ClientID %>").html("<ul>" + DirectoryToHtml(directory) + "</ul>");
        $ektron("#<%= ClientID %>").treeview(
        {
            toggle : function(index, element)
            {
                var $element = $ektron(element);

                if($element.html() == "")
                {
                    $ektron.ajax(
                    {
                        type: "POST",
                        cache: false,
                        async: false,
                        url: "<%= new Ektron.Cms.SiteAPI().ApplicationPath %>pagebuilder/foldertree.ashx",
                        data: {"path" : "\\" + $element.attr("data-ektron-path") + "\\"
                               <%= (Filter != "" ? ", \"filter\" : \"" + Filter + "\"" : "") %>},
                        success: function(msg)
                        {
                            var directory = eval("(" + msg + ")");
                            var el = $ektron(DirectoryToHtml(directory));
                            $element.append(el);
                            $ektron("#<%= ClientID %>").treeview({add: el});
                        }
                    });
                }
            }
        });
    }
});
-->
</script>