var INPUT_NAME_PREFIX = 'txtA'; // this is being set via script
var OPT_INPUT_NAME_PREFIX = 'opt_'; // this is being set via script
var RADIO_NAME = 'radInput'; // this is being set via script
var TABLE_NAME = 'tblAttributes'; // this should be named in the HTML
var ROW_BASE = 1; // first number (for display)
var hasLoaded = false;

window.onload=fillInRows;

function fillInRows()
{
	hasLoaded = true;
}

// CONFIG:
// myRowObject is an object for storing information about the table rows
function myRowObject(one, two, three, four, five, six, seven, eight, nine)
{
	this.one = one; // text object
	this.two = two; // text object
	this.three = three; // text object
	this.four = four; // text object
	this.five = five; // hidden text object
	this.six = six; // hidden text object
	this.seven = seven; // hidden text object
	this.eight = eight; // hidden text object
	this.nine = nine; // input radio object
}

/*
 * insertRowToTable
 * Insert and reorder
 */
function insertRowToTable()
{
	if (hasLoaded) {
		var tbl = document.getElementById(TABLE_NAME);
		var rowToInsertAt = tbl.tBodies[0].rows.length;
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
            if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.nine.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.nine.checked) {
				rowToInsertAt = i;
				break;
			}
		}
		addRowToTable(rowToInsertAt);
		i_reorderRows(tbl, rowToInsertAt);
	}
}
function editRowInTable(idx, aname, atype, adef)
{
    var rowPosition = idx;
    idx = idx - 1;
    if (hasLoaded) {
        var row;
        var tbl = document.getElementById(TABLE_NAME);
        if (tbl.tBodies[0].rows[idx].myRow == null) { tbl.tBodies[0].rows[idx].myRow = GetRowInfo(tbl.tBodies[0].rows[idx]); }
        
        var aid = tbl.tBodies[0].rows[idx].myRow.two.data
//	    tbl.tBodies[0].rows[idx].myRow.two.data = aname;
//	    tbl.tBodies[0].rows[idx].myRow.three.data = GetAttrLabel(atype);
//	    tbl.tBodies[0].rows[idx].myRow.four.data = adef;
//	    tbl.tBodies[0].rows[idx].myRow.six.value = aname;
//	    tbl.tBodies[0].rows[idx].myRow.seven.value = atype;
//	    tbl.tBodies[0].rows[idx].myRow.eight.value = adef;

	    var row = tbl.tBodies[0].rows[idx]; // get the row
		
		// cell 0 - text
		row.deleteCell(0);
		var cell0 = row.insertCell(0);
		var textAPos = document.createTextNode(rowPosition);
		cell0.appendChild(textAPos);
		
		// cell 1 - text
		row.deleteCell(1);
		var cell1 = row.insertCell(1);
		var textAName = document.createTextNode(aname);
		cell1.appendChild(textAName);
		
		// cell 2 - text
		row.deleteCell(2);
		var cell2 = row.insertCell(2);
		var textAType = document.createTextNode(GetAttrLabel(atype));
		cell2.appendChild(textAType);
		
		// cell 3 - text
		row.deleteCell(3);
		var cell3 = row.insertCell(3);
		var textADef = document.createTextNode(adef);
		cell3.appendChild(textADef);		
		
		var hdnAIdField = document.getElementById(INPUT_NAME_PREFIX + 'Id' + rowPosition)
		row.deleteCell(4);
		var cell4 = row.insertCell(4);
		cell4.appendChild(hdnAIdField);
		
		var hdnANameField = document.createElement('input');
		hdnANameField.setAttribute('type', 'hidden');
		hdnANameField.setAttribute('name', INPUT_NAME_PREFIX + 'Name' + rowPosition);
		hdnANameField.setAttribute('id', INPUT_NAME_PREFIX + 'Name' + rowPosition);	
		hdnANameField.setAttribute('value', aname);
		cell4.appendChild(hdnANameField);
		
		var hdnATypeField = document.createElement('input');
		hdnATypeField.setAttribute('type', 'hidden');
		hdnATypeField.setAttribute('name', INPUT_NAME_PREFIX + 'Type' + rowPosition);
		hdnATypeField.setAttribute('id', INPUT_NAME_PREFIX + 'Type' + rowPosition);	
		hdnATypeField.setAttribute('value', atype);
		cell4.appendChild(hdnATypeField);
		
		var hdnADefField = document.createElement('input');
		hdnADefField.setAttribute('type', 'hidden');
		hdnADefField.setAttribute('name', INPUT_NAME_PREFIX + 'Def' + rowPosition);
		hdnADefField.setAttribute('id', INPUT_NAME_PREFIX + 'Def' + rowPosition);	
		hdnADefField.setAttribute('value', adef);
		cell4.appendChild(hdnADefField);
		
		// cell 5 - input radio
		row.deleteCell(5);
		var cell5 = row.insertCell(5);
		var raEl;
		try {
			raEl = document.createElement('<input type="radio" name="' + RADIO_NAME + '" value="' + rowPosition + '">');
			var failIfNotIE = raEl.name.length;
		} catch(ex) {
			raEl = document.createElement('input');
			raEl.setAttribute('type', 'radio');
			raEl.setAttribute('name', RADIO_NAME);
			raEl.setAttribute('value', rowPosition);
		}
		cell5.appendChild(raEl);
	    
	    // UpdateRowInfo(tbl.tBodies[0].rows[idx]);
	    
	    row.myRow = new myRowObject(textAPos, textAName, textAType, textADef, hdnAIdField, hdnANameField, hdnATypeField, hdnADefField, raEl);
    }
}
function addRowToTable(num, aid, aname, atype, adef)
{
	if (hasLoaded) {
		var tbl = document.getElementById(TABLE_NAME);
		var nextRow = tbl.tBodies[0].rows.length;
		var iteration = nextRow + ROW_BASE;
		var i = 0;
		var count = 0;
		var objExistAttr = $ektron("table#tblAttributes").find("input[id^=txtAName]");
		if (objExistAttr.length > 0)
		{
		    for(i = 0; i < objExistAttr.length; i++)
		    {
		        if(objExistAttr[i].value == aname)
		        {
		            count = count + 1;
		        }
		    }
		}		
        if (count == 0)
        {
            if (num == null) { 
	            num = nextRow;
            } else {
	            iteration = num + ROW_BASE;
            }
            var row = tbl.tBodies[0].insertRow(num); // add the row
    		
            row.className = 'itemrow' + (iteration % 2);
    		
            // cell 0 - text
            var cell0 = row.insertCell(0);
            var textAPos = document.createTextNode(iteration);
            cell0.appendChild(textAPos);
    		
            // cell 1 - text
            var cell1 = row.insertCell(1);
            var textAName = document.createTextNode(aname);
            cell1.appendChild(textAName);
    		
            // cell 2 - text
            var cell2 = row.insertCell(2);
            var textAType = document.createTextNode(GetAttrLabel(atype));
            cell2.appendChild(textAType);
            // cell 3 - text
            var cell3 = row.insertCell(3);
            var textADef = document.createTextNode(adef);
            cell3.appendChild(textADef);		
    		
            // cell 4 - 4 hidden texts
            var cell4 = row.insertCell(4);
            var hdnAIdField = document.createElement('input');
            hdnAIdField.setAttribute('type', 'hidden');
            hdnAIdField.setAttribute('name', INPUT_NAME_PREFIX + 'Id' + iteration);
            hdnAIdField.setAttribute('id', INPUT_NAME_PREFIX + 'Id' + iteration);	
            hdnAIdField.setAttribute('value', aid.toString());
            cell4.appendChild(hdnAIdField);
            var hdnANameField = document.createElement('input');
            hdnANameField.setAttribute('type', 'hidden');
            hdnANameField.setAttribute('name', INPUT_NAME_PREFIX + 'Name' + iteration);
            hdnANameField.setAttribute('id', INPUT_NAME_PREFIX + 'Name' + iteration);	
            hdnANameField.setAttribute('value', aname);
            cell4.appendChild(hdnANameField);
            var hdnATypeField = document.createElement('input');
            hdnATypeField.setAttribute('type', 'hidden');
            hdnATypeField.setAttribute('name', INPUT_NAME_PREFIX + 'Type' + iteration);
            hdnATypeField.setAttribute('id', INPUT_NAME_PREFIX + 'Type' + iteration);	
            hdnATypeField.setAttribute('value', atype);
            cell4.appendChild(hdnATypeField);
            var hdnADefField = document.createElement('input');
            hdnADefField.setAttribute('type', 'hidden');
            hdnADefField.setAttribute('name', INPUT_NAME_PREFIX + 'Def' + iteration);
            hdnADefField.setAttribute('id', INPUT_NAME_PREFIX + 'Def' + iteration);	
            hdnADefField.setAttribute('value', adef);
            cell4.appendChild(hdnADefField);
    		
            // cell 5 - input radio
            var cell5 = row.insertCell(5);
            var raEl;
            try {
	            raEl = document.createElement('<input type="radio" name="' + RADIO_NAME + '" value="' + iteration + '">');
	            var failIfNotIE = raEl.name.length;
            } catch(ex) {
	            raEl = document.createElement('input');
	            raEl.setAttribute('type', 'radio');
	            raEl.setAttribute('name', RADIO_NAME);
	            raEl.setAttribute('value', iteration);
            }
            cell5.appendChild(raEl);

            row.myRow = new myRowObject(textAPos, textAName, textAType, textADef, hdnAIdField, hdnANameField, hdnATypeField, hdnADefField, raEl);
        }
        else
        {
            alert("An attribute by this name already exists.\n\nPlease enter different name.");
            return false;
        }
	}
}

