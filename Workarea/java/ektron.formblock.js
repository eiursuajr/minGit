/* Copyright 2009-2010, Ektron, Inc. */
if ("undefined" == typeof Ektron.FormBlock)
{
	Ektron.FormBlock = function(settings)
	{
		settings = $ektron.extend(
		{
			// (required) id // string			
			// (required) formId // Int64 as string
			isPoll: false, // boolean
			isSmartForm: false, // boolean
			//(if smart form) smartFormDesignId // Int64 as string
			//(if smart form) smartFormSchemaUrl // string
			legacyValidationRules: 0, // int
			// (required) smartFormContainingElementName // string
			// (required) formStateElementName // string
			onexception: null
		}, settings);
		
		this.init = function(settings)
		{
			if (!settings.id) throw new TypeError("Error: Ektron.FormBlock: 'id' is required.");
			if (!settings.formId) throw new TypeError("Error: Ektron.FormBlock: 'formId' is required.");
			if (!settings.smartFormContainingElementName) throw new TypeError("Error: Ektron.FormBlock: 'smartFormContainingElementName' is required.");
			if (!settings.formStateElementName) throw new TypeError("Error: Ektron.FormBlock: 'formStateElementName' is required.");
			
			this.onSubmitClick = function()
			{
				// event handler uses closure to access settings since 'this' is not the instance object when called
				Ektron.FormBlock.setTargetAction(settings.formId, "postback");
				Ektron.FormBlock.setState(settings.formStateElementName, "in");
			};
			
			$ektron.extend(this, settings);
			
			Ektron.FormBlock.setTargetAction(this.formId, "");
			$ektron("#" + this.smartFormContainingElementName + " input:submit, #" + this.smartFormContainingElementName + " input:image")
				.click(this.onSubmitClick);
		};
		this.init(settings);

		Ektron.FormBlock.instances[Ektron.FormBlock.instances.length] = this;
		Ektron.FormBlock.instances[this.id] = this;
	}; // constructor
	Ektron.FormBlock.onexception = Ektron.OnException.diagException;
	Ektron.FormBlock.instances = [];
	
	Ektron.FormBlock.create = function(settings)
	{
		var objInstance = Ektron.FormBlock.instances[settings.id];
		if (objInstance)
		{
			objInstance.init(settings);
		}
		else
		{
			objInstance = new Ektron.FormBlock(settings);
		}
		return objInstance;
	};

	Ektron.FormBlock.findActiveInstance = function()
	{
		var objActiveInstance = null;
		for (var i = 0; i < Ektron.FormBlock.instances.length; i++)
		{
			var objInstance = Ektron.FormBlock.instances[i];
			var currentState = document.getElementById(objInstance.formStateElementName);
			if (currentState != null && 0 == currentState.value.indexOf("in"))
			{
				objActiveInstance = objInstance;
				break;
			} 
		}
		return objActiveInstance;
	};

	Ektron.FormBlock.findByChildElement = function(oElem)
	{
		var objInstance = null;
		var eElem = $ektron(oElem);
		for (var i = 0; i < Ektron.FormBlock.instances.length; i++)
		{
			objInstance = Ektron.FormBlock.instances[i];
			if (eElem.parents("#" + objInstance.smartFormContainingElementName).length > 0)
			{
				break;
			} 
		}
		return objInstance;
	};

	Ektron.FormBlock.setState = function(id, state)
	{
		var fieldId = document.getElementById(id);
		if (fieldId != null)
		{
			fieldId.value = state;
		}
	};
	
	Ektron.FormBlock.setTargetAction = function(formId, state)
	{
		var fieldApplicationAPI = document.getElementById("ApplicationAPI" + formId);
		if (fieldApplicationAPI != null)
		{
			fieldApplicationAPI.value = state;
		}
	};
	
	Ektron.FormBlock.validate = function(objForm)
	{
		var bValid = true;
		var legacyValidation = false;
		var containingElement = null;
		var formId = "";
		var objInstance = Ektron.FormBlock.findActiveInstance();
		if (objInstance != null)
		{
			formId = objInstance.formId;
			legacyValidation = true;
			if (!objInstance.legacyValidationRules)
			{
				containingElement = document.getElementById(objInstance.smartFormContainingElementName);
			}
		} 
		if (containingElement != null && Ektron.SmartForm && "function" == typeof Ektron.SmartForm.onsubmitForm)
		{
			bValid = Ektron.SmartForm.onsubmitForm(containingElement);
		}
		else if (containingElement != null && "function" == typeof design_validateHtmlForm)
		{
			// Ektron.SmartForm.onsubmitForm
			bValid = true;
			var oElem = design_validateHtmlForm(containingElement);
			if (oElem && oElem.title != "") 
			{
				alert(oElem.title);
				// Ektron.SmartForm.focusOn
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
				bValid = false;
			}
			oElem = null;
		}
		else if (legacyValidation)
		{
			bValid = EkFmValidate(objForm);
		}
		if (false == bValid)
		{
			Ektron.FormBlock.setTargetAction(formId, "");
		}
		return bValid;
	};
	
	if ("undefined" == typeof window.customValidationStyle)
	{
		window.customValidationStyle = function(oElem, isValid)
		{
			var objInstance = Ektron.FormBlock.findByChildElement(oElem);
			if (objInstance && objInstance.isPoll)
			{
				return true; // no style for poll
			}
			else if (Ektron.SmartForm && "function" == typeof Ektron.SmartForm.validate_complete)
			{
				return false; // default style
			}
			else if ("function" == typeof design_validationStyle)
			{
				return design_validationStyle(oElem, isValid);
			}
			return false; // default style
		}
	}
}

if ("undefined" == typeof ektRefreshReport)
{
	function ektRefreshReport(sUrl)
	{
		var href = window.location.href;
		var param = "reporttarget=1";
		if (-1 == href.indexOf(param))
		{
			if (href.indexOf("?") > -1)
			{
				href += "&" + param;
			}
			else
			{
				href += "?" + param;
			}
		}
		window.location.replace(href);
	}
}