(function($) {
    if ("undefined" === typeof (Ektron)) Ektron = {};
    if ("undefined" === typeof (Ektron.Controls)) Ektron.Controls = {};
    if ("undefined" === typeof (Ektron.Controls.TodoList)) {
        Ektron.Controls.TodoList = {
            init: function(id, baseURL) {
                var $todoList = $("div#" + id + ".todoList");

                // Initialize the buttons
                $todoList.find("div.actions a.addTodo").button();

                $todoList.find("table.todoItems tr.todoItemComments a.post")
                        .button()
                        .removeClass("ui-corner-all")
			            .addClass("ui-corner-right");

                // Convert the select list into an autocompleting combobox
                $todoList.find("div.actions select.filter").combobox();

                var $tableItemRows = $todoList.find("tr.todoItemData");

                // Add the sliding comment functionality
                $tableItemRows.each(function(i) {
                    var $item = $(this);

                    var $commentsButton = $item.find("a.comments");
                    $commentsButton.click(function(evt) {
                        evt.preventDefault();
                        var $todoItemComments = $item.next("tr.todoItemComments");

                        if ($todoItemComments.is(":hidden")) {
                            $todoItemComments.show();
                            $todoItemComments.find('ul.comments').slideDown('fast');
                        } else {

                            $todoItemComments.find('ul.comments').slideUp('fast', function() {
                                $todoItemComments.hide();
                            });
                        }
                    });
                });

                // Initialize the dialogs
                var $addDialog = $todoList.find(".addTodo.dialog");
                var $addTodoItem = $addDialog.find("input.addTodoItem");
                var $addDueDate = $addDialog.find(".dueDate").datepicker({ minDate: 0, buttonImage: baseURL + '/css/images/calendar.png', buttonImageOnly: true, showOn: 'both' });
                $addDialog.find(".startDate").datepicker({
                    onSelect: function(dateText, inst) {
                        $addDueDate.datepicker("option", "minDate", new Date(dateText));
                    }, buttonImage: baseURL + '/css/images/calendar.png', buttonImageOnly: true, showOn: 'both'
                });
                $addDialog.dialog({
                    modal: true,
                    autoOpen: false,
                    buttons: {
                        "Create": function() {
                            $addTodoItem.trigger('click');
                        },

                        "Cancel": function() {
                            $(this).dialog("close");
                        }
                    }
                });

                var $addTodoButton = $todoList.find("a.addTodo");
                $addTodoButton.click(function() {
                    $addDialog.parent().appendTo($('form:first'));
                    $addDialog.dialog("open");
                });

                var $editDialog = $todoList.find(".editTodo.dialog");
                var $editTodoItem = $editDialog.find("input.editTodoItem");
                $editDialog.find(".startDate").datepicker({
                    onSelect: function(dateText, inst) {
                        $editDueDate.datepicker("option", "minDate", new Date(dateText));
                    }, buttonImage: baseURL + '/css/images/calendar.png', buttonImageOnly: true, showOn: 'both'
                });
                var $editDueDate = $editDialog.find(".dueDate").datepicker({ minDate: $editDialog.find(".startDate").datepicker("getDate"), buttonImage: baseURL + '/css/images/calendar.png', buttonImageOnly: true, showOn: 'both' });

                $editDialog.dialog({
                    modal: true,
                    autoOpen: false,
                    buttons: {
                        "Modify": function() {
                            $editTodoItem.trigger('click');
                        },

                        "Cancel": function() {
                            $(this).dialog("close");
                        }
                    }
                });

                $tableItemRows.each(function(i) {
                    var $item = $(this);
                    var todoItemData = eval("(" + $item.attr("data-todoItem") + ")");
                    var $editLink = $item.find("td.title");

                    $editLink.click(function() {
                        // parse the dates
                        var startDate = Ektron.Controls.TodoList.textToDate(todoItemData.StartDate);
                        var dueDate = Ektron.Controls.TodoList.textToDate(todoItemData.DueDate);
                        var assignedTo = todoItemData.AssignedTo;

                        // set the default values
                        $editDialog.find("input[type='hidden']").val(todoItemData.Id);
                        $editDialog.find(".descriptionValidator,.titleValidator").hide();
                        $editDialog.find(".title").val(todoItemData.Title);
                        $editDialog.find(".description").val(todoItemData.Description);
                        $editDialog.find(".status").val(todoItemData.Status);
                        $editDialog.find(".priority").val(todoItemData.Priority);
                        $editDialog.find(".startDate").datepicker("setDate", startDate);
                        $editDialog.find(".dueDate")
                            .datepicker("setDate", dueDate)
                            .datepicker("option", "minDate", startDate);

                        // initialize the assignment list
                        var users = {};
                        for (var i in assignedTo) {
                            var assignment = assignedTo[i];

                            users[assignment.UserId] = { "Id": assignment.UserId, "Username": assignment.UserName };
                        }

                        Ektron.Controls.UserSelector.reinit($editDialog.find(".ektron.userSelector").attr("id"), users);

                        $editDialog.parent().appendTo($('form:first'));
						$ektron("#ui-datepicker-div").hide();
                        $editDialog.dialog("open");

                    });
                });
            },

            textToDate: function(text) {
                return eval('new' + text.replace(/\//g, ' '));
            }
        };
    }
})($ektron);