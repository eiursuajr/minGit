var ecmMonths = "";
		function CallCalendar(date, pagename, ctrl, formname){
			regexp1 = / /gi;
			var template;
			var paremeter;
			var Url;
			var items = pagename.split("?");
			if (items.length == 2 ) {
				template = items[0].toLowerCase();
				paremeter = items[1].toLowerCase();
			}
			else{
				template = pagename.toLowerCase();
				paremeter= "";
			}

			if (date.replace(regexp1, "") == "")
			{
				Url = template + '?form=' + formname + '&ctrl=' + ctrl;
				if (paremeter.lenght){
					Url = Url + '&' + paremeter;
				}
				PopUpWindow (Url, 'Calendar', 325, 280, 0, 1);
			}
			else {
				if (!CheckDateOnly(date)) {
					date = "";
				}
				date = RemoveLeadingSpaces(date);
				Url = template + '?form=' + formname + '&ctrl=' + ctrl + '&startdate=' + date + '&showdate=1';
				if (paremeter.length){
					Url = Url + '&' + paremeter;
				}

				PopUpWindow (Url,'Calendar', 325, 280, 0, 1);
			}
		}

		function CnvString( cStr ){
			var nLen = cStr.length;
			var cNew = "";
			for (count=0; count < nLen; count++) {
				cTemp=cStr.substring(count, count+1);
				if ( cTemp == " " ) {cNew = cNew + "%20";}
				else {cNew = cNew + cTemp;}
			}
			return cNew;
		}
		function IsValidData(){
			//if (!bContentLoaded) {
				//alert("Please wait until your content has completed the loading process before saving/submitting your content.");
				//return false;
//			}
	//		myEditor1copyValue();
			return true;
		}
		option = new Array (2);
//		option[0] = new Image(); option[0].src = "images/header_exit_on.gif";
//		option[1] = new Image(); option[1].src = "images/header_exit.gif";

	function emp_onError(form_object, input_object, object_value, error_message)
    {
	alert(error_message);
       	return false;
    }



function emp_checkdate(object_value)
    {
    //Returns true if value is a date format or is NULL
    //otherwise returns false

    if (object_value.length == 0)
        return true;

    //Returns true if value is a date in the mm/dd/yyyy format
	isplit = object_value.indexOf('/');

	if (isplit == -1 || isplit == object_value.length)
		return false;

    sMonth = object_value.substring(0, isplit);

	if (sMonth.length == 0)
        return false;

	isplit = object_value.indexOf('/', isplit + 1);

	if (isplit == -1 || (isplit + 1 ) == object_value.length)
		return false;

    sDay = object_value.substring((sMonth.length + 1), isplit);

	if (sDay.length == 0)
        return false;

	sYear = object_value.substring(isplit + 1);

	if (!emp_checkinteger(sMonth)) //check month
		return false;
	else
	if (!emp_checkrange(sMonth, 1, 12)) //check month
		return false;
	else
	if (!emp_checkinteger(sYear)) //check year
		return false;
	else
	if (!emp_checkrange(sYear, 0, 9999)) //check year
		return false;
	else
	if (!emp_checkinteger(sDay)) //check day
		return false;
	else
	if (!emp_checkday(sYear, sMonth, sDay)) // check day
		return false;
	else
		return true;
    }

function emp_checkday(checkYear, checkMonth, checkDay)
    {

	maxDay = 31;

	if (checkMonth == 4 || checkMonth == 6 ||
			checkMonth == 9 || checkMonth == 11)
		maxDay = 30;
	else
	if (checkMonth == 2)
	{
		if (checkYear % 4 > 0)
			maxDay =28;
		else
		if (checkYear % 100 == 0 && checkYear % 400 > 0)
			maxDay = 28;
		else
			maxDay = 29;
	}

	return emp_checkrange(checkDay, 1, maxDay); //check day
    }



function emp_checkinteger(object_value)
    {
    //Returns true if value is a number or is NULL
    //otherwise returns false

    if (object_value.length == 0)
        return true;

    //Returns true if value is an integer defined as
    //   having an optional leading + or -.
    //   otherwise containing only the characters 0-9.
	var decimal_format = ".";
	var check_char;

    //The first character can be + -  blank or a digit.
	check_char = object_value.indexOf(decimal_format)
    //Was it a decimal?
    if (check_char < 1)
	return emp_checknumber(object_value);
    else
	return false;
    }



function emp_numberrange(object_value, min_value, max_value)
    {
    // check minimum
    if (min_value != null)
	{
        if (object_value < min_value)
		return false;
	}

    // check maximum
    if (max_value != null)
	{
	if (object_value > max_value)
		return false;
	}

    //All tests passed, so...
    return true;
    }



