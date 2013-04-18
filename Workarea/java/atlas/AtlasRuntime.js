//-----------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------
// AtlasRuntime.js
// Atlas Runtime Framework.
Function.abstractMethod =function(){throw 'Abstract method should be implemented';}
Function.createCallback =function(method,context){return function(){method(context);}
}
Function.createCallbackWithArguments =function(method,context){return function(){var args =[];for (var i =2;i <arguments.length;i++){args.add(arguments[i]);}
args.add(context);method.apply(null,args);}
}
Function.createDelegate =function(instance,method){return function(){method.apply(instance,arguments);}
}
Function.emptyMethod =function(){}
Function.prototype.getBaseMethod =function(instance,methodName){var baseMethod =null;var baseType =this.getBaseType();if (baseType){var directBaseType =baseType;if (instance._baseMethods){while (baseType){var methodKey =baseType.getName()+'.'+methodName;var method =instance._baseMethods[methodKey];if (method){return method;}
baseType =baseType._baseType;}
}
if (!baseMethod){return directBaseType.prototype[methodName];}
}
return null;}
Function.prototype.getBaseType =function(){return this._baseType;}
Function.prototype.getName =function(){return this._typeName;}
Function.parse =function(functionName){var fn =null;try {eval('fn = '+functionName);if (typeof(fn)!='function'){fn =null;}
}
catch (ex){}
return fn;}
Function.prototype._processBaseType =function(){if (this._basePrototypePending){var baseType =this._baseType;baseType._processBaseType();for (var memberName in baseType.prototype){var memberValue =baseType.prototype[memberName];if (!this.prototype[memberName]){this.prototype[memberName]=memberValue;}
}
delete this._basePrototypePending;}
}
Function.prototype.callBaseMethod =function(instance,methodName,baseArguments){var baseMethod =this.getBaseMethod(instance,methodName);if (baseMethod){if (!baseArguments){return baseMethod.apply(instance);}
else {return baseMethod.apply(instance,baseArguments);}
}
return null;}
Function.prototype.implementsInterface =function(interfaceType){var interfaces =this._interfaces;if (interfaces){if (interfaces.contains(interfaceType)){return true;}
}
var baseType =this._baseType;while (baseType){interfaces =baseType._interfaces;if (interfaces){if (interfaces.contains(interfaceType)){return true;}
}
baseType =baseType._baseType;}
return false;}
Function.prototype.inheritsFrom =function(parentType){var baseType =this._baseType;while (baseType){if (baseType ==parentType){return true;}
baseType =baseType._baseType;}
return false;}
Function.prototype.initializeBase =function(instance,baseArguments){this._processBaseType();if (this._interfaces){for (var i =0;i <this._interfaces.length;i++){this._interfaces[i].call(instance);}
}
if (this._baseType){if (!baseArguments){this._baseType.apply(instance);}
else {this._baseType.apply(instance,baseArguments);}
}
return instance;}
Function.prototype.isImplementedBy =function(instance){if (!instance)return false;var instanceType =Object.getType(instance);return instanceType.implementsInterface(this);}
Function.prototype.isInstanceOfType =function(instance){if (typeof(instance)=='undefined'||instance ==null){return false;}
if (instance instanceof this){return true;}
var instanceType =Object.getType(instance);if (instanceType ==this){return true;}
return instanceType.inheritsFrom(this);}
Function.prototype.registerBaseMethod =function(instance,methodName){if (!instance._baseMethods){instance._baseMethods ={};}
var methodKey =this.getName()+'.'+methodName;instance._baseMethods[methodKey]=instance[methodName];}
Function.createInstance =function(type){if (typeof(type)!='function'){type =Function.parse(type);}
return new type();}
Function.registerClass =function(typeName,baseType,interfaceType){var type =Function.parse(typeName);if (window.__safari){eval(typeName +'.prototype.constructor = '+typeName +';');}
type._typeName =typeName;if (baseType){type._baseType =baseType;type._basePrototypePending =true;}
if (interfaceType){type._interfaces =[];for (var i =2;i <arguments.length;i++){interfaceType =arguments[i];type._interfaces.add(interfaceType);}
}
return type;}
Function.registerAbstractClass =function(typeName,baseType){var type =Type.registerClass.apply(null,arguments);type._abstract =true;return type;}
Function.registerSealedClass =function(typeName,baseType){var type =Type.registerClass.apply(null,arguments);type._sealed =true;return type;}
Function.registerInterface =function(typeName){var type =Function.parse(typeName);type._typeName =typeName;type._interface =true;type._abstract =true;type._sealed =true;return type;}
Function.registerNamespace =function(namespacePath){var rootObject =window;var namespaceParts =namespacePath.split('.');for (var i =0;i <namespaceParts.length;i++){var currentPart =namespaceParts[i];if (!rootObject[currentPart]){rootObject[currentPart]=new Object();}
rootObject =rootObject[currentPart];}
}
Function._typeName ='Function';window.Type =Function;Object.getType =function(instance){return instance.constructor;}
Object._typeName ='Object';Boolean.parse =function(value){if (typeof(value)=='string'){return (value.toLowerCase()=='true');}
return value ?true :false;}
Number.parse =function(value){if (!value ||(value.length ==0)){return 0;}
return parseFloat(value);}
Number._typeName ='Number';String.prototype.endsWith =function(suffix){return (this.substr(this.length -suffix.length)==suffix);}
String.prototype.startsWith =function(prefix){return (this.substr(0,prefix.length)==prefix);}
String.prototype.trimLeft =function(){return this.replace(/^\s*/,"");}
String.prototype.trimRight =function(){return this.replace(/\s*$/,"");}
String.prototype.trim =function(){return this.trimRight().trimLeft();}
String.format =function(format){for (var i =1;i <arguments.length;i++){format =format.replace("{"+(i -1)+"}",arguments[i]);}
return format;}
String._typeName ='String';Array.prototype.add =function(item){this.push(item);}
Array.prototype.addRange =function(items){var length =items.length;if (length !=0){for (var index =0;index <length;index++){this.push(items[index]);}
}
}
Array.prototype.clear =function(){if (this.length >0){this.splice(0,this.length);}
}
Array.prototype.clone =function(){var clonedArray =[];var length =this.length;for (var index =0;index <length;index++){clonedArray[index]=this[index];}
return clonedArray;}
Array.prototype.contains =function(item){var index =this.indexOf(item);return (index >=0);}
Array.prototype.dequeue =function(){return this.shift();}
Array.prototype.indexOf =function(item){var length =this.length;if (length !=0){for (var index =0;index <length;index++){if (this[index]==item){return index;}
}
}
return -1;}
Array.prototype.insert =function(index,item){this.splice(index,0,item);}
Array.prototype.queue =function(item){this.push(item);}
Array.prototype.remove =function(item){var index =this.indexOf(item);if (index >=0){this.splice(index,1);}
}
Array.prototype.removeAt =function(index){this.splice(index,1);}
Array._typeName ='Array';Array.parse =function(value){return eval('('+value +')');}
RegExp.parse =function(value){if (value.startsWith('/')){var endSlashIndex =value.lastIndexOf('/');if (endSlashIndex >1){var expression =value.substring(1,endSlashIndex);var flags =value.substr(endSlashIndex +1);return new RegExp(expression,flags);}
}
return null;}
RegExp._typeName ='RegExp';Date._typeName ='Date';Type.registerNamespace('Web');Web._Debug =function(){this.assert =function(condition,message,displayCaller){if (!condition){message ='Assertion Failed: '+message +(displayCaller ?'\r\nat '+this.assert.caller :'');if (confirm(message +'\r\n\r\nBreak into debugger?')){this.fail(message);}
}
}
this.clearTrace =function(){var traceElement =document.getElementById('__atlas_trace');if (traceElement){var children =traceElement.childNodes;for(var i =children.length -2;i >0;i--){traceElement.removeChild(children[i]);}
document.getElementById('__atlas_trace').style.display ='none';}
}
this.dump =function(object,name,recursive,indentationPadding){name =name ?name :'';indentationPadding =indentationPadding ?indentationPadding :'';if (object ==null){this.trace(indentationPadding +name +': null');return;}
switch(typeof(object)){case 'undefined':this.trace(indentationPadding +name +': Undefined');break;case 'number':case 'string':case 'boolean':this.trace(indentationPadding +name +': '+object);break;default:var typeName =Object.getType(object).getName();this.trace(indentationPadding +name +(typeof(typeName)=='string'?' {'+typeName +'}':''));if ((indentationPadding =='')||recursive){var i,length,properties,p,v;if (Web.IArray.isImplementedBy(object)){length =object.get_length();for (i =0;i <length;i++){this.dump(object.getItem(i),'['+i +']',recursive,indentationPadding +'+');}
}
if (Web.ITypeDescriptorProvider.isImplementedBy(object)){var td =Web.TypeDescriptor.getTypeDescriptor(object);properties =td._getProperties();for (p in properties){var propertyInfo =properties[p];if (propertyInfo.name){v =Web.TypeDescriptor.getProperty(object,propertyInfo.name);this.dump(v,p,recursive,indentationPadding +'+');}
}
}
else {for (p in object){v =object[p];if (!Function.isInstanceOfType(v)&&!Web.Event.isInstanceOfType(v)){this.dump(v,p,recursive,indentationPadding +'+');}
}
}
}
}
}
this.fail =function(message){Debug.breakIntoDebugger(message);}
this.trace =function(text){Debug.writeln(text);var traceElement =document.getElementById('__atlas_trace');if (!traceElement){traceElement =document.createElement('FIELDSET');traceElement.id ='__atlas_trace';traceElement.style.backgroundColor ='white';traceElement.style.color ='black';traceElement.style.textAlign ='left';traceElement.style.font ='normal normal normal 1em/1.1em verdana,sans-serif';var legend =document.createElement('LEGEND');var legendText =document.createTextNode('Debugging Trace');legend.appendChild(legendText);traceElement.appendChild(legend);var clearButton =document.createElement('INPUT');clearButton.type ='button';clearButton.value ='Clear Trace';clearButton.onclick =debug.clearTrace;traceElement.appendChild(clearButton);document.body.appendChild(traceElement);}
var traceLine =document.createElement('DIV');traceLine.innerHTML =text;traceElement.insertBefore(traceLine,traceElement.childNodes[traceElement.childNodes.length -1]);traceElement.style.display ='block';}
}
window.debug =new Web._Debug();Web.Enum =new function(){function getValues(){if (!this._values){var values ={};for (var f in this){if (typeof(this[f])!='function'){values[f]=this[f];}
}
this._values =values;}
return this._values;}
function valueFromString(s){for (var f in this){if (f ==s){return this[f];}
}
throw 'Invalid Enumeration Value';}
function valueToString(value){for (var i in this){if (this[i]==value){return i;}
}
throw 'Invalid Enumeration Value';}
this.create =function(){var enumeration ={};enumeration.getValues =getValues;enumeration.parse =valueFromString;enumeration.toString =valueToString;for (var i =0;i <arguments.length;i++){enumeration[arguments[i]]=i;}
return enumeration;}
}
Web.Flags =new function(){function valueFromString(s){var parts =s.split('|');var value =0;for (var i =parts.length -1;i >=0;i--){var part =parts[i].trim();var found =false;for (var f in this){if (f ==part){value |=this[f];found =true;break;}
}
if (found ==false){throw 'Invalid Enumeration Value';}
}
return value;}
function valueToString(value){var sb =new Web.StringBuilder();for (var i in this){if ((this[i]&value)!=0){if (sb.isEmpty()==false){sb.append(' | ');}
sb.append(i);}
}
return sb.toString();}
this.create =function(){var flags =new Object();flags.parse =valueFromString;flags.toString =valueToString;for (var i =0;i <arguments.length;i +=2){var name =arguments[i];var value =arguments[i +1];flags[name]=value;}
return flags;}
}
Web.IArray =function(){this.get_length =Function.abstractMethod;this.getItem =Function.abstractMethod;}
Type.registerInterface("Web.IArray");Array.prototype.get_length =function(){return this.length;}
Array.prototype.getItem =function(index){return this[index];}
Array._interfaces =[];Array._interfaces.add(Web.IArray);Web.IDisposable =function(){this.dispose =Function.abstractMethod;}
Type.registerInterface('Web.IDisposable');Web.StringBuilder =function(initialText){var _parts =new Array();if ((typeof(initialText)=='string')&&(initialText.length !=0)){_parts.add(initialText);}
this.append =function(text){if ((text ==null)||(typeof(text)=='undefined')){return;}
if ((typeof(text)=='string')&&(text.length ==0)){return;}
_parts.add(text);}
this.appendLine =function(text){this.append(text);_parts.add('\r\n');}
this.clear =function(){_parts.clear();}
this.isEmpty =function(){return (_parts.length ==0);}
this.toString =function(delimiter){delimiter =delimiter ||'';return _parts.join(delimiter);}
}
Type.registerSealedClass('Web.StringBuilder');Web.ActionSequence =Web.Enum.create('BeforeEventHandler','AfterEventHandler');Web.IAction =function(){this.get_sequence =Function.abstractMethod;this.execute =Function.abstractMethod;this.setOwner =Function.abstractMethod;}
Type.registerInterface('Web.IAction');Web.Event =function(owner,autoInvoke){var _owner =owner;var _handlers =null;var _actions =null;var _autoInvoke =autoInvoke;var _invoked =false;this.get_autoInvoke =function(){return _autoInvoke;}
this._getActions =function(){if (_actions ==null){_actions =Web.Component.createCollection(_owner);}
return _actions;}
this._getHandlers =function(){if (_handlers ==null){_handlers =[];}
return _handlers;}
this._getOwner =function(){return _owner;}
this.isActive =function(){return ((_handlers !=null)&&(_handlers.length !=0))||((_actions !=null)&&(_actions.length !=0));}
this.get_isInvoked =function(){return _isInvoked;}
this.dispose =function(){if (_handlers){for (var h =_handlers.length -1;h >=0;h--){_handlers[h]=null;}
_handlers =null;}
if (_actions){_actions.dispose();_actions =null;}
_owner =null;}
this._setInvoked =function(value){_invoked =true;}
}
Type.registerSealedClass('Web.Event',null,Web.IDisposable);Web.Event.prototype.add =function(handler){this._getHandlers().add(handler);if (this.get_autoInvoke()&&this.get_isInvoked()){handler(this._getOwner(),Web.EventArgs.Empty);}
}
Web.Event.prototype.addAction =function(action){this._getActions().add(action);if (this.get_autoInvoke()&&this.get_isInvoked()){action.execute(this._getOwner(),Web.EventArgs.Empty);}
}
Web.Event.prototype.remove =function(handler){this._getHandlers().remove(handler);}
Web.Event.prototype.removeAction =function(action){this._getActions().remove(action);}
Web.Event.prototype.invoke =function(sender,eventArgs){if (this.isActive()){var actions =this._getActions();var handlers =this._getHandlers();var hasPostActions =false;var i;for (i =0;i <actions.length;i++){if (actions[i].get_sequence()==Web.ActionSequence.BeforeEventHandler){actions[i].execute(sender,eventArgs);}
else {hasPostActions =true;}
}
for (i =0;i <handlers.length;i++){handlers[i](sender,eventArgs);}
if (hasPostActions){for (i =0;i <actions.length;i++){if (actions[i].get_sequence()==Web.ActionSequence.AfterEventHandler){actions[i].execute(sender,eventArgs);}
}
}
this._setInvoked();}
}
Web.EventArgs =function(){this.getDescriptor =function(){var td =new Web.TypeDescriptor();return td;}
Web.EventArgs.registerBaseMethod(this,'getDescriptor');}
Type.registerClass('Web.EventArgs',null,Web.ITypeDescriptorProvider);Web.EventArgs.Empty =new Web.EventArgs();Web.CancelEventArgs =function(){Web.CancelEventArgs.initializeBase(this);var _canceled =false;this.get_canceled =function(){return _canceled;}
this.set_canceled =function(value){_canceled =value;}
this.getDescriptor =function(){var td =Web.CancelEventArgs.callBaseMethod(this,'getDescriptor');td.addProperty('canceled',Boolean);return td;}
Web.CancelEventArgs.registerBaseMethod(this,'getDescriptor');}
Type.registerClass('Web.CancelEventArgs',Web.EventArgs);Web.ApplicationType =Web.Enum.create('Other','InternetExplorer','Firefox');Web._Application =function(){var _scriptDOM;var _scriptObjectsNode;var _references;var _currentLoadingReference;var _unloading;var _markupContext;var _disposableObjects;var _type;window.attachEvent('onload',onWindowLoad);window.attachEvent('onunload',onWindowUnload);this.get_type =function(){if (!_type){_type =Web.ApplicationType.Other;if (navigator.userAgent.indexOf('MSIE')!=-1){_type =Web.ApplicationType.InternetExplorer;}
else if (navigator.userAgent.indexOf('Firefox')!=-1){_type =Web.ApplicationType.Firefox;}
}
return _type;}
this.get_userAgent =function(){return navigator.userAgent;}
this.load =new Web.Event(this);this.unload =new Web.Event(this);this.dispose =function(){}
this.findObject =function(id){if (_markupContext){return _markupContext.findObject(id,false);}
return null;}
this.getProperty =function(name,key){}
this.getService =function(serviceType){return null;}
this._initialize =function(){var atlasScript;var scripts =document.getElementsByTagName('script');for (var s =0;s <scripts.length;s++){if (scripts[s].type =='text/xml-script'){atlasScript =scripts[s];break;}
}
if (!atlasScript){onLoad();return;}
var scriptMarkup =atlasScript.text ?atlasScript.text :atlasScript.innerHTML;if (scriptMarkup.startsWith('<!--')){var startIndex =scriptMarkup.indexOf('<',1);var endIndex =scriptMarkup.lastIndexOf('>');endIndex =scriptMarkup.lastIndexOf('>',endIndex -1);scriptMarkup =scriptMarkup.substring(startIndex,endIndex +1);}
var scriptDOM =new XMLDOM(scriptMarkup);var scriptDocumentNode =scriptDOM.childNodes[0];var scriptDocumentItemNodes =scriptDocumentNode.childNodes;var referencesNode;var node;for (var i =scriptDocumentItemNodes.length -1;i >=0;i--){node =scriptDocumentItemNodes[i];if (node.nodeType !=1){continue;}
if (node.baseName =='components'){_scriptObjectsNode =node;}
else if (node.baseName =='references'){referencesNode =node;}
}
if (referencesNode){_references =[];for (var r =0;r <referencesNode.childNodes.length;r++){node =referencesNode.childNodes[r];if (node.nodeType !=1){continue;}
if (node.baseName =='add'){var srcAttribute =node.attributes.getNamedItem('src');if (srcAttribute){_references.queue(srcAttribute.nodeValue);}
}
}
}
if (_references &&_references.length){loadReferences();}
else {loadObjects();}
}
this.invokeMethod =function(methodName,parameters){var method =Function.parse(methodName);if (typeof(method)=='function'){method();}
}
this.registerDisposableObject =function(object){if (!_disposableObjects){_disposableObjects =[];}
_disposableObjects.add(object);}
this.unregisterDisposableObject =function(object){if (!_unloading &&_disposableObjects){_disposableObjects.remove(object);}
}
this.requiresReference =function(scriptPath){}
this.setProperty =function(name,value,key){}
function loadObjects(){onLoad();}
function loadReferences(){if (_currentLoadingReference){if ((_currentLoadingReference.readyState !='loaded')&&(_currentLoadingReference.readyState !='complete')){return;}
else {if (Web.Application.get_type()!=Web.ApplicationType.InternetExplorer){_currentLoadingReference.onload =null;}
else {_currentLoadingReference.onreadystatechange =null;}
_currentLoadingReference =null;}
}
if (_references &&_references.length){var reference =_references.dequeue();var scriptElement =document.createElement('script');_currentLoadingReference =scriptElement;if (Web.Application.get_type()!=Web.ApplicationType.InternetExplorer){scriptElement.readyState ='loaded';scriptElement.onload =loadReferences;}
else {scriptElement.onreadystatechange =loadReferences;}
scriptElement.type ='text/javascript';scriptElement.src =reference;var headElement =document.getElementsByTagName('head')[0];headElement.appendChild(scriptElement);return;}
loadObjects();}
function onLoad(){Web.Application.load.invoke(Web.Application,Web.EventArgs.Empty);var pageLoadHandler =Function.parse('pageLoad');if (typeof(pageLoadHandler)=='function'){pageLoadHandler();}
}
function onWindowLoad(){window.detachEvent('onload',onWindowLoad);Web.Application._initialize();}
function onWindowUnload(){window.detachEvent('onunload',onWindowUnload);Web.Application.unload.invoke(Web.Application,Web.EventArgs.Empty);var pageUnloadHandler =Function.parse('pageUnload');if (typeof(pageUnloadHandler)=='function'){pageUnloadHandler();}
if (_disposableObjects){_unloading =true;var count =_disposableObjects.length;for (var i =0;i <count;i++){_disposableObjects[i].dispose();}
_disposableObjects.clear();_disposableObjects =null;}
}
}
Type.registerSealedClass('Web._Application',null,Web.IDisposable);Web.Application =new Web._Application();Web.Component =function(registerAsDisposable){var _id =null;var _initialized =false;var _updating =false;var _events =[];if (registerAsDisposable){Web.Application.registerDisposableObject(this);}
this.get_id =function(){return _id;}
this.set_id =function(value){_id =value;}
this.get_isInitialized =function(){return _initialized;}
this.get_isUpdating =function(){return _updating;}
this.createEvent =function(autoInvoke){var eventObject =new Web.Event(this,autoInvoke);_events.add(eventObject);return eventObject;}
this.beginUpdate =function(){_updating =true;}
this.dispose =function(){if (_events){for (var e =_events.length -1;e >=0;e--){_events[e].dispose();_events[e]=null;}
_events =null;}
Web.Application.unregisterDisposableObject(this);}
Web.Component.registerBaseMethod(this,'dispose');this.endUpdate =function(){_updating =false;if (!_initialized){this.initialize();}
this.updated();}
this.initialize =function(){_initialized =true;}
Web.Component.registerBaseMethod(this,'initialize');this.updated =function(){}
Web.Component.registerBaseMethod(this,'updated');}
Type.registerAbstractClass('Web.Component',null,Web.IDisposable);Web.Component.createCollection =function(owner){var collection =[];collection._owner =owner;collection._add =collection.add;collection.add =function(item){this._add(item);item.setOwner(this._owner);}
collection.dispose =function(){for (var i =this.length -1;i >=0;i--){this[i].dispose();this[i]=null;}
this.clear();}
return collection;}
Type.registerNamespace('Web.Serialization');Web.Serialization.JSON =new function(){function serializeWithBuilder(object,stringBuilder){var i;switch (typeof object){case 'object':if (object){if (Array.isInstanceOfType(object)){stringBuilder.append('[');for (i =0;i <object.length;++i){if (i >0){stringBuilder.append(',');}
stringBuilder.append(serializeWithBuilder(object[i],stringBuilder));}
stringBuilder.append(']');} else {if (typeof object.serialize =='function'){stringBuilder.append(object.serialize());break;}
stringBuilder.append('{');var needComma =false;for (var name in object){if (name.startsWith('$')){continue;}
var value =object[name];if (typeof value !='undefined'&&typeof value !='function'){if (needComma){stringBuilder.append(',');}
else {needComma =true;}
stringBuilder.append(serializeWithBuilder(name,stringBuilder));stringBuilder.append(':');stringBuilder.append(serializeWithBuilder(value,stringBuilder));}
}
stringBuilder.append('}');}
}
else {stringBuilder.append('null');}
break;case 'number':if (isFinite(object)){stringBuilder.append(String(object));}
else {stringBuilder.append('null');}
break;case 'string':stringBuilder.append('"');var length =object.length;for (i =0;i <length;++i){var curChar =object.charAt(i);if (curChar >=' '){if (curChar =='\\'||curChar =='"'){stringBuilder.append('\\');}
stringBuilder.append(curChar);}
else {switch (curChar){case '\b':stringBuilder.append('\\b');break;case '\f':stringBuilder.append('\\f');break;case '\n':stringBuilder.append('\\n');break;case '\r':stringBuilder.append('\\r');break;case '\t':stringBuilder.append('\\t');break;default:stringBuilder.append('\\u00');stringBuilder.append(curChar.charCodeAt().toString(16));}
}
}
stringBuilder.append('"');break;case 'boolean':stringBuilder.append(object.toString());break;default:stringBuilder.append('null');break;}
}
this.serialize =function(object){var stringBuilder =new Web.StringBuilder();serializeWithBuilder(object,stringBuilder);return stringBuilder.toString();}
this.deserialize =function(data){return eval('('+data +')');}
}
Type.registerNamespace('Web.Net');Web.Net.WebResponse =function(requestor,userContext){var _requestor =requestor;var _userContext =userContext;var _resultObject;this.get_data =function(){return _requestor.responseText;}
this.get_object =function(){if (!_resultObject){var data =this.get_data();_resultObject =Web.Serialization.JSON.deserialize(data);}
return _resultObject;}
this.get_statusCode =function(){return _requestor.status;}
this.get_statusText =function(){return _requestor.statusText;}
this.get_userContext =function(){return _userContext;}
this.get_xml =function(){return _requestor.responseXML;}
this.getDescriptor =function(){var td =new Web.TypeDescriptor();td.addProperty('data',String,true);td.addProperty('object',Object,true);td.addProperty('statusCode',Number,true);td.addProperty('statusText',String,true);td.addProperty('xml',Object,true);return td;}
}
Type.registerClass('Web.Net.WebResponse',null,Web.ITypeDescriptorProvider);Web.Net.WebRequest =function(){Web.Net.WebRequest.initializeBase(this,[true]);var _url =null;var _effectiveUrl;var _timeoutInterval =0;var _headers =null;var _body =null;var _userContext =null;var _requestor =null;var _timer =null;var _isComplete =true;var _timedOut =false;var _aborted =false;var _response;this.get_aborted =function(){return _aborted;}
this.get_isActive =function(){return !_isComplete;}
this.get_body =function(){return _body;}
this.set_body =function(value){_body =value;}
this.get_headers =function(){if (_headers ==null){_headers ={};}
return _headers;}
this.get_response =function(){if (_isComplete &&_requestor &&!_response){_response =new Web.Net.WebResponse(_requestor,_userContext);_requestor =null;}
return _response;}
Web.Net.WebRequest.registerBaseMethod(this,'get_response');this.get_timeoutInterval =function(){return _timeoutInterval;}
this.set_timeoutInterval =function(value){_timeoutInterval =value;}
this.get_timedOut =function(){return _timedOut;}
this.get_url =function(){return _url;}
this.set_url =function(value){if (!_effectiveUrl){_url =value;}
}
this.aborted =this.createEvent();this.completed =this.createEvent();this.timeout =this.createEvent();this.abort =function(){if (_timer !=null){window.clearTimeout(_timer);_timer =null;}
if (_requestor !=null){_requestor.onreadystatechange =Function.emptyMethod;_requestor.abort();if (_isComplete ==false){_aborted =true;_isComplete =true;this.aborted.invoke(this,Web.EventArgs.Empty);}
_requestor =null;}
_response =null;_userContext =null;}
Web.Net.WebRequest.registerBaseMethod(this,'abort');this.dispose =function(){if (this.completed){this.completed.dispose();this.completed =null;}
if (this.timeout){this.timeout.dispose();this.timeout =null;}
this.abort();Web.Net.WebRequest.callBaseMethod(this,'dispose');}
Web.Net.WebRequest.registerBaseMethod(this,'dispose');this.getDescriptor =function(){var td =Web.Net.WebRequest.callBaseMethod(this,'getDescriptor');td.addProperty('isActive',Boolean,true);td.addProperty('response',Web.Net.WebResponse,true);td.addProperty('url',String);td.addProperty('timeoutInterval',Number);td.addEvent('aborted',true);td.addEvent('completed',true);td.addEvent('timeout',true);td.addMethod('invoke');td.addMethod('abort');return td;}
Web.Net.WebRequest.registerBaseMethod(this,'getDescriptor');this.getEffectiveUrl =function(){return _url;}
Web.Net.WebRequest.registerBaseMethod(this,'getEffectiveUrl');this.invoke =function(userContext){if (_isComplete ==false){this.abort();}
_isComplete =false;_aborted =false;_timedOut =false;_response =null;_userContext =userContext;_requestor =new XMLHttpRequest();_requestor.onreadystatechange =Function.createCallback(onReadyStateChange,this);if (!_effectiveUrl){_effectiveUrl =this.getEffectiveUrl();}
if (_body !=null){_requestor.open('POST',_effectiveUrl,true);if ((_headers ==null)||!_headers['Content-Type']){_requestor.setRequestHeader('Content-Type','application/x-www-form-urlencoded');}
}
else {_requestor.open('GET',_effectiveUrl,true);}
if (_headers !=null){for (var header in _headers){_requestor.setRequestHeader(header,_headers[header]);}
}
if (_timeoutInterval !=0){_timer =window.setTimeout(Function.createCallback(onTimeout,this),_timeoutInterval);}
_requestor.send(_body);}
Web.Net.WebRequest.registerBaseMethod(this,'invoke');function onReadyStateChange(context){if (_requestor.readyState ==4){if (_timer !=null){window.clearTimeout(_timer);_timer =null;}
_isComplete =true;_requestor.onreadystatechange =Function.emptyMethod;context.completed.invoke(context,Web.EventArgs.Empty);_requestor =null;_userContext =null;}
}
function onTimeout(context){if (_isComplete ==false){if (_timer !=null){window.clearTimeout(_timer);_timer =null;}
_timedOut =true;_isComplete =true;_requestor.onreadystatechange =Function.emptyMethod;_requestor.abort();context.timeout.invoke(context,Web.EventArgs.Empty);_requestor =null;_userContext =null;}
}
}
Type.registerClass('Web.Net.WebRequest',Web.Component);Web.Net.WebRequest.createQueryString =function(queryString){var sb =new Web.StringBuilder();var i =0;for (var arg in queryString){if (i !=0){sb.append('&');}
sb.append(arg);sb.append('=');sb.append(encodeURIComponent(queryString[arg]));i++;}
return sb.toString();}
Web.Net.WebRequest.createUrl =function(url,queryString){if (!queryString){return url;}
return url +'?'+Web.Net.WebRequest.createQueryString(queryString);}
Web.Net.MethodRequest =function(){Web.Net.MethodRequest.initializeBase(this);var _methodName =null;var _parameters =null;this.get_methodName =function(){return _methodName;}
this.set_methodName =function(value){_methodName =value;}
this.get_parameters =function(){if (_parameters ==null){_parameters ={};}
return _parameters;}
this.getDescriptor =function(){var td =Web.Net.MethodRequest.callBaseMethod(this,'getDescriptor');td.addProperty('methodName',String);td.addProperty('parameters',Object,true);return td;}
Web.Net.MethodRequest.registerBaseMethod(this,'getDescriptor');}
Type.registerAbstractClass('Web.Net.MethodRequest',Web.Net.WebRequest);Web.Net.MethodRequest.callMethod =function(request,methodName,params,onMethodComplete,onMethodTimeout,onMethodError,userContext){function onMethodCompleteInternal(sender,eventArgs){var response =sender.get_response();var statusCode =response.get_statusCode();var result =null;try {result =response.get_object();}
catch (ex){}
if (((statusCode <200)||(statusCode >=300))||Web.Net.MethodRequestError.isInstanceOfType(result)){if (onMethodError){onMethodError(result,response,userContext);}
}
else if (onMethodComplete){onMethodComplete(result,response,userContext);}
}
function onMethodTimeoutInternal(sender,eventArgs){onMethodTimeout(userContext);}
function onMethodErrorInternal(sender,eventArgs){onMethodError(sender.get_response(),userContext);}
request.set_methodName(methodName);if (onMethodComplete ||onMethodError){request.completed.add(onMethodCompleteInternal);}
if (onMethodTimeout){request.timeout.add(onMethodTimeoutInternal);}
if (params){var requestParams =request.get_parameters();for (var param in params){requestParams[param]=params[param];}
}
request.invoke(userContext);return request;}
Web.Net.MethodRequestError =function(message,stackTrace,exceptionType){var _message =message;var _stackTrace =stackTrace;var _exceptionType =exceptionType;this.get_message =function(){return _message;}
this.get_stackTrace =function(){return _stackTrace;}
this.get_exceptionType =function(){return _exceptionType;}
}
Type.registerClass('Web.Net.MethodRequestError',Web.Net.MethodRequestError);Web.Net.ServiceMethodRequest =function(){Web.Net.ServiceMethodRequest.initializeBase(this);this.getEffectiveUrl =function(){return Web.Net.WebRequest.createUrl(this.get_url(),{mn :this.get_methodName()} );}
this.invoke =function(userContext){this.set_body(Web.Serialization.JSON.serialize(this.get_parameters()));Web.Net.ServiceMethodRequest.callBaseMethod(this,'invoke',[userContext]);}
}
Type.registerClass('Web.Net.ServiceMethodRequest',Web.Net.MethodRequest);Web.Net.ServiceMethodRequest.callMethod =function(methodURL,methodName,params,onMethodComplete,onMethodTimeout,onMethodError,userContext){var request =new Web.Net.ServiceMethodRequest();request.set_url(methodURL);return Web.Net.MethodRequest.callMethod(request,methodName,params,onMethodComplete,onMethodTimeout,onMethodError,userContext);}
Web.Net.PageMethodRequest =function(){Web.Net.PageMethodRequest.initializeBase(this);this.getEffectiveUrl =function(){var form =document.forms[0];return form.action;}
this.invoke =function(userContext){var form =document.forms[0];var bodyDictionary =new Object();bodyDictionary["__serviceMethodName"]=this.get_methodName();bodyDictionary["__serviceMethodParams"]=Web.Serialization.JSON.serialize(this.get_parameters());var count =form.elements.length;var element;for (var i =0;i <count;i++){element =form.elements[i];var tagName =element.tagName.toLowerCase();if (tagName =="input"){var type =element.type;if ((type =="text"||type =="hidden"||type =="password"||((type =="checkbox"||type =="radio")&&element.checked))){bodyDictionary[element.name]=element.value;}
}
else if (tagName =="select"){var selectCount =element.options.length;for (var j =0;j <selectCount;j++){var selectChild =element.options[j];if (selectChild.selected ==true){bodyDictionary[element.name]=element.value;}
}
}
else if (tagName =="textarea"){bodyDictionary[element.name]=element.value;}
}
this.set_body(Web.Net.WebRequest.createQueryString(bodyDictionary));Web.Net.PageMethodRequest.callBaseMethod(this,'invoke',[userContext]);}
}
Type.registerClass('Web.Net.PageMethodRequest',Web.Net.MethodRequest);Web.Net.PageMethodRequest.callMethod =function(methodName,params,onMethodComplete,onMethodTimeout,onMethodError,userContext){var request =new Web.Net.PageMethodRequest();return Web.Net.MethodRequest.callMethod(request,methodName,params,onMethodComplete,onMethodTimeout,onMethodError,userContext);}
Type.registerNamespace('Web.Services');Web.Services.AuthenticationService =new function(){var _path ="ScriptServices/Microsoft/Web/Services/AuthenticationService.asmx";this.login =function(userName,password,onMethodComplete,onMethodTimeout,onMethodError){return Web.Net.ServiceMethodRequest.callMethod(_path,"Login",{userName:userName,password:password},onMethodComplete,onMethodTimeout,onMethodError);}
this.validateUser =function(userName,password,onMethodComplete,onMethodTimeout,onMethodError){return Web.Net.ServiceMethodRequest.callMethod(_path,"ValidateUser",{userName:userName,password:password},onMethodComplete,onMethodTimeout,onMethodError);}
}
Web.Services.ProfileService =new function(){var _path ="ScriptServices/Microsoft/Web/Services/ProfileService.asmx";this.getProfile =function(onMethodComplete,onMethodTimeout,onMethodError){return Web.Net.ServiceMethodRequest.callMethod(_path,"GetProfile",{},onMethodComplete,onMethodTimeout,onMethodError);}
this.getProfileProperties =function(properties,onMethodComplete,onMethodTimeout,onMethodError){return Web.Net.ServiceMethodRequest.callMethod(_path,"GetProfileProperties",properties,onMethodComplete,onMethodTimeout,onMethodError);}
this.setProfile =function(properties,onMethodComplete,onMethodTimeout,onMethodError){var o =new Object();o["dictionary"]=properties;return Web.Net.ServiceMethodRequest.callMethod(_path,"SetProfile",o,onMethodComplete,onMethodTimeout,onMethodError);}
}
Web._Profile =function(){function onProfileLoad(result){for (var propertyName in result){var propertyValue =result[propertyName];if (!Web.Profile[propertyName]){Web.Profile[propertyName]=propertyValue;}
}
}
this.refresh =function(){Web.Services.ProfileService.getProfile(onProfileLoad);}
this.update =function(){var o =new Object();for (var propertyName in this){if (typeof(this[propertyName])!='function'){o[propertyName]=this[propertyName];}
}
Web.Services.ProfileService.setProfile(o);}
}
Type.registerSealedClass('Web._Profile');Web.Profile =new Web._Profile();window.XMLDOM =function(markup){if (Web.Application.get_type()==Web.ApplicationType.InternetExplorer){var progIDs =['Msxml2.DOMDocument.6.0','Msxml2.DOMDocument.5.0','Msxml2.DOMDocument.4.0','Msxml2.DOMDocument.3.0','Msxml2.DOMDocument'];for (var i =0;i <progIDs.length;i++){try {var xmlDOM =new ActiveXObject(progIDs[i]);xmlDOM.async =false;xmlDOM.loadXML(markup);return xmlDOM;}
catch (ex){}
}
return null;}
else {var domParser =new DOMParser();return domParser.parseFromString(markup,'text/xml');}
}
function _loadIECompatLayer(w){if (!window.XMLHttpRequest){window.XMLHttpRequest =function(){var progIDs =['Msxml2.XMLHTTP','Microsoft.XMLHTTP'];for (var i =0;i <progIDs.length;i++){try {var xmlHttp =new ActiveXObject(progIDs[i]);return xmlHttp;}
catch (ex){}
}
return null;}
}
if (!Debug.breakIntoDebugger){Debug.breakIntoDebugger =function(message){Debug.writeln(message);eval('debugger;');}
}
}
_loadIECompatLayer(window);