"use strict";
var KTDatatablesServerSide = function () {



    // Shared variables
    var table;
    var dt;
    var filterPayment;

    // Private functions
    var initDatatable = function () {
        dt = $("#table_userListLoadTable").DataTable({
            ajax: {
                url: "user-list/LoadTable",
                type: "POST"
            },
            searchDelay: 500,
            processing: true,
            serverSide: true,
            order: [[1, "desc"]],
            stateSave: true,
            language: {
                url: '/assets/customjs/turkish.json',
            },

            columns: [
                { data: null },
                { data: "fullName", name: "fullName" },
                { data: "email", name: "email" },
                { data: "emailConfirmed", name: "emailConfirmed" },
                { data: "roles", name: "roles" }, 
                { data: "departmanName", name: "departmanName" },
                { data: null },
                { data: null }

            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var pageInfo = dt.page.info();
                        return pageInfo.start + meta.row + 1;
                    }
                },
                {
                    targets: 3,
                    orderable: false,
                    render: function (data) {
                        if (data === true) {
                            return `<div class="badge badge-light-success" >Onaylandı</div>`
                        }
                        if (data === false) {
                            return `<div class="badge badge-light-danger" >Onay Bekliyor</div>`
                        }
                    }
                },
                {
                    targets: 4,
                    orderable: false,
                    render: function (data) {
                        var roles = data.split(',');
                        var badges = roles.map(role => createBadge(role)).join(' ');
                        return badges;
                    }
                },
                {
                    targets: 6,
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        if (row.emailConfirmed === false) {
                            return ` <a href="#" onclick="approveUser('${row.id}')" class="btn btn-bg-success btn-sm text-white">Onayla</a> `;
                        }
                        return '';


                    }
                },
                {
                    targets: -1,
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        if (row.emailConfirmed === true) {
                            return `<a href="#" class="btn btn-icon btn-secondary">
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-person-up" viewBox="0 0 16 16">
                            <path d="M12.5 16a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7m.354-5.854 1.5 1.5a.5.5 0 0 1-.708.708L13 11.707V14.5a.5.5 0 0 1-1 0v-2.793l-.646.647a.5.5 0 0 1-.708-.708l1.5-1.5a.5.5 0 0 1 .708 0M11 5a3 3 0 1 1-6 0 3 3 0 0 1 6 0M8 7a2 2 0 1 0 0-4 2 2 0 0 0 0 4"/>
                            <path d="M8.256 14a4.5 4.5 0 0 1-.229-1.004H3c.001-.246.154-.986.832-1.664C4.484 10.68 5.711 10 8 10q.39 0 .74.025c.226-.341.496-.65.804-.918Q8.844 9.002 8 9c-5 0-6 3-6 4s1 1 1 1z"/>
                             </svg></a>`;

                        }
                        return '';
                    }
                }

            ]

        });

        table = dt.$;

        dt.on('draw', function () {
            KTMenu.createInstances();
        });
    }

    var handleSearchDatatable = function () {
        const filterSearch = document.querySelector('[data-kt-user-table-filter="search"] ');
        filterSearch.addEventListener('keyup', function (e) {
            dt.search(e.target.value).draw();
        });
    }

    return {
        init: function () {
            initDatatable();
            handleSearchDatatable();

        }
    }
}();

KTUtil.onDOMContentLoaded(function () {
    KTDatatablesServerSide.init();
});


function createBadge(role) {
    switch (role) {
        case "Basic":
            return "<span class='badge badge-info'>Araştırmacı</span>";
        case "Moderator":
            return "<span class='badge badge-danger'>Moderator</span>";
        case "Admin":
            return "<span class='badge badge-primary'>SPV</span>";
        case "SuperAdmin":
            return "<span class='badge badge-dark'>SuperAdmin</span>";
        default:
            return "<span class='badge badge-light'>Unknown</span>";
    }
}

function approveUser(id) {
    console.log(id)
    Swal.fire({
        title: 'Lütfen bekleyiniz',
        text: 'İşlem devam ediyor...',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
            fetch('user-approve', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ userId: id })
            })
                .then(response => response.json())
                .then(data => {
                    console.log("Kullanıcı onaylandı:", data);
                    Swal.fire({
                        icon: 'success',
                        title: 'Başarılı',
                        text: 'Kullanıcı başarıyla onaylandı!',
                        confirmButtonText: 'Tamam'
                    });
                })
                .catch((error) => {
                    console.error("Hata oluştu:", error);
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: 'Kullanıcı onaylama işlemi başarısız oldu!',
                        confirmButtonText: 'Tamam'
                    });
                });
        }
    });
}

