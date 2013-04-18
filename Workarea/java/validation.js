var whitespace = " \t\n\r";
var reAlphabetic = /^[a-z A-Z]+$/;
var reText = /^[^0-9]+$/;
var reInteger = /^\d+$/;
var digitsInSocialSecurityNumber = 9;
//var daysInMonth = makeArray[12];
var daysInMonth=new Array ("31","29","31","30","31","30","31","31","30","31","30","31");


// Valid U.S. Postal Codes for states, territories, armed forces, etc.
// See http://www.usps.gov/ncsc/lookups/abbr_state.txt.

var USStateCodeDelimiter = "|";
var USStateCodes = "AL|AK|AS|AZ|AR|CA|CO|CT|DE|DC|FM|FL|GA|GU|HI|ID|IL|IN|IA|KS|KY|LA|ME|MH|MD|MA|MI|MN|MS|MO|MT|NE|NV|NH|NJ|NM|NY|NC|ND|MP|OH|OK|OR|PW|PA|PR|RI|SC|SD|TN|TX|UT|VT|VI|VA|WA|WV|WI|WY|AE|AA|AE|AE|AP"

// U.S. ZIP codes have 5 or 9 digits.
// They are formatted as 12345 or 12345-6789.
var digitsInZIPCode1 = 5
var digitsInZIPCode2 = 9

var defaultEmptyOK = false

//VALIDATION TYPE ID:0
function isNone(strData){
	return true;
}

//VALIDATION TYPE ID:1
function isBlank(strData){
	if(strData=="") return false
	else return true;
}

function isSelectOptionNotZero(selectedIndex){
	if(selectedIndex==0) return false
	else return true;
}


//VALIDATION TYPE ID:2
function isDigit(strData) {
	if(strData=="") return false;
	//if (strData==null) return false;
	for (var iCtr = 0; iCtr < strData.length; iCtr++) {
		cDigit = strData.charAt(iCtr);
		
	    if ((cDigit < "0") || (cDigit > "9"))
				return false;
	}
	return true;
}

function  isNumeric( strValue ) {
/******************************************************************************
DESCRIPTION: Validates that a string contains only valid numbers.

PARAMETERS:
   strValue - String to be tested for validity

RETURNS:
   True if valid, otherwise false.
******************************************************************************/
  var objRegExp  =  /(^-?\d\d*\.\d*$)|(^-?\d\d*$)|(^-?\.\d\d*$)/;

  //check for numeric characters
  return objRegExp.test(strValue);
}

//VALIDATION TYPE ID:3
function isText(strData){
	return(isTextOnly(strData));
}

//VALIDATION TYPE ID:4
function isDate( strValue ) {
/************************************************
DESCRIPTION: Validates that a string contains only
    valid dates with 2 digit month, 2 digit day,
    4 digit year. Date separator can be ., -, or /.
    Uses combination of regular expressions and
    string parsing to validate date.
    Ex. mm/dd/yyyy or mm-dd-yyyy or mm.dd.yyyy

PARAMETERS:
   strValue - String to be tested for validity

RETURNS:
   True if valid, otherwise false.

REMARKS:
   Avoids some of the limitations of the Date.parse()
   method such as the date separator character.
*************************************************/
  var objRegExp = /^\d{1,2}(\/)\d{1,2}\1\d{4}$/

  //check to see if in correct format
  if(!objRegExp.test(strValue))
    return false; //doesn't match pattern, bad date
  else{
    var strSeparator='';
    for(var i=0;i<strValue.length;i++){
        strSeparator=strValue.charAt(i);
        if(isNaN(strSeparator)){
            break;
        }
    }
    var arrayDate = strValue.split(strSeparator); //split date into month, day, year
    var intDay = parseInt(arrayDate[1],10);
	var intYear = parseInt(arrayDate[2],10);
    var intMonth = parseInt(arrayDate[0],10);
    
    if (intMonth > 12) { return false; } //if month is invalid quit    
    //create a lookup for months not equal to Feb.
    var arrayLookup = { '01' : 31,'03' : 31, '04' : 30,'05' : 31,'06' : 30,'07' : 31,
                        '08' : 31,'09' : 30,'10' : 31,'11' : 30,'12' : 31}
    if (parseInt(intMonth) < 10)
    {
        intMonth = '0' + intMonth;
    }
    
    //check if month value and day value agree
    if(arrayLookup[intMonth] != null) {
      if(intDay <= arrayLookup[intMonth] && intDay != 0)
        return true; //found in lookup table, good date
    }    
    //check for February    
    if( ((intYear % 4 == 0 && intDay <= 29) || (intYear % 4 != 0 && intDay <=28)) && intDay !=0)
      return true; //Feb. had valid number of days
  }
  return false; //any other values, bad date
}


