//-----------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------
// AtlasUIMap.js
// Atlas Mapping UI Framework.
function VE_MapControl(latitude,longitude,zoomLevel,mapStyle,position,x,y,width,height){this.index=VE_MapControl.controlList.length;this.latitude=latitude;this.longitude=longitude;this.preferredLatitude=latitude;this.preferredLongitude=longitude;this.offsetLeft=0;this.offsetTop=0;this.zoomLevel=this._ValidateZoomLevel(latitude,longitude,zoomLevel);this.prevZoomLevel=this.zoomLevel;this.mapStyle=this._ValidateMapStyle(mapStyle);this.x=x;this.y=y;this.width=width;this.height=height;this.tileWidth=0;this.tileHeight=0;this.dragging=false;this.dragPan=false;this.panLatitude=null;this.panLongitude=null;this.keyboardPan=false;this.tiles=new Array();this.oldTiles=null;this.unusedTiles=new Array();this.lastMouseX=0;this.lastMouseY=0;this.originX=0;this.originY=0;this.zoomCounter=0;this.zooming=false;this.panning=false;this.panCounter=0;this.panningX=0;this.panningY=0;this.debug=false;this.onMouseClick=null;this.onMouseDown=null;this.onMouseUp=null;this.onStartContinuousPan=null;this.onEndContinuousPan=null;this.onStartZoom=null;this.onEndZoom=null;this.pushpins=new Array();this.unusedPushpins=new Array();this.element=document.createElement("div");this.element.style.overflow="hidden";this.element.style.padding="0";this.element.style.margin="0";this.element.style.position=position;this.element.style.left=x;this.element.style.top=y;this.element.style.width=width;this.element.style.height=height;this.element.style.zIndex=VE_MapControl.containerZIndex;this.map=document.createElement("div");this.map.style.position="absolute";this.map.style.top="0px";this.map.style.left="0px";this.map.style.padding="0";this.map.style.margin="0";this.map.style.zIndex=VE_MapControl.mapZIndex;this.element.appendChild(this.map);this.keyboard=document.createElement("input");this.keyboard.style.position="absolute";this.keyboard.style.top="0px";this.keyboard.style.left="0px";this.keyboard.style.width="1px";this.keyboard.style.height="1px";this.keyboard.style.padding="0";this.keyboard.style.margin="0";this.keyboard.style.zIndex=VE_MapControl.kbInputZIndex;this.keyboard.onkeydown=VE_MapControl._KeyDown;this.keyboard.onkeyup=VE_MapControl._KeyUp;this.element.appendChild(this.keyboard);this._CreateCopyright();this._SetBackgroundColor();this.SetCenterAndZoom(latitude,longitude,zoomLevel);VE_MapControl.controlList.push(this);}
VE_MapControl.prototype.SetCenter=function(latitude,longitude){this.SetCenterAndZoom(latitude,longitude,this.zoomLevel);}
VE_MapControl.prototype.SetMapStyle=function(mapStyle){if(this.panning||this.zooming){return;}mapStyle=this._ValidateMapStyle(mapStyle);if(this.mapStyle!=mapStyle){this.mapStyle=mapStyle;this._SetBackgroundColor();this.latitude=this.GetCenterLatitude();this.longitude=this.GetCenterLongitude();this._StartMap();this._RepositionPushpins();this._UpdateCopyright();}}
VE_MapControl.prototype._ValidateMapStyle=function(s){if(s=='a'||s=='h')return s;else return'r';}
VE_MapControl.prototype._SetBackgroundColor=function(){var bg="#f2ede4";if(this.mapStyle=='a'||this.mapStyle=='h')bg="#404040";this.element.style.background=bg;}
VE_MapControl.prototype.GetCenterLatitude=function(){return this.latitude;}
VE_MapControl.prototype.GetCenterLongitude=function(){return this.longitude;}
VE_MapControl.prototype._ComputeCenterPoint=function(capture){this.latitude=this.GetLatitude(this.height/2);this.longitude=this.GetLongitude(this.width/2);if(capture){this.preferredLatitude=this.latitude;this.preferredLongitude=this.longitude;}}
VE_MapControl.prototype.GetLatitude=function(y){return this._YToLat(this.originY+this.offsetY+y);}
VE_MapControl.prototype.GetLongitude=function(x){return this._XToLon(this.originX+this.offsetX+x);}
VE_MapControl.prototype.GetY=function(latitude){return this._LatToY(latitude)-this.originY-this.offsetY;}
VE_MapControl.prototype.GetX=function(longitude){return this._LonToX(longitude)-this.originX-this.offsetX;}
VE_MapControl.prototype.GetZoomLevel=function(){return this.zoomLevel;}
VE_MapControl.prototype.GetMapStyle=function(){return this.mapStyle;}
VE_MapControl.prototype.GetMetersPerPixel=function(latitude,zoomLevel){if(!latitude){latitude=this.latitude;}if(!zoomLevel){zoomLevel=this.zoomLevel;}return Math.cos(DegToRad(latitude))*this._MetersPerPixel(zoomLevel);}
VE_MapControl.prototype.Resize=function(width,height){if(!width||!height||width<=0||height<=0){return;}this.width=width;this.height=height;this.element.style.width=width;this.element.style.height=height;this.PanToLatLong(this.latitude,this.longitude);this._RepositionCopyright();}
VE_MapControl.prototype._ClearTiles=function(){var t=this.tiles;var u=this.unusedTiles;var m=this.map;if(!t||t.length==0)return;while(t.length>0){var tile=t.pop();tile.RemoveFromMap();tile.img.src=VE_MapControl.emptyTile;u.push(tile);}}
VE_MapControl.prototype._TileSize=function(){return 256;}
VE_MapControl.prototype._StartMap=function(){this._ClearTiles();var m=this.map;m.style.top="0px";m.style.left="0px";this.originX=Math.round(this._LonToXAtZ(this.longitude,this.zoomLevel)-this.width/2);this.originY=Math.round(this._LatToYAtZ(this.latitude,this.zoomLevel)-this.height/2);this.offsetX=0;this.offsetY=0;var s=this._TileSize();var ox=this.originX;var oy=this.originY;var b=VE_MapControl.buffer;var w=this.width;var h=this.height;var x1=Math.floor((ox-b)/s);var y1=Math.floor((oy-b)/s);var x2=Math.floor((ox+w+b)/s);var y2=Math.floor((oy+h+b)/s);x1=Math.max(0,x1);y1=Math.max(0,y1);for(var x=x1;x<=x2;x++)for(var y=y1;y<=y2;y++){this._RequestTile(x,y,this.zoomLevel,s,this.mapStyle);}}
VE_MapControl.prototype._UpdateMap=function(){var m=this.map;var s=this._TileSize();var ox=this.originX+this.offsetX;var oy=this.originY+this.offsetY;var w=this.width;var h=this.height;var b=VE_MapControl.buffer;var t=this.tiles;var x1=Math.floor((ox-b)/s);var y1=Math.floor((oy-b)/s);var x2=Math.floor((ox+w+b)/s);var y2=Math.floor((oy+h+b)/s);var oldx1=0;var oldy1=0;var oldx2=0;var oldy2=0;var first=true;var newTiles=new Array();for(var i=0;i<t.length;i++){var tile=t[i];var x=tile.tx;var y=tile.ty;if(x<x1||x>x2||y<y1||y>y2){tile.RemoveFromMap();}else{if(first){oldx1=x;oldy1=y;oldx2=x;oldy2=y;first=false;}else{oldx1=Math.min(oldx1,x);oldy1=Math.min(oldy1,y);oldx2=Math.max(oldx2,x);oldy2=Math.max(oldy2,y);}newTiles.push(tile);}}this.tiles=newTiles;x1=Math.max(0,x1);y1=Math.max(0,y1);for(var x=x1;x<=x2;x++)for(var y=y1;y<=y2;y++){if(x<oldx1||x>oldx2||y<oldy1||y>oldy2)this._RequestTile(x,y,this.zoomLevel,s,this.mapStyle);}}
VE_MapControl.worldBounds={z:1,x1:0,y1:0,x2:2,y2:2}
;VE_MapControl.usaBounds={z:10,x1:172,y1:356,x2:332,y2:444}
;VE_MapControl.prototype._GetBounds=function(zoom){if(zoom>=VE_MapControl.usaBounds.z)return VE_MapControl.usaBounds;return VE_MapControl.worldBounds;}
VE_MapControl.prototype._ClipDeltaX=function(deltaX){var b=this._GetBounds(this.zoomLevel);return this._ClipDelta(deltaX,this.width,this.originX+this.offsetX,b.z,b.x1,b.x2);}
VE_MapControl.prototype._ClipDeltaY=function(deltaY){var b=this._GetBounds(this.zoomLevel);return this._ClipDelta(deltaY,this.height,this.originY+this.offsetY,b.z,b.y1,b.y2);}
VE_MapControl.prototype._ClipDelta=function(delta,size,offset,bz,b1,b2){var z=this.zoomLevel;var s=this._TileSize();var min=s*(b1<<(z-bz));var max=s*(b2<<(z-bz));if(size>(max-min)){return((max-min-size)/2)-offset+min;}else if(offset+delta<min){return min-offset;}else if(offset+size+delta>max){return max-offset-size;}return delta;}
VE_MapControl.prototype._ClipLatitude=function(latitude,zoom){var y=this._LatToYAtZ(latitude,zoom);var b=this._GetBounds(zoom);y=this._ClipAbsolute(y,zoom,this.height,b.z,b.y1,b.y2);return this._YToLatAtZ(y,zoom);}
VE_MapControl.prototype._ClipLongitude=function(longitude,zoom){var x=this._LonToXAtZ(longitude,zoom);var b=this._GetBounds(zoom);x=this._ClipAbsolute(x,zoom,this.width,b.z,b.x1,b.x2);return this._XToLonAtZ(x,zoom);}
VE_MapControl.prototype._ClipAbsolute=function(abs,z,size,bz,b1,b2){var s=this._TileSize();var min=s*(b1<<(z-bz));var max=s*(b2<<(z-bz));var half=size/2;if(size>max-min){return(max-min)/2;}else if(abs<min+half){return min+half;}else if(abs+half>max){return max-half;}return abs;}
VE_MapControl.prototype._ValidateZoomLevel=function(latitude,longitude,z){if(z<VE_MapControl.minZoom){return VE_MapControl.minZoom;}if(z<VE_MapControl.usaBounds.z){return z;}var y=this._LatToYAtZ(latitude,z);var x=this._LonToXAtZ(longitude,z);var b=VE_MapControl.usaBounds;var s=this._TileSize();var minX=s*(b.x1<<(z-b.z));var maxX=s*(b.x2<<(z-b.z));var minY=s*(b.y1<<(z-b.z));var maxY=s*(b.y2<<(z-b.z));if(minX<x&&x<maxX&&minY<y&&y<maxY){return Math.min(VE_MapControl.maxZoom,z);}else{return Math.min(VE_MapControl.usaBounds.z-1,z);}}
VE_MapControl.GetMouseX=function(e){var posX=0;if(e.pageX){posX=e.pageX;}else if(e.clientX){posX=e.clientX+document.body.scrollLeft;}return posX;}
VE_MapControl.GetMouseY=function(e){var posY=0;if(e.pageY){posY=e.pageY;}else if(e.clientY){posY=e.clientY+document.body.scrollTop;}return posY;}
VE_MapControl.GetMouseScrollDelta=function(e){if(e.wheelDelta){return e.wheelDelta;}else if(e.detail){return-e.detail;}return 0;}
VE_MapControl.GetTarget=function(e){if(!e)e=window.event;var t=null;if(e.srcElement)t=e.srcElement;else if(e.target)t=e.target;if(t.nodeType==3)t=targ.parentNode;return t;}
VE_MapControl.prototype._CreateCopyright=function(){var bg=document.createElement("div");bg.unselectable="on";with(bg.style){position="absolute";width="230px";height="28px";top="1px";left="1px";zIndex=30;color="Black";display="none";font="7pt Verdana";textAlign="right";cursor="pointer";}var fg=document.createElement("div");fg.unselectable="on";with(fg.style){position="absolute";width="230px";height="28px";top="0px";left="0px";zIndex=31;color="White";display="none";font="7pt Verdana";textAlign="right";cursor="pointer";}this.copyrightBg=bg;this.copyrightFg=fg;this._RepositionCopyright();this.element.appendChild(bg);this.element.appendChild(fg);}
VE_MapControl.prototype._RepositionCopyright=function(){with(this.copyrightBg.style){top=(this.height-28)+"px";left=(this.width-240)+"px";display="block";}with(this.copyrightFg.style){top=(this.height-29)+"px";left=(this.width-241)+"px";display="block";}}
VE_MapControl.prototype._UpdateCopyright=function(){var z=this.zoomLevel;var s=this.mapStyle;var c="&copy; 2005 Microsoft Corp";if(z>=5&&(s=="r"||s=="h")){c+=" &nbsp; &copy; 2004 NAVTEQ";}if(s=="h"||s=="a"){c+="<br>Image courtesy of ";if(z<=9){c+="NASA";}else{c+="USGS";}}this.copyrightBg.innerHTML=c;this.copyrightFg.innerHTML=c;this._RepositionCopyright();}
VE_MapControl.prototype._MetersPerPixel=function(zl){return VE_MapControl.baseMetersPerPixel/(1<<zl);}
VE_MapControl.prototype._XToLonAtZ=function(x,zl){var metersPerPixel=this._MetersPerPixel(zl);var metersX=x*metersPerPixel-VE_MapControl.offsetMeters;return RadToDeg(metersX/VE_MapControl.earthRadius);}
VE_MapControl.prototype._LonToXAtZ=function(longitude,zl){var metersX=VE_MapControl.earthRadius*DegToRad(longitude);var metersPerPixel=this._MetersPerPixel(zl);return Math.round((metersX+VE_MapControl.offsetMeters)/metersPerPixel);}
VE_MapControl.prototype._YToLatAtZ=function(y,zl){var metersPerPixel=this._MetersPerPixel(zl);var metersY=VE_MapControl.offsetMeters-y*metersPerPixel;return RadToDeg(Math.PI/2-2*Math.atan(Math.exp(-metersY/VE_MapControl.earthRadius)));}
VE_MapControl.prototype._LatToYAtZ=function(latitude,zl){var sinLat=Math.sin(DegToRad(latitude));var metersY=VE_MapControl.earthRadius/2*Math.log((1+sinLat)/(1-sinLat));var metersPerPixel=this._MetersPerPixel(zl);return Math.round((VE_MapControl.offsetMeters-metersY)/metersPerPixel);}
VE_MapControl.prototype._XToLon=function(x){return this._XToLonAtZ(x,this.zoomLevel);}
VE_MapControl.prototype._LonToX=function(longitude){return this._LonToXAtZ(longitude,this.zoomLevel);}
VE_MapControl.prototype._YToLat=function(y){return this._YToLatAtZ(y,this.zoomLevel);}
VE_MapControl.prototype._LatToY=function(latitude){return this._LatToYAtZ(latitude,this.zoomLevel);}
VE_MapControl.prototype._XToXAtZ=function(x,z1,z2){var lon=this._XToLonAtZ(x,z1);return this._LonToXAtZ(lon,z2);}
VE_MapControl.prototype._YToYAtZ=function(y,z1,z2){var lat=this._YToLatAtZ(y,z1);return this._LatToYAtZ(lat,z2);}
function DegToRad(d){return d*Math.PI/180.0;}
function RadToDeg(r){return r*180.0/Math.PI;}
VE_MapControl.prototype.Debug=function(enabled){var t=this.tiles;for(var i=0;i<t.length;i++){t[i].img.style.border=enabled?"1px dashed red":"0";if(enabled){this.map.appendChild(t[i].overlay);}else{if(t[i].overlay.parentNode==this.map){this.map.removeChild(t[i].overlay);}}}var u=this.unusedTiles;for(var i=0;i<u.length;i++){u[i].img.style.border=enabled?"1px dashed red":"0";}this.debug=enabled;}
function VE_MapEvent(srcMapControl,latitude,longitude,zoomLevel){this.srcMapControl=srcMapControl;this.latitude=latitude;this.longitude=longitude;this.zoomLevel=zoomLevel;}
VE_MapControl.prototype._CreateEvent=function(latitude,longitude,zoomLevel){if(!latitude)latitude=this.GetCenterLatitude();if(!longitude)longitude=this.GetCenterLongitude();if(!zoomLevel)zoomLevel=this.zoomLevel;return new VE_MapEvent(this,latitude,longitude,this.zoomLevel);}
VE_MapControl.prototype._CallMouseDown=function(e){if(!e)e=this._CreateEvent();if(this.onMouseDown)this.onMouseDown(e);}
VE_MapControl.prototype._CallMouseUp=function(e){if(!e)e=this._CreateEvent();if(this.onMouseUp)this.onMouseUp(e);}
VE_MapControl.prototype._CallMouseClick=function(e){if(!e)e=this._CreateEvent();if(this.onMouseClick)this.onMouseClick(e);}
VE_MapControl.prototype._CallStartContinuousPan=function(e){if(!e)e=this._CreateEvent();if(this.onStartContinuousPan)this.onStartContinuousPan(e);}
VE_MapControl.prototype._CallEndContinuousPan=function(e){if(!e)e=this._CreateEvent();if(this.onEndContinuousPan)this.onEndContinuousPan(e);}
VE_MapControl.prototype._CallStartZoom=function(e){if(!e)e=this._CreateEvent();if(this.onStartZoom)this.onStartZoom(e);}
VE_MapControl.prototype._CallEndZoom=function(e){if(!e)e=this._CreateEvent();if(this.onEndZoom)this.onEndZoom(e);}
VE_MapControl.controlList=new Array();VE_MapControl.altitudes=[2000,500,150,50,25,12,6,3,1];VE_MapControl.scaledRadius=5429123.84;VE_MapControl.buffer=100;VE_MapControl.dragElement=null;VE_MapControl.dragControl=null;VE_MapControl.animatedMovementEnabled=1;VE_MapControl.zoomTotalSteps=5;VE_MapControl.tileUrlPrefixes=["http://tiles0.virtualearth.msn.com/tiles/","http://tiles1.virtualearth.msn.com/tiles/","http://tiles2.virtualearth.msn.com/tiles/","http://tiles3.virtualearth.msn.com/tiles/"];VE_MapControl.tileVersion=1;VE_MapControl.keyboardPanSpeed=15;VE_MapControl.panToLatLongSpeed=15;VE_MapControl.earthRadius=6378137;VE_MapControl.minZoom=1;VE_MapControl.maxZoom=19;VE_MapControl.requests=new Array();VE_MapControl.offsetMeters=20971520;VE_MapControl.baseMetersPerPixel=163840;VE_MapControl.emptyTile="i/spacer.gif";VE_MapControl.minLatitude=-85;VE_MapControl.maxLatitude=85;VE_MapControl.minLongitude=-180;VE_MapControl.maxLongitude=180;VE_MapControl.kbInputZIndex=0;VE_MapControl.containerZIndex=0;VE_MapControl.mapZIndex=1;VE_MapTile.baseZIndex=2;VE_MapTile.swapZIndex=1;VE_MapTile.debugZIndex=3;VE_MapPushpin.baseZIndex=11;VE_MapPushpin.topZIndex=20;VE_MapControl.FindControlByInput=function(input){var c=VE_MapControl.controlList;for(var i=0;i<c.length;i++){if(c[i].keyboard==input)return c[i];}return null;}
VE_MapControl._KeyDown=function(e){if(!e)var e=window.event;var t=VE_MapControl.GetTarget(e);var c=VE_MapControl.FindControlByInput(t);var s=VE_MapControl.keyboardPanSpeed;var x=c.panningX;var y=c.panningY;switch(e.keyCode){case 9:if(c.panning&&c.keyboardPan){c.StopContinuousPan();}return true;case 37:x=-s;break;case 38:y=-s;break;case 39:x=s;break;case 40:y=s;break;case 107:case 187:case 61:case 43:x=0;y=0;c.ZoomIn();break;case 109:case 189:x=0;y=0;c.ZoomOut();break;default:return false;}if(x||y)c.ContinuousPan(x,y,null,true);return false;}
VE_MapControl._KeyUp=function(e){if(!e)var e=window.event;var t=VE_MapControl.GetTarget(e);var c=VE_MapControl.FindControlByInput(t);var s=VE_MapControl.keyboardPanSpeed;var x=c.panningX;var y=c.panningY;switch(e.keyCode){case 37:x=0;break;case 38:y=0;break;case 39:x=0;break;case 40:y=0;break;default:return false;}c.ContinuousPan(x,y,null,true);return false;}
VE_MapControl._MouseDown=function(e){if(!e)e=window.event;e.cancelBubble=true;var t=VE_MapControl.GetTarget(e);var c=VE_MapControl.FindControlByTileImage(t);if(!c||c.zooming){return false;}if(c.panning){c.StopContinuousPan();}c.keyboard.focus();c.dragging=true;c.dragPan=false;c.lastMouseX=VE_MapControl.GetMouseX(e);c.lastMouseY=VE_MapControl.GetMouseY(e);t.style.cursor="move";if(t.setCapture)t.setCapture();VE_MapControl.dragElement=t;VE_MapControl.dragControl=c;c._CallStartContinuousPan();return false;}
VE_MapControl._MouseUp=function(e){if(!e)e=window.event;if(VE_MapControl.dragElement){var c=VE_MapControl.dragControl;if(!c){return false;}c.dragging=false;c._ComputeCenterPoint(true);VE_MapControl.dragElement.style.cursor="pointer";if(VE_MapControl.dragElement.releaseCapture)VE_MapControl.dragElement.releaseCapture();if(c.dragPan)c._CallEndContinuousPan();c.dragPan=false;var x=VE_MapControl.GetMouseX(e)-c.x;var y=VE_MapControl.GetMouseY(e)-c.y;c._CallMouseClick(c._CreateEvent(c.GetLatitude(y),c.GetLongitude(x)));}return false;}
VE_MapControl._MouseDoubleClick=function(e){if(!e)e=window.event;var c=VE_MapControl.dragControl;if(!c||c.panning||c.zooming){return false;}var lat=c._YToLat(c.originY+c.offsetY+VE_MapControl.GetMouseY(e)-c.offsetTop-c.y);var lon=c._XToLon(c.originX+c.offsetX+VE_MapControl.GetMouseX(e)-c.offsetLeft-c.x);if(e.altKey){if(c.zoomLevel==VE_MapControl.minZoom)c.PanToLatLong(lat,lon);else c.SetCenterAndZoom(lat,lon,c.zoomLevel-1);}else{if(c.zoomLevel+1!=c._ValidateZoomLevel(lat,lon,c.zoomLevel+1))c.PanToLatLong(lat,lon);else c.SetCenterAndZoom(lat,lon,c.zoomLevel+1);}return false;}
VE_MapControl._MouseMove=function(e){if(!e)e=window.event;if(VE_MapControl.dragElement){var c=VE_MapControl.dragControl;if(!c){return false;}if(c.dragging){var x=VE_MapControl.GetMouseX(e);var y=VE_MapControl.GetMouseY(e);c.PanMap(c.lastMouseX-x,c.lastMouseY-y);c.lastMouseX=x;c.lastMouseY=y;c.dragPan=true;if(e.cancelBubble)e.cancelBubble=true;}}return false;}
VE_MapControl._MouseWheel=function(e){if(!e)e=window.event;var t=VE_MapControl.GetTarget(e);var c=VE_MapControl.FindControlByTileImage(t);if(!c||c.panning||c.zooming){return false;}var delta=VE_MapControl.GetMouseScrollDelta(e);if(delta>0){c.ZoomIn();}else if(delta<0){c.ZoomOut();}e.cancelBubble=true;return false;}
VE_MapControl._ContextMenu=function(e){if(!e)e=window.event;e.cancelBubble=true;var t=VE_MapControl.GetTarget(e);var c=VE_MapControl.FindControlByTileImage(t);if(!c||c.debug){return true;}return false;}
VE_MapControl.prototype.PanMap=function(deltaX,deltaY){if(deltaX==0&&deltaY==0){return;}deltaX=this._ClipDeltaX(deltaX);deltaY=this._ClipDeltaY(deltaY);this.offsetX+=deltaX;this.offsetY+=deltaY;this.map.style.top=-this.offsetY+"px";this.map.style.left=-this.offsetX+"px";this._UpdateMap(deltaX,deltaY);this._UpdateCopyright();}
VE_MapControl.prototype.ContinuousPan=function(deltaX,deltaY,count,keyboardPan){if(this.zooming){return;}if(!count){count=-1;}this.panningX=deltaX;this.panningY=deltaY;this.panCounter=count;if(!deltaX&&!deltaY){this.StopContinuousPan();return;}this.keyboardPan=keyboardPan;if(!this.panning){this.panning=true;VE_MapControl._StepPan(this.index);this._CallStartContinuousPan();}}
VE_MapControl._StepPan=function(controlIndex){var c=VE_MapControl.controlList[controlIndex];if(c.panning){c.PanMap(c.panningX,c.panningY);if(c.panCounter>0)c.panCounter--;if(c.panCounter!=0)window.setTimeout("VE_MapControl._StepPan("+controlIndex+")",10);else c.StopContinuousPan();}}
VE_MapControl.prototype.StopContinuousPan=function(){this.panningX=0;this.panningY=0;this.panning=false;this.keyboardPanning=false;if(this.panLatitude!=null&&this.panLongitude!=null){var dx=this._LonToX(this.panLongitude)-(this.originX+this.offsetX+this.width/2);var dy=this._LatToY(this.panLatitude)-(this.originY+this.offsetY+this.height/2);this.PanMap(dx,dy);this.latitude=this.panLatitude;this.longitude=this.panLongitude;this.preferredLatitude=this.latitude;this.preferredLongitude=this.longitude;this.panLatitude=null;this.panLongitude=null;}else{this._ComputeCenterPoint(true);}this._CallEndContinuousPan();}
VE_MapControl.prototype.PanToLatLong=function(latitude,longitude){this.panLatitude=latitude;this.panLongitude=longitude;var x=this._LonToX(longitude);var y=this._LatToY(latitude);var dx=x-(this.originX+this.offsetX+this.width/2);var dy=y-(this.originY+this.offsetY+this.height/2);var d=Math.sqrt(dx*dx+dy*dy);if(!VE_MapControl.animatedMovementEnabled||Math.abs(dx)>2*this.width||Math.abs(dy)>2*this.height||d>1.5*Math.sqrt(this.width*this.width+this.height*this.height)){this.SetCenter(latitude,longitude);return;}var a=Math.atan2(dy,dx);var s=VE_MapControl.panToLatLongSpeed;var c=Math.ceil(d/s);s=Math.round(d/c);dx=Math.cos(a)*s;dy=Math.sin(a)*s;this.ContinuousPan(dx,dy,c);}
VE_MapControl.prototype.AddPushpin=function(id,lat,lon,width,height,className,innerHtml,zIndex){var x=Math.round(this._LonToX(lon)-this.originX);var y=Math.round(this._LatToY(lat)-this.originY);var pin=null;if(this.unusedPushpins.length>0){pin=this.unusedPushpins.pop();pin.Recycle(id,lat,lon,x,y,width,height,className,innerHtml,zIndex);}else{pin=new VE_MapPushpin(this,id,lat,lon,x,y,width,height,className,innerHtml,zIndex);}this.pushpins.push(pin);this.map.appendChild(pin.el);return pin.el;}
VE_MapControl.prototype.RemovePushpin=function(id){for(var i=0;i<this.pushpins.length;i++){var p=this.pushpins[i];if(p.id==id){this.pushpins.splice(i,1);this.map.removeChild(p.el);this.unusedPushpins.push(p);p.el.vePushpin=null;return;}}}
VE_MapControl.prototype.ClearPushpins=function(){var p=this.pushpins;while(p.length>0)this.map.removeChild(p.pop().el);}
VE_MapControl.prototype._RepositionPushpins=function(){for(var i=0;i<this.pushpins.length;i++){var p=this.pushpins[i];var x=Math.round(this._LonToX(p.lon)-this.originX);var y=Math.round(this._LatToY(p.lat)-this.originY);p.SetCurrentState(x,y);p.PrecomputeSteps();p.SetFactor(0);}}
function VE_MapPushpin(c,id,lat,lon,x,y,w,h,className,innerHtml,zIndex){this.c=c;this.id=id;this.lat=lat;this.lon=lon;this.w=w;this.h=h;var n=VE_MapControl.zoomTotalSteps+1;this.xs=new Array(n);this.ys=new Array(n);var el=document.createElement("div");el.id=id;el.vePushpin=this;el.className=className;el.style.position="absolute";el.style.zIndex=zIndex;el.innerHTML=innerHtml;el.ondblclick=VE_MapPushpin._MouseDoubleClick;el.onmousewheel=VE_MapPushpin._MouseWheel;el.unselectable="on";this.el=el;this.SetCurrentState(x,y);this.SetNextState(x,y);this.PrecomputeSteps();this.SetFactor(0);}
VE_MapPushpin.prototype.Recycle=function(id,lat,lon,x,y,w,h,className,innerHtml,zIndex){this.id=id;this.lat=lat;this.lon=lon;this.w=w;this.h=h;this.el.id=id;this.el.className=className;this.el.style.zIndex=zIndex;this.el.innerHTML=innerHtml;this.el.vePushpin=this;this.SetCurrentState(x,y);this.SetNextState(x,y);this.PrecomputeSteps();this.SetFactor(0);}
VE_MapPushpin.prototype.SetCurrentState=function(x,y){this.x=x;this.y=y;}
VE_MapPushpin.prototype.SetNextState=function(x,y){this._x=x;this._y=y;}
VE_MapPushpin.prototype.ClearSteps=function(){var n=VE_MapControl.zoomTotalSteps;for(var i=0;i<=n;i++){this.xs[i]=this.x;this.ys[i]=this.y;}}
VE_MapPushpin.prototype.PrecomputeSteps=function(){var n=VE_MapControl.zoomTotalSteps;for(var i=0;i<=n;i++){var a=i/n;var b=1.0-a;this.xs[i]=Math.floor(b*this.x+a*this._x-this.w/2);this.ys[i]=Math.floor(b*this.y+a*this._y-this.h/2);}}
VE_MapPushpin.prototype.SetFactor=function(i){with(this.el.style){left=this.xs[i];top=this.ys[i];}}
VE_MapPushpin.prototype.SwapStates=function(){var t=0;t=this.x;this.x=this._x;this._x=t;t=this.y;this.y=this._y;this._y=t;}
VE_MapPushpin._MouseDoubleClick=function(e){if(!e){e=window.event;}var t=VE_MapControl.GetTarget(e);if(!t){return false;}var p=t.vePushpin;if(!p){return false;}var c=p.c;if(!c||c.panning||c.zooming){return false;}if(e.altKey){if(c.zoomLevel==VE_MapControl.minZoom)c.PanToLatLong(p.lat,p.lon);else c.SetCenterAndZoom(p.lat,p.lon,c.zoomLevel-1);}else{if(c.zoomLevel==VE_MapControl.maxZoom)c.PanToLatLong(p.lat,p.lon);else c.SetCenterAndZoom(p.lat,p.lon,c.zoomLevel+1);}return false;}
VE_MapPushpin._MouseWheel=function(e){if(!e){e=window.event;}var t=VE_MapControl.GetTarget(e);if(!t){return false;}var p=t.vePushpin;if(!p){return false;}var c=p.c;if(!c||c.panning||c.zooming){return false;}var delta=VE_MapControl.GetMouseScrollDelta(e);if(delta>0){c.ZoomIn();}else if(delta<0){c.ZoomOut();}e.cancelBubble=true;return false;}
VE_MapControl.prototype._RequestTile=function(x,y,zl,size,style){var t=null;var max=1<<zl;if(x<0||y<0||x>=max||y>=max){return;}if(this.unusedTiles.length>0){t=this.unusedTiles.pop();t.Recycle(x,y,zl,style,(x*size-this.originX),(y*size-this.originY),size,size);}else{t=new VE_MapTile(this,x,y,zl,style,(x*size-this.originX),(y*size-this.originY),size,size);}if(t){if(this.debug){t.img.style.border="1px dashed red";}this.tiles.push(t);}}
function PrefixNumber(n){if(n>=0)return"+"+n;return n;}
VE_MapControl.FindControlByTileImage=function(tileImg){if(!tileImg.parentNode){return;}var m=tileImg.parentNode;var c=VE_MapControl.controlList;for(var i=0;i<c.length;i++){if(c[i].map==m)return c[i];}return null;}
function VE_MapTile(c,tx,ty,zl,ms,x,y,w,h){this.c=c;this.tx=tx;this.ty=ty;this.zl=zl;this.ms=ms;this.key=this._TileToQuadKey(zl,tx,ty);this.f=this._BuildFilename();this.id="tile_"+this.key;this.loading=true;this.unused=false;var n=VE_MapControl.zoomTotalSteps+1;this.xs=new Array(n);this.ys=new Array(n);this.ws=new Array(n);this.hs=new Array(n);var img=document.createElement("img");this.img=img;img.id=this.id;img.style.position="absolute";img.style.zIndex=VE_MapTile.baseZIndex;img.style.cursor="pointer";img.onmousedown=VE_MapControl._MouseDown;img.onmouseup=VE_MapControl._MouseUp;img.onmousemove=VE_MapControl._MouseMove;img.onmousewheel=VE_MapControl._MouseWheel;img.ondblclick=VE_MapControl._MouseDoubleClick;img.oncontextmenu=VE_MapControl._ContextMenu;if(window.addEventListener&&navigator.product&&navigator.product=="Gecko"){img.addEventListener("DOMMouseScroll",VE_MapControl._MouseWheel,false);}img.onload=VE_MapTile._Load;img.veTile=this;var overlay=document.createElement("div");this.overlay=overlay;overlay.style.position="absolute";overlay.style.background="white";overlay.style.color="red";overlay.style.width="1px";overlay.style.height="20px";overlay.style.zIndex=VE_MapTile.debugZIndex;overlay.innerHTML=this.key+"<br>"+tx+", "+ty;this.SetCurrentState(x,y,w,h);this.SetNextState(x,y,w,h);this.PrecomputeSteps();img.src=this.f;}
VE_MapTile.prototype._TileToQuadKey=function(zl,tx,ty){var quad="";for(var i=zl;i>0;i--){var mask=1<<(i-1);var cell=0;if((tx&mask)!=0){cell++;}if((ty&mask)!=0){cell+=2;}quad+=cell+"";}return quad;}
VE_MapTile.prototype._BuildFilename=function(){var server=((this.tx&1)+((this.ty&1)<<1))%VE_MapControl.tileUrlPrefixes.length;return VE_MapControl.tileUrlPrefixes[server]+this.ms+this.key+(this.ms=='r'?".png":".jpeg")+"?g="+VE_MapControl.tileVersion;}
VE_MapTile.prototype.Recycle=function(tx,ty,zl,ms,x,y,w,h){this.tx=tx;this.ty=ty;this.zl=zl;this.ms=ms;this.key=this._TileToQuadKey(zl,tx,ty);this.f=this._BuildFilename();this.id="tile_"+this.key;this.overlay.innerHTML=this.key+"<br>"+tx+", "+ty;var w=this.w;var h=this.h;this.SetCurrentState(x,y,w,h);this.SetNextState(x,y,w,h);this.PrecomputeSteps();this.RemoveFromMap();this.loading=true;this.unused=false;this.img.veTile=this;this.img.src=this.f;}
VE_MapTile.prototype.SetCurrentState=function(x,y,w,h){this.x=x;this.y=y;this.w=w;this.h=h;}
VE_MapTile.prototype.SetNextState=function(x,y,w,h){this._x=x;this._y=y;this._w=w;this._h=h;}
VE_MapTile.prototype.ClearSteps=function(){var n=VE_MapControl.zoomTotalSteps;for(var i=0;i<=n;i++){this.xs[i]=this.x;this.ys[i]=this.y;this.ws[i]=this.w;this.hs[i]=this.h;}}
VE_MapTile.prototype.PrecomputeSteps=function(){var n=VE_MapControl.zoomTotalSteps;for(var i=0;i<=n;i++){var a=i/n;var b=1.0-a;this.xs[i]=Math.floor(b*this.x+a*this._x);this.ys[i]=Math.floor(b*this.y+a*this._y);this.ws[i]=Math.ceil(b*this.w+a*this._w);this.hs[i]=Math.ceil(b*this.h+a*this._h);}}
VE_MapTile.prototype.SetFactor=function(i){if(!this.loading){if(i>=this.xs.length)i=this.xs.length-1;with(this.img.style){left=this.xs[i];top=this.ys[i];width=this.ws[i];height=this.hs[i];}with(this.overlay.style){left=this.xs[i];top=this.ys[i];}}}
VE_MapTile.prototype.SwapStates=function(){var t=0;with(this){t=x;x=_x;_x=t;t=y;y=_y;_y=t;t=w;w=_w;_w=t;t=h;h=_h;_h=t;}}
VE_MapTile.prototype.RemoveFromMap=function(){var m=this.c.map;var i=this.img;var o=this.overlay;if(i.parentNode==m){m.removeChild(i);}if(o.parentNode==m){m.removeChild(o);}this.unused=true;this.img.src=VE_MapControl.emptyTile;}
VE_MapTile._Load=function(){if(!this.veTile){return;}with(this.veTile){if(!loading){return;}loading=false;if(!unused){SetFactor(c.zoomCounter);c.map.appendChild(img);if(c.debug){c.map.appendChild(overlay);}}}this.veTile=null;}
VE_MapControl.prototype.SetViewport=function(lat1,lon1,lat2,lon2){var z=0;var w=Math.abs(this._LonToXAtZ(lon1,z)-this._LonToXAtZ(lon2,z));var h=Math.abs(this._LatToYAtZ(lat1,z)-this._LatToYAtZ(lat2,z));while(z<VE_MapControl.maxZoom&&w<this.width&&h<this.height){z++;w=Math.abs(this._LonToXAtZ(lon1,z)-this._LonToXAtZ(lon2,z));h=Math.abs(this._LatToYAtZ(lat1,z)-this._LatToYAtZ(lat2,z));}z=Math.max(0,Math.min(VE_MapControl.maxZoom,z-1));var lat=0.5*(lat1+lat2);var lon=0.5*(lon1+lon2);this.SetCenterAndZoom(lat,lon,z);}
VE_MapControl.prototype.SetBestMapView=function(l){if(!l||l.constructor!=Array){return;}var a=l[0].latitude;var b=l[0].longitude;var c=a;var d=b;for(var i=1;i<l.length;i++){a=Math.min(a,l[i].latitude);b=Math.min(b,l[i].longitude);c=Math.max(c,l[i].latitude);d=Math.max(d,l[i].longitude);}var dLat=(c-a)*0.1;var dLon=(d-b)*0.1;a-=dLat;b-=dLon;c+=dLat;d+=dLon;this.SetViewport(VE_MapControl.ClipLatitude(a),VE_MapControl.ClipLongitude(b),VE_MapControl.ClipLatitude(c),VE_MapControl.ClipLongitude(d));}
VE_MapControl.prototype.IncludePointInViewport=function(lat,lon){this.SetBestMapView(new Array(this,{latitude:lat,longitude:lon}));}
VE_MapControl.ClipLatitude=function(latitude){return VE_MapControl.Clip(latitude,VE_MapControl.minLatitude,VE_MapControl.maxLatitude);}
VE_MapControl.ClipLongitude=function(longitude){return VE_MapControl.Clip(longitude,VE_MapControl.minLongitude,VE_MapControl.maxLongitude);}
VE_MapControl.Clip=function(n,minValue,maxValue){if(n<minValue){return minValue;}if(n>maxValue){return maxValue;}return n;}
VE_MapControl.prototype.SetZoom=function(zoomLevel){zoomLevel=this._ValidateZoomLevel(this.preferredLatitude,this.preferredLongitude,zoomLevel);if(zoomLevel==this.zoomLevel){return;}this.SetCenterAndZoom(this.preferredLatitude,this.preferredLongitude,zoomLevel);}
VE_MapControl.prototype.ZoomIn=function(){if(this.zoomLevel>=VE_MapControl.maxZoom)return;this.SetZoom(this.zoomLevel+1);}
VE_MapControl.prototype.ZoomOut=function(){if(this.zoomLevel<=VE_MapControl.minZoom)return;this.SetZoom(this.zoomLevel-1);}
VE_MapControl.prototype.SetCenterAndZoom=function(latitude,longitude,newZoom){newZoom=this._ValidateZoomLevel(latitude,longitude,newZoom);this.preferredLatitude=latitude;this.preferredLongitude=longitude;if(this.zooming||this.panning||this.dragging){return;}this.zooming=true;this.prevZoomLevel=oldZoom;if(this.zoomLevel!=newZoom)this._CallStartZoom();latitude=this._ClipLatitude(latitude,newZoom);longitude=this._ClipLongitude(longitude,newZoom);this.latitude=latitude;this.longitude=longitude;var dx=(this.width/2)-this.GetX(longitude);var dy=(this.height/2)-this.GetY(latitude);var distance=Math.sqrt(dx*dx+dy*dy);var dynamic=(distance<this.width&&distance<this.height)&&(newZoom==this.zoomLevel-1||newZoom==this.zoomLevel+1)&&VE_MapControl.animatedMovementEnabled;if(dynamic){var oldOriginX=this.originX+this.offsetX;var oldOriginY=this.originY+this.offsetY;var oldZoom=this.zoomLevel;var newOriginX=Math.round(this._LonToXAtZ(longitude,newZoom)-this.width/2);var newOriginY=Math.round(this._LatToYAtZ(latitude,newZoom)-this.height/2);this.oldTiles=this.tiles;this.tiles=new Array();var ot=this.oldTiles;var nt=this.tiles;var pp=this.pushpins;for(var i=0;i<ot.length;i++){ot[i].SetCurrentState(ot[i].x-this.offsetX,ot[i].y-this.offsetY,ot[i].w,ot[i].h,ot[i].o);var x1=Math.floor(this._XToXAtZ(oldOriginX+ot[i].x,oldZoom,newZoom)-newOriginX);var y1=Math.floor(this._YToYAtZ(oldOriginY+ot[i].y,oldZoom,newZoom)-newOriginY);var x2=Math.ceil(this._XToXAtZ(oldOriginX+ot[i].x+ot[i].w,oldZoom,newZoom)-newOriginX);var y2=Math.ceil(this._YToYAtZ(oldOriginY+ot[i].y+ot[i].h,oldZoom,newZoom)-newOriginY);ot[i].SetNextState(x1,y1,x2-x1,y2-y1,100);ot[i].PrecomputeSteps();ot[i].img.style.zIndex=VE_MapTile.baseZIndex;}for(var i=0;i<pp.length;i++){pp[i].SetCurrentState(pp[i].x-this.offsetX,pp[i].y-this.offsetY);var x=Math.floor(this._LonToXAtZ(pp[i].lon,newZoom)-newOriginX);var y=Math.floor(this._LatToYAtZ(pp[i].lat,newZoom)-newOriginY);pp[i].SetNextState(x,y);pp[i].PrecomputeSteps();}this.zoomLevel=newZoom;this.latitude=latitude;this.longitude=longitude;this._StartMap();for(var i=0;i<nt.length;i++){var x1=Math.floor(this._XToXAtZ(newOriginX+nt[i].x,newZoom,oldZoom)-oldOriginX);var y1=Math.floor(this._YToYAtZ(newOriginY+nt[i].y,newZoom,oldZoom)-oldOriginY);var x2=Math.ceil(this._XToXAtZ(newOriginX+nt[i].x+nt[i].w,newZoom,oldZoom)-oldOriginX);var y2=Math.ceil(this._YToYAtZ(newOriginY+nt[i].y+nt[i].h,newZoom,oldZoom)-oldOriginY);nt[i].SetNextState(x1,y1,x2-x1,y2-y1,0);nt[i].SwapStates();nt[i].PrecomputeSteps();nt[i].img.style.zIndex=VE_MapTile.swapZIndex;}this.zoomCounter=0;VE_MapControl._StepAnimatedZoom(this.index);}else{this.oldTiles=this.tiles;this.tiles=new Array();this.zoomLevel=newZoom;this.latitude=latitude;this.longitude=longitude;this._StartMap();this._FinalizeZoom();this._RepositionPushpins();}}
VE_MapControl._StepAnimatedZoom=function(controlIndex){var c=VE_MapControl.controlList[controlIndex];if(!c.zooming)return;if(c.zoomCounter<=VE_MapControl.zoomTotalSteps){var f=c.zoomCounter++;for(var i=0;i<c.oldTiles.length;i++){c.oldTiles[i].SetFactor(f);}for(var i=0;i<c.tiles.length;i++){c.tiles[i].SetFactor(f);}for(var i=0;i<c.pushpins.length;i++){c.pushpins[i].SetFactor(f);}window.setTimeout("VE_MapControl._StepAnimatedZoom("+controlIndex+")",1);}else{c._FinalizeZoom();}}
VE_MapControl.prototype._FinalizeZoom=function(){var t=this.oldTiles;var u=this.unusedTiles;var m=this.map;if(t&&t.length>0){while(t.length>0){var tile=t.pop();tile.RemoveFromMap();u.push(tile);}this.oldTiles=null;}for(var i=0;i<this.tiles.length;i++){this.tiles[i].SwapStates();this.tiles[i].ClearSteps();}for(var i=0;i<this.pushpins.length;i++){this.pushpins[i].SwapStates();this.pushpins[i].ClearSteps();}this.zooming=false;this.zoomCounter=0;this._UpdateCopyright();if(this.prevZoomLevel!=this.zoomLevel)this._CallEndZoom();}
Web.UI.MapStyle =Web.Enum.create('Road','Aerial','Hybrid');Web.UI.ActivationType =Web.Enum.create('None','Hover','Click');Web.UI.VirtualEarthMap =function(associatedElement){Web.UI.VirtualEarthMap.initializeBase(this,[associatedElement]);var _map;var _pushpinActivation =Web.UI.ActivationType.None;var _popupPositioningMode =Web.UI.PositioningMode.BottomLeft;var _pushpins;var _data;var _latitude;var _longitude;var _zoomLevel =0;var _mapStyle =Web.UI.MapStyle.Road;var _width;var _height;var _pushpinImageURL;var _pushpinImageWidth =0;var _pushpinImageHeight =0;var _pushpinCssClass;var _popupCssClass;var _dataValueField;var _dataLatitudeField;var _dataLongitudeField;var _dataImageURLField;var _dataImageURLFormatString ="{0}";var _dataImageWidthField;var _dataImageHeightField;var _dataTextField;var _dataTextFormatString ="{0}";var _dataChangedDelegate;var _popupTemplate;var _queuedPushpinID;var _activatePushpinHandler;var _endContinuousPanHandler;this.endContinuousPan =this.createEvent();this.get_popupPositioningMode =function(){return _popupPositioningMode;}
this.set_popupPositioningMode =function(value){_popupPositioningMode =value;}
this.get_pushpinActivation =function(){return _pushpinActivation;}
this.set_pushpinActivation =function(value){_pushpinActivation =value;}
this.get_pushpins =function(){if (_pushpins ==null){_pushpins =Web.Component.createCollection(this);}
return _pushpins;}
this.get_data =function(){return _data;}
this.set_data =function(value){if (_data &&Web.INotifyCollectionChanged.isImplementedBy(_data)){_data.collectionChanged.remove(_dataChangedDelegate);}
_data =value;if (_data){if (!Web.Data.DataTable.isInstanceOfType(_data)){_data =new Web.Data.DataTable(_data);}
_data.collectionChanged.add(_dataChangedDelegate);}
this.render();this.raisePropertyChanged('data');}
this.get_popupTemplate =function(){return _popupTemplate;}
this.set_popupTemplate =function(template){_popupTemplate =template;this.render();this.raisePropertyChanged('popupTemplate');}
this.get_latitude =function(){if (_map){return _map.GetCenterLatitude();}
return _latitude;}
this.set_latitude =function(value){if (_map){_map.SetCenter(value,this.get_longitude());}
else {_latitude =value;}
}
this.get_longitude =function(){if (_map){return _map.GetCenterLongitude();}
return _longitude;}
this.set_longitude =function(value){if (_map){_map.SetCenter(this.get_latitude(),value);}
else {_longitude =value;}
}
this.get_zoomLevel =function(){if (_map){return _map.GetZoomLevel();}
return _zoomLevel;}
this.set_zoomLevel =function(value){if (_map){_map.SetZoom(value);}
else {_zoomLevel =value;}
}
this.get_width =function(){if (_map){return _map.width;}
return _width;}
this.set_width =function(value){if (_map){_map.Resize(value,this.get_height());}
else {_width =value;}
}
this.get_height =function(){if (_map){return _map.height;}
return _height;}
this.set_height =function(value){if (_map){_map.Resize(this.get_width(),value);}
else {_height =value;}
}
this.get_mapStyle =function(){if (_map){var value =_map.GetMapStyle();switch (value){case "a":return Web.UI.MapStyle.Aerial;case "h":return Web.UI.MapStyle.Hybrid;default:return Web.UI.MapStyle.Road;}
}
return _mapStyle;}
this.set_mapStyle =function(value){if (_map){value =this._parseMapStyle(value);_map.SetMapStyle(value);}
else {_mapStyle =value;}
}
this.get_pushpinImageURL =function(){return _pushpinImageURL;}
this.set_pushpinImageURL =function(value){_pushpinImageURL =value;}
this.get_pushpinImageWidth =function(){return _pushpinImageWidth;}
this.set_pushpinImageWidth =function(value){_pushpinImageWidth =value;}
this.get_pushpinImageHeight =function(){return _pushpinImageHeight;}
this.set_pushpinImageHeight =function(value){_pushpinImageHeight =value;}
this.get_pushpinCssClass =function(){return _pushpinCssClass;}
this.set_pushpinCssClass =function(value){_pushpinCssClass =value;}
this.get_popupCssClass =function(){return _popupCssClass;}
this.set_popupCssClass =function(value){_popupCssClass =value;}
this.get_dataValueField =function(){return _dataValueField;}
this.set_dataValueField =function(value){_dataValueField =value;}
this.get_dataLatitudeField =function(){return _dataLatitudeField;}
this.set_dataLatitudeField =function(value){_dataLatitudeField =value;}
this.get_dataLongitudeField =function(){return _dataLongitudeField;}
this.set_dataLongitudeField =function(value){_dataLongitudeField =value;}
this.get_dataImageURLField =function(){return _dataImageURLField;}
this.set_dataImageURLField =function(value){_dataImageURLField =value;}
this.get_dataImageURLFormatString =function(){return _dataImageURLFormatString;}
this.set_dataImageURLFormatString =function(value){_dataImageURLFormatString =value;}
this.get_dataImageWidthField =function(){return _dataImageWidthField;}
this.set_dataImageWidthField =function(value){_dataImageWidthField =value;}
this.get_dataImageHeightField =function(){return _dataImageHeightField;}
this.set_dataImageHeightField =function(value){_dataImageHeightField =value;}
this.get_dataTextField =function(){return _dataTextField;}
this.set_dataTextField =function(value){_dataTextField =value;}
this.get_dataTextFormatString =function(){return _dataTextFormatString;}
this.set_dataTextFormatString =function(value){_dataTextFormatString =value;}
this.initialize =function(){Web.UI.VirtualEarthMap.callBaseMethod(this,'initialize');if (_popupTemplate){_popupTemplate.initialize();}
if (_pushpins){this._ensureMap();for (var i =0;i <_pushpins.length;i++){this._addPushpin(_pushpins[i]);}
}
this.render();}
this.dispose =function(){if (_popupTemplate){_popupTemplate.dispose();_popupTemplate =null;}
if (_pushpins){for (var i =_pushpins.length -1;i >=0;i--){_pushpins[i].dispose();_pushpins[i]=null;}
_pushpins.clear();_pushpins =null;}
if (_map){_map.onEndContinuousPan =null;_map.ClearPushpins();_map =null;}
Web.UI.VirtualEarthMap.callBaseMethod(this,'dispose');}
this.getDescriptor =function(){var td =Web.UI.VirtualEarthMap.callBaseMethod(this,'getDescriptor');td.addProperty('popupPositioningMode',Web.UI.PositioningMode);td.addProperty('pushpinActivation',Web.UI.ActivationType);td.addProperty('pushpins',Array,true);td.addProperty('data',Object);td.addProperty('popupTemplate',Web.UI.ITemplate);td.addProperty('pushpinImageURL',String);td.addProperty('pushpinImageWidth',Number);td.addProperty('pushpinImageHeight',Number);td.addProperty('pushpinCssClass',String);td.addProperty('popupCssClass',String);td.addProperty('dataValueField',String);td.addProperty('dataLatitudeField',String);td.addProperty('dataLongitudeField',String);td.addProperty('dataImageURLField',String);td.addProperty('dataImageURLFormatString',String);td.addProperty('dataImageWidthField',Number);td.addProperty('dataImageHeightField',Number);td.addProperty('dataTextField',String);td.addProperty('dataTextFormatString',String);td.addProperty('latitude',Number);td.addProperty('longitude',Number);td.addProperty('width',Number);td.addProperty('height',Number);td.addProperty('zoomLevel',Number);td.addProperty('mapStyle',Web.UI.MapStyle);td.addMethod('activatePushpin',[Web.TypeDescriptor.createParameter('id',String)]);td.addMethod('panBy',[Web.TypeDescriptor.createParameter('deltaX',Number),Web.TypeDescriptor.createParameter('deltaY',Number)]);td.addMethod('panTo',[Web.TypeDescriptor.createParameter('latitude',Number),Web.TypeDescriptor.createParameter('longitude',Number)]);td.addMethod('continuousPanBy',[Web.TypeDescriptor.createParameter('deltaX',Number),Web.TypeDescriptor.createParameter('deltaY',Number),Web.TypeDescriptor.createParameter('count',Number)]);td.addMethod('stopContinuousPan');td.addMethod('setViewport',[Web.TypeDescriptor.createParameter('lat1',Number),Web.TypeDescriptor.createParameter('lon1',Number),Web.TypeDescriptor.createParameter('lat2',Number),Web.TypeDescriptor.createParameter('lon2',Number)]);td.addMethod('setBestMapView',[Web.TypeDescriptor.createParameter('locations',Array)]);td.addMethod('includePointInViewport',[Web.TypeDescriptor.createParameter('latitude',Number),Web.TypeDescriptor.createParameter('longitude',Number)]);td.addMethod('zoomIn');td.addMethod('zoomOut');return td;}
function onDataChanged(sender,args){if (args.get_action()!=Web.NotifyCollectionChangedAction.Update){this.render();}
}
_dataChangedDelegate =Function.createDelegate(this,onDataChanged);this.activatePushpin =function(id){if (!_pushpins){return;}
var pp;for (var i =0;i <_pushpins.length;i++){if (_pushpins[i].value ==id){pp =_pushpins[i];}
else {_pushpins[i]._popup.hide();}
}
if (pp){if (_pushpinActivation !=Web.UI.ActivationType.None){_queuedPushpinID =pp.value;}
this.panTo(pp.latitude,pp.longitude);}
}
function activatePushpinHandler(){if (_queuedPushpinID){for (var i =0;i <_pushpins.length;i++){if (_pushpins[i].value ==_queuedPushpinID){_pushpins[i]._popup.show();}
else {_pushpins[i]._popup.hide();}
}
_queuedPushpinID =null;}
}
this.panBy =function(deltaX,deltaY){_map.PanMap(deltaX,deltaY);}
this.panTo =function(latitude,longitude){_map.PanToLatLong(latitude,longitude);}
this.continuousPanBy =function(deltaX,deltaY,count){_map.ContinuousPan(deltaX,deltaY,count);}
this.stopContinuousPanBy =function(){_map.StopContinuousPan();}
this.setViewport =function(lat1,lon1,lat2,lon2){_map.SetViewport(lat1,lon1,lat2,lon2);}
this.setBestMapView =function(locations){_map.SetBestMapView(locations);}
this.includePointInViewport =function(latitude,longitude){_map.IncludePointInViewport(latitude,longitude);}
this.zoomIn =function(){_map.ZoomIn();}
this.zoomOut =function(){_map.ZoomOut();}
this._ensureMap =function(){if (!_map){if (!_width){if (this.element.offsetWidth >0){_width =this.element.offsetWidth;} else {_width =400;}
}
if (!_height){if (this.element.offsetHeight >0){_height =this.element.offsetHeight;} else {_height =400;}
}
_map =new VE_MapControl(_latitude,_longitude,_zoomLevel,this._parseMapStyle(_mapStyle),"relative",0,0,_width,_height);_endContinuousPanHandler =Function.createDelegate(this,endContinuousPanHandler);_map.onEndContinuousPan =_endContinuousPanHandler;_activatePushpinHandler =Function.createDelegate(this,activatePushpinHandler);this.endContinuousPan.add(_activatePushpinHandler);this.element.innerHTML ='';this.element.appendChild(_map.element);}
}
function endContinuousPanHandler(){this.endContinuousPan.invoke(this,Web.EventArgs.Empty);}
this.render =function(){if (!this.get_isInitialized()){return;}
this._ensureMap();var i,element;if (_pushpins){for (i =_pushpins.length -1;i >=0;i--){element =_pushpins[i]._popupTemplate;if (_pushpins[i]._isDataItem &&element){Web.UI.ITemplate.disposeInstance(element);_map.RemovePushpin(_pushpins[i].get_value());_pushpins.removeAt(i);}
}
}
var pushpins =this.get_pushpins();var items =this.get_data();var itemLength =items ?items.get_length():0;if (itemLength >0){for (i =0;i <itemLength;i++){var item =items.getItem(i);var pushpin =this._createPushpin(item.get__rowObject());pushpin._isDataItem =true;this._addPushpin(pushpin,item);pushpins.add(pushpin);}
}
}
this._parseMapStyle =function(value){switch (value){case Web.UI.MapStyle.Aerial:return "a";case Web.UI.MapStyle.Hybrid:return "h";default:return "r";}
}
this._createActivationBehavior =function(popupBehavior){var showHandler;var hideHandler =function(){popupBehavior.hide();}
var behavior;if (_pushpinActivation ==Web.UI.ActivationType.Hover){showHandler =function(){popupBehavior.show();}
behavior =new Web.UI.HoverBehavior();behavior.set_unhoverDelay(500);behavior.set_hoverElement(popupBehavior.control.element);behavior.hover.add(showHandler);behavior.unhover.add(hideHandler);}
else {showHandler =function(){if (popupBehavior.control.get_visible()){popupBehavior.hide();}
else {popupBehavior.show();}
}
behavior =new Web.UI.ClickBehavior();behavior.click.add(showHandler);}
return behavior;}
this._addPushpin =function(pushpin,dataItem){if (_map ==null){return;}
var latitude =pushpin.get_latitude();if (!latitude){latitude =this.get_latitude();}
var longitude =pushpin.get_longitude();if (!longitude){longitude =this.get_longitude();}
var width =pushpin.get_imageWidth();if (!width){width =_pushpinImageWidth;}
var height =pushpin.get_imageHeight();if (!height){height =_pushpinImageHeight;}
if (!pushpin.get_imageURL()){pushpin.set_imageURL(_pushpinImageURL);}
var pushpinElement =_map.AddPushpin(pushpin.get_value(),latitude,longitude,width,height,_pushpinCssClass,pushpin.get_innerHtml(),1000);var popupBehavior,popupContentElement;if (_pushpinActivation !=Web.UI.ActivationType.None){var popupElement =document.createElement("div");popupElement.className =_popupCssClass;this.element.appendChild(popupElement);var popupControl =new Web.UI.Control(popupElement);popupBehavior =new Web.UI.PopupBehavior();popupBehavior.set_parentElement(pushpinElement);popupBehavior.set_positioningMode(_popupPositioningMode);popupControl.get_behaviors().add(popupBehavior);popupBehavior.initialize();popupControl.initialize();popupContentElement =_popupTemplate.createInstance(popupElement,dataItem).instanceElement;var pushpinControl =new Web.UI.Control(pushpinElement);pushpinControl.initialize();var behavior =this._createActivationBehavior(popupBehavior);pushpinControl.get_behaviors().add(behavior);behavior.initialize();}
pushpin._popup =popupBehavior;pushpin._popupTemplate =popupContentElement;}
this._createPushpin =function(dataItem){var innerHtml;var id;var imageURL;var imageWidth;var imageHeight;var text ="";if (_dataValueField){id =dataItem[_dataValueField];}
if (_dataImageURLField){imageURL =String.format(_dataImageURLFormatString,dataItem[_dataImageURLField]);}
if (!imageURL ||imageURL.length ==0){imageURL =_pushpinImageURL;imageWidth =_pushpinImageWidth;imageHeight =_pushpinImageHeight;}
else {if (_dataImageWidthField){imageWidth =dataItem[_dataImageWidthField];}
if (_dataImageHeightField){imageHeight =dataItem[_dataImageHeightField];}
}
if (_dataTextField){text =String.format(_dataTextFormatString,dataItem[_dataTextField]);}
var pushpin =new Web.UI.Pushpin();pushpin.set_value(id);pushpin.set_imageURL(imageURL);pushpin.set_imageWidth(imageWidth);pushpin.set_imageHeight(imageHeight);pushpin.set_text(text);pushpin.set_latitude(dataItem[_dataLatitudeField]);pushpin.set_longitude(dataItem[_dataLongitudeField]);pushpin.initialize();return pushpin;}
}
Type.registerSealedClass('Web.UI.VirtualEarthMap',Web.UI.Control);Web.TypeDescriptor.addType('script','virtualEarthMap',Web.UI.VirtualEarthMap);Web.UI.Pushpin =function(){Web.UI.Pushpin.initializeBase(this,[true]);var _map;var _value;var _latitude;var _longitude;var _imageURL;var _imageWidth;var _imageHeight;var _text;this.get_value =function(){return _value;}
this.set_value =function(value){_value =value;}
this.get_latitude =function(){return _latitude;}
this.set_latitude =function(value){_latitude =value;}
this.get_longitude =function(){return _longitude;}
this.set_longitude =function(value){_longitude =value;}
this.get_imageURL =function(){return _imageURL;}
this.set_imageURL =function(value){_imageURL =value;}
this.get_imageWidth =function(){return _imageWidth;}
this.set_imageWidth =function(value){_imageWidth =value;}
this.get_imageHeight =function(){return _imageHeight;}
this.set_imageHeight =function(value){_imageHeight =value;}
this.get_text =function(){return _text;}
this.set_text =function(value){_text =value;}
this.get_innerHtml =function(){if (_imageURL &&_imageURL.length >0){return String.format("<img src=\"{0}\" alt=\"{1}\" title=\"{2}\" />",_imageURL,_text,_text);}
else {return _text;}
}
this.initialize =function(){Web.UI.Pushpin.callBaseMethod(this,'initialize');if (_map){_map._addPushpin(this);}
}
this.getDescriptor =function(){var td =Web.UI.Pushpin.callBaseMethod(this,'getDescriptor');td.addProperty('value',String);td.addProperty('latitude',Number);td.addProperty('longitude',Number);td.addProperty('imageURL',String);td.addProperty('imageWidth',Number);td.addProperty('imageHeight',Number);td.addProperty('text',String);return td;}
this.setOwner =function(map){_map =map;}
}
Type.registerSealedClass('Web.UI.Pushpin',Web.Component);Web.TypeDescriptor.addType('script','pushpin',Web.UI.Pushpin);