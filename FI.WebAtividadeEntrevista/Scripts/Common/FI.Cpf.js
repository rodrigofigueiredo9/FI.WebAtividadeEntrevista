/*
    FI.Cpf - máscara e validação de CPF.
    
    Uso:
      - Máscara: adicione o atributo data-mascara="cpf" ao input.
      - Validação: FI.Cpf.validar('529.982.247-25') => true
 */
var FI = window.FI || {};

FI.Cpf = {
    removerMascara: function (valor) {
        return (valor || '').replace(/\D/g, '');
    },

    // Aplica a máscara 999.999.999-99 conforme a digitação
    formatar: function (valor) {
        return this.removerMascara(valor).substring(0, 11)
            .replace(/^(\d{3})(\d)/, '$1.$2')
            .replace(/^(\d{3})\.(\d{3})(\d)/, '$1.$2.$3')
            .replace(/^(\d{3})\.(\d{3})\.(\d{3})(\d)/, '$1.$2.$3-$4');
    },

    // Mesmas regras do CpfHelper (servidor): formato, dígitos repetidos e dígitos verificadores
    validar: function (valor) {
        var cpf = (valor || '').trim();

        if (!/^(\d{11}|\d{3}\.\d{3}\.\d{3}-\d{2})$/.test(cpf))
            return false;

        var digitos = this.removerMascara(cpf);

        if (/^(\d)\1{10}$/.test(digitos))
            return false;

        return this.digitoVerificador(digitos, 9) === Number(digitos[9])
            && this.digitoVerificador(digitos, 10) === Number(digitos[10]);
    },

    digitoVerificador: function (digitos, quantidade) {
        var soma = 0;

        for (var i = 0; i < quantidade; i++)
            soma += Number(digitos[i]) * (quantidade + 1 - i);

        var resto = soma % 11;

        return resto < 2 ? 0 : 11 - resto;
    }
};

$(document).on('input', '[data-mascara="cpf"]', function () {
    this.value = FI.Cpf.formatar(this.value);
});
