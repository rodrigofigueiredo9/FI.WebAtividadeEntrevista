using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using FI.AtividadeEntrevista.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FI.AtividadeEntrevista.Tests.BLL
{
    /// <summary>
    /// Testes das regras de negócio de Cliente relacionadas ao CPF.
    /// Utiliza FakeDaoCliente para isolar o BO do banco de dados.
    /// </summary>
    [TestClass]
    public class BoClienteTests
    {
        private const string CpfValidoComMascara = "529.982.247-25";
        private const string CpfValidoSemMascara = "52998224725";
        private const string OutroCpfValido = "11144477735";

        private FakeDaoCliente _dao;
        private BoCliente _bo;

        [TestInitialize]
        public void Inicializar()
        {
            _dao = new FakeDaoCliente();
            _bo = new BoCliente(_dao);
        }

        private static Cliente NovoCliente(string cpf, long id = 0)
        {
            return new Cliente
            {
                Id = id,
                Nome = "João",
                Sobrenome = "da Silva",
                Nacionalidade = "brasileira",
                CEP = "01011-100",
                Estado = "SP",
                Cidade = "São Paulo",
                Logradouro = "Rua Boa Vista 253",
                Email = "email@email.com",
                Telefone = "(11) 2020-3030",
                CPF = cpf
            };
        }

        #region Incluir

        [TestMethod]
        public void Incluir_CpfValidoComMascara_DeveGravarSomenteDigitos()
        {
            long id = _bo.Incluir(NovoCliente(CpfValidoComMascara));

            Assert.AreEqual(1, id);
            Assert.IsNotNull(_dao.UltimoClienteIncluido);
            Assert.AreEqual(CpfValidoSemMascara, _dao.UltimoClienteIncluido.CPF);
        }

        [TestMethod]
        public void Incluir_DeveVerificarDuplicidadeSemIgnorarNenhumCliente()
        {
            _bo.Incluir(NovoCliente(CpfValidoComMascara));

            Assert.AreEqual(CpfValidoSemMascara, _dao.UltimoCpfVerificado);
            Assert.AreEqual(0, _dao.UltimoIdIgnorado);
        }

        [DataTestMethod]
        [DataRow("529.982.247-26", DisplayName = "Dígito verificador incorreto")]
        [DataRow("111.111.111-11", DisplayName = "Dígitos repetidos")]
        [DataRow("123", DisplayName = "Formato inválido")]
        [DataRow("", DisplayName = "Vazio")]
        [DataRow((string)null, DisplayName = "Nulo")]
        public void Incluir_CpfInvalido_DeveLancarRegraNegocioException(string cpf)
        {
            var ex = Assert.ThrowsException<RegraNegocioException>(() => _bo.Incluir(NovoCliente(cpf)));

            Assert.AreEqual(BoCliente.MensagemCpfInvalido, ex.Message);
            Assert.IsNull(_dao.UltimoClienteIncluido, "O cliente não deveria ser gravado.");
        }

        [TestMethod]
        public void Incluir_CpfJaCadastrado_DeveLancarRegraNegocioException()
        {
            _dao.AdicionarExistente(10, CpfValidoSemMascara);

            var ex = Assert.ThrowsException<RegraNegocioException>(() => _bo.Incluir(NovoCliente(CpfValidoComMascara)));

            Assert.AreEqual(BoCliente.MensagemCpfDuplicado, ex.Message);
            Assert.IsNull(_dao.UltimoClienteIncluido, "O cliente não deveria ser gravado.");
        }

        [TestMethod]
        public void Incluir_ClienteNulo_DeveLancarArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _bo.Incluir(null));
        }

        #endregion

        #region Alterar

        [TestMethod]
        public void Alterar_MantendoOProprioCpf_DevePermitir()
        {
            _dao.AdicionarExistente(5, CpfValidoSemMascara);

            _bo.Alterar(NovoCliente(CpfValidoComMascara, id: 5));

            Assert.IsNotNull(_dao.UltimoClienteAlterado);
            Assert.AreEqual(CpfValidoSemMascara, _dao.UltimoClienteAlterado.CPF);
            Assert.AreEqual(5, _dao.UltimoIdIgnorado, "A verificação deve desconsiderar o próprio cliente.");
        }

        [TestMethod]
        public void Alterar_ParaCpfDeOutroCliente_DeveLancarRegraNegocioException()
        {
            _dao.AdicionarExistente(5, CpfValidoSemMascara);
            _dao.AdicionarExistente(6, OutroCpfValido);

            var ex = Assert.ThrowsException<RegraNegocioException>(() => _bo.Alterar(NovoCliente(OutroCpfValido, id: 5)));

            Assert.AreEqual(BoCliente.MensagemCpfDuplicado, ex.Message);
            Assert.IsNull(_dao.UltimoClienteAlterado, "O cliente não deveria ser alterado.");
        }

        [TestMethod]
        public void Alterar_CpfInvalido_DeveLancarRegraNegocioException()
        {
            _dao.AdicionarExistente(5, CpfValidoSemMascara);

            var ex = Assert.ThrowsException<RegraNegocioException>(() => _bo.Alterar(NovoCliente("529.982.247-26", id: 5)));

            Assert.AreEqual(BoCliente.MensagemCpfInvalido, ex.Message);
            Assert.IsNull(_dao.UltimoClienteAlterado);
        }

        [TestMethod]
        public void Alterar_ClienteNulo_DeveLancarArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _bo.Alterar(null));
        }

        #endregion

        #region VerificarExistencia

        [TestMethod]
        public void VerificarExistencia_CpfComMascara_DeveConsultarSomenteDigitos()
        {
            _dao.AdicionarExistente(1, CpfValidoSemMascara);

            bool existe = _bo.VerificarExistencia(CpfValidoComMascara);

            Assert.IsTrue(existe);
            Assert.AreEqual(CpfValidoSemMascara, _dao.UltimoCpfVerificado);
        }

        [TestMethod]
        public void VerificarExistencia_CpfNaoCadastrado_DeveRetornarFalse()
        {
            Assert.IsFalse(_bo.VerificarExistencia(CpfValidoComMascara));
        }

        #endregion
    }
}
