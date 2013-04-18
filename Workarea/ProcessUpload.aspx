<%@ Page Language="C#" AutoEventWireup="true" enableViewStateMac ="false" Inherits="ProcessUpload" CodeFile="ProcessUpload.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
	    <title>ProcessUpload</title>
    <script type="text/javascript">
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
                                                var needReloadFlag = true;
                                                if (newUrl.lastIndexOf("/", newUrl.length - 1) != -1) {
                                                    needReloadFlag = false;
                                                }
                                                if (needReloadFlag)
	                                                newUrl = newUrl + AddReloadImageFlag(newUrl, num);
	                                        }
            			                   
                                           top.location.href = newUrl;
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
         </script>
         <asp:PlaceHolder id="uxAlertInvalidFileType" runat="server" visible="false">
            <script type="text/javascript">
               var invalidFiles = '<asp:literal id="jsInvalidFiles" runat="server"/>' 
                alert(invalidFiles);
            </script>
        </asp:PlaceHolder>
         <asp:PlaceHolder id="uxCloseThickBox" runat="server" visible="false">
            <script type="text/javascript">
             if(top.frames["ek_main"] != null) // In workarea, Can just close the thickbox
			      self.parent.ektb_remove(); 		
			 else if(top.frames["mainFrame"] != null) //An Iframe
			      ReloadFramePage();
			 else
		          ReloadParentPage();
            </script>
        </asp:PlaceHolder>
    </head>
    <body>
    </body>
</html>