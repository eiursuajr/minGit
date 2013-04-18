<%@ Control Language="C#" AutoEventWireup="true" CodeFile="realplayerparams.ascx.cs" Inherits="Multimedia_realplayerparams" %>
<script id="realplayerparams" type="text/javascript" language="javascript">
    Ektron.ready(function()
    {
        var jsrealplayer = new ektRealPlayer();
    });
    
    function PreviewRealPlayer()
    {
        jsrealplayer.preview();
    }
</script>
<!-- Real Player properties -->
<div id="realplayer-panel" style="display:none">
<input name="rp_type" type="hidden" id="rp_type" value="" />
<table width="100%" cellspacing="0" cellpadding="0" border="0">
	<tr>
	    <td colspan="2">
	        <table>
	            <tr>
	                <td>Center:<input onchange="PreviewRealPlayer()" type="checkbox" title="Center" id="rp_center" name="rp_center" /></td>
	                <td>NoLogo:<input onchange="PreviewRealPlayer()" type="checkbox" title="No Logo" name="rp_nologo" id="rp_nologo" /></td>
	            </tr>
	            <tr>
	                <td>Maintain Aspect:<input onchange="PreviewRealPlayer()" type="checkbox" title="Maintain Aspect Ratio" id="rp_aspect" name="rp_aspect" /></td>
	                <td>&nbsp;</td>	            
	            </tr>
	            <tr>
	                <td>Number of Loops:<input onchange="PreviewRealPlayer()" title="Number of Loops" type="text" id="rp_numloops" name="rp_numloops" style="width: 50px" /></td>
	                <td>Controls:
	                <select onchange="PreviewRealPlayer()" id="rp_controls" name="rp_controls">
	                    <option id="rp_controls_all" value="all" title="All (Default)">All (Default)</option>
	                    <option id="rp_controls_positionslider" value="PositionSlider" title="Position Slider">Position Slider</option>
	                    <option id="rp_controls_FFCtrl" value="FFCtrl" title="Fast Forward Button">Fast Forward Button</option>
	                    <option id="rp_controls_imagewindow" value="ImageWindow" title="Image Window">Image Window</option>
	                    <option id="rp_controls_infopanel" value="InfoPanel" title="Information Panel">Information Panel</option>
	                    <option id="rp_controls_MuteCtrl" value="MuteCtrl" title="Mute Button">Mute Button</option>
	                    <option id="rp_controls_PauseButton" value="PauseButton" title="Pause Button">Pause Button</option>
	                    <option id="rp_controls_PlayOnlyButton" value="PlayOnlyButton" title="Play Button">Play Button</option>
	                    <option id="rp_controls_RWCtrl" value="RWCtrl" title="Rewind Button">Rewind Button</option>
	                    <option id="rp_controls_StopButton" value="StopButton" title="Stop Button">Stop Button</option>
	                    <option id="rp_controls_VolumeSlider" value="VolumeSlider" title="Volume Slider">Volume Slider</option>
	                    <option id="rp_controls_HomeCtrl" value="HomeCtrl" title="www.real.com Home Button">www.real.com Home Button</option>
	                    <option id="rp_controls_MuteVolume" value="MuteVolume" title="Mute / Volume Bar">Mute / Volume Bar</option>
	                    <option id="rp_controls_StatusBar" value="StatusBar" title="Status Bar">Status Bar</option>
	                    <option id="rp_controls_StatusField" value="StatusField" title="Status Field">Status Field</option>
	                    <option id="rp_controls_PositionField" value="PositionField" title="PositionField">PositionField</option>	                    
	                </select>
	                </td>
	            </tr>
	        </table>
	    </td>
	</tr>							
</table>
</div> 
<!-- /Real Player Properties -->
