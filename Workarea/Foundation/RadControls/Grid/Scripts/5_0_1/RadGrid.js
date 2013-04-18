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
}
var RadGridNamespace={};
RadGridNamespace.Prefix="grid_";
RadGridNamespace.InitializeClient=function(_22){
var _23=document.getElementById(_22+"AtlasCreation");
if(!_23){
return;
}
var _24=document.createElement("script");
if(navigator.userAgent.indexOf("Safari")!=-1){
_24.innerHTML=_23.innerHTML;
}else{
_24.text=_23.innerHTML;
}
if(!window.netscape){
document.body.appendChild(_24);
document.body.removeChild(_24);
}else{
document.body.insertBefore(_24,document.body.firstChild);
_24.parentNode.removeChild(_24);
}
_23.parentNode.removeChild(_23);
};
RadGridNamespace.AsyncRequest=function(_25,_26,_27,e){
var _29=window[_27];
if(_29!=null&&typeof (_29.AsyncRequest)=="function"){
_29.AsyncRequest(_25,_26,e);
}
};
RadGridNamespace.AsyncRequestWithOptions=function(_2a,_2b,e){
var _2d=window[_2b];
if(_2d!=null&&typeof (_2d.AsyncRequestWithOptions)=="function"){
_2d.AsyncRequestWithOptions(_2a,e);
}
};
RadGridNamespace.GetVisibleCols=function(_2e){
var _2f=0;
for(var i=0,l=_2e.length;i<l;i++){
if(_2e[i].style.display=="none"){
continue;
}
_2f++;
}
return _2f;
};
RadGridNamespace.HideShowCells=function(_32,_33,_34,_35){
var _36=RadGridNamespace.GetVisibleCols(_35);
for(var i=0,l=_32.rows.length;i<l;i++){
if(_32.rows[i].cells.length!=_36){
if(_32.rows[i].cells.length==1){
_32.rows[i].cells[0].colSpan=_36;
}else{
for(var j=0;j<_32.rows[i].cells.length;j++){
if(_32.rows[i].cells[j].colSpan>1&&j>=_33){
if(!_34){
_32.rows[i].cells[j].colSpan=_32.rows[i].cells[j].colSpan-1;
}else{
_32.rows[i].cells[j].colSpan=_32.rows[i].cells[j].colSpan+1;
}
break;
}
}
}
}
var _3a=_32.rows[i].cells[_33];
var _3b=(navigator.userAgent.toLowerCase().indexOf("safari")!=-1&&navigator.userAgent.indexOf("Mac")!=-1)?0:1;
if(!_34){
if(_3a!=null&&_3a.colSpan==_3b&&_3a.style.display!="none"){
_3a.style.display="none";
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
RadGridNamespace.HideShowSelect(_3a,_34);
}
}
}else{
if(_3a!=null&&_3a.colSpan==_3b&&_3a.style.display=="none"){
_3a.style.display=(window.netscape)?"table-cell":"";
}
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
RadGridNamespace.HideShowSelect(_3a,_34);
}
}
}
};
RadGridNamespace.HideShowSelect=function(_3c,_3d){
if(!_3c){
return;
}
var _3e=_3c.getElementsByTagName("select");
for(var i=0;i<_3e.length;i++){
_3e[i].style.display=(_3d)?"":"none";
}
};
RadGridNamespace.GetWidth=function(_40){
var _41;
if(window.getComputedStyle){
_41=window.getComputedStyle(_40,"").getPropertyValue("width");
}else{
if(_40.currentStyle){
_41=_40.currentStyle.width;
}else{
_41=_40.offsetWidth;
}
}
if(_41.toString().indexOf("%")!=-1){
_41=_40.offsetWidth;
}
if(_41.toString().indexOf("px")!=-1){
_41=parseInt(_41);
}
return _41;
};
RadGridNamespace.GetScrollBarWidth=function(){
try{
if(typeof (RadGridNamespace.scrollbarWidth)=="undefined"){
var _42,_43=0;
var _44=document.createElement("div");
_44.style.position="absolute";
_44.style.top="-1000px";
_44.style.left="-1000px";
_44.style.width="100px";
_44.style.overflow="auto";
var _45=document.createElement("div");
_45.style.width="1000px";
_44.appendChild(_45);
document.body.appendChild(_44);
_42=_44.offsetWidth;
_43=_44.clientWidth;
document.body.removeChild(document.body.lastChild);
RadGridNamespace.scrollbarWidth=_42-_43;
if(RadGridNamespace.scrollbarWidth<=0||_43==0){
RadGridNamespace.scrollbarWidth=16;
}
}
return RadGridNamespace.scrollbarWidth;
}
catch(error){
return false;
}
};
RadGridNamespace.GetTableColGroup=function(_46){
try{
return _46.getElementsByTagName("colgroup")[0];
}
catch(error){
return false;
}
};
RadGridNamespace.GetTableColGroupCols=function(_47){
try{
var _48=new Array();
var _49=_47.childNodes[0];
for(var i=0;i<_47.childNodes.length;i++){
if((_47.childNodes[i].tagName)&&(_47.childNodes[i].tagName.toLowerCase()=="col")){
_48[_48.length]=_47.childNodes[i];
}
}
return _48;
}
catch(error){
return false;
}
};
RadGridNamespace.Confirm=function(_4b,e){
if(!confirm(_4b)){
e.cancelBubble=true;
e.returnValue=false;
return false;
}
};
RadGridNamespace.SynchronizeWithWindow=function(){
};
RadGridNamespace.IsRightToLeft=function(_4d){
try{
while(_4d){
if(_4d.currentStyle&&_4d.currentStyle.direction.toLowerCase()=="rtl"){
return true;
}else{
if(getComputedStyle&&getComputedStyle(_4d,"").getPropertyValue("direction").toLowerCase()=="rtl"){
return true;
}else{
if(_4d.dir.toLowerCase()=="rtl"){
return true;
}
}
}
_4d=_4d.parentNode;
}
return false;
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError,this.OnError);
}
};
RadGridNamespace.FireEvent=function(_4e,_4f,_50){
try{
var _51=true;
if(typeof (_4e[_4f])=="string"){
eval(_4e[_4f]);
}else{
if(typeof (_4e[_4f])=="function"){
if(_50){
switch(_50.length){
case 1:
_51=_4e[_4f](_50[0]);
break;
case 2:
_51=_4e[_4f](_50[0],_50[1]);
break;
}
}else{
_51=_4e[_4f]();
}
}
}
if(typeof (_51)!="boolean"){
return true;
}else{
return _51;
}
}
catch(error){
throw error;
}
};
RadGridNamespace.CheckParentNodesFor=function(_52,_53){
while(_52){
if(_52==_53){
return true;
}
_52=_52.parentNode;
}
return false;
};
RadGridNamespace.GetCurrentElement=function(e){
if(!e){
var e=window.event;
}
var _55;
if(e.srcElement){
_55=e.srcElement;
}else{
_55=e.target;
}
return _55;
};
RadGridNamespace.GetEventPosX=function(e){
var x=e.clientX;
var _58=RadGridNamespace.GetCurrentElement(e);
while(_58.parentNode){
if(typeof (_58.parentNode.scrollLeft)=="number"){
x+=_58.parentNode.scrollLeft;
}
_58=_58.parentNode;
}
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
x=parseInt(x)-parseInt(document.body.currentStyle.marginLeft);
}
if(RadGridNamespace.IsRightToLeft(document.body)){
x=x-RadGridNamespace.GetScrollBarWidth();
}
return x;
};
RadGridNamespace.GetEventPosY=function(e){
var y=e.clientY;
var _5b=RadGridNamespace.GetCurrentElement(e);
while(_5b.parentNode){
if(typeof (_5b.parentNode.scrollTop)=="number"){
y+=_5b.parentNode.scrollTop;
}
_5b=_5b.parentNode;
}
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
y=parseInt(y)-parseInt(document.body.currentStyle.marginTop);
}
return y;
};
RadGridNamespace.IsChildOf=function(_5c,_5d){
while(_5c.parentNode){
if(_5c.parentNode==_5d){
return true;
}
_5c=_5c.parentNode;
}
return false;
};
RadGridNamespace.GetFirstParentByTagName=function(_5e,_5f){
while(_5e.parentNode){
if(_5e.tagName.toLowerCase()==_5f.toLowerCase()){
return _5e;
}
_5e=_5e.parentNode;
}
return null;
};
RadGridNamespace.FindScrollPosX=function(_60){
var x=0;
while(_60.parentNode){
if(typeof (_60.parentNode.scrollLeft)=="number"){
x+=_60.parentNode.scrollLeft;
}
_60=_60.parentNode;
}
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
x=parseInt(x)-parseInt(document.body.currentStyle.marginLeft);
}
return x;
};
RadGridNamespace.FindScrollPosY=function(_62){
var y=0;
while(_62.parentNode){
if(typeof (_62.parentNode.scrollTop)=="number"){
y+=_62.parentNode.scrollTop;
}
_62=_62.parentNode;
}
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
y=parseInt(y)-parseInt(document.body.currentStyle.marginTop);
}
return y;
};
RadGridNamespace.FindPosX=function(_64){
try{
var x=0;
var _66=0;
if(_64.offsetParent){
while(_64.offsetParent){
x+=_64.offsetLeft;
if(_64.currentStyle&&_64.currentStyle.borderLeftWidth&&_64.currentStyle.borderLeftWidth.indexOf("px")!=-1&&!window.opera){
_66+=parseInt(_64.currentStyle.borderLeftWidth);
}
_64=_64.offsetParent;
}
}else{
if(_64.x){
x+=_64.x;
}
}
if(document.compatMode=="BackCompat"||navigator.userAgent.indexOf("Safari")!=-1){
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
x=parseInt(x)-parseInt(document.body.currentStyle.marginLeft);
}
if(document.defaultView&&document.defaultView.getComputedStyle&&document.defaultView.getComputedStyle(document.body,"").marginLeft.indexOf("px")!=-1&&!window.opera){
x=parseInt(x)+parseInt(document.defaultView.getComputedStyle(document.body,"").marginLeft);
}
}
return x+_66;
}
catch(error){
return x;
}
};
RadGridNamespace.FindPosY=function(_67){
var y=0;
var _69=0;
if(_67.offsetParent){
while(_67.offsetParent){
y+=_67.offsetTop;
if(_67.currentStyle&&_67.currentStyle.borderTopWidth&&_67.currentStyle.borderTopWidth.indexOf("px")!=-1&&!window.opera){
_69+=parseInt(_67.currentStyle.borderTopWidth);
}
_67=_67.offsetParent;
}
}else{
if(_67.y){
y+=_67.y;
}
}
if(document.compatMode=="BackCompat"||navigator.userAgent.indexOf("Safari")!=-1){
if(document.body.currentStyle&&document.body.currentStyle&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
y=parseInt(y)-parseInt(document.body.currentStyle.marginTop);
}
if(document.defaultView&&document.defaultView.getComputedStyle&&document.defaultView.getComputedStyle(document.body,"").marginTop.indexOf("px")!=-1&&!window.opera){
y=parseInt(y)+parseInt(document.defaultView.getComputedStyle(document.body,"").marginTop);
}
}
return y+_69;
};
RadGridNamespace.GetNodeNextSiblingByTagName=function(_6a,_6b){
while((_6a!=null)&&(_6a.tagName!=_6b)){
_6a=_6a.nextSibling;
}
return _6a;
};
RadGridNamespace.GetNodeNextSibling=function(_6c){
while(_6c!=null){
if(_6c.nextSibling){
_6c=_6c.nextSibling;
}else{
_6c=null;
}
if(_6c){
if(_6c.nodeType==1){
break;
}
}
}
return _6c;
};
RadGridNamespace.DeleteSubString=function(_6d,_6e,_6f){
return _6d=_6d.substring(0,_6e)+_6d.substring(_6f+1,_6d.length);
};
RadGridNamespace.ClearDocumentEvents=function(){
if(document.onmousedown!=this.mouseDownHandler){
this.documentOnMouseDown=document.onmousedown;
}
if(document.onselectstart!=this.selectStartHandler){
this.documentOnSelectStart=document.onselectstart;
}
if(document.ondragstart!=this.dragStartHandler){
this.documentOnDragStart=document.ondragstart;
}
this.mouseDownHandler=function(e){
return false;
};
this.selectStartHandler=function(){
return false;
};
this.dragStartHandler=function(){
return false;
};
document.onmousedown=this.mouseDownHandler;
document.onselectstart=this.selectStartHandler;
document.ondragstart=this.dragStartHandler;
};
RadGridNamespace.RestoreDocumentEvents=function(){
if((typeof (this.documentOnMouseDown)=="function")&&(document.onmousedown!=this.mouseDownHandler)){
document.onmousedown=this.documentOnMouseDown;
}else{
document.onmousedown="";
}
if((typeof (this.documentOnSelectStart)=="function")&&(document.onselectstart!=this.selectStartHandler)){
document.onselectstart=this.documentOnSelectStart;
}else{
document.onselectstart="";
}
if((typeof (this.documentOnDragStart)=="function")&&(document.ondragstart!=this.dragStartHandler)){
document.ondragstart=this.documentOnDragStart;
}else{
document.ondragstart="";
}
};
RadGridNamespace.AddStyleSheet=function(_71){
if(RadGridNamespace.StyleSheets==null){
RadGridNamespace.StyleSheets={};
}
var _72=RadGridNamespace.StyleSheets[_71];
if(_72!=null){
return null;
}
var css=null;
var _74=null;
var _75=document.getElementsByTagName("head")[0];
if(window.netscape||navigator.userAgent.indexOf("Safari")!=-1){
css=document.createElement("style");
css.media="all";
css.type="text/css";
_75.appendChild(css);
}else{
try{
css=document.createStyleSheet();
}
catch(e){
return false;
}
}
var _76=document.styleSheets[document.styleSheets.length-1];
RadGridNamespace.StyleSheets[_71]=_76;
return _76;
};
RadGridNamespace.AddRule=function(ss,_78,_79){
try{
if(!ss){
return false;
}
if(ss.insertRule&&navigator.userAgent.indexOf("Safari")==-1){
var _7a=ss.insertRule(_78+" {"+_79+"}",ss.cssRules.length);
return ss.cssRules[ss.cssRules.length-1];
}
if(navigator.userAgent.indexOf("Safari")!=-1){
ss.addRule(_78,_79);
return ss.cssRules[ss.cssRules.length-1];
}
if(ss.addRule){
ss.addRule(_78,_79);
return true;
}
return false;
}
catch(e){
return false;
}
};
RadGridNamespace.addClassName=function(_7b,_7c){
var s=_7b.className;
var p=s.split(" ");
if(p.length==1&&p[0]==""){
p=[];
}
var l=p.length;
for(var i=0;i<l;i++){
if(p[i]==_7c){
return;
}
}
p[p.length]=_7c;
_7b.className=p.join(" ");
};
RadGridNamespace.removeClassName=function(_81,_82){
if(_81.className.replace(/^\s*|\s*$/g,"")==_82){
_81.className="";
return;
}
var _83=_81.className.split(" ");
var _84=[];
for(var i=0,l=_83.length;i<l;i++){
if(_83[i]==""){
continue;
}
if(_82.indexOf(_83[i])==-1){
_84[_84.length]=_83[i];
}
}
_81.className=_84.join(" ");
return;
_81.className=(_81.className.toString()==_82)?"":_81.className.replace(_82,"").replace(/\s*$/g,"");
return;
var p=s.split(" ");
var np=[];
var l=p.length;
var j=0;
for(var i=0;i<l;i++){
if(p[i]!=_82){
np[j++]=p[i];
}
}
_81.className=np.join(" ");
};
RadGridNamespace.CheckIsParentDisplay=function(_8a){
try{
while(_8a){
if(_8a.style){
if(_8a.currentStyle){
if(_8a.currentStyle.display=="none"){
return false;
}
}else{
if(_8a.style.display=="none"){
return false;
}
}
}
_8a=_8a.parentNode;
}
if(window.top){
if(window.top.location!=window.location){
return false;
}
}
return true;
}
catch(e){
return false;
}
};
RadGridNamespace.EncodeURI=function(_8b){
if(encodeURI){
return encodeURI(_8b);
}else{
return escape(_8b);
}
};
if(typeof (window.RadControlsNamespace)=="undefined"){
window.RadControlsNamespace=new Object();
}
RadControlsNamespace.AppendStyleSheet=function(_8c,_8d,_8e){
if(!_8e){
return;
}
if(!_8c){
document.write("<"+"link"+" rel='stylesheet' type='text/css' href='"+_8e+"' />");
}else{
var _8f=document.createElement("link");
_8f.rel="stylesheet";
_8f.type="text/css";
_8f.href=_8e;
var _90=document.getElementById(_8d+"StyleSheetHolder");
if(_90!=null){
document.getElementById(_8d+"StyleSheetHolder").appendChild(_8f);
}
}
};
RadGridNamespace.RadGrid=function(_91){
var _92=window[_91.ClientID];
if(_92!=null&&typeof (_92.Dispose)=="function"){
window.setTimeout(function(){
_92.Dispose();
},100);
}
RadControlsNamespace.DomEventMixin.Initialize(this);
this.AttachDomEvent(window,"unload","OnWindowUnload");
window[_91.ClientID]=this;
window["grid_"+_91.ClientID]=this;
if(RadGridNamespace.DocumentCanBeModified()){
this._constructor(_91);
}else{
this.objectData=_91;
this.AttachDomEvent(window,"load","OnWindowLoad");
}
};
RadGridNamespace.DocumentCanBeModified=function(){
return (RadGridNamespace.DocumentHasBeenFullyLoaded==true)||(document.readyState=="complete")||window.opera||window.netscape;
};
RadGridNamespace.RadGrid.prototype.OnWindowUnload=function(e){
this.Dispose();
};
RadGridNamespace.RadGrid.prototype.OnWindowLoad=function(e){
RadGridNamespace.DocumentHasBeenFullyLoaded=true;
this._constructor(this.objectData);
this.objectData=null;
};
RadGridNamespace.RadGrid.prototype._constructor=function(_95){
this.Type="RadGrid";
if(_95.ClientSettings){
this.InitializeEvents(_95.ClientSettings.ClientEvents);
}
RadGridNamespace.FireEvent(this,"OnGridCreating");
for(var _96 in _95){
this[_96]=_95[_96];
}
this.Initialize();
RadGridNamespace.FireEvent(this,"OnMasterTableViewCreating");
this.GridStyleSheet=RadGridNamespace.AddStyleSheet(this.ClientID);
if(this.ClientSettings&&this.ClientSettings.Scrolling.AllowScroll&&this.ClientSettings.Scrolling.UseStaticHeaders){
var ID=_95.MasterTableView.ClientID;
_95.MasterTableView.ClientID=ID+"_Header";
this.MasterTableViewHeader=new RadGridNamespace.RadGridTable(_95.MasterTableView);
this.MasterTableViewHeader._constructor(this);
if(document.getElementById(ID+"_Footer")){
_95.MasterTableView.ClientID=ID+"_Footer";
this.MasterTableViewFooter=new RadGridNamespace.RadGridTable(_95.MasterTableView);
this.MasterTableViewFooter._constructor(this);
}
_95.MasterTableView.ClientID=ID;
}
this.MasterTableView._constructor(this);
RadGridNamespace.FireEvent(this,"OnMasterTableViewCreated");
this.DetailTablesCollection=new Array();
this.LoadDetailTablesCollection(this.MasterTableView,1);
this.AttachDomEvents();
RadGridNamespace.FireEvent(this,"OnGridCreated");
this.InitializeFeatures(_95);
if(typeof (window.event)=="undefined"){
window.event=null;
}
};
RadGridNamespace.RadGrid.prototype.Dispose=function(){
if(this.Disposed){
return;
}
this.Disposed=true;
try{
RadGridNamespace.FireEvent(this,"OnGridDestroying");
this.DisposeDomEventHandlers();
this.DisposeEvents();
this.GridStyleSheet=null;
this.DisposeFeatures();
this.DisposeDetailTablesCollection(this.MasterTableView,1);
if(this.MasterTableViewHeader!=null){
this.MasterTableViewHeader.Dispose();
}
if(this.MasterTableViewFooter!=null){
this.MasterTableViewFooter.Dispose();
}
if(this.MasterTableView!=null){
this.MasterTableView.Dispose();
}
this.DisposeProperties();
}
catch(error){
}
};
RadGridNamespace.RadGrid.ClientEventNames={OnGridCreating:true,OnGridCreated:true,OnGridDestroying:true,OnMasterTableViewCreating:true,OnMasterTableViewCreated:true,OnTableCreating:true,OnTableCreated:true,OnTableDestroying:true,OnScroll:true,OnKeyPress:true,OnRequestStart:true,OnRequestEnd:true,OnRequestError:true,OnError:true,OnRowDeleting:true,OnRowDeleted:true};
RadGridNamespace.RadGrid.prototype.IsClientEventName=function(_98){
return RadGridNamespace.RadGrid.ClientEventNames[_98]==true;
};
RadGridNamespace.RadGrid.prototype.InitializeEvents=function(_99){
for(var _9a in _99){
if(typeof (_99[_9a])!="string"){
continue;
}
if(this.IsClientEventName(_9a)){
if(_99[_9a]!=""){
var _9b=_99[_9a];
if(_9b.indexOf("(")!=-1){
this[_9a]=_9b;
}else{
this[_9a]=eval(_9b);
}
}else{
this[_9a]=null;
}
}
}
};
RadGridNamespace.RadGrid.prototype.DisposeEvents=function(){
for(var _9c in RadGridNamespace.RadGrid.ClientEventNames){
this[_9c]=null;
}
};
RadGridNamespace.RadGrid.prototype.GetDetailTable=function(_9d,_9e){
if(_9d.HierarchyIndex==_9e){
return _9d;
}
if(_9d.DetailTables){
for(var i=0;i<_9d.DetailTables.length;i++){
var res=this.GetDetailTable(_9d.DetailTables[i],_9e);
if(res){
return res;
}
}
}
};
RadGridNamespace.RadGrid.prototype.LoadDetailTablesCollection=function(_a1,_a2){
try{
if(_a1.Controls[0]!=null&&_a1.Controls[0].Rows!=null){
for(var i=0;i<_a1.Controls[0].Rows.length;i++){
var _a4=_a1.Controls[0].Rows[i].ItemType;
if(_a4=="NestedView"){
var _a5=_a1.Controls[0].Rows[i].NestedTableViews;
for(var j=0;j<_a5.length;j++){
var _a7=_a5[j];
if(_a7.Visible){
var _a8=this.GetDetailTable(this.MasterTableView,_a7.HierarchyIndex);
RadGridNamespace.FireEvent(this,"OnTableCreating",[_a8]);
_a7._constructor(this);
this.DetailTablesCollection[this.DetailTablesCollection.length]=_a7;
if(_a7.AllowFilteringByColumn){
this.InitializeFilterMenu(_a7);
}
RadGridNamespace.FireEvent(this,"OnTableCreated",[_a7]);
}
this.LoadDetailTablesCollection(_a7,_a2+1);
}
}
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.DisposeDetailTablesCollection=function(_a9,_aa){
if(_a9.Controls[0]!=null&&_a9.Controls[0].Rows!=null){
for(var i=0;i<_a9.Controls[0].Rows.length;i++){
var _ac=_a9.Controls[0].Rows[i].ItemType;
if(_ac=="NestedView"){
var _ad=_a9.Controls[0].Rows[i].NestedTableViews;
for(var j=0;j<_ad.length;j++){
var _af=_ad[j];
_af.Dispose();
}
}
}
}
};
RadGridNamespace.RadGrid.prototype.AddRtlClass=function(){
if(RadGridNamespace.IsRightToLeft(this.Control)){
RadGridNamespace.addClassName(this.Control,"RadGridRTL_"+this.Skin);
}
};
RadGridNamespace.RadGrid.prototype.Initialize=function(){
this.Control=document.getElementById(this.ClientID);
if(this.Control==null){
return;
}
this.Control.tabIndex=0;
this.AddRtlClass();
this.GridDataDiv=document.getElementById(this.ClientID+"_GridData");
if(this.GroupPanel){
this.GroupPanelControl=document.getElementById(this.GroupPanel.ClientID+"_GroupPanel");
}
this.GridHeaderDiv=document.getElementById(this.ClientID+"_GridHeader");
this.GridFooterDiv=document.getElementById(this.ClientID+"_GridFooter");
this.PostDataValue=document.getElementById(this.ClientID+"PostDataValue");
this.LoadingTemplate=document.getElementById(this.ClientID+"_LoadingTemplate");
this.PagerControl=document.getElementById(this.MasterTableView.ClientID+"_Pager");
this.TopPagerControl=document.getElementById(this.MasterTableView.ClientID+"_TopPager");
if(this.LoadingTemplate){
this.LoadingTemplate.style.display="none";
if(this.GridDataDiv){
this.GridDataDiv.appendChild(this.LoadingTemplate);
}
}
};
RadGridNamespace.RadGrid.prototype.DisposeProperties=function(){
this.Control=null;
this.GridDataDiv=null;
this.GroupPanelControl=null;
this.GridHeaderDiv=null;
this.GridFooterDiv=null;
this.PostDataValue=null;
this.LoadingTemplate=null;
this.PagerControl=null;
};
RadGridNamespace.RadGrid.prototype.InitializeFeatures=function(_b0){
if(!this.MasterTableView.Control){
return;
}
if(this.GroupPanelControl!=null){
this.GroupPanelObject=new RadGridNamespace.RadGridGroupPanel(this.GroupPanelControl,this);
}
if(this.ClientSettings&&this.ClientSettings.Scrolling.AllowScroll){
this.InitializeDimensions();
this.InitializeScroll();
}
if(this.AllowFilteringByColumn||this.MasterTableView.AllowFilteringByColumn){
var _b1=(this.MasterTableViewHeader)?this.MasterTableViewHeader:this.MasterTableView;
this.InitializeFilterMenu(_b1);
}
if(this.ClientSettings&&this.ClientSettings.AllowKeyboardNavigation&&this.MasterTableView.Rows){
if(!this.MasterTableView.RenderActiveItemStyleClass||this.MasterTableView.RenderActiveItemStyleClass==""){
if(this.MasterTableView.RenderActiveItemStyle&&this.MasterTableView.RenderActiveItemStyle!=""){
RadGridNamespace.AddRule(this.GridStyleSheet,".ActiveItemStyle"+this.MasterTableView.ClientID+"1 td",this.MasterTableView.RenderActiveItemStyle);
}else{
RadGridNamespace.AddRule(this.GridStyleSheet,".ActiveItemStyle"+this.MasterTableView.ClientID+"2 td","background-color:#FFA07A;");
}
}
if(this.ActiveRow==null){
this.ActiveRow=this.MasterTableView.Rows[0];
}
this.SetActiveRow(this.ActiveRow);
}
if(this.ClientSettings&&this.ClientSettings.Slider!=null&&this.ClientSettings.Slider!=""){
eval(this.ClientSettings.Slider);
}
if(window[this.ClientID+"_Slider"]){
this.Slider=new RadGridNamespace.Slider(window[this.ClientID+"_Slider"]);
}
};
RadGridNamespace.RadGrid.prototype.DisposeFeatures=function(){
if(this.Slider!=null){
this.Slider.Dispose();
this.Slider=null;
}
if(this.GroupPanelControl!=null){
this.GroupPanelObject.Dispose();
this.GroupPanelControl=null;
}
if(this.AllowFilteringByColumn||this.MasterTableView.AllowFilteringByColumn){
var _b2=(this.MasterTableViewHeader)?this.MasterTableViewHeader:this.MasterTableView;
this.DisposeFilterMenu(_b2);
}
this.Control=null;
};
RadGridNamespace.RadGrid.prototype.AsyncRequest=function(_b3,_b4,e){
var _b6;
if(this.StatusBarSettings!=null&&this.StatusBarSettings.StatusLabelID!=null&&this.StatusBarSettings.StatusLabelID!=""){
var _b7=document.getElementById(this.StatusBarSettings.StatusLabelID);
if(_b7!=null){
_b6=_b7.innerHTML;
_b7.innerHTML=this.StatusBarSettings.LoadingText;
}
}
var _b8=this.ClientID;
this.OnRequestEndInternal=function(){
RadGridNamespace.FireEvent(window[_b8],"OnRequestEnd");
if(_b7){
_b7.innerHTML=_b6;
}
};
RadAjaxNamespace.AsyncRequest(_b3,_b4,_b8,e);
};
RadGridNamespace.RadGrid.prototype.AjaxRequest=function(_b9,_ba){
this.AsyncRequest(_b9,_ba);
};
RadGridNamespace.RadGrid.prototype.ClearSelectedRows=function(){
for(var i=0;i<this.DetailTablesCollection.length;i++){
var _bc=this.DetailTablesCollection[i];
_bc.ClearSelectedRows();
}
this.MasterTableView.ClearSelectedRows();
};
RadGridNamespace.RadGrid.prototype.AsyncRequestWithOptions=function(_bd,e){
RadAjaxNamespace.AsyncRequestWithOptions(_bd,this.ClientID,e);
};
RadGridNamespace.RadGrid.prototype.DeleteRow=function(_bf,_c0,e){
var _c2=(e.srcElement)?e.srcElement:e.target;
if(!_c2){
return;
}
var row=_c2.parentNode.parentNode;
var _c4=row.parentNode.parentNode;
var _c5=row.rowIndex;
var _c6=row.cells.length;
var _c7=this.GetTableObjectByID(_bf);
var _c8=this.GetRowObjectByRealRow(_c7,row);
var _c9={Row:_c8};
if(!RadGridNamespace.FireEvent(this,"OnRowDeleting",[_c7,_c9])){
return;
}
_c4.deleteRow(row.rowIndex);
for(var i=_c5;i<_c4.rows.length;i++){
if(_c4.rows[i].cells.length!=_c6&&_c4.rows[i].style.display!="none"){
_c4.deleteRow(i);
i--;
}else{
break;
}
}
if(_c4.tBodies[0].rows.length==1&&_c4.tBodies[0].rows[0].style.display=="none"){
_c4.tBodies[0].rows[0].style.display="";
}
this.PostDataValue.value+="DeletedRows,"+_bf+","+_c0+";";
RadGridNamespace.FireEvent(this,"OnRowDeleted",[_c7,_c9]);
};
RadGridNamespace.RadGrid.prototype.SelectRow=function(_cb,_cc,e){
var _ce=(e.srcElement)?e.srcElement:e.target;
if(!_ce){
return;
}
var row=RadGridNamespace.GetFirstParentByTagName(_ce,"tr");
var _d0=RadGridNamespace.GetFirstParentByTagName(row,"table");
var _d1=row.rowIndex;
var _d2;
if(_cb==this.MasterTableView.UID){
_d2=this.MasterTableView;
}else{
for(var i=0;i<this.DetailTablesCollection.length;i++){
if(this.DetailTablesCollection[i].ClientID==_d0.id){
_d2=this.DetailTablesCollection[i];
break;
}
}
}
if(_d2!=null){
if(this.AllowMultiRowSelection){
_d2.SelectRow(row,false);
}else{
_d2.SelectRow(row,true);
}
}
};
RadGridNamespace.RadGrid.prototype.SelectAllRows=function(_d4,_d5,e){
var _d7=(e.srcElement)?e.srcElement:e.target;
if(!_d7){
return;
}
var row=_d7.parentNode.parentNode;
var _d9=row.parentNode.parentNode;
var _da=row.rowIndex;
var _db;
if(_d4==this.MasterTableView.UID){
_db=this.MasterTableView;
}else{
for(var i=0;i<this.DetailTablesCollection.length;i++){
if(this.DetailTablesCollection[i].UID==_d4){
_db=this.DetailTablesCollection[i];
break;
}
}
}
if(_db!=null){
if(this.AllowMultiRowSelection){
if(_db==this.MasterTableViewHeader){
_db=this.MasterTableView;
}
_db.ClearSelectedRows();
if(_d7.checked){
for(var i=0;i<_db.Control.tBodies[0].rows.length;i++){
var row=_db.Control.tBodies[0].rows[i];
_db.SelectRow(row,false);
}
}else{
for(var i=0;i<_db.Control.tBodies[0].rows.length;i++){
var row=_db.Control.tBodies[0].rows[i];
_db.DeselectRow(row);
}
this.UpdateClientRowSelection();
}
}
}
};
RadGridNamespace.RadGrid.prototype.UpdateClientRowSelection=function(){
var _dd=this.MasterTableView.GetSelectedRowsIndexes();
this.SavePostData("SelectedRows",this.MasterTableView.ClientID,_dd);
for(var i=0;i<this.DetailTablesCollection.length;i++){
_dd=this.DetailTablesCollection[i].GetSelectedRowsIndexes();
this.SavePostData("SelectedRows",this.DetailTablesCollection[i].ClientID,_dd);
}
};
RadGridNamespace.RadGrid.prototype.HandleActiveRow=function(e){
if((this.AllowRowResize)||(this.AllowRowSelect)){
var _e0=this.GetCellFromPoint(e);
if((_e0!=null)&&(_e0.parentNode.id!="")&&(_e0.parentNode.id!=-1)&&(_e0.cellIndex==0)){
var _e1=_e0.parentNode.parentNode.parentNode;
this.SetActiveRow(_e1,_e0.parentNode.rowIndex);
}
}
};
RadGridNamespace.RadGrid.prototype.SetActiveRow=function(_e2){
if(_e2==null){
return;
}
if(_e2.Owner.RenderActiveItemStyle){
RadGridNamespace.removeClassName(this.ActiveRow.Control,"ActiveItemStyle"+_e2.Owner.ClientID+"1");
}else{
RadGridNamespace.removeClassName(this.ActiveRow.Control,"ActiveItemStyle"+_e2.Owner.ClientID+"2");
}
RadGridNamespace.removeClassName(this.ActiveRow.Control,_e2.Owner.RenderActiveItemStyleClass);
if(this.ActiveRow.Control.style.cssText==_e2.Owner.RenderActiveItemStyle){
this.ActiveRow.Control.style.cssText="";
}
this.ActiveRow=_e2;
if(!this.ActiveRow.Owner.RenderActiveItemStyleClass||this.ActiveRow.Owner.RenderActiveItemStyleClass==""){
if(this.ActiveRow.Owner.RenderActiveItemStyle&&this.ActiveRow.Owner.RenderActiveItemStyle!=""){
RadGridNamespace.addClassName(this.ActiveRow.Control,"ActiveItemStyle"+this.ActiveRow.Owner.ClientID+"1");
}else{
RadGridNamespace.addClassName(this.ActiveRow.Control,"ActiveItemStyle"+this.ActiveRow.Owner.ClientID+"2");
}
}else{
RadGridNamespace.addClassName(this.ActiveRow.Control,this.ActiveRow.Owner.RenderActiveItemStyleClass);
}
this.SavePostData("ActiveRow",this.ActiveRow.Owner.ClientID,this.ActiveRow.RealIndex);
};
RadGridNamespace.RadGrid.prototype.GetNextRow=function(_e3,_e4){
if(_e3!=null){
if(_e3.tBodies[0].rows[_e4]!=null){
while(_e3.tBodies[0].rows[_e4]!=null){
_e4++;
if(_e4<=(_e3.tBodies[0].rows.length-1)){
return _e3.tBodies[0].rows[_e4];
}else{
return null;
}
}
}
}
};
RadGridNamespace.RadGrid.prototype.GetPreviousRow=function(_e5,_e6){
if(_e5!=null){
if(_e5.tBodies[0].rows[_e6]!=null){
while(_e5.tBodies[0].rows[_e6]!=null){
_e6--;
if(_e6>=0){
return _e5.tBodies[0].rows[_e6];
}else{
return null;
}
}
}
}
};
RadGridNamespace.RadGrid.prototype.GetNextHierarchicalRow=function(_e7,_e8){
if(_e7!=null){
if(_e7.tBodies[0].rows[_e8]!=null){
_e8++;
var row=_e7.tBodies[0].rows[_e8];
if(_e7.tBodies[0].rows[_e8]!=null){
if((row.cells[1]!=null)&&(row.cells[2]!=null)){
if((row.cells[1].getElementsByTagName("table").length>0)||(row.cells[2].getElementsByTagName("table").length>0)){
var _ea=this.GetNextRow(row.cells[2].firstChild,0);
return _ea;
}else{
return null;
}
}
}
}
}
};
RadGridNamespace.RadGrid.prototype.GetPreviousHierarchicalRow=function(_eb,_ec){
if(_eb!=null){
if(_eb.parentNode!=null){
if(_eb.parentNode.tagName.toLowerCase()=="td"){
var _ed=_eb.parentNode.parentNode.parentNode.parentNode;
var _ee=_eb.parentNode.parentNode.rowIndex;
return this.GetPreviousRow(_ed,_ee);
}else{
return null;
}
}else{
return this.GetPreviousRow(_eb,_ec);
}
}
};
RadGridNamespace.RadGrid.prototype.HandleCellEdit=function(e){
var _f0=RadGridNamespace.GetCurrentElement(e);
var _f1=RadGridNamespace.GetFirstParentByTagName(_f0,"td");
if(_f1!=null){
_f0=_f1;
var _f2=_f0.parentNode.parentNode.parentNode;
var _f3=this.GetTableObjectByID(_f2.id);
if((_f3!=null)&&(_f3.Columns.length>0)&&(_f3.Columns[_f0.cellIndex]!=null)){
if(_f3.Columns[_f0.cellIndex].ColumnType!="GridBoundColumn"){
return;
}
this.EditedCell=_f3.Control.rows[_f0.parentNode.rowIndex].cells[_f0.cellIndex];
this.CellEditor=new RadGridNamespace.RadGridCellEditor(this.EditedCell,_f3.Columns[_f0.cellIndex],this);
}
}
};
RadGridNamespace.RadGridCellEditor=function(_f4,_f5,_f6){
if(_f6.CellEditor){
return;
}
this.Control=document.createElement("input");
this.Control.style.border="1px groove";
this.Control.style.width="100%";
this.Control.value=_f4.innerHTML;
this.OldValue=this.Control.value;
_f4.innerHTML="";
var _f7=this;
this.Control.onblur=function(e){
if(!e){
var e=window.event;
}
_f4.removeChild(this);
_f4.innerHTML=this.value;
if(this.value!=_f7.OldValue){
alert(1);
}
_f6.CellEditor=null;
};
_f4.appendChild(this.Control);
if(this.Control.focus){
this.Control.focus();
}
};
if(!("console" in window)||!("firebug" in console)){
var names=["log","debug","info","warn","error","assert","dir","dirxml","group","groupEnd","time","timeEnd","count","trace","profile","profileEnd"];
window.console={};
for(var i=0;i<names.length;++i){
window.console[names[i]]=function(){
};
}
}
RadGridNamespace.Error=function(_f9,_fa,_fb){
if((!_f9)||(!_fa)||(!_fb)){
return false;
}
this.Message=_f9.message;
if(_fb!=null){
if("string"==typeof (_fb)){
try{
eval(_fb);
}
catch(e){
var _fc="";
_fc="";
_fc+="Telerik RadGrid Error:\r\n";
_fc+="-----------------\r\n";
_fc+="Message: \""+e.message+"\"\r\n";
_fc+="Raised by: "+_fa.Type+"\r\n";
alert(_fc);
}
}else{
if("function"==typeof (_fb)){
try{
_fb(this);
}
catch(e){
var _fc="";
_fc="";
_fc+="Telerik RadGrid Error:\r\n";
_fc+="-----------------\r\n";
_fc+="Message: \""+e.message+"\"\r\n";
_fc+="Raised by: "+_fa.Type+"\r\n";
alert(_fc);
}
}
}
}else{
this.Owner=_fa;
for(var _fd in _f9){
this[_fd]=_f9[_fd];
}
this.Message="";
this.Message+="Telerik RadGrid Error:\r\n";
this.Message+="-----------------\r\n";
this.Message+="Message: \""+_f9.message+"\"\r\n";
this.Message+="Raised by: "+_fa.Type+"\r\n";
alert(this.Message);
}
};
RadGridNamespace.RadGrid.prototype.GetTableObjectByID=function(id){
if(this.MasterTableView.ClientID==id||this.MasterTableView.UID==id){
return this.MasterTableView;
}else{
for(var i=0;i<this.DetailTablesCollection.length;i++){
if(this.DetailTablesCollection[i].ClientID==id||this.DetailTablesCollection[i].UID==id){
return this.DetailTablesCollection[i];
}
}
}
if(this.MasterTableViewHeader!=null){
if(this.MasterTableViewHeader.ClientID==id||this.MasterTableViewHeader.UID==id){
return table=this.MasterTableViewHeader;
}
}
};
RadGridNamespace.RadGrid.prototype.GetRowObjectByRealRow=function(_100,row){
if(_100.Rows!=null){
for(var i=0;i<_100.Rows.length;i++){
if(_100.Rows[i].Control==row){
return _100.Rows[i];
}
}
}
};
RadGridNamespace.RadGrid.prototype.SavePostData=function(){
try{
var _103=new String();
for(var i=0;i<arguments.length;i++){
_103+=arguments[i]+",";
}
_103=_103.substring(0,_103.length-1);
if(this.PostDataValue!=null){
switch(arguments[0]){
case "ReorderedColumns":
this.PostDataValue.value+=_103+";";
break;
case "HidedColumns":
var _105=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
_105="ShowedColumns"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
break;
case "ShowedColumns":
var _105=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
_105="HidedColumns"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
break;
case "HidedRows":
var _105=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
_105="ShowedRows"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
break;
case "ShowedRows":
var _105=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
_105="HidedRows"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
break;
case "ResizedColumns":
var _105=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
break;
case "ResizedRows":
var _105=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
break;
case "ResizedControl":
var _105=arguments[0]+","+arguments[1];
this.UpdatePostData(_103,_105);
break;
case "ClientCreated":
var _105=arguments[0]+","+arguments[1];
this.UpdatePostData(_103,_105);
break;
case "ScrolledControl":
var _105=arguments[0]+","+arguments[1];
this.UpdatePostData(_103,_105);
break;
case "AJAXScrolledControl":
var _105=arguments[0]+","+arguments[1];
this.UpdatePostData(_103,_105);
break;
case "SelectedRows":
var _105=arguments[0]+","+arguments[1]+",";
this.UpdatePostData(_103,_105);
break;
case "EditRow":
var _105=arguments[0]+","+arguments[1];
this.UpdatePostData(_103,_105);
break;
case "ActiveRow":
var _105=arguments[0]+","+arguments[1];
this.UpdatePostData(_103,_105);
break;
case "CollapsedRows":
var _105=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
_105="ExpandedRows"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
break;
case "ExpandedRows":
var _105=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
_105="CollapsedRows"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
break;
case "CollapsedGroupRows":
var _105=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
_105="ExpandedGroupRows"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
break;
case "ExpandedGroupRows":
var _105=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
_105="CollapsedGroupRows"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_103,_105);
break;
default:
this.UpdatePostData(_103,_103);
break;
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.UpdatePostData=function(_106,_107){
var _108,_109=new Array();
_108=this.PostDataValue.value.split(";");
for(var i=0;i<_108.length;i++){
if(_108[i].indexOf(_107)==-1){
_109[_109.length]=_108[i];
}
}
this.PostDataValue.value=_109.join(";");
this.PostDataValue.value+=_106+";";
};
RadGridNamespace.RadGrid.prototype.DeletePostData=function(_10b,_10c){
var _10d,_10e=new Array();
_10d=this.PostDataValue.value.split(";");
for(var i=0;i<_10d.length;i++){
if(_10d[i].indexOf(_10c)==-1){
_10e[_10e.length]=_10d[i];
}
}
this.PostDataValue.value=_10e.join(";");
};
RadGridNamespace.RadGrid.prototype.HandleDragAndDrop=function(e,_111){
try{
var _112=this;
if(_113!=null&&_113.Columns.length>0&&_113.Columns[_114]!=null&&!_113.Columns[_114].Reorderable){
this.Control.style.cursor="no-drop";
this.DisableDrop();
}else{
this.Control.style.cursor="";
}
if(this.MoveHeaderDiv!=null&&_111!=null&&_111.tagName.toLowerCase()!="th"&&!RadGridNamespace.IsChildOf(_111,this.MoveHeaderDivRefCell.parentNode)&&!(this.GroupPanelControl!=null&&RadGridNamespace.IsChildOf(_111,this.GroupPanelControl))){
this.Control.style.cursor="no-drop";
this.DisableDrop();
}else{
this.Control.style.cursor="";
}
if((_111!=null)&&(_111.tagName.toLowerCase()=="th")){
var _115=_111.parentNode.parentNode.parentNode;
var _113=this.GetTableObjectByID(_115.id);
var _114=RadGridNamespace.GetRealCellIndex(_113,_111);
if((_113!=null)&&(_113.Columns.length>0)&&(_113.Columns[_114]!=null)&&((_113.Columns[_114].Reorderable)||(_113.Owner.ClientSettings.AllowDragToGroup&&_113.Columns[_114].Groupable))){
var _116=RadGridNamespace.GetEventPosX(e);
var _117=RadGridNamespace.FindPosX(_111);
var endX=_117+_111.offsetWidth;
this.ResizeTolerance=5;
var _119=_111.title;
var _11a=_111.style.cursor;
if(!((_116>=endX-this.ResizeTolerance)&&(_116<=endX+this.ResizeTolerance))){
if(this.MoveHeaderDiv){
if(this.MoveHeaderDiv.innerHTML!=_111.innerHTML){
_111.title=this.ClientSettings.ClientMessages.DropHereToReorder;
_111.style.cursor="default";
if(_111.parentNode.parentNode.parentNode==this.MoveHeaderDivRefCell.parentNode.parentNode.parentNode){
this.MoveReorderIndicators(e,_111);
}else{
this.DisableDrop();
}
}
}else{
_111.title=this.ClientSettings.ClientMessages.DragToGroupOrReorder;
_111.style.cursor="move";
}
this.AttachDomEvent(_111,"mousedown","OnDragDropMouseDown");
}else{
_111.style.cursor=_11a;
_111.title="";
}
}
}
if(_113!=null&&_113.Columns.length>0&&_113.Columns[_114]!=null&&!_113.Columns[_114].Reorderable){
this.Control.style.cursor="no-drop";
this.DisableDrop();
}
if(this.MoveHeaderDiv!=null){
this.MoveHeaderDiv.style.visibility="";
this.MoveHeaderDiv.style.display="";
RadGridNamespace.RadGrid.PositionDragElement(this.MoveHeaderDiv,e);
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.DisableDrop=function(e){
if(this.ReorderIndicator1!=null){
this.ReorderIndicator1.style.visibility="hidden";
this.ReorderIndicator1.style.display="none";
this.ReorderIndicator1.style.position="absolute";
}
if(this.ReorderIndicator2!=null){
this.ReorderIndicator2.style.visibility=this.ReorderIndicator1.style.visibility;
this.ReorderIndicator2.style.display=this.ReorderIndicator1.style.display;
this.ReorderIndicator2.style.position=this.ReorderIndicator1.style.position;
}
};
RadGridNamespace.RadGrid.PositionDragElement=function(_11c,_11d){
_11c.style.top=_11d.clientY+document.documentElement.scrollTop+document.body.scrollTop+1+"px";
_11c.style.left=_11d.clientX+document.documentElement.scrollLeft+document.body.scrollLeft+1+"px";
};
RadGridNamespace.RadGrid.prototype.OnDragDropMouseDown=function(e){
var _11f=RadGridNamespace.GetCurrentElement(e);
var _120=false;
var form=document.getElementById(this.FormID);
if(form!=null&&form["__EVENTTARGET"]!=null&&form["__EVENTTARGET"].value!=""){
_120=true;
}
if((_11f.tagName.toLowerCase()=="input"&&_11f.type.toLowerCase()=="text")||(_11f.tagName.toLowerCase()=="textarea")){
return;
}
_11f=RadGridNamespace.GetFirstParentByTagName(_11f,"th");
if(_11f.tagName.toLowerCase()=="th"&&!this.IsResize){
if(((window.netscape||window.opera||navigator.userAgent.indexOf("Safari")!=-1)&&(e.button==0))||(e.button==1)){
this.CreateDragAndDrop(e,_11f);
}
RadGridNamespace.ClearDocumentEvents();
this.DetachDomEvent(_11f,"mousedown","OnDragDropMouseDown");
this.AttachDomEvent(document,"mouseup","OnDragDropMouseUp");
if(this.GroupPanelControl!=null){
this.AttachDomEvent(this.GroupPanelControl,"mouseup","OnDragDropMouseUp");
}
}
};
RadGridNamespace.RadGrid.prototype.OnDragDropMouseUp=function(e){
this.DetachDomEvent(document,"mouseup","OnDragDropMouseUp");
if(this.GroupPanelControl!=null){
this.DetachDomEvent(this.GroupPanelControl,"mouseup","OnDragDropMouseUp");
}
this.FireDropAction(e);
this.DestroyDragAndDrop(e);
RadGridNamespace.RestoreDocumentEvents();
};
RadGridNamespace.CopyAttributes=function(_123,_124){
for(var i=0;i<_124.attributes.length;i++){
try{
if(_124.attributes[i].name.toLowerCase()=="id"){
continue;
}
if(_124.attributes[i].value!=null&&_124.attributes[i].value!="null"&&_124.attributes[i].value!=""){
_123.setAttribute(_124.attributes[i].name,_124.attributes[i].value);
}
}
catch(e){
continue;
}
}
};
RadGridNamespace.RadGrid.prototype.CreateDragAndDrop=function(e,_127){
this.MoveHeaderDivRefCell=_127;
this.MoveHeaderDiv=document.createElement("div");
var _128=document.createElement("table");
if(this.MoveHeaderDiv.mergeAttributes){
this.MoveHeaderDiv.mergeAttributes(this.Control);
}else{
RadGridNamespace.CopyAttributes(this.MoveHeaderDiv,this.Control);
}
if(_128.mergeAttributes){
_128.mergeAttributes(this.MasterTableView.Control);
}else{
RadGridNamespace.CopyAttributes(_128,this.MasterTableView.Control);
}
_128.style.margin="0px";
_128.style.height=_127.offsetHeight+"px";
_128.style.width=_127.offsetWidth+"px";
var _129=document.createElement("thead");
var tr=document.createElement("tr");
_128.appendChild(_129);
_129.appendChild(tr);
tr.appendChild(_127.cloneNode(true));
this.MoveHeaderDiv.appendChild(_128);
document.body.appendChild(this.MoveHeaderDiv);
this.MoveHeaderDiv.style.height=_127.offsetHeight+"px";
this.MoveHeaderDiv.style.width=_127.offsetWidth+"px";
this.MoveHeaderDiv.style.position="absolute";
RadGridNamespace.RadGrid.PositionDragElement(this.MoveHeaderDiv,e);
if(window.netscape){
this.MoveHeaderDiv.style.MozOpacity=3/4;
}else{
this.MoveHeaderDiv.style.filter="alpha(opacity=75);";
}
this.MoveHeaderDiv.style.cursor="move";
this.MoveHeaderDiv.style.visibility="hidden";
this.MoveHeaderDiv.style.display="none";
this.MoveHeaderDiv.style.fontWeight="bold";
this.MoveHeaderDiv.onmousedown=null;
RadGridNamespace.ClearDocumentEvents();
if(this.ClientSettings.AllowColumnsReorder){
this.CreateReorderIndicators(_127);
}
};
RadGridNamespace.RadGrid.prototype.DestroyDragAndDrop=function(){
if(this.MoveHeaderDiv!=null){
var _12b=this.MoveHeaderDiv.parentNode;
_12b.removeChild(this.MoveHeaderDiv);
this.MoveHeaderDiv.onmouseup=null;
this.MoveHeaderDiv.onmousemove=null;
this.MoveHeaderDiv=null;
this.MoveHeaderDivRefCell=null;
this.DragCellIndex=null;
RadGridNamespace.RestoreDocumentEvents();
this.DestroyReorderIndicators();
}
};
RadGridNamespace.RadGrid.prototype.FireDropAction=function(e){
if((this.MoveHeaderDiv!=null)&&(this.MoveHeaderDiv.style.display!="none")){
var _12d=RadGridNamespace.GetCurrentElement(e);
if((_12d!=null)&&(this.MoveHeaderDiv!=null)){
if(_12d!=this.MoveHeaderDivRefCell){
var _12e=this.GetTableObjectByID(this.MoveHeaderDivRefCell.parentNode.parentNode.parentNode.id);
var _12f=_12e.HeaderRow;
if(RadGridNamespace.IsChildOf(_12d,_12f)){
if(_12d.tagName.toLowerCase()!="th"){
_12d=RadGridNamespace.GetFirstParentByTagName(_12d,"th");
}
var _130=_12d.parentNode.parentNode.parentNode;
var _131=this.MoveHeaderDivRefCell.parentNode.parentNode.parentNode;
if(_130.id==_131.id){
var _132=this.GetTableObjectByID(_130.id);
var _133=_12d.cellIndex;
if((window.attachEvent&&!window.opera&&!window.netscape)||navigator.userAgent.indexOf("Safari")!=-1){
_133=RadGridNamespace.GetRealCellIndex(_132,_12d);
}
var _134=this.MoveHeaderDivRefCell.cellIndex;
if((window.attachEvent&&!window.opera&&!window.netscape)||navigator.userAgent.indexOf("Safari")!=-1){
_134=RadGridNamespace.GetRealCellIndex(_132,this.MoveHeaderDivRefCell);
}
if(!_132||!_132.Columns[_133]){
return;
}
if(!_132.Columns[_133].Reorderable){
return;
}
_132.SwapColumns(_133,_134,(this.ClientSettings.ColumnsReorderMethod!="Reorder"));
if(this.ClientSettings.ColumnsReorderMethod=="Reorder"){
if((!this.ClientSettings.ReorderColumnsOnClient)&&(this.ClientSettings.PostBackReferences.PostBackColumnsReorder!="")){
eval(this.ClientSettings.PostBackReferences.PostBackColumnsReorder);
}
}
}
}else{
if(RadGridNamespace.CheckParentNodesFor(_12d,this.GroupPanelControl)){
if((this.ClientSettings.PostBackReferences.PostBackGroupByColumn!="")&&(this.ClientSettings.AllowDragToGroup)){
var _132=this.GetTableObjectByID(this.MoveHeaderDivRefCell.parentNode.parentNode.parentNode.id);
var _135=this.MoveHeaderDivRefCell.cellIndex;
if((window.attachEvent&&!window.opera&&!window.netscape)||navigator.userAgent.indexOf("Safari")!=-1){
_135=RadGridNamespace.GetRealCellIndex(_132,this.MoveHeaderDivRefCell);
}
var _136=_132.Columns[_135].RealIndex;
if(_132.Columns[_135].Groupable){
if(_132==this.MasterTableViewHeader){
this.SavePostData("GroupByColumn",this.MasterTableView.ClientID,_136);
}else{
this.SavePostData("GroupByColumn",_132.ClientID,_136);
}
eval(this.ClientSettings.PostBackReferences.PostBackGroupByColumn);
}
}
}
}
}
}
}
};
RadGridNamespace.GetRealCellIndex=function(_137,cell){
for(var i=0;i<_137.Columns.length;i++){
if(_137.Columns[i].Control==cell){
return i;
}
}
};
RadGridNamespace.RadGrid.prototype.CreateReorderIndicators=function(_13a){
if((this.ReorderIndicator1==null)&&(this.ReorderIndicator2==null)){
var _13b=this.MoveHeaderDivRefCell.parentNode.parentNode.parentNode;
var _13c=this.GetTableObjectByID(_13b.id);
var _13d=_13c.HeaderRow;
if(!RadGridNamespace.IsChildOf(_13a,_13d)){
return;
}
this.ReorderIndicator1=document.createElement("span");
this.ReorderIndicator2=document.createElement("span");
if(this.Skin==""||this.Skin=="None"){
this.ReorderIndicator1.innerHTML="&darr;";
this.ReorderIndicator2.innerHTML="&uarr;";
}else{
this.ReorderIndicator1.className="TopReorderIndicator_"+this.Skin;
this.ReorderIndicator2.className="BottomReorderIndicator_"+this.Skin;
this.ReorderIndicator1.style.width=this.ReorderIndicator1.style.height=this.ReorderIndicator2.style.width=this.ReorderIndicator2.style.height="10px";
}
this.ReorderIndicator1.style.backgroundColor="transparent";
this.ReorderIndicator1.style.color="darkblue";
this.ReorderIndicator1.style.font="bold 18px Arial";
this.ReorderIndicator2.style.backgroundColor=this.ReorderIndicator1.style.backgroundColor;
this.ReorderIndicator2.style.color=this.ReorderIndicator1.style.color;
this.ReorderIndicator2.style.font=this.ReorderIndicator1.style.font;
this.ReorderIndicator1.style.top=RadGridNamespace.FindPosY(_13a)-this.ReorderIndicator1.offsetHeight+"px";
this.ReorderIndicator1.style.left=RadGridNamespace.FindPosX(_13a)+"px";
this.ReorderIndicator2.style.top=RadGridNamespace.FindPosY(_13a)+_13a.offsetHeight+"px";
this.ReorderIndicator2.style.left=this.ReorderIndicator1.style.left;
this.ReorderIndicator1.style.visibility="hidden";
this.ReorderIndicator1.style.display="none";
this.ReorderIndicator1.style.position="absolute";
this.ReorderIndicator2.style.visibility=this.ReorderIndicator1.style.visibility;
this.ReorderIndicator2.style.display=this.ReorderIndicator1.style.display;
this.ReorderIndicator2.style.position=this.ReorderIndicator1.style.position;
document.body.appendChild(this.ReorderIndicator1);
document.body.appendChild(this.ReorderIndicator2);
}
};
RadGridNamespace.RadGrid.prototype.DestroyReorderIndicators=function(){
if((this.ReorderIndicator1!=null)&&(this.ReorderIndicator2!=null)){
document.body.removeChild(this.ReorderIndicator1);
document.body.removeChild(this.ReorderIndicator2);
this.ReorderIndicator1=null;
this.ReorderIndicator2=null;
}
};
RadGridNamespace.RadGrid.prototype.MoveReorderIndicators=function(e,_13f){
if((this.ReorderIndicator1!=null)&&(this.ReorderIndicator2!=null)){
this.ReorderIndicator1.style.visibility="visible";
this.ReorderIndicator1.style.display="";
this.ReorderIndicator2.style.visibility="visible";
this.ReorderIndicator2.style.display="";
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1){
additionalTop=parseInt(document.body.currentStyle.marginTop);
}
additionalTop-=(this.Skin==""||this.Skin=="None"&&navigator.userAgent.indexOf("Safari")==-1)?10:5;
if(navigator.userAgent.indexOf("Safari")!=-1){
additionalTop-=10;
}
this.ReorderIndicator1.style.top=RadGridNamespace.FindPosY(_13f)-this.ReorderIndicator1.offsetHeight/2+additionalTop+"px";
this.ReorderIndicator1.style.left=RadGridNamespace.FindPosX(_13f)-RadGridNamespace.FindScrollPosX(_13f)+document.documentElement.scrollLeft+document.body.scrollLeft+"px";
if(parseInt(this.ReorderIndicator1.style.left)<RadGridNamespace.FindPosX(this.Control)){
this.ReorderIndicator1.style.left=RadGridNamespace.FindPosX(this.Control)+5;
}
this.ReorderIndicator2.style.top=parseInt(this.ReorderIndicator1.style.top)+_13f.offsetHeight+this.ReorderIndicator2.offsetHeight+"px";
this.ReorderIndicator2.style.left=this.ReorderIndicator1.style.left;
this.ReorderIndicator2.style.zIndex=this.ReorderIndicator1.style.zIndex=99999;
}
};
RadGridNamespace.RadGrid.prototype.AttachDomEvents=function(){
try{
this.AttachDomEvent(this.Control,"mousemove","OnMouseMove");
this.AttachDomEvent(this.Control,"keydown","OnKeyDown");
this.AttachDomEvent(this.Control,"keyup","OnKeyUp");
this.AttachDomEvent(this.Control,"click","OnClick");
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.OnMouseMove=function(e){
try{
var _141=RadGridNamespace.GetCurrentElement(e);
if(this.ClientSettings.Resizing.AllowRowResize){
this.DetectResizeCursorsOnRows(e,_141);
this.MoveRowResizer(e);
}
if((this.ClientSettings.AllowDragToGroup)||(this.ClientSettings.AllowColumnsReorder)){
this.HandleDragAndDrop(e,_141);
}
}
catch(error){
return false;
}
};
RadGridNamespace.RadGrid.prototype.OnKeyDown=function(e){
var _143={KeyCode:e.keyCode,IsShiftPressed:e.shiftKey,IsCtrlPressed:e.ctrlKey,IsAltPressed:e.altKey,Event:e};
if(!RadGridNamespace.FireEvent(this,"OnKeyPress",[_143])){
return;
}
if(e.keyCode==16){
this.IsShiftPressed=true;
}
if(e.keyCode==17){
this.IsCtrlPressed=true;
}
if(this.ClientSettings&&this.ClientSettings.AllowKeyboardNavigation){
this.ActiveRow.HandleActiveRow(e);
}
if(e.keyCode==27&&this.MoveHeaderDiv){
this.DestroyDragAndDrop();
}
};
RadGridNamespace.RadGrid.prototype.OnClick=function(e){
};
RadGridNamespace.RadGrid.prototype.OnKeyUp=function(e){
if(e.keyCode==16){
this.IsShiftPressed=false;
}
if(e.keyCode==17){
this.IsCtrlPressed=false;
}
};
RadGridNamespace.RadGrid.prototype.DetectResizeCursorsOnRows=function(e,_147){
try{
var _148=this;
if((_147!=null)&&(_147.tagName.toLowerCase()=="td")&&!this.MoveHeaderDiv){
var _149=_147.parentNode.parentNode.parentNode;
var _14a=this.GetTableObjectByID(_149.id);
if(_14a!=null){
if(_14a.Columns!=null){
if(_14a.Columns[_147.cellIndex].ColumnType!="GridRowIndicatorColumn"){
return;
}
}
if(!_14a.Control.tBodies[0]){
return;
}
var _14b=this.GetRowObjectByRealRow(_14a,_147.parentNode);
if(_14b!=null){
var _14c=RadGridNamespace.GetEventPosY(e);
var _14d=RadGridNamespace.FindPosY(_147);
var endY=_14d+_147.offsetHeight;
this.ResizeTolerance=5;
var _14f=_147.title;
if((_14c>endY-this.ResizeTolerance)&&(_14c<endY+this.ResizeTolerance)){
_147.style.cursor="n-resize";
_147.title=this.ClientSettings.ClientMessages.DragToResize;
this.AttachDomEvent(_147,"mousedown","OnResizeMouseDown");
}else{
_147.style.cursor="default";
_147.title="";
this.DetachDomEvent(_147,"mousedown","OnResizeMouseDown");
}
}
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.OnResizeMouseDown=function(e){
this.CreateRowResizer(e);
RadGridNamespace.ClearDocumentEvents();
this.AttachDomEvent(document,"mouseup","OnResizeMouseUp");
};
RadGridNamespace.RadGrid.prototype.OnResizeMouseUp=function(e){
this.DetachDomEvent(document,"mouseup","OnResizeMouseUp");
this.DestroyRowResizerAndResizeRow(e,true);
RadGridNamespace.RestoreDocumentEvents();
};
RadGridNamespace.RadGrid.prototype.CreateRowResizer=function(e){
try{
this.DestroyRowResizer();
var _153=RadGridNamespace.GetCurrentElement(e);
if((_153!=null)&&(_153.tagName.toLowerCase()=="td")){
if(_153.cellIndex>0){
var _154=_153.parentNode.rowIndex;
_153=_153.parentNode.parentNode.parentNode.rows[_154].cells[0];
}
this.RowResizer=null;
this.CellToResize=_153;
var _155=_153.parentNode.parentNode.parentNode;
var _156=this.GetTableObjectByID(_155.id);
this.RowResizer=document.createElement("div");
this.RowResizer.style.backgroundColor="navy";
this.RowResizer.style.height="1px";
this.RowResizer.style.fontSize="1";
this.RowResizer.style.position="absolute";
this.RowResizer.style.cursor="n-resize";
if(_156!=null){
this.RowResizerRefTable=_156;
if(this.GridDataDiv){
this.RowResizer.style.left=RadGridNamespace.FindPosX(this.GridDataDiv)+"px";
var _157=(RadGridNamespace.FindPosX(this.GridDataDiv)+this.GridDataDiv.offsetWidth)-parseInt(this.RowResizer.style.left);
if(_157>_156.Control.offsetWidth){
this.RowResizer.style.width=_156.Control.offsetWidth+"px";
}else{
this.RowResizer.style.width=_157+"px";
}
if(parseInt(this.RowResizer.style.width)>this.GridDataDiv.offsetWidth){
this.RowResizer.style.width=this.GridDataDiv.offsetWidth+"px";
}
}else{
this.RowResizer.style.width=_156.Control.offsetWidth+"px";
this.RowResizer.style.left=RadGridNamespace.FindPosX(_153)+"px";
}
}
this.RowResizer.style.top=RadGridNamespace.GetEventPosY(e)-(RadGridNamespace.GetEventPosY(e)-e.clientY)+document.body.scrollTop+document.documentElement.scrollTop+"px";
var _158=document.body;
_158.appendChild(this.RowResizer);
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.DestroyRowResizerAndResizeRow=function(e,_15a){
try{
if((this.CellToResize!="undefined")&&(this.CellToResize!=null)&&(this.CellToResize.tagName.toLowerCase()=="td")&&(this.RowResizer!="undefined")&&(this.RowResizer!=null)){
var _15b;
if(this.GridDataDiv){
_15b=parseInt(this.RowResizer.style.top)+this.GridDataDiv.scrollTop-(RadGridNamespace.FindPosY(this.CellToResize));
}else{
_15b=parseInt(this.RowResizer.style.top)-(RadGridNamespace.FindPosY(this.CellToResize));
}
if(_15b>0){
var _15c=this.CellToResize.parentNode.parentNode.parentNode;
var _15d=this.GetTableObjectByID(_15c.id);
if(_15d!=null){
_15d.ResizeRow(this.CellToResize.parentNode.rowIndex,_15b);
}
}
}
if(_15a){
this.DestroyRowResizer();
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.DestroyRowResizer=function(){
try{
if((this.RowResizer!="undefined")&&(this.RowResizer!=null)&&(this.RowResizer.parentNode!=null)){
var _15e=this.RowResizer.parentNode;
_15e.removeChild(this.RowResizer);
this.RowResizer=null;
this.RowResizerRefTable=null;
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.MoveRowResizer=function(e){
try{
if((this.RowResizer!="undefined")&&(this.RowResizer!=null)&&(this.RowResizer.parentNode!=null)){
this.RowResizer.style.top=RadGridNamespace.GetEventPosY(e)-(RadGridNamespace.GetEventPosY(e)-e.clientY)+document.body.scrollTop+document.documentElement.scrollTop+"px";
if(this.ClientSettings.Resizing.EnableRealTimeResize){
this.DestroyRowResizerAndResizeRow(e,false);
this.UpdateRowResizerWidth(e);
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.UpdateRowResizerWidth=function(e){
var _161=RadGridNamespace.GetCurrentElement(e);
if((_161!=null)&&(_161.tagName.toLowerCase()=="td")){
var _162=this.RowResizerRefTable;
if(_162!=null){
if(this.GridDataDiv){
var _163=(RadGridNamespace.FindPosX(this.GridDataDiv)+this.GridDataDiv.offsetWidth)-parseInt(this.RowResizer.style.left);
if(_163>_162.Control.offsetWidth){
this.RowResizer.style.width=_162.Control.offsetWidth+"px";
}else{
this.RowResizer.style.width=_163+"px";
}
if(parseInt(this.RowResizer.style.width)>this.GridDataDiv.offsetWidth){
this.RowResizer.style.width=this.GridDataDiv.offsetWidth+"px";
}
}else{
this.RowResizer.style.width=_162.Control.offsetWidth+"px";
}
}
}
};
RadGridNamespace.RadGrid.prototype.SetHeaderAndFooterDivsWidth=function(){
if((document.compatMode=="BackCompat"&&navigator.userAgent.toLowerCase().indexOf("msie")!=-1)||(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1)){
if(this.ClientSettings.Scrolling.UseStaticHeaders){
if(this.GridHeaderDiv!=null&&this.GridDataDiv!=null&&this.GridHeaderDiv!=null){
this.GridHeaderDiv.style.width="100%";
if(this.GridHeaderDiv&&this.GridDataDiv){
if(this.GridDataDiv.offsetWidth>0){
this.GridHeaderDiv.style.width=this.GridDataDiv.offsetWidth-RadGridNamespace.GetScrollBarWidth()+"px";
}
}
if(this.GridHeaderDiv&&this.GridFooterDiv){
this.GridFooterDiv.style.width=this.GridHeaderDiv.style.width;
}
}
}
}
if(this.ClientSettings.Scrolling.AllowScroll&&this.ClientSettings.Scrolling.UseStaticHeaders){
var _164=RadGridNamespace.IsRightToLeft(this.GridHeaderDiv);
if((!_164&&this.GridHeaderDiv&&parseInt(this.GridHeaderDiv.style.marginRight)!=RadGridNamespace.GetScrollBarWidth())||(_164&&this.GridHeaderDiv&&parseInt(this.GridHeaderDiv.style.marginLeft)!=RadGridNamespace.GetScrollBarWidth())){
if(!_164){
this.GridHeaderDiv.style.marginRight=RadGridNamespace.GetScrollBarWidth()+"px";
this.GridHeaderDiv.style.marginLeft="";
}else{
this.GridHeaderDiv.style.marginLeft=RadGridNamespace.GetScrollBarWidth()+"px";
this.GridHeaderDiv.style.marginRight="";
}
}
if(this.GridHeaderDiv&&this.GridDataDiv){
if((this.GridDataDiv.clientWidth==this.GridDataDiv.offsetWidth)){
this.GridHeaderDiv.style.width="100%";
if(!_164){
this.GridHeaderDiv.style.marginRight="";
}else{
this.GridHeaderDiv.style.marginLeft="";
}
}
}
if(this.GroupPanelObject&&this.GroupPanelObject.Items.length>0&&navigator.userAgent.toLowerCase().indexOf("msie")!=-1){
if(this.MasterTableView&&this.MasterTableViewHeader){
this.MasterTableView.Control.style.width=this.MasterTableViewHeader.Control.offsetWidth+"px";
}
}
if(this.GridFooterDiv){
this.GridFooterDiv.style.marginRight=this.GridHeaderDiv.style.marginRight;
this.GridFooterDiv.style.marginLeft=this.GridHeaderDiv.style.marginLeft;
this.GridFooterDiv.style.width=this.GridHeaderDiv.style.width;
}
}
};
RadGridNamespace.RadGrid.prototype.SetDataDivHeight=function(){
if(this.GridDataDiv&&this.Control.style.height!=""){
this.GridDataDiv.style.height="10px";
var _165=0;
if(this.GroupPanelObject){
_165+=this.GroupPanelObject.Control.offsetHeight;
}
if(this.GridHeaderDiv){
_165+=this.GridHeaderDiv.offsetHeight;
}
if(this.GridFooterDiv){
_165+=this.GridFooterDiv.offsetHeight;
}
if(this.PagerControl){
_165+=this.PagerControl.offsetHeight;
}
if(this.TopPagerControl){
_165+=this.TopPagerControl.offsetHeight;
}
var _166=this.Control.clientHeight-_165;
if(_166>0){
var _167=this.Control.style.position;
if(window.netscape){
this.Control.style.position="absolute";
}
this.GridDataDiv.style.height=_166+"px";
if(window.netscape){
this.Control.style.position=_167;
}
}
}
};
RadGridNamespace.RadGrid.prototype.InitializeDimensions=function(){
try{
var _168=this;
this.InitializeAutoLayout();
this.ApplyFrozenScroll();
if(!this.EnableAJAX){
this.OnWindowResize();
}else{
var _169=function(){
_168.OnWindowResize();
};
if(window.netscape&&!window.opera){
_169();
}else{
setTimeout(_169,0);
}
}
this.Control.RadResize=function(){
_168.OnWindowResize();
};
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1){
setTimeout(function(){
_168.AttachDomEvent(window,"resize","OnWindowResize");
},0);
}else{
this.AttachDomEvent(window,"resize","OnWindowResize");
}
this.Control.RadShow=function(){
_168.OnWindowResize();
};
this.ClientSettings.Scrolling.FrozenColumnsCount+=this.MasterTableViewHeader.ExpandCollapseColumns.length+this.MasterTableViewHeader.GroupSplitterColumns.length;
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.OnWindowResize=function(e){
this.SetHeaderAndFooterDivsWidth();
this.SetDataDivHeight();
var _16b=this;
_16b.ApplyFrozenScroll();
};
RadGridNamespace.RadGrid.prototype.InitializeAutoLayout=function(){
if(this.ClientSettings.Scrolling.AllowScroll&&this.ClientSettings.Scrolling.UseStaticHeaders){
if(this.MasterTableView&&this.MasterTableViewHeader){
if(this.MasterTableView.TableLayout!="Auto"||window.netscape||window.opera){
return;
}
this.MasterTableView.Control.style.tableLayout=this.MasterTableViewHeader.Control.style.tableLayout="";
var _16c=this.MasterTableView.Control.tBodies[0].rows[this.ClientSettings.FirstDataRowClientRowIndex];
var _16d=this.MasterTableViewHeader.HeaderRow;
var _16e=Math.min(_16d.cells.length,_16c.cells.length);
for(var i=0;i<_16e;i++){
var col=this.MasterTableViewHeader.ColGroup.Cols[i];
if(!col){
continue;
}
if(col.width!=""){
continue;
}
var _171=_16d.cells[i].offsetWidth;
var _172=_16c.cells[i].offsetWidth;
var _173=(_171>_172)?_171:_172;
if(this.MasterTableViewFooter&&this.MasterTableViewFooter.Control){
if(this.MasterTableViewFooter.Control.tBodies[0].rows[0]&&this.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[i]){
if(this.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[i].offsetWidth>_173){
_173=this.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[i].offsetWidth;
}
}
}
if(_173<=0){
continue;
}
_16d.cells[i].style.width=_16c.cells[i].style.width=this.MasterTableView.ColGroup.Cols[i].width=col.width=_173;
if(this.MasterTableViewFooter&&this.MasterTableViewFooter.Control){
if(this.MasterTableViewFooter.Control.tBodies[0].rows[0]&&this.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[i]){
this.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[i].style.width=_173;
}
}
}
this.MasterTableView.Control.style.tableLayout=this.MasterTableViewHeader.Control.style.tableLayout="fixed";
if(this.MasterTableViewFooter&&this.MasterTableViewFooter.Control){
this.MasterTableViewFooter.Control.style.tableLayout="fixed";
}
if(window.netscape){
this.OnWindowResize();
}
}
}
};
RadGridNamespace.RadGrid.prototype.InitializeSaveScrollPosition=function(){
if(!this.ClientSettings.Scrolling.SaveScrollPosition){
return;
}
if(this.ClientSettings.Scrolling.ScrollTop!=""&&!this.ClientSettings.Scrolling.EnableAJAXScrollPaging){
this.GridDataDiv.scrollTop=this.ClientSettings.Scrolling.ScrollTop;
}
if(this.ClientSettings.Scrolling.ScrollLeft!=""){
var _174=document.getElementById(this.ClientID+"_Frozen");
if(this.GridHeaderDiv&&!_174){
this.GridHeaderDiv.scrollLeft=this.ClientSettings.Scrolling.ScrollLeft;
}
if(this.GridFooterDiv&&!_174){
this.GridFooterDiv.scrollLeft=this.ClientSettings.Scrolling.ScrollLeft;
}
if(_174){
_174.scrollLeft=this.ClientSettings.Scrolling.ScrollLeft;
}else{
this.GridDataDiv.scrollLeft=this.ClientSettings.Scrolling.ScrollLeft;
}
}
};
RadGridNamespace.RadGrid.prototype.InitializeAjaxScrollPaging=function(){
if(!this.ClientSettings.Scrolling.EnableAJAXScrollPaging){
return;
}
this.ScrollCounter=0;
this.CurrentAJAXScrollTop=0;
if(this.ClientSettings.Scrolling.AJAXScrollTop!=""){
this.CurrentAJAXScrollTop=this.ClientSettings.Scrolling.AJAXScrollTop;
}
var _175=this.CurrentPageIndex*this.MasterTableView.PageSize*20;
var _176=this.MasterTableView.PageCount*this.MasterTableView.PageSize*20;
var _177=this.MasterTableView.Control;
var _178=_177.offsetHeight;
if(!window.opera){
_177.style.marginTop=_175+"px";
_177.style.marginBottom=_176-_175-_178+"px";
}else{
_177.style.position="relative";
_177.style.top=_175+"px";
_177.style.marginBottom=_176-_178+"px";
}
this.CurrentAJAXScrollTop=_175;
this.GridDataDiv.scrollTop=_175;
this.CreateScrollerToolTip();
this.AttachDomEvent(this.GridDataDiv,"scroll","OnAJAXScroll");
};
RadGridNamespace.RadGrid.prototype.CreateScrollerToolTip=function(){
var _179=document.getElementById(this.ClientID+"ScrollerToolTip");
if(!_179){
this.ScrollerToolTip=document.createElement("span");
this.ScrollerToolTip.id=this.ClientID+"ScrollerToolTip";
this.ScrollerToolTip.style.backgroundColor="#F5F5DC";
this.ScrollerToolTip.style.border="1px solid";
this.ScrollerToolTip.style.position="absolute";
this.ScrollerToolTip.style.display="none";
this.ScrollerToolTip.style.font="icon";
this.ScrollerToolTip.style.padding="2";
document.body.appendChild(this.ScrollerToolTip);
}
};
RadGridNamespace.RadGrid.prototype.HideScrollerToolTip=function(){
var _17a=this;
setTimeout(function(){
var _17b=document.getElementById(_17a.ClientID+"ScrollerToolTip");
if(_17b&&_17b.parentNode){
_17b.style.display="none";
}
},200);
};
RadGridNamespace.RadGrid.prototype.ShowScrollerTooltip=function(_17c,_17d){
var _17e=document.getElementById(this.ClientID+"ScrollerToolTip");
if(_17e){
_17e.style.display="";
_17e.style.top=parseInt(RadGridNamespace.FindPosY(this.GridDataDiv))+Math.round(this.GridDataDiv.offsetHeight*_17c)+"px";
_17e.style.left=parseInt(RadGridNamespace.FindPosX(this.GridDataDiv))+this.GridDataDiv.offsetWidth-(this.GridDataDiv.offsetWidth-this.GridDataDiv.clientWidth)-_17e.offsetWidth+"px";
this.ApplyPagerTooltipText(_17e,_17d,this.MasterTableView.PageCount);
}
};
RadGridNamespace.RadGrid.prototype.ApplyPagerTooltipText=function(_17f,_180,_181){
var _182=this.ClientSettings.ClientMessages.PagerTooltipFormatString;
var _183=/\{0[^\}]*\}/g;
var _184=/\{1[^\}]*\}/g;
var _185=((_180==0)?1:_180+1);
var _186=_181;
_182=_182.replace(_183,_185).replace(_184,_186);
_17f.innerHTML=_182;
};
RadGridNamespace.RadGrid.prototype.InitializeScroll=function(){
var _187=this;
var grid=this;
var _189=function(){
grid.InitializeSaveScrollPosition();
};
if(window.netscape&&!window.opera){
window.setTimeout(_189,0);
}else{
_189();
}
this.InitializeAjaxScrollPaging();
this.AttachDomEvent(this.GridDataDiv,"scroll","OnGridScroll");
if(this.GridHeaderDiv){
this.AttachDomEvent(this.GridHeaderDiv,"scroll","OnGridScroll");
}
};
RadGridNamespace.RadGrid.prototype.ApplyFrozenScroll=function(){
this.isFrozenScroll=false;
if(this.MasterTableView.FrozenColumnsCount==0){
return;
}
var _18a=document.getElementById(this.ClientID+"_Frozen");
if(_18a){
var _18b=document.getElementById(this.ClientID+"_FrozenScroll");
this.AttachDomEvent(_18a,"scroll","OnGridFrozenScroll");
if(this.MasterTableView.Control.offsetWidth>this.GridDataDiv.clientWidth){
if(!window.netscape&&navigator.userAgent.indexOf("Safari")==-1){
_18a.style.height="100%";
}else{
_18a.style.height="16px";
}
_18b.style.width=this.GridDataDiv.scrollWidth+"px";
_18b.style.height="16px";
if(this.ClientSettings.Scrolling.SaveScrollPosition&&this.ClientSettings.Scrolling.ScrollLeft!=""){
_18a.scrollLeft=this.ClientSettings.Scrolling.ScrollLeft;
}
if(this.GridDataDiv.style.overflowX!=null){
this.GridDataDiv.style.overflowX="hidden";
}else{
_18a.style.marginTop="-16px";
_18a.style.zIndex=99999;
_18a.style.position="relative";
}
if(window.netscape&&!window.opera){
var _18c=RadGridNamespace.GetScrollBarWidth();
_18a.style.width=this.GridDataDiv.offsetWidth-_18c+"px";
}
this.isFrozenScroll=true;
}else{
_18a.style.height="";
_18b.style.width="";
this.GridDataDiv.style.overflow="auto";
this.isFrozenScroll=false;
}
}
};
RadGridNamespace.RadGrid.prototype.OnGridFrozenScroll=function(e){
var _18e=(e.srcElement)?e.srcElement:e.target;
if(this.ClientSettings.Scrolling.FrozenColumnsCount>this.MasterTableViewHeader.Columns.length){
this.isFrozenScroll=false;
}
if(this.isFrozenScroll){
var _18f=this.MasterTableViewHeader.Columns[this.ClientSettings.Scrolling.FrozenColumnsCount-1].Control;
var _190=RadGridNamespace.FindPosX(_18f)-RadGridNamespace.FindScrollPosX(_18f)+document.documentElement.scrollLeft+document.body.scrollLeft+_18f.offsetWidth;
var _191=_18e.scrollWidth-_190;
this.notFrozenColumns=[];
for(var i=this.ClientSettings.Scrolling.FrozenColumnsCount;i<this.MasterTableViewHeader.Columns.length;i++){
var _193=this.MasterTableViewHeader.Columns[i];
var _194=false;
if(window.netscape&&_193.Control.style.display=="none"){
_193.Control.style.display="table-cell";
_194=true;
}
this.notFrozenColumns[this.notFrozenColumns.length]={Index:i,Width:_193.Control.offsetWidth};
if(window.netscape&&_194){
_193.Control.style.display="none";
_194=false;
}
}
var _195=RadGridNamespace.GetScrollBarWidth();
if(window.netscape&&!window.opera){
_195=0;
}
var _196=Math.floor(_18e.scrollLeft/(_18e.scrollWidth-_18e.offsetWidth+_195)*100);
var _197=0;
var i=0;
while(i<this.notFrozenColumns.length-1){
var _193=this.notFrozenColumns[i];
var _198=Math.floor(_193.Width/_191*100);
if(_198+_197<_196){
this.MasterTableViewHeader.HideNotFrozenColumn(_193.Index);
_197+=_198;
}else{
this.MasterTableViewHeader.ShowNotFrozenColumn(_193.Index);
}
i++;
}
this.MasterTableView.Control.style.width=this.MasterTableViewHeader.Control.offsetWidth+"px";
if(this.MasterTableViewFooter){
this.MasterTableViewFooter.Control.style.width=this.MasterTableViewHeader.Control.offsetWidth+"px";
}
this.SavePostData("ScrolledControl",this.ClientID,this.GridDataDiv.scrollTop,_18e.scrollLeft);
}else{
}
};
RadGridNamespace.RadGrid.prototype.OnGridScroll=function(e){
var _19a=(e.srcElement)?e.srcElement:e.target;
if(window.opera&&this.isFrozenScroll){
this.GridDataDiv.scrollLeft=this.GridHeaderDiv.scrollLeft=0;
return;
}
if(this.ClientSettings.Scrolling.UseStaticHeaders){
if(!this.isFrozenScroll){
if(this.GridHeaderDiv){
if(_19a==this.GridHeaderDiv){
this.GridDataDiv.scrollLeft=this.GridHeaderDiv.scrollLeft;
}
if(_19a==this.GridDataDiv){
this.GridHeaderDiv.scrollLeft=this.GridDataDiv.scrollLeft;
}
}
if(this.GridFooterDiv){
this.GridFooterDiv.scrollLeft=this.GridDataDiv.scrollLeft;
}
}else{
if(this.GridHeaderDiv){
this.GridHeaderDiv.scrollLeft=this.GridDataDiv.scrollLeft;
}
if(this.GridFooterDiv){
this.GridFooterDiv.scrollLeft=this.GridDataDiv.scrollLeft;
}
}
}
this.SavePostData("ScrolledControl",this.ClientID,this.GridDataDiv.scrollTop,this.GridDataDiv.scrollLeft);
var evt={};
evt.ScrollTop=this.GridDataDiv.scrollTop;
evt.ScrollLeft=this.GridDataDiv.scrollLeft;
evt.ScrollControl=this.GridDataDiv;
evt.IsOnTop=(this.GridDataDiv.scrollTop==0)?true:false;
evt.IsOnBottom=((this.GridDataDiv.scrollHeight-this.GridDataDiv.offsetHeight+16)==this.GridDataDiv.scrollTop)?true:false;
RadGridNamespace.FireEvent(this,"OnScroll",[evt]);
};
RadGridNamespace.RadGrid.prototype.OnAJAXScroll=function(e){
if(this.GridDataDiv){
this.CurrentScrollTop=this.GridDataDiv.scrollTop;
}
this.ScrollCounter++;
var _19d=this;
RadGridNamespace.AJAXScrollHanlder=function(_19e){
if(_19d.ScrollCounter!=_19e){
return;
}
if(_19d.CurrentAJAXScrollTop!=_19d.GridDataDiv.scrollTop){
if(_19d.CurrentPageIndex==_19f){
return;
}
var _1a0=_19d.ClientID;
var _1a1=_19d.MasterTableView.ClientID;
_19d.SavePostData("AJAXScrolledControl",_19d.GridDataDiv.scrollLeft,_19d.LastScrollTop,_19d.GridDataDiv.scrollTop,_19f);
var _1a2=_19d.ClientSettings.PostBackFunction;
_1a2=_1a2.replace("{0}",_19d.UniqueID);
eval(_1a2);
}
_19d.ScrollCounter=0;
_19d.HideScrollerToolTip();
};
var evt={};
evt.ScrollTop=this.GridDataDiv.scrollTop;
evt.ScrollLeft=this.GridDataDiv.scrollLeft;
evt.ScrollControl=this.GridDataDiv;
evt.IsOnTop=(this.GridDataDiv.scrollTop==0)?true:false;
evt.IsOnBottom=((this.GridDataDiv.scrollHeight-this.GridDataDiv.offsetHeight+16)==this.GridDataDiv.scrollTop)?true:false;
RadGridNamespace.FireEvent(this,"OnScroll",[evt]);
var _1a4=this.GridDataDiv.scrollTop/(this.GridDataDiv.scrollHeight-this.GridDataDiv.offsetHeight+16);
var _19f=Math.round((this.MasterTableView.PageCount-1)*_1a4);
setTimeout("RadGridNamespace.AJAXScrollHanlder("+this.ScrollCounter+")",500);
this.ShowScrollerTooltip(_1a4,_19f);
};
RadGridNamespace.RadGridTable=function(_1a5){
if((!_1a5)||typeof (_1a5)!="object"){
return;
}
for(var _1a6 in _1a5){
this[_1a6]=_1a5[_1a6];
}
this.Type="RadGridTable";
this.ServerID=this.ID;
this.SelectedRows=new Array();
this.SelectedCells=new Array();
this.SelectedColumns=new Array();
this.ExpandCollapseColumns=new Array();
this.GroupSplitterColumns=new Array();
this.HeaderRow=null;
};
RadGridNamespace.RadGridTable.prototype._constructor=function(_1a7){
if((!_1a7)||typeof (_1a7)!="object"){
return;
}
this.Control=document.getElementById(this.ClientID);
if(!this.Control){
return;
}
this.ColGroup=RadGridNamespace.GetTableColGroup(this.Control);
if(!this.ColGroup){
return;
}
this.ColGroup.Cols=RadGridNamespace.GetTableColGroupCols(this.ColGroup);
this.Owner=_1a7;
if(this.Owner.ClientSettings){
this.InitializeEvents(this.Owner.ClientSettings.ClientEvents);
this.Control.style.overflow=((this.Owner.ClientSettings.Resizing.ClipCellContentOnResize&&((this.Owner.ClientSettings.Resizing.AllowColumnResize)||(this.Owner.ClientSettings.Resizing.AllowRowResize)))||(this.Owner.ClientSettings.Scrolling.AllowScroll&&this.Owner.ClientSettings.Scrolling.UseStaticHeaders))?"hidden":"";
}
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&this.Control.style.tableLayout=="fixed"&&this.Control.style.width.indexOf("%")!=-1){
this.Control.style.width="";
}
this.CreateStyles();
if(this.Owner.ClientSettings&&this.Owner.ClientSettings.Scrolling.AllowScroll&&this.Owner.ClientSettings.Scrolling.UseStaticHeaders){
if(this.ClientID.indexOf("_Header")!=-1||this.ClientID.indexOf("_Detail")!=-1){
this.Columns=this.GetTableColumns(this.Control,this.RenderColumns);
}else{
this.Columns=this.Owner.MasterTableViewHeader.Columns;
this.ExpandCollapseColumns=this.Owner.MasterTableViewHeader.ExpandCollapseColumns;
this.GroupSplitterColumns=this.Owner.MasterTableViewHeader.GroupSplitterColumns;
}
}else{
this.Columns=this.GetTableColumns(this.Control,this.RenderColumns);
}
if(this.Owner.ClientSettings&&this.Owner.ClientSettings.ShouldCreateRows){
this.InitializeRows(this.Controls[0].Rows);
}
};
RadGridNamespace.RadGridTable.prototype.Dispose=function(){
if(this.ColGroup&&this.ColGroup.Cols){
this.ColGroup.Cols=null;
this.ColGroup=null;
}
this.Owner=null;
this.DisposeEvents();
this.ExpandCollapseColumns=null;
this.GroupSplitterColumns=null;
this.DisposeRows();
this.DisposeColumns();
this.RenderColumns=null;
this.SelectedRows=null;
this.ExpandCollapseColumns=null;
this.DetailTables=null;
this.DetailTablesCollection=null;
this.Control=null;
this.HeaderRow=null;
};
RadGridNamespace.RadGridTable.prototype.CreateStyles=function(){
if(!this.SelectedItemStyleClass||this.SelectedItemStyleClass==""){
if(this.SelectedItemStyle&&this.SelectedItemStyle!=""){
RadGridNamespace.AddRule(this.Owner.GridStyleSheet,".SelectedItemStyle"+this.ClientID+"1 td",this.SelectedItemStyle);
}else{
RadGridNamespace.AddRule(this.Owner.GridStyleSheet,".SelectedItemStyle"+this.ClientID+"2 td","background-color:Navy;color:White;");
}
}
RadGridNamespace.addClassName(this.Control,"grid"+this.ClientID);
if(window.netscape&&!window.opera){
RadGridNamespace.AddRule(this.Owner.GridStyleSheet,".grid"+this.ClientID+" td","overflow: hidden;-moz-user-select:-moz-none;");
RadGridNamespace.AddRule(this.Owner.GridStyleSheet,".grid"+this.ClientID+" th","overflow: hidden;-moz-user-select:-moz-none;");
}else{
if(window.opera||navigator.userAgent.indexOf("Safari")!=-1){
var _1a8=this;
setTimeout(function(){
RadGridNamespace.AddRule(_1a8.Owner.GridStyleSheet,".grid"+_1a8.ClientID+" td","overflow: hidden;");
RadGridNamespace.AddRule(_1a8.Owner.GridStyleSheet,".grid"+_1a8.ClientID+" th","overflow: hidden;");
},100);
}else{
RadGridNamespace.AddRule(this.Owner.GridStyleSheet,".grid"+this.ClientID+" td","overflow: hidden; text-overflow: ellipsis;");
RadGridNamespace.AddRule(this.Owner.GridStyleSheet,".grid"+this.ClientID+" th","overflow: hidden; text-overflow: ellipsis;");
}
}
};
RadGridNamespace.RadGridTable.prototype.InitializeEvents=function(_1a9){
for(clientEvent in _1a9){
if(typeof (_1a9[clientEvent])!="string"){
continue;
}
if(!this.Owner.IsClientEventName(clientEvent)){
if(_1a9[clientEvent]!=""){
var _1aa=_1a9[clientEvent];
if(_1aa.indexOf("(")!=-1){
this[clientEvent]=_1aa;
}else{
this[clientEvent]=eval(_1aa);
}
}else{
this[clientEvent]=null;
}
}
}
};
RadGridNamespace.RadGridTable.prototype.DisposeEvents=function(){
for(var _1ab in RadGridNamespace.RadGridTable.ClientEventNames){
this[_1ab]=null;
}
};
RadGridNamespace.RadGridTable.prototype.InitializeRows=function(rows){
if(this.ClientID.indexOf("_Header")!=-1||this.ClientID.indexOf("_Footer")!=-1){
return;
}
try{
var _1ad=[];
for(var i=0;i<rows.length;i++){
if(!rows[i].Visible||rows[i].ClientRowIndex<0){
continue;
}
if(rows[i].ItemType=="THead"||rows[i].ItemType=="TFoot"){
continue;
}
RadGridNamespace.FireEvent(this,"OnRowCreating");
rows[i]._constructor(this);
_1ad[_1ad.length]=rows[i];
RadGridNamespace.FireEvent(this,"OnRowCreated",[rows[i]]);
}
this.Rows=_1ad;
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.DisposeRows=function(){
if(this.Rows!=null){
for(var i=0;i<this.Rows.length;i++){
var row=this.Rows[i];
row.Dispose();
}
this.Rows=null;
}
};
RadGridNamespace.RadGridTable.prototype.DisposeColumns=function(){
if(this.Columns!=null){
for(var i=0;i<this.Columns.length;i++){
var _1b2=this.Columns[i];
_1b2.Dispose();
}
this.Columns=null;
}
};
RadGridNamespace.RadGridTable.prototype.GetTableRows=function(_1b3,_1b4){
if(this.ClientID.indexOf("_Header")!=-1||this.ClientID.indexOf("_Footer")!=-1){
return;
}
try{
var _1b5=this;
var _1b6=new Array();
var j=0;
for(var i=0;i<_1b4.length;i++){
if((_1b4[i].ItemType=="THead")||(_1b4[i].ItemType=="TFoot")){
continue;
}
if((_1b4[i])&&(_1b4[i].Visible)){
RadGridNamespace.FireEvent(this,"OnRowCreating");
setTimeout(function(){
_1b6[_1b6.length]=_1b4[i]._constructor(_1b5);
},0);
RadGridNamespace.FireEvent(this,"OnRowCreated",[_1b6[j]]);
j++;
}
}
return _1b6;
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.GetTableHeaderRow=function(){
try{
if(this.Control.tHead){
for(var i=0;i<this.Control.tHead.rows.length;i++){
if(this.Control.tHead.rows[i]!=null){
if(this.Control.tHead.rows[i].cells[0]!=null){
if(this.Control.tHead.rows[i].cells[0].tagName!=null){
if(this.Control.tHead.rows[i].cells[0].tagName.toLowerCase()=="th"){
this.HeaderRow=this.Control.tHead.rows[i];
break;
}
}
}
}
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.GetTableColumns=function(_1ba,_1bb){
try{
this.GetTableHeaderRow();
var _1bc=new Array();
if(!this.HeaderRow){
return;
}
if(!this.HeaderRow.cells[0]){
return;
}
var j=0;
for(var i=0;i<_1bb.length;i++){
if(_1bb[i].Visible){
RadGridNamespace.FireEvent(this,"OnColumnCreating");
_1bc[_1bc.length]=new RadGridNamespace.RadGridTableColumn(_1bb[i]);
_1bc[j]._constructor(this.HeaderRow.cells[j],this);
_1bc[j].RealIndex=i;
if(_1bb[i].ColumnType=="GridExpandColumn"){
this.ExpandCollapseColumns[this.ExpandCollapseColumns.length]=_1bc[j];
}
if(_1bb[i].ColumnType=="GridGroupSplitterColumn"){
this.GroupSplitterColumns[this.GroupSplitterColumns.length]=_1bc[j];
}
if(_1bb[i].ColumnType=="GridRowIndicatorColumn"){
if(this.ClientID.indexOf("_Header")!=-1){
this.Owner.ClientSettings.Scrolling.FrozenColumnsCount++;
}
}
RadGridNamespace.FireEvent(this,"OnColumnCreated",[_1bc[j]]);
j++;
}
}
return _1bc;
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.RemoveTableLayOut=function(){
this.masterTableLayOut=this.Owner.MasterTableView.Control.style.tableLayout;
this.detailTablesTableLayOut=new Array();
for(var i=0;i<this.Owner.DetailTablesCollection.length;i++){
this.detailTablesTableLayOut[this.detailTablesTableLayOut.length]=this.Owner.DetailTablesCollection[i].Control.style.tableLayout;
this.Owner.DetailTablesCollection[i].Control.style.tableLayout="";
}
};
RadGridNamespace.RadGridTable.prototype.RestoreTableLayOut=function(){
this.Owner.MasterTableView.Control.style.tableLayout=this.masterTableLayOut;
for(var i=0;i<this.Owner.DetailTablesCollection.length;i++){
this.Owner.DetailTablesCollection[i].Control.style.tableLayout=this.detailTablesTableLayOut[i];
}
};
RadGridNamespace.RadGridTable.prototype.SelectRow=function(row,_1c2){
try{
if(!this.Owner.ClientSettings.Selecting.AllowRowSelect){
return;
}
var _1c3=this.Owner.GetRowObjectByRealRow(this,row);
if(_1c3!=null){
if(_1c3.ItemType=="Item"||_1c3.ItemType=="AlternatingItem"){
_1c3.SetSelected(_1c2);
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.DeselectRow=function(row){
try{
if(!this.Owner.ClientSettings.Selecting.AllowRowSelect){
return;
}
var _1c5=this.Owner.GetRowObjectByRealRow(this,row);
if(_1c5!=null){
if(_1c5.ItemType=="Item"||_1c5.ItemType=="AlternatingItem"){
this.RemoveFromSelectedRows(_1c5);
_1c5.RemoveSelectedRowStyle();
_1c5.Selected=false;
_1c5.CheckClientSelectColumns();
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.ResizeRow=function(_1c6,_1c7,_1c8){
try{
if(!this.Owner.ClientSettings.Resizing.AllowRowResize){
return;
}
if(!RadGridNamespace.FireEvent(this,"OnRowResizing",[_1c6,_1c7])){
return;
}
this.RemoveTableLayOut();
var _1c9=this.Control.style.tableLayout;
this.Control.style.tableLayout="";
var _1ca=this.Control.parentNode.parentNode.parentNode.parentNode;
var _1cb=this.Owner.GetTableObjectByID(_1ca.id);
var _1cc;
if(_1cb!=null){
_1cc=_1cb.Control.style.tableLayout;
_1cb.Control.style.tableLayout="";
}
if(!_1c8){
if(this.Control){
if(this.Control.rows[_1c6]){
if(this.Control.rows[_1c6].cells[0]){
this.Control.rows[_1c6].cells[0].style.height=_1c7+"px";
this.Control.rows[_1c6].style.height=_1c7+"px";
}
}
}
}else{
if(this.Control){
if(this.Control.tBodies[0]){
if(this.Control.tBodies[0].rows[_1c6]){
if(this.Control.tBodies[0].rows[_1c6].cells[0]){
this.Control.tBodies[0].rows[_1c6].cells[0].style.height=_1c7+"px";
this.Control.tBodies[0].rows[_1c6].style.height=_1c7+"px";
}
}
}
}
}
this.Control.style.tableLayout=_1c9;
if(_1cb!=null){
_1cb.Control.style.tableLayout=_1cc;
}
this.RestoreTableLayOut();
var _1cd=this.Owner.GetRowObjectByRealRow(this,this.Control.rows[_1c6]);
this.Owner.SavePostData("ResizedRows",this.Control.id,_1cd.RealIndex,_1c7+"px");
RadGridNamespace.FireEvent(this,"OnRowResized",[_1c6,_1c7]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.ResizeColumn=function(_1ce,_1cf){
if(isNaN(parseInt(_1ce))){
var _1d0="Column index must be of type \"Number\"!";
alert(_1d0);
return;
}
if(isNaN(parseInt(_1cf))){
var _1d0="Column width must be of type \"Number\"!";
alert(_1d0);
return;
}
if(_1ce<0){
var _1d0="Column index must be non-negative!";
alert(_1d0);
return;
}
if(_1cf<0){
var _1d0="Column width must be non-negative!";
alert(_1d0);
return;
}
if(_1ce>(this.Columns.length-1)){
var _1d0="Column index must be less than columns count!";
alert(_1d0);
return;
}
if(!this.Owner.ClientSettings.Resizing.AllowColumnResize){
return;
}
if(!this.Columns){
return;
}
if(!this.Columns[_1ce].Resizable){
return;
}
if(!RadGridNamespace.FireEvent(this,"OnColumnResizing",[_1ce,_1cf])){
return;
}
try{
if(this==this.Owner.MasterTableView&&this.Owner.MasterTableViewHeader){
this.Owner.MasterTableViewHeader.ResizeColumn(_1ce,_1cf);
}
var _1d1=this.Control.clientWidth;
var _1d2=this.Owner.Control.clientWidth;
if(this.HeaderRow){
var _1d3=this.HeaderRow.cells[_1ce].scrollWidth-_1cf;
}
if(window.netscape||window.opera){
if(this.HeaderRow){
if(this.HeaderRow.cells[_1ce]){
this.HeaderRow.cells[_1ce].style.width=_1cf+"px";
}
}
if(this==this.Owner.MasterTableViewHeader){
var _1d4=this.Owner.MasterTableView.Control.tBodies[0].rows[this.Owner.ClientSettings.FirstDataRowClientRowIndex];
if(_1d4){
if(_1d4.cells[_1ce]){
_1d4.cells[_1ce].style.width=_1cf+"px";
}
}
if(this.Owner.MasterTableViewFooter&&this.Owner.MasterTableViewFooter.Control){
if(this.Owner.MasterTableViewFooter.Control.tBodies[0].rows[0]&&this.Owner.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[_1ce]){
if(_1cf>0){
this.Owner.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[_1ce].style.width=_1cf+"px";
}
}
}
}
}
if(this.ColGroup){
if(this.ColGroup.Cols[_1ce]){
if(_1cf>0){
this.ColGroup.Cols[_1ce].width=_1cf+"px";
}
}
}
if(this==this.Owner.MasterTableViewHeader){
if(this.Owner.MasterTableView.ColGroup){
if(this.Owner.MasterTableView.ColGroup.Cols[_1ce]){
if(_1cf>0){
this.Owner.MasterTableView.ColGroup.Cols[_1ce].width=_1cf+"px";
}
}
}
if(this.Owner.MasterTableViewFooter&&this.Owner.MasterTableViewFooter.ColGroup){
if(this.Owner.MasterTableViewFooter.ColGroup.Cols[_1ce]){
if(_1cf>0){
this.Owner.MasterTableViewFooter.ColGroup.Cols[_1ce].width=_1cf+"px";
}
}
}
}
if(_1cf.toString().indexOf("px")!=-1){
_1cf=_1cf.replace("px","");
}
if(this==this.Owner.MasterTableView||this==this.Owner.MasterTableViewHeader){
if(_1cf.toString().indexOf("%")!=-1){
this.Owner.SavePostData("ResizedColumns",this.Owner.MasterTableView.ClientID,this.Columns[_1ce].RealIndex,_1cf);
}else{
this.Owner.SavePostData("ResizedColumns",this.Owner.MasterTableView.ClientID,this.Columns[_1ce].RealIndex,_1cf+"px");
}
}else{
if(_1cf.toString().indexOf("%")!=-1){
this.Owner.SavePostData("ResizedColumns",this.ClientID,this.Columns[_1ce].RealIndex,_1cf);
}else{
this.Owner.SavePostData("ResizedColumns",this.ClientID,this.Columns[_1ce].RealIndex,_1cf+"px");
}
}
if(this.Owner.MasterTableViewHeader){
this.Owner.ClientSettings.Resizing.ResizeGridOnColumnResize=true;
}
if(this.Owner.ClientSettings.Resizing.ResizeGridOnColumnResize){
if(this==this.Owner.MasterTableViewHeader){
for(var i=0;i<this.ColGroup.Cols.length;i++){
if(i!=_1ce&&this.ColGroup.Cols[i].width==""){
this.ColGroup.Cols[i].width=this.HeaderRow.cells[i].scrollWidth+"px";
this.Owner.MasterTableView.ColGroup.Cols[i].width=this.ColGroup.Cols[i].width;
if(this.Owner.MasterTableViewFooter&&this.Owner.MasterTableViewFooter.ColGroup){
this.Owner.MasterTableViewFooter.ColGroup.Cols[i].width=this.ColGroup.Cols[i].width;
}
}
}
this.Control.style.width=(this.Control.offsetWidth-_1d3)+"px";
this.Owner.MasterTableView.Control.style.width=this.Control.style.width;
if(this.Owner.MasterTableViewFooter&&this.Owner.MasterTableViewFooter.Control){
this.Owner.MasterTableViewFooter.Control.style.width=this.Control.style.width;
}
var _1d6=(this.Control.scrollWidth>this.Control.offsetWidth)?this.Control.scrollWidth:this.Control.offsetWidth;
var _1d7=this.Owner.GridDataDiv.offsetWidth;
this.Owner.SavePostData("ResizedControl",this.ClientID,_1d6+"px",_1d7+"px",this.Owner.Control.offsetHeight+"px");
}else{
if(window.netscape||window.opera){
this.Control.style.width=(this.Control.offsetWidth-_1d3)+"px";
this.Owner.Control.style.width=this.Control.style.width;
}
var _1d6=(this.Control.scrollWidth>this.Control.offsetWidth)?this.Control.scrollWidth:this.Control.offsetWidth;
this.Owner.SavePostData("ResizedControl",this.ClientID,_1d6+"px",this.Owner.Control.offsetWidth+"px",this.Owner.Control.offsetHeight+"px");
}
}else{
var _1d8=(this.Control.offsetWidth-_1d2)/this.ColGroup.Cols.length;
var _1d9="";
for(var i=_1ce+1;i<this.ColGroup.Cols.length;i++){
var _1da=0;
if(this.ColGroup.Cols[i].width!=""){
_1da=parseInt(this.ColGroup.Cols[i].width)-_1d8;
}
if(this.HeaderRow){
_1da=this.HeaderRow.cells[i].scrollWidth-_1d8;
}
this.ColGroup.Cols[i].width="";
if(this==this.Owner.MasterTableViewHeader){
this.Owner.MasterTableView.ColGroup.Cols[i].width="";
}
if(this.Owner.MasterTableViewFooter){
this.Owner.MasterTableViewFooter.ColGroup.Cols[i].width="";
}
}
if(_1d2>0){
this.Owner.Control.style.width=_1d2+"px";
}
this.Control.style.width=_1d1+"px";
if(this==this.Owner.MasterTableViewHeader){
this.Owner.MasterTableView.Control.style.width=this.Control.style.width;
}
if(this.Owner.MasterTableViewFooter){
this.Owner.MasterTableViewFooter.Control.style.width=this.Control.style.width;
}
}
if(this.Owner.GroupPanelObject&&this.Owner.GroupPanelObject.Items.length>0&&navigator.userAgent.toLowerCase().indexOf("msie")!=-1){
if(this.Owner.MasterTableView&&this.Owner.MasterTableViewHeader){
this.Owner.MasterTableView.Control.style.width=this.Owner.MasterTableViewHeader.Control.offsetWidth+"px";
}
}
RadGridNamespace.FireEvent(this,"OnColumnResized",[_1ce,_1cf]);
if(window.netscape){
this.Control.style.cssText=this.Control.style.cssText;
}
this.Owner.ApplyFrozenScroll();
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.ReorderColumns=function(_1db,_1dc){
if(isNaN(parseInt(_1db))){
var _1dd="First column index must be of type \"Number\"!";
alert(_1dd);
return;
}
if(isNaN(parseInt(_1dc))){
var _1dd="Second column index must be of type \"Number\"!";
alert(_1dd);
return;
}
if(_1db<0){
var _1dd="First column index must be non-negative!";
alert(_1dd);
return;
}
if(_1dc<0){
var _1dd="Second column index must be non-negative!";
alert(_1dd);
return;
}
if(_1db>(this.Columns.length-1)){
var _1dd="First column index must be less than columns count!";
alert(_1dd);
return;
}
if(_1dc>(this.Columns.length-1)){
var _1dd="Second column index must be less than columns count!";
alert(_1dd);
return;
}
if(!this.Owner.ClientSettings.AllowColumnsReorder){
return;
}
if(!this.Columns){
return;
}
if(!this.Columns[_1db].Reorderable){
return;
}
if(!this.Columns[_1dc].Reorderable){
return;
}
this.SwapColumns(_1db,_1dc);
if((!this.Owner.ClientSettings.ReorderColumnsOnClient)&&(this.Owner.ClientSettings.PostBackReferences.PostBackColumnsReorder!="")){
if(this==this.Owner.MasterTableView){
eval(this.Owner.ClientSettings.PostBackReferences.PostBackColumnsReorder);
}
}
};
RadGridNamespace.RadGridTable.prototype.SwapColumns=function(_1de,_1df,_1e0){
if(isNaN(parseInt(_1de))){
var _1e1="First column index must be of type \"Number\"!";
alert(_1e1);
return;
}
if(isNaN(parseInt(_1df))){
var _1e1="Second column index must be of type \"Number\"!";
alert(_1e1);
return;
}
if(_1de<0){
var _1e1="First column index must be non-negative!";
alert(_1e1);
return;
}
if(_1df<0){
var _1e1="Second column index must be non-negative!";
alert(_1e1);
return;
}
if(_1de>(this.Columns.length-1)){
var _1e1="First column index must be less than columns count!";
alert(_1e1);
return;
}
if(_1df>(this.Columns.length-1)){
var _1e1="Second column index must be less than columns count!";
alert(_1e1);
return;
}
if(!this.Owner.ClientSettings.AllowColumnsReorder){
return;
}
if(!this.Columns){
return;
}
if(!this.Columns[_1de].Reorderable){
return;
}
if(!this.Columns[_1df].Reorderable){
return;
}
try{
if(this==this.Owner.MasterTableView&&this.Owner.MasterTableViewHeader){
this.Owner.MasterTableViewHeader.SwapColumns(_1de,_1df,!this.Owner.ClientSettings.ReorderColumnsOnClient);
return;
}
if(typeof (_1e0)=="undefined"){
_1e0=true;
}
if(this.Owner.ClientSettings.ColumnsReorderMethod=="Reorder"){
if(_1df>_1de){
while(_1de+1<_1df){
this.SwapColumns(_1df-1,_1df,false);
_1df--;
}
}else{
while(_1df<_1de-1){
this.SwapColumns(_1df+1,_1df,false);
_1df++;
}
}
}
if(!RadGridNamespace.FireEvent(this,"OnColumnSwapping",[_1de,_1df])){
return;
}
var _1e2=this.Control;
var _1e3=this.Columns[_1de];
var _1e4=this.Columns[_1df];
this.Columns[_1de]=_1e4;
this.Columns[_1df]=_1e3;
var _1e5=this.ColGroup.Cols[_1de].width;
if(_1e5==""&&this.HeaderRow){
_1e5=this.HeaderRow.cells[_1de].offsetWidth;
}
var _1e6=this.ColGroup.Cols[_1df].width;
if(_1e6==""&&this.HeaderRow){
_1e6=this.HeaderRow.cells[_1df].offsetWidth;
}
var _1e7=this.Owner.ClientSettings.Resizing.AllowColumnResize;
var _1e8=(typeof (this.Columns[_1de].Resizable)=="boolean")?this.Columns[_1de].Resizable:false;
var _1e9=(typeof (this.Columns[_1df].Resizable)=="boolean")?this.Columns[_1df].Resizable:false;
this.Owner.ClientSettings.Resizing.AllowColumnResize=true;
this.Columns[_1de].Resizable=true;
this.Columns[_1df].Resizable=true;
this.ResizeColumn(_1de,_1e6);
this.ResizeColumn(_1df,_1e5);
this.Owner.ClientSettings.Resizing.AllowColumnResize=_1e7;
this.Columns[_1de].Resizable=_1e8;
this.Columns[_1df].Resizable=_1e9;
if(navigator.userAgent.indexOf("Safari")!=-1){
var _1ea=this.Columns[_1de].Control;
var _1eb=this.Columns[_1df].Control;
this.Columns[_1de].Control=_1eb;
this.Columns[_1df].Control=_1ea;
}
var _1ec=(this==this.Owner.MasterTableViewHeader)?this.Owner.MasterTableView.ClientID:this.ClientID;
this.Owner.SavePostData("ReorderedColumns",_1ec,this.Columns[_1de].UniqueName,this.Columns[_1df].UniqueName);
for(var i=0;i<_1e2.rows.length;i++){
if(_1e2.rows[i]!=null){
if((_1e2.rows[i].cells[_1de]!=null)&&(_1e2.rows[i].cells[_1df]!=null)){
if(_1e2.rows[i].cells[_1de].innerHTML!=null){
var _1ee=_1e2.rows[i].cells[_1de].innerHTML;
var _1ef=_1e2.rows[i].cells[_1df].innerHTML;
_1e2.rows[i].cells[_1de].innerHTML=_1ef;
_1e2.rows[i].cells[_1df].innerHTML=_1ee;
}
}
}
}
if(this.Owner.MasterTableViewHeader==this){
var _1e2=this.Owner.MasterTableView.Control;
for(var i=0;i<_1e2.rows.length;i++){
if(_1e2.rows[i]!=null){
if((_1e2.rows[i].cells[_1de]!=null)&&(_1e2.rows[i].cells[_1df]!=null)){
if(_1e2.rows[i].cells[_1de].innerHTML!=null){
var _1ee=_1e2.rows[i].cells[_1de].innerHTML;
var _1ef=_1e2.rows[i].cells[_1df].innerHTML;
_1e2.rows[i].cells[_1de].innerHTML=_1ef;
_1e2.rows[i].cells[_1df].innerHTML=_1ee;
}
}
}
}
}
if(_1e0&&(!this.Owner.ClientSettings.ReorderColumnsOnClient)&&(this.Owner.ClientSettings.PostBackReferences.PostBackColumnsReorder!="")){
eval(this.Owner.ClientSettings.PostBackReferences.PostBackColumnsReorder);
}
RadGridNamespace.FireEvent(this,"OnColumnSwapped",[_1de,_1df]);
this.Owner.InitializeFilterMenu(this);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.MoveColumnToLeft=function(_1f0){
if(isNaN(parseInt(_1f0))){
var _1f1="Column index must be of type \"Number\"!";
alert(_1f1);
return;
}
if(_1f0<0){
var _1f1="Column index must be non-negative!";
alert(_1f1);
return;
}
if(_1f0>(this.Columns.length-1)){
var _1f1="Column index must be less than columns count!";
alert(_1f1);
return;
}
if(!this.Owner.ClientSettings.AllowColumnsReorder){
return;
}
try{
if(!RadGridNamespace.FireEvent(this,"OnColumnMovingToLeft",[_1f0])){
return;
}
var _1f2=_1f0--;
this.SwapColumns(_1f0,_1f2);
RadGridNamespace.FireEvent(this,"OnColumnMovedToLeft",[_1f0]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.MoveColumnToRight=function(_1f3){
if(isNaN(parseInt(_1f3))){
var _1f4="Column index must be of type \"Number\"!";
alert(_1f4);
return;
}
if(_1f3<0){
var _1f4="Column index must be non-negative!";
alert(_1f4);
return;
}
if(_1f3>(this.Columns.length-1)){
var _1f4="Column index must be less than columns count!";
alert(_1f4);
return;
}
if(!this.Owner.ClientSettings.AllowColumnsReorder){
return;
}
try{
if(!RadGridNamespace.FireEvent(this,"OnColumnMovingToRight",[_1f3])){
return;
}
var _1f5=_1f3++;
this.SwapColumns(_1f3,_1f5);
RadGridNamespace.FireEvent(this,"OnColumnMovedToRight",[_1f3]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.CanShowHideColumn=function(_1f6){
if(!this.Owner.ClientSettings.AllowColumnHide){
return false;
}
if(isNaN(parseInt(_1f6))){
var _1f7="Column index must be of type \"Number\"!";
alert(_1f7);
return false;
}
if(_1f6<0){
var _1f7="Column index must be non-negative!";
alert(_1f7);
return false;
}
if(_1f6>(this.Columns.length-1)){
var _1f7="Column index must be less than columns count!";
alert(_1f7);
return false;
}
return true;
};
RadGridNamespace.RadGridTable.prototype.HideNotFrozenColumn=function(_1f8){
this.HideShowNotFrozenColumn(_1f8,false);
};
RadGridNamespace.RadGridTable.prototype.ShowNotFrozenColumn=function(_1f9){
this.HideShowNotFrozenColumn(_1f9,true);
};
RadGridNamespace.RadGridTable.prototype.HideShowNotFrozenColumn=function(_1fa,_1fb){
if(this.Owner.MasterTableViewHeader){
this.Owner.MasterTableViewHeader.Columns[_1fa].FrozenDisplay=_1fb;
if(!window.netscape&&navigator.userAgent.toLowerCase().indexOf("safari")==-1){
this.HideShowCol(this.Owner.MasterTableViewHeader,_1fa,_1fb);
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
RadGridNamespace.HideShowCells(this.Owner.MasterTableViewHeader.Control,_1fa,_1fb,this.Owner.MasterTableViewHeader.ColGroup.Cols);
}
}else{
RadGridNamespace.HideShowCells(this.Owner.MasterTableViewHeader.Control,_1fa,_1fb,this.Owner.MasterTableViewHeader.ColGroup.Cols);
}
}
if(this.Owner.MasterTableView){
this.Owner.MasterTableView.Columns[_1fa].FrozenDisplay=_1fb;
if(!window.netscape&&navigator.userAgent.toLowerCase().indexOf("safari")==-1){
this.HideShowCol(this.Owner.MasterTableView,_1fa,_1fb);
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
RadGridNamespace.HideShowCells(this.Owner.MasterTableView.Control,_1fa,_1fb,this.Owner.MasterTableView.ColGroup.Cols);
}
}else{
RadGridNamespace.HideShowCells(this.Owner.MasterTableView.Control,_1fa,_1fb,this.Owner.MasterTableView.ColGroup.Cols);
}
}
if(this.Owner.MasterTableViewFooter){
this.Owner.MasterTableViewFooter.Columns[_1fa].FrozenDisplay=_1fb;
if(!window.netscape&&navigator.userAgent.toLowerCase().indexOf("safari")==-1){
this.HideShowCol(this.Owner.MasterTableViewFooter,_1fa,_1fb);
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
RadGridNamespace.HideShowCells(this.Owner.MasterTableViewFooter.Control,_1fa,_1fb,this.Owner.MasterTableViewFooter.ColGroup.Cols);
}
}else{
RadGridNamespace.HideShowCells(this.Owner.MasterTableViewFooter.Control,_1fa,_1fb,this.Owner.MasterTableViewFooter.ColGroup.Cols);
}
}
};
RadGridNamespace.RadGridTable.prototype.HideColumn=function(_1fc){
if(!this.CanShowHideColumn(_1fc)){
return false;
}
try{
if(!RadGridNamespace.FireEvent(this,"OnColumnHiding",[_1fc])){
return;
}
this.HideShowColumn(_1fc,false);
if(this!=this.Owner.MasterTableViewHeader){
this.Owner.SavePostData("HidedColumns",this.ClientID,this.Columns[_1fc].RealIndex);
}
RadGridNamespace.FireEvent(this,"OnColumnHidden",[_1fc]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.ShowColumn=function(_1fd){
if(!this.CanShowHideColumn(_1fd)){
return false;
}
try{
if(!RadGridNamespace.FireEvent(this,"OnColumnShowing",[_1fd])){
return;
}
this.HideShowColumn(_1fd,true);
if(this!=this.Owner.MasterTableViewHeader){
this.Owner.SavePostData("ShowedColumns",this.ClientID,this.Columns[_1fd].RealIndex);
}
RadGridNamespace.FireEvent(this,"OnColumnShowed",[_1fd]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.HideShowCol=function(_1fe,_1ff,_200){
if(_1fe&&_1fe.ColGroup&&_1fe.ColGroup.Cols&&_1fe.ColGroup.Cols[_1ff]){
_1fe.ColGroup.Cols[_1ff].style.display=(_200)?"":"none";
}
};
RadGridNamespace.RadGridTable.prototype.HideShowColumn=function(_201,_202){
var _202=this.Columns[_201].Display=_202;
if(this.Owner.MasterTableViewHeader){
if(window.netscape||this.Owner.GridHeaderDiv){
this.HideShowCol(this.Owner.MasterTableViewHeader,_201,_202);
}
RadGridNamespace.HideShowCells(this.Owner.MasterTableViewHeader.Control,_201,_202,this.Owner.MasterTableViewHeader.ColGroup.Cols);
}
if(this.Owner.MasterTableView){
if(window.netscape||this.Owner.GridHeaderDiv){
this.HideShowCol(this.Owner.MasterTableView,_201,_202);
}
RadGridNamespace.HideShowCells(this.Owner.MasterTableView.Control,_201,_202,this.Owner.MasterTableView.ColGroup.Cols);
}
if(this.Owner.MasterTableViewFooter){
if(window.netscape||this.Owner.GridHeaderDiv){
this.HideShowCol(this.Owner.MasterTableViewFooter,_201,_202);
}
RadGridNamespace.HideShowCells(this.Owner.MasterTableViewFooter.Control,_201,_202,this.Owner.MasterTableViewFooter.ColGroup.Cols);
}
};
RadGridNamespace.RadGridTable.prototype.CanShowHideRow=function(_203){
if(!this.Owner.ClientSettings.AllowRowHide){
return false;
}
if(isNaN(parseInt(_203))){
var _204="Row index must be of type \"Number\"!";
alert(_204);
return false;
}
if(_203<0){
var _204="Row index must be non-negative!";
alert(_204);
return false;
}
if(_203>(this.Rows.length-1)){
var _204="Row index must be less than rows count!";
alert(_204);
return false;
}
return true;
};
RadGridNamespace.RadGridTable.prototype.HideRow=function(_205){
if(!this.CanShowHideRow(_205)){
return false;
}
try{
if(!RadGridNamespace.FireEvent(this,"OnRowHiding",[_205])){
return;
}
if(this.Rows){
if(this.Rows[_205]){
if(this.Rows[_205].Control){
this.Rows[_205].Control.style.display="none";
this.Rows[_205].Display=false;
}
}
}
if(this!=this.Owner.MasterTableViewHeader){
this.Owner.SavePostData("HidedRows",this.ClientID,this.Rows[_205].RealIndex);
}
RadGridNamespace.FireEvent(this,"OnRowHidden",[_205]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.ShowRow=function(_206){
if(!this.CanShowHideRow(_206)){
return false;
}
try{
if(!RadGridNamespace.FireEvent(this,"OnRowShowing",[_206])){
return;
}
if(this.Rows){
if(this.Rows[_206]){
if(this.Rows[_206].Control){
if(this.Rows[_206].ItemType!="NestedView"){
if(window.netscape){
this.Rows[_206].Control.style.display="table-row";
}else{
this.Rows[_206].Control.style.display="";
}
this.Rows[_206].Display=true;
}
}
}
}
if(this!=this.Owner.MasterTableViewHeader){
this.Owner.SavePostData("ShowedRows",this.ClientID,this.Rows[_206].RealIndex);
}
RadGridNamespace.FireEvent(this,"OnRowShowed",[_206]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.ExportToExcel=function(_207){
try{
this.Owner.SavePostData("ExportToExcel",this.ClientID,_207);
__doPostBack(this.Owner.UniqueID,"ExportToExcel");
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.ExportToWord=function(_208){
try{
this.Owner.SavePostData("ExportToWord",this.ClientID,_208);
__doPostBack(this.Owner.UniqueID,"ExportToWord");
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.ExportToCSV=function(_209){
try{
this.Owner.SavePostData("ExportToCSV",this.ClientID,_209);
__doPostBack(this.Owner.UniqueID,"ExportToCSV");
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.ExportToPdf=function(_20a){
try{
this.Owner.SavePostData("ExportToPdf",this.ClientID,_20a);
__doPostBack(this.Owner.UniqueID,"ExportToPdf");
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.AddToSelectedRows=function(_20b){
try{
this.SelectedRows[this.SelectedRows.length]=_20b;
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.IsInSelectedRows=function(_20c){
try{
for(var i=0;i<this.SelectedRows.length;i++){
if(this.SelectedRows[i]!=_20c){
return true;
}
}
return false;
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.ClearSelectedRows=function(){
var _20e=this.SelectedRows;
for(var i=0;i<this.SelectedRows.length;i++){
if(!RadGridNamespace.FireEvent(this,"OnRowDeselecting",[this.SelectedRows[i]])){
continue;
}
this.SelectedRows[i].Selected=false;
this.SelectedRows[i].CheckClientSelectColumns();
this.SelectedRows[i].RemoveSelectedRowStyle();
var last=this.SelectedRows[i];
try{
this.SelectedRows.splice(i,1);
i--;
}
catch(ex){
}
RadGridNamespace.FireEvent(this,"OnRowDeselected",[last]);
}
this.SelectedRows=new Array();
};
RadGridNamespace.RadGridTable.prototype.RemoveFromSelectedRows=function(_211){
try{
var _212=new Array();
for(var i=0;i<this.SelectedRows.length;i++){
var last=this.SelectedRows[i];
if(this.SelectedRows[i]!=_211){
_212[_212.length]=this.SelectedRows[i];
}else{
if(!this.Owner.AllowMultiRowSelection){
if(!RadGridNamespace.FireEvent(this,"OnRowDeselecting",[this.SelectedRows[i]])){
continue;
}
}
try{
this.SelectedRows.splice(i,1);
i--;
}
catch(ex){
}
_211.CheckClientSelectColumns();
setTimeout(function(){
RadGridNamespace.FireEvent(_211.Owner,"OnRowDeselected",[_211]);
},100);
}
}
this.SelectedRows=_212;
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.GetSelectedRowsIndexes=function(){
try{
var _215=new Array();
for(var i=0;i<this.SelectedRows.length;i++){
_215[_215.length]=this.SelectedRows[i].RealIndex;
}
return _215.join(",");
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.GetCellByColumnUniqueName=function(_217,_218){
if(this.ClientID.indexOf("_Header")!=-1){
return;
}
if((!_217)||(!_218)){
return;
}
if(!this.Columns){
return;
}
for(var i=0;i<this.Columns.length;i++){
if(this.Columns[i].UniqueName.toUpperCase()==_218.toUpperCase()){
return _217.Control.cells[i];
}
}
return null;
};
RadGridNamespace.RadGridTableColumn=function(_21a){
if((!_21a)||typeof (_21a)!="object"){
return;
}
RadControlsNamespace.DomEventMixin.Initialize(this);
for(var _21b in _21a){
this[_21b]=_21a[_21b];
}
this.Type="RadGridTableColumn";
this.ResizeTolerance=5;
this.CanResize=false;
};
RadGridNamespace.RadGridTableColumn.prototype._constructor=function(_21c,_21d){
this.Control=_21c;
this.Owner=_21d;
this.Index=_21c.cellIndex;
if((window.opera&&typeof (_21c.cellIndex)=="undefined")||(navigator.userAgent.indexOf("Safari")!=-1)){
var _21e=this;
setTimeout(function(){
_21e.Index=RadGridNamespace.GetRealCellIndex(_21e.Owner,_21e.Control);
},200);
}
this.AttachDomEvent(this.Control,"click","OnClick");
this.AttachDomEvent(this.Control,"dblclick","OnDblClick");
this.AttachDomEvent(this.Control,"mousemove","OnMouseMove");
this.AttachDomEvent(this.Control,"mousedown","OnMouseDown");
this.AttachDomEvent(this.Control,"mouseup","OnMouseUp");
this.AttachDomEvent(this.Control,"mouseover","OnMouseOver");
this.AttachDomEvent(this.Control,"mouseout","OnMouseOut");
this.AttachDomEvent(this.Control,"contextmenu","OnContextMenu");
};
RadGridNamespace.RadGridTableColumn.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
if(this.ColumnResizer){
this.ColumnResizer.Dispose();
}
this.Control=null;
this.Owner=null;
this.Index=null;
};
RadGridNamespace.RadGridTableColumn.prototype.OnContextMenu=function(e){
try{
if(!RadGridNamespace.FireEvent(this.Owner,"OnColumnContextMenu",[this.Index,e])){
return;
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.Owner.OnError);
}
};
RadGridNamespace.RadGridTableColumn.prototype.OnClick=function(e){
try{
if(!RadGridNamespace.FireEvent(this.Owner,"OnColumnClick",[this.Index])){
return;
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.Owner.OnError);
}
};
RadGridNamespace.RadGridTableColumn.prototype.OnDblClick=function(e){
try{
if(!RadGridNamespace.FireEvent(this.Owner,"OnColumnDblClick",[this.Index])){
return;
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.Owner.OnError);
}
};
RadGridNamespace.RadGridTableColumn.prototype.OnMouseMove=function(e){
if(this.Owner.Owner.ClientSettings&&this.Owner.Owner.ClientSettings.Resizing.AllowColumnResize&&this.Resizable&&this.Control.tagName.toLowerCase()=="th"){
var _223=RadGridNamespace.GetEventPosX(e);
var _224=RadGridNamespace.FindPosX(this.Control);
var endX=_224+this.Control.offsetWidth;
var _226=RadGridNamespace.GetCurrentElement(e);
if(this.Owner.Owner.GridDataDiv&&!this.Owner.Owner.GridHeaderDiv&&!window.netscape){
var _227=0;
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
_227=parseInt(document.body.currentStyle.marginLeft);
}
this.ResizeTolerance=10;
}
if((_223>=endX-this.ResizeTolerance)&&(_223<=endX+this.ResizeTolerance)&&!this.Owner.Owner.MoveHeaderDiv){
this.Control.style.cursor="e-resize";
this.Control.title=this.Owner.Owner.ClientSettings.ClientMessages.DragToResize;
this.CanResize=true;
_226.style.cursor="e-resize";
this.Owner.Owner.IsResize=true;
}else{
this.Control.style.cursor="";
this.Control.title="";
this.CanResize=false;
_226.style.cursor="";
this.Owner.Owner.IsResize=false;
}
}
};
RadGridNamespace.RadGridTableColumn.prototype.OnMouseDown=function(e){
if(this.CanResize){
if(((window.netscape||window.opera||navigator.userAgent.indexOf("Safari")!=-1)&&(e.button==0))||(e.button==1)){
var _229=RadGridNamespace.GetEventPosX(e);
var _22a=RadGridNamespace.FindPosX(this.Control);
var endX=_22a+this.Control.offsetWidth;
if((_229>=endX-this.ResizeTolerance)&&(_229<=endX+this.ResizeTolerance)){
this.ColumnResizer=new RadGridNamespace.RadGridColumnResizer(this,this.Owner.Owner.ClientSettings.Resizing.EnableRealTimeResize);
this.ColumnResizer.Position(e);
}
}
RadGridNamespace.ClearDocumentEvents();
}
};
RadGridNamespace.RadGridTableColumn.prototype.OnMouseUp=function(e){
RadGridNamespace.RestoreDocumentEvents();
};
RadGridNamespace.RadGridTableColumn.prototype.OnMouseOver=function(e){
if(!RadGridNamespace.FireEvent(this.Owner,"OnColumnMouseOver",[this.Index])){
return;
}
if(this.Owner.Owner.Skin!=""&&this.Owner.Owner.Skin!="None"){
RadGridNamespace.addClassName(this.Control,"GridHeaderOver_"+this.Owner.Owner.Skin);
}
};
RadGridNamespace.RadGridTableColumn.prototype.OnMouseOut=function(e){
if(!RadGridNamespace.FireEvent(this.Owner,"OnColumnMouseOut",[this.Index])){
return;
}
if(this.Owner.Owner.Skin!=""&&this.Owner.Owner.Skin!="None"){
RadGridNamespace.removeClassName(this.Control,"GridHeaderOver_"+this.Owner.Owner.Skin);
}
};
RadGridNamespace.RadGridColumnResizer=function(_22f,_230){
if(!_22f){
return;
}
RadControlsNamespace.DomEventMixin.Initialize(this);
this.Column=_22f;
this.IsRealTimeResize=_230;
this.CurrentWidth=null;
this.LeftResizer=document.createElement("span");
this.LeftResizer.style.backgroundColor="navy";
this.LeftResizer.style.width="1"+"px";
this.LeftResizer.style.position="absolute";
this.LeftResizer.style.cursor="e-resize";
this.RightResizer=document.createElement("span");
this.RightResizer.style.backgroundColor="navy";
this.RightResizer.style.width="1"+"px";
this.RightResizer.style.position="absolute";
this.RightResizer.style.cursor="e-resize";
this.ResizerToolTip=document.createElement("span");
this.ResizerToolTip.style.backgroundColor="#F5F5DC";
this.ResizerToolTip.style.border="1px solid";
this.ResizerToolTip.style.position="absolute";
this.ResizerToolTip.style.font="icon";
this.ResizerToolTip.style.padding="2";
this.ResizerToolTip.innerHTML="Width: <b>"+this.Column.Control.offsetWidth+"</b> <em>pixels</em>";
document.body.appendChild(this.LeftResizer);
document.body.appendChild(this.RightResizer);
document.body.appendChild(this.ResizerToolTip);
this.CanDestroy=true;
this.AttachDomEvent(document,"mouseup","OnMouseUp");
this.AttachDomEvent(this.Column.Owner.Owner.Control,"mousemove","OnMouseMove");
};
RadGridNamespace.RadGridColumnResizer.prototype.OnMouseUp=function(e){
this.Destroy(e);
};
RadGridNamespace.RadGridColumnResizer.prototype.OnMouseMove=function(e){
this.Move(e);
};
RadGridNamespace.RadGridColumnResizer.prototype.Position=function(e){
this.LeftResizer.style.top=RadGridNamespace.FindPosY(this.Column.Control)-RadGridNamespace.FindScrollPosY(this.Column.Control)+document.documentElement.scrollTop+document.body.scrollTop+"px";
this.LeftResizer.style.left=RadGridNamespace.FindPosX(this.Column.Control)-RadGridNamespace.FindScrollPosX(this.Column.Control)+document.documentElement.scrollLeft+document.body.scrollLeft+"px";
this.RightResizer.style.top=this.LeftResizer.style.top;
this.RightResizer.style.left=parseInt(this.LeftResizer.style.left)+this.Column.Control.offsetWidth+"px";
this.ResizerToolTip.style.top=parseInt(this.RightResizer.style.top)-20+"px";
this.ResizerToolTip.style.left=parseInt(this.RightResizer.style.left)-5+"px";
if(parseInt(this.LeftResizer.style.left)<RadGridNamespace.FindPosX(this.Column.Owner.Control)){
this.LeftResizer.style.display="none";
}
if(!this.Column.Owner.Owner.ClientSettings.Scrolling.AllowScroll){
this.LeftResizer.style.height=this.Column.Owner.Control.tBodies[0].offsetHeight+this.Column.Owner.Control.tHead.offsetHeight+"px";
}else{
if(this.Column.Owner.Owner.ClientSettings.Scrolling.UseStaticHeaders){
this.LeftResizer.style.height=this.Column.Owner.Owner.GridDataDiv.clientHeight+this.Column.Owner.Control.tHead.offsetHeight+"px";
}else{
this.LeftResizer.style.height=this.Column.Owner.Owner.GridDataDiv.clientHeight+"px";
}
}
this.RightResizer.style.height=this.LeftResizer.style.height;
};
RadGridNamespace.RadGridColumnResizer.prototype.Destroy=function(e){
if(this.CanDestroy){
this.DetachDomEvent(document,"mouseup","OnMouseUp");
this.DetachDomEvent(this.Column.Owner.Owner.Control,"mousemove","OnMouseMove");
if(this.CurrentWidth!=null){
if(this.CurrentWidth>0){
this.Column.Owner.ResizeColumn(this.Column.Control.cellIndex,this.CurrentWidth);
this.CurrentWidth=null;
}
}
document.body.removeChild(this.LeftResizer);
document.body.removeChild(this.RightResizer);
document.body.removeChild(this.ResizerToolTip);
this.CanDestroy=false;
}
};
RadGridNamespace.RadGridColumnResizer.prototype.Dispose=function(){
try{
this.Destroy();
}
catch(error){
}
this.DisposeDomEventHandlers();
this.MouseUpHandler=null;
this.MouseMoveHandler=null;
this.LeftResizer=null;
this.RightResizer=null;
this.ResizerToolTip=null;
};
RadGridNamespace.RadGridColumnResizer.prototype.Move=function(e){
this.LeftResizer.style.left=RadGridNamespace.FindPosX(this.Column.Control)-RadGridNamespace.FindScrollPosX(this.Column.Control)+document.documentElement.scrollLeft+document.body.scrollLeft+"px";
this.RightResizer.style.left=parseInt(this.LeftResizer.style.left)+(RadGridNamespace.GetEventPosX(e)-RadGridNamespace.FindPosX(this.Column.Control))+"px";
this.ResizerToolTip.style.left=parseInt(this.RightResizer.style.left)-5+"px";
var _236=parseInt(this.RightResizer.style.left)-parseInt(this.LeftResizer.style.left);
var _237=this.Column.Control.scrollWidth-_236;
this.ResizerToolTip.innerHTML="Width: <b>"+_236+"</b> <em>pixels</em>";
if(!RadGridNamespace.FireEvent(this.Column.Owner,"OnColumnResizing",[this.Column.Index,_236])){
return;
}
if(_236<=0){
this.RightResizer.style.left=this.RightResizer.style.left;
this.Destroy(e);
return;
}
this.CurrentWidth=_236;
if(this.IsRealTimeResize){
var _238=(navigator.userAgent.indexOf("Safari")!=-1)?RadGridNamespace.GetRealCellIndex(this.Column.Owner,this.Column.Control):this.Column.Control.cellIndex;
this.Column.Owner.ResizeColumn(_238,_236);
}else{
this.CurrentWidth=_236;
return;
}
if(RadGridNamespace.FindPosX(this.LeftResizer)!=RadGridNamespace.FindPosX(this.Column.Control)){
this.LeftResizer.style.left=RadGridNamespace.FindPosX(this.Column.Control)+"px";
}
if(RadGridNamespace.FindPosX(this.RightResizer)!=(RadGridNamespace.FindPosX(this.Column.Control)+this.Column.Control.offsetWidth)){
this.RightResizer.style.left=RadGridNamespace.FindPosX(this.Column.Control)+this.Column.Control.offsetWidth+"px";
}
if(RadGridNamespace.FindPosY(this.LeftResizer)!=RadGridNamespace.FindPosY(this.Column.Control)){
this.LeftResizer.style.top=RadGridNamespace.FindPosY(this.Column.Control)+"px";
this.RightResizer.style.top=RadGridNamespace.FindPosY(this.Column.Control)+"px";
}
if(this.Column.Owner.Owner.GridDataDiv){
this.LeftResizer.style.left=parseInt(this.LeftResizer.style.left.replace("px",""))-this.Column.Owner.Owner.GridDataDiv.scrollLeft+"px";
this.RightResizer.style.left=parseInt(this.LeftResizer.style.left.replace("px",""))+this.Column.Control.offsetWidth+"px";
this.ResizerToolTip.style.left=parseInt(this.RightResizer.style.left)-5+"px";
}
if(!this.Column.Owner.Owner.ClientSettings.Scrolling.AllowScroll){
this.LeftResizer.style.height=this.Column.Owner.Control.tBodies[0].offsetHeight+this.Column.Owner.Control.tHead.offsetHeight+"px";
}else{
if(this.Column.Owner.Owner.ClientSettings.Scrolling.UseStaticHeaders){
this.LeftResizer.style.height=this.Column.Owner.Owner.GridDataDiv.clientHeight+this.Column.Owner.Control.tHead.offsetHeight+"px";
}else{
this.LeftResizer.style.height=this.Column.Owner.Owner.GridDataDiv.clientHeight+"px";
}
}
this.RightResizer.style.height=this.LeftResizer.style.height;
};
RadGridNamespace.RadGridTableRow=function(_239){
if((!_239)||typeof (_239)!="object"){
return;
}
RadControlsNamespace.DomEventMixin.Initialize(this);
for(var _23a in _239){
this[_23a]=_239[_23a];
}
this.Type="RadGridTableRow";
var _23b=document.getElementById(this.OwnerID);
this.Control=_23b.tBodies[0].rows[this.ClientRowIndex];
if(!this.Control){
return;
}
this.Index=this.Control.sectionRowIndex;
this.RealIndex=this.RowIndex;
};
RadGridNamespace.RadGridTableRow.prototype._constructor=function(_23c){
this.Owner=_23c;
this.CreateStyles();
if(this.Selected){
this.LoadSelected();
}
this.CheckClientSelectColumns();
if(this.Owner.HierarchyLoadMode=="Client"){
if(this.Owner.Owner.ClientSettings.AllowExpandCollapse){
for(var i=0;i<this.Owner.ExpandCollapseColumns.length;i++){
var _23e=this.Owner.ExpandCollapseColumns[i].Control.cellIndex;
var _23f=this.Control.cells[_23e];
var html=this.Control.innerHTML;
if(!_23f){
continue;
}
var _241;
for(var j=0;j<_23f.childNodes.length;j++){
if(!_23f.childNodes[j].tagName){
continue;
}
var _243;
if(this.Owner.ExpandCollapseColumns[i].ButtonType=="ImageButton"){
_243="img";
}else{
if(this.Owner.ExpandCollapseColumns[i].ButtonType=="LinkButton"){
_243="a";
}else{
if(this.Owner.ExpandCollapseColumns[i].ButtonType=="PushButton"){
_243="button";
}
}
}
if(_23f.childNodes[j].tagName.toLowerCase()==_243){
_241=_23f.childNodes[j];
break;
}
}
if(_241){
var _244=this;
var _245=function(){
_244.OnHierarchyExpandButtonClick(this);
};
_241.onclick=_245;
_241.ondblclick=null;
_245=null;
}
_241=null;
}
}
}
if(this.Owner.GroupLoadMode=="Client"){
if(this.Owner.Owner.ClientSettings.AllowGroupExpandCollapse){
for(var i=0;i<this.Owner.GroupSplitterColumns.length;i++){
var _23e=this.Owner.GroupSplitterColumns[i].Control.cellIndex;
var html=this.Control.innerHTML;
var _23f=this.Control.cells[_23e];
if(!_23f){
continue;
}
var _241;
for(var j=0;j<_23f.childNodes.length;j++){
if(!_23f.childNodes[j].tagName){
continue;
}
if(_23f.childNodes[j].tagName.toLowerCase()=="img"){
_241=_23f.childNodes[j];
break;
}
}
if(_241){
var _244=this;
var _245=function(){
_244.OnGroupExpandButtonClick(this);
};
_241.onclick=_245;
_241.ondblclick=null;
_245=null;
}
_241=null;
}
}
}
this.AttachDomEvent(this.Control,"click","OnClick");
this.AttachDomEvent(this.Control,"dblclick","OnDblClick");
this.AttachDomEvent(document,"mousedown","OnMouseDown");
this.AttachDomEvent(document,"mouseup","OnMouseUp");
this.AttachDomEvent(document,"mousemove","OnMouseMove");
this.AttachDomEvent(this.Control,"mouseover","OnMouseOver");
this.AttachDomEvent(this.Control,"mouseout","OnMouseOut");
this.AttachDomEvent(this.Control,"contextmenu","OnContextMenu");
if(this.Owner.Owner.ClientSettings.ActiveRowData&&this.Owner.Owner.ClientSettings.ActiveRowData!=""){
var data=this.Owner.Owner.ClientSettings.ActiveRowData.split(";")[0].split(",");
if(data[0]==this.Owner.ClientID&&data[1]==this.RealIndex){
this.Owner.Owner.ActiveRow=this;
}
}
};
RadGridNamespace.GroupRowExpander=function(_247){
this.startRow=_247;
};
RadGridNamespace.GroupRowExpander.prototype.NotFinished=function(_248){
var _249=(this.currentGridRow!=null);
if(!_249){
return false;
}
var _24a=(this.currentGridRow.GroupIndex=="");
var _24b=(this.currentGridRow.GroupIndex==_248.GroupIndex);
var _24c=(this.currentGridRow.GroupIndex.indexOf(_248.GroupIndex+"_")==0);
return (_24a||_24b||_24c);
};
RadGridNamespace.GroupRowExpander.prototype.ToggleExpandCollapse=function(_24d){
var _24e=this.startRow;
var _24f=_24e.Owner;
var _250=_24d.parentNode.parentNode.sectionRowIndex;
var _251=_24f.Rows[_250];
if(_251.Expanded){
if(!RadGridNamespace.FireEvent(_251.Owner,"OnGroupCollapsing",[_251])){
return;
}
}else{
if(!RadGridNamespace.FireEvent(_251.Owner,"OnGroupExpanding",[_251])){
return;
}
}
var _252=_24f.Control.rows[_250+1];
if(!_252){
return;
}
this.currentRowIndex=_252.rowIndex;
this.lastGroupIndex=null;
while(true){
this.currentGridRow=_24f.Rows[this.currentRowIndex];
var _253=this.NotFinished(_251);
if(!_253){
break;
}
var _254=(this.lastGroupIndex!=null)&&(this.currentGridRow.GroupIndex.indexOf(this.lastGroupIndex)!=-1);
var _255=(this.currentGridRow.ItemType!="GroupHeader")&&(!this.currentGridRow.IsVisible());
var _256=_254&&_255;
if(this.currentGridRow.ItemType=="GroupHeader"&&!this.currentGridRow.Expanded){
if(this.currentGridRow.IsVisible()){
this.currentGridRow.Hide();
_24d.src=_24f.GroupSplitterColumns[0].ExpandImageUrl;
_24d.title=_24f.Owner.GroupingSettings.ExpandTooltip;
if(_24f.Rows[this.currentRowIndex+1]==null||_24f.Rows[this.currentRowIndex+1].ItemType=="GroupHeader"){
this.currentGridRow.Expanded=false;
}
}else{
_24d.src=_24f.GroupSplitterColumns[0].CollapseImageUrl;
_24d.title=_24f.Owner.GroupingSettings.CollapseTooltip;
this.currentGridRow.Show();
if(_24f.Rows[this.currentRowIndex+1]==null||_24f.Rows[this.currentRowIndex+1].ItemType=="GroupHeader"){
this.currentGridRow.Expanded=true;
}
}
this.lastGroupIndex=this.currentGridRow.GroupIndex;
}else{
if(!_256){
if(this.currentGridRow.ItemType=="NestedView"){
if(this.currentGridRow.Expanded){
if(this.currentGridRow.IsVisible()){
this.currentGridRow.Hide();
}else{
this.currentGridRow.Show();
}
}
}else{
if(this.currentGridRow.IsVisible()){
this.currentGridRow.Hide();
_24d.src=_24f.GroupSplitterColumns[0].ExpandImageUrl;
_24d.title=_24f.Owner.GroupingSettings.ExpandTooltip;
_251.Expanded=false;
}else{
_24d.src=_24f.GroupSplitterColumns[0].CollapseImageUrl;
_24d.title=_24f.Owner.GroupingSettings.CollapseTooltip;
this.currentGridRow.Show();
_251.Expanded=true;
}
}
}
}
this.currentRowIndex++;
}
if(_251.Expanded!=null){
if(_251.Expanded){
_24f.Owner.SavePostData("ExpandedGroupRows",_24f.ClientID,_251.RealIndex);
_24e.title=_24f.Owner.GroupingSettings.CollapseTooltip;
}else{
_24f.Owner.SavePostData("CollapsedGroupRows",_24f.ClientID,_251.RealIndex);
_24e.title=_24f.Owner.GroupingSettings.ExpandTooltip;
}
}
if(_251.Expanded){
if(!RadGridNamespace.FireEvent(_251.Owner,"OnGroupExpanded",[_251])){
return;
}
}else{
if(!RadGridNamespace.FireEvent(_251.Owner,"OnGroupCollapsed",[_251])){
return;
}
}
};
RadGridNamespace.RadGridTableRow.prototype.OnGroupExpandButtonClick=function(_257){
var _258=new RadGridNamespace.GroupRowExpander(this);
_258.ToggleExpandCollapse(_257);
};
RadGridNamespace.RadGridTableRow.prototype.OnHierarchyExpandButtonClick=function(_259){
var _25a=this.Owner.Control.rows[_259.parentNode.parentNode.rowIndex+1];
var _25b=this.Owner.Rows[_259.parentNode.parentNode.sectionRowIndex];
if(!_25a){
return;
}
if(this.TableRowIsVisible(_25a)){
if(!RadGridNamespace.FireEvent(this.Owner,"OnHierarchyCollapsing",[this])){
return;
}
this.HideTableRow(_25a);
_25b.Expanded=false;
if(this.Owner.ExpandCollapseColumns[0].ButtonType=="ImageButton"){
_259.src=this.Owner.ExpandCollapseColumns[0].ExpandImageUrl;
}else{
_259.innerHTML="+";
}
_259.title=this.Owner.Owner.HierarchySettings.ExpandTooltip;
this.Owner.Owner.SavePostData("CollapsedRows",this.Owner.ClientID,this.RealIndex);
if(!RadGridNamespace.FireEvent(this.Owner,"OnHierarchyCollapsed",[this])){
return;
}
}else{
if(!RadGridNamespace.FireEvent(this.Owner,"OnHierarchyExpanding",[this])){
return;
}
if(this.Owner.ExpandCollapseColumns[0].ButtonType=="ImageButton"){
_259.src=this.Owner.ExpandCollapseColumns[0].CollapseImageUrl;
}else{
_259.innerHTML="-";
}
_259.title=this.Owner.Owner.HierarchySettings.CollapseTooltip;
this.ShowTableRow(_25a);
_25b.Expanded=true;
this.Owner.Owner.SavePostData("ExpandedRows",this.Owner.ClientID,this.RealIndex);
if(!RadGridNamespace.FireEvent(this.Owner,"OnHierarchyExpanded",[this])){
return;
}
}
};
RadGridNamespace.RadGridTableRow.prototype.TableRowIsVisible=function(_25c){
return _25c.style.display!="none";
};
RadGridNamespace.RadGridTableRow.prototype.IsVisible=function(){
return this.TableRowIsVisible(this.Control);
};
RadGridNamespace.RadGridTableRow.prototype.HideTableRow=function(_25d){
if(this.TableRowIsVisible(_25d)){
_25d.style.display="none";
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
var _25e=_25d.getElementsByTagName("select");
for(var i=0;i<_25e.length;i++){
_25e[i].style.display="none";
}
}
}
};
RadGridNamespace.RadGridTableRow.prototype.Hide=function(){
this.HideTableRow(this.Control);
};
RadGridNamespace.RadGridTableRow.prototype.ShowTableRow=function(_260){
if(window.netscape||window.opera){
_260.style.display="table-row";
}else{
_260.style.display="block";
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
var _261=_260.getElementsByTagName("select");
for(var i=0;i<_261.length;i++){
_261[i].style.display="";
}
}
}
};
RadGridNamespace.RadGridTableRow.prototype.Show=function(){
this.ShowTableRow(this.Control);
};
RadGridNamespace.RadGridTableRow.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
this.Control=null;
this.Owner=null;
};
RadGridNamespace.RadGridTableRow.prototype.CreateStyles=function(){
if(!this.Owner.Owner.ClientSettings.ApplyStylesOnClient){
return;
}
switch(this.ItemType){
case "GroupHeader":
break;
case "EditFormItem":
this.Control.className+=" "+this.Owner.RenderEditItemStyleClass;
this.Control.style.cssText+=" "+this.Owner.RenderEditItemStyle;
break;
default:
var _263=eval("this.Owner.Render"+this.ItemType+"StyleClass");
if(typeof (_263)!="undefined"){
this.Control.className+=" "+_263;
}
var _264=eval("this.Owner.Render"+this.ItemType+"Style");
if(typeof (_264)!="undefined"){
this.Control.style.cssText+=" "+_264;
}
break;
}
if(!this.Display){
if(this.Control.style.cssText!=""){
if(this.Control.style.cssText.lastIndexOf(";")==this.Control.style.cssText.length-1){
this.Control.style.cssText+="display:none;";
}else{
this.Control.style.cssText+=";display:none;";
}
}else{
this.Control.style.cssText+="display:none;";
}
}
};
RadGridNamespace.RadGridTableRow.prototype.OnContextMenu=function(e){
try{
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowContextMenu",[this.Index,e])){
return;
}
if(this.Owner.Owner.ClientSettings.ClientEvents.OnRowContextMenu!=""){
if(e.preventDefault){
e.preventDefault();
}else{
e.returnValue=false;
return false;
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.Owner.OnError);
}
};
RadGridNamespace.RadGridTableRow.prototype.OnClick=function(e){
try{
if(this.Owner.Owner.RowResizer){
return;
}
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowClick",[this.Control.sectionRowIndex,e])){
return;
}
if(e.shiftKey&&this.Owner.SelectedRows[0]){
if(this.Owner.SelectedRows[0].Control.rowIndex>this.Control.rowIndex){
for(var i=this.Control.rowIndex;i<this.Owner.SelectedRows[0].Control.rowIndex+1;i++){
var _268=this.Owner.Owner.GetRowObjectByRealRow(this.Owner,this.Owner.Control.rows[i]);
if(_268){
if(!_268.Selected){
this.Owner.SelectRow(this.Owner.Control.rows[i],false);
}
}
}
}
if(this.Owner.SelectedRows[0].Control.rowIndex<this.Control.rowIndex){
for(var i=this.Owner.SelectedRows[0].Control.rowIndex;i<this.Control.rowIndex+1;i++){
var _268=this.Owner.Owner.GetRowObjectByRealRow(this.Owner,this.Owner.Control.rows[i]);
if(_268){
if(!_268.Selected){
this.Owner.SelectRow(this.Owner.Control.rows[i],false);
}
}
}
}
}
if(!e.shiftKey){
this.HandleRowSelection(e);
}
var _269=RadGridNamespace.GetCurrentElement(e);
if(!_269){
return;
}
if(!_269.tagName){
return;
}
if(_269.tagName.toLowerCase()=="input"||_269.tagName.toLowerCase()=="select"||_269.tagName.toLowerCase()=="option"||_269.tagName.toLowerCase()=="button"||_269.tagName.toLowerCase()=="a"||_269.tagName.toLowerCase()=="textarea"){
return;
}
if(this.ItemType=="Item"||this.ItemType=="AlternatingItem"){
if(this.Owner.Owner.ClientSettings.EnablePostBackOnRowClick){
var _26a=this.Owner.Owner.ClientSettings.PostBackFunction;
_26a=_26a.replace("{0}",this.Owner.Owner.UniqueID).replace("{1}","RowClick;"+this.ItemIndexHierarchical);
var form=document.getElementById(this.Owner.Owner.FormID);
if(form!=null&&form["__EVENTTARGET"]!=null&&form["__EVENTTARGET"].value==this.Owner.Owner.UniqueID){
form["__EVENTTARGET"].value="";
}
if(form!=null&&form["__EVENTTARGET"]!=null&&form["__EVENTTARGET"].value==""){
eval(_26a);
}
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.Owner.OnError);
}
};
RadGridNamespace.RadGridTableRow.prototype.HandleActiveRow=function(e){
var _26d=RadGridNamespace.GetCurrentElement(e);
if(_26d!=null&&_26d.tagName&&(_26d.tagName.toLowerCase()=="input"||_26d.tagName.toLowerCase()=="textarea")){
return;
}
if(this.Owner.Owner.ActiveRow!=null){
if(!RadGridNamespace.FireEvent(this.Owner,"OnActiveRowChanging",[this.Owner.Owner.ActiveRow])){
return;
}
if(e.keyCode==13){
this.Owner.Owner.SavePostData("EditRow",this.Owner.ClientID,this.Owner.Owner.ActiveRow.RealIndex);
eval(this.Owner.Owner.ClientSettings.PostBackReferences.PostBackEditRow);
}
if(e.keyCode==40){
var _26e=this.Owner.Rows[this.Owner.Owner.ActiveRow.Control.sectionRowIndex+1];
if(_26e!=null){
this.Owner.Owner.SetActiveRow(_26e);
this.ScrollIntoView(_26e);
}
}
if(e.keyCode==39){
return;
var _26e=this.Owner.Owner.GetNextHierarchicalRow(_26f,this.Owner.Owner.ActiveRow.Control.sectionRowIndex);
if(_26e!=null){
_26f=_26e.parentNode.parentNode;
this.Owner.Owner.SetActiveRow(_26f,_26e.sectionRowIndex);
this.ScrollIntoView(_26e);
}
}
if(e.keyCode==38){
var _270=this.Owner.Rows[this.Owner.Owner.ActiveRow.Control.sectionRowIndex-1];
if(_270!=null){
this.Owner.Owner.SetActiveRow(_270);
this.ScrollIntoView(_270);
}
}
if(e.keyCode==37){
return;
var _270=this.Owner.Owner.GetPreviousHierarchicalRow(_26f,this.Owner.Owner.ActiveRow.Control.sectionRowIndex);
if(_270!=null){
var _26f=_270.parentNode.parentNode;
this.Owner.Owner.SetActiveRow(_26f,_270.sectionRowIndex);
this.ScrollIntoView(_270);
}
}
if(e.keyCode==32){
if(this.Owner.Owner.ClientSettings.Selecting.AllowRowSelect){
this.Owner.Owner.ActiveRow.Owner.SelectRow(this.Owner.Owner.ActiveRow.Control,!this.Owner.Owner.AllowMultiRowSelection);
}
}
}
RadGridNamespace.FireEvent(this.Owner,"OnActiveRowChanged",[this.Owner.Owner.ActiveRow]);
if(window.netscape){
e.preventDefault();
return false;
}else{
e.returnValue=false;
}
};
RadGridNamespace.RadGridTableRow.prototype.ScrollIntoView=function(row){
if(row.Control&&row.Control.focus){
row.Control.scrollIntoView(false);
try{
row.Control.focus();
}
catch(e){
}
}
};
RadGridNamespace.RadGridTableRow.prototype.HandleExpandCollapse=function(){
};
RadGridNamespace.RadGridTableRow.prototype.HandleGroupExpandCollapse=function(){
};
RadGridNamespace.RadGridTableRow.prototype.HandleRowSelection=function(e){
var _273=RadGridNamespace.GetCurrentElement(e);
if(_273.onclick){
return;
}
if(_273.tagName.toLowerCase()=="input"||_273.tagName.toLowerCase()=="select"||_273.tagName.toLowerCase()=="option"||_273.tagName.toLowerCase()=="button"||_273.tagName.toLowerCase()=="a"||_273.tagName.toLowerCase()=="textarea"||_273.tagName.toLowerCase()=="img"){
return;
}
this.SetSelected(!e.ctrlKey,e);
};
RadGridNamespace.RadGridTableRow.prototype.CheckClientSelectColumns=function(){
if(!this.Owner.Columns){
return;
}
for(var i=0;i<this.Owner.Columns.length;i++){
if(this.Owner.Columns[i].ColumnType=="GridClientSelectColumn"){
var cell=this.Owner.GetCellByColumnUniqueName(this,this.Owner.Columns[i].UniqueName);
if(cell!=null){
var _276=cell.getElementsByTagName("input")[0];
if(_276!=null){
_276.checked=this.Selected;
}
}
}
}
};
RadGridNamespace.RadGridTableRow.prototype.SetSelected=function(_277,e){
if(!this.Selected){
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowSelecting",[this,e])){
return;
}
}
if((this.ItemType=="Item")||(this.ItemType=="AlternatingItem")){
if(_277){
this.SingleSelect();
}else{
this.MultiSelect();
}
}
this.CheckClientSelectColumns();
if(this.Selected){
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowSelected",[this,e])){
return;
}
}
};
RadGridNamespace.RadGridTableRow.prototype.SingleSelect=function(){
if(!this.Owner.Owner.ClientSettings.Selecting.AllowRowSelect){
return;
}
this.Owner.ClearSelectedRows();
this.Owner.Owner.ClearSelectedRows();
this.Selected=true;
this.ApplySelectedRowStyle();
this.Owner.AddToSelectedRows(this);
this.Owner.Owner.UpdateClientRowSelection();
};
RadGridNamespace.RadGridTableRow.prototype.SingleDeselect=function(){
if(!this.Owner.Owner.ClientSettings.Selecting.AllowRowSelect){
return;
}
this.Owner.ClearSelectedRows();
this.Owner.Owner.ClearSelectedRows();
this.Selected=false;
this.RemoveSelectedRowStyle();
this.Owner.RemoveFromSelectedRows(this);
this.Owner.Owner.UpdateClientRowSelection();
};
RadGridNamespace.RadGridTableRow.prototype.MultiSelect=function(){
if((!this.Owner.Owner.ClientSettings.Selecting.AllowRowSelect)||(!this.Owner.Owner.AllowMultiRowSelection)){
return;
}
if(this.Selected){
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowDeselecting",[this])){
return;
}
this.Selected=false;
this.RemoveSelectedRowStyle();
this.Owner.RemoveFromSelectedRows(this);
this.Owner.Owner.UpdateClientRowSelection();
}else{
this.Selected=true;
this.ApplySelectedRowStyle();
this.Owner.AddToSelectedRows(this);
this.Owner.Owner.UpdateClientRowSelection();
}
};
RadGridNamespace.RadGridTableRow.prototype.LoadSelected=function(){
this.ApplySelectedRowStyle();
this.Owner.AddToSelectedRows(this);
};
RadGridNamespace.RadGridTableRow.prototype.ApplySelectedRowStyle=function(){
if(!this.Owner.SelectedItemStyleClass||this.Owner.SelectedItemStyleClass==""){
if(this.Owner.SelectedItemStyle&&this.Owner.SelectedItemStyle!=""){
RadGridNamespace.addClassName(this.Control,"SelectedItemStyle"+this.Owner.ClientID+"1");
}else{
RadGridNamespace.addClassName(this.Control,"SelectedItemStyle"+this.Owner.ClientID+"2");
}
}else{
RadGridNamespace.addClassName(this.Control,this.Owner.SelectedItemStyleClass);
}
};
RadGridNamespace.RadGridTableRow.prototype.RemoveSelectedRowStyle=function(){
if(this.Owner.SelectedItemStyle){
RadGridNamespace.removeClassName(this.Control,"SelectedItemStyle"+this.Owner.ClientID+"1");
}else{
RadGridNamespace.removeClassName(this.Control,"SelectedItemStyle"+this.Owner.ClientID+"2");
}
RadGridNamespace.removeClassName(this.Control,this.Owner.SelectedItemStyleClass);
if(this.Control.style.cssText==this.Owner.SelectedItemStyle){
this.Control.style.cssText="";
}
};
RadGridNamespace.RadGridTableRow.prototype.OnDblClick=function(e){
try{
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowDblClick",[this.Control.sectionRowIndex,e])){
return;
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.Owner.OnError);
}
};
RadGridNamespace.RadGridTableRow.prototype.CreateRowSelectorArea=function(e){
if((this.Owner.Owner.RowResizer)||(e.ctrlKey)){
return;
}
var _27b=null;
if(e.srcElement){
_27b=e.srcElement;
}else{
if(e.target){
_27b=e.target;
}
}
if(!_27b.tagName){
return;
}
if(_27b.tagName.toLowerCase()=="input"||_27b.tagName.toLowerCase()=="textarea"){
return;
}
if((!this.Owner.Owner.ClientSettings.Selecting.AllowRowSelect)||(!this.Owner.Owner.AllowMultiRowSelection)){
return;
}
var _27c=RadGridNamespace.GetCurrentElement(e);
if((!_27c)||(!RadGridNamespace.IsChildOf(_27c,this.Control))){
return;
}
if(!this.RowSelectorArea){
this.RowSelectorArea=document.createElement("span");
this.RowSelectorArea.style.backgroundColor="navy";
this.RowSelectorArea.style.border="indigo 1px solid";
this.RowSelectorArea.style.position="absolute";
this.RowSelectorArea.style.font="icon";
if(window.netscape&&!window.opera){
this.RowSelectorArea.style.MozOpacity=1/10;
}else{
if(window.opera||navigator.userAgent.indexOf("Safari")>-1){
this.RowSelectorArea.style.opacity=0.1;
}else{
this.RowSelectorArea.style.filter="alpha(opacity=10);";
}
}
if(this.Owner.Owner.GridDataDiv){
this.RowSelectorArea.style.top=RadGridNamespace.FindPosY(this.Control)-this.Owner.Owner.GridDataDiv.scrollTop+"px";
this.RowSelectorArea.style.left=RadGridNamespace.FindPosX(this.Control)-this.Owner.Owner.GridDataDiv.scrollLeft+"px";
if(parseInt(this.RowSelectorArea.style.left)<RadGridNamespace.FindPosX(this.Owner.Owner.Control)){
this.RowSelectorArea.style.left=RadGridNamespace.FindPosX(this.Owner.Owner.Control)+"px";
}
}else{
this.RowSelectorArea.style.top=RadGridNamespace.FindPosY(this.Control)+"px";
this.RowSelectorArea.style.left=RadGridNamespace.FindPosX(this.Control)+"px";
}
document.body.appendChild(this.RowSelectorArea);
this.FirstRow=this.Control;
RadGridNamespace.ClearDocumentEvents();
}
};
RadGridNamespace.RadGridTableRow.prototype.DestroyRowSelectorArea=function(e){
if(this.RowSelectorArea){
var _27e=this.RowSelectorArea.style.height;
document.body.removeChild(this.RowSelectorArea);
this.RowSelectorArea=null;
RadGridNamespace.RestoreDocumentEvents();
var _27f=RadGridNamespace.GetCurrentElement(e);
var _280;
if((!_27f)||(!RadGridNamespace.IsChildOf(_27f,this.Owner.Control))){
return;
}
var _281=RadGridNamespace.GetFirstParentByTagName(_27f,"td");
if((_27f.tagName.toLowerCase()=="td")||(_27f.tagName.toLowerCase()=="tr")||_281.tagName.toLowerCase()=="td"){
if(_27f.tagName.toLowerCase()=="td"){
_280=_27f.parentNode;
}else{
if(_281.tagName.toLowerCase()=="td"){
_280=_281.parentNode;
}else{
if(_27f.tagName.toLowerCase()=="tr"){
_280=_27f;
}
}
}
for(var i=this.FirstRow.rowIndex;i<_280.rowIndex+1;i++){
var _283=this.Owner.Owner.GetRowObjectByRealRow(this.Owner,this.Owner.Control.rows[i]);
if(_283){
if(_27e!=""){
if(!_283.Selected){
this.Owner.SelectRow(this.Owner.Control.rows[i],false);
}
}
}
}
}
}
};
RadGridNamespace.RadGridTableRow.prototype.ResizeRowSelectorArea=function(e){
if((this.RowSelectorArea)&&(this.RowSelectorArea.parentNode)){
var _285=RadGridNamespace.GetCurrentElement(e);
if((!_285)||(!RadGridNamespace.IsChildOf(_285,this.Owner.Control))){
return;
}
var _286=parseInt(this.RowSelectorArea.style.left);
if(this.Owner.Owner.GridDataDiv){
var _287=RadGridNamespace.GetEventPosX(e)-this.Owner.Owner.GridDataDiv.scrollLeft;
}else{
var _287=RadGridNamespace.GetEventPosX(e);
}
var _288=parseInt(this.RowSelectorArea.style.top);
if(this.Owner.Owner.GridDataDiv){
var _289=RadGridNamespace.GetEventPosY(e)-this.Owner.Owner.GridDataDiv.scrollTop;
}else{
var _289=RadGridNamespace.GetEventPosY(e);
}
if((_287-_286-5)>0){
this.RowSelectorArea.style.width=_287-_286-5+"px";
}
if((_289-_288-5)>0){
this.RowSelectorArea.style.height=_289-_288-5+"px";
}
if(this.RowSelectorArea.offsetWidth>this.Owner.Control.offsetWidth){
this.RowSelectorArea.style.width=this.Owner.Control.offsetWidth+"px";
}
var _28a=(RadGridNamespace.FindPosX(this.Owner.Control)+this.Owner.Control.offsetHeight)-parseInt(this.RowSelectorArea.style.top);
if(this.RowSelectorArea.offsetHeight>_28a){
if(_28a>0){
this.RowSelectorArea.style.height=_28a+"px";
}
}
}
};
RadGridNamespace.RadGridTableRow.prototype.OnMouseDown=function(e){
if(this.Owner.Owner.ClientSettings.Selecting.EnableDragToSelectRows&&this.Owner.Owner.AllowMultiRowSelection){
if(!this.Owner.Owner.RowResizer){
this.CreateRowSelectorArea(e);
}
}
};
RadGridNamespace.RadGridTableRow.prototype.OnMouseUp=function(e){
this.DestroyRowSelectorArea(e);
};
RadGridNamespace.RadGridTableRow.prototype.OnMouseMove=function(e){
this.ResizeRowSelectorArea(e);
};
RadGridNamespace.RadGridTableRow.prototype.OnMouseOver=function(e){
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowMouseOver",[this.Control.sectionRowIndex,e])){
return;
}
if(this.Owner.Owner.Skin!=""&&this.Owner.Owner.Skin!="None"){
RadGridNamespace.addClassName(this.Control,"GridRowOver_"+this.Owner.Owner.Skin);
}
};
RadGridNamespace.RadGridTableRow.prototype.OnMouseOut=function(e){
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowMouseOut",[this.Control.sectionRowIndex,e])){
return;
}
if(this.Owner.Owner.Skin!=""&&this.Owner.Owner.Skin!="None"){
RadGridNamespace.removeClassName(this.Control,"GridRowOver_"+this.Owner.Owner.Skin);
}
};
RadGridNamespace.RadGridGroupPanel=function(_290,_291){
this.Control=_290;
this.Owner=_291;
this.Items=new Array();
this.groupPanelItemCounter=0;
this.getGroupPanelItems(this.Control,0);
var _292=this;
};
RadGridNamespace.RadGridGroupPanel.prototype.Dispose=function(){
this.UnLoadHandler=null;
this.Control=null;
this.Owner=null;
this.DisposeItems();
for(var _293 in this){
this[_293]=null;
}
};
RadGridNamespace.RadGridGroupPanel.prototype.DisposeItems=function(){
if(this.Items!=null){
for(var i=0;i<this.Items.length;i++){
var item=this.Items[i];
item.Dispose();
}
}
};
RadGridNamespace.RadGridGroupPanel.prototype.groupPanelItemCounter=0;
RadGridNamespace.RadGridGroupPanel.prototype.getGroupPanelItems=function(_296){
for(var i=0;i<_296.rows.length;i++){
var _298=false;
var row=_296.rows[i];
for(var j=0;j<row.cells.length;j++){
var cell=row.cells[j];
if(cell.tagName.toLowerCase()=="th"){
var _29c;
if(this.Owner.GroupPanel.GroupPanelItems[this.groupPanelItemCounter]){
_29c=this.Owner.GroupPanel.GroupPanelItems[this.groupPanelItemCounter].HierarchicalIndex;
}
if(_29c){
this.Items[this.Items.length]=new RadGridNamespace.RadGridGroupPanelItem(cell,this,_29c);
_298=true;
this.groupPanelItemCounter++;
}
}
if((cell.firstChild)&&(cell.firstChild.tagName)){
if(cell.firstChild.tagName.toLowerCase()=="table"){
this.getGroupPanelItems(cell.firstChild);
}
}
}
}
};
RadGridNamespace.RadGridGroupPanel.prototype.IsItem=function(_29d){
for(var i=0;i<this.Items.length;i++){
if(this.Items[i].Control==_29d){
return this.Items[i];
}
}
return null;
};
RadGridNamespace.RadGridGroupPanelItem=function(_29f,_2a0,_2a1){
RadControlsNamespace.DomEventMixin.Initialize(this);
this.Control=_29f;
this.Owner=_2a0;
this.HierarchicalIndex=_2a1;
this.Control.style.cursor="move";
this.AttachDomEvent(this.Control,"mousedown","OnMouseDown");
};
RadGridNamespace.RadGridGroupPanelItem.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
for(var _2a2 in this){
this[_2a2]=null;
}
this.Control=null;
this.Owner=null;
};
RadGridNamespace.RadGridGroupPanelItem.prototype.OnMouseDown=function(e){
if(((window.netscape||window.opera||navigator.userAgent.indexOf("Safari")!=-1)&&(e.button==0))||(e.button==1)){
this.CreateDragDrop(e);
this.CreateReorderIndicators(this.Control);
this.AttachDomEvent(document,"mouseup","OnMouseUp");
this.AttachDomEvent(document,"mousemove","OnMouseMove");
}
};
RadGridNamespace.RadGridGroupPanelItem.prototype.OnMouseUp=function(e){
this.FireDropAction(e);
this.DestroyDragDrop(e);
this.DestroyReorderIndicators();
this.DetachDomEvent(document,"mouseup","OnMouseUp");
this.DetachDomEvent(document,"mousemove","OnMouseMove");
};
RadGridNamespace.RadGridGroupPanelItem.prototype.OnMouseMove=function(e){
this.MoveDragDrop(e);
};
RadGridNamespace.RadGridGroupPanelItem.prototype.FireDropAction=function(e){
var _2a7=RadGridNamespace.GetCurrentElement(e);
if(_2a7!=null){
if(!RadGridNamespace.IsChildOf(_2a7,this.Owner.Control)){
this.Owner.Owner.SavePostData("UnGroupByExpression",this.HierarchicalIndex);
eval(this.Owner.Owner.ClientSettings.PostBackReferences.PostBackUnGroupByExpression);
}else{
var item=this.Owner.IsItem(_2a7);
if((_2a7!=this.Control)&&(item!=null)&&(_2a7.parentNode==this.Control.parentNode)){
this.Owner.Owner.SavePostData("ReorderGroupByExpression",this.HierarchicalIndex,item.HierarchicalIndex);
eval(this.Owner.Owner.ClientSettings.PostBackReferences.PostBackReorderGroupByExpression);
}
if(window.netscape){
this.Control.style.MozOpacity=4/4;
}else{
this.Control.style.filter="alpha(opacity=100);";
}
}
}
};
RadGridNamespace.RadGridGroupPanelItem.prototype.CreateDragDrop=function(e){
this.MoveHeaderDiv=document.createElement("div");
var _2aa=document.createElement("table");
if(this.MoveHeaderDiv.mergeAttributes){
this.MoveHeaderDiv.mergeAttributes(this.Owner.Owner.Control);
}else{
RadGridNamespace.CopyAttributes(this.MoveHeaderDiv,this.Control);
}
if(_2aa.mergeAttributes){
_2aa.mergeAttributes(this.Owner.Control);
}else{
RadGridNamespace.CopyAttributes(_2aa,this.Owner.Control);
}
_2aa.style.margin="0px";
_2aa.style.height=this.Control.offsetHeight+"px";
_2aa.style.width=this.Control.offsetWidth+"px";
_2aa.style.border="0px";
_2aa.style.borderCollapse="collapse";
_2aa.style.padding="0px";
var _2ab=document.createElement("thead");
var tr=document.createElement("tr");
_2aa.appendChild(_2ab);
_2ab.appendChild(tr);
tr.appendChild(this.Control.cloneNode(true));
this.MoveHeaderDiv.appendChild(_2aa);
document.body.appendChild(this.MoveHeaderDiv);
this.MoveHeaderDiv.style.height=_2aa.style.height;
this.MoveHeaderDiv.style.width=_2aa.style.width;
this.MoveHeaderDiv.style.position="absolute";
RadGridNamespace.RadGrid.PositionDragElement(this.MoveHeaderDiv,e);
if(window.netscape){
this.MoveHeaderDiv.style.MozOpacity=3/4;
}else{
this.MoveHeaderDiv.style.filter="alpha(opacity=75);";
}
this.MoveHeaderDiv.style.cursor="move";
this.MoveHeaderDiv.style.display="none";
this.MoveHeaderDiv.onmousedown=null;
RadGridNamespace.ClearDocumentEvents();
};
RadGridNamespace.RadGridGroupPanelItem.prototype.DestroyDragDrop=function(e){
if(this.MoveHeaderDiv!=null){
var _2ae=this.MoveHeaderDiv.parentNode;
_2ae.removeChild(this.MoveHeaderDiv);
this.MoveHeaderDiv.onmouseup=null;
this.MoveHeaderDiv.onmousemove=null;
this.MoveHeaderDiv=null;
RadGridNamespace.RestoreDocumentEvents();
}
};
RadGridNamespace.RadGridGroupPanelItem.prototype.MoveDragDrop=function(e){
if(this.MoveHeaderDiv!=null){
if(window.netscape){
this.Control.style.MozOpacity=1/4;
}else{
this.Control.style.filter="alpha(opacity=25);";
}
this.MoveHeaderDiv.style.visibility="";
this.MoveHeaderDiv.style.display="";
RadGridNamespace.RadGrid.PositionDragElement(this.MoveHeaderDiv,e);
var _2b0=RadGridNamespace.GetCurrentElement(e);
if(_2b0!=null){
if(RadGridNamespace.IsChildOf(_2b0,this.Owner.Control)){
var item=this.Owner.IsItem(_2b0);
if((_2b0!=this.Control)&&(item!=null)&&(_2b0.parentNode==this.Control.parentNode)){
this.MoveReorderIndicators(e,_2b0);
}else{
this.ReorderIndicator1.style.visibility="hidden";
this.ReorderIndicator1.style.display="none";
this.ReorderIndicator1.style.position="absolute";
this.ReorderIndicator2.style.visibility=this.ReorderIndicator1.style.visibility;
this.ReorderIndicator2.style.display=this.ReorderIndicator1.style.display;
this.ReorderIndicator2.style.position=this.ReorderIndicator1.style.position;
}
}
}
}
};
RadGridNamespace.RadGridGroupPanelItem.prototype.CreateReorderIndicators=function(_2b2){
if((this.ReorderIndicator1==null)&&(this.ReorderIndicator2==null)){
this.ReorderIndicator1=document.createElement("span");
this.ReorderIndicator2=document.createElement("span");
if(this.Owner.Owner.Skin==""||this.Owner.Owner.Skin=="None"){
this.ReorderIndicator1.innerHTML="&darr;";
this.ReorderIndicator2.innerHTML="&uarr;";
}else{
this.ReorderIndicator1.className="TopReorderIndicator_"+this.Owner.Owner.Skin;
this.ReorderIndicator2.className="BottomReorderIndicator_"+this.Owner.Owner.Skin;
this.ReorderIndicator1.style.width=this.ReorderIndicator1.style.height=this.ReorderIndicator2.style.width=this.ReorderIndicator2.style.height="10px";
}
this.ReorderIndicator1.style.backgroundColor="transparent";
this.ReorderIndicator1.style.color="darkblue";
this.ReorderIndicator1.style.font="bold 18px Arial";
this.ReorderIndicator2.style.backgroundColor=this.ReorderIndicator1.style.backgroundColor;
this.ReorderIndicator2.style.color=this.ReorderIndicator1.style.color;
this.ReorderIndicator2.style.font=this.ReorderIndicator1.style.font;
this.ReorderIndicator1.style.top=RadGridNamespace.FindPosY(_2b2)-this.ReorderIndicator1.offsetHeight+"px";
this.ReorderIndicator1.style.left=RadGridNamespace.FindPosX(_2b2)+"px";
this.ReorderIndicator2.style.top=RadGridNamespace.FindPosY(_2b2)+_2b2.offsetHeight+"px";
this.ReorderIndicator2.style.left=this.ReorderIndicator1.style.left;
this.ReorderIndicator1.style.visibility="hidden";
this.ReorderIndicator1.style.display="none";
this.ReorderIndicator1.style.position="absolute";
this.ReorderIndicator2.style.visibility=this.ReorderIndicator1.style.visibility;
this.ReorderIndicator2.style.display=this.ReorderIndicator1.style.display;
this.ReorderIndicator2.style.position=this.ReorderIndicator1.style.position;
document.body.appendChild(this.ReorderIndicator1);
document.body.appendChild(this.ReorderIndicator2);
}
};
RadGridNamespace.RadGridGroupPanelItem.prototype.DestroyReorderIndicators=function(){
if((this.ReorderIndicator1!=null)&&(this.ReorderIndicator2!=null)){
document.body.removeChild(this.ReorderIndicator1);
document.body.removeChild(this.ReorderIndicator2);
this.ReorderIndicator1=null;
this.ReorderIndicator2=null;
}
};
RadGridNamespace.RadGridGroupPanelItem.prototype.MoveReorderIndicators=function(e,_2b4){
if((this.ReorderIndicator1!=null)&&(this.ReorderIndicator2!=null)){
this.ReorderIndicator1.style.visibility="visible";
this.ReorderIndicator1.style.display="";
this.ReorderIndicator2.style.visibility="visible";
this.ReorderIndicator2.style.display="";
this.ReorderIndicator1.style.top=RadGridNamespace.FindPosY(_2b4)-this.ReorderIndicator1.offsetHeight+"px";
this.ReorderIndicator1.style.left=RadGridNamespace.FindPosX(_2b4)+"px";
this.ReorderIndicator2.style.top=RadGridNamespace.FindPosY(_2b4)+_2b4.offsetHeight+"px";
this.ReorderIndicator2.style.left=this.ReorderIndicator1.style.left;
}
};
RadGridNamespace.RadGridMenu=function(_2b5,_2b6,_2b7){
if(!_2b5||!_2b6){
return;
}
RadControlsNamespace.DomEventMixin.Initialize(this);
for(var _2b8 in _2b5){
this[_2b8]=_2b5[_2b8];
}
this.Owner=_2b6;
this.ItemData=_2b5.Items;
this.Items=[];
};
RadGridNamespace.RadGridMenu.prototype.Initialize=function(){
if(this.Control!=null){
return;
}
this.Control=document.createElement("table");
this.Control.style.backgroundColor=this.SelectColumnBackColor;
this.Control.style.border="outset 1px";
this.Control.style.fontSize="small";
this.Control.style.textAlign="left";
this.Control.cellPadding="0";
this.Control.style.borderCollapse="collapse";
this.Control.style.zIndex=998;
this.Skin=(this.Owner&&this.Owner.Owner&&this.Owner.Owner.Skin)||"None";
var _2b9=RadGridNamespace.IsRightToLeft(this.Owner.Control);
if(_2b9){
this.Control.style.direction="rtl";
RadGridNamespace.addClassName(this.Control,"RadGridRTL_"+this.Skin);
}
RadGridNamespace.addClassName(this.Control,"GridFilterMenu_"+this.Skin);
RadGridNamespace.addClassName(this.Control,this.CssClass);
this.Items=this.CreateItems(this.ItemData);
this.Control.style.position="absolute";
this.Control.style.display="none";
document.body.appendChild(this.Control);
var _2ba=document.createElement("img");
_2ba.src=this.SelectedImageUrl;
_2ba.src=this.NotSelectedImageUrl;
this.Control.style.zIndex=100000;
};
RadGridNamespace.RadGridMenu.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
this.DisposeItems();
this.ItemData=null;
this.Owner=null;
this.Control=null;
};
RadGridNamespace.RadGridMenu.prototype.CreateItems=function(_2bb){
var _2bc=[];
for(var i=0;i<_2bb.length;i++){
_2bc[_2bc.length]=new RadGridNamespace.RadGridMenuItem(_2bb[i],this);
}
return _2bc;
};
RadGridNamespace.RadGridMenu.prototype.DisposeItems=function(){
for(var i=0;i<this.Items.length;i++){
var item=this.Items[i];
item.Dispose();
}
this.Items=null;
};
RadGridNamespace.RadGridMenu.prototype.HideItem=function(_2c0){
for(var i=0;i<this.Items.length;i++){
if(this.Items[i].Value==_2c0){
this.Items[i].Control.style.display="none";
}
}
};
RadGridNamespace.RadGridMenu.prototype.ShowItem=function(_2c2){
for(var i=0;i<this.Items.length;i++){
if(this.Items[i].Value==_2c2){
this.Items[i].Control.style.display="";
}
}
};
RadGridNamespace.RadGridMenu.prototype.SelectItem=function(_2c4){
for(var i=0;i<this.Items.length;i++){
if(this.Items[i].Value==_2c4){
this.Items[i].Selected=true;
this.Items[i].SelectImage.src=this.SelectedImageUrl;
}else{
this.Items[i].Selected=false;
this.Items[i].SelectImage.src=this.NotSelectedImageUrl;
}
}
};
RadGridNamespace.RadGridMenu.prototype.Show=function(_2c6,_2c7,e){
this.Initialize();
this.Control.style.display="";
this.Control.style.top=e.clientY+document.documentElement.scrollTop+document.body.scrollTop+5+"px";
this.Control.style.left=e.clientX+document.documentElement.scrollLeft+document.body.scrollLeft+5+"px";
this.AttachHideEvents();
};
RadGridNamespace.RadGridMenu.prototype.OnKeyPress=function(e){
if(e.keyCode==27){
this.DetachHideEvents();
this.Hide();
}
};
RadGridNamespace.RadGridMenu.prototype.OnClick=function(e){
if(!e.cancelBubble){
this.DetachHideEvents();
this.Hide();
}
};
RadGridNamespace.RadGridMenu.prototype.AttachHideEvents=function(){
this.AttachDomEvent(document,"keypress","OnKeyPress");
this.AttachDomEvent(document,"click","OnClick");
};
RadGridNamespace.RadGridMenu.prototype.DetachHideEvents=function(){
this.DetachDomEvent(document,"keypress","OnKeyPress");
this.DetachDomEvent(document,"click","OnClick");
};
RadGridNamespace.RadGridMenu.prototype.Hide=function(){
if(this.Control.style.display==""){
this.Control.style.display="none";
}
};
RadGridNamespace.RadGridMenuItem=function(_2cb,_2cc){
for(var _2cd in _2cb){
this[_2cd]=_2cb[_2cd];
}
this.Owner=_2cc;
this.Skin=this.Owner.Skin;
this.Control=this.Owner.Control.insertRow(-1);
this.Control.insertCell(-1);
var _2ce=document.createElement("table");
_2ce.style.width="100%";
_2ce.cellPadding="0";
_2ce.cellSpacing="0";
_2ce.insertRow(-1);
var td1=_2ce.rows[0].insertCell(-1);
var td2=_2ce.rows[0].insertCell(-1);
if(this.Skin=="None"){
td1.style.borderTop="solid 1px "+this.Owner.SelectColumnBackColor;
td1.style.borderLeft="solid 1px "+this.Owner.SelectColumnBackColor;
td1.style.borderRight="none 0px";
td1.style.borderBottom="solid 1px "+this.Owner.SelectColumnBackColor;
td1.style.padding="2px";
td1.style.textAlign="center";
}else{
RadGridNamespace.addClassName(td1,"GridFilterMenuSelectColumn_"+this.Skin);
}
td1.style.width="16px";
td1.appendChild(document.createElement("img"));
td1.childNodes[0].src=this.Owner.NotSelectedImageUrl;
this.SelectImage=td1.childNodes[0];
if(this.Skin=="None"){
td2.style.borderTop="solid 1px "+this.Owner.TextColumnBackColor;
td2.style.borderLeft="none 0px";
td2.style.borderRight="solid 1px "+this.Owner.TextColumnBackColor;
td2.style.borderBottom="solid 1px "+this.Owner.TextColumnBackColor;
td2.style.padding="2px";
td2.style.backgroundColor=this.Owner.TextColumnBackColor;
td2.style.cursor="pointer";
}else{
RadGridNamespace.addClassName(td2,"GridFilterMenuTextColumn_"+this.Skin);
}
td2.innerHTML=this.Text;
this.Control.cells[0].appendChild(_2ce);
var _2d1=this;
this.Control.onclick=function(){
if(_2d1.Owner.Owner.Owner.EnableAJAX){
if(_2d1.Owner.Owner==_2d1.Owner.Owner.Owner.MasterTableViewHeader){
RadGridNamespace.AsyncRequest(_2d1.UID,_2d1.Owner.Owner.Owner.MasterTableView.UID+"!"+_2d1.Owner.Column.UniqueName,_2d1.Owner.Owner.Owner.ClientID);
}else{
RadGridNamespace.AsyncRequest(_2d1.UID,_2d1.Owner.Owner.UID+"!"+_2d1.Owner.Column.UniqueName,_2d1.Owner.Owner.Owner.ClientID);
}
}else{
var _2d2=_2d1.Owner.Owner.Owner.ClientSettings.PostBackFunction;
if(_2d1.Owner.Owner==_2d1.Owner.Owner.Owner.MasterTableViewHeader){
_2d2=_2d2.replace("{0}",_2d1.UID).replace("{1}",_2d1.Owner.Owner.Owner.MasterTableView.UID+"!"+_2d1.Owner.Column.UniqueName);
}else{
_2d2=_2d2.replace("{0}",_2d1.UID).replace("{1}",_2d1.Owner.Owner.UID+"!"+_2d1.Owner.Column.UniqueName);
}
eval(_2d2);
}
};
var _2d1=this;
this.Control.onmouseover=function(e){
if(_2d1.Skin=="None"){
this.cells[0].childNodes[0].rows[0].cells[0].style.backgroundColor=_2d1.Owner.HoverBackColor;
this.cells[0].childNodes[0].rows[0].cells[0].style.borderTop="solid 1px "+_2d1.Owner.HoverBorderColor;
this.cells[0].childNodes[0].rows[0].cells[0].style.borderLeft="solid 1px "+_2d1.Owner.HoverBorderColor;
this.cells[0].childNodes[0].rows[0].cells[0].style.borderBottom="solid 1px "+_2d1.Owner.HoverBorderColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.backgroundColor=_2d1.Owner.HoverBackColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.borderTop="solid 1px "+_2d1.Owner.HoverBorderColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.borderRight="solid 1px "+_2d1.Owner.HoverBorderColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.borderBottom="solid 1px "+_2d1.Owner.HoverBorderColor;
}else{
RadGridNamespace.addClassName(this.cells[0].childNodes[0].rows[0].cells[0],"GridFilterMenuHover_"+_2d1.Skin);
RadGridNamespace.addClassName(this.cells[0].childNodes[0].rows[0].cells[1],"GridFilterMenuHover_"+_2d1.Skin);
}
};
this.Control.onmouseout=function(e){
if(_2d1.Skin=="None"){
this.cells[0].childNodes[0].rows[0].cells[0].style.borderTop="solid 1px "+_2d1.Owner.SelectColumnBackColor;
this.cells[0].childNodes[0].rows[0].cells[0].style.borderLeft="solid 1px "+_2d1.Owner.SelectColumnBackColor;
this.cells[0].childNodes[0].rows[0].cells[0].style.borderBottom="solid 1px "+_2d1.Owner.SelectColumnBackColor;
this.cells[0].childNodes[0].rows[0].cells[0].style.backgroundColor="";
this.cells[0].childNodes[0].rows[0].cells[1].style.borderTop="solid 1px "+_2d1.Owner.TextColumnBackColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.borderRight="solid 1px "+_2d1.Owner.TextColumnBackColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.borderBottom="solid 1px "+_2d1.Owner.TextColumnBackColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.backgroundColor=_2d1.Owner.TextColumnBackColor;
}else{
RadGridNamespace.removeClassName(this.cells[0].childNodes[0].rows[0].cells[0],"GridFilterMenuHover_"+_2d1.Skin);
RadGridNamespace.removeClassName(this.cells[0].childNodes[0].rows[0].cells[1],"GridFilterMenuHover_"+_2d1.Skin);
}
};
};
RadGridNamespace.RadGridMenuItem.prototype.Dispose=function(){
this.Control.onclick=null;
this.Control.onmouseover=null;
this.Control.onmouseout=null;
var _2d5=this.Control.getElementsByTagName("table");
while(_2d5.length>0){
var _2d6=_2d5[0];
if(_2d6.parentNode!=null){
_2d6.parentNode.removeChild(_2d6);
}
}
this.Control=null;
this.Owner=null;
};
RadGridNamespace.RadGridFilterMenu=function(_2d7,_2d8){
RadGridNamespace.RadGridMenu.call(this,_2d7,_2d8);
};
RadGridNamespace.RadGridFilterMenu.prototype=new RadGridNamespace.RadGridMenu;
RadGridNamespace.RadGridFilterMenu.prototype.Show=function(_2d9,e){
this.Initialize();
if(!_2d9){
return;
}
this.Owner=_2d9.Owner;
this.Column=_2d9;
for(var i=0;i<this.Items.length;i++){
if(_2d9.DataTypeName=="System.Boolean"){
if((this.Items[i].Value=="GreaterThan")||(this.Items[i].Value=="LessThan")||(this.Items[i].Value=="GreaterThanOrEqualTo")||(this.Items[i].Value=="LessThanOrEqualTo")||(this.Items[i].Value=="Between")||(this.Items[i].Value=="NotBetween")){
this.Items[i].Control.style.display="none";
continue;
}
}
if(_2d9.DataTypeName!="System.String"){
if((this.Items[i].Value=="StartsWith")||(this.Items[i].Value=="EndsWith")||(this.Items[i].Value=="Contains")||(this.Items[i].Value=="DoesNotContain")||(this.Items[i].Value=="IsEmpty")||(this.Items[i].Value=="NotIsEmpty")){
this.Items[i].Control.style.display="none";
continue;
}
}
if(_2d9.FilterListOptions=="VaryByDataType"){
if(this.Items[i].Value=="Custom"){
this.Items[i].Control.style.display="none";
continue;
}
}
this.Items[i].Control.style.display="";
}
this.SelectItem(_2d9.CurrentFilterFunction);
var args={Menu:this,TableView:this.Owner,Column:this.Column,Event:e};
if(!RadGridNamespace.FireEvent(this.Owner,"OnFilterMenuShowing",[this.Owner,args])){
return;
}
this.Control.style.display="";
this.Control.style.top=e.clientY+document.documentElement.scrollTop+document.body.scrollTop+5+"px";
this.Control.style.left=e.clientX+document.documentElement.scrollLeft+document.body.scrollLeft+5+"px";
this.AttachHideEvents();
};
RadGridNamespace.RadGrid.prototype.InitializeFilterMenu=function(_2dd){
if(this.AllowFilteringByColumn||_2dd.AllowFilteringByColumn){
if(!_2dd||!_2dd.Control){
return;
}
if(!_2dd.Control.tHead){
return;
}
if(!_2dd.IsItemInserted){
var _2de=_2dd.Control.tHead.rows[_2dd.Control.tHead.rows.length-1];
}else{
var _2de=_2dd.Control.tHead.rows[_2dd.Control.tHead.rows.length-2];
}
if(!_2de){
return;
}
var _2df=_2de.getElementsByTagName("img");
var _2e0=this;
if(!_2dd.Columns){
return;
}
if(!_2dd.Columns[0]){
return;
}
var _2e1=_2dd.Columns[0].FilterImageUrl;
for(var i=0;i<_2df.length;i++){
var _2e3=RadGridNamespace.EncodeURI(_2e1);
if(_2df[i].getAttribute("src").indexOf(_2e3)==-1){
continue;
}
_2df[i].onclick=function(e){
if(!e){
var e=window.event;
}
e.cancelBubble=true;
var _2e5=this.parentNode.cellIndex;
if(window.attachEvent&&!window.opera&&!window.netscape){
_2e5=RadGridNamespace.GetRealCellIndexFormCells(this.parentNode.parentNode.cells,this.parentNode);
}
_2e0.FilteringMenu.Show(_2dd.Columns[_2e5],e);
if(e.preventDefault){
e.preventDefault();
}else{
e.returnValue=false;
return false;
}
};
}
this.FilteringMenu=new RadGridNamespace.RadGridFilterMenu(this.FilterMenu,_2dd);
}
};
RadGridNamespace.RadGrid.prototype.DisposeFilterMenu=function(_2e6){
if(this.FilteringMenu!=null){
this.FilteringMenu.Dispose();
this.FilteringMenu=null;
}
};
RadGridNamespace.GetRealCellIndexFormCells=function(_2e7,cell){
for(var i=0;i<_2e7.length;i++){
if(_2e7[i]==cell){
return i;
}
}
};
if(typeof (window.RadGridNamespace)=="undefined"){
window.RadGridNamespace=new Object();
}
RadGridNamespace.Slider=function(_2ea){
RadControlsNamespace.DomEventMixin.Initialize(this);
if(!document.readyState||document.readyState=="complete"||window.opera){
this._constructor(_2ea);
}else{
this.objectData=_2ea;
this.AttachDomEvent(window,"load","OnWindowLoad");
}
};
RadGridNamespace.Slider.prototype.OnWindowLoad=function(e){
this.DetachDomEvent(window,"load","OnWindowLoad");
this._constructor(this.objectData);
this.objectData=null;
};
RadGridNamespace.Slider.prototype._constructor=function(_2ec){
var _2ed=this;
for(var _2ee in _2ec){
this[_2ee]=_2ec[_2ee];
}
this.Owner=window[this.OwnerID];
this.OwnerGrid=window[this.OwnerGridID];
this.Control=document.getElementById(this.ClientID);
if(this.Control==null){
return;
}
this.Control.unselectable="on";
this.Control.parentNode.style.padding="10px";
this.ToolTip=document.createElement("div");
this.ToolTip.unselectable="on";
this.ToolTip.style.backgroundColor="#F5F5DC";
this.ToolTip.style.border="1px outset";
this.ToolTip.style.font="icon";
this.ToolTip.style.padding="2px";
this.ToolTip.style.marginTop="5px";
this.ToolTip.style.marginBottom="15px";
this.Control.appendChild(this.ToolTip);
this.Line=document.createElement("hr");
this.Line.unselectable="on";
this.Line.style.width="100%";
this.Line.style.height="2px";
this.Line.style.backgroundColor="buttonface";
this.Line.style.border="1px outset threedshadow";
this.Control.appendChild(this.Line);
this.Thumb=document.createElement("div");
this.Thumb.unselectable="on";
this.Thumb.style.position="relative";
this.Thumb.style.width="8px";
this.Thumb.style.marginTop="-15px";
this.Thumb.style.height="16px";
this.Thumb.style.backgroundColor="buttonface";
this.Thumb.style.border="1px outset threedshadow";
this.Control.appendChild(this.Thumb);
this.Link=document.createElement("a");
this.Link.unselectable="on";
this.Link.style.width="100%";
this.Link.style.height="100%";
this.Link.style.display="block";
this.Link.href="javascript:void(0);";
this.Thumb.appendChild(this.Link);
this.LineX=RadGridNamespace.FindPosX(this.Line);
this.AttachDomEvent(this.Control,"mousedown","OnMouseDown");
this.AttachDomEvent(this.Link,"keydown","OnKeyDown");
var _2ef=this.OwnerGrid.CurrentPageIndex/this.OwnerGrid.MasterTableView.PageCount;
this.SetPosition(_2ef*this.Line.offsetWidth);
var _2f0=parseInt(this.Thumb.style.left)/this.Line.offsetWidth;
var _2f1=Math.round((this.OwnerGrid.MasterTableView.PageCount-1)*_2f0);
this.OwnerGrid.ApplyPagerTooltipText(this.ToolTip,this.OwnerGrid.CurrentPageIndex,this.OwnerGrid.MasterTableView.PageCount);
};
RadGridNamespace.Slider.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
for(var _2f2 in this){
this[_2f2]=null;
}
this.Control=null;
this.Line=null;
this.Thumb=null;
this.ToolTip=null;
};
RadGridNamespace.Slider.prototype.OnKeyDown=function(e){
this.AttachDomEvent(this.Link,"keyup","OnKeyUp");
if(e.keyCode==39){
this.SetPosition(parseInt(this.Thumb.style.left)+this.Thumb.offsetWidth);
}
if(e.keyCode==37){
this.SetPosition(parseInt(this.Thumb.style.left)-this.Thumb.offsetWidth);
}
if(e.keyCode==39||e.keyCode==37){
var _2f4=parseInt(this.Thumb.style.left)/this.Line.offsetWidth;
var _2f5=Math.round((this.OwnerGrid.MasterTableView.PageCount-1)*_2f4);
this.OwnerGrid.ApplyPagerTooltipText(this.ToolTip,_2f5,this.OwnerGrid.MasterTableView.PageCount);
}
};
RadGridNamespace.Slider.prototype.OnKeyUp=function(e){
this.DetachDomEvent(this.Link,"keyup","OnKeyUp");
if(e.keyCode==39||e.keyCode==37){
var _2f7=this;
setTimeout(function(){
_2f7.ChangePage();
},100);
}
};
RadGridNamespace.Slider.prototype.OnMouseDown=function(e){
this.DetachDomEvent(this.Control,"mousedown","OnMouseDown");
if(((window.netscape||window.opera||navigator.userAgent.indexOf("Safari")!=-1))&&(e.button==0)||(e.button==1)){
this.SetPosition(RadGridNamespace.GetEventPosX(e)-this.LineX);
this.AttachDomEvent(document,"mousemove","OnMouseMove");
this.AttachDomEvent(document,"mouseup","OnMouseUp");
}
};
RadGridNamespace.Slider.prototype.OnMouseUp=function(e){
this.DetachDomEvent(document,"mousemove","OnMouseMove");
this.DetachDomEvent(document,"mouseup","OnMouseUp");
var _2fa=parseInt(this.Thumb.style.left)/this.Line.offsetWidth;
var _2fb=Math.round((this.OwnerGrid.MasterTableView.PageCount-1)*_2fa);
this.OwnerGrid.ApplyPagerTooltipText(this.ToolTip,_2fb,this.OwnerGrid.MasterTableView.PageCount);
var _2fc=this;
setTimeout(function(){
_2fc.ChangePage();
},100);
};
RadGridNamespace.Slider.prototype.OnMouseMove=function(e){
this.SetPosition(RadGridNamespace.GetEventPosX(e)-this.LineX);
var _2fe=parseInt(this.Thumb.style.left)/this.Line.offsetWidth;
var _2ff=Math.round((this.OwnerGrid.MasterTableView.PageCount-1)*_2fe);
this.OwnerGrid.ApplyPagerTooltipText(this.ToolTip,_2ff,this.OwnerGrid.MasterTableView.PageCount);
};
RadGridNamespace.Slider.prototype.GetPosition=function(e){
this.SetPosition(RadGridNamespace.GetEventPosX(e)-this.LineX);
};
RadGridNamespace.Slider.prototype.SetPosition=function(_301){
if(_301>=0&&_301<=this.Line.offsetWidth){
this.Thumb.style.left=_301+"px";
}
};
RadGridNamespace.Slider.prototype.ChangePage=function(){
var _302=parseInt(this.Thumb.style.left)/this.Line.offsetWidth;
var _303=Math.round((this.OwnerGrid.MasterTableView.PageCount-1)*_302);
if(this.OwnerGrid.CurrentPageIndex==_303){
this.AttachDomEvent(this.Control,"mousedown","OnMouseDown");
return;
}
this.OwnerGrid.SavePostData("AJAXScrolledControl",(this.OwnerGrid.GridDataDiv)?this.OwnerGrid.GridDataDiv.scrollLeft:"",(this.OwnerGrid.GridDataDiv)?this.OwnerGrid.LastScrollTop:"",(this.OwnerGrid.GridDataDiv)?this.OwnerGrid.GridDataDiv.scrollTop:"",_303);
var _304=this.OwnerGrid.ClientSettings.PostBackFunction;
_304=_304.replace("{0}",this.OwnerGrid.UniqueID);
eval(_304);
};

//BEGIN_ATLAS_NOTIFY
if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
