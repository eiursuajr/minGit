function addItemToSelectList(control,text,value, uniqueId){
    if (uniqueId == null){
		uniqueId = "";
    }
    if(document.getElementById(value).value==""){
        setSelectedItems(control,false);
        return false;
    }
    var selectedItemsList = document.getElementById(control);
    var currentOption;
    var index=selectedItemsList.length;
    for (index = 0; index < selectedItemsList.length; index++) {
        currentOption = selectedItemsList.options[index];
        if(document.getElementById(value).value==currentOption.value){
            alert("duplicate Text/Value supplied");return false;
        }
    }
    index=selectedItemsList.length;
    document.getElementById(control).length=index+1;
    document.getElementById(control).options[index].value=document.getElementById(value).value;
    document.getElementById(control).options[index].text=document.getElementById(text).value;
    $ektron(document.getElementById(control).options[index]).bind("click", function()
    {
        editSelectList(control,text,value, uniqueId);
    });
    document.getElementById(text).value="";
    document.getElementById(value).value="";
    updateSelectedValues(control, uniqueId);
}

function updateSelectedValues(control, uniqueId){
    if (uniqueId == null){
		uniqueId = "";
    }

    var selectedItemsList = document.getElementById(control);
    var selectedValues="";

    for (index = 0; index < selectedItemsList.length; index++) {
        if(selectedValues==""){
            selectedValues=selectedItemsList.options[index].value;
        }else{
            selectedValues=selectedValues+";"+selectedItemsList.options[index].value;
        }
    }
    document.getElementById("frm_text_" + uniqueId).value=selectedValues;
}

function setSelectedItems(control,flag){
    var selectedItemsList = document.getElementById(control);
    for (index = 0; index < selectedItemsList.length; index++) {
        selectedItemsList.options[index].selected=flag;
    }
}

function updateItemToSelectList(control,text,value,uniqueId){
    if (uniqueId == null){
		uniqueId = "";
    }
    if(document.getElementById(value).value==""){
        setSelectedItems(control,false);
        return false;
    }
    var currentOption;
    var duplicateIndex=-1;
    var selectedItemsList = document.getElementById(control);
    for (index = 0; index < selectedItemsList.length; index++) {
        currentOption = selectedItemsList.options[index];
        if(document.getElementById(value).value==currentOption.value && !currentOption.selected){
            duplicateIndex=index;
        }
    }
    if(duplicateIndex>-1)
    {
        alert("text already exists");
        return false;
    }
    for (index = 0; index < selectedItemsList.length; index++) {
        currentOption = selectedItemsList.options[index];

        if (currentOption.selected == true){
            document.getElementById(control).options[index].text=document.getElementById(text).value;
            document.getElementById(control).options[index].value=document.getElementById(value).value;
            $ektron(document.getElementById(control).options[index]).bind("click", function()
            {
                editSelectList(control,text,value, uniqueId);
            });
            document.getElementById(text).value="";
            document.getElementById(value).value="";
            break;
        }
    }
    updateSelectedValues(control, uniqueId);
}

function removeItemsFromSelectList(control, uniqueId){
    if (uniqueId == null){
		uniqueId = "";
    }
    var currentOption;
    var selectedItemsList = document.getElementById(control);
    for (index = 0; index < selectedItemsList.length; index++) {
        currentOption = selectedItemsList.options[index];
        if (currentOption.selected == true){
            document.getElementById(control).options[index]=null;
        }
    }
    document.getElementById("ItemText" + uniqueId).value="";
    document.getElementById("ItemText" + uniqueId).value="";
    updateSelectedValues(control, uniqueId);
}

function editSelectList(control,textControl,valueControl,uniqueId){
    if (uniqueId == null){
		uniqueId = "";
    }
    var currentOption;
    var selectedItemsList = document.getElementById(control);

    for (index = 0; index < selectedItemsList.length; index++){
      currentOption = selectedItemsList.options[index];
      if (currentOption.selected == true){
        document.getElementById(textControl).value=selectedItemsList.options[index].text;
        break;
      }
    }
    updateSelectedValues(control, uniqueId);
}

