CPA
===

Web Service para obtener CPA Argentino para una dirección especificada.

Operaciones: 

GetCPAByNombre: obtiene el CPA por el identificador de provincia en correo, nombre de localdidad, calle y altura.

params: 
idProvincia : String.
nombreLocalidad  - String.
calle  - String.
altura  - String.

GetCPAById :obtiene el CPA por el identificador de provincia, identificador de localdidad en correo, calle y altura.
idProvincia : String
idLocalidad  : String
calle  : String
altura  : String

GetIdLocalidad: obtiene el identificador de correo de una localidad en base al id de provincia y el nombre especificados
idProvincia : String
nombreLocalidad  : String

GetIdProvincia: obtiene el identificador de correo de una provincia en base a su nombre
nombreProvincia  : String

para una lectura mas rapida de los identificadores de provincias, reviar Web.config.

Se pueden realizar UnitTests con el proyecto CpsServicesTest.
