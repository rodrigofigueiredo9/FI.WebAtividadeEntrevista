using FI.AtividadeEntrevista.BLL;
using System;
using System.ComponentModel.DataAnnotations;

namespace FI.AtividadeEntrevista.Validacoes
{
    /// <summary>
    /// Valida se o valor informado é um CPF válido (formato e dígitos verificadores).
    /// Valores nulos ou vazios são considerados válidos: a obrigatoriedade deve ser tratada com [Required].
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class CpfAttribute : ValidationAttribute
    {
        public CpfAttribute() : base("Digite um CPF válido.")
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            string cpf = value as string;

            if (cpf == null)
                return false;

            if (string.IsNullOrWhiteSpace(cpf))
                return true;

            return CpfHelper.Validar(cpf);
        }
    }
}
