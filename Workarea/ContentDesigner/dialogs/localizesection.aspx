<%@ Page Language="C#" AutoEventWireup="true" CodeFile="localizesection.aspx.cs"
    Inherits="Ektron.ContentDesigner.Dialogs.LocalizeSection" %>

<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title id="pageTitle" runat="server"></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
</head>
<script type="text/javascript">
<!--
    InitializeRadWindow();
    var m_oEditor = null;
    var m_oFieldElem = null;
    var m_content = "";

    function loadcontent() {
        m_oFieldElem = null;
        var args = GetDialogArguments();
        var targetsLocaleArray;
        var excludeLocaleArray;
        var notesLocale;
        var contentElem;
        
        if (args)
        {
            m_oFieldElem = args.selectedField;
            m_oEditor = args.EditorObj;
            m_content = args.selectedHtml;
            if (m_oFieldElem != null)
            {
                //find the included targets and excluded languages for the selected content from content designer.
                var $fieldElem = $ektron(m_oFieldElem);
                var targetsLocale = $fieldElem.find("td.ektdesignns_localize_targets").text();
                var excludeLocale = $fieldElem.find("td.ektdesignns_localize_exclude").text();
                notesLocale = $fieldElem.find("td.ektdesignns_localize_note").html();
                contentElem = $fieldElem.find("> tbody > tr > td");
            }
            else if ($ektron(m_content).attr('class') != null &&
                $ektron(m_content).attr('class').indexOf('ektdesignns_localize') > -1)
            {
                var jqLocalize = $ektron(m_content);
                targetsLocale = $ektron("td.ektdesignns_localize_targets", jqLocalize).html();
                excludeLocale = $ektron("td.ektdesignns_localize_exclude", jqLocalize).html();
                notesLocale = $ektron("td.ektdesignns_localize_note", jqLocalize).html();
                contentElem = $ektron("> tbody > tr > td", jqLocalize);
            }
            if (notesLocale == '&nbsp;')
                notesLocale = '';
            if (targetsLocale != null) {
                targetsLocale = jQuery.trim(targetsLocale);
                if (targetsLocale == '&nbsp;')
                    targetsLocale = '';
                if (targetsLocale.length > 0) {
                    targetsLocaleArray = targetsLocale.split(" ");
                }
            }
            if (excludeLocale != null) {
                excludeLocale = jQuery.trim(excludeLocale);
                if (excludeLocale == '&nbsp;')
                    excludeLocale = '';
                if (excludeLocale.length > 0) {
                    excludeLocaleArray = excludeLocale.split(" ");
                }
            }
            if (contentElem != null && contentElem.length > 0)
            {
                m_content = Ektron.Xml.serializeXhtml(contentElem[0].childNodes);
            }
        }

        if (targetsLocaleArray != null) {
            for (var i = 0; i < targetsLocaleArray.length; i++) {
                var localeId = getLocaleId(targetsLocaleArray[i]);
                $ektron("#include_items" + localeId).prop("checked", true);
                disableIncludeCheck("exclude_items" + localeId, "include_items" + localeId);
            }
        }
        if (excludeLocaleArray != null) {
            for (var i = 0; i < excludeLocaleArray.length; i++) {
                var localeId = getLocaleId(excludeLocaleArray[i]);
                $ektron("#exclude_items" + localeId).prop("checked", true);
                disableIncludeCheck("include_items" + localeId, "exclude_items" + localeId);
            }
        }
        $ektron("#contentNotes").val(notesLocale);
    }

    function getLocaleId(locale) {
        var localeId;
        var targetCheckbox = $ektron("input[value=" + locale + "]");
        if (targetCheckbox != null && targetCheckbox.length > 0) {
            localeId = $ektron("input[value=" + locale + "]").attr("class");
        }
        return localeId;
    }
    //insert the selected languages and excluded languages for the selected content from content designer.
    function insertField() {
        var $includedCheckedItems = $ektron("input[name=include_items]:checked");
        var $excludedCheckedItems = $ektron("input[name=exclude_items]:checked");
        var contentNotes = $ektron("#contentNotes").val();
        //should be space delimited.
        var targetLanguages = "";
        var excludeLanguages = "";
        for (var i = 0; i < $includedCheckedItems.length; i++)
        {
            if (targetLanguages == "")
            {
                targetLanguages = $includedCheckedItems[i].value;
            }
            else
            {
                targetLanguages += " " + $includedCheckedItems[i].value;
            }
        }

        for (var i = 0; i < $excludedCheckedItems.length; i++)
        {
            if (excludeLanguages == "")
            {
                excludeLanguages = $excludedCheckedItems[i].value;
            }
            else
            {
                excludeLanguages += " " + $excludedCheckedItems[i].value;
            }
        }

        var sElem = "";
        if (targetLanguages != "" || excludeLanguages != "" || contentNotes != "")
        {
            if (m_oEditor.IsIE)
                m_content = getCorrectIEHTML(m_content, false);
            sElem = "<ektdesignns_localize ektdesignns_display=\"table\"";
            sElem += " targets=\"" + targetLanguages + "\"";
            sElem += " exclude=\"" + excludeLanguages + "\"";
            sElem += " note=\"" + $ektron.htmlEncode(contentNotes) + "\"";
            sElem += ">" + m_content + "</ektdesignns_localize>";
        }
        else if (m_oFieldElem != null)
        {
            sElem = m_content;
        }
    
	    CloseDlg(sElem);
    }

    function selectAll(includeitems, bSelect, excludeItems) {
        var includecheckboxVar = $ektron("#addlocalestocontent :checkbox[name=" + includeitems + "]");
        var excludecheckboxVar = $ektron("#addlocalestocontent :checkbox[name=" + excludeItems + "]");
        if (bSelect)
        {
            //disable the excluded checkboxes and enable all the included checkboxes
            if (includecheckboxVar != undefined && includecheckboxVar.length > 0)
            {
                for (var i = 0; i < includecheckboxVar.length; i++)
                {
                    includecheckboxVar[i].checked = true;
                    excludecheckboxVar[i].checked = false;
                    includecheckboxVar[i].disabled = false;
                    excludecheckboxVar[i].disabled = true;
                }
            }
        }
        else
        {
            if (includecheckboxVar != undefined && includecheckboxVar.length > 0)
            {
                for (var i = 0; i < includecheckboxVar.length; i++)
                {
                    includecheckboxVar[i].checked = false;
                    if (!includecheckboxVar[i].disabled)
                        includecheckboxVar[i].disabled = false;
                    excludecheckboxVar[i].disabled = false;
                }
            }
        }
        return false;
    }

    function disableIncludeCheck(disableCheckbox, enableCheckbox)
    {
        //disable the sender checkbox.
        var disabledcheckboxVar = $ektron("#" + disableCheckbox);
        var enabledcheckboxVar = $ektron("#" + enableCheckbox);
        if (disabledcheckboxVar != null || enabledcheckboxVar != null)
        {
            if (disabledcheckboxVar.length > 0 && enabledcheckboxVar.length > 0)
            {
                if (enabledcheckboxVar[0].checked == false)
                {
                    disabledcheckboxVar[0].disabled = false;
                }
                else
                {
                    disabledcheckboxVar[0].disabled = true;
                }
            }
        }
    }
    function getCorrectIEHTML(ieInnerHtml, convertToLowerCase) {
     var zz = ieInnerHtml;
     var z = zz.match(/<\/?\w+((\s+\w+(\s*=\s*(?:".*?"|'.*?'|[^'">\s]+))?)+\s*|\s*)\/?>/g);

      if (z){
        for (var i=0;i<z.length;i++){
          var y
              , zSaved = z[i]
              , attrRE = /\=[a-zA-Z\.\:\[\]_\(\)\&\$\%#\@\!0-9]+[?\s+|?>]/g;
          z[i] = z[i]
                  .replace(/(<?\w+)|(<\/?\w+)\s/,function(a){return a.toLowerCase();});
          y = z[i].match(attrRE);//deze match

           if (y){
            var j = 0
                , len = y.length
            while(j<len){
              var replaceRE = /(\=)([a-zA-Z\.\:\[\]_\(\)\&\$\%#\@\!0-9]+)?([\s+|?>])/g
                  , replacer = function(){
                      var args = Array.prototype.slice.call(arguments);
                      return '="'+(convertToLowerCase ? args[2].toLowerCase() : args[2])+'"'+args[3];
                    };
              z[i] = z[i].replace(y[j],y[j].replace(replaceRE,replacer));
              j++;
            }
           }
           zz = zz.replace(zSaved,z[i]);
         }
       }
      return zz;
     }
    Ektron.ready(function (event, eventName)
    {
        loadcontent();
        $('table.ektronGrid').find("th:last").addClass('right');
        $('table.ektronGrid tbody tr').find("td:last").addClass('right');
        BindOnRadWindowKeyDown();
    });
    //-->  
</script>
<body>
    <form id="form1" runat="server">
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" id="divTitleBar">
            <span id="WorkareaTitlebar">
                <asp:Literal ID="ltrToolBar" runat="server" ></asp:Literal>
            </span> 
        </div>
    </div>
    <div class="ektronPageContainer ektronPageInfo" id="addlocalestocontent" runat="server"
        style="overflow-y: auto; height: 250px">
        <asp:GridView ID="EnabledLocaleList" runat="server" AutoGenerateColumns="False" Width="100%"
            EnableViewState="False" GridLines="None" CssClass="ektronGrid">
            <HeaderStyle CssClass="title-header" />
            <RowStyle CssClass="center" />
        </asp:GridView>
        <hr />
    </div>
    <div id="origContent" class="ektronPageInfo" runat="server">
        <div class="Ektron_Dialogs_LineContainer">
            <div class="Ektron_TopSpaceSmall">
            </div>
            <div class="Ektron_StandardLine">
            </div>
        </div>
        <table width="100%">
            <tbody>
                <tr>
                    <td width="50%">
                        <a onclick="return selectAll('include_items',true,'exclude_items');" href="#" title="<%=GetMessage("lbl select all include languages")%>">
                            <%=GetMessage("lbl select all include languages")%>
                        </a><br />
                        <a onclick="return selectAll('include_items',false,'exclude_items');" href="#" title="<%=GetMessage("lbl deselect all include languages")%>">
                            <%=GetMessage("lbl deselect all include languages")%>
                        </a>
                    </td>
                    <td width="50%">
                        <a onclick="return selectAll('exclude_items',true,'include_items');" href="#" title="<%=GetMessage("lbl select all exclude languages")%>">
                            <%=GetMessage("lbl select all exclude languages")%>
                        </a><br />
                        <a onclick="return selectAll('exclude_items',false,'include_items');" href="#" title="<%=GetMessage("lbl deselect all exclude languages")%>">
                            <%=GetMessage("lbl deselect all exclude languages")%>
                        </a>
                    </td>
                </tr>
            </tbody>
        </table>
        <div class="Ektron_Dialogs_LineContainer">
            <div class="Ektron_TopSpaceSmall">
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <%=GetMessage("lbl content notes header")%>
            <br />
            <textarea rows="1" cols="2" style="height: 100px;" id="contentNotes"></textarea>
        </div>
        <div class="Ektron_Dialogs_LineContainer">
            <div class="Ektron_TopSpaceSmall">
            </div>
            <div class="Ektron_StandardLine">
            </div>
        </div>
        <ek:FieldDialogButtons ID="btnSubmit" OnOK="return insertField();" runat="server" />
    </div>
    </form>
</body>
</html>
