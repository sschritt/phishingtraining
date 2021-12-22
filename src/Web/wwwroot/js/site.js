// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
    var langToLoad = $('html').attr('lang');

    // https://datatables.net/reference/option/
    var dataTableOptions = {
        "paging": false,
        "pagingType": 'full',
        "pageLength": 25,
        "lengthChange": false,
        "searching": true,
        "ordering": true,
        "stateSave": true,
        "stateDuration": 60 * 60 * 24,
        "language": {},
        'aoColumnDefs': [{
            'bSortable': false,
            'aTargets': ['nosort']
        }]
    };
    if (langToLoad === 'de') {
        dataTableOptions.language = {
            "decimal": ",",
            "url": '/datatables.de.json'
        };
    }
    $('.table').DataTable(dataTableOptions);
});