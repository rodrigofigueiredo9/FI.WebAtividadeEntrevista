using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;

namespace FI.AtividadeEntrevista.DAL.Interfaces
{
    /// <summary>
    /// Contrato de acesso a dados de Cliente.
    /// Permite substituir o acesso ao banco por implementações falsas nos testes unitários.
    /// </summary>
    public interface IDaoCliente
    {
        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        /// <returns>Id do cliente incluído</returns>
        long Incluir(Cliente cliente);

        /// <summary>
        /// Altera um cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        void Alterar(Cliente cliente);

        /// <summary>
        /// Consulta o cliente pelo id
        /// </summary>
        /// <param name="id">Id do cliente</param>
        Cliente Consultar(long id);

        /// <summary>
        /// Exclui o cliente pelo id
        /// </summary>
        /// <param name="id">Id do cliente</param>
        void Excluir(long id);

        /// <summary>
        /// Lista todos os clientes
        /// </summary>
        List<Cliente> Listar();

        /// <summary>
        /// Pesquisa paginada de clientes
        /// </summary>
        List<Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd);

        /// <summary>
        /// Verifica se já existe um cliente com o CPF informado
        /// </summary>
        /// <param name="cpf">CPF somente com dígitos</param>
        /// <param name="idIgnorado">Id do cliente a ser desconsiderado na verificação (0 na inclusão)</param>
        /// <returns>true quando o CPF já estiver cadastrado para outro cliente</returns>
        bool VerificarExistencia(string cpf, long idIgnorado);
    }
}
