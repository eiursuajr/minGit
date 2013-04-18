if ("undefined" == typeof Ektron) { Ektron = {}; }
if ("undefined" == typeof Ektron.Namespace) {
    Ektron.Namespace = {
        // Register checks if a namespace already exists, adn if not creates it
        Register: function (namespace) {
            namespace = namespace.split('.');

            if (!window[namespace[0]]) {
                window[namespace[0]] = {};
            }

            var strFullNamespace = namespace[0];
            for (var i = 1; i < namespace.length; i++) {
                strFullNamespace += "." + namespace[i];
                eval("if(!window." + strFullNamespace + ")window." + strFullNamespace + "={};");
            }
        },

        // Checks to see if a given namespace already exists.  accepts the string representing the namespace as a parameter.
        Exists: function (namespace) {
            var exists = false;
            try {
                var namespace = eval(namespace);
                if (namespace) {
                    exists = true;
                }
            }
            catch (e) {
                return exists
            }

            return exists;
        }
    }
}