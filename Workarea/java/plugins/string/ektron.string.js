if ("undefined" == typeof Ektron.String)
{
	/*
	**************************************
	   Ektron Extensions to the Library
	**************************************
	*/
		
	Ektron.RegExp.Char.leftCurly2 = /\{\{/g;
	Ektron.RegExp.Char.rightCurly2 = /\}\}/g;
	Ektron.RegExp.tags = /<[^>]*>/g;
	
	$ektron.extend({
		htmlEncode: function(text) { return (text+"").replace(Ektron.RegExp.Char.amp, "&amp;").replace(Ektron.RegExp.Char.lt, "&lt;").replace(Ektron.RegExp.Char.gt, "&gt;").replace(Ektron.RegExp.Char.quot, "&quot;"); },
		// htmlEncodeText is optimized for text between tags. The " char is NOT encoded.
		htmlEncodeText: function(text) { return (text+"").replace(Ektron.RegExp.Char.amp, "&amp;").replace(Ektron.RegExp.Char.lt, "&lt;").replace(Ektron.RegExp.Char.gt, "&gt;"); },
		htmlDecode: function(text) { return (text+"").replace(Ektron.RegExp.Entity.amp, "&").replace(Ektron.RegExp.Entity.gt, ">").replace(Ektron.RegExp.Entity.lt, "<").replace(Ektron.RegExp.Entity.quot, "\""); },
		
		// remove all tags
		removeTags: function(html) { return (html+"").replace(Ektron.RegExp.tags, ""); },
		
		toInt: function(value, defaultValue)
		/*
			Converts a string to an integer or a default value if the string is not a number.
			defaultValue is optional.
		*/
		{
			var n = parseInt(value, 10);
			if (isNaN(n)) n = (defaultValue ? defaultValue : 0);
			return n;
		},
		
		toBool: function(value, defaultValue)
		/*
			Converts a value to a boolean or a default value if the string is null or empty
		*/
		{
			if ("string" == typeof value) value = value.toLowerCase();
			switch (value)
			{
				case "true":
				case true: 
				case "1":
				case 1: 
				case -1: 
				case "on":
				case "yes":
				case "y":
				case "t":
					return true;
				case "false":
				case false: 
				case "0":
				case 0: 
				case "off":
				case "no":
				case "n":
				case "f":
					return false;
				default: 
					return ("boolean" == typeof defaultValue) ? defaultValue : false;
			}
		},
		
		toStr: function(value, defaultValue) // can't extend native method toString
		/*
			Converts a value to a string or a default value if the value is null
		*/
		{
			if (value !== null)
			{
				try
				{
					return value.toString();
				}
				catch (ex)
				{
					// fall through to return defaultValue
				}
			}
			return ("string" == typeof defaultValue) ? defaultValue : "";
		},
		
		toLiteral: function(object)
		/*
			Returns a string that is a literal representation of object.
			Very similar to JSON, but allows all the constructs of JavaScript.
		*/
		{
			var sLiteral = "";

			switch (typeof object)
			{
			case "undefined":
				sLiteral = "undefined";
				break;
			case "string":
				sLiteral = '"' + Ektron.String.escape(object) + '"';
				break;
			case "object":
				if (null == object)
				{
					sLiteral = "null";
				}
				else if (typeof object.arguments != "undefined" && typeof object.caller != "undefined" && typeof object.length != "undefined")
				{
					// Function
					sLiteral = object.toString();
				}
				else if (typeof object.sort != "undefined" && typeof object.length != "undefined")
				{
					// Array
					// WARNING: Does not preserve named indexes.
					for (var i = 0; i < object.length; i++)
					{
						if (sLiteral.length > 0)
						{
							sLiteral += ", ";
						}
						sLiteral += arguments.callee(object[i]);
					}
					sLiteral = "[" + sLiteral + "]";
				}
				else
				{
					// Object
					for (var propName in object)
					{
						if (sLiteral.length > 0)
						{
							sLiteral += ", ";
						}
						sLiteral += "'" + propName + "':" + arguments.callee(object[propName]);
					}
					sLiteral = "{" + sLiteral + "}";
				}
				break;
			default:
				sLiteral = object.toString();
			}
			return sLiteral;
		},
        
		// adapted from http://community.hdri.net/blogs/ray_blog/archive/2006/02/27/5.aspx
		formatString : function(format, etc)
		{
			// e.g., formatString("hello {0}", "world") -> "hello world"
			// e.g., formatString("{{0}}={0}", "hi") -> "{0}=hi"
			var args = arguments;
			var start = 1;
			if (2 == arguments.length && arguments[1].constructor == Array)
			{
				args = arguments[1];
				start = 0;
			}
			for (var i = start; i < args.length; i++)
			{
				var re = new RegExp("\\{" + (i-start) + "\\}(?!\\})", "g");
				format = format.replace(re, args[i]);
			}
			return format.replace(Ektron.RegExp.Char.leftCurly2, "{").replace(Ektron.RegExp.Char.rightCurly2, "}");
		}
	});


	/*
	**************************************
	   Ektron String Class
	**************************************
	*/
	Ektron.String = function(value, startIndex, length)
	{
		this.append = function(value, startIndex, length)
		{
			if (startIndex >= 0)
			{
				if (length >= 0)
				{
					m_appended.push((value+"").substring(startIndex, startIndex + length));
				}
				else
				{
					m_appended.push((value+"").substring(startIndex));
				}
			}
			else
			{
				m_appended.push(value+"");
			}
			this.length += m_appended[m_appended.length-1].length;
			return this;
		};
		this.appendFormat = function(format, etc)
		{
			return this.append(Ektron.String.format.apply(Ektron.String, arguments));
		};
		this.appendLine = function(value)
		{
			if (value) this.append(value);
			this.append(this.lineTerminator);
			return this;
		};
		this.charAt = function(n)
		{
			m_concatAppended();
			return m_value.charAt(n);
		};
		this.charCodeAt = function(n)
		{
			m_concatAppended();
			return m_value.charCodeAt(n);
		};
		// TODO consider adding compare or compareTo
		this.concat = function(etc)
		{
			var args = arguments;
			if (1 == arguments.length && arguments[0].constructor == Array)
			{
				args = arguments[0];
			}
			for (var i = 0; i < args.length; i++)
			{
				this.append(args[i]);
			}
			return this;
		};
		this.contains = function(value, ignoreCase)
		{
			m_concatAppended();
			if (true === ignoreCase)
			{
				var re = new RegExp(Ektron.RegExp.escape(value), "i");
				return re.test(m_value);
			}
			else
			{
				return (m_value.indexOf(value) > -1);
			}
		};
		this.endsWith = function(value, ignoreCase)
		{
			m_concatAppended();
			var re = new RegExp(Ektron.RegExp.escape(value) + "$", (true === ignoreCase ? "i" : ""));
			return re.test(m_value);
		};
		this.equals = function(value, ignoreCase)
		{
			m_concatAppended();
			if (true === ignoreCase)
			{
				var re = new RegExp("^" + Ektron.RegExp.escape(value) + "$", "i");
				return re.test(m_value);
			}
			else
			{
				return (m_value == value);
			}
		};
		this.indexOf = function(value, startIndex, ignoreCase)
		{
			m_concatAppended();
			if (true === ignoreCase)
			{
				return m_value.toLowerCase().indexOf(value.toLowerCase(), startIndex);
			}
			else if (true === startIndex)
			{
				return m_value.toLowerCase().indexOf(value.toLowerCase());
			}
			else if ("number" == typeof startIndex)
			{
				return m_value.indexOf(value, startIndex);
			}
			else
			{
				return m_value.indexOf(value);
			}
		};
		this.insert = function(startIndex, value, count)
		// count is number of times to insert 'value'
		{
			if (typeof startIndex != "number")
			{
				count = value;
				value = startIndex;
				startIndex = 0;
			}
			if (0 === count) return this;
			if ("undefined" == typeof count) count = 1;
			m_concatAppended();
			// Split this instance into two parts at 'startIndex'
			var strTail = m_value.substring(startIndex);
			this.substring(0, startIndex);
			for (var i = 0; i < count; i++)
			{
				this.append(value);
			}
			this.append(strTail);
			return this;
		};
		this.lastIndexOf = function(value, startIndex, ignoreCase)
		{
			m_concatAppended();
			if (true === ignoreCase)
			{
				return m_value.toLowerCase().lastIndexOf(value.toLowerCase(), startIndex);
			}
			else if (true === startIndex)
			{
				return m_value.toLowerCase().lastIndexOf(value.toLowerCase());
			}
			else if ("number" == typeof startIndex)
			{
				return m_value.lastIndexOf(value, startIndex);
			}
			else
			{
				return m_value.lastIndexOf(value);
			}
		};
		this.padLeft = function(totalWidth, paddingChar)
		{
			m_concatAppended();
			var n = (totalWidth - m_value.length);
			if (n > 0)
			{
				var ary = new Array(n + 1);
				m_set(ary.join(paddingChar||" ") + m_value);
			}
			return this;
		};
		this.padRight = function(totalWidth, paddingChar)
		{
			m_concatAppended();
			var n = (totalWidth - m_value.length);
			if (n > 0)
			{
				var ary = new Array(n + 1);
				m_set(m_value + ary.join(paddingChar||" "));
			}
			return this;
		};
		this.remove = function(startIndex, length)
		{
			if (0 === length) return this;
			m_concatAppended();
			if (length >= 0)
			{
				var strTail = m_value.substring(startIndex + length);
				this.substring(0, startIndex);
				this.append(strTail);
			}
			else
			{
				this.substring(0, startIndex);
			}
			return this;
		};
		this.replace = function(oldValue, newValue, ignoreCase)
		{
			m_concatAppended();
			var re = new RegExp(Ektron.RegExp.escape(oldValue), "g" + (true === ignoreCase ? "i" : ""));
			m_set(m_value.replace(re, newValue));
			return this;
		};
		this.split = function(separator, count, removeEmptyEntries)
		// count is max number of items to return
		{
			if ("number" == typeof separator) // separator is missing, first arg is count
			{
				removeEmptyEntries = count;
				count = separator;
				separator = null;
			}
			else if ("boolean" == typeof separator) // separator is removeEmptyEntries
			{
				removeEmptyEntries = separator;
				separator = null;
			}
			else if ("boolean" == typeof count) // count is missing, it is removeEmptyEntries
			{
				removeEmptyEntries = count;
				count = -1;
			}
			if ("undefined" == typeof separator || null == separator) separator = "\n";
			if (0 === count) return [];
			m_concatAppended();
			var ary;
			if (count > 0)
			{
				ary = m_value.split(separator, count);
			}
			else
			{
				ary = m_value.split(separator);
			}
			if (null == ary) ary = [];
			if (true === removeEmptyEntries)
			{
				for (var i = 0; i < ary.length; i++)
				{
					if (0 == ary[i].length)
					{
						var n = 1;
						while (i+n < ary.length && 0 == ary[i+n].length)
						{
							n++;
						}
						ary.splice(i, n);
					}
				}
			}
			return ary;
		};
		this.startsWith = function(value, ignoreCase)
		{
			m_concatAppended();
			var re = new RegExp("^" + Ektron.RegExp.escape(value), (true === ignoreCase ? "i" : ""));
			return re.test(m_value);
		};
		this.substring = function(startIndex, length)
		{
			m_concatAppended();
			if (length >= 0)
			{
				m_set(m_value.substring(startIndex, startIndex + length));
			}
			else
			{
				m_set(m_value.substring(startIndex));
			}
			return this;
		};
		this.toLower = function()
		{
			m_concatAppended();
			m_set(m_value.toLowerCase());
			return this;
		};
		this.toString = function()
		{
			m_concatAppended();
			return m_value;
		};
		this.toUpper = function()
		{
			m_concatAppended();
			m_set(m_value.toUpperCase());
			return this;
		};
		this.trim = function(trimChars) 
		{
			m_concatAppended();
			if (trimChars)
			{
				this.trimStart(trimChars).trimEnd(trimChars);
			}
			else
			{
				m_set($ektron.trim(m_value));
			}
			return this;
		};
		this.trimEnd = function(trimChars)
		{
			m_concatAppended();
			if (trimChars)
			{
				var re = new RegExp("[" + Ektron.RegExp.escape(trimChars) + "]+$");
				m_set(m_value.replace(re,""));
			}
			else
			{
				m_set($ektron.rtrim(m_value));
			}
			return this;
		};
		this.trimStart = function(trimChars)
		{
			m_concatAppended();
			if (trimChars)
			{
				var re = new RegExp("^[" + Ektron.RegExp.escape(trimChars) + "]+");
				m_set(m_value.replace(re,""));
			}
			else
			{
				m_set($ektron.ltrim(m_value));
			}
			return this;
		};
		this.length = 0;
		this.lineTerminator = "\n";
		var me = this;
		var m_value = "";
		var m_appended = [];
		function m_set(value)
		{
			m_value = value;
			me.length = m_value.length;
			m_appended = [];
		};
		function m_concatAppended()
		{
			if (m_appended.length >= 1)
			{
				m_value += m_appended.join("");
				m_appended = [];
			}
			if (me.length < m_value.length)
			{
				m_value = m_value.substring(0, me.length);
			}
			else if (me.length > m_value.length)
			{
				var n = (me.length - m_value.length);
				var ary = new Array(n + 1);
				m_value = m_value + ary.join(" ");
			}
		};
		if (value)
		{ 
			m_set(String(value));
			if (startIndex >= 0)
			{
				this.substring(startIndex, length);
			}
		}
	};
    
	Ektron.String.format = $ektron.formatString;
	
	Ektron.String.escape = function(s) 
	{ 
		return (s+"")
			.replace(Ektron.RegExp.Char.backslash, "\\\\")
			.replace(Ektron.RegExp.Char.quot, "\\\"")
			.replace(Ektron.RegExp.Char.apos, "\\'")
			.replace(Ektron.RegExp.Char.lf, "\\n")
			.replace(Ektron.RegExp.Char.cr, ""); 
	};

	Ektron.String.escapeJavaScriptAttributeValue = function(s) 
	{ 
		return (s+"")
			.replace(Ektron.RegExp.Char.quot, "'")
			.replace(Ektron.RegExp.Char.lf, "\\n")
			.replace(Ektron.RegExp.Char.cr, ""); 
	};

}
