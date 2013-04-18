/* Copyright Ektron, Inc. 07/12/07 */function eWebEditProSectionType(){extendObject(this,eWebEditProInstanceType);this.description="Content in rectangular block with edit button";
 this.type="section";this.parameterPropertyNames=[];this.refreshStatus=eWebEditProSectionType_refreshStatus;
 this.currentPreference=eWebEditProSectionType_currentPreference;this.create=eWebEditProSectionType_create;
 }
eWebEditProAddInstanceType(eWebEditProSectionType);function eWebEditProSectionType_refreshStatus(parameters){
 var objPlatformInfo=new PlatformInfo();this.isSupported=((objPlatformInfo.isIE && objPlatformInfo.browserVersion >=4.0) || (objPlatformInfo.isNetscape && objPlatformInfo.browserVersion >=6.2)) && (parameters && !parameters.isPopup);
 this.isAutoInstallSupported=false;this.isInstalled=true;this.upgradeNeeded=false;
 }
function eWebEditProSectionType_currentPreference(width,height,parameters){return 425;
}
function eWebEditProSectionType_create(name,width,height,parameters){var objInstance=new eWebEditProSection(name,width,height,parameters);
 return objInstance;}
function eWebEditProSection(name,width,height,parameters){if (typeof parameters !="object" || (null==parameters)){
 parameters=eWebEditPro.parameters;} this.type="section";extendObject(this,eWebEditProInstance,name,width,height,parameters);
 this.readOnly=(true==parameters.readOnly);this.buttonName="";this.objPopup=null;
 var objPlatformInfo=new PlatformInfo();this.isIE=objPlatformInfo.isIE;var isDHTMLSupported=((objPlatformInfo.isIE && objPlatformInfo.browserVersion >=4.0) || (objPlatformInfo.isNetscape && objPlatformInfo.browserVersion >=6.2));
 if (isDHTMLSupported){this.setDHTML=eWebEditProSection_setDHTML;} this.showChanged=eWebEditProSection_showChanged;
 this.changedBorderWidth=parameters.getOptionalParameter("changedBorderWidth","thick");
 this.changedBorderStyle=parameters.getOptionalParameter("changedBorderStyle","dashed");
 this.changedBorderColor=parameters.getOptionalParameter("changedBorderColor","#ff9900");
 this.getContent=eWebEditProSection_getContent;this.createButtonHTML=eWebEditProSection_createButtonHTML;
 this.createTableHTML=eWebEditProSection_createTableHTML;this.createDivHTML=eWebEditProSection_createDivHTML;
 this.createHTML=eWebEditProSection_createHTML;this.isReady=eWebEditProSection_isReady;
 this.writeValue=eWebEditProSection_writeValue;this.readValue=eWebEditProSection_readValue;
 this.estimateContentSize=eWebEditProSection_estimateContentSize;this.overrideMethod("save",eWebEditProSection_save);
 this.clear=eWebEditProSection_clear;this.focus=eWebEditProSection_focus;this.getReadOnly=eWebEditProSection_getReadOnly;
 this.setReadOnly=eWebEditProSection_setReadOnly;this.isChanged=eWebEditProSection_isChanged;
 this.linkedWindow=eWebEditProSection_linkedWindow;this.linkedInstance=eWebEditProSection_linkedInstance;
 this.linkedPopup=eWebEditProSection_linkedPopup;this.refresh=eWebEditProSection_refresh;
 this.insertMediaFile=new Function();this.insertFileLink=new Function();var strHTML=this.createHTML(parameters);
 this.writeHTML(strHTML,parameters);}
 function eWebEditProSection_getContent(){var strContent="";var objElem=this.linkedElement();
 if (objElem){strContent=objElem.value;} if (0==strContent.length){strContent="<p>&nbsp;</p>";
 } return strContent;}
function eWebEditProSection_createButtonHTML(parameters){var strHTML="";if (!this.readOnly){
 this.buttonName=this.name +"Button";var saveWriteDisabled=parameters.writeDisabled;
 parameters.writeDisabled=true;var saveValidButton=eWebEditPro.validButton;eWebEditPro.validButton=function (){
 return true;};var objPopup=eWebEditPro.createButton(this.buttonName,this.name,parameters);
 parameters.writeDisabled=saveWriteDisabled;eWebEditPro.validButton=saveValidButton;
 this.objPopup=objPopup;if (objPopup){eWebEditPro.addEventHandler("onedit",'eWebEditPro.instances["' + this.name +'"].refresh(eWebEditPro.event.elementName)');
 strHTML=objPopup.html;} else{this.readOnly=true;} } return strHTML;}
