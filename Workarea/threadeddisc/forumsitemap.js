var arSitemapPathNodes = new Array();
var selectedNode;

function node(title, url, description, order){
    this.title = title;
    this.url = url;
    this.description = description;
    this.order = order;
}
function clearNodes()
{
    var NodesCollection = document.getElementById("sitemap_nodes");
    NodesCollection.innerHTML = '';
}
function renderSiteMapNodes() {	
    
    clearNodes();										    												        
    for (var i = 0; i < arSitemapPathNodes.length; i++)
    {
        createNode(arSitemapPathNodes[i]);
    }
}
function addSiteMapNode(button)
{
    
    var title = document.getElementById("sitemaptitle_input");
    var url = document.getElementById("sitemapurl_input");
    var desc = document.getElementById("sitemapdesc_input");
    if (Trim(title.value) == '')
    {
        alert("Title is required.");
        title.focus();
        return false;
    }
   
    if(title.value.indexOf('<')!=-1 || title.value.indexOf('>')!=-1 )
    {
        alert("'<' and '>' are not valid characters");
        title.value='';
        title.focus();
        return false;
    }
   if(url.value.indexOf('<')!=-1 || url.value.indexOf('>')!=-1 )
    {
        alert("'<' and '>' are not valid characters");
        url.value='';
        url.focus();
        return false;
    }
    if(desc.value.indexOf('<')!=-1 || desc.value.indexOf('>')!=-1 )
    {
        alert("'<' and '>' are not valid characters");
        desc.focus();
        return false;
    }   
          
    if (selectedNode != null)
    {
        arSitemapPathNodes[selectedNode.order].title = makeStringSafe(title.value.toString());
        arSitemapPathNodes[selectedNode.order].url = makeStringSafe(url.value.toString());
        arSitemapPathNodes[selectedNode.order].description = makeStringSafe(desc.value.toString());
        renderSiteMapNodes();
        clearSitemapForm();
    }
    else
    {
        objNode = new node(makeStringSafe(title.value.toString()) , makeStringSafe(url.value.toString()) , makeStringSafe(desc.value.toString()), arSitemapPathNodes.length);
        arSitemapPathNodes[arSitemapPathNodes.length] = objNode;
        createNode(objNode);
    }
    title.value = '';
    title.focus();
    url.value = '';	
    desc.value = '';
}
function createNode(node)
{
    var span_elem; 
    var att;
    var NodesCollection = document.getElementById("sitemap_nodes");
    var firfox = document.createElement("div")
    
    var newEntry = document.createElement("div")
    newEntry.id = "sitemap_" + makeSafeId(node.title.toLowerCase());
    newEntry.className = "SitemapPath";
    											        
    setOnClickSelectNode(newEntry, node);
    span_elem = document.createElement("span");
    span_elem.id = "sitemap_links_" + makeSafeId(node.title.toLowerCase());
    span_elem.innerHTML = "<img onclick=\"deleteNode('" + makeSafeId(node.title) + "');\" alt=\"Delete path\" src=\"../images/UI/Icons/delete.png\" />&nbsp;&nbsp;&nbsp;&nbsp;";
    newEntry.appendChild(span_elem);
 
    span_elem = document.createElement("span");
    span_elem.id = "sitemap_title_" + makeSafeId(node.title.toLowerCase());
    span_elem.innerHTML =  encodeAmpersand(node.title);
    newEntry.appendChild(span_elem);
    
    firfox.appendChild(newEntry)
    NodesCollection.appendChild(firfox);
    previewSitemapPath();
}
function editNode()
{    

    //fireSelectNode(id);
    document.getElementById("sitemaptitle_input").value = selectedNode.title.replace("\'", "'").replace(/&#39;/g, "'").replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g,'>').replace(/&quot;/g,'\"');
    document.getElementById("sitemapurl_input").value = selectedNode.url.replace("\'", "'");
    document.getElementById("sitemapdesc_input").value = selectedNode.description.replace("\'", "'").replace(/&#39;/g, "'").replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g,'>').replace(/&quot;/g,'\"');
    document.getElementById("btnAddSitepath").value = "Save";
}
function deleteNode(id)
{
    if (document.getElementById(clientName_chkInheritSitemapPath).checked)
    {
        alert("Sitemap Path is inherited from the parent folder.");
        return false;
    }
    fireSelectNode(id);
    var obj = document.getElementById("sitemap_" + makeSafeId(selectedNode.title.toLowerCase())).parentNode;
    obj.parentNode.removeChild(obj);
    
    arSitemapPathNodes.splice(selectedNode.order, 1);
    var node;
    for (var i=0; i<arSitemapPathNodes.length; i++) {
            arSitemapPathNodes[i].order = i;
    }
    
    clearSitemapForm();
    previewSitemapPath();
}

function clearSitemapForm()
{
    document.getElementById("sitemaptitle_input").value = "";
    document.getElementById("sitemapurl_input").value = "";
    document.getElementById("sitemapdesc_input").value = "";
    document.getElementById("btnAddSitepath").value = "Add";
    selectNode(null, null);
}

function selectNode(elem, node)
{
    selectedNode = node;
    var elements = DOMUtil.getElementByClassName( "ektronSitemapPathSelected" );	
    for(var i=0;i<elements.length;i++)
    {
        elements[i].className = "SitemapPath";
    }
    if (elem != null) {
        elem.className = "ektronSitemapPathSelected";
        editNode();
    }
}

function fireSelectNode(id)
{
   
    id = makeSafeId(id);
    var obj = document.getElementById("sitemap_" + id.toLowerCase())    
    if (obj != null)
    {
        if ( document.createEvent ) {
            var evObj = document.createEvent('MouseEvents');
            evObj.initEvent( 'click', true, true );
            obj.dispatchEvent(evObj);
        } else {
            obj.fireEvent("onclick");
        }
    }
}
function moveSitemapPathNode(direction)
{
    var position = null;
    var selPosition = null;
    if (document.getElementById(clientName_chkInheritSitemapPath).checked)
    {
        alert("Sitemap Path is inherited from the parent folder.");
        return false;
    }
    if (selectedNode == null)
    {
        alert("Item is not selected.");
        return false;
    }
    selPosition = selectedNode.order
    if (direction == "up") {
        if (selPosition == 0) 
            return false;
        position = selPosition - 1;
    } else {
        if (((arSitemapPathNodes.length - 1) == selPosition)) 
            return false;
        position = selPosition + 1;
    }
    //swap nodes in array
    var repNode = arSitemapPathNodes[position];
    var orgNode = arSitemapPathNodes[selPosition];
    
    repNode.order = selPosition;
    orgNode.order = position;	
    arSitemapPathNodes[selPosition] = repNode;
    arSitemapPathNodes[position] = orgNode;
    
    //swap html
    var repElem = document.getElementById("sitemap_" + makeSafeId(repNode.title.toLowerCase())).parentNode;
    var orgElem = document.getElementById("sitemap_" + makeSafeId(orgNode.title.toLowerCase())).parentNode;
    
    
    var saveHtml = repElem.innerHTML;
    repElem.innerHTML = orgElem.innerHTML;
    orgElem.innerHTML = saveHtml;    
    
    setOnClickSelectNode(document.getElementById("sitemap_" + makeSafeId(repNode.title.toLowerCase())), repNode);												        
    setOnClickSelectNode(document.getElementById("sitemap_" + makeSafeId(orgNode.title.toLowerCase())), orgNode);	
    											        
    previewSitemapPath();
}

function setOnClickSelectNode(elem, node)
{
    elem.onclick = function() { selectNode( this, node ); }
}

function previewSitemapPath()
{
    var preview = document.getElementById("sitepath_preview");
    var path = '';
    for (var i = 0; i < arSitemapPathNodes.length; i++)
    {
        if (path == '')
        {
            path = encodeAmpersand(arSitemapPathNodes[i].title);
        } else {
           path = path + ' >> ' + encodeAmpersand(arSitemapPathNodes[i].title);
//path = path + ' >> ' + arSitemapPathNodes[i].title;
        }
    }
    preview.innerHTML = path;
}
function saveSitemapPath()
{
    var node;
    var serializeNode = "";
    for (var i=0;i<arSitemapPathNodes.length;i++)
    {
        node = arSitemapPathNodes[i];
        serializeNode += "<node>";
        serializeNode += "<title>" + makeStringSafe(node.title.toString()) + "</title>";
        serializeNode += "<url>" + makeStringSafe(node.url.toString()) + "</url>";
        serializeNode += "<order>" + makeStringSafe(node.order.toString()) + "</order>";
        serializeNode += "<description>" + makeStringSafe(node.description.toString()) + "</description>";
        serializeNode += "</node>";
    }
    serializeNode = "<sitemap>" + serializeNode + "</sitemap>";
    document.getElementById("saved_sitemap_path").value = encodeURIComponent(serializeNode);
}
function InheritSitemapPath(bChecked)
{
    var inheritVBC = $ektron('div#dvBreadcrumb input#chkInheritSitemapPath');
    if (bChecked){        
        document.getElementById("hdnInheritSitemap").value = 'true';
        document.getElementById("AddSitemapNode").style.display = 'none';        
    } else {
        if(confirm('Are you sure you want to break inheritance?')){
            document.getElementById("hdnInheritSitemap").value = 'false';
            document.getElementById("AddSitemapNode").style.display = 'block';
        } else {
            inheritVBC[0].checked = !inheritVBC[0].checked;
            return false;
        }
    }
}
function PopBrowseWin(Scope, FolderPath, retField){
	
	var Url;
    if (FolderPath == "") {
        Url = '../browselibrary.aspx?actiontype=add&disableLinkManage=true&type=' + Scope;
	} else {
		Url = '../browselibrary.aspx?actiontype=add&disableLinkManage=true&type=' + Scope + '&autonav=' + FolderPath;
	}  
	if (retField != null)
	{
		Url = Url + '&RetField=' + retField;
	} 
    PopUpWindow(Url, 'BrowseLibrary', 790, 580, 1, 1);
}
function selectLibraryItem(libraryid, folder, qtitle, filename, type, returnField, sitePath){
    var title = document.getElementById("sitemaptitle_input");
    var url = document.getElementById("sitemapurl_input");
     if (url != null)
	   {    
	        if(sitePath == "/")
	        { 
	            if(filename.indexOf("/") == 0)
	                  url.value = filename.substring(1);
	            else
	                  url.value = filename;
	        }
	        else if(filename.indexOf("http://") > -1 ||filename.indexOf("https://")>-1)
	              url.value = filename;
	        else
		          url.value = filename.replace(sitePath, "");
	   }   
	if (title != null)
	{
	    if (title.value == '') {
	        title.value = qtitle;
	    }
	}
	url = null;
	title = null;
	
}

function makeStringSafe(val)
{
    val = val.replace(/\'/g, "\'");   
    val = val.replace(/\"/g, '\"');
    return val;
}

function makeSafeId(val)
{
    val = val.replace(/&#39;/g, "_");
    val = val.replace(/\'/g, "_");
    val = val.replace(/\"/g, '__');
    return val;
}

function encodeAmpersand(title)
{
//only this function
//hint "normalize"
   title=title.replace(/&lt;/g, '<');
   title=title.replace(/&gt;/g,'>');
   title=title.replace(/&quot;/g,'\"');
   title=title.replace(/&#39;/g,'\'');
   title=title.replace(/amp;/g,'');
   title=title.replace(/&/g,'&amp;');
   title=title.replace(/</g, '&lt;');
   title=title.replace(/>/g,'&gt;');
   title=title.replace(/\"/g,'&quot;');
   title=title.replace(/\'/g,"&#39;");
   return title
}
