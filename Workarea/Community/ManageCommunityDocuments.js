// JScript File
var ControlID = '';
var isloggedIn = true;
function Clearbuffer() {
    var objImageIDField = eval('document.form1.' + ControlID + '_image_ids');
        if (isloggedIn) {
            if (objImageIDField != null)
                objImageIDField.value = "";
            document.getElementById('idbufer').innerHTML = "";
            document.getElementById('clearid').style.display = "none";
            document.getElementById('__buttons_paste').style.display = "none";
            clearcheckboxes(); 
        } else {
            if (objImageIDField != null)
                objImageIDField.value = "";
            if (document.getElementById('idbufer') != null) 
                document.getElementById('idbufer').innerHTML = "";
            if (document.getElementById('clearid') != null)
                document.getElementById('clearid').style.display = "none";
            if (document.getElementById('__buttons_paste') != null)
                document.getElementById('__buttons_paste').style.display = "none";
        }
    }
    
    function MakeUnique()  {
        var u_imageid=new Array();
        var theids,match,i,x,z,arlength;
        z = 0;
        theids = eval('document.form1.' + ControlID + '_image_ids').value;
        if (theids.length > 0) {
            var aryTest = theids.split(',');
            arlength = aryTest.length - 1;
            for(i=0; i < arlength; i++) {
                match = 0;
                for(x=0; x < u_imageid.length; x++) {
                    if (aryTest[i] == u_imageid[x]) {
                        match = 1;
                    }
                 }
                 if (match == 0) {
                   u_imageid[z] = aryTest[i];
                   z = z + 1;
                 }
            }
            eval('document.form1.' + ControlID + '_image_ids').value = "";
            for(x=0; x < u_imageid.length; x++) {
               eval('document.form1.' + ControlID + '_image_ids').value = eval('document.form1.' + ControlID + '_image_ids').value + u_imageid[x] + ','; 
            }
        }
    }
    
    function clearcheckboxes() {
    var max = 0;
        var selcount = 0;
        if (typeof document.form1.imageid == "object") {
            if (typeof document.form1.imageid.length == "undefined")
            {
                max = 1; 
                document.form1.imageid.checked = false;
            }
            else
            {
                max = document.form1.imageid.length; 
                
                for (var idx = 0; idx < max; idx++) {
                    document.form1.imageid[idx].checked = false;
                } 
            }
        }
       
    }
    
       
    function copyimages() {
        var max = 0;
        var selcount = 0;
        if (typeof document.form1.imageid == "object") {
            if (typeof document.form1.imageid.length == "undefined")        {
                max = 1; 
                if (document.form1.imageid.checked == true) {
                       eval('document.form1.' + ControlID + '_image_ids').value = eval('document.form1.' + ControlID + '_image_ids').value + document.form1.imageid.value  + ",";
                       MakeUnique(eval('document.form1.' + ControlID + '_image_ids').value);
                       document.getElementById('idbufer').innerHTML = eval('document.form1.' + ControlID + '_image_ids').value;
                       document.getElementById('clearid').style.display = 'inline';
                       document.getElementById('__buttons_paste').style.display = 'inline';
                    selcount = 1;
                } 
            }
            else
            {
                max = document.form1.imageid.length; 
                
                for (var idx = 0; idx < max; idx++) {
                    if (document.form1.imageid[idx].checked == true) {
                        eval('document.form1.' + ControlID + '_image_ids').value = eval('document.form1.' + ControlID + '_image_ids').value + document.form1.imageid[idx].value  + ",";
                        selcount = selcount + 1;
                        document.getElementById('idbufer').innerHTML = eval('document.form1.' + ControlID + '_image_ids').value;
                        document.getElementById('clearid').style.display = 'inline';
                        document.getElementById('__buttons_paste').style.display = 'inline';
                    }
                }
                MakeUnique(eval('document.form1.' + ControlID + '_image_ids').value);
            }
            
            if (selcount == 0) {
                alert('No images are checked');
            }else
            {
                var value = eval('document.form1.' + ControlID + '_image_ids').value; 
                if (value.lastIndexOf(",") == value.length - 1)
                {
                    value = value.substring(0, value.length - 1);
                }
                value = value.replace(/,/g, ", ");
                document.getElementById('idbufer').innerHTML = value;
            }
        }
    }
    
    function Delete_Items(){
        eval('document.form1.' + ControlID + '_image_ids').value = "";
        copyimages();  // only want images to delete from this page.
        if (eval('document.form1.' + ControlID + '_image_ids').value == "") {
        } else {
            var delControl= document.getElementById(ControlID + "_Button2");
            delControl.click();
        }
   
    }
    
    function pastimages() {
        if (eval('document.form1.' + ControlID + '_image_ids').value == "") {
            alert('You have to have images in the paste buffer');
        } else {
            document.getElementById('idbufer').innerHTML = "";
            document.getElementById('clearid').style.display = "none";
            var addcat = document.getElementById(ControlID + "_Button1");
            addcat.click();
       }
    }
    
    function addtaxcat() {
        if (trim(document.form1.__EkSubCategoryTitle.value) == "") {
            alert('Type in a Category');
        } else {
            var addcat = document.getElementById(ControlID + "_Button3");
            addcat.click();
        }
    }
    function trim(s) { 
        var l=0; var r=s.length -1; 
        while(l < s.length && s[l] == ' ') 
        {     l++; } 
        while(r > l && s[r] == ' ') 
        {     r-=1;     } 
        return s.substring(l, r+1); 
    } 
    
    function ekShowPanel(id, bDisplay)
    {
        if (bDisplay) {
            document.getElementById(id).style.display = 'inline';
            document.getElementById('__EkSubCategoryTitle').focus();
            }
        else {
            document.getElementById(id).style.display = 'none';            
        }
    }
    
    function clickButton(e, ElementID)
    { 
        var evt = e ? e : window.event;                     
        if (evt.keyCode == 13)
        {    
            evt.cancelBubble = true;
            if (evt.stopPropagation) 
                evt.stopPropagation();
            addtaxcat();
            return false;        
        }
    }
    
    function OpenCloseDiv(divID, anchorID)
{
    var divID = document.getElementById(divID);
    var anchorID = document.getElementById(anchorID);
    if (divID.style.display == "none")
    {
        divID.style.display = "block";
        anchorID.className = "open";
    } else {
        divID.style.display = "none";
        anchorID.className = "closed";
    }
}
