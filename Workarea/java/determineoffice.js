function Browseris () {

            var agt=navigator.userAgent.toLowerCase();

            this.osver=1.0;

            if (agt)

            {

                        var stOSVer=agt.substring(agt.indexOf("windows ")+11);

                        this.osver=parseFloat(stOSVer);

            }

            this.major=parseInt(navigator.appVersion);

            this.nav=((agt.indexOf('mozilla')!=-1)&&((agt.indexOf('spoofer')==-1) && (agt.indexOf('compatible')==-1)));

            this.nav6=this.nav && (this.major==5);

            this.nav6up=this.nav && (this.major >=5);

            this.nav7up=false;

            if (this.nav6up)

            {

                        var navIdx=agt.indexOf("netscape/");

                        if (navIdx >=0 )

                                    this.nav7up=parseInt(agt.substring(navIdx+9)) >=7;

            }

            this.ie=(agt.indexOf("msie")!=-1);

            this.aol=this.ie && agt.indexOf(" aol ")!=-1;

            if (this.ie)

                        {

                        var stIEVer=agt.substring(agt.indexOf("msie ")+5);

                        this.iever=parseInt(stIEVer);

                        this.verIEFull=parseFloat(stIEVer);

                        }

            else

                        this.iever=0;

            this.ie4up=this.ie && (this.major >=4);

            this.ie5up=this.ie && (this.iever >=5);

            this.ie55up=this.ie && (this.verIEFull >=5.5);

            this.ie6up=this.ie && (this.iever >=6);

            this.winnt=((agt.indexOf("winnt")!=-1)||(agt.indexOf("windows nt")!=-1));

            this.win32=((this.major >=4) && (navigator.platform=="Win32")) ||

                        (agt.indexOf("win32")!=-1) || (agt.indexOf("32bit")!=-1);

            this.mac=(agt.indexOf("mac")!=-1);

            this.w3c=this.nav6up;

            this.safari=(agt.indexOf("safari")!=-1);

            this.safari125up=false;

            if (this.safari && this.major >=5)

            {

                        var navIdx=agt.indexOf("safari/");

                        if (navIdx >=0)

                                    this.safari125up=parseInt(agt.substring(navIdx+7)) >=125;

            }

}

var browseris=new Browseris();

var bis=browseris;

 
function ShowMultipleUpload ()
{
    if (browseris.ie5up && !browseris.mac)
    {
        try
        {
            var MultipleDocumentTest1 = new ActiveXObject('SharePoint.OpenDocuments.1');
            return true;
        }
        catch(e)
        {}
        try
        {
            var MultipleDocumentTest2 = new ActiveXObject('SharePoint.OpenDocuments.2');
            return true;
        }
        catch(e)
        {}
        try
        {
            var MultipleDocumentTest3 = new ActiveXObject('SharePoint.OpenDocuments.3');
            return true;
        }
        catch(e)
        {}
        try
        {
            var UploadControl = new ActiveXObject('STSUpld.UploadCtl');
            return true;
        }
        catch(e)
        {}
        try
        {
            var UploadControl = new ActiveXObject('Name.NameCtrl');
            return true;
        }
        catch(e)
        {}
        try
        {
            var UploadControl = new ActiveXObject('Name.NameCtrl.1');
            return true;
        }
        catch(e)
        {}
    }
    return false;
}

function CheckSTSUpload()
{
    if (browseris.ie5up && !browseris.mac)
    {
        try
        {
            var UploadControl = new ActiveXObject('STSUpld.UploadCtl');
            return true;
        }
        catch(e)
        {}
        try
        {
            var UploadControl = new ActiveXObject('Name.NameCtrl');
            return true;
        }
        catch(e)
        {}
        try
        {
            var UploadControl = new ActiveXObject('Name.NameCtrl.1');
            return true;
        }
        catch(e)
        {}
    }
    return false;
}
