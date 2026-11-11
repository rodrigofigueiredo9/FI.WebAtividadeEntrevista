using FI.AtividadeEntrevista.DAL.Interfaces;
using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;
using System.Linq;

namespace FI.AtividadeEntrevista.Tests.Fakes
{
    /// <summary>
    /// Implementação em memória de IDaoBeneficiario para os testes unitários (sem acesso ao banco).
    /// Registra as operações executadas para permitir verificações nos testes.
    /// </summary>
    public class FakeDaoBeneficiario : IDaoBeneficiario
    {
        private long _proximoId = 1;

        public List<Beneficiario> Beneficiarios { get; } = new List<Beneficiario>();

        public List<Beneficiario> Incluidos { get; } = new List<Beneficiario>();
        public List<Beneficiario> Alterados { get; } = new List<Beneficiario>();
        public List<long> Excluidos { get; } = new List<long>();

        public long Incluir(Beneficiario beneficiario)
        {
            long id = _proximoId++;
            Beneficiarios.Add(new Beneficiario { Id = id, CPF = beneficiario.CPF, Nome = beneficiario.Nome, IdCliente = beneficiario.IdCliente });
            Incluidos.Add(beneficiario);
            return id;
        }

        public void Alterar(Beneficiario beneficiario)
        {
            Beneficiario gravado = Beneficiarios.First(b => b.Id == beneficiario.Id);
            gravado.CPF = beneficiario.CPF;
            gravado.Nome = beneficiario.Nome;
            Alterados.Add(beneficiario);
        }

        public void Excluir(long id)
        {
            Beneficiarios.RemoveAll(b => b.Id == id);
            Excluidos.Add(id);
        }

        public List<Beneficiario> Listar(long idCliente)
        {
            return Beneficiarios
                .Where(b => b.IdCliente == idCliente)
                .Select(b => new Beneficiario { Id = b.Id, CPF = b.CPF, Nome = b.Nome, IdCliente = b.IdCliente })
                .ToList();
        }

        /// <summary>
        /// Adiciona um beneficiário já existente na "base" em memória
        /// </summary>
        public void AdicionarExistente(long id, long idCliente, string cpf, string nome)
        {
            Beneficiarios.Add(new Beneficiario { Id = id, IdCliente = idCliente, CPF = cpf, Nome = nome });
            if (id >= _proximoId)
                _proximoId = id + 1;
        }
    }
}
