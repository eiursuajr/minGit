<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DragDropCtl.aspx.cs" Inherits="Workarea_DragDropCtl" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
		    <title>DragDropCtl</title>  
     <style type="text/css" >
         body {
            font-size: 75% !important; color: #222; background: #fff; font-family: Arial, Tahoma, sans-serif;
        }
        form#dropupload{
            display:none;
        }
        /*
        .greenHoverDD:hover {background-color:#E6EFC2;border:1px solid #C6D880;color:#529214;}
        .redHoverDD:hover {background-color:#fbe3e4;border:1px solid #fbc2c4;color:#d12f19;}
        .buttonDD {display:block;margin:0;background-color:#f5f5f5;border:1px solid #dedede;border-top:1px solid #eee;border-left:1px solid #eee;font-size:100%;line-height:100%;text-decoration:none;color:#565656;cursor:pointer;padding:5px 10px 6px 7px;padding: .25em 1em .25em 2.25em; margin: 0 0 0 .75em; background-repeat: no-repeat;}
        .buttonLeftDD {float:Left;}
        .WaitLoadDD {padding: 0 auto; clear: both; margin-top: .5em;text-align:center;position:relative;margin-top:-150px;}
		*/
        #DragDropUI {position:relative;z-index:999;}
        .dmsSupportedFileTypes {font-size: .8em; margin: 0 .5em;}
        #divFileTypes {display:none;clear: both; padding: .5em; margin: 0 auto .5em; border: solid 1px #ccc;}
        #DMStabs {position: relative;}
        .ui-helper-clearfix:after { content: "."; display: block; height: 0; clear: both; visibility: hidden; }
        .ui-helper-clearfix { display: inline-block; }
        /* required comment for clearfix to work in Opera \*/
        * html .ui-helper-clearfix { height:1%; }
        .ui-helper-clearfix { display:block; }
       /*
        input.buttonUpload {background-image: url(images/ui/icons/checkIn.png); background-position: .6em center;}
        input.buttonCancel {background-image: url(images/ui/icons/cancel.png);background-position:.15em center;}
        */
        div.helpassets { position: absolute; right: 1em; top: 1em; z-index:999;}
        div.helpassets #aHelp {text-indent: -9000px; height: 16px; width: 16px; display: block; background-image:url('images/UI/Icons/help.png');
			/*add asset modal - move help button so that it actually works -- not sure why*/
			top: 120px;
			right: 12px;
		}
        /* end clearfix */
      </style>
		<script type="text/javascript">
		<!--//--><![CDATA[//><!--
		////////////////////// Properties //////////////////////
		// Properties are set from ViewFolder.ascx
		////////////////////////////////////////////////////////
		var folderID = 0;
		var contLang = -1;
		var publishHtml = 0;
		var contentID = 0;
		var folderPath = "content/";
		
		var mode_set = '<asp:literal id="jsModeSet" runat="server"/>';
		var mode = '<asp:literal id="jsMode" runat="server"/>';
		var mode_id = '<asp:literal id="jsModeId" runat="server"/>';
        		
		function FolderPath(path)
		{
		    if (path != null)
			{
				folderPath = path;
			}	
		}
		
		function FolderID(id)
		{
			if (id != null)
			{
				folderID = id;
			}			
		}
		
		function SetFolderPath(path)
		{
		    var divDAV = document.getElementById("divDAV_dropuploader");
		    var iframeDAV = document.getElementById("iframeDAV_dropuploader");
		    if(divDAV)
		    {
		        iframeDAV_dropuploader.location.href = "about:blank";
		        divDAV.navigateFrame("http://localhost/webdav/hi/", "iframeDAV_dropuploader");
		    }
		}

		var m_Url = '<asp:literal id="jsMetaUrl" runat="server"/>';
		function uploadClick()
		{
		    var uploadCheck  = document.getElementById("isFileUploadComplete");
		    if(uploadCheck != null)
		    {
		        if(uploadCheck.value == "true")
		        {
		            if(m_Url != "" && "undefined" != typeof(self.parent.ektb_show))
                    	    {
                        	if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) >= 9)
                        	{
                            		self.parent.ektb_show("Add asset data", "about:blank");
                            		setTimeout( function() 
                            		{
                            	   	 ReSizeContainer("Add asset data");	// iframe manipulation happened in here.
                            		}, 100);
                        	}
                        	else
                        	{
		                    self.parent.ektb_show("Add asset data", m_Url);
                        	}
                    	}
		        else           
                    	{
		                closeDialog();
                    	}
		    }
		    else
               	    {
		            setTimeout("uploadClick();",  500);
                    }
		    }
		}
		
		function showLoadingBar()
		{
		   var waitImg  = document.getElementById("ek_DmsFileUploadWait");
		   var tContent =  document.getElementById("DragDropUI") ;
		   if(tContent != null)
		    {
		            tContent.style.position="relative";
		            tContent.style.left="-10000px";
                    
		    }
		   if(waitImg != null)
		    {
		            waitImg.style.position =  'relative';
		            waitImg.style.top = '-150px';
                    waitImg.style.left = '0px';
		    }
		}
		
		function CheckFileExists()
		{
          if ($ektron("#assetcontrolupdate").val() == "update" && !(CheckFileName()))
              return false;
          if ($ektron("#forceExtension").val() == "true" && !(CheckFileExtension()))
              return false;	  
		  var folder_id = '<asp:literal id="jsFolderID" runat="server"/>';
		  var lang_id = '<asp:literal id="jsLanguageID" runat="server"/>';
		  var tax_id = '<asp:literal id="jsCheckFileTaxId" runat="server"/>';
		  PageMethods.CheckFileExists(document.getElementById('ekFileUpload').value,folder_id,lang_id,tax_id,FileExistsCallBack);
		  return false;
		}

		function CheckFileExtension()
		{
		    return CheckFile("extension");
		}
		function CheckFileName()
		{
		    return CheckFile("name");
        }

        function CheckFile(action)
        {
            var oldfilename = document.getElementById('oldfilename');
            if ($ektron("#oldfilename").length > 0 && $ektron("#ekFileUpload").length > 0 && $ektron("#ekFileUpload").val().length > 0)
            {
                var uploadfilename = '';
                if (navigator.userAgent.toLowerCase().indexOf("firefox") > -1)
                    uploadfilename = $ektron("#ekFileUpload").val();
                else
                {
                    var justfilename = document.getElementById('ekFileUpload').value.match(/(.*)[\/\\]([^\/\\]+\.\w+)$/);
                    if (justfilename != null && justfilename.length > 2 && justfilename[2] != null && justfilename[2].length > 0)
                        uploadfilename = justfilename[2];
                }

                if (action == "name")
                {
                    if (oldfilename.value.toLowerCase() != uploadfilename.toLowerCase())
                    {
                        var msg = '<asp:literal id="jsUpdateErrorMsg" runat="server"/>';
                        alert(msg + oldfilename.value);
                        return false;
                    }
                }
                else if (action == "extension")
                {
                    if (/[^.]+$/.exec(oldfilename.value.toLowerCase())[0] != /[^.]+$/.exec(uploadfilename.toLowerCase())[0])
                    {
                        var extMsg = '<asp:literal id="jsExtensionErrorMsg" runat="server"/>';
                        alert(extMsg + /[^.]+$/.exec(oldfilename.value.toLowerCase())[0]);
                        return false;
                    }
                }
            }
            return true;
        }
		
		function toggleSupportedFileTypes()
		{
		   $ektron("#divFileTypes").toggle("slow");
		   var toggleText = $ektron(".toggleSupportedFileTypes").text();
		   var hideFTypes = '<asp:literal id="jsHideFTypes" runat="server"/>';
		   var showFTypes = '<asp:literal id="jsShowFTypes" runat="server"/>';
		   if (toggleText == showFTypes) {
		       
		        $ektron(".toggleSupportedFileTypes").text(hideFTypes);
		        $ektron(".toggleSupportedFileTypes").attr("title", hideFTypes)
		    }
		    else {
		        
		        $ektron(".toggleSupportedFileTypes").text(showFTypes);
		        $ektron(".toggleSupportedFileTypes").attr("title", showFTypes)
		    }
		}
		
		function FileExistsCallBack(result)
        {
             if(result != "") 
             {
               if(result == "O")
                   {
                          alert('<asp:literal id="jsCheckedOutMsg" runat="server"/>'); 
                          return false;
                   }
               else
                   {
                        if(confirm('<asp:literal id="jsFileOverwriteMsg" runat="server"/>'))
                            {
                               showLoadingBar();
                               __doPostBack('uploadFile','');
                            }
                        else
                               return false;
                   }
                     
             }
            else
            {
                showLoadingBar();
               __doPostBack('uploadFile','');
            }
         }
		
	$ektron(document).ready(function() {
	    var uploadCheck  = document.getElementById("isFileUploadComplete");
	    if (!(ShowMultipleUpload() && CheckSTSUpload()))
        {
           if(document.getElementById("liMultipleDMS")!= null)
              document.getElementById("liMultipleDMS").style.display ="none";
           if(document.getElementById("tabMultipleDMS")!= null)  
              document.getElementById("tabMultipleDMS").style.display ="none";
        }
      
        if(uploadCheck !== null)
        {
		    if(uploadCheck.value == "invalid")
	        {
		       var ddTabs = $ektron("#DMStabs").tabs({
                    select: function(event, ui)
                    {
                            ReSizeContainer(ui);   
                    }
                });
		       ddTabs.tabs('select', 0);
		    }
            else
            {
               $ektron("#DMStabs").tabs({
                    select: function(event, ui)
                    {
                            ReSizeContainer(ui);   
                    }
                });
            }
	    }
	    else
	    {
           $ektron("#DMStabs").tabs({
                select: function(event, ui)
                {
                        ReSizeContainer(ui);   
                }
            });
	    }
        ReSizeContainer("");
	    $ektron("form#dropupload").show();
	    if ($ektron(".confirmationURL").length > 0 && $ektron("#NextUsing").length > 0)
	        $ektron(".confirmationURL").attr("value", $ektron("#NextUsing").val());
	    
	});
	
	    function ReSizeContainer(ui)
	    {
            if (self.parent !== null)
            {
                var iframeParent = $ektron(self.parent.document);
                var iframes = iframeParent.find("iframe");
                var dragDropIframeIsThickbox = false;
                var dragDropIframe, iframeParentWrapper;
                iframes.each(function(i)
                {
                      try
                       {
                        var contents = $ektron(this).contents();
                        var hasMyDragDrop = (contents.find("div#DMStabs a.tabFileUpload").length > 0) ? true : false;
                        if (hasMyDragDrop)
                        {
							$('#ekFileUpload').val('');
                           dragDropIframe = iframes.eq(i);
                           iframeParentWrapper = dragDropIframe.parent();
                        }
                       }
                       catch(e)
                       {
                       
                       }
                });
                try
                {
                    if (dragDropIframe.is("#EkTB_iframeContent"))
                    {
                        dragDropIframeIsThickbox = true;
                    }
                }
                catch( ex ) { 
	            }
                // set new height and width
                var newHeight, newWidth, tabClicked, tabText;
                if ("string" == typeof ui)
                {
                    tabClicked = null;  // ie9, the tabs is not loaded yet.
                    tabText = ui;
                }
                else
                {
                    tabClicked = $ektron(ui.tab);
                    tabText = tabClicked.text();
                }
                switch (tabText)
                {
                    case "<asp:literal id="jsMultipleDMS" runat="server"/>":
                        newHeight = "500";
                        newWidth = "700";
                        newIframeHeight = "550";
                        break;
                    default:
                        // file upload
                        newHeight = "200";
                        newWidth = "525";
                        newIframeHeight = "180";
                        break;
                }
                 newHeight = "500";
                        newWidth = "700";
                        newIframeHeight = "550";
                // raise an event that indicates that a DMS tab was clicked
                if (self.parent.$ektron)
                {
                    self.parent.$ektron(self.parent.document).trigger({
                        type: "ektronDmsTabClicked",
                        tab: tabClicked,
                        iframe: dragDropIframe
                    });
                }
                if(dragDropIframe!=null)
                    dragDropIframe.css({
                        "width": "100%",
                        "height": newIframeHeight
                    });
                if (dragDropIframeIsThickbox)
                {
                    iframeParentWrapper.css({
                        "width": newWidth + "px",
                        "height": newHeight + "px",
                        "margin-left": -(parseInt(newWidth / 2, 10)) + "px",
                        "margin-top": -(parseInt(newHeight / 2, 10)) + "px"
                    });
                    if(m_Url != "" && $ektron.browser.msie && parseInt($ektron.browser.version, 10) >= 9)
                    {
                        dragDropIframe.attr("src", m_Url);
                        iframeParent.find("#EkTB_load").css("display", "none");
                    }
                } 
            }
    	 }
	    
	    function cancelDialog()
		{
		  self.parent.ektb_remove();      
		}
	
        function closeDialog()
		{

			 if(top.frames["ek_main"] != null) // In workarea, Can just close the thickbox
			      self.parent.ektb_remove(); 		
			 else if(top.frames["mainFrame"] != null) //An Iframe
			      ReloadFramePage();
			 else
		          ReloadParentPage();  
		          
		     var tDDContentTabs =  document.getElementById("DragDropUI") ;
		   	 if(tDDContentTabs != null)
		    	 {
		          	tDDContentTabs.style.position="relative";
		           	tDDContentTabs.style.left="-10000px";
                 }
		}		
		function ReloadFramePage()
		{
                //Not in workarea and an Iframe
                var buffer = '';
	            try {
	                buffer = new String( top.frames["mainFrame"].location.href );
	            }
	            catch( ex ) {
	            }
                if (buffer.indexOf("#") != -1)
	            {
                    //Found a # sign
		            var sUrl = top.frames["mainFrame"].location.pathname;
		            var taxonomyId = "";
			        taxonomyId = '<asp:literal id="jsTaxonomyIdReloadFrame" runat="server"/>';
			       
		            if(taxonomyId != "")
		            {
		                var tempBuffer = new String( top.frames["mainFrame"].location.pathname );
		                if (tempBuffer.indexOf("__taxonomyid=") > -1)
		                {
		                    var startindex = tempBuffer.indexOf("__taxonomyid=");
		                    var endindex = tempBuffer.indexOf("&", startindex);

		                    if (endindex == -1)
		                    {
		                        endindex = tempBuffer.length;
		                        startindex--;
		                    }
		                    else
		                        endindex++;

		                    var replaceTerm = tempBuffer.substring(startindex, endindex);
		                    tempBuffer = tempBuffer.replace(replaceTerm, "");
		                }

		                if (tempBuffer.indexOf("?") > -1)
			                sUrl = tempBuffer + "&__taxonomyid=" +taxonomyId;
			            else
			                sUrl = tempBuffer + "?__taxonomyid=" + taxonomyId;
			        }

    	            var num = GetImgReloadNumber(sUrl);
    	            sUrl = CleanExisitingImgReloadNum(sUrl);
                    top.frames["mainFrame"].location.href = sUrl + AddReloadImageFlag(sUrl, num);                
	            }
	            else
	            {
	                var num = GetImgReloadNumber(top.frames["mainFrame"].location.href);
	    	        var newUrl = CleanExisitingImgReloadNum(top.frames["mainFrame"].location.href);
                    top.frames["mainFrame"].location.href = newUrl + AddReloadImageFlag(newUrl, num);
                }
		}

        function ReloadParentPage()
		{
                var buffer = '';
     	        var objDoc = null;
	            try {
	                        if (top != null && top.opener != null)
	                        {
	                            buffer = new String( top.opener.location );
	                            objDoc = top.opener;
	                        }
	                        else if(self != null && self.parent != null)
	                        {
	                            buffer = new String( self.parent.location );
	                            objDoc = self.parent;
	                        }
                	        
	                }
	            catch( ex ) {
		         
	            }
	            
	            var taxonomyId = '<asp:literal id="jsTaxonomyId" runat="server"/>';
	            var sUrl ;
	            
	            if (buffer.indexOf("#") != -1)                         
	                 {
	                            sUrl = objDoc.location.pathname;
	                            if(objDoc.location.search != "")
	                              sUrl = sUrl + objDoc.location.search;
	                              
		                        if(taxonomyId != "")
		                             {
		                                    var tempBuffer = new String( objDoc.parent.location.pathname );
		                                    if(objDoc.parent.location.search != "")
		                                    tempBuffer = tempBuffer + objDoc.parent.location.search ; 
		                                    if (tempBuffer.indexOf("__taxonomyid=") > -1)
		                                           {

	                                                       var startindex = tempBuffer.indexOf("__taxonomyid=");
		                                                    var endindex = tempBuffer.indexOf("&", startindex);

		                                                    if (endindex == -1)
		                                                    {
		                                                        endindex = tempBuffer.length;
		                                                        startindex--;
		                                                    }
		                                                    else
		                                                        endindex++;

		                                                    var replaceTerm = tempBuffer.substring(startindex, endindex);
		                                                    tempBuffer = tempBuffer.replace(replaceTerm, "");
		                                           }

		                                    if (tempBuffer.indexOf("?") > -1)
			                                    sUrl = tempBuffer + "&__taxonomyid=" +taxonomyId;
			                                else
			                                    sUrl = tempBuffer + "?__taxonomyid=" + taxonomyId;
			                        }
			                    else
			                        {
			                                  if(sUrl.indexOf("__taxonomyid=") > -1)
			                                  {
			                                        //No TaxID Remove Existing;
			                                        var startindex = sUrl.indexOf("__taxonomyid=");
		                                            var endindex = sUrl.indexOf("&", startindex);

		                                            if (endindex == -1)
		                                            {
		                                                endindex = sUrl.length;
		                                                startindex--;
		                                            }
		                                            else
		                                                endindex++;

		                                            var replaceTerm = sUrl.substring(startindex, endindex);
		                                            sUrl = sUrl.replace(replaceTerm, "");
			                                  }
			                        }

	    	                        if (self.parent != null)
	    	                         {
	                                    
	                                    var num = GetImgReloadNumber(sUrl);
	    	                            sUrl = CleanExisitingImgReloadNum(sUrl);
                                        objDoc.location.href = sUrl + AddReloadImageFlag(sUrl, num);
                                     }
                                      else
                                     {
	                                  
	                                   var num = GetImgReloadNumber(sUrl);
	    	                           sUrl = CleanExisitingImgReloadNum(sUrl);
                                       objDoc.location.href = sUrl + AddReloadImageFlag(sUrl, num);
                                    }
	                 } //end   if (buffer.indexOf("#") != -1)
	            else
	            {
	                                //No #
	                                var sUrl1="";
	                                if (taxonomyId != "")
                                    {
		                                sUrl1 = sUrl1 + "?__taxonomyid=" + taxonomyId;
		                            }
		                          
		                            if(top != null && top.location != null && top.location.href != "")
		                            {
		                                    var tempBuffer = new String( top.location.href );
			                                var poundsign = tempBuffer.indexOf("#");
			                                var newUrl = tempBuffer;
                                            if (poundsign > -1)
                                            {
                                                newUrl = tempBuffer.substring(0,poundsign);
                                                var num = GetImgReloadNumber(newUrl);
	    	                                    newUrl = CleanExisitingImgReloadNum(newUrl);
                                                newUrl = newUrl + AddReloadImageFlag(newUrl, num);
                                                newUrl=newUrl.replace(/__taxonomyid=[0-9]+/ig,'__taxonomyid='+taxonomyId);
                                               
                                            }
			                                else
			                                {
                                                var num = GetImgReloadNumber(newUrl);
	    	                                    newUrl = CleanExisitingImgReloadNum(newUrl);
                                                var needReloadFlag=true;
                                                if(newUrl.endsWith("/"))
                                                {
                                                    needReloadFlag=false;
                                                }
                                                if(needReloadFlag)
			                                        newUrl = newUrl + AddReloadImageFlag(newUrl, num);
			                                }
            			                   
                                           top.location.href = newUrl;
                                           if(newUrl.indexOf("ekimgreload=1") > 0)
                                            {
					                          top.location.reload(true);
                                            }
                                    }
		                            else if (parent != null)
	    	                         {
	    	                              if (sUrl1 != "")
	    	                                {
		                                            //sUrl1 not 0
	    		                                    sUrl1 = parent.location.href;
	    		                                    var tempBuffer = new String( parent.location.href );
                                                    if (tempBuffer.indexOf("__taxonomyid=") > -1)
                                                        {
		                                                        //__taxonomyid exists
                                                                var startindex = tempBuffer.indexOf("__taxonomyid=");
                                                                var endindex = tempBuffer.indexOf("&", startindex);

                                                                if (endindex == -1)
                                                                   {
                                                                       endindex = tempBuffer.length;
                                                                        startindex--;
		                                                           }
                                                                else
		                                                            endindex++;

                                                                var replaceTerm = tempBuffer.substring(startindex, endindex);
                                                                tempBuffer = tempBuffer.replace(replaceTerm, "");
                                                        }

                                                   if (tempBuffer.indexOf("?") > -1)
                                                        {
                                                                var poundsign = tempBuffer.indexOf("#");
                                                                if (poundsign > -1)
                                                                {
			                                                        if(tempBuffer.substring(0,poundsign).lastIndexOf("&") == tempBuffer.substring(0,poundsign).length)
			                                                        {
                           		                                        sUrl1 = tempBuffer.substring(0,poundsign) + "__taxonomyid=" +taxonomyId;
			                                                        }
			                                                        else
			                                                        {
                            	                                        sUrl1 = tempBuffer.substring(0,poundsign) + "&__taxonomyid=" +taxonomyId;
			                                                        }
			                                                        var num = GetImgReloadNumber(sUrl1);
	    	                                                        sUrl1 = CleanExisitingImgReloadNum(sUrl1);
                                                                    sUrl1 = sUrl1 + AddReloadImageFlag(sUrl1, num) + tempBuffer.substring(poundsign);
                                                                }
                                                                else
                                                                {
			                                                        if(tempBuffer.lastIndexOf("&") == tempBuffer.length)
			                                                        {
                                                                        sUrl1 = tempBuffer + "__taxonomyid=" +taxonomyId;
			                                                        }
			                                                        else
			                                                        {
                            	                                        sUrl1 = tempBuffer + "&__taxonomyid=" +taxonomyId;
			                                                        }
			                                                        var num = GetImgReloadNumber(sUrl1);
	    	                                                        sUrl1 = CleanExisitingImgReloadNum(sUrl1);
	                                                                sUrl1 = sUrl1 + AddReloadImageFlag(sUrl1, num);
	                                                            }
	                                                  } //End if (tempBuffer.indexOf("?") > -1)
	                                             else
	                                                  {
	                                                                sUrl1 = tempBuffer + "?__taxonomyid=" + taxonomyId;
	                                                                var num = GetImgReloadNumber(sUrl1);
	    	                                                        sUrl1 = CleanExisitingImgReloadNum(sUrl1);
	                                                                sUrl1 = sUrl1 + AddReloadImageFlag(sUrl1, num);
                                                             
	                                                  }
                                                var querystring = sUrl1.substring(sUrl1.indexOf("?"), sUrl1.length);
                                                parent.location.href = parent.location.pathname + querystring;
		                                 } //End  if (sUrl1 != "")
		                         else
		                                {
	                                        
			                                var num = GetImgReloadNumber(parent.location.pathname + parent.location.search);
	    	                                var newUrl = CleanExisitingImgReloadNum(parent.location.pathname + parent.location.search);
			                                parent.location.href = newUrl + AddReloadImageFlag(parent.location.pathname + parent.location.search, num);
			                            }
		                     }
		              else
		                    {
                                var tempBuffer = new String( top.location.href );
			                    var poundsign = tempBuffer.indexOf("#");
			                    var newUrl = tempBuffer;
                                if (poundsign > -1)
                                {
                                    newUrl = tempBuffer.substring(0,poundsign);
                                    var num = GetImgReloadNumber(newUrl);
	    	                        newUrl = CleanExisitingImgReloadNum(newUrl);
                                    newUrl = newUrl + AddReloadImageFlag(newUrl, num);
                                    newUrl=newUrl.replace(/__taxonomyid=[0-9]+/ig,'__taxonomyid='+taxonomyId);
                                   
                                }
			                    else
			                    {
                                    var num = GetImgReloadNumber(newUrl);
	    	                        newUrl = CleanExisitingImgReloadNum(newUrl);
			                        newUrl = newUrl + AddReloadImageFlag(newUrl, num);
			                    }
			                   
                                top.location.href = newUrl;
                            }
		        }
		}

		function AddReloadImageFlag(path, num)
		{
                if(num != 1)
                    num = 1;
                else
                    num = 2;

                var result = "";
                var delim = (path.indexOf("?") >= 0) ? "&" : "?";
                var parmName = "ekimgreload";
                if (path.indexOf(parmName) < 0){
                    result += delim + parmName + "=" + num;
                }
                return (result);
         }
		
		function GetImgReloadNumber(url)
		{
            if(url.indexOf("ekimgreload=2") > 0)
    	        return 2;
            else
                return 1;
		}
		function CleanExisitingImgReloadNum(url)
        {
            var newUrl = new String( url );
            // if it is just one parameter in a string of them, remove it
            newUrl = newUrl.replace("&ekimgreload=1", "");
            newUrl = newUrl.replace("&ekimgreload=2", "");

            // if it is the first parameter in a string with others, make sure to switch the next param's '&' with a '?'
            newUrl = newUrl.replace("?ekimgreload=1&", "?");
            newUrl = newUrl.replace("?ekimgreload=2&", "?");

            // if it is the only param, just remove it
            newUrl = newUrl.replace("?ekimgreload=1", "");
            newUrl = newUrl.replace("?ekimgreload=2", "");
            return newUrl;
        }
		
		function MultipleDocumentUpload(s)
        {   //s=1 2003 mode
            //s=0 2010 mode

            var folderRequireManualAlias = '<asp:literal id="jsfolderRequireManualAlias" runat="server"/>';
	        var ManualAliasAlert='<asp:literal id="jsManualAliasAlert" runat="server"/>' ;

            var hid_taxrequied=document.getElementById('requireMetaTaxonomy');
            var btnUpload=document.getElementById('btnMUpload');
            var td_process=document.getElementById('td_lbtn_processTaxMeta');
            var tbl_upload_message=document.getElementById('tbl_upload_message');


            if(folderRequireManualAlias=='true')
                alert(ManualAliasAlert);

            if(hid_taxrequied.value=='True' &&s ==0)// TaxMeata are is required AND 2010 upload
            {
                btnUpload.style.display='none';
                td_lbtn_processTaxMeta.style.display='block';
            }

            if(hid_taxrequied.value=='False' &&s ==0)
            {
                btnUpload.value='Done';
                btnUpload.setAttribute('disabled','disabled');
                tbl_upload_message.style.display='block';
            }

            if(document.getElementById("idUploadCtl") != null)
            {
                document.getElementById("idUploadCtl").MultipleUpload();
            }
            if(s!=0)
            {
              //2003/2007 upload require the form posted to processupload.aspx, need to requrn true to get the form posted.

              document.forms["dropupload"].action="ProcessUpload.aspx";
          	  document.forms["dropupload"].submit();
              return false;
            }
            else
                return false;
        }
                
		function ContentID(id)
		{
		    if( id != null )
		    {
		        contentID = id;
		    }
		}
		function ContentLanguage(id)
		{
			if (id != null)
			{
				contLang = id;
			}
		}
		function PublishHtml(id)
		{
			if (id != null)
			{
				publishHtml = id;
			}
		}
		////////////////////// /Properties //////////////////////
		function Startup() {
			if (typeof FolderID != "function")
			{
				//setTimeout("Startup()", 10);
			}
		}
		
		function ConditionalSet()
		{	
			if( mode_set ) {
			    if( mode == "0" ) {
			        FolderID(mode_id);
			    }
			    else if( mode == "1" ) {
			        ContentID(mode_id);
			    }
			}
		}
		
		function SetDMSExt(ext, title) {
            if ((typeof EktAsset == 'object') && (EktAsset.instances[0].isReady()))
            {    
                var objectInstance = EktAsset.instances[0]
                if(objectInstance)
                {
                    objectInstance = objectInstance.editor;
                    if(objectInstance)
                    {
                        objectInstance.FileTypes = "'*." + ext + "'";
                        objectInstance.SetDragDropText(title);
                    }
                }      
            } 
            else {
                //setTimeout('SetDMSExt("' + ext + '", "' + title + '")',100); ektronWindow
            }
        }
		ConditionalSet();
		//--><!]]>
        </script>
    </head>     
