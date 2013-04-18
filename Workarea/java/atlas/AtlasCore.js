//-----------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------
// AtlasCore.js
// Atlas Core Framework.
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
Web.Attributes =new function(){this.defineAttribute =function(attributeName){this[attributeName]=attributeName;}
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
Type.registerSealedClass('Web.StringBuilder');Web.Attributes.defineAttribute('Element');Web.TypeDescriptor =function(){var _properties ={};var _events ={};var _methods ={};var _attributes ={};this._getAttributes =function(){return _attributes;}
this._getEvents =function(){return _events;}
this._getMethods =function(){return _methods;}
this._getProperties =function(){return _properties;}
}
Web.TypeDescriptor.prototype.addAttribute =function(attributeName,attributeValue){this._getAttributes()[attributeName]=attributeValue;}
Web.TypeDescriptor.prototype.addEvent =function(eventName,supportsActions){this._getEvents()[eventName]={name:eventName,actions:supportsActions };}
Web.TypeDescriptor.prototype.addMethod =function(methodName,associatedParameters){this._getMethods()[methodName]={name:methodName,parameters:associatedParameters };}
Web.TypeDescriptor.prototype.addProperty =function(propertyName,propertyType,readOnly){if (!readOnly){readOnly =false;}
var associatedAttributes;if (arguments.length >3){associatedAttributes ={};for (var i =3;i <arguments.length;i +=2){var attribute =arguments[i];var value =arguments[i +1];associatedAttributes[attribute]=value;}
}
this._getProperties()[propertyName]={name:propertyName,type:propertyType,isReadOnly:readOnly,attributes:associatedAttributes };}
Web.TypeDescriptor.addType =function(tagPrefix,tagName,type){if (!Web.TypeDescriptor._registeredTags){Web.TypeDescriptor._registeredTags ={};}
var tagTable =Web.TypeDescriptor._registeredTags[tagPrefix];if (!tagTable){tagTable ={};Web.TypeDescriptor._registeredTags[tagPrefix]=tagTable;}
tagTable[tagName]=type;}
Web.TypeDescriptor.createParameter =function(parameterName,parameterType){return {name:parameterName,type:parameterType };}
Web.TypeDescriptor.getType =function(tagPrefix,tagName){if (Web.TypeDescriptor._registeredTags){var tagNameTable =Web.TypeDescriptor._registeredTags[tagPrefix];if (tagNameTable){return tagNameTable[tagName];}
}
return null;}
Web.TypeDescriptor.getTypeDescriptor =function(instance){var type =Object.getType(instance);var td =type._descriptor;if (!td){td =instance.getDescriptor();type._descriptor =td;}
return td;}
Web.TypeDescriptor.initializeInstance =function(instance,node,markupContext){var td =Web.TypeDescriptor.getTypeDescriptor(instance);if (!td){return null;}
var supportsBatchedUpdates =false;if (Web.ISupportBatchedUpdates.isImplementedBy(instance)){supportsBatchedUpdates =true;instance.beginUpdate();}
var i,a;var attr,attrName;var propertyInfo,propertyName,propertyType,propertyValue;var eventInfo,eventValue;var setter,getter;var properties =td._getProperties();var events =td._getEvents();var attributes =node.attributes;if (attributes){for (a =attributes.length -1;a >=0;a--){attr =attributes[a];attrName =attr.nodeName;propertyInfo =properties[attrName];if (propertyInfo){propertyType =propertyInfo.type;propertyValue =attr.nodeValue;var delayedSet =false;if (propertyType ==Object){var elementsOnly =(propertyInfo.attributes &&propertyInfo.attributes[Web.Attributes.Element]);if (!elementsOnly){markupContext.addReference(instance,propertyInfo,propertyValue);delayedSet =true;}
else {propertyValue =markupContext.findObject(propertyValue,elementsOnly);}
}
if (!delayedSet){propertyName =propertyInfo.name;setter =instance['set_'+propertyName];if (propertyType !=Object){if (propertyType ==Array){propertyValue =Array.parse('['+propertyValue +']');}
else if (propertyType !=String){propertyValue =propertyType.parse(propertyValue);}
}
setter.call(instance,propertyValue);}
}
else {eventInfo =events[attrName];if (eventInfo){var handler =Function.parse(attr.nodeValue);if (handler){eventValue =instance[eventInfo.name];eventValue.add(handler);}
}
}
}
}
var childNodes =node.childNodes;if (childNodes &&(childNodes.length !=0)){for (i =childNodes.length -1;i >=0;i--){var childNode =childNodes[i];var nodeName =childNode.baseName;propertyInfo =properties[nodeName];if (propertyInfo){propertyName =propertyInfo.name;propertyType =propertyInfo.type;if (propertyInfo.isReadOnly){getter =instance['get_'+propertyName];var nestedObject =getter.call(instance);if (propertyType ==Array){if (childNode.childNodes.length !=0){var items =Web.TypeDescriptor.processMarkupNodes(childNode.childNodes,markupContext);for (var itemIndex =0;itemIndex <items.length;itemIndex++){nestedObject.add(items[itemIndex]);}
}
}
else if (propertyType ==Object){attributes =childNode.attributes;for (a =attributes.length -1;a >=0;a--){attr =attributes[a];nestedObject[attr.nodeName]=attr.nodeValue;}
}
else {Web.TypeDescriptor.initializeInstance(nestedObject,childNode,markupContext);}
}
else {propertyValue =null;if (propertyType ==String){propertyValue =childNode.text;}
else if (childNode.childNodes.length !=0){var valueNode;for (var childNodeIndex =0;childNodeIndex <childNode.childNodes.length;childNodeIndex++){if (childNode.childNodes[childNodeIndex].nodeType !=1){continue;}
valueNode =childNode.childNodes[childNodeIndex];break;}
if (valueNode){propertyValue =Web.TypeDescriptor.processMarkupNode(valueNode,markupContext);}
}
if (propertyValue){setter =instance['set_'+propertyName];setter.call(instance,propertyValue);}
}
}
else {eventInfo =events[nodeName];if (eventInfo &&eventInfo.actions){var actions =Web.TypeDescriptor.processMarkupNodes(childNode.childNodes,markupContext);if (actions.length){eventValue =instance[eventInfo.name];for (var e =0;e <actions.length;e++){eventValue.addAction(actions[e]);}
}
}
}
}
}
if (supportsBatchedUpdates){markupContext.addEndUpdate(instance);}
return instance;}
Web.TypeDescriptor.processMarkupNode =function(node,markupContext){var parsedObject =null;var tagPrefix =node.prefix;if (!tagPrefix ||(tagPrefix.length ==0)){tagPrefix ='script';}
var tagName =node.baseName;var tagType =Web.TypeDescriptor.getType(tagPrefix,tagName);if (tagType){var parseMethod =tagType.parseFromMarkup;if (!parseMethod){var baseType =tagType.getBaseType();while (baseType){parseMethod =baseType.parseFromMarkup;if (parseMethod){break;}
baseType =baseType.getBaseType();}
tagType.parseFromMarkup =parseMethod;}
if (parseMethod){parsedObject =parseMethod.call(null,tagType,node,markupContext);}
}
return parsedObject;}
Web.TypeDescriptor.processMarkupNodes =function(nodes,markupContext){var objects =[];for (var i =0;i <nodes.length;i++){var objectNode =nodes[i];if (objectNode.nodeType !=1){continue;}
var processedObject =this.processMarkupNode(objectNode,markupContext);if (processedObject){objects.add(processedObject);}
}
return objects;}
Web.TypeDescriptor.unload =function(){if (Web.TypeDescriptor._registeredTags){Web.TypeDescriptor._registeredTags =null;}
}
Web.TypeDescriptor.getAttribute =function(instance,attributeName){var td =Web.TypeDescriptor.getTypeDescriptor(instance);if (!td){return null;}
return td._getAttributes()[attributeName];}
Web.TypeDescriptor.getProperty =function(instance,propertyName,key){if (Web.ICustomTypeDescriptor.isImplementedBy(instance)){return instance.getProperty(propertyName,key);}
if ((propertyName ==null)||(propertyName.length ==0)){return instance;}
var td =Web.TypeDescriptor.getTypeDescriptor(instance);if (!td){return null;}
var propertyInfo =td._getProperties()[propertyName];if (!propertyInfo){return null;}
var getter =instance['get_'+propertyInfo.name];var object =getter.call(instance);if ((propertyInfo.type ==Object)&&propertyInfo.isReadOnly &&key){object =object[key];}
return object;}
Web.TypeDescriptor.setProperty =function(instance,propertyName,value,key){if (Web.ICustomTypeDescriptor.isImplementedBy(instance)){instance.setProperty(propertyName,value,key);return;}
var td =Web.TypeDescriptor.getTypeDescriptor(instance);if (td){var propertyInfo =td._getProperties()[propertyName];if (propertyInfo){if (!propertyInfo.isReadOnly){if ((propertyInfo.type !=String)&&(typeof(value)=='string')&&propertyInfo.type.parse){value =propertyInfo.type.parse(value);}
var setter =instance['set_'+propertyInfo.name];setter.call(instance,value);}
else if ((propertyInfo.type ==Object)&&key){var getter =instance['get_'+propertyInfo.name];var object =getter.call(instance);object[key]=value;}
}
}
}
Web.TypeDescriptor.invokeMethod =function(instance,methodName,parameters){if (Web.ICustomTypeDescriptor.isImplementedBy(instance)){return instance.invokeMethod(methodName,parameters);}
var td =Web.TypeDescriptor.getTypeDescriptor(instance);if (td){var methodInfo =td._getMethods()[methodName];if (methodInfo){var method =instance[methodInfo.name];if (!methodInfo.parameters ||!methodInfo.parameters.length){return method.call(instance);}
else {if (!parameters){return null;}
var arguments =[];for (var i =0;i <methodInfo.parameters.length;i++){var parameterInfo =methodInfo.parameters[i];var value =parameters[parameterInfo.name];if (value &&(parameterInfo.type !=String)&&(typeof(value)=='string')){value =parameterInfo.type.parse(value);}
arguments[i]=value;}
return method.apply(instance,arguments);}
}
}
return null;}
Web.TypeDescriptor.getPropertyType =function(instance,propertyName,key){if (Web.ICustomTypeDescriptor.isImplementedBy(instance)){return Object;}
if (key){return Object;}
var td =Web.TypeDescriptor.getTypeDescriptor(instance);if (!td){return null;}
var propertyInfo =td._getProperties()[propertyName];if (!propertyInfo){return null;}
return propertyInfo.type;}
Web.TypeDescriptor.MarkupContext =function(document,global,parentContext,dataContext){var _document =document;var _global =global;var _parentContext =parentContext;var _dataContext =dataContext;var _dataContextHidden;var _objects ={};var _pendingReferences =[];var _pendingEndUpdates =[];this.get_dataContext =function(){if (_dataContextHidden){return null;}
return _dataContext;}
this.get_document =function(){return _document;}
this.get_isGlobal =function(){return _global;}
this.addObject =function(id,object){_objects[id]=object;}
this.addEndUpdate =function(instance){_pendingEndUpdates.add(instance);}
this.addReference =function(instance,propertyInfo,reference){_pendingReferences.add({o:instance,p:propertyInfo,r:reference });}
this.complete =function(){_document =null;_dataContext =null;var i;for (i =0;i <_pendingReferences.length;i++){var pendingReference =_pendingReferences[i];var instance =pendingReference.o;var propertyInfo =pendingReference.p;var propertyValue =pendingReference.r;var object =this.findObject(propertyValue,false);if (object){var setter =instance['set_'+propertyInfo.name];setter.call(instance,object);}
}
_pendingReferences =null;for (i =0;i <_pendingEndUpdates.length;i++){_pendingEndUpdates[i].endUpdate();}
_pendingEndUpdates =null;}
this.dispose =function(){for (var o in _objects){if (Web.IDisposable.isImplementedBy(_objects[o])){_objects[o].dispose();}
_objects[o]=null;}
_document =null;_objects =null;_parentContext =null;_dataContext =null;}
this.findObject =function(id,elementsOnly){var object;if (elementsOnly){if (_document){object =_document.getElementById(id);}
if (!object &&_parentContext){object =_parentContext.findObject(id,elementsOnly);}
return object;}
object =_objects[id];if (!object &&_document){object =_document.getElementById(id);}
if (!object &&_parentContext){object =_parentContext.findObject(id,elementsOnly);}
return object;}
this.hideDataContext =function(){if (!_dataContextHidden){_dataContextHidden =true;return true;}
return false;}
this.restoreDataContext =function(){_dataContextHidden =false;}
}
Web.ISupportBatchedUpdates =function(){this.beginUpdate =Function.abstractMethod;this.endUpdate =Function.abstractMethod;}
Type.registerInterface('Web.ISupportBatchedUpdates');Web.ICustomTypeDescriptor =function(){this.getProperty =Function.abstractMethod;this.setProperty =Function.abstractMethod;this.invokeMethod =Function.abstractMethod;}
Type.registerInterface('Web.ICustomTypeDescriptor');Web.ITypeDescriptorProvider =function(){this.getDescriptor =Function.abstractMethod;}
Type.registerInterface('Web.ITypeDescriptorProvider');Web.ActionSequence =Web.Enum.create('BeforeEventHandler','AfterEventHandler');Web.IAction =function(){this.get_sequence =Function.abstractMethod;this.execute =Function.abstractMethod;this.setOwner =Function.abstractMethod;}
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
this.getDescriptor =function(){var td =new Web.TypeDescriptor();td.addEvent('load',true);td.addEvent('unload',true);return td;}
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
function loadObjects(){_markupContext =new Web.TypeDescriptor.MarkupContext(document,true);if (_scriptObjectsNode){Web.TypeDescriptor.processMarkupNodes(_scriptObjectsNode.childNodes,_markupContext);_scriptObjectsNode =null;}
_markupContext.complete();onLoad();}
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
if (_markupContext){_markupContext.dispose();_markupContext =null;}
Web.TypeDescriptor.unload();}
}
Type.registerSealedClass('Web._Application',null,Web.IDisposable,Web.ITypeDescriptorProvider,Web.ICustomTypeDescriptor);Web.TypeDescriptor.addType('script','application',Web._Application);Web._Application.parseFromMarkup =function(type,node,markupContext){if (!markupContext.get_isGlobal()){return null;}
Web.TypeDescriptor.initializeInstance(Web.Application,node,markupContext);return Web.Application;}
Web.Application =new Web._Application();Web.INotifyPropertyChanged =function(){this.propertyChanged =null;}
Type.registerInterface('Web.INotifyPropertyChanged');Web.INotifyCollectionChanged =function(){this.collectionChanged =null;}
Type.registerInterface('Web.INotifyCollectionChanged');Web.PropertyChangedEventArgs =function(propertyName){Web.PropertyChangedEventArgs.initializeBase(this);var _propertyName =propertyName;this.get_propertyName =function(){return _propertyName;}
this.getDescriptor =function(){var td =Web.PropertyChangedEventArgs.callBaseMethod(this,'getDescriptor');td.addProperty('propertyName',String,true);return td;}
}
Type.registerSealedClass('Web.PropertyChangedEventArgs',Web.EventArgs);Web.NotifyCollectionChangedAction =Web.Enum.create('Add','Remove','Update','Reset');Web.CollectionChangedEventArgs =function(action,changedItem){Web.CollectionChangedEventArgs.initializeBase(this);var _action =action;var _changedItem =changedItem;this.get_action =function(){return _action;}
this.get_changedItem =function(){return _changedItem;}
this.getDescriptor =function(){var td =Web.CollectionChangedEventArgs.callBaseMethod(this,'getDescriptor');td.addProperty('action',Web.NotifyCollectionChangedAction,true);td.addProperty('changedItem',Object,true);return td;}
}
Type.registerSealedClass('Web.CollectionChangedEventArgs',Web.EventArgs);Web.BindingDirection =Web.Enum.create('In','Out','InOut');Web.BindingEventArgs =function(value,direction,targetPropertyType,transformerArgument){Web.BindingEventArgs.initializeBase(this);var _value =value;var _direction =direction;var _targetPropertyType =targetPropertyType;var _transformerArgument =transformerArgument;this.get_direction =function(){return _direction;}
this.get_targetPropertyType =function(){return _targetPropertyType;}
this.get_transformerArgument =function(){return _transformerArgument;}
this.get_value =function(){return _value;}
this.set_value =function(value){_value =value;}
this.getDescriptor =function(){var td =Web.BindingEventArgs.callBaseMethod(this,'getDescriptor');td.addProperty('direction',Web.BindingDirection,true);td.addProperty('targetPropertyType',Object,true);td.addProperty('transformerArgument',Object,true);td.addProperty('value',Object);return td;}
Web.BindingEventArgs.registerBaseMethod(this,'getDescriptor');}
Type.registerSealedClass('Web.BindingEventArgs',Web.CancelEventArgs);Web.Binding =function(){Web.Binding.initializeBase(this);var _id;var _component;var _componentNotificationHandler;var _dataContext;var _dataContextInUse;var _dataContextNotificationHandler;var _dataPath;var _dataPathParts;var _direction =Web.BindingDirection.In;var _property;var _propertyKey;var _automatic =true;var _transformerArgument;this.get_automatic =function(){return _automatic;}
this.set_automatic =function(value){if (!_dataContextInUse){_automatic =value;}
}
this.get_dataContext =function(){return _dataContext;}
this.set_dataContext =function(value){if (!_dataContextInUse){_dataContext =value;}
}
this.get_dataPath =function(){return _dataPath;}
this.set_dataPath =function(value){if (!_dataContextInUse){_dataPath =value;}
}
this.get_direction =function(){return _direction;}
this.set_direction =function(value){if (!_dataContextInUse){_direction =value;}
}
this.get_id =function(){return _id;}
this.set_id =function(value){_id =value;}
this.get_property =function(){return _property;}
this.set_property =function(value){if (!_dataContextInUse){_property =value;}
}
this.get_propertyKey =function(){return _propertyKey;}
this.set_propertyKey =function(value){if (!_dataContextInUse){_propertyKey =value;}
}
this.get_transformerArgument =function(){return _transformerArgument;}
this.set_transformerArgument =function(value){_transformerArgument =value;}
this.transform =new Web.Event(null);this.dispose =function(){this.transform.dispose();if (_componentNotificationHandler){_component.propertyChanged.remove(_componentNotificationHandler);_componentNotificationHandler =null;}
if (_dataContextNotificationHandler){if (Web.INotifyPropertyChanged.isImplementedBy(_dataContextInUse)){_dataContextInUse.propertyChanged.remove(_dataContextNotificationHandler);}
_dataContextNotificationHandler =null;}
_dataContextInUse =null;_dataContext =null;_component =null;}
this.evaluate =function(direction){if (direction ==Web.BindingDirection.In){this.evaluateIn();}
else {this.evaluateOut();}
}
this.evaluateDataPath =function(){var object =_dataContextInUse;for (var i =0;i <_dataPathParts.length -1;i++){object =Web.TypeDescriptor.getProperty(object,_dataPathParts[i]);if (!object){return null;}
}
return object;}
this.evaluateIn =function(){if (!_dataContextInUse){return;}
if ((_direction ==Web.BindingDirection.In)||(_direction ==Web.BindingDirection.InOut)){var propertyObject =_dataContextInUse;var propertyName =_dataPath;if (_dataPathParts){propertyObject =this.evaluateDataPath();if (propertyObject ==null){return;}
propertyName =_dataPathParts[_dataPathParts.length -1];}
var value =Web.TypeDescriptor.getProperty(propertyObject,propertyName);var canceled =false;if (this.transform.isActive()){var targetPropertyType =Web.TypeDescriptor.getPropertyType(_component,_property,_propertyKey);var be =new Web.BindingEventArgs(value,Web.BindingDirection.In,targetPropertyType,_transformerArgument);this.transform.invoke(this,be);canceled =be.get_canceled();value =be.get_value();}
if (!canceled){Web.TypeDescriptor.setProperty(_component,_property,value,_propertyKey);}
}
}
this.evaluateOut =function(){if (!_dataContextInUse){return;}
if ((_direction ==Web.BindingDirection.InOut)||(_direction ==Web.BindingDirection.Out)){var value =Web.TypeDescriptor.getProperty(_component,_property,_propertyKey);var canceled =false;var propertyObject =_dataContextInUse;var propertyName =_dataPath;if (_dataPathParts){propertyObject =this.evaluateDataPath();propertyName =_dataPathParts[_dataPathParts.length -1];}
if (this.transform.isActive()){var targetPropertyType =Web.TypeDescriptor.getPropertyType(propertyObject,propertyName);var be =new Web.BindingEventArgs(value,Web.BindingDirection.Out,targetPropertyType,_transformerArgument);this.transform.invoke(this,be);canceled =be.get_canceled();value =be.get_value();}
if (!canceled){Web.TypeDescriptor.setProperty(propertyObject,propertyName,value);}
}
}
this.getDescriptor =function(){var td =new Web.TypeDescriptor();td.addProperty('automatic',Boolean);td.addProperty('dataContext',Object);td.addProperty('dataPath',String);td.addProperty('direction',Web.BindingDirection);td.addProperty('id',String);td.addProperty('property',String);td.addProperty('propertyKey',String);td.addProperty('transformerArgument',String);td.addEvent('transform',false);td.addMethod('evaluateIn');td.addMethod('evaluateOut');return td;}
this.initialize =function(component){_component =component;_dataContextInUse =_dataContext;if (!_dataContextInUse){_dataContextInUse =_component.get_dataContext();}
if (_dataPath &&_dataPath.indexOf('.')>0){_dataPathParts =_dataPath.split('.');}
if (_automatic){if (Web.INotifyPropertyChanged.isImplementedBy(_component)){_componentNotificationHandler =Function.createDelegate(this,this._onComponentPropertyChanged);_component.propertyChanged.add(_componentNotificationHandler);}
if (_dataContextInUse &&Web.INotifyPropertyChanged.isImplementedBy(_dataContextInUse)){_dataContextNotificationHandler =Function.createDelegate(this,this._onDataContextPropertyChanged);_dataContextInUse.propertyChanged.add(_dataContextNotificationHandler);}
this.evaluate(Web.BindingDirection.In);}
}
this._onComponentPropertyChanged =function(sender,eventArgs){if (eventArgs.get_propertyName()==_property){this.evaluate(Web.BindingDirection.Out);}
}
this._onDataContextPropertyChanged =function(sender,eventArgs){var compareProperty =_dataPath;if (_dataPathParts){compareProperty =_dataPathParts[0];}
if (eventArgs.get_propertyName()==compareProperty){this.evaluate(Web.BindingDirection.In);}
}
}
Type.registerSealedClass('Web.Binding',null,Web.IDisposable,Web.ITypeDescriptorProvider);Web.TypeDescriptor.addType('script','binding',Web.Binding);Web.Binding.parseFromMarkup =function(type,node,markupContext){var newBinding =new Web.Binding();var builtInTransform;var transformAttribute =node.attributes.getNamedItem('transform');if (transformAttribute){var transformValue =transformAttribute.nodeValue;builtInTransform =Web.Binding.Transformers[transformValue];}
if (builtInTransform){newBinding.transform.add(builtInTransform);node.attributes.removeNamedItem('transform');}
var binding =Web.TypeDescriptor.initializeInstance(newBinding,node,markupContext);if (builtInTransform){node.attributes.setNamedItem(transformAttribute)}
if (binding){if (binding.get_id()){markupContext.addObject(binding.get_id(),binding);}
return binding;}
else {newBinding.dispose();}
return null;}
Web.Binding.Transformers ={};Web.Binding.Transformers.Invert =function(sender,eventArgs){eventArgs.set_value(!eventArgs.get_value());}
Web.Binding.Transformers.ToString =function(sender,eventArgs){var value =eventArgs.get_value();var newValue ='';var formatString =eventArgs.get_transformerArgument();var placeHolder =(formatString &&(formatString.length !=0))?formatString.indexOf('{0}'):-1;if (eventArgs.get_direction()==Web.BindingDirection.In){if (placeHolder !=-1){newValue =String.format(formatString,value);}
else if (value){newValue =value.toString();}
else {newValue =formatString;}
}
else {if (placeHolder !=-1){value =value.substr(placeHolder,3 +value.length -formatString.length);}
else if (formatString ==value){value =null;}
var propertyType =eventArgs.get_targetPropertyType();if (!value){if (propertyType ==Boolean){newValue =false;}
else if (propertyType ==Number){newValue =0;}
else if (propertyType ==String){newValue ='';}
else {newValue =null;}
}
else {newValue =propertyType.parse(value);}
}
eventArgs.set_value(newValue);}
Web.Binding.Transformers.Add =function(sender,eventArgs){var value =eventArgs.get_value();if (typeof(value)!='number'){value =Number.parse(value);}
var delta =eventArgs.get_transformerArgument();if (!delta){delta =1;}
if (typeof(delta)!='number'){delta =Number.parse(delta);}
if (eventArgs.get_direction()==Web.BindingDirection.Out){delta =-delta;}
var newValue =value +delta;if (eventArgs.get_targetPropertyType()!='number'){newValue =newValue.toString();}
eventArgs.set_value(newValue);}
Web.Binding.Transformers.Multiply =function(sender,eventArgs){var value =eventArgs.get_value();if (typeof(value)!='number'){value =Number.parse(value);}
var factor =eventArgs.get_transformerArgument();if (!factor){factor =1;}
if (typeof(factor)!='number'){factor =Number.parse(factor);}
var newValue;if (eventArgs.get_direction()==Web.BindingDirection.Out){newValue =value /factor;}
else {newValue =value *factor;}
if (eventArgs.get_targetPropertyType()!='number'){newValue =newValue.toString();}
eventArgs.set_value(newValue);}
Web.Component =function(registerAsDisposable){var _id =null;var _initialized =false;var _updating =false;var _bindings =null;var _dataContext =null;var _events =[];if (registerAsDisposable){Web.Application.registerDisposableObject(this);}
this.get_bindings =function(){if (!_bindings){_bindings =[];}
return _bindings;}
this.get_dataContext =function(){return _dataContext;}
Web.Component.registerBaseMethod(this,'get_dataContext');this.set_dataContext =function(value){_dataContext =value;}
this.get_id =function(){return _id;}
this.set_id =function(value){_id =value;}
this.get_isInitialized =function(){return _initialized;}
this.get_isUpdating =function(){return _updating;}
this.createEvent =function(autoInvoke){var eventObject =new Web.Event(this,autoInvoke);_events.add(eventObject);return eventObject;}
this.propertyChanged =this.createEvent();this.beginUpdate =function(){_updating =true;}
this.dispose =function(){if (_bindings){for (var i =0;i <_bindings.length;i++){_bindings[i].dispose();_bindings[i]=null;}
_bindings =null;}
if (_events){for (var e =_events.length -1;e >=0;e--){_events[e].dispose();_events[e]=null;}
_events =null;}
Web.Application.unregisterDisposableObject(this);}
Web.Component.registerBaseMethod(this,'dispose');this.endUpdate =function(){_updating =false;if (!_initialized){this.initialize();}
this.updated();}
this.getDescriptor =function(){var td =new Web.TypeDescriptor();td.addProperty('bindings',Array,true);td.addProperty('dataContext',Object);td.addProperty('id',String);td.addEvent('propertyChanged',true);return td;}
Web.Component.registerBaseMethod(this,'getDescriptor');this.initialize =function(){if (_bindings){for (var i =0;i <_bindings.length;i++){_bindings[i].initialize(this);}
}
_initialized =true;}
Web.Component.registerBaseMethod(this,'initialize');this.raisePropertyChanged =function(propertyName){this.propertyChanged.invoke(this,new Web.PropertyChangedEventArgs(propertyName));}
Web.Component.registerBaseMethod(this,'raisePropertyChanged');this.updated =function(){}
Web.Component.registerBaseMethod(this,'updated');}
Type.registerAbstractClass('Web.Component',null,Web.IDisposable,Web.ITypeDescriptorProvider,Web.ISupportBatchedUpdates,Web.INotifyPropertyChanged);Web.Component.parseFromMarkup =function(type,node,markupContext){var newComponent =Type.createInstance(type);var dataContextHidden =false;var dataContext =markupContext.get_dataContext();if (dataContext){dataContextHidden =markupContext.hideDataContext();}
var component =Web.TypeDescriptor.initializeInstance(newComponent,node,markupContext);if (component){if (component.get_id()){markupContext.addObject(component.get_id(),component);}
if (dataContext){component.set_dataContext(dataContext);}
}
else {newComponent.dispose();}
if (dataContextHidden){markupContext.restoreDataContext();}
return component;}
Web.Component.createCollection =function(owner){var collection =[];collection._owner =owner;collection._add =collection.add;collection.add =function(item){this._add(item);item.setOwner(this._owner);}
collection.dispose =function(){for (var i =this.length -1;i >=0;i--){this[i].dispose();this[i]=null;}
this.clear();}
return collection;}
Web.Reference =function(){var _component;this.get_component =function(){return _component;}
this.set_component =function(value){_component =value;}
this.dispose =function(){_component =null;}
this.getDescriptor =function(){var td =new Web.TypeDescriptor();td.addProperty('component',Object);return td;}
}
Type.registerSealedClass('Web.Reference',null,Web.ITypeDescriptorProvider,Web.IDisposable);Web.TypeDescriptor.addType('script','reference',Web.Reference);Web.Reference.parseFromMarkup =function(type,node,markupContext){var newReference =new Web.Reference();var reference =Web.TypeDescriptor.initializeInstance(newReference,node,markupContext);if (reference){return reference;}
else {newReference.dispose();}
return null;}
Web.Action =function(){Web.Action.initializeBase(this,[false]);var _eventSource;var _eventArgs;var _result;var _target;var _sequence =Web.ActionSequence.BeforeEventHandler;this.get_dataContext =function(){return this;}
this.get_eventArgs =function(){return _eventArgs;}
this.get_result =function(){return _result;}
this.get_sequence =function(){return _sequence;}
this.set_sequence =function(value){_sequence =value;}
this.get_sender =function(){return _eventSource;}
this.get_target =function(){return _target;}
this.set_target =function(value){_target =value;}
this.dispose =function(){_target =null;_eventSource =null;Web.Action.callBaseMethod(this,'dispose');}
Web.Action.registerBaseMethod(this,'dispose');this.execute =function(sender,eventArgs){_eventArgs =eventArgs;var bindings =this.get_bindings();var i;for (i =0;i <bindings.length;i++){bindings[i].evaluateIn();}
_result =this.performAction();for (i =0;i <bindings.length;i++){bindings[i].evaluateOut();}
_eventArgs =null;_result =null;}
this.getDescriptor =function(){var td =Web.Action.callBaseMethod(this,'getDescriptor');td.addProperty('eventArgs',Web.EventArgs);td.addProperty('result',Object);td.addProperty('sender',Object);td.addProperty('sequence',Web.ActionSequence);td.addProperty('target',Object);return td;}
Web.Action.registerBaseMethod(this,'getDescriptor');this.initialize =function(){var bindings =this.get_bindings();for (var i =0;i <bindings.length;i++){bindings[i].set_automatic(false);}
Web.Action.callBaseMethod(this,'initialize');}
this.setOwner =function(eventSource){_eventSource =eventSource;}
}
Type.registerAbstractClass('Web.Action',Web.Component,Web.IAction);Web.Action.parseFromMarkup =function(type,node,markupContext){var newAction =Type.createInstance(type);var action =Web.TypeDescriptor.initializeInstance(newAction,node,markupContext);if (action){if (action.get_id()){markupContext.addObject(action.get_id(),action);}
return action;}
else {newAction.dispose();}
return null;}
Web.InvokeMethodAction =function(){Web.InvokeMethodAction.initializeBase(this);var _method;var _parameters;this.get_method =function(){return _method;}
this.set_method =function(value){_method =value;}
this.get_parameters =function(){if (!_parameters){_parameters ={};}
return _parameters;}
this.getDescriptor =function(){var td =Web.InvokeMethodAction.callBaseMethod(this,'getDescriptor');td.addProperty('method',String);td.addProperty('parameters',Object,true);return td;}
this.performAction =function(){return Web.TypeDescriptor.invokeMethod(this.get_target(),_method,_parameters);}
}
Type.registerSealedClass('Web.InvokeMethodAction',Web.Action);Web.TypeDescriptor.addType('script','invokeMethod',Web.InvokeMethodAction);Web.SetPropertyAction =function(){Web.SetPropertyAction.initializeBase(this);var _property;var _propertyKey;var _value;this.get_property =function(){return _property;}
this.set_property =function(value){_property =value;}
this.get_propertyKey =function(){return _propertyKey;}
this.set_propertyKey =function(value){_propertyKey =value;}
this.get_value =function(){return _value;}
this.set_value =function(value){_value =value;}
this.getDescriptor =function(){var td =Web.SetPropertyAction.callBaseMethod(this,'getDescriptor');td.addProperty('property',String);td.addProperty('propertyKey',String);td.addProperty('value',String);return td;}
this.performAction =function(){Web.TypeDescriptor.setProperty(this.get_target(),_property,_value,_propertyKey);return null;}
}
Type.registerSealedClass('Web.SetPropertyAction',Web.Action);Web.TypeDescriptor.addType('script','setProperty',Web.SetPropertyAction);Web.Timer =function(){Web.Timer.initializeBase(this,[true]);var _interval =1000;var _enabled;var _timer =null;this.getDescriptor =function(){var td =Web.Timer.callBaseMethod(this,'getDescriptor');td.addProperty('interval',Number);td.addProperty('enabled',Boolean);td.addEvent('tick',true);return td;}
this.get_interval =function(){return _interval;}
this.set_interval =function(value){if (_interval !=value){_interval =value;this.raisePropertyChanged('interval');if (!this.get_isUpdating()&&(_timer !=null)){stopTimer();startTimer(this);}
}
}
this.get_enabled =function(){return _enabled;}
this.set_enabled =function(value){if (value !=this.get_enabled()){_enabled =value;this.raisePropertyChanged('enabled');if (!this.get_isUpdating()){if (value){startTimer(this);}
else {stopTimer();}
}
}
}
this.tick =this.createEvent();this.dispose =function(){this.set_enabled(false);stopTimer();Web.Timer.callBaseMethod(this,'dispose');}
this.updated =function(){Web.Timer.callBaseMethod(this,'updated');if (_enabled){stopTimer();startTimer(this);}
}
function _timerCallback(context){context.tick.invoke(context,Web.EventArgs.Empty);}
function startTimer(instance){_timer =window.setInterval(Function.createCallback(_timerCallback,instance),_interval);}
function stopTimer(){window.clearInterval(_timer);_timer =null;}
}
Type.registerSealedClass('Web.Timer',Web.Component);Web.TypeDescriptor.addType('script','timer',Web.Timer);Web.Counter =function(){Web.Timer.initializeBase(this,[false]);var _value =0;var _lowerBound =Number.NaN;var _upperBound =Number.NaN;this.getDescriptor =function(){var td =Web.Counter.callBaseMethod(this,'getDescriptor');td.addProperty('value',Number);td.addProperty('lowerBound',Number);td.addProperty('upperBound',Number);td.addProperty('canDecrement',Boolean);td.addProperty('canIncrement',Boolean);td.addMethod('increment');td.addMethod('decrement');return td;}
this.get_canDecrement =function(){return isNaN(_lowerBound)||(_value >_lowerBound);}
this.get_canIncrement =function(){return isNaN(_upperBound)||(_value <_upperBound);}
this.get_lowerBound =function(){return _lowerBound;}
this.set_lowerBound =function(value){if ((isNaN(value)&&isNaN(_lowerBound))||(value ==_lowerBound))return;var oldCanDecrement =this.get_canDecrement();_lowerBound =value;this.raisePropertyChanged('lowerBound');if (oldCanDecrement !=this.get_canDecrement()){this.raisePropertyChanged('canDecrement');}
}
this.get_upperBound =function(){return _upperBound;}
this.set_upperBound =function(value){if ((isNaN(value)&&isNaN(_upperBound))||(value ==_upperBound))return;var oldCanIncrement =this.get_canIncrement();_upperBound =value;this.raisePropertyChanged('upperBound');if (oldCanIncrement !=this.get_canIncrement()){this.raisePropertyChanged('canIncrement');}
}
this.get_value =function(){return _value;}
this.set_value =function(value){if ((isNaN(_lowerBound)||(value >=_lowerBound))&&(isNaN(_upperBound)||(value <=_upperBound))&&(_value !=value)){var oldCanDecrement =this.get_canDecrement();var oldCanIncrement =this.get_canIncrement();_value =value;this.raisePropertyChanged('value');if (oldCanDecrement !=this.get_canDecrement()){this.raisePropertyChanged('canDecrement');}
if (oldCanIncrement !=this.get_canIncrement()){this.raisePropertyChanged('canIncrement');}
}
}
this.decrement =function(){this.set_value(_value -1);}
this.increment =function(){this.set_value(_value +1);}
}
Type.registerSealedClass('Web.Counter',Web.Component);Web.TypeDescriptor.addType('script','counter',Web.Counter);Type.registerNamespace('Web.Serialization');Web.Serialization.JSON =new function(){function serializeWithBuilder(object,stringBuilder){var i;switch (typeof object){case 'object':if (object){if (Array.isInstanceOfType(object)){stringBuilder.append('[');for (i =0;i <object.length;++i){if (i >0){stringBuilder.append(',');}
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
if (_requestor !=null){_requestor.onreadystatechange =Function.emptyMethod;_requestor.abort();if (_isComplete ==false){_aborted =true;_isComplete =true;this.raisePropertyChanged('isActive');this.aborted.invoke(this,Web.EventArgs.Empty);}
_requestor =null;}
_response =null;_userContext =null;}
Web.Net.WebRequest.registerBaseMethod(this,'abort');this.dispose =function(){if (this.completed){this.completed.dispose();this.completed =null;}
if (this.timeout){this.timeout.dispose();this.timeout =null;}
this.abort();Web.Net.WebRequest.callBaseMethod(this,'dispose');}
Web.Net.WebRequest.registerBaseMethod(this,'dispose');this.getDescriptor =function(){var td =Web.Net.WebRequest.callBaseMethod(this,'getDescriptor');td.addProperty('isActive',Boolean,true);td.addProperty('response',Web.Net.WebResponse,true);td.addProperty('url',String);td.addProperty('timeoutInterval',Number);td.addEvent('aborted',true);td.addEvent('completed',true);td.addEvent('timeout',true);td.addMethod('invoke');td.addMethod('abort');return td;}
Web.Net.WebRequest.registerBaseMethod(this,'getDescriptor');this.getEffectiveUrl =function(){return _url;}
Web.Net.WebRequest.registerBaseMethod(this,'getEffectiveUrl');this.invoke =function(userContext){if (_isComplete ==false){this.abort();}
_isComplete =false;_aborted =false;_timedOut =false;_response =null;_userContext =userContext;this.raisePropertyChanged('isActive');_requestor =new XMLHttpRequest();_requestor.onreadystatechange =Function.createCallback(onReadyStateChange,this);if (!_effectiveUrl){_effectiveUrl =this.getEffectiveUrl();}
if (_body !=null){_requestor.open('POST',_effectiveUrl,true);if ((_headers ==null)||!_headers['Content-Type']){_requestor.setRequestHeader('Content-Type','application/x-www-form-urlencoded');}
}
else {_requestor.open('GET',_effectiveUrl,true);}
if (_headers !=null){for (var header in _headers){_requestor.setRequestHeader(header,_headers[header]);}
}
if (_timeoutInterval !=0){_timer =window.setTimeout(Function.createCallback(onTimeout,this),_timeoutInterval);}
_requestor.send(_body);}
Web.Net.WebRequest.registerBaseMethod(this,'invoke');function onReadyStateChange(context){if (_requestor.readyState ==4){if (_timer !=null){window.clearTimeout(_timer);_timer =null;}
_isComplete =true;_requestor.onreadystatechange =Function.emptyMethod;context.raisePropertyChanged('isActive');context.completed.invoke(context,Web.EventArgs.Empty);_requestor =null;_userContext =null;}
}
function onTimeout(context){if (_isComplete ==false){if (_timer !=null){window.clearTimeout(_timer);_timer =null;}
_timedOut =true;_isComplete =true;_requestor.onreadystatechange =Function.emptyMethod;_requestor.abort();context.raisePropertyChanged('isActive');context.timeout.invoke(context,Web.EventArgs.Empty);_requestor =null;_userContext =null;}
}
}
Type.registerClass('Web.Net.WebRequest',Web.Component);Web.TypeDescriptor.addType('script','webRequest',Web.Net.WebRequest);Web.Net.WebRequest.createQueryString =function(queryString){var sb =new Web.StringBuilder();var i =0;for (var arg in queryString){if (i !=0){sb.append('&');}
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
Type.registerClass('Web.Net.ServiceMethodRequest',Web.Net.MethodRequest);Web.TypeDescriptor.addType('script','serviceMethod',Web.Net.ServiceMethodRequest);Web.Net.ServiceMethodRequest.callMethod =function(methodURL,methodName,params,onMethodComplete,onMethodTimeout,onMethodError,userContext){var request =new Web.Net.ServiceMethodRequest();request.set_url(methodURL);return Web.Net.MethodRequest.callMethod(request,methodName,params,onMethodComplete,onMethodTimeout,onMethodError,userContext);}
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
Type.registerClass('Web.Net.PageMethodRequest',Web.Net.MethodRequest);Web.TypeDescriptor.addType('script','pageMethod',Web.Net.PageMethodRequest);Web.Net.PageMethodRequest.callMethod =function(methodName,params,onMethodComplete,onMethodTimeout,onMethodError,userContext){var request =new Web.Net.PageMethodRequest();return Web.Net.MethodRequest.callMethod(request,methodName,params,onMethodComplete,onMethodTimeout,onMethodError,userContext);}
Type.registerNamespace('Web.Data');Web.Data.DataColumn =function(columnName,dataType,defaultValue){var _columnName =columnName;var _dataType =dataType;var _defaultValue =defaultValue;this.get_columnName =function(){return _columnName;}
this.get_dataType =function(){return _dataType;}
this.get_defaultValue =function(){return _defaultValue;}
this.dispose =function(){_columnName =null;_dataType =null;_defaultValue =null;}
this.getDescriptor =function(){var td =new Web.TypeDescriptor();td.addProperty('columnName',String,true);td.addProperty('dataType',Web.Type,true);td.addProperty('defaultValue',Object,true);return td;}
}
Type.registerSealedClass('Web.Data.DataColumn',null,Web.ITypeDescriptorProvider,Web.IDisposable);Web.Data.DataRow =function(objectDataRow,dataTableOwner,index){var _owner =dataTableOwner;var _row =objectDataRow;var _index =index;this.get__isDirty =function(){return typeof(_row._original)=="object";}
this.get__index =function(){return _index;}
this._set__index =function(index){_index =index;}
this.get__rowObject =function(){return _row;}
this.dispose =function(){if (this.propertyChanged){this.propertyChanged.dispose();}
_row =null;_owner =null;}
this.getDescriptor =function(){var td =new Web.TypeDescriptor();td.addProperty('_isDirty',Boolean,true);td.addProperty('_index',Boolean,true);td.addProperty('_rowObject',Object,true);td.addEvent('propertyChanged',true);return td;}
this.initialize =function(){this.propertyChanged.initialize(this);}
this.propertyChanged =new Web.Event(null);this.getProperty =function(name,key){if (name =="_isDirty")return this.get__isDirty();if (name =="_index")return _index;if ((name ==null)||(name =="")||(name =="_rowObject")){return typeof(_row._rowObject)!="undefined"?_row._rowObject :_row;}
if (Web.ITypeDescriptorProvider.isImplementedBy(_row)){return Web.TypeDescriptor.getProperty(_row,name,key);}
return _row[name];}
this.setProperty =function(name,value,key){var keys =_owner ?_owner.get_keys():null;if ((keys &&keys.contains(name))||(_row[name]==value))return;var isDirty =this.get__isDirty();if (!isDirty &&_owner){var original ={};for (var columnName in _row){if ((columnName.charAt(0)!='_')&&(typeof(_row[columnName])!="function")){original[columnName]=_row[columnName];}
}
_row._original =original;}
if (Web.ITypeDescriptorProvider.isImplementedBy(_row)){Web.TypeDescriptor.setProperty(_row,name,value,key);}
else {_row[name]=value;}
this.raisePropertyChanged(name);if (!isDirty){this.raisePropertyChanged("_isDirty");}
if (_owner){_owner.raiseCollectionChanged(Web.NotifyCollectionChangedAction.Update,_row);}
}
this.invokeMethod =function(methodName,parameters){}
this.raisePropertyChanged =function(propertyName){this.propertyChanged.invoke(this,new Web.PropertyChangedEventArgs(propertyName));}
}
Type.registerSealedClass('Web.Data.DataRow',null,Web.ICustomTypeDescriptor,Web.INotifyPropertyChanged,Web.ITypeDescriptorProvider,Web.IDisposable);Web.Data.DataTable =function(tableArray,columns,keys){var _array =tableArray;if (!Array.isInstanceOfType(_array)){_array =[_array];}
var _columns =Array.isInstanceOfType(columns)?columns :[];var _keys =Array.isInstanceOfType(keys)?keys :[];var _rows =[];var _deletedRows =[];var _newRows =[];var _updatedRows =[];this.get_columns =function(){return _columns;}
this.get_keys =function(){return _keys;}
this.get_isDirty =function(){return (_deletedRows.length !=0)||(_newRows.length !=0)||(_updatedRows.length !=0);}
this.get_length =function(){return _array.length;}
this.add =function(rowObject){var row;if (Web.Data.DataRow.isInstanceOfType(rowObject)){row =rowObject;rowObject =rowObject.get__rowObject();}
else {row =new Web.Data.DataRow(rowObject,this);}
row._set__index(_array.length);var columns =this.get_columns();if (columns){for(var i =columns.length -1;i >=0;i--){var column =columns[i];if (typeof(rowObject[column.get_columnName()])=="undefined"){rowObject[column.get_columnName()]=column.get_defaultValue();}
}
}
var oldIsDirty =this.get_isDirty();_array.add(rowObject);_rows.add(row);_newRows.add(rowObject);this.raiseCollectionChanged(Web.NotifyCollectionChangedAction.Add,rowObject);this.raisePropertyChanged("length");if (!oldIsDirty){this.raisePropertyChanged("isDirty");}
return row;}
this.clear =function(){if (this.get_length()>0){var oldIsDirty =this.get_isDirty();for (var i =_rows.length -1;i >=0;i--){var row =_rows[i];if (row){var rowObject =row.get__rowObject();if (!_newRows.contains(rowObject)){_deletedRows.add(rowObject);}
}
}
_rows =[];_array =[];this.raiseCollectionChanged(Web.NotifyCollectionChangedAction.Reset,null);this.raisePropertyChanged("length");if (!oldIsDirty){this.raisePropertyChanged("isDirty");}
}
}
this.getChanges =function(){return {updated :_updatedRows,inserted :_newRows,deleted :_deletedRows};}
this.getColumn =function(name){for (var c =_columns.length -1;c >=0;c--){var column =_columns[c];if (column.get_columnName()==name){return column;}
}
return null;}
this.getItem =function(index){var row =_rows[index];if (!row){var rowObject =_array[index];row =Web.Data.DataRow.isInstanceOfType(rowObject)?rowObject :new Web.Data.DataRow(_array[index],this,index);_rows[index]=row;}
return row;}
this.remove =function(rowObject){if (Web.Data.DataRow.isInstanceOfType(rowObject)){rowObject =rowObject.get__rowObject();}
var oldIsDirty =this.get_isDirty();var index =_array.indexOf(rowObject);_array.removeAt(index);_rows.removeAt(index);index =_newRows.indexOf(rowObject);if (index !=-1){_newRows.removeAt(index);}
else {_deletedRows.add(rowObject);}
this.raiseCollectionChanged(Web.NotifyCollectionChangedAction.Remove,rowObject);this.raisePropertyChanged("length");if (oldIsDirty !=this.get_isDirty()){this.raisePropertyChanged("isDirty");}
}
this.dispose =function(){if (this.collectionChanged){this.collectionChanged.dispose();this.collectionChanged =null;}
if (this.propertyChanged){this.propertyChanged.dispose();this.propertyChanged =null;}
var i,row;for (i =_rows.length -1;i >=0;i--){row =_rows[i];if (row){_rows[i].dispose();}
}
for (i =_deletedRows.length -1;i >=0;i--){row =_deletedRows[i];if (row &&row.dispose){row.dispose();}
}
for (i =_newRows.length -1;i >=0;i--){row =_newRows[i];if (row &&row.dispose){row.dispose();}
}
for (i =_updatedRows.length -1;i >=0;i--){row =_updatedRows[i];if (row &&row.dispose){row.dispose();}
}
_rows =null;_deletedRows =null;_newRows =null;_updatedRows =null;_columns =null;_array =null;_keys =null;}
this.getDescriptor =function(){var td =new Web.TypeDescriptor();td.addProperty('columns',Array,true);td.addProperty('keys',Array,true);td.addProperty('length',Number,true);td.addProperty('isDirty',Boolean,true);td.addMethod('add');td.addMethod('clear');td.addMethod('remove');td.addEvent('collectionChanged',true);td.addEvent('propertyChanged',true);return td;}
this.initialize =function(){this.collectionChanged.initialize(this);this.propertyChanged.initialize(this);}
this.collectionChanged =new Web.Event(null);this.propertyChanged =new Web.Event(null);this.raiseCollectionChanged =function(action,changedItem){if (action ==Web.NotifyCollectionChangedAction.Update){if ((_updatedRows.indexOf(changedItem)==-1)&&(_newRows.indexOf(changedItem)==-1)){var oldIsDirty =this.get_isDirty();_updatedRows.add(changedItem);if (!oldIsDirty){this.raisePropertyChanged("isDirty");}
}
}
this.collectionChanged.invoke(this,new Web.CollectionChangedEventArgs(action,changedItem));}
this.raisePropertyChanged =function(propertyName){this.propertyChanged.invoke(this,new Web.PropertyChangedEventArgs(propertyName));}
}
Type.registerSealedClass('Web.Data.DataTable',null,Web.IArray,Web.INotifyPropertyChanged,Web.INotifyCollectionChanged,Web.ITypeDescriptorProvider,Web.IDisposable);Web.Data.DataView =function(){Web.Data.DataView.initializeBase(this);var _data =null;var _filtered =null;var _filters;var _dataChangedDelegate;this.get_data =function(){return _data;}
this.set_data =function(data){_filtered =null;if (_data &&Web.INotifyCollectionChanged.isImplementedBy(_data)){_data.collectionChanged.remove(_dataChangedDelegate);}
_data =data;if (_data){if (!Web.Data.DataTable.isInstanceOfType(_data)){_data =new Web.Data.DataTable(_data);}
_data.collectionChanged.add(_dataChangedDelegate);}
this.raisePropertyChanged('data');this.raisePropertyChanged('filteredData');}
this.get_filteredData =function(){if (!_filtered &&_data){var filtered =[];var filters =this.get_filters();var filterLength =filters.get_length();for (var i =0;i <_data.get_length();i++){var valid =true;var item =_data.getItem(i);for (var j =0;j <filterLength;j++){if (!filters[j].filter(item)){valid =false;break;}
}
if (valid){filtered.add(item);}
}
_filtered =new Web.Data.DataTable(filtered);}
return _filtered;}
this.get_filters =function(){if (!_filters){_filters =Web.Component.createCollection(this);}
return _filters;}
this.get_length =function(){return _filtered ?_filtered.length :0;}
this.dispose =function(){if (_filters){_filters.dispose();_filters =null;}
Web.Data.DataView.callBaseMethod(this,'dispose');}
Web.Data.DataView.registerBaseMethod(this,'dispose');this.getDescriptor =function(){var td =Web.Data.DataView.callBaseMethod(this,'getDescriptor');td.addProperty("data",Web.Data.DataTable);td.addProperty("filteredData",Web.Data.DataTable,true);td.addProperty("filters",Array,true);td.addProperty("length",Number,true);return td;}
Web.Data.DataView.registerBaseMethod(this,'getDescriptor');this.initialize =function(){Web.Data.DataView.callBaseMethod(this,'initialize');if (_filters){for (var i =0;i <_filters.length;i++){_filters[i].initialize(this);}
}
}
Web.Data.DataView.registerBaseMethod(this,'initialize');function onDataChanged(sender,args){_filtered =null;this.raisePropertyChanged('filteredData');}
_dataChangedDelegate =Function.createDelegate(this,onDataChanged);this.getItem =function(index){return _filtered ?_filtered[index]:null;}
}
Type.registerClass('Web.Data.DataView',Web.Component);Web.TypeDescriptor.addType('script','dataView',Web.Data.DataView,Web.IArray);Web.Data.DataFilter =function(){Web.Data.DataFilter.initializeBase(this,[false]);this.filter =Function.abstractMethod;this.get_dataContext =function(){var dc =Web.Data.DataFilter.callBaseMethod(this,'get_dataContext');if (!dc){if (this.owner){dc =this.owner.get_dataContext();}
}
return dc;}
Web.Data.DataFilter.registerBaseMethod(this,'get_dataContext');this.dispose =function(){this.owner =null;Web.Data.DataFilter.callBaseMethod(this,'dispose');}
Web.Data.DataFilter.registerBaseMethod(this,'dispose');this.setOwner =function(owner){this.owner =owner;}
}
Type.registerAbstractClass('Web.Data.DataFilter',Web.Component);Web.Data.PropertyFilter =function(){Web.Data.PropertyFilter.initializeBase(this,[false]);var _property;var _value;this.get_property =function(){return _property;}
this.set_property =function(name){_property =name;}
this.get_value =function(){return _value;}
this.set_value =function(value){_value =value;}
this.filter =function(item){if (Web.ITypeDescriptorProvider.isImplementedBy(item)){return Web.TypeDescriptor.getProperty(item,_property)==_value;}
return item[_property]==_value;}
this.getDescriptor =function(){var td =Web.Data.PropertyFilter.callBaseMethod(this,'getDescriptor');td.addProperty('property',String);td.addProperty('value',String);return td;}
}
Type.registerSealedClass('Web.Data.PropertyFilter',Web.Data.DataFilter);Web.TypeDescriptor.addType('script','propertyFilter',Web.Data.PropertyFilter);Web.Data.DataSource =function(){Web.Data.DataSource.initializeBase(this,[true]);var _data =new Web.Data.DataTable([]);var _serviceURL ="";var _selectParameters ={};var _isReady =true;var _dataChangedDelegate;this.get_data =function(){return _data;}
this._set_data =function(data){var oldIsDirtyAndReady =this.get_isDirtyAndReady();var oldIsReady =this.get_isReady();var oldRowCount =this.get_rowCount();if (_data){_data.propertyChanged.remove(_dataChangedDelegate);}
_data =data;if (_data){_data.propertyChanged.add(_dataChangedDelegate);}
this.raisePropertyChanged('data');if (oldIsDirtyAndReady !=this.get_isDirtyAndReady()){this.raisePropertyChanged('isDirtyAndReady');}
if (oldIsReady !=this.get_isReady()){this.raisePropertyChanged('isReady');}
if (oldRowCount !=this.get_rowCount()){this.raisePropertyChanged('rowCount');}
}
this.get_isDirtyAndReady =function(){return _isReady &&_data &&_data.get_isDirty();}
this.get_isReady =function(){return _isReady;}
this._set_isReady =function(value){if (_isReady !=value){var oldDirtyAndReady =this.get_isDirtyAndReady();_isReady =value;this.raisePropertyChanged("isReady");if (this.get_isDirtyAndReady()!=oldDirtyAndReady){this.raisePropertyChanged("isDirtyAndReady");}
}
}
this.get_selectParameters =function(){return _selectParameters;}
this.get_serviceURL =function(){return _serviceURL;}
this.set_serviceURL =function(url){_serviceURL =url;this.raisePropertyChanged('serviceURL');}
this.get_rowCount =function(){if (_data){return _data.get_length();}
return 0;}
this.dispose =function(){if (_data){_data.dispose();}
_data =null;Web.Data.DataSource.callBaseMethod(this,'dispose');}
this.getDescriptor =function(){var td =Web.Data.DataSource.callBaseMethod(this,'getDescriptor');td.addProperty('data',Object);td.addProperty('isDirtyAndReady',Boolean,true);td.addProperty('isReady',Boolean,true);td.addProperty('rowCount',Number,true);td.addProperty('serviceURL',String);td.addProperty('selectParameters',Object,true);td.addMethod('select');td.addMethod('update');td.addEvent('dataAvailable',true);return td;}
this.dataAvailable =this.createEvent();function onDataPropertyChanged(sender,args){switch(args.get_propertyName()){case "isDirty":this.raisePropertyChanged("isDirtyAndReady");break;case "length":this.raisePropertyChanged("rowCount");break;}
}
_dataChangedDelegate =Function.createDelegate(this,onDataPropertyChanged);function onSelectComplete(rawData){var oldDirtyAndReady =this.get_isDirtyAndReady();this._set_data(rawData);_isReady =true;this.raisePropertyChanged("isReady");if (this.get_isDirtyAndReady()!=oldDirtyAndReady){this.raisePropertyChanged("isDirtyAndReady");}
this.dataAvailable.invoke(this,Web.EventArgs.Empty);}
function ready(){this._set_isReady(true);}
this.select =function(){this._set_isReady(false);Web.Net.ServiceMethodRequest.callMethod(_serviceURL,"GetData",{parameters:_selectParameters},Function.createDelegate(this,onSelectComplete),Function.createDelegate(this,ready));}
this.update =function(){if (_data &&_data.get_isDirty()){var changes =_data.getChanges();this._set_isReady(false);Web.Net.ServiceMethodRequest.callMethod(_serviceURL,"SaveData",{changeList:changes,parameters:_selectParameters},Function.createDelegate(this,onSelectComplete),Function.createDelegate(this,ready));} }
}
Type.registerSealedClass('Web.Data.DataSource',Web.Component);Web.TypeDescriptor.addType('script','dataSource',Web.Data.DataSource);Type.registerNamespace('Web.Services');Web.Services.AuthenticationService =new function(){var _path ="ScriptServices/Microsoft/Web/Services/AuthenticationService.asmx";this.login =function(userName,password,onMethodComplete,onMethodTimeout,onMethodError){return Web.Net.ServiceMethodRequest.callMethod(_path,"Login",{userName:userName,password:password},onMethodComplete,onMethodTimeout,onMethodError);}
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
Type.registerSealedClass('Web._Profile');Web.TypeDescriptor.addType('script','profile',Web._Profile);Web._Profile.parseFromMarkup =function(type,node,markupContext){Web.Profile.refresh();return Web.Profile;}
Web.Profile =new Web._Profile();window.XMLDOM =function(markup){if (Web.Application.get_type()==Web.ApplicationType.InternetExplorer){var progIDs =['Msxml2.DOMDocument.6.0','Msxml2.DOMDocument.5.0','Msxml2.DOMDocument.4.0','Msxml2.DOMDocument.3.0','Msxml2.DOMDocument'];for (var i =0;i <progIDs.length;i++){try {var xmlDOM =new ActiveXObject(progIDs[i]);xmlDOM.async =false;xmlDOM.loadXML(markup);return xmlDOM;}
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