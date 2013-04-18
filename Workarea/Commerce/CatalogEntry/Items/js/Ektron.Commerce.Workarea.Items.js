//Define Ektron object only if it's not already defined
if (Ektron === undefined) {
	Ektron = {};
}

//Define Ektron.Commerce object only if it's not already defined
if (Ektron.Commerce === undefined) {
	Ektron.Commerce = {};
}

//Define Ektron.Commerce object only if it's not already defined
if (Ektron.Commerce.CatalogEntry === undefined) {
	Ektron.Commerce.CatalogEntry = {};
}

//Ektron Catalog Entry Items Object
Ektron.Commerce.CatalogEntry.Items = {
	//Generic function that checks if the string is empty.
	isEmpty: function (strData) {
		return ((strData == null) || (strData.length == 0));
	},
	//Generic function that checks if the string entered is just white space.
	isWhitespace: function (strData) {
		var whitespace = " \t\n\r";
		var iCtr,
			cTemp;

		if (Ektron.Commerce.CatalogEntry.Items.isEmpty(strData))
			return true;

		for (iCtr = 0; iCtr < strData.length; iCtr++) {
			var cTemp = strData.charAt(iCtr);
			if (whitespace.indexOf(cTemp) == -1)
				return false;
		}

		return true;
	},
	DefaultView: {
		Add: {
			addItem: function (id, title) {
				if (title == $ektron("#content_title")[0].value) {
					alert("Please Note: You can not select the current entry as one of the item.");
					return false;
				}
				var itemsList = $ektron("div.EktronCommerceItems ul");
				var newItem = itemsList.children("li.cloneItem").clone();

				//create new row by cloning placeholder row
				newItem.removeAttr("id").removeClass("cloneItem").removeClass("hide").addClass("defaultViewItem");

				//add id text and input value
				var spanId = newItem.find("span.id");
				var inputId = spanId.children("input.id");
				inputId.attr("value", id);
				spanId.html(String(id) + spanId.html());

				//add title text and input value
				var spanTitle = newItem.find("span.title");
				var inputTitle = spanTitle.children("input.title");
				inputTitle.attr("value", String(title));
				spanTitle.html(String(title) + spanTitle.html());

				//ensure marked for delete input is set to false
				newItem.find("span.delete input.markedForDelete").attr("value", false);

				//set empty item row to hidden (since we now have an item)
				var emptyItem = itemsList.children("li.emptyItem");
				if (!emptyItem.hasClass("hide")) {
					emptyItem.addClass("hide");
				}

				var existIds = $ektron("ul.ui-sortable").find("li.defaultViewItem").find("span.id").find("input.id");
				var i = 0;
				var count = 0;
				if (existIds.length > 0) {
					for (i = 0; i < existIds.length; i++) {
						var addId = existIds[i].value;
						if (addId == id) {
							count++;
						}
					}
					if (count > 0) {
						alert("There is already an item by this title or id. \n\nPlease select other item.");
						return false;
					}
					else {
						//add new row to table
						itemsList.append(newItem);
					}
				}
				else {
					//add new row to table
					itemsList.append(newItem);
				}


				//re-initialize Items sort
				Ektron.Commerce.CatalogEntry.Items.DefaultView.Sort.bind();

				//re-stripe rows
				Ektron.Commerce.CatalogEntry.Items.DefaultView.stripe();

				//scroll to top if ie6
				if ($ektron.browser.msie && $ektron.browser.version < 7)
					$ektron('html, body').animate({ scrollTop: 0 }, 'slow');

				//update Json data
				Ektron.Commerce.CatalogEntry.Items.DefaultView.Data.update();

				//inform Pricing tab that product now has an item
				//when a product becomes a complex product (when an item is added),
				//Pricing Tiers must be hidden
				if (Ektron.Commerce.Pricing != undefined) {
					Ektron.Commerce.Pricing.Tier.itemAdded();
				}
			},
			viewItems: function () {
				$ektron('#ItemsModal').modalShow();
			}
		},
		Data: {
			init: function () {
				//initialize Json data as array
				Ektron.Commerce.CatalogEntry.Items.data = new Array();

				//populate Json data
				$ektron("div.EktronCommerceItems ul li.defaultViewItem").each(function (i) {

					//retrieve json values
					var itemId = $ektron(this).find("span.id input.id").attr("value");
					var itemTitle = $ektron(this).find("span.title input.title").attr("value");
					var itemMarkedForDelete = $ektron(this).find("span.delete input.markedForDelete").attr("value");

					//create json object
					var ItemData = {
						Order: i,
						Id: itemId,
						Title: itemTitle,
						MarkedForDelete: itemMarkedForDelete
					}

					//populate json data array with item values
					Ektron.Commerce.CatalogEntry.Items.data.push(ItemData);
				});

				//populate items input field with stringified json data
				$ektron("div.EktronCommerceItems input.itemData").attr("value", Ektron.JSON.stringify(Ektron.Commerce.CatalogEntry.Items.data));

				//alert($ektron("div.EktronCommerceItems input.itemData").attr("value"));
			},
			update: function () {
				//re-initialize json data array
				Ektron.Commerce.CatalogEntry.Items.DefaultView.Data.init();
			}
		},
		init: function () {
			if (Ektron.Commerce.CatalogEntry.Items.editable == "true") {
				//initialize json data
				Ektron.Commerce.CatalogEntry.Items.DefaultView.Data.init();

				//initialize Items sort
				Ektron.Commerce.CatalogEntry.Items.DefaultView.Sort.bind();
			}

			//stripe rows
			Ektron.Commerce.CatalogEntry.Items.DefaultView.stripe();
		},
		markForDelete: function (elem) {
			var link = $ektron(elem);
			var item = link.parents("li.defaultViewItem");

			//update UI
			if (link.hasClass("undo")) {
				//set input markedForDelete to FALSE
				item.find("span.delete input.markedForDelete").attr("value", false);

				//update UI
				link.removeClass("undo");
				link.attr("title", "Mark Item For Delete");
				item.find("span.id").css("color", "").css("text-decoration", "none");
				item.find("span.title").css("color", "").css("text-decoration", "none");
			}
			else {
				//set input markedForDelete to TRUE
				item.find("span.delete input.markedForDelete").attr("value", true);

				//update UI
				link.addClass("undo");
				link.attr("title", "Undo Mark Item For Delete");
				item.find("span.id").css("text-decoration", "line-through").css("color", "silver");
				item.find("span.title").css("text-decoration", "line-through").css("color", "silver");
			}

			//update json data array
			Ektron.Commerce.CatalogEntry.Items.DefaultView.Data.update();
		},
		Sort: {
			bind: function () {
				if ($ektron("div.EktronCommerceItems ul").is("ui-sortable")) {
					$ektron("div.EktronCommerceItems ul").sortable("destroy");
				}
				$ektron("div.EktronCommerceItems ul").sortable({
					items: "li.defaultViewItem",
					opacity: .8,
					revert: false,
					zIndex: 99999,
					scroll: true,
					cursor: "move",
					containment: $ektron("div.EktronCommerceItems div.itemWrapper"),
					placeholder: "placeholder",
					helper: function (e, el) {
						var helper = $ektron(el).clone();
						helper.css("width", $ektron(el).width());
						helper.prependTo("div.EktronCommerceItems div.itemWrapper ul");
						return helper;
					},
					stop: function (e, ui) {
						//re-stripe rows
						Ektron.Commerce.CatalogEntry.Items.DefaultView.stripe();

						//update json data array
						Ektron.Commerce.CatalogEntry.Items.DefaultView.Data.update();
					}
				});
			}
		},
		stripe: function () {
			$ektron("div.EktronCommerceItems ul.defaultViewItems li.defaultViewItem:odd").removeClass("stripe");
			$ektron("div.EktronCommerceItems ul.defaultViewItems li.defaultViewItem:even").addClass("stripe");
		}
	},
	data: {},
	editable: "false",
	escapeAndEncode: function (string) {
		return string
			.replace(/&/g, "&amp;")
			.replace(/</g, "&lt;")
			.replace(/>/g, "&gt;")
			.replace(/'/g, "\'")
			.replace(/\"/g, "\"")
	},
	init: function () {
		//retrieve editable value from hidden field
		Ektron.Commerce.CatalogEntry.Items.editable = $ektron("div.EktronCommerceItems input.itemEditable").val();

		//initialize items modal if editable is true
		if (Ektron.Commerce.CatalogEntry.Items.editable == "true") {
			Ektron.Commerce.CatalogEntry.Items.Modal.init();
		}

		//retrieve view value from hidden field
		Ektron.Commerce.CatalogEntry.Items.view = $ektron("div.EktronCommerceItems input.itemView").val();

		//set json mode hidden field
		$ektron("div.EktronCommerceItems input#ItemDataDeserializationMode").val($ektron("div.EktronCommerceItems input.jsonMode").val());

		//initialize view
		switch (Ektron.Commerce.CatalogEntry.Items.view) {
			case "kit":
				Ektron.Commerce.CatalogEntry.Items.KitView.init();
				break;
			case "default":
				Ektron.Commerce.CatalogEntry.Items.DefaultView.init();
				break;
		}
	},
	KitView: {
		bindEvents: function () {
			//ensure numerics only in dollar edit field & two digits only in cents feild
			$ektron("div.EktronCommerceItems input.itemPriceModifierDollarsEdit, div.EktronCommerceItems input.itemPriceModifierCentsEdit, div.itemsTabModal input.addItemPriceAmountDollarsEdit, div.itemsTabModal input.addItemPriceAmountCentsEdit")
				.bind("keypress", function (evt) {
					var evt = (evt) ? evt : window.event;
					var charCode = (evt.which != null) ? evt.which : evt.keyCode;
					if ($ektron(evt.target).hasClass("itemPriceModifierDollarsEdit") || $ektron(evt.target).hasClass("addItemPriceAmountDollarsEdit")) {
						return (charCode < 32 || (charCode >= 48 && charCode <= 57))
					} else {
						var digits = $ektron(evt.target).val();
						if (digits.length < 2 || charCode == 8) {
							if (charCode < 32 || (charCode >= 48 && charCode <= 57)) {
								return charCode;
							} else {
								return false;
							}
						} else {
							return false;
						}
					}
				});
			//ensure groupname (add group mode) does not have spaces or other invalid characters
			$ektron("div.addKitNode input.addGroupName")
				.bind("keydown", function (evt) {
					var evt = (evt) ? evt : window.event;
					var charCode = (evt.which != null) ? evt.which : evt.keyCode;
					return (
								(charCode != 16) &&
								(charCode == 32) ||
								(charCode == 8) ||
								(charCode >= 48 && charCode <= 57) ||
								(charCode >= 65 && charCode <= 90)
							)
				});
			//ensure groupname (edit group mode) does not have spaces or other invalid characters
			$ektron("div.groupDetails input.groupNameEdit")
				.bind("keydown", function (evt) {
					var evt = (evt) ? evt : window.event;
					var charCode = (evt.which != null) ? evt.which : evt.keyCode;
					return (
								(charCode != 16) &&
								(charCode == 32) ||
								(charCode == 8) ||
								(charCode >= 48 && charCode <= 57) ||
								(charCode >= 65 && charCode <= 90)
							)
				});


			//if editable make kit group list and kit item list sortable.
			if (Ektron.Commerce.CatalogEntry.Items.editable == "true") {
				Ektron.Commerce.CatalogEntry.Items.KitView.bindSortable();
			}
		},
		bindSortable: function () {
			$ektron("div.kitView ul.kitGroups li.kitGroup").addClass("kitGroupEditable");
			if ($ektron("div.kitView ul.kitGroups").is("ui-sortable")) {
				$ektron("div.kitView ul.kitGroups").sortable("destroy");
			}
			$ektron("div.kitView ul.kitGroups").sortable({
				items: "li.kitGroup",
				opacity: .8,
				revert: false,
				zIndex: 99999,
				scroll: true,
				cursor: "move",
				placeholder: "placeholder",
				helper: function (e, el) {
					var helper = $ektron(el).clone();
					helper.css("width", $ektron(el).width());
					helper.prependTo("div.kitView");
					return helper;
				},
				stop: function (e, ui) {
					//update json data array
					Ektron.Commerce.CatalogEntry.Items.KitView.Data.update();
				}
			});

			$ektron("div.kitView ul.kitItems li.kitItem").addClass("kitItemEditable");
			if ($ektron("div.kitView ul.kitItems").is("ui-sortable")) {
				$ektron("div.kitView ul.kitItems").sortable("destroy");
			}
			$ektron("div.kitView ul.kitItems").sortable({
						items: "li.kitItem",
						opacity: .8,
						revert: false,
						zIndex: 99999,
						scroll: true,
						cursor: "move",
						placeholder: "placeholder",
						helper: function (e, el) {
							var helper = $ektron(el).clone();
							helper.css("width", $ektron(el).width());
							helper.prependTo("div.kitView");
							return helper;
						},
						stop: function (e, ui) {
							//update json data array
							Ektron.Commerce.CatalogEntry.Items.KitView.Data.update();
						}
					});
		},
		Data: {
			init: function () {
				//initialize Json data as array
				Ektron.Commerce.CatalogEntry.Items.data = new Array();

				//populate json data
				$ektron("div.EktronCommerceItems ul.kitGroups li.kitGroup").each(function (i) {

					//retrieve json values
					var groupId = $ektron(this).children("input.id").attr("value");
					var groupName = $ektron(this).children("input.name").attr("value");
					var groupDescription = $ektron(this).children("input.description").attr("value");
					var groupMarkedForDelete = $ektron(this).children("input.markedForDelete").attr("value");

					//create json object
					var GroupData = {
						Order: i,
						Id: groupId,
						Title: groupName,
						Description: groupDescription,
						MarkedForDelete: groupMarkedForDelete,
						Items: {}
					}

					GroupData.Items = new Array();

					$ektron(this).find("li.kitItem").each(function (x) {
						var itemId = $ektron(this).children("input.id").attr("value");
						var itemName = $ektron(this).children("input.name").attr("value");
						var itemExtraText = $ektron(this).children("input.extraText").attr("value");
						var itemPriceModifierPlusMinus = $ektron(this).children("input.priceModifierPlusMinus").attr("value");
						var itemPriceModifierDollars = $ektron(this).children("input.priceModifierDollars").attr("value");
						var itemPriceModifierCents = $ektron(this).children("input.priceModifierCents").attr("value");
						var itemMarkedForDelete = $ektron(this).children("input.markedForDelete").attr("value");

						var Item = {
							Order: x,
							Id: itemId,
							Title: itemName,
							ExtraText: itemExtraText,
							PriceModifierPlusMinus: itemPriceModifierPlusMinus,
							PriceModifierDollars: itemPriceModifierDollars,
							PriceModifierCents: itemPriceModifierCents,
							MarkedForDelete: itemMarkedForDelete
						}
						GroupData.Items.push(Item);
					});

					//populate json data array with item values
					Ektron.Commerce.CatalogEntry.Items.data.push(GroupData);

					//update the group name under Kit Components section
					var kitGroupName = $ektron("li.kitGroup");
					if (kitGroupName.length > 0 && typeof (kitGroupName.find("span.kitGroupLabel")[i]) != "undefined") {
						kitGroupName.find("span.kitGroupLabel")[i].innerHTML = Ektron.Commerce.CatalogEntry.Items.data[i].Title;
					}
				});

				//populate items input field with stringified json data
				$ektron("div.EktronCommerceItems input.itemData").attr("value", Ektron.JSON.stringify(Ektron.Commerce.CatalogEntry.Items.data));

				//alert($ektron("div.EktronCommerceItems input.itemData").attr("value"));
			},
			update: function () {
				//re-initialize json data array
				Ektron.Commerce.CatalogEntry.Items.KitView.Data.init();
			}
		},
		Display: {
			Buttons: {
				cancel: function (el) {
					//show group/item in view mode
					Ektron.Commerce.CatalogEntry.Items.KitView.Display.show(el, 'button', 'view');
				},
				getButtonDisplayMode: function (mode, markedForDelete) {
					//get selected node
					var selectedNode = $ektron("div.EktronCommerceItems div.kitView td.list li.selectedItem");

					//get markedForDelete
					markedForDelete = selectedNode.children("input.markedForDelete").val();

					var buttonDisplayModes;
					switch (mode) {
						case "view":
							buttonDisplayModes = {
								edit: markedForDelete == "true" ? "none" : "block",
								markedForDelete: markedForDelete == "true" ? "none" : "block",
								restore: markedForDelete == "true" ? "block" : "none"
							}
							break;
						case "edit":
							buttonDisplayModes = {
								ok: "block",
								cancel: "block"
							}
							break;
					}
					return buttonDisplayModes;
				},
				ok: function (el) {
					//get selected node
					var selectedNode = $ektron("div.EktronCommerceItems div.kitView td.list li.selectedItem");

					//get mode
					var mode = selectedNode.hasClass("kitGroup") == true ? "group" : "item"; ;

					//get detail wrapper
					var wrapperClass = mode == "group" ? ".groupDetails" : ".itemDetails";
					var wrapper = $ektron(wrapperClass);

					//ensure name field is not blank - if so alert the user and abort update
					var nameField = wrapper.find("input.nameField");
					var nameFieldValue = nameField.val();
					if (nameFieldValue.length == 0 || Ektron.Commerce.CatalogEntry.Items.isWhitespace(nameFieldValue)) {
						//name has no value - alert user
						alert($ektron("div.EktronCommerceItems input.blankNameFieldError").val());
					} else {
						//update group or item hidden fields 
						switch (mode) {
							case "group":
								var name = Ektron.Commerce.CatalogEntry.Items.escapeAndEncode(wrapper.find("input.groupNameEdit").val());
								var description = Ektron.Commerce.CatalogEntry.Items.escapeAndEncode(wrapper.find("textarea.groupDescriptionEdit").val());
								selectedNode.children("input.name").val(name);
								selectedNode.children("input.description").val(description);
								break;
							case "item":
								var name = Ektron.Commerce.CatalogEntry.Items.escapeAndEncode(wrapper.find("input.itemNameEdit").val());
								var extraText = Ektron.Commerce.CatalogEntry.Items.escapeAndEncode(wrapper.find("textarea.itemExtraTextEdit").val());
								var priceModifierAddSubtract = wrapper.find("select.itemPriceModifierAddSubtractEdit option:selected").val();
								var dollars = Ektron.Commerce.CatalogEntry.Items.escapeAndEncode(wrapper.find("input.itemPriceModifierDollarsEdit").val());
								var cents = Ektron.Commerce.CatalogEntry.Items.escapeAndEncode(wrapper.find("input.itemPriceModifierCentsEdit").val());

								//ensure dollars is not blank
								if (dollars.length == 0) {
									dollars = "0";
								}

								//ensure cents is not blank and is two digits
								if (cents.length < 2) {
									if (cents.length == 0) {
										cents = "00";
									} else {
										cents = cents + "0";
									}
								}

								selectedNode.children("input.name").val(name);
								selectedNode.children("input.extraText").val(extraText);
								selectedNode.children("input.priceModifierPlusMinus").val(priceModifierAddSubtract);
								selectedNode.children("input.priceModifierDollars").val(dollars);
								selectedNode.children("input.priceModifierCents").val(cents);
								break;
						}

						//update data values
						Ektron.Commerce.CatalogEntry.Items.KitView.Data.update();

						//show group/item in view mode
						Ektron.Commerce.CatalogEntry.Items.KitView.Display.show(el, 'button', 'view');
					}
				},
				markForDelete: function (el) {
					//get selected node
					var selectedNode = $ektron("div.EktronCommerceItems div.kitView td.list li.selectedItem");

					//get mode
					var mode = selectedNode.hasClass("kitGroup") == true ? "group" : "item"; ;

					//get detail wrapper
					var wrapperClass = mode == "group" ? ".groupDetails" : ".itemDetails";
					var wrapper = $ektron(wrapperClass);

					//update group or item hidden fields 
					switch (mode) {
						case "group":
							//mark node for delete
							selectedNode.children("input.markedForDelete").val("true");
							selectedNode.addClass("markedForDelete");
							selectedNode.find("li.kitItem").each(function (i) {
								$ektron(this).addClass("markedForDelete");
								$ektron(this).children("input.markedForDelete").val("true");
							});
							selectedNode.find("li.addKitItem").css("display", "none");
							break;
						case "item":
							//mark node for delete
							selectedNode.children("input.markedForDelete").val("true");
							selectedNode.addClass("markedForDelete");
							break;
					}

					//update data values
					Ektron.Commerce.CatalogEntry.Items.KitView.Data.update();

					//show group/item in view mode
					Ektron.Commerce.CatalogEntry.Items.KitView.Display.show(el, 'button', 'view');
				},
				restore: function (el) {
					//get selected node
					var selectedNode = $ektron("div.EktronCommerceItems div.kitView td.list li.selectedItem");

					//get mode
					var mode = selectedNode.hasClass("kitGroup") == true ? "group" : "item"; ;

					//get detail wrapper
					var wrapperClass = mode == "group" ? ".groupDetails" : ".itemDetails";
					var wrapper = $ektron(wrapperClass);

					//update group or item hidden fields 
					switch (mode) {
						case "group":
							//mark node for delete
							selectedNode.children("input.markedForDelete").val("false");
							selectedNode.removeClass("markedForDelete");
							selectedNode.find("li.kitItem").each(function (i) {
								$ektron(this).removeClass("markedForDelete");
								$ektron(this).children("input.markedForDelete").val("false");
							});
							selectedNode.find("li.addKitItem").css("display", "block");
							break;
						case "item":
							//mark node for delete
							selectedNode.children("input.markedForDelete").val("false");
							selectedNode.removeClass("markedForDelete");
							selectedNode.parent().parent().children("input.markedForDelete").val("false");
							selectedNode.parent().parent().removeClass("markedForDelete");
							selectedNode.parent().parent().find("li.addKitItem").css("display", "block");
							break;
					}

					//update data values
					Ektron.Commerce.CatalogEntry.Items.KitView.Data.update();

					//show group/item in view mode
					Ektron.Commerce.CatalogEntry.Items.KitView.Display.show(el, 'button', 'view');
				},
				show: function (buttonDisplayModes) {
					//set button display defaults to none
					var buttonDisplayModeDefaults = { ok: "none", cancel: "none", edit: "none", markedForDelete: "none", restore: "none" }

					//merge buttonDisplayModes with buttonDisplayModeDefaults
					buttonDisplayModes = $ektron.extend({}, buttonDisplayModeDefaults, buttonDisplayModes || {});

					//get item wrapper
					var buttonWrapper = $ektron("div.EktronCommerceItems div.kitView p.editButtons");

					//show buttons
					buttonWrapper
						.find("a.ok")
						.css("display", buttonDisplayModes.ok);
					buttonWrapper
						.find("a.cancel")
						.css("display", buttonDisplayModes.cancel);
					buttonWrapper
						.find("a.edit")
						.css("display", buttonDisplayModes.edit);
					buttonWrapper
						.find("a.markForDelete")
						.css("display", buttonDisplayModes.markedForDelete);
					buttonWrapper
						.find("a.restore")
						.css("display", buttonDisplayModes.restore);

					//show buttons
					buttonWrapper.css("display", "block");

				}
			},
			edit: function (displayType, editDisplayValues, editDisplayModes) {
				//get group or item edit mode defaults
				var editDisplayModeDefaults;
				editDisplayModeDefaults = displayType == "group" ? Ektron.Commerce.CatalogEntry.Items.KitView.Group.getDisplayModeDefaults() : {};
				editDisplayModeDefaults = displayType == "item" ? Ektron.Commerce.CatalogEntry.Items.KitView.Item.getDisplayModeDefaults() : editDisplayModeDefaults;

				//merge displayModes with displayModeDefaults
				editDisplayModes = $ektron.extend({}, editDisplayModeDefaults, editDisplayModes || {});

				//display group or item data
				switch (displayType) {
					case "group":
						Ektron.Commerce.CatalogEntry.Items.KitView.Group.setDisplayEdit(editDisplayValues, editDisplayModes);
						break;
					case "item":
						Ektron.Commerce.CatalogEntry.Items.KitView.Item.setDisplayEdit(editDisplayValues, editDisplayModes);
						break;
				}
			},
			view: function (displayType, viewDisplayValues, viewDisplayModes) {
				//set edit field display defaults to none
				var viewDisplayModeDefaults;
				viewDisplayModeDefaults = displayType == "group" ? Ektron.Commerce.CatalogEntry.Items.KitView.Group.getDisplayModeDefaults() : {};
				viewDisplayModeDefaults = displayType == "item" ? Ektron.Commerce.CatalogEntry.Items.KitView.Item.getDisplayModeDefaults() : viewDisplayModeDefaults;

				//merge displayModes with displayModeDefaults
				viewDisplayModes = $ektron.extend({}, viewDisplayModeDefaults, viewDisplayModes || {});

				//display group or item data
				switch (displayType) {
					case "group":
						Ektron.Commerce.CatalogEntry.Items.KitView.Group.setDisplayView(viewDisplayValues, viewDisplayModes);
						break;
					case "item":
						Ektron.Commerce.CatalogEntry.Items.KitView.Item.setDisplayView(viewDisplayValues, viewDisplayModes);
						break;
				}
			},
			show: function (el, source, mode) {
				//get selected item - determine show type (list item click, or button click)
				var selectedItem;
				switch (source) {
					case "list":
						//get selected item
						selectedItem = $ektron(el).parent();
						//unmark any previously selected item
						$ektron("div.EktronCommerceItems div.kitView td.list .selectedItem").removeClass("selectedItem");
						//mark selected item
						selectedItem.addClass("selectedItem");
						break;
					case "button":
						selectedItem = $ektron("div.EktronCommerceItems div.kitView td.list .selectedItem");
						break;
				}

				//get kit or item display values
				var displayValues;
				var displayType;
				var viewDisplayModes;
				var editDisplayModes;

				displayType = selectedItem.hasClass("kitGroup") == true ? "group" : "item";
				switch (displayType) {
					case "group":
						displayValues = Ektron.Commerce.CatalogEntry.Items.KitView.Group.getDisplayValues(selectedItem);
						viewDisplayModes = mode == "view" ? Ektron.Commerce.CatalogEntry.Items.KitView.Group.getDisplayModes() : {};
						editDisplayModes = mode == "edit" ? Ektron.Commerce.CatalogEntry.Items.KitView.Group.getDisplayModes() : {};
						break;
					case "item":
						displayValues = Ektron.Commerce.CatalogEntry.Items.KitView.Item.getDisplayValues(selectedItem);
						viewDisplayModes = mode == "view" ? Ektron.Commerce.CatalogEntry.Items.KitView.Item.getDisplayModes() : {};
						editDisplayModes = mode == "edit" ? Ektron.Commerce.CatalogEntry.Items.KitView.Item.getDisplayModes() : {};
						break;
				}

				//set view labels 
				Ektron.Commerce.CatalogEntry.Items.KitView.Display.view(displayType, displayValues, viewDisplayModes);
				//set edit fields
				Ektron.Commerce.CatalogEntry.Items.KitView.Display.edit(displayType, displayValues, editDisplayModes);

				//set buttons
				var buttonDisplayModes = Ektron.Commerce.CatalogEntry.Items.KitView.Display.Buttons.getButtonDisplayMode(mode);
				Ektron.Commerce.CatalogEntry.Items.KitView.Display.Buttons.show(buttonDisplayModes);

				//show group or item details
				$ektron("div.EktronCommerceItems div.kitView div.itemDetails").css("display", displayType == "item" ? "block" : "none");
				$ektron("div.EktronCommerceItems div.kitView div.groupDetails").css("display", displayType == "group" ? "block" : "none");
			}
		},
		init: function () {
			//bind events
			Ektron.Commerce.CatalogEntry.Items.KitView.bindEvents();

			//initialize group treeview
			Ektron.Commerce.CatalogEntry.Items.KitView.Treeview.init();

			//intialize json data
			Ektron.Commerce.CatalogEntry.Items.KitView.Data.init();
		},
		Item: {
			Add: {
				addItem: function (el) {
					//get item values from modal
					var addItemsModal = $ektron("#ItemsModal div.addKitItem");

					//get new item values
					var name = Ektron.Commerce.CatalogEntry.Items.escapeAndEncode(addItemsModal.find("input.addItemName").val());
					var extraText = Ektron.Commerce.CatalogEntry.Items.escapeAndEncode(addItemsModal.find("textarea.addItemExtraText").val());
					var priceModifierAddSubtract = addItemsModal.find("select.addItemPriceModifierAddSubtractEdit option:selected").val();
					var dollars = addItemsModal.find("input.addItemPriceAmountDollarsEdit").val();
					var cents = addItemsModal.find("input.addItemPriceAmountCentsEdit").val();

					//ensure dollars is not blank
					if (dollars.length == 0) {
						dollars = "0";
					}

					//ensure cents is not blank and is two digits
					if (cents.length < 2) {
						if (cents.length == 0) {
							cents = "00";
						} else {
							cents = cents + "0";
						}
					}


					//get kit group li
					var kitGroup = $ektron(".addItemMarker");

					//get marked kit group parent ul
					var kitGroups = kitGroup.parent();

					//get kit item clone
					var itemClone = kitGroups.find("li.kitGroupClone ul.kitItems li.kitItemClone").clone(true);

					//remove clone class and add item class
					itemClone
						.removeClass("kitItemClone")
						.addClass("kitItem");

					//set new item values
					itemClone
						.find("input.name")
						.val(name);
					itemClone
						.find("input.extraText")
						.val(extraText);
					itemClone
						.find("input.priceModifierPlusMinus")
						.val(priceModifierAddSubtract);
					itemClone
						.find("input.priceModifierDollars")
						.val(dollars);
					itemClone
						.find("input.priceModifierCents")
						.val(cents);
					itemClone
						.find("span.kitItemLabel")
						.html(name);

					//get marked kitGroup li descendant, addKitItem
					var addKitItem = $ektron("div.EktronCommerceItems div.kitView li.addItemMarker ul.kitItems li.addKitItem");

					//insert new item
					itemClone.insertBefore(addKitItem);

					//remove marker from selected li
					kitGroup.removeClass("addItemMarker");

					//re-initialize sorting
					Ektron.Commerce.CatalogEntry.Items.KitView.bindSortable();
				},
				showAddItemModal: function (el) {

					//mark kitGroup li
					$ektron(el).parents("li.kitGroup").addClass("addItemMarker");

					var kitItemModal = $ektron('#ItemsModal');
					kitItemModal
						.find("div.addKitGroup")
						.css("display", "none");
					kitItemModal
						.find("div.addKitItem")
						.css("display", "block");
					kitItemModal
						.find("span.kitGroupModalItemTitle")
						.css("display", "block");
					kitItemModal
						.find("span.kitGroupModalGroupTitle")
						.css("display", "none");

					kitItemModal.modalShow();
				}
			},
			getDisplayValues: function (selectedItem) {
				//get item values
				return displayValues = {
					itemName: selectedItem.children("input.name").val(),
					itemExtraText: selectedItem.children("input.extraText").val(),
					itemPriceModifier: {
						addSubtract: selectedItem.find("input.priceModifierPlusMinus").val(),
						dollars: selectedItem.find("input.priceModifierDollars").val(),
						cents: selectedItem.find("input.priceModifierCents").val(),
						optionAdd: selectedItem.find("input.priceModifierPlusMinus").val() == "+" ? "selected" : "",
						optionSubtract: selectedItem.find("input.priceModifierPlusMinus").val() == "-" ? "selected" : ""
					},
					markedForDelete: selectedItem.children("input.markedForDelete").val()
				}
			},
			getDisplayModes: function () {
				//set edit field display defaults to none
				return viewDisplayModes = {
					itemExtraText: "block",
					itemName: "block",
					itemPriceModifier: {
						addSubtract: "block",
						cents: "block",
						decimalPoint: "block",
						dollars: "block",
						optionAdd: "selected",
						optionSubtract: "",
						view: "block"
					},
					requiredGlyph: "inline",
					requiredDescription: "block"
				}
			},
			getDisplayModeDefaults: function () {
				//set edit field display defaults to none
				return displayValueDefaults = {
					itemExtraText: "none",
					itemName: "none",
					itemPriceModifier: {
						addSubtract: "none",
						cents: "none",
						decimalPoint: "none",
						dollars: "none",
						optionAdd: "selected",
						optionSubtract: "",
						view: "none"
					},
					markedForDelete: "false",
					requiredGlyph: "none",
					requiredDescription: "none"
				}
			},
			setDisplayEdit: function (editDisplayValues, editDisplayModes) {
				//get item wrapper
				var itemWrapper = $ektron("div.EktronCommerceItems div.kitView div.itemDetails");

				//set field display modes and values
				itemWrapper
					.find("input.itemNameEdit")
					.val(editDisplayValues.itemName)
					.css("display", editDisplayModes.itemName);
				itemWrapper
					.find("textarea.itemExtraTextEdit")
					.val(editDisplayValues.itemExtraText)
					.css("display", editDisplayModes.itemExtraText);
				itemWrapper
					.find("select.itemPriceModifierAddSubtractEdit")
					.val(editDisplayValues.itemPriceModifier.addSubtract)
					.css("display", editDisplayModes.itemPriceModifier.addSubtract);
				itemWrapper
					.find("input.itemPriceModifierDollarsEdit")
					.val(editDisplayValues.itemPriceModifier.dollars)
					.css("display", editDisplayModes.itemPriceModifier.dollars);
				itemWrapper
					.find("span.decimalPoint")
					.val(editDisplayValues.itemPriceModifier.dollars)
					.css("display", editDisplayModes.itemPriceModifier.decimalPoint);
				itemWrapper
					.find("input.itemPriceModifierCentsEdit")
					.val(editDisplayValues.itemPriceModifier.cents)
					.css("display", editDisplayModes.itemPriceModifier.cents);
				itemWrapper
					.find("option.add")
					.attr("selected", editDisplayValues.itemPriceModifier.optionAdd);
				itemWrapper
					.find("option.subtract")
					.attr("selected", editDisplayValues.itemPriceModifier.optionSubtract);
				itemWrapper
					.find("span.required")
					.css("display", editDisplayModes.requiredGlyph);
				itemWrapper
					.find("p.requiredFieldNote")
					.css("display", editDisplayModes.requiredDescription);
			},
			setDisplayView: function (viewDisplayValues, viewDisplayModes) {
				//get item wrapper
				var itemWrapper = $ektron("div.EktronCommerceItems div.kitView div.itemDetails");

				//set field display modes and values
				itemWrapper.find("span.itemNameView")
					.html(viewDisplayValues.itemName)
					.css("display", viewDisplayModes.itemName);
				itemWrapper.find("span.itemExtraTextView")
					.html(viewDisplayValues.itemExtraText)
					.css("display", viewDisplayModes.itemExtraText);
				itemWrapper.find("span.itemPriceModifierView")
					.html(
						viewDisplayValues.itemPriceModifier.addSubtract +
						viewDisplayValues.itemPriceModifier.dollars +
						"." +
						viewDisplayValues.itemPriceModifier.cents)
					.css("display", viewDisplayModes.itemPriceModifier.view);
				itemWrapper
					.find("span.required")
					.attr("display", viewDisplayModes.requiredGlyph);
				itemWrapper
					.find("p.requiredFieldNote")
					.attr("display", viewDisplayModes.requiredDescription);
			}
		},
		Group: {
			Add: {
				addItem: function (el) {
					//get item values from modal
					var addItemsModal = $ektron("#ItemsModal div.addKitGroup");

					//get new item values
					var name = Ektron.Commerce.CatalogEntry.Items.escapeAndEncode(addItemsModal.find("input.addGroupName").val());
					var description = Ektron.Commerce.CatalogEntry.Items.escapeAndEncode(addItemsModal.find("textarea.addGroupDescription").val());

					//get kit group ul
					var kitGroup = $ektron(".addItemMarker");

					//get kit group clone
					var groupClone = kitGroup.find("li.kitGroupClone").clone(true);

					//remove clone class and add group class		           
					groupClone
						.removeClass("kitGroupClone")
						.addClass("kitGroup");

					//remove item clone
					groupClone
						.find("li.kitItemClone")
						.remove();

					//set new item values
					groupClone
						.children("input.name")
						.val(name);
					groupClone
						.children("input.description")
						.val(description);
					groupClone
						.children("span.kitGroupLabel")
						.html(name);

					//insert new item
					groupClone.insertBefore("ul.kitGroups li.kitGroupClone");

					//remove marker from ul
					kitGroup.removeClass("addItemMarker");

					//re-initialize sorting
					Ektron.Commerce.CatalogEntry.Items.KitView.bindSortable();

					//ie click fix
					$ektron("ul.kitGroups li.addKitGroup span.kitGroupLabel a").css("zoom", "1");
				},
				showAddItemModal: function (el) {
					//mark kitGroup ul
					var kitGroup = $ektron(el).parents("ul.kitGroups").addClass("addItemMarker");

					var kitGroupModal = $ektron('#ItemsModal');
					kitGroupModal
						.find("div.addKitGroup")
						.css("display", "block");
					kitGroupModal
						.find("div.addKitItem")
						.css("display", "none");
					kitGroupModal
						.find("span.kitGroupModalGroupTitle")
						.css("display", "block");
					kitGroupModal
						.find("span.kitGroupModalItemTitle")
						.css("display", "none");

					kitGroupModal.modalShow();
				}
			},
			unescapeHTML: function (html) {
				var htmlNode = document.createElement("DIV");
				htmlNode.innerHTML = html;
				if (htmlNode.innerText !== undefined)
					return htmlNode.innerText; // IE
				return htmlNode.textContent; // FF
			},
			getDisplayValues: function (selectedItem) {
				//get group values
				return displayValues = {
					groupName: Ektron.Commerce.CatalogEntry.Items.KitView.Group.unescapeHTML(selectedItem.children("input.name").val()),
					groupDescription: Ektron.Commerce.CatalogEntry.Items.KitView.Group.unescapeHTML(selectedItem.children("input.description").val()),
					markedForDelete: selectedItem.children("input.markedForDelete").val()
				}
			},
			getDisplayModes: function () {
				//set display <span> tags to block (set to none by default)
				return viewDisplayModes = {
					groupName: "block",
					groupDescription: "block",
					requiredGlyph: "inline",
					requiredDescription: "block"
				}
			},
			getDisplayModeDefaults: function () {
				//set edit fields to block (set to none by default)
				return displayValueDefaults = {
					groupName: "none",
					groupDescription: "none",
					requiredGlyph: "none",
					requiredDescription: "none"
				}
			},
			setDisplayEdit: function (editDisplayValues, editDisplayModes) {
				//get group wrapper
				var groupWrapper = $ektron("div.EktronCommerceItems div.kitView div.groupDetails");

				//set field display modes and values
				groupWrapper
					.find("input.groupNameEdit")
					.val(editDisplayValues.groupName)
					.css("display", editDisplayModes.groupName);
				groupWrapper
					.find("textarea.groupDescriptionEdit")
					.val(editDisplayValues.groupDescription)
					.css("display", editDisplayModes.groupDescription);
				groupWrapper
					.find("span.required")
					.css("display", editDisplayModes.requiredGlyph);
				groupWrapper
					.find("p.requiredFieldNote")
					.css("display", editDisplayModes.requiredDescription);
			},
			setDisplayView: function (viewDisplayValues, viewDisplayModes) {
				//get group wrapper
				var groupWrapper = $ektron("div.EktronCommerceItems div.kitView div.groupDetails");

				//set field display modes and values
				groupWrapper.find("span.groupNameView")
					.html(viewDisplayValues.groupName)
					.css("display", viewDisplayModes.groupName);
				groupWrapper.find("span.groupDescriptionView")
					.html(viewDisplayValues.groupDescription)
					.css("display", viewDisplayModes.groupDescription);
				groupWrapper
					.find("span.required")
					.css("display", viewDisplayModes.groupDescription);
				groupWrapper
					.find("p.requiredFieldNote")
					.css("display", viewDisplayModes.groupDescription);
			}
		},
		Treeview: {
			init: function () {
				//initialize treeview
				$ektron("#EktronCommerceItemsKitViewGroups").treeview({
					collapsed: true
				});
			}
		}
	},
	Modal: {
		KitView: {
			Buttons: {
				cancel: function () {
					Ektron.Commerce.CatalogEntry.Items.Modal.hide();
				},
				clearFields: function () {
					//clear input fields
					$ektron("div.EktronCommerceItems div.itemsTabModal input:text, div.EktronCommerceItems div.itemsTabModal textarea").each(function (i) {
						var field = $ektron(this);
						if (field.hasClass("addItemPriceAmountDollarsEdit")) {
							//dollars
							field.val("0");
						} else if (field.hasClass("addItemPriceAmountCentsEdit")) {
							//cents
							field.val("00");
						} else {
							//text and textarea
							field.val("");
						}
					});
					$ektron("div.EktronCommerceItems div.itemsTabModal option").each(function (i) {
						var option = $ektron(this);
						option.attr("selected", option.hasClass("add") ? "selected" : "");
						option.attr("selected", option.hasClass("subtract") ? "" : "selected");
					});
				},
				ok: function (el) {
					//determine if group of item is being added
					var type;
					var wrapper;
					var wrappers = $ektron(el).parents("p.addKitNodeButtons").prevAll("div");
					var div;
					wrappers.each(function (i) {
						div = $ektron(this);
						if (div.css("display") == "block") {
							type = div.hasClass("addKitGroup") == true ? "group" : "item";
							wrapper = div;
						}
					});


					var nameField = wrapper.find("input.nameField");
					var nameFieldValue = nameField.val();
					if (nameFieldValue.length == 0 || Ektron.Commerce.CatalogEntry.Items.isWhitespace(nameFieldValue)) {
						//name has no value - alert user
						alert($ektron("div.EktronCommerceItems input.blankNameFieldError").val());
					} else {
						//name has value - process input
						switch (type) {
							case "group":
								//ensure name field isn't empty
								Ektron.Commerce.CatalogEntry.Items.KitView.Group.Add.addItem(el);
								Ektron.Commerce.CatalogEntry.Items.Modal.hide();
								break;
							case "item":
								Ektron.Commerce.CatalogEntry.Items.KitView.Item.Add.addItem(el);
								Ektron.Commerce.CatalogEntry.Items.Modal.hide();
								break;
						}

						//clear input fields
						Ektron.Commerce.CatalogEntry.Items.Modal.KitView.Buttons.clearFields();

						//update json data
						Ektron.Commerce.CatalogEntry.Items.KitView.Data.init();
					}
				}
			}
		},
		init: function () {
			$ektron('#ItemsModal').drag('.itemsModalHeader');
			$ektron('#ItemsModal').modal({
				modal: true,
				overlay: 0,
				toTop: true,
				onShow: function (hash) {
					hash.o.fadeTo("fast", 0.5, function () {
						var originalWidth = hash.w.width();
						hash.w.find("h4").css("width", originalWidth + "px");
						var width = "-" + String(originalWidth / 2) + "px";
						hash.w.css("margin-left", width);
						hash.w.fadeIn("fast");
					});
				},
				onHide: function (hash) {
					hash.w.fadeOut("fast");
					hash.o.fadeOut("fast", function () {
						if (hash.o)
							hash.o.remove();
						//clear input fields
						Ektron.Commerce.CatalogEntry.Items.Modal.KitView.Buttons.clearFields();
					});
				}
			});
			$ektron('#ItemsModal').find("h3 img").bind("click", function () {
				$ektron("ul.kitGroups").removeClass("addItemMarker");
				Ektron.Commerce.CatalogEntry.Items.Modal.hide();
			});
		},
		hide: function () {
			$ektron('#ItemsModal').modalHide();
		}
	},
	//kit or default
	view: ""
}

//Intialiaze Ektron Commerce Catalog Entry
Ektron.ready(function(){
	Ektron.Commerce.CatalogEntry.Items.init();
});