<%@ Control Language="C#" ClassName="ajaxfoldertree" %>

<script runat="server">
    public string CallbackFunc = "selectFolder";
    public string SitePath = "http://localhost/cms400min";
    public int FolderID = 0;
    public bool DelayLoad = false;
</script>

<div id="ekFolder<%= ID %>_0">
</div>

<script language="javascript" type="text/javascript" src="<%= SitePath %>/js/ajaxfoldertree.js"></script>
<script language="javascript" type="text/javascript">
<!--
var url = "<%= SitePath %>/ajaxfoldertree.aspx";

function ekSelectFolder<%= ID %>(folderId)
{
    <%= CallbackFunc %>(folderId);
}

if(!<%= DelayLoad %>)
    getSubTree("<%= ID %>", 0);
-->
</script>