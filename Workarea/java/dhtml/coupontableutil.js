var INPUT_NAME_PREFIX = 'txtitem'; // this is being set via script
var RADIO_NAME = 'radInput'; // this is being set via script
var TABLE_NAME = 'tblApplies'; // this should be named in the HTML
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
	this.two = two; // image object
	this.three = three; // text object
	this.four = four; // text object
	this.five = five; // text object
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

/*
 * addRowToTable
 * Inserts at row 'num', or appends to the end if no arguments are passed in. Don't pass in empty strings.
 */
function addRowToTable(num, eid, elang, etitle, etype)
{
	if (hasLoaded) {
		var tbl = document.getElementById(TABLE_NAME);
		var nextRow = tbl.tBodies[0].rows.length;
		var iteration = nextRow + ROW_BASE;
		if (num == null) { 
			num = nextRow;
		} else {
			iteration = num + ROW_BASE;
		}
		
		// add the row
		var row = tbl.tBodies[0].insertRow(num);
		
		row.className = 'itemrow' + (iteration % 2);
	
		// CONFIG: This whole section can be configured
		
		var cell0 = row.insertCell(0);
		var textPos = document.createTextNode(iteration);
		cell0.appendChild(textPos);

		var cell1 = row.insertCell(1);		
		var imgType = document.createElement('img');
		imgType.setAttribute('src', GetEntryImage(etype));
		cell1.appendChild(imgType);
		
		var cell2 = row.insertCell(2);
		var textId = document.createTextNode(eid);
		cell2.appendChild(textId);
		
		var cell3 = row.insertCell(3);
		var textLang = document.createTextNode(elang);
		cell3.appendChild(textLang);
		
		var cell4 = row.insertCell(4);
		var textEntry = document.createTextNode(etitle);
		cell4.appendChild(textEntry);
		
		var cell5 = row.insertCell(5);
		var hdnId = document.createElement('input');
		hdnId.setAttribute('type', 'hidden');
		hdnId.setAttribute('name', INPUT_NAME_PREFIX + 'id' + iteration);
		hdnId.setAttribute('id', INPUT_NAME_PREFIX + 'id' + iteration);	
		hdnId.setAttribute('value', eid.toString());
		cell5.appendChild(hdnId);
		var hdnLang = document.createElement('input');
		hdnLang.setAttribute('type', 'hidden');
		hdnLang.setAttribute('name', INPUT_NAME_PREFIX + 'lang' + iteration);
		hdnLang.setAttribute('id', INPUT_NAME_PREFIX + 'lang' + iteration);	
		hdnLang.setAttribute('value', elang);
		cell5.appendChild(hdnLang);
		var hdnIdx = document.createElement('input');
		hdnIdx.setAttribute('type', 'hidden');
		hdnIdx.setAttribute('name', INPUT_NAME_PREFIX + 'posidx' + iteration);
		hdnIdx.setAttribute('id', INPUT_NAME_PREFIX + 'posidx' + iteration);	
		hdnIdx.setAttribute('value', iteration);
		cell5.appendChild(hdnIdx);
		
		var cell6 = row.insertCell(6);
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
		cell6.appendChild(raEl);
		
		// row.myRow = new myRowObject(textPos, textId, textEntry, hdnId, hdnEntry, hdnIdx, raEl);
		row.myRow = new myRowObject(textPos, imgType, textId, textLang, textEntry, hdnId, hdnLang, hdnIdx, raEl);
	}
}

// CONFIG: this entire function is affected by myRowObject settings
// If there isn't a checkbox in your row, then this function can't be used.
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
				// tbl.tBodies[0].rows[i].myRow.one.data = count; // text
				tbl.tBodies[0].rows[i].children[5].children[2].name = INPUT_NAME_PREFIX + 'posidx' + count;
				tbl.tBodies[0].rows[i].children[5].children[2].id = INPUT_NAME_PREFIX + 'posidx' + count;
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
function getCheckedInt() {
    var iRet = -1;
	if (hasLoaded) {
		var tbl = document.getElementById(TABLE_NAME);
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.nine.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.nine.checked) {
				iRet = i;
				break;
			}
		}
	}
	return iRet;
}
function deleteChecked() {
	if (hasLoaded) {
		var checkedObjArray = new Array();
		var cCount = 0;
	
		var tbl = document.getElementById(TABLE_NAME);
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i], i); }
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
function GetRowInfo(rowObj, idx) {
	var textPos = document.createTextNode(rowObj.children[0].innerText);
	var imgType = rowObj.children[1].children[0];
	var textId = document.createTextNode(rowObj.children[2].innerText);
	var textLang = document.createTextNode(rowObj.children[3].innerText);
    var textEntry = document.createTextNode(rowObj.children[4].innerText);
	var hdnId = rowObj.children[5].children[0];
	var hdnLang = rowObj.children[5].children[1];
	var hdnIdx = rowObj.children[5].children[2];
	var raEl = rowObj.children[6].children[0];

	return new myRowObject(textPos, imgType, textId, textLang, textEntry, hdnId, hdnLang, hdnIdx, raEl);
}
function WriteRowInfo(rowObj) {	
	rowObj.children[0].innerText = "<img src='" + GetEntryImage(rowObj.myRow.one) + "' />";
	rowObj.children[1].innerText = rowObj.myRow.two.data; //id
    rowObj.children[2].innerText = rowObj.myRow.three.data; //lang
    rowObj.children[3].innerText = rowObj.myRow.four.data; //title
	rowObj.children[4].innerHTML = "<input type='hidden' name='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "id' id='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "id' value='" + rowObj.myRow.two.data + "' />";
	rowObj.children[4].innerHTML += "<input type='hidden' name='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "lang' id='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "lang' value='" + rowObj.myRow.three.data + "' />";
	rowObj.children[4].innerHTML += "<input type='hidden' name='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "type' id='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "type' value='" + rowObj.myRow.four.data + "' />";
	rowObj.children[4].innerHTML += "<input type='hidden' name='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "posidx' id='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "posidx' value='" + rowObj.myRow.one.data + "' />";
	rowObj.children[5].innerHTML = '<input type="radio" name="' + RADIO_NAME + '" value="' + rowObj.myRow.one.data + '">';
}
