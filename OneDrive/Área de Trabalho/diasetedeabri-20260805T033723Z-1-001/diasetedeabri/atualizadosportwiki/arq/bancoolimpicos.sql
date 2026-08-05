-- 1. Banco de Dados
CREATE DATABASE bdolimpicoJueGu;
USE bdolimpicoJueGu;

-- 2. Modalidades
CREATE TABLE modalidades(
    codModalidade int primary key auto_increment,
    nomeModalidade varchar(50)
);

INSERT INTO modalidades (nomeModalidade) VALUES
('Atletismo'),('Natação'),('Vôlei de Quadra'),('Vôlei de Praia'), ('Ginástica Artística');

-- 3. Provas
CREATE TABLE provas(
    codProva int primary key auto_increment,
    prova varchar(100),
    codModalidade int,
    CONSTRAINT fkModProva FOREIGN KEY (codModalidade) REFERENCES modalidades(codModalidade)
);

INSERT INTO provas (prova, codModalidade) VALUES
('10000m Masculino', 1), ('100m Feminino', 1), ('100m Masculino', 1), ('100m com barreiras Feminino', 1),
('110m com Barreiras Masculino', 1), ('1500m Masculino', 1), ('200m Feminino', 1), ('200m Masculino', 1),
('20km Marcha Atlética Feminina', 1), ('20km Marcha Atlética Masculino', 1), ('3000m com Obstáculos Feminino', 1),
('3000m com Obstáculos Masculino', 1), ('400m Feminino', 1), ('400m Feminino Feminino', 1), ('400m Masculino', 1),
('400m com Barreiras Feminina', 1), ('400m com Barreiras Feminino', 1), ('400m com Barreiras Masculino', 1),
('5000m Feminino', 1), ('5000m Masculino', 1), ('50km Marcha Atlética', 1), ('60m Masculino', 1),
('800m Feminino', 1), ('800m Masculino', 1), ('80m com Barreiras Feminino', 1), ('Arremesso de Peso Feminino', 1),
('Arremesso de Peso Masculino', 1), ('Arremesso do Peso Masculino', 1), ('Cross Country Masculino', 1),
('Decatlon', 1), ('Decatlon Masculino', 1), ('Heptatlo Feminino', 1), ('Lançamento de Dardo Feminino', 1),
('Lançamento de Dardo Masculino', 1), ('Lançamento de Disco Feminino', 1), ('Lançamento de Disco Masculino', 1),
('Lançamento do Dardo', 1), ('Lançamento do Dardo Feminino', 1), ('Lançamento do Disco Feminino', 1),
('Lançamento do Disco Masculino', 1), ('Lançamento do Martelo Masculino', 1), ('Maratona Feminina', 1),
('Maratona Masculina', 1), ('Marcha Atletica Masculina 20km', 1), ('Marcha Atlética 20 Km Feminino', 1),
('Marcha Atlética 20 Km Masculino', 1), ('Marcha Atlética 50 Km Masculino', 1), ('Marcha Atlética Feminino 20km', 1),
('Pentatlo Feminino', 1), ('Revezamento 4 x 100m Feminino', 1), ('Revezamento 4 x 100m Masculino', 1),
('Revezamento 4 x 400 Masculino', 1), ('Revezamento 4 x 400m Feminino', 1), ('Revezamento 4 x 400m Masculino', 1),
('Revezamento 4 x 400m Misto', 1), ('Revezamento Marcha Atlética Misto', 1), ('Revezametno 4 x 400m Masculino', 1),
('Salto Triplo Feminino', 1), ('Salto Triplo Masculino', 1), ('Salto com Vara Feminino', 1), ('Salto com Vara Masculino', 1),
('Salto em Altura Feminino', 1), ('Salto em Altura Masculino', 1), ('Salto em Distância Feminino', 1), ('Salto em Distância Masculino', 1);

INSERT INTO provas (prova, codModalidade) VALUES
('100m Borboleta Feminino', 2), ('100m Borboleta Masculino', 2), ('100m Costas Feminino', 2), ('100m Costas Masculino', 2),
('100m Livre Feminino', 2), ('100m Livre Masculino', 2), ('100m Peito Feminino', 2), ('100m Peito Masculino', 2),
('1500m Livre Feminino', 2), ('1500m Livre Masculino', 2), ('50m Livre Feminino', 2), ('50m Livre Masculino', 2);

-- 4. Localização
CREATE TABLE estados (
  codEstado int primary key auto_increment,
  nomeEstado varchar(255) NOT NULL
);

