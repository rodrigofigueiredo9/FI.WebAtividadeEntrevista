using FI.AtividadeEntrevista.DAL.Interfaces;
using System;
using System.Collections.Generic;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoCliente
    {
        public const string MensagemCpfInvalido = "Digite um CPF válido.";
        public const string MensagemCpfDuplicado = "Já existe um cliente cadastrado com este CPF.";

        private readonly IDaoCliente _daoCliente;

        public BoCliente(IDaoCliente daoCliente)
        {
            _daoCliente = daoCliente;
        }

        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        /// <exception cref="RegraNegocioException">CPF inválido ou já cadastrado</exception>
        public long Incluir(DML.Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            ValidarCpf(cliente, 0);

            return _daoCliente.Incluir(cliente);
        }

        /// <summary>
        /// Altera um cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        /// <exception cref="RegraNegocioException">CPF inválido ou já cadastrado para outro cliente</exception>
        public void Alterar(DML.Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            ValidarCpf(cliente, cliente.Id);

            _daoCliente.Alterar(cliente);
        }

        /// <summary>
        /// Consulta o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public DML.Cliente Consultar(long id)
        {
            return _daoCliente.Consultar(id);
        }

        /// <summary>
        /// Excluir o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public void Excluir(long id)
        {
            _daoCliente.Excluir(id);
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Listar()
        {
            return _daoCliente.Listar();
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            return _daoCliente.Pesquisa(iniciarEm, quantidade, campoOrdenacao, crescente, out qtd);
        }

        /// <summary>
        /// Verifica se já existe um cliente cadastrado com o CPF informado
        /// </summary>
        /// <param name="CPF">CPF com ou sem máscara</param>
        /// <param name="idIgnorado">Id do cliente a ser desconsiderado (utilizado na alteração)</param>
        /// <returns>true quando o CPF já estiver cadastrado para outro cliente</returns>
        public bool VerificarExistencia(string CPF, long idIgnorado = 0)
        {
            return _daoCliente.VerificarExistencia(CpfHelper.RemoverMascara(CPF), idIgnorado);
        }

        /// <summary>
        /// Valida o CPF do cliente (formato, dígitos verificadores e duplicidade)
        /// e o normaliza para o formato armazenado no banco (somente dígitos).
        /// </summary>
        private void ValidarCpf(DML.Cliente cliente, long idIgnorado)
        {
            if (!CpfHelper.Validar(cliente.CPF))
                throw new RegraNegocioException(MensagemCpfInvalido);

            cliente.CPF = CpfHelper.RemoverMascara(cliente.CPF);

            if (_daoCliente.VerificarExistencia(cliente.CPF, idIgnorado))
                throw new RegraNegocioException(MensagemCpfDuplicado);
        }
    }
}
