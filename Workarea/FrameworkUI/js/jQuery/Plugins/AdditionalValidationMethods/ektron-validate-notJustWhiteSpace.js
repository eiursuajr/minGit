$ektron.validator.addMethod("notjustwhitespace", function (value, element) {
    return /(?:^$)|\S/i.test(value);
}, "Not just white space please");