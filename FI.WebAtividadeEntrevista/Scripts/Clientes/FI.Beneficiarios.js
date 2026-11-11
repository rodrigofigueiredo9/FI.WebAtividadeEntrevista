
var FI = window.FI || {};

FI.Beneficiarios = {
    lista: [],
    indiceEmEdicao: null,

    // Carrega a lista inicial (vazia na inclusão; vinda do servidor na alteração)
    carregar: function (beneficiarios) {
        this.lista = beneficiarios || [];
        this.limparCampos();
        this.renderizar();
    },

    // Inclui um novo beneficiário ou confirma a alteração do beneficiário em edição
    incluir: function () {
        var cpf = $('#BeneficiarioCPF').val();
        var nome = $.trim($('#BeneficiarioNome').val());

        if (!FI.Cpf.validar(cpf))
            return this.exibirMensagem('Digite um CPF válido.');

        if (!nome)
            return this.exibirMensagem('Digite o nome do beneficiário.');

        if (this.cpfJaIncluido(cpf))
            return this.exibirMensagem('Já existe um beneficiário com este CPF.');

        if (this.indiceEmEdicao === null) {
            this.lista.push({ Id: 0, CPF: cpf, Nome: nome });
        } else {
            this.lista[this.indiceEmEdicao].CPF = cpf;
            this.lista[this.indiceEmEdicao].Nome = nome;
        }

        this.limparCampos();
        this.renderizar();
    },

    // Carrega o beneficiário nos campos; a confirmação é feita pelo botão "Incluir"
    alterar: function (indice) {
        this.indiceEmEdicao = indice;
        $('#BeneficiarioCPF').val(this.lista[indice].CPF);
        $('#BeneficiarioNome').val(this.lista[indice].Nome);
        this.exibirMensagem('');
    },

    excluir: function (indice) {
        this.lista.splice(indice, 1);
        this.limparCampos();
        this.renderizar();
    },

    // Verifica se o CPF já está na lista (desconsiderando o beneficiário em edição)
    cpfJaIncluido: function (cpf) {
        var digitos = FI.Cpf.removerMascara(cpf);

        for (var i = 0; i < this.lista.length; i++) {
            if (i !== this.indiceEmEdicao && FI.Cpf.removerMascara(this.lista[i].CPF) === digitos)
                return true;
        }

        return false;
    },

    limparCampos: function () {
        this.indiceEmEdicao = null;
        $('#BeneficiarioCPF').val('');
        $('#BeneficiarioNome').val('');
        this.exibirMensagem('');
    },

    exibirMensagem: function (mensagem) {
        $('#mensagemBeneficiario').text(mensagem).toggle(!!mensagem);
    },

    renderizar: function () {
        var corpo = $('#gridBeneficiarios').empty();

        $.each(this.lista, function (indice, beneficiario) {
            $('<tr>')
                .append($('<td>').text(beneficiario.CPF))
                .append($('<td>').text(beneficiario.Nome))
                .append($('<td>')
                    .append($('<button type="button" class="btn btn-sm btn-primary">Alterar</button>')
                        .click(function () { FI.Beneficiarios.alterar(indice); }))
                    .append(' ')
                    .append($('<button type="button" class="btn btn-sm btn-primary">Excluir</button>')
                        .click(function () { FI.Beneficiarios.excluir(indice); })))
                .appendTo(corpo);
        });
    }
};

$(document).ready(function () {
    $('#btnIncluirBeneficiario').click(function () {
        FI.Beneficiarios.incluir();
    });

    // Ao fechar o pop-up, descarta o que estava sendo digitado (a lista é mantida)
    $('#modalBeneficiarios').on('hidden.bs.modal', function () {
        FI.Beneficiarios.limparCampos();
    });
});
