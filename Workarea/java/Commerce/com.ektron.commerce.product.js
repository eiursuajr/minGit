// Class Ektron_Ecommerce_ProductClass:
function Ektron_Ecommerce_ProductClass(id)
{
    ///////////////////////////////////////////////////////
    // Instance members - variables:
    this._id = id;
    this._ajaxCall = null;
	this._ajaxCallbackId = "";
	this._readyFlag = false;
	this._browserInitDelay = 100;
	this._retryDelay = 50;
	this._ajaxImage = "";
	this._imagePath = "";
	this._blockedItems = [];
	this._filterSelected = "";

    ///////////////////////////////////////////////////////
    // Instance members - methods:


    ///////////////////////////////////
    // Initialize: Called when this object is first created.
	this.Constructor = function()
	{
	    // Initialize state here:
	    // ...
	    var productObjId = this.GetId();
	    //$ektron(document).load(function(){Ektron_Ecommerce_ProductClass.GetObject(productObjId).Initialize()});
	    setTimeout(function() { Ektron_Ecommerce_ProductClass.GetObject(productObjId).Initialize(); }, this._browserInitDelay);
	};

    ///////////////////////////////////
    // Initialize: 
    //
	this.Initialize = function()
	{
	    this.InitializeUI();
	};

    ///////////////////////////////////
    // GetId: Returns the client-id associated with the server control.
	this.GetId = function()
	{
	    return (this._id);
	};

    ///////////////////////////////////
    //
	this.IsReady = function()
	{
	    return (this._readyFlag);
	};

    ///////////////////////////////////
    //
	this.SetAjaxCall = function(ptr)
	{
	    this._ajaxCall = ptr;
	};

    ///////////////////////////////////
    //
	this.GetAjaxCall = function()
	{
	    return (this._ajaxCall);
	};

    ///////////////////////////////////
    //
	this.SetAjaxCallbackId = function(id)
	{
	    this._ajaxCallbackId = id;
	};

    ///////////////////////////////////
    //
	this.GetAjaxCallbackId = function()
	{
	    return (this._ajaxCallbackId);
	};

    ///////////////////////////////////
    //
	this.SetAjaxImage = function(src)
	{
	    this._ajaxImage = src;
	};

    ///////////////////////////////////
    //
	this.SetImagePath = function(path)
	{
	    this._imagePath = path;
	};


    ///////////////////////////////////
    //
	this.DoAjaxCallback = function(args, context)
	{
	    var targObj = null;

	    // Show Ajax-loading image if specified:
	    if (this._ajaxImage.length > 0)
	    {
	        this.ShowAjaxWorking();
	    }

	    // Do Ajax-Callback to server:
	    var fPtr = this.GetAjaxCall();
	    if (("undefined" != typeof fPtr) && (null !== fPtr))
	    {
	        // Use MS Ajax client side code:
	        (fPtr)(this.GetId(), args, this.GetId().toString());
	    }
	    else
	    {
	        // Do not use MS Ajax client side code:
	        var ektronPostParams = "";
	        ektronPostParams += "&__CALLBACKID=" + this.GetAjaxCallbackId(); // Must use ASP.NET UniqueID as that's what's expected on server side when control is used with master pages.
	        ektronPostParams += "&__CALLBACKPARAM=" + escape(args);
	        ektronPostParams += "&__VIEWSTATE="; // + encodeURIComponent($ektron("#__VIEWSTATE").attr("value"));
	        context = this.GetId().toString();
	        var ajaxUrl = window.location.href.replace(window.location.hash, ""); // IE/Windows will block callbacks with a pound/hash symbol in the URL.
	        $ektron.ajax({
	            type: "POST",
	            url: ajaxUrl,
	            data: ektronPostParams,
	            dataType: "html",
	            error: function(ektronCallbackResult) { Ektron_Ecommerce_ProductClass.GetObject(context).AjaxCallback_DisplayError(ektronCallbackResult, context); },
	            success: function(ektronCallbackResult)
	            {
	                var PACKET_START = "EKTRON_PACKET_START|";
	                var payload = String(ektronCallbackResult);
	                var idx = payload.indexOf(PACKET_START);
	                if (idx >= 0) {
	                    var payload = payload.substring(idx + PACKET_START.length);
	                }
	                Ektron_Ecommerce_ProductClass.GetObject(context).AjaxCallback_DisplayResult(payload, context);
	            }
	        });
	    }

	    return (false); // cancel events.
	};

    ///////////////////////////////////
    //
	this.AjaxCallback_DisplayResult = function(args, context)
	{
	    if (args && args.length)
	    {
	        var checkOutObj = Ektron_Ecommerce_ProductClass.GetObject(context, false);
	        var responseBlocks = args.split("|");
	        if (responseBlocks && responseBlocks.length)
	        {
	            var resStatus = responseBlocks[0];
	            if ("1" == resStatus)
	            {
	                var jsonLength = parseInt(responseBlocks[1], 10);
	                var markupLength = parseInt(responseBlocks[2], 10);
	                var markupTargetId = responseBlocks[3];
	                var deferJson = ("1" == responseBlocks[4]);
	                var json = "";
	                var markup = "";
	                var jsonPayloadOffset = (responseBlocks[0].length + 1 + responseBlocks[1].length + 1 + responseBlocks[2].length + 1 + responseBlocks[3].length + 1 + responseBlocks[4].length + 1);
	                if (!deferJson)
	                {
	                    if (jsonLength)
	                    {
	                        json = args.substr(jsonPayloadOffset, jsonLength);
	                        eval(json);
	                    }
	                }
	                var marupPayloadOffset = jsonPayloadOffset + jsonLength;
	                if (markupLength)
	                {
	                    markup = args.substr(marupPayloadOffset, markupLength);
	                    if (markupTargetId.length > 0)
	                    {
	                        $ektron(markupTargetId).html(markup);
	                    }
	                }
	                if (deferJson)
	                {
	                    if (jsonLength)
	                    {
	                        json = args.substr(jsonPayloadOffset, jsonLength);
	                        eval(json);
	                    }
	                }
	            }
	        }
	    }
	};

    ///////////////////////////////////
    //
	this.AjaxCallback_DisplayError = function(args, context)
	{
	    if (context && context.length > 0)
	    {
	        var checkOutObj = Ektron_Ecommerce_ProductClass.GetObject(context, false);
	    }
	    var statusCode = "";
	    var statusText = "";
	    if (args && args.status && args.status.toString().length > 0)
	    {
	        statusCode = "Status: " + args.status.toString();
	    }
	    if (args && args.statusText && args.statusText.length > 0)
	    {
	        statusText = " " + args.statusText;
	    }
	    var errorDetails = "";
	    if ((statusCode.length > 0) || (statusText.length > 0))
	    {
	        errorDetails = " Details: " + statusCode + ((statusCode.length > 0) ? ", " : "") + statusText;
	    }
	    Ektron_Ecommerce_ProductClass.LogIt('Error Occurred in Ektron_Ecommerce_ProductClass: Ajax-Error!' + errorDetails);
	};

    ///////////////////////////////////
    //
	this.Replace = function(str, findstr, replacestr)
	{
	    var ret = str;
	    if (ret != '')
	    {
	        var index = ret.indexOf(findstr);
	        while (index >= 0)
	        {
	            ret = ret.replace(findstr, replacestr);
	            index = ret.indexOf(findstr);
	        }
	    }
	    return (ret);
	};

    ///////////////////////////////////
    // This prevents the browser from caching/auto-completing fields (which would overwrite ajax callback values on page refresh).
	this.DisableInputAutocomplete = function()
	{
	    $ektron("#EktronProductCtl_productContainer_" + this.GetId() + " input[type='text']").attr("autocomplete", "off");
	};

    ///////////////////////////////////
    //
	this.InitializeUI = function()
	{
	    this._readyFlag = true;
	    this.DisableInputAutocomplete();
	    this.UpdatePrice();
	};

    ///////////////////////////////////
    //
	this.ShowAjaxWorking = function()
	{
	    // handled elsewhere:
	    //    var containerObj = document.getElementById("_ResultsContainer_" + this.GetId());
	    //    if (containerObj){
	    //        containerObj.innerHTML = '<span class="_ResultLoading" >  <img src="' + this._ajaxImage + '" />  </span> ';
	    //        containerObj.style.display = "block";
	    //    }
	};

	this.HideMutableShowBusy = function()
	{
	    // hide mutable controls and show busy image:
	    $ektron("#EktronProduct_" + this.GetId() + " .EktronProduct_MutableControlContainer").children().hide();
	    $ektron("#EktronProduct_" + this.GetId() + " .EktronProduct_AjaxBusyImageContainer").show();
	};

	this.ShowMutableClearBusy = function()
	{
	    // hide mutable controls and show busy image:
	    $ektron("#EktronProduct_" + this.GetId() + " .EktronProduct_MutableControlContainer").children().show();
	    $ektron("#EktronProduct_" + this.GetId() + " .EktronProduct_AjaxBusyImageContainer").hide();
	};

	this.AddKitToBasket = function(srcObj)
	{
	    var sItemList = '';
	    var iConfig = '';
	    var hdn = document.getElementById('ekkitconfig' + this.GetId());
	    if (hdn && 'undefined' != typeof hdn)
	    {
	        var iLoop = 0;
	        iConfig = hdn.value;
	        while (document.getElementById('opt_grp_' + this.GetId() + '_' + iLoop + '_id') !== null)
	        {
	            var iGrpId = document.getElementById('opt_grp_' + this.GetId() + '_' + iLoop + '_id').value;
	            var jLoop = 0;
	            while (document.getElementById('opt_grp_' + this.GetId() + '_' + iLoop + '_' + jLoop) !== null)
	            {
	                var iOptId = document.getElementById('opt_grp_' + this.GetId() + '_' + iLoop + '_' + jLoop + '_id').value;
	                if (document.getElementById('opt_grp_' + this.GetId() + '_' + iLoop + '_' + jLoop).checked)
	                {
	                    if (sItemList != '')
	                    {
	                        sItemList += ',' + iGrpId + '-' + iOptId;
	                    }
	                    else
	                    {
	                        sItemList = iGrpId + '-' + iOptId;
	                    }
	                }
	                jLoop = jLoop + 1;
	            }
	            iLoop = iLoop + 1;
	        }
	    }
	    srcObj.href += '?product=' + $ektron(srcObj).attr("data-ektron-baseproductid") + '&config=' + sItemList;
	    return true;
	};

	this.AddVariantToBasket = function(srcObj)
	{
	    var selItems = $ektron('#EktronProductControl_' + this.GetId() + ' .price_modifier:checked');
	    srcObj.href += '?product=' + $ektron(srcObj).attr("data-ektron-baseproductid") + '&variant=' + selItems.attr("data-ektron-variantid");
	    return true;
	};

	this.UpdatePrice = function()
	{
	    var priceTblObj = $ektron("#pricingTable_" + this.GetId());
	    //var priceTblObj = $ektron("#pricingTable_" + this.GetId())[0];
	    if (priceTblObj.length === 0)
	    {
	        // not a kit or variant:
	        $ektron('#ekproductprice' + this.GetId()).text($ektron('#ekproductsaleprice' + this.GetId()).val());
	        return;
	    }

	    var tbl = null;
	    try { tbl = eval(priceTblObj.val()); }
	    catch (e) { tbl = null; }
	    if (tbl == null || tbl.length === 0) {
	        return;
	    }

	    // build index:
	    var arrayIndex = "";
	    var selItems = $ektron('#EktronProductControl_' + this.GetId() + ' .price_modifier:checked');
	    for (var idx = 0; idx < selItems.length; ++idx)
	    {
	        arrayIndex += "[" + selItems.filter("[data-ektron-kitgroupweight=" + (selItems.length - idx - 1).toString() + "]").val() + "]";
	    }

	    var subtotal = eval("tbl" + arrayIndex + ";");
	    if (null === subtotal)
	    {
	        return;
	    }
	    $ektron('#ekproductkitprice' + this.GetId()).text(subtotal);
	    $ektron('#ekproductprice' + this.GetId()).text(subtotal);
	};

    ///////////////////////////////////
    // Call Constructor at creation time:
    this.Constructor();
}  //  end of instance-level Ektron_Ecommerce_ProductClass class definition.
///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////
// Ektron_Ecommerce_ProductClass Static members (variables and constants):
Ektron_Ecommerce_ProductClass.enableShowLog = false;

