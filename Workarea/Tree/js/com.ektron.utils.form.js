//////////
//
// name: FormUtil
// desc: This static class acts as a utility for manipulating form elements
// auth: william.cava@ektron.com
// date: July 8, 2005
//
var FormUtil = 
{
	//////////
	//
	// name: addOptionToList
	// desc: Given the input parameters, this function creates a
	//       new option element and adds it to the given select
	//       list. If an optional boolean "selected" parameter is
	//       passed, it is used to determine whether the new element
	//       is selected or not.
	//

	addOptionToList: function( selectList, name, value, selected )
	{
		selected = selected == true ? true : false;
		var options = selectList.options;
		var option = new Option( name, value )
		option.selected = selected;
		options[options.length] = option;
		
		if( selected ) {
			FormUtil.selectOptionByValue( selectList, optionValue );
		}
	},

	//////////
	//
	// name: getOptionByValue
	// desc: Given a selectList and an option value, this function
	//       returns the option element with the provided value
	//
		
	getOptionByValue: function( selectList, value )
	{
		return FormUtil.getOptionByField( selectList, "value", value );
	},

	//////////
	//
	// name: getOptionByField
	// desc: Given a selectList and a field name/value, this function
	//       returns the option element that matches the criteria
	//
		
	getOptionByField: function( selectList, fieldName, fieldValue )
	{
		var options = selectList.options;
		var returnValue = null;
		for( var i = 0; i < options.length; i++ ) {
			var option = options[i];
			if( option[fieldName] == fieldValue ) {
				returnValue = option;
				break;
			}
		}
		
		return returnValue;
	},

	//////////
	//
	// name: removeOptionByValue
	// desc: Given a selectList and a value, this function searches
	//       for the child option in the list and once it is found,
	//       removes it from the list.
	//

	removeOptionByValue: function( selectList, optionValue )
	{
		var options = selectList.options;
		for( var i = 0; i < options.length; i++ ) {
			var option = options[i];
			if( option.value == optionValue ) {
				selectList.removeChild( option );
			}
		}
	},

	//////////
	//
	// name: selectOptionByName
	// desc: Given an 
	//

	selectOptionByName: function( selectList, optionName )
	{
		FormUtil.selectOptionByField( selectList, "name", optionName );
	},
	
	//////////
	//
	// name: selectOptionByValue
	// desc: Given an 
	//

	selectOptionByValue: function( selectList, optionValue )
	{
		FormUtil.selectOptionByField( selectList, "value", optionValue );
	},
	
	//////////
	//
	// name: selectOptionByField
	// desc: Given an 
	//

	selectOptionByField: function( selectList, fieldName, fieldValue )
	{
		var options = selectList.options;
		for( var i = 0; i < options.length; i++ ) {
			var option = options[i];
			option.selected = false;
			if( option[fieldName] == fieldValue ) {
				option.selected = true;
			}
		}
	}
}