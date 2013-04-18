<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ImageModification.ascx.cs" Inherits="Widgets_Dialogs_ImageModification" %>

<script type="text/javascript">
/************************************************** ***********************
This code is from Dynamic Web Coding at http://www.dyn-web.com/
See Terms of Use at http://www.dyn-web.com/bus/terms.html
regarding conditions under which you may use this code.
This notice must be retained in the code as is!
************************************************** ***********************/

function getDocHeight(doc) {
  var docHt = 0, sh, oh;
  if (doc.height) docHt = doc.height;
  else if (doc.body) {
    if (doc.body.scrollHeight) docHt = sh = doc.body.scrollHeight;
    if (doc.body.offsetHeight) docHt = oh = doc.body.offsetHeight;
    if (sh && oh) docHt = Math.max(sh, oh);
  }
  return docHt;
}

function setIframeHeight(iframeName, height, width) {
  var iframeWin = window.frames[iframeName];
  var iframeEl = document.getElementById? document.getElementById(iframeName): document.all? document.all[iframeName]: null;
  if ( iframeEl ) {
    iframeEl.style.height = "auto"; // helps resize (for some) if new doc shorter than previous
    var docHt = null;
    var docWidth = null;

    if( typeof( window.innerWidth ) == 'number' ) {
      //Non-IE
      docWidth = window.innerWidth;
      docHt = window.innerHeight;
    } else if( document.documentElement &&
      ( document.documentElement.clientWidth || document.documentElement.clientHeight ) ) {
      //IE 6+ in 'standards compliant mode'
      docWidth = document.documentElement.clientWidth;
      //myHeight = document.documentElement.clientHeight;
      docHt = getDocHeight(iframeWin.document);
    } else if( document.body && ( document.body.clientWidth || document.body.clientHeight ) ) {
      //IE 4 compatible
      docWidth = document.body.clientWidth;
      docHt = document.body.clientHeight;
    }

    // need to add to height to be sure it will all show because iframes are stupid
    if (docHt) {
      var maxdim = <asp:Literal runat="server" ID="txtMaxImageHeight" text="4000" />;
      if (height != undefined) {
        maxdim = height;
      }
      if (maxdim < 400) {
        maxdim = 400;
      }
      if (true == $ektron.browser.safari)
      {
        width = width * maxdim / height;
      }
      iframeEl.style.height = /*docHt + 30 + "px"*/ maxdim + "px";
      iframeEl.style.width = width + "px";
    }
  }
}

function loadIframe(iframeName, url) {
  if ( window.frames[iframeName] ) {
    window.frames[iframeName].location = url;
    return false;
  }
  else return true;
}
</script>
<style>
    .ektronPageInfo
    {
    	padding: 0 !important;
    }
</style>
<script type="text/javascript">
var imageDelete = true;
function saveChangeCallback()
{
    var width = null;
    var height = null;
    if (document.frames != undefined) {
      width = document.frames['imageaffect'].document.forms[0].hdnSrcFileWidth.value;
      height = document.frames['imageaffect'].document.forms[0].hdnSrcFileHeight.value
    } else {
      width = document.getElementById('imageaffect').contentDocument.forms[0].hdnSrcFileWidth.value;
      height = document.getElementById('imageaffect').contentDocument.forms[0].hdnSrcFileHeight.value;
    }
    var retValue =
    {
	    ImageURL    : document.forms[0].imagetool$hdnSrcFileURL.value
	    , Width     : width
	    , Height    : height
    };
    CloseDlg(retValue);
}
function saveChanges()
{
    imageDelete = false;
    document.body.style.cursor = "wait";

    $ektron('#divWait').modal({ 
     trigger: '',
     toTop: true,
     modal: true,
 	 overlay: 0,
     onShow: function(hash) {
         var originalWidth = hash.w.width()-5;  // -5 is to get rid of horiz scrollbar
	     hash.w.find("h4").css("width", originalWidth + "px");
	     var width = "-" + String(originalWidth / 2) + "px";
	     hash.w.css("margin-left", width);
	     hash.o.fadeTo("fast", 0.5, function() {
	 	    hash.w.fadeIn("fast");
	 });
     }, 
     onHide: function(hash) {
         hash.w.fadeOut("fast");
         hash.o.fadeOut("fast", function(){
 		         if (hash.o) 
 		 		         hash.o.remove();
         });
	}  
	});
	$ektron("#divWait").modalShow();

    $ektron.ajax({
      url: "ImageEdit.aspx?s=1",
      cache: false,
      success: function(html){
            saveChangeCallback();
            return true;
      }});
}
function cancelChanges()
{
    $ektron.ajax({
      url: "ImageEdit.aspx?c=1",
      cache: false,
      success: function(html){
            return true;
      }});
    CloseDlg();
}
function handleWindowClose()
{
    if (imageDelete) {
        cancelChanges();
    }
}
window.onbeforeunload = handleWindowClose;


