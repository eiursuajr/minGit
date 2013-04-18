/*  Use this namespaced object to access any of the QueryString parameters
        by key.  For example:
        Given the URL http://www.ektron.com?id=224
        To alert the key "id" value we would simply do the following
        Ektron.ready(function()
            {
                alert(Ektron.QueryString["id"]);
            }
        );
*/

Ektron.QueryString = {};
window.location.search.replace(new RegExp("([^?=&]+)(=([^&]*))?", "g"),
    function($0, $1, $2, $3) {
        Ektron.QueryString[$1] = $3;
    }
);