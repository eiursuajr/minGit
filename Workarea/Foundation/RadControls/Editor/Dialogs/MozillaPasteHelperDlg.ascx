<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MozillaPasteHelperDlg.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.MozillaPasteHelperDlg" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ OutputCache Duration="600" VaryByParam="Language;SkinPath" %>

<style type="text/css">
#mainform
{
    height: 90%;
}
#Ektron_FrameContentMiddle
{
	overflow: hidden;
}
#ContentTextarea
{
	overflow: auto;
}
#ContentIFrame
{
	border: inset 2px;
}
</style>

<div id="Ektron_FrameContentTop">
    <div class="Ektron_FrameInnerTube">
        <div class="Ektron_TopLabel">
            <%--ektron start--%>
            <label id="lblTextarea"><script type="text/javascript">
            <!--
				localization.showText('PasteContent');
			// -->
			</script></label>
            <%--ektron end--%>
        </div>
    </div>
</div>
        
<div id="Ektron_FrameContentMiddle">
	<iframe id="ContentIFrame" class="Text" src="javascript:''" frameborder="0" style="visibility:hidden; width:99%; height:90%"></iframe>
	<textarea id="ContentTextarea" class="Text" style="display:none; width:99%; height:90%" tabindex="1" onkeyup="PasteHelper_onkeyup(event);"></textarea>
</div>

<div id="Ektron_FrameContentBottom">
    <div class="Ektron_Dialogs_ButtonContainer">
        <button id="btnSubmit" title="Ok" class="Ektron_StandardButton" onclick="return OkClicked();" tabindex="3" type="button"><script type="text/javascript">
        <!--
			localization.showText('OK');
		// -->
		</script></button>
        <span class="Ektron_LeftSpaceSmall"></span>
		<button id="btnCancel" title="Cancel" class="Ektron_StandardButton" onclick="CloseDlg();" tabindex="4" type="button"><script type="text/javascript">
        <!--
			localization.showText('Cancel');
		// -->
		</script></button>
		<span class="Ektron_LeftSpaceSmall"></span>
		<!-- Ektron begin -->
		<button id="btnPreview" title="Preview" class="Ektron_StandardButton" onclick="preview();" tabindex="5" type="button"><script type="text/javascript">
        <!--
			localization.showText('Preview');
		// -->
		</script></button>
		<!-- Ektron end -->
    </div>
