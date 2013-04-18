<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImageScore.aspx.cs" Inherits="Widgets_Dialogs_ImageScore" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">    
    <title>Image Score</title>
    <style runat="server">    
    DIV.floating_menu
    {
        BORDER-RIGHT: black 1px solid;
        BORDER-TOP: black 1px solid;
        BORDER-LEFT: black 1px solid;
        BORDER-BOTTOM: black 1px solid;
        PADDING-RIGHT: 3px;
        PADDING-LEFT: 3px;
        PADDING-BOTTOM: 3px;
        PADDING-TOP: 3px;
        Z-INDEX: 1;
        LEFT: 10%;
        FLOAT: none;
        VISIBILITY: visible;
        MARGIN: 2px;
        POSITION: absolute;
        TOP: 10%;
        BACKGROUND-COLOR: transparent;
    }  
    DIV.floating_scale
    {
        BORDER: gray none 0px;
        PADDING-RIGHT: 0px;
        PADDING-LEFT: 0px;
        PADDING-BOTTOM: 0px;
        PADDING-TOP: 0px;
        Z-INDEX: 1;
        LEFT: 10%;
        FLOAT: none;
        VISIBILITY: visible;
        MARGIN: 0px;
        POSITION: absolute;
        TOP: 10%;
        BACKGROUND-COLOR: transparent;
    } 
    .opaque_area
    {
        BACKGROUND-COLOR: gray;
        filter:progid:DXImageTransform.Microsoft.Alpha(opacity = 50);
    } 
    DIV.floating_highlight
    {
        position:absolute;
        left:0px; top:0px;
        width:100%; height:100%;
        visibility:hidden;
        border-top:silver solid 1px;
        border-bottom:silver solid 1px;
        border-left:silver solid 1px;
        border-right:silver solid 1px;
        font-size:8pt;
    }
    .floating_textentry
    {
        background-color:Transparent;
        font-family:Verdana;
        font-size:12pt;
        font-weight:normal;
        text-shadow:white 2px 2px 2px;
        POSITION: absolute;
        BORDER-RIGHT: black 3px inset;
        BORDER-TOP: gray 3px solid;
        BORDER-LEFT: gray 3px solid;
        BORDER-BOTTOM: black 3px inset;
        PADDING-RIGHT: 0px;
        PADDING-LEFT: 0px;
        PADDING-BOTTOM: 0px;
        PADDING-TOP: 0px;
        Z-INDEX: 1;
        FLOAT: none;
        MARGIN: 0px;
        POSITION: absolute;
        TOP: 10%;
    }
 	.tickmarkstep
	{
	    /* TODO: Ross - Move CSS to WorkArea.css & fix this path */
		background-image:url(http://localhost/workarear2/workarea/Resources/Workarea/Images/tickmark.gif);
        PADDING-RIGHT: 0px;
        PADDING-LEFT: 0px;
        PADDING-BOTTOM: 0px;
        PADDING-TOP: 0px;
        MARGIN: 0px;
        BORDER-RIGHT: black 0px solid;
        BORDER-TOP: black 0px solid;
        BORDER-LEFT: black 0px solid;
        BORDER-BOTTOM: black 0px solid;
		FONT-SIZE:3pt;			
		WIDTH:14px;
		HEIGHT:14px;		
	}
    .floating_slider
    {
        Z-INDEX: 1;
        LEFT: 1%;
        FLOAT: none;
        VISIBILITY: visible;
        MARGIN: 0px;
        POSITION: absolute;
        TOP: 1%;
        BACKGROUND-COLOR: transparent;
		WIDTH:14px;
		HEIGHT:14px;		
    } 	
</style>  

</head>

<body oncontextmenu="return false"
 leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0">

<form id="form1" runat="server">
    <div id="dblclickwatch" runat="server" style="height:100%;">
        <asp:Image ID="ImageDisplay" runat="server" BorderWidth="0px" Height="100%" />
        
        <asp:HiddenField ID="CommandModeField" runat="server" />
        <asp:HiddenField ID="CommandActionField" runat="server" />
        
        <asp:HiddenField ID="SourceModImageField" runat="server" />
        <asp:HiddenField ID="DestModImageField" runat="server" />
        <asp:HiddenField ID="TmpRelativeFileNameField" runat="server" />
        <asp:HiddenField ID="ModImageIDField" runat="server" />
        <asp:HiddenField ID="LastCmdIDField" runat="server" />
        
        <asp:HiddenField ID="DataOnSubmit" runat="server" />
        <asp:HiddenField ID="TextStartX" runat="server" />
        <asp:HiddenField ID="TextStartY" runat="server" />
        <asp:HiddenField ID="TextWidth" runat="server" />
        <asp:HiddenField ID="TextHeight" runat="server" />
        <asp:HiddenField ID="SliderLevelField" runat="server" />

        <div class="floating_menu" id="RotateMenu" style="visibility:hidden">
            <asp:Button ToolTip="Rotate Left" ID="btnRotLeft" runat="server" OnClick="RotateImageLeft_Click" Text=" << " />
            <asp:Button ToolTip="Rotate Right" ID="btnRotRight" runat="server" OnClick="RotateImageRight_Click" Text=" >> " />
        </div>

        <div class="floating_menu" runat="server" id="OldBrightnessMenu" visible="false">
            <asp:Button ToolTip="Decrease Brightness (fast)" ID="btnDecBrFast" runat="server" OnClick="DecBrFast_Click" Text=" << " />
            <asp:Button ToolTip="Decrease Brighness (slow)" ID="btnDecBrSlow" runat="server" OnClick="DecBrSlow_Click" Text=" <  " />
            <asp:Button ToolTip="Increase Brightness (slow)" ID="btnIncBrSlow" runat="server" OnClick="IncBrSlow_Click" Text="  > " />
            <asp:Button ToolTip="Increase Brightness (fast)" ID="btnIncBrFast" runat="server" OnClick="IncBrFast_Click" Text=" >> " />
        </div>
        <div class="floating_scale" runat="server" id="BrightnessMenu" style="visibility:hidden">
            <asp:ImageButton ID="ImageButton_13" runat="server" OnClick="AddBrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None"  /><br />
            
            <div runat="server" id="TickArea" visible="true">
            <asp:ImageButton ToolTip="Raise Brightness 100%" ID="ImageButton_1" runat="server" OnClick="Add100BrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None"  /><br />
            <asp:ImageButton ToolTip="Raise Brightness 80%" ID="ImageButton_2" runat="server" OnClick="Add80BrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None" /><br />
            <asp:ImageButton ToolTip="Raise Brightness 60%" ID="ImageButton_3" runat="server" OnClick="Add60BrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None" /><br />
            <asp:ImageButton ToolTip="Raise Brightness 40%" ID="ImageButton_4" runat="server" OnClick="Add40BrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None" /><br />
            <asp:ImageButton ToolTip="Raise Brightness 20%" ID="ImageButton_5" runat="server" OnClick="Add20BrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None" /><br />

            <asp:ImageButton ToolTip="Set Brightness 0%" ID="ImageButton_6" runat="server" OnClick="ZeroBrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None" /><br />

            <asp:ImageButton ToolTip="Lower Brightness 20%" ID="ImageButton_7" runat="server" OnClick="Sub20BrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None" /><br />
            <asp:ImageButton ToolTip="Lower Brightness 40%" ID="ImageButton_8" runat="server" OnClick="Sub40BrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None" /><br />
            <asp:ImageButton ToolTip="Lower Brightness 60%" ID="ImageButton_9" runat="server" OnClick="Sub60BrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None" /><br />
            <asp:ImageButton ToolTip="Lower Brightness 80%" ID="ImageButton_10" runat="server" OnClick="Sub80BrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None" /><br />
            <asp:ImageButton ToolTip="Lower Brightness 100%" ID="ImageButton_11" runat="server" OnClick="Sub100BrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None" /><br />
            </div>
            
            <asp:ImageButton ID="ImageButton_12" runat="server" OnClick="SubBrightnessStep" BorderWidth="0" Height="14px" Width="14px" BorderStyle="None" /><br />           

        </div>

        <asp:HiddenField ID="TextFieldName" runat="server" Value="" />
        <div runat="server" id="TextInsertData" visible="false">
            <asp:HiddenField ID="ImgTextFont" runat="server" Value="Verdana" />
            <asp:HiddenField ID="ImgTextSize" runat="server" Value="12" />
            <asp:HiddenField ID="ImgTextBold" runat="server" Value="true" />
            <asp:HiddenField ID="ImgTextItal" runat="server" Value="false" />
            <asp:HiddenField ID="ImgTextColor" runat="server" Value="0" />
        </div>

        <asp:Literal ID="UIJavaScript" runat="server" Text=""></asp:Literal> 
        
        <asp:HiddenField ID="IsExec" runat="server" Value="" />
        <asp:HiddenField ID="hdnSrcFileWidth" runat="server" />
        <asp:HiddenField ID="hdnSrcFileHeight" runat="server" />

    </div>
 </form>        
</body>
</html>
