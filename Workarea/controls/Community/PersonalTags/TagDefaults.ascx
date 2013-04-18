<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TagDefaults.ascx.cs" Inherits="controls_Community_PersonalTags_TagDefaults" %>
<%@ Reference Page="../../../Community/PersonalTags.aspx" %>

<script type="text/javascript" language="javascript">
//This code ported form FlagSets.aspx

function doSubmit(){
    var tagList = "";
    for (i = 0; i < arrFlag.length; i++) {
        var elem = document.getElementById("tagLanguage_" + i);
    
        if (!CheckForillegalChar(arrFlagName[i]))
            return false; 
         
        if (tagList.length > 0) tagList += ";";
        tagList += arrFlagName[i];
    }
    
    var elem = document.getElementById("newTags");
    elem.value = tagList

    document.forms[0].submit();
}

function CheckForillegalChar(tag) {
   if (Trim(tag) == '')
   {
        alert('<asp:Literal ID="error_EmptyTag" runat="server"/>');
        return false;
   } else { 

        //alphanumeric plus _ -
        var tagRegEx = /[!"#$%&'()*+,./:;<=>?@[\\\]^`{|}~ ]+/;
        if(tagRegEx.test(tag)==true) {
            alert('<asp:Literal ID="error_InvalidChars" runat="server"/>');
            return false;
        }
       
   }
   return true;
}

var arrFlagID = new Array(0);
var arrFlag = new Array(0);
var arrFlagName = new Array(0);

function addFlag() {
    
    arrFlagID.push("0");
    arrFlag.push(arrFlag.length);
    arrFlagName.push("");
    displayFlag();
}

function addFlagInit(fid,fname) {
      arrFlagID.push(fid);
      arrFlag.push(arrFlag.length);
      arrFlagName.push(fname);
}

function displayFlag() {
    var sItem = '';
    var sList = '';
    document.getElementById('pFlag').innerHTML='';
    for (intI = 0; intI < arrFlag.length; intI++) {
        sItem = createFlag(arrFlagID[intI], arrFlag[intI], arrFlagName[intI], intI, arrFlag.length);
        sList += sItem;
    }
    document.getElementById('pFlag').innerHTML = sList;
    document.getElementById('Flaglength').value = arrFlag.length;

}

function saveFlag(intId,strValue,type) {
    if (type == "tagText") {
        arrFlagName[intId]=strValue;
    }
}  


function removeFlag(id) {
    //remove last
    var cnfm = confirm("<%=m_containerPage.RefMsg.GetMessage("js confirm remove tag")%>");
    if (cnfm == true)
    {
      if (arrFlag.length > 0) { 
         arrFlagID.splice(id,1); 
         arrFlag.pop(); 
         arrFlagName.splice(id,1);
      }
      
      //if last flag removed, add new blank one
      if (arrFlag.length == 0){
        addFlag();
      }
      displayFlag(); 
    }
}


</script> 

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div> 
<div class="ektronPageContainer ektronPageInfo">
    <div id="tbledit">
	    <asp:Literal ID="literalTags" runat="server" />
	    <div class="ektronTopSpace"></div>
	    <asp:Literal ID="literalAddTag" runat="server" />
    </div>
</div>

<input type="hidden" id="newTags" name="newTags" value=""/>
