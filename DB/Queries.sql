CREATE DATABASE BancaEnLineaDB
GO

USE BancaEnLineaDB
GO

/* ============================================================
   TABLA: rol
   ============================================================ */
CREATE TABLE dbo.rol (
    id_rol INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(50) NOT NULL,
    descripcion NVARCHAR(200) NULL,
    activo BIT NOT NULL CONSTRAINT DF_rol_activo DEFAULT 1,

    CONSTRAINT PK_rol PRIMARY KEY (id_rol),
    CONSTRAINT UQ_rol_nombre UNIQUE (nombre)
);
GO

/* ============================================================
   TABLA: usuario
   ============================================================ */
CREATE TABLE dbo.usuario (
    id_usuario INT IDENTITY(1,1) NOT NULL,
    id_rol INT NOT NULL,
    nombres NVARCHAR(100) NOT NULL,
    apellidos NVARCHAR(100) NOT NULL,
    dpi VARCHAR(20) NOT NULL,
    email VARCHAR(150) NOT NULL,
    telefono VARCHAR(20) NULL,
    direccion NVARCHAR(250) NULL,
    estado VARCHAR(20) NOT NULL CONSTRAINT DF_usuario_estado DEFAULT 'Activo',
    fecha_creacion DATETIME2(0) NOT NULL CONSTRAINT DF_usuario_fecha_creacion DEFAULT SYSDATETIME(),

    CONSTRAINT PK_usuario PRIMARY KEY (id_usuario),
    CONSTRAINT FK_usuario_rol FOREIGN KEY (id_rol) REFERENCES dbo.rol(id_rol),
    CONSTRAINT UQ_usuario_dpi UNIQUE (dpi),
    CONSTRAINT UQ_usuario_email UNIQUE (email),
    CONSTRAINT CK_usuario_estado CHECK (estado IN ('Activo', 'Inactivo'))
);
GO

/* ============================================================
   TABLA: cuenta
   ============================================================ */
CREATE TABLE dbo.cuenta (
    id_cuenta INT IDENTITY(1,1) NOT NULL,
    id_usuario INT NOT NULL,
    numero_cuenta VARCHAR(30) NOT NULL,
    tipo_cuenta VARCHAR(20) NOT NULL,
    saldo_actual DECIMAL(18,2) NOT NULL CONSTRAINT DF_cuenta_saldo DEFAULT 0,
    estado VARCHAR(20) NOT NULL CONSTRAINT DF_cuenta_estado DEFAULT 'Activa',
    fecha_apertura DATETIME2(0) NOT NULL CONSTRAINT DF_cuenta_fecha_apertura DEFAULT SYSDATETIME(),

    CONSTRAINT PK_cuenta PRIMARY KEY (id_cuenta),
    CONSTRAINT FK_cuenta_usuario FOREIGN KEY (id_usuario) REFERENCES dbo.usuario(id_usuario),
    CONSTRAINT UQ_cuenta_numero UNIQUE (numero_cuenta),
    CONSTRAINT CK_cuenta_tipo CHECK (tipo_cuenta IN ('Ahorro', 'Monetaria')),
    CONSTRAINT CK_cuenta_estado CHECK (estado IN ('Activa', 'Inactiva', 'Bloqueada')),
    CONSTRAINT CK_cuenta_saldo CHECK (saldo_actual >= 0)
);
GO

/* ============================================================
   TABLA: beneficiario
   ============================================================ */
CREATE TABLE dbo.beneficiario (
    id_beneficiario INT IDENTITY(1,1) NOT NULL,
    id_usuario_origen INT NOT NULL,
    id_cuenta_destino INT NOT NULL,
    alias NVARCHAR(100) NOT NULL,
    estado VARCHAR(20) NOT NULL CONSTRAINT DF_beneficiario_estado DEFAULT 'Activo',
    fecha_agregado DATETIME2(0) NOT NULL CONSTRAINT DF_beneficiario_fecha DEFAULT SYSDATETIME(),

    CONSTRAINT PK_beneficiario PRIMARY KEY (id_beneficiario),
    CONSTRAINT FK_beneficiario_usuario_origen FOREIGN KEY (id_usuario_origen) REFERENCES dbo.usuario(id_usuario),
    CONSTRAINT FK_beneficiario_cuenta_destino FOREIGN KEY (id_cuenta_destino) REFERENCES dbo.cuenta(id_cuenta),
    CONSTRAINT UQ_beneficiario_usuario_cuenta UNIQUE (id_usuario_origen, id_cuenta_destino),
    CONSTRAINT CK_beneficiario_estado CHECK (estado IN ('Activo', 'Inactivo'))
);
GO

