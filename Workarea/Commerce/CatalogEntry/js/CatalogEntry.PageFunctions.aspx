<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CatalogEntry.PageFunctions.aspx.cs" Inherits="Ektron.Cms.Commerce.Workarea.CatalogEntry.CatalogEntry_PageFunctions_Js" %>

Ektron.ready(function(){
    if (!parent.Ektron.Commerce || !parent.Ektron.Commerce.Coupons) {
        ResizeFrame(0);
    }
    setCheckVariable();
$ektron("body").click(function(){
        MenuUtil.hide();
    });
});

var inPublishProcess = false;
var click= false;
var checkVariable = false;

function Close()
{
    if (parent != null && parent != self && typeof parent.ektb_remove == 'function')
    {
        parent.ektb_remove();
    } else {
        self.close();
    }
}
function CloseEntryModal()
{
    ektb_remove();
}

function ToggleTangible(checkTangible)
{
    var toggle = !(checkTangible.checked);
    
    document.getElementById('txt_weight').disabled = toggle;
    document.getElementById('txt_height').disabled = toggle;
    document.getElementById('txt_length').disabled = toggle;
    document.getElementById('txt_width').disabled = toggle;

}

function ToggleInventory(checkInventory)
{
    var toggle = (checkInventory.checked);
    
    document.getElementById('txt_instock').disabled = toggle;
    document.getElementById('txt_onorder').disabled = toggle;
    document.getElementById('txt_reorder').disabled = toggle;
}

function ToggleAvail(checkArchived)
{
    var toggle = checkArchived.checked;
    
    document.getElementById('chk_buyable').disabled = toggle;
    if ( toggle ) { document.getElementById('chk_buyable').checked = !toggle; }

}

function RefreshPage(taxonomyOverriderId, bDynamicBox)
{
    if (bDynamicBox) {
        parent.ektb_remove();
        parent.location.reload(true);
    } else {
        if (top.opener.location.href.indexOf("#") != -1)
        {
            var loc = top.opener.location.href.split('#');
            if (taxonomyOverriderId != '') {
                var tempBuffer = new String( loc[0] );
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
                    top.opener.location.href = tempBuffer + "&__taxonomyid=" +taxonomyOverriderId;
                else
                    top.opener.location.href = tempBuffer + "?__taxonomyid=" + taxonomyOverriderId;
		            
               } else {
                top.opener.location.href = loc[0];
            }
        } else {
            top.opener.location.reload(true);
        }
    }      
}

function RemoveEntryImage(path) {
    var elem = null;
    var elemThumb = null;
    elem = document.getElementById( 'entry_image' );
    if (elem != null)
    {
        elem.value = '';
    }
    elemThumb = document.getElementById( 'entry_image_thumb' );
    if ( elemThumb != null )
    {
        elemThumb.src = path;
    }
}

function setCheckVariable()
{
    checkVariable = true;   
}

function ResizeFrame(val) 
{
    if ((typeof(top.ResizeFrame) == "function") && top != self) {
        top.ResizeFrame(val);
    }
}
function checkXmlForm()
{
    var errReason = 0;
    var errReasonT = 0;
    var errAccess = false;
    var errMessage = "";
    var sInvalidContent = "Continue saving invalid document?";
    var errContent = "Your data is not valid.";
    var objValidateInstance = null;
    if ("object" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances)
    {
        var objContentEditor = Ektron.ContentDesigner.instances['<asp:Literal ID="litEditorName" runat="server" />'];
        if (objContentEditor) 
        {
            errMessage = objContentEditor.validateContent();
        }
        if (errMessage != null && errMessage != "") 
        {
            if ("object" == typeof errMessage && "undefined" == typeof errMessage.code) 
            {
                AddError(errMessage.join("\n\n\n"));                    
            }
            else if ("object" == typeof errMessage) 
            {
                errReason = errMessage.code;
                errAccess = true;
                var objValidateAccess = null;
                for (var i = 0; i < objContentEditor.Modules.length; i++)
                {
                    if ("EkRadEditorXhtmlValidator" == objContentEditor.Modules[i].Id)
                    {
                        objValidateAccess = objContentEditor.Modules[i];
                    }
                }
                if (objValidateAccess && objValidateAccess.ContentField) 
                {
                    objValidateAccess.ContentField.value = errMessage.msg;
                    objValidateAccess.DoctypeField.value = errMessage.doctype;
                    objValidateAccess.ToggleCheckbox.checked = true;
                    objValidateAccess.ShowIframe(true);  
                    objValidateAccess.ValidateForm.action = objValidateAccess.getActionPage();
                    objValidateAccess.ValidateForm.submit();
                }
            }
            else if ("string" == typeof errMessage && errMessage.length > 0) 
            {
                AddError(errMessage);	             
            }
        }
    }
}

