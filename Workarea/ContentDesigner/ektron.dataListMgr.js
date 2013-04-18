/// <reference path="ektron.js" />
/* Copyright 2003-2009, Ektron, Inc. */

if (!Ektron.DataListManager) (function() 
{
	Ektron.DataListManager = 
	{
		init: function DataListManager_init(settings)
		{
			settings = $ektron.extend(
			{
				serverUrl: location.protocol + "//" + location.host,
				srcPath: "",
				langType: "", // LCID (decimal, not hex)
				ekXml: null
			}, settings);
			settings.langType = settings.langType + ""; // ensure as string
			if (0 == settings.srcPath.indexOf(settings.serverUrl))
			{
				// strip off serverUrl from beginning of srcPath
				settings.srcPath = settings.srcPath.substr(settings.serverUrl.length);
			}

			s_settings = settings;
					
			if (!Ektron.Xml)
			{
				throw new ReferenceError("Ektron.DataListManager depends on Ektron.Xml. Please include ektron.xml.js.");
			}
			
			this.replaceDataLists = DataListManager_replaceDataLists;
			this.getDataList = DataListManager_getDataList;
		}
	};

	var s_settings = {};
	
	function DataListManager_replaceDataLists(containingElement)
	{
// TODO is this needed?
//		if (!document || !document.body)
//		{
//			setTimeout(function()
//			{
//				arguments.callee.apply(me, arguments);
//			}, 200); // too soon, try again later
//			return;
//		}
		var aryDatalistCache = new Array();
		$ektron("select[ektdesignns_datasrc], div.ektdesignns_choices[ektdesignns_datasrc], div.ektdesignns_checklist[ektdesignns_datasrc], div.design_choices[ektdesignns_datasrc], div.design_checklist[ektdesignns_datasrc]", containingElement).each(function()
		{
			var oElem = this;
			var datasrc = oElem.getAttribute("ektdesignns_datasrc");
			var datalist = oElem.getAttribute("ektdesignns_datalist");
			if ("string" == typeof datalist && datalist.length > 0)
			{
				if ("undefined" == typeof aryDatalistCache[datalist])
				{
					var strSelect = oElem.getAttribute("ektdesignns_dataselect");
					var strCaptionXPath = oElem.getAttribute("ektdesignns_captionxpath");
					var strValueXPath = oElem.getAttribute("ektdesignns_valuexpath");
					var strNamespaces = oElem.getAttribute("ektdesignns_datanamespaces");
					aryDatalistCache[datalist] = Ektron.DataListManager.getDataList(oElem.tagName, datasrc, strSelect, strCaptionXPath, strValueXPath, strNamespaces);
					// Datalist will be empty if running in editor b/c access is denied.
				}
				if (aryDatalistCache[datalist].length > 0)
				{
					s_replaceDataList(oElem, aryDatalistCache[datalist]);
				}
			} // if datalist
		}); // each
	}

	function s_replaceDataList(oElem, strDatalist)
	{
		if ("SELECT" == oElem.tagName)
		{
			var nNumOrigItemsToKeep = 0;
			var validation = oElem.getAttribute("ektdesignns_validation");
			if ("select-req" == validation)
			{
				nNumOrigItemsToKeep = 1;
			}
			var strOrigDataList = "";
			for (var iOption = 0; iOption < oElem.options.length; iOption++)
			{
				var oOption = oElem.options[iOption];
				oOption.innerHTML = $ektron.htmlEncodeText(oOption.text);
				strOrigDataList += Ektron.Xml.serializeXhtml(oOption);
			}
			var strHtml = s_transformDataList(strOrigDataList, strDatalist, nNumOrigItemsToKeep);
			strHtml = strHtml.replace(/<select[^>]*>/,"").replace("</select>",""); // remove SELECT tag
			$ektron(oElem).html(strHtml); // innerHTML for SELECT element doesn't work, so use jQuery
			if (oElem.multiple && oElem.size < 2 && oElem.options.length > 12)
			{
				oElem.size = 12;
			}
		}
		else // choices and checklist
		{
			var oOrigListElem = $ektron(oElem).children("OL").get(0);
			var strOrigDataList = Ektron.Xml.serializeXhtml(oOrigListElem);
			// Can't mix DHTML DOM and XML DOM nodes, so transform returns string of HTML.
			var strHtml = s_transformChoiceDataList(strOrigDataList, strDatalist);
			strHtml = strHtml.replace(/<ol[^>]*>/,"").replace("</ol>",""); // remove OL tag, it's same as orig
			oOrigListElem.innerHTML = strHtml; // replace LI elements
		}
	};
	    
	function DataListManager_getDataList(strDDFieldTagName, strSource, strSelect, strCaptionXPath, strValueXPath, strNamespaces, strLangType)
	{
	// Fetches data list from strSource
		// assert("SELECT" == strDDFieldTagName || "DIV" == strDDFieldTagName)
	    
		var strPrefixes = "";
		if ("undefined" == typeof strNamespaces || null == strNamespaces)
		{
			strNamespaces = "";
		}
		else
		{
			strPrefixes = extractPrefixesFromNamespaces(strNamespaces);
			if (strPrefixes.length > 0) 
			{
				strPrefixes = " exclude-result-prefixes=\"" + strPrefixes + "\"";
			}
		}
	    
		var langType = (strLangType || s_settings.langType) + "";
		if (langType.length > 0)
		{
			langType = "&LangType=" + langType;
		}
		strSource = strSource.replace("&LangType=-1", langType);
	    
		var strXSLT = [
		"<?xml version=\"1.0\"?>",
		"<xsl:stylesheet version=\"1.0\" " + strPrefixes + " xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" " + strNamespaces + ">",
		"<xsl:output method=\"xml\" version=\"1.0\" omit-xml-declaration=\"yes\" indent=\"yes\"/>",
		"<xsl:param name=\"localeUrl\" select=\"'" + s_settings.serverUrl + s_settings.srcPath + "resourcexml.aspx?name=DataListSpec" + langType.replace("&","&amp;") + "'\"/>",
		"<xsl:variable name=\"localeXml\" select=\"document($localeUrl)/*\"/>",
		"<xsl:template match=\"/\">",
		"  <select>",
		"  <xsl:for-each select=\"" + strSelect + "\">",
		"    <option>",
		"      <xsl:if test=\"" + strValueXPath + "\">",
		"        <xsl:attribute name=\"value\">",
		"          <xsl:value-of select=\"" + strValueXPath + "\"/>",
		"        </xsl:attribute>",
		"      </xsl:if>",
		"      <xsl:choose>",
		"      	 <xsl:when test=\"@localeRef\">",
		"      	   <xsl:value-of select=\"$localeXml/data[@name=current()/@localeRef]/value/text()\"/>",
		"      	 </xsl:when>",
		"      	 <xsl:otherwise>",
		"      	   <xsl:value-of select=\"" + strCaptionXPath + "\"/>",
		"      	 </xsl:otherwise>",
		"      </xsl:choose>",
		"    </option>",
		"  </xsl:for-each>",
		"  </select>",
		"</xsl:template>",
		"</xsl:stylesheet>"].join('\n');
	    
		// Transform
		var strHtml = s_settings.ekXml.xslTransform(strSource, strXSLT);
		strHtml = $ektron.trim(strHtml);
	    
		if (strHtml.indexOf("<option") >= 0 || 0 == strHtml.length) 
		{
			return strHtml;
		} 
		else 
		{
			if (Ektron.Xml.onexception) Ektron.Xml.onexception(new Error("Function getDataList returned unexpected result.\n" + strHtml), arguments);
			return ""; // transform error
		}
	}

	function s_transformChoiceDataList(strOrigDataList, strNewDataList)
	{
		// bug The document('') function is the web page in IE6 and the xml source document in Mozilla
		var strXSLT = "";
		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">",
		"<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>",
		"<xsl:template match=\"ol\">",
		"<xslout:variable name=\"nameID\" select=\"'{li/input/@name}'\"/>",
		"<xslout:variable name=\"inputType\" select=\"'{li/input/@type}'\"/>",
		"</xsl:template>",
		"<xsl:template match=\"text()\"/>",
		"</xsl:stylesheet>"].join('\n');
		var strVariables = s_settings.ekXml.xslTransform(strOrigDataList, strXSLT);

		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">",
		"<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>",
		"<xsl:template match=\"/\">",
		"	<xsl:apply-templates/>",
		"</xsl:template>",
		"<xsl:template match=\"ol/li/input[@checked]\">",
		"	<xslout:if test=\"not(option[@value=xpathLiteralString{@value}gnirtSlaretiLhtapx])\">", 
		"    	<xsl:copy-of select=\"..\"/>", // copy LI element
		"	</xslout:if>",
		"</xsl:template>",
		"<xsl:template match=\"text()\"/>",
		"</xsl:stylesheet>"].join('\n');
		var strOldSelectedSnip = s_settings.ekXml.xslTransform(strOrigDataList, strXSLT);

		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">",
		"<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>",
		"<xsl:template match=\"/\">",
		"	<xsl:for-each select=\"ol/li/input[1]/@*[starts-with(name(),'ektdesignns_')]\">",
		"		<xslout:attribute name=\"{name()}\"><xsl:value-of select=\".\"/></xslout:attribute>",
		"	</xsl:for-each>",
		"	<xsl:apply-templates/>",
		"</xsl:template>",
		"<xsl:template match=\"ol/li/input[@checked]\">",
		"	<xslout:if test=\"@value=xpathLiteralString{@value}gnirtSlaretiLhtapx\">", 
		"           <xslout:attribute name=\"checked\">checked</xslout:attribute>",
		"	</xslout:if>",
		"</xsl:template>",
		"<xsl:template match=\"text()\"/>",
		"</xsl:stylesheet>"].join('\n');
		var strNewSelectedSnip = s_settings.ekXml.xslTransform(strOrigDataList, strXSLT);

		var strOlTag = strOrigDataList.match(/<ol[^>]*>/)[0];
		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" exclude-result-prefixes=\"ektdesign\" xmlns:ektdesign=\"urn:ektdesign\">",
		"<xsl:output method=\"xml\" version=\"1.0\" indent=\"yes\" omit-xml-declaration=\"yes\"/>",
	////    "<ektdesign:datalist>",
	////    strOrigDataList; // original ol/li list
	////    "</ektdesign:datalist>",
		// bug The document('') function is the web page in IE6 and the xml source document in Mozilla
	////    "<xsl:variable name=\"oldDataList\" select=\"document('')/xsl:stylesheet/ektdesign:datalist\"/>",
	////    "<xsl:variable name=\"nameID\" select=\"$oldDataList/ol/li/input/@name\"/>",
	////    "<xsl:variable name=\"inputType\" select=\"$oldDataList/ol/li/input/@type\"/>",
	////    "<xsl:variable name=\"ektAttr\" select=\"$oldDataList/ol/li/input/@*[starts-with(name(),'ektdesignns_')]\"/>",
		strVariables,
		// copy original OL tag
		"<xsl:template match=\"/\">",
	////    "    <ol>",
	////    "        <xsl:copy-of select=\"$oldDataList/ol/@*\"/>",
		"	" + strOlTag + "",
		"        <xsl:apply-templates/>",
		"    </ol>",
		"</xsl:template>",
		// copy checked values that are not in the new data list
		"<xsl:template match=\"select\">",
	////    "    <xsl:copy-of select=\"$oldDataList/ol/li/input[@checked and not(@value=current()/option/@value)]\"/>",
		strOldSelectedSnip,
		"    <xsl:apply-templates select=\"node()\"/>",
		"</xsl:template>",
		// process option tags
		"<xsl:template match=\"option\">",
		"    <xsl:variable name=\"modelID\" select=\"generate-id()\"/>",
		"    <xsl:variable name=\"displayOption\" select=\"text()\"/>",
		"    <xsl:variable name=\"valueOption\" select=\"@value\"/>",
		"     <li>",
		// copy attributes except 'checked'
		"    <input type=\"{$inputType}\" id=\"{$modelID}\" title=\"{$displayOption}\" value=\"{$valueOption}\" name=\"{$nameID}\" ektdesignns_nodetype=\"item\">",
	////    "    <xsl:copy-of select=\"$ektAttr\"/>",
		// check if checked in the old data list
	////    "    <xsl:if test=\"$oldDataList/ol/li/input[@value=current()/@value]/@checked\">",
	////    "        <xsl:attribute name=\"checked\">checked</xsl:attribute>",
	////    "    </xsl:if>",
		strNewSelectedSnip,
		"    </input>",
		"    <label for=\"{$modelID}\"><xsl:value-of select=\"$displayOption\"/></label>",
		"    </li>",
		"</xsl:template>",
		// copy the text
		"<xsl:template match=\"*\">",
		"   <xsl:copy>",
		"       <xsl:copy-of select=\"@*\"/>",
		"       <xsl:apply-templates select=\"node()\"/>",
		"   </xsl:copy>",
		"</xsl:template>",
	    
		"</xsl:stylesheet>"].join('\n');

		// Transform
		var strHtml = s_settings.ekXml.xslTransform(strNewDataList, strXSLT);
		return strHtml;
	}

	function s_transformDataList(strOrigDataList, strNewDataList, nNumOrigItemsToKeep)
	{
		// bug The document('') function is the web page in IE6 and the xml source document in Mozilla
		strOrigDataList = "<select>" + strOrigDataList + "</select>";
		var strOldPredicate = "@selected";
		if ("number" == typeof nNumOrigItemsToKeep)
		{
			strOldPredicate += " or position()&lt;=" + nNumOrigItemsToKeep;
		}
		var strXSLT = "";
		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">",
		"<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>",
		"<xsl:template match=\"/\">",
		"	<xsl:apply-templates/>",
		"</xsl:template>",
		"<xsl:template match=\"option[" + strOldPredicate + "]\">",
		"	<xslout:if test=\"not(option[@value=xpathLiteralString{@value}gnirtSlaretiLhtapx])\">", 
		"    	<xsl:copy-of select=\".\"/>",
		"	</xslout:if>",
		"</xsl:template>",
		"<xsl:template match=\"text()\"/>",
		"</xsl:stylesheet>"].join('\n');
		var strOldSelectedSnip = s_settings.ekXml.xslTransform(strOrigDataList, strXSLT);

		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:xslout=\"alias\">",
		"<xsl:namespace-alias stylesheet-prefix=\"xslout\" result-prefix=\"xsl\"/>",
		"<xsl:template match=\"/\">",
		"	<xsl:apply-templates/>",
		"</xsl:template>",
		"<xsl:template match=\"option[@selected]\">",
		"	<xslout:if test=\"@value=xpathLiteralString{@value}gnirtSlaretiLhtapx\">", 
		"           <xslout:attribute name=\"selected\">selected</xslout:attribute>",
		"	</xslout:if>",
		"</xsl:template>",
		"<xsl:template match=\"text()\"/>",
		"</xsl:stylesheet>"].join('\n');
		var strNewSelectedSnip = s_settings.ekXml.xslTransform(strOrigDataList, strXSLT);
		
		strXSLT = [
		"<?xml version='1.0'?>",
		"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" exclude-result-prefixes=\"ektdesign\" xmlns:ektdesign=\"urn:ektdesign\">",
		"<xsl:output method=\"xml\" version=\"1.0\" indent=\"yes\" omit-xml-declaration=\"yes\"/>",
	////    "<ektdesign:datalist>",
	////    strOrigDataList; // original OPTION list
	////    "</ektdesign:datalist>",
		// bug The document('') function is the web page in IE6 and the xml source document in Mozilla
	////    "<xsl:variable name=\"oldDataList\" select=\"document('')/xsl:stylesheet/ektdesign:datalist\"/>",
		"<xsl:template match=\"select\">",
		"   <select>", 
		"   <!-- copy selected values that are not in the new data list -->",
	////    "   <xsl:copy-of select=\"$oldDataList/option[@selected and not(@value=current()/option/@value)]\"/>",
		strOldSelectedSnip,
	    
		"   <!-- process option tags -->",
		"   <xsl:apply-templates select=\"node()\"/>",
		"   </select>",
		"</xsl:template>",
		"<xsl:template match=\"option\">",
		"   <xsl:copy>",
		"       <!-- copy attributes except 'selected' -->",
		"       <xsl:copy-of select=\"@*[name() != 'selected']\"/>",
		"       <!-- check if selected in the old data list -->",
	////    "       <xsl:if test=\"$oldDataList/option[@value=current()/@value]/@selected\">",
	////    "           <xsl:attribute name=\"selected\">selected</xsl:attribute>",
	////    "       </xsl:if>",
		strNewSelectedSnip,
	    
		"       <!-- copy the text -->",
		"       <xsl:copy-of select=\"node()\"/>",
		"       </xsl:copy>",
		"</xsl:template>",
		"<xsl:template match=\"*\"> ",
		"   <xsl:copy>",
		"       <xsl:copy-of select=\"@*\"/>",
		"       <xsl:apply-templates select=\"node()\"/>",
		"   </xsl:copy>",
		"</xsl:template>",
		"</xsl:stylesheet>"].join('\n');
	    
		// Transform
		var strHtml = s_settings.ekXml.xslTransform(strNewDataList, strXSLT);
		strHtml = $ektron.trim(strHtml);
	    
		if (strHtml.indexOf("<option") >= 0 || 0 == strHtml.length) 
		{
			return strHtml;
		} 
		else 
		{
			if (Ektron.Xml.onexception) Ektron.Xml.onexception(new Error("Function transformDataList returned unexpected result.\n" + strHtml), arguments);
			return ""; // transform error
		}
	}

	// private static
	function extractPrefixesFromNamespaces(strNamespaces)
	// strNamespaces is a delimited list of namespace declarations
	{
		var aryNSPrefix = new Array();
		var aryAllNS = strNamespaces.match(/xmlns:\w+=['"][^'"]*['"]/g);
		if (null == aryAllNS) return "";
		aryAllNS.sort();
		var prev = "";
		for (var i = 0; i < aryAllNS.length; i++)
		{
			if (aryAllNS[i] != prev) // exclude duplicates
			{
				var aryNS = aryAllNS[i].match(/xmlns:(\w+)=['"]([^'"]*)['"]/);
				aryNSPrefix[aryNSPrefix.length] = aryNS[1];
				// URI = aryNS[2];
				prev = aryAllNS[i];
			}
		}
		return aryNSPrefix.join(" ");
	}
})(); // Ektron.DataListManager namespace

