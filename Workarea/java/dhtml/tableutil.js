var INPUT_NAME_PREFIX = 'txtBundle'; // this is being set via script
var OPT_INPUT_NAME_PREFIX = 'opt_'; // this is being set via script
var RADIO_NAME = 'radInput'; // this is being set via script
var TABLE_NAME = 'tblProductItems'; // this should be named in the HTML
var OPT_TABLE_NAME = 'tblOptions'; // this should be named in the HTML
var DIV_NAME = 'kitgroups'; // this should be named in the HTML
var ROW_BASE = 1; // first number (for display)
var hasLoaded = false;

window.onload=fillInRows;

function fillInRows()
{
	hasLoaded = true;
}

// CONFIG:
// myRowObject is an object for storing information about the table rows
function myRowObject(one, two, three, four, five)
{
	this.one = one; // text object
	this.two = two; // text object
	this.three = three; // text object
	this.four = four; // hidden text object
	this.five = five; // input radio object
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
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.four.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.four.checked) {
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
function addRowToTable(num, eid, etitle)
{
	var titleExist = $ektron('table#tblProductItems').find('input:hidden');
	var numTitle = titleExist.length;
	var numI = 0;
	for(numI = 0; numI < numTitle; numI++)
	{
	    if(titleExist[numI].value == eid)
	    {
	        alert("The item you have selected, already exists in the list. Please select other item.");
	        return false;
	    }
	}
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

		// cell 0 - text
		var cell0 = row.insertCell(0);
		var textPos = document.createTextNode(iteration);
		cell0.appendChild(textPos);

		// cell 1 - text
		var cell1 = row.insertCell(1);
		var textId = document.createTextNode(eid);
		cell1.appendChild(textId);

		// cell 2 - text
		var cell2 = row.insertCell(2);
		var textEntry = document.createTextNode(iteration);
		if (etitle != null) { textEntry = document.createTextNode(etitle); }
		cell2.appendChild(textEntry);

		// cell 3 - hidden text
		var cell3 = row.insertCell(3);
		var hdnField = document.createElement('input');
		hdnField.setAttribute('type', 'hidden');
		hdnField.setAttribute('name', INPUT_NAME_PREFIX + iteration);
		hdnField.setAttribute('id', INPUT_NAME_PREFIX + iteration);
		hdnField.setAttribute('value', eid.toString());
		cell3.appendChild(hdnField);

		// cell 4 - input radio
		var cell4 = row.insertCell(4);
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
		cell4.appendChild(raEl);

		// Pass in the elements you want to reference later
		// Store the myRow object in each row
		row.myRow = new myRowObject(textPos, textId, textEntry, hdnField, raEl);
		return true;
	}
}

function getCheckedItemIndex()
{
    var selectedIdx = -1;

	if (hasLoaded) {
		var checkedObjArray = new Array();
		var cCount = 0;

		var tbl = document.getElementById(TABLE_NAME);

		if ( tbl.tBodies[0].rows.length > 0 )
		{
		    for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		        if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			    //if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.five.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.five.checked)
				try{
					if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.five.getAttribute('type') == 'radio' && (tbl.tBodies[0].rows[i].myRow.five.getAttribute("checked") == 1 || tbl.tBodies[0].rows[i].myRow.five.value == 1)) {
					    selectedIdx = i;
					    break;
			    	}
				}
				catch(ex){
					//
				}
		    }
        }
        else
        {
            selectedIdx = -2;
        }
	}

	return selectedIdx;
}

