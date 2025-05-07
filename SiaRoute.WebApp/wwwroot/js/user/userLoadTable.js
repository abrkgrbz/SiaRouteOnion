"use strict";
var KTDatatablesServerSide = function () {



    // Shared variables
    var table;
    var dt;
    var filterPayment;

    // Private functions
    var initDatatable = function () {
        const urlParams = new URLSearchParams(window.location.search);
        const projectId = urlParams.get('id');
        dt = $("#table_userList").DataTable({
            ajax: {
                url: "project-users/LoadTable",
                type: "POST",
                data: function (d) {
                    d.projectId = projectId;
                }
            },
            searchDelay: 500,
            processing: true,
            serverSide: true,
            order: [[1, "asc"]],
            select: {
                style: 'multi',
                selector: 'td:first-child input[type="checkbox"]',
                className: 'row-selected'
            },
            stateSave: true,
            language: {
                url: '/assets/customjs/turkish.json',
            },

            columns: [
                { data: "id" },
                { data: "fullname", name: "fullname" },
                { data: "email", name: "email" },
                { data: "departmanName", name: "departmanName" } 
            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    render: function (data) {
                        return `
                            <div class="form-check form-check-sm form-check-custom form-check-solid">
                                <input class="form-check-input" type="checkbox" value="${data}" />
                            </div>`;
                    }
                } 

            ]
        });

        table = dt.$;

        dt.on('draw', function () {
            initToggleToolbar();
            KTMenu.createInstances();
        });
    }

    var handleSearchDatatable = function () {
        const filterSearch = document.querySelector('[data-kt-user-table-filter="search"] ');
        filterSearch.addEventListener('keyup', function (e) {
            dt.search(e.target.value).draw();
        });
    }
    var initToggleToolbar = function () {
        const container = document.querySelector('#table_userList');
        const checkboxes = container.querySelectorAll('[type="checkbox"]');
        const deleteSelected = document.querySelector('[data-kt-docs-table-select="delete_selected"]');

        // Proje ID'sini URL'den al
        const urlParams = new URLSearchParams(window.location.search);
        const projectId = urlParams.get('id');

        checkboxes.forEach(c => {
            c.addEventListener('click', function () {
                setTimeout(function () {
                    toggleToolbars();
                }, 50);
            });
        });

        deleteSelected.addEventListener('click', function () {
            const selectedIds = Array.from(checkboxes)
                .filter(checkbox => checkbox.checked)
                .map(checkbox => checkbox.value);

            Swal.fire({
                text: "Seçili kullanıcıları ilgi projeye atamak istediğinize emin misiniz?",
                icon: "warning",
                showCancelButton: true,
                buttonsStyling: false,
                showLoaderOnConfirm: true,
                confirmButtonText: "Evet",
                cancelButtonText: "Vazgeç",
                customClass: {
                    confirmButton: "btn fw-bold btn-success",
                    cancelButton: "btn fw-bold btn-active-light-danger"
                },
            }).then(function (result) {
                if (result.value) {
                    // AJAX POST isteği
                    fetch('add-user-project', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({ projectId: parseInt(projectId), userIds: selectedIds })

                    }).then(response => response.json())
                        .then(data => {
                            Swal.fire({
                                text: "Kullanıcı ekleme işlemi başarıyla gerçekleşti",
                                icon: "success",
                                buttonsStyling: false,
                                confirmButtonText: "Tamam",
                                customClass: { confirmButton: "btn fw-bold btn-primary" }
                            }).then(function () {

                                const headerCheckbox = container.querySelectorAll('[type="checkbox"]')[0];
                                headerCheckbox.checked = false;
                            });

                        })
                        .catch(error => {
                            Swal.fire({
                                text: "Bir hata oluştu, lütfen tekrar deneyin",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Tamam",
                                customClass: { confirmButton: "btn fw-bold btn-primary" }
                            });
                            console.error('Error:', error);
                        });
                } else if (result.dismiss === 'cancel') {
                    Swal.fire({
                        text: "Seçili kullanıcılar eklenmedi",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Tamam",
                        customClass: {
                            confirmButton: "btn fw-bold btn-primary",
                        }
                    });
                }
            });
        });

    }

    var toggleToolbars = function () {
        const container = document.querySelector('#table_userList');
        const toolbarBase = document.querySelector('[data-kt-docs-table-toolbar="base"]');
        const toolbarSelected = document.querySelector('[data-kt-docs-table-toolbar="selected"]');
        const selectedCount = document.querySelector('[data-kt-docs-table-select="selected_count"]');
        const allCheckboxes = container.querySelectorAll('tbody [type="checkbox"]');

        let checkedState = false;
        let count = 0;

        allCheckboxes.forEach(c => {
            if (c.checked) {
                checkedState = true;
                count++;
            }
        });

        if (checkedState) {
            selectedCount.innerHTML = count;
            toolbarBase.classList.add('d-none');
            toolbarSelected.classList.remove('d-none');
        } else {
            toolbarBase.classList.remove('d-none');
            toolbarSelected.classList.add('d-none');
        }
    }

    return {
        init: function () {
            initDatatable();
            handleSearchDatatable();
            initToggleToolbar();

        }
    }
}();

KTUtil.onDOMContentLoaded(function () {
    KTDatatablesServerSide.init();
});


function showDetailsProject(id) {
    window.location.href = `project-details?id=${id}`
}

 