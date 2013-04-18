// Copyright 2007-2009 Ektron, Inc.

// NOTE: Subclassing the RadEditor JavaScript class does not work b/c the class must be defined prior to the subclass.
// This means the .js file would need to be included, then the Ektron .js file, then the object would be created.
// The way the telerik files are includes and the RadEditor object created by RadEditorInitialize makes subclassing
// impossible. The prototype methods are defined otherwise. Besides, subclassing, the standard JavaScript way, requires
// creating an object by calling the constructor without arguments, which seems less than elegant.

// ContentDesigner singleton
if (typeof Ektron.ContentDesigner != "object") Ektron.ContentDesigner = {};
if ("undefined" == typeof Ektron.ContentDesigner.instances) $ektron.extend(Ektron.ContentDesigner, new (function()
{
	this.instances = [];
	this.actionOnUnload = "EDITOR_ONUNLOAD_SAVE"; 

	this.add = function ContentDesigner_add(editor)
	{
		if (!editor) return;
		try
		{
		    this.remove(editor); // prevent duplicates
			if (0 == this.instances.length)
			{
                $ektron(window.document).keydown(m_onkeydown);
                var oWin = window;
				try
		        {
				    while (oWin != top)
				    {
			            oWin = oWin.parent;
			            $ektron(oWin.document).keydown(m_onkeydown);
			        }
			    }
				catch (ex)
			    {}
				
				m_old_onbeforeunload = window.onbeforeunload;
				window.onbeforeunload = function ContentDesigner_onbeforeunload(event)
				{
					var msg = "";
					try
					{
						event = event || window.event;  
					    
						if (m_old_onbeforeunload)
						{
							try
							{
								var result = m_old_onbeforeunload.apply(this, arguments);
								if ("string" == typeof result)
								{
									msg = result;
								}
								else if ("string" == typeof event.returnValue)
								{
									msg = event.returnValue;
								}
							}
							catch (ex)
							{
								Ektron.OnException(Ektron.ContentDesigner, null, ex, arguments, m_old_onbeforeunload);
							}
						}
						
						var sAction = Ektron.ContentDesigner.getActionOnUnload();
						if ("EDITOR_ONUNLOAD_SAVE" == sAction)
						{
						    RadEditorNamespace.SaveAllEditors(false); // save content to their hidden field(s) without validation.
						}
						else if ("EDITOR_ONUNLOAD_PROMPT" == sAction)
						{
						    var locMsg; // undefined
						    for (var i = 0; i < Ektron.ContentDesigner.instances.length; i++)
						    {
							    var objInstance = Ektron.ContentDesigner.instances[i];
								locMsg = objInstance.GetLocalizedString("ContentCheckedOut", "The content is currently checked out.");
							    break;
						    }
						    if (locMsg.length > 0)
						    {
						        if (msg.length > 0)
						        {
							        msg += "\n";
						        }
						        msg += locMsg;
						    }
						    event.returnValue = msg; // IE
                        }
					}
					catch (ex)
					{
						Ektron.OnException(Ektron.ContentDesigner, null, ex, arguments);
					}
					if (msg.length > 0)
					{
						return msg;
					}
				};
			}
			this.instances[this.instances.length] = editor;
			this.instances[editor.Id] = editor;
		}
		catch (ex)
		{
			Ektron.OnException(this, null, ex, arguments);
		}
	};
	
	this.remove = function ContentDesigner_remove(editor)
	{
		if (!editor) return;
		try
		{
			for (var i = 0; i < this.instances.length; i++)
			{
				if (editor == this.instances[i])
				{
					this.instances.splice(i, 1);
					if (0 == this.instances.length)
			        {
                        $ektron(window.document).unbind("keydown", m_onkeydown);
                        var oWin = window;
				        try
		                {
				            while (oWin != top)
				            {
			                    oWin = oWin.parent;
			                    $ektron(oWin.document).unbind("keydown", m_onkeydown);
			                }
			            }
				        catch (ex)
			            {}
				        
				        // cannot use $ektron.unbind 
				        window.onbeforeunload = m_old_onbeforeunload;
				        m_old_onbeforeunload = null;
			        }
					break;
				}
			}
			this.instances[editor.Id] = null;
			delete this.instances[editor.Id];
		}
		catch (ex)
		{
			Ektron.OnException(this, null, ex, arguments);
		}
	};
	
	this.setActionOnUnload = function ContentDesigner_setActionOnUnload(sAction)
	{
	    sAction = (sAction+"").toUpperCase();
	    switch(sAction)
	    {
	        case "EDITOR_ONUNLOAD_PROMPT":
	        case "EDITOR_ONUNLOAD_SAVE":
	        case "EDITOR_ONUNLOAD_NOSAVE":
	            this.actionOnUnload = sAction;
	            break;
	        default:
	            this.actionOnUnload = "EDITOR_ONUNLOAD_SAVE";
	            Ektron.ContentDesigner.onexception(new RangeError("Unknown action: " + sAction), arguments);
	            break;
	    }
	};
	
	this.getActionOnUnload = function ContentDesigner_getActionOnUnload()
	{	    
	    return this.actionOnUnload;
	};
	
	var m_old_onbeforeunload; // undefined
	
	function m_onkeydown(event)
	{
		try
		{
			//alert("ekradeditor.js:" + event.keyCode + ":" + event.shiftKey + ":" + event.ctrlKey + ":" + event.altKey);
			// in IE, it absorbs the backspace key and onbeforeUnload is not fired. 
			// in FF, onbeforeUnload is fired before this "false" is returned.
			if (/* BACKSPACE_KEY */ 8 == event.keyCode && !event.shiftKey && !event.ctrlKey && !event.altKey && 
					!$ektron.isEditableElement(event.target))
			{
				return false; // cancel action
			}
		}
		catch (ex)
		{
			Ektron.OnException(Ektron.ContentDesigner, null, ex, arguments);
		}
	}
})() ); // Ektron.ContentDesigner singleton

Ektron.ContentDesigner.onexception = function(ex, args, callee)
{
	if (document.cookie && document.cookie.indexOf("ContentDesigner.onexception=") > -1)
	{
		Ektron.OnException.alertException(ex, args, callee);
	}
	if (typeof console != "undefined" && console && console.error && document.cookie && document.cookie.indexOf("ContentDesigner.console=") > -1)
	{
		Ektron.OnException.consoleException(ex, args, callee);
	}
};

Ektron.ContentDesigner.trace = function()
{
    try
    {
	    if (typeof console != "undefined" && console && console.log && console.log.apply && document.cookie && document.cookie.indexOf("ContentDesigner.console=") > -1)
	    {
		    console.log.apply(console, arguments);
	    }
	}
	catch (ex) {}
};

Ektron.ContentDesigner.onContentWindowChange = function(win, targetElement, context)
{
	try
	{
		//Ektron.ContentDesigner.trace("onContentWindowChange");
		if (win && context && context.editorId)
		{
			var theEditor = Ektron.ContentDesigner.instances[context.editorId];
			theEditor.ContentWindow = win;
			theEditor.Document = theEditor.ContentWindow.document;
			theEditor.ContentArea = theEditor.Document.body;

			if (theEditor.ContentAreaEventHandlers)
			{
				var srcElement = theEditor.IsIE ? theEditor.Document.body : theEditor.Document;
				for (var evName in theEditor.ContentAreaEventHandlers)
				{
					var eventHandler = theEditor.ContentAreaEventHandlers[evName];
					if ("function" == typeof eventHandler)
					{
						RadEditorNamespace.Utils.AttachEventEx(srcElement, evName, eventHandler);
					}
				}
			}

			if (theEditor.isMozilla)
			{
				RadEditorNamespace.ConfigureMozillaEditMode(theEditor);
			    //targetElement is the Div, not the iframe body
			    if ("dataentry" == theEditor.ekParameters.editMode && $ektron(targetElement).hasClass("design_richarea"))
			    {
			        // related to #40716
			        // reset the Ektron.SmartForm.selectedField in ektron.smartforms.js b/c FF3 cannot get the focus back to richarea.
		            // Ektron.SmartForm.selectedField cannot to null b/c ektron.smartforms.js will return the first editable element 
		            // if that is null.
					theEditor.sfInstance.setSelectedField(targetElement);
			    }
			}
		}
	}
	catch (ex)
	{
		Ektron.OnException(this, null, ex, arguments);
	}
};

Ektron.ContentDesigner.onLoadIFrame = function(win, targetElement, doc, context)
{
	try
	{
		//Ektron.ContentDesigner.trace("onLoadIFrame");
		if ("boolean" == typeof win.parent.ektronContentUsability)
		{
			win.ektronContentUsability = win.parent.ektronContentUsability;
		}
		Ektron.SelectionRange.ensureContentUsability(doc.body);
		
		if ("boolean" == typeof win.parent.ektronHideShowElementState)
		{
			win.ektronHideShowElementState = win.parent.ektronHideShowElementState;
		}
		if ($ektron.browser.mozilla && context.editorId)
        {
            var ctlEditoId = context.editorId.replace(/_ContentDesigner\b/, "");
            $ektron(window).trigger("onReady_" + ctlEditoId);
        }
	}
	catch (ex)
	{
		Ektron.OnException(this, null, ex, arguments);
	}
};
// workaround IE9 events not being bubbled up from the iframe layer. Source can be found at radEditor_EventsManager.js
Ektron.ContentDesigner.onIE9MouseUp = function(event, win, targetElement, context)
{	
	try
	{
		//Ektron.ContentDesigner.trace("onIE9MouseUp");
        if ("undefined" == typeof context.editorId) return;
		var theEditor = Ektron.ContentDesigner.instances[context.editorId];
		theEditor.FireEvent(RadEditorNamespace.RADEVENT_SEL_CHANGED, event);
	}
	catch (ex)
	{
		Ektron.OnException(this, null, ex, arguments);
	}
};
Ektron.ContentDesigner.onIE9MouseDown = function(event, win, targetElement, context)
{	
	try
	{
		//Ektron.ContentDesigner.trace("onIE9MouseDown");
        if ("undefined" == typeof context.editorId) return;
		var theEditor = Ektron.ContentDesigner.instances[context.editorId];
		theEditor.SaveTypedContent(false, "Saving typed content onmousedown");
	}
	catch (ex)
	{
		Ektron.OnException(this, null, ex, arguments);
	}
};
Ektron.ContentDesigner.onIE9KeyUp = function(event, win, targetElement, context)
{	
	try
	{
		//Ektron.ContentDesigner.trace("onIE9KeyUp");
        if ("undefined" == typeof context.editorId) return;
		var theEditor = Ektron.ContentDesigner.instances[context.editorId];
		theEditor.OnKeyUp(event);
	}
	catch (ex)
	{
		Ektron.OnException(this, null, ex, arguments);
	}
};
Ektron.ContentDesigner.onIE9KeyDown = function(event, win, targetElement, context)
{	
	try
	{
		//Ektron.ContentDesigner.trace("onIE9KeyDown");
        if ("undefined" == typeof context.editorId) return;
		var theEditor = Ektron.ContentDesigner.instances[context.editorId];
		theEditor.OnKeyDown(event);
	}
	catch (ex)
	{
		Ektron.OnException(this, null, ex, arguments);
	}
};
Ektron.ContentDesigner.onIE9ContextMenu = function(event, win, targetElement, context)
{	
	try
	{
		//Ektron.ContentDesigner.trace("onIE9ContextMenu");
        if ("undefined" == typeof context.editorId) return;
		var theEditor = Ektron.ContentDesigner.instances[context.editorId];
		theEditor.FireEvent(RadEditorNamespace.RADEVENT_CONTEXTMENU, event);
		if (theEditor.IsIE && true == event.cancelBubble) return false;
	}
	catch (ex)
	{
		Ektron.OnException(this, null, ex, arguments);
	}
};
Ektron.ContentDesigner.onIE9RadEventSelChanged = function(event, win, targetElement, context)
{	
	try
	{
		//Ektron.ContentDesigner.trace("onIE9RadEventSelChanged");
        if ("undefined" == typeof context.editorId) return;
		var theEditor = Ektron.ContentDesigner.instances[context.editorId];
		theEditor.FireEvent(RadEditorNamespace.RADEVENT_SEL_CHANGED, event);
	}
	catch (ex)
	{
		Ektron.OnException(this, null, ex, arguments);
	}
};
Ektron.ContentDesigner.onIE9BeforeEditFocus = function (event, win, targetElement, context)
{
    try
    {
        //Ektron.ContentDesigner.trace("onIE9BeforeEditFocus");
        if ("undefined" == typeof context.editorId) return;
        var theEditor = Ektron.ContentDesigner.instances[context.editorId];
        theEditor.activateToolbar();
    }
    catch (ex)
    {
        Ektron.OnException(this, null, ex, arguments);
    }
};
Ektron.ContentDesigner.onEditFieldButtonClick = function(event, win, targetElement, context)
{	
	try
	{
		//Ektron.ContentDesigner.trace("onEditFieldButtonClick");
		var theEditor = Ektron.ContentDesigner.instances[context.editorId];
		var toolsArray = theEditor.Tools;
		for (var i = 0; i < toolsArray.length; i++)
		{
			var oTool = toolsArray[i];
			if ("EkFieldProp" == oTool.Name)
			{							
				theEditor.Fire("EkFieldProp");
				break;
			}
			else if ("EkHtmlFieldProp" == oTool.Name)	
			{
				theEditor.Fire("EkHtmlFieldProp");
				break;
			}		
		}
	}
	catch (ex)
	{
		Ektron.OnException(this, null, ex, arguments);
	}
};
Ektron.ContentDesigner.onFieldButtonClick = function(event, win, targetElement, context)
{
	try
	{
		//Ektron.ContentDesigner.trace("onFieldButtonClick");
		var theEditor = Ektron.ContentDesigner.instances[context.editorId];
		var oElem = theEditor.SmartForm.getFieldFromFieldButton(targetElement);
		var sClassNames = oElem.className.toLowerCase();
		var bFound = false;
		$ektron.each(sClassNames.split(/\s+/), function(i, className) 
		{
			bFound = true; // be optimistic
			switch (className)
			{
				case "design_resource":
				case "ektdesignns_resource":
					theEditor.sfInstance.setSelectedField(targetElement);
					theEditor.Fire("EkResourceSelectorPopup");
					break;
				case "design_calendar":
				case "ektdesignns_calendar":
					if (Ektron.ContentDesigner && "function" == typeof Ektron.ContentDesigner.PopupCalendar)
					{
						var defaultValue = oElem.getAttribute("datavalue");
						if (null == defaultValue && typeof defaultValue != "undefined")
						{
							// Needed for Opera b/c "value" is interpreted as a number and not a string, eg, "2007-01-09" is read as "2007"
							// In FireFox/Mozilla/Netscape7, the .value attribute is undefined if not standard (e.g., span)
							// and .getAttribute("value") is null when .value is standard (e.g., input).
							defaultValue = oElem.getAttribute("value");
						}
						Ektron.ContentDesigner.PopupCalendar(theEditor, oElem, defaultValue);
					}
					else
					{
						theEditor.sfInstance.setSelectedField(targetElement);
						theEditor.Fire("EkCalendarPopup");
					}
					break;
				case "design_imageonly":
				case "ektdesignns_imageonly":
					if (Ektron.ContentDesigner && "function" == typeof Ektron.ContentDesigner.PopupResource)
					{
						Ektron.ContentDesigner.PopupResource(theEditor, oElem, "image", "");
					}
					else
					{
						theEditor.sfInstance.setSelectedField(targetElement);
						theEditor.Fire("EkImageOnlyPopup");
					}
					break;
				case "design_filelink":
				case "ektdesignns_filelink":
					if (Ektron.ContentDesigner && "function" == typeof Ektron.ContentDesigner.PopupResource)
					{
						Ektron.ContentDesigner.PopupResource(theEditor, oElem, "file", "");
					}
					else
					{
						theEditor.sfInstance.setSelectedField(targetElement);
						theEditor.Fire("EkFileLinkPopup");
					}
					break;
				default:
					bFound = false;
					break;
			}
			if (bFound) return false; // exit loop
		});
		if (!bFound)
		{
			Ektron.ContentDesigner.onexception(new RangeError("Unknown field button type: " + oElem.className), arguments);
		}
	}
	catch (ex)
	{
		Ektron.OnException(this, null, ex, arguments);
	}
};
Ektron.ContentDesigner.onContextMenu = function(event, win, targetElement, context)
{
	try
	{
		var theEditor = Ektron.ContentDesigner.instances[context.editorId];
		theEditor.FireEvent(RadEditorNamespace.RADEVENT_CONTEXTMENU, event);
	}
	catch (ex)
	{
		Ektron.OnException(this, null, ex, arguments);
	}
};

