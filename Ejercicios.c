
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
ClientesTODOSArticulos <= π nombre,apellido Cliente |x| (ClientesxArticulo % ArticulosTodos)


SQL:

2.Listar el contacto de los clientes que tienen pedidos en el año 2023 y también en 2024.
3.Eliminar el Pedido con id_ped 12345.
4.Listar apellido y nombre de ios clientes que compraron todos los artículos. En SQL ordenar por apellido.
5.Listar para cada cliente, el DN!, nombre, apellido y cantidad de pedidos. Ordenar por cantidad de pedidos descendentemente.
6.Listar los artículos que se vendieron más de 15 veces durante el 2023.

σ

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








Resolver del 1 al 4 AR y del 2 al 6 en SQL
Socio-(DNI,CUIL, apellido, nombre, domicilio, telefono)
Libro-(ISBN, título, autor, género, stock, precio)
(runchy a
muchar
Tino a mucho.. Total unbarb Rancial musha Parcial I
rolan
Vancial andre) IR
Préstamo (id_prestamo fecha Prestamo, fecha Devolucion Tentativa, fechaDevEfectiva?, DNI(FK)) Detalle Préstamo=(id_prestamo (FK). ISBN (FK), precio, estado_dev?)
375
1. Listar los datos de los socios que tienen préstamos con devoluciones (fecha DevEfectiva) mayor a la fecha pactada y también tienen devoluciones donde el estado_dev es malo.
Listar los socios que solo tienen préstamos en el mes actual. (NO PORTA S. DEVOLUERON O
ONO)
Modificar el precio de los libros incrementándolo en un 10%.
4. Listar los socios que tuvieron prestados todos los libros.
5. Informar la cantidad de préstamos del mes actual.
6. Listar todos los datos del socio o de los socios que tienen mayor cantidad de préstamos en el año 2023.