function getCheckedInt(vithValue)
{
    var iRet = -1;
	if (hasLoaded) {
		var tbl = document.getElementById(TABLE_NAME);
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.nine.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.nine.checked) {
				if (vithValue) { iRet = tbl.tBodies[0].rows[i].myRow.five.value; } else { iRet = i; } 
				break;
			}
		}
	}
	return iRet;
}
function getCheckedObj()
{
    var oRet = null;
	if (hasLoaded) {
		var tbl = document.getElementById(TABLE_NAME);
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.nine.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.nine.checked) {
				oRet = tbl.tBodies[0].rows[i].myRow;
				break;
			}
		}
	}
	return oRet;
}
function deleteChecked()
{
	if (hasLoaded) {
		var checkedObjArray = new Array();
		var cCount = 0;
	
		var tbl = document.getElementById(TABLE_NAME);
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.nine.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.nine.checked) {
				checkedObjArray[cCount] = tbl.tBodies[0].rows[i];
				cCount++;
			}
		}
		if (checkedObjArray.length > 0) {
			var rIndex = checkedObjArray[0].sectionRowIndex;
			deleteRows(checkedObjArray);
			i_reorderRows(tbl, rIndex);
		}
	}
}

function WriteValues()
{
    var sValue = "";
    var itype = document.getElementById('drp_type').options[document.getElementById('drp_type').selectedIndex].value;
	if (hasLoaded && (itype == 3 || itype == 1 || itype == 0)) {
		var tbl = document.getElementById(TABLE_NAME);
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow) {
				if (sValue != "") { sValue += "," + tbl.tBodies[0].rows[i].myRow.two.data; }
				else { sValue = tbl.tBodies[0].rows[i].myRow.two.data; }
			}
		}
	}
	document.getElementById('hdn_bundled').value = sValue; 
}

