using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAtividadeEntrevista.Models;
using WebAtividadeEntrevista.Services.Interfaces;

namespace WebAtividadeEntrevista.Services
{
    public class ClienteService : IClienteService
    {
        private readonly BoCliente _boCliente;
        private readonly BoBeneficiario _boBeneficiario;

        public ClienteService() : this(new BoCliente(), new BoBeneficiario())
        {
        }

        public ClienteService(BoCliente boCliente, BoBeneficiario boBeneficiario)
        {
            _boCliente = boCliente;
            _boBeneficiario = boBeneficiario;
        }

        public long Incluir(ClienteModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            List<Beneficiario> beneficiarios = ParaBeneficiarios(model);
            _boBeneficiario.Validar(beneficiarios);

            model.Id = _boCliente.Incluir(ParaCliente(model));

            _boBeneficiario.Salvar(model.Id, beneficiarios);

            return model.Id;
        }

        public void Alterar(ClienteModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            List<Beneficiario> beneficiarios = ParaBeneficiarios(model);

            _boBeneficiario.Validar(beneficiarios);

            _boCliente.Alterar(ParaCliente(model));

            _boBeneficiario.Salvar(model.Id, beneficiarios);
        }

        public ClienteModel Consultar(long id)
        {
            Cliente cliente = _boCliente.Consultar(id);

            if (cliente == null)
                return null;

            ClienteModel model = ParaModel(cliente);

            model.Beneficiarios = _boBeneficiario.Listar(id)
                .Select(b => new BeneficiarioModel
                {
                    Id = b.Id,
                    CPF = CpfHelper.Formatar(b.CPF),
                    Nome = b.Nome
                })
                .ToList();

            return model;
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

        private static List<Beneficiario> ParaBeneficiarios(ClienteModel model)
        {
            return (model.Beneficiarios ?? new List<BeneficiarioModel>())
                .Select(b => new Beneficiario
                {
                    Id = b.Id,
                    CPF = b.CPF,
                    Nome = b.Nome
                })
                .ToList();
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
