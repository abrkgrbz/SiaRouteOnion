"use strict";
var KTDatatablesServerSide = function () {
    // Shared variables
    var table;
    var dt;
    var filterPayment;

    // Private functions
    var initDatatable = function () {
        dt = $("#table_projectList").DataTable({
            ajax: {
                url: "project-list/LoadTable",
                type: "POST"
            },
            "scrollY": 600,
            "scrollX": true,
            searchDelay: 500,
            processing: true,
            serverSide: true,
            order: [[0, "desc"]],
            stateSave: true,
            language: {
                url: '/assets/customjs/turkish.json',
            },

            columns: [
                { data: "projectId", name: "projectId" },
                { data: "projectName", name: "projectName" },
                { data: "projectHeader", name: "projectHeader" },
                { data: "projectOfficers", name: "projectOfficers" },
                { data: "projectStatus", name: "projectStatus" },
                { data: "planlananSahaBitis", name: "planlananSahaBitis" },
                { data: "planlananRaporTeslim", name: "planlananRaporTeslim" },

                { data: null },
                { data: null },
            ],
            columnDefs: [
                {
                    targets: 3,
                    render: function (data, type, row) {
                        // Kullanılacak renklerin dizisi
                        const colors = [
                            'bg-info text-inverse-info',
                            'bg-warning text-inverse-warning',
                            'bg-success text-inverse-success',
                            'bg-danger text-inverse-danger',
                            'bg-primary text-inverse-primary',
                            'bg-dark text-inverse-dark',
                            'bg-secondary text-inverse-secondary',
                        ];

                        // Symbol grup başlangıcı
                        let html = '<div class="symbol-group symbol-hover mb-9">';

                        // Her bir elemanı işleyerek HTML string oluşturma
                        html += data.map((item, index) => {
                            // Rastgele bir rengi seç
                            const colorClass = colors[index % colors.length];

                            return `
                <div class="symbol symbol-35px symbol-circle" data-bs-toggle="tooltip" title data-bs-original-title="${item}">
                    <span class="symbol-label ${colorClass} fw-bolder">${item}</span>
                </div>
            `;
                        }).join('');

                        // Symbol grup bitişi
                        html += '</div>';

                        return html;
                    }
                }, 
                {
                    targets: 5,
                    render: function (data, type, row) {
                        moment.locale("tr");
                        // Veri kontrolü: null, undefined veya geçersiz tarih olup olmadığını kontrol et
                        if (data === null || data === undefined || data === "" || !moment(data).isValid()) {
                            return '-';
                        }
                        return moment(data).format('ll');
                    }
                }, 
                {
                    targets: 6,
                    render: function (data, type, row) {
                        moment.locale("tr");
                        // Veri kontrolü: null, undefined veya geçersiz tarih olup olmadığını kontrol et
                        if (data === null || data === undefined || data === "" || !moment(data).isValid()) {
                            return '-';
                        }
                        return moment(data).format('ll');
                    }
                },
                {
                    targets: 4,
                    render: function (data, type, row) {
                        if (data === 1) {
                            return `<div class="badge badge-light-warning" >Onay Toplantısı</div>`
                        }
                        if (data === 2) {
                            return `<div class="badge badge-light-info" >Soru Formu Paylaşımı</div>`
                        }
                        if (data === 3) {
                            return `<div class="badge badge-light-dark" >Script Yazımı</div>`
                        }
                        if (data === 4) {
                            return `<div class="badge badge-light-primary" >Script Onayı</div>`
                        }
                        if (data === 5) {
                            return `<div class="badge badge-light-danger" >Kota Dosyası</div>`
                        }
                        if (data === 6) {
                            return `<div class="badge badge-light-success" >Saha Başlangıç</div>`
                        }
                        if (data === 7) {
                            return `<div class="badge badge-secondary ">Sahada</div>`
                        }
                        if (data === 8) {
                            return `<div class="badge badge-warning"> Saha Bitis</div>`
                        }
                        if (data === 9) {
                            return `<div class="badge badge-info" >Kodlama</div>`
                        }
                        if (data === 10) {
                            return `<div class="badge badge-dark" >Tablolama</div>`
                        }
                        if (data === 11) {
                            return `<div class="badge badge-primary" >MS Analizleri</div>`
                        }
                        if (data === 12) {
                            return `<div class="badge badge-danger" >Ek Istekler</div>`
                        }
                        if (data === 13) {
                            return `<div class="badge badge-light" >Raporlama</div>`
                        }
                    }
                },
                //{
                //    targets: 3,
                //    render: function (data, type, row) {
                //        if (data === true) {
                //            return `<div class="badge badge-light-success" >Aktif</div>`
                //        }
                //        else{
                //            return `<div class="badge badge-light-danger" >Pasif</div>`
                //        }

                //    }
                //},
                {
                    targets: 7,
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return `

                       <a href="#" onclick="showDetailsProject(${row.projectId})" class="btn btn-active-icon-info btn-active-text-info">
                        <span class="svg-icon svg-icon-1">
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
<path opacity="0.3" d="M22.1 11.5V12.6C22.1 13.2 21.7 13.6 21.2 13.7L19.9 13.9C19.7 14.7 19.4 15.5 18.9 16.2L19.7 17.2999C20 17.6999 20 18.3999 19.6 18.7999L18.8 19.6C18.4 20 17.8 20 17.3 19.7L16.2 18.9C15.5 19.3 14.7 19.7 13.9 19.9L13.7 21.2C13.6 21.7 13.1 22.1 12.6 22.1H11.5C10.9 22.1 10.5 21.7 10.4 21.2L10.2 19.9C9.4 19.7 8.6 19.4 7.9 18.9L6.8 19.7C6.4 20 5.7 20 5.3 19.6L4.5 18.7999C4.1 18.3999 4.1 17.7999 4.4 17.2999L5.2 16.2C4.8 15.5 4.4 14.7 4.2 13.9L2.9 13.7C2.4 13.6 2 13.1 2 12.6V11.5C2 10.9 2.4 10.5 2.9 10.4L4.2 10.2C4.4 9.39995 4.7 8.60002 5.2 7.90002L4.4 6.79993C4.1 6.39993 4.1 5.69993 4.5 5.29993L5.3 4.5C5.7 4.1 6.3 4.10002 6.8 4.40002L7.9 5.19995C8.6 4.79995 9.4 4.39995 10.2 4.19995L10.4 2.90002C10.5 2.40002 11 2 11.5 2H12.6C13.2 2 13.6 2.40002 13.7 2.90002L13.9 4.19995C14.7 4.39995 15.5 4.69995 16.2 5.19995L17.3 4.40002C17.7 4.10002 18.4 4.1 18.8 4.5L19.6 5.29993C20 5.69993 20 6.29993 19.7 6.79993L18.9 7.90002C19.3 8.60002 19.7 9.39995 19.9 10.2L21.2 10.4C21.7 10.5 22.1 11 22.1 11.5ZM12.1 8.59998C10.2 8.59998 8.6 10.2 8.6 12.1C8.6 14 10.2 15.6 12.1 15.6C14 15.6 15.6 14 15.6 12.1C15.6 10.2 14 8.59998 12.1 8.59998Z" fill="currentColor"/>
<path d="M17.1 12.1C17.1 14.9 14.9 17.1 12.1 17.1C9.30001 17.1 7.10001 14.9 7.10001 12.1C7.10001 9.29998 9.30001 7.09998 12.1 7.09998C14.9 7.09998 17.1 9.29998 17.1 12.1ZM12.1 10.1C11 10.1 10.1 11 10.1 12.1C10.1 13.2 11 14.1 12.1 14.1C13.2 14.1 14.1 13.2 14.1 12.1C14.1 11 13.2 10.1 12.1 10.1Z" fill="currentColor"/>
</svg>
                        </span>Detay
                        </a>
                        `
                    }
                },
                {
                    targets: -1,
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return `
                        <a href="#" onclick="deleteProject(${row.projectId})" class="btn btn-active-icon-danger btn-active-text-danger">
                        <span class="svg-icon svg-icon-1">
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
<path opacity="0.3" d="M8.38 22H21C21.2652 22 21.5196 21.8947 21.7071 21.7072C21.8946 21.5196 22 21.2652 22 21C22 20.7348 21.8946 20.4804 21.7071 20.2928C21.5196 20.1053 21.2652 20 21 20H10L8.38 22Z" fill="currentColor"/>
<path d="M15.622 15.6219L9.855 21.3879C9.66117 21.582 9.43101 21.7359 9.17766 21.8409C8.92431 21.946 8.65275 22 8.37849 22C8.10424 22 7.83268 21.946 7.57933 21.8409C7.32598 21.7359 7.09582 21.582 6.90199 21.3879L2.612 17.098C2.41797 16.9042 2.26404 16.674 2.15903 16.4207C2.05401 16.1673 1.99997 15.8957 1.99997 15.6215C1.99997 15.3472 2.05401 15.0757 2.15903 14.8224C2.26404 14.569 2.41797 14.3388 2.612 14.145L8.37801 8.37805L15.622 15.6219Z" fill="currentColor"/>
<path opacity="0.3" d="M8.37801 8.37805L14.145 2.61206C14.3388 2.41803 14.569 2.26408 14.8223 2.15906C15.0757 2.05404 15.3473 2 15.6215 2C15.8958 2 16.1673 2.05404 16.4207 2.15906C16.674 2.26408 16.9042 2.41803 17.098 2.61206L21.388 6.90198C21.582 7.0958 21.736 7.326 21.841 7.57935C21.946 7.83269 22 8.10429 22 8.37854C22 8.65279 21.946 8.92426 21.841 9.17761C21.736 9.43096 21.582 9.66116 21.388 9.85498L15.622 15.6219L8.37801 8.37805Z" fill="currentColor"/>
</svg>
                        </span>Sil
                        </a>
                        `
                    }
                }

            ],
            createdRow: function (row, data, dataIndex) {
                $(row).find('td:eq(2)').attr('data-filter', data.projectStatus);
                $(row).find('td:eq(3)').attr('data-filter', data.isActive);
                $(row).find('td:eq(4)').attr('data-filter', data.created);
            }
        });

        table = dt.$;

        dt.on('draw', function () {
            KTMenu.createInstances();
        });
    }

    var handleFilterDatatable = () => {
        filterPayment = document.querySelectorAll('[data-kt-project-table-filter="isActive"] [name="isActive"]');
        const filterButton = document.querySelector('[data-kt-project-table-filter="filter"]');

        filterButton.addEventListener('click', function () {
            let paymentValue = '';
            filterPayment.forEach(r => {
                if (r.checked) {

                    paymentValue = r.value;
                }

            });
            dt.search(paymentValue).draw();
        });
    }
    var handleSearchDatatable = function () {
        const filterSearch = document.querySelector('[data-kt-user-table-filter="search"] ');
        filterSearch.addEventListener('keyup', function (e) {
            dt.search(e.target.value).draw();
        });
    }
    var handleResetForm = () => {
        const resetButton = document.querySelector('[data-kt-project-table-filter="reset"]');

        resetButton.addEventListener('click', function () {
            dt.search('').draw();
        });
    }
    return {
        init: function () {
            initDatatable();
            handleResetForm();
            handleFilterDatatable();
            handleSearchDatatable();
        }
    }
}();

