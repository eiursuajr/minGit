<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DeleteDeviceConfiguration.ascx.cs" Inherits="DeleteDeviceConfiguration" %>
 
<script type="text/javascript" language="javascript">
     
     function ConfirmConfigurationDelete()
      {
	        var itemChecked = false ;
	        if(document.forms[0].deleteConfigurationId)
	             {
	                if(document.forms[0].deleteConfigurationId.length)
	                {
		                for(var x=0;x<document.forms[0].deleteConfigurationId.length;x++)
		                    {
			
			                    if(document.forms[0].deleteConfigurationId[x].checked == true)
				                    itemChecked = true ;
		                    }
	                 } 
	                else 
	                 {
		                if(document.forms[0].deleteConfigurationId.checked) { itemChecked = true ; }
	                 }
                  }
	        if(!itemChecked) 
	        {
		        alert('<asp:Literal id="ltrMsgSelItem" runat="server" />') ;
		        return(false) ;
	        }
	        
	        var answer = confirm('<asp:Literal id="ltrMsgDelConfig" runat="server" />') ;
	        if(answer)
	        {
	            document.forms[0].submit();
	        }
	        else
	        {
	            return false;
	        }
	        
	        
    }
</script>
    
<div id="dhtmltooltip"></div>
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
        <div class="ektronToolbar" id="divToolBar" runat="server"></div>
    </div>

    <div class="ektronPageContainer">
         <div title="Device Type List" id="dvDeviceList" class="ektronPageInfo">
              <asp:DataGrid id="DeviceListGrid"
                            runat="server"
                            CssClass="ektronGrid"
                            AutoGenerateColumns="False"
                            EnableViewState="False">
                            <HeaderStyle CssClass="title-header" />
              </asp:DataGrid>
         </div>
   </div> 