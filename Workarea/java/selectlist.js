function addItemToSelectList(control,text,value){
    if(document.getElementById(value).value==""){
        setSelectedItems(control,false);
        return false;
    }    
    if(!CheckForillegalChar(document.getElementById(value).value)){
            document.getElementById(value).value = "";
		    return;
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
    document.getElementById(text).value="";
    document.getElementById(value).value="";
    updateSelectedValues(control);
}

function updateSelectedValues(control){
    var selectedItemsList = document.getElementById(control);
    var selectedValues="";
    for (index = 0; index < selectedItemsList.length; index++) {
        if(selectedValues==""){
            selectedValues=selectedItemsList.options[index].value;
        }else{
            selectedValues=selectedValues+";"+selectedItemsList.options[index].value;
        }
    }
    document.getElementById("selectedvalues").value=selectedValues; 
}

function setSelectedItems(control,flag){
    var selectedItemsList = document.getElementById(control);
    for (index = 0; index < selectedItemsList.length; index++) {
        selectedItemsList.options[index].selected=flag;
    }
}

function updateItemToSelectList(control,text,value){
    if(document.getElementById(value).value==""){
        setSelectedItems(control,false);
        return false;
    }    
    if(!CheckForillegalChar(document.getElementById(value).value)){
            document.getElementById(value).value = "";
		    return;
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
    if(duplicateIndex>-1){alert("text already exists");return false;}
    for (index = 0; index < selectedItemsList.length; index++) {
        currentOption = selectedItemsList.options[index];

        if (currentOption.selected == true){
            document.getElementById(control).options[index].text=document.getElementById(text).value;
            document.getElementById(control).options[index].value=document.getElementById(value).value;
            document.getElementById(text).value="";
            document.getElementById(value).value="";
            break;
        }
    }
    updateSelectedValues(control);
}

function removeItemsFromSelectList(control){
    var currentOption;
    var selectedItemsList = document.getElementById(control);
    for (index = 0; index < selectedItemsList.length; index++) {
        currentOption = selectedItemsList.options[index];
        if (currentOption.selected == true){
            document.getElementById(control).options[index]=null;
        }
    }
    document.getElementById("ItemText").value="";
    document.getElementById("ItemText").value="";
    updateSelectedValues(control);
}

function editSelectList(control,textControl,valueControl){
    var currentOption;
    var selectedItemsList = document.getElementById(control);
    
    for (index = 0; index < selectedItemsList.length; index++){
      currentOption = selectedItemsList.options[index];
      if (currentOption.selected == true){
        document.getElementById(textControl).value=selectedItemsList.options[index].text;
        break;
      }
    }
    updateSelectedValues(control);
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

function moveItemUp(control){
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
    updateSelectedValues(control);
}

function moveItemDown(control) {
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
    updateSelectedValues(control);
}

function CheckForillegalChar(value) {

    if (Trim(value) == ''){
        alert("Please enter a name for the SelectList Item."); 
        return false; 
        } 
    else {     
        //var tagRegEx = /[!"#$%&'()*+,./:;<=>?@[\\\]^`{|}~ ]+/; Defect # 49068 - Item in select list custom user property cannot contain spaces
        //Removing space from above regex
        var tagRegEx = /[!"#$%&'()*+,./:;<=>?@[\\\]^`{|}~]+/;
            if(tagRegEx.test(value)==true)
                {alert("SelectList Item can only include alphanumeric characters.");
                return false; }                       
    }
    return true;
}