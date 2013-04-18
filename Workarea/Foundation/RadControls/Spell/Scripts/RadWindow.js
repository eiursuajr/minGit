function RadWindow(id){
this.IsIE=(null!=document.all)&&(window.opera==null);
this.IsQuirksMode=(document.all&&!window.opera&&"CSS1Compat"!=document.compatMode);
this.Id=id;
this.Width=0;
this.Height=0;
this.OnClientClosing=null;
this.ContentWindow=null;
this.ContentWrapperTable=null;
this.Caption=null;
this.X=0;
this.Y=0;
this.ShowContentWhenMoving=true;
this.CanMove=true;
this.CanResize=true;
this.DragMode="";
this.IsModal=false;
this.Container=null;
this.Parent=null;
this.Argument=null;
this.ReturnValue=null;
this.ExitCode=null;
this.ZIndex=0;
this.AdjustPosInterval=-1;
this.CallbackFunc=null;
this.OnLoadFunc=null;
this.Param=null;
this.ModalSetCapture=false;
this.UseRadWindow=true;
this.Window=null;
this.InnerHTML=null;
this._overImage=null;
}
RadWindow.prototype.OnLoad=function(){
if(this.Window&&""!=this.Window.document.title){
this.SetCaption(this.Window.document.title);
}
if(this.OnLoadFunc){
this.OnLoadFunc();
}
};
RadWindow.prototype.SetCapture=function(_2){
if(this.UseRadWindow){
if(null!=_2){
this.bContainerCapture=_2;
}else{
if(null!=this.bContainerCapture){
_2=this.bContainerCapture;
}else{
_2=false;
}
}
if(this.ModalSetCapture&&this.IsIE){
this.ContentWrapperTable.setCapture(_2);
}
}
};
RadWindow.prototype.ReleaseCapture=function(){
if(this.UseRadWindow){
if(this.ModalSetCapture&&this.IsIE){
if(this.ContentWrapperTable){
this.ContentWrapperTable.releaseCapture();
}
}
}
};
RadWindow.prototype.SetZIndex=function(_3){
this.ZIndex=_3;
if(this.ContentWrapperTable){
this.ContentWrapperTable.style.zIndex=this.ZIndex;
}
};
RadWindow.prototype.ToggleContent=function(){
if(this.UseRadWindow&&this.IsIE){
var _4="";
if(parseInt(this.Height)==parseInt(this.ContentWrapperTable.style.height)){
this.SetHeight(0);
_4="none";
}else{
this.SetHeight(this.Height);
_4="inline";
}
}
};
RadWindow.prototype.IsVisible=function(){
if(this.ContentWrapperTable){
return this.ContentWrapperTable.style.display=="";
}
return false;
};
RadWindow.prototype.ShowWindow=function(_5,x,y){
if(null==_5){
_5=true;
}
var _8=_5?"":"none";
if(this.ContentWrapperTable){
this.ContentWrapperTable.style.display=_8;
}
if(_5){
if(null!=x&&null!=y){
x+=10;
if(this.ContentWrapperTable){
this.ContentWrapperTable.style.left=x+"px";
this.ContentWrapperTable.style.top=y+"px";
}
}
}
if(this.Parent){
this.Parent.OnShowWindow(this,_5);
}
};
RadWindow.prototype.Initialize2=function(_9,_a,_b,_c,_d){
this.Initialize(_9,_a);
this.IsModal=_c;
this.Container=_b;
this.SetZIndex(_d);
};
RadWindow.prototype.Initialize=function(_e,_f){
if(this.Id){
this.ContentWrapperTable=document.getElementById("RadWindowContentWrapper"+this.Id);
this.ContentWindow=document.getElementById("RadWindowContentWindow"+this.Id);
this.Caption=document.getElementById("RadWindowCaption"+this.Id);
if(null==_f){
var _f=true;
}
this.ShowWindow(_f);
}else{
alert("No window Id provided");
}
};
RadWindow.prototype.SetContentWindowSize=function(_10,_11){
this.Width=_10;
this.Height=_11;
};
RadWindow.prototype.SetContentVisible=function(_12){
if(this.ContentWindow){
this.ContentWindow.style.visibility=_12?"visible":"hidden";
}
};
RadWindow.prototype.Close=function(_13,_14,_15){
if(null!=this.OnClientClosing&&(this.OnClientClosing(_13)==false)){
return;
}
this.ShowWindow(false);
this.ExitCode=_13;
if(this.AdjustPosInterval>-1){
window.clearInterval(this.AdjustPosInterval);
this.AdjustPosInterval=-1;
}
if(this.IsModal){
this.ReleaseCapture();
}
try{
if(this.CallbackFunc&&false!=_15){
this.CallbackFunc(this.ReturnValue,this.Param);
}
if(_14){
_14(this.ReturnValue,this.Param);
}
}
catch(ex){
}
if(this.Parent){
this.Parent.DestroyWindow(this);
}
if(!this.UseRadWindow&&this.Window){
this.Window.close();
}
};
RadWindow.prototype.ToggleCanMove=function(_16){
if(!this.UseRadWindow){
return;
}
this.CanMove=!this.CanMove;
_16.className=this.CanMove?"RadERadWindowButtonPinOff":"RadERadWindowButtonPinOn";
if(!this.CanMove){
if(this.IsIE){
this.TopOffset=parseInt(this.ContentWrapperTable.style.top)-RadGetScrollTop(document);
this.StartUpdatePosTimer(100);
}else{
this.ContentWrapperTable.style.position="fixed";
}
}else{
if(this.IsIE){
window.clearInterval(this.AdjustPosInterval);
this.TopOffset=null;
}else{
this.ContentWrapperTable.style.position="absolute";
}
}
};
RadWindow.prototype.StartUpdatePosTimer=function(_17){
if(!this.UseRadWindow){
return;
}
this.AdjustPosInterval=window.setInterval("UpdateWindowPos('"+this.Id+"')",_17);
};
function UpdateWindowPos(_18){
var wnd=GetEditorRadWindowManager().LookupWindow(_18);
if(wnd){
wnd.SetPosition();
}
}
RadWindow.prototype.CanDrag=function(){
if(!this.UseRadWindow){
return true;
}
return ("move"==this.DragMode&&this.CanMove)||("size"==this.DragMode&&this.CanResize);
};
RadWindow.prototype.OnDragStart=function(e){
this.SetActive(true);
if(!this.CanDrag()){
return;
}
this.X=(e.offsetX==null)?e.layerX:e.offsetX;
this.Y=(e.offsetY==null)?e.layerY:e.offsetY;
MousePosX=e.clientX;
MousePosY=e.clientY;
this.SetContentVisible(this.ShowContentWhenMoving);
RadWindowDragStart();
};
RadWindow.prototype.SetActive=function(_1b){
if(!this.UseRadWindow){
return;
}
if(_1b){
if(null!=RadWindowActiveWindow&&RadWindowActiveWindow!=this){
RadWindowActiveWindow.SetActive(false);
}
RadWindowActiveWindow=this;
if(this.Parent){
this.Parent.ActivateWindow(this);
}
}else{
if(this==RadWindowActiveWindow){
RadWindowActiveWindow=null;
}
}
};
RadWindow.prototype.HitTest=function(x,y){
var _1e=parseInt(this.ContentWrapperTable.style.left);
if(isNaN(_1e)){
return false;
}
var top=parseInt(this.ContentWrapperTable.style.top);
if(isNaN(top)){
return false;
}
return _1e<=x&&x<=(_1e+this.Width)&&top<=y&&y<=(top+this.Height);
};
RadWindow.prototype.SetPosition=function(_20,top){
if(!this.UseRadWindow){
if(this.Window){
this.Window.dialogLeft=_20;
this.Window.dialogTop=top;
}
}else{
if(!_20){
_20=this.ContentWrapperTable.style.left;
}
if(!top){
if(this.TopOffset!=null){
top=parseInt(this.TopOffset)+RadGetScrollTop(document);
}else{
top=this.ContentWrapperTable.style.top;
}
}
_20=parseInt(_20);
top=parseInt(top);
if(!isNaN(_20)&&!isNaN(top)){
if(_20<=0){
_20=0;
}
if(top<=0){
top=0;
}
this.ContentWrapperTable.style.left=_20+"px";
this.ContentWrapperTable.style.top=top+"px";
}
}
};
RadWindow.prototype.GetWidth=function(){
var _22=0;
if(!this.UseRadWindow){
if(this.Window){
_22=this.Window.dialogWidth;
}
}else{
if(this.IsIE){
_22=parseInt(this.ContentWrapperTable.clientWidth);
}else{
_22=parseInt(this.ContentWrapperTable.scrollWidth);
}
if(isNaN(_22)){
_22=0;
}
}
return _22;
};
RadWindow.prototype.SetWidth=function(_23){
_23=parseInt(_23);
if(isNaN(_23)){
return;
}
if(!this.UseRadWindow){
if(this.Window){
if(this.Window.dialogWidth){
this.Window.dialogTop=this.Window.screenTop-30;
this.Window.dialogLeft=this.Window.screenLeft-4;
this.Window.dialogWidth=_23+"px";
}else{
this.Window.outerWidth=_23;
}
}
}else{
if(_23){
this.ContentWrapperTable.style.width=_23+"px";
}
}
};
RadWindow.prototype.GetHeight=function(){
var _24=0;
if(!this.UseRadWindow){
if(this.Window){
_24=this.Window.dialogHeight;
}
}else{
if(this.IsIE){
_24=parseInt(this.ContentWrapperTable.clientHeight);
if(isNaN(_24)){
_24=0;
}
}else{
_24=this.ContentWrapperTable.scrollHeight;
}
}
return _24;
};
RadWindow.prototype.SetHeight=function(_25){
_25=parseInt(_25);
if(isNaN(_25)){
return;
}
if(!this.UseRadWindow){
if(this.Window){
if(this.Window.dialogWidth){
this.Window.dialogTop=this.Window.screenTop-30;
this.Window.dialogLeft=this.Window.screenLeft-4;
this.Window.dialogHeight=_25+"px";
}else{
this.Window.outerHeight=_25;
}
}
}else{
var _26=this.ContentWrapperTable;
var _27=_26.getElementsByTagName("TABLE")[0];
if(_27){
_27.setAttribute("border","1");
}
this.ContentWrapperTable.style.height=_25+"px";
this.FixIeHeight(this.ContentWrapperTable,_25);
_27.setAttribute("border","0");
}
};
RadWindow.prototype.FixIeHeight=function(_28,_29){
if(this.IsIE&&"CSS1Compat"==document.compatMode){
var _2a=_28.offsetHeight;
if(_2a!=0&&_2a!=_29){
var _2b=(_2a-_29);
var _2c=(_29-_2b);
if(_2c>0){
_28.style.height=(_2c+4)+"px";
}
}
}
};
function RadWindowUnInitializeDrag(_2d){
var _2e=RadWindowActiveWindowSpan;
_2d.SetPosition(_2e.style.left,_2e.style.top);
var _2f=_2d.IsQuirksMode?6:0;
_2f+=_2d.IsIE?2:0;
extraHeight=_2f;
if(_2d.IsIE&&!_2d.IsQuirksMode&&extraHeight>0){
extraHeight-=2;
}
_2d.SetSize(_2f+(_2e.clientWidth?_2e.clientWidth:_2e.offsetWidth),extraHeight+(_2e.clientHeight?_2e.clientHeight:_2e.offsetHeight));
if(RadWindowActiveWindowSpan){
RadWindowActiveWindowSpan.style.visibility="hidden";
}
}
RadWindow.prototype.SetSize=function(_30,_31){
if(!this.UseRadWindow&&!document.all&&this.Window&&this.Window.resizeTo){
this.Window.resizeTo(_30,_31);
}else{
this.SetWidth(_30);
this.SetHeight(_31);
}
if(_30>0){
this.Width=_30;
}
if(_31>0){
this.Height=_31;
}
};
RadWindow.prototype.SetCaption=function(_32){
if(this.Caption){
this.Caption.innerHTML=_32;
}
};
var RadWindowActiveWindow=null;
var RadWindowActiveWindowSpan=null;
var MousePosX=0;
var MousePosY=0;
function RadWindowDragStart(){
if(!RadWindowActiveWindow.CanDrag()){
return;
}
if(document.all&&document.body.attachEvent){
document.body.setCapture();
document.body.attachEvent("onmousemove",RadWindowDrag);
document.body.attachEvent("onmouseup",RadWindowDragEnd);
}else{
if(document.addEventListener){
document.addEventListener("mousemove",RadWindowDrag,false);
document.addEventListener("mouseup",RadWindowDragEnd,false);
}
}
RadWindowInitializeDrag(RadWindowActiveWindow);
}
function RadWindowDragEnd(){
if(document.all&&document.body.detachEvent){
document.body.detachEvent("onmousemove",RadWindowDrag);
document.body.detachEvent("onmouseup",RadWindowDragEnd);
document.body.releaseCapture();
}else{
if(document.removeEventListener){
document.removeEventListener("mousemove",RadWindowDrag,false);
document.removeEventListener("mouseup",RadWindowDragEnd,false);
}
}
if(RadWindowActiveWindow.IsModal){
RadWindowActiveWindow.SetCapture();
}
RadWindowUnInitializeDrag(RadWindowActiveWindow);
RadWindowActiveWindow.SetContentVisible(true);
}
function RadWindowDrag(e){
if(RadWindowActiveWindow.CanDrag()){
switch(RadWindowActiveWindow.DragMode){
case "move":
RadWindowMove(e);
break;
case "size":
RadWindowSize(e);
break;
}
}
e.cancelBubble=true;
e.returnValue=false;
if(e.preventDefault){
e.preventDefault();
}
MousePosX=e.clientX;
MousePosY=e.clientY;
return false;
}
function RadWindowInitializeDrag(_34){
if(!_34){
return;
}
if(!RadWindowActiveWindowSpan){
RadWindowActiveWindowSpan=document.createElement("DIV");
RadWindowActiveWindowSpan.className="RadETableWrapperResizeSpan";
RadWindowActiveWindowSpan.style.position="absolute";
RadWindowActiveWindowSpan.style.zIndex=50000;
document.body.appendChild(RadWindowActiveWindowSpan);
}
RadWindowActiveWindowSpan.style.visibility="visible";
RadWindowActiveWindowSpan.style.top=_34.ContentWrapperTable.style.top;
RadWindowActiveWindowSpan.style.left=_34.ContentWrapperTable.style.left;
RadWindowActiveWindowSpan.style.width=parseInt(_34.GetWidth())+"px";
RadWindowActiveWindowSpan.style.height=parseInt(_34.GetHeight())+"px";
switch(_34.DragMode){
case "move":
RadWindowActiveWindowSpan.style.cursor="default";
break;
case "size":
RadWindowActiveWindowSpan.style.cursor="se-resize";
break;
}
}
function RadWindowMove(e){
var _36=RadWindowActiveWindow.X;
var _37=RadWindowActiveWindow.Y;
var el=RadWindowActiveWindowSpan;
var _39=0;
var top=0;
if(document.all){
_39=e.clientX*1+RadGetScrollLeft(document)-_36;
top=e.clientY*1+RadGetScrollTop(document)-_37;
}else{
_39=e.pageX*1-_36;
top=e.pageY*1-_37;
}
if(_39<0){
_39=0;
}
if(top<0){
top=0;
}
el.style.left=_39+"px";
el.style.top=top+"px";
}
var minWidth=155;
var minHeight=43;
function RadWindowSize(e){
var _3c=e.clientX-MousePosX;
var _3d=e.clientY-MousePosY;
var _3e=RadWindowActiveWindowSpan;
var _3f=_3e.clientWidth?_3e.clientWidth:_3e.offsetWidth;
var _40=_3e.clientHeight?_3e.clientHeight:_3e.offsetHeight;
if(document.all&&!window.opera&&"CSS1Compat"!=document.compatMode){
_3f=_3e.offsetWidth;
_40=_3e.offsetHeight;
}
_3f+=_3c;
_40+=_3d;
if(_3f<minWidth){
_3f=minWidth;
}
if(_40<minHeight){
_40=minHeight;
}
RadWindowActiveWindowSpan.style.width=_3f+"px";
RadWindowActiveWindowSpan.style.height=_40+"px";
}
function GetTopWindow(){
var _41=null;
var _42=false;
try{
if(window.dialogArguments.parentWindow&&_42){
_42=true;
}
}
catch(ex){
_42=false;
}
if(window.dialogArguments!=null&&_42){
_41=window.dialogArguments.parentWindow;
}else{
if(window.opener&&!document.all&&window.isRadWindow){
_41=opener;
}else{
_41=window;
}
}
var _43=false;
while(_41.parent&&!_43){
try{
topWindowTagName=_41.parent.tagName.toUpperCase();
}
catch(exception){
topWindowTagName="";
}
if(_41.parent==_41){
break;
}
try{
if(_41.parent.document.domain!=window.document.domain){
break;
}
}
catch(exc){
_43=true;
continue;
}
try{
if(_41.frameElement!=null&&(_41.frameElement.tagName!="IFRAME"||_41.frameElement.name!="RadWindowContent")){
break;
}
}
catch(exc){
alert("in the Exception!");
_43=true;
}
_41=_41.parent;
}
return _41;
}
function Document_OnFocus(e){
if(!e){
e=window.event;
}
GetEditorRadWindowManager().ActivateWindow();
}
function Document_OnKeyDown(e){
if(!e){
e=window.event;
}
return GetEditorRadWindowManager().OnKeyDown(e);
}
function RadWindowInfo(){
this.IsIE=(null!=document.all);
this.ID=null;
this.Url="";
this.InnerHtml="";
this.InnerObject=null;
this.Width=300;
this.Height=200;
this.Caption="";
this.IsVisible=true;
this.Argument=null;
this.CallbackFunc=null;
this.OnLoadFunc=null;
this.Param=null;
this.Resizable=true;
this.Movable=true;
this.CloseHide=false;
this.UseClassicDialogs=true;
}
function GetEditorRadWindowManager(){
var _46=GetTopWindow();
if(!_46.radWindowManager){
_46.radWindowManager=new RadEditorWindowManager();
}
return _46.radWindowManager;
}
function RadEditorWindowManager(){
this.ChildWindows=new Array();
this.ActiveWindow=null;
this.TopWindowZIndex=10001;
this.ContainerPool=new Array();
this.IsIE=(null!=document.all)&&(window.opera==null);
this._overImage=null;
document.body.onfocus=Document_OnFocus;
if(this.IsIE&&document.body.attachEvent){
document.body.attachEvent("onkeydown",Document_OnKeyDown);
}else{
if(document.body.addEventListener){
document.body.addEventListener("keydown",Document_OnKeyDown,false);
}
}
}
RadEditorWindowManager.prototype.ShowModalWindow=function(_47){
var wnd=this.CreateWindow(true,_47);
return wnd;
};
RadEditorWindowManager.prototype.ShowModallessWindow=function(_49){
var wnd=this.CreateWindow(false,_49);
return wnd;
};
RadEditorWindowManager.prototype.CreateWindow=function(_4b,_4c){
if(!_4c){
return null;
}
if(window.opera){
_4c.UseClassicDialogs=true;
}
var id="";
if(!_4c.ID||_4c.ID==""){
id=this.ChildWindows.length;
}else{
id=_4c.ID;
}
var _4e="";
if(null==_4c.Caption){
_4e="["+id+"]";
}else{
_4e=_4c.Caption;
}
var _4f=this.GetWindow(id);
_4f.Argument=_4c.Argument;
_4f.Width=_4c.Width;
_4f.Height=_4c.Height;
_4f.CallbackFunc=_4c.CallbackFunc;
_4f.Param=_4c.Param;
_4f.CanResize=_4c.Resizable;
_4f.CanMove=_4c.Movable;
_4f.OnLoadFunc=_4c.OnLoadFunc;
_4f.UseRadWindow=!_4c.UseClassicDialogs;
if(_4c.UseClassicDialogs&&this.IsIE){
var _50="status:no;"+"resizable:yes;"+"center:yes;"+"help:no;"+"minimize:no;"+"maximize:no;"+"scroll:no;"+"border:thin;"+"statusbar:no;"+"dialogWidth:"+_4c.Width+"px;"+"dialogHeight:"+_4c.Height+"px";
if(_4c.InnerHtml&&_4c.InnerHtml!=""){
_4f.InnerHTML=_4c.InnerHtml;
}
if(!_4c.Url||""==_4c.Url){
_4c.Url="javascript:''";
}
var _51={parentWindow:window,radWindow:_4f,IsRadWindowArgs:true};
window.showModalDialog(_4c.Url,_51,_50);
}else{
if(_4c.UseClassicDialogs&&!this.IsIE){
if(!_4c.Url||""==_4c.Url){
_4c.Url="javascript:''";
}
window.childRadWindow=_4f;
_4f.Window=window.open(_4c.Url,"_blank","status=no,toolbar=no,location=no,resizable=yes,menubar=no,width="+_4c.Width+",height="+_4c.Height+",modal=yes");
}else{
if(!_4c.UseClassicDialogs){
var _52=null;
if(this.ContainerPool.length>0){
_52=this.ContainerPool.pop();
}else{
_52=document.createElement("SPAN");
document.body.appendChild(_52);
}
var _53=this.BuildWrapperTableHtml(id,_4c.Width,_4c.Height,_4e,_4b,_4c.CloseHide);
_52.innerHTML=_53;
var _54=(null!=_4c.InnerObject)?_4c.InnerObject:document.getElementById("RadWindowContentFrame"+id);
_4f.Initialize2(_54,true,_52,_4b,++this.TopWindowZIndex);
var frm=document.getElementById("RadWindowContentFrame"+id);
_4f.Window=null!=frm?frm.contentWindow:null;
_4f.Iframe=frm;
if(_4c.InnerHtml&&_4c.InnerHtml!=""){
var doc=frm.contentWindow.document;
doc.open();
doc.write(_4c.InnerHtml);
doc.close();
}else{
if(_4c.Url&&_4c.Url!=""){
frm.src=_4c.Url;
}
}
if(_4b){
this.SetCapture(_4f,false);
}
if(null==_4c.IsVisible){
_4c.IsVisible=false;
}
var _57=this.GetWindowStartPosition(_4c);
_4f.SetSize(_4c.Width,_4c.Height);
_4f.ShowWindow(_4c.IsVisible,_57.horizontal,_57.vertical);
}
}
}
return _4f;
};
RadEditorWindowManager.prototype.GetWindowStartPosition=function(_58){
var _59=parseInt(_58.Height)/2;
var _5a=parseInt(_58.Width)/2;
var _5b=this.GetDocumentVisibleCenter();
return {horizontal:_5b.horizontal-_5a,vertical:_5b.vertical-_59};
};
RadEditorWindowManager.prototype.GetDocumentVisibleCenter=function(){
var _5c=0;
var _5d=0;
var _5e=document.body;
var _5f=!((RadControlsNamespace.Browser.IsMozilla||RadControlsNamespace.Browser.IsIE)&&document.compatMode!="CSS1Compat");
if(_5f&&!RadControlsNamespace.Browser.IsSafari){
_5e=document.documentElement;
}
if(window.innerWidth){
_5c=window.innerWidth;
_5d=window.innerHeight;
}else{
_5c=_5e.clientWidth;
_5d=_5e.clientHeight;
}
var _60=this.GetVisibleCenterCoordinate(_5e.scrollLeft,_5c);
var _61=this.GetVisibleCenterCoordinate(_5e.scrollTop,_5d);
return {horizontal:_60,vertical:_61};
};
RadEditorWindowManager.prototype.GetVisibleCenterCoordinate=function(_62,_63){
var _64=parseInt(_63)/2;
return parseInt(_62)+_64;
};
RadEditorWindowManager.prototype.DestroyWindow=function(_65){
var _66=this.GetPrevWindow(_65.Id);
this.UnregisterChild(_65);
if(_66!=_65){
this.ActivateWindow(_66);
}
if(_65.Iframe){
_65.Iframe.src="javascript:'';";
}
eval(this.GetWindowVarName(_65.Id)+" = null;");
if(_65.Container){
this.ContainerPool.push(_65.Container);
}
};
RadEditorWindowManager.prototype.GetPrevWindow=function(id){
var _68=false;
var _69=null;
for(var i=this.ChildWindows.length-1;i>=0;i--){
var wnd=this.ChildWindows[i];
if(wnd&&wnd.Id==id){
_68=true;
}else{
if(wnd&&_68){
return wnd;
}else{
if(null==_69){
_69=wnd;
}
}
}
}
return _69;
};
RadEditorWindowManager.prototype.CloseWindow=function(id){
var wnd=this.LookupWindow(id);
if(wnd){
wnd.Close();
}
};
RadEditorWindowManager.prototype.ActivateWindow=function(_6e){
if(!_6e){
_6e=this.ActiveWindow;
}
if(_6e){
if(_6e.IsModal){
this.SetCapture(_6e,false);
}
if(_6e!=this.ActiveWindow){
if(this.ActiveWindow){
this.ActiveWindow.SetZIndex(this.TopWindowZIndex-1);
}
_6e.SetZIndex(this.TopWindowZIndex);
this.ActiveWindow=_6e;
}
if(_6e.IsModal){
this.ShowOverImage(_6e,true);
}
}
};
RadEditorWindowManager.prototype.RegisterChild=function(_6f){
this.ChildWindows[this.ChildWindows.length]=_6f;
};
RadEditorWindowManager.prototype.UnregisterChild=function(_70){
for(var i=0;i<this.ChildWindows.length;i++){
var wnd=this.ChildWindows[i];
if(wnd==_70){
this.ChildWindows[i]=null;
return;
}
}
};
RadEditorWindowManager.prototype.SetCapture=function(_73,_74){
try{
if(_73){
_73.SetCapture(_74);
}
}
catch(ex){
}
};
RadEditorWindowManager.prototype.LookupWindow=function(id){
for(var i=0;i<this.ChildWindows.length;i++){
var wnd=this.ChildWindows[i];
if(wnd&&id==wnd.Id){
return wnd;
}
}
return null;
};
RadEditorWindowManager.prototype.LookupRadWindowByBrowserWindowRef=function(_78){
for(var i=0;i<this.ChildWindows.length;i++){
var _7a=this.ChildWindows[i];
if(null!=_7a&&_78==_7a.Window){
return _7a;
}
}
return null;
};
RadEditorWindowManager.prototype.GetCurrentRadWindow=function(_7b){
if(_7b.dialogArguments&&(true==_7b.dialogArguments.IsRadWindowArgs)){
return _7b.dialogArguments.radWindow;
}else{
if(_7b.opener!=null&&_7b.opener.childRadWindow!=null){
return _7b.opener.childRadWindow;
}else{
var _7c=this.LookupRadWindowByBrowserWindowRef(_7b);
return _7c;
}
}
};
RadEditorWindowManager.prototype.GetWindow=function(id){
var wnd=this.LookupWindow(id);
if(!wnd){
var _7f=this.GetWindowVarName(id);
eval(_7f+" = new RadWindow('"+id+"');");
wnd=eval(_7f);
wnd.Parent=this;
this.RegisterChild(wnd);
}
return wnd;
};
RadEditorWindowManager.prototype.GetWindowVarName=function(id){
return "window.radWindow_"+id;
};
RadEditorWindowManager.prototype.GetWindowFromPos=function(x,y){
var wnd=null;
for(var i=0;i<this.ChildWindows;i++){
var _85=this.ChildWindows[i];
if(_85&&_85.HitText(x,y)){
if(!wnd||wnd.ZIndex<_85.ZIndex){
wnd=_85;
}
}
}
return wnd;
};
RadEditorWindowManager.prototype.OnShowWindow=function(_86,_87){
if(_87){
this.ActiveWindow=_86;
}else{
if(this.ActiveWindow==_86){
this.ActiveWindow=null;
}
}
if(_86.IsModal){
this.ShowOverImage(_86,_87);
}
};
RadEditorWindowManager.prototype.OnKeyDown=function(evt){
switch(evt.keyCode){
case 115:
if(evt.altKey&&this.ActiveWindow){
}else{
if(evt.ctrlKey&&this.ActiveWindow){
}
}
evt.keyCode=8;
break;
case 27:
if(this.ActiveWindow){
this.ActiveWindow.Close();
}
break;
default:
return;
}
evt.cancelBubble=true;
evt.returnValue=false;
};
RadEditorWindowManager.prototype.BuildWrapperTableHtml=function(id,_8a,_8b,_8c,_8d,_8e){
var url=document.all?"javascript:'';":"";
var _90=_8e?"ShowWindow(false)":"Close()";
var _91="";
_91+="\t\t<table border='0' id='RadWindowContentWrapper"+id+"' class='RadETableWrapper' style='width: "+_8a+";height:"+_8b+";z-index:1000; position:absolute;' cellspacing='0' cellpadding='0' >\n"+"\t\t\t<tr  style='height:1px;'>\n"+"\t\t\t\t<td width='1' class='RadETableWrapperHeaderLeft' nowrap></td>\n"+"\t\t\t\t<td width='100%' class='RadETableWrapperHeaderCenter' nowrap='true' onmousedown='radWindow_"+id+".DragMode=\"move\"; return radWindow_"+id+".OnDragStart(event);' onselectstart='return false;'>\n"+"\t\t\t\t\t<span id='RadWindowCaption"+id+"' onselectstart='return false;' class='RadERadWindowHeader'>"+_8c+"</span>\n"+"\t\t\t\t</td>\n";
if(!_8d){
_91+="\t\t<td width='1' class='RadETableWrapperHeaderCenter' nowrap>\n"+"\t\t\t\t\t<span class='RadERadWindowButtonPinOff' id='ButtonPin' onclick='radWindow_"+id+".ToggleCanMove(this)'>&nbsp;</span>"+"\t\t\t</td>\n";
}
_91+="\t\t\t\t<td width='1' class='RadETableWrapperHeaderCenter' nowrap>\n"+"\t\t\t<span class='RadERadWindowButtonClose' id='ButtonClose' onclick='radWindow_"+id+"."+_90+"'>&nbsp;</span>\n"+"\t\t\t\t</td>\n"+"\t\t\t\t<td width='1' class='RadETableWrapperHeaderRight' nowrap></td>\n"+"\t\t\t</tr>\n"+"\t\t\t<tr style='height:100%' >\n"+"\t\t\t\t<td style='height:100%;' colspan='5'>\n"+"\t\t\t\t\t<table height='100%' style='height:100%' border='0' width='100%'  cellspacing='0' cellpadding='0'>\n"+"\t\t\t\t\t\t<tr style='height:100%'>\n"+"\t\t\t\t\t\t\t<td width='1' class='RadETableWrapperBodyLeft' nowrap></td>\n"+"\t\t\t\t\t\t\t<td id='RadWindowContentWindow"+id+"'  style='height:100%;border:0px solid blue;' height='100%' width='100%' class='RadETableWrapperBodyCenter' align='left' valign='top' onselectstart='return false;'>\n"+"\t\t\t\t\t\t\t\t\t<iframe name='RadWindowContent' frameborder='0px' style='height:100%;width:100%;border:0px solid green' id='RadWindowContentFrame"+id+"' src='"+url+"' scrolling='no' border='no' ></iframe>\n"+"\t\t\t\t\t\t\t</td>\n"+"\t\t\t\t\t\t\t<td width='1' class='RadETableWrapperBodyRight' nowrap></td>\n"+"\t\t\t\t\t\t</tr>\n"+"\t\t\t\t\t</table>\n"+"\t\t\t\t</td>\n"+"\t\t\t</tr>\n"+"\t\t\t<tr style='height:1px;'>\n"+"\t\t\t\t<td colspan='5' width='100%' style='height:1px;' >\n"+"\t\t\t\t\t<table border='0' width='100%' height='1' cellspacing='0' cellpadding='0'>\n"+"\t\t\t\t\t\t<tr>\n"+"\t\t\t\t\t\t\t<td width='1' class='RadETableWrapperFooterLeft' nowrap>&nbsp;</td>\n"+"\t\t\t\t\t\t\t<td colspan='3' id='BorderBottom' width='100%' class='RadETableWrapperFooterCenter' nowrap>&nbsp;</td>\t\t\n"+"\t\t\t\t\t\t\t<td width='1' id='CornerBottomRight' class='RadETableWrapperFooterRight' onmousedown='radWindow_"+id+".DragMode=\"size\"; return radWindow_"+id+".OnDragStart(event);' onselectstart='return false;' nowrap>&nbsp;</td>\n"+"\t\t\t\t\t\t</tr>\n"+"\t\t\t\t\t</table>\n"+"\t\t\t\t</td>\n"+"\t\t\t</tr>\n"+"\t\t</table>\n";
return _91;
};
RadEditorWindowManager.prototype.SetOverImage=function(_92){
this._overImage=document.getElementById(_92);
};
RadEditorWindowManager.prototype.GetOverImage=function(){
if(!this._overImage){
this._overImage=document.getElementById("OVER_IMG");
}
return this._overImage;
};
RadEditorWindowManager.prototype.ShowOverImage=function(wnd,_94){
var _95=this.GetOverImage();
if(_95){
if(wnd&&_94){
var _96=RadControlsNamespace.Screen.GetViewPortSize();
_95.style.position="absolute";
_95.style.left=0;
_95.style.top=0;
_95.style.width=_96.width+"px";
_95.style.height=_96.height+"px";
_95.style.zIndex=wnd.ZIndex;
_95.style.display="";
}else{
_95.style.display="none";
}
}
};
function RadGetScrollTop(_97){
if(_97.documentElement&&_97.documentElement.scrollTop){
return _97.documentElement.scrollTop;
}else{
return _97.body.scrollTop;
}
}
function RadGetScrollLeft(_98){
if(_98.documentElement&&_98.documentElement.scrollLeft){
return _98.documentElement.scrollLeft;
}else{
return _98.body.scrollLeft;
}
}
function CloseDlg(_99,_9a,_9b){
if(window.radWindow){
window.radWindow.ReturnValue=_99;
window.radWindow.Close(null,_9a,_9b);
}
}
function InitializeRadWindow(_9c){
window["GetDialogArguments"]=function(_9d){
if(window["radWindow"]){
if(_9d){
return window["radWindow"].Argument.CustomArguments;
}else{
return window["radWindow"].Argument;
}
}else{
return null;
}
};
window["isRadWindow"]=true;
window["radWindow"]=GetEditorRadWindowManager().GetCurrentRadWindow(window);
if(window["radWindow"]){
if(window.dialogArguments){
window["radWindow"].Window=window;
}
window["radWindow"].OnLoad();
}
var _9e=GetDialogArguments();
if(_9e){
if(_9e.HideEditorStatusBar){
_9e.HideEditorStatusBar(_9c);
}
}
if(document.addEventListener){
document.onclick=function(e){
var _a0=e.target;
if(_a0&&(_a0.tagName=="BUTTON"||(_a0.tagName=="INPUT"&&_a0.type.toLowerCase()=="button"))){
e.cancelBuble=true;
e.returnValue=false;
if(e.preventDefault){
e.preventDefault();
}
return false;
}
};
}
}

//BEGIN_ATLAS_NOTIFY
// Ektron if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
