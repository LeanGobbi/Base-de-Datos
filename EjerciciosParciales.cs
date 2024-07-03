
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
DELETE FROM Pedidos p
WHERE (p.id_ped = 12345)
DELETE FROM Detalle_Pedido dp
WHERE (dp.id_ped = 12345)

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
SELECT s.DNI, s.CUIL, s.apellido, s.nombre, s.domicilio, s.telefono, COUNT (p.idprestamo) as cantidad_prestamos
FROM Socio s
INNER JOIN Prestamo p ON (p.DNI = s.DNI)
WHERE YEAR (p.fechaPrestamo) = 2023
GROUP BY s.DNI, s.CUIL, s.apellido, s.nombre, s.domicilio, s.telefono
HAVING cantidad_prestamos > ALL (
  SELECT COUNT (p.id_prestamo)
  FROM Prestamo p
  WHERE YEAR (p.fechaPrestamo) = 2023
  GROUP BY s.DNI)
                                                                         
  
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
