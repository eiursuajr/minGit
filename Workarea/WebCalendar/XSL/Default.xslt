<xsl:stylesheet version="1.0" extension-element-prefixes="ekext"
exclude-result-prefixes="dl xsl"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:dl="urn:datalist"
xmlns:ekext="urn:ektron:extension-object:common">
  <xsl:output method="xml" version="1.0" encoding="UTF-8"
  indent="yes" omit-xml-declaration="yes" />
  <xsl:strip-space elements="*" />
  <xsl:variable name="ektdesignns_fieldlist"
  select="/*/ektdesignpackage_list/fieldlist" />
  <xsl:param name="baseURL" select="''" />
  <xsl:param name="LangType" select="''" />
  <xsl:param name="userTimeZone" select="''" />
  <xsl:param name="userCulture" select="''" />
  <xsl:key name="weekdays" match="datalist[@name='weekdays']/item"
  use="@value" />
  <xsl:key name="numbersuffix"
  match="datalist[@name='numbersuffix']/item" use="@value" />
  <xsl:key name="textnumber"
  match="datalist[@name='textnumber']/item" use="@value" />
  <xsl:key name="months" match="datalist[@name='months']/item"
  use="@value" />
  <dl:root>
    <datalist name="months">
      <item value="1">January</item>
      <item value="2">February</item>
      <item value="3">March</item>
      <item value="4">April</item>
      <item value="5">May</item>
      <item value="6">June</item>
      <item value="7">July</item>
      <item value="8">August</item>
      <item value="9">September</item>
      <item value="10">October</item>
      <item value="11">November</item>
      <item value="12">December</item>
    </datalist>
    <datalist name="weekdays">
      <item value="0">None</item>
      <item value="1">Sunday</item>
      <item value="2">Monday</item>
      <item value="4">Tuesday</item>
      <item value="8">Wednesday</item>
      <item value="16">Thursday</item>
      <item value="32">Friday</item>
      <item value="64">Saturday</item>
      <item value="62">Weekdays</item>
      <item value="65">Weekends</item>
      <item value="127">Day</item>
    </datalist>
    <datalist name="numbersuffix">
      <item value="1">st</item>
      <item value="2">nd</item>
      <item value="3">rd</item>
      <item value="4">th</item>
    </datalist>
    <datalist name="textnumber">
      <item value="-1">last</item>
      <item value="0"></item>
      <item value="1">first</item>
      <item value="2">second</item>
      <item value="3">third</item>
      <item value="4">fourth</item>
      <item value="5">th</item>
    </datalist>
  </dl:root>
  <xsl:variable name="weekdayslist"
  select="document('')/*/dl:*/datalist[@name='weekdays']/item" />
  <xsl:variable name="suffixlist"
  select="document('')/*/dl:*/datalist[@name='numbersuffix']/item" />
  <xsl:variable name="textnumberlist"
  select="document('')/*/dl:*/datalist[@name='textnumber']/item" />
  <xsl:variable name="monthlist"
  select="document('')/*/dl:*/datalist[@name='months']/item" />
  <xsl:variable name="starttime">
    <xsl:choose>
      <xsl:when test="not($userTimeZone = '')">
        <xsl:choose>
          <xsl:when test="function-available('ekext:convertUTCtoLocal')">

            <xsl:value-of select="ekext:convertUTCtoLocal(/root/StartTime, $userTimeZone)" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="/root/StartTime" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="/root/StartTime" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>
  <xsl:variable name="endtime">
    <xsl:choose>
      <xsl:when test="not($userTimeZone = '')">
        <xsl:choose>
          <xsl:when test="function-available('ekext:convertUTCtoLocal')">

            <xsl:value-of select="ekext:convertUTCtoLocal(/root/EndTime, $userTimeZone)" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="/root/EndTime" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="/root/EndTime" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>
  <xsl:variable name="recurrenceendtime">
    <xsl:if test="count(/root/Recurrence/RecurrenceEndDate) &gt; 0">

      <xsl:choose>
        <xsl:when test="not($userTimeZone = '')">
          <xsl:choose>
            <xsl:when test="function-available('ekext:convertUTCtoLocal')">

              <xsl:value-of select="ekext:convertUTCtoLocal(/root/Recurrence/RecurrenceEndDate, $userTimeZone)" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="/root/Recurrence/RecurrenceEndDate" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="/root/Recurrence/RecurrenceEndDate" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:variable>
  <xsl:template match="/" xml:space="preserve">
      
