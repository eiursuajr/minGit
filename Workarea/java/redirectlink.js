if ("undefined" == typeof (Ektron)) { Ektron = {}; }
if ("undefined" == typeof (Ektron.Controls)) { Ektron.Controls = {}; }
if ("undefined" == typeof (Ektron.Controls.FormBlock)) { Ektron.Controls.FormBlock = {}; }
if ("undefined" == typeof (Ektron.Controls.FormBlock.Utility)) { Ektron.Controls.FormBlock.Utility = {}; }
if ("undefined" == typeof (Ektron.Controls.FormBlock.Utility.RedirectLink)) 
{
    Ektron.Controls.FormBlock.Utility.RedirectLink = {
        //methods
        init: function ()
        {
            if (document.links.length > 0)
            {
	            var objAElem = document.links[0];
	            for (var i = 0; i < document.links.length; i++)
	            {
		            objAElem = document.links[i];
		            if ("RedirectionLink" == objAElem.id)
		            {
		                var win = this.getTargetWindow(objAElem);
	                    win.location.href = objAElem.href;
                        break;
		            }
	            }
            }
        },
        getTargetWindow: function (objAElem)
        {
            // objAElem: reference to A (hyperlink) element
            var win = window;
            if (!objAElem) return win;
            var target = objAElem.target;
            if (typeof target != "string") return win;
            switch (target.toLowerCase())
            {
                case "":
                    win = window;
                    break;
                case "_blank":
                    win = window.open("", target);
                    break;
                case "_parent":
                    win = window.parent;
                    break;
                case "_self":
                    win = window.self;
                    break;
                case "_top":
                    win = window.top;
                    break;
                default:
                    win = this.findFrame(target);
                    break;
            }
            if (!win) win = window;
            return win;
        },
        findFrame: function (name)
        {
            switch (typeof name)
	        {
	        case "string":
		        if (name.length > 0)
		        {
			        return this.rfnFindFrame(name, top);
		        }
		        else
		        {
			        return window;
		        }
		        break;
	        case "number":
		        return window.frames[name];
		        break;
	        case "undefined":
		        return window;
		        break;
	        default:
		        return null;
		        break;
	        }
	        return null;
        },
        rfnFindFrame: function (name, objWindow)
        {
            var win = objWindow.frames[name];
	        if (win) return win;
	        for (var i = 0; i < objWindow.frames.length; i++)
	        {
		        win = this.rfnFindFrame(name, objWindow.frames[i]);
		        if (win) return win;
	        }
	        return null;
        }
    };
}
