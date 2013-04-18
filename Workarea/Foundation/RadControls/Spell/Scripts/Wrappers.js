if(typeof (window.RadSpellNamespace)=="undefined"){
window.RadSpellNamespace=new Object();
}
window.RadSpellWrappers=new Object();
RadSpellNamespace.escapeNewLines=function(_1){
var _2=_1.replace(/\n/gi,"<telerikcr />");
_2=_2.replace(/\r/gi,"<teleriklf />");
return _2;
};
RadSpellNamespace.unEscapeNewLines=function(_3){
var _4=_3.replace(/\<telerikcr\s*\/\>/gi,"\n");
var _4=_4.replace(/\<teleriklf\s*\/\>/gi,"\r");
return _4;
};
function HtmlElement(_5,_6){
this.htmlElement=_5;
if(_6==null){
_6=document;
}
this.ownerDocument=_6;
}
HtmlElement.prototype.isEnabled=function(){
return (this.htmlElement.disabled!=true);
};
HtmlElement.prototype.setEnabled=function(_7){
var _8=(_7==true)?"enable":"disable";
this[_8]();
};
HtmlElement.prototype.disabledClassName=function(){
return "";
};
HtmlElement.prototype.enabledClassName=function(){
return "";
};
HtmlElement.prototype.enable=function(){
this.htmlElement.disabled=false;
if(this.enabledClassName()!=""){
this.htmlElement.className=this.enabledClassName();
}
};
HtmlElement.prototype.disable=function(){
this.htmlElement.disabled=true;
if(this.disabledClassName()!=""){
this.htmlElement.className=this.disabledClassName();
}
};
HtmlElement.prototype.isVisible=function(){
return this.htmlElement.style.visibility!="hidden";
};
HtmlElement.prototype.hide=function(){
this.htmlElement.style.visibility="hidden";
};
HtmlElement.prototype.getDisplayStyle=function(){
return this.htmlElement.style.display;
};
HtmlElement.prototype.setDisplayStyle=function(_9){
this.htmlElement.style.display=_9;
};
HtmlElement.prototype.show=function(){
this.htmlElement.style.visibility="visible";
};
HtmlElement.getById=function(_a,_b){
if(_b==null){
_b=document;
}
if(_b.all!=null){
return _b.all[_a];
}
return _b.getElementById(_a);
};
function createElementWrapperClass(){
var _c=function(_d,_e){
this.base=HtmlElement;
this.base(_d,_e);
};
_c.prototype=new HtmlElement();
_c.fromId=function(_f,_10){
var _11=HtmlElement.getById(_f,_10);
if(_11==null){
return null;
}else{
return new _c(_11,_10);
}
};
return _c;
}
RadSpellWrappers.Div=createElementWrapperClass();
RadSpellWrappers.Div.prototype.getText=function(){
return this.htmlElement.innerHTML;
};
RadSpellWrappers.Div.prototype.setText=function(_12){
this.htmlElement.innerHTML=_12;
};
RadSpellWrappers.Div.prototype.setClickHandler=function(_13){
this.htmlElement.onclick=_13;
};
RadSpellWrappers.HtmlSelect=createElementWrapperClass();
RadSpellWrappers.HtmlSelect.prototype.optionCount=function(){
return this.htmlElement.options.length;
};
RadSpellWrappers.HtmlSelect.prototype.addOption=function(_14,_15){
var _16=new Option(_14,_15);
if(this.htmlElement.options.add!=null){
this.htmlElement.options.add(_16);
}else{
this.htmlElement.options[this.optionCount()]=_16;
}
};
RadSpellWrappers.HtmlSelect.prototype.clearOptions=function(){
this.htmlElement.options.length=0;
};
RadSpellWrappers.HtmlSelect.prototype.getSelectedIndex=function(){
return this.htmlElement.selectedIndex;
};
RadSpellWrappers.HtmlSelect.prototype.setSelectedIndex=function(_17){
this.htmlElement.selectedIndex=_17;
};
RadSpellWrappers.HtmlSelect.prototype.getItem=function(_18){
return this.htmlElement.options[_18];
};
RadSpellWrappers.TextArea=createElementWrapperClass();
RadSpellWrappers.TextArea.prototype.getText=function(){
return this.htmlElement.value;
};
RadSpellWrappers.TextArea.prototype.setText=function(_19){
this.htmlElement.value=_19;
};
RadSpellWrappers.IFrame=createElementWrapperClass();
RadSpellWrappers.IFrame.prototype.getText=function(){
var _1a=this.innerBody().innerHTML;
return _1a!=null?_1a:"";
};
RadSpellWrappers.IFrame.prototype.setText=function(_1b){
this.innerBody().innerHTML=_1b;
};
RadSpellWrappers.IFrame.prototype.innerDocument=function(){
var _1c=this.htmlElement.id;
var _1d=null;
if(this.ownerDocument.frames!=null&&this.ownerDocument.frames[_1c]!=null){
_1d=this.ownerDocument.frames[_1c];
}
if(_1d==null){
_1d=HtmlElement.getById(_1c,this.ownerDocument);
}
if(_1d.document!=null){
return _1d.document;
}else{
return _1d.contentWindow.document;
}
};
RadSpellWrappers.IFrame.prototype.innerBody=function(){
return this.innerDocument().body;
};
RadSpellWrappers.IFrame.prototype.innerWindow=function(){
var _1e=this.htmlElement.id;
var _1f=null;
if(this.ownerDocument.frames!=null&&this.ownerDocument.frames[_1e]!=null){
return this.ownerDocument.frames[_1e];
}
if(_1f==null){
var _20=HtmlElement.getById(_1e,this.ownerDocument);
return _20.contentWindow;
}
return null;
};
RadSpellWrappers.IFrame.prototype.isEditable=function(){
return typeof (this.innerBody().contentEditable)!="undefined";
};
RadSpellWrappers.IFrame.prototype.setEditable=function(_21){
if(this.isEditable()){
this.innerBody().contentEditable=_21;
}
};
RadSpellWrappers.IFrame.prototype.getEditable=function(){
if(!this.isEditable()){
return false;
}else{
return this.innerBody().contentEditable=="true"||this.innerBody().contentEditable==true;
}
};
RadSpellWrappers.Button=createElementWrapperClass();
RadSpellWrappers.Button.prototype.disabledClassName=function(){
return "ButtonDisabled";
};
RadSpellWrappers.Button.prototype.enabledClassName=function(){
return "Button";
};
RadSpellWrappers.Button.prototype.getCaption=function(){
return this.htmlElement.innerHTML;
};
RadSpellWrappers.Button.prototype.setCaption=function(_22){
this.htmlElement.innerHTML=_22;
};

//BEGIN_ATLAS_NOTIFY
// Ektron if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