<div class="WebEventDetail">      
<p class="title">     
<xsl:value-of select="/root/DisplayTitle" />   </p>      
<p class="starttime">When:         
<xsl:choose>     
<xsl:when test="function-available('ekext:formatDateTime')">      
<xsl:choose>       
<xsl:when test="/root/IsAllDay = 'True'">        
<xsl:value-of select="ekext:formatDateTime(/root/StartTime, $userCulture, 'D')" />
       </xsl:when>       
<xsl:otherwise>        
<xsl:value-of select="ekext:formatDateTime($starttime, $userCulture, 'f')" />
       </xsl:otherwise>      </xsl:choose>     </xsl:when>     
<xsl:otherwise>      
<xsl:value-of select="$starttime" />     </xsl:otherwise>   
</xsl:choose>    
<xsl:choose>          
<xsl:when test="/root/IsAllDay = 'True'"> (all day)</xsl:when>     
    
<xsl:when test="not(contains(/root/Duration, '.')) or substring-before(/root/Duration, ':') &lt; '1.00'">
      -       
<xsl:choose>       
<xsl:when test="function-available('ekext:formatDateTime')">       

<xsl:value-of select="ekext:formatDateTime($endtime, $userCulture, 't')" />
       </xsl:when>       
<xsl:otherwise>        
<xsl:value-of select="$endtime" />       </xsl:otherwise>     
</xsl:choose>     </xsl:when>     
<xsl:otherwise> -       
<xsl:choose>       
<xsl:when test="function-available('ekext:formatDateTime')">       

<xsl:value-of select="ekext:formatDateTime($endtime, $userCulture, 'f')" />
       </xsl:when>       
<xsl:otherwise>        
<xsl:value-of select="$endtime" />       </xsl:otherwise>     
</xsl:choose>     </xsl:otherwise>    </xsl:choose>        
<xsl:if test="/root/IsVariance = 'True' or (count(/root/Recurrence) &gt; 0 and not(/root/Recurrence/RecurrenceType = 0))">
     
<span class="isrecurring">Recurs</span>    </xsl:if>   </p>      
<xsl:if test="count(/root/Recurrence) &gt; 0">       
<div class="recurrenceinfo">         
<xsl:if test="count(/root/Recurrence/ReoccursDaily) &gt; 0">       
   
<xsl:if test="/root/Recurrence/ReoccursDaily/Multiple &gt; 1">     
 
<p class="interval">Occurs: Every         
<xsl:value-of select="/root/Recurrence/ReoccursDaily/Multiple" />  
    days</p>      </xsl:if>           
<xsl:if test="count(/root/Recurrence/ReoccursDaily/DaysOfWeek) &gt; 0">
       
<p class="daysofweek">Days of the week:               
<ul>         
<xsl:for-each select="/root/Recurrence/ReoccursDaily/DaysOfWeek">  
       
<xsl:variable name="value" select="string(.)" />          
<xsl:variable name="display-value"
select="($weekdayslist[@value=$value])[1]/." />                   
<li>           
<xsl:copy-of select="$display-value/node()" />          </li>      
  </xsl:for-each>        </ul>       </p>      </xsl:if>    
</xsl:if>         
<xsl:if test="count(/root/Recurrence/ReoccursWeekly) &gt; 0">      
    
<xsl:if test="/root/Recurrence/ReoccursWeekly/Multiple &gt; 1">    
  
<p class="interval">Occurs: Every         
<xsl:value-of select="/root/Recurrence/ReoccursWeekly/Multiple" /> 
     weeks</p>      </xsl:if>           
<xsl:if test="count(/root/Recurrence/ReoccursWeekly/DaysOfWeek) &gt; 0">
       
<p class="daysofweek">Days of the week:               
<ul>         
<xsl:for-each select="/root/Recurrence/ReoccursWeekly/DaysOfWeek"> 
       
<xsl:variable name="value" select="string(.)" />         
<xsl:variable name="display-value"
select="($weekdayslist[@value=$value])[1]/." />                 
<li>          
<xsl:copy-of select="$display-value/node()" />         </li>       
</xsl:for-each>        </ul>       </p>      </xsl:if>    
</xsl:if>         
<xsl:if test="count(/root/Recurrence/ReoccursMonthly) &gt; 0">     
     
<p class="interval">Occurs: On the              
<xsl:choose>               
<xsl:when test="/root/Recurrence/ReoccursMonthly/DayOfMonth &lt; 5">
         
