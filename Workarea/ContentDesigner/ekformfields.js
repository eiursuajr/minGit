var theEditor = null;
var localization = null;
if (window.parent && window.parent.theEditor)
{
	theEditor = window.parent.theEditor;
    localization = theEditor.Localization; // for the colorpicker
    if (!Ektron.ContentDesigner) Ektron.ContentDesigner = window.parent.Ektron.ContentDesigner;
    if (!Ektron.SmartForm) Ektron.SmartForm = window.parent.theEditor.EkScriptWindow.Ektron.SmartForm;
}
else if (window.opener && window.opener.theEditor)
{
	theEditor = window.opener.theEditor;
    localization = theEditor.Localization; // for safari colorpicker
    if (!Ektron.ContentDesigner) Ektron.ContentDesigner = window.opener.Ektron.ContentDesigner;
    if (!Ektron.SmartForm) Ektron.SmartForm = window.opener.theEditor.EkScriptWindow.Ektron.SmartForm;
}

function EkFormFields()
{
    this.lFieldNameLimits = 45;
    this.strNodeType = "element";
    this.bNameSerializable = false; //TODO: for formdesign
    this.ContainerStyle = "";
    this.strNoLngAvail = theEditor.GetLocalizedString("sNoLongerAvail", "(No longer available)");
 
    this.FixId = EkFormFields_FixId;
    this.findLabelElement = EkFormFields_findLabelElement;
    this.isDDFieldElement = EkFormFields_isDDFieldElement;
    
    this.promptOnValidateAction = EkFormFields_PromptOnValidateAction;
    this.getVariableNames = EkFormFields_GetVariableNames;   
    this.getResourceFieldDisplayValue = EkFormFields_GetResourceFieldDisplayValue;
}

function EkFormFields_isDDFieldElement(oElem)
{
	if (!oElem) return false;
	if (!oElem.getAttribute) return false;
	var name = oElem.getAttribute("ektdesignns_name");
	if ("string" == typeof name && name.length > 0) return true;
	var bind = oElem.getAttribute("ektdesignns_bind");
	if ("string" == typeof bind && bind.length > 0) return true;
	return false;
}

function EkFormFields_findLabelElement(id, doc)
{
	doc = (doc ? doc : document);
	var oElem = null;
	var aryElems = doc.body.getElementsByTagName("label");
	for (var i = 0; i < aryElems.length; i++)
	{
		if (aryElems[i].htmlFor == id)
		{
			oElem = aryElems[i];
			break;
		}
	}
	return oElem;
}

