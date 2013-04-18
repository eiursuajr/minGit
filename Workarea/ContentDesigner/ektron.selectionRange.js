if (!Ektron.SelectionRange) (function() 
{
	// We don't need the precision of Ektron.RegExp.CharacterClass.s, so just use \s for better performance.
	Ektron.RegExp.IsIndentWhitespace = new RegExp("^[ \\t\\r\\n]+$"); // whitespace between tags to indent tags
	Ektron.RegExp.IsBlankWhitespace = new RegExp("^(\\s|\xa0|&nbsp;|&#160;)+$", "i");

	Ektron.SelectionRange = function SelectionRange(settings)
	{
		var m_settings = $ektron.extend(
		{
			window: window,
			onexception: null
		}, settings);
		this.onexception = m_settings.onexception;
		
		var m_selection = null;
		var m_range = null;
		
		this.collapse = SelectionRange_collapse;
		this.getContainerElement = SelectionRange_getContainerElement;
		this.getStartElement = SelectionRange_getStartElement;
		this.getEndElement = SelectionRange_getEndElement;
		this.getElements = SelectionRange_getElements;
		this.indent = SelectionRange_indent;
		this.outdent = SelectionRange_outdent;
		this.orderedList = SelectionRange_orderedList;
		this.unorderedList = SelectionRange_unorderedList;

		this.getDomSelection = function SelectionRange_getDomSelection()
		{
			if (m_selection) return m_selection;
			try
			{
				var win = m_settings.window;
				if (win.getSelection)
				{
					m_selection = win.getSelection();
				}
				else if (win.document)
				{
					m_selection = win.document.selection;
				}
			}
			catch (ex)
			{
				return Ektron.OnException(this, Ektron.OnException.returnValue(null), ex, arguments);
			}
			return m_selection;
		};
		
		this.getDomRange = function SelectionRange_getDomRange()
		{
			if (m_range) return m_range;
			try
			{
				var sel = this.getDomSelection();
				if (sel.createRange) // IE
				{
					try
					{
						m_range = sel.createRange();
					}
					catch (ex)
					{
						sel.clear();
						m_range = sel.createRange();
					}
				}
				else if (sel.getRangeAt && sel.rangeCount > 0) // Mozilla, Opera
				{
					m_range = sel.getRangeAt(0);
				}
				else // Safari
				{
					var win = m_settings.window;
					m_range = win.document.createRange();
				}
			}
			catch (ex)
			{
				return Ektron.OnException(this, Ektron.OnException.returnValue(null), ex, arguments);
			}
			return m_range;
		};
		
		this.moveToNode = function SelectionRange_moveToNode(node)
		{
			var bDone = false;
			try
			{
				$ektron(node).each(function()
				{
					if (/* TEXT_NODE */ 3 == this.nodeType && !Ektron.RegExp.IsIndentWhitespace.test(this.nodeValue))
					{
						var doc = this.ownerDocument;
						if (doc.createRange)
						{
							m_range = doc.createRange();
							// selectNodeContents will actually selected the TEXT_NODE, whereas selectNode selects the containing element
							m_range.selectNodeContents(this);
							m_range.collapse(true);
							var sel = doc.defaultView.getSelection();
							sel.removeAllRanges();							
							sel.addRange(m_range);
						}
						else if (doc.body.createTextRange)
						{
							var me = this;
							var parent = this.parentNode;
							m_range = doc.body.createTextRange();
							m_range.moveToElementText(parent);
							m_range.collapse(true);
							m_range.move("character", 1);
							m_range.move("character", -1);
							$ektron(parent).contents().each(function()
							{
								if (me == this) return false; // break
								if (/* TEXT_NODE */ 3 == this.nodeType)
								{
									m_range.move("character", this.nodeValue.length);
								}
							});
							m_range.select();
						}
						bDone = true;
					}
					else if (/* ELEMENT_NODE */ 1 == this.nodeType)
					{
						// recurse to process child nodes
						$ektron(this).contents().each(arguments.callee);
					}
					if (bDone) return false; // break
				});
				if (!bDone)
				{
					Ektron.ContentDesigner.trace("Failed to move cursor to element.");
				}
			}
			catch (ex)
			{
				return Ektron.OnException(this, null, ex, arguments);
			}
		};
		
		this.execCommand = function SelectionRange_execCommand(commandName, UIFlag, value)
		{
			try
			{
				var bUIFlag = (true == UIFlag);
				if ("undefined" == typeof value) value = "";
				return m_settings.window.document.execCommand(commandName, bUIFlag, value);
			}
			catch (ex)
			{
				return Ektron.OnException(this, Ektron.OnException.returnValue(false), ex, arguments);
			}
		};
		
	}; // end class
	Ektron.SelectionRange.onexception = Ektron.OnException.diagException;	
		
	function SelectionRange_collapse(toStart)
	{
		toStart = (true == toStart); // ensure boolean
		try
		{
			var sel = this.getDomSelection();
			if (sel && typeof sel.collapseToStart != "undefined")
			{
				if (!sel.isCollapsed)
				{		
					if (toStart)
					{
						sel.collapseToStart();
					}
					else
					{				
						sel.collapseToEnd();
					}				
				}
			}
			else
			{
				var rng = this.getDomRange();
				if (rng && typeof rng.collapse != "undefined")
				{
					rng.collapse(toStart);
					rng.select();
				}
			}
		}
		catch (ex)
		{
			return Ektron.OnException(this, null, ex, arguments);
		}
	}
	
	function SelectionRange_getContainerElement()
	{
		var elem = null;
		try
		{
			var range = this.getDomRange();
			if (range.commonAncestorContainer) // Mozilla, Chrome, Safari, Opera
			{
				elem = range.commonAncestorContainer;
				if (elem.nodeType != 1) elem = elem.parentNode;
			}
			else if (range.parentElement) // IE TextRange
			{
				elem = range.parentElement();
				if ("LI" == elem.tagName)
				{
					// Correct parentElement() to include OL,UL if all LI are selected
					// seems OK with DT and DD
					var rngElem = elem.ownerDocument.body.createTextRange();
					rngElem.moveToElementText(elem);
					while (range.inRange(rngElem) && !range.isEqual(rngElem))
					{
						elem = elem.parentNode;
						rngElem.moveToElementText(elem);
					}
				}
			}
			else if (range.commonParentElement) // IE ControlRange
			{
				elem = range.commonParentElement();
			}
		}
		catch (ex)
		{
			return Ektron.OnException(this, Ektron.OnException.returnValue(null), ex, arguments);
		}
		return elem;
	}
	
	function SelectionRange_getStartElement()
	{
		var elem = null;
		try
		{
			var range = this.getDomRange();
			if (range.startContainer) // Mozilla, Chrome, Safari, Opera
			{
				elem = range.startContainer;
				if (elem.nodeType != 1) elem = elem.parentNode;
			}
			else if (range.parentElement) // IE TextRange
			{
				var rng = range.duplicate();
				rng.collapse(true);
				elem = rng.parentElement();
			}
			else if (range.item && range.length > 0) // IE ControlRange
			{
				elem = range.item(0);
			}
		}
		catch (ex)
		{
			return Ektron.OnException(this, Ektron.OnException.returnValue(null), ex, arguments);
		}
		return elem;
	}
	
	function SelectionRange_getEndElement()
	{
		var elem = null;
		try
		{
			var range = this.getDomRange();
			if (range.endContainer) // Mozilla, Chrome, Safari, Opera
			{
				elem = range.endContainer;
				if (elem.nodeType != 1) elem = elem.parentNode;
			}
			else if (range.parentElement) // IE TextRange
			{
				var rng = range.duplicate();
				rng.collapse(false);
				elem = rng.parentElement();
			}
			else if (range.item && range.length > 0) // IE ControlRange
			{
				elem = range.item(range.length - 1);
			}
		}
		catch (ex)
		{
			return Ektron.OnException(this, Ektron.OnException.returnValue(null), ex, arguments);
		}
		return elem;
	}

	function SelectionRange_getElements(tags)
	{
		var objElems = null; // returns a $ektron instance
		try
		{
			var range = this.getDomRange();
			var containerElem = this.getContainerElement();
			var bIsListItemTags = (mc_oListItemTags == tags);
			var rngElem = null;
			var rngStart = null;
			var rngEnd = null;
			if (range.inRange) // IE
			{
				rngElem = containerElem.ownerDocument.body.createTextRange();
				rngStart = range.duplicate();
				rngStart.collapse(true);
				rngEnd = range.duplicate();
				rngEnd.collapse(false);
			}
			// Must find("*"); can't find(tags) or filter(tags) b/c DOM order will change
			$ektron(containerElem).find("*").each(function()
			{
				try
				{
					if (!(this.tagName in tags)) return;
					var bAddElem = false;
					if (range.isPointInRange) // Mozilla, Safari, Chrome, not Opera
					{
						if (bIsListItemTags) 
						{
							// If beginning of element is in range
							if (range.isPointInRange(this, 0))
							{
								var parent = this.parentNode;
								var bParentStartInRange = range.isPointInRange(parent, 0);
								var bParentEndInRange = range.isPointInRange(parent, parent.childNodes.length);
								// Add if containerElem is not completely within range
								if (!bParentStartInRange || !bParentEndInRange)
								{
									bAddElem = true;
								}
							}
						}
						else
						{
							// If any part of element is in range
							if (range.isPointInRange(this, 0) || (this.childNodes && range.isPointInRange(this, this.childNodes.length)))
							{
								var parent = this.parentNode;
								var bParentStartInRange = range.isPointInRange(parent, 0);
								var bParentEndInRange = range.isPointInRange(parent, parent.childNodes.length);
								// Add if parent completely encloses range
								if (!bParentStartInRange && !bParentEndInRange)
								{
									bAddElem = true;
								}
							}
						}
					}
					else if (range.inRange) // IE
					{
						if (bIsListItemTags)
						{
							rngElem.moveToElementText(this);
							rngElem.collapse(true);
							// If beginning of element is in range
							if (range.inRange(rngElem))
							{
								var parent = this.parentNode;
								rngElem.moveToElementText(parent);
								// Add if containerElem is not completely within range
								if (!range.inRange(rngElem) || parent == containerElem)
								{
									bAddElem = true;
								}
							}
						}
						else
						{
							rngElem.moveToElementText(this);
							// If any part of element is in range
							var bContainsStart = rngElem.inRange(rngStart);
							var bContainsEnd = rngElem.inRange(rngEnd);
							if (range.inRange(rngElem) || (bContainsStart && !bContainsEnd) || (!bContainsStart && bContainsEnd))
							{
								var parent = this.parentNode;
								rngElem.moveToElementText(parent);
								// Add if parent completely encloses range
								if (rngElem.inRange(range))
								{
									bAddElem = true;
								}
							}
						}
					}
					if (bAddElem)
					{
						if (null == objElems)
						{
							objElems = $ektron(this);
						}
						else
						{
							objElems = objElems.add(this);
						}
					}
				}
				catch (ex)
				{
					Ektron.SelectionRange.onexception(ex, arguments);
				}
			});
		}
		catch (ex)
		{
			return Ektron.OnException(this, Ektron.OnException.returnValue(null), ex, arguments);
		}
		return objElems;
	}

	function SelectionRange_indent()
	{
		try
		{
			var startElem = this.getStartElement();
			var endElem = this.getEndElement();
			var startTag = "";
			var endTag = "";
			var startObj = null;
			var endObj = null;

			if (!startElem || !endElem) return false;
			
			var bodyElem = startElem.ownerDocument.body;
			
			// Find elements which are relevant to indent
			startObj = $ektron(startElem).closest(mc_sIndentableTags);
			startObj = p_preferListItem(startObj);
			if (startElem != endElem)
			{
				endObj = $ektron(endElem).closest(mc_sIndentableTags);
				endObj = p_preferListItem(endObj);
			}
			else
			{
				endObj = startObj;
			}
			
			startElem = startObj.get(0);
			startTag = startElem ? startElem.tagName : "";
			endElem = endObj.get(0);
			endTag = endElem ? endElem.tagName : "";
			
			// Ignore table cells unless only one table cell is selected
			if (startElem != endElem)
			{
				if (startTag in mc_oTableCellTags)
				{
					startObj = startObj.closest("table");
					startElem = startObj.get(0);
					startTag = startElem ? startElem.tagName : "";
				}
				if (endTag in mc_oTableCellTags)
				{
					endObj = endObj.closest("table");
					endElem = endObj.get(0);
					endTag = endElem ? endElem.tagName : "";
				}
			}

			if (startElem != endElem)
			{
				if ((startTag in mc_oBlockTags) || (endTag in mc_oBlockTags))
				{
					// BLOCK tags: wrap with BLOCKQUOTE
					var objElems = this.getElements(mc_oBlockTags);
					if (objElems && objElems.length > 0)
					{
						objElems.wrapAll("<blockquote></blockquote>");
						startElem = objElems[0];
					}
					else
					{
						Ektron.SelectionRange.onexception(new Error("No block elements in selection range."), arguments);
					}
				}
				else if ((startTag in mc_oListItemTags) && (endTag in mc_oListItemTags))
				{
					// LIST ITEM tags: nest
					var objElems = startObj.add(this.getElements(mc_oListItemTags)).add(endObj);
					p_nestListItems(objElems);
				}
				else
				{
					startElem = bodyElem;
					$ektron(bodyElem).wrapInner("<blockquote></blockquote>");
				}
			}
			else // only one element selected
			{
				if (startTag in mc_oBlockTags)
				{
					if ("true" == startElem.getAttribute("contenteditable"))
					{
						// Editable BLOCK tags: wrap contents with BLOCKQUOTE	
						startObj.wrapInner("<blockquote></blockquote>");
					}
					else
					{
						// BLOCK tags: wrap with BLOCKQUOTE	
						startObj.wrap("<blockquote></blockquote>");
					}
				}
				else if (startTag in mc_oListItemTags)
				{
					// LIST ITEM tags: nest
					p_nestListItems(startObj);
				}
				else if (startTag in mc_oTableCellTags)
				{
					// TD, TH: wrap contents with BLOCKQUOTE
					startObj.wrapInner("<blockquote></blockquote>");
				}
				else
				{
					startElem = bodyElem;
					$ektron(bodyElem).wrapInner("<blockquote></blockquote>");
				}
			}
			if (startElem) this.moveToNode(startElem);
		}
		catch (ex)
		{
			return Ektron.OnException(this, Ektron.OnException.returnValue(false), ex, arguments);
		}
		return true;
	}
	
	function SelectionRange_outdent()
	{
		try
		{
			var startElem = this.getStartElement();
			var endElem = this.getEndElement();
			var startObj = null;
			var endObj = null;
			var startTag = "";
			var endTag = "";
			
			if (!startElem || !endElem) return false;
			
			// Find elements which are relevant to indent
			startObj = $ektron(startElem).closest(mc_sOutdentableTags);
			if (startElem != endElem)
			{
				endObj = $ektron(endElem).closest(mc_sOutdentableTags);
			}
			else
			{
				endObj = startObj;
			}
			
			startElem = startObj.get(0);
			startTag = startElem ? startElem.tagName : "";
			endElem = endObj.get(0);
			endTag = endElem ? endElem.tagName : "";
			
			if ((startTag in mc_oListItemTags) && (endTag in mc_oListItemTags))
			{
				// LIST ITEM tags: nest
				var objElems = startObj.add(this.getElements(mc_oListItemTags)).add(endObj);
				var objUnnestElemt = p_unnestListItems(objElems).eq(0);
				if (0 == objUnnestElemt.contents().length)
				{
				    objUnnestElemt.append("&#160;");
				}
				var childNode = objUnnestElemt.get(0);
				if (childNode) this.moveToNode(childNode);
			}
			else
			{
				var objElems = this.getElements(mc_oBlockquoteTags);
				if (!objElems || 0 == objElems.length)
				{
					if ((startTag in mc_oBlockquoteTags) && (endTag in mc_oBlockquoteTags))
					{
						objElems = startObj.add(endObj);
					}
					else if (startTag in mc_oBlockquoteTags)
					{
						objElems = startObj;
					}
					else if (endTag in mc_oBlockquoteTags)
					{
						objElems = endObj;
					}
				}
				if (objElems && objElems.length > 0)
				{
					// BLOCKQUOTE tags: unwrap
					// #49568 - Indent groups paragraphs together making outdent of a single paragraph difficult
					return this.execCommand(RadEditorNamespace.RADCOMMAND_OUTDENT);
					//var childNode = objElems.unwrapInner().get(0);
					//if (childNode) this.moveToNode(childNode);
				}
			}
		}
		catch (ex)
		{
			return Ektron.OnException(this, Ektron.OnException.returnValue(false), ex, arguments);
		}
		return true;
	}
	
	function SelectionRange_orderedList()
	{
		try
		{
			if (!p_changeListType.call(this, "UL", "OL"))
			{
				this.execCommand(RadEditorNamespace.RADCOMMAND_INSERT_ORDERED_LIST);
				if ($ektron.browser.mozilla && parseInt($ektron.browser.version, 10) >= 2 && parseInt($ektron.browser.version, 10) < 5)
				{
				    p_removeFF4BrList(this);
				}
				return false;
			}
		}
		catch (ex)
		{
			return Ektron.OnException(this, Ektron.OnException.returnValue(false), ex, arguments);
		}
		return true;
	}
	
	function SelectionRange_unorderedList()
	{
		try
		{
			if (!p_changeListType.call(this, "OL", "UL"))
			{
				this.execCommand(RadEditorNamespace.RADCOMMAND_INSERT_UNORDERED_LIST);
				if ($ektron.browser.mozilla && parseInt($ektron.browser.version, 10) >= 2 && parseInt($ektron.browser.version, 10) < 5)
				{
				    p_removeFF4BrList(this);
				}
				return false;
			}
		}
		catch (ex)
		{
			return Ektron.OnException(this, Ektron.OnException.returnValue(false), ex, arguments);
		}
		return true;
	}
	
	function p_removeFF4BrList(objSelRng)
	{
	    var oRng = objSelRng.getDomRange();
        var oElem = objSelRng.getContainerElement();
	    var strHtml = oElem.innerHTML;
	    $ektron(oElem).html(strHtml.replace(/[<]li[^>]*[>]\s*\t*[<]br[^>]*[>]\s*[<]\/\s*li[>]/gi, ""));
	}
	
	function p_changeListType(fromTag, toTag)
	// p_changedListType.call with object b/c it depends on 'this'
	// return true if changed, false means to run default behavior
	{
		var bChanged = false;
		try
		{
			// #43764 - Changing nested numbered list bulleted list generates invalid XHTML because UL is child of OL
			var startElem = this.getStartElement();
			var endElem = this.getEndElement();				
			if (!startElem || !endElem) return false;
			
			// Find elements which are relevant to indent
			var objList = $ektron(startElem).closest("OL,UL");
			var listElem = objList.get(0);
			if (!listElem) return false;
			if (startElem != endElem)
			{
				endElem = $ektron(endElem).closest("OL,UL").get(0);
				if (listElem != endElem) return false;
			}
			
			var strListTag = listElem ? listElem.tagName : "";
			if (fromTag == strListTag)
			{
				// change list type, losing attributes
				var objNewList = $ektron(listElem.ownerDocument.createElement(toTag));
				objNewList.append(objList.contents());
				objList.after(objNewList).remove();
				this.moveToNode(startElem);
				bChanged = true;
			}
		}
		catch (ex)
		{
			return Ektron.OnException(this, Ektron.OnException.returnValue(false), ex, arguments);
		}
		return bChanged;
	}
	
	
	function p_preferListItem(obj)
	{
		try
		{
			if (obj.is(mc_sBlockTags))
			{
				var objItem = obj.closest(mc_sListItemTags);
				if (1 == objItem.length)
				{
					// If no text nodes between list item and blocking tag, then choose list item
					var bDone = false;
					var bFoundText = false;
					var objElemBlock = obj.get(0);
					objItem.each(function()
					{
						if (objElemBlock == this)
						{
							bDone = true;
						}
						else if (/* TEXT_NODE */ 3 == this.nodeType)
						{
							bFoundText = true;
							bDone = true;
						}
						else if (/* ELEMENT_NODE */ 1 == this.nodeType)
						{
							// recurse to process child nodes
							$ektron(this).contents().each(arguments.callee);
						}
						if (bDone) return false; // break
					});
					if (!bFoundText)
					{
						obj = objItem;
					}
				}
			}
		}
		catch (ex)
		{
			Ektron.SelectionRange.onexception(ex, arguments);
		}
		return obj;
	}
	
	function p_nestListItems(objListItems)
	{
		// objListItems is a $ektron object
		return objListItems.each(function nestListItem()
		{
			try
			{
				var objItem = $ektron(this);
				var objList = objItem.parent();
				var sItemTagName = this.tagName;
				var sListTagName = this.parentNode.tagName;
				var objPrev = objItem.prev();
				if (objPrev && 1 == objPrev.length)
				{
					if (0 == objItem.contents().length)
					{
					    objItem.append("&#160;");
					}
					var objPrevChild = objPrev.children(":last");
					if (objPrevChild.is(mc_oListItemTags[sItemTagName]))
					{
						// merge item into nested list in preceding sibling
						objPrevChild.append(objItem);
					}
					else
					{
						// create a new nested list and append to preceding sibling
						var newList = $ektron(this.parentNode.cloneNode(false)); // $ektron(selector).clone() is deep, we need shallow
						newList.removeAttr("id");
						objPrev.append(newList.append(objItem));
					}
				}
				else
				{
					// item is first in list
					var newItem = $ektron(this.cloneNode(false)); // $ektron(selector).clone() is deep, we need shallow
					newItem.removeAttr("id");
					var newList = $ektron(this.parentNode.cloneNode(false)); // $ektron(selector).clone() is deep, we need shallow
					newList.removeAttr("id");
					objList.prepend(newItem.append("&#160;").append(newList.append(objItem)));
				}
			}
			catch (ex)
			{
				Ektron.SelectionRange.onexception(ex, arguments);
			}
		});
	}
	
	function p_unnestListItems(objListItems)
	{
		// objListItems is a $ektron object
		var ret = [];
		objListItems.each(function unnestListItem()
		{
			try
			{
				var objItem = $ektron(this);
				var objList = objItem.parent();
				var sItemTagName = this.tagName;
				var objNextOuterListItem = objList.closest(sItemTagName);
				if (objNextOuterListItem && 1 == objNextOuterListItem.length)
				{
					// nest following siblings
					p_nestListItems(objItem.nextAll());
					objItem.insertAfter(objNextOuterListItem);
					ret = ret.concat(this);
				}
				else
				{
					var objItemContents = objItem.contents();
					// nest following siblings
					p_nestListItems(objItem.empty().nextAll());
					objList.after(objItem.contents());
					objItem.remove();
					objItem = objList;
					var objPara = null;
					objItemContents.each(function()
					{
						var objContent = $ektron(this);
						if (objContent.is(mc_sBlockTags))
						{
							if (objPara)
							{
								objItem.after(objPara);
								ret = ret.concat(objPara.get(0));
								objItem = objPara;
								objPara = null;
							}
							objItem.after(objContent);
							ret = ret.concat(this);
							objItem = objContent;
						}
						else
						{
							if (!objPara)
							{
								objPara = $ektron("<p></p>", this.ownerDocument);
							}
							objPara.append(objContent);
							// Delete P if empty
							if (p_isBlank(objPara))
							{
								objPara.remove();
								objPara = null;
							}
						}
					});
					if (objPara)
					{
						objItem.after(objPara);
						ret = ret.concat(objPara.get(0));
					}
				}
				// Delete OL/UL if empty
				if (0 == objList.children().length)
				{
					objList.remove();
				}
				if (objNextOuterListItem && 1 == objNextOuterListItem.length)
				{
					// Delete parent LI if empty
					if (p_isBlank(objNextOuterListItem))
					{
						objNextOuterListItem.remove();
						objNextOuterListItem = null;
					}
				}
			}
			catch (ex)
			{
				Ektron.SelectionRange.onexception(ex, arguments);
			}
		});
		return objListItems.pushStack(ret);
	}
	
	function p_isBlank(objItem)
	{
		var bIsBlank = false;
		try
		{
			if (objItem.contents().length <= 1)
			{
				var strText = objItem.text();
				bIsBlank = Ektron.RegExp.IsBlankWhitespace.test(strText);
			}
		}
		catch (ex)
		{
			Ektron.SelectionRange.onexception(ex, arguments);
		}
		return bIsBlank;
	}
		

    Ektron.SelectionRange.isBlockTag = function isBlockTag(sTagName)
    {
        if (!sTagName) return false;
        return (sTagName in mc_oBlockTags);
    };
    
    
    Ektron.SelectionRange.ContentUsability =
    {
		paraMarker: "",
		textMarker: "",
		eolMarker: "",
		localize: function localizeContentUsability(objResource)
		{
			this.paraMarker = Ektron.String.format("<p class=\"contentUsability\" title=\"{0}\">&#160;</p>", 
								objResource.GetLocalizedString("sTempPara", "temporary paragraph, click here to add a new paragraph")); 
			this.textMarker = Ektron.String.format("<span class=\"contentUsability\" title=\"{0}\">&#160;</span>", 
								objResource.GetLocalizedString("sTempSpace", "temporary space, click here to type"));
			// fyi: zero-width space = "&#8203;"
			// don't use SPAN at end of line to reduce possibility of pasting into temporary SPAN tag
			this.eolMarker = "&#160;";
		},
		add: function addContentUsability(containingElement)
		{
			p_ensureCursorSelectability(containingElement);
			$ektron(".contentUsability", containingElement)
				.addClass("contentUsability-nohover")
				.mouseenter(p_contentUsabilityHover)
				.mouseleave(p_contentUsabilityRefresh)
				.click(p_contentUsabilityMakePermanent);
			this.recurseFrames(arguments);
		},
		remove: function removeContentUsability(containingElement)
		{
			$ektron("p.contentUsability", containingElement).each(function()
			{
				var $this = $ektron(this);
				if (1 == $this.contents().length && "\xA0" == $this.text())
				{
					$this.remove();
				}
				else
				{
					$this.removeAttr("class");
				}
			});
			$ektron("span.contentUsability", containingElement).each(function()
			{
				var $this = $ektron(this);
				if (1 == $this.contents().length && "\xA0" == $this.text())
				{
					$this.remove();
				}
				else
				{
					$this.unwrapInner();
				}
			});
			this.recurseFrames(arguments);
		},
		recurseFrames: function recurseFramesContentUsability(args)
		{
			if ($ektron.browser.mozilla)
			{
				var containingElement = args[0];
				var doc = containingElement.ownerDocument;
				var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
				for (var i = 0; i < win.frames.length; i++)
				{
					try
					{
						win.frames[i].ektronContentUsability = win.ektronContentUsability;
						args.callee.call(this, win.frames[i].document.body);
					} catch (ex) { }
				}
			}
		},
		wrapBodyText: function wrapBodyText(containingElement)
		{
    		p_wrapBodyText(containingElement);
			$ektron("div.ektdesignns_richarea, div.design_richarea", containingElement).each(function()
			{
				p_wrapBodyText(this);
			});
		}
	};
	
    Ektron.SelectionRange.ensureContentUsability = function(containingElement)
    {
		var doc = containingElement.ownerDocument;
		var win = (doc.defaultView ? doc.defaultView : doc.parentWindow);
		Ektron.SelectionRange.ContentUsability.wrapBodyText(containingElement);
		if (win.ektronContentUsability)
		{
			Ektron.SelectionRange.ContentUsability.add(containingElement);
		}
		else
		{
			Ektron.SelectionRange.ContentUsability.remove(containingElement);
		}
    }
	
    function p_contentUsabilityHover(ev)
    {
		$ektron(this).addClass("contentUsability-hover").removeClass("contentUsability-nohover");
    }

    function p_contentUsabilityRefresh(ev)
    {
		var $this = $ektron(this);
		if (p_isBlank($this))
        {
			$this.removeClass("contentUsability-hover").addClass("contentUsability-nohover");
        }
        else
        {
			p_contentUsabilityMakePermanent(ev);
        }
    }
    
    function p_contentUsabilityMakePermanent(ev)
    {
		var $this = $ektron(this);
        $this
			.unbind("mouseenter", p_contentUsabilityHover)
			.unbind("mouseleave", p_contentUsabilityRefresh)
			.unbind("click", p_contentUsabilityMakePermanent)
			.removeClass("contentUsability-hover")
			.removeClass("contentUsability-nohover")
			.removeClass("contentUsability")
			.removeAttr("title");
			
		if ("SPAN" == this.tagName)
		{
			$this.unwrapInner(); // remove SPAN tag, hopefully, but it could fail
		}
    }

    function p_ensureCursorSelectability(contentElement)
	{
		// We need to ensure the cursor may be placed throughout the document to workaround limitations in the browsers.
		// See also ContentOutGoing.xslt, which removes what is added here.
		if (!contentElement) return;
		var me = this;
		var paraMarker = Ektron.SelectionRange.ContentUsability.paraMarker;
		var textMarker = Ektron.SelectionRange.ContentUsability.textMarker;
		var eolMarker = Ektron.SelectionRange.ContentUsability.eolMarker;

		// Before and after block control elements
		// #35165: FF needs a non-breaking space for a cursor to be clicked into before and after a form object.
		$ektron("div,table,fieldset,form", contentElement).each(function()
		{
			try
			{
				if (!p_isBlockControlSiblingCursorSelectable(this.previousSibling, false))
				{
					if ($ektron.isEditableElement(this.parentNode))
					{
						$ektron(this).before(paraMarker);
					}
				}
				if ($ektron.isEditableElement(this.parentNode) && !$ektron(this).hasClass("design_iframe_membrane"))
				{
					if ($ektron.isEditableElement(this.parentNode))
					{
						$ektron(this).after(paraMarker);
					}
				}
			}
			catch (ex)
			{
				Ektron.OnException(me, Ektron.ContentDesigner.onexception, ex, arguments);
			}
		});

		function p_ensureParagraphTopAndBottom()
		{
			try
			{
				if ($ektron.isEditableElement(this))
				{
					// Ensure paragraph at top
					if (!p_isChildCursorSelectable(this.firstChild, true))
					{
						$ektron(this).prepend(paraMarker);
					}
					// Ensure paragraph at bottom (IE seems to handle this OK, but we'll do it consistently for all browsers)
					if (!p_isChildCursorSelectable(this.lastChild, false))
					{
						$ektron(this).append(paraMarker);
					}
				}
			}
			catch (ex)
			{
				Ektron.OnException(me, Ektron.ContentDesigner.onexception, ex, arguments);
			}
		}
		
		if ($ektron.browser.mozilla)
		{
			p_ensureParagraphTopAndBottom.call(contentElement);
		}
		else
		{
			$ektron("div.ektdesignns_richarea, div.design_richarea", contentElement).andSelf().each(p_ensureParagraphTopAndBottom);
		}
		
		// Ensure text nodes at critical places within tags for cursor to select.
		// In the beginning and end of container elements.
		$ektron("p,div,li,th,td", contentElement).each(function()
		{
			try
			{
				if (!p_isChildCursorSelectable(this.lastChild, false))
				{
					if ($ektron.isEditableElement(this))
					{
						$ektron(this).append(eolMarker); // end-of-line marker
					}
				}
				if (!p_isChildCursorSelectable(this.firstChild, true))
				{
					if (!$ektron(this).hasClass("design_richarea") && $ektron.isEditableElement(this))
					{
						$ektron(this).prepend(textMarker);
					}
				}
			}
			catch (ex)
			{
				Ektron.OnException(me, Ektron.ContentDesigner.onexception, ex, arguments);
			}
		});
		
		// Before and after inline elements, excluding b, i, u, etc. which are very easy to unapply
		$ektron("a,label", contentElement).each(function()
		{
			try
			{
				if (null == this.previousSibling || (this.previousSibling.nodeType != 3/*TEXT_NODE*/ && !$ektron(this.previousSibling).hasClass("contentUsability")))
				{
					if ($ektron.isEditableElement(this.parentNode))
					{
						$ektron(this).before(textMarker);
					}
				}
				if (null == this.nextSibling || (this.nextSibling.nodeType != 3/*TEXT_NODE*/ && !$ektron(this.previousSibling).hasClass("contentUsability")))
				{
					if ($ektron.isEditableElement(this.parentNode))
					{
						$ektron(this).after(textMarker);
					}
				}
			}
			catch (ex)
			{
				Ektron.OnException(me, Ektron.ContentDesigner.onexception, ex, arguments);
			}
		});
		
		// Before and after inline control elements.
		$ektron("input,select,textarea,button,img", contentElement).each(function()
		{
			try
			{
				if (!p_isInlineControlSiblingCursorSelectable(this.previousSibling, false))
				{
					if ($ektron.isEditableElement(this.parentNode))
					{
						$ektron(this).before(textMarker);
					}
				}
				if (!p_isInlineControlSiblingCursorSelectable(this.nextSibling, true))
				{
					if ($ektron.isEditableElement(this.parentNode))
					{
						$ektron(this).after(textMarker);
					}
				}
			}
			catch (ex)
			{
				Ektron.OnException(me, Ektron.ContentDesigner.onexception, ex, arguments);
			}
		});
	};
	
	function p_isChildCursorSelectable(node, bNextSibling)
	{
		if (null == node) return false;
		var bSelectable = false;
		switch (node.nodeType)
		{
			case /*ELEMENT_NODE*/1:
				bSelectable = ("P" == node.tagName || "DIV" == node.tagName || ("SPAN" == node.tagName && $ektron(node).hasClass("contentUsability")));
				break;
			case /*TEXT_NODE*/3:
				if ("" == node.nodeValue || Ektron.RegExp.IsIndentWhitespace.test(node.nodeValue)) 
				{
					node = (bNextSibling ? node.nextSibling : node.previousSibling);
					bSelectable = p_isChildCursorSelectable(node, bNextSibling);
				}
				else
				{
					bSelectable = true;
				}
				break;
			default:
				break;
		}
		return bSelectable;
	}
	
	function p_isInlineControlSiblingCursorSelectable(node, bNextSibling)
	{
		if (null == node) return false;
		var bSelectable = false;
		switch (node.nodeType)
		{
			case /*ELEMENT_NODE*/1:
				bSelectable = !Ektron.SelectionRange.isBlockTag(node.tagName);
				if (bSelectable) bSelectable = 
				(
					node.tagName != "INPUT" && 
					node.tagName != "SELECT" && 
					node.tagName != "TEXTAREA" && 
					node.tagName != "BUTTON" && 
					node.tagName != "IMG"
				);
				if (!bSelectable) 
				{
				    if (bNextSibling && "design_edit_fieldprop" == node.previousSibling.className)
				    {
				        bSelectable = true;
				    }
				    else if (!bNextSibling && "design_edit_fieldprop" == node.className)
				    {
				        bSelectable = true;
				    }
				}    
				break;
			case /*TEXT_NODE*/3:
				if ("" == node.nodeValue || Ektron.RegExp.IsIndentWhitespace.test(node.nodeValue)) 
				{
					node = (bNextSibling ? node.nextSibling : node.previousSibling);
					bSelectable = p_isInlineControlSiblingCursorSelectable(node, bNextSibling);
				}
				else
				{
					bSelectable = true;
				}
				break;
			default:
				break;
		}
		return bSelectable;
	}
	
	function p_isBlockControlSiblingCursorSelectable(node, bNextSibling)
	{
		if (null == node) return false;
		var bSelectable = false;
		switch (node.nodeType)
		{
			case /*ELEMENT_NODE*/1:
				bSelectable = 
				(
					node.tagName != "DIV" && 
					node.tagName != "TABLE" && 
					node.tagName != "FIELDSET" && 
					node.tagName != "FORM"
				);
				break;
			case /*TEXT_NODE*/3:
				if ("" == node.nodeValue || Ektron.RegExp.IsIndentWhitespace.test(node.nodeValue)) 
				{
					node = (bNextSibling ? node.nextSibling : node.previousSibling);
					bSelectable = p_isBlockControlSiblingCursorSelectable(node, bNextSibling);
				}
				else
				{
					bSelectable = true;
				}
				break;
			default:
				break;
		}
		return bSelectable;
	}
	
    function p_wrapBodyText(containingElement)
	{
		var me = this;
		var eWrappers = null;
		var aryNodes = [];
		var pendingNode = null;
		var bHasTextToWrap = false;
		var iFirstNode = -1;
		try
		{
			if (!containingElement || !containingElement.childNodes) return null;
			for (var i = 0; i < containingElement.childNodes.length; i++)
			{
				try
				{
					var objNode = containingElement.childNodes[i];
					// #48391 - Content Designer adds extra tags in the source in FireFox Browser
					if (/* TEXT_NODE */3 == objNode.nodeType)
					{
						var bIsWhitespace = Ektron.RegExp.IsIndentWhitespace.test(objNode.nodeValue);
						if (objNode.nodeValue != "" && !bIsWhitespace)
						{
							addNode(objNode);
							bHasTextToWrap = true;
						}
						else if (bIsWhitespace && aryNodes.length > 0)
						{
							pendingNode = objNode;
						}
					}
					else if (!Ektron.SelectionRange.isBlockTag(objNode.nodeName))
					{
						addNode(objNode);
					}
					else if (aryNodes.length > 0)
					{
						if (bHasTextToWrap)
						{
							i = iFirstNode;
							iFirstNode = -1;
							bHasTextToWrap = false;
							var wrapper = wrapAllNodes(aryNodes);
							if (wrapper) eWrappers = (null == eWrappers ? $ektron(wrapper) : eWrappers.add(wrapper));
						}
						aryNodes = [];
					}
					objNode = null;
				}
				catch (ex)
				{
					Ektron.OnException(me, Ektron.ContentDesigner.onexception, ex, arguments);
				}
			}
			if (aryNodes.length > 0)
			{
				if (bHasTextToWrap)
				{
					bHasTextToWrap = false;
					var wrapper = wrapAllNodes(aryNodes);
					if (wrapper) eWrappers = (null == eWrappers ? $ektron(wrapper) : eWrappers.add(wrapper));
				}
				aryNodes = [];
			}
		}
		catch (ex)
		{
			Ektron.OnException(me, Ektron.ContentDesigner.onexception, ex, arguments);
		}
		return eWrappers;
		
		function addNode(objNode)
		{
			try
			{
				if (-1 == iFirstNode) iFirstNode = i;
				if (pendingNode)
				{
					aryNodes.push(pendingNode);
					pendingNode = null;
				}
				aryNodes.push(objNode);
			} 
			catch (ex)
			{
				Ektron.OnException(me, Ektron.ContentDesigner.onexception, ex, arguments);
			}
		}
		
		function wrapAllNodes(aryNodes)
		{
			// Can't use jQuery wrapAll because it only wraps elements, not all types of nodes.
			// assert aryNodes && aryNodes.length > 0
			try
			{
				var oNode = aryNodes[0];
				var wrapper = oNode.ownerDocument.createElement("p");
				oNode.parentNode.insertBefore(wrapper, oNode);
				for (var i = 0; i < aryNodes.length; i++)
				{
					var objNode = aryNodes[i];
					if (/* TEXT_NODE */3 == objNode.nodeType)
					{
						// trim indent whitespace
						if (0 == i)
						{
							objNode.nodeValue = objNode.nodeValue.replace(/^[ \t\r\n]+/,"");
						}
						else if (i == aryNodes.length - 1)
						{
							objNode.nodeValue = objNode.nodeValue.replace(/[ \t\r\n]+$/,"");
						}
					}
					wrapper.appendChild(objNode);
				}
				return wrapper;
			} 
			catch (ex)
			{
				Ektron.OnException(me, Ektron.ContentDesigner.onexception, ex, arguments);
			}
			return null;
		}
	}
	

	function PropertyArray()
	{
		this.merge = function PropertyArray_merge()
		{
			for (var i = 0; i < arguments.length; i++)
			{
				var obj = arguments[i];
				if (typeof obj != "object") throw new TypeError("Arguments must be objects. Argument[" + i + "] = '" + typeof obj + "'");
				$ektron.extend(this, obj);
			}
			return this;
		};
		
		this.add = function PropertyArray_add()
		{
			for (var i = 0; i < arguments.length; i++)
			{
				var prop = arguments[i];
				if (typeof prop != "string") throw new TypeError("Arguments must be strings. Argument[" + i + "] = '" + typeof prop + "'");
				this[prop] = true;
			}
			return this;
		};
		
		this.empty = function PropertyArray_empty()
		{
			for (var p in this)
			{
				var val = this[p];
				if ("number" == typeof val || "string" == typeof val || "boolean" == typeof val)
				{
					delete this[p];
				}
			}
			return this;
		};
		
		this.join = function PropertyArray_join(separator)
		{
			var a = [];
			for (var p in this)
			{
				var val = this[p];
				if ("number" == typeof val || "string" == typeof val || ("boolean" == typeof val && val))
				{
					a.push(p);
				}
			}
			return a.join(separator);
		};
		
		this.toString = this.join;
	}
	
	var mc_oBlockTags = 
	{ 
		P: "*", DIV: "*", TABLE: "*", FIELDSET: "*", CENTER: "*",
		H1: "*", H2: "*", H3: "*", H4: "*", H5: "*", H6: "*", 
		BLOCKQUOTE: "*", CENTER: "*", HR: "*", PRE: "*", ADDRESS: "*",
		OL: "LI", UL: "LI", DIR: "LI", MENU: "LI", DL: "DT,DD",
		NOFRAMES: "*", ISINDEX: "*", FORM: "*"
	};
	var mc_sBlockTags = (new PropertyArray()).merge(mc_oBlockTags).join(",");
	var mc_oBlockquoteTags = { BLOCKQUOTE: "*" };
	var mc_oListItemTags = { LI: "OL,UL,DIR,MENU", DT: "DL", DD: "DL" };
	var mc_sListItemTags = (new PropertyArray()).merge(mc_oListItemTags).join(",");
	var mc_oTableCellTags = { TD: "TABLE", TH: "TABLE" };
	var mc_sIndentableTags = (new PropertyArray()).merge(mc_oBlockTags, mc_oListItemTags, mc_oTableCellTags).join(",");
	var mc_sOutdentableTags = (new PropertyArray()).merge(mc_oBlockquoteTags, mc_oListItemTags).join(",");
})();


