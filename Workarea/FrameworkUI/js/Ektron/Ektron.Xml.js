/* Copyright 2007-2009, Ektron, Inc. */
/* XML functionality dependent on Sarissa */
(function () {

    Ektron.Xml = function Xml(settings) {
        /*
        Settings:
			
        srcPath		Replaces [srcpath] in URLs, eg, "[srcpath]ValidateSpec.xml"
        ajaxPath	URL to ekajax*.aspx files, defaults to srcPath
        onexception	(optional) exception handler function for this object
        */
        //if (typeof Sarissa != "object") throw new Error("Sarissa must be included.");
        //if (Sarissa.VERSION != "0.9.9-Ektron") throw new Error("Sarissa version 0.9.9-Ektron is required. The included version is: " + Sarissa.VERSION);
        settings = settings || {};
        var me = this;
        var m_srcPath = settings.srcPath || "";
        var m_ajaxPath = settings.ajaxPath || settings.srcPath || "";
        this.onexception = settings.onexception;

        this.loadXml = Xml_loadXml;
        this.loadXslt = Xml_loadXslt;
        this.xslTransform = Xml_xslTransform;
        this.validateXml = Xml_validateXml;
        this.validateXsd = Xml_validateXsd;
        this.ajaxTransform = Xml_ajaxTransform;
        this.ajaxValidation = Xml_ajaxValidation;
        this.resolveSrcPath = Xml_resolveSrcPath;
        this.fixXml = Xml_fixXml;
        this.indentXml = Xml_indentXml;

        function Xml_loadXml(xml, onexception) {
            // Returns XML DOM document object or null
            try {
                if (typeof xml != "string") return null;
                if (xml.length <= 2) return null;
                var bIsUrl = isUrl(xml);
                if (bIsUrl && s_cacheXml[xml]) return s_cacheXml[xml];

                var xmlDoc = Sarissa.getDomDocument();
                if ("string" == typeof xmlDoc || null == xmlDoc) {
                    throw new ReferenceError("Unable to create XML DOM Document");
                }
                xmlDoc.async = false;
                var strErrMsg = "";
                if (bIsUrl) {
                    var url = this.resolveSrcPath(xml);
                    if (typeof xmlDoc.load != "undefined") {
                        xmlDoc.load(url);
                    }
                    else {
                        var xmlHttp = getXmlHttp();
                        xmlHttp.open("GET", url, false);
                        xmlHttp.send(null);
                        xmlDoc = Ektron.Xml.parseXml(xmlHttp.responseText, Ektron.OnException.returnException);
                    }
                }
                else {
                    var fixed = {};
                    var bDone = false;
                    while (!bDone) {
                        bDone = true;
                        xmlDoc = Ektron.Xml.parseXml(xml, Ektron.OnException.returnException);
                        if ("string" == typeof xmlDoc) // error message
                        {
                            strErrMsg = xmlDoc;
                            if ($ektron.browser.msie) // IE
                            {
                                if (fixed.illegalChars != true
								 && (strErrMsg.indexOf("invalid character") >= 0
								  || strErrMsg.indexOf("tags were not closed") >= 0
								   )) {
                                    xml = m_fixIllegalCharacters(xml);
                                    fixed.illegalChars = true;
                                    bDone = false;
                                }
                                else if (fixed.namespacePrefix != true
								 && strErrMsg.indexOf("undeclared namespace prefix") >= 0) {
                                    xml = m_fixUndeclaredNamespacePrefixes(xml);
                                    fixed.namespacePrefix = true;
                                    bDone = false;
                                }
                                else if (fixed.comments != true
								 && strErrMsg.indexOf("Incorrect syntax was used in a comment") >= 0) {
                                    xml = m_fixComments(xml);
                                    fixed.comments = true;
                                    bDone = false;
                                }
                                else if (fixed.entityNames != true
								 && (strErrMsg.indexOf("semi colon") >= 0
								  || strErrMsg.indexOf("name was started with an invalid character") >= 0
								  || strErrMsg.indexOf("undefined entity") >= 0
									)) {
                                    xml = m_fixUnknownEntityNames(xml);
                                    fixed.entityNames = true;
                                    bDone = false;
                                }
                            }
                            else if ($ektron.browser.mozilla) // Firefox
                            {
                                if (fixed.wellFormed != true
								 && (strErrMsg.indexOf("not well-formed") >= 0
								  || strErrMsg.indexOf("no element found") >= 0
								  || strErrMsg.indexOf("undefined entity") >= 0
								   )) {
                                    xml = m_fixIllegalCharacters(xml);
                                    xml = m_fixComments(xml);
                                    xml = m_fixUnknownEntityNames(xml);
                                    fixed.wellFormed = true;
                                    bDone = false;
                                }
                                else if (fixed.namespacePrefix != true
								 && strErrMsg.indexOf("prefix not bound to a namespace") >= 0) {
                                    xml = m_fixUndeclaredNamespacePrefixes(xml);
                                    fixed.namespacePrefix = true;
                                    bDone = false;
                                }
                            }
                            else if ($ektron.browser.safari) // Safari, Chrome
                            {
                                if (fixed.illegalChars != true
								 && (strErrMsg.indexOf("PCDATA invalid") >= 0 // Chrome
								  || strErrMsg.indexOf("internal error") >= 0 // Safari
								  || strErrMsg.indexOf("out of allowed range") >= 0 // Safari, Chrome
								   )) {
                                    xml = m_fixIllegalCharacters(xml);
                                    fixed.illegalChars = true;
                                    bDone = false;
                                }
                                else if (fixed.namespacePrefix != true
								 && strErrMsg.indexOf("Namespace prefix") >= 0) {
                                    xml = m_fixUndeclaredNamespacePrefixes(xml);
                                    fixed.namespacePrefix = true;
                                    bDone = false;
                                }
                                else if (fixed.comments != true
								 && (strErrMsg.indexOf("double-hyphen") >= 0 // Safari
								  || strErrMsg.indexOf("Comment not terminated") >= 0 // Chrome
								    )) {
                                    xml = m_fixComments(xml);
                                    fixed.comments = true;
                                    bDone = false;
                                }
                                else if (fixed.entityNames != true
								 && (strErrMsg.indexOf("EntityRef") >= 0
								 || (strErrMsg.indexOf("Entity") >= 0 && strErrMsg.indexOf("not defined") >= 0)
								    )) {
                                    xml = m_fixUnknownEntityNames(xml);
                                    fixed.entityNames = true;
                                    bDone = false;
                                }
                            }
                            else if ($ektron.browser.opera) // Opera
                            {
                                // error message is always "Unknown source"
                                if (fixed.allKnown != true) {
                                    xml = this.fixXml(xml);
                                    fixed.allKnown = true;
                                    bDone = false;
                                }
                            }
                            if (bDone) {
                                if (fixed.missingRoot != true) {
                                    // add root tags in case that is the problem
                                    xml = "<root>" + xml + "</root>";
                                    fixed.missingRoot = true;
                                    bDone = false;
                                }
                            }
                        } // if error
                    } // while parse
                } // url or string
                var strErrMsg = (("string" == typeof xmlDoc) ? xmlDoc : Sarissa.getParseErrorText(xmlDoc));
                if (strErrMsg != Sarissa.PARSED_OK) {
                    throw new Error(strErrMsg + "\n\nXML:\n" + xml);
                }
                if (bIsUrl) {
                    s_cacheXml[xml] = xmlDoc;
                }
                return xmlDoc;
            }
            catch (ex) {
                return Ektron.OnException(this, onexception, ex, arguments);
            }
        };

        function Xml_loadXslt(xslt, onexception) {
            // Returns XSLT DOM document object or null
            try {
                if (typeof xslt != "string") return null;
                if (xslt.length <= 2) return null;
                var bIsUrl = isUrl(xslt);
                if (bIsUrl && s_cacheXslt[xslt]) return s_cacheXslt[xslt];
                var xslDoc = Sarissa.getXsltDocument();
                if ("string" == typeof xslDoc || null == xslDoc) {
                    throw new ReferenceError("Unable to create XSLT DOM Document");
                }
                xslDoc.async = false;
                if (bIsUrl) {
                    var url = this.resolveSrcPath(xslt);
                    if (typeof xslDoc.load != "undefined") {
                        xslDoc.load(url);
                    }
                    else {
                        var xmlHttp = getXmlHttp();
                        xmlHttp.open("GET", url, false);
                        xmlHttp.send(null);
                        xslDoc = Ektron.Xml.parseXml(xmlHttp.responseText, Ektron.OnException.throwException);
                    }
                }
                else {
                    if (xsltMustBeUrl(xslt)) {
                        throw new RangeError("XSLT string documents cannot contain document(''), xsl:include, or xsl:import");
                    }
                    if (typeof xslDoc.loadXML != "undefined") {
                        xslDoc.loadXML(xslt);
                    }
                    else {
                        xslDoc = Ektron.Xml.parseXml(xslt, Ektron.OnException.throwException);
                    }
                }
                var strErrMsg = (("string" == typeof xslDoc) ? xslDoc : Sarissa.getParseErrorText(xslDoc));
                if (strErrMsg != Sarissa.PARSED_OK) {
                    throw new Error(strErrMsg);
                }
                if (bIsUrl) {
                    s_cacheXslt[xslt] = xslDoc;
                }
                return xslDoc;
            }
            catch (ex) {
                return Ektron.OnException(this, onexception, ex, arguments);
            }
        };

        function Xml_xslTransform(xml, xslt, args, onexception) {
            // args is optional array of objects w/ properties 'name' and 'value'.
            // Returns a string
            // For IE (and may be others) the output must be wrapped by an outer tag
            try {
                if ("undefined" == typeof XSLTProcessor) //for Safari
                {
                    return this.ajaxTransform(xml, xslt, args);
                }

                var bIsUrl = isUrl(xslt);
                if (!bIsUrl &&
					xml.indexOf('<root') != 0 &&
					xslt.indexOf('ektdesignns_role="root"') > -1 &&
					xslt.indexOf('<xsl:stylesheet') == 0
					)
                {
                    bIsUrl = true;
                }
                if (m_ajaxPath && bIsUrl) {
                    // XSLT may use extension object (ekext:) so run on server
                    // Firefox transform may not properly HTML encode so run on server
                    return this.ajaxTransform(xml, xslt, args);
                }
                else if (!bIsUrl && ( /* xsltUsesParam(xslt) || */xsltMustBeUrl(xslt) || xsltUsesExtObj(xslt))) {
                    return this.ajaxTransform(xml, xslt, args);
                }
                else {
                    xml = $ektron.trim(xml);
                    var bRootTagAdded = false;
                    //                    if ("<" == xml.substr(0, 1) && xml.substr(xml.length - 7) != "</root>")
                    //                    {
                    //                        xml = "<root>" + xml + "</root>";
                    //                        bRootTagAdded = true;
                    //                    }
                    // IE9, '«' and '»' are invalid characters in XSLTProcessor. (part 1)			
                    xml = xml.replace(/\xAB/g, "&amp;#171;").replace(/\xBB/g, "&amp;#187;");
                    var xmlDoc = this.loadXml(xml);
                    if ("string" == typeof xmlDoc) return xmlDoc;
                    if (null == xmlDoc) throw new Error("Unable to load XML document");

                    var processor = null;
                    if (bIsUrl && s_cacheXslProc[xslt]) {
                        processor = s_cacheXslProc[xslt];
                        processor.clearParameters();
                    }
                    else {
                        var xsltDoc = this.loadXslt(xslt);
                        if ("string" == typeof xsltDoc) return xsltDoc;
                        if (null == xsltDoc) throw new Error("Unable to load XSLT document");

                        processor = new XSLTProcessor();
                        processor.importStylesheet(xsltDoc); // cached xsltDoc can only be imported once
                        if (bIsUrl) {
                            s_cacheXslProc[xslt] = processor;
                        }
                    }
                    processor.setParameter("", "currentDate", Ektron.Xml.serializeDate(new Date()));
                    if (args) {
                        for (var i = 0; i < args.length; i++) {
                            processor.setParameter("", args[i].name, args[i].value);
                        }
                    }
                    var newDoc = null;
                    // default output method is indeterminate until AFTER the transform is run (http://www.w3.org/TR/xslt#output)
                    try
                    {
                        if ("html" == processor.outputMethod) // may be undefined
                        {
                            newDoc = processor.transformToFragment(xmlDoc, window.document);
                            if ("string" == typeof newDoc) return newDoc; // error 
                            if (null == newDoc.firstChild) return ""; // result is empty string

                            var container = window.document.createElement('div');
                            container.appendChild(newDoc.firstChild);
                            result = container.innerHTML;
                        }
                        else {
                            //newDoc = processor.transformToDocument(xmlDoc); // requires root tag in output
                            var ownerDoc = Sarissa.getDomDocument();
                            newDoc = processor.transformToFragment(xmlDoc, ownerDoc);
                            if ("string" == typeof newDoc) return newDoc;
                            if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) >= 9) {
                                result = Ektron.Xml.ensureWelFormed(newDoc.xml);
                            }
                            else {
                                result = Ektron.Xml.serializeXml(newDoc);
                            }
                        }
                    }
                    catch (ex)
                    {
                        return this.ajaxTransform(xml, xslt, args); //#63316
                    }
                    if (bRootTagAdded) {
                        // the root tag added for IE9 loadXml() needs to be removed.
                        result = result.replace(/^<root>/i, "").replace(/<\/root>$/i, "");
                    }
                    // IE9, '«' and '»' are invalid characters in XSLTProcessor. (part 2)			
                    result = result.replace(/&amp;#171;/g, "&#171;").replace(/&amp;#187;/g, "&#187;");
                    if ($ektron.browser.opera) {
                        // Opera may wrap output with result tags, so remove them
                        result = result.replace(/^<result>/, "").replace(/<\/result>$/, "");
                    }
                    // Mozilla may wrap output with transformiix:result tags, so remove them
                    result = result.replace(/<transformiix:result[^>]*>/, "").replace("</transformiix:result>", "");
                    // Mozilla ignores namespace-alias, so use reg exp to manually 'alias'.
                    result = result.replace(/xslout:/g, "xsl:");
                    result = result.replace(/<\?[^\?]*\?>/, ""); // remove xml declaration, if it exists
                    // Can't use js extension to call xpathLiteralString in the XSLT, so do it here.
                    result = result.replace(/xpathLiteralString(.*?)gnirtSlaretiLhtapx/g,
								function (s, p1) {
								    if (p1.indexOf("'") >= 0) {
								        return "concat('" + p1.replace(/\'/g, "',&quot;'&quot;,'") + "')";
								    }
								    else {
								        return "'" + p1 + "'";
								    }
								});
                    return result;
                }
            }
            catch (ex) {
                return Ektron.OnException(this, onexception, ex, arguments);
            }
        };
        //Xml_xslTransform.onexception = Ektron.OnException.returnArgument(0); // arg 0 is the xml
        Xml_xslTransform.onexception = function (ex, args) {
            if (document.cookie && document.cookie.indexOf("Xml_xslTransform.onexception=true") > -1) {
                var msg = Ektron.OnException.exceptionMessage(ex);
                msg = msg.replace(/\&lt;br ?\/?\&gt;/gi, "\n");
                if ($ektron.htmlDecode) msg = $ektron.htmlDecode(msg);
                msg = "XSLT Transformation Error\n" + msg + "\n";
                if (args && args.length > 0) msg += "XML:\n" + args[0] + "\n";
                if (args && args.length > 1) msg += "XSLT:\n" + args[1] + "\n";
                alert(msg);
            }
            if (args && args.length > 0) return args[0]; // arg 0 is the xml
        };

        function Xml_validateXml(xml, xsd, nsuri, onexception) {
            // xsd and nsuri may be string (1 schema) or array (multiple schemas)
            // Returns null if valid or array of error messages.
            try {
                if (isUrl(xml)) {
                    xml = this.resolveSrcPath(xml);
                }
                var schemas = new Array();
                var namespaces = new Array();
                if ("string" == typeof xsd && "string" == typeof nsuri) {
                    if (isUrl(xsd)) {
                        schemas[0] = this.resolveSrcPath(xsd);
                    }
                    else {
                        schemas[0] = xsd;
                    }
                    namespaces[0] = nsuri;
                }
                else if ("object" == typeof xsd && "object" == typeof nsuri && xsd && nsuri && xsd.length == nsuri.length) {
                    for (var i = 0; i < xsd.length; i++) {
                        if (isUrl(xsd[i])) {
                            schemas[i] = this.resolveSrcPath(xsd[i]);
                        }
                        else {
                            schemas[i] = xsd[i];
                        }
                        namespaces[i] = nsuri[i];
                    }
                }
                else {
                    throw new TypeError("'xsd' and 'nsuri' must be strings (for single schema) or array (for multiple schemas)");
                }

                return this.ajaxValidation(xml, schemas, namespaces);
            }
            catch (ex) {
                return Ektron.OnException(this, onexception, ex, arguments);
            }
        };

        function Xml_validateXsd(xsd, onexception) {
            // Returns null if valid or array of error messages.
            try {
                if (isUrl(xsd)) {
                    xsd = this.resolveSrcPath(xsd);
                }
                return this.ajaxValidation("", [xsd], ["http://www.w3.org/2001/XMLSchema"]);
            }
            catch (ex) {
                return Ektron.OnException(this, onexception, ex, arguments);
            }
        };

        // Safari/2.0 has XSLT built in, but it is not exposed to JavaScript,
        // but is only applied to XML documents that have a stylesheet
        // declaration when they are loaded. 
        // Google's Ajaxslt is only partly implemented and is not a good solution. 
        // The ajaxTransform and ajaxValidation methods are adapted from 
        // http://www.w3schools.com/ajax/ajax_browsers.asp 

        function Xml_ajaxTransform(xml, xslt, args, onexception) {
            // args is optional array of objects w/ properties 'name' and 'value'.
            try {
                if (isUrl(xml)) {
                    xml = this.resolveSrcPath(xml);
                }
                if (isUrl(xslt)) {
                    xslt = this.resolveSrcPath(xslt);
                }

                var strArgs = "";
                if (args) {
                    for (var i = 0; i < args.length; i++) {
                        strArgs += "&arg" + i + "=" + encodeURIComponent(args[i].name + "=" + args[i].value);
                    }
                }
                var url = m_ajaxPath + "ekajaxtransform.aspx";
                var sessionid = $ektron('.ektronsessionidfield').val();
                var xid = $ektron('[name=xid]').val();
                if (!(xid > 0))
                    xid = $ektron('[name=xml_collection_id]').val();
                var xmlHttp = getXmlHttp();
                xmlHttp.open("POST", url, false);
                xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp.send("xml=" + encodeURIComponent(xml) + "&sessionid=" + sessionid + "&xid=" + xid + "&xslt=" + encodeURIComponent(xslt) + strArgs);
                var output = $ektron.trim(xmlHttp.responseText); // to return xml dom object: return xmlHttp.responseXML;
                if (output.indexOf("XmlExceptionMessage") > -1 || output.indexOf("ekAjaxTransformError") > -1)
                {
                    var matchResult = output.match(/<body[^>]*>([\w\W]*?)<\/body>/);
                    if (matchResult && matchResult.length >= 2)
                    {
                        throw new Error(matchResult[1]);
                    }
                    else {
                        throw new Error(output);
                    }
                }
                return output;
            }
            catch (ex) {
                return Ektron.OnException(this, onexception, ex, arguments);
            }
        };
        Xml_ajaxTransform.onexception = Ektron.OnException.throwException;

        function Xml_ajaxValidation(xml, schemas, namespaces, onexception) {
            try {
                var strSchemas = "";
                for (var i = 0; i < schemas.length; i++) {
                    strSchemas += "&xsd" + i + "=" + encodeURIComponent(schemas[i]) + "&nsuri" + i + "=" + encodeURIComponent(namespaces[i]);
                }

                var url = m_ajaxPath + "ekajaxvalidation.aspx";
                var xmlHttp = getXmlHttp();
                xmlHttp.open("POST", url, false);
                xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp.send("xml=" + encodeURIComponent(xml) + strSchemas);
                var response = $ektron.trim(xmlHttp.responseText);
                var ary = response.match(/<body><div>([\w\W\r\n]*)<\/div><\/body>/);
                msg = (ary && ary.length >= 2 ? ary[1] : "");
                if (0 == msg.length) {
                    return null; // valid
                }
                else {
                    return (msg.split("\n\n\n")); // array of error messages
                }
            }
            catch (ex) {
                return Ektron.OnException(this, onexception, ex, arguments);
            }
        };
        Xml_ajaxValidation.onexception = Ektron.OnException.throwException;

        function Xml_resolveSrcPath(url) {
            url = url.replace(/.*(\[|%5B)srcpath(\]|%5D)\/?/i, m_srcPath);
            url = url.replace(/.*(\[|%5B)eWebEditProPath(\]|%5D)\/?/i, m_srcPath);
            return url;
        };

        function Xml_fixXml(xml, onexception) {
            // Returns XML string with some common problems fixed.
            try {
                xml = m_fixIllegalCharacters(xml);
                xml = m_fixUndeclaredNamespacePrefixes(xml);
                xml = m_fixComments(xml);
                xml = m_fixUnknownEntityNames(xml);
            }
            catch (ex) {
                return Ektron.OnException(this, onexception, ex, arguments);
            }
            return xml;
        };

        function Xml_indentXml(xml, onexception) {
            // Return XML string with indentations
            try {
                var strIndent = "";
                xml = xml.replace(/(<\!\-\-[\w\W]*?\-\->)|(<\!\[CDATA\[[\w\W]*?\]\]>)|(<\/[^>]+>)|(<[^>]+\/>)|(<[^>]+><\/[^>]+>)|(<[^>]+>)|([^<]+)/g,
                    function ($0_match, $1_comment, $2_cdata, $3_closingTag, $4_emptyShortNotation, $5_emptyLongNotation, $6_openingTag, $7_textNode) {
                        if ($1_comment || $2_cdata) {
                            return $0_match + "\n";
                        }
                        else if ($3_closingTag) {
                            if (strIndent.length > 0) strIndent = strIndent.substring(0, strIndent.length - 2);
                            return strIndent + $0_match + "\n";
                        }
                        else if ($7_textNode) {
                            var retLine = $ektron.trim($7_textNode);
                            if (retLine.length > 0) {
                                retLine = strIndent + retLine + "\n";
                            }
                            return retLine;
                        }
                        else {
                            var retLine = strIndent + $0_match + "\n";
                            if ($6_openingTag) {
                                strIndent += "  ";
                            }
                            return retLine;
                        }
                    });
            }
            catch (ex) {
                return Ektron.OnException(this, onexception, ex, arguments);
            }
            return xml;
        };

        // private methods

        // http://www.w3.org/TR/2006/REC-xml-20060816/#NT-Char
        // Char    ::=    #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF] 
        Ektron.RegExp.illegalXmlCharacters = /[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD]/g;

        function m_fixIllegalCharacters(xml) {
            return xml.replace(Ektron.RegExp.illegalXmlCharacters, "");
        }

        Ektron.Xml.namespaces =
		{
		    asp: "http://schemas.microsoft.com/ASPNET/20"
		, cms: "urn:Ektron.Cms.Controls"
		, admin: "http://webns.net/mvcb/"
		, atom: "http://www.w3.org/2005/Atom"
		, content: "http://purl.org/rss/1.0/modules/content/"
		, dc: "http://purl.org/dc/elements/1.1/"
		, foaf: "http://xmlns.com/foaf/0.1/"
		, opml: "http://www.opml.org/spec2"
		, rdf: "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
		, rdfs: "http://www.w3.org/2000/01/rdf-schema#"
		, msxsl: "urn:schemas-microsoft-com:xslt"
		, math: "http://www.w3.org/1998/Math/MathML"
		, svg: "http://www.w3.org/2000/svg"
		, its: "http://www.w3.org/2005/11/its"
		, htm: "http://www.w3.org/1999/xhtml"
		, html: "http://www.w3.org/1999/xhtml"
		, xhtml: "http://www.w3.org/1999/xhtml"
		, xlink: "http://www.w3.org/1999/xlink"
		, xforms: "http://www.w3.org/2002/xforms"
		, xs: "http://www.w3.org/2001/XMLSchema"
		, xsd: "http://www.w3.org/2001/XMLSchema"
		, xsi: "http://www.w3.org/2001/XMLSchema-instance"
		, xsl: "http://www.w3.org/1999/XSL/Transform"
		};
        function m_fixUndeclaredNamespacePrefixes(xml) {
            // A common problem is undeclared namespaces, particularly when 
            // MS Word content is pasted that has smart tags.
            // Get list of namespace prefixes
            var strNamespaces = "";
            var prefixes = {};
            function savePrefix($0_match, $1_prefix) {
                try {
                    prefixes[$1_prefix] = true;
                }
                catch (ex) {
                    // just in case some illegal prefix is found
                }
                return $0_match; // not actually changing anything
            };
            // Match tag name prefixes
            xml.replace(/<(\w+):\w+/g, savePrefix);
            // Match attribute prefixes
            xml.replace(/\s(\w+):\w+=(?=[^<>]*>)/g, savePrefix);

            for (var p in prefixes) {
                if (p != "xml" && p != "xmlns") {
                    var re = new RegExp("xmlns:" + p + "=");
                    if (!re.test(xml)) {
                        var ns = Ektron.Xml.namespaces[p] || ("urn:unknown:" + p);
                        strNamespaces += " xmlns:" + p + "=\"" + ns + "\"";
                    }
                }
            }
            if (strNamespaces.length > 0) {
                xml = "<root" + strNamespaces + ">" + xml + "</root>";
            }
            return xml;
        };

        function m_fixComments(xml) {
            xml = xml.replace(/<\!--[\-]+/, "<!\x2D\x2D");
            xml = xml.replace(/--[\-]+>/, "\x2D\x2D>");
            xml = xml.replace(/(<\!--)([\w\W]*?)(-->)/g, function ($0_match, $1_open, $2_data, $3_close) {
                // "--" is not allowed in comments, even if valid JavaScript :-(
                return $1_open + $2_data.replace(/-{2,}/g, function ($0_match) {
                    return $0_match.replace(/-/g, "=");
                }) + $3_close;
            });
            return xml;
        }

        function m_fixUnknownEntityNames(xml) {
            // A common problem is "&" in URL that should be "&amp;"
            // Likewise, &nbsp; or other HTML entity name is used.
            // Convert & to &amp; unless it is an XML entity or cdata/comment.
            function protectAmp($0_match, $1_open, $2_data, $3_close) {
                return $1_open + $2_data.replace(/&/g, "ektTempAmp") + $3_close;
            };
            xml = xml.replace(Ektron.RegExp.Entity.entityName, function ($0_match, $1_name) {
                var codePoint = Ektron.Xml.htmlEntity[$1_name];
                if (codePoint) {
                    return "&#" + codePoint + ";";
                }
                else {
                    return $0_match;
                }
            });
            xml = xml.replace(/(<\!--)([\w\W]*?)(-->)/g, protectAmp);
            xml = xml.replace(/(<\!\[CDATA\[)([\w\W]*?)(\]\]>)/g, protectAmp);
            xml = xml.replace(/&(?!#|amp;|lt;|gt;|quot;|apos;)/g, "&amp;");
            xml = xml.replace(/ektTempAmp/g, "&");
            return xml;
        };

    }; // constructor

    if (document.cookie && document.cookie.indexOf("Ektron.Xml.onexception=true") > -1) {
        Ektron.Xml.onexception = function (ex, args) {
            var msg = Ektron.OnException.exceptionMessage(ex);
            msg = msg.replace(/\&lt;br ?\/?\&gt;/gi, "\n");
            if ($ektron.htmlDecode) msg = $ektron.htmlDecode(msg);
            alert(msg);
        };
    }

    /* ******************************************* */
    /*                                             */
    /*     parseXml       serializeXml             */
    /*                                             */
    /* ******************************************* */

    Ektron.Xml.parseXml = function parseXml(xml, onexception) {
        try {
            if (!s_domParser) s_domParser = new DOMParser();
            var xmlDoc = s_domParser.parseFromString(xml, "text/xml");
            var strErrMsg = Sarissa.getParseErrorText(xmlDoc);
            if (Ektron.OnException.returnException == onexception && strErrMsg != Sarissa.PARSED_OK) return strErrMsg; //error is expected: for example fixXml() will be called.
            if (xml && strErrMsg != Sarissa.PARSED_OK) throw new Error(strErrMsg);
            return xmlDoc;
        }
        catch (ex) {
            return Ektron.OnException(this, onexception, ex, arguments);
        }
    };
    Ektron.Xml.parseXml.onexception = Ektron.OnException.returnValue(null);
    var s_domParser = null;

    Ektron.Xml.ensureWelFormed = function ensureWelFormed(xml) {
        if (typeof xml != "string") return "";
        if (0 == xml.length) return "";
        // Opera prepends xml declaration
        if (/^<\?xml version\=\"1\.0\"\?>/.test(xml)) {
            xml = xml.substring(21);
        }
        xml = xml.replace(/<([a-z1-6]+)([^>]*)\/>/g, function ($0_tag, $1_tagName, $2_attrs) {
            // Check if HTML tag that needs to use long notation
            var tagNotation = Ektron.Xml.htmlTagCount[$1_tagName];
            if (2 == tagNotation) {
                return "<" + $1_tagName + $2_attrs + "></" + $1_tagName + ">";
            }
            else if (1 == tagNotation) {
                return "<" + $1_tagName + $ektron.rtrim($2_attrs) + " />"; // ensure space before />
            }
            else {
                return $0_tag; // no change
            }
        });
        return xml;
    };
    Ektron.Xml.serializeXml = function serializeXml(xmlDoc) {
        if (!s_xmlSerializer) s_xmlSerializer = new XMLSerializer();
        var xml = s_xmlSerializer.serializeToString(xmlDoc);
        return this.ensureWelFormed(xml);
    };
    var s_xmlSerializer = null;

    Ektron.Xml.htmlTagCount = /* 2=long notation, eg, <a></a>; 1=short notation, eg, <br /> */
	{
	a: 2, abbr: 2, acronym: 2, address: 2, applet: 2, area: 1,
	b: 2, base: 1, basefont: 1, bdo: 2, bgsound: 2, big: 2, blink: 2, blockquote: 2, body: 2, br: 1, button: 2,
	caption: 2, center: 2, cite: 2, code: 2, col: 1, colgroup: 2, comment: 2,
	dd: 2, del: 2, dfn: 2, dir: 2, div: 2, dl: 2, dt: 2, em: 2, embed: 2,
	fieldset: 2, font: 2, form: 2, frame: 1, frameset: 2,
	h1: 2, h2: 2, h3: 2, h4: 2, h5: 2, h6: 2, head: 2, hr: 1, html: 2,
	i: 2, iframe: 2, img: 1, input: 1, ins: 2, isindex: 1, kbd: 2, keygen: 1,
	label: 2, legend: 2, li: 2, link: 1, listing: 2,
	map: 2, marquee: 2, menu: 2, meta: 1, nobr: 2, noembed: 2, noframes: 2, noscript: 2,
	object: 2, ol: 2, optgroup: 2, option: 2,
	p: 2, param: 1, plaintext: 2, pre: 2, q: 2, rb: 2, rbc: 2, rp: 2, rt: 2, rtc: 2, ruby: 2,
	s: 2, samp: 2, script: 2, select: 2, small: 2, span: 2, strike: 2, strong: 2, style: 2, sub: 2, sup: 2,
	table: 2, tbody: 2, td: 2, textarea: 2, tfoot: 2, th: 2, thead: 2, title: 2, tr: 2, tt: 2,
	u: 2, ul: 2, "var": 2, wbr: 2, xml: 2, xmp: 2
};

    // XHTML 1.0 Entities
    Ektron.Xml.htmlEntity =
	{
	    // A.2.1. Latin-1 characters (xhtml-lat1.ent)
	    nbsp: 160, iexcl: 161, cent: 162, pound: 163, curren: 164, yen: 165, brvbar: 166, sect: 167, uml: 168,
	    copy: 169, ordf: 170, laquo: 171, not: 172, shy: 173, reg: 174, macr: 175, deg: 176, plusmn: 177,
	    sup2: 178, sup3: 179, acute: 180, micro: 181, para: 182, middot: 183, cedil: 184, sup1: 185, ordm: 186,
	    raquo: 187, frac14: 188, frac12: 189, frac34: 190, iquest: 191,
	    Agrave: 192, Aacute: 193, Acirc: 194, Atilde: 195, Auml: 196, Aring: 197, AElig: 198, Ccedil: 199,
	    Egrave: 200, Eacute: 201, Ecirc: 202, Euml: 203, Igrave: 204, Iacute: 205, Icirc: 206, Iuml: 207, ETH: 208,
	    Ntilde: 209, Ograve: 210, Oacute: 211, Ocirc: 212, Otilde: 213, Ouml: 214, times: 215, Oslash: 216,
	    Ugrave: 217, Uacute: 218, Ucirc: 219, Uuml: 220, Yacute: 221, THORN: 222, szlig: 223,
	    agrave: 224, aacute: 225, acirc: 226, atilde: 227, auml: 228, aring: 229, aelig: 230, ccedil: 231,
	    egrave: 232, eacute: 233, ecirc: 234, euml: 235, igrave: 236, iacute: 237, icirc: 238, iuml: 239, eth: 240,
	    ntilde: 241, ograve: 242, oacute: 243, ocirc: 244, otilde: 245, ouml: 246, divide: 247, oslash: 248,
	    ugrave: 249, uacute: 250, ucirc: 251, uuml: 252, yacute: 253, thorn: 254, yuml: 255,
	    // A.2.2. Special characters (xhtml-special.ent)
	    OElig: 338, oelig: 339, Scaron: 352, scaron: 353, Yuml: 376, circ: 710, tilde: 732,
	    ensp: 8194, emsp: 8195, thinsp: 8201, zwnj: 8204, zwj: 8205, lrm: 8206, rlm: 8207,
	    ndash: 8211, mdash: 8212, lsquo: 8216, rsquo: 8217, sbquo: 8218, ldquo: 8220, rdquo: 8221, bdquo: 8222,
	    dagger: 8224, Dagger: 8225, permil: 8240, lsaquo: 8249, rsaquo: 8250, euro: 8364,
	    // A.2.3. Symbols (xhtml-symbol.ent)
	    fnof: 402, Alpha: 913, Beta: 914, Gamma: 915, Delta: 916, Epsilon: 917, Zeta: 918, Eta: 919,
	    Theta: 920, Iota: 921, Kappa: 922, Lambda: 923, Mu: 924, Nu: 925, Xi: 926, Omicron: 927, Pi: 928,
	    Rho: 929, Sigma: 931, Tau: 932, Upsilon: 933, Phi: 934, Chi: 935, Psi: 936, Omega: 937,
	    alpha: 945, beta: 946, gamma: 947, delta: 948, epsilon: 949, zeta: 950, eta: 951,
	    theta: 952, iota: 953, kappa: 954, lambda: 955, mu: 956, nu: 957, xi: 958, omicron: 959, pi: 960,
	    rho: 961, sigmaf: 962, sigma: 963, tau: 964, upsilon: 965, phi: 966, chi: 967, psi: 968, omega: 969,
	    thetasym: 977, upsih: 978, piv: 982, bull: 8226, hellip: 8230, prime: 8242, Prime: 8243,
	    oline: 8254, frasl: 8260, weierp: 8472, image: 8465, real: 8476, trade: 8482, alefsym: 8501,
	    larr: 8592, uarr: 8593, rarr: 8594, darr: 8595, harr: 8596, crarr: 8629,
	    lArr: 8656, uArr: 8657, rArr: 8658, dArr: 8659, hArr: 8660,
	    forall: 8704, part: 8706, exist: 8707, empty: 8709, nabla: 8711, isin: 8712,
	    notin: 8713, ni: 8715, prod: 8719, sum: 8721, minus: 8722, lowast: 8727, radic: 8730, prop: 8733,
	    infin: 8734, ang: 8736, and: 8743, or: 8744, cap: 8745, cup: 8746, "int": 8747, there4: 8756,
	    sim: 8764, cong: 8773, asymp: 8776, ne: 8800, equiv: 8801, le: 8804, ge: 8805,
	    sub: 8834, sup: 8835, nsub: 8836, sube: 8838, supe: 8839, oplus: 8853, otimes: 8855,
	    perp: 8869, sdot: 8901, lceil: 8968, rceil: 8969, lfloor: 8970, rfloor: 8971,
	    lang: 9001, rang: 9002, loz: 9674, spades: 9824, clubs: 9827, hearts: 9829, diams: 9830
	};
    Ektron.RegExp.Entity.entityName = /&(\w+);/g;

    /* ******************************************* */
    /*                                             */
    /*     serializeXhtml                          */
    /*                                             */
    /*     Depends on ektron.string.js             */
    /*                                             */
    /* ******************************************* */

    Ektron.Xml.serializeXhtml = function serializeXhtml(nodes)
    // Adapted from RadConvertToXhtmlFilter GetXhtml
    {
        if (!Ektron.String) {
            throw new ReferenceError("Ektron.Xml.serializeXhtml depends on Ektron.String. Please include ektron.string.js.");
        }
        if (!nodes) return "";
        var m_uniqueIds = {};
        var m_sb = new Ektron.String();
        if (11 == nodes.nodeType) // #document-fragment
        {
            nodes = nodes.childNodes;
        }
        if (nodes.length >= 0 && "undefined" == typeof nodes.nodeType) {
            if (nodes.length > 0) {
                var node = nodes[0];
                if ("undefined" == typeof node) throw m_invalidNodesArgumentError(nodes);
                for (var i = 0; i < nodes.length; i++) {
                    node = nodes[i]
                    if (node) {
                        if ("undefined" == typeof node.nodeType) throw m_invalidNodesArgumentError(nodes);
                        m_appendNodeXhtml(node);
                    }
                }
            }
        }
        else {
            if ("undefined" == typeof nodes.nodeType) throw m_invalidNodesArgumentError(nodes);
            m_appendNodeXhtml(nodes);
        }
        return m_sb.toString();

        function m_invalidNodesArgumentError(nodes) {
            return new TypeError(Ektron.String.format("Error in Ektron.Xml.serializeXhtml: 'nodes' must be a Node or array of Node. 'nodes' is of type '{0}'.", typeof nodes));
        }

        // optimized for speed
        function m_appendNodeXhtml(node) {
            //IE ONLY - because of PasteFromWord problem
            if (node.uniqueID) {
                if (m_uniqueIds[node.uniqueID]) return;
                else m_uniqueIds[node.uniqueID] = true;
            }

            switch (node.nodeType) {
                case 1: // ELEMENT_NODE
                    //TEKI: Prevents XML closing tags from being displayed, however we do not support XML for the time being
                    if (node.tagName.charAt(0) == '/') return;

                    var name = m_nodeName(node);

                    if ("!" == name)//IE below 6
                    {
                        m_sb.append(node.text);
                        return;
                    }

                    m_sb.append("<" + name);

                    //TEKI - BUG IN IE, does not give width and height attribute values!
                    if (document.all) {
                        if ("img" == name) {
                            var oImg = node.ownerDocument.createElement("IMG");
                            oImg.mergeAttributes(node);
                            if (oImg.width) {
                                // to avoid duplicate width attributes in IE
                                for (var i = 0; i < node.attributes.length; i++) {
                                    if ("width" == node.attributes[i].name) {
                                        node.setAttribute("width", oImg.width);
                                        break;
                                    }
                                }
                            }
                            if (oImg.height) {
                                // to avoid duplicate height attributes in IE
                                for (var i = 0; i < node.attributes.length; i++) {
                                    if ("height" == node.attributes[i].name) {
                                        node.setAttribute("height", oImg.height);
                                        break;
                                    }
                                }
                            }
                        }
                        else if ("area" == name) //RE5-4669 - Image map AREA attributes disappear after switch to WYSIWYG mode
                        {
                            if (node.shape) {
                                m_sb.append(' shape="' + node.shape.toLowerCase() + '"');
                            }
                            if (node.coords) {
                                m_sb.append(' coords="' + node.getAttribute("coords") + '"');
                            }
                            //RE5-6222 - IE adds about:blank
                            if (node.href) {
                                var cleanedHref = node.href.replace("about:blank", "");
                                m_sb.append(' href="' + cleanedHref + '"');
                            }
                            //#49097: ie8 - Remove the attribute so that it does not get parsed in the attributes collection
                            if (node.shape) {
                                node.removeAttribute("shape", 0);
                            }
                            if (node.coords) {
                                node.removeAttribute("coords", 0);
                            }
                            if (node.href) {
                                node.removeAttribute("href", 0);
                            }
                        }
                    }

                    /*
                    Note: serializeHTMLElement used to force ["ektdesignns_bind","ektdesignns_nodetype","ektdesignns_content","class","type","value","selected","checked"]
                    This below handles "value", "checked" and "selected". Attribute/property "class" and "type" may need some work. 
                    The "ektdesignns_" attributes should be set using .setAttribute() and then serialize fine.
                    */
                    var bSkipChecked = false;
                    var bSkipSelected = false;
                    if ("input" == name && "string" == typeof node.value) {
                        node.setAttribute("value", node.value);
                    }
                    if ("boolean" == typeof node.defaultChecked) {
                        if (node.checked) {
                            node.setAttribute("checked", node.checked);
                            // IE8 won't add to the attributes array, so serialize it now
                            m_sb.append(" checked=\"checked\"");
                            bSkipChecked = true;
                        }
                    }
                    if ("boolean" == typeof node.defaultSelected) {
                        if (node.selected) {
                            node.setAttribute("selected", node.selected);
                            // IE8 won't add to the attributes array, so serialize it now
                            m_sb.append(" selected=\"selected\"");
                            bSkipSelected = true;
                        }
                    }

                    var attrs = node.attributes;
                    var numAttrs = attrs.length;
                    for (var i = 0; i < numAttrs; i++) {
                        var attrName = attrs[i].name.toLowerCase();
                        // #44521: IE8 treats "complete" as an attribute.
                        if ("img" == name && "complete" == attrName) continue;
                        if (bSkipChecked && "checked" == attrName) continue;
                        if (bSkipSelected && "selected" == attrName) continue;
                        m_appendAttributeXhtml(attrs[i], node);
                    }

                    switch (name) {
                        case "script":
                            m_sb.append(">");
                            m_sb.append(node.text);
                            m_sb.append("</scr" + "ipt>");
                            break;
                        case "textarea":
                            m_sb.append(">");
                            m_sb.append($ektron.htmlEncodeText(node.value));
                            m_sb.append("</textarea>");
                            break;
                        case "title":
                        case "style":
                        case "comment":
                        case "noscript":
                            m_sb.append(">");
                            m_sb.append(node.innerHTML);
                            m_sb.append("</" + name + ">");
                            break;
                        default:
                            if (node.hasChildNodes() || 2 == Ektron.Xml.htmlTagCount[name]) {
                                m_sb.append(">");

                                // childNodes
                                var cs = node.childNodes;
                                var numChildren = cs.length;
                                for (var i = 0; i < numChildren; i++) {
                                    m_appendNodeXhtml(cs[i]);
                                }

                                if (0 == numChildren && ("p" == name || "td" == name)) {
                                    m_sb.append("&#160;");
                                }

                                m_sb.append("</" + name + ">");
                            }
                            else {
                                m_sb.append(" />");
                            }
                            break;
                    } // switch
                    break;

                case 3: // TEXT_NODE
                    m_sb.append($ektron.htmlEncodeText(node.nodeValue));
                    break;

                case 4: // CDATA_SECTION_NODE
                    m_sb.append("<![CDA" + "TA[\n");
                    m_sb.append(node.nodeValue);
                    m_sb.append("\n]" + "]>");
                    break;

                case 8: // COMMENT_NODE
                    //TEKI: 2 problems with comments in Mozilla - node.text is not defined! also - none of the values returns the value surrounded in <!-- -->

                    var commentValue = node.text;
                    if (!node.text && node.nodeValue) {
                        commentValue = "<!--" + node.nodeValue + "-->";
                    }

                    m_sb.append(commentValue);
                    if (/(^<\?xml)|(^<\!DOCTYPE)/.test(commentValue)) {
                        m_sb.append("\n");
                    }
                    break;
            }
        }

        // optimized for speed
        function m_appendAttributeXhtml(oAttrNode, oElementNode) {
            var name = m_nodeName(oAttrNode);
            switch (name) {
                case "selected":
                    if (oElementNode.selected) {
                        m_sb.append(" selected=\"selected\"");
                    }
                    return;
                case "checked":
                    if (oElementNode.checked) {
                        m_sb.append(" checked=\"checked\"");
                    }
                    return;
                case "disabled":
                    if (oElementNode.disabled) {
                        m_sb.append(" disabled=\"disabled\"");
                    }
                    return;
                case "style":
                    if (!oAttrNode.specified) return;
                    m_sb.append(" style=\"");
                    m_sb.append($ektron.htmlEncode(oElementNode.style.cssText));
                    m_sb.append("\"");
                    return;
                case "name":
                    // IE says that <a name> is not specified, even when it is.
                    if (oElementNode.tagName != "A" || !oElementNode.name) {
                        if (!oAttrNode.specified) return;
                    }
                    break;
                case "type":
                    break;
                case "value":
                    break;
                default:
                    if (!oAttrNode.specified) return;
                    if (/^jquery\d+/i.test(name)) {
                        // discard it. This attribute is added by ektron.js (jQuery) by clone() method.
                        return;
                    }
                    else if (/^\{/.test(name) || /\}$/.test(name)) {
                        // discard it. 
                        // Some Ie version change the 'type="text"' expression into something like
                        // this: {95919BE0-C436-4eab-8083-096E94826667}="input text value"
                        // Source: http://www.developerutility.com/ToolBox.htm and http://www.developerutility.com/JsToolBox.js 
                        // Also #54162, INPUTLOADED_{[class id]}="true"
                        return;
                    }
                    else if (/^propdescname$/i.test(name)) {
                        // #54376 - IE8 with jquery issue. The bookmark name attribute was created as "propdescname" in IE8. Hence, the bookmark link cannot find 
                        // the bookmark to jump to. 
                        name = "name";
                    }
                    break;
            }

            var value = oAttrNode.nodeValue;
            // #38750 - Not able to save the smartform if the alt text is blank.After getting the alt attribute missing 
            if (!value || !isNaN(value)) value = oElementNode.getAttribute(oAttrNode.nodeName); // IE5.x bugs for number values
            if (!value) return;

            m_sb.append(" " + name + "=\"");
            m_sb.append($ektron.htmlEncode(value));
            m_sb.append("\"");
        }

        function m_nodeName(node) {
            if (node.scopeName && node.scopeName != "HTML") // scopeName is IE only
            {
                // do not force to lower case
                return node.scopeName + ":" + node.nodeName; // scopeName is namespace prefix
            }
            else if (node.expando) // expando is IE only
            {
                // do not force to lower case
                return node.nodeName;
            }
            else {
                // .prefix is always null in FF and Safari/Chrome
                // .nodeName for elements is always upper case in FF and Safari/Chrome
                // .nodeName for attributes is always lower case in FF and Safari/Chrome
                // .localName for elements is always lower case in Safari/Chrome
                // force xhtml names to lower case
                return node.nodeName.toLowerCase();
            }
        }
    };


    /* ******************************************* */
    /*                                             */
    /*     parseDate       serializeDate            */
    /*                                             */
    /* ******************************************* */

    Ektron.Xml.isDate = function isDate(xmlDate) {
        return (/^[0-9]{4}\-[0-9]{2}\-[0-9]{2}$/.test(xmlDate));
    };

    Ektron.Xml.parseDate = function parseDate(xmlDate, onexception) {
        if (null == xmlDate || "" == xmlDate) return null;
        // ISO-8601 format: YYYY-MM-DD, but allows text after the date
        try {
            if (!/^[0-9]{4}\-[0-9]{2}\-[0-9]{2}/.test(xmlDate)) throw new RangeError("xmlDate must be in ISO-8601 format YYYY-MM-DD.");
            return new Date(xmlDate.substring(0, 4), xmlDate.substring(5, 7) - 1, xmlDate.substring(8, 10));
        }
        catch (ex) {
            return Ektron.OnException(this, onexception, ex, arguments);
        }
    };
    Ektron.Xml.parseDate.onexception = Ektron.OnException.returnValue(null);

    Ektron.Xml.serializeDate = function serializeDate(date) {
        // Returns ISO-8601 format: YYYY-MM-DD or empty string if not a Date
        if (!(date instanceof Date)) return "";

        var yyyy = date.getFullYear() + "";
        var mm = (date.getMonth() + 1) + "";
        if (1 == mm.length) {
            mm = "0" + mm;
        }
        var dd = date.getDate() + "";
        if (1 == dd.length) {
            dd = "0" + dd;
        }

        return yyyy + "-" + mm + "-" + dd;
    };

    // shared private

    var s_cacheXml = [];
    var s_cacheXslt = [];
    var s_cacheXslProc = [];
    Ektron.Xml.UnitTest_cacheXslProc = s_cacheXslProc;

    function getXmlHttp() {
        try {
            return new XMLHttpRequest(); // Firefox, Opera 8.0+, Safari
        }
        catch (ex) { };
        try {
            return new ActiveXObject("Msxml2.XMLHTTP"); // Internet Explorer
        }
        catch (ex) { };
        try {
            return new ActiveXObject("Microsoft.XMLHTTP"); // Internet Explorer
        }
        catch (ex) {
            throw new ReferenceError("Your browser does not support AJAX!");
        };
    };

    function xsltMustBeUrl(xslt) {
        if ($ektron.browser.safari) {
            // document() does not work in Safari/Chrome
            if (xslt.indexOf("document(") >= 0) return true;
        }
        // document('') does not reference XSLT when XSLT is a string in Mozilla and MSXML
        return (xslt.indexOf("document('')") >= 0 || xslt.indexOf("xsl:include") >= 0 || xslt.indexOf("xsl:import") >= 0);
    };
    Ektron.Xml.UnitTest_xsltMustBeUrl = xsltMustBeUrl;

    function xsltUsesParam(xslt) {
        // The XSLT may rely on parameters supplied by CommonAPI.XSLTransform
        return (xslt.indexOf("xsl:param") >= 0);
    };

    function xsltUsesExtObj(xslt) {
        return (xslt.indexOf("ektron:extension-object") >= 0);
    };

    function isUrl(s) {
        return (s ? (-1 == s.indexOf("<") && -1 == s.indexOf("\n") && /\w[\.\?\/]\w/.test(s)) : false);
    };


    /**
    * ====================================================================
    * About Sarissa: http://dev.abiss.gr/sarissa
    * ====================================================================
    * Sarissa is an ECMAScript library acting as a cross-browser wrapper for native XML APIs.
    * The library supports Gecko based browsers like Mozilla and Firefox,
    * Internet Explorer (5.5+ with MSXML3.0+), Konqueror, Safari and Opera
    * @version 0.9.9.4
    * @author: Copyright 2004-2008 Emmanouil Batsis, mailto: mbatsis at users full stop sourceforge full stop net
    * ====================================================================
    * Licence
    * ====================================================================
    * Sarissa is free software distributed under the GNU GPL version 2 (see <a href="gpl.txt">gpl.txt</a>) or higher, 
    * GNU LGPL version 2.1 (see <a href="lgpl.txt">lgpl.txt</a>) or higher and Apache Software License 2.0 or higher 
    * (see <a href="asl.txt">asl.txt</a>). This means you can choose one of the three and use that if you like. If 
    * you make modifications under the ASL, i would appreciate it if you submitted those.
    * In case your copy of Sarissa does not include the license texts, you may find
    * them online in various formats at <a href="http://www.gnu.org">http://www.gnu.org</a> and 
    * <a href="http://www.apache.org">http://www.apache.org</a>.
    *
    * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY 
    * KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
    * WARRANTIES OF MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE 
    * AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
    * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
    * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
    * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    */
    /**
    * <p>Sarissa is a utility class. Provides "static" methods for DOMDocument, 
    * DOM Node serialization to XML strings and other utility goodies.</p>
    * @constructor
    * @static
    */
    function Sarissa() { }
    Sarissa.VERSION = "0.9.9.4-Ektron";
    Sarissa.PARSED_OK = "Document contains no parsing errors";
    Sarissa.PARSED_EMPTY = "Document is empty";
    Sarissa.PARSED_UNKNOWN_ERROR = "Not well-formed or other error";
    Sarissa.IS_ENABLED_TRANSFORM_NODE = false;
    Sarissa.REMOTE_CALL_FLAG = "gr.abiss.sarissa.REMOTE_CALL_FLAG";
    /** @private */
    Sarissa._lastUniqueSuffix = 0;
    /** @private */
    Sarissa._getUniqueSuffix = function () {
        return Sarissa._lastUniqueSuffix++;
    };
    /** @private */
    Sarissa._SARISSA_IEPREFIX4XSLPARAM = "";
    /** @private */
    Sarissa._SARISSA_HAS_DOM_IMPLEMENTATION = document.implementation && true;
    /** @private */
    Sarissa._SARISSA_HAS_DOM_CREATE_DOCUMENT = Sarissa._SARISSA_HAS_DOM_IMPLEMENTATION && document.implementation.createDocument;
    /** @private */
    Sarissa._SARISSA_HAS_DOM_FEATURE = Sarissa._SARISSA_HAS_DOM_IMPLEMENTATION && document.implementation.hasFeature;
    /** @private */
    Sarissa._SARISSA_IS_MOZ = Sarissa._SARISSA_HAS_DOM_CREATE_DOCUMENT && Sarissa._SARISSA_HAS_DOM_FEATURE;
    /** @private */
    Sarissa._SARISSA_IS_SAFARI = navigator.userAgent.toLowerCase().indexOf("safari") != -1 || navigator.userAgent.toLowerCase().indexOf("konqueror") != -1;
    /** @private */
    Sarissa._SARISSA_IS_SAFARI_OLD = Sarissa._SARISSA_IS_SAFARI && (parseInt((navigator.userAgent.match(/AppleWebKit\/(\d+)/) || {})[1], 10) < 420);
    /** @private */
    Sarissa._SARISSA_IS_IE = document.all && window.ActiveXObject && navigator.userAgent.toLowerCase().indexOf("msie") > -1 && navigator.userAgent.toLowerCase().indexOf("opera") == -1;
    /** @private */
    Sarissa._SARISSA_IS_OPERA = navigator.userAgent.toLowerCase().indexOf("opera") != -1;
    if (!window.Node || !Node.ELEMENT_NODE) {
        Node = { ELEMENT_NODE: 1, ATTRIBUTE_NODE: 2, TEXT_NODE: 3, CDATA_SECTION_NODE: 4, ENTITY_REFERENCE_NODE: 5, ENTITY_NODE: 6, PROCESSING_INSTRUCTION_NODE: 7, COMMENT_NODE: 8, DOCUMENT_NODE: 9, DOCUMENT_TYPE_NODE: 10, DOCUMENT_FRAGMENT_NODE: 11, NOTATION_NODE: 12 };
    }

    //This breaks for(x in o) loops in the old Safari
    if (Sarissa._SARISSA_IS_SAFARI_OLD) {
        HTMLHtmlElement = document.createElement("html").constructor;
        Node = HTMLElement = {};
        HTMLElement.prototype = HTMLHtmlElement.__proto__.__proto__;
        HTMLDocument = Document = document.constructor;
        var x = new DOMParser();
        XMLDocument = x.constructor;
        Element = x.parseFromString("<Single />", "text/xml").documentElement.constructor;
        x = null;
    }
    if (typeof XMLDocument == "undefined" && typeof Document != "undefined") { XMLDocument = Document; }

    // IE initialization
    if (Sarissa._SARISSA_IS_IE) {
        // for XSLT parameter names, prefix needed by IE
        Sarissa._SARISSA_IEPREFIX4XSLPARAM = "xsl:";
        // used to store the most recent ProgID available out of the above
        var _SARISSA_DOM_PROGID = "";
        var _SARISSA_XMLHTTP_PROGID = "";
        var _SARISSA_DOM_XMLWRITER = "";
        /**
        * Called when the sarissa.js file is parsed, to pick most recent
        * ProgIDs for IE, then gets destroyed.
        * @memberOf Sarissa
        * @private
        * @param idList an array of MSXML PROGIDs from which the most recent will be picked for a given object
        * @param enabledList an array of arrays where each array has two items; the index of the PROGID for which a certain feature is enabled
        */
        Sarissa.pickRecentProgID = function (idList) {
            // found progID flag
            var bFound = false, e;
            var o2Store;
            for (var i = 0; i < idList.length && !bFound; i++) {
                try {
                    var oDoc = new ActiveXObject(idList[i]);
                    o2Store = idList[i];
                    bFound = true;
                } catch (objException) {
                    // trap; try next progID
                    e = objException;
                }
            }
            if (!bFound) {
                throw "Could not retrieve a valid progID of Class: " + idList[idList.length - 1] + ". (original exception: " + e + ")";
            }
            idList = null;
            return o2Store;
        };
        // pick best available MSXML progIDs
        _SARISSA_DOM_PROGID = null;
        _SARISSA_THREADEDDOM_PROGID = null;
        _SARISSA_XSLTEMPLATE_PROGID = null;
        _SARISSA_XMLHTTP_PROGID = null;
        // commenting the condition out; we need to redefine XMLHttpRequest 
        // anyway as IE7 hardcodes it to MSXML3.0 causing version problems 
        // between different activex controls 
        //if(!window.XMLHttpRequest){
        /**
        * Emulate XMLHttpRequest
        * @constructor
        */

        XMLHttpRequest = function () {
            if (!_SARISSA_XMLHTTP_PROGID) {
                _SARISSA_XMLHTTP_PROGID = Sarissa.pickRecentProgID(["Msxml2.XMLHTTP.6.0", "MSXML2.XMLHTTP.3.0", "MSXML2.XMLHTTP", "Microsoft.XMLHTTP"]);
            }
            return new ActiveXObject(_SARISSA_XMLHTTP_PROGID);
        };

        // we dont need this anymore
        //============================================
        // Factory methods (IE)
        //============================================
        // see non-IE version
        Sarissa.getDomDocument = function (sUri, sName) {
            if (!_SARISSA_DOM_PROGID) {
                _SARISSA_DOM_PROGID = Sarissa.pickRecentProgID(["Msxml2.DOMDocument.6.0", "Msxml2.DOMDocument.3.0", "MSXML2.DOMDocument", "MSXML.DOMDocument", "Microsoft.XMLDOM"]);
            }
            var oDoc = new ActiveXObject(_SARISSA_DOM_PROGID);
            oDoc.resolveExternals = true; // for MSXML 6.0	Ektron
            // if a root tag name was provided, we need to load it in the DOM object
            if (sName) {
                // create an artificial namespace prefix 
                // or reuse existing prefix if applicable
                var prefix = "";
                if (sUri) {
                    if (sName.indexOf(":") > 1) {
                        prefix = sName.substring(0, sName.indexOf(":"));
                        sName = sName.substring(sName.indexOf(":") + 1);
                    } else {
                        prefix = "a" + Sarissa._getUniqueSuffix();
                    }
                }
                // use namespaces if a namespace URI exists
                if (sUri) {
                    oDoc.loadXML('<' + prefix + ':' + sName + " xmlns:" + prefix + "=\"" + sUri + "\"" + " />");
                } else {
                    oDoc.loadXML('<' + sName + " />");
                }
            }
            return oDoc;
        };

        // Ektron Begin
        Sarissa.getXsltDocument = function (sUri, sName) {
            if (!_SARISSA_THREADEDDOM_PROGID) {
                _SARISSA_THREADEDDOM_PROGID = Sarissa.pickRecentProgID(["MSXML2.FreeThreadedDOMDocument.6.0", "MSXML2.FreeThreadedDOMDocument.4.0", "MSXML2.FreeThreadedDOMDocument.5.0", "MSXML2.FreeThreadedDOMDocument.3.0"]);
            };
            var oDoc = new ActiveXObject(_SARISSA_THREADEDDOM_PROGID);
            Sarissa.setXpathNamespaces(oDoc, "xmlns:xsl='http://www.w3.org/1999/XSL/Transform'");
            oDoc.resolveExternals = true; // MSXML 2.0 and later
            if ("MSXML2.FreeThreadedDOMDocument.6.0" == _SARISSA_THREADEDDOM_PROGID) {
                oDoc.setProperty("AllowDocumentFunction", true); // This property is supported in MSXML 3.0 SP4, 4.0 SP2, 5.0, and 6.0. The default value is true for 3.0, 4.0, and 5.0. The default value is false for 6.0.
                oDoc.setProperty("AllowXsltScript", true); // This property is supported in MSXML 3.0 SP8, 5.0 SP2, and 6.0. The default value is true for 3.0 and 5.0. The default value is false for 6.0.
                oDoc.setProperty("ProhibitDTD", false); // This property is supported in MSXML 3.0 SP5, 4.0 SP3, 5.0 SP2, and 6.0. The default value is false for 3.0, 4.0, and 5.0. The default value is true for 6.0.
            }
            // if a root tag name was provided, we need to load it in the DOM object
            if (sName) {
                // create an artifical namespace prefix 
                // or reuse existing prefix if applicable
                var prefix = "";
                if (sUri) {
                    if (sName.indexOf(":") > 1) {
                        prefix = sName.substring(0, sName.indexOf(":"));
                        sName = sName.substring(sName.indexOf(":") + 1);
                    } else {
                        prefix = "a" + Sarissa._getUniqueSuffix();
                    }
                }
                // use namespaces if a namespace URI exists
                if (sUri) {
                    oDoc.loadXML('<' + prefix + ':' + sName + " xmlns:" + prefix + "=\"" + sUri + "\"" + " />");
                } else {
                    oDoc.loadXML('<' + sName + " />");
                }
            }
            return oDoc;
        };
        // Ektron End

        // see non-IE version   
        Sarissa.getParseErrorText = function (oDoc) {
            var parseErrorText = Sarissa.PARSED_OK;
            if (oDoc && oDoc.parseError && oDoc.parseError.errorCode && oDoc.parseError.errorCode != 0) {
                parseErrorText = "XML Parsing Error: " + oDoc.parseError.reason +
					"\nLocation: " + oDoc.parseError.url +
					"\nLine Number " + oDoc.parseError.line + ", Column " +
					oDoc.parseError.linepos +
					":\n" + oDoc.parseError.srcText +
					"\n";
                for (var i = 0; i < oDoc.parseError.linepos; i++) {
                    parseErrorText += "-";
                }
                parseErrorText += "^\n";
            }
            else if (oDoc.documentElement === null) {
                parseErrorText = Sarissa.PARSED_EMPTY;
            }
            return parseErrorText;
        };
        // see non-IE version
        Sarissa.setXpathNamespaces = function (oDoc, sNsSet) {
            oDoc.setProperty("SelectionLanguage", "XPath");
            oDoc.setProperty("SelectionNamespaces", sNsSet);
        };
        /**
        * A class that reuses the same XSLT stylesheet for multiple transforms.
        * @constructor
        */
        XSLTProcessor = function () {
            if (!_SARISSA_XSLTEMPLATE_PROGID) {
                _SARISSA_XSLTEMPLATE_PROGID = Sarissa.pickRecentProgID(["Msxml2.XSLTemplate.6.0", "MSXML2.XSLTemplate.3.0"]);
            }
            this.template = new ActiveXObject(_SARISSA_XSLTEMPLATE_PROGID);
            this.processor = null;
        };
        /**
        * Imports the given XSLT DOM and compiles it to a reusable transform
        * <b>Note:</b> If the stylesheet was loaded from a URL and contains xsl:import or xsl:include elements,it will be reloaded to resolve those
        * @param {DOMDocument} xslDoc The XSLT DOMDocument to import
        */
        XSLTProcessor.prototype.importStylesheet = function (xslDoc) {
            // Ektron Begin
            // xslDoc MUST be created using getXsltDocument()
            // Ektron End
            xslDoc.setProperty("SelectionLanguage", "XPath");
            xslDoc.setProperty("SelectionNamespaces", "xmlns:xsl='http://www.w3.org/1999/XSL/Transform'");
            // Ektron Begin
            var converted = xslDoc;
            // Ektron End

            var output = converted.selectSingleNode("//xsl:output");
            //this.outputMethod = output ? output.getAttribute("method") : "html";
            if (output) {
                this.outputMethod = output.getAttribute("method");
            }
            else {
                delete this.outputMethod;
            }
            this.template.stylesheet = converted;
            this.processor = this.template.createProcessor();
            // for getParameter and clearParameters
            this.paramsSet = [];
        };

        /**
        * Transform the given XML DOM and return the transformation result as a new DOM document
        * @param {DOMDocument} sourceDoc The XML DOMDocument to transform
        * @return {DOMDocument} The transformation result as a DOM Document
        */
        XSLTProcessor.prototype.transformToDocument = function (sourceDoc) {
            // Ektron begin
            if (!_SARISSA_DOM_PROGID) {
                _SARISSA_DOM_PROGID = Sarissa.pickRecentProgID(["Msxml2.DOMDocument.6.0", "Msxml2.DOMDocument.3.0", "MSXML2.DOMDocument", "MSXML.DOMDocument", "Microsoft.XMLDOM"]);
            }
            // Ektron end
            // fix for bug 1549749
            var outDoc;
            if (_SARISSA_THREADEDDOM_PROGID) {
                this.processor.input = sourceDoc;
                outDoc = new ActiveXObject(_SARISSA_DOM_PROGID);
                this.processor.output = outDoc;
                this.processor.transform();
                return outDoc;
            }
            else {
                if (!_SARISSA_DOM_XMLWRITER) {
                    _SARISSA_DOM_XMLWRITER = Sarissa.pickRecentProgID(["Msxml2.MXXMLWriter.6.0", "Msxml2.MXXMLWriter.3.0", "MSXML2.MXXMLWriter", "MSXML.MXXMLWriter", "Microsoft.XMLDOM"]);
                }
                this.processor.input = sourceDoc;
                outDoc = new ActiveXObject(_SARISSA_DOM_XMLWRITER);
                this.processor.output = outDoc;
                this.processor.transform();
                var oDoc = new ActiveXObject(_SARISSA_DOM_PROGID);
                oDoc.loadXML(outDoc.output + "");
                return oDoc;
            }
        };

        /**
        * Transform the given XML DOM and return the transformation result as a new DOM fragment.
        * <b>Note</b>: The xsl:output method must match the nature of the owner document (XML/HTML).
        * @param {DOMDocument} sourceDoc The XML DOMDocument to transform
        * @param {DOMDocument} ownerDoc The owner of the result fragment
        * @return {DOMDocument} The transformation result as a DOM Document
        */
        XSLTProcessor.prototype.transformToFragment = function (sourceDoc, ownerDoc) {
            // Ektron begin
            if (!_SARISSA_DOM_PROGID) {
                _SARISSA_DOM_PROGID = Sarissa.pickRecentProgID(["Msxml2.DOMDocument.6.0", "Msxml2.DOMDocument.3.0", "MSXML2.DOMDocument", "MSXML.DOMDocument", "Microsoft.XMLDOM"]);
            }
            // Ektron end
            this.processor.input = sourceDoc;
            this.processor.transform();
            var s = this.processor.output;
            var f = ownerDoc.createDocumentFragment();
            var container;
            if (this.outputMethod == 'text') {
                f.appendChild(ownerDoc.createTextNode(s));
            } else if (ownerDoc.body && ownerDoc.body.innerHTML) {
                container = ownerDoc.createElement('div');
                container.innerHTML = s;
                while (container.hasChildNodes()) {
                    f.appendChild(container.firstChild);
                }
            }
            else {
                var oDoc = new ActiveXObject(_SARISSA_DOM_PROGID);
                if (s.substring(0, 5) == '<?xml') {
                    s = s.substring(s.indexOf('?>') + 2);
                }
                var xml = ''.concat('<my>', s, '</my>');
                oDoc.loadXML(xml);
                container = oDoc.documentElement;
                while (container.hasChildNodes()) {
                    f.appendChild(container.firstChild);
                }
            }
            return f;
        };

        /**
        * Set global XSLT parameter of the imported stylesheet
        * @param {String} nsURI The parameter namespace URI
        * @param {String} name The parameter base name
        * @param {String} value The new parameter value
        */
        XSLTProcessor.prototype.setParameter = function (nsURI, name, value) {
            // make value a zero length string if null to allow clearing
            value = value ? value : "";
            // nsURI is optional but cannot be null
            if (nsURI) {
                this.processor.addParameter(name, value, nsURI);
            } else {
                this.processor.addParameter(name, value);
            }
            // update updated params for getParameter
            nsURI = "" + (nsURI || "");
            if (!this.paramsSet[nsURI]) {
                this.paramsSet[nsURI] = [];
            }
            this.paramsSet[nsURI][name] = value;
        };
        /**
        * Gets a parameter if previously set by setParameter. Returns null
        * otherwise
        * @param {String} name The parameter base name
        * @param {String} value The new parameter value
        * @return {String} The parameter value if reviously set by setParameter, null otherwise
        */
        XSLTProcessor.prototype.getParameter = function (nsURI, name) {
            nsURI = "" + (nsURI || "");
            if (this.paramsSet[nsURI] && this.paramsSet[nsURI][name]) {
                return this.paramsSet[nsURI][name];
            } else {
                return null;
            }
        };

        /**
        * Clear parameters (set them to default values as defined in the stylesheet itself)
        */
        XSLTProcessor.prototype.clearParameters = function () {
            for (var nsURI in this.paramsSet) {
                for (var name in this.paramsSet[nsURI]) {
                    if (nsURI != "") {
                        this.processor.addParameter(name, "", nsURI);
                    } else {
                        this.processor.addParameter(name, "");
                    }
                }
            }
            this.paramsSet = [];
        };
    } else { /* end IE initialization, try to deal with real browsers now ;-) */
        if (Sarissa._SARISSA_HAS_DOM_CREATE_DOCUMENT) {
            /**
            * <p>Ensures the document was loaded correctly, otherwise sets the
            * parseError to -1 to indicate something went wrong. Internal use</p>
            * @private
            */
            Sarissa.__handleLoad__ = function (oDoc) {
                Sarissa.__setReadyState__(oDoc, 4);
            };
            /**
            * <p>Attached by an event handler to the load event. Internal use.</p>
            * @private
            */
            _sarissa_XMLDocument_onload = function () {
                Sarissa.__handleLoad__(this);
            };
            /**
            * <p>Sets the readyState property of the given DOM Document object.
            * Internal use.</p>
            * @memberOf Sarissa
            * @private
            * @param oDoc the DOM Document object to fire the
            *          readystatechange event
            * @param iReadyState the number to change the readystate property to
            */
            Sarissa.__setReadyState__ = function (oDoc, iReadyState) {
                oDoc.readyState = iReadyState;
                oDoc.readystate = iReadyState;
                if (oDoc.onreadystatechange != null && typeof oDoc.onreadystatechange == "function") {
                    oDoc.onreadystatechange();
                }
            };

            Sarissa.getDomDocument = function (sUri, sName) {
                var oDoc = document.implementation.createDocument(sUri ? sUri : null, sName ? sName : null, null);
                if (!oDoc.onreadystatechange) {

                    /**
                    * <p>Emulate IE's onreadystatechange attribute</p>
                    */
                    oDoc.onreadystatechange = null;
                }
                if (!oDoc.readyState) {
                    /**
                    * <p>Emulates IE's readyState property, which always gives an integer from 0 to 4:</p>
                    * <ul><li>1 == LOADING,</li>
                    * <li>2 == LOADED,</li>
                    * <li>3 == INTERACTIVE,</li>
                    * <li>4 == COMPLETED</li></ul>
                    */
                    oDoc.readyState = 0;
                }
                oDoc.addEventListener("load", _sarissa_XMLDocument_onload, false);
                return oDoc;
            };
            Sarissa.getXsltDocument = Sarissa.getDomDocument; // Ektron
            if (window.XMLDocument) {
                // do nothing
            } // TODO: check if the new document has content before trying to copynodes, check  for error handling in DOM 3 LS
            else if (Sarissa._SARISSA_HAS_DOM_FEATURE && window.Document && !Document.prototype.load && document.implementation.hasFeature('LS', '3.0')) {
                //Opera 9 may get the XPath branch which gives creates XMLDocument, therefore it doesn't reach here which is good
                /**
                * <p>Factory method to obtain a new DOM Document object</p>
                * @memberOf Sarissa
                * @param {String} sUri the namespace of the root node (if any)
                * @param {String} sUri the local name of the root node (if any)
                * @returns {DOMDOcument} a new DOM Document
                */
                Sarissa.getDomDocument = function (sUri, sName) {
                    var oDoc = document.implementation.createDocument(sUri ? sUri : null, sName ? sName : null, null);
                    return oDoc;
                };
            }
            else {
                Sarissa.getDomDocument = function (sUri, sName) {
                    var oDoc = document.implementation.createDocument(sUri ? sUri : null, sName ? sName : null, null);
                    // looks like safari does not create the root element for some unknown reason
                    if (oDoc && (sUri || sName) && !oDoc.documentElement) {
                        oDoc.appendChild(oDoc.createElementNS(sUri, sName));
                    }
                    return oDoc;
                };
                Sarissa.getXsltDocument = Sarissa.getDomDocument; // Ektron
            }
        } //if(Sarissa._SARISSA_HAS_DOM_CREATE_DOCUMENT)
    }
    //==========================================
    // Common stuff
    //==========================================
    if (!window.DOMParser) {
        if (Sarissa._SARISSA_IS_SAFARI) {
            /**
            * DOMParser is a utility class, used to construct DOMDocuments from XML strings
            * @constructor
            */
            DOMParser = function () { };
            /** 
            * Construct a new DOM Document from the given XMLstring
            * @param {String} sXml the given XML string
            * @param {String} contentType the content type of the document the given string represents (one of text/xml, application/xml, application/xhtml+xml). 
            * @return {DOMDocument} a new DOM Document from the given XML string
            */
            DOMParser.prototype.parseFromString = function (sXml, contentType) {
                var xmlhttp = new XMLHttpRequest();
                xmlhttp.open("GET", "data:text/xml;charset=utf-8," + encodeURIComponent(sXml), false);
                xmlhttp.send(null);
                return xmlhttp.responseXML;
            };
        } else if (Sarissa.getDomDocument && Sarissa.getDomDocument() && Sarissa.getDomDocument(null, "bar").xml) {
            DOMParser = function () { };
            DOMParser.prototype.parseFromString = function (sXml, contentType) {
                var doc = Sarissa.getDomDocument();
                doc.loadXML(sXml);
                return doc;
            };
        }
    }

    if ((typeof (document.importNode) == "undefined") && Sarissa._SARISSA_IS_IE) {
        try {
            /**
            * Implementation of importNode for the context window document in IE.
            * If <code>oNode</code> is a TextNode, <code>bChildren</code> is ignored.
            * @param {DOMNode} oNode the Node to import
            * @param {boolean} bChildren whether to include the children of oNode
            * @returns the imported node for further use
            */
            document.importNode = function (oNode, bChildren) {
                var tmp;
                if (oNode.nodeName == '#text') {
                    return document.createTextNode(oNode.data);
                }
                else {
                    if (oNode.nodeName == "tbody" || oNode.nodeName == "tr") {
                        tmp = document.createElement("table");
                    }
                    else if (oNode.nodeName == "td") {
                        tmp = document.createElement("tr");
                    }
                    else if (oNode.nodeName == "option") {
                        tmp = document.createElement("select");
                    }
                    else {
                        tmp = document.createElement("div");
                    }
                    if (bChildren) {
                        tmp.innerHTML = oNode.xml ? oNode.xml : oNode.outerHTML;
                    } else {
                        tmp.innerHTML = oNode.xml ? oNode.cloneNode(false).xml : oNode.cloneNode(false).outerHTML;
                    }
                    return tmp.getElementsByTagName("*")[0];
                }
            };
        } catch (e) { }
    }
    if (!Sarissa.getParseErrorText) {
        /**
        * <p>Returns a human readable description of the parsing error. Usefull
        * for debugging. Tip: append the returned error string in a &lt;pre&gt;
        * element if you want to render it.</p>
        * <p>Many thanks to Christian Stocker for the initial patch.</p>
        * @memberOf Sarissa
        * @param {DOMDocument} oDoc The target DOM document
        * @returns {String} The parsing error description of the target Document in
        *          human readable form (preformated text)
        */
        Sarissa.getParseErrorText = function (oDoc) {
            var parseErrorText = Sarissa.PARSED_OK;
            if ((!oDoc) || (!oDoc.documentElement)) {
                parseErrorText = Sarissa.PARSED_EMPTY;
            } else if (oDoc.documentElement.tagName == "parsererror") {
                parseErrorText = oDoc.documentElement.firstChild.data;
                parseErrorText += "\n" + oDoc.documentElement.firstChild.nextSibling.firstChild.data;
            } else if (oDoc.getElementsByTagName("parsererror").length > 0) {
                var parsererror = oDoc.getElementsByTagName("parsererror")[0];
                parseErrorText = Sarissa.getText(parsererror, true) + "\n";
            } else if (oDoc.parseError && oDoc.parseError.errorCode != 0) {
                parseErrorText = Sarissa.PARSED_UNKNOWN_ERROR;
            }
            return parseErrorText;
        };
    }
    /**
    * Get a string with the concatenated values of all string nodes under the given node
    * @param {DOMNode} oNode the given DOM node
    * @param {boolean} deep whether to recursively scan the children nodes of the given node for text as well. Default is <code>false</code>
    * @memberOf Sarissa
    */
    Sarissa.getText = function (oNode, deep) {
        var s = "";
        var nodes = oNode.childNodes;
        for (var i = 0; i < nodes.length; i++) {
            var node = nodes[i];
            var nodeType = node.nodeType;
            if (nodeType == Node.TEXT_NODE || nodeType == Node.CDATA_SECTION_NODE) {
                s += node.data;
            } else if (deep === true && (nodeType == Node.ELEMENT_NODE || nodeType == Node.DOCUMENT_NODE || nodeType == Node.DOCUMENT_FRAGMENT_NODE)) {
                s += Sarissa.getText(node, true);
            }
        }
        return s;
    };
    if (!window.XMLSerializer && Sarissa.getDomDocument && Sarissa.getDomDocument("", "foo", null).xml) {
        /**
        * Utility class to serialize DOM Node objects to XML strings
        * @constructor
        */
        XMLSerializer = function () { };
    }
    if (!window.XMLSerializer.serializeToString && Sarissa.getDomDocument && Sarissa.getDomDocument("", "foo", null).xml) {
        /**
        * Serialize the given DOM Node to an XML string
        * @param {DOMNode} oNode the DOM Node to serialize
        */
        XMLSerializer.prototype.serializeToString = function (oNode) {
            return oNode.xml;
        };
    }

    /**
    * Strips tags from the given markup string. If the given string is 
    * <code>undefined</code>, <code>null</code> or empty, it is returned as is. 
    * @memberOf Sarissa
    * @param {String} s the string to strip the tags from
    */
    Sarissa.stripTags = function (s) {
        return s ? s.replace(/<[^>]+>/g, "") : s;
    };
    /**
    * <p>Deletes all child nodes of the given node</p>
    * @memberOf Sarissa
    * @param {DOMNode} oNode the Node to empty
    */
    Sarissa.clearChildNodes = function (oNode) {
        // need to check for firstChild due to opera 8 bug with hasChildNodes
        while (oNode.firstChild) {
            oNode.removeChild(oNode.firstChild);
        }
    };
    /**
    * <p> Copies the childNodes of nodeFrom to nodeTo</p>
    * <p> <b>Note:</b> The second object's original content is deleted before 
    * the copy operation, unless you supply a true third parameter</p>
    * @memberOf Sarissa
    * @param {DOMNode} nodeFrom the Node to copy the childNodes from
    * @param {DOMNode} nodeTo the Node to copy the childNodes to
    * @param {boolean} bPreserveExisting whether to preserve the original content of nodeTo, default is false
    */
    Sarissa.copyChildNodes = function (nodeFrom, nodeTo, bPreserveExisting) {
        if (Sarissa._SARISSA_IS_SAFARI && nodeTo.nodeType == Node.DOCUMENT_NODE) { // SAFARI_OLD ??
            nodeTo = nodeTo.documentElement; //Apparently there's a bug in safari where you can't appendChild to a document node
        }

        if ((!nodeFrom) || (!nodeTo)) {
            throw "Both source and destination nodes must be provided";
        }
        if (!bPreserveExisting) {
            Sarissa.clearChildNodes(nodeTo);
        }
        var ownerDoc = nodeTo.nodeType == Node.DOCUMENT_NODE ? nodeTo : nodeTo.ownerDocument;
        var nodes = nodeFrom.childNodes;
        var i;
        if (typeof (ownerDoc.importNode) != "undefined") {
            for (i = 0; i < nodes.length; i++) {
                nodeTo.appendChild(ownerDoc.importNode(nodes[i], true));
            }
        } else {
            for (i = 0; i < nodes.length; i++) {
                nodeTo.appendChild(nodes[i].cloneNode(true));
            }
        }
    };

    /**
    * <p> Moves the childNodes of nodeFrom to nodeTo</p>
    * <p> <b>Note:</b> The second object's original content is deleted before 
    * the move operation, unless you supply a true third parameter</p>
    * @memberOf Sarissa
    * @param {DOMNode} nodeFrom the Node to copy the childNodes from
    * @param {DOMNode} nodeTo the Node to copy the childNodes to
    * @param {boolean} bPreserveExisting whether to preserve the original content of nodeTo, default is
    */
    Sarissa.moveChildNodes = function (nodeFrom, nodeTo, bPreserveExisting) {
        if ((!nodeFrom) || (!nodeTo)) {
            throw "Both source and destination nodes must be provided";
        }
        if (!bPreserveExisting) {
            Sarissa.clearChildNodes(nodeTo);
        }
        var nodes = nodeFrom.childNodes;
        // if within the same doc, just move, else copy and delete
        if (nodeFrom.ownerDocument == nodeTo.ownerDocument) {
            while (nodeFrom.firstChild) {
                nodeTo.appendChild(nodeFrom.firstChild);
            }
        } else {
            var ownerDoc = nodeTo.nodeType == Node.DOCUMENT_NODE ? nodeTo : nodeTo.ownerDocument;
            var i;
            if (typeof (ownerDoc.importNode) != "undefined") {
                for (i = 0; i < nodes.length; i++) {
                    nodeTo.appendChild(ownerDoc.importNode(nodes[i], true));
                }
            } else {
                for (i = 0; i < nodes.length; i++) {
                    nodeTo.appendChild(nodes[i].cloneNode(true));
                }
            }
            Sarissa.clearChildNodes(nodeFrom);
        }
    };

    /** 
    * <p>Serialize any <strong>non</strong> DOM object to an XML string. All properties are serialized using the property name
    * as the XML element name. Array elements are rendered as <code>array-item</code> elements, 
    * using their index/key as the value of the <code>key</code> attribute.</p>
    * @memberOf Sarissa
    * @param {Object} anyObject the object to serialize
    * @param {String} objectName a name for that object, to be used as the root element name
    * @return {String} the XML serialization of the given object as a string
    */
    Sarissa.xmlize = function (anyObject, objectName, indentSpace) {
        indentSpace = indentSpace ? indentSpace : '';
        var s = indentSpace + '<' + objectName + '>';
        var isLeaf = false;
        if (!(anyObject instanceof Object) || anyObject instanceof Number || anyObject instanceof String || anyObject instanceof Boolean || anyObject instanceof Date) {
            s += Sarissa.escape("" + anyObject);
            isLeaf = true;
        } else {
            s += "\n";
            var isArrayItem = anyObject instanceof Array;
            for (var name in anyObject) {
                s += Sarissa.xmlize(anyObject[name], (isArrayItem ? "array-item key=\"" + name + "\"" : name), indentSpace + "   ");
            }
            s += indentSpace;
        }
        return (s += (objectName.indexOf(' ') != -1 ? "</array-item>\n" : "</" + objectName + ">\n"));
    };

    /** 
    * Escape the given string chacters that correspond to the five predefined XML entities
    * @memberOf Sarissa
    * @param {String} sXml the string to escape
    */
    Sarissa.escape = function (sXml) {
        return sXml.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&apos;");
    };

    /** 
    * Unescape the given string. This turns the occurences of the predefined XML 
    * entities to become the characters they represent correspond to the five predefined XML entities
    * @memberOf Sarissa
    * @param  {String}sXml the string to unescape
    */
    Sarissa.unescape = function (sXml) {
        return sXml.replace(/&apos;/g, "'").replace(/&quot;/g, "\"").replace(/&gt;/g, ">").replace(/&lt;/g, "<").replace(/&amp;/g, "&");
    };

    /*
    Ektron
    Removed:
    Sarissa.updateCursor
    Sarissa.updateContentFromURI
    Sarissa.updateContentFromNode
    Sarissa.formToQueryString
    Sarissa.updateContentFromForm
    */
    //   EOF

    /**
    * ====================================================================
    * About
    * ====================================================================
    * Sarissa cross browser XML library - IE XPath Emulation 
    * @version 0.9.9.4
    * @author: Copyright 2004-2007 Emmanouil Batsis, mailto: mbatsis at users full stop sourceforge full stop net
    *
    * This script emulates Internet Explorer's selectNodes and selectSingleNode
    * for Mozilla. Associating namespace prefixes with URIs for your XPath queries
    * is easy with IE's setProperty. 
    * USers may also map a namespace prefix to a default (unprefixed) namespace in the
    * source document with Sarissa.setXpathNamespaces
    *
    * ====================================================================
    * Licence
    * ====================================================================
    * Sarissa is free software distributed under the GNU GPL version 2 (see <a href="gpl.txt">gpl.txt</a>) or higher, 
    * GNU LGPL version 2.1 (see <a href="lgpl.txt">lgpl.txt</a>) or higher and Apache Software License 2.0 or higher 
    * (see <a href="asl.txt">asl.txt</a>). This means you can choose one of the three and use that if you like. If 
    * you make modifications under the ASL, i would appreciate it if you submitted those.
    * In case your copy of Sarissa does not include the license texts, you may find
    * them online in various formats at <a href="http://www.gnu.org">http://www.gnu.org</a> and 
    * <a href="http://www.apache.org">http://www.apache.org</a>.
    *
    * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY 
    * KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
    * WARRANTIES OF MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE 
    * AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
    * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
    * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
    * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    */
    if (Sarissa._SARISSA_HAS_DOM_FEATURE && document.implementation.hasFeature("XPath", "3.0")) {
        /**
        * <p>SarissaNodeList behaves as a NodeList but is only used as a result to <code>selectNodes</code>,
        * so it also has some properties IEs proprietery object features.</p>
        * @private
        * @constructor
        * @argument i the (initial) list size
        */
        SarissaNodeList = function (i) {
            this.length = i;
        };
        /** 
        * <p>Set an Array as the prototype object</p> 
        * @private
        */
        SarissaNodeList.prototype = [];
        /** 
        * <p>Inherit the Array constructor </p> 
        * @private
        */
        SarissaNodeList.prototype.constructor = Array;
        /**
        * <p>Returns the node at the specified index or null if the given index
        * is greater than the list size or less than zero </p>
        * <p><b>Note</b> that in ECMAScript you can also use the square-bracket
        * array notation instead of calling <code>item</code>
        * @argument i the index of the member to return
        * @returns the member corresponding to the given index
        * @private
        */
        SarissaNodeList.prototype.item = function (i) {
            return (i < 0 || i >= this.length) ? null : this[i];
        };
        /**
        * <p>Emulate IE's expr property
        * (Here the SarissaNodeList object is given as the result of selectNodes).</p>
        * @returns the XPath expression passed to selectNodes that resulted in
        *          this SarissaNodeList
        * @private
        */
        SarissaNodeList.prototype.expr = "";
        /** dummy, used to accept IE's stuff without throwing errors */
        if (window.XMLDocument && (!XMLDocument.prototype.setProperty)) {
            XMLDocument.prototype.setProperty = function (x, y) { };
        }
        /**
        * <p>Programmatically control namespace URI/prefix mappings for XPath
        * queries.</p>
        * <p>This method comes especially handy when used to apply XPath queries
        * on XML documents with a default namespace, as there is no other way
        * of mapping that to a prefix.</p>
        * <p>Using no namespace prefix in DOM Level 3 XPath queries, implies you
        * are looking for elements in the null namespace. If you need to look
        * for nodes in the default namespace, you need to map a prefix to it
        * first like:</p>
        * <pre>Sarissa.setXpathNamespaces(oDoc, "xmlns:myprefix'http://mynsURI'");</pre>
        * <p><b>Note 1 </b>: Use this method only if the source document features
        * a default namespace (without a prefix), otherwise just use IE's setProperty
        * (moz will rezolve non-default namespaces by itself). You will need to map that
        * namespace to a prefix for queries to work.</p>
        * <p><b>Note 2 </b>: This method calls IE's setProperty method to set the
        * appropriate namespace-prefix mappings, so you dont have to do that.</p>
        * @param oDoc The target XMLDocument to set the namespace mappings for.
        * @param sNsSet A whilespace-seperated list of namespace declarations as
        *            those would appear in an XML document. E.g.:
        *            <code>&quot;xmlns:xhtml=&apos;http://www.w3.org/1999/xhtml&apos;
        * xmlns:&apos;http://www.w3.org/1999/XSL/Transform&apos;&quot;</code>
        * @throws An error if the format of the given namespace declarations is bad.
        */
        Sarissa.setXpathNamespaces = function (oDoc, sNsSet) {
            //oDoc._sarissa_setXpathNamespaces(sNsSet);
            oDoc._sarissa_useCustomResolver = true;
            var namespaces = sNsSet.indexOf(" ") > -1 ? sNsSet.split(" ") : [sNsSet];
            oDoc._sarissa_xpathNamespaces = [];
            for (var i = 0; i < namespaces.length; i++) {
                var ns = namespaces[i];
                var colonPos = ns.indexOf(":");
                var assignPos = ns.indexOf("=");
                if (colonPos > 0 && assignPos > colonPos + 1) {
                    var prefix = ns.substring(colonPos + 1, assignPos);
                    var uri = ns.substring(assignPos + 2, ns.length - 1);
                    oDoc._sarissa_xpathNamespaces[prefix] = uri;
                } else {
                    throw "Bad format on namespace declaration(s) given";
                }
            }
        };
        /**
        * @private Flag to control whether a custom namespace resolver should
        *          be used, set to true by Sarissa.setXpathNamespaces
        */
        XMLDocument.prototype._sarissa_useCustomResolver = false;
        /** @private */
        XMLDocument.prototype._sarissa_xpathNamespaces = [];
        /**
        * <p>Extends the XMLDocument to emulate IE's selectNodes.</p>
        * @argument sExpr the XPath expression to use
        * @argument contextNode this is for internal use only by the same
        *           method when called on Elements
        * @returns the result of the XPath search as a SarissaNodeList
        * @throws An error if no namespace URI is found for the given prefix.
        */
        XMLDocument.prototype.selectNodes = function (sExpr, contextNode, returnSingle) {
            var nsDoc = this;
            var nsresolver;
            if (this._sarissa_useCustomResolver) {
                nsresolver = function (prefix) {
                    var s = nsDoc._sarissa_xpathNamespaces[prefix];
                    if (s) {
                        return s;
                    }
                    else {
                        throw "No namespace URI found for prefix: '" + prefix + "'";
                    }
                };
            }
            else {
                nsresolver = this.createNSResolver(this.documentElement);
            }
            var result = null;
            if (!returnSingle) {
                var oResult = this.evaluate(sExpr,
					(contextNode ? contextNode : this),
					nsresolver,
					XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
                var nodeList = new SarissaNodeList(oResult.snapshotLength);
                nodeList.expr = sExpr;
                for (var i = 0; i < nodeList.length; i++) {
                    nodeList[i] = oResult.snapshotItem(i);
                }
                result = nodeList;
            }
            else {
                result = this.evaluate(sExpr,
					(contextNode ? contextNode : this),
					nsresolver,
					XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
            }
            return result;
        };
        /**
        * <p>Extends the Element to emulate IE's selectNodes</p>
        * @argument sExpr the XPath expression to use
        * @returns the result of the XPath search as an (Sarissa)NodeList
        * @throws An
        *             error if invoked on an HTML Element as this is only be
        *             available to XML Elements.
        */
        Element.prototype.selectNodes = function (sExpr) {
            var doc = this.ownerDocument;
            if (doc.selectNodes) {
                return doc.selectNodes(sExpr, this);
            }
            else {
                throw "Method selectNodes is only supported by XML Elements";
            }
        };
        /**
        * <p>Extends the XMLDocument to emulate IE's selectSingleNode.</p>
        * @argument sExpr the XPath expression to use
        * @argument contextNode this is for internal use only by the same
        *           method when called on Elements
        * @returns the result of the XPath search as an (Sarissa)NodeList
        */
        XMLDocument.prototype.selectSingleNode = function (sExpr, contextNode) {
            var ctx = contextNode ? contextNode : null;
            return this.selectNodes(sExpr, ctx, true);
        };
        /**
        * <p>Extends the Element to emulate IE's selectSingleNode.</p>
        * @argument sExpr the XPath expression to use
        * @returns the result of the XPath search as an (Sarissa)NodeList
        * @throws An error if invoked on an HTML Element as this is only be
        *             available to XML Elements.
        */
        Element.prototype.selectSingleNode = function (sExpr) {
            var doc = this.ownerDocument;
            if (doc.selectSingleNode) {
                return doc.selectSingleNode(sExpr, this);
            }
            else {
                throw "Method selectNodes is only supported by XML Elements";
            }
        };
        Sarissa.IS_ENABLED_SELECT_NODES = true;
    }

    Ektron.Xml.UnitTest_Sarissa = Sarissa;

})();    // Ektron.Xml
