if(typeof (RadSpellNamespace)=="undefined"){
RadSpellNamespace={};
}
if(typeof (RadSpellNamespace.Localization)=="undefined"){
RadSpellNamespace.Localization=[];
}
RadSpellNamespace.Localization.ProcessRawArray=function(_1){
var _2=_1[0];
if(typeof (RadSpellNamespace.Localization[_2])=="undefined"){
RadSpellNamespace.Localization[_2]=[];
}
for(var i=1;i<_1.length;i+=2){
RadSpellNamespace.Localization[_2][_1[i]]=_1[i+1];
}
};

//BEGIN_ATLAS_NOTIFY
// Ektron if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
