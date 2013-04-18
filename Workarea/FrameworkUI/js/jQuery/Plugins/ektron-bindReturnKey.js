(function ($) {
    var methods = {
        init: function (callback) {
            this.bind("keypress.bindReturnKey", function (event) {
                if (event.which == '13') // return key pressed?
                {
                    if (null !== callback) {
                        callback(event, this);
                    }
                    return false;
                }
            });
        },
        destroy: function () {
            return this.each(function () {
                $(window).unbind(".bindReturnKey");
            })
        }
    };
    $.fn.bindReturnKey = function (method) {
        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else if (typeof method === 'function') {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.bindReturnKey');
        }
    };
})($ektron);