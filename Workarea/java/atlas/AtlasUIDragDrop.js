//-----------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------
// AtlasUIDragDrop.js
// Atlas Drag and Drop UI Framework.
Web.UI._DragDropManager =function(){var _instance;this._getInstance =function(){if (_instance ==null){if (Web.Application.get_type()==Web.ApplicationType.InternetExplorer){_instance =new Web.UI.IEDragDropManager();}
else {_instance =new Web.UI.GenericDragDropManager();}
_instance.initialize();}
return _instance;}
this.startDragDrop =function(dragSource,dragVisual,context){this._getInstance().startDragDrop(dragSource,dragVisual,context);}
this.registerDropTarget =function(target){this._getInstance().registerDropTarget(target);}
this.unregisterDropTarget =function(target){this._getInstance().unregisterDropTarget(target);}
}
Web.UI.DragDropManager =new Web.UI._DragDropManager();Web.UI.IDragSource =function(){this.get_dataType =Function.abstractMethod;this.get_data =Function.abstractMethod;this.get_dragMode =Function.abstractMethod;this.onDragStart =Function.abstractMethod;this.onDrag =Function.abstractMethod;this.onDragEnd =Function.abstractMethod;}
Type.registerInterface('Web.UI.IDragSource');Web.UI.IDropTarget =function(){this.get_dropTargetElement =Function.abstractMethod;this.canDrop =Function.abstractMethod;this.drop =Function.abstractMethod;this.onDragEnterTarget =Function.abstractMethod;this.onDragLeaveTarget =Function.abstractMethod;this.onDragInTarget =Function.abstractMethod;}
Type.registerInterface('Web.UI.IDropTarget');Web.UI.DragMode =Web.Enum.create('Copy','Move');Web.UI.IEDragDropManager =function(){Web.UI.IEDragDropManager.initializeBase(this);var _dropTargets;var _radius =10;var _activeDragVisual;var _activeContext;var _activeDragSource;var _underlyingTarget;var _oldOffset;var _potentialTarget;var _isDragging =false;var _mouseUpHandler;var _documentMouseMoveHandler;var _documentDragOverHandler;var _dragStartHandler;var _mouseMoveHandler;var _dragEnterHandler;var _dragLeaveHandler;var _dragOverHandler;var _dropHandler;this.initialize =function(){Web.UI.IEDragDropManager.callBaseMethod(this,'initialize');_mouseUpHandler =Function.createDelegate(this,mouseUpHandler);_documentMouseMoveHandler =Function.createDelegate(this,documentMouseMoveHandler);_documentDragOverHandler =Function.createDelegate(this,documentDragOverHandler);_dragStartHandler =Function.createDelegate(this,dragStartHandler);_mouseMoveHandler =Function.createDelegate(this,mouseMoveHandler);_dragEnterHandler =Function.createDelegate(this,dragEnterHandler);_dragLeaveHandler =Function.createDelegate(this,dragLeaveHandler);_dragOverHandler =Function.createDelegate(this,dragOverHandler);_dropHandler =Function.createDelegate(this,dropHandler);}
Web.UI.IEDragDropManager.registerBaseMethod(this,'initialize');this.dispose =function(){for (var i =0;i <_dropTargets;i++){this.unregisterDropTarget(_dropTargets[i]);}
Web.UI.IEDragDropManager.callBaseMethod(this,'dispose');}
Web.UI.IEDragDropManager.registerBaseMethod(this,'dispose');this.startDragDrop =function(dragSource,dragVisual,context){var ev =window.event;if (_isDragging){return;}
_underlyingTarget =null;_activeDragSource =dragSource;_activeDragVisual =dragVisual;_activeContext =context;var mousePosition ={x:ev.clientX,y:ev.clientY };dragVisual.originalPosition =dragVisual.style.position;if (dragVisual.style.position.length ==0){dragVisual.originalPosition =dragVisual.style.position ="absolute";}
document._lastPosition =mousePosition;dragVisual.startingPoint =mousePosition;var scrollOffset =this.getScrollOffset(dragVisual,true);dragVisual.startingPoint =this.addPoints(dragVisual.startingPoint,scrollOffset);if (dragVisual.style.position =="absolute"){dragVisual.startingPoint =this.subtractPoints(dragVisual.startingPoint,Web.UI.Control.getLocation(dragVisual));}
else {var left =parseInt(dragVisual.style.left);var top =parseInt(dragVisual.style.top);if (isNaN(left))left ="0";if (isNaN(top))top ="0";dragVisual.startingPoint =this.subtractPoints(dragVisual.startingPoint,{x:left,y:top });}
this._prepareForDomChanges();dragSource.onDragStart();this._recoverFromDomChanges();this._wireEvents();this._drag(true);}
Web.UI.IEDragDropManager.registerBaseMethod(this,'startDragDrop');this._stopDragDrop =function(cancelled){var ev =window.event;if (_activeDragSource !=null){this._unwireEvents();if (!cancelled){cancelled =(_underlyingTarget ==null);}
if (!cancelled &&_underlyingTarget !=null){_underlyingTarget.drop(_activeDragSource.get_dragMode(),_activeDragSource.get_dataType(),_activeDragSource.get_data(_activeContext));}
_activeDragSource.onDragEnd(cancelled);_activeDragVisual.style.position =_activeDragVisual.originalPosition;_activeDragSource =null;_activeContext =null;_activeDragVisual =null;_isDragging =false;_potentialTarget =null;ev.returnValue =false;}
}
Web.UI.IEDragDropManager.registerBaseMethod(this,'_stopDragDrop');this._drag =function(isInitialDrag){var ev =window.event;var mousePosition ={x:ev.clientX,y:ev.clientY };document._lastPosition =mousePosition;var scrollOffset =this.getScrollOffset(_activeDragVisual,true);var position =this.addPoints(this.subtractPoints(mousePosition,_activeDragVisual.startingPoint),scrollOffset);if (!isInitialDrag &&parseInt(_activeDragVisual.style.left)==position.x &&parseInt(_activeDragVisual.style.top)==position.y){return;}
Web.UI.Control.setLocation(_activeDragVisual,position);this._prepareForDomChanges();_activeDragSource.onDrag();this._recoverFromDomChanges();_potentialTarget =this._findPotentialTarget(_activeDragSource,_activeDragVisual);var movedToOtherTarget =(_potentialTarget !=_underlyingTarget ||_potentialTarget ==null);if (movedToOtherTarget &&_underlyingTarget !=null){this._leaveTarget(_activeDragSource,_underlyingTarget);}
if (_potentialTarget !=null){if (movedToOtherTarget){_underlyingTarget =_potentialTarget;this._enterTarget(_activeDragSource,_underlyingTarget);}
else {this._moveInTarget(_activeDragSource,_underlyingTarget);}
}
else {_underlyingTarget =null;}
}
Web.UI.IEDragDropManager.registerBaseMethod(this,'_drag');this._wireEvents =function(){document.attachEvent("onmouseup",_mouseUpHandler);document.attachEvent("onmousemove",_documentMouseMoveHandler);document.body.attachEvent("ondragover",_documentDragOverHandler);_activeDragVisual.attachEvent("ondragstart",_dragStartHandler);_activeDragVisual.attachEvent("ondragend",_mouseUpHandler);_activeDragVisual.attachEvent("ondrag",_mouseMoveHandler);}
Web.UI.IEDragDropManager.registerBaseMethod(this,'_wireEvents');this._unwireEvents =function(){_activeDragVisual.detachEvent("ondrag",_mouseMoveHandler);_activeDragVisual.detachEvent("ondragend",_mouseUpHandler);_activeDragVisual.detachEvent("ondragstart",_dragStartHandler);document.body.detachEvent("ondragover",documentDragOverHandler);document.detachEvent("onmousemove",_documentMouseMoveHandler);document.detachEvent("onmouseup",_mouseUpHandler);}
Web.UI.IEDragDropManager.registerBaseMethod(this,'_unwireEvents');this.registerDropTarget =function(dropTarget){if (_dropTargets ==null){_dropTargets =[];}
_dropTargets.add(dropTarget);this._wireDropTargetEvents(dropTarget);}
Web.UI.IEDragDropManager.registerBaseMethod(this,'registerDropTarget');this.unregisterDropTarget =function(dropTarget){this._unwireDropTargetEvents(dropTarget);_dropTargets.remove(dropTarget);dropTarget.dispose();}
Web.UI.IEDragDropManager.registerBaseMethod(this,'unregisterDropTarget');this._wireDropTargetEvents =function(dropTarget){var associatedElement =dropTarget.get_dropTargetElement();associatedElement._dropTarget =dropTarget;associatedElement.attachEvent("ondragenter",_dragEnterHandler);associatedElement.attachEvent("ondragleave",_dragLeaveHandler);associatedElement.attachEvent("ondragover",_dragOverHandler);associatedElement.attachEvent("ondrop",_dropHandler);}
Web.UI.IEDragDropManager.registerBaseMethod(this,'_wireDropTargetEvents');this._unwireDropTargetEvents =function(dropTarget){var associatedElement =dropTarget.get_dropTargetElement();associatedElement._dropTarget =null;associatedElement.detachEvent("ondragenter",_dragEnterHandler);associatedElement.detachEvent("ondragleave",_dragLeaveHandler);associatedElement.detachEvent("ondragover",_dragOverHandler);associatedElement.detachEvent("ondrop",_dropHandler);}
Web.UI.IEDragDropManager.registerBaseMethod(this,'_unwireDropTargetEvents');function dragStartHandler(){var ev =window.event;document.selection.empty();var dt =ev.dataTransfer;var dataType =_activeDragSource.get_dataType().toLowerCase();var data =_activeDragSource.get_data(_activeContext);if (data !=null){if (dataType !="text"&&dataType !="url"){dataType ="text";if (data.innerHTML !=null){data =data.innerHTML;}
}
dt.effectAllowed ="move";dt.setData(dataType,data.toString());}
}
function mouseUpHandler(){this._stopDragDrop(false);}
function documentMouseMoveHandler(){this._dragDrop();}
function documentDragOverHandler(){var ev =window.event;ev.returnValue =(_potentialTarget ==null);}
function mouseMoveHandler(){this._drag();}
function dragEnterHandler(){var ev =window.event;if (_isDragging){ev.returnValue =false;}
else {var dataObjects =Web.UI.IEDragDropManager._getDataObjectsForDropTarget(this._getDropTarget(ev.srcElement));for (var i =0;i <dataObjects.length;i++){this._dropTarget.onDragEnterTarget(Web.UI.DragMode.Copy,dataObjects[i].type,dataObjects[i].value);}
}
}
function dragLeaveHandler(){var ev =window.event;if (_isDragging){ev.returnValue =false;}
else {var dataObjects =Web.UI.IEDragDropManager._getDataObjectsForDropTarget(this._getDropTarget(ev.srcElement));for (var i =0;i <dataObjects.length;i++){this._dropTarget.onDragLeaveTarget(Web.UI.DragMode.Copy,dataObjects[i].type,dataObjects[i].value);}
}
}
function dragOverHandler(){var ev =window.event;if (_isDragging){ev.returnValue =false;}
else {var dataObjects =Web.UI.IEDragDropManager._getDataObjectsForDropTarget(this._getDropTarget(ev.srcElement));for (var i =0;i <dataObjects.length;i++){this._dropTarget.onDragInTarget(Web.UI.DragMode.Copy,dataObjects[i].type,dataObjects[i].value);}
}
}
function dropHandler(){var ev =window.event;if (!_isDragging){var dataObjects =Web.UI.IEDragDropManager._getDataObjectsForDropTarget(this._getDropTarget(ev.srcElement));for (var i =0;i <dataObjects.length;i++){this._dropTarget.drop(Web.UI.DragMode.Copy,dataObjects[i].type,dataObjects[i].value);}
}
ev.returnValue =false;}
this._getDropTarget =function(element){while (element){if (element._dropTarget !=null){return element._dropTarget;}
element =element.parentNode;}
return null;}
this._dragDrop =function(){if (_isDragging){return;}
_isDragging =true;_activeDragVisual.dragDrop();document.selection.empty();}
this._moveInTarget =function(dragSource,dropTarget){this._prepareForDomChanges();dropTarget.onDragInTarget(dragSource.get_dragMode(),dragSource.get_dataType(),dragSource.get_data(_activeContext));this._recoverFromDomChanges();}
this._enterTarget =function(dragSource,dropTarget){this._prepareForDomChanges();dropTarget.onDragEnterTarget(dragSource.get_dragMode(),dragSource.get_dataType(),dragSource.get_data(_activeContext));this._recoverFromDomChanges();}
this._leaveTarget =function(dragSource,dropTarget){this._prepareForDomChanges();dropTarget.onDragLeaveTarget(dragSource.get_dragMode(),dragSource.get_dataType(),dragSource.get_data(_activeContext));this._recoverFromDomChanges();}
this._findPotentialTarget =function(dragSource,dragVisual){var ev =window.event;if (_dropTargets ==null){return null;}
var type =dragSource.get_dataType();var mode =dragSource.get_dragMode();var scrollOffset =this.getScrollOffset(document.body,true);var x =ev.clientX +scrollOffset.x;var y =ev.clientY +scrollOffset.y;var cursorRect ={x:x -_radius,y:y -_radius,width:_radius *2,height:_radius *2 };var targetRect;for (var i =0;i <_dropTargets.length;i++){targetRect =Web.UI.Control.getBounds(_dropTargets[i].get_dropTargetElement());if (Web.UI.Control.overlaps(cursorRect,targetRect)&&_dropTargets[i].canDrop(mode,type)){return _dropTargets[i];}
}
return null;}
this._prepareForDomChanges =function(){_oldOffset =Web.UI.Control.getLocation(_activeDragVisual);}
this._recoverFromDomChanges =function(){var newOffset =Web.UI.Control.getLocation(_activeDragVisual);if (_oldOffset.x !=newOffset.x ||_oldOffset.y !=newOffset.y){_activeDragVisual.startingPoint =this.subtractPoints(_activeDragVisual.startingPoint,this.subtractPoints(_oldOffset,newOffset));scrollOffset =this.getScrollOffset(_activeDragVisual,true);var position =this.addPoints(this.subtractPoints(document._lastPosition,_activeDragVisual.startingPoint),scrollOffset);Web.UI.Control.setLocation(_activeDragVisual,position);}
}
this.addPoints =function(p1,p2){return {x:p1.x +p2.x,y:p1.y +p2.y };}
this.subtractPoints =function(p1,p2){return {x:p1.x -p2.x,y:p1.y -p2.y };}
this.getScrollOffset =function(element,recursive){var left =element.scrollLeft;var top =element.scrollTop;if (recursive){var parent =element.parentNode;while (parent !=null &&parent.scrollLeft !=null){left +=parent.scrollLeft;top +=parent.scrollTop;if (parent ==document.body &&(left !=0 &&top !=0))break;parent =parent.parentNode;}
}
return {x:left,y:top };}
this.getBrowserRectangle =function(){var width =window.innerWidth;var height =window.innerHeight;if (width ==null){width =document.body.clientWidth;}
if (height ==null){height =document.body.clientHeight;}
return {x:0,y:0,width:width,height:height };}
this.getNextSibling =function(item){for (item =item.nextSibling;item !=null;item =item.nextSibling){if (item.innerHTML !=null){return item;}
}
return null;}
this.hasParent =function(element){return (element.parentNode !=null &&element.parentNode.tagName !=null);}
}
Type.registerClass('Web.UI.IEDragDropManager',Web.Component);Web.UI.IEDragDropManager._getDataObjectsForDropTarget =function(dropTarget){if (dropTarget ==null){return [];}
var ev =window.event;var dataObjects =[];var dataTypes =["URL","Text"];var data;for (var i =0;i <dataTypes.length;i++){if (dropTarget.canDrop(Web.UI.DragMode.Copy,dataTypes[i])){data =ev.dataTransfer.getData(dataTypes[i]);if (data !=null){dataObjects.add({type :dataTypes[i],value :data });}
}
}
return dataObjects;}
Web.UI.GenericDragDropManager =function(){Web.UI.GenericDragDropManager.initializeBase(this);var _dropTargets;var _scrollEdgeConst =40;var _scrollByConst =10;var _scroller;var _scrollDeltaX;var _scrollDeltaY;var _activeDragVisual;var _activeContext;var _activeDragSource;var _oldOffset;var _potentialTarget;var _mouseUpHandler;var _mouseMoveHandler;var _keyPressHandler;this.initialize =function(){Web.UI.GenericDragDropManager.callBaseMethod(this,"initialize");_mouseUpHandler =Function.createDelegate(this,mouseUpHandler);_mouseMoveHandler =Function.createDelegate(this,mouseMoveHandler);_keyPressHandler =Function.createDelegate(this,keyPressHandler);if (window.__safari){Web.UI.GenericDragDropManager.__loadSafariCompatLayer(this);}
_scroller =new Web.Timer();_scroller.set_interval(10);_scroller.tick.add(scrollerTickHandler);}
this.startDragDrop =function(dragSource,dragVisual,context){_activeDragSource =dragSource;_activeDragVisual =dragVisual;_activeContext =context;Web.UI.GenericDragDropManager.callBaseMethod(this,"startDragDrop",[dragSource,dragVisual,context]);}
this._stopDragDrop =function(cancelled){_scroller.set_enabled(false);Web.UI.GenericDragDropManager.callBaseMethod(this,"_stopDragDrop",[cancelled]);}
this._drag =function(isInitialDrag){Web.UI.GenericDragDropManager.callBaseMethod(this,"_drag",[isInitialDrag]);this._autoScroll();}
this._wireEvents =function(){document.attachEvent("onmouseup",_mouseUpHandler);document.attachEvent("onmousemove",_mouseMoveHandler);document.attachEvent("onkeypress",_keyPressHandler);}
this._unwireEvents =function(){document.detachEvent("onkeypress",_keyPressHandler);document.detachEvent("onmousemove",_mouseMoveHandler);document.detachEvent("onmouseup",_mouseUpHandler);}
this._wireDropTargetEvents =function(dropTarget){}
this._unwireDropTargetEvents =function(dropTarget){}
function mouseUpHandler(){this._stopDragDrop(false);}
function mouseMoveHandler(){this._drag();}
function keyPressHandler(){var ev =window.event;if (ev.keyCode ==27){this._stopDragDrop(true);}
}
this._autoScroll =function(){var ev =window.event;var browserRect =this.getBrowserRectangle();if (browserRect.width >0){_scrollDeltaX =_scrollDeltaY =0;if (ev.clientX <browserRect.x +_scrollEdgeConst)_scrollDeltaX =-_scrollByConst;else if (ev.clientX >browserRect.width -_scrollEdgeConst)_scrollDeltaX =_scrollByConst;if (ev.clientY <browserRect.y +_scrollEdgeConst)_scrollDeltaY =-_scrollByConst;else if (ev.clientY >browserRect.height -_scrollEdgeConst)_scrollDeltaY =_scrollByConst;if (_scrollDeltaX !=0 ||_scrollDeltaY !=0){_scroller.set_enabled(true);}
else {_scroller.set_enabled(false);}
}
}
function scrollerTickHandler(){var oldLeft =document.body.scrollLeft;var oldTop =document.body.scrollTop;window.scrollBy(_scrollDeltaX,_scrollDeltaY);var newLeft =document.body.scrollLeft;var newTop =document.body.scrollTop;var dragVisual =_activeDragVisual;var position ={x:parseInt(dragVisual.style.left)+(newLeft -oldLeft),y:parseInt(dragVisual.style.top)+(newTop -oldTop)};Web.UI.Control.setLocation(dragVisual,position);}
}
Type.registerSealedClass('Web.UI.GenericDragDropManager',Web.UI.IEDragDropManager);if (window.__safari){Web.UI.GenericDragDropManager.__loadSafariCompatLayer =function(ddm){ddm._getScrollOffset =ddm.getScrollOffset;ddm.getScrollOffset =function(element,recursive){return {x:0,y:0 };}
ddm._getBrowserRectangle =ddm.getBrowserRectangle;ddm.getBrowserRectangle =function(){var browserRect =ddm._getBrowserRectangle();var offset =ddm._getScrollOffset(document.body,true);return {x:browserRect.x +offset.x,y:browserRect.y +offset.y,width:browserRect.width +offset.x,height:browserRect.height +offset.y };}
}
}
Web.UI.RepeatDirection =Web.Enum.create('Horizontal','Vertical');Web.UI.DragDropList =function(){Web.UI.DragDropList.initializeBase(this);var _isDragging;var _acceptedDataTypes =[];var _dataType;var _dragMode;var _dragVisual;var _direction =Web.UI.RepeatDirection.Vertical;var _emptyTemplate;var _emptyTemplateInstance;var _dropCueTemplate;var _dropCueTemplateInstance;var _floatContainerInstance;var _originalParent;var _originalNextSibling;var _originalZIndex;var _currentContext;var _data;this.get_data =function(){return _data;}
this.set_data =function(value){_data =value;}
this.initialize =function(){Web.UI.DragDropList.callBaseMethod(this,'initialize');this.control.element.__dragDropList =this;Web.UI.DragDropManager.registerDropTarget(this);}
this.getDescriptor =function(){var td =Web.UI.DragDropList.callBaseMethod(this,'getDescriptor');td.addProperty("acceptedDataTypes",Array);td.addProperty("data",Object);td.addProperty("dataType",String);td.addProperty("emptyTemplate",Web.UI.ITemplate);td.addProperty("dropCueTemplate",Web.UI.ITemplate);td.addProperty("direction",Web.UI.RepeatDirection);td.addProperty("dragMode",Web.UI.DragMode);return td;}
Web.UI.DragDropList.registerBaseMethod(this,'getDescriptor');this.startDragDrop =function(dragObject,context,dragVisual){if (!_isDragging){_isDragging =true;_currentContext =context;if (dragVisual ==null){dragVisual =this.createDragVisual(dragObject);}
else {_dragVisual =dragVisual;}
Web.UI.DragDropManager.startDragDrop(this,dragVisual,context);}
}
this.createDragVisual =function(dragObject){if (_dragMode ==Web.UI.DragMode.Copy){_dragVisual =dragObject.cloneNode(true);}
else {_dragVisual =dragObject;}
var oldOffset =Web.UI.DragDropManager._getInstance().getScrollOffset(dragObject,true);_dragVisual.style.width =dragObject.offsetWidth +"px";_dragVisual.style.height =dragObject.offsetHeight +"px";_dragVisual.style.opacity ="0.4";_dragVisual.style.filter ="progid:DXImageTransform.Microsoft.BasicImage(opacity=0.4);";_originalZIndex =_dragVisual.style.zIndex;_dragVisual.style.zIndex =99999;_originalParent =_dragVisual.parentNode;_originalNextSibling =Web.UI.DragDropManager._getInstance().getNextSibling(_dragVisual);var currentLocation =Web.UI.Control.getLocation(dragObject,true);var dragVisualContainer =this._getFloatContainer();Web.UI.Control.setLocation(dragVisualContainer,currentLocation);if (Web.UI.DragDropManager._getInstance().hasParent(_dragVisual)){_dragVisual.parentNode.removeChild(_dragVisual);}
dragVisualContainer.appendChild(_dragVisual);var newOffset =Web.UI.DragDropManager._getInstance().getScrollOffset(dragObject,true);if (oldOffset.x !=newOffset.x ||oldOffset.y !=newOffset.y){var diff =Web.UI.DragDropManager._getInstance().subtractPoints(oldOffset,newOffset);Web.UI.Control.setLocation(dragVisualContainer,Web.UI.DragDropManager._getInstance().subtractPoints(currentLocation,diff));}
return dragVisualContainer;}
this.get_emptyTemplate =function(){return _emptyTemplate;}
this.set_emptyTemplate =function(value){_emptyTemplate =value;}
this.get_dataType =function(){return _dataType;}
this.set_dataType =function(value){_dataType =value;}
this.get_data =function(context){return context;}
this.get_dragMode =function(){return _dragMode;}
this.set_dragMode =function(value){_dragMode =value;}
this.dispose =function(){this.control.element.__dragDropList =null;Web.UI.DragDropList.callBaseMethod(this,'dispose');}
this.onDragStart =function(){this._validate();}
this.onDrag =function(){}
this.onDragEnd =function(cancelled){if (_floatContainerInstance !=null){if (_dragMode ==Web.UI.DragMode.Copy){_floatContainerInstance.removeChild(_dragVisual);}
else {_dragVisual.style.opacity ="0.999";_dragVisual.style.filter ="";_dragVisual.style.zIndex =_originalZIndex ?_originalZIndex :0;if (cancelled){_dragVisual.parentNode.removeChild(_dragVisual);if (_originalNextSibling !=null){_originalParent.insertBefore(_dragVisual,_originalNextSibling);}
else {_originalParent.appendChild(_dragVisual);}
}
else {if (_dragVisual.parentNode ==_floatContainerInstance){_dragVisual.parentNode.removeChild(_dragVisual);}
}
}
document.body.removeChild(_floatContainerInstance);}
else {_dragVisual.parentNode.removeChild(_dragVisual);}
if (!cancelled &&_data !=null &&_dragMode ==Web.UI.DragMode.Move){var data =this.get_data(_currentContext);if (_data !=null &&data !=null){_data.remove(data);}
}
_isDragging =false;this._validate();}
this.get_direction =function(){return _direction;}
this.set_direction =function(value){_direction =value;}
this.get_acceptedDataTypes =function(){return _acceptedDataTypes;}
this.set_acceptedDataTypes =function(value){_acceptedDataTypes =value;}
this.get_dropCueTemplate =function(){return _dropCueTemplate;}
this.set_dropCueTemplate =function(value){_dropCueTemplate =value;}
this.get_dropTargetElement =function(){return this.control.element;}
this.canDrop =function(dragMode,dataType){for (var i =0;i <_acceptedDataTypes.length;i++){if (_acceptedDataTypes[i]==dataType){return true;}
}
return false;}
this.drop =function(dragMode,dataType,data){if (dataType =="HTML"&&dragMode ==Web.UI.DragMode.Move){dragVisual =data;var potentialNextSibling =this._findPotentialNextSibling(dragVisual);this._setDropCueVisible(false,dragVisual);dragVisual.parentNode.removeChild(dragVisual);if (potentialNextSibling !=null){this.control.element.insertBefore(dragVisual,potentialNextSibling);}
else {this.control.element.appendChild(dragVisual);}
}
else {this._setDropCueVisible(false);}
if (_data !=null &&data !=null){_data.add(data);}
}
this.onDragEnterTarget =function(dragMode,dataType,data){if (dataType =="HTML"){this._setDropCueVisible(true,data);this._validate();}
}
this.onDragLeaveTarget =function(dragMode,dataType,data){if (dataType =="HTML"){this._setDropCueVisible(false);this._validate();}
}
this.onDragInTarget =function(dragMode,dataType,data){if (dataType =="HTML"){this._setDropCueVisible(true,data);}
}
this._setDropCueVisible =function(visible,dragVisual){if (_dropCueTemplate !=null){if (visible){if (_dropCueTemplateInstance ==null){var documentContext =document.createDocumentFragment();_dropCueTemplateInstance =_dropCueTemplate.createInstance(documentContext).instanceElement;}
var potentialNextSibling =this._findPotentialNextSibling(dragVisual);if (!Web.UI.DragDropManager._getInstance().hasParent(_dropCueTemplateInstance)){if (potentialNextSibling !=null){this.control.element.insertBefore(_dropCueTemplateInstance,potentialNextSibling);}
else {this.control.element.appendChild(_dropCueTemplateInstance);}
_dropCueTemplateInstance.style.width =dragVisual.offsetWidth +"px";_dropCueTemplateInstance.style.height =dragVisual.offsetHeight +"px";}
else {if (Web.UI.DragDropManager._getInstance().getNextSibling(_dropCueTemplateInstance)!=potentialNextSibling){this.control.element.removeChild(_dropCueTemplateInstance);if (potentialNextSibling !=null){this.control.element.insertBefore(_dropCueTemplateInstance,potentialNextSibling);}
else {this.control.element.appendChild(_dropCueTemplateInstance);}
}
}
}
else {if (_dropCueTemplateInstance !=null &&Web.UI.DragDropManager._getInstance().hasParent(_dropCueTemplateInstance)){this.control.element.removeChild(_dropCueTemplateInstance);}
}
}
}
this._findPotentialNextSibling =function(dragVisual){var dragVisualRect =Web.UI.Control.getBounds(dragVisual);var isVertical =(_direction ==Web.UI.RepeatDirection.Vertical);var nodeRect;for (var node =this.control.element.firstChild;node !=null;node =node.nextSibling){if (node.innerHTML !=null &&node !=_dropCueTemplateInstance &&node !=_emptyTemplateInstance){nodeRect =Web.UI.Control.getBounds(node);if ((!isVertical &&dragVisualRect.x <=nodeRect.x)||(isVertical &&dragVisualRect.y <=nodeRect.y)){return node;}
}
}
return null;}
this._validate =function(){var visible =(_dropCueTemplateInstance ==null ||!Web.UI.DragDropManager._getInstance().hasParent(_dropCueTemplateInstance));var count =0;for (var node =this.control.element.firstChild;node !=null;node =node.nextSibling){if (node.innerHTML !=null &&node !=_emptyTemplateInstance &&node !=_dropCueTemplateInstance){count++;}
}
if (count >0){visible =false;}
this._setEmptyTemplateVisible(visible);}
this._setEmptyTemplateVisible =function(visible){if (_emptyTemplate !=null){if (visible){if (_emptyTemplateInstance ==null){_emptyTemplateInstance =_emptyTemplate.createInstance(this.control.element).instanceElement;}
else if (!Web.UI.DragDropManager._getInstance().hasParent(_emptyTemplateInstance)){this.control.element.appendChild(_emptyTemplateInstance);}
}
else {if (_emptyTemplateInstance !=null &&Web.UI.DragDropManager._getInstance().hasParent(_emptyTemplateInstance)){this.control.element.removeChild(_emptyTemplateInstance);}
}
}
}
this._getFloatContainer =function(){if (_floatContainerInstance ==null){_floatContainerInstance =document.createElement(this.control.element.tagName);var none ="0px 0px 0px 0px";_floatContainerInstance.style.position ="absolute";_floatContainerInstance.style.padding =none;_floatContainerInstance.style.margin =none;document.body.appendChild(_floatContainerInstance);}
else if (!Web.UI.DragDropManager._getInstance().hasParent(_floatContainerInstance)){document.body.appendChild(_floatContainerInstance);}
return _floatContainerInstance;}
}
Type.registerSealedClass('Web.UI.DragDropList',Web.UI.Behavior,Web.UI.IDragSource,Web.UI.IDropTarget,Web.IDisposable);Web.TypeDescriptor.addType('script','dragDropList',Web.UI.DragDropList);Web.UI.DataSourceDropTarget =function(){Web.UI.DataSourceDropTarget.initializeBase(this);var _acceptedDataTypes;var _append =true;var _target;var _property ="data";this.get_append =function(){return _append;}
this.set_append =function(value){_append =value;}
this.get_target =function(){return _target;}
this.set_target =function(value){_target =value;}
this.get_property =function(){return _property;}
this.set_property =function(value){_property =value;}
this.get_acceptedDataTypes =function(){return _acceptedDataTypes;}
this.set_acceptedDataTypes =function(value){_acceptedDataTypes =value;}
this.initialize =function(){Web.UI.DataSourceDropTarget.callBaseMethod(this,'initialize');Web.UI.DragDropManager.registerDropTarget(this);}
this.getDescriptor =function(){var td =Web.UI.DataSourceDropTarget.callBaseMethod(this,'getDescriptor');td.addProperty("acceptedDataTypes",Array);td.addProperty("append",Boolean);td.addProperty("target",Object);td.addProperty("property",String);return td;}
this.get_dropTargetElement =function(){return this.control.element;}
this.canDrop =function(dragMode,dataType){for (var i =0;i <_acceptedDataTypes.length;i++){if (_acceptedDataTypes[i]==dataType){return true;}
}
return false;}
this.drop =function(dragMode,type,data){if (data){var p;var target =_target ?_target :this.control;if (_append){p =target["get_"+_property];if (p){var targetData =p();if (targetData){targetData.add(data);} else {p =target["set_"+_property];if (p){p.call(target,data);}
}
}
}
else {p =target["set_"+_property];if (p){p.call(target,data);}
}
}
}
this.onDragEnterTarget =function(dragMode,type,data){}
this.onDragLeaveTarget =function(dragMode,type,data){}
this.onDragInTarget =function(dragMode,type,data){}
}
Type.registerSealedClass('Web.UI.DataSourceDropTarget',Web.UI.Behavior,Web.UI.IDropTarget);Web.TypeDescriptor.addType('script','dataSourceDropTarget',Web.UI.DataSourceDropTarget);Web.UI.DraggableListItem =function(){Web.UI.DraggableListItem.initializeBase(this);var _data;var _handle;var _dragVisualTemplate;var _dragVisualTemplateInstance;this.get_data =function(){if (_data ==null){var dragSource =this._findDragSource();if (dragSource !=null &&dragSource.get_dataType()=="HTML"){return this.control.element;}
}
return _data;}
this.set_data =function(value){_data =value;}
this.get_handle =function(){return _handle;}
this.set_handle =function(value){if (_handle !=null){_handle.detachEvent("onmousedown",this._handleMouseDown);_handle.__draggableBehavior =null;}
if (value.element){value =value.element;}
_handle =value;_handle.__draggableBehavior =this;_handle.attachEvent("onmousedown",this._handleMouseDown);_handle.__draggableBehavior =this;}
this.get_dragVisualTemplate =function(){return _dragVisualTemplate;}
this.set_dragVisualTemplate =function(value){_dragVisualTemplate =value;}
this.getDescriptor =function(){var td =Web.UI.DraggableListItem.callBaseMethod(this,'getDescriptor');td.addProperty("data",Object);td.addProperty("handle",Object,false,Web.Attributes.Element,true);td.addProperty("dragVisualTemplate",Web.UI.ITemplate);return td;}
this._handleMouseDown =function(){_handle.__draggableBehavior._handleMouseDownInternal();}
this._handleMouseDownInternal =function(){var ev =window.testEvent ?window.testEvent :window.event;if (ev.button <=1){var dragSource =this._findDragSource();if (dragSource !=null){var dragVisual =this._createDragVisual();dragSource.startDragDrop(this.control.element,this.get_data(),dragVisual);ev.returnValue =false;}
}
}
this._createDragVisual =function(){var ev =window.testEvent ?window.testEvent :window.event;if (_dragVisualTemplate !=null){if (_dragVisualTemplateInstance ==null){_dragVisualTemplateInstance =_dragVisualTemplate.createInstance(this.control.element).instanceElement;}
else if (!Web.UI.DragDropManager._getInstance().hasParent(_dragVisualTemplateInstance)){this.control.element.appendChild(_dragVisualTemplateInstance);}
var location ={x:ev.clientX,y:ev.clientY };location =Web.UI.DragDropManager._getInstance().addPoints(location,Web.UI.DragDropManager._getInstance().getScrollOffset(document.body,true));Web.UI.Control.setLocation(_dragVisualTemplateInstance,location);}
return _dragVisualTemplateInstance;}
this._findDragSource =function(){var element =this.control.element;while (element !=null){if (element.__dragDropList !=null){return element.__dragDropList;}
element =element.parentNode;}
return null;}
}
Type.registerSealedClass('Web.UI.DraggableListItem',Web.UI.Behavior);Web.TypeDescriptor.addType('script','draggableListItem',Web.UI.DraggableListItem);Web.UI.FloatingBehavior =function(){Web.UI.FloatingBehavior.initializeBase(this);var _handle;var _reparented;var _startLocation;var _mouseDownHandler =Function.createDelegate(this,mouseDownHandler);;this.move =this.createEvent();this.get_handle =function(){return _handle;}
this.set_handle =function(value){if (_handle !=null){_handle.detachEvent("onmousedown",_mouseDownHandler);}
_handle =value;_handle.attachEvent("onmousedown",_mouseDownHandler);}
this.initialize =function(){Web.UI.FloatingBehavior.callBaseMethod(this,'initialize');Web.UI.DragDropManager.registerDropTarget(this);}
this.dispose =function(){if (_handle !=null){_handle.detachEvent("onmousedown",_mouseDownHandler);}
_mouseDownHandler =null;Web.UI.FloatingBehavior.callBaseMethod(this,'dispose');}
this.getDescriptor =function(){var td =Web.UI.DraggableListItem.callBaseMethod(this,'getDescriptor');td.addProperty("handle",Object,false,Web.Attributes.Element,true);td.addEvent("move",true);return td;}
function mouseDownHandler(){var el =this.control.element;_startLocation =Web.UI.Control.getLocation(el);if (!_reparented){_reparented =true;el.style.width =el.offsetWidth +"px";el.style.height =el.offsetHeight +"px";el.style.position ="absolute";document.body.appendChild(el);Web.UI.Control.setLocation(el,_startLocation);}
window.event.returnValue =false;this.startDragDrop(this.control.element);}
this.get_dataType =function(){return "_floatingObject";}
this.get_data =function(context){return null;}
this.get_dragMode =function(){return Web.UI.DragMode.Move;}
this.onDragStart =Function.emptyMethod;this.onDrag =Function.emptyMethod;this.onDragEnd =function(canceled){if (!canceled){var cancelArgs =new Web.CancelEventArgs();this.move.invoke(this,cancelArgs);canceled =cancelArgs.get_canceled();}
if (canceled){Web.UI.Control.setLocation(this.control.element,_startLocation);}
}
this.startDragDrop =function(dragVisual){Web.UI.DragDropManager.startDragDrop(this,dragVisual,null);}
this.get_dropTargetElement =function(){return document.body;}
this.canDrop =function(dragMode,dataType){return (dataType =="_floatingObject");}
this.drop =Function.emptyMethod;this.onDragEnterTarget =Function.emptyMethod;this.onDragLeaveTarget =Function.emptyMethod;this.onDragInTarget =Function.emptyMethod;}
Type.registerSealedClass('Web.UI.FloatingBehavior',Web.UI.Behavior,Web.UI.IDragSource,Web.UI.IDropTarget,Web.IDisposable);Web.TypeDescriptor.addType('script','floatingBehavior',Web.UI.FloatingBehavior);