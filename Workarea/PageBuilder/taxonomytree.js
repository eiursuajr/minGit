//Define Ektron.PageBuilder object only if it's not already defined
if (Ektron === undefined) {
    Ektron = {};
}

Ektron.TaxonomyTree = {
    appPath: '',
    init: function(appPath, selectedNodesCtrlId) {
        var JSCallBackOnChange = $ektron("input#" + selectedNodesCtrlId).parent().parent().find(".hdnJSCallBack input").val();
        Ektron.TaxonomyTree.appPath = appPath;
        var els = $ektron("ul.EktronTaxonomyTree");
        for (var i = 0; i < els.length; i++) {
            Ektron.TaxonomyTree.configTreeview(els[i], selectedNodesCtrlId);
        }

        $ektron("input.categoryCheck").live('click', function(e) {
            e.stopPropagation();
            $ektron(this).parent().parent(".expandable").children("div").click();
            if (this.checked) {
                //if checked now, uncheck and disable all children nodes
                var parentuls = $ektron(this).parents("ul[data-ektron-taxid]");
                for (var i = 0; i < parentuls.length; i++) {
                    //check parent box
                    var parentid = $ektron(parentuls[i]).attr("data-ektron-taxid");
                    var box = $ektron("span[data-ektron-taxid='" + parentid + "'] > input");
                    box.attr("checked", "true");
                    box.attr("disabled", "disabled");
                }
            } else {
                //if unchecked now, enable all parent nodes except those above another checked node
                var disabled = $ektron("input.categoryCheck:checked[disabled]");
                disabled.attr("checked", "");
                disabled.attr("disabled", "");

                var disabled = $ektron("input.categoryCheck:checked");
                for (var j = 0; j < disabled.length; j++) {
                    var parentuls = $ektron(disabled[j]).parents("ul[data-ektron-taxid]");
                    for (var i = 0; i < parentuls.length; i++) {
                        //check parent box
                        var parentid = $ektron(parentuls[i]).attr("data-ektron-taxid");
                        var box = $ektron("span[data-ektron-taxid='" + parentid + "'] > input");
                        box.attr("checked", "true");
                        box.attr("disabled", "disabled");
                    }
                }
            }
            //get all checked nodes, concatenate, store in textbox
            var checked = $ektron("input.categoryCheck:checked");
            var string = "";
            for (var i = 0; i < checked.length; i++) {
                if (!checked[i].disabled) {
                    string += $ektron(checked[i]).parent().attr("data-ektron-taxid") + ",";
                }
            }
            $ektron("input#" + selectedNodesCtrlId)[0].value = string.substring(0, string.length - 1);
            if (JSCallBackOnChange != "") {
                eval(JSCallBackOnChange + "('" + string.substring(0, string.length - 1) + "')");
            }
        });

        var preselectitem = $ektron("input#" + selectedNodesCtrlId);
        var preselect = preselectitem.val();
        if (typeof (preselect) != 'undefined' && preselect.length > 0) {
            var locations = preselect.split('|');
            for (var i = 0; i < locations.length; i++) {
                Ektron.TaxonomyTree.openToSelectedItem(locations[i].split(','), 0);
            }
        }
        //get all checked nodes, concatenate, store in textbox
        var checked = $ektron("input.categoryCheck:checked");
        var string = "";
        for (var i = 0; i < checked.length; i++) {
            if (!checked[i].disabled) {
                string += $ektron(checked[i]).parent().attr("data-ektron-taxid") + ",";
            }
        }

        preselectitem.val(string.substring(0, string.length - 1));
    },
    configTreeview: function(el, selectedNodesCtrlId) {
        var pblloading = null;
        if (typeof parent != "undefined") {
            if (typeof (parent.$ektron) != "undefined") {
                pblloading = parent.$ektron("div.wizardStatus");
            }
        }

        $ektron(el).treeview({
            toggle: function(index, element) {
                var doajax = function() {
                    $ektron.ajax({
                        type: "POST",
                        cache: false,
                        async: false,
                        url: Ektron.TaxonomyTree.appPath + "pagebuilder/taxonomytree.ashx",
                        data: { "taxid": $element.attr("data-ektron-taxid") },
                        success: function(msg) {
                            var thisel = $ektron(msg);
                            $element.append(thisel);
                            $ektron(el).treeview({ add: thisel });
                            //are any ancestors checked? if so, disable me
                            if (pblloading != null) {
                                parent.$ektron("div.wizardStatus").fadeOut(300);
                            }
                        }
                    });
                }

                var $element = $ektron(element);
                if ($element.html() == "") {
                    if (pblloading != null && pblloading.length > 0) {
                        pblloading.fadeIn(100);
                        doajax();
                    } else {
                        doajax();
                    }
                }
            }
        });
    },
    openToSelectedItem: function(items, tries) {
        if (items.length > 0) {
            if (items[0] == "") {
                items.shift();
                Ektron.TaxonomyTree.openToSelectedItem(items, tries);
                return;
            }
            var item = $ektron(".treecontainer span[data-ektron-taxid='" + items[0] + "']");
            if (item.length == 0) {
                if (tries < 8) {
                    setTimeout(function() { Ektron.TaxonomyTree.openToSelectedItem(items, tries + 1); }, 250);
                }
                return;
            }
            if (items.length == 1) {
                //check the box
                var input = item.children("input");
                if (input != null && input.length > 0) {
                    input.attr("checked", "checked");
                    //disable all parents
                    var parentuls = $ektron(item).parents("ul[data-ektron-taxid]");
                    for (var i = 0; i < parentuls.length; i++) {
                        //check parent box
                        var parentid = $ektron(parentuls[i]).attr("data-ektron-taxid");
                        var box = $ektron("span[data-ektron-taxid='" + parentid + "'] > input");
                        box.attr("checked", "true");
                        box.attr("disabled", "disabled");
                    }
                }
            } else {
                //open the node
                if (item.parent().hasClass("closed")) {
                    item.parent().find("div.hitarea").click();
                }
                items.shift();
                Ektron.TaxonomyTree.openToSelectedItem(items, 0);
            }
        }
    }
};
