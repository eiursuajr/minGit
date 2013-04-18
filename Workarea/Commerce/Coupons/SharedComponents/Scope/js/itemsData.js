Ektron.ready(function(){
	//page load init
    Ektron.Commerce.Coupons.Scope.ItemsData.init();
	var msAjax = Sys.WebForms.PageRequestManager.getInstance();
	if (msAjax != null) {
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.Commerce.Coupons.Scope.ItemsData.initAfterAjax);
	}
});

//define Ektron object only if it's not already defined
if (Ektron === undefined) {
    Ektron = {};
}

//define Ektron.Commerce object only if it's not already defined
if (Ektron.Commerce === undefined) {
    Ektron.Commerce = {};
}

//define Ektron.Commerce.Coupons object only if it's not already defined
if (Ektron.Commerce.Coupons === undefined) {
    Ektron.Commerce.Coupons = {};
}

//define Ektron.Commerce.Coupons.Scope object only if it's not already defined
if (Ektron.Commerce.Coupons.Scope === undefined) {
    Ektron.Commerce.Coupons.Scope = {};
}

//define Ektron.Commerce.Coupons.Scope.ItemsData object only if it's not already defined
if (Ektron.Commerce.Coupons.Scope.ItemsData === undefined) {
    //Ektron.Commerce.Coupons.Scope.ItemsData Object
    Ektron.Commerce.Coupons.Scope.ItemsData = {
        Actions: {
            addRows: function (rows) {
                //add product/folder/taxonomy to table
                var itemsDataTable = $ektron("div.items div.itemsData table.items");
                var itemsDataTableBody = itemsDataTable.find("tbody");
                var noItemsRow = itemsDataTable.find("tr.noItems");
                var cloneRow = itemsDataTable.find("tr.cloneRow");

                //add rows
                for (var i = 0; i < rows.length; i++) {

                    //ensure rows are not added more than once
                    if (itemsDataTableBody.find("input.id[value=" + rows[i].Id + "]").length == 0) {
                        //make new row
                        var row = cloneRow.clone();
                        //set row cell values
                        row.find("td.id").text(rows[i].Id);
                        row.find("td.name").text(rows[i].Name);
                        row.find("td.path").text(rows[i].Path);
                        switch (rows[i].TypeCode) {
                            case "0":
                                row.find("td.type").text(rows[i].SubType);
                                break;
                            case "1":
                                row.find("td.type").text(rows[i].Type);
                                break
                            case "2":
                                row.find("td.type").text(rows[i].Type);
                                break;
                        }

                        //set row input values
                        row.find("input.id").attr("value", rows[i].Id);
                        row.find("input.name").attr("value", rows[i].Name);
                        row.find("input.path").attr("value", rows[i].Path);
                        row.find("input.type").attr("value", rows[i].Type);
                        row.find("input.typeCode").attr("value", rows[i].TypeCode);
                        if (rows[i].TypeCode == "0") {
                            row.find("input.subType").attr("value", rows[i].SubType);
                        }
                        row.find("input.markedForDelete").attr("value", rows[i].MarkedForDelete);

                        row.removeClass("cloneRow").addClass("item");
                        itemsDataTableBody.append(row);
                        if (rows[i].MarkedForDelete == "true") {
                            //pass the marked for delete image to the markForDelete() fuction
                            Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.markForDelete(row.find("td.data img.markForDelete"));
                        }
                    }
                    else {
                        if (rows[i].MarkedForDelete == "true") {
                            // mark existing row for deletion
                            Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.markForDelete(itemsDataTable.find("tr:has(input.id[value=" + rows[i].Id + "])").find("td.data img.markForDelete"));
                        }
                        else {
                            // clear deletion flags
                            Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.restore(itemsDataTable.find("tr:has(input.id[value=" + rows[i].Id + "])").find("td.data img.restore"));
                        }
                    }
                }

                noItemsRow.hide(); //hide no items row
                Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.toggleHeaderMarkAllForDeleteAndMarkAllForRestoreButtons(); //toggle mark all for delete and restore all buttons if necessary
                Ektron.Commerce.Coupons.Scope.ItemsData.stripeRows() //add row striping
                Ektron.Commerce.Coupons.Scope.ItemsData.Data.update() //update json data

                //ie doesn't like tr display none so display none on tds instead
                if ($ektron.browser.msie)
                    $ektron("table.items tr.noItems td").addClass("resetBorders");
            },
            select: function (type) {
                Ektron.Commerce.Coupons.Scope.ItemsData.Modal.Content.setSource(type); //set iframe source
                Ektron.Commerce.Coupons.Scope.ItemsData.Modal.show(); //show modal
            },
            Items: {
                getTypeList: function (itemType) {
                    var idList = "";
                    // get ids for passed type, not marked for deletion
                    $ektron("div.itemsData table.items tbody tr.item").each(function (i) {
                        if ($ektron(this).find("td.data input.type").attr("value").toLowerCase() == itemType
                            && $ektron(this).find("td.data input.markedForDelete").attr("value").toLowerCase() == "false") {
                            idList += ((idList.length > 0) ? "," : "") + $ektron(this).find("td.data input.id").val();
                        }
                    });
                    return idList;
                },
                markForDelete: function (el) {
                    var buttonDelete = $ektron(el);
                    var buttonRestore = buttonDelete.next();
                    var couponDataCell = buttonDelete.parent();

                    //toggle mark for delete/restore images
                    buttonDelete.hide();
                    buttonRestore.show();
                    buttonDelete.parents("tr.item").find("td").addClass("markedForDelete");

                    //mark row for delete
                    couponDataCell.find("input.markedForDelete").attr("value", "true");

                    //update json data
                    Ektron.Commerce.Coupons.Scope.ItemsData.Data.update() //update json data

                    //toggle mark all for delete and restore all buttons if necessary
                    Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.toggleHeaderMarkAllForDeleteAndMarkAllForRestoreButtons();
                },
                markForDeletePage: function () {
                    //mark all coupons for delete on current page only
                    $ektron("div.items div.itemsData table.items tbody tr.item td.data input.markedForDelete[value='false']").each(function (i) {
                        //pass the marked for delete image to the markForDelete() fuction
                        Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.markForDelete($ektron(this).nextAll("img.markForDelete"));
                    });

                    //toggle header mark for delete / restore button
                    var headerButtonDelete = $ektron("div.items div.itemsData table.items thead th.markForDelete img.markForDelete");
                    var headerButtonRestore = $ektron("div.items div.itemsData table.items thead th.markForDelete img.restore");
                    headerButtonDelete.hide();
                    headerButtonRestore.show();
                },
                restore: function (el) {
                    var buttonRestore = $ektron(el);
                    var buttonDelete = buttonRestore.prev();
                    var couponDataCell = buttonDelete.parent();

                    //toggle mark for delete/restore images
                    buttonDelete.show();
                    buttonRestore.hide();
                    buttonRestore.parents("tr.item").find("td").removeClass("markedForDelete");

                    //unmark row for delete
                    couponDataCell.find("input.markedForDelete").attr("value", "false");

                    //update json data
                    Ektron.Commerce.Coupons.Scope.ItemsData.Data.update() //update json data

                    //toggle mark all for delete and restore all buttons if necessary
                    Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.toggleHeaderMarkAllForDeleteAndMarkAllForRestoreButtons();
                },
                restorePage: function () {
                    //restore all coupons on current page only
                    $ektron("div.items div.itemsData table.items tbody tr.item td.data input.markedForDelete[value='true']").each(function (i) {
                        //pass the restore image to the restore fuction to initiate the restore
                        Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.restore($ektron(this).nextAll("img.restore"));
                    });

                    //toggle header mark for delete / restore button
                    var headerButtonDelete = $ektron("div.items div.itemsData table.items thead th.markForDelete img.markForDelete");
                    var headerButtonRestore = $ektron("div.items div.itemsData table.items thead th.markForDelete img.restore");
                    headerButtonDelete.show();
                    headerButtonRestore.hide();
                },
                toggleHeaderMarkAllForDeleteAndMarkAllForRestoreButtons: function () {
                    //if all coupons in page are marked for delete, toggle header button to show "restore all" instead of "mark all for delete"
                    var totalCouponRows = $ektron("div.items div.itemsData table.items tbody tr.item").length;
                    var totalCouponRowsMarkedForDelete = $ektron("div.items div.itemsData table.items tbody tr.item td.data input.markedForDelete[value='true']").length;

                    var headerButtonDelete = $ektron("div.items div.itemsData table.items thead th.markForDelete img.markForDelete");
                    var headerButtonRestore = $ektron("div.items div.itemsData table.items thead th.markForDelete img.restore");
                    if (totalCouponRows === totalCouponRowsMarkedForDelete) {
                        headerButtonDelete.hide(); //hide mark all for delete
                        headerButtonRestore.show(); //show restore all
                    }
                    else {
                        headerButtonDelete.show(); //show mark all for delete
                        headerButtonRestore.hide(); //hide restore all
                    }
                }
            },
            ok: function () {
                var selectedItemsStringified = $ektron("#EktronCouponItems iframe.ektronCouponItemsIframe").contents().find("input.data").attr("value");
                if ("undefined" != typeof (selectedItemsStringified)) {
                    var selectedItems = Ektron.JSON.parse(selectedItemsStringified);
                    Ektron.Commerce.Coupons.Scope.ItemsData.Actions.addRows(selectedItems); //add rows to table
                }
                Ektron.Commerce.Coupons.Scope.ItemsData.Modal.hide(); //hide modal
            }
        },
        Data: {
            init: function () {
                //initailize json data array
                var data = $ektron("div.itemsData").children("input.data").attr("value");
                if ("undefined" != typeof (data) && data !== "") {
                    Ektron.Commerce.Coupons.Scope.ItemsData.Data.data = Ektron.JSON.parse(data);
                    Ektron.Commerce.Coupons.Scope.ItemsData.Actions.addRows(Ektron.Commerce.Coupons.Scope.ItemsData.Data.data); //add rows to table
                } else {
                    Ektron.Commerce.Coupons.Scope.ItemsData.Data.update();
                }
            },
            update: function () {
                //clear and rebuild data array
                Ektron.Commerce.Coupons.Scope.ItemsData.Data.data = new Array();

                var itemRows = $ektron("div.items div.itemsData table.items tbody tr.item");
                if (itemRows.length == 0) {
                    $ektron("div.items div.itemsData table.items tbody tr.noItems").show();
                } else {
                    itemRows.each(function (i) {
                        var id = $ektron(this).find("input.id").attr("value");
                        var name = $ektron(this).find("input.name").attr("value");
                        var path = $ektron(this).find("input.path").attr("value");
                        var type = $ektron(this).find("input.type").attr("value");
                        var typeCode = $ektron(this).find("input.typeCode").attr("value");
                        var subType;
                        switch (typeCode) {
                            case "0":
                                subType = $ektron(this).find("input.subType").attr("value");
                                break;
                            case "1":
                                subType = "0";
                                break
                            case "2":
                                subType = "0";
                                break;
                        }
                        var markedForDelete = $ektron(this).find("input.markedForDelete").attr("value");

                        var data = {
                            Id: id,
                            Name: name,
                            Path: path,
                            Type: type,
                            TypeCode: typeCode,
                            SubType: subType,
                            MarkedForDelete: markedForDelete
                        }

                        Ektron.Commerce.Coupons.Scope.ItemsData.Data.data.push(data);
                    });

                    $ektron("div.itemsData").children("input.data").attr("value", Ektron.JSON.stringify(Ektron.Commerce.Coupons.Scope.ItemsData.Data.data));
                }
            }
        },
        init: function () {
            //finish init
            Ektron.Commerce.Coupons.Scope.ItemsData.initAfterAjax();
        },
        initAfterAjax: function () {
            //init data
            Ektron.Commerce.Coupons.Scope.ItemsData.Data.init();
            if ("undefined" === typeof (Ektron.Commerce.Coupons.Scope.ItemsData.Data.data)) {
                Ektron.Commerce.Coupons.Scope.ItemsData.Data.data = [];
            }
            //bind to modal
            Ektron.Commerce.Coupons.Scope.ItemsData.Modal.init();
        },
        Modal: {
            cancel: function () {
                Ektron.Commerce.Coupons.Scope.ItemsData.Modal.hide();
            },
            Content: {
                setSource: function (mode) {
                    var header = $ektron("#EktronCouponItems span.ui-dialog-title");
                    var iframe = $ektron("#EktronCouponItems iframe");
                    var templatePath = "../SharedComponents/Scope/ItemsSelection/";
                    switch (mode) {
                        case "catalog":
                            header.html("Add Catalog");
                            var lanaguageId = $ektron("div.itemsData input.languageId").attr("value");
                            var catalogPath = templatePath + "selectFolderProduct.aspx?mode=catalog&languageId=" + lanaguageId + "&idlist=";
                            var categoryArgs = Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.getTypeList("catalog");
                            iframe.attr("src", catalogPath + categoryArgs);
                            break;
                        case "category":
                            header.html("Add Category");
                            var lanaguageId = $ektron("div.itemsData input.languageId").attr("value");
                            var categoryPath = templatePath + "selecttaxonomy.aspx?languageId=" + lanaguageId + "&idlist=";
                            var categoryArgs = Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.getTypeList("taxonomy");
                            iframe.attr("src", categoryPath + categoryArgs);
                            break;
                        case "product":
                            header.html("Add Product");
                            var lanaguageId = $ektron("div.itemsData input.languageId").attr("value");
                            var catalogPath = templatePath + "selectFolderProduct.aspx?mode=product&languageId=" + lanaguageId + "&idlist=";
                            var categoryArgs = Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.getTypeList("product");
                            iframe.attr("src", catalogPath + categoryArgs);
                            break;
                    }
                }
            },
            hide: function () {
                $ektron("#EktronCouponItems").modalHide();
            },
            init: function () {
                var modal = $ektron("#EktronCouponItems");
                modal.drag('.itemsModalHeader');
                modal.modal({
                    toTop: true,
                    modal: true,
                    overlay: 0,
                    onShow: function (hash) {
                        hash.o.fadeTo("fast", 0.5, function () {
                            hash.w.fadeIn("fast");
                        });
                    },
                    onHide: function (hash) {
                        hash.w.fadeOut("fast");
                        hash.o.fadeOut("fast", function () {
                            if (hash.o)
                                hash.o.remove();
                        });
                    }
                });
            },
            show: function () {
                $ektron("#EktronCouponItems").modalShow();
            }
        },
        stripeRows: function () {
            $ektron("div.items div.itemsData table.items tr.item").removeClass("stripe");
            $ektron("div.items div.itemsData table.items tr.item:odd").addClass("stripe");
        }
    }
}
