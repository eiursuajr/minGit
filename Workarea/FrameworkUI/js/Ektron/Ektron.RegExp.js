Ektron.RegExp = {};
Ektron.RegExp.escape = function(s) {
    return (s + "").replace(Ektron.RegExp.escape.re, "\\$&");
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
Other category “Zs” Any other Unicode
“space separator”
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

Ektron.RegExp.ltrim = new RegExp("^" + Ektron.RegExp.CharacterClass.s + "+");
Ektron.RegExp.rtrim = new RegExp(Ektron.RegExp.CharacterClass.s + "+$");