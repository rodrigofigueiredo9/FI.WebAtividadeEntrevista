using FI.AtividadeEntrevista.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FI.AtividadeEntrevista.Tests.BLL
{
    // Testes de validação e formatação de CPF.
    [TestClass]
    public class CpfHelperTests
    {
        #region Validar

        [DataTestMethod]
        [DataRow("529.982.247-25", DisplayName = "Com máscara")]
        [DataRow("52998224725", DisplayName = "Somente dígitos")]
        [DataRow("111.444.777-35", DisplayName = "Outro CPF válido")]
        [DataRow("000.000.001-91", DisplayName = "Iniciado com zeros (exemplo do layout)")]
        [DataRow("000.000.002-72", DisplayName = "Iniciado com zeros (exemplo do layout 2)")]
        [DataRow(" 529.982.247-25 ", DisplayName = "Com espaços nas extremidades")]
        public void Validar_CpfValido_DeveRetornarTrue(string cpf)
        {
            Assert.IsTrue(CpfHelper.Validar(cpf));
        }

        [DataTestMethod]
        [DataRow("529.982.247-26", DisplayName = "Segundo dígito verificador incorreto")]
        [DataRow("529.982.247-15", DisplayName = "Primeiro dígito verificador incorreto")]
        [DataRow("010.011.111-00", DisplayName = "Placeholder do layout (inválido)")]
        public void Validar_DigitoVerificadorIncorreto_DeveRetornarFalse(string cpf)
        {
            Assert.IsFalse(CpfHelper.Validar(cpf));
        }

        [DataTestMethod]
        [DataRow("000.000.000-00")]
        [DataRow("111.111.111-11")]
        [DataRow("99999999999")]
        public void Validar_TodosOsDigitosIguais_DeveRetornarFalse(string cpf)
        {
            Assert.IsFalse(CpfHelper.Validar(cpf));
        }

        [DataTestMethod]
        [DataRow("529.982.247", DisplayName = "Menos de 11 dígitos")]
        [DataRow("5299822472", DisplayName = "10 dígitos")]
        [DataRow("529982247250", DisplayName = "12 dígitos")]
        [DataRow("529.982.247-2a", DisplayName = "Contém letra")]
        [DataRow("abc", DisplayName = "Texto")]
        [DataRow("529-982-247.25", DisplayName = "Separadores trocados")]
        [DataRow("529.982.24725", DisplayName = "Máscara incompleta")]
        public void Validar_FormatoInvalido_DeveRetornarFalse(string cpf)
        {
            Assert.IsFalse(CpfHelper.Validar(cpf));
        }

        [DataTestMethod]
        [DataRow((string)null, DisplayName = "Nulo")]
        [DataRow("", DisplayName = "Vazio")]
        [DataRow("   ", DisplayName = "Somente espaços")]
        public void Validar_NuloOuVazio_DeveRetornarFalse(string cpf)
        {
            Assert.IsFalse(CpfHelper.Validar(cpf));
        }

        #endregion

        #region RemoverMascara

        [DataTestMethod]
        [DataRow("529.982.247-25", "52998224725")]
        [DataRow("52998224725", "52998224725")]
        [DataRow(" 529.982.247-25 ", "52998224725")]
        [DataRow("", "")]
        public void RemoverMascara_DeveManterSomenteDigitos(string cpf, string esperado)
        {
            Assert.AreEqual(esperado, CpfHelper.RemoverMascara(cpf));
        }

        [TestMethod]
        public void RemoverMascara_Nulo_DeveRetornarStringVazia()
        {
            Assert.AreEqual(string.Empty, CpfHelper.RemoverMascara(null));
        }

        #endregion

        #region Formatar

        [DataTestMethod]
        [DataRow("52998224725", "529.982.247-25")]
        [DataRow("529.982.247-25", "529.982.247-25")]
        [DataRow("00000000191", "000.000.001-91")]
        public void Formatar_CpfCom11Digitos_DeveAplicarMascara(string cpf, string esperado)
        {
            Assert.AreEqual(esperado, CpfHelper.Formatar(cpf));
        }

        [DataTestMethod]
        [DataRow("123", DisplayName = "Poucos dígitos")]
        [DataRow("", DisplayName = "Vazio")]
        [DataRow((string)null, DisplayName = "Nulo")]
        public void Formatar_SemOnzeDigitos_DeveRetornarValorOriginal(string cpf)
        {
            Assert.AreEqual(cpf, CpfHelper.Formatar(cpf));
        }

        #endregion
    }
}
