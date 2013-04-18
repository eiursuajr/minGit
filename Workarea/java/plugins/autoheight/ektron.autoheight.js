$ektron.fn.autoheight = function(settings)
{
	var settings = $ektron.extend(
	{
		bindEvents: true,
		onexception: null
	}, settings);
	
	function autoheightIFrame()
	{
		var m_iframe = this;
		function m_autoheightIFrame()
		{
			// assert(this === m_iframe.contentWindow.document);
			try
			{
				if (this.body)
				{
					var objIFrame = $ektron(m_iframe);
					var ht = 0;
					if ($ektron.browser.mozilla)
					{
						ht = this.documentElement.offsetHeight; // number
					}
					else
					{
						ht = this.documentElement.scrollHeight; // number
					}
					if (0 == ht) return;
					var maxht = objIFrame.css("max-height"); // get inherited max-height (string)
					if ("none" == maxht) maxht = "";
					if (maxht && -1 == maxht.indexOf("px"))
					{
						if (objIFrame.height() != ht) 
						{
							objIFrame.height(ht);
							$ektron(m_iframe.ownerDocument).trigger($ektron.fn.autoheight.triggerName, [objIFrame, ht]);
						}
					}
					else if (objIFrame.height() != ht)
					{
						if ($ektron.browser.mozilla || $ektron.browser.safari)
						{
							m_iframe.style.overflowY = "hidden"; // resize assuming no scroll bars
						}
						else
						{
							m_iframe.style.overflowY = "visible";
							// #44127: overflowX - hidden will make the content flow outside the set-width panel.
//							if ($ektron.browser.msie)
//							{
//							    if ($ektron.browser.version < 7) //IE6 browser will handle the overflow in x-axis.
//							    {
//							        m_iframe.style.overflowX = "hidden";
//							    }
//							}
						}
						objIFrame.height(ht);
						if (maxht)
						{
							if (parseInt("0" + maxht, 10) < ht)
							{
								m_iframe.style.overflowY = "auto";
							}
						}
						$ektron(m_iframe.ownerDocument).trigger($ektron.fn.autoheight.triggerName, [objIFrame, ht]);
					}
					if ($ektron.browser.msie && $ektron.browser.version >= 7) //IE7+
					{
					    if ((objIFrame.width() - $ektron(oDoc.body).width()) < 8)
                        {
                            oDoc.documentElement.style.overflowX = "hidden";
                        }
					}
				}
			}
			catch (ex)
			{
				Ektron.OnException($ektron.fn.autoheight, settings.onexception, ex, arguments);
			}
		};
		try
		{
			if (m_iframe.contentWindow && m_iframe.contentWindow.document)
			{
				var oDoc = m_iframe.contentWindow.document;
				// Changing height will change offsetHeight if body in iframe is a percentage height,
				// so remove height style in body and html tags
				if (oDoc.documentElement) 
				{
					oDoc.documentElement.style.height = "";
					var maxht = $ektron(m_iframe).css("max-height"); // get inherited max-height (string)
					if ("none" == maxht) maxht = "";
					if (!maxht)
					{
						if ($ektron.browser.mozilla || $ektron.browser.safari)
						{
							oDoc.documentElement.style.overflowY = "hidden";
						}
						else
						{
							oDoc.documentElement.style.overflowY = "visible";
							if ($ektron.browser.msie)
							{
							    if ($ektron.browser.version < 7) //IE6 browser will handle the overflow in x-axis.
							    {
							        oDoc.documentElement.style.overflowX = "hidden";
							    }
							    // #44127: overflowX - hidden will make the content flow outside the set-width panel.
//							    else //IE7+
//							    {
//                                    m_iframe.style.overflowX = "hidden";
//							    }
							}
						}
					}
				}
				if (oDoc.body) oDoc.body.style.height = "";
				if (settings.bindEvents)
				{
					$ektron(oDoc).keyup(m_autoheightIFrame).click(m_autoheightIFrame).each(m_autoheightIFrame);
				}
				else
				{
					$ektron(oDoc).each(m_autoheightIFrame);
				}
			}
		}
		catch (ex)
		{
			Ektron.OnException($ektron.fn.autoheight, settings.onexception, ex, arguments);
		}
	};
	
//FF textarea scrollHeight needs to set height to 1px to recalc but causes flash.
//Possible fix: clone textarea to calc scrollHeight so it is not visible.
//	function autoheightTextarea()
//	{
//		try
//		{
//			var objTextarea = $ektron(this);
//			var maxht = objTextarea.css("max-height"); // get inherited max-height (string)
//			if ("none" == maxht) maxht = "";
//			objTextarea.height(1); // needed by Firefox & Safari to reduce size of scrollHeight
//			var ht = this.scrollHeight; // number (textarea.scrollHeight is bad in Opera)
//			objTextarea.height(ht);
//			if (maxht)
//			{
//				if (maxht.indexOf("px") > 0)
//				{
//					if (parseInt("0" + maxht, 10) < ht)
//					{
//						this.style.overflowY = "auto";
//					}
//					else
//					{
//						this.style.overflowY = "hidden";
//					}
//				}
//			}
//			else
//			{
//				this.style.overflowY = "hidden";
//			}
//		}
//		catch (ex)
//		{
//			Ektron.OnException($ektron.fn.autoheight, settings.onexception, ex, arguments);
//		}
//	};

	if (settings.bindEvents) 
	{
		this.filter("iframe").load(autoheightIFrame).each(autoheightIFrame);
//		this.filter("textarea").keyup(autoheightTextarea).click(autoheightTextarea).each(autoheightTextarea);
	}
	else
	{
		this.filter("iframe").each(autoheightIFrame);
//		this.filter("textarea").each(autoheightTextarea);
	}
	return this;
};
$ektron.fn.autoheight.triggerName = "EktronAutoheight";
$ektron.fn.autoheight.onexception = Ektron.OnException.diagException;
