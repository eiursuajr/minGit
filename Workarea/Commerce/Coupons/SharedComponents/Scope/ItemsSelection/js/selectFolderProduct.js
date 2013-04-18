if (Ektron === undefined) { Ektron = {}; }
if (Ektron.Commerce === undefined) { Ektron.Commerce = {}; }
if (Ektron.Commerce.Coupons === undefined) { Ektron.Commerce.Coupons = {}; }
if (Ektron.Commerce.Coupons.Scope === undefined) { Ektron.Commerce.Coupons.Scope = {}; }
if (Ektron.Commerce.Coupons.Scope.Items === undefined) { Ektron.Commerce.Coupons.Scope.Items = {}; }
if (Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct === undefined) {
	//Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct Object
	Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct = {
		Actions: {
			add: function (data) {
				Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.add(data);
			},
			Click: {
				checkbox: function (elem) {
					var id = $ektron(elem).nextAll("input.id").attr("value");
					var name = $ektron(elem).nextAll("input.name").attr("value");
					var path = $ektron(elem).nextAll("input.path").attr("value");
					var type = $ektron(elem).nextAll("input.type").attr("value");
					var subType = $ektron(elem).nextAll("input.subtype").attr("value");
					var typeCode = $ektron(elem).nextAll("input.typeCode").attr("value");

					id = "undefined" == typeof (id) ? "" : id;
					name = "undefined" == typeof (name) ? "" : name;
					path = "undefined" == typeof (path) ? "" : path;
					type = "undefined" == typeof (type) ? "" : type;
					subType = "undefined" == typeof (subType) ? "" : subType;
					typeCode = "undefined" == typeof (typeCode) ? "" : typeCode;

					var itemData = {
						Id: id,
						Name: name,
						Path: path,
						Type: type,
						SubType: subType,
						TypeCode: typeCode
					}

					if (elem.checked) {
						Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Actions.add(itemData);
					}
					else {
						Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Actions.remove(itemData);
					}
				},
				item: function (elem, folderId, typeCode) {
					if ($ektron(elem).hasClass("catalogFolder") == true && $ektron(elem).parent().parent().find("input.selected")[0].checked == true) {
						//get "previously selected" message
						var message;
						switch (typeCode) {
							case 0:
								message = Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.LocalizedStrings.selectedProductClickMessage;
								break;
							case 1:
								message = Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.LocalizedStrings.selectedFolderClickMessage;
								break;
						}
						alert(message);
					}
					else {
						var href;
						var input;
						switch (typeCode) {
							case 0:
								input = $ektron("input.folderList");
								//href = "select_folder.aspx?id=" + folderId + "&folderidlist=";
								break;
							case 1:
								input = $ektron("input.productList");
								//href = "select_folder.aspx?id=" + folderId + "&entryidlist=";
								break;
						}
						//get previously selected ids
						var typeList = parent.Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.getTypeList(typeCode);
						//set input - folder or product list
						input.attr("value", typeList);

						//set folderId
						$ektron("input.folderId").attr("value", folderId);

						//reload page
						$ektron("form").submit();
						//window.location.href = href + typeList;
					}
				}
			},
			remove: function (data) {
				Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.remove(data);
				//parent.Ektron.Commerce.Coupons.Scope.ItemsData.Actions.remove(data);
			}
		},
		Data: {
			add: function (data) {
			    //update all data
				Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.data.push(data);
				$ektron("input.data").attr("value", Ektron.JSON.stringify(Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.data));

				switch (data.Type) {
					case "catalog":
						//update folder id list
						Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.folderIds.push(data.Id);
						var serliazedFolderIds = Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.folderIds.toString();
						$ektron("input.folderIds").attr("value", serliazedFolderIds);
						//alert("Folder IDs: " + $ektron("input.folderIds").attr("value"));
						break;
					case "product":
						//update folder id list
						Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.productIds.push(data.Id);
						var serliazedProductIds = Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.productIds.toString();
						$ektron("input.productIds").attr("value", serliazedProductIds);
						//alert("Product IDs: " + $ektron("input.productIds").attr("value"));
						break;
				}
			},
			init: function () {
				//initialize data objects
				if ("undefined" === typeof (Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.data)) {
					Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.data = [];
				}
				if ("undefined" === typeof (Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.folderIds)) {
					Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.folderIds = [];
				}
				if ("undefined" === typeof (Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.productIds)) {
					Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.productIds = [];
				}

				//initailize json data array
				var data = $ektron("input.data").attr("value");

	            if ("undefined" != typeof (data)) {
	                Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.data = $ektron.isArray(Ektron.JSON.parse(data)) ? Ektron.JSON.parse(data) : [];
	            }

				//initialize folder id list
				var folderIds = $ektron("input.folderIds");
				var folderIdsValue = folderIds.attr("value");
				if ("undefined" != typeof (folderIdsValue) && folderIds[0].value != null && folderIds[0].length != 0) {
					Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.folderIds = new Array(folderIdsValue);
				} else {
					Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.folderIds = new Array();
				}
				var displayedFolders = $ektron("table.folders td.data");
				var selectedFolders = Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.folderIds;
				for (var i = 0; i < selectedFolders.length; i++) {
					displayedFolders.each(function () {
						if ($ektron(this).find("input.id").attr("value") == selectedFolders[i]) {
							$ektron(this).find("input.selected").attr("checked", "checked");
						}
					});
				}


				//initialize product id list
				var productIds = $ektron("input.productIds");
				var productIdsValue = productIds.attr("value");
				if ("undefined" != typeof (productIdsValue) && productIds[0].value != null && productIds[0].length != 0) {
					Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.productIds = new Array(productIdsValue);
				} else {
					Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.productIds = new Array();
				}

				var displayedProducts = $ektron("table.products td.data");
				var selectedProducts = Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.productIds;
				for (var i = 0; i < selectedFolders.length; i++) {
					displayedProducts.each(function () {
						if ($ektron(this).find("input.id").attr("value") == selectedProducts[i]) {
							$ektron(this).find("input.selected").attr("checked", "checked");
						}
					});
				}
			},
			remove: function (data) {
				var selectedData = Ektron.JSON.parse($ektron("input.data").attr("value"));
				for (var i = 0; i < selectedData.length; i++) {
					if (data.Id == selectedData[i].Id) {
						if ("undefined" == typeof selectedData[i].NewlyAdded
							|| selectedData[i].NewlyAdded == "true")
							selectedData.splice(i, 1);
						else
							selectedData[i].MarkedForDelete = "true";
						break;
					}
                }

                Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.data = $ektron.isArray(selectedData) ? selectedData : [];
			
				$ektron("input.data").attr("value", Ektron.JSON.stringify(Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.data));

				switch (data.Type) {
					case "catalog":
						//update folder id list
						var selectedFolderIds = String($ektron("input.folderIds").attr("value")).split(",");
						for (var i = 0; i < selectedFolderIds.length; i++) {
							if (data.Id == selectedFolderIds[i]) {
								selectedFolderIds.splice(i, 1);
							}
						}
						Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.folderIds = selectedFolderIds;
						$ektron("input.folderIds").attr("value", Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.folderIds.toString());
						//alert("Folder IDs: " + $ektron("input.folderIds").attr("value"));
						break;
					case "product":
						//update folder id list
						var selectedProductIds = String($ektron("input.productIds").attr("value")).split(",");
						for (var i = 0; i < selectedProductIds.length; i++) {
							if (data.Id == selectedProductIds[i]) {
								selectedProductIds.splice(i, 1);
							}
						}
						Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.productIds = selectedProductIds;
						$ektron("input.productIds").attr("value", Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.productIds.toString());
						break;
				}
			}
		},
		init: function () {
			//initailize json data
			Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Data.init();

			//initialize localized strings
			Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.LocalizedStrings = Ektron.JSON.parse($ektron("input.localizedStrings").attr("value"));
		}
	}
}

Ektron.ready(function(){
	Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.init();
});