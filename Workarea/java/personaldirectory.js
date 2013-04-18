function toggleVisibility(itm)
{
  var itmref = document.getElementById(itm);
  if (itmref != null && itmref.style.display == 'none')
  {
	document.getElementById(itm).style.display = '';
  }
  else
  {
	document.getElementById(itm).style.display = 'none';
  }
}

function ismaxlength(obj, key, ty, maxlen){
//        var mlength=obj.getAttribute? parseInt(obj.getAttribute("maxlength")) : 50;
        var spanobj = document.getElementById('ek_fcharspan' + key);
        if (spanobj != null) 
        { 
            
            if (ty == 'up' && obj.value.length > maxlen) { obj.value = obj.value.substring(0, maxlen); }
            spanobj.innerHTML = obj.value.length + '/' + maxlen;
        }
    }

function pdhdlr(act, key)
{
    var nodeid = document.getElementById(key + '_inode').value;
    switch (act)
    {
        case "remove" :
            var sItemList = '';
            var e=document.getElementsByTagName("input");
            for(var i=0; i < e.length; i++)
            {
                
                if (e[i].type == "checkbox" && e[i].checked == true && (e[i].id.indexOf(key + "_f") == 0 || e[i].id.indexOf(key + "_i") == 0))
                {
                    if (sItemList == '')
                    {
                        sItemList = e[i].id;
                    }
                    else
                    {
                        sItemList = sItemList + ',' + e[i].id;
                    }
                }
            }
            if (sItemList == '')
            {
                alert('Please select items to remove.');
            }
            else
            {
                var atext = new Array(nodeid, sItemList);
                if (confirm('Delete these items?') == true)
                {
                    RemoveItems(atext, '', key);
                }
            }
            break;
        case "add" :
            var sname = document.getElementById(key + '_fName').value;
            var sdesc = document.getElementById(key + '_fDesc').value;
            if (ValidateFolderForm(key) == true)
            {
                var atext = new Array(nodeid, sname, sdesc);
                AddPersonalFolder(atext,'',key);
            }   
            break;
        case "addfav" :
            var oSpan = document.getElementById(key + '_add2fav');
            oSpan.innerHTML = 'Adding...';
            var atext = new Array('add', 1);
            AddtoFavorite(atext,'',key);
            break;
        case "checkfav" :
            var atext = new Array('check', 1);
            AddtoFavorite(atext,'',key);
            break;
        case "addfr" :
            var oSpan = document.getElementById(key + '_add2fr');
            oSpan.innerHTML = 'Adding...';
            var atext = new Array('add', 1);
            AddtoFriend(atext,'',key);
            break;
        case "checkfr" :
            var atext = new Array('check', 1);
            AddtoFriend(atext,'',key);
            break;
    }
}
	   
var req = null;
function pd_loadXMLDoc(url) 
{
    // branch for native XMLHttpRequest object
    if (window.XMLHttpRequest) {
        req = new XMLHttpRequest();
        req.onreadystatechange = pd_processReqChange;
        req.open("GET", url, true);
        req.send(null);
    // branch for IE/Windows ActiveX version
    } else if (window.ActiveXObject) {
        req = new ActiveXObject("Microsoft.XMLHTTP");
        if (req) {
            req.onreadystatechange = pd_processReqChange;
            req.open("GET", url, true);
            req.send();
        }
    }
}
function pd_processReqChange() 
{
    // only if req shows "complete"
    if (req.readyState == 4) {
        // only if "OK"
        if (req.status == 200) {
            // ...processing statements go here...
            
      response  = req.responseXML.documentElement;

      method    = response.getElementsByTagName('method')[0].firstChild.data;

      result    = response.getElementsByTagName('result')[0].firstChild.data;

      key       = response.getElementsByTagName('key')[0].firstChild.data;
        
      req       = null; 
      
      eval(method + '(\'\', result, key);');
        } else {
            alert("There was a problem retrieving the XML data:\n" + req.statusText);
        }
    }
}

function RemoveItems(input, response, key)
{
  if (response != ''){ 
    // Response mode
    if (response > -1){
	        window.location = window.location.href;
    }else{
            alert('fail');
    } 

  }else{
    // Input mode
    var s_path = document.getElementById('ekapppath').value;
    url = s_path + 'AJAXbase.aspx?action=removeitems&node=' + input[0] + '&itemlist=' + input[1] + '&key=' + key;
    pd_loadXMLDoc(url);
  }
}

function AddtoFavorite(input, response, key)
{
  if (response != ''){ 
    // Response mode
    if (response == -1){
	    alert('fail');
    }else{
        var oSpan = document.getElementById(key + '_add2fav');
        oSpan.innerHTML = response;
    } 
  }else{
    // Input mode
    var s_path = document.getElementById('ekapppath').value;
    url = s_path + 'AJAXbase.aspx?action=addfavorite&mode=' + input[0] + '&item=' + input[1] + '&key=' + key;
    pd_loadXMLDoc(url);
  }
}

function AddtoFriend(input, response, key)
{
  if (response != ''){ 
    // Response mode
    if (response == -1){
	    alert('fail');
    }else{
        var oSpan = document.getElementById(key + '_add2fr');
        oSpan.innerHTML = response;
    } 

  }else{
    // Input mode
    var s_path = document.getElementById('ekapppath').value;
    url = s_path + 'AJAXbase.aspx?action=addfriend&mode=' + input[0] + '&item=' + input[1] + '&key=' + key;
    pd_loadXMLDoc(url);
  }
}

function AddPersonalFolder(input, response, key)
{
  if (response != ''){ 
    // Response mode
    if (response > -1){
	        toggleVisibility(key + '_addfolderpanel');
	        window.location = window.location.href;
    }else{
            alert('fail');
    } 

  }else{
    // Input mode
    var s_path = document.getElementById('ekapppath').value;
    url = s_path + 'AJAXbase.aspx?action=addpersonalfolder&node=' + input[0] + '&name=' + input[1] + '&key=' + key + '&desc=' + escape(input[2]);
    pd_loadXMLDoc(url);
  }
}

function ValidateFolderForm(key,alertFName)
{
    var bret = true;
    var sErr = '';
    var sname = document.getElementById(key + '_fName').value;
    
    if (sname == "")
    {
        sErr = alertFName + "\n";
        bret = false;
    }
	if (bret == false)
	{
	    alert(sErr);
	}
	return bret;
}
function ValidateLinkForm(key,alertName,alertLink)
{
    var bret = true;
    var sErr = '';
    var sname = document.getElementById(key + '_LName').value;
    var slink = document.getElementById(key + '_Link').value;
    
    if (sname == "")
    {
        sErr = alertName+"\n";
        alert(sErr);
        bret = false;
    }
	if (slink == "")
	{
	    sErr='';
	    sErr = alertLink+"\n";
        alert(sErr);
        bret = false;
	}
	return bret;
}
function ReturnMoveDir(key)
{
    var e=document.getElementsByTagName("input");
    for (var i=0; i < e.length; i++)
    {
        if (e[i].type == "radio" && e[i].checked == true && e[i].id.indexOf(key + "_mvdir_") == 0)
        {
            return e[i].value.substring((key + "_mv_").length);
        }
    }
}

