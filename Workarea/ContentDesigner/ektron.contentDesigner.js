//Ajax call to load ContentDesigner in context for Ektron CMS Content editing.

$ektron.fn.contentDesigner = function(uniqueCallbackId, data)
{
    try
    {
        if (this.length > 1) throw new RangeError("Only one element may be selected to edit in context.");

        // Show/Hide Loading graphics
        var me = this;
        var newId = "EktronAjaxLoading_cloned";
        this.ajaxStart(function()
        {
            var menuZIndex = 0;
            $ektron(".EktronEditorsMenu").each(function()
            {
                var thisZIndex = parseInt($ektron(this).css("z-index"), 10);
                if (thisZIndex > menuZIndex)
                {
                    menuZIndex = thisZIndex;
                }
            });

            $ektron("#" + newId).remove();
            
            var oImg = me.children(".EktronAjaxLoading img");
            var sTop = ($ektron(window).height() / 2) - (oImg.height() / 2) + $ektron(window).scrollTop() + "px";
            var sLeft = ($ektron(window).width() / 2) - (oImg.width() / 2) + $ektron(window).scrollLeft() + "px";
            me.children(".EktronAjaxLoading")
				.clone()
				.attr("id", newId)
				.css(
				{
					position: "absolute",
					top: sTop,
					left: sLeft,
					"z-index": menuZIndex + 10
                }).prependTo("body");

            $ektron("div.ektronModalOverlay").css("opacity", 0);  
            $ektron("#" + newId).modal().modalHide().modalShow();  
        }); 
        this.ajaxStop(function()
        {
            $ektron("#" + newId).modal().modalHide();
            $ektron("#" + newId).remove();
        }); 
        
        var editorId; // undefined
        var callbackData = "object" == typeof data ? data  
						: typeof data != "undefined" ? { data: data } 
						: null;
        var bInit = true;
        this.ajaxCallback(
        {
	        uniqueId: uniqueCallbackId,
	        data: $ektron.extend(
	        { 
		        command: "edit", 
		        width: this.width(), 
		        height: this.height()
	        },  callbackData),
	        success: function(data)
	        {
		        var aryMatch = data.match(/id=\"(\w+)_wrapper\"/);
		        if (aryMatch && aryMatch.length > 1)
		        {
			        editorId = aryMatch[1];
		        }
	        },
	        complete: function()
	        {
				if (typeof GetRadEditor != "function") throw new Error("GetRadEditor function not found. Editor ID=" + editorId);
		        var editor = GetRadEditor(editorId);
		        if (!editor) throw new Error("Editor not found. ID=" + editorId);
		        editor.uniqueCallbackId = uniqueCallbackId;
		        editor.callbackData = callbackData;
				// Next, Ektron.ContentDesignerInContext.OnClientLoad will be called. It is assigned to the 
				// OnClientLoad property of the ContentDesigner server control.
	        },
			onexception: Ektron.ContentDesignerInContext.onexception
        });
    }
    catch (ex)
    {
        Ektron.OnException(Ektron.ContentDesignerInContext, null, ex, arguments);
    }
    return this;
};

Ektron.ContentDesignerInContext = 
{
	OnClientLoad : function(editor)
	{
	    var m_old_onerror; // undefined 
		try
		{
			Ektron.ContentDesigner.setActionOnUnload("EDITOR_ONUNLOAD_PROMPT");
			m_old_onerror = window.onerror;	   //#44567
			window.onerror = function ContentDesignerInContext_onerror(event)
			{
			    try
			    {
			        if (m_old_onerror)
			        {
			            try
			            {
			                var result = m_old_onerror.apply(this, arguments);
			            }
			            catch (ex)
			            {
			                Ektron.OnException(Ektron.ContentDesignerInContext, null, ex, arguments, m_old_onerror);
			            }
			        }
			        return true;
			    }
			    catch (ex)
			    {
			        Ektron.OnException(Ektron.ContentDesignerInContext, null, ex, arguments);
			    }
			};
			editor.activateToolbar();
            editor.SetFocus();
		} 
		catch (ex)
		{
			Ektron.OnException(Ektron.ContentDesignerInContext, null, ex, arguments);
		}
		editor.autoheight();
	},	
		
	onexception : Ektron.OnException.diagException
};
