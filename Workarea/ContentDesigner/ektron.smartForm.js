/// <reference path="ektron.js" />
/// <reference path="ektron.xml.js" />
/* Copyright 2003-2010, Ektron, Inc. */

if (!Ektron.SmartForm) (function()
{
    Ektron.SmartForm = function SmartForm(containingElement, settings)
    {
        // containingElement may be element name (string) or object reference
        settings = $ektron.extend(
		{
		    srcPath: "",
		    skinPath: "",
		    editorSkinPath: "", // for radEditor skin files
		    localizedStrings: {},
		    langType: "", // LCID (decimal, not hex)
		    CssFilesArray: [],
		    enableExpandCollapse: true,
		    onexception: null
		}, settings);
        settings.skinPath = settings.skinPath || settings.srcPath;
        settings.editorSkinPath = settings.editorSkinPath || settings.skinPath || settings.srcPath;
        settings.langType = settings.langType + ""; // ensure as string

        this.settings = settings;
        this.onexception = settings.onexception;
        if ("string" == typeof containingElement)
        {
            this.id = containingElement;
        }
        else if ("object" == typeof containingElement && containingElement)
        {
            this.id = containingElement.id;
        }

        this.state = "";
        if (!s_ekXml) s_ekXml = new Ektron.Xml({ srcPath: settings.srcPath });
        Ektron.SmartForm.ekXml = s_ekXml;

        this.init = SmartForm_init;
        this.onload = SmartForm_onload;
        this.onbeforesave = SmartForm_onbeforesave;
        this.onaftersave = SmartForm_onaftersave;
        this.localizeString = function(id, defaultString) { return this.settings.localizedStrings[id] || defaultString; };
        this.localizeFormData = SmartForm_localizeFormData;
        this.getSelectedField = SmartForm_getSelectedField;
        this.setSelectedField = SmartForm_setSelectedField;
        this.getContent = SmartForm_getContent;
        this.getContentElement = Ektron.SmartForm.getContentElement; // dual static and instance method
        this.validateForm = Ektron.SmartForm.validateForm;  // dual static and instance method
        // reserved: this.validateContent which should call validateForm and validate against schema
        this.enableExpandCollapse = SmartForm_enableExpandCollapse;
        this.toggleExpandCollapse = SmartForm_toggleExpandCollapse;
        this.isRootLocation = SmartForm_isRootLocation;
        this.autoheight = SmartForm_autoheight;
        this.qualifySrcPath = SmartForm_qualifySrcPath;
        this.unqualifySrcPath = SmartForm_unqualifySrcPath;
        this.replaceSrcPath = SmartForm_replaceSrcPath;

        // static methods available in instance for convenience
        this.isFieldButton = Ektron.SmartForm.isFieldButton;
        this.isFieldLink = Ektron.SmartForm.isFieldLink;
        this.isDDFieldElement = Ektron.SmartForm.isDDFieldElement;
        this.isCustomTag = Ektron.SmartForm.isCustomTag;
        this.selectElement = Ektron.SmartForm.selectElement;
        this.getRichAreaParent = Ektron.SmartForm.getRichAreaParent;

        this.repeater = new Ektron.SmartForm.Repeater(this.settings);

        if (null == document.getElementById(this.id))
        {
            // Caused by programming error or server control that registered this JS, but was not actually rendered.
            this.state = "error-NoContainingElement";
            Ektron.SmartForm.onexception(new RangeError("Failed to find the Smart Form containing element by id. ID = " + this.id), arguments);
            return;
        }
        var bOK = true;
        if (1 == Ektron.SmartForm.instances.length)
        {
            // Check the first one to ensure it has class too.
            var prevId = Ektron.SmartForm.instances[0].id;
            var prevInstance = $ektron("#" + prevId);
            if (1 == prevInstance.length)
            {
                bOK = bOK && prevInstance.hasClass("design_content");
            }
            else
            {
                // Previous instance no longer exists (probably due to Ajax)
                Ektron.SmartForm.instances = [];
            }
        }
        if (Ektron.SmartForm.instances.length > 0 || this.id != "design_content")
        {
            bOK = bOK && $ektron("#" + this.id).hasClass("design_content");
            if (!bOK) throw new Error("Smart Form element must have class 'design_content'.");
        }
        if (Ektron.SmartForm.instances[this.id])
        {
            // Caused by programming error or multiple Ajax requests for the same ID.
            Ektron.SmartForm.onexception(new RangeError("An instance of SmartForm with this id already exists. ID = " + this.id));
        }
        Ektron.SmartForm.instances[Ektron.SmartForm.instances.length] = this;
        Ektron.SmartForm.instances[this.id] = this;

        this.init(containingElement, settings);
    }; // constructor
    Ektron.SmartForm.onexception = Ektron.OnException.diagException;
    Ektron.SmartForm.instances = [];

    Ektron.SmartForm.create = function(containingElement, settings)
    {
        var id = "design_content";
        if ("string" == typeof containingElement)
        {
            id = containingElement;
        }
        else if ("object" == typeof containingElement && containingElement)
        {
            id = containingElement.id;
        }
        if (null == document.getElementById(id))
        {
            // Caused by programming error or server control that registered this JS, but was not actually rendered.
            Ektron.SmartForm.onexception(new RangeError("Failed to find the Smart Form containing element by id. ID = " + id), arguments);
            return null;
        }
        var objInstance = Ektron.SmartForm.instances[id];
        if (objInstance)
        {
            objInstance.init(containingElement, settings);
        }
        else
        {
            objInstance = new Ektron.SmartForm(containingElement, settings);
        }
        return objInstance;
    };

    function SmartForm_init(settings)
    {
        $ektron.extend(this.settings, settings);
        this.designMode = false;
        this.selectedField = null;
        this.isRootLoc = false;
        Ektron.SmartForm.cache_xmlDocument = ""; // ideally would be instance member
        Ektron.SmartForm.prevalidatingForm = false; // ideally would be instance member

        var oElem = document.getElementById("dsg_validate"); // ideally would be per instance
        if (!oElem)
        {
            oElem = document.createElement("div");
            oElem.id = "dsg_validate";
            oElem.style.display = "none";
            document.body.appendChild(oElem);
        }

        for (var i = 0; i < Ektron.SmartForm.docEvents.length; i++)
        {
            $ektron(document).bind(Ektron.SmartForm.docEvents[i].name, Ektron.SmartForm.docEvents[i].fnHandler);
        }

        this.state = "initialized";
    }

    var s_ekXml = null;

    Ektron.SmartForm.findContentElement = function SmartForm_findContentElement(oElem)
    {
        var containingElement = $ektron(oElem).closest(".design_content").get(0);
        if (!containingElement) containingElement = Ektron.SmartForm.getContentElement();
        if (!containingElement) return null;
        return containingElement;
    };

    Ektron.SmartForm.findByChildElement = function SmartForm_findByChildElement(oElem)
    {
        var n = Ektron.SmartForm.instances.length;
        if (1 == n) return Ektron.SmartForm.instances[0];
        if (0 == n) return null;
        var containingElement = Ektron.SmartForm.findContentElement(oElem);
        if (!containingElement) return null;
        return Ektron.SmartForm.instances[containingElement.id];
    };

    function SmartForm_onload(settings)
    {
        try
        {
            if (settings) $ektron.extend(this.settings, settings);
            this.settings.langType = this.settings.langType + ""; // ensure as string

            this.repeater.hideContextMenu();
            if (this.watchdogTimer)
            {
                clearInterval(this.watchdogTimer);
                this.watchdogTimer = null;
            }
            var oContentElem = this.getContentElement(this.id);
            if (oContentElem)
            {
                this.designMode = $ektron(oContentElem).hasClass("design_mode_design");
                var oFormElem = this.getContentElement(); // in case of iframe
                this.qualifySrcPath(oFormElem);
                Ektron.SmartForm.fixMinHeight(oFormElem);
                Ektron.DataListManager.init({ srcPath: this.settings.srcPath, langType: this.settings.langType, ekXml: s_ekXml });
                Ektron.DataListManager.replaceDataLists(oFormElem);
                if (this.designMode)
                {
                    $ektron("TABLE[ektdesignns_name]").addClass("show_design_border");
                }
                else
                {
                    $ektron("TABLE[ektdesignns_name]").removeClass("show_design_border");
                    this.enableExpandCollapse(true, oFormElem);
                    this.localizeFormData(oFormElem);
                    this.repeater.addShortCuts(oFormElem);
                    this.repeater.addMinElements(oFormElem);
                    Ektron.SmartForm.precalcForm(oFormElem);
                    if (this.settings.prevalidate) Ektron.SmartForm.prevalidateForm(oFormElem);
                }
                Ektron.SmartForm.fixContentEditable(oContentElem); // async, must be last
                this.state = "loaded";
            }
        }
        catch (ex)
        {
            Ektron.OnException(this, Ektron.OnException.ignoreException, ex, arguments);
        }
    }

    function SmartForm_onbeforesave()
    {
        try
        {
            if (this.state != "loaded") return;
            this.repeater.hideContextMenu();
            if (this.watchdogTimer)
            {
                clearInterval(this.watchdogTimer);
                this.watchdogTimer = null;
            }
            var oFormElem = this.getContentElement(this.id);
            if (oFormElem)
            {
                this.setSelectedField(null);
                Ektron.SmartForm.unfixContentEditable(oFormElem);
                Ektron.SmartForm.setFormValues(oFormElem);
                Ektron.SmartForm.recalcForm(oFormElem);
                if (!this.designMode)
                {
                    this.enableExpandCollapse(false, oFormElem);
                }
                Ektron.SmartForm.unfixMinHeight(oFormElem);
                this.unqualifySrcPath(oFormElem);
            }
            this.state = "saving";
        }
        catch (ex)
        {
            Ektron.OnException(this, Ektron.OnException.ignoreException, ex, arguments);
        }
    }

    function SmartForm_onaftersave()
    {
        try
        {
            if (this.state != "saving") return;
            var oFormElem = this.getContentElement();
            if (oFormElem)
            {
                this.qualifySrcPath(oFormElem);
                Ektron.SmartForm.fixMinHeight(oFormElem);
                if (!this.designMode)
                {
                    this.enableExpandCollapse(true, oFormElem);
                }
                var oContentElem = this.getContentElement(this.id);
                Ektron.SmartForm.fixContentEditable(oContentElem); // async, must be last
            }
            this.state = "loaded";
        }
        catch (ex)
        {
            Ektron.OnException(this, Ektron.OnException.ignoreException, ex, arguments);
        }
    }

    function SmartForm_localizeFormData(containingElement)
    {
        var oFormElem = this.getContentElement(containingElement);
        if (!oFormElem) return;
        var me = this;
        // Look for date fields to localize the iso date.
        // <span class="design_calendar" datavalue="2004-04-16"><input value=""/></span>
        $ektron(".design_calendar", oFormElem).each(function()
        {
            try
            {
                // Needed for Opera b/c "value" is interpreted as a number and not a string, eg, "2007-01-09" is read as "2007"
                var strDate = this.getAttribute("datavalue");
                if (null == strDate || "undefined" == typeof strDate)
                {
                    strDate = this.getAttribute("value"); // legacy content uses "value", not "datavalue"
                }
                if ("string" == typeof strDate)
                {
                    var joElem = $ektron(this).find("input");
                    // ISO-8601 format: CCYY-MM-DD
                    var oTempDate = Ektron.Xml.parseDate(strDate);
                    if (oTempDate != null)
                    {
                        joElem.val(oTempDate.toLocaleDateString());
                    }
                    else
                    {
                        joElem.val(strDate); // error in date format
                    }
                }
            }
            catch (ex)
            {
                Ektron.OnException(me, Ektron.OnException.ignoreException, ex, arguments);
            }
        });
    }

    function SmartForm_getContent(getType)
    {
        var content = "";
        switch (getType)
        {
            case "datadocumentxml":
                content = this.getContent();
                content = Ektron.SmartForm.ekXml.xslTransform(content, "[srcPath]PresentationToData.xslt");
                break;
            default:
                this.onbeforesave();
                var containingElement = this.getContentElement();

                // #54244, #53528, #55372 - rich text style width and height attributes interfere with the auto height scripts.
                if (true == this.designMode)
                {
                    $ektron("div.ektdesignns_richarea", containingElement).css("width", "").css("height", "");
                }
                //#59691 - At this line, the code really expects the containingElement to get the content. However, sometime the
                // div.design_mode_design#design_content is deleted. Putting a space there as the returning content would re-create  
                // the containingElement div back in the editor when the editor is switching back to design mode.
                if (containingElement)
                {
                    content = Ektron.Xml.serializeXhtml(containingElement.childNodes);
                }
                else
                {
                    content = "&#160;";
                }
                this.onaftersave();
                break;
        }
        return content;
    }

    Ektron.SmartForm.getContentElement = function SmartForm_getContentElement(containingElement)
    // containingElement may be undefined, element name (string) or object reference
    {
        var oElem = null;
        if (!containingElement)
        {
            oElem = document.getElementById(this.id || "design_content");
            if (Ektron.SmartForm.isEditWithIFrame(oElem))
            {
                oElem = oElem.firstChild.contentWindow.document.body;
            }
            else if (Ektron.SmartForm.isEditWithIFrameCache(oElem))
            {
                oElem = oElem.firstChild.nextSibling;
            }
        }
        else if ("object" == typeof containingElement && containingElement != null)
        {
            oElem = containingElement;
        }
        else if ("string" == typeof containingElement && containingElement.length > 0)
        {
            oElem = document.getElementById(containingElement);
        }
        else
        {
            oElem = document.getElementById(this.id || "design_content");
        }
        return oElem;
    };


    function SmartForm_enableExpandCollapse(bEnable, containingElement)
    {
        if (!this.settings.enableExpandCollapse) return;
        var oElement = this.getContentElement(containingElement);
        if (!oElement) return;

        if (bEnable)
        {
            var sHtml = '<a class="design_view_button" href="#" onclick="design_toggleExpandCollapse(this);return false;"><img class="design_view_button" height="11" width="11" border="0" alt="' + this.localizeString("sHide", "Hide") + '" title="' + this.localizeString("sHide", "Hide") + '" src="' + this.settings.skinPath + 'viewcollapsed.gif" /></a>';
            $ektron("legend:not(:has(a.design_view_button))").prepend(sHtml);
        }
        else
        {
            $ektron("legend > a.design_view_button").remove();
        }
    }

    function SmartForm_toggleExpandCollapse(oThis)
    {
        try
        {
            if (!oThis) return;
            var joThis = $ektron(oThis);
            if (-1 == oThis.firstChild.src.indexOf("viewexpanded.gif"))
            {
                oThis.firstChild.src = this.settings.skinPath + "viewexpanded.gif";
                oThis.firstChild.alt = this.localizeString("sShow", "Show");
                oThis.firstChild.title = this.localizeString("sShow", "Show");
                joThis.parent().next().hide();
            }
            else
            {
                oThis.firstChild.src = this.settings.skinPath + "viewcollapsed.gif";
                oThis.firstChild.alt = this.localizeString("sHide", "Hide");
                oThis.firstChild.title = this.localizeString("sHide", "Hide");
                joThis.parent().next().show();
            }
        }
        catch (ex)
        {
            Ektron.OnException(this, Ektron.OnException.ignoreException, ex, arguments);
        }
    }

    function SmartForm_isRootLocation()
    {
        return this.isRootLoc;
    }

    Ektron.SmartForm.isDDFieldElement = function SmartForm_isDDFieldElement(oElem)
    {
        if (!oElem) return false;
        if (oElem.nodeType != 1/*NODE_ELEMENT*/) return false;
        var name = oElem.getAttribute("ektdesignns_name");
        if ("string" == typeof name && name.length > 0) return true;
        var bind = oElem.getAttribute("ektdesignns_bind");
        if ("string" == typeof bind && bind.length > 0) return true;
        return false;
    };

    Ektron.SmartForm.isCustomTag = function SmartForm_isCustomTag(oElem)
    {
        if (!oElem) return false;
        if (!oElem.getAttribute) return false;
        var display = oElem.getAttribute("ektdesignns_display");
        if ("string" == typeof display && display.length > 0) return true;
        return false;
    };

    Ektron.SmartForm.isFieldButton = function SmartForm_isFieldButton(oElem)
    {
        if (!oElem) return false;
        return ("IMG" == oElem.tagName && $ektron(oElem).hasClass("design_fieldbutton"));
    };

    Ektron.SmartForm.isFieldLink = function SmartForm_isFieldLink(oElem)
    {
        if (!oElem) return false;
        if (oElem.tagName != "A") return false;
        var parent = oElem.parentNode;
        if (!parent || false == Ektron.SmartForm.isDDFieldElement(parent)) return false;
        var eFieldBtn = $ektron(oElem).siblings("IMG");
        if (eFieldBtn.length > 0)
        {
            var bFieldLink = false;
            $ektron.each(eFieldBtn, function()
            {
                if (eFieldBtn.hasClass("design_fieldbutton"))
                {
                    bFieldLink = true;
                    return false; // out of each
                }
            });
            return bFieldLink;
        }
        return false;
    };

    Ektron.SmartForm.getFieldFromFieldButton = function SmartForm_getFieldFromFieldButton(oElem)
    {
        // assert(Ektron.SmartForm.isFieldButton(oElem));
        var elemField = null;
        var eField = $ektron(oElem).parents().each(function()
        {
            if ($ektron(this).attr("ektdesignns_name") || $ektron(this).attr("ektdesignns_bind"))
            {
                elemField = this;
                return false; // break
            }
        });
        return elemField;
    };

    Ektron.SmartForm.getRichAreaParent = function SmartForm_getRichAreaParent(oElem)
    {
        if (!oElem) return null;
        var eRichArea = $ektron(oElem).closest(".design_richarea");
        if (1 == eRichArea.length)
        {
            return eRichArea.get(0); //BODY tag for FF and DIV tag for IE
        }
        return null;
    }

    function SmartForm_getSelectedField()
    {
        if (null == this.selectedField)
        {
            var oElem = this.getContentElement();
            var thisField;
            //#58308: the selection are shifted to the parent blocking tag.
            var parentField = getSelectionElement(oElem.ownerDocument);
            if ($ektron(parentField).hasClass("design_selected_field"))
            {
                thisField = parentField;
            }
            else
            {
                $ektron("*[ektdesignns_name]", parentField).each(function()
                {
                    var eThis = $ektron(this);
                    if (eThis.hasClass("design_selected_field"))
                    {
                        thisField = eThis.get(0);
                        return false;
                    }
                });
            }
            if (Ektron.SmartForm.isDDFieldElement(thisField) || Ektron.SmartForm.isCustomTag(thisField))
            {
                return thisField;
            }
            else
            {
                var oRichAreaParent = Ektron.SmartForm.getRichAreaParent(thisField);
                if (oRichAreaParent != null)
                {
                    return oRichAreaParent;
                }
            }
        }
        else if (null == this.selectedField.ownerDocument || (this.selectedField.id != "" && null == this.selectedField.ownerDocument.getElementById(this.selectedField.id)))
        {
            this.selectedField = null;
        }
        else if (null == this.selectedField.parentNode || /* DOCUMENT_FRAGMENT_NODE */11 == this.selectedField.parentNode.nodeType)
        {
            this.selectedField = null;
        }
        return this.selectedField;
    }

    function SmartForm_setSelectedField(oElem)
    {
        if (oElem)
        {
            this.setSelectedField(null);
            var joElem = $ektron(oElem);
            if (this.designMode && Ektron.SmartForm.isFieldButton(oElem))
            {
                oElem = Ektron.SmartForm.getFieldFromFieldButton(oElem);
            }
            this.selectedField = oElem;
            if (this.designMode)
            {
                $ektron(this.selectedField).addClass("design_selected_field");
            }
        }
        else if (this.selectedField != null) // null == oElem
        {
            $ektron(this.selectedField).removeClass("design_selected_field");
            this.selectedField = null;
        }
        else if (null == oElem)
        {
            $ektron(".design_selected_field").removeClass("design_selected_field");
        }
    }

    Ektron.SmartForm.selectElement = function SmartForm_selectElement(targetElement, selectedField)
    {
        //setting the correct selection, codes are extracted from SelectElement() in 1radEditorUtils.js
        var doc = ("#document" == targetElement.nodeName ? targetElement : targetElement.ownerDocument);
        var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
        var range;
        if (doc.selection && !win.opera) //IE 
        {
            switch (selectedField.tagName)
            {
                case "TABLE":
                case "IMG":
                case "HR":
                case "INPUT":
                case "SELECT":
                case "TEXTAREA":
                    range = doc.body.createControlRange();
                    range.add(selectedField);
                    break;
                case "UL":
                case "OL":
                    //TEKI - Make sure you select the list!
                    range = doc.body.createTextRange();
                    range.moveToElementText(selectedField);

                    var parEl = range.parentElement();
                    if (parEl.tagName != "UL" || parEl.tagName != "OL")
                    {
                        range.moveEnd("character", -1);
                    }
                    break;
                /*
                case "A":									
                range = doc.body.createTextRange();
					
					range.moveToElementText(selectedField);
                var parEl = range.parentElement();
					
					if (parEl.tagName!= "A")
                {						
                range.moveEnd("character", 1);					
                range.moveStart("character", 1);						
                range.moveEnd("character", -1);
                }
                //alert ("Is it A?" + range.parentElement().tagName);
                if (range.parentElement().tagName == "A") 
                {						
                range.select();
                //alert ("Selected the range!" + range.parentElement().outerHTML);
                }
					
					var newRange = doc.selection.createRange();
                newRange.setEndPoint("StartToStart",range);
                newRange.setEndPoint("EndToEnd",range);
                newRange.select();
                alert (newRange.parentElement().outerHTML);
                alert("2 " + doc.selection.createRange().parentElement().outerHTML);
                break;
                */ 
                default:
                    range = doc.body.createTextRange();
                    range.moveToElementText(selectedField); //does not work in IE8
                    break;
            }
            if (range)
            {
                range.select();
            }
        }
        else if (win.getSelection) // Mozilla
        {
            range = doc.createRange();
            range.selectNode(selectedField);

            var selection = win.getSelection();
            selection.removeAllRanges();
            selection.addRange(range);
        }
    };

    // these event handlers are applied to the document object
    Ektron.SmartForm.docEvents = [
	 { name: "mouseup", fnHandler: function(event) { Ektron.SmartForm.raiseEventforIE9(event, "onIE9MouseUp"); } }
    , { name: "mousedown", fnHandler: function(event) { Ektron.SmartForm.raiseEventforIE9(event, "onIE9MouseDown"); } }
    , { name: "keyup", fnHandler: function(event) { Ektron.SmartForm.raiseEventforIE9(event, "onIE9KeyUp"); } }
    , { name: "contextmenu", fnHandler: function(event) { Ektron.SmartForm.raiseEventforIE9(event, "onIE9ContextMenu"); } }
    , { name: "click", fnHandler: function(event) { Ektron.SmartForm.setFocusedElement(event); } }
	, { name: "keydown", fnHandler: function(event)
	{
	    Ektron.SmartForm.raiseEventforIE9(event, "onIE9KeyDown");
	    event = (event ? event : window.event);
	    if (13 == event.keyCode && !event.ctrlKey && !event.shiftKey && !event.altKey)
	    {
	        var targetElement = (event.target ? event.target : event.srcElement);
	        if (targetElement && targetElement.nodeType != 1 /*Node.ELEMENT_NODE*/) targetElement = targetElement.parentNode;
	        if (targetElement && targetElement.nodeType != 1 /*Node.ELEMENT_NODE*/) targetElement = null;
	        if (!targetElement) return;
	        var sf = Ektron.SmartForm.findByChildElement(targetElement);
	        if (!sf) return;
	        if (!sf.designMode)
	        {
	            Ektron.SmartForm.setFocusedElement(event);
	        }
	    }
	    else if (9 == event.keyCode && !event.ctrlKey && !event.shiftKey && !event.altKey) // TAB into the editing area for section 508
	    {
	        var targetElement = (event.target ? event.target : event.srcElement);
	        if (!targetElement) return;
	        var bFirstElem = false;
	        var eElem;
	        if ("HTML" == targetElement.tagName)
	        {
	            eElem = $ektron("div.design_mode_entry", targetElement).find("*[ektdesignns_name]:first");
	            targetElement = eElem.get(0);
	            bFirstElem = true;
	        }
	        var sf = Ektron.SmartForm.findByChildElement(targetElement);
	        if (!sf) return;
	        if (!sf.designMode && $ektron.browser.mozilla)
	        {
	            if (bFirstElem)
	            {
	                $ektron("*[tabindex]:first", eElem).focus();
	                eElem.focus();
	            }
	            //cancel event 
	            event.cancelBubble = true;
	        }
	    }
	}
	}
	];
    Ektron.SmartForm.raiseEventforIE9 = function SmartForm_raiseEventforIE9(event, eventName)
    {
        if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) >= 9) //#61003
        {
            event = (event ? event : window.event);
            var targetElement = (event.target ? event.target : event.srcElement);
            if (targetElement && targetElement.nodeType != 1 /*Node.ELEMENT_NODE*/) targetElement = targetElement.parentNode;
            if (targetElement && targetElement.nodeType != 1 /*Node.ELEMENT_NODE*/) targetElement = null;
            if (!targetElement) return;
            var sf = Ektron.SmartForm.findByChildElement(targetElement);
            if (!sf) return;
            if (!sf.settings.editorId) return;
            var doc = ("#document" == targetElement.nodeName ? targetElement : targetElement.ownerDocument);
            var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
            Ektron.ContentDesigner.raiseEvent(win, eventName, [event, win, targetElement, sf.settings]);
        }
    }
    Ektron.SmartForm.setFocusedElement = function SmartForm_setFocusedElement(event)
    {
        Ektron.SmartForm.raiseEventforIE9(event, "onIE9BeforeEditFocus");
        event = (event ? event : window.event);
        var targetElement = (event.target ? event.target : event.srcElement);
        if (targetElement && targetElement.nodeType != 1 /*Node.ELEMENT_NODE*/) targetElement = targetElement.parentNode;
        if (targetElement && targetElement.nodeType != 1 /*Node.ELEMENT_NODE*/) targetElement = null;
        if (!targetElement) return;
        var sf = Ektron.SmartForm.findByChildElement(targetElement);
        if (!sf) return;
        var selectedField = null;
        var editField = null;
        if (sf.designMode)
        {
            sf.setSelectedField(null);
            var bDone = false;
            var oElem = targetElement;
            while (!bDone && oElem && oElem.tagName != "BODY")
            {
                switch (oElem.tagName)
                {
                    case "INPUT":
                    case "SELECT":
                    case "BUTTON":
                        if (Ektron.SmartForm.isDDFieldElement(oElem))
                        {
                            selectedField = oElem;
                            bDone = true;
                        }
                        else
                        {
                            oElem = oElem.parentNode;
                        }
                        break;
                    case "TABLE":
                    case "FIELDSET":
                    case "TEXTAREA":
                        if (Ektron.SmartForm.isDDFieldElement(oElem) || Ektron.SmartForm.isCustomTag(oElem))
                        {
                            selectedField = oElem;
                        }
                        else if ($ektron(oElem).parent().hasClass("ektdesignns_mergelist"))
                        {
                            oElem = oElem.parentNode;
                            selectedField = oElem;
                        }
                        bDone = true;
                        break;
                    case "TD":
                    case "TH":
                        if ("false" == oElem.getAttribute("contenteditable"))
                        {
                            oElem = oElem.parentNode;
                        }
                        else
                        {
                            bDone = true;
                        }
                        break;
                    case "DIV":
                    case "SPAN":
                        var eElem = $ektron(oElem);
                        if (eElem.hasClass("design_edit_fieldprop"))
                        {
                            var selectedId = eElem.attr("data-ektron-forfield");
                            editField = oElem.ownerDocument.getElementById(selectedId);
                            bDone = true;
                        }
                        else if (Ektron.SmartForm.isDDFieldElement(oElem))
                        {
                            selectedField = oElem;
                            bDone = true;
                        }
                        else if ("design_membrane" == oElem.className.toLowerCase())
                        {
                            bDone = true;
                        }
                        else
                        {
                            oElem = oElem.parentNode;
                        }
                        break;
                    case "LABEL":
                        var id = oElem.htmlFor;
                        oElem = oElem.ownerDocument.getElementById(id);
                        break;
                    case "IMG":
                    case "A":
                        if (false == Ektron.SmartForm.isDDFieldElement(oElem))
                        {
                            selectedField = oElem;
                            bDone = true;
                        }
                        break;
                    default:
                        oElem = oElem.parentNode;
                        break;
                } // switch
            } // while

            sf.isRootLoc = false;
            if (!bDone)
            {
                sf.isRootLoc = true;
            }
            else if (oElem)
            {
                sf.isRootLoc = (0 == $ektron(oElem).parents("[ektdesignns_name]").length);
            }

            var doc = ("#document" == targetElement.nodeName ? targetElement : targetElement.ownerDocument);
            var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
            if (selectedField)
            {
                sf.setSelectedField(selectedField);
                selectedField = sf.getSelectedField();
                Ektron.SmartForm.selectElement(targetElement, selectedField);
                Ektron.SmartForm.raiseEventforIE9(event, "onIE9RadEventSelChanged");
            }
            else if (editField)
            {
                sf.setSelectedField(editField);
                editField = sf.getSelectedField();
                Ektron.SmartForm.selectElement(targetElement, editField);
                Ektron.ContentDesigner.raiseEvent(win, "onEditFieldButtonClick", [event, win, editField, sf.settings]);
            }

            if (win.ekSmartFormTargetElement && win.ekSmartFormTargetElement != targetElement) //#60211
            {
                win.ekSmartFormSelectedRange = null;
            }
        }
        else
        {
            var oElem = targetElement;
            if (Ektron.SmartForm.isFieldButton(oElem))
            {
                var doc = ("#document" == targetElement.nodeName ? targetElement : targetElement.ownerDocument);
                var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
                Ektron.ContentDesigner.raiseEvent(win, "onFieldButtonClick", [event, win, oElem, sf.settings]);
            }
            else if ("A" == oElem.tagName && (window.name.indexOf("RadEContentIframe") > -1 || true == Ektron.SmartForm.isFieldLink(oElem)))
            {
                //#48283: all hyperlinks in the editor and all hyperlinks in filelink field inside and outside the editor
                return false;
            }
            else if ("CAPTION" == oElem.tagName)
            {
                //#64874: enable ektdesignns_localize as custom tag selection in Data Entry content
                var eTableElem = $ektron(oElem).closest("table");
                if (eTableElem.hasClass("ektdesignns_localize"))
                {
                    $ektron(".design_selected_field").removeClass("design_selected_field");
                    selectedField = eTableElem.get(0);
                    sf.setSelectedField(selectedField);
                    selectedField = sf.getSelectedField();
                    Ektron.SmartForm.selectElement(targetElement, selectedField);
                    eTableElem.addClass("design_selected_field");
                    Ektron.SmartForm.raiseEventforIE9(event, "onIE9RadEventSelChanged");
                }
            }
        }
    }
    if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) >= 8) //#44519 & #46035
    {
        Ektron.SmartForm.docEvents.push(
		{ name: "mouseup", fnHandler: function(event)
		{
		    setSmartFormSelectedRange(event);
		}
		});
        Ektron.SmartForm.docEvents.push(
		{ name: "keyup", fnHandler: function(event)
		{
		    setSmartFormSelectedRange(event);
		}
		});

        function setSmartFormSelectedRange(event)
        {
            event = (event ? event : window.event);
            var targetElement = (event.target ? event.target : event.srcElement);
            if (targetElement && targetElement.nodeType != 1 /*Node.ELEMENT_NODE*/) targetElement = targetElement.parentNode;
            if (targetElement && targetElement.nodeType != 1 /*Node.ELEMENT_NODE*/) targetElement = null;
            if (!targetElement) return;
            if ("design_mode_design" == targetElement.className) return; //#59701
            var sf = Ektron.SmartForm.findByChildElement(targetElement);
            if (!sf) return;

            var selectedField = sf.getSelectedField();
            var doc = ("#document" == targetElement.nodeName ? targetElement : targetElement.ownerDocument);
            var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
            win.ekSmartFormTargetElement = null;
            win.ekSmartFormSelectedRange = null;
            if ("design_edit_fieldprop" == targetElement.className)
            {
                // #57693, #57698: reset the ekSmartFormSelectedRange when the edit icon is clicked.
                win.ekSmartFormSelectedRange = null;
                return;
            }
            var rng;
            if (selectedField)
            {
                if (!sf.designMode && $ektron(selectedField).hasClass("design_richarea"))
                {
                    // #61509: remember the text selection inside the rich area field
                    rng = ekCreateRange(doc.selection);
                }
                else if (sf.designMode)
                {
                    rng = doc.body.createControlRange();
                    rng.add(selectedField);
                }
            }
            else if ("TABLE" == targetElement.tagName || "TD" == targetElement.tagName || "TH" == targetElement.tagName)
            {
                rng = null; // #60836
            }
            else
            {
                rng = ekCreateRange(doc.selection);
            }
            win.ekSmartFormTargetElement = targetElement; //#53244
            win.ekSmartFormSelectedRange = rng;
        }
    }

    Ektron.SmartForm.fixMinHeight = function SmartForm_fixMinHeight(containingElement)
    {
        if ($ektron.browser.msie && "BackCompat" == document.compatMode)
        {
            var aryElems = containingElement.getElementsByTagName("div");
            for (var i = 0; i < aryElems.length; i++)
            {
                oElem = aryElems[i];
                if (oElem.currentStyle.minHeight != "auto")
                {
                    oElem.style.height = oElem.currentStyle.minHeight;
                }
            }
        }
    };

    Ektron.SmartForm.unfixMinHeight = function SmartForm_unfixMinHeight(containingElement)
    {
        if ($ektron.browser.msie && "BackCompat" == document.compatMode)
        {
            var aryElems = containingElement.getElementsByTagName("div");
            for (var i = 0; i < aryElems.length; i++)
            {
                oElem = aryElems[i];
                if (oElem.currentStyle.minHeight != "auto")
                {
                    oElem.style.height = "";
                }
            }
        }
    };

    function SmartForm_qualifySrcPath(containingElement)
    {
        try // just in case the reg exp of the src path is bad
		{
            this.replaceSrcPath(containingElement, /.*(\[|%5B)skinpath(\]|%5D)/, this.settings.skinPath, true);
            this.replaceSrcPath(containingElement, /.*(\[|%5B)srcpath(\]|%5D)btn/, this.settings.skinPath + "btn", true);
            this.replaceSrcPath(containingElement, /.*(\[|%5B)srcpath(\]|%5D)additem.gif/, this.settings.skinPath + "additem.gif", true);
            this.replaceSrcPath(containingElement, /.*(\[|%5B)srcpath(\]|%5D)designmenu.gif/, this.settings.skinPath + "designmenu.gif", true);
            this.replaceSrcPath(containingElement, /.*(\[|%5B)srcpath(\]|%5D)/, this.settings.srcPath, true);
        }
        catch (ex)
        {
            Ektron.OnException(this, Ektron.OnException.ignoreException, ex, arguments);
        }
    }

    function SmartForm_unqualifySrcPath(containingElement)
    {
        try // just in case the reg exp of the src path is bad
		{
            this.replaceSrcPath(containingElement, new RegExp(".*" + this.settings.srcPath, "i"), "[srcpath]", false);
            this.replaceSrcPath(containingElement, new RegExp(".*" + this.settings.skinPath, "i"), "[skinpath]", false);
        }
        catch (ex)
        {
            Ektron.OnException(this, Ektron.OnException.ignoreException, ex, arguments);
        }
    }

    function SmartForm_replaceSrcPath(containingElement, reSrcPath, strReplacement, bSetSrc)
    {
        var oElement = this.getContentElement(containingElement);
        if (!oElement) return;
        var imgTags = oElement.getElementsByTagName("img");
        var strClass;
        var strSrc;
        var imgTag;
        for (var i = 0; i < imgTags.length; i++)
        {
            imgTag = imgTags[i];
            strClass = imgTag.className + "";
            if (0 == strClass.indexOf("design_"))
            {
                strSrc = imgTag.src + "";
                if (strSrc.length > 0)
                {
                    if (bSetSrc)
                    {
                        imgTag.src = strSrc.replace(reSrcPath, strReplacement);
                        imgTag.removeAttribute("data-ektron-url");
                    }
                    else
                    {
                        imgTag.setAttribute("data-ektron-url", strSrc.replace(reSrcPath, strReplacement));
                    }
                }
            }
        }
    }

    function SmartForm_autoheight()
    {
        var me = this;
        var bindEvents = !this.settings.autoheightActive;
        this.settings.autoheightActive = true;
        if (bindEvents)
        {
            $ektron(document).bind($ektron.fn.autoheight.triggerName, function()
            {
                Ektron.ContentDesigner.raiseEvent(window, "onAutoheight", [me.settings]);
            });
        }
        $ektron("iframe.contenteditable").autoheight({ bindEvents: bindEvents });
        $ektron("iframe.contenteditable", this.getContentElement()).autoheight({ bindEvents: bindEvents });
    }

    if (typeof Ektron.ContentDesigner != "object") Ektron.ContentDesigner = {};
    Ektron.ContentDesigner.raiseEvent = function(win, eventName, args)
    {
        //raise an event to EkRadEditor.js
        if (win.parent && win.parent.Ektron && win.parent.Ektron.ContentDesigner)
        {
            var fnEventHandler = win.parent.Ektron.ContentDesigner[eventName];
            if ("function" == typeof fnEventHandler)
            {
                fnEventHandler.apply(win.parent.Ektron.ContentDesigner, args);
            }
        }
    };

    if (!Ektron.ContentDesigner.trace)
    {
        Ektron.ContentDesigner.trace = function()
        {
            if (typeof console != "undefined" && console && console.log && document.cookie && document.cookie.indexOf("ContentDesigner.console=") > -1)
            {
                console.log.apply(console, arguments);
            }
        };
    }
})();   // Ektron.SmartForm namespace
/// <reference path="ektron.js" />
/* Copyright 2003-2009, Ektron, Inc. */

