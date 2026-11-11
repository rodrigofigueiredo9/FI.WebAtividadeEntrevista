using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using WebAtividadeEntrevista.Models;
using WebAtividadeEntrevista.Services.Interfaces;

namespace WebAtividadeEntrevista.Services
{
    public class ClienteService : IClienteService
    {
        private readonly BoCliente _boCliente;

        public ClienteService(BoCliente boCliente)
        {
            _boCliente = boCliente;
        }

        public long Incluir(ClienteModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.Id = _boCliente.Incluir(ParaCliente(model));

            return model.Id;
        }

        public void Alterar(ClienteModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _boCliente.Alterar(ParaCliente(model));
        }

        public ClienteModel Consultar(long id)
        {
            Cliente cliente = _boCliente.Consultar(id);

            return cliente == null ? null : ParaModel(cliente);
        }

        public List<Cliente> Pesquisar(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            return _boCliente.Pesquisa(iniciarEm, quantidade, campoOrdenacao, crescente, out qtd);
        }

        private static Cliente ParaCliente(ClienteModel model)
        {
            return new Cliente()
            {
                Id = model.Id,
                CEP = model.CEP,
                Cidade = model.Cidade,
                Email = model.Email,
                Estado = model.Estado,
                Logradouro = model.Logradouro,
                Nacionalidade = model.Nacionalidade,
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                Telefone = model.Telefone,
                CPF = model.CPF
            };
        }

        private static ClienteModel ParaModel(Cliente cliente)
        {
            return new ClienteModel()
            {
                Id = cliente.Id,
                CEP = cliente.CEP,
                Cidade = cliente.Cidade,
                Email = cliente.Email,
                Estado = cliente.Estado,
                Logradouro = cliente.Logradouro,
                Nacionalidade = cliente.Nacionalidade,
                Nome = cliente.Nome,
                Sobrenome = cliente.Sobrenome,
                Telefone = cliente.Telefone,
                CPF = CpfHelper.Formatar(cliente.CPF)
            };
        }
    }
}