function clickBrightnessBar(position)
{
    document.getElementById('ImageButton_1').src = 'images/hticktop.gif';
    document.getElementById('ImageButton_2').src = 'images/htickmark.gif';
    document.getElementById('ImageButton_3').src = 'images/htickmark.gif';
    document.getElementById('ImageButton_4').src = 'images/htickmark.gif';
    document.getElementById('ImageButton_5').src = 'images/htickmark.gif';
    document.getElementById('ImageButton_6').src = 'images/htickmajor.gif';
    document.getElementById('ImageButton_7').src = 'images/htickmark.gif';
    document.getElementById('ImageButton_8').src = 'images/htickmark.gif';
    document.getElementById('ImageButton_9').src = 'images/htickmark.gif';
    document.getElementById('ImageButton_10').src = 'images/htickmark.gif';
    document.getElementById('ImageButton_11').src = 'images/htickbottom.gif';
    document.getElementById('imageaffect').contentWindow.document.getElementById(position).click();
    document.getElementById(position).src = 'images/htickselect.gif';
}
</script>

<asp:HiddenField ID="hdnCurrentCommand" runat="server" />
<asp:HiddenField ID="hdnCallerId" runat="server" />
<asp:HiddenField ID="hdnDialogUrl" runat="server" />
<asp:HiddenField ID="hdnCallbackFunction" runat="server" />
<asp:HiddenField ID="hdnSrcFileURL" runat="server" />
<div class="ektronPageHeader" id="editContentToolbar">
    <div style="height: 13px;"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server">
        <table>
            <tr>
                <td>
                    <div style="width: 100%; height: 30px;">
                        <input type="image" ID="btnSave" alt="Save Changes"
                            runat="server" onclick="saveChanges(); return false;"
                            src="../images/UI/Icons/save.png" />
                        <asp:ImageButton ID="btnCancel"
                            runat="server" OnClientClick="cancelChanges(); return false;"
                            ImageUrl="../images/UI/Icons/cancel.png" />
                            &nbsp;&nbsp;
                 
                        <asp:ImageButton ID="btnCropImage" runat="server" OnClientClick="imageDelete=false;" onclick="btnCropImage_Click" />            
                        <asp:ImageButton ID="btnResizeImage" runat="server" OnClientClick="imageDelete=false;" onclick="btnResizeImage_Click" />            
                        <asp:ImageButton ID="btnRotateImage" runat="server" OnClientClick="imageDelete=false;" onclick="btnRotateImage_Click" />
                        <asp:ImageButton ID="btnBrightImage" runat="server" OnClientClick="imageDelete=false;"  onclick="btnBrightImage_Click" />           
                     <!--   <asp:ImageButton ID="btnTextImage" runat="server" OnClientClick="imageDelete=false;" onclick="btnTextImage_Click" />            -->
                        <asp:ImageButton ID="btnUndoImage" runat="server" OnClientClick="imageDelete=false;" onclick="btnUndoImage_Click" />           
                        <asp:ImageButton ID="btnRedoImage" runat="server" OnClientClick="imageDelete=false;" onclick="btnRedoImage_Click" />
                        
                        <!-- &nbsp;&nbsp;&nbsp;&nbsp; Current Action:-->
                        <asp:Label ID="lblCurrentAction" runat="server" Text="" />
                    </div>
                </td>
            </tr>
        </table>
    </div>  
