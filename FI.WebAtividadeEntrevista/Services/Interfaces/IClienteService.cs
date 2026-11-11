using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;
using WebAtividadeEntrevista.Models;

namespace WebAtividadeEntrevista.Services.Interfaces
{
    public interface IClienteService
    {
        long Incluir(ClienteModel model);
        void Alterar(ClienteModel model);
        ClienteModel Consultar(long id);
        List<Cliente> Pesquisar(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd);
    }
}