<xsl:variable name="value"
select="/root/Recurrence/ReoccursMonthly/DayOfMonth" />         
<xsl:variable name="display-value"
select="($textnumberlist[@value=$value])[1]/." />                 
<xsl:copy-of select="$display-value/node()" />              
</xsl:when>           
<xsl:when test="/root/Recurrence/ReoccursMonthly/DayOfMonth &gt; 4">
         
<xsl:value-of select="/root/Recurrence/ReoccursMonthly/DayOfMonth" />
        
<xsl:variable name="display-value"
select="($textnumberlist[@value=5])[1]/." />                 
<xsl:copy-of select="$display-value/node()" />        </xsl:when>  
       </xsl:choose>             
<xsl:if test="/root/Recurrence/ReoccursMonthly/RepeatMonthBy = 'day'">
        
<xsl:for-each select="/root/Recurrence/ReoccursMonthly/DaysOfWeek">
        
<xsl:variable name="value" select="string(.)" />         
<xsl:variable name="display-value"
select="($weekdayslist[@value=$value])[1]/." />                 
<xsl:copy-of select="$display-value/node()" />             
</xsl:for-each>       </xsl:if> of every             
<xsl:choose>               
<xsl:when test="/root/Recurrence/ReoccursMonthly/Multiple = 2">    
    other</xsl:when>               
<xsl:when test="/root/Recurrence/ReoccursMonthly/Multiple &gt; 2 and /root/Recurrence/ReoccursMonthly/Multiple &lt; 5">
         
<xsl:variable name="value"
select="/root/Recurrence/ReoccursMonthly/Multiple" />         
<xsl:variable name="display-value"
select="($textnumberlist[@value=$value])[1]/." />                 
<xsl:copy-of select="$display-value/node()" />              
</xsl:when>           
<xsl:when test="/root/Recurrence/ReoccursMonthly/Multiple &gt; 4"> 
        
<xsl:value-of select="/root/Recurrence/ReoccursMonthly/Multiple" />
        
<xsl:variable name="display-value"
select="($textnumberlist[@value=5])[1]/." />                 
<xsl:copy-of select="$display-value/node()" />              
</xsl:when>          </xsl:choose>       month           </p>    
</xsl:if>         
<xsl:if test="count(/root/Recurrence/ReoccursYearly) &gt; 0">      
    
<p class="interval">Occurs: On the             
<xsl:choose>               
<xsl:when test="/root/Recurrence/ReoccursYearly/DayOfMonth &lt; 5">
        
<xsl:variable name="dayofmonthvalue"
select="/root/Recurrence/ReoccursYearly/DayOfMonth" />         
<xsl:variable name="display-dayofmonthvalue"
select="($textnumberlist[@value=$dayofmonthvalue])[1]/." />        
        
<xsl:copy-of select="$display-dayofmonthvalue/node()" />           
  </xsl:when>               
<xsl:otherwise>                 
<xsl:choose>                   
<xsl:when test="/root/Recurrence/ReoccursYearly/DayOfMonth &lt; 20">
           
<xsl:value-of select="/root/Recurrence/ReoccursYearly/DayOfMonth" />
          
<xsl:variable name="display-dayofmonthvalue"
select="($suffixlist[@value=5])[1]/." />                     
<xsl:copy-of select="$display-dayofmonthvalue/node()" />           
         </xsl:when>                   
<xsl:when test="/root/Recurrence/ReoccursYearly/DayOfMonth mod 10 &lt; 5">
                     
<xsl:value-of select="/root/Recurrence/ReoccursYearly/DayOfMonth" />
          
<xsl:variable name="dayofmonthvalue"
select="/root/Recurrence/ReoccursYearly/DayOfMonth mod 10" />      
    
<xsl:variable name="display-dayofmonthvalue"
select="($suffixlist[@value=$dayofmonthvalue])[1]/." />           
<xsl:copy-of select="$display-dayofmonthvalue/node()" />           
      </xsl:when>                   
<xsl:otherwise>                     
<xsl:value-of select="/root/Recurrence/ReoccursYearly/DayOfMonth" />
          
<xsl:variable name="display-dayofmonthvalue"
select="($suffixlist[@value=5])[1]/." />                     
<xsl:copy-of select="$display-dayofmonthvalue/node()" />           
         </xsl:otherwise>         </xsl:choose>       
</xsl:otherwise>         </xsl:choose>                 
<xsl:if test="count(/root/Recurrence/ReoccursYearly/DaysOfWeek) &gt; 0">
        
<xsl:for-each select="/root/Recurrence/ReoccursYearly/DaysOfWeek"> 
       
