/*
* Depends:
*  jQuery.validation.js
*  jquery.ui.core.js
*  ektron.glob.js
*  optional: ektron-maskedInput-helpers.js
*/
//  This method tests if the date is valid based on the client side accessible Localization date format string

$ektron.validator.addMethod("dateByFormat", function (value, element) {
    var isDate = false;
    var elementObj = $ektron(element);
    var culture = elementObj.data()["ektron-global-culture"] ? elementObj.data()["ektron-global-culture"] : "default";
    // ensure the globalization plugin is loaded
    if ("undefined" == typeof ($ektron.global.parseDate)) {
        throw "The Globalization plugin must be loaded to use this method.";
        return isDate;
    }
    try {
        // if the value is the an empty string return true
        if (value == "") {
            return true;
        }
        var tryDate = $ektron.global.parseDate(value, "d", culture);
        if (tryDate != null) {
            isDate = true;
        }
    }
    catch (e) {
        return false;
    }
    return this.optional(element) || isDate;
}, "Please enter a correctly formatted date.");