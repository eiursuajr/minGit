Ektron.ready(function() {
	Ektron.Commerce.Coupons.List.init();
	var msAjax = Sys.WebForms.PageRequestManager.getInstance();
	if (msAjax != null) {
	    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.Commerce.Coupons.List.initAfterAjax);
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

//define Ektron.Commerce.Coupons.List object only if it's not already defined
if (Ektron.Commerce.Coupons.List === undefined) {
	//Ektron Commerce Coupons CouponList Object
	Ektron.Commerce.Coupons.List = {
	    Actions: {
	        checkSearchState: function() {
		        $ektron("#txtSearch").clearInputLabel();
		    },
	        confirmSave: function(){
	            //get clone row
	            var cloneRow = $ektron("#EktronCouponConfirmSave div.couponsMarkedForDelete table tbody tr.cloneRow");
	            var confirmationTableTbody = cloneRow.parent();

	            //populate table with coupons that have been marked for delete
	            for (var i=0;i<Ektron.Commerce.Coupons.List.data.length;i++) {
	                var couponRow = cloneRow.clone(true);
	                if (!(i % 2) == 0)
	                    couponRow.addClass("stripe");
		            couponRow.children("td.id").html(Ektron.Commerce.Coupons.List.data[i].Id);
		            couponRow.children("td.code").html(Ektron.Commerce.Coupons.List.data[i].Code);
		            couponRow.children("td.currencyName").html(Ektron.Commerce.Coupons.List.data[i].CurrencyName);
		            couponRow.children("td.description").html(Ektron.Commerce.Coupons.List.data[i].Description);
		            couponRow.find("td.restore input.id").attr("value", Ektron.Commerce.Coupons.List.data[i].Id);
		            couponRow
		                .removeClass("cloneRow")
		                .addClass("couponRowMarkedForDelete");
		            confirmationTableTbody.append(couponRow);
		        }

	            //set modal header
	            $ektron("#EktronCouponListModal div.header span.modalHeader").hide();
	            $ektron("#EktronCouponListModal div.header span#confirmSaveHeader").show();

	            //set modal body
	            $ektron("#EktronCouponListModal div.body div.modalBody").hide();
	            $ektron("#EktronCouponListModal div.body div#EktronCouponConfirmSave").show();

	            //set modal footer
	            $ektron("#EktronCouponListModal div.footer div.modalFooter").hide();
	            $ektron("#EktronCouponListModal div.footer div#confirmSaveFooter").show();

	            //show confirmation modal
	            $ektron("#EktronCouponListModal").modalShow();

	            //prevent link button from posting back
	            return false;
	        },
	        markForDelete: function(el) {
	            var buttonDelete = $ektron(el);
	            var buttonRestore = buttonDelete.next();
	            var couponDataCell = buttonDelete.parent();

	            //toggle mark for delete/restore images
	            buttonDelete.hide();
	            buttonRestore.show();
	            buttonDelete.parents("tr").find("td.couponData").addClass("markedForDelete");

	            //mark row for delete
	            couponDataCell.find("input.markedForDelete").attr("value", "true");

	            //update json data
	            Ektron.Commerce.Coupons.List.Data.add(couponDataCell);

	            //toggle save and cancel buttons
	            Ektron.Commerce.Coupons.List.Actions.toggleSaveAndCancelButtons();

	            //toggle mark all for delete and restore all buttons if necessary
                Ektron.Commerce.Coupons.List.Actions.toggleHeaderMarkAllForDeleteAndMarkAllForRestoreButtons();
	        },
	        markForDeletePage: function() {
	            //mark all coupons for delete on current page only
	            $ektron("table.coupons tbody td.couponActions input.markedForDelete[value='false']").each(function(i){
	                //pass the restore image to the restore fuction to initiate the restore
	                Ektron.Commerce.Coupons.List.Actions.markForDelete($ektron(this).nextAll("img.markForDelete"));
	            });

	            //toggle header mark for delete / restore button
	            var headerButtonDelete = $ektron("table.coupons thead th.markForDelete img.markForDelete");
	            var headerButtonRestore = $ektron("table.coupons thead th.markForDelete img.restore");
	            headerButtonDelete.hide();
	            headerButtonRestore.show();
	        },
	        restore: function(el) {
	            var buttonRestore = $ektron(el);
	            var buttonDelete = buttonRestore.prev();
	            var couponDataCell = buttonDelete.parent();

	            //toggle mark for delete/restore images
	            buttonDelete.show();
	            buttonRestore.hide();
	            buttonRestore.parents("tr").find("td.couponData").removeClass("markedForDelete");

	            //unmark row for delete
	            couponDataCell.find("input.markedForDelete").attr("value", "false");

	            //update json data
	            Ektron.Commerce.Coupons.List.Data.remove(couponDataCell);

	            //toggle save and cancel buttons
	            Ektron.Commerce.Coupons.List.Actions.toggleSaveAndCancelButtons();

	            //toggle mark all for delete and restore all buttons if necessary
                Ektron.Commerce.Coupons.List.Actions.toggleHeaderMarkAllForDeleteAndMarkAllForRestoreButtons();
	        },
	        restorePage: function() {
	            //restore all coupons on current page only
	            $ektron("table.coupons tbody td.couponActions input.markedForDelete[value='true']").each(function(i){
	                //pass the restore image to the restore fuction to initiate the restore
	                Ektron.Commerce.Coupons.List.Actions.restore($ektron(this).nextAll("img.restore"));
	            });

	             //toggle header mark for delete / restore button
	            var headerButtonDelete = $ektron("table.coupons thead th.markForDelete img.markForDelete");
	            var headerButtonRestore = $ektron("table.coupons thead th.markForDelete img.restore");
	            headerButtonDelete.show();
	            headerButtonRestore.hide();
	        },
	        toggleHeaderMarkAllForDeleteAndMarkAllForRestoreButtons: function() {
	            //if all coupons in page are marked for delete, toggle header button to show "restore all" instead of "mark all for delete"
		        var totalCouponRows = $ektron("table.coupons tbody tr").length;
		        var totalCouponRowsMarkedForDelete = $ektron("table.coupons tbody td.couponActions input.markedForDelete[value='true']").length;

		        var headerButtonDelete = $ektron("table.coupons thead th.markForDelete img.markForDelete");
	            var headerButtonRestore = $ektron("table.coupons thead th.markForDelete img.restore");
		        if (totalCouponRows === totalCouponRowsMarkedForDelete) {
	                headerButtonDelete.hide();  //hide mark all for delete
	                headerButtonRestore.show(); //show restore all
		        } else {
		            headerButtonDelete.show(); //show mark all for delete
	                headerButtonRestore.hide(); //hide restore all
		        }
	        },
	        toggleSaveAndCancelButtons: function(){
	            if (Ektron.Commerce.Coupons.List.data.length > 0) {
	                //at least one item is marked for delete - show save and cancel buttons
	                $ektron("div.couponList div.actions ul li.save").css("display", "inline");
	                $ektron("div.couponList div.actions ul li.cancel").css("display", "inline");
	            } else {
	                //nothing is marked for delete - hide save and cancel
	                $ektron("div.couponList div.actions ul li.save").css("display", "none");
	                $ektron("div.couponList div.actions ul li.cancel").css("display", "none");
	            }
	        }
	    },
	    bindEvents: function(){
	        //highlight table row on hover
	        $ektron("div.couponList table.coupons tbody tr").hover(
                function(){
                    $ektron(this).children("td").addClass("hover");
                },
                function(){
                    $ektron(this).children("td").removeClass("hover");
                }
            );

	        //redirect page to coupon detail view when user clicks on coupon row
	        $ektron("div.couponList table.coupons tr td.couponData").bind("click", function(){
	            var couponId = $ektron(this).parent().find("input.id").attr("value");
	            if (Ektron.Commerce.Coupons.List.data.length > 0)
	            {
	                //coupons have been marked for delete - confirm leave page before redirect
	                $ektron("#EktronCouponConfirmViewCouponDetail tfoot p.modalActions input.id").attr("value", couponId);
	                $ektron("#EktronCouponConfirmViewCouponDetail").modalShow();
	            } else {
	                //no coupons have been marked for delete - proceed with redirect
	                Ektron.Commerce.Coupons.List.Modal.ViewCouponDetail.go(couponId);
	            }
            });
	    },
	    Data: {
	        add: function(couponDataCell) {
			    //retrieve json values
                var id = couponDataCell.find("input.id").attr("value");
                var isEnabled = couponDataCell.find("input.isEnabled").attr("value");
                var code = couponDataCell.find("input.code").attr("value");
                var currencyId = couponDataCell.find("input.currencyId").attr("value");
                var currencyName = couponDataCell.find("input.currencyName").attr("value");
                var description = couponDataCell.find("input.description").attr("value");
                var startDate = couponDataCell.find("input.startDate").attr("value");
                var expirationDate = couponDataCell.find("input.expirationDate").attr("value");
                var markedForDelete = couponDataCell.find("input.markedForDelete").attr("value");

			    //create json object
			    var jsonData = {
				    Id: id,
				    IsEnabled: isEnabled,
				    Code: code,
				    CurrencyId: currencyId,
				    CurrencyName: currencyName,
				    Description: description,
				    StartDate: startDate,
				    ExpirationDate: expirationDate,
				    MarkedForDelete: markedForDelete
			    }

			    //populate json data array with item values
			    Ektron.Commerce.Coupons.List.data.push(jsonData);

			    //populate items input field with stringified json data
			    $ektron("div.couponList input.CouponListClientData").attr("value", Ektron.JSON.stringify(Ektron.Commerce.Coupons.List.data));

			    //alert($ektron("div.couponList input.CouponListClientData").attr("value"));
		    },
		    matchMarkedForDeleteAfterAjax: function(){
		        //get deleted coupon ids, and populate deletedCouponIds array
		        var deletedCouponIdsArray = new Array();
		        for (var i=0;i<Ektron.Commerce.Coupons.List.data.length;i++) {
		            deletedCouponIdsArray.push(Ektron.Commerce.Coupons.List.data[i].Id)
		        }

		        //transform deletedCouponIds array into object so we can use "in operator"
		        var deletedCouponIdsObject = {}
		        for (var i=0;i<deletedCouponIdsArray.length;i++) {
		            //set the property to be the id, and value to blank (we don't need a value in this case)
		            deletedCouponIdsObject[deletedCouponIdsArray[i]] = '';
		        }

		        //loop through displayed coupons and compare to those already marked for delete
		        var couponRows = $ektron("div.couponList table.coupons tr td.couponActions input.id");
		        couponRows.each(function(x){
		            var couponId = $ektron(this).attr("value");
		            //use "in" operator to test marked for delete data for coupon id
		            if (couponId in deletedCouponIdsObject){
		                //row has been marked for delete - update UI accordingly

		                //first, ensure row data does not appear in Ektron.Commerce.Coupons.List.data twice
		                //simulate restore button click - pass img element to restore()
		                //this removes the row from the array if the row has been marked for delete and nothing if it hasn't
		                Ektron.Commerce.Coupons.List.Actions.restore($ektron(this).nextAll("img.restore"));

		                //second, re-add row data to Ektron.Commerce.Coupons.List.data
		                //simulate markForDelete button click - pass img element to markedForDelete()
		                //this not only re-adds the row to Ektron.Commerce.Coupons.List.data, but also
		                //updates the UI to grey out row, and sets the restore button to display:block
		                Ektron.Commerce.Coupons.List.Actions.markForDelete($ektron(this).nextAll("img.markForDelete"));
		            }
		        });

		        //toggle mark all for delete and restore all buttons if necessary
                Ektron.Commerce.Coupons.List.Actions.toggleHeaderMarkAllForDeleteAndMarkAllForRestoreButtons();

		        //alert($ektron("div.couponList input.CouponListClientData").attr("value"));
		    },
		    remove: function(couponDataCell) {

		        //get id of row to remove from json data
		        var couponId = couponDataCell.find("input.id").attr("value");

		        //loop through json array to get row index
		        var couponRowIndex = -1;
		        for (var i=0;i<Ektron.Commerce.Coupons.List.data.length;i++)
		        {
		            if (Ektron.Commerce.Coupons.List.data[i].Id === couponId) {
		                couponRowIndex = i;
		                break;
		            }
		        }
		        if (couponRowIndex > -1) {
		            Ektron.Commerce.Coupons.List.data.splice(couponRowIndex, 1);
		        }

		        //populate items input field with stringified json data
			    $ektron("div.couponList input.CouponListClientData").attr("value", Ektron.JSON.stringify(Ektron.Commerce.Coupons.List.data));

			    //alert($ektron("div.couponList input.CouponListClientData").attr("value"));
		    }
	    },
	    escapeAndEncode: function(string) {
		    return string
			    .replace(/&/g, "&amp;")
			    .replace(/</g, "&lt;")
			    .replace(/>/g, "&gt;")
			    .replace(/'/g, "\'")
			    .replace(/\"/g, "\"")
	    },
		init: function(){
		     //initialize Json data as array
	        Ektron.Commerce.Coupons.List.data = new Array();

	        //finish initialization
			Ektron.Commerce.Coupons.List.initAfterAjax();

			$ektron("#txtSearch").inputLabel();
		},
		initAfterAjax: function(){
		    //bind events
			Ektron.Commerce.Coupons.List.bindEvents();

			//match rows marked for delete, and mark them for delete
			Ektron.Commerce.Coupons.List.Data.matchMarkedForDeleteAfterAjax();

			//initialize modal
	        Ektron.Commerce.Coupons.List.Modal.init();

			//re-initialinze ektron grid
			Ektron.Workarea.Grids.init();

			$ektron("#txtSearch").inputLabel();
		},
		Modal: {
		    Cancel: {
		        no: function(){
		            Ektron.Commerce.Coupons.List.Modal.hide('EktronCouponListModal');
		        },
		        yes: function(mode){
		            //restore all coupons marked for delete in two steps
		            //Step 1 - restore all coupons on the page that is currenly displayed
		            $ektron("table.coupons tbody td.couponActions input.markedForDelete[value='true']").each(function(i){
		                //pass the restore image to the restore fuction to initiate the restore
		                Ektron.Commerce.Coupons.List.Actions.restore($ektron(this).nextAll("img.restore"));
		            });
		            //Step 2 - re-initialize the array (thereby restoring any coupons marked for delete on other pages)
		            Ektron.Commerce.Coupons.List.data = new Array();
		            $ektron('#EktronCouponListModal').modalHide();
		        },
		        show: function(){
		             //set modal header
                    $ektron("#EktronCouponListModal div.header span.modalHeader").hide();
                    $ektron("#EktronCouponListModal div.header span#confirmCancelHeader").show();

                    //set modal body
                    $ektron("#EktronCouponListModal div.body div.modalBody").hide();
                    $ektron("#EktronCouponListModal div.body div#EktronCouponConfirmCancel").show();

                    //set modal footer
                    $ektron("#EktronCouponListModal div.footer div.modalFooter").hide();
                    $ektron("#EktronCouponListModal div.footer div#confirmCancelFooter").show();

                    //show confirmation modal
                    $ektron("#EktronCouponListModal").modalShow();
                }
		    },
		    hide: function(modal) {
		        $ektron('#' + modal).modalHide();
		    },
		    init: function(){
		        $ektron(".ektronModalClose").hover(
		            function(){
		                $ektron(this).addClass("ui-state-hover");
		            },
		            function(){
		                $ektron(this).removeClass("ui-state-hover");
		            }
		        );

                var modal = $ektron("#EktronCouponListModal");
                modal.drag('.header');
                modal.modal({
                    toTop: true,
                    modal: true,
		            overlay: 0,
                    onShow: function(hash) {
			            hash.o.fadeTo("fast", 0.5, function(){
		                    hash.w.fadeIn("fast");
	                    });
                    },
                    onHide: function(hash) {
                        hash.w.fadeOut("fast");
			            hash.o.fadeOut("fast", function(){
				            if (hash.o)
					            hash.o.remove();
			            });
                    }
                });
		    },
		    Save: {
		        cancel: function(){
		            //delete all rows (except clone row) from save confirmation modal
		            $ektron("table.couponsMarkedForDelete tbody tr.couponRowMarkedForDelete").each(function(i){
		                $ektron(this).remove();
		            });

		            //hide modal
		            Ektron.Commerce.Coupons.List.Modal.hide('EktronCouponListModal');
		        },
		        ok: function() {
		            //user has selected "ok" to submit form - hide modal
		            Ektron.Commerce.Coupons.List.Modal.hide('EktronCouponListModal');

		            //fire postback
		            __doPostBack('lbSave','');
		        },
		        restore: function(elem)
		        {
		            //remove row from modal
	                var restoredCouponRow = $ektron(elem).parents("tr.couponRowMarkedForDelete")
	                var tbody = restoredCouponRow.parent();
	                restoredCouponRow.remove();

	                //re-do striping
	                if (tbody.children("tr.couponRowMarkedForDelete").length > 0){
		                tbody.children("tr.couponRowMarkedForDelete").removeClass("stripe");
		                tbody.children("tr.couponRowMarkedForDelete:odd").addClass("stripe");
	                }

	                //if user restored the only coupon marked for delete, hide modal
	                var couponsMarkedForDelete = tbody.find("tr.couponRowMarkedForDelete");
	                if (couponsMarkedForDelete.length === 0 ) {
	                    Ektron.Commerce.Coupons.List.Modal.hide('EktronCouponListModal');
	                }

	                //find row in non-modal (main) table - if there, toggle restore
		            var couponId = $ektron(elem).prev().attr("value");
		            var selector = "input.id[value='" + couponId + "']";
		            var restoredCoupon = $ektron(selector);

		            //mark row in non-modal (main) table as restored
		            restoredCoupon.nextAll("input.markedForDelete").attr("value", "false");

		            //toggle row buttons/class in non-modal (main) table
		            var el = restoredCoupon.nextAll("img.restore");
		            var buttonRestore = $ektron(el);
	                var buttonDelete = buttonRestore.prev();

	                //toggle mark for delete/restore images
	                buttonDelete.show();
	                buttonRestore.hide();
	                buttonRestore.parents("tr").find("td.couponData").removeClass("markedForDelete");

		            //update json data
		            var couponDataCell = buttonRestore.parent();
	                Ektron.Commerce.Coupons.List.Data.remove(couponDataCell);

	                //toggle save and cancel buttons
	                Ektron.Commerce.Coupons.List.Actions.toggleSaveAndCancelButtons();

	                //toggle mark all for delete and restore all buttons if necessary
                    Ektron.Commerce.Coupons.List.Actions.toggleHeaderMarkAllForDeleteAndMarkAllForRestoreButtons();
		        }
		    },
		    show: function(modal) {
		        $ektron('#' + modal).modalShow();
		    },

		    ViewCouponDetail: {
		        go: function(couponId){
		            window.location = "../Properties/properties.aspx?couponId=" + couponId;
		        }
		    }
		}
	}
}