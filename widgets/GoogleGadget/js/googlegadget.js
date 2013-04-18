function GadgetEscapeHTML(id)
{
    var tbData = $ektron("#"+id);
    var result;
    if(tbData.length) {  
    result = tbData.val();
        // less-thans (<)	    result = result.replace(/\</g,'&lt;');	    // greater-thans (>)	    result = result.replace(/\>/g,'&gt;');
	    tbData.val(result);
    }    
  return true;
}

if (typeof (Ektron) == "undefined") Ektron = {};
if (typeof (Ektron.Widget) == "undefined") Ektron.Widget = {};
if (typeof (Ektron.Widget.GoogleGadget) == "undefined") {
    Ektron.Widget.GoogleGadget =
    {
        Init: function(id, gadgets) {
            var ul = $ektron("#" + id + " ul");

            var alt = false;
            for (var i in gadgets) {
                var gadget = gadgets[i];
                var li = Ektron.Widget.GoogleGadget.MakeGadget(gadget);
                li.addClass((alt = !alt) ? "alt1" : "alt2");
                li.hover(function() {
                    $(this).addClass("hover");
                },
                function() {
                    $(this).removeClass("hover");
                });
                li.click((function(gadget) {
                    return function() {
                        var textarea = $("#" + id + " textarea");
                        if (textarea.length > 0) {
                            textarea.attr("value", gadget.embed);
                        }
                    };
                })(gadget));
                ul.prepend(li.get(0));
            }
        },

        MakeGadget: function(gadget) {
            var li = $("<li></li>");
            var img = $("<img></img>");
            var a = $("<a></a>");
            var span = $("<span></span>");

            img.addClass("gadget-thumbnail");
            img.attr("src", gadget.thumbnailUrl);
            img.attr("alt", gadget.title);

            a.attr("href", "javascript://");
            a.html(gadget.title);

            span.addClass("gadget-description");
            span.html(gadget.description);

            li.append(img.get(0));
            li.append(a.get(0));
            li.append(span.get(0));

            return li;
        }
    };
}

if( typeof Sys != "undefined" ){
   Sys.Application.notifyScriptLoaded();
}