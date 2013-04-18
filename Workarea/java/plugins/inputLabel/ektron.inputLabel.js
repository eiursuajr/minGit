/**
 * jQuery Initial input value replacer
 *
 * Sets input value attribute to a starting value
 * Based on the work by
 * @initial author Marco "DWJ" Solazzi - hello@dwightjack.com
 *  modified and adapted for Ektron by Keith M. Pepin
 * @license  Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php) and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
 * @copyright Copyright (c) 2008 Marco Solazzi
 * @version 1.4
 * @requires jQuery 1.2.x
 */
(function($)
{
/*
    The plugin takes a single parameter.

    @param {Object} [options] An object containing one or more of the following options:

    * cssClass:            class that defines the look of the label text and determines
                           if the value has been changed,
                           default: "ektronInputLabelDefaultState";

    * e:                   event which triggers initial text clearing,
                           default: "focus";

    * force:               execute this script even if input value is not empty,
                           default: false;

    * keep:                if value of field is empty on blur, re-apply initial text,
                           default: true;

    * clearInputTrigger:   a selector that will be bound with an onclick event to clear the
                           text input if it currently has the CSS class defined by cssClass.
                           default: false;

    * text:                Initial value displayed in the field.  Can be either a string,
                           an Ektron Library reference [ example: $ektron("#elementX") ],
                           or boolean false (default) to search for the related label,
                           default: false;

     @methods the InputLabel plugin exposes the following methods

     * clearInputLabel:    clears the field if the label is showing.  Useful if you are
                           programatically submitting the form via AJAX for example.

     * getInputLabelValue: gets the value of the field if it has been changed,
                           otherwise returns empty string.  Useful for validation or
                           programatic submissions of data via AJAX.

     @properties the InputLabel plugin exposes the following properties

     * $.fn.inputLabel.cssClass, the default class used when the cssCLass option is not used.
*/
    var hasLabel = "ektronInputLabelShown";
    $.fn.inputLabel = function(options)
    {
        var o = $.extend(
            {
	            cssClass: $.fn.inputLabel.cssClass,
	            clearInputTrigger: false,
	            e: "focus",
	            force: false,
	            keep: true
	        },
		    options || {}
		);

        return this.each(function ()
		{

		    var t = $(this);
		    var innerText = (o.text || false);
		    var caller = "";
		    var clearInput = function(e)
		    {
		        if(e.data)
		        {
		            if (e.data.caller)
		            {
		                caller = e.data.caller;
		            }
		            else
		            {
		                caller = "";
		            }
		        }
		        var value = $.trim(t.val());
			    if (e.type === o.e && t.hasClass(hasLabel) === true)
			    {
				    t.removeClass(o.cssClass).removeClass(hasLabel);
				    t.val("");
				    if (!o.keep)
				    {
					    t.unbind(o.e+" blur",clearInput);
				    }
			    }
			    else if ((caller === "__doPostBack" ||  caller === "WebForm_DoPostBackWithOptions") && t.hasClass(hasLabel) === true)
			    {
			        t.val("");
			        if (caller == "WebForm_DoPostBackWithOptions")
			        {
			            window.setTimeout(function()
			            {
			                t.each(function(i)
			                {
			                    $(this).addClass(o.cssClass + " " + hasLabel).val(innerText);
			                });
			            }, 20);
			        }
			    }
			    else if (e.type == "blur" && value == "" && o.keep)
			    {
				    t.addClass(o.cssClass + " " + hasLabel).val(innerText);
			    }
			    else if (e.type == "submit" && t.hasClass(hasLabel))
			    {
			        t.val("");
			    }
		    };

		    if (!innerText)
		    {
			    var id = t.attr("id");
			    innerText = $(this).parents("form").find("label[for=" + id + "]").hide().text();
		    }
		    else
		    {
			    if (typeof innerText != "string")
			    {
				    innerText = $ektron(innerText).text();
			    }
			}
			innerText = $ektron.trim(innerText);
			if (o.force || t.val() == "")
			{
				t.addClass(o.cssClass + " " + hasLabel).val(innerText);
			}
			t.bind(o.e+" blur",clearInput);
			t.parents("form").bind("submit", clearInput);

			//play nice with ms-ajax && ms js library
			if (typeof __doPostBack == "function")
			{
			    $("*[href*='__doPostBack'], *[onclick*='__doPostBack'], *[onchange*='__doPostBack']").bind("click", {caller: "__doPostBack"}, clearInput);
			}
			if (typeof WebForm_DoPostBackWithOptions == "function")
			{
			    $("*[href*='WebForm_DoPostBackWithOptions'], *[onclick*='WebForm_DoPostBackWithOptions'], *[onchange*='WebForm_DoPostBackWithOptions']").bind("click", {caller: "WebForm_DoPostBackWithOptions"}, clearInput);
			}

			if (o.clearInputTrigger !== false && o.clearInputTrigger.length > 0)
			{
			    $(o.clearInputTrigger).each(function(e){
			        $(this).bind("click", clearInput);
			    });
			}
		});
    };
    // provide property to read the default class value
    $.fn.inputLabel.cssClass = "ektronInputLabel";

	// define and expose the clearInputLabel method
	$.fn.clearInputLabel = function()
	{
	    if($(this).hasClass(hasLabel))
	    {
	        $(this).val("");
	    }
	};

	// define and expose the clearInputLabel method
	$.fn.getInputLabelValue = function()
	{
	    if($(this).hasClass(hasLabel))
	    {
	        return "";
	    }
	    else
	    {
	        return $(this).val();
	    }
	};
})($ektron);