//Initialize
Ektron.ready(function() {
	Ektron.Commerce.MediaTab.init();
});

//Initialize MediaTab object
$ektron.addLoadEvent(function() {
	Ektron.Commerce.MediaTab.init();
});

//define Ektron.Commerce object only if it's not already defined
if (Ektron.Commerce === undefined) {
	Ektron.Commerce = {};
}

//Ektron Commerce Media Tab Object
Ektron.Commerce.MediaTab = {
	emSize: {},
	escapeAndEncode: function(string) {
	    if (string != undefined)
		{
		    return string
			    .replace("&", "&amp;")
			    .replace("<", "&lt;")
			    .replace(">", "&gt;")
			    .replace("'", "&apos;")
			    .replace("\"", "&quot;")
			    .replace("\"", "\\");
		}
	},
	getEmSize: function() {	
		var emSize = $ektron("body").css("font-size");
        emSize = emSize.replace(/px/g, "");  //remove the "px" from the returned value
		Ektron.Commerce.MediaTab.emSize = +emSize;
	},
	init: function() {
		//intialize modal
		Ektron.Commerce.MediaTab.initModal();
		
		//calculate the pixel size of an em (this is necessary for IE)
		Ektron.Commerce.MediaTab.getEmSize();
			
		//determine media type - this is set to images now only
		//this will need to change later if we add support for 
		//other media types
		var mediaType = "image";
		
		//initialize media type
		switch(mediaType) {
			case "image":
				Ektron.Commerce.MediaTab.Images.initImages();
				break;
			case "other":
				//to be implemented
				break;
		}
	},
	initModal: function() {
		//initialize mediatab modal
		$ektron('#ektronMediaModal').drag('.ektronMediaModalHeader');
		$ektron('#ektronMediaModal').modal({
		    modal: true,
		    toTop: true,
			overlay: 0,
		    onShow: function(hash) {
				hash.o.fadeTo("fast", 0.5, function() {
					var originalWidth = hash.w.width();
					hash.w.find("h4").css("width",  originalWidth + "px");
					var width = "-" + String(originalWidth / 2) + "px";
					hash.w.css("margin-left", width);
					hash.w.fadeIn("fast");
				});
		    }, 
		    onHide: function(hash) {
		        hash.w.fadeOut("fast", function() {
					if (hash.w.find("div.fullsizeMediaWrapper").hasClass("mediaOverflow")) {
						hash.w.find("div.fullsizeMediaWrapper").removeClass("mediaOverflow");
					}
				});
				hash.o.fadeOut("fast", function(){
					if (hash.o) 
						hash.o.remove();
				});
		    }
		});
		
		//initialize mediatab modal
		var modalIframe = $ektron("#ifrm_AddImage");
		var modalIframeSrc = modalIframe.attr("src");
		$ektron('#ektronMediaAddNewImageModal').drag('.ektronMediaModalHeader');		
		$ektron('#ektronMediaAddNewImageModal').modal({
			trigger: '.addImage',
		    modal: true,
			overlay: 0,
		    onShow: function(hash) {
				hash.o.fadeTo("fast", 0.5, function() {
				    modalIframe.attr("src", modalIframeSrc);
					var originalWidth = hash.w.width();
					hash.w.find("h4").css("width",  originalWidth + "px");
					var width = "-" + String(originalWidth / 2) + "px";
					hash.w.css("margin-left", width);
					hash.w.fadeIn("fast");
				});
				hash.o.fadeOut("fast", function(){
					if (hash.o) 
						hash.o.remove();
				});
		    }, 
		    onHide: function(hash) {
		        hash.w.fadeOut("fast", function() {
					if (hash.w.find("div.fullsizeMediaWrapper").hasClass("mediaOverflow")) {
						hash.w.find("div.fullsizeMediaWrapper").removeClass("mediaOverflow");
					}
				});
				hash.o.fadeOut("fast", function(){
					if (hash.o) 
						hash.o.remove();
				});
		    }
		});
	},
	Images: {
		addNewImage: function(newImageObj){
			var newImageItem;
			
			//add new image to imageData object
			var image = {
				Id: newImageObj.id,
				Title: Ektron.Commerce.MediaTab.escapeAndEncode(newImageObj.title),
				AltText: Ektron.Commerce.MediaTab.escapeAndEncode(newImageObj.altText),
				Path: Ektron.Commerce.MediaTab.escapeAndEncode(newImageObj.path),
				Width: newImageObj.width,
				Height: newImageObj.height,								
				Default: "false",
				MarkedForDelete: "false",
				Thumbnails: []
			};
			
			//initialize Thumbnails as array
			//image.Thumbnails = new Array();
			
			var placeholderListItem = $ektron("ul.images").children("li:first");
			var isPlaceholder = false;
			if (placeholderListItem.hasClass("placeholder")) {
			    isPlaceholder = true;
			}
			
			if (isPlaceholder === true) {
			    newImageItem = placeholderListItem;
			} else {
			    newImageItem = placeholderListItem.clone();
			}
		    newImageItem.MarkedForDelete = "true";
			newImageItem.find("input.imageId").attr("value", newImageObj.id);
			
			var title = newImageItem.find("ul.mediaAttributes li.title div.title span");
			title.text(newImageObj.title);
			var altText = newImageItem.find("ul.mediaAttributes li.altText div.altText span");
			altText.text(newImageObj.altText);
			
			newImageItem.find("p.path").text(newImageObj.path);
			newImageItem.find("p.type").text("");
			newImageItem.find("p.mediaPreview img").attr("src", newImageObj.path);
			newImageItem.find("p.mediaPreview img").attr("alt", newImageObj.altText);
			newImageItem.find("p.mediaPreview img").attr("title", newImageObj.title);
			newImageItem.find("p.mediaPreview img").attr("width", newImageObj.width);
			newImageItem.find("p.mediaPreview img").attr("height", newImageObj.height);			
			newImageItem.find("p.mediaPreview img").attr("style", "");
			newImageItem.find("input.default").attr("value", "false");
			newImageItem.find("input.MarkForDelete").attr("value", "true");
			
			//get all thumbnails in cloned image list item
			var thumbnailRow = newImageItem.find("tr.thumbnailRowWrapper");
			var currentThumbnails = thumbnailRow.children("td");
			
			//remove cloned thumbnails except for the last one
			if (currentThumbnails.length > 1){
				currentThumbnails.each(function(i){
					if (i > 0) {
						$ektron(this).remove();
					}
				});
			}
			
			//create thumbnail markup clone (thumbnail placeholder)
			var thumbnail = thumbnailRow.children("td:first");
			thumbnail.find("span.height").text("");
			thumbnail.find("span.width").text("");
			thumbnail.find("p.thumbnailPreview img.thumbnail").attr("src", "");
			thumbnail.find("p.thumbnailPreview img.thumbnail").attr("style", "");
			thumbnail.find("p.thumbnailPreview img.thumbnail").attr("title", "");
			thumbnail.find("p.thumbnailPreview img.thumbnail").attr("alt", "");
			
			//remove existing thumbnail markup
			thumbnailRow.children("td").each(function(i){
				$ektron(this).remove();
			});

			//add new thumbnails
			for (i=0;i<newImageObj.Thumbnails.length;i++)
			{
			    if ( newImageObj.Thumbnails[i] != null ) 
			    {
			        var strExt = newImageObj.Thumbnails[i].path.substring(newImageObj.Thumbnails[i].path.length - 3);
			        if (strExt == "gif"){newImageObj.Thumbnails[i].path = newImageObj.Thumbnails[i].path.replace('.gif', '.png');}
			        var thumbnailClone = thumbnail.clone();
			        thumbnailClone.find("p.thumbnailPreview img.thumbnail").attr("src", newImageObj.Thumbnails[i].path);
			        thumbnailClone.find("p.thumbnailPreview img.thumbnail").attr("alt", newImageObj.altText);
			        thumbnailClone.find("p.thumbnailPreview img.thumbnail").attr("title", newImageObj.title);
			        thumbnailRow.append(thumbnailClone);
			        var thumbnailData = Ektron.Commerce.MediaTab.escapeAndEncode(newImageObj.Thumbnails[i].path);
			        image.Thumbnails.push(thumbnailData);
			    }
			}
			
			//remove the thumbnail markup placeholder
			thumbnail.remove();
			
			
			Ektron.Commerce.MediaTab.Images.imageData.push(image);
			
			//update postback field
			Ektron.Commerce.MediaTab.Images.setPostbackField();
			
			//append new image
			if (placeholderListItem.hasClass("placeholder")) {
			    newImageItem.removeClass("placeholder");
			} else {
			    $ektron("ul.images").append(newImageItem);
			}
			
			//scale-down the fullsize image (if the image is larger) than the wrapper, or
			//position the fullsize image to the center/middle of the wrapper (if the image is smaller than the wrapper)
			var fullsizeImage = $ektron("ul.images").children("li.mediaItem:last").find("p.mediaPreview img");
			Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight = newImageObj.height;
			Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth = newImageObj.width;
			
			//set size of fullsize image container
			var mediaPreviewWrapper = fullsizeImage.parent();
			var mediaAttributeTableHeight = mediaPreviewWrapper.next().height();
			mediaPreviewWrapper.css("height", mediaAttributeTableHeight - 2);
			mediaPreviewWrapper.css("width", mediaAttributeTableHeight - 2);
			mediaPreviewWrapper.children("span.viewFullsize").css("width", mediaAttributeTableHeight - 2);
			
			Ektron.Commerce.MediaTab.Images.alignFullsizeImagesToWrapper(fullsizeImage, false);
			
			var button = newImageItem.find("a.deleteImageSelector");
			var mediaItem = button.parents("li.mediaItem");
			var imageDataId = mediaItem.prevAll().length;
			var j = 0;
			for ( var i = 0; i < imageDataId; i++ )
			{
			    if( Ektron.Commerce.MediaTab.Images.imageData[i].MarkedForDelete === "true" )
			    {
			        j++;		            
			    }			    
			}
			
			if( (j != 0) && (j == imageDataId) )
			{
		        Ektron.Commerce.MediaTab.Images.imageData[imageDataId].MarkedForDelete = "true";
		        Ektron.Commerce.MediaTab.Images.markForDelete(newImageItem.find("a.deleteImageSelector"));
		    }

			//hide modal
			$ektron("#ifrm_AddImage").attr("src", "");
			$ektron("#ektronMediaAddNewImageModal").modalHide();
			//binds the events to newly added image.
			Ektron.Commerce.MediaTab.Images.bindEvents(image);
			
		},
		alignFullsizeImagesToWrapper: function(images, calculateDimensions) {
			$ektron.each(images, function(){
				//get image dimensions
				Ektron.Commerce.MediaTab.Images.Dimensions.image = $ektron(this);
				if (calculateDimensions === true) {
					Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight = Ektron.Commerce.MediaTab.Images.Dimensions.image.height();
					Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth = Ektron.Commerce.MediaTab.Images.Dimensions.image.width();
				}
				
				//set image dimension lables
				Ektron.Commerce.MediaTab.Images.setFullsizeImageDimensionLabels();
				
				//set image type label
				Ektron.Commerce.MediaTab.Images.setFullsizeImageType();
				
				//get parent dimensions
				Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight = Ektron.Commerce.MediaTab.Images.Dimensions.image.parent().height();
				Ektron.Commerce.MediaTab.Images.Dimensions.parentWidth = Ektron.Commerce.MediaTab.Images.Dimensions.image.parent().width();

				//determine image position - scale or center
				if (
						Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight > Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight || 
						Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth > Ektron.Commerce.MediaTab.Images.Dimensions.parentWidth
					) 
				{
					//scale image
					Ektron.Commerce.MediaTab.Images.scaleImageToWrapper();						
				} else {
					//center image
					Ektron.Commerce.MediaTab.Images.centerImageInWrapper();
				}
			
				//display image
				Ektron.Commerce.MediaTab.Images.Dimensions.image.fadeIn("slow");
			});
		},
		bindEvents: function() {
			//bind click to .showFullsizeModal to clone image for modal
			$ektron(".showFullsizeModal").bind("click", function(){
				
				var modalTrigger = $ektron(this);
				var clone = $ektron(this).children("img").clone();
				clone.addClass("fullsizeImage");
				clone.css("width", "auto");
				clone.css("height", "auto");
				var container = $ektron("#ektronMediaModal table.fullsizeMedia tbody").find("div.fullsizeMediaWrapper");
				if (container.find("img").length > 0) {
					container.find("img").remove();
				}
				container.append(clone);
				
				var fullsizeImage = $ektron("#ektronMediaModal table.fullsizeMedia tbody img.fullsizeImage");
		        
				var width = modalTrigger.parent().find("ul.mediaAttributes p.width").text().replace("px", "");
				var height = modalTrigger.parent().find("ul.mediaAttributes p.height").text().replace("px", "");
				
				$ektron("#ektronMediaModal table.fullsizeMedia tbody th.height span").text(height + "px").fadeIn("normal");
				$ektron("#ektronMediaModal table.fullsizeMedia tbody th.width span").text(width + "px").fadeIn("normal");
				
				if (width > (40 * Ektron.Commerce.MediaTab.emSize)) {
					container.addClass("mediaOverflow");
				}
				
				$ektron('#ektronMediaModal').modalShow();
			});
			
			//set toggle onHover over fullsize image preview to display "view fullsize" cue
			var mediaPreviewWindow = $ektron("div.EktronMediaTabWrapper p.mediaPreview");
			mediaPreviewWindow.hover(
				function() {
					$ektron(this).children("span.viewFullsize")
					    .css("display", "block")
					    .css("position", "absolute")
					    .fadeIn("normal");
				},
				function() {
					$ektron(this).children("span.viewFullsize").css("display", "block").fadeOut("normal");
				}
			);
			
			//set toggle onHover over thumbnail image preview to display "view fullsize" cue
			var thumbnailPreviewWindow = $ektron("div.EktronMediaTabWrapper p.thumbnailPreview img");
			thumbnailPreviewWindow.hover(
				function() {
					$ektron(this).prev("span.viewFullsize").fadeIn("normal");
				},
				function() {
					$ektron(this).prev("span.viewFullsize").fadeOut("normal");
				}
			);
			
			//set toggle onHover over thumbnail "set default" button
			var thumbnailDefaultButton = $ektron("div.EktronMediaTabWrapper p.defaultThumbnail")
			thumbnailDefaultButton.hover(
				function() {
					if ($ektron(this).hasClass("selectedThumbnail") === false) {
						$ektron(this).css("background-image", "url(images/defaultThumbnailHover.gif)");
					}
				},
				function() {
					if ($ektron(this).hasClass("selectedThumbnail") === false) {
						$ektron(this).css("background-image", "url(images/defaultThumbnail.gif)");
					}
				}
			);
		},
		centerImageInWrapper: function() {
			var newTop;
			var newLeft;
			
			//calculate middle
			if (Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight > Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight)
				newTop = "-" + ((Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight - Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight) / 2) + "px";
			if (Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight < Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight)
				newTop = ((Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight - Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight) / 2) + "px";
				
			//calculate center
			if (Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth > Ektron.Commerce.MediaTab.Images.Dimensions.parentWidth)
				newLeft = "-" + ((Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth - Ektron.Commerce.MediaTab.Images.Dimensions.parentWidth) / 2) + "px";
			if (Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth < Ektron.Commerce.MediaTab.Images.Dimensions.parentWidth) 
				newLeft = ((Ektron.Commerce.MediaTab.Images.Dimensions.parentWidth - Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth) / 2) + "px";
		
			//set top & left of product image center & middle of parent
			Ektron.Commerce.MediaTab.Images.Dimensions.image.css("top", newTop);
			Ektron.Commerce.MediaTab.Images.Dimensions.image.css("left", newLeft);
		},
		Dimensions: {
			emSize: 0,
			image: {},
			imageHeight: 0,
			imageWidth: 0,
			parentHeight: 0,
			parentWidth: 0
		},
		Edit: {
			cancel: function(editWrapper) {
				editWrapper.find("input").remove();
				editWrapper.find("span").css("display", "inline");
				editWrapper.find("img.revise").css("display", "inline");
				editWrapper.find("img.reviseOK").css("display", "none");
				editWrapper.find("img.reviseCancel").css("display", "none");
			},
			ok: function(editWrapper) {
				var span = editWrapper.find("span");
				var input = editWrapper.find("input");
				var newText = input.attr("value");
				span.text(newText);
				input.remove();
				editWrapper.find("span").css("display", "inline");
				editWrapper.find("img.revise").css("display", "inline");
				editWrapper.find("img.reviseOK").css("display", "none");
				editWrapper.find("img.reviseCancel").css("display", "none");
				
				var index = editWrapper.parents("li.mediaItem").prevAll().length;
				if (editWrapper.hasClass("title")) {
					Ektron.Commerce.MediaTab.Images.imageData[index].Title = Ektron.Commerce.MediaTab.escapeAndEncode(newText);
				} else {
					Ektron.Commerce.MediaTab.Images.imageData[index].AltText = Ektron.Commerce.MediaTab.escapeAndEncode(newText);
				}				
				//update postback field
				Ektron.Commerce.MediaTab.Images.setPostbackField();
			},
			showForm: function(editWrapper) {
				editWrapper.find("img.revise").css("display", "none");
				editWrapper.find("img.reviseOK").css("display", "inline");
				editWrapper.find("img.reviseCancel").css("display", "inline");
				var span = editWrapper.children("span");
				var currentValue = span.text();
				span.css("display", "none");
				editWrapper.append("<input type=\"text\" value=\"" + currentValue + "\" />");
			}
		},
		getImageData: function() {
			$ektron("div.mediaGroup ul.images").children("li").each(function(i){
		        var image = {
			        Id: "",
			        Title: "",
			        AltText: "",
			        Path: "",
			        Default: "",
			        Width: "",
			        Height: "",
			        MarkedForDelete: "",
			        Thumbnails: []
			    };
			    
				var id = $ektron(this).find("input.imageId").attr("value");
				var title = Ektron.Commerce.MediaTab.escapeAndEncode($ektron(this).find("div.title span").text());
				var altText = Ektron.Commerce.MediaTab.escapeAndEncode($ektron(this).find("div.altText span").text());
				var path = Ektron.Commerce.MediaTab.escapeAndEncode($ektron(this).find("p.path").text());
				var width = Ektron.Commerce.MediaTab.escapeAndEncode($ektron(this).find("p.width").text());
				var height = Ektron.Commerce.MediaTab.escapeAndEncode($ektron(this).find("p.height").text());
				var defaultImage = $ektron(this).find("input.default").attr("value");
				var markedForDelete = "false";
				
				//add image data
				image.Id = id;
				image.Title = title;
				image.AltText = altText;
				image.Path = path;
				image.Width = width;
				image.Height = height;
				image.Default = defaultImage;
				image.MarkedForDelete = markedForDelete;
				
				//intialize Thumbnails array
			    //image.Thumbnails = new Array();
			    
			    //add thumbnail paths
			    for (i=0;i<$ektron(this).find("p.thumbnailPreview img.thumbnail").length;i++){
				    var path = $ektron(this).find("p.thumbnailPreview img.thumbnail").attr("src");
				    var thumbnailData = Ektron.Commerce.MediaTab.escapeAndEncode($ektron(this).find("p.thumbnailPreview img.thumbnail")[i].src);
				    image.Thumbnails.push(thumbnailData);
			    }
				Ektron.Commerce.MediaTab.Images.imageData.push(image);
			});
			
			//update postback field
			Ektron.Commerce.MediaTab.Images.setPostbackField();
		},
		imageData: {},
		initImages: function() {
		    //initialize imageData as Array
		    Ektron.Commerce.MediaTab.Images.imageData = new Array();
		    
			//populate imageData JSON object
			Ektron.Commerce.MediaTab.Images.getImageData();
			
			//size the fullsize image wrapper (as square) based on the height of the attribute table
			Ektron.Commerce.MediaTab.Images.setFullsizeWrapperDimensions();
			
			//scale-down the fullsize image (if the image is larger) than the wrapper, or
			//position the fullsize image to the center/middle of the wrapper (if the image is smaller than the wrapper)
			var fullsizeImages = $ektron("div.EktronMediaTabWrapper p.mediaPreview img");
			Ektron.Commerce.MediaTab.Images.alignFullsizeImagesToWrapper(fullsizeImages, true);
			
			//bind hover event to image wrapper
			Ektron.Commerce.MediaTab.Images.bindEvents();
		},
		markForDelete: function(buttonObj){
			var button = $ektron(buttonObj);
			var mediaItem = button.parents("li.mediaItem");
			var imageDataId = mediaItem.prevAll().length;
			
			if (Ektron.Commerce.MediaTab.Images.imageData[imageDataId].MarkedForDelete === "true") {
				mediaItem.find("p.mediaPreview img").fadeTo("fast", 1);
				mediaItem.find("ul.mediaAttributes input").attr("disabled", "");
				mediaItem.find("p.actions a.defaultImageSelector").fadeIn("normal");
				mediaItem.find("p.actions a.viewThumbnails").fadeIn("normal");
				
				mediaItem.find("div.edit").each(function(i){
					var editWrapper = $ektron(this);
					if (editWrapper.children("span").css("display") === "none") {
						editWrapper.find("img.reviseCancel").fadeIn("normal");
						editWrapper.find("img.reviseOK").fadeIn("normal");
					} else {
						editWrapper.find("img.revise").fadeIn("normal");
					}
				});
				button.children("span").text("Delete");
				Ektron.Commerce.MediaTab.Images.imageData[imageDataId].MarkedForDelete = "false";
			} else {
				mediaItem.find("p.mediaPreview img").fadeTo("fast", 0.5);
				mediaItem.find("ul.mediaAttributes input").attr("disabled", "disabled");
				mediaItem.find("p.actions a.defaultImageSelector").fadeOut("normal");
				mediaItem.find("p.actions a.viewThumbnails").fadeOut("normal");
				mediaItem.find("div.edit img").fadeOut("normal");
				button.children("span").text("Restore");
				Ektron.Commerce.MediaTab.Images.imageData[imageDataId].MarkedForDelete = "true";
			}
			
			//update postback field
			Ektron.Commerce.MediaTab.Images.setPostbackField();
		},
		scaleImageToWrapper: function() {
			var scaled = false;
			var multiplier;
			var newHeight;
			var newWidth;
			
			if ((scaled === false) && (Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight > Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth)) {
				//image is taller than it is wide
				multiplier = Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight / Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight;
				newHeight = multiplier * Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight;
				newWidth = multiplier * Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth;
				scaled = true;
			}
			
			if ((scaled === false) && (Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight < Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth)) {
				//image is wider than it is tall
				multiplier = Ektron.Commerce.MediaTab.Images.Dimensions.parentWidth / Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth;
				newWidth = multiplier * Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth;
				newHeight = multiplier * Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight;
			}
			
			if ((scaled === false) && (Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight === Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth)) {
				//image is square
				newWidth = Ektron.Commerce.MediaTab.Images.Dimensions.parentWidth;
				newHeight = Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight;
			}
			
			Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight = newHeight;
			Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth = newWidth;
			
			//set top & left of product image center & middle of parent
			Ektron.Commerce.MediaTab.Images.Dimensions.image.css("height", newHeight);
			Ektron.Commerce.MediaTab.Images.Dimensions.image.css("width", newWidth);
			
			Ektron.Commerce.MediaTab.Images.centerImageInWrapper();
		},
		setDefault: function(obj) {
			var selectedImageButton = $ektron(obj);
			//set all images to non-default image state
			selectedImageButton.parents("ul.mediaList").find("a.defaultImage")
				.removeClass("defaultImage")
				.children("span").text("Set as Default");
			selectedImageButton.parents("ul.mediaList").find("input.default").attr("value", "false");
			for (i=0;i<Ektron.Commerce.MediaTab.Images.imageData.length;i++) {
			  Ektron.Commerce.MediaTab.Images.imageData[i].Default = "false";
			}
			
			//set selected image to default image state
			selectedImageButton
				.addClass("defaultImage")
				.children("span").text("Default Image");
			selectedImageButton.parents("ul.mediaList").find("input.default").attr("value", "true");
			var index = selectedImageButton.parents("li.mediaItem").prevAll().length;
			Ektron.Commerce.MediaTab.Images.imageData[index].Default = "true";
			
			//update postback field
			Ektron.Commerce.MediaTab.Images.setPostbackField();
		},
		setPostbackField:function(){
			$ektron("#Ektron_Commerce_Workarea_MediaTab_Images").attr("value", Ektron.JSON.stringify(Ektron.Commerce.MediaTab.Images.imageData));
		},
		setFullsizeImageDimensionLabels: function(){
			//set image dimension labels
			Ektron.Commerce.MediaTab.Images.Dimensions.image.parent().parent().find("p.height").text(Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight + "px");
			Ektron.Commerce.MediaTab.Images.Dimensions.image.parent().parent().find("p.width").text(Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth + "px");
		},
		setFullsizeImageType: function(){
			//set image dimension labels
			var imageSrc = Ektron.Commerce.MediaTab.Images.Dimensions.image.attr("src");
			
			if (imageSrc != null)
			{
			    //get extension type
			    var imageType = imageSrc.split("").reverse().join("");
			    var extensionMarker = imageType.indexOf(".");
			    imageType = imageType.substring(0,extensionMarker);
			    imageType = imageType.split("").reverse().join("");
			    Ektron.Commerce.MediaTab.Images.Dimensions.image.parent().parent().find("p.type").text("." + imageType);
			}
		},
		setFullsizeWrapperDimensions: function() {
			//set image wrapper height and width
			var mediaPreviewWrappers = $ektron("div.EktronMediaTabWrapper p.mediaPreview");
			
			$ektron.each(mediaPreviewWrappers, function(){
				var currentMediaWrapper = $ektron(this);
				
				currentMediaWrapper.find("img").css("height", "10px");
				currentMediaWrapper.find("img").css("width", "10px");
				
				var mediaAttributeTableHeight = currentMediaWrapper.next().height();
				currentMediaWrapper.css("height", mediaAttributeTableHeight - 2);
				currentMediaWrapper.css("width", mediaAttributeTableHeight - 2);
				currentMediaWrapper.children("span.viewFullsize").css("width", mediaAttributeTableHeight - 2);
				
				currentMediaWrapper.find("img").css("height", "");
				currentMediaWrapper.find("img").css("width", "");
			});
		},
		Thumbnails: {
			toggle: function(obj) {
				
				var button = $ektron(obj);
				var thumbnailWrapper = button.parent().next();
				var thumbnailImageWrapper = button.parent().next().find("div.thumbnailImageWrapper");
			
				//set the width of the thumbnail container for ie to force overflow
				if (!$ektron.browser.mozilla) {
					thumbnailImageWrapper.css("width", thumbnailImageWrapper.parents("li").width() - ((Ektron.Commerce.MediaTab.emSize * 4)));
				}
				
				//update show/hide label
				var buttonLabel = button.children("span");
				
				if (thumbnailWrapper.css("display") === "none") {
					buttonLabel.text("Hide Thumbnails");
				} else {
					buttonLabel.text("View Thumbnails");
				}
				
				//show thumbnails
				thumbnailWrapper.slideToggle("normal", function() {
					thumbnailImageWrapper.find("img.thumbnail").each(function(i) {
						var currentImage = $ektron(this);
						var height = currentImage.height();
						var width = currentImage.width();
						
						var elementHeight = currentImage.parent().parent().find("span.height");
						var elementWidth = currentImage.parent().parent().find("span.width");
						
						elementHeight.text(height + "px");
						elementWidth.text(width + "px");
						
						elementHeight.fadeIn("normal");
						elementWidth.fadeIn("normal");
					});
				});
			}
		}
	}
};