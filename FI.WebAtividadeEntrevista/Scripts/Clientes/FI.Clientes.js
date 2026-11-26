
$(document).ready(function () {
    $('#formCadastro').submit(function (e) {
        e.preventDefault();

        if (!FI.Cpf.validar($(this).find("#CPF").val())) {
            ModalDialog("Ocorreu um erro", "Digite um CPF válido.");
            return;
        }

        $.ajax({
            url: urlPost,
            method: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({
                "NOME": $(this).find("#Nome").val(),
                "CEP": $(this).find("#CEP").val(),
                "Email": $(this).find("#Email").val(),
                "Sobrenome": $(this).find("#Sobrenome").val(),
                "Nacionalidade": $(this).find("#Nacionalidade").val(),
                "Estado": $(this).find("#Estado").val(),
                "Cidade": $(this).find("#Cidade").val(),
                "Logradouro": $(this).find("#Logradouro").val(),
                "Telefone": $(this).find("#Telefone").val(),
                "CPF": $(this).find("#CPF").val(),
                "Beneficiarios": FI.Beneficiarios.lista
            }),
            error:
            function (r) {
                if (r.status == 400)
                    ModalDialog("Ocorreu um erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
            },
            success:
            function (r) {
                ModalDialog("Sucesso!", r)
                $("#formCadastro")[0].reset();
                FI.Beneficiarios.carregar([]);
            }
        });
    })
    
})
