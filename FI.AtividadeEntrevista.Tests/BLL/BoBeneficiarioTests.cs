using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using FI.AtividadeEntrevista.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace FI.AtividadeEntrevista.Tests.BLL
{
    /// <summary>
    /// Testes das regras de negócio de Beneficiário.
    /// Utiliza FakeDaoBeneficiario para isolar o BO do banco de dados.
    /// </summary>
    [TestClass]
    public class BoBeneficiarioTests
    {
        private const long IdCliente = 10;
        private const long IdOutroCliente = 20;

        private FakeDaoBeneficiario _dao;
        private BoBeneficiario _bo;

        [TestInitialize]
        public void Inicializar()
        {
            _dao = new FakeDaoBeneficiario();
            _bo = new BoBeneficiario(_dao);
        }

        private static Beneficiario Novo(string cpf, string nome, long id = 0)
        {
            return new Beneficiario { Id = id, CPF = cpf, Nome = nome };
        }

        #region Validar

        [TestMethod]
        public void Validar_ListaValida_DeveNormalizarCpfENome()
        {
            var lista = new List<Beneficiario>
            {
                Novo("000.000.001-91", "  Beneficiário João  "),
                Novo("00000000272", "Beneficiário José")
            };

            _bo.Validar(lista);

            Assert.AreEqual("00000000191", lista[0].CPF);
            Assert.AreEqual("Beneficiário João", lista[0].Nome);
            Assert.AreEqual("00000000272", lista[1].CPF);
        }

        [TestMethod]
        public void Validar_ListaNulaOuVazia_NaoDeveLancarExcecao()
        {
            _bo.Validar(null);
            _bo.Validar(new List<Beneficiario>());
        }

        [DataTestMethod]
        [DataRow((string)null, DisplayName = "Nulo")]
        [DataRow("", DisplayName = "Vazio")]
        [DataRow("   ", DisplayName = "Somente espaços")]
        public void Validar_NomeNaoInformado_DeveLancarRegraNegocioException(string nome)
        {
            var lista = new List<Beneficiario> { Novo("000.000.001-91", nome) };

            var ex = Assert.ThrowsException<RegraNegocioException>(() => _bo.Validar(lista));

            Assert.AreEqual(BoBeneficiario.MensagemNomeObrigatorio, ex.Message);
        }

        [DataTestMethod]
        [DataRow("000.000.001-92", DisplayName = "Dígito verificador incorreto")]
        [DataRow("111.111.111-11", DisplayName = "Dígitos repetidos")]
        [DataRow("123", DisplayName = "Formato inválido")]
        [DataRow("", DisplayName = "Vazio")]
        [DataRow((string)null, DisplayName = "Nulo")]
        public void Validar_CpfInvalido_DeveLancarRegraNegocioException(string cpf)
        {
            var lista = new List<Beneficiario> { Novo(cpf, "Maria") };

            var ex = Assert.ThrowsException<RegraNegocioException>(() => _bo.Validar(lista));

            Assert.AreEqual(BoBeneficiario.MensagemCpfInvalido, ex.Message);
        }

        [DataTestMethod]
        [DataRow("000.000.001-91", "000.000.001-91", DisplayName = "Ambos com máscara")]
        [DataRow("000.000.001-91", "00000000191", DisplayName = "Com e sem máscara")]
        public void Validar_CpfRepetidoNaLista_DeveLancarRegraNegocioException(string cpf1, string cpf2)
        {
            var lista = new List<Beneficiario>
            {
                Novo(cpf1, "Beneficiário João"),
                Novo(cpf2, "Beneficiário José")
            };

            var ex = Assert.ThrowsException<RegraNegocioException>(() => _bo.Validar(lista));

            Assert.AreEqual(BoBeneficiario.MensagemCpfDuplicado, ex.Message);
        }

        #endregion

        #region Salvar

        [TestMethod]
        public void Salvar_NovosBeneficiarios_DeveIncluirComIdDoClienteECpfSemMascara()
        {
            var lista = new List<Beneficiario>
            {
                Novo("000.000.001-91", "Beneficiário João"),
                Novo("000.000.002-72", "Beneficiário José")
            };

            _bo.Salvar(IdCliente, lista);

            Assert.AreEqual(2, _dao.Incluidos.Count);
            Assert.IsTrue(_dao.Incluidos.All(b => b.IdCliente == IdCliente));
            Assert.AreEqual("00000000191", _dao.Incluidos[0].CPF);
            Assert.IsTrue(lista.All(b => b.Id > 0), "Os beneficiários incluídos devem receber o Id gerado.");
            Assert.AreEqual(0, _dao.Alterados.Count);
            Assert.AreEqual(0, _dao.Excluidos.Count);
        }

        [TestMethod]
        public void Salvar_BeneficiarioExistente_DeveAlterar()
        {
            _dao.AdicionarExistente(1, IdCliente, "00000000191", "Nome Antigo");

            _bo.Salvar(IdCliente, new List<Beneficiario> { Novo("000.000.001-91", "Nome Novo", id: 1) });

            Assert.AreEqual(1, _dao.Alterados.Count);
            Assert.AreEqual("Nome Novo", _dao.Beneficiarios.Single().Nome);
            Assert.AreEqual(0, _dao.Incluidos.Count);
            Assert.AreEqual(0, _dao.Excluidos.Count);
        }

        [TestMethod]
        public void Salvar_BeneficiarioRemovidoDaLista_DeveExcluir()
        {
            _dao.AdicionarExistente(1, IdCliente, "00000000191", "Beneficiário João");
            _dao.AdicionarExistente(2, IdCliente, "00000000272", "Beneficiário José");

            _bo.Salvar(IdCliente, new List<Beneficiario> { Novo("000.000.001-91", "Beneficiário João", id: 1) });

            CollectionAssert.AreEqual(new List<long> { 2 }, _dao.Excluidos);
            Assert.AreEqual(1, _dao.Listar(IdCliente).Count);
        }

        [TestMethod]
        public void Salvar_IncluindoAlterandoEExcluindo_DeveSincronizarComOBanco()
        {
            _dao.AdicionarExistente(1, IdCliente, "00000000191", "Beneficiário João");
            _dao.AdicionarExistente(2, IdCliente, "00000000272", "Beneficiário José");

            var lista = new List<Beneficiario>
            {
                Novo("000.000.001-91", "Beneficiário João Silva", id: 1),
                Novo("529.982.247-25", "Beneficiária Maria")
            };

            _bo.Salvar(IdCliente, lista);

            Assert.AreEqual(1, _dao.Alterados.Count);
            Assert.AreEqual(1, _dao.Incluidos.Count);
            CollectionAssert.AreEqual(new List<long> { 2 }, _dao.Excluidos);

            var gravados = _dao.Listar(IdCliente);
            Assert.AreEqual(2, gravados.Count);
            Assert.IsTrue(gravados.Any(b => b.Nome == "Beneficiário João Silva"));
            Assert.IsTrue(gravados.Any(b => b.CPF == "52998224725"));
        }

        [TestMethod]
        public void Salvar_ListaNula_DeveExcluirTodosOsBeneficiariosDoCliente()
        {
            _dao.AdicionarExistente(1, IdCliente, "00000000191", "Beneficiário João");
            _dao.AdicionarExistente(2, IdCliente, "00000000272", "Beneficiário José");

            _bo.Salvar(IdCliente, null);

            Assert.AreEqual(2, _dao.Excluidos.Count);
            Assert.AreEqual(0, _dao.Listar(IdCliente).Count);
        }

        [TestMethod]
        public void Salvar_ListaInvalida_NaoDeveGravarNada()
        {
            _dao.AdicionarExistente(1, IdCliente, "00000000191", "Beneficiário João");

            var lista = new List<Beneficiario>
            {
                Novo("000.000.002-72", "Beneficiário José"),
                Novo("000.000.002-72", "Beneficiário Repetido")
            };

            Assert.ThrowsException<RegraNegocioException>(() => _bo.Salvar(IdCliente, lista));

            Assert.AreEqual(0, _dao.Incluidos.Count);
            Assert.AreEqual(0, _dao.Alterados.Count);
            Assert.AreEqual(0, _dao.Excluidos.Count);
        }

        [TestMethod]
        public void Salvar_IdDeBeneficiarioDeOutroCliente_DeveIncluirComoNovoSemAlterarOOriginal()
        {
            _dao.AdicionarExistente(5, IdOutroCliente, "00000000191", "Beneficiário de outro cliente");

            _bo.Salvar(IdCliente, new List<Beneficiario> { Novo("000.000.002-72", "Tentativa", id: 5) });

            Assert.AreEqual(0, _dao.Alterados.Count);
            Assert.AreEqual(1, _dao.Incluidos.Count);
            Assert.AreEqual("Beneficiário de outro cliente", _dao.Listar(IdOutroCliente).Single().Nome);
        }

        [TestMethod]
        public void Salvar_MesmoCpfEmClientesDiferentes_DevePermitir()
        {
            _dao.AdicionarExistente(1, IdOutroCliente, "00000000191", "Beneficiário de outro cliente");

            _bo.Salvar(IdCliente, new List<Beneficiario> { Novo("000.000.001-91", "Beneficiário João") });

            Assert.AreEqual(1, _dao.Incluidos.Count);
            Assert.AreEqual(1, _dao.Listar(IdCliente).Count);
            Assert.AreEqual(1, _dao.Listar(IdOutroCliente).Count);
        }

        #endregion

        #region Listar

        [TestMethod]
        public void Listar_DeveRetornarSomenteOsBeneficiariosDoCliente()
        {
            _dao.AdicionarExistente(1, IdCliente, "00000000191", "Beneficiário João");
            _dao.AdicionarExistente(2, IdOutroCliente, "00000000272", "Beneficiário José");

            var lista = _bo.Listar(IdCliente);

            Assert.AreEqual(1, lista.Count);
            Assert.AreEqual("Beneficiário João", lista[0].Nome);
        }

        #endregion
    }
}
