/* Copyright Ektron, Inc. 05/20/09 */function eWebEditProOverrider(){this.originalFunctions=[];this.overrideFunction=eWebEditProOverrider_overrideFunction;
 this.callOriginalFunction=eWebEditProOverrider_callOriginalFunction;this.NOERROR="noerror";
 this.ERROR_API="api";this.ERROR_NOTDEFINED="notdefined";}
function eWebEditProOverrider_overrideFunction(strFunctionName,strNewFunctionName){
 var errCode=this.NOERROR;if (0==this.originalFunctions.length){this.originalFunctions=new Array();
 } eval('var bIsFunction = ("function" == typeof ' + strFunctionName +')');if (bIsFunction){
 var fnOrig=eval(strFunctionName);var fnNew=eval(strNewFunctionName);if (fnOrig !=fnNew){
 this.originalFunctions[this.originalFunctions.length]=fnOrig;this.originalFunctions[strFunctionName]=fnOrig;
 eval(strFunctionName +" = fnNew");} } else{alert(strFunctionName +" is not defined.");
 errCode=this.ERROR_NOTDEFINED;} return errCode;}
function eWebEditProOverrider_callOriginalFunction(strFunctionName,objArguments){
 var fnOrig=this.originalFunctions[strFunctionName];if ("function"==typeof fnOrig){
 var result=fnOrig.apply(window,objArguments);return result;} else{alert(strFunctionName +" is not defined.");
 return;}}
var g_eWebEditProOverrider=null;if ("function"==typeof Page_ClientValidate){if (!g_eWebEditProOverrider){
 g_eWebEditProOverrider=new eWebEditProOverrider();} g_eWebEditProOverrider.overrideFunction("Page_ClientValidate","eWebEditPro_Page_ClientValidate");
}
if ("function"==typeof __doPostBack){if (!g_eWebEditProOverrider){g_eWebEditProOverrider=new eWebEditProOverrider();
 } g_eWebEditProOverrider.overrideFunction("__doPostBack","eWebEditPro___doPostBack");
}
function eWebEditPro_Page_ClientValidate(){if (eWebEditPro){eWebEditPro.save();} return g_eWebEditProOverrider.callOriginalFunction("Page_ClientValidate",arguments);
}
function eWebEditPro___doPostBack(eventTarget,eventArgument){if (eWebEditPro_save()){
 g_eWebEditProOverrider.callOriginalFunction("__doPostBack",arguments);}}
function eWebEditPro_save(){if (eWebEditPro){return eWebEditPro.save();} else{return confirm('eWebEditPro is null or undefined. eWebEditPro content cannot be posted.');
 }}
