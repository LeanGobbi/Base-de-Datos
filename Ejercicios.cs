
Consultas (Realizar en AR: 1, 2, 3 y 4 y en SQL: 2, 3, 4, 5 y 6)

Cliente = (id_cli, DNI, apellido, nombre, domicilio, contacto) 
Artículo (id_art, tipo, desc, stock, precio)
Pedidos (id_ped, fechaPed, fechaEnv, id_cli (FK)) 
Detalle_Pedido (id_ped (FK), id_art (FK), cant, pre_unit)

1. Listar los DNI, apellido y nombre de los clientes que solo compraron artículos con el tipo 'Heladera'. 
2. Listar el contacto de los clientes que tienen pedidos en el año 2023 y también en 2024.
3. Eliminar el Pedido con id_ped 12345.
4. Listar apellido y nombre de ios clientes que compraron todos los artículos. En SQL ordenar por apellido.
5. Listar para cada cliente, el DN!, nombre, apellido y cantidad de pedidos. Ordenar por cantidad de pedidos descendentemente.
6. Listar los artículos que se vendieron más de 15 veces durante el 2023.

﻿AR:

1. 
ArticulosNOHeladeras <= (σ (tipo <> 'Heladera') (Articulo))
ClientesNOHeladeras <= Cliente |x| ( Pedidos |x| ( Detalle_Pedido |x| ArticuloNOHeladeras))
ClientesSOLOHeladeras <= π DNI, apellido, nombre, domicilio, contacto (Cliente - ClientesNOHeladeras)

Otra solucion:

ArticulosNOHeladeras <= (σ (tipo <> 'Heladera') (Articulo))
ArticulosHeladeras <= (σ (tipo = 'Heladera') (Articulo))
ClientesNOHeladeras <= Cliente |x| ( Pedidos |x| ( Detalle_Pedido |x| ArticuloNOHeladeras))
ClientesHeladeras <= Cliente |x| ( Pedidos |x| ( Detalle_Pedido |x| ArticuloHeladeras))
ClientesSOLOHeladeras <= π DNI, apellido, nombre, domicilio, contacto (ClientesHeladeras - ClientesNOHeladeras)


2. 
Clientes2023 <= (σ (fechaPed >= '01/01/2023') ^ (fechaPed =< '31/12/2023') (Cliente |x| Pedidos))
Clientes2024 <= (σ (fechaPed >= '01/01/2024') ^ (fechaPed =< '31/12/2024') (Cliente |x| Pedidos))
Clientes2023y2024 <= π contacto (Clientes2023 ∩ Clientes2024)

3.
Pedidos <= Pedidos - (σ (id_ped = 12345) (Pedidos))
Detalle_Pedido <= Detalle_Pedido - (σ (id_ped = 12345) (Detalle_Pedido))

4.
ClientesxArticulo <= π id_cli, id_art Cliente |x| (Pedidos |x| Detalle_Pedido))
ArticulosTodos <= π id_art (Articulo)
ClientesTODOSArticulos <= π nombre,apellido (Cliente |x| (ClientesxArticulo % ArticulosTodos))


SQL:

2. 
SELECT DISTINCT c.contacto
FROM Cliente c
INNER JOIN Pedidos p ON (p.id_cli = c.id_cli)
WHERE YEAR (p.fechaPed) = 2023

INTERSECT
  SELECT DISTINCT c.contacto
  FROM Cliente c
  INNER JOIN Pedidos p ON (p.id_cli = c.id_cli)
  WHERE YEAR (p.fechaPed) = 2024

3.
DELETE FROM Detalle_Pedido dp
WHERE (dp.id_ped = 12345)
  
DELETE FROM Pedidos p
WHERE (p.id_ped = 12345)

4.
SELECT c.apellido, c.nombre
FROM Cliente c
WHERE NOT EXISTS (
  SELECT *
  FROM Articulo a
    WHERE NOT EXISTS (
      SELECT *
      FROM Detalle_Pedido dp
      INNER JOIN Pedidos p ON (p.id_ped = dp.id_ped)
      WHERE (dp.id_art = a.id_art) AND (p.id_cli = c.id_cli)))
ORDER BY apellido

5.
SELECT c.dni, c.nombre, c.apellido, count (id_ped) as cantidad_pedidos
FROM Cliente c
LEFT JOIN Pedidos p ON (p.id_cli = c.id_cli)
GROUP BY dni, nombre, apellido
ORDER BY cantidad_pedidos DESC

