if (Ektron.PFWidgets === undefined) {
	Ektron.PFWidgets = {};
}

Ektron.PFWidgets.WebCalendar = {
    parentID: "",
    webserviceURL: "/pagebuilder/widgets/WebCalendar/LSHandler.ashx",
    setupAll: function(webservice) {
        Ektron.PFWidgets.WebCalendar.webserviceURL = webservice;
        $ektron(".WCEdit").tabs();
        Ektron.PFWidgets.WebCalendar.Folder.ConfigFolderTree();
        Ektron.PFWidgets.WebCalendar.User.init();
        Ektron.PFWidgets.WebCalendar.Group.init();
        var sel = $ektron("select.bgcolor[which]");
        for (var i = 0; i < sel.length; i++) {
            var val = $ektron(sel[i]).attr("which");
            $ektron(sel[i]).find("option[value=" + val + "]").attr("selected", "selected");
        }
        $ektron("select.bgcolor").msDropDown();
    },
    createRequestObj: function(action, pagenum, searchtext, objectid) {
        request = {
            "action": action,
            "page": pagenum,
            "searchText": searchtext,
            "objectID": objectid
        };
        return Ektron.JSON.stringify(request);
    }
};

Ektron.PFWidgets.WebCalendar.DataStore = {
    AddSource: function(type, id, name) {
        if ($ektron(".WCEdit .HiddenWCData").val().indexOf(type + "-" + id) == -1) {
            $ektron("tr.initbgcolor").removeClass("initbgcolor");
            var template = $ektron(".SelectedCalendars .selectedtemplate").html();
            template = template.replace("{id}", id);
            template = template.replace("{type}", type);
            template = template.replace("{name}", name);
            var randomnumber = Math.floor(Math.random() * 9999);
            template = template.split("uniqueid").join(randomnumber);
            template = "<tr class=\"source initbgcolor\">" + template + "</tr>";
            $ektron(".SelectedCalendars table").append(template);
            $ektron("tr.initbgcolor select.bgcolor").msDropDown();

            Ektron.PFWidgets.WebCalendar.DataStore.UpdateSaveString();
            Ektron.PFWidgets.WebCalendar.DataStore.FixHighlighting();
        }
        $ektron(".WCEdit").tabs('select', 0);
        return false;
    },
    RemoveSource: function(el) { //id-type-color:id-type-color
        $ektron(el).parent().parent().remove();
        Ektron.PFWidgets.WebCalendar.DataStore.UpdateSaveString();
        Ektron.PFWidgets.WebCalendar.DataStore.FixHighlighting();
    },
    UpdateSaveString: function() {
        var savestring = "";
        var rows = $ektron(".WCEdit .SelectedCalendars tr.source");
        for (var i = 0; i < rows.length; i++) {
            var sourceid = $ektron(rows[i]).find(".id").html();
            var backcolor = $ektron(rows[i]).find("select.bgcolor option:selected").val();
            savestring += sourceid + "-" + backcolor;
            if (i < rows.length - 1) savestring += ":";
        }
        $ektron(".WCEdit .HiddenWCData").val(savestring);
    },
    FixHighlighting: function() {
        var rows = $ektron(".SelectedCalendars tr.source");
        if (rows.length == 0) {
            $ektron(".SelectedCalendars .nosources").show();
        } else {
            $ektron(".SelectedCalendars .nosources").hide();
        }
        for (var i = 0; i < rows.length; i++) {
            var $el = $ektron(rows[i]);
            $el.removeClass("even");
            if (i % 2 == 0) $el.addClass("even");
        }
    }
};

