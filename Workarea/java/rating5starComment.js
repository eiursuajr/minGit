function dhdlr(key, dtype)
	    {
	        switch (dtype)
	        {
	            case "out" :
	                (document.getElementById(key + 'Popup')).style.position = 'absolute';
	                (document.getElementById(key + 'Popup')).style.visibility = 'hidden';
	                break;
	            case "over" :
	                (document.getElementById(key + 'Popup')).style.position = '';
	                (document.getElementById(key + 'Popup')).style.visibility = 'visible';
	                break;
	            case "close" :
	                (document.getElementById(key + 'Popup')).style.position = 'absolute';
	                (document.getElementById(key + 'Popup')).style.visibility = 'hidden';
	               break;
                case "rate" :
	                (document.getElementById(key + 'Popup')).style.position = 'absolute';
	                (document.getElementById(key + 'Popup')).style.visibility = 'hidden';
	                (document.getElementById(key + 'Popup_Msg')).style.visibility = 'visible';
                   //debugger;
                    setTimeout("dhdlr('" + key + "', 'closemsg');",1000);
                    break;
   	            case "closemsg" :
	                (document.getElementById(key + 'Popup_Msg')).style.visibility = 'hidden';
                    break;

            }
            
            
        }
function hideTag(key)
{
    
	//(document.getElementById(key + 'Popup_Msg')).style.visibility = 'hidden';
    eval('_LoadReview' + key + '("","control=' + key + '");');
}
	          
function rhdlr(key, rposition, rtype)
	    {
	        var rDef = document.getElementById(key + 'ekrelected').value;
	        var s_ipath = document.getElementById('ekimgpath').value;
	        var riS = s_ipath + 'star_s.gif';;
	        var riF = s_ipath + 'star_f.gif';
	        var riE = s_ipath + 'star_e.gif';
	        var sreview = encodeURIComponent(document.getElementById(key + '_tbComments').value);
	        var bmod = document.getElementById(key + '_mod').value
	        if (rtype == "out")
	        {
	            //(document.getElementById('Popup')).style.position = 'absolute';
	            //(document.getElementById('Popup')).style.visibility = 'hidden';
	            for (var i = 2; i <= 10; i = i + 2)
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
	                        document.getElementById(key + 'ri' + i).src = riS;
	                    }
	                    else
	                    {
	                        document.getElementById(key + 'ri' + i).src = riF;
	                    }
	                }
	                else
	                {
	                    document.getElementById(key + 'ri' + i).src = riE;
	                }
	            }
	        }
	        else if (rtype == "over")
	        {
	            if ((document.getElementById(key + 'Popup')).style.visibility != 'visible')
	            {
	                (document.getElementById(key + 'Popup')).style.position = 'absolute';
	                (document.getElementById(key + 'Popup')).style.visibility = 'visible';
	            }
	            for (var i = 2; i <= 10; i = i + 2)
	            {
	                if (i <= rposition)
	                {
	                    document.getElementById(key + 'ri' + i).src = riS;
	                }
	                else
	                {
	                    if (i <= rDef)
	                    {
	                        document.getElementById(key + 'ri' + i).src = riE;
	                    }
	                    else
	                    {
	                        document.getElementById(key + 'ri' + i).src = riE;
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
	        else if (rtype == "rate")
	        {
	            var iekcontent = document.getElementById(key + 'ekcontentid').value;
	            var atext = new Array(rposition, iekcontent, true, '');
	            AddEditRating(atext,'', key);
	            document.getElementById(key + '_israted').value = 1;
	        }
	        else // "click"
	        {
	            var iekcontent = document.getElementById(key + 'ekcontentid').value;
	            rposition = document.getElementById(key + 'ekrelected').value
	            var atext = new Array(rposition, iekcontent, bmod, sreview);
	            AddEditRating(atext,'',key);
	            document.getElementById(key + '_israted').value = 1;
	            dhdlr(key, 'rate');
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
      // hideTag(key);
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
	        rhdlr(key, (response), 'save');
    }else{
            alert('fail');
    } 

  }else{
    // Input mode
    var s_path = document.getElementById('ekapppath').value;
    url = s_path + 'AJAXbase.aspx?action=addeditcontentrating&key=' + key + '&rating=' + input[0] + '&contentid=' + input[1] + '&approved=' + input[2] + '&review=' + escape(input[3]);
    // alert(url);
    loadXMLDoc(url);
  }

}

function ValidateReviewForm()
{
    var bret = true;
    var sErr = '';
    var key = document.getElementById('ekkey').value;
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

function ClearReviewForm(key)
{
    var key = document.getElementById('ekkey').value;
    document.getElementById(key + '_rating').value = 0;
    document.getElementById(key + 'ekrelected').value = 0;
    document.getElementById(key + '_actualValue').value = '';
    // document.getElementById(key + '_agree').checked = false;
	rhdlr(key, 0,'out');
}