//VALIDATION TYPE ID:5
function isEmail(strData) {
	var	iCtr,jCtr,sLength,atPos,cs,cTemp;

	varErrMsg="Invalid email address detected!";
	if (isEmpty(strData))
		if (isEmail.arguments.length == 1)
			return false;
		else
			return (isEmail.arguments[1] == true);

		   
    if (isWhitespace(strData))
		return false;
		    
    iCtr = 1;
    sLength = strData.length;

    iCtr = strData.indexOf("@");
    atPos = iCtr;
		    
    if ( iCtr < 0 )
		return false;
	else 
		iCtr+=2;
	
	iCtr = strData.lastIndexOf(".");
    if ( iCtr < 0 )
		return false;
	else 
		iCtr++;			
			    
    if (iCtr > sLength)
		return false;
			
	cTemp	= "";
	cs		= "";
	for(jCtr = atPos+1; jCtr < strData.length; jCtr++) {
		cTemp =  strData.charAt(jCtr);
		if( (cTemp != ".") && (cTemp != "-") )
			cs += cTemp;
	}
			
	iCtr = strData.lastIndexOf( " " );
	if ( iCtr > 0 )
		return false;
	else 
		iCtr++;	
	for(i=0;i<strData.length;i++)
	{
		cTemp =  strData.charAt(i);
		if((cTemp=="?")||(cTemp=="(")||(cTemp==")")||(cTemp=="=")||(cTemp=="+")||(cTemp=="~")||(cTemp=="`")||(cTemp=="!")||(cTemp=="#")||(cTemp=="$")||(cTemp=="%")||(cTemp=="^")||(cTemp=="&")||(cTemp=="*"))
		{
			return false;
		}
	}
	return true;
}

//VALIDATION TYPE ID:6
function isCreditCard(ccNumber) {
   var ccDigits = extractDigits(ccNumber);
   if (checkMod10(ccDigits) && validCardType(ccDigits)) {
      return 1;
   } else {
     return 0; 
   }
}



//SUB FUNCTIONS WHICH IS USED BY ONE OR MORE MAIN FUNCTIONS...
function isEmpty(strData) {
	return ((strData == null) || (strData.length == 0));
}

function isWhitespace(strData) {
	var iCtr,cTemp;
    if (isEmpty(strData))
		return true;
    for (iCtr = 0; iCtr < strData.length; iCtr++) {   
        var cTemp = strData.charAt(iCtr);
		if (whitespace.indexOf(cTemp) == -1)
			return false;
    }
    return true;
}

function validCardType(ccNumber) {
   var cardLengths = new Array (
         'v', 13, 'v', 16, 'm', 16,
         'a', 15, 'c', 14, 'd', 16);
   var cardDigits = new Array (
         'v', '4', 'm', '51', 'm', '52', 'm', '53' ,
         'm', '54', 'm', '55', 'a', '34', 'a', '37',
         'c', '300', 'c', '301', 'c', '302', 'c', '303',
         'c', '304', 'c', '305', 'c', '36', 'c', '38',
         'd', '6011');
   var validCard = false;
   var correctLength = false;
   var cardType = '' ;
   for (var i = 0; i < cardDigits.length - 1; i += 2) {
      if (cardDigits[i + 1] == ccNumber.substr(0, cardDigits[i + 1].length)) {
         validCard = true;
         cardType = cardDigits[i];
         break;
      }
   }
   if (validCard) {
      var cardLen = ccNumber.length;
      for (var i = 0; i < cardLengths.length - 1; i += 2) {
         if ((cardType == cardLengths[i]) && (cardLen == cardLengths[i + 1])) {
            correctLength = true;
            break;
         }
      }
      validCard = correctLength;
   }
   return validCard;
}

function checkMod10(ccNumber) {
   var translateMap = '0246813579';
   var digitSum = 0;
   var translateFlag = ((ccNumber.length % 2) == 0);
   for (var i = 0; i < ccNumber.length; i++) {
       digitSum += parseInt(translateFlag ?
            translateMap.charAt(ccNumber.charAt(i)) :
            ccNumber.charAt(i) , 10)
      translateFlag = !translateFlag;
   }
   return (digitSum % 10) == 0;
}

function extractDigits(mixedString) {
   var digitsOnly = '';
   var thisDigit = '';
   for (var i = 0; i < mixedString.length; i++) {
      thisDigit = mixedString.charAt(i);
      if (thisDigit >= '0' && thisDigit <= '9')
         digitsOnly = digitsOnly + thisDigit;
   }
   return digitsOnly;
}

