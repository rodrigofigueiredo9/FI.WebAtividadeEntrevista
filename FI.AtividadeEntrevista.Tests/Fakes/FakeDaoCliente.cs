using FI.AtividadeEntrevista.DAL.Interfaces;
using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;
using System.Linq;

namespace FI.AtividadeEntrevista.Tests.Fakes
{
    /// <summary>
    /// Implementação em memória de IDaoCliente para os testes unitários (sem acesso ao banco).
    /// Registra as chamadas recebidas para permitir verificações nos testes.
    /// </summary>
    public class FakeDaoCliente : IDaoCliente
    {
        private long _proximoId = 1;

        public List<Cliente> Clientes { get; } = new List<Cliente>();

        public Cliente UltimoClienteIncluido { get; private set; }
        public Cliente UltimoClienteAlterado { get; private set; }
        public string UltimoCpfVerificado { get; private set; }
        public long? UltimoIdIgnorado { get; private set; }

        public long Incluir(Cliente cliente)
        {
            cliente.Id = _proximoId++;
            Clientes.Add(cliente);
            UltimoClienteIncluido = cliente;
            return cliente.Id;
        }

        public void Alterar(Cliente cliente)
        {
            Clientes.RemoveAll(c => c.Id == cliente.Id);
            Clientes.Add(cliente);
            UltimoClienteAlterado = cliente;
        }

        public Cliente Consultar(long id)
        {
            return Clientes.FirstOrDefault(c => c.Id == id);
        }

        public void Excluir(long id)
        {
            Clientes.RemoveAll(c => c.Id == id);
        }

        public List<Cliente> Listar()
        {
            return Clientes.ToList();
        }

        public List<Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            qtd = Clientes.Count;
            return Clientes.Skip(iniciarEm).Take(quantidade).ToList();
        }

        public bool VerificarExistencia(string cpf, long idIgnorado)
        {
            UltimoCpfVerificado = cpf;
            UltimoIdIgnorado = idIgnorado;
            return Clientes.Any(c => c.CPF == cpf && c.Id != idIgnorado);
        }

        // Adiciona um cliente já existente na "base" em memória
        public Cliente AdicionarExistente(long id, string cpf)
        {
            var cliente = new Cliente { Id = id, CPF = cpf, Nome = "Existente " + id };
            Clientes.Add(cliente);
            if (id >= _proximoId)
                _proximoId = id + 1;
            return cliente;
        }
    }
}
