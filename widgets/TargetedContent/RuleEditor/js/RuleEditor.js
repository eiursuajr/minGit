if ("undefined" == typeof Ektron.RuleEditor) Ektron.RuleEditor = 
{
    ResourceText: // TODO localize
    {
		sOK: "OK",
		sCancel: "Cancel",
		sCaptionIf: "If...",
		sCaptionOr: "or",
		sButtonAnd: "and",
		sMsgNoRules: "Click the '+ or' button to define the condition."
    },
    
    CssClasses:
    /* Contains all of the CSS Classes used in the Rule Builder. 
    
       These classes are referenced in HTML templates via: 
       {Property Name} */
    {
        RuleEditor: "ektronRuleEditor",
        RulesList: "rulesList", 
        RuleItem: "ruleItem",
        RuleAndLabel: "and", 
        RuleMessage: "message", 
        AddRuleList: "addRuleList",
        DeleteRule: "deleteAnd",
        RuleTemplatesData: "initRuleTemplatesData",
        RulesData: "initRulesData",
        SavedRulesData: "savedRulesData"
    },
    
    Exceptions: 
    /* Contains all of the exceptions used in the Rule Builder */
    {
        MissingAddButton: "Missing add button", 
        MissingRulesList: "Missing rules list",
        RuleNotFound: "Rule does not exist",
        InvalidRuleTemplate: "Rule template does not exist"
    },
    
    HTMLTemplates: 
    /* Contains all of the static HTML templates used for rendering */
    {
        Rule: '<div><a href="#deleteRule" class="{DeleteRule} ek-button ek-button-icon-only ui-corner-all ui-state-default" title="Delete"><span class="ui-icon ui-icon-circle-minus"></span></a>'+
              '<span class="{RuleMessage}"></span></div>',
        AddRuleMenu: "<ul class=\"ektron addRuleList ui-state-default\"></ul>",
        AddRuleSubmenu: '<li class="addRuleSubmenu"><a href="#" onclick="return false;"></a></li>',
        AddRuleButton: '<li class="addButtonItem"><a href="#addRule"></a></li>'
    }, 
    
    EditablePluginSettings:
    {
		submit: "OK", // TODO localize
		cancel: "Cancel", // TODO localize
		onblur: "ignore", // "submit" doesn't work for two reasons (1) onblur fires after onclick of the "Save" button, (2) onblur fires before "Cancel" button if mouse click is held down
		style: "display: inline"
    },
    
    /* Contains all of the rule templates to be used in the editor */
    RuleTemplates: {},
    
    instances: [],
    
    init: function(id, containerId)
    {
        var objInstance = { $container: $ektron("#" + containerId) };
        
        Ektron.RuleEditor.instances[id] = objInstance;
        
        if (0 == objInstance.$container.length) return null;
        
        objInstance.$container.addClass(Ektron.RuleEditor.CssClasses.RuleEditor);
        
        /* Read initial rule templates and rules from hidden field. */
        var ruleTemplates = Ektron.JSON.parse(objInstance.$container.find("."+Ektron.RuleEditor.CssClasses.RuleTemplatesData).val());
        var initialRules = Ektron.JSON.parse(objInstance.$container.find("."+Ektron.RuleEditor.CssClasses.RulesData).val());
        
        objInstance.constants = 
        /* Contains all constants */
        {
            InvalidRule: -1
        }
        
        objInstance.helpers = 
        /* Contains helper functions */
        {
            IsValidRuleID: function(ruleId)
            {
                return (ruleId != objInstance.constants.InvalidRule);
            }
        }
        
        objInstance.components = 
        /* Contains references to all pre-rendered DOM elements for the rule builder 
        */
        {
            addButtonList: objInstance.$container.find("."+Ektron.RuleEditor.CssClasses.AddRuleList),
            rulesList: objInstance.$container.find("."+Ektron.RuleEditor.CssClasses.RulesList)
        };
        
        objInstance.controller = 
        /* Contains all methods performing ACTIONS on the rule builder
        
           These methods should never be called by a MODEL, and should 
           never call each other. Ideally, each method should be called 
           by an event raised by the user (i.e. a button click, or 
           a onchanged event, etc...).
          */
        {
            /* Constructor */
            
            Init: function()
            {
                ruleTemplates = objInstance.ruleTemplateModel.FindAll();
                
                objInstance.view.Init(ruleTemplates);
            },
            
            /* Public Methods */
            
            AddRule: function(ruleTemplateID, parentRuleID)
            /* Adds a rule to the system and updates the UI. 
            
               A rule without a parent should take constants.InvalidRule for 
               parentRuleID. 
            */
            {
                var ruleTemplate = objInstance.ruleTemplateModel.FindByID(ruleTemplateID);
                if (ruleTemplate != null && ruleTemplate.enabled)
                {
					objInstance.ruleModel.Create(
					{
						ruleTemplate: ruleTemplate, 
						parentID: parentRuleID
					});
					var rules = objInstance.ruleModel.FindAll();
	                
					var ruleTemplates = objInstance.ruleTemplateModel.FindAll();
	            
					objInstance.view.RulesEditor(rules, ruleTemplates);
                }
            },
            
            LoadRules: function(rules)
            {
                var ruleTemplate;
                //add 'or' rules first
                $ektron.each(rules, function(i)
                {
                    if (objInstance.helpers.IsValidRuleID(rules[i].parentID) == false)
                    {
                        ruleTemplate = objInstance.ruleTemplateModel.FindByID(rules[i].ruleTemplateID);
                        if (ruleTemplate != null && ruleTemplate.enabled)
                        {
							objInstance.ruleModel.Create(
							{	// Ektron.RuleEditor.Rule
								id: rules[i].id,
								parentID: rules[i].parentID,
								ruleTemplate: ruleTemplate,
								values: rules[i].values
							});
                        }
                    }
                });
                
                // add 'and' rules last
                $ektron.each(rules, function(i)
                {
                    if (objInstance.helpers.IsValidRuleID(rules[i].parentID) == true)
                    {
                        ruleTemplate = objInstance.ruleTemplateModel.FindByID(rules[i].ruleTemplateID);
                        
                        if (ruleTemplate != null && ruleTemplate.enabled)
                        {
							objInstance.ruleModel.Create(
							{	// Ektron.RuleEditor.Rule
								id: rules[i].id,
								parentID: rules[i].parentID,
								ruleTemplate: ruleTemplate,
								values: rules[i].values
							});
						}
                    }
                });
                
                var rules = objInstance.ruleModel.FindAll();
                
                var ruleTemplates = objInstance.ruleTemplateModel.FindAll();
            
                objInstance.view.RulesEditor(rules, ruleTemplates);
            },
            
            DeleteRule: function(rule)
            /* Deletes a rule from the system and updates the UI. */
            {
                objInstance.ruleModel.Delete(rule);
                var rules = objInstance.ruleModel.FindAll();
            
                var ruleTemplates = objInstance.ruleTemplateModel.FindAll();
            
                objInstance.view.RulesEditor(rules, ruleTemplates);
            },
            
            SaveRules: function()
            /* Saves and serializes the rule builder data. 
               Does NOT update the UI.*/
            {
                var rules = objInstance.ruleModel.FindAll();
                var rulesById = {};
                for (var id in rules)
                {
                    if (rules.hasOwnProperty(id))
                    {
						var rule = rules[id];
                        rulesById[id] = 
                        {	// Ektron.RuleEditor.Rule
							id: rule.id,
							ruleTemplateID: rule.ruleTemplate.id,
							values: rule.values,
							parentID: rule.parentID
                        };
                    }
                }
                objInstance.view.SavedRules(rulesById);
            },
            
            SaveField: function(rule, fieldID, value)
            /* Saves the value for the field in a rule.
               Does NOT update the UI. */
            {
                objInstance.ruleModel.SaveField(rule, fieldID, value);
            }
        };
        
        objInstance.ruleTemplateModel = 
        /* Contains all methods for CRUD operations on rule templates.
        
           These methods should only be called by the CONTROLLER 
           object, and should NEVER be called directly by the VIEW 
           object. Instead, all information needed by the VIEW should 
           be passed to it the controller. */
        {
            /* Public Methods */
            FindByID: function(ruleTemplateID)
            /* Returns the rule template with ID ruleTemplateID. */
            {
                var ruleTemplate = this.p_ruleTemplates[ruleTemplateID];
                if (typeof(ruleTemplate) == "undefined")
                {
                    Ektron.OnException.diagException(new Error(Ektron.RuleEditor.Exceptions.InvalidRuleTemplate), arguments);
                    ruleTemplate = null;
                }
                
                return ruleTemplate;
            },
            
            FindAll: function()
            /* Returns all rule templates in the system. */
            {
                return this.p_ruleTemplates;
            },
            
            Create: function(ruleTemplate)
            {
                this.p_ruleTemplates[ruleTemplate.id] = ruleTemplate;
            },
            
            p_ruleTemplates: {}
        };
        
        objInstance.ruleModel = 
        /* Contains all methods for CRUD operations on rules.
        
           These methods should only be called by the CONTROLLER 
           object, and should NEVER be called directly by the VIEW 
           object. Instead, all information needed by the VIEW should 
           be passed to it the controller. */
        {
            /* Public Methods */
            
            Create: function(settings)
            /* Creates and returns a new rule.
            
               If the settings object specifies and id of a rule that 
               already exists, this rule will be overwritten. 
            */
            {
                var defaults = 
                {
                    childIDs: [],
                    parentID: objInstance.constants.InvalidRule, 
                    values: {}
                };
                
                if (typeof(settings.ruleTemplate) == "undefined")
                {
                    throw Ektron.RuleEditor.Exceptions.InvalidRuleTemplate;
                }
                
                var rule = {};
                $ektron.extend(true, rule, defaults, settings);
                
                if (typeof(rule.id) == "undefined")
                {
                    // Calculate the next valid id for an inserted rule
                    rule.id = this.p_NextID();
                }
                else
                {
                    this.p_SetID(rule.id);
                }
                
                // Set each field value to its default value, from the rules template,
                // if the value is not already set in the settings parameter.
                for (var fieldID in rule.ruleTemplate.fields)
                {
                    if ("undefined" == typeof(rule.values[fieldID]) && rule.ruleTemplate.fields.hasOwnProperty(fieldID))
                    {
						rule.values[fieldID] = rule.ruleTemplate.fields[fieldID].defaultValue;
                    }
                }
                
                // Store the new rule in our private data object
                this.p_data.rules[rule.id] = rule;
                
                // If there's a parent rule, add the child ID to the parent rule
                if (objInstance.helpers.IsValidRuleID(rule.parentID) == true)
                {
                    if (typeof(this.p_data.rules[rule.parentID]) == "undefined")
                    {
                        throw Ektron.RuleEditor.Exceptions.RuleNotFound;
                    }
                    this.p_data.rules[rule.parentID].childIDs.push(rule.id);
                }
                
                return rule;
            },
            
            Delete: function(rule)
            /* Deletes a rule */
            {
                // If the rule has a parent, then remove the current 
                // rule's id from the parent's child ID list
                var parentID = this.p_data.rules[rule.id].parentID;
                if (objInstance.helpers.IsValidRuleID(parentID))
                {
                    for (i = 0; i < this.p_data.rules[parentID].childIDs.length; i++)
                    {
                        if (this.p_data.rules[parentID].childIDs[i] == rule.id)
                        {
                            this.p_data.rules[parentID].childIDs.splice(i, 1);
                            break;
                        }
                    }
                }
                
                // Replace current rule with first child rule
                if (rule.childIDs.length > 0)
                {
                    this.p_data.rules[rule.id] = $ektron.extend(true, {}, this.p_data.rules[rule.childIDs[0]]);
                    this.p_data.rules[rule.id].id = rule.id;
                    this.p_data.rules[rule.id].parentID = rule.parentID;
                    this.p_data.rules[rule.id].childIDs = rule.childIDs.slice(0);
                    this.p_data.rules[rule.id].childIDs.splice(0, 1);
                    
                    delete this.p_data.rules[rule.childIDs[0]];
                }
                else
                {
                    // Delete the rule
                    delete this.p_data.rules[rule.id];
                }
            },
            
            DeleteByID: function(id)
            /* Deletes the rule with the specified identifier */
            {
                var rule = this.FindByID(id);
                this.Delete(rule);
            },
            
            FindByID: function(id)
            /* Finds and returns the rule with the specified identifier */
            {
                return this.p_data.rules[id];
            },
            
            FindAll: function()
            /* Returns all rules */
            {
                return this.p_data.rules;
            },
            
            SaveField: function(rule, fieldID, value)
            /* Saves a field */
            {
                rule.values[fieldID] = value;
            },
            
            /* Private Methods */
            
            p_NextID: function()
            /* Returns the next unique ID 
               (Note: This function should be used to create ID's for 
                      new rules) */
            {
                return (++this.p_minID);
            },
            
            p_SetID: function(id)
            /* Sets the next unique ID 
               (Note: This function should be used when inserting new items
                      into the rule data) */
            {
                this.p_minID = Math.max(this.p_minID, id);
            },
            
            /* Private Properties */
            
            p_minID: 0,
            
            /* Note: Perhaps this can be pulled out if it only has one 
                     property */
            p_data:
            {
                rules: {}
            }
        };
        
        objInstance.renderer = 
        /* Object for rendering elements.
        
           All methods in here should take '$container', a jQuery 
           object, as the first argument, and should perform all 
           rendering INSIDE of the container.
           
           The purpose of this object is to abstract the rendering 
           of each visual $container (i.e. a rule, a field, a list of 
           rules, etc...). These methods should only be called from 
           within the VIEW object, and should be used for CREATING 
           DOM elements.
        */
        {
            FormatTemplateText: function(text)
            /* Replaces the {CssClass} directives in the text with 
               their corresponding css class from 
               Ektron.RuleEditor.CssClasses. */
            {
                return text.replace(/\{([^\}]+)\}/g, function($0_match, $1_key)
                {
                    return Ektron.RuleEditor.CssClasses[$1_key];
                });
            }, 
            
            fields: 
            /* Contains all of the functions used for rendering 
               fields */
            {
                String: function($container, field, value, callback)
                /* Adds String field DOM elements to the container */
                {
                    $container.html(value);
                    $container.editable(
						function(value, settings)
						{ 
							callback(value); 
							return value; 
						},
                        $ektron.extend(
                        {
							type: "text",
							tooltip: field.name.caption,
							placeholder: "(" + field.name.caption + ")"
						}, Ektron.RuleEditor.EditablePluginSettings));
                    return $container;
                },
                
                Enum: function($container, field, value, callback)
                /* Adds Enum field DOM elements to the container */
                {
                    var selectMap = {};

                    // Add all of the enumeration values to the select list map (for jEditable)
                    for (var i = 0; i < field.options.length; i++)
                    {
						var option = field.options[i];
                        selectMap[option.value] = option.caption;
                    }
                    
                    $container.html(selectMap[value]);
                    $container.editable(
						function(value, settings)
						{ 
							callback(value); 
							return selectMap[value]; 
						},
                        $ektron.extend(
                        {
							type: "select",
							data: Ektron.JSON.stringify(selectMap),
							tooltip: field.name.caption,
							placeholder: "(" + field.name.caption + ")"
						}, Ektron.RuleEditor.EditablePluginSettings));
                    return $container;
                },
                
                Bool: function($container, field, value, callback)
                {
                    var selectMap = 
                    {
						"false": "false", // TODO localize
						"true": "true"
                    };
                    
                    $container.html(selectMap[value]);
                    $container.editable(
						function(value, settings)
						{ 
							callback(value); 
							return selectMap[value]; 
						},
                        $ektron.extend(
                        {
							type: "select",
							data: Ektron.JSON.stringify(selectMap),
							tooltip: field.name.caption,
							placeholder: "(" + field.name.caption + ")"
						}, Ektron.RuleEditor.EditablePluginSettings));
                    return $container;
                },
                
                Numeric: function($container, field, value, callback)
                {
                    $container.html(value);
                    $container.editable(
						function(value, settings)
						{ 
							callback(value); 
							return value; 
						},
                        $ektron.extend(
                        {
							type: "text",
							tooltip: field.name.caption,
							placeholder: "(" + field.name.caption + ")"
						}, Ektron.RuleEditor.EditablePluginSettings));
                    return $container;
                },
                
                Date: function($container, field, value, callback)
                {
                    $container.html(value);
                    $container.editable(
						function(value, settings)
						{ 
							callback(value); 
							return value; 
						},
                        $ektron.extend(
                        {
							type: "text",
							tooltip: field.name.caption,
							placeholder: "(" + field.name.caption + ")"
						}, Ektron.RuleEditor.EditablePluginSettings));
                    return $container;
                }
            },
            
            RenderField: function($container, field, value, callback)
            /* Adds field DOM elements to the container */
            {
                $container.addClass(field.type + "Type");
                
                return objInstance.renderer.fields[field.type]($container, field, value, callback);
            },
            
            RenderContainer: function($container, ruleTemplates)
            {
                objInstance.renderer.RenderAddButton(objInstance.components.addButtonList, 
                                              ruleTemplates, 
                                              objInstance.constants.InvalidRule);
            },
            
            RenderAddButton: function($container, ruleTemplates, parentRuleID)
            {
                // Render the add buttons for each rule template
                var $ul = $ektron(Ektron.RuleEditor.HTMLTemplates.AddRuleMenu);
                $container.after($ul);

                for (var id in ruleTemplates)
                {
                    if (ruleTemplates.hasOwnProperty(id) && ruleTemplates[id].enabled)
                    {
                        var $addButtonListItem = $ektron(this.FormatTemplateText(Ektron.RuleEditor.HTMLTemplates.AddRuleButton));
                        var $addButtonAnchor = $addButtonListItem.children("a");
                        $addButtonAnchor.html(ruleTemplates[id].title);
                        $addButtonAnchor.click((function(ruleTemplate){return function(evt)
                        {
                            // Protect from update panels mucking with my link
                            evt.preventDefault();
                            objInstance.controller.AddRule(ruleTemplate.id, parentRuleID);
                        };})(ruleTemplates[id]));
                        
                        $addButtonListItem.hover(Ektron.RuleEditor.hoverIn, Ektron.RuleEditor.hoverOut);
                        
						if (ruleTemplates[id].group)
						{
							var $ul2 = $ul.find("ul." + ruleTemplates[id].group.value);
							if (0 == $ul2.length)
							{
								$ul2 = $ektron(Ektron.RuleEditor.HTMLTemplates.AddRuleMenu);
								$ul2.addClass(ruleTemplates[id].group.value);
								var $submenu = $ektron(Ektron.RuleEditor.HTMLTemplates.AddRuleSubmenu);
								var $submenuAnchor = $submenu.children("a");
								$submenuAnchor.html(ruleTemplates[id].group.caption);
								$submenu.append($ul2);
								$ul.append($submenu);
							}
							$ul2.append($addButtonListItem);
						}
						else
						{
							$ul.append($addButtonListItem);
						}
                    }
                }
                
				$ul.dropdown({button:$container});
                
                return $container;
            },
            
            RenderTopLevelRules: function($container, rules, ruleTemplates)
            {
                var isFirstRule = true;
                // Render all of the rules
                for (var i in rules)
                {
                    if (rules.hasOwnProperty(i))
                    {
                        // Skip rules with parents 
                        // (these are rendered in renderer.Rule)
                        if (objInstance.helpers.IsValidRuleID(rules[i].parentID) == true)
                        {
                            continue;
                        }
                        
                        var $fieldset = $ektron(
                            '<fieldset class="ui-widget-content"><legend>' + 
                            (isFirstRule ? Ektron.RuleEditor.ResourceText.sCaptionIf : Ektron.RuleEditor.ResourceText.sCaptionOr) +
                            '</legend></fieldset>');
                            
                        $container.append($fieldset);
                        
                        // Render the rule!
                        if (rules[i].childIDs.length > 0)
                        {
                            objInstance.renderer.RenderRule($fieldset, rules[i], ruleTemplates);
                        }
                        // if only one rule, pre-generate the list in which to store it
                        else
                        {
                            var $ul = $ektron("<ul><li></li></ul>");
                            var $li = $ul.find("li");
                            objInstance.renderer.RenderRule($li, rules[i], ruleTemplates);
                            $fieldset.append($ul);
                        }
                        
                        var $andButton = $ektron('<a href="#add" class="ek-button ek-button-icon-left ui-corner-all ui-state-default"><span class="ui-icon ui-icon-plus"></span>' + Ektron.RuleEditor.ResourceText.sButtonAnd + '</a>');
                        $andButton.hover(Ektron.RuleEditor.hoverIn, Ektron.RuleEditor.hoverOut);
                        $fieldset.append($andButton);
                                            
                        objInstance.renderer.RenderAddButton($andButton, ruleTemplates, rules[i].id);
                        
                        isFirstRule = false;
                    }
                }
                
                // if no rules were processed, print out a default state message
                if (isFirstRule === true)
                {
                    $container.html(Ektron.RuleEditor.ResourceText.sMsgNoRules);
                }
            },
            
            RenderRule: function($container, rule, ruleTemplates)
            {
                var $item = $ektron(this.FormatTemplateText(Ektron.RuleEditor.HTMLTemplates.Rule));
                
                // Register the Delete action
                var $itemDeleteButton = $item.find("."+Ektron.RuleEditor.CssClasses.DeleteRule);
                
                $itemDeleteButton.bind("click", 
                    { rule: rule }, 
                    function(event)
                    {
                        event.preventDefault();
                        objInstance.controller.DeleteRule(event.data.rule);
                    });
                
                $itemDeleteButton.addClass("ui-state-default ui-corner-all ui-button");
                $itemDeleteButton.hover(Ektron.RuleEditor.hoverIn, Ektron.RuleEditor.hoverOut);
                
                // Get the placeholder for the rule
                var $itemText = $item.find("."+Ektron.RuleEditor.CssClasses.RuleMessage);
                
                // Replace the {fieldID} directives with <span class="fieldID"></span>.
                // 
                // The reason for doing this is so that when the events are added later, 
                // they will not destroy the previously hooked up events when a string 
                // replace is used. 
                var fieldText = rule.ruleTemplate.text;
                for (var fieldID in rule.ruleTemplate.fields)
                {
                    if (rule.ruleTemplate.fields.hasOwnProperty(fieldID))
                    {
                        fieldText = fieldText.replace("{" + fieldID + "}", '<span class="field ' + fieldID + '"></span>');
                    }
                }
                
                // Add the new span tags to the field placeholder
                $itemText.html(fieldText);
                
                // Now loop through the newly created fields and initialize them 
                // in the placeholder.
                for (fieldID in rule.ruleTemplate.fields)
                {
                    if (rule.ruleTemplate.fields.hasOwnProperty(fieldID))
                    {
                        // Get the field information (type, values, etc...)
                        var field = rule.ruleTemplate.fields[fieldID];
                        
                        // Get the saved value
                        var value = rule.values[fieldID];
                        
                        var $fieldElement = $itemText.find("span." + fieldID);
                        
                        // Render the field
                        objInstance.renderer.RenderField($fieldElement, field, value, 
							(function(fieldID)
                            {
                                return function(value)
                                {
                                    objInstance.controller.SaveField(rule, fieldID, value);
                                };
                            })(fieldID));
                    }
                }
                
                // Get the list of rules and render them               
                if (rule.childIDs.length > 0)
                {
                    var $itemRulesList = $ektron('<ul></ul>');
                    var $li = $ektron('<li></li>');
                    $li.append($item);
                    $itemRulesList.append($li);
                    for (var i = 0; i < rule.childIDs.length; i++)
                    {
                        var $innerLi = $ektron('<li></li>');
                        objInstance.renderer.RenderRule($innerLi, objInstance.ruleModel.FindByID(rule.childIDs[i]), ruleTemplates);
                        $itemRulesList.append($innerLi);
                    }
                    $container.append($itemRulesList);
                }
                else
                {
                    $container.append($item);
                }
            }
        };
        
        objInstance.view = 
        /* Contains all functions for DISPLAYING (updating the DOM), 
           and hooking up events.
        
           These methods should only be called from the CONTROLLER. 
           DO NOT CALL A MODEL FROM THE VIEW... EVER. 
           */
        {
            /* Constructor */
            Init: function(ruleTemplates)
            {
                // Render the add buttons for each rule template
                objInstance.renderer.RenderContainer(objInstance.$container, ruleTemplates);
            },

            RulesEditor: function(rules, ruleTemplates)
            /* Displays the rule editor data */
            {
                objInstance.components.rulesList.html("");
                objInstance.renderer.RenderTopLevelRules(objInstance.components.rulesList, rules, ruleTemplates);
            },
            
            SavedRules: function(rulesById)
            {
                var strRulesById = Ektron.JSON.stringify(rulesById);
				var savedRulesData = objInstance.$container.find("."+Ektron.RuleEditor.CssClasses.SavedRulesData);
				savedRulesData.val(strRulesById);
				$ektron('.jsonToSave').val(strRulesById);
            }
        };
        
        if (objInstance.components.addButtonList.length !== 1)
        {
            throw Ektron.RuleEditor.Exceptions.MissingAddButtonList;
        }
        if (objInstance.components.rulesList.length !== 1)
        {
            throw Ektron.RuleEditor.Exceptions.MissingRulesList;
        }
        
        objInstance.components.addButtonList.hover(Ektron.RuleEditor.hoverIn, Ektron.RuleEditor.hoverOut);

        Ektron.RuleEditor.RuleTemplates = {};
        for (var template in ruleTemplates)
        {
            if (ruleTemplates.hasOwnProperty(template))
            {
                objInstance.ruleTemplateModel.Create(ruleTemplates[template]);
            }
        }
        
        objInstance.controller.Init();
        
        objInstance.controller.LoadRules(initialRules);
        
        return objInstance;
    },
    
    save: function(id)
    {
		var objInstance = Ektron.RuleEditor.instances[id];
		if (objInstance)
		{
		    if (this.validateRuleName(objInstance.$container))
		    {
			    objInstance.controller.SaveRules();
			}
			else 
			{
			    return false;
			}
		}
    },
    
    cancel: function(e)
    {
        location.href = 'TargetContentEdit.Aspx?PageId=-100'; 
        e.returnValue = false; 
        e.cancel = true;
        return false;
    },
    validateRuleName: function($container)
    {
        var tbRuleName = $container.closest("div.targetedContent-rule-editor").siblings("input[id$='_tbRulesetName']");
        if (1 == tbRuleName.length && "" == $ektron.trim(tbRuleName.val()))
	    {
	        alert("A Name is required.");
	        return false;
	    }
		return true;
    },
    
    hoverIn: function() { $ektron(this).addClass("ui-state-hover"); },
    hoverOut: function() { $ektron(this).removeClass("ui-state-hover"); }
};