if(typeof (RadSpellNamespace)=="undefined"){
RadSpellNamespace={};
}
RadSpellNamespace.SpellDialogControl=function(_1){
this.Id=_1[0];
this.Language=_1[1];
RadControlsNamespace.DomEventsMixin.Initialize(this);
this.Suggestions=this.FindChildElement(RadSpellNamespace.SpellDialogControl.SuggestionsID);
this.Suggestions.focus();
this.DisableElement(this.Suggestions);
this.Ignore=this.GetConfiguredButton(RadSpellNamespace.SpellDialogControl.IgnoreID,"IgnoreHandler");
this.IgnoreAll=this.GetConfiguredButton(RadSpellNamespace.SpellDialogControl.IgnoreAllID,"IgnoreAllHandler");
this.AddCustom=this.GetConfiguredButton(RadSpellNamespace.SpellDialogControl.AddCustomID,"AddCustomHandler");
this.Change=this.GetConfiguredButton(RadSpellNamespace.SpellDialogControl.ChangeID,"ChangeHandler");
this.ChangeAll=this.GetConfiguredButton(RadSpellNamespace.SpellDialogControl.ChangeAllID,"ChangeAllHandler");
this.Undo=this.GetConfiguredButton(RadSpellNamespace.SpellDialogControl.UndoID,"UndoHandler");
this.Cancel=this.GetConfiguredButton(RadSpellNamespace.SpellDialogControl.CancelID,"CancelHandler");
this.EnableButton(this.Cancel);
this.Localization=RadSpellNamespace.Localization[this.Language];
this.AttachDomEvent(window,"load","StartSpellCheck");
this.SpellProcessor=null;
this.ContentDisplay=null;
var _2=this;
this.ModeChangedHandler=function(_3,_4){
_2.ContentDisplay_ModeChanged(_4);
};
this.SuggestionsDblClickHandler=function(_5,_6){
_2.Suggestions_DblClick(_6);
};
};
RadSpellNamespace.SpellDialogControl.SpellCheckServiceID="_SpellCheckService";
RadSpellNamespace.SpellDialogControl.TextContainerID="_TextContainer";
RadSpellNamespace.SpellDialogControl.TextEditorID="_TextEditor";
RadSpellNamespace.SpellDialogControl.SuggestionsID="_Suggestions";
RadSpellNamespace.SpellDialogControl.IgnoreID="_Ignore";
RadSpellNamespace.SpellDialogControl.IgnoreAllID="_IgnoreAll";
RadSpellNamespace.SpellDialogControl.AddCustomID="_AddCustom";
RadSpellNamespace.SpellDialogControl.ChangeID="_Change";
RadSpellNamespace.SpellDialogControl.ChangeAllID="_ChangeAll";
RadSpellNamespace.SpellDialogControl.UndoID="_Undo";
RadSpellNamespace.SpellDialogControl.CancelID="_Cancel";
RadSpellNamespace.SpellDialogControl.prototype={IgnoreHandler:function(e){
if(this.ContentDisplay.IsEditMode){
this.ContentDisplay.SwitchMode();
}else{
this.SpellProcessor.Ignore();
this.RaiseOnClientWordIgnoredEvent(false);
this.MoveForward();
}
},IgnoreAllHandler:function(e){
this.SpellProcessor.IgnoreAll();
this.RaiseOnClientWordIgnoredEvent(true);
this.MoveForward();
},AddCustomHandler:function(e){
var _a=this.SpellProcessor.GetCurrentBadWord().wordString;
if(!confirm(this.Localization["AddWord1"]+_a+this.Localization["AddWord2"])){
return;
}
var _b=this.GetService();
var _c=GetDialogArguments(true);
if(_c.DictionaryLanguage){
_b.DictionaryLanguage=_c.DictionaryLanguage;
}
this.DisableAllButtons();
var _d=this;
_b.AddCustomWord(_a,function(_e,_f){
_d.AddCustomResponseReceived(_f);
});
},AddCustomResponseReceived:function(_10){
var _11=this.SpellProcessor.GetCurrentBadWord().wordString;
this.SpellProcessor.ProcessCustomWordAddition();
this.RaiseOnClientCustomWordAddedEvent(_11);
this.ProcessCurrentStep();
},ChangeHandler:function(e){
this.ChangeCurrentWord(this.ContentDisplay.GetReplacementWord());
},ChangeAllHandler:function(e){
if(this.SpellProcessor.AreAllWordsFixed()){
return;
}
this.SpellProcessor.ChangeAll(this.ContentDisplay.GetReplacementWord());
this.RaiseOnClientWordCorrectedEvent(true);
this.MoveForward();
},UndoHandler:function(e){
var _15=this.SpellProcessor.GetLastActionBadWordStartCharOffsets();
this.SpellProcessor.Undo();
this.RaiseOnClientActionUndoneEvent(_15);
this.ProcessCurrentStep();
},CancelHandler:function(e){
this.CloseDialog(this.SpellProcessor.IsTextChanged()&&confirm(this.Localization["Confirm"]));
},DisableButton:function(_17){
this.DisableElement(_17);
this.SetElementClass(_17,"buttonDisabled");
},EnableButton:function(_18){
this.EnableElement(_18);
this.SetElementClass(_18,"button");
},SetButtonEnabled:function(_19,_1a){
if(_1a){
this.EnableButton(_19);
}else{
this.DisableButton(_19);
}
},DisableElement:function(_1b){
_1b.disabled=true;
},EnableElement:function(_1c){
_1c.disabled=false;
},SetElementClass:function(_1d,_1e){
_1d.className=_1e;
},FindChildElement:function(_1f){
return document.getElementById(this.Id+_1f);
},GetConfiguredButton:function(_20,_21){
var _22=this.FindChildElement(_20);
if(_22){
this.AttachDomEvent(_22,"click",_21);
this.DisableButton(_22);
}
return _22;
},GetService:function(){
return window[this.Id+RadSpellNamespace.SpellDialogControl.SpellCheckServiceID];
},ChangeCurrentWord:function(_23){
if(this.SpellProcessor.AreAllWordsFixed()){
return;
}
this.SpellProcessor.Change(_23);
this.RaiseOnClientWordCorrectedEvent(false);
this.MoveForward();
},MoveForward:function(){
this.SpellProcessor.MoveToNextWord();
this.ProcessCurrentStep();
},ProcessCurrentStep:function(){
if(!this.SpellProcessor.AreAllWordsFixed()){
this.UpdateContent();
}else{
this.CloseDialog(true);
}
},PrepareClose:function(_24){
this.DisableAllButtons();
if(this.ContentDisplay){
this.ContentDisplay.DetachEvent("ModeChanged",this.ModeChangedHandler);
this.ContentDisplay.DetachEvent("SuggestionsDblClick",this.SuggestionsDblClickHandler);
this.ContentDisplay.DisposeDomEvents();
this.ContentDisplay=null;
}
this.RaiseDialogClosingEvents();
if(this.SpellProcessor){
if(this.SpellProcessor.AreAllWordsFixed()){
this.RadSpell.SetSpellChecked(true);
}
if(_24){
this.ApplyChanges();
}
this.SpellProcessor=null;
}
this.RadSpell.RaiseEvent("OnClientDialogClosing",{});
this.RadSpell=null;
this.DisposeDomEvents();
this.DetachWindowClosingHandler();
},CloseDialog:function(_25){
this.PrepareClose(_25);
CloseDlg();
},StartSpellCheck:function(){
var _26=GetDialogArguments(true);
this.RadSpell=_26.RadSpell;
this.TextSource=_26.TextSource;
this.AttachWindowClosingHandler();
var _27=this.GetService();
if(_26.DictionaryLanguage){
_27.DictionaryLanguage=_26.DictionaryLanguage;
}
this.TextToCheck=this.EscapeNewLines(this.TextSource.GetText());
var _28=this;
_27.SpellCheck(this.TextToCheck,function(_29,_2a){
_28.SpellCheckResponseReceived(_2a);
});
},AttachWindowClosingHandler:function(){
var _2b=this;
window.radWindow.OnClientClosing=function(){
_2b.PrepareClose(false);
};
},DetachWindowClosingHandler:function(){
window.radWindow.OnClientClosing=null;
},SpellCheckResponseReceived:function(_2c){
this.SpellProcessor=new RadSpellNamespace.SpellProcessor(this.TextToCheck,_2c.BadWords,_2c.WordOffsets);
if(!this.SpellProcessor.AreAllWordsFixed()){
this.InitContentDisplay();
this.UpdateContent();
}else{
this.CloseDialog(true);
}
},EscapeNewLines:function(_2d){
var _2e=_2d.replace(/\n/gi,"<telerikcr />");
_2e=_2e.replace(/\r/gi,"<teleriklf />");
return _2e;
},UnEscapeNewLines:function(_2f){
var _30=_2f.replace(/\<telerikcr\s*\/\>/gi,"\n");
var _30=_30.replace(/\<teleriklf\s*\/\>/gi,"\r");
return _30;
},UpdateContent:function(){
if(this.ContentDisplay.IsEditMode){
this.ContentDisplay.SwitchMode();
}
this.ContentDisplay.ShowText(this.SpellProcessor.GetCurrentErrorContent());
this.FocusCurrentError();
this.ContentDisplay.FillSuggestions(this.SpellProcessor.GetCurrentSuggestions());
this.UpdateButtonStates();
},FocusCurrentError:function(){
var _31=document.getElementById(this.SpellProcessor.GetHighlightedElementId());
if(_31){
if(window.radWindow==null&&_31.scrollIntoView!=null){
_31.scrollIntoView();
}else{
if(!this.ContentDisplay.IsEditMode){
this.ContentDisplay.TextContainer.scrollTop=_31.offsetTop;
_31.focus();
}
}
}
},DisableAllButtons:function(){
this.DisableButton(this.Ignore);
this.DisableButton(this.IgnoreAll);
this.DisableButton(this.Change);
this.DisableButton(this.ChangeAll);
this.DisableButton(this.Undo);
this.DisableButton(this.Cancel);
if(this.AddCustom){
this.DisableButton(this.AddCustom);
}
},IsElementEnabled:function(_32){
return !_32.disabled;
},InitContentDisplay:function(){
this.ContentDisplay=new RadSpellNamespace.ContentDisplay(this,this.FindChildElement(RadSpellNamespace.SpellDialogControl.TextContainerID),this.FindChildElement(RadSpellNamespace.SpellDialogControl.TextEditorID),this.Suggestions);
this.ContentDisplay.AttachEvent("ModeChanged",this.ModeChangedHandler);
this.ContentDisplay.AttachEvent("SuggestionsDblClick",this.SuggestionsDblClickHandler);
this.ContentDisplay.Focus();
},UpdateButtonStates:function(){
var _33=this.SpellProcessor.GetCurrentSuggestions().length>0;
this.EnableButton(this.Ignore);
this.SetButtonEnabled(this.IgnoreAll,!this.ContentDisplay.IsEditMode);
this.SetButtonEnabled(this.Undo,this.SpellProcessor.GetCurrentUndoAction()!=null);
this.SetButtonEnabled(this.Change,this.ContentDisplay.IsEditMode||_33);
this.SetButtonEnabled(this.ChangeAll,!this.ContentDisplay.IsEditMode&&_33);
if(this.AddCustom!=null){
this.SetButtonEnabled(this.AddCustom,!this.ContentDisplay.IsEditMode);
}
this.Ignore.innerHTML=this.ContentDisplay.IsEditMode?this.Localization["UndoEdit"]:this.Localization["Ignore"];
},ContentDisplay_ModeChanged:function(_34){
var _35="";
if(_34.IsEditMode){
_35=this.SpellProcessor.GetCurrentBadWord().wordString;
}else{
_35=this.SpellProcessor.GetCurrentErrorContent();
}
this.UpdateButtonStates();
this.ContentDisplay.ShowText(_35);
this.FocusCurrentError();
},Suggestions_DblClick:function(_36){
if(this.SpellProcessor.GetCurrentSuggestions().length>0){
this.ChangeCurrentWord(_36.SelectedValue);
}else{
alert(this.Localization["Nosuggestions"]);
}
},ApplyChanges:function(){
this.TextSource.SetText(this.UnEscapeNewLines(this.SpellProcessor.TextToCheck));
},RaiseDialogClosingEvents:function(){
if(this.SpellProcessor&&this.SpellProcessor.AreAllWordsFixed()){
var _37={SuppressCompleteMessage:false};
this.RadSpell.RaiseEvent("OnClientCheckFinished",_37);
if(!_37.SuppressCompleteMessage){
alert(this.Localization["SpellCheckComplete"]);
}
}else{
this.RadSpell.RaiseEvent("OnClientCheckCancelled",{});
}
},RaiseOnClientWordCorrectedEvent:function(_38){
var _39=this.SpellProcessor.GetCurrentBadWord();
var _3a={originalWord:_39.originalWordString,newWord:_39.wordString,offsets:this.SpellProcessor.GetLastActionBadWordStartCharOffsets(),changeAll:_38};
this.RadSpell.RaiseEvent("OnClientWordCorrected",_3a);
},RaiseOnClientWordIgnoredEvent:function(_3b){
var _3c=this.SpellProcessor.GetCurrentBadWord();
var _3d={ignoreWord:_3c.originalWordString,offsets:this.SpellProcessor.GetLastActionBadWordStartCharOffsets(),ignoreAll:_3b};
this.RadSpell.RaiseEvent("OnClientWordIgnored",_3d);
},RaiseOnClientActionUndoneEvent:function(_3e){
var _3f=this.SpellProcessor.GetCurrentBadWord();
var _40={originalWord:_3f.originalWordString,offsets:_3e};
this.RadSpell.RaiseEvent("OnClientActionUndone",_40);
},RaiseOnClientCustomWordAddedEvent:function(_41){
var _42={customWord:_41};
this.RadSpell.RaiseEvent("OnClientCustomWordAdded",_42);
}};

//BEGIN_ATLAS_NOTIFY
// Ektron if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