var aSubmitErr = new Array();

function OpenCalendar(bStartDate) 
{
	if (true == bStartDate) 
	{
		document.forms[0].go_live.value = Trim(document.forms[0].go_live.value);CallCalendar(document.forms[0].go_live.value, 'calendar.aspx', 'go_live', 'frmMain');
	} 
		else if (false == bStartDate) 
	{
		document.forms[0].end_date.value = Trim(document.forms[0].end_date.value);CallCalendar(document.forms[0].end_date.value, 'calendar.aspx', 'end_date', 'frmMain');
	}
}

<% =GetJsLibraryRemoveHTML() %>
<% =GetJsLibraryToggleDiv()%>

var nLimit; 
var temp = 0; //set to lValidCounter in CatalogEntry.aspx.vb which is zero
if (temp == "") { 
	nLimit = 0; 
} else { 
	nLimit = parseInt(temp); 
	if (isNaN(nLimit)) { 
		nLimit = 0; 
	} 
}

function CheckKeyValue(item, keys) { 
	var keyArray = keys.split("",""); 
	for (var i = 0; i < keyArray.length; i++) 
	{ 
		if ((document.layers) || ((!document.all) && (document.getElementById))) { 
			if (item.which == keyArray[i]) { 
				return false; 
			} 
		} else { 
			if (event.keyCode == keyArray[i]) { 
				return false; 
			} 
		} 
	} 
} 

function UpdateActualPrice(obj){
   var actualPrice = $ektron('div.ektron_PricingWrapper').find('input.actualPrice:text');
   var i = 0;
   for( i = 0; i < actualPrice.length; i++ ) {
       if(obj.id != actualPrice[i].id && actualPrice[i].value == 0.00 && !isNaN(obj.value) ) {
           var currencyPrice = $ektron('div.ektron_PricingWrapper').find('input.actualPrice:checkbox')[i];
           if(currencyPrice == undefined){ return false; }
           if(currencyPrice.checked == true) {
               var exchangeRate = currencyPrice.parentNode.parentNode.innerHTML.substring(currencyPrice.parentNode.parentNode.innerHTML.lastIndexOf('='));
               exchangeRate = exchangeRate.substring(6);
               if (isNaN(exchangeRate)) {
                   exchangeRate = exchangeRate.substring(1);
               }
               actualPrice[i].value = obj.value * exchangeRate;
           }
       }
   }
}

function UpdateListPrice(obj){
   var listPrice = $ektron('img[alt="List Price"]');
   var i = 0, k = 0;
   for( i = 0; i < listPrice.length; i++ ) {
       var elemListPrice = listPrice[i].parentNode.nextSibling;
       while (elemListPrice.nodeType == 3){ 
			// Fix for Mozilla/FireFox Empty Space becomes a TextNode
			elemListPrice = elemListPrice.nextSibling;
       };
       var finalListElement = $ektron(elemListPrice).find('input:text')[0];
       if(obj.id != finalListElement.id && finalListElement.value == 0.00 && !isNaN(obj.value)) {
           var currencyPrice = $ektron('div.ektron_PricingWrapper').find('input.actualPrice:checkbox')[i];
           if(currencyPrice == undefined){ return false; }
           if(currencyPrice.checked == true) {
               var exchangeRate = currencyPrice.parentNode.parentNode.innerHTML.substring(currencyPrice.parentNode.parentNode.innerHTML.lastIndexOf('='));
               exchangeRate = exchangeRate.substring(6);
               for (k = 0; k < exchangeRate.length; k++){
               if (isNaN(exchangeRate)) 
               {exchangeRate = exchangeRate.substring(1);}
               }
               finalListElement.value = obj.value * exchangeRate;
           }
       }
   }
}

