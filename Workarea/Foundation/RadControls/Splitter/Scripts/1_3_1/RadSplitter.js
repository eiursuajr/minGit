if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
if(typeof (window.RadControlsNamespace.Box)=="undefined"||typeof (window.RadControlsNamespace.Box.Version)==null||window.RadControlsNamespace.Box.Version<2){
window.RadControlsNamespace.Box={Version:2,GetOuterWidth:function(_1){
return _1.offsetWidth;
},GetOuterHeight:function(_2){
return _2.offsetHeight;
},SetOuterHeight:function(_3,_4){
if(_4<=0||_4==""){
_3.style.height="";
}else{
_3.style.height=_4+"px";
var _5=_3.offsetHeight-_4;
var _6=_4-_5;
if(_6>0){
_3.style.height=_6+"px";
}else{
_3.style.height="";
}
}
},SetOuterWidth:function(_7,_8){
if(_8<=0||_8==""){
_7.style.width="";
}else{
_7.style.width=_8+"px";
var _9=_7.offsetWidth-_8;
var _a=_8-_9;
if(_a>0){
_7.style.width=_a+"px";
}else{
_7.style.width="";
}
return _a;
}
},GetPropertyValue:function(_b,_c){
var _d=this.GetStyle(_b);
return this.GetStyleValues(_d,_c);
},GetStyle:function(_e){
if(document.defaultView&&document.defaultView.getComputedStyle){
return document.defaultView.getComputedStyle(_e,null);
}else{
if(_e.currentStyle){
return _e.currentStyle;
}else{
return _e.style;
}
}
}};
};if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
if(typeof (window.RadControlsNamespace.Browser)=="undefined"||typeof (window.RadControlsNamespace.Browser.Version)==null||window.RadControlsNamespace.Browser.Version<1){
window.RadControlsNamespace.Browser={Version:1};
window.RadControlsNamespace.Browser.ParseBrowserInfo=function(){
this.IsMacIE=(navigator.appName=="Microsoft Internet Explorer")&&((navigator.userAgent.toLowerCase().indexOf("mac")!=-1)||(navigator.appVersion.toLowerCase().indexOf("mac")!=-1));
this.IsSafari=(navigator.userAgent.toLowerCase().indexOf("safari")!=-1);
this.IsSafari3=(this.IsSafari&&navigator.userAgent.toLowerCase().indexOf("ersion/3.")!=-1);
this.IsMozilla=window.netscape&&!window.opera;
this.IsNetscape=/Netscape/.test(navigator.userAgent);
this.IsOpera=window.opera;
this.IsOpera9=window.opera&&(parseInt(window.opera.version())>8);
this.IsIE=!this.IsMacIE&&!this.IsMozilla&&!this.IsOpera&&!this.IsSafari;
this.IsIE7=/MSIE 7/.test(navigator.appVersion);
this.StandardsMode=this.IsSafari||this.IsOpera9||this.IsMozilla||document.compatMode=="CSS1Compat";
this.IsMac=/Mac/.test(navigator.userAgent);
};
RadControlsNamespace.Browser.ParseBrowserInfo();
};if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
if(typeof (window.RadControlsNamespace.Log)=="undefined"||typeof (window.RadControlsNamespace.Log.Version)==null||window.RadControlsNamespace.Log.Version<1){
RadControlsNamespace.LL={DEBUG:4,INFO:3,WARN:2,ERROR:1,NONE:0};
RadControlsNamespace.Log=function(ll,_2){
this.Version=1;
this.LogLevel=ll;
var _3=this;
this.__debug_console_buffer="";
if(this.LogLevel!=RadControlsNamespace.LL.NONE&&_2){
window.onerror=function(_4,_5,_6){
_3.Error(["JS error. Msg[%s], url[%s], line[%d]",_4,_5,_6]);
return false;
};
}
this.Write=function(_7,ll){
try{
this.__debug_console_element_id="__debug_console__";
if(ll>this.LogLevel){
return;
}
var d=new Date();
var _a=d.getHours()+":"+d.getMinutes()+":"+d.getSeconds()+"."+d.getMilliseconds();
if(typeof (console)!="undefined"&&!document.all){
for(var i=0;i<_7.length;i++){
_7[i]="\""+_7[i]+"\"";
}
_7[0]="\"["+_a+"] "+_7[0].substr(1);
var _c=_7.join(",");
var _d="info";
switch(ll){
case RadControlsNamespace.LL.DEBUG:
_d="debug";
break;
case RadControlsNamespace.LL.WARN:
_d="warn";
break;
case RadControlsNamespace.LL.ERROR:
_d="error";
break;
case RadControlsNamespace.LL.INFO:
default:
_d="info";
}
eval("console."+_d+"("+_c+")");
}else{
var _e=document.getElementById(this.__debug_console_element_id);
if(_e&&_e.style.display=="none"){
return;
}
_7[0]="["+_a+"] "+_7[0];
var _c=_7[0];
for(var i=1;i<_7.length;i++){
_c=_c.replace(/\%\w/,"<b>"+_7[i]+"</b>");
}
var _f=_c;
_f=_f.replace(/\\n/,"<br>");
switch(ll){
case RadControlsNamespace.LL.ERROR:
_f="<font style=\"background-color:#ff0000\">"+_f+"</font>";
case RadControlsNamespace.LL.WARN:
_f="<font style=\"background-color:#ff9999\">"+_f+"</font>";
}
if(document.readyState!="complete"){
this.__debug_console_buffer=_f+"<br>"+this.__debug_console_buffer;
return;
}
if(_e==null){
var _10=document.createElement("DIV");
var _11=0;
var top=document.body.clientHeight-215;
_10.innerHTML="<a href=\"javascript:void(null)\" onclick = \"javascript: var left = document.getElementById('"+this.__debug_console_element_id+"_left').value;var t = document.getElementById('"+this.__debug_console_element_id+"').parentElement;t.style.left=left;var r=5;\">left</a>:<input id=\""+this.__debug_console_element_id+"_left\" style=\"width:30;height:10;font-size:9px;\" value=\""+_11+"\"> <a href=\"javascript:void(null)\" onclick = \"javascript: var top = document.getElementById('"+this.__debug_console_element_id+"_top').value;var t = document.getElementById('"+this.__debug_console_element_id+"').parentElement;t.style.top=top;var r=5;\">top</a>: <input id=\""+this.__debug_console_element_id+"_top\" style=\"width:30;height:10;font-size:9px;\" value=\""+top+"\"> <a href = \"javascript:var t = document.getElementById('"+this.__debug_console_element_id+"').innerHTML = ''\">clear</a> <a href = \"javascript:var t = document.getElementById('"+this.__debug_console_element_id+"').style.display = 'none'\">hide</a> <a href = \"javascript:var t = document.getElementById('"+this.__debug_console_element_id+"').style.display = ''\">show</a> <a href = \"javascript:var t = document.getElementById('"+this.__debug_console_element_id+"').parentElement.style.display='none'\">close</a> ";
_10.style.width="1000";
_10.style.height=200;
_10.style.position="absolute";
_10.style.font="10px Verdana";
_10.style.lineHeight="1.5";
_10.style.top=top;
_10.style.left=_11;
_10.style.textAlign="left";
_10.style.border="2px solid #aaaaaa";
_10.style.backgroundColor="#dddddd";
_e=document.createElement("DIV");
_e.id=this.__debug_console_element_id;
_e.style.width="100%";
_e.style.height=200;
_e.style.overflow="auto";
_e.style.border="1px solid #aaaaaa";
_e.style.textAlign="left";
_e.style.backgroundColor="#eeeeee";
_10.appendChild(_e);
document.body.appendChild(_10);
}
if(this.__debug_console_buffer){
this.__debug_console_buffer="<br><b>------------- FROM BUFFER (before onload) -------------</b><br>"+this.__debug_console_buffer;
}
_e.innerHTML=_f+"<br>"+_e.innerHTML.substr(0,10000)+this.__debug_console_buffer;
this.__debug_console_buffer="";
}
}
catch(e){
if(currentLL==RadControlsNamespace.LL.DEBUG){
alert("this.Debug.js error ->"+e);
}
}
};
this.Debug=function(_13){
this.Write(_13,RadControlsNamespace.LL.DEBUG);
};
this.Error=function(_14){
this.Write(_14,RadControlsNamespace.LL.ERROR);
};
this.Warning=function(_15){
this.Write(_15,RadControlsNamespace.LL.WARN);
};
};
};if(typeof window.RadControlsNamespace=="undefined"){
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
};if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
if(typeof (window.RadControlsNamespace.JSON)=="undefined"||typeof (window.RadControlsNamespace.JSON.Version)==null||window.RadControlsNamespace.JSON.Version<1){
window.RadControlsNamespace.JSON={Version:1,copyright:"(c)2005 JSON.org",license:"http://www.crockford.com/JSON/license.html",stringify:function(v,_2){
var a=[];
var _4=arguments[2]||{};
function e(s){
a[a.length]=s;
}
function g(x){
var c,i,l,v;
switch(typeof x){
case "object":
if(x){
if(x instanceof Array){
e("[");
l=a.length;
for(i=0;i<x.length;i+=1){
v=x[i];
if(typeof v!="undefined"&&typeof v!="function"){
if(l<a.length){
e(",");
}
g(v);
}
}
e("]");
return "";
}else{
if(typeof x.valueOf=="function"){
e("{");
l=a.length;
for(i in x){
v=x[i];
if(_2&&v==_2[i]){
continue;
}
var _a=typeof v;
if(_a=="undefined"||_a=="function"){
continue;
}
if(_a=="object"&&!_4[i]){
continue;
}
if(l<a.length){
e(",");
}
g(i);
e(":");
g(v);
}
return e("}");
}
}
}
e("null");
return "";
case "number":
e(isFinite(x)?+x:"null");
return "";
case "string":
l=x.length;
e("\"");
for(i=0;i<l;i+=1){
c=x.charAt(i);
if(c>=" "){
if(c=="\\"||c=="\""){
e("\\");
}
e(c);
}else{
switch(c){
case "\b":
e("\\b");
break;
case "\f":
e("\\f");
break;
case "\n":
e("\\n");
break;
case "\r":
e("\\r");
break;
case "\t":
e("\\t");
break;
default:
c=c.charCodeAt();
e("\\u00"+Math.floor(c/16).toString(16)+(c%16).toString(16));
}
}
}
e("\"");
return "";
case "boolean":
e(String(x));
return "";
default:
e("null");
return "";
}
}
g(v,0);
return a.join("");
},stringifyHashTable:function(_b,_c,_d){
var a=[];
if(!_d){
_d=[];
}
for(var i=0;i<_b.length;i++){
var ser=this.stringify(_b[i],_d[i]);
if(ser=="{}"){
continue;
}
a[a.length]="\""+_b[i][_c]+"\":"+ser;
}
return "{"+a.join(",")+"}";
},parse:function(_11){
return (/^([ \t\r\n,:{}\[\]]|"(\\["\\\/bfnrtu]|[^\x00-\x1f"\\]+)*"|-?\d+(\.\d*)?([eE][+-]?\d+)?|true|false|null)+$/.test(_11))&&eval("("+_11+")");
}};
};function RadSplitter_Pane(_1){
this.Log=new RadControlsNamespace.Log(_1["logLevel"],true);
this.ID=_1["ID"];
this.splitter=_1["splitter"];
this.Index=_1["index"];
this.indexInSplitItems=_1["indexInSplitItems"];
this.width=_1["width"];
if(this.width.indexOf("px")>-1){
this.width=parseInt(this.width,10);
}
this.height=_1["height"];
if(this.height.indexOf("px")>-1){
this.height=parseInt(this.height,10);
}
this.originalDimensions={width:this.width,height:this.height};
if(_1["originalWidth"]!=null){
this.originalDimensions.width=_1["originalWidth"];
}
if(_1["originalHeight"]!=null){
this.originalDimensions.height=_1["originalHeight"];
}
this.persistScrollPosition=_1["persistScrollPosition"];
this.supportedEvents=["OnClientPaneCollapsed","OnClientPaneExpanded","OnClientPaneResized","OnClientBeforePaneCollapse","OnClientBeforePaneExpand","OnClientBeforePaneResize"];
for(var i=0;i<this.supportedEvents.length;i++){
this[this.supportedEvents[i]]=_1[this.supportedEvents[i]];
}
this.contentUrl=_1["contentUrl"];
this.minWidth=_1["minWidth"];
this.maxWidth=_1["maxWidth"];
this.minHeight=_1["minHeight"];
this.maxHeight=_1["maxHeight"];
this.initiallyCollapsed=_1["initiallyCollapsed"];
this.collapsedDirection=_1["collapsedDirection"];
this.isCollapsed=false;
this.expandedSize=0;
if(this.initiallyCollapsed){
this.expandedSize=parseInt(_1["expandedSize"]);
}
this.locked=_1["locked"];
this.stateString="";
this.childSplitter=null;
if(_1["childSplitterID"]){
this.childSplitter=eval(_1["childSplitterID"]);
}
this.stateFieldId=_1["stateFieldId"];
this.enableScrolling=_1["enableScrolling"];
this.scrollLeft=_1["scrollLeft"];
this.scrollTop=_1["scrollTop"];
this.lastCollapseDirection=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
this.box=window.RadControlsNamespace.Box;
this.containerElement=document.getElementById("RAD_SPLITTER_PANE_"+this.ID);
this.contentContainerElement=document.getElementById("RAD_SPLITTER_PANE_CONTENT_"+this.ID);
if(this.splitter.IsVertical()){
this.getVarSize=this.GetWidth;
this.setVarSize=this.SetWidth;
this.getOrigVarSize=this.getOrigWidth;
this.setOrigVarSize=this.setOrigWidth;
this.getVarMinSize=this.GetMinWidth;
this.getVarMaxSize=this.GetMaxWidth;
this.getAvailIncreaseDelta=this.getWidthAvailIncreaseDelta;
this.getAvailDecreaseDelta=this.getWidthAvailDecreaseDelta;
}else{
this.getVarSize=this.GetHeight;
this.setVarSize=this.SetHeight;
this.getOrigVarSize=this.getOrigHeight;
this.setOrigVarSize=this.setOrigHeight;
this.getVarMinSize=this.GetMinHeight;
this.getVarMaxSize=this.GetMaxHeight;
this.getAvailIncreaseDelta=this.getHeightAvailIncreaseDelta;
this.getAvailDecreaseDelta=this.getHeightAvailDecreaseDelta;
}
window[this.ID]=this;
}
RadSplitter_Pane.prototype.Init=function(){
if(this.Inited){
return;
}
this.Log.Debug(["Pane.Init start ID[%d]",this.ID]);
if(this.IsExternalContent()){
this.setExternalContent(this.contentUrl);
}
RadControlsNamespace.DomEventMixin.Initialize(this);
var _3=this.GetContainerElement();
var _4=this.GetContentContainerElement();
if(!this.IsExternalContent()){
this.AttachDomEvent(_4,"scroll","OnScroll");
}
RadControlsNamespace.EventMixin.Initialize(this);
var _5=this.supportedEvents;
_3=null;
if(this.initiallyCollapsed){
var _6=this;
var _7=function(){
_6.doInitialCollapse();
};
this.splitter.AttachEvent("OnClientLoaded",_7);
}
if(this.enableScrolling&&this.persistScrollPosition){
var _6=this;
var _8=function(){
_6.SetScrollPos(_6.scrollLeft,_6.scrollTop);
};
this.splitter.AttachEvent("OnClientLoaded",_8);
}
this.SaveState();
this.Inited=true;
this.Log.Debug(["Pane.Init end ID[%d]",this.ID]);
};
RadSplitter_Pane.prototype.SaveState=function(){
var _9=document.getElementById(this.stateFieldId);
var _a=this.GetScrollPos();
var _b={"originalWidth":this.originalDimensions.width,"originalHeight":this.originalDimensions.height,"width":this.GetWidth(),"height":this.GetHeight(),"collapsed":this.IsCollapsed(),"collapsedDirection":this.lastCollapseDirection,"scrollLeft":_a.left,"scrollTop":_a.top,"expandedSize":this.expandedSize,"contentUrl":this.GetContentUrl(),"minWidth":this.minWidth,"maxWidth":this.maxWidth,"minHeight":this.minHeight,"maxHeight":this.maxHeight,"locked":this.locked};
var _c=RadControlsNamespace.JSON.stringify(_b);
this.stateString=_c;
_9.value=_c;
};
RadSplitter_Pane.prototype.GetState=function(){
return this.stateString;
};
RadSplitter_Pane.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
};
RadSplitter_Pane.prototype.Resize=function(_d,_e){
this.splitter.resizePanes(_d,this,_e);
};
RadSplitter_Pane.prototype.Print=function(_f){
var _10="width="+this.GetWidth()+"px, height="+this.GetHeight()+"px, scrollbars=1";
var _11=(this.IsExternalContent())?this.GetContentUrl():"about:blank";
var _12=window.open(_11,"",_10,false);
if(this.IsExternalContent()){
try{
var t=function(){
_12.print();
};
setTimeout(t,1000);
}
catch(e){
}
return;
}
var _14="";
if(_f){
_14="<head>";
for(var i=0;i<_f.length;i++){
_14+="<link href = '"+_f[i]+"' rel='stylesheet' type='text/css'></link>";
}
_14+="</head>";
}
var _16=_14+"<body>"+this.GetContent()+"</body>";
_12.document.open();
_12.document.write(_16);
_12.document.close();
_12.print();
};
RadSplitter_Pane.prototype.Collapse=function(_17){
if(this.IsCollapsed()){
return true;
}
if(!_17){
_17=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
}
var _18=this.getTargetSplitBar(_17);
var _19=false;
if(_18!=null){
var _1a=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
if(_18.indexInSplitItems<this.indexInSplitItems){
_1a=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward;
}
_19=_18.collapseTargetPane(_1a);
}else{
_19=this.splitter.collapsePane(this,_17);
}
return _19;
};
RadSplitter_Pane.prototype.Expand=function(_1b){
if(!this.IsCollapsed()){
return true;
}
if(!_1b){
_1b=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
}
var _1c=this.getTargetSplitBar(_1b);
var _1d=false;
if(_1c!=null){
var _1e=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
if(_1c.indexInSplitItems<this.indexInSplitItems){
_1e=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward;
}
_1d=_1c.collapseTargetPane(_1e);
}else{
_1d=this.splitter.expandPane(this,_1b);
}
return _1d;
};
RadSplitter_Pane.prototype.OnScroll=function(){
if(this.persistScrollPosition){
this.SaveState();
}
};
RadSplitter_Pane.prototype.SetContent=function(_1f){
if(this.IsExternalContent()){
var _20=this.GetContentContainerElement();
_20.style.overflow=this.contentOverflow.overflow;
_20.style.overflowX=this.contentOverflow.overflowX;
_20.style.overflowY=this.contentOverflow.overflowY;
this.contentUrl=null;
this.SaveState();
}
var _21=this.GetContentContainerElement();
_21.innerHTML=_1f;
};
RadSplitter_Pane.prototype.GetContent=function(){
if(this.IsExternalContent()){
return "";
}
var _22=this.GetContentContainerElement();
return _22.innerHTML;
};
RadSplitter_Pane.prototype.SetContentUrl=function(url){
this.setExternalContent(url);
this.contentUrl=url;
this.SaveState();
};
RadSplitter_Pane.prototype.GetContentUrl=function(){
if(!this.IsExternalContent()){
return "";
}
return this.contentUrl;
};
RadSplitter_Pane.prototype.IsExternalContent=function(){
return (this.contentUrl!=null&&this.contentUrl!="");
};
RadSplitter_Pane.prototype.SetMinWidth=function(_24){
this.minWidth=_24;
this.SaveState();
};
RadSplitter_Pane.prototype.GetMinWidth=function(){
var _25=this.minWidth;
var _26=0;
if(this.IsSplitterContainer()&&this.childSplitter.IsVertical()){
_26=this.childSplitter.GetMinWidth();
}
return Math.max(_25,_26);
};
RadSplitter_Pane.prototype.SetMinHeight=function(_27){
this.minHeight=_27;
this.SaveState();
};
RadSplitter_Pane.prototype.GetMinHeight=function(){
var _28=this.minHeight;
var _29=0;
if(this.IsSplitterContainer()&&!this.childSplitter.IsVertical()){
_29=this.childSplitter.GetMinHeight();
}
return Math.max(_28,_29);
};
RadSplitter_Pane.prototype.SetMaxWidth=function(_2a){
this.maxWidth=_2a;
this.SaveState();
};
RadSplitter_Pane.prototype.GetMaxWidth=function(){
var _2b=this.getMaxSize(true);
this.Log.Debug(["Pane.GetMaxWidth maxWidth[%d], paneID[%d]",_2b,this.ID]);
return _2b;
};
RadSplitter_Pane.prototype.SetMaxHeight=function(_2c){
this.maxHeight=_2c;
this.SaveState();
};
RadSplitter_Pane.prototype.GetMaxHeight=function(){
var _2d=this.getMaxSize(false);
this.Log.Debug(["Pane.GetMaxHeight maxHeight[%d], paneID[%d]",_2d,this.ID]);
return _2d;
};
RadSplitter_Pane.prototype.SetWidth=function(_2e){
if(_2e<0||_2e==this.width){
return;
}
this.setWidth(_2e);
if(this.IsSplitterContainer()){
this.childSplitter.SetWidth(this.GetInnerWidth());
}
};
RadSplitter_Pane.prototype.SetHeight=function(_2f){
if(_2f<0||_2f==this.height){
return;
}
this.setHeight(_2f);
if(this.IsSplitterContainer()){
this.childSplitter.SetHeight(this.GetInnerHeight());
}
};
RadSplitter_Pane.prototype.GetInnerWidth=function(_30){
return this.width;
if(this.IsSplitterContainer()){
return this.width;
}
if(this.width>2*this.splitter.panesBorderSize){
return this.width-2*this.splitter.panesBorderSize;
}
return 0;
};
RadSplitter_Pane.prototype.GetInnerHeight=function(_31){
return this.height;
if(this.IsSplitterContainer()){
return this.height;
}
if(this.height>2*this.splitter.panesBorderSize){
return this.height-2*this.splitter.panesBorderSize;
}
return 0;
};
RadSplitter_Pane.prototype.GetWidth=function(){
return this.width;
};
RadSplitter_Pane.prototype.GetHeight=function(){
return this.height;
};
RadSplitter_Pane.prototype.GetSplitter=function(){
return this.splitter;
};
RadSplitter_Pane.prototype.IsSplitterContainer=function(){
return (this.childSplitter!=null&&document.getElementById("RAD_SPLITTER_"+this.childSplitter.ID)!=null);
};
RadSplitter_Pane.prototype.GetContainerElement=function(){
return this.containerElement;
};
RadSplitter_Pane.prototype.GetContentContainerElement=function(){
return this.contentContainerElement;
};
RadSplitter_Pane.prototype.GetExtContentContainerElement=function(){
return this.extContentContainerElement;
};
RadSplitter_Pane.prototype.IsLocked=function(){
return this.locked;
};
RadSplitter_Pane.prototype.Lock=function(){
this.locked=true;
this.SaveState();
};
RadSplitter_Pane.prototype.UnLock=function(){
this.locked=false;
this.SaveState();
};
RadSplitter_Pane.prototype.IsCollapsed=function(){
return this.isCollapsed;
};
RadSplitter_Pane.prototype.IsExpanded=function(){
return !this.IsCollapsed();
};
RadSplitter_Pane.prototype.ResetSize=function(){
this.setWidth(this.originalDimensions.width);
this.setHeight(this.originalDimensions.height);
};
RadSplitter_Pane.prototype.IsFixedSize=function(){
return this.isFixedUnit(this.getVarSize());
};
RadSplitter_Pane.prototype.IsFreeSize=function(){
var _32=this.getVarSize();
if(_32==""){
return true;
}
return false;
};
RadSplitter_Pane.prototype.IsInitialFreeSize=function(){
var _33=this.getOrigVarSize();
if(_33==""){
return true;
}
return false;
};
RadSplitter_Pane.prototype.SetFreeSize=function(){
if(this.splitter.IsVertical()){
this.originalDimensions.width="";
}else{
this.originalDimensions.height="";
}
};
RadSplitter_Pane.prototype.IsPercentSize=function(){
var _34=this.getVarSize();
if(_34.toString().indexOf("%")>-1){
return true;
}
return false;
};
RadSplitter_Pane.prototype.isFixedUnit=function(_35){
if(!_35||_35.toString().indexOf("%")>-1){
return false;
}
return true;
};
RadSplitter_Pane.prototype.GetScrollPos=function(){
var _36=this.GetContentContainerElement();
return {left:_36.scrollLeft,top:_36.scrollTop};
};
RadSplitter_Pane.prototype.SetScrollPos=function(_37,_38){
var _39=this.GetContentContainerElement();
_39.scrollLeft=_37;
_39.scrollTop=_38;
};
RadSplitter_Pane.prototype.getWidthAvailDecreaseDelta=function(){
if(this.IsCollapsed()||this.IsLocked()){
return 0;
}
var _3a=this.GetWidth()-this.GetMinWidth();
this.Log.Debug(["Pane.getWidthAvailDecreaseDelta delta[%d] pane[%d]",_3a,this.ID]);
return _3a;
};
RadSplitter_Pane.prototype.getWidthAvailIncreaseDelta=function(){
if(this.IsCollapsed()||this.IsLocked()){
return 0;
}
var _3b=this.GetMaxWidth()-this.GetWidth();
this.Log.Debug(["Pane.getWidthAvailIncreaseDelta delta[%d] pane[%d]",_3b,this.ID]);
return _3b;
};
RadSplitter_Pane.prototype.getHeightAvailDecreaseDelta=function(){
if(this.IsCollapsed()||this.IsLocked()){
return 0;
}
return this.GetHeight()-this.GetMinHeight();
};
RadSplitter_Pane.prototype.getHeightAvailIncreaseDelta=function(){
if(this.IsCollapsed()||this.IsLocked()){
return 0;
}
return this.GetMaxHeight()-this.GetHeight();
};
RadSplitter_Pane.prototype.setHeight=function(_3c){
this.Log.Debug(["Pane.setHeight height[%d] pane[%d]",_3c,this.ID]);
this.setSize(_3c,false);
};
RadSplitter_Pane.prototype.setWidth=function(_3d){
this.Log.Debug(["Pane.setWidth width[%d] pane[%d]",_3d,this.ID]);
this.setSize(_3d,true);
};
RadSplitter_Pane.prototype.setSize=function(_3e,_3f){
_3e=parseInt(_3e,10);
var _40=(_3f)?"width":"height";
if(isNaN(_3e)||_3e==this[_40]){
return;
}
_3e=Math.max(_3e,0);
this[_40]=_3e;
var _41=this.GetContentContainerElement();
var _42=(_3f)?"SetOuterWidth":"SetOuterHeight";
var _43=(_3f)?"GetInnerWidth":"GetInnerHeight";
if(_41){
this.box[_42](_41,this[_43]());
if(this.IsExternalContent()){
var _44=this.extContentContainerElement;
this.box[_42](_44,this[_43]());
}
}
this.box[_42](this.GetContainerElement(),_3e);
this.SaveState();
};
RadSplitter_Pane.prototype.doInitialCollapse=function(){
this.initialCollapseMode=true;
this.Log.Debug(["Pane.doInitialCollapse ID[%s]",this.ID]);
var _45=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
_45=this.collapsedDirection;
var _46=this.getTargetSplitBar(_45);
if(_46!=null){
var _47=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
if(_46.indexInSplitItems<this.indexInSplitItems){
_47=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward;
}
this.Log.Debug(["Pane.doInitialCollapse [splitBar.collapseTargetPane] ID[%s]",this.ID]);
_46.collapseTargetPane(_47);
}else{
this.Log.Debug(["Pane.doInitialCollapse [splitter.collapsePane] ID[%s]",this.ID]);
this.splitter.collapsePane(this,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward);
}
this.initialCollapseMode=false;
};
RadSplitter_Pane.prototype.getOrigWidth=function(){
return this.originalDimensions.width;
};
RadSplitter_Pane.prototype.getOrigHeight=function(){
return this.originalDimensions.height;
};
RadSplitter_Pane.prototype.setOrigWidth=function(_48){
this.originalDimensions.width=_48;
};
RadSplitter_Pane.prototype.setOrigHeight=function(_49){
this.originalDimensions.height=_49;
};
RadSplitter_Pane.prototype.collapse=function(_4a){
this.lastCollapseDirection=_4a;
this.GetContentContainerElement().style.display="none";
if(this.splitter.IsVertical()){
this.GetContainerElement().style.display="none";
}else{
document.getElementById("RAD_SPLITTER_PANE_TR_"+this.ID).style.display="none";
if(document.all&&this.Index==0){
var _4b=this.splitter.GetSplitBarByIndex(0);
if(_4b!=null){
_4b.GetContainerElement().style.borderTop="0px";
}
}
}
this.isCollapsed=true;
if(this.getVarSize()>0&&(!this.initialCollapseMode||!this.expandedSize)){
this.expandedSize=this.getVarSize();
}
if(this.splitter.IsVertical()){
this.width=0;
}else{
this.height=0;
}
this.SaveState();
};
RadSplitter_Pane.prototype.show=function(){
this.GetContentContainerElement().style.display="";
if(this.splitter.IsVertical()){
this.GetContainerElement().style.display="";
}else{
document.getElementById("RAD_SPLITTER_PANE_TR_"+this.ID).style.display="";
}
};
RadSplitter_Pane.prototype.expand=function(_4c){
this.show();
this.isCollapsed=false;
this.setVarSize(_4c);
if(this.splitter.IsVertical()){
this.setHeight(this.splitter.GetInnerHeight());
}else{
this.setWidth(this.splitter.GetInnerWidth());
}
this.callRadShow();
this.Log.Debug(["Pane.expand newSize[%d], ID[%d]",_4c,this.ID]);
this.SaveState();
};
RadSplitter_Pane.prototype.getTargetSplitBar=function(_4d,_4e){
if(typeof (_4e)=="undefined"){
_4e=true;
}
if(!_4d){
_4d=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
}
var _4f=(_4d==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward)?this.indexInSplitItems+1:this.indexInSplitItems-1;
this.Log.Debug(["Pane.getTargetSplitBar direction[%d], fwd[%d], splitBarIndex[%d], ID[%d]",_4d,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward,_4f,this.ID]);
var _50=this.splitter.getSplitBarByItemIndex(_4f);
if(_50!=null&&_50.IsCollapseDirectionEnabled(_4d)){
return _50;
}
if(_4e){
_4d=(_4d==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward)?RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward:RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
return this.getTargetSplitBar(_4d,false);
}
return null;
};
RadSplitter_Pane.prototype.hideContent=function(){
this.GetContentContainerElement().style.display="none";
if(this.IsSplitterContainer()){
var _51=this.childSplitter.GetPanes();
for(var i=0;i<_51.length;i++){
_51[i].hideContent();
}
}
};
RadSplitter_Pane.prototype.showContent=function(){
this.GetContentContainerElement().style.display="";
if(this.IsSplitterContainer()){
var _53=this.childSplitter.GetPanes();
for(var i=0;i<_53.length;i++){
_53[i].showContent();
}
}
};
RadSplitter_Pane.prototype.callRadResize=function(){
RadControlsNamespace.CallRadResize(this.GetContainerElement());
};
RadSplitter_Pane.prototype.callRadShow=function(){
RadControlsNamespace.CallRadShow(this.GetContainerElement());
};
RadSplitter_Pane.prototype.setExternalContent=function(url){
var _56=(this.enableScrolling)?"auto":"no";
var _57=(!document.all)?"margin-bottom:-3px;":"";
var _58="<iframe id='RAD_SPLITTER_PANE_EXT_CONTENT_"+this.ID+"' name='"+this.ID+"' src='"+url+"' frameborder=0 scrolling='"+_56+"' style='"+_57+"'></iframe>";
this.GetContentContainerElement().innerHTML=_58;
this.extContentContainerElement=document.getElementById("RAD_SPLITTER_PANE_EXT_CONTENT_"+this.ID);
var _59=this.GetInnerWidth();
var _5a=this.GetInnerHeight();
if(this.isFixedUnit(_59)){
this.box.SetOuterWidth(this.extContentContainerElement,_59);
}
if(this.isFixedUnit(_5a)){
this.box.SetOuterHeight(this.extContentContainerElement,_5a);
}
var _5b=this.GetContentContainerElement();
this.contentOverflow={overflow:_5b.style.overflow,overflowX:_5b.style.overflowX,overflowY:_5b.style.overflowY};
_5b.style.overflow="hidden";
_5b.style.overflowX="hidden";
_5b.style.overflowY="hidden";
};
RadSplitter_Pane.prototype.getMaxSize=function(_5c){
var _5d=(_5c)?"maxWidth":"maxHeight";
var _5e=(_5c)?this.maxWidth:this.maxHeight;
var _5f=0;
if(this.IsSplitterContainer()){
var _60=false;
if(_5c&&!this.childSplitter.IsVertical()||!_5c&&this.childSplitter.IsVertical()){
_60=true;
}
if(!_60){
_5f=(_5c)?this.childSplitter.GetMaxWidth():this.childSplitter.GetMaxHeight();
if(_5f!=null){
if(_5e!=null){
_5e=Math.min(_5e,_5f);
}else{
_5e=_5f;
}
}
}
}
return _5e;
};;if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
if(typeof (window.RadControlsNamespace.Utils)=="undefined"||typeof (window.RadControlsNamespace.Utils.Version)==null||window.RadControlsNamespace.Utils.Version<1.1){
window.RadControlsNamespace.Utils={Version:1.1};
window.RadControlsNamespace.CallRadResize=function(_1){
var _2=_1.getElementsByTagName("*");
for(var i=0;i<_2.length;i++){
var _4=_2[i];
if(_4.RadResize){
_4.RadResize();
}
if(_4.RadResizeStopLookup){
return;
}
}
};
window.RadControlsNamespace.CallRadShow=function(_5){
var _6=_5.getElementsByTagName("*");
for(var i=0;i<_6.length;i++){
var _8=_6[i];
if(_8.RadShow){
_8.RadShow();
}
if(_8.RadShowStopLookup){
return;
}
}
};
window.RadControlsNamespace.AppendStyleSheet=function(_9,_a,_b){
if(!_b){
return;
}
var _c=window.netscape&&!window.opera;
if(!_9&&_c){
document.write("<"+"link"+" rel='stylesheet' type='text/css' href='"+_b+"' />");
}else{
var _d=document.createElement("link");
_d.rel="stylesheet";
_d.type="text/css";
_d.href=_b;
document.getElementsByTagName("head")[0].appendChild(_d);
}
};
};if(typeof window.RadControlsNamespace=="undefined"){
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
};function RadSplitter_SplitBar(_1){
this.Log=new RadControlsNamespace.Log(_1["logLevel"],true);
this.ID=_1["ID"];
this.resizeStep=_1["resizeStep"];
this.collapseMode=_1["collapseMode"];
this.resizeEnabled=_1["resizeEnabled"];
this.prevPaneId=_1["prevPaneId"];
this.nextPaneId=_1["nextPaneId"];
this.browser=window.RadControlsNamespace.Browser;
this.box=window.RadControlsNamespace.Box;
this.splitter=_1["splitter"];
this.isLiveResize=_1["liveResize"];
this.Index=_1["index"];
this.indexInSplitItems=_1["indexInSplitItems"];
this.collapseImageUrl=_1["collapseImageUrl"];
this.expandImageUrl=_1["expandImageUrl"];
this.collapsed={};
this.collapsed[RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward]=false;
this.collapsed[RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward]=false;
}
RadSplitter_SplitBar.prototype.Init=function(){
this.prevPane=this.splitter.GetPaneById(this.prevPaneId);
this.nextPane=this.splitter.GetPaneById(this.nextPaneId);
RadControlsNamespace.DomEventMixin.Initialize(this);
var _2=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
if(this.IsCollapseDirectionEnabled(_2)){
var _3=this.GetCollapseBarElement(_2);
this.AttachDomEvent(_3,"mousedown","CollapseBarFwdOnMouseDown");
this.AttachDomEvent(_3,"mouseover","CollapseBarFwdOnMouseOver");
this.AttachDomEvent(_3,"mouseout","CollapseBarFwdOnMouseOut");
}
var _2=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward;
if(this.IsCollapseDirectionEnabled(_2)){
var _3=this.GetCollapseBarElement(_2);
this.AttachDomEvent(_3,"mousedown","CollapseBarBackOnMouseDown");
this.AttachDomEvent(_3,"mouseover","CollapseBarBackOnMouseOver");
this.AttachDomEvent(_3,"mouseout","CollapseBarBackOnMouseOut");
}
if(this.resizeEnabled){
var _4=this.GetSplitterBarElement();
this.AttachDomEvent(_4,"mousedown","OnMouseDown");
this.AttachDomEvent(_4,"mouseover","OnMouseOver");
this.AttachDomEvent(_4,"mouseout","OnMouseOut");
}
this.setCursorStyle();
var _5=this.splitter.getSplitBarsSize()/this.splitter.splitBars.length;
var _6=document.getElementById("RAD_SPLITBAR_SPACER_"+this.ID);
if(this.splitter.IsVertical()){
this.box.SetOuterWidth(this.GetContainerElement(),_5);
if(_6){
_6.style.width=_5+"px";
}
}else{
this.box.SetOuterHeight(this.GetContainerElement(),_5);
if(_6){
_6.style.height=_5+"px";
}
}
};
RadSplitter_SplitBar.prototype.IsCollapseDirectionEnabled=function(_7){
if(this.collapseMode==RadSplitterNamespace.RAD_SPLITBAR_COLLAPSE_MODE.Both){
return true;
}
if(_7==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward&&this.collapseMode==RadSplitterNamespace.RAD_SPLITBAR_COLLAPSE_MODE.Forward){
return true;
}
if(_7==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward&&this.collapseMode==RadSplitterNamespace.RAD_SPLITBAR_COLLAPSE_MODE.Backward){
return true;
}
return false;
};
RadSplitter_SplitBar.prototype.IsResizeEnabled=function(){
return this.resizeEnabled;
};
RadSplitter_SplitBar.prototype.GetContainerElement=function(){
return this.GetSplitterBarElement();
};
RadSplitter_SplitBar.prototype.GetSplitterBarElement=function(){
return document.getElementById(this.ID);
};
RadSplitter_SplitBar.prototype.GetCollapseBarElement=function(_8){
var _9="Forward";
if(_8==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward){
_9="Backward";
}
return document.getElementById("RAD_SPLITTER_BAR_COLLAPSE_"+_9+"_"+this.ID);
};
RadSplitter_SplitBar.prototype.IsCollapsed=function(_a){
return this.collapsed[_a];
};
RadSplitter_SplitBar.prototype.GetWidth=function(){
return this.box.GetOuterWidth(this.GetSplitterBarElement());
};
RadSplitter_SplitBar.prototype.GetHeight=function(){
return this.box.GetOuterHeight(this.GetSplitterBarElement());
};
RadSplitter_SplitBar.prototype.CollapseBarFwdOnMouseOut=function(e){
RadControlsNamespace.DomEvent.StopPropagation(e);
var _c=this.GetCollapseBarElement(RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward);
_c.className=(this.splitter.IsVertical())?"collapseBar":"collapseBarHorizontal";
return false;
};
RadSplitter_SplitBar.prototype.CollapseBarFwdOnMouseOver=function(e){
RadControlsNamespace.DomEvent.StopPropagation(e);
var _e=this.GetCollapseBarElement(RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward);
_e.className=(this.splitter.IsVertical())?"collapseBarOver":"collapseBarOverHorizontal";
return false;
};
RadSplitter_SplitBar.prototype.CollapseBarFwdOnMouseDown=function(e){
if(e.button&&e.button!=1){
return true;
}
RadControlsNamespace.DomEvent.PreventDefault(e);
RadControlsNamespace.DomEvent.StopPropagation(e);
this.collapseTargetPane(RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward);
var _10=this;
var _11=function(){
_10.CollapseBarFwdOnMouseOut();
};
setTimeout(_11,10);
return false;
};
RadSplitter_SplitBar.prototype.CollapseBarBackOnMouseOut=function(e){
RadControlsNamespace.DomEvent.StopPropagation(e);
var _13=this.GetCollapseBarElement(RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward);
_13.className=(this.splitter.IsVertical())?"collapseBar":"collapseBarHorizontal";
return false;
};
RadSplitter_SplitBar.prototype.CollapseBarBackOnMouseOver=function(e){
RadControlsNamespace.DomEvent.StopPropagation(e);
var _15=this.GetCollapseBarElement(RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward);
_15.className=(this.splitter.IsVertical())?"collapseBarOver":"collapseBarOverHorizontal";
return false;
};
RadSplitter_SplitBar.prototype.CollapseBarBackOnMouseDown=function(e){
if(e.button&&e.button!=1){
return true;
}
RadControlsNamespace.DomEvent.PreventDefault(e);
RadControlsNamespace.DomEvent.StopPropagation(e);
this.collapseTargetPane(RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward);
var _17=this;
var _18=function(){
_17.CollapseBarBackOnMouseOut();
};
setTimeout(_18,10);
return false;
};
RadSplitter_SplitBar.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
};
RadSplitter_SplitBar.prototype.OnMouseDown=function(e){
RadControlsNamespace.DomEvent.PreventDefault(e);
RadControlsNamespace.DomEvent.StopPropagation(e);
if(this.IsCollapsed(RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward)||this.IsCollapsed(RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward)){
return false;
}
this.maxDecreaseDelta=this.getAvailDecreaseDelta();
this.maxIncreaseDelta=this.getAvailIncreaseDelta();
var _1a=this.GetSplitterBarElement();
var pos=window.RadControlsNamespace.Screen.GetElementPosition(_1a);
this.Log.Debug(["SplitBar.onMouseDown: panex[%d], paney[%d], mousex[%d], mousey[%d]\\n"+"maxDecrease[%d], maxIncrease[%d]",pos.x,pos.y,e.clientX,e.clientY,this.maxDecreaseDelta,this.maxIncreaseDelta]);
this.mouseStartX=e.clientX;
this.mouseStartY=e.clientY;
this.targetResizePane=this.splitter.getAvailAdjacentPane(this.prevPane.Index+1,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward);
if(this.targetResizePane==null){
return false;
}
this.liveResMouseX=e.clientX;
this.liveResMouseY=e.clientY;
this.liveResPaneStartSize=this.targetResizePane.getVarSize();
this.mouseOffsetX=e.clientX-pos.x;
this.mouseOffsetY=e.clientY-pos.y;
this.handlerStartLeftPos=pos.x;
this.handlerStartTopPos=pos.y;
this.currentDelta=0;
this.AttachDomEvent(document,"mouseup","OnMouseUp");
this.AttachDomEvent(document,"mousemove","OnMouseMove");
return false;
};
RadSplitter_SplitBar.prototype.OnMouseMove=function(e){
RadControlsNamespace.DomEvent.PreventDefault(e);
RadControlsNamespace.DomEvent.StopPropagation(e);
var _1d=0;
if(this.splitter.IsVertical()){
_1d=e.clientX-this.mouseStartX;
}else{
_1d=e.clientY-this.mouseStartY;
}
var _1e=150;
if(!this.helperBar){
var _1f=document.createElement("TABLE");
_1f.className=this.splitter.GetContainerElement().className;
_1f.style.borderCollapse="separate";
_1f.cellSpacing=_1e;
_1f.cellPadding=0;
_1f.style.borderWidth="0px";
_1f.style.background="";
_1f.style.cursor=this.getCursorStyle();
var _20=document.createElement("TBODY");
_1f.appendChild(_20);
var TR=document.createElement("TR");
_20.appendChild(TR);
var TD=document.createElement("TD");
TR.appendChild(TD);
var _23=document.createElement("DIV");
_23.className="helperBarDrag";
_23.style.width=this.GetWidth()-this.splitter.borderSize*(this.splitter.IsVertical()?1:0)+"px";
_23.style.height=Math.ceil(this.GetHeight()-this.splitter.borderSize*(!this.splitter.IsVertical()?1:1/2))+"px";
TD.appendChild(_23);
_1f.style.position="absolute";
var _24=0;
var _25=0;
if(!document.all&&this.splitter.IsNested()){
if(this.splitter.IsVertical()){
_25+=2;
}else{
_24+=2;
}
}
_1f.style.left=this.handlerStartLeftPos-_1e+_24+"px";
_1f.style.top=this.handlerStartTopPos-_1e+_25+"px";
_1f.style.zIndex=1;
this.helperBar=document.body.insertBefore(_1f,document.body.firstChild);
this.helperBarDecoration=_23;
}
var _26=false;
if(_1d<((-1)*this.maxDecreaseDelta)){
_26=true;
_1d=this.maxDecreaseDelta*(-1);
}
if(_1d>this.maxIncreaseDelta){
_26=true;
_1d=this.maxIncreaseDelta;
}
if(this.resizeStep>0&&!this.isLiveResize){
_1d-=_1d%this.resizeStep;
}
this.helperBarDecoration.className="helperBarDrag";
if(this.splitter.IsVertical()){
this.helperBar.style.left=this.handlerStartLeftPos-_1e+this.splitter.borderSize/2+_1d+"px";
}else{
this.helperBar.style.top=this.handlerStartTopPos-_1e+this.splitter.borderSize/2+_1d+"px";
}
if(!this.isLiveResize){
this.currentDelta=_1d;
}
if(_26){
if(this.helperBarDecoration){
this.helperBarDecoration.className="helperBarError";
}
if(!this.isLiveResize){
return false;
}
}
if(this.isLiveResize){
var _27=32;
if(this.lastUpdate&&((new Date()-this.lastUpdate)<_27)){
this.Log.Warning(["RadSplitter_SplitBar.onMouseMove Skip this live resize."]);
return false;
}
this.lastUpdate=new Date();
if(this.splitter.IsVertical()){
_1d=e.clientX-this.liveResMouseX;
}else{
_1d=e.clientY-this.liveResMouseY;
}
this.liveResMouseX=e.clientX;
this.liveResMouseY=e.clientY;
var _28=_1d;
if(_26){
var _29=this.liveResPaneStartSize;
var _2a=this.targetResizePane.getVarSize();
var _2b=(this.splitter.IsVertical())?(this.liveResMouseX-this.mouseStartX):(this.liveResMouseY-this.mouseStartY);
if(_2b>0){
_28=this.maxIncreaseDelta-(_2a-_29);
}else{
_28=this.maxDecreaseDelta-(_29-_2a);
_28*=-1;
}
if(_28==0){
return;
}
}
this.targetResizePane.Resize(_28,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward);
}
return false;
};
RadSplitter_SplitBar.prototype.OnMouseUp=function(e){
RadControlsNamespace.DomEvent.PreventDefault(e);
RadControlsNamespace.DomEvent.StopPropagation(e);
this.DetachDomEvent(document,"mousemove","OnMouseMove");
this.DetachDomEvent(document,"mouseup","OnMouseUp");
if(this.helperBar){
this.helperBar.parentNode.removeChild(this.helperBar);
this.helperBar=null;
}
if(!this.isLiveResize&&this.currentDelta!=0){
this.Log.Debug(["SplitBar.onMouseUp Going to resize. delta is [%d]",this.currentDelta]);
this.targetResizePane.Resize(this.currentDelta,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward);
}
return false;
};
RadSplitter_SplitBar.prototype.OnMouseOver=function(e){
if(!this.isInactive){
var _2e=this.GetSplitterBarElement();
_2e.className=(this.splitter.IsVertical())?"resizeBarOver":"resizeBarOverHorizontal";
}
};
RadSplitter_SplitBar.prototype.OnMouseOut=function(e){
if(!this.isInactive){
var _30=this.GetSplitterBarElement();
_30.className=(this.splitter.IsVertical())?"resizeBar":"resizeBarHorizontal";
}
};
RadSplitter_SplitBar.prototype.setCursorStyle=function(){
var _31=this.GetSplitterBarElement();
_31.style.cursor=this.getCursorStyle();
};
RadSplitter_SplitBar.prototype.getCursorStyle=function(){
if(!this.IsResizeEnabled()){
return "";
}
if(this.splitter.IsVertical()){
return "w-resize";
}else{
return "n-resize";
}
};
RadSplitter_SplitBar.prototype.getCollapseTarget=function(_32){
return (_32==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward)?this.prevPane:this.nextPane;
};
RadSplitter_SplitBar.prototype.getAvailDecreaseDelta=function(){
var _33=this.splitter.getAvailAdjacentPane(this.prevPane.Index+1,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward);
if(_33==null){
return 0;
}
var _34=0;
_34=_33.getAvailDecreaseDelta();
if(_34<=0){
return 0;
}
var _35=this.splitter.getAvailIncreaseDelta(_33.Index,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward);
return Math.min(_35,_34);
};
RadSplitter_SplitBar.prototype.getAvailIncreaseDelta=function(){
var _36=this.splitter.getAvailAdjacentPane(this.prevPane.Index+1,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward);
if(_36==null){
return 0;
}
var _37=0;
_37=_36.getAvailIncreaseDelta();
if(_37<=0){
return 0;
}
var _38=this.splitter.getAvailDecreaseDelta(_36.Index,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward);
return Math.min(_38,_37);
};
RadSplitter_SplitBar.prototype.getCollapseBarHeight=function(_39){
if(this.GetCollapseBarElement(_39)==null){
return 0;
}
return this.box.GetOuterHeight(this.GetCollapseBarElement(_39));
};
RadSplitter_SplitBar.prototype.getCollapseImageUrl=function(_3a){
var _3b=this.collapseImageUrl;
if(_3a==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward){
_3b=this.expandImageUrl;
}
return _3b;
};
RadSplitter_SplitBar.prototype.getExpandImageUrl=function(_3c){
var _3d=this.expandImageUrl;
if(_3c==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward){
_3d=this.collapseImageUrl;
}
return _3d;
};
RadSplitter_SplitBar.prototype.collapseTargetPane=function(_3e){
var _3f=this.getCollapseTarget(_3e);
if(!_3f){
return false;
}
if(_3f.IsLocked()){
this.showExpandCollapseError(_3e);
return false;
}
var _40=this.GetCollapseBarElement(_3e);
var _41=this.GetSplitterBarElement();
var _42=false;
if(this.IsCollapsed(_3e)){
if(_3f.IsCollapsed()){
if(this.splitter.expandPane(_3f,_3e)){
if(_40!=null){
_40.src=this.getCollapseImageUrl(_3e);
}
this.setActive();
this.collapsed[_3e]=false;
_42=true;
}else{
this.showExpandCollapseError(_3e);
}
}
}else{
if(!_3f.IsCollapsed()){
if(this.splitter.collapsePane(_3f,_3e)){
if(_40!=null){
_40.src=this.getExpandImageUrl(_3e);
}
this.setInactive();
this.collapsed[_3e]=true;
_42=true;
}else{
this.showExpandCollapseError(_3e);
}
}
}
if(_42){
var _43=(_3e==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward)?RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward:RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
var _44=this.GetCollapseBarElement(_43);
if(_44!=null){
_44.style.display=(_3f.IsCollapsed())?"none":"";
}
}
return _42;
};
RadSplitter_SplitBar.prototype.showExpandCollapseError=function(_45){
var _46=this.GetCollapseBarElement(_45);
if(_46==null){
return;
}
var _47=(this.splitter.IsVertical())?"collapseBar":"collapseBarHorizontal";
var _48=(this.splitter.IsVertical())?"collapseBarError":"collapseBarErrorHorizontal";
setTimeout(function(){
setCollapseBarCss(_48);
},0);
setTimeout(function(){
setCollapseBarCss(_47);
},200);
setTimeout(function(){
setCollapseBarCss(_48);
},400);
setTimeout(function(){
setCollapseBarCss(_47);
},600);
setTimeout(function(){
setCollapseBarCss(_48);
},800);
setTimeout(function(){
setCollapseBarCss(_47);
},1000);
function setCollapseBarCss(_49){
_46.className=_49;
}
};
RadSplitter_SplitBar.prototype.setActive=function(){
var _4a=this.GetSplitterBarElement();
this.setCursorStyle();
_4a.className=(this.splitter.IsVertical())?"resizeBar":"resizeBarHorizontal";
this.isInactive=false;
};
RadSplitter_SplitBar.prototype.setInactive=function(){
var _4b=this.GetSplitterBarElement();
_4b.style.cursor="";
var _4c=(this.splitter.IsVertical())?"resizeBarInactive":"resizeBarInactiveHorizontal";
_4b.className=(this.splitter.IsVertical())?"resizeBarInactive":"resizeBarInactiveHorizontal";
this.isInactive=true;
};;try{
document.execCommand("BackgroundImageCache",false,true);
}
catch(e){
}
function RadSplitter(_1){
_1["logLevel"]=eval(_1["logLevel"]);
this.Log=new RadControlsNamespace.Log(_1["logLevel"],true);
this.ID=_1["ID"];
if(typeof (RadControlsNamespace.__splitters__)=="undefined"){
RadControlsNamespace.__splitters__=[];
}
if(RadControlsNamespace.__splitters__[this.ID]){
this.Log.Debug(["RadSplitter dispose the old splitter. ID[%d]",this.ID]);
RadControlsNamespace.__splitters__[this.ID].Dispose();
}
RadControlsNamespace.__splitters__[this.ID]=this;
this.panes=[];
this.panesByIndex=[];
this.panesById=[];
this.splitBars=[];
this.splitBarsByIndex=[];
this.splitBarsById=[];
this.splitBarsByItemIndex=[];
this.config=_1;
this.browser=RadControlsNamespace.Browser;
this.screen=RadControlsNamespace.Screen;
this.box=window.RadControlsNamespace.Box;
RadControlsNamespace.EventMixin.Initialize(this);
RadControlsNamespace.DomEventMixin.Initialize(this);
this.heightOffset=_1["heightOffset"];
this.fullScreenMode=_1["fullScreenMode"];
if(this.fullScreenMode){
try{
document.body.style.height="100%";
document.documentElement.style.height="100%";
document.forms[0].style.height="100%";
}
catch(e){
}
}
this.resizeMode=_1["resizeMode"];
this.resizeStep=_1["resizeStep"];
this.parentPaneId=_1["parentPaneId"];
this.supportedEvents=["OnClientLoaded","OnClientResized","OnClientBeforeResize"];
for(var i=0;i<this.supportedEvents.length;i++){
this[this.supportedEvents[i]]=_1[this.supportedEvents[i]];
}
this.borderSize=(this.IsNested())?0:_1["borderSize"];
this.splitBarsSize=_1["splitBarsSize"];
this.panesBorderSize=_1["panesBorderSize"];
this.width=_1["width"];
if(this.width.indexOf("px")>-1){
this.width=parseInt(this.width,10);
}
this.height=_1["height"];
if(this.height.indexOf("px")>-1){
this.height=parseInt(this.height,10);
}
this.originalDimensions={width:this.width,height:this.height};
this.orientation=_1["orientation"];
this.resizeWithBrowserWindow=_1["resizeWithBrowserWindow"];
this.resizeWithParentPane=_1["resizeWithParentPane"];
this.visibleDuringInit=_1["visibleDuringInit"];
var _3=this.screen.GetViewPortSize();
this.WindowWidth=_3.width;
this.WindowHeight=_3.height;
this.initialSizeSet=false;
this.containerElement=document.getElementById("RAD_SPLITTER_"+this.ID);
this.wrapperElement=document.getElementById(this.ID);
if((typeof (__radsplitter__page_loaded)!="undefined"&&__radsplitter__page_loaded)||"complete"==document.readyState){
if(this.IsNested()&&this.resizeWithParentPane){
try{
var _4=window[this.parentPaneId];
if(_4&&_4.Inited){
_4.childSplitter=this;
window["RadPaneConfig_"+this.parentPaneId].childSplitterID=this.ID;
}
}
catch(e){
}
}
var _5=this;
var t=function(){
_5.Init();
};
setTimeout(t,10);
}else{
RadControlsNamespace.InitSplitterOnLoad(this);
this.Log.Debug(["Splitter Attach to load event to init ID[%d]",this.ID]);
}
}
RadSplitter.prototype.Init=function(){
if(this.Inited){
return;
}
this.Log.Debug(["Splitter.Init start ID[%d]",this.ID]);
var _7=this.config["panes"];
for(var i=0;i<_7.length;i++){
var _9=window["RadPaneConfig_"+_7[i]];
_9["splitter"]=this;
_9["index"]=i;
_9["logLevel"]=this.config["logLevel"];
var _a=new RadSplitter_Pane(_9);
this.addPane(_a);
}
var _b=this.config["splitBars"];
for(var i=0;i<_b.length;i++){
var _c=window["RadSplitBarConfig_"+_b[i]];
_c["splitter"]=this;
_c["liveResize"]=this.config["liveResize"];
_c["index"]=i;
_c["logLevel"]=this.config["logLevel"];
var _d=new RadSplitter_SplitBar(_c);
this.addSplitBar(_d);
}
for(var i=0;i<this.splitBars.length;i++){
this.splitBars[i].Init();
}
for(var i=0;i<this.panes.length;i++){
this.panes[i].Init();
}
if(this.isDisplayed()){
this.calculateInitialSize();
}
this.handleToResizeEvent=false;
if(this.originalDimensions.width.toString().indexOf("%")>-1||this.originalDimensions.height.toString().indexOf("%")>-1){
if(!(this.arePanesFixedSize()&&!this.containsInitialRelativeSizedPanes())){
this.handleToResizeEvent=true;
}
}
var _e=this.getWrapperElement();
var _f=this;
_e.RadShow=function(){
var t=function(){
if(!_f.isDisplayed()){
return;
}
_f.HandleResize(true);
};
window.setTimeout(t,10);
};
if(this.handleToResizeEvent&&this.resizeWithBrowserWindow&&!this.IsNested()){
this.Log.Debug(["Splitter.setSplitterSize attach to the resize event. ID[%d]",this.ID]);
this.AttachDomEvent(window,"resize","_HandleResize");
_e.RadResize=function(){
var t=function(){
if(!_f.isDisplayed()){
return;
}
_f.HandleResize(true);
};
window.setTimeout(t,10);
};
}
this.AttachDomEvent(window,"unload","Dispose");
this.Inited=true;
this.Log.Debug(["Splitter.Init End ID[%d].",this.ID]);
this.RaiseEvent("OnClientLoaded",{splitterObj:this});
};
RadSplitter.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
for(var i=0;i<this.panes.length;i++){
this.panes[i].Dispose();
}
for(var i=0;i<this.splitBars.length;i++){
this.splitBars[i].Dispose();
}
var _13=this.getWrapperElement();
if(_13!=null){
_13.RadShow=null;
_13.RadResize=null;
}
for(var i in this){
this[i]=null;
}
};
RadSplitter.prototype.IsNested=function(){
return this.parentPaneId!=null;
};
RadSplitter.prototype.SetResizeMode=function(_14){
this.resizeMode=_14;
};
RadSplitter.prototype.GetResizeMode=function(){
return this.resizeMode;
};
RadSplitter.prototype.SetHeightOffset=function(_15){
this.heightOffset=_15;
};
RadSplitter.prototype.GetHeightOffset=function(){
return this.heightOffset;
};
RadSplitter.prototype.GetWidth=function(){
return this.width;
};
RadSplitter.prototype.GetHeight=function(){
return this.height;
};
RadSplitter.prototype.GetInnerWidth=function(){
return this.GetWidth()-this.getBordersDiff();
};
RadSplitter.prototype.GetInnerHeight=function(){
return this.GetHeight()-this.getBordersDiff();
};
RadSplitter.prototype.GetPanes=function(){
return this.panes;
};
RadSplitter.prototype.Resize=function(_16,_17,_18){
if(!this.initialSizeSet){
this.calculateInitialSize();
}
var _19=new Date();
if(!_18){
if(!this.RaiseEvent("OnClientBeforeResize",{splitterObj:this,newWidth:_16,newHeight:_17})){
return false;
}
}
this.Log.Debug(["Splitter.Resize time#1 [%d], [%d]",_19-new Date(),this.ID]);
this.Log.Debug(["Splitter.Resize width[%d], height[%d], currentWidth[%d], currentHeight[%d], splID[%d]",_16,_17,this.GetWidth(),this.GetHeight(),this.ID]);
var _1a=false;
var _1b=false;
var _1c=0;
var _1d=this.GetWidth();
var _1e=this.GetHeight();
if(_16!=null&&_16!=_1d){
var _1f=_16-_1d;
this.setContainerOuterWidth(_16);
this.width=_16;
if(this.IsVertical()){
_1c=_1f;
_1a=true;
}else{
_1b=true;
}
}
if(_17!=null&&_17!=_1e){
var _20=_17-this.GetHeight();
this.setContainerOuterHeight(_17);
this.height=_17;
if(!this.IsVertical()){
_1c=_20;
_1a=true;
}else{
_1b=true;
}
}
if(_1b){
for(var i=0;i<this.panes.length;i++){
var _22=this.panes[i];
if(_22.IsCollapsed()){
continue;
}
var _23=_22.GetWidth();
var _24=_22.GetHeight();
if(this.IsVertical()){
_22.SetHeight(_17);
}else{
_22.SetWidth(_16);
}
_22.RaiseEvent("OnClientPaneResized",{paneObj:_22,oldWidth:_23,oldHeight:_24,newWidth:_22.GetWidth(),newHeight:_22.GetHeight()});
_22.callRadResize();
}
}
if(_1a){
this.Log.Debug(["Splitter.Resize delta is [%d], splID[%d]",_1c,this.ID]);
if(this.containsInitialFreeSizedPanes()){
var _25=this.getInitialFreeSizedPanes();
var _26=_25.length;
var _27=parseInt(_1c/_26,10);
this.Log.Debug(["Splitter.Resize Free sized panes detected  splID[%d]. Pane delta[%d]",this.ID,_27]);
var _28=0;
for(var i=0;i<_25.length;i++){
var _22=_25[i];
var _29=(_27>0)?_22.getAvailIncreaseDelta():_22.getAvailDecreaseDelta();
var _2a=_27;
if(_29<Math.abs(_27)){
_2a=_29*((_27>0)?1:-1);
_28+=_27-_2a;
}
this.Log.Debug(["Splitter.Resize delta[%d], paneAvailDelta[%d], pane[%d]",_2a,_29,_22.ID]);
var _2b=_22.getVarSize()+_2a;
var _24=_22.GetHeight();
var _23=_22.GetWidth();
_22.setVarSize(_2b);
if(this.Inited&&!_18){
_22.RaiseEvent("OnClientPaneResized",{paneObj:_22,oldWidth:_23,oldHeight:_24,newWidth:_22.GetWidth(),newHeight:_22.GetHeight()});
_22.callRadResize();
}
}
this.Log.Debug(["Splitter.Resize deltaLeft[%d] after resize of the free panes",_28]);
this.fixPanesRounding(_25);
if(_28!=0){
_28*=-1;
this.resizeProportional(_28,null,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward);
}
}else{
_1c*=-1;
this.resizeProportional(_1c,null,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward);
}
}
if(!_18){
this.RaiseEvent("OnClientResized",{splitterObj:this,newWidth:this.GetWidth(),newHeight:this.GetHeight(),oldWidth:_1d,oldHeight:_1e});
}
return true;
};
RadSplitter.prototype.GetEndPane=function(){
return this.GetPaneByIndex(this.panes.length-1);
};
RadSplitter.prototype.GetStartPane=function(){
return this.GetPaneByIndex(0);
};
RadSplitter.prototype.IsVertical=function(){
return (this.orientation==RadSplitterNamespace.RAD_SPLITTER_ORIENTATION.VERTICAL);
};
RadSplitter.prototype.GetMinWidth=function(_2c,_2d){
var _2e=this.getMinMaxSize(_2c,_2d,true,true);
this.Log.Debug(["Splitter.GetMinWidth calculatedMinWidth[%d]",_2e]);
return _2e;
};
RadSplitter.prototype.GetMaxWidth=function(_2f,_30){
var _31=this.getMinMaxSize(_2f,_30,false,true);
this.Log.Debug(["Splitter.GetMaxWidth calculatedMaxWidth[%d]",_31]);
return _31;
};
RadSplitter.prototype.GetMinHeight=function(_32,_33){
var _34=this.getMinMaxSize(_32,_33,true,false);
this.Log.Debug(["Splitter.GetMinHeight calculatedMinHeight[%d]",_34]);
return _34;
};
RadSplitter.prototype.GetMaxHeight=function(_35,_36){
var _37=this.getMinMaxSize(_35,_36,false,false);
this.Log.Debug(["Splitter.GetMaxHeight calculatedMaxHeight[%d]",_37]);
return _37;
};
RadSplitter.prototype.GetPaneByIndex=function(_38){
return this.panesByIndex[_38];
};
RadSplitter.prototype.GetPaneById=function(_39){
return this.panesById[_39];
};
RadSplitter.prototype.GetSplitBarByIndex=function(_3a){
return this.splitBarsByIndex[_3a];
};
RadSplitter.prototype.getSplitBarByItemIndex=function(_3b){
return this.splitBarsByItemIndex[_3b];
};
RadSplitter.prototype.GetSplitBarById=function(_3c){
return this.splitBarsById[_3c];
};
RadSplitter.prototype.GetSplitBars=function(){
return this.splitBars;
};
RadSplitter.prototype.SetWidth=function(_3d){
this.setWidth(_3d);
};
RadSplitter.prototype.SetHeight=function(_3e){
this.setHeight(_3e);
};
RadSplitter.prototype.GetContainerElement=function(){
return this.containerElement;
};
RadSplitter.prototype.getWrapperElement=function(){
return this.wrapperElement||document.getElementById(this.ID);
};
RadSplitter.prototype.resizeAdjacentPane=function(_3f,_40,_41){
if(_3f==0){
return;
}
var _42=this.getAvailAdjacentPane(_40.Index,_41);
if(_42==null){
this.Log.Warning(["Splitter.resizeAdjacentPane: Can not find suitable adjacent pane. PaneId[%d], ResizeDirection[%d] RETURN.",_40.ID,_41]);
return false;
}
if(!this.isCollapseMode&&!this.isExpandMode){
if(!_40.RaiseEvent("OnClientBeforePaneResize",{paneObj:_40,delta:_3f,direction:_41})){
return false;
}
if(!_42.RaiseEvent("OnClientBeforePaneResize",{paneObj:_42,delta:_3f*-1,direction:_41})){
return false;
}
}
this.Log.Debug(["Splitter.resizeAdjacentPane: Found Adjacent pane[%d], for pane[%d].",_42.ID,_40.ID]);
var _43=_40.GetWidth();
var _44=_40.GetHeight();
var _45=_42.GetWidth();
var _46=_42.GetHeight();
var _47=_40.getVarSize()+_3f;
var _48=_42.getVarSize()-_3f;
this.Log.Debug(["Splitter.resizeAdjacentPane: Set PaneSize[%d], AdjacentSize[%d], AdjacentIndex[%d]",_47,_48,_42.ID]);
_40.setVarSize(_47);
_42.setVarSize(_48);
if(!this.isCollapseMode&&!this.isExpandMode){
_40.RaiseEvent("OnClientPaneResized",{paneObj:_40,oldWidth:_43,oldHeight:_44,newWidth:_40.GetWidth(),newHeight:_40.GetHeight()});
_40.callRadResize();
}
_42.RaiseEvent("OnClientPaneResized",{paneObj:_42,oldWidth:_45,oldHeight:_46,newWidth:_42.GetWidth(),newHeight:_42.GetHeight()});
_42.callRadResize();
return true;
};
RadSplitter.prototype.resizeEndPane=function(_49,_4a,_4b){
if(_49==0){
return;
}
var _4c=(_4b==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward)?this.GetEndPane():this.GetStartPane();
if(!this.isCollapseMode&&!this.isExpandMode){
if(!_4a.RaiseEvent("OnClientBeforePaneResize",{paneObj:_4a,delta:_49,direction:_4b})){
return false;
}
if(!_4c.RaiseEvent("OnClientBeforePaneResize",{paneObj:_4c,delta:_49*-1,direction:_4b})){
return false;
}
}
var _4d=_4a.GetWidth();
var _4e=_4a.GetHeight();
var _4f=_4c.GetWidth();
var _50=_4c.GetHeight();
var _51=_4a.getVarSize()+_49;
_4a.setVarSize(_51);
var _52=_4c.getVarSize()-_49;
_4c.setVarSize(_52);
this.Log.Debug(["Splitter.resizeEndPane: Set current pane size[%d], end pane size[%d]",_51,_52]);
if(!this.isCollapseMode&&!this.isExpandMode){
_4a.RaiseEvent("OnClientPaneResized",{paneObj:_4a,oldWidth:_4d,oldHeight:_4e,newWidth:_4a.GetWidth(),newHeight:_4a.GetHeight()});
_4a.callRadResize();
}
_4c.RaiseEvent("OnClientPaneResized",{paneObj:_4c,oldWidth:_4f,oldHeight:_50,newWidth:_4c.GetWidth(),newHeight:_4c.GetHeight()});
_4c.callRadResize();
return true;
};
RadSplitter.prototype.resizeProportional=function(_53,_54,_55){
if(_53==0){
return;
}
var _56=(this.isCollapseMode||this.isExpandMode)?false:true;
this.Log.Debug(["Splitter.resizeProportional - raiseResizeEvents[%d]",_56]);
if(_56&&_54!=null){
if(!_54.RaiseEvent("OnClientBeforePaneResize",{paneObj:_54,delta:_53,direction:_55})){
return;
}
}
var _57=Math.abs(_53);
var _58=0;
var _59=(_53>0);
var _5a=(_54!=null)?((_55==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward)?_54.Index+1:_54.Index-1):0;
var _5b=[];
if(_55==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward){
for(var i=_5a;i<this.panes.length;i++){
var _5d=this.GetPaneByIndex(i);
_5b[_5b.length]=_5d;
_58+=_5d.getVarSize();
}
}else{
for(var i=_5a;i>=0;i--){
var _5d=this.GetPaneByIndex(i);
_5b[_5b.length]=_5d;
_58+=_5d.getVarSize();
}
}
if(_5b.length<2&&_54!=null){
this.Log.Debug(["Splitter.resizeProportional - no enough panes for propotional resize. Do AdjacentPane resize."]);
this.resizeAdjacentPane(_53,_54,_55);
return;
}
this.Log.Debug(["Splitter.resizeProportional - delta[%d], panesSize[%d], decreaseMode[%d]",_53,_58,_59]);
var _5e=[];
do{
var _5f=_58;
var _60=0;
for(var i=0;i<_5b.length;i++){
if(_5e[i]){
continue;
}
var _61=_5b[i];
if(_61.IsCollapsed()||_61.IsLocked()){
_5e[i]=true;
continue;
}
var _62=_61.getVarSize();
var _63=(_59)?_61.getAvailDecreaseDelta():_61.getAvailIncreaseDelta();
var _64=_62/_5f;
var _65=_57*_64;
this.Log.Debug(["Splitter.resizeProportional AllDelta[%d], paneId[%d], paneSize[%d], paneWeight[%d],\\n paneAvailDelta[%d], paneNeededDelta[%d], panesSize[%d]",_57,_61.ID,_62,_64,_63,_65,_58]);
if((_65-_63)>0){
_5e[i]=true;
}
var _66=Math.min(_65,_63);
if(_59){
_66*=-1;
}
var _67=_62+_66;
if(_56){
if(!_61.RaiseEvent("OnClientBeforePaneResize",{paneObj:_61,delta:parseInt(_67),direction:_55})){
_5e[i]=true;
continue;
}
}
_60+=_65-Math.abs(_66);
this.Log.Debug(["Splitter.resizeProportional Applying paneDelta[%d], deltaLeft[%d]",_66,_60]);
var _68=_61.GetWidth();
var _69=_61.GetHeight();
_61.setVarSize(_67);
_61.RaiseEvent("OnClientPaneResized",{paneObj:_61,oldWidth:_68,oldHeight:_69,newWidth:_61.GetWidth(),newHeight:_61.GetHeight()});
_61.callRadResize();
_58+=(_5e[i])?(-1)*_61.getVarSize()+_66:_66;
}
_57=_60;
}while(_60!=0);
if(_54!=null){
var _6a=_54.GetWidth();
var _6b=_54.GetHeight();
_54.setVarSize(_54.getVarSize()+_53);
if(_56){
_54.RaiseEvent("OnClientPaneResized",{paneObj:_54,oldWidth:_6a,oldHeight:_6b,newWidth:_54.GetWidth(),newHeight:_54.GetHeight()});
_54.callRadResize();
}
}
this.fixPanesRounding(_5b);
};
RadSplitter.prototype.fixPanesRounding=function(_6c){
if(!_6c){
_6c=this.GetPanes();
}
var _6d=(this.IsVertical())?this.getPanesAvailWidth():this.getPanesAvailHeight();
this.Log.Debug(["Splitter.fixPanesRounding Splitter panes size[%d], splID[%d]",_6d,this.ID]);
var _6e=this.GetPanes();
var _6f=0;
for(var i=0;i<_6e.length;i++){
var _71=_6e[i].getVarSize();
this.Log.Debug(["Splitter.fixPanesRounding panes size[%d], paneID[%d], splID[%d]",_71,_6e[i].ID,this.ID]);
_6f+=_71;
}
var _72=_6d-_6f;
if(_72!=0){
this.Log.Warning(["Splitter.fixPanesRounding Found rounding error delta[%d], splID[%d]",_72,this.ID]);
for(var i=0;i<_6c.length;i++){
var _73=_6c[i];
if(_73.IsCollapsed()||_73.IsLocked()){
continue;
}
if(!_73.RaiseEvent("OnClientBeforePaneResize",{paneObj:_73,delta:_72,direction:RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward})){
continue;
}
var _74=_73.GetWidth();
var _75=_73.GetHeight();
var _76=false;
if(_72>0){
if(_73.getAvailIncreaseDelta()>0){
_76=true;
}
}else{
if(_73.getAvailDecreaseDelta()>0){
_76=true;
}
}
if(_76){
this.Log.Warning(["Splitter.fixPanesRounding Applying round correction[%d], paneID[%d]",_72,_73.ID]);
_73.setVarSize(_73.getVarSize()+_72);
_73.RaiseEvent("OnClientPaneResized",{paneObj:_73,oldWidth:_74,oldHeight:_75,newWidth:_73.GetWidth(),newHeight:_73.GetHeight()});
_73.callRadResize();
break;
}
}
}
};
RadSplitter.prototype.addPane=function(_77){
this.panes[this.panes.length]=_77;
this.panesByIndex[_77.Index]=_77;
this.panesById[_77.ID]=_77;
};
RadSplitter.prototype.addSplitBar=function(_78){
this.splitBars[this.splitBars.length]=_78;
this.splitBarsByIndex[_78.Index]=_78;
this.splitBarsById[_78.ID]=_78;
this.splitBarsByItemIndex[_78.indexInSplitItems]=_78;
};
RadSplitter.prototype.setWidth=function(_79){
if(_79==this.width){
return;
}
this.Log.Debug(["Splitter.setWidth: width[%d]",_79]);
var _7a=_79;
if(_79.toString().indexOf("%")>-1){
_7a=this.calculatePercentWidth(_79);
}
_7a=parseInt(_7a,10);
if(isNaN(_7a)){
return;
}
this.Resize(_7a,null);
};
RadSplitter.prototype.setHeight=function(_7b){
if(_7b==this.height){
return;
}
this.Log.Debug(["Splitter.setHeight ID[%d], height[%d]",this.ID,_7b]);
var _7c=_7b;
if(_7b.toString().indexOf("%")>-1){
_7c=this.calculatePercentHeight(_7b);
}
_7c=parseInt(_7c,10);
if(isNaN(_7c)){
return;
}
this.Resize(null,_7c);
};
RadSplitter.prototype.setContainerOuterWidth=function(_7d){
this.width=_7d;
this.Log.Debug(["Splitter.setContainerOuterWidth width[%d], splID[%d]",_7d,this.ID]);
this.box.SetOuterWidth(this.GetContainerElement(),_7d);
};
RadSplitter.prototype.setContainerOuterHeight=function(_7e){
this.height=_7e;
this.Log.Debug(["Splitter.setContainerOuterHeight height[%d], splID[%d]",_7e,this.ID]);
this.box.SetOuterHeight(this.GetContainerElement(),_7e);
};
RadSplitter.prototype.setContainerInnerWidth=function(_7f){
this.setContainerOuterWidth(_7f+this.getBordersDiff());
};
RadSplitter.prototype.setContainerInnerHeight=function(_80){
this.setContainerOuterHeight(_80+this.getBordersDiff());
};
RadSplitter.prototype.arePanesFixedSize=function(){
var _81=this.GetPanes();
for(var i=0;i<_81.length;i++){
if(!_81[i].IsFixedSize()){
return false;
}
}
return true;
};
RadSplitter.prototype.containsFreeSizedPanes=function(){
var _83=this.GetPanes();
for(var i=0;i<_83.length;i++){
if(_83[i].IsFreeSize()){
return true;
}
}
return false;
};
RadSplitter.prototype.containsInitialRelativeSizedPanes=function(){
if(this.containsInitialFreeSizedPanes()){
return true;
}
var _85=this.GetPanes();
for(var i=0;i<_85.length;i++){
if(_85[i].getOrigVarSize().indexOf("%")){
return true;
}
}
return false;
};
RadSplitter.prototype.containsInitialFreeSizedPanes=function(){
var _87=this.GetPanes();
for(var i=0;i<_87.length;i++){
if(_87[i].IsInitialFreeSize()){
return true;
}
}
return false;
};
RadSplitter.prototype.arePanesPercenSize=function(){
var _89=this.GetPanes();
for(var i=0;i<_89.length;i++){
if(!_89[i].IsPercentSize()){
return false;
}
}
return true;
};
RadSplitter.prototype._HandleResize=function(){
this.HandleResize(false);
};
RadSplitter.prototype.HandleResize=function(_8b){
if(!this.isDisplayed()){
return;
}
if(!this.initialSizeSet){
this.calculateInitialSize();
}
var _8c=new Date();
var now=new Date();
if((now-this.lastResize)<150){
this.pendingResize=true;
return;
}
this.pendingResize=false;
this.Log.Debug(["Splitter.ResizeHandler. ID [%d]",this.ID]);
var _8e=this.screen.GetViewPortSize();
var _8f=_8e.width;
var _90=_8e.height;
if(!_8b&&this.WindowHeight==_90&&this.WindowWidth==_8f){
this.Log.Warning(["Splitter.ResizeHandler - skip resize due to same dimensions ID [%d]",this.ID]);
return;
}
this.Log.Warning(["Splitter.ResizeHandler - windowW[%d] windowH[%d] ID [%d]",_8f,_90,this.ID]);
this.WindowHeight=_90;
this.WindowWidth=_8f;
var _91=null;
if(this.originalDimensions.width.toString().indexOf("%")>-1){
var _92=this.calculatePercentWidth();
if(_92!=this.width){
_91=_92;
}
}
var _93=null;
if(this.originalDimensions.height.toString().indexOf("%")>-1){
var _94=this.calculatePercentHeight();
if(_94!=this.height){
_93=_94;
}
}
this.Log.Warning(["Splitter.ResizeHandler . Calculated width[%d] height[%d] ID [%d]",_91,_93,this.ID]);
this.Resize(_91,_93);
this.lastResize=new Date();
var _95=this;
this.Log.Warning(["Splitter.HandleResize real resize done for [%d]ms, ID[%d]",new Date()-_8c,this.ID]);
var t=function(){
if(_95.pendingResize){
_95.HandleResize();
}
};
var _97=new Date();
this.Log.Warning(["Splitter.HandleResize done for [%d]ms, ID[%d]",_97-_8c,this.ID]);
setTimeout(t,200);
};
RadSplitter.prototype.setSplitterSize=function(){
this.Log.Debug(["Splitter.setSplitterSize . ID[%d], real Size [%d]",this.ID,this.box.GetOuterWidth(this.GetContainerElement())]);
var _98;
if(this.arePanesFixedSize()&&!this.containsInitialFreeSizedPanes()){
var _99=this.getPanesVarSize();
var _9a=this.getSplitBarsSize();
var _9b=this.getBordersSize();
var _9c=_99+_9a+_9b;
this.Log.Debug(["Splitter.setSplitterSize Reset the splitter size to[%d], panesSize[%d], splitBarsSize[%d], bordersSize[%d] ID[%d]",_9c,_99,_9a,_9b,this.ID]);
if(this.IsVertical()){
this.changeOriginalWidth(_9c);
this.setContainerOuterWidth(_9c);
}else{
this.changeOriginalHeight(_9c);
this.setContainerOuterHeight(_9c);
}
}
if(this.originalDimensions.width.toString().indexOf("%")>-1){
var _9d=this.calculatePercentWidth();
this.Log.Debug(["Splitter.setSplitterSize Recalculated width [%d], original width[%d], ID[%d]",_9d,this.originalDimensions.width,this.ID]);
this.setContainerInnerWidth(_9d);
if(!this.IsVertical()){
var _9e=this.GetPanes();
var _9f=this.GetInnerWidth();
for(var i=0;i<_9e.length;i++){
var _a1=_9e[i];
_9e[i].SetWidth(_9f);
}
}
}
if(this.originalDimensions.height.toString().indexOf("%")>-1){
var _a2=this.calculatePercentHeight();
this.Log.Debug(["Splitter.setSplitterSize Recalculated height [%d], original height[%d], ID[%d]",_a2,this.originalDimensions.height,this.ID]);
this.setContainerOuterHeight(_a2);
if(this.IsVertical()){
var _9e=this.GetPanes();
var _a3=this.GetInnerHeight();
for(var i=0;i<_9e.length;i++){
var _a1=_9e[i];
_9e[i].SetHeight(_a3);
}
}
}
};
RadSplitter.prototype.fixSplitterActualSize=function(){
if(this.originalDimensions.height.toString().indexOf("%")==-1){
return;
}
var _a4=this.GetContainerElement();
if(parseInt(_a4.style.height)==this.height){
return;
}
this.setContainerOuterHeight(this.height);
};
RadSplitter.prototype.calculatePercentWidth=function(_a5){
return this.calculatePercentSize(_a5,null).width;
};
RadSplitter.prototype.calculatePercentHeight=function(_a6){
return this.calculatePercentSize(null,_a6).height;
};
RadSplitter.prototype.calculatePercentSize=function(_a7,_a8){
if(!_a7){
_a7=this.originalDimensions.width;
}
if(!_a8){
_a8=this.originalDimensions.height;
}
var _a9=this.getWrapperElement();
_a9.style.display="none";
for(var i=0;i<this.panes.length;i++){
this.panes[i].hideContent();
}
var _ab=document.createElement("DIV");
_ab.style.height=_a8;
_ab.style.width=_a7;
_ab=_a9.parentNode.appendChild(_ab);
if(document.documentElement){
var _ac=document.documentElement.style.overflowX;
document.documentElement.style.overflowX="hidden";
}
var _ad=this.box.GetOuterHeight(_ab);
var _ae=this.box.GetOuterWidth(_ab);
if(document.documentElement){
document.documentElement.style.overflowX=_ac;
}
_ab.parentNode.removeChild(_ab);
for(var i=0;i<this.panes.length;i++){
this.panes[i].showContent();
}
_a9.style.display="";
var _af=2*this.borderSize;
var _b0=_ad-_af-this.heightOffset;
var _b1=_ae-_af;
return {width:_b1,height:_b0};
};
RadSplitter.prototype.setPanesSize=function(){
var _b2=this.GetPanes();
this.Log.Debug(["Splitter.setPanesSize . ID[%d]",this.ID]);
var _b3=this.arePanesPercenSize();
for(var i=0;i<_b2.length;i++){
var _b5=_b2[i];
if(_b5.IsPercentSize()){
var _b6=(this.IsVertical())?this.getPanesAvailWidth():this.getPanesAvailHeight();
var _b7=(this.IsVertical())?_b5.originalDimensions.width:_b5.originalDimensions.height;
var _b8=parseInt(_b7)*_b6/100;
_b8=parseInt(_b8);
_b5.setVarSize(_b8);
_b5.callRadResize();
}
}
if(this.containsFreeSizedPanes()){
this.Log.Debug(["Splitter.setPanesSize Detected free sized panes splID[%d]",this.ID]);
var _b9=(this.IsVertical())?this.width:this.height;
var _ba=this.getSplitBarsSize();
var _bb=this.getBordersSize();
var _bc=(this.IsVertical())?this.getPanesAvailWidth():this.getPanesAvailHeight();
var _b2=this.GetPanes();
var _bd=0;
for(var i=0;i<_b2.length;i++){
if(_b2[i].IsFixedSize()){
_bc-=_b2[i].getVarSize();
}else{
if(_b2[i].IsLocked()||(_b2[i].initiallyCollapsed&&_b2[i].expandedSize>0)){
continue;
}
_bd++;
}
}
var _be=_bc/_bd;
_be=parseInt(_be);
this.Log.Debug(["Splitter.setPanesSize Found [%d] free sized panes. Avail free size[%d], size per pane[%d], splitterSize[%d], splitBarsSize[%d], panesBorderSize[%d], splID[%d]",_bd,_bc,_be,_b9,_ba,this.panesBorderSize,this.ID]);
var _bf=this.getFreeSizedPanes();
for(var i=0;i<_bf.length;i++){
if(_bf[i].IsLocked()||(_bf[i].initiallyCollapsed&&_bf[i].expandedSize>0)){
continue;
}
_bf[i].setVarSize(_be);
_bf[i].callRadResize();
}
}
};
RadSplitter.prototype.getFreeSizedPanes=function(){
var _c0=this.GetPanes();
var _c1=[];
for(var i=0;i<_c0.length;i++){
if(_c0[i].IsFreeSize()){
_c1[_c1.length]=_c0[i];
}
}
return _c1;
};
RadSplitter.prototype.getInitialFreeSizedPanes=function(){
var _c3=this.GetPanes();
var _c4=[];
for(var i=0;i<_c3.length;i++){
if(_c3[i].IsInitialFreeSize()){
_c4[_c4.length]=_c3[i];
}
}
return _c4;
};
RadSplitter.prototype.getPanesVarSize=function(){
var _c6=this.GetPanes();
var _c7=0;
for(var i=0;i<_c6.length;i++){
_c7+=_c6[i].getVarSize();
}
return _c7;
};
RadSplitter.prototype.getPanesAvailWidth=function(){
var _c9=this.GetWidth()-this.getBordersSize();
if(this.IsVertical()){
_c9-=this.getSplitBarsSize();
}
return _c9;
};
RadSplitter.prototype.getPanesAvailHeight=function(){
var _ca=this.GetHeight()-this.getBordersSize();
if(!this.IsVertical()){
_ca-=this.getSplitBarsSize();
}
return _ca;
};
RadSplitter.prototype.getPanesBordersSize=function(){
var _cb=0;
for(var i=0;i<this.panes.length;i++){
if(!this.panes[i].IsCollapsed()){
_cb++;
}
}
return (_cb-1)*this.panesBorderSize;
};
RadSplitter.prototype.getBordersSize=function(){
var _cd=0;
for(var i=0;i<this.panes.length;i++){
if(!this.panes[i].IsCollapsed()){
_cd++;
}
}
return (Math.max(_cd+this.splitBars.length-1,0))*this.panesBorderSize+this.getBordersDiff();
};
RadSplitter.prototype.getBordersDiff=function(){
var _cf=0;
return _cf;
};
RadSplitter.prototype.getSplitBarsSize=function(){
var _d0=0;
if(this.splitBarsSizeCalculated){
return this.splitBarsSize;
}
if(this.splitBars.length>0){
var _d1=0;
if(typeof (this.splitBarsSize)!="undefined"){
_d1=parseInt(this.splitBarsSize);
}else{
var _d2=this.GetSplitBarByIndex(0);
var _d3=_d2.GetContainerElement();
var _d4=document.createElement("DIV");
_d4.className=this.GetContainerElement().className;
_d4.style.position="absolute";
_d4.style.top=-1000;
_d4.style.left=-1000;
_d4.style.width=500;
_d4.style.height=500;
var _d5=document.createElement("DIV");
_d5.className=_d3.className;
_d5.style.backgroundColor="red";
_d4.appendChild(_d5);
_d4=document.body.appendChild(_d4);
_d1=(this.IsVertical())?this.box.GetOuterWidth(_d5):this.box.GetOuterHeight(_d5);
_d1-=2*this.panesBorderSize;
_d1=Math.max(_d1,0);
_d4.parentNode.removeChild(_d4);
}
_d0=this.splitBars.length*_d1;
}
this.splitBarsSizeCalculated=true;
this.splitBarsSize=_d0;
return _d0;
};
RadSplitter.prototype.getAvailIncreaseDelta=function(_d6,_d7){
var _d8=0;
switch(this.resizeMode){
case RadSplitterNamespace.RAD_SPLITTER_RESIZE_MODE.END_PANE:
var _d9=(_d7==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward)?this.GetEndPane():this.GetStartPane();
_d8=_d9.getAvailIncreaseDelta();
break;
case RadSplitterNamespace.RAD_SPLITTER_RESIZE_MODE.PROPORTIONAL:
var _da=[];
if(_d7==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward){
for(var i=_d6+1;i<this.panes.length;i++){
_da[_da.length]=this.GetPaneByIndex(i);
}
}else{
for(var i=_d6-1;i>=0;i--){
_da[_da.length]=this.GetPaneByIndex(i);
}
}
for(var i=0;i<_da.length;i++){
var _dc=_da[i];
_d8+=_dc.getAvailIncreaseDelta();
}
break;
case RadSplitterNamespace.RAD_SPLITTER_RESIZE_MODE.ADJACENT_PANE:
default:
var _dd=this.getAvailAdjacentPane(_d6,_d7);
if(_dd==null){
return 0;
}
this.Log.Debug(["Splitter.getAvailIncreaseDelta: GotAdjacentPane[%d]",_dd.ID]);
_d8+=_dd.getAvailIncreaseDelta();
}
_d8=Math.max(_d8,0);
this.Log.Debug(["Splitter.getAvailIncreaseDelta: delta[%d]",_d8]);
return _d8;
};
RadSplitter.prototype.getAvailAdjacentPane=function(_de,_df){
if((this.panes.length-1)==_de){
this.Log.Debug(["Splitter.getAvailAdjacentPane: Detected End Pane Index[%d]",_de]);
_df=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward;
}else{
if(_de==0){
this.Log.Debug(["Splitter.getAvailAdjacentPane: Detected Start Pane Index[%d]",_de]);
_df=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
}
}
var _e0=(_df==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward)?1:-1;
var _e1=_de+_e0;
var _e2=null;
do{
_e2=this.GetPaneByIndex(_e1);
if(_e2==null){
return null;
}
_e1+=_e0;
}while(_e2.IsCollapsed()||_e2.IsLocked());
return _e2;
};
RadSplitter.prototype.getAvailDecreaseDelta=function(_e3,_e4){
var _e5=0;
switch(this.resizeMode){
case RadSplitterNamespace.RAD_SPLITTER_RESIZE_MODE.END_PANE:
var _e6=(_e4==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward)?this.GetEndPane():this.GetStartPane();
_e5=_e6.getAvailDecreaseDelta();
break;
case RadSplitterNamespace.RAD_SPLITTER_RESIZE_MODE.PROPORTIONAL:
this.Log.Debug(["paneIndex[%d], panesLength[%d]",_e3,this.panes.length]);
var _e7=[];
if(_e4==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward){
for(var i=_e3+1;i<this.panes.length;i++){
_e7[_e7.length]=this.GetPaneByIndex(i);
}
}else{
for(var i=_e3-1;i>=0;i--){
_e7[_e7.length]=this.GetPaneByIndex(i);
}
}
for(var i=0;i<_e7.length;i++){
var _e9=_e7[i];
_e5+=_e9.getAvailDecreaseDelta();
}
break;
case RadSplitterNamespace.RAD_SPLITTER_RESIZE_MODE.ADJACENT_PANE:
default:
var _ea=this.getAvailAdjacentPane(_e3,_e4);
if(_ea==null){
return 0;
}
this.Log.Debug(["Splitter.getAvailDecreaseDelta: Found AdjacentPane[%d] for paneIndex[%d]",_ea.ID,_e3]);
_e5=_ea.getAvailDecreaseDelta();
}
_e5=Math.max(_e5,0);
this.Log.Debug(["Splitter.getAvailDecreaseDelta: maxDelta[%d]",_e5]);
return _e5;
};
RadSplitter.prototype.collapsePane=function(_eb,_ec){
if(!_eb.initialCollapseMode&&!_eb.RaiseEvent("OnClientBeforePaneCollapse",{paneObj:_eb})){
return false;
}
this.isCollapseMode=true;
if(typeof (_ec)=="undefined"){
_ec=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
}
var _ed=_eb.getVarSize();
if(_ed>0){
_ed+=this.panesBorderSize;
}
this.Log.Debug(["Splitter.collapsePane delta[%d], Fwd[%s]",_ed,(_ec==RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward)]);
var _ee=this.getAvailIncreaseDelta(_eb.Index,_ec);
if(_ee<_ed){
this.Log.Warning(["Splitter.collapsePane Can not collapse pane. No enough delta. CollapseDelta[%d], availDelta[%d], targetPane[%d]",_ed,_ee,_eb.ID]);
this.isCollapseMode=false;
return false;
}
_ed*=-1;
_eb.collapse(_ec);
_eb.Resize(_ed,_ec);
if(!_eb.initialCollapseMode){
_eb.RaiseEvent("OnClientPaneCollapsed",{paneObj:_eb});
}
this.isCollapseMode=false;
return true;
};
RadSplitter.prototype.expandPane=function(_ef,_f0){
if(!_ef.RaiseEvent("OnClientBeforePaneExpand",{paneObj:_ef})){
return false;
}
this.isExpandMode=true;
if(typeof (_f0)=="undefined"){
_f0=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
}
var _f1=_ef.expandedSize;
this.Log.Debug(["Splitter.expandPane paneId[%d], delta [%d]",_ef.ID,_f1]);
_f1+=this.panesBorderSize;
var _f2=this.getAvailDecreaseDelta(_ef.Index,_f0);
if(_f2<=0){
this.Log.Warning(["No avail delta . availDelta[%d], expandDelta[%d]",_f2,_f1]);
this.isExpandMode=false;
return false;
}
var _f3=_ef.getVarMinSize();
if(_f3>_f2){
this.Log.Warning(["Splitter.expandPane No enough avail delta paneID[%d], minExpandDelta[%d], availDelta[%d]",_ef.ID,_f3,_f2]);
this.isExpandMode=false;
return false;
}
var _f4=Math.min(_f2,_f1);
this.Log.Debug(["Splitter.expandPane Applying paneId[%d], expandedPaneSize[%d], availDelta[%d], delta [%d]",_ef.ID,_ef.expandedSize,_f2,_f4]);
_ef.show();
_ef.Resize(_f4,_f0);
_f4-=this.panesBorderSize;
_ef.expand(_f4);
_ef.RaiseEvent("OnClientPaneExpanded",{paneObj:_ef});
this.isExpandMode=false;
return true;
};
RadSplitter.prototype.resizePanes=function(_f5,_f6,_f7){
this.Log.Debug(["Splitter.resizePanes delta[%d]",_f5]);
if(typeof (_f7)=="undefined"){
_f7=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
}
switch(this.resizeMode){
case RadSplitterNamespace.RAD_SPLITTER_RESIZE_MODE.END_PANE:
this.resizeEndPane(_f5,_f6,_f7);
break;
case RadSplitterNamespace.RAD_SPLITTER_RESIZE_MODE.PROPORTIONAL:
this.resizeProportional(_f5,_f6,_f7);
break;
case RadSplitterNamespace.RAD_SPLITTER_RESIZE_MODE.ADJACENT_PANE:
default:
this.resizeAdjacentPane(_f5,_f6,_f7);
}
};
RadSplitter.prototype.changeOriginalWidth=function(_f8){
this.originalDimensions.width=_f8;
};
RadSplitter.prototype.changeOriginalHeight=function(_f9){
this.originalDimensions.height=_f9;
};
RadSplitter.prototype.getMinMaxSize=function(_fa,_fb,_fc,_fd){
if(!_fa){
_fa=0;
}
if(!_fb){
_fb=this.panes.length;
}
_fa=Math.max(0,_fa);
_fb=Math.min(_fb,this.panes.length);
var _fe=(_fd)?"GetWidth":"GetHeight";
var _ff=(_fc)?"GetMin":"GetMax";
_ff+=(_fd)?"Width":"Height";
var _100=this.getSplitBarsSize()+this.getBordersSize();
for(var i=_fa;i<_fb;i++){
var _102=this.GetPaneByIndex(i);
_100+=(_102.IsLocked())?_102[_fe]():_102[_ff]();
}
return _100;
};
RadSplitter.prototype.isDisplayed=function(){
return (this.GetContainerElement().offsetWidth!=0);
};
RadSplitter.prototype.calculateInitialSize=function(){
if(this.initialSizeSet){
return;
}
if(!this.isDisplayed()){
return;
}
this.setSplitterSize();
this.setPanesSize();
this.fixSplitterActualSize();
if(!this.visibleDuringInit){
var _103=this.GetContainerElement();
_103.style.visibility="visible";
}
this.initialSizeSet=true;
};;if(typeof window.RadSplitterNamespace=="undefined"){
window.RadSplitterNamespace={};
}
RadSplitterNamespace.RAD_SPLITTER_ORIENTATION={VERTICAL:1,HORIZONTAL:2};
RadSplitterNamespace.RAD_SPLITTER_RESIZE_MODE={ADJACENT_PANE:1,PROPORTIONAL:2,END_PANE:3};
RadSplitterNamespace.RAD_SPLITTER_DIRECTION={Forward:1,Backward:2};
RadSplitterNamespace.RAD_SPLITBAR_COLLAPSE_MODE={None:1,Forward:2,Backward:3,Both:4};
RadSplitterNamespace.RAD_SPLITTER_SLIDE_DIRECTION={Right:1,Left:2,Top:3,Bottom:4};;if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
RadControlsNamespace.AttachedOnLoad=false;
RadControlsNamespace.SplittersToLoad=[];
RadControlsNamespace.InitSplitterOnLoad=function(_1){
RadControlsNamespace.SplittersToLoad[RadControlsNamespace.SplittersToLoad.length]=_1;
if(!RadControlsNamespace.AttachedOnLoad){
RadControlsNamespace.DomEventMixin.Initialize(this);
this.AttachDomEvent(window,"load","InitSplitters");
}
this.InitSplitters=function(){
__radsplitter__page_loaded=true;
for(var i=0;i<RadControlsNamespace.SplittersToLoad.length;i++){
RadControlsNamespace.SplittersToLoad[i].Init();
}
};
};
RadControlsNamespace.MarkSplitterToInit=function(_3){
if(typeof (RadControlsNamespace.__splitters__)=="undefined"){
return;
}
if(RadControlsNamespace.__splitters__[_3]){
RadControlsNamespace.__splitters__[_3].Inited=false;
}
};;//BEGIN_ATLAS_NOTIFY
if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
