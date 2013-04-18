/* Copyright Ektron, Inc. 03/23/09 */var g_design_srcPath="";var ELEMENT_NODE=1;var TEXT_NODE=3;var CDATA_SECTION_NODE=4;
var g_design_ektPlatformInfo=null;function design_isSafari(){if (null==g_design_ektPlatformInfo &&"function"==typeof PlatformInfo){
 g_design_ektPlatformInfo=new PlatformInfo;} if (g_design_ektPlatformInfo){return (g_design_ektPlatformInfo.isSafari);
 } else{return (false);}}
function design_onsubmitForm(form){var oElem=design_validateHtmlForm(form);if (oElem && oElem.title !=""){
 alert(oElem.title);if ('function'==typeof oElem.scrollIntoView ||'object'==typeof oElem.scrollIntoView){
 oElem.scrollIntoView();} if (design_canElementReceiveFocus(oElem)){oElem.focus();
 } return false;} return true;}
function design_validateHtmlForm(form){if (form && form._form) form=form._form;if (null==g_design_ektPlatformInfo &&"function"==typeof PlatformInfo){
 g_design_ektPlatformInfo=new PlatformInfo;} if (g_design_ektPlatformInfo){if (g_design_ektPlatformInfo.isNetscape && g_design_ektPlatformInfo.browserVersion < 6.2) return null;
 if (g_design_ektPlatformInfo.isIE && g_design_ektPlatformInfo.browserVersion < 5) return null;
 } var oForm;switch (typeof form){case"string": case"number": oForm=document.forms[form];
 break;case"object": oForm=form;break;default: oForm=document.forms[0];break;} if (!oForm) return null;
 return design_prevalidateElement(oForm,null);}
function design_prevalidateElement(oElem,oFirstInvalidElem){if (!oElem) return oFirstInvalidElem;
 if ("undefined"==typeof oElem.getAttribute) return oFirstInvalidElem;if ("design_prototype"==oElem.className) return oFirstInvalidElem;
 if ("object"==typeof oElem.currentStyle && oElem.currentStyle !=null){if ("none"==oElem.currentStyle.display) return oFirstInvalidElem;
 if ("hidden"==oElem.currentStyle.visibility) return oFirstInvalidElem;} var validation=oElem.getAttribute("ektdesignns_validation");
 if (validation && validation !="none"){oElem.removeAttribute("ektdesignns_isvalid");
 design_validate_result=true;if ("function"==typeof oElem.onblur){oElem.onblur();
 } else{var sFn=oElem.getAttribute("onblur");if (sFn){try{oElem.fnonblur=new Function(sFn);
 oElem.fnonblur();} catch (e){sFn=sFn.replace(/([\(\,]\s*)this(\s*[\,\)])/,'$1oElem$2');
 var fn=new Function(sFn);eval(sFn);} } } if (null==oFirstInvalidElem && false==design_validate_result){
 oFirstInvalidElem=oElem;} } if (typeof oElem.childNodes !="undefined"){for (var i=0;
 i < oElem.childNodes.length;i++){if (ELEMENT_NODE==oElem.nodeType){oFirstInvalidElem=design_prevalidateElement(oElem.childNodes.item(i),oFirstInvalidElem);
 } } } return oFirstInvalidElem;}
var g_oElemContainerForAttributes=null;function design_getProtectedAttribute(oElem,name){
 var retValue;if (oElem){var ektAttr=oElem.getAttribute("ctagattrs");if ("string"==typeof ektAttr){
 if (null==g_oElemContainerForAttributes){g_oElemContainerForAttributes=document.createElement("span");
 } var strAttrs=ektAttr.replace(/\@zzquote\;/g,'"');strAttrs=strAttrs.replace(/\@zzsquo\;/g,"'");
 strAttrs=strAttrs.replace(/\@zzamp\;/g,"&");strAttrs=strAttrs.replace(/\@zzlt\;/g,"<");
 strAttrs=strAttrs.replace(/\@zzgt\;/g,">");g_oElemContainerForAttributes.innerHTML="<span " + strAttrs +"> </span>";
 retValue=g_oElemContainerForAttributes.firstChild.getAttribute(name);} } return retValue;
}
function design_getAttribute(oElem,name){var retValue;if (oElem){switch (name){case"selected": if (oElem.selected){
 retValue="selected";} break;case"checked": if (oElem.checked){retValue="checked";
 } break;default: retValue=oElem.getAttribute(name);if ("undefined"==typeof retValue || null==retValue){
 retValue=design_getProtectedAttribute(oElem,name);} } } return retValue;}
function design_getValue(oElem){if (!oElem) return;var bSupportInnerHTML=(typeof oElem.innerHTML !="undefined");
 if (typeof oElem.value !="undefined"){if ("INPUT"==oElem.tagName && ("checkbox"==oElem.type ||"radio"==oElem.type)){
 var strValue=oElem.value +"";if (strValue.length > 0 && strValue !="true" && strValue !="on"){
 if (oElem.checked){return strValue;} else{return"";} } else{if (oElem.checked){return true;
 } else{return false;} } } else{return oElem.value +"";} } else if (typeof oElem.getAttribute !="undefined" && oElem.getAttribute("datavalue") !=null){
 return oElem.getAttribute("datavalue");} else if (typeof oElem.getAttribute !="undefined" && oElem.getAttribute("value") !=null){
 return oElem.getAttribute("value");} else if (bSupportInnerHTML &&"content-req"==design_getAttribute(oElem,"ektdesignns_validation")){
 return oElem.innerHTML;} else if (bSupportInnerHTML &&"mixed"==design_getAttribute(oElem,"ektdesignns_datatype")){
 return oElem.innerText;} else if (typeof oElem.innerText !="undefined"){return oElem.innerText;
 } else if (bSupportInnerHTML){return oElem.innerHTML.replace(/\<[^>]*\>/g,"");} else{
 return;}}
function design_setValue(oElem,value){if (!oElem) return;if (typeof oElem.value !="undefined"){
 if ("INPUT"==oElem.tagName && ("checkbox"==oElem.type ||"radio"==oElem.type)){if ("true"==value || true==value ||"on"==value){
 oElem.checked=true;} else if ("false"==value || false==value){oElem.checked=false;
 } else{oElem.value=value;} } else{oElem.value=value;} } else if (typeof oElem.getAttribute !="undefined" && oElem.getAttribute("value") !=null){
 oElem.value=value;} else if (typeof oElem.innerHTML !="undefined" &&"mixed"==design_getAttribute(oElem,"ektdesignns_datatype")){
 oElem.innerHTML=value;} else if (typeof oElem.innerText !="undefined"){oElem.innerText=value;
 }}
function design_evaluate(expression,value){var obj=new Object();obj.text=value +"";
 obj.fnDesignEvaluateExpression=new Function("return " + expression);return obj.fnDesignEvaluateExpression();
}
function design_normalize_re(re,oElem){if (typeof g_design_prevalidateFormReentry=="undefined" || g_design_prevalidateFormReentry !=true){
 var value=design_getValue(oElem);if ("undefined"==typeof value) return;if ("undefined" !=typeof RegExp.lastIndex){
 RegExp.lastIndex=0;} re.lastIndex=0;var ary=re.exec(value);value=(null==ary ?"" : ary[0]);
 design_normalize_complete(oElem,value);}}
