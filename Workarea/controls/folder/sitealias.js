var arSiteAliasNames = new Array();
var selectedNode;

function item(aliasname,order)
{
    this.aliasname=aliasname;
    this.order=order;
}

function clearAliasNameList()
{
    var AliasNames = document.getElementById("divSiteAliasList");
    AliasNames.innerHTML = '';
}

function renderSiteAliasNames() {

    clearAliasNameList();
    for (var index = 0; index < arSiteAliasNames.length; index++)
    {
        createAliasList(arSiteAliasNames[index]);
    }
}

function addSiteAliasName()
{
    var aliasname = document.getElementById("txtAliasName");
    var len = aliasname.value.length;


    if (Trim(aliasname.value) == '')
    {
        alert(SiteAliasNoNameMsg);
        aliasname.focus();
        return false;
    }


    if(aliasname.value.charAt(len-1) == '/')
    {

       alert("Alias Name cannot end with a slash '/' ");
       aliasname.value='';
       aliasname.focus();
       return false;
     }


     if(aliasname.value.indexOf('<')!=-1 || aliasname.value.indexOf('>')!=-1 || aliasname.value.indexOf('?') != -1 || aliasname.value.indexOf(',') != -1 || aliasname.value.indexOf('\\') != -1 || aliasname.value.indexOf(':') != -1 || aliasname.value.indexOf('|') != -1 || aliasname.value.indexOf('\'') != -1 || aliasname.value.indexOf('^') != -1 || aliasname.value.indexOf('$') != -1 || aliasname.value.indexOf('&') != -1 || aliasname.value.indexOf('%') != -1 || aliasname.value.indexOf('!') != -1 || aliasname.value.indexOf('*') != -1 || aliasname.value.indexOf('\"') != -1)
    {
       // alert("'<','?' and '>' are not valid characters");
        alert(",\<>:|?'&^%$!*\" are not valid characters");
        aliasname.value='';
        aliasname.focus();
        return false;
    }

    if (selectedNode != null)
    {
        arSiteAliasNames[selectedNode.order].aliasname = aliasname.value.toString();
        renderSiteAliasNames();
        clearAliasName();
    }
    else
    {
        objItem = new item(aliasname.value.toString(),arSiteAliasNames.length);
        arSiteAliasNames[arSiteAliasNames.length] = objItem;
        createAliasList(objItem);
    }
    aliasname.value = '';
    aliasname.focus();

}

function createAliasList(item)
{
    var span_elem;
    var att;
    var aliasList = document.getElementById("divSiteAliasList");
    var newDivAliasList = document.createElement("div");

    var newEntry = document.createElement("div");
    newEntry.id = "sitealias_" + item.aliasname.toLowerCase();
    newEntry.className = "SitemapPath";

    setOnClickSelectAliasName(newEntry, item);

    span_elem = document.createElement("span");
    span_elem.id = "sitealias_links_" + item.aliasname.toLowerCase();
    span_elem.innerHTML = "<img onclick=\"deleteAliasName('" + item.aliasname + "');\" alt=\"" + SiteAliasDelIconTooltip + "\" src=\"images/application/delete2.gif\" />&nbsp;&nbsp;&nbsp;&nbsp;";
    newEntry.appendChild(span_elem);

    span_elem = document.createElement("span");
    span_elem.id = "sitealias_title_" + item.aliasname.toLowerCase();
    span_elem.innerHTML =  item.aliasname;
    newEntry.appendChild(span_elem);

    newDivAliasList.appendChild(newEntry)
    aliasList.appendChild(newDivAliasList);

}

function editAliasName()
{


    document.getElementById("txtAliasName").value = selectedNode.aliasname;
    document.getElementById("btnAddSiteAlias").value = "Save";
}
function deleteAliasName(id)
{

    fireSelectAliasName(id);
    var obj = document.getElementById("sitealias_" + id.toLowerCase()).parentNode;
    obj.parentNode.removeChild(obj);

    arSiteAliasNames.splice(selectedNode.order, 1);

    for (var i=0; i<arSiteAliasNames.length; i++) {
            arSiteAliasNames[i].order = i;
    }

      clearAliasName();

}

function clearAliasName()
{
    document.getElementById("txtAliasName").value = "";
    document.getElementById("btnAddSiteAlias").value = SiteAliasAddBtnText;
    selectAliasName(null, null);
}

function selectAliasName(elem, item)
{
    selectedNode = item;
    var elements = DOMUtil.getElementByClassName( "ektronSitemapPathSelected" );
    for(var i=0;i<elements.length;i++)
    {
        elements[i].className = "SitemapPath";
    }
    if (elem != null) {
        elem.className = "ektronSitemapPathSelected";
        editAliasName();
    }
}

function fireSelectAliasName(id)
{


    var obj = document.getElementById("sitealias_" + id.toLowerCase())
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

function setOnClickSelectAliasName(elem, item)
{
    elem.onclick = function() { selectAliasName( this, item ); }
}

function viewSiteAlias()
{
    var viewAlias = document.getElementById("viewSiteAliasList");
    var  aliasList= '';
    for (var i = 0; i < arSiteAliasNames.length; i++)
    {
        if (aliasList == '')
        {
            aliasList = arSiteAliasNames[i].aliasname;
        } else {
           aliasList = aliasList + ' , ' + arSiteAliasNames[i].aliasname;

        }
    }
    viewAlias.innerHTML = aliasList;
}

function saveSiteAliasList()
{
    var aliasNameList = " " ;
    for (var i=0;i<arSiteAliasNames.length;i++)
    {
        aliasNameList = aliasNameList + "," + arSiteAliasNames[i].aliasname;

    }
    if (document.getElementById("savedSiteAlias"))
    {
        document.getElementById("savedSiteAlias").value = aliasNameList;
    }
}

