$ektron.validator.addMethod("decimal", function (value, element) {
    var isDecimal = false;

    // ensure the globalization plugin is loaded
    if ("undefined" == typeof ($ektron.global.parseFloat)) {
        throw "The Globalization plugin must be loaded to use this method.";
        return isDecimal;
    }
    try {
        var tryDecimal = $ektron.global.parseFloat(value);
        if (tryDecimal != null) {
            isDecimal = true;
        }
    }
    catch (e) {
        return false;
    }
    return this.optional(element) || isDecimal;
}, "Please enter a correctly formatted decimal.");