function RadTicker(_1){
var _2=window[_1];
if(_2&&typeof (_2.Dispose)=="function"){
_2.Dispose();
}
_2=null;
this.ClientID=_1;
this.ControlElement=document.getElementById(_1);
this.CurrentLine=0;
this.CurrentLength=0;
var _3=this;
this.UnLoadHandler=function(){
_3.Dispose();
};
this.AttachEvent(window,"unload",_3.UnLoadHandler);
if(window.OnCallbackRequestStart){
var _4=this;
var _5=window.OnCallbackRequestStart;
window.OnCallbackRequestStart=function(){
_4.ClearTimeouts();
_5();
};
}
}
RadTicker.prototype.Dispose=function(){
this.disposed=true;
try{
this.ClearTimeouts();
this.DetachEvent(window,"unload",this.UnLoadHandler);
this.UnLoadHandler=null;
this.ControlElement=null;
}
catch(error){
}
};
RadTicker.prototype.AttachEvent=function(_6,_7,_8){
try{
if(_6.attachEvent){
_6.attachEvent("on"+_7,_8);
}else{
_6.addEventListener(_7,_8,true);
}
}
catch(error){
}
};
RadTicker.prototype.DetachEvent=function(_9,_a,_b){
if(_9==null||_a==null||_b==null){
return;
}
try{
if(_9.detachEvent){
_9.detachEvent("on"+_a,_b);
}else{
_9.removeEventListener(_a,_b,true);
}
}
catch(error){
}
};
RadTicker.prototype.Start=function(){
if(this.AutoStart){
this.StartTicker();
}
};
RadTicker.prototype.StartTicker=function(){
this.Reset=1;
this.TickLine(0);
};
RadTicker.prototype.TickNextLine=function(){
this.TickLine(this.CurrentLine);
};
RadTicker.prototype.TickLine=function(_c){
if(this.disposed==true){
return;
}
this.CurrentLength=0;
this.ControlElement.innerHTML="";
this.CurrentLine=_c;
this.TickOne(_c);
};
RadTicker.prototype.ResetTicker=function(){
this.CurrentLength=0;
this.ClearTimeouts();
this.ControlElement.innerHTML="";
return;
};
RadTicker.prototype.TrimString=function(_d){
return _d.replace(/^\s{1,}/ig,"").replace(/\s{1,}$/ig,"");
};
RadTicker.prototype.TickOne=function(_e){
if(this.disposed==true){
return;
}
var _f=this.TrimString(this["TickerLine"+_e]);
var _10=_f.length;
var _11=this.CurrentLength;
if(_11<_10){
var _12=this.ControlElement.innerHTML;
if(_f.charAt(_11)=="&"){
_12=_12+"&amp;";
}else{
if(_f.charAt(_11)==" "&&_11+1<_10&&_f.charAt(_11+1)==" "){
_12=_12+" "+"&nbsp;";
this.CurrentLength++;
}else{
_12=_12+_f.charAt(_11);
}
}
this.ControlElement.innerHTML=_12;
this.CurrentLength++;
var _13=this;
this.tickTimeOut=window.setTimeout(function(){
_13.TickOne(_e);
},this.TickSpeed);
}else{
this.OnLineEnd();
}
};
RadTicker.prototype.ClearTimeouts=function(_14){
window.clearTimeout(this.tickTimeOut);
window.clearTimeout(this.lineTimeOut);
};
RadTicker.prototype.OnLineEnd=function(_15){
this.CurrentLength=0;
var _16=(this.CurrentLine+1)%this.TickerLines;
if(_16<=this.CurrentLine&&!this.Loop){
this.OnTickerEnd();
return;
}else{
this.CurrentLine=_16;
}
if(this.AutoAdvance){
var _17=this;
this.lineTimeOut=window.setTimeout(function(){
_17.TickLine(_16);
},this.LineDuration);
}
};
RadTicker.prototype.OnTickerEnd=function(){
if(this.OnTickerEndCode){
eval(this.OnTickerEndCode);
}
};

//BEGIN_ATLAS_NOTIFY
if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
