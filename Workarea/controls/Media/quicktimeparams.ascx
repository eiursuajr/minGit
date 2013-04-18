<%@ Control Language="C#" AutoEventWireup="true" CodeFile="quicktimeparams.ascx.cs" Inherits="Multimedia_quicktimeparams" %>
<!-- Quicktime player properties -->
<script type="text/javascript" language="JavaScript">
    var qt_cp = new ColorPicker('window');
    function pickColor( color ) { 
        document.getElementById( "qt_bgcolor" ).value = color;
        previewQTPlayer();
    }
    qt_cp.writeDiv();
</script>
<div id="quicktime-panel"  style="display:none">
<script type="text/javascript" language="JavaScript" src="controls/media/Quicktimeparams.js"></script>
<script id="quicktimeparams" type="text/javascript" language="javascript">
    var jsquicktime = new ektQuicktime();
    function previewQTPlayer() {
        jsquicktime.preview();
    }
</script>
<table width="100%" cellspacing="0" cellpadding="0" border="0">
	<tr>
	    <td nowrap="nowrap" >palindrome: <input title="palindrome" type="checkbox" onchange="previewQTPlayer()" id="qt_palindrome" name="qt_palindrome" />
        </td>	             
	    <td>Scale: 
        <select onchange="previewQTPlayer()" name="qt_scale" id="qt_scale">
            <option id="qt_scale_tofit" value="tofit" title="Scale to Exact Fit">Exact Fit</option>
            <option id="qt_scale_aspect" value="aspect" title="Scale and Maintain Aspect Ratio">Aspect</option>
        </select>
        </td>							    
	</tr>
	<tr>
	<td colspan="2" title="Background Color Setting">Background Color: 
	    <input title="Change Background Color Text" onchange="previewQTPlayer()" type="text" id="qt_bgcolor" name="bgcolor" size="20" value="" /> 
	    <a title="Select Background Color" href="#" onclick="qt_cp.select(document.forms[0].qt_bgcolor,'pick');return false;" id="pick"">Select</a></td>
	</tr>
	<tr>
	    <td colspan="2">
	    <table>
	    <tr>	    
	    <td>Controller: </td><td><input title="Quicktime Controller" onchange="previewQTPlayer()" id="qt_controller" name="qt_controller" type="checkbox" /></td>
	    <td></td><td></td>
	    </tr>
	    <tr>
	    <td>Hide:</td><td> <input title="Hide Quicktime Player" onchange="previewQTPlayer()" id="qt_hidden" name="qt_hidden" type="checkbox" /></td>
	    <td>Play every frame:</td><td> <input title="Play every frame" onchange="previewQTPlayer()" id="qt_playeveryframe" name="qt_playeveryframe" type="checkbox" /></td>
	    </tr>
	    </table>
	    </td>
	</tr>	
		<tr>
		    <td colspan="2"><asp:Literal ID="DisplayQuicktimeResults" Runat="server" />
                </td>
		</tr>							
</table>
</div> 
<!-- /Quicktime Player properties -->