INSERT INTO estados (nomeEstado) VALUES
('Acre'),('Alagoas'),('Alemanha'),('Amapá'),('Amazonas'),('Argentina'),('Armênia'),('Austrália'),
('Bahia'),('Bielorussia'),('Bélgica'),('Ceará'),('China'),('Colômbia'),('Croácia'),('Cuba'),
('Distrito Federal'),('EUA'),('Espanha'),('Espírito Santo'),('França'),('Goiás'),('Grã-Bretanha'),
('Holanda'),('Hungria'),('Inglaterra'),('Itália'),('Japão'),('Lituânia'),('Maranhão'),('Mato Grosso'),
('Mato Grosso do Sul'),('Minas Gerais'),('Paraná'),('Paraíba'),('Pará'),('Pernambuco'),('Piauí'),
('Polônia'),('Portugal'),('Rio Grande do Norte'),('Rio Grande do Sul'),('Rio de Janeiro'),('Rondônia'),
('Roraima'),('Santa Catarina'),('Sergipe'),('Suiça'),('Suécia'),('São Paulo'),('Sérvia'),('Uruguai'),('nan');

CREATE TABLE cidades(
    codCidade int primary key auto_increment,
    nomeCidade varchar(255) NOT NULL,
    codEstado int,
    foreign key (codEstado) REFERENCES estados(codEstado)
);

INSERT INTO cidades (nomeCidade, codEstado) VALUES ('São Paulo', 50), ('Guarulhos', 50), ('Rio de Janeiro', 43), ('Aracaju', 47);

-- 5. Edição
CREATE TABLE edicao (
  codedicao int primary key auto_increment,
  ano int,
  sede varchar(30)
);

INSERT INTO edicao (ano, sede) VALUES
(1900, 'Paris'), (1920, 'Antuérpia'), (1924, 'Paris'), (1932, 'Los Angeles'), (1936, 'Berlim'),
(1948, 'Londres'), (1952, 'Helsinque'), (1956, 'Melbourne'), (1960, 'Roma'), (1964, 'Tóquio'),
(1968, 'Cidade do México'), (1972, 'Munique'), (1976, 'Montreal'), (1980, 'Moscou'), (1984, 'Los Angeles'),
(1988, 'Seul'), (1992, 'Barcelona'), (1996, 'Atlanta'), (2000, 'Sydney'), (2004, 'Atenas'),
(2008, 'Pequim'), (2012, 'Londres'), (2016, 'Rio de Janeiro'), (2020, 'Tóquio'), (2024, 'Paris'),
(2028, 'Los Angeles'), (2032, 'Brisbane');

-- 6. Atletas
CREATE TABLE atletas (
  codAtleta int primary key auto_increment,
  nomeAtleta varchar(255),
  dataNascimento varchar(20),
  sexo char(1),
  altura decimal(5,2) DEFAULT NULL,
  peso decimal(5,2) DEFAULT NULL,
  codCidade int,
  CONSTRAINT fkAtletasCid FOREIGN KEY (codCidade) REFERENCES cidades(codCidade)
);

-- Inserindo Atletas (Atenção: ID da cidade 1 é São Paulo no meu INSERT simplificado)
INSERT INTO atletas (nomeAtleta, dataNascimento, sexo, altura, peso, codCidade) VALUES
('Adhemar Ferreira da Silva', '1927-09-29', 'M', NULL, NULL, 1),
('Aderval Luiz Arvani', '1949-01-07', 'M', NULL, NULL, 1),
('Stephanie Balduccini', '2004-09-20', 'F', NULL, NULL, 1),
('Thaissa Barbosa Presti', '1988-04-26', 'F', NULL, NULL, 1),
('Wanda dos Santos', '1932-06-01', 'F', NULL, NULL, 1),
('Manuel dos Santos Filho', '1939-02-22', 'M', NULL, NULL, 1),
('Marcelo Teles Negrão', '1972-10-10', 'M', NULL, NULL, 1),
('Fofão', '1970-03-10', 'F', NULL, NULL, 1),
('Rebeca Andrade', '1999-05-08', 'F', 1.51, 46.00, 4); -- PESO CORRIGIDO PARA DECIMAL

ALTER TABLE atletas
MODIFY COLUMN peso DECIMAL(5,2);

-- 7. Resultados Atletas (TABELA CORRIGIDA)
CREATE TABLE resultadosatletas (
  codAtletaRes int primary key auto_increment,
  codAtleta int,
  codProva int,
  edicao int,
  resultado varchar(255) DEFAULT NULL,
  medalha varchar(255) DEFAULT NULL,
  foreign key (codatleta) references atletas(codAtleta),
  foreign key (codProva) references provas(codProva), -- AGORA APONTA PARA A TABELA PROVAS
  foreign key (edicao) references edicao(codedicao)  -- AGORA APONTA PARA A TABELA EDICAO
);

