<%@ Control Language="C#" %>
<%@ Register TagPrefix="radcb" Namespace="Telerik.WebControls" Assembly="RadComboBox.NET2" %>
<%@ Register TagPrefix="picker" TagName="ColorPicker" Src="../../Foundation/RadControls/Editor/Controls/ColorPicker.ascx" %>
<script type="text/javascript" language="javascript" src="../DropDownMenu.js"></script>
<script type="text/javascript" language="javascript" src="../ColorPicker.js"></script>
<style>
    .selected
    {
        background-color: #808080;
    }
</style>
<script runat="server">
    protected void Page_Load(Object src, EventArgs e)
    {
		Ektron.Cms.ContentAPI api = new Ektron.Cms.ContentAPI();
		Ektron.Cms.Common.EkMessageHelper refMsg = api.EkMsgRef;
		
		string skinPath = api.AppPath + "csslib/Editor/";
		this.FontColorPicker.SkinPath = skinPath;
		this.BackgroundColorPicker.SkinPath = skinPath;
		
		this.lblBgColor.InnerHtml = refMsg.GetMessage("lbl bg color");
		this.lblBold.InnerHtml = refMsg.GetMessage("lbl bold");
		this.lblFontColor.InnerHtml = refMsg.GetMessage("lbl font color");
		this.lblFontName.InnerHtml = refMsg.GetMessage("lbl font name");
		this.lblFontSize.InnerHtml = refMsg.GetMessage("lbl font size");
		this.lblItalic.InnerHtml = refMsg.GetMessage("lbl italic");
		this.lblTextAlign.InnerHtml = refMsg.GetMessage("lbl text align");
		this.lblTextLine.InnerHtml = refMsg.GetMessage("lbl text line");

		string sUnassigned = refMsg.GetMessage("generic unassign in parens");
		this.cboBold.Items[0].Text = sUnassigned;
        this.cboBold.Items[0].ToolTip = sUnassigned;
		this.cboBold.Items[1].Text = refMsg.GetMessage("font weight normal");
        this.cboBold.Items[1].ToolTip = refMsg.GetMessage("font weight normal");
		this.cboBold.Items[2].Text = refMsg.GetMessage("font weight bold");
        this.cboBold.Items[2].ToolTip = refMsg.GetMessage("font weight bold");
		this.cboItalic.Items[0].Text = sUnassigned;
        this.cboItalic.Items[0].ToolTip = sUnassigned;
		this.cboItalic.Items[1].Text = refMsg.GetMessage("font style normal");
        this.cboItalic.Items[1].ToolTip = refMsg.GetMessage("font style normal");
		this.cboItalic.Items[2].Text = refMsg.GetMessage("font style italic");
        this.cboItalic.Items[2].ToolTip = refMsg.GetMessage("font style italic");
		this.cboTextAlign.Items[0].Text = sUnassigned;
        this.cboTextAlign.Items[0].ToolTip = sUnassigned;
		this.cboTextAlign.Items[1].Text = refMsg.GetMessage("text align left");
        this.cboTextAlign.Items[1].ToolTip = refMsg.GetMessage("text align left");
		this.cboTextAlign.Items[2].Text = refMsg.GetMessage("text align center");
        this.cboTextAlign.Items[2].ToolTip = refMsg.GetMessage("text align center");
		this.cboTextAlign.Items[3].Text = refMsg.GetMessage("text align right");
        this.cboTextAlign.Items[3].ToolTip = refMsg.GetMessage("text align right");
		this.cboTextAlign.Items[4].Text = refMsg.GetMessage("text align justify");
        this.cboTextAlign.Items[4].ToolTip = refMsg.GetMessage("text align justify");
		this.cboTextLine.Items[0].Text = sUnassigned;
        this.cboTextLine.Items[0].ToolTip = sUnassigned;
		this.cboTextLine.Items[1].Text = refMsg.GetMessage("text decoration none");
        this.cboTextLine.Items[1].ToolTip = refMsg.GetMessage("text decoration none");
		this.cboTextLine.Items[2].Text = refMsg.GetMessage("text decoration underline");
        this.cboTextLine.Items[2].ToolTip = refMsg.GetMessage("text decoration underline");
		this.cboTextLine.Items[3].Text = refMsg.GetMessage("text decoration strikethrough");
        this.cboTextLine.Items[3].ToolTip = refMsg.GetMessage("text decoration strikethrough");
		
        if (!Page.ClientScript.IsClientScriptBlockRegistered("EkFieldDataStyleScript"))
		{
			string ScriptText = EkFieldDataStyleScript.InnerText;
            ScriptText = ScriptText.Replace("<%=cboFontName.ClientID%>", cboFontName.ClientID);
            ScriptText = ScriptText.Replace("<%=cboFontSize.ClientID%>", cboFontSize.ClientID);
            ScriptText = ScriptText.Replace("<%=cboBold.ClientID%>", cboBold.ClientID);
            ScriptText = ScriptText.Replace("<%=cboItalic.ClientID%>", cboItalic.ClientID);
            ScriptText = ScriptText.Replace("<%=cboTextAlign.ClientID%>", cboTextAlign.ClientID);
            ScriptText = ScriptText.Replace("<%=cboTextLine.ClientID%>", cboTextLine.ClientID);
            ScriptText = ScriptText.Replace("<%=FontColorPicker.ClientID%>", FontColorPicker.ClientID);
            ScriptText = ScriptText.Replace("<%=BackgroundColorPicker.ClientID%>", BackgroundColorPicker.ClientID);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EkFieldDataStyleScript", ScriptText, true);
		}
        EkFieldDataStyleScript.Visible = false;
	}

	protected void cboFontProp_OnDataBound(Object sender, EventArgs e)
	{
		Ektron.Cms.Common.EkMessageHelper refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
		string sUnassigned = refMsg.GetMessage("generic unassign in parens");
		RadComboBoxItem item = ((RadComboBox)sender).Items[0];
		item.Text = sUnassigned;
		// RadComboBox fails to assign value with empty strings. :-(
		// so use "u/a" to represent unassigned (ie, empty string)
		item.Value = "u/a";
	}