/* ============================================================
   TABLA: transferencia
   ============================================================ */
CREATE TABLE dbo.transferencia (
    id_transferencia INT IDENTITY(1,1) NOT NULL,
    id_cuenta_origen INT NOT NULL,
    id_cuenta_destino INT NOT NULL,
    monto DECIMAL(18,2) NOT NULL,
    descripcion NVARCHAR(250) NULL,
    fecha_transferencia DATETIME2(0) NOT NULL CONSTRAINT DF_transferencia_fecha DEFAULT SYSDATETIME(),
    estado VARCHAR(20) NOT NULL CONSTRAINT DF_transferencia_estado DEFAULT 'Completada',
    referencia VARCHAR(50) NOT NULL,

    CONSTRAINT PK_transferencia PRIMARY KEY (id_transferencia),
    CONSTRAINT FK_transferencia_cuenta_origen FOREIGN KEY (id_cuenta_origen) REFERENCES dbo.cuenta(id_cuenta),
    CONSTRAINT FK_transferencia_cuenta_destino FOREIGN KEY (id_cuenta_destino) REFERENCES dbo.cuenta(id_cuenta),
    CONSTRAINT UQ_transferencia_referencia UNIQUE (referencia),
    CONSTRAINT CK_transferencia_monto CHECK (monto > 0),
    CONSTRAINT CK_transferencia_estado CHECK (estado IN ('Completada', 'Rechazada', 'Anulada')),
    CONSTRAINT CK_transferencia_cuentas_diferentes CHECK (id_cuenta_origen <> id_cuenta_destino)
);
GO

/* ============================================================
   TABLA: movimiento_cuenta
   ============================================================ */
CREATE TABLE dbo.movimiento_cuenta (
    id_movimiento INT IDENTITY(1,1) NOT NULL,
    id_cuenta INT NOT NULL,
    tipo_movimiento VARCHAR(20) NOT NULL,
    categoria VARCHAR(30) NOT NULL,
    monto DECIMAL(18,2) NOT NULL,
    saldo_anterior DECIMAL(18,2) NOT NULL,
    saldo_posterior DECIMAL(18,2) NOT NULL,
    fecha_movimiento DATETIME2(0) NOT NULL CONSTRAINT DF_movimiento_fecha DEFAULT SYSDATETIME(),
    referencia_tipo VARCHAR(30) NULL,
    referencia_id INT NULL,
    descripcion NVARCHAR(250) NULL,

    CONSTRAINT PK_movimiento_cuenta PRIMARY KEY (id_movimiento),
    CONSTRAINT FK_movimiento_cuenta FOREIGN KEY (id_cuenta) REFERENCES dbo.cuenta(id_cuenta),
    CONSTRAINT CK_movimiento_tipo CHECK (tipo_movimiento IN ('Credito', 'Debito')),
    CONSTRAINT CK_movimiento_categoria CHECK (
        categoria IN (
            'Apertura',
            'Transferencia',
            'DesembolsoPrestamo',
            'PagoPrestamo',
            'Ajuste'
        )
    ),
    CONSTRAINT CK_movimiento_monto CHECK (monto > 0),
    CONSTRAINT CK_movimiento_saldos CHECK (saldo_anterior >= 0 AND saldo_posterior >= 0)
);
GO

/* ============================================================
   TABLA: solicitud_prestamo
   ============================================================ */
CREATE TABLE dbo.solicitud_prestamo (
    id_solicitud_prestamo INT IDENTITY(1,1) NOT NULL,
    id_usuario INT NOT NULL,
    id_cuenta_desembolso INT NOT NULL,
    monto_solicitado DECIMAL(18,2) NOT NULL,
    plazo_meses INT NOT NULL,
    destino_prestamo NVARCHAR(200) NOT NULL,
    estado VARCHAR(20) NOT NULL CONSTRAINT DF_solicitud_estado DEFAULT 'Pendiente',
    fecha_solicitud DATETIME2(0) NOT NULL CONSTRAINT DF_solicitud_fecha DEFAULT SYSDATETIME(),
    observaciones NVARCHAR(500) NULL,
    aprobado_por INT NULL,
    fecha_resolucion DATETIME2(0) NULL,

    CONSTRAINT PK_solicitud_prestamo PRIMARY KEY (id_solicitud_prestamo),
    CONSTRAINT FK_solicitud_usuario FOREIGN KEY (id_usuario) REFERENCES dbo.usuario(id_usuario),
    CONSTRAINT FK_solicitud_cuenta_desembolso FOREIGN KEY (id_cuenta_desembolso) REFERENCES dbo.cuenta(id_cuenta),
    CONSTRAINT FK_solicitud_aprobado_por FOREIGN KEY (aprobado_por) REFERENCES dbo.usuario(id_usuario),
    CONSTRAINT CK_solicitud_monto CHECK (monto_solicitado > 0),
    CONSTRAINT CK_solicitud_plazo CHECK (plazo_meses > 0),
    CONSTRAINT CK_solicitud_estado CHECK (estado IN ('Pendiente', 'Aprobada', 'Rechazada'))
);
GO

