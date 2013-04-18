<%@ Page Language="C#" AutoEventWireup="true" Inherits="Collections" CodeFile="Collections.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head id="Head1" runat="server">
    <script type="text/javascript">
    <!--        //--><![CDATA[//><!--
        //hide drag drop uploader frame/////
        if (typeof top.HideDragDropWindow != "undefined")
        {
            top.HideDragDropWindow();
        }
        ////////////////////////////////////
        /***********************************************
        * Contractible Headers script- Dynamic Drive (www.dynamicdrive.com)
        * This notice must stay intact for legal use. Last updated Oct 21st, 2003.
        * Visit http://www.dynamicdrive.com/ for full source code
        ***********************************************/

        var enablepersist = "off" //Enable saving state of content structure using session cookies? (on/off)
        var collapseprevious = "no" //Collapse previously open content when opening present? (yes/no)

        if (document.getElementById)
        {
            document.write('<style type="text/css">')
            document.write('.switchcontent{display:none;}')
            document.write('</style>')
        }

        function getElementbyClass(classname)
        {
            ccollect = new Array()
            var inc = 0
            var alltags = document.all ? document.all : document.getElementsByTagName("*")
            for (i = 0; i < alltags.length; i++)
            {
                if (alltags[i].className == classname)
                    ccollect[inc++] = alltags[i]
            }
        }

        function contractcontent(omit)
        {
            var inc = 0
            while (ccollect[inc])
            {
                if (ccollect[inc].id != omit)
                    ccollect[inc].style.display = "none"
                inc++
            }
        }

        function expandcontent(cid)
        {
            if (typeof ccollect != "undefined")
            {
                if (collapseprevious == "yes")
                    contractcontent(cid)
                document.getElementById(cid).style.display = (document.getElementById(cid).style.display != "block") ? "block" : "none"
            }
        }

        function revivecontent()
        {
            contractcontent("omitnothing")
            selectedItem = getselectedItem()
            selectedComponents = selectedItem.split("|")
            for (i = 0; i < selectedComponents.length - 1; i++)
                document.getElementById(selectedComponents[i]).style.display = "block"
        }

        function get_cookie(Name)
        {
            var search = Name + "="
            var returnvalue = "";
            if (document.cookie.length > 0)
            {
                offset = document.cookie.indexOf(search)
                if (offset != -1)
                {
                    offset += search.length
                    end = document.cookie.indexOf(";", offset);
                    if (end == -1) end = document.cookie.length;
                    returnvalue = unescape(document.cookie.substring(offset, end))
                }
            }
            return returnvalue;
        }

        function getselectedItem()
        {
            if (get_cookie(window.location.pathname) != "")
            {
                selectedItem = get_cookie(window.location.pathname)
                return selectedItem
            }
            else
                return ""
        }

        function saveswitchstate()
        {
            var inc = 0, selectedItem = ""
            while (ccollect[inc])
            {
                if (ccollect[inc].style.display == "block")
                    selectedItem += ccollect[inc].id + "|"
                inc++
            }

            document.cookie = window.location.pathname + "=" + selectedItem
        }

        function do_onload()
        {
            getElementbyClass("switchcontent");
            if (enablepersist == "on" && typeof ccollect != "undefined")
            {
                revivecontent();
            }
            metaselect_initialize();
            ta_populateSelectedList();
        }

        if (enablepersist == "on" && document.getElementById)
            window.onunload = saveswitchstate
        //--><!]]>


        function LoadLanguage(inVal, Path)
        {
            if (inVal == '0') { return false; }
            top.notifyLanguageSwitch(inVal, -1);
            document.location = decodeURIComponent(escape(Path + '&LangType=' + inVal));
        }

        function ta_editSelectList()
        {
            var listObj = document.getElementById("template_list");
            var currentOption;
            var textControl = document.getElementById("template_text");
            if (listObj && textControl)
            {
                for (index = 0; index < listObj.length; index++)
                {
                    currentOption = listObj.options[index];
                    if (currentOption.selected == true)
                    {
                        textControl.value = listObj.options[index].text;
                        break;
                    }
                }
                ta_updateSelectedValues();
            }
        }

        function VerifyLibraryAssest()
        {
            if (typeof document.forms.AddMenuItem.title == "object")
            {
                document.forms.AddMenuItem.title.value = Trim(document.forms.AddMenuItem.title.value);
                if (document.forms.AddMenuItem.title.value == "")
                {
                    alert("Title is required");
                    return false;
                }
            }
            if (typeof document.forms.AddMenuItem.id == "object")
            {
                document.forms.AddMenuItem.id.value = Trim(document.forms.AddMenuItem.id.value);
                if (document.forms.AddMenuItem.id.value == "")
                {
                    alert("Invalid Library Item.");
                    return false;
                }
            }
            return true;
        }
        function selectLibraryItem(libraryid, folder, title, filename, type, returnField)
        {
            var ObjField = eval(returnField);
            var site_Path = "<asp:literal id="jsSitePath" runat="server"/>";
            if (ObjField != null)
            {
                if (site_Path == "/")
                {
                    if (filename.indexOf("/") == 0)
                        ObjField.value = filename.substring(1);
                    else
                        ObjField.value = filename;
                }
                else if (filename.indexOf("http://") == 0)
                    ObjField.value = filename;
                else
                    ObjField.value = filename.replace(site_Path,'');

            }
             else
            {
                document.forms[0].title.value = title;
                document.forms[0].DefaultTitle.value = title;
                document.forms[0].id.value = libraryid;
            }
        }
        function PopBrowseWin(Scope, FolderPath, retField, qdonly)
        {
            var Url;
            if (FolderPath == "")
            {
                Url = 'browselibrary.aspx?actiontype=add&scope=' + Scope;
            } else
            {
                Url = 'browselibrary.aspx?actiontype=add&scope=' + Scope + '&autonav=' + FolderPath;
            }
            if (retField != null)
            {
                Url = Url + '&RetField=' + retField;
            }
            if ((qdonly != undefined) && (qdonly != null) && (qdonly != ''))
            {
                Url = Url + '&qdo=1';
            }
            PopUpWindow(Url, 'BrowseLibrary', 790, 580, 1, 1);
        }
        function ta_moveItemUp()
        {
            var currentOption;
            var prevOption;
            var prevOptionValue;
            var prevOptionText;
            var prevOptionSelectState;
            var wasPrevIterSelected = false;
            var wasPrevIterMoved = false;
            var listObj = document.getElementById("template_list");

            if (listObj)
            {
                for (index = 0; index < listObj.length; index++)
                {
                    currentOption = listObj.options[index];
                    if (currentOption.selected == true)
                    {
                        if (index == 0)
                        {
                            wasPrevIterMoved = false;
                        }
                        else
                        {
                            if (wasPrevIterSelected)
                            {
                                if (wasPrevIterMoved)
                                {
                                    prevOption = listObj.options[index - 1];
                                    prevOptionValue = prevOption.value;
                                    prevOptionText = prevOption.text;
                                    prevOptionSelectState = prevOption.selected;
                                    prevOption.value = currentOption.value;
                                    prevOption.text = currentOption.text;
                                    prevOption.selected = true;
                                    currentOption.value = prevOptionValue;
                                    currentOption.text = prevOptionText;
                                    currentOption.selected = prevOptionSelectState;
                                    wasPrevInterMoved = true;
                                }
                                else
                                {
                                    wasPrevIterMoved = false;
                                }
                            }
                            else
                            {
                                prevOption = listObj.options[index - 1];
                                prevOptionValue = prevOption.value;
                                prevOptionText = prevOption.text;
                                prevOptionSelectState = prevOption.selected;
                                prevOption.value = currentOption.value;
                                prevOption.text = currentOption.text;
                                prevOption.selected = true;
                                currentOption.value = prevOptionValue;
                                currentOption.text = prevOptionText;
                                currentOption.selected = prevOptionSelectState;
                                wasPrevIterMoved = true;
                            }
                        }
                        wasPrevIterSelected = true;
                    }
                    else
                    {
                        wasPrevIterSelected = false;
                        wasPrevIterMoved = false;
                    }
                }
                ta_updateSelectedValues();
            }
        }

        function ta_moveItemDown()
        {
            var currentOption;
            var nextOption;
            var nextOptionValue;
            var nextOptionText;
            var nextOptionSelectState;
            var wasPrevIterSelected = false;
            var wasPrevIterMoved = false;

            var listObj = document.getElementById("template_list");
            if (listObj)
            {
                for (index = listObj.length - 1; index >= 0; index--)
                {
                    currentOption = listObj.options[index];
                    if (currentOption.selected == true)
                    {
                        if (index == listObj.length - 1)
                        {
                            wasPrevIterMoved = false;
                        }
                        else
                        {
                            if (wasPrevIterSelected)
                            {
                                if (wasPrevIterMoved)
                                {
                                    nextOption = listObj.options[index + 1];
                                    nextOptionValue = nextOption.value;
                                    nextOptionText = nextOption.text;
                                    nextOptionSelectState = nextOption.selected;
                                    nextOption.value = currentOption.value;
                                    nextOption.text = currentOption.text;
                                    nextOption.selected = true;
                                    currentOption.value = nextOptionValue;
                                    currentOption.text = nextOptionText;
                                    currentOption.selected = nextOptionSelectState;
                                    wasPrevIterMoved = true;
                                }
                                else
                                {
                                    wasPrevIterMoved = false;
                                }
                            }
                            else
                            {
                                nextOption = listObj.options[index + 1];
                                nextOptionValue = nextOption.value;
                                nextOptionText = nextOption.text;
                                nextOptionSelectState = nextOption.selected;
                                nextOption.value = currentOption.value;
                                nextOption.text = currentOption.text;
                                nextOption.selected = true;
                                currentOption.value = nextOptionValue;
                                currentOption.text = nextOptionText;
                                currentOption.selected = nextOptionSelectState;
                                wasPrevIterMoved = true;
                            }
                        }
                        wasPrevIterSelected = true;
                    }
                    else
                    {
                        wasPrevIterSelected = false;
                        wasPrevIterMoved = false;
                    }
                }
                ta_updateSelectedValues();
            }
        }

        function ta_addItemToSelectList()
        {
            var listObj = document.getElementById("template_list");

            var noTitleMsg = "<asp:literal id='ltr_templatenoTitleMsg' runat='server' />";

            if (document.getElementById("template_text").value.length == 0)
            {
                alert(noTitleMsg);
                return false;
            }

            if (listObj)
            {
                var currentOption;
                var index = listObj.length;
                for (index = 0; index < listObj.length; index++)
                {
                    currentOption = listObj.options[index];
                    if (document.getElementById("template_text").value == currentOption.value)
                    {
                        alert("duplicate Text/Value supplied"); return false;
                    }
                }
                index = listObj.length;
                listObj.length = index + 1;
                listObj.options[index].value = document.getElementById("template_text").value;
                listObj.options[index].text = document.getElementById("template_text").value;
                document.getElementById("template_text").value = "";
                ta_updateSelectedValues();
            }
        }

        function ta_updateItemToSelectList()
        {
            var currentOption;
            var duplicateIndex = -1;
            var listObj = document.getElementById("template_list");
            if (listObj)
            {
                for (index = 0; index < listObj.length; index++)
                {
                    currentOption = listObj.options[index];
                    if (document.getElementById("template_text").value == currentOption.value && !currentOption.selected)
                    {
                        duplicateIndex = index;
                    }
                }
                if (duplicateIndex > -1) { alert("text already exists"); return false; }
                for (index = 0; index < listObj.length; index++)
                {
                    currentOption = listObj.options[index];

                    if (currentOption.selected == true)
                    {
                        listObj.options[index].text = document.getElementById("template_text").value;
                        listObj.options[index].value = document.getElementById("template_text").value;
                        document.getElementById("template_text").value = "";
                        break;
                    }
                }
                ta_updateSelectedValues();
            }
        }

        function ta_removeItemsFromSelectList()
        {
            var currentOption;
            var listObj = document.getElementById("template_list");
            if (listObj)
            {
                for (index = 0; index < listObj.length; index++)
                {
                    currentOption = listObj.options[index];
                    if (currentOption.selected == true)
                    {
                        listObj.options[index] = null;
                    }
                }
                document.getElementById("template_text").value = "";
                ta_updateSelectedValues();
            }
        }

        function ta_updateSelectedValues()
        {
            var selectedValues = "";
            var listObj = document.getElementById("template_list");
            if (listObj)
            {
                for (index = 0; index < listObj.length; index++)
                {
                    if (selectedValues == "")
                    {
                        selectedValues = listObj.options[index].value;
                    } else
                    {
                        selectedValues = selectedValues + ";" + listObj.options[index].value;
                    }
                }
                document.getElementById("associated_templates").value = selectedValues;
            }
        }

        function VerifyAddMenuItem()
        {
            if (typeof document.forms.AddMenuItem.Title == "object")
            {
                document.forms.AddMenuItem.Title.value = Trim(document.forms.AddMenuItem.Title.value);
                if (document.forms.AddMenuItem.Title.value == "")
                {
                    alert("Title is Required");
                    return false;
                }
            }
            if (typeof document.forms.AddMenuItem.Link == "object")
            {
                document.forms.AddMenuItem.Link.value = Trim(document.forms.AddMenuItem.Link.value);
                if (document.forms.AddMenuItem.Link.value == "")
                {
                    alert("Item Link is required ");
                    return false;
                }
            }
            if (typeof document.forms.AddMenuItem.frm_menu_image_override == "object")
            {
                if (document.forms.AddMenuItem.frm_menu_image_override.checked)
                {
                    if (document.forms.AddMenuItem.frm_menu_image.value == "")
                    {
                        alert("Image path is required if the checkbox \"Use Image Instead Of Title\" is checked.");
                        return false;
                    }
                }
            }
            if (typeof document.forms.AddMenuItem.Description == "object")
            {
                if (document.forms.AddMenuItem.Description.value.length > 254)
                {
                    alert(MsgHelper.GetMessage("alt The menu description should be less than 255 charecters."));
                    document.forms.AddMenuItem.Description.focus();
                    return false;
                }
            }
            return true;
        }

        function ConfirmNavDelete()
        {
            return confirm("<asp:literal id="jsDeleteConfirm" runat="server"/>");
        }
        function ConfirmMenuDelete()
        {
            return confirm("Are you sure you want to delete this menu?");
        }
        function UpdateView()
        {
            var objSelSupertype = document.getElementById('selAssetSupertype');
            if (objSelSupertype != null)
            {
                var ContType = objSelSupertype.value;
                if (replaceQueryString != "") { replaceQueryString = replaceQueryString + "&" }
                document.location.href = location.pathname + "?" + replaceQueryString + "ContType=" + ContType;
            }

        }
        function VerifyMenuForm()
        {

            var titleAlertMsg="<asp:Literal runat='server' id='ltr_titleAlertMsg' />";
            var linkAlertMsg="<asp:Literal runat='server' id='ltr_linkAlertMsg' />";
            var imagepathAlertMsg="<asp:Literal runat='server' id='ltr_imagepathAlertMsg' />";

            document.forms.menu.frm_menu_title.value = Trim(document.forms.menu.frm_menu_title.value);

            document.forms.menu.frm_menu_description.value = Trim(document.forms.menu.frm_menu_description.value);
            if (document.forms.menu.frm_menu_description.value.length > 254)
            {
                alert("The menu description should be less than 255 charecters.");
                document.forms.menu.frm_menu_description.focus();
                return false;
            }
            if (document.forms.menu.frm_menu_title.value == "")
            {
                alert(titleAlertMsg);
                document.forms.menu.frm_menu_title.focus();
                return false;
            }

            var ckbImage = document.getElementById("frm_menu_image_override");

            if (("object" == typeof (ckbImage)) && (ckbImage != null))
            {
                if (true == document.forms.menu.frm_menu_image_override.checked)
                {
                    var strTemp
                    strTemp = Trim(document.forms.menu.frm_menu_image.value)
                    if (strTemp.length <= 0)
                    {
                        alert(imagepathAlertMsg);
                        document.forms.menu.frm_menu_image.focus();
                        return false;
                    }
                }
            }

            var obj_template = document.getElementById("frm_menu_template");
            if (("object" == typeof (obj_template)) && (obj_template != null))
            {
                var obj_template_set = document.getElementById("frm_set_to_template");

                if (("object" == typeof (obj_template_set)) && (obj_template_set != null))
                {
                    if ((obj_template_set.value == "") && (Trim(obj_template.value) != ""))
                    {
                        var blnAns;
                        blnAns = confirm('Do you want to apply the specified template to all content items already on this menu? If so, click "OK". \n\n If you click "Cancel", the template will only be applied as new content items are added to the menu. Note that you can use the Edit Menu Item screen to manually apply this template to any content items already on the menu. \n\n Note:  The specified template will only be applied this menu, not its sub-menus.');
                        if (true == blnAns)
                        {
                            obj_template_set.value = "true";
                        }
                    }
                }
            }

            return true;
        }

        function selectClearAll(obj)
        {
            if (obj.checked)
            {
                SelectAll();
                return false;
            }
            else
            {
                ClearAll();
                return false;
            }
        }
        
        function SelectAll()
        {
            var lLoop = 0;
            for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++)
            {
                if ((document.forms.selections[lLoop].type.toLowerCase() == "checkbox")
					&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_check") != -1))
                {
                    document.forms.selections[lLoop].checked = true;
                }
            }
            var cArray = Collections.split(",");
            for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++)
            {
                if ((document.forms.selections[lLoop].type.toLowerCase() == "hidden")
							&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_hidden") != -1))
                {
                    var cIndex = document.forms.selections[lLoop].name.toLowerCase().replace("frm_hidden", "");
                    document.forms.selections[lLoop].value = cArray[cIndex];
                }
            }
        }

        function ClearAll()
        {
            var lLoop = 0;
            for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++)
            {
                if ((document.forms.selections[lLoop].type.toLowerCase() == "checkbox")
					&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_check") != -1))
                {
                    document.forms.selections[lLoop].checked = false;
                }
            }
            for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++)
            {
                if ((document.forms.selections[lLoop].type.toLowerCase() == "hidden")
							&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_hidden") != -1))
                {
                    document.forms.selections[lLoop].value = 0;
                }
            }
        }

        function Move(sDir, objList, objOrder)
        {
            if (objList.selectedIndex != null && objList.selectedIndex >= 0)
            {
                nSelIndex = objList.selectedIndex;
                sSelValue = objList[nSelIndex].value;
                sSelText = objList[nSelIndex].text;
                objList[nSelIndex].selected = false;
                if (sDir == "up" && nSelIndex > 0)
                {
                    sSwitchValue = objList[nSelIndex - 1].value;
                    sSwitchText = objList[nSelIndex - 1].text;
                    objList[nSelIndex].value = sSwitchValue;
                    objList[nSelIndex].text = sSwitchText;
                    objList[nSelIndex - 1].value = sSelValue;
                    objList[nSelIndex - 1].text = sSelText;
                    objList[nSelIndex - 1].selected = true;
                }
                else if (sDir == "dn" && nSelIndex < (objList.length - 1))
                {
                    sSwitchValue = objList[nSelIndex + 1].value;
                    sSwitchText = objList[nSelIndex + 1].text;
                    objList[nSelIndex].value = sSwitchValue;
                    objList[nSelIndex].text = sSwitchText;
                    objList[nSelIndex + 1].value = sSelValue;
                    objList[nSelIndex + 1].text = sSelText;
                    objList[nSelIndex + 1].selected = true;
                }
            }
            objOrder.value = "";
            for (i = 0; i < objList.length; i++)
            {
                objOrder.value = objOrder.value + objList[i].value;
                if (i < (objList.length - 1))
                {
                    objOrder.value = objOrder.value + ",";
                }
            }
        }
        
        function SelLanguage(inVal, path)
        {
            top.notifyLanguageSwitch(inVal, -1);
            document.location = decodeURIComponent(escape(path + '&LangType=' + inVal));
        }
        
        function VerifyCollectionForm()
        {
            return true;
        }
        
        function ta_populateSelectedList()
        {
            var listObj = document.getElementById("template_list");
            var values = "";
            if (document.getElementById("associated_templates") != null)
            {
                values = document.getElementById("associated_templates").value;
            }

            if (listObj && (listObj.length == 0) && (values != ""))
            {
                var availableList = values.split(";");
                var index = listObj.length;
                for (index1 = 0; index1 < availableList.length; index1++)
                {
                    listObj.length = index + 1;
                    listObj.options[index].value = availableList[index1];
                    listObj.options[index].text = availableList[index1];
                    index += 1;
                }
            }
        }
        
        function GetIDs()
        {
            var lLoop = 0;
            var llang = "";
            document.forms[0].frm_content_ids.value = "";
            document.forms[0].frm_content_languages.value = "";
            document.forms[0].frm_folder_ids.value = "";
            for (lLoop = 0; lLoop < document.forms[0].length; lLoop++)
            {
                if ((document.forms[0][lLoop].type.toLowerCase() == "hidden")
					&& (document.forms[0][lLoop].name.toLowerCase().search("frm_hidden") != -1)
					&& (document.forms[0][lLoop].value != 0))
                {
                    llang = document.forms[0][lLoop].name;
                    llang = llang.replace("frm_hidden", "");
                    document.forms[0].frm_content_ids.value = document.forms[0].frm_content_ids.value + "," + document.forms[0][lLoop].value;
                    document.forms[0].frm_content_languages.value = document.forms[0].frm_content_languages.value + "," + document.forms[0]["frm_languages" + llang].value;
                    document.forms[0].frm_folder_ids.value = document.forms[0].frm_folder_ids.value + "," + document.forms[0][lLoop].fid;
                }
            }

            document.forms[0].frm_content_ids.value = document.forms[0].frm_content_ids.value.substring(1, document.forms[0].frm_content_ids.value.length);
            document.forms[0].frm_content_languages.value = document.forms[0].frm_content_languages.value.substring(1, document.forms[0].frm_content_languages.value.length);
            document.forms[0].frm_folder_ids.value = document.forms[0].frm_folder_ids.value.substring(1, document.forms[0].frm_folder_ids.value.length);
            if (document.forms[0].frm_content_ids.value.length == 0)
            {
                return false;
            }
            return true;
        }
        
        function CheckKeyValue(item, keys)
        {
            var keyArray = keys.split(",");
            for (var i = 0; i < keyArray.length; i++)
            {
                if ((document.layers) || ((!document.all) && (document.getElementById)))
                {
                    if (item.which == keyArray[i])
                    {
                        return false;
                    }
                }
                else
                {
                    if (event.keyCode == keyArray[i])
                    {
                        return false;
                    }
                }
            }
        }
        
        function CheckMenuLinkProtocol(elemObj) {
            if (elemObj == null || elemObj.value == null || elemObj.value == "")
                return;

            var str = elemObj.value;
            if (StartsWith(str, "http://") || StartsWith(str, "https://") || StartsWith(str, "ftp://")) {
                $ektron(".menuLinkSitePathPrefix").hide();
            } else {
                $ektron(".menuLinkSitePathPrefix").show();
            }
        }

        function StartsWith(str, val) {
            return (str.indexOf(val) == 0);
        }

        function SubmitForm(FormName, Validate)
        {
            if (Validate.length > 0)
            {
                if (eval(Validate))
                {
                    document.forms[FormName].submit();
                    return false;
                }
                else
                {
                    alert("<asp:literal id="jsNoItemSelectedToDelete" runat="server"/>");
                    return false;
                }
            }
            else
            {
                document.forms[FormName].submit();
                return false;
            }
        }

        function SubmitFormSelections(Validate)
        {
            if (Validate.length > 0)
            {
                if (eval(Validate))
                {
                    document.forms[0].submit();
                    return false;
                }
                else
                {
                    alert("<asp:literal id="jsNoItemSelected" runat="server"/>");
                    return false;
                }
            }
            else
            {
                document.forms[0].submit();
                return false;
            }
        }
        
        function GetCellObject(MyObj)
        {
            var tmpName = "";

            tmpName = MyObj.id;
            if (tmpName.indexOf("link_") >= 0)
            {
                tmpName = tmpName.replace("link_", "cell_");
            }
            else if (tmpName.indexOf("cell_") >= 0)
            {
                tmpName = tmpName;
            }
            else
            {
                tmpName = tmpName.replace("image_", "image_cell_");
            }
            MyObj = document.getElementById(tmpName);
            return (MyObj);
        }

        function SelectButton(MyObj)
        {
            // Do not execute the following lines (leaves button selected,
            // which can be annoying if window is not refreshed/closed...):
            //MyObj = GetCellObject(MyObj);
            //UnSelectButtons();
            //MyObj.className = "button-selectedOver";
        }
        function RollOver(MyObj)
        {
            MyObj = GetCellObject(MyObj);
            if (IsBrowserSafari())
            {
                if (m_prevObj && (m_prevObj != MyObj))
                {
                    RollOut(m_prevObj);
                }
                MyObj.className = "button-over";
                m_prevObj = MyObj;
            } else
            {
                MyObj.className = "button-over";
            }
        }

        function RollOut(MyObj)
        {
            MyObj = GetCellObject(MyObj);
            MyObj.className = "button";
        }
        function ShowTransString(Text)
        {
            var ObjId = "WorkareaTitlebar";
            var ObjShow = document.getElementById('_' + ObjId);
            var ObjHide = document.getElementById(ObjId);
            if ((typeof ObjShow != "undefined") && (ObjShow != null))
            {
                ObjShow.innerHTML = Text;
                ObjShow.style.display = "inline";
                if ((typeof ObjHide != "undefined") && (ObjHide != null))
                {
                    ObjHide.style.display = "none";
                }
            }

        }
        
        function HideTransString()
        {
            var ObjId = "WorkareaTitlebar";
            var ObjShow = document.getElementById(ObjId);
            var ObjHide = document.getElementById('_' + ObjId);

            if ((typeof ObjShow != "undefined") && (ObjShow != null))
            {
                ObjShow.style.display = "inline";
                if ((typeof ObjHide != "undefined") && (ObjHide != null))
                {
                    ObjHide.style.display = "none";
                }
            }
        }
        
        function resetCPostback()
        {
            $ektron(".isCPostData").attr("value", "");
        }
        
        function searchcollection()
        {
            if ($ektron("#txtSearch").val().indexOf('\"') != -1)
            {
                alert('remove all quote(s) then click search');
                return false;
            }
            $ektron("#txtSearch").clearInputLabel();
            $ektron(".isSearchPostData").attr("value", "1");
            $ektron(".isCPostData").attr("value", "true");
            document.forms[0].submit();
            return true;
        }
        
        function CheckForReturn(e)
        {
            var keynum;
            var keychar;

            if (window.event) // IE
            {
                keynum = e.keyCode
            }
            else if (e.which) // Netscape/Firefox/Opera
            {
                keynum = e.which
            }

            if (keynum == 13)
            {
                document.getElementById('btnSearch').focus();
            }
        }
        
        function LoadFolderPicker() //(type, tagtype, metadataFormTagId)
        {
            var pageObj, frameObj
            var id = 0;
            var title = '';
            var delimeterChar = ";";
            var metadataFormTagId = 1;
            var menuFlag = (window.location.toString().toLowerCase().indexOf("action=editmenu") ? "&menuflag=true" : "");

            if (isBrowserIE())
            {
                // Configure the Meta window to be visible:
                frameObj = document.getElementById('FolderPickerPage');
                if (ek_ma_validateObject(frameObj))
                {
                    window.scrollTo(0, 0); // ensure that the iframe will be in view.
                    frameObj.src = 'blankredirect.aspx?MetaSelectContainer.aspx?FolderID=' + id + '&browser=0&WantXmlInfo=1&metadataFormTagId=' + metadataFormTagId + '&separator=' + delimeterChar + '&selectids=' + id + '&selecttitles=' + title + menuFlag;

                    pageObj = document.getElementById('FolderPickerPageContainer');
                    pageObj.style.display = '';
                    pageObj.style.width = '100%'; //'85%';
                    pageObj.style.height = '100%'; //'90%';

                    // Ensure that the transparent layer completely covers the parent window:
                    pageObj = document.getElementById('FolderPickerAreaOverlay');
                    pageObj.style.display = '';
                    pageObj.style.width = '100%';
                    pageObj.style.height = '100%';
                }
            }
            else
            {
                // Browser is Netscape, use a seperate pop-up window:
                var windObj = window.open('blankredirect.aspx?MetaSelectContainer.aspx?FolderID=' + id + '&browser=1&WantXmlInfo=1&metadataFormTagId=' + metadataFormTagId + '&separator=' + delimeterChar + '&selectids=' + id + '&selecttitles=' + title + menuFlag, 'Preview', 'width=' + 600 + ',height=' + 400 + ',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no');
            }
        }

        function ek_ma_validateObject(obj)
        {
            return ((obj != null) &&
		((typeof (obj)).toLowerCase() != 'undefined') &&
		((typeof (obj)).toLowerCase() != 'null'))
        }

        function isBrowserIE()
        {
            var ua = window.navigator.userAgent.toLowerCase();
            return ((ua.indexOf('msie') > -1) && (!(ua.indexOf('opera') > -1)));
        }

        function ek_ma_CloseChildPage()
        {
            var pageObj = document.getElementById('FolderPickerPageContainer');

            // Configure the Meta window to be invisible:
            pageObj.style.display = 'none';
            pageObj.style.width = '1px';
            pageObj.style.height = '1px';

            // Ensure that the transparent layer does not cover any of the parent window:
            pageObj = document.getElementById('FolderPickerAreaOverlay');
            pageObj.style.display = 'none';
            pageObj.style.width = '1px';
            pageObj.style.height = '1px';
        }

        function ek_ma_CloseMetaChildPage()
        {
            ek_ma_CloseChildPage();
        }

        function ek_ma_ReturnMediaUploaderValue(selectedIdName, title, metadataFormTagId)
        {
            var obj, testObj;
            var delimeterChar = ";";
            var namIdArray, titleArray;
            var idx;

            // clear original values:
            ek_ma_ClearSelection(metadataFormTagId, "");

            // save new selections:
            var frm_associated_folder_list_obj = document.getElementById("associated_folder_id_list");
            if (frm_associated_folder_list_obj)
            {
                frm_associated_folder_list_obj.value = selectedIdName;
            }
            var frm_associated_folder_title_list_obj = document.getElementById("associated_folder_title_list");
            if (frm_associated_folder_title_list_obj)
            {
                frm_associated_folder_title_list_obj.value = title;
            }

            // list the items for the user:
            if (delimeterChar && delimeterChar.length)
            {
                namIdArray = selectedIdName.split(delimeterChar);
                titleArray = title.split(delimeterChar);
                for (idx = 0; idx < namIdArray.length; idx++)
                {
                    var itemId = namIdArray[idx];
                    var itemTitle;
                    if (titleArray && titleArray[idx])
                    {
                        itemTitle = titleArray[idx];
                    }
                    else
                    {
                        itemTitle = "";
                    }
                    ek_ma_addMetaRow(itemId, itemTitle, metadataFormTagId)
                }
            }
        }

        function ek_ma_ClearSelection(metadataFormTagId, msgText)
        {
            var childObj, tempEl, tblBodyObj, rowObj, cellObj, textObj;
            var containerObj = document.getElementById("EnhancedMetadataMultiContainer" + metadataFormTagId.toString());
            if (containerObj)
            {
                while (childObj = containerObj.lastChild)
                {
                    tempEl = containerObj.removeChild(childObj);
                }

                if (msgText && msgText.length)
                {
                    tblBodyObj = document.createElement("tbody");
                    tblBodyObj = containerObj.appendChild(tblBodyObj);
                    rowObj = document.createElement("tr");
                    rowObj = tblBodyObj.appendChild(rowObj);
                    cellObj = document.createElement("td");
                    cellObj = rowObj.appendChild(cellObj);
                    textObj = document.createTextNode(msgText);
                    textObj = cellObj.appendChild(textObj);
                }
            }

            var frm_associated_folder_list_obj = document.getElementById("associated_folder_id_list");
            if (frm_associated_folder_list_obj)
            {
                frm_associated_folder_list_obj.value = "";
            }

            var frm_associated_folder_title_list_obj = document.getElementById("associated_folder_title_list");
            if (frm_associated_folder_title_list_obj)
            {
                frm_associated_folder_title_list_obj.value = "";
            }
        }

        function ek_ma_addMetaRow(id, title, metadataFormTagId)
        {
            var tblBodyObj, rowObj, cellObj, textObj;
            var thumbnail, idx, textStr, obj;
            var cellBgColor = "";
            var containerObj = document.getElementById("EnhancedMetadataMultiContainer"
			+ metadataFormTagId.toString());
            if (containerObj)
            {
                if (id && id.length)
                {
                    // if no table-body, must add one:
                    tblBodyObj = containerObj.firstChild;
                    if (null == tblBodyObj)
                    {
                        tblBodyObj = document.createElement("tbody");
                        tblBodyObj = containerObj.appendChild(tblBodyObj);
                    }

                    // determine background color based on odd/even current row count:
                    if (tblBodyObj.childNodes && (tblBodyObj.childNodes.length & 1))
                    {
                        cellBgColor = "#eeeeee";
                    }

                    // add cell with title and id (with appropriate background color):
                    rowObj = document.createElement("tr");
                    rowObj = tblBodyObj.appendChild(rowObj);
                    cellObj = document.createElement("td");
                    cellObj = rowObj.appendChild(cellObj);
                    if (cellBgColor.length) // && cellObj.bgColor)
                    {
                        cellObj.bgColor = cellBgColor;
                    }

                    textStr = title + " (Folder ID: " + id + ")";
                    textObj = document.createTextNode(textStr);
                    textObj = cellObj.appendChild(textObj);
                }
            }
        }

        function ek_ma_getSelectedFormTagId() { return (1); }
        function ek_ma_getDelimiter(metadataFormTagId) { return (";"); }

        function ek_ma_getId(metadataFormTagId)
        {
            var result = "";
            var frm_associated_folder_list_obj = document.getElementById("associated_folder_id_list");
            if (frm_associated_folder_list_obj)
            {
                result = frm_associated_folder_list_obj.value;
            }
            return (result);
        }

        function ek_ma_getTitle(metadataFormTagId)
        {
            var result = "";
            var frm_associated_folder_title_list_obj = document.getElementById("associated_folder_title_list");
            if (frm_associated_folder_title_list_obj)
            {
                result = frm_associated_folder_title_list_obj.value;
            }
            return (result);
        }

        function metaselect_initialize()
        {
            var ids, title;
            var formTagId = ek_ma_getSelectedFormTagId();
            var frm_associated_folder_list_obj = document.getElementById("associated_folder_id_list");
            if (frm_associated_folder_list_obj)
            {
                ids = frm_associated_folder_list_obj.value;
            }

            var frm_associated_folder_title_list_obj = document.getElementById("associated_folder_title_list");
            if (frm_associated_folder_title_list_obj)
            {
                title = frm_associated_folder_title_list_obj.value;
            }

            if (ids != null && ids.length)
            {
                ek_ma_ReturnMediaUploaderValue(ids, title, formTagId);
            }
            else
            {
                ek_ma_ClearSelection(formTagId, "None selected");
            }
        }



        //--><!]]>
    </script>

    <asp:PlaceHolder id="phHeadTag" runat="server"></asp:PlaceHolder>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <meta http-equiv="Cache-Control" content="no-cache, must-revalidate" />
    <meta http-equiv="pragma" content="no-cache" />
    <title>Collections</title>

    <script type="text/javascript" src="java/stylehelper.js"></script>

    <style type="text/css">
        span.pathInfo
        {
            display: inline-block;
            color: #000000;
            margin-left: .5em;
            padding: .25em;
            border: solid 1px #ccc;
            cursor: default;
            background-color: #eee;
        }
        .info img
        {
            margin-right: .5em;
        }
    </style>
    <!-- Styles -->
    <link type="text/css" rel="stylesheet" href="java/plugins/modal/ektron.modal.css" />
    <link type="text/css" rel="stylesheet" href="java/plugins/treeview/ektron.treeview.css" />
    <!-- Scripts -->
    <script type="text/javascript" src="java/plugins/modal/ektron.modal.js"></script>

    <script type="text/javascript" src="java/plugins/treeview/ektron.treeview.js"></script>
    
    <script type="text/javascript">
        Ektron.ready(function()
        {
            if ($ektron.browser.msie == true && parseInt(jQuery.browser.version, 10) < 8)
            {
                var heightFixer = $ektron(".heightFix");
                heightFixer.css("height", heightFixer.outerHeight() + "px");
            }

            window.setTimeout( function() { CheckMenuLinkProtocol($ektron(".subMenuLinkText").get(0)); }, 100);
        });
    </script>
</head>
<body>
    <!-- #include file="java/collections.js" -->
    <input id="hidValue" type="hidden" runat="server" />
    
    <input id="hidSitePath" type="hidden" runat="server" />
    <asp:PlaceHolder ID="plhCollections" runat="server"></asp:PlaceHolder>
</body>
</html>
