//////////
//
// name: Vector
// desc: Provides an abstract datatype vector
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function Vector()
{
	this.add			= __Vector_add;
	this.clear			= __Vector_clear;
	this.contains		= __Vector_contains;
	this.elementAt		= __Vector_elementAt;
	this.getSize		= __Vector_getSize;
	this.toString		= __Vector_toString;
	this.remove			= __Vector_remove;
	this.isEmpty		= __Vector_isEmpty;
	
	//////////
	//
	// private member variables
	//
	
	this.data = new Array();
}

//////////
//
// name: add
// desc: Appends the specified element to the end of this Vector.
// 

function __Vector_add( item )
{
	this.data[this.data.length] = item;
}

//////////
//
// name: clear
// desc: Removes all of the elements from this Vector
// 

function __Vector_clear()
{
	this.data = new Array();
}

//////////
//
// name: elementAt
// desc: Returns the component at the specified index
// 

function __Vector_elementAt( index )
{
	var value = null;
	if( index < this.data.index ) {
		value = this.data[index];
	}
	
	return value;
}

//////////
//
// name: getSize
// desc: Returns the number of components in this vector
// 

function __Vector_getSize()
{
	return this.data.length;
}

//////////
//
// name: toString
// desc: Returns a string representation of this Vector,
//       containing the String representation of each element.
// 

function __Vector_toString()
{
	var buf = "";
	
	for( var i = 0; i < this.data.length; i++ ) {
		buf += "this[" + i + "]=" + this.data[i] + " ";
	}
	
	return buf;
}

//////////
//
// name: isEmpty
// desc: Tests if this vector has no components
// 

function __Vector_isEmpty()
{
	var length = 0;

	if( this.data ) {
		length = this.data.length;
	}

	return length;
}

//////////
//
// name: contains
// desc: Tests if the specified object is a component in this vector
// 

function __Vector_contains( value )
{
	var contains = false;
	for( var i = 0; i < this.data.length; i++ ) {
		if( this.data[i] == value ) {
			return true;
			break;
		}
	}
}

//////////
//
// name: remove
// desc: Removes the first occurrence of the specified element in this
//       Vector If the Vector does not contain the element, it is unchanged.
// 

function __Vector_remove( index )
{
	// todo
	throw "not implemeted";
}

