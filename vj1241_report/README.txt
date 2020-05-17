El contenido de este directorio proporciona una plantilla en LaTeX
para la elaboración de la memoria del trabajo o proyecto de fin de
grado.


DESCRIPCIÓN DE LOS FICHEROS

El fichero principal es «memoria.tex». En dicho fichero se declaran
aspectos como el título de la memoria y el nombre del autor del
trabajo. También es el encargado de cargar la cabecera con las macros
utilizadas en la memoria así como el resto de ficheros que constituyen
los distintos capítulos y apéndices de la memoria.

El fichero «memoria_cabecera.tex» carga los paquetes utilizados en la
memoria y define una serie de macros.

Los ficheros que empiezan por «cap» y «apend», corresponden a los
capítulos y apéndices de la memoria. En el caso de añadir más
capítulos, éstos deberán incluirse también en el fichero
«memoria.tex».

En el directorio «img/» se encuentras las imágenes utilizadas por la
plantilla. Es recomendable añadir el resto de imágenes en este
directorio para dejar lo más limpio posible el directorio principal.

Por último, el fichero «biblio/sbmspaalpha.bst» se encuentra la
definición del formato que se utiliza por defecto para mostrar la
bibliografía.


GENERACIÓN DEL FICHERO PDF

Se puede hacer de dos formas:

1. Desde la línea de comandos:

1.1. Ejecutando el comando «make pdf». (En este caso también se puede
     utilizar el comando «make clean» para borrar los ficheros
     temporales.)

1.2. Ejecutando el comando «pdflatex memoria.tex». (Es recomendable
     ejecutar dicho comando 3 veces cuando se quiera asegurar que
     todas las referencias cruzadas se han actualizado correctamente.)

2. Utilizando el comando apropiado en un editor de LaTeX. Si el editor
lo permite, conviene declarar el fichero «memoria.tex» como documento
maestro. Por ejemplo, en TeXMaker se puede declarar el fichero
«memoria.tex» como documento maestro seleccionando en el menú
«Opciones» la entrada «Definir documento actual como 'documento
maestro'». Si se hace esto, cada vez que se pulse sobre el botón de
compilación se recompilará toda la memoria (independientemente de qué
fichero se esté editando en un momento dado).

DERECHOS DE COPIA Y LICENCIA

Los derechos de copia de esta plantilla pertenecen a Sergio Barrachina
Mir, quien la proporciona bajo la licencia:

Creative Commons Attribution-NonCommercial-ShareAlike (CC BY-NC-SA)

http://creativecommons.org/licenses/by-nc-sa/3.0/
http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode

