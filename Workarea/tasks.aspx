<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" Inherits="tasks"
    CodeFile="tasks.aspx.cs" %>

<%@ Register TagPrefix="ektron" TagName="ContentDesigner" Src="controls/Editor/ContentDesignerWithValidator.ascx" %>
<%@ Register TagPrefix="uxEktron" TagName="Paging" Src="controls/paging/paging.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <meta http-equiv="Cache-Control" content="no-cache, must-revalidate" />
    <meta http-equiv="pragma" content="no-cache" />
    <title>
        <%=AppName + " Tasks"%></title>
    <asp:Literal ID="ltrStyleHelper" runat="server"></asp:Literal>
    <link rel="stylesheet" href="csslib/ektron.workarea.css" type="text/css" />
    <link rel="stylesheet" href="csslib/ektron.fixedpositiontoolbar.css" type="text/css" />
    <link rel="stylesheet" href="java/plugins/modal/ektron.modal.css" type="text/css" />
    <script type="text/javascript" src="java/toolbar_roll.js"></script>
    <script type="text/javascript" src="java/empjsfunc.js"></script>
    <script type="text/javascript" src="java/dateonlyjsfunc.js"></script>
    <script type="text/javascript" src="java/internCalendarDisplayFuncs.js"></script>

    <script type="text/javascript" src="java/ektron.workarea.js"></script>
    <script type="text/javascript" src="java/plugins/modal/ektron.modal.js"></script>
    <script type="text/javascript" src="java/stylehelper.js"></script>
    <script type="text/javascript">        var AutoNav;</script>
    <style type="text/css">
        .minHeight
        {
            padding-top: .25em !important;
            padding-bottom: .25em !important;
            line-height: 16pt !important;
        }
        a.buttonNone
        {
            background-image: url(Images/ui/icons/remove.png);
            background-position: .25em center;
        }
        a.buttonSet
        {
            /*background-image: url(Images/ui/icons/go.png);*/
            /*background-position: .25em center;*/
        }
        a.buttonViewTask
        {
            /*background-image: url(Images/ui/icons/taskView.png);*/
            /*background-position: .25em center;*/
        }
        #FormDataSubmitted > table
        {
            border-collapse: separate;
            border-spacing: 0;
            border-right: solid 1px #BBDDf6;
            border-bottom: solid 1px #ccc;
        }
        #FormDataSubmitted > table td, #FormDataSubmitted > table th
        {
            border-top: solid 1px #BBDDf6;
            border-left: solid 1px #BBDDf6;
            padding: 0 .25em;
        }
        #FormDataSubmitted > table th
        {
            background: #D5E7F4;
            color: #555;
        }
        div#dvComment p
        {
            margin: 0em;
            margin-bottom: .0001pt;
        }
        .MakeLink
        {
            color: Blue;
            border-bottom: dotted;
        }
        iframe#RadEContentIframedescription
        {
            margin-bottom: 0;
        }
    </style>
    <script type="text/javascript">
            <!--            //--><![CDATA[//><!--
        var appPath = '<asp:Literal ID="ltrAppPath" runat="server"></asp:Literal>';
        var jsId="<asp:literal id="jsId" runat="server"/>";
         
        <asp:Literal ID="ltrGenerateJS" runat="server"></asp:Literal>
        function CloseChildPage() {
            if (IsBrowserIE_Email()) {
                var pageObj = document.getElementById("FrameContainer");
                pageObj.style.display = "none";
                pageObj.style.width = "1px";
                pageObj.style.height = "1px";
            }

        }
        function ReturnChildValue(contentid, contenttitle, qlink, folderid, contentlanguage) {
            CloseChildPage();
            document.getElementById("contentidspan").innerHTML = "<div id=\"div3\" style=\"display: none;\"></div><span id=\"contentidspan\" style=\"display: inline-block; background-color: #fff; margin-right: .5em; border: solid 1px #DEDEDE; padding-right: .5em; padding-left: .5em;\">(" + contentid + ")&nbsp;" + contenttitle + "</span>";
            document.getElementById("a_change").style.visibility = "visible";
            document.getElementById("a_none").style.visibility = "visible";
            document.getElementById("content_id").value = contentid;
            document.getElementById("state").selectedIndex = 0;
            document.getElementById("state").disabled = true;
            document.getElementById("current_language").value = contentlanguage;

            var objLanguage = document.getElementById("language");

            if (("object" == typeof (objLanguage)) && (objLanguage != null)) {
                if (objLanguage.disabled == false) { objLanguage.disabled = true; }
            }
        }
        function UnSelectContent() {
            document.getElementById("contentidspan").innerHTML = "<div id=\"div3\" style=\"display: none;\"></div><div id=\"contentidspan\" style=\"display: inline-block;\">" + "<a class=\"button buttonInline greenHover minHeight buttonCheckAll\" href=\"#\" onclick=\"LoadChildPage();return true;\">Select</a></div>";
            document.getElementById("a_change").style.visibility = "hidden";
            document.getElementById("a_none").style.visibility = "hidden";
            document.getElementById("content_id").value = "";
            document.getElementById("state").selectedIndex = 0
            document.getElementById("state").disabled = false
            document.getElementById("current_language").value = 0;

            var objLanguage = document.getElementById("language");

            if (("object" == typeof (objLanguage)) && (objLanguage != null)) {
                if (objLanguage.disabled == true) { objLanguage.disabled = false; }
            }
        }
        

        function IsChildWaiting() {
            var pageObj = document.getElementById("FrameContainer");
            if (pageObj == null) {
                return (false);
            }
            if (pageObj.style.display == "") {
                return (true);
            }
            else {
                return (false);
            }
        }
        function SubmitForm(EditorPrefix, Validate) {
            if (typeof (Page_ClientValidate) == "function") {
                validationResult = Page_ClientValidate();
            }
            if (!validationResult) {
                return false;
            }
            var FormName = EditorPrefix + "Form";
            var objLanguage = document.getElementById("language");
            if (Validate.length > 0) {
                if (eval(Validate)) {
                    if (("object" == typeof (objLanguage)) && (objLanguage != null)) {
                        if (objLanguage.disabled == true) { objLanguage.disabled = false; }
                    }
                    document.forms[FormName].submit();
                    return false;
                }
                else {
                    return false;
                }
            }
            else {
                if (("object" == typeof (objLanguage)) && (objLanguage != null)) {
                    if (objLanguage.disabled == true) { objLanguage.disabled = false; }
                }
                document.forms[FormName].submit();
                return false;
            }
        }
        function VerifyForm() {
            var regexp1 = /"/gi;
            document.getElementById("task_title").value = Trim(document.getElementById("task_title").value.replace(regexp1, "'"));
            if (document.getElementById("task_title").value == "") {
                alert('<asp:Literal ID="ltrTitleRequired" runat="server"></asp:Literal>');
                return false;
            }
            if (document.getElementById("content_id").value == "" || document.getElementById("content_id").value == "0") {
                alert('<asp:Literal ID="ltrvalidc" runat="server"></asp:Literal>');
                return false;
            }
            if ((document.getElementById("assigned_to_user_id").value == "") && (document.getElementById("assigned_to_usergroup_id").value == "")) {
                alert('<asp:Literal ID="ltrtaskc" runat="server"></asp:Literal>');
                return false;
            }
            if(typeof document.getElementById("start_date").value != "undefined")
            {
                document.getElementById("start_date").value = Trim(document.getElementById("start_date").value);
                document.getElementById("hdnstartdate").value = Trim(document.getElementById("start_date").value);
            }
            if(typeof document.getElementById("due_date").value != "undefined")
            {
                document.getElementById("due_date").value = Trim(document.getElementById("due_date").value);
                document.getElementById("hdnduedate").value = Trim(document.getElementById("due_date").value);
            }
            if((typeof document.getElementById("start_date").value != "undefined") && (typeof document.getElementById("due_date").value != "undefined"))
            {
                if (document.getElementById("start_date").value != "" && document.getElementById("due_date").value != "") {
                    if (!EkDTCompareDates(document.getElementById("start_date"), document.getElementById("due_date"))) {
                        var msg = '<asp:Literal ID="ltrStartDatec" runat="server"></asp:Literal>';
                        alert(msg);
                        return false;
                    }
                }
            }
            return true;
        }


            function CheckKeyValue(item, keys) {
                var keyArray = keys.split(",");
                for (var i = 0; i < keyArray.length; i++) {
                    if ((document.layers) || ((!document.all) && (document.getElementById))) {
                        if (item.which == keyArray[i]) {
                            return false;
                        }
                    }
                    else {
                        if (event.keyCode == keyArray[i]) {
                            return false;
                        }
                    }
                }
            }

            function Trim(string) {
                if (string.length > 0) {
                    string = RemoveLeadingSpaces(string);
                }
                if (string.length > 0) {
                    string = RemoveTrailingSpaces(string);
                }
                return string;
            }

            function RemoveLeadingSpaces(string) {
                while (string.substring(0, 1) == " ") {
                    string = string.substring(1, string.length);
                }
                return string;
            }

            function RemoveTrailingSpaces(string) {
                while (string.substring((string.length - 1), string.length) == " ") {
                    string = string.substring(0, (string.length - 1));
                }
                return string;
            }

            function selectUser(userType, id, name, showSelect) {
                var NavObjUser;
                var NavObjNoUser;
                var NS4;
                if (document.layers) {
                    NS4 = true;
                    document.taskinfo.nsfourname.value = name;
                }
                else if (document.all) {
                    NavObjUser = document.all["user"];
                    NavObjNoUser = document.all["nouser"];
                }
                else {
                    NavObjUser = document.getElementById("user");
                    NavObjNoUser = document.getElementById("nouser");
                }
                if (!NS4) {
                    NavObjNoUser.style.display = "none";
                }
                if (userType == "0") {
                    document.getElementById("assigned_to_user_id").value = "";
                    document.getElementById("assigned_to_usergroup_id").value = "0";
                    if (!NS4) {
                        if (showSelect == "1") {
                            NavObjUser.innerHTML = name;
                        }
                        else {
                            NavObjUser.innerHTML = name;
                        }
                    }
                }
                else if (userType == "1") {
                    document.getElementById("assigned_to_user_id").value = id;
                    document.getElementById("assigned_to_usergroup_id").value = "";
                    document.getElementById("assigned_by_user_id").value = document.getElementById("current_user_id").value;
                    if (!NS4) {
                        if (showSelect == "1") {
                            NavObjUser.innerHTML = "<img src='" + appPath + "images/UI/Icons/user.png' border='0' align='absbottom'>" + name;
                        }
                        else {
                            NavObjUser.innerHTML = "<img src='" + appPath + "images/UI/Icons/user.png' border='0' align='absbottom'>" + name;
                        }

                    }
                }
                else if (userType == "2") {
                    document.getElementById("assigned_to_user_id").value = "";
                    document.getElementById("assigned_to_usergroup_id").value = id;
                    document.getElementById("assigned_by_user_id").value = document.getElementById("current_user_id").value;
                    if (!NS4) {
                        if (showSelect == "1") {
                            NavObjUser.innerHTML = "<img src='" + appPath + "images/UI/Icons/users.png' border='0' align='absbottom'>" + name;

                        }
                        else {
                            NavObjUser.innerHTML = "<img src='" + appPath + "images/UI/Icons/users.png' border='0' align='absbottom'>" + name;
                        }
                    }
                }
                if (!NS4) {
                    NavObjUser.style.display = "block";
                }
            }

            function ShowUsers() {
                PopUpWindow("selectusergroup.aspx?id=" + document.getElementById("content_id").value + "&LangType=" + document.getElementById("current_language").value, "SelectUser", 400, 300, 1, 1);
            }

            function ConfirmDelete() {
                return confirm('<asp:Literal ID="ltrDeleteTaskc" runat="server"></asp:Literal>');
            }

            function GetTaskType() {
                var selTaskType = document.getElementById("task_type");

                if (("object" == typeof (selTaskType)) && (selTaskType != null)) {
                    return selTaskType.value;
                }
                else {
                    return -1;
                }
            }

            function SubmitTaskTypeForm(FormName, ValidateFunction) {
                if (ValidateFunction.length > 0) {
                    if (eval(ValidateFunction)) {
                        document.forms[FormName].submit();
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else {
                    document.forms[FormName].submit();
                    return true;
                }
            }

            function FieldIsEmpty(TextField, FieldName_Message) {
                var strValue;

                strValue = Trim(TextField.value);
                if (0 == strValue.length) {
                    alert(FieldName_Message);
                    TextField.focus();
                    return true;
                }
                else {
                    return false;
                }
            }

            function FieldExceedMax(TextField, FieldName_Message, lngMax) {
                var strValue;

                strValue = Trim(TextField.value);
                if (strValue.length > lngMax) {
                    alert(FieldName_Message + ' ' + lngMax + '.');
                    TextField.focus();
                    return true;
                }
                else {
                    return false;
                }
            }

            function VerifyTaskTypeForm() {
                var regexp1 = /"/g; 	//g -replace all matches; i - case insensitive

                if (true == document.forms.AddTaskTypeForm.radio_new_category.checked) {
                    document.forms.AddTaskTypeForm.new_category.value = Trim(document.forms.AddTaskTypeForm.new_category.value.replace(regexp1, "'"));

                    if (true == FieldIsEmpty(document.forms.AddTaskTypeForm.new_category, '<asp:Literal ID="ltrNewTaskCatReq" runat="server"></asp:Literal>')) {
                        return false;
                    }

                    if (true == FieldExceedMax(document.forms.AddTaskTypeForm.new_category, '<asp:Literal ID="ltrAllTaskCatMax" runat="server"></asp:Literal>', 50)) {
                        return false;
                    }

                    if (true == DoesTaskTypeExist(document.forms.AddTaskTypeForm.new_category.value, "")) {
                        return false;
                    }
                }

                document.forms.AddTaskTypeForm.new_task_type_title.value = Trim(document.forms.AddTaskTypeForm.new_task_type_title.value.replace(regexp1, "'"));

                if (true == FieldIsEmpty(document.forms.AddTaskTypeForm.new_task_type_title, '<asp:Literal ID="ltrTaskTypeReq" runat="server"></asp:Literal>')) {
                    return false;
                }

                if (true == FieldExceedMax(document.forms.AddTaskTypeForm.new_task_type_title, '<asp:Literal ID="ltrTaskTypeMax" runat="server"></asp:Literal>', 50)) {
                    return false;
                }

                if (true == document.forms.AddTaskTypeForm.radio_existing_category.checked) {
                    if (true == DoesTaskTypeExist(document.forms.AddTaskTypeForm.existing_category[document.forms.AddTaskTypeForm.existing_category.options.selectedIndex].text, document.forms.AddTaskTypeForm.new_task_type_title.value)) {
                        return false;
                    }
                }
                document.forms.AddTaskTypeForm.new_task_type_description.value = Trim(document.forms.AddTaskTypeForm.new_task_type_description.value.replace(regexp1, "'"));

                if (true == FieldExceedMax(document.forms.AddTaskTypeForm.new_task_type_description, '<asp:Literal ID="ltrTaskTypeDescMax" runat="server"></asp:Literal>', 255)) {
                    return false;
                }

                return true;
            }

            function VerifyEditTaskTypeForm() {
                var regexp1 = /"/g; 	//g -replace all matches; i - case insensitive
                var objForm = document.forms.EditTaskTypeForm;
                objForm.task_type_title.value = Trim(objForm.task_type_title.value.replace(regexp1, "'"));

                if (true == FieldIsEmpty(objForm.task_type_title, '<asp:Literal ID="ltrTaskTypeReq2" runat="server"></asp:Literal>')) {
                    return false;
                }

                if (true == FieldExceedMax(objForm.task_type_title, '<asp:Literal ID="ltrTaskTypeMax2" runat="server"></asp:Literal>', 50)) {
                    return false;
                }

                if (true == DoesTaskTypeExist(objForm.category_name.value, objForm.task_type_title.value)) {
                    return false;
                }

                objForm.task_type_description.value = Trim(objForm.task_type_description.value.replace(regexp1, "'"));

                if (true == FieldExceedMax(objForm.task_type_description, '<asp:Literal ID="ltrTaskTypeDescMax2" runat="server"></asp:Literal>', 255)) {
                    return false;
                }

                if (typeof objForm.task_type_active != "undefined" && true == objForm.task_type_active.checked) {
                    if (false == confirm('<asp:Literal ID="ltrMarkInActive" runat="server"></asp:Literal>')) {
                        return false;
                    }
                }
                return true;
            }

            function VerifyCategoryChange(elemSave, categoryID) {
                var regexp1 = /"/g; 	//g -replace all matches; i - case insensitive
                var strValue;
                var aSplit = new Array;
                document.getElementById("task_category_change").value = Trim(document.getElementById("task_category_change").value.replace(regexp1, "'"));
                if (true == FieldIsEmpty(document.getElementById("task_category_change"), '<asp:Literal ID="ltrTaskTypeReq3" runat="server"></asp:Literal>')) {
                    return false;
                }

                if (true == FieldExceedMax(document.getElementById("task_category_change"), '<asp:Literal ID="ltrTaskTypeMax3" runat="server"></asp:Literal>', 50)) {
                    return false;
                }

                if (true == DoesTaskTypeExist(document.getElementById("task_category_change").value, "")) {
                    return false;
                }

                strValue = document.getElementById("task_category_change").value;
                aSplit = strValue.split("");
                strValue = "";
                for (var i = 0; i < aSplit.length; i++) {
                    if ('&' != aSplit[i]) {
                        strValue = strValue + aSplit[i];
                    }
                    else {
                        strValue = strValue + '<asp:Literal ID="ltrEncode" runat="server"></asp:Literal>';
                    }
                }
                document.getElementById(elemSave).href = "tasks.aspx?action=ViewTaskType&task_category_title=" + strValue;
                document.getElementById(elemSave).href = document.getElementById(elemSave).href + "&task_type_id=" + categoryID;
                return true;
            }
            function AddSelectOption(selObj, optObj) {
                if ((typeof (selObj.options.add)).toLowerCase() != 'undefined') {
                    selObj.options.add(optObj);
                } else {
                    selObj.options[selObj.options.length] = optObj;
                }
            }
            function DoesTaskTypeExist(strCategory, strType) {
                if (null == aAllExistingTaskType[0]) {
                    return false;
                }
                else {
                    for (var i = 0; i < aAllExistingTaskType.length; i++) {
                        if ((Trim(aAllExistingTaskType[i][1])).toLowerCase() == (Trim(strCategory)).toLowerCase()) {
                            if (strType != "") { strType = Trim(strType); }
                            if ("" == strType) {
                                var objCompare;
                                objCompare = document.getElementById("comparecategoryid");
                                if ("object" == typeof (objCompare) && objCompare != null) {
                                    if (aAllExistingTaskType[i][0] != (objCompare.value)) {
                                        alert('<asp:Literal ID="ltrTaskTypeExists" runat="server"></asp:Literal>');
                                        return true;
                                    }
                                }
                                else {
                                    alert('<asp:Literal ID="ltrTaskTypeExists2" runat="server"></asp:Literal>');
                                    return true;
                                }
                            }
                            else {
                                for (var j = 1; j < aAllExistingTaskType[i].length; j++) {
                                    if ((aAllExistingTaskType[i][j][1]).toLowerCase() == strType.toLowerCase()) {
                                        var objCompareType;
                                        objCompareType = document.getElementById("comparetasktypeid");
                                        if ("object" == typeof (objCompareType) && objCompareType != null) {
                                            if (aAllExistingTaskType[i][j][0] != (objCompareType.value)) {
                                                alert('<asp:Literal ID="ltrTaskExistsSameParent" runat="server"></asp:Literal>');
                                                return true;
                                            }
                                        }
                                        else {
                                            alert('<asp:Literal ID="ltrTaskExistsSameParent2" runat="server"></asp:Literal>');
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }

            function ConfirmDeleteTaskType() {
                var blnChecked = false;
                var lngIndex;
                var strID;
                var aSplit = new Array;
                var strSelectedTaskType = "";
                var inputs = document.getElementsByTagName('input');

                for (lngIndex = 0; lngIndex < inputs.length; lngIndex++) {
                    if (inputs[lngIndex].type == "checkbox") {
                        if (inputs[lngIndex].id != "ckb_all") {
                            strID = inputs[lngIndex].id;
                            aSplit = strID.split('_');
                            if ("ckb" == aSplit[0]) {
                                if (true == inputs[lngIndex].checked) {
                                    blnChecked = true;
                                    if ("" == strSelectedTaskType) {
                                        strSelectedTaskType = aSplit[aSplit.length - 1];
                                    }
                                    else {
                                        strSelectedTaskType = strSelectedTaskType + "," + aSplit[aSplit.length - 1];
                                    }
                                }
                            }
                        }
                    }
                }


                if (true == blnChecked) {
                    if (true == confirm('<asp:Literal ID="ltrDeleteTaskType" runat="server"></asp:Literal>')) {
                        document.frmViewTaskType.tasktype_ids.value = Trim(strSelectedTaskType);
                        document.frmViewTaskType.submit();
                    }
                }
                else {
                    alert('<asp:Literal ID="ltrSelectOneTaskType" runat="server"></asp:Literal>');
                }
                return false;
            }

            function DisplayTaskCategoryDropDown(strFormName) {
                var lngIndex;
                var newElem = document.createElement("OPTION");
                newElem.value = -1;
                newElem.text = '[' + '<asp:Literal ID="ltrDDNotSpecified" runat="server"></asp:Literal>' + ']';
                document.forms[strFormName].task_category.length = 0;
                AddSelectOption(document.forms[strFormName].task_category, newElem);

                if (aAllCategory[0] != null) {
                    for (lngIndex = 0; lngIndex < aAllCategory.length; lngIndex++) {
                        if (aAllCategory[lngIndex][2][0] != null) {
                            var bAddOption = true;
                            if (aAllCategory[lngIndex][4] > 1) // check activity for form task
                            {
                                if (strFormName.indexOf("Add") > -1 || strFormName.indexOf("Edit") > -1) {
                                    bAddOption = false;
                                }
                            }
                            if (bAddOption) {
                                var newElem = document.createElement("OPTION");
                                newElem.value = aAllCategory[lngIndex][0];
                                newElem.text = aAllCategory[lngIndex][1];
                                AddSelectOption(document.forms[strFormName].task_category, newElem);
                            }
                        }
                    }
                }
            }

            function DisplayTaskTypeDropDown(strFormName) {
                var lngIndex;
                var aAllTaskType = new Array;
                var lngValue;
                var newElem = document.createElement("OPTION");
                var selSelect = document.forms[strFormName].task_category;

                lngValue = selSelect.options[selSelect.selectedIndex].value;
                document.forms[strFormName].task_type.length = 0;

                if (lngValue > 0) {
                    for (lngIndex = 0; lngIndex < aAllCategory.length; lngIndex++) {
                        if (lngValue == aAllCategory[lngIndex][0]) {
                            aAllTaskType = aAllCategory[lngIndex][2];
                        }
                    }

                    if (aAllTaskType[0] != null) {
                        for (lngIndex = 0; lngIndex < aAllTaskType.length; lngIndex++) {
                            newElem = document.createElement("OPTION");
                            newElem.value = aAllTaskType[lngIndex][0];
                            newElem.text = aAllTaskType[lngIndex][1];
                            AddSelectOption(document.forms[strFormName].task_type, newElem);
                        }
                    }
                }
                else {
                    newElem = document.createElement("OPTION");
                    newElem.value = -1;
                    newElem.text = '[' + '<asp:Literal ID="ltrDDNotSpecified2" runat="server"></asp:Literal>' + ']';
                    AddSelectOption(document.forms[strFormName].task_type, newElem);
                }

            }

            function SelectTaskCategoryDropDown(strFormName, valValue) {
                var lngIndex;
                var newElem = document.createElement("OPTION");

                for (lngIndex = 0; lngIndex < document.forms[strFormName].task_category.options.length; lngIndex++) {
                    newElem = document.forms[strFormName].task_category.options[lngIndex];
                    if (valValue == newElem.value) {
                        newElem.selected = true;
                    }
                }
            }

            function SelectTaskTypeDropDown(strFormName, valValue) {
                var lngIndex;
                var newElem = document.createElement("OPTION");

                for (lngIndex = 0; lngIndex < document.forms[strFormName].task_type.options.length; lngIndex++) {
                    newElem = document.forms[strFormName].task_type.options[lngIndex];
                    if (valValue == newElem.value) {
                        newElem.selected = true;
                    }
                }
            }


            function IsBrowserIE() {
                return (document.all ? true : false);
            }

            function ConfirmBack(strFormID) {
                var blnConfirm = false;

                if ("AddTaskTypeForm" == strFormID) {
                    if (Trim(document.forms.AddTaskTypeForm.new_task_type_title.value) != "") {
                        blnConfirm = true;
                    }
                }
                else if ("EditTaskTypeForm" == strFormID) {
                    blnConfirm = true;
                }

                if (true == blnConfirm) {
                    return window.confirm('<asp:Literal ID="ltrGoBackNoSave" runat="server"></asp:Literal>');
                }
                else {
                    return true;
                }
            }

            function LoadTaskTypePage(strTaskTypeInfo) {
                var pageObj, frameObj
                frameObj = document.getElementById("TaskTypeChildPage");
                if ((typeof (frameObj) == "object") && (frameObj != null)) {
                    frameObj.src = "blankredirect.aspx?tasks.aspx?" + strTaskTypeInfo;
                    pageObj = document.getElementById("TaskTypeFrameContainer");
                    pageObj.style.display = "";
                    pageObj.style.width = "85%";
                    pageObj.style.height = "380px";

                    pageObj = document.getElementById("TaskTypeOverLay");
                    pageObj.style.display = "";
                    pageObj.style.width = "100%";
                    pageObj.style.height = "100%";
                }
            }

            function CloseTaskTypeChildPage(bRefresh, input_value, lngCategory, lngType) {
                var pageObj = document.getElementById("TaskTypeFrameContainer");
                var form_name = "";
                var aStore = new Array;
                pageObj.style.display = "none";
                pageObj.style.width = "1px";
                pageObj.style.height = "1px";
                pageObj = document.getElementById("TaskTypeOverLay");
                pageObj.style.display = "none";
                pageObj.style.width = "1px";
                pageObj.style.height = "1px";

                if ('boolean' == typeof (bRefresh) && (true == bRefresh)) {
                    aStore = input_value;
                    aAllCategory = aStore;

                    if ("AddTask" == action_client) {
                        form_name = "AddTaskForm";
                        this.location.reload();
                        
                    }
                    else  //EditTask
                    {
                        form_name = "EditTaskForm";
                    }

                    DisplayTaskCategoryDropDown(form_name);
                    SelectTaskCategoryDropDown(form_name, lngCategory);
                    DisplayTaskTypeDropDown(form_name);
                    SelectTaskTypeDropDown(form_name, lngType);
                }
                else
                {
                  if(false== bRefresh)
                  {
                    window.parent.$ektron('#TaskTypeFrameContainer, #TaskTypeOverLay').hide();
                  }
                }
            }

            function CloseTaskTypePage(bRefresh, input_value, lngCategory, lngType)   //this function is copied from email.asp: CloseEmailChildPage()
            {
                if (typeof (parent) != "undefined") {
                    CloseTaskTypeChildPage(bRefresh, input_value, lngCategory, lngType);
                }
                else if (!IsBrowserIE()) {
                    window.close(); // For Netscape, this is running in a popup-window.
                }
            }

            function CloseEmailChildPage() {
                var pageObj = document.getElementById("EmailFrameContainer");

                // Configure the email window to be invisible:
                pageObj.style.display = "none";
                pageObj.style.width = "1px";
                pageObj.style.height = "1px";

                // Ensure that the transparent layer does not cover any of the parent window:
                pageObj = document.getElementById("EmailActiveOverlay");
                pageObj.style.display = "none";
                pageObj.style.width = "1px";
                pageObj.style.height = "1px";
            }

            function LoadUserListChildPage() {
                var idx, name, prefix, userGrpId, pageObj, qtyElements, hid1, hid2, frameObj, haveTargets = false;

                hid1 = document.getElementById("rptHtml");
                if (hid1.value != "") {
                    hid1.value = "";
                }
                hid1.value = document.forms[0].innerHTML; // FF does not support outerHTML;
                document.getElementById("rptTitle").value = document.getElementById("WorkareaTitlebar").innerHTML;
                // Build either a user array or a group array:
                if (IsBrowserIE_Email()) {
                    frameObj = document.getElementById("EmailChildPage");
                    if ((typeof (frameObj) == "object") && (frameObj != null)) {
                        frameObj.src = "blankredirect.aspx?SelectUserGroup.aspx?action=Report";
                        pageObj = document.getElementById("EmailFrameContainer");
                        pageObj.style.display = "";
                        pageObj.style.width = "85%";
                        pageObj.style.height = "90%";

                        pageObj = document.getElementById("EmailActiveOverlay");
                        pageObj.style.display = "";
                        pageObj.style.width = "100%";
                        pageObj.style.height = "100%";
                    }
                }
                else {
                    PopUpWindow_Email("blankredirect.aspx?SelectUserGroup.aspx?action=Report", "CMSEmail", 490, 500, 1, 1);
                }
            }
            $ektron("#TaskTypeFrameContainer").modal(
            {
                trigger: '',
                modal: true,
                toTop: true,
                onShow: function(hash) {
                    hash.o.fadeIn();
                    hash.w.fadeIn();
                },
                onHide: function(hash) {
                    hash.w.fadeOut("fast");
                    hash.o.fadeOut("fast", function() {
                        if (hash.o) {
                            hash.o.remove();
                        }
                    }
                    );
                }
            });

        var aShownTask = new Array;
        var blnHasNotSpecified = false;

        function IsAllChecked() {
            for (var i = 0; i < document.forms.viewtasks.elements.length; ++i) {
                if ("checkbox" == document.forms.viewtasks.elements[i].type) {
                    var obj = document.getElementById("task" + document.forms.viewtasks.elements[i].id);
                    if ('object' == typeof (obj) && null != obj) {
                        if ('undefined' != typeof (obj.style)) {
                            if (obj.style.display != "none") {
                                if (true != document.forms.viewtasks.elements[i].checked) {
                                    if (document.forms.viewtasks.elements[i].name != "all") {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }


        function checkAll(bChecked) {
            for (var i = 0; i < document.forms.viewtasks.elements.length; ++i) {
                if (document.forms.viewtasks.elements[i].type == "checkbox") {
                    var obj = document.getElementById("task" + document.forms.viewtasks.elements[i].id);
                    if ('object' == typeof (obj) && null != obj) {
                        if ('undefined' != typeof (obj.style)) {
                            if (obj.style.display != "none") {
                                document.forms.viewtasks.elements[i].checked = bChecked;
                            }
                        }
                    }
                }
            }
        }

        //END Task Type - LZ

        function unCheckAll(bChecked) {
            for (var i = 0; i < document.forms.viewtasks.elements.length; i++) {
                if (document.forms.viewtasks.elements[i].type == "checkbox") {
                    if (document.forms.viewtasks.elements[i].checked)
                        document.forms.viewtasks.elements[i].checked = false;
                }
            }
        }


        function setTaskStateForSelTasks(purgeTasks) {
            var bFound = false;
            var bRet;
            var taskIDVal = "";
            // Task Type - LZ
            for (var i = 0; i < document.forms.viewtasks.elements.length; i++) {
                if ("checkbox" == document.forms.viewtasks.elements[i].type) {
                    if (document.forms.viewtasks.elements[i].checked & document.forms.viewtasks.elements[i].value != "on") {
                        var obj = document.getElementById("task" + document.forms.viewtasks.elements[i].id);
                        if ('object' == typeof (obj) && null != obj) {
                            if ('undefined' != typeof (obj.style)) {
                                if (obj.style.display != "none") {
                                    if (document.forms.viewtasks.elements[i].name != "all") {
                                        taskIDVal = taskIDVal + document.forms.viewtasks.elements[i].value + ",";
                                        bFound = true;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            //END Task Type - LZ
            if (bFound == false) {
                alert('<asp:Literal ID="ltrAtleastTaskc" runat="server"></asp:Literal>');
                return false;
            }
            else {
                if (purgeTasks == 1)
                    bRet = confirm('<asp:Literal ID="ltrPurgeTaskc" runat="server"></asp:Literal>');
                else if (purgeTasks == 2)
                    bRet = confirm('<asp:Literal ID="ltrAssignTaskc" runat="server"></asp:Literal>');
                else if (purgeTasks == 0)
                    bRet = confirm('<asp:Literal ID="ltrChangeTaskc" runat="server"></asp:Literal>');
                if (bRet) {
                    document.getElementById("taskids").value = taskIDVal;
                    document.getElementById("purge").value = purgeTasks;
                    document.forms.viewtasks.action = "<asp:literal id="ltrFormAction" runat="server"/>";
                    document.forms.viewtasks.submit();
                }
            }
        }

        function checkAllFalse() {
            document.forms.viewtasks.all.checked = false;
        }

        function getTaskForUser() {
            document.forms.viewtasks.submit();
        }

        // ### Task Type Client Function for "ViewTasks" ###
        function AddShownTaskID(strID) {
            var lngTaskType;
            var aSplit;
            var lngIndex;
            var lngIndex2;

            aShownTask[aShownTask.length] = strID;
            aSplit = strID.split('_');

            if ("NotS" == aSplit[aSplit.length - 1]) {
                blnHasNotSpecified = true;
            }
            else {
                lngTaskType = parseInt(aSplit[aSplit.length - 1]);

                if ((lngTaskType > 0) && (aAllCategory[0] != null)) {
                    for (lngIndex = 0; lngIndex < aAllCategory.length; lngIndex++) {
                        if (aAllCategory[lngIndex][2][0] != null) {
                            for (lngIndex2 = 0; lngIndex2 < aAllCategory[lngIndex][2].length; lngIndex2++) {
                                if (lngTaskType == aAllCategory[lngIndex][2][lngIndex2][0]) {
                                    aAllCategory[lngIndex][2][lngIndex2][2] = true;
                                    aAllCategory[lngIndex][3] = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        function FillInShowTaskType() {
            var lngIndex;

            var newElem = document.createElement("OPTION");
            newElem.value = 0;
            newElem.text = '<asp:Literal ID="ltrDDAll" runat="server"></asp:Literal>';
            AddSelectOption(document.forms["viewtasks"].show_task_type, newElem);

            if (true == blnHasNotSpecified) {
                var newElem = document.createElement("OPTION");
                newElem.value = -1;
                newElem.text = '[' + '<asp:Literal ID="ltrDDNotSpecified3" runat="server"></asp:Literal>' + ']';
                AddSelectOption(document.forms["viewtasks"].show_task_type, newElem);
            }

            if (aAllCategory[0] != null) {
                for (lngIndex = 0; lngIndex < aAllCategory.length; lngIndex++) {
                    if (true == aAllCategory[lngIndex][3])  //if catgory will display
                    {
                        var newOptGroup = document.createElement("OPTGROUP");
                        newOptGroup.label = aAllCategory[lngIndex][1];

                        var is55 = false;
                        if (true == IsBrowserIE()) {
                            var ua = window.navigator.userAgent.toLowerCase();
                            var pIE = ua.indexOf("msie ");
                            var ver = parseFloat(ua.substring(pIE + 5));
                            is55 = (ver == 5.5);
                        }

                        if (true == is55) {
                            for (lngIndex2 = 0; lngIndex2 < aAllCategory[lngIndex][2].length; lngIndex2++) {
                                if (aAllCategory[lngIndex][2][lngIndex2] != null && true == aAllCategory[lngIndex][2][lngIndex2][2]) {
                                    var newElem = document.createElement("OPTION");
                                    newElem.value = aAllCategory[lngIndex][2][lngIndex2][0];
                                    newElem.innerHTML = aAllCategory[lngIndex][2][lngIndex2][1] + " ";
                                    //newOptGroup.appendChild(newElem);
                                    document.forms["viewtasks"].show_task_type.appendChild(newElem);
                                }
                            }
                        }
                        else {
                            for (lngIndex2 = 0; lngIndex2 < aAllCategory[lngIndex][2].length; lngIndex2++) {
                                if (aAllCategory[lngIndex][2][lngIndex2] != null && true == aAllCategory[lngIndex][2][lngIndex2][2]) {
                                    var newElem = document.createElement("OPTION");
                                    newElem.value = aAllCategory[lngIndex][2][lngIndex2][0];
                                    newElem.innerHTML = aAllCategory[lngIndex][2][lngIndex2][1] + " ";
                                    newOptGroup.appendChild(newElem);
                                }
                            }

                            document.forms["viewtasks"].show_task_type.appendChild(newOptGroup);
                        }
                    }
                }
            }

        }

        function RefreshTasksWithTaskType() {
            var lngTaskType;
            var lngIndex;
            var aSplit;

            if (aShownTask.length <= 0) {
                return false;
            }
            //checkAllFalse();
            lngTaskType = document.getElementById("show_task_type").value;
            for (lngIndex = 0; lngIndex < aShownTask.length; lngIndex++) {
                aSplit = aShownTask[lngIndex].split('_');

                if (lngTaskType < 0)			//Not specified
                {
                    if ("NotS" == aSplit[aSplit.length - 1]) {
                        document.getElementById(aShownTask[lngIndex]).style.display = (true == IsBrowserIE() ? "block" : "table-row");
                    }
                    else {
                        document.getElementById(aShownTask[lngIndex]).style.display = "none";
                    }
                }
                else if (0 == lngTaskType)	//Show ALL
                {
                    document.getElementById(aShownTask[lngIndex]).style.display = (true == IsBrowserIE() ? "block" : "table-row");
                }
                else	//Show Only the task type selected
                {
                    if ("NotS" == aSplit[aSplit.length - 1]) {
                        document.getElementById(aShownTask[lngIndex]).style.display = "none";
                    }
                    else if (lngTaskType == parseInt(aSplit[aSplit.length - 1])) {
                        document.getElementById(aShownTask[lngIndex]).style.display = (true == IsBrowserIE() ? "block" : "table-row");
                    }
                    else {
                        document.getElementById(aShownTask[lngIndex]).style.display = "none";
                    }
                }
            }

            document.forms.viewtasks.all.checked = eval(IsAllChecked());
        }

        function GoToPage(ui)
        {
            var selectedPage = Ektron.Workarea.Paging.selectedpage(ui);
            var basepath = '<asp:literal id="ltrPageBasePath" runat="server"/>';
            window.location = basepath.replace("{0}", selectedPage + 1);
        }
            //--><!]]>
    </script>
    <asp:Literal ID="ltrEmailScript" runat="server"></asp:Literal>
</head>
<body style="overflow:auto">
    <asp:Panel ID="pnlAddTask" runat="server" Visible="false">
        <form name="taskinfo" id="AddTaskForm" runat="server">
        <input type="hidden" name="OrderBy" value="<%= (Server.HtmlEncode(Request.QueryString["OrderBy"])) %>"
            id="Hidden1" />
        <div id="FrameContainer" style="position: absolute; top: 68px; left: 55px; width: 1px;
            height: 1px; display: none;">
            <iframe id="ChildPage" name="ChildPage" frameborder="yes" marginheight="2" marginwidth="2"
                width="100%" height="100%" scrolling="auto"></iframe>
        </div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" title="Add Task">
                <%=m_refStyle.GetTitleBar(MsgHelper.GetMessage("btn add task"))%>
            </div>
            <div class="ektronToolbar">
                <table>
                    <tr>
                        <asp:Literal ID="ltrAddTaskToolBar" runat="server"></asp:Literal>
                        <td>
                            <%=m_refStyle.GetHelpButton(action, "")%>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <table class="ektronGrid">
                <tr>
                    <td class="label" title="Title">
                        <%=MsgHelper.GetMessage("generic title label")%>
                    </td>
                    <td class="value">
                        <input type="Text" title="Enter Title here" name="task_title" maxlength="200" value="<%=objTask.TaskTitle%>"
                            id="task_title" />
                    </td>
                </tr>
                <asp:Literal ID="ltrAddTask" runat="server"></asp:Literal>
                <tr>
                    <td class="label" title="Start Date">
                        <%=MsgHelper.GetMessage("generic start date label")%>
                    </td>
                    <td class="value" title="Select a Start Date here">
                        <%=DisplayDateSelector("taskinfo", "start_date_span", "start_date", true, objTask.StartDate)%>
                    </td>
                </tr>
                <tr>
                    <td class="label" title="Due Date">
                        <%=MsgHelper.GetMessage("lbl Due Date")%>:
                    </td>
                    <td class="value" title="Select Due Date here">
                        <%=DisplayDateSelector("taskinfo", "due_date", "due_date", true, objTask.DueDate)%>
                    </td>
                </tr>
            </table>
            <div class="ektronHeader" title="Description">
                <%=MsgHelper.GetMessage("description label")%></div>
            <asp:PlaceHolder ID="AddTaskValidatorHolder" runat="server" />
            <div class="ektronBorder addTaskEditorWrapper">
                <asp:PlaceHolder ID="AddTaskEditorHolder" runat="server" />
                <!-- eWebEditProEditor("description", "595", "200", cTask.Description)) -->
            </div>
        </div>
        <input type="hidden" name="netscape" onkeypress="return CheckKeyValue(event,'34');"
            id="Hidden2" />
        <input type="hidden" name="hdnstartdate" value="" id="hdnstartdate" />
        <input type="hidden" name="hdnduedate" value="" id="hdnduedate" />
        <input type="hidden" name="content_id" value="<%=(ContentId)%>" id="content_id" />
        <input type="hidden" name="assigned_to_user_id" value="" id="assigned_to_user_id" />
        <input type="hidden" name="assigned_to_usergroup_id" value="<% if (ContentId != -1) { %>0<% } %>"
            id="assigned_to_usergroup_id" />
        <input type="hidden" name="orderyby" value="<%=(Server.HtmlEncode(Request.QueryString["orderby"]))%>"
            id="orderyby" />
        <input type="hidden" name="ty" value="<%=(Server.HtmlEncode(Request.QueryString["ty"]))%>"
            id="ty" />
        <input type="hidden" name="stateful" value="<%=(objTask.State)%>" id="stateful" />
        <input type="hidden" name="current_user_id" value="<%=(currentUserID)%>" id="current_user_id" />
        <input type="hidden" name="assigned_by_user_id" value="<%=currentUserID%>" id="assigned_by_user_id" />
        <input type="hidden" name="current_language" value="<%=objTask.ContentLanguage%>"
            id="current_language" />
        </form>
    </asp:Panel>
    <asp:Panel ID="pnlViewTasks" runat="server" Visible="false">
        <form action="tasks.aspx?Action=UpdateStatePurge<%=closeOnFinish %><%=callBackPage%>"
        name="taskinfo" method="Post" id="viewtasks" runat="server">
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
                <%= (m_refStyle.GetTitleBar(sTitleBar)) %>
            </div>
            <div class="ektronToolbar">
                <table>
                    <tr>
                        <%bool primaryCssApplied = false; %>
                        <%if (actiontype == "both")
                          {%>
                        <% if (Convert.ToBoolean(canI["CanIAddTask"]))
                           {%>
                        <%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", "tasks.aspx?action=AddTask&orderby=" + Request.QueryString["orderby"] + "&ty=" + actiontype, MsgHelper.GetMessage("btn add task"), MsgHelper.GetMessage("btn add"), "", StyleHelper.AddButtonCssClass, !primaryCssApplied))%>
                        <%primaryCssApplied = true; %>
                        <% } %>
                        <%
                            if ((IsAdmin) & (cTasks != null && cTasks.Count > 0))
                            {
                        %>
                        <%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "tasks.aspx?action=DeleteAllTasks&orderby=" + Request.QueryString["orderby"] + "&ty=" + actiontype, MsgHelper.GetMessage("lbl delete task"), MsgHelper.GetMessage("btn delete"), "", StyleHelper.DeleteButtonCssClass, !primaryCssApplied))%>
                        <%primaryCssApplied = true; %>
                        <% } %>
                        <% } %>
                        <%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/email.png", "#", MsgHelper.GetMessage("Email Report button text"), MsgHelper.GetMessage("btn email"), "onclick=LoadUserListChildPage();", StyleHelper.EmailButtonCssClass, !primaryCssApplied))%>
                        <%primaryCssApplied = true; %>
                        <%--<%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "javascript:history.back();", MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), ""))%>--%>
                        <td>
                            <%=m_refStyle.GetHelpButton(action, "")%>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="ektronPageContainer">
            <asp:Literal ID="ltrViewTasks" runat="server"></asp:Literal>
        
            <uxEktron:Paging ID="uxPaging" runat="server" />

            <script type="text/javascript">
            <!--
                Tasks = {
                    // added Tasks object with addLoadEvent method to correct timing problem for FillInShowTaskType();
                    addOnLoadEvent: function(fnc) {
                        if (typeof window.addEventListener != "undefined")
                            window.addEventListener("load", fnc, false);
                        else if (typeof window.attachEvent != "undefined") {
                            window.attachEvent("onload", fnc);
                        }
                        else {
                            if (window.onload != null) {
                                var oldOnload = window.onload;
                                window.onload = function(e) {
                                    oldOnload(e);
                                    window[fnc]();
                                };
                            }
                            else
                                window.onload = fnc;
                        }
                    }
                }

                Tasks.addOnLoadEvent(function() {
                    FillInShowTaskType();
                });
		    //-->
            </script>

        </div>
        </form>
    </asp:Panel>
    <asp:Panel ID="pnlViewTask" Visible="false" runat="server">
	<form name="frmviewtask" action="tasks.aspx" method="post" id="Form3">
		<div class="ektronPageHeader">
		    <div class="ektronTitlebar">
			    <%= (m_refStyle.GetTitleBar("View Task")) %>
		    </div>
		    <script type="text/javascript">
		    <!--
		   	    function openComment(str)
		   	    {
		   		    if (str !="")
		   		    {
		   			    str = str + "&LangType=" + <%=objTask.LanguageID%>
					    window.open( str,"cmt_win","width=650,height=350,resizable,scrollbars,status,titlebar");
				    }
				    else
				    {
				    window.open("<%= m_refContentApi.AppPath%>taskcomment.aspx?ref_type=T&ref_id="+<%=RefId%>+ "&LangType=" + <%=objTask.LanguageID%>,"cmt_win","width=650,height=350,resizable,scrollbars,status,titlebar");
				    }
			    }
			    function DoSort(key){
				    document.frmviewtask.action="tasks.aspx?action=ViewTask&orderby="+replace_string(key," ","%20")+"&tid="+document.frmviewtask.tid.value+"&ty="+document.frmviewtask.ty.value;
                    <%if (fromViewContent != "") { %>
                        var dosorttext = document.frmviewtask.action=document.frmviewtask.action + "&fromViewContent="+ <%=fromViewContent%>;
                    <% } %>
                    document.frmviewtask.submit();
			    }
			    function replace_string(string,text,by) {
				    var strLength = string.length, txtLength = text.length;
				    if ((strLength == 0) || (txtLength == 0)) return string;
				    var i = string.indexOf(text);
				    if ((!i) && (text != string.substring(0,txtLength))) return string;
				    if (i == -1) return string;
				    var newstr = string.substring(0,i) + by;
				    if (i+txtLength < strLength)
				    newstr += replace_string(string.substring(i+txtLength,strLength),text,by);
				    return newstr;
			    }
			    function openTaskHistory(str)
			    {
				    window.open(str,"cmt_win","width=650,height=400,resizable,scrollbars,status,titlebar");
			    }
			//-->
		    </script>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <asp:Literal ID="ltrViewTaskToolBar" runat="server"></asp:Literal>
				    </tr>
			    </table>
		    </div>
		</div>
		<div class="ektronPageInfo ektronPageContainer">
		    <div class="tabContainerWrapper">
                <div class="tabContainer">
                <ul>
                    <li>
                        <a title="<%=MsgHelper.GetMessage("lbl Properties View Task")%>" href="#dvPropertiesViewTask">
                            <%=MsgHelper.GetMessage("properties text")%>
                        </a>
                    </li>
                    <li>
                        <a title="<%=MsgHelper.GetMessage("comment text")%>" href="#dvComment">
                            <%=MsgHelper.GetMessage("comments label")%>
                        </a>
                    </li>
                </ul>
                <div id="dvPropertiesViewTask">
		            <table class="ektronGrid" style="width:98%">
			            <tr>
				            <td class="label" title="Task Title"><%=(MsgHelper.GetMessage("lbl task title"))%>:</td>
				            <td class="readOnlyValue" title="<%=(objTask.TaskTitle)%>"><%=(objTask.TaskTitle)%></td>
			            </tr>
			            <tr>
				            <td class="label" title="Assigned to"><%=(MsgHelper.GetMessage("lbl Assigned to"))%>:</td>
				            <td class="readOnlyValue">
				                <asp:Literal ID="ltrAssignedTo" runat="server"></asp:Literal>
				            </td>
			            </tr>
			            <tr>
				            <td class="label" title="Assigned By"><%=(MsgHelper.GetMessage("lbl Assigned By"))%>:</td>
				            <td class="readOnlyValue" title="<%=(objTask.AssignedByUser)%>"><%=(m_refEmail.MakeByUserTaskEmailLink(objTask,true))%></td>
			            </tr>
			            <tr>
				            <td class="label" title="Language"><%=(MsgHelper.GetMessage("res_lngsel_lbl"))%></td>
				            <td class="readOnlyValue" title="<%=(objTask.Language)%>"><%=(objTask.Language)%></td>
			            </tr>
			            <asp:Literal ID="ltrViewTaskLinks" runat="server"></asp:Literal>
			            <tr>
				            <td class="label"><%=(MsgHelper.GetMessage("lbl priority"))%>:</td>
				            <asp:Literal ID="ltrViewTaskPriority" runat="server"></asp:Literal>
			            </tr>
			            <%=DisplayTaskType(action)%>
			            <tr>
				            <td class="label" title="State"><%=(MsgHelper.GetMessage("lbl state"))%>:</td>
				            <td class="readOnlyValue">
				            <asp:Literal ID="ltrViewTaskState" runat="server"></asp:Literal>
				            </td>
			            </tr>
			            <tr>
				            <td class="label" title="Start Date"><%=(MsgHelper.GetMessage("generic start date label"))%></td>
				            <td class="readOnlyValue" title="Start Date Value"><%=DisplayDateSelector("frmviewtask", "start_date", "start_date_span", false, objTask.StartDate)%></td>
			            </tr>
			            <tr>
				            <td class="label" title="Due Date"><%=(MsgHelper.GetMessage("lbl Due Date"))%>:</td>
				            <td class="readOnlyValue" title="Due Date Value"><%=DisplayDateSelector("frmviewtask", "due_date", "due_date_span", false, objTask.DueDate)%></td>
			            </tr>
			            <tr>
				            <td class="label" title="Created by"><%=(MsgHelper.GetMessage("created by label"))%></td>
				            <td class="readOnlyValue" title="<%=(objTask.CreatedByUser)%>">
					            <%=(objTask.CreatedByUser)%>
				            </td>
			            </tr>
			            <tr>
				            <td class="label" title="Description"><%=(MsgHelper.GetMessage("description label"))%></td>
				            <td class="readOnlyValue">
				                <asp:Literal ID="ltrViewTaskDescription" runat="server"></asp:Literal>
				            </td>
			            </tr>
		            </table>
		        </div>
		        <div id="dvComment">
	                <div class="ektronHeader" title="Comments"><%=(MsgHelper.GetMessage("comments label"))%></div>
                    <table class="ektronGrid"  style="width:98%">
	                    <tr class="title-header">
		                  <asp:Literal ID="ltrViewTaskComments" runat="server"></asp:Literal>  
	                    </tr>
	                    <%=ListComments(objTask.LanguageID)%>
                    </table>
                </div>
            </div>
	    </div>
	</div>
		<input type="hidden" name="orderby" value="<%=OrderBy%>" />
		<input type="hidden" name="tid" value="<%=RefId%>" />
		<input type="hidden" name="ty" value="<%=Server.HtmlEncode(Request["ty"])%>" />
	</form>
    </asp:Panel>
    <asp:Panel ID="pnlViewContentTask" runat="server" Visible="false">
        <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
			    <%= (m_refStyle.GetTitleBar(sTitleBar)) %>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "javascript:history.back();", MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true))%>
					    <td><%=m_refStyle.GetHelpButton(action, "")%></td>
				    </tr>
			    </table>
		    </div>
		</div>
		<table width="100%">
			<tr>
				<td><a href="tasks.aspx?action=viewcontenttask&cid=<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["cid"])%>&LangType=<%=languageID%>&orderby=title&ty=<%=(actiontype)%><%=callBackPage%>" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>"><%=(MsgHelper.GetMessage("generic Title"))%></a></td>
				<td><a href="tasks.aspx?action=viewcontenttask&cid=<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["cid"])%>&LangType=<%=languageID%>&orderby=id&ty=<%=(actiontype)%><%=callBackPage%>" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>"><%=(MsgHelper.GetMessage("generic ID"))%></a></td>
				<td><a href="tasks.aspx?action=viewcontenttask&cid=<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["cid"])%>&LangType=<%=languageID%>&orderby=state&ty=<%=(actiontype)%><%=callBackPage%>" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">State</a></td>
				<td><a href="tasks.aspx?action=viewcontenttask&cid=<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["cid"])%>&LangType=<%=languageID%>&orderby=priority&ty=<%=(actiontype)%><%=callBackPage%>" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">Priority</a></td>
				<td><a href="tasks.aspx?action=viewcontenttask&cid=<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["cid"])%>&LangType=<%=languageID%>&orderby=duedate&ty=<%=(actiontype)%><%=callBackPage%>" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">Due Date</a></td>
				<%
				if ((actiontype == "by") | (actiontype == "all") | (actiontype == "both")) {
				%>
				<td><a href="tasks.aspx?action=viewcontenttask&cid=<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["cid"])%>&LangType=<%=languageID%>&orderby=assignedto&ty=<%=(actiontype)%><%=callBackPage%>" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">Assigned To</a></td>
				<%
				}
                if ((actiontype == "to") | (actiontype == "all") | (actiontype == "both"))
                {
				%>
				<td><a href="tasks.aspx?action=viewcontenttask&cid=<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["cid"])%>&LangType=<%=languageID%>&orderby=assignedby&ty=<%=(actiontype)%><%=callBackPage%>" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">Assigned By</a></td>
				<%
                    }
				%>

				<td style="color: #2e6e9e;">Last Added Comment</td>
				<td style="color: #2e6e9e;">Create Date</td>
			</tr>
			<asp:Literal ID="ltrViewContentTaskBody" runat="server"></asp:Literal>
		</table>
    </asp:Panel>
    <asp:Panel ID="pnlViewTaskType" runat="server" Visible="false">
        <script type="text/javascript">
<!--

            function ToggleShowHideTaskType(strID) {
                var strDivID;
                var aSplit = new Array;

                aSplit = strID.split('_');
                strDivID = "div_" + aSplit[aSplit.length - 1];

                if ("none" == document.getElementById(strDivID).style.display) {
                    document.getElementById(strDivID).style.display = "block";
                    document.getElementById(strID).src = "<%=AppPath%>images/UI/Icons/TaskTypes/collapse.gif"
                }
                else {
                    document.getElementById(strDivID).style.display = "none";
                    document.getElementById(strID).src = "<%=AppPath%>images/UI/Icons/TaskTypes/expand.gif"
                }

            }

            function checkTaskType(strID, blnChecked) {
                var lngIndex;
                var blnAllSelected = true;
                var aSplit = new Array;
                var strCategoryCkb;

                if (false == document.getElementById(strID).checked) {
                    aSplit = strID.split('_');
                    if (aSplit.length <= 0) {
                        return false;
                    }
                    aSplit.length = aSplit.length - 1;
                    strCategoryCkb = aSplit.join('_');
                    document.getElementById(strCategoryCkb).checked = false;
                    document.getElementById("ckb_all").checked = false;
                }
            }

            function checkAll(blnChecked, strExceptID) {
                var lngIndex;
                var strID;
                var aSplit = new Array;

                for (lngIndex = 0; lngIndex < document.all.length; lngIndex++) {
                    if (document.all[lngIndex].type == "checkbox") {
                        strID = document.all[lngIndex].id;
                        aSplit = strID.split('_');
                        if ("ckb" == aSplit[0]) {
                            if (strExceptID != document.all[lngIndex].id) {
                                document.all[lngIndex].checked = blnChecked;
                            }
                        }
                    }
                }
            }

            function checkTaskTypeS(strID, blnChecked) {
                var aSplit = new Array;
                var aSplit2 = new Array;
                var strID;
                var strFrmID;
                var lngIndex;

                aSplit = strID.split('_');
                strFrmID = "frm_" + aSplit[aSplit.length - 1];

                if (null != document.getElementById(strFrmID)) {
                    for (lngIndex = 0; lngIndex < document.getElementById(strFrmID).elements.length; lngIndex++) {
                        if (document.getElementById(strFrmID).elements[lngIndex].type == "checkbox") {
                            strID = document.getElementById(strFrmID).elements[lngIndex].id;
                            aSplit = strID.split('_');
                            if ("ckb" == aSplit[0]) {
                                document.getElementById(strFrmID).elements[lngIndex].checked = blnChecked;
                            }
                        }
                    }
                }

                if (false == blnChecked) {
                    document.getElementById("ckb_all").checked = false;
                }
            }

//-->
</script>
    <%sTitleBar= MsgHelper.GetMessage("lbl view categorization"); %>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
                <%= (m_refStyle.GetTitleBar(sTitleBar)) %>
            </div>
            <div class="ektronToolbar">
                <table id="Table11">
                    <tr>
                        <%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", "tasks.aspx?action=AddTaskType" , MsgHelper.GetMessage("title add task type"), MsgHelper.GetMessage("title add task type"), "", StyleHelper.AddButtonCssClass, true))%>
                        <% if (CollectionNotEmpty(colAllCategory)) { %>
                        <%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "tasks.aspx?action=DeleteTaskType&" + callBackPage , MsgHelper.GetMessage("title delete task type"), MsgHelper.GetMessage("title delete task type"), "onclick=\"return ConfirmDeleteTaskType();\"", StyleHelper.DeleteButtonCssClass))%>
                        <% } %>
                        <td>
                            <%=m_refStyle.GetHelpButton(action, "")%>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

<script type="text/javascript">
    $ektron(document).ready(function () {
        $ektron(".ektronPageInfo td").css({
            padding: '14px'
        });
    });
</script>
        <div class="ektronPageContainer ektronPageInfo">
            <table width="100%">
                <% if (!CollectionNotEmpty(colAllCategory))
                   { %>
                    <%=MsgHelper.GetMessage("lbl no existing task type")%>
                <% }
                   else
                   { %>
                   <tr>
					<td>
						<table class="ektronGrid">
						    <tr class="title-header">
							    <td width="1%" nowrap="nowrap">
							        <table>
							            <tr>
							                <td><input title="Check here to Delete" id="ckb_all" type="checkbox" name="ckb_all" onclick="checkAll(this.checked, this.id)"/></td>
							                <td style="white-space:nowrap" title="Check to Delete"><%=MsgHelper.GetMessage("lbl check delete")%></td>
							            </tr>
							        </table>
							     </td>
							    <td title="Category"><%=MsgHelper.GetMessage("lbl task category")%></td>
						    </tr>
						    <asp:Literal ID="ltrViewTaskType" runat="server"></asp:Literal>
                        </table>
                    </td>
                   </tr>
                <% } %>
            </table>
        </div>
        <form action="tasks.aspx?action=DeleteTaskType" id="frmViewTaskType" name="frmViewTaskType"
        method="post">
        <input type="hidden" id="tasktype_ids" name="tasktype_ids" value="" />
        <input type="hidden" id="comparecategoryid" name="comparecategoryid" value="<%=lngCompareCategoryID%>" />
        </form>
    </asp:Panel>
    <asp:Panel ID="pnlDeleteAllTasks" runat="server" Visible="false">
        <script type="text/javascript">
	<!--  
		var aShownTask =new Array;
		var blnHasNotSpecified=false;

		function IsAllChecked()
		{
			for (var i = 0; i < document.forms.deletetasks.elements.length; ++i)
			{
				if (document.forms.deletetasks.elements[i].type == "checkbox")
				{
				  var obj = document.getElementById("task" + document.forms.deletetasks.elements[i].id);
				  if ('object' == typeof(obj) && null != obj)
				  {
					if ('undefined' != typeof(obj.style))
					{
						if (obj.style.display != "none")
						{
							if (true!=document.forms.deletetasks.elements[i].checked)
							{
								if (document.forms.deletetasks.elements[i].name!="all")
								{
									return false;
								}
							}
						}
					}
				  }
				}
			}
			return true;
		}


		function checkAll(bChecked)
		{
			for (var i = 0; i < document.forms.deletetasks.elements.length; i++)
			{
				if (document.forms.deletetasks.elements[i].type == "checkbox")
				{
					var obj = document.getElementById("task" + document.forms.deletetasks.elements[i].id);
					if ('object' == typeof(obj) && null != obj)
					{
						if ('undefined' != typeof(obj.style))
						{
							if (obj.style.display != "none")
							{
								document.forms.deletetasks.elements[i].checked = bChecked;
							}
						}
					}
				}
			}
		}

		function checkDeleteForm(){
			var bFound = false;
			var bRet;
			for (var i = 0; i < document.forms.deletetasks.elements.length; i++){
				if (document.forms.deletetasks.elements[i].type == "checkbox"){
					if (document.forms.deletetasks.elements[i].checked){
						bFound = true;
					}
				}
			}
			if (bFound == false){
				alert('Please select at least one task.');
				return false;
			}
			else{
				bRet = confirm('Are you sure you want to delete the selected tasks(s)?');
				if (bRet){
					document.forms.deletetasks.submit();
					return false;
				}
			}
		}
		function checkAllFalse(){
			document.forms.deletetasks.all.checked = false;
		}

		function AddShownTaskID(strID)
		{
			var lngTaskType;
			var aSplit;
			var lngIndex;
			var lngIndex2;

			aShownTask[aShownTask.length]=strID;
			aSplit=strID.split('_');

			if ("NotS"==aSplit[aSplit.length-1])
			{
				blnHasNotSpecified=true;
			}
			else
			{
				lngTaskType=parseInt(aSplit[aSplit.length-1]);

				if ((lngTaskType>0) && (aAllCategory[0]!=null))
				{
					for (lngIndex=0; lngIndex<aAllCategory.length; lngIndex++)
					{
						if (aAllCategory[lngIndex][2][0]!=null)
						{
							for (lngIndex2=0; lngIndex2<aAllCategory[lngIndex][2].length; lngIndex2++)
							{
								if (lngTaskType==aAllCategory[lngIndex][2][lngIndex2][0])
								{
									aAllCategory[lngIndex][2][lngIndex2][2]=true;
									aAllCategory[lngIndex][3]=true;
								}
							}
						}
					}
				}
			}
		}

		function FillInShowTaskType()
		{
			var lngIndex;

			var newElem=document.createElement("OPTION");
			newElem.value=0;
			newElem.text='<%=MsgHelper.GetMessage("dd all")%>';
			AddSelectOption(document.getElementById("show_task_type"), newElem);

			if (true==blnHasNotSpecified)
			{
				var newElem=document.createElement("OPTION");
				newElem.value=-1;
				newElem.text="["+"<%=MsgHelper.GetMessage("dd not specified")%>"+"]";
				AddSelectOption(document.getElementById("show_task_type"), newElem);
			}

			if (aAllCategory[0]!=null)
			{
				for (lngIndex=0; lngIndex<aAllCategory.length; lngIndex++)
				{
					if (true==aAllCategory[lngIndex][3])  //if catgory will display
					{
						var newOptGroup=document.createElement("OPTGROUP");
						newOptGroup.label=aAllCategory[lngIndex][1];

						var is55=false;
						if (true==IsBrowserIE())
						{
							var ua=window.navigator.userAgent.toLowerCase();
							var pIE=ua.indexOf("msie ");
							var ver=parseFloat(ua.substring(pIE + 5)) ;
							is55=(ver==5.5);
						}

						if (true==is55)
						{
							for (lngIndex2=0; lngIndex2<aAllCategory[lngIndex][2].length; lngIndex2++)
							{
								if (aAllCategory[lngIndex][2][lngIndex2] != null && true == aAllCategory[lngIndex][2][lngIndex2][2])
								{
									var newElem=document.createElement("OPTION");
									newElem.value=aAllCategory[lngIndex][2][lngIndex2][0]  ;
									newElem.innerHTML=aAllCategory[lngIndex][2][lngIndex2][1];
									document.getElementById("show_task_type").appendChild(newElem);
								}
							}
						}
						else
						{
							for (lngIndex2=0; lngIndex2<aAllCategory[lngIndex][2].length; lngIndex2++)
							{
								if (aAllCategory[lngIndex][2][lngIndex2] != null && true == aAllCategory[lngIndex][2][lngIndex2][2])
								{
									var newElem=document.createElement("OPTION");
									newElem.value=aAllCategory[lngIndex][2][lngIndex2][0]  ;
									newElem.innerHTML=aAllCategory[lngIndex][2][lngIndex2][1];
									newOptGroup.appendChild(newElem);
								}
							}

							document.getElementById("show_task_type").appendChild(newOptGroup);
						}
					}
				}
			}

		}

		function RefreshTasksWithTaskType()
		{
			var lngTaskType;
			var lngIndex;
			var aSplit;

			if (aShownTask.length<=0 )
			{
				return false;
			}

			lngTaskType=document.getElementById("show_task_type").value;
			for (lngIndex=0; lngIndex<aShownTask.length;lngIndex++)
			{
				aSplit= aShownTask[lngIndex].split('_');

				if (lngTaskType<0 )			//Not specified
				{
					if ("NotS"==aSplit[aSplit.length-1])
					{
						document.getElementById(aShownTask[lngIndex]).style.display = (true==IsBrowserIE()? "block":"table-row");
					}
					else
					{
						document.getElementById(aShownTask[lngIndex]).style.display = "none";
					}
				}
				else if (0==lngTaskType)	//Show ALL
				{
					document.getElementById(aShownTask[lngIndex]).style.display = (true==IsBrowserIE()? "block":"table-row");
				}
				else	//Show Only the task type selected
				{
					if ("NotS"==aSplit[aSplit.length-1])
					{
						document.getElementById(aShownTask[lngIndex]).style.display = "none";
					}
					else if (lngTaskType==parseInt(aSplit[aSplit.length-1]))
					{
						document.getElementById(aShownTask[lngIndex]).style.display = (true==IsBrowserIE()? "block":"table-row");
					}
					else
					{
						document.getElementById(aShownTask[lngIndex]).style.display = "none";
					}
				}
			}

			document.forms.deletetasks.all.checked = eval(IsAllChecked());

		}
	//-->
	</script>

        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
                <%= (m_refStyle.GetTitleBar(sTitleBar)) %>
            </div>
            <div class="ektronToolbar">
                <table id="Table21">
                    <tr>
                        <%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "tasks.aspx?action=ViewTasks&ty=" + actiontype , MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true))%>
                        <% if (Convert.ToBoolean(canI["CanIDeleteTask"]))
                           { %>
                        <%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "#", "Delete Tasks", MsgHelper.GetMessage("btn delete"), "onclick=\"checkDeleteForm();\"", StyleHelper.DeleteButtonCssClass, true )) %>
                        <% } %>
                        <td>
                            <%=m_refStyle.GetHelpButton(action, "")%>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <table class="ektronGrid">
            <tr>
                <td class="label" title="Show Task Type">
                    <% =(MsgHelper.GetMessage("lbl show task type"))%>
                </td>
                <td class="value">
                    <select name="show_task_type" id="Select1" onchange="RefreshTasksWithTaskType();">
                    </select>
                </td>
            </tr>
        </table>
        <form name="deletetasks" runat="server" id="deletetasks">
        <div class="ektronPageContainer">
            <table width="100%" class="ektronGrid">
                <tr class="title-header">
                    <td width="1">
                        <input title="Check All" type="checkbox" name="all" onclick="checkAll(document.forms.deletetasks.all.checked);"
                            id="Checkbox1" />
                    </td>
                    <td>
                        <a href="tasks.aspx?action=DeleteAllTasks&orderby=title&ty=<%=(actiontype)%>" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>"
                            title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">
                            <%=(MsgHelper.GetMessage("generic Title"))%></a>
                    </td>
                    <td>
                        <a href="tasks.aspx?action=DeleteAllTasks&orderby=id&ty=<%=(actiontype)%>" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>"
                            title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">
                            <%=(MsgHelper.GetMessage("generic ID"))%></a>
                    </td>
                    <%
			    if ((actiontype == "by") | (actiontype == "all") | (actiontype == "both")) {
                    %>
                    <td title="Assigned To">
                        Assigned To
                    </td>
                    <%
			    }
                if ((actiontype == "to") | (actiontype == "all") | (actiontype == "both"))
                {
                    %>
                    <td>
                        <a href="tasks.aspx?action=DeleteAllTasks&orderby=assignedby&ty=<%=(actiontype)%>"
                            alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">
                            Assigned By</a>
                    </td>
                    <%
                    }
                    %>
                    <td>
                        <a href="tasks.aspx?action=DeleteAllTasks&orderby=duedate&ty=<%=(actiontype)%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">
                            Due Date</a>
                    </td>
                    <td>
                        <a href="tasks.aspx?action=DeleteAllTasks&orderby=state&ty=<%=(actiontype)%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">
                            State</a>
                    </td>
                    <td>
                        <a href="tasks.aspx?action=DeleteAllTasks&orderby=priority&ty=<%=(actiontype)%>"
                            title="<%=(MsgHelper.GetMessage("click to sort msg"))%>">Priority</a>
                    </td>
                </tr>
                <asp:Literal ID="ltrDeleteAllTasks" runat="server"></asp:Literal>
            </table>
        </div>
        <asp:Literal ID="ltrTaskIds" runat="server"></asp:Literal>
        <input type="hidden" name="orderby" value="<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["orderby"])%>"
            id="orderby" />
        <input type="hidden" name="ty" value="<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["ty"])%>"
            id="Hidden4" />
        </form>
        <script type="text/javascript">            FillInShowTaskType(); </script>
    </asp:Panel>
    <asp:Panel ID="pnlEditTask" runat="server" Visible="false">
        <asp:Literal ID="ltrEditTaskScript" runat="server"></asp:Literal>
        <form name="taskinfo" id="EditTaskForm" runat="server">
            <div id="FrameContainer" style="position: absolute; top: 68px; left: 55px; width: 1px;
            height: 1px; display: none;">
            <iframe id="ChildPage" name="ChildPage" frameborder="yes" marginheight="2" marginwidth="2"
                width="100%" height="100%" scrolling="auto"></iframe>
        </div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
                <%
                switch (Convert.ToInt64(cTask.TaskTypeID))
                {
                    case (long)Ektron.Cms.Common.EkEnumeration.TaskType.BlogPostComment:
                        Response.Write(m_refStyle.GetTitleBar(MsgHelper.GetMessage("lbl edit postcomment")));
                        break;
                    case (long)Ektron.Cms.Common.EkEnumeration.TaskType.TopicReply:
                        Response.Write(m_refStyle.GetTitleBar(MsgHelper.GetMessage("lbl edit topicreply")));
                        break;
                    default:
                        Response.Write(m_refStyle.GetTitleBar("Edit Task"));
                        break;
                }
                %>
            </div>
            <div class="ektronToolbar">
                <table>
                    <tr>
                        <%
						string BackString  = string.Empty;
						if (!string.IsNullOrEmpty(Request.QueryString["blogid"]))
                        {
						    BackString = "content.aspx?id=" + Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["blogid"]) + "&action=ViewContentByCategory&LangType=" + ContentLanguage + "&ContType=13&contentid=" + Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["contentid"]);
                            Response.Write("<input type=\"hidden\" id=\"blogid\" name=\"blogid\" value=\"" + Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["blogid"]) + "\"/>");
                        }
						else
                        {
						    BackString = "tasks.aspx?action=ViewTask&tid=" + TaskID + "&ty=" + actiontype + callBackPage;
                        }
						Response.Write(m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", BackString, MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "onclick=\"if(IsChildWaiting()) { return false; } else { return true; }\"", StyleHelper.BackButtonCssClass, true)); %>
                        <%=(m_refStyle.GetButtonEventsWCaption(AppUI.AppPath  + "images/UI/Icons/save.png", "#", "Update Task", MsgHelper.GetMessage("btn update"), "onclick=\"if(IsChildWaiting()) return false; else return SubmitForm('EditTask', 'VerifyForm()');\"", StyleHelper.SaveButtonCssClass, true)) %>
                        <td>
                            <%=m_refStyle.GetHelpButton(action, "")%>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="ektronPageContainer">
            <% if (cTask.TaskTypeID == (long)Ektron.Cms.Common.EkEnumeration.TaskType.BlogPostComment | cTask.TaskTypeID == (long)Ektron.Cms.Common.EkEnumeration.TaskType.TopicReply) { %>
            <table height="20" style="background-color: white" width="100%">
                <tr>
                    <td class="tab_actived" id="dvProperties" width="1%" nowrap onclick="ShowPane('dvProperties');return false;">
                        <b>&nbsp;<asp:Literal ID="divPropertyText" runat="server" />&nbsp;</b>
                    </td>
                    <td class="tab_spacer" width="1%" nowrap>
                        &nbsp;</td>
                    <td class="tab_disabled" id="dvMetadata" width="1%" nowrap onclick="ShowPane('dvMetadata');return false;">
                        <b>&nbsp;<asp:Literal ID="divMetadataText" runat="server" />&nbsp;</b>
                    </td>
                    <td class="tab_last" width="91%" nowrap>
                        &nbsp;</td>
                </tr>
            </table>
            <div style="width: 95%; height: 95%" id="_dvProperties">
            <% } %>
            <table class="ektronGrid">
                <asp:Literal ID="ltrNotBlogTopic" runat="server"></asp:Literal>
                <asp:Literal ID="ltrNotBlogTopic2" runat="server"></asp:Literal>
                <tr>
                    <td class="label" title="Content">
                        <%=MsgHelper.GetMessage("content content label") %>
                    </td>
                    <td>
                        <asp:Literal ID="ltrEditTaskBody1" runat="server"></asp:Literal>
                    </td>
                </tr>
                <asp:Literal ID="ltrNotBlogTopic3" runat="server"></asp:Literal>
                <tr>
                    <td class="label" title="State">
                        State:
                    </td>
                    <td>
                        <asp:Literal ID="ltrEditTaskSelect" runat="server"></asp:Literal>
                    </td>
                </tr>
                <asp:Literal ID="ltrNotBlogTopic4" runat="server"></asp:Literal>
                <asp:Literal ID="ltrNotBlogTopic5" runat="server"></asp:Literal>
                <tr>
                        <td class="ektronHeader" colspan="2" title="Description">
                            <%=MsgHelper.GetMessage("description label") %>
                            <asp:PlaceHolder ID="EditTaskValidatorHolder" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:PlaceHolder ID="EditTaskEditorHolder" runat="server" />
                            <!--eWebEditProEditor("description", "595", "200", cTask.Description))-->
                        </td>
                    </tr>
            </table>
            <% if (cTask.TaskTypeID == (long)Ektron.Cms.Common.EkEnumeration.TaskType.BlogPostComment | cTask.TaskTypeID == (long)Ektron.Cms.Common.EkEnumeration.TaskType.TopicReply) { %>
            </div>
            <div style="display: none;" id="_dvMetadata">
                <table>
                    <asp:Literal ID="ltrMetaData" runat="server"></asp:Literal>
                </table>
            </div>
            <% } %>
            <input type="hidden" name="netscape" onkeypress="return CheckKeyValue(event,'34');" id="Hidden8" />
            <input type="hidden" name="hdnstartdate" value="<%=(cTask.StartDate)%>" id="hdnstartdate" />
            <input type="hidden" name="hdnduedate" value="<%=(cTask.DueDate)%>" id="hdnduedate" />
            <input type="hidden" name="content_id" value="<%=(cTask.ContentID)%>" id="content_id" />
            <input type="hidden" name="assigned_to_user_id" value="<%=(cTask.AssignedToUserID)%>" id="assigned_to_user_id" />
            <input type="hidden" name="assigned_to_usergroup_id" value="<%=(cTask.AssignToUserGroupID)%>" id="assigned_to_usergroup_id" />
            <input type="hidden" name="assigned_by_user_id" value="<%=(cTask.AssignedByUserID)%>" id="assigned_by_user_id" />
            <input type="hidden" name="task_id" value="<%=(cTask.TaskID)%>" id="task_id" />
            <input type="hidden" name="stateful" value="<%=(cTask.State)%>" id="stateful" />
            <input type="hidden" name="current_user_id" value="<%=(currentUserID)%>" id="current_user_id" />
            <input type="hidden" name="current_language" value="<%=cTask.ContentLanguage%>" id="current_language" />
        </div>
        </form>
    </asp:Panel>
    <asp:Panel ID="pnlEditTaskType" runat="server" Visible="false">
    <form action="tasks.aspx?Action=EditTaskType&ty=SaveTaskType&is_child=<%=is_child%>&<%=callBackPage%>" name="EditTaskTypeForm" method="Post" ID="EditTaskTypeForm">
	        <div class="ektronPageHeader">
	            <div class="ektronTitlebar">
			        <%= (m_refStyle.GetTitleBar(sTitleBar)) %>
		        </div>
		        <div class="ektronToolbar">
			        <table>
				        <tr>
				        <% if (is_child) { %>
					        <%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "#" , MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "onclick=\"if (window.parent.$ektron) {window.parent.$ektron('#TaskTypeFrameContainer, #TaskTypeOverLay').hide();}\"", StyleHelper.BackButtonCssClass, true))%>
						<% }else{ %>
								<%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "tasks.aspx?action=ViewTaskType" + callBackPage , MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true))%>
						<% } %>
							<%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("title save task type"), MsgHelper.GetMessage("title save task type"), "onclick=\"return SubmitTaskTypeForm('EditTaskTypeForm', 'VerifyEditTaskTypeForm()');\"", StyleHelper.SaveButtonCssClass, true))%>
				            <%=StyleHelper.ActionBarDivider%>
							<td><%=m_refStyle.GetHelpButton(action, "")%></td>
				        </tr>
			        </table>
		        </div>
		    </div>
		    <div class="ektronPageContainer ektronPageInfo">
		        <table class="ektronGrid">
			        <tr>
				        <td class="label" title="Type"><%=MsgHelper.GetMessage("lbl task type")%>:</td>
				        <td class="value"><input title="Enter Type here" id="task_type_title" type="text" name="task_type_title" maxlength="50" value="<%=colTaskType["task_type_title"] %>"/></td>
			        </tr>
			        <tr>
			            <td class="label" title="Description"><%=MsgHelper.GetMessage("lbl task type description")%>:</td>
				        <td class="value"><textarea title="Enter Description here" id="task_type_description" name="task_type_description" maxlength="255" wrap="soft"><%=colTaskType["task_type_description"] %></textarea></td>
			        </tr>
			        <% if (Convert.ToInt32(colTaskType["active"]) <= 1) { %>
			            <tr>
					        <td class="label" title="Not Available"><%=MsgHelper.GetMessage("lbl not available")%>:</td>
					        <asp:Literal ID="ltrEditTaskTypeChk" runat="server"></asp:Literal>
				        </tr>
			        <% } %>
		        </table>
		    </div>

	        <input type="hidden" name="task_type_id" value="<%= (Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["task_type_id"])) %>" ID="task_type_id"/>
	        <input type="hidden" name="category_name" value="<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(colParentCategory["task_type_title"].ToString())%>" ID="category_name"/>
	        <input type="hidden" name="comparetasktypeid" value="<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["task_type_id"])%>" ID="comparetasktypeid"/>
    </asp:Panel>
    <asp:Panel ID="pnlAddTaskType" runat="server" Visible="false">
        <form action="tasks.aspx?action=AddTaskType&ty=SaveTaskType&is_child=<%=is_child%>&<%=callBackPage%>" name="AddTaskTypeForm" method="Post" id="AddTaskTypeForm">
	    <div class="ektronPageHeader">
            <div class="ektronTitlebar">
	            <%= (m_refStyle.GetTitleBar(MsgHelper.GetMessage("title add task type")))%>
            </div>
            <div class="ektronToolbar">
	            <table>
		            <tr>

                    <% if (is_child) { %>
			            <%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "#" , MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "onclick=\"CloseTaskTypePage(false,aAllCategory,-1,-1);\"", StyleHelper.BackButtonCssClass, true))%>
                    <% } else { %>
			            <%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "tasks.aspx?action=ViewTaskType" + callBackPage , MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "onclick=\"return ConfirmBack('AddTaskTypeForm');\"", StyleHelper.BackButtonCssClass, true))%>
		            <% } %>
			            <%=(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#" , MsgHelper.GetMessage("title save task type"), MsgHelper.GetMessage("title save task type"), "onclick=\"return SubmitTaskTypeForm('AddTaskTypeForm', 'VerifyTaskTypeForm()');\"", StyleHelper.SaveButtonCssClass, true))%>
		                <%=StyleHelper.ActionBarDivider%>
						<td><%=m_refStyle.GetHelpButton(action, "")%></td>
		            </tr>
	            </table>
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <table class="ektronGrid">
                <tr>
                    <td class="label" title="Type"><%=MsgHelper.GetMessage("lbl task type")%>:</td>
	                <td class="value"><input title="Enter Type Text here" id="new_task_type_title" type="text" name="new_task_type_title" maxlength="50" value="" /></td>
	            </tr>
	            <tr>
	                <td class="label" title="Description"><%=MsgHelper.GetMessage("lbl task type description")%>:</td>
	                <td class="value"><textarea title="Enter Description here" id="new_task_type_description" name="new_task_type_description" wrap="soft" maxlength="255"></textarea></td>
	            </tr>
	            <tr>
	                <td class="label" title="Not Available"><%=MsgHelper.GetMessage("lbl not available")%>:</td>
	                <td class="value"><input title="Enable/Disable Available" id="new_active" type="checkbox" name="new_active" /></td>
	            </tr>
	        </table>
	        <div class="ektronTopSpaceSmall"></div>
	        <fieldset>
	            <legend title="Task Category">Task Category</legend>
	            <asp:Literal ID="ltrAddTaskType" runat="server"></asp:Literal>
		        <div class="ektronTopSpaceSmall"></div>
		        <input id="new_category" type="text" name="new_category" maxlength="50" value="" onclick="document.getElementById('radio_new_category').checked=true";/>
	        </fieldset>
        </div>
        </form>
    </asp:Panel>
</body>
</html>

<script type="text/javascript">
<!--
CommentPopUpPage="commentpopup.aspx?ref_type=T&id=<%=Server.HtmlEncode(Request["tid"])%>";
CommentSaveType="<%=Server.HtmlEncode(Request.QueryString["action"])%>"
//-->
</script>

