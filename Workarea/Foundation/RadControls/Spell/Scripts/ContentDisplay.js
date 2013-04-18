if(typeof (RadSpellNamespace)=="undefined"){
RadSpellNamespace={};
}
RadSpellNamespace.ContentDisplay=function(_1,_2,_3,_4){
RadControlsNamespace.DomEventsMixin.Initialize(this);
RadControlsNamespace.EventMixin.Initialize(this);
this.SpellDialogControl=_1;
this.IsEditMode=false;
this.TextContainer=_2;
this.TextEditor=_3;
this.Suggestions=_4;
this.Localization=_1.Localization;
this.AttachDomEvent(this.TextContainer,"click","SwitchMode");
this.InitSuggestions();
};
RadSpellNamespace.ContentDisplay.prototype={ShowText:function(_5){
if(this.IsEditMode){
this.TextEditor.value=_5;
}else{
this.TextContainer.innerHTML=_5;
}
},Focus:function(){
if(this.IsEditMode){
this.TextEditor.focus();
}else{
this.Suggestions.focus();
}
},FindWordElement:function(id){
var _7=this.display().htmlElement.getElementsByTagName("a");
for(var i=0;i<_7.length;i++){
var _9=_7[i];
if(_9.id=="spell_highlight_"+id){
return _9;
}
}
return null;
},SwitchMode:function(){
this.IsEditMode=!this.IsEditMode;
if(this.IsEditMode){
this.TextContainer.style.display="none";
this.TextEditor.style.display="block";
this.TextEditor.focus();
this.DisableElement(this.Suggestions);
}else{
this.TextContainer.style.display="";
this.TextEditor.style.display="none";
this.EnableElement(this.Suggestions);
}
this.RaiseEvent("ModeChanged",{IsEditMode:this.IsEditMode});
this.Focus();
},GetReplacementWord:function(){
if(this.IsEditMode){
return this.TextEditor.value;
}else{
return this.GetSelectedSuggestion();
}
},GetSelectedSuggestion:function(){
return this.Suggestions.options[this.Suggestions.selectedIndex].value;
},FillSuggestions:function(_a){
this.ClearSuggestions();
if(_a.length>0){
for(var i=0;i<_a.length;i++){
this.AddSuggestion(_a[i]);
}
}else{
this.AddSuggestion(this.Localization["Nosuggestions"]);
}
this.Suggestions.selectedIndex=0;
},ClearSuggestions:function(){
this.Suggestions.options.length=0;
},AddSuggestion:function(_c){
this.Suggestions.options[this.Suggestions.options.length]=new Option(_c,_c);
},EnableElement:function(_d){
this.SpellDialogControl.EnableElement(_d);
},DisableElement:function(_e){
this.SpellDialogControl.DisableElement(_e);
},InitSuggestions:function(){
this.EnableElement(this.Suggestions);
this.AttachDomEvent(this.Suggestions,"dblclick","OnSuggestionsDblClick");
},OnSuggestionsDblClick:function(){
this.RaiseEvent("SuggestionsDblClick",{SelectedValue:this.GetSelectedSuggestion()});
}};

//BEGIN_ATLAS_NOTIFY
// Ektron if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
