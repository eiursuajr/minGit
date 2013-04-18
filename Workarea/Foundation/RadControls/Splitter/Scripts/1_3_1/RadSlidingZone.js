if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
if(typeof (window.RadControlsNamespace.Ease)=="undefined"||typeof (window.RadControlsNamespace.Ease.Version)==null||window.RadControlsNamespace.Ease.Version<1.3){
RadControlsNamespace.Ease=function(_1,_2,_3,_4,_5,_6){
this.Element=_1;
if(_6){
this.Overlay=new RadControlsNamespace.Overlay(_1);
}
this.OffsetX=_3;
this.OffsetY=_4;
this.Invert=false;
var _7=this.Element.parentNode;
this.ExpandConfig=this.MergeConfig(_2.ExpandAnimation);
this.CollapseConfig=this.MergeConfig(_2.CollapseAnimation);
this.Ticker=new RadControlsNamespace.Ticker(this);
this.Listener=_5;
this.SlideParent=false;
};
RadControlsNamespace.Ease.Version=1.3;
RadControlsNamespace.Ease.Coef=0;
RadControlsNamespace.Ease.prototype={SetSide:function(_8){
this.InitialSide=_8.charAt(0).toUpperCase()+_8.substr(1,_8.length-1);
this.Invert=false;
if(_8=="right"){
_8="left";
this.Invert=true;
}
if(_8=="bottom"){
_8="top";
this.Invert=true;
}
this.Side=_8;
this.Horizontal=_8=="left";
},MergeConfig:function(_9){
if(!_9.Type){
_9.Type="OutQuint";
}
if(!_9.Duration){
_9.Duration=200;
}
return _9;
},GetSide:function(){
return this.InitialSide;
},ShowElements:function(){
if(!this.Element.parentNode){
return;
}
if(!this.Element.parentNode.style){
return;
}
this.Element.parentNode.style.display="block";
this.Element.style.display="block";
this.Element.parentNode.style.overflow="hidden";
},Dispose:function(){
this.Ticker.Stop();
this.Element=null;
if(this.Overlay){
this.Overlay.Dispose();
}
},ResetState:function(_a){
this.ShowElements();
if(_a){
var _b=(this.Horizontal?this.Element.offsetWidth:this.Element.offsetHeight);
if(!this.Invert){
_b=-_b;
}
this.SetPosition(_b);
}
this.InitialPosition=this.GetPosition();
},UpdateContainerSize:function(){
if(!this.Element.parentNode){
return;
}
if(!this.Element.offsetWidth||!this.Element.offsetHeight){
return;
}
if(this.Invert){
if(this.Side=="left"){
this.Element.parentNode.style.height=this.Element.offsetHeight+"px";
}else{
if(this.Side=="top"){
this.Element.parentNode.style.width=this.Element.offsetWidth+"px";
}
}
return;
}
var _c=0;
var _d=0;
if(this.Element.style.top!=""){
_c=Math.max(parseInt(this.Element.style.top),0);
}
if(this.Element.style.left!=""){
_d=Math.max(parseInt(this.Element.style.left),0);
}
if(this.SlideParent){
_c=parseInt(this.Element.style.top);
if(isNaN(_c)){
_c=0;
}
}
if(typeof (RadMenuItem)!="undefined"&&this.Listener instanceof RadMenuItem){
if(this.Element.parentNode.style.height!=this.Element.offsetHeight+_c+"px"){
this.Element.parentNode.style.height=Math.max(this.Element.offsetHeight+_c,0)+"px";
}
if(this.Element.parentNode.style.width!=(this.Element.offsetWidth+_d)+"px"){
this.Element.parentNode.style.width=Math.max(this.Element.offsetWidth+_d,0)+"px";
}
}else{
if(this.Element.parentNode.offsetHeight!=this.Element.offsetHeight+_c){
this.Element.parentNode.style.height=Math.max(this.Element.offsetHeight+_c,0)+"px";
}
if(this.Element.parentNode.offsetWidth!=(this.Element.offsetWidth+_d)){
this.Element.parentNode.style.width=Math.max(this.Element.offsetWidth+_d,0)+"px";
}
}
},GetSize:function(){
return this.Horizontal?this.Element.offsetWidth:this.Element.offsetHeight;
},GetPosition:function(){
if(!this.Element.style[this.Side]){
return 0;
}
return parseInt(this.Element.style[this.Side]);
},SetPosition:function(_e){
this.Element.style[this.Side]=_e+"px";
},Out:function(){
this.ResetState();
this.Direction=-1;
if(this.Invert){
this.Delta=this.GetSize()-this.GetPosition();
}else{
this.Delta=this.GetPosition()-this.GetSize();
}
this.Start(this.CollapseConfig);
},In:function(){
this.ResetState(true);
this.Direction=1;
this.Delta=-this.GetPosition();
this.Start(this.ExpandConfig);
},Start:function(_f){
if(_f.Type=="None"){
this.UpdateContainerSize();
this.Ticker.Stop();
this.ChangePosition(this.InitialPosition+this.Delta);
if(this.Overlay){
this.Overlay.Update();
}
this.UpdateContainerSize();
this.OnTickEnd();
return;
}
this.Tween=_f.Type;
this.Ticker.Configure(_f);
this.Ticker.Start();
this.UpdateContainerSize();
},ChangePosition:function(_10){
if(isNaN(_10)){
return;
}
var _11,_12,_13;
if(this.Invert){
if(this.Horizontal){
_11=this.Element.offsetWidth;
_12="width";
_13=this.OffsetX;
}else{
_11=this.Element.offsetHeight;
_12="height";
_13=this.OffsetY;
}
this.SetPosition(0);
var _14=Math.max(1,_11-_10)+"px";
this.Element.parentNode.style[_12]=_14;
this.Element.parentNode.style[this.Side]=((_11-_10+_13)*-1)+"px";
}else{
this.Element.style[this.Side]=_10+"px";
}
if(this.Listener&&this.Listener.OnEase){
this.Listener.OnEase(_10);
}
},OnTick:function(_15){
var _16=Math.round(Penner[this.Tween](_15,this.InitialPosition,this.Delta,this.Ticker.Duration));
if(_16==this.InitialPosition+this.Delta){
this.Ticker.Stop();
return;
}
this.ChangePosition(_16);
this.UpdateContainerSize();
if(this.Overlay){
this.Overlay.Update();
}
},OnTickEnd:function(){
try{
if(this.Direction==0){
return;
}
this.ChangePosition(this.InitialPosition+this.Delta);
if(this.Overlay){
this.Overlay.Update();
}
if(this.Direction>0){
this.Element.parentNode.style.overflow="visible";
if(this.Listener&&this.Listener.OnExpandComplete){
this.Listener.OnExpandComplete();
}
}else{
this.Element.parentNode.style.display="none";
if(this.Listener){
this.Listener.OnCollapseComplete();
}
}
this.Direction=0;
}
catch(e){
}
}};
};var Penner={};
Penner.Linear=function(t,b,c,d){
return c*t/d+b;
};
Penner.InQuad=function(t,b,c,d){
return c*(t/=d)*t+b;
};
Penner.OutQuad=function(t,b,c,d){
return -c*(t/=d)*(t-2)+b;
};
Penner.InOutQuad=function(t,b,c,d){
if((t/=d/2)<1){
return c/2*t*t+b;
}
return -c/2*((--t)*(t-2)-1)+b;
};
Penner.InCubic=function(t,b,c,d){
return c*(t/=d)*t*t+b;
};
Penner.OutCubic=function(t,b,c,d){
return c*((t=t/d-1)*t*t+1)+b;
};
Penner.InOutCubic=function(t,b,c,d){
if((t/=d/2)<1){
return c/2*t*t*t+b;
}
return c/2*((t-=2)*t*t+2)+b;
};
Penner.InQuart=function(t,b,c,d){
return c*(t/=d)*t*t*t+b;
};
Penner.OutQuart=function(t,b,c,d){
return -c*((t=t/d-1)*t*t*t-1)+b;
};
Penner.InOutQuart=function(t,b,c,d){
if((t/=d/2)<1){
return c/2*t*t*t*t+b;
}
return -c/2*((t-=2)*t*t*t-2)+b;
};
Penner.InQuint=function(t,b,c,d){
return c*(t/=d)*t*t*t*t+b;
};
Penner.OutQuint=function(t,b,c,d){
return c*((t=t/d-1)*t*t*t*t+1)+b;
};
Penner.InOutQuint=function(t,b,c,d){
if((t/=d/2)<1){
return c/2*t*t*t*t*t+b;
}
return c/2*((t-=2)*t*t*t*t+2)+b;
};
Penner.InSine=function(t,b,c,d){
return -c*Math.cos(t/d*(Math.PI/2))+c+b;
};
Penner.OutSine=function(t,b,c,d){
return c*Math.sin(t/d*(Math.PI/2))+b;
};
Penner.InOutSine=function(t,b,c,d){
return -c/2*(Math.cos(Math.PI*t/d)-1)+b;
};
Penner.InExpo=function(t,b,c,d){
return (t==0)?b:c*Math.pow(2,10*(t/d-1))+b;
};
Penner.OutExpo=function(t,b,c,d){
return (t==d)?b+c:c*(-Math.pow(2,-10*t/d)+1)+b;
};
Penner.InOutExpo=function(t,b,c,d){
if(t==0){
return b;
}
if(t==d){
return b+c;
}
if((t/=d/2)<1){
return c/2*Math.pow(2,10*(t-1))+b;
}
return c/2*(-Math.pow(2,-10*--t)+2)+b;
};
Penner.InCirc=function(t,b,c,d){
return -c*(Math.sqrt(1-(t/=d)*t)-1)+b;
};
Penner.OutCirc=function(t,b,c,d){
return c*Math.sqrt(1-(t=t/d-1)*t)+b;
};
Penner.InOutCirc=function(t,b,c,d){
if((t/=d/2)<1){
return -c/2*(Math.sqrt(1-t*t)-1)+b;
}
return c/2*(Math.sqrt(1-(t-=2)*t)+1)+b;
};
Penner.InElastic=function(t,b,c,d,a,p){
if(t==0){
return b;
}
if((t/=d)==1){
return b+c;
}
if(!p){
p=d*0.3;
}
if((!a)||a<Math.abs(c)){
a=c;
var s=p/4;
}else{
var s=p/(2*Math.PI)*Math.asin(c/a);
}
return -(a*Math.pow(2,10*(t-=1))*Math.sin((t*d-s)*(2*Math.PI)/p))+b;
};
Penner.OutElastic=function(t,b,c,d,a,p){
if(t==0){
return b;
}
if((t/=d)==1){
return b+c;
}
if(!p){
p=d*0.3;
}
if((!a)||a<Math.abs(c)){
a=c;
var s=p/4;
}else{
var s=p/(2*Math.PI)*Math.asin(c/a);
}
return a*Math.pow(2,-10*t)*Math.sin((t*d-s)*(2*Math.PI)/p)+c+b;
};
Penner.InOutElastic=function(t,b,c,d,a,p){
if(t==0){
return b;
}
if((t/=d/2)==2){
return b+c;
}
if(!p){
p=d*(0.3*1.5);
}
if((!a)||a<Math.abs(c)){
a=c;
var s=p/4;
}else{
var s=p/(2*Math.PI)*Math.asin(c/a);
}
if(t<1){
return -0.5*(a*Math.pow(2,10*(t-=1))*Math.sin((t*d-s)*(2*Math.PI)/p))+b;
}
return a*Math.pow(2,-10*(t-=1))*Math.sin((t*d-s)*(2*Math.PI)/p)*0.5+c+b;
};
Penner.InBack=function(t,b,c,d,s){
if(s==undefined){
s=1.70158;
}
return c*(t/=d)*t*((s+1)*t-s)+b;
};
Penner.OutBack=function(t,b,c,d,s){
if(s==undefined){
s=1.70158;
}
return c*((t=t/d-1)*t*((s+1)*t+s)+1)+b;
};
Penner.InOutBack=function(t,b,c,d,s){
if(s==undefined){
s=1.70158;
}
if((t/=d/2)<1){
return c/2*(t*t*(((s*=(1.525))+1)*t-s))+b;
}
return c/2*((t-=2)*t*(((s*=(1.525))+1)*t+s)+2)+b;
};
Penner.InBounce=function(t,b,c,d){
return c-Penner.OutBounce(d-t,0,c,d)+b;
};
Penner.OutBounce=function(t,b,c,d){
if((t/=d)<(1/2.75)){
return c*(7.5625*t*t)+b;
}else{
if(t<(2/2.75)){
return c*(7.5625*(t-=(1.5/2.75))*t+0.75)+b;
}else{
if(t<(2.5/2.75)){
return c*(7.5625*(t-=(2.25/2.75))*t+0.9375)+b;
}else{
return c*(7.5625*(t-=(2.625/2.75))*t+0.984375)+b;
}
}
}
};
Penner.InOutBounce=function(t,b,c,d){
if(t<d/2){
return Penner.InBounce(t*2,0,c,d)*0.5+b;
}
return Penner.OutBounce(t*2-d,0,c,d)*0.5+c*0.5+b;
};;if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
if(typeof (window.RadControlsNamespace.Overlay)=="undefined"||typeof (window.RadControlsNamespace.Overlay.Version)==null||window.RadControlsNamespace.Overlay.Version<1.1){
window.RadControlsNamespace.Overlay=function(_1){
if(!this.SupportsOverlay()){
return;
}
this.Element=_1;
this.Shim=document.createElement("IFRAME");
this.Shim.src="javascript:'';";
this.Element.parentNode.insertBefore(this.Shim,this.Element);
if(_1.style.zIndex>0){
this.Shim.style.zIndex=_1.style.zIndex-1;
}
this.Shim.style.position="absolute";
this.Shim.style.border="0px";
this.Shim.frameBorder=0;
this.Shim.style.filter="progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=0)";
this.Shim.disabled="disabled";
};
window.RadControlsNamespace.Overlay.Version=1.1;
RadControlsNamespace.Overlay.prototype.SupportsOverlay=function(){
return (RadControlsNamespace.Browser.IsIE&&!RadControlsNamespace.Browser.IsIE7);
};
RadControlsNamespace.Overlay.prototype.Update=function(){
if(!this.SupportsOverlay()){
return;
}
this.Shim.style.top=this.ToUnit(this.Element.style.top);
this.Shim.style.left=this.ToUnit(this.Element.style.left);
this.Shim.style.width=this.Element.offsetWidth+"px";
this.Shim.style.height=this.Element.offsetHeight+"px";
};
RadControlsNamespace.Overlay.prototype.ToUnit=function(_2){
if(!_2){
return "0px";
}
return parseInt(_2)+"px";
};
RadControlsNamespace.Overlay.prototype.Dispose=function(){
if(!this.SupportsOverlay()){
return;
}
if(this.Shim.parentNode){
this.Shim.parentNode.removeChild(this.Shim);
}
this.Element=null;
this.Shim=null;
};
};function RadSplitter_SlidingPane(_1){
this.ID=_1["ID"];
this.Log=new RadControlsNamespace.Log(_1["logLevel"],true);
this.slidingZone=_1["slidingZone"];
this.Index=_1["index"];
this.title=_1["title"];
this.minWidth=_1["minWidth"];
this.maxWidth=_1["maxWidth"];
this.minHeight=_1["minHeight"];
this.maxHeight=_1["maxHeight"];
this.width=parseInt(_1["width"]);
this.height=parseInt(_1["height"]);
this.resizable=_1["enableResize"];
this.dockable=_1["enableDock"];
this.slideDuration=_1["slideDuration"];
this.stateFieldId=_1["stateFieldId"];
this.resizeStep=_1["resizeStep"];
this.resizeBarSize=4;
this.slideBorderSize=0;
this.supportedEvents=["OnClientBeforePaneDock","OnClientPaneDocked","OnClientBeforePaneResize","OnClientPaneResized","OnClientBeforePaneUnDock","OnClientPaneUnDocked","OnClientBeforePaneExpand","OnClientPaneExpanded","OnClientBeforePaneCollapse","OnClientPaneCollapsed"];
for(var i=0;i<this.supportedEvents.length;i++){
this[this.supportedEvents[i]]=_1[this.supportedEvents[i]];
}
if(!RadControlsNamespace.Browser.IsIE&&this.slidingZone.isHorizontalSlide()){
var _3=this.getTitleContainerElement();
if(_3!=null){
_3.style.lineHeight=1;
_3.innerHTML=this.title.split("").join("<br/>");
}
}
this.box=window.RadControlsNamespace.Box;
this.isExpanded=false;
this.isDocked=false;
this.SaveState();
}
RadSplitter_SlidingPane.prototype.Init=function(){
RadControlsNamespace.DomEventMixin.Initialize(this);
RadControlsNamespace.EventMixin.Initialize(this);
var _4=this.GetSlideContainer();
this.AttachDomEvent(_4,"mouseover","SlidingContainer_OnMouseOver");
this.AttachDomEvent(_4,"mouseout","SlidingContainer_OnMouseOut");
var _5=this.GetSlidingPaneResizeContainer();
this.AttachDomEvent(_5,"mouseover","ResizeSlidePane_OnMouseOver");
this.AttachDomEvent(_5,"mouseout","ResizeSlidePane_OnMouseOut");
this.AttachDomEvent(_5,"mousedown","ResizeSlidePane_OnMouseDown");
this.AttachDomEvent(this.GetDockIconElement(),"mousedown","DockElement_OnMouseDown");
this.AttachDomEvent(this.GetDockIconElement(),"mouseover","DockElement_OnMouseOver");
this.AttachDomEvent(this.GetDockIconElement(),"mouseout","DockElement_OnMouseOut");
this.AttachDomEvent(this.GetUnDockIconElement(),"mousedown","UnDockElement_OnMouseDown");
this.AttachDomEvent(this.GetUnDockIconElement(),"mouseover","UnDockElement_OnMouseOver");
this.AttachDomEvent(this.GetUnDockIconElement(),"mouseout","UnDockElement_OnMouseOut");
this.AttachDomEvent(this.GetCollapseIconElement(),"mousedown","CollapseElement_OnMouseDown");
this.AttachDomEvent(this.GetCollapseIconElement(),"mouseover","CollapseElement_OnMouseOver");
this.AttachDomEvent(this.GetCollapseIconElement(),"mouseout","CollapseElement_OnMouseOut");
};
RadSplitter_SlidingPane.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
};
RadSplitter_SlidingPane.prototype.SaveState=function(){
var _6=document.getElementById(this.stateFieldId);
var _7={"width":this.width,"height":this.height,"title":this.title,"minWidth":this.minWidth,"maxWidth":this.maxWidth,"minHeight":this.minHeight,"maxHeight":this.maxHeight,"enableResize":this.resizable,"enableDock":this.dockable};
var _8=RadControlsNamespace.JSON.stringify(_7);
this.stateString=_8;
_6.value=_8;
};
RadSplitter_SlidingPane.prototype.IsResizable=function(){
return this.resizable;
};
RadSplitter_SlidingPane.prototype.IsDockable=function(){
return this.dockable;
};
RadSplitter_SlidingPane.prototype.SetDockable=function(_9){
this.dockable=_9;
this.SaveState();
};
RadSplitter_SlidingPane.prototype.SetResizable=function(_a){
this.resizable=_a;
this.SaveState();
};
RadSplitter_SlidingPane.prototype.GetState=function(){
return this.stateString;
};
RadSplitter_SlidingPane.prototype.GetContentContainer=function(){
return document.getElementById("RAD_SLIDING_PANE_CONTENT_"+this.ID);
};
RadSplitter_SlidingPane.prototype.SetWidth=function(_b){
this.width=parseInt(_b,10);
this.SaveState();
};
RadSplitter_SlidingPane.prototype.GetWidth=function(){
return this.width;
};
RadSplitter_SlidingPane.prototype.SetHeight=function(_c){
this.height=parseInt(_c,10);
this.SaveState();
};
RadSplitter_SlidingPane.prototype.GetHeight=function(){
return this.height;
};
RadSplitter_SlidingPane.prototype.SetTitle=function(_d){
this.title=_d;
this.SaveState();
};
RadSplitter_SlidingPane.prototype.GetTitle=function(){
return this.title;
};
RadSplitter_SlidingPane.prototype.GetContent=function(){
var _e=this.GetContentContainer();
return _e.innerHTML;
};
RadSplitter_SlidingPane.prototype.GetMinWidth=function(){
return this.minWidth;
};
RadSplitter_SlidingPane.prototype.GetMaxWidth=function(){
return this.maxWidth;
};
RadSplitter_SlidingPane.prototype.SetMinWidth=function(_f){
this.minWidth=parseInt(_f);
this.SaveState();
};
RadSplitter_SlidingPane.prototype.SetMaxWidth=function(_10){
this.minWidth=parseInt(_10);
this.SaveState();
};
RadSplitter_SlidingPane.prototype.GetMinHeight=function(){
return this.minHeight;
};
RadSplitter_SlidingPane.prototype.GetMaxHeight=function(){
return this.maxHeight;
};
RadSplitter_SlidingPane.prototype.SetMinHeight=function(_11){
this.minHeight=parseInt(_11);
this.SaveState();
};
RadSplitter_SlidingPane.prototype.SetMaxHeight=function(_12){
this.minHeight=parseInt(_12);
this.SaveState();
};
RadSplitter_SlidingPane.prototype.SetContent=function(_13){
var _14=this.GetContentContainer();
_14.innerHTML=_13;
};
RadSplitter_SlidingPane.prototype.GetSlideContainer=function(){
return document.getElementById("RAD_SPLITTER_SLIDING_ZONE_CONTAINER_"+this.ID);
};
RadSplitter_SlidingPane.prototype.GetDockIconElement=function(){
return document.getElementById("RAD_SPLITTER_SLIDING_PANE_DOCK_"+this.ID);
};
RadSplitter_SlidingPane.prototype.GetUnDockIconElement=function(){
return document.getElementById("RAD_SPLITTER_SLIDING_PANE_UNDOCK_"+this.ID);
};
RadSplitter_SlidingPane.prototype.GetCollapseIconElement=function(){
return document.getElementById("RAD_SPLITTER_SLIDING_PANE_COLLAPSE_"+this.ID);
};
RadSplitter_SlidingPane.prototype.GetSlidingContainerTitle=function(){
return document.getElementById("RAD_SPLITTER_SLIDING_TITLE_"+this.ID);
};
RadSplitter_SlidingPane.prototype.GetSlidingPaneResizeContainer=function(){
return document.getElementById("RAD_SPLITTER_SLIDING_ZONE_RESIZE_"+this.ID);
};
RadSplitter_SlidingPane.prototype.IsExpanded=function(){
return this.isExpanded;
};
RadSplitter_SlidingPane.prototype.IsDocked=function(){
return this.isDocked;
};
RadSplitter_SlidingPane.prototype.ResizeSlidePane_OnMouseOver=function(e){
var _16=this.GetSlidingPaneResizeContainer();
_16.className=(this.slidingZone.isHorizontalSlide())?"slideContainerResizeOver":"slideContainerResizeOverHorizontal";
};
RadSplitter_SlidingPane.prototype.ResizeSlidePane_OnMouseOut=function(e){
e=(e)?e:window.event;
var _18=this.GetSlidingPaneResizeContainer();
_18.className=(this.slidingZone.isHorizontalSlide())?"slideContainerResize":"slideContainerResizeHorizontal";
};
RadSplitter_SlidingPane.prototype.ResizeSlidePane_OnMouseDown=function(e){
this.Log.Debug(["SlidingPane.ResizeSlidePane_OnMouseDown zone[%d]",this.ID]);
e=(e)?e:window.event;
var _1a=this.slidingZone.isHorizontalSlide();
RadControlsNamespace.DomEvent.PreventDefault(e);
RadControlsNamespace.DomEvent.StopPropagation(e);
this.maxDecreaseDelta=(_1a)?this.GetWidth()-this.GetMinWidth():this.GetHeight()-this.GetMinHeight();
var _1b=(_1a)?this.GetMaxWidth()-this.GetWidth():this.GetMaxHeight()-this.GetHeight();
var _1c=this.slidingZone.containerPane;
var _1d=_1c.getAvailIncreaseDelta();
var _1e=this.slidingZone.splitter;
var _1f=_1e.getAvailDecreaseDelta(_1c.Index,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward);
var _20=Math.min(_1f,_1d);
var _21=this.slidingZone.GetDockedPaneId();
if(_21!=null){
var _22=this.slidingZone.GetPaneById(_21);
_20+=(_1a)?_22.GetWidth():_22.GetHeight();
}
_20-=(_1a)?this.GetWidth():this.GetHeight();
this.maxIncreaseDelta=Math.min(_1b,_20);
if(this.slidingZone.IsLeftDirection()||this.slidingZone.IsTopDirection()){
var t=this.maxIncreaseDelta;
this.maxIncreaseDelta=this.maxDecreaseDelta;
this.maxDecreaseDelta=t;
}
var _24=this.GetSlidingPaneResizeContainer();
var pos=window.RadControlsNamespace.Screen.GetElementPosition(_24);
this.Log.Debug(["SlidingPane.onMouseDown: panex[%d], paney[%d], mousex[%d], mousey[%d]\\n"+"maxDecrease[%d], maxIncrease[%d]",pos.x,pos.y,e.clientX,e.clientY,this.maxDecreaseDelta,this.maxIncreaseDelta]);
this.mouseStartX=e.clientX;
this.mouseStartY=e.clientY;
this.mouseOffsetX=e.clientX-pos.x;
this.mouseOffsetY=e.clientY-pos.y;
this.handlerStartLeftPos=pos.x;
this.handlerStartTopPos=pos.y;
this.currentDelta=0;
this.AttachDomEvent(document,"mouseup","ResizeSlidePane_OnMouseUp");
this.AttachDomEvent(document,"mousemove","ResizeSlidePane_OnMouseMove");
this.resizeMode=true;
return false;
};
RadSplitter_SlidingPane.prototype.ResizeSlidePane_OnMouseUp=function(e){
var _27=this.slidingZone.isHorizontalSlide();
this.Log.Debug(["SlidingPane.ResizeSlidePane_OnMouseUp zone[%d]",this.ID]);
e=(e)?e:window.event;
RadControlsNamespace.DomEvent.PreventDefault(e);
RadControlsNamespace.DomEvent.StopPropagation(e);
this.DetachDomEvent(document,"mousemove","ResizeSlidePane_OnMouseMove");
this.DetachDomEvent(document,"mouseup","ResizeSlidePane_OnMouseUp");
this.resizeMode=false;
if(this.helperBar){
this.helperBar.parentNode.removeChild(this.helperBar);
this.helperBar=null;
this.helperBarOverlay.Dispose();
}
if(this.slidingZone.IsLeftDirection()||this.slidingZone.IsTopDirection()){
this.currentDelta*=-1;
}
if(!this.IsExpanded()){
return false;
}
if(this.currentDelta!=0&&this.RaiseEvent("OnClientBeforePaneResize",{paneObj:this,delta:this.currentDelta})){
this.Log.Debug(["SlidingPane.resizeSlidePane_OnMouseUp Going to resize. delta is [%d]",this.currentDelta]);
var _28=null;
var _29=null;
if(_27){
_28=this.slidingContainerWidth+this.currentDelta;
}else{
_29=this.slidingContainerHeight+this.currentDelta;
}
this.setSlidingContainerSize(_28,_29);
if(this.slidingZone.IsLeftDirection()){
var _2a=this.GetSlideContainer();
var _2b=parseInt(_2a.style.left);
_2a.style.left=_2b-this.currentDelta+"px";
}
if(this.slidingZone.IsTopDirection()){
var _2a=this.GetSlideContainer();
var _2c=parseInt(_2a.style.top);
_2a.style.top=_2c-this.currentDelta+"px";
}
var _2d=this.GetWidth();
var _2e=this.GetHeight();
if(_27){
this.SetWidth(_28);
}else{
this.SetHeight(_29);
}
this.RaiseEvent("OnClientPaneResized",{paneObj:this,oldWidth:_2d,newWidth:this.GetWidth(),oldHeight:_2e,newHeight:this.GetHeight()});
this.callRadResize();
}
return false;
};
RadSplitter_SlidingPane.prototype.ResizeSlidePane_OnMouseMove=function(e){
RadControlsNamespace.DomEvent.PreventDefault(e);
RadControlsNamespace.DomEvent.StopPropagation(e);
this.resizeMode=true;
var _30=this.slidingZone.isHorizontalSlide();
var _31=150;
if(!this.helperBar){
var _32=this.slidingZone.splitter;
var _33=document.createElement("TABLE");
_33.className=_32.GetContainerElement().className;
_33.style.borderCollapse="separate";
_33.cellSpacing=_31;
_33.cellPadding=0;
_33.style.borderWidth="0px";
_33.style.background="";
var _34=document.createElement("TBODY");
_33.appendChild(_34);
var TR=document.createElement("TR");
_34.appendChild(TR);
var TD=document.createElement("TD");
TR.appendChild(TD);
var _37=document.createElement("DIV");
_37.className="helperBarSlideDrag";
TD.appendChild(_37);
_33.style.position="absolute";
_33.style.left=this.handlerStartLeftPos-_31+"px";
_33.style.top=this.handlerStartTopPos-_31+"px";
this.helperBar=document.body.insertBefore(_33,document.body.firstChild);
if(_30){
this.box.SetOuterHeight(_37,this.slidingZone.containerPane.GetHeight());
this.box.SetOuterWidth(_37,3);
}else{
this.box.SetOuterWidth(_37,this.slidingZone.containerPane.GetWidth());
this.box.SetOuterHeight(_37,3);
}
this.helperBarDecoration=_37;
this.helperBarOverlay=new RadControlsNamespace.Overlay(this.helperBarDecoration);
}
var _38=(_30)?e.clientX-this.mouseStartX:e.clientY-this.mouseStartY;
var _39=false;
if(_38<((-1)*this.maxDecreaseDelta)){
_39=true;
_38=this.maxDecreaseDelta*(-1);
}
if(_38>this.maxIncreaseDelta){
_39=true;
_38=this.maxIncreaseDelta;
}
if(this.resizeStep>0){
_38-=_38%this.resizeStep;
}
this.helperBarDecoration.className=(_30)?"helperBarSlideDrag":"helperBarSlideDragHorizontal";
var _3a=(_30)?"w-resize":"n-resize";
this.helperBarDecoration.style.cursor=_3a;
if(_30){
this.helperBar.style.left=this.handlerStartLeftPos+_38-_31+"px";
}else{
this.helperBar.style.top=this.handlerStartTopPos+_38-_31+"px";
}
this.helperBar.style.zIndex=this.GetSlideContainer().style.zIndex+1;
this.helperBar.style.cursor=_3a;
this.helperBarOverlay.Update();
this.currentDelta=_38;
if(_39){
if(this.helperBarDecoration){
this.helperBarDecoration.className="helperBarSlideError";
}
}
return false;
};
RadSplitter_SlidingPane.prototype.callRadResize=function(){
RadControlsNamespace.CallRadResize(this.GetContentContainer());
};
RadSplitter_SlidingPane.prototype.callRadShow=function(){
RadControlsNamespace.CallRadShow(this.GetContentContainer());
};
RadSplitter_SlidingPane.prototype.SlidingContainer_OnMouseOut=function(e){
if(this.IsDocked()){
return;
}
if(this.resizeMode){
return;
}
if(!this.IsExpanded()){
return;
}
if(this.isMouseInPaneRectangle(e)){
return;
}
e=(e)?e:window.event;
var _3c=this;
var f=function(){
_3c.slidingZone.paneTabInMover=null;
_3c.slidingZone.CollapsePane(_3c.ID);
};
window.clearTimeout(this.slidingZone.paneTabMoutTimeout);
this.slidingZone.paneTabMoutTimeout=window.setTimeout(f,1000);
};
RadSplitter_SlidingPane.prototype.SlidingContainer_OnMouseOver=function(e){
if(this.IsDocked()){
return;
}
e=(e)?e:window.event;
if(this.resizeMode){
return;
}
window.clearTimeout(this.slidingZone.paneTabMoutTimeout);
};
RadSplitter_SlidingPane.prototype.DockElement_OnMouseDown=function(e){
if(e.button&&e.button!=1){
return true;
}
this.Log.Debug(["SlidingPane.DockElement_OnMouseDown zone[%d]",this.ID]);
if(!this.IsExpanded()){
return;
}
if(!this.slidingZone.CollapsePane(this.ID,true)){
return;
}
this.slidingZone.DockPane(this.ID);
this.GetDockIconElement().className="slideHeaderIcon";
};
RadSplitter_SlidingPane.prototype.DockElement_OnMouseOver=function(e){
if(e.button&&e.button!=1){
return true;
}
this.GetDockIconElement().className="slideHeaderIconOver";
};
RadSplitter_SlidingPane.prototype.DockElement_OnMouseOut=function(e){
if(e.button&&e.button!=1){
return true;
}
this.GetDockIconElement().className="slideHeaderIcon";
};
RadSplitter_SlidingPane.prototype.UnDockElement_OnMouseDown=function(e){
if(e.button&&e.button!=1){
return true;
}
this.Log.Debug(["SlidingPane.UnDockElement_OnMouseDown zone[%d]",this.ID]);
if(!this.IsDocked()){
return;
}
this.slidingZone.UnDockPane(this.ID);
};
RadSplitter_SlidingPane.prototype.UnDockElement_OnMouseOver=function(e){
if(e.button&&e.button!=1){
return true;
}
this.GetUnDockIconElement().className="slideHeaderIconOver";
};
RadSplitter_SlidingPane.prototype.UnDockElement_OnMouseOut=function(e){
if(e.button&&e.button!=1){
return true;
}
this.GetUnDockIconElement().className="slideHeaderIcon";
};
RadSplitter_SlidingPane.prototype.CollapseElement_OnMouseDown=function(e){
if(e.button&&e.button!=1){
return true;
}
this.Log.Debug(["SlidingPane.CollapseElement_OnMouseDown zone[%d]",this.ID]);
if(!this.IsExpanded()){
return;
}
this.slidingZone.paneTabInMover=null;
this.slidingZone.CollapsePane(this.ID);
};
RadSplitter_SlidingPane.prototype.CollapseElement_OnMouseOver=function(e){
if(e.button&&e.button!=1){
return true;
}
this.GetCollapseIconElement().className="slideHeaderIconOver";
};
RadSplitter_SlidingPane.prototype.CollapseElement_OnMouseOut=function(e){
if(e.button&&e.button!=1){
return true;
}
this.GetCollapseIconElement().className="slideHeaderIcon";
};
RadSplitter_SlidingPane.prototype.expandSlidingContainer=function(){
var _48=this.GetSlideContainer();
var _49=_48.parentNode;
_48.style.left="0px";
_48.style.top="0px";
var _4a=RadControlsNamespace.Screen.GetElementPosition(this.slidingZone.GetZoneContainer());
var _4b=0;
if(this.slidingZone.dockedPaneId){
var _4c=this.slidingZone.GetPaneById(this.slidingZone.dockedPaneId);
if(_4c.Index<this.Index){
_4b=this.box.GetOuterHeight(this.slidingZone.GetZoneContainer());
}
}
var _4d=this.slideBorderSize;
_49.style.position="absolute";
var _4e=_4a.y;
if(this.slidingZone.IsBottomDirection()){
var _4f=this.slidingZone.GetTabsContainer();
_4e+=_4f.offsetHeight;
}
_49.style.top=_4e+"px";
var _50=RadControlsNamespace.Screen.GetElementPosition(_49);
if(_50.y!=_4e){
_49.style.top=_4e-_4d-(_50.y-_4e)+"px";
}else{
_49.style.top=_4e-_4d+"px";
}
var _51=(this.IsResizable()&&(RadControlsNamespace.Browser.IsIE7||RadControlsNamespace.Browser.IsMozilla))?this.resizeBarSize+1:1;
var _52=0;
if(this.slidingZone.IsLeftDirection()){
var _4f=this.slidingZone.GetTabsContainer();
var _53=0;
var _54=RadControlsNamespace.Screen.GetElementPosition(_4f);
_49.style.left=_54.x+_51+"px";
var _50=RadControlsNamespace.Screen.GetElementPosition(_49);
if(_50.x!=_54.x){
_53=_50.x-_54.x;
}
var _55=_54.x-_53;
_49.style.left="";
_52=-1*_55;
}
if(this.slidingZone.IsTopDirection()){
var _4f=this.slidingZone.GetTabsContainer();
var _56=0;
var _54=RadControlsNamespace.Screen.GetElementPosition(_4f);
_49.style.top=_54.y+_51+"px";
var _50=RadControlsNamespace.Screen.GetElementPosition(_49);
if(_50.y!=_54.y){
_56=_50.y-_54.y;
}
var _57=_54.y-_56;
_49.style.top="";
_4b=-1*_57;
}
var _58=_48;
var _59={ExpandAnimation:{Type:"Linear",Duration:this.slideDuration},CollapseAnimation:{Type:"Linear",Duration:50}};
var _5a=this;
var _5b={OnExpandComplete:function(){
_49.style.width="0px";
_49.style.height="0px";
if(RadControlsNamespace.Browser.IsMozilla){
_5a.GetContentContainer().style.overflow=_5a.contentOverflow;
_5a.GetContentContainer().style.overflowX=_5a.contentOverflowX;
_5a.GetContentContainer().style.overflowY=_5a.contentOverflowY;
}
_5a.RaiseEvent("OnClientPaneExpanded",{paneObj:_5a});
_5a.callRadShow();
}};
try{
this.Ease.Ticker.Stop();
this.Ease.Overlay.Dispose();
this.Ease.Overlay=null;
}
catch(e){
}
if(RadControlsNamespace.Browser.IsMozilla){
this.contentOverflow=this.GetContentContainer().style.overflow;
this.contentOverflowX=this.GetContentContainer().style.overflowX;
this.contentOverflowY=this.GetContentContainer().style.overflowY;
this.GetContentContainer().style.overflow="hidden";
}
this.Ease=new RadControlsNamespace.Ease(_58,_59,_52,_4b,_5b,true);
var _5c="";
if(this.slidingZone.IsLeftDirection()){
_5c="right";
}
if(this.slidingZone.IsRightDirection()){
_5c="left";
}
if(this.slidingZone.IsTopDirection()){
_5c="bottom";
}
if(this.slidingZone.IsBottomDirection()){
_5c="top";
}
this.Ease.SetSide(_5c);
this.Ease.ShowElements();
this.Ease.UpdateContainerSize();
this.Ease.In();
};
RadSplitter_SlidingPane.prototype.collapseSlidingContainer=function(){
this.Log.Debug(["SlidingPane.collapseSlidingContainer zone[%d]",this.ID]);
this.hideSlidingContainer();
try{
this.Ease.Overlay.Dispose();
this.Ease.Overlay=null;
this.Ease.Ticker.Stop();
}
catch(e){
}
this.RaiseEvent("OnClientPaneCollapsed",{paneObj:this});
};
RadSplitter_SlidingPane.prototype.setSlidingContainerSize=function(_5d,_5e){
this.Log.Debug(["SlidingPane.setSlidingContainerSize set width[%d], height[%d], zone[%d]",_5d,_5e,this.ID]);
var _5f=this.GetSlideContainer();
var _60=this.GetContentContainer();
var _61=this.GetSlidingContainerTitle();
if(_5d==null){
_5d=(this.slidingZone.isHorizontalSlide())?this.GetWidth():this.slidingZone.containerPane.GetWidth();
}
if(_5e==null){
_5e=(this.slidingZone.isVerticalSlide())?this.GetHeight():this.slidingZone.containerPane.GetHeight();
}
var _62=10;
var _63=66;
var _64=_5d-_63-_62;
_64=Math.max(0,_64);
this.box.SetOuterWidth(_61,_64);
this.box.SetOuterWidth(_60,_5d);
this.box.SetOuterWidth(_5f,_5d);
this.box.SetOuterWidth(_5f.parentNode,_5d);
this.slidingContainerWidth=_5d;
var _65=this.box.GetOuterHeight(_61);
this.box.SetOuterHeight(_61,_65);
this.box.SetOuterHeight(_5f,_5e-this.slideBorderSize);
this.box.SetOuterHeight(_60.parentNode,_5e-_65-this.slideBorderSize);
this.box.SetOuterHeight(_60,_5e-_65-this.slideBorderSize);
this.slidingContainerHeight=_5e;
if(this.Ease&&this.Ease.Overlay){
try{
this.Ease.Overlay.Update();
}
catch(e){
}
}
};
RadSplitter_SlidingPane.prototype.showSlidingContainer=function(){
var _66=this.GetSlideContainer();
_66.style.display="";
_66.parentNode.style.display="";
};
RadSplitter_SlidingPane.prototype.hideSlidingContainer=function(){
var _67=this.GetSlideContainer();
_67.style.display="none";
_67.style.top="";
_67.style.left="";
_67.parentNode.style.display="none";
_67.parentNode.style.top="";
_67.parentNode.style.left="";
};
RadSplitter_SlidingPane.prototype.dockSlidingContainer=function(){
var _68=this.GetSlideContainer();
_68.style.position="static";
_68.parentNode.style.position="";
_68.firstChild.className="slideContainerDocked";
};
RadSplitter_SlidingPane.prototype.unDockSlidingContainer=function(){
var _69=this.GetSlideContainer();
_69.style.position="absolute";
_69.parentNode.style.position="absolute";
_69.parentNode.style.top="";
_69.parentNode.style.left="";
_69.firstChild.className="slideContainer";
};
RadSplitter_SlidingPane.prototype.setSlidingContainerResizable=function(_6a){
var _6b=this.GetSlidingPaneResizeContainer();
_6b.style.display=(_6a)?"":"none";
};
RadSplitter_SlidingPane.prototype.setSlidingContainerDockable=function(_6c){
this.GetDockIconElement().style.display=(_6c)?"":"none";
};
RadSplitter_SlidingPane.prototype.setIconsExpandedState=function(){
this.hideAllIcons();
this.setSlidingContainerDockable(this.IsDockable());
this.GetCollapseIconElement().style.display="";
this.GetDockIconElement().className="slideHeaderIcon";
this.GetCollapseIconElement().className="slideHeaderIcon";
};
RadSplitter_SlidingPane.prototype.setIconsDockedState=function(){
this.hideAllIcons();
this.GetUnDockIconElement().style.display="";
this.GetUnDockIconElement().className="slideHeaderIcon";
};
RadSplitter_SlidingPane.prototype.hideAllIcons=function(){
this.GetDockIconElement().style.display="none";
this.GetUnDockIconElement().style.display="none";
this.GetCollapseIconElement().style.display="none";
};
RadSplitter_SlidingPane.prototype.getTitleContainerElement=function(){
return document.getElementById("RAD_SLIDING_PANE_TEXT_"+this.ID);
};
RadSplitter_SlidingPane.prototype.dock=function(){
this.setIconsDockedState();
this.setSlidingContainerResizable(false);
this.dockSlidingContainer();
this.showSlidingContainer();
this.setSlidingContainerSize();
this.GetSlideContainer().parentNode.style.width=this.GetWidth()+"px";
this.callRadShow();
this.isExpanded=false;
this.isDocked=true;
};
RadSplitter_SlidingPane.prototype.undock=function(){
this.isExpanded=false;
this.isDocked=false;
this.hideSlidingContainer();
};
RadSplitter_SlidingPane.prototype.expand=function(){
this.unDockSlidingContainer();
if(document.documentElement){
var _6d=document.documentElement.style.overflowX;
document.documentElement.style.overflowX="hidden";
}
this.showSlidingContainer();
this.setSlidingContainerSize();
this.setSlidingContainerResizable(this.IsResizable());
this.setIconsExpandedState();
this.expandSlidingContainer();
if(document.documentElement){
document.documentElement.style.overflowX=_6d;
}
this.isExpanded=true;
this.isDocked=false;
};
RadSplitter_SlidingPane.prototype.collapse=function(){
this.isExpanded=false;
this.isDocked=false;
this.collapseSlidingContainer();
};
RadSplitter_SlidingPane.prototype.isMouseInPaneRectangle=function(e){
var _6f=this.GetSlideContainer();
var _70=this.box.GetOuterWidth(_6f);
var _71=this.box.GetOuterHeight(_6f);
var pos=window.RadControlsNamespace.Screen.GetElementPosition(_6f);
var _73=pos.x;
var _74=pos.y;
var _75=e.clientX;
var _76=e.clientY;
if(_75==-1||_76==-1){
return true;
}
var _77=(_73<=_75&&(_73+_70)>=_75&&_74<=_76&&(_74+_71)>=_76);
return _77;
};;function RadSplitter_SlidingZone(_1){
this.ID=_1["ID"];
_1["logLevel"]=eval(_1["logLevel"]);
this.Log=new RadControlsNamespace.Log(_1["logLevel"],true);
if(typeof (RadControlsNamespace.__spl_sliding_zones__)=="undefined"){
RadControlsNamespace.__spl_sliding_zones__=[];
}
if(RadControlsNamespace.__spl_sliding_zones__[this.ID]){
this.Log.Debug(["RadSplitter_SlidingZone dispose the old sliding zone. ID[%d]",this.ID]);
RadControlsNamespace.__spl_sliding_zones__[this.ID].Dispose();
}
this.slidingPanes=[];
this.slidingPanesById=[];
this.containerPaneId=_1["parentPaneId"];
this.containerSplitterId=_1["parentSplitterId"];
this.expandDirection=RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
this.width=_1["width"];
this.clickToOpen=_1["clickToOpen"];
this.config=_1;
this.slideDirection=_1["slideDirection"];
this.initiallyDockedPaneId=_1["initiallyDockedPaneId"];
this.initiallyExpandedPaneId=_1["initiallyExpandedPaneId"];
this.stateFieldId=_1["stateFieldId"];
this.box=window.RadControlsNamespace.Box;
this.supportedEvents=["OnClientLoaded"];
for(var i=0;i<this.supportedEvents.length;i++){
this[this.supportedEvents[i]]=_1[this.supportedEvents[i]];
}
this.dockedPaneId=null;
this.expandedPaneId=null;
this.initOnLoad();
}
RadSplitter_SlidingZone.prototype.Init=function(_3){
if(this.Inited&&!_3){
return;
}
this.Log.Debug(["SlidingZone.Init start ID[%d]",this.ID]);
for(var i=0;i<this.config["panes"].length;i++){
var _5=this.config["panes"][i];
var _6=window["RadSlidingPaneConfig_"+_5];
_6["slidingZone"]=this;
_6["logLevel"]=this.config["logLevel"];
_6["index"]=i;
_6["resizeStep"]=this.config["resizeStep"];
_6["slideDuration"]=this.config["slideDuration"];
var _7=new RadSplitter_SlidingPane(_6);
this.slidingPanes[this.slidingPanes.length]=_7;
this.slidingPanesById[_7.ID]=_7;
}
document.getElementById("RAD_SLIDING_ZONE_PANES_CONTAINER_"+this.ID).style.display="";
this.splitter=window[this.containerSplitterId];
this.containerPane=this.splitter.GetPaneById(this.containerPaneId);
RadControlsNamespace.DomEventMixin.Initialize(this);
RadControlsNamespace.EventMixin.Initialize(this);
var _8=this.slidingPanes;
for(var i=0;i<_8.length;i++){
var _9=this.GetTabContainer(this.slidingPanes[i].ID);
this.AttachDomEvent(_9,"mousedown","PaneTab_OnMouseDown");
this.AttachDomEvent(_9,"mouseover","PaneTab_OnMouseOver");
this.AttachDomEvent(_9,"mouseout","PaneTab_OnMouseOut");
_8[i].Init();
}
var _a=this;
var _b=function(_c,_d){
_a.handleParentPaneResized(_d);
};
var _e=function(_f,_10){
return _a.handleBeforeParentPaneResized(_10);
};
var _11=function(_12,_13){
_a.handleSplitterResized(_13);
};
this.containerPane.AttachEvent("OnClientBeforePaneResize",_e);
this.containerPane.AttachEvent("OnClientPaneResized",_b);
this.splitter.AttachEvent("OnClientResized",_11);
this.AttachDomEvent(window,"unload","Dispose");
this.Inited=true;
if(this.initiallyDockedPaneId!=""){
this.DockPane(this.initiallyDockedPaneId);
}
if(this.initiallyExpandedPaneId!=""){
this.ExpandPane(this.initiallyExpandedPaneId);
}
if(!this.IsLeftDirection()&&RadControlsNamespace.Browser.IsMozilla){
this.containerPane.GetContentContainerElement().style.overflow="";
}
this.SaveState();
this.RaiseEvent("OnClientLoaded",{slidingZoneObj:this});
this.Log.Debug(["SlidingZone.Init done zoneID[%d]",this.ID]);
};
RadSplitter_SlidingZone.prototype.SaveState=function(){
var _14=document.getElementById(this.stateFieldId);
var _15={"dockedPaneId":this.dockedPaneId||"","expandedPaneId":this.expandedPaneId||"","clickToOpen":this.clickToOpen};
var _16=RadControlsNamespace.JSON.stringify(_15);
this.stateString=_16;
_14.value=_16;
};
RadSplitter_SlidingZone.prototype.GetState=function(){
return this.stateString;
};
RadSplitter_SlidingZone.prototype.SetClickToOpen=function(_17){
this.clickToOpen=_17;
this.SaveState();
};
RadSplitter_SlidingZone.prototype.GetClickToOpen=function(){
return this.clickToOpen;
};
RadSplitter_SlidingZone.prototype.GetPaneById=function(_18){
return this.slidingPanesById[_18];
};
RadSplitter_SlidingZone.prototype.GetWidth=function(_19){
return this.width;
};
RadSplitter_SlidingZone.prototype.GetZoneContainer=function(){
return document.getElementById(this.ID);
};
RadSplitter_SlidingZone.prototype.GetTabsContainer=function(){
return document.getElementById("RAD_SLIDING_ZONE_TABS_CONTAINER_"+this.ID);
};
RadSplitter_SlidingZone.prototype.GetTabContainer=function(_1a){
return document.getElementById("RAD_SLIDING_PANE_TAB_"+_1a);
};
RadSplitter_SlidingZone.prototype.GetExpandedPaneId=function(){
return this.expandedPaneId;
};
RadSplitter_SlidingZone.prototype.GetDockedPaneId=function(){
return this.dockedPaneId;
};
RadSplitter_SlidingZone.prototype.DockPane=function(_1b){
var _1c=this.GetPaneById(_1b);
if(!_1c||!_1c.IsDockable()){
return false;
}
if(!_1c.RaiseEvent("OnClientBeforePaneDock",{paneObj:_1c})){
return false;
}
this.Log.Debug(["SlidingZone.DockPane paneId[%d], zoneID[%d]",_1b,this.ID]);
if(this.dockedPaneId){
if(!this.UnDockPane(this.dockedPaneId)){
return false;
}
}
this.dockingMode=true;
var _1d=(this.isHorizontalSlide())?_1c.GetWidth():_1c.GetHeight();
var _1e=this.splitter.getAvailIncreaseDelta(this.containerPane.Index,RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward);
this.Log.Debug(["SlidingZone.DockPane Available delta[%d]",_1e]);
var _1f=(this.isHorizontalSlide())?_1c.GetMinWidth():_1c.GetMinHeight();
if(_1e<_1f){
this.Log.Debug(["SlidingZone.DockPane No enough Available delta"]);
return false;
}
var _20=_1d+this.getTabsContainerSize();
var _21=this.containerPane.getVarSize();
var _22=_21+_1e;
var _23=Math.min(_22,_20);
var _24=_23-_21;
this.Log.Debug(["SlidingZone.DockPane parent pane delta[%d], zoneID[%d]",_24,this.ID]);
if(_24!=0){
var _25=(this.IsLeftDirection()||this.IsTopDirection())?RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward:RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
this.containerPane.Resize(_24,_25);
}
_1c.dock();
this.setTabDockedState(_1b);
if(this.isVerticalSlide()){
var _26=_1c.GetHeight()+this.getTabsContainerSize();
var _27=this.GetZoneContainer();
this.box.SetOuterHeight(document.getElementById("RAD_SLIDING_ZONE_PANES_CONTAINER_"+this.ID),_1c.GetHeight());
this.box.SetOuterHeight(_27,_26);
}
this.dockedPaneId=_1b;
this.SaveState();
this.Log.Debug(["SlidingZone.DockPane done pane[%d]",_1b]);
this.dockingMode=false;
_1c.RaiseEvent("OnClientPaneDocked",{paneObj:_1c});
return true;
};
RadSplitter_SlidingZone.prototype.UnDockPane=function(_28){
var _29=this.GetPaneById(_28);
if(!_29){
return false;
}
if(!_29.RaiseEvent("OnClientBeforePaneUnDock",{paneObj:_29})){
return false;
}
this.Log.Debug(["SlidingZone.UnDockPane pane[%d]",_28]);
if(!_28){
_28=this.dockedPaneId;
}
this.Log.Debug(["RadSplitter_SlidingZone.UnDockPane paneId[%d]",_28]);
if(this.containerPane.IsCollapsed()){
return false;
}
this.dockingMode=true;
_29.undock();
this.dockedPaneId=null;
var _2a=this.getTabsContainerSize();
if(this.isVerticalSlide()){
var _2b=this.GetZoneContainer();
this.box.SetOuterHeight(document.getElementById("RAD_SLIDING_ZONE_PANES_CONTAINER_"+this.ID),0);
this.box.SetOuterHeight(_2b,_2a);
}
var _2c=this.containerPane.getVarSize();
var _2d=_2c-_2a;
this.Log.Debug(["RadSplitter_SlidingZone.UnDockPane got zoneSize[%d], paneSize[%d], delta[%d], paneId[%d]",_2a,_2c,_2d,_28]);
var _2e=(this.IsLeftDirection()||this.IsTopDirection())?RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Backward:RadSplitterNamespace.RAD_SPLITTER_DIRECTION.Forward;
this.containerPane.Resize(_2d*(-1),_2e);
this.setTabDefaultState(_28);
this.SaveState();
this.Log.Debug(["SlidingZone.UnDockPane done[%d]",_28]);
this.dockingMode=false;
_29.RaiseEvent("OnClientPaneUnDocked",{paneObj:_29});
this.paneTabInMover=null;
return true;
};
RadSplitter_SlidingZone.prototype.GetPanes=function(){
return this.slidingPanes;
};
RadSplitter_SlidingZone.prototype.ExpandPane=function(_2f){
if(this.dockedPaneId==_2f){
return false;
}
var _30=this.GetPaneById(_2f);
var _31=this.GetTabContainer(_2f);
if(!_30||!_31){
return false;
}
if(!_30.RaiseEvent("OnClientBeforePaneExpand",{paneObj:_30})){
return false;
}
this.Log.Debug(["SlidingZone.ExpandPane paneId[%d], zone[%d]",_2f,this.ID]);
this.setTabExpandedState(_2f);
_30.expand();
this.expandedPaneId=_2f;
this.SaveState();
this.Log.Debug(["SlidingZone.ExpandPane Done paneId[%d], zone[%d]",_2f,this.ID]);
return true;
};
RadSplitter_SlidingZone.prototype.CollapsePane=function(_32,_33){
this.Log.Debug(["SlidingZone.CollapsePane paneId[%d], zone[%d]",_32,this.ID]);
if(this.expandedPaneId!=_32){
return true;
}
if(this.dockedPaneId==_32){
return false;
}
var _34=this.GetPaneById(_32);
var _35=this.GetTabContainer(_32);
if(!_34||!_35){
return false;
}
_33=(_33)?true:false;
if(!_34.RaiseEvent("OnClientBeforePaneCollapse",{paneObj:_34,goingToDock:_33})){
return false;
}
this.setTabDefaultState(_32);
_34.collapse();
this.expandedPaneId=null;
this.SaveState();
this.Log.Debug(["SlidingZone.CollapsePane DONE paneId[%d], zone[%d]",_32,this.ID]);
return true;
};
RadSplitter_SlidingZone.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
if(this.Ease){
this.Ease.Dispose();
}
for(var i=0;i<this.slidingPanes.length;i++){
this.slidingPanes[i].Dispose();
}
};
RadSplitter_SlidingZone.prototype.HideTab=function(_37){
this.Log.Debug(["SlidingZone.HideTab paneID[%d], zone[%d]",_37,this.ID]);
var _38=this.GetTabContainer(_37);
if(_38==null){
return;
}
_38.style.display="none";
};
RadSplitter_SlidingZone.prototype.ShowTab=function(_39){
this.Log.Debug(["SlidingZone.ShowTab paneID[%d], zone[%d]",_39,this.ID]);
var _3a=this.GetTabContainer(_39);
if(_3a==null){
return;
}
_3a.style.display="";
};
RadSplitter_SlidingZone.prototype.IsTabDisplayed=function(_3b){
var _3c=this.GetTabContainer(_3b);
if(_3c==null){
return false;
}
return (_3c.style.display!="none");
};
RadSplitter_SlidingZone.prototype.PaneTab_OnMouseOver=function(e){
e=(e)?e:window.event;
window.clearTimeout(this.paneTabMoutTimeout);
var _3e=RadControlsNamespace.DomEvent.GetTarget(e);
var _3f=this.getPaneIdFromTabElement(_3e);
if(!_3f){
return;
}
if(_3f==this.paneTabInMover){
return;
}
this.Log.Debug(["SlidingZone.PaneTab_OnMouseOver pane [%d], zone[%d]",_3f,this.ID]);
this.paneTabInMover=_3f;
if(!this.clickToOpen){
if(this.expandedPaneId!=null){
if(!this.CollapsePane(this.expandedPaneId)){
return;
}
}
this.ExpandPane(_3f);
}
};
RadSplitter_SlidingZone.prototype.PaneTab_OnMouseOut=function(e){
if(this.clickToOpen){
return;
}
e=(e)?e:window.event;
var _41=RadControlsNamespace.DomEvent.GetTarget(e);
var _42=this.getPaneIdFromTabElement(_41);
if(!_42){
return;
}
var _43=this;
var f=function(){
_43.paneTabInMover=null;
_43.CollapsePane(_42);
};
this.paneTabMoutTimeout=window.setTimeout(f,100);
};
RadSplitter_SlidingZone.prototype.PaneTab_OnMouseDown=function(e){
this.Log.Debug(["SlidingZone.PaneTab_OnMouseDown zone[%d]",this.ID]);
if(!this.clickToOpen){
return;
}
e=(e)?e:window.event;
var _46=RadControlsNamespace.DomEvent.GetTarget(e);
this.Log.Debug(["SlidingZone.PaneTab_OnMouseDown got target element [%d]",_46.id]);
var _47=this.getPaneIdFromTabElement(_46);
this.Log.Debug(["SlidingZone.PaneTab_OnMouseDown found paneId[%d]",_47]);
if(this.expandedPaneId==_47){
this.CollapsePane(_47);
}else{
if(this.expandedPaneId){
if(!this.CollapsePane(this.expandedPaneId)){
return;
}
}
this.ExpandPane(_47);
}
};
RadSplitter_SlidingZone.prototype.IsLeftDirection=function(){
return (this.slideDirection==RadSplitterNamespace.RAD_SPLITTER_SLIDE_DIRECTION.Left);
};
RadSplitter_SlidingZone.prototype.IsRightDirection=function(){
return (this.slideDirection==RadSplitterNamespace.RAD_SPLITTER_SLIDE_DIRECTION.Right);
};
RadSplitter_SlidingZone.prototype.IsTopDirection=function(){
return (this.slideDirection==RadSplitterNamespace.RAD_SPLITTER_SLIDE_DIRECTION.Top);
};
RadSplitter_SlidingZone.prototype.IsBottomDirection=function(){
return (this.slideDirection==RadSplitterNamespace.RAD_SPLITTER_SLIDE_DIRECTION.Bottom);
};
RadSplitter_SlidingZone.prototype.isHorizontalSlide=function(){
return (this.slideDirection==RadSplitterNamespace.RAD_SPLITTER_SLIDE_DIRECTION.Left||this.slideDirection==RadSplitterNamespace.RAD_SPLITTER_SLIDE_DIRECTION.Right);
};
RadSplitter_SlidingZone.prototype.isVerticalSlide=function(){
return !this.isHorizontalSlide();
};
RadSplitter_SlidingZone.prototype.getPaneIdFromTabElement=function(_48){
while(_48&&_48.tagName!="DIV"){
_48=_48.parentNode;
}
if(!_48||_48.id.indexOf("RAD_SLIDING_PANE_TAB_")==-1){
return "";
}
return _48.id.substr("RAD_SLIDING_PANE_TAB_".length);
};
RadSplitter_SlidingZone.prototype.initOnLoad=function(){
if(typeof (window[this.containerSplitterId])=="undefined"||!window[this.containerSplitterId].Inited){
var _49=this;
var _4a=function(){
_49.initOnLoad();
};
window.setTimeout(_4a,10);
}else{
this.Init();
}
};
RadSplitter_SlidingZone.prototype.handleBeforeParentPaneResized=function(_4b){
if(this.dockingMode){
return true;
}
this.Log.Debug(["SlidingZone.handleBeforeParentPaneResized delta[%d] zoneID[%d]",_4b.delta,this.ID]);
var _4c=_4b.delta;
if(this.dockedPaneId){
var _4d=this.GetPaneById(this.dockedPaneId);
if(!_4d.IsResizable()){
return false;
}
var _4e=this.getTabsContainerSize();
var _4f=_4e+(this.isHorizontalSlide())?_4d.GetMinWidth():_4d.GetMinHeight();
var _50=_4e+(this.isHorizontalSlide())?_4d.GetMaxWidth():_4d.GetMaxHeight();
var _51=(this.isHorizontalSlide())?_4d.slidingContainerWidth:_4d.slidingContainerHeight;
var _52=_51+_4c;
if(_52>_50||_52<_4f){
return false;
}
return _4d.RaiseEvent("OnClientBeforePaneResize",{paneObj:_4d,delta:_4c});
}
return true;
};
RadSplitter_SlidingZone.prototype.getTabsContainerSize=function(){
var _53=this.GetTabsContainer();
return (this.isHorizontalSlide())?this.box.GetOuterWidth(_53):this.box.GetOuterHeight(_53);
};
RadSplitter_SlidingZone.prototype.handleParentPaneResized=function(_54){
if(this.dockingMode){
return;
}
this.Log.Debug(["SlidingZone.handleParentPaneResized zoneID[%d]. Docked pane[%d]",this.ID,this.dockedPaneId]);
if(!this.dockedPaneId){
return;
}
var _55=this.GetPaneById(this.dockedPaneId);
if(!_55.IsResizable()){
return false;
}
var _56=_55.slidingContainerWidth;
var _57=_54.newWidth-_54.oldWidth;
var _58=_56+_57;
var _59=_55.slidingContainerHeight;
var _5a=_54.newHeight-_54.oldHeight;
var _5b=_59+_5a;
_55.setSlidingContainerSize(_58,_5b);
var _5c=_55.GetWidth();
var _5d=_54.oldHeight;
var _5b=_54.newHeight;
_55.SetWidth(_58);
_55.SetHeight(_5b);
if(this.isVerticalSlide()){
var _5e=_5b;
var _5f=this.GetZoneContainer();
this.box.SetOuterHeight(_5f,_5e);
this.box.SetOuterHeight(document.getElementById("RAD_SLIDING_ZONE_PANES_CONTAINER_"+this.ID),_5b-this.getTabsContainerSize());
}
_55.RaiseEvent("OnClientPaneResized",{paneObj:_55,oldWidth:_5c,newWidth:_55.GetWidth(),oldHeight:_5d,newHeight:_5b});
};
RadSplitter_SlidingZone.prototype.handleSplitterResized=function(_60){
this.Log.Debug(["SlidingZone.handleSplitterResized zoneID[%d]",this.ID]);
if(this.expandedPaneId){
if(_60.newWidth!=_60.oldWidth||_60.newHeight!=_60.oldHeight){
var _61=this.expandedPaneId;
this.CollapsePane(_61);
this.ExpandPane(_61);
}
}
};
RadSplitter_SlidingZone.prototype.setTabDefaultState=function(_62){
var _63=this.GetTabContainer(_62);
if(_63==null){
return false;
}
_63.className="paneTabContainer";
};
RadSplitter_SlidingZone.prototype.setTabDockedState=function(_64){
var _65=this.GetTabContainer(_64);
if(_65==null){
return false;
}
_65.className="paneTabContainerDocked";
};
RadSplitter_SlidingZone.prototype.setTabExpandedState=function(_66){
var _67=this.GetTabContainer(_66);
if(_67==null){
return false;
}
_67.className="paneTabContainerExpanded";
};;if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
RadControlsNamespace.Ticker=function(_1){
this.Listener=_1;
this.IntervalPointer=null;
};
RadControlsNamespace.Ticker.prototype={Configure:function(_2){
this.Duration=_2.Duration;
this.Interval=16;
},Start:function(){
clearInterval(this.IntervalPointer);
this.TimeElapsed=0;
var _3=this;
var _4=function(){
_3.Tick();
};
this.Tick();
this.IntervalPointer=setInterval(_4,this.Interval);
},Tick:function(){
this.TimeElapsed+=this.Interval;
this.Listener.OnTick(this.TimeElapsed);
if(this.TimeElapsed>=this.Duration){
this.Stop();
}
},Stop:function(){
if(this.IntervalPointer){
this.Listener.OnTickEnd();
clearInterval(this.IntervalPointer);
this.IntervalPointer=null;
}
}};;//BEGIN_ATLAS_NOTIFY
if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
