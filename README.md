Los querys para crear la base de datos y la tabla, e insertar los datos fueron los siguientes:

CREATE DATABASE IF NOT EXISTS productos_db;
USE productos_db;

CREATE TABLE productos (
    id             INT AUTO_INCREMENT PRIMARY KEY,
    nombre         VARCHAR(100)  NOT NULL,
    descripcion    VARCHAR(255)  NULL,
    precio         DECIMAL(10,2) NOT NULL,
    stock          INT           NOT NULL DEFAULT 0,
    fecha_creacion DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO productos (nombre, descripcion, precio, stock) VALUES
('Monster', 'Bebida energetica', 48.00, 15),
('Red Bull',  'Lo mismo que lo de arriba pero mas caro',    79.00, 8);
