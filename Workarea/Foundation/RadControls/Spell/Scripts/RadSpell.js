function GetRadSpell(_1){
return window[_1];
}
if(typeof (RadSpellNamespace)=="undefined"){
RadSpellNamespace={};
}
RadSpellNamespace.RadSpell=function(_2){
this.Id=_2[0];
this.OnClientCheckStarted=_2[1];
this.OnClientCheckFinished=_2[2];
this.OnClientWordCorrected=_2[3];
this.OnClientWordIgnored=_2[4];
this.OnClientActionUndone=_2[5];
this.OnClientCheckCancelled=_2[6];
this.OnClientDialogClosing=_2[7];
this.OnClientCustomWordAdded=_2[8];
this.ControlToCheck=_2[9];
this.DictionaryLanguage=null;
this._dictionaryLanguage=_2[10];
this.ClientTextSource=_2[11];
RadControlsNamespace.EventMixin.Initialize(this);
RadControlsNamespace.DomEventsMixin.Initialize(this);
this.LanguagesDropdown=document.getElementById(this.Id+RadSpellNamespace.RadSpell.LanguagesDropdownID);
this.SpellCheckButton=document.getElementById(this.Id+RadSpellNamespace.RadSpell.SpellCheckButtonID);
if(this.SpellCheckButton){
this.AttachDomEvent(this.SpellCheckButton,"click","StartSpellCheck");
if(this.SpellCheckButton.tagName!="input"){
this.AttachDomEvent(this.SpellCheckButton,"keypress","StartSpellCheck");
}
}
this._isSpellChecked=false;
this.AttachEvent("OnClientCheckFinished",function(_3,_4){
this._isSpellChecked=true;
});
this.AttachEvent("OnClientCheckStarted",function(_5,_6){
this._isSpellChecked=false;
});
};
RadSpellNamespace.RadSpell.LanguagesDropdownID="_Language";
RadSpellNamespace.RadSpell.SpellCheckButtonID="_SpellCheck";
RadSpellNamespace.RadSpell.DialogOpenerID="_DialogOpener";
RadSpellNamespace.RadSpell.SpellCheckedID="_SpellChecked";
RadSpellNamespace.RadSpell.prototype={StartSpellCheck:function(){
var _7=this.GetTextSource();
if(_7!=null){
this.SpellCheck(_7);
}else{
alert("Cannot find a TextSource. Please, set the ControlToCheck server-side property, or use the SetTextSource() client-side method.");
}
},SpellCheck:function(_8){
if(this.RaiseEvent("OnClientCheckStarted",{})==false){
return;
}
var _9={DictionaryLanguage:this.GetSelectedLanguage(),TextSource:_8,RadSpell:this};
this.GetDialogOpener().Open("SpellCheckDialog",_9);
},GetSelectedLanguage:function(){
if(this.DictionaryLanguage){
return this.DictionaryLanguage;
}
if(!this.LanguagesDropdown){
return null;
}
var _a=this.LanguagesDropdown.options[this.LanguagesDropdown.selectedIndex];
return _a!=null?_a.value:null;
},GetTextSource:function(){
if(this._source==null){
if(this.ControlToCheck){
var _b=document.getElementById(this.ControlToCheck);
this.SetTextSource(new HtmlElementTextSource(_b));
}else{
if(this.ClientTextSource){
this.SetTextSource(eval(this.ClientTextSource));
}
}
}
return this._source;
},IsSpellChecked:function(){
return this._isSpellChecked;
},SetSpellChecked:function(_c){
document.getElementById(this.Id+RadSpellNamespace.RadSpell.SpellCheckedID).value="true";
this._isSpellChecked=_c;
},SetTextSource:function(_d){
this._source=_d;
},GetDialogOpener:function(){
return window[this.Id+RadSpellNamespace.RadSpell.DialogOpenerID];
}};

//BEGIN_ATLAS_NOTIFY
// Ektron if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