// http://www.w3.org/TR/xmlschema-2/#NCName
// NCName ::=  (Letter | '_') (NCNameChar)*
// NCNameChar ::=  Letter | Digit | '.' | '-' | '_' | CombiningChar | Extender
Ektron.RegExp.CharacterClass.Letter = "[\x41-\x5A\x61-\x7A\xC0-\xD6\xD8-\xF6\xF8-\xFF\u0100-\u0131\u0134-\u013E\u0141-\u0148\u014A-\u017E\u0180-\u01C3\u01CD-\u01F0\u01F4-\u01F5\u01FA-\u0217\u0250-\u02A8\u02BB-\u02C1\u0386\u0388-\u038A\u038C\u038E-\u03A1\u03A3-\u03CE\u03D0-\u03D6\u03DA\u03DC\u03DE\u03E0\u03E2-\u03F3\u0401-\u040C\u040E-\u044F\u0451-\u045C\u045E-\u0481\u0490-\u04C4\u04C7-\u04C8\u04CB-\u04CC\u04D0-\u04EB\u04EE-\u04F5\u04F8-\u04F9\u0531-\u0556\u0559\u0561-\u0586\u05D0-\u05EA\u05F0-\u05F2\u0621-\u063A\u0641-\u064A\u0671-\u06B7\u06BA-\u06BE\u06C0-\u06CE\u06D0-\u06D3\u06D5\u06E5-\u06E6\u0905-\u0939\u093D\u0958-\u0961\u0985-\u098C\u098F-\u0990\u0993-\u09A8\u09AA-\u09B0\u09B2\u09B6-\u09B9\u09DC-\u09DD\u09DF-\u09E1\u09F0-\u09F1\u0A05-\u0A0A\u0A0F-\u0A10\u0A13-\u0A28\u0A2A-\u0A30\u0A32-\u0A33\u0A35-\u0A36\u0A38-\u0A39\u0A59-\u0A5C\u0A5E\u0A72-\u0A74\u0A85-\u0A8B\u0A8D\u0A8F-\u0A91\u0A93-\u0AA8\u0AAA-\u0AB0\u0AB2-\u0AB3\u0AB5-\u0AB9\u0ABD\u0AE0\u0B05-\u0B0C\u0B0F-\u0B10\u0B13-\u0B28\u0B2A-\u0B30\u0B32-\u0B33\u0B36-\u0B39\u0B3D\u0B5C-\u0B5D\u0B5F-\u0B61\u0B85-\u0B8A\u0B8E-\u0B90\u0B92-\u0B95\u0B99-\u0B9A\u0B9C\u0B9E-\u0B9F\u0BA3-\u0BA4\u0BA8-\u0BAA\u0BAE-\u0BB5\u0BB7-\u0BB9\u0C05-\u0C0C\u0C0E-\u0C10\u0C12-\u0C28\u0C2A-\u0C33\u0C35-\u0C39\u0C60-\u0C61\u0C85-\u0C8C\u0C8E-\u0C90\u0C92-\u0CA8\u0CAA-\u0CB3\u0CB5-\u0CB9\u0CDE\u0CE0-\u0CE1\u0D05-\u0D0C\u0D0E-\u0D10\u0D12-\u0D28\u0D2A-\u0D39\u0D60-\u0D61\u0E01-\u0E2E\u0E30\u0E32-\u0E33\u0E40-\u0E45\u0E81-\u0E82\u0E84\u0E87-\u0E88\u0E8A\u0E8D\u0E94-\u0E97\u0E99-\u0E9F\u0EA1-\u0EA3\u0EA5\u0EA7\u0EAA-\u0EAB\u0EAD-\u0EAE\u0EB0\u0EB2-\u0EB3\u0EBD\u0EC0-\u0EC4\u0F40-\u0F47\u0F49-\u0F69\u10A0-\u10C5\u10D0-\u10F6\u1100\u1102-\u1103\u1105-\u1107\u1109\u110B-\u110C\u110E-\u1112\u113C\u113E\u1140\u114C\u114E\u1150\u1154-\u1155\u1159\u115F-\u1161\u1163\u1165\u1167\u1169\u116D-\u116E\u1172-\u1173\u1175\u119E\u11A8\u11AB\u11AE-\u11AF\u11B7-\u11B8\u11BA\u11BC-\u11C2\u11EB\u11F0\u11F9\u1E00-\u1E9B\u1EA0-\u1EF9\u1F00-\u1F15\u1F18-\u1F1D\u1F20-\u1F45\u1F48-\u1F4D\u1F50-\u1F57\u1F59\u1F5B\u1F5D\u1F5F-\u1F7D\u1F80-\u1FB4\u1FB6-\u1FBC\u1FBE\u1FC2-\u1FC4\u1FC6-\u1FCC\u1FD0-\u1FD3\u1FD6-\u1FDB\u1FE0-\u1FEC\u1FF2-\u1FF4\u1FF6-\u1FFC\u2126\u212A-\u212B\u212E\u2180-\u2182\u3041-\u3094\u30A1-\u30FA\u3105-\u312C\uAC00-\uD7A3\u4E00-\u9FA5\u3007\u3021-\u3029]";
Ektron.RegExp.CharacterClass.Digit = "[\x30-\x39\u0660-\u0669\u06F0-\u06F9\u0966-\u096F\u09E6-\u09EF\u0A66-\u0A6F\u0AE6-\u0AEF\u0B66-\u0B6F\u0BE7-\u0BEF\u0C66-\u0C6F\u0CE6-\u0CEF\u0D66-\u0D6F\u0E50-\u0E59\u0ED0-\u0ED9\u0F20-\u0F29]";
Ektron.RegExp.CharacterClass.CombiningChar = "[\u0300-\u0345\u0360-\u0361\u0483-\u0486\u0591-\u05A1\u05A3-\u05B9\u05BB-\u05BD\u05BF\u05C1-\u05C2\u05C4\u064B-\u0652\u0670\u06D6-\u06DC\u06DD-\u06DF\u06E0-\u06E4\u06E7-\u06E8\u06EA-\u06ED\u0901-\u0903\u093C\u093E-\u094C\u094D\u0951-\u0954\u0962-\u0963\u0981-\u0983\u09BC\u09BE\u09BF\u09C0-\u09C4\u09C7-\u09C8\u09CB-\u09CD\u09D7\u09E2-\u09E3\u0A02\u0A3C\u0A3E\u0A3F\u0A40-\u0A42\u0A47-\u0A48\u0A4B-\u0A4D\u0A70-\u0A71\u0A81-\u0A83\u0ABC\u0ABE-\u0AC5\u0AC7-\u0AC9\u0ACB-\u0ACD\u0B01-\u0B03\u0B3C\u0B3E-\u0B43\u0B47-\u0B48\u0B4B-\u0B4D\u0B56-\u0B57\u0B82-\u0B83\u0BBE-\u0BC2\u0BC6-\u0BC8\u0BCA-\u0BCD\u0BD7\u0C01-\u0C03\u0C3E-\u0C44\u0C46-\u0C48\u0C4A-\u0C4D\u0C55-\u0C56\u0C82-\u0C83\u0CBE-\u0CC4\u0CC6-\u0CC8\u0CCA-\u0CCD\u0CD5-\u0CD6\u0D02-\u0D03\u0D3E-\u0D43\u0D46-\u0D48\u0D4A-\u0D4D\u0D57\u0E31\u0E34-\u0E3A\u0E47-\u0E4E\u0EB1\u0EB4-\u0EB9\u0EBB-\u0EBC\u0EC8-\u0ECD\u0F18-\u0F19\u0F35\u0F37\u0F39\u0F3E\u0F3F\u0F71-\u0F84\u0F86-\u0F8B\u0F90-\u0F95\u0F97\u0F99-\u0FAD\u0FB1-\u0FB7\u0FB9\u20D0-\u20DC\u20E1\u302A-\u302F\u3099\u309A]";
Ektron.RegExp.CharacterClass.Extender = "[\xB7\u02D0\u02D1\u0387\u0640\u0E46\u0EC6\u3005\u3031-\u3035\u309D-\u309E\u30FC-\u30FE]";
Ektron.RegExp.CharacterClass.NCNameChar = "[" + (Ektron.RegExp.CharacterClass.Letter + Ektron.RegExp.CharacterClass.Digit + "\\.\\-\\_" + Ektron.RegExp.CharacterClass.CombiningChar + Ektron.RegExp.CharacterClass.Extender).replace(/[\[\]]/g, "") + "]";
var reFirstCharNCName = new RegExp("^" + Ektron.RegExp.CharacterClass.Letter + "|^_");
var reNonNCNameChar = new RegExp(Ektron.RegExp.CharacterClass.NCNameChar.replace("[","[^") + "+", "g");

