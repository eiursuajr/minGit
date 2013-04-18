Ektron.ready(function() {
    Ektron.Controls.ClientPaging.init();
});

//define Ektron object only if it's not already defined
if (Ektron === undefined) {
    Ektron = {};
}

//define Ektron.Workarea object only if it's not already defined
if (Ektron.Controls === undefined) {
    Ektron.Controls = {};
}

//define Ektron.Workarea.Paging object only if it's not already defined
if (Ektron.Controls.ClientPaging === undefined) {
    Ektron.Controls.ClientPaging = {
        bindEvents: function() {
            var numericFields = $ektron("div.paging input.currentPage");
            numericFields.unbind();

            //validate code on keypress/keydown depending on browser
            //ensure numerics only in dollar edit field & two digits only in cents feild
            if (!$ektron.browser.msie) {
                numericFields.bind("keypress", function(e) {

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
                numericFields.bind("keydown", function(e) {
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
        click: function(ui) {
            // Ektron.Controls.ClientPaging.currentPageIndex = parseInt($ektron(ui).parents("div.paging").find("input.currentPageIndex").attr("value"), 10)
            var adHocPage = parseInt($ektron(ui).parents("div.paging").find("input.adHocPage").attr("value"), 10) - 1; //fix zero index
            //var totalPages = parseInt($ektron(ui).parents("div.paging").find("input.totalPages").attr("value"), 10);
            var requestType = $ektron(ui)[0].id;

            switch (requestType) {
                case "FirstPage":
                    Ektron.Controls.ClientPaging.selectedPage = 1;
                    break;
                case "PreviousPage":
                    Ektron.Controls.ClientPaging.selectedPage = Ektron.Controls.ClientPaging.selectedPage - 1 <= 1 ? 1 : Ektron.Controls.ClientPaging.selectedPage - 1;
                    break;
                case "NextPage":
                    Ektron.Controls.ClientPaging.selectedPage = Ektron.Controls.ClientPaging.selectedPage + 1 >= Ektron.Controls.ClientPaging.totalPages ? Ektron.Controls.ClientPaging.totalPages : Ektron.Controls.ClientPaging.selectedPage + 1;
                    break;
                case "LastPage":
                    Ektron.Controls.ClientPaging.selectedPage = Ektron.Controls.ClientPaging.totalPages;
                    break;
                case "AdHoc":
                    var isAdHocPageOk = false;
                    if (adHocPage >= totalPages && !isAdHocPageOk) {
                        Ektron.Controls.ClientPaging.selectedPage = Ektron.Controls.ClientPaging.totalPages;
                        isAdHocPageOk = true;
                    }
                    if (adHocPage <= 0 && !isAdHocPageOk) {
                        Ektron.Controls.ClientPaging.selectedPage = 0;
                        isAdHocPageOk = true;
                    }
                    if (!isAdHocPageOk) {
                        Ektron.Controls.ClientPaging.selectedPage = adHocPage;
                    }
                    break;
            }

            $ektron(ui).parents("div.paging").find("input.selectedPage").attr("value", Ektron.Controls.ClientPaging.selectedPage);

            Ektron.Controls.ClientPaging.setupNavigation();

            var data = { 'SelectedPage': Ektron.Controls.ClientPaging.selectedPage };
            $ektron(document).trigger("Ektron.Controls.ClientPaging.PageEvent", [data]);
        
            return false;
        },
        setupNavigation : function(){
        
            Ektron.Controls.ClientPaging.currentPageIndex = Ektron.Controls.ClientPaging.selectedPage;
            $ektron("span.pageNumber input.adHocPage").val(Ektron.Controls.ClientPaging.selectedPage);

            var firstPageEnabled = (Ektron.Controls.ClientPaging.totalPages == 0) || (Ektron.Controls.ClientPaging.selectedPage > 1);
            var previousPageEnabled = (Ektron.Controls.ClientPaging.totalPages == 0) || (Ektron.Controls.ClientPaging.selectedPage > 1);
            var nextPageEnabled = (Ektron.Controls.ClientPaging.totalPages == 0) || (Ektron.Controls.ClientPaging.currentPageIndex < Ektron.Controls.ClientPaging.totalPages);
            var lastPageEnabled = (Ektron.Controls.ClientPaging.totalPages == 0) || (Ektron.Controls.ClientPaging.currentPageIndex < Ektron.Controls.ClientPaging.totalPages);

            $ektron("div.paging li a").unbind('click');

            if (firstPageEnabled) {
                $ektron("li a#FirstPage").removeClass("disabled");
                $ektron("li a#FirstPage").bind("click", function() { Ektron.Controls.ClientPaging.click(this); });
            } else {
                $ektron("li a#FirstPage").addClass("disabled");
            }

            if (previousPageEnabled) {
                $ektron("li a#PreviousPage").removeClass("disabled");
                $ektron("li a#PreviousPage").bind("click", function() { Ektron.Controls.ClientPaging.click(this); });
            } else {
                $ektron("li a#PreviousPage").addClass("disabled");
            }

            if (nextPageEnabled) {
                $ektron("li a#NextPage").removeClass("disabled");
                $ektron("li a#NextPage").bind("click", function() { Ektron.Controls.ClientPaging.click(this); });
            } else {
                $ektron("li a#NextPage").addClass("disabled");
            }

            if (lastPageEnabled) {
                $ektron("li a#LastPage").removeClass("disabled");
                $ektron("li a#LastPage").bind("click", function() { Ektron.Controls.ClientPaging.click(this); });
            } else {
                $ektron("li a#LastPage").addClass("disabled");
            }
        },
        totalPages: 0,
        selectedPage: 0,
        currentPageIndex: 0,
        init: function() {
            //init bind events
            Ektron.Controls.ClientPaging.bindEvents();
        }
    };
}