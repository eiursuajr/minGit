//-----------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------
// AtlasUIGlitz.js
// Atlas Glitz UI Framework.
Web.UI.Glitz =new function(){this.getElementOpacity =function(element){var hasOpacity =false;var opacity;if (element.filters){var filters =element.filters;if (filters.length !=0){var alphaFilter =filters['DXImageTransform.Microsoft.Alpha'];if (alphaFilter !=null){opacity =alphaFilter.opacity /100.0;hasOpacity =true;}
}
}
else {var computedStyle =document.defaultView.getComputedStyle;opacity =computedStyle(element,null).getPropertyValue('opacity');hasOpacity =true;}
if (hasOpacity ==false){return 1.0;}
return parseFloat(opacity);}
this.interpolate =function(value1,value2,percentage){return value1 +(value2 -value1)*(percentage /100);}
this.setElementOpacity =function(element,value){if (element.filters){var filters =element.filters;var createFilter =true;if (filters.length !=0){var alphaFilter =filters['DXImageTransform.Microsoft.Alpha'];if (alphaFilter !=null){createFilter =false;alphaFilter.opacity =value *100;}
}
if (createFilter){element.style.filter ='progid:DXImageTransform.Microsoft.Alpha(opacity='+(value *100)+')';}
}
else {element.style.opacity =value;}
}
}
Web.UI.OpacityBehavior =function(){Web.UI.OpacityBehavior.initializeBase(this);var _value =1;this.get_value =function(){if (!this.control){return _value;}
var element =this.control.element;return Web.UI.Glitz.getElementOpacity(element);}
this.set_value =function(value){if (!this.control){_value =value;return;}
var element =this.control.element;Web.UI.Glitz.setElementOpacity(element,value);}
this.getDescriptor =function(){var td =Web.UI.OpacityBehavior.callBaseMethod(this,'getDescriptor');td.addProperty('value',Number);return td;}
this.initialize =function(){Web.UI.OpacityBehavior.callBaseMethod(this,'initialize');if (_value !=1){this.set_value(_value);}
}
}
Type.registerSealedClass('Web.UI.OpacityBehavior',Web.UI.Behavior);Web.TypeDescriptor.addType('script','opacity',Web.UI.OpacityBehavior);Web.UI.LayoutBehavior =function(){Web.UI.LayoutBehavior.initializeBase(this);var _left;var _top;var _width;var _height;this.get_height =function(){return _height;}
this.set_height =function(value){_height =value;if (this.control){this.control.element.style.height =_height;}
}
this.get_left =function(){return _left;}
this.set_left =function(value){_left =value;if (this.control){this.control.element.style.left =_left;}
}
this.get_top =function(){return _top;}
this.set_top =function(value){_top =value;if (this.control){this.control.element.style.top =_top;}
}
this.get_width =function(){return _width;}
this.set_width =function(value){_width =value;if (this.control){this.control.element.style.width =_width;}
}
this.getDescriptor =function(){var td =Web.UI.LayoutBehavior.callBaseMethod(this,'getDescriptor');td.addProperty('height',String);td.addProperty('left',String);td.addProperty('top',String);td.addProperty('width',String);return td;}
this.initialize =function(){Web.UI.LayoutBehavior.callBaseMethod(this,'initialize');if (_height){this.set_height(_height);}
if (_left){this.set_left(_left);}
if (_top){this.set_top(_top);}
if (_width){this.set_width(_width);}
}
}
Type.registerSealedClass('Web.UI.LayoutBehavior',Web.UI.Behavior);Web.TypeDescriptor.addType('script','layout',Web.UI.LayoutBehavior);Web.UI.Animation =function(){Web.UI.Animation.initializeBase(this,[false]);var _duration =1;var _fps =25;var _target;var _tickHandler;var _timer;var _percentComplete =0;var _percentDelta;var _parentAnimation;this.get_duration =function(){return _duration;}
this.set_duration =function(value){_duration =value;}
this.get_fps =function(){return _fps;}
this.set_fps =function(value){_fps =value;}
this.get_isActive =function(){return (_timer !=null);}
this.get_isPlaying =function(){return (_timer !=null)&&_timer.get_enabled();}
this.get_percentComplete =function(){return _percentComplete;}
this.get_target =function(){return _target;}
this.set_target =function(value){_target =value;}
this.ended =this.createEvent();this.started =this.createEvent();this.dispose =function(){if (_timer){_timer.tick.remove(_tickHandler);_timer.dispose();_timer =null;}
_tickHandler =null;_target =null;Web.UI.Animation.callBaseMethod(this,'dispose');}
Web.UI.Animation.registerBaseMethod(this,'dispose');this.getAnimatedValue =Function.abstractMethod;this.getDescriptor =function(){var td =Web.UI.Animation.callBaseMethod(this,'getDescriptor');td.addProperty('duration',Number);td.addProperty('fps',Number);td.addProperty('isActive',Boolean);td.addProperty('isPlaying',Boolean);td.addProperty('percentComplete',Number);td.addProperty('target',Object);td.addMethod('play');td.addMethod('pause');td.addMethod('stop');return td;}
Web.UI.Animation.registerBaseMethod(this,'getDescriptor');this.onEnd =function(){}
Web.UI.Animation.registerBaseMethod(this,'onEnd');this.onStart =function(){}
Web.UI.Animation.registerBaseMethod(this,'onStart');this.onStep =function(percentage){this.setValue(this.getAnimatedValue(percentage));}
Web.UI.Animation.registerBaseMethod(this,'onStep');this.pause =function(){if (!_parentAnimation){if (_timer){_timer.set_enabled(false);this.raisePropertyChanged('isPlaying');}
}
}
this.play =function(){if (!_parentAnimation){var resume =true;if (!_timer){resume =false;if (!_tickHandler){_tickHandler =Function.createDelegate(this,this._onTimerTick);}
_timer =new Web.Timer();_timer.set_interval(1000 /_fps);_timer.tick.add(_tickHandler);_percentDelta =100 /(_duration *_fps);this.onStart();this._updatePercentComplete(0,true);}
_timer.set_enabled(true);this.raisePropertyChanged('isPlaying');if (!resume){this.raisePropertyChanged('isActive');}
}
}
this.setOwner =function(owner){_parentAnimation =owner;}
this.setValue =Function.abstractMethod;this.stop =function(){if (!_parentAnimation){if (_timer){_timer.dispose();_timer =null;this._updatePercentComplete(100);this.onEnd();this.raisePropertyChanged('isPlaying');this.raisePropertyChanged('isActive');}
}
}
this._onTimerTick =function(){this._updatePercentComplete(_percentComplete +_percentDelta,true);}
this._updatePercentComplete =function(percentComplete,animate){if (percentComplete >100){percentComplete =100;}
_percentComplete =percentComplete;this.raisePropertyChanged('percentComplete');if (animate){this.onStep(percentComplete);}
if (percentComplete ==100){this.stop();}
}
}
Type.registerAbstractClass('Web.UI.Animation',Web.Component);Web.UI.PropertyAnimation =function(){Web.UI.PropertyAnimation.initializeBase(this,[false]);var _property;var _propertyKey;this.get_property =function(){return _property;}
this.set_property =function(value){_property =value;}
this.get_propertyKey =function(){return _propertyKey;}
this.set_propertyKey =function(value){_propertyKey =value;}
this.ended =this.createEvent();this.started =this.createEvent();this.getDescriptor =function(){var td =Web.UI.PropertyAnimation.callBaseMethod(this,'getDescriptor');td.addProperty('property',String);td.addProperty('propertyKey',String);return td;}
Web.UI.PropertyAnimation.registerBaseMethod(this,'getDescriptor');this.setValue =function(value){Web.TypeDescriptor.setProperty(this.get_target(),_property,value,_propertyKey);}
}
Type.registerAbstractClass('Web.UI.PropertyAnimation',Web.UI.Animation);Web.UI.InterpolatedAnimation =function(){Web.UI.InterpolatedAnimation.initializeBase(this);var _startValue;var _endValue;this.get_endValue =function(){return _endValue;}
this.set_endValue =function(value){_endValue =value;}
this.get_startValue =function(){return _startValue;}
this.set_startValue =function(value){_startValue =value;}
}
Type.registerAbstractClass('Web.UI.InterpolatedAnimation',Web.UI.PropertyAnimation);Web.UI.DiscreteAnimation =function(){Web.UI.DiscreteAnimation.initializeBase(this);var _values;this.get_values =function(){return _values;}
this.set_values =function(value){_values =value;}
this.getAnimatedValue =function(percentage){var index =Math.round((percentage /100)*(_values.length -1));return _values[index];}
this.getDescriptor =function(){var td =Web.UI.DiscreteAnimation.callBaseMethod(this,'getDescriptor');td.addProperty('values',Array);return td;}
}
Type.registerSealedClass('Web.UI.DiscreteAnimation',Web.UI.PropertyAnimation);Web.TypeDescriptor.addType('script','discreteAnimation',Web.UI.DiscreteAnimation);Web.UI.NumberAnimation =function(){Web.UI.NumberAnimation.initializeBase(this);var _integralValues;this.get_integralValues =function(){return _integralValues;}
this.set_integralValues =function(value){_integralValues =value;}
this.getAnimatedValue =function(percentage){var value =Web.UI.Glitz.interpolate(this.get_startValue(),this.get_endValue(),percentage);if (_integralValues){value =Math.round(value);}
return value;}
this.getDescriptor =function(){var td =Web.UI.NumberAnimation.callBaseMethod(this,'getDescriptor');td.addProperty('startValue',Number);td.addProperty('endValue',Number);td.addProperty('integralValues',Boolean);return td;}
}
Type.registerSealedClass('Web.UI.NumberAnimation',Web.UI.InterpolatedAnimation);Web.TypeDescriptor.addType('script','numberAnimation',Web.UI.NumberAnimation);Web.UI.ColorAnimation =function(){Web.UI.ColorAnimation.initializeBase(this);var _interpolateRed;var _interpolateGreen;var _interpolateBlue;this.getAnimatedValue =function(percentage){var startColor =this.get_startValue();var endColor =this.get_endValue();var red =startColor.get_red();var green =startColor.get_green();var blue =startColor.get_blue();if (_interpolateRed){red =Math.round(Web.UI.Glitz.interpolate(red,endColor.get_red(),percentage));}
if (_interpolateGreen){green =Math.round(Web.UI.Glitz.interpolate(green,endColor.get_green(),percentage));}
if (_interpolateBlue){blue =Math.round(Web.UI.Glitz.interpolate(blue,endColor.get_blue(),percentage));}
var color =new Web.UI.Color(red,green,blue);return color.toString();}
this.getDescriptor =function(){var td =Web.UI.ColorAnimation.callBaseMethod(this,'getDescriptor');td.addProperty('startValue',Web.UI.Color);td.addProperty('endValue',Web.UI.Color);return td;}
this.onStart =function(){var startColor =this.get_startValue();var endColor =this.get_endValue();_interpolateRed =(startColor.get_red()!=endColor.get_red());_interpolateGreen =(startColor.get_green()!=endColor.get_green());_interpolateBlue =(startColor.get_blue()!=endColor.get_blue());}
}
Type.registerSealedClass('Web.UI.ColorAnimation',Web.UI.InterpolatedAnimation);Web.TypeDescriptor.addType('script','colorAnimation',Web.UI.ColorAnimation);Web.UI.LengthAnimation =function(){Web.UI.LengthAnimation.initializeBase(this);var _unit ='px';this.get_unit =function(){return _unit;}
this.set_unit =function(value){_unit =value;}
this.getAnimatedValue =function(percentage){var value =Web.UI.Glitz.interpolate(this.get_startValue(),this.get_endValue(),percentage);return Math.round(value)+_unit;}
this.getDescriptor =function(){var td =Web.UI.LengthAnimation.callBaseMethod(this,'getDescriptor');td.addProperty('startValue',Number);td.addProperty('endValue',Number);td.addProperty('unit',String);return td;}
}
Type.registerSealedClass('Web.UI.LengthAnimation',Web.UI.InterpolatedAnimation);Web.TypeDescriptor.addType('script','lengthAnimation',Web.UI.LengthAnimation);Web.UI.CompositeAnimation =function(){Web.UI.DiscreteAnimation.initializeBase(this);var _animations =Web.Component.createCollection(this);this.get_animations =function(){return _animations;}
this.getAnimatedValue =function(percentage){}
this.dispose =function(){_animations.dispose();_animations =null;Web.UI.CompositeAnimation.callBaseMethod(this,'dispose');}
this.getDescriptor =function(){var td =Web.UI.CompositeAnimation.callBaseMethod(this,'getDescriptor');td.addProperty('animations',Array,true);return td;}
this.onEnd =function(){for (var i =0;i <_animations.length;i++){_animations[i].onEnd();}
}
this.onStart =function(){for (var i =0;i <_animations.length;i++){_animations[i].onStart();}
}
this.onStep =function(percentage){for (var i =0;i <_animations.length;i++){_animations[i].onStep(percentage);}
}
}
Type.registerSealedClass('Web.UI.CompositeAnimation',Web.UI.Animation);Web.TypeDescriptor.addType('script','compositeAnimation',Web.UI.CompositeAnimation);Web.UI.FadeEffect =Web.Enum.create('FadeIn','FadeOut');Web.UI.FadeAnimation =function(){Web.UI.FadeAnimation.initializeBase(this);var _effect =Web.UI.FadeEffect.FadeIn;this.get_effect =function(){return _effect;}
this.set_effect =function(value){_effect =value;}
this.getAnimatedValue =function(percentage){var startValue =0;var endValue =1;if (_effect ==Web.UI.FadeEffect.FadeOut){startValue =1;endValue =0;}
return Web.UI.Glitz.interpolate(startValue,endValue,percentage);}
this.getDescriptor =function(){var td =Web.UI.FadeAnimation.callBaseMethod(this,'getDescriptor');td.addProperty('effect',Web.UI.FadeEffect);return td;}
this.onStart =function(){var opacity =0;if (_effect ==Web.UI.FadeEffect.FadeOut){opacity =1;}
this.setValue(opacity);}
this.onEnd =function(){var opacity =1;if (_effect ==Web.UI.FadeEffect.FadeOut){opacity =0;}
this.setValue(opacity);}
this.setValue =function(value){Web.UI.Glitz.setElementOpacity(this.get_target().element,value);}
}
Type.registerSealedClass('Web.UI.FadeAnimation',Web.UI.Animation);Web.TypeDescriptor.addType('script','fadeAnimation',Web.UI.FadeAnimation);