function ccExpire(ccDate){
	var date=new Date();
	var currmm=date.getMonth();
	var curryr=date.getYear();
	var arr_date=ccDate.split("/")
	if (arr_date.length!=2){
		return false;
	}

	if ((arr_date[0].length!=2)||(arr_date[1].length!=2)||(isDigit(arr_date[0])==false) || (isDigit(arr_date[1])==false) || (parseInt(arr_date[0],10)>12) || (parseInt(arr_date[1],10)>99)||(parseInt('20'+arr_date[1],10)<parseInt(curryr,10))||((parseInt(arr_date[0],10)<parseInt(currmm,10))&&(parseInt('20'+arr_date[1],10)<=parseInt(curryr,10))) ){
		return false;
	}
	return true;
}
function isAlphabetic (s)

{     
	return reAlphabetic.test(s)
   
}
function isTextOnly (s)
{
	return reText.test(s)
}
function daysInFebruary (year)
{   
    return (  ((year % 4 == 0) && ( (!(year % 100 == 0)) || (year % 400 == 0) ) ) ? 29 : 28 );
}

function isValidNumCharacters(strData,minVal,maxVal){
	var bMin = false;
	var bMax = false;
	var bRet = false;
	var bRetMin = false;
	var bRetMax = false;
	if ((typeof minVal != "undefined") && (minVal != "")) {	bMin = true; }
	if ((typeof maxVal != "undefined") && (maxVal != "")) {	bMax = true; }
	if (bMin) {
		if (strData.length >= minVal) { bRetMin = true; }
	}
	else {
		bRetMin = true;
	}
	if (bMax) {
		if (strData.length<=maxVal) { bRetMax = true; }
	}
	else {
		bRetMax = true;
	}
	
	bRet = bRetMin && bRetMax;
	
	return bRet;
}


function isRange(strData,minVal,maxVal){
	
	if(isNumeric(minVal) && isNumeric(maxVal)){
		if( (strData-minVal) >= 0 && (maxVal-strData) >= 0 ) return true;
		else return false;
	}
	if(isNumeric(minVal)){
		if((strData-minVal) >= 0) return true;
		else return false;
	}
	if(isNumeric(maxVal)){
		if((maxVal-strData) >= 0 ) return true;
		else return false;
	}
}

function ConditionDate(dDate)
{
    var a = dDate.split("/");
    for( var i = 0; i < a.length; i++ ) {
        if (a[i].length == 4) // year
        {
            //do nothing
        }
        else if (a[i].length == 1) // day/month
        {
            a[i] = '0' + a[i];
        }
    }
    dDate = a[0] + '/' + a[1] + '/' + a[2];
    return dDate;
}


function isDateRange(strData,minVal,maxVal){
    strData = ConditionDate(strData);
    minVal = ConditionDate(minVal);
    maxVal = ConditionDate(maxVal);
	if(isDate(minVal) && isDate(maxVal)){
		if((compareDates(strData,minVal)==0)||(compareDates(strData,minVal)==1) && ((compareDates(strData,maxVal)==0)||compareDates(strData,maxVal)==-1)) return true;
		else return false;
	}
	if(isDate(minVal)){
		if((compareDates(strData,minVal)==0)||(compareDates(strData,minVal)==1)) return true;
		else return false;
	}
	if(isDate(maxVal)){
		if((compareDates(strData,maxVal)==0)||(compareDates(strData,maxVal)==-1)) return true;
		else return false;
	}
}
function compareDates (value1, value2) {
   var date1, date2;
   var month1, month2;
   var year1, year2;

   month1 = value1.substring (0, value1.indexOf ("/"));
   date1 = value1.substring (value1.indexOf ("/")+1, value1.lastIndexOf ("/"));
   year1 = value1.substring (value1.lastIndexOf ("/")+1, value1.length);

   month2 = value2.substring (0, value2.indexOf ("/"));
   date2 = value2.substring (value2.indexOf ("/")+1, value2.lastIndexOf ("/"));
   year2 = value2.substring (value2.lastIndexOf ("/")+1, value2.length);
	if (date1.length == 1)
	{
		date1 = "0" + date1;
	}
	if (date2.length == 1)
	{
		date2 = "0" + date2;
	}
   if (year1 > year2) return 1;
   else if (year1 < year2) return -1;
   else if (month1 > month2) return 1;
   else if (month1 < month2) return -1;
   else if (date1 > date2) return 1;
   else if (date1 < date2) return -1;
   else return 0;
} 

// isStateCode (STRING s [, BOOLEAN emptyOK])
// 
// Return true if s is a valid U.S. Postal Code 
// (abbreviation for state).
//
// For explanation of optional argument emptyOK,
// see comments of function isInteger.

function isStateCode(s)
{   
	s = s.toUpperCase();
	if (isEmpty(s)) {
		if (isStateCode.arguments.length == 1){  return defaultEmptyOK; }
	}
    else {		
		return ( (USStateCodes.indexOf(s) != -1) &&
			(s.indexOf(USStateCodeDelimiter) == -1) );		
	}
}


