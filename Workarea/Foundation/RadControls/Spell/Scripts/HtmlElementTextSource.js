function HtmlElementTextSource(_1){
if(_1==null){
alert("Could not find target HTML element. Please verify that ControlToCheck points to a valid control.");
this.element={value:""};
}else{
if(_1.nodeName=="IFRAME"){
var _2=new RadSpellWrappers.IFrame(_1,document);
this.element=_2.innerDocument().body;
}else{
this.element=_1;
}
}
}
HtmlElementTextSource.prototype={GetText:function(){
var _3="";
if(this.element.value!=null){
try{
_3=this.element.value;
}
catch(exc){
alert("Error getting text from control.\n"+exc.message);
}
}else{
if(this.element.innerHTML!=null){
try{
_3=this.element.innerHTML;
}
catch(exc){
alert("Error getting HTML from the control.\n"+exc.message);
}
}else{
alert("No value or innerHTML attribute. Cannot access text.");
}
}
return _3;
},SetText:function(_4){
if(this.element.value!=null){
this.element.value=_4;
}else{
if(this.element.innerHTML!=null){
this.element.innerHTML=_4;
}else{
alert("No value or innerHTML attribute. Cannot access text.");
}
}
},getText:function(){
return this.GetText();
},setText:function(_5){
this.SetText(_5);
}};
HtmlElementTextSource.replaceSpecial=function(_6){
var _7=new Array("&euro;","&cent;","&pound;","&yen;","&curren;","&copy;","&reg;","&trade;","&plusmn;","&ne;","&asymp;","&le;","&ge;","&divide;","&times;","&infin;","&frac12;","&frac14;","&frac34;","&sup2;","&sup3;","&permil;","&para;","&sect;","&alpha;","&beta;","&Delta;","&micro;","&Omega;","&sum;","&Oslash;","&ang;","&ordm;","&laquo;","&raquo;","&middot;","&bull;","&dagger;","&Dagger;","&fnof;");
var _8=new Array("\xe2\u201a\xac","\xc2\xa2","\xc2\xa3","\xc2\xa5","\xc2\xa4","\xc2\xa9","\xc2\xae","\xe2\u201e\xa2","\xc2\xb1","\xe2\u2030\xa0","\xe2\u2030\u02c6","\xe2\u2030\xa4","\xe2\u2030\xa5","\xc3\xb7","\xc3\u2014","\xe2\u02c6\u017e","\xc2\xbd","\xc2\xbc","\xc2\xbe","\xc2\xb2","\xc2\xb3","\xe2\u20ac\xb0","\xc2\xb6","\xc2\xa7","\xce\xb1","\xce\xb2","\xce\u201d","\xc2\xb5","\xce\xa9","\xe2\u02c6\u2018","\xc3\u02dc","\xe2\u02c6\xa0","\xc2\xba","\xc2\xab","\xc2\xbb","\xc2\xb7","\xe2\u20ac\xa2","\xe2\u20ac\xa0","\xe2\u20ac\xa1","\xc6\u2019");
for(var i=0;i<_7.length;i++){
_6=_6.replace(new RegExp(_8[i]),_7[i],"ig");
}
return _6;
};

//BEGIN_ATLAS_NOTIFY
// Ektron if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