function emp_checknumber(object_value)
    {
    //Returns true if value is a number or is NULL
    //otherwise returns false

    if (object_value.length == 0)
        return true;

    //Returns true if value is a number defined as
    //   having an optional leading + or -.
    //   having at most 1 decimal point.
    //   otherwise containing only the characters 0-9.
	var start_format = " .+-0123456789";
	var number_format = " .0123456789";
	var check_char;
	var decimal = false;
	var trailing_blank = false;
	var digits = false;

    //The first character can be + - .  blank or a digit.
	check_char = start_format.indexOf(object_value.charAt(0))
    //Was it a decimal?
	if (check_char == 1)
	    decimal = true;
	else if (check_char < 1)
		return false;

	//Remaining characters can be only . or a digit, but only one decimal.
	for (var i = 1; i < object_value.length; i++)
	{
		check_char = number_format.indexOf(object_value.charAt(i))
		if (check_char < 0)
			return false;
		else if (check_char == 1)
		{
			if (decimal)		// Second decimal.
				return false;
			else
				decimal = true;
		}
		else if (check_char == 0)
		{
			if (decimal || digits)
				trailing_blank = true;
        // ignore leading blanks

		}
	        else if (trailing_blank)
			return false;
		else
			digits = true;
	}
    //All tests passed, so...
    return true
    }

function emp_checkrange(object_value, min_value, max_value)
    {
    //if value is in range then return true else return false

    if (object_value.length == 0)
        return true;


    if (!emp_checknumber(object_value))
	{
	return false;
	}
    else
	{
	return (emp_numberrange((eval(object_value)), min_value, max_value));
	}

    //All tests passed, so...
    return true;
    }



function emp_checktime(object_value)
{
	//Returns true if value is in time format or is NULL
	//otherwise returns false

	if (object_value.length == 0)
		return true;

	isplit = object_value.indexOf(':');

	if (isplit == -1 || isplit == object_value.length)
		return false;

  sHour = object_value.substring(0, isplit);
	iminute = object_value.indexOf(':', isplit + 1);

	if (iminute == -1 || iminute == object_value.length)
		sMin = object_value.substring((sHour.length + 1));
	else
		sMin = object_value.substring((sHour.length + 1), iminute);

    if (!emp_checkinteger(sHour)) //check hour
		return false;
    else
    if (!emp_checkrange(sHour, 0, 12)) //check hour
		return false;

	if ((sMin.length == 0) || (!emp_checkinteger(sMin))) //check minutes
		return false;
	else
	if (!emp_checkrange(sMin, 0, 59)) // check minutes
		return false;

	// did they specify seconds
    if (iminute != -1)
	{
		sSec = object_value.substring(iminute + 1);

		if ((sSec.length == 0) || (!emp_checkinteger(sSec))) //check seconds
			return false;
		else
		if (!emp_checkrange(sSec, 0, 59)) //check seconds
			return false;
	}
  return true;
 }


function emp_checkampm(time, ampm)
	{
	//returns true if AM or PM, else returns false
	if (time == '')
		return true
	else if (ampm == 'AM' || ampm == 'PM')
		return true;
	else
		return false;
	}


function RemoveLeadingSpaces(date) {
	while(date.substring(0,1) == " ") {
		date = date.substring(1, date.length);
	}
	return date;
}

function CompareDates(earlyDate, laterDate) {
	if (earlyDate == "")
	{
		return true;
	}
	if (laterDate == "")
	{
		return true;
	}
	var earlyDateTime = earlyDate.split(" ");
	var laterDateTime = laterDate.split(" ");

	var earlyDateArray = earlyDateTime[0].split("-");
	var laterDateArray = laterDateTime[0].split("-");

	// check year
	if (earlyDateArray[2] < laterDateArray[2]) {
		return true;
	}
	if (earlyDateArray[2] > laterDateArray[2]) {
		return false;
	}

	// check month
	var myMonths = ecmMonths.split(",");
	var earlyMonth = 0;
	for(var iLoop = 0; iLoop < myMonths.length; iLoop++) {
		if (earlyDateArray[1].toLowerCase() == myMonths[iLoop].toLowerCase()) {
			earlyMonth = iLoop;
			break;
		}
	}
	var laterMonth = 0;
	for(var iLoop = 0; iLoop < myMonths.length; iLoop++) {
		if (laterDateArray[1].toLowerCase() == myMonths[iLoop].toLowerCase()) {
			laterMonth = iLoop;
			break;
		}
	}
	if ((earlyMonth < laterMonth)) {
		return true;
	}
	if ((earlyMonth > laterMonth)) {
		return false;
	}

	// check day
	if (earlyDateArray[0].length < 2) {
		earlyDateArray[0] = "0" + earlyDateArray[0];
	}
	if (laterDateArray[0].length < 2) {
		laterDateArray[0] = "0" + laterDateArray[0];
	}
	if (earlyDateArray[0] < laterDateArray[0]) {
		return true;
	}
	if (earlyDateArray[0] > laterDateArray[0]) {
		return false;
	}
	return false;
}

