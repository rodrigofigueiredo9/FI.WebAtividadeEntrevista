using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebAtividadeEntrevista.Models;
using WebAtividadeEntrevista.Services;
using WebAtividadeEntrevista.Services.Interfaces;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClienteController() : this(new ClienteService())
        {
        }

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            if (!this.ModelState.IsValid)
                return ErrosDeValidacao();

            try
            {
                _clienteService.Incluir(model);
            }
            catch (RegraNegocioException ex)
            {
                return Erro(ex.Message);
            }

            return Json("Cadastro efetuado com sucesso");
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            if (!this.ModelState.IsValid)
                return ErrosDeValidacao();

            try
            {
                _clienteService.Alterar(model);
            }
            catch (RegraNegocioException ex)
            {
                return Erro(ex.Message);
            }

            return Json("Cadastro alterado com sucesso");
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            ClienteModel model = _clienteService.Consultar(id);

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = (jtSorting ?? string.Empty).Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = _clienteService.Pesquisar(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private JsonResult ErrosDeValidacao()
        {
            List<string> erros = (from item in ModelState.Values
                                  from error in item.Errors
                                  select error.ErrorMessage).ToList();

            return Erro(string.Join(Environment.NewLine, erros));
        }

        private JsonResult Erro(string mensagem)
        {
            Response.StatusCode = 400;
            return Json(mensagem);
        }
    }
}
