using FI.AtividadeEntrevista.Validacoes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FI.AtividadeEntrevista.Tests.Validacoes
{
    [TestClass]
    public class CpfAttributeTests
    {
        private readonly CpfAttribute _atributo = new CpfAttribute();

        [DataTestMethod]
        [DataRow("529.982.247-25")]
        [DataRow("52998224725")]
        public void IsValid_CpfValido_DeveRetornarTrue(string cpf)
        {
            Assert.IsTrue(_atributo.IsValid(cpf));
        }

        [DataTestMethod]
        [DataRow("529.982.247-26")]
        [DataRow("111.111.111-11")]
        [DataRow("abc")]
        public void IsValid_CpfInvalido_DeveRetornarFalse(string cpf)
        {
            Assert.IsFalse(_atributo.IsValid(cpf));
        }

        [DataTestMethod]
        [DataRow((string)null, DisplayName = "Nulo")]
        [DataRow("", DisplayName = "Vazio")]
        [DataRow("   ", DisplayName = "Somente espaços")]
        public void IsValid_NuloOuVazio_DeveRetornarTrue_PoisObrigatoriedadeEhDoRequired(string cpf)
        {
            Assert.IsTrue(_atributo.IsValid(cpf));
        }

        [TestMethod]
        public void IsValid_ValorQueNaoEhTexto_DeveRetornarFalse()
        {
            Assert.IsFalse(_atributo.IsValid(52998224725));
        }

        [TestMethod]
        public void Validacao_ModeloComCpfInvalido_DeveRetornarMensagemPadrao()
        {
            var modelo = new ModeloDeTeste { CPF = "529.982.247-26" };
            var resultados = new List<ValidationResult>();

            bool valido = Validator.TryValidateObject(modelo, new ValidationContext(modelo), resultados, validateAllProperties: true);

            Assert.IsFalse(valido);
            Assert.AreEqual(1, resultados.Count);
            Assert.AreEqual("Digite um CPF válido.", resultados[0].ErrorMessage);
        }

        [TestMethod]
        public void Validacao_ModeloComCpfValido_DeveSerValido()
        {
            var modelo = new ModeloDeTeste { CPF = "529.982.247-25" };
            var resultados = new List<ValidationResult>();

            bool valido = Validator.TryValidateObject(modelo, new ValidationContext(modelo), resultados, validateAllProperties: true);

            Assert.IsTrue(valido);
        }

        private class ModeloDeTeste
        {
            [Cpf]
            public string CPF { get; set; }
        }
    }
}