function deleteChecked()
{
	if (hasLoaded) {
		var checkedObjArray = new Array();
		var cCount = 0;

		var tbl = document.getElementById(TABLE_NAME);
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			//if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.five.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.five.checked) {
			if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.five.getAttribute('type') == 'radio' && (tbl.tBodies[0].rows[i].myRow.five.getAttribute("checked") == 1 || tbl.tBodies[0].rows[i].myRow.five.value == 1)) {
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

function WriteBundleValues()
{
    var sValue = "";
    var itype = document.getElementById('hdn_entrytype').value;
	if (hasLoaded && (itype == 3 || itype == 1 || itype == 0)) {
		var tbl = document.getElementById(TABLE_NAME);
		if (tbl != null){
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myRow == null) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myRow) {
				if (sValue != "") { sValue += "," + tbl.tBodies[0].rows[i].myRow.two.data; }
				else { sValue = tbl.tBodies[0].rows[i].myRow.two.data; }
			    }
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
function reorderRows(drctn)
{
    var tbl = document.getElementById(TABLE_NAME);
    var idx = -1;
    var reacquire = false;

    for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
        try { var checkValid = tbl.tBodies[0].rows[i].myRow.one.data; }
        catch(err) { reacquire = true; }
        if (tbl.tBodies[0].rows[i].myRow == null || reacquire) { tbl.tBodies[0].rows[i].myRow = GetRowInfo(tbl.tBodies[0].rows[i]); }
        if (document.forms[0].radInput && document.forms[0].radInput.length > 0 && document.forms[0].radInput[i] != null)
        {
            if ( document.forms[0].radInput[i].checked ) {
			    idx = i;
			    break;
		    }
		} else {
		    if (tbl.tBodies[0].rows[i].myRow && tbl.tBodies[0].rows[i].myRow.five.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myRow.five.checked) {
		        idx = i;
			    break;
		    }
		}
	}
    if (idx > -1 && hasLoaded) {
        if (drctn == 'up')
        {
            if (idx > 0)
            {
				var row = $ektron(tbl.tBodies[0].rows[idx]);
				row.insertBefore(row.prev());
				row.parent().children("tr").each(function(i){
					$ektron(this).attr("class", "itemrow" + String(i));
					$ektron(this).children("td:first").text(String(i + 1));
				});

				var tbody = $ektron(tbl.tBodies[0]);
				tbody.find("tr:even").css("background-color", "#ffffff");
				tbody.find("tr:odd").css("background-color", "#d8e6ff");

                //if (tbl.tBodies[0].rows[idx].myRow == null) { tbl.tBodies[0].rows[idx].myRow = GetRowInfo(tbl.tBodies[0].rows[idx]); }
                //if (tbl.tBodies[0].rows[idx - 1].myRow == null) { tbl.tBodies[0].rows[idx - 1].myRow = GetRowInfo(tbl.tBodies[0].rows[idx - 1]); }
                //var eTitleTemp = tbl.tBodies[0].rows[idx].myRow.three.data;
                //var eIdTemp = tbl.tBodies[0].rows[idx].myRow.two.data;
                //var ePosTemp = tbl.tBodies[0].rows[idx].myRow.four.value;
                //tbl.tBodies[0].rows[idx].myRow.two.data = tbl.tBodies[0].rows[idx - 1].myRow.two.data;
                //tbl.tBodies[0].rows[idx - 1].myRow.two.data = eIdTemp;
                //tbl.tBodies[0].rows[idx].myRow.three.data = tbl.tBodies[0].rows[idx - 1].myRow.three.data;
                //tbl.tBodies[0].rows[idx - 1].myRow.three.data = eTitleTemp;
                //tbl.tBodies[0].rows[idx].myRow.four.value = tbl.tBodies[0].rows[idx - 1].myRow.four.value;
                //tbl.tBodies[0].rows[idx - 1].myRow.four.value = ePosTemp;
                //WriteRowInfo(tbl.tBodies[0].rows[idx]);
                //WriteRowInfo(tbl.tBodies[0].rows[idx - 1]);
                //tbl.tBodies[0].rows[idx - 1].myRow.five.checked = true;
                //tbl.tBodies[0].rows[idx].myRow.five.checked = false;
            }
        }
        else if (drctn == 'down')
        {
            if ((idx + 1) < tbl.tBodies[0].rows.length)
            {
				var row = $ektron(tbl.tBodies[0].rows[idx]);
				row.insertAfter(row.next());
				row.parent().children("tr").each(function(i){
					$ektron(this).attr("class", "itemrow" + String(i));
					$ektron(this).children("td:first").text(String(i + 1));
				});

				var tbody = $ektron(tbl.tBodies[0]);
				tbody.find("tr:even").css("background-color", "#ffffff");
				tbody.find("tr:odd").css("background-color", "#d8e6ff");

                //if (tbl.tBodies[0].rows[idx].myRow == null) { tbl.tBodies[0].rows[idx].myRow = GetRowInfo(tbl.tBodies[0].rows[idx]); }
                //if (tbl.tBodies[0].rows[idx + 1].myRow == null) { tbl.tBodies[0].rows[idx + 1].myRow = GetRowInfo(tbl.tBodies[0].rows[idx + 1]); }
                //var eTitleTemp = tbl.tBodies[0].rows[idx].myRow.three.data;
                //var eIdTemp = tbl.tBodies[0].rows[idx].myRow.two.data;
                //var ePosTemp = tbl.tBodies[0].rows[idx].myRow.four.value;
                //tbl.tBodies[0].rows[idx].myRow.two.data = tbl.tBodies[0].rows[idx + 1].myRow.two.data;
                //tbl.tBodies[0].rows[idx + 1].myRow.two.data = eIdTemp;
                //tbl.tBodies[0].rows[idx].myRow.three.data = tbl.tBodies[0].rows[idx + 1].myRow.three.data;
                //tbl.tBodies[0].rows[idx + 1].myRow.three.data = eTitleTemp;
                //tbl.tBodies[0].rows[idx].myRow.four.value = tbl.tBodies[0].rows[idx + 1].myRow.four.value;
                //tbl.tBodies[0].rows[idx + 1].myRow.four.value = ePosTemp;
                //WriteRowInfo(tbl.tBodies[0].rows[idx]);
                //WriteRowInfo(tbl.tBodies[0].rows[idx + 1]);
                //tbl.tBodies[0].rows[idx + 1].myRow.five.checked = true;
                //tbl.tBodies[0].rows[idx].myRow.five.checked = false;
            }
        }
        // i_reorderRows(tbl, 0);
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
				tbl.tBodies[0].rows[i].myRow.five.value = count; // input radio

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

    var textPos;
	var textId;
    var textEntry;
	var hdnField;
	var raEl;
    if ( rowObj.children != null ) {

	    textPos = document.createTextNode(rowObj.children[0].innerText);
	    textId = document.createTextNode(rowObj.children[1].innerText);
        textEntry = document.createTextNode(rowObj.children[2].innerText);
	    hdnField = rowObj.children[3].children[0];
	    raEl = rowObj.children[4].children[0];

    } else {

        textPos = document.createTextNode(rowObj.childNodes[1].innerHTML);
	    textId = document.createTextNode(rowObj.childNodes[3].innerHTML);
        textEntry = document.createTextNode(rowObj.childNodes[5].innerHTML);
	    hdnField = rowObj.childNodes[7].childNodes[0];
	    raEl = rowObj.childNodes[9].childNodes[0];

    }
	return new myRowObject(textPos, textId, textEntry, hdnField, raEl);
}
function WriteRowInfo(rowObj)
{

    if ( rowObj.children != null )
    {

	    rowObj.children[0].innerText = rowObj.myRow.one.data;
	    rowObj.children[1].innerText = rowObj.myRow.two.data;
        rowObj.children[2].innerText = rowObj.myRow.three.data;
        rowObj.children[3].innerHTML = "<input type='hidden' name='" + INPUT_NAME_PREFIX + rowObj.children[0].innerText + "' id='" + INPUT_NAME_PREFIX + rowObj.children[0].innerText + "' value='" + rowObj.children[1].innerText + "' />";
	    rowObj.children[4].innerHTML = '<input type="radio" name="' + RADIO_NAME + '" value="' + rowObj.children[0].innerText + '">';

    }
    else
    {

        if ( rowObj.childNodes.length > 5)
        {
            rowObj.childNodes[1].innerHTML = rowObj.myRow.one.data;
	        rowObj.childNodes[3].innerHTML = rowObj.myRow.two.data;
            rowObj.childNodes[5].innerHTML = rowObj.myRow.three.data;
	        rowObj.childNodes[7].innerHTML = "<input type='hidden' name='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "' id='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "' value='" + rowObj.myRow.two.data + "' />";
	        rowObj.childNodes[9].innerHTML = '<input type="radio" name="' + RADIO_NAME + '" value="' + rowObj.myRow.one.data + '">';
        }
        else
        {
            rowObj.childNodes[0].innerHTML = rowObj.myRow.one.data;
	        rowObj.childNodes[1].innerHTML = rowObj.myRow.two.data;
            rowObj.childNodes[2].innerHTML = rowObj.myRow.three.data;
	        rowObj.childNodes[3].innerHTML = "<input type='hidden' name='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "' id='" + INPUT_NAME_PREFIX + rowObj.myRow.one.data + "' value='" + rowObj.myRow.two.data + "' />";
	        rowObj.childNodes[4].innerHTML = '<input type="radio" name="' + RADIO_NAME + '" value="' + rowObj.myRow.one.data + '">';
        }

    }
}




//
// myOptionObject is an object for storing information about the table rows
//
function myOptionObject(one, two, three, four, five, six)
{
	this.one = one; // text object
	this.two = two; // text object
	this.three = three; // text object
	this.four = four; // hidden text object
	this.five = five; // text object
	this.six = six; // radio object
}
function DeleteCheckedOption(tblId)
{
	if (hasLoaded) {
		var checkedObjArray = new Array();
		var cCount = 0;

		var tbl = document.getElementById(OPT_TABLE_NAME + tblId);
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myOption == null) { tbl.tBodies[0].rows[i].myOption = GetRowInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myOption && tbl.tBodies[0].rows[i].myOption.five.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myOption.five.checked) {
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
function addOption(num, tblId)
{
	if (hasLoaded) {
		var tbl = document.getElementById(OPT_TABLE_NAME + tblId);
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

		// cell 0 - text
		var cell0 = row.insertCell(0);
		var textPos = document.createTextNode(iteration);
		cell0.width = '1%';
		cell0.appendChild(textPos);

		// cell 1 - text field
		var cell1 = row.insertCell(1);
		var txtName = document.createElement('input');
		txtName.setAttribute('type', 'text');
		txtName.setAttribute('id', OPT_INPUT_NAME_PREFIX + tblId + '_' + iteration + '_name');
		txtName.setAttribute('maxlength', 50);
		txtName.setAttribute('size', 50);
		txtName.setAttribute('value', '');
		txtName.setAttribute('name', OPT_INPUT_NAME_PREFIX + tblId + '_' + iteration + '_name');
		cell1.appendChild(txtName);

		// cell 2 - text field
		var cell2 = row.insertCell(2);
		var txtExtra = document.createElement('input');
		txtExtra.setAttribute('type', 'text');
		txtExtra.setAttribute('id', OPT_INPUT_NAME_PREFIX + tblId + '_' + iteration + '_text');
		txtExtra.setAttribute('maxlength', 50);
		txtExtra.setAttribute('size', 10);
		txtExtra.setAttribute('value', '');
		txtExtra.setAttribute('name', OPT_INPUT_NAME_PREFIX + tblId + '_' + iteration + '_text');
		cell2.appendChild(txtExtra);

		// cell 3 - hidden text
		var cell3 = row.insertCell(3);
		var hdnId = document.createElement('input');
		hdnId.setAttribute('id', OPT_INPUT_NAME_PREFIX + tblId + '_' + iteration + '_id');
		hdnId.setAttribute('type', 'hidden');
		hdnId.setAttribute('value', '0');
		hdnId.setAttribute('name', OPT_INPUT_NAME_PREFIX + tblId + '_' + iteration + '_id');
		cell3.width = '1%';
		cell3.appendChild(hdnId);

		// cell 4 - text fiel
		var cell4 = row.insertCell(4);
		var txtChange = document.createElement('input');
		txtChange.setAttribute('type', 'text');
		txtChange.setAttribute('id', OPT_INPUT_NAME_PREFIX + tblId + '_' + iteration + '_val');
		txtChange.setAttribute('maxlength', 10);
		txtChange.setAttribute('size', 5);
		txtChange.setAttribute('value', '0.00');
		txtChange.setAttribute('name', OPT_INPUT_NAME_PREFIX + tblId + '_' + iteration + '_val');
		cell4.width = '3%';
		cell4.appendChild(txtChange);

		// cell 5 - input radio
		var cell5 = row.insertCell(5);
		var raEl;
		try {
			raEl = document.createElement('<input type="radio" name="' + RADIO_NAME + tblId + '" value="' + iteration + '">');
			var failIfNotIE = raEl.name.length;
		} catch(ex) {
			raEl = document.createElement('input');
			raEl.setAttribute('type', 'radio');
			raEl.setAttribute('name', RADIO_NAME + tblId);
			raEl.setAttribute('value', iteration);
		}
		cell5.width = '1%';
		cell5.appendChild(raEl);

		// Pass in the elements you want to reference later
		// Store the myRow object in each row
		row.myOption = new myOptionObject(textPos, txtName, txtExtra, hdnId, txtChange, raEl);
	}
}
function DeleteCheckedOption(tblId) {
	if (hasLoaded) {
		var checkedObjArray = new Array();
		var cCount = 0;

		var tbl = document.getElementById(OPT_TABLE_NAME + tblId);
		for (var i=0; i<tbl.tBodies[0].rows.length; i++) {
		    if (tbl.tBodies[0].rows[i].myOption == null) { tbl.tBodies[0].rows[i].myOption = GetOptionInfo(tbl.tBodies[0].rows[i]); }
			if (tbl.tBodies[0].rows[i].myOption && tbl.tBodies[0].rows[i].myOption.six.getAttribute('type') == 'radio' && tbl.tBodies[0].rows[i].myOption.six.checked) {
				checkedObjArray[cCount] = tbl.tBodies[0].rows[i];
				cCount++;
			}
		}
		if (checkedObjArray.length > 0) {
			var rIndex = checkedObjArray[0].sectionRowIndex;
			deleteOptions(checkedObjArray);
			i_reorderOptions(tbl, rIndex);
		} else { alert('Please Select an option.'); }
	}
}
function GetOptionInfo(rowObj) {
	var textPos;
	var txtName;
	var txtExtra;
	var hdnId;
    var txtChange;
    var raEl;

    if ( rowObj.children != null )
    {

        textPos = document.createTextNode(rowObj.children[0].innerText);
	    txtName = rowObj.children[1].children[0];
	    txtExtra = rowObj.children[2].children[0];
	    hdnId = rowObj.children[3].children[0];
        txtChange = rowObj.children[4].children[0];
        raEl = rowObj.children[5].children[0];

    }
    else
    {

        textPos = document.createTextNode(rowObj.childNodes[1].innerHTML);
	    txtName = rowObj.childNodes[3].childNodes[0];
	    txtExtra = rowObj.childNodes[5].childNodes[0];
	    hdnId = rowObj.childNodes[7].childNodes[0];
        txtChange = rowObj.childNodes[9].childNodes[0];
        raEl = rowObj.childNodes[11].childNodes[0];

    }

	return new myOptionObject(textPos, txtName, txtExtra, hdnId, txtChange, raEl);
}
function deleteOptions(rowObjArray) {
	if (hasLoaded) {
		for (var i=0; i<rowObjArray.length; i++) {
			var rIndex = rowObjArray[i].sectionRowIndex;
			rowObjArray[i].parentNode.deleteRow(rIndex);
		}
	}
}
function i_reorderOptions(tbl, startingIndex) {
	if (hasLoaded) {
		if (tbl.tBodies[0].rows[startingIndex]) {
			var count = startingIndex + ROW_BASE;
			for (var i=startingIndex; i<tbl.tBodies[0].rows.length; i++) {

				// CONFIG: next line is affected by myRowObject settings
				tbl.tBodies[0].rows[i].myOption.one.data = count; // text

				// CONFIG: next line is affected by myRowObject settings
				// tbl.tBodies[0].rows[i].myRow.two.name = INPUT_NAME_PREFIX + count; // input text

				// CONFIG: next line is affected by myRowObject settings
				// var tempVal = tbl.tBodies[0].rows[i].myRow.two.value.split(' '); // for debug purposes
				// tbl.tBodies[0].rows[i].myRow.two.value = count + ' was' + tempVal[0]; // for debug purposes

				// CONFIG: next line is affected by myRowObject settings
				tbl.tBodies[0].rows[i].myOption.six.value = count; // input radio

				// CONFIG: requires class named classy0 and classy1
				tbl.tBodies[0].rows[i].className = 'itemrow' + (count % 2);

				count++;
			}
		}
	}
}
var iGrpIdx = 100;
function addGroup() {
    var dvGroups = document.getElementById(DIV_NAME);
    iGrpIdx = iGrpIdx + 1;
    var sDiv = "";
    // sDiv += '<div id="kitgroup' + iGrpIdx + '">';
    sDiv += '<fieldset><table width="100%" border="0">';
    sDiv += '<tr>';
    sDiv += '	<td align="left" nowrap="nowrap" valign="top" style="width: 80px" class="input-box-text">Name:</td>';
    sDiv += '	<td align="left" valign="top">';
    sDiv += '		<input type="text" id="optg_' + iGrpIdx + '_name" name="optg_' + iGrpIdx + '_name" maxlength="50" size="50" value="" />';
    sDiv += '<input type="hidden" id="optg_' + iGrpIdx + '_id" name="optg_' + iGrpIdx + '_id" value="0"" />';
    sDiv += '<input type="hidden" id="optg_' + iGrpIdx + '_posidx" name="optg_' + iGrpIdx + '_posidx" value="' + iGrpIdx + '"" /><br />	</td>';
    sDiv += '	<td align="right" valign="top" style="width: 1px"><img src="../images/ui/icons/delete.png" border="0" onclick="deleteGroup(' + iGrpIdx + ', this);" /></td>';
    sDiv += '</tr><tr>';
    sDiv += '	<td align="left" nowrap="nowrap" valign="top" style="width: 80px" class="input-box-text">Image:</td>';
    sDiv += '	<td align="left" valign="top" colspan="2">';
    sDiv += '		<input type="text" id="optg_' + iGrpIdx + '_img" name="optg_' + iGrpIdx + '_img" maxlength="50" size="50" value="" /><br />	</td>';
    sDiv += '</tr><tr>';
    sDiv += '	<td align="left" nowrap="nowrap" valign="top" style="width: 80px" class="input-box-text">Description:</td>';
    sDiv += '	<td align="left" valign="top" colspan="2">';
    sDiv += '		<input type="text" id="optg_' + iGrpIdx + '_desc" name="optg_' + iGrpIdx + '_desc" maxlength="255" size="50" value="" /><br />	</td>';
    sDiv += '</tr><tr>';
    sDiv += '	<td align="left" nowrap="nowrap" valign="top" style="width: 80px" class="input-box-text">Options:<br/></td>';
    sDiv += '	<td align="left" valign="top" colspan="2">';
    sDiv += '	    <table border="0" width="100%" cellspacing="0" id="tblOptions' + iGrpIdx + '">';
    sDiv += '	        <thead><tr class="item_header"><th></th><th>Name</th><th>Extra Text</th><th></th><th>Change</th><th>&#160;</th></tr></thead>';
    sDiv += '	        <tbody>';
    sDiv += '	        </tbody>';
    sDiv += '	    </table>';
    sDiv += '	    &nbsp;<img src="../images/ui/icons/add.png" border="0" onclick="addOption(null, ' + iGrpIdx + ');" />&nbsp;&nbsp;<img src="../images/ui/icons/delete.png" border="0" onclick="DeleteCheckedOption(' + iGrpIdx + ');" />';
    sDiv += ' </td>';
    sDiv += '</tr>';
    sDiv += '</table>';
    sDiv += '</fieldset><br/>';
    // sDiv += '</div>';
    var txt = document.createTextNode(sDiv);
    var dvGroup = document.createElement('div');
    dvGroup.setAttribute('id', 'kitgroup' + iGrpIdx);
    dvGroup.innerHTML = sDiv;
    dvGroups.appendChild(dvGroup);
}
function deleteGroup(idx, thisobj) {
    var dvGroups = document.getElementById(DIV_NAME);
    if (idx == null) { idx = dvGroups.children.length; }
    var dvGroup = document.getElementById('kitgroup' + idx);
    if(dvGroup != null && idx > -1) {
        dvGroups.removeChild(dvGroup);
        updateGroups(dvGroups);
    }
}
function updateGroups(dvGroups)
{

    if ( dvGroups.children != null )
    {

        for (var i = 0; i < dvGroups.children.length; i++)
        {

            dvGroups.children[i].children[0].children[0].rows[0].children[1].children[0].id = 'optg_' + i + '_name';
            dvGroups.children[i].children[0].children[0].rows[0].children[1].children[0].name = 'optg_' + i + '_name';
            dvGroups.children[i].children[0].children[0].rows[0].children[1].children[2].id = 'optg_' + i + '_posidx';
            dvGroups.children[i].children[0].children[0].rows[0].children[1].children[2].name = 'optg_' + i + '_posidx';

        }

    }
    else
    {
        var groupIdx = 0;
        for (var i = 0; i < dvGroups.childNodes.length; i++)
        {

            if (dvGroups.childNodes[i].tagName == 'DIV')
            {

                //       Div           fieldset      table         rows
                dvGroups.childNodes[i].childNodes[1].childNodes[0].rows[0].childNodes[3].childNodes[1].id = 'optg_' + groupIdx + '_name';
                dvGroups.childNodes[i].childNodes[1].childNodes[0].rows[0].childNodes[3].childNodes[1].name = 'optg_' + groupIdx + '_name';
                dvGroups.childNodes[i].childNodes[1].childNodes[0].rows[0].childNodes[3].childNodes[3].id = 'optg_' + groupIdx + '_posidx';
                dvGroups.childNodes[i].childNodes[1].childNodes[0].rows[0].childNodes[3].childNodes[3].name = 'optg_' + groupIdx + '_posidx';
                groupIdx = groupIdx + 1;

            }

        }

    }

}