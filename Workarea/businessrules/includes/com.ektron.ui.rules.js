var RuleWizard =
{
	displayScreen: function()
	{
		// make sure we're sorted by order before we display rules
		ruleset.sort(sortByOrder)

		var content = "";
		content += "<div class='ruleset' id='rulesetpane'>";
		for( var i = 0; i < ruleset.length; i++ ) {
			var rule = ruleset[i];
			content += "<div class='ruleEditorItem' onclick=\"RuleWizard.toggleRuleItem(this)\" id='" + rule.id + "_ruleItem'>";
			content += "<input type='checkbox' ";

			if( rule.active ) {
				content += " checked=true";
			}

			if (s_action == "view")
			{
			    content += " disabled";
			}

			content += " onclick=\"RuleWizard.selectRuleItem(this);RuleWizard.toggleActiveState(this);event.cancelBubble=true\">";
			content += "<span class='ruleEditorItemName'>" + rule.name + "</span>";
			content += "&nbsp;<a href=\"#\" onclick=\"window.open('wizard-with-steps.aspx?action=view&id=" + rule.id + "','','width=480,height=350,resizable,scrollbars=no,status=1');event.cancelBubble=true;\">(View)</a></div>";
		}
		content += "</div>";

		var container = document.getElementById("rulesetContainer");
		container.innerHTML = content;
	},

	getSelectedRule: function()
	{
		// this returns an array
		var selectedRules = DOMUtil.getElementByClassName( "ruleEditorItemSelected" );
		var selectedRule  = null;

		if( selectedRules != null ) {
			selectedRule  = selectedRules.length > 0 ? selectedRules[0] : null;
		}

		return selectedRule;
	},

	moveRuleItem: function( direction, ruleEditorItem )
	{
		if( ruleEditorItem != null ) {
			var rule = RuleWizard.getRuleByHtmlId(ruleEditorItem.id);

			if( rule != null ) {
				var id   = rule.id;
				var newPosition = null;

				newPosition = rule.order + 1;
				if( direction == "up" ) {
					newPosition = rule.order - 1;
				}

				RuleWizard.swapRulesByOrder(rule.order, newPosition);
				RuleWizard.displayScreen( "ruleset", ruleset );

				// after we call displayScreen, we have to reselect
				// our originally selected rule
				ruleEditorItem = document.getElementById( id + "_ruleItem" );
				ruleEditorItem.className = "ruleEditorItemSelected";
			}
		}
	},

	removeRuleItem: function( ruleEditorItem )
	{
		if( ruleEditorItem != null ) {
			var rule = RuleWizard.getRuleByHtmlId(ruleEditorItem.id);
			if( rule != null ) {
			    var finalok = confirm("Remove selected rules from this ruleset only? The rules will not be deleted.");
			    if (finalok == true)
			    {
                    RuleWizard.removeRuleById( rule.id );

                    // when we remove a rule, we create a "gap" in the order
                    // values. E.g. order "3" may be missing. NormalizeOrderValues
                    // makes sure the ordere values are monotonically increasing
                    RuleWizard.normalizeOrderValues();

                    RuleWizard.displayScreen( "ruleset", ruleset );
				}
			}
		}
		else
		{
		    alert(msgRuleToRemove);
		}
	},

	///////////
	//
	// name: normalizeOrderValues
	// desc: When we remove a rule from the ruleset, we create a "gap" in
	// the order values. E.g. order "3" may be missing. NormalizeOrderValues
	// makes sure the ordere values are monotonically increasing
	//
	normalizeOrderValues: function()
	{
		ruleset.sort(sortByOrder);
		for( var i = 0; i < ruleset.length; i++ ) {
			var rule = ruleset[i];
			rule.order = i;
		}
	},

	///////////
	//
	// name: selectRuleItem
	// desc: Given an DOM element, this makes it appear selected
	//
	selectRuleItem: function( ruleEditorItem )
	{
	    if (ruleEditorItem.type != "checkbox")
	    {
		    ruleEditorItem.className = "ruleEditorItemSelected";
		}
	},

	toggleRuleItem: function( ruleEditorItem )
	{
		var toggleClassName = "ruleEditorItemSelected"
		if ( (ruleEditorItem.className == "ruleEditorItemSelected")) {
			toggleClassName = "ruleEditorItem";
		}

		// deselect all currently selected
		var elements = DOMUtil.getElementByClassName( "ruleEditorItemSelected" );
		for( var i = 0; i < elements.length; i++ ) {
			elements[i].className = "ruleEditorItem";
		}

		// select our item
		ruleEditorItem.className = toggleClassName;
	},

	getRuleById: function( id )
	{
		var rule = null;
		for( var i = 0; i < ruleset.length; i++ ) {
			rule = ruleset[i]
			if( rule.id == id ) {
				break;
			}
		}

		return rule;
	},

	getRuleByPosition: function( position )
	{
		var rule = null;

		for( var i = 0; i < ruleset.length; i++ ) {
			rule = ruleset[i]
			if( rule.order == position ) {
				break;
			}
		}

		return rule;
	},

	swapRulesByOrder: function( pos1, pos2 )
	{
		var ruleAtPos1 = RuleWizard.getRuleByPosition( pos1 );
		var ruleAtPos2 = RuleWizard.getRuleByPosition( pos2 );

		if( pos2 > -1 ) {
			if( ruleAtPos1 != null ) {
				if( ruleAtPos2 != null ) {
					var tmpPos1 = ruleAtPos1.order;
					ruleAtPos1.order = ruleAtPos2.order;
					ruleAtPos2.order = tmpPos1;
				}
			}
		}
	},

	toggleActiveState: function(checkBoxElement)
	{
		var ruleItemElement = checkBoxElement.parentNode;
		var rule = RuleWizard.getRuleByHtmlId(ruleItemElement.id);
		rule.active = rule.active ? false : true;
	},

	getRuleByHtmlId: function( HtmlId )
	{
		return RuleWizard.getRuleById(parseInt(HtmlId, 10));
	},

	removeRuleById: function( id )
	{
		var rule = null;
		var rulesetCopy = [];
		for( var i = 0; i < ruleset.length; i++ ) {
			rule = ruleset[i];
			if( rule.id != id ) {
				rulesetCopy[rulesetCopy.length] = rule;
			}
		}

		ruleset = rulesetCopy;
	},

	showNewRuleWizard: function()
	{
		window.location.href = "wizard-with-steps.aspx?action=add"; //,'Wizard','menubar=no,location=no,resizable=yes,scrollbars=yes,status=no');
	},

	showEditRuleWizard: function(ruleEditorItem)
	{

	    var fail = true;
	    if( ruleEditorItem != null ) {
			var rule = RuleWizard.getRuleByHtmlId(ruleEditorItem.id);
			if ( rule != null ) {
			    fail = false;
				window.location.href = "wizard-with-steps.aspx?action=edit&id=" + rule.id + "&rulesetid=" + document.getElementById("rulesetid").value; //,'Wizard','enubar=no,location=no,resizable=yes,scrollbars=yes,status=no');
			}
		}
		if (fail == true) {
		    alert(msgSelectRule);
		}
	},

	ruleset: null
}

function sortByOrder(a, b) {
	return( a["order"] - b["order"] )
}