function EkFormFields_FixId(strID)
{
    var s = strID;
    if (!reFirstCharNCName.test(s))
    {
        s = "_" + s;
    }
    s = s.replace(/([a-z])\s+(?=[A-Z])/g, "$1"); // lower to upper, eg, "Two Words" becomes "TwoWords"
    s = s.replace(/(\d)\s+(?=\d)/g, "$1_"); // digit to digit, convert space to "_", eg, "1 2" becomes "1_2"
    s = s.replace(/(\w)\s+(?=\d)/g, "$1"); // letter to digit, eg, "Word 123" becomes "Word123"
    s = s.replace(/(\d)\s+(?=\w)/g, "$1"); // digit to letter, eg, "123 Word" becomes "123Word"
    s = s.replace(reNonNCNameChar, "_"); // convert non-NCNameChar to "_"
    s = s.replace(/^(b|i)$/i, "_$1"); // #62821: single character "b,B,i,I" are preserved ID, eg, "B" becomes "_B"

    return s;
}

function EkFormFields_PromptOnValidateAction(errObj)
{
    if (0 == errObj.length)
    {
        return true;
    }
    if (errObj.srcElement)
    {
        try
        {
            // if the tab that the focusElem is on is not visible, it will throw an error when the following focus is set on one of its fields.
            errObj.srcElement.focus();
        }
        catch (ex) {}
    }
    if (errObj.message.length)
    {
        alert(errObj.message);
    }
    return false;
}

function EkFormFields_GetVariableNames(strText)
{
    var ret = [];
    if (strText.length > 0)
    {
        //get variable names like "{NAME}" or "{X}" from the input string.
        ret = strText.match(/\{[\w\.\-]+\}/g);	
    }
    return ret;
}