///////////////////////////////////////////////////////////
// Ektron_Ecommerce_ProductClass Static members (methods):

///////////////////////////////////
//
Ektron_Ecommerce_ProductClass.GetObject = function(id, creatable)
{
    var fullId = "ProductObj_" + id;
    var allowCreation = true;
    if ("undefined" != typeof creatable)
    {
        allowCreation = (true === creatable);
    }
    if (("undefined" != typeof window[fullId]) && (null !== window[fullId]))
    {
        return (window[fullId]);
    }
    else if (allowCreation)
    {
        var obj = new Ektron_Ecommerce_ProductClass(id);
        window[fullId] = obj;
        return (obj);
    }
};

///////////////////////////////////
//
Ektron_Ecommerce_ProductClass.IsReady = function(id)
{
    var obj = Ektron_Ecommerce_ProductClass.GetObject(id, false);
    if (obj)
    {
        return (obj.IsReady());
    }
    else
    {
        return (false);
    }
};

///////////////////////////////////
//
Ektron_Ecommerce_ProductClass.DoAction = function(event, parm2)
{
    var result = false; // cancel click for links.
    if (this.objectId && this.action)
    {
        var obj = Ektron_Ecommerce_ProductClass.GetObject(this.objectId);
        if (obj)
        {
            var evt = event;
            if (!evt)
            {
                evt = window.event;
            }

            switch (this.action)
            {
                default:
                    obj.TestIt(event, parm2);
                    break;
            }
        }
    }
    return (result);
};

///////////////////////////////////
//
Ektron_Ecommerce_ProductClass.LogIt = function(msg)
{
    var textMsg = "Ektron_Ecommerce_ProductClass: " + msg;
    if (Ektron_Ecommerce_ProductClass.enableShowLog)
    {
        var logObj = $ektron("#Product_log");
        if (!logObj || !logObj.length)
        {
            logObj = $ektron("<div class='Product_log' id='Product_log'><h3>Product Debug Message Log</h3></div>");
            $ektron(".EktronProductCtl").append(logObj);
        }
        logObj.append("<div class='Product_log_msg'>" + textMsg + "</div>");
    }
    else
    {
        if (window.console && window.console.log)
        {
            window.console.log(textMsg);
        }
        else if (window.Debug && window.Debug.writeln)
        {
            window.Debug.writeln(textMsg);
        }
    }
};

///////////////////////////////////
//
Ektron_Ecommerce_ProductClass.addLoadEvent = function(func)
{
    var oldonload = window.onload;
    if (typeof window.onload != 'function')
    {
        window.onload = func;
    } else
    {
        window.onload = function()
        {
            if (oldonload)
            {
                oldonload();
            }
            func();
        };
    }
};