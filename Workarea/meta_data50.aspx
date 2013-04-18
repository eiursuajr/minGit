<%@ Page Language="C#" AutoEventWireup="true" Inherits="meta_data50" CodeFile="meta_data50.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
        <asp:literal id="StyleSheetJS" runat="server" />
		<title></title>
        <style type="text/css">
                    div#pleaseWait
                    {
	                    display: none;
                        width: 128px;
                        height: 128px;
                        margin: -64px 0 0 -64px;
                        background-color: #fff;
                        background-image: url("images/ui/loading_big.gif");
                        background-repeat: no-repeat;
                        text-indent: -10000px;
                        border: none;
                        padding: 0;
                        top: 50%;
                    }
                    textarea {width:90% !important;}
                </style>
        <asp:PlaceHolder ID="phInlineScript" runat="server">
            <script  type="text/javascript">
                    <!--//--><![CDATA[//><!--
                    Ektron.ready( function()
                    {
                        // PLEASE WAIT MODAL
                        $ektron("#pleaseWait").modal({
                            trigger: '',
                            modal: true,
                            toTop: true,
                            onShow: function(hash){
                                hash.o.fadeIn();
                                hash.w.fadeIn();
                            },
                            onHide: function(hash) {
                                hash.w.fadeOut("fast");
                                hash.o.fadeOut("fast", function(){
                                    if (hash.o){
	                                    hash.o.remove();
                                    }
                                });
                            }
                        });
                    });

	                var invalidFormatMsg = "<%=(_MessageHelper.GetMessage("js: invalid date format error msg"))%>";
	                var invalidYearMsg = "<%=(_MessageHelper.GetMessage("js: invalid year error msg"))%>";
	                var invalidMonthMsg = "<%=(_MessageHelper.GetMessage("js: invalid month error msg"))%>";
	                var invalidDayMsg = "<%=(_MessageHelper.GetMessage("js: invalid day error msg"))%>";
	                var invalidTimeMsg = "<%=(_MessageHelper.GetMessage("js: invalid time error msg"))%>";
	                var buttonPressed = false;
	                var ecmMonths = "empty,<%= siteRef.GetClientMonthNames() %>" ;
	                var ecmDays = "<%= siteRef.GetClientDayNames() %>" ;
	                var g_enableDataTypeWarningMsg = false;
	                var g_origDataStyle = "";

	                function VerifyMetaForm () {                      
		                document.metadefinition.frm_metatypename.value = Trim(document.metadefinition.frm_metatypename.value);
		                if (document.metadefinition.frm_metatypename.value == "") {
			                alert("<%= _MessageHelper.GetMessage("js: alert meta def name required").Replace("'","\'") %>");
			                document.metadefinition.frm_metatypename.focus();
			                return false;
		                }

		                var arrMetaName = ["DateCreated","DateModified","GoLiveDate","ExpiryDate","ExpiryType","TaxCategory","ContentID","ContentLanguage","ContentType","FolderId","QuickLink","FolderName","MapLongitude","MapLatitude","MapAddress","EDescription","MetaInfo","CMSPath","CMSSize","InPerm","Searchable", "MapDate"];
		                var found = false;
                        var i = 0;
                        for (i = 0; i < arrMetaName.length; ++i)
                        {
                          if (arrMetaName[i].toLowerCase() == document.metadefinition.frm_metatypename.value.toLowerCase())
                           {
                            found = true;
                            break;
                           }
                        }
                          if ((found))
                           {
                             alert('<%=_MessageHelper.GetMessage("lbl invalid MetaData Name")%>');
                             return false;
                           }


		                 if (!ValidateMetaNameNotUsed()){
			                return false;
		                }
		                for (var lLoop = 0; lLoop < document.metadefinition.frm_metatypename.value.length; lLoop++) {
			                var Char = document.metadefinition.frm_metatypename.value.substring(lLoop, lLoop + 1);
			                if ((Char == "[") || (Char == "]") || (Char == ";") || (Char == ",") || (Char == '"')) {
				                alert("<%= _MessageHelper.GetMessage("js: alert meta def invalid chars").Replace("'","\'") %>");
				                document.metadefinition.frm_metatypename.focus();
				                return false;
			                }
		                }
		                document.metadefinition.frm_metaseparator.value = Trim(document.metadefinition.frm_metaseparator.value);
		                if (document.metadefinition.frm_metaseparator.value == "" && document.getElementById("MetaTagType").value == "1") {
			                alert("<%= _MessageHelper.GetMessage("js: alert meta def separator required") %>");
			                //frm_metaseparator is now a hidden field
			                //document.metadefinition.frm_metaseparator.focus();
			                return false;
		                }
		                document.metadefinition.frm_metadefault.value = Trim(document.metadefinition.frm_metadefault.value);
		                if (document.metadefinition.frm_metadefault.value.length > 2000) {
			                alert("<%= (_MessageHelper.GetMessage("js: alert default text over limit") + " ")  %>" + (document.metadefinition.frm_metadefault.value.length - 2000));
			                return false;
		                }

		                if ((document.forms[0].frm_metanametitle.value=='number')
			                || (document.forms[0].frm_metanametitle.value=='byte')
			                || (document.forms[0].frm_metanametitle.value=='double')
			                || (document.forms[0].frm_metanametitle.value=='float')
			                || (document.forms[0].frm_metanametitle.value=='integer')
			                || (document.forms[0].frm_metanametitle.value=='long')
			                || (document.forms[0].frm_metanametitle.value=='short'))
			                {
			                // Verify that the default value is either blank or has only numeric chars if this is a numeric property
			                if(!numberValidate(document.forms[0].frm_metadefault)) {
				                return false ;
			                }
		                }
		                return true;
	                }

	                function ValidateMetaNameNotUsed()
	                {
		                var namesObj = window.document.getElementById("meta_type_names");
		                if (null == namesObj){
			                // We return true if this hidden field doesn't exist,
			                // as this indicates that we are NOT creating new Meta:
			                return true;
		                }
		                else {
			                var newNameObj = document.getElementById("MetaTypeName");
			                if (null == newNameObj){
				                return false;
			                }
			                else {
				                var newMetaName = newNameObj.value;
				                var metaNames = namesObj.value.split(",");
				                for (var idx=0; idx < metaNames.length; idx++){
					                if (metaNames[idx] == newMetaName){
						                alert("<%= _MessageHelper.GetMessage("js: alert name in use").Replace("'","\'") %>");
						                return false;
					                }
				                }
				                return true;
			                }
		                }
	                }

	                function OnSubmitMetaDefinition(FormName)
	                {
                // TODO auto update default select list or display an alert if disabled
		                var objForm = document.forms[FormName];
		                var objElem = "";
		                var value;
		                var objMetaTagType = document.getElementById("MetaTagType");

		                if (objForm && objMetaTagType != "")
		                {
			                var strSuffix = "_" + objMetaTagType.value;
			                if (<%=MetaTagType_Searchable%> == objMetaTagType.value)
			                {
				                // copy default for specific style (eg, date) to default for 'searchable'
				                objElem = objForm.elements["frm_metanametitle_<%=MetaTagType_Searchable%>"];
				                if (typeof objElem != "undefined")
				                {
					                var strStyle = "_" + objElem.value;
					                objElem = objForm.elements["frm_metadefault" + strSuffix + strStyle];
					                if (typeof objElem != "undefined")
					                {
						                objForm.elements["frm_metadefault" + strSuffix].value = objElem.value;
					                }
				                }
			                }

			                // copy values for specific type (eg, searchable) to generic fields that will be processed
			                var formElementNames = ["frm_metanametitle",
									                "frm_metaremoveduplicates", "frm_metacasesensitive",
									                "frm_metaseparator",
									                "frm_selectable_only", "frm_allow_multi", "frm_MetaSelectableText",
									                "frm_metadefault"];
			                for (var i = 0; i < formElementNames.length; i++)
			                {
				                objElem = objForm.elements[formElementNames[i] + strSuffix];
				                if (typeof objElem != "undefined")
				                {
					                value = GetElementValue(objElem);
					                SetElementValue(objForm.elements[formElementNames[i]], value);
				                }
			                }
		                }
	                }

	                function ConfirmDelete() {
		                if (!confirm("<%= _MessageHelper.GetMessage("js: confirm meta type delete") %>")) {
			                return false;
		                }
		                return confirm("<%= _MessageHelper.GetMessage("js: confirm few moments") %>");
	                }

	                function SubmitForm(FormName, Validate) {
	                    $ektron("#pleaseWait").modalShow();
		                if (!ShowDataTypeChangeWarning(null, true)) {
		                    $ektron("#pleaseWait").modalHide();
			                return false;
		                }
		                OnSubmitMetaDefinition(FormName);
		                if (!ValidateMeta(FormName))
		                {
		                    $ektron("#pleaseWait").modalHide();
			                return false;
		                }
 		                if (Validate.length > 0) {
			                if (eval(Validate)) {
				                document.forms[FormName].submit();
				                return false;
			                }
			                else {
		                        $ektron("#pleaseWait").modalHide();
				                return false;
			                }
		                }
		                else {
			                document.forms[FormName].submit();
			                return false;
		                }
	                }

	                function OnChangeMetaTagType(objSelect)
	                {
	                    var value = objSelect.value;
		                for (var i = 0; i < objSelect.options.length; i++)
		                {
			                ShowElement("idMetaTagType_" + objSelect.options[i].value, (value == objSelect.options[i].value));
		                }
	                }

	                function OnChangeMetaRemoveDuplicates(objCheckBox)
	                {
		                var bEnable = objCheckBox.checked
		                EnableElement("MetaCaseSensitiveLabel_<%=MetaTagType_Meta%>", bEnable);
	                }

	                function OnChangeMetaEditable(objCheckBox)
	                {
		                var bEnable = objCheckBox.checked
		                EnableElement("AllowMultiLabel_<%=MetaTagType_Meta%>", bEnable);
		                EnableElement("MetaSelectableTextLabel_<%=MetaTagType_Meta%>", bEnable);
		                EnableElement("SelectableOnlyLabel_<%=MetaTagType_Meta%>", bEnable);
	                }

	                function OnChangeSelectable(objCheckBox)
	                {
		                var bEnable = objCheckBox.checked
		                EnableElement("AllowMultiLabel_<%=MetaTagType_Meta%>", bEnable);
		                EnableElement("MetaSelectableTextLabel_<%=MetaTagType_Meta%>", bEnable);
	                }

	                function UpdateValidationField(form, type, fieldname)
	                {
		                if(form.needed_validation)
		                {
			                form.needed_validation.value = fieldname + "," + type;
		                }
	                }

	                var g_bSavedListText = false;
	                var g_strSavedListText = "";
	                function OnChangeSearchPropStyle(objSelect)
	                {
		                var value = objSelect.value;
		                for (var i = 0; i < objSelect.options.length; i++)
		                {
			                ShowElement("idSearchPropStyle_" + objSelect.options[i].value, (value == objSelect.options[i].value));
		                }

		                var bBoolean = ("<%=BOOLEAN_PROP%>" == value);
		                var bSelectable = ("<%=SELECT1_PROP%>" == value || "<%=SELECT_PROP%>" == value);

		                // Handle properties that need to define a list
		                ShowElement("idSelectListSeparator", bSelectable);
		                ShowElement("idSelectListLabel", bSelectable);
		                ShowElement("idSelectListText", bSelectable);

		                var objForm = document.forms["metadefinition"];
		                if (objForm)
		                {
			                if(objForm.elements["frm_metanametitle_<%=MetaTagType_Searchable%>"])
			                {
				                var strListStyle = objForm.elements["frm_metanametitle_<%=MetaTagType_Searchable%>"].value;
				                UpdateValidationField(objForm, strListStyle, "frm_metadefault_<%=MetaTagType_Searchable%>_" + strListStyle);
			                }

			                objForm.elements["frm_selectable_only_<%=MetaTagType_Searchable%>"].value = (bSelectable || bBoolean ? "on" : "");
			                objForm.elements["frm_allow_multi_<%=MetaTagType_Searchable%>"].value = ("<%=SELECT_PROP%>" == value ? "on" : "");

			                var objListText = objForm.elements["frm_MetaSelectableText_<%=MetaTagType_Searchable%>"];
			                if (objListText)
			                {
				                if (bBoolean)
				                {
					                // assign Yes/No values
					                var strNo = "<%=_MessageHelper.GetMessage("generic No")%>";
					                var strYes = "<%=_MessageHelper.GetMessage("generic Yes")%>";
					                var strSeparator = objForm.elements["frm_metaseparator_<%=MetaTagType_Searchable%>"].value;
					                if (!g_bSavedListText)
					                {
						                // preserve non-Boolean value
						                g_strSavedListText = objListText.value;
						                g_bSavedListText = true;
					                }
					                objListText.value = strNo + strSeparator + strYes;
				                }
				                else if (g_bSavedListText)
				                {
					                // restore non-Boolean value
					                objListText.value = g_strSavedListText;
					                g_bSavedListText = false;
				                }
			                }
		                }

		                ShowDataTypeChangeWarning(value, false);
	                }

	                // Metadata-Control-Behaviour enumeration:
	                var mcb_LeaveUnlessUserDefaults = 1;
	                var mcb_TestLeaveUnlessUserDefaults = 2;
	                var mcb_ForceUserDefaults = 3;

	                function ShowObject(obj){
		                if (obj && obj.style) {
			                obj.style.display = '';
		                }
	                }

	                function HideObject(obj){
		                if (obj && obj.style) {
			                obj.style.display = 'none';
		                }
	                }

	                //-------------------------------------------------------------
	                //  Data-type source-target matrix:
	                //							-Target Type-
	                //               text   number  date  boolean  select1  select
	                //      text	   -     T-L/D  T-L/D    D         D        D
	                //      number	  L/D      -       D     D         D        D
	                //      date	  L/D      D       -     D         D        D
	                //      boolean	  L/D      D       D     -         D        D
	                //      select1	  L/D      D       D     D         -       L/D
	                //      select	  L/D      D       D     D         D        -
	                //
	                //      Key:
	                //      D = Set to Default value.
	                //      T-L/D = Test type (using SQL); leave value untouched if possible, replace all others with default.
	                //      L/D = Leave data value untouched, unless user desires default value.
	                //-------------------------------------------------------------
	                function DislaySelectMetadataItems(behaviour, containerObj, msgObj, strSrcType, strTargType)
	                 {
	                    var strWarnPre = '';
		                var strWarnPost = '';
		                var strWarnPreWarning = 'Attention: ';
		                var strWarnNotCompatible = 'If stored data is not compatible with new style, that data will be lost.';
		                var strWarnIrreversible = 'It may be impossible to restore the original values after this action is taken!';
		                var strWarnPostLostIrreversible = '\nExisting data will be overwritten with the default value.';
		                var strWarnPostIrreversible = '\n' + strWarnNotCompatible + ' ' + strWarnIrreversible;

		                var tdexistObj = document.getElementById('td_metadata_use_existing_data');
		                var existObj = document.getElementById('metadata_use_existing_data');
		                var tdexistdefObj = document.getElementById('td_metadata_use_existing_data_default');
		                var existdefObj = document.getElementById('metadata_use_existing_data_default');
		                var tddefaultObj = document.getElementById('td_metadata_use_default');
		                var defaultObj = document.getElementById('metadata_use_default');

		                // ensure objects are not null:
		                if ((!tdexistObj) || (!existObj) || (!tdexistdefObj) || (!existdefObj)
			                 || (!tddefaultObj) || (!defaultObj) || (!containerObj) || (!msgObj)){
			                return;
		                }

		                switch (behaviour) {
			                case mcb_LeaveUnlessUserDefaults:
				                // leave data as-is unless user wants default value to be used.
				                //alert('mcb_LeaveUnlessUserDefaults');
				                ShowObject(tdexistObj); // show use-existing-data button.
				                ShowObject(tddefaultObj); // show use-default button.
				                HideObject(tdexistdefObj); // hide the use-existing-if-possible-else-use-default button.
				                existObj.checked = true; // set default to use existing data.
				                strWarnPre = '';
				                strWarnPost = strWarnPostIrreversible;
				                break;

			                case mcb_TestLeaveUnlessUserDefaults:
				                // attempt to leave the data as-is unless it is invalid for
				                // the new type, if invalid or user selects default then use default.
				                //alert('mcb_TestLeaveUnlessUserDefaults');
				                ShowObject(tddefaultObj); // show use-default button.
				                ShowObject(tdexistdefObj); // hide the use-existing-if-possible-else-use-default button.
				                HideObject(tdexistObj); // Hide use-existing-data button.
				                existdefObj.checked = true; // set default to use existing data.
				                strWarnPre = strWarnPreWarning;
				                strWarnPost = strWarnPostLostIrreversible;
				                break;

			                case mcb_ForceUserDefaults:
				                // default value must be used.
				                //alert('mcb_ForceUserDefaults');
				                ShowObject(tddefaultObj); // show use-default button.
				                HideObject(tdexistdefObj); // hide the use-existing-if-possible-else-use-default button.
				                HideObject(tdexistObj); // hide the use-existing-data button.
				                defaultObj.checked = true; // set default to use existing data.
				                strWarnPre = strWarnPreWarning;
				                strWarnPost = strWarnPostLostIrreversible;
				                break;

			                default:
				                break;
		                }

		                //if (strWarnPost.length > 0) {
		                //	strWarnPost = ", " + strWarnPost;
		                //}
		                strMsg = strWarnPre + 'Converting from ' + strSrcType + ' to ' + strTargType + ': ' + strWarnPost;
		                msgObj.innerText = strMsg;
		                ShowObject(containerObj); // show the message.

		                // highlight if warning:
		                if ((strWarnPre.length > 0) || (strWarnPost.length > 0)) {
			                msgobjClassName = 'important';
		                } else {
			                msgobjClassName = '';
		                }
		                if ('undefined' != (typeof(msgObj.className)).toLowerCase()) {
			                msgObj.className = msgobjClassName;
		                }
	                }

	                function ShowDataTypeChangeWarning(selectValue, popupFlag)
	                {
		                // Show proper warning message for selected style (data type):
		                var containerObj = document.getElementById('dataStyleContainer');
		                var msgObj = document.getElementById('dataStyleChangeWarning');
		                var tagtypeObj = document.getElementById('MetaTagType');
		                var strMsg, strSrcType, strTargType, msgobjClassName, behaviour;
		                var hiddenSrcTypeObj = document.getElementById('frm_original_data_style');
		                var hiddenTargTypeObj = document.getElementById('frm_target_data_style');

		                if ((!containerObj) || (!msgObj) || (!tagtypeObj) || (!hiddenSrcTypeObj) || (!hiddenTargTypeObj)) {
			                alert('Error: cannot access form element in ShowDataTypeChangeWarning()!');
			                return true; // error, default to normal operation.
		                }
		                if (tagtypeObj.value != <%=MetaTagType_Searchable%>){
			                return true; // not a searchable type, do not warn user & don't prevent form from submitting.
		                }

		                if (popupFlag){
			                if (g_enableDataTypeWarningMsg && msgObj.innerHTML && (msgObj.innerHTML.length > 0)) {
				                return window.confirm(msgObj.innerHTML); // give user one last chance to avoid data loss.
			                } else {
				                return true; // don't prevent form from submitting.
			                }
		                } else {
			                if ((!g_enableDataTypeWarningMsg) || (selectValue == g_origDataStyle)){
					                containerObj.style.display = 'none';
					                msgObj.innerText = '';
			                } else {
				                switch (selectValue) {
					                case 'text':
						                strTargType = 'text';
						                // When converting to text, allow all types to
						                // either keep existing data or to use default:
						                behaviour = mcb_LeaveUnlessUserDefaults;
						                switch (g_origDataStyle) {
							                case 'text':
								                strSrcType = 'text';
								                break;
							                case 'number':
								                strSrcType = 'number';
								                break;
							                case 'date':
								                strSrcType = 'date';
								                break;
							                case 'boolean':
								                strSrcType = 'boolean';
								                break;
							                case 'select1':
								                strSrcType = 'select1';
								                break;
							                case 'select':
								                strSrcType = 'select';
								                break;
							                default:
								                strSrcType = 'unknown';
								                behaviour = mcb_ForceUserDefaults;
						                }
						                break;
					                case 'number':
						                strTargType = 'number';
						                behaviour = mcb_ForceUserDefaults;
						                switch (g_origDataStyle) {
							                case 'text':
								                strSrcType = 'text';
								                behaviour = mcb_TestLeaveUnlessUserDefaults;
								                break;
							                case 'number':
								                strSrcType = 'number';
								                behaviour = mcb_LeaveUnlessUserDefaults;
								                break;
							                case 'date':
								                strSrcType = 'date';
								                break;
							                case 'boolean':
								                strSrcType = 'boolean';
								                break;
							                case 'select1':
								                strSrcType = 'select1';
								                break;
							                case 'select':
								                strSrcType = 'select';
								                break;
							                default:
								                strSrcType = 'unknown';
						                }
						                break;
					                case 'date':
						                strTargType = 'date';
						                behaviour = mcb_ForceUserDefaults;
						                switch (g_origDataStyle) {
							                case 'text':
								                strSrcType = 'text';
								                behaviour = mcb_TestLeaveUnlessUserDefaults;
								                break;
							                case 'number':
								                strSrcType = 'number';
								                break;
							                case 'date':
								                strSrcType = 'date';
								                behaviour = mcb_LeaveUnlessUserDefaults;
								                break;
							                case 'boolean':
								                strSrcType = 'boolean';
								                break;
							                case 'select1':
								                strSrcType = 'select1';
								                break;
							                case 'select':
								                strSrcType = 'select';
								                break;
							                default:
								                strSrcType = 'unknown';
						                }
						                break;
					                case 'boolean':
						                strTargType = 'boolean';
						                behaviour = mcb_ForceUserDefaults;
						                switch (g_origDataStyle) {
							                case 'text':
								                strSrcType = 'text';
								                break;
							                case 'number':
								                strSrcType = 'number';
								                break;
							                case 'date':
								                strSrcType = 'date';
								                break;
							                case 'boolean':
								                strSrcType = 'boolean';
								                behaviour = mcb_LeaveUnlessUserDefaults;
								                break;
							                case 'select1':
								                strSrcType = 'select1';
								                break;
							                case 'select':
								                strSrcType = 'select';
								                break;
							                default:
								                strSrcType = 'unknown';
						                }
						                break;
					                case 'select1':
						                strTargType = 'select1';
						                behaviour = mcb_ForceUserDefaults;
						                switch (g_origDataStyle) {
							                case 'text':
								                strSrcType = 'text';
								                break;
							                case 'number':
								                strSrcType = 'number';
								                break;
							                case 'date':
								                strSrcType = 'date';
								                break;
							                case 'boolean':
								                strSrcType = 'boolean';
								                break;
							                case 'select1':
								                strSrcType = 'select1';
								                behaviour = mcb_LeaveUnlessUserDefaults;
								                break;
							                case 'select':
								                strSrcType = 'select';
								                break;
							                default:
								                strSrcType = 'unknown';
						                }
						                break;
					                case 'select':
						                strTargType = 'select';
						                behaviour = mcb_ForceUserDefaults;
						                switch (g_origDataStyle) {
							                case 'text':
								                strSrcType = 'text';
								                break;
							                case 'number':
								                strSrcType = 'number';
								                break;
							                case 'date':
								                strSrcType = 'date';
								                break;
							                case 'boolean':
								                strSrcType = 'boolean';
								                break;
							                case 'select1':
								                strSrcType = 'select1';
								                behaviour = mcb_LeaveUnlessUserDefaults;
								                break;
							                case 'select':
								                strSrcType = 'select';
								                behaviour = mcb_LeaveUnlessUserDefaults;
								                break;
							                default:
								                strSrcType = 'unknown';
						                }
						                break;
					                default:
						                strTargType = 'unknown!';
						                behaviour = mcb_ForceUserDefaults;
						                switch (g_origDataStyle) {
							                case 'text':
								                strSrcType = 'text';
								                break;
							                case 'number':
								                strSrcType = 'number';
								                break;
							                case 'date':
								                strSrcType = 'date';
								                break;
							                case 'boolean':
								                strSrcType = 'boolean';
								                break;
							                case 'select1':
								                strSrcType = 'select1';
								                break;
							                case 'select':
								                strSrcType = 'select';
								                break;
							                default:
								                strSrcType = 'unknown';
						                }
				                }
				                DislaySelectMetadataItems(behaviour, containerObj, msgObj, strSrcType, strTargType);
				                // save source and target data types for action page-code:
				                hiddenSrcTypeObj.value = strSrcType;
				                hiddenTargTypeObj.value = strTargType;
			                }
		                }
	                }

	                var g_strSelectListText = "";
	                function OnChangeSelectList(objText)
	                {
		                var objForm = document.forms["metadefinition"];
		                if (objForm && objText && g_strSelectListText != objText.value)
		                {
			                g_strSelectListText = objText.value;

			                EnableElement("frm_metadefaultLabel_<%=MetaTagType_Searchable%>", false);

			                var strListStyle = objForm.elements["frm_metanametitle_<%=MetaTagType_Searchable%>"].value;
			                //var objButton = objForm.elements["frm_metadefault_<%=MetaTagType_Searchable%>_" + strListStyle + "_update"];
			                var objButton = document.getElementById("frm_metadefault_<%=MetaTagType_Searchable%>_" + strListStyle + "_update");

			                if (objButton && objButton.disabled != "undefined")
			                {
				                objButton.disabled = false;
			                }
		                }
	                }

	                function UpdateSelectList(objButton, strSelectName)
	                {
		                if (objButton && objButton.disabled != "undefined")
		                {
			                objButton.disabled = true;
		                }

		                var objForm = document.forms["metadefinition"];
		                if (objForm)
		                {
			                var strSeparator = objForm.elements["frm_metaseparator_<%=MetaTagType_Searchable%>"].value;
			                var strListText = objForm.elements["frm_MetaSelectableText_<%=MetaTagType_Searchable%>"].value;
			                var objSelect = objForm.elements[strSelectName];

			                if (objSelect)
			                {
				                // remember the old value so it can be reselected if it still exists
				                var strOldValue = objSelect.value;
				                var nOldIndex = objSelect.selectedIndex;
				                var aryItems = strListText.split(strSeparator);
				                var iOption = 0;
				                var bAddNoSelection = false;
				                if (objSelect.options.length > 0)
				                {
					                if ("" == objSelect.options[0].value)
					                {
						                bAddNoSelection = true;
					                }
				                }
				                // remove items in list
				                objSelect.options.length = 0;
				                // update options
				                if (bAddNoSelection)
				                {
					                objSelect.options[iOption++] = new Option("(No Selection)", "", false, false);
				                }
				                var value;
				                var objOption;
				                var bSelected = false;
				                for (var i = 0; i < aryItems.length; i++)
				                {
					                value = Trim(aryItems[i]);
					                if (value != "")
					                {
						                if (nOldIndex >= 0)
						                {
							                bSelected = (strOldValue == value);
						                }
						                objSelect.options[iOption++] = new Option(value, value, false, bSelected);
					                }
				                }
			                }
		                }

		                EnableElement("frm_metadefaultLabel_<%=MetaTagType_Searchable%>", true);
	                }

	                function UpdateStatus()
	                {
		                var objElem
		                objElem = document.getElementById("MetaTagType");
		                if ("undefined" != (typeof(objElem)) && objElem != null && objElem != "")
		                {
			                OnChangeMetaTagType(objElem);
		                }
		                objElem = document.getElementById("MetaEditable");
		                if ("undefined" != (typeof(objElem)) && objElem != null && objElem != "")
		                {
			                OnChangeMetaEditable(objElem);
		                }
		                objElem = document.getElementById("SelectableOnly_<%=MetaTagType_Meta%>");
		                if ("undefined" != (typeof(objElem)) && objElem != null && objElem != "")
		                {
			                OnChangeSelectable(objElem);
		                }
		                objElem = document.getElementById("MetaRemoveDuplicates_<%=MetaTagType_Meta%>");
		                if ("undefined" != (typeof(objElem)) && objElem != null && objElem != "")
		                {
			                OnChangeMetaRemoveDuplicates(objElem);
		                }
		                objElem = document.getElementById("MetaNameTitle_<%=MetaTagType_Searchable%>");
		                if ("undefined" != (typeof(objElem)) && objElem != null && objElem != "")
		                {
			                OnChangeSearchPropStyle(objElem);
		                }
	                }
	                //--><!]]>
                </script>
        </asp:PlaceHolder>
	</head>
	<body>
	<asp:Literal id="DebugErrLit" runat="server"></asp:Literal>
	<asp:Panel ID="pnlViewAllDefs" runat="server" >
	        <div class="ektronPageHeader">
	        <div class="ektronTitlebar" title="View Metadata Definitions">
		        <%=_StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view meta definitions msg"))%>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <asp:Literal ID="ltrToolbar" runat="server" ></asp:Literal>
				    </tr>
			    </table>
		    </div>
		</div>
		<div class="ektronPageContainer ektronPageGrid">
		    <table width="100%" class="ektronGrid">
			            <tr class="title-header">
				            <td width="20%" title="Name"><%=_MessageHelper.GetMessage("generic Name")%></td>
				            <td width="1%" align="center" title="ID"><%=_MessageHelper.GetMessage("generic ID")%></td>
				            <td title="Default Text"><%=_MessageHelper.GetMessage("generic Default Text")%></td>
			            </tr>
			            <asp:Literal ID="ltrList" runat="server" ></asp:Literal>
		    </table>
		</div>
			<script type="text/javascript">
		    <!--//--><![CDATA[//><!--
			function LoadLanguage(FormName){
				var num=document.forms[FormName].selLang.selectedIndex;
				document.forms[FormName].action="meta_data50.aspx?action=<%=Request.QueryString["action"]%>"+"&LangType="+document.forms[FormName].selLang.options[num].value;
				document.forms[FormName].submit();
				return false;
			}
			//--><!]]>
		</script>
	</asp:Panel>
	<asp:Panel ID="pnlAddEditViewDef" Visible="false"  runat="server" >
	<% if(!bNew){ %>
	    <script type="text/javascript">
	          g_enableDataTypeWarningMsg = true;
              g_origDataStyle = "<%=cMetaType["MetaNameTitle"].ToString().ToLower()%>";
	    </script>
	    <% } %>
	   <script type="text/javascript">
	   		$ektron(document).ready(function(){
	   			var firstListIdentifier = $ektron("#idSearchPropStyle_select .readOnlyValue #dvMetadata table tbody tr > td:first-child");
	   			var lastListIdentifier = $ektron("#idSearchPropStyle_select .readOnlyValue #dvMetadata table tbody tr > td:last-child");
	   			var moveMetaListIdentifier = $ektron("#idSearchPropStyle_select .readOnlyValue #dvMetadata table tbody tr > td.moveMeta a");
	   			firstListIdentifier.css( {width: '40%'} );
	   			lastListIdentifier.css( {width: '40%'} );
	   			moveMetaListIdentifier.css({
	   				marginLeft: 'auto',
	   				marginRight: 'auto'
	   			});
	   		});
	   </script>
	   <form action="meta_data50.aspx?action=<%=strAction%>&LangType=<%=ContentLanguage%>" name="metadefinition" method="post">
       <div class="ektronWindow" id="pleaseWait"><h3><%=_MessageHelper.GetMessage("one moment msg")%></h3></div>

		<input type="hidden" name="frm_metatypeid" value="<%=id%>" />
		<div class="ektronPageHeader">
		    <div class="ektronTitlebar">
			    <asp:Literal ID="ltrTitle" runat="server" ></asp:Literal>
		    </div>
		    <div class="ektronToolbar">
			   <asp:Literal ID="ltrToolbar2" runat="server" ></asp:Literal>
		    </div>
		</div>
		
		<div class="ektronPageContainer ektronPageInfo">
		   <div id="viewGrid" runat="server" >
		   
		   </div>
		</div>
		   

		<input type="hidden" name="frm_metanametitle" value="<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(strMetaNameTitle)%>" />
		<input type="hidden" name="frm_metaremoveduplicates" value="<%=bMetaRemoveDuplicates%>" />
		<input type="hidden" name="frm_metacasesensitive" value="<%=bMetaCaseSensitive%>" />
		<input type="hidden" name="frm_metaseparator" value="<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(strMetaSeparator)%>" />
		<input type="hidden" name="frm_selectable_only" value="<%=bSelectableOnly%>" />
		<input type="hidden" name="frm_allow_multi" value="<%=bAllowMulti%>" />
		<input type="hidden" name="frm_MetaSelectableText" value="<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(strMetaSelectableText)%>" />
		<input type="hidden" name="frm_metadefault" value="<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(strMetaDefault)%>" />
		<input type="hidden" id="frm_original_data_style" name="frm_original_data_style" value="" />
		<input type="hidden" id="frm_target_data_style" name="frm_target_data_style" value="" />
  	</form>
</asp:Panel>
	
	
	<%	if (bView == false && action.ToLower() != "viewallmetadefinitions")
    { %>
	    <script type="text/javascript">
	    <!--//--><![CDATA[//><!--
		    // Ensure page has had time to load:
		    function callUpdateStatus()
		    {
			    if ("undefined" != (typeof(UpdateStatus)).toLowerCase()){
				    UpdateStatus();
			    }else{
				    setTimeout(callUpdateStatus, 500);
			    }
		    }
		    callUpdateStatus();
	    //--><!]]>
	    </script>
	<% }%>
</body>
</html>

