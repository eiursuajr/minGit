(function()
{
    var m_objFormField = null;
    var m_oFieldElem = null;
    var m_OrigNodeName = "";
    var m_srcPath = "";
    var m_skinPath = "";
    var m_langType = theEditor ? (theEditor.ekParameters.userLanguage+"") : "";
    var m_ekXml = null;
    var m_smartForm = null;
    var m_bCustom = true;
    var m_$NewCustomTd = null;
    var m_$NewROTd = null;
    
    var m_isFieldValidationControlLoaded = false;
    var m_surrogateValidationElement = null;

	window.initField = function initField()
	{
		if (null == m_ekXml)
		{
			m_srcPath = g_srcPath;
			m_skinPath = g_skinPath;
			m_ekXml = new Ektron.Xml({ srcPath:m_srcPath });
		}
		
		var settings = 
		{	srcPath: m_srcPath
		,	skinPath: m_skinPath
		,	langType: m_langType
		,	enableExpandCollapse: false
		};
		m_smartForm = new Ektron.SmartForm("design_content", settings);
	    m_smartForm.onload();
	    m_objFormField = new EkFormFields();
        m_oFieldElem = null;
        var origDataList = "";
        
		var oContentElement = null;
	    var oFieldElem = null;
	    var bIsRootLoc = false;
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
	    }
	    if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.setRootTagVisible(bIsRootLoc);
	    ekFieldValidationControl.init(oContentElement);

        var joFieldElem = $ektron(oFieldElem);
	    if (m_objFormField.isDDFieldElement(oFieldElem) && ("SELECT" == oFieldElem.tagName || 
			("DIV" == oFieldElem.tagName && (joFieldElem.hasClass("ektdesignns_choices") || joFieldElem.hasClass("ektdesignns_checklist")))))
	    {
	        ekFieldNameControl.read(oFieldElem);
	        if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.read(oFieldElem);
	        ekFieldDataStyleControl.read(oFieldElem);
			ekFieldValidationControl.read(oFieldElem, "ektdesignns_datatype");
	        var objOL = oFieldElem.firstChild;
	        var strMinOccurs = "";
	        while (objOL && objOL.nodeType != 1) // Node.ELEMENT_NODE
	        {
	            objOL = objOL.nextSibling; // browser, e.g. IE 7, may return the next text node as firstChild
	        }
            if (objOL && "OL" == objOL.tagName) //OL tag in Choices list or check list
	        {   
	            strMinOccurs = $ektron.toStr(objOL.getAttribute("ektdesignns_minoccurs"));
	            if ("unbounded" === objOL.getAttribute("ektdesignns_maxoccurs"))
	            {   
	                document.getElementById("optAllowMulti").checked = true;
	            }
	            else
	            {
			        document.getElementById("optAllowOne").checked = true;
	            }
	        }
	        else //SELECT
	        {
			    strMinOccurs = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_minoccurs"));
			    if ("unbounded" === oFieldElem.getAttribute("ektdesignns_maxoccurs"))
	            {
	                document.getElementById("optAllowMulti").checked = true;
	            }
	            else
	            {
			        document.getElementById("optAllowOne").checked = true;
	            }
	        }
	        var strValidation = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_validation"));
	        if (strValidation && "-req" == strValidation.substr(strValidation.length - 4))
            {
                document.getElementById("chkSelRequired").checked = true;
            }
            else if (strMinOccurs && strMinOccurs.length > 0 && strMinOccurs != "0")
            {
                document.getElementById("chkSelRequired").checked = true;
            }
            var objOnBlur = oFieldElem.getAttribute("onblur"); 
            if (objOnBlur) // onblur might be null
            {
                if (objOnBlur.toString().indexOf("design_validate_select(") > -1)
                {
                    document.getElementById("chkItem1Invalid").checked = true;
                }
            }
            var datalist = oFieldElem.getAttribute("ektdesignns_datalist");     
            if (datalist != "" && datalist != null)
            {
                var input = document.getElementById(cboList.InputID);
                for (var i = 0; i < cboList.Items.length; i++)
                {
                    if (datalist == cboList.Items[i].Value)
                    {
                        input.value = cboList.Items[i].Text;  
                    }
                }
                //document.getElementById("ItemListDiv").disabled = true;
            }
                       
            var tagName = oFieldElem.tagName;
            if ("SELECT" == tagName)
            {
                m_OrigNodeName = "select";
                if (oFieldElem.multiple || oFieldElem.size > 1)
                {
                    document.getElementById("displayListBox").checked = true;
                }
                else
                {
					document.getElementById("displayDropList").checked = true;
                }
            }
            else
            {
                m_OrigNodeName = "div";
				var oFirst = $ektron(oFieldElem).children(":first");
                if (oFirst.length > 0)
                {
                    if (oFirst.hasClass("design_list_vertical"))
                    {
                        document.getElementById("displayVerticalList").checked = true;
                    }
                    else
                    {
                        document.getElementById("displayHorizontalList").checked = true;
                    }
                }
            } 
            // create tabular data table
            origDataList = Ektron.Xml.serializeXhtml(oFieldElem);
            
            var args = [
            { name: "baseURL", value: m_srcPath },
            { name: "LangType", value: m_langType }
            ];

            var sItemListTable = "";
            if (datalist != "" && datalist != null)
            {
                // loading pre-defined datalist
                sItemListTable = m_ekXml.xslTransform(origDataList, "[srcPath]ChoicesFieldDataItemsRO.xslt", args);
                m_bCustom = false;
            }
            else
            {
                sItemListTable = m_ekXml.xslTransform(origDataList, "[srcPath]ChoicesFieldDataItems.xslt", args);
                m_bCustom = true;
            }
            var $ItemList = $ektron("#ItemListDiv", document);
            $ItemList.html(sItemListTable);
            defineDatalistFirstRow($ItemList, true);
            m_smartForm.onload();  
		    m_oFieldElem = oFieldElem;
		    ekFieldNameControl.initSetting(true);
		}
		else
		{
		    defineDatalistFirstRow($ektron("#ItemListDiv", document), false);
		    ekFieldNameControl.setDefaultFieldNames(sDefaultPrefix, sDefaultId);
	        if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.setName(sDefaultPrefix + sDefaultId);
	        ekFieldNameControl.initSetting(false);
		}
		if (typeof ekFieldAdvancedControl != "undefined") 
		{
			ekFieldAdvancedControl.setDefaultVal("Value");
			ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
	    }
	    updateOptions();
	    AssignEventHandler();
        var tabValidation = RadTabStrip1.FindTabById(RadTabStrip1.ClientID + "_Validation");
	    if (m_bCustom)
	    {
			if (tabValidation) tabValidation.Enable();
	    }
	    else
	    {
            if (tabValidation) tabValidation.Disable();
	    }
	};

	window.insertField = function insertField()
	{
	    if (false == Ektron.SmartForm.validateForm())
	    {
			var currentTab = RadTabStrip1.SelectedTab;
			if (currentTab.Value != "General")
			{
				var tab = RadTabStrip1.FindTabById(RadTabStrip1.ClientID + "_General");
				if (tab)
				{
					tab.SelectParents();
				}
			}  
	        alert($ektron("#dsg_validate").attr("title"));
	        return false;
	    }
	    var oFieldElem = m_oFieldElem; 
	    var $DataListContainer = $ektron("#select", document);//<table id="select"...
	    //http://api.jquery.com/hidden-selector/
        //$(elem).css('visibility','hidden').is(':hidden') == false
	    var $FirstRow = $ektron("tr.design_list_first_row", $DataListContainer); 
	    if (1 == $FirstRow.length && "none" == $FirstRow.css("display"))
	    {
	        $FirstRow.remove(); 
	    }
        $FirstRow = $ektron("tr.design_list_first_rowRO", $DataListContainer); 
	    if (1 == $FirstRow.length && "none" == $FirstRow.css("display"))
	    {
	        $FirstRow.remove(); 
	    }

	    var sItemListTable = Ektron.Xml.serializeXhtml($DataListContainer.get(0)); 
	    
	    // need to remove the disabled attributes from the pre-defined datalist, otherwise PresentationToData.xslt will
	    // ignore the text/value pairs.
	    sItemListTable = sItemListTable.replace(/\sdisabled=\"disabled\"/gim, "");
	    var sItemListXML = m_ekXml.xslTransform(sItemListTable, "[srcpath]PresentationToData.xslt");
	    sItemListXML = sItemListXML.replace(/\sselected=\"false\"/gim, "");
	    sItemListXML = sItemListXML.replace(/\sdisabled=\"false\"/gim, "");

        if (document.getElementById("displayVerticalList").checked || document.getElementById("displayHorizontalList").checked)
        {
            // div
            var objOL = null;
            var className = "";
            var maxoccurs = "";
            var nodeType = "";
            if (document.getElementById("optAllowMulti").checked)
            {
                className = "ektdesignns_checklist";
                maxoccurs = "unbounded";
                nodeType = "checkbox"
            }
            else
            {
                className = "ektdesignns_choices";
                maxoccurs = "1";
                nodeType = "radio";
            }        
            if (null == oFieldElem || "select" == m_OrigNodeName)
            {
                oFieldElem = null;
                oFieldElem = document.createElement("div");
                oFieldElem.title = ekFieldNameControl.getToolTip();
                oFieldElem.setAttribute("contenteditable", "false");
            }

            oFieldElem.className = className;
            var designList = "design_list_vertical";
            if (document.getElementById("displayHorizontalList").checked)
            {
                designList = "design_list_horizontal";
            }
          
            // transformToDocument for replacing Data List
            var args = [
            { name: "elemName", value: ekFieldNameControl.getName() },
            { name: "nodeType", value: nodeType }
            ];
            oFieldElem.innerHTML = m_ekXml.xslTransform(sItemListXML, "[srcpath]SelectTagToChoicesField.xslt", args)
						+ "&#160;"; // &#160; is hack for moveToElementText
            objOL = oFieldElem.firstChild;
            objOL.className = designList;
            objOL.setAttribute("ektdesignns_maxoccurs", maxoccurs);
            if (document.getElementById("chkSelRequired").checked)
            {
				var sValidation = "design_validate_choice(1, -1, this, '" + ResourceText.sOptionsReqd + "')";
                objOL.setAttribute("ektdesignns_minoccurs", 1);
                objOL.setAttribute("ektdesignns_validation", "choice-req");
                objOL.setAttribute("onblur", sValidation);
                objOL.setAttribute("onkeypress", sValidation);
                objOL.setAttribute("onclick", sValidation);
            }
            objOL.title = ekFieldNameControl.getToolTip();
        }
        else
        {
            // select
            // replace Data List
            // oFieldElem.innerHTML NOTE: setting innerHTML for select element is bug-ridden in all browsers
            //
            // oFieldElem is not m_oFieldElem in this case; it will always be a new element, 
            // so it doesn't matter whether we use document or oFieldElem.ownerDocument
            //
            // This is a placeholder div to contain the select element.
            var oHolder = document.createElement("div"); 
            //sItemListXML is produced by our xslt. We know how it looks.
            var sMultiple = "";
            if (document.getElementById("optAllowMulti").checked)
	        {
				sMultiple = " multiple=\"multiple\"";
	        }
            sItemListXML = sItemListXML.replace(/\<select\>/, "<select name=\"" + ekFieldNameControl.getName() + "\"" + sMultiple + ">"); 
            oHolder.innerHTML = sItemListXML;
            oFieldElem = oHolder.firstChild;
            oHolder = null;

            oFieldElem.removeAttribute("onblur");
            oFieldElem.removeAttribute("ektdesignns_validation");
            if (document.getElementById("chkItem1Invalid").checked)
            {
                oFieldElem.setAttribute("onblur", "design_validate_select(1, this, '" + ResourceText.sFirstNotValid + "')");
                oFieldElem.setAttribute("ektdesignns_validation", "select-req");
            }
            if (document.getElementById("optAllowMulti").checked)
	        {   
	            oFieldElem.setAttribute("ektdesignns_maxoccurs", "unbounded");
                oFieldElem.multiple = true;
                oFieldElem.setAttribute("multiple", "multiple");
	        }
	        else
	        {
			    oFieldElem.setAttribute("ektdesignns_maxoccurs", "1");
				oFieldElem.multiple = false;
				oFieldElem.removeAttribute("multiple");
	        }
            if (document.getElementById("displayListBox").checked)
            {
				var size = (m_oFieldElem != null ? m_oFieldElem.size : oFieldElem.size);
				if ("undefined" == typeof size || 0 == size || size > oFieldElem.length)
				{
					size = oFieldElem.length;
					if (size > 12) // arbitrary default max size
					{
						size = 12
					}
				}
				if (size < 2) // must be at least 2
				{
					size = 2;
				}
				oFieldElem.size = size;
				oFieldElem.setAttribute("size", size);
            }
            else
            {
				oFieldElem.size = 1;
				oFieldElem.setAttribute("size", "1");
            }
        }
            
        if (typeof ekFieldAdvancedControl != "undefined") 
        {
			ekFieldAdvancedControl.updateControl(ekFieldNameControl);
			ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
        }
        else
        {
			oFieldElem.setAttribute("ektdesignns_nodetype", "element");
        }
        ekFieldNameControl.update(oFieldElem);
        if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.update(oFieldElem);
        ekFieldDataStyleControl.update(oFieldElem);

        var input = document.getElementById(cboList.InputID);
        var datalistText = input.value;
        var datalist = "";
        for (var i = 0; i < cboList.Items.length; i++)
        {
            if (datalistText == cboList.Items[i].Text)
            {
                datalist = cboList.Items[i].Value;  
            }
        }
        oFieldElem.removeAttribute("ektdesignns_datalist");
        oFieldElem.removeAttribute("ektdesignns_datasrc");
        oFieldElem.removeAttribute("ektdesignns_dataselect");
        oFieldElem.removeAttribute("ektdesignns_captionxpath");
        oFieldElem.removeAttribute("ektdesignns_valuexpath");
        oFieldElem.removeAttribute("ektdesignns_datanamespaces");
        oFieldElem.removeAttribute("ektdesignns_datatype");
        oFieldElem.removeAttribute("ektdesignns_basetype");
        oFieldElem.removeAttribute("ektdesignns_schema");
        if (datalist && datalist.length > 0)
        {
            oFieldElem.setAttribute("ektdesignns_datalist", datalist);
			try
            {
				// <datalist name="id" localeRef="idref" src="url" cache="true|false" 
				//            select="xpath" captionxpath="xpath" valuexpath="xpath" namespaces="uri list">
				//     <schema datatype="string">content</schema>
				//     <item value="string" default="true|false">string</item>
				// <datalist>
				var strDatalistUrl = "[srcpath]DataListSpec.xml"; // would be nice to not hardcode
                var xmlDoc = m_ekXml.loadXml(strDatalistUrl);
                var dl = xmlDoc.selectSingleNode("//datalist[@name='" + datalist + "']");
                if (dl)
                {
					var bCache = $ektron.toBool(dl.getAttribute("cache"), true); // default 'true' defined in DataListSpec.xsd
					if (!bCache)
					{
						var strSource = $ektron.toStr(dl.getAttribute("src"));
						if (strSource)
						{
							// Make relative to host
							strSource = m_ekXml.resolveSrcPath(strSource);
							var strHost = location.protocol + "//" + location.host;
							var lenHost = strHost.length;
							if (strHost.substr(0,lenHost).toLowerCase() == strSource.substr(0,lenHost).toLowerCase())
							{
								strSource = strSource.substr(lenHost); // remove strHost from the beginning
							}
							
							var strSelect = $ektron.toStr(dl.getAttribute("select"));
							if ("" == strSelect || "*" == strSelect)
							{
								strSelect = "/*/*";
							}
							else if (strSelect.charAt(0) != "/")
							{
								strSelect = "/" + strSelect;
							}
							var strCaptionXPath = dl.getAttribute("captionxpath");
							var strValueXPath = dl.getAttribute("valuexpath");
							var strNamespaces = dl.getAttribute("namespaces");
							oFieldElem.setAttribute("ektdesignns_datasrc", strSource);
							oFieldElem.setAttribute("ektdesignns_dataselect", strSelect);
							if (strCaptionXPath) oFieldElem.setAttribute("ektdesignns_captionxpath", strCaptionXPath);
							if (strValueXPath) oFieldElem.setAttribute("ektdesignns_valuexpath", strValueXPath);
							if (strNamespaces) oFieldElem.setAttribute("ektdesignns_datanamespaces", strNamespaces);
						}
					}
					var oSchema = dl.selectSingleNode("schema");
					if (oSchema)
					{
						var strDataType = oSchema.getAttribute("datatype");
						var strSchema = $ektron.trim(Ektron.Xml.serializeXml(oSchema));
						// Remove the outer <schema> tags
						strSchema = strSchema.replace(/^<schema[^>]*>/,"").replace(/<\/schema>$/,"");
						if (strDataType) oFieldElem.setAttribute("ektdesignns_datatype", strDataType);
						if (strSchema) oFieldElem.setAttribute("ektdesignns_schema", strSchema);
						oSchema = null;
					}
					var strDataType = dl.getAttribute("datatype"); // takes precedence over schema/@datatype
					if (strDataType) oFieldElem.setAttribute("ektdesignns_datatype", "validation:" + strDataType);
					var strBaseType = dl.getAttribute("treeImg");
					if (strBaseType) oFieldElem.setAttribute("ektdesignns_basetype", strBaseType);
					dl = null;
				} // dl
				xmlDoc = null;
            }
            catch (ex)
            {
				// ignore
				Ektron.OnException.diagException(ex, arguments);
            }
        } // datalist
        else
        {
			// We only need a few attributes because validation for choice field is not just validation of a single value.
			var oTempElem = document.createElement("input");
			ekFieldValidationControl.update(oTempElem); 
			var strDataType = oTempElem.getAttribute("ektdesignns_datatype");
			if (strDataType) 
			{
				oFieldElem.setAttribute("ektdesignns_datatype", strDataType);
			}
			else
			{
				strDataType = oTempElem.getAttribute("ektdesignns_validation");
				if (strDataType) oFieldElem.setAttribute("ektdesignns_datatype", "validation:" + strDataType);
			}
			var strBaseType = oTempElem.getAttribute("ektdesignns_basetype");
			if (strBaseType) oFieldElem.setAttribute("ektdesignns_basetype", strBaseType);
			var strInvalidMsg = oTempElem.getAttribute("ektdesignns_invalidmsg");
			if (strInvalidMsg) oFieldElem.setAttribute("ektdesignns_invalidmsg", strInvalidMsg);
			oTempElem = null;
        }
		CloseDlg(oFieldElem);	
	};
	
	window.validateChoicesFieldItemValue = function validateChoicesFieldItemValue(oElem)
	{
		var value = Ektron.SmartForm.getValue(oElem);
		if ("undefined" == typeof value) return; // no data to test
		var $oElem = $ektron(oElem);   //$oElem.attr([id^="value"]) is not valid
        if ("value" == $oElem.attr("id").substr(0, 5) && ($oElem.closest("tr").hasClass("design_list_first_row") || $oElem.closest("tr").hasClass("design_list_first_rowRO")))
        {
            oElem.title = "";
		    $oElem.removeClass("design_validation_failed");
		    return; //if true == chkItem1Invalid, allow the first value box be empty.
        }

		if (null == m_surrogateValidationElement)
		{
			// We only need a few attributes because validation for choice field is not just validation of a single value.
			m_surrogateValidationElement = document.createElement("input");
			ekFieldValidationControl.update(m_surrogateValidationElement); 
		}
		var strInvalidMsg = m_surrogateValidationElement.getAttribute("ektdesignns_invalidmsg");
		if (!strInvalidMsg)
		{
			strInvalidMsg = ResourceText.sCannotBeBlank;
		}
		oElem.title = ""; // the tool tip would accumulate error messages as they changed, so clear it out
		var oInvalidElem = oElem;
		if (value != "")
		{
			Ektron.SmartForm.setValue(m_surrogateValidationElement, value);
			oInvalidElem = Ektron.SmartForm.prevalidateElement(m_surrogateValidationElement);
		}
		var result = (null == oInvalidElem);

		Ektron.SmartForm.validate_complete(oElem, result, strInvalidMsg);

		return result;
	};
        
    window.item1InvalidChanged = function(bItem1Invalid)
    {
        var $FirstRow = $ektron("div#ItemListDiv tr.design_list_first_row", document);
        var $FirstRORow = $ektron("div#ItemListDiv tr.design_list_first_rowRO", document);
        if (1 == $FirstRow.length || 1 == $FirstRORow.length)
        {
            if (bItem1Invalid)
            {
                //need to show the first row
                if (m_bCustom)
                {
                    //$FirstRow.show("slow"); //jquery 1.3.2 adds display:block and jquery 1.4.1 adds display:table-row
                    $FirstRow.css("display", "");
                }
                else
                {
                    //$FirstRORow.show("slow"); //jquery 1.3.2 adds display:block and jquery 1.4.1 adds display:table-row
                    $FirstRORow.css("display", "");
                }
            }
            else
            {
                //need to hide the first row
                $FirstRow.hide();
                $FirstRORow.hide();
            }
        }
    };
    
    window.updateOptions = function updateOptions()
    {
        if (document.getElementById("displayDropList").checked)
        {
            document.getElementById("chkItem1Invalid").disabled = false;
            document.getElementById("optAllowOne").checked = true;
            document.getElementById("optAllowOne").disabled = true;
            document.getElementById("optAllowMulti").disabled = true;
            document.getElementById("chkSelRequired").disabled = true;
        }
        else
        {
            document.getElementById("chkItem1Invalid").checked = false;
            document.getElementById("chkItem1Invalid").disabled = true;
            document.getElementById("optAllowOne").disabled = false;
            document.getElementById("optAllowMulti").disabled = false;
            document.getElementById("chkSelRequired").disabled = false;
        }
        item1InvalidChanged(document.getElementById("chkItem1Invalid").checked);
    };
    
    window.UpdateItemDisabled = function UpdateItemDisabled(obj)
    {
        var objCurrent = null;
        if ("INPUT" == obj.tagName)
        {
            objCurrent = obj;
        }
        if (objCurrent)
        {
            var sIdSuffix = objCurrent.id.replace(/disabled/, "");
            var objChkItemSelected = document.getElementById("selected" + sIdSuffix);
            if (objCurrent.checked)
			{
				objChkItemSelected.checked = false;
				objChkItemSelected.disabled = true;
			}
			else
			{
				objChkItemSelected.disabled = false;
			}
		}
    };
    
    window.UpdateItemSelected = function UpdateItemSelected(obj)
    {
        var objCurrent = null;
        if ("INPUT" == obj.tagName)
        {
            objCurrent = obj;
        }
        if (objCurrent)
        {
            // check if it only allows one default selected value.
            if (document.getElementById("optAllowOne").checked)
            {
                $ektron("#ItemListDiv :checkbox:even").each( function() {
                    if (this != objCurrent)
                    {
                        $ektron(this).removeAttr("checked");
                    }
                });
            }
            
            var sIdSuffix = objCurrent.id.replace(/selected/, "");
            var objChkItemDisabled = document.getElementById("disabled" + sIdSuffix);
            if (true == objChkItemDisabled.checked)
            {
                $ektron(objCurrent).removeAttr("checked");
			}
		}
    };
    
    function defineDatalistFirstRow($DataListContainer, bReload)
    {
        if (0 == $DataListContainer.length) return;
        var $Tbody = $ektron("tbody", $DataListContainer); 
        var $FirstRow = $Tbody.children(":first-child");
        if (1 == $FirstRow.length)
        {
            if (bReload)
            {    
                
                if (1 == $Tbody.children(":last-child").children().length) //custom datalist
                {
                    //document.getElementById("chkItem1Invalid").checked = false;
                }
                else
                {
                    var $FirstValueBox = $ektron("input[id^='value']", $FirstRow);
                    if ("" == $FirstValueBox.val())
                    {            
                        $FirstRow.addClass("design_list_first_rowRO");
                        var $FirstTextBox = $ektron("input[id^='text']", $FirstRow);
                        if ("" == $FirstTextBox.val())
                        {
                            $FirstTextBox.val(ResourceText.sFirstRowText);
                        }
                    }
                }
            }

            var $NewFirstRow;
            var dataList = "<select><option>" + ResourceText.sFirstRowText + "</option></select>";
            var args = [
            { name: "baseURL", value: m_srcPath },
            { name: "LangType", value: m_langType }
            ];
            var tblTemp = "";
            if (null == m_$NewCustomTd)
            {
                tblTemp = m_ekXml.xslTransform(dataList, "[srcPath]ChoicesFieldDataItems.xslt", args);
                var $NewCustomTd = $ektron("tbody", tblTemp).children(":first-child");
                $NewCustomTd.children("td:first-child").html(""); //remove image link
                $NewCustomTd.children("td:last-child").children("input:checkbox").attr("disabled", "disabled"); //disabled the disable checkbox
                $NewCustomTd.addClass("design_list_first_row");
                m_$NewCustomTd = $NewCustomTd;
            }
            if (null == m_$NewROTd)
            {
                tblTemp = m_ekXml.xslTransform(dataList, "[srcPath]ChoicesFieldDataItemsRO.xslt", args);
                m_$NewROTd = $ektron("tbody", tblTemp).children(":first-child").addClass("design_list_first_rowRO");
            }
            if (m_bCustom)
            {
                if (bReload && true == document.getElementById("chkItem1Invalid").checked)
                {
                    $FirstRow.addClass("design_list_first_row");
                    $ektron("input[ektdesignns_name='selected']", $FirstRow).removeAttr("checked");
                    $ektron("input[ektdesignns_name='disabled']", $FirstRow).attr("disabled", "disabled");
                }
                else
                {
                    if (0 == $ektron("tr.design_list_first_row", $Tbody).length)
                    {
                        $FirstRow.before(m_$NewCustomTd);
                    }
                }
            }
            else 
            {
                if (0 == $ektron("tr.design_list_first_rowRO", $Tbody).length)
                {
                    $FirstRow.before(m_$NewROTd);
                }
            }
        }
    }
    
    function updateValidation(obj)
    {
        $ektron("#chkSelRequired").attr("checked", obj.checked);
    }
    function AssignEventHandler()
    {
        $ektron("#ItemListDiv :checkbox:even").click(function () { 
            UpdateItemSelected(this);
        });
        $ektron("#ItemListDiv :checkbox:odd").click(function () {
            UpdateItemDisabled(this);
        });
        $ektron("#chkItem1Invalid").click(function () {
            updateValidation(this);
        });
    }
//  This is a nice-to-have visual feature.  However, I have difficult getting it to work in IE.  After switching from checkbox to radio box, 
//  IE loses the onclick events.  
//    function switchCheckboxes()
//    {  
//        var sType = (document.getElementById("optAllowOne").checked ? "radio" : "checkbox");
//        if ("radio" == sType)
//        { 
//            $ektron("#ItemListDiv :checkbox:even").each( function() {
//                var oInput = $ektron(this);
//                var sInput = oInput.parent().html();
//                sInput = sInput.replace(/ type=\"?checkbox\"?/i, " type=\"radio\"");
//                //sInput = sInput.replace(/ name=\"?[^\"]*\"?/i, "").replace(/\<input\s+/i, "<input name=\"selected\" "); //<INPUT name="selected" ...
//                oInput.parent().html(sInput); 
//                oInput.click(function () { UpdateItemSelected(this); });
//            });
//        }
//        else // "checkbox" == sType
//        {
//            $ektron("#ItemListDiv :radio").each( function() {
//                var oInput = $ektron(this);
//                var sInput = oInput.parent().html();
//                sInput = sInput.replace(/ type=\"?radio\"?/i, " type=\"checkbox\"");
//                oInput.parent().html(sInput); 
//                oInput.click(function () { UpdateItemSelected(this); });
//            });
//        }
//    } 
    window.OnClientSelectedIndexChanged = function OnClientSelectedIndexChanged(item)
    {
        var html = "";
        var args = [
        { name: "baseURL", value: m_srcPath },
        { name: "LangType", value: m_langType }
        ];

		if (null == item.Value || "" == item.Value)
        {
            m_bCustom = true;  
        }
        else
        {
            try
            {
				var xml = "<select></select>";
				var strDatalistUrl = "[srcpath]DataListSpec.xml"; // would be nice if not hardcoded
                var xmlDoc = m_ekXml.loadXml(strDatalistUrl);
                var dl = xmlDoc.selectSingleNode("//datalist[@name='" + item.Value + "']");
                if (dl)
                {
					var bDropListAppearance = false;
					var strSource = dl.getAttribute("src");
					if (strSource)
					{
						bDropListAppearance = true;
						var strSelect = $ektron.toStr(dl.getAttribute("select"));
						var strCaptionXPath = $ektron.toStr(dl.getAttribute("captionxpath"));
						var strValueXPath = $ektron.toStr(dl.getAttribute("valuexpath"));
						var strNamespaces = $ektron.toStr(dl.getAttribute("namespaces"));
						xml = Ektron.DataListManager.getDataList("", strSource, strSelect, strCaptionXPath, strValueXPath, strNamespaces);
						if (0 == xml.length) xml = "<select></select>";
						// include <item> elements too
						var objItems = dl.selectNodes("item");
						if (objItems && objItems.length > 0)
						{
							var strItems = m_ekXml.xslTransform(strDatalistUrl, "[srcpath]DatalistToSelectOptions.xslt", 
									args.concat({ name: "name", value: item.Value })); 
							strItems = strItems.replace("</select>", ""); // remove closing </select>, keep opening tag
							xml = xml.replace("<select>", strItems); // strItems has opening <select> tag, but not closing
						}
					}
					else
					{
						xml = m_ekXml.xslTransform(strDatalistUrl, "[srcpath]DatalistToSelectOptions.xslt", 
									args.concat({ name: "name", value: item.Value })); 
						if (0 == xml.length) xml = "<select></select>";	
					}
					
					//#49820
					document.getElementById("optAllowOne").checked = true;
					document.getElementById("chkItem1Invalid").checked = false;
					var objItems = dl.selectNodes("item"); 
					if (objItems && objItems.length > 0 && "" == objItems[0].getAttribute("value"))
					{
					    document.getElementById("chkItem1Invalid").checked = true;
					    bDropListAppearance = true;
					}
					var strValidation = dl.getAttribute("validation");
					document.getElementById("chkSelRequired").checked = false;
					if (strValidation && "select-req" == strValidation)
					{
					    document.getElementById("chkSelRequired").checked = true;
					}
					document.getElementById("displayVerticalList").checked = true;
					if (true == bDropListAppearance)
					{
					    document.getElementById("displayDropList").checked = true;
					}
				}
				
                html = m_ekXml.xslTransform(xml, "[srcpath]ChoicesFieldDataItemsRO.xslt", args);
                m_bCustom = false;
                //document.getElementById("ItemListDiv").disabled = true;
            }
            catch (ex)
            {
				Ektron.OnException.diagException(ex, arguments);
                m_bCustom = true;
            }
        }
        var tabValidation = RadTabStrip1.FindTabById(RadTabStrip1.ClientID + "_Validation");
        if (m_bCustom)
        {
            html = m_ekXml.xslTransform("<select></select>", "[srcpath]ChoicesFieldDataItems.xslt", args);
            document.getElementById("ItemListDiv").disabled = false; 
            if (tabValidation) tabValidation.Enable(); 
        }
        else
        {
			if (tabValidation) tabValidation.Disable();
        }

        var $ItemList = $ektron("#ItemListDiv", document);
        $ItemList.html(html);
        defineDatalistFirstRow($ItemList, true);
        updateOptions(); // this needs to be call or re-call after defineDatalistFirstRow()
        m_smartForm.onload();
    };
    
	window.ClientTabSelectedHandler = function ClientTabSelectedHandler(sender, eventArgs)
	{          
	    var tab = eventArgs.Tab;  
	    var tabSelected = tab.Value.toLowerCase();
	    switch(tabSelected)
	    {
	        case "advanced":
	            ekFieldAdvancedControl.updateControl(ekFieldNameControl); 
	            break;
	        case "validation":
	            if (typeof ekFieldAdvancedControl != "undefined") ekFieldNameControl.updateName(ekFieldAdvancedControl); 
                if (!m_isFieldValidationControlLoaded)
                {
					m_isFieldValidationControlLoaded = true;
					ekFieldValidationControl.reloadV8nBox("ReadOnlyField", "datatype");
				}
				m_surrogateValidationElement = null;
	            break;
	        case "general":
	            if (typeof ekFieldAdvancedControl != "undefined") ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
	            break;
	        default:
	            if (typeof ekFieldAdvancedControl != "undefined") ekFieldNameControl.updateName(ekFieldAdvancedControl); 
	            break;
	    }
	};
})(); // namespaced
