if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
RadControlsNamespace.DomEventsMixin=function(){
};
RadControlsNamespace.DomEventsMixin.Initialize=function(_1){
_1.AttachDomEvent=this.AttachDomEvent;
_1.DetachDomEvent=this.DetachDomEvent;
_1.DisposeDomEvents=this.DisposeDomEvents;
_1.ClearEventPointers=this.ClearEventPointers;
_1.RegisterForAutomaticDisposal=this.RegisterForAutomaticDisposal;
_1.AutomaticDispose=this.AutomaticDispose;
_1.CreateEventHandler=this.CreateEventHandler;
_1.private_AttachDomEvent=this.private_AttachDomEvent;
_1.ClearEventPointers();
};
RadControlsNamespace.DomEventsMixin.CreateEventHandler=function(_2){
var _3=this;
return function(e){
if(!e){
e=window.event;
}
return _3[_2](e);
};
};
RadControlsNamespace.DomEventsMixin.AttachDomEvent=function(_5,_6,_7){
var _8=this.CreateEventHandler(_7);
this._eventPointers[this._eventPointers.length]=[_5,_6,_8];
this.private_AttachDomEvent(_5,_6,_8);
};
RadControlsNamespace.DomEventsMixin.private_AttachDomEvent=function(_9,_a,_b){
if(_9.attachEvent){
_9.attachEvent("on"+_a,_b);
}else{
if(_9.addEventListener){
_9.addEventListener(_a,_b,false);
}
}
};
RadControlsNamespace.DomEventsMixin.DetachDomEvent=function(_c,_d,_e){
if(_c.detachEvent){
_c.detachEvent("on"+_d,_e);
}
};
RadControlsNamespace.DomEventsMixin.DisposeDomEvents=function(){
for(var i=0;i<this._eventPointers.length;i++){
this.DetachDomEvent(this._eventPointers[i][0],this._eventPointers[i][1],this._eventPointers[i][2]);
}
this.ClearEventPointers();
};
RadControlsNamespace.DomEventsMixin.RegisterForAutomaticDisposal=function(_10){
var me=this;
var _12=this.CreateEventHandler(_10);
var _13=function(){
_12();
me.DisposeDomEvents();
me=null;
};
this.private_AttachDomEvent(window,"unload",_13);
};
RadControlsNamespace.DomEventsMixin.ClearEventPointers=function(){
this._eventPointers=[];
};

//BEGIN_ATLAS_NOTIFY
// Ektron if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