function UpdateSalesPrice(obj)
{
   var listPrice = $ektron('img[alt="Our Sales Price"]');
     var i = 0, k = 0;
   for( i = 0; i < listPrice.length; i++ ) 
   {
       var elemListPrice = listPrice[i].parentNode.nextSibling;
       while (elemListPrice.nodeType == 3) 
	   { 
	   		// Fix for Mozilla/FireFox Empty Space becomes a TextNode
			elemListPrice = elemListPrice.nextSibling;
       };
       var finalListElement = $ektron(elemListPrice).find('input:text')[0];
       if(obj.id != finalListElement.id && finalListElement.value == 0.00 && !isNaN(obj.value) ) 
	   {
           var currencyPrice = $ektron('div.ektron_PricingWrapper').find('input.actualPrice:checkbox')[i];
           if (currencyPrice == undefined) {
		   	return false;
		   }
	           if(currencyPrice.checked == true) {
	               var exchangeRate = currencyPrice.parentNode.parentNode.innerHTML.substring(currencyPrice.parentNode.parentNode.innerHTML.lastIndexOf('='));
	               exchangeRate = exchangeRate.substring(6);
	               for (k = 0; k < exchangeRate.length; k++){
	               if (isNaN(exchangeRate)) 
	               {exchangeRate = exchangeRate.substring(1);}
	               }
	               finalListElement.value = obj.value * exchangeRate;
	       }
       }
   }
}

function DeleteItem() {
    var iAttr = getCheckedItemIndex(false);
    if ( iAttr == -2 ) {
        '<asp:Literal ID="litMessageNoItems" runat="server" />'
    } else if ( iAttr == -1 ) {
        '<asp:Literal ID="litMessagePleaseSelectItem" runat="server" />'
    } else {
        deleteChecked();
    }
}

function isInteger(s) {
   return s.length > 0 && !(/[^0-9]/).test(s);
}

function validate_bundle(){
   var productType = document.getElementById('hdn_productType').value;
   var j = 0;
   var count = 0;
   
   if(productType == '2') {
       var kitGroups = $ektron('div.EktronCommerceItems ul.kitGroups');
       
       if(kitGroups.children("li.kitGroup").length == 0) {
            <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotEnoughItemsKit" runat="server" />');
       }
       else
       {
           if(kitGroups.find("li.kitItem input.markedForDelete[value='false']").length == 0 )
           {
               <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotEnoughItemsKit1" runat="server" />');
           }
       }       
   } 
   else if( productType == '3') 
   {
       var lenProductItems = $ektron('li.defaultViewItem').find('span.delete');
       count = 0;
       if( lenProductItems.length <= 1 )
       {    
            <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotEnoughItemsBundle" runat="server" />');
       }
       else
       {
            for( j = 0; j < lenProductItems.length; j++ )
            {
                if( lenProductItems.find('input:hidden')[j].value == 'true' )
                {
                    count = count + 1;
                }
            }
            if( count == lenProductItems.length - 1 )
            {
                <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotEnoughItemsBundle1" runat="server" />');                
            }
       }
   }
}

