# FI.WebAtividadeEntrevista
Teste prático do processo seletivo. Adição do CPF do Cliente, cadastro de Beneficiários e Testes Unitários.

## Como executar
1. Abra `FI.WebAtividadeEntrevista.sln` no Visual Studio.
2. Compile a solution (os pacotes NuGet são restaurados automaticamente).
3. Execute o projeto `FI.WebAtividadeEntrevista` (F5) e acesse a aba **Clientes**.

O banco de dados (`FI.WebAtividadeEntrevista/App_Data/BancoDeDados.mdf`) já está versionado com todas as alterações aplicadas, no formato do SQL Server 2019. A conexão utiliza a instância padrão `(LocalDB)\MSSQLLocalDB`.

> Obs: se a máquina possuir uma versão mais nova do LocalDB (2022 ou 2025), a instância `MSSQLLocalDB` usará essa versão e converterá o `.mdf` automaticamente na primeira execução. A conversão não pode ser desfeita.

## Funcionalidades implementadas
### Decisões técnicas
- CPF armazenado somente com os 11 dígitos: A máscara é aplicada apenas na exibição.
- Regras de negócio na camada BLL: As validações (CPF válido, duplicidade) ficam nos BOs (`BoCliente`, `BoBeneficiario`), que lançam `RegraNegocioException` com a mensagem exibida ao usuário. O front-end repete as validações apenas para dar retorno imediato.
- Camada de serviço no projeto Web: `ClienteService` converte os modelos da tela e orquestra a gravação do cliente e dos beneficiários, com o `ClienteController` cuidando somente do fluxo HTTP.
- Interfaces nos DAOs (`IDaoCliente`, `IDaoBeneficiario`): recebidas pelo construtor dos BOs, para permitir os testes unitários sem banco de dados.
- Beneficiários validados antes de gravar o cliente: Se algum beneficiário for inválido, nada é gravado. Ao salvar, a lista da tela é sincronizada com o banco: inclui os novos, altera os existentes e exclui os removidos.
- Máscara e validação de CPF em JavaScript (`Scripts/Common/FI.Cpf.js`) sem plugins: Basta adicionar `data-mascara="cpf"` ao campo.
### CPF do cliente
- Campo CPF nas telas de inclusão e alteração, seguindo o padrão visual dos demais campos.
- Preenchimento obrigatório, com máscara `999.999.999-99`.
- Validação dos dígitos verificadores, no navegador e no servidor.
- Não permite cadastrar um CPF já existente.
### Beneficiários
- Botão "Beneficiários" nas telas de inclusão e alteração, que abre um pop-up com 'CPF' e 'Nome'.
- Grid com os beneficiários incluídos, permitindo Alterar e Excluir Beneficiários.
- CPF com máscara e validação dos dígitos verificadores.
- Não permite mais de um beneficiário com o mesmo CPF para o mesmo cliente.
- Os beneficiários são gravados somente após clicar em Salvar na tela do Cliente.

## Banco de dados
As alterações estão em scripts de migração em `FI.AtividadeEntrevista/DAL/Scripts`:
`001_CLIENTES_CPF.sql` 
Coluna `CPF` na tabela `CLIENTES`; `ALTER PROC` das procedures de inclusão, alteração, consulta, pesquisa e verificação de clientes; criação da `FI_SP_DelCliente`, chamada pelo código mas inexistente no banco original.
`002_BENEFICIARIOS.sql` 
Procedures `FI_SP_IncBenef`, `FI_SP_ConsBenef` e `FI_SP_DelBenef`. A tabela `BENEFICIARIOS` (`ID`, `CPF`, `NOME`, `IDCLIENTE`) e a `FI_SP_AltBenef` já existiam no banco original e foram mantidas.

Os scripts já estão aplicados no `.mdf` versionado. Para aplicá-los em uma cópia do banco original, execute-os na ordem (001 e depois 002). Os scripts originais de criação das procedures, em `DAL/Clientes/Procedures`, foram mantidos sem alteração.

## Testes unitários
Os testes não acessam o banco: os DAOs são substituídos por implementações em memória (`Fakes`).
- `CpfHelperTests`: validação, remoção de máscara e formatação de CPF.
- `CpfAttributeTests`: atributo de validação `[Cpf]`.
- `BoClienteTests`: CPF inválido, CPF duplicado, alteração mantendo o próprio CPF e gravação sem máscara.
- `BoBeneficiarioTests`: validações e sincronização da lista com o banco (inclusão, alteração e exclusão).