-- 8. Inserção de Resultados
INSERT INTO resultadosatletas (codAtleta, codProva, edicao, resultado, medalha) VALUES
(1,'59','6','8ºLugar',''),
(1,'59','7','1ºLugar','Ouro'),
(1,'59','8','8ºLugar','Ouro'),
(1,'59','9','14ºLugar','');

CREATE TABLE usuarios (
  id INT PRIMARY KEY AUTO_INCREMENT,
  username VARCHAR(100) UNIQUE NOT NULL,
  password_hash VARCHAR(255) NOT NULL, 
  role VARCHAR(30) NOT NULL, 
  ativo TINYINT(1) DEFAULT 1,
  criado_em DATETIME DEFAULT CURRENT_TIMESTAMP
);


-- Inserindo Provas de Ginástica (ID 5)
INSERT INTO provas (prova, codModalidade) VALUES
('Argolas Masculino', 5), ('Barra Fixa', 5), ('Barra Fixa Masculino', 5), ('Barras Assimétricas Feminino', 5),
('Barras Paralelas Masculino', 5), ('Cavalo com Alças Masculino', 5), ('Equipes Feminino', 5), ('Equipes Masculino', 5),
('Individual All-Around Feminino', 5), ('Individual All-Around Masculino', 5), ('Individual Geral Feminino', 5),
('Individual Geral Masculino', 5), ('Salto sobre a mesa Feminino', 5), ('Salto sobre a mesa Masculino', 5),
('Solo Feminino', 5), ('Solo Masculino', 5), ('Trave de Equilibrio Feminino', 5);

-- Resultados da Rebeca (ID 9)
INSERT INTO resultadosatletas (codAtleta, codProva, edicao, resultado, medalha) VALUES
(9,'70','23','11ºLugar',''),
(9,'71','24','1ºLugar','Ouro');

DELIMITER $$
create procedure sp_GetAtletasByEdicao(in p_edicao int)
begin
 select distinct
 a.codAtleta,
 a.nomeAtleta,
 a.dataNascimento,
 a.sexo,
 a.codCidade,
 m.codModalidade,
 m.nomeModalidade
 from resultadosatletas r
 join provas p on p.codProva = r.codProva
 join atletas a on a.codAtleta = r.codAtleta
 left join modalidades m on m.codModalidade = p.codModalidade
where r.edicao = p_edicao;
END$$
DELIMITER ;

-- =============================================
-- SP 1: Busca dados completos do atleta por ID
-- =============================================
DELIMITER $$
CREATE PROCEDURE sp_BuscarAtletaPorId(in id INT)
BEGIN

    SELECT
        a.codAtleta,
        a.nomeAtleta,
        a.dataNascimento,
        a.sexo,
        c.codCidade,
        c.nomeCidade,
        e.nomeEstado,
        m.codModalidade,
        m.nomeModalidade,
        p.Prova,
        r.resultado,
        r.medalha
    FROM atletas a
    JOIN cidades c          ON c.codCidade    = a.codCidade
    JOIN estados e          ON e.codEstado    = c.codEstado


    JOIN resultadosatletas r ON r.codAtleta   = a.codAtleta
    JOIN provas p           ON p.codProva     = r.codProva
    JOIN modalidades m      ON m.codModalidade = p.codModalidade
    WHERE a.codAtleta = id;
END$$
DELIMITER ;

drop procedure sp_BuscarAtletaPorId; 

call sp_BuscarAtletaPorId (1);

-- =============================================
-- SP 2: Busca resultados e edições do atleta por ID
-- =============================================
DELIMITER $$
CREATE PROCEDURE sp_BuscarResultadosAtletaPorId (in id INT)
BEGIN

    SELECT
        p.Prova,
        e.ano,
        e.sede,
        r.resultado,
        r.medalha
    FROM resultadosatletas r
    JOIN provas p  ON p.codProva    = r.codProva
    JOIN edicao e  ON e.codedicao   = r.edicao
    WHERE r.codAtleta = @id;
END $$
DELIMITER ;
call sp_BuscarResultadosAtletaPorId(1);
call sp_GetAtletasByEdicao(7);

-- 9. Conferência
SELECT * FROM modalidades;

Select * from usuarios;
SELECT * FROM provas;
SELECT * FROM atletas;
SELECT * FROM edicao;
SELECT * FROM resultadosatletas;
select * from cidades;
select * from estados;