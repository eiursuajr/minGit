//define Ektron object only if it's not already defined
if (typeof(Ektron) == "undefined")
{
	Ektron = {};
}

//define Ektron.Workarea object only if it's not already defined
if (typeof(Ektron.Workarea) == "undefined")
{
	Ektron.Workarea = {};
}

//define Ektron.Workarea.SearchBox object only if it's not already defined
if (typeof(Ektron.Workarea.SearchBox) == "undefined")
{
    Ektron.Workarea.SearchBox =
    {
        init: function()
        {
            if (typeof($ektron) != "undefined" && typeof($ektron.fn.inputLabel) != "undefined") {
                $ektron("#txtSearch").inputLabel();
                if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) < 8) {
                    $ektron("input.ektronWorkareaSearch").attr("value", "");
                }
            }
            else
            {
                window.setTimeout("Ektron.Workarea.SearchBox.init()", 100);
            }
        }
    }
}

Ektron.ready(function()
{
    Ektron.Workarea.SearchBox.init();
});