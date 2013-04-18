$.validator.addMethod("regexp", function (value, element, regexp)
{
    if (regexp.constructor != RegExp)
    {
        regexp = new RegExp(regexp);
    }
    else
    {
        if (regexp.global)
        {
            regexp.lastIndex = 0;
        }
    }
    return this.optional(element) || regexp.test(value);
}, "This form field is invalid.");