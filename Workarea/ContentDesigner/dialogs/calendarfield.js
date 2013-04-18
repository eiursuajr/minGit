(function ()
{
    var m_objFormField = null;
    var m_oFieldElem = null;
    var m_strDatePickerSelector = ".DatePickerContainer input.DatePicker_input";

    window.initField = function initField()
    {
        m_objFormField = new EkFormFields();
        m_oFieldElem = null;

        var oContentElement = null;
        var oFieldElem = null;
        var bIsRootLoc = false;
        var sDefaultId = "";
        var sDefaultPrefix = "";
        var sContentTree = "";
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
        ekFieldValidationControl.setDefaultVal(""); // "" = date, but not required

        var date = null;
        if (m_objFormField.isDDFieldElement(oFieldElem) && "SPAN" == oFieldElem.tagName && $ektron(oFieldElem).hasClass("ektdesignns_calendar"))
        {
            ekFieldNameControl.read(oFieldElem);
            if (typeof ekFieldUseControl != "undefined") ekFieldUseControl.read(oFieldElem);
            if (typeof ekFieldAllowControl != "undefined") ekFieldAllowControl.read(oFieldElem);
            if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.read(oFieldElem);
            ekFieldDataStyleControl.read(oFieldElem);
            ekFieldValidationControl.read(oFieldElem);
            date = Ektron.Xml.parseDate(oFieldElem.getAttribute("datavalue")); //value attribute in INPUT tag. 
            if (date)
            {
                $ektron(m_strDatePickerSelector).datepicker('setDate', date);
            }

            var chkCurrent = document.getElementById("chkCurrentDate");
            if (chkCurrent)
            {
                document.getElementById("chkCurrentDate").checked = "";
                if (oFieldElem.getAttribute("ektdesignns_default") != null && oFieldElem.getAttribute("ektdesignns_default") != "")
                {
                    document.getElementById("chkCurrentDate").checked = "checked";
                }
                var bCurrentDate = ("checked" == document.getElementById("chkCurrentDate").checked) ? true : false;
                this.onDefaultCurrentDateChecked(bCurrentDate);
            }

            m_oFieldElem = oFieldElem;
            ekFieldNameControl.initSetting(true);
        }
        else
        {
            ekFieldNameControl.setDefaultFieldNames(sDefaultPrefix, sDefaultId);
            if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.setName(sDefaultPrefix + sDefaultId);
            ekFieldNameControl.initSetting(false);
        }

        var defaultDate = (null == date ? new Date() : date).toLocaleDateString();
        if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.setDefaultVal(defaultDate);
        if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
    }

    window.insertField = function insertField()
    {
        if (false == validateDialog())
        {
            return false;
        }

        var oFieldElem = m_oFieldElem;
        var objInput = null;
        if (null == oFieldElem)
        {
            oFieldElem = document.createElement("span");
            oFieldElem.className = "ektdesignns_calendar";
            oFieldElem.setAttribute("ektdesignns_basetype", "calendar");
            oFieldElem.setAttribute("contenteditable", "false");
        }

        objInput = $ektron(oFieldElem).children("input").get(0);
        if (!objInput)
        {
            objInput = oFieldElem.ownerDocument.createElement("input");
            objInput.type = "text";
            objInput.setAttribute("readonly", "readonly");
            objInput.size = 30;
            objInput.setAttribute("unselectable", "on");
            oFieldElem.appendChild(objInput);
            var objTextNode = oFieldElem.ownerDocument.createTextNode(" ");
            oFieldElem.appendChild(objTextNode);

            var strImgUrl = g_skinPath + "btncalendar.gif";

            var objFieldBtn = oFieldElem.ownerDocument.createElement("img");
            objFieldBtn.setAttribute("unselectable", "on");
            objFieldBtn.className = "design_fieldbutton";
            objFieldBtn.src = strImgUrl;
            objFieldBtn.alt = "Select date";
            objFieldBtn.width = "16";
            objFieldBtn.height = "16";
            oFieldElem.appendChild(objFieldBtn);
        }

        if (typeof ekFieldAdvancedControl != "undefined")
        {
            ekFieldAdvancedControl.updateControl(ekFieldNameControl, ekFieldAllowControl.isRepeatable());
            ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
            ekFieldAdvancedControl.updateFieldAllowControl(ekFieldAllowControl);
            ekFieldAdvancedControl.updateFieldUseControl(ekFieldUseControl);
        }
        ekFieldNameControl.update(oFieldElem);
        if (typeof ekFieldUseControl != "undefined") ekFieldUseControl.update(oFieldElem);
        if (typeof ekFieldAllowControl != "undefined") ekFieldAllowControl.update(oFieldElem);
        if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.update(oFieldElem);
        ekFieldDataStyleControl.update(oFieldElem);
        ekFieldValidationControl.update(oFieldElem);
        var date = $ektron(m_strDatePickerSelector).datepicker('getDate');
        var defVal = Ektron.Xml.serializeDate(date);
        oFieldElem.setAttribute("datavalue", defVal);
        objInput.value = ektLocalizeDate(defVal); //IE
        objInput.setAttribute("value", objInput.value); //FF
        oFieldElem.setAttribute("ektdesignns_default", "");
        if (document.getElementById("chkCurrentDate") && document.getElementById("chkCurrentDate").checked)
        {
            oFieldElem.setAttribute("ektdesignns_default", "$currentDate");
            oFieldElem.setAttribute("datavalue", "");
            objInput.setAttribute("value", "");
            objInput.value = "";
        }

        CloseDlg(oFieldElem);
    }

    window.onDefaultCurrentDateChecked = function (bChecked)
    {
        var $datePickerInput = $ektron("input.DatePicker_input");
        if (bChecked)
        {
            $datePickerInput.val("").attr("disabled", "disabled");
            $datePickerInput.siblings("img").attr("disabled", "disabled");
        }
        else
        {
            $datePickerInput.attr("disabled", "");
            $datePickerInput.siblings("img").attr("disabled", "");
        }
    }

    function ektLocalizeDate(date)
    {
        var oTempDate = Ektron.Xml.parseDate(date);
        if (oTempDate != null)
        {
            return oTempDate.toLocaleDateString();
        }
        else
        {
            return date;
        }
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
                bContinue = EkFormFields_PromptOnValidateAction(ret[0]);
            }
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
                if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.updateControl(ekFieldNameControl, ekFieldAllowControl.isRepeatable());
                break;
            case "validation":
                if (typeof ekFieldAdvancedControl != "undefined") ekFieldNameControl.updateName(ekFieldAdvancedControl);
                ekFieldValidationControl.reloadV8nBox("CalendarField", "calendar");
                break;
            case "general":
                if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
                if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.updateFieldAllowControl(ekFieldAllowControl);
                if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.updateFieldUseControl(ekFieldUseControl);
                break;
            default:
                if (typeof ekFieldAdvancedControl != "undefined") ekFieldNameControl.updateName(ekFieldAdvancedControl);
                break;
        }
    }
})();  // namespaced
