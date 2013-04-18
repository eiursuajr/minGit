if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
if(typeof (window.RadControlsNamespace.DomEventMixin)=="undefined"||typeof (window.RadControlsNamespace.DomEventMixin.Version)==null||window.RadControlsNamespace.DomEventMixin.Version<3){
RadControlsNamespace.DomEventMixin={Version:3,Initialize:function(_1){
_1.CreateEventHandler=this.CreateEventHandler;
_1.AttachDomEvent=this.AttachDomEvent;
_1.DetachDomEvent=this.DetachDomEvent;
_1.DisposeDomEventHandlers=this.DisposeDomEventHandlers;
_1._domEventHandlingEnabled=true;
_1.EnableDomEventHandling=this.EnableDomEventHandling;
_1.DisableDomEventHandling=this.DisableDomEventHandling;
_1.RemoveHandlerRegister=this.RemoveHandlerRegister;
_1.GetHandlerRegister=this.GetHandlerRegister;
_1.AddHandlerRegister=this.AddHandlerRegister;
_1.handlerRegisters=[];
},EnableDomEventHandling:function(){
this._domEventHandlingEnabled=true;
},DisableDomEventHandling:function(){
this._domEventHandlingEnabled=false;
},CreateEventHandler:function(_2,_3){
var _4=this;
return function(e){
if(!_4._domEventHandlingEnabled&&!_3){
return;
}
return _4[_2](e||window.event);
};
},AttachDomEvent:function(_6,_7,_8,_9){
var _a=this.CreateEventHandler(_8,_9);
var _b=this.GetHandlerRegister(_6,_7,_8);
if(_b!=null){
this.DetachDomEvent(_b.Element,_b.EventName,_8);
}
var _c={"Element":_6,"EventName":_7,"HandlerName":_8,"Handler":_a};
this.AddHandlerRegister(_c);
if(_6.addEventListener){
_6.addEventListener(_7,_a,false);
}else{
if(_6.attachEvent){
_6.attachEvent("on"+_7,_a);
}
}
},DetachDomEvent:function(_d,_e,_f){
var _10=null;
var _11="";
if(typeof _f=="string"){
_11=_f;
_10=this.GetHandlerRegister(_d,_e,_11);
if(_10==null){
return;
}
_f=_10.Handler;
}
if(!_d){
return;
}
if(_d.removeEventListener){
_d.removeEventListener(_e,_f,false);
}else{
if(_d.detachEvent){
_d.detachEvent("on"+_e,_f);
}
}
if(_10!=null&&_11!=""){
this.RemoveHandlerRegister(_10);
_10=null;
}
},DisposeDomEventHandlers:function(){
for(var i=0;i<this.handlerRegisters.length;i++){
var _13=this.handlerRegisters[i];
if(_13!=null){
this.DetachDomEvent(_13.Element,_13.EventName,_13.Handler);
}
}
this.handlerRegisters=[];
},RemoveHandlerRegister:function(_14){
try{
var _15=_14.index;
for(var i in _14){
_14[i]=null;
}
this.handlerRegisters[_15]=null;
}
catch(e){
}
},GetHandlerRegister:function(_17,_18,_19){
for(var i=0;i<this.handlerRegisters.length;i++){
var _1b=this.handlerRegisters[i];
if(_1b!=null&&_1b.Element==_17&&_1b.EventName==_18&&_1b.HandlerName==_19){
return this.handlerRegisters[i];
}
}
return null;
},AddHandlerRegister:function(_1c){
_1c.index=this.handlerRegisters.length;
this.handlerRegisters[this.handlerRegisters.length]=_1c;
}};
RadControlsNamespace.DomEvent={};
RadControlsNamespace.DomEvent.PreventDefault=function(e){
if(!e){
return true;
}
if(e.preventDefault){
e.preventDefault();
}
e.returnValue=false;
return false;
};
RadControlsNamespace.DomEvent.StopPropagation=function(e){
if(!e){
return;
}
if(e.stopPropagation){
e.stopPropagation();
}else{
e.cancelBubble=true;
}
};
RadControlsNamespace.DomEvent.GetTarget=function(e){
if(!e){
return null;
}
return e.target||e.srcElement;
};
RadControlsNamespace.DomEvent.GetRelatedTarget=function(e){
if(!e){
return null;
}
return e.relatedTarget||(e.type=="mouseout"?e.toElement:e.fromElement);
};
RadControlsNamespace.DomEvent.GetKeyCode=function(e){
if(!e){
return 0;
}
return e.which||e.keyCode;
};
};if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
if(typeof (window.RadControlsNamespace.EventMixin)=="undefined"||typeof (window.RadControlsNamespace.EventMixin.Version)==null||window.RadControlsNamespace.EventMixin.Version<2){
RadControlsNamespace.EventMixin={Version:2,Initialize:function(_1){
_1._listeners={};
_1._eventsEnabled=true;
_1.AttachEvent=this.AttachEvent;
_1.DetachEvent=this.DetachEvent;
_1.RaiseEvent=this.RaiseEvent;
_1.EnableEvents=this.EnableEvents;
_1.DisableEvents=this.DisableEvents;
_1.DisposeEventHandlers=this.DisposeEventHandlers;
},DisableEvents:function(){
this._eventsEnabled=false;
},EnableEvents:function(){
this._eventsEnabled=true;
},AttachEvent:function(_2,_3){
if(!this._listeners[_2]){
this._listeners[_2]=[];
}
this._listeners[_2][this._listeners[_2].length]=(RadControlsNamespace.EventMixin.ResolveFunction(_3));
},DetachEvent:function(_4,_5){
var _6=this._listeners[_4];
if(!_6){
return false;
}
var _7=RadControlsNamespace.EventMixin.ResolveFunction(_5);
for(var i=0;i<_6.length;i++){
if(_7==_6[i]){
_6.splice(i,1);
return true;
}
}
return false;
},DisposeEventHandlers:function(){
for(var _9 in this._listeners){
var _a=null;
if(this._listeners.hasOwnProperty(_9)){
_a=this._listeners[_9];
for(var i=0;i<_a.length;i++){
_a[i]=null;
}
_a=null;
}
}
},ResolveFunction:function(_c){
if(typeof (_c)=="function"){
return _c;
}else{
if(typeof (window[_c])=="function"){
return window[_c];
}else{
return new Function("var Sender = arguments[0]; var Arguments = arguments[1];"+_c);
}
}
},RaiseEvent:function(_d,_e){
if(!this._eventsEnabled){
return true;
}
var _f=true;
if(this[_d]){
var _10=RadControlsNamespace.EventMixin.ResolveFunction(this[_d])(this,_e);
if(typeof (_10)=="undefined"){
_10=true;
}
_f=_f&&_10;
}
if(!this._listeners[_d]){
return _f;
}
for(var i=0;i<this._listeners[_d].length;i++){
var _12=this._listeners[_d][i];
var _10=_12(this,_e);
if(typeof (_10)=="undefined"){
_10=true;
}
_f=_f&&_10;
}
return _f;
}};
};function RadTextBox(id,_2,_3){
this.DisposeOldInstance(id);
this.Constructor(id);
this.Initialize(_2,_3);
}
RadTextBox.Extend=function(_4){
for(var i in this.prototype){
if(_4[i]){
continue;
}
_4[i]=this.prototype[i];
}
};
RadInputErrorReason={ParseError:0,OutOfRange:1};
RadTextBox.prototype={DisposeOldInstance:function(id){
try{
var _7=window[id];
if(_7!=null){
_7.Dispose();
window[id]=null;
}
}
catch(e){
}
},Constructor:function(id){
this.ClientID=id;
this.WrapperElementID=id+"_wrapper";
this.TextBoxElement=document.getElementById(id+"_text");
this.OriginalTextBoxCssText=this.TextBoxElement.style.cssText;
if(this.OriginalTextBoxCssText.indexOf(";")!=this.OriginalTextBoxCssText.length-1){
this.OriginalTextBoxCssText+=";";
}
this.InitializeHiddenElement(id);
this.InitializeValidationField(id);
this.SelectionEnd=0;
this.SelectionStart=0;
this.Focused=false;
this.Enabled=true;
this.Hovered=false;
this.Invalid=false;
this.IsEmptyMessage=false;
RadControlsNamespace.EventMixin.Initialize(this);
RadControlsNamespace.DomEventMixin.Initialize(this);
},Dispose:function(){
this.DisposeDomEventHandlers();
this.DisposeEventHandlers();
var _9;
for(_9 in this){
this[_9]=null;
}
},CallBase:function(_a,_b){
return RadTextBox.prototype[_a].apply(this,_b);
},Initialize:function(_c,_d){
this.Styles=_d;
this.LoadCongfiguration(_c);
this.LoadClientEvents(_c);
this.AttachEventHandlers();
this.UpdateDisplayValue();
this.UpdateCssClass();
this.InitializeButtons();
var _e=this;
this.AttachDomEvent(window,"unload","Dispose");
this.InitialValue=this.GetValue();
this.RaiseEvent("OnLoad",null);
},LoadCongfiguration:function(_f){
for(var i in _f){
if(i!="ClientEvents"){
this[i]=_f[i];
}
}
},LoadClientEvents:function(_11){
var _12=null;
for(var _13 in _11.ClientEvents){
var _12=eval(_11.ClientEvents[_13]);
if(typeof (_12)=="function"){
this.AttachEvent(_13,_12);
}
}
},AttachEventHandlers:function(){
this.AttachToTextBoxEvent("keyup","TextBoxKeyUpHandler");
this.AttachToTextBoxEvent("keypress","TextBoxKeyPressHandler");
this.AttachToTextBoxEvent("blur","TextBoxBlurHandler");
this.AttachToTextBoxEvent("focus","TextBoxFocusHandler");
this.AttachToTextBoxEvent("mouseout","TextBoxMouseOutHandler");
this.AttachToTextBoxEvent("mouseover","TextBoxMouseOverHandler");
this.AttachToTextBoxEvent("keydown","TextBoxKeyDownHandler");
if(window.addEventListener){
this.AttachToTextBoxEvent("DOMMouseScroll","TextBoxMouseWheelHandler");
this.AttachToTextBoxEvent("dragdrop","TextBoxDragDropHandler");
}else{
this.AttachToTextBoxEvent("mousewheel","TextBoxMouseWheelHandler");
this.AttachToTextBoxEvent("drop","TextBoxDropHandler");
}
},InitializeHiddenElement:function(id){
this.HiddenElement=document.getElementById(id);
},InitializeValidationField:function(id){
},IsMultiLine:function(){
return this.TextBoxElement.tagName.toUpperCase()=="TEXTAREA";
},TextBoxKeyPressHandler:function(e){
var _17=/MSIE/.test(navigator.userAgent);
var _18=_17?e.keyCode:e.which;
if(!this.RaiseEvent("OnKeyPress",{"DomEvent":e,"KeyCode":_18,"KeyCharacter":String.fromCharCode(_18)})){
return RadControlsNamespace.DomEvent.PreventDefault(e);
}
if((_18==13)&&!this.IsMultiLine()){
if(this.AutoPostBack){
this.RaisePostBackEvent();
}
return true;
}
},TextBoxKeyUpHandler:function(e){
this.UpdateHiddenValueOnKeyPress(e);
},UpdateHiddenValueOnKeyPress:function(){
this.UpdateHiddenValue();
},AttachToTextBoxEvent:function(_1a,_1b){
this.AttachDomEvent(this.TextBoxElement,_1a,_1b);
},TextBoxBlurHandler:function(e){
this.Focused=false;
this.SetValue(this.GetTextBoxValue());
this.RaiseEvent("OnBlur",{"DomEvent":e});
},TextBoxFocusHandler:function(e){
this.Focused=true;
this.UpdateDisplayValue();
this.UpdateCssClass();
this.UpdateSelectionOnFocus();
this.RaiseEvent("OnFocus",{"DomEvent":e});
},TextBoxMouseOutHandler:function(e){
this.Hovered=false;
this.UpdateCssClass();
this.RaiseEvent("OnMouseOut",{"DomEvent":e});
},TextBoxMouseOverHandler:function(e){
this.Hovered=true;
this.UpdateCssClass();
this.RaiseEvent("OnMouseOver",{"DomEvent":e});
},TextBoxDropHandler:function(e){
this.SetValue(e.dataTransfer.getData("text"));
},TextBoxDragDropHandler:function(e){
this.SetValue(this.GetTextBoxValue());
},TextBoxMouseWheelHandler:function(e){
var _23;
if(this.Focused){
if(e.wheelDelta){
_23=e.wheelDelta/120;
if(window.opera){
_23=-_23;
}
}else{
if(e.detail){
_23=-e.detail/3;
}
}
if(_23>0){
this.HandleWheel(false);
}else{
this.HandleWheel(true);
}
return true;
}
return false;
},HandleWheel:function(_24){
},TextBoxKeyDownHandler:function(e){
},Disable:function(){
this.Enabled=false;
this.TextBoxElement.disabled="disabled";
this.UpdateCssClass();
this.RaiseEvent("OnDisable",null);
},Enable:function(){
this.Enabled=true;
this.TextBoxElement.disabled="";
this.UpdateCssClass();
this.RaiseEvent("OnEnable",null);
},Focus:function(){
this.TextBoxElement.focus();
},Blur:function(){
this.TextBoxElement.blur();
},SetValue:function(_26){
var _27=this.SetHiddenValue(_26);
if(_27==false){
_26="";
}
this.TriggerDOMChangeEvent(this.GetValidationField());
this.RaiseValueChangedEvent(_26,this.InitialValue);
if(typeof (_27)=="undefined"||_27==true){
this.SetTextBoxValue(this.GetEditValue());
this.UpdateDisplayValue();
this.UpdateCssClass();
}
},TriggerDOMChangeEvent:function(_28){
if(_28.fireEvent&&document.createEventObject){
var _29=document.createEventObject();
_28.fireEvent("onchange",_29);
}else{
if(_28.dispatchEvent){
var _2a=true;
var _29=document.createEvent("HTMLEvents");
_29.initEvent("change",_2a,true);
_28.dispatchEvent(_29);
}
}
},_SetValue:function(_2b){
var _2c=this.SetHiddenValue(_2b);
if(typeof (_2c)=="undefined"||_2c==true){
this.SetTextBoxValue(this.GetEditValue());
}
},RaiseValueChangedEvent:function(_2d,_2e){
if(_2d.toString()==_2e.toString()){
return false;
}
this.InitialValue=this.GetValue();
var _2f=this.RaiseEvent("OnValueChanged",this.ValueChangedEventArgs(_2d,_2e));
if(this.AutoPostBack&&_2f){
this.RaisePostBackEvent();
}
return _2f;
},Clear:function(){
this.SetValue("");
},SetTextBoxValue:function(_30){
if(this.TextBoxElement.value!=_30){
this.TextBoxElement.value=_30;
}
},GetTextBoxValue:function(_31){
return this.TextBoxElement.value;
},GetWrapperElement:function(){
return document.getElementById(this.WrapperElementID);
},UpdateDisplayValue:function(){
if(this.Focused){
this.SetTextBoxValue(this.GetEditValue());
}else{
if(this.IsEmpty()&&this.EmptyMessage){
this.IsEmptyMessage=true;
this.SetTextBoxValue(this.EmptyMessage);
}else{
this.IsEmptyMessage=false;
this.SetTextBoxValue(this.GetDisplayValue());
}
}
},UpdateSelectionOnFocus:function(){
switch(this.SelectionOnFocus){
case 0:
break;
case 1:
this.SetCaretPosition(0);
break;
case 2:
if(this.TextBoxElement.value.length>0){
this.SetCaretPosition(this.TextBoxElement.value.length);
}
break;
case 3:
this.SelectAllText();
break;
default:
this.SetCaretPosition(0);
break;
}
},RaiseErrorEvent:function(_32){
if(this.InEventRaise){
return;
}
this.InEventRaise=true;
var _33=this.RaiseEvent("OnError",_32);
if(_33!=false){
this.Invalid=true;
this.ErrorHandlingCanceled=false;
this.UpdateCssClass();
var _34=this;
var _35=function(){
_34.Invalid=false;
_34.UpdateCssClass();
};
setTimeout(_35,100);
}else{
this.ErrorHandlingCanceled=true;
}
this.InEventRaise=false;
},RaisePostBackEvent:function(){
eval(this.PostBackEventReferenceScript);
},UpdateCssClass:function(){
if(this.Enabled&&(!this.IsEmptyMessage)&&(!this.IsNegative())){
this.TextBoxElement.style.cssText=this.OriginalTextBoxCssText+this.UpdateCssText(this.Styles["EnabledStyle"][0]);
this.TextBoxElement.className=this.Styles["EnabledStyle"][1];
}
if(this.Enabled&&(!this.IsEmptyMessage)&&this.IsNegative()){
this.TextBoxElement.style.cssText=this.OriginalTextBoxCssText+this.UpdateCssText(this.Styles["NegativeStyle"][0]);
this.TextBoxElement.className=this.Styles["NegativeStyle"][1];
}
if(this.Enabled&&this.IsEmptyMessage){
this.TextBoxElement.style.cssText=this.OriginalTextBoxCssText+this.UpdateCssText(this.Styles["EmptyMessageStyle"][0]);
this.TextBoxElement.className=this.Styles["EmptyMessageStyle"][1];
}
if(this.Hovered){
this.TextBoxElement.style.cssText=this.OriginalTextBoxCssText+this.UpdateCssText(this.Styles["HoveredStyle"][0]);
this.TextBoxElement.className=this.Styles["HoveredStyle"][1];
}
if(this.Focused){
this.TextBoxElement.style.cssText=this.OriginalTextBoxCssText+this.UpdateCssText(this.Styles["FocusedStyle"][0]);
this.TextBoxElement.className=this.Styles["FocusedStyle"][1];
}
if(this.Invalid){
this.TextBoxElement.style.cssText=this.OriginalTextBoxCssText+this.UpdateCssText(this.Styles["InvalidStyle"][0]);
this.TextBoxElement.className=this.Styles["InvalidStyle"][1];
}
if(!this.Enabled){
this.TextBoxElement.style.cssText=this.OriginalTextBoxCssText+this.UpdateCssText(this.Styles["DisabledStyle"][0]);
this.TextBoxElement.className=this.Styles["DisabledStyle"][1];
}
},UpdateCssText:function(_36){
var _37=_36.split(";");
var i;
var _39="";
for(i=0;i<_37.length;i++){
var _3a=_37[i].split(":");
if(_3a.length==2){
var _3b=""+_3a[0].toLowerCase();
if(_3b!="width"&&_3b!="height"){
_39+=_37[i]+";";
}
}
}
return _39;
},CalculateSelection:function(){
if(window.opera||!document.selection){
this.SelectionEnd=this.TextBoxElement.selectionEnd;
this.SelectionStart=this.TextBoxElement.selectionStart;
return;
}
var s1=document.selection.createRange();
if(s1.parentElement()!=this.TextBoxElement){
return;
}
var s=s1.duplicate();
s.move("character",-this.TextBoxElement.value.length);
s.setEndPoint("EndToStart",s1);
var _3e=s.text.length;
var _3f=s.text.length+s1.text.length;
this.SelectionEnd=Math.max(_3e,_3f);
this.SelectionStart=Math.min(_3e,_3f);
},ApplySelection:function(){
if(window.opera||!document.selection){
this.TextBoxElement.selectionStart=this.SelectionStart;
this.TextBoxElement.selectionEnd=this.SelectionEnd;
return;
}
this.TextBoxElement.select();
sel=document.selection.createRange();
sel.collapse();
sel.moveStart("character",this.SelectionStart);
sel.collapse();
sel.moveEnd("character",this.SelectionEnd-this.SelectionStart);
sel.select();
},SelectText:function(_40,end){
this.SelectionStart=_40;
this.SelectionEnd=end;
this.ApplySelection();
},SelectAllText:function(){
if(this.TextBoxElement.value.length>0){
this.SelectText(0,this.TextBoxElement.value.length);
return true;
}
return false;
},SetCaretPosition:function(_42){
this.SelectionStart=_42;
this.SelectionEnd=_42;
this.ApplySelection();
},UpdateHiddenValue:function(){
return this.SetHiddenValue(this.TextBoxElement.value);
},InitializeButtons:function(){
this.Button=null;
var _43=document.getElementById(this.WrapperElementID);
var _44=_43.getElementsByTagName("a");
for(i=0;i<_44.length;i++){
if(_44[i].className.indexOf("gobutton")!=(-1)){
this.Button=_44[i];
this.AttachDomEvent(this.Button,"click","ButtonClickHandler");
}
}
},ButtonClickHandler:function(e){
var _46={"ButtonName":"Button"};
this.RaiseEvent("OnButtonClick",_46);
},SetHiddenValue:function(_47){
if(this.HiddenElement.value!=_47.toString()){
this.HiddenElement.value=_47;
}
this.SetValidationField(_47);
return true;
},SetValidationField:function(_48){
},GetValidationField:function(){
return this.HiddenElement;
},ClearHiddenValue:function(){
this.HiddenElement.value="";
},ValueChangedEventArgs:function(_49,_4a){
if(_4a==null){
_4a=this.HiddenElement.value;
}
return {"NewValue":_49,"OldValue":_4a};
},GetValue:function(){
return this.HiddenElement.value;
},GetDisplayValue:function(){
return this.HiddenElement.value;
},GetEditValue:function(){
return this.HiddenElement.value;
},IsEmpty:function(){
return this.HiddenElement.value=="";
},IsNegative:function(){
return false;
},IsReadOnly:function(){
return this.TextBoxElement.readOnly||!this.Enabled;
}};
if(typeof (window.RadControlsNamespace)=="undefined"){
window.RadControlsNamespace=new Object();
}
RadControlsNamespace.AppendStyleSheet=function(_4b,_4c,_4d){
if(!_4d){
return;
}
var _4e=window.netscape&&!window.opera;
if(!_4b&&_4e){
document.write("<"+"link"+" rel='stylesheet' type='text/css' href='"+_4d+"' />");
}else{
var _4f=document.createElement("link");
_4f.rel="stylesheet";
_4f.type="text/css";
_4f.href=_4d;
document.getElementsByTagName("head")[0].appendChild(_4f);
}
};
if(typeof (WebForm_AutoFocus)=="function"){
WebForm_AutoFocus=function(_50){
var _51=document.getElementById(_50);
var _52=false;
if((_51.style)&&(typeof (_51.style.visibility)!="undefined")&&(_51.style.visibility=="hidden")&&(typeof (_51.style.width)!="undefined")&&((_51.style.width=="0px")||(_51.style.width=="0pt"))&&(_51.tagName.toLowerCase()=="input")){
_52=true;
}
if(_52){
_50=_50+"_text";
}
var _53;
if(__nonMSDOMBrowser){
_53=document.getElementById(_50);
}else{
_53=document.all[_50];
}
var _54=_53;
if(_53&&(!WebForm_CanFocus(_53))){
_54=WebForm_FindFirstFocusableChild(_53);
}
if(_54){
try{
_54.focus();
if(__nonMSDOMBrowser){
_54.scrollIntoView(false);
}
if(window.__smartNav){
window.__smartNav.ae=_54.id;
}
}
catch(e){
}
}
};
};if(typeof (Telerik)=="undefined"){
Telerik={};
}
if(Telerik.TextInputEvents==null){
Telerik.TextInputEvents={};
}
Telerik.TextInputEvents.ValueListener=function(_1){
this.Owner=_1;
this.EventRequest=null;
};
Telerik.TextInputEvents.ValueListener.prototype={AddChangeEventRequest:function(_2,_3){
if(this.EventRequest==null){
this.EventRequest={New:_2,Old:_3};
}else{
this.EventRequest.New=_2;
}
},QueueChangeEventRequest:function(_4,_5){
this.CancelPreviousRequest();
this.AddChangeEventRequest(_4,_5);
var _6=this;
var _7=function(){
_6.ValueChangedAction=null;
_6.ProcessEvents();
};
if(this.Owner.DelayValueChangedEvent()){
this.ValueChangedAction=window.setTimeout(_7,300);
}else{
_7();
}
},CancelPreviousRequest:function(){
if(this.ValueChangedAction!=null){
window.clearTimeout(this.ValueChangedAction);
this.ValueChangedAction=null;
}
},Dispose:function(){
this.CancelPreviousRequest();
},ProcessEvents:function(){
if(this.EventRequest!=null){
this.Owner.RaiseValueChangedEvent(this.EventRequest.New,this.EventRequest.Old);
this.EventRequest=null;
}
}};;//BEGIN_ATLAS_NOTIFY
if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