/* ============================================================
   TABLA: prestamo
   Nota:
   - Al aprobar una solicitud se crea un préstamo en estado 'Aprobado'.
   - Al desembolsarlo cambia a 'Activo'.
   ============================================================ */
CREATE TABLE dbo.prestamo (
    id_prestamo INT IDENTITY(1,1) NOT NULL,
    id_solicitud_prestamo INT NOT NULL,
    id_usuario INT NOT NULL,
    monto_aprobado DECIMAL(18,2) NOT NULL,
    tasa_interes DECIMAL(9,6) NOT NULL,
    plazo_meses INT NOT NULL,
    saldo_pendiente DECIMAL(18,2) NOT NULL,
    fecha_desembolso DATETIME2(0) NULL,
    estado VARCHAR(20) NOT NULL CONSTRAINT DF_prestamo_estado DEFAULT 'Aprobado',

    CONSTRAINT PK_prestamo PRIMARY KEY (id_prestamo),
    CONSTRAINT FK_prestamo_solicitud FOREIGN KEY (id_solicitud_prestamo) REFERENCES dbo.solicitud_prestamo(id_solicitud_prestamo),
    CONSTRAINT FK_prestamo_usuario FOREIGN KEY (id_usuario) REFERENCES dbo.usuario(id_usuario),
    CONSTRAINT UQ_prestamo_solicitud UNIQUE (id_solicitud_prestamo),
    CONSTRAINT CK_prestamo_monto CHECK (monto_aprobado > 0),
    CONSTRAINT CK_prestamo_tasa CHECK (tasa_interes >= 0),
    CONSTRAINT CK_prestamo_plazo CHECK (plazo_meses > 0),
    CONSTRAINT CK_prestamo_saldo CHECK (saldo_pendiente >= 0),
    CONSTRAINT CK_prestamo_estado CHECK (estado IN ('Aprobado', 'Activo', 'Pagado', 'EnMora', 'Cancelado'))
);
GO

/* ============================================================
   TABLA: cuota_prestamo
   ============================================================ */
CREATE TABLE dbo.cuota_prestamo (
    id_cuota INT IDENTITY(1,1) NOT NULL,
    id_prestamo INT NOT NULL,
    numero_cuota INT NOT NULL,
    fecha_vencimiento DATE NOT NULL,
    capital_programado DECIMAL(18,2) NOT NULL,
    interes_programado DECIMAL(18,2) NOT NULL,
    mora_programada DECIMAL(18,2) NOT NULL CONSTRAINT DF_cuota_mora_programada DEFAULT 0,
    monto_total_programado DECIMAL(18,2) NOT NULL,
    capital_pagado DECIMAL(18,2) NOT NULL CONSTRAINT DF_cuota_capital_pagado DEFAULT 0,
    interes_pagado DECIMAL(18,2) NOT NULL CONSTRAINT DF_cuota_interes_pagado DEFAULT 0,
    mora_pagada DECIMAL(18,2) NOT NULL CONSTRAINT DF_cuota_mora_pagada DEFAULT 0,
    estado VARCHAR(20) NOT NULL CONSTRAINT DF_cuota_estado DEFAULT 'Pendiente',

    CONSTRAINT PK_cuota_prestamo PRIMARY KEY (id_cuota),
    CONSTRAINT FK_cuota_prestamo FOREIGN KEY (id_prestamo) REFERENCES dbo.prestamo(id_prestamo),
    CONSTRAINT UQ_cuota_prestamo_numero UNIQUE (id_prestamo, numero_cuota),
    CONSTRAINT CK_cuota_numero CHECK (numero_cuota > 0),
    CONSTRAINT CK_cuota_montos_programados CHECK (
        capital_programado >= 0 AND
        interes_programado >= 0 AND
        mora_programada >= 0 AND
        monto_total_programado > 0
    ),
    CONSTRAINT CK_cuota_montos_pagados CHECK (
        capital_pagado >= 0 AND
        interes_pagado >= 0 AND
        mora_pagada >= 0
    ),
    CONSTRAINT CK_cuota_total_programado CHECK (
        monto_total_programado = capital_programado + interes_programado + mora_programada
    ),
    CONSTRAINT CK_cuota_estado CHECK (estado IN ('Pendiente', 'Parcial', 'Pagada', 'Vencida'))
);
GO

