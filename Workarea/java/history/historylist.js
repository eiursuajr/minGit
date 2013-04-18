function CompareAnalytics(itemid, itemlang) {

    var oldid = GetValue(document.getElementsByName('oldid'));
    var diff = GetValue(document.getElementsByName('diff'));

    if (oldid == diff)
    {
        alert("The items are the same");
    }
    else
    {
        var eOldVerTd = $ektron("input[value='" + oldid + "']").parents(".history-list").map(function () {  
                                 if ("TD" == this.tagName)
                                    return this;  
                              }) 
        var oldVer = eOldVerTd.next(".history-list").text();
        var eDiffVerTd = $ektron("input[value='" + diff + "']").parents(".history-list").map(function () {  
                                 if ("TD" == this.tagName)
                                    return this;  
                              }) 
        var ver = eDiffVerTd.next(".history-list").text();
        window.open('analytics/compare.aspx?itemid=' + itemid + '&oldid=' + oldid + '&oldver=' + oldVer + '&diff=' + diff + '&ver=' + ver + '&LangType=' + itemlang, 'Analytics400', 'width=900,height=580,scrollable=1,resizable=1');
    }

}

function GetValue(radioObj) {

    if (!radioObj)
        return "";
    var radioLength = radioObj.length;
    if (radioLength == undefined)
        if (radioObj.checked)
        return radioObj.value;
    else
        return "";
    for (var i = 0; i < radioLength; i++) {
        if (radioObj[i].checked) {
            return radioObj[i].value;
        }
    }
    return "";

}
function EmailUser(userid, windowname) {
    window.open('email.aspx?userarray=' + userid, windowname, 'width=460,height=425,scrollable=1,resizable=1');
}