Ektron.ContentDesigner.onAutoheight = function(context)
{
    try
    {
        var theEditor = Ektron.ContentDesigner.instances[context.editorId];
        $ektron(theEditor.ContentAreaElement).autoheight({ bindEvents: false });
    }
    catch (ex)
    {
        Ektron.OnException(this, null, ex, arguments);
    }    
};

Ektron.ContentDesigner.onCreateContentDesigner = function (sfInstance, context)
{//#52965
    try
    {
        var theEditor = Ektron.ContentDesigner.instances[context.editorId];
        theEditor.SmartForm = theEditor.EkScriptWindow.Ektron.SmartForm;
        theEditor.sfInstance = sfInstance;

        var containingElement = theEditor.getContentElement();
        if (containingElement)
        {
            var doc = containingElement.ownerDocument;
            var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
            win.ektronContentUsability = ("true" == theEditor.ekParameters.ShowTemporaryMarkers.toLowerCase());
            if ($ektron.browser.mozilla)
            {
                win.parent.ektronContentUsability = win.ektronContentUsability;
            }
            Ektron.SelectionRange.ContentUsability.localize(theEditor);
            Ektron.SelectionRange.ensureContentUsability(containingElement);

            win.ektronHideShowElementState = false;
            if ($ektron.browser.mozilla)
            {
                win.parent.ektronHideShowElementState = win.ektronHideShowElementState;
            }
            //            if ($ektron.browser.mozilla)
            //            {
            //                $ektron(window).bind("onContentDesignerReady" + theEditor.Id, function()
            //                {
            //                    var ctlEditoId = theEditor.Id.replace(/_ContentDesigner\b/, "");
            //                    $ektron(window).trigger("onReady_" + ctlEditoId);
            //                    $ektron(window).unbind("onContentDesignerReady" + theEditor.Id);
            //                });
            //            }
        }
        if ($ektron.browser.msie)
        {
            $ektron("button.RadEToggleButton").focus(function (e)
            {
                theEditor.SetToolState(theEditor.Tools, null); // don't pass along the event so the toolbar won't think it is in an editable area.
                return RadEditorNamespace.Utils.CancelEvent(e);
            });
        }

        Ektron.ContentDesigner.trace("Created SmartForm instance id=" + context.editorId);

        // check if re-entrancy happened
        if ("string" == typeof theEditor.contentCallbackQueue.newContent)
        {
            theEditor.SetContentAreaHtml(theEditor.contentCallbackQueue.newContent);
            theEditor.contentCallbackQueue.newContent = null;
        }
        while (theEditor.contentCallbackQueue.length > 0)
        {
            theEditor.tryCallback(theEditor.contentCallbackQueue.shift());
        }
        theEditor.contentCallbackQueue = null;
    }
    catch (ex)
    {
        Ektron.OnException(this, null, ex, arguments);
    }
};
// ****************************************************************************

function EkRadEditor()
{
	this.serverUrl = location.protocol + "//" + location.host;
	this.isMozilla = (window.netscape && !window.opera);
	
	this.sfInstance = null;
	this.designContent = "";
	this.isSelectionEditable = EkRadEditor_isSelectionEditable;
	this.getFirstEditableLocationIE = EkRadEditor_getFirstEditableLocationIE;
	this.addFilters = EkRadEditor_addFilters;
	this.InitRadEvents = EkRadEditor_InitRadEvents;
	this.GetDialogUrl = EkRadEditor_GetDialogUrl;
	this.getContentElement = EkRadEditor_getContentElement;
	this.InsertImage = EkRadEditor_InsertImage;
	this.ExecuteBrowserCommand = EkRadEditor_ExecuteBrowserCommand;
	this.ExecuteInsertObjectCommand = EkRadEditor_ExecuteInsertObjectCommand;
    this.removeStyles = EkRadEditor_RemoveStyles;
	this.validateContent = EkRadEditor_validateContent;
	this.validateDesign = EkRadEditor_validateDesign;
	this.validateXmlContent = EkRadEditor_validateXmlContent;
	this.validateContentXPathExpr = EkRadEditor_validateContentXPathExpr;
	this.validateSchemaDesign = EkRadEditor_validateSchemaDesign;
	this.validateAccessibility = EkRadEditor_validateAccessibility;
	this.checkCompatibility = EkRadEditor_checkCompatibility;
    this.OnContentError = EkRadEditor_OnContentError;
    this.autoheight = EkRadEditor_autoheight;
    this.autoheightActive = false;
	this.getToolbarFlavor = EkRadEditor_getToolbarFlavor;
	this.activateToolbar = EkRadEditor_activateToolbar;
	this.destroyToolbar = EkRadEditor_destroyToolbar;
	this.destroyEditor = EkRadEditor_destroyEditor;
	this.onContentPaste = EkRadEditor_onContentPaste;
	this.isWordContent = EkRadEditor_isWordContent;
    this.isWebKitBrowserContent = EkRadEditor_isWebKitBrowserContent;
	this.setDesignContent = EkRadEditor_setDesignContent;
	if (this.IsIE)
	{
		this.SetFocus = EkRadEditor_SetFocusIE;
		this.SetActive = this.SetFocus; //IE ONLY
	}
	if (this.OnBeforePaste)
	{
		this.OnBeforePaste = EkRadEditor_OnBeforePaste;
	}
    this.SetEditable = function(editable) { };
    
	this.setContent = EkRadEditor_setContent;
	this.getContent = EkRadEditor_getContent;
	
	this.SetContentAreaHtml = EkRadEditor_SetContentAreaHtml;
	this.GetHiddenTextareaValue = EkRadEditor_GetHiddenTextareaValue;
	this.private_SetPageHtml = EkRadEditor_private_SetPageHtml;

	//Called by modules, validators, commands
	this.GetHtml = EkRadEditor_GetHtml;
	this.GetText = EkRadEditor_GetText;
	
	this.contentCache = null;
	this.initialContent = null;
	this.initialFieldList = null;
	
	var m_isChanged = false;
	var m_isChangedTimer = null;
	this.isChanged = function() { return this.SmartForm && m_isChanged; };
	this.setIsChanged = function(isChanged) 
	{ 
		var wasChanged = m_isChanged; 
		m_isChanged = isChanged; 
		if (m_isChangedTimer) clearTimeout(m_isChangedTimer);
		if (!m_isChanged)
		{
			m_isChangedTimer = setTimeout(function() { m_isChanged = true; }, 5000);
		}
		return wasChanged; 
	};
	this.setIsChanged(false); // begin timer
	
	this.setStatusMessage = function(message)
	{
		// See RadEditorRenderer.cs function RenderEditorBodyInternal
		$ektron("img.ContentDesignerStatusMessage").attr("alt", message).attr("title", message);
	};
	
	this.tryCallback = function(callback)
	{
		if ("function" == typeof callback)
		{
			try
			{
				callback();
			}
			catch (ex)
			{
				Ektron.OnException(this, Ektron.ContentDesigner.onexception, ex, [], callback);
			}
		}
	};
	
	// Replace telerik's SymbolsArray with one that contains no HTML entity names to be XML friendly.
	this.SymbolsArray = [
	        "&#169;", "&#174;", "&#153;", "&#128;", "&#162;", "&#163;", "&#165;", "&#164;", "&#161;", "&#191;", 
	        "&#149;", "&#183;", "&#133;", "&#150;", "&#151;", "&#173;", "&#175;", "&#134;", "&#135;", "&#172;", 
			"&#130;", "&#132;", "&#145;", "&#146;", "&#147;", "&#148;", "&#139;", "&#155;", "&#171;", "&#187;",
			"&#184;", "&#168;", "&#186;", "&#185;", "&#178;", "&#179;", "&#170;", "&#188;", "&#189;", "&#190;", 
			"&#192;", "&#193;", "&#194;", "&#195;", "&#196;", "&#197;", "&#198;", "&#199;", "&#208;", "&#200;", 
			"&#201;", "&#202;", "&#203;", "&#204;", "&#205;", "&#206;", "&#207;", "&#209;", "&#210;", "&#211;", 
			"&#212;", "&#213;", "&#214;", "&#216;", "&#140;", "&#138;", "&#217;", "&#218;", "&#219;", "&#220;", 
			"&#221;", "&#159;", "&#142;", "&#222;", "&#223;", "&#224;", "&#225;", "&#226;", "&#227;", "&#228;", 
			"&#229;", "&#230;", "&#231;", "&#240;", "&#232;", "&#233;", "&#234;", "&#235;", "&#236;", "&#237;", 
			"&#238;", "&#239;", "&#241;", "&#242;", "&#243;", "&#244;", "&#245;", "&#246;", "&#248;", "&#156;", 
			"&#154;", "&#249;", "&#250;", "&#251;", "&#252;", "&#253;", "&#255;", "&#158;", "&#254;", "&#180;",
			"&#182;", "&#166;", "&#167;", "&#215;", "&#247;", "&#177;", "&#8776;", "&#8800;", "&#8804;", "&#8805;",   
			"&#945;", "&#946;", "&#947;", "&#952;", "&#956;", "&#960;", "&#969;", "&#176;", "&#137;", "&#131;"
			];

	Ektron.ContentDesigner.add(this);
}
EkRadEditor.overrides = Ektron.Class.overrides("RadEditor", [
			"InitRadEvents",			"GetDialogUrl", 
			"SetContentAreaHtml", 		"GetHiddenTextareaValue",	"OnBeforePaste"
			]);

function EkRadEditor_addFilters()
{
	this.filter = new EkContentFilter(this);
	// The managed filter is for general content processing.
	// .filter is for pasting and must always be in "design" mode, not "dataentry".
	this.filter.mode = "design";
	if ("dataentry" == this.ekParameters.editMode)
	{
		this.FiltersManager.AddAt(new EkDataEntryFilter(this, this.dataEntryXslt, this.dataEntryXsltArgs), 0);
	}
	else
	{
		this.FiltersManager.AddAt(new EkContentFilter(this), 0);
	}
}

