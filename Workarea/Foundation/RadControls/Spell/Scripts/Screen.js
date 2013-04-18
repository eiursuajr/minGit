if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
if(typeof (window.RadControlsNamespace.Screen)=="undefined"||typeof (window.RadControlsNamespace.Screen.Version)==null||window.RadControlsNamespace.Screen.Version<1.1){
window.RadControlsNamespace.Screen={Version:1.1,GetViewPortSize:function(){
var _1=0;
var _2=0;
var _3=document.body;
if(RadControlsNamespace.Browser.StandardsMode&&!RadControlsNamespace.Browser.IsSafari){
_3=document.documentElement;
}
if(RadControlsNamespace.Browser.IsMozilla&&document.compatMode!="CSS1Compat"){
_3=document.body;
}
if(window.innerWidth){
_1=window.innerWidth;
_2=window.innerHeight;
}else{
_1=_3.clientWidth;
_2=_3.clientHeight;
}
_1+=_3.scrollLeft;
_2+=_3.scrollTop;
return {width:_1-6,height:_2-6};
},GetElementPosition:function(el){
var _5=null;
var _6={x:0,y:0};
var _7;
if(el.getBoundingClientRect){
_7=el.getBoundingClientRect();
var _8=document.documentElement.scrollTop||document.body.scrollTop;
var _9=document.documentElement.scrollLeft||document.body.scrollLeft;
_6.x=_7.left+_9-2;
_6.y=_7.top+_8-2;
return _6;
}else{
if(document.getBoxObjectFor){
try{
_7=document.getBoxObjectFor(el);
_6.x=_7.x-2;
_6.y=_7.y-2;
}
catch(e){
}
}else{
_6.x=el.offsetLeft;
_6.y=el.offsetTop;
_5=el.offsetParent;
if(_5!=el){
while(_5){
_6.x+=_5.offsetLeft;
_6.y+=_5.offsetTop;
_5=_5.offsetParent;
}
}
}
}
if(window.opera){
_5=el.offsetParent;
while(_5&&_5.tagName.toLowerCase()!="body"&&_5.tagName.toLowerCase()!="html"){
_6.x-=_5.scrollLeft;
_6.y-=_5.scrollTop;
_5=_5.offsetParent;
}
}else{
_5=el.parentNode;
while(_5&&_5.tagName.toLowerCase()!="body"&&_5.tagName.toLowerCase()!="html"){
_6.x-=_5.scrollLeft;
_6.y-=_5.scrollTop;
_5=_5.parentNode;
}
}
return _6;
},ElementOverflowsTop:function(_a){
return this.GetElementPosition(_a).y<0;
},ElementOverflowsLeft:function(_b){
return this.GetElementPosition(_b).x<0;
},ElementOverflowsBottom:function(_c,_d){
var _e=this.GetElementPosition(_d).y+RadControlsNamespace.Box.GetOuterHeight(_d);
return _e>_c.height;
},ElementOverflowsRight:function(_f,_10){
var _11=this.GetElementPosition(_10).x+RadControlsNamespace.Box.GetOuterWidth(_10);
return _11>_f.width;
}};
}

//BEGIN_ATLAS_NOTIFY
// Ektron if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
