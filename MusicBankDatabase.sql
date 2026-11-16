-- =============================================
-- PROJETO: MusicBank
-- SCRIPT DE CRIAÇÃO DO BANCO DE DADOS
-- =============================================

IF DB_ID('MusicBankDB') IS NOT NULL
BEGIN
    ALTER DATABASE MusicBankDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MusicBankDB;
END
GO

CREATE DATABASE MusicBankDB;
GO

USE MusicBankDB;
GO

-- TABELA USUARIO
CREATE TABLE Usuario (
    IdUsuario INT IDENTITY(1, 1) PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Email VARCHAR(50) NOT NULL UNIQUE,
    Senha VARCHAR(255) NOT NULL,
    TipoUsuario VARCHAR(20) NOT NULL
        CHECK (TipoUsuario IN ('Admin', 'Usuario')),
    Ativo BIT NOT NULL DEFAULT 1
);
GO

-- TABELA MUSICA
CREATE TABLE Musica (
    IdMusica INT IDENTITY(1, 1) PRIMARY KEY,
    NomeMusica VARCHAR(50) NOT NULL,
    ClipeMusica VARCHAR(MAX),
    NomeArtista VARCHAR(100),
    DataLancamento DATE NOT NULL,
        CHECK (YEAR(DataLancamento) <= YEAR(GETDATE())),
    Duracao INT NOT NULL,
        CHECK ( Duracao > 0 ),
    ImagemCapa VARCHAR(MAX)
);
GO

-- TABELA GENEROMUSICAL
CREATE TABLE GeneroMusical (
    IdGeneroMusical INT IDENTITY(1, 1) PRIMARY KEY,
    GeneroMusical VARCHAR(50) NOT NULL UNIQUE
);
GO

-- TABELA MUSICAGENERO
CREATE TABLE MusicaGenero (
    IdMusica INT NOT NULL,
    IdGeneroMusical INT NOT NULL,

    CONSTRAINT PK_MusicaGenero PRIMARY KEY(IdMusica, IdGeneroMusical),
    CONSTRAINT FK_MusicaMusicaGenero
        FOREIGN KEY(IdMusica) REFERENCES Musica(IdMusica) ON DELETE CASCADE,
    CONSTRAINT FK_GeneroMusical
        FOREIGN KEY(IdGeneroMusical) REFERENCES GeneroMusical(IdGeneroMusical) ON DELETE CASCADE
);
GO

-- TRIGGER: Exclusão de Usuário - SOFT DELETE
CREATE TRIGGER TR_UsuarioSoftDelete
ON Usuario
INSTEAD OF DELETE
AS
BEGIN
    UPDATE Usuario
    SET Ativo = 0
    WHERE IdUsuario IN
        (SELECT IdUsuario FROM deleted)
END;
GO

-- DADOS INICIAIS (SEED DATA)
INSERT INTO Usuario (Nome, Email, Senha, TipoUsuario)
VALUES
('Ana Silva', 'ana@email.com', 
    CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', 'admin123'), 2),
    'Admin');
GO

INSERT INTO GeneroMusical (GeneroMusical)
VALUES
('Jazz'),
('Forró'),
('Funk');
GO

INSERT INTO Musica (NomeMusica, ClipeMusica, NomeArtista, DataLancamento, Duracao, ImagemCapa)
VALUES
( 'By Your Side',
  'https://youtu.be/C8QJmI_V3j4?si=FHQDbTwyyJO19BbA',
  'SADE',
  '2000-10-03',
  265,
  'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSf-uwsA1gQYh3rY1MW_O96jjkdp6VYEkIG07DcgWB2KA&s=10'
);
GO

INSERT INTO MusicaGenero (IdMusica, IdGeneroMusical)
VALUES
(1, 1); -- JAZZ
GO

-- CONSULTAS DE CONFERÊNCIA
SELECT * FROM Musica;
SELECT * FROM GeneroMusical;
SELECT * FROM Usuario;

SELECT 
    m.NomeMusica, 
    YEAR(m.DataLancamento) AS AnoLancamento, 
    m.Duracao, 
    g.GeneroMusical AS GeneroMusical
FROM Musica m
    JOIN MusicaGenero mg
        ON m.IdMusica = mg.IdMusica
    JOIN GeneroMusical g
        ON g.IdGeneroMusical = mg.IdGeneroMusical
ORDER BY m.NomeMusica;
GO