function EkFormFields_GetResourceFieldDisplayValue(resourceText, waPath, type, id, title, langType, nav)
{
	var sDisplay = "";
	if (id || 0 === id)
	{
	    var sType = type.toLowerCase();
	    switch (sType)
	    {
	        case "folder":
	            sDisplay = getIdTypeLocaleString(resourceText, "folder", langType) + ":" + id;
	            if ("children" == nav)
	            {
		            sDisplay += " " + resourceText.sItems; 
	            }
	            if ("descendant" == nav)
	            {
	                sDisplay += " + " + resourceText.sChildren; 
	            }
	            break;
	        default:
	            sDisplay = getIdTypeLocaleString(resourceText, type, langType) + ":" + id;   
	            break;
	    }
	    if (title.length > 0) title += " ";
	    if (sDisplay.length > 0) sDisplay = title + "\xAB" + $ektron.trim(sDisplay) + "\xBB";
	}
	return sDisplay;
	
	function getIdTypeLocaleString(resourceText, sLocaleId, langType)
	{
	    var sIdType = resourceText.idType[sLocaleId];
        if (!sIdType || 0 == sIdType.length) 	        
        {
            var url = location.protocol + "//" + location.host + waPath + "webservices/rest.svc/resourcetext.json?key=" + encodeURIComponent(sLocaleId) + "&LangType=" + langType;
            $ektron.ajax({
			    type: "GET",
			    async: false,
			    url: url,
			    success: function(data)
			    { 
                    sIdType = data;
                    resourceText.idType[sLocaleId] = sIdType; 
                },
			    dataType: "json"
		    });
        }
        if (!sIdType || 0 == sIdType.length || " -HC" == sIdType.substr(sIdType.length - 4)) 	        
        {
            var lcolon = sLocaleId.indexOf(":");
            if (lcolon > 0)
            {
                var lcolon2 = sLocaleId.substr(lcolon + 1).indexOf(":");
                if (lcolon2 > 0)
                {
                    var topkey = sLocaleId.substring(0, lcolon + lcolon2 + 1);
                    sIdType = resourceText.idType[topkey];
                }
            }
        }
        if (!sIdType || 0 == sIdType.length) 	        
        {
            sIdType = resourceText.idType["content"];
        }
	    return sIdType;
	}
}

/*********** Ektron.XPathExpression ************/

