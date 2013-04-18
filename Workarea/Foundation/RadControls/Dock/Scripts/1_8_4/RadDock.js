if(typeof (window.RadDockNamespace)=="undefined"){
window.RadDockNamespace=new Object();
}
if(typeof (window.RadControlsNamespace)=="undefined"){
window.RadControlsNamespace=new Object();
}
window.RadDockNamespace.Browser={};
window.RadDockNamespace.Browser.Initialize=function(){
this.IsMacIE=(navigator.appName=="Microsoft Internet Explorer")&&((navigator.userAgent.toLowerCase().indexOf("mac")!=-1)||(navigator.appVersion.toLowerCase().indexOf("mac")!=-1));
this.IsSafari=(navigator.userAgent.toLowerCase().indexOf("safari")!=-1);
this.IsMozilla=window.netscape&&!window.opera;
this.IsOpera=window.opera;
this.IsOpera9=window.opera&&(parseInt(window.opera.version())>8);
this.IsIE=!this.IsMacIE&&!this.IsMozilla&&!this.IsOpera;
this.IsCompat=RadDockNamespace.Browser.IsSafari||RadDockNamespace.Browser.IsOpera9||RadDockNamespace.Browser.IsMozilla||document.compatMode=="CSS1Compat";
};
RadDockNamespace.Browser.Initialize();;if("undefined"==typeof (RadDockNamespace)){
RadDockNamespace=new Object();
}
var RadDockableObjectGripFlags={None:0,TitleBar:1,Top:2,Bottom:4,Left:8,Right:16,All:31,Auto:32};
RadDockNamespace.RadDockingModeFlags={Disabled:0,AlwaysDock:1,NeverDock:2,Dockable:3};
RadDockNamespace.RadDockableObjectBehaviorFlags={None:0,Resize:1,Collapse:2,Close:4,Pin:8,Resizable:7};
RadDockNamespace.RadDockableObjectState={Expanded:1,Pinned:2,Closed:4};
RadDockNamespace.InitDockableObject=function(_1,_2){
RadDockNamespace.InitMoveableObject(_1);
Object.Extend(_1,RadDockNamespace.RadDockableObject);
Object.Extend(_1,RadDockNamespace.RadEventHandlerList);
var _3=(document.all)?"iframe":"span";
var _4=document.createElement(_3);
_4=_4.cloneNode(true);
_4.src="javascript:'';";
_4.frameBorder=0;
_4.scrolling="no";
_4.style.filter="progid:DXImageTransform.Microsoft.Alpha(opacity=0)";
_4.style.overflow="hidden";
_4.style.display="inline";
_1.Overlay=document.body.insertBefore(_4,document.body.firstChild);
var i=0;
_1.IsDockingEnabled=_2[i++];
_1.ServerID=_2[i++];
_1.Behavior=_2[i++];
_1.DockingMode=_2[i++];
_1.AllowedDockingZoneTypes=_2[i++];
_1.AllowedDockingZones=_2[i++];
var _6=_2[i++];
_1.ParentDockingZone=_6?document.getElementById(_6):null;
_1.FloatingObjectEnabledGrips=_2[i++];
_1.DockedObjectEnabledGrips=_2[i++];
var _7=_2[i++];
_1.ClientRow=document.getElementById(_2[i++]);
_1.controlState=_2[i++];
_1.ExpandedHeight=_2[i++];
var _8=_2[i++];
var _9=_2[i++];
_1.clientEventHandlers=eval("("+_2[i++]+")");
_1.HiddenInput=document.getElementById(_9);
_1.InitGrips(_7);
_1.InitCommands(_8);
_1.SetDisplayStyle();
function setOverlayInitialPosition(){
_4.style.position="absolute";
if(_1.style.position=="absolute"){
var _a=RadGetElementRect(_1);
_4.style.left=_a.left;
_4.style.top=_a.top;
_4.style.width=_a.width;
_4.style.height=_a.height;
RadDockNamespace.FixIeHeight(_4);
}else{
_4.style.left=-10000;
_4.style.top=-10000;
}
}
if(!document.readyState||"complete"==document.readyState){
RadDockNamespace.FixIeHeight(_1);
_1.SaveState();
setOverlayInitialPosition();
}else{
if(window.attachEvent){
var _b=function(){
RadDockNamespace.FixIeHeight(_1);
_1.SaveState();
setOverlayInitialPosition();
};
window.attachEvent("onload",_b);
}
}
_1.ClientRow.cells[0].style.height=document.all?"100%":"";
var _c=["TitlebarCell","TitleCell","HorizontalGripCell1","HorizontalGripCell2","VerticalGripCell1","VerticalGripCell2"];
for(var i=0;i<_c.length;i++){
var _d=_c[i];
var _o=document.getElementById(_1.id+"_"+_d);
if(_o!=null){
RadDockNamespace.AttachEvent(_o,"dragstart",new Function("return false"));
_o.setAttribute("unselectable","on");
}
}
var _f=["DockStateChanged","Dock","UnDock","DragStart","Drag","Drop","DragEnd"];
for(var i=0;i<_f.length;i++){
_1.AddEventHandler(_f[i],_1.ExecuteClientEventHandler);
}
if(_1.IsPinned()){
if(!_1.IsClosed()&&!_1.IsDocked()){
_1.Pin();
}else{
_1.needPin=true;
}
}
};
RadDockNamespace.RadDockableObject={IsDockableObject:true,GetState:function(_10){
return ((this.controlState&_10)>0);
},SetState:function(_11,_12){
var old=this.controlState;
if(_12){
this.controlState|=_11;
}else{
this.controlState&=~_11;
}
if(old!=this.controlState){
this.UpdateCommandsState();
this.SaveState();
}
},UpdateCommandsState:function(){
var _14=this.IsDocked();
var _15=this.GetState(RadDockNamespace.RadDockableObjectState.Expanded);
var _16=this.GetCommandByName("Collapse");
if(_16){
_16.Enable(_15);
}
var _17=this.GetCommandByName("Expand");
if(_17){
_17.Enable(!_15);
}
var _18=this.GetState(RadDockNamespace.RadDockableObjectState.Pinned);
cmd=this.GetCommandByName("Unpin");
if(cmd){
cmd.Enable(!_14&&_18);
}
cmd=this.GetCommandByName("Pin");
if(cmd){
cmd.Enable(!_14&&!_18);
}
cmd=this.GetCommandByName("Close");
if(cmd){
cmd.Enable(true);
}
},IsExpanded:function(){
return this.GetState(RadDockNamespace.RadDockableObjectState.Expanded);
},IsPinned:function(){
return this.GetState(RadDockNamespace.RadDockableObjectState.Pinned);
},IsClosed:function(){
return this.GetState(RadDockNamespace.RadDockableObjectState.Closed);
},OnShowHide:function(){
var _19=this.IsVisible();
this.SetState(RadDockNamespace.RadDockableObjectState.Closed,!_19);
if(_19){
this.SetDisplayStyle();
}
if(_19&&this.needPin){
this.Pin();
this.needPin=null;
}
},Dispose:function(){
if(null!=this.Commands){
for(var i=0;i<this.Commands.length;i++){
this.Commands[i]=null;
}
}
this.Commands=null;
this.ParentDockingZone=null;
this.TitleBar=null;
this.TopGrip=null;
this.BottomGrip=null;
this.LeftGrip=null;
this.RightGrip=null;
this.ClientRow=null;
this.Manager=null;
this.DockedObjectEnabledGrips=null;
this.FloatingObjectEnabledGrips=null;
},SaveState:function(){
var _1b=new Array();
var _1c=(this.IsVisible()?this.offsetWidth:this.__offsetWidth);
var _1d=(this.IsVisible()?this.offsetHeight:this.__offsetHeight);
_1b[_1b.length]=this.style.top;
_1b[_1b.length]=this.style.left;
_1b[_1b.length]=(_1c+"px");
_1b[_1b.length]=(this.IsExpanded()?(_1d+"px"):this.ExpandedHeight);
_1b[_1b.length]=this.controlState;
_1b[_1b.length]=this.style.zIndex;
_1b[_1b.length]=(this.ParentDockingZone?this.ParentDockingZone.ServerID:"");
var _1e=new Array();
var _1f=this.Commands;
var cmd;
for(var i=0;i<_1f.length;i++){
cmd=_1f[i];
var _22=new Array();
_22[_22.length]=cmd.Name;
_22[_22.length]=cmd.IsEnabled();
_22[_22.length]=true;
_1e[_1e.length]=_22.join("!");
}
_1b[_1b.length]=_1e.join("|");
this.HiddenInput.value=_1b.join(",");
},IsDocked:function(){
return ((this.DockingMode&RadDockNamespace.RadDockingModeFlags.AlwaysDock)>0&&this.ParentDockingZone!=null);
},IsFloating:function(){
return ((this.DockingMode&RadDockNamespace.RadDockingModeFlags.NeverDock)>0&&this.ParentDockingZone==null);
},CanFloat:function(){
return ((this.DockingMode&RadDockNamespace.RadDockingModeFlags.NeverDock)>0);
},CanResize:function(){
return ((this.Behavior&RadDockNamespace.RadDockableObjectBehaviorFlags.Resize)>0);
},CanDockTo:function(_23){
if(!_23){
return false;
}
if(0==(this.DockingMode&RadDockNamespace.RadDockingModeFlags.AlwaysDock)){
return false;
}
if(RadDockingZoneTypeFlags.Custom==this.AllowedDockingZoneTypes){
var _24=_23.id;
var _25=this.AllowedDockingZones;
for(var i=0;i<_25.length;i++){
if(_24==_25[i]){
return true;
}
}
return false;
}else{
return ((this.AllowedDockingZoneTypes&_23.Type)>0);
}
},DockTo:function(_27,_28){
if(!_27){
return;
}
if(!this.CanDockTo(_27)){
return;
}
_27.Dock(this);
this.Docked();
},Undock:function(){
this.ParentDockingZone=null;
this.parentNode.removeChild(this);
var _29=document.getElementsByTagName("FORM")[0];
_29=_29?_29:document.body;
_29.appendChild(this);
this.BringToFront();
},Docked:function(_2a){
this.SetDisplayStyle();
this.SetGripsState();
this.SaveState();
var cmd=this.GetCommandByName("Pin");
if(cmd){
cmd.Enable(false);
}
cmd=this.GetCommandByName("Unpin");
if(cmd){
cmd.Enable(false);
}
if(false!=_2a){
this.FireEvent("DockStateChanged",{docked:true},this);
this.FireEvent("Dock","",this);
}
},Undocked:function(){
this.SetDisplayStyle();
this.SetGripsState();
var cmd=this.GetCommandByName("Pin");
if(cmd){
cmd.Enable(!this.IsPinned());
}
cmd=this.GetCommandByName("Unpin");
if(cmd){
cmd.Enable(this.IsPinned());
}
if(this.FloatingWidth||this.FloatingHeigth){
this.SetSize(this.FloatingWidth,this.FloatingHeigth);
this.FloatingWidth=null;
this.FloatingHeigth=null;
}
this.SaveState();
this.FireEvent("DockStateChanged",{docked:false},this);
this.FireEvent("UnDock","",this);
},SetDisplayStyle:function(){
if(this.IsClosed()){
return;
}
if(!this.IsDockingEnabled){
if(!document.all){
this.style.setProperty("clear","both","");
}
return;
}
if(this.IsDocked()){
this.style.position="";
this.style.display="";
if(this.Overlay){
this.Overlay.style.display="none";
}
if((this.ParentDockingZone.Type&RadDockingZoneTypeFlags.Horizontal)>0){
this.style.display="inline";
this.SetFloatLeft();
}else{
this.RemoveFloatLeft();
}
}else{
this.style.position="absolute";
if(this.Overlay){
this.Overlay.style.display="";
}
if(document.all){
this.style.display="";
}else{
this.style.setProperty("clear","both","");
}
this.RemoveFloatLeft();
}
},InitGrips:function(_2d){
this.TitleBar=null;
this.TopGrip=null;
this.BottomGrip=null;
this.LeftGrip=null;
this.RightGrip=null;
if(!_2d||!_2d.length){
return;
}
var _2e=0;
this.TitleBar=document.getElementById(_2d[_2e++]);
this.TopGrip=document.getElementById(_2d[_2e++]);
this.BottomGrip=document.getElementById(_2d[_2e++]);
this.LeftGrip=document.getElementById(_2d[_2e++]);
this.RightGrip=document.getElementById(_2d[_2e++]);
},SetGripVisible:function(_2f,_30){
if(_2f){
_2f.style.display=(_30?"":"none");
}
},IsGripVisible:function(_31){
return (_31.style.display!="none");
},SetGripsState:function(){
this.SetGripVisible(this.TitleBar,this.CalcGripVisibleState(RadDockableObjectGripFlags.TitleBar));
this.SetGripVisible(this.TopGrip,this.CalcGripVisibleState(RadDockableObjectGripFlags.Top));
this.SetGripVisible(this.BottomGrip,this.CalcGripVisibleState(RadDockableObjectGripFlags.Bottom));
this.SetGripVisible(this.LeftGrip,this.CalcGripVisibleState(RadDockableObjectGripFlags.Left));
this.SetGripVisible(this.RightGrip,this.CalcGripVisibleState(RadDockableObjectGripFlags.Right));
},CalcGripVisibleState:function(_32){
var _33;
if(this.IsDocked()){
_33=this.DockedObjectEnabledGrips;
if(RadDockableObjectGripFlags.Auto==_33){
switch(this.ParentDockingZone.Type){
case RadDockingZoneTypeFlags.Top:
case RadDockingZoneTypeFlags.Bottom:
case RadDockingZoneTypeFlags.Left:
case RadDockingZoneTypeFlags.Right:
return (_32==RadDockableObjectGripFlags.TitleBar);
case RadDockingZoneTypeFlags.Horizontal:
return (_32==RadDockableObjectGripFlags.Left);
case RadDockingZoneTypeFlags.Vertical:
return (_32==RadDockableObjectGripFlags.Top);
default:
return false;
}
}
}else{
_33=this.FloatingObjectEnabledGrips;
if(RadDockableObjectGripFlags.Auto==_33){
return (_32==RadDockableObjectGripFlags.TitleBar);
}
}
return ((_33&_32)>0);
},CanCollapse:function(){
return ((this.Behavior&RadDockNamespace.RadDockableObjectBehaviorFlags.Collapse)>0);
},Expand:function(_34){
if(null==_34){
_34=true;
}
if(_34==this.IsExpanded()){
return;
}
if(!_34&&this.IsExpanded()){
this.ExpandedHeight=(this.offsetHeight+"px");
}else{
if(document.all){
var _35=this.style.height;
var _36=this.ClientRow.style.display;
var _37=this.ClientRow;
var _38=_37.cells[0];
var _39=_38.firstChild.style.display;
var _3a=_38.firstChild.style.height;
var _3b=_38.firstChild.style["float"];
this.ClientRow.style.display="";
_38.firstChild.style.display="";
_38.firstChild.style.height="100%";
this.style.height=parseInt(this.ExpandedHeight)+"px";
var _3c=RadDockNamespace.FixIeHeight(this,true);
this.ExpandedHeight=parseInt(this.ExpandedHeight)-_3c;
this.style.height=_35;
this.ClientRow.style.display=_36;
_38.firstChild.style.display=_39;
_38.firstChild.style.height=_3a;
_38.firstChild.style["float"]=_3b;
}
}
this.SetState(RadDockNamespace.RadDockableObjectState.Expanded,_34);
if(this.Manager.UseEffects){
var _37=this.ClientRow;
var _38=_37.cells[0];
var _3d=this;
var _3e=function(){
_38.firstChild.style.display="none";
_37.style.display="";
};
var _3f=function(){
if(_34){
_38.firstChild.style.display="";
_38.firstChild.style.height="100%";
}
_37.style.display=_34?"":"none";
_3d.SaveState();
};
var rc=this.GetRect();
new RadEffect.Resize({object:this,height:_34?this.ExpandedHeight:this.GetRect(this.TitleBar).height,width:rc.width,beforeStart:_3e,afterFinish:_3f,duration:1/3});
}else{
this.ClientRow.style.display=_34?"":"none";
this.style.height=_34?(parseInt(this.ExpandedHeight)+"px"):"1px";
if(this.Overlay){
this.Overlay.style.height=this.style.height;
}
this.SaveState();
}
},Collapse:function(){
this.Expand(false);
},Pin:function(){
var _41=this;
var _42=0;
var _43=0;
var _44=true;
var _45=function(){
if(_44){
var rc=_41.GetRect();
_42=rc.left;
_43=rc.top;
_44=false;
}
_41.MoveTo(RadDockNamespace.GetScrollLeft()+_42,RadDockNamespace.GetScrollTop()+_43);
_41.SaveState();
};
this.timerID=setInterval(_45,10);
this.SetState(RadDockNamespace.RadDockableObjectState.Pinned,true);
},Unpin:function(){
clearInterval(this.timerID);
this.timerID=null;
this.SetState(RadDockNamespace.RadDockableObjectState.Pinned,false);
},InitCommands:function(_47){
this.Commands=new Array();
this.CommandHandlerList=new Array();
var _48,_49,id;
for(var i=0;i<_47.length;i++){
_48=_47[i];
id=_48[0];
_49=document.getElementById(id);
if(_49){
_48.splice(0,1);
RadDockNamespace.InitDockableObjectCommand(_49,this,_48);
this.Commands[this.Commands.length]=_49;
var _4c=RadDockNamespace.CommandHandlerList[_49.Name];
if(_4c&&!_49.OnCommand){
_49.OnCommand=_4c;
}
}
}
},GetCommandByName:function(_4d){
var cmd;
for(var i=0;i<this.Commands.length;i++){
cmd=this.Commands[i];
if(cmd&&cmd.Name==_4d){
return cmd;
}
}
return null;
},OnExpand:function(_50,_51){
_50.Expand();
},OnCollapse:function(_52,_53){
_52.Collapse();
},OnPin:function(_54,_55){
_54.Pin();
},OnUnpin:function(_56,_57){
_56.Unpin();
},OnClose:function(_58,_59){
_58.Hide();
},ExecuteClientEventHandler:function(obj,_5b){
var _5c=_5b.eventName;
try{
obj["On"+_5c](obj,_5b);
}
catch(e){
}
},OnDock:function(_5d,_5e){
if(this.clientEventHandlers["OnDock"]){
eval(this.clientEventHandlers["OnDock"])(_5d,_5e);
}
},OnUnDock:function(_5f,_60){
if(this.clientEventHandlers["OnUnDock"]){
eval(this.clientEventHandlers["OnUnDock"])(_5f,_60);
}
},OnDragEnd:function(_61,_62){
if(this.clientEventHandlers["OnDragEnd"]){
eval(this.clientEventHandlers["OnDragEnd"])(_61,_62);
}
},OnDockStateChanged:function(_63,_64){
if(this.clientEventHandlers["OnDockStateChanged"]){
eval(this.clientEventHandlers["OnDockStateChanged"])(_63,_64);
}
},OnDrag:function(_65,_66){
if(this.clientEventHandlers["OnDrag"]){
eval(this.clientEventHandlers["OnDrag"])(_65,_66);
}
},OnDragStart:function(_67,_68){
if(this.clientEventHandlers["OnDragStart"]){
eval(this.clientEventHandlers["OnDragStart"])(_67,_68);
}
},OnDrop:function(_69,_6a){
if(this.clientEventHandlers["OnDrop"]){
eval(this.clientEventHandlers["OnDrop"])(_69,_6a);
}
},SetFloatLeft:function(){
this.style.styleFloat="left";
},RemoveFloatLeft:function(){
this.style.styleFloat="";
if(this.style.removeAttribute){
this.style.removeAttribute("styleFloat");
}
}};
RadDockNamespace.CommandHandlerList=[];
RadDockNamespace.CommandHandlerList["Expand"]=RadDockNamespace.RadDockableObject.OnExpand;
RadDockNamespace.CommandHandlerList["Collapse"]=RadDockNamespace.RadDockableObject.OnCollapse;
RadDockNamespace.CommandHandlerList["Close"]=RadDockNamespace.RadDockableObject.OnClose;
RadDockNamespace.CommandHandlerList["Pin"]=RadDockNamespace.RadDockableObject.OnPin;
RadDockNamespace.CommandHandlerList["Unpin"]=RadDockNamespace.RadDockableObject.OnUnpin;;if("undefined"==typeof (RadDockNamespace)){
RadDockNamespace=new Object();
}
RadDockNamespace.InitDockableObjectCommand=function(_1,_2,_3){
if(!_1){
return;
}
Object.Extend(_1,RadDockNamespace.RadDockableObjectCommand);
_1.ParentDockableObject=_2;
var i=0;
_1.Name=_3[i++];
_1.PostBackClientEventString=_3[i++];
_1.OnCommand=eval(_3[i++]);
};
RadDockNamespace.RadDockableObjectCommand={ParentDockableObject:null,Name:"",PostBackClientEventString:"",OnCommand:null,Dispose:function(){
this.onclick=null;
this.ParentDockableObject=null;
},onclick:function(_5){
this.Exec();
},Exec:function(){
if(!this.ParentDockableObject){
return;
}
var _6=true;
var _7=this.OnCommand;
if("function"==typeof (_7)){
_6=(false!=_7(this.ParentDockableObject,this));
}
if(_6&&this.PostBackClientEventString){
eval(this.PostBackClientEventString);
}
},Enable:function(_8){
this.style.display=((_8!=false)?"":"none");
},IsEnabled:function(){
return (this.style.display!="none");
}};;if("undefined"==typeof (RadDockNamespace)){
RadDockNamespace=new Object();
}
RadDockNamespace.RadDock_ActiveObject=null;
RadDockNamespace.RadDock_DragStartFired=false;
RadDockNamespace.RadDock_ActiveDockingZone=null;
RadDockNamespace.RadDock_DragHelper=null;
RadDockNamespace.RadDock_DragHelperTooltip=null;
RadDockNamespace.RadDock_HoverDockableObject=null;
RadDockNamespace.RadDock_Action="";
RadDockNamespace.RadDock_MouseX=0;
RadDockNamespace.RadDock_MouseY=0;
RadDockNamespace.RadDock_IsActiveObjectDragged=false;
RadDockNamespace.RadDock_MouseMove=function(_1){
if(!_1){
_1=window.event;
}
if(RadDockNamespace.RadDock_ActiveObject){
RadDockNamespace.RadDock_DragActiveObject(_1);
RadDockNamespace.RadDock_IsActiveObjectDragged=true;
}else{
if(!this.effect){
var o=RadDockNamespace.RadDock_GetHoverdDockableObjectAction(_1);
RadDockNamespace.RadDock_HoverDockableObject=o?o.hoveredObject:null;
RadDockNamespace.RadDock_Action=o?o.action:null;
RadDockNamespace.RadDock_SetCursor(RadDockNamespace.RadDock_Action);
}
}
};
RadDockNamespace.RadDock_MouseDown=function(_3){
if(!_3){
_3=window.event;
}
if(RadDockNamespace.RadDock_HoverDockableObject){
RadDockNamespace.RadDock_StartDrag(RadDockNamespace.RadDock_HoverDockableObject,_3);
if(_3.preventDefault){
_3.preventDefault();
}
_3.returnValue=false;
}
};
RadDockNamespace.RadDock_MouseUp=function(_4){
if(!_4){
_4=window.event;
}
if(RadDockNamespace.RadDock_ActiveObject){
var _5=RadDockNamespace.RadDock_ActiveObject;
if(_5.FireEvent){
_5.FireEvent("DragEnd","",_5);
}
RadDockNamespace.RadDock_EndDrag(_4);
}
};
RadDockNamespace.RadDock_KeyDown=function(_6){
if(!_6){
_6=window.event;
}
if(27==_6.keyCode){
if(RadDockNamespace.RadDock_ActiveObject){
RadDockNamespace.RadDock_CancelDrag(_6);
}
if(this.effect){
this.effect.SetToStop();
}
}
};
RadDockNamespace.RadDock_StartDrag=function(_7,_8){
RadDockNamespace.RadDock_IsActiveObjectDragged=false;
RadDockNamespace.RadDock_ActiveObject=_7;
RadDockNamespace.RadDock_DragStartFired=false;
RadDockNamespace.RadDock_MouseX=_8.clientX;
RadDockNamespace.RadDock_MouseY=_8.clientY;
if(RadDockNamespace.RadDock_ActiveObject.IsFloating()){
RadDockNamespace.RadDock_ActiveObject.BringToFront();
}
};
RadDockNamespace.RadDock_EndDrag=function(_9){
if(RadDockNamespace.RadDock_IsActiveObjectDragged){
RadDockNamespace.RadDock_IsActiveObjectDragged=false;
var _a=RadDockNamespace.RadDock_ActiveObject;
var _b=_a.Manager.UseEffects;
if(_b){
RadDockNamespace.RadDock_EndDragWithEffects(_9);
}else{
RadDockNamespace.RadDock_EndDragNoEffect(_9);
}
if(_a.FireEvent){
_a.FireEvent("Drop","",_a);
}
}
RadDockNamespace.RadDock_CancelDrag(_9);
};
RadDockNamespace.RadDock_EndDragNoEffect=function(_c){
var _d=RadDockNamespace.RadDock_ActiveDockingZone;
var _e=RadDockNamespace.RadDock_ActiveObject;
var _f=_e.ParentDockingZone;
var _10,_11;
if(_d){
_d.Dock(_e);
_10=_e;
}else{
if(RadDockNamespace.RadDock_DragHelper){
var rc=RadDockNamespace.RadDock_DragHelper.GetRect();
if("move"==RadDockNamespace.RadDock_Action&&_e.IsDocked()){
if(!_e.CanFloat()){
return;
}
_e.Undock();
_11=_e;
}
_e.SetRect(rc);
}
}
_e.SaveState();
if(_f){
_f.SaveState();
}
if(_10){
_10.Docked();
}else{
if(_11){
_11.Undocked();
}
}
_10=null;
_11=null;
};
RadDockNamespace.RadDock_EndDragWithEffects=function(_13){
var _14=RadDockNamespace.RadDock_ActiveDockingZone;
var _15=RadDockNamespace.RadDock_ActiveObject;
var _16=RadDockNamespace.RadDock_Action;
var _17=_15.ParentDockingZone;
var _18,_19;
var _1a,_1b,_1c;
var _1d,_1e;
_1d=_15.GetRect();
if("move"==_16&&_15.ParentDockingZone){
_1a=RadDockNamespace.RadDock_CreateSubstObject(_15,false);
}
if(_14){
_14.Dock(_15);
_15.Docked(false);
_18=_15;
_1e=_15.GetRect();
_1b=RadDockNamespace.RadDock_CreateSubstObject(_15,true);
_1c=_15.cloneNode(3);
document.body.appendChild(_1c);
_1c.style.position="absolute";
_1c.style.display="";
_1c.GetRect=_15.GetRect;
_1c.SetRect=_15.SetRect;
_1c.SetSize=_15.SetSize;
_1c.Overlay=_15.Overlay;
_1c.MoveTo=_15.MoveTo;
_15.style.display="none";
}else{
if("move"==_16&&_15.IsDocked()){
if(!_15.CanFloat()){
if(_1a){
_1a.parentNode.removeChild(_1a);
_1a=null;
}
return;
}
_15.Undock();
_19=_15;
}
_1c=_15;
_1e=RadDockNamespace.RadDock_DragHelper.GetRect();
}
if("move"==_16){
_1c.SetRect(_1d);
}
var i=0;
var _20=new Array();
if(_1a){
_20[i++]=new RadEffect.Resize({object:_1a,width:0,height:0,sync:true});
}
if(_1b){
_20[i++]=new RadEffect.Resize({object:_1b,width:_1e.width,height:_1e.height,sync:true});
}
if(!("move"!=_16&&_15.ParentDockingZone)){
_20[i++]=new RadEffect.MoveTo({object:_1c,destX:_1e.left,destY:_1e.top,sync:true});
}
if("move"!=_16){
var h=_1c.style.height;
var _22=RadDockNamespace.FixIeHeight(_1c);
_1c.style.height=h;
_20[i++]=new RadEffect.Resize({object:_1c,width:_1e.width,height:_1e.height-_22,sync:true});
}
var _23=this;
var _24=function(){
if(_1a){
_1a.parentNode.removeChild(_1a);
_1a=null;
}
if(_1b){
_1b.parentNode.removeChild(_1b);
_1b=null;
}
if(_1c&&_1c!=_15){
_1c.parentNode.removeChild(_1c);
_1c=null;
}
if(_18){
_18.Docked();
}else{
if(_19){
_19.Undocked();
}
}
if(_17){
_17.SaveState();
_17=null;
}
if(_14){
_14.SaveState();
_14=null;
}
_15.SaveState();
_18=null;
_19=null;
_23.effect=null;
RadDockNamespace.RadDock_SetCursor("default");
};
var _25=_15.Manager;
this.effect=new RadEffect.Parallel({fps:_25.EffectsUpdateSpeed,duration:_25.EffectsDuration,beforeStart:function(){
RadDockNamespace.RadDock_SetCursor("wait");
},afterFinish:_24,effects:_20});
};
RadDockNamespace.RadDock_CreateSubstObject=function(_26,_27){
var _28;
if(document.all){
_28=document.createElement("span");
}else{
_28=document.createElement("table");
_28.insertRow(0).insertCell(0);
}
var rc=_26.GetRect();
_28.style.width=(_27?0:rc.width)+"px";
_28.style.height=(_27?0:rc.height)+"px";
_28.style.font="normal 1px arial";
_26.parentNode.insertBefore(_28,_26);
return _28;
};
RadDockNamespace.RadDock_CancelDrag=function(_2a){
RadDockNamespace.RadDock_HideDragHelper();
if(RadDockNamespace.RadDock_ActiveDockingZone){
RadDockNamespace.RadDock_ActiveDockingZone.Highlight(false);
}
RadDockNamespace.RadDock_ActiveObject=null;
RadDockNamespace.RadDock_DragStartFired=false;
RadDockNamespace.RadDock_ActiveDockingZone=null;
RadDockNamespace.RadDock_MouseX=null;
RadDockNamespace.RadDock_MouseY=null;
if(RadDockNamespace.RadDock_DragHelperTooltip){
RadDockNamespace.RadDock_DragHelperTooltip.Hide();
RadDockNamespace.RadDock_DragHelperTooltip=null;
}
};
RadDockNamespace.RadDock_DragActiveObject=function(_2b){
var dx=_2b.clientX-RadDockNamespace.RadDock_MouseX;
var dy=_2b.clientY-RadDockNamespace.RadDock_MouseY;
var _2e=RadDockNamespace.RadDock_ActiveObject;
var _2f=_2e.Manager;
if(!RadDockNamespace.RadDock_DragHelper){
RadDockNamespace.RadDock_DragHelper=RadDockNamespace.RadDock_InitDragHelper(_2e);
}
if(_2f.ShowToolTipWhileDragging&&!RadDockNamespace.RadDock_DragHelperTooltip){
RadDockNamespace.RadDock_DragHelperTooltip=RadDockNamespace.RadDock_InitDragHelperTooltip(_2e);
}
var _30=RadDockNamespace.RadDock_DragHelper;
if("move"==RadDockNamespace.RadDock_Action){
_30.MoveBy(dx,dy);
var _31=_30.GetRect();
RadDockNamespace.RadDock_ActiveDockingZone=_2f.DockingZoneHitTest(_2e,_2b,_31);
RadDockNamespace.RadDock_ScrollDocumentToFit(_31);
RadDockNamespace.RadDock_ShowTooltip(_2b,_31,true);
if(_2e.FireEvent){
if(!RadDockNamespace.RadDock_DragStartFired){
RadDockNamespace.RadDock_DragStartFired=true;
_2e.FireEvent("DragStart","",_2e);
}
_2e.FireEvent("Drag","",_2e);
}
}else{
var _32=_30.childNodes[0];
if(_32){
_32.style.width="1px";
_32.style.height="1px";
}
_30.Resize(RadDockNamespace.RadDock_Action,dx,dy);
var _31=_30.GetRect();
if(_32){
_32.style.width=_31.width+"px";
_32.style.height=_31.height+"px";
RadDockNamespace.FixIeHeight(_32);
}
RadDockNamespace.RadDock_ShowTooltip(_2b,_31,false);
}
RadDockNamespace.RadDock_MouseX=_2b.clientX;
RadDockNamespace.RadDock_MouseY=_2b.clientY;
};
RadDockNamespace.RadDock_ShowTooltip=function(_33,_34,_35){
var _36=RadDockNamespace.RadDock_DragHelperTooltip;
if(_36){
if(_35){
_36.SetText("("+_34.left+","+_34.top+")");
}else{
_36.SetText("("+_34.width+" x "+_34.height+")");
}
var x=_33.clientX+5+RadDockNamespace.GetScrollLeft();
var y=_33.clientY+5+RadDockNamespace.GetScrollTop();
_36.ShowAt(x,y);
}
};
RadDockNamespace.RadDock_InitDragHelper=function(_39){
var _3a=_39.Manager;
if(!_3a.DragHelper){
return;
}
if(!_3a.IsDragHelperMoved&&document.all){
var _3b=_3a.DragHelper.cloneNode(true);
document.body.appendChild(_3b);
_3a.DragHelper.parentNode.removeChild(_3a.DragHelper);
_3a.IsDragHelperMoved=true;
_3a.DragHelper=_3b;
}
var _3c=_3a.DragHelper;
if(!_3c){
return null;
}
_3c.innerHTML="";
if(_3a.ShowContentWhileDragging){
var rc=_39.GetRect();
var _3e=_39.cloneNode(true);
_3e.style.position="";
_3e.style.display="";
_3c.appendChild(_3e);
}
_3c.style.position="absolute";
_3c.Show();
_3c.SetRect(_39.GetRect());
_3c.BringToFront();
return _3c;
};
RadDockNamespace.RadDock_InitDragHelperTooltip=function(_3f){
var _40=_3f.Manager;
if(!_40.DragHelperTooltip){
return;
}
if(!_40.IsDragTooltipMoved&&document.all){
var _41=_40.DragHelperTooltip.cloneNode(true);
document.body.appendChild(_41);
_40.DragHelperTooltip.parentNode.removeChild(_40.DragHelperTooltip);
_40.IsDragTooltipMoved=true;
_40.DragHelperTooltip=_41;
}
var _42=_40.DragHelperTooltip;
if(!_42){
return null;
}
_42.BringToFront();
return _42;
};
RadDockNamespace.RadDock_HideDragHelper=function(){
var _43=RadDockNamespace.RadDock_DragHelper;
if(_43){
_43.Hide();
_43.innerHTML="";
}
RadDockNamespace.RadDock_DragHelper=null;
};
RadDockNamespace.RadDock_GetHoverdDockableObjectAction=function(_44){
var _45=null;
var _46="";
var _47=RadDockNamespace.GetEventSrcElement(_44);
switch(_47.className){
case "RadDockableObjectResizeable":
_46="resize";
break;
case "RadDockableObjectTitleBar":
case "RadDockableObjectTitle":
case "RadDockableObjectHorizontalGrip":
case "RadDockableObjectVerticalGrip":
_46="move";
break;
default:
return null;
}
_45=RadDockNamespace.RadDock_GetParentDockableObject(_47);
if(!_45){
return null;
}
if(_45.IsPinned()){
return null;
}
if(_45.DockingMode==RadDockNamespace.RadDockingModeFlags.Disabled){
return null;
}
if(_45.ParentDockingZone&&!_45.ParentDockingZone.DockEnabled){
return null;
}
if("resize"==_46){
if(!_45.IsExpanded()){
return null;
}
if(!_45.CanResize()){
return null;
}
var _48=_45.ParentDockingZone;
if(_48&&_48.FixedSizeMode){
return null;
}
_46=_45.CalcResizeDir(_44);
}
return {hoveredObject:_45,action:_46};
};
RadDockNamespace.RadDock_GetParentDockableObject=function(_49){
var _4a=_49;
while(_4a&&_4a.parentNode){
switch(_4a.className){
case "RadDockableObjectResizeable":
case "RadDockableObjectFixed":
return (_4a.IsDockableObject?_4a:null);
}
_4a=_4a.parentNode;
}
return null;
};
RadDockNamespace.RadDock_SetCursor=function(_4b,_4c){
if(!_4b){
_4b="";
}
if(!_4c||!_4c.style){
_4c=document.body;
}
if(_4c.style.cursor!=_4b){
_4c.style.cursor=_4b;
}
};
RadDockNamespace.RadDock_ScrollDocumentToFit=function(_4d){
var cy=0;
var _4f=RadDockNamespace.GetScrollTop();
var _50=(document.body.clientHeight+_4f);
if(_4d.top<_4f){
cy=-((_4f-_4d.top)+1);
}else{
if(_4d.bottom>_50){
cy=(_4d.bottom-_50)+1;
}
}
var cx=0;
var _52=RadDockNamespace.GetScrollLeft();
var _53=(document.body.clientWidth+_52);
if(_4d.left<_52){
cx=-((_52-_4d.left)+1);
}else{
if(_4d.right>_53){
cx=(_4d.right-_53)+1;
}
}
window.scrollBy(cx,cy);
};
RadDock_GetDockingZone=function(id){
var el=document.getElementById(id);
return (el.IsDockingZone?el:null);
};
RadDock_GetDockableObject=function(id){
var el=document.getElementById(id);
if(el==null){
return null;
}
return (el.IsDockableObject?el:null);
};;if("undefined"==typeof (RadDockNamespace)){
RadDockNamespace=new Object();
}
RadDockNamespace.RadDockingManager=function(_1,_2,_3,_4,_5,_6,_7,_8,_9,_a,_b){
this.Document=document;
this.ShowContentWhileDragging=_6;
this.ShowToolTipWhileDragging=_a;
this.DockingZones=new Array();
this.DockableObjects=new Array();
this.zoneIndexTable=[];
this.objectIndexTable=[];
this.UseEffects=_7;
this.EffectsUpdateSpeed=_8;
this.EffectsDuration=_9;
this.DockingZonesCount=_b;
this.RegisterDockingZones(_2);
this.RegisterDockableObjects(_3);
this.SaveDockingZonesState();
this.InitDragHelper(_4,_5);
this.EnableDocking(_1);
var _c=this;
var _d=function(){
if(_c){
_c.Dispose();
}
};
RadDockNamespace.AttachEvent(window,"unload",_d);
};
RadDockNamespace.RadDockingManager.prototype.InitDragHelper=function(_e,_f){
var _10=document.getElementById(_e);
if(_10){
RadDockNamespace.InitMoveableObject(_10);
if(document.all){
_10.style.filter="progid:DXImageTransform.Microsoft.Alpha(opacity=25)";
}else{
_10.style.setProperty("-moz-opacity","0.25","");
}
}
this.DragHelper=_10;
var _11=document.getElementById(_f);
if(_11){
RadDockNamespace.RadTooltip_Init(_11);
}
this.DragHelperTooltip=_11;
};
RadDockNamespace.RadDockingManager.prototype.RegisterDockingZones=function(_12){
for(var i=0;i<_12.length;i++){
this.RegisterDockingZone(_12[i]);
}
};
RadDockNamespace.RadDockingManager.prototype.SaveDockingZonesState=function(){
for(var i=0;i<this.DockingZones.length;i++){
this.DockingZones[i].SaveState();
}
};
RadDockNamespace.RadDockingManager.prototype.RegisterDockableObjects=function(_15){
for(var i=0;i<_15.length;i++){
this.RegisterDockableObject(_15[i]);
}
};
RadDockNamespace.RadDockingManager.prototype.RegisterDockingZone=function(_17){
if(!_17||!_17.length){
return;
}
var id=_17[0];
var _19=this.Document.getElementById(id);
if(!_19){
return;
}
RadDockNamespace.RadDockingZone_Init(_19,_17.slice(1));
_19.Manager=this;
var _1a=this.zoneIndexTable[_19.id];
if(null==_1a){
_1a=this.DockingZones.length;
}
this.DockingZones[_1a]=_19;
this.zoneIndexTable[_19.id]=_1a;
};
RadDockNamespace.RadDockingManager.prototype.RegisterDockableObject=function(_1b){
if(!_1b||!_1b.length){
return;
}
var id=_1b[0];
var _1d=this.Document.getElementById(id);
if(!_1d){
return;
}
RadDockNamespace.InitDockableObject(_1d,_1b.slice(1));
_1d.Manager=this;
var _1e=this.objectIndexTable[_1d.id];
if(null==_1e){
_1e=this.DockableObjects.length;
}
this.DockableObjects[_1e]=_1d;
this.objectIndexTable[_1d.id]=_1e;
this.SaveDockingZonesState();
};
RadDockNamespace.RadDockingManager.prototype.EnableDocking=function(_1f){
this.DockEnabled=_1f;
if(this.DockEnabled){
RadDockNamespace.AttachEvent(this.Document,"mousemove",RadDockNamespace.RadDock_MouseMove);
RadDockNamespace.AttachEvent(this.Document,"mousedown",RadDockNamespace.RadDock_MouseDown);
RadDockNamespace.AttachEvent(this.Document,"mouseup",RadDockNamespace.RadDock_MouseUp);
RadDockNamespace.AttachEvent(this.Document,"keydown",RadDockNamespace.RadDock_KeyDown);
}else{
RadDockNamespace.DetachEvent(this.Document,"mousemove",RadDockNamespace.RadDock_MouseMove);
RadDockNamespace.DetachEvent(this.Document,"mousedown",RadDockNamespace.RadDock_MouseDown);
RadDockNamespace.DetachEvent(this.Document,"mouseup",RadDockNamespace.RadDock_MouseUp);
RadDockNamespace.DetachEvent(this.Document,"keydown",RadDockNamespace.RadDock_KeyDown);
}
};
RadDockNamespace.RadDockingManager.prototype.DockingZoneHitTest=function(_20,_21,_22){
var _23;
var _24;
for(var i=0;i<this.DockingZones.length;i++){
_23=this.DockingZones[i];
if(_23.HitTest(_20,_21,_22)){
_24=_23;
}
}
for(var i=0;i<this.DockingZones.length;i++){
_23=this.DockingZones[i];
_23.Highlight(_23==_24);
}
return _24;
};
RadDockNamespace.RadDockingManager.prototype.Dispose=function(){
for(var i=0;i<this.DockableObjects.length;i++){
this.DockableObjects[i].Dispose();
this.DockableObjects[i]=null;
}
for(var i=0;i<this.DockingZones.length;i++){
this.DockingZones[i].Dispose();
this.DockingZones[i]=null;
}
this.DockableObjects=null;
this.DockingZones=null;
this.Document=null;
this.DragHelper=null;
RadDock_ActiveObject=null;
RadDock_ActiveDockingZone=null;
RadDock_DragHelper=null;
RadDock_HoverDockableObject=null;
};
RadDockNamespace.RadDockingManager.prototype.SaveState=function(){
var o;
for(var i=0;i<this.DockableObjects.length;i++){
o=this.DockableObjects[i];
if(o&&o.SaveState){
o.SaveState();
}
}
for(var i=0;i<this.DockingZones.length;i++){
o=this.DockingZones[i];
if(o&&o.SaveState){
o.SaveState();
}
}
};;if("undefined"==typeof (RadDockNamespace)){
RadDockNamespace=new Object();
}
var RadDockingZoneTypeFlags={Vertical:1,Horizontal:2,Top:(16|2),Bottom:(32|2),Left:(48|1),Right:(64|1),All:(18|34|49|65),Custom:255};
RadDockNamespace.RadDockingZoneFixedSizeFlags={None:0,ByWidth:1,ByHeight:2,ByWidthHeight:(1|2)};
RadDockNamespace.RadDockingZone_Init=function(_1,_2){
Object.Extend(_1,RadDockNamespace.RadDockingZone);
var i=0;
_1.Type=_2[i++];
_1.DockEnabled=_2[i++];
_1.HighlightedStyleText=_2[i++];
_1.ServerID=_2[i++];
var _4=_2[i++];
_1.FixedSizeMode=_2[i++];
_1.HiddenInput=document.getElementById(_4);
if(_1.tagName=="TABLE"&&_1.rows.length&&_1.rows[0].cells.length){
_1.rows[0].cells[0].style.verticalAlign="top";
}
RadDockNamespace.AttachEvent(window,"load",_1.FixObjectSizeOnInit.Bind(_1));
};
RadDockNamespace.RadDockingZone={IsDockingZone:true,Type:null,DockEnabled:null,HighlightedStyleText:null,ServerID:null,FixedSizeMode:null,HiddenInput:null,Dispose:function(){
this.HoveredElement=null;
this.Manager=null;
this.HiddenInput=null;
},GetContainer:function(){
return ((this.tagName=="TABLE")?this.rows[0].cells[0]:this);
},GetDockedObjects:function(){
var _5=new Array();
var _6;
var _7=this.GetContainer();
var _8=_7.childNodes;
for(var i=0;i<_8.length;i++){
_6=_8[i];
if(!_6.IsDockableObject){
continue;
}
_5[_5.length]=_6;
}
return _5;
},SetAtPosition:function(_a,_b){
if(!_a){
return;
}
if(_a.ParentDockingZone!=this){
return;
}
var _c;
var _d=this.GetContainer();
var _e=_d.childNodes;
var _f=new Array();
var _10=-1;
for(var i=0;i<_e.length;i++){
_c=_e[i];
if(!_c.IsDockableObject){
continue;
}
if(_c==_a){
_10=_f.length;
}
_f[_f.length]=_c;
}
var _12=_b-_10;
if(1==_12){
_b+=1;
}
if(_f.length==_b){
_d.appendChild(_a);
}else{
var _13=_f[_b];
if(_13){
_d.insertBefore(_a,_13);
}
}
this.SaveState();
},GetPosition:function(_14){
if(!_14){
return -1;
}
if(_14.ParentDockingZone!=this){
return -1;
}
var _15;
var pos=-1;
var _17=this.GetContainer().childNodes;
for(var i=0;i<_17.length;i++){
_15=_17[i];
if(!_15.IsDockableObject){
continue;
}
pos++;
if(_15==_14){
break;
}
}
return pos;
},Dock:function(_19,_1a){
if(!_19){
return;
}
this.AdjustObjectSize(_19);
_19.ParentDockingZone=this;
_19.parentNode.removeChild(_19);
var _1b=this.GetContainer();
if(this.HoveredElement&&this.HoveredElement!=this){
if(!_1a){
_1a=this.HoveredElement;
}
_1b.insertBefore(_19,_1a);
}else{
_1b.appendChild(_19);
}
this.Highlight(false);
_1b=null;
this.SaveState();
},AdjustObjectSize:function(_1c){
if(this.FixedSizeMode){
var rc=_1c.GetRect();
if(!_1c.FloatingWidth){
_1c.FloatingWidth=rc.width;
}
if(_1c.Expanded){
if(!_1c.FloatingHeigth){
_1c.FloatingHeigth=rc.height;
}
}
if(!this.FixedWidth||!this.FixedHeight){
var _1e=this.GetRect();
this.FixedWidth=_1e.width;
this.FixedHeight=_1e.height;
}
}else{
if(_1c.FloatingWidth||_1c.FloatingHeigth){
_1c.SetSize(_1c.FloatingWidth,_1c.FloatingHeigth);
_1c.FloatingWidth=null;
_1c.FloatingHeigth=null;
}
}
var _1f=false;
if((this.FixedSizeMode&RadDockNamespace.RadDockingZoneFixedSizeFlags.ByWidth)>0){
_1f=true;
_1c.style.width=(this.FixedWidth+"px");
}
if((this.FixedSizeMode&RadDockNamespace.RadDockingZoneFixedSizeFlags.ByHeight)>0){
if(_1c.Expanded){
_1c.style.height=(this.FixedHeight+"px");
}else{
_1c.ExpandedHeight=(this.FixedHeight+"px");
}
}
},FixObjectSizeOnInit:function(){
var arr=this.GetDockedObjects();
for(var i=0;i<arr.length;i++){
this.AdjustObjectSize(arr[i]);
}
},GetRect:function(_22){
return RadGetElementRect(_22||this);
},HitTest:function(_23,_24,_25){
this.HoveredElement=null;
if(!this.DockEnabled){
return false;
}
var _26=this.GetRect();
if(!_25){
_25=_23.GetRect();
}
var x=RadDockNamespace.GetScrollLeft()+_24.clientX;
var y=RadDockNamespace.GetScrollTop()+_24.clientY;
var _29=_26.PointInRect(x,y)&&_23.CanDockTo(this);
if(_29){
this.HoveredElement=this;
var _2a=(this.tagName=="TABLE")?this.rows[0].cells[0]:this;
var _2b,_2c;
for(var i=0;i<_2a.childNodes.length;i++){
_2b=_2a.childNodes[i];
if(_2b.IsDockableObject&&_2b.IsDockingEnabled&&_23!=_2b){
_2c=(_2b.GetRect().PointInRect(x,y));
if(_2c){
this.HoveredElement=_2b;
break;
}
}
}
_2a=null;
}
return _29;
},Highlight:function(_2e){
var _2f=(this.tagName=="TABLE")?this.rows[0].cells[0]:this;
_2e=(_2e!=false);
if(_2e){
var _30=this.HoveredElement;
this.HighlightElement(this,this==_30);
var _31;
for(var i=0;i<_2f.childNodes.length;i++){
_31=_2f.childNodes[i];
if(_31.Undock){
this.HighlightElement(_31,_31==_30);
}
}
}else{
if(!_2e&&this.IsHighlighted){
this.HighlightElement(this,false);
var _31;
for(var i=0;i<_2f.childNodes.length;i++){
_31=_2f.childNodes[i];
if(_31.Undock){
this.HighlightElement(_31,false);
}
}
}
}
this.IsHighlighted=_2e;
_2f=null;
},HighlightElement:function(_33,_34){
_34=(_34!=false);
if(_34&&null==_33.oldStyleText){
_33.oldStyleText=_33.style.cssText;
_33.style.cssText+=(";"+this.HighlightedStyleText);
}else{
if(!_34&&null!=_33.oldStyleText){
_33.style.cssText=_33.oldStyleText;
_33.oldStyleText=null;
}
}
},SaveState:function(){
var _35=new Array();
var _36=this.GetContainer().childNodes;
var _37;
for(var i=0;i<_36.length;i++){
_37=_36[i];
if(!_37.IsDockableObject){
continue;
}
_35[_35.length]=_37.ServerID;
}
this.HiddenInput.value=_35.join(",");
},ExpandAll:function(_39){
var _3a=this.GetContainer();
var _3b;
for(var i=0;i<_3a.childNodes.length;i++){
_3b=_3a.childNodes[i];
if(_3b.IsDockableObject&&_3b.CanCollapse()){
_3b.Expand(_39);
}
}
},CollapseAllObjects:function(){
this.ExpandAll(false);
},ExpandAllObjects:function(){
this.ExpandAll(true);
}};;if("undefined"==typeof (RadDockNamespace)){
RadDockNamespace=new Object();
}
Object.Extend=function(_1,_2){
for(var _3 in _2){
_1[_3]=_2[_3];
}
};
RadDockNamespace.AttachEvent=function(_4,_5,_6){
if(_4.attachEvent){
_4.attachEvent("on"+_5,_6);
}else{
if(_4.addEventListener){
_4.addEventListener(_5,_6,false);
}
}
};
RadDockNamespace.DetachEvent=function(_7,_8,_9){
if(_7.detachEvent){
_7.detachEvent("on"+_8,_9);
}else{
if(_7.removeEventListener){
_7.removeEventListener(_8,_9,false);
}
}
};
RadDockNamespace.GetEventSrcElement=function(_a){
if(!_a){
return null;
}
if(_a.srcElement){
return _a.srcElement;
}
if(_a.target){
return _a.target;
}
return null;
};
RadDockNamespace.GetScrollTop=function(){
if(document.documentElement&&document.documentElement.scrollTop){
return document.documentElement.scrollTop;
}else{
return document.body.scrollTop;
}
};
RadDockNamespace.GetScrollLeft=function(){
if(document.documentElement&&document.documentElement.scrollLeft){
return document.documentElement.scrollLeft;
}else{
return document.body.scrollLeft;
}
};
RadDockNamespace.ParseInt=function(_b,_c){
if(!_c){
_c=0;
}
var _d=parseInt(_b);
return (isNaN(_d)?_c:_d);
};
RadDockNamespace.FixIeHeight=function(_e){
var _f=0;
if(document.all&&"CSS1Compat"==document.compatMode){
var _10=_e.offsetHeight;
_f=(_10-parseInt(_e.style.height));
if(_f>0){
var _11=(parseInt(_e.style.height)-_f);
if(_11>0){
_e.style.height=_11+"px";
}
}
}
return _f;
};
if(typeof (RadControlsNamespace)=="undefined"){
RadControlsNamespace={};
}
RadControlsNamespace.AppendStyleSheet=function(_12,_13,_14){
if(!_14){
return;
}
if(!_12){
document.write("<"+"link"+" rel='stylesheet' type='text/css' href='"+_14+"' />");
}else{
var _15=document.createElement("LINK");
_15.rel="stylesheet";
_15.type="text/css";
_15.href=_14;
document.getElementById(_13+"StyleSheetHolder").appendChild(_15);
}
};;Function.prototype.Extends=function(_1){
for(var _2 in _1.prototype){
this.prototype[_2]=_1.prototype[_2];
}
return this;
};
Function.prototype.Bind=function(_3){
var _4=this;
return function(){
_4.apply(_3,arguments);
};
};
RadEffect={};
RadEffect.Create=function(){
return function(){
this.Initialize.apply(this,arguments);
};
};
RadEffect.Base=function(){
};
RadEffect.Base.prototype.Initialize=function(){
var _5=arguments[0]||{};
_5.fps=_5.fps||25;
_5.duration=_5.duration||1;
this.Params=_5;
if(!_5.sync){
this.Start();
}
};
RadEffect.Base.prototype.Start=function(){
var _6=this.Params;
if(this.OnStart){
this.OnStart();
}
if(_6.beforeStart){
_6.beforeStart();
}
this.Frame=0;
this.StartTime=new Date().getTime();
this.EndTime=this.StartTime+(_6.duration*1000);
this.Loop();
};
RadEffect.Base.prototype.SetToStop=function(){
this.toStop=true;
};
RadEffect.Base.prototype.Loop=function(){
var _7=this.Params;
var _8=this.toStop;
var _9=new Date().getTime();
var _a=Math.round((_9-this.StartTime)*_7.fps/1000);
if(_a>this.Frame&&!_8){
this.Frame=_a;
var _b=Math.min((_9-this.StartTime)/(this.EndTime-this.StartTime),1);
_8=this.Update(_b)||(_9>=this.EndTime);
}
if(!_8){
this.Timer=setTimeout(this.Loop.Bind(this),20);
}else{
this.Stopped();
}
};
RadEffect.Base.prototype.Update=function(_c){
return true;
};
RadEffect.Base.prototype.Stopped=function(){
if(this.Params.afterFinish){
this.Params.afterFinish();
}
};
RadEffect.Batch=RadEffect.Create();
RadEffect.Batch.Extends(RadEffect.Base);
RadEffect.Batch.prototype.OnStart=function(){
this.Params.duration=100000;
this.startEffect=true;
this.effectIndex=0;
};
RadEffect.Batch.prototype.Update=function(_d){
var _e=this.Params.effects;
if(this.startEffect){
effect=_e[this.effectIndex];
if(effect){
effect.Start();
effect.Params.afterFinish=this.StartNext.Bind(this);
this.startEffect=false;
this.effectIndex++;
}
}
return (this.startEffect&&null==_e[this.effectIndex]);
};
RadEffect.Batch.prototype.StartNext=function(){
this.startEffect=true;
};
RadEffect.Parallel=RadEffect.Create();
RadEffect.Parallel.Extends(RadEffect.Base);
RadEffect.Parallel.prototype.OnStart=function(){
var _f=this.Params.effects;
for(var i=0;i<_f.length;i++){
_f[i].OnStart();
}
};
RadEffect.Parallel.prototype.Update=function(pos){
var _12=this.Params.effects;
for(var i=0;i<_12.length;i++){
_12[i].Update(pos);
}
};
RadEffect.MoveTo=RadEffect.Create();
RadEffect.MoveTo.Extends(RadEffect.Base);
RadEffect.MoveTo.prototype.OnStart=function(){
var _14=this.Params;
this.object=_14.object;
this.object.position="absolute";
var _15=_14.object.GetRect();
this.X=_15.left;
this.Y=_15.top;
this.destX=(null!=_14.destX?_14.destX:_15.left);
this.destY=(null!=_14.destY?_14.destY:_15.top);
this.diffX=(this.destX-this.X);
this.diffY=(this.destY-this.Y);
};
RadEffect.MoveTo.prototype.Update=function(pos){
var _17=this.Params;
var _18=this.object;
var x=this.X+pos*this.diffX;
var y=this.Y+pos*this.diffY;
_18.style.left=x+"px";
_18.style.top=y+"px";
if(_18.Overlay){
_18.Overlay.style.left=_18.style.left;
_18.Overlay.style.top=_18.style.top;
}
};
RadEffect.Resize=RadEffect.Create();
RadEffect.Resize.Extends(RadEffect.Base);
RadEffect.Resize.prototype.OnStart=function(){
var _1b=this.Params;
var _1c=_1b.object;
this.width=parseInt(_1c.style.width||_1c.clientWidth);
this.height=parseInt(_1c.style.height||_1c.clientHeight);
_1b.width=parseInt(null!=_1b.width?_1b.width:this.width);
_1b.height=parseInt(null!=_1b.height?_1b.height:this.height);
if(_1b.width<0){
_1b.width=0;
}
if(_1b.height<0){
_1b.height=0;
}
this.dWidth=(_1b.width-this.width);
this.dHeight=(_1b.height-this.height);
};
RadEffect.Resize.prototype.Update=function(pos){
var _1e=this.Params;
var _1f=_1e.object;
var w=this.width+pos*this.dWidth;
var h=this.height+pos*this.dHeight;
_1f.style.width=Math.floor(w)+"px";
_1f.style.height=Math.floor(h)+"px";
if(_1f.Overlay){
_1f.Overlay.style.width=_1f.style.width;
_1f.Overlay.style.height=_1f.style.height;
}
};;if("undefined"==typeof (RadDockNamespace)){
RadDockNamespace=new Object();
}
RadDockNamespace.RadEventHandlerList={_eventHandlerList:null,eventHandlerList:function(){
var _1=this._eventHandlerList;
if(!_1){
_1=new Array();
this._eventHandlerList=_1;
}
return _1;
},AddEventHandler:function(_2,_3){
var _4=this.eventHandlerList();
var _5=_4[_2];
if(!_5){
_5=new Array();
_4[_2]=_5;
}
_5[_5.length]=_3;
},RemoveEventHandler:function(_6,_7){
var _8=this.eventHandlerList();
var _9=_8[_6];
if(_9){
for(var i=_9.length-1;i>=0;i--){
if(_9[i]==_7){
_9.splice(i,1);
}
}
}
},FireEvent:function(_b,_c,_d){
var _e=this.eventHandlerList();
var _f=_e[_b];
if(!_c){
_c={};
}
_c.eventName=_b;
if(_f){
if(!_d){
_d=this;
}
var _10;
for(var i=0;i<_f.length;i++){
if(false==this.ExecEventHandler(_f[i],_d,_c)){
return false;
}
}
}
},ExecEventHandler:function(_12,_13,_14){
if(_12){
var _15=typeof (_12);
switch(_15.toLowerCase()){
case "function":
return _12(_13,_14);
case "string":
eval(_12);
break;
}
}
}};;if("undefined"==typeof (RadDockNamespace)){
RadDockNamespace=new Object();
}
RadDockNamespace.ResizeDirFlags={NoResize:0,North:1,South:2,East:4,West:8,SE:6,NE:5,SW:10,NW:9,NSEW:15};
RadDockNamespace.InitMoveableObject=function(_1){
if(!_1){
return;
}
Object.Extend(_1,RadDockNamespace.RadMoveableObject);
_1.__offsetWidth=(_1.offsetWidth?_1.offsetWidth:parseInt(_1.style.width));
_1.__offsetHeight=(_1.offsetHeight?_1.offsetHeight:parseInt(_1.style.height));
};
RadDockNamespace.RadMoveableObject={MinWidth:20,MaxWidth:100000,MinHeight:20,MaxHeight:100000,AllowedResizeDir:RadDockNamespace.ResizeDirFlags.NSEW,__offsetWidth:0,__offsetHeight:0,MoveBy:function(dx,dy){
var x=RadDockNamespace.ParseInt(this.style.left,0);
var y=RadDockNamespace.ParseInt(this.style.top,0);
this.MoveTo(x+dx,y+dy);
},MoveTo:function(x,y,_8){
x=RadDockNamespace.ParseInt(x,0);
y=RadDockNamespace.ParseInt(y,0);
if(!_8){
this.style.position="absolute";
this.style.left=(x+"px");
this.style.top=(y+"px");
if(this.Overlay){
this.Overlay.style.left=this.style.left;
this.Overlay.style.top=this.style.top;
}
}else{
var _9=this.GetRect();
this.Undock();
this.MoveTo(_9.left,_9.top);
var t=new RadEffect.MoveTo({object:this,destX:x,destY:y});
}
},FixIeHeight:function(_b){
if(!document.all){
return;
}
if("CSS1Compat"==document.compatMode){
var _c=this.GetRect();
var _d=_c.height;
var _e=(_d-parseInt(_b.style.height));
if(_e>0){
var _f=(parseInt(_b.style.height)-_e);
if(_f>0){
_b.style.height=_f+"px";
}
}
}
},SetSize:function(_10,_11){
_10=RadDockNamespace.ParseInt(_10,-1);
_11=RadDockNamespace.ParseInt(_11,-1);
if(_10>-1){
this.style.width=_10+"px";
if(this.Overlay){
this.Overlay.style.width=this.style.width;
}
}
if(_11>-1){
this.style.height=_11+"px";
if(this.Overlay){
this.Overlay.style.height=this.style.height;
}
}
RadDockNamespace.FixIeHeight(this);
if(this.Overlay){
RadDockNamespace.FixIeHeight(this.Overlay);
}
},Inflate:function(_12,_13,_14,_15){
var rc=this.GetRect();
var top=rc.top+_13;
var _18=rc.left+_12;
if(top<0){
_13=-rc.top;
}
if(_18<0){
_12=-rc.left;
}
top=rc.top+_13;
_18=rc.left+_12;
if(null==_14){
_14=-_12;
}
if(null==_15){
_15=-_13;
}
var _19=rc.width+_14;
var _1a=rc.height+_15;
_19=Math.max(this.MinWidth,_19);
_19=Math.min(this.MaxWidth,_19);
_1a=Math.max(this.MinHeight,_1a);
_1a=Math.min(this.MaxHeight,_1a);
if(rc.width!=_19){
this.MoveBy(_12,0);
this.SetSize(_19,null);
}
if(rc.height!=_1a){
this.MoveBy(0,_13);
this.SetSize(null,_1a);
}
},BringToFront:function(){
var _1b=0;
var _1c=0;
var _1d=this.parentNode.childNodes;
var _1e;
for(var i=0;i<_1d.length;i++){
_1e=_1d[i];
if(1!=_1e.nodeType){
continue;
}
_1c=parseInt(_1e.style.zIndex);
if(_1c>_1b){
_1b=_1c;
}
}
this.style.zIndex=(_1b+2);
if(this.Overlay){
this.Overlay.style.zIndex=this.style.zIndex-1;
}
if(!document.all){
var _20=this.parentNode;
_20.removeChild(this);
_20.appendChild(this);
}
},Resize:function(dir,dX,dY){
switch(dir.toLowerCase()){
case "n-resize":
this.Inflate(0,dY,null,null);
break;
case "s-resize":
this.Inflate(0,0,0,dY);
break;
case "w-resize":
this.Inflate(dX,0,null,null);
break;
case "e-resize":
this.Inflate(0,0,dX,0);
break;
case "ne-resize":
this.Inflate(0,dY,dX,null);
break;
case "nw-resize":
this.Inflate(dX,dY,null,null);
break;
case "se-resize":
this.Inflate(0,0,dX,dY);
break;
case "sw-resize":
this.Inflate(dX,0,null,dY);
break;
}
},GetRect:function(_24){
return RadGetElementRect(_24||this);
},SetRect:function(_25){
this.MoveTo(_25.left,_25.top);
this.SetSize(_25.width,_25.height);
},CalcResizeDir:function(_26){
var x=_26.clientX+RadDockNamespace.GetScrollLeft();
var y=_26.clientY+RadDockNamespace.GetScrollTop();
var rc=this.GetRect();
var _2a=RadDockNamespace.ParseInt(this.style.borderWidth,0)+10;
var _2b="";
if(((this.AllowedResizeDir&RadDockNamespace.ResizeDirFlags.North)>0)&&(rc.top<=y)&&(y<=(rc.top+_2a))){
_2b="n";
}else{
if(((this.AllowedResizeDir&RadDockNamespace.ResizeDirFlags.South)>0)&&((rc.bottom-_2a)<=y)&&(y<=rc.bottom)){
_2b="s";
}
}
if(((this.AllowedResizeDir&RadDockNamespace.ResizeDirFlags.West)>0)&&(rc.left<=x)&&(x<=(rc.left+_2a))){
_2b+="w";
}else{
if(((this.AllowedResizeDir&RadDockNamespace.ResizeDirFlags.East)>0)&&((rc.right-_2a)<=x)&&(x<=rc.right)){
_2b+="e";
}
}
if(_2b){
_2b+="-resize";
}
return _2b;
},IsVisible:function(){
return (this.style.display!="none");
},Show:function(_2c){
if(_2c==false){
this.__offsetWidth=this.offsetWidth;
this.__offsetHeight=this.offsetHeight;
this.OldDisplayStyle=this.style.display;
this.style.display="none";
if(this.Overlay){
this.Overlay.style.display=this.style.display;
}
}else{
this.style.display=this.OldDisplayStyle?this.OldDisplayStyle:"";
if(this.Overlay){
this.Overlay.style.display=this.style.display;
}
}
if(this.OnShowHide){
this.OnShowHide();
}
},Hide:function(){
this.Show(false);
}};;if("undefined"==typeof (RadDockNamespace)){
RadDockNamespace=new Object();
}
RadDockNamespace.RadRectangle=function(_1,_2,_3,_4){
this.left=(null!=_1?_1:0);
this.top=(null!=_2?_2:0);
this.width=(null!=_3?_3:0);
this.height=(null!=_4?_4:0);
this.right=_1+_3;
this.bottom=_2+_4;
};
RadDockNamespace.RadRectangle.prototype.Clone=function(){
return new RadRectangle(this.left,this.top,this.width,this.height);
};
RadDockNamespace.RadRectangle.prototype.PointInRect=function(x,y){
return (this.left<=x&&x<=(this.left+this.width)&&this.top<=y&&y<=(this.top+this.height));
};
RadDockNamespace.RadRectangle.prototype.Intersects=function(_7){
if(null==_7){
return false;
}
if(this==_7){
return true;
}
return (_7.left<this.right&&_7.top<this.bottom&&_7.right>this.left&&_7.bottom>this.top);
};
RadDockNamespace.RadRectangle.prototype.ToString=function(){
return "left:"+this.left+" "+"right:"+this.right+" "+"top:"+this.top+" "+"bottom:"+this.bottom+" "+"("+this.width+" x "+this.height+")";
};
RadDockNamespace.RadRectangle.prototype.Intersection=function(_8){
if(null==_8){
return false;
}
if(this==_8){
return this.Clone();
}
if(!this.Intersects(_8)){
return new RadRectangle();
}
var _9=Math.max(this.left,_8.left);
var _a=Math.max(this.top,_8.top);
var _b=Math.min(this.right,_8.right);
var _c=Math.min(this.bottom,_8.bottom);
return new RadRectangle(_9,_b,_b-_9,_c-_a);
};
function RadGetElementRect(_d){
if(!_d){
_d=this;
}
var _e=0;
var _f=0;
var _10=_d.offsetWidth;
var _11=_d.offsetHeight;
if(_d.x){
_e=_d.x;
_f=_d.y;
}else{
var _12=_d;
while(_12!=null){
_e+=_12.offsetLeft-_12.scrollLeft;
_f+=_12.offsetTop-_12.scrollTop;
_12=_12.offsetParent;
}
}
_e=RadDockNamespace.ParseInt(_e,0);
_f=RadDockNamespace.ParseInt(_f,0);
_10=RadDockNamespace.ParseInt(_10,0);
_11=RadDockNamespace.ParseInt(_11,0);
return new RadDockNamespace.RadRectangle(_e,_f,_10,_11);
}
RadDockNamespace.RadGetCurrentStyle=function(_13){
if(_13.currentStyle){
return _13.currentStyle;
}else{
return document.defaultView.getComputedStyle(_13,null);
}
};;if("undefined"==typeof (RadDockNamespace)){
RadDockNamespace=new Object();
}
RadDockNamespace.RadTooltip_Init=function(_1){
if(!_1){
return;
}
Object.Extend(_1,RadDockNamespace.RadTooltip);
};
RadDockNamespace.RadTooltip={SetText:function(_2){
this.innerHTML="";
this.appendChild(document.createTextNode(_2));
},ShowAt:function(x,y){
this.style.display="";
this.style.left=x+"px";
this.style.top=y+"px";
},Hide:function(){
this.style.display="none";
},BringToFront:function(){
RadDockNamespace.RadMoveableObject.BringToFront.call(this);
}};;//BEGIN_ATLAS_NOTIFY
if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
