/*

jQuery AlphaNumeric
by Paulo P. Marinas
Sponsored by IT Group, Inc.

Did you ever have the need to prevent users from entering certain characters into your form?

Looking at the plugins available at jQuery, I found a great plugin made by Sam Collet called Numeric.

But it was too limited, what if I'm asking the user to create a username? Or what if I need to enter a number with decimals or an IP Address? There is another great plugin called Masked Input by Josh Bush, which can also control user input by defining a mask. The problem however with that was the length of text to be inserted must be defined as well. Again, what if I needed to control input of a username? I can't tell how many characters the user will be using, and I can't force him to use just 8 characters ,so I created AlphaNumeric.

jQuery AlphaNumeric is a javascript control plugin that allows you to limit what characters a user can enter on textboxes or textareas. Have fun. 

Examples

    Allow only alphanumeric characters:  
        $('.sample').alphanumeric();
    
    Allow only alphanumeric characters, and some exceptions like dot(.), comma (,) and space + Toggle Code
        $('.sample').alphanumeric({allow:"., "});
    
    Allow only lowercase alpha characters + Toggle Code
        $('.sample').alpha({nocaps:true});
    
    Allow only numeric characters + Toggle Code
        $('.sample').numeric();
    
    Allow only numeric characters, and some exceptions like dot (.) + Toggle Code
        $('.sample').numeric({allow:"."});
    
    Make a custom rule and define only certain characters to prevent, like dot (.), one (1), and a (a) + Toggle Code
        $('.sample').alphanumeric({ichars:'.1a'});

*/
(function($){

	$.fn.alphanumeric = function(p) { 

		p = $.extend({
			ichars: "!@#$%^&*()+=[]\\\';,/{}|\":<>?~`.- ",
			nchars: "",
			allow: ""
		  }, p);	

		return this.each
			(
				function() 
				{

					if (p.nocaps) p.nchars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
					if (p.allcaps) p.nchars += "abcdefghijklmnopqrstuvwxyz";
					
					s = p.allow.split('');
					for ( i=0;i<s.length;i++) if (p.ichars.indexOf(s[i]) != -1) s[i] = "\\" + s[i];
					p.allow = s.join('|');
					
					var reg = new RegExp(p.allow,'gi');
					var ch = p.ichars + p.nchars;
					ch = ch.replace(reg,'');

					$(this).keypress
						(
							function (e)
								{
								
									if (!e.charCode) k = String.fromCharCode(e.which);
										else k = String.fromCharCode(e.charCode);
										
									if (ch.indexOf(k) != -1) e.preventDefault();
									if (e.ctrlKey&&k=='v') e.preventDefault();
									
								}
								
						);
						
					$(this).bind('contextmenu',function () {return false});
									
				}
			);

	};

	$.fn.numeric = function(p) {
	
		var az = "abcdefghijklmnopqrstuvwxyz";
		az += az.toUpperCase();

		p = $.extend({
			nchars: az
		  }, p);	
		  	
		return this.each (function()
			{
				$(this).alphanumeric(p);
			}
		);
			
	};
	
	$.fn.alpha = function(p) {

		var nm = "1234567890";

		p = $.extend({
			nchars: nm
		  }, p);	

		return this.each (function()
			{
				$(this).alphanumeric(p);
			}
		);
			
	};	

})($ektron);
