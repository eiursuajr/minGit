/* Copyright Ektron, Inc. 12/05/06 */function CEktMonarch(){this.aryAutoInstall=[];this.requestAutoInstall=CEktMonarch_requestAutoInstall;
 this.autoInstallDone=CEktMonarch_autoInstallDone;}
function CEktMonarch_requestAutoInstall(objAutoInstall){if (!objAutoInstall || typeof objAutoInstall !="object"){
 return;} for (var iCounter=0;iCounter < this.aryAutoInstall.length;iCounter++){if (objAutoInstall==this.aryAutoInstall[iCounter]){
 return;} } this.aryAutoInstall.push(objAutoInstall);if (1==this.aryAutoInstall.length){
 if ("function"==typeof objAutoInstall.doInstall){objAutoInstall.doInstall();} }}
function CEktMonarch_autoInstallDone(){var bDoNext;do{bDoNext=false;this.aryAutoInstall.shift();
 if (0==this.aryAutoInstall.length){window.history.go(0);} else{var objAutoInstall=this.aryAutoInstall[0];
 if ("function"==typeof objAutoInstall.doInstall){objAutoInstall.doInstall();} else{
 bDoNext=true;} } } while (true==bDoNext);}
if ("undefined"==typeof ektMonarch){ektMonarch=new CEktMonarch;}
function JSEventModel(){this.eventHandlers=[];this.initEvent=JSEventModel_initEvent;
 this.raiseEvent=JSEventModel_raiseEvent;this.addEventHandler=JSEventModel_addEventHandler;
 this.invokeEventHandler=JSEventModel_invokeEventHandler;}
function JSEventModel_initEvent(eventName){this.event=new Object();if (eventName){
 this.event.eventName=eventName;} else{this.event.eventName="";} if (typeof this.status !="undefined"){
 this.event.status=this.status;}}
function JSEventModel_raiseEvent(name){if (this.eventHandlers[name]){for (var i=0;
 i < this.eventHandlers[name].length;i++){this.invokeEventHandler(this.eventHandlers[name][i]);
 } } return this.invokeEventHandler(this[name]);}
function JSEventModel_addEventHandler(name,handler){if (!this[name]){this[name]=function(){
 };} if (0==this.eventHandlers.length){this.eventHandlers=new Array();} if (typeof this.eventHandlers[name] !="object" || typeof this.eventHandlers[name].length !="number"){
 var objArray=new Array();this.eventHandlers[this.eventHandlers.length]=objArray;
 this.eventHandlers[name]=objArray;} this.eventHandlers[name][this.eventHandlers[name].length]=handler;
}
function JSEventModel_invokeEventHandler(handler){if (handler){this.vEventHandler=handler;
 switch (typeof this.vEventHandler){case"function": return this.vEventHandler();break;
 case"object": if (this.vEventHandler["raiseEvent"]){return this.vEventHandler.raiseEvent(name);
 } break;case"string": this.vEventHandler=new Function(this.vEventHandler);return this.vEventHandler();
 break;case"undefined": break;default: alert("Invalid type: " + typeof this.vEventHandler);
 } }}
function cloneObject(object){if ("object"==typeof object){var newObject=null;if (object !=null){
 newObject=new Object();for (var propName in object){if (typeof object[propName]=="object"){
 if (null==object[propName]){newObject[propName]=null;} else{newObject[propName]=cloneObject(object[propName]);
 } } else{newObject[propName]=object[propName];} } } return newObject;} else{return object;
 }}
function reconstructObject(sReconstructor){var newObject=new Object();extendObject(newObject,new Function(sReconstructor));
 return newObject;}
function extendObject(object,fnBase){object.overrideMethod=Inheritance_overrideMethod;
 object.extendObjectBase=fnBase;if (arguments.length <=2){object.extendObjectBase();
 } else{var strConstructorCall='object.extendObjectBase(arguments[2]';for (var i=3;
 i < arguments.length;i++){strConstructorCall +=', arguments[' + i +']';} strConstructorCall +=')';
 eval(strConstructorCall);} object.extendObjectBase=null;}
function Inheritance_overrideMethod(strMethodName,fnNewMethod,strBaseName){if (!strBaseName){
 strBaseName="super";} strBaseName +="_" + strMethodName;this[strBaseName]=this[strMethodName];
 this[strMethodName]=fnNewMethod;}
function notifyObject(objNotify,fnNotify,value){var vResult=value;if ("function"==typeof fnNotify){
 if ("object"==typeof objNotify && objNotify){objNotify.notify=fnNotify;vResult=objNotify.notify(value);
 } else{vResult=fnNotify(value);} } return vResult;}
function ClassModel(){this.properties=[];this.defineProperty=ClassModel_defineProperty;
 this.subclassEvent=ClassModel_subclassEvent;this.reconstructor=ClassModel_reconstructor;
 extendObject(this,JSEventModel);}
function ClassModel_defineProperty(name,value){if (0==this.properties.length){this.properties=new Array();
 } this.properties[this.properties.length]=name;this[name]=value;}
function ClassModel_reconstructor(){var sReconstructor="extendObject(this, ClassModel);\n";
 var name="";var value="";for (var i=0;i < this.properties.length;i++){name=this.properties[i];
 value=toLiteral(this[name]);if (value !="undefined"){sReconstructor +="this.defineProperty(\"" + name +"\", " + value +");\n";
 } } return sReconstructor;}
var g_ClassModel_eventCount=0;function ClassModel_subclassEvent(fnOldEvent,sNewEvent){
 if (typeof sNewEvent !="string" || !sNewEvent["length"]){return fnOldEvent;} g_ClassModel_eventCount++;
 var objHook=new ClassModel();var sNewEventName="fnNewEvent" + g_ClassModel_eventCount;
 objHook.defineProperty(sNewEventName,new Function(sNewEvent));if (fnOldEvent){var sOldEventName="fnOldEvent" + g_ClassModel_eventCount;
 objHook.defineProperty(sOldEventName,fnOldEvent);} var sCode="";sCode +="extendObject(this, new Function(unescape('" + escape(objHook.reconstructor()) +"')));\n";
 sCode +="var bResult = this." + sNewEventName +"();\n";if (fnOldEvent){sCode +="if (bResult || 'undefined' == typeof bResult) bResult = this." + sOldEventName +"();\n";
 } sCode +="return bResult;\n";return new Function(sCode);}
function toIdentifier(name){var sId=name +"";sId=sId.replace(/\W/g,"_");sId=sId.replace(/(^\d)/,"id$1");
 return sId;}
function toLiteral(object){var sLiteral="";switch (typeof object){case"undefined": sLiteral="undefined";
 break;case"string": sLiteral='"' + object.replace(/\"/g,'\\\"') +'"';break;case"object": if (null==object){
 sLiteral="null";} else if ("undefined"==typeof object.length){for (var propName in object){
 if (sLiteral.length > 0){sLiteral +=", ";} sLiteral +="'" + propName +"':" + toLiteral(object[propName]);
 } if (sLiteral.length > 0){sLiteral="{" + sLiteral +"}";} else{sLiteral="new Object()";
 } } else if ("function"==typeof object.sort){for (var i=0;i < object.length;i++){
 if (sLiteral.length > 0){sLiteral +=", ";} sLiteral +=toLiteral(object[i]);} if (sLiteral.length > 0){
 sLiteral="[" + sLiteral +"]";} else{sLiteral="new Array()";} } else{sLiteral=object.toString();
 } break;default: sLiteral=object.toString();} return sLiteral;}
function toFunction(object){var fn;switch (typeof object){case"function": fn=object;
 break;case"undefined": case"string": case"object": if (object){eval("fn = " + object.toString());
 } else{fn=new Function();} break;default: fn=new Function("return " + toLiteral(object));
 } return fn;}
function PlatformInfo(){var ua=window.navigator.userAgent.toLowerCase();this.isWindows=(ua.indexOf("win") > -1);
 this.isWinXPSP2=(ua.indexOf("SV1") > -1);this.isWinVista=(ua.indexOf("windows nt 6.") > -1);
 this.isMac=(ua.indexOf("mac") > -1);this.isSun=(ua.indexOf("sunos") > -1);this.isUnix=( this.isSun || (ua.indexOf("x11") > -1) || (ua.indexOf("irix") > -1) || (ua.indexOf("hp-ux") > -1) || (ua.indexOf("sco") > -1) || (ua.indexOf("unix_sv") > -1) || (ua.indexOf("unix_system_v") > -1) || (ua.indexOf("ncr") > -1) || (ua.indexOf("reliantunix") > -1) || (ua.indexOf("dec") > -1) || (ua.indexOf("osf1") > -1) || (ua.indexOf("dec_alpha") > -1) || (ua.indexOf("alphaserver") > -1) || (ua.indexOf("ultrix") > -1) || (ua.indexOf("alphastation") > -1) || (ua.indexOf("sinix") > -1) || (ua.indexOf("aix") > -1) || (ua.indexOf("inux") > -1) || (ua.indexOf("bsd") > -1) || (ua.indexOf("freebsd") > -1));
 var pOpera=ua.indexOf("opera");this.isOpera=(pOpera > -1);this.isSafari=(ua.indexOf("safari") > -1);
 this.isNetscape=((window.navigator.appName=="Netscape") && !this.isOpera);this.isFirefox=((ua.indexOf("firefox/1.") !=-1) && (!this.isOpera));
 this.isNetscape60=false;var pIE=ua.indexOf("msie ");this.isIE=((pIE > -1) && !this.isOpera);
 if (this.isFirefox){this.isNetscape=true;this.isOpera=false;this.isNetscape60=false;
 } if (this.isOpera){this.browserVersion=parseFloat(ua.substring(pOpera + 6));} else if (this.isIE){
 this.browserVersion=parseFloat(ua.substring(pIE + 5));} else if (this.isNetscape){
 var pNetscape=ua.indexOf("netscape/");if (pNetscape > -1){this.browserVersion=parseFloat(ua.substring(pNetscape + 9));
 } else{var pNetscape6=ua.indexOf("netscape6");if (pNetscape6 > -1){this.browserVersion=parseFloat(ua.substring(pNetscape6 + 10));
 this.isNetscape60=(this.browserVersion >=6.0 && this.browserVersion < 6.1);} else{
 this.browserVersion=parseFloat(window.navigator.appVersion);if (this.browserVersion >=5.0){
 var pMozilla=ua.indexOf("rv:");if (pMozilla > -1){if (ua.indexOf("rv:0.9.4") > -1){
 this.browserVersion=6.2;} else{var nRVversion=parseFloat(ua.substring(pMozilla + 3));
 if (nRVversion >=1.0 && nRVversion < 1.4){this.browserVersion=7.0;} else if (nRVversion >=1.4 && nRVversion < 1.5){
 this.browserVersion=7.1;} else if (nRVversion >=1.7 && nRVversion < 1.8){this.browserVersion=7.2;
 } else if (nRVversion >=1.8){this.browserVersion=7.23;} else{this.isNetscape=false;
 } } } else{this.isNetscape=false;} } } } } else{this.browserVersion=parseFloat(window.navigator.appVersion);
 }}