if (!Ektron.SmartForm.fixContentEditable) (function() 
{

	Ektron.SmartForm.fixContentEditable = function SmartForm_fixContentEditable(containingElement)
	{
		if ($ektron.browser.mozilla)
		{
			// find outermost DIV(s) w/ contenteditable="true"
			var oElem = containingElement;
			if ("true" == oElem.getAttribute("contenteditable") && oElem.className != "design_iframe_membrane")
			{
				Ektron.SmartForm.editContentsWithIFrame(oElem); // don't worry about children
				Ektron.SmartForm.showParentIFrameScrollbars(oElem, false);
			}
			else
			{
				var aryElems = containingElement.getElementsByTagName("div");
                var bContainsRichAreaFld = false;
				for (var i = 0; i < aryElems.length; i++)
				{
					oElem = aryElems[i];
					if ("true" == oElem.getAttribute("contenteditable") && oElem.className != "design_iframe_membrane")
					{
						Ektron.SmartForm.editContentsWithIFrame(oElem); 
                        bContainsRichAreaFld = true;
					}
				} 
                // #61770: data entry content with no rich area field needs to fire off the onReady even in onLoadIFrame (EkRadEditor.js)
                if (false == bContainsRichAreaFld)
                {
				    var oDoc = containingElement.ownerDocument;
                    var sf = Ektron.SmartForm.findByChildElement(oElem);
                    Ektron.ContentDesigner.raiseEvent(window, "onLoadIFrame", [window, containingElement, oDoc, sf.settings]);
				}
			}
		}
	};

	Ektron.SmartForm.unfixContentEditable = function SmartForm_unfixContentEditable(containingElement)
	{
		if ($ektron.browser.mozilla)
		{
			// find outermost DIV(s) w/ contenteditable="true"
			var oElem = containingElement;
			if ("true" == oElem.getAttribute("contenteditable") && oElem.className != "design_iframe_membrane")
			{
				Ektron.SmartForm.replaceIFrameWithContents(oElem); // don't worry about children
				Ektron.SmartForm.showParentIFrameScrollbars(oElem, true);
			}
			else
			{
				var aryElems = containingElement.getElementsByTagName("div");
				for (var i = 0; i < aryElems.length; i++)
				{
					oElem = aryElems[i];
					if ("true" == oElem.getAttribute("contenteditable") && oElem.className != "design_iframe_membrane")
					{
						Ektron.SmartForm.replaceIFrameWithContents(oElem); 
					}
				}
			}
		}
	};

	Ektron.SmartForm.isEditWithIFrame = function SmartForm_isEditWithIFrame(oElem)
	{
		return (oElem && oElem.hasChildNodes() && 
			"IFRAME" == oElem.firstChild.tagName && 
			"contenteditable" == oElem.firstChild.className && 
			oElem.firstChild.contentWindow &&
			oElem.firstChild.contentWindow.document);
	};

	Ektron.SmartForm.isEditWithIFrameCache = function SmartForm_isEditWithIFrameCache(oElem)
	{
		return (oElem && oElem.hasChildNodes() && 
			"IFRAME" == oElem.firstChild.tagName && 
			"design_cache" == oElem.firstChild.className);
	};

	Ektron.SmartForm.removeCachedIFrames = function SmartForm_removeCachedIFrames(oElem)
	{
		$ektron("iframe.design_cache", oElem).remove();
		$ektron(oElem).find("div.design_iframe_membrane").each(function()
		{
			// Remove membrane, but retain its contents.
			$ektron(this).unwrapInner();
		});
	};
	
	function m_fixFocus()
	{
		if (Ektron.SmartForm.isEditWithIFrame(this)) 
		{
			var oWin = this.firstChild.contentWindow;
			setTimeout(function()
			{
				oWin.focus();
			}, 1);
		}
	}

	Ektron.SmartForm.editContentsWithIFrame = function SmartForm_editContentsWithIFrame(oElem)
	{
		if (Ektron.SmartForm.isEditWithIFrame(oElem)) 
		{
			return; // already done
		}
		else if (Ektron.SmartForm.isEditWithIFrameCache(oElem))
		{
			// Content is already cached in the IFRAME, just use it.
			// oElem.firstChild is the IFRAME
			// oElem.firstChild.nextSibling is the DIV membrane
			oElem.firstChild.className = "contenteditable";
			oElem.firstChild.nextSibling.innerHTML = "";
		}
		else
		{
			var sf = Ektron.SmartForm.findByChildElement(oElem);
			var strId = "";
			if (oElem.id)
			{
				strId = "&id=" + oElem.id;
			}
			var strCssFile = "";
			if (sf.settings.CssFilesArray)
			{
				for (var i = 0; i < sf.settings.CssFilesArray.length; i++)
				{
					strCssFile = "&css" + i + "=" + sf.settings.CssFilesArray[i];
				}
			}
			var strHeight = "";
			if (sf.designMode)
			{
				strHeight = "&height=99%";
			}
			
			var eElem = $ektron(oElem);
			eElem.unbind("focus", m_fixFocus);
			eElem.focus(m_fixFocus);

            var EditorContentAreaCss = "EditorContentArea.css";
            if ("rtl" == sf.settings.Direction)
            {
                EditorContentAreaCss = "EditorContentArea_RTL.css";
            }

			// place an IFRAME and set frame's document.designMode="on"
			var oIFrame = oElem.ownerDocument.createElement("iframe");
			oIFrame.src = sf.settings.srcPath + "ekformsiframe.aspx?js=no&eca=" + sf.settings.editorSkinPath + EditorContentAreaCss + strCssFile + strHeight + strId;
			oIFrame.className = "contenteditable";
			oIFrame.frameBorder = "0";
			oIFrame.marginHeight = 0;
			oIFrame.marginWidth = 0;
			// can't access iframe's document yet
			oElem.contentCache = eElem.html(); // content to copy into IFRAME when ready
			oIFrame.onload = function iframeOnLoad()
			{
				var oWin = this.contentWindow; // 'this' is oIFrame
				var oDoc = oWin.document;
				var eDoc = $ektron(oDoc);
				if (!sf.designMode)
				{
					$ektron(oDoc.body).addClass("design_richarea");
				}

				var objSelectionRange = null;
				var oElemWin = oElem.ownerDocument.defaultView;
				if (oElemWin.parent && oElemWin.parent.Ektron && oElemWin.parent.Ektron.SelectionRange)
				{
					objSelectionRange = new oElemWin.parent.Ektron.SelectionRange({ window: oWin });
				}
				
		//		var oHead = oDoc.getElementsByTagName("head")[0];
		//		
		//		var oLink = oDoc.createElement("link");
		//		oLink.setAttribute("rel", "stylesheet", 0);
		//		oLink.setAttribute("type", "text/css", 0);
		//		oLink.setAttribute("href", "ektron.smartForm.css", 0);
		//		oHead.appendChild(oLink);

				for (var i = 0; i < Ektron.SmartForm.docEvents.length; i++)
				{
					eDoc.bind(Ektron.SmartForm.docEvents[i].name, Ektron.SmartForm.docEvents[i].fnHandler);
				}
				
				if (oElem.onblur)
				{
					eDoc.blur(function(event){ oElem.onblur(); });
				}
				if (oElem.onfocus)
				{
					eDoc.focus(function(event){ oElem.onfocus(oElem); });
				}
				eDoc.focus(function(event)
				{
					event = (event ? event : oWin.event);
					var targetElement = (event.target ? event.target : event.srcElement);
					
					if (objSelectionRange)
					{
						var sel = objSelectionRange.getDomSelection();
						if (sel && sel.focusNode && "BODY" == sel.focusNode.nodeName)
						{
							objSelectionRange.moveToNode(sel.focusNode);
						}
					}
			
					// A workaround for SELECT to be selected in FF when users click on it.  Its onclick events is lost.
					var oFieldElem = event.originalTarget;
					if (oFieldElem && "OPTION" == oFieldElem.tagName)
					{
						oFieldElem = oFieldElem.parentNode;
					}
					if ("undefined" == typeof oFieldElem)
					{
						// #40604: To workaround a bug in FF3 (https://bugzilla.mozilla.org/show_bug.cgi?id=446670)
						var selection = oWin.getSelection();			
						var rng = null;
						if (selection.getRangeAt)
						{
							if (selection.rangeCount > 0)
							{
								rng = selection.getRangeAt(0);
								var selectedContent = rng.cloneContents();	
								var targetNodes = selectedContent.childNodes;
								if (/*DOCUMENT_FRAGMENT_NODE*/11 == selectedContent.nodeType && 1 == targetNodes.length && /*ELEMENT_NODE*/1 == targetNodes[0].nodeType && targetNodes[0].id.length > 0)
								{
									oFieldElem = oDoc.getElementById(targetNodes[0].id);
								} 
							}
						}
					}
					if (oFieldElem && "SELECT" == oFieldElem.tagName && Ektron.SmartForm.isDDFieldElement(oFieldElem))
					{
						sf.selectedField = oFieldElem;
						Ektron.SmartForm.selectElement(targetElement, sf.selectedField);
					}

					if (!sf.designMode)
					{
						Ektron.ContentDesigner.raiseEvent(oWin, "onContentWindowChange", [oWin, oElem, sf.settings]);
					}
				}); // "onfocus"
				eDoc.mouseup(function(event)
				{
					event = (event ? event : window.event);
					var targetElement = (event.target ? event.target : event.srcElement);
					var sel = oWin.getSelection();
					if (sel.rangeCount > 0)
					{
						var rng = sel.getRangeAt(0);
						var selectedContent = rng.cloneContents();	
						var targetNodes = selectedContent.childNodes;		
						// #40604: To workaround a bug in FF3 (https://bugzilla.mozilla.org/show_bug.cgi?id=446670  )
						if (/*DOCUMENT_FRAGMENT_NODE*/11 == selectedContent.nodeType)
						{
							for (var i = 0; i < targetNodes.length; i++)
							{
								var thisNode = targetNodes[i];			                
								if (/*ELEMENT_NODE*/1 == thisNode.nodeType && "SELECT" == thisNode.tagName && Ektron.SmartForm.isDDFieldElement(thisNode))
								{
				                    oFieldElem = oDoc.getElementById(thisNode.id);
				                    sf.selectedField = oFieldElem;
									Ektron.SmartForm.selectElement(targetElement, sf.selectedField);
									break;
								}
							 }
						 }
					}
				}); // "onmouseup"
				
				eDoc.bind("contextmenu", function(event)
				{
					event = (event ? event : window.event);
					var targetElement = (event.target ? event.target : event.srcElement);
					var doc = ("#document" == targetElement.nodeName ? targetElement : targetElement.ownerDocument);
					var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
					Ektron.ContentDesigner.raiseEvent(win, "onContextMenu", [event, win, oElem, sf.settings]);
				}); // "oncontextmenu"
				
				// For #34860 and #36685
				var mc_PreserveUrlAttr = "data-ektron-url";
				var mc_UrlAttrs = [ "href", "src", "background", "dynsrc" ]; // optimized order

				eDoc.keypress(function(event)
				{
					event = (event ? event : window.event); 
					var bContentEditable = true;
					var oNode = getSelectionElement(this); // 'this' is oDoc (Mozilla)
					while (oNode != null)
					{
						if ("function" == typeof oNode.getAttribute)
						{
							var sContEdit = oNode.getAttribute("contenteditable");
							if ("false" == sContEdit)
							{
								bContentEditable = false;
								break;
							}
							else if ("true" == sContEdit)
							{
								bContentEditable = true;
								break;
							}
						}
						oNode = oNode.parentNode;
					}
					
					if (!bContentEditable)
					{
						if ("function" == typeof event.preventDefault) event.preventDefault();
					}
					return bContentEditable;
				}); // "onkeypress"
				
				eDoc.keyup(function(event)
				{
					event = (event ? event : window.event); 
					// 'this' is oDoc (Mozilla)
					// In FF 2, event.altKey is undefined
					// ctrl+v for paste
					if (event.ctrlKey && "V".charCodeAt(0) == event.keyCode && !event.shiftKey && !event.altKey)
					{
						// #36685 - using firefox,image and quicklink path breaks if it is pasted again in the same content
						// Fix corrupted URLs using saved URL
						$ektron("[" + mc_PreserveUrlAttr + "]", this).each(function()
						{
							for (var i = 0; i < mc_UrlAttrs.length; i++)
							{
								var attr = mc_UrlAttrs[i];
								if (this.hasAttribute(attr))
								{
									var ektValue = this.getAttribute(mc_PreserveUrlAttr);
									var value = this.getAttribute(attr);
									if (ektValue != value)
									{
										// Only set attribute if it is different so as to not perturb the element.
										this.setAttribute(attr, ektValue);
									}
									break; // exit for
								}
							}
						});
					}
				}); // "onkeyup"
				
				// #34860 - Dragging Image and dropping after the text is not placing the Image when adding Html content.Instead it is placing the Image Text.(FF)
				// Detect mouse action on selection and preserve URL in custom attribute to be restored when dropped.
				var m_preserveList = [];
				function m_preserveUrl()
				{
					// 'this' is an HTML element
					var bPreserved = false;
					for (var i = 0; i < mc_UrlAttrs.length; i++)
					{
						var attr = mc_UrlAttrs[i];
						if (this.hasAttribute(attr))
						{
							var value = this.getAttribute(attr); // Don't use property b/c it will be fully qualified
							this.setAttribute(mc_PreserveUrlAttr, value);
							bPreserved = true;
						}
					}
					if (bPreserved)
					{
						var id = this.id;
						if (!id)
						{
							id = mc_PreserveUrlAttr + Math.floor(Math.random() * 1679616).toString(36); // 4 digit alphanum
							this.id = id;
						}
						m_preserveList.push(id);
					}
				}
				function m_restoreUrl(event)
				{
					if (m_preserveList.length > 0)
					{
						for (var i = 0; i < m_preserveList.length; i++)
						{
							var id = m_preserveList[i];
							var oElem = $ektron("#" + id, oDoc).get(0);
							// There may have been duplicates and so the id may have already been removed.
							if (oElem)
							{
								for (var j = 0; j < mc_UrlAttrs.length; j++)
								{
									var attr = mc_UrlAttrs[j];
									if (oElem.hasAttribute(attr) && oElem.hasAttribute(mc_PreserveUrlAttr))
									{
										var ektValue = oElem.getAttribute(mc_PreserveUrlAttr);
										var value = oElem.getAttribute(attr);
										if (ektValue != value)
										{
											// Only set attribute if it is different so as to not perturb the element.
											oElem.setAttribute(attr, ektValue);
										}
										//oElem.removeAttribute(mc_PreserveUrlAttr); will be removed in ContentOutgoing.xslt
										break; // exit for
									}
								}
								if (0 == oElem.id.indexOf(mc_PreserveUrlAttr))
								{
									oElem.id = "";
									oElem.removeAttribute("id");
								}
							}
						}
						m_preserveList = [];
					}
				}
				
				eDoc.mousedown(function(event)
				{
					event = (event ? event : window.event);
					var targetElement = (event.target ? event.target : event.srcElement);
					var sel = oWin.getSelection();
					try 
					{
						// May throw "Permission denied to access property 'nodeType' from a non-chrome context
						// Seen in FF 3.5 when cursor is moved INTO input text field using arrow key.
						if (sel.rangeCount > 0)
						{
							var rng = sel.getRangeAt(0);
							var node = rng.commonAncestorContainer;
							if (1 /*Node.ELEMENT_NODE*/ == node.nodeType)
							{
								$ektron(node).children().each(m_preserveUrl);
							}
						}
						if (targetElement && 1 /*Node.ELEMENT_NODE*/ == targetElement.nodeType)
						{
							$ektron(targetElement).children().andSelf().each(m_preserveUrl);
						}
					}
					catch (ex)
					{
						return null;
					}
				}); // "onmousedown"
				eDoc.mouseup(m_restoreUrl);
				eDoc.mouseout(m_restoreUrl);
				
				if ("" == oElem.contentCache)
				{
					oElem.contentCache = "<p>&#160;</p>";
				}
				try
				{
					// set designMode prior to loading content so 
					// SCRIPT tags are not evaluated in $ektron(selector).domManip (defect #35400)
					oDoc.designMode = "on";
				}
				catch (ex)
				{
					// ignore
					Ektron.ContentDesigner.raiseEvent(oWin, "onexception", [new Error("Failed to set designMode. Error: " + ex.message), arguments]);
				}
				$ektron(oDoc.body).html(oElem.contentCache); // copy content into IFRAME
				oElem.contentCache = null;
				
				// #34901 - using firefox,if there are multiple richarea fields, the cursor keeps going back to second field while I am typing in the first field
				if (sf.designMode)
				{
					Ektron.ContentDesigner.raiseEvent(oWin, "onContentWindowChange", [oWin, oElem, sf.settings]);
				}
				else
				{
					var oFormElem = sf.getContentElement();
					if (oFormElem)
					{
						Ektron.SmartForm.precalcForm(oFormElem);
						if (sf.settings.prevalidate) Ektron.SmartForm.prevalidateForm(oFormElem);
					}
					
					try
					{
					    // remove 'loading' indicator and temporary markers, also trigger removal of loading to edit.aspx
						while (oElem.firstChild.tagName != "IFRAME")
						{
							oElem.removeChild(oElem.firstChild);
                            var cw = window.parent;
                            cw.$ektron(cw).trigger('ektron_editor_ff_iframe_remove');
						}
					}
					catch (ex)
					{
						// ignore
						Ektron.ContentDesigner.raiseEvent(oWin, "onexception", [new Error("Failed to set designMode. Error: " + ex.message), arguments]);
					}

		            var oLastChild = oElem.lastChild;
		            if ("P" == oLastChild.tagName && $ektron(oLastChild).hasClass("contentUsability")) {
		                oElem.removeChild(oElem.lastChild);
		            }
				}
				Ektron.ContentDesigner.raiseEvent(oWin, "onLoadIFrame", [oWin, oElem, oDoc, sf.settings]);
			}; // oIFrame.onload
			
			if (true == sf.settings.autoheightActive)
			{
				$ektron(oIFrame.ownerDocument).bind($ektron.fn.autoheight.triggerName, function()
				{
					Ektron.ContentDesigner.raiseEvent(window, "onAutoheight", [sf.settings]);
				});
				$ektron(oIFrame).autoheight();
			}
			else if (!sf.designMode)
			{
				$ektron(oIFrame).autoheight();
			}
			
			// remove content
			while (oElem.hasChildNodes())
			{
				oElem.removeChild(oElem.firstChild);
			}
			
			if (!sf.designMode)
			{
				// insert 'loading' indicator
                var cw = window.parent;
                cw.$ektron(cw).trigger('ektron_editor_ff_iframe_add');
				$ektron(oElem).append("<img class=\"design_loading_indicator\" src=\"" + sf.settings.skinPath + "iframeloading.png\" />");
			}
			
			// insert iframe
			oElem.appendChild(oIFrame);
			
			var oMembrane = oElem.ownerDocument.createElement("div");
			oMembrane.className = "design_iframe_membrane";
			oMembrane.setAttribute("contenteditable", "true"); // #40836, see ektron.js
			oElem.appendChild(oMembrane);
			// Clean up because this function uses closure and will stay in memory
			oMembrane = null;
			oIFrame = null;
		}
	};

	Ektron.SmartForm.replaceIFrameWithContents = function SmartForm_replaceIFrameWithContents(oElem)
	{
		if (Ektron.SmartForm.isEditWithIFrame(oElem))
		{
			var eElem = $ektron(oElem);
			eElem.unbind("focus", m_fixFocus);
			// #48381: in v1.3.2 jquery.html(content), it cleans the content and calls merge(). When the global variable 
			// $ektron.support.getAll is not true, the comment node (8 == nodeType) in the content will be removed. 
			// The following code make sure the comment in the content is retained. Then, reset the $ektron.support.getAll.
			var jqueryGetAll = $ektron.support.getAll;
			$ektron.support.getAll = true;
			try
			{
			    if ("string" == typeof oElem.contentCache) 
			    {
				    eElem.html(oElem.contentCache);
			    }
			    else
			    {
				    // Doug Domeny 2008-03-06 
				    // Tested with Firefox 2.0.0.12.
				    // Need to keep IFRAME in DOM otherwise it resets.
				    // It reloads and requires time (asynchronously) to be ready.
				    // Errors are thrown when execcommand is called too soon.
				    // Can't even move it without it reloading.
				    // oElem.firstChild is the IFRAME
				    // oElem.firstChild.nextSibling is the DIV membrane
				    // Copy content from IFRAME into DIV membrane
				    $ektron(oElem.firstChild.nextSibling).html($ektron(oElem.firstChild.contentWindow.document.body).html());
				    oElem.firstChild.className = "design_cache";
			    }
			}
			catch (ex) 
			{
			    Ektron.OnException(Ektron.SmartForm, Ektron.OnException.ignoreException, ex, arguments);
			}
            finally
            {
			    $ektron.support.getAll = jqueryGetAll;
			}
		}
	};

	Ektron.SmartForm.showParentIFrameScrollbars = function SmartForm_showParentIFrameScrollbars(oElem, bShow)
	{
		try
		{
			// override CSS padding
			document.body.style.padding = (bShow ? "" : "0px;");
			oElem.style.padding = (bShow ? "" : "0px;");
			oElem.ownerDocument.defaultView.frameElement.style.overflow = (bShow==false ? "hidden" : "auto");
		}
		catch (ex)
		{
			Ektron.OnException(Ektron.SmartForm, Ektron.OnException.ignoreException, ex, arguments);
		}
	};

})(); // Ektron.SmartForm namespace extensions