/* ============================================================
   TABLA: pago_prestamo
   ============================================================ */
CREATE TABLE dbo.pago_prestamo (
    id_pago_prestamo INT IDENTITY(1,1) NOT NULL,
    id_prestamo INT NOT NULL,
    id_cuota INT NOT NULL,
    id_cuenta_origen INT NOT NULL,
    monto_pagado DECIMAL(18,2) NOT NULL,
    capital_abonado DECIMAL(18,2) NOT NULL CONSTRAINT DF_pago_capital DEFAULT 0,
    interes_abonado DECIMAL(18,2) NOT NULL CONSTRAINT DF_pago_interes DEFAULT 0,
    mora_abonada DECIMAL(18,2) NOT NULL CONSTRAINT DF_pago_mora DEFAULT 0,
    fecha_pago DATETIME2(0) NOT NULL CONSTRAINT DF_pago_fecha DEFAULT SYSDATETIME(),
    estado VARCHAR(20) NOT NULL CONSTRAINT DF_pago_estado DEFAULT 'Aplicado',
    registrado_por INT NOT NULL,

    CONSTRAINT PK_pago_prestamo PRIMARY KEY (id_pago_prestamo),
    CONSTRAINT FK_pago_prestamo FOREIGN KEY (id_prestamo) REFERENCES dbo.prestamo(id_prestamo),
    CONSTRAINT FK_pago_cuota FOREIGN KEY (id_cuota) REFERENCES dbo.cuota_prestamo(id_cuota),
    CONSTRAINT FK_pago_cuenta_origen FOREIGN KEY (id_cuenta_origen) REFERENCES dbo.cuenta(id_cuenta),
    CONSTRAINT FK_pago_registrado_por FOREIGN KEY (registrado_por) REFERENCES dbo.usuario(id_usuario),
    CONSTRAINT CK_pago_monto CHECK (monto_pagado > 0),
    CONSTRAINT CK_pago_abonos CHECK (
        capital_abonado >= 0 AND
        interes_abonado >= 0 AND
        mora_abonada >= 0
    ),
    CONSTRAINT CK_pago_total CHECK (
        monto_pagado = capital_abonado + interes_abonado + mora_abonada
    ),
    CONSTRAINT CK_pago_estado CHECK (estado IN ('Aplicado', 'Anulado'))
);
GO

/* ============================================================
   DATA INICIAL PARA PRUEBAS
   ============================================================ */
INSERT INTO dbo.rol (nombre, descripcion)
VALUES
    ('Administrador', 'Usuario interno para gestionar usuarios, cuentas y préstamos.'),
    ('Cliente', 'Usuario cliente que posee cuentas, realiza transferencias y solicita préstamos.');
GO

INSERT INTO dbo.usuario (
    id_rol,
    nombres,
    apellidos,
    dpi,
    email,
    telefono,
    direccion,
    estado
)
VALUES
    (1, 'Administrador', 'Sistema', '0000000000001', 'admin@demo.com', '55550001', 'Oficina central', 'Activo'),
    (2, 'Juan', 'Pérez', '1111111111111', 'juan.perez@demo.com', '55550002', 'Ciudad de Guatemala', 'Activo'),
    (2, 'María', 'García', '2222222222222', 'maria.garcia@demo.com', '55550003', 'Ciudad de Guatemala', 'Activo');
GO

INSERT INTO dbo.cuenta (
    id_usuario,
    numero_cuenta,
    tipo_cuenta,
    saldo_actual,
    estado
)
VALUES
    (2, '1000000001', 'Ahorro', 5000.00, 'Activa'),
    (3, '1000000002', 'Ahorro', 1500.00, 'Activa');
GO

INSERT INTO dbo.movimiento_cuenta (
    id_cuenta,
    tipo_movimiento,
    categoria,
    monto,
    saldo_anterior,
    saldo_posterior,
    referencia_tipo,
    referencia_id,
    descripcion
)
VALUES
    (1, 'Credito', 'Apertura', 5000.00, 0.00, 5000.00, 'Cuenta', 1, 'Saldo inicial de cuenta de prueba'),
    (2, 'Credito', 'Apertura', 1500.00, 0.00, 1500.00, 'Cuenta', 2, 'Saldo inicial de cuenta de prueba');
GO

SELECT * FROM dbo.rol;
SELECT * FROM dbo.usuario;
SELECT * FROM dbo.cuenta;
SELECT * FROM dbo.movimiento_cuenta;