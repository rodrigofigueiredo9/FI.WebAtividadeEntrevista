
$(document).ready(function () {

    if (document.getElementById("gridClientes"))
        $('#gridClientes').jtable({
            title: 'Clientes',
            paging: true, //Enable paging
            pageSize: 5, //Set page size (default: 10)
            sorting: true, //Enable sorting
            defaultSorting: 'Nome ASC', //Set default sorting
            actions: {
                listAction: urlClienteList,
            },
            fields: {
                Nome: {
                    title: 'Nome',
                    width: '50%'
                },
                Email: {
                    title: 'Email',
                    width: '35%'
                },
                Acoes: {
                    title: '',
                    sorting: false,
                    display: function (data) {
                        var $alterar = $('<button class="btn btn-primary btn-sm">Alterar</button>')
                            .click(function () {
                                window.location.href = urlAlteracao + '/' + data.record.Id;
                            });

                        var $excluir = $('<button class="btn btn-danger btn-sm">Excluir</button>')
                            .click(function () {
                                excluirCliente(data.record);
                            });

                        return $('<span></span>').append($alterar, ' ', $excluir);
                    }
                }
            }
        });

    //Load student list from server
    if (document.getElementById("gridClientes"))
        $('#gridClientes').jtable('load');
})

function excluirCliente(cliente) {
    if (!window.confirm('Deseja realmente excluir o cliente "' + cliente.Nome + '"? Os beneficiários também serão excluídos.'))
        return;

    $.ajax({
        url: urlExclusao,
        method: "POST",
        data: { id: cliente.Id },
        error:
        function (r) {
            if (r.status == 400)
                ModalDialog("Ocorreu um erro", r.responseJSON);
            else
                ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
        },
        success:
        function (r) {
            ModalDialog("Sucesso!", r);
            $('#gridClientes').jtable('reload');
        }
    });
}
