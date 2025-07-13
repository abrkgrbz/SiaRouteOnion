"use strict";
var KTDatatablesServerSide = function () {
    // Shared variables
    var table;
    var dt;
    var filterPayment;

    // Private functions
    var initDatatable = function () {
        dt = $('#table_projectList').DataTable({
            ajax: {
                url: "project-list/LoadTable",
                type: "POST"
            },
            scrollY: 600,
            scrollX: true,
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
                { data: null }
            ],
            columnDefs: [
                {
                    targets: 3,
                    render: function (data, type, row) {
                        // Kullanılacak renklerin dizisi
                        const colors = [
                            'bg-primary text-inverse-primary',
                            'bg-success text-inverse-success',
                            'bg-danger text-inverse-danger',
                            'bg-warning text-inverse-warning',
                            'bg-info text-inverse-info',
                            'bg-dark text-inverse-dark'
                        ];

                        // Symbol grup başlangıcı
                        let html = '<div class="symbol-group symbol-hover mb-3">';

                        // Her bir elemanı işleyerek HTML string oluşturma
                        if (data && data.length) {
                            html += data.map((item, index) => {
                                // İsim baş harflerini al
                                const initials = item.split(' ').map(n => n[0]).join('');
                                // Rastgele bir rengi seç
                                const colorClass = colors[index % colors.length];

                                return `
                        <div class="symbol symbol-35px symbol-circle" data-bs-toggle="tooltip" title="${item}">
                            <span class="symbol-label ${colorClass} fw-bolder">${initials}</span>
                        </div>
                    `;
                            }).join('');
                        } else {
                            html += `<span class="text-muted">Ekip atanmamış</span>`;
                        }

                        // Symbol grup bitişi
                        html += '</div>';

                        return html;
                    }
                },
                {
                    targets: 5,
                    render: function (data, type, row) {
                        // Veri kontrolü: null, undefined veya geçersiz tarih olup olmadığını kontrol et
                        if (data === null || data === undefined || data === "" || !moment(data).isValid()) {
                            return '-';
                        }
                        return moment(data).format('DD.MM.YYYY');
                    }
                },
                {
                    targets: 6,
                    render: function (data, type, row) {
                        // Veri kontrolü: null, undefined veya geçersiz tarih olup olmadığını kontrol et
                        if (data === null || data === undefined || data === "" || !moment(data).isValid()) {
                            return '-';
                        }
                        return moment(data).format('DD.MM.YYYY');
                    }
                },
                {
                    targets: 4,
                    render: function (data, type, row) {
                        let statusClass = '';
                        let statusText = '';

                        switch (parseInt(data)) {
                            case 1: statusClass = 'badge-light-warning'; statusText = 'Onay Toplantısı'; break;
                            case 2: statusClass = 'badge-light-info'; statusText = 'Soru Formu Paylaşımı'; break;
                            case 3: statusClass = 'badge-light-dark'; statusText = 'Script Yazımı'; break;
                            case 4: statusClass = 'badge-light-primary'; statusText = 'Script Onayı'; break;
                            case 5: statusClass = 'badge-light-danger'; statusText = 'Kota Dosyası'; break;
                            case 6: statusClass = 'badge-light-success'; statusText = 'Saha Başlangıç'; break;
                            case 7: statusClass = 'badge-secondary'; statusText = 'Sahada'; break;
                            case 8: statusClass = 'badge-warning'; statusText = 'Saha Bitiş'; break;
                            case 9: statusClass = 'badge-info'; statusText = 'Kodlama'; break;
                            case 10: statusClass = 'badge-dark'; statusText = 'Tablolama'; break;
                            case 11: statusClass = 'badge-primary'; statusText = 'MS Analizleri'; break;
                            case 12: statusClass = 'badge-danger'; statusText = 'Ek İstekler'; break;
                            case 13: statusClass = 'badge-light'; statusText = 'Raporlama'; break;
                            default: statusClass = 'badge-light-secondary'; statusText = 'Belirsiz'; break;
                        }

                        return `<div class="badge ${statusClass}">${statusText}</div>`;
                    }
                },
                {
                    targets: 7,
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return `
                       <a href="#" onclick="showDetailsProject(${row.projectId})" class="btn btn-sm btn-light-primary">
                            <i class="bi bi-eye me-1"></i> Detaylar
                       </a>
                    `;
                    }
                },
                {
                    targets: 8,
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return `
                            <a href="#" onclick="deleteProject(${row.projectId})" class="btn btn-sm btn-light-danger">
                                <i class="bi bi-trash me-1"></i> Sil
                            </a>
                        `;
                    }
                }
            ],
            drawCallback: function () {
                // Tooltips'leri etkinleştir
                $('[data-bs-toggle="tooltip"]').tooltip();
            }
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
        const searchInput = $('[data-kt-user-table-filter="search"]');

        // Arama kutusu olayını dinle
        searchInput.on('keyup', function () {
            const searchTerm = $(this).val().toLowerCase();
            
            // Grid view aktifse
            if ($("#grid-view").hasClass("active")) {
                // Grid view'daki projeleri filtrele
                filterGridProjects(searchTerm);
            } else {
                // Liste görünümünde DataTable'ın kendi arama işlevini kullan
                dt.search(searchTerm).draw();
            }
        });

        // Grid view'daki projeleri filtreleme fonksiyonu
        function filterGridProjects(searchTerm) {
            const projectCards = $('#grid-view .project-card');

            // Eğer arama terimi boşsa, tüm projeleri göster
            if (!searchTerm) {
                projectCards.closest('.col-md-6').show();
                return;
            }

            // Her proje kartını kontrol et
            projectCards.each(function () {
                const card = $(this);
                const projectHeader = card.find('.fs-3').text().toLowerCase();
                const projectCode = card.find('.fs-7').text().toLowerCase();
                const projectStatus = card.find('.project-status-badge').text().toLowerCase();

                // Eğer arama terimi proje başlığı, kodu veya durumunda bulunuyorsa kartı göster, yoksa gizle
                if (projectHeader.includes(searchTerm) ||
                    projectCode.includes(searchTerm) ||
                    projectStatus.includes(searchTerm)) {
                    card.closest('.col-md-6').show();
                } else {
                    card.closest('.col-md-6').hide();
                }
            });

            // Eğer hiç sonuç yoksa, kullanıcıya bilgi ver
            if ($('#grid-view .project-card:visible').length === 0) {
                if ($('#grid-view .no-results-message').length === 0) {
                    $('#grid-view .row').append('<div class="col-12 text-center p-10 no-results-message"><div class="text-muted">Aramanızla eşleşen proje bulunamadı.</div></div>');
                }
            } else {
                $('#grid-view .no-results-message').remove();
            }
        }

        // Tab değişiminde arama kutusunu temizle
        $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
            searchInput.val('');

            // Görünüme göre filtrelemeyi sıfırla
            if ($("#grid-view").hasClass("active")) {
                $('#grid-view .project-card').closest('.col-md-6').show();
                $('#grid-view .no-results-message').remove();
            } else {
                $('#table_projectList').DataTable().search('').draw();
            }
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
