/* 
*  Depends:
*  jquery.ui.core.js
*  ektron-jquery.ui.datepicker.js
*  ektron-validate.js
*  ektron-maskedInput-helpers.js
*/

$ektron.validator.addMethod("required", function (value, element, param) {
    if ($ektron(element).is(".hasDatepicker")) {
        if ($ektron(".ui-datepicker:visible").length > 0 && value != "") {
            return true;
        }
    }
    // check if dependency is met
    if (!this.depend(param, element))
        return "dependency-mismatch";
    switch (element.nodeName.toLowerCase()) {
        case 'select':
            // could be an array for select-multiple or a string, both are fine this way
            var val = $(element).val();
            return val && val.length > 0;
        case 'input':
            if (this.checkable(element))
                return this.getLength(value, element) > 0;
        default:
            return $.trim(value).length > 0;
    }
});