function populateSelectedList(control,values){
    var selectedItemsList = document.getElementById(control);
    if(values!="" && selectedItemsList.length==0) {
        var availableList = values.split(";");
        var selectedItemsList = document.getElementById(control);
        var index=selectedItemsList.length;
	    for (index1 = 0; index1 < availableList.length; index1++){
            document.getElementById(control).length=index+1;
            document.getElementById(control).options[index].value=availableList[index1];
            document.getElementById(control).options[index].text=availableList[index1];
            index+=1;
		}
	}
}

function moveItemUp(control,uniqueId){
    if (uniqueId == null){
		uniqueId = "";
    }
    var currentOption;
    var prevOption;
    var prevOptionValue;
    var prevOptionText;
    var prevOptionSelectState;
    var wasPrevIterSelected = false;
    var wasPrevIterMoved = false;

    var selectedItemsList = document.getElementById(control);

    for (index = 0; index < selectedItemsList.length; index++){
        currentOption = selectedItemsList.options[index];
        if (currentOption.selected == true){
            if (index == 0){
              wasPrevIterMoved = false;
            }
            else{
                if (wasPrevIterSelected){
                    if (wasPrevIterMoved){
                        prevOption = selectedItemsList.options[index - 1];
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
                    else{
                        wasPrevIterMoved = false;
                    }
                }
                else{
                    prevOption = selectedItemsList.options[index - 1];
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
          else {
            wasPrevIterSelected = false;
            wasPrevIterMoved = false;
        }
    }
    updateSelectedValues(control, uniqueId);
}

function moveItemDown(control,uniqueId){
    if (uniqueId == null){
		uniqueId = "";
    }
    var currentOption;
    var nextOption;
    var nextOptionValue;
    var nextOptionText;
    var nextOptionSelectState;
    var wasPrevIterSelected = false;
    var wasPrevIterMoved = false;
    var selectedItemsList = document.getElementById(control);

    for (index = selectedItemsList.length - 1; index >= 0; index--){
        currentOption = selectedItemsList.options[index];
        if (currentOption.selected == true) {
            if (index == selectedItemsList.length - 1) {
                wasPrevIterMoved = false;
            }
            else{
                if (wasPrevIterSelected){
                    if (wasPrevIterMoved) {
                        nextOption = selectedItemsList.options[index + 1];
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
                    else {
                        wasPrevIterMoved = false;
                    }
                }
                else{
                    nextOption = selectedItemsList.options[index + 1];
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
        else {
            wasPrevIterSelected = false;
            wasPrevIterMoved = false;
        }
    }
    updateSelectedValues(control, uniqueId);
}

function SetSelectListDefault(control, uniqueId, confirmMsg) {
    if (uniqueId == null){
		uniqueId = "";
    }
	var defObj = document.getElementById('frm_text_' + uniqueId + 'default');
	if (null != defObj) {
		if (confirm(confirmMsg)) {
			// clear the control:
			var targCtrl = document.getElementById(control);
			if (null != targCtrl) {
				targCtrl.length = 0;
			}
			// now populate it with default values:
			var strDefaultValue = defObj.value;
			populateSelectedList(control, strDefaultValue);
			// now update the hidden field:
			updateSelectedValues(control, uniqueId);
		}
	}
}

function ek_ma_updateEditButton()
{
    var objEdit = $ektron('#dvMetadata a.buttonEdit');

    objEdit.removeClass('buttonEdit');
    objEdit.addClass('buttonUpdate');
    $ektron('span#spn_edit').attr('innerHTML', 'Update');
}

function ek_ma_updateUpdateButton()
{
    var objEdit = $ektron('#dvMetadata a.buttonUpdate');

    objEdit.removeClass('buttonUpdate');
    objEdit.addClass('buttonEdit');
    $ektron('span#spn_edit').attr('innerHTML', 'Edit');
}