// If there isn't an element with an onclick event in your row, then this function can't be used.
function deleteCurrentRow(obj)
{
	if (hasLoaded) {
		var delRow = obj.parentNode.parentNode;
		var tbl = delRow.parentNode.parentNode;
		var rIndex = delRow.sectionRowIndex;
		var rowArray = new Array(delRow);
		deleteRows(rowArray);
		i_reorderRows(tbl, rIndex);
	}
}
function i_reorderRows(tbl, startingIndex)
{
	if (hasLoaded) {
		if (tbl.tBodies[0].rows[startingIndex]) {
			var count = startingIndex + ROW_BASE;
			for (var i=startingIndex; i<tbl.tBodies[0].rows.length; i++) {
			
				// CONFIG: next line is affected by myRowObject settings
				tbl.tBodies[0].rows[i].myRow.one.data = count; // text
				
				// CONFIG: next line is affected by myRowObject settings
				// tbl.tBodies[0].rows[i].myRow.two.name = INPUT_NAME_PREFIX + count; // input text
				
				// CONFIG: next line is affected by myRowObject settings
				// var tempVal = tbl.tBodies[0].rows[i].myRow.two.value.split(' '); // for debug purposes
				// tbl.tBodies[0].rows[i].myRow.two.value = count + ' was' + tempVal[0]; // for debug purposes
				
				// CONFIG: next line is affected by myRowObject settings
				tbl.tBodies[0].rows[i].myRow.nine.value = count; // input radio
				
				// CONFIG: requires class named classy0 and classy1
				tbl.tBodies[0].rows[i].className = 'itemrow' + (count % 2);
				
				count++;
			}
		}
	}
}
function deleteRows(rowObjArray) {
	if (hasLoaded) {
		for (var i=0; i<rowObjArray.length; i++) {
			var rIndex = rowObjArray[i].sectionRowIndex;
			rowObjArray[i].parentNode.deleteRow(rIndex);
		}
	}
}
function GetRowInfo(rowObj) {

    if (rowObj.children != null) {
	
	    var textAPos = document.createTextNode(rowObj.children[0].innerText);
	    var textAName = document.createTextNode(rowObj.children[1].innerText);
	    var textAType = document.createTextNode(rowObj.children[2].innerText);
	    var textADef = document.createTextNode(rowObj.children[3].innerText);
	    var hdnAIdField = rowObj.children[4].children[0];
	    var hdnANameField = rowObj.children[4].children[1];
	    var hdnATypeField = rowObj.children[4].children[2];
	    var hdnADefField = rowObj.children[4].children[3];
	    var raEl = rowObj.children[5].children[0];
    
    } else {
    
        var textAPos = document.createTextNode(rowObj.childNodes[1].innerHTML);
	    var textAName = document.createTextNode(rowObj.childNodes[3].innerHTML);
	    var textAType = document.createTextNode(rowObj.childNodes[5].innerHTML);
	    var textADef = document.createTextNode(rowObj.childNodes[7].innerHTML);
	    var hdnAIdField = rowObj.childNodes[9].childNodes[0];
	    var hdnANameField = rowObj.childNodes[9].childNodes[1];
	    var hdnATypeField = rowObj.childNodes[9].childNodes[2];
	    var hdnADefField = rowObj.childNodes[9].childNodes[3];
	    var raEl = rowObj.childNodes[11].childNodes[0];
    
    }
	return new myRowObject(textAPos, textAName, textAType, textADef, hdnAIdField, hdnANameField, hdnATypeField, hdnADefField, raEl);
}
function UpdateRowInfo(rowObj) {	
    var tmpTwo = rowObj.myRow.two.data;
    var tmpFour = rowObj.myRow.four.data;
    
    if (rowObj.children != null) {
	    
	    rowObj.children[1].innerText = tmpTwo;
        rowObj.children[2].innerText = rowObj.myRow.three.data;
        rowObj.children[3].innerText = tmpFour;
       
	    rowObj.children[4].children[1].value = tmpTwo;
	    rowObj.children[4].children[2].value = rowObj.myRow.seven.value;
	    rowObj.children[4].children[3].value = tmpFour
	    
	} else {
	    
	    rowObj.childNodes[3].innerHTML = tmpTwo;
	    rowObj.childNodes[5].innerHTML = rowObj.myRow.three.data;
	    rowObj.childNodes[7].innerHTML = tmpFour;
	    
	    rowObj.childNodes[9].childNodes[1].value = tmpTwo;
	    rowObj.childNodes[9].childNodes[2].value = rowObj.myRow.seven.value;
	    rowObj.childNodes[9].childNodes[3].value = tmpFour
	    
	}
}