function isZIPCode( strValue ) {
/************************************************
DESCRIPTION: Validates that a string a United
  States zip code in 5 digit format or zip+4
  format. 99999 or 99999-9999

PARAMETERS:
   strValue - String to be tested for validity

RETURNS:
   True if valid, otherwise false.

*************************************************/
var objRegExp  = /(^\d{5}$)|(^\d{5}-\d{4}$)/;

  //check for valid US Zipcode
  return objRegExp.test(strValue);
}


 
// Declaring required variables
var digits = "0123456789";
// non-digit characters which are allowed in phone numbers
var phoneNumberDelimiters = "()- ";
// characters which are allowed in international phone numbers
// (a leading + is OK)
var validWorldPhoneChars = phoneNumberDelimiters + "+";
// Minimum no of digits in an international phone no.
var minDigitsInIPhoneNumber = 10;

function isInteger(s)
{   var i;
    for (i = 0; i < s.length; i++)
    {   
        // Check that current character is number.
        var c = s.charAt(i);
        if (((c < "0") || (c > "9"))) return false;
    }
    // All characters are numbers.
    return true;
}

function stripCharsInBag(s, bag)
{   var i;
    var returnString = "";
    // Search through string's characters one by one.
    // If character is not in bag, append to returnString.
    for (i = 0; i < s.length; i++)
    {   
        // Check that current character isn't whitespace.
        var c = s.charAt(i);
        if (bag.indexOf(c) == -1) returnString += c;
    }
    return returnString;
}

function checkInternationalPhone(strPhone){
s=stripCharsInBag(strPhone,validWorldPhoneChars);
return (isInteger(s) && s.length >= minDigitsInIPhoneNumber);
}

function isValidateUSPhone( strValue ){
	
	
	if ((strValue==null)||(strValue =="")){
		return false
	}
	if (checkInternationalPhone(strValue)==false){
		return false
	}
	return true
 }

//VALIDATION TYPE ID:7
// function isSSN(strData){
//	return (reInteger.test(strData) && strData.length == digitsInSocialSecurityNumber)
// }

function isSSN( strValue ) {
/************************************************
DESCRIPTION: Validates that a string a Social Security Number
  format. 999-99-9999 or999999999
PARAMETERS:
   strValue - String to be tested for validity
RETURNS:
   True if valid, otherwise false.
*************************************************/
var objRegExp  = /^\d{3}\-?\d{2}\-?\d{4}$/;
  return objRegExp.test(strValue);
}

function isCanadianpostalcode( strValue ) {
/************************************************
DESCRIPTION: Validates that a string a Canadian Postal Code
  format.  Z5Z-5Z5 or Z5Z5Z5
PARAMETERS:
   strValue - String to be tested for validity
RETURNS:
   True if valid, otherwise false.
*************************************************/
var objRegExp  = /^\D{1}\d{1}\D{1}\-?\d{1}\D{1}\d{1}$/;
  return objRegExp.test(strValue);
}

function isTime( strValue ) {
/************************************************
DESCRIPTION: Validates that a string a Time
  format.   HH:MM or HH:MM:SS or HH:MM:SS.mmm
PARAMETERS:
   strValue - String to be tested for validity
RETURNS:
   True if valid, otherwise false.
*************************************************/
var objRegExp  = /^([1-9]|1[0-2]):[0-5]\d(:[0-5]\d(\.\d{1,3})?)?$/;
  return objRegExp.test(strValue);
}




function isIPAddress(strValue) {
/************************************************
DESCRIPTION: Validates that a string is IP address
  format.  999.999.999.999
PARAMETERS:
   strValue - String to be tested for validity
RETURNS:
   True if valid, otherwise false.
*************************************************/
var objRegExp  = /^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$/;
  return objRegExp.test(strValue);
}

function isDollarAmount(strValue) {
/************************************************
DESCRIPTION: Validates that a string is a Dollar amount
  format.  100, 100.00, $100 or $100.00
PARAMETERS:
   strValue - String to be tested for validity
RETURNS:
   True if valid, otherwise false.
*************************************************/
var objRegExp  = /^((\$\d*)|(\$\d*\.\d{2})|(\d*)|(\d*\.\d{2}))$/;
	if (isEmpty(strValue)) { return false; }
	else { return objRegExp.test(strValue); }
}

function isCanadianSocialInsuranceNumber(strValue) {
/************************************************
DESCRIPTION: Validates that a string is a Canadian Social Insurance Number
  format.  999999999
PARAMETERS:
   strValue - String to be tested for validity
RETURNS:
   True if valid, otherwise false.
*************************************************/
var objRegExp  = /^\d{9}$/;
  return objRegExp.test(strValue);
}