<body>
    <form id="dropupload" runat="server">
    
         <div class="ektronModalBody" runat="server" id="DragDropUI">
                 <div id="DMStabs" >
                        <div class="helpassets">
                    	<div class="ektronToolbar">
                        <a id="aHelp" runat="server" class="help" title="Click here to get help">
                        	<img src="images/UI/Icons/help.png" />
                        </a>
                         </div>
					</div>

	                    <ul>
                            <li><a class="tabFileUpload" href="#tabFileUpload"><asp:Literal ID="ltrFload" runat="server" ></asp:Literal></a></li>
                            <li id="liMultipleDMS" runat="server"><a href="#tabMultipleDMS"><asp:Literal ID="ltrMDMS" runat="server" ></asp:Literal></a></li>
	                        <li id="liDragDrop" visible="false"  runat="server"><a href="#tabDragDrop"><asp:Literal ID="ltrDD" runat="server" ></asp:Literal></a></li>
                        </ul>
                        
                           <div id="tabFileUpload">
		                   <div id="ek_DMSUploadUI" runat="server" >
		                            <asp:ScriptManager runat="server"  ID="ScriptManager1" EnablePageMethods="true" />
                                    <asp:Literal ID="ltrStatus" runat="server" /><br />
                                    <asp:FileUpload size="50"  ID="ekFileUpload" Width="80%" runat="server" /><br /><br />
                                    <asp:Button CssClass="greenHoverDD button  buttonLeftDD buttonUpload" ID="uploadFile" runat="server" OnClientClick="return CheckFileExists();" OnClick="uploadFile_Click" /> 
                                    <asp:Button CssClass="redHoverDD button  buttonLeftDD buttonCancel" ID="btnCancel" runat="server"  OnClientClick="cancelDialog();" />                                                  
                                 </div>
	                    </div>
	                     <div runat="server" id="tabMultipleDMS">
	                        <div runat="server" id="pnl_DMSMultiUpload" visible="false" class="ui-helper-clearfix" style="margin-bottom: .5em">
								<asp:Literal ID="ltrNoUpload" runat="server" ></asp:Literal>
                                <asp:Button runat="server" ID="btnMUpload"  Text="Upload"  CssClass="greenHoverDD button  buttonLeftDD buttonUpload" value="Upload" PostBackUrl="ProcessUpload.aspx"   />
                                <a class="toggleSupportedFileTypes" style="float:right; text-decoration:underline;" href="#" onclick="toggleSupportedFileTypes();"><asp:Literal ID="ltrShowFTypes" runat="server" ></asp:Literal></a>
	                        </div>
                            <div id="pnl_OfficeVerSelector" runat="server" visible="true" class="ui-helper-clearfix" style="margin-bottom: .5em">
                                <asp:Literal runat="server" ID="lit_VerionSelect"></asp:Literal><br />
                                <asp:RadioButtonList runat="server" ID="rbl_OfficeVersion">
                                    <asp:ListItem Text="" Value="2010" />
                                    <asp:ListItem Text="" Value="other" />
                                </asp:RadioButtonList><br />
                                <asp:Button runat="server" ID="btn_VersionSelect" Text="OK" OnClick="btn_VersionSelect_Click" />

                            </div>
                            <div id="pnl_versionToggle" runat="server">
                            <table id="tbl_upload_message"  style="display:none;">
                                <tr>
                                    <td><span style="color:Red">
                                        <asp:Label runat="server" ID="lbl_upload_message" />
                                    </span></td>
                                </tr>
                            </table>
                            <table width="100%">
                                <tr>
                                    <td id="td_lbtn_processTaxMeta" style="display:none;">
                                    <asp:Label runat="server" ID="lbl_processTacMeta" ForeColor="Red"></asp:Label>
                                        <asp:Button runat="server" ID="btn_processTaxMeta" OnClick="lbtn_processTaxMeta_Click" />
                                    </td>
                                    <td style="text-align:right; width:25%"><asp:LinkButton runat="server" ID="lbtn_toggleVersion"  OnClick="lbtn_toggleVersion_Click" /></td>
                                </tr>
                            </table>
                                
                            </div>
	                    </div>
	                    
	                     <div runat="server" visible="false" id="tabDragDrop">
		                    
	                    </div>
                </div>
        </div> 
        <div runat="server" class="WaitLoadDD" id="ek_DmsFileUploadWait" style="position:relative;left:-10000px" >
         <asp:Literal ID="ltrPleaseWait" runat="server" ></asp:Literal> <br />
         <img src="images/application/loading_bar.gif" alt="uploading image" />
        </div>
        <input id="isFileUploadComplete" runat="server" type="hidden" value="" />     
        <input type="hidden" name="Cmd" value="Save" />
        <input type="hidden" name="NextUsing" id="NextUsing" runat ="server" />
        <input type="hidden" value="New" />
        <input type="hidden" name="putopts" id="putopts" value="true" runat="server"/>
        <input type="hidden" name="destination" id="destination"  runat ="server" />
        <input type="hidden" name="Confirmation-URL" class="confirmationURL" />
        <input type="hidden" name="PostURL"  id="PostURL"  runat ="server" />
        <input type="hidden" name="VTI-GROUP" value="0" />
        <input type="hidden" name="type" value="multiple,add" id="type" runat="server" />
        <input type="hidden" name="content_id" id="content_id" runat="server" />
        <input type="hidden" name="content_folder" id="content_folder" runat="server" />
        <input type="hidden" name="content_language" id="content_language" runat="server" />
        <input type="hidden" name="content_teaser" id="content_teaser" runat="server" />
        <input type="hidden" name="requireMetaTaxonomy" id="requireMetaTaxonomy" runat="server" />
        <input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value="" runat="server" />
        <input type="hidden" name="isImage" id="isImage" value="" runat="server" />
        <input type="hidden" name="IsSearchable" id="IsSearchable" value="IsSearchable" runat="server" />
        <input type="hidden" name="oldfilename" id="oldfilename" runat="server" />
        <input type="hidden" name="assetcontrolupdate" id="assetcontrolupdate" runat="server" />
        <input type="hidden" name="forceExtension" id="forceExtension" runat="server" />
    </form>
</body>
</html>
