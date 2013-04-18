function AllowOnlyNumeric(e) 
{
    var key;
    // Get the ASCII value of the key that the user entered
    if (navigator.appName.lastIndexOf("Microsoft Internet Explorer") > -1)
        key = e.keyCode;
    else
        key = e.which;

    if ((key == 0 || key == 8 || key == 9))
        return true;
    // Verify if the key entered was a numeric character (0-9) or a decimal (.)
    if ((key > 47 && key < 58))
    // If it was, then allow the entry to continue
        return true;
    else { // If it was not, then dispose the key and continue with entry
        e.returnValue = null;
        return false;
    }
}
function MouseClickEvent()
 {
    // Disable cut ,copy and paste in propeties tab textboxes
    return false;
}