if (typeof Ektron.ContentDesigner != "object") Ektron.ContentDesigner = {};
if ("undefined" == typeof Ektron.ContentDesigner.bubbleUpEvent)
{
	Ektron.ContentDesigner.bubbleUpEvent = function(eventName)
	{
		if (typeof this[eventName] != "function")
		{
			this[eventName] = function()
			{
				if (window != window.parent)
				{
					this.raiseEvent(window, eventName, arguments);
				}
			};
		}
	};
	Ektron.ContentDesigner.bubbleUpEvent("onexception");
	Ektron.ContentDesigner.bubbleUpEvent("onContentWindowChange");
	Ektron.ContentDesigner.bubbleUpEvent("onLoadIFrame");
	Ektron.ContentDesigner.bubbleUpEvent("onFieldButtonClick");
	Ektron.ContentDesigner.bubbleUpEvent("onEditFieldButtonClick");
	Ektron.ContentDesigner.bubbleUpEvent("onContextMenu");
	Ektron.ContentDesigner.bubbleUpEvent("onAutoheight");
};
/// <reference path="ektron.js" />
/* Copyright 2003-2010, Ektron, Inc. */

if (!Ektron.SmartForm.Repeater) (function()
{
	Ektron.SmartForm.Repeater = function SmartForm_Repeater(settings)
	{
		// Note: Simulate Web Forms 2.0 (www.whatwg.org) attributes: repeat, repeat-max, repeat-min, repeat-start, repeat-template
		this.settings = settings;
		this.onexception = settings.onexception;

		this.current = null;
		this.prototype = null;
		this.setCurrent_event_srcElement = null;
		
		this.localizeString = function(id, defaultString) { return this.settings.localizedStrings[id] || defaultString; };

		this.setFocus = SmartForm_Repeater_setFocus;
		this.setCurrent = SmartForm_Repeater_setCurrent;
		this.setPrototype = SmartForm_Repeater_setPrototype;
		this.getRowSection = SmartForm_Repeater_getRowSection;
		this.insert = SmartForm_Repeater_insert;
		this.getTemplateElement = SmartForm_Repeater_getTemplateElement;
		this.isRow = SmartForm_Repeater_isRow;
		this.addShortCuts = SmartForm_Repeater_addShortCuts;
		this.addMinElements = SmartForm_Repeater_addMinElements;
		this.numElements = SmartForm_Repeater_numElements;
		this.getMax = SmartForm_Repeater_getMax;
		this.getMin = SmartForm_Repeater_getMin;
		this.atMax = SmartForm_Repeater_atMax;
		this.atMin = SmartForm_Repeater_atMin;
		this.insertAbove_disabled = SmartForm_Repeater_insertAbove_disabled;
		this.insertAbove = SmartForm_Repeater_insertAbove;
		this.insertBelow_disabled = SmartForm_Repeater_insertBelow_disabled;
		this.insertBelow = SmartForm_Repeater_insertBelow;
		this.duplicate_disabled = SmartForm_Repeater_duplicate_disabled;
		this.duplicate = SmartForm_Repeater_duplicate;
		this.remove_disabled = SmartForm_Repeater_remove_disabled;
		this.remove = SmartForm_Repeater_remove;
		this.moveUp_disabled = SmartForm_Repeater_moveUp_disabled;
		this.moveUp = SmartForm_Repeater_moveUp;
		this.moveDown_disabled = SmartForm_Repeater_moveDown_disabled;
		this.moveDown = SmartForm_Repeater_moveDown;
		this.replace = SmartForm_Repeater_replace;
		this.insertRows = SmartForm_Repeater_insertRows;
		this.showContextMenu = SmartForm_Repeater_showContextMenu;
		this.hideContextMenu = SmartForm_Repeater_hideContextMenu;
	}; // constructor
	Ektron.SmartForm.Repeater.onexception = Ektron.SmartForm.onexception;

	function SmartForm_Repeater_setFocus(oRow)
	{
		this.setCurrent(null, oRow);
		var oElem = $ektron(":input:first", oRow).get(0);
		if (oElem) 
		{
			setTimeout(function() 
			{ 
				try 
				{ 
					oElem.focus();	
				} catch (ex) {} 
			}, 1);
		}
	}
	function SmartForm_Repeater_setCurrent(event, oRow)
	{
		if (event)
		{
			if (this.setCurrent_event_srcElement == event.srcElement) 
			{
                this.current = oRow; // #61730
				return;
			}
			else
			{
				// The event object does not support custom properties.
				// Could use event.cancelBubble=true, but this is dangerous
				// because the event handlers may need to know for actions
				// other than setting the current row.
				this.setCurrent_event_srcElement = event.srcElement;
			}
		}
		if (this.current) Ektron.SmartForm.Repeater.highlight(this.current, false);	
		this.current = oRow;
		// tfoot = prototypes; tr[1] = 0; tr[2] = 1+; (1-based indexes)
		// source: table/tbody[n]/tr[n] target: table/tfoot/tr[2] 
		var oPrototype = $ektron(oRow).parent().parent().get(0).tFoot.rows[1];
		this.setPrototype(oPrototype);
	}
	function SmartForm_Repeater_setPrototype(oRow)
	{
		this.prototype = oRow;
	}
	function SmartForm_Repeater_getRowSection(oRow)
	{
		return $ektron(oRow).parent().get(0);
	}

	function SmartForm_Repeater_insert(oPrototype, index)
	{
		var oSection = this.getRowSection(this.current);
		var oRow = oSection.insertRow(index);
		
		Ektron.SmartForm.unfixContentEditable(oPrototype); 
		
		// cloneNode will add "value=on" to input tags that are checked and don't have a value attribute.
		var newRow = $ektron(oPrototype).clone(true).removeAttr("id").makeIdentifiersUnique();
		$ektron(oRow).replaceWith(newRow); // returns the old row
		oRow = oSection.rows[index];
        Ektron.SmartForm.precalcForm(oRow);
		Ektron.SmartForm.prevalidateElement(oRow, null);
		//this.addShortCuts(oRow); // clone seems to preserve the short cut events
		this.setFocus(oRow);

		// Any cloned 'unfixed' IFRAMEs are dead, remove them.
		Ektron.SmartForm.removeCachedIFrames(oRow);
		
		Ektron.SmartForm.fixContentEditable(oPrototype); // async
		Ektron.SmartForm.fixContentEditable(oRow); // async
	}

	function SmartForm_Repeater_getTemplateElement()
	{
		var oElem = null;
		var oSection = this.getRowSection(this.current);
		var listitemTemplate = oSection.getAttribute("ektdesignns_listitem_template"); 
		if ("string" == typeof listitemTemplate)
		{
			oElem = $ektron(oSection).parent().get(0).getElementById(listitemTemplate);
		}
		else
		{
			if (Ektron.SmartForm.isDDFieldElement(this.prototype))
			{
				oElem = this.prototype;
			}
			else
			{
				oElem = $ektron(this.prototype.cells[1]).children().get(0);
			}
		}
		return oElem;
	}

	function SmartForm_Repeater_isRow(oElem)
	{	   
		if (!oElem) return false;
		if (!oElem.onclick) return false;	
		if ($ektron(oElem).children().get(0).colSpan > 1) return false;
		return true;
	}
	
	function SmartForm_Repeater_addShortCuts(containingElement)
	{
		var oFormElem = Ektron.SmartForm.getContentElement(containingElement);
		if (!oFormElem) return;
		try
		{
			var me = this;
			$ektron("input:text[ektdesignns_maxoccurs]", oFormElem).keydown(function(e)
			{
				switch (e.which)
				{
					case 8: // Backspace
						if ("" == this.value)
						{
							setTimeout(function()
							{
								me.remove();
							}, 1);
						}
						break;
					case 13: // Enter
						setTimeout(function()
						{
							me.insertBelow();
						}, 1);
						break;
					case 38: // Up arrow
						var oSibling = $ektron(me.current).prev().get(0);
						if (me.isRow(oSibling))
						{
							setTimeout(function()
							{
								me.setFocus(oSibling);
							}, 1);
						}
						break;
					case 40: // Down arrow
						var oSibling = $ektron(me.current).next().get(0);
						if (me.isRow(oSibling))
						{
							setTimeout(function()
							{
								me.setFocus(oSibling);
							}, 1);
						}
						break;
					default:
						//alert(e.which);
				}
			});
		}
		catch (ex)
		{
			Ektron.OnException(this, Ektron.OnException.ignoreException, ex, arguments);
		}
	}

	function SmartForm_Repeater_addMinElements(containingElement)
	{
		var oFormElem = Ektron.SmartForm.getContentElement(containingElement);
		if (!oFormElem) return;
		try
		{
			var oElem;
			var oParentElem;
			var strAddMin;
			var nMinToAdd;
			var strID;
			var aryElems = oFormElem.getElementsByTagName("td");
			if (!aryElems) return;
			for (var i = 0; i < aryElems.length; i++)
			{
				oElem = aryElems[i];
				strAddMin = oElem.getAttribute("ektdesignns_addmin");
				if (strAddMin != null && "string" == typeof strAddMin && strAddMin.length > 0)
				{
					// Get number of elements to add to make the minimum.
					nMinToAdd = parseInt(strAddMin, 10);
					// Get TR that is parent of this TD.
					oParentElem = $ektron(oElem).parent().get(0);
					strID = oParentElem.id + "";
					if (strID.length > 0 && nMinToAdd > 0)
					{
						this.insertRows(strID, nMinToAdd);
					}
				}
			} // for
		}
		catch (ex)
		{
			Ektron.OnException(this, Ektron.OnException.ignoreException, ex, arguments);
		}
	}

	function SmartForm_Repeater_numElements(oSection) 
	{
		var joSectionChildren = $ektron(oSection).children();	
		var nChildren = joSectionChildren.length;
		var nElements = nChildren;
		for (var i = 0; i < nChildren; i++)
		{
			if (!this.isRow(joSectionChildren.get(i)))
			{
				nElements--;
			}
		}
		return nElements;
	}

	function SmartForm_Repeater_getMax()
	{
		var oElem = this.getTemplateElement();
		var vLimit = oElem.getAttribute("ektdesignns_maxoccurs");
		var nLimit = 1;
		if ("string" == typeof vLimit)
		{
			if ("unbounded" == vLimit)
			{
				return "unbounded";
			}
			else
			{
				nLimit = parseInt(vLimit, 10);
			}
		}
		else if ("number" == typeof vLimit)
		{
			nLimit = vLimit;
		}
		return nLimit; // or "unbounded"
	}
	function SmartForm_Repeater_getMin()
	{
		var oElem = this.getTemplateElement();
		var vLimit = oElem.getAttribute("ektdesignns_minoccurs");
		var strUse = oElem.getAttribute("ektdesignns_use");
		var nLimit = 1;
		if ("optional" == strUse)
		{
			nLimit = 0;
		}
		
		if ("string" == typeof vLimit)
		{
			nLimit = parseInt(vLimit, 10);
		}
		else if ("number" == typeof vLimit)
		{
			nLimit = vLimit;
		}
		return nLimit;
	}
	function SmartForm_Repeater_atMax()
	{
		var max = this.getMax();
		if ("unbounded" == max) return false;
		var oSection = this.getRowSection(this.current);
		var numElements = this.numElements(oSection);
		return (numElements >= max);
	}
	function SmartForm_Repeater_atMin()
	{
		var min = this.getMin();
		var oSection = this.getRowSection(this.current);
		var numElements = this.numElements(oSection); 
		return (numElements <= min);
	}

	// actions
	function SmartForm_Repeater_insertAbove_disabled()
	{
		return this.atMax();
	}
	function SmartForm_Repeater_insertAbove()
	{
		if (!this.insertAbove_disabled())
		{
			this.insert(this.prototype, this.current.sectionRowIndex);
		}
		this.hideContextMenu();
	}
	function SmartForm_Repeater_insertBelow_disabled()
	{
		return this.atMax();
	}
	function SmartForm_Repeater_insertBelow()
	{
		if (!this.insertBelow_disabled())
		{
			this.insert(this.prototype, this.current.sectionRowIndex + 1);
		}
		this.hideContextMenu();
	}
	function SmartForm_Repeater_duplicate_disabled()
	{
		return this.atMax();
	}
	function SmartForm_Repeater_duplicate()
	{
		if (!this.duplicate_disabled())
		{
			Ektron.SmartForm.Repeater.highlight(this.current,false);
			this.insert(this.current, this.current.sectionRowIndex);
		}
		this.hideContextMenu();
	}
	function SmartForm_Repeater_remove_disabled()
	{
		return this.atMin();
	}
	function SmartForm_Repeater_remove()
	{
		if (!this.remove_disabled())
		{
			var oSection = this.getRowSection(this.current);
			if (this.isRow(this.current))
			{
				var max = this.getMax();
				if (max !== "unbounded" && 1 === this.numElements(oSection))
				{
					var cur = this.current;
					this.insert($ektron(this.prototype).prev().get(0), this.current.sectionRowIndex + 1);
					this.current = cur;
				}
			}
			var eCur = $ektron(this.current);
			var oSibling = eCur.prev().get(0);
			if (!this.isRow(oSibling))
			{
				oSibling = eCur.next().get(0);
			}
			eCur.remove();
			this.current = null;
			if (this.isRow(oSibling))
			{
				this.setFocus(oSibling);
			}
		}
		this.hideContextMenu();
	}
	function SmartForm_Repeater_moveUp_disabled()
	{
		var oSibling = $ektron(this.current).prev().get(0);
		return (!this.isRow(oSibling));
	}
	function SmartForm_Repeater_moveUp()
	{
		if (!this.moveUp_disabled())
		{
			var oRow = $ektron(this.current).prev().get(0);
			Ektron.SmartForm.unfixContentEditable(oRow); 
			
			$ektron(this.current).prev().insertAfter(this.current);
			
			Ektron.SmartForm.removeCachedIFrames(oRow);
			Ektron.SmartForm.fixContentEditable(oRow); // async
		}
		this.hideContextMenu();
	}
	function SmartForm_Repeater_moveDown_disabled()
	{
		var oSibling = $ektron(this.current).next().get(0);
		return (!this.isRow(oSibling));
	}
	function SmartForm_Repeater_moveDown()
	{
		if (!this.moveDown_disabled())
		{
			var oRow = $ektron(this.current).next().get(0);
			Ektron.SmartForm.unfixContentEditable(oRow); 
			
			$ektron(this.current).next().insertBefore(this.current);
			
			Ektron.SmartForm.removeCachedIFrames(oRow);
			Ektron.SmartForm.fixContentEditable(oRow); // async
		}
		this.hideContextMenu();
	}

	function SmartForm_Repeater_replace()
	{
		// Can't call this.insertBelow because we need to force the insertion prior to removing.
		var cur = this.current; // 'insert()' changes 'current'
		this.insert(this.prototype, this.current.sectionRowIndex + 1); 
		this.current = cur;
		this.remove(); // remove only after inserting the new one
	}
	
	function SmartForm_Repeater_insertRows(id, nCount)
	{
		// Replace row specified by 'id' with the specified number of prototype rows.
		var oRow = document.getElementById(id);
		if (oRow)
		{
			this.setCurrent(null, oRow);
			for (var i = 0; i < nCount; i++)
			{
				this.insertAbove();
			}
			// Can't call this.replace or _remove because we need to force removal.
			$ektron(oRow).remove();
		}
	}

	Ektron.SmartForm.Repeater.highlight = function SmartForm_Repeater_highlight(oRow, bOn)
	{
		var re = new RegExp((bOn ? "_normal" : "_hover"));
		var strReplace = (bOn ? "_hover" : "_normal");
		var oCell = null;
		for (var i = 0; i < oRow.cells.length; i++)
		{
			oCell = oRow.cells[i];
			oCell.className = oCell.className.replace(re,strReplace);
		}
	};
	
	function SmartForm_Repeater_showContextMenu(event, oElem)
	{
		this.setCurrent(event, $ektron(oElem).parent().parent().get(0));
		Ektron.SmartForm.Repeater.highlight(this.current, true);
		
		if (!this.contextMenu)
		{
			this.contextMenu = new Ektron.SmartForm.Repeater.ContextMenu(this.settings);
			var me = this;
			$ektron(document.body).click(function SmartForm_Repeater_body_click(event)
			{
				event = (event ? event : window.event);
				var oElem = event.srcElement;
				// setCapture and releaseCapture disable the a:hover effect
				// ref: How To Create a Mouse Capture Drop-down Menu 
				// href: http://msdn.microsoft.com/workshop/author/dhtml/howto/mousecaptureddm.asp
				if (!oElem.menutype)
				{
					me.hideContextMenu();
				}
			});
		}
		this.contextMenu.show(event, oElem);
	}
	function SmartForm_Repeater_hideContextMenu()
	{
		if (this.contextMenu) this.contextMenu.hide();
		if (this.current) Ektron.SmartForm.Repeater.highlight(this.current, false);
	}
	
	// ContextMenu Class
	
	Ektron.SmartForm.Repeater.ContextMenu = function SmartForm_Repeater_ContextMenu(settings)
	{
		this.settings = settings;
		
		this.localizeString = function(id, defaultString) { return this.settings.localizedStrings[id] || defaultString; };

		this.isVisible = SmartForm_Repeater_ContextMenu_isVisible;
		this.update = SmartForm_Repeater_ContextMenu_update;
		this.show = SmartForm_Repeater_ContextMenu_show;
		this.hide = SmartForm_Repeater_ContextMenu_hide;
		this.createMenuElement = SmartForm_Repeater_ContextMenu_createMenuElement;
		
		this.contextMenuElement = this.createMenuElement();
		document.body.appendChild(this.contextMenuElement.parentNode);
	}; // constructor
	
	function SmartForm_Repeater_ContextMenu_createMenuElement()
	{
		var $contextMenu = $ektron("div.design_container > div.design_contextmenu");
		if ($contextMenu.length > 0)
		{
			return $contextMenu.get(0);
		}
		
		var oMenuItems = [ 
			{ action: "design_row_insertAbove", caption: this.localizeString("mnuInsAbv", "Insert Above"), icon: "insabove" }
		,	{ action: "design_row_insertBelow", caption: this.localizeString("mnuInsBel", "Insert Below"), icon: "insbelow" }
		,	{ action: "design_row_duplicate", caption: this.localizeString("mnuDupl", "Duplicate"), icon: "duplicate" }
		,	{ action: "design_row_moveUp", caption: this.localizeString("mnuMvUp", "Move Up"), icon: "moveup" }
		,	{ action: "design_row_moveDown", caption: this.localizeString("mnuMvDn", "Move Down"), icon: "movedown" }
		,	{ action: "design_row_remove", caption: this.localizeString("mnuRem", "Remove"), icon: "remove" }
		];

		// structure: div/div/table/tbody/tr[]/td[0]/a/img, td[1]/a
		var oContainer = document.createElement("div");
		oContainer.contentEditable = false;
		oContainer.unselectable = "on";
		oContainer.className = "design_container";

		var oElem = document.createElement("div");
		oContainer.appendChild(oElem);
		oElem.className = "design_contextmenu";
		oElem.style.display = "none";
		oElem.style.visibility = "hidden";
		
		var oTable = document.createElement("table");
		oElem.appendChild(oTable);
		oTable.cellSpacing = 0;
		oTable.cellPadding = 0;
		oTable.border = 0;
		
		var oTBody = document.createElement("tbody");
		oTable.appendChild(oTBody);	
		var oRow = null;
		var oCell = null;
		var oItem = null;
		var strBeginHTML = '';
		var strEndHTML = '</a>';
		for (var i = 0; i < oMenuItems.length; i++)
		{
			oItem = oMenuItems[i];
			oRow = oTBody.insertRow(oTBody.rows.length);
			oRow.setAttribute("ektdesignns_action", oItem.action); // used to update status
			oCell = oRow.insertCell(oRow.cells.length);
			strBeginHTML = '<a class="design_menuitem" href="#" onclick="if(!this.isDisabled){' + oItem.action + '()};return false;" menutype="menuitem"><img src="' + this.settings.skinPath + oItem.icon + '.gif" width="16" height="16" border="0"/>'; 
			oCell = oRow.insertCell(oRow.cells.length);
			oCell.innerHTML = strBeginHTML + oItem.caption + strEndHTML;
		}
		
		return oContainer.firstChild;
	}
	
	function SmartForm_Repeater_ContextMenu_isVisible() 
	{
		return (this.contextMenuElement.style.visibility != "hidden"); 
	}
	
	function SmartForm_Repeater_ContextMenu_update()
	{
		// structure: .design_contextmenu/table/tbody/tr[]
		var strAction = "";
		var bDisabled = false;
		var oRow = null;
		var oElem = null;
		var strSrc;
		var imgTags;
		var oTable = this.contextMenuElement.firstChild; 
		var oTBody = oTable.tBodies[0]; 
		for (iRow = 0; iRow < oTBody.rows.length; iRow++)
		{
			oRow = oTBody.rows[iRow];
			strAction = oRow.getAttribute("ektdesignns_action");
			bDisabled = (window[strAction + '_disabled'])();
			oRow.disabled = bDisabled;
			
			if (bDisabled)
			{
				$ektron(".design_menuitem", oRow).each(function()
				{
					this.className = "design_menuitem_disabled";
					$ektron("img", this).each(function()
					{
						strSrc = this.src + "";
						this.src = strSrc.replace(/\.gif/, "_disabled.gif");
					});
				});
			}
			else
			{
				$ektron(".design_menuitem_disabled", oRow).each(function()
				{
					this.className = "design_menuitem";
					$ektron("img", this).each(function()
					{
						strSrc = this.src + "";
						this.src = strSrc.replace(/_disabled\.gif/, ".gif");
					});
				});
			}
		}
	}

	function SmartForm_Repeater_ContextMenu_show(event, oElem)
	{
		this.update();

		var offset = $ektron(oElem).offset();
		var x = offset.left + oElem.offsetWidth;
		var y = offset.top;
		
		Ektron.SmartForm.showWindowedControls(false);
		var oStyle = this.contextMenuElement.style;  
		oStyle.left = x + "px";
		oStyle.top = y + "px"; 
		oStyle.display = "block";
		oStyle.visibility = "visible";
		
		$ektron("td:last", this.contextMenuElement).get(0).scrollIntoView(false);// #57069	
		event.cancelBubble = true;
		//this.contextMenuElement.focus(); Note: caused scroll problem IE 7/Vista
	}
	
	function SmartForm_Repeater_ContextMenu_hide()
	{
		if (typeof this.contextMenuElement != "undefined" && this.contextMenuElement != null) 
		{
			oStyle = this.contextMenuElement.style;
			oStyle.display = "none";
			oStyle.visibility = "hidden";
		}
		Ektron.SmartForm.showWindowedControls(true);
	}
	
})(); // Ektron.SmartForm.Repeater namespace

