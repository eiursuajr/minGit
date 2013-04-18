<%@ Page Language="C#" AutoEventWireup="true" CodeFile="synonyms.aspx.cs" Inherits="Workarea_search_synonyms" %>

<%@ Reference Control="../controls/search/addsynonym.ascx" %>
<%@ Reference Control="../controls/search/viewsynonyms.ascx" %>
<%@ Reference Control="../controls/search/viewsynonym.ascx" %>
<%@ Reference Control="../controls/search/deletesynonym.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Synonyms</title>
    <asp:Literal ID="styleSheetJs" runat="server" />

    <script type="text/javascript">
	<!--
	var synonymID = '<asp:Literal id="jsSynonymId" runat="server" />';
	var action = '<asp:Literal id="jsPageAction" runat="server" />';
	function LoadLanguage(num){
		document.forms[0].action="synonyms.aspx?id="+synonymID+"&action="+action+"&LangType="+num;
		document.forms[0].submit();
		return false;
	}

    function VerifyDeleteSynonym() {
        var binReturn;
        binReturn = confirm('<asp:Literal id="jsConfirmDeleteSet" runat="server" />');
        return binReturn;
    }

    function VerifyFollowDupe() {
        var binReturn;
        binReturn = confirm('<asp:Literal id="jsConfirmFollowDupe" runat="server" />');
        return binReturn
    }

    function checkAddSynonyms(objForm) {
        // this function will check and validate the form prior to Adding the new Synonym Set
        var synonymTerms = objForm.addsynonym$synonymTerms.value;
        var binErr = false;
        var strErrMsg = "";
        var strMinTermsReq  = '<asp:Literal id="jsMinTwoTerms" runat="server" />'
        var regExpComma = new RegExp("[,]");
        var regExpSemiColon = new RegExp("[;]");
        var regExpParenethesis = new RegExp("[()]");
        var regExpLessThan = new RegExp("[<]");
        var regExpGreaterThan = new RegExp("[>]");

        if (synonymTerms.length == 0) {
            binErr = true;
            strErrMsg += strMinTermsReq;
        }
        else {
            if (!(regExpSemiColon.exec(synonymTerms) != null)) {
                binErr = true;
                strErrMsg += strMinTermsReq
            }
            else {
                var arrTerms = $ektron.trim(synonymTerms).split(";");
                binTermErr = false;
                for (termCounter = 0; termCounter < arrTerms.length; termCounter++) {
                    if (arrTerms[termCounter].length == 0) {
                        binTermErr = true;
                    }
                }
                if (binTermErr) {
                    strErrMsg += strMinTermsReq;
                }
                if (regExpComma.exec(synonymTerms) != null) {
                    binErr = true;
                    strErrMsg += '<asp:Literal id="jsTermsNoCommas" runat="server" />';
                }
                if (regExpParenethesis.exec(synonymTerms) != null) {
                    binErr = true;
                    strErrMsg += '<asp:Literal id="jsTermNoParenthesis" runat="server" />';
                }
                if (regExpLessThan.exec(synonymTerms) != null || regExpGreaterThan.exec(synonymTerms) != null)
                {
                    binErr = true;
                    strErrMsg += '<asp:Literal id="jsTermsNoLessGreater" runat="server" />';
                }
            }
        }
        if (binErr == false) {

            objForm.submit();
        }
        else {
            alert(strErrMsg);
        }
    }
	//-->
    </script>

    <style type="text/css">
        div.viewSynonymTerms
        {
            padding: 0em;
            line-height: 1.75em;
            height: 20em;
            overflow: auto;
        }
        ul.duplicates
        {
            height: 15em;
            overflow: auto;
            border: solid 1px #ccc;
            margin: .5em 0;
            padding: .25em;
            background: #fff;
            list-style-type: none;
        }
        ul.duplicates li:first-letter
        {
            text-transform: capitalize;
        }
        textarea#addsynonym_synonymTerms
        {
            height: 5em;
            font-size: 1em;
            font-family: inherit;
        }
    </style>
</head>
<body>
    <asp:Literal ID="CloseScriptJS" runat="server" />
    <form id="Form1" method="post" runat="server">
    <asp:PlaceHolder ID="DataHolder" runat="server"></asp:PlaceHolder>
    </form>
</body>
</html>
