function rhdlr(key, rposition, rtype)
	    {
	        var rDef = document.getElementById(key + 'ekrelected').value;
	        var s_ipath = document.getElementById('ekimgpath').value;
	        var rilS = s_ipath + 'star_l_s.gif';
	        var rirS = s_ipath + 'star_r_s.gif';
	        var rilF = s_ipath + 'star_l_f.gif';
	        var rirF = s_ipath + 'star_r_f.gif';
	        var rilE = s_ipath + 'star_l_e.gif';
	        var rirE = s_ipath + 'star_r_e.gif';
	        if (rtype == "out")
	        {
	            for (var i = 1; i <= 10; i++)
	            {
	                if (i <= rDef)
	                {
	                    var bIsrated = 1;
	                    try
	                    {
	                        bIsrated = document.getElementById(key + '_israted').value;
	                    }
	                    catch(err) { }
	                    if (bIsrated == 1)
	                    {
	                        if (i % 2 == 0)
	                        {
	                            document.getElementById(key + 'ri' + i).src = rirS;
	                        }
	                        else
	                        {
	                            document.getElementById(key + 'ri' + i).src = rilS;
	                        }
	                    }
	                    else
	                    {
	                        if (i % 2 == 0)
	                        {
	                            document.getElementById(key + 'ri' + i).src = rirF;
	                        }
	                        else
	                        {
	                            document.getElementById(key + 'ri' + i).src = rilF;
	                        }
	                    }
	                }
	                else
	                {
	                    if (i % 2 == 0)
	                    {
	                        document.getElementById(key + 'ri' + i).src = rirE;
	                    }
	                    else
	                    {
	                        document.getElementById(key + 'ri' + i).src = rilE;
	                    }
	                }
	            }
	        }
	        else if (rtype == "over")
	        {
	            for (var i = 1; i <= 10; i++)
	            {
	                if (i <= rposition)
	                {
	                    if (i % 2 == 0)
	                    {
	                        document.getElementById(key + 'ri' + i).src = rirS;
	                    }
	                    else
	                    {
	                        document.getElementById(key + 'ri' + i).src = rilS;
	                    }
	                }
	                else
	                {
	                    if (i <= rDef)
	                    {
	                        if (i % 2 == 0)
	                        {
	                            document.getElementById(key + 'ri' + i).src = rirE;
	                        }
	                        else
	                        {
	                            document.getElementById(key + 'ri' + i).src = rilE;
	                        }
	                    }
	                    else
	                    {
	                        if (i % 2 == 0)
	                        {
	                            document.getElementById(key + 'ri' + i).src = rirE;
	                        }
	                        else
	                        {
	                            document.getElementById(key + 'ri' + i).src = rilE;
	                        }
	                    }
	                }
	            }
	        }
	        else if (rtype == "save")
	        {
	            document.getElementById(key + 'ekrelected').value = rposition;
	            rhdlr(key, 0,'out');
	        }
	        else if (rtype == "select")
	        {
	            document.getElementById(key + 'ekrelected').value = rposition;
	            document.getElementById(key + '_rating').value = rposition;
	            rhdlr(key, 0,'out');
	        }
	        else // "click"
	        {
	            var iekcontent = document.getElementById(key + 'ekcontentid').value;
	            var atext = new Array(rposition, iekcontent, false);
	            AddEditRating(atext,'', key);
	            document.getElementById(key + '_israted').value = 1;
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
      
      key    = response.getElementsByTagName('key')[0].firstChild.data;
      
      eval(method + '(\'\', result, key);');
        } else {
            alert("There was a problem retrieving the XML data:\n" + req.statusText);
        }
    }
}

function AddEditRating(input, response, key)
{
  if (response != ''){ 
    // Response mode
    if (response > -1){
	        // alert('success');
	        rhdlr(key, response, 'save');
    }else{
            alert('fail');
    } 

  }else{
    // Input mode
    var s_path = document.getElementById('ekapppath').value;
    url = s_path + 'AJAXbase.aspx?action=addeditcontentrating&rating=' + input[0] + '&contentid=' + input[1] + '&approved=' + input[2] + '&key=' + key;
    // alert(url);
    loadXMLDoc(url);
  }

}

function ValidateReviewForm(key)
{
    var bret = true;
    var sErr = '';
    if (document.getElementById(key + '_rating').value < 1)
    {
        sErr = "You need to provide a rating.\n";
        bret = false;
    }
//  uncomment out these lines if you want to require review text.	
//	if (document.getElementById(key + '_actualValue').value == '')
//	{
//	    sErr = sErr + "You need to provide a review.\n";
//	    bret = false;
//	}
//  uncomment out these lines if you want to add a terms and conditions that needs to be accepted.	
//	if (document.getElementById(key + '_agree').checked == false)
//	{
//	    sErr = sErr + "You must agree to the terms and conditions.\n";
//	    bret = false;
//	}
	if (bret == false)
	{
	    alert(sErr);
	}
	else
	{
	    document.getElementById(key + '_wasSubmitted').value = '1';
	}
	return bret;
}

function ClearReviewForm()
{
    var key = document.getElementById('ekkey').value;
    document.getElementById(key + '_rating').value = 0;
    document.getElementById(key + 'ekrelected').value = 0;
    document.getElementById(key + '_actualValue').value = '';
    // document.getElementById(key + '_agree').checked = false;
	rhdlr(key, 0,'out');
}
