using FI.AtividadeEntrevista.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FI.AtividadeEntrevista.Tests
{
    /// <summary>
    /// Testes de verificação da infraestrutura do projeto de testes.
    /// Garantem que o MSTest está configurado e que o projeto de negócio é referenciado corretamente.
    /// </summary>
    [TestClass]
    public class ProjetoDeTestesTests
    {
        [TestMethod]
        public void ProjetoDeTestes_DeveExecutarTestes()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ProjetoDeTestes_DeveReferenciarProjetoDeNegocio()
        {
            var assembly = typeof(BoCliente).Assembly;

            Assert.AreEqual("FI.AtividadeEntrevista", assembly.GetName().Name);
        }
    }
}