if (!Ektron.SmartForm.showWindowedControls)
{
	Ektron.SmartForm.showWindowedControls = function SmartForm_showWindowedControls(bShow)
	{
		if (bShow)
		{
			$ektron(".design_selectwrapper").each(function()
			{
				var eThis = $ektron(this);
				eThis.after(eThis.contents()).remove();
			});
		}
		else
		{
			var oNewElem = document.createElement("span");
			oNewElem.contentEditable = false;
			oNewElem.className = "design_selectwrapper";
			
			$ektron("select").each(function()
			{
				var eThis = $ektron(this);
				if (0 == eThis.parents(".design_selectwrapper").length)
				{
					eThis.wrap(oNewElem);
				}
			});
		}
		if (!bShow)
		{
			if (typeof document.selection != "undefined")
			{
				if ("function" == typeof document.selection.empty)
				{
					document.selection.empty(); // selected object handles appear above the dialog
				}
			}
		}
	};
}

function design_row_onmouse(event, oSource)
{
	// source: tr/td[1]/a 
	var oRow = $ektron(oSource).parent().parent().get(0);
	var contextMenu = null;
	var sf = Ektron.SmartForm.findByChildElement(oRow);
	if (sf) contextMenu = sf.repeater.contextMenu;
	if (!contextMenu || !contextMenu.isVisible())
	{
		Ektron.SmartForm.Repeater.highlight(oRow, (event.type == "mouseover"));
	}
}