function design_validate_re(re,oElem,invalidmsg){var value=design_getValue(oElem);
 if ("undefined"==typeof value) return;if ("undefined" !=typeof RegExp.lastIndex){
 RegExp.lastIndex=0;} re.lastIndex=0;var result=re.test(value);design_validate_complete(oElem,result,invalidmsg);
 return result;}
function design_normalize_js(expression,oElem){if (typeof g_design_prevalidateFormReentry=="undefined" || g_design_prevalidateFormReentry !=true){
 var value=design_getValue(oElem);if ("undefined"==typeof value) return;var value=design_evaluate(expression,value);
 design_normalize_complete(oElem,value);}}
function design_validate_js(expression,oElem,invalidmsg){var value=design_getValue(oElem);
 if ("undefined"==typeof value) return;var result=design_evaluate(expression,value);
 design_validate_complete(oElem,result,invalidmsg);return result;}
function design_normalize_complete(oElem,value){design_setValue(oElem,value);}
var design_validate_result=true;function design_validate_complete(oElem,result,invalidmsg){
 design_validate_result=result;if (!oElem) return result;if (invalidmsg &&"string"==typeof oElem.title){
 var p=oElem.title.indexOf(invalidmsg);if (p >=0){if (p > 0 &&"\n"==oElem.title.charAt(p-1)){
 p -=1;} oElem.title=oElem.title.substring(0,p);} p=oElem.title.length - 1;if (p >=0 &&"\n"==oElem.title.charAt(p)){
 while (p >=0 &&"\n"==oElem.title.charAt(p)){p--;} oElem.title=oElem.title.substring(0,p);
 } } if (!result){if (invalidmsg && ("string"==typeof oElem.title)){if (-1==oElem.title.indexOf(invalidmsg)){
 if (oElem.title.length > 0){oElem.title +=" \n";} oElem.title +=invalidmsg;} } }
 if ("function"==typeof customValidationStyle){customValidationStyle(oElem,result);
 } else{design_validationStyle(oElem,result);}}
function design_validationStyle(oElem,isValid){var parent=null;var elTypeName=oElem.tagName;
 var specialCaseBorder=(design_isSafari() && ("INPUT"==elTypeName)) || ("SELECT"==elTypeName);
 if ("object"==typeof oElem){parent=oElem.parentNode;if (("object"==typeof oElem.style) && ("object"==typeof parent)){
 if (isValid){if (specialCaseBorder){if (("SPAN"==parent.tagName) && ("design_validation_failed"==parent.className)){
 parent.className="design_validation_passed";} } else{oElem.style.borderTopStyle="";
 oElem.style.borderRightStyle="";oElem.style.borderBottomStyle="";oElem.style.borderLeftStyle="";
 oElem.style.borderTopColor="";oElem.style.borderRightColor="";oElem.style.borderBottomColor="";
 oElem.style.borderLeftColor="";oElem.style.borderTopWidth="";oElem.style.borderRightWidth="";
 oElem.style.borderBottomWidth="";oElem.style.borderLeftWidth="";oElem.style.margin="2px";
 } } else{if (("undefined"==typeof g_design_designMode) || (g_design_designMode !=true)){
 if (specialCaseBorder){if ((parent.tagName !="SPAN") || ((parent.className !="design_validation_failed") && (parent.className !="design_validation_passed"))){
 var wrapper=document.createElement("span");wrapper=parent.insertBefore(wrapper,oElem);
 oElem=parent.removeChild(oElem);oElem=wrapper.appendChild(oElem);parent=wrapper;
 } parent.className="design_validation_failed";} else{oElem.style.borderStyle="dashed";
 oElem.style.borderColor="red";oElem.style.borderWidth="2px";oElem.style.margin="0";
 } } } } }}
function design_validate_select(minIndex,oElem,invalidmsg){if (!oElem) return;if ("undefined"==typeof oElem.selectedIndex){
 return;} var result=(oElem.selectedIndex >=minIndex);design_validate_complete(oElem,result,invalidmsg);
 return result;}
function design_validate_choice(minSelected,maxSelected,oElem,invalidmsg){if (!oElem) return;
 if ("undefined"==typeof oElem.getElementsByTagName) return;var num_checked=0;var oCurrElem;
 var bUseChecked;var aryElements=null;var validation=oElem.getAttribute("ektdesignns_validation");
 if ("choice-req"==validation){aryElements=oElem.getElementsByTagName("input");bUseChecked=true;
 } else if ("select-req"==validation){aryElements=oElem.getElementsByTagName("option");
 bUseChecked=false;} if (aryElements){for (var i=0;i < aryElements.length;i++){oCurrElem=aryElements[i];
 if (bUseChecked){if (oCurrElem.checked){num_checked++;} } else{if (oCurrElem.selected){
 num_checked++;} } } } var result=(minSelected <=num_checked && (maxSelected <=0 || num_checked <=maxSelected));
 design_validate_complete(oElem,result,invalidmsg);return result;}
function design_normalize_isbn(value){value=value +"";value=value.replace(/[\s\-]/g,"").toUpperCase();
 return value;}
function design_validate_isbn(value){var result=design_validate_isbn10(value) || design_validate_isbn13(value);
 return result;}
function design_validate_isbn10(value){var result=true;value=value +"";var re=new RegExp("^[0-9]{9}[0-9X]$");
 if (!re.test(value)) return false;var check=0;var weight=10;for (var i=0;i < value.length;
 i++){var c=value.charCodeAt(i);if (88==c && 1==weight){check +=10;weight--;} else if (48 <=c && c <=57){
 check +=(c - 48) * weight--;} } result=(0==weight && 0==(check % 11));return result;
}
function design_validate_isbn13(value){value=value +"";var re=new RegExp("^[0-9]{13}$");
 if (!re.test(value)) return false;var check=0;var n=13;var weight=1;for (var i=0;
 i < value.length;i++){var c=value.charCodeAt(i);if (48 <=c && c <=57){check +=(c - 48) * weight;
 weight=(1==weight ? 3 : 1);n--;} } return (0==n && 0==(check % 10));}
function design_normalize_issn(value){value=value +"";value=value.replace(/[\s\-]/g,"").toUpperCase();
 return value;}
function design_validate_issn(value){value=value +"";var re=new RegExp("^[0-9]{7}[0-9X]$");
 if (!re.test(value)) return false;var check=0;var weight=8;for (var i=0;i < value.length;
 i++){var c=value.charCodeAt(i);if (88==c && 1==weight){check +=10;weight--;} else if (48 <=c && c <=57){
 check +=(c - 48) * weight--;} } return (0==weight && 0==(check % 11));}
function design_current_date(){var oCurrentDate=new Date();var mm=(oCurrentDate.getMonth() + 1);
 if (mm <=9) mm="0" + mm;var dd=oCurrentDate.getDate();if (dd <=9) dd="0" + dd;return (oCurrentDate.getFullYear() +"-" + mm +"-" + dd);
}
function design_validate_future_date(date){date=date +"";if (10==date.length){return (date >=design_current_date());
 } return false;}
function design_canElementReceiveFocus(oElem){if (!oElem) return false;var strType=oElem.type +"";
 if ("hidden"==strType) return false;if ("object"==typeof oElem.currentStyle){if ("none"==oElem.currentStyle.display) return false;
 if ("hidden"==oElem.currentStyle.visibility) return false;} var strDisabled=oElem.disabled +"";
 if ("true"==strDisabled) return false;if (oElem.isDisabled) return false;var strIsTextEdit=oElem.isTextEdit +"";
 if ("false"==strIsTextEdit) return false;var strFocusMethod=typeof oElem.focus;if ("function" !=strFocusMethod &&"object" !=strFocusMethod) return false;
 return true;}
