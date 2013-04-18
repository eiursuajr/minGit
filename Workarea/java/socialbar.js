function SB_Error(key, serr, sbidx)
{
    document.getElementById(key + '_sbh_' + sbidx).onclick = 'void(0);';
    document.getElementById(key + '_sbh_' + sbidx).innerHTML = serr;
}

function SB_Remove(key, oid, otype, sbidx, ilang)
{
    document.getElementById(key + '_sbh_' + sbidx).onclick = 'void(0);';
    document.getElementById(key + '_sbh_' + sbidx).innerHTML = Ektron.SocialBar.WrapText('Removing...', Ektron.SocialBar.WrapLinkTextClass);
    var atext = new Array('remove', ilang);
    AJAX_AddTo(atext, '', key, oid, otype, sbidx);
}

function SB_AddTo(key, oid, otype, sbidx, ilang)
{
    document.getElementById(key + '_sbh_' + sbidx).onclick = 'void(0);';
    document.getElementById(key + '_sbh_' + sbidx).innerHTML = Ektron.SocialBar.WrapText('Adding...', Ektron.SocialBar.WrapLinkTextClass);
    var atext = new Array('add', ilang);
    AJAX_AddTo(atext, '', key, oid, otype, sbidx);
}

function SB_AddLinkTo(key,link,title, otype, sbidx, ilang)
{
    
    document.getElementById(key + '_sbh_' + sbidx).onclick = 'void(0);';
    document.getElementById(key + '_sbh_' + sbidx).innerHTML = Ektron.SocialBar.WrapText('Adding...', Ektron.SocialBar.WrapLinkTextClass);
    var atext = new Array('addlink', ilang);
    AJAX_AddLinkTo(atext, '', key, link,title,otype,sbidx);
}

function SB_RemoveLink(key, link, title, otype, sbidx, ilang)
{
    
    document.getElementById(key + '_sbh_' + sbidx).onclick = 'void(0);';
    document.getElementById(key + '_sbh_' + sbidx).innerHTML = Ektron.SocialBar.WrapText('Removing...', Ektron.SocialBar.WrapLinkTextClass);
    var atext = new Array('removelink', ilang);
    AJAX_AddLinkTo(atext, '', key, link,title, otype,sbidx);
}
function SB_Process(key, response, sb_msg, sbtext, oid, otype, idx, sbtext, ilang)
{
    // if (sb_msg != '') { alert(sb_msg); }
    var s_imgpath = document.getElementById('ekimgpath').value;
    var image = document.getElementById(key + '_sbimg_' + idx);
    if (response == -1 && otype == 19)
    {
        if (image != null) 
            image.src = s_imgpath + 'bookmarks/invite.gif';
        document.getElementById(key + '_sbh_' + idx).innerHTML = Ektron.SocialBar.WrapText(sbtext, Ektron.SocialBar.WrapLinkTextClass);
        document.getElementById(key + '_sbh_' + idx).alt = sbtext;
        document.getElementById(key + '_sbh_' + idx).onclick = 'void(0);';
        document.getElementById(key + '_sbh_' + idx).href = 'javascript: void(0)';
    }
    else if (response == 1)
    {
        if (image != null) 
            image.src = s_imgpath + 'bookmarks/addto.gif';
        document.getElementById(key + '_sbh_' + idx).innerHTML = Ektron.SocialBar.WrapText(sbtext, Ektron.SocialBar.WrapLinkTextClass);
        document.getElementById(key + '_sbh_' + idx).alt = sbtext;
        document.getElementById(key + '_sbh_' + idx).title = sbtext;
        document.getElementById(key + '_sbh_' + idx).href = "javascript: SB_AddTo('" + key + "', " + oid + ", '" + otype + "', " + idx + ", " + ilang + "); void(0)";
        document.getElementById(key + '_sbh_' + idx).onclick = "";
    }
    else if (response == 0)
    {
        if (image != null) 
            image.src = s_imgpath + 'bookmarks/remove.gif';
        document.getElementById(key + '_sbh_' + idx).innerHTML = Ektron.SocialBar.WrapText(sbtext, Ektron.SocialBar.WrapLinkTextClass);
        document.getElementById(key + '_sbh_' + idx).alt = sbtext;
        document.getElementById(key + '_sbh_' + idx).title = sbtext;
        document.getElementById(key + '_sbh_' + idx).href = "javascript: SB_Remove('" + key + "', " + oid + ", '" + otype + "', " + idx + ", " + ilang + "); void(0)";
        document.getElementById(key + '_sbh_' + idx).onclick = "";
    }
}
function SB_ProcessLink(key, response, sb_msg, sbtext, link , title, otype, idx, sbtext, ilang)
{
    // if (sb_msg != '') { alert(sb_msg); }
    var s_imgpath = document.getElementById('ekimgpath').value;
    var image = document.getElementById(key + '_sbimg_' + idx);
    if (response == -1 && otype == 19)
    {
        if (image != null) 
            image.src = s_imgpath + 'bookmarks/invite.gif';
        document.getElementById(key + '_sbh_' + idx).innerHTML = Ektron.SocialBar.WrapText(sbtext, Ektron.SocialBar.WrapLinkTextClass);
        document.getElementById(key + '_sbh_' + idx).alt = sbtext;
        document.getElementById(key + '_sbh_' + idx).onclick = 'void(0);';
        document.getElementById(key + '_sbh_' + idx).href = 'javascript: void(0)';
    }
    else if (response == 1)
    {
        if (image != null) 
            image.src = s_imgpath + 'bookmarks/addto.gif';
        document.getElementById(key + '_sbh_' + idx).innerHTML = Ektron.SocialBar.WrapText(sbtext, Ektron.SocialBar.WrapLinkTextClass);
        document.getElementById(key + '_sbh_' + idx).alt = sbtext;
        document.getElementById(key + '_sbh_' + idx).title = sbtext;
        document.getElementById(key + '_sbh_' + idx).href = "javascript: SB_AddLinkTo('" + key + "', '" + link + "','" + title +"', '" + otype + "', " + idx + ", " + ilang + "); void(0)";
        document.getElementById(key + '_sbh_' + idx).onclick = "";
        
    }
    else if (response == 0)
    {
        if (image != null) 
            image.src = s_imgpath + 'bookmarks/remove.gif';
        document.getElementById(key + '_sbh_' + idx).innerHTML = Ektron.SocialBar.WrapText(sbtext, Ektron.SocialBar.WrapLinkTextClass);
        document.getElementById(key + '_sbh_' + idx).alt = sbtext;
        document.getElementById(key + '_sbh_' + idx).title = sbtext;
        document.getElementById(key + '_sbh_' + idx).href = "javascript: SB_RemoveLink('" + key + "', '" + link + "','" + title + "', '" + otype + "', " + idx + ", " + ilang + "); void(0)";
        document.getElementById(key + '_sbh_' + idx).onclick = "";
    }
    
}
  	   
