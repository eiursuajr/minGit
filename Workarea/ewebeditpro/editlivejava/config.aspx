<%@ Page Language="C#" %>

<?xml version="1.0" encoding="utf-8" ?>
<!--

This file customizes and configures EditLive! for Java.

TIP: this file can be dynamically generated using ASP, JSP or PHP to achieve runtime changes to settings

-->
<editlive>

    <!-- Default content for the editor -->
    <document>   
		<html>
			<head runat="server">
				<% 
                    string strPath = string.Empty;
                    string SitePath = Request.QueryString["sitepath"];
                    if (Request.QueryString["css"] != "")
                    {
                        if (Request.ServerVariables["SERVER_PORT_SECURE"] == "1")
                        {
                            strPath = "https://";
                        }
                        else
                        {
                            strPath = "http://";
                        }

                        strPath = strPath + Request.ServerVariables["SERVER_NAME"];
                        if (Request.ServerVariables["SERVER_PORT"] != "80")
                        {
                            strPath = strPath + ":" + Request.ServerVariables["SERVER_PORT"];
                        }

                        ////strPath = strPath & Request.ServerVariables("PATH_INFO")
                        ////strPath = Replace(strPath, AppPath & "edit.aspx", "") & SitePath
                        strPath = strPath + SitePath + Request.QueryString["css"];
				%>
					<link rel="stylesheet" href="<%=(strPath)%>" type="text/css"/>
				<% 
                    }
				%> 
				<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />				
			</head>
			<body>
			</body>
		</html>
    </document>    
    <!-- 
    Add your Ephox-provided license key here 
    -->
	<!-- #include file="editlive.lic" -->
        
    <!-- 
    Specify the location of the spell checker
    -->
    <spellCheck jar="<%=(Request.QueryString["apppath"])%>ewebeditpro/editlivejava/dictionaries/en_us_4_0.jar" useNotModified="false">
    </spellCheck>
    
    <!-- 
    Specify HTML filter settings 
    -->
    <htmlFilter 
        outputXHTML="true" 
        outputXML="false" 
        indentContent="false"
        logicalEmphasis="true" 
        quoteMarks="false" 
        removeFontTags="false" 
        uppercaseTags="false" 
        uppercaseAttributes="false" 
        wrapLength="0">
    </htmlFilter>
 
    <!-- 
    Specify settings for the Design (WYSIWYG) view(s) of the editor. Set tabPlacement="off" to disable the tabs.
    -->
    <wysiwygEditor 
    	tabPlacement="bottom" 
    	brOnEnter="false" 
    	showDocumentNavigator="false" 
    	disableInlineImageResizing="false" 
    	disableInlineTableResizing="false"
    >
       <!-- Define Custom Tags actions -->
       <!--
       <customTags>
          <doubleClickActions>
             <action.../>
          </doubleClickActions>
       </customTags>
       -->
       <!-- Define additional symbols for the symbol dialog here -->
       <!--
       <symbols></symbols>
       -->
    </wysiwygEditor>
    <!-- 
    Specify settings for the Source (code) view of the editor 
    -->
    <sourceEditor showBodyOnly="true"/>
    
    <!-- 
    Specify options for content that EditLive has detected has been pasted from Microsoft Word 
    -->
    <wordImport styleOption="user_prompt"/>
    
    <!-- 
    Specify options for content that EditLive has detected has been pasted from another HTML document 
    -->
    <htmlImport styleOption="merge_embedded_styles"/>    
    
    <mediaSettings>
        <!-- 
        Specify HTTP upload settings
            'base' is the base URL used to update the 'src' attributes of any local files in the HTML source
            'href' is your server-side script for handling multipart-formdata uploads from ELJ
        -->

        <httpUpload 
            base="../images" 
            href="bin/xt_upload.asp">

            <!-- 
            Specify any additional fields to post with the image data 
            -->
            <!--<httpUploadData name="hello" data="world"/> -->

        </httpUpload>    
        
        <images allowLocalImages="true" allowUserSpecified="true">
            <!-- 
            The list of images which appear in the Insert Image dialog.
            TIP: Dynamically generate this from your database or repository to achieve an easy image library.
            -->
            
            <imageList>                
                    
            </imageList>
            
            <!--
            Specify WebDAV configuration information
            -->
            <!--<webdav>
                <repository 
                    name="Sample" 
                    baseDir="http://www.yourserver.com/webDAV"
                    defaultDir="SampleDir" 
                    webDAVBaseURL="../webDAV"
                />
            </webdav>-->
        </images>
        <multimedia>
            <types>
                <type name="Macromedia Flash" type="application/x-shockwave-flash" extension="swf" allowCustomParams="true" urlParam="movie">
                    <param name="movie" />
                    <param name="quality" />
                    <param name="bgcolor" />
                </type>
                <type name="QuickTime Movie" type="video/quicktime" extension="mov" allowCustomParams="true">
                    <param name="autohref" />
                    <param name="autoplay" />
                    <param name="bgcolor" />
                    <param name="cache" />
                    <param name="controller" />
                    <param name="correction" />
                    <param name="dontflattenwhensaving" />
                    <param name="enablejavascript" />
                    <param name="endtime" />
                    <param name="fov" />
                    <param name="height" />
                    <param name="href" />
                    <param name="kioskmode" />
                    <param name="loop" />
                    <param name="movieid" />
                    <param name="moviename" />
                    <param name="node" />
                    <param name="pan" />
                    <param name="playeveryframe" />
                    <param name="qtsrcchokespeed" />
                    <param name="scale" />
                    <param name="starttime" />
                    <param name="target" />
                    <param name="targetcache" />
                    <param name="tilt" />
                    <param name="urlsubstitute" />
                    <param name="volume" />
                </type>
                    <type name="Window Media" type="application/x-mplayer2" extension="asf" allowCustomParams="true" urlParam="fileName">
                    <param name="animationAtStart" />
                    <param name="autoStart" />
                    <param name="showControls" />
                    <param name="clickToPlay" />
                    <param name="transparentAtStart" />
                </type>
                <type name="Window Media (Streaming)" type="application/x-mplayer2" extension="asx" allowCustomParams="true" urlParam="fileName">
                    <param name="animationAtStart" />
                    <param name="autoStart" />
                    <param name="showControls" />
                    <param name="clickToPlay" />
                    <param name="transparentAtStart" />
                </type>
                <type name="WAV Audio" type="application/x-mplayer2" extension="wav" allowCustomParams="true" />
                <type name="MP3 Audio" type="application/x-mplayer2" extension="mp3" allowCustomParams="true" />
                <type name="AVI" type="application/x-mplayer2" extension="avi" allowCustomParams="true" />
            </types>
            <!--
            Specify WebDAV configuration information
            -->
            <!--
            <webdav>
                <repository 
                    name="Sample" 
                    baseDir="http://www.yourserver.com/webDAV"
                    defaultDir="SampleDir" 
                    webDAVBaseURL="../webDAV"
                />
            </webdav>
            -->        
        </multimedia>
    </mediaSettings>
    
    
    <hyperlinks>
    
        <hyperlinkList>
        </hyperlinkList>
        
        <mailtoList>
        </mailtoList>
        
    </hyperlinks>
    
    <!-- 
    Customize the EditLive! menus
    
    Note: you must display some sort of Ephox copyright statement within your application, only 
    remove the About menu (by setting showAboutMenu="false") if you have correctly attributed Ephox's 
    copyright in the appropriate place(s) within your application.
    -->
    <menuBar showAboutMenu="false">
                   
        <menu name="&amp;Edit">
            <menuItem name="Undo"/>
            <menuItem name="Redo"/>
            <menuSeparator/>
            <menuItem name="Cut"/>
            <menuItem name="Copy"/>
            <menuItem name="Paste"/>
            <menuItem name="PasteSpecial"/>
            <menuSeparator/>
            <menuItem name="Select"/>
            <menuItem name="SelectAll"/>
            <menuSeparator/>
            <menuItem name="Find"/>
            <menuSeparator/>        
        </menu>
        
        <menu name="&amp;View">
            <menuItemGroup name="SourceView"/>
            <menuSeparator/>
            <menuItem name="Popout"/>  
            <menuSeparator/>
            <menuItem name="showDocumentNavigator"/>
            <menuSeparator/>
            <menuItem name="ParagraphMarker"/>        
        </menu>
        
        <menu name="&amp;Insert">
            <menuItem name="InsTable"/>
            <menuSeparator/>
            <menuItem name="HLink"/>
            <menuItem name="Bookmark"/>
            <menuItem name="RemoveHyperlink" />            
            <menuSeparator/>
            <!--
            <menuItem name="ImageServer"/> 
            <menuItem name="InsertObject"/>
            -->
             <customMenuItem 
                name="MyCustomMenuItem"
                text="Files, Images, Hyperlinks"
                action="raiseEvent"
                value="lauchLibraryHTML"
            />
            <customMenuItem 
                name="AddWikiMenuItem"
                text="Add Wiki Link"
                action="raiseEvent"
                value="lauchWikipopup"
            />
            <menuSeparator/>           
            <menuItem name="Symbol"/>
            <menuItem name="HRule"/> 
            <!--
            <menuSeparator/>
            <menuItem name="DateTime"/>              
            <menuSeparator/>
            <menuItem name="insertcomment"/>    
            -->        
        </menu>
        
        <menu name="F&amp;ormat">
            <submenu name="Style"/>
            <submenu name="Face"/>
            <submenu name="Size"/>
            <menuSeparator/>
            <menuItem name="Bold"/>
            <menuItem name="Italic"/>
            <menuItem name="Underline"/>
            <menuSeparator/>
            <menuItemGroup name="Align"/>
            <menuSeparator/>
            <menuItemGroup name="List"/>
            <menuItem name="DecreaseIndent"/>
            <menuItem name="IncreaseIndent"/>
            <menuItem name="PropList"/>
            <menuSeparator/>
            <menuItem name="HighlightColor"/>
            <menuItem name="Color"/>
            <menuSeparator/>
            <menuItemGroup name="Script"/>
            <menuItem name="Strike"/>
            <menuSeparator/>
            <menuItem name="RemoveFormatting"/>
            <menuItem name="FormatPainter"/>       
        </menu>
        
        <menu name="&amp;Tools">
            <menuItem name="Spelling"/>
            <menuItem name="BackgroundSpellChecking"/>
            <menuSeparator/>
            <menuItem name="Accessibility"/>
            <menuSeparator/>
            <menuItem name="WordCount"/>        
        </menu>
        
        <menu name="T&amp;able">
            <menuItem name="InsTable"/>
            <menuItem name="InsRowCol"/>
            <menuItem name="InsCell"/>
            <menuSeparator/>
            <menuItem name="DelRow"/>
            <menuItem name="DelCol"/>
            <menuItem name="DelCell"/>
            <menuSeparator/>
            <menuItem name="Split"/>
            <menuItem name="Merge"/>
            <menuSeparator/>
            <menuItem name="PropCell"/>
            <menuItem name="PropRow"/>
            <menuItem name="PropCol"/>
            <menuItem name="PropTable"/>
            <menuSeparator/>
            <menuItem name="Gridlines"/>
        </menu>
        
    </menuBar>
    
    <!-- 
    Customize the EditLive! toolbars
    -->
    <toolbars>    
        <toolbar name="Command">
        <!--
            <toolbarButton name="New"/>
            <toolbarButton name="Open"/>
            <toolbarButton name="Save"/>
            <toolbarSeparator/>
            <toolbarButton name="Print"/>
            <toolbarSeparator/>
          -->
            <toolbarButton name="Spelling"/>
            <toolbarButton name="BackgroundSpellChecking"/>     
            <toolbarButton name="Find"/>
            <toolbarSeparator/>  
            <toolbarButton name="Cut"/>
            <toolbarButton name="Copy"/>
            <toolbarButton name="Paste"/>
            <toolbarButton name="FormatPainter" /> 
            <toolbarSeparator/>
            <toolbarButton name="Undo"/>
            <toolbarButton name="Redo"/> 
            <toolbarSeparator/>
            <toolbarButton name="HLink"/>
            <!--
            <toolbarButton name="ImageServer"/>        
            -->
            <% if (Request.QueryString["minoptions"] != "true")
               { %>
            <customToolbarButton 
                name="MyCustomToolbarButton"
                text="Files, Images, Hyperlinks"
				imageURL="<%=(Request.QueryString["apppath"] + "ewebeditpro/library.gif")%>"
                action="raiseEvent"
                value="lauchLibraryHTML" />
                <customToolbarButton 
                name="wikiLink"
                text="Add Wiki Link"
				imageURL="<%=(Request.QueryString["apppath"] + "ewebeditpro/btnWiki.gif")%>"
                action="raiseEvent"
                value="lauchWikipopup" />
            <% } %>
            <toolbarButton name="Symbol"/>
            <toolbarButton name="HRule"/>
            <toolbarSeparator/>
            <toolbarButton name="InsTableWizard"/>
            <toolbarButton name="InsRow"/>
            <toolbarButton name="InsCol"/>
            <toolbarButton name="DelRow"/>
            <toolbarButton name="DelCol"/>
            <toolbarButton name="Split"/>
            <toolbarButton name="Merge"/>
            <toolbarButton name="Gridlines"/>  
            <toolbarSeparator/>
            <toolbarButton name="ParagraphMarker"/>
            <toolbarSeparator/>   
            <toolbarButton name="Popout"/>
        </toolbar>
        
        <toolbar name="Format">
            <!-- 
            Styles from any embeddedd or external stylesheets will also be automatically added to the Styles drop-down 
            -->
            <toolbarComboBox name="Style">
                <comboBoxItem name="P" text="Normal"/>
                <comboBoxItem name="H1" text="Heading 1"/>
                <comboBoxItem name="H2" text="Heading 2"/>
                <comboBoxItem name="H3" text="Heading 3"/>
                <comboBoxItem name="H4" text="Heading 4"/>
                <comboBoxItem name="H5" text="Heading 5"/>
                <comboBoxItem name="H6" text="Heading 6"/>
                <comboBoxItem name="PRE" text="Formatted"/>
                <comboBoxItem name="ADDRESS" text="Address"/>
            </toolbarComboBox>
            <!-- 
            You can remove the Font drop-down if you just want users to use Styles.
            The following fonts are part of the Microsoft Core Web Fonts and are available on at least Mac OS X and Windows
            To change the default font, change the embedded style sheet in the 'style' element above.
            
            <toolbarComboBox name="Face">
                <comboBoxItem name="Arial" text="Arial"/>
                <comboBoxItem name="Arial Black" text="Arial Black"/>
                <comboBoxItem name="Arial Narrow" text="Arial Narrow"/>
                <comboBoxItem name="Comic Sans MS" text="Comic Sans MS"/>
                <comboBoxItem name="Courier New" text="Courier New"/>
                <comboBoxItem name="Georgia" text="Georgia"/>
                <comboBoxItem name="Impact" text="Impact"/>
                <comboBoxItem name="Times New Roman" text="Times New Roman"/>
                <comboBoxItem name="Trebuchet MS" text="Trebuchet MS"/>
                <comboBoxItem name="Verdana" text="Verdana"/>
            </toolbarComboBox>
            -->
            <!-- 
            Font Size drop-down 
            
            <toolbarComboBox name="Size">
                <comboBoxItem name="1" text="8pt"/>
                <comboBoxItem name="2" text="10pt"/>
                <comboBoxItem name="3" text="12pt"/>
                <comboBoxItem name="4" text="14pt"/>
                <comboBoxItem name="5" text="18pt"/>
                <comboBoxItem name="6" text="24pt"/>
                <comboBoxItem name="7" text="36pt"/>
            </toolbarComboBox>
           -->
            <toolbarSeparator/>
            <toolbarButton name="Bold"/>
            <toolbarButton name="Italic"/>
            <toolbarButton name="Underline"/>
            <toolbarSeparator/>
            <toolbarButtonGroup name="Align"/>
            <toolbarSeparator/>
            <toolbarButtonGroup name="List"/>
            <toolbarButton name="DecreaseIndent"/>
            <toolbarButton name="IncreaseIndent"/>
            <toolbarSeparator/>
            <toolbarButton name="HighlightColor"/>
            <toolbarButton name="Color"/>
        </toolbar>
    </toolbars>
    
    <!-- 
    Customize the EditLive! shortcut menu
    -->
    <shortcutMenu>
        <shrtMenu>
            <shrtMenuItem name="Undo"/>
            <shrtMenuItem name="Redo"/>
            <shrtMenuSeparator/>
            <shrtMenuItem name="Cut"/>
            <shrtMenuItem name="Copy"/>
            <shrtMenuItem name="Paste"/>
            <shrtMenuSeparator/>
            <shrtMenuItem name="Select"/>   
            <shrtMenuSeparator/>
            <shrtMenuItem name="Hyperlink"/>
            <shrtMenuItem name="RemoveHyperlink"/>
            <shrtMenuItem name="PropImage"/>
            <shrtMenuItem name="PropObject"/>
            <shrtMenuItem name="PropList"/>
            <shrtMenuItem name="PropHR"/>
            <shrtMenuSeparator/>
            <shrtMenuItem name="Split"/>
            <shrtMenuItem name="Merge"/>
            <shrtMenuSeparator/> 
            <shrtMenuItem name="PropTable"/>
            <shrtMenuItem name="PropRow"/>
            <shrtMenuItem name="PropCol"/>
            <shrtMenuItem name="PropCell"/>
            <shrtMenuSeparator/>
            <shrtMenuItem name="EditTag"/>
        </shrtMenu>
    </shortcutMenu>
</editlive>