</div>  
<div class="ektronPageContainer ektronPageInfo">
    <div id="ResizeInfoArea" class="imgcntpnl" runat="server" style="" visible="false"> 
        <table>
        <tr>
            <td>
                <span id="LabelImageSizeArea" runat="server">
                <asp:Label ID="lblImageDimensions" runat="server" Text="Resize Image:" />
                </span>
            </td>
 
            <td class="imgtbtndne">
                <asp:RegularExpressionValidator ID="vldHeight" runat="server"      
                                        ErrorMessage="> 0" 
                                        ControlToValidate="txtImageResizeHeight"     
                                        ValidationExpression="^\d+$" /><asp:HiddenField ID="hdnImageResizeOrigHeight" runat="server" />   
                <asp:RegularExpressionValidator ID="vldWidth" runat="server"     
                                        ErrorMessage="> 0" 
                                        ControlToValidate="txtImageResizeWidth"     
                                        ValidationExpression="^\d+$" /><asp:HiddenField ID="hdnImageResizeOrigWidth" runat="server" />
                <asp:Label ID="lblImageFileSize" runat="server" CssClass="imgtlinstsm"  Text="Current File Size:" /><asp:Label ID="lblImageFileSizeInKs" runat="server" Text="Label" CssClass="imgtlinstsm" /><asp:Label ID="lblImageSizeUnit" runat="server" Text="Label" CssClass="imgtlinstsm" />&nbsp;
            </td> 
       </tr>
        <tr style="background-color:#cccccc;">
            <td>
                <asp:Label ID="lblImageResizeWidth" runat="server" Text="Width:"  CssClass="imgtlinstsm" />
                <asp:TextBox ID="txtImageResizeWidth" Text="0" runat="server" Columns="3" Width="40px"  CssClass="imgtlinstsm" />
                <asp:Label ID="lblImageResizeHeight" runat="server" Text="Height:"   CssClass="imgtlinstsm" />
                <asp:TextBox ID="txtImageResizeHeight" Text="0" runat="server" Columns="3" Width="40px" CssClass="imgtlinstsm" />
                <asp:CheckBox ToolTip="Keep Aspect Ratio" ID="chkImageResizeAspect" runat="server" Checked="true" CssClass="imgtlinstsm" />
                <asp:Label ID="lblImageResizeAspect" runat="server" Text="Keep Ratio"  CssClass="imgtlinstsm" />
             </td>
            <td class="imgtbtndne" >
                <div id="ImageResizeCommand" runat="server">       
                    <asp:Button ID="btnDoImageResize" runat="server" Text="Done" OnClientClick="imageDelete=false;" onclick="btnDoImageResize_Click" />
                </div>
            </td>
        </tr>
        </table>      
    </div>
        
    <div id="CropInfoArea" class="imgcntpnl" runat="server" visible="false">
        <table><tr>
        <td>
           <span style="display:block;">
                <asp:Literal ID="litCropHelpTitle" runat="server" />
            </span>
            <span class='imgtlinstsm'>
                <asp:Literal ID="litCropHelpDescription" runat="server" />
            </span>
        </td>
        
        <td class="imgtbtndne" ><input title="Done Image Crop" type=button ID="btnDoImageCrop" runat="server" value="Done"
                    onclick="imageDelete=false; document.getElementById('imageaffect').contentWindow.EkImgToolExecSelection(); document.getElementById('imagetool_CropInfoArea').style.display='none';" /></td>
        </tr></table>
    </div>
        
    <div id="TextInfoArea" class="imgcntpnl" runat="server" visible="false">
        <table><tr>
        <td>
            <span style="display:block;">
                <asp:Literal ID="litTextHelpTitle" runat="server" />
            </span>
            <span class='imgtlinstsm'>
                <asp:Literal ID="litTextHelpDescription" runat="server" />
            </span>
        </td>
        <td class="imgtbtndne" ><input type=button ID="btnDoImageText" runat="server" value="Done"
                    onclick="imageDelete=false; document.getElementById('imageaffect').contentWindow.EkImgToolExecSelection(); document.getElementById('imagetool_TextInfoArea').style.display='none';" /></td>
        </tr></table>
    </div>
    
        
    <div id="RotateInfoArea" class="imgcntpnl" runat="server" visible="false">
        <table><tr>
        <td><asp:Label ID="lblRotateHelp" runat="server" Text="Rotate Image:" /></td>
        <td  >
            <input style="font-size:11px" type="button" title="Rotate Left" ID="btnRotateLeft" runat="server" value="<<"
                onclick="imageDelete=false; document.getElementById('imageaffect').contentWindow.document.getElementById('btnRotLeft').click();" />  
            <input id="btnRotateRight" runat="server" title="Rotate Right" onclick="imageDelete=false; document.getElementById('imageaffect').contentWindow.document.getElementById('btnRotRight').click();"
                style="font-size: 11px" type="button" value=">>" />  
        </td>
        <td class="imgtbtndne" ><input title="Done Image Rotate" type=button ID="btnFinishImageRotate" runat="server" value="Done"
                    onclick="imageDelete=false; document.getElementById('imageaffect').contentWindow.document.getElementById('RotateMenu').style.display='none'; document.getElementById('imagetool_RotateInfoArea').style.display='none';" /></td>
        </tr></table>
    </div>

    <div id="BrightnessInfoArea" class="imgcntpnl" runat="server" visible="false">
        <table><tr>
        <td><asp:Label ID="lblBrightnessHelp" runat="server" Text="Adjust Brightness:" /></td>
        <td>
            <!--img src="images/tickjumpdown.gif" border=0 /--><img
             src="images/hticktop.gif" id="ImageButton_1" onclick="clickBrightnessBar('ImageButton_1')" border=0 /><img 
             src="images/htickmark.gif" id="ImageButton_2" onclick="clickBrightnessBar('ImageButton_2')" border=0 /><img
             src="images/htickmark.gif" id="ImageButton_3" onclick="clickBrightnessBar('ImageButton_3')" border=0 /><img
             src="images/htickmark.gif" id="ImageButton_4" onclick="clickBrightnessBar('ImageButton_4')" border=0 /><img
             src="images/htickmark.gif" id="ImageButton_5" onclick="clickBrightnessBar('ImageButton_5')" border=0 /><img
             src="images/htickselect.gif" id="ImageButton_6" onclick="clickBrightnessBar('ImageButton_6')" border=0 /><img
             src="images/htickmark.gif" id="ImageButton_7" onclick="clickBrightnessBar('ImageButton_7')" border=0 /><img
             src="images/htickmark.gif" id="ImageButton_8" onclick="clickBrightnessBar('ImageButton_8')" border=0 /><img
             src="images/htickmark.gif" id="ImageButton_9" onclick="clickBrightnessBar('ImageButton_9')" border=0 /><img
             src="images/htickmark.gif" id="ImageButton_10" onclick="clickBrightnessBar('ImageButton_10')" border=0 /><img
             src="images/htickbottom.gif" id="ImageButton_11" onclick="clickBrightnessBar('ImageButton_11')" border=0 /><!--img
             src="images/tickjump.gif" border=0 /-->
        </td>
        <td class="imgtbtndne" ><input title="Done Image Brightness" type="button" id="btnFinishImageBrightness" runat="server" value="Done"
                    onclick="imageDelete=false; document.getElementById('imageaffect').contentWindow.document.getElementById('BrightnessMenu').style.display='none'; document.getElementById('imagetool_BrightnessInfoArea').style.display='none';" /></td>
        </tr></table>
    </div>

    <div id="EkImgToolEditAreaDisplay" runat="server" style="padding-top: 32px;" >    
        <%=DrawImageEditArea()%>    
    </div> 
    
    <div id="divWait" class="ektronWindow">
        <center>
        <asp:Label ID="lblSaving" runat="Server" Text="Saving..." />
        <img alt="" src="../images/application/ajax-loader_circle_lg.gif" />
        </center>
    </div>
</div>
