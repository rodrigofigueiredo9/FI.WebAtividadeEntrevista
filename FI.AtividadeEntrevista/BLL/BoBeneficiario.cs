using FI.AtividadeEntrevista.DAL.Interfaces;
using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;
using System.Linq;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBeneficiario
    {
        public const string MensagemNomeObrigatorio = "O nome do beneficiário é obrigatório.";
        public const string MensagemCpfInvalido = "Digite um CPF válido para o beneficiário.";
        public const string MensagemCpfDuplicado = "Não é permitido mais de um beneficiário com o mesmo CPF.";

        private readonly IDaoBeneficiario _daoBeneficiario;

        public BoBeneficiario() : this(new DAL.DaoBeneficiario())
        {
        }

        public BoBeneficiario(IDaoBeneficiario daoBeneficiario)
        {
            _daoBeneficiario = daoBeneficiario;
        }

        /// <summary>
        /// Lista os beneficiários de um cliente
        /// </summary>
        public List<Beneficiario> Listar(long idCliente)
        {
            return _daoBeneficiario.Listar(idCliente);
        }

        /// <summary>
        /// Valida os beneficiários de um cliente: nome obrigatório, CPF válido e sem CPF repetido.
        /// </summary>
        /// <exception cref="RegraNegocioException">Quando alguma regra for violada</exception>
        public void Validar(List<Beneficiario> beneficiarios)
        {
            if (beneficiarios == null)
                return;

            HashSet<string> cpfs = new HashSet<string>();

            foreach (Beneficiario beneficiario in beneficiarios)
            {
                if (string.IsNullOrWhiteSpace(beneficiario.Nome))
                    throw new RegraNegocioException(MensagemNomeObrigatorio);

                if (!CpfHelper.Validar(beneficiario.CPF))
                    throw new RegraNegocioException(MensagemCpfInvalido);

                beneficiario.Nome = beneficiario.Nome.Trim();
                beneficiario.CPF = CpfHelper.RemoverMascara(beneficiario.CPF);

                if (!cpfs.Add(beneficiario.CPF))
                    throw new RegraNegocioException(MensagemCpfDuplicado);
            }
        }

        /// <summary>
        /// Grava os beneficiários do cliente conforme a lista da tela:
        /// </summary>
        /// <exception cref="RegraNegocioException">Quando alguma regra for violada</exception>
        public void Salvar(long idCliente, List<Beneficiario> beneficiarios)
        {
            beneficiarios = beneficiarios ?? new List<Beneficiario>();

            Validar(beneficiarios);

            List<long> idsGravados = _daoBeneficiario.Listar(idCliente).Select(b => b.Id).ToList();
            List<long> idsDaTela = beneficiarios.Select(b => b.Id).ToList();

            foreach (long id in idsGravados.Where(id => !idsDaTela.Contains(id)))
                _daoBeneficiario.Excluir(id);

            foreach (Beneficiario beneficiario in beneficiarios)
            {
                beneficiario.IdCliente = idCliente;

                if (idsGravados.Contains(beneficiario.Id))
                    _daoBeneficiario.Alterar(beneficiario);
                else
                    beneficiario.Id = _daoBeneficiario.Incluir(beneficiario);
            }
        }
    }
}
