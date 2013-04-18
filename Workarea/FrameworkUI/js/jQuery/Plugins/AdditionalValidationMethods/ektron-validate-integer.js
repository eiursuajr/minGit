$ektron.validator.addMethod("integer", function (value, element) {
    var isInteger = false;

    // ensure the globalization plugin is loaded
    if ("undefined" == typeof ($ektron.global.parseInt)) {
        throw "The Globalization plugin must be loaded to use this method.";
        return isInteger;
    }
    try {
        var tryInteger = $ektron.global.parseInt(value);
        if (tryInteger != null) {
            isInteger = true;
        }
    }
    catch (e) {
        return false;
    }
    return this.optional(element) || isInteger;
}, "Please enter a correctly formatted decimal.");