function EkRadEditor_InitRadEvents()
{
	this.RadEditor_InitRadEvents();
	
	var thisEditor = this;
	this.EkScriptWindow = this.ContentWindow;
	this.EkScriptDocument = this.Document;
    
// #34626: with the following code, FireFox will pop up 2 Data Entry dialogs.  
//	if (this.isMozilla)
//	{
//		this.AttachEventHandler("onclick",function(event)
//		{	
//			event = (event ? event : window.event);
//			var targetElement = (event.target ? event.target : event.srcElement);
//			var doc = ("#document" == targetElement.nodeName ? targetElement : targetElement.ownerDocument);	
//			var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
//			if (win != thisEditor.ContentWindow && Ektron.ContentDesigner && "function" == typeof Ektron.ContentDesigner.onFieldButtonClick) 
//			{
//				Ektron.ContentDesigner.onFieldButtonClick(event, win, targetElement, { editorId: thisEditor.Id });
//			}
//		});
//	}

    // #33731: this attach the onclick and onkeypress event to FF so it will hide the context menu when there is a keypress or a mouse click.
    // These events are attached by Ektron.ContentDesigner.onContentWindowChange 
    if (this.isMozilla)
	{
	    this.AttachEventHandler("onclick", function(event)
		{	
	        HideContextMenu();
	    });
	    if ("dataentry" == thisEditor.ekParameters.editMode)
		{
	        //related #40716: This is the place that we can connect into the onfocus event for an input field in dataentry mode. 
	        //need to reset the focus editor's info.
	        this.AttachEventHandler("onfocus", function(event)
		    {	
	        
		        var doc = ("#document" == event.target.nodeName ? event.target : event.target.ownerDocument);
		        var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
				Ektron.ContentDesigner.onContentWindowChange(win, event.target, { editorId: thisEditor.Id });

	        });
	    }
//#35261 - Mac-ff: adding double space when hit enter in the editor area
//Commented out partial fix for #33731. 
//Not sure why this prevents the onkeypress event from running the rest of the handlers. Tried various options. 
//source: Releases\400.cms400_7.5.2.49\WEBSRC\WorkArea\ContentDesigner\EkRadEditor.js, 400\WEBSRC\WorkArea\ContentDesigner\EkRadEditor.js
//version: 7.5.2.49rrr, 7.5.3.03
//changelist: 34700
//	    this.AttachEventHandler("onkeypress", function(event)
//		{	
//	        HideContextMenu();
//	    });
	}
    function HideContextMenu()
    {
        var popup = window["RadEditorPopupInstance"];
        if (popup)
        {
            popup.Hide();
        }
    }
    
    this.setStatusMessage(this.GetLocalizedString("StatusOK", "Status: OK"));
}

function EkRadEditor_isSelectionEditable()
{
    //#53244: Another workaround for IE8 in EditInContext. The issue was originally discovered in eIntranet 2.0. 
    var oSelElem = this.ContentWindow.ekSmartFormTargetElement;
    if (!oSelElem || false == $ektron.isEditableElement(oSelElem)) oSelElem = this.GetSelectedElement();
    if ($ektron.browser.msie && oSelElem)  
    {
        //#44400: This is a workaround for IE8. IE8 loses focus in EditInContext, returning the DIV with the HIDDEN "__VIEWSTATE"
        if ("DIV" == oSelElem.tagName && oSelElem.firstChild)
        {
            var sId = oSelElem.firstChild.id;
            if ("__VIEWSTATE" == sId || "__EVENTTARGET" == sId || "__EVENTARGUMENT" == sId) //#47614
            {
                oSelElem = this.getFirstEditableLocationIE();
            }
        }
        //#62472: IE8 requires a P tag to have a TEXT_NODE to be contentEditable.
        else if ("P" == oSelElem.tagName && null == oSelElem.firstChild)
        {
            oSelElem.innerHTML = "&#160;";
            return true;
        }
    }
    if (!oSelElem) oSelElem = this.Document.body;
    return $ektron.isEditableElement(oSelElem);
}

function EkRadEditor_getFirstEditableLocationIE()
{
    var oSelElem = null;    
    if ($ektron.browser.msie && this.Mode == RadEditorNamespace.RADEDITOR_DESIGN_MODE)
    {
        // Selection is outside editor. Start with editor's main content area.
	    var objEkContentArea = this.getContentElement();
	    if (null == objEkContentArea)
	    {
		    objEkContentArea = this.ContentArea;
	    }
	    // Find first editable element
	    if (objEkContentArea.isContentEditable)
	    {
		    oSelElem = objEkContentArea;
	    }
	    else 
	    {
		    for (var i = 0; i < objEkContentArea.all.length; i++)
		    {
			    oSelElem = objEkContentArea.all[i];
			    if (oSelElem.isContentEditable)
			    {
				    break; // exit for loop, we found an editable element
			    }
		    }
	    }	    
	}  
    return oSelElem;
}

function EkRadEditor_GetDialogUrl(dialogName)
{
    var result = this.RadEditor_GetDialogUrl(dialogName); // call base class method
    result += "&AccessChecks=" + this.ekParameters.AccessChecks;
    result += "&LibraryAllowed=" + this.ekParameters.LibraryAllowed;
    result += "&FolderId=" + this.ekParameters.FolderId;
    result += "&CanModifyImg=" + this.ekParameters.CanModifyImg;
    result += "&ShowTemporaryMarkers=" + this.ekParameters.ShowTemporaryMarkers;
    result += "&WordPasteThreshold=" + this.ekParameters.WordPasteThreshold;
    result += "&CurrentCharCount=" + this.ekParameters.CurrentCharCount;
    result += "&ShowPasteWarning=" + this.ekParameters.ShowPasteWarning;
	
    return result;
}

function EkRadEditor_getContentElement()
{
	var oElem = null;
	if (this.sfInstance) 
	{
	    oElem = this.sfInstance.getContentElement();
	}
	return oElem;
}

function EkRadEditor_setDesignContent(content)
{
    if (this.Mode == RadEditorNamespace.RADEDITOR_DESIGN_MODE)
    {
		if (this.designContent)
		{
			// we were previewing data entry
			$ektron(".design_mode_entry", this.EkScriptDocument.body)
				.attr("contentEditable", true)
				.addClass("design_mode_design")
				.removeClass("design_mode_entry");
			this.designContent = "";
			this.ekParameters.editMode = "design";
        }
    }
    else if (this.Mode == RadEditorNamespace.RADEDITOR_PREVIEW_MODE)
    {
        this.designContent = content;
        this.ekParameters.editMode = "dataentry";
        var args = null;
        try
        {
            var strDataEntryXslt = this.ekXml.xslTransform(content, "[srcPath]DesignToEntryXSLT.xslt", args, Ektron.OnException.throwException);
            var strDataSchema = this.ekXml.xslTransform(content, "[srcPath]DesignToSchema.xslt", args, Ektron.OnException.throwException);
            this.setContent("dataentryxslt", strDataEntryXslt);
            this.setContent("dataschema", strDataSchema);
            if (this.sfInstance) this.sfInstance.onbeforesave();
            $ektron(".design_mode_design ", this.EkScriptDocument.body)
                .attr("contentEditable", false)
                .addClass("design_mode_entry")
                .removeClass("design_mode_design")
                .empty();
            this.ContentWindow = this.EkScriptWindow;
			this.Document = this.ContentWindow.document;
			this.ContentArea = this.Document.body;
        }
        catch (ex)
        {
            var errmsg = this.GetLocalizedString("ErrProcessContent", "An error occurred when processing the content.");
            alert(errmsg + "\n\n" + Ektron.OnException.exceptionMessage(ex));
            this.Mode = RadEditorNamespace.RADEDITOR_DESIGN_MODE;
            this.setDesignContent(this.designContent);
        }
    }
}

function EkRadEditor_InsertImage(url, oImgVal)
{
	var strImg = "<img src=\"" + url + "\" data-ektron-url=\"" + url + "\"";
	var strAlt = "";
	if (oImgVal && "string" == typeof oImgVal.imageAltText && oImgVal.imageAltText.length > 0)
	{
	    strAlt = oImgVal.imageAltText;
	}
	else
	{
		strAlt = url;
	}
	strAlt = $ektron.htmlEncode(strAlt);
	strImg += " alt=\"" + strAlt + "\" title=\"" + strAlt + "\" />";
	var sTitle = this.Localization[RadEditorNamespace.RADCOMMAND_INSERT_IMAGE];
	this.PasteHtml(strImg, sTitle, false, true, true);
}

function EkRadEditor_ExecuteBrowserCommand(sCmdID, bCanUnexecute, value, selChanged)
{
    var sTitle = this.Localization[sCmdID];
	// #34132: Passing a setFocus = false to ExecuteCommand when Deleting a Table is a workaround for this focus issue.
	this.ExecuteCommand(RadEditorNamespace.RadBrowserCommand.New(sTitle
											, sCmdID
											, this.ContentWindow
											, value), (RadEditorNamespace.RADCOMMAND_DELETE == sCmdID ? false : true));
    // #49079 : FF - remove the select class SPAN that FF added to the ex-hyperlink.
    if (RadEditorNamespace.RADCOMMAND_UNLINK == sCmdID)
    {
        if (this.sfInstance)this.sfInstance.setSelectedField(null);
        $ektron(".design_selected_field", this.ContentArea).unwrapInner();
    }
	this.SetActive();
	this.SetFocus();
	if (true == selChanged) this.FireEvent(RadEditorNamespace.RADEVENT_SEL_CHANGED, null);
}

function EkRadEditor_RemoveStyles()
{
    var strContent = this.getContent();
    strContent = RadEditorNamespace.StripFormatting(strContent, "STYLE"); 
    strContent = strContent.replace(/(<[^>]+)\sclass="[^"]*Apple-style-\w+"([^>]*>)/ig, "$1$2");
    this.setContent("document", strContent);
}

function EkRadEditor_ExecuteInsertObjectCommand(oObject, sTitle)
{
    // Replacing ExecuteInsertObjectCommand at 2radeditor.js
    this.SetFocus();
    var sPasteString = RadEditorNamespace.Utils.GetOuterHtml(oObject);
    return this.ExecuteCommand(RadEditorNamespace.RadPasteHtmlCommand.New(sTitle, this.ContentWindow, sPasteString));
}

function EkRadEditor_validateContent()
{
	if ("dataentry" == this.ekParameters.editMode)
	{
		return this.validateXmlContent();
	}
	else
	{
		return this.validateDesign();
	}
}

function EkRadEditor_validateDesign()
{
	// for design mode
	try
	{
		var strContent = this.getContent();

		if (strContent.length > 0 && -1 == strContent.indexOf("<"))
		{
			strContent = [ "<p>", strContent, "</p>" ].join("");
		}
		if (strContent.length > 0)
		{
			var err = null;
			var contentElem = this.getContentElement();
			if (contentElem)
			{
				var eRootElems = $ektron(contentElem).find("[ektdesignns_role='root']");
				if (eRootElems.length > 1)
				{
					err = this.GetLocalizedString("sDupRootTags", "Only one field may be assigned as the Root Tag.") + "\n\n" + 
							this.GetLocalizedString("sFields", "Fields") + ":";
					eRootElems.each(function()
					{
						err += "\n    " + this.getAttribute("ektdesignns_caption") + " (" + this.getAttribute("ektdesignns_name") + ")";
					});
				}
			}
		
			if (null == err)
			{
				var strSchema = this.ekXml.xslTransform(strContent, "[srcPath]DesignToSchema.xslt", null, Ektron.OnException.returnException); 
				if (strSchema.length > 0 && strSchema.indexOf("<xs:schema") >= 0)
				{
					err = this.ekXml.validateXsd(strSchema);
				}
				else if (strSchema.length > 0) //if strSchema contains error
				{
					err = strSchema;
				}
				if (null == err)
				{
					err = this.validateSchemaDesign(strSchema);
				}
			}
			if (null == err && this.ekParameters.SchemaFiles != null && this.ekParameters.SchemaFiles.length > 0 && false == canValidateContent(strContent))
			{
			    var schemaFiles = getSchemaParam(this.ekParameters.SchemaFiles);  
			    var schemaNamespaces = getSchemaParam(this.ekParameters.SchemaNamespaces);    
			    err = this.ekXml.validateXml(createContentPage(strContent), schemaFiles, schemaNamespaces);
			}
			if (null == err)
			{
				err = this.validateContentXPathExpr(strContent);
			}
			if (null == err)
			{
				err = this.validateAccessibility(strContent);
			}
			if (err != null)
			{
			    this.contentCache = null;
			}
			return err;
		}
	}
	catch (ex)
	{
		Ektron.OnException(this, Ektron.ContentDesigner.onexception, ex, arguments);
	}
	
	function getSchemaParam(arySchema) //private
	{
	    var schemalParam = arySchema;
	    schemalParam = schemalParam.substr(1, schemalParam.length - 2); // remove the [] at the outer boundary.
	    var arySchemalParam = schemalParam.split(",");
	    schemalParam = arySchemalParam[0];
	    if (arySchemalParam.length > 1)
	    {
	        schemalParam = arySchemalParam;
	    }
	    return schemalParam;
	}
	
    function canValidateContent(strContent)	//private
    {
        //return true for smart form content (contain custom tags).
        var bIsSmartForm = false;
        if (strContent.indexOf("ektdesignns_") > -1)
        {
            bIsSmartForm = true;
        }
        return bIsSmartForm;
    }	
    
    function createContentPage(strContent)	//private
    {
        return "<body> " + strContent + " </body>";
    }
}

