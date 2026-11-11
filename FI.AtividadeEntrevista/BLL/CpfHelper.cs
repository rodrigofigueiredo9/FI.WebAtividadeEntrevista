using System.Linq;
using System.Text.RegularExpressions;

namespace FI.AtividadeEntrevista.BLL
{
    public static class CpfHelper
    {
        public const int QuantidadeDigitos = 11;

        // Formatos aceitos: somente dígitos (99999999999) ou com a máscara completa (999.999.999-99).
        private static readonly Regex FormatoAceito = new Regex(@"^(\d{11}|\d{3}\.\d{3}\.\d{3}-\d{2})$", RegexOptions.Compiled);

        public static string RemoverMascara(string cpf)
        {
            if (cpf == null)
                return string.Empty;

            return new string(cpf.Where(char.IsDigit).ToArray());
        }

        public static string Formatar(string cpf)
        {
            string digitos = RemoverMascara(cpf);

            if (digitos.Length != QuantidadeDigitos)
                return cpf;

            return string.Format("{0}.{1}.{2}-{3}",
                digitos.Substring(0, 3),
                digitos.Substring(3, 3),
                digitos.Substring(6, 3),
                digitos.Substring(9, 2));
        }

        public static bool Validar(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = cpf.Trim();

            if (!FormatoAceito.IsMatch(cpf))
                return false;

            int[] digitos = RemoverMascara(cpf).Select(c => c - '0').ToArray();

            // Sequências de dígitos iguais (ex.: 111.111.111-11) passam no cálculo, mas não são CPFs válidos
            if (digitos.All(d => d == digitos[0]))
                return false;

            return digitos[9] == CalcularDigitoVerificador(digitos, 9)
                && digitos[10] == CalcularDigitoVerificador(digitos, 10);
        }

        private static int CalcularDigitoVerificador(int[] digitos, int quantidade)
        {
            int soma = 0;
            int peso = quantidade + 1;

            for (int i = 0; i < quantidade; i++)
                soma += digitos[i] * (peso - i);

            int resto = soma % 11;

            return resto < 2 ? 0 : 11 - resto;
        }
    }
}