Ektron.PFWidgets.WebCalendar.Folder = {
    DirectoryToHtml: function(directory) {
        var html = "";
        for (var i = 0; i < directory.subdirectories.length; i++) {
            html += "<li class=\"closed";
            if (i == directory.subdirectories.length - 1) html += " last";
            if (directory.subdirectories[i].iscalendar) html += " calendar";
            html += "\"><span class=\"folder\" data-ektron-folid=\"" + directory.subdirectories[i].id + "\">";
            if (!directory.subdirectories[i].iscalendar) {
                html += directory.subdirectories[i].name;
            } else {
            html += "<span class=\"add\" onclick=\"return Ektron.PFWidgets.WebCalendar.DataStore.AddSource('System', " + directory.subdirectories[i].id + ", '" + directory.subdirectories[i].name + "');\">" + directory.subdirectories[i].name + "</span>";
            }
            html += "</span>";
            if (directory.subdirectories[i].haschildren) html += "<ul data-ektron-folid=\"" + directory.subdirectories[i].id + "\"></ul>";
            html += "</li>";
        }

        return html;
    },
    ConfigFolderTree: function() {
        $ektron.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: Ektron.PFWidgets.WebCalendar.webserviceURL,
            data: { "request": Ektron.PFWidgets.WebCalendar.createRequestObj("getchildfolders", 0, "", 0) },
            success: function(msg) {
                var directory = eval("(" + msg + ")");
                $ektron("ul.EktronFolderTree").html(Ektron.PFWidgets.WebCalendar.Folder.DirectoryToHtml(directory));
                $ektron("ul.EktronFolderTree").treeview(
                {
                    toggle: function(index, element) {
                        var $element = $ektron(element);

                        if ($element.html() === "") {
                            $ektron.ajax(
                            {
                                type: "POST",
                                cache: false,
                                async: false,
                                url: Ektron.PFWidgets.WebCalendar.webserviceURL,
                                data: { "request": Ektron.PFWidgets.WebCalendar.createRequestObj("getchildfolders", 0, "", $element.attr("data-ektron-folid")) },
                                success: function(msg) {
                                    var directory = eval("(" + msg + ")");
                                    var el = $ektron(Ektron.PFWidgets.WebCalendar.Folder.DirectoryToHtml(directory));
                                    $ektron(element).append(el);
                                    $ektron("ul.EktronFolderTree").treeview({ add: el });
                                }
                            });
                        }
                    }
                });
            }
        });
    }
};

Ektron.PFWidgets.WebCalendar.User = {
    init: function() {
        $ektron(".WebCalByUser .UserName").bind("keypress", function(e) {
            var keycode;
            if (window.event) // IE
                keycode = e.keyCode;
            else if (e.which) // Netscape/Firefox/Opera
                keycode = e.which;
            if (keycode == 13) {
                e.returnValue = false;
                e.cancel = true;
                setTimeout("Ektron.PFWidgets.WebCalendar.User.ExecuteSearch(0);", 1);
                return false;
            }
        });
    },
    Output: function(userlist) {
        var template = $ektron(".WebCalByUser .resultsTemplate").html();
        var html = "";
        for (var i = 0; i < userlist.users.length; i++) {
            var thisitem = template;
            thisitem = thisitem.replace("usericon", userlist.users[i].avatar);
            thisitem = thisitem.replace("fname", userlist.users[i].fname).replace("fname", userlist.users[i].fname);
            thisitem = thisitem.replace("lname", userlist.users[i].lname).replace("lname", userlist.users[i].lname);
            thisitem = thisitem.replace("email", userlist.users[i].email);
            thisitem = thisitem.replace("templid", userlist.users[i].userid);
            html += "<tr class=\"results\" style=\"display:none;\">" + thisitem + "</tr>";
        }
        $ektron(".WebCalByUser .results").remove();
        $ektron(".WebCalByUser table").append(html);
    },
    Paging: function(userlist) {
        var showpaging = false;
        if (userlist.pages > 1) {
            if (userlist.thispage < userlist.pages - 1) {
                showpaging = true;
                var next = $ektron(".WebCalByUser .paging .next");
                next.unbind("click").bind("click", function() {
                    Ektron.PFWidgets.WebCalendar.User.ExecuteSearch(userlist.thispage + 1);
                });
                next.show();
            }
            if (userlist.thispage > 0) {
                showpaging = true;
                var prev = $ektron(".WebCalByUser .paging .prev");
                prev.unbind("click").bind("click", function() {
                    Ektron.PFWidgets.WebCalendar.User.ExecuteSearch(userlist.thispage - 1);
                });
                prev.show();
            }
        }
    },
    ExecuteSearch: function(page) {
        Ektron.PFWidgets.WebCalendar.User.UserSearch($ektron(".WebCalByUser .searcharea .Execute")[0], page);
    },
    UserSearch: function(el, page) {
        if (!page) page = 0;
        $ektron(".WebCalByUser .center, .WebCalByUser .results, .WebCalByUser .paging span span").hide();
        $ektron(".WebCalByUser .Searching").show();
        $el = $ektron(el);
        var searchtext = $el.parent().find(".UserName").val();
        if (searchtext != "") {
            $ektron.ajax({
                type: "POST",
                cache: false,
                async: false,
                url: Ektron.PFWidgets.WebCalendar.webserviceURL,
                data: { "request": Ektron.PFWidgets.WebCalendar.createRequestObj("getusers", page, searchtext, 0) },
                success: function(msg) {
                    $ektron(".WebCalByUser .center, .WebCalByUser .results").hide();
                    try {
                        var userlist = eval("(" + msg + ")");
                        if (userlist.users.length == 0) {
                            $ektron(".WebCalByUser .noResults").show();
                        } else {
                            Ektron.PFWidgets.WebCalendar.User.Output(userlist);
                            $ektron(".WebCalByUser .results").show();
                        }
                        Ektron.PFWidgets.WebCalendar.User.Paging(userlist);
                    } catch (ex) {
                        $ektron(".WebCalByUser .noResults").show();
                    }
                }
            });
        } else {
            $ektron(".WebCalByUser .center, .WebCalByUser .results").hide();
            $ektron(".WebCalByUser .noResults").show();
        }
    }
};