function design_row_showContextMenu(event, oElem)
{
	var sf = Ektron.SmartForm.findByChildElement(oElem);
	if (sf) Ektron.SmartForm.Repeater.current = sf.repeater; // hack
	if (sf) return sf.repeater.showContextMenu.apply(sf.repeater, arguments);
}

function design_row_setCurrent(event, oRow)
{
	var sf = Ektron.SmartForm.findByChildElement(oRow);
	if (sf) Ektron.SmartForm.Repeater.current = sf.repeater; // hack
	if (sf) return sf.repeater.setCurrent.apply(sf.repeater, arguments);
}

function design_row_insertAbove()
{
	return Ektron.SmartForm.Repeater.current.insertAbove();
}
function design_row_insertBelow()
{
	return Ektron.SmartForm.Repeater.current.insertBelow();
}
function design_row_duplicate()
{
	return Ektron.SmartForm.Repeater.current.duplicate();
}
function design_row_remove()
{
	return Ektron.SmartForm.Repeater.current.remove();
}
function design_row_moveUp()
{
	return Ektron.SmartForm.Repeater.current.moveUp();
}
function design_row_moveDown()
{
	return Ektron.SmartForm.Repeater.current.moveDown();
}
function design_row_replace()
{
	return Ektron.SmartForm.Repeater.current.replace();
}
function design_row_insertAbove_disabled()
{
	return Ektron.SmartForm.Repeater.current.insertAbove_disabled();
}
function design_row_insertBelow_disabled()
{
	return Ektron.SmartForm.Repeater.current.insertBelow_disabled();
}
function design_row_duplicate_disabled()
{
	return Ektron.SmartForm.Repeater.current.duplicate_disabled();
}
function design_row_remove_disabled()
{
	return Ektron.SmartForm.Repeater.current.remove_disabled();
}
function design_row_moveUp_disabled()
{
	return Ektron.SmartForm.Repeater.current.moveUp_disabled();
}
function design_row_moveDown_disabled()
{
	return Ektron.SmartForm.Repeater.current.moveDown_disabled();
}

/// <reference path="ektron.js" />
/* Copyright 2003-2009, Ektron, Inc. */