6.
SELECT a.id_art, a.tipo, a.desc, a.stock, a.precio
FROM Articulo a
INNER JOIN Detalle_Pedido dp ON (dp.id_art = a.id_art)
INNER JOIN Pedidos p ON (p.id_ped = dp.id_ped)
WHERE YEAR (fechaPed) = 2023
GROUP BY a.id_art, a.tipo, a.desc, a.stock, a.precio
HAVING COUNT (*) > 15



Resolver del 1 al 4 AR y del 2 al 6 en SQL
Socio = (DNI,CUIL, apellido, nombre, domicilio, telefono)
Libro = (ISBN, título, autor, género, stock, precio)
Préstamo = (id_prestamo, fechaPrestamo, fechaDevolucionTentativa, fechaDevEfectiva?, DNI(FK)) 
Detalle_Préstamo = (id_prestamo (FK), ISBN (FK), precio, estado_dev?)
  
1. Listar los datos de los socios que tienen préstamos con devoluciones (fechaDevEfectiva) mayor a la fecha pactada y también tienen devoluciones donde el estado_dev es malo.
2. Listar los socios que solo tienen préstamos en el mes actual. (DEVUELTOS O NO)
3. Modificar el precio de los libros incrementándolo en un 10%.
4. Listar los socios que tuvieron prestados todos los libros.
5. Informar la cantidad de préstamos del mes actual.
6. Listar todos los datos del socio o de los socios que tienen mayor cantidad de préstamos en el año 2023.

AR:

1.
SociosDevolucionesFechaMayor <= π DNI (σ (fechaDevEfectiva > fechaDevolucionTentativa) (Prestamo))
SociosEstadoDevMalo <= π DNI (σ (estado_dev = 'MALO') (Prestamo |x| Detalle_Prestamo))
SociosFinales <= π DNI, CUIL, apellido, nombre, domicilio, telefono (Socio |x| (SociosDevolucionesFechaMayor ∩ SociosEstadoDevMalo))

2.

SociosPrestamosJunio <= π DNI (σ (fechaPrestamo >= '01/06/2024') ∧ (fechaPrestamo =< '30/06/2024') (Socio |x| Prestamo))
SociosPrestamosNOJunio <= π DNI (σ (fechaPrestamo < '01/06/2024') ∨ (fechaPrestamo > '30/06/2024') (Socio |x| Prestamo))
SociosPrestamosSOLOJunio <= π DNI, CUIL, apellido, nombre, domicilio, telefono (Socio |x| (SociosPrestamosJunio - SociosPrestamosNOJunio))

3. 
δ (precio = precio * 1,1) (Libro)