function SubmitForm(iAction, timeoutCalled) 
{
    if (timeoutCalled == null)
        timeoutCalled = false;
    else if (timeoutCalled && Trim(document.getElementById('content_title').value) == '')
	    document.getElementById('content_title').value = 'Timed out checkin';
    $ektron('#pleaseWait').modalShow();
    <% =GetJsLibraryResetErrorFunctionName() %>();
	document.getElementById('hdn_publishaction').value = iAction;
	var height = document.getElementById('txt_height').value;
	var width = document.getElementById('txt_width').value;
	var length = document.getElementById('txt_length').value;
	var weight = document.getElementById('txt_weight').value;
	var instock = document.getElementById('txt_instock').value;
	var onorder = document.getElementById('txt_onorder').value;
	var reorder = document.getElementById('txt_reorder').value;
	var quantity = document.getElementById('txt_quantity');
	var sku = document.getElementById('txt_sku');
	
	function validate_BasePrice(timeoutCalled){
	        var defaultCurrencyId = $ektron("input#hdn_defaultCurrency");
	        var defaultCnstList = 'input#ektron_UnitPricing_ListPrice_' + defaultCurrencyId[0].defaultValue;
            var defaultCurrencyListPrice = $ektron(defaultCnstList)[0].value;
            var defaultCnstSale = 'input#ektron_UnitPricing_SalesPrice_' + defaultCurrencyId[0].defaultValue;
            var defaultCurrencySalePrice = $ektron(defaultCnstSale)[0].value;
            
            if (iAction != 4 && iAction != 5){
                if(!timeoutCalled && (defaultCurrencyListPrice == '0.00' || defaultCurrencySalePrice == '0.00')){
                    if(confirm('<asp:Literal ID="litConfirmBasePrice" runat="server" />')){
                        return true;
                    } else {                
				        return false;
                    }
                } else {
                    return true;
                }
            } else {
                return true;
            }        
        }
	
	function validate_FloatPricing(timeoutCalled){
            var chkFloat = $ektron("table.ektron_UnitPricing_Table").find("input:checkbox");
	        var totalChkFloat = 0;
	        var ret = true;
	        for(totalChkFloat = 0; totalChkFloat < chkFloat.length; totalChkFloat++)
	        {
	            var cmnId = chkFloat[totalChkFloat].id.substring(chkFloat[totalChkFloat].id.lastIndexOf("_"));
	            var listPriceId = 'input#ektron_UnitPricing_ListPrice' + cmnId;
	            var listPrice = $ektron(listPriceId)[0].value;
	            var salePriceId = 'input#ektron_UnitPricing_SalesPrice' + cmnId;
	            var salePrice = $ektron(salePriceId)[0].value;
	            if(!timeoutCalled && (!chkFloat[totalChkFloat].checked && (listPrice == '0.00' && salePrice == '0.00')))
	            {
                    if(confirm('<asp:Literal ID="litConfirmFloatPrice" runat="server" />')){
                        ret = true;
                    } else {
                        ret = false;
                    }
	            }
	        }
	        if(ret != false){
	            return true;
	        } else {
	            return false;
	        }
	    }
	
	if (iAction != 5) // cancel
	{
        Ektron.Commerce.MediaTab.Images.getImageData();

	    if (sku.disabled == false)
	    {
	        if(!timeoutCalled && (sku.value == '' || sku.value == '0' || sku.value.indexOf(' ') != -1))
		    {
		        <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotValidSku" runat="server" />');
		    }
		    if(sku.value.indexOf('<')>-1 ||sku.value.indexOf('>')>-1)
		    {
		        <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litCannotContaintSpecialCharacters" runat="server" />');
		    }
		    if(isNaN(quantity.value) || quantity.value < 1 || !isInteger(quantity.value)) 
		    {
		        if (timeoutCalled)
		            quantity.value = 1;
		        else
		        {
		            <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotValidPropQuantity" runat="server" />');
			        quantity.focus();
			    }
		    }
    		
		    var bTngItm = document.getElementById('chk_tangible');
    		
		    if (bTngItm.checked == true) 
		    {
			    if (isNaN(height) || isNaN(width) || isNaN(length) || isNaN(weight) || height == '' || width == '' || length == '' || weight == '' || height == 0 || width == 0 || length == 0 || weight == 0) 
			    {
			        <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotValidDimensions" runat="server" />');
			    }
		    }
		    if( height > 9999 || width > 9999 || length > 9999 || weight > 9999 )
            {
			        <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litInvalidExcessDimensions" runat="server" />');        
                    $ektron('body.UiMain').find('td#dvEntry').click();            
            }
            
		    if (isNaN(instock) || isNaN(onorder) || isNaN(reorder)) 
		    {
		        <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotValidInventory" runat="server" />');
		    }
    			
		    validate_bundle();
    		
    		
		    if(!validate_BasePrice(timeoutCalled)){
                $ektron('#pleaseWait').modalHide();	        
			    $ektron('td#dvPricing').click();
			    buttonPressed = false;
			    return false;
		    }
		    var attrLength = 0;
		    var tiers = $ektron('td.tierQuantity').find('input').length;
		    var prices = $ektron('td.tierPrice').find('input').length;
		    var inputPriceLength = $ektron('table.ektron_UnitPricing_Table').find('input:text').length;
		    var k = 0;
		    var i = 0;
    		
		    for (k = 0; k < inputPriceLength; k++)
		    {
			    var inputPriceObj = $ektron('table.ektron_UnitPricing_Table').find('input:text')[k];
			    if (isNaN(inputPriceObj.value.replace(/,/g, ''))) 
			    {
				    if (inputPriceObj.id.indexOf('ektron_UnitPricing_ActualPrice_') == 0) 
				    {
				        <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotValidPurchase" runat="server" />');
				    }
				    else 
					    if (inputPriceObj.id.indexOf('ektron_UnitPricing_ListPrice_') == 0) 
					    {
					        <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotValidList" runat="server" />');
					    }
					    else 
						    if (inputPriceObj.id.indexOf('ektron_UnitPricing_SalesPrice_') == 0) 
						    {
						        <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotValidSales" runat="server" />');
						    }
			    }
		    }
    		
		    if(!validate_FloatPricing(timeoutCalled)){
                $ektron('#pleaseWait').modalHide();		    
			    $ektron('td#dvPricing').click();
			    buttonPressed = false;
			    return false;
		    }
    		
		    var txtAttr = $ektron('div#_dvAttrib').find('input:text');
		    for (attrLength = 0; attrLength < txtAttr.length; attrLength++) 
		    {
			    if (isNaN(txtAttr[attrLength].value.replace(/,/g, ''))) 
			    {
			        <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotValidNumericAttribute" runat="server" />');
			    }
		    }
    		
		    var dateAttr = $ektron('div#_dvAttrib').find('span');
		    for(attrLength = 0; attrLength < dateAttr.length; attrLength++){
		        if(dateAttr[attrLength].innerHTML == '[None]'){
			        <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageEmptyDateAttribute" runat="server" />');			    
                }
            }		        
    		
		    for (i = 0; i < tiers; i++) 
		    {
			    var quantValue = $ektron('td.tierQuantity').find('input')[i].value;
			    if (quantValue == '') { quantValue = '0'; }
			    if (isNaN(quantValue.replace(/,/g, '')) || !isInteger(quantValue.replace(/,/g, ''))) 
			    {
			        <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotValidQuantity" runat="server" />');
			    }
		    }
    		
		    for (j = 0; j < prices; j++) 
		    {
			    var priceValue = $ektron('td.tierPrice').find('input')[j].value;
			    if (isNaN(priceValue.replace(/,/g, ''))) 
			    {
				    <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageNotValidPrice" runat="server" />');
			    }
		    }
    			
		    if (iAction == 0 || iAction == 1 || iAction == 2) 
		    {			
			    if (!ValidateMeta(0)) 
			    {
                    $ektron('#pleaseWait').modalHide();			    
				    $ektron('td#dvMeta').click();
				    buttonPressed = false;
				    return false;
			    }
		    }
	    }
        <asp:PlaceHolder ID="phTaxonomyCategoryRequired" runat="server" Visible="False">
            if (!timeoutCalled && (iAction != 3 || iAction != 4) && (Trim(document.getElementById('taxonomyselectedtree').value) == '')) {
                <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageTaxonomyCategoryRequired" runat="server" />');
                $ektron('body.UiMain').find('td#dvCategories').click();
            }
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phAliasRequired" runat="server" Visible="false">
            var manualAlias = document.getElementById('frm_manalias').value;
            if (!timeoutCalled && (iAction != 3 || iAction != 4) && manualAlias == '') {
                <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageAliasRequired" runat="server" />');
                $ektron('body.UiMain').find('td#dvAlias').click();
            }
        </asp:PlaceHolder>
    }

    switch (iAction) { 
        //"Save"
	    case 0: 
            handler_Save(); 
            break; 
        //"Checkin"
	    case 1: 
            handler_CheckinHandler(timeoutCalled); 
            break; 
	    //"Submit"
	    case 2: 
            handler_SubmitHandler(); 
            break; 
	    //"UndoCheckout"
        case 4: 
            handler_UndoCheckoutHandler(); 
            break;
	    //"Cancel" 
        case 5: 
            handler_CancelHandler(); 
            break; 
    }
}

