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

function fhdlr(key, fposition, ftype)
{
    //  var fDef = document.getElementById(key + 'ekrelected').value;
    var s_ipath = document.getElementById('ekimgpath').value;
    var scomment = document.getElementById(key + '_flagMessage').value;
    var ilang = document.getElementById(key + '_lang').value;
    switch (ftype)
    {
        case "close" :
            toggleVisibility(key + '_flagItPanel')
            break;
        default : // "click"
            if (ValidateFlagForm(key, fposition) == true)
            {
                var iekcontent = document.getElementById(key + 'ekcontentid').value;
                var atext = new Array(fposition, iekcontent, scomment, ilang);
                AddEditFlag(atext, '', key);
                document.getElementById(key + '_israted').value = 1;
            }
    } 
}
	   
var req;
function loadXMLDoc(url) 
{
    // branch for native XMLHttpRequest object
    if (window.XMLHttpRequest) {
        req = new XMLHttpRequest();
        req.onreadystatechange = processReqChange;
        req.open("GET", url, true);
        req.send(null);
    // branch for IE/Windows ActiveX version
    } else if (window.ActiveXObject) {
        req = new ActiveXObject("Microsoft.XMLHTTP");
        if (req) {
            req.onreadystatechange = processReqChange;
            req.open("GET", url, true);
            req.send();
        }
    }
}
function processReqChange() 
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
      
      eval(method + '(\'\', result, key);');
        } else {
            alert("There was a problem retrieving the XML data:\n" + req.statusText);
        }
    }
}

function AddEditFlag(input, response, key)
{
  if (response != ''){ 
    // Response mode
    if (response > -1){
	        fhdlr(key, (response), 'close');
    }else{
            alert('fail');
    } 

  }else{
    // Input mode
    var s_path = document.getElementById('ekapppath').value;
    url = s_path + 'AJAXbase.aspx?action=addeditcontentflag&lang=' + input[3] + '&flag=' + input[0] + '&contentid=' + input[1] + '&key=' + key + '&comment=' + escape(input[2]);
    // alert(url);
    loadXMLDoc(url);
  }

}

function ValidateFlagForm(key, fposition)
{
    var bret = true;
    var sErr = '';
    var scomment = document.getElementById(key + '_flagMessage').value;
    if (fposition == "")
    {
        sErr = "You need to select a flag.\n";
        bret = false;
    }
//  uncomment out these lines if you want to require comment text.	
//	if (scomment == '')
//	{
//	    sErr = sErr + "You need to provide a comment.\n";
//	    bret = false;
//	}
	if (bret == false)
	{
	    alert(sErr);
	}
	return bret;
}
