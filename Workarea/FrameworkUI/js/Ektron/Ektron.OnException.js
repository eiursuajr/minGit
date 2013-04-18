Ektron.OnException = function(me, onexception, ex, args, callee)
/*
callee is optional
USAGE: (not a class; this is an event, do not use 'new')
function myFunction(arg1, arg2, ..., onexception)
{
try
{
// code
}
catch (ex)
{
Ektron.OnException(this, onexception, ex, arguments);
}
try
{
callback(arg);
}
catch (ex)
{
Ektron.OnException(this, onexception, ex, [arg], callback);
}
}
myObject.myFunction(arg0, arg1, Ektron.OnException.returnValue(null)); // return null if error
myObject.myFunction(arg0, arg1, Ektron.OnException.returnArgument(0)); // return arg0 if error
myObject.myFunction(arg0, arg1, Ektron.OnException.returnException); // return error message if error
myObject.myFunction(arg0, arg1, function(ex, args) // custom response if error
{
return "An error occurred. arg1=" + args[1] + " Error=" + ex.message;
});
*/
{
    var returnValue; // default undefined
    var method = function() { };
    if (callee) {
        method = callee;
    }
    else if (args && args.callee) {
        method = args.callee;
    }
    var onexceptionList =
        [
			Ektron.onexception // namespace
        , me.constructor.onexception // class
        , me.onexception // object
        , method.onexception // method
        , onexception // argument, normally args[args.length-1]
        ];
    for (var i = 0; i < onexceptionList.length; i++) {
        var onexception = onexceptionList[i];
        if ("function" == typeof onexception && (0 == i || onexception != onexceptionList[i - 1])) {
            var result = onexception.call(me, ex, args, callee);
            if (typeof result != "undefined") returnValue = result;
        }
    }
    if ("undefined" == typeof returnValue) {
        throw ex;
    }
    return returnValue;
};
Ektron.OnException.exceptionMessage = function(ex, args, callee)
/*
args is optioal
callee is optional
*/
{
    var msg = "";
    try {
        // Have seen 'ex' be a string in FF 2.
        msg = ex.message || ex || "";
        msg = msg.replace("&lt;br /&gt;", "\n").replace("&lt;hr /&gt;", "\n");
    }
    catch (exIgnore) { };
    var file = "";
    try {
        file = ex.filename || ex.fileName || ex.sourceURL || "";
        if (file) file = "\nFile: " + file;
    }
    catch (exIgnore) { };
    var line = "";
    try {
        // Have see "permission denied" error in FF 2 when attempting to read ex.lineNumber
        line = ex.lineNumber || ex.line || "";
        if (line) line = "\nLine: " + line;
    }
    catch (exIgnore) { };
    var func = "";
    try {
        if (callee || (args && args.callee)) {
            var funcCode = String(callee || args.callee);
            var funcCodeLines = funcCode.split("\n");
            var max = Ektron.OnException.exceptionMessage.maxLinesOfCode;
            if (funcCodeLines && funcCodeLines.length > max) {
                funcCodeLines.splice(max, funcCodeLines.length - max);
                funcCode = funcCodeLines.join("\n") + "...";
            }
            func += "\n\nFunction:\n" + funcCode + "\n";
            if (args && args.length > 0) {
                func += "Arguments:\n";
            }
        }
        if (args && args.length) {
            for (var i = 0; i < args.length; i++) {
                var arg = args[i];
                if (arg && "object" == typeof arg) {
                    if (/* ELEMENT_NODE */1 == arg.nodeType) {
                        var show = "[HTMLElement " + arg.tagName;
                        if (arg.id) show += " id=\"" + arg.id + "\"";
                        if (arg.className) show += " class=\"" + arg.className + "\"";
                        show += "]";
                        arg = show;
                    }
                    else if (/* TEXT_NODE */3 == arg.nodeType) {
                        arg = "[Node \"" + arg.nodeValue + "\"]";
                    }
                }
                func += "\n" + arg + "\n";
            }
        }
    }
    catch (exIgnore) { };
    return msg + file + line + func;
};
Ektron.OnException.exceptionMessage.maxLinesOfCode = 24;
Ektron.OnException.ignoreException = function(ex, args, callee) { return null; };
Ektron.OnException.throwException = function(ex, args, callee) { throw ex; };
Ektron.OnException.returnException = function(ex, args, callee) { return Ektron.OnException.exceptionMessage(ex); };
Ektron.OnException.returnValue = function(v) {
    return function(ex, args, callee) { return v; };
};
Ektron.OnException.returnArgument = function(n) {
    return function(ex, args, callee) { if (args && args.length > n) return args[n]; };
};
Ektron.OnException.alertException = function(ex, args, callee) { alert(Ektron.OnException.exceptionMessage(ex, args, callee)); };
Ektron.OnException.consoleException = function(ex, args, callee) { if (typeof console != "undefined") console.error(Ektron.OnException.exceptionMessage(ex, args, callee)); /* firebug */ };
Ektron.OnException.diagException = function(ex, args, callee) {
    if (document != null && document.cookie && document.cookie.indexOf("Ektron.diagException=") > -1) {
        if (confirm("Click OK to debug or Cancel to continue.\n\n" + Ektron.OnException.exceptionMessage(ex, args, callee))) {
            //YUI compression fails to compress if the debugger command is in there wholesale
            eval('debugg' + 'er');
        }
    }
    return null;
}