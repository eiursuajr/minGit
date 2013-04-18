var StatementWizard =
{
	displayScreen: function()
	{
		StatementWizard.renderTemplates( "condition" );
		StatementWizard.renderTemplates( "actiontrue" );
		StatementWizard.renderTemplates( "actionfalse" );
		StatementWizard.populateTemplateParams();
	},
	
	renderTemplates: function( type )
	{
		var content = "";
		var templates;
		if (type == "condition") {
		    templates = condition_templates;
		}
		else if (type == "actiontrue"){
		    templates = actiontrue_templates;
		}
		else if (type =="actionfalse") {
		    templates = actionfalse_templates;
		}
		
		for( var i = 0; i < templates.length; i++ ) {
			var template = templates[i];
			var id = template.id;
			var name = template.name;
			var template_text = template.template;
			var active = template.active;

			content += "<span class='conditionTemplate'>"
			content += "<input " + ( active ? "checked=checked" : "" );
			if (s_action == "view")
			{
			    content += " disabled ";
			}
			content += " onclick='RuleWizardManager.setDefaultRuleName(this);' type='checkbox' id='checkbox" + id + "' name='checkbox' value='" + type + "_" + id + "' />";
			content += "<span id='" + type + "_" + id + "'>" + template_text + "</span>";
			content += "</span>";
		}
		
		var container = document.getElementById( type + "Container" );
		container.innerHTML = content;
	},
	
	populateTemplateParams: function()
	{
    	var templates_data = { "condition": condition_template_params, "actiontrue": actiontrue_template_params, "actionfalse": actionfalse_template_params };
		for( var type in templates_data ) {
            var type_template_params = templates_data[type];
	        for( var i = 0; i < type_template_params.length; i++ ) {
	            template_params = type_template_params[i];
	            var id = template_params["id"];
	            var params = template_params["params"];
	            for( var j = 0; j < params.length; j++ ) {
	                var name_value_pair = params[j];
	                for( var k in name_value_pair ) {
	                    var param_name = k;
	                    var param_value = name_value_pair[k];
	                    var key = type + "{:_:}" + id + "{:_:}" + param_name;
	                    currentTemplateElement = document.getElementById(key);
                    	var dataBoxText = document.getElementById( "dataBoxText" );
                    	dataBoxText.value = param_value;
	                    RuleWizardManager.submitInputForm();
	                }
	            }
	        }
        }
	}
}