if (!Ektron.SmartForm.setFormValues) (function() 
{
	Ektron.SmartForm.setFormValues = function SmartForm_setFormValues(containingElement)
	{
		// Need to explicitly set the 'value' attribute to the value of the 'value' property 
		// so that innerHTML will return the current value rather than the original value.
		// Ref http://www.thescripts.com/forum/thread456550.html as seen on 2007-03-26
		// Similar for SELECT element.
		var oFormElem = Ektron.SmartForm.getContentElement(containingElement);
		if (!oFormElem) return;
		var oElem;
		var aryElems = oFormElem.getElementsByTagName("input");
		for (var i = 0; i < aryElems.length; i++)
		{
			oElem = aryElems[i];
			Ektron.SmartForm.setValue(oElem, Ektron.SmartForm.getValue(oElem));
		}
		
		aryElems = oFormElem.getElementsByTagName("textarea");
		for (var i = 0; i < aryElems.length; i++)
		{
			oElem = aryElems[i];
			Ektron.SmartForm.setValue(oElem, Ektron.SmartForm.getValue(oElem));
		}
		
		aryElems = oFormElem.getElementsByTagName("option");
		for (var i = 0; i < aryElems.length; i++)
		{
			oElem = aryElems[i];
			if (oElem.selected && oElem.value != "")
			{
				oElem.setAttribute("selected", "selected");
			}
			else
			{
				oElem.removeAttribute("selected");
			}
		}
	};

	Ektron.SmartForm.getValue = function SmartForm_getValue(oElem)
	{
		if (!oElem) return;
		var oContentElem = oElem; // oContentElem is different if oElem's content is in an iframe.
		if ("true" == oElem.getAttribute("contenteditable"))
		{
			if (Ektron.SmartForm.isEditWithIFrame(oElem))
			{
				oContentElem = oElem.firstChild.contentWindow.document.body;
				if (null == oContentElem)
				{
					// alert("IFrame body is missing");
					return; // no data
				}
			}
			else if (Ektron.SmartForm.isEditWithIFrameCache(oElem))
			{
				oContentElem = oElem.firstChild.nextSibling;
				if (null == oContentElem)
				{
					// alert("IFrame membrane is missing");
					return; // no data
				}
			}
		}
		if (typeof oElem.value != "undefined")
		{
			if ("INPUT" == oElem.tagName && ("checkbox" == oElem.type || "radio" == oElem.type))
			{
				var strValue = oElem.value + "";
				if (strValue.length > 0 && strValue != "true" && strValue != "on")
				{
					if (oElem.checked)
					{
						return strValue;
					}
					else
					{
						return "";
					}
				}
				else // boolean
				{
					if (oElem.checked)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			else
			{
				return oElem.value + ""; // Note: This string conversion is needed for Safari, 
										 // as the regular expression fails to handle the value 
										 // properly without it until some value has been placed
										 // into the input field (value may then be removed and it
										 // still works!). This way it always works properly.
										 // (Thanks Doug D! -BCB)
			}
		}
		else if (typeof oElem.getAttribute != "undefined" && oElem.getAttribute("datavalue") != null)
		{
			// Needed for Opera b/c "value" is interpreted as a number and not a string, eg, "2007-01-09" is read as "2007"
			return oElem.getAttribute("datavalue");
		}
		else if (typeof oElem.getAttribute != "undefined" && oElem.getAttribute("value") != null)
		{
			// In FireFox/Mozilla/Netscape7, the .value attribute is undefined if not standard (e.g., span)
			// and .getAttribute("value") is null when .value is standard (e.g., input).
			return oElem.getAttribute("value");
		}
		var strContent = "";
		if ("mixed" == oElem.getAttribute("ektdesignns_content") || "content-req" == oElem.getAttribute("ektdesignns_validation"))
		{
			try
			{
				strContent = Ektron.Xml.serializeXhtml(oContentElem.childNodes);
			}
			catch (ex)
			{
				Ektron.SmartForm.onexception(ex, [oContentElem.childNodes], Ektron.Xml.serializeXhtml);
			}
		}
		if (strContent.length > 0)
		{
			strContent = strContent.replace(/^\n+/,"").replace(/\n+$/,""); // trim newlines
			// If content is "blank", then empty it.
			if (Ektron.SmartForm.isBlankContent(strContent))
			{
				strContent = "";
			}
			return strContent;
		}
		if (typeof oContentElem.innerText != "undefined")
		{
			return oContentElem.innerText;
		}
		else if (typeof oContentElem.textContent != "undefined")
		{
			return oContentElem.textContent;
		}
		else if (typeof oContentElem.innerHTML != "undefined")
		{
			return $ektron.removeTags(oContentElem.innerHTML);
		}
		else
		{
			return; // no data to test
		}
	};

	// see comment in s_setFormValues
	Ektron.SmartForm.setValue = function SmartForm_setValue(oElem, value)
	{
		if (!oElem) return;
		var oContentElem = oElem; // oContentElem is different if oElem's content if in an iframe.
		if ("true" == oElem.getAttribute("contenteditable"))
		{
			if (Ektron.SmartForm.isEditWithIFrame(oElem))
			{
				oContentElem = oElem.firstChild.contentWindow.document.body;
			}
			else if (Ektron.SmartForm.isEditWithIFrameCache(oElem))
			{
				oContentElem = oElem.firstChild.nextSibling;
			}
		}
		if (typeof oElem.value != "undefined")
		{
			if ("INPUT" == oElem.tagName && ("checkbox" == oElem.type || "radio" == oElem.type))
			{
				if ("true" === value || true === value || "on" === value) // boolean
				{
					oElem.checked = true;
					oElem.setAttribute("checked", "checked");
				}
				else if ("false" === value || false === value) // boolean
				{
					oElem.checked = false;
					oElem.removeAttribute("checked");
				}
				else 
				{
					if ("string" == typeof value && value.length > 0)
					{
						oElem.value = value;
						oElem.setAttribute("value", value);
					}
					if (oElem.checked)
					{
						oElem.setAttribute("checked", "checked");
					}
					else
					{
						oElem.removeAttribute("checked");
					}
				}
			}
			else
			{
				if ("TEXTAREA" == oElem.tagName)
				{
					// Safari loses line breaks when settings innerText.
					// Safari, at least some times, returns innerText as empty string.
					if (typeof oElem.innerText != "undefined")
					{
						oElem.innerText = value;
					}
					if (typeof oElem.textContent != "undefined")
					{
						oElem.textContent = value;
					}
				}
				else
				{
					oElem.setAttribute("value", value);
					if (typeof oElem.getAttribute != "undefined" && oElem.getAttribute("datavalue") != null)
					{
						// Needed for Opera b/c "value" is interpreted as a number and not a string, eg, "2007-01-09" is read as "2007"
						oElem.datavalue = value;
						oElem.setAttribute("datavalue", value);
					}
				}
				oElem.value = value;
			}
		}
		else if (typeof oElem.getAttribute != "undefined" && oElem.getAttribute("datavalue") != null)
		{
			// Needed for Opera b/c "value" is interpreted as a number and not a string, eg, "2007-01-09" is read as "2007"
			oElem.datavalue = value;
			oElem.setAttribute("datavalue", value);
		}
		else if (typeof oElem.getAttribute != "undefined" && oElem.getAttribute("value") != null)
		{
			// In FireFox/Mozilla/Netscape7, the .value attribute is undefined if not standard (e.g., span)
			// and .getAttribute("value") is null when .value is standard (e.g., input).
			oElem.value = value;
			oElem.setAttribute("value", value);
		}
		else if (typeof oContentElem.innerHTML != "undefined" && "mixed" == oElem.getAttribute("ektdesignns_datatype"))
		{
			oContentElem.innerHTML = value;
		}
		else if (typeof oContentElem.innerText != "undefined")
		{
			oContentElem.innerText = value;
		}
		else if (typeof oContentElem.textContent != "undefined")
		{
			oContentElem.textContent = value;
		}
	};

	Ektron.SmartForm.serializeSimpleElements = function SmartForm_serializeSimpleElements(containingElement)
	{
		if (Ektron.SmartForm.cache_xmlDocument) return Ektron.SmartForm.cache_xmlDocument;
		var strXml = "<root></root>";
		// Could use Ektron.String for string builder, but currently does not depend on it and usually the serialized string is not huge.
		var oFormElem = Ektron.SmartForm.getContentElement(containingElement);
		if (oFormElem)
		{
			var analysis = 
			{
				hasRoot: false
			}
			var oSchema = Ektron.SmartForm.getContentElement("design_schema");
			var oSchemaToData = Ektron.SmartForm.getContentElement("design_schematodata_xslt");
			if (oSchema && oSchema.innerText.length > 0 && oSchemaToData && oSchemaToData.innerText.length > 0)
			{
				var sViewPage = "<ektdesign:viewPage" + s_rfnSerializeAttributes(oFormElem) + ">" + s_rfnSerializeSimpleElements(oFormElem, 0, analysis) + "</ektdesign:viewPage>";
				var strXSLT = oSchemaToData.innerText;
				strXSLT = strXSLT.replace(/<ektdesign:viewPage>[\s\S]+<\/ektdesign:viewPage>/, sViewPage);
				strXml = Ektron.SmartForm.ekXml.xslTransform(oSchema.innerText, strXSLT);
			}
			else
			{
				strXml = s_rfnSerializeSimpleElements(oFormElem, 0, analysis);
				if (!analysis.hasRoot)
				{
					strXml = "<root" + s_rfnSerializeAttributes(oFormElem) + ">" + strXml + "</root>";
				}
			}
		}
		
		Ektron.ContentDesigner.trace("SmartForm XML Document\n" + strXml);
		
		return strXml;
	};

	function s_rfnSerializeSimpleElements(oElem, level, out_analysis)
	{
		if (!oElem) return "";
		if ("design_prototype" == oElem.className) return "";
		var bIsRelevant = (oElem.getAttribute("ektdesignns_isrelevant") != "false");
		if (false == bIsRelevant) return "";

		var bProcessElement = true;
		var bind = oElem.getAttribute("ektdesignns_bind");
		var name = oElem.getAttribute("ektdesignns_name");
		var nodetype = oElem.getAttribute("ektdesignns_nodetype");
		if (!nodetype) nodetype = "element";
		var bBoundElement = ("string" == typeof bind && bind.length > 0 && "element" == nodetype);
		var bNamedElement = ("string" == typeof name && name.length > 0 && "element" == nodetype);
		var bTextNode = ("string" == typeof name && name.length > 0 && "text" == nodetype);
		if (bBoundElement)
		{
			// Just list simple elements
			if (Ektron.SmartForm.isContainerElement(oElem, oElem.tagName))
			{
				// serialize after the contents are processed
				level += 1;
			}
			else
			{
				switch (oElem.tagName)
				{
				case "TEXTAREA":
				case "SELECT":
					return Ektron.Xml.serializeXhtml(oElem);
				default:
					var value = Ektron.SmartForm.serializeValue(oElem);
					if ("undefined" == typeof value)
					{
						bProcessElement = false;
					}
					else
					{
						return Ektron.Xml.serializeXhtml(oElem);
					}
				}
			}
		}
		else if (bNamedElement)
		{
			// Just list simple elements
			if ("mixed" == oElem.getAttribute("ektdesignns_content"))
			{
				value = Ektron.SmartForm.getValue(oElem);
				return serializeElement(name, value);
			}
			else if (Ektron.SmartForm.isContainerElement(oElem, oElem.tagName))
			{
				// serialize after the contents are processed
				level += 1;
				if (!out_analysis.hasRoot && 1 == level)
				{
					var role = oElem.getAttribute("ektdesignns_role");
					if ("root" == role)
					{
						out_analysis.hasRoot = true;
					}
				}
			}
			else
			{
				var value = Ektron.SmartForm.serializeValue(oElem);
				if ("undefined" == typeof value)
				{
					bProcessElement = false;
				}
				else if ($ektron.isArray(value))
				{
					var html = "";
					for (var i = 0; i < value.length; i++)
					{
						html += serializeElement(name, $ektron.htmlEncode(value[i]));
					}
					return html;
				}
				else
				{
					return serializeElement(name, $ektron.htmlEncode(value));
				}
			}
		}
		else if (bTextNode)
		{
			var value = Ektron.SmartForm.serializeValue(oElem);
			if ("undefined" == typeof value)
			{
				bProcessElement = false;
			}
			else
			{
				return $ektron.htmlEncode(value);
			}
		}
	    
		var attrs = "";
		var content = "";
		if (bProcessElement && ekHasChildren(oElem))
		{
			var joChildren = $ektron(oElem).children();
			for (var iChild = 0; iChild < joChildren.length; iChild++)
			{
				content += s_rfnSerializeSimpleElements(joChildren.get(iChild), level, out_analysis); // Recurse
			}
			if (bBoundElement)
			{
				alert("Error: SerializeSimpleElements of a bound element is not implemented.");
				return ""; //Ektron.SmartForm.serializeHTMLElement(oElem, content);
			}
			else if (bNamedElement)
			{
				for (var iChild = 0; iChild < joChildren.length; iChild++)
				{
					attrs += s_rfnSerializeAttributes(joChildren.get(iChild)); // Recurse
				}
				return serializeElement(name, content, attrs);
			}
		}

		return content;
	}

	function s_rfnSerializeAttributes(oElem)
	{
		if (!oElem) return "";
		if ("design_prototype" == oElem.className) return "";

		var name = oElem.getAttribute("ektdesignns_name");
		var nodetype = oElem.getAttribute("ektdesignns_nodetype");
		if (!nodetype) nodetype = "element";
		var bNamed = ("string" == typeof name && name.length > 0);
		if (bNamed && "attribute" == nodetype)
		{
			// Just list simple elements
			var value = Ektron.SmartForm.serializeValue(oElem);
			if ("SPAN" == oElem.tagName && "undefined" == typeof value)
			{
				value = "";
			}
			if (typeof value != "undefined")
			{
				return serializeAttribute(name, value);
			}
		}
	    
		var attrs = "";
		if (!bNamed && ekHasChildren(oElem))
		{
			var joChildren = $ektron(oElem).children();
			for (var iChild = 0; iChild < joChildren.length; iChild++)
			{
				attrs += s_rfnSerializeAttributes(joChildren.get(iChild));
			}
		}
		return attrs;
	}

	Ektron.SmartForm.serializeValue = function SmartForm_serializeValue(oElem)
	{
		// returns value or undefined
		switch (oElem.tagName)
		{
		case "INPUT":
			var inputType = oElem.type.toLowerCase();
			if ("checkbox" == inputType)
			{
				var strValue = oElem.value.toLowerCase(); // will be "on" if no attribute
				if ("true" == strValue || "on" == strValue || "" == strValue)
				{
					// Boolean
					return (oElem.checked ? "true" : "false");
				}
			}
			else if ("text" == inputType || "password" == inputType || "hidden" == inputType)
			{
				return oElem.value;
			}
			break;
		case "TEXTAREA":
			return oElem.value;
		case "SELECT":
			if (oElem.multiple)
			{
				var values = [];
				for (var i = 0; i < oElem.length; i++)
				{
					if (oElem.options[i].selected)
					{
						values.push(oElem.options[i].value);
					}
				}
				return values;
			}
			else
			{
			return oElem.value;
			}
			break;
		case "SPAN":
			var value = oElem.getAttribute("datavalue");
			if (value != null && typeof value != "undefined")
			{
				return value;
			}
			else
			{
				value = oElem.getAttribute("value");
				if (value != null && typeof value != "undefined")
				{
					return value;
				}
			}
			break;
		default:
		}
		return; // undefined value
	};

	Ektron.SmartForm.isContainerElement = function SmartForm_isContainerElement(oElem, tagName)
	{
		if ("FIELDSET" == tagName || "DIV" == tagName)
		{
			return true;
		}
		else if ("TABLE" == tagName || "TR" == tagName)
		{
			if (oElem)
			{
				var attrValue; // initially undefined
				attrValue = oElem.getAttribute("ektdesignns_bind");
				if ("string" == typeof attrValue && attrValue.length > 0)
				{
					return true;
				}
				attrValue = oElem.getAttribute("ektdesignns_name");
				if ("string" == typeof attrValue && attrValue.length > 0)
				{
					return true;
				}
			}
		}
		return false;
	};

	function serializeElement(tagName, content, attributes)
	{
		if ("undefined" == typeof attributes) attributes = "";
		var tag = tagName.toLowerCase();
		var bEmptyTag = ("xsl:" == tag.substr(0,4) || (1 == Ektron.Xml.htmlTagCount[tag]));
		if (bEmptyTag && ("undefined" == typeof content || ("string" == typeof content && 0 == content.length) || (null == content)))
		{
			return "<" + tagName + attributes + " />\n";
		}
		else
		{
			return "<" + tagName + attributes + ">" + content + "</" + tagName + ">\n";
		}
	}
	
	function serializeAttribute(name, value)
	{
		return " " + name + "=\"" + $ektron.htmlEncode(value) + "\"";
	}

	// from ektron.string.js
	if (!$ektron.htmlEncode)
	{
		$ektron.htmlEncode = function(text) { return (text+"").replace(Ektron.RegExp.Char.amp, "&amp;").replace(Ektron.RegExp.Char.lt, "&lt;").replace(Ektron.RegExp.Char.gt, "&gt;").replace(Ektron.RegExp.Char.quot, "&quot;"); };
	}
	if (!$ektron.removeTags)
	{		
		$ektron.removeTags = function(html) { return (html+"").replace(Ektron.RegExp.tags, ""); };
	}

})(); // Ektron.SmartForm namespace extensions
/// <reference path="ektron.js" />
/* Copyright 2003-2011, Ektron, Inc. */

if (!Ektron.SmartForm.onsubmitForm) (function() 
{
	Ektron.SmartForm.onsubmitForm = function SmartForm_onsubmitForm(form)
	// form is name or index of, or reference to, the form element to validate.
	// Returns true if valid element, or false.
	{
		var oElem = Ektron.SmartForm.validateHtmlForm(form);
		if (oElem && oElem.title != "") {
			alert(oElem.title);
			Ektron.SmartForm.focusOn(oElem);
			return false;
		}
		return true;
	};

	Ektron.SmartForm.prevalidateForm = function SmartForm_prevalidateForm(containingElement)
	// containingElement may be element name (string) or object reference
	{
		if (Ektron.SmartForm.prevalidatingForm) return;
		var oFormElem = Ektron.SmartForm.getContentElement(containingElement);
		if (!oFormElem) return;
		
		Ektron.SmartForm.prevalidatingForm = true;
		Ektron.SmartForm.cache_xmlDocument = Ektron.SmartForm.serializeSimpleElements(containingElement);
		var oFirstInvalidElem = Ektron.SmartForm.prevalidateElement(oFormElem);
		Ektron.SmartForm.cache_xmlDocument = "";
		Ektron.SmartForm.prevalidatingForm = false;
		Ektron.SmartForm.markValidity("dsg_prevalidate", oFirstInvalidElem);
		return oFirstInvalidElem;
	};

	Ektron.SmartForm.validateForm = function SmartForm_validateForm(strElemName) // dual static and instance method
	// returns true if valid, false if at least one field contains an invalid value.
	{
		var oFormElem = this.getContentElement(strElemName); // dual static and instance method
		Ektron.SmartForm.unfixContentEditable(oFormElem);
		Ektron.SmartForm.recalcForm(oFormElem);
		// Check if any field is invalid, if so, get first invalid field.
		var oElem = Ektron.SmartForm.prevalidateForm(oFormElem);
		var invalidMessage = "";
		if (oElem)
		{
			invalidMessage = oElem.title;
			Ektron.SmartForm.focusOn(oElem);
		}
		this.isValid = (null == oElem);
		this.invalidElement = oElem;
		this.invalidMessage = invalidMessage;
		Ektron.SmartForm.markValidity("dsg_validate", oElem);
		Ektron.SmartForm.fixContentEditable(oFormElem); // async, must be last
		return (null == oElem);
	};

	Ektron.SmartForm.markValidity = function SmartForm_markValidity(strResultName, oInvalidElem)
	{
		var oResultElem = document.getElementById(strResultName);
		if (oResultElem)
		{
			// NOTE: when VB is polling for ektdesignns_isvalid, setting the property flags that validation is complete.
			if (oInvalidElem)
			{
				oResultElem.title = oInvalidElem.title;
				oResultElem.setAttribute("ektdesignns_isvalid", false); 
			}
			else
			{
				oResultElem.title = "";
				oResultElem.setAttribute("ektdesignns_isvalid", true); 
			}
		}
	};

	Ektron.SmartForm.validateHtmlForm = function SmartForm_validateHtmlForm(form)
	// form is name or index of, or reference to, the form element to validate.
	// Returns first invalid element, or null.
	{
		// #34658 - Ektron Form Validation Does Not Fire On Pages with an asp:ScriptManager
		// Problem is that ScriptManager calls form onsubmit function with 'this' referring to its own object rather than the form element. That object has a '_form' property that points to the form element.
		if (form && form._form) form = form._form; 
		
		var oForm;
		switch (typeof form)
		{
		case "string":
		case "number":
			oForm = document.forms[form];
			break;
		case "object":
			oForm = form;
			break;
		default:
			oForm = document.forms[0];
			break;
		}
		if (!oForm) return null;
		return Ektron.SmartForm.prevalidateElement(oForm);
	};

	Ektron.SmartForm.validateElement = function SmartForm_validateElement(oElem)
	{
		var oInvalidElem = Ektron.SmartForm.prevalidateElement(oElem);
		var containingElement = Ektron.SmartForm.findContentElement(oElem);
		Ektron.SmartForm.recalcForm(containingElement);
		return (null == oInvalidElem);
	};
	
	Ektron.SmartForm.prevalidateElement = function SmartForm_prevalidateElement(oElem, oFirstInvalidElem)
	{
		if ("undefined" == typeof oFirstInvalidElem) oFirstInvalidElem = null;
		if (!oElem) return oFirstInvalidElem;
		if ("undefined" == typeof oElem.getAttribute) return oFirstInvalidElem;
		if ("design_prototype" == oElem.className) return oFirstInvalidElem;
		
		var relevant = oElem.getAttribute("ektdesignns_relevant");
		if (relevant)
		{
			oElem.removeAttribute("ektdesignns_isrelevant");
			if (false == Ektron.SmartForm.evaluateXPathBoolean(relevant, oElem))
			{
				oElem.setAttribute("ektdesignns_isrelevant", "false");
				$ektron(oElem).hide();
				return oFirstInvalidElem;
			}
			else
			{
				$ektron(oElem).show();
			}
		}
		
		if ("object" == typeof oElem.currentStyle && oElem.currentStyle != null) 
		{
			if ("none" == oElem.currentStyle.display) return oFirstInvalidElem;
			if ("hidden" == oElem.currentStyle.visibility) return oFirstInvalidElem;
		}
		var validation = oElem.getAttribute("ektdesignns_validation");
		if (validation && validation != "none")
		{
			oElem.removeAttribute("ektdesignns_isvalid");
			design_validate_result = true; // just in case onblur handler fails to set the result
			if ("function" == typeof oElem.onblur)
			{
				oElem.onblur(); // return value in global design_validate_result
			}
			else //if ("string" == typeof oElem.onblur) //for mac browsers
			{	 // or "object" - again, for mac (safari), see notes below.
				// Safari 2.0.4/Mac typeof oElem.onblur is "function"
				var sFn = oElem.getAttribute("onblur");
				if (sFn)
				{
					try
					{
						var fnOnBlur = new Function(sFn);
						fnOnBlur.call(oElem);
					}
					catch (ex)
					{
						// ********************************************************
						//    ATTENTION
						//      
						//		  Safari appears to invoke the new function we 
						//		create here (to handle the on-blur event (and 
						//		perform the validation) in the wrong context; 
						//      it runs in the calling windows context - which
						//      is a problem when we use things like the date
						//      picker popup, functions/etc that exist in the 
						//      main window are not available in the popup window
						//      contentext - so the code fails...
						//      
						//        If, instead, we evaluate the function then it
						//      runs in the context of this main window and behaves
						//      properly. We catch that failure and attempt to 
						//      handle it for Safari (or any other similarly 
						//      mis-behaving browser).
						//      
						//      Note that we cannot use the 'this' pointer
						//      in this case, we must replace it with the variable
						//      name that points to the element object - in this 
						//      case 'oElem' Also note that we asume that the 
						//      this pointer will only be passed once in the parameter
						//      list, that it will come before the comments, etc.,
						//      otherwise the following regular expression will need
						//      to be updated.
						//      
						// ********************************************************
						sFn = sFn.replace(/([\(\,]\s*)this(\s*[\,\)])/, '$1oElem$2');
						var fn = new Function(sFn);
						eval(sFn);
					}
				}
			}
			if (null == oFirstInvalidElem && false == design_validate_result) 
			{
				oFirstInvalidElem = oElem;
			}
		}

		if (typeof oElem.childNodes != "undefined") 
		{
			for (var i = 0; i < oElem.childNodes.length; i++)
			{
				if (1 /*Node.ELEMENT_NODE*/ == oElem.nodeType)
				{
					oFirstInvalidElem = Ektron.SmartForm.prevalidateElement(oElem.childNodes.item(i), oFirstInvalidElem);
				}
			}
		}
		return oFirstInvalidElem;
	};
	
	function s_countSimpleElements(containingElement)
	{
		var nCount = 0;
		var oFormElem = Ektron.SmartForm.getContentElement(containingElement);
		if (oFormElem)
		{
			// This doesn't account for calendar (date) fields, which should be
			// OK because they are not used in calculations. At this time, this
			// function is used to determine whether the calculation fields need
			// to be updated.
			var aryTagNames = ["input", "select", "textarea"];
			var aryElems;
			var oElem;
			for (var iTagName = 0; iTagName < aryTagNames.length; iTagName++)
			{
				aryElems = oFormElem.getElementsByTagName(aryTagNames[iTagName]);
				for (var i = 0; i < aryElems.length; i++)
				{
					oElem = aryElems[i];
					if (Ektron.SmartForm.isDDFieldElement(oElem))
					{
						nCount++; 
					}
				}
			}
		}
		return nCount;
	}

	function s_detectChange(containingElement)
	{
		var oFormElem = Ektron.SmartForm.getContentElement(containingElement);
		if (!oFormElem) return;
		// INPUT, TEXTAREA, and SELECT support onchange
		var aryTagNames = ["input", "select", "textarea"];
		var aryElems;
		var oElem;
		for (var iTagName = 0; iTagName < aryTagNames.length; iTagName++)
		{
			aryElems = oFormElem.getElementsByTagName(aryTagNames[iTagName]);
			for (var i = 0; i < aryElems.length; i++)
			{
				oElem = aryElems[i];
				if (Ektron.SmartForm.isDDFieldElement(oElem))
				{
					try
					{
                        // #66433: need to bind the form validation onchange/onclick events on top of any customer added events.
                        var eElem = $ektron(oElem);
                        if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) < 9)
                        {
                            //ie 8 and earlier calls the original events again after s_onChange.
                            var eventName, jEventName;
						if ("checkbox" == oElem.type)
						{
                                eventName = "onclick";
                                jEventName = "click";
						}
						else
						{
                                eventName = "onchange";
                                jEventName = "change";
                            }
                            var prevEvent = eElem.attr(eventName);
                            var newEventFunction;
                            if ("undefined" !== typeof (prevEvent))
                            {
                                newEventFunction = function (event)
                                {
                                    eval(prevEvent);
                                    s_onChange();
                                };
                                eElem.removeAttr(eventName);
                            }
                            else
                            {
                                newEventFunction = function (event)
                                {
                                    s_onChange();
                                }
                            }
                            eElem.bind(jEventName, function(event) {
                                newEventFunction(event);
                            });
                        }
                        else
                        {
                            // ie 9+, FF, Chrome do not have the circular event in IE 8.
                            if ("checkbox" == oElem.type)
						    {
							    eElem.bind("click", s_onChange);
						    }
						    else
						    {
							    eElem.bind("change", s_onChange);
						    }
						}
					}
					catch (ex)
					{
						Ektron.OnException(Ektron.SmartForm, Ektron.OnException.ignoreException, ex, arguments);
					}
				}
			}
		}
	}

	function s_onChange()
	// 'this' is the element that raised the event.
	{
		// The onchange event fires prior to the onblur event, so we need to call prevalidateElement manually
		// before recalculating the form in case the element has onblur defined to normalize the value.
		Ektron.SmartForm.validateElement(this);
	}

	Ektron.SmartForm.precalcForm = function SmartForm_precalcForm(containingElement)
	{
		// Count the number of simple elements
		var nSimpleElementCount = 0; // if the number changes, the form needs to be recalculated
		var oFormElem = Ektron.SmartForm.getContentElement(containingElement);
		if (!oFormElem) return;
		s_detectChange(oFormElem);
		nSimpleElementCount = s_countSimpleElements(oFormElem);
		Ektron.SmartForm.recalcForm(oFormElem);
		var sfInstance = Ektron.SmartForm.findByChildElement(oFormElem);
		if (sfInstance.watchdogTimer)//should only have one watchdog per instance.
		{
		    clearInterval(sfInstance.watchdogTimer);
		    sfInstance.watchdogTimer = null;
		}
		sfInstance.watchdogTimer = setInterval(function SmartForm_watchdogChange(containingElement)
		{
			var oFormElem = Ektron.SmartForm.getContentElement(containingElement);
			if (!oFormElem) return;
			var nCount = s_countSimpleElements(oFormElem);
			// Check if fields were added or removed.
			if (nSimpleElementCount != nCount)
			{
				// check if fields were added, so reassign onchange event handlers so the new field is considered.
				if (nSimpleElementCount < nCount)
				{
					s_detectChange(oFormElem);
				}
				nSimpleElementCount = nCount;
				Ektron.SmartForm.recalcForm(oFormElem);
			}
		}, 2000);
	};

	Ektron.SmartForm.recalcForm = function SmartForm_recalcForm(containingElement)
	{
		// Search for ektdesignns_calculate expando attribute.
		// At this time, only INPUT elements may have the 'calculate' attr. 
		// Add other elements as needed.
		var oFormElem = Ektron.SmartForm.getContentElement(containingElement);
		if (!oFormElem) return;
		var aryElems = oFormElem.getElementsByTagName("input");
		var oElem;
		var exprCalc;
		for (var i = 0; i < aryElems.length; i++)
		{
			oElem = aryElems[i];
			exprCalc = oElem.getAttribute("ektdesignns_calculate");
			if ("string" == typeof exprCalc)
			{
				if ("xpathr:" == exprCalc.substring(0, 7)) // only xpathr is supported for now
				{
					// Can't cached the xml doc b/c calculating changes the value.
					design_normalize_xpathr("string(" + exprCalc.substring(7) + ")", oElem);
					// Validate here in case it needs to be normalized.
					Ektron.SmartForm.prevalidateElement(oElem);
				}
			}
		}
		var sfInstance = Ektron.SmartForm.findByChildElement(oFormElem);
		if (!sfInstance.designMode)
		{
		    $ektron("[ektdesignns_relevant]", oFormElem).each(function()
		    {
			    Ektron.SmartForm.prevalidateElement(this);
		    });
		}
	};
	
	Ektron.SmartForm.evaluateXPathBoolean = function SmartForm_evaluateXPathBoolean(expression, oElem)
	{
		var oFormElem = Ektron.SmartForm.findContentElement(oElem);

		var value = Ektron.SmartForm.serializeSimpleElements(oFormElem);
		if ("" == value) return; // no data to test
		var sXPath = Ektron.SmartForm.getXPath(oElem, oFormElem);
		if ("" == sXPath) return; // no xpath, probably prototype
		
		expression = expression.replace(/^xpathr\:/, ""); 
		var strXSLT = Ektron.SmartForm.buildXpathrTemplate(sXPath, "<xsl:value-of select=\"boolean(" + $ektron.htmlEncode(expression) + ")\"/>\n");

		var result = ("true" == Ektron.SmartForm.ekXml.xslTransform(value, strXSLT));
		
		Ektron.ContentDesigner.trace("evaluateXPathBoolean Elem='%s' XPath='%s' Expression='%s' Result=%s", oElem.id, sXPath, expression, result);

		return result;
	};

	Ektron.SmartForm.evaluate = function SmartForm_evaluate(expression, value)
	{
		// 'expression' is evaluated such that 'this.text' in the expression is the 'value' argument.
		return (new Function("return " + expression)).call({ text: value + "" });
	};

	Ektron.SmartForm.normalize_complete = function SmartForm_normalize_complete(oElem, value)
	{
		Ektron.SmartForm.setValue(oElem, value);
	};

	Ektron.SmartForm.validate_complete = function SmartForm_validate_complete(oElem, result, invalidmsg)
	{
		design_validate_result = result;
		if (!oElem) return result;
		// Netscape 4.7 does not support oElem.title and oElem.style.
		
		if (invalidmsg && "string" == typeof oElem.title)
		{
			// Remove message from title attribute if it was appended.
			var p = oElem.title.indexOf(invalidmsg);
			if (p >= 0)
			{
				if (p > 0 && "\n" == oElem.title.charAt(p-1))
				{
					p -= 1;
				}
				oElem.title = oElem.title.substring(0, p);
			}
			// remove trailing line breaks
			p = oElem.title.length - 1;
			if (p >= 0 && "\n" == oElem.title.charAt(p))
			{
				while (p >= 0 && "\n" == oElem.title.charAt(p))
				{
					p--;
				}
				oElem.title = oElem.title.substring(0, p);
			}
		}

		if (!result)
		{
			if (invalidmsg && ("string" == typeof oElem.title))
			{
				// Append message to title attribute unless it is already present.
				if (-1 == oElem.title.indexOf(invalidmsg))
				{
					if (oElem.title.length > 0)
					{
						oElem.title += " \n";
					}
					oElem.title += invalidmsg;	
				}
			}
		}

		var bStyled = false;
		// Check for the presence of a customer defined validation-styling function:
		if ("function" == typeof customValidationStyle)
		{
			// call the users custom validation-styling function:
			bStyled = customValidationStyle(oElem, result);
		}
		if (false === bStyled)
		{
			// use our built-in validation-styling function:
			s_validationStyle(oElem, result);
		}
	};

	function s_validationStyle(oElem, isValid)
	{
		var parent = null;
		var elTypeName = oElem.tagName;

		var specialCaseBorder = (($ektron.browser.mozilla || $ektron.browser.safari) && ("INPUT" == elTypeName && "checkbox" == oElem.type))
								|| ("SELECT" == elTypeName);
		
		if ("object" == typeof oElem)
		{
			parent = oElem.parentNode;
			
			if (("object" == typeof oElem.style) && ("object" == typeof parent))
			{
				if (isValid)
				{
					if (specialCaseBorder)
					{
						if (("SPAN" == parent.tagName) 
							&& ("design_validation_failed" == parent.className))
						{
							parent.className = "design_validation_passed";
						}
					}
					else
					{
						$ektron(oElem).removeClass("design_validation_failed");
					}
				}
				else
				{
					var sf = Ektron.SmartForm.findByChildElement(oElem);
					if (sf && (("undefined" == typeof sf.designMode) || (sf.designMode != true)))
					{
						if (specialCaseBorder)
						{
							// Ensure that the element is wrapped in our own 
							// span tag, so we can control the border style:
							if ((parent.tagName != "SPAN") 
								|| ((parent.className != "design_validation_failed")
									&& (parent.className != "design_validation_passed")))
							{
								var wrapper = document.createElement("span");
								wrapper = parent.insertBefore(wrapper, oElem);
								oElem = parent.removeChild(oElem);
								oElem = wrapper.appendChild(oElem);
								parent = wrapper;
							}

							parent.className = "design_validation_failed";
						}
						else
						{
							$ektron(oElem).addClass("design_validation_failed");
						}
					}
				}
			}
		}
	}
	
	// We don't need the precision of Ektron.RegExp.CharacterClass.s, so just use \s for better performance.
	Ektron.RegExp.IsBlankWhitespace = new RegExp("^(\\s|\xa0|&nbsp;|&#160;)+$", "i");
	Ektron.RegExp.IsBlankParagraph = new RegExp("^\\s*<p>(\\s|\xa0|&nbsp;|&#160;)*<\\/p>\\s*$", "i");
	Ektron.RegExp.IsXhtmlBreakTagOnly = new RegExp("^\\s*<br\\s[^\\/]*\/>\\s*$", "i");
	Ektron.RegExp.IsContentUsability = new RegExp("^\\s*<(p|span)[^>]+contentUsability[^>]+>(\\s|\xa0|&nbsp;|&#160;)*<\\/\\1>\\s*$", "i");

	Ektron.SmartForm.isBlankContent = function SmartForm_isBlankContent(content)
	{
		var bIsBlank =  
			Ektron.RegExp.IsBlankWhitespace.test(content) || 
			Ektron.RegExp.IsBlankParagraph.test(content) || 
			Ektron.RegExp.IsXhtmlBreakTagOnly.test(content) ||
			Ektron.RegExp.IsContentUsability.test(content);
		return bIsBlank;
	};
	
	Ektron.SmartForm.focusOn = function SmartForm_focusOn(oElem)
	{
		try
		{
			oElem.scrollIntoView();
		} 
		catch (ex) { } // ignore
		try
		{
			oElem.focus();
		} 
		catch (ex) { } // ignore
	};
})(); // Ektron.SmartForm namespace extensions
/// <reference path="ektron.js" />
/* Copyright 2003-2009, Ektron, Inc. */

