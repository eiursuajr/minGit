if(typeof (RadControlsNamespace)=="undefined"){
RadControlsNamespace={};
}
if(typeof (RadControlsNamespace.DialogOpener)=="undefined"||typeof (RadControlsNamespace.DialogOpener.Version)==null||RadControlsNamespace.DialogOpener.Version<1){
RadControlsNamespace.DialogOpener=function(_1){
this.Id=_1[0];
this._handlerUrl=_1[1];
this.DialogDefinitions=_1[2];
this.AdditionalQueryString=_1[3];
this.UseClassicDialogs=_1[4];
this.DialogParametersMode=_1[5];
this.UseEmbeddedScripts=_1[6];
this.PageGuid=_1[7];
this.Language=_1[8];
this.Skin=_1[9];
GetEditorRadWindowManager().SetOverImage(this.Id+"_OverImg");
};
RadControlsNamespace.DialogOpener.prototype={Open:function(_2,_3,_4,_5,_6){
if(!_5){
_5=this.GetDialogDefinition(_2);
}
var _7={DialogParameters:_5.Parameters,CommonParameters:this.GetCommonParameters(),CustomArguments:_3};
var _8=this.CreateRadWindowInfo(_2,_7,_4,_5,_6);
return GetEditorRadWindowManager().ShowModalWindow(_8);
},GetDialogDefinition:function(_9){
return this.DialogDefinitions[_9];
},GetCommonParameters:function(){
return this.DialogDefinitions["CommonParameters"];
},CreateRadWindowInfo:function(_a,_b,_c,_d,_e){
var _f=new RadWindowInfo();
_f.Url=this.GetFullHandlerUrl(_a);
_f.Width=_d.Width;
_f.Height=_d.Height;
_f.Caption=_d.Title;
_f.IsVisible=true;
_f.Argument=_b;
_f.OnLoadFunc=null;
_f.Param=_e;
_f.Resizable=_d.Resizable;
_f.Movable=_d.Movable;
_f.UseClassicDialogs=this.UseClassicDialogs;
_f.CallbackFunc=_c;
return _f;
},GetFullHandlerUrl:function(_10){
var _11=this._handlerUrl.indexOf("?")<0?"?":"&";
var _12=this._handlerUrl+_11+"DialogName="+_10+"&UseEmbeddedScripts="+this.UseEmbeddedScripts+"&Language="+this.Language+"&Skin="+this.Skin+"&Mode="+this.DialogParametersMode+"";
if(this.DialogParametersMode!=0){
_12+="&PageGuid="+this.PageGuid+"&ControlID="+this.Id;
}
return _12+this.GetFormattedAdditionalQueryString();
},GetFormattedAdditionalQueryString:function(){
var _13=this.AdditionalQueryString.substring(0,1);
if(_13!="&"){
return "&"+this.AdditionalQueryString;
}
return this.AdditionalQueryString;
}};
RadControlsNamespace.DialogOpener.Version=1;
}

//BEGIN_ATLAS_NOTIFY
// Ektron if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
