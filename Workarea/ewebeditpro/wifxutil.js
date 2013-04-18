/* Copyright Ektron, Inc. 12/05/06 */function WebImageFXUtil(){this.trim=WebImageFXUtil_trim;this.lTrim=WebImageFXUtil_lTrim;
 this.rTrim=WebImageFXUtil_rTrim;this.HTMLEncode=WebImageFXUtil_HTMLEncode;this.isOpenerAvailable=WebImageFXUtil_isOpenerAvailable;
 this.getOpenerInstance=WebImageFXUtil_getOpenerInstance;this.languageCode=WebImageFXUtil_getLanguageCode();
 this.queryArgs=[];var objQuery=WebImageFXUtil_parseQuery();for (var p in objQuery){
 this.queryArgs[this.queryArgs.length]=objQuery[p];this.queryArgs[p]=objQuery[p];
 } this.editorName=this.queryArgs["editorName"];if ("undefined"==typeof this.editorName){
 this.editorName=this.queryArgs["editorname"];}}
function WebImageFXUtil_trim(s){var s=s +"";s=WebImageFXUtil_lTrim(s);s=WebImageFXUtil_rTrim(s);
 return s;}
function WebImageFXUtil_lTrim(s){var s=s +"";s=s.replace(/^\s+/,"");return s;}
function WebImageFXUtil_rTrim(s){var s=s +"";s=s.replace(/\s+$/,"");return s;}
function WebImageFXUtil_HTMLEncode(s){var strHTML=s +"";strHTML=strHTML.replace(/\&/g,"&amp;");
 strHTML=strHTML.replace(/\</g,"&lt;");strHTML=strHTML.replace(/\>/g,"&gt;");strHTML=strHTML.replace(/\"/g,"&quot;");
 return strHTML;}
function WebImageFXUtil_getLanguageCode(){var strLanguageCode="";if (navigator.language){
 strLanguageCode=navigator.language;} if (navigator.userLanguage){strLanguageCode=navigator.userLanguage;
 } var strTranslatedLangCodes="zh-tw";if (strTranslatedLangCodes.indexOf(strLanguageCode)==-1){
 strLanguageCode=strLanguageCode.substring(0,2);var strTranslatedLanguages="ar,da,de,es,fr,he,it,ja,ko,nl,pt,ru,sv,zh";
 if (strTranslatedLanguages.indexOf(strLanguageCode)==-1){strLanguageCode="";} } return strLanguageCode;
}
function WebImageFXUtil_isOpenerAvailable(){if (top.opener && !(top.opener.closed) && top.opener.eWebEditPro){
 return true;} else{return false;}}
function WebImageFXUtil_getOpenerInstance(editorName){if (!editorName) editorName=this.editorName;
 if (this.isOpenerAvailable() && editorName){return top.opener.WebImageFX.instances[editorName];
 } else{return null;}}
function WebImageFXUtil_parseQuery(){var objQuery=new Object();var strQuery=top.location.search.substring(1);
 var aryQuery=strQuery.split("&");var pair=[];for (var i=0;i < aryQuery.length;i++){
 pair=aryQuery[i].split("=");if (2==pair.length){objQuery[unescape(pair[0])]=unescape(pair[1]);
 } } return objQuery;}
var WebImageFXUtil=new WebImageFXUtil;