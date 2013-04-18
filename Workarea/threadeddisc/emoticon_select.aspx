<%@ Page Language="C#" AutoEventWireup="true" CodeFile="emoticon_select.aspx.cs" Inherits="threadeddisc_emoticon_select" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Emoticons</title>
    <script type="text/javascript" language="javascript" src="../ContentDesigner/RadWindow.js" >
</script>
    <style>
        .selected
        {
            border: blue solid 1px;
        }
    </style>
</head>
<body onload="this.focus();">
    <form id="form1" runat="server">
    <script type="text/javascript">
    <!--
    InitializeRadWindow();
   
    function InsertEmoticon(id, isrc) 
    {
        var src_img = document.getElementById('emoticon_' + id);
        var insert_img = document.createElement('img');
        insert_img.src = src_img.src;

        var bContentDesigner = false;
		try
	    {
	       var args = this.GetDialogArguments();
	       if(args)
	        bContentDesigner = true;
	    }
		catch(e){}
		if (bContentDesigner)
		{
		    // The editor does not like the short-cut code as the image alt or title. 
		    // The following would strip out the name of the emoticon from the filename.
		    var sAlt = isrc;
		    if (".gif" == sAlt.substr(sAlt.length - 4).toLowerCase())
		    {
		        var lSlash = sAlt.lastIndexOf("/");
		        if (lSlash > -1)
		        {
		            sAlt = sAlt.substring(lSlash + 1, sAlt.length - 4);
		        }
		    }
		    var retArgs = { sFilename: isrc, sCaption: sAlt};
            if ("function" == typeof this.CloseDlg)
            {
                this.CloseDlg(retArgs);
            }
		}
		else
		{
		    if (!isEWebEditPro)
            {
            window.opener.FTB_API['content_html'].CreateIMGtag(isrc, '');
             }
          else 
              {
            var tempDIV = document.createElement('div');
            tempDIV.appendChild(insert_img);
            window.opener.eWebEditPro['<%=sEditorName%>'].pasteHTML(tempDIV.innerHTML);
            tempDIV = null;
              }
        this.close();
        }
    }
    function InsertToJSEditor(stext)
    {
        if (IsBrowserIE())
        {
            window.opener.FTB_API['content_html'].InsertEmoticon(stext);
        }
        else
        {
            var existingtext = window.opener.FTB_API['content_html'].GetHtml();
            if (Trim(existingtext) == '')
            {
                window.opener.FTB_API['content_html'].SetHtml(stext);
            }
            else
            {
                window.opener.FTB_API['content_html'].InsertEmoticon(stext);
            }
        }
    }
    
    function IsBrowserIE() 
	{
		// document.all is an IE only property
		return (document.all ? true : false);
	}
	
	function Trim (string) {
	    if (string.length > 0) {
		    string = RemoveLeadingSpaces (string);
	    }
	    if (string.length > 0) {
		    string = RemoveTrailingSpaces(string);
	    }
	    return string;
    }

    function RemoveLeadingSpaces(string) {
	    while(string.substring(0, 1) == " ") {
		    string = string.substring(1, string.length);
	    }
	    return string;
    }

    function RemoveTrailingSpaces(string) {
	    while(string.substring((string.length - 1), string.length) == " ") {
		    string = string.substring(0, (string.length - 1));
	    }
	    return string;
    }
    //-->
    </script>
    <div>
      <table border="0" style ="background-color:White">
       <tr>
            <asp:Literal ID="ltrImage" runat="server" />
        </tr>
    </table>
    </div>
    <script type="text/javascript">
        <!--
        var win = window;
        window.setTimeout(function()
		{
            win.focus();
            $ektron("img:first").addClass("selected").focus();
            $ektron("img").keydown(function(e) {
                keypressEventHandler(e, win);
            });
            $ektron(win).keydown(function(e) {
                keypressEventHandler(e, this);
             });
             
             function keypressEventHandler(e, win)
             {
                var element = e.srcElement ? e.srcElement: e.target;
                var eImg = $ektron(element);
                switch (e.keyCode)
                {
                    case 32: //space bar
                        $ektron("img.selected").removeClass("selected");
                        eImg.addClass("selected"); 
                        eImg.get(0).scrollIntoView(false);
                        break;
                    case 13: //enter
                        var id = $ektron("img.selected").attr("id").replace(/emoticon_/, "");
                        var isrc = $ektron("img.selected").attr("src");
                        InsertEmoticon(id, isrc);
                        e.stopImmediatePropagation();
                        return false;
                        break;
                    case 27: //escape
                        if ("function" == typeof win.CloseDlg)
                        {
                            win.CloseDlg();
                        }
                        break;
                }
             }
        }, 1);
        //-->
    </script>
    </form>
</body>
</html>