function design_HTMLEncode(s){var strHTML=s +"";strHTML=strHTML.replace(/\&/g,"&amp;");
 strHTML=strHTML.replace(/\</g,"&lt;");strHTML=strHTML.replace(/\>/g,"&gt;");strHTML=strHTML.replace(/\"/g,"&quot;");
 return strHTML;}
function design_serializeHTMLAttribute(oElem,name){if (!oElem) return"";try{var attr="";
 if ("class"==name){attr=oElem.className;} else{attr=design_getAttribute(oElem,name);
 } if ("string"==typeof attr && attr.length > 0){return" " + name +"=\"" + design_HTMLEncode(attr) +"\"";
 } else if ("boolean"==typeof attr && true==attr){return" " + name +"=\"" + name +"\"";
 } else{return"";} } catch (e){return"";}}
function design_serializeHTMLElement(oElem,content){if (!oElem) return"";var tagName=oElem.tagName.toLowerCase();
 var sAttrs="";var attrNames=["ektdesignns_bind","ektdesignns_nodetype","ektdesignns_content","class","type","value","selected","checked"];
 for (var i=0;i < attrNames.length;i++){sAttrs +=design_serializeHTMLAttribute(oElem,attrNames[i]);
 } if ("undefined"==typeof content){content="";for (var i=0;i < oElem.childNodes.length;
 i++){var oChild=oElem.childNodes[i];switch (oChild.nodeType){case ELEMENT_NODE: content +=design_serializeHTMLElement(oChild);
 break;case TEXT_NODE: content +=oChild.nodeValue;break;default: break;} } } return design_serializeElement(tagName,content,sAttrs);
}
function design_serializeElement(tagName,content,attributes){if ("undefined"==typeof attributes) attributes="";
 if ("undefined"==typeof content || ("string"==typeof content && 0==content.length) || (null==content)){
 return"<" + tagName + attributes +" />\n";} else{return"<" + tagName + attributes +">" + content +"</" + tagName +">\n";
 }}
function design_xml_loadXML(xml){try{if (typeof xml !="string") return null;if (xml.length <=2) return null;
 var xmlDoc=Sarissa.getDomDocument();if ("string"==typeof xmlDoc || null==xmlDoc) return"Unable to create XML DOM Document";
 xmlDoc.async=false;if (xml.indexOf("<") >=0){var objParser=new DOMParser();xmlDoc=objParser.parseFromString(xml,"text/xml");
 if (Sarissa.getParseErrorText(xmlDoc) !=Sarissa.PARSED_OK){xml="<root>" + xml +"</root>";
 xmlDoc=objParser.parseFromString(xml,"text/xml");} } else{var url=xml;url=url.replace(/.*(\[|%5B)srcpath(\]|%5D)\/?/i,g_design_srcPath);
 url=url.replace(/.*(\[|%5B)eWebEditProPath(\]|%5D)\/?/i,g_design_srcPath);xmlDoc.load(url);
 } var strErrMsg=Sarissa.getParseErrorText(xmlDoc);if (strErrMsg !=Sarissa.PARSED_OK){
 return strErrMsg;} return xmlDoc;} catch (e){return e.message;}}
function design_xml_loadXSLT(xslt){try{if (typeof xslt !="string") return null;if (xslt.length <=2) return null;
 var xslDoc=Sarissa.getXsltDocument();if ("string"==typeof xslDoc || null==xslDoc) return"Unable to create XSLT DOM Document";
 xslDoc.async=false;if (xslt.indexOf("<") >=0){if (typeof xslDoc.loadXML !="undefined"){
 xslDoc.loadXML(xslt);} else{var objParser=new DOMParser();xslDoc=objParser.parseFromString(xslt,"text/xml");
 } } else{var url=xslt;url=url.replace(/.*(\[|%5B)srcpath(\]|%5D)\/?/i,g_design_srcPath);
 url=url.replace(/.*(\[|%5B)eWebEditProPath(\]|%5D)\/?/i,g_design_srcPath);xslDoc.load(url);
 } var strErrMsg=Sarissa.getParseErrorText(xslDoc);if (strErrMsg !=Sarissa.PARSED_OK){
 return strErrMsg;} return xslDoc;} catch (e){return e.message;}}
function design_transformToDocument(xml,xslt){try{var xmlDoc=design_xml_loadXML(xml);
 if ("string"==typeof xmlDoc) return xmlDoc;if (null==xmlDoc) return"Unable to load XML document";
 var xsltDoc=design_xml_loadXSLT(xslt);if ("string"==typeof xsltDoc) return xsltDoc;
 if (null==xsltDoc) return"Unable to load XSLT document";var processor=new XSLTProcessor();
 processor.importStylesheet(xsltDoc);var newDoc=processor.transformToDocument(xmlDoc);
 return newDoc;} catch (e){return e.message;}}
function design_transform(xml,xslt){try{var xmlDoc=design_xml_loadXML(xml);if ("string"==typeof xmlDoc) return xmlDoc;
 if (null==xmlDoc) return"Unable to load XML document";var xsltDoc=design_xml_loadXSLT(xslt);
 if ("string"==typeof xsltDoc) return xsltDoc;if (null==xsltDoc) return"Unable to load XSLT document";
 var processor=new XSLTProcessor();processor.importStylesheet(xsltDoc);var ownerDoc=Sarissa.getDomDocument();
 var newDoc=processor.transformToFragment(xmlDoc,ownerDoc);if ("string"==typeof newDoc) return newDoc;
 var result=(new XMLSerializer()).serializeToString(newDoc);result=result.replace(/<transformiix:result[^>]*>/,"").replace("</transformiix:result>","");
 result=result.replace(/xslout:/g,"xsl:");result=result.replace(/<\?[^\?]*\?>/,"");result=result.replace(/xpathLiteralString(.*?)gnirtSlaretiLhtapx/g,function(s,p1){
 return xpathLiteralString(p1);} );return result;} catch (e){return e.message;}}
function xpathLiteralString(s){if (s.indexOf("'") >=0){return"concat('" + s.replace(/\'/g,"',&quot;'&quot;,'") +"')";
 } else{return"'" + s +"'";}}
function design_replaceDataLists(){if (!document || !document.body){setTimeout('design_replaceDataLists()',200);
 return;} var aryDatalistCache=new Array();var aryTags=new Array();aryTags[0]=document.body.getElementsByTagName("select");
 aryTags[1]=document.body.getElementsByTagName("ektdesignns_choices");aryTags[2]=document.body.getElementsByTagName("ektdesignns_checklist");
 for (var iTag=0;iTag < aryTags.length;iTag++){var aryElems=aryTags[iTag];for (var iElem=0;
 iElem < aryElems.length;iElem++){var oElem=aryElems[iElem];var datasrc=design_getAttribute(oElem,"ektdesignns_datasrc");
 if ("string"==typeof datasrc && datasrc.length > 0){var datalist=design_getAttribute(oElem,"ektdesignns_datalist");
 if ("string"==typeof datalist && datalist.length > 0){if ("undefined"==typeof aryDatalistCache[datalist]){
 var strSelect=design_getAttribute(oElem,"ektdesignns_dataselect");var strCaptionXPath=design_getAttribute(oElem,"ektdesignns_captionxpath");
 var strValueXPath=design_getAttribute(oElem,"ektdesignns_valuexpath");var strNamespaces=design_getAttribute(oElem,"ektdesignns_datanamespaces");
 aryDatalistCache[datalist]=design_getDataList(oElem.tagName,datasrc,strSelect,strCaptionXPath,strValueXPath,strNamespaces);
 } if (aryDatalistCache[datalist].length > 0){if ("SELECT"==oElem.tagName){var nNumOrigItemsToKeep=0;
 var validation=design_getAttribute(oElem,"ektdesignns_validation");if ("select-req"==validation){
 nNumOrigItemsToKeep=1;} var strOrigDataList="";for (var iOption=0;iOption < oElem.options.length;
 iOption++){var oOption=oElem.options[iOption];strOrigDataList +=design_serializeHTMLElement(oOption,design_HTMLEncode(oOption.text));
 } var xmlDoc=design_transformDataList(strOrigDataList,aryDatalistCache[datalist],nNumOrigItemsToKeep);
 if (typeof xmlDoc !="string"){var newOptions=xmlDoc.getElementsByTagName("option");
 var numOptions=(newOptions !=null ? newOptions.length : 0);if (oElem.options.length > numOptions){
 oElem.options.length=numOptions;} if (oElem.multiple && oElem.size < 2 && numOptions > 12){
 oElem.size=12;} for (var iOption=0;iOption < numOptions;iOption++){var newOption=newOptions[iOption];
 var attrs=newOption.attributes;var attr;var text=(newOption.firstChild ? newOption.firstChild.nodeValue :"");
 attr=attrs.getNamedItem("value");var value=(attr ? attr.nodeValue :"");attr=attrs.getNamedItem("selected");
 var bSelected=("selected"==(attr ? attr.nodeValue :""));oElem.options[iOption]=new Option(text,value,bSelected,bSelected);
 } } else{alert(xmlDoc);} } else{var oOrigListElem=oElem.nextSibling;while (oOrigListElem.tagName !="OL") oOrigListElem=oOrigListElem.nextSibling;
 var strOrigDataList=design_serializeHTMLElement(oOrigListElem);var strHtml=design_transformChoiceDataList(strOrigDataList,aryDatalistCache[datalist]);
 strHtml=strHtml.replace(/<ol[^>]*>/,"").replace("</ol>","");oOrigListElem.innerHTML=strHtml;
 } } } } } } }
setTimeout('design_replaceDataLists()',1);function design_transformChoiceDataList(strOrigDataList,strNewDataList){
 var strXSLT="";strXSLT="";strXSLT +="<?xml version='1.0'?>\n";strXSLT +="<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">\n";
 strXSLT +="<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>\n";
 strXSLT +="<xsl:template match=\"ol\">\n";strXSLT +="<xslout:variable name=\"nameID\" select=\"'{li/input/@name}'\"/>\n";
 strXSLT +="<xslout:variable name=\"inputType\" select=\"'{li/input/@type}'\"/>\n";
 strXSLT +="</xsl:template>\n";strXSLT +="<xsl:template match=\"text()\"/>\n";strXSLT +="</xsl:stylesheet>\n";
 var strVariables=design_transform(strOrigDataList,strXSLT);strXSLT="";strXSLT +="<?xml version='1.0'?>\n";
 strXSLT +="<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">\n";
 strXSLT +="<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>\n";
 strXSLT +="<xsl:template match=\"/\">\n";strXSLT +="	<xsl:apply-templates/>\n";strXSLT +="</xsl:template>\n";
 strXSLT +="<xsl:template match=\"ol/li/input[@checked]\">\n";strXSLT +="	<xslout:if test=\"not(option[@value=xpathLiteralString{@value}gnirtSlaretiLhtapx])\">\n";
 strXSLT +="    	<xsl:copy-of select=\"..\"/>\n";strXSLT +="	</xslout:if>\n";strXSLT +="</xsl:template>\n";
 strXSLT +="<xsl:template match=\"text()\"/>\n";strXSLT +="</xsl:stylesheet>\n";var strOldSelectedSnip=design_transform(strOrigDataList,strXSLT);
 strXSLT="";strXSLT +="<?xml version='1.0'?>\n";strXSLT +="<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">\n";
 strXSLT +="<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>\n";
 strXSLT +="<xsl:template match=\"/\">\n";strXSLT +="	<xsl:for-each select=\"ol/li/input[1]/@*[starts-with(name(),'ektdesignns_')]\">\n";
 strXSLT +="		<xslout:attribute name=\"{name()}\"><xsl:value-of select=\".\"/></xslout:attribute>\n";
 strXSLT +="	</xsl:for-each>\n";strXSLT +="	<xsl:apply-templates/>\n";strXSLT +="</xsl:template>\n";
 strXSLT +="<xsl:template match=\"ol/li/input[@checked]\">\n";strXSLT +="	<xslout:if test=\"@value=xpathLiteralString{@value}gnirtSlaretiLhtapx\">\n";
 strXSLT +="           <xslout:attribute name=\"checked\">checked</xslout:attribute>\n";
 strXSLT +="	</xslout:if>\n";strXSLT +="</xsl:template>\n";strXSLT +="<xsl:template match=\"text()\"/>\n";
 strXSLT +="</xsl:stylesheet>\n";var strNewSelectedSnip=design_transform(strOrigDataList,strXSLT);
 strXSLT="";strXSLT +="<?xml version='1.0'?>\n";strXSLT +="<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" exclude-result-prefixes=\"ektdesign\" xmlns:ektdesign=\"urn:ektdesign\">\n";
 strXSLT +="<xsl:output method=\"xml\" version=\"1.0\" indent=\"yes\" omit-xml-declaration=\"yes\"/>\n";
 strXSLT +=strVariables;strXSLT +="<xsl:template match=\"/\">\n";var strOlTag=strOrigDataList.match(/<ol[^>]*>/);
 strXSLT +="	" + strOlTag +"\n";strXSLT +="        <xsl:apply-templates/>\n";strXSLT +="    </ol>\n";
 strXSLT +="</xsl:template>\n";strXSLT +="<xsl:template match=\"select\">\n";strXSLT +=strOldSelectedSnip;
 strXSLT +="    <xsl:apply-templates select=\"node()\"/>\n";strXSLT +="</xsl:template>\n";
 strXSLT +="<xsl:template match=\"option\">\n";strXSLT +="    <xsl:variable name=\"modelID\" select=\"generate-id()\"/>\n";
 strXSLT +="    <xsl:variable name=\"displayOption\" select=\"text()\"/>\n";strXSLT +="    <xsl:variable name=\"valueOption\" select=\"@value\"/>\n";
 strXSLT +="     <li>\n";strXSLT +="    <input type=\"{$inputType}\" id=\"{$modelID}\" title=\"{$displayOption}\" value=\"{$valueOption}\" name=\"{$nameID}\">\n";
 strXSLT +=strNewSelectedSnip;strXSLT +="    </input>\n";strXSLT +="    <label for=\"{$modelID}\"><xsl:value-of select=\"$displayOption\"/></label>\n";
 strXSLT +="    </li>\n";strXSLT +="</xsl:template>\n";strXSLT +="<xsl:template match=\"*\">\n";
 strXSLT +="   <xsl:copy>\n";strXSLT +="       <xsl:copy-of select=\"@*\"/>\n";strXSLT +="       <xsl:apply-templates select=\"node()\"/>\n";
 strXSLT +="   </xsl:copy>\n";strXSLT +="</xsl:template>\n";strXSLT +="</xsl:stylesheet>\n";
 var strHtml=design_transform(strNewDataList,strXSLT);return strHtml;}
function design_transformDataList(strOrigDataList,strNewDataList){strOrigDataList="<select>" + strOrigDataList +"</select>";
 var strOldPredicate="@selected";if ("number"==typeof nNumOrigItemsToKeep){strOldPredicate +=" or position()&lt;=" + nNumOrigItemsToKeep;
 } var strXSLT="";strXSLT="";strXSLT +="<?xml version='1.0'?>\n";strXSLT +="<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">\n";
 strXSLT +="<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>\n";
 strXSLT +="<xsl:template match=\"/\">\n";strXSLT +="	<xsl:apply-templates/>\n";strXSLT +="</xsl:template>\n";
 strXSLT +="<xsl:template match=\"option[" + strOldPredicate +"]\">\n";strXSLT +="	<xslout:if test=\"not(option[@value=xpathLiteralString{@value}gnirtSlaretiLhtapx])\">\n";
 strXSLT +="    	<xsl:copy-of select=\".\"/>\n";strXSLT +="	</xslout:if>\n";strXSLT +="</xsl:template>\n";
 strXSLT +="<xsl:template match=\"text()\"/>\n";strXSLT +="</xsl:stylesheet>\n";var strOldSelectedSnip=design_transform(strOrigDataList,strXSLT);
 strXSLT="";strXSLT +="<?xml version='1.0'?>\n";strXSLT +="<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">\n";
 strXSLT +="<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>\n";
 strXSLT +="<xsl:template match=\"/\">\n";strXSLT +="	<xsl:apply-templates/>\n";strXSLT +="</xsl:template>\n";
 strXSLT +="<xsl:template match=\"option[@selected]\">\n";strXSLT +="	<xslout:if test=\"@value=xpathLiteralString{@value}gnirtSlaretiLhtapx\">\n";
 strXSLT +="           <xslout:attribute name=\"selected\">selected</xslout:attribute>\n";
 strXSLT +="	</xslout:if>\n";strXSLT +="</xsl:template>\n";strXSLT +="<xsl:template match=\"text()\"/>\n";
 strXSLT +="</xsl:stylesheet>\n";var strNewSelectedSnip=design_transform(strOrigDataList,strXSLT);
 strXSLT="";strXSLT +="<?xml version='1.0'?>\n";strXSLT +="<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" exclude-result-prefixes=\"ektdesign\" xmlns:ektdesign=\"urn:ektdesign\">\n";
 strXSLT +="<xsl:output method=\"xml\" version=\"1.0\" indent=\"yes\" omit-xml-declaration=\"yes\"/>\n";
 strXSLT +="<xsl:template match=\"select\">\n";strXSLT +="   <select>\n";strXSLT +="   <!-- copy selected values that are not in the new data list -->\n";
 strXSLT +=strOldSelectedSnip;strXSLT +="   <!-- process option tags -->\n";strXSLT +="   <xsl:apply-templates select=\"node()\"/>\n";
 strXSLT +="   </select>\n";strXSLT +="</xsl:template>\n";strXSLT +="<xsl:template match=\"option\">\n";
 strXSLT +="   <xsl:copy>\n";strXSLT +="       <!-- copy attributes except 'selected' -->\n";
 strXSLT +="       <xsl:copy-of select=\"@*[name() != 'selected']\"/>\n";strXSLT +="       <!-- check if selected in the old data list -->\n";
 strXSLT +=strNewSelectedSnip;strXSLT +="       <!-- copy the text -->\n";strXSLT +="       <xsl:copy-of select=\"node()\"/>\n";
 strXSLT +="       </xsl:copy>\n";strXSLT +="</xsl:template>\n";strXSLT +="<xsl:template match=\"*\"> \n";
 strXSLT +="   <xsl:copy>\n";strXSLT +="       <xsl:copy-of select=\"@*\"/>\n";strXSLT +="       <xsl:apply-templates select=\"node()\"/>\n";
 strXSLT +="   </xsl:copy>\n";strXSLT +="</xsl:template>\n";strXSLT +="</xsl:stylesheet>";
 var xmlDoc=design_transformToDocument(strNewDataList,strXSLT);return xmlDoc;}
function design_getDataList(strDDFieldTagName,strSource,strSelect,strCaptionXPath,strValueXPath,strNamespaces){
 var strPrefixes="";if ("undefined"==typeof strNamespaces || null==strNamespaces){
 strNamespaces="";} else{strPrefixes=design_extractPrefixesFromNamespaces(strNamespaces);
 if (strPrefixes.length > 0){strPrefixes=" exclude-result-prefixes=\"" + strPrefixes +"\"";
 } } var strXSLT="";strXSLT +="<?xml version=\"1.0\"?>\n";strXSLT +="<xsl:stylesheet version=\"1.0\" " + strPrefixes +" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" " + strNamespaces +">\n";
 strXSLT +="<xsl:output method=\"xml\" version=\"1.0\" omit-xml-declaration=\"yes\" indent=\"yes\"/>\n";
 strXSLT +="<xsl:template match=\"/\">\n";strXSLT +="  <select>\n";strXSLT +="  <xsl:for-each select=\"" + strSelect +"\">\n";
 strXSLT +="    <option>\n";strXSLT +="      <xsl:if test=\"" + strValueXPath +"\">\n";
 strXSLT +="        <xsl:attribute name=\"value\">\n";strXSLT +="          <xsl:value-of select=\"" + strValueXPath +"\"/>\n";
 strXSLT +="        </xsl:attribute>\n";strXSLT +="      </xsl:if>\n";strXSLT +="      <xsl:value-of select=\"" + strCaptionXPath +"\"/>\n";
 strXSLT +="    </option>\n";strXSLT +="  </xsl:for-each>\n";strXSLT +="  </select>\n";
 strXSLT +="</xsl:template>\n";strXSLT +="</xsl:stylesheet>";var strHtml=design_transform(strSource,strXSLT);
 strHtml=strHtml.replace(/^\s+/,"").replace(/\s+$/,"");if (strHtml.indexOf("<option") >=0 || 0==strHtml.length){
 return strHtml;} else{return"";}}
function design_extractPrefixesFromNamespaces(strNamespaces){var aryNSPrefix=new Array();
 var aryAllNS=strNamespaces.match(/xmlns:\w+=['"][^'"]*['"]/g);if (null==aryAllNS) return"";
 aryAllNS.sort();var prev="";for (var i=0;i < aryAllNS.length;i++){if (aryAllNS[i] !=prev){
 var aryNS=aryAllNS[i].match(/xmlns:(\w+)=['"]([^'"]*)['"]/);aryNSPrefix[aryNSPrefix.length]=aryNS[1];
 prev=aryAllNS[i];} } return aryNSPrefix.join(" ");}
function Sarissa(){}
;Sarissa.VERSION="0.9.7.8";Sarissa.PARSED_OK="Document contains no parsing errors";
Sarissa.PARSED_EMPTY="Document is empty";Sarissa.PARSED_UNKNOWN_ERROR="Not well-formed or other error";
Sarissa.IS_ENABLED_TRANSFORM_NODE=false;var _sarissa_iNsCounter=0;var _SARISSA_IEPREFIX4XSLPARAM="";
var _SARISSA_HAS_DOM_IMPLEMENTATION=document.implementation && true;var _SARISSA_HAS_DOM_CREATE_DOCUMENT=_SARISSA_HAS_DOM_IMPLEMENTATION && document.implementation.createDocument;
var _SARISSA_HAS_DOM_FEATURE=_SARISSA_HAS_DOM_IMPLEMENTATION && document.implementation.hasFeature;
var _SARISSA_IS_MOZ=_SARISSA_HAS_DOM_CREATE_DOCUMENT && _SARISSA_HAS_DOM_FEATURE;
var _SARISSA_IS_SAFARI=(navigator.userAgent && navigator.vendor && (navigator.userAgent.toLowerCase().indexOf("applewebkit") !=-1 || navigator.vendor.indexOf("Apple") !=-1));
var _SARISSA_IS_IE=document.all && window.ActiveXObject && navigator.userAgent.toLowerCase().indexOf("msie") > -1 && navigator.userAgent.toLowerCase().indexOf("opera")==-1;
if(!window.Node || !Node.ELEMENT_NODE){Node={ELEMENT_NODE: 1,ATTRIBUTE_NODE: 2,TEXT_NODE: 3,CDATA_SECTION_NODE: 4,ENTITY_REFERENCE_NODE: 5,ENTITY_NODE: 6,PROCESSING_INSTRUCTION_NODE: 7,COMMENT_NODE: 8,DOCUMENT_NODE: 9,DOCUMENT_TYPE_NODE: 10,DOCUMENT_FRAGMENT_NODE: 11,NOTATION_NODE: 12}
;}
;if(typeof XMLDocument=="undefined" && typeof Document !="undefined"){XMLDocument=Document;
 }
 if(_SARISSA_IS_IE){_SARISSA_IEPREFIX4XSLPARAM="xsl:";var _SARISSA_DOM_PROGID="";
 var _SARISSA_XMLHTTP_PROGID="";var _SARISSA_DOM_XMLWRITER="";Sarissa.pickRecentProgID=function (idList){
 var bFound=false;for(var i=0;i < idList.length && !bFound;i++){try{var oDoc=new ActiveXObject(idList[i]);
 o2Store=idList[i];bFound=true;}catch (objException){};};if (!bFound){alert("Failed to create XML parser: " + idList[0]);
 };idList=null;return o2Store;};_SARISSA_DOM_PROGID=null;_SARISSA_THREADEDDOM_PROGID=null;
 _SARISSA_XSLTEMPLATE_PROGID=null;_SARISSA_XMLHTTP_PROGID=null;if(!window.XMLHttpRequest){
 XMLHttpRequest=function(){if(!_SARISSA_XMLHTTP_PROGID){_SARISSA_XMLHTTP_PROGID=Sarissa.pickRecentProgID(["MSXML2.XMLHTTP.6.0","MSXML2.XMLHTTP.4.0","MSXML2.XMLHTTP.5.0","MSXML2.XMLHTTP.3.0"]);
 };return new ActiveXObject(_SARISSA_XMLHTTP_PROGID);};};Sarissa.getDomDocument=function(sUri,sName){
 if(!_SARISSA_DOM_PROGID){_SARISSA_DOM_PROGID=Sarissa.pickRecentProgID(["MSXML2.DOMDocument.6.0","MSXML2.DOMDocument.4.0","MSXML2.DOMDocument.5.0","MSXML2.DOMDocument.3.0"]);
 };var oDoc=new ActiveXObject(_SARISSA_DOM_PROGID);oDoc.resolveExternals=true;if (sName){
 var prefix="";if(sUri){if(sName.indexOf(":") > 1){prefix=sName.substring(0,sName.indexOf(":"));
 sName=sName.substring(sName.indexOf(":")+1);}else{prefix="a" + (_sarissa_iNsCounter++);
 };};if(sUri){oDoc.loadXML('<' + prefix+':'+sName +" xmlns:" + prefix +"=\"" + sUri +"\"" +" />");
 } else{oDoc.loadXML('<' + sName +" />");};};return oDoc;};Sarissa.getXsltDocument=function(sUri,sName){
 if(!_SARISSA_THREADEDDOM_PROGID){_SARISSA_THREADEDDOM_PROGID=Sarissa.pickRecentProgID(["MSXML2.FreeThreadedDOMDocument.6.0","MSXML2.FreeThreadedDOMDocument.4.0","MSXML2.FreeThreadedDOMDocument.5.0","MSXML2.FreeThreadedDOMDocument.3.0"]);
 };var oDoc=new ActiveXObject(_SARISSA_THREADEDDOM_PROGID);Sarissa.setXpathNamespaces(oDoc,"xmlns:xsl='http://www.w3.org/1999/XSL/Transform'");
 oDoc.resolveExternals=true;if ("MSXML2.FreeThreadedDOMDocument.6.0"==_SARISSA_THREADEDDOM_PROGID){
 oDoc.setProperty("AllowDocumentFunction",true);oDoc.setProperty("AllowXsltScript",true);
 oDoc.setProperty("ProhibitDTD",false);} if (sName){var prefix="";if(sUri){if(sName.indexOf(":") > 1){
 prefix=sName.substring(0,sName.indexOf(":"));sName=sName.substring(sName.indexOf(":")+1);
 }else{prefix="a" + (_sarissa_iNsCounter++);};};if(sUri){oDoc.loadXML('<' + prefix+':'+sName +" xmlns:" + prefix +"=\"" + sUri +"\"" +" />");
 } else{oDoc.loadXML('<' + sName +" />");};};return oDoc;};Sarissa.getParseErrorText=function (oDoc){
 var parseErrorText=Sarissa.PARSED_OK;if(oDoc && oDoc.parseError && oDoc.parseError.errorCode && oDoc.parseError.errorCode !=0){
 parseErrorText="XML Parsing Error: " + oDoc.parseError.reason +"\nLocation: " + oDoc.parseError.url +"\nLine Number " + oDoc.parseError.line +", Column " + oDoc.parseError.linepos +":\n" + oDoc.parseError.srcText +"\n";
 for(var i=0;i < oDoc.parseError.linepos;i++){parseErrorText +="-";};parseErrorText +="^\n";
 } else if(oDoc.documentElement==null){parseErrorText=Sarissa.PARSED_EMPTY;};return parseErrorText;
 };Sarissa.setXpathNamespaces=function(oDoc,sNsSet){oDoc.setProperty("SelectionLanguage","XPath");
 oDoc.setProperty("SelectionNamespaces",sNsSet);};XSLTProcessor=function(){if(!_SARISSA_XSLTEMPLATE_PROGID){
 _SARISSA_XSLTEMPLATE_PROGID=Sarissa.pickRecentProgID(["MSXML2.XSLTemplate.6.0","MSXML2.XSLTemplate.4.0","MSXML2.XSLTemplate.5.0","MSXML2.XSLTemplate.3.0"]);
 };this.template=new ActiveXObject(_SARISSA_XSLTEMPLATE_PROGID);this.processor=null;
 };XSLTProcessor.prototype.importStylesheet=function(xslDoc){xslDoc.setProperty("SelectionLanguage","XPath");
 xslDoc.setProperty("SelectionNamespaces","xmlns:xsl='http://www.w3.org/1999/XSL/Transform'");
 var output=xslDoc.selectSingleNode("//xsl:output");this.outputMethod=output ? output.getAttribute("method") :"html";
 this.template.stylesheet=xslDoc;this.processor=this.template.createProcessor();this.paramsSet=new Array();
 };XSLTProcessor.prototype.transformToDocument=function(sourceDoc){if(_SARISSA_THREADEDDOM_PROGID){
 this.processor.input=sourceDoc;var outDoc=new ActiveXObject(_SARISSA_DOM_PROGID);
 this.processor.output=outDoc;this.processor.transform();return outDoc;} else{if(!_SARISSA_DOM_XMLWRITER){
 _SARISSA_DOM_XMLWRITER=Sarissa.pickRecentProgID(["MSXML2.MXXMLWriter.6.0","MSXML2.MXXMLWriter.3.0","MSXML2.MXXMLWriter","MSXML.MXXMLWriter","Microsoft.XMLDOM"]);
 };this.processor.input=sourceDoc;var outDoc=new ActiveXObject(_SARISSA_DOM_XMLWRITER);
 this.processor.output=outDoc;this.processor.transform();var oDoc=new ActiveXObject(_SARISSA_DOM_PROGID);
 oDoc.loadXML(outDoc.output+"");return oDoc;};};XSLTProcessor.prototype.transformToFragment=function (sourceDoc,ownerDoc){
 this.processor.input=sourceDoc;this.processor.transform();var s=this.processor.output;
 var f=ownerDoc.createDocumentFragment();if (this.outputMethod=='text'){f.appendChild(ownerDoc.createTextNode(s));
 } else if (ownerDoc.body && ownerDoc.body.innerHTML){var container=ownerDoc.createElement('div');
 container.innerHTML=s;while (container.hasChildNodes()){f.appendChild(container.firstChild);
 } } else{var oDoc=new ActiveXObject(_SARISSA_DOM_PROGID);if (s.substring(0,5)=='<?xml'){
 s=s.substring(s.indexOf('?>') + 2);} var xml=''.concat('<my>',s,'</my>');oDoc.loadXML(xml);
 var container=oDoc.documentElement;while (container.hasChildNodes()){f.appendChild(container.firstChild);
 } } return f;};XSLTProcessor.prototype.setParameter=function(nsURI,name,value){value=value ? value :"";
 if(nsURI){this.processor.addParameter(name,value,nsURI);}else{this.processor.addParameter(name,value);
 };if(!this.paramsSet[""+nsURI]){this.paramsSet[""+nsURI]=new Array();};this.paramsSet[""+nsURI][name]=value;
 };XSLTProcessor.prototype.getParameter=function(nsURI,name){nsURI="" + nsURI;if(this.paramsSet[nsURI] && this.paramsSet[nsURI][name]){
 return this.paramsSet[nsURI][name];}else{return null;};};XSLTProcessor.prototype.clearParameters=function(){
 for(var nsURI in this.paramsSet){for(var name in this.paramsSet[nsURI]){if(nsURI){
 this.processor.addParameter(name,"",nsURI);}else{this.processor.addParameter(name,"");
 };};};this.paramsSet=new Array();};}
else{if(_SARISSA_HAS_DOM_CREATE_DOCUMENT){Sarissa.__handleLoad__=function(oDoc){Sarissa.__setReadyState__(oDoc,4);
 };_sarissa_XMLDocument_onload=function(){Sarissa.__handleLoad__(this);};Sarissa.__setReadyState__=function(oDoc,iReadyState){
 oDoc.readyState=iReadyState;oDoc.readystate=iReadyState;if (oDoc.onreadystatechange !=null && typeof oDoc.onreadystatechange=="function") oDoc.onreadystatechange();
 };Sarissa.getDomDocument=function(sUri,sName){var oDoc=document.implementation.createDocument(sUri?sUri:null,sName?sName:null,null);
 if(!oDoc.onreadystatechange){oDoc.onreadystatechange=null;};if(!oDoc.readyState){
 oDoc.readyState=0;};oDoc.addEventListener("load",_sarissa_XMLDocument_onload,false);
 return oDoc;};Sarissa.getXsltDocument=Sarissa.getDomDocument;if(window.XMLDocument){
 } else if(_SARISSA_HAS_DOM_FEATURE && window.Document && !Document.prototype.load && document.implementation.hasFeature('LS','3.0')){
 Sarissa.getDomDocument=function(sUri,sName){var oDoc=document.implementation.createDocument(sUri?sUri:null,sName?sName:null,null);
 return oDoc;};Sarissa.getXsltDocument=Sarissa.getDomDocument;} else{Sarissa.getDomDocument=function(sUri,sName){
 var oDoc=document.implementation.createDocument(sUri?sUri:null,sName?sName:null,null);
 if(oDoc && (sUri || sName) && !oDoc.documentElement){oDoc.appendChild(oDoc.createElementNS(sUri,sName));
 };return oDoc;};Sarissa.getXsltDocument=Sarissa.getDomDocument;};};}
;if(!window.DOMParser){if(_SARISSA_IS_SAFARI){DOMParser=function(){};DOMParser.prototype.parseFromString=function(sXml,contentType){
 var xmlhttp=new XMLHttpRequest();xmlhttp.open("GET","data:text/xml;charset=utf-8," + encodeURIComponent(sXml),false);
 xmlhttp.send(null);return xmlhttp.responseXML;};}else if(Sarissa.getDomDocument && Sarissa.getDomDocument() && Sarissa.getDomDocument(null,"bar").xml){
 DOMParser=function(){};DOMParser.prototype.parseFromString=function(sXml,contentType){
 var doc=Sarissa.getDomDocument();doc.loadXML(sXml);return doc;};};}
;if((typeof(document.importNode)=="undefined") && _SARISSA_IS_IE){try{document.importNode=function(oNode,bChildren){
 var tmp;if (oNode.nodeName=='#text'){return document.createTextElement(oNode.data);
 } else{if(oNode.nodeName=="tbody" || oNode.nodeName=="tr"){tmp=document.createElement("table");
 } else if(oNode.nodeName=="td"){tmp=document.createElement("tr");} else if(oNode.nodeName=="option"){
 tmp=document.createElement("select");} else{tmp=document.createElement("div");};
 if(bChildren){tmp.innerHTML=oNode.xml ? oNode.xml : oNode.outerHTML;}else{tmp.innerHTML=oNode.xml ? oNode.cloneNode(false).xml : oNode.cloneNode(false).outerHTML;
 };return tmp.getElementsByTagName("*")[0];};};}catch(e){};}
;if(!Sarissa.getParseErrorText){Sarissa.getParseErrorText=function (oDoc){var parseErrorText=Sarissa.PARSED_OK;
 if(!oDoc.documentElement){parseErrorText=Sarissa.PARSED_EMPTY;} else if(oDoc.documentElement.tagName=="parsererror"){
 parseErrorText=oDoc.documentElement.firstChild.data;parseErrorText +="\n" + oDoc.documentElement.firstChild.nextSibling.firstChild.data;
 } else if(oDoc.getElementsByTagName("parsererror").length > 0){var parsererror=oDoc.getElementsByTagName("parsererror")[0];
 parseErrorText=Sarissa.getText(parsererror,true)+"\n";} else if(oDoc.parseError && oDoc.parseError.errorCode !=0){
 parseErrorText=Sarissa.PARSED_UNKNOWN_ERROR;};return parseErrorText;};}
;Sarissa.getText=function(oNode,deep){var s="";var nodes=oNode.childNodes;for(var i=0;
 i < nodes.length;i++){var node=nodes[i];var nodeType=node.nodeType;if(nodeType==Node.TEXT_NODE || nodeType==Node.CDATA_SECTION_NODE){
 s +=node.data;} else if(deep==true && (nodeType==Node.ELEMENT_NODE || nodeType==Node.DOCUMENT_NODE || nodeType==Node.DOCUMENT_FRAGMENT_NODE)){
 s +=Sarissa.getText(node,true);};};return s;}
;if(!window.XMLSerializer && Sarissa.getDomDocument && Sarissa.getDomDocument("","foo",null).xml){
 XMLSerializer=function(){};XMLSerializer.prototype.serializeToString=function(oNode){
 return oNode.xml;};}
;Sarissa.stripTags=function (s){return s.replace(/<[^>]+>/g,"");}
;Sarissa.clearChildNodes=function(oNode){while(oNode.firstChild){oNode.removeChild(oNode.firstChild);
 };}
;Sarissa.copyChildNodes=function(nodeFrom,nodeTo,bPreserveExisting){if((!nodeFrom) || (!nodeTo)){
 throw"Both source and destination nodes must be provided";};if(!bPreserveExisting){
 Sarissa.clearChildNodes(nodeTo);};var ownerDoc=nodeTo.nodeType==Node.DOCUMENT_NODE ? nodeTo : nodeTo.ownerDocument;
 var nodes=nodeFrom.childNodes;if(typeof(ownerDoc.importNode) !="undefined"){for(var i=0;
i < nodes.length;i++){nodeTo.appendChild(ownerDoc.importNode(nodes[i],true));};} else{
 for(var i=0;i < nodes.length;i++){nodeTo.appendChild(nodes[i].cloneNode(true));}
;};}
;Sarissa.moveChildNodes=function(nodeFrom,nodeTo,bPreserveExisting){if((!nodeFrom) || (!nodeTo)){
 throw"Both source and destination nodes must be provided";};if(!bPreserveExisting){
 Sarissa.clearChildNodes(nodeTo);};var nodes=nodeFrom.childNodes;if(nodeFrom.ownerDocument==nodeTo.ownerDocument){
 while(nodeFrom.firstChild){nodeTo.appendChild(nodeFrom.firstChild);};} else{var ownerDoc=nodeTo.nodeType==Node.DOCUMENT_NODE ? nodeTo : nodeTo.ownerDocument;
 if(typeof(ownerDoc.importNode) !="undefined"){for(var i=0;i < nodes.length;i++){
 nodeTo.appendChild(ownerDoc.importNode(nodes[i],true));};}else{for(var i=0;i < nodes.length;
i++){nodeTo.appendChild(nodes[i].cloneNode(true));};};Sarissa.clearChildNodes(nodeFrom);
 };}
;Sarissa.xmlize=function(anyObject,objectName,indentSpace){indentSpace=indentSpace?indentSpace:'';
 var s=indentSpace +'<' + objectName +'>';var isLeaf=false;if(!(anyObject instanceof Object) || anyObject instanceof Number || anyObject instanceof String || anyObject instanceof Boolean || anyObject instanceof Date){
 s +=Sarissa.escape(""+anyObject);isLeaf=true;}else{s +="\n";var itemKey='';var isArrayItem=anyObject instanceof Array;
 for(var name in anyObject){s +=Sarissa.xmlize(anyObject[name],(isArrayItem?"array-item key=\""+name+"\"":name),indentSpace +"   ");
 };s +=indentSpace;};return s +=(objectName.indexOf(' ')!=-1?"</array-item>\n":"</" + objectName +">\n");
}
;Sarissa.escape=function(sXml){return sXml.replace(/&/g,"&amp;").replace(/</g,"&lt;").replace(/>/g,"&gt;").replace(/\"/g,"&quot;").replace(/\'/g,"&apos;");
}
;Sarissa.unescape=function(sXml){return sXml.replace(/&apos\;/g,"'").replace(/&quot\;/g,"\"").replace(/&gt\;/g,">").replace(/&lt\;/g,"<").replace(/&amp\;/g,"&");
}
;if(_SARISSA_HAS_DOM_FEATURE && document.implementation.hasFeature("XPath","3.0")){
 function SarissaNodeList(i){this.length=i;};SarissaNodeList.prototype=new Array(0);
 SarissaNodeList.prototype.constructor=Array;SarissaNodeList.prototype.item=function(i){
 return (i < 0 || i >=this.length)?null:this[i];};SarissaNodeList.prototype.expr="";
 if(window.XMLDocument && (!XMLDocument.prototype.setProperty)){XMLDocument.prototype.setProperty=function(x,y){
};};Sarissa.setXpathNamespaces=function(oDoc,sNsSet){oDoc._sarissa_useCustomResolver=true;
 var namespaces=sNsSet.indexOf(" ")>-1?sNsSet.split(" "):new Array(sNsSet);oDoc._sarissa_xpathNamespaces=new Array(namespaces.length);
 for(var i=0;i < namespaces.length;i++){var ns=namespaces[i];var colonPos=ns.indexOf(":");
 var assignPos=ns.indexOf("=");if(colonPos > 0 && assignPos > colonPos+1){var prefix=ns.substring(colonPos+1,assignPos);
 var uri=ns.substring(assignPos+2,ns.length-1);oDoc._sarissa_xpathNamespaces[prefix]=uri;
 }else{throw"Bad format on namespace declaration(s) given";};};};XMLDocument.prototype._sarissa_useCustomResolver=false;
 XMLDocument.prototype._sarissa_xpathNamespaces=new Array();XMLDocument.prototype.selectNodes=function(sExpr,contextNode,returnSingle){
 var nsDoc=this;var nsresolver=this._sarissa_useCustomResolver ? function(prefix){
 var s=nsDoc._sarissa_xpathNamespaces[prefix];if(s)return s;else throw"No namespace URI found for prefix: '" + prefix+"'";
 } : this.createNSResolver(this.documentElement);var result=null;if(!returnSingle){
 var oResult=this.evaluate(sExpr,(contextNode?contextNode:this),nsresolver,XPathResult.ORDERED_NODE_SNAPSHOT_TYPE,null);
 var nodeList=new SarissaNodeList(oResult.snapshotLength);nodeList.expr=sExpr;for(var i=0;
i<nodeList.length;i++) nodeList[i]=oResult.snapshotItem(i);result=nodeList;} else{
 result=oResult=this.evaluate(sExpr,(contextNode?contextNode:this),nsresolver,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue;
 };return result;};Element.prototype.selectNodes=function(sExpr){var doc=this.ownerDocument;
 if(doc.selectNodes) return doc.selectNodes(sExpr,this);else throw"Method selectNodes is only supported by XML Elements";
 };XMLDocument.prototype.selectSingleNode=function(sExpr,contextNode){var ctx=contextNode?contextNode:null;
 return this.selectNodes(sExpr,ctx,true);};Element.prototype.selectSingleNode=function(sExpr){
 var doc=this.ownerDocument;if(doc.selectSingleNode) return doc.selectSingleNode(sExpr,this);
 else throw"Method selectNodes is only supported by XML Elements";};Sarissa.IS_ENABLED_SELECT_NODES=true;
}
;