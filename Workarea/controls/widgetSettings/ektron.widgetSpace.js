

    function SelectAllWidgets()
    {
        var widgets = $ektron(".widget");
        widgets.each(function (i)
        {
            var widget = $ektron(widgets[i]);
            var checkbox = widget.find("input");
            if(!checkbox.is(":checked"))
            {
                widget.addClass("selected");
                ToggleCheckbox(checkbox);
            }
        });
    }

    function SelectWidgets(widgetIds)
    {
        for(var i in widgetIds)
        {
            var id = widgetIds[i];
            SelectWidget(id);
        }
    }

    function SelectWidget(id)
    {
        var checkbox = $ektron(".widget input#widget" + id);
        var widget = checkbox.parent(".widget");
        if(!checkbox.is(":checked"))
        {
            widget.addClass("selected");
            ToggleCheckbox(checkbox);
        }
    }

    function UnselectAllWidgets()
    {
        var widgets = $ektron(".widget");
        widgets.each(function (i)
        {
            var widget = $ektron(widgets[i]);
            var checkbox = widget.find("input");
            if(checkbox.is(":checked"))
            {
                widget.removeClass("selected");
                ToggleCheckbox(checkbox);
            }
        });
    }

    function ToggleCheckbox(checkbox) {
        if (checkbox.is(":checked")) {
            checkbox.removeAttr("checked");
        }
        else {
            checkbox.attr("checked", "checked");
        }
    }

    Ektron.ready(function ()
    {
        // Initialize widgetType display
        var widgets = $ektron(".widget");
        widgets.each(function (i)
        {
            var widget = $(widgets[i]);
            if(widget.find("input").is(":checked"))
            {
                widget.addClass("selected");
            }

            widget.click(function ()
            {
                var widgetCheckbox = widget.find("input");

                ToggleCheckbox(widgetCheckbox);
                if(widgetCheckbox.is(":checked"))
                {
                    widget.addClass("selected");
                }
                else
                {
                    widget.removeClass("selected");
                }
            });
        });
    });