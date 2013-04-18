//-----------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------
// AtlasControls.js
// Atlas UI Toolkit.
Type.registerNamespace('Web.UI');Web.UI.Label =function(associatedElement){Web.UI.Label.initializeBase(this,[associatedElement]);this.get_text =function(){return this.element.innerHTML;}
this.set_text =function(value){this.element.innerHTML =value;}
this.getDescriptor =function(){var td =Web.UI.Label.callBaseMethod(this,'getDescriptor');td.addProperty('text',String);return td;}
Web.UI.Label.registerBaseMethod(this,'getDescriptor');}
Type.registerClass('Web.UI.Label',Web.UI.Control);Web.TypeDescriptor.addType('script','label',Web.UI.Label);Web.UI.Image =function(associatedElement){Web.UI.Image.initializeBase(this,[associatedElement]);this.get_alternateText =function(){return this.element.alt;}
this.set_alternateText =function(value){this.element.alt =value;}
this.get_height =function(){return this.element.height;}
this.set_height =function(value){this.element.height =value;}
this.get_imageURL =function(){return this.element.src;}
this.set_imageURL =function(value){this.element.src =value;}
this.get_width =function(){return this.element.width;}
this.set_width =function(value){this.element.width =value;}
this.getDescriptor =function(){var td =Web.UI.Image.callBaseMethod(this,'getDescriptor');td.addProperty('alternateText',String);td.addProperty('height',Number);td.addProperty('imageURL',String);td.addProperty('width',Number);return td;}
Web.UI.Image.registerBaseMethod(this,'getDescriptor');}
Type.registerClass('Web.UI.Image',Web.UI.Control);Web.TypeDescriptor.addType('script','image',Web.UI.Image);Web.UI.HyperLink =function(associatedElement){Web.UI.HyperLink.initializeBase(this,[associatedElement]);var _clickHandler;this.get_navigateURL =function(){return this.element.href;}
this.set_navigateURL =function(value){this.element.href =value;}
this.getDescriptor =function(){var td =Web.UI.HyperLink.callBaseMethod(this,'getDescriptor');td.addProperty('navigateURL',String);td.addEvent('click',true);return td;}
Web.UI.HyperLink.registerBaseMethod(this,'getDescriptor');this.click =this.createEvent();this.dispose =function(){if (_clickHandler){this.element.detachEvent('onclick',_clickHandler);_clickHandler =null;}
Web.UI.HyperLink.callBaseMethod(this,'dispose');}
this.initialize =function(){Web.UI.HyperLink.callBaseMethod(this,'initialize');_clickHandler =Function.createDelegate(this,this._onClick);this.element.attachEvent('onclick',_clickHandler);}
this._onClick =function(){this.click.invoke(this,Web.EventArgs.Empty);}
}
Type.registerClass('Web.UI.HyperLink',Web.UI.Label);Web.TypeDescriptor.addType('script','hyperLink',Web.UI.HyperLink);Web.UI.Button =function(associatedElement){Web.UI.Button.initializeBase(this,[associatedElement]);var _clickHandler;this.getDescriptor =function(){var td =Web.UI.Button.callBaseMethod(this,'getDescriptor');td.addEvent('click',true);return td;}
Web.UI.Button.registerBaseMethod(this,'getDescriptor');this.click =this.createEvent();this.dispose =function(){if (_clickHandler){this.element.detachEvent('onclick',_clickHandler);_clickHandler =null;}
Web.UI.Button.callBaseMethod(this,'dispose');}
this.initialize =function(){Web.UI.Button.callBaseMethod(this,'initialize');_clickHandler =Function.createDelegate(this,this._onClick);this.element.attachEvent('onclick',_clickHandler);}
this._onClick =function(){this.click.invoke(this,Web.EventArgs.Empty);}
}
Type.registerClass('Web.UI.Button',Web.UI.Control);Web.TypeDescriptor.addType('script','button',Web.UI.Button);Web.UI.CheckBox =function(associatedElement){Web.UI.CheckBox.initializeBase(this,[associatedElement]);var _clickHandler;this.get_checked =function(){return this.element.checked;}
this.set_checked =function(value){if (value !=this.get_checked()){this.element.checked =value;this.raisePropertyChanged('checked');}
}
this.click =this.createEvent();this.getDescriptor =function(){var td =Web.UI.CheckBox.callBaseMethod(this,'getDescriptor');td.addProperty('checked',Boolean);td.addEvent('click',true);return td;}
Web.UI.CheckBox.registerBaseMethod(this,'getDescriptor');this.dispose =function(){if (_clickHandler){this.element.detachEvent('onclick',_clickHandler);_clickHandler =null;}
Web.UI.CheckBox.callBaseMethod(this,'dispose');}
this.initialize =function(){Web.UI.CheckBox.callBaseMethod(this,'initialize');_clickHandler =Function.createDelegate(this,this._onClick);this.element.attachEvent('onclick',_clickHandler);}
this._onClick =function(){this.raisePropertyChanged('checked');this.click.invoke(this,Web.EventArgs.Empty);}
}
Type.registerClass('Web.UI.CheckBox',Web.UI.Control);Web.TypeDescriptor.addType('script','checkBox',Web.UI.CheckBox);Web.UI.Select =function(associatedElement){Web.UI.Select.initializeBase(this,[associatedElement]);var _selectionChangedHandler;var _data;var _textProperty;var _valueProperty;var _firstItemText;this.get_data =function(){return _data;}
this.set_data =function(data){if (_data &&Web.INotifyCollectionChanged.isImplementedBy(_data)){_data.collectionChanged.remove(_dataChangedDelegate);}
_data =data;if (_data){if (!Web.Data.DataTable.isInstanceOfType(_data)){_data =new Web.Data.DataTable(_data);}
_data.collectionChanged.add(_dataChangedDelegate);}
dataBind.call(this);this.raisePropertyChanged('data');}
this.get_firstItemText =function(){return _firstItemText;}
this.set_firstItemText =function(value){if (_firstItemText !=value){_firstItemText =value;this.raisePropertyChanged('firstItemText');dataBind.call(this);}
}
this.get_selectedValue =function(){return this.element.value;}
this.set_selectedValue =function(value){this.element.value =value;}
this.get_textProperty =function(){return _textProperty;}
this.set_textProperty =function(name){_textProperty =name;this.raisePropertyChanged('textProperty');}
this.get_valueProperty =function(){return _valueProperty;}
this.set_valueProperty =function(name){_valueProperty =name;this.raisePropertyChanged('valueProperty');}
this.selectionChanged =this.createEvent();function dataBind(){var options =this.element.options;var selectedValues =[];var i;for (i =options.length -1;i >=0;i--){if (options(i).selected){selectedValues.add(options(i).value);}
options.remove(i);}
var option;if (_firstItemText &&(_firstItemText.length !=0)){option =document.createElement("OPTION");option.text =_firstItemText;option.value ="";options.add(option);}
if (_data){var length =_data.get_length();for (i =0;i <length;i++){var item =_data.getItem(i);option =document.createElement("OPTION");option.text =Web.TypeDescriptor.getProperty(item,_textProperty);option.value =Web.TypeDescriptor.getProperty(item,_valueProperty);option.selected =selectedValues.contains(option.value);options.add(option);}
}
}
_dataChangedDelegate =Function.createDelegate(this,dataBind);this.dispose =function(){if (_selectionChangedHandler){this.element.detachEvent('onchange',_selectionChangedHandler);_selectionChangedHandler =null;}
Web.UI.Select.callBaseMethod(this,'dispose');}
this.getDescriptor =function(){var td =Web.UI.Select.callBaseMethod(this,'getDescriptor');td.addProperty("data",Web.Data.DataTable);td.addProperty('firstItemText',String);td.addProperty('selectedValue',String);td.addProperty('textProperty',String);td.addProperty('valueProperty',String);td.addEvent('selectionChanged',true);return td;}
Web.UI.Select.registerBaseMethod(this,'getDescriptor');this.initialize =function(){Web.UI.Select.callBaseMethod(this,'initialize');_selectionChangedHandler =Function.createDelegate(this,this._onSelectionChanged);this.element.attachEvent('onchange',_selectionChangedHandler);}
this._onSelectionChanged =function(){this.raisePropertyChanged('selectedValue');this.selectionChanged.invoke(this,Web.EventArgs.Empty);}
}
Type.registerClass('Web.UI.Select',Web.UI.Control);Web.TypeDescriptor.addType('script','select',Web.UI.Select);Web.UI.TextBox =function(associatedElement){Web.UI.TextBox.initializeBase(this,[associatedElement]);var _text;var _changeHandler;var _keyPressHandler;this.get_text =function(){return this.element.value;}
this.set_text =function(value){if (this.element.value !=value){this.element.value =value;this.raisePropertyChanged('text');}
}
this.dispose =function(){if (_changeHandler){this.element.detachEvent('onchange',_changeHandler);_changeHandler =null;}
if (_keyPressHandler){this.element.detachEvent('onkeypress',_keyPressHandler);_keyPressHandler =null;}
Web.UI.TextBox.callBaseMethod(this,'dispose');}
this.getDescriptor =function(){var td =Web.UI.TextBox.callBaseMethod(this,'getDescriptor');td.addProperty('text',String);td.addAttribute(Web.Attributes.ValueProperty,'text');return td;}
Web.UI.TextBox.registerBaseMethod(this,'getDescriptor');this.initialize =function(){Web.UI.TextBox.callBaseMethod(this,'initialize');_text =this.element.value;_changeHandler =Function.createDelegate(this,this._onChanged);this.element.attachEvent('onchange',_changeHandler);_keyPressHandler =Function.createDelegate(this,this._onKeyPress);this.element.attachEvent('onkeypress',_keyPressHandler);}
this._onChanged =function(){if (this.element.value !=_text){_text =this.element.value;this.raisePropertyChanged('text');}
}
this._onKeyPress =function(){var e =window.event;var key =e.keyCode;if (key ==13){if (this.element.value !=_text){_text =this.element.value;this.raisePropertyChanged('text');}
}
}
}
Type.registerClass('Web.UI.TextBox',Web.UI.InputControl);Web.TypeDescriptor.addType('script','textBox',Web.UI.TextBox);Web.UI.PopupBehavior =function(){Web.UI.PopupBehavior.initializeBase(this);var _x =0;var _y =0;var _positioningMode =Web.UI.PositioningMode.Absolute;var _parentElement;var _moveHandler;this.get_parentElement =function(){return _parentElement;}
this.set_parentElement =function(element){_parentElement =element;this.raisePropertyChanged('parentElement');}
this.get_positioningMode =function(){return _positioningMode;}
this.set_positioningMode =function(mode){_positioningMode =mode;this.raisePropertyChanged('positioningMode');}
this.get_x =function(){return _x;}
this.set_x =function(x){_x =x;if (this.control.get_visible()){this.show();}
this.raisePropertyChanged('x');}
this.get_y =function(){return _y;}
this.set_y =function(y){_y =y;if (this.control.get_visible()){this.show();}
this.raisePropertyChanged('y');}
this.hide =function(){this.control.set_visible(false);var elt =this.control.element;if (elt.originalWidth){elt.style.width =elt.originalWidth +"px";elt.originalWidth =null;}
if (window.navigator &&window.navigator.appName =="Microsoft Internet Explorer"&&!window.opera){var childFrame =elt._hideWindowedElementsIFrame;if (childFrame){childFrame.style.display ="none";}
}
}
this.show =function(){this.control.set_visible(true);var elt =this.control.element;var offsetParent =elt.offsetParent;if (!offsetParent)offsetParent =document.documentElement;var offsetParentLocation =Web.UI.Control.getLocation(offsetParent);var parent =_parentElement ?_parentElement :offsetParent;var parentBounds =Web.UI.Control.getBounds(parent);var diff ={x:parentBounds.x -offsetParentLocation.x,y:parentBounds.y -offsetParentLocation.y};var width =elt.offsetWidth -(elt.clientLeft ?elt.clientLeft *2 :0);var height =elt.offsetHeight -(elt.clientTop ?elt.clientTop *2 :0);var position;switch (_positioningMode){case Web.UI.PositioningMode.Center:position ={x:Math.round(parentBounds.width /2 -width /2),y:Math.round(parentBounds.height /2 -height /2)};break;case Web.UI.PositioningMode.BottomLeft:position ={x:0,y:parentBounds.height
};break;case Web.UI.PositioningMode.BottomRight:position ={x:parentBounds.width -width,y:parentBounds.height
};break;case Web.UI.PositioningMode.TopLeft:position ={x:0,y:-elt.offsetHeight
};break;case Web.UI.PositioningMode.TopRight:position ={x:parentBounds.width -width,y:-elt.offsetHeight
};break;default:position ={x:0,y:0};}
position.x +=_x +diff.x;position.y +=_y +diff.y;Web.UI.Control.setLocation(elt,position);elt.style.width =width +"px";var newPosition =Web.UI.Control.getBounds(elt);var documentWidth =self.innerWidth ?self.innerWidth :document.documentElement.clientWidth;if (!documentWidth){documentWidth =document.body.clientWidth;}
if (newPosition.x +newPosition.width >documentWidth -5){position.x -=newPosition.x +newPosition.width -documentWidth +5;}
if (newPosition.x <0){position.x -=newPosition.x;}
if (newPosition.y <0){position.y -=newPosition.y;}
Web.UI.Control.setLocation(elt,position);if ((Web.Application.get_type()==Web.ApplicationType.InternetExplorer)&&!window.opera){var childFrame =elt._hideWindowedElementsIFrame;if (!childFrame){childFrame =document.createElement("iframe");childFrame.src ="about:blank";childFrame.style.position ="absolute";childFrame.style.display ="none";childFrame.scrolling ="no";childFrame.frameBorder ="0";childFrame.style.filter ="progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=0)";elt.parentNode.insertBefore(childFrame,elt);elt._hideWindowedElementsIFrame =childFrame;_moveHandler =Function.createDelegate(this,moveHandler);elt.attachEvent('onmove',_moveHandler);}
childFrame.style.top =elt.style.top;childFrame.style.left =elt.style.left;childFrame.style.width =elt.offsetWidth +"px";childFrame.style.height =elt.offsetHeight +"px";childFrame.style.display =elt.style.display;if (elt.currentStyle &&elt.currentStyle.zIndex){childFrame.style.zIndex =elt.currentStyle.zIndex;}
else if (elt.style.zIndex){childFrame.style.zIndex =elt.style.zIndex;}
}
}
this.getDescriptor =function(){var td =Web.UI.PopupBehavior.callBaseMethod(this,'getDescriptor');td.addProperty('parentElement',Object,false,Web.Attributes.Element,true);td.addProperty('positioningMode',Web.UI.PositioningMode);td.addProperty('x',Number);td.addProperty('y',Number);td.addMethod('show');td.addMethod('hide');return td;}
Web.UI.PopupBehavior.registerBaseMethod(this,'getDescriptor');this.initialize =function(){Web.UI.PopupBehavior.callBaseMethod(this,'initialize');this.hide();this.control.element.style.position ="absolute";}
Web.UI.PopupBehavior.registerBaseMethod(this,'initialize');this.dispose =function(){if (_moveHandler &&this.control &&this.control.element){this.control.element.detachEvent('onmove',_moveHandler);}
_parentElement =null;Web.UI.PopupBehavior.callBaseMethod(this,'dispose');}
Web.UI.PopupBehavior.registerBaseMethod(this,'dispose');function moveHandler(){var elt =this.control.element;if (elt._hideWindowedElementsIFrame){elt.parentNode.insertBefore(elt._hideWindowedElementsIFrame,elt);elt._hideWindowedElementsIFrame.style.top =elt.style.top;elt._hideWindowedElementsIFrame.style.left =elt.style.left;}
}
}
Type.registerClass('Web.UI.PopupBehavior',Web.UI.Behavior);Web.TypeDescriptor.addType('script','popupBehavior',Web.UI.PopupBehavior);Web.UI.PositioningMode =Web.Enum.create('Absolute','Center','BottomLeft','BottomRight','TopLeft','TopRight');Web.UI.ClickBehavior =function(){Web.UI.ClickBehavior.initializeBase(this);var _clickHandler;this.click =this.createEvent();this.dispose =function(){this.control.element.detachEvent('onclick',_clickHandler);Web.UI.ClickBehavior.callBaseMethod(this,'dispose');}
this.initialize =function(){Web.UI.ClickBehavior.callBaseMethod(this,'initialize');_clickHandler =Function.createDelegate(this,clickHandler);this.control.element.attachEvent('onclick',_clickHandler);}
this.getDescriptor =function(){var td =new Web.TypeDescriptor();td.addEvent('click',true);return td;}
function clickHandler(){this.click.invoke(this,Web.EventArgs.Empty);}
}
Type.registerSealedClass('Web.UI.ClickBehavior',Web.UI.Behavior);Web.TypeDescriptor.addType('script','clickBehavior',Web.UI.ClickBehavior);Web.UI.HoverBehavior =function(){Web.UI.HoverBehavior.initializeBase(this);var _hoverHandler;var _unHoverHandler;var _hoverElement;var _unhoverDelay =0;var _hoverCount =0;this.get_hoverElement =function(){return _hoverElement;}
this.set_hoverElement =function(element){_hoverElement =element;this.raisePropertyChanged('hoverElement');}
this.get_unhoverDelay =function(){return _unhoverDelay;}
this.set_unhoverDelay =function(ms){_unhoverDelay =ms;this.raisePropertyChanged('unhoverDelay');}
this.getDescriptor =function(){var td =new Web.TypeDescriptor();td.addProperty('hoverElement',Object,false,Web.Attributes.Element,true);td.addProperty('unhoverDelay',Number);td.addEvent('hover',true);td.addEvent('unhover',true);return td;}
this.hover =this.createEvent();this.unhover =this.createEvent();this.dispose =function(){if (_hoverHandler){this.control.element.detachEvent('onmouseover',_hoverHandler);this.control.element.detachEvent('onfocus',_hoverHandler);_hoverHandler =null;}
if (_unHoverHandler){this.control.element.detachEvent('onmouseout',_unHoverHandler);this.control.element.detachEvent('onblur',_unHoverHandler);_hoverHandler =null;}
Web.UI.HoverBehavior.callBaseMethod(this,'dispose');}
this.initialize =function(){Web.UI.HoverBehavior.callBaseMethod(this,'initialize');_hoverHandler =Function.createDelegate(this,hoverHandler);this.control.element.attachEvent('onmouseover',_hoverHandler);this.control.element.attachEvent('onfocus',_hoverHandler);_unHoverHandler =Function.createDelegate(this,_unhoverDelay ?delayedUnhoverHandler :unHoverHandler);this.control.element.attachEvent('onmouseout',_unHoverHandler);this.control.element.attachEvent('onblur',_unHoverHandler);if (_hoverElement){_hoverElement.attachEvent('onmouseover',_hoverHandler);_hoverElement.attachEvent('onfocus',_hoverHandler);_hoverElement.attachEvent('onmouseout',_unHoverHandler);_hoverElement.attachEvent('onblur',_unHoverHandler);}
}
function delayedUnhoverHandler(){window.setTimeout(Function.createDelegate(this,unHoverHandler),_unhoverDelay);}
function hoverHandler(){_hoverCount++;this.hover.invoke(this,Web.EventArgs.Empty);}
function unHoverHandler(){_hoverCount--;if (_hoverCount ==0){this.unhover.invoke(this,Web.EventArgs.Empty);}
}
}
Type.registerSealedClass('Web.UI.HoverBehavior',Web.UI.Behavior);Web.TypeDescriptor.addType('script','hoverBehavior',Web.UI.HoverBehavior);Web.UI.AutoCompleteBehavior =function(){Web.UI.AutoCompleteBehavior.initializeBase(this);var _serviceURL;var _serviceMethod;var _minimumPrefixLength =3;var _completionSetCount =10;var _completionInterval =1000;var _completionListElement;var _popupBehavior;var _timer;var _cache;var _currentPrefix;var _selectIndex;var _focusHandler;var _blurHandler;var _keyDownHandler;var _mouseDownHandler;var _mouseUpHandler;var _mouseOverHandler;var _tickHandler;this.get_completionInterval =function(){return _completionInterval;}
this.set_completionInterval =function(value){_completionInterval =value;}
this.get_completionList =function(){return _completionListElement;}
this.set_completionList =function(value){_completionListElement =value;}
this.get_completionSetCount =function(){return _completionSetCount;}
this.set_completionSetCount =function(value){_completionSetCount =value;}
this.get_minimumPrefixLength =function(){return _minimumPrefixLength;}
this.set_minimumPrefixLength =function(value){_minimumPrefixLength =value;}
this.get_serviceMethod =function(){return _serviceMethod;}
this.set_serviceMethod =function(value){_serviceMethod =value;}
this.get_serviceURL =function(){return _serviceURL;}
this.set_serviceURL =function(value){_serviceURL =value;}
this.dispose =function(){if (_timer){_timer.tick.remove(_tickHandler);_timer.dispose();}
var element =this.control.element;element.detachEvent('onfocus',_focusHandler);element.detachEvent('onblur',_blurHandler);element.detachEvent('onkeydown',_keyDownHandler);_completionListElement.detachEvent('onmousedown',_mouseDownHandler);_completionListElement.detachEvent('onmouseup',_mouseUpHandler);_completionListElement.detachEvent('onmouseover',_mouseOverHandler);_tickHandler =null;_focusHandler =null;_blurHandler =null;_keyDownHandler =null;_mouseDownHandler =null;_mouseUpHandler =null;_mouseOverHandler =null;Web.UI.AutoCompleteBehavior.callBaseMethod(this,'dispose');}
this.getDescriptor =function(){var td =new Web.TypeDescriptor();td.addProperty('completionInterval',Number);td.addProperty('completionList',Object,false,Web.Attributes.Element,true);td.addProperty('completionSetCount',Number);td.addProperty('minimumPrefixLength',Number);td.addProperty('serviceMethod',String);td.addProperty('serviceURL',String);return td;}
this.initialize =function(){Web.UI.AutoCompleteBehavior.callBaseMethod(this,'initialize');_tickHandler =Function.createDelegate(this,this._onTimerTick);_focusHandler =Function.createDelegate(this,this._onGotFocus);_blurHandler =Function.createDelegate(this,this._onLostFocus);_keyDownHandler =Function.createDelegate(this,this._onKeyDown);_mouseDownHandler =Function.createDelegate(this,this._onListMouseDown);_mouseUpHandler =Function.createDelegate(this,this._onListMouseUp);_mouseOverHandler =Function.createDelegate(this,this._onListMouseOver);_timer =new Web.Timer();_timer.set_interval(_completionInterval);_timer.tick.add(_tickHandler);var element =this.control.element;element.autocomplete ="off";element.attachEvent('onfocus',_focusHandler);element.attachEvent('onblur',_blurHandler);element.attachEvent('onkeydown',_keyDownHandler);var elementBounds =Web.UI.Control.getBounds(element);var completionListStyle =_completionListElement.style;completionListStyle.visibility ='hidden';completionListStyle.backgroundColor ='window';completionListStyle.color ='windowtext';completionListStyle.border ='solid 1px buttonshadow';completionListStyle.cursor ='default';completionListStyle.unselectable ='unselectable';completionListStyle.overflow ='hidden';completionListStyle.width =(elementBounds.width -2)+'px';_completionListElement.attachEvent('onmousedown',_mouseDownHandler);_completionListElement.attachEvent('onmouseup',_mouseUpHandler);_completionListElement.attachEvent('onmouseover',_mouseOverHandler);document.body.appendChild(_completionListElement);var popupControl =new Web.UI.Control(_completionListElement);_popupBehavior =new Web.UI.PopupBehavior();_popupBehavior.set_parentElement(element);_popupBehavior.set_positioningMode(Web.UI.PositioningMode.BottomLeft);popupControl.get_behaviors().add(_popupBehavior);_popupBehavior.initialize();popupControl.initialize();}
this._hideCompletionList =function(){_popupBehavior.hide();_completionListElement.innerHTML ='';_selectIndex =-1;}
this._highlightItem =function(item){var children =_completionListElement.childNodes;for (var i =0;i <children.length;i++){var child =children[i];if (child !=item){child.style.backgroundColor ='window';child.style.color ='windowtext';}
}
item.style.backgroundColor ='highlight';item.style.color ='highlighttext';}
this._onListMouseDown =function(){this._setText(window.event.srcElement.innerHTML);}
this._onListMouseUp =function(){this.control.focus();}
this._onListMouseOver =function(){var item =window.event.srcElement;_selectIndex =-1;this._highlightItem(item);}
this._onGotFocus =function(){_timer.set_enabled(true);}
this._onKeyDown =function(){var e =window.event;if (e.keyCode ==27){this._hideCompletionList();e.returnValue =false;}
else if (e.keyCode ==38){if (_selectIndex >0){_selectIndex--;this._highlightItem(_completionListElement.childNodes[_selectIndex]);e.returnValue =false;}
}
else if (e.keyCode ==40){if (_selectIndex <(_completionListElement.childNodes.length -1)){_selectIndex++;this._highlightItem(_completionListElement.childNodes[_selectIndex]);e.returnValue =false;}
}
else if (e.keyCode ==13){if (_selectIndex !=-1){this._setText(_completionListElement.childNodes[_selectIndex].innerHTML);e.returnValue =false;}
}
if (e.keyCode !=9){_timer.set_enabled(true);}
}
this._onLostFocus =function(){_timer.set_enabled(false);this._hideCompletionList();}
function _onMethodComplete(result,response,context){var acBehavior =context[0];var prefixText =context[1];acBehavior._update(prefixText,result,true);}
this._onTimerTick =function(sender,eventArgs){if (_serviceURL &&_serviceMethod){var text =this.control.element.value;if (text.trim().length <_minimumPrefixLength){this._update('',null,false);return;}
if (_currentPrefix !=text){_currentPrefix =text;if (_cache &&_cache[text]){this._update(text,_cache[text],false);return;}
Web.Net.ServiceMethodRequest.callMethod(_serviceURL,_serviceMethod,{prefixText :_currentPrefix,count:_completionSetCount },_onMethodComplete,null,null,[this,text ]);}
}
}
this._setText =function(text){_timer.set_enabled(false);_currentPrefix =text;this.control.element.value =text;this._hideCompletionList();}
this._update =function(prefixText,completionItems,cacheResults){if (cacheResults){if (!_cache){_cache ={};}
_cache[prefixText]=completionItems;}
_completionListElement.innerHTML ='';_selectIndex =-1;if (completionItems &&completionItems.length){for (var i =0;i <completionItems.length;i++){var itemElement =document.createElement('div');itemElement.innerHTML =completionItems[i];itemElement.__item ='';var itemElementStyle =itemElement.style;itemElementStyle.padding ='1px';itemElementStyle.textAlign ='left';itemElementStyle.textOverflow ='ellipsis';itemElementStyle.backgroundColor ='window';itemElementStyle.color ='windowtext';_completionListElement.appendChild(itemElement);}
_popupBehavior.show();}
else {_popupBehavior.hide();}
}
}
Type.registerSealedClass('Web.UI.AutoCompleteBehavior',Web.UI.Behavior);Web.TypeDescriptor.addType('script','autoComplete',Web.UI.AutoCompleteBehavior);Web.UI.RequiredFieldValidator =function(){Web.UI.RequiredFieldValidator.initializeBase(this);this.validate =function(value){if (!value){return false;}
if (String.isInstanceOfType(value)){if (value.length ==0){return false;}
}
return true;}
}
Type.registerSealedClass('Web.UI.RequiredFieldValidator',Web.UI.Validator);Web.TypeDescriptor.addType('script','requiredFieldValidator',Web.UI.RequiredFieldValidator);Web.UI.TypeValidator =function(){Web.UI.TypeValidator.initializeBase(this);var _type;this.get_type =function(){return _type;}
this.set_type =function(value){_type =value;}
this.getDescriptor =function(){var td =Web.UI.TypeValidator.callBaseMethod(this,'getDescriptor');td.addProperty('type',Function);return td;}
this.validate =function(value){var valid =true;if (value &&value.length){try {var number =_type.parse(value);if (isNaN(number)){valid =false;}
}
catch (ex){valid =false;}
}
return valid;}
}
Type.registerSealedClass('Web.UI.TypeValidator',Web.UI.Validator);Web.TypeDescriptor.addType('script','typeValidator',Web.UI.TypeValidator);Web.UI.RangeValidator =function(){Web.UI.RangeValidator.initializeBase(this);var _lowerBound;var _upperBound;this.get_lowerBound =function(){return _lowerBound;}
this.set_lowerBound =function(value){_lowerBound =value;}
this.get_upperBound =function(){return _upperBound;}
this.set_upperBound =function(value){_upperBound =value;}
this.getDescriptor =function(){var td =Web.UI.RangeValidator.callBaseMethod(this,'getDescriptor');td.addProperty('lowerBound',Number);td.addProperty('upperBound',Number);return td;}
this.validate =function(value){if (value &&value.length){return ((value <=_upperBound)&&(value >=_lowerBound));}
return true;}
}
Type.registerSealedClass('Web.UI.RangeValidator',Web.UI.Validator);Web.TypeDescriptor.addType('script','rangeValidator',Web.UI.RangeValidator);Web.UI.RegexValidator =function(){Web.UI.RegexValidator.initializeBase(this);var _regex;this.get_regex =function(){return _regex;}
this.set_regex =function(value){_regex =value;}
this.getDescriptor =function(){var td =Web.UI.RegexValidator.callBaseMethod(this,'getDescriptor');td.addProperty('regex',RegExp);return td;}
this.validate =function(value){if (value &&value.length){var matches =_regex.exec(value);return (matches &&(matches[0]==value));}
return true;}
}
Type.registerSealedClass('Web.UI.RegexValidator',Web.UI.Validator);Web.TypeDescriptor.addType('script','regexValidator',Web.UI.RegexValidator);Web.UI.CustomValidationEventArgs =function(value){Web.UI.CustomValidationEventArgs.initializeBase(this);var _value =value;var _isValid =true;this.get_value =function(){return _value;}
this.get_isValid =function(){return _isValid;}
this.set_isValid =function(value){_isValid =value;}
}
Web.UI.CustomValidator =function(){Web.UI.CustomValidator.initializeBase(this);this.validateValue =this.createEvent();this.getDescriptor =function(){var td =Web.UI.CustomValidator.callBaseMethod(this,'getDescriptor');td.addEvent('validateValue',false);return td;}
this.validate =function(value){if (value &&value.length){var cve =new Web.UI.CustomValidationEventArgs(value);this.validateValue.invoke(this,cve);return cve.get_isValid();}
return true;}
}
Type.registerSealedClass('Web.UI.CustomValidator',Web.UI.Validator);Web.TypeDescriptor.addType('script','customValidator',Web.UI.CustomValidator);Web.UI.ValidationErrorLabel =function(associatedElement){Web.UI.ValidationErrorLabel.initializeBase(this,[associatedElement]);var _associatedControl;var _validatedHandler;this.get_associatedControl =function(){return _associatedControl;}
this.set_associatedControl =function(value){if (_associatedControl &&_validatedHandler){_associatedControl.validated.remove(_validatedHandler);}
if (Web.UI.IValidationTarget.isImplementedBy(value)){_associatedControl =value;}
if (_associatedControl){if (!_validatedHandler){_validatedHandler =Function.createDelegate(this,this._onControlValidated);}
_associatedControl.validated.add(_validatedHandler);}
}
this.dispose =function(){if (_associatedControl){if (_validatedHandler){_associatedControl.validated.remove(_validatedHandler);_validatedHandler =null;}
_associatedControl =null;}
Web.UI.ValidationErrorLabel.callBaseMethod(this,'dispose');}
this.getDescriptor =function(){var td =Web.UI.ValidationErrorLabel.callBaseMethod(this,'getDescriptor');td.addProperty('associatedControl',Object);return td;}
this.initialize =function(){Web.UI.ValidationErrorLabel.callBaseMethod(this,'initialize');this.set_visible(false);}
this._onControlValidated =function(sender,eventArgs){var isInvalid =_associatedControl.get_isInvalid();var tooltip ='';if (isInvalid){tooltip =_associatedControl.get_validationMessage();}
this.set_visible(isInvalid);this.element.title =tooltip;}
}
Type.registerSealedClass('Web.UI.ValidationErrorLabel',Web.UI.Label);Web.TypeDescriptor.addType('script','validationErrorLabel',Web.UI.ValidationErrorLabel);Type.registerNamespace('Web.UI.Data');Web.UI.Data.ItemView =function(associatedElement){Web.UI.Data.ItemView.initializeBase(this,[associatedElement]);var _data;var _dataIndex =0;var _itemTemplate;var _emptyTemplate;var _renderPending =true;function prepareChange(){return {dataIndex:this.get_dataIndex(),canMoveNext:this.get_canMoveNext(),canMovePrevious:this.get_canMovePrevious()};}
function triggerChangeEvents(oldState){var dataIndex =this.get_dataIndex();if (oldState.dataIndex !=dataIndex){this.raisePropertyChanged('dataIndex');oldState.dataIndex =dataIndex;}
var canMoveNext =this.get_canMoveNext();if (oldState.canMoveNext !=canMoveNext){this.raisePropertyChanged('canMoveNext');oldState.canMoveNext =canMoveNext;}
var canMovePrevious =this.get_canMovePrevious();if (oldState.canMovePrevious !=canMovePrevious){this.raisePropertyChanged('canMovePrevious');oldState.canMovePrevious =canMovePrevious;}
}
this.get_canMoveNext =function(){if (!_data)return false;return (_dataIndex <_data.get_length()-1);}
this.get_canMovePrevious =function(){if (!_data)return false;return (_dataIndex >0);}
this.get_data =function(){return _data;}
this.set_data =function(value){var oldState =prepareChange.call(this);_data =value;if (_data){if (!Web.Data.DataTable.isInstanceOfType(_data)){_data =new Web.Data.DataTable(_data);}
}
var newLength =_data ?_data.get_length():0;if (_dataIndex >=newLength){this.set_dataIndex(0);}
if (this.get_isUpdating()){_renderPending =true;}
else {this.render();}
this.raisePropertyChanged('data');triggerChangeEvents.call(this,oldState);}
this.get_dataContext =function(){return this.get_dataItem();}
Web.UI.Data.ItemView.registerBaseMethod(this,'get_dataContext');this.get_dataIndex =function(){return _dataIndex;}
this.set_dataIndex =function(value){if (_dataIndex !=value){var oldState =prepareChange.call(this);_dataIndex =value;if (this.get_isUpdating()){_renderPending =true;}
else {this.render();}
triggerChangeEvents.call(this,oldState);}
}
this.get_dataItem =function(){if (_data){return _data.getItem(_dataIndex);}
return null;}
this.get_emptyTemplate =function(){return _emptyTemplate;}
this.set_emptyTemplate =function(value){if (_emptyTemplate){_emptyTemplate.dispose();}
_emptyTemplate =value;if (this.get_isUpdating()){_renderPending =true;}
else {this.render();}
}
this.get_itemTemplate =function(){return _itemTemplate;}
this.set_itemTemplate =function(value){if (_itemTemplate){_itemTemplate.dispose();}
_itemTemplate =value;if (this.get_isUpdating()){_renderPending =true;}
else {this.render();}
}
this.dispose =function(){if (_itemTemplate){_itemTemplate.dispose();_itemTemplate =null;}
if (_emptyTemplate){_emptyTemplate.dispose();_emptyTemplate =null;}
Web.UI.Data.ItemView.callBaseMethod(this,'dispose');}
Web.UI.Data.ItemView.registerBaseMethod(this,'getDescriptor');this.getDescriptor =function(){var td =Web.UI.Data.ItemView.callBaseMethod(this,'getDescriptor');td.addProperty('canMoveNext',Boolean,true);td.addProperty('canMovePrevious',Boolean,true);td.addProperty('data',Web.Data.DataTable);td.addProperty('dataIndex',Number);td.addProperty('dataItem',Object,true);td.addProperty('itemTemplate',Web.UI.ITemplate);td.addProperty('emptyTemplate',Web.UI.ITemplate);td.addMethod('addItem');td.addMethod('deleteCurrentItem');td.addMethod('moveNext');td.addMethod('movePrevious');return td;}
Web.UI.Data.ItemView.registerBaseMethod(this,'getDescriptor');this.addItem =function(){if (_data){var oldState =prepareChange.call(this);_data.add({});this.set_dataIndex(_data.get_length()-1);triggerChangeEvents.call(this,oldState);}
}
this.deleteCurrentItem =function(){if (_data){var oldState =prepareChange.call(this);_data.remove(this.get_dataItem());if (this.get_dataIndex()>=_data.get_length()){this.set_dataIndex(_data.get_length()-1);}
triggerChangeEvents.call(this,oldState);}
}
this.initialize =function(){Web.UI.Data.ItemView.callBaseMethod(this,'initialize');if (_itemTemplate){_itemTemplate.initialize();}
if (_emptyTemplate){_emptyTemplate.initialize();}
this.render();}
this.moveNext =function(){if (_data){var oldState =prepareChange.call(this);var newIndex =this.get_dataIndex()+1;if (newIndex <_data.get_length()){this.set_dataIndex(newIndex);}
triggerChangeEvents.call(this,oldState);}
}
this.movePrevious =function(){if (_data){var oldState =prepareChange.call(this);var newIndex =this.get_dataIndex()-1;if (newIndex >=0){this.set_dataIndex(newIndex);}
triggerChangeEvents.call(this,oldState);}
}
this.render =function(){if (this.element.childNodes.length){Web.UI.ITemplate.disposeInstance(this.element);}
this.element.innerHTML ='';var template;if (_data &&_data.get_length()){template =_itemTemplate;}
else {template =_emptyTemplate;}
if (template){template.createInstance(this.element,this.get_dataContext());}
_renderPending =false;}
}
Type.registerClass('Web.UI.Data.ItemView',Web.UI.Control);Web.TypeDescriptor.addType('script','itemView',Web.UI.Data.ItemView);Web.UI.Data.ListView =function(associatedElement){Web.UI.Data.ListView.initializeBase(this,[associatedElement]);var _itemClass;var _alternatingItemClass;var _data =null;var _layoutTemplate =null;var _itemTemplate =null;var _separatorTemplate =null;var _emptyTemplate =null;var _itemTemplateParentElementId;var _itemElements =[];var _separatorElements =[];var _dataChangedDelegate;this.get_alternatingItemCssClass =function(){return _alternatingItemClass;}
this.set_alternatingItemCssClass =function(name){_alternatingItemClass =name;this.render();this.raisePropertyChanged('alternatingItemCssClass');}
this.get_data =function(){return _data;}
this.set_data =function(data){if (_data &&Web.INotifyCollectionChanged.isImplementedBy(_data)){_data.collectionChanged.remove(_dataChangedDelegate);}
_data =data;if (_data){if (!Web.Data.DataTable.isInstanceOfType(_data)){_data =new Web.Data.DataTable(_data);}
_data.collectionChanged.add(_dataChangedDelegate);}
this.render();this.raisePropertyChanged('data');}
this.get_length =function(){return Array.isInstanceOfType(_data)?_data.length :0;}
this.get_layoutTemplate =function(){return _layoutTemplate;}
this.set_layoutTemplate =function(template){_layoutTemplate =template;this.render();this.raisePropertyChanged('layoutTemplate');}
this.get_itemCssClass =function(){return _itemClass;}
this.set_itemCssClass =function(name){_itemClass =name;this.render();this.raisePropertyChanged('itemCssClass');}
this.get_itemTemplate =function(){return _itemTemplate;}
this.set_itemTemplate =function(template){_itemTemplate =template;this.render();this.raisePropertyChanged('itemTemplate');}
this.get_itemTemplateParentElementId =function(){return _itemTemplateParentElementId;}
this.set_itemTemplateParentElementId =function(id){_itemTemplateParentElementId =id;this.raisePropertyChanged('itemTemplateParentElementId');}
this.get_separatorTemplate =function(){return _separatorTemplate;}
this.set_separatorTemplate =function(template){_separatorTemplate =template;this.render();this.raisePropertyChanged('separatorTemplate');}
this.get_emptyTemplate =function(){return _emptyTemplate;}
this.set_emptyTemplate =function(template){_emptyTemplate =template;this.render();this.raisePropertyChanged('emptyTemplate');}
this.getDescriptor =function(){var td =Web.UI.Data.ListView.callBaseMethod(this,'getDescriptor');td.addProperty("alternatingItemCssClass",String);td.addProperty("data",Web.Data.DataTable);td.addProperty("length",Number,true);td.addProperty("layoutTemplate",Web.UI.ITemplate);td.addProperty("itemCssClass",String);td.addProperty("itemTemplate",Web.UI.ITemplate);td.addProperty("itemTemplateParentElementId",String);td.addProperty("separatorTemplate",Web.UI.ITemplate);td.addProperty("emptyTemplate",Web.UI.ITemplate);return td;}
Web.UI.Data.ListView.registerBaseMethod(this,'getDescriptor');function onDataChanged(sender,args){if (args.get_action()!=Web.NotifyCollectionChangedAction.Update){this.render();}
}
_dataChangedDelegate =Function.createDelegate(this,onDataChanged);this.getItem =function(index){return _itemElements[index];}
this.initialize =function(){Web.UI.Data.ListView.callBaseMethod(this,'initialize');if (_itemTemplate){_itemTemplate.initialize();}
if (_separatorTemplate){_separatorTemplate.initialize();}
if (_emptyTemplate){_emptyTemplate.initialize();}
if (_layoutTemplate){_layoutTemplate.initialize();}
this.render();}
Web.UI.Data.ListView.registerBaseMethod(this,'initialize');this.dispose =function(){if (_layoutTemplate){_layoutTemplate.dispose();_layoutTemplate =null;}
if (_itemTemplate){_itemTemplate.dispose();_itemTemplate =null;}
if (_separatorTemplate){_separatorTemplate.dispose();_separatorTemplate =null;}
if (_emptyTemplate){_emptyTemplate.dispose();_emptyTemplate =null;}
_itemElements =null;_separatorElements =null;Web.UI.Data.ListView.callBaseMethod(this,'dispose');}
Web.UI.Data.ListView.registerBaseMethod(this,'getDescriptor');function findItemTemplateParentCallback(instanceElement,markupContext,id){return markupContext.findObject(id,true);}
this.render =function(){var i,element;for (i =_itemElements.length -1;i >=0;i--){element =_itemElements[i];if (element){Web.UI.ITemplate.disposeInstance(element);}
}
_itemElements =[];for (i =_separatorElements.length -1;i >=0;i--){element =_separatorElements[i];if (element){Web.UI.ITemplate.disposeInstance(element);}
}
_separatorElements =[];if (this.element.childNodes.length){Web.UI.ITemplate.disposeInstance(this.element);}
this.element.innerHTML ='';var items =this.get_data();var itemLength =items ?items.get_length():0;if (itemLength >0){var template =this.get_layoutTemplate();if (template){var itemTemplate =this.get_itemTemplate();var separatorTemplate =this.get_separatorTemplate();var layoutTemplateInstance =template.createInstance(this.element,null,findItemTemplateParentCallback,_itemTemplateParentElementId);var itemTemplateParent =layoutTemplateInstance.callbackResult;var lengthm1 =itemLength -1;for (i =0;i <itemLength;i++){var item =items.getItem(i);if (itemTemplate){element =itemTemplate.createInstance(itemTemplateParent,item).instanceElement;if (_itemClass){if ((i %2 ==1)&&(_alternatingItemClass)){element.className =_alternatingItemClass;}
else {element.className =_itemClass;}
}
_itemElements[i]=element;}
if (separatorTemplate &&(i !=lengthm1)&&itemTemplateParent){_separatorElements[i]=separatorTemplate.createInstance(itemTemplateParent).instanceElement;}
}
}
}
else {var emptyTemplate =this.get_emptyTemplate();if (emptyTemplate){emptyTemplate.createInstance(this.element);}
}
}
}
Type.registerClass('Web.UI.Data.ListView',Web.UI.Control);Web.TypeDescriptor.addType('script','listView',Web.UI.Data.ListView,Web.IArray);