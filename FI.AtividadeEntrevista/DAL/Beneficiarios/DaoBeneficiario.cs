using FI.AtividadeEntrevista.DAL.Interfaces;
using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace FI.AtividadeEntrevista.DAL
{
    /// <summary>
    /// Classe de acesso a dados de Beneficiário
    /// </summary>
    internal class DaoBeneficiario : AcessoDados, IDaoBeneficiario
    {
        public long Incluir(Beneficiario beneficiario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter("NOME", beneficiario.Nome));
            parametros.Add(new SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new SqlParameter("IDCLIENTE", beneficiario.IdCliente));

            DataSet ds = base.Consultar("FI_SP_IncBenef", parametros);
            long ret = 0;
            if (ds.Tables[0].Rows.Count > 0)
                long.TryParse(ds.Tables[0].Rows[0][0].ToString(), out ret);
            return ret;
        }

        public void Alterar(Beneficiario beneficiario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter("NOME", beneficiario.Nome));
            parametros.Add(new SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new SqlParameter("ID", beneficiario.Id));

            base.Executar("FI_SP_AltBenef", parametros);
        }

        public void Excluir(long id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter("ID", id));

            base.Executar("FI_SP_DelBenef", parametros);
        }

        public List<Beneficiario> Listar(long idCliente)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter("IDCLIENTE", idCliente));

            DataSet ds = base.Consultar("FI_SP_ConsBenef", parametros);

            List<Beneficiario> lista = new List<Beneficiario>();
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    lista.Add(new Beneficiario
                    {
                        Id = row.Field<long>("ID"),
                        CPF = row.Field<string>("CPF"),
                        Nome = row.Field<string>("NOME"),
                        IdCliente = row.Field<long>("IDCLIENTE")
                    });
                }
            }

            return lista;
        }
    }
}
