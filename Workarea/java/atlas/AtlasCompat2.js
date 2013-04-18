//-----------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------
// AtlasCompat2.js
// Atlas Compat Framework (Part 2).
function _loadSafariCompat2Layer(w){Web.TypeDescriptor.prototype._addEvent =Web.TypeDescriptor.prototype.addEvent;Web.TypeDescriptor.prototype._addProperty =Web.TypeDescriptor.prototype.addProperty;Web.TypeDescriptor.prototype._addMethod =Web.TypeDescriptor.prototype.addMethod;Web.TypeDescriptor._createParameter =Web.TypeDescriptor.createParameter;Web.TypeDescriptor._addType =Web.TypeDescriptor.addType;Web.TypeDescriptor.prototype.addEvent =function(eventName,supportsActions){this._addEvent(eventName,supportsActions);var lcEventName =eventName.toLowerCase();if (eventName !=lcEventName){this._addEvent(lcEventName,supportsActions);this._getEvents()[lcEventName].name =eventName;}
}
Web.TypeDescriptor.prototype.addProperty =function(propertyName,propertyType,readOnly){this._addProperty.apply(this,arguments);var lcPropertyName =propertyName.toLowerCase();if (propertyName !=lcPropertyName){var baseArguments =[];baseArguments.add(lcPropertyName);for (var a =1;a <arguments.length;a++){baseArguments.add(arguments[a]);}
this._addProperty.apply(this,baseArguments);this._getProperties()[lcPropertyName].name =propertyName;}
}
Web.TypeDescriptor.prototype.addMethod =function(methodName,associatedParameters){if (associatedParameters){for (var i =associatedParameters.length -1;i >=0;i--){associatedParameters[i].name =associatedParameters[i].name.toLowerCase();}
}
this._addMethod(methodName,associatedParameters);}
Web.TypeDescriptor.createParameter =function(parameterName,parameterType){return Web.TypeDescriptor._createParameter(parameterName.toLowerCase(),parameterType);}
Web.TypeDescriptor.addType =function(tagPrefix,tagName,type,lowerCaseOnly){if (!lowerCaseOnly){Web.TypeDescriptor._addType(tagPrefix,tagName,type);}
var lcTagName =tagName.toLowerCase();if (tagName !=lcTagName){Web.TypeDescriptor._addType(tagPrefix,lcTagName,type);}
if (lcTagName =="image"){Web.TypeDescriptor._addType(tagPrefix,"image_",type);}
}
if (Web.TypeDescriptor._registeredTags !=null){var tagTable;for (var tagPrefix in Web.TypeDescriptor._registeredTags){tagTable =Web.TypeDescriptor._registeredTags[tagPrefix];for (var tagName in tagTable){if (typeof(tagTable[tagName])=="function"){Web.TypeDescriptor.addType(tagPrefix,tagName,tagTable[tagName],true);}
}
}
}
}
if (window.__safari){_loadSafariCompat2Layer(window);}
