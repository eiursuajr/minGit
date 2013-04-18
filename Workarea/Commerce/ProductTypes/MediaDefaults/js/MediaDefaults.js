//Intialiaze MediaDefaults
Ektron.ready(function(){
	Ektron.Commerce.ProductTypes.MediaDefaults.init();
});

//Define Ektron object only if it's not already defined
if (Ektron === undefined) {
	Ektron = {};
}

//Define Ektron.Commerce object only if it's not already defined
if (Ektron.Commerce === undefined) {
	Ektron.Commerce = {};
}

//Define Ektron.Commerce.ProductTypes object only if it's not already defined
if (Ektron.Commerce.ProductTypes === undefined) {
	Ektron.Commerce.ProductTypes = {};
}

//Ektron MediaDefaults Object
if (Ektron.Commerce.ProductTypes.MediaDefaults === undefined) {
    Ektron.Commerce.ProductTypes.MediaDefaults = {
        Add: {
            add: function(el) {
                var button = $ektron(el);
                
                //verify fields aren't blank
                //verify name
                var input = $ektron("div#MediaDefaultsModal div.ui-dialog-content input.name");
                var name = input.attr("value");
                if (name == "" || "undefined" == name) {
                    alert(Ektron.Commerce.ProductTypes.MediaDefaults.LocalizedStrings.blankNameField);
                    return false;
                } else {
                     //Check for special characters
                    if (name.indexOf('>') > -1 || name.indexOf('<') > -1 || name.indexOf(';') > -1 || name.indexOf('/') > -1 || name.indexOf(':') > -1 || name.indexOf('*') > -1 || name.indexOf('?') > -1
                       || name.indexOf('\\\"') > -1 || name.indexOf('|') > -1 || name.indexOf('&') > -1 || name.indexOf('%') > -1 || name.indexOf('&') > -1 || name.indexOf('+') > -1)
                    {
                        alert(Ektron.Commerce.ProductTypes.MediaDefaults.LocalizedStrings.cannotContainSpecialCharacters);
                        return false;
                    }
                    //validate unique name
                    var dataName = "[filename]" + name;
                    var data = Ektron.JSON.parse($ektron("div.EktronMediaDefaults input.mediaDefaultsData").attr("value"));
                    for (var i=0; i < data.length; i++)
                    {
                        if (dataName == data[i].Name){
                            alert(Ektron.Commerce.ProductTypes.MediaDefaults.LocalizedStrings.duplicateNameField);
                            return false;
                        }
                    }
                }
                //verify width
                var width = $ektron("div#MediaDefaultsModal div.ui-dialog-content input.width").attr("value");
                if (width == "" || width == undefined) {
                    alert(Ektron.Commerce.ProductTypes.MediaDefaults.LocalizedStrings.blankNameField);
                    return false;
                }
                if(isNaN(width))
                { 
                    alert(Ektron.Commerce.ProductTypes.MediaDefaults.LocalizedStrings.cannotBeAlphabetic);
                    return false;
                }
                //verify height
                var height = $ektron("div#MediaDefaultsModal div.ui-dialog-content input.height").attr("value");
                if (height == "" || height == undefined) {
                    alert(Ektron.Commerce.ProductTypes.MediaDefaults.LocalizedStrings.blankNameField);
                    return false;
                }
                 if(isNaN(height))
                { 
                    alert(Ektron.Commerce.ProductTypes.MediaDefaults.LocalizedStrings.cannotBeAlphabetic);
                    return false;
                }
                //begin add - get and clone clonerow
                var tbody = $ektron("div.EktronMediaDefaults table tbody");
                var cloneRow = tbody.children("tr.mediaDefaultsCloneRow").clone(true);
                
                //set name fields
                cloneRow.find("td.name span.value").text(name);
                cloneRow.find("td.name input.name").attr("value", name);
                
                //set width fields
                cloneRow.find("td.width span.number").text(width);
                cloneRow.find("td.width input.width").attr("value", width);
                
                //set height fields
                cloneRow.find("td.height span.number").text(height);
                cloneRow.find("td.height input.height").attr("value", height);
                
                //set data fields
                cloneRow.find("input.id").attr("value", "0");
                cloneRow.find("input.name").attr("value", name);
                cloneRow.find("input.width").attr("value", width);
                cloneRow.find("input.height").attr("value", height);
                cloneRow.find("input.markedForDelete").attr("value", "false");      

                //hide empty row
                tbody.find("tr.mediaDefaultsEmptyRow").hide();

                //append cloned row
                cloneRow.removeClass("mediaDefaultsCloneRow").addClass("mediaDefault")
                tbody.append(cloneRow);
                		
				//hide modal
				Ektron.Commerce.ProductTypes.MediaDefaults.Modal.hide();
				
				//re-initialize json data array
				Ektron.Commerce.ProductTypes.MediaDefaults.Data.update();
            }
        },
        bindEvents: function(){
	       //ensure numerics only in numeric edit field
	        $ektron("div.EktronMediaDefaults input.numeric, div.EktronMediaDefaults div.mediaDefaultsModal input.height, div.EktronMediaDefaults div.mediaDefaultsModal input.width")
	            .bind("keypress", function(evt){
	                var evt = (evt) ? evt : window.event;
	                var charCode = (evt.which != null) ? evt.which : evt.keyCode;
	                return (charCode < 32 || (charCode >=48 && charCode <= 57))
	        }),
	        $ektron("div.EktronMediaDefaults input.width")
	            .bind("keypress", function(evt){
	                var evt = (evt) ? evt : window.event;
	                var charCode = (evt.which != null) ? evt.which : evt.keyCode;
	                return (charCode < 32 || (charCode >=48 && charCode <= 57))
	        }),
	        $ektron("div.EktronMediaDefaults input.height")
	            .bind("keypress", function(evt){
	                var evt = (evt) ? evt : window.event;
	                var charCode = (evt.which != null) ? evt.which : evt.keyCode;
	                return (charCode < 32 || (charCode >=48 && charCode <= 57))
	        });
	        
	    },
	    Buttons: {
	        cancel: function(el){
	            var button = $ektron(el).parent();
	            //toggle ok & cancel, hide mark for delete & restore
	            var wrapper = button.parents("tr.mediaDefault");
	            var IsMarked = wrapper.find("td.data input.markedForDelete").attr("value");
	            if (IsMarked != "true")
	            {
	            
	                button.parent().find("a.edit").show();
	                button.parent().find("a.markForDelete").show();
	                button.parent().find("a.restore").hide();
	                button.parent().find("a.ok").hide();
	                button.parent().find("a.cancel").hide();
    	                
	            }
	            
	            //toggle input and span
	            wrapper.find("td.name span").show();
	            wrapper.find("td.width span[class!='units']").show();
	            wrapper.find("td.height span[class!='units']").show();
	            wrapper.find("input").hide();
	            
	        },
	        edit: function(el) {
	            //cancel all other rows in edit mode
	            $ektron("div.EktronMediaDefaults p.actions a.cancel").trigger("click");
	            
	            var button = $ektron(el).parent();
	            //toggle ok & cancel, hide mark for delete & restore
	            button.parent().find("a.edit").hide();
	            button.parent().find("a.markForDelete").hide();
	            button.parent().find("a.restore").hide();
	            button.parent().find("a.ok").show();
	            button.parent().find("a.cancel").show();
	            
	            //toggle input and span
	            var wrapper = button.parents("tr.mediaDefault");
	            wrapper.find("td.name span.value").hide();
	            wrapper.find("td.width span.number").hide();
	            wrapper.find("td.height span.number").hide();
	            wrapper.find("input").show();
	        },
	        markedForDelete: function(el){
	           //get clicked button
	            var button = $ektron(el).parent();
	            
	            //update data value
	            var wrapper = button.parents("tr.mediaDefault");
	            var name = wrapper.find("td.data input.markedForDelete").attr("value", "true");
	            
	            //toggle ok & cancel, hide mark for delete & restore
	            button.parent().find("a.edit").hide();
	            button.parent().find("a.markForDelete").hide();
	            button.parent().find("a.restore").show();
	            button.parent().find("a.ok").hide();
	            button.parent().find("a.cancel").hide();
	            
	            //toggle input and span
	            wrapper.find("span").addClass("markedForDelete").show();
	            wrapper.find("span.example").addClass("markedForDelete");
	            wrapper.children("input").hide();
	            
	            //re-initialize json data array
				Ektron.Commerce.ProductTypes.MediaDefaults.Data.update();
	        },
	        ok: function(el){
	            //get clicked button
	            var button = $ektron(el).parent();
	        
	            //update data value
	            var wrapper = button.parents("tr.mediaDefault");
	            var name = wrapper.find("td.name input.name").attr("value");
	            var width = wrapper.find("td.width input.width").attr("value");
	            var height = wrapper.find("td.height input.height").attr("value");
	            
	            //if any are blank, set them back to what they were before
	            name = (name != "") ? name : wrapper.find("td.data input.name");
	            width = (width != "") ? width : wrapper.find("td.data input.width");
	            height = (height != "") ? height : wrapper.find("td.data input.height");
	            
	            //set spans and data input fields to proper values
	            wrapper.find("td.name span.value").text(name);
	            wrapper.find("td.width span.number").text(width);
	            wrapper.find("td.height span.number").text(height);
	            wrapper.find("td.data input.name").attr("value", name);
	            wrapper.find("td.data input.width").attr("value", width);
	            wrapper.find("td.data input.height").attr("value", height);
	            
	            //toggle ok & cancel, hide mark for delete & restore
	            button.parent().find("a.edit").show();
	            button.parent().find("a.markForDelete").show();
	            button.parent().find("a.restore").hide();
	            button.parent().find("a.ok").hide();
	            button.parent().find("a.cancel").hide();
	            
	            //toggle input and span
	            var wrapper = button.parents("tr.mediaDefault");
	            wrapper.find("td.name span").show();
	            wrapper.find("td.width span[class!='units']").show();
	            wrapper.find("td.height span[class!='units']").show();
	            wrapper.find("input").hide();
	            
	            //re-initialize json data array
				Ektron.Commerce.ProductTypes.MediaDefaults.Data.update();
	        },
	        restore: function(el){
	            //get clicked button
	            var button = $ektron(el).parent();
	            
	            //update data value
	            var wrapper = button.parents("tr.mediaDefault");
	            var name = wrapper.find("td.data input.markedForDelete").attr("value", "false");
	            
	            //toggle ok & cancel, hide mark for delete & restore
	            button.parent().find("a.edit").show();
	            button.parent().find("a.markForDelete").show();
	            button.parent().find("a.restore").hide();
	            button.parent().find("a.ok").hide();
	            button.parent().find("a.cancel").hide();
	            
	            //toggle input and span
	            wrapper.find("span").removeClass("markedForDelete").show();
	            wrapper.find("span.example").removeClass("markedForDelete");
	            wrapper.children("input").hide();
	            
	            //re-initialize json data array
				Ektron.Commerce.ProductTypes.MediaDefaults.Data.update();
	        }
	    },
	    Data: {
			init: function(){
				//initialize Json data as array
				var data = new Array();
				
				//populate Json data
				$ektron("div.EktronMediaDefaults table tbody tr.mediaDefault").each(function(i){
				
					//retrieve json values
					var id = $ektron(this).find("td.data input.id").val();
					var name = "[filename]" + $ektron(this).find("td.data input.name").val();
					var width = $ektron(this).find("td.data input.width").val();
					var height = $ektron(this).find("td.data input.height").val();
					var markedForDelete = $ektron(this).find("td.data input.markedForDelete").val();
					
					//create json object
					var MediaDefaultsData = {
						Order: i,
						Id: id,
						Name: name,
						Width: width,
						Height: height,
						MarkedForDelete: markedForDelete
					}
					
					//populate json data array with item values
					data.push(MediaDefaultsData);
				});
				
				//populate items input field with stringified json data
				$ektron("div.EktronMediaDefaults input.mediaDefaultsData").val(Ektron.JSON.stringify(data));
				
				//alert($ektron("div.EktronMediaDefaults input.mediaDefaultsData").val());
			},
			update: function(){
				//re-initialize json data array
				Ektron.Commerce.ProductTypes.MediaDefaults.Data.init();
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
	    init: function() {
	        //get localized strings
	        var stringsField = $ektron("div.EktronMediaDefaults input.localizedStrings");
	        if (stringsField && "undefined" != typeof stringsField && stringsField.length)
	            Ektron.Commerce.ProductTypes.MediaDefaults.LocalizedStrings = Ektron.JSON.parse($ektron("div.EktronMediaDefaults input.localizedStrings").attr("value"));
	        
            //bind events
            Ektron.Commerce.ProductTypes.MediaDefaults.bindEvents();
	        
	        //stripe rows
			//Ektron.Commerce.ProductTypes.Items.DefaultView.stripe();
			
			//initialize modal
			Ektron.Commerce.ProductTypes.MediaDefaults.Modal.init();
			
			//initialize json data array
			Ektron.Commerce.ProductTypes.MediaDefaults.Data.init();
	    },
	    LocalizedStrings: {},
	    Modal: {
	        clearFields: function(){
	            $ektron('#MediaDefaultsModal').find("input").val("");
	        },
	        init: function(){
			    $ektron('#MediaDefaultsModal').drag('.mediaDefaultsModalModalHeader');
			    $ektron('#MediaDefaultsModal').modal({
			        modal: true,
				    overlay: 0,
				    toTop: true,
			        onShow: function(hash) {
			            //cancel all other rows in edit mode
	                    $ektron("div.EktronMediaDefaults p.actions a.cancel").trigger("click");
					    hash.o.fadeTo("fast", 0.5, function() {
						    var originalWidth = hash.w.width();
						    hash.w.find("h4").css("width",  originalWidth + "px");
						    var width = "-" + String(originalWidth / 2) + "px";
						    hash.w.css("margin-left", width);
						    hash.w.fadeIn("fast");
					    });
			        },
			        onHide: function(hash) {
		                hash.w.fadeOut("fast");
					    hash.o.fadeOut("fast", function(){
						    if (hash.o) 
							    hash.o.remove();
						    //clear input fields
                            Ektron.Commerce.ProductTypes.MediaDefaults.Modal.clearFields();
					    });
		            }  
			    });
			    $ektron('#MediaDefaultsModal').find("h3 img").bind("click", function(){
				    Ektron.Commerce.ProductTypes.Items.Modal.hide();
			    });
		    },
		    hide: function(){
			    $ektron('#MediaDefaultsModal').modalHide();
		    },
		    show: function(){
			    $ektron('#MediaDefaultsModal').modalShow();
		    }
	    }
	}
}