function eWebEditProSection_createTableHTML(parameters){var objPlatformInfo=new PlatformInfo();
 var bUseLayer=(objPlatformInfo.isNetscape && objPlatformInfo.browserVersion < 5.0);
 var strButtonHTML=this.createButtonHTML(parameters);var strWidth="";if (this.width && this.width !="0"){
 strWidth +=' width="' + this.width +'"';} var strCommonDefault="";var strOuterDefault=strCommonDefault;
 var strInnerDefault=strCommonDefault;var strOuterStyle='style="';strOuterStyle +=parameters.getOptionalParameter("sectionOuterStyle",strOuterDefault);
 strOuterStyle +='"';var strInnerStyle='style="';strInnerStyle +=parameters.getOptionalParameter("sectionInnerStyle",strInnerDefault);
 strInnerStyle +='"';var strHTML='\n<table ' + strWidth +' cellspacing="0" cellpadding="2" border="1" ' + strOuterStyle +'>\n';
 if (strButtonHTML){strHTML +='\n<tr><td ' + strWidth +' bgcolor="#cccccc">\n';strHTML +=strButtonHTML;
 strHTML +='</td></tr>\n';} strHTML +='<tr><td ' + strWidth +' ' + strInnerStyle;
 if (bUseLayer){strHTML +='>\n<ilayer id="' + this.id +'" ' + strWidth +'>\n' + this.getContent() +'\n</ilayer>\n';
 } else{strHTML +=' id="' + this.id +'">\n' + this.getContent();} strHTML +='\n</td></tr></table>\n';
 return strHTML;}
function eWebEditProSection_createDivHTML(parameters){var strButtonHTML=this.createButtonHTML(parameters);
 var strDimStyle="";if (this.width && this.width !="0"){strDimStyle +='width:' + this.width +';';
 } if (this.height && this.height !="0"){strDimStyle +='height:' + this.height +';';
 } var strCommonDefault="";if (strButtonHTML){strCommonDefault +="border-style:solid;border-color:green;";
 } var strOuterDefault=strCommonDefault;if (strButtonHTML){strOuterDefault +="border-width:2px;padding:1px;";
 } var strInnerDefault=strCommonDefault;if (strButtonHTML){strInnerDefault +="border-width:0;border-top-width:1px;padding:1px;margin:0;";
 } var strOuterStyle='style="';if (strButtonHTML){strOuterStyle +=strDimStyle;} strOuterStyle +=parameters.getOptionalParameter("sectionOuterStyle",strOuterDefault);
 strOuterStyle +='"';var strInnerStyle='style="';strInnerStyle +=parameters.getOptionalParameter("sectionInnerStyle",strInnerDefault);
 strInnerStyle +='"';var strHTML='';if (strButtonHTML){strHTML +='\n<div ' + strOuterStyle +'>\n';
 strHTML +=strButtonHTML;} strHTML +='\n<div id="' + this.id +'" ' + strInnerStyle +'>\n' + this.getContent() +'\n</div>\n';
 if (strButtonHTML){strHTML +='</div>\n';} return strHTML;}
function eWebEditProSection_createHTML(parameters){return this.createTableHTML(parameters);
 }
function eWebEditProSection_isReady(){var isReady=false;if (("undefined"==typeof this.objSectionDoc) || (null==this.objSectionDoc)){
 this.objSectionDoc=findDocument(this.editorDocument,this.editorWindow) } if (this.objSectionDoc){
 if (this.objSectionDoc.all){if (this.objSectionDoc.all[this.id]){isReady=true;} }
 else if (this.objSectionDoc.getElementById){if (this.objSectionDoc.getElementById(this.id)){
 isReady=true;} } else if (this.objSectionDoc.layers){if (this.objSectionDoc.layers[this.id]){
 isReady=true;} } } return isReady;}