var req;
function SB_loadXMLDoc(url) 
{
    if (window.XMLHttpRequest) { // branch for native XMLHttpRequest object
        req = new XMLHttpRequest();
        req.onreadystatechange = SB_processReqChange;
        req.open("GET", url, true);
        req.send(null);
    } else if (window.ActiveXObject) { // branch for IE/Windows ActiveX version
        req = new ActiveXObject("Microsoft.XMLHTTP");
        if (req) {
            req.onreadystatechange = SB_processReqChange;
            req.open("GET", url, true);
            req.send();
        }
    }
}
function SB_processReqChange() 
{
    if (req.readyState == 4) { // only if req shows "complete"
        if (req.status == 200) { // only if "OK"
          response  = req.responseXML.documentElement;
          method    = response.getElementsByTagName('method')[0].firstChild.data;
          result    = response.getElementsByTagName('result')[0].firstChild.data;
          key       = response.getElementsByTagName('key')[0].firstChild.data;
          sb_msg    = response.getElementsByTagName('returnmsg')[0].firstChild.data;
          idx       = response.getElementsByTagName('idx')[0].firstChild.data;
          //var action = (response.getElementsByTagName('action').length > 0) ? response.getElementsByTagName('action')[0].firstChild.data : "";
          var action = (
            response.getElementsByTagName('action') != null
            && response.getElementsByTagName('action').length > 0 
            && response.getElementsByTagName('action')[0] != null
            && response.getElementsByTagName('action')[0].length > 0 
            && response.getElementsByTagName('action')[0].firstChild != null
            && response.getElementsByTagName('action')[0].firstChild.length > 0 
            && response.getElementsByTagName('action')[0].firstChild.data != null
            && response.getElementsByTagName('action')[0].firstChild.data.length > 0 
            ) ? response.getElementsByTagName('action')[0].firstChild.data : "";
          if (result == 'error')
          {
              SB_Error(key, sb_msg, idx);
          }
          else
          {
              oid       = response.getElementsByTagName('oid')[0].firstChild.data;
              otype     = response.getElementsByTagName('otype')[0].firstChild.data;
              sbtext    = response.getElementsByTagName('retmsg')[0].firstChild.data;
			  ilang     = response.getElementsByTagName('ilang')[0].firstChild.data;
			  if( oid == 0)
			  {
			    title     = response.getElementsByTagName('title')[0].firstChild.data;
			    link      = response.getElementsByTagName('link')[0].firstChild.data;
			     eval(method + '(sb_msg, result, key, link, title, otype, idx, sbtext, ilang);');
			  }
			  else
			  {  
                eval(method + '(sb_msg, result, key, oid, otype, idx, sbtext, ilang);');
              }
              
	        // set the class name of the containing anchor-tag, if the action is specified:
	        if (action != null && "undefined" != typeof action.length && action.length > 0 && "undefined" != typeof $ektron) {
	            switch (action) {
	                case "addFavorite":
	                        $ektron('#' + key + '_sbh_' + idx).removeClass('removeFavorite').addClass('addFavorite');
	                    break;
	                    
	                case "removeFavorite":
	                        $ektron('#' + key + '_sbh_' + idx).removeClass('addFavorite').addClass('removeFavorite');
	                    break;
	                    
	                default:
	                    // do nothing.
	            }
	        }
	        
		        //dispatch ajax finished event
		        $ektron(document).trigger("EktronSocialbarAjaxFinished", [method]);
          }
        } else {
            alert("There was a problem retrieving the XML data:\n" + req.statusText);
        }
    }
}

