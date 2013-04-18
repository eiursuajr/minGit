
function ektBaseMultimedia() {
    this.preview = ektMultimedia_previewMediaPlayer;
    this.selectOption = ektMultimedia_selectOption;
    this.save = ektMultimedia_saveMediaPlayer;
    this.submit = ektMultimedia_submitMediaPlayer;    
    this.defaultPlayer = false; 
    this.CLSID = "";
    this.Codebase = "";
    this.URL = "";
}

//Name: SelectOption
//Desc:
function ektMultimedia_selectOption( objElem, value ) {
    if ( objElem != null ) {
        for ( var i=0; i<objElem.length; i++ ) {
            if ( value.toString().toLowerCase() == objElem.options[i].value.toString().toLowerCase() ) {                
                objElem.selectedIndex = i;
                break;
            }
        }
    }
}

//Name: saveWMPlayer
//Desc:
function ektMultimedia_saveMediaPlayer( Field ) {
    var objText = this.createMeidaPlayerObject("save");
    Field.value = objText;
    return true;    
}
       
//Name: preview
//Desc:
function ektMultimedia_previewMediaPlayer() {

    document.getElementById( this.name + '-preview' ).innerHTML = "Plese wait...";
    var objText = this.createMeidaPlayerObject();    
    if ( objText == "" ) {
        document.getElementById( this.name + '-preview' ).innerHTML = "Error: Object is not created.";
    }
    else {
        document.getElementById( this.name + '-preview' ).innerHTML = objText;
    }
}


//Name: Submit
//Desc:
function ektMultimedia_submitMediaPlayer() {
    if (this.validateForm())
    {
        this.saveMediaPlayer( document.getElementById("content_html") );
        document.Form1[0].submit();
    }
    else {
        return false;       
    }
    return true;
}

function readValue(obj, attName)
{
    if (navigator.appName.indexOf("Microsoft Internet")==-1)
    {
        if (obj.attributes[attName] != null)
        {
            return obj.attributes[attName].value;
        }        
        else
        {
            return "";
        }
    }
    else {
        try{
            if (obj[attName] != null) {
                return obj[attName];
            }
            else {
                return "";
            }
        } catch(ex) {
            return "";
        }
    }    
}
function getObject(Name)
{ 
  if (navigator.appName.indexOf("Microsoft Internet")==-1)
  {
    if (document.embeds && document.embeds[Name])
      return document.embeds[Name]; 
  }
  else // if (navigator.appName.indexOf("Microsoft Internet")!=-1)
  {
    return document.getElementById(Name);
  }
}    