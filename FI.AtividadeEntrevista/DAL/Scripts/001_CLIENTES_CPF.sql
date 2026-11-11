/*
    Migração 001 - CPF do cliente

    Tabela CLIENTES
    - Adiciona a coluna CPF (somente os 11 dígitos, sem máscara)

    Procedures
    - FI_SP_IncClienteV2, FI_SP_AltCliente, FI_SP_ConsCliente e FI_SP_PesqCliente: passam a gravar/retornar o CPF
    - FI_SP_VerificaCliente: recebe o @ID do cliente a ser desconsiderado (alteração)
    - FI_SP_DelCliente: não existia no banco; é criada (vazia) e em seguida alterada
*/

IF COL_LENGTH('dbo.CLIENTES', 'CPF') IS NULL
BEGIN
    ALTER TABLE dbo.CLIENTES ADD CPF VARCHAR(11) NOT NULL
END
GO

ALTER PROC FI_SP_IncClienteV2
    @NOME          VARCHAR (50) ,
    @SOBRENOME     VARCHAR (255),
    @NACIONALIDADE VARCHAR (50) ,
    @CEP           VARCHAR (9)  ,
    @ESTADO        VARCHAR (2)  ,
    @CIDADE        VARCHAR (50) ,
    @LOGRADOURO    VARCHAR (500),
    @EMAIL         VARCHAR (2079),
    @TELEFONE      VARCHAR (15),
    @CPF           VARCHAR (11)
AS
BEGIN
	INSERT INTO CLIENTES (NOME, SOBRENOME, NACIONALIDADE, CEP, ESTADO, CIDADE, LOGRADOURO, EMAIL, TELEFONE, CPF) 
	VALUES (@NOME, @SOBRENOME,@NACIONALIDADE,@CEP,@ESTADO,@CIDADE,@LOGRADOURO,@EMAIL,@TELEFONE,@CPF)

	SELECT SCOPE_IDENTITY()
END
GO

ALTER PROC FI_SP_AltCliente
    @NOME          VARCHAR (50) ,
    @SOBRENOME     VARCHAR (255),
    @NACIONALIDADE VARCHAR (50) ,
    @CEP           VARCHAR (9)  ,
    @ESTADO        VARCHAR (2)  ,
    @CIDADE        VARCHAR (50) ,
    @LOGRADOURO    VARCHAR (500),
    @EMAIL         VARCHAR (2079),
    @TELEFONE      VARCHAR (15),
    @CPF           VARCHAR (11),
	@Id           BIGINT
AS
BEGIN
	UPDATE CLIENTES 
	SET 
		NOME = @NOME, 
		SOBRENOME = @SOBRENOME, 
		NACIONALIDADE = @NACIONALIDADE, 
		CEP = @CEP, 
		ESTADO = @ESTADO, 
		CIDADE = @CIDADE, 
		LOGRADOURO = @LOGRADOURO, 
		EMAIL = @EMAIL, 
		TELEFONE = @TELEFONE,
		CPF = @CPF
	WHERE Id = @Id
END
GO

ALTER PROC FI_SP_ConsCliente
	@ID BIGINT
AS
BEGIN
	IF(ISNULL(@ID,0) = 0)
		SELECT NOME, SOBRENOME, NACIONALIDADE, CEP, ESTADO, CIDADE, LOGRADOURO, EMAIL, TELEFONE, CPF, ID FROM CLIENTES WITH(NOLOCK)
	ELSE
		SELECT NOME, SOBRENOME, NACIONALIDADE, CEP, ESTADO, CIDADE, LOGRADOURO, EMAIL, TELEFONE, CPF, ID FROM CLIENTES WITH(NOLOCK) WHERE ID = @ID
END
GO

ALTER PROC FI_SP_PesqCliente
	@iniciarEm int,
	@quantidade int,
	@campoOrdenacao varchar(200),
	@crescente bit	
AS
BEGIN
	DECLARE @SCRIPT NVARCHAR(MAX)
	DECLARE @CAMPOS NVARCHAR(MAX)
	DECLARE @ORDER VARCHAR(50)
	
	IF(@campoOrdenacao = 'EMAIL')
		SET @ORDER =  ' EMAIL '
	ELSE
		SET @ORDER = ' NOME '

	IF(@crescente = 0)
		SET @ORDER = @ORDER + ' DESC'
	ELSE
		SET @ORDER = @ORDER + ' ASC'

	SET @CAMPOS = '@iniciarEm int,@quantidade int'
	SET @SCRIPT = 
	'SELECT ID, NOME, SOBRENOME, NACIONALIDADE, CEP, ESTADO, CIDADE, LOGRADOURO, EMAIL, TELEFONE, CPF FROM
		(SELECT ROW_NUMBER() OVER (ORDER BY ' + @ORDER + ') AS Row, ID, NOME, SOBRENOME, NACIONALIDADE, CEP, ESTADO, CIDADE, LOGRADOURO, EMAIL, TELEFONE, CPF FROM CLIENTES WITH(NOLOCK))
		AS ClientesWithRowNumbers
	WHERE Row > @iniciarEm AND Row <= (@iniciarEm+@quantidade) ORDER BY'
	
	SET @SCRIPT = @SCRIPT + @ORDER
			
	EXECUTE SP_EXECUTESQL @SCRIPT, @CAMPOS, @iniciarEm, @quantidade

	SELECT COUNT(1) FROM CLIENTES WITH(NOLOCK)
END
GO

-- @ID: cliente que deve ser desconsiderado na verificação (utilizado na alteração); 0 na inclusão
ALTER PROC FI_SP_VerificaCliente
	@CPF VARCHAR(11),
	@ID  BIGINT = 0
AS
BEGIN
	SELECT 1 FROM CLIENTES WITH(NOLOCK) WHERE CPF = @CPF AND ID <> ISNULL(@ID, 0)
END
GO

-- FI_SP_DelCliente não existe no banco original: cria uma versão vazia para que o ALTER abaixo possa ser aplicado
IF OBJECT_ID('dbo.FI_SP_DelCliente', 'P') IS NULL
    EXEC('CREATE PROC FI_SP_DelCliente AS RETURN')
GO

ALTER PROC FI_SP_DelCliente
	@ID BIGINT
AS
BEGIN
	DELETE CLIENTES WHERE ID = @ID
END
GO
