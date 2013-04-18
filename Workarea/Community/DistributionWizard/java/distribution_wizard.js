function HandleWindowClose()
{
    if(typeof ExecWindowCloseAction == 'function')
    {
        ExecWindowCloseAction();
    }
    
    return true;
}

function ExecHttpRequest(reqType,url,asynch) 
{
    if (window.XMLHttpRequest) {
        request = new XMLHttpRequest();
    } else if (window.ActiveXObject) {
        request = new ActiveXObject("Msxml2.XMLHTTP");
        if (!request) {
            request = new ActiveXObject("Microsoft.XMLHTTP");
        }
    }
	
    if (request) 
    {
        if (reqType.toLowerCase() != "post") 
        {
            InitReq(reqType, url, asynch);
        } 
        else 
        {
            var args = arguments[4];
            if (args != null && args.length > 0) {
                InitReq(reqType, url, asynch, args);
            }
        }
    }
}

function InitReq(reqType, url, bool) 
{
    try 
    {
        request.open(reqType, url, bool);
     
        if (reqType.toLowerCase() == "post") 
        {    
            request.setRequestHeader("Content-Type", "application/x-ww-form-urlencoded; charset=UTF-8");
            request.send(arguments[4]);
        } 
        else 
        {
            request.send(null);
        }
    } 
    catch (errv) {}
}