function handler_Save() { 
    <% =GetJsLibraryToggleDivFunctionName()%>('pleaseWait');
	checkXmlForm();
	validate_Title(); 
	validate_Subscription();
	validate_Quantity();
	WriteBundleValues();
	<% =GetJsLibraryShowErrorFunctionName()%>('document.forms[0].submit();', null, "$ektron('#pleaseWait').modalHide();");
} 

function handler_CheckinHandler(timeoutCalled) { 
	if (!timeoutCalled)
	    checkXmlForm();
	validate_Title(timeoutCalled); 
	validate_Subscription();
	validate_Quantity(); 
	<% =GetJsLibraryToggleDivFunctionName()%>('pleaseWait'); 
	WriteBundleValues();
	<% =GetJsLibraryShowErrorFunctionName()%>('document.forms[0].submit();', null, "$ektron('#pleaseWait').modalHide();");
} 

function handler_SubmitHandler() { 
	checkXmlForm();
	validate_Title(); 
	validate_Subscription();
	validate_Quantity();
	<% =GetJsLibraryToggleDivFunctionName()%>('pleaseWait');
	WriteBundleValues();
	<% =GetJsLibraryShowErrorFunctionName()%>('document.forms[0].submit();', null, "$ektron('#pleaseWait').modalHide();");
} 

