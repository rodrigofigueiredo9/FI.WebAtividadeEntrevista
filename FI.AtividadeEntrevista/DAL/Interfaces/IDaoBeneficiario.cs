using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;

namespace FI.AtividadeEntrevista.DAL.Interfaces
{
    public interface IDaoBeneficiario
    {
        long Incluir(Beneficiario beneficiario);
        void Alterar(Beneficiario beneficiario);
        void Excluir(long id);
        List<Beneficiario> Listar(long idCliente);
    }
}
