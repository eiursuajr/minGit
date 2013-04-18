<%@ Control Language="C#" ClassName="ajaxcontenttree" %>

<script runat="server">
    public string CallbackFunc = "selectContent";
    public string SitePath = "./workarea/MOSS";
    public int FolderID = 0;
    public bool DelayLoad = false;
</script>

<div id="ekFolder<%= ID %>_0">
</div>
<div id="ekContent<%= ID %>_0">
</div>

<script language="javascript" type="text/javascript">
// expandCallback : string number string ->
// Receives the data from the getSubTree() ajax call
function expandCallback(controlId, folderId, data)
{
    // Get a reference to the given folder tree node
    var control = document.getElementById("ekFolder" + controlId + "_" + folderId);
    if(!control) return "";
    // and append the children to it.
    control.innerHTML = control.innerHTML.substring(0, control.innerHTML.length - 44);
    // Swap the folder image.
    control.innerHTML = control.innerHTML.replace(/plusopenfolder\.gif/, "minusopenfolder.gif");
    control.innerHTML += data;
}

function ContentCallback(controlId, folderId, data)
{
    var control = document.getElementById("ekContent" + controlId + "_0");
    if(!control) return "";
    control.innerHTML = data;
}

// ExecScript : string string -> element
// Executes an AJAX call to the specified url, using the given script id
function ExecScript(url, scriptId)
{
    if(!document.createElement) return null;
    
    // Initialize a new script element
    element = document.createElement("script");
    
    if(!element) return null;
    
    element.src = url + "&cleanupid=" + scriptId;
    element.id = scriptId;
    element.type = "text/javascript";
    
    // and append it to the document head
    var head = document.getElementsByTagName("head")[0];
    
    if(!head) return null;
    
    head.appendChild(element);
    
    return element;
}

// cleanUp : string -> 
// Deletes the script tag with the given ID
function cleanUp(id)
{
    // get a reference to the script tag
    var item = document.getElementById(id);
    
    if(!item) return null;
    
    // and get a reference to the head element
    var head = document.getElementsByTagName("head")[0];
    
    if(!head) return null;
    
    // finally, remove the script tag from the head
    head.removeChild(item);
}

// toggleTree : string number -> 
// Expands or collapses the specified tree node
function toggleTree(controlId, folderId)
{
    // Get references to the folder tree node and to the container inside 
    // of the node
    var div = document.getElementById("ekDiv" + controlId + "_" + folderId);
    var control = document.getElementById("ekFolder" + controlId + "_" + folderId);
    
    // If BOTH of the controls exist, then expand or detract the node instead of 
    // refetching the data
    if(div != null && control != null)
    {
        if(div.style.display == "none")
        {
            div.style.display = "";
            control.innerHTML = control.innerHTML.replace(/plusopenfolder\.gif/, 
                                                      "minusopenfolder.gif");
        }
        else
        {
            div.style.display = "none";
            control.innerHTML = control.innerHTML.replace(/minusopenfolder\.gif/, 
                                                      "plusopenfolder.gif");
        }
        
        return;
    }
    
    // If we've made it this far, then the children of the given node have 
    // not been fetched, so fetch and display the children
    getSubTree(controlId, folderId);
}

// getSubTree : string number -> expandCallback(string)
// Returns the HTML for the child nodes of the given folder ID
function getSubTree(controlId, folderId)
{
    // Get a reference to the folder tree node
    var control = document.getElementById("ekFolder" + controlId + "_" + folderId);
    if(!control) return;
    
    // Display some loading text
    control.innerHTML += "<div class=\"ekTreeLoading\">Loading...</div>";

    // Fetch the children
    ExecScript(url + "?controlid=" + controlId + "&folderid="  + folderId, 
               "ekScript" + controlId + "_" + folderId);
}

// clearSubTree : string number -> 
// Deletes the inner HTML of the given folder tree node
function clearSubTree(controlId, folderId)
{
    // Get a reference to the folder tree node
    var control = document.getElementById("ekFolder" + controlId + "_" + folderId);
    if(!control) return;
    
    // And delete its contents
    control.innerHTML = "";
}</script>
<script language="javascript" type="text/javascript">
<!--
var url = "<%= SitePath %>/ajaxfoldertree.aspx";
var contenturl = "<%= SitePath %>/ajaxcontenttree.aspx";

alert("test");

function ekSelectFolder<%= ID %>(object, folderId)
{
    var placeholder = document.getElementById("ekContent<%= ID %>_0");
    
    if(!document.createElement) return null;
    
    var scriptId = "ekScript<%= ID %>_" + folderId;
    
    // Initialize a new script element
    var element = document.createElement("script");
    
    if(!element) return null;
    
    element.src = contenturl + "?controlid=<%= ID %>&folderid="  + folderId + "&cleanupid=" + scriptId;
    element.id = scriptId;
    element.type = "text/javascript";
    
    // and append it to the document head
    var head = document.getElementsByTagName("head")[0];
    
    if(!head) return null;
    
    head.appendChild(element);
    
    return element;
}

function ekSelectContent(object, contentId){
    alert(contentId);
    //<%= CallbackFunc %>(contentId);
}

function ekExpandCallback<%= ID %>(){
    //<%= CallbackFunc %>(contentId);
}

getSubTree("<%= ID %>", 0);
-->
</script>