function WebImageFXUtil(){this.trim=WebImageFXUtil_trim;this.lTrim=WebImageFXUtil_lTrim;
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
var WebImageFXUtil=new WebImageFXUtil;function PluginElement(strTagName){this.tagName=strTagName.toLowerCase();
 this.isEmpty=("embed"==this.tagName);this.width=0;this.height=0;this.parameters=[];
 this.events=[];if (this.isEmpty){this.defineParam=PluginElement_Attr_defineParam;
 } else{this.defineParam=PluginElement_Param_defineParam;} this.defineEvent=PluginElement_defineEvent;
 this.copyHTMLAttributes=PluginElement_copyHTMLAttributes;this.HTMLEncode=WebImageFXUtil_HTMLEncode;
 this.createHTML=PluginElement_createHTML;this.createHTMLAttributes=PluginElement_createHTMLAttributes;
 this.createParametersHTML=PluginElement_createParametersHTML;this.createEventsHTML=PluginElement_createEventsHTML;
 this.defineDocParams=PluginElement_defineDocParams;this.defineNamedParams=PluginElement_defineNamedParams;
 this.defineNamedEvents=PluginElement_defineNamedEvents;}
function PluginElement_copyHTMLAttributes(objSource){this.id=objSource.id;this.name=objSource.name;
 this.width=objSource.width;this.height=objSource.height;}
function PluginElement_Param_defineParam(name,value){if (0==this.parameters.length){
 this.parameters=new Array();} var strValue=value +"";if (strValue.indexOf('"') >=0){
 strValue=this.HTMLEncode(strValue);} this.parameters[this.parameters.length]='\n<param name="' + name +'" value="' + strValue +'">';
}
function PluginElement_Attr_defineParam(name,value){if (0==this.parameters.length){
 this.parameters=new Array();} var strValue=value +"";if (strValue.indexOf('"') >=0){
 strValue=this.HTMLEncode(strValue);} this.parameters[this.parameters.length]='\n' + name +'="' + strValue +'"';
}
function PluginElement_defineEvent(name,value){var errCode=0;if (0==this.events.length){
 this.events=new Array();} if (this["name"]){this.events[this.events.length]='\n<scr' +'ipt language="JavaScript1.2" type="text/javascript"><' +'!--' +'\nfunction ' + toIdentifier(this.name) +'_' + name +'\n{\n' + value +'\n}\n// --' +'><' +'/sc' +'ript>\n';
 } return errCode;}
function PluginElement_createHTML(){var strHTML="";if ("function"==typeof this.createBeforeHTML){
 strHTML +=this.createBeforeHTML();} strHTML +='\n<' + this.tagName +' ';strHTML +=this.createHTMLAttributes();
 if (this.isEmpty || this.defineParam==PluginElement_Attr_defineParam){strHTML +=this.createParametersHTML();
 strHTML +='>';} else{strHTML +='>';strHTML +=this.createParametersHTML();strHTML +='\n</' + this.tagName +'>';
 } if ("function"==typeof this.createAfterHTML){strHTML +=this.createAfterHTML();
 } return strHTML;}
function PluginElement_createHTMLAttributes(){var strHTML="";if (this["id"]){strHTML +='\nid="' + this.id +'"';
 strHTML +='\nname="' + this.id +'"';} else if (this["name"]){strHTML +='\nid="' + this.name +'"';
 strHTML +='\nname="' + this.name +'"';} if ("object"==this.tagName){if (this["classid"]){
 strHTML +='\nclassid="CLSID:' + this.classid +'"';} if (this["codebase"]){strHTML +='\ncodebase="' + this.codebase +'"';
 } } if ("embed"==this.tagName){if (this["mimetype"]){strHTML +='\ntype="' + this.mimetype +'"';
 } if (this["pluginspage"]){strHTML +='\npluginspage="' + this.pluginspage +'"';}
 } if (!this.width || this.width=="0"){this.width=500;} strHTML +='\nwidth="' + this.width +'"';
 if (!this.height || this.height=="0"){this.height=300;} strHTML +='\nheight="' + this.height +'"';
 if (this[this.tagName +"Attributes"]){strHTML +='\n' + this[this.tagName +"Attributes"];
 } return strHTML;}
function PluginElement_createParametersHTML(){var strHTML="";for (var i=0;i < this.parameters.length;
 i++){strHTML +=this.parameters[i];} return strHTML;}
function PluginElement_createEventsHTML(){var strHTML="";for (var i=0;i < this.events.length;
 i++){strHTML +=this.events[i];} return strHTML;}
function PluginElement_defineDocParams(parameters){if (!parameters["charset"]){var objDoc=findDocument(parameters.getEditorDocument(),parameters.editorWindow);
 if (objDoc && objDoc.charset){this.defineParam("charset",objDoc.charset);} else{
 this.defineParam("charset","iso-8859-1");} }}
function PluginElement_defineNamedParams(parameters){var sParamName;var vParamValue;
 for (var i=0;i < parameters.names.length;i++){sParamName=parameters.names[i];vParamValue=parameters[sParamName];
 if (vParamValue){this.defineParam(sParamName,vParamValue);} }}
function PluginElement_defineNamedEvents(parameters){var sEventName;var sEventApi;
 var vEventValue;for (var i=0;i < parameters.events.length;i++){sEventName=parameters.events[i].name;
 sEventApi=sEventName +"(" + parameters.events[i].args +")";vEventValue=parameters[sEventName];
 if (typeof vEventValue=="string"){if (vEventValue.length > 0){var sEventType=sEventName.toLowerCase();
 if ("on"==sEventType.substring(0,2)){sEventType=sEventType.substring(2);} var strEvtObj="WebImageFX.event = {type:'" + sEventType +"', srcName:'" + this.name +"'};";
 var strEventHandler=strEvtObj + vEventValue;var errCode=this.defineEvent(sEventApi,strEventHandler);
 var strMsg="";if (2==errCode){strMsg="Double quotes are not allowed in an event handler. Please use single quotes.";
 } else{strMsg="Unknown error in event handler. Error code: " + errCode;} if (errCode !=0){
 strMsg +="\n\nEditor name: " + this.name;strMsg +="\nEvent name: " + sEventName;
 strMsg +="\nEvent handler: " + vEventValue;alert(strMsg);} } } else if (typeof vEventValue !="undefined"){
 alert("Event '" + sEventName +"' must be a string.");} }}
function WebImageFXParameters(){var i;this.defineProperty("path",WebImageFXDefaults.path);
 this.defineProperty("maxContentSize",WebImageFXDefaults.maxContentSize);this.defineProperty("editorGetMethod",WebImageFXDefaults.editorGetMethod);
 this.defineProperty("names",new Array());i=0;this.names[i++]="srcPath";this.names[i++]="license";
 this.names[i++]="locale";this.names[i++]="config";var name;for (i=0;i < this.names.length;
 i++){name=this.names[i];if (typeof WebImageFXDefaults[name] !="undefined"){this.defineProperty(name,WebImageFXDefaults[name]);
 } } this.defineProperty("events",new Array());i=0;this.events[i++]={name:"ondblclickelement",args:"oelement"}
;this.events[i++]={name:"onexeccommand",args:"strcmdname,strtextdata,ldata"};this.events[i++]={
name:"onfocus",args:""};this.events[i++]={name:"onblur",args:""};this.events[i++]={
name:"EditComplete",args:"strloadname,strsavename"};this.events[i++]={name:"EditCommandComplete",args:"strcmdname"}
;this.events[i++]={name:"EditCommandStart",args:"strcmdname"};this.events[i++]={name:"ImageError",args:"strerrorid, strerrdesc, strimagename, strcmdname"}
;this.events[i++]={name:"LoadingImage",args:"strimagename, strsavefilename, stroldimagename, strsavename"}
;this.events[i++]={name:"SavingImage",args:"strimagename, strsavefilename"};this.events[i++]={
name:"UpdateImage",args:"strimagename, strsavefilename"};this.events[i++]={name:"LicenseValidity",args:"strisvalid, strlicense"}
;for (i=0;i < this.events.length;i++){name=this.events[i].name;if (typeof WebImageFXDefaults[name] !="undefined"){
 this[name]=WebImageFXDefaults[name];} } if ("function"==typeof WebImageFXButtonTag){
 this.buttonTag=new WebImageFXButtonTag();} if ("function"==typeof WebImageFXPopup){
 this.popup=new WebImageFXPopup();} if ("function"==typeof installPopup){this.installPopup=new installPopup();
 } this.getOptionalParameter=WebImageFXParameters_getOptionalParameter;this.getEditorDocument=WebImageFXParameters_getEditorDocument;
 this.relocate=WebImageFXParameters_relocate;this.reset=WebImageFXParameters_reset;
 this.definePropertyName=WebImageFXParameters_definePropertyName;this.isLicense=WebImageFXParameters_isLicense;
}
WebImageFXParameters.prototype=new ClassModel;function WebImageFXParameters_reset(){
 var i;var name;for (i=0;i < this.properties.length;i++){name=this.properties[i];
 if (typeof this[name] !="object" && typeof WebImageFXDefaults[name] !="undefined"){
 this[name]=WebImageFXDefaults[name];} } for (i=0;i < this.names.length;i++){name=this.names[i];
 if (typeof WebImageFXDefaults[name] !="undefined"){this[name]=WebImageFXDefaults[name];
 } } for (i=0;i < this.events.length;i++){name=this.events[i].name;if (typeof WebImageFXDefaults[name] !="undefined"){
 this[name]=WebImageFXDefaults[name];} }}
function WebImageFXParameters_getOptionalParameter(name,defaultValue){if (typeof this[name] !="undefined"){
 return this[name];} else if (typeof WebImageFXDefaults[name] !="undefined"){return WebImageFXDefaults[name];
 } else{return defaultValue;}}
function WebImageFXParameters_definePropertyName(name){var bDefine=true;if ("undefined"==typeof this[name]){
 if ("undefined"==typeof WebImageFXDefaults[name]){bDefine=false;} else{this[name]=WebImageFXDefaults[name];
 } } else{for (var i=0;i < this.properties.length;i++){if (this.properties[i]==name){
 bDefine=false;} } if (bDefine){this.defineProperty(name,this[name]);} } return bDefine;
}
function WebImageFXParameters_relocate(evalWindow){var prevWindow=this.WebImageFXWindow;
 this.WebImageFXWindow=evalWindow;for (var i=0;i < this.events.length;i++){var vHandler=this[this.events[i].name];
 if ("string"==typeof vHandler && vHandler.length > 0){if (prevWindow){var re=new RegExp("^" + prevWindow +"\.");
 vHandler=vHandler.replace(re,"");} if (this.WebImageFXWindow){this[this.events[i].name]=this.WebImageFXWindow +"." + vHandler;
 } } }}
function WebImageFXParameters_getEditorDocument(){var evalDocument="document";if (this.editorDocument){
 evalDocument=this.editorDocument;} else if (this.editorDocumentLayer){evalDocument=this.editorDocumentLayer;
 } return evalDocument;}
function WebImageFXParameters_isLicense(strModifier,strLicense){if ("string"==typeof strLicense){
 var re=new RegExp("\(([^\)]+\-)?" + strModifier +"(\-[^\(]+)?\)");return (strLicense.search(re) >=0);
 } else{return false;}}
function conditional_write(strHTML,objHtml,parameters){if (objHtml){objHtml.html +=strHTML;
 } if (!parameters.writeDisabled){var evalWindowDocument="";if (parameters.editorWindow){
 evalWindowDocument=parameters.editorWindow +".";} evalWindowDocument +=parameters.getEditorDocument();
 eval(evalWindowDocument +'.write(strHTML)');}}
function findWindow(evalWindow){var objWin=window;if (typeof evalWindow=="string" && evalWindow.length > 0){
 objWin=eval(evalWindow);} return objWin;}
function findDocument(evalDocument,evalWindow){var objDoc=null;if (!evalDocument){
 evalDocument="document";} var objWin=findWindow(evalWindow);if (objWin){if ((("unknown"==typeof objWin.closed) || !objWin.closed) && ("object"==typeof objWin["document"])){
 objDoc=eval('objWin.' + evalDocument);} else{objDoc=null;} } else{objDoc=eval(evalDocument);
 } return objDoc;}
function findElementDocumentLayer(name,evalWindow){var objInfo=findFormAndElement(name,evalWindow);
 return objInfo.evalDocument;}
function findElementForm(name,evalWindow){var objInfo=findFormAndElement(name,evalWindow);
 return objInfo.objForm;}
function findElement(name,evalWindow){var objInfo=findFormAndElement(name,evalWindow);
 return objInfo.objElem;}
function findElementFormName(name,evalWindow){var objInfo=findFormAndElement(name,evalWindow);
 return objInfo.formName;}
function findFormAndElementName(name,evalWindow){var aryNames=new Array(3);var objInfo=findFormAndElement(name,evalWindow);
 aryNames[0]=objInfo.formName;aryNames[1]=objInfo.elemName;aryNames[2]=objInfo.evalDocument;
 return aryNames;}
function findFormAndElement(name,evalWindow){var objInfo={evalDocument:"document",formName:"",objForm:null,elemName:"",objElem:null}
;var aryNames=splitFormAndElementName(name);objInfo.formName=aryNames[0];objInfo.elemName=aryNames[1];
 var objDoc=findDocument(objInfo.evalDocument,evalWindow);findFormAndElementInfo(objDoc,objInfo);
 return objInfo;}
function findFormAndElementInfo(objDoc,objInfo){var bFound=false;var formName=objInfo.formName;
 var elemName=objInfo.elemName;var foundForm=null;var foundElem=null;if (objDoc && objDoc.forms && elemName){
 var aForm;var anElem;if (formName){aForm=findFormByName(objDoc,formName);if (aForm){
 anElem=findElementByName(aForm,elemName);if (anElem){foundForm=aForm;foundElem=anElem;
 bFound=true;} } if (!foundForm){elemName=formName +"." + elemName;formName="";} }
 if (!foundForm){var aryFoundFormNames=new Array();for (var i=0;i < objDoc.forms.length;
 i++){aForm=objDoc.forms[i];anElem=findElementByName(aForm,elemName);if (anElem){
 var formName=aForm.name;if (!formName){formName=i +"";} if (!foundForm){foundForm=aForm;
 } if (!foundElem){foundElem=anElem;} aryFoundFormNames[aryFoundFormNames.length]=formName;
 bFound=true;} } if (aryFoundFormNames.length > 1){var sMsg="Ambiguous element name: " + elemName +"\n" +"Please specify a form name: " + aryFoundFormNames.toString() +"\n" +"Example: " + formName +"." + elemName +"\n";
 alert(sMsg);} } } if (bFound){objInfo.formName=formName;objInfo.elemName=elemName;
 objInfo.objForm=foundForm;objInfo.objElem=foundElem;} else if (objDoc && objDoc.layers){
 for (var i=0;i < objDoc.layers.length;i++){bFound=findFormAndElementInfo(objDoc.layers[i].document,objInfo);
 if (bFound){objInfo.evalDocument="document.layers[" + i +"]." + objInfo.evalDocument;
 break;} } } return bFound;}
function findFormByName(objDoc,formName){for (var i=0;i < objDoc.forms.length;i++){
 if (objDoc.forms[i].name==formName){return objDoc.forms[i];} } return objDoc.forms[formName];
}
function findElementByName(objForm,elemName){for (var i=0;i < objForm.elements.length;
 i++){if (objForm.elements[i].name==elemName){return objForm.elements[i];} } return objForm.elements[elemName];
}
function splitFormAndElementName(name){var aryNames=new Array(2);var i=name.indexOf(".");
 if (-1==i){aryNames[0]="";aryNames[1]=name;} else{aryNames[0]=name.substring(0,i);
 aryNames[1]=name.substring(i + 1);} return aryNames;}
var g_WebImageFXCookie_Count=0;function WebImageFXCookie(name,evalDocument,evalWindow){
 if (name){this.name=name;} else{this.name="cookie" + g_WebImageFXCookie_Count++;
 } this.evalDocument=evalDocument;this.evalWindow=evalWindow;this.expiresInSeconds=3 * 365 * 24 * 60 * 60;
 this.setCookie=WebImageFXCookie_setCookie;this.getCookie=WebImageFXCookie_getCookie;
 this.removeCookie=WebImageFXCookie_removeCookie;}
function WebImageFXCookie_setCookie(args){var expDateDefault=new Date();expDateDefault.setTime(expDateDefault.getTime() + this.expiresInSeconds * 1000);
 var argv=this.setCookie.arguments;var argc=this.setCookie.arguments.length;var name="";
 var value="";if (argc==1){name=this.name;value=argv[0];} else if (argc==2){name=argv[0];
 value=argv[1];} var expires=(argc > 2) ? argv[2] : expDateDefault;var path=(argc > 3) ? argv[3] : null;
 var domain=(argc > 4) ? argv[4] : null;var secure=(argc > 5) ? argv[5] : false;if ("object"==typeof expires){
 expires=expires.toGMTString();} var objDoc=findDocument(this.evalDocument,this.evalWindow);
 objDoc.cookie=name +"=" + escape(value) + (expires ?"; expires=" + expires :"") + (path ?"; path=" + path :"") + (domain ?"; domain=" + domain :"") + (secure ?"; secure" :"");
}
function WebImageFXCookie_getCookie(name){if (!name){name=this.name;} var objDoc=findDocument(this.evalDocument,this.evalWindow);
 var strCookie=objDoc.cookie +"";var aryPairs=strCookie.split(";");var pair=[];for (var i=0;
 i < aryPairs.length;i++){pair=aryPairs[i].split("=");if (pair.length==2){var key;
 key=pair[0] +"";key=WebImageFXUtil_trim(key);if (key==name){return unescape(pair[1]);
 } } } return;}
function WebImageFXCookie_removeCookie(name){if (!name){name=this.name;} var expDate=new Date();
 expDate.setTime(expDate.getTime() - 1);var objDoc=findDocument(this.evalDocument,this.evalWindow);
 objDoc.cookie=name +"=; expires=" + expDate.toGMTString();}
EWEP_STATUS_INSTALLED="installed";EWEP_STATUS_NOTLOADED="notloaded";EWEP_STATUS_LOADING="loading";
EWEP_STATUS_LOADED="loaded";EWEP_STATUS_SAVING="saving";EWEP_STATUS_SAVED="saved";
EWEP_STATUS_NOTSUPPORTED="notsupported";EWEP_STATUS_NOTINSTALLED="notinstalled";EWEP_STATUS_FATALERROR="fatalerror";
EWEP_STATUS_UNABLETOSAVE="unabletosave";EWEP_STATUS_SIZEEXCEEDED="sizeexceeded";EWEP_STATUS_MISSINGNOTIFY="missingnotify";
EWEP_STATUS_CANCELED="canceled";g_wifxInstanceTypes=new Array();function WebImageFXAddInstanceType(fnInstanceType){
 if ("function"==typeof fnInstanceType){g_wifxInstanceTypes[g_wifxInstanceTypes.length]=fnInstanceType;
 if ("object"==typeof WebImageFX){WebImageFX.addInstanceType(fnInstanceType);} } else{
 alert("WebImageFXAddInstanceType argument must be a function.\nIt is " + typeof fnInstanceType);
 }}
function WebImageFXInstanceType(){this.description="";this.type="unknown";this.parameterPropertyNames=[];
 this.refreshStatus=WebImageFXInstanceType_refreshStatus;this.valid=WebImageFXInstanceType_valid;
 this.currentPreference=WebImageFXInstanceType_currentPreference;this.create=WebImageFXInstanceType_create;
 this.clear=WebImageFXInstanceType_clear;this.fixDimension=WebImageFXInstanceType_fixDimension;
}
function WebImageFXInstanceType_refreshStatus(parameters){this.isSupported=false;
 this.isAutoInstallSupported=false;this.isInstalled=false;this.upgradeNeeded=false;
 }
function WebImageFXInstanceType_valid(){return (this.isSupported && (this.isInstalled || this.isAutoInstallSupported));
}
function WebImageFXInstanceType_currentPreference(width,height,parameters){return 0;
}
function WebImageFXInstanceType_create(name,width,height,parameters){return null;
}
function WebImageFXInstanceType_clear(){}
function WebImageFXInstanceType_fixDimension(dimension){var newDimension=dimension;
 if ("string"==typeof dimension){if (dimension.indexOf("%") >=0){newDimension=600;
 } } return newDimension;}
function WebImageFXInstance(name,width,height,parameters){if (typeof parameters !="object" || (null==parameters)){
 parameters=WebImageFX.parameters;} extendObject(this,JSEventModel);this.id=name;
 this.name="" + name;this.width=width;this.height=height;this.editor=null;this.asyncActive=false;
 this.receivedEvent=false;extendObject(this,AsyncMethods);this.editorDocument=parameters.getEditorDocument();
 this.editorWindow=parameters.editorWindow;this.maxContentSize=parameters.maxContentSize;
 this.isSizeExceeded=WebImageFXInstance_isSizeExceeded;this.linkTo=WebImageFXInstance_linkTo;
 this.linkedElement=WebImageFXInstance_linkedElement;this.linkedWindow=WebImageFXInstance_linkedWindow;
 this.linkedInstance=WebImageFXInstance_linkedInstance;this.linkedPopup=WebImageFXInstance_linkedPopup;
 this.isInstance=WebImageFXInstance_isInstance;this.writeHTML=WebImageFXInstance_writeHTML;
 if (typeof cSETMETHOD !="undefined"){this.editorSetMethod=cSETMETHOD;} if (!parameters.editorGetMethod){
 if (typeof cGETMETHOD !="undefined"){this.editorGetMethod=cGETMETHOD;} } else{this.editorGetMethod=parameters.editorGetMethod;
 } this.writeValue=WebImageFXInstance_writeValue;this.readValue=WebImageFXInstance_readValue;
 this.estimateContentSize=WebImageFXInstance_estimateContentSize;this.raiseSizeExceededError=WebImageFXInstance_raiseSizeExceededError;
 this.clear=WebImageFXInstance_clear;this.focus=WebImageFXInstance_focus;this.getReadOnly=WebImageFXInstance_getReadOnly;
 this.setReadOnly=WebImageFXInstance_setReadOnly;this.isChanged=WebImageFXInstance_isChanged;
 this.insertMediaFile=WebImageFXInstance_insertMediaFile;this.insertMediaFileDeferred=WebImageFXInstance_insertMediaFileDeferred;
 this.status=EWEP_STATUS_NOTLOADED;this.html="";if (parameters.linkToName && parameters.linkToWindow){
 this.linkTo(parameters.linkToName,parameters.linkToWindow);} else{this.linkTo(this.name,this.editorWindow);
 }}
 function WebImageFXInstance_writeHTML(strHTML,parameters){if (strHTML.length > 0){
 if (parameters.showPluginElement || parameters.showActiveXElement){strHTML='<textarea rows="10" cols="60">' + strHTML +'</textarea><br>' + strHTML;
 } conditional_write(strHTML,this,parameters);}}
function WebImageFXInstance_getReadOnly(){if (this.isReady()){if ("string"==typeof cREADONLY){
 return this.editor.getPropertyBoolean(cREADONLY);} else{return false;} } else{return false;
 }}
function WebImageFXInstance_setReadOnly(bValue){if (this.isReady()){if ("string"==typeof cREADONLY){
 this.editor.setProperty(cREADONLY,bValue);} }}
function WebImageFXInstance_isChanged(){if (this.isReady()){if ("string"==typeof cCHANGEDMETHOD){
 return eval('this.editor.' + cCHANGEDMETHOD);} else{return true;} } else{return false;
 }}
function WebImageFXInstance_writeValue(strValue,fnNotify){eval('this.editor.' + this.editorSetMethod +'(strValue)');
 notifyObject(this,fnNotify);}
function WebImageFXInstance_readValue(fnNotify){var vResult=eval('this.editor.' + this.editorGetMethod +'()');
 notifyObject(this,fnNotify,vResult);}
function WebImageFXInstance_estimateContentSize(editorEstimateContentSize){var nContentSize=0;
 if ("string"==typeof editorEstimateContentSize && editorEstimateContentSize){nContentSize=this.editor.EstimateContentSize(editorEstimateContentSize);
 } return nContentSize;}
function WebImageFXInstance_raiseSizeExceededError(nContentSize){this.status=EWEP_STATUS_SIZEEXCEEDED;
 this.initEvent("onerror");this.event.source="save";this.event.contentSize=nContentSize;
 this.event.maxContentSize=this.maxContentSize;if (this.raiseEvent("onerror") !=false){
 if (WebImageFXMessages.sizeExceeded){alert(WebImageFXMessages.sizeExceeded);} }}
function WebImageFXInstance_clear(){this.editor=null;this.receivedEvent=false;this.status=EWEP_STATUS_NOTLOADED;
}
function WebImageFXInstance_focus(){if (this.isReady()){this.editor.focus();}}
function WebImageFXInstance_isSizeExceeded(size){var bIsSizeExceeded=false;if (this.maxContentSize > 0){
 bIsSizeExceeded=(size > this.maxContentSize);} return bIsSizeExceeded;}
function WebImageFXInstance_linkTo(name,evalWindow){this.linkName=name;this.formName="";
 this.elemName="";this.evalDocument="document";this.evalWindow=evalWindow;}
function WebImageFXInstance_linkedElement(){var objElem=null;if (this.linkName && !this.elemName && !this.formName){
 var aryNames=findFormAndElementName(this.linkName,this.evalWindow);this.formName=aryNames[0];
 this.elemName=aryNames[1];this.evalDocument=aryNames[2];} if (this.formName && this.elemName){
 var objDoc=findDocument(this.evalDocument,this.evalWindow);if (objDoc){var objForm=findFormByName(objDoc,this.formName);
 if (objForm){objElem=findElementByName(objForm,this.elemName);} } } return objElem;
}
function WebImageFXInstance_linkedWindow(){return findWindow(this.evalWindow);}
function WebImageFXInstance_linkedInstance(){var objInstance=null;var objWin=this.linkedWindow();
 if (objWin && objWin.WebImageFX){objInstance=objWin.WebImageFX.instances[this.linkName];
 } return objInstance;}
function WebImageFXInstance_linkedPopup(){var objPopup=null;var objWin=this.linkedWindow();
 if (objWin && objWin.WebImageFX){objPopup=objWin.WebImageFX.popups[this.linkName];
 } return objPopup;}
function WebImageFXInstance_isInstance(objField){if (!objField) return false;var strFieldName=objField.name;
 return (this.name==strFieldName || this.id==strFieldName);}
function WebImageFXInstance_insertMediaFile(strSrcFileLocation,bLocalFile,strFileTitle,strFileType,nWidth,nHeight){
 setTimeout('WebImageFX.instances["' + this.name +'"].insertMediaFileDeferred(' + toLiteral(strSrcFileLocation) +', ' + bLocalFile +', ' + toLiteral(strFileTitle) +', ' + toLiteral(strFileType) +', ' + nWidth +', ' + nHeight +')',1);
}
function WebImageFXInstance_insertMediaFileDeferred(strSrcFileLocation,bLocalFile,strFileTitle,strFileType,nWidth,nHeight){
 var objMedia=this.editor.MediaFile();objMedia.setProperty("IsLocal",bLocalFile);
 objMedia.setProperty("SrcFileLocationName",strSrcFileLocation);objMedia.setProperty("FileTitle",strFileTitle);
 objMedia.setProperty("FileType","IMAGE");objMedia.setProperty("ImageWidth",nWidth);
 objMedia.setProperty("ImageHeight",nHeight);this.editor.ExecCommand("cmdmfuinsert",strSrcFileLocation,bLocalFile);
}
function AsyncMethods(){this.asyncCallMethod=AsyncMethods_asyncCallMethod;this.asyncSetProperty=AsyncMethods_asyncSetProperty;
 this.asyncGetProperty=AsyncMethods_asyncGetProperty;this.asyncGetPropertyString=AsyncMethods_asyncGetPropertyString;
 this.asyncGetPropertyInteger=AsyncMethods_asyncGetPropertyInteger;this.asyncGetPropertyBoolean=AsyncMethods_asyncGetPropertyBoolean;
 this.validAsyncArguments=AsyncMethods_validAsyncArguments;this.localCall=AsyncMethods_localCall;
 }
function AsyncMethods_asyncCallMethod(methodName,argv,objNotify,fnNotify){return this.localCall(methodName,argv,objNotify,fnNotify);
}
function AsyncMethods_asyncSetProperty(propertyName,value,objNotify,fnNotify){var methodName="setProperty";
 var argv=[propertyName,value];return this.localCall(methodName,argv,objNotify,fnNotify);
}
function AsyncMethods_asyncGetProperty(propertyName,objNotify,fnNotify){var methodName="getProperty";
 var argv=[propertyName];return this.localCall(methodName,argv,objNotify,fnNotify);
}
function AsyncMethods_asyncGetPropertyString(propertyName,objNotify,fnNotify){var methodName="getPropertyString";
 var argv=[propertyName];return this.localCall(methodName,argv,objNotify,fnNotify);
}
function AsyncMethods_asyncGetPropertyInteger(propertyName,objNotify,fnNotify){var methodName="getPropertyInteger";
 var argv=[propertyName];return this.localCall(methodName,argv,objNotify,fnNotify);
}
function AsyncMethods_asyncGetPropertyBoolean(propertyName,objNotify,fnNotify){var methodName="getPropertyBoolean";
 var argv=[propertyName];return this.localCall(methodName,argv,objNotify,fnNotify);
}
function AsyncMethods_validAsyncArguments(methodName,argv,objNotify,fnNotify){var strErrorMsg="Error in asynchronous call.\n";
 if (!methodName){alert(strErrorMsg +"The name of the method to call is missing.");
 return false;} if (typeof argv !="object" || typeof argv.length !="number"){alert(strErrorMsg +"The method arguments must be passed as an array.");
 return false;} if (typeof objNotify !="object"){alert(strErrorMsg +"The objNotify argument must be an object or 'null'.");
 return false;} if (typeof fnNotify !="function"){alert(strErrorMsg +"The fnNotify argument must be a function.");
 return false;} return true;}
function AsyncMethods_localCall(methodName,argv,objNotify,fnNotify){if (!this.validAsyncArguments(methodName,argv,objNotify,fnNotify)){
 return false;} var sCode=methodName +"(";for (var i=0;i < argv.length;i++){if (i !=0){
 sCode +=", ";} sCode +=toLiteral(argv[i]);} sCode +=")";var result=eval("this.editor." + sCode);
 notifyObject(objNotify,fnNotify,result);return true;}
function AxEskerElem(){extendObject(this,PluginElement,"embed");this.pluginName="Ektron Plug-in for WebImageFX";
 if (this.browserVersion < 5.0){this.mimetype="application/x-eskerplus";} else{this.mimetype="application/x-eskeractivex";
 } this.defineEvent=AxEskerElem_defineEvent;this.createBeforeHTML=null;this.createAfterHTML=null;
 this.overrideMethod("createHTMLAttributes",AxEskerElem_createHTMLAttributes);this.overrideMethod("defineDocParams",AxEskerElem_defineDocParams);
 this.pluginInstalled=AxEskerElem_pluginInstalled;this.pluginVersionInstalled=AxEskerElem_pluginVersionInstalled;
}
function AxEskerElem_defineEvent(name,value){var errCode=0;if (0==this.events.length){
 this.events=new Array();} var strValue=value +"";if (strValue.indexOf('"') >=0){
 strValue="alert('ERROR: Event handler contains double quotes. Event: " + name +"')";
 errCode=2;} var sValueFn='function() {' + strValue +'} ()';if (this.browserVersion < 6.1){
 this.events[this.events.length]='\n' + name +'="' + sValueFn +'"';} else{this.events[this.events.length]='\nevent' + this.events.length +'="' + name +'=' + sValueFn +'"';
 } return errCode;}
function AxEskerElem_createHTMLAttributes(){var strHTML="";if (this["classid"]){if (this.browserVersion < 5.0){
 strHTML +='\nclassid="CLSID:' + this.classid +'"';} else{strHTML +='\nclsid="' + this.classid +'"';
 } } if (this["codebase"]){if (this["deployer"]){this.codebase=this.deployer;} else{
 this.codebase=this.codebase.replace(/\.cab/i,".ocx");} strHTML +='\ncodebase="' + this.codebase +'"';
 } strHTML +=this.super_createHTMLAttributes();strHTML +=this.createEventsHTML();
 return strHTML;}
function AxEskerElem_defineDocParams(parameters){var objDoc=findDocument(parameters.getEditorDocument(),parameters.editorWindow);
 if (!objDoc){objDoc=document;} this.defineParam("href",objDoc.location.href);this.defineParam("cookie",objDoc.cookie);
 this.super_defineDocParams(parameters);}
function AxEskerElem_pluginInstalled(){var bInstalled=false;if (this.browserVersion >=4.0 && this.browserVersion < 5.0){
 if (window.navigator.plugins["Esker ActiveX Plug-in"] && window.navigator.plugins[this.pluginName]){
 var pluginDesc=window.navigator.plugins["Esker ActiveX Plug-in"].description +"";
 var pluginVersion=parseFloat(pluginDesc.substring(30,40));if (pluginVersion >=4.5){
 bInstalled=true;} } } else if (this.browserVersion >=6.0 && this.browserVersion < 7.0){
 if (window.navigator.plugins["Esker ActiveX Plug-in for Netscape 6"] && window.navigator.plugins[this.pluginName]){
 var pluginDesc=window.navigator.plugins["Esker ActiveX Plug-in for Netscape 6"].description +"";
 var pluginVersion=parseFloat(pluginDesc.substring(30,40));if ((this.browserVersion >=6.0 && this.browserVersion < 6.1) && pluginVersion==6.4){
 bInstalled=true;} else if (this.browserVersion==6.1 && pluginVersion==6.5){bInstalled=true;
 } else if (this.browserVersion >=6.2 && pluginVersion >=6.6){bInstalled=true;} }
 } else if (this.browserVersion >=7.0){if (window.navigator.plugins["Esker ActiveX Plug-in for Netscape 7"] && window.navigator.plugins[this.pluginName]){
 var pluginDesc=window.navigator.plugins["Esker ActiveX Plug-in for Netscape 7"].description +"";
 var pluginVersion=parseFloat(pluginDesc.substring(30,40));if (this.browserVersion < 7.1 && (pluginVersion >=7.0 && pluginVersion < 7.2)){
 bInstalled=true;} else if ((this.browserVersion >=7.1 && this.browserVersion < 7.2) && (pluginVersion >=7.2 && pluginVersion < 7.6)){
 bInstalled=true;} else if (this.browserVersion >=7.2 && pluginVersion >=7.7){bInstalled=true;
 } } } else if (this.browserVersion >=7.23){if (window.navigator.plugins["Esker ActiveX Plug-in for Netscape 7"] && window.navigator.plugins[this.pluginName]){
 var pluginDesc=window.navigator.plugins["Esker ActiveX Plug-in for Netscape 7"].description +"";
 var pluginVersion=parseFloat(pluginDesc.substring(30,40));if (pluginVersion > 7.9){
 bInstalled=true;} } } return bInstalled;}
function AxEskerElem_pluginVersionInstalled(){var versionInstalled="";if (window.navigator.plugins[this.pluginName]){
 versionInstalled=window.navigator.plugins[this.pluginName].description +"";} return versionInstalled;
}
function ActiveXElement(strProgId){this.progid=strProgId;extendObject(this,PlatformInfo);
 this.isSupported=(this.isWindows && ((this.isIE && this.browserVersion >=4.0) || (this.isNetscape && this.browserVersion >=4.0)));
 this.isAutoInstallSupported=false;this.isInstalled=false;if (this.isSupported && this.isNetscape){
 extendObject(this,AxEskerElem);this.isAutoInstallSupported=false;this.isInstalled=this.pluginInstalled();
 this.versionInstalled=this.pluginVersionInstalled();} else if (this.isSupported){
 extendObject(this,AxObjectElem);var bActiveXVersionInstalledAvailable=false;if (this.isIE && this.browserVersion < 5.0){
 this.isAutoInstallSupported=false;if ("function"==typeof isVBScriptSupported){if (isVBScriptSupported()){
 bActiveXVersionInstalledAvailable=true;} } } else{this.isAutoInstallSupported=true;
 bActiveXVersionInstalledAvailable=(typeof ActiveXVersionInstalled !="undefined");
 } if (bActiveXVersionInstalledAvailable && this.progid){this.versionInstalled=ActiveXVersionInstalled(this.progid) +"";
 this.isInstalled=(this.versionInstalled.length > 0);if (this.isInstalled){this.isAutoInstallSupported=true;
 } } else{this.isInstalled=this.isAutoInstallSupported;} } this.compareVersion=ActiveXElement_compareVersion;
 }
function ActiveXElement_compareVersion(strVersion,strVersionInstalled){if (typeof strVersion !="string"){
 return 0;} if (typeof strVersionInstalled !="string"){return 0;} var aryVersion=strVersion.split(",");
 var aryInstalled=strVersionInstalled.split(",");var nCount=Math.min(aryVersion.length,aryInstalled.length);
 var nVersion=0;var nVersionInstalled=0;for (var i=0;i < nCount;i++){nVersion=aryVersion[i] - 0;
 nVersionInstalled=aryInstalled[i] - 0;if (nVersionInstalled < nVersion){return -1;
 } else if (nVersionInstalled > nVersion){return 1;} } return 0;}
function AxObjectElem(){extendObject(this,PluginElement,"object");this.defineEvent=AxObjectElem_defineEvent;
 this.createBeforeHTML=null;this.createAfterHTML=this.createEventsHTML;}
function AxObjectElem_defineEvent(name,value){var errCode=0;if (0==this.events.length){
 this.events=new Array();} var strValue=value +"";if (strValue.indexOf('"') >=0){
 strValue="alert('ERROR: Event handler contains double quotes. Event: " + name +"')";
 errCode=2;} if (this["id"]){this.events[this.events.length]='\n<scr' +'ipt language="JavaScript1.2" type="text/javascript" for="' + this.id +'" event="' + name +'">\n' + strValue +'\n<' +'/sc' +'ript>';
 } return errCode;}
var cPROGID="WebImageFX.ImageEditor";var cCLASSID="CC4E480A-BD2D-4859-94DB-C72F7EE79C1D";
var cVERSION="1,2,0,18";var cSETMETHOD="EditFile";var cGETMETHOD="PublishHtml";var cREADONLY="ReadOnly";
 var cCHANGEDMETHOD="getPropertyBoolean('IsDirty')";var cWIFXLPKOBJECT="<OBJECT CLASSID='clsid:5220cb21-c88d-11cf-b347-00aa00a28331'><PARAM NAME='LPKPath' VALUE='" + WIFXPath +"webimagefx.lpk'></OBJECT>";
var editorEstimateContentSize="whole";function WebImageFXActiveXType(){extendObject(this,WebImageFXInstanceType);
 this.description="ActiveX control";this.type="activex";this.parameterPropertyNames=["objectAttributes","embedAttributes"];
 this.numInstances=0;this.isNetscape60=false;this.refreshStatus=WebImageFXActiveXType_refreshStatus;
 this.overrideMethod("valid",WebImageFXActiveXType_valid);this.currentPreference=WebImageFXActiveXType_currentPreference;
 this.create=WebImageFXActiveXType_create;this.clear=WebImageFXActiveXType_clear;
 this.checkLicense=WebImageFXActiveXType_checkLicense;}
WebImageFXAddInstanceType(WebImageFXActiveXType);function WebImageFXActiveXType_refreshStatus(parameters){
 if (typeof parameters !="object" || (null==parameters)){parameters=WebImageFX.parameters;
 } var objPluginElement=new ActiveXElement(cPROGID);this.isSupported=objPluginElement.isSupported && this.checkLicense(parameters);
 this.isAutoInstallSupported=objPluginElement.isAutoInstallSupported;this.isInstalled=objPluginElement.isInstalled;
 this.versionInstalled=objPluginElement.versionInstalled;this.upgradeNeeded=(objPluginElement.compareVersion(cVERSION,this.versionInstalled) < 0);
 this.isNetscape60=objPluginElement.isNetscape60;objPluginElement=null;}
function WebImageFXActiveXType_checkLicense(parameters){var strLicense=parameters["license"];
 if ("string"==typeof strLicense && strLicense.length > 0){var aryLicenseKeys=strLicense.split(",");
 var numLicenseKeys=aryLicenseKeys.length;var numOtherLicenseKeys=0;for (var i=0;
 i < aryLicenseKeys.length;i++){for (var iType=0;iType < g_wifxInstanceTypes.length;
 iType++){var objInstanceType=new g_wifxInstanceTypes[iType]();var strModifier=objInstanceType.type;
 if (parameters.isLicense(strModifier,aryLicenseKeys[i])){numOtherLicenseKeys++;}
 objInstanceType=null;} } return (numLicenseKeys > numOtherLicenseKeys);} else{var objDoc=findDocument(parameters.getEditorDocument(),parameters.editorWindow);
 if (!objDoc){objDoc=document;} var strHostname=objDoc.location.hostname +"";strHostname=strHostname.toLowerCase();
 return ("localhost"==strHostname ||"127.0.0.1"==strHostname);}}
function WebImageFXActiveXType_valid(){if (this.isNetscape60 && this.numInstances >=1){
 return false;} return (this.super_valid());}
function WebImageFXActiveXType_currentPreference(width,height,parameters){var wd=width * 1;
 var ht=height * 1;if (this.numInstances < 3 && (wd >=500 || isNaN(wd)) && (ht >=150 || isNaN(ht))){
 return 500;} else{return 350;}}
function WebImageFXActiveXType_create(name,width,height,parameters){if (this.isNetscape60){
 width=this.fixDimension(width);height=this.fixDimension(height);} var objInstance=new WebImageFXEditor(name,width,height,parameters);
 this.numInstances++;return objInstance;}
function WebImageFXActiveXType_clear(){this.numInstances=0;}
function writeInitfucntion(){var strHtml="function writeWIFXInit(editorName, sLicense, sConfig, sLocale){";
 strHtml +="var objImageEdit = WebImageFX.instances[editorName].editor;";strHtml +="if (typeof objImageEdit == 'object' && objImageEdit != null)";
 strHtml +="{";strHtml +="objImageEdit.Initialize(sLicense, sConfig, sLocale); ";
 strHtml +="}";strHtml +="}";return strHtml;}
function WebImageFXEditor(name,width,height,parameters){if (typeof parameters !="object" || (null==parameters)){
 parameters=WebImageFX.parameters;} this.type="activex";extendObject(this,WebImageFXInstance,name,width,height,parameters);
 this.createHTML=WebImageFXEditor_createHTML;this.createInit=WebImageFXEditor_createInit;
 this.isReady=WebImageFXEditor_isReady;var strHTML=this.createHTML(parameters);this.writeHTML(strHTML,parameters);
 var strInit=this.createInit(name);this.writeHTML(strInit,parameters);this.isReady();
}
 var s_numInstances=0;function WebImageFXEditor_createHTML(parameters){var sHtml="";
 var objActiveXElement=new ActiveXElement(cPROGID);this.isIE=objActiveXElement.isIE;
 this.isNetscape=objActiveXElement.isNetscape;this.isNetscape60=objActiveXElement.isNetscape60;
 this.browserVersion=objActiveXElement.browserVersion;this.isSupported=objActiveXElement.isSupported;
 this.isInstalled=objActiveXElement.isInstalled;if (this.isSupported){s_numInstances++;
 if (objActiveXElement.isIE && objActiveXElement.browserVersion < 5.0 && s_numInstances==1){
 sHtml +='<object classid="clsid:2D360201-FFF5-11D1-8D03-00A0C959BC0A" id="DHTMLSafe1" height="0" width="0" codebase="' + parameters.path +'dhtmled.cab#version=6,1,0,8243"></object>';
 } if (typeof cWIFXLPKOBJECT=="string" && cWIFXLPKOBJECT.length > 0 && objActiveXElement.isIE && s_numInstances==1){
 sHtml +=cWIFXLPKOBJECT;} if (!objActiveXElement.isInstalled && !objActiveXElement.isAutoInstallSupported){
 this.status=EWEP_STATUS_NOTINSTALLED;} objActiveXElement.copyHTMLAttributes(this);
 objActiveXElement.classid=cCLASSID;if (parameters["embedAttributes"]){objActiveXElement.embedAttributes=parameters.embedAttributes;
 } if (parameters["objectAttributes"]){objActiveXElement.objectAttributes=parameters.objectAttributes;
 } var strMajorVersion=cVERSION.substring(0,1);if (strMajorVersion <="2"){strMajorVersion="";
 } if (objActiveXElement.isIE){objActiveXElement.codebase=parameters.path +'webimagefx' + strMajorVersion +'.cab#version=' + cVERSION;
 } else{objActiveXElement.codebase="webimagefx" + strMajorVersion +".ocx#version=" + cVERSION;
 } objActiveXElement.defineDocParams(parameters);objActiveXElement.defineNamedParams(parameters);
 objActiveXElement.defineNamedEvents(parameters);sHtml +=objActiveXElement.createHTML();
 } else{this.status=EWEP_STATUS_NOTSUPPORTED;} objActiveXElement=null;return sHtml;
}
function WebImageFXEditor_createInit(sEditorName){var sHtml="";var objActiveXElement=new ActiveXElement(cPROGID);
 this.isSupported=objActiveXElement.isSupported;if (this.isSupported){if (typeof parameters !="object" || (null==parameters)){
 parameters=WebImageFX.parameters;} var sLicenseKey=parameters["license"];var sConfigURL=parameters["config"];
 var sLocaleURL=parameters["locale"];var sLocation=document.location +"";var protocal=sLocation.substr(0,sLocation.indexOf(document.domain));
 if (sConfigURL.indexOf(protocal) < 0){sConfigURL=protocal + document.domain + sConfigURL;
 } if (sLocaleURL.indexOf(protocal) < 0){sLocaleURL=protocal + document.domain + sLocaleURL;
 } sHtml +='<scr' +'ipt language="JavaScript1.2">';sHtml +='WebImageFX.addEventHandler("onready", \'writeWIFXInit(\"' + sEditorName +'\", \"' + sLicenseKey +'\", \"' + sConfigURL +'\", \"' + sLocaleURL +'\")\');';
 sHtml +=writeInitfucntion();sHtml +='</scr' +'ipt>';} else{this.status=EWEP_STATUS_NOTSUPPORTED;
 } objActiveXElement=null;return sHtml;}
function WebImageFXEditor_isReady(){var isReady=false;if (("undefined"==typeof this.editor) || (null==this.editor)){
 var id=this.id;if (this.isIE){var evalWindowDocument="";if (this.editorWindow){evalWindowDocument=this.editorWindow +".";
 } evalWindowDocument +=this.editorDocument;this.editor=eval(evalWindowDocument)[id];
 } else if (this.isNetscape){if (this.editorWindow){this.editor=eval(this.editorWindow)[id];
 } else{this.editor=window[id];} } else{this.editor=null;} } if ((typeof this.editor !="undefined") && (this.editor !=null)){
 if (this.isIE && this.browserVersion < 5.0){if (this.receivedEvent){isReady=(typeof this.editor.Locale !="undefined");
 } else{isReady=false;} } else if (this.isNetscape && this.browserVersion >=6.0){
 isReady=(typeof this.editor.Locale !="undefined");} else{isReady=(eval('typeof this.editor.' + this.editorSetMethod +' != "undefined"'));
 } } else{isReady=false;} return isReady;}
function WebImageFXPopup(){extendObject(this,commonPopup);this.url=WebImageFXDefaults.popupUrl;
 this.windowName=WebImageFXDefaults.popupWindowName;this.windowFeatures=WebImageFXDefaults.popupWindowFeatures;
 this.query=WebImageFXDefaults.popupQuery;this.open=WebImageFXPopup_open;this.getInstance=WebImageFXPopup_getInstance;
}
function WebImageFXPopup_open(){if (this.url){var strUrl=this.url;if (this.elementName){
 strUrl +="?element=" + escape(this.elementName);if (this.elementWindow){strUrl +="&elementWindow=" + escape(this.elementWindow);
 } if (this.parametersName){strUrl +="&parameters=" + this.parametersName;} if (this.query){
 strUrl +="&" + this.query;} } this.objWindow=window.open(strUrl,this.windowName,this.windowFeatures);
 } else{this.objWindow=null;} return this.objWindow;}
function WebImageFXPopup_getInstance(){var objInstance=null;if (this.isOpen() && this.objWindow.WebImageFX){
 objInstance=this.objWindow.WebImageFX.instances[0];} return objInstance;}
function installPopup(){extendObject(this,commonPopup);this.isBlocked=false;this.url=WebImageFXDefaults.installPopupUrl;
 this.windowName=WebImageFXDefaults.installPopupWindowName;this.windowFeatures=WebImageFXDefaults.installPopupWindowFeatures;
 this.query=WebImageFXDefaults.installPopupQuery;this.open=installPopup_open;}
function installPopup_open(){this.isBlocked=false;if (this.url){var strUrl=this.url;
 if (this.query){strUrl +="?" + this.query;} this.objWindow=window.open(strUrl,this.windowName,this.windowFeatures);
 this.isBlocked=(null==this.objWindow);} else{this.objWindow=null;} return this.objWindow;
 }
function commonPopup(){this.objWindow=null;this.isOpen=commonPopup_isOpen;this.isClosedWindow=commonPopup_isClosedWindow;
 this.close=commonPopup_close;this.focus=commonPopup_focus;this.getWindow=commonPopup_getWindow;
}
function commonPopup_isOpen(){var bRet=false;try{bRet=(this.objWindow && !this.objWindow.closed);
 } catch (ex){bRet=false;} return bRet;}
function commonPopup_isClosedWindow(){try{return (this.objWindow && this.objWindow.closed);
 } catch (ex){return true;}}
function commonPopup_close(){if (this.isOpen()){try{this.objWindow.close();} catch (ex){
 } } this.objWindow=null;}
function commonPopup_focus(){if (this.isOpen()){this.objWindow.focus();}}
function commonPopup_getWindow(){if (this.isOpen()){return this.objWindow;} else{
 return null;}}
var EWEP_ONUNLOAD_PROMPT="prompt";var EWEP_ONUNLOAD_SAVE="save";var EWEP_ONUNLOAD_NOSAVE="nosave";
var g_WebImageFXFactory_singleton=false;function WebImageFXFactory(){if (g_WebImageFXFactory_singleton){
 alert("Only one instance of WebImageFXFactory may be created.");return;} g_WebImageFXFactory_singleton=true;
 this.delayOnLoadRetry=100;this.timeLimitOnLoadRetry=3000;this.parameters=new WebImageFXParameters;
 if (typeof cVERSION !="undefined"){this.version=cVERSION;} this.addInstanceType=WebImageFXFactory_addInstanceType;
 this.refreshStatus=WebImageFXFactory_refreshStatus;this.valid=WebImageFXFactory_valid;
 this.validButton=WebImageFXFactory_validButton;this.autoInstallExpected=WebImageFXFactory_autoInstallExpected;
 this.instances=[];this.create=WebImageFXFactory_create;this.popups=[];this.createButton=WebImageFXFactory_createButton;
 this.edit=WebImageFXFactory_edit;this.findPopup=WebImageFXFactory_findPopup;this.createPopup=WebImageFXFactory_createPopup;
 this.checkPopupsInterval=100;this.checkPopups=WebImageFXFactory_checkPopups;this.defineParameterPropertyNames=WebImageFXFactory_defineParameterPropertyNames;
 this.hookOnSubmit=WebImageFXFactory_hookOnSubmit;this.clear=WebImageFXFactory_clear;
 this.isChanged=WebImageFXFactory_isChanged;this.nextFormField=WebImageFXFactory_nextFormField;
 this.searchFormElements=WebImageFXFactory_searchFormElements;this.parseQuery=WebImageFXUtil_parseQuery;
 this.resolvePath=WebImageFXFactory_resolvePath;this.openDialog=WebImageFXFactory_openDialog;
 this.autoInstallCookie=new WebImageFXCookie("autoinstallfx");this.autoInstallRefresh=WebImageFXFactory_autoInstallRefresh;
 this.autoInstallChecks=WebImageFXFactory_autoInstallChecks;this.doInstall=WebImageFXFactory_doInstall;
 this.instanceTypes=[];for (var i=0;i < g_wifxInstanceTypes.length;i++){this.addInstanceType(g_wifxInstanceTypes[i]);
 } this.selectedType=this.instanceTypes[this.instanceTypes.length-1].type;this.refreshStatus();
}
 WebImageFXFactory.prototype=new ClassModel;function WebImageFXFactory_addInstanceType(fnInstanceType){
 var objInstanceType=new fnInstanceType();if ("undefined"==typeof this.instanceTypes[objInstanceType.type]){
 this.instanceTypes[this.instanceTypes.length]=objInstanceType;} this.instanceTypes[objInstanceType.type]=objInstanceType;
 if (this.parameters){objInstanceType.refreshStatus(this.parameters);}}
function WebImageFXFactory_refreshStatus(parameters){if (typeof parameters !="object" || (null==parameters)){
 parameters=this.parameters;} extendObject(this,PlatformInfo);if (parameters["preferredType"]){
 if (this.instanceTypes[parameters["preferredType"]]){WebImageFX.selectedType=parameters["preferredType"];
 } } for (var i=0;i < this.instanceTypes.length;i++){this.instanceTypes[i].refreshStatus(parameters);
 } var objInstanceType=this.instanceTypes[this.selectedType];if (objInstanceType){
 this.isSupported=objInstanceType.isSupported;this.isAutoInstallSupported=objInstanceType.isAutoInstallSupported;
 this.isInstalled=objInstanceType.isInstalled;this.upgradeNeeded=objInstanceType.upgradeNeeded;
 this.versionInstalled=objInstanceType.versionInstalled;} else{this.isSupported=false;
 this.isAutoInstallSupported=false;this.isInstalled=false;this.upgradeNeeded=false;
 } this.status=(this.isSupported ? (this.isInstalled ? EWEP_STATUS_INSTALLED : EWEP_STATUS_NOTINSTALLED) : EWEP_STATUS_NOTSUPPORTED);
 return this.status;}
function WebImageFXFactory_valid(){var bValid=(this.isSupported && (this.isInstalled || this.isAutoInstallSupported));
 var objInstanceType=this.instanceTypes[this.selectedType];if (bValid && objInstanceType){
 return objInstanceType.valid();} else{return bValid;}}
function WebImageFXFactory_validButton(){return (this.isSupported);}
function WebImageFXFactory_autoInstallExpected(){return (this.valid() && this.isAutoInstallSupported && (!this.isInstalled || this.upgradeNeeded));
}
function WebImageFXFactory_autoInstallRefresh(){if (this.autoInstallCookie){this.autoInstallCookie.removeCookie();
 location.reload();location.href="#";}}
function WebImageFXFactory_doInstall(){if (this.parameters.installPopup && this.parameters.installPopup.url){
 this.parameters.installPopup.open();if (!this.parameters.installPopup.isBlocked){
 if (this.autoInstallCookie){this.autoInstallCookie.setCookie(this.version);} } }
 else{if (ektMonarch){ektMonarch.autoInstallDone();} }}
function WebImageFXFactory_autoInstallChecks(){var objInstallPopup=this.parameters.installPopup;
 if ( null !=objInstallPopup ){if (objInstallPopup.isClosedWindow()){objInstallPopup.close();
 if (ektMonarch){ektMonarch.autoInstallDone();} } else{setTimeout("WebImageFX.autoInstallChecks()",this.checkPopupsInterval);
 } } else{if (ektMonarch){ektMonarch.autoInstallDone();} }}
function WebImageFXFactory_create(name,width,height,parameters){if (typeof parameters !="object" || (null==parameters)){
 parameters=this.parameters;} this.defineParameterPropertyNames(parameters);if (this.autoInstallExpected()){
 this.refreshStatus(parameters);} this.initEvent("oncreate");this.event.name=name;
 this.event.width=width;this.event.height=height;this.event.parameters=parameters;
 if (this.raiseEvent("oncreate")==false){return null;} name=this.event.name;width=this.event.width;
 height=this.event.height;parameters=this.event.parameters;if (typeof parameters !="object" || (null==parameters)){
 parameters=this.parameters;} var objInstance=null;parameters.isPopup=false;parameters.linkToName="";
 parameters.linkToWindow="";var strPopupElementWindow="top.opener";if (top.opener && top.location.search){
 var args=WebImageFXUtil.queryArgs;if (args["element"]){parameters.isPopup=true;if (args["elementWindow"]){
 strPopupElementWindow +="." + args["elementWindow"];} if (args["parameters"]){var objElem=findElement(args["parameters"],strPopupElementWindow);
 if (objElem && objElem["value"]){var sParameters=unescape(objElem.value);if (sParameters.length > 0){
 extendObject(parameters,new Function(sParameters));this.refreshStatus(parameters);
 } } } parameters.linkToName=args["element"];parameters.linkToWindow=strPopupElementWindow;
 } args=null;} if (!this.isNetscape60){parameters.editorDocumentLayer=findElementDocumentLayer(name,parameters.editorWindow);
 } var bShowAutoInstallMessage=false;if (this.autoInstallCookie){this.autoInstallCookie.evalDocument=parameters.getEditorDocument();
 this.autoInstallCookie.evalWindow=parameters.editorWindow;} var bExpectReload;bExpectReload=false;
 if (this.autoInstallExpected() && parameters.installPopup && parameters.installPopup.url){
 this.isAutoInstallSupported=false;if (this.upgradeNeeded){this.isInstalled=false;
 } bShowAutoInstallMessage=true;var bNeedAutoInstall=true;if (this.autoInstallCookie){
 bNeedAutoInstall=(this.version !=this.autoInstallCookie.getCookie());} if (bNeedAutoInstall){
 if (ektMonarch){this.parameters=parameters;ektMonarch.requestAutoInstall(WebImageFX);
 } bExpectReload=true;} } else if (this.isAutoInstallSupported && parameters.installPopup){
 if (this.autoInstallCookie){this.autoInstallCookie.removeCookie();} } var objInstanceType=null;
 var iPreferredType=0;objInstanceType=this.instanceTypes[iPreferredType];objInstanceType.isSupported=objInstanceType.isSupported && this.isSupported;
 objInstanceType.isAutoInstallSupported=objInstanceType.isAutoInstallSupported && this.isAutoInstallSupported;
 objInstanceType.isInstalled=objInstanceType.isInstalled && this.isInstalled;objInstanceType.upgradeNeeded=objInstanceType.upgradeNeeded && this.upgradeNeeded;
 if (iPreferredType >=0){objInstanceType=this.instanceTypes[iPreferredType];if (false==bExpectReload){
 if (!objInstanceType.isInstalled && this.isNetscape){if (parameters.installPopup.isBlocked && WebImageFXMessages.clientInstallSP2Message){
 conditional_write(WebImageFXMessages.clientInstallSP2Message,objInstance,parameters);
 } else if (WebImageFXMessages.clientInstallMessage){if (this.isWinVista && WebImageFXMessages.clientMsiInstallMessage){
 conditional_write(WebImageFXMessages.clientMsiInstallMessage,objInstance,parameters);
 } else{conditional_write(WebImageFXMessages.clientInstallMessage,objInstance,parameters);
 } } } else{objInstance=objInstanceType.create(name,width,height,parameters);} } objInstanceType=this.instanceTypes["activex"];
 if (objInstanceType){if (objInstanceType.isSupported && !objInstanceType.isInstalled){
 if (WebImageFXMessages.clientInstallMessage){} } } if (bShowAutoInstallMessage){
 if (WebImageFXMessages.clientAutoInstallMessage){} } } if (!objInstance){return null;
 } if (this.instances.length==0){this.instances=new Array();} this.instances[this.instances.length]=objInstance;
 this.instances[name]=objInstance;if (!window["WebImageFXUnloadHandled"]){window.WebImageFXUnloadHandled=true;
 } if (typeof this.actionOnUnload=="undefined"){this.actionOnUnload=EWEP_ONUNLOAD_NOSAVE;
 } if (!window["WebImageFXLoadHandled"]){window.WebImageFXLoadHandled=true;} if (objInstance){
 objInstance.totalDelayOnLoadRetry=0;} return objInstance;}
function WebImageFXFactory_createButton(buttonName,elementName,parameters){var oPopup=null;
 this.initEvent("oncreatebutton");this.event.buttonName=buttonName;this.event.elementName=elementName;
 this.event.parameters=parameters;if (this.raiseEvent("oncreatebutton")==false){return;
 } buttonName=this.event.buttonName;elementName=this.event.elementName;parameters=this.event.parameters;
 if (this.validButton()){if (typeof parameters !="object" || (null==parameters)){
 parameters=this.parameters;} if (!this.isNetscape60){parameters.editorDocumentLayer=findElementDocumentLayer(elementName,parameters.editorWindow);
 } this.defineParameterPropertyNames(parameters);var aryNames=findFormAndElementName(elementName);
 var name=aryNames[0] + aryNames[1];var i=this.popups.length;oPopup=this.createPopup(parameters.popup,elementName,parameters.editorWindow,"wifxParameters" + i);
 oPopup.html="";if (!aryNames[0]){if (WebImageFXMessages.elementNotFoundMessage){
 conditional_write(WebImageFXMessages.elementNotFoundMessage + elementName,null,parameters);
 } } conditional_write('<input type="hidden" name="' + oPopup.parametersName +'" value="' + escape(parameters.reconstructor()) +'">',oPopup,parameters);
 if (buttonName && parameters.buttonTag && parameters.buttonTag.valid()){var sButtonHtml=parameters.buttonTag.createHTML(buttonName,elementName);
 conditional_write(sButtonHtml,oPopup,parameters);} } return oPopup;}
function WebImageFXFactory_edit(elementName){this.initEvent("onbeforeedit");this.event.elementName=elementName;
 if (this.raiseEvent("onbeforeedit")==false){return;} elementName=this.event.elementName;
 var oPopup=this.popups[elementName];if (!oPopup){oPopup=this.createPopup(this.parameters.popup,elementName,this.parameters.editorWindow);
 } oPopup.changed=false;oPopup.open();setTimeout("WebImageFX.checkPopups()",WebImageFX.checkPopupsInterval);
}
function WebImageFXFactory_findPopup(elementName){var oPopup=null;for (var i=0;i < this.popups.length;
 i++){if (this.popups[i].elementName==elementName){oPopup=this.popups[i];break;} }
 return oPopup;}
function WebImageFXFactory_createPopup(oPopupPrototype,elementName,elementWindow,parametersName){
 var oPopup=null;if (oPopupPrototype){oPopup=cloneObject(oPopupPrototype);} else{
 oPopup=new WebImageFXPopup;} if (elementName){oPopup.elementName=elementName;} if (elementWindow){
 oPopup.elementWindow=elementWindow;} if (parametersName){oPopup.parametersName=parametersName;
 } if (this.popups.length==0){this.popups=new Array();} this.popups[this.popups.length]=oPopup;
 if (elementName){this.popups[elementName]=oPopup;} return oPopup;}
function WebImageFXFactory_checkPopups(){var objPopup=null;var numOpenPopups=0;for (var i=0;
 i < this.popups.length;i++){objPopup=this.popups[i];if (objPopup.isOpen()){numOpenPopups++;
 } else if (objPopup.isClosedWindow()){this.initEvent("onedit");this.event.elementName=this.popups[i].elementName;
 this.event.popup=this.popups[i];this.raiseEvent("onedit");objPopup.close();} } objPopup=null;
 if (numOpenPopups > 0){setTimeout("WebImageFX.checkPopups()",WebImageFX.checkPopupsInterval);
 }}
function WebImageFXFactory_defineParameterPropertyNames(parameters){parameters.definePropertyName("preferredType");
 for (var iType=0;iType < this.instanceTypes.length;iType++){var aryParameterPropertyNames=this.instanceTypes[iType].parameterPropertyNames;
 for (var iName=0;iName < aryParameterPropertyNames.length;iName++){parameters.definePropertyName(aryParameterPropertyNames[iName]);
 } }}
function WebImageFXFactory_resolvePath(strURL){return this.parameters.path + strURL;
}
function WebImageFXFactory_openDialog(editorName,fileName,query,windowName,windowFeatures){
 var strUrl=fileName +"";if (strUrl.indexOf("?") < 0){strUrl +="?";} else{strUrl +="&";
 } strUrl +="editorName=" + escape(editorName) +"&editorname=" + escape(editorName);
 if (query){strUrl +="&" + query;} var oWin=window.open(strUrl,windowName,windowFeatures);
 if (null==oWin && WebImageFXMessages.popupBlockedMessage){alert(WebImageFXMessages.popupBlockedMessage);
 } return oWin;}
function WebImageFXFactory_isChanged(){for (var i=0;i < this.instances.length;i++){
 if (this.instances[i].isChanged()){return true;} } return false;}
function WebImageFXFactory_hookOnSubmit(objForm){if (!objForm || typeof objForm !="object"){
 return;} var bHook=false;if (objForm.onsubmit){bHook=true;var objHook=new ClassModel();
 objHook.defineProperty("fnOldOnSubmit",objForm.onsubmit);} var sCode="";if (bHook){
 sCode +="extendObject(this, new Function(unescape('" + escape(objHook.reconstructor()) +"')));\n";
 } sCode +="var bReturned = false;\n";sCode +="var bResult = false;\n";if (bHook){
 sCode +="if (bResult || 'undefined' == typeof bResult) bResult = this.fnOldOnSubmit();\n";
 } sCode +="if (bResult && bReturned) {\n";sCode +='  if (("function" == typeof this.submit) || (("object" == typeof this.submit) && ("undefined" == typeof this.submit.value))) { this.submit() } \n';
 sCode +='  else {\n';sCode +='    var sMsg = "Asynchronous WebImageFX methods require the form.submit method.\\n";\n';
 sCode +='	  sMsg += "A form element named \\\'submit\\\' already exists that replaced the form.submit method.\\n";\n';
 sCode +='	  sMsg += "\\nPlease change the name of the \\\'submit\\\' element to another name.\\n";\n';
 sCode +='	  if (this.submit.outerHTML) {\n';sCode +='	    sMsg += "The \\\'submit\\\' element is defined as: " + this.submit.outerHTML + "\\n";\n';
 sCode +='	  }\n';sCode +='	  sMsg += "\\nThe form cannot be submitted asynchronously until this is fixed.\\n";\n';
 sCode +='	  alert(sMsg);\n';sCode +='  }\n';sCode +="}\n";sCode +="return bResult; });\n";
 sCode +="bReturned = true;\n";sCode +="return bResult;\n";var fnNewOnSubmit=new Function(sCode);
 objForm.onsubmit=fnNewOnSubmit;return fnNewOnSubmit;}
function WebImageFXFactory_clear(){if (this.instanceTypes.length > 0){for (var i=this.instanceTypes.length - 1;
 i >=0;i--){this.instanceTypes[i].clear();} } if (this.instances.length > 0){for (var i=this.instances.length - 1;
 i >=0;i--){var name=this.instances[i].name;this[name]=null;this.instances[i].clear();
 this.instances[name]=null;this.instances[i]=null;this.instances.length--;} this.instances=[];
 } if (this.popups.length > 0){for (var i=this.popups.length - 1;i >=0;i--){var elementName=this.popups[i].elementName;
 this.popups[i].close();this.popups[i]=null;if (elementName){this.popups[elementName]=null;
 } this.popups.length--;} this.popups=new Array();} }
function WebImageFXFactory_nextFormField(objInstance){var objField=this.searchFormElements(objInstance,false);
 if (!objField){objField=this.searchFormElements(objInstance,true);} return objField;
}
function WebImageFXFactory_searchFormElements(objInstance,bSearchForNextField){for (var iForm=0;
 iForm < document.forms.length;iForm++){for (var iElem=0;iElem < document.forms[iForm].elements.length;
 iElem++){var objElem=document.forms[iForm].elements[iElem];if (objInstance.isInstance(objElem)){
 bSearchForNextField=true;} else if (bSearchForNextField){if (canElementReceiveFocus(objElem)){
 return objElem;} else{for (var i=0;i < this.instances[i].length;i++){if (this.instances[i].isInstance(objElem)){
 return this.instances[i];} } } } } } return null;}
function canElementReceiveFocus(objElem){if (!objElem) return false;var strType=objElem.type +"";
 if ("hidden"==strType) return false;var strDisabled=objElem.disabled +"";if ("true"==strDisabled) return false;
 var strIsTextEdit=objElem.isTextEdit +"";if ("false"==strIsTextEdit) return false;
 var strFocusMethod=typeof objElem.focus;if ("function" !=strFocusMethod &&"object" !=strFocusMethod) return false;
 return true;}
var WebImageFX=new WebImageFXFactory;