function handler_UndoCheckoutHandler() { 
	if (confirm('<asp:Literal ID="litMessageConfirmCloseNoSaveEntry1" runat="server" />')) {
	    <% =GetJsLibraryResetErrorFunctionName()%>();
	    <% =GetJsLibraryToggleDivFunctionName()%>('pleaseWait');
	    <% =GetJsLibraryShowErrorFunctionName()%>('document.forms[0].submit();');
	}
	else
	{
	    $ektron('#pleaseWait').modalHide();	    
	}
} 

function handler_CancelHandler() {
    <% =GetJsLibraryToggleDivFunctionName()%>('pleaseWait');
	if (confirm('<asp:Literal ID="litMessageConfirmCloseNoSaveEntry2" runat="server" />')) {
		window.location.href = '../content.aspx?action=ViewContentByCategory&id=<% =GetFolderId() %>'; 
	}
	else
	{   
	    $ektron('#pleaseWait').modalHide();
	}
} 

function validate_Subscription()
{

    if (!validate_RecurringBilling())
        return false
    
    if (document.getElementById('txt_sku').disabled == false && <% =GetEntryType().GetHashCode()%> == <% =Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct.GetHashCode()%> && typeof validate_SubscriptionDelegate == 'function') {
        
        return (validate_SubscriptionDelegate());
        
    } else {
    
        return true;
        
    }
    
}

function validate_RecurringBilling()
{
    
    var usesRecurring = (document.getElementById('PricingTabRecurringBillingUseRecurrentBilling') != null && document.getElementById('PricingTabRecurringBillingUseRecurrentBilling').selectedIndex == 0);
    var billingInterval = (usesRecurring ? document.getElementById('PricingTabRecurringBillingInterval').value : 0);

    if (usesRecurring && !(billingInterval >= 1)) {
    
        <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageErrorBillingInterval" runat="server" />');
        return false;
        
    } else {
        return true;
    }
}

function validate_Title() {
	var sTitle = Trim(document.getElementById('content_title').value);
	if (sTitle == '') {
	    <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageErrorEntryTitleRequired" runat="server" />');
	}
    HasIllegalChar('content_title', "<asp:Literal ID="litMessageEntryDisallowedCharacters" runat="server" />");
} 

function validate_Quantity() { 
	var iQuantity = parseInt(Trim(document.getElementById('txt_quantity').value)); 
	if ( !(iQuantity > 0) ) {
	    <% =GetJsLibraryAddErrorFunctionName() %>('<asp:Literal ID="litMessageErrorEntryQuantityRequired" runat="server" />');
	} 
}

<% =GetJsLibraryAddError() %>
<% =GetJsLibraryShowError() %>
<% =GetJsLibraryResetError() %>
<% =GetJsLibraryHasIllegalCharacters() %>
<% =GetJsLibraryToggleDiv() %>
<% =GetJsLibraryResizeFrame() %>

function openBundleSelection() { 
	ektb_show('','itemselection.aspx?id=<%=GetFolderId()%>&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true', null); 
}

function ShowCurrency(currencyList)  { 
	var selectedCurrencyId = currencyList.options[currencyList.selectedIndex].value; 
	currencyTables = document.getElementsByTagName('table'); 
	for (var i = 0; i < currencyTables.length; i++)  { 
		if (currencyTables[i].id.indexOf('tblcurrency') == 0) { 
			if (currencyTables[i].id == 'tblcurrency' + selectedCurrencyId) {
			    <% =GetJsLibraryToggleDivFunctionName()%>(currencyTables[i].id, true);
				document.getElementById('currencylabel').innerHTML = currencyList.options[currencyList.selectedIndex].text;
			} else {
			    <% =GetJsLibraryToggleDivFunctionName()%>(currencyTables[i].id, false);
			}
		} 
	} 
}

function PreviewContent(obj, action, contTitle){
    SubmitForm(action);
    window.open(obj,'contTitle','scrollbars=yes,resizable=yes');
    return false;    
}