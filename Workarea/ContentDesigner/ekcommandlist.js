// Copyright 2007-2010 Ektron, Inc.
(function ()
{
    var sDefaultFieldPrefix = "Field";
    var sContentIsValid = "Content is valid.";
    var sContentIsInvalid = "Content is not valid";
    var sWindowsMediaVideo = "Windows Media Video";
    var sContinueSaving = "Continue saving invalid document?";
    var sDesignIsCompatible = "The design changes are compatible with existing data.";
    var sInvalidLocalizeSectionSelection = "You cannot apply 'localize section' button on the selected text. You can only apply it to complete paragraphs.";

    function restorePosition(win)
    {
        win.focus();
        var content = win.document.body.innerHTML;
        win.document.body.innerHTML = content;
    }
    function insertContent(editor, content, strCommandTitle, oOrigElem)
    {
        if (null == content) return;
        var bPossibleEditFields = (oOrigElem && editor.sfInstance.isDDFieldElement(oOrigElem));
        var eOrigElem;
        if (oOrigElem) eOrigElem = $ektron(oOrigElem);
        if (bPossibleEditFields)
        {
            var eContent = $ektron(content);
            var newId = eContent.attr("id");
            var newName = eContent.attr("ektdesignns_caption");
            var oldId = oOrigElem.getAttribute("id");
        }
        if ("object" == typeof content && content && "undefined" == typeof content.nodeType && typeof content.length != "undefined")
        {
            var elems = content;
            if (elems.length > 0)
            {
                content = "";
                for (var i = 0; i < elems.length; i++)
                {
                    content += Ektron.Xml.serializeXhtml(elems[i]);
                }
            }
            else
            {
                content = "";
            }
        }
        else if ("object" == typeof content && content != oOrigElem)
        {
            content = Ektron.Xml.serializeXhtml(content);
        }
        if ("string" == typeof content)
        {
            // related to #40716, due to the changes made in EkRadEditor.js, g_design_selectedField was set to the richarea.
            if (oOrigElem && eOrigElem.hasClass("design_richarea"))
            {
                // it should not replace the richarea by this content. It should insert into the richarea in DataEntry mode.
                oOrigElem = null;
                bPossibleEditFields = false;
            }
            // Don't use replaceElement here because an invalid situation (e.g., DIV inside P, cit. #48172) may occur.
            //			if ("object" == typeof oOrigElem && oOrigElem != null)
            //			{
            //				// replace the original field element with an element of a different type.
            //				replaceElement(editor, content, oOrigElem);
            //			}
            //			else
            //			{
            content = editor.filter.GetDesignContent(content);

            // HtmlToClick space should only be inserted if it is a new field.

            // This is a workaround on the focus issue in FireFox.
            // The focus is usually lost to outside the BODY after the first field element is inserted into the content.
            // If that situation occurs when the 2nd field element is inserted, FireFox will crash. The following detects
            // that situation and restore position by putting the cursor to the 0 location of the content (instead of crashing).
            if (typeof editor.ContentWindow.getSelection == "function")
            {
                var selection = editor.ContentWindow.getSelection();
                var started = selection.anchorOffset;
                var ended = selection.focusOffset;
                if (selection.anchorNode && 1 == selection.anchorNode.nodeType && selection.anchorNode.tagName.toUpperCase() != "BODY" && 0 == started && 0 == ended)
                {
                    // #58905: do not restore the position if the focus is still inside the editor.
                    var objSelRng = new Ektron.SelectionRange({ window: editor.ContentWindow });
                    var oElem = objSelRng.getContainerElement();
                    var containElement = $ektron(oElem).closest("#design_content");
                    if (0 == containElement.length) // not inside editor
                    {
                        restorePosition(editor.ContentWindow);
                    }
                }
            }
            if (bPossibleEditFields)
            {
                editor.contentCache = null; // Ektron
                var eEditIcon = eOrigElem.prevAll("span[data-ektron-forfield='" + oldId + "']");
                var eCheckboxLabel = eOrigElem.nextAll("label[htmlFor='" + newId + "']"); //checkbox field label (if any)
                if ($ektron.browser.safari)//#49914 for Safari
                {
                    editor.sfInstance.setSelectedField(oOrigElem);
                    editor.sfInstance.selectElement(oOrigElem, oOrigElem);
                }
                editor.PasteHtml(content, strCommandTitle, true, true, true);
                if (eEditIcon.length > 0)
                {
                    eCheckboxLabel.remove(); //checkbox field label (if any)
                    eEditIcon.remove();
                    eOrigElem.remove();
                }
                editor.sfInstance.setSelectedField(null);
            }
            else
            {
                // paste the new element 
                editor.PasteHtml(content, strCommandTitle, true, true, true);
            }
            //			}
        }
        else
        {
            // need to clear cache as the field has been swifted in the dialog and the editor needs to know the content has been changed.
            editor.contentCache = null; // Ektron
            if (bPossibleEditFields)
            {
                var eEditIcon = $ektron(oOrigElem).prev("span.design_edit_fieldprop");
                if (1 == eEditIcon.length)
                {
                    var sEditPropToolTip = editor.GetLocalizedString("sEditPropToolTip", "Edit Field:") + " " + newName;
                    eEditIcon.attr("data-ektron-forfield", newId).attr("alt", sEditPropToolTip).attr("title", sEditPropToolTip);
                }
            }
        }
    }

    function replaceElement(editor, content, oOrigElem)
    {
        if ("object" == typeof content && content != oOrigElem)
        {
            content = Ektron.Xml.serializeXhtml(content);
        }
        if ("string" == typeof content)
        {
            content = editor.filter.GetDesignContent(content);

            editor.contentCache = null; // Ektron
            var oParent = oOrigElem.parentNode;
            var oPlaceHolder = oParent.ownerDocument.createElement("div");
            oPlaceHolder.innerHTML = content;
            oParent.replaceChild(oPlaceHolder.firstChild, oOrigElem);
            oOrigElem = null;
            oPlaceHolder = null;
        }
        // need to release the previous object as the globel selected field.
        editor.sfInstance.setSelectedField(null);
    }

    function createCmdArgs(editor, oFieldElem)
    {
        var bIsRootLoc = editor.sfInstance.isRootLocation();
        var oContentElement = editor.sfInstance.getContentElement();
        var uniqueId = getDDUniqueId(editor, oContentElement);
        var args =
		{
		    contentElement: oContentElement,
		    selectedField: oFieldElem,
		    isRootLocation: bIsRootLoc,
		    fieldPrefix: editor.GetLocalizedString("sDefaultFieldPrefix", sDefaultFieldPrefix),
		    fieldId: uniqueId,
		    scrolling: "auto"
		};
        return args;
    }

    function createCmdArgsWFormFieldTree(editor, oFieldElem)
    {
        if (typeof editor.fieldListArray != "object") throw new TypeError(Ektron.String.format("Error: fieldListArray must be an object. fieldListArray is of type '{0}'.", typeof editor.fieldListArray));
        var sExcludeClass = "";
        var xmlTree = getContentTree(editor, null, editor.formContent, sExcludeClass);
        if ("string" == typeof editor.fieldListArray.newRoot)
        {
            // Patch XPath path from form editor to match the XML data to be transformed.
            xmlTree = xmlTree.replace(/\/root\//gi, editor.fieldListArray.newRoot);
        }
        else if ("undefined" == typeof editor.fieldListArray.newRoot)
        {
            // Patch XPath path from form editor to match the XML data to be transformed.
            xmlTree = xmlTree.replace(/\/root\//gi, "/*/Data/");
        }

        var args = createCmdArgs(editor, oFieldElem);
        args.contentTree = xmlTree;
        args.fieldListArray = editor.fieldListArray;
        return args;
    }

    function createCmdArgsWTree(editor, oFieldElem, sExcludeClass, sDatatype)
    {
        var sContent = editor.getContent();
        var xmlTree = getContentTree(editor, oFieldElem, sContent, null, sDatatype);
        var args = createCmdArgs(editor, oFieldElem);
        args.contentTree = xmlTree;
        return args;
    }

    function getContentTree(editor, oFieldElem, sContent, sExcludeClass, sDatatype)
    {
        var srcPath = editor.ekParameters.srcPath;
        var skinPath = editor.ekParameters.skinPath;
        var strXSLT = srcPath + "DesignToFieldTree.xslt";
        var args = [
		  { name: "configUrl", value: srcPath + "ValidateSpec.xml" }
		, { name: "srcPath", value: srcPath }
		, { name: "skinPath", value: skinPath }
		, { name: "LangType", value: editor.ekParameters.userLanguage }
		];
        if ("string" == typeof sExcludeClass) // may be undefined
        {
            args.push({ name: "excludeClass", value: sExcludeClass });
            //			Ektron.ContentDesigner.trace("excludeClass = " + sExcludeClass);
        }
        if (oFieldElem || "string" == typeof sDatatype) // may be undefined
        {
            if (oFieldElem)
            {
                if (oFieldElem.className != "ektdesignns_calendar")
                {
                    sDatatype = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_datatype"), sDatatype);
                }
            }
            args.push({ name: "currentDatatype", value: sDatatype });
            //			Ektron.ContentDesigner.trace("currentDatatype = " + sDatatype);
        }

        var sFieldList = editor.ekXml.xslTransform(sContent, strXSLT, args);
        //		Ektron.ContentDesigner.trace("Content:\n" + sContent);
        //		Ektron.ContentDesigner.trace("FieldList:\n" + sFieldList);
        return sFieldList;
    }

    function getDDUniqueId(editor, oContent)
    {
        var nFieldNum = 1;
        var bFoundElem = true;
        var strFieldId = "";
        while (true == bFoundElem)
        {
            strFieldId = editor.GetLocalizedString("sDefaultFieldPrefix", sDefaultFieldPrefix) + nFieldNum;
            var oFoundElem = oContent.ownerDocument.getElementById(strFieldId);
            if (oFoundElem)
            {
                bFoundElem = true;
                nFieldNum = nFieldNum + 1;
            }
            else
            {
                bFoundElem = false;
            }
        }
        return nFieldNum;
    }

    function createErrorString(editor, err)
    {
        var errmsg = "";
        if ("object" == typeof err && err.length > 0)
        {
            errmsg = err.join("\n\n\n");
        }
        else if ("object" == typeof err && "string" == typeof err.msg)
        {
            errmsg = editor.GetLocalizedString("ContentInvalid", sContentIsInvalid) + "\n\n" + err.msg;
        }
        else
        {
            errmsg = err;
        }
        return errmsg;
    }

    function cleanText(strTextData)
    {
        return strTextData.replace(/<\/?(P|TD|LI)[^>]*>|\n|\r/gi, "");
    }

    function checkIfTextEmpty(textToCheck)
    {
        return /^(\s|\xA0|&nbsp;|&#160;)*$/.test(textToCheck);
    }

    function getDDFieldinSelection(editor)
    {
        //#60078: IE requires 3 clicks to highlight the field element in selection. The below uses the Ektron.SelectionRange to find the selected field element 
        //when the field does not yet have the "design_selected_field" class.
        var objSelRng = new Ektron.SelectionRange({ window: editor.ContentWindow });
        var oElem = objSelRng.getContainerElement();
        if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) >= 9) //editor.IsIE9
        {
            var parentClassName = "";
            var parentElem = null;
            if (oElem && oElem.parentNode)
            {
                parentClassName = oElem.parentNode.className;
            }
            var bHasDDFieldParent = false;
            $ektron.each(parentClassName.split(/\s+/), function (i, className)
            {
                var preClassName = className.substr(0, 12);
                if ("ektdesignns_" == preClassName || "design_selected_field" == className)
                {
                    bHasDDFieldParent = true;
                    parentElem = oElem.parentNode;
                }
                else if ("design_list_" == preClassName)
                {
                    bHasDDFieldParent = true;
                    parentElem = $ektron(oElem).closest(".design_selected_field").get(0);
                }
            });
            if (bHasDDFieldParent)
            {
                oElem = parentElem;
            }
            else
            {
                // need the DD field inside the oElem
                var eElem = $ektron("[ektdesignns_name]", oElem);
                if (1 == eElem.length)
                {
                    oElem = $ektron("[ektdesignns_name]", oElem).get(0);
                }
            }
        }
        if (editor.sfInstance.isDDFieldElement(oElem))
        {
            editor.sfInstance.setSelectedField(oElem);
            editor.sfInstance.selectElement(oElem, oElem);
            return oElem;
        }
        return null;
    }

    function hideContextMenu()
    {
        var popup = window["RadEditorPopupInstance"];
        if (popup)
        {
            popup.Hide();
        }
    }

    // Smart Form	
    function previewData(commandName, editor, oTool)
    {
        var sTitle = editor.Localization[commandName];
        var dataType = "";
        switch (commandName.toLowerCase())
        {
            case "ekpreviewxsd":
                dataType = "dataschema";
                break;
            case "ekpreviewfld":
                dataType = "datafieldlist";
                break;
            case "ekpreviewndx":
                dataType = "dataindex";
                break;
            case "ekpreviewxsl":
                dataType = "datapresentationxslt";
                break;
            case "ekpreviewxml":
            default:
                dataType = "datadocumentxml";
                break;
        }
        var content = editor.getContent(dataType);
        var indentedContent = editor.ekXml.indentXml(content);
        var args = {
            title: sTitle
	    , content: indentedContent
        };

        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/previewdata.aspx"
		, args
		, 700
		, 550
		, null
		, null
		, sTitle);
        return false;
    }

    RadEditorCommandList["EkPreviewXml"] = previewData;
    RadEditorCommandList["EkPreviewXsd"] = previewData;
    RadEditorCommandList["EkPreviewFld"] = previewData;
    RadEditorCommandList["EkPreviewNdx"] = previewData;
    RadEditorCommandList["EkPreviewXsl"] = previewData;

    RadEditorCommandList["EkFieldProp"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        if ($ektron.browser.msie && null == oFieldElem)
        {
            oFieldElem = getDDFieldinSelection(editor);
        }
        if (oFieldElem)
        {
            var tagName = oFieldElem.tagName.toLowerCase();
            if ("img" == tagName && editor.sfInstance.isDDFieldElement(oFieldElem.parentNode))
            {
                editor.sfInstance.setSelectedField(oFieldElem.parentNode);
                editor.sfInstance.selectElement(oFieldElem, oFieldElem.parentNode);
                oFieldElem = oFieldElem.parentNode;
                tagName = oFieldElem.tagName.toLowerCase();
            }
            else if ("li" == tagName)
            {
                var eSelElem = $ektron(oFieldElem).closest(".design_selected_field");
                if (1 == eSelElem.length)
                {
                    oFieldElem = eSelElem.get(0);
                    tagName = oFieldElem.tagName.toLowerCase();
                }
            }
            var targetCommandName = "";
            switch (tagName)
            {
                case "fieldset":
                    targetCommandName = "EkGroupBox";
                    break;
                case "table":
                    if (-1 == oFieldElem.className.indexOf("ektdesignns_") && oFieldElem.className.indexOf("design_selected_field") > -1)
                    {
                        // EkTabularDataBox field does not have a specified className.
                        targetCommandName = "EkTabularDataBox";
                    }
                    else if (oFieldElem.className.indexOf("ektdesignns_conditional") > -1)
                    {
                        targetCommandName = "EkConditional";
                    }
                    break;
                case "textarea":
                    targetCommandName = "EkTextField";
                    break;
                case "input":
                    if ("checkbox" == oFieldElem.type)
                    {
                        targetCommandName = "EkCheckBoxField";
                    }
                    else if (oFieldElem.className.indexOf("design_calculation") > -1)
                    {
                        targetCommandName = "EkCalculatedField";
                    }
                    else
                    {
                        targetCommandName = "EkTextField";
                    }
                    break;
                case "div":
                case "span":
                    $ektron.each(oFieldElem.className.split(/\s+/), function (i, className)
                    {
                        switch (className)
                        {
                            case "ektdesignns_choices":
                            case "ektdesignns_checklist":
                                targetCommandName = "EkChoicesField";
                                break;
                            case "ektdesignns_calendar":
                                targetCommandName = "EkCalendarField";
                                break;
                            case "ektdesignns_richarea":
                                targetCommandName = "EkTextField";
                                break;
                            case "ektdesignns_imageonly":
                                targetCommandName = "EkImageOnlyField";
                                break;
                            case "ektdesignns_filelink":
                                targetCommandName = "EkFileLinkField";
                                break;
                            case "ektdesignns_resource":
                                targetCommandName = "EkResourceSelectorField";
                                break;
                        }
                    });
                    break;
                case "select":
                    targetCommandName = "EkChoicesField";
                    break;
                default:
                    //alert(oFieldElem.tagName);
                    targetCommandName = "";
                    break;
            }
            if (targetCommandName.length > 0)
            {
                RadEditorCommandList[targetCommandName](targetCommandName, editor, oTool);
            }
        }
    };

    RadEditorCommandList["EkResourceSelectorField"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgs(editor, oFieldElem);
        args.EditorObj = editor;
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/resourceselectorfield.aspx"
		, args
		, 490
		, 430
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    RadEditorCommandList["EkGroupBox"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgsWTree(editor, oFieldElem);
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/groupbox.aspx"
		, args
		, 490
		, 430
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    RadEditorCommandList["EkTabularDataBox"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgsWTree(editor, oFieldElem);
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/tabulardatabox.aspx"
		, args
		, 490
		, 550
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    RadEditorCommandList["EkCheckBoxField"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgsWTree(editor, oFieldElem);
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/checkboxfield.aspx"
		, args
		, 620
		, 440
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    RadEditorCommandList["EkTextField"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgsWTree(editor, oFieldElem);
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/textfield.aspx"
		, args
		, 620
		, 550
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    RadEditorCommandList["EkChoicesField"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgs(editor, oFieldElem);
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
        editor.ekParameters.srcPath + "dialogs/choicesfield.aspx"
        , args
        , 480
        , 570
        , callback
        , null
        , sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    RadEditorCommandList["EkCalculatedField"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgsWTree(editor, oFieldElem);
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/calculatedfield.aspx"
		, args
		, 620
		, 535
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    RadEditorCommandList["EkCalendarField"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgsWTree(editor, oFieldElem, null, "date");
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/calendarfield.aspx"
		, args
		, 620
		, 440
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    RadEditorCommandList["EkImageOnlyField"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgs(editor, oFieldElem);
        args.EditorObj = editor;
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/imageonlyfield.aspx"
		, args
		, 600
		, 540
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    RadEditorCommandList["EkFileLinkField"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        if (null == oFieldElem && false == editor.isSelectionEditable()) return;
        var args = createCmdArgs(editor, oFieldElem);
        args.EditorObj = editor;
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/filelinkfield.aspx"
		, args
		, 600
		, 580
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    RadEditorCommandList["EkMergeField"] = function (commandName, editor, oTool)
    {
        if (typeof editor.formContent != "string") throw new TypeError(Ektron.String.format("Error: formContent must be a string. formContent is of type '{0}'.", typeof editor.formContent));
        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgsWFormFieldTree(editor, oFieldElem);
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		    editor.ekParameters.srcPath + "dialogs/mergefield.aspx"
		    , args
		    , 450
		    , 430
		    , callback
		    , null
		    , sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            if ("string" == typeof returnValue)
            {
                insertContent(editor, returnValue, sTitle, oFieldElem);
            }
        }
    };

    RadEditorCommandList["EkConditional"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;
        var oFieldElem = editor.sfInstance.getSelectedField();
        var args;
        if ("string" == typeof editor.formContent)
        {
            args = createCmdArgsWFormFieldTree(editor, oFieldElem);
        }
        else
        {
            args = createCmdArgsWTree(editor, oFieldElem);
        }
        args.EditorObj = editor;
        args.selectedHtml = editor.GetSelectionHtml(); //can be img, text node etc.
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		    editor.ekParameters.srcPath + "dialogs/conditionalsection.aspx"
		    , args
		    , 600
		    , 540
		    , callback
		    , null
		    , sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    RadEditorCommandList["EkLocalize"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;
        var oFieldElem = editor.sfInstance.getSelectedField();
        if (null == oFieldElem && false == editor.isSelectionEditable()) return;
        var selectedElement = editor.GetSelectedElement();
        if (null == oFieldElem)
        {
            oFieldElem = selectedElement;
        }
        if (!$ektron(oFieldElem).hasClass("ektdesignns_localize"))
        {
            oFieldElem = $ektron(oFieldElem).parents("table.ektdesignns_localize")[0];
        }
        var selectedtxt = selectedElement.innerHTML;
        var bListElem = false;
        if (null == oFieldElem)
        {
            if ("LI" == selectedElement.tagName || "UL" == selectedElement.tagName || "OL" == selectedElement.tagName)
            {
                bListElem = true;
                var eSelectedList = $ektron(selectedElement).closest("UL");
                if (0 == eSelectedList.length)
                {
                    eSelectedList = $ektron(selectedElement).closest("OL");
                }
                selectedElement = eSelectedList.get(0);
            }
            var selectionHtml = editor.GetSelectionHtml();
            if (isAllowedSelection(selectionHtml))
            {
                selectedtxt = selectionHtml;
            }
            else
            {
                selectedtxt = $ektron(selectedElement).clone().wrap('<div></div>').parent().html(); //get outerHTML
                selectedtxt = reToLowerCase(selectedtxt); // unfortunately, the above jquery will return all the upper case tagName in IE
                editor.sfInstance.selectElement(selectedElement, selectedElement);
            }
            if (!isAllowedSelection(selectedtxt))
            {
                alert(editor.GetLocalizedString("sInvalidLocalizeSectionSelection", sInvalidLocalizeSectionSelection));
                return;
            }
        }
        
        var args = createCmdArgs(editor, oFieldElem);
        args.EditorObj = editor;
        args.selectedHtml = selectedtxt;  //must be blocking tag element. 
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/localizesection.aspx"
		, args
		, 600
		, 540
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            if (returnValue != "")
            {
                if (oFieldElem != null && editor.IsIE && $ektron(oFieldElem).hasClass("ektdesignns_localize"))
                {
                    var locTargets = $ektron(returnValue).attr('targets');
                    var locExclude = $ektron(returnValue).attr('exclude');
                    var locNote = $ektron(returnValue).attr('note');

                    $ektron("td.ektdesignns_localize_targets", oFieldElem).html(locTargets);
                    $ektron("td.ektdesignns_localize_exclude", oFieldElem).html(locExclude);
                    $ektron("td.ektdesignns_localize_note", oFieldElem).html(locNote);
                }
                else
                {
                    if (bListElem)
                    {
                        replaceElement(editor, returnValue, selectedElement);
                    }
                    else
                    {
                        insertContent(editor, returnValue, sTitle, selectedElement);
                    }
                }
            }
        }

        function isAllowedSelection(selectionHTML)
        {
            var bAllowed = false;
            var eSelElem = $ektron(selectionHTML);
            if (0 == eSelElem.length) return false;
            var tagName = eSelElem.get(0).tagName;
            switch (tagName.toUpperCase())
            {
                case "P":
                case "DIV":
                case "BLOCKQUOTE":
                case "H1":
                case "H2":
                case "H3":
                case "H4":
                case "H5":
                case "H6":
                case "OL":
                case "UL":
                    bAllowed = true;
                    break;
                default:
                    bAllowed = false;
                    break;
            }
            return bAllowed;
        }

        // convert all tagName to lower case
        // http://www.webdeveloper.com/forum/showthread.php?t=134544
        function reToLowerCase(str)
        {
            var M = str.match(/(< *\w+)/g);
            var L = M.length;
            var temp = '';
            for (var i = 0; i < L; i++)
            {
                var tem = M[i].substring(1);
                tem = tem.toLowerCase();

                if (temp.search(tem) != -1) continue;
                var rx = new RegExp("(< *\/? *)" + tem, 'gi');
                temp += tem + '; '
                str = str.replace(rx, '$1' + tem);
            }
            str = str.replace(/> *</g, '>\n<');
            return str;
        }
    };

    RadEditorCommandList["EkLocalizeRemove"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;
        var oFieldElem = editor.sfInstance.getSelectedField();
        if (null == oFieldElem)
            oFieldElem = editor.GetSelectedElement();

        if (!$ektron(oFieldElem).hasClass("ektdesignns_localize"))
            oFieldElem = $ektron(oFieldElem).parents("table.ektdesignns_localize")[0];

        if (oFieldElem != null)
        {
            editor.sfInstance.setSelectedField(oFieldElem);
            editor.sfInstance.selectElement(oFieldElem, oFieldElem);

            var localizeContent = $ektron("tbody>tr>td", oFieldElem).contents();
            $ektron(oFieldElem).replaceWith(localizeContent);

            hideContextMenu();
        }
        return false;
    };

    RadEditorCommandList["EkValidateDesign"] = function (commandName, editor, oTool)
    {
        // for design mode
        var err = editor.validateDesign();
        if (null == err)
        {
            alert(editor.GetLocalizedString("sContentIsValid", sContentIsValid));
        }
        else
        {
            alert(createErrorString(editor, err));
        }
        return false;
    };

    RadEditorCommandList["EkValidateData"] = function (commandName, editor, oTool)
    {
        // for data entry mode
        var err = editor.validateXmlContent();
        if (null == err)
        {
            alert(editor.GetLocalizedString("sContentIsValid", sContentIsValid));
        }
        else
        {
            alert(createErrorString(editor, err));
        }
        return false; //In case the command does not require updating the editor state (such as a dialog open command)
    };

    RadEditorCommandList["EkCompatibility"] = function (commandName, editor, oTool)
    {
        // for design mode
        var err = editor.checkCompatibility();
        if (null == err)
        {
            alert(editor.GetLocalizedString("sDesignIsCompatible", sDesignIsCompatible));
        }
        else
        {
            alert(createErrorString(editor, err));
        }
        return false;
    };

    //Data Entry Commands
    RadEditorCommandList["EkRichAreaSource"] = function (commandName, editor, oTool)
    {
        var sTitle = editor.Localization[commandName];
        var oFieldElem = editor.sfInstance.getSelectedField();
        var sContent = Ektron.Xml.serializeXhtml(oFieldElem.childNodes);
        sContent = editor.filter.GetHtmlContent(sContent);
        var args =
	    { title: sTitle
	    , EditorObj: editor
	    , content: sContent
	    , contentType: "xhtml"
	    };
        editor.ShowDialog(editor.GetDialogUrl("MozillaPasteHelperDlg")
		, args
		, 700
		, 550
		, callback
		, null
		, sTitle
	    );
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            if ("string" == typeof returnValue)
            {
                sContent = editor.filter.GetDesignContent(returnValue);
                oFieldElem.innerHTML = sContent;
                Ektron.SelectionRange.ensureContentUsability(oFieldElem);
                editor.SmartForm.validateElement(oFieldElem);
            }
            editor.sfInstance.setSelectedField(null);
        }
    };

    var s_previousSelectedResourceIdType = "";
    var s_previousSelectedResourceIdValue = "";
    var s_previousResourceSearchTerm = "";
    RadEditorCommandList["EkResourceSelectorPopup"] = function (commandName, editor, oTool)
    {
        // for data entry mode
        var oFieldBtn = editor.sfInstance.getSelectedField();
        var oFieldElem = editor.SmartForm.getFieldFromFieldButton(oFieldBtn);
        var sSearchBy = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_searchby"), "folder taxonomy words");
        var sFilterBy = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_filterby"), "content:htmlcontent ");
        var sIdType = $ektron.toStr(oFieldElem.getAttribute("datavalue_idtype"), "content:htmlcontent");
        var sResourceDisplay = $ektron.toStr(oFieldElem.getAttribute("datavalue_displayvalue"), "");
        var sSelectorType;
        var height;
        switch (sIdType)
        {
            case "collection":
                sSelectorType = "collection";
                sSearchBy = "collection";
                sFilterBy = "collection";
                height = 580;
                break;
            case "taxonomy":
                sSelectorType = "taxonomy";
                height = 450;
                sSearchBy = "taxonomy ";
                sFilterBy = "taxonomy";
                break;
            case "folder":
                sSelectorType = "folder";
                height = 450;
                sSearchBy = "folder ";
                break;
            case "content":
            default:
                sSelectorType = "content";
                height = 580;
                break;
        }
        var args = {
            appPath: editor.ekParameters.srcPath
    , langType: editor.ekParameters.contentLanguage
    , selectedField: oFieldElem
    , idType: (sIdType || s_previousSelectedResourceIdType)
    , idValue: $ektron.toStr(oFieldElem.getAttribute("datavalue"), s_previousSelectedResourceIdValue)
	, searchByFolder: (/\bfolder\b/.test(sSearchBy))
	, searchByTaxonomy: (/\btaxonomy\b/.test(sSearchBy))
	, searchByWords: (/\bwords\b/.test(sSearchBy))
	, filterBy: sFilterBy
    , selectorType: sSelectorType
    , defaultFolder: ($ektron.toInt(oFieldElem.getAttribute("ektdesignns_defaultfolder"), 0))
    , folderNavigation: oFieldElem.getAttribute("ektdesignns_foldernavigation")
    , startFolderTitle: oFieldElem.getAttribute("ektdesignns_startfoldertitle")
    , searchTerm: s_previousResourceSearchTerm
    , display: sResourceDisplay
        };
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/resourceselectorpopup.aspx?LangType=" + editor.ekParameters.contentLanguage + "&SelectorType=" + sIdType + "&idType=" + sIdType
		, args
		, 500
		, height
		, callback
		, null
		, editor.Localization[commandName]);
        return false;

        function callback(returnValue)
        {
            if (returnValue && returnValue.idType) //Resource Selector Object
            {
                var idValue;
                switch (returnValue.selectorType)
                {
                    case "folder":
                        idValue = returnValue.folderId;
                        break;
                    case "collection":
                    case "taxonomy":
                        idValue = returnValue.resourceId;
                        break;
                    case "content":
                    default:
                        idValue = returnValue.idValue;
                        break;
                }
                s_previousSelectedResourceIdType = returnValue.idType;
                s_previousSelectedResourceIdValue = idValue;
                if (returnValue.searchTerm)
                {
                    s_previousResourceSearchTerm = returnValue.searchTerm;
                }
                oFieldElem.setAttribute("datavalue_idtype", returnValue.idType);
                oFieldElem.setAttribute("datavalue", $ektron.toInt(idValue, 0).toString());
                oFieldElem.setAttribute("datavalue_displayvalue", returnValue.sDisplay);
                $ektron("span", oFieldElem).html(returnValue.sDisplay);

                editor.SmartForm.validateElement(oFieldElem);
                editor.sfInstance.setSelectedField(null);
            }
        }
    };

    RadEditorCommandList["EkCalendarPopup"] = function (commandName, editor, oTool)
    {
        // for data entry mode
        var oFieldBtn = editor.sfInstance.getSelectedField();
        var oFieldElem = editor.SmartForm.getFieldFromFieldButton(oFieldBtn);
        var args = { selectedField: oFieldElem };
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/calendarpopup.aspx"
		, args
		, 200
		, 320
		, callback
		, null
		, editor.Localization[commandName]);
        return false;

        function callback(returnValue)
        {
            replaceElement(editor, returnValue, oFieldElem);
            editor.SmartForm.validateElement(oFieldElem);
            editor.sfInstance.setSelectedField(null);
        }
    };
    RadEditorCommandList["EkFileLinkPopup"] = function (commandName, editor, oTool)
    {
        // for data entry mode
        var oFieldBtn = editor.sfInstance.getSelectedField();
        var oFieldElem = editor.SmartForm.getFieldFromFieldButton(oFieldBtn);
        // modified from RADCOMMAND_SET_LINK_PROPERTIES and GetSelectionLinkArgument at u_radEditor__ListsofCommands.js
        var folderId = editor.ekParameters.FolderId;
        var args = getFileLinkArgument(oFieldElem);
        args.editor = editor;
        args.folderId = folderId;
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.GetDialogUrl("LinkManager")
		, args
		, 600
		, 470
		, callback
		, { cmdName: sTitle }
		, sTitle);
        return false;

        function callback(returnValue)
        {
            if (returnValue) //LinkObject
            {
                var objAnch = null;
                var strUrl = returnValue.href;
                var sText = returnValue.text;
                while (oFieldElem.firstChild)
                {
                    oFieldElem.removeChild(oFieldElem.firstChild);
                }
                if (strUrl.length > 0)
                {
                    objAnch = oFieldElem.ownerDocument.createElement("a");
                    oFieldElem.appendChild(objAnch);
                    objAnch.setAttribute("href", strUrl);
                    objAnch.setAttribute("data-ektron-url", strUrl);
                    if (returnValue.title.length > 0)
                    {
                        objAnch.setAttribute("title", returnValue.title);
                    }
                    if (returnValue.target.length > 0)
                    {
                        objAnch.setAttribute("target", returnValue.target);
                    }
                    if (returnValue.className.length > 0)
                    {
                        objAnch.className = returnValue.className;
                    }
                    $ektron(objAnch).text(sText);
                    var objTextNode = oFieldElem.ownerDocument.createTextNode(" ");
                    oFieldElem.appendChild(objTextNode);
                }
                oFieldElem.appendChild(oFieldBtn);

                insertContent(editor, oFieldElem, sTitle, oFieldElem);
                editor.SmartForm.validateElement(oFieldElem);
                editor.sfInstance.setSelectedField(null);
            }
        }

        function getFileLinkArgument(oFieldElem)
        {
            editor.SetFocus();
            var argument =
		{
		    realLinkObject: null,
		    href: "",
		    className: "",
		    text: "",
		    target: "",
		    name: "",
		    title: "",
		    showText: false,
		    documentAnchors: document.anchors,
		    CssClasses: []
		};

            var joElem = $ektron(oFieldElem).find("a");
            var oElem = joElem.get(0);
            if (joElem.length > 0)
            {
                argument.realLinkObject = oElem;
                argument.href = RadEditorNamespace.GetAnchorToCurrentPage(oElem, editor.IsIE);
                argument.className = oElem.className;
                argument.text = $ektron.htmlDecode(joElem.html());
                argument.target = oElem.target;

                argument.name = oElem.name;
                argument.title = oElem.title;

                editor.SelectElement(oElem);
            }
            //Obtain the CssClass array for the A
            argument.CssClasses = editor.GetCssClassesByTagName("A", editor.Document);

            if (!RadEditorNamespace.Utils.Trim(argument.text))
            {
                argument.text = "";
            }
            argument.showText = !RadEditorNamespace.Utils.HasHtmlContent(argument.text);
            return argument;
        }

    };
    RadEditorCommandList["EkImageOnlyPopup"] = function (commandName, editor, oTool)
    {
        // for data entry mode
        var oFieldBtn = editor.sfInstance.getSelectedField();
        var oFieldElem = editor.SmartForm.getFieldFromFieldButton(oFieldBtn);
        // modified from RADCOMMAND_SET_IMAGE_PROPERTIES at u_radEditor__ListsofCommands.js
        var folderId = editor.ekParameters.FolderId;
        commandName = "SetImageProperties";
        var oImg = $ektron(oFieldElem).find("img").get(0);
        if (editor.sfInstance.isFieldButton(oImg))
        {
            var args = null;
            editor.ShowDialog(
		editor.workareaPath + "mediamanager.aspx?actiontype=library&action=ViewLibraryByCategory&scope=images&text=imgonlyselect&autonav=" + folderId
		, args
		, 790
		, 550
		, callback
		, null
		, editor.Localization[commandName]);
        }
        else
        {
            var cssClasses = editor.GetCssClassesByTagName("IMG", editor.Document);
            var argument =
		{
		    imageToModify: oImg
			, EditorObj: editor
			, CssClasses: cssClasses
			, ThumbnailSuffix: editor.ThumbSuffix
			, InternalParameters: editor.GetDialogInternalParameters(commandName)
			, folderId: folderId
			, commandType: "imgpopupmediaselect"
		};
            var callBackParam =
			{
			    CommandTitle: editor.Localization[commandName]
				, OriginalImage: oImg
			};

            editor.ShowDialog(editor.GetDialogUrl(commandName)
				, argument
				, 600
				, 470
				, callback
				, callBackParam
				, editor.Localization[commandName]);

        }
        return false;

        function callback(returnValue)
        {
            if (returnValue) //Image Object || Library Object
            {
                var objImg = null;
                var sAttrSrc = null;
                if (returnValue.src)
                {
                    sAttrSrc = returnValue.getAttribute("src");
                }
                var strUrl = (returnValue.href || sAttrSrc || returnValue.sFilename);
                while (oFieldElem.firstChild)
                {
                    oFieldElem.removeChild(oFieldElem.firstChild);
                }
                if (strUrl && strUrl.length > 0 && strUrl != returnValue.baseURI)
                {
                    if ("IMG" == returnValue.tagName)
                    {
                        // Image Object
                        objImg = returnValue;
                    }
                    else
                    {
                        // Library Object
                        objImg = oFieldElem.ownerDocument.createElement("img");
                        objImg.setAttribute("src", strUrl);
                        objImg.setAttribute("data-ektron-url", strUrl);
                        objImg.alt = returnValue.sCaption;
                    }
                    oFieldElem.appendChild(objImg);
                    var objTextNode = oFieldElem.ownerDocument.createTextNode(" ");
                    oFieldElem.appendChild(objTextNode);
                }
                oFieldElem.appendChild(oFieldBtn);
                insertContent(editor, oFieldElem, "Insert Image", oFieldElem);
                editor.SmartForm.validateElement(oFieldElem);
                editor.sfInstance.setSelectedField(null);
            }
        }
    };
    RadEditorCommandList["EkLibrary"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode == RadEditorNamespace.RADEDITOR_HTML_MODE) return;
        // for opening up the library
        var oFieldElem = editor.sfInstance.getSelectedField();
        // #40716: if Library item is not allowed be in this oFieldElem, try not to popup Library window. 
        if (typeof oFieldElem != "undefined" && oFieldElem)
        {
            var sTag = oFieldElem.tagName;
            if (!isAllowedObject(sTag))
            {
                if (false == ekCanHaveChildren(oFieldElem)) return;
                if ("TEXTAREA" == sTag) return;
            }
        }
        if (false == editor.isSelectionEditable()) return;
        var selElemSafari = null;
        if ($ektron.browser.safari)
        {
            // #43971: in Safari, if user hits ENTER, does NOT type and insert Library item, the cursor is in "<p><br></p>". 
            // If a PasteHtml is performed, the editor cannot get a selection to paste the content into. Therefore, at the ekLibrary
            // command call, it passes along the BR element for the replaceElement function to swap the content with.
            var cursorLocation = editor.GetSelectedElement();
            if (cursorLocation && cursorLocation.firstChild && "BR" == cursorLocation.firstChild.tagName)
            {
                selElemSafari = cursorLocation.firstChild;
            }
        }
        var args = null;
        var folderId = editor.ekParameters.FolderId;
        editor.ShowDialog(
		editor.workareaPath + "mediamanager.aspx?actiontype=library&scope=all&autonav=" + folderId
		, args
		, 790
		, 550
		, callback
		, null
		, editor.Localization[commandName]);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            if (returnValue)
            {
                var sHtml = "";
                var sCaption = $ektron.htmlEncode(returnValue.sCaption);
                oFieldElem = (editor.sfInstance.getSelectedField() || selElemSafari);
                switch (returnValue.sType.toLowerCase())
                {
                    case "images":
                        sHtml = "<img src=\"" + returnValue.sFilename + "\" alt=\"" + sCaption + "\" title=\"" + sCaption + "\" />";
                        insertContent(editor, sHtml, "Insert Library Image", oFieldElem);
                        break;
                    case "thumbnail":
                        sHtml = returnValue.sFilename;
                        insertContent(editor, sHtml, "Insert Thumbnail", oFieldElem);
                        break;
                    case "files":
                    case "hyperlinks":
                    case "quicklinks":
                    default:
                        var selectedHtml = editor.GetSelectionHtml(); //can be img, text node etc.
                        var replacedHtml = cleanText(selectedHtml);
                        if (typeof replacedHtml != "undefined" && replacedHtml != null && false == checkIfTextEmpty(replacedHtml))
                        {
                            var oSelElem = editor.GetSelectedElement();
                            var eSelElem = $ektron(oSelElem);
                            if ($ektron.browser.mozilla && "LI" == oSelElem.tagName && oSelElem.lastChild && "BR" == oSelElem.lastChild.tagName)
                            {
                                // #61338: FF adds in BR tags and editor did not removed it.
                                $ektron("br:last-child", oSelElem).remove();
                            }
                            if (replacedHtml.indexOf("<") > -1)
                            {
                                var joHtml = $ektron(replacedHtml);
                                if (joHtml.length >= 1)
                                {
                                    var sTag = joHtml.get(0).tagName;
                                    if (isAllowedObject(sTag))
                                    {
                                        //replacedHtml will be the <IMG>, <APPLET>, <OBJECT>, or the <IFRAME> object.
                                    }
                                    else if ($ektron.isEditableElement(oSelElem))
                                    {
                                        replacedHtml = $ektron.removeTags(replacedHtml);
                                    }
                                    else
                                    {
                                        replacedHtml = sCaption;
                                    }
                                }
                                else if (!$ektron.isEditableElement(oSelElem))
                                {
                                    replacedHtml = sCaption;
                                }
                            }
                            sHtml = "<a href=\"" + returnValue.sFilename + "\" data-ektron-url=\"" + returnValue.sFilename + "\" title=\"" + sCaption + "\">" + replacedHtml + "</a>";
                        }
                        else
                        {
                            sHtml = "<a href=\"" + returnValue.sFilename + "\" data-ektron-url=\"" + returnValue.sFilename + "\" title=\"" + sCaption + "\">" + sCaption + "</a>";
                        }
                        insertContent(editor, sHtml, "Insert Library Link", oFieldElem);
                        break;
                }
            }
        }

        function isAllowedObject(tagName)
        {
            var bAllowed = false;
            tagName = tagName.toUpperCase();
            switch (tagName)
            {
                case "IMG":
                case "APPLET":
                case "OBJECT":
                case "IFRAME":
                    bAllowed = true;
                    break;
                default:
                    bAllowed = false;
                    break;
            }
            return bAllowed;
        }
    };
    RadEditorCommandList["EkTranslate"] = function (commandName, editor, oTool)
    {
        // for opening up the translate window
        //var args = createCmdArgs(editor, oFieldElem);
        var args = { "content": editor.getContent() };
        var targetlanguage = (editor.ekParameters.contentLanguage || jsContentLanguage || 0);
        var sourcelanguage = (jsDefaultContentLanguage || 0);
        var editorname = editor.Id;
        var contentid = jsId; //jsId, jsDefaultContentLanguage, jsContentLanguage are variables in edit.aspx
        editor.ShowDialog(
		editor.ekParameters.srcPath + "../worldlingo.aspx?LangType=" + targetlanguage + "&DefaultContentLanguage=" + sourcelanguage + "&htmleditor=" + encodeURIComponent(editorname) + "&id=" + contentid
		, args
		, 550
		, 553
		, callback
		, null
		, editor.Localization[commandName]);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            if (returnValue)
            {
                if ("string" == typeof returnValue)
                {
                    editor.setContent("document", returnValue);
                }
            }
        }
    };

    RadEditorCommandList["EkAddLinkPage"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var selectedObj = editor.GetSelection();
        var strTextData = $ektron.trim(selectedObj.GetHtmlText());
        var replacedText = cleanText(strTextData);
        if (checkIfTextEmpty(replacedText) == true)
        {
            alert('You must select some text.');
        }
        else
        {
            var wikititle = "";
            var target = "";
            var strArticleTitle = (selectedObj.GetText() || $ektron(strTextData).attr("alt") || strTextData);
            var folderId = editor.ekParameters.FolderId;
            var args = { selectedText: strArticleTitle, selectedHTML: strTextData, scrolling: "auto" };
            var myRe = new RegExp("folderid=\"([^\">])*");
            var ar = myRe.exec(strTextData);
            if (ar != null && ar.length > 0)
            {
                folderId = ar[0].replace("folderid=\"", "");
            }
            myRe = new RegExp("wikititle=\"([^\">])*");
            ar = myRe.exec(strTextData);
            if (ar != null && ar.length > 0)
            {
                wikititle = "&wikititle=" + ar[0].replace("wikititle=\"", "");
            }
            myRe = new RegExp("target=\"([^\">])*");
            ar = myRe.exec(strTextData);
            if (ar != null && ar.length > 0)
            {
                target = "&target=" + ar[0].replace("target=\"", "");
            }
            editor.ShowDialog(
			editor.ekParameters.srcPath + "../ewebeditpro/wikipopup.aspx?editorName=" + escape(editor.Id) + "&FolderID=" + folderId + wikititle + target
			, args
			, 680
			, 385
			, callback
			, null
			, editor.Localization[commandName]);
        }
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            if (returnValue)
            {
                if ("string" == typeof returnValue.sHtml)
                {
                    editor.PasteHtml(returnValue.sHtml);
                }
            }
        }
    };
    //HTML Form
    RadEditorCommandList["EkHtmlFieldProp"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        if ($ektron.browser.msie && null == oFieldElem)
        {
            oFieldElem = getDDFieldinSelection(editor);
        }
        if (oFieldElem)
        {
            var targetCommandName = "";
            switch (oFieldElem.tagName.toLowerCase())
            {
                case "input":
                    if ("checkbox" == oFieldElem.type)
                    {
                        targetCommandName = "EkHtmlCheckBoxField";
                    }
                    else
                    {
                        targetCommandName = "EkHtmlTextField";
                    }
                    break;
                case "textarea":
                    targetCommandName = "EkHtmlTextField";
                    break;
                case "div":
                case "span":
                    $ektron.each(oFieldElem.className.split(/\s+/), function (i, className)
                    {
                        if ("ektdesignns_choices" == className || "ektdesignns_checklist" == className)
                        {
                            targetCommandName = "EkHtmlChoicesField";
                        }
                        else if ("ektdesignns_calendar" == className)
                        {
                            targetCommandName = "EkHtmlCalendarField";
                        }
                    });
                    break;
                case "select":
                    targetCommandName = "EkHtmlChoicesField";
                    break;
                default:
                    //alert(oFieldElem.tagName);
                    targetCommandName = "";
                    break;
            }
            if (targetCommandName.length > 0)
            {
                RadEditorCommandList[targetCommandName](targetCommandName, editor, oTool);
            }
        }
    };

    //HTML Form
    RadEditorCommandList["EkHtmlCheckBoxField"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgsWTree(editor, oFieldElem);
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/htmlcheckboxfield.aspx"
		, args
		, 620
		, 440
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    RadEditorCommandList["InsertWMV"] = function (commandName, editor, oTool)
    {
        var sTitle = editor.Localization[commandName];
        var args = { content: sTitle };
        editor.ShowDialog(
          editor.ekParameters.srcPath + "dialogs/WindowsMediaVideo.aspx"
          , args
          , 400
          , 200
          , callback
          , null
          , sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            if (returnValue)
            {
                var strWmvSrc = returnValue.url;
                var strHtml = Ektron.String.format(
		'<embed name="MediaPlayer" type="application/x-mplayer2" width="200" height="160" src="{0}" title="{1}" autostart="0"></embed>',
					$ektron.htmlEncode(strWmvSrc), $ektron.htmlEncode(editor.GetLocalizedString("sWindowsMediaVideo", sWindowsMediaVideo))) + " ";
                insertContent(editor, strHtml, sTitle);
            }
        }
    };

    RadEditorCommandList["EkEmoticonSelect"] = function (commandName, editor, oTool)
    {
        // for opening up the library
        var oFieldElem = editor.sfInstance.getSelectedField();
        var sTitle = editor.Localization[commandName];
        var args = { content: sTitle };
        editor.ShowDialog(
		editor.workareaPath + "threadeddisc/emoticon_select.aspx"
		, args
		, 270
		, 230
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            if (returnValue)
            {
                var sHtml = "";
                var sCaption = $ektron.htmlEncode(returnValue.sCaption);
                oFieldElem = editor.sfInstance.getSelectedField();

                sHtml = "<img src=\"" + returnValue.sFilename + "\" alt=\"" + sCaption + "\" title=\"" + sCaption + "\" />";
                insertContent(editor, sHtml, sTitle, oFieldElem);
            }
        }
    };

    RadEditorCommandList["EkHtmlTextField"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgsWTree(editor, oFieldElem);
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/htmltextfield.aspx"
		, args
		, 620
		, 500
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };
    RadEditorCommandList["EkHtmlChoicesField"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgs(editor, oFieldElem);
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
        editor.ekParameters.srcPath + "dialogs/htmlchoicesfield.aspx"
        , args
        , 480
        , 570
        , callback
        , null
        , sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };
    RadEditorCommandList["EkHtmlCalendarField"] = function (commandName, editor, oTool)
    {
        // no events firing off at the text area in RADEDITOR_HTML_MODE to disable this toolbar in setToolState
        if (editor.Mode != RadEditorNamespace.RADEDITOR_DESIGN_MODE) return;

        var oFieldElem = editor.sfInstance.getSelectedField();
        var args = createCmdArgsWTree(editor, oFieldElem, null, "date");
        var sTitle = editor.Localization[commandName];
        editor.ShowDialog(
		editor.ekParameters.srcPath + "dialogs/htmlcalendarfield.aspx"
		, args
		, 620
		, 440
		, callback
		, null
		, sTitle);
        hideContextMenu();
        return false;

        function callback(returnValue)
        {
            insertContent(editor, returnValue, sTitle, oFieldElem);
        }
    };

    // Edit In Context
    RadEditorCommandList["EkInContextSave"] = function (commandName, editor, oTool)
    {
        var err = editor.validateDesign();
        if (err != null)
        {
            var checkLevel = editor.ekParameters.AccessChecks.toLowerCase();
            if ("enforce" == checkLevel)
            {
                alert(createErrorString(editor, err));
                return false;
            }
            else if ("warn" == checkLevel)
            {
                if (false == confirm(createErrorString(editor, err) + "\n\n" + editor.GetLocalizedString("sContinueSaving", sContinueSaving)))
                {
                    return false;
                }
            }
        }

        var strContent = editor.getContent();
        editor.destroyEditor();
        $ektron("#" + editor.Id + "_wrapper").parent().ajaxCallback(editor.uniqueCallbackId,
		$ektron.extend({ command: "save", content: strContent }, editor.callbackData));
    };
    // Edit In Context
    RadEditorCommandList["EkInContextCancel"] = function (commandName, editor, oTool)
    {
        editor.destroyEditor();
        $ektron("#" + editor.Id + "_wrapper").parent().ajaxCallback(editor.uniqueCallbackId,
		$ektron.extend({ command: "cancel" }, editor.callbackData));
    };
    //#64940: for removing all styles attributes in the entire content
    RadEditorCommandList["EkRemoveStyles"] = function (commandName, editor, oTool)
    {
        editor.removeStyles();
    };
    // Toggle placeholder icons such as bookmarks, custom tags, paragraph spacing etc.
    RadEditorNamespace.EktronContentUsabilityCommand =
    {
        New: function ()
        {
            //Call parent initializer
            var obj = RadEditorNamespace.RadCommandBase.New(null, false);
            RadEditorNamespace.Utils.ExtendObject(obj, this); //Assign props and functions!
            return obj;
        },

        GetState: function (oWindow)
        {
            return oWindow.ektronContentUsability ? RadEditorNamespace.RADCOMMAND_STATE_ON : RadEditorNamespace.RADCOMMAND_STATE_OFF;
        }
    };

    RadEditorNamespace.UpdateCommandsArray["EkInsertRemoveTempMarkers"] = RadEditorNamespace.EktronContentUsabilityCommand.New();
    RadEditorCommandList["EkInsertRemoveTempMarkers"] = function (commandName, editor, oTool)
    {
        var containingElement = editor.sfInstance.getContentElement();
        var doc = containingElement.ownerDocument;
        var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
        win.ektronContentUsability = (win.ektronContentUsability ? false : true);
        if (win.ektronContentUsability)
        {
            Ektron.SelectionRange.ContentUsability.add(containingElement);
        }
        else
        {
            Ektron.SelectionRange.ContentUsability.remove(containingElement);
        }
    };

    RadEditorCommandList["EkHideShowElements"] = function (commandName, editor, oTool)
    {
        var containingElement = editor.sfInstance.getContentElement();
        var doc = containingElement.ownerDocument;
        var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
        win.ektronHideShowElementState = (win.ektronHideShowElementState ? false : true);
        if (win.ektronHideShowElementState)
        {
            $ektron("a[name]", containingElement).addClass("design_bookmark");
            $ektron(".design_fixedsize_placeholder", containingElement).removeClass("design_placeholder-hidden");
            $ektron("input[ektdesignns_hidden='true']", containingElement).removeClass("design_placeholder-hidden");
            oTool.SetState(RadEditorNamespace.RADCOMMAND_STATE_ON, true);
        }
        else
        {
            $ektron("a[name]", containingElement).removeClass("design_bookmark");
            $ektron(".design_fixedsize_placeholder", containingElement).addClass("design_placeholder-hidden");
            $ektron("input[ektdesignns_hidden='true']", containingElement).addClass("design_placeholder-hidden");
            oTool.SetState(RadEditorNamespace.RADCOMMAND_STATE_OFF, true);
        }
    };
})();                                              // namespaced