KTUtil.onDOMContentLoaded(function () {
    KTDatatablesServerSide.init();
});


function showDetailsProject(id) { 
    window.location.href = `project-details?id=${id}`
}


async function checkUserRole() {
    const response = await fetch("check-role");
    const data = await response.json(); 
    return data.isAuthorized;
}

async function deleteProject(id) { 
    if (await checkUserRole())
    {
        Swal.fire({
            text: "Bu projeyi silmek istediğinize emin misiniz?",
            icon: "warning",
            showCancelButton: true,
            buttonsStyling: false,
            confirmButtonText: "Evet, Sil",
            cancelButtonText: "Hayır, İptal",
            customClass: {
                confirmButton: "btn btn-danger",
                cancelButton: "btn btn-secondary"
            }
        }).then(function (result) {
            if (result.isConfirmed) {
                $.ajax({
                    url: 'delete-project',
                    type: 'POST',
                    data: { projectId: id },
                    success: function (response) {
                        Swal.fire({
                            text: "Proje başarıyla silindi.",
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Tamam",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            }
                        }).then(function () {
                            // Tabloyu yeniden yükle
                            $('#table_projectList').DataTable().ajax.reload();
                        });
                    },
                    error: function (xhr, status, error) {
                        Swal.fire({
                            text: "Proje silinirken hata oluştu. Lütfen tekrar deneyin.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Tamam",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            }
                        });
                    }
                });
            }
        });
    }
    else
    {
        Swal.fire({
            text: "Silmek için yetkiniz bulunmamaktadır!",
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Tamam",
            customClass: {
                confirmButton: "btn btn-danger"
            }
        });
    }
      
}