function EkRadEditor_validateSchemaDesign(strSchema)
{
	var me = this;
    var bValid = true;
    var validationMsg = [];
    var globalElements = [];
    var xmlDoc = this.ekXml.loadXml(strSchema); 
    if (xmlDoc.hasChildNodes() && xmlDoc.childNodes.length > 0)
    {
        for (var i = 0; i < xmlDoc.childNodes.length; i++)
        {
            var objNode = xmlDoc.childNodes[i];
            bValid = rfnValidateSchemaDOM(objNode);
            if (validationMsg.length > 0)
            {
                return validationMsg;
            }
        }
        if (globalElements.length > 1)
        {
			var valMsg = me.GetLocalizedString("sMultRootTags", "A field assigned as the Root Tag must contain all other fields.") + "\n\n" + 
					me.GetLocalizedString("sFields", "Fields") + ":";
			for (var i = 0; i < globalElements.length; i++)
			{
				valMsg += "\n    " + globalElements[i];
			}
			return valMsg;
		}
    }
    return null;
    
    function rfnValidateSchemaDOM(objParentNode)
    { 
        // private function to check duplicate fieldnames (id)
        var bValid = true;
        if (null == objParentNode) return null;
        if (!objParentNode.hasChildNodes()) return null;
        if (0 == objParentNode.childNodes.length) return null;
        var aryFieldNames = [];
        var strFieldName = "";
        for (var j = 0; j < objParentNode.childNodes.length; j++)
        {
            var objNode = objParentNode.childNodes[j];
            if (objNode != null)
            {
                if ("xs:element" == objNode.nodeName)
                {
                    var objAttr = objNode.attributes.getNamedItem("name");
                    if (objAttr)
                    {
                        strFieldName = objAttr.nodeValue;
                        // XML is case-sensitive
                        if (strFieldName && strFieldName.length > 0)
                        {
                            if (true == aryFieldNames[strFieldName])
                            {
                                validationMsg[validationMsg.length] = me.GetLocalizedString("sFieldsUniqueNames", "Fields within the same group must have unique names.") + " " + 
									me.GetLocalizedString("sDupFieldName", "Duplicate field name:") + " " + strFieldName; // duplicate field name
                                bValid = false;
                                break;
                            }
                            else
                            {
                                aryFieldNames[strFieldName] = true;
                            }
                            if ("xs:schema" == objParentNode.nodeName)
                            {
								globalElements.push(strFieldName);
                            }
                        }
                    }
                }
                if (objNode.nodeName != "xs:simpleType" && objNode.nodeName != "xs:attributeGroup")
                {
					if (false === rfnValidateSchemaDOM(objNode))
					{
						bValid = false;
						break;
					}
                }
            }
        }
        return bValid;	
    }
}

function EkRadEditor_validateContentXPathExpr(strContent) 
{
	// Check if XPath expressions contain field variables, e.g., {X}
	// Returns error message.
	var args = [
        { name: "LangType", value: this.ekParameters.userLanguage }
    ];
	var strErrorMessage = this.ekXml.xslTransform(strContent, "[srcPath]ValidateDesign.xslt", args, Ektron.OnException.returnException); 
	if (0 == strErrorMessage.length)
	{
		return null; // valid
	}
	else
	{
		return (strErrorMessage.split("\n\n\n")); // array of error messages
	}
}

function EkRadEditor_validateAccessibility(strContent)
{
    // return null when the content pass the accessibility test.
	var err = null;
	try
	{
		var xmlDoc = this.ekXml.loadXml("[srcPath]ValidateSpec.xml"); 
		var editor = this;
		$ektron("datadesign>validate", xmlDoc).children("[enabled='true']").each(function()
		{ 
			try
			{
				var name = this.getAttribute("name");
				var id = this.getAttribute("id");
				var validationType = this.tagName;
				if (editor.ekParameters.AccessChecks.toLowerCase() != "none")
				{
					var returnMsg = "";
					if ("xslt" == validationType)
					{
						var args = [ 
							{ name: "baseURL", value: "" }, 
							{ name: "outputFormat", value: "text" } 
						];
						var baseURL = editor.ekParameters.ImagePaths;
						if (-1 == baseURL.indexOf(","))
						{
							baseURL = baseURL.replace(/\~/, editor.ApplicationPath);
							baseURL = baseURL.replace(/\/{2}/, "/");	
							args = [
								{ name: "baseURL", value: editor.serverUrl + baseURL }
							,	{ name: "outputFormat", value: "text" }
							];
						} 
						var xslt = this.getAttribute("src");
						var sPreHtml = "<html><head></head><body>";
						var sPostHtml = "</body></html>";
						strContent = sPreHtml + strContent + sPostHtml;
						try
						{
							returnMsg = editor.ekXml.xslTransform(strContent, xslt, args);
						}
						catch (ex) {}
						if (returnMsg.length > 0)
						{
							err = {code : -1000, msg : returnMsg, doctype : ""};
						}
					}
				}
			}
			catch (ex)
			{
				Ektron.OnException(editor, Ektron.ContentDesigner.onexception, ex, arguments);
			}
		});
	}
	catch (ex)
	{
		Ektron.OnException(this, Ektron.ContentDesigner.onexception, ex, arguments);
	}
    return err;
}
 
function EkRadEditor_validateXmlContent()
{
	// for data entry mode
	var err = null;
	try
	{
		if (!this.SmartForm.validateForm("design_content"))
		{
			// design_validateForm has returned false for invalid content. The following is to get the error message.
			var oInvalidElem = this.EkScriptWindow.document.getElementById('dsg_validate');
			if (oInvalidElem)
			{
				err = oInvalidElem.title; 
			}
			else
			{
				err = this.GetLocalizedString("ContentInvalid", "Content is not valid");
			}
		}
		else 
		{
			var strXml = "";
			if (RadEditorNamespace.RADEDITOR_PREVIEW_MODE == this.Mode)
			{
			    strXml = this.getContent("preview");
			}
			else
			{
			    strXml = this.getContent();
			}
			if (strXml.length > 0 && -1 == strXml.indexOf("<"))
			{
				strXml = [ "<p>", strXml, "</p>" ].join("");
			}
			//Ektron.ContentDesigner.trace("validateXmlContent\n" + strXml);
			if (this.dataSchema)
			{
				// validate against schema
				err = this.ekXml.validateXml(strXml, this.dataSchema, "");
			}	    
			if (null == err)
			{
		        
				err = this.validateAccessibility(strXml);
			}
		}
	}
	catch (ex)
	{
		Ektron.OnException(this, Ektron.ContentDesigner.onexception, ex, arguments);
	}
	if (err != null)
	{
	    this.contentCache = null;
	}
	return err;
}

function EkRadEditor_checkCompatibility()
{
	// for design mode
	try
	{
		if (null == this.initialContent) return;
		if (null == this.initialFieldList)
		{
			var strInitialContent = this.initialContent;
			if (strInitialContent.length > 0 && -1 == strInitialContent.indexOf("<"))
			{
				strInitialContent = [ "<p>", strInitialContent, "</p>" ].join("");
			}
			this.initialFieldList = this.ekXml.xslTransform(strInitialContent, "[srcPath]DesignToFieldList.xslt");
		}
		
		var strFieldList = this.getContent("datafieldlist");
		var strBothFieldLists = [ "<root>", this.initialFieldList, strFieldList, "</root>" ].join("\n");


		var args = [
            { name: "LangType", value: this.ekParameters.userLanguage }
        ];
		var strErrorMessage = this.ekXml.xslTransform(strBothFieldLists, "[srcPath]CompatibilityReport.xslt", args);
		if (0 == strErrorMessage.length)
		{ 
			if (null == this.initialSchema)
		    {
			    var strInitialContent = this.initialContent;
			    this.initialSchema = this.ekXml.xslTransform(strInitialContent, "[srcPath]DesignToSchema.xslt");
		    }
            var strSchema = this.getContent("dataschema");
            if (strSchema != this.initialSchema)
            {
                return this.GetLocalizedString("sDesignSchemaChanged", "The design schema has changed. The design may not be compatible with any exisiting data.");
            }
            else
            {
                return null; // compatible
            }
		}
		else
		{
			return (strErrorMessage.split("\n\n\n")); // array of error messages
		}
	}
	catch (ex)
	{
		Ektron.OnException(this, Ektron.ContentDesigner.onexception, ex, arguments);
	}
}

function EkRadEditor_SetFocusIE()
{
	try
	{
		if (this.Mode == RadEditorNamespace.RADEDITOR_DESIGN_MODE)
		{
			var oSelElem = null;
			if (this.Document)
			{
				// Calling focus() when selection.type="None" caused the selection to move 
				// to the top of the content. Calling setActive() when selection.type="None" 
				// caused the selection to be lost (and the command to be ignored).
				// Example, undo, indent, bullets. See defects #33191, #34701, #34702
				if (this.Document.selection && ("None" == this.Document.selection.type || "Control" == this.Document.selection.type))
				{
					return; 
				}
				// Preserve selected element
				oSelElem = getSelectionElement(this.Document);
				if (null == oSelElem)
				{
					oSelElem = this.Document.activeElement;
				}
				if (oSelElem != null)
				{
					if (oSelElem.isContentEditable)
					{
						// #34125: in designer mode, if it loops up to the parent, the page focus is incorrect for the table
						// properties (insert row/column, delete row/column, merge cell etc.) to work.
						// in dataentry mode, it is needed to insert controls (img, hr) into the richarea field.
						if ("dataentry" == this.ekParameters.editMode)
						{
							// Need the top editable element, not the inner elements that inherit editability
							if (oSelElem != null)
							{		
								var joSelElem = $ektron(oSelElem);
								var joParent = joSelElem.parent();
								while (joParent.length > 0 && joParent.get(0).isContentEditable)
								{
									joSelElem = joParent;
									joParent = joSelElem.parent();
								}
								oSelElem = joSelElem.get(0);
							}
						}
					}
					else
					{	
						oSelElem = this.getFirstEditableLocationIE(); 
					}
				}
			}
				
			try // #34227 - error when you enter text and click enter key in the field properties text box
			{
				this.ContentWindow.focus();
			}
			catch (ex)
			{
				// ignore error
			}

			if (!this.IsIE9 && oSelElem != null)
			{
				oSelElem.setActive();					
			}
		}
		else if (this.Mode == RadEditorNamespace.RADEDITOR_HTML_MODE) 
		{
			try
			{
				this.ContentTextarea.focus();
			}
			catch (ex)
			{
				// ignore error
			}
		}
	}
	catch (ex)
	{
		Ektron.OnException(this, Ektron.ContentDesigner.onexception, ex, arguments);
	}
}

function EkRadEditor_private_SetPageHtml(newContent, forceNewDocument, callback)//forceNewDocument - only on editor load!
{
	if (-1 != newContent.toLowerCase().indexOf("<html"))//<html> is not good as you can have <html xmlns="http://www.w3.org/1999/xhtml">
	{
        // get content within the body tag.
        newContent = newContent.replace(/[\w\W]*?<body[^>]*>([\w\W]*?)<\/body>[\w\W]*/i, "$1");
        newContent = newContent.replace(/[\w\W]*?<html[^>]*>([\w\W]*?)<\/html>[\w\W]*/i, "$1");
	}

	if (forceNewDocument)
	{
		var bIsInitialized = false;
		var me = this;
		function initContentAreaElement()
		{
			if (bIsInitialized) return;
			var oIFrame = me.ContentAreaElement;
			if (!oIFrame) return;
			var oWin = oIFrame.contentWindow;
			if (!oWin) return;
			var oDoc = oWin.document;
			if (!oDoc) return;
			var oBody = oDoc.body;
			if (!oBody) return;
			var oHead = oDoc.getElementsByTagName("HEAD")[0];
			if (!oHead || !oHead.hasChildNodes()) return;

			bIsInitialized = true;
			me.ContentWindow = oWin;
			me.Document = oDoc;
			me.ContentArea = oBody
			//Attach the browser events -> Must be here or IE does not work!
			me.InitRadEvents();

			//Set enhanced edit mode (dashed table borders, etc) if necessary
			me.EnableEnhancedEdit = !me.EnableEnhancedEdit;
			me.ToggleEnhancedEdit();
            me.ParagraphsArray =  [
						 [me.GetLocalizedString("sNormal", "Normal"), "<p>"] 
						, ["<h1>" + me.GetLocalizedString("sH1", "Heading 1") + "</h1>", "<h1>"]
						, ["<h2>" + me.GetLocalizedString("sH2", "Heading 2") + "</h2>", "<h2>"]
						, ["<h3>" + me.GetLocalizedString("sH3", "Heading 3") + "</h3>", "<h3>"]
						, ["<h4>" + me.GetLocalizedString("sH4", "Heading 4") + "</h4>", "<h4>"]
						, ["<h5>" + me.GetLocalizedString("sH5", "Heading 5") + "</h5>", "<h5>"]
						, ["<pre>" + me.GetLocalizedString("sFormatted", "Formatted") + "</pre>", "<pre>"]
						, ["<address>" + me.GetLocalizedString("sAddress", "Address") + "</address>", "<address>"]
					];
					
			me.SetContentAreaHtml(newContent, callback);
		};

		RadEditorNamespace.Utils.AttachEventEx(this.ContentAreaElement, "load", initContentAreaElement);
		initContentAreaElement();
	}

	//If this code is not (specifically!) here, SAFARI does not set height to the content IFRAME
	//RE5-4843- However, even with this code, it CANNOT DEAL WITH height 100% or any percentage
	if (this.IsSafari)
	{
		if(this.Height && this.Height.indexOf("%") == -1)
		{
			this.ContentAreaElement.style.height = this.Height;
		}
		else
		{
			try //Ektron
			{
			    var oTd = this.ContentAreaElement.parentNode;
			    var oDiv = this.Document.createElement("div");
			    oDiv.style.height = "100%";
			    oDiv.innerHTML = "&nbsp;";
			    oTd.appendChild(oDiv);
			    var oHeight = RadEditorNamespace.Utils.GetRect(oTd).height;
			    oDiv.parentNode.removeChild(oDiv);
			    this.ContentAreaElement.style.height = oHeight;
			}// Ektron start : if any exception occurs, e.g. this.Document is null (#39124), it will just set the Height. 	    
			catch (ex)
			{
			    this.ContentAreaElement.style.height = this.Height;
			}
		}
	}

	if (!forceNewDocument)
	{
	    //Set the content in the content area
	    this.SetContentAreaHtml(newContent, callback);
	}
}