function AJAX_AddTo(input, response, key, oid, otype, sbidx, sbtext, ilang)
{
  if (response != ''){ 
    SB_Process(key, response, input, sbtext, oid, otype, sbidx, sbtext, ilang);
  }else{
    var s_path = document.getElementById('ekapppath').value;
    url = s_path + 'AJAXbase.aspx?action=addto&oid=' + oid + '&otype=' + otype + '&lang=' + input[1] + '&mode=' + input[0] + '&idx=' + sbidx + '&key=' + key +'&sbtext=' + sbtext;
    SB_loadXMLDoc(url);
  }

}

function AJAX_AddLinkTo(input, response, key, link, title, otype, sbidx, sbtext, ilang)
{
  
  if (response != ''){ 
    SB_ProcessLink(key, response, input, sbtext, link, title, otype, sbidx, sbtext, ilang);
  }else{
    var s_path = document.getElementById('ekapppath').value;
    url = s_path + 'AJAXbase.aspx?action=addto&link=' + link + '&otype=' + otype + '&lang=' + input[1] + '&mode=' + input[0] + '&idx=' + sbidx + '&key=' + key +'&title=' + title;
    SB_loadXMLDoc(url);
  }

}

/// <reference path="ektron.js" />
// Copyright 2010 Ektron, Inc.

// SocialBar singleton
if (typeof Ektron.SocialBar != "object") Ektron.SocialBar = {};
if (typeof Ektron.SocialBar.WrapText == "undefined") {
    Ektron.SocialBar.WrapText = function (text, className) {
        return ("<span class='" + className + "'>" + text + "</span>");
    };
}
if (typeof Ektron.SocialBar.WrapLinkTextClass == "undefined") {
    Ektron.SocialBar.WrapLinkTextClass = "EktronSocialBarLinkText";
}
if ("undefined" == typeof Ektron.SocialBar.instances) 
{
	$ektron.extend(Ektron.SocialBar, new (function()
	{
		this.instances = [];

		this.add = function SocialBar_add(objInstance)
		{
			if (!objInstance) return;
			try
			{
				this.remove(objInstance); // prevent duplicates
				this.instances[this.instances.length] = objInstance;
				this.instances[objInstance.id] = objInstance;
			}
			catch (ex)
			{
				Ektron.OnException(this, null, ex, arguments);
			}
		};
		
		this.remove = function SocialBar_remove(objInstance)
		{
			if (!objInstance) return;
			try
			{
				for (var i = 0; i < this.instances.length; i++)
				{
					if (objInstance == this.instances[i])
					{
						this.instances.splice(i, 1);
						break;
					}
				}
				this.instances[objInstance.id] = null;
				delete this.instances[objInstance.id];
				
				if (0 === this.instances.length)
				{
					$ektron("ul.ekSocialBar a").die("click", Ektron.SocialBar.onClick);
				}
			}
			catch (ex)
			{
				Ektron.OnException(this, null, ex, arguments);
			}
		};
		
		this.onClick = function SocialBar_onClick()
		{
			try
			{
				var hyperlinkElement = this; // the source of the onclick event
				var id = $ektron(hyperlinkElement).closest("ul.ekSocialBar").attr("id");
				var objInstance = Ektron.SocialBar.instances[id];
				if (objInstance)
				{
					var classAttr = $ektron(hyperlinkElement).closest("li").attr("class") || "";
					var aType = classAttr.match(/ekSocialBar(\w+)/);
					if (aType != null && aType.length >= 2)
					{
						var sType = aType[1];
						$ektron.ajaxCallback(objInstance.uniqueCallbackId,
						{
							pagerequest: "RaiseEvent.SocialBarClick"
						,	item: sType
						,	languageId: objInstance.languageId
						,	objectId: objInstance.objectId
						,	objectType: objInstance.objectType
						});
					}
				}
			}
			catch (ex)
			{
				Ektron.OnException(Ektron.SocialBar, null, ex, arguments);
			}
		};
		
		this.onexception = Ektron.OnException.diagException;
		
	})() ); // Ektron.SocialBar singleton
	
	Ektron.ready(function()
	{
		$ektron("ul.ekSocialBar a").live("click", Ektron.SocialBar.onClick);
	});
}

