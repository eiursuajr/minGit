var templateParamValues = {};
var currentTemplateElement = null;

var RuleWizardManager =
{
	//////////
	//
	// name: showInputForm
	// desc: shows the input form corresponding to the input element
	//       associated with element { predicate id, form name }.
	//

	showInputForm: function( evt, element )
	{
	    if (s_action == "view")
		{
		    // Do nothing;
		} else {
	        currentTemplateElement = element;
		    var dataBox = document.getElementById( "dataBox" );
		    dataBox.style.display = "inline";
    		
		    var e = window.event ? window.event : evt;
		    
		    dataBox.style.left = e.clientX + 'px';
		    dataBox.style.top = e.clientY + 'px';
		    if (IsBrowserIE())
		    {
		        dataBox.style.position = "absolute";
    		}
    		else
    		{
    		    dataBox.style.position = "fixed";
    		}
		    RuleWizardManager.highlightSelectedParam(element)

		    var data = templateParamValues[element.id];
		    var dataBoxText = document.getElementById( "dataBoxText" );
		    dataBoxText.value = data ? data : "";
		    dataBoxText.focus();
		}
	},
	
	highlightSelectedParam: function( element )
	{
	    RuleWizardManager.unhighlightSelectedParams();
		element.className = "dataInputTextSelected";
	},	

	unhighlightSelectedParams: function()
	{
		// deselect all currently selected
		var elements = DOMUtil.getElementByClassName( "dataInputTextSelected" );

		for( var i = 0; i < elements.length; i++ ) {
			elements[i].className = "dataInputText";
		}
	},
	
	//////////
	//
	// name: submitInputForm
	// desc: Takes the data in the input form and stores it to memory.
	//       It also updates the UI with the submitted values. If the
	//       values submited are empty, it removes the items from the
	//       UI and udpates memory accordingly.
	//
	submitInputForm: function()
	{
	    RuleWizardManager.submitInputFormBase(false);
	},
	
	submitInputFormCheck: function()
	{
	    RuleWizardManager.submitInputFormBase(true);
	},

	submitInputFormBase: function(checkval)
	{
		//alert( "submitInputForm" );
		var bgo = true;
		var dataBoxText = document.getElementById( "dataBoxText" );
		var id    = RuleWizardManager.getCurrentTemplateId();
		var name  = RuleWizardManager.getCurrentTemplateName();
		var type  = RuleWizardManager.getCurrentTemplateType();
		var value = dataBoxText.value;

        // type is "action" or "predicate"
        // id is the template id
        // name is the name of the param
        if (checkval == true)
        {
            if (Trim(value).length < 1)
            {
                bgo = false;
                alert('Please enter in a value');
            }
        }
        if (bgo == true)
        {
            var displayValue = currentTemplateElement.title;
		    if( value != "" ) {
			    displayValue += ' "<b>' + value + '</b>" ';
		    }
		    currentTemplateElement.innerHTML = displayValue;
		    value = value == "" ? null : value;
            templateParamValues[currentTemplateElement.id] = value;
		    RuleWizardManager.hideInputForm();
		}
	},
	
	//////////
	//
	// name: hideInputForm
	// desc: Hides the input form
	//

	hideInputForm: function()
	{
	    RuleWizardManager.unhighlightSelectedParams();
		var dataBox = document.getElementById( "dataBox" );
		dataBox.style.display = "none";
	},
	
	//////////
	//
	// name: submitWizardForm
	// desc: Submits the rule wizard data to update or create
	//       a new rule. This function takes the form data and
	//       builds up a querystring.
	//

	submitWizardForm: function()
	{
	    var ruleForm = document.forms[0];
		var boxes = ruleForm.elements["checkbox"];
		var bCondition = false;
		var bActionTrue = false;
		var bActionFalse = false;
		var iCurrentID = -1;
		var logicalOperatorList = ruleForm["logicalOperator"];
		var soutput = "<rule name=\"" + escape(ruleForm["ruleNameText"].value) + "\">";
		var querystring = "wizard-with-steps.aspx?id=" + ruleForm["ruleid"].value + "&rulesetid=" + ruleForm["rulesetid"].value + "&type=" + ruleForm["action"].value + "&action=process&rule_name=" + escape(ruleForm["ruleNameText"].value) + "&";
		querystring += "logical_operator=" + escape(logicalOperatorList.options[logicalOperatorList.selectedIndex].value) + "&";
        //soutput += "<condition><predicate type=\"" + escape(logicalOperatorList.options[logicalOperatorList.selectedIndex].value) + "\">"
		for( var i = 0; i < boxes.length; i++ ) {
			var box = boxes[i];
			if( box.checked == true ) {
                var type_and_id = box.value.split( "_" );
                var type = type_and_id[0];
                var id = type_and_id[1];
                var document_all =  document.all || document.getElementsByTagName('*');
                for( var j = 0; j < document_all.length; j++ ) {
                    var element = document_all[j];
                    if( element.id.indexOf( type + "{:_:}" + id + "{:_:}" ) != -1 ) {
                        var data = RuleWizardManager.parseTemplateIdData(element.id);
                        if( templateParamValues[element.id] == null ) {
                            if (data["type"] == "condition")
                            {
                                alert( "A value for the Condition \"" + condition_templates[data["id"]].name + "'\" has been checked, but not set.\nTo set a value, click on an underlined word" );                                
                            }
                            else if (data["type"] == "actiontrue")
                            {
                                alert( "A value for the Action on True \"" + actiontrue_templates[data["id"]].name + "'\" has been checked, but not set.\nTo set a value, click on an underlined word" );
                            }
                            else if (data["type"] == "actionfalse")
                            {
                                alert( "A value for the Action on False \"" + actionfalse_templates[data["id"]].name + "'\" has been checked, but not set.\nTo set a value, click on an underlined word" );
                            }
                            return;
                        }
                        
                        var rExp = new RegExp("\\[" + data["name"] + "\\]","g");
                        if (data["type"] == "condition") 
                        {
                            condition_templates[data["id"]].predicate = condition_templates[data["id"]].predicate.replace(rExp, templateParamValues[element.id]);
                            querystring += data["type"] + "_" + condition_templates[data["id"]].templateid + "_" + data["id"] + "_" + escape( data["name"] );
                        }
                        else if (data["type"] == "actiontrue")
                        {
                            querystring += data["type"] + "_" + actiontrue_templates[data["id"]].templateid + "_" + data["id"] + "_" + escape( data["name"] );
                        }
                        else if (data["type"] == "actionfalse")
                        {
                            querystring += data["type"] + "_" + actionfalse_templates[data["id"]].templateid + "_" + data["id"] + "_" + escape( data["name"] );
                        }
                        querystring += "=";
                        querystring += escape( templateParamValues[element.id] ) + "&";
                    }
                }
			}
		}
		
		for (var j = 0; j < condition_templates.length; j++)
		{
		     soutput += condition_templates[j].predicate;
		}
		//soutput += "</predicate></condition></rule>";
		window.location.href = querystring;
	},
	
	setDefaultRuleName: function( checkBoxElement )
	{
	    if( checkBoxElement.checked == true ) {
    	    var checkBoxValue = checkBoxElement.value;
    	    // get the verbiage of the currently checked rule
    	    var ruleItemTextElement = document.getElementById( checkBoxValue );
    	    var ruleVerbiage = ruleItemTextElement.innerText;
    	    var ruleNameElement = document.forms[0]["ruleNameText"];
            var ruleName = ruleNameElement.value;
            if( ruleName == "" ) {
        	    //ruleNameElement.value = ruleVerbiage;
        	}
        }
	},

    getCurrentTemplateId: function()
	{
        var data = RuleWizardManager.getCurrentTemplateData();
        return data["id"];
	},

	getCurrentTemplateName: function()
	{
        var data = RuleWizardManager.getCurrentTemplateData();
        return data["name"];
	},

	getCurrentTemplateType: function()
	{
        var data = RuleWizardManager.getCurrentTemplateData();
        return data["type"];
	},
	
	getCurrentTemplateData: function()
	{
	    return RuleWizardManager.parseTemplateIdData(currentTemplateElement.id);
	},
	
	parseTemplateIdData: function( template )
	{
	    // takes the array and returns a map with the following keys
	    var keys = { _0 : "type", _1 : "id", _2 : "name" };
	    var templateData = {};
	    var templateInfo = template.split( "{:_:}" );
	    for( var i = 0; i < templateInfo.length; i++ ) {
	        templateData[keys["_" + i]] = templateInfo[i];
        }
		return templateData;
	},

	data : {}
}