</div>
<script type="text/javascript">	 
<!--
	var m_oContentIFrame = document.getElementById("ContentIFrame");
	var m_oContentTextarea = document.getElementById("ContentTextarea");
	var m_fnGetFilteredContent; // undefined
    var m_contentType; // undefined
    var m_oEditor = null;
    var m_contentCache = null;
    var m_imgBusy = null;
	
	function InitDialog()
	{
		var args = GetDialogArguments();
        m_oEditor = args.EditorObj;
        m_contentType = args.contentType;
		m_fnGetFilteredContent = args.getFilteredContent;
	    if (args.title) 
	    {
	        document.getElementById("lblTextarea").innerHTML = args.title;
	    }

		if ("rich" == m_contentType)
		{
			m_oContentIFrame.style.visibility = "visible";
			$ektron(m_oContentIFrame).load(function iframeOnLoad()
			{
				var oWin = this.contentWindow; // 'this' is m_oContentIFrame
				var oDoc = oWin.document;

				$ektron(oDoc).keyup(Warning_onkeyup);
				
				// #42581 - preview
				$ektron(oDoc).keyup(PasteHelper_onkeyup);
				
				try
				{
					oDoc.body.innerHTML = "";
					if ($ektron.browser.msie)
					{
						oDoc.body.contentEditable = "true";
					}
					else
					{
						oDoc.designMode = "on";
					}
				}
				catch (ex)
				{
					// ignore
					Ektron.ContentDesigner.raiseEvent(oWin, "onexception", [new Error("Failed to set designMode/contentEditable. Error: " + ex.message), arguments]);
				}
			}); // onload
			
			// Assigned CSS file(s) to paste area
			var strCssFile = "";
			if (m_oEditor.CssFilesArray)
			{
				for (var i = 0; i < m_oEditor.CssFilesArray.length; i++)
				{
					strCssFile = "&css" + i + "=" + m_oEditor.CssFilesArray[i];
				}
			}
			m_oContentIFrame.src = m_oEditor.ekParameters.srcPath + "ekformsiframe.aspx?js=no&height=99%&id=" + m_oEditor.Id + strCssFile;
			m_oContentIFrame.focus();
		}
		else
		{
			document.getElementById("btnPreview").style.display = "none";
			m_oContentIFrame.style.display = "none";
			m_oContentTextarea.style.display = "block";
			if (args.content) 
		    {
		        m_oContentTextarea.value = args.content;
		    }
			m_oContentTextarea.focus();
		}
	}
	
	function PasteHelper_onkeyup(event)
	{
		event = (event ? event : window.event); 
		m_contentCache = null;
		// In FF 2, event.altKey is undefined
		// ctrl+v for paste
		if (event.ctrlKey && "V".charCodeAt(0) == event.keyCode && !event.shiftKey && !event.altKey)
		{
			preview();
		}
	}

	function WarnWhenTooMuchContent(txt) {
		var showWarning = m_oEditor.ekParameters.ShowPasteWarning;
		
		if (!(showWarning === 'true')) return;
		
		var warningThreshold = m_oEditor.ekParameters.WordPasteThreshold;
		var globalCharCount = m_oEditor.ekParameters.CurrentCharCount;

		if ((typeof txt != "undefined" && txt.length > warningThreshold) || globalCharCount > warningThreshold) {
			m_oEditor.ekParameters.ShowPasteWarning = false;
			
			var warningMessage = '';
			try { warningMessage = m_oEditor.GetLocalizedString('WordPasteWarningMessage', 'Warning: pasting large amounts of content or numerous items that total to a larger amount of content may result slower browser performance.'); } catch (e) { }

			alert(warningMessage);
		}
	}

	function Warning_onkeyup(event) {
		event = (event ? event : window.event);
		
		if (event.currentTarget != null &&
			event.currentTarget.body.childNodes != null &&
				event.currentTarget.body.childNodes.length > 0) {

			WarnWhenTooMuchContent(m_oContentIFrame.contentWindow.document.body.textContent);
		}
	}

	function filterRichContent(oBody)
	{
		var content = Ektron.Xml.serializeXhtml(oBody.childNodes);
		if ("function" == typeof m_fnGetFilteredContent != "function")
		{
			content = m_fnGetFilteredContent(content);
		}
		// ektron start - #52915, #52383: Clean up Markup with HTML Tidy
        content = content.replace(/> /g, ">&#160;");// #66716 - preserve the space that will be removed by the HTML tidy
        $ektron.ajax(
		{
			type: "POST",
			async: false,
			url: m_oEditor.ekParameters.srcPath + "ekajaxtidy.aspx",
			data: { html: content },
			dataType: "html",
			success: function(data)
			{ 
				data = $ektron.trim(data);
				if (data.indexOf("ekAjaxTidyError") > -1)
				{
					var matchResult = data.match(/<body[^>]*>([\w\W]*?)<\/body>/);
					if (matchResult.length >= 2)
					{
						errorMessage = matchResult[1];
					}
					else
					{
						errorMessage = data;
					}
				}
				else
				{
					content = data;
				}
			},
			error: function(xhr)
			{
				errorMessage = "Ajax Error: " + xhr.status + ": " + xhr.statusText;
			}
		});
        // ektron end
	    return content;
	}
		
	function preview()
	{
		setTimeout(function()
		{
			var buttons = $ektron("button:enabled");
			buttons.attr("disabled", true);
			document.body.style.cursor = "wait";
			setTimeout(function()
			{
				try
				{
					if ("rich" == m_contentType)
					{
						var oBody = m_oContentIFrame.contentWindow.document.body;
						var eBody = $ektron(oBody);

						m_contentCache = filterRichContent(oBody);
						var content = m_contentCache;
						if (m_oEditor)
						{
							// IE (seen in IE 7, FF 3 and Saf 3 are OK) won't catch error in GetDesignContent,
							// suspect b/c it's in a different window.
							var err = null;
							content = m_oEditor.filter.GetDesignContent(content, function(ex) 
							{ 
								err = ex;
								return "";
							}); 
							if (err) throw err;
						}
						eBody.html(content);
						var oElem = eBody.children(":first-child").get(0);
						if (oElem && oElem.scrollIntoView) 
						{
							try
							{
								oElem.scrollIntoView();
							}
							catch (ex) { }
						}					
					}
					else
					{
						if (m_oContentTextarea.createTextRange)
						{
							var range = m_oContentTextarea.createTextRange();
							range.collapse(true);
							range.moveStart('character', 0);
							range.moveEnd('character', 0);
							range.select();
						}
						else
						{
							m_oContentTextarea.value = m_oContentTextarea.value;
						}
					}
				}
				catch (ex)
				{
					Ektron.ContentDesigner.raiseEvent(window, "onexception", [ex, arguments]);
				}
				finally
				{
					document.body.style.cursor = "auto";
					buttons.attr("disabled", false);
				}
				try
				{
					var oElem = document.getElementById("btnSubmit");
					if (oElem && oElem.focus)
					{
						oElem.focus();
					}
				}
				catch (ex) { }
			}, 1);
		}, 1);
	}
	
	function OkClicked()
	{
		var content = "";
		var bValid = true;
		if ("rich" == m_contentType)
		{
			if (m_contentCache)
			{
				content = m_contentCache;
			}
			else
			{
				var oBody = m_oContentIFrame.contentWindow.document.body;
				content = filterRichContent(oBody);

				WarnWhenTooMuchContent(content);
			}
		}
		else
		{    
		    content = m_oContentTextarea.value;
		    if ("function" == typeof m_fnGetFilteredContent)
		    {
				content = m_fnGetFilteredContent(content);
			}
		    if (m_oEditor)
		    {
				content = m_oEditor.ekXml.fixXml(content); 
				var xmlDoc = Ektron.Xml.parseXml("<root>" + content + "</root>", Ektron.OnException.returnException); 
				if ("string" == typeof xmlDoc) 
				{
					bValid = false;
					if (confirm(xmlDoc + "\n\n" + loc["QueryAutoCorrect"]))
					{
						var oDiv = document.createElement("DIV");
						oDiv.innerHTML = content;
						content = Ektron.Xml.serializeXhtml(oDiv.childNodes);
						m_oContentTextarea.value = content;
						oDiv = null;
					}
				}
		    }
		}
		if (bValid)
		{
		    CloseDlg(content);
		}
	}

	AttachEvent(window, "load", InitDialog);
	
	if (typeof Ektron.ContentDesigner != "object") Ektron.ContentDesigner = {};
	Ektron.ContentDesigner.raiseEvent = function(win, eventName, args)
	{
		//raise an event to EkRadEditor.js
		if (win.parent && win.parent.parent && win.parent.parent.Ektron && win.parent.parent.Ektron.ContentDesigner)
		{
			var fnEventHandler = win.parent.parent.Ektron.ContentDesigner[eventName];
			if ("function" == typeof fnEventHandler)
			{
				fnEventHandler.apply(win.parent.parent.Ektron.ContentDesigner, args);
			}
		}
	};
//-->
</script>
