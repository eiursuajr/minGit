(function()
{
    var m_objFormField = null;
    var m_oFieldElem = null;

    var m_bChangingState = false;

    var m_bPrevRichArea = false;
    var m_bPrevMultiLine = false;
    var m_bPrevReadOnly = false;
    var m_bPrevHidden = false;
    var m_bPrevPassword = false;

    window.initField = function initField()
    {
        m_objFormField = new EkFormFields();
        m_oFieldElem = null;

        var oContentElement = null;
        var oFieldElem = null;
        var bIsRootLoc = false;
        var sContentTree = "";
        var sDefaultId = "";
        var sDefaultPrefix = "";
        var args = GetDialogArguments();
        if (args)
        {
            oContentElement = args.contentElement;
            oFieldElem = args.selectedField;
            bIsRootLoc = args.isRootLocation;
            sDefaultPrefix = args.fieldPrefix;
            sDefaultId = args.fieldId;
            sContentTree = args.contentTree;
        }
        if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.setRootTagVisible(bIsRootLoc);
        ekFieldValidationControl.init(oContentElement);
        ekFieldValidationControl.loadContentTree(sContentTree);
        ekFieldValidationControl.setDefaultVal("string");

        if (m_objFormField.isDDFieldElement(oFieldElem))
        {
            var type = "";
            var tagName = oFieldElem.tagName;
            var inputType = oFieldElem.type;
            var bReadOnly = false;
            if ("INPUT" == tagName && ("text" == inputType || "hidden" == inputType || "password" == inputType))
            {
                m_bReload = true;
                type = "plaintext";
                bReadOnly = oFieldElem.readOnly;
                document.getElementById("txtDefVal").value = oFieldElem.value;
                document.getElementById("taDefVal").value = "";
                document.getElementById("richDefVal").value = "";
            }
            else if ("TEXTAREA" == tagName)
            {
                type = "textarea";
                strContent = oFieldElem.value;
                bReadOnly = oFieldElem.readOnly;
                document.getElementById("txtDefVal").value = "";
                document.getElementById("taDefVal").value = oFieldElem.value;
                document.getElementById("richDefVal").value = "";
            }
            else if ("DIV" == tagName && $ektron(oFieldElem).hasClass("ektdesignns_richarea"))
            {
                type = "richarea";
                strContent = oFieldElem.innerHTML;
                document.getElementById("txtDefVal").value = "";
                document.getElementById("taDefVal").value = ""; 
                //#59688
                var defVal = "";
                var eChild = $ektron(oFieldElem);
                var fieldTxt = eChild.text();
                fieldTxt = $ektron.trim(fieldTxt);
                if (fieldTxt.length > 0)
                {
                    eChild.removeAttr("data-ektron-preserve");
                    defVal = Ektron.Xml.serializeXhtml(oFieldElem.childNodes);
                }
                document.getElementById("richDefVal").value = defVal;
                var attr = oFieldElem.getAttribute("readonly");
                bReadOnly = ("string" == typeof attr && attr.length > 0);
            }

            if (type)
            {
                ekFieldNameControl.read(oFieldElem);
                if (typeof ekFieldUseControl != "undefined") ekFieldUseControl.read(oFieldElem);
                if (typeof ekFieldAllowControl != "undefined") ekFieldAllowControl.read(oFieldElem);
                if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.read(oFieldElem);
                ekFieldDataStyleControl.read(oFieldElem);
                ekFieldValidationControl.read(oFieldElem);
                ekFieldStyleControl.read(oFieldElem);

                // Option checkboxes
                m_bChangingState = true;
                document.getElementById("chkRichArea").checked = ("richarea" == type);
                document.getElementById("chkMultiline").checked = ("textarea" == type || "richarea" == type || "hidden" == inputType || $ektron.toBool(oFieldElem.getAttribute("ektdesignns_hidden")));
                document.getElementById("chkReadOnly").checked = bReadOnly;
                document.getElementById("chkHidden").checked = ("hidden" == inputType || $ektron.toBool(oFieldElem.getAttribute("ektdesignns_hidden")));
                document.getElementById("chkPassword").checked = ("password" == inputType);
                document.getElementById("chkKeepPTag").checked = ("richarea" == type && "string" == typeof oFieldElem.getAttribute("ektdesignns_datadisplay"));
                m_bChangingState = false;

                m_oFieldElem = oFieldElem;
            }
            else
            {
                // need to load the default Id when it is not one of the Text field type.
                loadDefaultNames(sDefaultPrefix, sDefaultId);
            }
            ekFieldNameControl.initSetting(true);
        }
        else
        {
            loadDefaultNames(sDefaultPrefix, sDefaultId);
            ekFieldNameControl.initSetting(false);
        }
        if (typeof ekFieldAdvancedControl != "undefined")
        {
            ekFieldAdvancedControl.setDefaultVal(ResourceText.sValue);
            ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
        }
        updateDisplay();
    }

    window.insertField = function insertField()
    {
        if (false == validateDialog())
        {
            return false;
        }

        var bRichArea = document.getElementById("chkRichArea").checked;
        var bMultiline = document.getElementById("chkMultiline").checked;
        var bReadOnly = document.getElementById("chkReadOnly").checked;
        var bHidden = document.getElementById("chkHidden").checked;
        var bPassword = document.getElementById("chkPassword").checked;

        var tagName = "";
        var inputType = "";
        if (bRichArea)
        {
            tagName = "DIV";
        }
        else if (bMultiline && !bHidden)
        {
            tagName = "TEXTAREA";
        }
        else
        {
            tagName = "INPUT";
            if (bPassword)
            {
                inputType = "password";
            }
            else if (bHidden)
            {
                inputType = "hidden";
            }
            else
            {
                inputType = "text";
            }
        }

        //FireFox does not render type="hidden" as a visible field.
        var viewInputType = inputType;
        if ("hidden" == viewInputType)
        {
            viewInputType = "text";
        }
        var oFieldElem = m_oFieldElem;
        if (null == oFieldElem || oFieldElem.tagName != tagName)
        {
            if ("INPUT" == tagName)
            {
                oFieldElem = $ektron("<input type=\"" + viewInputType + "\" name=\"" + ekFieldNameControl.getName() + "\" />").get(0);
                if ("text" == inputType)
                {
                    oFieldElem.maxlength = 2000;
                }
            }
            else if ("TEXTAREA" == tagName)
            {
                oFieldElem = $ektron("<textarea name=\"" + ekFieldNameControl.getName() + "\"></textarea>").get(0);
            }
            else // if ("DIV" == tagName) // richarea 
            {
                oFieldElem = document.createElement("div");
                oFieldElem.className = "ektdesignns_richarea";
            }
        }
        else if (window.radWindow && window.radWindow.IsIE && oFieldElem.name != ekFieldNameControl.getName())
        {
            // "The NAME attribute cannot be set at run time on elements dynamically created with the createElement method." - MSDN
            var bCopyAttributes = false;
            if ("INPUT" == oFieldElem.tagName)
            {
                oFieldElem = $ektron("<input type=\"" + viewInputType + "\" name=\"" + ekFieldNameControl.getName() + "\" />").get(0);
                bCopyAttributes = true;
            }
            else if ("TEXTAREA" == oFieldElem.tagName)
            {
                oFieldElem = $ektron("<textarea name=\"" + ekFieldNameControl.getName() + "\"></textarea>").get(0);
                bCopyAttributes = true;
            }
            if (bCopyAttributes)
            {
                for (var i = 0; i < m_oFieldElem.attributes.length; i++)
                {
                    var attr = m_oFieldElem.attributes[i];
                    if (attr.specified && attr.name != "type" && attr.name != "class")//#60607
                    {
                        oFieldElem.setAttribute(attr.name, attr.value);
                    }
                }
            }
        }
        else if (oFieldElem.tagName == tagName && "INPUT" == tagName && oFieldElem.type != inputType)
        {
            oFieldElem = $ektron("<input type=\"" + viewInputType + "\" name=\"" + ekFieldNameControl.getName() + "\" />").get(0);
            if ("text" == inputType)
            {
                oFieldElem.maxlength = 2000;
            }
        }

        if (typeof ekFieldAdvancedControl != "undefined")
        {
            ekFieldAdvancedControl.updateControl(ekFieldNameControl, ((typeof ekFieldAllowControl != "undefined") ? ekFieldAllowControl.isRepeatable() : false));
            ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
            ekFieldAdvancedControl.updateFieldAllowControl(ekFieldAllowControl);
            ekFieldAdvancedControl.updateFieldUseControl(ekFieldUseControl);
        }
        else
        {
            oFieldElem.setAttribute("ektdesignns_nodetype", "element");
        }
        ekFieldNameControl.update(oFieldElem);
        if (typeof ekFieldUseControl != "undefined") ekFieldUseControl.update(oFieldElem);
        if (typeof ekFieldAllowControl != "undefined") ekFieldAllowControl.update(oFieldElem);
        if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.update(oFieldElem);
        ekFieldDataStyleControl.update(oFieldElem);
        ekFieldValidationControl.update(oFieldElem);
        ekFieldStyleControl.update(oFieldElem);

        if ("DIV" == tagName) // richarea 
        {
            var sHTML = (document.getElementById("richDefVal").value || "&#160;"); // need a space in the DIV for FF to click in it.
            $ektron(oFieldElem).html(sHTML);
            oFieldElem.removeAttribute("ektdesignns_datadisplay");
            if (document.getElementById("chkKeepPTag").checked)
            {
                oFieldElem.setAttribute("ektdesignns_datadisplay", "block");
            }
        }
        else // INPUT or TEXTAREA
        {
            var joFieldElem = $ektron(oFieldElem);
            if (bReadOnly)
            {
                //oFieldElem.readOnly = true; //readOnly attribute is "read only". 
                oFieldElem.setAttribute("readonly", "readonly");
                //add a class to this tag so the look can be changed in ekforms.css
                joFieldElem.addClass("design_readonly");
            }
            else
            {
                //oFieldElem.readOnly = false; //readOnly attribute is "read only". 
                oFieldElem.removeAttribute("readonly");
                if (joFieldElem.hasClass("design_readonly"))
                {
                    joFieldElem.removeClass("design_readonly");
                }
            }
            var oDefaultValueElem;
            joFieldElem.removeClass("design_textfield");
            //#60607: jquery 1.5.1 fails to remove class attribute for IE b/c className attribute is empthy but class attribute has its value.
            var elemStyleClass = oFieldElem.getAttribute("class");
            if (elemStyleClass && elemStyleClass.length > 0)
            {
                oFieldElem.removeAttribute("class");
            }
            if ("INPUT" == tagName)
            {
                oFieldElem.removeAttribute("ektdesignns_hidden");
                if (bHidden)
                {
                    oDefaultValueElem = document.getElementById("taDefVal");
                    oFieldElem.setAttribute("ektdesignns_hidden", "true");
                    oFieldElem.value = oDefaultValueElem.value;
                }
                else
                {
                    oDefaultValueElem = document.getElementById("txtDefVal");
                    if ("text" == joFieldElem.attr("type"))
                    {
                        joFieldElem.addClass("design_textfield");
                    }
                    oFieldElem.value = oDefaultValueElem.value;
                }
                oFieldElem.setAttribute("value", oFieldElem.value);
            }
            else // if ("TEXTAREA" == tagName)
            {
                oDefaultValueElem = document.getElementById("taDefVal");
                oFieldElem.value = oDefaultValueElem.value;
                joFieldElem.addClass("design_textfield");
                joFieldElem.text(oFieldElem.value);
            }

            if (bHidden || bReadOnly)
            {
                if (!validateDefaultValue(oFieldElem, oDefaultValueElem))
                {
                    return false;
                }
            }
        }

        CloseDlg(oFieldElem);
    }

    function loadDefaultNames(sDefaultPrefix, sDefaultId)
    {
        ekFieldNameControl.setDefaultFieldNames(sDefaultPrefix, sDefaultId);
        if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.setName(sDefaultPrefix + sDefaultId);
    }

    window.updateDisplay = function updateDisplay()
    {
        updateOptions();

        updateDefaultValue();

        var objHidden = document.getElementById("chkHidden");
        if (objHidden.checked)
        {
            if (typeof ekFieldUseControl != "undefined") ekFieldUseControl.disable();
            if (typeof ekFieldAllowControl != "undefined") ekFieldAllowControl.disable();
        }
        else
        {
            if (typeof ekFieldUseControl != "undefined") ekFieldUseControl.enable();
            if (typeof ekFieldAllowControl != "undefined")
            {
                if (typeof ekFieldAdvancedControl != "undefined")
                {
                    ekFieldAdvancedControl.updateFieldAllowControl(ekFieldAllowControl);
                    ekFieldAdvancedControl.updateFieldUseControl(ekFieldUseControl);
                }
                else
                {
                    ekFieldAllowControl.enable();
                    ekFieldUseControl.enable();
                }
            }
        }
        var bRichArea = document.getElementById("chkRichArea").checked;
        var bMultiline = document.getElementById("chkMultiline").checked;
        var bReadOnly = document.getElementById("chkReadOnly").checked;
        var bHidden = document.getElementById("chkHidden").checked;
        var bPassword = document.getElementById("chkPassword").checked;

        //        var bDisabledV8n = (bHidden || bReadOnly);
        for (var i = 0; i < RadTabStrip1.Tabs.length; i++)
        {
            //            if ("Validation" == RadTabStrip1.Tabs[i].Value)
            //            {
            //                //Hidden and Read only do not allow validation task.
            //                if (bDisabledV8n)
            //                { 
            //                    RadTabStrip1.Tabs[i].Disable();    
            //                }
            //                else
            //                {
            //                    RadTabStrip1.Tabs[i].Enable();
            //                }
            //            }            
            //            else 
            if ("DataStyle" == RadTabStrip1.Tabs[i].Value)
            {
                //Rich Area does not allow data style task.
                if (bRichArea)
                {
                    RadTabStrip1.Tabs[i].Disable();
                }
                else
                {
                    RadTabStrip1.Tabs[i].Enable();
                }
            }
        }
    }

    function updateDefaultValue()
    {
        var objRichArea = document.getElementById("RichAreaDefaultContainer");
        var objMultiline = document.getElementById("MultilineDefaultContainer");
        var objSingleLine = document.getElementById("SingleLineDefaultContainer");

        var bRichArea = document.getElementById("chkRichArea").checked;
        var bMultiline = !bRichArea && document.getElementById("chkMultiline").checked;
        var bSingleLine = !bRichArea && !bMultiline;

        var bPrevRichArea = false;
        var bPrevMultiline = false;
        var bPrevSingleLine = false;

        var strDefaultValue = "";
        if (objRichArea.style.display != "none")
        {
            bPrevRichArea = true;
            //			strDefaultValue = document.getElementById("richDefVal").value;
        }
        if (objMultiline.style.display != "none")
        {
            bPrevMultiline = true;
            strDefaultValue = document.getElementById("taDefVal").value;
        }
        if (objSingleLine.style.display != "none")
        {
            bPrevSingleLine = true;
            strDefaultValue = document.getElementById("txtDefVal").value;
        }

        if (bRichArea && !bPrevRichArea)
        {
            if (strDefaultValue.length > 0) document.getElementById("richDefVal").value = strDefaultValue;
            objRichArea.style.display = "";
            objMultiline.style.display = "none";
            objSingleLine.style.display = "none";
        }
        else if (bMultiline && !bPrevMultiline)
        {
            if (strDefaultValue.length > 0 && bPrevSingleLine) document.getElementById("taDefVal").value = strDefaultValue;
            objRichArea.style.display = "none";
            objMultiline.style.display = "";
            objSingleLine.style.display = "none";
        }
        else if (bSingleLine && !bPrevSingleLine)
        {
            //            document.getElementById("txtDefVal").value = strDefaultValue;
            objRichArea.style.display = "none";
            objMultiline.style.display = "none";
            objSingleLine.style.display = "";
        }
    }

    function updateOptions()
    {
        if (m_bChangingState) return;
        m_bChangingState = true;

        var objRichArea = document.getElementById("chkRichArea");
        var objMultiline = document.getElementById("chkMultiline");
        var objReadOnly = document.getElementById("chkReadOnly");
        var objHidden = document.getElementById("chkHidden");
        var objPassword = document.getElementById("chkPassword");
        var objKeepPTag = document.getElementById("chkKeepPTag");

        var bRichArea = objRichArea.checked;
        var bMultiline = objMultiline.checked;
        var bReadOnly = objReadOnly.checked;
        var bHidden = objHidden.checked;
        var bPassword = objPassword.checked;

        var bDisabled = false;

        // RichArea
        bDisabled = false;
        if (!objRichArea.disabled && bDisabled) m_bPrevRichArea = bRichArea;
        if (objRichArea.disabled && !bDisabled) bRichArea = m_bPrevRichArea;
        objRichArea.checked = (bRichArea && !bDisabled);
        objRichArea.disabled = bDisabled;
 
        objKeepPTag.disabled = true;
        if (objRichArea.checked) 
        {
            objKeepPTag.disabled = false;
        }

        // Multiline
        bDisabled = (bPassword || bHidden || bRichArea);
        if (!objMultiline.disabled && bDisabled) m_bPrevMultiline = bMultiline;
        if (objMultiline.disabled && !bDisabled) bMultiline = m_bPrevMultiline;
        objMultiline.checked = ((bMultiline && !bDisabled) || bHidden || bRichArea) && !bPassword;
        objMultiline.disabled = bDisabled;

        // ReadOnly
        bDisabled = (bPassword || bHidden || bRichArea);
        if (!objReadOnly.disabled && bDisabled) m_bPrevReadOnly = bReadOnly;
        if (objReadOnly.disabled && !bDisabled) bReadOnly = m_bPrevReadOnly;
        objReadOnly.checked = ((bReadOnly && !bDisabled) || bHidden) && !bPassword;
        if (bRichArea) objReadOnly.checked = false;
        objReadOnly.disabled = bDisabled;

        // Hidden
        bDisabled = (bPassword || bRichArea);
        if (!objHidden.disabled && bDisabled) m_bPrevHidden = bHidden;
        if (objHidden.disabled && !bDisabled) bHidden = m_bPrevHidden;
        objHidden.checked = ((bHidden && !bDisabled) && !bPassword);
        objHidden.disabled = bDisabled;

        // Password
        bDisabled = (bRichArea);
        if (!objPassword.disabled && bDisabled) m_bPrevPassword = bPassword;
        if (objPassword.disabled && !bDisabled) bPassword = m_bPrevPassword;
        objPassword.checked = (bPassword && !bDisabled);
        objPassword.disabled = bDisabled;

        m_bChangingState = false;
        var fieldoptions =
        {
            optRichArea: objRichArea.checked,
            optMultiline: objMultiline.checked,
            optReadOnly: objReadOnly.checked,
            optHidden: objHidden.checked,
            optPassword: objPassword.checked
        };
        reloadValidationTab(fieldoptions);
        reloadFieldStyleTab(fieldoptions);
    }

    function getCurrentOptions()
    {
        var fieldoptions =
        {
            optRichArea: document.getElementById("chkRichArea").checked,
            optMultiline: document.getElementById("chkMultiline").checked,
            optReadOnly: document.getElementById("chkReadOnly").checked,
            optHidden: document.getElementById("chkHidden").checked,
            optPassword: document.getElementById("chkPassword").checked
        };
        return fieldoptions;
    }

    function reloadValidationTab(opt)
    {
        var reloadxml = "TextField";
        var validationName = "plaintext";
        if (null == opt)
        {
            opt = getCurrentOptions();
        }
        if (opt.optRichArea)
        {
            reloadxml = "RichAreaField";
            validationName = "richarea";
        }
        else if (opt.optHidden || opt.optReadOnly)
        {
            reloadxml = "ReadOnlyField";
            validationName = "datatype";
        }
        else if (opt.Multiline)
        {
            reloadxml = "TextAreaField";
            validationName = "textarea";
        }
        else if (opt.optPassword)
        {
            reloadxml = "PasswordField";
            validationName = "password";
        }
        ekFieldValidationControl.reloadV8nBox(reloadxml, validationName);
    }

    function reloadFieldStyleTab(opt)
    {
        var fieldType = "TextField"
        if (opt.optRichArea)
        {
            fieldType = "RichAreaField";
        }
        else if (opt.optHidden)
        {
            fieldType = "HiddenField";
        }
        else if (opt.optReadOnly)
        {
            fieldType = "ReadOnlyField";
        }
        else if (opt.optMultiline)
        {
            fieldType = "TextAreaField";
        }
        else if (opt.optPassword)
        {
            fieldType = "PasswordField";
        }
        ekFieldStyleControl.updateControl(fieldType);
    }

    function validateDialog()
    {
        var bContinue = true;
        var ret = [];
        $ektron(document).trigger("onValidateDialog", [ret]);
        if (ret && ret.length > 0)
        {
            if ("ucFieldValidation" == ret[0].name)
            {
                var currentTab = RadTabStrip1.SelectedTab;
                if (currentTab.Value != "Validation")
                {
                    var tab = RadTabStrip1.FindTabById(RadTabStrip1.ClientID + "_Validation");
                    if (tab)
                    {
                        tab.SelectParents();
                    }
                }
                bContinue = m_objFormField.promptOnValidateAction(ret[0]);
            }
        }
        return bContinue;
    }

    function validateDefaultValue(oFieldElem, oDefaultValueElem)
    {
        var bContinue = true;
        var errObj = null;
        var oInvalidElem = null;
        if (oFieldElem.ownerDocument == oDefaultValueElem.ownerDocument)
        {
            oInvalidElem = Ektron.SmartForm.prevalidateElement(oFieldElem);
        }
        else
        {
            // bring into this window so smartform.js is available to prevalidate the element
            var fieldHtml = Ektron.Xml.serializeXhtml(oFieldElem);
            var surrogateValidationElement = $ektron(fieldHtml).get(0);
            oInvalidElem = Ektron.SmartForm.prevalidateElement(surrogateValidationElement);
        }
        if (oInvalidElem != null)
        {
            errObj =
            {
                name: "DefaultValue",
                message: ResourceText.sInvalidDefVal,
                srcElement: oDefaultValueElem
            };

            var currentTab = RadTabStrip1.SelectedTab;
            if (currentTab.Value != "General")
            {
                var tab = RadTabStrip1.FindTabById(RadTabStrip1.ClientID + "_General");
                if (tab)
                {
                    tab.SelectParents();
                }
            }
            bContinue = m_objFormField.promptOnValidateAction(errObj);
        }
        return bContinue;
    }

    window.ClientTabSelectedHandler = function ClientTabSelectedHandler(sender, eventArgs)
    {
        var tab = eventArgs.Tab;
        var tabSelected = tab.Value.toLowerCase();
        switch (tabSelected)
        {
            case "advanced":
                ekFieldAdvancedControl.enableAttributeType(!document.getElementById("chkRichArea").checked);
                ekFieldAdvancedControl.updateControl(ekFieldNameControl, ((typeof ekFieldAllowControl != "undefined") ? ekFieldAllowControl.isRepeatable() : false));
                break;
            case "validation":
                if (typeof ekFieldAdvancedControl != "undefined") ekFieldNameControl.updateName(ekFieldAdvancedControl);
                var opt = getCurrentOptions();
                reloadValidationTab(opt);
                break;
            case "general":
                if (typeof ekFieldAdvancedControl != "undefined")
                {
                    ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
                    if (typeof ekFieldAllowControl != "undefined")
                    {
                        ekFieldAdvancedControl.updateFieldAllowControl(ekFieldAllowControl);
                        var objHidden = document.getElementById("chkHidden");
                        if (objHidden.checked)
                        {
                            ekFieldAllowControl.disable();
                        }
                    }
                    if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.updateFieldUseControl(ekFieldUseControl);
                }
                break;
            default:
                if (typeof ekFieldAdvancedControl != "undefined") ekFieldNameControl.updateName(ekFieldAdvancedControl);
                break;
        }
    }
})();  // namespaced
