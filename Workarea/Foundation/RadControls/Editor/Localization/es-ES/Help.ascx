<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.BaseDialogControl"%>
<table cellspacing="0" cellpadding="2" border="1" bordercolor="#000000" style="font:normal 10px Arial">
	<tr>
		<td colspan="3" align="middle"><strong>BOTONES GENERALES</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del dise�o" alt="Bot�n del dise�o" src="<%= this.SkinPath %>Img/ButtonDesign.gif"></td>
		<td>Bot�n del dise�o - interruptores Ektron eWebEdit400 en modo del dise�o.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del HTML" alt="Bot�n del HTML" src="<%= this.SkinPath %>Img/ButtonHtml.gif"></td>
		<td>Bot�n del HTML - interruptores Ektron eWebEdit400 en modo del HTML.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n de la inspecci�n previo" alt="Bot�n de la inspecci�n previo" src="<%= this.SkinPath %>Img/ButtonPreview.gif"></td>
		<td>Bot�n de la inspecci�n previo - interruptores Ektron eWebEdit400 en modo de la inspecci�n previo.</td>
		<td>-</td>
	</tr>
	
	<tr>
		<td align="middle"><img title="Convertir a may�sculas" alt="Convertir a may�sculas" src="<%= this.SkinPath %>Buttons/ConvertToUpper.gif"></td>
		<td>Convertir el texto de la selecci�n actual al may�scula, preservando los elementos del no-texto tales como im�genes y tablas.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Convertir a min�scula" alt="Convertir a min�scula" src="<%= this.SkinPath %>Buttons/ConvertToLower.gif"></td>
		<td>Convertir el texto de la selecci�n actual a la min�scula, preservando los elementos del no-texto tales como im�genes y tablas.
	</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Permitir que los usuarios creen mapas" alt="Permitir que los usuarios creen mapas" src="<%= this.SkinPath %>Buttons/ImageMapDialog.gif"></td>
		<td>Permitir que los usuarios creen mapas de imagen con draging sobre las im�genes y crear �reas del hyperlink de diversas formas.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Formato bloques del c�digo" alt="Formato bloques del c�digo" src="<%= this.SkinPath %>Buttons/FormatCodeBlock.gif"></td>
		<td>Permitir que los usuarios inserten y que ajusten a formato bloques del c�digo en el contenido.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Tama�o de fuente" alt="Tama�o de fuente" src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Permite que el usuario se apliquen al tama�o de fuente actual de la selecci�n medido en pixeles, m�s bien que un 1 de tama�o fijo a 7 (al igual que la herramienta de FontSize).</td>
		<td>-</td>
	</tr>
		
	<tr>
		<td align="middle"><img title="Modo de palanca de la pantalla" alt="Modo de palanca de la pantalla" src="<%= this.SkinPath %>Buttons/ToggleScreenMode.gif"></td>
		<td>Modo de palanca de la pantalla - interruptores Ektron eWebEdit400 en modo de la pantalla completa.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="La demostraci�n/la frontera de la piel" alt="La demostraci�n/la frontera de la piel" src="<%= this.SkinPath %>Buttons/ToggleTableBorder.gif"></td>
		<td>La demostraci�n/la frontera de la piel - las demostraciones u oculta las fronteras alrededor de las tablas en el �rea contenta.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Zumbido" alt="Zumbido" src="<%= this.SkinPath %>Buttons/Zoom.gif"></td>
		<td>Zumbido - cambios el nivel de la ampliaci�n del texto.</td>
		<td>-</td>
	</tr>

	<tr>
		<td align="middle"><img title="Encargado del m�dulo" alt="Encargado del m�dulo" src="<%= this.SkinPath %>Buttons/ModuleManager.gif"></td>
		<td>Encargado del m�dulo - activa los m�dulos de /Deactivates de una lista drop-down de m�dulos disponibles.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Muelle de palanca" alt="Muelle de palanca" src="<%= this.SkinPath %>Buttons/ToggleDocking.gif"></td>
		<td>Muelle de palanca - muelles todos toolbars flotantes a sus �reas de muelle respectivas.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Repetir el comando pasado" alt="Repetir el comando pasado" src="<%= this.SkinPath %>Buttons/RepeatLastCommand.gif"></td>
		<td>Repetir el comando pasado - un atajo para repetir la acci�n pasada realizada.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Encontrar y substituir" alt="Encontrar y substituir" src="<%= this.SkinPath %>Buttons/FindAndReplace.gif"></td>
		<td>Encontrar y substituir - encontrar (y substituye) el texto en el �rea contenta del redactor.</td>
		<td>Ctrl+F</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n de impresi�n" alt="Bot�n de impresi�n" src="<%= this.SkinPath %>Buttons/Print.gif"></td>
		<td>Bot�n de impresi�n - impresiones el contenido del Ektron eWebEdit400 o del Web page entero.</td>
		<td>Ctrl+P</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del encanto" alt="Bot�n del encanto" src="<%= this.SkinPath %>Buttons/Spellcheck.gif"></td>
		<td>Bot�n del encanto - lanzamientos el comprobador de ortograf�a.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del corte" alt="Bot�n del corte" src="<%= this.SkinPath %>Buttons/Cut.gif"></td>
		<td>Bot�n del corte - cortes el contenido y las copias seleccionados �l al sujetapapeles.</td>
		<td>Ctrl+X</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del copy" alt="Bot�n del copy" src="<%= this.SkinPath %>Buttons/Copy.gif"></td>
		<td>Bot�n del copy - copias el contenido seleccionado al sujetapapeles.</td>
		<td>Ctrl+C</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n de la goma" alt="Bot�n de la goma" src="<%= this.SkinPath %>Buttons/Paste.gif"></td>
		<td>Bot�n de la goma - gomas el contenido copiado del sujetapapeles en el redactor.</td>
		<td>Ctrl+V</td>
	</tr>
	<tr>
		<td align="middle"><img title="La goma del bot�n de la palabra" alt="La goma del bot�n de la palabra" src="<%= this.SkinPath %>Buttons/PasteFromWord.gif"></td>
		<td>La goma del bot�n de la palabra - contenido de las gomas copiado de palabra y quita las etiquetas tela-antip�ticas.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="La goma de fuentes de la limpieza de la palabra y los tama�os abotonan" alt="La goma de fuentes de la limpieza de la palabra y los tama�os abotonan" src="<%= this.SkinPath %>Buttons/PasteFromWordNoFontsNoSizes.gif"></td>
		<td>La goma de fuentes de la limpieza de la palabra y los tama�os abotonan - limpia todas las etiquetas Palabra-espec�ficas y quita nombres de la fuente y tama�os del texto.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pegar el bot�n llano del texto" alt="Pegar el bot�n llano del texto" src="<%= this.SkinPath %>Buttons/PastePlaintext.gif"></td>
		<td>Pegar el bot�n llano del texto - texto llano de las gomas (ning�n formato) en el redactor.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Goma como bot�n del HTML" alt="Goma como bot�n del HTML" src="<%= this.SkinPath %>Buttons/PasteAsHtml.gif"></td>
		<td>Goma como bot�n del HTML - c�digo del HTML de las gomas en el �rea y las subsistencias contentas todas las etiquetas del HTML.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Deshacer el bot�n" alt="Deshacer el bot�n" src="<%= this.SkinPath %>Buttons/Undo.gif"></td>
		<td>Deshacer el bot�n - deshace la acci�n pasada.</td>
		<td>Ctrl+Z</td>
	</tr>
	<tr>
		<td align="middle"><img title="Hacer de nuevo el bot�n" alt="Hacer de nuevo el bot�n" src="<%= this.SkinPath %>Buttons/Redo.gif"></td>
		<td>Hacer de nuevo el bot�n - hace de nuevo/repite la acci�n pasada, se ha deshecho que.</td>
		<td>Ctrl+Y</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del separador del formato" alt="Bot�n del separador del formato" src="<%= this.SkinPath %>Buttons/Sweeper.gif"></td>
		<td>Bot�n del separador del formato - quita costumbre o todo el formato del texto seleccionado.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Ayuda r�pida" alt="Ayuda r�pida" src="<%= this.SkinPath %>Buttons/Help.gif"></td>
		<td>Ayuda r�pida - lanzamientos la ayuda r�pida que est�s viendo actualmente.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Sobre el di�logo" alt="Sobre el di�logo" src="<%= this.SkinPath %>Buttons/AboutDialog.gif"></td>
		<td>Sobre el di�logo - demostraciones la versi�n actual y las credenciales de Ektron eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>INSERTAR Y MANEJAR LOS ACOPLAMIENTOS, las TABLAS, los CARACTERES ESPECIALES, las IM�GENES y los MEDIOS</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del encargado de la imagen" alt="Bot�n del encargado de la imagen" src="<%= this.SkinPath %>Buttons/ImageManager.gif"></td>
		<td>Bot�n del encargado de la imagen - rellenos una imagen de una carpeta predefinida de la imagen.</td>
		<td>Ctrl+G</td>
	</tr>
	<tr>
		<td align="middle"><img title="Mapa de imagen" alt="Mapa de imagen" src="<%= this.SkinPath %>Buttons/ImageMapDialog.gif"></td>
		<td>Mapa de imagen - permite que los usuarios definan �reas clickable dentro de la imagen.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n absoluto de la posici�n del objeto" alt="Bot�n absoluto de la posici�n del objeto" src="<%= this.SkinPath %>Buttons/AbsolutePosition.gif"></td>
		<td>Bot�n absoluto de la posici�n del objeto - sistemas una posici�n absoluta de un objeto (movimiento libre).</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n de la tabla del relleno" alt="Bot�n de la tabla del relleno" src="<%= this.SkinPath %>Buttons/InsertTable.gif"></td>
		<td>Bot�n de la tabla del relleno - rellenos una tabla en el Ektron eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Fronteras de palanca de la tabla" alt="Fronteras de palanca de la tabla" src="<%= this.SkinPath %>Buttons/ToggleBorders.gif"></td>
		<td>  Fronteras de palanca de la tabla - fronteras de las palancas de todas las tablas dentro del redactor. </td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Relleno Snippet" alt="Relleno Snippet" src="<%= this.SkinPath %>Buttons/InsertSnippet.gif"></td>
		<td>Relleno Snippet - snippets predefinidos rellenos del c�digo.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Elemento de la forma del relleno" alt="Elemento de la forma del relleno" src="<%= this.SkinPath %>Buttons/InsertFormElement.gif"></td>
		<td>Elemento de la forma del relleno - rellenos un elemento de la forma de una lista drop-down con los elementos disponibles.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Insertar el bot�n de la fecha" alt="Insertar el bot�n de la fecha" src="<%= this.SkinPath %>Buttons/InsertDate.gif"></td>
		<td>Insertar el bot�n de la fecha - fecha actual de los rellenos.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Insertar el bot�n de Tiempo" alt="Insertar el bot�n de Tiempo" src="<%= this.SkinPath %>Buttons/InsertTime.gif"></td>
		<td>Insertar el bot�n de Tiempo - tiempo actual de los rellenos.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="El bot�n de destello del encargado" alt="El bot�n de destello del encargado" src="<%= this.SkinPath %>Buttons/FlashManager.gif"></td>
		<td>El bot�n de destello del encargado - rellenos una animaci�n de destello y te deja fijar sus caracter�sticas.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del encargado de los medios de Windows" alt="Bot�n del encargado de los medios de Windows" src="<%= this.SkinPath %>Buttons/MediaManager.gif"></td>
		<td>Bot�n del encargado de los medios de Windows - rellenos que los medios de Windows se oponen (AVI, MPEG, WAV, etc.) y que te dejan fijar sus caracter�sticas.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Encargado del documento" alt="Encargado del documento" src="<%= this.SkinPath %>Buttons/DocumentManager.gif"></td>
		<td>Encargado del documento - rellenos un acoplamiento a un documento en el servidor (pdf, doc., etc.)</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Quitar el bot�n del Hyperlink" alt="Quitar el bot�n del Hyperlink" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Bot�n del encargado del Hyperlink - marcas el texto o la imagen seleccionado un hyperlink.</td>
		<td>Ctrl+K</td>
	</tr>
	<tr>
		<td align="middle"><img title="Quitar el bot�n del Hyperlink" alt="Quitar el bot�n del Hyperlink" src="<%= this.SkinPath %>Buttons/Unlink.gif"></td>
		<td>Quitar el bot�n del Hyperlink - quita el hyperlink del texto o de la imagen seleccionado.</td>
		<td>Ctrl+Shift+K</td>
	</tr>
	<tr>
		<td align="middle"><img title="Car�cter especial del relleno dropdown" alt="Car�cter especial del relleno dropdown" src="<%= this.SkinPath %>Buttons/Symbols.gif"></td>
		<td>Car�cter especial del relleno dropdown - rellenos un car�cter especial (&euro; &reg; , face= " Arial " del <font > �, �</font>, etc.)</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Acoplamiento de encargo del relleno dropdown" alt="Acoplamiento de encargo del relleno dropdown" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Acoplamiento de encargo del relleno dropdown - rellenos un acoplamiento interno o externo de una lista predefinida.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Elegir la plantilla del HTML" alt="Elegir la plantilla del HTML" src="<%= this.SkinPath %>Buttons/TemplateManager.gif"></td>
		<td>Elegir la plantilla del HTML - se aplica y plantilla del HTML de una lista predefinida de plantillas.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>CREAR, AJUSTAR A FORMATO Y CORREGIR LOS P�RRAFOS y las L�NEAS</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n nuevo del p�rrafo del relleno" alt="Bot�n nuevo del p�rrafo del relleno" src="<%= this.SkinPath %>Buttons/InsertParagraph.gif"></td>
		<td>Bot�n nuevo del p�rrafo del relleno - nuevo p�rrafo de los rellenos.</td>
		<td>Ctrl+M</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n Dropdown del estilo del p�rrafo" alt="Bot�n Dropdown del estilo del p�rrafo" src="<%= this.SkinPath %>Buttons/Paragraph.gif"></td>
		<td>Bot�n Dropdown del estilo del p�rrafo - aplica estilos est�ndares del texto al texto seleccionado.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n de Outdent" alt="Bot�n de Outdent" src="<%= this.SkinPath %>Buttons/Outdent.gif"></td>
		<td>Bot�n de Outdent - p�rrafos de las mellas a la izquierda.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n de la mella" alt="Bot�n de la mella" src="<%= this.SkinPath %>Buttons/Indent.gif"></td>
		<td>Bot�n de la mella - p�rrafos de las mellas a la derecha.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Align Left button" alt="Align Left button" src="<%= this.SkinPath %>Buttons/JustifyLeft.gif"></td>
		<td>Align Left button - Aligns the selected paragraph to the left.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Center button" alt="Center button" src="<%= this.SkinPath %>Buttons/JustifyCenter.gif"></td>
		<td>Center button - Aligns the selected paragraph to the center.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Alinear el bot�n derecho" alt="Alinear el bot�n derecho" src="<%= this.SkinPath %>Buttons/JustifyRight.gif"></td>
		<td>Alinear el bot�n derecho - alinea el p�rrafo seleccionado con la derecha.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Justificar el bot�n" alt="Justificar el bot�n" src="<%= this.SkinPath %>Buttons/JustifyFull.gif"></td>
		<td>Justificar el bot�n - justifica el p�rrafo seleccionado.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n de la lista Bulleted" alt="Bot�n de la lista Bulleted" src="<%= this.SkinPath %>Buttons/InsertUnorderedList.gif"></td>
		<td>Bot�n de la lista Bulleted - crea una lista bulleted de la selecci�n.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n numerado de la lista - crea una lista numerada de la selecci�n" alt="Bot�n numerado de la lista - crea una lista numerada de la selecci�n" src="<%= this.SkinPath %>Buttons/InsertOrderedList.gif"></td>
		<td>Bot�n numerado de la lista - crea una lista numerada de la selecci�n.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Linea horizontal del relleno (e.g. regla horizontal) bot�n" alt="Linea horizontal del relleno (e.g. regla horizontal) bot�n" src="<%= this.SkinPath %>Buttons/InsertHorizontalRule.gif"></td>
		<td>Linea horizontal del relleno (e.g. regla horizontal) bot�n - rellenos una linea horizontal en la posici�n del cursor.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>CREAR, AJUSTAR A FORMATO Y CORREGIR EL TEXTO, la FUENTE y las LISTAS</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n en negrilla" alt="Bot�n en negrilla" src="<%= this.SkinPath %>Buttons/Bold.gif"></td>
		<td>Bot�n en negrilla - aplica el formato en negrilla al texto seleccionado.</td>
		<td>Ctrl+B</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del it�lico" alt="Bot�n del it�lico" src="<%= this.SkinPath %>Buttons/Italic.gif"></td>
		<td>Bot�n del it�lico - aplica el formato del it�lico al texto seleccionado.</td>
		<td>Ctrl+I</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n de la raya" alt="Bot�n de la raya" src="<%= this.SkinPath %>Buttons/Underline.gif"></td>
		<td>Bot�n de la raya - aplica el formato de la raya al texto seleccionado.</td>
		<td>Ctrl+U</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n de Strikethrough" alt="Bot�n de Strikethrough" src="<%= this.SkinPath %>Buttons/StrikeThrough.gif"></td>
		<td>Bot�n de Strikethrough - aplica el formato del strikethrough al texto seleccionado.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del exponente" alt="Bot�n del exponente" src="<%= this.SkinPath %>Buttons/Superscript.gif"></td>
		<td>Bot�n del exponente - marcas un exponente del texto.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n suscrito" alt="Bot�n suscrito" src="<%= this.SkinPath %>Buttons/Subscript.gif"></td>
		<td>Bot�n suscrito - marcas un sub�ndice del texto.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n selecto de la fuente" alt="Bot�n selecto de la fuente" src="<%= this.SkinPath %>Buttons/FontName.gif"></td>
		<td>Bot�n selecto de la fuente - sistemas la tipograf�a de la fuente.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del tama�o de fuente" alt="Bot�n del tama�o de fuente" src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Bot�n del tama�o de fuente - sistemas el tama�o de fuente.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del color del texto (primero plano)" alt="Bot�n del color del texto (primero plano)" src="<%= this.SkinPath %>Buttons/ForeColor.gif"></td>
		<td>Bot�n del color del texto (primero plano) - cambios el color del primero plano del texto seleccionado.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bot�n del color del texto (fondo)" alt="Bot�n del color del texto (fondo)" src="<%= this.SkinPath %>Buttons/BackColor.gif"></td>
		<td>Bot�n del color del texto (fondo) - cambios el color del fondo del texto seleccionado.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Los estilos de encargo dropdown" alt="Los estilos de encargo dropdown" src="<%= this.SkinPath %>Buttons/Class.gif"></td>
		<td>Los estilos de encargo dropdown - aplica el costumbre, estilos predefinidos al texto seleccionado.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Formato bloques del c�digo" alt="Formato bloques del c�digo" src="<%= this.SkinPath %>Buttons/FormatCodeBlock.gif"></td>
		<td>Permitir que los usuarios inserten y que ajusten a formato bloques del c�digo en el contenido.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Acoplamientos de encargo dropdown" alt="Acoplamientos de encargo dropdown" src="<%= this.SkinPath %>Buttons/CustomLinks.gif"></td>
		<td>Acoplamientos de encargo dropdown - costumbre de los rellenos, acoplamiento predefinido.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>OTROS ATAJOS DEL TECLADO</strong></td>
	</tr>
	<tr>
		<td>-</td>
		<td>Selecciona todo el texto, im�genes y tablas en el redactor.</td>
		<td>Ctrl+A</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Encuentra una cadena de texto o de n�meros en la p�gina.</td>
		<td>Ctrl+F</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Cierra la ventana activa.</td>
		<td>Ctrl+W</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Cierra el uso activo.</td>
		<td>Ctrl+F4</td>
	</tr>
</table>