function EkRadEditor_SetContentAreaHtml(newContent, callback)
{
	var objEkContentArea = this.getContentElement();
	if (null == objEkContentArea)
	{
		// prevent re-entrancy
		if (this.contentCallbackQueue)
		{
			this.contentCallbackQueue.push(callback);
			this.contentCallbackQueue.newContent = newContent; // keep most recent content
			return;
		}
		this.contentCallbackQueue = [callback]; //#52965
		
		var srcPath = this.ekParameters.srcPath;
		var resPath = this.ekParameters.resPath;
		var skinPath = this.ekParameters.skinPath;
		var editorSkinPath = this.ekParameters.editorSkinPath; // for radEditor skin files
		
		if ("" == newContent && false == this.NewLineBr)
		{
		    newContent = "<p>&#160;</p>";
		}
		if ("dataentry" == this.ekParameters.editMode)
		{
			newContent = "<div id=\"design_content\" class=\"design_mode_entry\" style=\"width: 100%; height:100%;\" contentEditable=\"false\">" + newContent + "</div>";
		}
		else
		{
			newContent = "<div id=\"design_content\" class=\"design_mode_design\" style=\"width: 100%; height:100%;\" contentEditable=\"true\">" + newContent + "</div>";
		}
		
		// Resolve paths prior to loading in editor to avoid 404 errors.
		// See design_qualifySrcPath in ekforms.js
		newContent = newContent.replace(/(\"|\')[^\[\"\']*\[skinpath\]/g, "$1" + skinPath);
		newContent = newContent.replace(/(\"|\')[^\[\"\']*\[srcpath\]btn/g, "$1" + skinPath + "btn");
		newContent = newContent.replace(/(\"|\')[^\[\"\']*\[srcpath\]additem.gif/g, "$1" + skinPath + "additem.gif");
		newContent = newContent.replace(/(\"|\')[^\[\"\']*\[srcpath\]designmenu.gif/g, "$1" + skinPath + "designmenu.gif");
		newContent = newContent.replace(/(\"|\')[^\[\"\']*\[srcpath\]/g, "$1" + srcPath);

		this.RadEditor_SetContentAreaHtml(newContent); // call base class method
		
//		var me = this;
		var objScriptDoc = this.EkScriptDocument;
		var objHeadElem = objScriptDoc.getElementsByTagName("HEAD")[0];
		if (!objHeadElem.hasChildNodes())
		{
			alert("ERROR: the HEAD element was empty when attempting to set the content.");
			throw new Error("The HEAD element is empty.");
		}

//		function includeStyle(url)
//		{
//			var objLink = objScriptDoc.createElement("link");
//			objLink.rel = "Stylesheet";
//			objLink.type = "text/css";
//			objLink.href = url;
//			objHeadElem.appendChild(objLink);
//		};
		
		function appendScript(code)
		{
			var objScript = objScriptDoc.createElement("script");
			objScript.type = "text/javascript";
			if ($ektron.browser.msie)
			{
				objScript.text = code; // Opera 9.5, .text is read-only
			}
			else
			{
				objScript.appendChild(objScriptDoc.createTextNode(code));
			}
			objHeadElem.appendChild(objScript);
		};
		
//		function includeScript(url, callback)
//		{
//			var objScript = objScriptDoc.createElement("script");
//			objHeadElem.appendChild(objScript);
//			objScript.type = "text/javascript";
//			if ("function" == typeof callback) 
//			{
//				var done = false;
//				function callbackOnce()
//				{
//					if (!done)
//					{
//						done = true;
//						me.tryCallback(callback);
//					}
//				};
//				objScript.onload = objScript.onreadystatechange = function()
//				{
//					var state = this.readyState;
//					if (!state || "loaded" == state) 
//					{
//						// Let this event finish before calling the callback
//						setTimeout(callbackOnce, 40); // give a little extra time in case it's not really done yet
//					}
//					else if ("complete" == state)
//					{
//						setTimeout(function() 
//						{ 
//							if (!done)
//							{
//								Ektron.ContentDesigner.onexception(new Error(Ektron.String.format(
//								"Timed out waiting for readyState 'loaded' when loading {0}.", url)), arguments);
//								callbackOnce();
//							}
//						}, 1000);
//					}
//				};
//				setTimeout(function() 
//				{ 
//					if (!done)
//					{
//						includeScript.failed.push(url);
//						includeScript.timeoutLimit = 3000;
//						callbackOnce();
//					}
//				}, includeScript.timeoutLimit);
//			}
//			objScript.src = url;
//		};
//		includeScript.timeoutLimit = 30000;
//		includeScript.failed = [];
		
//		appendScript( //#46530
//		[
//			"if (typeof design_div_focus != \"function\") design_div_focus = function(){};"
//			,"if (typeof design_div_blur != \"function\") design_div_blur = function(){};"
//			,"if (typeof design_row_setCurrent != \"function\") design_row_setCurrent = function(){};"
//			,"if (typeof design_row_onmouse != \"function\") design_row_onmouse = function(){};"
//			,"if (typeof design_row_showContextMenu != \"function\") design_row_showContextMenu = function(){};"
//		].join("\n"));
			
//		includeStyle(skinPath + "ektron.smartForm.css");
//		
//		includeScript(resPath + "ektron.js", function(){
//		includeScript(resPath + "plugins/autoheight/ektron.autoheight.js", function(){
//		includeScript(resPath + "plugins/string/ektron.string.js", function(){
//		includeScript(resPath + "ektron.xml.js", function(){
//		includeScript(srcPath + "ekxbrowser.js", function(){
//		includeScript(srcPath + "ektron.smartForm.js", function()
//		{
//			if (includeScript.failed.length > 0)
//			{
//				alert("ERROR: The following JavaScript files failed to load:\n" + includeScript.failed.join("\n"));
//				throw new Error("JavaScript files failed to load in the content layer.");
//			}
//			appendScript(
//			[
//				"$ektron.ready();"
//				,"var settings = "
//				,"{   srcPath: \"" + srcPath + "\""
//				,",   skinPath: \"" + skinPath + "\""
//				,",   editorSkinPath: \"" + editorSkinPath + "\""
//				,",   langType: " + me.ekParameters.contentLanguage
//				,",   editorId: \"" + me.Id + "\""
//				,",   CssFilesArray: ", $ektron.toLiteral(me.CssFilesArray)
//				,",   localizedStrings: "
//				,"    { stdOK: ", $ektron.toLiteral(me.GetLocalizedString("stdOK", "OK"))
//				,"    , stdCancel: ", $ektron.toLiteral(me.GetLocalizedString("stdCancel", "Cancel"))
//				,"    , mnuInsAbv: ", $ektron.toLiteral(me.GetLocalizedString("mnuInsAbv", "Insert Above"))
//				,"    , mnuInsBel: ", $ektron.toLiteral(me.GetLocalizedString("mnuInsBel", "Insert Below"))
//				,"    , mnuDupl: ", $ektron.toLiteral(me.GetLocalizedString("mnuDupl", "Duplicate"))
//				,"    , mnuMvUp: ", $ektron.toLiteral(me.GetLocalizedString("mnuMvUp", "Move Up"))
//				,"    , mnuMvDn: ", $ektron.toLiteral(me.GetLocalizedString("mnuMvDn", "Move Down"))
//				,"    , mnuRem: ", $ektron.toLiteral(me.GetLocalizedString("mnuRem", "Remove"))
//				,"    , sFld: ", $ektron.toLiteral(me.GetLocalizedString("sFld", "Field"))
//				,"    , sInvFld: ", $ektron.toLiteral(me.GetLocalizedString("sInvFld", "At least one field is not valid. Please correct it and try again."))
//				,"    , sShow: ", $ektron.toLiteral(me.GetLocalizedString("sShow", "Show"))
//				,"    , sHide: ", $ektron.toLiteral(me.GetLocalizedString("sHide", "Hide"))
//				,"    }"
//				,"};"
//				,"try {"
//				,"    var sf = new Ektron.SmartForm('design_content', settings);" 
//				,"    sf.onload();" 
//				,"} catch (ex) {"
//				,"    alert('Error initializing smart form: ' + ex.message);"
//				,"}"
//			].join("\n"));
//			
//			me.SmartForm = me.EkScriptWindow.Ektron.SmartForm;
//			me.sfInstance = me.SmartForm.instances["design_content"];
//			
//			Ektron.SelectionRange.ensureContentUsability(me.getContentElement());

//			Ektron.ContentDesigner.trace("Created SmartForm instance id=" + me.Id);

//			me.tryCallback(callback);
//			
//			// check if re-entrancy happened
//			if ("string" == typeof me.contentCallbackQueue.newContent)
//			{
//				me.SetContentAreaHtml(me.contentCallbackQueue.newContent);
//				me.contentCallbackQueue.newContent = null;
//			}
//			while (me.contentCallbackQueue.length > 0)
//			{
//				me.tryCallback(me.contentCallbackQueue.shift());
//			}
//			me.contentCallbackQueue = null; 
//		}); }); }); }); }); });
		
		appendScript(
		[
            //#46530
            "if (typeof design_div_focus != \"function\") design_div_focus = function(){};"
			,"if (typeof design_div_blur != \"function\") design_div_blur = function(){};"
			,"if (typeof design_row_setCurrent != \"function\") design_row_setCurrent = function(){};"
			,"if (typeof design_row_onmouse != \"function\") design_row_onmouse = function(){};"
			,"if (typeof design_row_showContextMenu != \"function\") design_row_showContextMenu = function(){};"
			,"var settings = "
			,"{   srcPath: \"" + srcPath + "\""
			,",   skinPath: \"" + skinPath + "\""
			,",   editorSkinPath: \"" + editorSkinPath + "\""
			,",   langType: " + this.ekParameters.contentLanguage
			,",   editorId: \"" + this.Id + "\""
            ,",   Direction: \"" + this.ekParameters.Direction + "\""
			,",   CssFilesArray: ", $ektron.toLiteral(this.CssFilesArray)
			,",   localizedStrings: "
			,"    { stdOK: ", $ektron.toLiteral(this.GetLocalizedString("stdOK", "OK"))
			,"    , stdCancel: ", $ektron.toLiteral(this.GetLocalizedString("stdCancel", "Cancel"))
			,"    , mnuInsAbv: ", $ektron.toLiteral(this.GetLocalizedString("mnuInsAbv", "Insert Above"))
			,"    , mnuInsBel: ", $ektron.toLiteral(this.GetLocalizedString("mnuInsBel", "Insert Below"))
			,"    , mnuDupl: ", $ektron.toLiteral(this.GetLocalizedString("mnuDupl", "Duplicate"))
			,"    , mnuMvUp: ", $ektron.toLiteral(this.GetLocalizedString("mnuMvUp", "Move Up"))
			,"    , mnuMvDn: ", $ektron.toLiteral(this.GetLocalizedString("mnuMvDn", "Move Down"))
			,"    , mnuRem: ", $ektron.toLiteral(this.GetLocalizedString("mnuRem", "Remove"))
			,"    , sFld: ", $ektron.toLiteral(this.GetLocalizedString("sFld", "Field"))
			,"    , sInvFld: ", $ektron.toLiteral(this.GetLocalizedString("sInvFld", "At least one field is not valid. Please correct it and try again."))
			,"    , sShow: ", $ektron.toLiteral(this.GetLocalizedString("sShow", "Show"))
			,"    , sHide: ", $ektron.toLiteral(this.GetLocalizedString("sHide", "Hide"))
			,"    }"
			,"};"
			,"var sf = null;"
			,"try {"
			,"    sf = Ektron.SmartForm.create('design_content', settings);" 
			,"    if (sf) sf.onload();" 
			,"} catch (ex) {"
			,"    alert('Error initializing smart form: ' + ex.message);"
			,"} finally {"
            ,"    Ektron.ContentDesigner.raiseEvent(window, \"onCreateContentDesigner\", [sf, settings]);" //#52965
            ,"}"
		].join("\n"));

        //#52965: A Timing issue is found here with this.sfInstance is called when the above scripts from appendScript inside the editor
        //(Ektron.SmartForm.create('design_content', settings)) is not completed. Hence, there are errors when ContentDesigner is loaded 
        //below. This situation occurs when more than expected CSS and/or JS files is loaded to the FF editor iframe. The fixes is to raise
        //an event when the sf is completely created. In the event, it will finished loading the editor by calling the callback below. This 
        //issue is originally found in eIntranet 2.0 site.
//		this.SmartForm = this.EkScriptWindow.Ektron.SmartForm; 
//		this.sfInstance = this.SmartForm.instances["design_content"];

//		var containingElement = this.getContentElement();
//		if (containingElement)
//		{
//		    var doc = containingElement.ownerDocument;
//		    var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
//		    win.ektronContentUsability = true;
//			if ($ektron.browser.mozilla)
//			{
//		        win.parent.ektronContentUsability = win.ektronContentUsability;
//		    }
//		}

//		    Ektron.SelectionRange.ContentUsability.localize(this);
//		    Ektron.SelectionRange.ensureContentUsability(containingElement);

//		Ektron.ContentDesigner.trace("Created SmartForm instance id=" + this.Id);

//		this.tryCallback(callback);
//		
//		// check if re-entrancy happened
//		if ("string" == typeof this.contentCallbackQueue.newContent)
//		{
//			this.SetContentAreaHtml(this.contentCallbackQueue.newContent);
//			this.contentCallbackQueue.newContent = null;
//		}
//		while (this.contentCallbackQueue.length > 0)
//		{
//			this.tryCallback(this.contentCallbackQueue.shift());
//		}
//		this.contentCallbackQueue = null; 
    }
    else
    {
		try
		{
		    if ("" == newContent && false == this.NewLineBr)
	        {
	            newContent = "<p>&#160;</p>";
	        }
	        RadEditorNamespace.SetElementInnerHTML(objEkContentArea, newContent);
			this.sfInstance.onload({ prevalidate: (RadEditorNamespace.RADEDITOR_PREVIEW_MODE == this.Mode) }); // defined in ektron.sfInstance.js
			Ektron.SelectionRange.ensureContentUsability(this.getContentElement());
		}
		catch (e) {;}//RE5-1732 - Exception when malformed HTML with a PRE tag.
		this.tryCallback(callback);
    }
    this.contentCache = null; 
}

function EkRadEditor_setContent(setType, newContent)
{
	switch (setType)
	{
	case "text":
		this.SetHtml($ektron.htmlEncode(newContent));
		break;
	case "dataentryxslt":
		this.dataEntryXslt = newContent;
		this.dataEntryXsltArgs = [
		{ name: "baseURL", value: this.serverUrl },
		{ name: "LangType", value: this.ekParameters.contentLanguage } 
		];
		if (this.FiltersManager && this.FiltersManager.Filters)
		{
			// update the EkDataEntryFilter filter
			for (var i = 0; i < this.FiltersManager.Filters.length; i++)
			{
				if ("EkDataEntryFilter" == this.FiltersManager.Filters[i].Name)
				{
					// this.Filters seems not to be used
					this.FiltersManager.Filters[i] = new EkDataEntryFilter(this, this.dataEntryXslt, this.dataEntryXsltArgs); 
					break;
				}
			}
		}
		break;
	case "dataschema":
		this.dataSchema = newContent;
		break;
	default:
		this.SetHtml(newContent);
		break;
	}
}

function EkRadEditor_getContent(getType)
{
	var bRefreshData = false;
	if ("design" == this.ekParameters.editMode || RadEditorNamespace.RADEDITOR_PREVIEW_MODE == this.Mode)
	{
	    bRefreshData = true;
	}
	var content = "";
	switch (getType)
	{
	case "text":
		return this.GetText();
		break;
	case "dataentryxslt":
		if (bRefreshData)
	    {
	        content = this.GetHtml(true);
	        return this.ekXml.xslTransform(content, "[srcPath]DesignToEntryXSLT.xslt");
	    }
	    else
	    {
		    return this.dataEntryXslt;
		}
		break;
	case "dataschema":
	    if (bRefreshData)
	    {
	        content = this.GetHtml(true);
	        return this.ekXml.xslTransform(content, "[srcPath]DesignToSchema.xslt");
	    }
	    else
	    {
		    return this.dataSchema;
		}
		break;
    case "preview":
        content = this.GetHtml(true, RadEditorNamespace.RADEDITOR_DESIGN_MODE);
        content = this.ekXml.xslTransform(content, "[srcPath]PresentationToData.xslt");
        return content;
        break;
    case "datapresentationxslt":
        if (bRefreshData)
	    {
	    	var args = [
	    		{ name: "srcPath", value: this.ekParameters.srcPath }
	    	,	{ name: "workareaPath", value: this.workareaPath } 
	    	];
            content = this.GetHtml(true);
	        return this.ekXml.xslTransform(content, "[srcPath]DesignToViewXslt.xslt", args);
	    }
	    else
	    {
	        return this.dataPresentationXslt;
	    }
        break;
    case "datafieldlist":	  
        if (bRefreshData)
	    {
			if (this.formContent)
			{
	    		var args = [
	    			{ name: "srcPath", value: this.ekParameters.srcPath }
	    		,	{ name: "rootXPath", value: "/*/Data" } 
	    		];
				return this.ekXml.xslTransform(this.formContent, "[srcPath]DesignToFieldList.xslt", args);
			}
			else
			{
				content = this.GetHtml(true);
				return this.ekXml.xslTransform(content, "[srcPath]DesignToFieldList.xslt");
			}
	    }
	    else
	    {
	        return this.dataFieldList;
	    }
        break;
    case "datafieldlistjs":	  
		if (this.formContent)
		{
    		var args = [
    			{ name: "srcPath", value: this.ekParameters.srcPath }
    		,	{ name: "rootXPath", value: "/*/Data" } 
    		];
			return this.ekXml.xslTransform(this.formContent, "[srcPath]DesignToFieldListJS.xslt", args);
		}
		else
		{
			content = this.GetHtml(true);
			return this.ekXml.xslTransform(content, "[srcPath]DesignToFieldListJS.xslt");
		}
        break;
    case "dataindex":
        if (bRefreshData)
	    {
            content = this.GetHtml(true);
	        return this.ekXml.xslTransform(content, "[srcPath]DesignToIndex.xslt");
	    }
	    else
	    {   
	        return this.dataIndex;
	    }
        break;
    case "datadocumentxml":
        if (bRefreshData)
	    {
            content = this.GetHtml(true);
	        return this.ekXml.xslTransform(content, "[srcPath]PresentationToData.xslt");
	    }
	    else
	    {
		    return this.GetHtml(true);
	    }
        break;    
	default:
		this.setIsChanged(false);    
		return this.GetHtml(true);
		break;
	}
}

function EkRadEditor_GetHiddenTextareaValue()
{
	var oDataDocXml = this.FindElement("DataDocumentXml");
	if (oDataDocXml)
	{
		var oDataEntryXslt = this.FindElement("DataEntryXslt");
		if (oDataEntryXslt)
		{
			if (oDataEntryXslt.value.length > 0)
			{
				this.setContent("dataentryxslt", oDataEntryXslt.value);
				var strXml = oDataDocXml.value;
				if (0 == strXml.length)
				{
					strXml = "<root></root>";
				}
				this.SetHiddenTextareaValue(TelerikNamespace.Utils.EncodePostbackContent(strXml)); //#49753
			}
			
			// Indicate we have already read the xml and xslt and done the initial transform
			// Clear out value so it is not submitted back to the web server
			oDataDocXml.value = "";
			oDataEntryXslt.value = "";
		}
		var oDataSchema = this.FindElement("DataSchema");
		if (oDataSchema && oDataSchema.value.length > 0)
		{
			this.setContent("dataschema", oDataSchema.value);
			// Clear out value so it is not submitted back to the web server
			oDataSchema.value = "";
		}
	}
	return this.RadEditor_GetHiddenTextareaValue();
}

function EkRadEditor_GetHtml(isFiltered, mode)//isFiltered = true -> goes through filters.
{
    if ("undefined" == typeof mode)
    {
        mode = this.Mode;
    }
    if (mode != RadEditorNamespace.RADEDITOR_PREVIEW_MODE && this.contentCache) 
    {
		return this.contentCache;
	}
	var content = "";
	try
	{
		if (mode == RadEditorNamespace.RADEDITOR_DESIGN_MODE)
		{
			if (this.sfInstance) 
			{
				content = this.sfInstance.getContent();
				//Ektron.ContentDesigner.trace("Content. id=" + this.Id + "\n" + content);
				
				// #33968 - New Editor:Tags are not preserved When adding Html content.
				// If content is "blank", then empty it.
				if (this.SmartForm.isBlankContent(content))
				{
					content = "";
				}
			}
			else
			{
				content = this.GetHiddenTextareaValue();
				Ektron.ContentDesigner.trace("Warning: No SmartForm instance. Getting original content. id=" + this.Id);
		    }
			if (true == isFiltered) 
			{
				content = this.FiltersManager.GetHtmlContent(content);
			}
		}
		else if (mode == RadEditorNamespace.RADEDITOR_PREVIEW_MODE)
		{
		    content = this.designContent;
		}
		else if (mode == RadEditorNamespace.RADEDITOR_HTML_MODE)
		{
			//Always clean the value from indent before returning it!
			this.CleanIndent();
			content = this.ContentTextarea.value;
			content = this.ekXml.fixXml(content); // Ektron: do minimal filtering
			// Ektron: don't filter source. It may be XML data and corrupted by filters.
			// #34911 - "root element has incomplete content", message is given when saving smart form while in source view
			// Do not filter code from source view. Filtering corrupts xml data.
			
			//44317: error when you delete code from html mode and publish the smart form
			if ("dataentry" == this.ekParameters.editMode && "" == content)
			{
			    var errmsg = this.GetLocalizedString("ErrNoRootElem", "Error: Root element is missing.");
			    alert(errmsg);
			    content = null;
			}
		}
	}
	catch (ex)
	{
		// Error "Ektron is not defined" occurs in Firefox when the editor markup is removed via Ajax, but the JavaScript remains.
		if ("Ektron is not defined" == ex.message)
		{
			this.destroyEditor();
		}
		else
		{
			Ektron.OnException(this, Ektron.ContentDesigner.onexception, ex, arguments);
		}
	}
	if (true == isFiltered)
	{
		this.contentCache = content;
	}
	return content;
}

function EkRadEditor_GetText()
{
	return $ektron.removeTags(this.getContent());
}

function EkRadEditor_OnContentError(ex)
{
    var msg = this.GetLocalizedString("sErrOnContent", "An error occurred when processing the content.") + "\n\n" + Ektron.OnException.exceptionMessage(ex);
    alert(msg);
    this.contentCache = null;
}

function EkRadEditor_OnBeforePaste(oEvent)
{
	// #33244 - Error When paste the text in the text box of smartform which has multiple lines options& indexed.(IE6)..
	// telerik code calls range.pasteHTML into textarea field, which causes error. Wrap with try/catch.
	try
	{
		this.RadEditor_OnBeforePaste(oEvent);
	}
	catch (ex)
	{
		// ignore
	}
}

// for Edit In Context
function EkRadEditor_autoheight()
{
	var bindEvents = !this.autoheightActive;
	this.autoheightActive = true;
	if (bindEvents)
	{
		var me = this;
		$ektron(document).bind($ektron.fn.autoheight.triggerName, function()
		{
			var ht = $ektron(me.ContentAreaElement).height();
			$ektron("#RadEWrapper" + me.Id).height(ht);
		});
	}
	if (this.ContentAreaElement) 
	{
		$ektron(this.ContentAreaElement).autoheight({ bindEvents: bindEvents });
	}
	if (this.sfInstance && this.sfInstance.autoheight) 
	{
		this.sfInstance.autoheight();
	}
}

// for Edit In Context
function EkRadEditor_getToolbarFlavor(oManager)
{
	var ToolbarFlavors = oManager.ToolbarFlavors;
	for (var i=0; i < ToolbarFlavors.length; i++)
	{
		if (this == ToolbarFlavors[i].Editor)
		{				
			return ToolbarFlavors[i];
		}
	}
	return null;
}

// for Edit In Context
function EkRadEditor_activateToolbar()
{
	if (true == this.IsToolbarModeEnabled(RadEditorNamespace.ToolbarModesEnum.ShowOnFocus))
	{
		var oManager = RadEditorNamespace.GetShowOnFocusToolbarManager();
		if (oManager)
		{
			var oFlavorObject = this.getToolbarFlavor(oManager);
			oManager.SetEditorFocus(oFlavorObject);
			var oHolder = oManager.ToolbarHolder[this.Id];
			if (oHolder)
			{
				// push the cancel button to the far right
				$ektron(".RadEToolbar", oHolder)
					.css("float", "left")
					.filter(":last")
						.css("float", "right");
			}
		}
	}
}

// for Edit In Context and other Ajax cases
function EkRadEditor_destroyEditor()
{
	Ektron.ContentDesigner.remove(this);
    // close down all popup radwindows
    try
    {
        $ektron("body .RadERadWindowButtonClose").click();
    }
    catch(ex){}
    // remove the toolbar of the InContext editor.
    this.destroyToolbar();
}

// for Edit In Context
function EkRadEditor_destroyToolbar()
{
	if (true == this.IsToolbarModeEnabled(RadEditorNamespace.ToolbarModesEnum.ShowOnFocus))
	{
		var oManager = RadEditorNamespace.GetShowOnFocusToolbarManager();
		if (oManager)
		{
			var oHolder = oManager.ToolbarHolder[this.Id];
			if (oHolder)
			{
				oHolder.parentNode.removeChild(oHolder);
				oManager.ToolbarHolder[this.Id] = null;
			}
			var oOverlay = oManager.OverlayFrame[this.Id];
			if (oOverlay)
			{
				oOverlay.parentNode.removeChild(oOverlay);
				oManager.OverlayFrame[this.Id] = null;
			}
		}
	}
}

function EkRadEditor_isWordContent(content)
{
    if (content.match(/<[^>]+\s(class|style)=("[^"]*mso|[^ >]*mso)/i) || 
		content.match(/<meta[^>]*\scontent="?Word.Document"?/i)) 
	{
	    return true;
	}
	return false;
}

function EkRadEditor_isWebKitBrowserContent(content)
{
    if (content.match(/<[^>]+\sclass=("[^"]*Apple-style-|[^ >]*Apple-style-)/i)) 
	{
	    return true;
	}
	return false;
}

// for Preserving MSWord at Paste
function EkRadEditor_onContentPaste()
{
	if (this.ClearPasteFormatting == RadEditorNamespace.CLEAR_PASTE_FORMATTING_NONE_SUPRESS_MESSAGE) return;
    var currentContent = this.getContent();
    if (this.isWordContent(currentContent))
    {
        this.setContent("document", currentContent);
    }
    else if (this.isWebKitBrowserContent(currentContent))
    {
        this.removeStyles();
    }
//    var dirtyText = "";
//    var cleanedText = "";
//    if (this.ClearPasteFormatting & RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD)
//	{
//		var dirtyText = this.GetHtml(false);
//		cleanedText = RadEditorNamespace.StripFormatting(dirtyText, "WORD");
//		this.setContent("document", cleanedText); 
//	}
//    else if (((RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD_PRESERVE_CLASSES & this.ClearPasteFormatting) == RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD_PRESERVE_CLASSES) 
//        || ((RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD_PRESERVE_STYLES & this.ClearPasteFormatting) == RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD_PRESERVE_STYLES))
//    {
//        dirtyText = this.GetHtml(false);
//        if ((dirtyText.match(/style="[^"]*?mso[^"]*?"/ig) || dirtyText.match(/class="?[^"]*?mso[^"]*?"?/ig)) && confirm(this.Localization['AskWordCleaning']))
//	    {
//            var options = 
//			{
//			    preserveWordClasses: ((RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD_PRESERVE_CLASSES & this.ClearPasteFormatting) == RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD_PRESERVE_CLASSES),
//	            preserveWordStyles: ((RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD_PRESERVE_STYLES & this.ClearPasteFormatting) == RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD_PRESERVE_STYLES)
//			};
//	        cleanedText = RadEditorNamespace.StripFormatting(dirtyText, "WORD", options);
//	        this.setContent(contentType, cleanedText);
//        }
//    }
}

/* ********************************************************************* */

function EkRadOnClientInit(editor)
{
	var params = window["Ek" + editor.Id + "Params"]; // JS to create object is in ContentDesigner.cs
	editor.ekParameters = params;
	editor.workareaPath = editor.ekParameters.workareaPath;
	
	editor.ekXml = new Ektron.Xml({ srcPath:editor.ekParameters.srcPath });

	// Call the user's function.
	if (params.EkRadOnClientInit) editor.AttachClientEvent("EkRadOnClientInit", params.EkRadOnClientInit);
	editor.ExecuteClientEvent("EkRadOnClientInit");
	
	if (params.EkRadOnClientLoad) editor.AttachClientEvent("EkRadOnClientLoad", params.EkRadOnClientLoad);
	if (params.EkRadOnClientCommandExecuting) editor.AttachClientEvent("EkRadOnClientCommandExecuting", params.EkRadOnClientCommandExecuting);
	if (params.EkRadOnClientCommandExecuted)  editor.AttachClientEvent("EkRadOnClientCommandExecuted", params.EkRadOnClientCommandExecuted);
	if (params.EkRadOnClientModeChange) editor.AttachClientEvent("EkRadOnClientModeChange", params.EkRadOnClientModeChange);
	if (params.EkRadOnClientSubmit) editor.AttachClientEvent("EkRadOnClientSubmit", params.EkRadOnClientSubmit);
	if (params.EkRadOnClientCancel) editor.AttachClientEvent("EkRadOnClientCancel", params.EkRadOnClientCancel);
	EkRadEditor.overrides(editor);
	
	Ektron.Class.overrides("RadEditorNamespace", ["CleanPastedContent", "SetElementInnerHTML"]).call(function()
	{
		this.CleanPastedContent = function(editor, dirtyText)
		{
			var cleanedText = this.RadEditorNamespace_CleanPastedContent(editor, dirtyText);
			/*
			#35383 - object expected error when clicking in a certain location in the editor
			Problem is authored JavaScript event handlers running and causing JavaScript errors.
			Changed name of all "on" JavaScript event handler attributes to prevent them from executing.
			Changed back when content is saved.
			source: EkRadEditor.js, ContentInComing.xslt
			version: 7.5.3.09
			changelist: 35251
			It's not perfect b/c it will falsely match " onclick && n > 1"
			*/
			cleanedText = cleanedText.replace(/\b(onabort|onactivate|onafter\w+|onbefore\w+|onblur|onchange|onclick|oncontextmenu|ondblclick|ondeactivate|ondrag\w*|ondrop|onerror\w*|onfocus\w*|onhelp|onkey\w+|onload|onmouse\w+|onmove\w*|onpaste|onreadystatechange|onreset|onresize\w*|onscroll|onselect\w*|onstop|onsubmit|onunload)(?=[^<>]*>)/ig, "ektron35383_$1");
			return cleanedText;
		};
		this.SetElementInnerHTML = function(elem, content)
		{
			if ("string" == typeof content)
			{
				content = content.replace(/\xa0/g, "&#160;"); // non-breaking spaces must be char ref to be preserved
			}
			return this.RadEditorNamespace_SetElementInnerHTML(elem, content); 
		};
	}, RadEditorNamespace); // overrides

	Ektron.Class.overrides("RadSelection", ["IsControl"]).call(function()
	{
		this.IsControl = function()
		{
			try
			{
				return this.RadSelection_IsControl.apply(this, arguments); // pass all arguments
			}
			catch (ex)
			{
				return false;
			}
		};
	}, RadEditorNamespace.RadSelection); // overrides
		
	// Cannot override RadEditorNamespace.RadStyleCommand or any other command factory b/c
	// the u_radEditor__ListOfCommands.js creates the commands before we have a chance
	// to run.
}

function EkRadOnClientLoad(editor)
{
	// Call the user's function.
	return editor.ExecuteClientEvent("EkRadOnClientLoad");
}

function EkRadOnClientCommandExecuting(editor)
{
	// Call the user's function.
	return editor.ExecuteClientEvent("EkRadOnClientCommandExecuting");
}

function EkRadOnClientCommandExecuted(editor)
{
	// Call the user's function.
	return editor.ExecuteClientEvent("EkRadOnClientCommandExecuted");
}

function EkRadOnClientModeChange(editor)
{
	// Call the user's function.
	return editor.ExecuteClientEvent("EkRadOnClientModeChange");
}

function EkRadOnClientSubmit(editor)
{
	// Call the user's function.
	return editor.ExecuteClientEvent("EkRadOnClientSubmit");
}

function EkRadOnClientCancel(editor)
{
	// Call the user's function.
	return editor.ExecuteClientEvent("EkRadOnClientCancel");
}


function EkDataEntryFilter(editor, dataEntryXslt, xsltArgs)
{
    Ektron.Class.inherits(this, editor.filter);
    this.Name = "EkDataEntryFilter"; // arguments.callee.toString().match(/function (\w+)\(/)[1];
    this.IsDom = true;
    this.Description = "Converts between data entry view and XML data view";
    this.mode = "dataentry";

    this.HtmlContentCache = "";

	this.GetHtmlContent = function(content, onexception)
	{
		// content is a dataentrypage
		//Ektron.ContentDesigner.trace(editor.Id + " EkDataEntryFilter_GetHtmlContent PresentationToData");
		if (!content) return "";
		try
		{
			this.HtmlContentCache = content;
			content = this.EkContentFilter_GetHtmlContent(content);
			content = editor.ekXml.xslTransform(content, "[srcPath]PresentationToData.xslt"); 
		}
		catch(ex)
		{
			return Ektron.OnException(this, onexception, ex, arguments);
		}
		return content;
	}
	
	this.GetDesignContent = function(content, onexception)
	{
		// content is datadocumentxml
		//Ektron.ContentDesigner.trace(editor.Id + " EkDataEntryFilter_GetDesignContent dataEntryXslt=" + dataEntryXslt);
		if (dataEntryXslt)
		{
			try
			{
				content = editor.ekXml.xslTransform(content, dataEntryXslt, xsltArgs);
				content = this.EkContentFilter_GetDesignContent(content);
			}
			catch(ex)
			{
				return Ektron.OnException(this, onexception, ex, arguments);
			}
			return content;
		}
		else
		{
			return this.HtmlContentCache;
		}
	}
}

function EkContentFilter(editor)
{
	try
	{
		this.Name = "EkContentFilter"; 
		this.Description = "Converts between WYSIWYG view and source view";
		//this.mode = undefined; use editor.ekParameters.editMode
		var strHost = editor.serverUrl;
		var lenHost = strHost.length;
		var skinPath = editor.ekParameters.skinPath;
		if (strHost.toLowerCase() == skinPath.substr(0,lenHost).toLowerCase())
		{
			skinPath = skinPath.substr(lenHost); // remove strHost from the beginning
		}
		var srcPath = editor.ekParameters.srcPath;
		if (strHost.toLowerCase() == srcPath.substr(0,lenHost).toLowerCase())
		{
			srcPath = srcPath.substr(lenHost); // remove strHost from the beginning
		}
		var args = [
		// assert args[0] is for "mode", see below in two places
		{ name: "mode", value: editor.ekParameters.editMode }, // editMode may change
		{ name: "skinPath", value: skinPath }, // eg, /cms400Developer/workarea/csslib/ContentDesigner/
		{ name: "srcPath", value: srcPath }, // eg, /cms400Developer/workarea/ContentDesigner/
		{ name: "baseURL", value: strHost }, // eg, http://www.ektron.com
		{ name: "LangType", value: editor.ekParameters.contentLanguage },
		{ name:	"sEditPropToolTip", value: editor.GetLocalizedString("sEditPropToolTip", "Edit Field:")}
		];
		
		this.GetHtmlContent = function(content, onexception)
		{
			//Ektron.ContentDesigner.trace(editor.Id + " EkContentFilter_GetHtmlContent ContentOutGoing");
			if (!content) return "";
			try
			{
				if (content.indexOf("<") >= 0)
				{
                    /*
					#35383 - object expected error when clicking in a certain location in the editor
					Problem is authored JavaScript event handlers running and causing JavaScript errors.
					Changed name of all "on" JavaScript event handler attributes to prevent them from executing.
					Changed back when content is saved.
					source: EkRadEditor.js, ContentInComing.xslt
					version: 7.5.3.09
					changelist: 35251
					*/
					content = content.replace(/ektron35383_/g, "");
					content = RadEditorNamespace.StripWordFormatting(content, "WORDML");
					args[0].value = this.mode ? this.mode : editor.ekParameters.editMode;
					content = editor.ekXml.xslTransform(content, "[srcPath]ContentOutGoing.xslt", args);
					var placeholders = editor.filter.placeholders;
					if (placeholders)
					{
						for (var i = 0; i < placeholders.length; i++)
						{
							ph = placeholders[i];
							// each name is unique, so only need to replace once
							content = content.replace("[ektdesignns_placeholder_" + ph.name + "]", ph.value);
						}
					}
				}
			}
			catch(ex)
			{
				return Ektron.OnException(this, onexception, ex, arguments);
			}
			return content; 
		}
		
		this.GetDesignContent = function(content, onexception)
		{
			//Ektron.ContentDesigner.trace(editor.Id + " EkContentFilter_GetDesignContent ContentInComing");
			if (!content) return "";
			try
			{
				if (content.indexOf("<") >= 0)
				{
					var options = 
					{
						preserveWordClasses: ((RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD_PRESERVE_CLASSES & editor.ClearPasteFormatting) == RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD_PRESERVE_CLASSES),
						preserveWordStyles: ((RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD_PRESERVE_STYLES & editor.ClearPasteFormatting) == RadEditorNamespace.CLEAR_PASTE_FORMATTING_WORD_PRESERVE_STYLES)
					};
					var clearValue = "WORDML";
					if (editor.isWordContent(content))
 					{
						// contain Microsoft Office (e.g., Word) content
						clearValue = "WORD";
					}
					content = RadEditorNamespace.StripWordFormatting(content, clearValue, options);
					content = fixSnippets(content, editor);
					
					var onTransformException = Ektron.OnException.throwException;
					var tidyOnException = false;
					if (tidyOnException) 
					{
						onTransformException = function onTransformExceptionHandler(ex, args, callee)
						{
							var html = args[0];
							var xslt = args[1];
							var xsltArgs = args[2];
							var xsltOnException = args[3];
							var errorMessage = null;
							var content = "";
							$ektron.ajax(
							{
								type: "POST",
								async: false,
								url: editor.ekParameters.srcPath + "ekajaxtidy.aspx",
								data: { html: html },
								dataType: "html",
								success: function(data)
								{ 
									data = $ektron.trim(data);
									if (data.indexOf("ekAjaxTidyError") > -1)
									{
										var matchResult = data.match(/<body[^>]*>([\w\W]*?)<\/body>/);
										if (matchResult.length >= 2)
										{
											errorMessage = matchResult[1];
										}
										else
										{
											errorMessage = data;
										}
									}
									else
									{
										content = editor.ekXml.xslTransform(data, xslt, xsltArgs, xsltOnException);
									}
								},
								error: function(xhr)
								{
									errorMessage = "Ajax Error: " + xhr.status + ": " + xhr.statusText;
								}
							});
							if (errorMessage)
							{
								throw new Error("Original Transform Error:\n" + Ektron.OnException.exceptionMessage(ex) + "\n\n\n" +
												"Tidy Error: " + errorMessage);
							}
							// ajax is SYNCHRONOUS
							return content;
						};
					}
	
					args[0].value = this.mode ? this.mode : editor.ekParameters.editMode;
					content = editor.ekXml.xslTransform(content, "[srcPath]ContentInComing.xslt", args, onTransformException);
					editor.filter.placeholders = [];
					content = content.replace(m_reContentInComingPlaceholder, function($0_match, $1_id, $2_content)
					{
						editor.filter.placeholders.push({ name: $1_id, value: $2_content });
						return "";
					});
				}
			}
			catch(ex)
			{
				return Ektron.OnException(this, onexception, ex, arguments);
			}
			return content;
		}
		
		var m_reContentInComingPlaceholder = new RegExp("<ektdesignns_placeholder_(\\w+)>([\\w\\W]*)<\\/ektdesignns_placeholder_\\1>", "g");
		
		// Allow for custom tags that may start with standard HTML tag names
		var m_reHtmlLinkElement = new RegExp("<LINK(?=[\\s\\/>])([^>]*)>\\s*(<\\/LINK>)?", "ig");
		var m_reHtmlStyleElement = new RegExp("<STYLE(?=[\\s\\/>])([^>]*)>([\\w\\W]*?)<\\/STYLE>", "ig");
		var m_reHtmlScriptElement = new RegExp("<SCRIPT(?=[\\s\\/>])([^>]*)>([\\w\\W]*?)<\\/SCRIPT>", "ig");
		var m_reHtmlEmbedElement = new RegExp("<EMBED(?=[\\s\\/>])([^>]*)>\\s*(<\\/EMBED>)?", "ig");
		var m_reHtmlObjectElement = new RegExp("<OBJECT(?=[\\s\\/>])([^>]*)>([\\w\\W]*?)<\\/OBJECT>", "ig");
		var m_reHtmlParamElement = new RegExp("<PARAM(?=[\\s\\/>])([^>]*)>(<\\/PARAM>)?", "ig");
		var m_reHtmlAttributeWithQuotes = new RegExp("\\s(\\w+(?:\:\\w+)?)\\s*\\=\\s*(\"[^\"]*\")", "g");
		var m_reHtmlAttributeWoQuotes = new RegExp("\\s(\\w+(?:\:\\w+)?)\\=([^\"][^\\s>]*)", "g");
		var m_reHtmlAttributeWoValue = new RegExp("\\s(\\w+(?:\:\\w+)?)(?!\\=)", "g");

		function fixSnippets(content, editor) // that is, OBJECT, EMBED, SCRIPT, LINK and STYLE tags
		{
		    if (true == editor.AllowScripts)
		    {
			    content = content.replace(m_reHtmlScriptElement, function($0_match, $1_attrs, $2_content)
			    {
				    var sb = new Ektron.String();
				    sb.append("<script");
				    var bHasType = false;
				    var bHasSrc = false;
				    appendAttributes($1_attrs, sb, function(name, value)
				    {
					    if ("type" == name) 
					    {
						    bHasType = true;
					    }
					    else if ("src" == name) 
					    {
						    bHasSrc = true;
					    }
				    });
				    if (!bHasType)
				    {
					    sb.append(" type=\"text/javascript\"");
				    }
				    if (bHasSrc)
				    {
					    sb.append("></script>");
				    }
				    else
				    {
					    sb.append("><!--\n");
					    $2_content = $2_content.replace(/^\s*<\!--[\-]*\s*/, "").replace(/\s*(\/\/)?\s*[\-]*-->\s*$/, "");
					    sb.append($2_content);
					    sb.append("\n// --></script>");
				    }
				    return sb.toString();
			    });
			}
			else
			{
			    content = content.replace(m_reHtmlScriptElement, "");
			}
			content = content.replace(m_reHtmlEmbedElement, function($0_match, $1_attrs)
			{
				var sb = new Ektron.String();
				sb.append("<embed");
				appendAttributes($1_attrs, sb);
				sb.append("></embed>");
				return sb.toString();;
			});
			content = content.replace(m_reHtmlObjectElement, function($0_match, $1_attrs, $2_content)
			{
				var sb = new Ektron.String();
				sb.append("<object");
				appendAttributes($1_attrs, sb);
				sb.append(">");
				$2_content = $2_content.replace(m_reHtmlParamElement, function($0_match, $1_attrs)
				{
					var sbParam = new Ektron.String();
					sbParam.append("<param");
					appendAttributes($1_attrs, sbParam);
					sbParam.append(" />");
					return sbParam.toString();
				});
				sb.append($2_content);
				sb.append("</object>");
				return sb.toString();
			});
			content = content.replace(m_reHtmlLinkElement, function($0_match, $1_attrs)
			{
				var sb = new Ektron.String();
				sb.append("<link");
				appendAttributes($1_attrs, sb);
				sb.append(" />");
				return sb.toString();;
			});
			content = content.replace(m_reHtmlStyleElement, function($0_match, $1_attrs, $2_content)
			{
				var sb = new Ektron.String();
				sb.append("<style");
				var bHasType = false;
				appendAttributes($1_attrs, sb, function(name, value)
				{
					if ("type" == name) 
					{
						bHasType = true;
					}
				});
				if (!bHasType)
				{
					sb.append(" type=\"text/css\"");
				}
				sb.append(">");
				sb.append($2_content);
				sb.append("</style>");
				return sb.toString();
			});
			return content;
		}
		function appendAttributes(attrs, sb, callback)
		{
			// attributes are removed as processed; corrected attributes are appended to 'sb'
			var bCallback = ("function" == typeof callback);
			attrs = attrs.replace(m_reHtmlAttributeWithQuotes, function($0_match, $1_name, $2_value)
			{
				var name = $1_name.toLowerCase();
				var value = $2_value.replace(/&(?!#|amp;|lt;|gt;|quot;|apos;)/g, "&amp;");
				if (bCallback) callback(name, value);
				sb.append(" ");
				sb.append(name);
				sb.append("=");
				// assert $2_value is wrapped by quotes
				sb.append(value);
				return "";
			});
			attrs = attrs.replace(m_reHtmlAttributeWoQuotes, function($0_match, $1_name, $2_value)
			{
				var name = $1_name.toLowerCase();
				var value = $2_value.replace(/&(?!#|amp;|lt;|gt;|quot;|apos;)/g, "&amp;");
				if (bCallback) callback(name, value);
				sb.append(" ");
				sb.append(name);
				sb.append("=\"");
				// assert $2_value contains no quotes
				sb.append(value);
				sb.append("\"");
				return "";
			});
			attrs = attrs.replace(m_reHtmlAttributeWoValue, function($0_match, $1_name)
			{
				var name = $1_name.toLowerCase();
				var value = name;
				if (bCallback) callback(name, value);
				sb.append(" ");
				sb.append(name);
				sb.append("=\"");
				sb.append(value);
				sb.append("\"");
				return "";
			});
		}
	}
	catch (ex)
	{
		Ektron.OnException(this, Ektron.ContentDesigner.onexception, ex, arguments);
	}
}

function EkRadEditorDomInspector(moduleArgs)
{
    Ektron.Class.inherits(this, new RadEditorDomInspector(moduleArgs), "RadEditorDomInspector");
    
    this.CreatePath = EkRadEditorDomInspector_CreatePath;
}

function EkRadEditorDomInspector_CreatePath()
{
	this.RadEditorDomInspector_CreatePath();
	var links = this.ModuleElement.getElementsByTagName("A");
	if (links)
	{
		for (var i = 0; i < links.length; i++)
		{
			var link = links[i];
			if (link.Parent)
			{
				var oElem = link.Parent.DomElement;
				if (oElem)
				{
					if ("design_content" == oElem.id)
					{
						this.ModuleElement.removeChild(link.nextSibling); // the " > "
						this.ModuleElement.removeChild(link);
					}
				}
			}
		}
	} 			
}

function EkRadEditorXhtmlValidator(moduleArgs)
{
    Ektron.Class.inherits(this, new RadEditorXhtmlValidator(moduleArgs), "RadEditorXhtmlValidator");

    this.Id = "EkRadEditorXhtmlValidator";
    this.Validate = EkRadEditorXhtmlValidator_Validate;
	this.CreateHeader = EkRadEditorXhtmlValidator_CreateHeader;
    this.prepareResultPane = EkRadEditorXhtmlValidator_prepareResultPane;
    this.getActionPage = EkRadEditorXhtmlValidator_getActionPage;
}

function EkRadEditorXhtmlValidator_Validate()
{
	try
	{
		if (this.DoctypeSelect)
		{
			var sHtml = this.Editor.getContent();
			this.ValidateForm.action = this.getActionPage();	
			this.prepareResultPane(); 
			var bCustomSelect = false;
			switch (this.DoctypeSelect.value)
			{
				case "ektaccesseval":
					try
					{         
						//Ektron XSLT Transformation (ektaccesseval.xslt) with Sarissa
						var sPreHtml = "<html><head></head><body>";
						var sPostHtml = "</body></html>";
						sHtml = sPreHtml + sHtml + sPostHtml;
						var baseURL = this.Editor.ekParameters.ImagePaths;
						var args = [ { name: "baseURL", value: "" } ];
						if (-1 == baseURL.indexOf(","))
						{
							baseURL = baseURL.replace(/\~/, this.Editor.ApplicationPath);
							baseURL = baseURL.replace(/\/{2}/, "/");	
							args = [
							{ name: "baseURL", value: this.Editor.serverUrl + baseURL }
							];
						} 
						try
						{
							var newDocument = this.Editor.ekXml.xslTransform(sHtml, "[srcpath]ektaccesseval.xslt", args);
							// The document was parsed/loaded just fine, go on
							if (newDocument.indexOf("<html>") > -1)
							{
								this.ContentField.value = newDocument;
							}
							else
							{
								this.ContentField.value = sPreHtml + "<li>Error in Transform.</li>" + sPostHtml;
							}
						}
						catch (ex)
						{ 
							this.ContentField.value = sPreHtml + "<li>Error in Transform: " + ex.message + "</li>" + sPostHtml;
						}
						this.DoctypeField.value = "";
						bCustomSelect = true;
					}
					catch(ex)
					{
					}
					break;
				case "schema":
					var err = this.Editor.validateDesign();
					var doctype = "";
					if (err != null)
					{
						if ("object" == typeof err && typeof err.msg != "undefined") 
						{
							doctype = err.doctype;
							err = err.msg;
						}
						else if ("object" == typeof err)
						{
							err = "<ul><li>" + err.join("</li><li>") + "</li></ul>";
						}
						else
						{
							err = "<ul><li>" + err + "</li></ul>";
						}
					}
					else
					{
						err = "";
					}
					this.ContentField.value = err;
					this.DoctypeField.value = doctype;
					bCustomSelect = true;
					break;
				case "xmlcontent":
					var err = this.Editor.validateXmlContent();
					var doctype = "";
					if (err != null)
					{
						if ("object" == typeof err && typeof err.msg != "undefined") 
						{
							doctype = err.doctype;
							err = err.msg;
						}
						else if ("object" == typeof err)
						{
							err = "<ul><li>" + err.join("</li><li>") + "</li></ul>";
						}
						else
						{
							err = "<ul><li>" + err + "</li></ul>";
						}
					}
					else
					{
						err = "";
					}
					this.ContentField.value = err;
					this.DoctypeField.value = doctype;
					bCustomSelect = true;
					break; 
				default:
					// these are the validators available online.
					if ("!DOCTYPE " == this.DoctypeSelect.value.substr(0, 9).toUpperCase())
					{
						// online validator.W3.org, the original ones supported by RadEditor.
						this.RadEditorXhtmlValidator_Validate(); // call base class method
						bCustomSelect = false;
					}
					else
					{
						if ("reportresult=" ==this.DoctypeSelect.value.substr(0, 13))
						{
							//ContentField.value should already been set. So, the report pane is 
							//just displaying the report without doing the POST again.
							this.DoctypeField.value = this.DoctypeSelect.value;
						}
						else
						{
							// other custom online validation specified in ValidateSpec.xml
							this.ContentField.value = "<div>" + sHtml + "</div>";
							this.DoctypeField.value = this.DoctypeSelect.value;
						}
						bCustomSelect = true;
					}
					break;
			}
			if (true == bCustomSelect)
			{
				this.ValidateForm.submit();   
			}
		}
	}
	catch (ex)
	{
		Ektron.OnException(this, Ektron.ContentDesigner.onexception, ex, arguments);
	}
}

function EkRadEditorXhtmlValidator_CreateHeader()
{
	try
	{
		this.RadEditorXhtmlValidator_CreateHeader(); // call base class method

		this.ValidateButton.value = this.GetLocalizedString("ValidateContent", "Validate Content");
	}
	catch (ex)
	{
		Ektron.OnException(this, Ektron.ContentDesigner.onexception, ex, arguments);
	}
}

function EkRadEditorXhtmlValidator_prepareResultPane() 
{
    this.ShowIframe(true);    		
    if (this.ToggleCheckbox && !this.ToggleCheckbox.checked) this.ToggleCheckbox.checked = true;
}

function EkRadEditorXhtmlValidator_getActionPage() 
{
    return this.Editor.serverUrl + this.Editor.RadControlsDir + "Editor/Xhtml/XhtmlValidator.aspx";	
}

function EkRadEditorSpellEngineUI(editor)
{
    Ektron.Class.inherits(this, new RadEditorSpellEngineUI(editor), "RadEditorSpellEngineUI");

    this.ClearWrongWords = EkRadEditorSpellEngineUI_ClearWrongWords;
    this.MoveToNextWrongWord = EkRadEditorSpellEngineUI_MoveToNextWrongWord;
    this.IsHighlightedRemaining = EkRadEditorSpellEngineUI_IsHighlightedRemaining;
    this.LoopIntoIFrames = EkRadEditorSpellEngineUI_LoopIntoIFrames;
}

function EkRadEditorSpellEngineUI_ClearWrongWords(wrongWord, correctWord)
{ 
    this.LoopIntoIFrames( function (obj)
    {
	    obj.RadEditorSpellEngineUI_ClearWrongWords(wrongWord, correctWord); // call base class method
    });
}

function EkRadEditorSpellEngineUI_MoveToNextWrongWord()
{
    this.LoopIntoIFrames( function (obj)
    {
        var oMatch = null;
	    oMatch = obj.RadEditorSpellEngineUI_MoveToNextWrongWord();
        if (oMatch)
        {
            return false;
        }
    });
}

function EkRadEditorSpellEngineUI_IsHighlightedRemaining()
{
    var IsHighlightedRemaining = false;
    this.LoopIntoIFrames( function (obj)
    {
        IsHighlightedRemaining = obj.RadEditorSpellEngineUI_IsHighlightedRemaining(); // call base class method
        if (true == IsHighlightedRemaining)
        {
            return false;
        }
    });
    return IsHighlightedRemaining;
}

function EkRadEditorSpellEngineUI_LoopIntoIFrames(coreFunction)
{
    var oIFrame = $ektron("iframe.contenteditable", this.Editor.getContentElement());
    if (oIFrame && oIFrame.length > 1)			
    {
        // remember the editor settings for later
        var tmpContentWindow = this.Editor.ContentWindow;
        var tmpDocument = this.Editor.Document;
        var tmpContentArea = this.Editor.ContentArea;

        // loop thru' each iframe to find the first wrong words
        var oEngine = this;
        oIFrame.each( function ()
        {
            oEngine.Editor.ContentWindow = this.contentWindow;
            oEngine.Editor.Document = oEngine.Editor.ContentWindow.document;
            oEngine.Editor.ContentArea = oEngine.Editor.Document.body;
		    
		    if (false === coreFunction(oEngine))
		    {
		        return false;
		    }
        });
        // restore the editor settings
        this.Editor.ContentWindow = tmpContentWindow;
		this.Editor.Document = tmpDocument; 
        this.Editor.ContentArea = tmpContentArea;
    }
    else
    {	
		coreFunction(this);
    }
}

