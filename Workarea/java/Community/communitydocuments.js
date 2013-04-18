var EkWorkspaceEditId = 0;
var alertCopyStr = '';
var alertMoveStr = '';
var alertDeleteStr = '';
var alertWorkspace = '';
var alertCatName = '';
var ErrCatName = '';
var ErrWorkSpaceName = '';

function WorkspaceCleanAjaxContent() {
    //clean out content before refresh.
    if (document.getElementById("EkTB_ajaxContent") != null) {
        document.getElementById("EkTB_ajaxContent").innerHTML = "";
    }
}
function WorkspaceShareOnClick(uniqueId, ids, userId, communityDocumentsObjcetType) {
    if (ids == '') {
        alert("Currently there are no folders to share.");
        ektb_remove();
        return false;
    }
    var arId = ids.split(',');
    var i;
    var name, value;
    var query = '';
    var upperBound = 4;
    if (communityDocumentsObjcetType == 1) {
        upperBound = 2;
    }
    for (i = 0; i < arId.length; i++) {
        name = arId[i].replace(uniqueId + '_workspace_choice_', '');
        var j;
        for (j = 0; j < upperBound; j++) {
            value = '';
            var objChoices = document.getElementById(arId[i] + j);
            if (objChoices.checked) {
                value = objChoices.value;
                break;
            }
        }
        if (value != '') {
            if (query == '') {
                query = name + ',' + value;
            } else {
                query += ';' + name + ',' + value;
            }
        }
    }
    WorkspaceCleanAjaxContent();
    __LoadTaxonomy(IAjax.getArguements(), 'pagerequest=shareworkspace&control=' + uniqueId + '&uid=' + userId + '&share=' + query + '&type=' + communityDocumentsObjcetType);
}

function WorkspaceShowSharePanel(uniqueId, bDisplay) {
    var obj = document.getElementById(uniqueId + '_WorkspaceShare');
    var objAddWS = document.getElementById(uniqueId + '_addWS');
    if (bDisplay) {
        if (obj != null) {
            obj.style.display = 'block';
        }

        if (objAddWS != null) {
            objAddWS.style.display = 'none';
        }
    } else {
        if (obj != null) {
            obj.style.display = 'none';
        }
        if (objAddWS != null) {
            objAddWS.style.display = 'block';
        }
    }
}
function WorkspaceShowAddPanel(obj, uniqueId) {
    EkWorkspaceEditId = 0;
    WorkspaceShowSharePanel(uniqueId, false);
    var t = obj.title || obj.name || null;
    var a = obj.href || obj.alt;
    var g = obj.rel || false;
    ektb_show(t, a, g);
    obj.blur();
    return false;

}

function WorkspaceShowDeletePanel(obj, uniqueId) {
    var text = document.getElementById(uniqueId + '_EkSubCategoryDeleteMsg').value;
    var id = document.getElementById(uniqueId + '_EkSubCategoryIdEdit').value;
    if (confirm(text)) {
        ektb_remove();
        WorkspaceCleanAjaxContent();
        __LoadTaxonomy(IAjax.getArguements(), 'pagerequest=deleteworkspace&control=' + uniqueId + '&id=' + id);
    }
}

function WorkspaceShowEditPanel(obj, uniqueId, bDisplay, text, id, delMsg) {
    var t = obj.title || obj.name || null;
    var a = obj.href || obj.alt;
    var g = obj.rel || false;
    ektb_show(t, a, g);
    obj.blur();

    EkWorkspaceEditId = id;
    document.getElementById(uniqueId + '_EkSubCategoryDeleteMsg').value = delMsg;
    document.getElementById(uniqueId + '_EkSubCategoryIdEdit').value = id;
    document.getElementById(uniqueId + '_EkSubCategoryTitleEdit').value = text;
    return false;
}

function ShowWorkspaceSharePanel(obj, UniqueId, closeText) {
    var ct = ("undefined" != typeof closeText) ? closeText : "close";
    var t = obj.title || obj.name || null;
    var a = obj.href || obj.alt;
    var g = obj.rel || false;
    ektb_show(t, a, g, ct);
    obj.blur();
}

function ShowDynamicContentBox(obj, UniqueId) {
    var t = obj.title || obj.name || null;
    var a = obj.href || obj.alt;
    var g = obj.rel || false;
    ektb_show(t, a, g);
    obj.blur();
}