</script>
<clientscript id="EkFieldDataStyleScript" runat="server">
function EkFieldDataStyleControl()
{
    this.read = function(oFieldElem)
    {
        if (null == oFieldElem) return;
        var objStyle = oFieldElem.style;
        var sStyle = "";
        if (objStyle)
        {
            sStyle = objStyle.fontFamily;
            this.setComboBoxValue(&lt;%=cboFontName.ClientID%&gt;, sStyle);

            sStyle = objStyle.fontSize;
            this.setComboBoxValue(&lt;%=cboFontSize.ClientID%&gt;, sStyle);

            sStyle = objStyle.fontWeight;
            this.setComboBoxValue(&lt;%=cboBold.ClientID%&gt;, sStyle);

            sStyle = objStyle.fontStyle;
            this.setComboBoxValue(&lt;%=cboItalic.ClientID%&gt;, sStyle);

            sStyle = objStyle.textAlign;
            this.setComboBoxValue(&lt;%=cboTextAlign.ClientID%&gt;, sStyle);

            sStyle = objStyle.textDecoration;
            this.setComboBoxValue(&lt;%=cboTextLine.ClientID%&gt;, sStyle);

            &lt;%=FontColorPicker.ClientID%&gt;.SelectColor(objStyle.color.toUpperCase());
            &lt;%=BackgroundColorPicker.ClientID%&gt;.SelectColor(objStyle.backgroundColor.toUpperCase());
        }
    }
    this.update = function(oFieldElem)
    {
        if (null == oFieldElem) return;
        var objStyle = oFieldElem.style;
        if (objStyle)
        {
			var objComboBox = null;
			objComboBox = &lt;%=cboFontName.ClientID%&gt;;
            var fontFamily = (&lt;%=cboFontName.ClientID%&gt;.GetText() || ""); // AllowCustomText="True"
            if (objComboBox.FindItemByText(fontFamily) != null)
            {
				fontFamily = objComboBox.GetValue();
            }
            objComboBox = &lt;%=cboFontSize.ClientID%&gt;;
            var fontSize = (&lt;%=cboFontSize.ClientID%&gt;.GetText() || ""); // AllowCustomText="True"
            if (objComboBox.FindItemByText(fontSize) != null)
            {
				fontSize = objComboBox.GetValue();
            }
            var fontWeight = (&lt;%=cboBold.ClientID%&gt;.GetValue() || "");
            var fontStyle = (&lt;%=cboItalic.ClientID%&gt;.GetValue() || "");
            var textAlign = (&lt;%=cboTextAlign.ClientID%&gt;.GetValue() || "");
            var textDecoration = (&lt;%=cboTextLine.ClientID%&gt;.GetValue() || "");
            if ("u/a" == fontFamily) fontFamily = "";
            if ("u/a" == fontSize) fontSize = "";
            if ("u/a" == fontWeight) fontWeight = "";
            if ("u/a" == fontStyle) fontStyle = "";
            if ("u/a" == textAlign) textAlign = "";
            if ("u/a" == textDecoration) textDecoration = "";
            if (fontFamily != objStyle.fontFamily)
            {
                // IE (7 at least) will insert empty string to cssText if there is fontFamily is an empty string.
                objStyle.fontFamily = fontFamily;
            }
            objStyle.fontSize = fontSize;
            objStyle.fontWeight = fontWeight;
            objStyle.fontStyle = fontStyle;
            objStyle.textAlign = textAlign;
            objStyle.textDecoration = textDecoration;

            objStyle.color = &lt;%=FontColorPicker.ClientID%&gt;.SelectedColor;
            objStyle.backgroundColor = &lt;%=BackgroundColorPicker.ClientID%&gt;.SelectedColor;  
            
            if ("" == fontFamily)
            {
                var text = objStyle.cssText;
                var newtext = text.replace(/\bFONT\-FAMILY\:\s*(\;|$)/i, "");
                if (text != newtext)
                {
                    try
                    {
                        objStyle.cssText = newtext;
                    }
                    catch (ex) {}
                }
            }
        } 
    }
    this.setComboBoxValue = function(comboBox, value)
    {
		if (!value) value = "u/a";
		comboBox.SetValue(value);
		comboBox.SetText(value);
        for (var i = 0; i < comboBox.Items.length; i++)
        {
            var itemValue = comboBox.Items[i].Value;
			if (!itemValue) itemValue = "u/a";
            if (value.toLowerCase() == itemValue.toLowerCase())
            {
                var itemText = comboBox.Items[i].Text;
				comboBox.SetText(itemText);
                break;
            }
        }
    }
}
var ekFieldDataStyleControl = new EkFieldDataStyleControl();
</clientscript>
<asp:PlaceHolder ID="FieldDataStyle" runat="server">
<tr>
    <td><label title="Font Name" for="cboFontName" class="Ektron_StandardLabel" id="lblFontName" runat="server">Font Name:</label></td>
    <td>
        <radcb:radcombobox id="cboFontName" runat="server" 
            DataSourceID="XmlDataSource"
            AllowCustomText="True"
            DataTextField="text"
            DataValueField="data" 
            OnDataBound="cboFontProp_OnDataBound">
        </radcb:radcombobox>

        <asp:XmlDataSource ID="XmlDataSource" runat="server"
            DataFile="../../ContentDesigner/FontName.xml">
        </asp:XmlDataSource>
	</td>
