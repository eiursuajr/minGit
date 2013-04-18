<%@ Control Language="C#" AutoEventWireup="true" CodeFile="commonparams.ascx.cs" Inherits="Multimedia_commonparams" %>
<%@ Register Src="flashparams.ascx" TagName="flashparams" TagPrefix="uc2" %>
<%@ Register Src="quicktimeparams.ascx" TagName="quicktimeparams" TagPrefix="uc3" %>
<%@ Register Src="realplayerparams.ascx" TagName="realplayerparams" TagPrefix="uc4" %>
<%@ Register Src="windowsmediaparams.ascx" TagName="windowsmediaparams" TagPrefix="uc5" %>
<style type="text/css">
    .MediaTabNotActive {             
        position:relative;
    	padding: 2px 2px 2px 2px;
        background-color: #ADC5EF;
        border: thin solid;             
    }
    .MediaTabActive {
        position:relative;
    	padding: 2px 2px 2px 2px;
        background-color: white;
        border: thin solid;            
    }     
input {
	text-decoration:none;
}
div#Multimedia{ margin: 0 10%;background: #9BD1FA; }
b.rtop, b.rbottom{display:block;background: #FFF}
b.rtop b, b.rbottom b{
    display:block;
    height: 1px;
    overflow: hidden; 
    background: silver;
}
b.r1{margin: 0 5px}
b.r2{margin: 0 3px}
b.r3{margin: 0 2px}
b.rtop b.r4, b.rbottom b.r4{margin: 0 1px;height: 2px}     
</style>

<div id="CommonParams-panel" style="display:block">
<script type="text/javascript" language="javascript">
   
    var arrID = new Array();   
    var myTitle; 
    var selectedPlayer = "";
    function enableMediaTab( chk, tab_name ) {
        var tab = document.getElementById( tab_name );
        if (chk.checked) {
            tab.style.display = "block"; 
            selectMediaTab( tab );              
            
        } else {            
            tab.style.display = "none";
        }            
    }
    function selectMediaTab( tab )
    {          
        var i;
        selectedPlayer = "";
        for (i=0; i<arrID.length; i++)
        {
            if ( document.getElementById( arrID[i] + '-chk' ).checked)
            {
                if (arrID[i].toString().toLowerCase() == tab.id.toString().toLowerCase()) {
                    tab.className = "MediaTabActive";
                    selectedPlayer = arrID[i];
                    document.getElementById( arrID[i] + '-panel' ).style.display = "block";
                    document.getElementById( arrID[i] + '-preview').style.display = "block";                
                    eval('js' + arrID[i] + '.fill("' + myTitle + '_' + arrID[i] + '");');           
                    eval('js' + arrID[i] + '.preview();');           
                } else {
                    document.getElementById( arrID[i] ).className = "MediaTabNotActive";
                    document.getElementById( arrID[i] + '-panel' ).style.display = "none";
                    document.getElementById( arrID[i] + '-preview').innerHTML = "";
                    document.getElementById( arrID[i] + '-preview').style.display = "none";
                }
            }                   
        }
    }
    function saveMultimediaObjectsXML(action) {        
        var MultimediaXML = "";
        var iCount = 0;
        for (i=0; i<arrID.length; i++)
        {
            if (document.getElementById( arrID[i] + '-chk' ).checked) 
            {                                
                MultimediaXML += "<MediaPlayer player=\"" + arrID[i] + "\" ";
                if (document.getElementById( arrID[i] + '-chk' ).disabled) MultimediaXML += "default=\"true\"";
                MultimediaXML += ">";
                MultimediaXML += eval('js' + arrID[i] + '.createMeidaPlayerObject(action);');        
                MultimediaXML += "</MediaPlayer>";   
                iCount++;
            }
        }
        var width = document.getElementById( "media_width" ).value;
        var height = document.getElementById( "media_height" ).value;
        if (width == "") {width = "100%";}
        if (height == "") {height = "100%"}
        if (!isNaN(width.charAt(width.length - 1))) {
            width = width + "px";
        }
        if (!isNaN(height.charAt(height.length - 1))) {
            height = height + "px";
        }
        MultimediaXML = "<MediaPlayer player=\"MediaPlaceholder\"><div id=\"" + myTitle + "_Pane\" style=\"width:" + width + ";height:" + height + "\" class=\"MediaPlayerArea\"></div></MediaPlayer>" + MultimediaXML;
        document.getElementById( "content_html" ).value = "<root>" + MultimediaXML + "</root>";
       
        return true;
    }
    function showPreview() {
        eval('js' + selectedPlayer + '.preview()');
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
                    objectInstance.SetDragDropText("Drag and drop " + ext + " file that is associated with \"" + title + "\" to replace the current file.");
                }
            }      
        } 
        else {
            setTimeout('SetDMSExt("' + ext + '", "' + title + '")',100);
        }
    }
  

