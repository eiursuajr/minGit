// Class Ektron_Ecommerce_ProductListClass:
function Ektron_Ecommerce_ProductListClass(id)
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
        var productlistObjId = this.GetId();
        //$ektron(document).load(function(){Ektron_Ecommerce_ProductListClass.GetObject(productlistObjId).Initialize()});
        setTimeout(function() { Ektron_Ecommerce_ProductListClass.GetObject(productlistObjId).Initialize(); }, this._browserInitDelay);
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
                error: function(ektronCallbackResult) { Ektron_Ecommerce_ProductListClass.GetObject(context).AjaxCallback_DisplayError(ektronCallbackResult, context); },
                success: function(ektronCallbackResult)
                {
                    var PACKET_START = "EKTRON_PACKET_START|";
                    var payload = String(ektronCallbackResult);
                    var idx = payload.indexOf(PACKET_START);
                    if (idx >= 0) {
                        var payload = payload.substring(idx + PACKET_START.length);
                    }
                    Ektron_Ecommerce_ProductListClass.GetObject(context).AjaxCallback_DisplayResult(payload, context);
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
            var checkOutObj = Ektron_Ecommerce_ProductListClass.GetObject(context, false);
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
            var checkOutObj = Ektron_Ecommerce_ProductListClass.GetObject(context, false);
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
        Ektron_Ecommerce_ProductListClass.LogIt('Error Occurred in Ektron_Ecommerce_ProductListClass: Ajax-Error!' + errorDetails);
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
        $ektron("#EktronProductListCtl_productlistContainer_" + this.GetId() + " input[type='text']").attr("autocomplete", "off");
    };

    ///////////////////////////////////
    //
    this.InitializeUI = function()
    {
        this._readyFlag = true;
        this.DisableInputAutocomplete();
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

    this.Login = function()
    {
        var emText = $ektron("#EktronProductList_" + this.GetId() + " .EktronProductList_LoginBlock .EktronProductList_LoginEmail input:text")[0].value;
        var codeText = $ektron("#EktronProductList_" + this.GetId() + " .EktronProductList_LoginBlock .EktronProductList_LoginPassword input:password")[0].value;
        var hdnElm = $ektron("#EktronProductList_" + this.GetId() + " #EktronProductList_LoginBlock_Hidden_" + this.GetId());
        if (hdnElm && hdnElm[0] && (emText.length > 0) && (codeText.length > 0))
        {
            hdnElm[0].value = "__opcode=logcustomerin&param__email=" + emText + "&param__code=" + codeText;
            // do postback:
            return (true);
        }
        else
        {
            return (false);
        }
    };

    this.HideMutableShowBusy = function()
    {
        // hide mutable controls and show busy image:
        $ektron("#EktronProductList_" + this.GetId() + " .EktronProductList_MutableControlContainer").children().hide();
        $ektron("#EktronProductList_" + this.GetId() + " .EktronProductList_AjaxBusyImageContainer").show();
    };

    this.ShowMutableClearBusy = function()
    {
        // hide mutable controls and show busy image:
        $ektron("#EktronProductList_" + this.GetId() + " .EktronProductList_MutableControlContainer").children().show();
        $ektron("#EktronProductList_" + this.GetId() + " .EktronProductList_AjaxBusyImageContainer").hide();
    };

    this.GetPage = function(pageId)
    {
        this.HideMutableShowBusy();
        var parms_filterSelected = "";
        if (this._filterSelected.length > 0)
        {
            parms_filterSelected = "&param__filterid=" + this._filterSelected;
        }
        this.DoAjaxCallback("__opcode=getpage&param__pageid=" + pageId + parms_filterSelected, "");
        return (true);
    };

    this.FilterResults = function(filterId)
    {
        if (filterId > 0)
        {
            this.HideMutableShowBusy();
            this._filterSelected = filterId.toString();
            this.DoAjaxCallback("__opcode=filterby&param__filterid=" + filterId, "");
            return (true);
        }
    };

    this.FilterResultsSelect = function(elmObject)
    {
        if (elmObject && "undefined" != typeof elmObject.value)
        {
            this.HideMutableShowBusy();
            this._filterSelected = elmObject.value;
            this.DoAjaxCallback("__opcode=filterby&param__filterid=" + elmObject.value, "");
            return (true);
        }
    };

    this.NextPage = function(pageId)
    {
        this.HideMutableShowBusy();
        var parms_filterSelected = "";
        if (this._filterSelected.length > 0)
        {
            parms_filterSelected = "&param__filterid=" + this._filterSelected;
        }
        this.DoAjaxCallback("__opcode=nextpage&param__pageid=" + pageId + parms_filterSelected, "");
        return (true);
    };

    this.PreviousPage = function(pageId)
    {
        this.HideMutableShowBusy();
        var parms_filterSelected = "";
        if (this._filterSelected.length > 0)
        {
            parms_filterSelected = "&param__filterid=" + this._filterSelected;
        }
        this.DoAjaxCallback("__opcode=previouspage&param__pageid=" + pageId + parms_filterSelected, "");
        return (true);
    };

    ///////////////////////////////////
    // Call Constructor at creation time:
    this.Constructor();
} //  end of instance-level Ektron_Ecommerce_ProductListClass class definition.
///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////
// Ektron_Ecommerce_ProductListClass Static members (variables and constants):
Ektron_Ecommerce_ProductListClass.enableShowLog = false;

///////////////////////////////////////////////////////////
// Ektron_Ecommerce_ProductListClass Static members (methods):

///////////////////////////////////
//
Ektron_Ecommerce_ProductListClass.GetObject = function(id, creatable)
{
    var fullId = "ProductListObj_" + id;
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
        var obj = new Ektron_Ecommerce_ProductListClass(id);
        window[fullId] = obj;
        return (obj);
    }
};

///////////////////////////////////
//
Ektron_Ecommerce_ProductListClass.IsReady = function(id)
{
    var obj = Ektron_Ecommerce_ProductListClass.GetObject(id, false);
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
Ektron_Ecommerce_ProductListClass.DoAction = function(event, parm2)
{
    var result = false; // cancel click for links.
    if (this.objectId && this.action)
    {
        var obj = Ektron_Ecommerce_ProductListClass.GetObject(this.objectId);
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
Ektron_Ecommerce_ProductListClass.LogIt = function(msg)
{
    var textMsg = "Ektron_Ecommerce_ProductListClass: " + msg;
    if (Ektron_Ecommerce_ProductListClass.enableShowLog)
    {
        var logObj = $ektron("#ProductList_log");
        if (!logObj || !logObj.length)
        {
            logObj = $ektron("<div class='ProductList_log' id='ProductList_log'><h3>ProductList Debug Message Log</h3></div>");
            $ektron(".EktronProductListCtl").append(logObj);
        }
        logObj.append("<div class='ProductList_log_msg'>" + textMsg + "</div>");
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

// predefine the paging handlers we'll call when loading a new page, 
// just in case someone doesn't want them defined in their XSL
function EktronProductList_initialize() 
{
}

function EktronProductList_alignImages()
{
}

///////////////////////////////////
//
Ektron_Ecommerce_ProductListClass.addLoadEvent = function(func)
{
    var oldonload = window.onload;
    if (typeof window.onload != 'function')
    {
        window.onload = func;
    }
    else
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