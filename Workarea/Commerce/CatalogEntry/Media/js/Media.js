Ektron.ready(function()
{
    Ektron.Commerce.MediaTab.init();
});

//define Ektron.Commerce object only if it's not already defined
if (Ektron.Commerce === undefined)
{
    Ektron.Commerce = {};
}

if (Ektron.Commerce.MediaTab === undefined)
{
    //Ektron Commerce Media Tab Object
    Ektron.Commerce.MediaTab = {
        emSize: {},
        escapeAndEncode: function(string)
        {
            if (string !== undefined)
            {
                return string
	                .replace(/&/g, "&amp;")
	                .replace(/</g, "&lt;")
	                .replace(/>/g, "&gt;")
	                .replace(/'/g, "\'")
	                .replace(/\"/g, "\"");
            }
        },
        getEmSize: function()
        {
            var emSize;
            var em = $ektron('<div/>')
                .css({ position: 'absolute', top: 0, left: 0, height: '1.0em', visibility: 'hidden', lineHeight: '1.0em' })
                .text('M').appendTo("body");
            emSize = em[0].style.pixelHeight;
            em.remove();
            Ektron.Commerce.MediaTab.emSize = emSize;
        },
        init: function()
        {
            //intialize modal
            Ektron.Commerce.MediaTab.initModal();

            //calculate the pixel size of an em (this is necessary for IE)
            Ektron.Commerce.MediaTab.getEmSize();

            //determine media type - this is set to images now only
            //this will need to change later if we add support for 
            //other media types
            var mediaType = "image";

            //initialize media type
            switch (mediaType)
            {
                case "image":
                    Ektron.Commerce.MediaTab.Images.init();
                    break;
                case "other":
                    //to be implemented
                    break;
            }
        },
        initModal: function()
        {
            //initialize mediatab modal
            $ektron('#ektronMediaModal').drag('.ektronMediaModalHeader');
            $ektron('#ektronMediaModal').modal({
                modal: true,
                toTop: true,
                overlay: 0,
                onShow: function(hash)
                {
                    hash.o.fadeTo("fast", 0.5, function()
                    {
                        var originalWidth = hash.w.width();
                        hash.w.find("h4").css("width", originalWidth + "px");
                        var width = "-" + String(originalWidth / 2) + "px";
                        hash.w.css("margin-left", width);
                        hash.w.fadeIn("fast");
                    });
                },
                onHide: function(hash)
                {
                    hash.w.fadeOut("fast", function()
                    {
                        if (hash.w.find("div.fullsizeMediaWrapper").hasClass("mediaOverflow"))
                        {
                            hash.w.find("div.fullsizeMediaWrapper").removeClass("mediaOverflow");
                        }
                    });
                    hash.o.fadeOut("fast", function()
                    {
                        if (hash.o)
                        {
                            hash.o.remove();
                        }
                    });
                }
            });
            //initialize mediatab modal
            $ektron('#ektronMediaAddNewImageModal').drag('.ektronMediaModalHeader');
            $ektron('#ektronMediaAddNewImageModal').modal({
                modal: true,
                toTop: true,
                overlay: 0,
                onShow: function(hash)
                {
                    if ($ektron.browser.msie === true && $ektron.browser.version < 7)
                    {
                        hash.w.show();
                    } else
                    {
                        hash.o.fadeTo("fast", 0.5, function()
                        {
                            var originalWidth = hash.w.width();
                            hash.w.find("h4").css("width", originalWidth + "px");
                            var width = "-" + String(originalWidth / 2) + "px";
                            hash.w.css("margin-left", width);
                            hash.w.fadeIn("fast");
                        });
                    }
                },
                onHide: function(hash)
                {
                    hash.w.fadeOut("fast", function()
                    {
                        if (hash.w.find("div.fullsizeMediaWrapper").hasClass("mediaOverflow"))
                        {
                            hash.w.find("div.fullsizeMediaWrapper").removeClass("mediaOverflow");
                        }
                        var modalIframe = $ektron("div#ektronMediaAddNewImageModal iframe");
                        modalIframe.removeAttr("src");
                    });
                    hash.o.fadeOut("fast", function()
                    {
                        if (hash.o)
                        {
                            hash.o.remove();
                        }
                    });
                }
            });
        },
        Images: {
            addImageShowForm: function(productTypeId)
            {

                // allow selecting existing images from the library
                window.open('../mediamanager.aspx?actiontype=library&scope=images&productTypeId=' + productTypeId + '&productmode=true&showthumb=false&autonav=', 'Preview', 'width=' + 850 + ',height=' + 500 + ',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no');
                return false;
            },
            addNewImageShowForm: function()
            {
                var modalIframe = $ektron("div#ektronMediaAddNewImageModal iframe");
                var modalIframeSrc = $ektron("div#ektronMediaAddNewImageModal input.iframeAddImageSrc").val();
                modalIframe.attr("src", modalIframeSrc);
                $ektron('#ektronMediaAddNewImageModal').modalShow();
                return false;
            },
            addNewImage: function(newImageObj)
            {
                //add new image to imageData object
                var imageData = {
                    Id: newImageObj.id,
                    Title: Ektron.Commerce.MediaTab.escapeAndEncode(newImageObj.title),
                    AltText: Ektron.Commerce.MediaTab.escapeAndEncode(newImageObj.altText),
                    Path: Ektron.Commerce.MediaTab.escapeAndEncode(newImageObj.path),
                    Width: newImageObj.width,
                    Height: newImageObj.height,
                    Default: "false",
                    MarkedForDelete: "false"
                };

                //get itemClone and clone it
                var listItemClone = $ektron("div.mediaGroup ul.mediaList li.mediaItemClone").clone();

                //set values
                listItemClone.find("input.imageId").attr("value", imageData.Id);

                listItemClone.find("td.title span").text(imageData.Title);
                listItemClone.find("input.title").attr("value", imageData.Title);

                listItemClone.find("td.altText span").text(imageData.AltText);
                listItemClone.find("input.altText").attr("value", imageData.AltText);

                listItemClone.find("td.path div").text(imageData.Path);
                listItemClone.find("input.path").attr("value", imageData.Path);

                // don't display empty/zero width - really just missing info
                listItemClone.find("td.width").text((imageData.Width.length > 0 && imageData.Width != "0") ? imageData.Width + "px" : "");
                listItemClone.find("input.width").attr("value", imageData.Width);

                // don't display empty/zero height - really just missing info
                listItemClone.find("td.height").text((imageData.Height.length > 0 && imageData.Height != "0") ? imageData.Height + "px" : "");
                listItemClone.find("input.height").attr("value", imageData.Height);

                listItemClone.find("td.type").text("." + Ektron.Commerce.MediaTab.Images.setFullsizeImageType(imageData.Path));

                listItemClone.find("th.mediaPreview img")
					.attr("src", imageData.Path)
					.attr("alt", imageData.AltText)
					.attr("title", imageData.Title)
					.css("height", imageData.Height + "px")
					.css("width", imageData.Width + "px");

                listItemClone.find("input.default").attr("value", "false");
                listItemClone.find("input.MarkForDelete").attr("value", "true");

                //get thumbnail node placeholder
                var thumbnailWrapper = listItemClone.find("tr.thumbnailRowWrapper");
                var thumbnailCloneNode = listItemClone.find("tr.thumbnailRowWrapper td");
                var thumbnailNode = thumbnailCloneNode.clone();

                //add new thumbnails or hide thumbnail-related nodes if no thumbnails)
                if (newImageObj.Thumbnails === undefined)
                {
                    //there are no thumbnails - remove thumbnail div and show thumbnail button
                    listItemClone.find("div.thumbnails").remove();
                    listItemClone.find("table.mediaImage tfoot a.viewThumbnails").remove();
                } else
                {
                    //add thumbnails
                    for (i = 0; i < newImageObj.Thumbnails.length; i++)
                    {
                        if (newImageObj.Thumbnails[i] !== null)
                        {
                            var thumbnailClone = thumbnailNode.clone();
                            thumbnailClone.find("p.thumbnailPreview img.thumbnail")
								.attr("src", newImageObj.Thumbnails[i].path + newImageObj.Thumbnails[i].imageName)
								.attr("alt", newImageObj.Thumbnails[i].title)
								.attr("title", newImageObj.Thumbnails[i].title);
                            thumbnailClone.find("p.thumbnailPreview input.thumbnailHeight").attr("value", newImageObj.Thumbnails[i].height);
                            thumbnailClone.find("p.thumbnailPreview input.thumbnailWidth").attr("value", newImageObj.Thumbnails[i].width);
                            thumbnailClone.find("p.thumbnailPreview input.thumbnailPath").attr("value", newImageObj.Thumbnails[i].path);
                            thumbnailClone.find("p.thumbnailPreview input.thumbnailImageName").attr("value", newImageObj.Thumbnails[i].imageName);
                            thumbnailClone.find("p.thumbnailPreview input.thumbnailTitle").attr("value", newImageObj.Thumbnails[i].title);

                            thumbnailWrapper.append(thumbnailClone);
                        }
                    }
                }

                //remove the thumbnail markup placeholder
                thumbnailCloneNode.remove();

                Ektron.Commerce.MediaTab.Images.imageData.push(imageData);

                //if there is an "no image" item image present, remove it
                $ektron("div.mediaGroup ul.mediaList li.mediaImageNone").remove();

                //append new image
                var mediaList = $ektron("div.mediaGroup ul.mediaList");
                listItemClone
					.removeClass("mediaItemClone")
					.addClass("mediaItem");
                mediaList.append(listItemClone);

                //set fullsize wrapper dimensions
                Ektron.Commerce.MediaTab.Images.setFullsizeWrapperDimensions("last");

                //scale-down the fullsize image (if the image is larger) than the wrapper, or
                //position the fullsize image to the center/middle of the wrapper (if the image is smaller than the wrapper)
                var fullsizeImage = $ektron("ul.mediaList li.mediaItem:last th.mediaPreview img");
                setTimeout(function()
                {
                    Ektron.Commerce.MediaTab.Images.alignFullsizeImagesToWrapper(fullsizeImage);

                    //reset hide modal
                    var addImageModal = $ektron("#ektronMediaAddNewImageModal").modalHide();

                    //binds the events to newly added image.
                    Ektron.Commerce.MediaTab.Images.bindEvents();

                    //update postback field
                    Ektron.Commerce.MediaTab.Images.getImageData();
                }, 500);
            },
            alignFullsizeImagesToWrapper: function(images)
            {
                $ektron.each(images, function()
                {
                    //get image dimensions
                    var width;
                    var height;

                    Ektron.Commerce.MediaTab.Images.Dimensions.image = $ektron(this);
                    width = Ektron.Commerce.MediaTab.Images.Dimensions.image.width();
                    height = Ektron.Commerce.MediaTab.Images.Dimensions.image.height();
                    if (width == "0" && height == "0")
                    {
                        width = Ektron.Commerce.MediaTab.Images.Dimensions.image.attr("data-ektron-commerce-media-image-width");
                        height = Ektron.Commerce.MediaTab.Images.Dimensions.image.attr("data-ektron-commerce-media-image-height");
                        if ("undefined" == width && "undefined" == "height")
                        {
                            Ektron.Commerce.MediaTab.Images.Dimensions.image
                                .css("position", "absolute")
                                .css("left", "-10000px")
                                .css("display", "block");
                            width = Ektron.Commerce.MediaTab.Images.Dimensions.image.width();
                            height = Ektron.Commerce.MediaTab.Images.Dimensions.image.height();
                            Ektron.Commerce.MediaTab.Images.Dimensions.image
                                .css("display", "none")
                                .css("left", "0")
                                .css("position", "static");
                        }
                    }

                    Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth = width;
                    Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight = height;

                    //set image dimension lables
                    Ektron.Commerce.MediaTab.Images.setFullsizeImageDimensionLabels();

                    //set image type label
                    var _type = Ektron.Commerce.MediaTab.Images.Dimensions.image.attr("src");
                    if (_type.indexOf("?") != -1)
                    {
                        _type = _type.substring(0, _type.indexOf("?"));
                    }
                    Ektron.Commerce.MediaTab.Images.Dimensions.image.parents("table.mediaImage").find("td.type").text("." + Ektron.Commerce.MediaTab.Images.setFullsizeImageType(_type));

                    //get parent dimensions
                    Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight = String(Ektron.Commerce.MediaTab.Images.Dimensions.image.parent().css("height")).replace("px", "");
                    Ektron.Commerce.MediaTab.Images.Dimensions.parentWidth = String(Ektron.Commerce.MediaTab.Images.Dimensions.image.parent().css("width")).replace("px", "");

                    //determine image position - scale or center
                    if (Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight > Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight ||
						Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth > Ektron.Commerce.MediaTab.Images.Dimensions.parentWidth)
                    {
                        //scale image
                        Ektron.Commerce.MediaTab.Images.scaleImageToWrapper();
                    }

                    //display image
                    Ektron.Commerce.MediaTab.Images.Dimensions.image.fadeIn("slow");
                });
            },
            bindEvents: function()
            {
                //bind click to .showFullsizeModal to clone image for modal
                $ektron(".showFullsizeModal").bind("click", function()
                {

                    var modalTrigger = $ektron(this);
                    var clone = $ektron(this).children("img").clone();
                    clone.addClass("fullsizeImage");
                    clone.css("width", "auto");
                    clone.css("height", "auto");
                    var container = $ektron("#ektronMediaModal table.fullsizeMedia tbody").find("div.fullsizeMediaWrapper");
                    container.empty();
                    container.append(clone);

                    var fullsizeImage = $ektron("#ektronMediaModal table.fullsizeMedia tbody img.fullsizeImage");

                    var width = modalTrigger.parents("table.mediaImage").find("tbody td.width").text();
                    var height = modalTrigger.parents("table.mediaImage").find("tbody td.height").text();
                    $ektron("#ektronMediaModal table.fullsizeMedia tbody th.height span").text(height).fadeIn("normal");
                    $ektron("#ektronMediaModal table.fullsizeMedia tbody th.width span").text(width).fadeIn("normal");

                    if (width > (40 * Ektron.Commerce.MediaTab.emSize))
                    {
                        container.addClass("mediaOverflow");
                    }

                    $ektron('#ektronMediaModal').modalShow();
                });

                //set toggle onHover over fullsize image preview to display "view fullsize" cue
                var mediaPreviewWindow = $ektron("div.EktronMediaTabWrapper table.mediaImage th.mediaPreview");
                mediaPreviewWindow.hover(function()
                {
                    var boundElement = $ektron(this);
                    var image = boundElement.children("img");
                    var imageHeight = image.height();
                    var imageWidth = image.width();
                    var boundElementHeight = boundElement.height();
                    var boundElementWidth = boundElement.width();
                    var viewFullsizeLabel = boundElement.children("span.viewFullsize");
                    var viewFullsizeLabelPositionTop = (boundElementHeight / 2) - (imageHeight / 2);
                    var viewFullsizeLabelPositionLeft = (boundElementWidth / 2) - (imageWidth / 2);
                    var labelPosition;
                    if ($ektron.browser.msie === true)
                    {
                        labelPosition = ($ektron.browser.version <= 7) ? "-3px" : "-3px";
                    }
                    else
                    {
                        labelPosition = "0px";
                    }

                    var labelWidth;
                    if ($ektron.browser.msie === true)
                    {
                        labelWidth = ($ektron.browser.version <= 7) ? (boundElementWidth + 2) + "px" : (boundElementWidth + 1) + "px";
                    }
                    else
                    {
                        labelWidth = (boundElementWidth + 1) + "px";
                    }

                    viewFullsizeLabel
						.css("display", "block")
						.css("position", "absolute")
						.css("width", labelWidth)
						.css("margin-top", "-" + viewFullsizeLabelPositionTop + "px")
						.css("margin-left", labelPosition)
						.fadeIn("normal");
                }, function()
                {
                    $ektron(this).children("span.viewFullsize").css("display", "block").fadeOut("normal");
                });

                //set toggle onHover over thumbnail "set default" button
                var thumbnailDefaultButton = $ektron("div.EktronMediaTabWrapper p.defaultThumbnail");
                thumbnailDefaultButton.hover(function()
                {
                    if ($ektron(this).hasClass("selectedThumbnail") === false)
                    {
                        $ektron(this).css("background-image", "url(images/defaultThumbnailHover.gif)");
                    }
                }, function()
                {
                    if ($ektron(this).hasClass("selectedThumbnail") === false)
                    {
                        $ektron(this).css("background-image", "url(images/defaultThumbnail.gif)");
                    }
                });
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
                cancel: function(editWrapper)
                {
                    editWrapper.find("input").remove();
                    editWrapper.find("span").css("display", "inline");
                    editWrapper.find("img.revise").css("display", "inline");
                    editWrapper.find("img.reviseOK").css("display", "none");
                    editWrapper.find("img.reviseCancel").css("display", "none");
                },
                ok: function(editWrapper)
                {
                    var span = editWrapper.find("span");
                    var input = editWrapper.find("input.editForm");
                    var newText = input.attr("value");
                    span.text(newText);
                    input.remove();
                    editWrapper.find("span").css("display", "inline");
                    editWrapper.find("img.revise").css("display", "inline");
                    editWrapper.find("img.reviseOK").css("display", "none");
                    editWrapper.find("img.reviseCancel").css("display", "none");

                    editWrapper.children("input").val(newText);

                    //update postback field
                    Ektron.Commerce.MediaTab.Images.getImageData();
                },
                showForm: function(editWrapper)
                {
                    editWrapper.find("img.revise").css("display", "none");
                    editWrapper.find("img.reviseOK").css("display", "inline");
                    editWrapper.find("img.reviseCancel").css("display", "inline");
                    var span = editWrapper.children("span");
                    var currentValue = span.text();
                    span.css("display", "none");
                    editWrapper.append("<input type=\"text\" class=\"editForm\" value=\"" + currentValue + "\" />");
                }
            },
            getImageData: function()
            {
                Ektron.Commerce.MediaTab.Images.imageData = [];
                $ektron("div.mediaGroup ul.images li.mediaItem").each(function(i)
                {
                    var mediaItem = $ektron(this);
                    var id = mediaItem.find("input.imageId").attr("value");
                    var title = Ektron.Commerce.MediaTab.escapeAndEncode(mediaItem.find("input.title").val());
                    var altText = Ektron.Commerce.MediaTab.escapeAndEncode(mediaItem.find("input.altText").val());
                    var path = Ektron.Commerce.MediaTab.escapeAndEncode(mediaItem.find("input.path").val());
                    var width = mediaItem.find("input.width").val();
                    var height = mediaItem.find("input.height").val();
                    var defaultImage = mediaItem.find("input.default").val();
                    var markedForDelete = mediaItem.find("input.markedForDelete").val();
                    var gallery = mediaItem.find("select.gallery option:selected").val();

                    //add image data
                    var image = {};
                    image.Id = id;
                    image.Title = title;
                    image.AltText = altText;
                    image.Path = path;
                    image.Width = width;
                    image.Height = height;
                    image.Default = defaultImage;
                    image.MarkedForDelete = markedForDelete;
                    image.Gallery = gallery;

                    var thumbnails = mediaItem.find("div.thumbnails tr.thumbnailRowWrapper td");
                    if (thumbnails.length > 0)
                    {
                        image.Thumbnails = [];
                        thumbnails.each(function(i)
                        {
                            var thumbnail = {
                                ImageName: $ektron(this).find("input.thumbnailImageName").attr("value"),
                                Title: Ektron.Commerce.MediaTab.escapeAndEncode($ektron(this).find("input.thumbnailTitle").val()),
                                Path: $ektron(this).find("input.thumbnailPath").attr("value"),
                                Height: $ektron(this).find("input.thumbnailHeight").attr("value"),
                                Width: $ektron(this).find("input.thumbnailWidth").attr("value")
                            };
                            image.Thumbnails.push(thumbnail);
                        });
                    }

                    Ektron.Commerce.MediaTab.Images.imageData.push(image);
                });

                //update postback field
                $ektron("div.EktronMediaTabWrapper input.mediaTabData").val(Ektron.JSON.stringify(Ektron.Commerce.MediaTab.Images.imageData));

                //alert($ektron("div.EktronMediaTabWrapper input.mediaTabData").val());
            },
            imageData: {},
            init: function()
            {
                //size the fullsize image wrapper (as square) based on the height of the attribute table
                Ektron.Commerce.MediaTab.Images.setFullsizeWrapperDimensions("all");

                //scale-down the fullsize image (if the image is larger) than the wrapper, or
                //position the fullsize image to the center/middle of the wrapper (if the image is smaller than the wrapper)
                var fullsizeImages = $ektron("div.EktronMediaTabWrapper ul.mediaList li.mediaItem table tbody th.mediaPreview img");
                Ektron.Commerce.MediaTab.Images.alignFullsizeImagesToWrapper(fullsizeImages);

                //bind hover event to image wrapper
                Ektron.Commerce.MediaTab.Images.bindEvents();

                //initialize imageData as Array
                Ektron.Commerce.MediaTab.Images.imageData = [];

                //populate imageData JSON object
                Ektron.Commerce.MediaTab.Images.getImageData();
            },
            markForDelete: function(buttonObj)
            {
                var button = $ektron(buttonObj);
                var mediaItem = button.parents("li.mediaItem");
                var markedForDeleteField = mediaItem.find("input.markedForDelete");

                if (markedForDeleteField.val() == "true")
                {
                    mediaItem.find("th.mediaPreview img").fadeTo("fast", 1);
                    mediaItem.find("p.actions a.deleteImageSelector img.delete").css("display", "block");
                    mediaItem.find("p.actions a.deleteImageSelector img.restore").css("display", "none");
                    mediaItem.find("p.actions a.deleteImageSelector").addClass("redHover").removeClass("blueHover");
                    mediaItem.find("p.actions a.defaultImageSelector").fadeIn("normal");
                    mediaItem.find("p.actions a.viewThumbnails").fadeIn("normal");

                    mediaItem.find("div.edit").each(function(i)
                    {
                        var editWrapper = $ektron(this);
                        if (editWrapper.children("span").css("display") === "none")
                        {
                            editWrapper.find("img.reviseCancel").fadeIn("normal");
                            editWrapper.find("img.reviseOK").fadeIn("normal");
                        }
                        else
                        {
                            editWrapper.find("img.revise").fadeIn("normal");
                        }
                    });
                    button.children("span").text("Mark For Delete");
                    markedForDeleteField.val("false");
                }
                else
                {
                    mediaItem.find("th.mediaPreview img").fadeTo("fast", 0.5);
                    mediaItem.find("p.actions a.deleteImageSelector img.delete").css("display", "none");
                    mediaItem.find("p.actions a.deleteImageSelector img.restore").css("display", "block");
                    mediaItem.find("p.actions a.deleteImageSelector").addClass("blueHover").removeClass("redHover");
                    mediaItem.find("p.actions a.defaultImageSelector").fadeOut("normal");
                    mediaItem.find("p.actions a.viewThumbnails").fadeOut("normal");
                    mediaItem.find("div.edit img").fadeOut("normal");
                    button.children("span").text("Restore");
                    markedForDeleteField.val("true");
                }

                //update postback field
                Ektron.Commerce.MediaTab.Images.getImageData();
            },
            setGalleryDisplay: function()
            {
                //update postback field
                Ektron.Commerce.MediaTab.Images.getImageData();
            },
            scaleImageToWrapper: function()
            {
                var scaled = false;
                var multiplier;
                var newHeight = 0;
                var newWidth = 0;
                var originalHeight = Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight;
                var originalWidth = Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth;

                if ((scaled === false) && (Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight > Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth))
                {
                    //image is taller than it is wide
                    newHeight = Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight;
                    multiplier = Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight / Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight;
                    newHeight = multiplier * Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight;
                    newWidth = multiplier * Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth;
                    scaled = true;
                }

                if ((scaled === false) && (Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight < Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth))
                {
                    //image is wider than it is tall
                    multiplier = Ektron.Commerce.MediaTab.Images.Dimensions.parentWidth / Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth;
                    newWidth = multiplier * Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth;
                    newHeight = multiplier * Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight;
                    scaled = true;
                }

                if ((scaled === false) && (Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight === Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth))
                {
                    //image is square
                    if (Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight >= Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight)
                    {
                        newWidth = Ektron.Commerce.MediaTab.Images.Dimensions.parentWidth;
                        newHeight = Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight;
                    }
                    if (Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight < Ektron.Commerce.MediaTab.Images.Dimensions.parentHeight)
                    {
                        newWidth = Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth;
                        newHeight = Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight;
                    }
                }
                // round off any weird fractions
                newHeight = Math.round(newHeight);
                newWidth = Math.round(newWidth);
                Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight = newHeight;
                Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth = newWidth;

                //set top & left of product image center & middle of parent
                Ektron.Commerce.MediaTab.Images.Dimensions.image.css("height", newHeight + "px");
                var heightCell = Ektron.Commerce.MediaTab.Images.Dimensions.image.parents("table.mediaImage").find("td.height");
                if (heightCell.find("span").length == 0)
                {
                    heightCell.html("<span>" + originalHeight + "px</span>");
                }

                Ektron.Commerce.MediaTab.Images.Dimensions.image.css("width", newWidth + "px");
                var widthCell = Ektron.Commerce.MediaTab.Images.Dimensions.image.parents("table.mediaImage").find("td.width");
                if (widthCell.find("span").length == 0)
                {
                    widthCell.html("<span>" + originalWidth + "px</span>");
                }
            },
            setDefault: function(obj)
            {
                var selectedImageButton = $ektron(obj);
                if (selectedImageButton.parents("li.mediaItem").find("input.default").val() == "true")
                { //unset
                    selectedImageButton.parents("ul.mediaList").find("li.mediaItem a.defaultImage").removeClass("defaultImage").children("span").text("Set as Product Icon");
                    selectedImageButton.parents("ul.mediaList").find("li.mediaItem input.default").val("false");
                }
                else
                {
                    //set all images to non-default image state
                    selectedImageButton.parents("ul.mediaList").find("li.mediaItem a.defaultImage").removeClass("defaultImage").children("span").text("Set as Product Icon");
                    selectedImageButton.parents("ul.mediaList").find("li.mediaItem input.default").val("false");

                    //set selected image to default image state
                    selectedImageButton.addClass("defaultImage").children("span").text("Unset as Product Icon");
                    selectedImageButton.parents("li.mediaItem").find("input.default").val("true");
                }
                //update postback field
                Ektron.Commerce.MediaTab.Images.getImageData();
            },
            setFullsizeImageDimensionLabels: function()
            {
                //set image dimension labels
                Ektron.Commerce.MediaTab.Images.Dimensions.image.parents("table.mediaImage").find("td.height span").text(Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight + "px");
                Ektron.Commerce.MediaTab.Images.Dimensions.image.parents("table.mediaImage").find("td.width span").text(Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth + "px");
                Ektron.Commerce.MediaTab.Images.Dimensions.image.parents("table.mediaImage").find("input.height").val(Ektron.Commerce.MediaTab.Images.Dimensions.imageHeight);
                Ektron.Commerce.MediaTab.Images.Dimensions.image.parents("table.mediaImage").find("input.width").val(Ektron.Commerce.MediaTab.Images.Dimensions.imageWidth);
            },
            setFullsizeImageType: function(srcPath)
            {
                if (srcPath !== null)
                {
                    //get extension type
                    var imageType = srcPath.split("").reverse().join("");
                    var extensionMarker = imageType.indexOf(".");
                    imageType = imageType.substring(0, extensionMarker);
                    imageType = imageType.split("").reverse().join("");
                    return imageType;
                }
            },
            setFullsizeWrapperDimensions: function(allOrLast)
            {
                //set image wrapper height and width
                var mediaPreviewWrappers = (allOrLast == "all") ? $ektron("div.EktronMediaTabWrapper li.mediaItem th.mediaPreview") : $ektron("div.EktronMediaTabWrapper li.mediaItem:last th.mediaPreview");

                $ektron.each(mediaPreviewWrappers, function()
                {
                    var currentMediaWrapper = $ektron(this);

                    currentMediaWrapper.find("img").css("height", "10px");
                    currentMediaWrapper.find("img").css("width", "10px");

                    var mediaAttributeTableHeight = currentMediaWrapper.parents("table.mediaImage").children("tbody").height();
                    currentMediaWrapper.css("height", mediaAttributeTableHeight - 2);
                    currentMediaWrapper.css("width", mediaAttributeTableHeight - 2);
                    currentMediaWrapper.find("img").css("height", "");
                    currentMediaWrapper.find("img").css("width", "");
                });
            },
            Thumbnails: {
                toggle: function(obj)
                {

                    var button = $ektron(obj);
                    var thumbnailWrapper = button.parents("table.mediaImage").next();
                    var thumbnailImageWrapper = button.parents("table.mediaImage").next().find("div.thumbnailImageWrapper");

                    //set the width of the thumbnail container for ie to force overflow
                    if (!$ektron.browser.mozilla)
                    {
                        thumbnailImageWrapper.css("width", thumbnailImageWrapper.parents("li").width() - ((Ektron.Commerce.MediaTab.emSize * 4)));
                    }

                    //update show/hide label
                    var buttonLabel = button.children("span");

                    if (thumbnailWrapper.css("display") === "none")
                    {
                        buttonLabel.text("Hide Thumbnails");
                    }
                    else
                    {
                        buttonLabel.text("View Thumbnails");
                    }

                    //show thumbnails
                    thumbnailWrapper.slideToggle("normal", function()
                    {
                        thumbnailImageWrapper.find("img.thumbnail").each(function(i)
                        {
                            var currentImage = $ektron(this);
                            var height = currentImage.height();
                            var width = currentImage.width();

                            var elementHeight = currentImage.parent().parent().find("span.height");
                            var elementWidth = currentImage.parent().parent().find("span.width");

                            elementHeight.text(height + "px");
                            elementWidth.text(width + "px");

                            elementHeight.fadeIn("normal");
                            elementWidth.fadeIn("normal");

                            var thumbHeight = currentImage.parent().parent().find("input.thumbnailHeight");
                            var thumbWidth = currentImage.parent().parent().find("input.thumbnailWidth");

                            if (height > 0)
                                thumbHeight.val(height);
                            if (width > 0)
                                thumbWidth.val(width);
                        });
                    });
                }
            }
        }
    };
}