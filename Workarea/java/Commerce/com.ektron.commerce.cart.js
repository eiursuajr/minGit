// Class CartClass:
function CartClass (id) {
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
    this._currentCartName = "";
    
    ///////////////////////////////////////////////////////
    // Instance members - methods:


    ///////////////////////////////////
    // Initialize: Called when this object is first created.
    this.Constructor = function()
    {
        // Initialize state here:
        // ...
        var cartObjId = this.GetId();
        //$ektron(document).load(function(){CartClass.GetObject(cartObjId).Initialize()});
        setTimeout(function() { CartClass.GetObject(cartObjId).Initialize(); }, this._browserInitDelay);
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
    this.GetCurrentCartName = function()
    {
        return (this._currentCartName);
    };
    
    ///////////////////////////////////
    //
    this.SetCurrentCartName = function(name)
    {
        this._currentCartName = name;
    };
    
    ///////////////////////////////////
    //
    this.EmptyCart = function(flag)
    {
        if (flag)
        {
            $ektron("#EktronCartCtl_" + this.GetId() + " .checkoutCartButton").hide();
            $ektron("#EktronCartCtl_" + this.GetId() + " .deleteCartButton").hide();
            $ektron("#EktronCartCtl_" + this.GetId() + " .updateCartButton").hide();
            $ektron("#EktronCartCtl_" + this.GetId() + " .applyCartButton").hide();
        }
        else
        {
            $ektron("#EktronCartCtl_" + this.GetId() + " .checkoutCartButton").show();
            $ektron("#EktronCartCtl_" + this.GetId() + " .deleteCartButton").show();
            $ektron("#EktronCartCtl_" + this.GetId() + " .updateCartButton").show();
            $ektron("#EktronCartCtl_" + this.GetId() + " .applyCartButton").show();
        }
    };
    
    ///////////////////////////////////
    //
    this.DoAjaxCallback = function(args, context, page)
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
            ektronPostParams += "&" + $("#" + this.GetId().toString()).find("input,textarea,select,hidden").serialize();

            context = this.GetId().toString();
            $ektron.ajax({
                type: "POST",
                url: String(window.location),
                data: ektronPostParams,
                dataType: "html",
                success: function(ektronCallbackResult)
                {
                    var PACKET_START = "EKTRON_PACKET_START|";
                    var payload = String(ektronCallbackResult);
                    var idx = payload.indexOf(PACKET_START);
                    if (idx >=0) {
                        var payload = payload.substring(idx + PACKET_START.length);
                    }
                    CartClass.GetObject(context).AjaxCallback_DisplayResult(payload, context);
                }
            });
        }

        return (false); // cancel events.
    };
	
	///////////////////////////////////
    //
    this.AjaxCallback_DisplayResult = function(args, context)
    {
        return this.ProcessCallbackResult(args);
    };

	///////////////////////////////////
    //
    this.ProcessCallbackResult = function(args)
    {
        if (args && args.length >= 9)
        {
            try
            {
                var payloadType = args.substr(0, 1);
                var targetSelectorLength = parseInt(args.substr(1, 6), 10);
                var payloadLength = parseInt(args.substr(7, 6), 10);
                var nextBlock = 1 + 6 + 6 + targetSelectorLength + payloadLength;

                switch (payloadType)
                {
                    case "m":
                        if ("undefined" != typeof $ektron)
                        {
                            var selector = args.substr(13, targetSelectorLength);
                            var markup = args.substr(13 + targetSelectorLength, payloadLength);
                            if (selector && selector.length && markup && markup.length)
                            {
                                $ektron(selector).html(markup);
                            }
                        }
                        break;

                    case "j":
                        var json = args.substr(13 + targetSelectorLength, payloadLength);
                        if (json && json.length)
                        {
                            eval(json);
                        }
                        break;

                    default:
                        break; // unknown type.
                }
                this.ProcessCallbackResult(args.substr(nextBlock));
            }
            catch (err)
            {
                CartClass.LogIt('Error in ProcessCallbackResult: ' + ((err && err.description) ? err.description : "unknown error"));
            }
        }
    };

	///////////////////////////////////
    //
    this.AjaxCallback_DisplayError = function()
    {
        this.UnBlockCurrentItems();
        CartClass.LogIt('Error Occurred in CartClass: Ajax-Error!');
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
    // This prevents the browser from cahching/auto-completing the quatity fields (which would overwrite ajax callback values on page refresh).
    this.DisableInputAutocomplete = function()
    {
        $ektron("#EktronCartCtl_cartContainer_" + this.GetId() + " input[type='text']").attr("autocomplete", "off");
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
        var containerObj = document.getElementById("_ResultsContainer_" + this.GetId());
        if (containerObj)
        {
            containerObj.innerHTML = '<span class="_ResultLoading" >  <img src="' + this._ajaxImage + '" />  </span> ';
            containerObj.style.display = "block";
        }
    };
    
    ///////////////////////////////////
    //
    this.GetContainerElement = function()
    {
        var containerId = "CartCtl_" + this.GetId();
        return (document.getElementById(containerId));
    };
    
    ///////////////////////////////////
    //
    this.IsValid = function(obj)
    {
        return (("undefined" != typeof obj) && (null !== obj));
    };

    ///////////////////////////////////
    //
    this.GetLocalizedString = function(key)
    {
        var result = $ektron("#EktronCartCtl_" + this.GetId() + "_" + key).val();
        return (result !== null ? result : "");
    };

    ///////////////////////////////////
    //
    this.RemoveAllCartItems = function()
    {
        if (confirm(this.GetLocalizedString("emptyActiveCartQuery")))
        {
            this.DoAjaxCallback("__opcode=removeall_cart", "", "");
        }
        return (false);
    };

    ///////////////////////////////////
    //
    this.UpdateCart = function()
    {
        var args = "__opcode=update_cart";
        var rows = $ektron("#EktronCartCtl_" + this.GetId() + " table.cartTable tr.rowSku");
        if (rows && rows.length)
        {
            var prodId = "";
            var prodQty = "";
            var itmId = "";
            var obj = null;
            var el = null;

            for (var idx = 0; idx < rows.length; idx++)
            {
                itmId += "&__itemid=" + rows.find(" .itemIdText").eq(idx).val();
                prodId += "&__prodid=" + rows.find(" .productIdText").eq(idx).val();
                prodQty += "&__prodqty=" + this.FilterNonNumeric(rows.find(" .productQtyText").eq(idx).val());
            }
            args += itmId + prodId + prodQty;
            this.DoAjaxCallback(args, "", "");
        }
        return (false);
    };
    
    ///////////////////////////////////
    //
    this.RemoveCartItem = function(itemId, productId)
    {
        var args = "__opcode=remove_cart_item";
        args += "&__itemid=" + itemId + "&__prodid=" + productId;
        this.DoAjaxCallback(args, "", "");
        return (false);
    };

    ///////////////////////////////////
    //
    this.RenameCart = function()
    {
        $ektron("#EktronCartCtl_" + this.GetId() + " [name='RenameCartUI']").show();
        $ektron("#EktronCartCtl_" + this.GetId() + " [name='RenameCartButton']").hide();
        return (false);
    };
    
    ///////////////////////////////////
    //
    this.RenameCartOk = function()
    {
        var name = this.FilterName($ektron("#EktronCartCtl_" + this.GetId() + " [name='RenameCartField']")[0].value);
        if (name.length > 0)
        {
            this.SetCurrentCartName(name);
            this.DoAjaxCallback("__opcode=rename_cart&__newname=" + name, "", "");
        }
        return (false);
    };

    ///////////////////////////////////
    //
    this.RenameCartCancel = function(name)
    {
        $ektron("#EktronCartCtl_" + this.GetId() + " [name='RenameCartUI']").hide();
        $ektron("#EktronCartCtl_" + this.GetId() + " [name='RenameCartButton']").show();
        return (false);
    };

    ///////////////////////////////////
    //
    this.CreateCart = function()
    {
        $ektron("#EktronCartCtl_" + this.GetId() + " [name='CreateCartUI']").show();
        $ektron("#EktronCartCtl_" + this.GetId() + " [name='CreateCartButton']").hide();
        return (false);
    };

    ///////////////////////////////////
    //
    this.CreateCartOk = function()
    {
        var name = this.FilterName($ektron("#EktronCartCtl_" + this.GetId() + " [name='CreateCartField']")[0].value);
        if (name.length > 0)
        {
            this.SetCurrentCartName(name);
            this.DoAjaxCallback("__opcode=create_cart&__name=" + name, "", "");
        }
        return (false);
    };

    ///////////////////////////////////
    //
    this.CreateCartCancel = function(name)
    {
        $ektron("#EktronCartCtl_" + this.GetId() + " [name='CreateCartUI']").hide();
        $ektron("#EktronCartCtl_" + this.GetId() + " [name='CreateCartButton']").show();
        return (false);
    };

    ///////////////////////////////////
    //
    this.ApplyCoupon = function()
    {
        $ektron("#EktronCartCtl_" + this.GetId() + " [name='ApplyCouponUI']").show();
        return (false);
    };

    ///////////////////////////////////
    //
    this.ApplyCouponOk = function()
    {
        var name = this.FilterName($ektron("#EktronCartCtl_" + this.GetId() + " [name='ApplyCouponField']")[0].value);
        if (name.length > 0)
        {
            this.DoAjaxCallback("__opcode=apply_coupon&__code=" + name, "", "");
        }
        return (false);
    };

    ///////////////////////////////////
    //
    this.ApplyCouponCancel = function(name)
    {
        $ektron("#EktronCartCtl_" + this.GetId() + " [name='ApplyCouponUI']").hide();
        $ektron("#EktronCartCtl_" + this.GetId() + " [name='ApplyCouponButton']").show();
        return (false);
    };

    ///////////////////////////////////
    //
    this.RemoveCoupon = function(couponCode)
    {
        if (couponCode.length > 0)
        {
            this.DoAjaxCallback("__opcode=remove_coupon&__code=" + couponCode, "", "");
        }
        return (false);
    };

    ///////////////////////////////////
    //
    this.ShowCartButtons = function(flag)
    {
        if (flag)
        {
            $ektron("#EktronCartCtl_" + this.GetId() + " .cartButtons").show();
        }
        else
        {
            $ektron("#EktronCartCtl_" + this.GetId() + " .cartButtons").hide();
        }
        return (false);
    };
    
    ///////////////////////////////////
    //
    this.SelectCart = function(id)
    {
        if (!this.IsBlocking())
        {
            this.DoAjaxCallback("__opcode=select_cart&__cartid=" + id, "", "");
        }
        return (false);
    };

    ///////////////////////////////////
    //
    this.DeletSavedCart = function(id)
    {
        if (!this.IsBlocking())
        {
            this.DoAjaxCallback("__opcode=delete_saved_cart&__cartid=" + id, "", "");
        }
        return (false);
    };

    ///////////////////////////////////
    //
    this.BlockItems = function(items)
    {
        var self = this;
        if (items && (items.ektron || items.jquery) && $ektron(this).block)
        {
            items.each(function(index)
            {
                $ektron(this).block();
                self._blockedItems.push($ektron(this));
            });
        }
    };

    ///////////////////////////////////
    //
    this.UnBlockCurrentItems = function()
    {
        var obj = null;
        while (this._blockedItems && this._blockedItems.length)
        {
            obj = this._blockedItems.pop();
            if (obj && (obj.ektron || obj.jquery))
            {
                obj.unblock();
            }
        }
    };

    ///////////////////////////////////
    //
    this.IsBlocking = function()
    {
        return (0 < this._blockedItems.length);
    };
    
    ///////////////////////////////////
    //
    this.RefeshSavedCarts = function()
    {
        this.DoAjaxCallback("__opcode=refresh_saved_carts", "", "");
        return (false);
    };
    
	///////////////////////////////////
	// Usage: Call onkeypress='FilterCartNameKeyStrokes(event)' will 
	// only allow specific characters to be entered into input field.
	// Note: For editing purposes, does allow control codes (e.g. backspace, 
	// left-arrow, delete, etc.) to be passed through for editing:
    this.FilterCartNameKeyStrokes = function(eventObj)
    {
        if (eventObj)
        {
            var charKey = 0;
            var ctrl = 0;
            var ctrlKey = false;

            // IE doesn't pass control-codes, only standard keys (seems to handle internally):
            if ("undefined" == typeof eventObj.charCode && "undefined" != typeof eventObj.keyCode)
            {
                charKey = eventObj.keyCode;
            }

            // FireFox passes normal chars in charCode, control chars in keyCode:
            if ("undefined" != typeof eventObj.charCode && "undefined" != typeof eventObj.keyCode)
            {
                ctrl = eventObj.keyCode;
                charKey = eventObj.charCode;
            }

            if ("undefined" != typeof eventObj.ctrlKey)
            {
                ctrlKey = eventObj.ctrlKey;
            }

            if ((charKey > 47 && charKey < 58) // numbers
                || (charKey >= 97 && charKey <= 122) // letters 'a' thru 'z'
                || (charKey >= 65 && charKey <= 90) // letters 'A' thru 'Z'
                || (charKey == 95) // other allowed naming characters: '_'
                || (charKey == 32) // space
                || (ctrlKey && 118 == charKey) // <ctrl>-v (paste)
                || (8 == ctrl) // backspace
                || (46 == ctrl) // delete
                || (9 == ctrl) // tab
                || (ctrl > 36 && ctrl < 41)) // arrow-keys
            {
                return (true);
            } // accept keypress (pass on to control).
        }
        return (false); // eat keypress.
    };

	///////////////////////////////////
	// removes unwanted characters.
    this.FilterName = function(name)
    {
        var result = "";
        for (var idx = 0; idx < name.length; idx++)
        {
            if (name.substr(idx, 1) != "'"
	            && name.substr(idx, 1) != "`"
	            && name.substr(idx, 1) != "\""
	            && name.substr(idx, 1) != "\\"
	            && name.substr(idx, 1) != "/")
            {
                result += name.substr(idx, 1);
            }
        }
        return (result);
    };
    
	///////////////////////////////////
	// removes all non-numeric characters.
    this.FilterNonNumeric = function(inputText)
    {
        var result = inputText + "";
        result = result.replace(/[^0-9]/g, "");
        return result;
    };
    
	///////////////////////////////////
	// Usage: Call onkeypress='FilterCouponNameKeyStrokes(event)' will 
	// only allow specific characters to be entered into input field.
	// Note: For editing purposes, does allow control codes (e.g. backspace, 
	// left-arrow, delete, etc.) to be passed through for editing:
    this.FilterCouponNameKeyStrokes = function(eventObj)
    {
        if (eventObj)
        {
            var charKey = 0;
            var ctrl = 0;
            var ctrlKey = false;

            // IE doesn't pass control-codes, only standard keys (seems to handle internally):
            if ("undefined" == typeof eventObj.charCode
                && "undefined" != typeof eventObj.keyCode)
            { charKey = eventObj.keyCode; }

            // FireFox passes normal chars in charCode, control chars in keyCode:
            if ("undefined" != typeof eventObj.charCode
                && "undefined" != typeof eventObj.keyCode)
            {
                ctrl = eventObj.keyCode;
                charKey = eventObj.charCode;
            }

            if ("undefined" != typeof eventObj.ctrlKey)
            {
                ctrlKey = eventObj.ctrlKey;
            }

            // allow any key accept single and double quotes:
            if ((charKey != 34)
                && (charKey != 39)
                && (charKey != 43)
                && (charKey != 38)
                && (charKey != 47)
                && (charKey != 96))
            { return (true); } // accept keypress (pass on to control).
        }
        return (false); // eat keypress.
    };
    

    ///////////////////////////////////
    // Call Constructor at creation time:
    this.Constructor();
} //  end of instance-level CartClass class definition.
///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////
// CartClass Static members (variables and constants):
CartClass.enableShowLog = false;

///////////////////////////////////////////////////////////
// CartClass Static members (methods):

///////////////////////////////////
//
CartClass.GetObject = function(id, creatable)
{
    var fullId = "CartObj_" + id;
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
        var obj = new CartClass(id);
        window[fullId] = obj;
        return (obj);
    }
};

///////////////////////////////////
//
CartClass.IsReady = function(id)
{
    var obj = CartClass.GetObject(id, false);
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
CartClass.DoAction = function(event, parm2)
{
    var result = false; // cancel click for links.
    if (this.objectId && this.action)
    {
        var obj = CartClass.GetObject(this.objectId);
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
CartClass.LogIt = function(msg)
{
    var textMsg = "CartClass: " + msg;
    if (CartClass.enableShowLog)
    {
        var logObj = $ektron("#Cart_log");
        if (!logObj || !logObj.length)
        {
            logObj = $ektron("<div class='Cart_log' id='Cart_log'><h3>Cart Debug Message Log</h3></div>");
            $ektron(".EktronCartCtl").append(logObj);
        }
        logObj.append("<div class='Cart_log_msg'>" + textMsg + "</div>");
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
CartClass.addLoadEvent = function(func)
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