if (!Ektron.SmartForm.buildXslt) (function() 
{
	Ektron.SmartForm.buildXslt = function SmartForm_buildXslt(templates, method)
	// templates (string) of XSLT templates
	// method (optional string), valid values: "xml", "html" or "text"
	// returns (string) XSLT document
	{
		if (!method) method = "xml";
		
		var strXSLT = [
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">",
		"<xsl:param name=\"currentDate\" select=\"'undefined'\"/>",
		"<xsl:output method=\"" + method + "\" version=\"1.0\" encoding=\"UTF-8\" indent=\"yes\" omit-xml-declaration=\"yes\"/>",
		"<xsl:decimal-format NaN=\"\"/>",
		templates,
		// Override pre-defined templates
		"<xsl:template match=\"*|/\">",
		"    <xsl:apply-templates select=\"@*|node()\"/>",
		"</xsl:template>",
		"<xsl:template match=\"text()|@*\"/>", // don't emit text nodes
		"</xsl:stylesheet>"].join("\n");

		return strXSLT;
	};

	function s_numberMatch(oElem, oCurrentElem, sXPath)
	{
		var number = 1;
		var bFound = false;
		if (!oElem || !oCurrentElem) return 1;
		$ektron("*", oElem).each(function()
		{
			if (oCurrentElem == oElem) 
			{
				bFound = true;
				return false; // exit loop
			}
			if (sXPath == this.getAttribute("ektdesignns_bind")) number++;
		});
		return (bFound ? number : 1);
	}

	Ektron.SmartForm.getXPath = function SmartForm_getXPath(oCurrentElem, containingElement)
	{
		var sXPath = "/";
		var oFormElem = Ektron.SmartForm.getContentElement(containingElement);
		if (oFormElem && oCurrentElem)
		{
			var bind = oCurrentElem.getAttribute("ektdesignns_bind");
			if ("string" == typeof bind && bind.length > 0)
			{
				sXPath = bind + "#" + s_numberMatch(oFormElem, oCurrentElem, bind);
			}
			else
			{
				sXPath = s_rfnGetXPath(oFormElem, oCurrentElem, "/root", {}); 
			}
		}
		return sXPath;
	};

	function s_rfnGetXPath(oElem, oCurrentElem, sXPath, predicates)
	{
		if (!oElem) return "";
		if ("design_prototype" == oElem.className) return "";

		var bProcessElement = true;
		var name = oElem.getAttribute("ektdesignns_name");
		if ("string" == typeof name && name.length > 0)
		{
			// Just list simple elements
			if (Ektron.SmartForm.isContainerElement(oElem, oElem.tagName))
			{
				bProcessElement = true;
			}
			else
			{
				var value = Ektron.SmartForm.serializeValue(oElem);
				bProcessElement = (typeof value != "undefined");
			}
			
			if (bProcessElement)
			{
				var nodetype = oElem.getAttribute("ektdesignns_nodetype");
				if (!nodetype) nodetype = "element";
				switch (nodetype)
				{
				case "element":
					if ("/root" == sXPath)
					{
						var role = oElem.getAttribute("ektdesignns_role");
						if ("root" == role)
						{
							sXPath = "";
						}
					}
					sXPath += "/" + name;
					var nPredicate = predicates[sXPath];
					if ("number" == typeof nPredicate)
					{
						nPredicate++;
					}
					else
					{
						nPredicate = 1;
					}
					predicates[sXPath] = nPredicate;
					sXPath += "[" + nPredicate + "]";
					break;
				case "attribute":
					sXPath += "/@" + name;
					break;
				case "mixed":
					// no change
					break;
				case "text":
					// no change
					break;
				default:
					// no change
				}
			}
			
			if (oCurrentElem == oElem)
			{
				return sXPath;
			}
		}
	  
		if (bProcessElement && ekHasChildren(oElem))
		{	
			var xpath = "";
			var joChildren = $ektron(oElem).children();
			for (var iChild = 0; iChild < joChildren.length; iChild++)
			{
				xpath = s_rfnGetXPath(joChildren.get(iChild), oCurrentElem, sXPath, predicates); // Recurse
				if (xpath.length > 0)
				{
					return xpath; 
				}
			}
		}

		return "";
	}

	Ektron.SmartForm.buildXpathrTemplate = function SmartForm_buildXpathrTemplate(sXPath, sCode)
	{
		var strXSLT = '';
	    
		var pNumber = sXPath.indexOf("#");
		if (-1 == pNumber)
		{
			strXSLT = [
			"<xsl:template match=\"" + sXPath + "\">",
			"    " + sCode,
			"</xsl:template>"].join('\n');
		}
		else
		{
			sNumber = sXPath.substring(pNumber+1);
			sXPath = sXPath.substring(0,pNumber);
			
			strXSLT = [
			"<xsl:template match=\"" + sXPath + "\">",
			"    <xsl:variable name=\"number\">",
			"        <xsl:number count=\"" + sXPath + "\" level=\"any\"",
			"    </xsl:variable>",
			"    <xsl:if test=\"$number=" + sNumber + "\">",
			"        " + sCode,
			"    </xsl:if>",
			"</xsl:template>"].join('\n');
		}
		
		strXSLT = Ektron.SmartForm.buildXslt(strXSLT);
		
		//Ektron.ContentDesigner.trace("buildXpathrTemplate XPath='%s' sCode='%s' XSLT='%s'", sXPath, sCode, strXSLT);
		
		return strXSLT;
	};
})(); // Ektron.SmartForm namespace extensions

var design_validate_result = true;

function design_div_focus(oElement)
{
	var bPrimeEmptyElement = false;
	if ( ! oElement.hasChildNodes())
	{
		bPrimeEmptyElement = true;
	}
	else if (1 == oElement.childNodes.length && 3 /*Node.TEXT_NODE*/ == oElement.firstChild.nodeType)
	{
		var strText = oElement.firstChild.nodeValue;
		if (0 == strText.length || /^\n+$/.test(strText))
		{
			bPrimeEmptyElement = true;
		}
	}
	if (bPrimeEmptyElement)
	{
		if ($ektron.browser.msie)
		{
			oElement.innerHTML = "<p>&#160;</p>";
		}
		else if ($ektron.browser.safari)
		{
			oElement.innerHTML = "<p><br class=\"khtml-block-placeholder\" /></p>";
		}
	}
}

function design_div_blur(oElement)
{
}

function design_toggleExpandCollapse(oElem)
{
	var sf = Ektron.SmartForm.findByChildElement(oElem);
	if (sf) return sf.toggleExpandCollapse(oElem);
}

function design_normalize_xpathr(expression, oElem)
{
	var oFormElem = Ektron.SmartForm.findContentElement(oElem);
	if (Ektron.SmartForm.prevalidatingForm) return; // only normalize for actual onblur event
	var value = Ektron.SmartForm.serializeSimpleElements(oFormElem); 
	if ("" == value) return; // no data to test
	var sXPath = Ektron.SmartForm.getXPath(oElem, oFormElem);
	if ("" == sXPath) return; // no xpath, probably prototype
	
    var strXSLT = Ektron.SmartForm.buildXpathrTemplate(sXPath, "<xsl:copy-of select=\"" + $ektron.htmlEncode(expression) + "\"/>\n");

	value = Ektron.SmartForm.ekXml.xslTransform(value, strXSLT);
	if ("NaN" == value) value = "";

	Ektron.ContentDesigner.trace("normalize_xpathr Elem=%s XPath='%s' Expression='%s' Value='%s'", oElem.id, sXPath, expression, value);

	Ektron.SmartForm.normalize_complete(oElem, value);
}

function design_validate_xpathr(expression, oElem, invalidmsg)
{
	var result = Ektron.SmartForm.evaluateXPathBoolean(expression, oElem);
	if (typeof result != "boolean") return;
	Ektron.SmartForm.validate_complete(oElem, result, invalidmsg);
	return result;
}

function design_normalize_xpath(expression, oElem)
{
	if (Ektron.SmartForm.prevalidatingForm) return; // only normalize for actual onblur event
	var value = Ektron.SmartForm.getValue(oElem);
	if ("undefined" == typeof value) return; // no data to test
	
    var strXSLT = [
    "<xsl:template match=\"/root\">",
    "    <xsl:copy-of select=\"" + $ektron.htmlEncode(expression) + "\"/>",
    "</xsl:template>"].join('\n');
	
	strXSLT = Ektron.SmartForm.buildXslt(strXSLT);
	
	value = "<root>" + value + "</root>"; 

	value = Ektron.SmartForm.ekXml.xslTransform(value, strXSLT);
	if ("NaN" == value) value = "";

	Ektron.SmartForm.normalize_complete(oElem, value);
}

function design_validate_xpath(expression, oElem, invalidmsg)
{
	var value = Ektron.SmartForm.getValue(oElem);
	if ("undefined" == typeof value) return; // no data to test

    var strXSLT = [    
    "<xsl:template match=\"/root\">",
    "    <xsl:value-of select=\"boolean(" + $ektron.htmlEncode(expression) + ")\"/>",
    "</xsl:template>"].join('\n');
	
	strXSLT = Ektron.SmartForm.buildXslt(strXSLT, "text");
	
	value = "<root>" + value + "</root>"; 

	var result = ("true" == Ektron.SmartForm.ekXml.xslTransform(value, strXSLT));
	
	Ektron.SmartForm.validate_complete(oElem, result, invalidmsg);
	return result;
}

function design_normalize_xslt(url, oElem)
{
	if (Ektron.SmartForm.prevalidatingForm) return; // only normalize for actual onblur event
	var value = Ektron.SmartForm.getValue(oElem);
	if ("undefined" == typeof value) return; // no data to test
	
	value = "<root>" + value + "</root>"; 

	value = Ektron.SmartForm.ekXml.xslTransform(value, url);
	if ("NaN" == value) value = "";
	
	Ektron.SmartForm.normalize_complete(oElem, value);
}

function design_validate_xslt(url, oElem, invalidmsg)
{
	var value = Ektron.SmartForm.getValue(oElem);
	if ("undefined" == typeof value) return; // no data to test

	value = "<root>" + value + "</root>"; 

	var result = ("true" == Ektron.SmartForm.ekXml.xslTransform(value, url));
	
	Ektron.SmartForm.validate_complete(oElem, result, invalidmsg);
	return result;
}

// legacy
function design_validate(re, oElem, value) 
// value is optional
// returns true if valid or indeterminate, false if value fails reg exp.
{
	if (!oElem) return;
	var data = value;
	if ("undefined" == typeof value)
	{
		data = Ektron.SmartForm.getValue(oElem);
		if ("undefined" == typeof data)
		{
			return; // no data to test
		}
	}

	var result = re.test(data);

	Ektron.SmartForm.validate_complete(oElem, result);
	return result;
}

function design_normalize_re(re, oElem)
{
	// only normalize for actual onblur event
	if (typeof Ektron.SmartForm.prevalidatingForm == "undefined" || Ektron.SmartForm.prevalidatingForm != true) 
	{
		var value = Ektron.SmartForm.getValue(oElem);
		if ("undefined" == typeof value) return; // no data to test
		if ("undefined" != typeof RegExp.lastIndex) 
		{
			RegExp.lastIndex = 0;
		}
		re.lastIndex = 0;
		
		var ary = re.exec(value);
		
		value = (null == ary ? "" : ary[0]);
		Ektron.SmartForm.normalize_complete(oElem, value);
	}
}

function design_validate_re(re, oElem, invalidmsg)
{
	var value = Ektron.SmartForm.getValue(oElem);
	if ("undefined" == typeof value) return; // no data to test
	if ("undefined" != typeof RegExp.lastIndex) 
	{
		RegExp.lastIndex = 0;
	}
	re.lastIndex = 0;

	var result = re.test(value);

	Ektron.SmartForm.validate_complete(oElem, result, invalidmsg);

	return result;
}

function design_normalize_js(expression, oElem)
{
	// only normalize for actual onblur event
	if (typeof Ektron.SmartForm.prevalidatingForm == "undefined" || Ektron.SmartForm.prevalidatingForm != true) 
	{
		var value = Ektron.SmartForm.getValue(oElem);
		if ("undefined" == typeof value) return; // no data to test
		
		value = Ektron.SmartForm.evaluate(expression, value);
		
		Ektron.SmartForm.normalize_complete(oElem, value);
	}
}

function design_validate_js(expression, oElem, invalidmsg)
// value is optional
// returns true if valid or indeterminate, false if value fails reg exp.
{
	var value = Ektron.SmartForm.getValue(oElem);
	if ("undefined" == typeof value) return; // no data to test
	
	var result = Ektron.SmartForm.evaluate(expression, value);
	
	Ektron.SmartForm.validate_complete(oElem, result, invalidmsg);
	return result;
}

function design_validate_select(minIndex, oElem, invalidmsg) 
// minIndex = -1, 0, 1 etc.. (-1 = not selected; 0 = 1st on list etc)
// returns true if valid or indeterminate, false if index is 0 or -1.
{
	if (!oElem) return;
	if ("undefined" == typeof oElem.selectedIndex)
	{
		return; // not a select element
	}

	var result = (oElem.selectedIndex >= minIndex);
	// this has no visual effect on select tag, but it is needed to set the design_validate_result (global var).
	Ektron.SmartForm.validate_complete(oElem, result, invalidmsg);
	return result;
}

function design_validate_choice(minSelected, maxSelected, oElem, invalidmsg) 
// returns true if valid or indeterminate, false otherwise.
// maxSelected = -1 if it has no limits.
{
	if (!oElem) return;
	if ("undefined" == typeof oElem.getElementsByTagName) return;
	var num_checked = 0;
	var oCurrElem;
	var bUseChecked;
	var aryElements = null;
	var validation = oElem.getAttribute("ektdesignns_validation");
	if ("choice-req" == validation)
	{
		aryElements = oElem.getElementsByTagName("input");
		bUseChecked = true;
	}
	else if ("select-req" == validation) //list box
	{
		aryElements = oElem.getElementsByTagName("option");
		bUseChecked = false;
	}
	if (aryElements)
	{
		for (var i = 0; i < aryElements.length; i++)
		{
			oCurrElem = aryElements[i];
			if (bUseChecked)
			{
				if (oCurrElem.checked)
				{
					num_checked++;
				}
			}
			else //list box
			{
				if (oCurrElem.selected)
				{
					num_checked++;
				}
			}
		}
	}
	var result = (minSelected <= num_checked && (maxSelected <= 0 || num_checked <= maxSelected));	
	Ektron.SmartForm.validate_complete(oElem, result, invalidmsg);
	return result;
}

function design_normalize_isbn(value)
{
	value = value + "";
	value = value.replace(/[\s\-]/g, "").toUpperCase(); // remove spaces and hyphens
	return value;
}

function design_validate_isbn(value)
// returns true if valid or indeterminate, false otherwise.
{
	var result = design_validate_isbn10(value) || design_validate_isbn13(value);
	return result;
}

function design_validate_isbn10(value)
// returns true if valid or indeterminate, false otherwise.
{
	var result = true;
	value = value + "";
	var re = new RegExp("^[0-9]{9}[0-9X]$"); // or "^[0-9 \-]{9,12}[0-9X]$"
	if (!re.test(value)) return false;
	
	// adapted from http://www.merlyn.demon.co.uk/js-misc0.htm#ISBN
	var check = 0;
	var weight = 10;
	for (var i = 0; i < value.length; i++) 
	{
		var c = value.charCodeAt(i);
		if (88 == c && 1 == weight) // final X
		{ 
			check += 10; 
			weight--;
		} 
		else if (48 <= c && c <= 57) // 0-9 
		{
			check += (c - 48) * weight--;
		}		
	}
	result = (0 == weight && 0 == (check % 11));
	return result;
}

function design_validate_isbn13(value)
// returns true if valid or indeterminate, false otherwise.
{
	value = value + "";
	var re = new RegExp("^[0-9]{13}$"); // or "^[0-9 \-]{13,17}$"
	if (!re.test(value)) return false;
	
	// adapted from http://www.merlyn.demon.co.uk/js-misc0.htm#ISBN
	var check = 0;
	var n = 13;
	var weight = 1;
	for (var i = 0; i < value.length; i++) 
	{
		var c = value.charCodeAt(i);
		if (48 <= c && c <= 57) // 0-9 
		{
			check += (c - 48) * weight;
			weight = (1 == weight ? 3 : 1); // toggle b/n 1 and 3
			n--;
		}		
	}
	return (0 == n && 0 == (check % 10));
}

function design_normalize_issn(value)
{
	value = value + "";
	value = value.replace(/[\s\-]/g, "").toUpperCase(); // remove spaces and hyphens
	return value;
}

