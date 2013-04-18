<%@ Control Language="C#" AutoEventWireup="true" Inherits="viewadconfigure" CodeFile="viewadconfigure.ascx.cs" %>

<style type="text/css">
    .spacer5em {
        margin-bottom: .5em
    }
</style>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <!-- Version / Build / Service Pack -->
    <span id="versionNumber" runat="server"></span>
    <span id="buildNumber" runat="server"></span>
	<span id="servicePack" runat="server"></span>

	<div class="info" id="licenseMessageContainer" runat="server">
	    <div class="ektronTopSpace"></div>
		<div class="important" id="licenseMessage" runat="server"></div>
	</div>
	
	<div class="ektronTopSpace"></div>
	<div class="ui-widget" id="sync" runat="server">
        <div class="ui-state-highlight ui-corner-all" style="padding: 0 0.7em; margin-top: 20px;"> 
            <span class="ui-icon ui-icon-info" style="float: left; margin-right: 0.3em;"></span>
                <asp:Literal ID="ltr_status" runat="server"></asp:Literal>  
        </div>
        <div class="spacer5em"></div>
    </div>

    <!-- Active Directory Installed -->
    <fieldset>
        <legend><span id="installed" runat="server"></span></legend>    
		<table class="ektronGrid">
			<tr>			    
				<td class="label" id="TD_flag" runat="server"></td>				
				<td class="readOnlyValue" id="TD_flagenabled" runat="server"></td>
			</tr>
			<tr>
			    <td colspan="2" style="padding-left:30px">
			        <table class="ektronForm">
			            <tr>			    					
				            <td class="label" id="TD_dirflag" runat="server"></td>				
				            <td class="readOnlyValue" colspan="2" id="TD_intflag" runat="server"></td>
			            </tr>
			            <tr>			    
				            <td class="label" id="TD_autouser" runat="server"></td>				
				            <td class="readOnlyValue" id="TD_autouserflag" runat="server"></td>
			            </tr>
			            <tr>			    
				            <td class="label" id="TD_autogroup" runat="server"></td>				
				            <td class="readOnlyValue" id="TD_autogroupflag" runat="server"></td>
			            </tr>	
			        </table>		    
			    </td>
			</tr>
		</table>
    </fieldset>
    
    <!-- AutoAddType -->
    <div class="ektronTopSpaceSmall"></div>
    <fieldset>
	    <legend id="lgd_autoAddType" class="ektronHeader" runat="server"></legend>   
        <table class="ektronForm">
            <tr>
                <td class="label"><span id="autoAddTypeProperty" runat="server"></span>:</td>
	            <td class="readOnlyValue"><span id="autoAddTypeValue" runat="server"></span></td>
            </tr>
        </table>
    </fieldset>
    
    <!-- User Property Association -->
    <div class="ektronTopSpaceSmall"></div>
    <fieldset>
        <legend id="userProperty" runat="server"></legend>
		<table class="ektronForm">
			<tr>
				<td id="TD_cmstitle" style="padding-right:6px;" runat="server"></td>				
				<td id="TD_dirproptitle" runat="server"></td>
			</tr>
			<asp:Literal ID="mapping_list" Runat="server" />
		</table>
	</fieldset>

    <!-- CMS Admistrator Group Association -->
    <div id="TR_domaindetail" runat="server">
        <div class="ektronTopSpaceSmall"></div>
        <fieldset>
            <legend id="adminGroupMap" runat="server"></legend>    	
		    <table>
			    <tr>
				    <td id="TD_grpnameval" runat="server"></td>
				    <td class="info">@</td>
				    <td id="TD_grpDomainVal" runat="server"></td>
			    </tr>	
		    </table>
	    </fieldset>
    </div>

    <!-- Domain -->
    <div class="ektronTopSpace"></div>
    <table class="ektronForm">        
        <tr>
            <td class="label" id="domain" runat="server"></td>	        
            <td class="readOnlyValue" id="domainValue" runat="server"></td>
        </tr>
    </table>
</div>

