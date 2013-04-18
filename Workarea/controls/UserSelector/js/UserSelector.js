if ("undefined" === typeof(Ektron)) { Ektron = {}; }
if ("undefined" === typeof(Ektron.Controls)) { Ektron.Controls = {}; }
if ("undefined" === typeof(Ektron.Controls.UserSelector)) {
    Ektron.Controls.UserSelector = {
        userLists: {},

        /*
        * init
        *
        * Initializes a user selector control. 
        * 
        * id (string):                  User list identifier.
        * handlerPath (string):         Path to request handler for the 
        *                               autocomplete search box.
        * minimumSearchLength (number): Minimum number of characters to type 
        *                               in the autocomplete box before 
        *                               polling the server for search results.
        * searchFieldName (string):     User property on which to search for 
        *                               results.
        */
        init: function(id, handlerPath, minimumSearchLength, searchFieldName, customParameters) {
            var $widget = $("#" + id);
            var $ucUserName = $widget.find('.username');
            var $ids = $widget.find("input[type='hidden']");
            var $users = $widget.find(".users");

            // userList object provides an interface for managing
            // selected users. this class stores the users in an array,
            // updates the display list, and provides methods for 
            // serializing the stored data.
            var userList = Ektron.Controls.UserSelector.userLists[id] =
            {
                _$element: null,
                _users: {},
                init: function($output) {
                    userList._$element = $output;

                    var users = eval($ids.val());

                    for (var i in users) {
                        userList.add(users[i]);
                    }
                },
                clear: function() {
                    $users.find("li.user").remove();
                    userList._users = {};
                    userList._$element.val("");
                },
                add: function(user) {
                    if (!userList.contains(user.Id)) {
                        userList._users[user.Id] = user;
                        $("<li></li>")
                            .addClass("user")
                            .attr("data-userId", user.Id)
                            .html(user["Username"])
                            .click(function() { userList.remove(user.Id); })
                            .appendTo($users);
                        userList._$element.val(userList.serialize());
                    }
                },
                contains: function(userId) {
                    return ("undefined" !== typeof (userList._users[userId]));
                },
                remove: function(userId) {
                    $users.find("li.user[data-userId='" + userId + "']").remove();
                    delete (userList._users[userId]);
                    userList._$element.val(userList.serialize());
                },
                serialize: function() {
                    var ids = [];
                    for (var i in userList._users) {
                        ids.push(userList._users[i]);
                    }
                    return JSON.stringify(ids);
                }
            };

            userList.init($ids);

            $ucUserName.autocomplete({
                source: Ektron.Controls.UserSelector.search(handlerPath, customParameters, searchFieldName),
                search: function() {
                    if (this.value.length < minimumSearchLength) {
                        return false;
                    }
                },
                focus: function() {
                    return false;
                },
                select: function(event, ui) {
                    this.value = "";
                    userList.add(ui.item.value);

                    return false;
                }
            });

            var $addUserDialog = $widget.find(".userDialog");

            $addUserDialog.dialog({
                autoOpen: false,
                modal: true,
                buttons: {
                    "Cancel": function() {
                        $(this).dialog("close");
                    }
                }
            });

            $widget.find(".usernameWrapper").click(function(evt) {
                Ektron.Controls.UserSelector.browseUsers(handlerPath, customParameters, $addUserDialog,
                function(user) {
                    userList.add(user);
                });
            });

            $widget.find(".username").click(function(evt) { evt.stopPropagation(); });
        },

        browseUsers: function(handlerPath, customParameters, $addUserDialog, callback) {
            var $addUserList = $addUserDialog.find(".users");

            $addUserDialog.dialog("open");

            var pageIndex = 1;

            var $previousPage = $addUserDialog.find(".previousPage");
            var $nextPage = $addUserDialog.find(".nextPage");
            var $go = $addUserDialog.find(".go");
            var $firstPage = $addUserDialog.find(".firstPage");
            var $lastPage = $addUserDialog.find(".lastPage");

            var $pageCount = $addUserDialog.find(".pageCount");
            var $currentPage = $addUserDialog.find(".currentPage");

            // page count ... need getter and setter to update page count display
            var pageCount = 1;

            var setPageCount = function(count) {
                pageCount = count;
                $pageCount.html(count);
            };

            var getPageCount = function() {
                return pageCount;
            };

            // current page ... need getter and setter to update current page display
            var currentPage = 1;

            var setCurrentPage = function(index) {
                currentPage = index;
                $currentPage.val(index);
            };

            var getCurrentPage = function() {
                return currentPage;
            };

            // selectPage ... retrieves the page from the web service and updates the display
            var selectPage = function(index) {
                Ektron.Controls.UserSelector.getPage(handlerPath, customParameters, index, 10, "Username",
                    function(response) {
                        $addUserList.html("");
                        setPageCount(Math.max(1, response.TotalPages));
                        setCurrentPage(Math.max(1, index));
                        $.each(response.Users, function(i) {
                            var user = response.Users[i];
                            var $user = $("<li></li>")
                                .addClass("user")
                                .attr("data-userData", JSON.stringify(user))
                                .click(function() {
                                    callback(user);
                                    $addUserDialog.dialog('close');
                                })
                                .html(user.Username);

                            $addUserList.append($user);
                        });
                    });
            };

            // wire up paging events
            $previousPage.click(function(evt) {
                evt.preventDefault(true);
                var currentPage = getCurrentPage();
                if (currentPage > 1) {
                    selectPage(currentPage - 1);
                }
            });

            $nextPage.click(function(evt) {
                evt.preventDefault(true);
                var currentPage = getCurrentPage();
                if (currentPage < getPageCount()) {
                    selectPage(currentPage + 1);
                }
            });

            $firstPage.click(function(evt) {
                evt.preventDefault(true);
                selectPage(1);
            });

            $lastPage.click(function(evt) {
                evt.preventDefault(true);
                selectPage(getPageCount());
            });

            $go.click(function(evt) {
                evt.preventDefault(true);
                var targetPage = $currentPage.val();

                if (Ektron.Controls.UserSelector.isNumeric(targetPage)) {
                    selectPage(Math.min(Math.max(1, targetPage), getPageCount()));
                }
                else {
                    alert("Must enter a valid page number.");
                }
            });

            selectPage(pageIndex);
        },

        search: function(handlerPath, customParameters, searchFieldName) {
            return function(request, response) {
                var queryString = $.extend(customParameters, {
                    action: "search",
                    query: request.term,
                    field: searchFieldName
                });
                $.getJSON(handlerPath, queryString, function(data, textStatus) {
                    if (data.Success == true &&
		                "undefined" != typeof (data.Users)) {
                        var values = [];
                        for (var i in data.Users) {
                            var user = data.Users[i];
                            user.Username = user[searchFieldName];
                            values.push({ label: user[searchFieldName], value: user });
                        }
                        response(values);
                    } else {
                        alert(data.ErrorMessage);
                    }
                });
            }
        },

        getPage: function(handlerPath, customParameters, index, pageSize, orderBy, callback) {
            var queryString = $.extend(customParameters, {
                action: "getPage",
                index: index,
                pageSize: pageSize,
                orderBy: orderBy
            });

            $.getJSON(handlerPath, queryString, function(data, textStatus) {
                if (data.Success == true &&
	                "undefined" != typeof (data.Users)) {
                    callback(data);
                } else {
                    alert(data.ErrorMessage);
                }
            });
        },

        split: function(text) {
            return text.split(/,\s*/);
        },

        reinit: function(id, users) {
            var userList = Ektron.Controls.UserSelector.userLists[id];

            userList.clear();

            for (var i in users) {
                userList.add(users[i]);
            }
        },

        isNumeric: function(str) {
            return ('number' === typeof (str) || /^\d*$/.test(str));
        }
    };
}