if (!Ektron.XPathExpression) (function ()
{
    var s_reSimpleXPath = new RegExp(Ektron.RegExp.CharacterClass.NCNameChar.replace("[", "[\\/\\@"));
    var s_reFirstCharNCNameXPath = new RegExp("^\\/" + Ektron.RegExp.CharacterClass.Letter + "|^\\/_");

    Ektron.XPathExpression = function XPathExpression()
    {
        this.txtFieldId = "";
        this.txtFieldName = "";
        this.contentElement = null;
        this.fieldElem = null;
        this.treeViewId = "";

        this.loadContentTree = XPathExpression_loadContentTree;
        this.expandFieldNameSelection = XPathExpression_expandFieldNameSelection;
        this.updateXPath = XPathExpression_updateXPath;
        this.getFieldPath = XPathExpression_getFieldPath;
        this.applyComparator = XPathExpression_applyComparator;
        this.removeComparator = XPathExpression_removeComparator;
        this.applyToFieldNames = XPathExpression_applyToFieldNames;
        this.getXPathOfSelectedElement = XPathExpression_getXPathOfSelectedElement;
        this.fullXPath = XPathExpression_fullXPath;
        this.relativeXPath = XPathExpression_relativeXPath;

        this.p_objTextSel = null;
        var me = this;

        this.init = function XPathExpression_init(fieldId, fieldName, contentElement, fieldElem, treeViewControlId)
        {
            //Ektron.ContentDesigner.trace(Ektron.String.format("XPathExpression_init: id={0} elem={1}", fieldId, fieldElem));
            this.txtFieldId = fieldId;
            this.txtFieldName = fieldName;
            this.contentElement = contentElement;
            this.fieldElem = fieldElem;
            this.treeViewId = treeViewControlId;
            if (ekFieldTreeViewControl && ekFieldTreeViewControl[this.treeViewId])
            {
                ekFieldTreeViewControl[this.treeViewId].setXPathExpression(this);
                if (this.contentTree)
                {
                    ekFieldTreeViewControl[this.treeViewId].load(this.contentTree);
                }
            }
            var oElem = document.getElementById(this.txtFieldId);
            if (oElem && oElem.ownerDocument.selection && oElem.ownerDocument.selection.createRange) // IE
            {
                $ektron(oElem).bind("click", m_captureTextSelection).bind("select", m_captureTextSelection);
            }
        }

        function m_captureTextSelection() // IE only
        {
            // 'this' is an HTML Input Text Element
            var rng = this.ownerDocument.selection.createRange();
            var rngStart = rng.duplicate();
            rngStart.collapse();
            rngStart.moveStart("character", -this.value.length);
            var rngEnd = rng.duplicate();
            rngEnd.moveStart("character", -this.value.length);
            me.p_objTextSel =
			{
			    start: rngStart.text.length
			, end: rngEnd.text.length
			};
        }
    }

    function XPathExpression_loadContentTree(contentTree)
    {
        this.contentTree = contentTree;
        if (ekFieldTreeViewControl && ekFieldTreeViewControl[this.treeViewId])
        {
            ekFieldTreeViewControl[this.treeViewId].load(contentTree);
        }
    }

    function XPathExpression_expandFieldNameSelection()
    {
        var oElem = document.getElementById(this.txtFieldId);
        // assert oElem is HTMLInputElement type="text"
        var start = 0;
        var fieldName = "";
        var objTextSel =
		{
		    text: fieldName
		, start: start
		, end: start + fieldName.length
		};
		if ("number" == typeof oElem.selectionStart && !$ektron.browser.msie) // FF, Safari, Opera
        {
            start = oElem.selectionStart;
            fieldName = this.getFieldPath(oElem.value, start, objTextSel);
            if (fieldName.length > 0)
            {
                oElem.selectionStart = objTextSel.start;
                oElem.selectionEnd = objTextSel.end;
            }
        }
        else if (this.p_objTextSel) // IE
        {
            start = this.p_objTextSel.start;
            fieldName = this.getFieldPath(oElem.value, start, objTextSel);
            // Don't bother actually trying to change the selection, it's already been lost.
            if (fieldName.length > 0)
            {
                this.p_objTextSel = objTextSel;
            }
        }
        oElem = null;
        return objTextSel;
    }

    function XPathExpression_updateXPath(objTextSel, xpath)
    {
        //checks if sXPathField is for validation and if custom validation is allowed
        if (!this.txtFieldId) return;
        var oElem = document.getElementById(this.txtFieldId);
        if (oElem && !oElem.disabled && xpath.length > 0)
        {
            var strCurrentXPath = this.getXPathOfSelectedElement();
            xpath = this.relativeXPath(xpath, strCurrentXPath);
            var text = oElem.value;
            var searchText = objTextSel.text;
            // replace all when selection is "{...}"
            if (Ektron.XPathExpression.isFieldNameVariable(searchText))
            {
                searchText = searchText.replace("{", "\\{").replace("}", "\\}"); // treat { and } as literals
                var reSearchText = new RegExp(searchText, "gi");
                oElem.value = text.replace(reSearchText, xpath);
            }
            else
            {
                oElem.value = text.substring(0, objTextSel.start) + xpath + text.substring(objTextSel.end);
            }
            oElem.focus();
            objTextSel.text = "";
            objTextSel.start = 0;
            objTextSel.end = 0;
            this.p_objTextSel = null;
        }
        oElem = null;
    }

    function XPathExpression_getFieldPath(text, start, refResult)
    {
        // Expand Start position to include ID chars and "/"
        // Returns Position and Length of field name, if Start is within a field name.
        var fieldPath = "";
        var fieldStart = start;
        var p = start;
        var ch = "";
        var len = text.length;
        if (p < len)
        {
            // Expand left
            if (p >= 0)
            {
                ch = text.charAt(p);
                if ("}" == ch || "\"" == ch || "'" == ch)
                {
                    p -= 1;
                }
            }
            while (p >= 0)
            {
                ch = text.charAt(p);
                if (s_reSimpleXPath.test(ch))
                {
                    p -= 1;
                }
                else if ("{" == ch || "\"" == ch || "'" == ch || "$" == ch)
                {
                    p -= 1;
                    break;
                }
                else
                {
                    break;
                }
            }
            fieldStart = p + 1;

            // Expand right
            p = start;
            if (p < len)
            {
                ch = text.charAt(p);
                if ("{" == ch || "\"" == ch || "'" == ch || "$" == ch)
                {
                    p += 1;
                }
            }
            while (p < len)
            {
                ch = text.charAt(p);
                if (s_reSimpleXPath.test(ch))
                {
                    p += 1;
                }
                else if ("}" == ch || "\"" == ch || "'" == ch)
                {
                    p += 1;
                    break;
                }
                else
                {
                    break;
                }
            }
            if (p + 1 < len)
            {
                if ("()" == text.substring(p, p + 2))
                {
                    p += 2;
                }
            }

            if (p > fieldStart)
            {
                fieldPath = text.substring(fieldStart, p);
            }
        }
        if ("object" == typeof refResult && refResult != null)
        {
            refResult.text = fieldPath;
            refResult.start = fieldStart;
            refResult.end = fieldStart + fieldPath.length;
        }
        return fieldPath;
    }

    // To do a relative compare, the dashes ('-') are removed and the value
    // is converted to a number, so '2005-07-06' becomes 20050706, which can be
    // compared relative to other values.
    // Note: empty values will result in NaN and the compare will be false.
    var s_comparators =
	{
	    date:
		{
		    left: "number(translate(", right: ",'-',''))"
		}
	};

    function XPathExpression_applyComparator(xpath, datatype)
    {
        for (var t in s_comparators)
        {
            var comparator = s_comparators[t];
            xpath = this.applyToFieldNames(xpath, datatype, t, comparator);
        }

        //Ektron.ContentDesigner.trace(Ektron.String.format("XPathExpression_applyComparator: xpath='{0}'", xpath));
        return xpath;
    }

    function XPathExpression_removeComparator(xpath, datatype)
    {
        // This is quick and dirty.
        // Assume 'left' and 'right' only exist if applied to a field path.
        xpath = new Ektron.String(xpath);
        for (var t in s_comparators)
        {
            var comparator = s_comparators[t];
            xpath.replace(comparator.left, "").replace(comparator.right, "");
        }

        return xpath.toString();
    }

    function XPathExpression_applyToFieldNames(xpath, datatype, comparatorType, comparator)
    {
        var aryResult = [];
        var objTextSel = {};
        var strCurrentXPath = this.getXPathOfSelectedElement();

        var pStart = xpath.length - 1; // Start at the end and work backward
        var strResult = xpath;
        while (pStart >= 0)
        {
            var strFieldPath = this.getFieldPath(strResult, pStart, objTextSel);
            var pFound = objTextSel.start;
            var nLength = strFieldPath.length;
            if (strFieldPath.length > 0)
            {
                var bApply = false;
                if (Ektron.XPathExpression.isSelf(strFieldPath))
                {
                    bApply = (datatype == comparatorType);
                }
                else if (Ektron.XPathExpression.isStringConstant(strFieldPath))
                {
                    bApply = ("date" == comparatorType) && Ektron.Xml.isDate(strFieldPath.substring(1, strFieldPath.length - 1)); // remove quotes first
                }
                else if (Ektron.XPathExpression.isNumberConstant(strFieldPath))
                {
                    bApply = false;
                }
                else if ("$currentDate" == strFieldPath)
                {
                    bApply = ("date" == comparatorType);
                }
                else if (Ektron.Xml.isDate(strFieldPath))
                {
                    // User forgot to type the quotes, so add them
                    strFieldPath = "'" + strFieldPath + "'";
                    bApply = ("date" == comparatorType);
                }
                else if (!Ektron.XPathExpression.isFieldNameVariable(strFieldPath))
                {
                    var strFullFieldPath = this.fullXPath(strFieldPath, strCurrentXPath);
                    var reContainsXPath = new RegExp("Value\=\"" + Ektron.RegExp.escape(strFullFieldPath) + "\" EktDatatype=\"" + comparatorType + "\"");
                    if (reContainsXPath.test(this.contentTree))
                    {
                        bApply = true;
                    }
                }

                if (bApply)
                {
                    // order will be reversed when done
                    aryResult.push(strResult.substring(pFound + nLength));
                    aryResult.push(comparator.right);
                    aryResult.push(strFieldPath);
                    aryResult.push(comparator.left);
                    strResult = strResult.substring(0, pFound);
                }
                pStart = pFound;
            }
            pStart -= 1;
        }
        aryResult.push(strResult);
        aryResult.reverse();
        return aryResult.join("");
    }

    function XPathExpression_getXPathOfSelectedElement()
    {
        var oElem = this.fieldElem;
        var bNewField = false;
        if (!oElem && this.contentElement)
        {
            oElem = getSelectionElement(this.contentElement.ownerDocument);
            $ektron(oElem).parents().each(function ()
            {
                if ($ektron(this).attr("ektdesignns_name"))
                {
                    oElem = this;
                    bNewField = true;
                    return false; // break;
                }
            });
        }
        if (!oElem) return "";
        if (!Ektron.SmartForm) alert("Missing Ektron.SmartForm");
        if (!Ektron.SmartForm.getXPath) alert("Missing Ektron.SmartForm.getXPath");
        var xpath = Ektron.SmartForm.getXPath(oElem, this.contentElement);
        if (bNewField)
        {
            xpath += "/NewField";
        }
        //Ektron.ContentDesigner.trace(Ektron.String.format("XPathExpression_getXPathOfSelectedElement: xpath='{0}'", xpath));
        return xpath;
    }

    function XPathExpression_fullXPath(strXPath, strCurrentFullXPath)
    {
        // If xpath contains "//" it will be returned without change.
        // Examples,
        // xpath="../c"     and current="/root/a/b/d" returns "/root/a/b/c"
        // xpath="../../c"  and current="/root/a/b/d" returns "/root/a/c"
        // xpath="../a/b/c" and current="/root/d"     returns "/root/a/b/c"
        // xpath="./b"      and current="/root/a"     returns "/root/a/b"
        // xpath="."        and current="/root/a/b/c" returns "/root/a/b/c"
        // xpath="" returns current. Implied "."
        // xpath="b" and current="/root/a" returns "/root/b". Implied leading "../"
        // xpath="a" and current="/root/a" returns "/root/a". Implied leading "../"

        var strFullXPath = "";
        var nXPathLength = 0;
        var P = 0;
        var P2 = 0;

        nXPathLength = strXPath.length;
        if (0 == nXPathLength)
        {
            strFullXPath = strCurrentFullXPath;
        }
        else if (0 == strCurrentFullXPath.length)
        {
            strFullXPath = strXPath;
        }
        else if (strXPath.indexOf("//") > -1)
        {
            strFullXPath = strXPath;
        }
        else if ("/" == strXPath.charAt(0))
        {
            strFullXPath = strXPath;
        }
        else if ("." == strXPath)
        {
            strFullXPath = strCurrentFullXPath;
        }
        else if ("./" == strXPath.substring(0, 2))
        {
            strFullXPath = strCurrentFullXPath + strXPath.substring(1);
        }
        else if ("../" == strXPath.substring(0, 3))
        {
            P = strCurrentFullXPath.length; // start at end of strCurrentFullXPath
            P2 = 3; // start at position following "../"
            while (true)
            {
                P = strCurrentFullXPath.lastIndexOf("/", P);
                if (-1 == P)
                {
                    // Failed to find "/"
                    // aberrant
                    strFullXPath = strXPath;
                    break;
                }
                if ("../" == strXPath.substring(P2, P2 + 3))
                {
                    P = P - 1;
                    P2 = P2 + 3;
                    if (-1 == P)
                    {
                        // aberrant
                        strFullXPath = strXPath;
                        break;
                    }
                    else if (P2 >= nXPathLength)
                    {
                        strFullXPath = strCurrentFullXPath.substring(0, P);
                    }
                    // loop again
                }
                else
                {
                    strFullXPath = strCurrentFullXPath.substring(0, P + 1) + strXPath.substring(P2);
                    break;
                }
            }
        }
        else
        {
            // field name, implied leading ../
            P = strCurrentFullXPath.lastIndexOf("/");
            if (P > -1)
            {
                strFullXPath = strCurrentFullXPath.substring(0, P) + strXPath;
            }
            else
            {
                strFullXPath = strCurrentFullXPath + "/" + strXPath;
            }
        }
        //Ektron.ContentDesigner.trace(Ektron.String.format("XPathExpression_fullXPath: '{0}' orig='{1}' cur='{2}'", strFullXPath, strXPath, strCurrentFullXPath));
        return strFullXPath;
    }

    function XPathExpression_relativeXPath(strXPath, strCurrentFullXPath)
    {
        // If leading char is "/" it must be followed by "root" otherwise it will be returned unchanged.
        // If xpath contains "//" it will be returned without change.
        // Examples,
        // xpath="/root/a/b/c" and current="/root/a/b/d" returns "../c"
        // xpath="/root/a/c"   and current="/root/a/b/d" returns "../../c"
        // xpath="/root/a/b/c" and current="/root/a/b/c" returns "."
        // xpath="/root/a/b/c" and current="/root/d"     returns "../a/b/c"
        // xpath="/root/a/b"   and current="/root/a"     returns "./b", not "b" b/c that implies "../b"
        // xpath="" returns "."
        // xpath="/root/b" and current="/root/a" returns "../b"
        // xpath="/root/a" and current="/root/a" returns "."
        //
        // Unit testing
        //Print RelativeXPath("/root/a/b/c", "/root/a/b/d") = "../c"
        //Print RelativeXPath("/root/a/c", "/root/a/b/d") = "../../c"
        //Print RelativeXPath("/root/a/c", "/root/a[1]/b[1]/d[1]") = "../../c"
        //Print RelativeXPath("/root/a/b/c", "/root/a/b/c") = "."
        //Print RelativeXPath("/root/a/b/c", "/root/d") = "../a/b/c"
        //Print RelativeXPath("/root/a/b", "/root/a") = "./b"
        //Print RelativeXPath("/root/a/b", "/root/a[1]") = "./b"
        //Print RelativeXPath("", "") = "."
        //Print RelativeXPath("/root/b", "/root/a") = "../b"
        //Print RelativeXPath("/root/a", "/root/a") = "."
        //Print RelativeXPath("/root/a", "/root/a[1]") = "."

        var strRelXPath = "";
        var nXPathLength = 0;

        nXPathLength = strXPath.length;
        if (0 == nXPathLength)
        {
            strRelXPath = ".";
        }
        else if (0 == strCurrentFullXPath.length)
        {
            strRelXPath = strXPath;
        }
        else if (!s_reFirstCharNCNameXPath.test(strCurrentFullXPath))
        {
            strRelXPath = strXPath;
        }
        else if (!s_reFirstCharNCNameXPath.test(strXPath))
        {
            strRelXPath = strXPath;
        }
        else if (strXPath.indexOf("//") > -1)
        {
            strRelXPath = strXPath;
        }
        else if (strCurrentFullXPath == strXPath.substring(0, strCurrentFullXPath.length))
        {
            strRelXPath = "." + strXPath.substring(strCurrentFullXPath.length);
        }
        else
        {
            var aryXPath = [];
            var nXPathCount = 0;
            var aryCurXPath = [];
            var nCurXPathCount = 0;
            var nCommonPathCount = 0;

            aryXPath = strXPath.split("/");
            nXPathCount = aryXPath.length;
            aryCurXPath = strCurrentFullXPath.split("/");
            nCurXPathCount = aryCurXPath.length;

            nCommonPathCount = 0;
            while (nCommonPathCount < nXPathCount && nCommonPathCount < nCurXPathCount)
            {
                if (aryXPath[nCommonPathCount] == aryCurXPath[nCommonPathCount] ||
						(aryXPath[nCommonPathCount] + "[1]") == aryCurXPath[nCommonPathCount])
                {
                    aryXPath[nCommonPathCount] = "";
                    nCommonPathCount += 1;
                }
                else
                {
                    break;
                }
            }
            //Ektron.ContentDesigner.trace(Ektron.String.format("XPathExpression_relativeXPath: CommonPathCount={0} CurXPathCount={1} xpath='{2}'", nCommonPathCount, nCurXPathCount, aryXPath.join("/")));

            var sb = new Ektron.String();

            // Join recreates the XPath string, but without the common parts, but the delimiters are kept
            // because the number of items didn't change (it's a pain to ReDim arrays). So call Mid() to
            // remove the extra leading delimiters.
            if (nCommonPathCount < nCurXPathCount)
            {
                // Prepend with enough "../"
                sb.insert("../", nCurXPathCount - nCommonPathCount);
                sb.append(aryXPath.join("/"), nCommonPathCount);
            }
            else
            {
                sb.append(".");
                if (nCommonPathCount >= 1) sb.append(aryXPath.join("/"), nCommonPathCount - 1);
            }
            strRelXPath = sb.toString();
        }
        //Ektron.ContentDesigner.trace(Ektron.String.format("XPathExpression_relativeXPath: '{0}' orig='{1}' cur='{2}'", strRelXPath, strXPath, strCurrentFullXPath));
        return strRelXPath;
    }

    Ektron.XPathExpression.isFieldNameVariable = function (name)
    {
        return (/^\{[^\}]*\}$/.test(name));
    };

    Ektron.XPathExpression.isSelf = function (xpath)
    {
        return ("." == xpath);
    };

    Ektron.XPathExpression.isBooleanConstant = function (xpath)
    {
        return ("true()" == xpath || "false()" == xpath);
    };

    Ektron.XPathExpression.isStringConstant = function (xpath)
    {
        if (typeof xpath != "string" || xpath.length < 2) return false;
        var ch1 = xpath.charAt(0);
        var ch2 = xpath.charAt(xpath.length - 1);
        return (("'" == ch1 && "'" == ch2) || ('"' == ch1 && '"' == ch2));
    };

    Ektron.XPathExpression.isNumberConstant = function (xpath)
    {
        return (!isNaN(xpath))
    };
})();       // Ektron.XPathExpression 