</script>

    <asp:Literal ID="ltInclude" runat="server"></asp:Literal>

<input type="hidden" id="media_text" name="media_text" />
    <asp:Literal ID="ltCheckboxes" runat="server"></asp:Literal>
<table id="main"  cellspacing="0" cellpadding="0" border="0">
<tr>
    <td style="width:110px" align="center" valign="top">
        <div class="MediaTabNotActive" style="display:none; vertical-align: middle; text-align: center;" id="quicktime" onclick="selectMediaTab(this)"><img title="Quicktime" alt="Quicktime" src="images/application/qt_icon.gif" width="16px" height="16px"/><br />Quicktime</div>
        <div class="MediaTabNotActive" style="display:none; vertical-align: middle; text-align: center;" id="windowsmedia" onclick="selectMediaTab(this)"><img title="Windows Media Player" alt="Windows Media Player" src="images/application/wm_icon.gif" width="16px" height="16px"/><br />Windows Media</div>
        <div class="MediaTabNotActive" style="display:none; vertical-align: middle; text-align: center;" id="realplayer" onclick="selectMediaTab(this)"><img title="Real Player" alt="Real Player" src="images/application/rp_icon.gif" width="16px" height="16px"/><br />Real Player</div>
        <div class="MediaTabNotActive" style="display:none; vertical-align: middle; text-align: center;" id="flash" onclick="selectMediaTab(this)">Flash</div>
    </td>
    <td>           
    <div class="Multimedia" style="border:thin 1px gray">    
        <!--  
        <b class="rtop"><b class="r1"></b><b class="r2"></b><b class="r3"></b><b class="r4"></b></b>	
        -->
        <fieldset><legend><strong><asp:label ID="lblMultimedia" runat="server" /> </strong></legend>
        <table cellspacing="0" cellpadding="0" border="0">
	        <tr>        	    
		        <td><asp:label ID="lblwidth" runat="server" />:<input title="Width" onchange="showPreview()" id="media_width" style="WIDTH: 64px" type="text" size="5" name="media_width" />&nbsp;<asp:label ID="lblHeight" runat="server" />:<input title="Height" onchange="showPreview()" id="media_height" style="WIDTH: 64px" type="text" size="5" name="media_height" />&nbsp;</td>
	        </tr>
	        <tr>
	            <td><asp:label ID="lblAuto" runat="server" /><input title="Auto" onchange="showPreview()" id="media_autostart" type="checkbox" name="media_autostart" />&nbsp;<asp:PlaceHolder
                    ID="PlaceHolder1" runat="server">
                    <asp:label ID="lblLoop" runat="server" /><input title="Loop" onchange="showPreview()" type="checkbox" id="media_loop" name="media_loop" /></asp:PlaceHolder></td>
	        </tr>
	        <tr>
	            <td>
	                <table>
	                    <tr>    
                            <td align="left" valign="top">
                                <asp:Panel ID="FlashPanel" Visible="false" runat="server">
                                    <uc2:flashparams ID="Flashparams1" runat="server" />
                                </asp:Panel>
                                <asp:Panel ID="QuicktimePanel" Visible="false" runat="server">
                                    <uc3:quicktimeparams ID="Quicktimeparams1" runat="server" />
                                </asp:Panel>
                                <asp:Panel ID="RealPlayerPanel" Visible="false" runat="server">
                                    <uc4:realplayerparams ID="Realplayerparams1" runat="server" />
                                </asp:Panel>
                                <asp:Panel ID="WindowsMediaPanel" Visible="false" runat="server">
                                    <uc5:windowsmediaparams ID="Windowsmediaparams1" runat="server" /> 
                                </asp:Panel>                                 
                            </td>
                        </tr>
	                </table>
	            </td>
	        </tr>
	        <tr>
	        <td>
	            <asp:Literal ID="ltResults" runat="server"></asp:Literal>
	        </td>
	        </tr>
        </table>	
        </fieldset>    
        <!--  
    	<b class="rbottom"><b class="r4"></b><b class="r3"></b><b class="r2"></b><b class="r1"></b></b>
<input type="button" id="flashthing" onclick="enableMediaTab(document.getElementById( 'flash-chk' ), 'flash')" value="test" />
    	-->
      </div>  
</td>
</tr>
</table>
</div>
	         

