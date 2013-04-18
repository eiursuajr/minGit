<%@ Control Language="C#" AutoEventWireup="true" CodeFile="flashparams.ascx.cs" Inherits="Multimedia_flashparams" %>
<!-- Quicktime player properties -->
<script type="text/javascript" language="JavaScript">
    var fl_cp = new ColorPicker('window');
    function pickColor( color ) { 
        document.getElementById( "fl_bgcolor" ).value = color;
        previewFlash();
    }
    fl_cp.writeDiv();
</script>

<div id="flash-panel"  style="display:none">
<script type="text/javascript" language="JavaScript" src="controls/media/flashparams.js">
</script>
<script id="flashparams" type="text/javascript" language="javascript">
    var jsflash = new ektFlashPlayer();
    function previewFlash() {
        jsflash.preview();
    }
</script>
<table cellspacing="0" cellpadding="0" border="0">
    <tr>
       <td nowrap align="left" title="Menu">Menu:</td>
       <td nowrap align="left"><input onclick="previewFlash();" id="menu" title="Menu" type="checkbox" /></td>
        
       <td nowrap align="left" title="Background Color">BGColor:</td>
       <td align="left"><input title="Change Background Color" type="text" onchange="previewFlash();" id="fl_bgcolor" name="bgcolor" size="20" value="" readonly="readOnly" /> <a title="Select Background Color" href="#" onclick="fl_cp.select(document.forms[0].fl_bgcolor,'fl_pick');return false;" name="fl_pick" id="fl_pick">Select</a></td>

       </tr>
       <tr>
       <td nowrap align="left" title="Alignment Options">Align:</td>
       <td nowrap align="left"><select onchange="previewFlash();" id="align">
       <option id="align_l" value="l" title="Align Left">Left</option>
       <option id="align_t" value="t" title="Align Top">Top</option>
       <option id="align_r" value="r" title="Align Right">Right</option>
       <option id="align_b" value="b" title="Align Bottom">Bottom</option>
       </select></td>
       <td nowrap align="left" title="Alignment Options">SAlign:</td>
       <td nowrap align="left"><select onchange="previewFlash();" id="salign">
       <option id="salign_l" value="l" title="Align to the Left Edge">Left Edge</option>
       <option id="salign_t" value="t" title="Align to the Top Edge">Top Edge</option>
       <option id="salign_r" value="r" title="Align to the Right Edge">Right Edge</option>
       <option id="salign_b" value="b" title="Align to the Bottom Edge">Bottom Edge</option>
       <option id="salign_tl" value="tl" title="Align to the Top Left">Top Left</option>
       <option id="salign_tr" value="tr" title="Align to the Top Right">Top Right</option>
       <option id="salign_bl" value="bl" title="Align to the Bottom Left">Bottom Left</option>
       <option id="salign_br" value="br" title="Align to the Bottom Right">Bottom Right</option>
       </select></td>
       </tr>
       <tr>
       <td nowrap align="left" title="Quality Setting">Quality:</td>
       <td nowrap align="left"><select onchange="previewFlash();" id="quality">
       <option id="quality_Low" value="Low" title="Set to Low Quality">Low</option>
       <option id="quality_Autolow" value="Autolow" title="Set to Autolow Quality">Autolow</option>
       <option id="quality_Autohigh" value="Autohigh" title="Set to Autohigh Quality">Autohigh</option>
       <option id="quality_Medium" value="Medium" title="Set to Medium Quality">Medium</option>
       <option id="quality_High" value="High" title="Set to High Quality">High</option>
       <option id="quality_Best" value="Best" title="Set to Best Quality">Best</option>
       </select></td>
       <td nowrap align="left" title="Scale">Scale:</td>
       <td nowrap align="left"><select onchange="previewFlash();" id="scale">
       <option id="scale_default" value="ShowAll" title="Scale to Default(Show all)">Default(Show all)</option>
       <option id="scale_NoBorder" value="NoBorder" title="Scale to No Border">No Border</option>
       <option id="scale_ExactFit" value="ExactFit" title="Scale to Exact Fit">Exact Fit</option>
       </select></td>

       </tr>
       <tr>
       <td nowrap align="left" title="Mode">WMode:</td>
       <td nowrap align="left"><select onchange="previewFlash();" id="wmode">
       <option id="wmode_Window" value="Window" title="Window">Window</option>
       <option id="wmode_Opaque" value="Opaque" title="Opaque">Opaque</option>
       <option id="wmode_Transparent" value="Transparent" title="Transparent">Transparent</option>
       </select></td>
       <td nowrap align="left">&nbsp;</td>
       <td>&nbsp;</td>
       </tr>

       </table>
</div>
