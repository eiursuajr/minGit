if(typeof (RadSpellNamespace)=="undefined"){
RadSpellNamespace={};
}
RadSpellNamespace.SpellProcessor=function(_1,_2,_3){
this.TextToCheck=_1;
this.BadWords=_2;
this.WordOffsets=_3;
this.CurrentBadWordIndex=0;
this.HighlightedElementIdPrefix="spell_highlight_";
this.UndoActions=[];
this.SurroundingWordsCount=10;
};
RadSpellNamespace.SpellProcessor.CHANGE=0;
RadSpellNamespace.SpellProcessor.CHANGEALL=1;
RadSpellNamespace.SpellProcessor.IGNORE=2;
RadSpellNamespace.SpellProcessor.IGNOREALL=3;
RadSpellNamespace.SpellProcessor.ADDCUSTOM=4;
RadSpellNamespace.SpellProcessor.prototype={MoveToNextWord:function(){
var _4=this.GetNextBadWordIndex();
if(_4<0){
return;
}
this.CurrentBadWordIndex=_4;
},MoveToPreviousWord:function(){
this.CurrentBadWordIndex=this.GetNextBadWordIndex(0);
},GetNextBadWordIndex:function(_5){
if(typeof (_5)!="undefined"){
}else{
_5=this.CurrentBadWordIndex;
}
for(var i=_5;i<this.BadWords.length;i++){
if(this.GetBadWord(i).originalWordString==null){
return i;
}
}
return -1;
},AreAllWordsFixed:function(){
return this.GetNextBadWordIndex()<0;
},GetCurrentErrorContent:function(){
var _7=this.GetCurrentBadWord();
return this.GetStartString(this.CurrentBadWordIndex,true)+"<a style='border-bottom: 1px dotted red;font-weight: bold;' id="+this.GetHighlightedElementId()+">"+_7.wordString+"</a>"+this.GetEndString(this.CurrentBadWordIndex,true);
},GetHighlightedElementId:function(){
return this.HighlightedElementIdPrefix+this.GetCurrentBadWord().wordString;
},GetStartString:function(_8,_9){
var _a=0;
var _b=this.GetBadWord(_8);
if(_9&&(_b.textOffset>this.SurroundingWordsCount)){
var _c=_b.textOffset-this.SurroundingWordsCount;
_a=this.WordOffsets[_c];
}
return this.TextToCheck.substring(_a,this.GetWordStartCharIndex(_8));
},GetEndString:function(_d,_e){
var _f=this.TextToCheck.length;
var _10=this.GetBadWord(_d);
if(_e&&(this.WordOffsets.length>_10.textOffset+this.SurroundingWordsCount)){
var _11=_10.textOffset+this.SurroundingWordsCount;
_f=this.WordOffsets[_11];
}
return this.TextToCheck.substring(this.GetWordEndCharIndex(_d),_f);
},GetWordStartCharIndex:function(_12){
return this.WordOffsets[this.GetWordOffset(_12)];
},GetWordEndCharIndex:function(_13){
return this.GetWordStartCharIndex(_13)+this.GetBadWord(_13).wordString.length;
},GetBadWord:function(_14){
return this.BadWords[_14];
},GetCurrentBadWord:function(){
return this.GetBadWord(this.CurrentBadWordIndex);
},GetWordOffset:function(_15){
return this.GetBadWord(_15).textOffset;
},GetCurrentWordOffset:function(){
return this.GetCurrentBadWord().textOffset;
},GetCurrentSuggestions:function(){
return this.GetCurrentBadWord().suggestionsString;
},Ignore:function(){
this.ChangeWord(this.GetCurrentBadWord().wordString,RadSpellNamespace.SpellProcessor.IGNORE);
},IgnoreAll:function(){
this.ChangeAllWords(this.GetCurrentBadWord().wordString,RadSpellNamespace.SpellProcessor.IGNOREALL);
},Change:function(_16){
this.ChangeWord(_16,RadSpellNamespace.SpellProcessor.CHANGE);
},ChangeAll:function(_17){
this.ChangeAllWords(_17,RadSpellNamespace.SpellProcessor.CHANGEALL);
},ProcessCustomWordAddition:function(){
var _18=this.GetSameWordStringBadWordIndexes();
for(var i=_18.length-1;i>=0;i--){
var _1a=_18[i];
this.BadWords.splice(_1a,1);
}
this.MoveToNextWord();
},ChangeWord:function(_1b,_1c){
var _1d=[this.CurrentBadWordIndex];
this.ProcessChange(_1d,_1b,false,_1c);
this.RegisterUndoStep(_1d);
},ChangeAllWords:function(_1e,_1f){
var _20=this.GetSameWordStringBadWordIndexes();
this.ProcessChange(_20,_1e,false,_1f);
this.RegisterUndoStep(_20);
},ChangeText:function(_21,_22){
this.TextToCheck=this.GetStartString(_21,false)+_22+this.GetEndString(_21,false);
},GetOffsetsByBadWordIndexes:function(_23){
var _24=[];
for(var i=0;i<_23.length;i++){
_24[_24.length]=this.GetWordOffset(_23[i]);
}
return _24;
},GetLastActionBadWordOffsets:function(){
var _26=this.GetCurrentUndoAction();
return this.GetOffsetsByBadWordIndexes(_26);
},GetLastActionBadWordStartCharOffsets:function(){
var _27=[];
var _28=this.GetLastActionBadWordOffsets();
for(var i=0;i<_28.length;i++){
_27[_27.length]=this.WordOffsets[_28[i]];
}
return _27;
},GetSameWordStringBadWordIndexes:function(){
var _2a=this.GetCurrentBadWord().wordString;
var _2b=[this.CurrentBadWordIndex];
for(var _2c=this.CurrentBadWordIndex+1;_2c<this.BadWords.length;_2c++){
if(this.GetBadWord(_2c).wordString==_2a){
_2b[_2b.length]=_2c;
}
}
return _2b;
},Undo:function(){
var _2d=this.GetCurrentUndoAction();
var _2e=this.GetBadWord(_2d[0]).originalWordString;
this.ProcessChange(_2d,_2e,true);
this.UnregisterUndoStep();
this.MoveToPreviousWord();
},CorrectWord:function(_2f,_30,_31){
if(this.GetBadWord(_2f).originalWordString){
return;
}
var _32=this.GetBadWord(_2f);
_32.originalWordString=_32.wordString;
_32.wordString=_30;
_32.correctionAction=_31;
},RestoreWord:function(_33){
var _34=this.GetBadWord(_33);
_34.wordString=_34.originalWordString;
_34.originalWordString=null;
_34.correctionAction=null;
},ProcessChange:function(_35,_36,_37,_38){
for(var i=0;i<_35.length;i++){
var _3a=_35[i];
var _3b=this.GetBadWord(_3a);
var _3c=_36.length-_3b.wordString.length;
this.AdjustOffsetsAfterChangedWord(_3b,_3c);
this.ChangeText(_3a,_36);
if(_37){
this.RestoreWord(_3a);
}else{
this.CorrectWord(_3a,_36,_38);
}
}
},AdjustOffsetsAfterChangedWord:function(_3d,_3e){
for(var _3f=_3d.textOffset+1;_3f<this.WordOffsets.length;_3f++){
this.WordOffsets[_3f]+=_3e;
}
},GetCurrentUndoAction:function(){
if(this.UndoActions.length==0){
return null;
}
return this.UndoActions[this.UndoActions.length-1];
},RegisterUndoStep:function(_40){
this.UndoActions[this.UndoActions.length]=_40;
},UnregisterUndoStep:function(){
this.UndoActions=this.UndoActions.slice(0,-1);
},IsTextChanged:function(){
for(var i=0;i<this.CurrentBadWordIndex;i++){
var _42=this.GetBadWord(i).correctionAction;
if(_42==RadSpellNamespace.SpellProcessor.CHANGE||_42==RadSpellNamespace.SpellProcessor.CHANGEALL){
return true;
}
}
return false;
}};

//BEGIN_ATLAS_NOTIFY
// Ektron if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
