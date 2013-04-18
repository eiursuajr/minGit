/*
* Ektron UI ToTop
*
* Copyright (c) 2011, 
* Dual licensed under the MIT or GPL licenses.
* http://www.opensource.org/licenses/mit-license.php
* http://www.gnu.org/licenses/gpl.html
*
* Depends:
*	ektron-jquery-1.4.4.js
*/

(function ($)
{
    $.fn.toTop = function ()
    {
        this.prependTo("form:first");
        return this;
    };
})($ektron);