function eWebEditProSection_getReadOnly(){return this.readOnly;}
function eWebEditProSection_setReadOnly(bValue){}
function eWebEditProSection_isChanged(){var objPopup=this.linkedPopup();return (objPopup && objPopup.isOpen());
 }
function eWebEditProSection_writeValue(strValue,fnNotify){if (this.isReady()){if (this.setDHTML){
 this.setDHTML(this.objSectionDoc,this.id,strValue);} } notifyObject(this,fnNotify);
 }
function eWebEditProSection_readValue(fnNotify){var vResult="";if (this.isReady()){
 var objElem=this.linkedElement();if (objElem){vResult=objElem.value;} } notifyObject(this,fnNotify,vResult);
}
function eWebEditProSection_estimateContentSize(editorEstimateContentSize){return 0;
 }
function eWebEditProSection_save(objValueDestination,objNotify,fnNotify){var objPopup=this.linkedPopup();
 if (objPopup && objPopup.isOpen()){setTimeout('eWebEditPro.instances["' + this.name +'"].linkedPopup().focus()',1);
 this.status=EWEP_STATUS_CANCELED;notifyObject(objNotify,fnNotify,false);return false;
 } else{return this.super_save(objValueDestination,objNotify,fnNotify);}}
function eWebEditProSection_clear(){if (this.objPopup){this.objPopup.close();} this.objSectionDoc=null;
 this.objPopup=null;this.status=EWEP_STATUS_NOTLOADED;}
function eWebEditProSection_focus(){if (this.isReady()){if (this.buttonName){var objElem=findElement(this.buttonName,this.editorWindow);
 if (objElem){if ("function"==typeof objElem.focus || (("object"==typeof objElem.focus) && ("undefined"==typeof objElem.focus.value))){
 objElem.focus();} } } }}
function eWebEditProSection_linkedWindow(){var objWin=null;var objPopup=this.linkedPopup();
 if (objPopup){objWin=objPopup.getWindow();} return objWin;}
function eWebEditProSection_linkedInstance(){var objInstance=null;if (!this.isIE){
 var objPopup=this.linkedPopup();if (objPopup){objInstance=objPopup.getInstance();
 } } return objInstance;}
function eWebEditProSection_linkedPopup(){return this.objPopup;}
function eWebEditProSection_refresh(strElementName){if ("undefined"==strElementName || this.name==strElementName){
 if (this.isReady()){var objElem=this.linkedElement();var objPopup=this.linkedPopup();
 if (objElem && objPopup && objPopup.changed){if (this.setDHTML){this.setDHTML(this.objSectionDoc,this.id,objElem.value);
 } else{this.showChanged(this.objSectionDoc,this.id);} } } }}
function eWebEditProSection_setDHTML(objDoc,strElementId,strHTML){if (!objDoc || !strElementId){
 return;} strHTML=strHTML +"";if (0==strHTML.length){strHTML='<p>&nbsp;</p>';} if (objDoc.all){
 var objElem=objDoc.all[strElementId];if (objElem){objElem.innerHTML=strHTML;} } else if (objDoc.getElementById){
 var objElem=objDoc.getElementById(strElementId);if (objElem){objElem.innerHTML=strHTML;
 } } else if (objDoc.layers){var objLayer=objDoc.layers[strElementId];if (objLayer){
 objDoc=objLayer.document;objDoc.open('text/html',"replace");objDoc.write(strHTML);
 objDoc.close();var x=window.innerWidth;var y=window.innerHeight;window.resizeBy(1,1);
 if (x==window.innerWidth && y==window.innerHeight){if (eWebEditProMessages.maximizedWindow){
 alert(eWebEditProMessages.maximizedWindow);} else{alert("The browser window must not be maximized to update the page.");
 } } window.resizeBy(-1,-1);} } else{alert("Unable to dynamically write HTML to element " + strElementId);
 }}
function eWebEditProSection_showChanged(objDoc,strElementId){if (!objDoc || !strElementId){
 return;} var objElem=null;if (objDoc.all){objElem=objDoc.all[strElementId];} else if (objDoc.getElementById){
 objElem=objDoc.getElementById(strElementId);} if (objElem){objElem.style.borderWidth=this.changedBorderWidth;
 objElem.style.borderStyle=this.changedBorderStyle;objElem.style.borderColor=this.changedBorderColor;
 }}