function WorkspaceAddCategory(uniqueId, path, folderId, action, type) {
    var name = '';
    if (action == 'update') {
        if (document.getElementById(uniqueId + '_EkSubCategoryTitleEdit')) {
            name = document.getElementById(uniqueId + '_EkSubCategoryTitleEdit').value;
            action = 'update';
        }
    }
    else {
        if (document.getElementById(uniqueId + '_EkSubCategoryTitle')) {
            name = document.getElementById(uniqueId + '_EkSubCategoryTitle').value;
            action = 'add';
        }
    }

    name = ektjq.trim(name);

    if (name == '') {
        if (type == '')
            alert(alertCatName);
        else
            alert(alertWorkspace);
        return;
    }

    if ((name.indexOf('>') > -1) || (name.indexOf('<') > -1)) {
        if (type == '')
            alert(ErrCatName + "'<', '>'");
        else
            alert(ErrWorkSpaceName + "'<', '>'");

        return;
    }
    if ((name.indexOf("\\") >= 0) || (name.indexOf("/") >= 0) || (name.indexOf(":") >= 0) || (name.indexOf("*") >= 0) || (name.indexOf("?") >= 0) || (name.indexOf("\"") >= 0) || (name.indexOf("<") >= 0) || (name.indexOf(">") >= 0) || (name.indexOf("|") >= 0) || (name.indexOf("&") >= 0) || (name.indexOf("\'") >= 0)) {
        if (type == '')
            alert(ErrCatName + "('\\', '/', ':', '*', '?', ' \" ', '<', '>', '|', '&', '\'').");
        else
            alert(ErrWorkSpaceName + "('\\', '/', ':', '*', '?', ' \" ', '<', '>', '|', '&', '\'').");
        return;
    }

    var upperBound = 4;
    if (type == "Group") {
        upperBound = 2;
    }
    var j;
    var query;
    var fId;
    var value;
    if (type != '') {
        for (j = 0; j < upperBound; j++) {
            value = '';
            fId = uniqueId + '_workspace_choice_add_' + j;
            var objChoices = document.getElementById(fId);
            fId = fId.replace(uniqueId + '_workspace_choice_add_', '');
            if (objChoices.checked) {
                value = objChoices.value;
                break;
            }

        }
    }
    WorkspaceCleanAjaxContent();
    ektb_remove();
    OnAddCategory(name, document.getElementById("taxonomyselectedtree").value, folderId, action, EkWorkspaceEditId, type, value);

}

function WorkspaceClickButton(e, uniqueId, path, folderId, action, type) {
    var evt = e ? e : window.event;
    if (evt.keyCode == 13) {
        evt.cancelBubble = true;
        if (evt.stopPropagation)
            evt.stopPropagation();
        WorkspaceAddCategory(uniqueId, path, folderId, action, type);
        return false;
    }
}

/* Manage Content Selection Buffer */
var arMenuData = new Array();

function AddToBuffer(data) {
    arMenuData.push(data);
    ShowBuffer();
}
function IsInBuffer(data) {
    for (var i = 0; i < arMenuData.length; i++) {
        if (arMenuData[i].Id == data.Id) {
            return true;
        }
    }
    return false;
}
function CopyToBuffer(id, title) {
    var data = new Object();
    data.Id = id;
    data.Title = title;

    if (!IsInBuffer(data)) {
        AddToBuffer(data);
    }

}
function ShowBuffer() {
    var titles = '';
    var ids = '';

    if (arMenuData.length === 0) {
        ektj$("div.Ekt_AccordianItem").hide();
        ektj$("div.Ekt_Accordian").hide();

        return;
    } else {
        if (ektj$("div.Ekt_AccordianMain")[0] != null) {
            ektj$("div.Ekt_AccordianMain")[0].style.visibility = 'visible';
        }
        ektj$("div.Ekt_Accordian").show();
    }

    for (var i = 0; i < arMenuData.length; i++) {
        if (titles == '') {
            titles = arMenuData[i].Title;
        } else {
            titles += "," + arMenuData[i].Title;
        }
        if (ids == '') {
            ids = arMenuData[i].Id;
        }
        else {
            ids += "," + arMenuData[i].Id;
        }
    }
    ektj$("#CopyBuffer").empty();
    ektj$("#CopyBuffer").append(titles.replace(/,/g, "<br/>"));
    ektj$("#EktSelBufferId")[0].value = escape(ids);
    ektj$("#EktSelBufferTitle")[0].value = escape(titles);
}
function ClearBuffer() {
    arMenuData = new Array();
    ShowBuffer();
}

function DeleteItem(id, title, uniqueId) {
    if (confirm(alertDeleteStr + ':\n' + unescape(title))) {
        setTimeout("Ektron.DMSMenu.HideMenu();", 25);
        __LoadTaxonomy(IAjax.getArguements(), 'pagerequest=deleteitem&delid=' + id + '&control=' + uniqueId + '&__selectedTreeId=' + document.getElementById("taxonomyselectedtree").value);
    }
}
function DeleteSelectedItems(uniqueId) {
    if (confirm(alertDeleteStr + ':\n' + unescape(ektj$("#EktSelBufferTitle")[0].value))) {

        __LoadTaxonomy(IAjax.getArguements(), 'pagerequest=deleteselitems&control=' + uniqueId);
    }
}
function CopySelectedItems(uniqueId, folderId, categoryId) {
    if (confirm(alertCopyStr + ':\n' + unescape(ektj$("#EktSelBufferTitle")[0].value))) {

        __LoadTaxonomy(IAjax.getArguements(), 'pagerequest=copyselitems&control=' + uniqueId + '&folderId=' + folderId + '&newcatId=' + categoryId);
        ClearBuffer();

    }
}
function MoveSelectedItems(uniqueId, folderId, categoryId) {
    if (confirm(alertMoveStr + ':\n' + unescape(ektj$("#EktSelBufferTitle")[0].value))) {

        __LoadTaxonomy(IAjax.getArguements(), 'pagerequest=moveselitems&control=' + uniqueId + '&folderId=' + folderId + '&newcatId=' + categoryId);
        ClearBuffer();
    }
}
/* Manage Content Selection Buffer */
