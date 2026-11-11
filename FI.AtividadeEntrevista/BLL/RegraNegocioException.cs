using System;

namespace FI.AtividadeEntrevista.BLL
{
    /// <summary>
    /// Exceção lançada quando uma regra de negócio é violada.
    /// A mensagem é destinada ao usuário final e pode ser exibida diretamente na tela.
    /// </summary>
    [Serializable]
    public class RegraNegocioException : Exception
    {
        public RegraNegocioException(string mensagem) : base(mensagem)
        {
        }
    }
}