</tr>
<tr>
    <td><label title="Font Size" for="cboFontSize" class="Ektron_StandardLabel" id="lblFontSize" runat="server">Font Size:</label></td>
    <td>
        <radcb:radcombobox id="cboFontSize" runat="server" 
            DataSourceID="XmlDataSource2"
            AllowCustomText="True"
            DataTextField="text"
            DataValueField="data" 
            OnDataBound="cboFontProp_OnDataBound">
        </radcb:radcombobox>

        <asp:XmlDataSource ID="XmlDataSource2" runat="server"
            DataFile="../../ContentDesigner/FontSize.xml">
        </asp:XmlDataSource>
	</td>
</tr>
<tr>
    <td><label title="Bold" for="cboBold" class="Ektron_StandardLabel" id="lblBold" runat="server">Bold:</label></td>
    <td>
        <radcb:radcombobox id="cboBold" runat="server">
            <Items>
               <radcb:RadComboBoxItem Text="(Unassigned)" Value="u/a" />
               <radcb:RadComboBoxItem Text="Normal" Value="normal" />
               <radcb:RadComboBoxItem Text="Bold" Value="bold" />
            </Items>
        </radcb:radcombobox>
	</td>
</tr>
<tr>
    <td><label title="Italic" for="cboItalic" class="Ektron_StandardLabel" id="lblItalic" runat="server">Italic:</label></td>
    <td>
        <radcb:radcombobox id="cboItalic" runat="server">
            <Items>
               <radcb:RadComboBoxItem Text="(Unassigned)" Value="u/a" />
               <radcb:RadComboBoxItem Text="Normal" Value="normal" />
               <radcb:RadComboBoxItem Text="Italic" Value="italic" />
            </Items>
        </radcb:radcombobox>
	</td>
</tr>
<tr>
    <td><label title="Text Alignment" for="cboTextAlign" class="Ektron_StandardLabel" id="lblTextAlign" runat="server">Text Alignment:</label></td>
    <td>
        <radcb:radcombobox id="cboTextAlign" runat="server">
            <Items>
               <radcb:RadComboBoxItem Text="(Unassigned)" Value="u/a" />
               <radcb:RadComboBoxItem Text="Left" Value="left" />
               <radcb:RadComboBoxItem Text="Center" Value="center" />
               <radcb:RadComboBoxItem Text="Right" Value="right" />
               <radcb:RadComboBoxItem Text="Justify" Value="justify" />
            </Items>
        </radcb:radcombobox>
	</td>