function design_validate_issn(value)
// returns true if valid or indeterminate, false otherwise.
{
	value = value + "";
	var re = new RegExp("^[0-9]{7}[0-9X]$"); // or "^[0-9]{4}\-?[0-9]{3}[0-9X]$"
	if (!re.test(value)) return false;
	
	// adapted from http://www.merlyn.demon.co.uk/js-misc0.htm#ISBN
	var check = 0;
	var weight = 8;
	for (var i = 0; i < value.length; i++) 
	{
		var c = value.charCodeAt(i);
		if (88 == c && 1 == weight) // final X
		{ 
			check += 10; 
			weight--;
		} 
		else if (48 <= c && c <= 57) // 0-9 
		{
			check += (c - 48) * weight--;
		}		
	}
	return (0 == weight && 0 == (check % 11));
}

function design_current_date()
{
	// Returns current date in format yyyy-mm-dd
	var oCurrentDate = new Date();
	var mm = (oCurrentDate.getMonth() + 1);
	if (mm <= 9) mm = "0" + mm;
	var dd = oCurrentDate.getDate();
	if (dd <= 9) dd = "0" + dd;
	return (oCurrentDate.getFullYear() + "-" + mm + "-" + dd);
}

//function design_default_current_date(date)
//{
//	date = date + "";
//	if (date.length != 10) 
//	{
//		date = design_current_date();
//		var oTempDate = new Date(date.substr(0,4), parseInt(date.substr(5,2),10)-1, date.substr(8,2));
//		var strDate = (oTempDate.toLocaleDateString ? oTempDate.toLocaleDateString() : oTempDate.toLocaleString());
//		var oDateElem = oElem.firstChild;
//		while (oDateElem && oDateElem.tagName != "INPUT")
//		{
//			oDateElem = oDateElem.nextSibling;
//		}
//		if (oDateElem != null) oDateElem.value = strDate;
//	}
//	return date;
//}

function design_validate_future_date(date)
// returns true if valid or indeterminate, false otherwise.
{
	date = date + "";
	if (10 == date.length) 
	{
		return (date >= design_current_date());
	}
	return false;
}
/// <reference path="ektron.js" />
/* Copyright 2003-2009, Ektron, Inc. */

if (!Ektron.DataListManager) (function() 
{
	Ektron.DataListManager = 
	{
		init: function DataListManager_init(settings)
		{
			settings = $ektron.extend(
			{
				serverUrl: location.protocol + "//" + location.host,
				srcPath: "",
				langType: "", // LCID (decimal, not hex)
				ekXml: null
			}, settings);
			settings.langType = settings.langType + ""; // ensure as string
			if (0 == settings.srcPath.indexOf(settings.serverUrl))
			{
				// strip off serverUrl from beginning of srcPath
				settings.srcPath = settings.srcPath.substr(settings.serverUrl.length);
			}

			s_settings = settings;
					
			if (!Ektron.Xml)
			{
				throw new ReferenceError("Ektron.DataListManager depends on Ektron.Xml. Please include ektron.xml.js.");
			}
			
			this.replaceDataLists = DataListManager_replaceDataLists;
			this.getDataList = DataListManager_getDataList;
		}
	};

	var s_settings = {};
	
	function DataListManager_replaceDataLists(containingElement)
	{
// TODO is this needed?
//		if (!document || !document.body)
//		{
//			setTimeout(function()
//			{
//				arguments.callee.apply(me, arguments);
//			}, 200); // too soon, try again later
//			return;
//		}
		var aryDatalistCache = new Array();
		$ektron("select[ektdesignns_datasrc], div.ektdesignns_choices[ektdesignns_datasrc], div.ektdesignns_checklist[ektdesignns_datasrc], div.design_choices[ektdesignns_datasrc], div.design_checklist[ektdesignns_datasrc]", containingElement).each(function()
		{
			var oElem = this;
			var datasrc = oElem.getAttribute("ektdesignns_datasrc");
			var datalist = oElem.getAttribute("ektdesignns_datalist");
			if ("string" == typeof datalist && datalist.length > 0)
			{
				if ("undefined" == typeof aryDatalistCache[datalist])
				{
					var strSelect = oElem.getAttribute("ektdesignns_dataselect");
					var strCaptionXPath = oElem.getAttribute("ektdesignns_captionxpath");
					var strValueXPath = oElem.getAttribute("ektdesignns_valuexpath");
					var strNamespaces = oElem.getAttribute("ektdesignns_datanamespaces");
					aryDatalistCache[datalist] = Ektron.DataListManager.getDataList(oElem.tagName, datasrc, strSelect, strCaptionXPath, strValueXPath, strNamespaces);
					// Datalist will be empty if running in editor b/c access is denied.
				}
				if (aryDatalistCache[datalist].length > 0)
				{
					s_replaceDataList(oElem, aryDatalistCache[datalist]);
				}
			} // if datalist
		}); // each
	}

	function s_replaceDataList(oElem, strDatalist)
	{
		if ("SELECT" == oElem.tagName)
		{
			var nNumOrigItemsToKeep = 0;
			var validation = oElem.getAttribute("ektdesignns_validation");
			if ("select-req" == validation)
			{
				nNumOrigItemsToKeep = 1;
			}
			var strOrigDataList = "";
			for (var iOption = 0; iOption < oElem.options.length; iOption++)
			{
				var oOption = oElem.options[iOption];
				oOption.innerHTML = $ektron.htmlEncodeText(oOption.text);
				strOrigDataList += Ektron.Xml.serializeXhtml(oOption);
			}
			var strHtml = s_transformDataList(strOrigDataList, strDatalist, nNumOrigItemsToKeep);
			strHtml = strHtml.replace(/<select[^>]*>/,"").replace("</select>",""); // remove SELECT tag
			$ektron(oElem).html(strHtml); // innerHTML for SELECT element doesn't work, so use jQuery
			if (oElem.multiple && oElem.size < 2 && oElem.options.length > 12)
			{
				oElem.size = 12;
			}
		}
		else // choices and checklist
		{
			var oOrigListElem = $ektron(oElem).children("OL").get(0);
			var strOrigDataList = Ektron.Xml.serializeXhtml(oOrigListElem);
			// Can't mix DHTML DOM and XML DOM nodes, so transform returns string of HTML.
			var strHtml = s_transformChoiceDataList(strOrigDataList, strDatalist);
			strHtml = strHtml.replace(/<ol[^>]*>/,"").replace("</ol>",""); // remove OL tag, it's same as orig
			oOrigListElem.innerHTML = strHtml; // replace LI elements
		}
	};
	    
	function DataListManager_getDataList(strDDFieldTagName, strSource, strSelect, strCaptionXPath, strValueXPath, strNamespaces, strLangType)
	{
	// Fetches data list from strSource
		// assert("SELECT" == strDDFieldTagName || "DIV" == strDDFieldTagName)
	    
		var strPrefixes = "";
		if ("undefined" == typeof strNamespaces || null == strNamespaces)
		{
			strNamespaces = "";
		}
		else
		{
			strPrefixes = extractPrefixesFromNamespaces(strNamespaces);
			if (strPrefixes.length > 0) 
			{
				strPrefixes = " exclude-result-prefixes=\"" + strPrefixes + "\"";
			}
		}
	    
		var langType = (strLangType || s_settings.langType) + "";
		if (langType.length > 0)
		{
			langType = "&LangType=" + langType;
		}
		strSource = strSource.replace("&LangType=-1", langType);
	    
		var strXSLT = [
		"<?xml version=\"1.0\"?>",
		"<xsl:stylesheet version=\"1.0\" " + strPrefixes + " xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" " + strNamespaces + ">",
		"<xsl:output method=\"xml\" version=\"1.0\" omit-xml-declaration=\"yes\" indent=\"yes\"/>",
		"<xsl:param name=\"localeUrl\" select=\"'" + s_settings.serverUrl + s_settings.srcPath + "resourcexml.aspx?name=DataListSpec" + langType.replace("&","&amp;") + "'\"/>",
		"<xsl:variable name=\"localeXml\" select=\"document($localeUrl)/*\"/>",
		"<xsl:template match=\"/\">",
		"  <select>",
		"  <xsl:for-each select=\"" + strSelect + "\">",
		"    <option>",
		"      <xsl:if test=\"" + strValueXPath + "\">",
		"        <xsl:attribute name=\"value\">",
		"          <xsl:value-of select=\"" + strValueXPath + "\"/>",
		"        </xsl:attribute>",
		"      </xsl:if>",
		"      <xsl:choose>",
		"      	 <xsl:when test=\"@localeRef\">",
		"      	   <xsl:value-of select=\"$localeXml/data[@name=current()/@localeRef]/value/text()\"/>",
		"      	 </xsl:when>",
		"      	 <xsl:otherwise>",
		"      	   <xsl:value-of select=\"" + strCaptionXPath + "\"/>",
		"      	 </xsl:otherwise>",
		"      </xsl:choose>",
		"    </option>",
		"  </xsl:for-each>",
		"  </select>",
		"</xsl:template>",
		"</xsl:stylesheet>"].join('\n');
	    
		// Transform
		var strHtml = s_settings.ekXml.xslTransform(strSource, strXSLT);
		strHtml = $ektron.trim(strHtml);
	    
		if (strHtml.indexOf("<option") >= 0 || 0 == strHtml.length) 
		{
			return strHtml;
		} 
		else 
		{
			if (Ektron.Xml.onexception) Ektron.Xml.onexception(new Error("Function getDataList returned unexpected result.\n" + strHtml), arguments);
			return ""; // transform error
		}
	}

	function s_transformChoiceDataList(strOrigDataList, strNewDataList)
	{
		// bug The document('') function is the web page in IE6 and the xml source document in Mozilla
		var strXSLT = "";
		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">",
		"<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>",
		"<xsl:template match=\"ol\">",
		"<xslout:variable name=\"nameID\" select=\"'{li/input/@name}'\"/>",
		"<xslout:variable name=\"inputType\" select=\"'{li/input/@type}'\"/>",
		"</xsl:template>",
		"<xsl:template match=\"text()\"/>",
		"</xsl:stylesheet>"].join('\n');
		var strVariables = s_settings.ekXml.xslTransform(strOrigDataList, strXSLT);

		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">",
		"<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>",
		"<xsl:template match=\"/\">",
		"	<xsl:apply-templates/>",
		"</xsl:template>",
		"<xsl:template match=\"ol/li/input[@checked]\">",
		"	<xslout:if test=\"not(option[@value=xpathLiteralString{@value}gnirtSlaretiLhtapx])\">", 
		"    	<xsl:copy-of select=\"..\"/>", // copy LI element
		"	</xslout:if>",
		"</xsl:template>",
		"<xsl:template match=\"text()\"/>",
		"</xsl:stylesheet>"].join('\n');
		var strOldSelectedSnip = s_settings.ekXml.xslTransform(strOrigDataList, strXSLT);

		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">",
		"<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>",
		"<xsl:template match=\"/\">",
		"	<xsl:for-each select=\"ol/li/input[1]/@*[starts-with(name(),'ektdesignns_')]\">",
		"		<xslout:attribute name=\"{name()}\"><xsl:value-of select=\".\"/></xslout:attribute>",
		"	</xsl:for-each>",
		"	<xsl:apply-templates/>",
		"</xsl:template>",
		"<xsl:template match=\"ol/li/input[@checked]\">",
		"	<xslout:if test=\"@value=xpathLiteralString{@value}gnirtSlaretiLhtapx\">", 
		"           <xslout:attribute name=\"checked\">checked</xslout:attribute>",
		"	</xslout:if>",
		"</xsl:template>",
		"<xsl:template match=\"text()\"/>",
		"</xsl:stylesheet>"].join('\n');
		var strNewSelectedSnip = s_settings.ekXml.xslTransform(strOrigDataList, strXSLT);

		var strOlTag = strOrigDataList.match(/<ol[^>]*>/)[0];
		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" exclude-result-prefixes=\"ektdesign\" xmlns:ektdesign=\"urn:ektdesign\">",
		"<xsl:output method=\"xml\" version=\"1.0\" indent=\"yes\" omit-xml-declaration=\"yes\"/>",
	////    "<ektdesign:datalist>",
	////    strOrigDataList; // original ol/li list
	////    "</ektdesign:datalist>",
		// bug The document('') function is the web page in IE6 and the xml source document in Mozilla
	////    "<xsl:variable name=\"oldDataList\" select=\"document('')/xsl:stylesheet/ektdesign:datalist\"/>",
	////    "<xsl:variable name=\"nameID\" select=\"$oldDataList/ol/li/input/@name\"/>",
	////    "<xsl:variable name=\"inputType\" select=\"$oldDataList/ol/li/input/@type\"/>",
	////    "<xsl:variable name=\"ektAttr\" select=\"$oldDataList/ol/li/input/@*[starts-with(name(),'ektdesignns_')]\"/>",
		strVariables,
		// copy original OL tag
		"<xsl:template match=\"/\">",
	////    "    <ol>",
	////    "        <xsl:copy-of select=\"$oldDataList/ol/@*\"/>",
		"	" + strOlTag + "",
		"        <xsl:apply-templates/>",
		"    </ol>",
		"</xsl:template>",
		// copy checked values that are not in the new data list
		"<xsl:template match=\"select\">",
	////    "    <xsl:copy-of select=\"$oldDataList/ol/li/input[@checked and not(@value=current()/option/@value)]\"/>",
		strOldSelectedSnip,
		"    <xsl:apply-templates select=\"node()\"/>",
		"</xsl:template>",
		// process option tags
		"<xsl:template match=\"option\">",
		"    <xsl:variable name=\"modelID\" select=\"generate-id()\"/>",
		"    <xsl:variable name=\"displayOption\" select=\"text()\"/>",
		"    <xsl:variable name=\"valueOption\" select=\"@value\"/>",
		"     <li>",
		// copy attributes except 'checked'
		"    <input type=\"{$inputType}\" id=\"{$modelID}\" title=\"{$displayOption}\" value=\"{$valueOption}\" name=\"{$nameID}\" ektdesignns_nodetype=\"item\">",
	////    "    <xsl:copy-of select=\"$ektAttr\"/>",
		// check if checked in the old data list
	////    "    <xsl:if test=\"$oldDataList/ol/li/input[@value=current()/@value]/@checked\">",
	////    "        <xsl:attribute name=\"checked\">checked</xsl:attribute>",
	////    "    </xsl:if>",
		strNewSelectedSnip,
		"    </input>",
		"    <label for=\"{$modelID}\"><xsl:value-of select=\"$displayOption\"/></label>",
		"    </li>",
		"</xsl:template>",
		// copy the text
		"<xsl:template match=\"*\">",
		"   <xsl:copy>",
		"       <xsl:copy-of select=\"@*\"/>",
		"       <xsl:apply-templates select=\"node()\"/>",
		"   </xsl:copy>",
		"</xsl:template>",
	    
		"</xsl:stylesheet>"].join('\n');

		// Transform
		var strHtml = s_settings.ekXml.xslTransform(strNewDataList, strXSLT);
		return strHtml;
	}

	function s_transformDataList(strOrigDataList, strNewDataList, nNumOrigItemsToKeep)
	{
		// bug The document('') function is the web page in IE6 and the xml source document in Mozilla
		strOrigDataList = "<select>" + strOrigDataList + "</select>";
		var strOldPredicate = "@selected";
		if ("number" == typeof nNumOrigItemsToKeep)
		{
			strOldPredicate += " or position()&lt;=" + nNumOrigItemsToKeep;
		}
		var strXSLT = "";
		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">",
		"<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>",
		"<xsl:template match=\"/\">",
		"	<xsl:apply-templates/>",
		"</xsl:template>",
		"<xsl:template match=\"option[" + strOldPredicate + "]\">",
		"	<xslout:if test=\"not(option[@value=xpathLiteralString{@value}gnirtSlaretiLhtapx])\">", 
		"    	<xsl:copy-of select=\".\"/>",
		"	</xslout:if>",
		"</xsl:template>",
		"<xsl:template match=\"text()\"/>",
		"</xsl:stylesheet>"].join('\n');
		var strOldSelectedSnip = s_settings.ekXml.xslTransform(strOrigDataList, strXSLT);

		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">",
		"<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>",
		"<xsl:template match=\"/\">",
		"	<xsl:apply-templates/>",
		"</xsl:template>",
		"<xsl:template match=\"option[@selected]\">",
		"	<xslout:if test=\"@value=xpathLiteralString{@value}gnirtSlaretiLhtapx\">", 
		"           <xslout:attribute name=\"selected\">selected</xslout:attribute>",
		"	</xslout:if>",
		"</xsl:template>",
		"<xsl:template match=\"text()\"/>",
		"</xsl:stylesheet>"].join('\n');
		var strNewSelectedSnip = s_settings.ekXml.xslTransform(strOrigDataList, strXSLT);
		
		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" exclude-result-prefixes=\"ektdesign\" xmlns:ektdesign=\"urn:ektdesign\">",
		"<xsl:output method=\"xml\" version=\"1.0\" indent=\"yes\" omit-xml-declaration=\"yes\"/>",
	////    "<ektdesign:datalist>",
	////    strOrigDataList; // original OPTION list
	////    "</ektdesign:datalist>",
		// bug The document('') function is the web page in IE6 and the xml source document in Mozilla
	////    "<xsl:variable name=\"oldDataList\" select=\"document('')/xsl:stylesheet/ektdesign:datalist\"/>",
		"<xsl:template match=\"select\">",
		"   <select>", 
		"   <!-- copy selected values that are not in the new data list -->",
	////    "   <xsl:copy-of select=\"$oldDataList/option[@selected and not(@value=current()/option/@value)]\"/>",
		strOldSelectedSnip,
	    
		"   <!-- process option tags -->",
		"   <xsl:apply-templates select=\"node()\"/>",
		"   </select>",
		"</xsl:template>",
		"<xsl:template match=\"option\">",
		"   <xsl:copy>",
		"       <!-- copy attributes except 'selected' -->",
		"       <xsl:copy-of select=\"@*[name() != 'selected']\"/>",
		"       <!-- check if selected in the old data list -->",
	////    "       <xsl:if test=\"$oldDataList/option[@value=current()/@value]/@selected\">",
	////    "           <xsl:attribute name=\"selected\">selected</xsl:attribute>",
	////    "       </xsl:if>",
		strNewSelectedSnip,
	    
		"       <!-- copy the text -->",
		"       <xsl:copy-of select=\"node()\"/>",
		"       </xsl:copy>",
		"</xsl:template>",
		"<xsl:template match=\"*\"> ",
		"   <xsl:copy>",
		"       <xsl:copy-of select=\"@*\"/>",
		"       <xsl:apply-templates select=\"node()\"/>",
		"   </xsl:copy>",
		"</xsl:template>",
		"</xsl:stylesheet>"].join('\n');
	    
		// Transform
		var strHtml = s_settings.ekXml.xslTransform(strNewDataList, strXSLT);
		strHtml = $ektron.trim(strHtml);
	    
		if (strHtml.indexOf("<option") >= 0 || 0 == strHtml.length) 
		{
			return strHtml;
		} 
		else 
		{
			if (Ektron.Xml.onexception) Ektron.Xml.onexception(new Error("Function transformDataList returned unexpected result.\n" + strHtml), arguments);
			return ""; // transform error
		}
	}

	// private static
	function extractPrefixesFromNamespaces(strNamespaces)
	// strNamespaces is a delimited list of namespace declarations
	{
		var aryNSPrefix = new Array();
		var aryAllNS = strNamespaces.match(/xmlns:\w+=['"][^'"]*['"]/g);
		if (null == aryAllNS) return "";
		aryAllNS.sort();
		var prev = "";
		for (var i = 0; i < aryAllNS.length; i++)
		{
			if (aryAllNS[i] != prev) // exclude duplicates
			{
				var aryNS = aryAllNS[i].match(/xmlns:(\w+)=['"]([^'"]*)['"]/);
				aryNSPrefix[aryNSPrefix.length] = aryNS[1];
				// URI = aryNS[2];
				prev = aryAllNS[i];
			}
		}
		return aryNSPrefix.join(" ");
	}
})(); // Ektron.DataListManager namespace

