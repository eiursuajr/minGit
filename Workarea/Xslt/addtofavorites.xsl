<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ekt="http://www.ektron.com/InformationArchitecture" exclude-result-prefixes="ekt">
  <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes" standalone="no"/>

  <xsl:template match="/">
    <script type="text/javascript" src="[[ekapppath]]java/personaldirectory.js"/>
    <input type="hidden" name="[[ekkey]]_inode" id="[[ekkey]]_inode" value="[[eknode]]"/>
    <input type="hidden" name="ekapppath" id="ekapppath" value="[[ekapppath]]"/>
    <link rel="stylesheet" type="text/css" href="/websrc/WorkArea/csslib/personaldirectory.css"></link>
    <span class="Add2FavLinkSpan" id="[[ekkey]]_add2fav"><script type="text/javascript" language="javascript">pdhdlr('checkfav', '[[ekkey]]');</script></span>
  </xsl:template>

</xsl:stylesheet>