4.
SociosxPrestamo <= π DNI, ISBN (Prestamo |x| Detalle_Prestamo)
LibrosTodos <= π ISBN (Libro)
SociosLibrosTodos <= π DNI, CUIL, apellido, nombre, domicilio, telefono (Socio |x| (SociosxPrestamo % LibrosTodos)

SQL:

2.
SELECT s.DNI, s.CUIL, s.apellido, s.nombre, s.domicilio, s.telefono                                                                         
FROM Socio s
INNER JOIN Prestamo p ON (p.DNI = s.DNI)
WHERE MONTH (p.fechaPrestamo) = 6 and s.DNI NOT IN (
  SELECT p.DNI
  FROM Prestamo p
  WHERE MONTH (p.fechaPrestamo) <> 6)

3.                                                                       
UPDATE Libro l 
SET (l.precio = l.precio * 1.1)

4.
SELECT s.DNI, s.CUIL, s.apellido, s.nombre, s.domicilio, s.telefono 
FROM Socio s
WHERE NOT EXISTS (
  SELECT *
  FROM Libro l
  WHERE NOT EXISTS (
    SELECT *
      FROM Detalle_Prestamo dp
      INNER JOIN Prestamo p ON (p.id_prestamo = dp.id_prestamo)
      WHERE ((s.DNI = p.DNI) and (l.ISBN = dp.ISBN))))
      
5.
SELECT COUNT (p.id_prestamo) AS cantidad_prestamos
FROM Prestamo p
WHERE MONTH (p.fechaPrestamo) = 6

6.                                                                                                                                               
SELECT s.DNI, s.CUIL, s.apellido, s.nombre, s.domicilio, s.telefono
FROM Socio s
INNER JOIN Prestamo p ON (p.DNI = s.DNI)
WHERE YEAR (p.fechaPrestamo) = 2023
GROUP BY s.DNI, s.CUIL, s.apellido, s.nombre, s.domicilio, s.telefono
HAVING COUNT (p.id_prestamo) > ALL (
  SELECT COUNT (p2.id_prestamo)
  FROM Prestamo p2
  WHERE YEAR (p2.fechaPrestamo) = 2023
  GROUP BY p2.DNI)



ALGEBRA RELACIONAL
                                                                         
Ejercicio 5
Marca = (#marca, nombMarca, descripción)
Modelo = (#modelo, nombModelo, descripción)
Celular = (#chip, #modelo, #marca, descripción, precio, color, #local, vendido) //vendido será true si fue
vendido
Local = (#local, calle, nro, ciudad, teléfono, razon_social)
                                                                         
1. Listar nombres de marcas que se venden en locales de ‘Lincoln’ y en locales de ‘Junín’.
2. Listar nombres de marcas que solo se vendieron en locales de ‘La Plata’.
3. Actualizar el precio de todos los celulares del local con razón social ‘VendeCeluZ’ aumentando el
precio actual un 70%.
4. Listar razón social y teléfono de locales que vendieron celulares de modelo ‘modelo1’ y que se
encuentren en ‘La Plata’.
5. Listar información de locales que hayan vendido celulares de todas las marcas.
6. Listar información de modelos que solo se vendieron en ‘Capital Federal’.
7. Borrar el local con #local: 35.
8. Lista los locales que vendieron celular de la marca ‘marcaA’ que también vendieron la marca ‘marcaB’

1.
MarcasLincoln <= π nombMarca (σ (ciudad = 'Lincoln') (Local |x| (Marca |x| Celular)))
MarcasJunin <= π nombMarca (σ (ciudad = 'Junin') (Local |x| (Marca |x| Celular)))
MarcasAmbas <= (MarcasLincoln ∩ MarcasJunin)                           

2.
MarcasLP <= π nombMarca (σ (ciudad = 'La Plata') (Local |x| (Marca |x| Celular)))
MarcasNOLP <= π nombMarca (σ (ciudad <> 'La Plata') (Local |x| (Marca |x| Celular)))                            
MarcasSOLOLP  <= (MarcasLP - MarcasNOLP)                          

3.
δ (precio = precio * 1.7) <= (σ (razon_social = 'VendeCeluZ') (Local |x| Celular))
                            
4.
LocalesModelo1 <= π razon_social, telefono (σ (nombModelo = 'modelo1') (Modelo |x| (Celular |x| Local)))
LocalesLP <= π razon_social, telefono (σ (ciudad = 'La Plata') (Local |x| (Marca |x| Celular))
LocalesFinales <= (LocalesModelo1 ∩ LocalesLP)                         
                           
5.
MarcasTodas <= π marca (Marca)
LocalesxMarcas <= π local, marca (Celular)                                      
LocalesTodasMarcas <= π local, calle, nro, ciudad, telefono, razon_social (Local |x| (LocalesxMarcas % MarcasTodas))

6.  
ModelosCP <= π modelo, nombModelo, descripcion (σ (ciudad = 'Capital Federal') (Local |x| (Modelo |x| Celular)))
ModelosFUERACP <= π modelo, nombModelo, descripcion (σ (ciudad <> 'Capital Federal') (Local |x| (Modelo|x| Celular)))
ModelosSOLOCP <= (ModelosCP - ModelosFUERACP)                           

7.
Local <= Local - (σ (#local = 35) (Local))                                      

8.
LocalesMarcaA <= π #local, calle, nro, ciudad, teléfono, razon_social (σ (nombMarca = 'marcaA') (Marca |x| (Celular |x| Local)))
LocalesMarcaB <= π #local, calle, nro, ciudad, teléfono, razon_social (σ (nombMarca = 'marcaB') (Marca |x| (Celular |x| Local)))
LocalesFinales <= (LocalesMarcaA ∩ LocalesMarcaB)   

SQL
                                       
Ejercicio 7
Cartero = (DNI,nombreYApe, dirección, teléfono)
Sucursal = (IDSUC, nombreS,direcciónS, teléfonoS)
Envio = (NROENVIO, DNI, IDCLIENTEEnvia, IDCLIENTERecibe, IDSUC, fecha, recibido, fechaRecibe,
direcciónEntrega) //recibido es blanco sino se entregó aún el envío
Cliente = (IDCLIENTE, DNI,nombreYApe, dirección, teléfono)
                                       
1. Reportar nombre y apellido, dirección y teléfono del cartero que realizó más envíos.
2. Listar para cada sucursal, la cantidad de envíos realizados a alguna dirección de envío que
contenga el string ‘Ju’. Informar nombre de sucursal y cantidad de envíos correspondiente. Ordenar
por nombre sucursal.
3. Listar datos personales de carteros que entregaron envíos a todas las sucursales.
4. Informar cantidad de envíos no entregados del mes de mayo de 2019.
5. Borrar al cliente con IDCLIENTE: 334.
6. Listar datos personales de carteros que no entregaron envío a clientes(receptor) residentes en ‘La
Plata’ pero si realizaron envíos de clientes (emisor) que residen en ‘Wilde’ (el cliente que envía vive
en Wilde).
7. Reportar información de sucursales que realizaron envíos durante 2020 o que tengan dirección en
Tucuman.
8. Listar datos personales de clientes que no realizaron envíos a la sucursal con nombre ‘La Amistad
1’.
9. Listar datos personales de carteros que realizaron envíos durante 2019 a clientes con DNI inferior a
27329882.
10. Listar los datos de los carteros que aún no hayan realizado ninguna entrega.

1.
SELECT c.nombreYApe, c.direccion, c.telefono
FROM Cartero c
INNER JOIN Envio e ON (e.DNI = c.DNI)
GROUP BY c.nombreYApe, c.direccion, c.telefono
HAVING COUNT (e.NROENVIO) > ALL (
  SELECT COUNT (e2.NROENVIO)
  FROM Envio e2
  GROUP BY e2.DNI)
                                       
2.
SELECT s.nombreS, COUNT (e.NROENVIO) as cantidad_envios
FROM Sucursal s
LEFT JOIN Envio e ON ((e.IDSUC = s.IDSUC) and (e.DireccionEntrega LIKE "%Ju%"))
GROUP BY s.nombreS
ORDER BY s.nombreS

3.
SELECT c.DNI, c.nombreYApe, c.direccion, c.telefono
FROM Cartero c
WHERE NOT EXISTS (
  SELECT * 
  FROM Sucursal s
  WHERE NOT EXISTS (
    SELECT *
    FROM Envio e
    WHERE ((s.IDSUC = e.IDSUC) and (c.DNI = e.DNI))))

4.     
SELECT COUNT (*)
FROM Envio e
WHERE ((e.recibido IS NULL) and (YEAR (e.fecha) = 2019) and (MONTH (e.fecha) = 5))

5.
DELETE FROM Envio e
WHERE ((e.IDCLIENTEEnvia = 334) or (e.IDCLIENTERecibe) = 334))

DELETE FROM Cliente c
WHERE (c.IDCLIENTE = 334)                                      

6.
SELECT c.DNI, c.nombreYApe, c.direccion, c.telefono
FROM Cartero c
INNER JOIN Envio e ON (c.DNI = e.DNI)
INNER JOIN Cliente cli ON ((e.IDCLIENTERecibe = cli.IDCLIENTE) and (e.recibido IS NULL))
WHERE (cli.direccion = 'La Plata')
                                                                         
INTERSECT

SELECT c.DNI, c.nombreYApe, c.direccion, c.telefono
FROM Cartero c
INNER JOIN Envio e ON (c.DNI = e.DNI)
INNER JOIN Cliente cli ON ((e.IDCLIENTEEnvia = cli.IDCLIENTE) and (e.recibido IS NOT NULL))
WHERE (cli.direccion = 'Wilde')

7. 
SELECT s.IDSUC, s.nombreS, s.direccionS, s.telefonoS
FROM Sucursal s
INNER JOIN Envio e ON (e.IDSUC = s.IDSUC)
WHERE YEAR (e.fecha) = 2020 // O AGREGO UN AND CON s.DIRECCIONS = TUCUMAN
                                                                        
UNION

SELECT s.IDSUC, s.nombreS, s.direccionS, s.telefonoS
FROM Sucursal s
WHERE (s.direccionS = 'Tucuman')

                                                                         
8.
SELECT c.IDCLIENTE, c.DNI, c.nombreYApe, c.direccion, c.telefono
FROM Cliente c
INNER JOIN Envio e ON (c.IDCLIENTE = e.IDCLIENTEEnvia) 
INNER JOIN Sucursal s ON (s.IDSUC = e.IDSUC)
WHERE (s.nombreS <> 'La Amistad 1')

UNION   

SELECT c.IDCLIENTE, c.DNI, c.nombreYApe, c.direccion, c.telefono
FROM Cliente c
INNER JOIN Envio e ON (c.IDCLIENTE = e.IDCLIENTERecibe) 
INNER JOIN Sucursal s ON (s.IDSUC = e.IDSUC)

9.
SELECT c.DNI, c.nombreYApe, c.direccion, c.telefono
FROM Cartero c
INNER JOIN Envio e ON (c.DNI = e.DNI)
INNER JOIN Cliente cli ON (cli.IDCLIENTE = e.IDCLIENTERecibe)                                                                         
WHERE (YEAR (e.fecha) = 2019 and (cli.DNI < 27329882))

10.
SELECT c.DNI, c.nombreYApe, c.direccion, c.telefono
FROM Cartero c 
LEFT JOIN Envio e ON (c.DNI = e.DNI)
WHERE (e.NROENVIO IS NULL)
                                                                         
Cartero = (DNI,nombreYApe, dirección, teléfono)
Sucursal = (IDSUC, nombreS,direcciónS, teléfonoS)
Envio = (NROENVIO, DNI, IDCLIENTEEnvia, IDCLIENTERecibe, IDSUC, fecha, recibido, fechaRecibe,
direcciónEntrega) //recibido es blanco sino se entregó aún el envío
Cliente = (IDCLIENTE, DNI,nombreYApe, dirección, teléfono)

                  
1. Reportar nombre y apellido, dirección y teléfono del cartero que realizó más envíos.
2. Listar para cada sucursal, la cantidad de envíos realizados a alguna dirección de envío que
contenga el string ‘Ju’. Informar nombre de sucursal y cantidad de envíos correspondiente. Ordenar
por nombre sucursal.
3. Listar datos personales de carteros que entregaron envíos a todas las sucursales.
4. Informar cantidad de envíos no entregados del mes de mayo de 2019.
5. Borrar al cliente con IDCLIENTE: 334.
6. Listar datos personales de carteros que no entregaron envío a clientes(receptor) residentes en ‘La
Plata’ pero si realizaron envíos de clientes (emisor) que residen en ‘Wilde’ (el cliente que envía vive
en Wilde).
7. Reportar información de sucursales que realizaron envíos durante 2020 o que tengan dirección en
Tucuman.
8. Listar datos personales de clientes que no realizaron envíos a la sucursal con nombre ‘La Amistad
1’.
9. Listar datos personales de carteros que realizaron envíos durante 2019 a clientes con DNI inferior a
27329882.
10. Listar los datos de los carteros que aún no hayan realizado ninguna entrega.                                                                         


PaquetesTuristicos = (codP, cantDias, cantPersonas, precio, disponible, destino, detalles)
Tipo_Promocion = (codPromo, detalle)
Promocion = (codPromo(FK), desde, hasta?, condición, descuento, codP(FK)) // El descuento es un % Compra (ticket, fecha)
Detalle= (ticket(FK), codP(FK), cantidad, precio_unitario, descuento) //Descuento es un monto y es 0 cuando no tiene descuento
Nota una compra puede incluir más de un paquete turistico diferente si es así tendrá más de un detalle

Realizar 1, 2, 3 y 4 en AR y 2, 3, 4, 5 y 6 en SQL

1. Listar la información de los paquetes turisticos que tienen promociones vigentes con descuento mayor al 20%, junto a la información de la promoción y el tipo de promoción.
2. Listar los paquetes turísticos que no tienen ni tuvieron promociones.
3. Listar los paquetes turísticos que tuvieron o tienen promociones de todos los tipos. 
4. Modificar el paquete turistico con código 12345 a precio=20.256 y cantDias=4
5. Listar los datos de los paquetes turísticos y cantidad de aquellos paquetes que estén en más de 10 ventas en el 2022. Ordenar por código de paquete ascendente.
6. Listar el importe total (sin considerar descuentos), el importe a cobrar (total menos descuentos) y la cantidad de detalles correspondientes a la compra nro 123456789.
Nota: Cuando se pide datos de X, mostrar todos los datos de la tabla a la que hace referencia




ALGEBRA RELACIONAL:

1.                                                                        
PaquetesTuristicos20% <= π codP, cantDias, cantPersonas, precio, disponible, destino, detalles, codPromo(FK), desde, hasta?, condición, descuento, detalle  (σ (descuento > 20) (Tipo_Promocion |x| (PaquetesTuristicos |x| Promocion)))

2.
PaquetesConPromocion <=  π codP, cantDias, cantPersonas, precio, disponible, destino, detalles (PaquetesTuristicos |x| Promocion)                                                                    
PaquetesSinPromocion <=  PaquetesTuristicos - PaquetesConPromocion

3.
TiposTodos <= π codPromo (Tipo_Promocion)
PaquetesxPromocion <= π codP, codPromo (Promocion)        
PaquetesTodasPromociones <= π codP, cantDias, cantPersonas, precio, disponible, destino, detalles  (PaquetesTuristicos |x| (PaquetesxPromocion % TiposTodos))                                                                        

4.
δ ((precio = 20.256) ^ (cantDias = 4)) <= (σ (codigo = 12345) (PaquetesTuristicos))

SQL: 

2.
SELECT p.codP, p.cantDias, p.cantPersonas, p.precio, p.disponible, p.destino, p.detalles                                                                         
FROM PaquetesTuristicos p
LEFT JOIN Promocion pro ON (p.codP = pro.codP)
WHERE (pro.codPromo IS NULL)

3.
SELECT p.codP, p.cantDias, p.cantPersonas, p.precio, p.disponible, p.destino, p.detalles                                                                         
FROM PaquetesTuristicos p
WHERE NOT EXISTS (
  SELECT *
  FROM Tipo_Promocion tpro
  WHERE NOT EXISTS (
    SELECT *
    FROM Promocion pro
    WHERE ((p.codP = pro.codP) and (pro.codPromo = tpro.codPromo))))

4.
UPDATE PaquetesTuristicos pt
SET (pt.precio = 20.256, pt.cantDias = 4)
WHERE (pt.codP = 12345)

5.
SELECT p.codP, p.cantDias, p.cantPersonas, p.precio, p.disponible, p.destino, p.detalles,  SUM (d.cantidad) as cantidad_paquetesTuristicos                                                                         
FROM PaquetesTuristicos p
INNER JOIN Detalle d ON (p.codP = d.codP)
INNER JOIN Compra c ON (d.ticket = c.ticket)
WHERE YEAR (c.fecha) = 2022 
GROUP BY p.codP, p.cantDias, p.cantPersonas, p.precio, p.disponible, p.destino, p.detalles               
HAVING COUNT (c.ticket) > 10
ORDER BY p.codP

6.
SELECT SUM (d.precio_unitario * d.cantidad) as importe_total, SUM (d.cantidad * (d.precio_unitario - d.descuento)) as importe_cobrar, COUNT (d.ticket) as cantidad_detalles
FROM Compra c
INNER JOIN Detalle d ON (c.ticket = d.ticket)
WHERE (c.ticket = 1234556789)
GROUP BY c.ticket

PaquetesTuristicos = (codP, cantDias, cantPersonas, precio, disponible, destino, detalles)
Tipo_Promocion = (codPromo, detalle)
Promocion = (codPromo(FK), desde, hasta?, condición, descuento, codP(FK)) // El descuento es un % 
Compra (ticket, fecha)
Detalle = (ticket(FK), codP(FK), cantidad, precio_unitario, descuento) //Descuento es un monto y es 0 cuando no tiene descuento
Nota una compra puede incluir más de un paquete turistico diferente si es así tendrá más de un detalle

1. Listar la información de los paquetes turisticos que tienen promociones vigentes con descuento mayor al 20%, junto a la información de la promoción y el tipo de promoción.
2. Listar los paquetes turísticos que no tienen ni tuvieron promociones.
3. Listar los paquetes turísticos que tuvieron o tienen promociones de todos los tipos. 
4. Modificar el paquete turistico con código 12345 a precio=20.256 y cantDias=4
5. Listar los datos de los paquetes turísticos y cantidad de aquellos paquetes que estén en más de 10 ventas en el 2022. Ordenar por código de paquete ascendente.
6. Listar el importe total (sin considerar descuentos), el importe a cobrar (total menos descuentos) y la cantidad de detalles correspondientes a la compra nro 123456789.
Nota: Cuando se pide datos de X, mostrar todos los datos de la tabla a la que hace referencia




                                                                         