function CompareDatesOnly(earlyDate, laterDate) {
	if (earlyDate == "")
	{
		return true;
	}
	if (laterDate == "")
	{
		return true;
	}
	var earlyDateTime = earlyDate.split(" ");
	var laterDateTime = laterDate.split(" ");

	var earlyDateArray = earlyDateTime[0].split("-");
	var laterDateArray = laterDateTime[0].split("-");

	// check year
	if (earlyDateArray[2] < laterDateArray[2]) {
		return true;
	}
	if (earlyDateArray[2] > laterDateArray[2]) {
		return false;
	}

	// check month
	var myMonths = ecmMonths.split(",");
	var earlyMonth = 0;
	for(var iLoop = 0; iLoop < myMonths.length; iLoop++) {
		if (earlyDateArray[1].toLowerCase() == myMonths[iLoop].toLowerCase()) {
			earlyMonth = iLoop;
			break;
		}
	}
	var laterMonth = 0;
	for(var iLoop = 0; iLoop < myMonths.length; iLoop++) {
		if (laterDateArray[1].toLowerCase() == myMonths[iLoop].toLowerCase()) {
			laterMonth = iLoop;
			break;
		}
	}
	if ((earlyMonth < laterMonth)) {
		return true;
	}
	if ((earlyMonth > laterMonth)) {
		return false;
	}

	// check day
	if (earlyDateArray[0].length < 2) {
		earlyDateArray[0] = "0" + earlyDateArray[0];
	}
	if (laterDateArray[0].length < 2) {
		laterDateArray[0] = "0" + laterDateArray[0];
	}
	if (earlyDateArray[0] < laterDateArray[0]) {
		return true;
	}
	if (earlyDateArray[0] > laterDateArray[0]) {
		return false;
	}
}


function CheckDateOnly(date) {
	if (date == "")
	{
		return true;
	}
	date = RemoveLeadingSpaces(date);
	var date_time = date.split(" ");
	if (date_time.length == 0) {
		//var msg = invalidDateOnlyFormatMsg;
		//alert(msg);
		return false;
	}
	var mydate = date_time[0].split("-");
	if (mydate.length != 3) {
		//var msg = invalidDateOnlyFormatMsg;
		//alert(msg);
		return false;
	}
	if ((mydate[2] < 1970) || (mydate[2] > 2039)) {
		//var msg = invalidYearMsg;
		//alert(msg);
		return false;
	}

	var regexp1 = / /gi;
	mydate[1] = mydate[1].replace(regexp1, "");
	var MyMonths = ecmMonths.split(",");
	var ecmMonth = 0;
	for(var iLoop = 0; iLoop < MyMonths.length; iLoop++) {
		if (mydate[1].toLowerCase() == MyMonths[iLoop].toLowerCase()) {
			ecmMonth = iLoop;
			break;
		}
	}
	if (ecmMonth == 0) {
		var msg = invalidMonthMsg;
		alert(msg);
		return false;
	}
	if( (!mydate[0].length) || (!emp_checkday(mydate[2], ecmMonth, mydate[0]))) {
		var msg = invalidDayMsg;
		alert(msg);
		return false;
	}
	return true;
}

function convertMonthAbbrevToNumber(inMonthAbbrev) {
	var cvMonths = ecmMonths.split(',') ;
	for(var iLoop = 1; iLoop < cvMonths.length; iLoop++) {
		if (cvMonths[iLoop].toLowerCase() == inMonthAbbrev.toLowerCase()) {
		 	return iLoop - 1;
		}
	}
	return -1 ;
}

function checkTimeOnly(inTimeStr) {
	// Verify Valid Time Format
	if(inTimeStr.match(/^((0?[1-9]|1[012])(:[0-5]\d){0,2}(\ [AP]M))$|^([01]\d|2[0-3])(:[0-5]\d){1,2}(\ [AP]M)$/i)) { return true ; }
	else { alert('Times must be in either of the following formats:\n"hh:mm:ss am", "hh:mm am" or "24:mm:ss"') ; return false ; }
}

function compareDateAndTime(earlyDate, lateDate) {
	var tStr = earlyDate ;
	var sSpl = tStr.split(' ') ;
	var dSpl = sSpl[0].split('-') ;
	var tSpl = sSpl[1].split(':') ;
	if ((sSpl[2].toLowerCase()=='pm')&&(tSpl[0]!='12')) {
		tSpl[0] = parseInt(tSpl[0]) + 12 ;
	}
	if ((sSpl[2].toLowerCase()=='am')&&(tSpl[0]=='12')) {
		tSpl[0] = 0 ;
	}
	var eDt = new Date(dSpl[2],convertMonthAbbrevToNumber(dSpl[1]),dSpl[0],tSpl[0],tSpl[1],tSpl[2]) ;

	tStr = lateDate ;
	sSpl = tStr.split(' ') ;
	dSpl = sSpl[0].split('-') ;
	tSpl = sSpl[1].split(':') ;
	if ((sSpl[2].toLowerCase()=='pm')&&(tSpl[0]!='12')) {
		tSpl[0] = parseInt(tSpl[0]) + 12 ;
	}
	if ((sSpl[2].toLowerCase()=='am')&&(tSpl[0]=='12')) {
		tSpl[0] = 0 ;
	}
	var lDt = new Date(dSpl[2],convertMonthAbbrevToNumber(dSpl[1]),dSpl[0],tSpl[0],tSpl[1],tSpl[2]) ;

	if (lDt<eDt){ return false ; }
		else { return true ; }

}