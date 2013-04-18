Ektron.ready(function() {
    Ektron.Workarea.Paging.init();
});

//define Ektron object only if it's not already defined
if (Ektron === undefined) {
    Ektron = {};
}

//define Ektron.Workarea object only if it's not already defined
if (Ektron.Workarea === undefined) {
    Ektron.Workarea = {};
}

//define Ektron.Workarea.Paging object only if it's not already defined
if (Ektron.Workarea.Paging === undefined) {
    Ektron.Workarea.Paging = {
        bindEvents: function () {
            var numericFields = $ektron("div.paging input.currentPage");
            numericFields.unbind();

            //validate code on keypress/keydown depending on browser
            //ensure numerics only in dollar edit field & two digits only in cents feild
            if (!$ektron.browser.msie) {
                numericFields.bind("keypress", function (e) {

                    var charCode;
                    e = (e) ? e : window.event;
                    charCode = (e.which !== null) ? e.which : e.keyCode;

                    //allow ctrl+x and ctrl+v
                    if ((e.ctrlKey && charCode == 118) || (e.ctrlKey && charCode == 120)) {
                        return true;
                    } else {
                        //charCode 8 == BACKSPACE, 
                        //charCode 48-58 == [0-9],
                        //charCode 91-106 == num lock + [0-9],
                        //charCode 37 == Arrow Left, 
                        //charCode 39 == Arrow Right, 
                        //charCode 9 == TAB
                        if ((charCode == 8) || (charCode == 9) || (charCode >= 48 && charCode <= 57)) {
                            return true;
                        } else {
                            return false;
                        }
                    }
                });
            } else {
                numericFields.bind("keydown", function (e) {
                    var charCode;
                    e = (e) ? e : window.event;
                    charCode = (e.which !== null) ? e.which : e.keyCode;

                    //allow ctrl+x and ctrl+v
                    if ((e.ctrlKey && charCode == 88) || (e.ctrlKey && charCode == 86)) {
                        return true;
                    } else {
                        if (e.shiftKey) {
                            //do not allow shift+numeric keys
                            return false;
                        }
                        //charCode 8 == BACKSPACE, charCode 48-58 == [0-9],charCode 37 == Arrow Left, charCode 39 == Arrow Right, charCode 9 == TAB
                        if ((charCode == 8) || (charCode == 9) || (charCode >= 48 && charCode <= 57) || (charCode == 37) || (charCode == 39) || (charCode >= 91 && charCode <= 105)) {
                            return true;
                        } else {
                            return false;
                        }
                    }
                });
            }
        },
        click: function (ui) {
            var selectedPage = Ektron.Workarea.Paging.selectedpage(ui);
            $ektron(ui).parents("div.paging").find("input.selectedPage").attr("value", selectedPage);
        },
        selectedpage: function (ui) {
            var selectedPage;
            var currentPageIndex = parseInt($ektron(ui).parents("div.paging").find("input.currentPageIndex").attr("value"), 10);
            var adHocPage = parseInt($ektron(ui).parents("div.paging").find("input.adHocPage").attr("value"), 10) - 1; //fix zero index
            var totalPages = parseInt($ektron(ui).parents("div.paging").find("input.totalPages").attr("value"), 10);
            var requestType = $ektron(ui).attr("class");

            switch (requestType) {
                case "FirstPage":
                    selectedPage = 0;
                    break;
                case "PreviousPage":
                    selectedPage = currentPageIndex - 1 <= 0 ? 0 : currentPageIndex - 1;
                    break;
                case "NextPage":
                    selectedPage = currentPageIndex + 1 >= totalPages ? totalPages : currentPageIndex + 1;
                    break;
                case "LastPage":
                    selectedPage = totalPages;
                    break;
                case "AdHoc":
                    var isAdHocPageOk = false;
                    if (adHocPage >= totalPages && !isAdHocPageOk) {
                        selectedPage = totalPages;
                        isAdHocPageOk = true;
                    }
                    if (adHocPage <= 0 && !isAdHocPageOk) {
                        selectedPage = 0;
                        isAdHocPageOk = true;
                    }
                    if (!isAdHocPageOk) {
                        selectedPage = adHocPage;
                    }
                    break;
            }
            return selectedPage;
        },
        init: function () {
            //init bind events
            Ektron.Workarea.Paging.bindEvents();
        }
    };
}