Ektron.PFWidgets.WebCalendar.Group = {
    init: function() {
        $ektron(".WebCalByGroup .GroupName").bind("keypress", function(e) {
            var keycode;
            if (window.event) // IE
                keycode = e.keyCode;
            else if (e.which) // Netscape/Firefox/Opera
                keycode = e.which;
            if (keycode == 13) {
                e.returnValue = false;
                e.cancel = true;
                setTimeout("Ektron.PFWidgets.WebCalendar.Group.ExecuteSearch(0);", 1);
                return false;
            }
        });
    },
    Output: function(grouplist) {
        var template = $ektron(".WebCalByGroup .resultsTemplate").html();
        var html = "";
        for (var i = 0; i < grouplist.groups.length; i++) {
            var thisitem = template;
            thisitem = thisitem.replace("groupicon", grouplist.groups[i].avatar);
            thisitem = thisitem.replace("gname", grouplist.groups[i].gname).replace("gname", grouplist.groups[i].gname);
            thisitem = thisitem.replace("gdesc", grouplist.groups[i].gdesc);
            thisitem = thisitem.replace("templid", grouplist.groups[i].groupid);
            html += "<tr class=\"results\" style=\"display:none;\">" + thisitem + "</tr>";
        }
        $ektron(".WebCalByGroup .results").remove();
        $ektron(".WebCalByGroup table").append(html);
    },
    Paging: function(grouplist) {
        var showpaging = false;
        if (grouplist.pages > 1) {
            if (grouplist.thispage < grouplist.pages - 1) {
                showpaging = true;
                var next = $ektron(".WebCalByGroup .paging .next");
                next.unbind("click").bind("click", function() {
                    Ektron.PFWidgets.WebCalendar.Group.ExecuteSearch(grouplist.thispage + 1);
                });
                next.show();
            }
            if (grouplist.thispage > 0) {
                showpaging = true;
                var prev = $ektron(".WebCalByGroup .paging .prev");
                prev.unbind("click").bind("click", function() {
                    Ektron.PFWidgets.WebCalendar.Group.ExecuteSearch(grouplist.thispage - 1);
                });
                prev.show();
            }
        }
    },
    ExecuteSearch: function(page) {
        Ektron.PFWidgets.WebCalendar.Group.GroupSearch($ektron(".WebCalByGroup .searcharea .Execute")[0], page);
    },
    GroupSearch: function(el, page) {
        if (!page) page = 0;
        $ektron(".WebCalByGroup .center, .WebCalByGroup .results, .WebCalByGroup .paging span span").hide();
        $ektron(".WebCalByGroup .Searching").show();
        $el = $ektron(el);
        var searchtext = $el.parent().find(".GroupName").val();
        if (searchtext != "") {
            $ektron.ajax({
                type: "POST",
                cache: false,
                async: false,
                url: Ektron.PFWidgets.WebCalendar.webserviceURL,
                data: { "request": Ektron.PFWidgets.WebCalendar.createRequestObj("getgroups", page, searchtext, 0) },
                success: function(msg) {
                    $ektron(".WebCalByGroup .center, .WebCalByGroup .results").hide();
                    try {
                        var grouplist = eval("(" + msg + ")");
                        if (grouplist.groups.length == 0) {
                            $ektron(".WebCalByGroup .noResults").show();
                        } else {
                            Ektron.PFWidgets.WebCalendar.Group.Output(grouplist);
                            $ektron(".WebCalByGroup .results").show();
                        }
                        Ektron.PFWidgets.WebCalendar.Group.Paging(grouplist);
                    } catch (ex) {
                        $ektron(".WebCalByGroup .noResults").show();
                    }
                }
            });
        } else {
            $ektron(".WebCalByGroup .center, .WebCalByUser .results").hide();
            $ektron(".WebCalByGroup .noResults").show();
        }
    }
};