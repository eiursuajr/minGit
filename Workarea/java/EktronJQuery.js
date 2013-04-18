if ("undefined" == typeof $ektron)
{
	(function(){
    /*
        Ektron JavaScript Library
        Copyright (c) 2008 Ektron, Inc.
        All rights reserved

        Instructions to upgrade this Ektron library from jQuery
        (These instructions also exist as a Word document w/ pictures)

        1. Add this comment
        2. Add "Based on" to the jQuery comment just below
        3. Remove jQuery $Date and $Rev comments
        4. Remove the 2 sections w/ comments that start w/ Map
        5. Remove the noConflict function
        6. Replace '"jQuery"' with '"ektronjs"' (including double quotes!)
        7. Replace CASE-SENSITIVE 'jQuery' with '$ektron' (w/o quotes)
        8. Replace '$(' with '$ektron(' (w/o quotes)
        9. Remove declaration of window.$
        10. Add the Ektron stuff at the bottom
    */
	/*
	 * Based on jQuery 1.2.6 - New Wave Javascript
	 *
	 * Copyright (c) 2008 John Resig (jquery.com)
	 * Dual licensed under the MIT (MIT-LICENSE.txt)
	 * and GPL (GPL-LICENSE.txt) licenses.
	 *
	 */

	var $ektron = window.$ektron = function( selector, context ) {
		// The $ektron object is actually just the init constructor 'enhanced'
		return new $ektron.fn.init( selector, context );
	};

	// A simple way to check for HTML strings or ID strings
	// (both of which we optimize for)
	var quickExpr = /^[^<]*(<(.|\s)+>)[^>]*$|^#(\w+)$/,

	// Is it a simple selector
		isSimple = /^.[^:#\[\.]*$/,

	// Will speed up references to undefined, and allows munging its name.
		undefined;

	$ektron.fn = $ektron.prototype = {
		init: function( selector, context ) {
			// Make sure that a selection was provided
			selector = selector || document;

			// Handle $ektron(DOMElement)
			if ( selector.nodeType ) {
				this[0] = selector;
				this.length = 1;
				return this;
			}
			// Handle HTML strings
			if ( typeof selector == "string" ) {
				// Are we dealing with HTML string or an ID?
				var match = quickExpr.exec( selector );

				// Verify a match, and that no context was specified for #id
				if ( match && (match[1] || !context) ) {

					// HANDLE: $ektron(html) -> $ektron(array)
					if ( match[1] )
						selector = $ektron.clean( [ match[1] ], context );

					// HANDLE: $ektron("#id")
					else {
						var elem = document.getElementById( match[3] );

						// Make sure an element was located
						if ( elem ){
							// Handle the case where IE and Opera return items
							// by name instead of ID
							if ( elem.id != match[3] )
								return $ektron().find( selector );

							// Otherwise, we inject the element directly into the $ektron object
							return $ektron( elem );
						}
						selector = [];
					}

				// HANDLE: $ektron(expr, [context])
				// (which is just equivalent to: $ektron(content).find(expr)
				} else
					return $ektron( context ).find( selector );

			// HANDLE: $ektron(function)
			// Shortcut for document ready
			} else if ( $ektron.isFunction( selector ) )
				return $ektron( document )[ $ektron.fn.ready ? "ready" : "load" ]( selector );

			return this.setArray($ektron.makeArray(selector));
		},

		// The current version of $ektron being used
		jquery: "1.2.6",

		// The number of elements contained in the matched element set
		size: function() {
			return this.length;
		},

		// The number of elements contained in the matched element set
		length: 0,

		// Get the Nth element in the matched element set OR
		// Get the whole matched element set as a clean array
		get: function( num ) {
			return num == undefined ?

				// Return a 'clean' array
				$ektron.makeArray( this ) :

				// Return just the object
				this[ num ];
		},

		// Take an array of elements and push it onto the stack
		// (returning the new matched element set)
		pushStack: function( elems ) {
			// Build a new $ektron matched element set
			var ret = $ektron( elems );

			// Add the old object onto the stack (as a reference)
			ret.prevObject = this;

			// Return the newly-formed element set
			return ret;
		},

		// Force the current matched set of elements to become
		// the specified array of elements (destroying the stack in the process)
		// You should use pushStack() in order to do this, but maintain the stack
		setArray: function( elems ) {
			// Resetting the length to 0, then using the native Array push
			// is a super-fast way to populate an object with array-like properties
			this.length = 0;
			Array.prototype.push.apply( this, elems );

			return this;
		},

		// Execute a callback for every element in the matched set.
		// (You can seed the arguments with an array of args, but this is
		// only used internally.)
		each: function( callback, args ) {
			return $ektron.each( this, callback, args );
		},

		// Determine the position of an element within
		// the matched set of elements
		index: function( elem ) {
			var ret = -1;

			// Locate the position of the desired element
			return $ektron.inArray(
				// If it receives a $ektron object, the first element is used
				elem && elem.jquery ? elem[0] : elem
			, this );
		},

		attr: function( name, value, type ) {
			var options = name;

			// Look for the case where we're accessing a style value
			if ( name.constructor == String )
				if ( value === undefined )
					// Ektron re-added "|| undefined" from 1.2.3 jQuery Ticket #3017
					return this[0] && $ektron[ type || "attr" ]( this[0], name ) || undefined;

				else {
					options = {};
					options[ name ] = value;
				}

			// Check to see if we're setting style values
			return this.each(function(i){
				// Set all the styles
				for ( name in options )
					$ektron.attr(
						type ?
							this.style :
							this,
						name, $ektron.prop( this, options[ name ], type, i, name )
					);
			});
		},

		css: function( key, value ) {
			// ignore negative width and height values
			if ( (key == 'width' || key == 'height') && parseFloat(value) < 0 )
				value = undefined;
			return this.attr( key, value, "curCSS" );
		},

		text: function( text ) {
			if ( typeof text != "object" && text != null )
				return this.empty().append( (this[0] && this[0].ownerDocument || document).createTextNode( text ) );

			var ret = "";

			$ektron.each( text || this, function(){
				$ektron.each( this.childNodes, function(){
					if ( this.nodeType != 8 )
						ret += this.nodeType != 1 ?
							this.nodeValue :
							$ektron.fn.text( [ this ] );
				});
			});

			return ret;
		},

		wrapAll: function( html ) {
			if ( this[0] )
				// The elements to wrap the target around
				$ektron( html, this[0].ownerDocument )
					.clone()
					.insertBefore( this[0] )
					.map(function(){
						var elem = this;

						while ( elem.firstChild )
							elem = elem.firstChild;

						return elem;
					})
					.append(this);

			return this;
		},

		wrapInner: function( html ) {
			return this.each(function(){
				$ektron( this ).contents().wrapAll( html );
			});
		},

		wrap: function( html ) {
			return this.each(function(){
				$ektron( this ).wrapAll( html );
			});
		},

		append: function() {
			return this.domManip(arguments, true, false, function(elem){
				if (this.nodeType == 1)
					this.appendChild( elem );
			});
		},

		prepend: function() {
			return this.domManip(arguments, true, true, function(elem){
				if (this.nodeType == 1)
					this.insertBefore( elem, this.firstChild );
			});
		},

		before: function() {
			return this.domManip(arguments, false, false, function(elem){
				this.parentNode.insertBefore( elem, this );
			});
		},

		after: function() {
			return this.domManip(arguments, false, true, function(elem){
				this.parentNode.insertBefore( elem, this.nextSibling );
			});
		},

		end: function() {
			return this.prevObject || $ektron( [] );
		},

		find: function( selector ) {
			var elems = $ektron.map(this, function(elem){
				return $ektron.find( selector, elem );
			});

			return this.pushStack( /[^+>] [^+>]/.test( selector ) || selector.indexOf("..") > -1 ?
				$ektron.unique( elems ) :
				elems );
		},

		clone: function( events ) {
			// Do the clone
			var ret = this.map(function(){
				if ( $ektron.browser.msie && !$ektron.isXMLDoc(this) ) {
					// IE copies events bound via attachEvent when
					// using cloneNode. Calling detachEvent on the
					// clone will also remove the events from the orignal
					// In order to get around this, we use innerHTML.
					// Unfortunately, this means some modifications to
					// attributes in IE that are actually only stored
					// as properties will not be copied (such as the
					// the name attribute on an input).
					var clone = this.cloneNode(true),
						container = document.createElement("div");
					container.appendChild(clone);
					return $ektron.clean([container.innerHTML])[0];
				} else
					return this.cloneNode(true);
			});

			// Need to set the expando to null on the cloned set if it exists
			// removeData doesn't work here, IE removes it from the original as well
			// this is primarily for IE but the data expando shouldn't be copied over in any browser
			var clone = ret.find("*").andSelf().each(function(){
				if ( this[ expando ] != undefined )
					this[ expando ] = null;
			});

			// Copy the events from the original to the clone
			if ( events === true )
				this.find("*").andSelf().each(function(i){
					if (this.nodeType == 3)
						return;
					var events = $ektron.data( this, "events" );

					for ( var type in events )
						for ( var handler in events[ type ] )
							$ektron.event.add( clone[ i ], type, events[ type ][ handler ], events[ type ][ handler ].data );
				});

			// Return the cloned set
			return ret;
		},

		filter: function( selector ) {
			return this.pushStack(
				$ektron.isFunction( selector ) &&
				$ektron.grep(this, function(elem, i){
					return selector.call( elem, i );
				}) ||

				$ektron.multiFilter( selector, this ) );
		},

		not: function( selector ) {
			if ( selector.constructor == String )
				// test special case where just one selector is passed in
				if ( isSimple.test( selector ) )
					return this.pushStack( $ektron.multiFilter( selector, this, true ) );
				else
					selector = $ektron.multiFilter( selector, this );

			var isArrayLike = selector.length && selector[selector.length - 1] !== undefined && !selector.nodeType;
			return this.filter(function() {
				return isArrayLike ? $ektron.inArray( this, selector ) < 0 : this != selector;
			});
		},

		add: function( selector ) {
			return this.pushStack( $ektron.unique( $ektron.merge(
				this.get(),
				typeof selector == 'string' ?
					$ektron( selector ) :
					$ektron.makeArray( selector )
			)));
		},

		is: function( selector ) {
			return !!selector && $ektron.multiFilter( selector, this ).length > 0;
		},

		hasClass: function( selector ) {
			return this.is( "." + selector );
		},

		val: function( value ) {
			if ( value == undefined ) {

				if ( this.length ) {
					var elem = this[0];

					// We need to handle select boxes special
					if ( $ektron.nodeName( elem, "select" ) ) {
						var index = elem.selectedIndex,
							values = [],
							options = elem.options,
							one = elem.type == "select-one";

						// Nothing was selected
						if ( index < 0 )
							return null;

						// Loop through all the selected options
						for ( var i = one ? index : 0, max = one ? index + 1 : options.length; i < max; i++ ) {
							var option = options[ i ];

							if ( option.selected ) {
								// Get the specifc value for the option
								value = $ektron.browser.msie && !option.attributes.value.specified ? option.text : option.value;

								// We don't need an array for one selects
								if ( one )
									return value;

								// Multi-Selects return an array
								values.push( value );
							}
						}

						return values;

					// Everything else, we just grab the value
					} else
						return (this[0].value || "").replace(/\r/g, "");

				}

				return undefined;
			}

			if( value.constructor == Number )
				value += '';

			return this.each(function(){
				if ( this.nodeType != 1 )
					return;

				if ( value.constructor == Array && /radio|checkbox/.test( this.type ) )
					this.checked = ($ektron.inArray(this.value, value) >= 0 ||
						$ektron.inArray(this.name, value) >= 0);

				else if ( $ektron.nodeName( this, "select" ) ) {
					var values = $ektron.makeArray(value);

					$ektron( "option", this ).each(function(){
						this.selected = ($ektron.inArray( this.value, values ) >= 0 ||
							$ektron.inArray( this.text, values ) >= 0);
					});

					if ( !values.length )
						this.selectedIndex = -1;

				} else
					this.value = value;
			});
		},

		html: function( value ) {
			return value == undefined ?
				(this[0] ?
					this[0].innerHTML :
					null) :
				this.empty().append( value );
		},

		replaceWith: function( value ) {
			return this.after( value ).remove();
		},

		eq: function( i ) {
			return this.slice( i, i + 1 );
		},

		slice: function() {
			return this.pushStack( Array.prototype.slice.apply( this, arguments ) );
		},

		map: function( callback ) {
			return this.pushStack( $ektron.map(this, function(elem, i){
				return callback.call( elem, i, elem );
			}));
		},

		andSelf: function() {
			return this.add( this.prevObject );
		},

		data: function( key, value ){
			var parts = key.split(".");
			parts[1] = parts[1] ? "." + parts[1] : "";

			if ( value === undefined ) {
				var data = this.triggerHandler("getData" + parts[1] + "!", [parts[0]]);

				if ( data === undefined && this.length )
					data = $ektron.data( this[0], key );

				return data === undefined && parts[1] ?
					this.data( parts[0] ) :
					data;
			} else
				return this.trigger("setData" + parts[1] + "!", [parts[0], value]).each(function(){
					$ektron.data( this, key, value );
				});
		},

		removeData: function( key ){
			return this.each(function(){
				$ektron.removeData( this, key );
			});
		},

		domManip: function( args, table, reverse, callback ) {
			var clone = this.length > 1, elems;

			return this.each(function(){
				if ( !elems ) {
					elems = $ektron.clean( args, this.ownerDocument );

					if ( reverse )
						elems.reverse();
				}

				var obj = this;

				if ( table && $ektron.nodeName( this, "table" ) && $ektron.nodeName( elems[0], "tr" ) )
					obj = this.getElementsByTagName("tbody")[0] || this.appendChild( this.ownerDocument.createElement("tbody") );

				var scripts = $ektron( [] );

				$ektron.each(elems, function(){
					var elem = clone ?
						$ektron( this ).clone( true )[0] :
						this;

					// execute all scripts after the elements have been injected
					if ( $ektron.nodeName( elem, "script" ) )
						scripts = scripts.add( elem );
					else {
						// Remove any inner scripts for later evaluation
						if ( elem.nodeType == 1 )
							scripts = scripts.add( $ektron( "script", elem ).remove() );

						// Inject the elements into the document
						callback.call( obj, elem );
					}
				});
				// Ektron: Don't evaluate script that is in editable container (defect #35400)
				// isContentEditable for non-Mozilla (ie, Firefox 2), designMode for all
				if (this.isContentEditable != true && this.ownerDocument.designMode != "on") // Ektron
				{
				scripts.each( evalScript );
				}
			});
		}
	};

	// Give the init function the $ektron prototype for later instantiation
	$ektron.fn.init.prototype = $ektron.fn;

	function evalScript( i, elem ) {
		try // Ektron, added try/catch for #35400 in Firefox
		{
		if ( elem.src )
			$ektron.ajax({
				url: elem.src,
				async: false,
				dataType: "script"
			});

		else
			$ektron.globalEval( elem.text || elem.textContent || elem.innerHTML || "" );

		if ( elem.parentNode )
			elem.parentNode.removeChild( elem );
	}
		catch (ex)
		{
		}
}

	function now(){
		return +new Date;
	}

	$ektron.extend = $ektron.fn.extend = function() {
		// copy reference to target object
		var target = arguments[0] || {}, i = 1, length = arguments.length, deep = false, options;

		// Handle a deep copy situation
		if ( target.constructor == Boolean ) {
			deep = target;
			target = arguments[1] || {};
			// skip the boolean and the target
			i = 2;
		}

		// Handle case when target is a string or something (possible in deep copy)
		if ( typeof target != "object" && typeof target != "function" )
			target = {};

		// extend $ektron itself if only one argument is passed
		if ( length == i ) {
			target = this;
			--i;
		}

		for ( ; i < length; i++ )
			// Only deal with non-null/undefined values
			if ( (options = arguments[ i ]) != null )
				// Extend the base object
				for ( var name in options ) {
					var src = target[ name ], copy = options[ name ];

					// Prevent never-ending loop
					if ( target === copy )
						continue;

					// Recurse if we're merging object values
					if ( deep && copy && typeof copy == "object" && !copy.nodeType )
						target[ name ] = $ektron.extend( deep,
							// Never move original objects, clone them
							src || ( copy.length != null ? [ ] : { } )
						, copy );

					// Don't bring in undefined values
					else if ( copy !== undefined )
						target[ name ] = copy;

				}

		// Return the modified object
		return target;
	};

	var expando = "ektronjs" + now(), uuid = 0, windowData = {},
		// exclude the following css properties to add px
		exclude = /z-?index|font-?weight|opacity|zoom|line-?height/i,
		// cache defaultView
		defaultView = document.defaultView || {};

	$ektron.extend({
		// See test/unit/core.js for details concerning this function.
		isFunction: function( fn ) {
			return !!fn && typeof fn != "string" && !fn.nodeName &&
				fn.constructor != Array && /^[\s[]?function/.test( fn + "" );
		},

		// check if an element is in a (or is an) XML document
		isXMLDoc: function( elem ) {
			return elem.documentElement && !elem.body ||
				elem.tagName && elem.ownerDocument && !elem.ownerDocument.body;
		},

		// Evalulates a script in a global context
		globalEval: function( data ) {
			data = $ektron.trim( data );

			if ( data ) {
				// Inspired by code by Andrea Giammarchi
				// http://webreflection.blogspot.com/2007/08/global-scope-evaluation-and-dom.html
				var head = document.getElementsByTagName("head")[0] || document.documentElement,
					script = document.createElement("script");

				script.type = "text/javascript";
				if ( $ektron.browser.msie )
					script.text = data;
				else
					script.appendChild( document.createTextNode( data ) );

				// Use insertBefore instead of appendChild  to circumvent an IE6 bug.
				// This arises when a base node is used (#2709).
				head.insertBefore( script, head.firstChild );
				head.removeChild( script );
			}
		},

		nodeName: function( elem, name ) {
			return elem.nodeName && elem.nodeName.toUpperCase() == name.toUpperCase();
		},

		cache: {},

		data: function( elem, name, data ) {
			elem = elem == window ?
				windowData :
				elem;

			var id = elem[ expando ];

			// Compute a unique ID for the element
			if ( !id )
				id = elem[ expando ] = ++uuid;

			// Only generate the data cache if we're
			// trying to access or manipulate it
			if ( name && !$ektron.cache[ id ] )
				$ektron.cache[ id ] = {};

			// Prevent overriding the named cache with undefined values
			if ( data !== undefined )
				$ektron.cache[ id ][ name ] = data;

			// Return the named cache data, or the ID for the element
			return name ?
				$ektron.cache[ id ][ name ] :
				id;
		},

		removeData: function( elem, name ) {
			elem = elem == window ?
				windowData :
				elem;

			var id = elem[ expando ];

			// If we want to remove a specific section of the element's data
			if ( name ) {
				if ( $ektron.cache[ id ] ) {
					// Remove the section of cache data
					delete $ektron.cache[ id ][ name ];

					// If we've removed all the data, remove the element's cache
					name = "";

					for ( name in $ektron.cache[ id ] )
						break;

					if ( !name )
						$ektron.removeData( elem );
				}

			// Otherwise, we want to remove all of the element's data
			} else {
				// Clean up the element expando
				try {
					delete elem[ expando ];
				} catch(e){
					// IE has trouble directly removing the expando
					// but it's ok with using removeAttribute
					if ( elem.removeAttribute )
						elem.removeAttribute( expando );
				}

				// Completely remove the data cache
				delete $ektron.cache[ id ];
			}
		},

		// args is for internal usage only
		each: function( object, callback, args ) {
			var name, i = 0, length = object.length;

			if ( args ) {
				if ( length == undefined ) {
					for ( name in object )
						if ( callback.apply( object[ name ], args ) === false )
							break;
				} else
					for ( ; i < length; )
						if ( callback.apply( object[ i++ ], args ) === false )
							break;

			// A special, fast, case for the most common use of each
			} else {
				if ( length == undefined ) {
					for ( name in object )
						if ( callback.call( object[ name ], name, object[ name ] ) === false )
							break;
				} else
					for ( var value = object[0];
						i < length && callback.call( value, i, value ) !== false; value = object[++i] ){}
			}

			return object;
		},

		prop: function( elem, value, type, i, name ) {
			// Handle executable functions
			if ( $ektron.isFunction( value ) )
				value = value.call( elem, i );

			// Handle passing in a number to a CSS property
			return value && value.constructor == Number && type == "curCSS" && !exclude.test( name ) ?
				value + "px" :
				value;
		},

		className: {
			// internal only, use addClass("class")
			add: function( elem, classNames ) {
				$ektron.each((classNames || "").split(/\s+/), function(i, className){
					if ( elem.nodeType == 1 && !$ektron.className.has( elem.className, className ) )
						elem.className += (elem.className ? " " : "") + className;
				});
			},

			// internal only, use removeClass("class")
			remove: function( elem, classNames ) {
				if (elem.nodeType == 1)
					elem.className = classNames != undefined ?
						$ektron.grep(elem.className.split(/\s+/), function(className){
							return !$ektron.className.has( classNames, className );
						}).join(" ") :
						"";
			},

			// internal only, use hasClass("class")
			has: function( elem, className ) {
				return $ektron.inArray( className, (elem.className || elem).toString().split(/\s+/) ) > -1;
			}
		},

		// A method for quickly swapping in/out CSS properties to get correct calculations
		swap: function( elem, options, callback ) {
			var old = {};
			// Remember the old values, and insert the new ones
			for ( var name in options ) {
				old[ name ] = elem.style[ name ];
				elem.style[ name ] = options[ name ];
			}

			callback.call( elem );

			// Revert the old values
			for ( var name in options )
				elem.style[ name ] = old[ name ];
		},

		css: function( elem, name, force ) {
			if ( name == "width" || name == "height" ) {
				var val, props = { position: "absolute", visibility: "hidden", display:"block" }, which = name == "width" ? [ "Left", "Right" ] : [ "Top", "Bottom" ];

				function getWH() {
					val = name == "width" ? elem.offsetWidth : elem.offsetHeight;
					var padding = 0, border = 0;
					$ektron.each( which, function() {
						padding += parseFloat($ektron.curCSS( elem, "padding" + this, true)) || 0;
						border += parseFloat($ektron.curCSS( elem, "border" + this + "Width", true)) || 0;
					});
					val -= Math.round(padding + border);
				}

				if ( $ektron(elem).is(":visible") )
					getWH();
				else
					$ektron.swap( elem, props, getWH );

				return Math.max(0, val);
			}

			return $ektron.curCSS( elem, name, force );
		},

		curCSS: function( elem, name, force ) {
			var ret, style = elem.style;

			// A helper method for determining if an element's values are broken
			function color( elem ) {
				if ( !$ektron.browser.safari )
					return false;

				// defaultView is cached
				var ret = defaultView.getComputedStyle( elem, null );
				return !ret || ret.getPropertyValue("color") == "";
			}

			// We need to handle opacity special in IE
			if ( name == "opacity" && $ektron.browser.msie ) {
				ret = $ektron.attr( style, "opacity" );

				return ret == "" ?
					"1" :
					ret;
			}
			// Opera sometimes will give the wrong display answer, this fixes it, see #2037
			if ( $ektron.browser.opera && name == "display" ) {
				var save = style.outline;
				style.outline = "0 solid black";
				style.outline = save;
			}

			// Make sure we're using the right name for getting the float value
			if ( name.match( /float/i ) )
				name = styleFloat;

			if ( !force && style && style[ name ] )
				ret = style[ name ];

			else if ( defaultView.getComputedStyle ) {

				// Only "float" is needed here
				if ( name.match( /float/i ) )
					name = "float";

				name = name.replace( /([A-Z])/g, "-$1" ).toLowerCase();

				var computedStyle = defaultView.getComputedStyle( elem, null );

				if ( computedStyle && !color( elem ) )
					ret = computedStyle.getPropertyValue( name );

				// If the element isn't reporting its values properly in Safari
				// then some display: none elements are involved
				else {
					var swap = [], stack = [], a = elem, i = 0;

					// Locate all of the parent display: none elements
					for ( ; a && color(a); a = a.parentNode )
						stack.unshift(a);

					// Go through and make them visible, but in reverse
					// (It would be better if we knew the exact display type that they had)
					for ( ; i < stack.length; i++ )
						if ( color( stack[ i ] ) ) {
							swap[ i ] = stack[ i ].style.display;
							stack[ i ].style.display = "block";
						}

					// Since we flip the display style, we have to handle that
					// one special, otherwise get the value
					ret = name == "display" && swap[ stack.length - 1 ] != null ?
						"none" :
						( computedStyle && computedStyle.getPropertyValue( name ) ) || "";

					// Finally, revert the display styles back
					for ( i = 0; i < swap.length; i++ )
						if ( swap[ i ] != null )
							stack[ i ].style.display = swap[ i ];
				}

				// We should always get a number back from opacity
				if ( name == "opacity" && ret == "" )
					ret = "1";

			} else if ( elem.currentStyle ) {
				var camelCase = name.replace(/\-(\w)/g, function(all, letter){
					return letter.toUpperCase();
				});

				ret = elem.currentStyle[ name ] || elem.currentStyle[ camelCase ];

				// From the awesome hack by Dean Edwards
				// http://erik.eae.net/archives/2007/07/27/18.54.15/#comment-102291

				// If we're not dealing with a regular pixel number
				// but a number that has a weird ending, we need to convert it to pixels
				if ( !/^\d+(px)?$/i.test( ret ) && /^\d/.test( ret ) ) {
					// Remember the original values
					var left = style.left, rsLeft = elem.runtimeStyle.left;

					// Put in the new values to get a computed value out
					elem.runtimeStyle.left = elem.currentStyle.left;
					style.left = ret || 0;
					ret = style.pixelLeft + "px";

					// Revert the changed values
					style.left = left;
					elem.runtimeStyle.left = rsLeft;
				}
			}

			return ret;
		},

		clean: function( elems, context ) {
			var ret = [];
			context = context || document;
			// !context.createElement fails in IE with an error but returns typeof 'object'
			if (typeof context.createElement == 'undefined')
				context = context.ownerDocument || context[0] && context[0].ownerDocument || document;

			$ektron.each(elems, function(i, elem){
				if ( !elem )
					return;

				if ( elem.constructor == Number )
					elem += '';

				// Convert html string into DOM nodes
				if ( typeof elem == "string" ) {
					// Fix "XHTML"-style tags in all browsers
					elem = elem.replace(/(<(\w+)[^>]*?)\/>/g, function(all, front, tag){
						return tag.match(/^(abbr|br|col|img|input|link|meta|param|hr|area|embed)$/i) ?
							all :
							front + "></" + tag + ">";
					});

					// Trim whitespace, otherwise indexOf won't work as expected
					var tags = $ektron.trim( elem ).toLowerCase(), div = context.createElement("div");

					var wrap =
						// option or optgroup
						!tags.indexOf("<opt") &&
						[ 1, "<select multiple='multiple'>", "</select>" ] ||

						!tags.indexOf("<leg") &&
						[ 1, "<fieldset>", "</fieldset>" ] ||

						tags.match(/^<(thead|tbody|tfoot|colg|cap)/) &&
						[ 1, "<table>", "</table>" ] ||

						!tags.indexOf("<tr") &&
						[ 2, "<table><tbody>", "</tbody></table>" ] ||

				 		// <thead> matched above
						(!tags.indexOf("<td") || !tags.indexOf("<th")) &&
						[ 3, "<table><tbody><tr>", "</tr></tbody></table>" ] ||

						!tags.indexOf("<col") &&
						[ 2, "<table><tbody></tbody><colgroup>", "</colgroup></table>" ] ||

						// IE can't serialize <link> and <script> tags normally
						$ektron.browser.msie &&
						[ 1, "div<div>", "</div>" ] ||

						[ 0, "", "" ];

					// Go to html and back, then peel off extra wrappers
					div.innerHTML = wrap[1] + elem + wrap[2];

					// Move to the right depth
					while ( wrap[0]-- )
						div = div.lastChild;

					// Remove IE's autoinserted <tbody> from table fragments
					if ( $ektron.browser.msie ) {

						// String was a <table>, *may* have spurious <tbody>
						var tbody = !tags.indexOf("<table") && tags.indexOf("<tbody") < 0 ?
							div.firstChild && div.firstChild.childNodes :

							// String was a bare <thead> or <tfoot>
							wrap[1] == "<table>" && tags.indexOf("<tbody") < 0 ?
								div.childNodes :
								[];

						for ( var j = tbody.length - 1; j >= 0 ; --j )
							if ( $ektron.nodeName( tbody[ j ], "tbody" ) && !tbody[ j ].childNodes.length )
								tbody[ j ].parentNode.removeChild( tbody[ j ] );

						// IE completely kills leading whitespace when innerHTML is used
						if ( /^\s/.test( elem ) )
							div.insertBefore( context.createTextNode( elem.match(/^\s*/)[0] ), div.firstChild );

					}

					elem = $ektron.makeArray( div.childNodes );
				}

				if ( elem.length === 0 && (!$ektron.nodeName( elem, "form" ) && !$ektron.nodeName( elem, "select" )) )
					return;

				if ( elem[0] == undefined || $ektron.nodeName( elem, "form" ) || elem.options )
					ret.push( elem );

				else
					ret = $ektron.merge( ret, elem );

			});

			return ret;
		},

		attr: function( elem, name, value ) {
			// don't set attributes on text and comment nodes
			if (!elem || elem.nodeType == 3 || elem.nodeType == 8)
				return undefined;

			var notxml = !$ektron.isXMLDoc( elem ),
				// Whether we are setting (or getting)
				set = value !== undefined,
				msie = $ektron.browser.msie;

			// Try to normalize/fix the name
			name = notxml && $ektron.props[ name ] || name;

			// Only do all the following if this is a node (faster for style)
			// IE elem.getAttribute passes even for style
			if ( elem.tagName ) {

				// These attributes require special treatment
				var special = /href|src|style/.test( name );

				// Safari mis-reports the default selected property of a hidden option
				// Accessing the parent's selectedIndex property fixes it
				if ( name == "selected" && $ektron.browser.safari )
					elem.parentNode.selectedIndex;

				// If applicable, access the attribute via the DOM 0 way
				if ( name in elem && notxml && !special ) {
					if ( set ){
						// We can't allow the type property to be changed (since it causes problems in IE)
						if ( name == "type" && $ektron.nodeName( elem, "input" ) && elem.parentNode )
							throw "type property can't be changed";

						elem[ name ] = value;
					}

					// browsers index elements by id/name on forms, give priority to attributes.
					if( $ektron.nodeName( elem, "form" ) && elem.getAttributeNode(name) )
						return elem.getAttributeNode( name ).nodeValue;

					return elem[ name ];
				}

				if ( msie && notxml &&  name == "style" )
					return $ektron.attr( elem.style, "cssText", value );

				if ( set )
					// convert the value to a string (all browsers do this but IE) see #1070
					elem.setAttribute( name, "" + value );

				var attr = msie && notxml && special
						// Some attributes require a special call on IE
						? elem.getAttribute( name, 2 )
						: elem.getAttribute( name );

				// Non-existent attributes return null, we normalize to undefined
				return attr === null ? undefined : attr;
			}

			// elem is actually elem.style ... set the style

			// IE uses filters for opacity
			if ( msie && name == "opacity" ) {
				if ( set ) {
					// IE has trouble with opacity if it does not have layout
					// Force it by setting the zoom level
					elem.zoom = 1;

					// Set the alpha filter to set the opacity
					elem.filter = (elem.filter || "").replace( /alpha\([^)]*\)/, "" ) +
						(parseInt( value ) + '' == "NaN" ? "" : "alpha(opacity=" + value * 100 + ")");
				}

				return elem.filter && elem.filter.indexOf("opacity=") >= 0 ?
					(parseFloat( elem.filter.match(/opacity=([^)]*)/)[1] ) / 100) + '':
					"";
			}

			name = name.replace(/-([a-z])/ig, function(all, letter){
				return letter.toUpperCase();
			});

			if ( set )
				elem[ name ] = value;

			return elem[ name ];
		},

		trim: function( text ) {
			return (text || "").replace( /^\s+|\s+$/g, "" );
		},

		makeArray: function( array ) {
			var ret = [];

			if( array != null ){
				var i = array.length;
				//the window, strings and functions also have 'length'
				if( i == null || array.split || array.setInterval || array.call )
					ret[0] = array;
				else
					while( i )
						ret[--i] = array[i];
			}

			return ret;
		},

		inArray: function( elem, array ) {
			for ( var i = 0, length = array.length; i < length; i++ )
			// Use === because on IE, window == document
				if ( array[ i ] === elem )
					return i;

			return -1;
		},

		merge: function( first, second ) {
			// We have to loop this way because IE & Opera overwrite the length
			// expando of getElementsByTagName
			var i = 0, elem, pos = first.length;
			// Also, we need to make sure that the correct elements are being returned
			// (IE returns comment nodes in a '*' query)
			if ( $ektron.browser.msie ) {
				while ( elem = second[ i++ ] )
					if ( elem.nodeType != 8 )
						first[ pos++ ] = elem;

			} else
				while ( elem = second[ i++ ] )
					first[ pos++ ] = elem;

			return first;
		},

		unique: function( array ) {
			var ret = [], done = {};

			try {

				for ( var i = 0, length = array.length; i < length; i++ ) {
					var id = $ektron.data( array[ i ] );

					if ( !done[ id ] ) {
						done[ id ] = true;
						ret.push( array[ i ] );
					}
				}

			} catch( e ) {
				ret = array;
			}

			return ret;
		},

		grep: function( elems, callback, inv ) {
			var ret = [];

			// Go through the array, only saving the items
			// that pass the validator function
			for ( var i = 0, length = elems.length; i < length; i++ )
				if ( !inv != !callback( elems[ i ], i ) )
					ret.push( elems[ i ] );

			return ret;
		},

		map: function( elems, callback ) {
			var ret = [];

			// Go through the array, translating each of the items to their
			// new value (or values).
			for ( var i = 0, length = elems.length; i < length; i++ ) {
				var value = callback( elems[ i ], i );

				if ( value != null )
					ret[ ret.length ] = value;
			}

			return ret.concat.apply( [], ret );
		}
	});

	var userAgent = navigator.userAgent.toLowerCase();

	// Figure out what browser is being used
	$ektron.browser = {
		version: (userAgent.match( /.+(?:rv|it|ra|ie)[\/: ]([\d.]+)/ ) || [])[1],
		safari: /webkit/.test( userAgent ),
		opera: /opera/.test( userAgent ),
		msie: /msie/.test( userAgent ) && !/opera/.test( userAgent ),
		mozilla: /mozilla/.test( userAgent ) && !/(compatible|webkit)/.test( userAgent )
	};

	var styleFloat = $ektron.browser.msie ?
		"styleFloat" :
		"cssFloat";

	$ektron.extend({
		// Check to see if the W3C box model is being used
		boxModel: !$ektron.browser.msie || document.compatMode == "CSS1Compat",

		props: {
			"for": "htmlFor",
			"class": "className",
			"float": styleFloat,
			cssFloat: styleFloat,
			styleFloat: styleFloat,
			readonly: "readOnly",
			maxlength: "maxLength",
			cellspacing: "cellSpacing"
		}
	});

	$ektron.each({
		parent: function(elem){return elem.parentNode;},
		parents: function(elem){return $ektron.dir(elem,"parentNode");},
		next: function(elem){return $ektron.nth(elem,2,"nextSibling");},
		prev: function(elem){return $ektron.nth(elem,2,"previousSibling");},
		nextAll: function(elem){return $ektron.dir(elem,"nextSibling");},
		prevAll: function(elem){return $ektron.dir(elem,"previousSibling");},
		siblings: function(elem){return $ektron.sibling(elem.parentNode.firstChild,elem);},
		children: function(elem){return $ektron.sibling(elem.firstChild);},
		contents: function(elem){return $ektron.nodeName(elem,"iframe")?elem.contentDocument||elem.contentWindow.document:$ektron.makeArray(elem.childNodes);}
	}, function(name, fn){
		$ektron.fn[ name ] = function( selector ) {
			var ret = $ektron.map( this, fn );

			if ( selector && typeof selector == "string" )
				ret = $ektron.multiFilter( selector, ret );

			return this.pushStack( $ektron.unique( ret ) );
		};
	});

	$ektron.each({
		appendTo: "append",
		prependTo: "prepend",
		insertBefore: "before",
		insertAfter: "after",
		replaceAll: "replaceWith"
	}, function(name, original){
		$ektron.fn[ name ] = function() {
			var args = arguments;

			return this.each(function(){
				for ( var i = 0, length = args.length; i < length; i++ )
					$ektron( args[ i ] )[ original ]( this );
			});
		};
	});

	$ektron.each({
		removeAttr: function( name ) {
			$ektron.attr( this, name, "" );
			if (this.nodeType == 1)
				this.removeAttribute( name );
		},

		addClass: function( classNames ) {
			$ektron.className.add( this, classNames );
		},

		removeClass: function( classNames ) {
			$ektron.className.remove( this, classNames );
		},

		toggleClass: function( classNames ) {
			$ektron.className[ $ektron.className.has( this, classNames ) ? "remove" : "add" ]( this, classNames );
		},

		remove: function( selector ) {
			if ( !selector || $ektron.filter( selector, [ this ] ).r.length ) {
				// Prevent memory leaks
				$ektron( "*", this ).add(this).each(function(){
					$ektron.event.remove(this);
					$ektron.removeData(this);
				});
				if (this.parentNode)
					this.parentNode.removeChild( this );
			}
		},

		empty: function() {
			// Remove element nodes and prevent memory leaks
			$ektron( ">*", this ).remove();

			// Remove any remaining nodes
			while ( this.firstChild )
				this.removeChild( this.firstChild );
		}
	}, function(name, fn){
		$ektron.fn[ name ] = function(){
			return this.each( fn, arguments );
		};
	});

	$ektron.each([ "Height", "Width" ], function(i, name){
		var type = name.toLowerCase();

		$ektron.fn[ type ] = function( size ) {
			// Get window width or height
			return this[0] == window ?
				// Opera reports document.body.client[Width/Height] properly in both quirks and standards
				$ektron.browser.opera && document.body[ "client" + name ] ||

				// Safari reports inner[Width/Height] just fine (Mozilla and Opera include scroll bar widths)
				$ektron.browser.safari && window[ "inner" + name ] ||

				// Everyone else use document.documentElement or document.body depending on Quirks vs Standards mode
				document.compatMode == "CSS1Compat" && document.documentElement[ "client" + name ] || document.body[ "client" + name ] :

				// Get document width or height
				this[0] == document ?
					// Either scroll[Width/Height] or offset[Width/Height], whichever is greater
					Math.max(
						Math.max(document.body["scroll" + name], document.documentElement["scroll" + name]),
						Math.max(document.body["offset" + name], document.documentElement["offset" + name])
					) :

					// Get or set width or height on the element
					size == undefined ?
						// Get width or height on the element
						(this.length ? $ektron.css( this[0], type ) : null) :

						// Set the width or height on the element (default to pixels if value is unitless)
						this.css( type, size.constructor == String ? size : size + "px" );
		};
	});

	// Helper function used by the dimensions and offset modules
	function num(elem, prop) {
		return elem[0] && parseInt( $ektron.curCSS(elem[0], prop, true), 10 ) || 0;
	}var chars = $ektron.browser.safari && parseInt($ektron.browser.version) < 417 ?
			"(?:[\\w*_-]|\\\\.)" :
			"(?:[\\w\u0128-\uFFFF*_-]|\\\\.)",
		quickChild = new RegExp("^>\\s*(" + chars + "+)"),
		quickID = new RegExp("^(" + chars + "+)(#)(" + chars + "+)"),
		quickClass = new RegExp("^([#.]?)(" + chars + "*)");

	$ektron.extend({
		expr: {
			"": function(a,i,m){return m[2]=="*"||$ektron.nodeName(a,m[2]);},
			"#": function(a,i,m){return a.getAttribute("id")==m[2];},
			":": {
				// Position Checks
				lt: function(a,i,m){return i<m[3]-0;},
				gt: function(a,i,m){return i>m[3]-0;},
				nth: function(a,i,m){return m[3]-0==i;},
				eq: function(a,i,m){return m[3]-0==i;},
				first: function(a,i){return i==0;},
				last: function(a,i,m,r){return i==r.length-1;},
				even: function(a,i){return i%2==0;},
				odd: function(a,i){return i%2;},

				// Child Checks
				"first-child": function(a){return a.parentNode.getElementsByTagName("*")[0]==a;},
				"last-child": function(a){return $ektron.nth(a.parentNode.lastChild,1,"previousSibling")==a;},
				"only-child": function(a){return !$ektron.nth(a.parentNode.lastChild,2,"previousSibling");},

				// Parent Checks
				parent: function(a){return a.firstChild;},
				empty: function(a){return !a.firstChild;},

				// Text Check
				contains: function(a,i,m){return (a.textContent||a.innerText||$ektron(a).text()||"").indexOf(m[3])>=0;},

				// Visibility
				visible: function(a){return "hidden"!=a.type&&$ektron.css(a,"display")!="none"&&$ektron.css(a,"visibility")!="hidden";},
				hidden: function(a){return "hidden"==a.type||$ektron.css(a,"display")=="none"||$ektron.css(a,"visibility")=="hidden";},

				// Form attributes
				enabled: function(a){return !a.disabled;},
				disabled: function(a){return a.disabled;},
				checked: function(a){return a.checked;},
				selected: function(a){return a.selected||$ektron.attr(a,"selected");},

				// Form elements
				text: function(a){return "text"==a.type;},
				radio: function(a){return "radio"==a.type;},
				checkbox: function(a){return "checkbox"==a.type;},
				file: function(a){return "file"==a.type;},
				password: function(a){return "password"==a.type;},
				submit: function(a){return "submit"==a.type;},
				image: function(a){return "image"==a.type;},
				reset: function(a){return "reset"==a.type;},
				button: function(a){return "button"==a.type||$ektron.nodeName(a,"button");},
				input: function(a){return /input|select|textarea|button/i.test(a.nodeName);},

				// :has()
				has: function(a,i,m){return $ektron.find(m[3],a).length;},

				// :header
				header: function(a){return /h\d/i.test(a.nodeName);},

				// :animated
				animated: function(a){return $ektron.grep($ektron.timers,function(fn){return a==fn.elem;}).length;}
			}
		},

		// The regular expressions that power the parsing engine
		parse: [
			// Match: [@value='test'], [@foo]
			/^(\[) *@?([\w-]+) *([!*$^~=]*) *('?"?)(.*?)\4 *\]/,

			// Match: :contains('foo')
			/^(:)([\w-]+)\("?'?(.*?(\(.*?\))?[^(]*?)"?'?\)/,

			// Match: :even, :last-child, #id, .class
			new RegExp("^([:.#]*)(" + chars + "+)")
		],

		multiFilter: function( expr, elems, not ) {
			var old, cur = [];

			while ( expr && expr != old ) {
				old = expr;
				var f = $ektron.filter( expr, elems, not );
				expr = f.t.replace(/^\s*,\s*/, "" );
				cur = not ? elems = f.r : $ektron.merge( cur, f.r );
			}

			return cur;
		},

		find: function( t, context ) {
			// Quickly handle non-string expressions
			if ( typeof t != "string" )
				return [ t ];

			// check to make sure context is a DOM element or a document
			if ( context && context.nodeType != 1 && context.nodeType != 9)
				return [ ];

			// Set the correct context (if none is provided)
			context = context || document;

			// Initialize the search
			var ret = [context], done = [], last, nodeName;

			// Continue while a selector expression exists, and while
			// we're no longer looping upon ourselves
			while ( t && last != t ) {
				var r = [];
				last = t;

				t = $ektron.trim(t);

				var foundToken = false,

				// An attempt at speeding up child selectors that
				// point to a specific element tag
					re = quickChild,

					m = re.exec(t);

				if ( m ) {
					nodeName = m[1].toUpperCase();

					// Perform our own iteration and filter
					for ( var i = 0; ret[i]; i++ )
						for ( var c = ret[i].firstChild; c; c = c.nextSibling )
							if ( c.nodeType == 1 && (nodeName == "*" || c.nodeName.toUpperCase() == nodeName) )
								r.push( c );

					ret = r;
					t = t.replace( re, "" );
					if ( t.indexOf(" ") == 0 ) continue;
					foundToken = true;
				} else {
					re = /^([>+~])\s*(\w*)/i;

					if ( (m = re.exec(t)) != null ) {
						r = [];

						var merge = {};
						nodeName = m[2].toUpperCase();
						m = m[1];

						for ( var j = 0, rl = ret.length; j < rl; j++ ) {
							var n = m == "~" || m == "+" ? ret[j].nextSibling : ret[j].firstChild;
							for ( ; n; n = n.nextSibling )
								if ( n.nodeType == 1 ) {
									var id = $ektron.data(n);

									if ( m == "~" && merge[id] ) break;

									if (!nodeName || n.nodeName.toUpperCase() == nodeName ) {
										if ( m == "~" ) merge[id] = true;
										r.push( n );
									}

									if ( m == "+" ) break;
								}
						}

						ret = r;

						// And remove the token
						t = $ektron.trim( t.replace( re, "" ) );
						foundToken = true;
					}
				}

				// See if there's still an expression, and that we haven't already
				// matched a token
				if ( t && !foundToken ) {
					// Handle multiple expressions
					if ( !t.indexOf(",") ) {
						// Clean the result set
						if ( context == ret[0] ) ret.shift();

						// Merge the result sets
						done = $ektron.merge( done, ret );

						// Reset the context
						r = ret = [context];

						// Touch up the selector string
						t = " " + t.substr(1,t.length);

					} else {
						// Optimize for the case nodeName#idName
						var re2 = quickID;
						var m = re2.exec(t);

						// Re-organize the results, so that they're consistent
						if ( m ) {
							m = [ 0, m[2], m[3], m[1] ];

						} else {
							// Otherwise, do a traditional filter check for
							// ID, class, and element selectors
							re2 = quickClass;
							m = re2.exec(t);
						}

						m[2] = m[2].replace(/\\/g, "");

						var elem = ret[ret.length-1];

						// Try to do a global search by ID, where we can
						if ( m[1] == "#" && elem && elem.getElementById && !$ektron.isXMLDoc(elem) ) {
							// Optimization for HTML document case
							var oid = elem.getElementById(m[2]);

							// Do a quick check for the existence of the actual ID attribute
							// to avoid selecting by the name attribute in IE
							// also check to insure id is a string to avoid selecting an element with the name of 'id' inside a form
							if ( ($ektron.browser.msie||$ektron.browser.opera) && oid && typeof oid.id == "string" && oid.id != m[2] )
								oid = $ektron('[@id="'+m[2]+'"]', elem)[0];

							// Do a quick check for node name (where applicable) so
							// that div#foo searches will be really fast
							ret = r = oid && (!m[3] || $ektron.nodeName(oid, m[3])) ? [oid] : [];
						} else {
							// We need to find all descendant elements
							for ( var i = 0; ret[i]; i++ ) {
								// Grab the tag name being searched for
								var tag = m[1] == "#" && m[3] ? m[3] : m[1] != "" || m[0] == "" ? "*" : m[2];

								// Handle IE7 being really dumb about <object>s
								if ( tag == "*" && ret[i].nodeName.toLowerCase() == "object" )
									tag = "param";

								r = $ektron.merge( r, ret[i].getElementsByTagName( tag ));
							}

							// It's faster to filter by class and be done with it
							if ( m[1] == "." )
								r = $ektron.classFilter( r, m[2] );

							// Same with ID filtering
							if ( m[1] == "#" ) {
								var tmp = [];

								// Try to find the element with the ID
								for ( var i = 0; r[i]; i++ )
									if ( r[i].getAttribute("id") == m[2] ) {
										tmp = [ r[i] ];
										break;
									}

								r = tmp;
							}

							ret = r;
						}

						t = t.replace( re2, "" );
					}

				}

				// If a selector string still exists
				if ( t ) {
					// Attempt to filter it
					var val = $ektron.filter(t,r);
					ret = r = val.r;
					t = $ektron.trim(val.t);
				}
			}

			// An error occurred with the selector;
			// just return an empty set instead
			if ( t )
				ret = [];

			// Remove the root context
			if ( ret && context == ret[0] )
				ret.shift();

			// And combine the results
			done = $ektron.merge( done, ret );

			return done;
		},

		classFilter: function(r,m,not){
			m = " " + m + " ";
			var tmp = [];
			for ( var i = 0; r[i]; i++ ) {
				var pass = (" " + r[i].className + " ").indexOf( m ) >= 0;
				if ( !not && pass || not && !pass )
					tmp.push( r[i] );
			}
			return tmp;
		},

		filter: function(t,r,not) {
			var last;

			// Look for common filter expressions
			while ( t && t != last ) {
				last = t;

				var p = $ektron.parse, m;

				for ( var i = 0; p[i]; i++ ) {
					m = p[i].exec( t );

					if ( m ) {
						// Remove what we just matched
						t = t.substring( m[0].length );

						m[2] = m[2].replace(/\\/g, "");
						break;
					}
				}

				if ( !m )
					break;

				// :not() is a special case that can be optimized by
				// keeping it out of the expression list
				if ( m[1] == ":" && m[2] == "not" )
					// optimize if only one selector found (most common case)
					r = isSimple.test( m[3] ) ?
						$ektron.filter(m[3], r, true).r :
						$ektron( r ).not( m[3] );

				// We can get a big speed boost by filtering by class here
				else if ( m[1] == "." )
					r = $ektron.classFilter(r, m[2], not);

				else if ( m[1] == "[" ) {
					var tmp = [], type = m[3];

					for ( var i = 0, rl = r.length; i < rl; i++ ) {
						var a = r[i], z = a[ $ektron.props[m[2]] || m[2] ];

						if ( z == null || /href|src|selected/.test(m[2]) )
							z = $ektron.attr(a,m[2]) || '';

						if ( (type == "" && !!z ||
							 type == "=" && z == m[5] ||
							 type == "!=" && z != m[5] ||
							 type == "^=" && z && !z.indexOf(m[5]) ||
							 type == "$=" && z.substr(z.length - m[5].length) == m[5] ||
							 (type == "*=" || type == "~=") && z.indexOf(m[5]) >= 0) ^ not )
								tmp.push( a );
					}

					r = tmp;

				// We can get a speed boost by handling nth-child here
				} else if ( m[1] == ":" && m[2] == "nth-child" ) {
					var merge = {}, tmp = [],
						// parse equations like 'even', 'odd', '5', '2n', '3n+2', '4n-1', '-n+6'
						test = /(-?)(\d*)n((?:\+|-)?\d*)/.exec(
							m[3] == "even" && "2n" || m[3] == "odd" && "2n+1" ||
							!/\D/.test(m[3]) && "0n+" + m[3] || m[3]),
						// calculate the numbers (first)n+(last) including if they are negative
						first = (test[1] + (test[2] || 1)) - 0, last = test[3] - 0;

					// loop through all the elements left in the $ektron object
					for ( var i = 0, rl = r.length; i < rl; i++ ) {
						var node = r[i], parentNode = node.parentNode, id = $ektron.data(parentNode);

						if ( !merge[id] ) {
							var c = 1;

							for ( var n = parentNode.firstChild; n; n = n.nextSibling )
								if ( n.nodeType == 1 )
									n.nodeIndex = c++;

							merge[id] = true;
						}

						var add = false;

						if ( first == 0 ) {
							if ( node.nodeIndex == last )
								add = true;
						} else if ( (node.nodeIndex - last) % first == 0 && (node.nodeIndex - last) / first >= 0 )
							add = true;

						if ( add ^ not )
							tmp.push( node );
					}

					r = tmp;

				// Otherwise, find the expression to execute
				} else {
					var fn = $ektron.expr[ m[1] ];
					if ( typeof fn == "object" )
						fn = fn[ m[2] ];

					if ( typeof fn == "string" )
						fn = eval("false||function(a,i){return " + fn + ";}");

					// Execute it against the current filter
					r = $ektron.grep( r, function(elem, i){
						return fn(elem, i, m, r);
					}, not );
				}
			}

			// Return an array of filtered elements (r)
			// and the modified expression string (t)
			return { r: r, t: t };
		},

		dir: function( elem, dir ){
			var matched = [],
				cur = elem[dir];
			while ( cur && cur != document ) {
				if ( cur.nodeType == 1 )
					matched.push( cur );
				cur = cur[dir];
			}
			return matched;
		},

		nth: function(cur,result,dir,elem){
			result = result || 1;
			var num = 0;

			for ( ; cur; cur = cur[dir] )
				if ( cur.nodeType == 1 && ++num == result )
					break;

			return cur;
		},

		sibling: function( n, elem ) {
			var r = [];

			for ( ; n; n = n.nextSibling ) {
				if ( n.nodeType == 1 && n != elem )
					r.push( n );
			}

			return r;
		}
	});
	/*
	 * A number of helper functions used for managing events.
	 * Many of the ideas behind this code orignated from
	 * Dean Edwards' addEvent library.
	 */
	$ektron.event = {

		// Bind an event to an element
		// Original by Dean Edwards
		add: function(elem, types, handler, data) {
			if ( elem.nodeType == 3 || elem.nodeType == 8 )
				return;

			// For whatever reason, IE has trouble passing the window object
			// around, causing it to be cloned in the process
			if ( $ektron.browser.msie && elem.setInterval )
				elem = window;

			// Make sure that the function being executed has a unique ID
			if ( !handler.guid )
				handler.guid = this.guid++;

			// if data is passed, bind to handler
			if( data != undefined ) {
				// Create temporary function pointer to original handler
				var fn = handler;

				// Create unique handler function, wrapped around original handler
				handler = this.proxy( fn, function() {
					// Pass arguments and context to original handler
					return fn.apply(this, arguments);
				});

				// Store data in unique handler
				handler.data = data;
			}

			// Init the element's event structure
			var events = $ektron.data(elem, "events") || $ektron.data(elem, "events", {}),
				handle = $ektron.data(elem, "handle") || $ektron.data(elem, "handle", function(){
					// Handle the second event of a trigger and when
					// an event is called after a page has unloaded
					if ( typeof $ektron != "undefined" && !$ektron.event.triggered )
						return $ektron.event.handle.apply(arguments.callee.elem, arguments);
				});
			// Add elem as a property of the handle function
			// This is to prevent a memory leak with non-native
			// event in IE.
			handle.elem = elem;

			// Handle multiple events separated by a space
			// $ektron(...).bind("mouseover mouseout", fn);
			$ektron.each(types.split(/\s+/), function(index, type) {
				// Namespaced event handlers
				var parts = type.split(".");
				type = parts[0];
				handler.type = parts[1];

				// Get the current list of functions bound to this event
				var handlers = events[type];

				// Init the event handler queue
				if (!handlers) {
					handlers = events[type] = {};

					// Check for a special event handler
					// Only use addEventListener/attachEvent if the special
					// events handler returns false
					if ( !$ektron.event.special[type] || $ektron.event.special[type].setup.call(elem) === false ) {
						// Bind the global event handler to the element
						if (elem.addEventListener)
							elem.addEventListener(type, handle, false);
						else if (elem.attachEvent)
							elem.attachEvent("on" + type, handle);
					}
				}

				// Add the function to the element's handler list
				handlers[handler.guid] = handler;

				// Keep track of which events have been used, for global triggering
				$ektron.event.global[type] = true;
			});

			// Nullify elem to prevent memory leaks in IE
			elem = null;
		},

		guid: 1,
		global: {},

		// Detach an event or set of events from an element
		remove: function(elem, types, handler) {
			// don't do events on text and comment nodes
			if ( elem.nodeType == 3 || elem.nodeType == 8 )
				return;

			var events = $ektron.data(elem, "events"), ret, index;

			if ( events ) {
				// Unbind all events for the element
				if ( types == undefined || (typeof types == "string" && types.charAt(0) == ".") )
					for ( var type in events )
						this.remove( elem, type + (types || "") );
				else {
					// types is actually an event object here
					if ( types.type ) {
						handler = types.handler;
						types = types.type;
					}

					// Handle multiple events seperated by a space
					// $ektron(...).unbind("mouseover mouseout", fn);
					$ektron.each(types.split(/\s+/), function(index, type){
						// Namespaced event handlers
						var parts = type.split(".");
						type = parts[0];

						if ( events[type] ) {
							// remove the given handler for the given type
							if ( handler )
								delete events[type][handler.guid];

							// remove all handlers for the given type
							else
								for ( handler in events[type] )
									// Handle the removal of namespaced events
									if ( !parts[1] || events[type][handler].type == parts[1] )
										delete events[type][handler];

							// remove generic event handler if no more handlers exist
							for ( ret in events[type] ) break;
							if ( !ret ) {
								if ( !$ektron.event.special[type] || $ektron.event.special[type].teardown.call(elem) === false ) {
									if (elem.removeEventListener)
										elem.removeEventListener(type, $ektron.data(elem, "handle"), false);
									else if (elem.detachEvent)
										elem.detachEvent("on" + type, $ektron.data(elem, "handle"));
								}
								ret = null;
								delete events[type];
							}
						}
					});
				}

				// Remove the expando if it's no longer used
				for ( ret in events ) break;
				if ( !ret ) {
					var handle = $ektron.data( elem, "handle" );
					if ( handle ) handle.elem = null;
					$ektron.removeData( elem, "events" );
					$ektron.removeData( elem, "handle" );
				}
			}
		},

		trigger: function(type, data, elem, donative, extra) {
			// Clone the incoming data, if any
			data = $ektron.makeArray(data);

			if ( type.indexOf("!") >= 0 ) {
				type = type.slice(0, -1);
				var exclusive = true;
			}

			// Handle a global trigger
			if ( !elem ) {
				// Only trigger if we've ever bound an event for it
				if ( this.global[type] )
					$ektron("*").add([window, document]).trigger(type, data);

			// Handle triggering a single element
			} else {
				// don't do events on text and comment nodes
				if ( elem.nodeType == 3 || elem.nodeType == 8 )
					return undefined;

				var val, ret, fn = $ektron.isFunction( elem[ type ] || null ),
					// Check to see if we need to provide a fake event, or not
					event = !data[0] || !data[0].preventDefault;

				// Pass along a fake event
				if ( event ) {
					data.unshift({
						type: type,
						target: elem,
						preventDefault: function(){},
						stopPropagation: function(){},
						timeStamp: now()
					});
					data[0][expando] = true; // no need to fix fake event
				}

				// Enforce the right trigger type
				data[0].type = type;
				if ( exclusive )
					data[0].exclusive = true;

				// Trigger the event, it is assumed that "handle" is a function
				var handle = $ektron.data(elem, "handle");
				if ( handle )
					val = handle.apply( elem, data );

				// Handle triggering native .onfoo handlers (and on links since we don't call .click() for links)
				if ( (!fn || ($ektron.nodeName(elem, 'a') && type == "click")) && elem["on"+type] && elem["on"+type].apply( elem, data ) === false )
					val = false;

				// Extra functions don't get the custom event object
				if ( event )
					data.shift();

				// Handle triggering of extra function
				if ( extra && $ektron.isFunction( extra ) ) {
					// call the extra function and tack the current return value on the end for possible inspection
					ret = extra.apply( elem, val == null ? data : data.concat( val ) );
					// if anything is returned, give it precedence and have it overwrite the previous value
					if (ret !== undefined)
						val = ret;
				}

				// Trigger the native events (except for clicks on links)
				if ( fn && donative !== false && val !== false && !($ektron.nodeName(elem, 'a') && type == "click") ) {
					this.triggered = true;
					try {
						elem[ type ]();
					// prevent IE from throwing an error for some hidden elements
					} catch (e) {}
				}

				this.triggered = false;
			}

			return val;
		},

		handle: function(event) {
			// returned undefined or false
			var val, ret, namespace, all, handlers;

			event = arguments[0] = $ektron.event.fix( event || window.event );

			// Namespaced event handlers
			namespace = event.type.split(".");
			event.type = namespace[0];
			namespace = namespace[1];
			// Cache this now, all = true means, any handler
			all = !namespace && !event.exclusive;

			handlers = ( $ektron.data(this, "events") || {} )[event.type];

			for ( var j in handlers ) {
				var handler = handlers[j];

				// Filter the functions by class
				if ( all || handler.type == namespace ) {
					// Pass in a reference to the handler function itself
					// So that we can later remove it
					event.handler = handler;
					event.data = handler.data;

					ret = handler.apply( this, arguments );

					if ( val !== false )
						val = ret;

					if ( ret === false ) {
						event.preventDefault();
						event.stopPropagation();
					}
				}
			}

			return val;
		},

		fix: function(event) {
			if ( event[expando] == true )
				return event;

			// store a copy of the original event object
			// and "clone" to set read-only properties
			var originalEvent = event;
			event = { originalEvent: originalEvent };
			var props = "altKey attrChange attrName bubbles button cancelable charCode clientX clientY ctrlKey currentTarget data detail eventPhase fromElement handler keyCode metaKey newValue originalTarget pageX pageY prevValue relatedNode relatedTarget screenX screenY shiftKey srcElement target timeStamp toElement type view wheelDelta which".split(" ");
			for ( var i=props.length; i; i-- )
				event[ props[i] ] = originalEvent[ props[i] ];

			// Mark it as fixed
			event[expando] = true;

			// add preventDefault and stopPropagation since
			// they will not work on the clone
			event.preventDefault = function() {
				// if preventDefault exists run it on the original event
				if (originalEvent.preventDefault)
					originalEvent.preventDefault();
				// otherwise set the returnValue property of the original event to false (IE)
				originalEvent.returnValue = false;
			};
			event.stopPropagation = function() {
				// if stopPropagation exists run it on the original event
				if (originalEvent.stopPropagation)
					originalEvent.stopPropagation();
				// otherwise set the cancelBubble property of the original event to true (IE)
				originalEvent.cancelBubble = true;
			};

			// Fix timeStamp
			event.timeStamp = event.timeStamp || now();

			// Fix target property, if necessary
			if ( !event.target )
				event.target = event.srcElement || document; // Fixes #1925 where srcElement might not be defined either

			// check if target is a textnode (safari)
			if ( event.target.nodeType == 3 )
				event.target = event.target.parentNode;

			// Add relatedTarget, if necessary
			if ( !event.relatedTarget && event.fromElement )
				event.relatedTarget = event.fromElement == event.target ? event.toElement : event.fromElement;

			// Calculate pageX/Y if missing and clientX/Y available
			if ( event.pageX == null && event.clientX != null ) {
				var doc = document.documentElement, body = document.body;
				event.pageX = event.clientX + (doc && doc.scrollLeft || body && body.scrollLeft || 0) - (doc.clientLeft || 0);
				event.pageY = event.clientY + (doc && doc.scrollTop || body && body.scrollTop || 0) - (doc.clientTop || 0);
			}

			// Add which for key events
			if ( !event.which && ((event.charCode || event.charCode === 0) ? event.charCode : event.keyCode) )
				event.which = event.charCode || event.keyCode;

			// Add metaKey to non-Mac browsers (use ctrl for PC's and Meta for Macs)
			if ( !event.metaKey && event.ctrlKey )
				event.metaKey = event.ctrlKey;

			// Add which for click: 1 == left; 2 == middle; 3 == right
			// Note: button is not normalized, so don't use it
			if ( !event.which && event.button )
				event.which = (event.button & 1 ? 1 : ( event.button & 2 ? 3 : ( event.button & 4 ? 2 : 0 ) ));

			return event;
		},

		proxy: function( fn, proxy ){
			// Set the guid of unique handler to the same of original handler, so it can be removed
			proxy.guid = fn.guid = fn.guid || proxy.guid || this.guid++;
			// So proxy can be declared as an argument
			return proxy;
		},

		special: {
			ready: {
				setup: function() {
					// Make sure the ready event is setup
					bindReady();
					return;
				},

				teardown: function() { return; }
			},

			mouseenter: {
				setup: function() {
					if ( $ektron.browser.msie ) return false;
					$ektron(this).bind("mouseover", $ektron.event.special.mouseenter.handler);
					return true;
				},

				teardown: function() {
					if ( $ektron.browser.msie ) return false;
					$ektron(this).unbind("mouseover", $ektron.event.special.mouseenter.handler);
					return true;
				},

				handler: function(event) {
					// If we actually just moused on to a sub-element, ignore it
					if ( withinElement(event, this) ) return true;
					// Execute the right handlers by setting the event type to mouseenter
					event.type = "mouseenter";
					return $ektron.event.handle.apply(this, arguments);
				}
			},

			mouseleave: {
				setup: function() {
					if ( $ektron.browser.msie ) return false;
					$ektron(this).bind("mouseout", $ektron.event.special.mouseleave.handler);
					return true;
				},

				teardown: function() {
					if ( $ektron.browser.msie ) return false;
					$ektron(this).unbind("mouseout", $ektron.event.special.mouseleave.handler);
					return true;
				},

				handler: function(event) {
					// If we actually just moused on to a sub-element, ignore it
					if ( withinElement(event, this) ) return true;
					// Execute the right handlers by setting the event type to mouseleave
					event.type = "mouseleave";
					return $ektron.event.handle.apply(this, arguments);
				}
			}
		}
	};

	$ektron.fn.extend({
		bind: function( type, data, fn ) {
			return type == "unload" ? this.one(type, data, fn) : this.each(function(){
				$ektron.event.add( this, type, fn || data, fn && data );
			});
		},

		one: function( type, data, fn ) {
			var one = $ektron.event.proxy( fn || data, function(event) {
				$ektron(this).unbind(event, one);
				return (fn || data).apply( this, arguments );
			});
			return this.each(function(){
				$ektron.event.add( this, type, one, fn && data);
			});
		},

		unbind: function( type, fn ) {
			return this.each(function(){
				$ektron.event.remove( this, type, fn );
			});
		},

		trigger: function( type, data, fn ) {
			return this.each(function(){
				$ektron.event.trigger( type, data, this, true, fn );
			});
		},

		triggerHandler: function( type, data, fn ) {
			return this[0] && $ektron.event.trigger( type, data, this[0], false, fn );
		},

		toggle: function( fn ) {
			// Save reference to arguments for access in closure
			var args = arguments, i = 1;

			// link all the functions, so any of them can unbind this click handler
			while( i < args.length )
				$ektron.event.proxy( fn, args[i++] );

			return this.click( $ektron.event.proxy( fn, function(event) {
				// Figure out which function to execute
				this.lastToggle = ( this.lastToggle || 0 ) % i;

				// Make sure that clicks stop
				event.preventDefault();

				// and execute the function
				return args[ this.lastToggle++ ].apply( this, arguments ) || false;
			}));
		},

		hover: function(fnOver, fnOut) {
			return this.bind('mouseenter', fnOver).bind('mouseleave', fnOut);
		},

		ready: function(fn) {
			// Attach the listeners
			bindReady();

			// If the DOM is already ready
			if ( $ektron.isReady )
				// Execute the function immediately
				fn.call( document, $ektron );

			// Otherwise, remember the function for later
			else
				// Add the function to the wait list
				$ektron.readyList.push( function() { return fn.call(this, $ektron); } );

			return this;
		}
	});

	$ektron.extend({
		isReady: false,
		readyList: [],
		// Handle when the DOM is ready
		ready: function() {
			// Make sure that the DOM is not already loaded
			if ( !$ektron.isReady ) {
				// Remember that the DOM is ready
				$ektron.isReady = true;

				// If there are functions bound, to execute
				if ( $ektron.readyList ) {
					// Execute all of them
					$ektron.each( $ektron.readyList, function(){
						this.call( document );
					});

					// Reset the list of functions
					$ektron.readyList = null;
				}

				// Trigger any bound ready events
				$ektron(document).triggerHandler("ready");
			}
		}
	});

	var readyBound = false;

	function bindReady(){
		if ( readyBound ) return;
		readyBound = true;

		// Mozilla, Opera (see further below for it) and webkit nightlies currently support this event
		if ( document.addEventListener && !$ektron.browser.opera)
			// Use the handy event callback
			document.addEventListener( "DOMContentLoaded", $ektron.ready, false );

		// If IE is used and is not in a frame
		// Continually check to see if the document is ready
		if ( $ektron.browser.msie && window == top ) (function(){
			if ($ektron.isReady) return;
			try {
				// If IE is used, use the trick by Diego Perini
				// http://javascript.nwbox.com/IEContentLoaded/
				document.documentElement.doScroll("left");
			} catch( error ) {
				setTimeout( arguments.callee, 0 );
				return;
			}
			// and execute any waiting functions
			$ektron.ready();
		})();

		if ( $ektron.browser.opera )
			document.addEventListener( "DOMContentLoaded", function () {
				if ($ektron.isReady) return;
				for (var i = 0; i < document.styleSheets.length; i++)
					if (document.styleSheets[i].disabled) {
						setTimeout( arguments.callee, 0 );
						return;
					}
				// and execute any waiting functions
				$ektron.ready();
			}, false);

		if ( $ektron.browser.safari ) {
			var numStyles;
			(function(){
				if ($ektron.isReady) return;
				if ( document.readyState != "loaded" && document.readyState != "complete" ) {
					setTimeout( arguments.callee, 0 );
					return;
				}
				if ( numStyles === undefined )
					numStyles = $ektron("style, link[rel='stylesheet']").length;
				if ( document.styleSheets.length != numStyles ) {
					setTimeout( arguments.callee, 0 );
					return;
				}
				// and execute any waiting functions
				$ektron.ready();
			})();
		}

		// A fallback to window.onload, that will always work
		$ektron.event.add( window, "load", $ektron.ready );
	}

	$ektron.each( ("blur,focus,load,resize,scroll,unload,click,dblclick," +
		"mousedown,mouseup,mousemove,mouseover,mouseout,change,select," +
		"submit,keydown,keypress,keyup,error").split(","), function(i, name){

		// Handle event binding
		$ektron.fn[name] = function(fn){
			return fn ? this.bind(name, fn) : this.trigger(name);
		};
	});

	// Checks if an event happened on an element within another element
	// Used in $ektron.event.special.mouseenter and mouseleave handlers
	var withinElement = function(event, elem) {
		// Check if mouse(over|out) are still within the same parent element
		var parent = event.relatedTarget;
		// Traverse up the tree
		while ( parent && parent != elem ) try { parent = parent.parentNode; } catch(error) { parent = elem; }
		// Return true if we actually just moused on to a sub-element
		return parent == elem;
	};

	// Prevent memory leaks in IE
	// And prevent errors on refresh with events like mouseover in other browsers
	// Window isn't included so as not to unbind existing unload events
	$ektron(window).bind("unload", function() {
		$ektron("*").add(document).unbind();
	});
	$ektron.fn.extend({
		// Keep a copy of the old load
		_load: $ektron.fn.load,

		load: function( url, params, callback ) {
			if ( typeof url != 'string' )
				return this._load( url );

			var off = url.indexOf(" ");
			if ( off >= 0 ) {
				var selector = url.slice(off, url.length);
				url = url.slice(0, off);
			}

			callback = callback || function(){};

			// Default to a GET request
			var type = "GET";

			// If the second parameter was provided
			if ( params )
				// If it's a function
				if ( $ektron.isFunction( params ) ) {
					// We assume that it's the callback
					callback = params;
					params = null;

				// Otherwise, build a param string
				} else {
					params = $ektron.param( params );
					type = "POST";
				}

			var self = this;

			// Request the remote document
			$ektron.ajax({
				url: url,
				type: type,
				dataType: "html",
				data: params,
				complete: function(res, status){
					// If successful, inject the HTML into all the matched elements
					if ( status == "success" || status == "notmodified" )
						// See if a selector was specified
						self.html( selector ?
							// Create a dummy div to hold the results
							$ektron("<div/>")
								// inject the contents of the document in, removing the scripts
								// to avoid any 'Permission Denied' errors in IE
								.append(res.responseText.replace(/<script(.|\s)*?\/script>/g, ""))

								// Locate the specified elements
								.find(selector) :

							// If not, just inject the full result
							res.responseText );

					self.each( callback, [res.responseText, status, res] );
				}
			});
			return this;
		},

		serialize: function() {
			return $ektron.param(this.serializeArray());
		},
		serializeArray: function() {
			return this.map(function(){
				return $ektron.nodeName(this, "form") ?
					$ektron.makeArray(this.elements) : this;
			})
			.filter(function(){
				return this.name && !this.disabled &&
					(this.checked || /select|textarea/i.test(this.nodeName) ||
						/text|hidden|password/i.test(this.type));
			})
			.map(function(i, elem){
				var val = $ektron(this).val();
				return val == null ? null :
					val.constructor == Array ?
						$ektron.map( val, function(val, i){
							return {name: elem.name, value: val};
						}) :
						{name: elem.name, value: val};
			}).get();
		}
	});

	// Attach a bunch of functions for handling common AJAX events
	$ektron.each( "ajaxStart,ajaxStop,ajaxComplete,ajaxError,ajaxSuccess,ajaxSend".split(","), function(i,o){
		$ektron.fn[o] = function(f){
			return this.bind(o, f);
		};
	});

	var jsc = now();

	$ektron.extend({
		get: function( url, data, callback, type ) {
			// shift arguments if data argument was ommited
			if ( $ektron.isFunction( data ) ) {
				callback = data;
				data = null;
			}

			return $ektron.ajax({
				type: "GET",
				url: url,
				data: data,
				success: callback,
				dataType: type
			});
		},

		getScript: function( url, callback ) {
			return $ektron.get(url, null, callback, "script");
		},

		getJSON: function( url, data, callback ) {
			return $ektron.get(url, data, callback, "json");
		},

		post: function( url, data, callback, type ) {
			if ( $ektron.isFunction( data ) ) {
				callback = data;
				data = {};
			}

			return $ektron.ajax({
				type: "POST",
				url: url,
				data: data,
				success: callback,
				dataType: type
			});
		},

		ajaxSetup: function( settings ) {
			$ektron.extend( $ektron.ajaxSettings, settings );
		},

		ajaxSettings: {
			url: location.href,
			global: true,
			type: "GET",
			timeout: 0,
			contentType: "application/x-www-form-urlencoded",
			processData: true,
			async: true,
			data: null,
			username: null,
			password: null,
			accepts: {
				xml: "application/xml, text/xml",
				html: "text/html",
				script: "text/javascript, application/javascript",
				json: "application/json, text/javascript",
				text: "text/plain",
				_default: "*/*"
			}
		},

		// Last-Modified header cache for next request
		lastModified: {},

		ajax: function( s ) {
			// Extend the settings, but re-extend 's' so that it can be
			// checked again later (in the test suite, specifically)
			s = $ektron.extend(true, s, $ektron.extend(true, {}, $ektron.ajaxSettings, s));

			var jsonp, jsre = /=\?(&|$)/g, status, data,
				type = s.type.toUpperCase();

			// convert data if not already a string
			if ( s.data && s.processData && typeof s.data != "string" )
				s.data = $ektron.param(s.data);

			// Handle JSONP Parameter Callbacks
			if ( s.dataType == "jsonp" ) {
				if ( type == "GET" ) {
					if ( !s.url.match(jsre) )
						s.url += (s.url.match(/\?/) ? "&" : "?") + (s.jsonp || "callback") + "=?";
				} else if ( !s.data || !s.data.match(jsre) )
					s.data = (s.data ? s.data + "&" : "") + (s.jsonp || "callback") + "=?";
				s.dataType = "json";
			}

			// Build temporary JSONP function
			if ( s.dataType == "json" && (s.data && s.data.match(jsre) || s.url.match(jsre)) ) {
				jsonp = "jsonp" + jsc++;

				// Replace the =? sequence both in the query string and the data
				if ( s.data )
					s.data = (s.data + "").replace(jsre, "=" + jsonp + "$1");
				s.url = s.url.replace(jsre, "=" + jsonp + "$1");

				// We need to make sure
				// that a JSONP style response is executed properly
				s.dataType = "script";

				// Handle JSONP-style loading
				window[ jsonp ] = function(tmp){
					data = tmp;
					success();
					complete();
					// Garbage collect
					window[ jsonp ] = undefined;
					try{ delete window[ jsonp ]; } catch(e){}
					if ( head )
						head.removeChild( script );
				};
			}

			if ( s.dataType == "script" && s.cache == null )
				s.cache = false;

			if ( s.cache === false && type == "GET" ) {
				var ts = now();
				// try replacing _= if it is there
				var ret = s.url.replace(/(\?|&)_=.*?(&|$)/, "$1_=" + ts + "$2");
				// if nothing was replaced, add timestamp to the end
				s.url = ret + ((ret == s.url) ? (s.url.match(/\?/) ? "&" : "?") + "_=" + ts : "");
			}

			// If data is available, append data to url for get requests
			if ( s.data && type == "GET" ) {
				s.url += (s.url.match(/\?/) ? "&" : "?") + s.data;

				// IE likes to send both get and post data, prevent this
				s.data = null;
			}

			// Watch for a new set of requests
			if ( s.global && ! $ektron.active++ )
				$ektron.event.trigger( "ajaxStart" );

			// Matches an absolute URL, and saves the domain
			var remote = /^(?:\w+:)?\/\/([^\/?#]+)/;

			// If we're requesting a remote document
			// and trying to load JSON or Script with a GET
			if ( s.dataType == "script" && type == "GET"
					&& remote.test(s.url) && remote.exec(s.url)[1] != location.host ){
				var head = document.getElementsByTagName("head")[0];
				var script = document.createElement("script");
				script.src = s.url;
				if (s.scriptCharset)
					script.charset = s.scriptCharset;

				// Handle Script loading
				if ( !jsonp ) {
					var done = false;

					// Attach handlers for all browsers
					script.onload = script.onreadystatechange = function(){
						if ( !done && (!this.readyState ||
								this.readyState == "loaded" || this.readyState == "complete") ) {
							done = true;
							success();
							complete();
							head.removeChild( script );
						}
					};
				}

				head.appendChild(script);

				// We handle everything using the script element injection
				return undefined;
			}

			var requestDone = false;

			// Create the request object; Microsoft failed to properly
			// implement the XMLHttpRequest in IE7, so we use the ActiveXObject when it is available
			var xhr = window.ActiveXObject ? new ActiveXObject("Microsoft.XMLHTTP") : new XMLHttpRequest();

			// Open the socket
			// Passing null username, generates a login popup on Opera (#2865)
			if( s.username )
				xhr.open(type, s.url, s.async, s.username, s.password);
			else
				xhr.open(type, s.url, s.async);

			// Need an extra try/catch for cross domain requests in Firefox 3
			try {
				// Set the correct header, if data is being sent
				if ( s.data )
					xhr.setRequestHeader("Content-Type", s.contentType);

				// Set the If-Modified-Since header, if ifModified mode.
				if ( s.ifModified )
					xhr.setRequestHeader("If-Modified-Since",
						$ektron.lastModified[s.url] || "Thu, 01 Jan 1970 00:00:00 GMT" );

				// Set header so the called script knows that it's an XMLHttpRequest
				xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");

				// Set the Accepts header for the server, depending on the dataType
				xhr.setRequestHeader("Accept", s.dataType && s.accepts[ s.dataType ] ?
					s.accepts[ s.dataType ] + ", */*" :
					s.accepts._default );
			} catch(e){}

			// Allow custom headers/mimetypes
			if ( s.beforeSend && s.beforeSend(xhr, s) === false ) {
				// cleanup active request counter
				s.global && $ektron.active--;
				// close opended socket
				xhr.abort();
				return false;
			}

			if ( s.global )
				$ektron.event.trigger("ajaxSend", [xhr, s]);

			// Wait for a response to come back
			var onreadystatechange = function(isTimeout){
				// The transfer is complete and the data is available, or the request timed out
				if ( !requestDone && xhr && (xhr.readyState == 4 || isTimeout == "timeout") ) {
					requestDone = true;

					// clear poll interval
					if (ival) {
						clearInterval(ival);
						ival = null;
					}

					status = isTimeout == "timeout" && "timeout" ||
						!$ektron.httpSuccess( xhr ) && "error" ||
						s.ifModified && $ektron.httpNotModified( xhr, s.url ) && "notmodified" ||
						"success";

					if ( status == "success" ) {
						// Watch for, and catch, XML document parse errors
						try {
							// process the data (runs the xml through httpData regardless of callback)
							data = $ektron.httpData( xhr, s.dataType, s.dataFilter );
						} catch(e) {
							status = "parsererror";
						}
					}

					// Make sure that the request was successful or notmodified
					if ( status == "success" ) {
						// Cache Last-Modified header, if ifModified mode.
						var modRes;
						try {
							modRes = xhr.getResponseHeader("Last-Modified");
						} catch(e) {} // swallow exception thrown by FF if header is not available

						if ( s.ifModified && modRes )
							$ektron.lastModified[s.url] = modRes;

						// JSONP handles its own success callback
						if ( !jsonp )
							success();
					} else
						$ektron.handleError(s, xhr, status);

					// Fire the complete handlers
					complete();

					// Stop memory leaks
					if ( s.async )
						xhr = null;
				}
			};

			if ( s.async ) {
				// don't attach the handler to the request, just poll it instead
				var ival = setInterval(onreadystatechange, 13);

				// Timeout checker
				if ( s.timeout > 0 )
					setTimeout(function(){
						// Check to see if the request is still happening
						if ( xhr ) {
							// Cancel the request
							xhr.abort();

							if( !requestDone )
								onreadystatechange( "timeout" );
						}
					}, s.timeout);
			}

			// Send the data
			try {
				xhr.send(s.data);
			} catch(e) {
				$ektron.handleError(s, xhr, null, e);
			}

			// firefox 1.5 doesn't fire statechange for sync requests
			if ( !s.async )
				onreadystatechange();

			function success(){
				// If a local callback was specified, fire it and pass it the data
				if ( s.success )
					s.success( data, status );

				// Fire the global callback
				if ( s.global )
					$ektron.event.trigger( "ajaxSuccess", [xhr, s] );
			}

			function complete(){
				// Process result
				if ( s.complete )
					s.complete(xhr, status);

				// The request was completed
				if ( s.global )
					$ektron.event.trigger( "ajaxComplete", [xhr, s] );

				// Handle the global AJAX counter
				if ( s.global && ! --$ektron.active )
					$ektron.event.trigger( "ajaxStop" );
			}

			// return XMLHttpRequest to allow aborting the request etc.
			return xhr;
		},

		handleError: function( s, xhr, status, e ) {
			// If a local callback was specified, fire it
			if ( s.error ) s.error( xhr, status, e );

			// Fire the global callback
			if ( s.global )
				$ektron.event.trigger( "ajaxError", [xhr, s, e] );
		},

		// Counter for holding the number of active queries
		active: 0,

		// Determines if an XMLHttpRequest was successful or not
		httpSuccess: function( xhr ) {
			try {
				// IE error sometimes returns 1223 when it should be 204 so treat it as success, see #1450
				return !xhr.status && location.protocol == "file:" ||
					( xhr.status >= 200 && xhr.status < 300 ) || xhr.status == 304 || xhr.status == 1223 ||
					$ektron.browser.safari && xhr.status == undefined;
			} catch(e){}
			return false;
		},

		// Determines if an XMLHttpRequest returns NotModified
		httpNotModified: function( xhr, url ) {
			try {
				var xhrRes = xhr.getResponseHeader("Last-Modified");

				// Firefox always returns 200. check Last-Modified date
				return xhr.status == 304 || xhrRes == $ektron.lastModified[url] ||
					$ektron.browser.safari && xhr.status == undefined;
			} catch(e){}
			return false;
		},

		httpData: function( xhr, type, filter ) {
			var ct = xhr.getResponseHeader("content-type"),
				xml = type == "xml" || !type && ct && ct.indexOf("xml") >= 0,
				data = xml ? xhr.responseXML : xhr.responseText;

			if ( xml && data.documentElement.tagName == "parsererror" )
				throw "parsererror";

			// Allow a pre-filtering function to sanitize the response
			if( filter )
				data = filter( data, type );

			// If the type is "script", eval it in global context
			if ( type == "script" )
				$ektron.globalEval( data );

			// Get the JavaScript object, if JSON is used.
			if ( type == "json" )
				data = eval("(" + data + ")");

			return data;
		},

		// Serialize an array of form elements or a set of
		// key/values into a query string
		param: function( a ) {
			var s = [];

			// If an array was passed in, assume that it is an array
			// of form elements
			if ( a.constructor == Array || a.jquery )
				// Serialize the form elements
				$ektron.each( a, function(){
					s.push( encodeURIComponent(this.name) + "=" + encodeURIComponent( this.value ) );
				});

			// Otherwise, assume that it's an object of key/value pairs
			else
				// Serialize the key/values
				for ( var j in a )
					// If the value is an array then the key names need to be repeated
					if ( a[j] && a[j].constructor == Array )
						$ektron.each( a[j], function(){
							s.push( encodeURIComponent(j) + "=" + encodeURIComponent( this ) );
						});
					else
						s.push( encodeURIComponent(j) + "=" + encodeURIComponent( $ektron.isFunction(a[j]) ? a[j]() : a[j] ) );

			// Return the resulting serialization
			return s.join("&").replace(/%20/g, "+");
		}

	});
	$ektron.fn.extend({
		show: function(speed,callback){
			return speed ?
				this.animate({
					height: "show", width: "show", opacity: "show"
				}, speed, callback) :

				this.filter(":hidden").each(function(){
					this.style.display = this.oldblock || "";
					if ( $ektron.css(this,"display") == "none" ) {
						var elem = $ektron("<" + this.tagName + " />").appendTo("body");
						this.style.display = elem.css("display");
						// handle an edge condition where css is - div { display:none; } or similar
						if (this.style.display == "none")
							this.style.display = "block";
						elem.remove();
					}
				}).end();
		},

		hide: function(speed,callback){
			return speed ?
				this.animate({
					height: "hide", width: "hide", opacity: "hide"
				}, speed, callback) :

				this.filter(":visible").each(function(){
					this.oldblock = this.oldblock || $ektron.css(this,"display");
					this.style.display = "none";
				}).end();
		},

		// Save the old toggle function
		_toggle: $ektron.fn.toggle,

		toggle: function( fn, fn2 ){
			return $ektron.isFunction(fn) && $ektron.isFunction(fn2) ?
				this._toggle.apply( this, arguments ) :
				fn ?
					this.animate({
						height: "toggle", width: "toggle", opacity: "toggle"
					}, fn, fn2) :
					this.each(function(){
						$ektron(this)[ $ektron(this).is(":hidden") ? "show" : "hide" ]();
					});
		},

		slideDown: function(speed,callback){
			return this.animate({height: "show"}, speed, callback);
		},

		slideUp: function(speed,callback){
			return this.animate({height: "hide"}, speed, callback);
		},

		slideToggle: function(speed, callback){
			return this.animate({height: "toggle"}, speed, callback);
		},

		fadeIn: function(speed, callback){
			return this.animate({opacity: "show"}, speed, callback);
		},

		fadeOut: function(speed, callback){
			return this.animate({opacity: "hide"}, speed, callback);
		},

		fadeTo: function(speed,to,callback){
			return this.animate({opacity: to}, speed, callback);
		},

		animate: function( prop, speed, easing, callback ) {
			var optall = $ektron.speed(speed, easing, callback);

			return this[ optall.queue === false ? "each" : "queue" ](function(){
				if ( this.nodeType != 1)
					return false;

				var opt = $ektron.extend({}, optall), p,
					hidden = $ektron(this).is(":hidden"), self = this;

				for ( p in prop ) {
					if ( prop[p] == "hide" && hidden || prop[p] == "show" && !hidden )
						return opt.complete.call(this);

					if ( p == "height" || p == "width" ) {
						// Store display property
						opt.display = $ektron.css(this, "display");

						// Make sure that nothing sneaks out
						opt.overflow = this.style.overflow;
					}
				}

				if ( opt.overflow != null )
					this.style.overflow = "hidden";

				opt.curAnim = $ektron.extend({}, prop);

				$ektron.each( prop, function(name, val){
					var e = new $ektron.fx( self, opt, name );

					if ( /toggle|show|hide/.test(val) )
						e[ val == "toggle" ? hidden ? "show" : "hide" : val ]( prop );
					else {
						var parts = val.toString().match(/^([+-]=)?([\d+-.]+)(.*)$/),
							start = e.cur(true) || 0;

						if ( parts ) {
							var end = parseFloat(parts[2]),
								unit = parts[3] || "px";

							// We need to compute starting value
							if ( unit != "px" ) {
								self.style[ name ] = (end || 1) + unit;
								start = ((end || 1) / e.cur(true)) * start;
								self.style[ name ] = start + unit;
							}

							// If a +=/-= token was provided, we're doing a relative animation
							if ( parts[1] )
								end = ((parts[1] == "-=" ? -1 : 1) * end) + start;

							e.custom( start, end, unit );
						} else
							e.custom( start, val, "" );
					}
				});

				// For JS strict compliance
				return true;
			});
		},

		queue: function(type, fn){
			if ( $ektron.isFunction(type) || ( type && type.constructor == Array )) {
				fn = type;
				type = "fx";
			}

			if ( !type || (typeof type == "string" && !fn) )
				return queue( this[0], type );

			return this.each(function(){
				if ( fn.constructor == Array )
					queue(this, type, fn);
				else {
					queue(this, type).push( fn );

					if ( queue(this, type).length == 1 )
						fn.call(this);
				}
			});
		},

		stop: function(clearQueue, gotoEnd){
			var timers = $ektron.timers;

			if (clearQueue)
				this.queue([]);

			this.each(function(){
				// go in reverse order so anything added to the queue during the loop is ignored
				for ( var i = timers.length - 1; i >= 0; i-- )
					if ( timers[i].elem == this ) {
						if (gotoEnd)
							// force the next step to be the last
							timers[i](true);
						timers.splice(i, 1);
					}
			});

			// start the next in the queue if the last step wasn't forced
			if (!gotoEnd)
				this.dequeue();

			return this;
		}

	});

	var queue = function( elem, type, array ) {
		if ( elem ){

			type = type || "fx";

			var q = $ektron.data( elem, type + "queue" );

			if ( !q || array )
				q = $ektron.data( elem, type + "queue", $ektron.makeArray(array) );

		}
		return q;
	};

	$ektron.fn.dequeue = function(type){
		type = type || "fx";

		return this.each(function(){
			var q = queue(this, type);

			q.shift();

			if ( q.length )
				q[0].call( this );
		});
	};

	$ektron.extend({

		speed: function(speed, easing, fn) {
			var opt = speed && speed.constructor == Object ? speed : {
				complete: fn || !fn && easing ||
					$ektron.isFunction( speed ) && speed,
				duration: speed,
				easing: fn && easing || easing && easing.constructor != Function && easing
			};

			opt.duration = (opt.duration && opt.duration.constructor == Number ?
				opt.duration :
				$ektron.fx.speeds[opt.duration]) || $ektron.fx.speeds.def;

			// Queueing
			opt.old = opt.complete;
			opt.complete = function(){
				if ( opt.queue !== false )
					$ektron(this).dequeue();
				if ( $ektron.isFunction( opt.old ) )
					opt.old.call( this );
			};

			return opt;
		},

		easing: {
			linear: function( p, n, firstNum, diff ) {
				return firstNum + diff * p;
			},
			swing: function( p, n, firstNum, diff ) {
				return ((-Math.cos(p*Math.PI)/2) + 0.5) * diff + firstNum;
			}
		},

		timers: [],
		timerId: null,

		fx: function( elem, options, prop ){
			this.options = options;
			this.elem = elem;
			this.prop = prop;

			if ( !options.orig )
				options.orig = {};
		}

	});

	$ektron.fx.prototype = {

		// Simple function for setting a style value
		update: function(){
			if ( this.options.step )
				this.options.step.call( this.elem, this.now, this );

			($ektron.fx.step[this.prop] || $ektron.fx.step._default)( this );

			// Set display property to block for height/width animations
			if ( this.prop == "height" || this.prop == "width" )
				this.elem.style.display = "block";
		},

		// Get the current size
		cur: function(force){
			if ( this.elem[this.prop] != null && this.elem.style[this.prop] == null )
				return this.elem[ this.prop ];

			var r = parseFloat($ektron.css(this.elem, this.prop, force));
			return r && r > -10000 ? r : parseFloat($ektron.curCSS(this.elem, this.prop)) || 0;
		},

		// Start an animation from one number to another
		custom: function(from, to, unit){
			this.startTime = now();
			this.start = from;
			this.end = to;
			this.unit = unit || this.unit || "px";
			this.now = this.start;
			this.pos = this.state = 0;
			this.update();

			var self = this;
			function t(gotoEnd){
				return self.step(gotoEnd);
			}

			t.elem = this.elem;

			$ektron.timers.push(t);

			if ( $ektron.timerId == null ) {
				$ektron.timerId = setInterval(function(){
					var timers = $ektron.timers;

					for ( var i = 0; i < timers.length; i++ )
						if ( !timers[i]() )
							timers.splice(i--, 1);

					if ( !timers.length ) {
						clearInterval( $ektron.timerId );
						$ektron.timerId = null;
					}
				}, 13);
			}
		},

		// Simple 'show' function
		show: function(){
			// Remember where we started, so that we can go back to it later
			this.options.orig[this.prop] = $ektron.attr( this.elem.style, this.prop );
			this.options.show = true;

			// Begin the animation
			this.custom(0, this.cur());

			// Make sure that we start at a small width/height to avoid any
			// flash of content
			if ( this.prop == "width" || this.prop == "height" )
				this.elem.style[this.prop] = "1px";

			// Start by showing the element
			$ektron(this.elem).show();
		},

		// Simple 'hide' function
		hide: function(){
			// Remember where we started, so that we can go back to it later
			this.options.orig[this.prop] = $ektron.attr( this.elem.style, this.prop );
			this.options.hide = true;

			// Begin the animation
			this.custom(this.cur(), 0);
		},

		// Each step of an animation
		step: function(gotoEnd){
			var t = now();

			if ( gotoEnd || t > this.options.duration + this.startTime ) {
				this.now = this.end;
				this.pos = this.state = 1;
				this.update();

				this.options.curAnim[ this.prop ] = true;

				var done = true;
				for ( var i in this.options.curAnim )
					if ( this.options.curAnim[i] !== true )
						done = false;

				if ( done ) {
					if ( this.options.display != null ) {
						// Reset the overflow
						this.elem.style.overflow = this.options.overflow;

						// Reset the display
						this.elem.style.display = this.options.display;
						if ( $ektron.css(this.elem, "display") == "none" )
							this.elem.style.display = "block";
					}

					// Hide the element if the "hide" operation was done
					if ( this.options.hide )
						this.elem.style.display = "none";

					// Reset the properties, if the item has been hidden or shown
					if ( this.options.hide || this.options.show )
						for ( var p in this.options.curAnim )
							$ektron.attr(this.elem.style, p, this.options.orig[p]);
				}

				if ( done )
					// Execute the complete function
					this.options.complete.call( this.elem );

				return false;
			} else {
				var n = t - this.startTime;
				this.state = n / this.options.duration;

				// Perform the easing function, defaults to swing
				this.pos = $ektron.easing[this.options.easing || ($ektron.easing.swing ? "swing" : "linear")](this.state, n, 0, 1, this.options.duration);
				this.now = this.start + ((this.end - this.start) * this.pos);

				// Perform the next step of the animation
				this.update();
			}

			return true;
		}

	};

	$ektron.extend( $ektron.fx, {
		speeds:{
			slow: 600,
 			fast: 200,
 			// Default speed
 			def: 400
		},
		step: {
			scrollLeft: function(fx){
				fx.elem.scrollLeft = fx.now;
			},

			scrollTop: function(fx){
				fx.elem.scrollTop = fx.now;
			},

			opacity: function(fx){
				$ektron.attr(fx.elem.style, "opacity", fx.now);
			},

			_default: function(fx){
				fx.elem.style[ fx.prop ] = fx.now + fx.unit;
			}
		}
	});
	// The Offset Method
	// Originally By Brandon Aaron, part of the Dimension Plugin
	// http://jquery.com/plugins/project/dimensions
	$ektron.fn.offset = function() {
		var left = 0, top = 0, elem = this[0], results;

		if ( elem ) with ( $ektron.browser ) {
			var parent       = elem.parentNode,
				offsetChild  = elem,
				offsetParent = elem.offsetParent,
				doc          = elem.ownerDocument,
				safari2      = safari && parseInt(version) < 522 && !/adobeair/i.test(userAgent),
				css          = $ektron.curCSS,
				fixed        = css(elem, "position") == "fixed";

			// Use getBoundingClientRect if available
			if ( elem.getBoundingClientRect ) {
				var box = elem.getBoundingClientRect();

				// Add the document scroll offsets
				add(box.left + Math.max(doc.documentElement.scrollLeft, doc.body.scrollLeft),
					box.top  + Math.max(doc.documentElement.scrollTop,  doc.body.scrollTop));

				// IE adds the HTML element's border, by default it is medium which is 2px
				// IE 6 and 7 quirks mode the border width is overwritable by the following css html { border: 0; }
				// IE 7 standards mode, the border is always 2px
				// This border/offset is typically represented by the clientLeft and clientTop properties
				// However, in IE6 and 7 quirks mode the clientLeft and clientTop properties are not updated when overwriting it via CSS
				// Therefore this method will be off by 2px in IE while in quirksmode
				add( -doc.documentElement.clientLeft, -doc.documentElement.clientTop );

			// Otherwise loop through the offsetParents and parentNodes
			} else {

				// Initial element offsets
				add( elem.offsetLeft, elem.offsetTop );

				// Get parent offsets
				while ( offsetParent ) {
					// Add offsetParent offsets
					add( offsetParent.offsetLeft, offsetParent.offsetTop );

					// Mozilla and Safari > 2 does not include the border on offset parents
					// However Mozilla adds the border for table or table cells
					if ( mozilla && !/^t(able|d|h)$/i.test(offsetParent.tagName) || safari && !safari2 )
						border( offsetParent );

					// Add the document scroll offsets if position is fixed on any offsetParent
					if ( !fixed && css(offsetParent, "position") == "fixed" )
						fixed = true;

					// Set offsetChild to previous offsetParent unless it is the body element
					offsetChild  = /^body$/i.test(offsetParent.tagName) ? offsetChild : offsetParent;
					// Get next offsetParent
					offsetParent = offsetParent.offsetParent;
				}

				// Get parent scroll offsets
				while ( parent && parent.tagName && !/^body|html$/i.test(parent.tagName) ) {
					// Remove parent scroll UNLESS that parent is inline or a table to work around Opera inline/table scrollLeft/Top bug
					if ( !/^inline|table.*$/i.test(css(parent, "display")) )
						// Subtract parent scroll offsets
						add( -parent.scrollLeft, -parent.scrollTop );

					// Mozilla does not add the border for a parent that has overflow != visible
					if ( mozilla && css(parent, "overflow") != "visible" )
						border( parent );

					// Get next parent
					parent = parent.parentNode;
				}

				// Safari <= 2 doubles body offsets with a fixed position element/offsetParent or absolutely positioned offsetChild
				// Mozilla doubles body offsets with a non-absolutely positioned offsetChild
				if ( (safari2 && (fixed || css(offsetChild, "position") == "absolute")) ||
					(mozilla && css(offsetChild, "position") != "absolute") )
						add( -doc.body.offsetLeft, -doc.body.offsetTop );

				// Add the document scroll offsets if position is fixed
				if ( fixed )
					add(Math.max(doc.documentElement.scrollLeft, doc.body.scrollLeft),
						Math.max(doc.documentElement.scrollTop,  doc.body.scrollTop));
			}

			// Return an object with top and left properties
			results = { top: top, left: left };
		}

		function border(elem) {
			add( $ektron.curCSS(elem, "borderLeftWidth", true), $ektron.curCSS(elem, "borderTopWidth", true) );
		}

		function add(l, t) {
			left += parseInt(l, 10) || 0;
			top += parseInt(t, 10) || 0;
		}

		return results;
	};


	$ektron.fn.extend({
		position: function() {
			var left = 0, top = 0, results;

			if ( this[0] ) {
				// Get *real* offsetParent
				var offsetParent = this.offsetParent(),

				// Get correct offsets
				offset       = this.offset(),
				parentOffset = /^body|html$/i.test(offsetParent[0].tagName) ? { top: 0, left: 0 } : offsetParent.offset();

				// Subtract element margins
				// note: when an element has margin: auto the offsetLeft and marginLeft
				// are the same in Safari causing offset.left to incorrectly be 0
				offset.top  -= num( this, 'marginTop' );
				offset.left -= num( this, 'marginLeft' );

				// Add offsetParent borders
				parentOffset.top  += num( offsetParent, 'borderTopWidth' );
				parentOffset.left += num( offsetParent, 'borderLeftWidth' );

				// Subtract the two offsets
				results = {
					top:  offset.top  - parentOffset.top,
					left: offset.left - parentOffset.left
				};
			}

			return results;
		},

		offsetParent: function() {
			var offsetParent = this[0].offsetParent;
			while ( offsetParent && (!/^body|html$/i.test(offsetParent.tagName) && $ektron.css(offsetParent, 'position') == 'static') )
				offsetParent = offsetParent.offsetParent;
			return $ektron(offsetParent);
		}
	});


	// Create scrollLeft and scrollTop methods
	$ektron.each( ['Left', 'Top'], function(i, name) {
		var method = 'scroll' + name;

		$ektron.fn[ method ] = function(val) {
			if (!this[0]) return;

			return val != undefined ?

				// Set the scroll offset
				this.each(function() {
					this == window || this == document ?
						window.scrollTo(
							!i ? val : $ektron(window).scrollLeft(),
							 i ? val : $ektron(window).scrollTop()
						) :
						this[ method ] = val;
				}) :

				// Return the scroll offset
				this[0] == window || this[0] == document ?
					self[ i ? 'pageYOffset' : 'pageXOffset' ] ||
						$ektron.boxModel && document.documentElement[ method ] ||
						document.body[ method ] :
					this[0][ method ];
		};
	});
	// Create innerHeight, innerWidth, outerHeight and outerWidth methods
	$ektron.each([ "Height", "Width" ], function(i, name){

		var tl = i ? "Left"  : "Top",  // top or left
			br = i ? "Right" : "Bottom"; // bottom or right

		// innerHeight and innerWidth
		$ektron.fn["inner" + name] = function(){
			return this[ name.toLowerCase() ]() +
				num(this, "padding" + tl) +
				num(this, "padding" + br);
		};

		// outerHeight and outerWidth
		$ektron.fn["outer" + name] = function(margin) {
			return this["inner" + name]() +
				num(this, "border" + tl + "Width") +
				num(this, "border" + br + "Width") +
				(margin ?
					num(this, "margin" + tl) + num(this, "margin" + br) : 0);
		};

	});})();

	/*
    **************************************
       Ektron Namespace
    **************************************
    */
    Ektron = {};
    /*
    If you are creating a new class, add it to the Ektron namespace.
    USAGE:
    (function(){
        Ektron.MyClass = function MyClass(...)
        {
        }; // constructor
        // shared public
        Ektron.MyClass.sharedPublic = function sharedPublic(...)
        {
        };
        // shared private
        var s_private = [];
        function private()
        {
        };
    })(); // Ektron.MyClass
    */

	/*
    **************************************
       Ektron.RegExp Namespace
    **************************************
    */
    Ektron.RegExp = {};

    Ektron.RegExp.escape = function(s)
	{
		return (s+"").replace(Ektron.RegExp.escape.re, "\\$&");
	};
	Ektron.RegExp.escape.re = /[^\w\s]/g;

	Ektron.RegExp.Char = {};
	Ektron.RegExp.Char.amp = /\&/g;
	Ektron.RegExp.Char.lt = /</g;
	Ektron.RegExp.Char.gt = />/g;
	Ektron.RegExp.Char.apos = /\'/g;
	Ektron.RegExp.Char.quot = /\"/g;
	Ektron.RegExp.Char.lf = /\n/g;
	Ektron.RegExp.Char.cr = /\r/g;
	Ektron.RegExp.Char.backslash = /\\/g;

	Ektron.RegExp.Entity = {};
	Ektron.RegExp.Entity.amp = /\&amp;/g;
	Ektron.RegExp.Entity.lt = /\&lt;/g;
	Ektron.RegExp.Entity.gt = /\&gt;/g;
	Ektron.RegExp.Entity.apos = /\&apos;/g;
	Ektron.RegExp.Entity.quot = /\&quot;/g;

	Ektron.RegExp.CharacterClass = {};

	// IE (as of IE 7) omits non-breaking space and other Unicode space separators in "\s".
	Ektron.RegExp.CharacterClass.s = "[\t\x0b\f \xa0\u1680\u180e\u2000-\u200a\u202f\u205f\u3000\n\r\u2028\u2029]";
	/*
	ECMA-262 3rd Edition - December 1999

	15.10.2.12 CharacterClassEscape
	The production CharacterClassEscape :: s evaluates by returning the set of characters containing the
	characters that are on the right-hand side of the WhiteSpace (7.2) or LineTerminator (7.3)
	productions.

	7.2 White Space
	\u0009 Tab <TAB>
	\u000B Vertical Tab <VT>
	\u000C Form Feed <FF>
	\u0020 Space <SP>
	\u00A0 No-break space <NBSP>
	Other category �Zs� Any other Unicode
	�space separator�

	7.3 Line Terminator
	\u000A Line Feed <LF>
	\u000D Carriage Return <CR>
	\u2028 Line separator <LS>
	\u2029 Paragraph separator <PS>

	Unicode Regular Expressions http://unicode.org/reports/tr18/
	Unicode Regular Expression Guidelines http://unicode.org/reports/tr18/tr18-6d2.html
	UNICODE CHARACTER DATABASE http://www.unicode.org/Public/UNIDATA/UCD.html
	Revision 5.1.0
	Date 2008-03-25
	http://www.unicode.org/Public/UNIDATA/PropList.txt
	# PropList-5.1.0.txt
	# Date: 2008-03-20, 17:55:27 GMT [MD]
	0020          ; White_Space # Zs       SPACE
	00A0          ; White_Space # Zs       NO-BREAK SPACE
	1680          ; White_Space # Zs       OGHAM SPACE MARK
	180E          ; White_Space # Zs       MONGOLIAN VOWEL SEPARATOR
	2000..200A    ; White_Space # Zs  [11] EN QUAD..HAIR SPACE
	202F          ; White_Space # Zs       NARROW NO-BREAK SPACE
	205F          ; White_Space # Zs       MEDIUM MATHEMATICAL SPACE
	3000          ; White_Space # Zs       IDEOGRAPHIC SPACE

	Reference: http://en.wikipedia.org/wiki/Space_(punctuation)#Table_of_spaces
	*/

	/*
    **************************************
       Ektron.OnException
    **************************************
    */
    Ektron.OnException = function(me, onexception, ex, args)
    /*
    USAGE: (not a class; this is an event, do not use 'new')
        function myFunction(arg1, arg2, ..., onexception)
        {
	        try
	        {
		        // code
	        }
	        catch (ex)
	        {
		        Ektron.OnException(this, onexception, ex, arguments);
	        }
        }
        myObject.myFunction(arg0, arg1, Ektron.OnException.returnValue(null)); // return null if error
        myObject.myFunction(arg0, arg1, Ektron.OnException.returnArgument(0)); // return arg0 if error
        myObject.myFunction(arg0, arg1, Ektron.OnException.returnException); // return error message if error
        myObject.myFunction(arg0, arg1, function(ex, args) // custom response if error
        {
	        return "An error occurred. arg1=" + args[1] + " Error=" + ex.message;
        });
    */
    {
        var returnValue; // default undefined
        var onexceptionList =
        [
			Ektron.onexception // namespace
        ,	me.constructor.onexception // class
        ,	me.onexception // object
        ,	args.callee.onexception // method
        ,	onexception // argument, normally args[args.length-1]
        ];
        for (var i = 0; i < onexceptionList.length; i++)
        {
	        var onexception = onexceptionList[i];
	        if ("function" == typeof onexception && (0 == i || onexception != onexceptionList[i-1]))
	        {
		        var result = onexception.call(me, ex, args);
		        if (typeof result != "undefined") returnValue = result;
	        }
        }
        if ("undefined" == typeof returnValue)
        {
	        throw ex;
        }
        return returnValue;
    };
    Ektron.OnException.exceptionMessage = function(ex)
    {
        var file = ex.fileName || ex.sourceURL || "";
        if (file) file = "\nFile: " + file;
        var line = ex.lineNumber || ex.line || "";
        if (line) line = "\nLine: " + line;
        return ex.message + file + line;
    };
    Ektron.OnException.ignoreException = function(ex, args) { return null; };
    Ektron.OnException.throwException = function(ex, args) { throw ex; };
    Ektron.OnException.returnException = function(ex, args) { return Ektron.OnException.exceptionMessage(ex); };
    Ektron.OnException.returnValue = function(v)
    {
        return function(ex, args) { return v; };
    };
    Ektron.OnException.returnArgument = function(n)
    {
        return function(ex, args) { if (args && args.length > n) return args[n]; };
    };
    Ektron.OnException.alertException = function(ex, args) { alert(Ektron.OnException.exceptionMessage(ex)); };
    Ektron.OnException.consoleException = function(ex, args) { if (console) console.error(Ektron.OnException.exceptionMessage(ex)); /* firebug */ };
    /*Ektron.OnException.debugException = function(ex, args) { debugger; }; Debugger statement is not supported by Safari*/

	/*
    **************************************
       Ektron.Class Singleton Object
    **************************************
    */
    // This function implements multiple inheritance and is compatible with prototype inheritance.
    Ektron.Class =
    {
        functionName : function(fn)
        {
	        if (typeof fn != "function") throw new TypeError("fn must be of type Function");
	        var a = fn.toString().match(/function (\w+)\(/);
	        return (a != null ? a[1] : "anonymous");
        },
        nonEnumerables : ["toLocaleString", "toString", "valueOf"],
        inherits : function (thisObject, objBase, baseClassName)
        /*
        USAGE:
	        Example 1
	        function MyClass(arg1, arg2, ...)
	        {
		        Ektron.Class.inherits(this, new MyBaseClass(arg1, arg2, ...));
		        this.myMethod = function(arg1, arg2, ...)
		        {
			        this.MyBaseClass_myMethod(arg1, arg2, ...); // call base class method
		        };
	        }
	        var objMy = new MyClass(arg1, arg2, arg3);
	        Example 2
	        function MyClass(base, arg1, arg2, ...)
	        {
		        Ektron.Class.inherits(this, base, "base");
		        this.myMethod = function(arg1, arg2, ...)
		        {
			        this.base_myMethod(arg1, arg2, ...); // call base class method
		        };
	        }
	        var objMyBase = new MyBaseClass(arg1, arg2, ...);
	        var objMy = new MyClass(objMyBase, arg1, arg2, arg3);
	        Note: multiple inheritance is supported
        */
        {
	        if (typeof thisObject != "object") throw new TypeError("thisObject must be of type Object");
	        if (null === thisObject) throw new RangeError("thisObject is null");
	        if (typeof objBase != "object") throw new TypeError("objBase must be of type Object");
	        if (null === objBase) throw new RangeError("objBase is null");
	        if (typeof objBase.constructor != "function") throw new TypeError("objBase.constructor must be of type Function");
	        if (typeof baseClassName != "string" && typeof baseClassName != "undefined") throw new TypeError("baseClassName must be of type String or undefined");
	        if ("undefined" == typeof baseClassName)
	        {
		        for (var p in objBase.constructor.prototype)
		        {
			        throw new TypeError("baseClassName must be specified when objBase is derived using prototype");
		        }
	        }
	        if (typeof baseClassName != "string")
	        {
		        baseClassName = Ektron.Class.functionName(objBase.constructor);
	        }
	        if ("anonymous" == baseClassName) throw new TypeError("baseClassName must be specified when objBase constructor is anonymous");
	        // Copy properties from base object to thisObject
	        for (var p in objBase)
	        {
		        if (p != "constructor")
		        {
			        thisObject[p] = objBase[p];
			        if ("function" == typeof objBase[p])
			        {
				        thisObject[baseClassName + "_" + p] = objBase[p];
			        }
		        }
	        }
	        for (var i in Ektron.Class.nonEnumerables)
	        {
		        thisObject[baseClassName + "_" + Ektron.Class.nonEnumerables[i]] = objBase[Ektron.Class.nonEnumerables[i]];
	        }
	        return thisObject;
        },
        overrides : function(baseClassName, methods)
        /*
	        baseClassName: (optional) the (arbitrary) name of the base class, used to access base methods
	        methods: (optional) array of method names (as strings) to override.
			        If undefined, all methods are overridable.
        */
        {
	        if (typeof baseClassName != "string" && typeof baseClassName != "undefined") throw new TypeError("baseClassName must be of type String or undefined");
	        if (typeof baseClassName != "string") baseClassName = Ektron.Class.functionName(objBase.constructor);
	        if (typeof methods != "undefined" && methods != null && methods.constructor != Array) throw new TypeError("methods must be of type Array or undefined");

	        return function (objBase, args)
	        /*
	        USAGE:
		        function MyClass(arg_1, arg_2, ...)
		        {
			        this.myMethod = function(arg_m1, arg_m2, ...)
			        {
				        this.base_myMethod(arg_m1, arg_m2, ...); // call base class method
			        };
		        }
		        MyClass.overrides = Ektron.Class.overrides("base", [ "myMethod" ]);
		        var objMyBase = new MyBaseClass(arg_b1, arg_b2, ...);
		        MyClass.overrides(objMyBase, [arg_1, arg_2, ...]);
	        */
	        {
		        if (typeof objBase != "object") throw new TypeError("objBase must be of type Object");
		        if (null === objBase) throw new RangeError("objBase is null");
		        if (typeof objBase.constructor != "function") throw new TypeError("objBase.constructor must be of type Function");
		        if (typeof args != "object" && typeof args != "undefined") throw new TypeError("args must be of type Array or undefined");
		        if ("undefined" == typeof args) args = [];
		        objBase.constructor = this;
		        var name = "";
		        // Copy base methods in case they are overridden
		        if ("object" == typeof methods && methods.constructor == Array)
		        {
			        for (var i = 0; i < methods.length; i++)
			        {
				        name = baseClassName + "_" + methods[i];
				        if ("undefined" == typeof objBase[name])
				        {
					        objBase[name] = objBase[methods[i]];
				        }
			        }
		        }
		        else
		        {
			        var aryBaseMethods = [];
			        for (var p in objBase)
			        {
				        if (("function" == typeof objBase[p]) && (p != "constructor"))
				        {
					        aryBaseMethods[p] = objBase[p];
				        }
			        }
			        for (var p in aryBaseMethods)
			        {
				        name = baseClassName + "_" + p;
				        if ("undefined" == typeof objBase[name])
				        {
					        objBase[name] = aryBaseMethods[p];
				        }
			        }
			        for (var i in Ektron.Class.nonEnumerables)
			        {
				        name = baseClassName + "_" + Ektron.Class.nonEnumerables[i];
				        if ("undefined" == typeof objBase[name])
				        {
					        objBase[name] = objBase[Ektron.Class.nonEnumerables[i]];
				        }
			        }
		        }
		        // 'this' is the derived class constructor function
		        this.apply(objBase, args);
		        return objBase;
	        };
        }
    };

	Ektron.RegExp.ltrim = new RegExp("^" + Ektron.RegExp.CharacterClass.s + "+");
	Ektron.RegExp.rtrim = new RegExp(Ektron.RegExp.CharacterClass.s + "+$");

    // Ektron speed improvement
    $ektron.trim = function( text )
    {
        return (text+"").replace(Ektron.RegExp.ltrim,"").replace(Ektron.RegExp.rtrim,"");
    };

    /*
    **************************************
       Ektron Extensions to the Library
    **************************************
    */
    $ektron.extend({
        // Left Trim method
        ltrim: function(text) { return (text+"").replace(Ektron.RegExp.ltrim,""); },
        // Right Trim method
        rtrim: function(text) { return (text+"").replace(Ektron.RegExp.rtrim,""); },

        // Method to add new functions to the window.onload event while preserving any existing onload functionality
        addLoadEvent: function(fn)
        {
	        var oldOnLoad = window.onload;
	        if (typeof window.onload != 'function')
	        {
		        window.onload = fn;
	        }
	        else
	        {
		        window.onload = function()
		        {
			        oldOnLoad();
			        fn();
		        }
	        }
        }
    });

    $ektron.fn.extend({
        // Correct uniqueness for all id attributes and assoc labels.
        // Correct uniqueness for all name attributes.
        makeIdentifiersUnique: function(makeUnique)
        // makeUnique: (optional) function that returns unique identifier given an identifier (string)
        {
	        var descendantOrSelf = this.find("*").andSelf();
	        var strUniqueSuffix = Math.floor(Math.random() * 1679616).toString(36); // 4 digit alphanum
	        // makeUnique must ensure the new unique 'id' continues to match <label for>
	        makeUnique = ("function" == typeof makeUnique ? makeUnique : function(id)
	        {
		        // Remove suffix of "_" 4-digit-alphanum (if it exists), then append new suffix
		        return id.replace(/_[0-9a-z]{4}$/,"") + "_" + strUniqueSuffix;
	        });

	        descendantOrSelf.filter("[id]").each(function()
	        {
		        this.id = makeUnique(this.id);
	        });

	        descendantOrSelf.filter("label").each(function()
	        {
		        this.htmlFor = makeUnique(this.htmlFor);
	        });

	        // caution: do not split id & name uniqueness code b/c for <a>, id should equal name,
	        // which won't be the case if the .random() function is called twice.

	        descendantOrSelf.filter("[name]").each(function()
	        {
		        try
		        {
			        if ($ektron.browser.msie)
			        {
				        // Microsoft JScript allows the name to be changed at run time.
				        // HOWEVER!
				        // This does not cause the name in the programming model to change
				        // in the collection of elements, but it does change the name used
				        // for submitting elements. The NAME attribute cannot be set at run time
				        // on elements dynamically created with the createElement method.
				        // To create an element with a name attribute, include the attribute
				        // and value when using the createElement method.
				        var strHTML = this.outerHTML + "";
				        strHTML = strHTML.replace(new RegExp("name=" + this.name, "g"), "name=" + makeUnique(this.name));
				        $ektron(this).replaceWith(strHTML);
			        }
			        else
			        {
				        this.name = makeUnique(this.name);
			        }
		        }
		        catch (ex)
		        {
			        // ignore
		        };
	        });

	        return this;
        }
    });

    // Ektron Overrides of the Library
    Ektron.Class.overrides("jquery", ["clone"]).call(function()
    {
        // override methods
        /* Usage:
	        $ektron().clone(true);
	        $ektron().clone(true).makeIdentifiersUnique();
        */
        this.clone = function(events)
        {
	        var ret = this.jquery_clone.apply(this, arguments);	// pass all arguments in case jquery API changes

	        // Copy .value and .checked attributes of form fields
	        var dstFormElements = ret.find("*").andSelf().filter(":input");
	        if (dstFormElements.length > 0)
	        {
		        var srcFormElements = this.find("*").andSelf().filter(":input");
		        dstFormElements.each(function(i)
		        {
			        $ektron(this).val( srcFormElements.eq(i).val() );
			        if ("checkbox" == this.type || "radio" == this.type)
			        {
				        this.checked = srcFormElements.get(i).checked;
			        }
		        });
	        }
	        return ret;
        };
    }, $ektron.fn);

    Ektron.Class.overrides("jquery", ["handle"]).call(function()
    {
        this.handle = function(event)
        {
	        try
	        {
		        return $ektron.event.jquery_handle.apply(this, arguments); // pass all arguments in case jquery API changes
	        }
	        catch (ex)
	        {
		        return false;
	        }
        };
    }, $ektron.event);
}
alert("A reference to the deprecated file \"EktronJQuery.js\" has been detected.\n\nThis reference should be changed to point to ektron.js\nand registered via code behind.\n\nFor more information please see the Ektron Knowledge Base article at:\nhttp:\/\/dev.ektron.com\/kb_article.aspx?id=20758");
