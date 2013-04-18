/* Copyright Ektron, Inc. 07/12/07 */function eWebEditProUtil(){this.lTrim=eWebEditProUtil_lTrim;this.rTrim=eWebEditProUtil_rTrim;
 this.trim=eWebEditProUtil_trim;this.HTMLEncode=eWebEditProUtil_HTMLEncode;this.isOpenerAvailable=eWebEditProUtil_isOpenerAvailable;
 this.getOpenerInstance=eWebEditProUtil_getOpenerInstance;this.languageCode=eWebEditProUtil_getLanguageCode();
 this.queryArgs=[];var objQuery=eWebEditProUtil_parseQuery();for (var p in objQuery){
 this.queryArgs[this.queryArgs.length]=objQuery[p];this.queryArgs[p]=objQuery[p];
 } this.editorName=this.queryArgs["editorName"];if ("undefined"==typeof this.editorName){
 this.editorName=this.queryArgs["editorname"];}}
function eWebEditProUtil_trim(s){var s=s +"";s=eWebEditProUtil_lTrim(s);s=eWebEditProUtil_rTrim(s);
 return s;}
function eWebEditProUtil_lTrim(s){var s=s +"";s=s.replace(/^\s+/,"");return s;}
function eWebEditProUtil_rTrim(s){var s=s +"";s=s.replace(/\s+$/,"");return s;}
function eWebEditProUtil_HTMLEncode(s){var strHTML=s +"";strHTML=strHTML.replace(/\&/g,"&amp;");
 strHTML=strHTML.replace(/\</g,"&lt;");strHTML=strHTML.replace(/\>/g,"&gt;");strHTML=strHTML.replace(/\"/g,"&quot;");
 return strHTML;}
function eWebEditProUtil_getLanguageCode(){var strLanguageCode="";if (navigator.language){
 strLanguageCode=navigator.language;} if (navigator.userLanguage){strLanguageCode=navigator.userLanguage;
 } var strTranslatedLangCodes="zh-tw";if (strTranslatedLangCodes.indexOf(strLanguageCode)==-1){
 strLanguageCode=strLanguageCode.substring(0,2);var strTranslatedLanguages="ar,da,de,es,fr,he,it,ja,ko,nl,pt,ru,sv,zh";
 if (strTranslatedLanguages.indexOf(strLanguageCode)==-1){strLanguageCode="";} } return strLanguageCode;
}
function eWebEditProUtil_isOpenerAvailable(){if (top.opener && !(top.opener.closed) && top.opener.eWebEditPro){
 return true;} else{return false;}}
function eWebEditProUtil_getOpenerInstance(editorName){if (!editorName) editorName=this.editorName;
 if (this.isOpenerAvailable() && editorName){return top.opener.eWebEditPro.instances[editorName];
 } else{return null;}}
function eWebEditProUtil_parseQuery(){var objQuery=new Object();var strQuery=top.location.search.substring(1);
 var aryQuery=strQuery.split("&");var pair=[];for (var i=0;i < aryQuery.length;i++){
 pair=aryQuery[i].split("=");if (2==pair.length){objQuery[unescape(pair[0])]=unescape(pair[1]);
 } } return objQuery;}
var eWebEditProUtil=new eWebEditProUtil;