<xsl:variable name="value" select="string(.)" />         
<xsl:variable name="display-value"
select="($weekdayslist[@value=$value])[1]/." />                 
<xsl:copy-of select="$display-value/node()" />       
</xsl:for-each>       </xsl:if> of              
<xsl:variable name="Month"
select="/root/Recurrence/ReoccursYearly/Month" />       
<xsl:variable name="display-Month"
select="($monthlist[@value=$Month])[1]/." />       
<xsl:copy-of select="$display-Month/node()" />      every        
<xsl:choose>               
<xsl:when test="/root/Recurrence/ReoccursYearly/Multiple = 2">     
  other </xsl:when>               
<xsl:when test="/root/Recurrence/ReoccursYearly/Multiple &gt; 2 and /root/Recurrence/ReoccursYearly/Multiple &lt; 5">
         
<xsl:variable name="value"
select="/root/Recurrence/ReoccursYearly/Multiple" />         
<xsl:variable name="display-value"
select="($textnumberlist[@value=$value])[1]/." />                 
<xsl:copy-of select="$display-value/node()" />        </xsl:when>  
        
<xsl:when test="/root/Recurrence/ReoccursYearly/Multiple &gt; 4">  
        
<xsl:value-of select="/root/Recurrence/ReoccursYearly/Multiple" /> 
       
<xsl:variable name="display-value"
select="($suffixlist[@value=5])[1]/." />                 
<xsl:copy-of select="$display-value/node()" />        </xsl:when>  
       </xsl:choose> year       </p>         </xsl:if>         
<xsl:if test="count(/root/Recurrence) &gt; 0 and not(/root/Recurrence/RecurrenceType = 0)">
      
<p class="enddate">             
<xsl:choose>              
<xsl:when test="count(/root/Recurrence/ReoccursDaily) &gt; 0 and /root/Recurrence/ReoccursDaily/numOccurences &gt; 0">
           Occurs:          
<xsl:value-of select="/root/Recurrence/ReoccursDaily/numOccurences" />
       times</xsl:when>              
<xsl:when test="count(/root/Recurrence/ReoccursWeekly) &gt; 0 and /root/Recurrence/ReoccursWeekly/numOccurences &gt; 0">
           Occurs:          
<xsl:value-of select="/root/Recurrence/ReoccursWeekly/numOccurences" />
       times</xsl:when>              
<xsl:when test="count(/root/Recurrence/ReoccursMonthly) &gt; 0 and /root/Recurrence/ReoccursMonthly/numOccurences &gt; 0">
           Occurs:          
<xsl:value-of select="/root/Recurrence/ReoccursMonthly/numOccurences" />
       times</xsl:when>              
<xsl:when test="count(/root/Recurrence/ReoccursYearly) &gt; 0 and /root/Recurrence/ReoccursYearly/numOccurences &gt; 0">
           Occurs:          
<xsl:value-of select="/root/Recurrence/ReoccursYearly/numOccurences" />
       times</xsl:when>              
<xsl:when test="contains(/root/Recurrence/RecurrenceEndDate, '9999')">
        Repeats: Forever</xsl:when>              
<xsl:otherwise> Repeats: Until          
<xsl:choose>          
<xsl:when test="function-available('ekext:formatDateTime')">       
   
<xsl:choose>            
<xsl:when test="contains(ekext:formatDateTime($recurrenceendtime, 1033, 'hh:mmtt'), '12:00AM')">
             
<xsl:value-of select="ekext:formatDateTime($recurrenceendtime, $userCulture, 'D')" />
            </xsl:when>            
<xsl:otherwise>             
<xsl:value-of select="ekext:formatDateTime($recurrenceendtime, $userCulture, 'f')" />
            </xsl:otherwise>           </xsl:choose>         
</xsl:when>          
<xsl:otherwise>           
<xsl:value-of select="$recurrenceendtime" />         
</xsl:otherwise>         </xsl:choose>        </xsl:otherwise>     
 </xsl:choose>          </p>         </xsl:if>       </div>     
</xsl:if>      
<xsl:if test="not(/root/Location = '')">        
<p class="location">Where:     
<xsl:value-of select="/root/Location" />    </p>       </xsl:if>   
  
<p class="description">Description:        
<div class="innerdescription">     
<xsl:value-of select="/root/Description/node()"
disable-output-escaping="yes" />     
<xsl:text>
 
</xsl:text>    </div>      </p>    </div>     
</xsl:template>
</xsl:stylesheet>
