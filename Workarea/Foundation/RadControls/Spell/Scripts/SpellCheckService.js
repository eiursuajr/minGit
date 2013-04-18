if(typeof (RadSpellNamespace)=="undefined"){
RadSpellNamespace={};
}
function GetSpellCheckService(_1){
return window[_1];
}
RadSpellNamespace.SpellCheckService=function(_2){
this.Initialized=false;
this.Id=_2[0];
this._callbackUrl=_2[1];
this.DictionaryLanguage=_2[2];
this.Configuration=_2[3];
this._waitingForResponse=false;
this._xmlHttpRequest=null;
this._onResponseReceivedHandler=null;
this._onErrorHandler=null;
RadControlsNamespace.DomEventsMixin.Initialize(this);
};
RadSpellNamespace.SpellCheckService.prototype={SpellCheck:function(_3,_4,_5){
var _6=this.SetupXmlHttpRequest(_4,_5);
if(_6){
this.SendXmlHttpRequest(this.GetSpellCheckPostArguments(_3));
}
return _6;
},AddCustomWord:function(_7,_8,_9){
var _a=this.SetupXmlHttpRequest(_8,_9);
if(_a){
this.SendXmlHttpRequest(this.GetAddCustomPostArguments(_7));
}
return _a;
},SetupXmlHttpRequest:function(_b,_c){
if(!this._waitingForResponse){
this._waitingForResponse=true;
this._onResponseReceivedHandler=RadControlsNamespace.EventMixin.ResolveFunction(_b);
this._onErrorHandler=_c?RadControlsNamespace.EventMixin.ResolveFunction(_c):this.SpellErrorHandler;
return true;
}
return false;
},SendXmlHttpRequest:function(_d){
if(typeof (XMLHttpRequest)!="undefined"){
this._xmlHttpRequest=new XMLHttpRequest();
}else{
if(typeof (ActiveXObject)!="undefined"){
this._xmlHttpRequest=new ActiveXObject("Microsoft.XMLHTTP");
}else{
return;
}
}
this._xmlHttpRequest.onreadystatechange=this.CreateEventHandler("HandleCallback");
this._xmlHttpRequest.open("POST",this.GetTimeStampedCallbackUrl(),true);
this._xmlHttpRequest.setRequestHeader("Content-Type","application/x-www-form-urlencoded");
this._xmlHttpRequest.send(_d);
},GetTimeStampedCallbackUrl:function(){
var _e=this._callbackUrl.indexOf("?")<0?"?":"&";
return this._callbackUrl+_e+"RadUploadTimeStamp="+new Date().getTime();
},HandleCallback:function(){
if(this._xmlHttpRequest.readyState!=4){
return;
}
this._waitingForResponse=false;
this.ProcessCallbackResponse();
this.Cleanup();
},ProcessCallbackResponse:function(){
if(this.ErrorOccured()){
this.RaiseErrorEvent();
return;
}
var _f=this._xmlHttpRequest.responseText;
if(_f){
var _10=null;
try{
eval(_f);
_10=SpellCheckResult;
}
catch(ex){
this.RaiseErrorEvent({"ErrorCode":500,"ErrorText":ex.message,"Message":ex.message,"Text":_f,"Xml":this._xmlHttpRequest.responseXml});
return;
}
this._onResponseReceivedHandler(this,_10);
}
},ErrorOccured:function(){
if(!document.all){
return false;
}
if(this._xmlHttpRequest.status==0||this._xmlHttpRequest.status==200){
return false;
}
return true;
},SpellErrorHandler:function(_11,_12){
var _13="";
if(_12.ErrorCode==404){
_13="RadSpell error. SpellCheckHandler was not found: \r\n\r\n"+"Please, see the help for more details: RadSpell 3.x - Configuration - SpellCheckHandler.\r\n\r\n"+this._callbackUrl+"\r\n\r\nDid you register the SpellCheckHandler in web.config?";
}else{
if(_12.ErrorCode>0&&_12.ErrorCode!=200){
_13="RadSpell error. Server returned error: "+_12.ErrorCode+" \r\n\r\n"+"Please, see the help for more details: RadSpell 3.x - Configuration - SpellCheckHandler.\r\n\r\n"+this._callbackUrl+"\r\n\r\n"+_12.ErrorText+"\r\n\r\n"+_12.Text+"\r\n\r\n";
}
}
alert(_13);
},RaiseErrorEvent:function(_14){
if(!_14){
_14={"ErrorCode":this._xmlHttpRequest.status,"ErrorText":this._xmlHttpRequest.statusText,"Message":this._xmlHttpRequest.statusText,"Text":this._xmlHttpRequest.responseText,"Xml":this._xmlHttpRequest.responseXml};
}
this._onErrorHandler(this,_14);
},Cleanup:function(){
this._onResponseReceivedHandler=null;
this._onErrorHandler=null;
this._xmlHttpRequest.onreadystatechange=function(){
};
},GetSpellCheckPostArguments:function(_15){
return "DictionaryLanguage="+this.EncodeData(this.DictionaryLanguage)+"&Configuration="+this.EncodeData(this.Configuration)+"&CommandArgument="+this.EncodeData(_15)+"&CommandName=SpellCheck";
},EncodeData:function(_16){
return (encodeURIComponent)?encodeURIComponent(_16):escape(_16);
},GetAddCustomPostArguments:function(_17){
return "DictionaryLanguage="+this.EncodeData(this.DictionaryLanguage)+"&Configuration="+this.EncodeData(this.Configuration)+"&CommandArgument="+this.EncodeData(_17)+"&CommandName=AddCustom";
}};

//BEGIN_ATLAS_NOTIFY
// Ektron if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
