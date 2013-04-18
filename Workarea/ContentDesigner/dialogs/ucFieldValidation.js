function EkFieldValidationControl(clientID, references)
{
	var m_val_ekXPathExpr = new Ektron.XPathExpression();
	
	this.clientID = clientID;
	this.name = "";
	this.validationName = "";
	this.customCalLang = "";
	this.customCalExpr = "";
	this.customValLang = "";
	this.configXML = null;
	this.contentElement = null;
	this.fieldElem = null;
	this.fieldAttributes = {};
	this.defaultErrorMessage = "";
	
	this.init = function(contentElement)
	{
		this.contentElement = contentElement;
	}
	this.setDefaultVal = function(sType)
	{
		document.getElementById("datatypeSelectedValue").value = sType;
	}
	this.loadContentTree = function(contentTree)
	{
		m_val_ekXPathExpr.loadContentTree(contentTree);
	}
	this.read = function(oFieldElem, attrName)
	{
		if (!attrName) attrName = "ektdesignns_validation";
		this.fieldAttributes =
		{
			ektdesignns_validation: null,
			ektdesignns_translate: null,
			ektdesignns_datatype: null,
			ektdesignns_basetype: null,
			ektdesignns_schema: null,
			ektdesignns_normalize: null,
			ektdesignns_validate: null,
			ektdesignns_invalidmsg: null,
			onblur: null
		};
		for (var attr in this.fieldAttributes)
		{
			if ("onblur" == attr)
			{
				var node = oFieldElem.getAttributeNode(attr);
				this.fieldAttributes[attr] = node ? node.nodeValue : null;
			}
			else
			{
				this.fieldAttributes[attr] = oFieldElem.getAttribute(attr);
			}
		}
		this.fieldElem = oFieldElem;
		var customValExpr = ""
		this.name = (this.fieldAttributes[attrName] || "none");
		if ("ektdesignns_datatype" == attrName)
		{
			// Translate from datatype to <choice name> in ValidateSpec.xml <validation name="datatype">
			switch (this.name)
			{
				case "string": 
					this.name = "none"; 
					break;
				case "nonNegativeInteger": 
					this.name = "nonNegInt";
					break;
				case "anyURI": 
					this.name = "url";
					break;
				default:
					// datatype is the same as choice name
					this.name = this.name.replace(/validation\:/, "");
					break;
			}
		}
		document.getElementById("v8nSelectedValue").value = this.name;
		if ("custom" == this.name)
		{
			var strDataType = this.fieldAttributes["ektdesignns_datatype"] || "";
			document.getElementById("datatypeSelectedValue").value = strDataType;
			var strValue = $ektron.htmlDecode(this.fieldAttributes["ektdesignns_normalize"]);
			if ("xpath:" == strValue.substr(0, 6))
			{
				this.customCalLang = "xpath";
				this.customCalExpr = strValue.substr(6);
			}
			else if ("xpathr:" == strValue.substr(0, 7))
			{
				this.customCalLang = "xpathr";
				this.customCalExpr = strValue.substr(7);
			}
			strValue = $ektron.htmlDecode(this.fieldAttributes["ektdesignns_validate"]);
			strValue = m_val_ekXPathExpr.removeComparator(strValue, ("ektdesignns_calendar" == oFieldElem.className ? "date" : strDataType));
			if ("xpath:" == strValue.substr(0, 6))
			{
				this.customValLang = "xpath";
				customValExpr = strValue.substr(6);
			}
			else if ("xpathr:" == strValue.substr(0, 7))
			{
				this.customValLang = "xpathr";
				customValExpr = strValue.substr(7);
			}
		}
		document.getElementById("txtValXPath").value = customValExpr;
		var ErrMsg = (this.fieldAttributes["ektdesignns_invalidmsg"] || "");
		document.getElementById("ErrorMessage").value = $ektron.htmlDecode(ErrMsg);
   }
	this.update = function(oFieldElem)
	{
		this.fieldElem = oFieldElem;
		for (var attr in this.fieldAttributes)
		{
			oFieldElem.removeAttribute(attr);
		}
		if (null == references.cboV8n.SelectedItem) 
		{
			for (var attr in this.fieldAttributes)
			{
				var value = this.fieldAttributes[attr];
				if (value != null)
				{
					value = value + ""; // ensure string
					if (value.length > 0)
					{
						oFieldElem.setAttribute(attr, value);
					}
				}
			}
			return;
		}
		else
		{
			this.name = (references.cboV8n.SelectedItem.Value || "custom");
		}
		if ("none" == this.name || 0 == this.name.length)
		{
			return;
		}

		var customErrorMessage = document.getElementById("ErrorMessage").value;
		var strNameNotReq = "";
		var strREPattern = "";
		var strCaption = "";
		var strSchema = "";
		var strCalLang = "";
		var strCalExpr = "";
		var strValLang = "";
		var strValExpr = "";
		var strInvalidMsg = "";
		var strDataType = "";
		var strBaseType = "";
		var strTranslate = "";
		var strDefaultTranslate = "";
		var strAction = "";
		var matchNode = null;
		var bCommonREExprUsed = false;
		var strExt = "";
		if (this.name.length > 4)
		{
			strExt = this.name.substr(this.name.length - 4);
			if ("-req" == strExt)
			{
				strNameNotReq = this.name.substr(0, this.name.length - 4);
			}
        }
        
		var strDefaultBaseType = "text";

		if (null == this.configXML) //cache the xml
		{
			var path = window.location.href;
			var pos = path.indexOf("/dialogs/");
			if (pos > -1)
			{
				path = path.substr(0, pos + 1);
			}
			var ekXml = new Ektron.Xml({ srcPath: path });
			this.configXML = ekXml.loadXml("[srcPath]ValidateSpec.xml");
		}
		if (this.configXML != null)   
		{
			for (var i=0; i < this.configXML.lastChild.childNodes.length; i++)
			{
				var oNode = this.configXML.lastChild.childNodes[i];
				if (oNode.nodeType != 1 /*Node.ELEMENT_NODE*/) continue;
				if ("validation" == oNode.tagName && this.validationName.toLowerCase() == $ektron.toStr(oNode.getAttribute("name")).toLowerCase())
				{
					for (var j = 0; j < oNode.childNodes.length; j++)
					{ 
						var oChildNode = oNode.childNodes[j];
						if (oChildNode.nodeType != 1 /*Node.ELEMENT_NODE*/) continue;
						var nodeName = $ektron.toStr(oChildNode.getAttribute("name"));
						if (strNameNotReq === nodeName)
						{
							matchNode = oChildNode;
							strREPattern = $ektron.toStr(oChildNode.getAttribute("pattern"));
							if (strREPattern && strREPattern.length > 0)
							{
								bCommonREExprUsed = true;
								strCaption = this.defaultErrorMessage;  
								strDefaultDataType = "string";
								strDefaultBaseType = $ektron.toStr(oChildNode.getAttribute("treeImg"));
								strDefaultTranslate = $ektron.toStr(oChildNode.getAttribute("translate"));
							}
							// keep searching in case exact match is found
						}
						else if (this.name === nodeName)
						{
							matchNode = oChildNode;
							strREPattern = $ektron.toStr(oChildNode.getAttribute("pattern"));
							if (strREPattern && strREPattern.length > 0)
							{
								bCommonREExprUsed = true;
								strCaption = this.defaultErrorMessage; 
								strDefaultDataType = "string";
								strDefaultBaseType = $ektron.toStr(oChildNode.getAttribute("treeImg"));
								strDefaultTranslate = $ektron.toStr(oChildNode.getAttribute("translate"));
							}
							break;
						}
					} // for
					
					oFieldElem.setAttribute("ektdesignns_validation", this.name);
             
					if ("custom" == this.name) 
					{
						strCaption = "";
						strDataType = references.cboDataType.GetValue();
						if ("{\"\"}" == strDataType) strDataType = "";
						// Find strBaseType
						if ("custom" == oChildNode.tagName)
						{
							var oCustomNode = oChildNode;
							for (var k = 0; k < oCustomNode.childNodes.length; k++)
							{
								var oCurrNode = oCustomNode.childNodes[k];
								if (oCurrNode.nodeType != 1 /*Node.ELEMENT_NODE*/) continue;
								if ("selections" == oCurrNode.tagName && "datatype" === oCurrNode.getAttribute("name"))
								{
									for (var m = 0; m < oCurrNode.childNodes.length; m++)    
									{
										oNode = oCurrNode.childNodes[m];
										if (oNode.nodeType != 1 /*Node.ELEMENT_NODE*/) continue;
										if ("listchoice" == oNode.tagName && strDataType === oNode.getAttribute("value"))
										{
											strBaseType = $ektron.toStr(oNode.getAttribute("treeImg"));
											strTranslate = $ektron.toStr(oNode.getAttribute("translate"));
											break;   
										}
									}
									if (strBaseType != "") break;
								}
							}
						}
						strSchema = "";
						if (this.customCalLang.length > 0)
						{
							strCalLang = this.customCalLang;
						}
						else
						{
							strCalLang = "xpathr"; // may reference other nodes within the XML document
						}
						strCalExpr = this.customCalExpr;
						if (this.customValLang.length > 0)
						{
							strValLang = this.customValLang;
						}
						else
						{
							strValLang = "xpathr"; // may reference other nodes within the XML document
						}
						strValExpr = document.getElementById("txtValXPath").value;
						strValExpr = m_val_ekXPathExpr.applyComparator(strValExpr, ("ektdesignns_calendar" == oFieldElem.className ? "date" : strDataType));
						strInvalidMsg = customErrorMessage;
					}
					else if (this.name === matchNode.getAttribute("name") && !bCommonREExprUsed)
					{
						oNode = matchNode;
						strBaseType = $ektron.toStr(oNode.getAttribute("treeImg"));  
						strTranslate = $ektron.toStr(oNode.getAttribute("translate"));
						for (var n = 0; n < oNode.childNodes.length; n++) 
						{
							var ConfigExpression = null;
							var oChildNode = oNode.childNodes[n];
							if (oChildNode.nodeType != 1 /*Node.ELEMENT_NODE*/) continue;
							var tagName = oChildNode.tagName;
							switch(tagName)
							{
								case "caption":
									strCaption = oChildNode.text || this.defaultErrorMessage;
									break;
								case "schema":
									strDataType = $ektron.toStr(oChildNode.getAttribute("datatype"));
									var oSchema = oChildNode;
									if (oSchema.childNodes.length > 0)
									{
										strSchema = Ektron.Xml.serializeXml(oSchema);
										// Remove outer tag
										strSchema = strSchema.replace(/^\s*<schema[^>]*>\s*/,"").replace(/\s*<\/schema>\s*$/,"");
										// Remove namespace
										strSchema = strSchema.replace(/\s+xmlns\:xs=\"http\:\/\/www\.w3\.org\/2001\/XMLSchema\"/g, "");
										// Remove indentation
										strSchema = strSchema.replace(/\n[\t ]+/g, "");
									}
									break;
								case "calculate":
									ConfigExpression = this.GetConfigExpression(oChildNode); //byref
									strCalLang = ConfigExpression.ConfigExpressionLang;
									strCalExpr = ConfigExpression.ConfigExpressionValue;
									break;
								case "validate":
									ConfigExpression = this.GetConfigExpression(oChildNode, strREPattern); //byref
									strValLang = ConfigExpression.ConfigExpressionLang;
									strValExpr = ConfigExpression.ConfigExpressionValue;
									strValExpr = m_val_ekXPathExpr.applyComparator(strValExpr, ("ektdesignns_calendar" == oFieldElem.className ? "date" : strDataType));
									if (customErrorMessage.length > 0)
									{
										strInvalidMsg = customErrorMessage;
									}
									else if ("errormessage" == oNode.firstChild.tagName) 
									{
										strInvalidMsg = oNode.firstChild.text;
									}    
									else
									{
										strInvalidMsg = "";
									}
									break;
								default:
									break;
							}
						} // for
					}
					else if (bCommonREExprUsed)
					{
						//already created the default above
						strDataType = strDefaultDataType;
						strBaseType = strDefaultBaseType;
						strTranslate = strDefaultTranslate;
                        
						if ("-req" == strExt)
						{
							strSchema = strREPattern;
							strValExpr = "/^(" + strREPattern + ")$/"; // with wholeline=true
						}    
						else
						{
							strSchema = "(" + strREPattern + ")|(.{0})";
							strValExpr = "/^(" + strREPattern + ")$|^$/";
						}
						strSchema = "<xs:pattern value=\"" + strSchema.replace(/\"/g, "\&quot;") + "\" />";
						strValLang = "re";
						strInvalidMsg = customErrorMessage;
					}
					break;
				}   
			}  // for  
			if (0 == strInvalidMsg.length)
			{
				strInvalidMsg = strCaption;
			}
            
			if (strTranslate && strTranslate.length > 0)
			{
				oFieldElem.setAttribute("ektdesignns_translate", strTranslate);
			}
			if (strDataType && strDataType.length > 0)
			{
				oFieldElem.setAttribute("ektdesignns_datatype", strDataType);
			}
			if (strBaseType && strBaseType.length > 0)
			{
				oFieldElem.setAttribute("ektdesignns_basetype", strBaseType);
			}
			if (strSchema && strSchema.length > 0)
			{
				oFieldElem.setAttribute("ektdesignns_schema", strSchema); 
			}
			if (strCalExpr && strCalExpr.length > 0)
			{
				strAction += this.SerializeExpression(oFieldElem, strCalLang, strCalExpr, "normalize", "normalize");
			}
			if (strValExpr && strValExpr.length > 0)
			{
				strAction += this.SerializeExpression(oFieldElem, strValLang, strValExpr, "validate", "validate", strInvalidMsg);
			}
			if (customErrorMessage.length > 0)
			{
				oFieldElem.setAttribute("ektdesignns_invalidmsg", customErrorMessage);
			}
			if (strAction.length > 0)
			{
				oFieldElem.setAttribute("onblur", Ektron.String.escapeJavaScriptAttributeValue(strAction));
			}
		}
	}
	this.showCustomValidation = function(bShow)
	{
		if (bShow)
		{
			$ektron("#fsCustomV8n").show();
		}
		else
		{
			this.enableCustomValidation(false);
			$ektron("#fsCustomV8n").hide();
		}
	}
	this.enableCustomValidation = function(bEnabled)
	{
		document.getElementById("fsCustomV8n").disabled = !bEnabled;
		document.getElementById(references.cboDataType.ClientID).disabled = !bEnabled;
		document.getElementById("txtValXPath").disabled = !bEnabled;
		document.getElementById(references.cboValEx.ClientID).disabled = !bEnabled;
		$ektron("#fsCustomV8n").css("opacity", bEnabled ? 1.00 : 0.50);
		if (bEnabled) 
		{
			if (this.validationName != this.validationLoaded)
			{
				var dataTypeBox = references.cboDataType;   
				dataTypeBox.RequestItems(this.validationName, false);

				var examplesBox = references.cboValEx;
				examplesBox.SetText(" ");
				examplesBox.RequestItems(this.validationName, false);
                
				this.validationLoaded = this.validationName;
				
				m_val_ekXPathExpr.init("txtValXPath", EkFieldValidationResourceText.sCondition, this.contentElement, this.fieldElem, references.validationTree.ClientID);
			}

			this.showCustomValidation(true);
			$ektron("#" + references.validationTree.fsTree.ClientID).show();
			// add tabindex to the tree nodes when they become visible so they can be tab into.
            var RadTreeReady = setInterval(function()
            {
                var treeview = ekFieldTreeViewControl[references.validationTree.ClientID].setupTreeNodes();
                if (treeview && treeview.SelectedNodesCount() > 0)
                {
                    clearInterval(RadTreeReady);
                }
            }, 1000);
        }
		else
		{
			$ektron("#" + references.validationTree.fsTree.ClientID).hide();
		}
	}
	this.reloadV8nBox = function(sReloadXml, sValidationName)
	{
		if (0 == sReloadXml.length) return;
		if (0 == this.validationName.length || sValidationName != this.validationName)
		{
			this.validationName = sValidationName;
			var comboBox = references.cboV8n;
			var bEnableCustom = ("custom" == this.name);
			document.getElementById("v8nSelectedValue").value = this.name || "none";
			comboBox.RequestItems(this.validationName, false);
			document.getElementById("ErrorMessage").value = "";
			this.enableCustomValidation(bEnableCustom);
		}
	}  
	this.GetConfigExpression = function(oNode, sDefValue) 
	{
		var strLanguage = "";
		var strValue = "";
		for (var n = 0; n < oNode.childNodes.length; n++) 
		{
			var oTheChild = oNode.childNodes[n];
			if (oTheChild.nodeType != 1 /*Node.ELEMENT_NODE*/) continue;
			var tagName = oTheChild.tagName;
			switch(tagName)
			{
				case "regexp":
					// <regexp pattern="pattern" wholeline=t|f ignorecase=t|f global=t|f multiline=t|f>/pattern/flags</regexp>
					strLanguage = "re";
					strValue = oTheChild.text; 
					if (0 == strValue.length)
					{
						if (sDefValue && 0 == sDefValue.length)
						{
							sDefValue = ".*";
						}
						strValue = $ektron.toStr(oTheChild.getAttribute("pattern"), sDefValue);
						if ($ektron.toBool(oTheChild.getAttribute("wholeline"), false))
						{
							strValue = "^(" + strValue + ")$";
						}
						strValue = "/" + strValue + "/";
						if ($ektron.toBool(oTheChild.getAttribute("ignorecase"), false))
						{
							strValue = strValue + "i";
						}
						if ($ektron.toBool(oTheChild.getAttribute("global"), false))
						{
							strValue = strValue + "g";
						}
						if ($ektron.toBool(oTheChild.getAttribute("multiline"), false))
						{
							strValue = strValue + "m";
						}
					}
					break;
				case "script":
					// <script value="javascript-expression"/>
					strLanguage = "js";
					strValue = $ektron.toStr(oTheChild.getAttribute("value"));
					break;
				case "xpath":
					// <xpath select="xpath-expression" selfonly=t|f/>
					// See also Serialize and Deserialize
					if ($ektron.toBool(oTheChild.getAttribute("selfonly"), true))
					{
						strLanguage = "xpath";
					}
					else
					{
						strLanguage = "xpathr"; // whole xml document from root, not just self
					}
					strValue = $ektron.toStr(oTheChild.getAttribute("select"));
					break;
				case "xslt":
					// <xslt src="url"/>
					strLanguage = "xslt";
					strValue = $ektron.toStr(oTheChild.getAttribute("src"));
					break;
				default:
					break;
			}
		} // for
		return {ConfigExpressionLang:strLanguage, ConfigExpressionValue:strValue};
	}
	this.SerializeExpression = function(oFieldElem, strLanguage, strValue, strAttrib, strFunction, strInvalidMsg)
	{                
		// Utility function
		var strExpression = "";
		oFieldElem.setAttribute("ektdesignns_" + strAttrib, strLanguage + ":" + strValue);
		if ("re" == strLanguage)
		{
			strExpression = strValue;
		}
		else
		{
			// design_validate_js and _xpath etc.
			strExpression = $ektron.toLiteral(strValue);
		}
		var strAction = "design_" + strFunction + "_" + strLanguage + "(" + strExpression + ",this";
		if (strInvalidMsg && strInvalidMsg.length > 0)
		{
			strAction = strAction + "," + $ektron.toLiteral(strInvalidMsg);
		}
		strAction = strAction + ");";
		return strAction;
	}
}


function cboV8n_OnClientSelectedV8nIndexChanged(item)
{
    var name = (item.Value || "custom");
    var bEnableCustom = ("custom" == name);
    ekFieldValidationControl.name = name;
    ekFieldValidationControl.enableCustomValidation(bEnableCustom);
    ekFieldValidationControl.defaultErrorMessage = ("none" == name ? "" : item.Text);
    document.getElementById("ErrorMessage").value = ekFieldValidationControl.defaultErrorMessage;
}
function onValComboOpening(combobox)
{
    // disabled the custom validation if the v8n option is not "(custom)"
    return !document.getElementById("fsCustomV8n").disabled;    
}
function cboValEx_OnClientSelectedIndexChanged(item)
{
    document.getElementById("txtValXPath").value = item.Value;
}
function cboV8n_SetSelectedItem(combo)
{
    if (0 == combo.Items.length) return;
	var customItem = combo.FindItemByValue("custom");
	ekFieldValidationControl.showCustomValidation(customItem != null);
	
    var preselectedValue = document.getElementById("v8nSelectedValue").value;
    var comboItem = combo.FindItemByValue(preselectedValue);
    if (null == comboItem && preselectedValue && preselectedValue.length > 4)
    {
		var strExt = preselectedValue.substr(preselectedValue.length - 4);
		if ("-req" == strExt)
		{
			preselectedValue = preselectedValue.substr(0, preselectedValue.length - 4);
			comboItem = combo.FindItemByValue(preselectedValue);
		}
    }
    if (null == comboItem) comboItem = combo.Items[0];
    var name = (comboItem.Value || "custom");
    document.getElementById("ErrorMessage").value = ("none" == name ? "" : ekFieldValidationControl.fieldAttributes["ektdesignns_invalidmsg"]);
    comboItem.Select();
}
function cboDataType_SetSelectedItem(combo)
{
    var preselectedValue = document.getElementById("datatypeSelectedValue").value;
    if ("" == preselectedValue)
    {
		preselectedValue = "{\"\"}";
    }
    if (combo.Items.length > 0)
	{
	    var comboItem = combo.FindItemByValue(preselectedValue);
        if (comboItem != null)
        {
            comboItem.Select();
        }
	}
}

Ektron.ready(function()
{
	$ektron(document).bind("onValidateDialog", function(ev, oRet)
	{
		var errObj = null;
		if (false == document.getElementById("txtValXPath").disabled)
		{
			var strText = document.getElementById("txtValXPath").value;
			var aFieldNameVariable = EkFormFields_GetVariableNames(strText);	  
			if (aFieldNameVariable && aFieldNameVariable.length > 0)
			{
				//Still contains a field name variable that needs to be replaced.
				var sVar = aFieldNameVariable.join(", ");
				errObj = 
				{
					name:       "ucFieldValidation",
					message:    Ektron.String.format(EkFieldValidationResourceText.sExprContainsVars, sVar), 
					srcElement: document.getElementById("txtValXPath")
				};
			}
			if (errObj)
			{
				oRet.push(errObj);
			}
		}
	});
});