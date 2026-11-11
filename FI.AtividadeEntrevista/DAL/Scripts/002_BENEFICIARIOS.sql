/*
    Migração 002 - Beneficiários do cliente

    Procedures
    - FI_SP_IncBenef: inclui um beneficiário e retorna o ID gerado
    - FI_SP_ConsBenef: lista os beneficiários de um cliente
    - FI_SP_DelBenef: exclui um beneficiário
    - FI_SP_AltBenef: já existe no banco original e é utilizada sem alteração
*/

IF OBJECT_ID('dbo.FI_SP_IncBenef', 'P') IS NULL
    EXEC('CREATE PROC FI_SP_IncBenef AS RETURN')
GO

ALTER PROC FI_SP_IncBenef
    @NOME      VARCHAR (50),
    @CPF       VARCHAR (11),
    @IDCLIENTE BIGINT
AS
BEGIN
	INSERT INTO BENEFICIARIOS (NOME, CPF, IDCLIENTE)
	VALUES (@NOME, @CPF, @IDCLIENTE)

	SELECT SCOPE_IDENTITY()
END
GO

IF OBJECT_ID('dbo.FI_SP_ConsBenef', 'P') IS NULL
    EXEC('CREATE PROC FI_SP_ConsBenef AS RETURN')
GO

ALTER PROC FI_SP_ConsBenef
	@IDCLIENTE BIGINT
AS
BEGIN
	SELECT ID, CPF, NOME, IDCLIENTE FROM BENEFICIARIOS WITH(NOLOCK) WHERE IDCLIENTE = @IDCLIENTE
END
GO

IF OBJECT_ID('dbo.FI_SP_DelBenef', 'P') IS NULL
    EXEC('CREATE PROC FI_SP_DelBenef AS RETURN')
GO

ALTER PROC FI_SP_DelBenef
	@ID BIGINT
AS
BEGIN
	DELETE BENEFICIARIOS WHERE ID = @ID
END
GO