</tr>
<tr>
    <td><label title="Text Line" for="cboTextLine" class="Ektron_StandardLabel" id="lblTextLine" runat="server">Text Line:</label></td>
    <td>
        <radcb:radcombobox id="cboTextLine" runat="server">
            <Items>
               <radcb:RadComboBoxItem Text="(Unassigned)" Value="u/a" />
               <radcb:RadComboBoxItem Text="None" Value="none" />
               <radcb:RadComboBoxItem Text="Underline" Value="underline" />
               <radcb:RadComboBoxItem Text="Strikethrough" Value="line-through" />
            </Items>
        </radcb:radcombobox>
	</td>
</tr>
<tr>
    <td><label title="Font Color" for="chkColorUnassigned" class="Ektron_StandardLabel" id="lblFontColor" runat="server">Font Color:</label></td>
    <td><picker:colorpicker id="FontColorPicker" runat="server" /></td>
</tr>
<tr>
    <td><label title="Background Color" for="chkColorUnassigned" class="Ektron_StandardLabel" id="lblBgColor" runat="server">Background Color:</label></td>
    <td><picker:colorpicker id="BackgroundColorPicker" runat="server" /></td>
</tr>
</asp:PlaceHolder> 
<script type="text/javascript">
<!--

if ("undefined" === typeof(Ektron)){ Ektron = {}; }
if ("undefined" === typeof(Ektron.Editor)){ Ektron.Editor = {}; }
if ("undefined" === typeof(Ektron.Editor.StyleDialog))
{ 
    Ektron.Editor.StyleDialog = 
    {
        bindEvents: function(divButton, divCombo)
        {
            if ($ektron.browser.msie)
            {
                $ektron("input.ComboBoxInput_Default").focus(function()
                {
                    $ektron(this).closest("div.ComboBox_Default").focus();
                });
            }
        
            divCombo.keydown(function(e) 
            {
                var clientid = $ektron(this).attr("id");
                var placeholder = $ektron("div#" + clientid + "_DropDownPlaceholder");
                var curr = $ektron(".ComboBoxItemHover_Default", placeholder);
                var input = $ektron("input", this);
                var image = $ektron("img", this);
                switch (e.keyCode)
                {
                    case 32: //space bar
                        $ektron(this).children("img.ComboBoxImage_Default").click();
                        placeholder.css("display", "block");
                        placeholder.children("ComboBoxItemHover_Default").focus();
                        break;
                    case 38: // up arrow
                        curr.addClass("ComboBoxItem_Default").removeClass("ComboBoxItemHover_Default");
                        var display = curr.prev("div.ComboBoxItem_Default").addClass("ComboBoxItemHover_Default").text();
                        updateComboBoxControl(display);
                        break;
                    case 40: // down arrow
                        curr.addClass("ComboBoxItem_Default").removeClass("ComboBoxItemHover_Default");
                        var display = curr.next("div.ComboBoxItem_Default").addClass("ComboBoxItemHover_Default").text();
                        updateComboBoxControl(display);
                        break;
                }
                
                function updateComboBoxControl(display)
                {
                    if ("" == display)
                    {
                        curr.addClass("ComboBoxItemHover_Default");
                        display = curr.text();
                    }
                    $ektron("input.ComboxInputHover_Default").removeClass("ComboBoxInputHover_Default");
                    input.addClass("ComboBoxInputHover_Default").val(display);
                    $ektron("img.ComboBoxImageHover_Default").removeClass("ComboBoxImageHover_Default");
                    image.addClass("ComboBoxImageHover_Default");
                }
            });
            
            divButton.keydown(function(e) {
                var button = $ektron(this).closest("button");
                var clientid = $ektron(this).attr("id");
                var menuElemntId = clientid.replace(/MENU_BUTTON_SPAN_/g,"MENU_ELEMENT_"); 
                //space bar and enter
                if (32 == e.keyCode || 13 == e.keyCode)
                { 
                    button.click();
                    $ektron("div#" + menuElemntId).css("visibility", "visible"); //for IE
                    var tableId = clientid.replace(/MENU_BUTTON_SPAN_/g,"COLOR_TABLE_"); 
                    var table = $ektron("table#" + tableId);
                    
                    $ektron("td", table).each(function(i, e)
                    {
                        $ektron(this).attr("tabindex", divCombo.length + divButton.length + i); 
                    });
                    
                    $ektron("td", table).keydown(function(evt)
                    { 
                        var element = evt.srcElement ? evt.srcElement: evt.target;
                        var td = $ektron(element);
                        if (element.tagName != "TD") td = td.closest("td");
                        switch (evt.keyCode)
                        {
                            case 32: //space bar
                                $ektron("div", table).css("border-width", "1px");
                                $ektron("span", table).removeClass("selected");
                                var colorCell = td.children("div");
                                if (1 == colorCell.length)
                                {
                                    colorCell.css("border-width", "2px").focus(); 
                                }
                                else
                                {
                                    td.children("span").addClass("selected").focus();
                                }
                                break;
                            case 13: //enter
                                var colorCell = td.children("div");
                                if ("2px" == colorCell.css("border-width") || (colorCell.css("borderWidth") && 0 == colorCell.css("borderWidth").indexOf("2px")))
                                {
                                    td.click();
                                }
                                else
                                {
                                    $ektron("span", table).each( function() {
                                        var selectCustomCell = $ektron(this);
                                        if (selectCustomCell.hasClass("selected"))
                                        {
                                            selectCustomCell.closest("td").click();
                                            selectCustomCell.removeClass("selected");
                                            return false;
                                        }
                                    });
                                }
                                $ektron("div", table).css("border-width", "1px");
                                evt.stopImmediatePropagation();
                                return false;
                                break;
                            case 27: //escape
                                $ektron("td:first", table).click();//no color and close popup
                                break;
                            case 9: //tab
                                if ($ektron.browser.msie)
                                {
                                    // need to draw this border b/c IE does not show the dash border around the selected div. 
                                    $ektron("div", table).css("border-width", "1px");
                                    var currTabId = parseInt(td.attr("tabindex"));
                                    var nextTabId = currTabId + 1;
                                    var colorCell = $ektron("td[tabindex='" + nextTabId + "'] div", table);
                                    if (1 == colorCell.length)
                                    {
                                        colorCell.css("border-width", "2px").focus();
                                    }
                                    else 
                                    {
                                        //event element is not relaiable. need to find out by jquery.
                                        var selectedCell = $ektron("span.selected", table);
                                        if (0 == selectedCell.length)
                                        {
                                            $ektron("span:first", table).addClass("selected").focus();
                                        }
                                        else
                                        {
                                            $ektron("span", table).each( function() {
                                                var selectCustomCell = $ektron(this);
                                                if (selectCustomCell.hasClass("selected"))
                                                {
                                                    nextTabId = parseInt(selectCustomCell.closest("td").attr("tabindex")) + 1;
                                                    selectCustomCell.removeClass("selected");
                                                    var customCell = $ektron("td[tabindex='" + nextTabId + "'] span", table);
                                                    if (1 == customCell.length)
                                                    {
                                                        customCell.addClass("selected").focus();
                                                    }
                                                    else
                                                    {
                                                        $ektron("td:first div", table).css("border-width", "2px").focus();
                                                    }
                                                    return false;
                                                }
                                            });
                                        }
                                    }
                                    evt.stopImmediatePropagation();
                                    return false;
                                }
                                break;
                        }
                    });
                    table.focus();
                    $ektron("div", table).css("border-width", "1px");
                    $ektron("span", table).removeClass("selected");
                    $ektron("td:first div", table).css("border-width", "2px").focus();
                    e.stopImmediatePropagation();
                    return false;
                }
                else if (27 == e.keyCode)
                {
                    //escape
                    $ektron("div#" + menuElemntId).css("visibility", "hidden");
                }
            });
        },
        init: function()
        {
            var win = window;
	        var divButton = $ektron("div.ColorPickerMenuSpan");
	        var btn = $ektron("button.ColorPickerMainButton");
	        if (divButton.length > 0)
	        {
                var inputCombo = $ektron("input.ComboBoxInput_Default");
                var divCombo = $ektron("div.ComboBox_Default");
                if (!$ektron.browser.msie)
                {
                    inputCombo.each(function(i, e)
                    {
                        $ektron(this).attr("tabindex", i + 1); 
                    }); 
                }
                divCombo.each(function(i, e)
                {
                    $ektron(this).attr("tabindex", i + 1); 
                });
                divButton.each(function(i, e)
                {
                    //both IE and FF need tabIndex on the div, not on button. 
                    //tabIndex="0" does not work too.
                    $ektron(this).attr("tabindex", divCombo.length + 1 + i);
                });
                this.bindEvents(divButton, divCombo);
            }
        }
    };
}


// use Data Style tab with keyboard 
Ektron.ready(function(){
	Ektron.Editor.StyleDialog.init();
});
//-->
</script>
