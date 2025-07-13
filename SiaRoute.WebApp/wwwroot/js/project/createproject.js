$(document).ready(function () {
    // Değişkenler
    const formWizard = $('.form-wizard');
    const formWizardSteps = $('.form-wizard-step');
    const formWizardPanels = $('.form-wizard-panel');
    const prevStepBtn = $('#prev-step');
    const nextStepBtn = $('#next-step');
    const submitFormBtn = $('#submit-form');
    const progressBar = $('.form-wizard-progress-bar');

    let currentStep = 1;
    const totalSteps = formWizardSteps.length;

    // İlerleme çubuğunu güncelle
    function updateProgressBar() {
        const progress = ((currentStep - 1) / (totalSteps - 1)) * 100;
        progressBar.css('width', `${progress}%`);
    }

    // Adımı Değiştir
    function goToStep(step) {
        // Geçerli adımı ve paneli gizle
        $(`.form-wizard-step[data-step="${currentStep}"]`).removeClass('active');
        $(`.form-wizard-panel[data-step="${currentStep}"]`).removeClass('active');

        // Yeni adımı ve paneli göster
        $(`.form-wizard-step[data-step="${step}"]`).addClass('active');
        $(`.form-wizard-panel[data-step="${step}"]`).addClass('active');

        // Tamamlanan adımları işaretle
        formWizardSteps.each(function () {
            const stepNum = $(this).data('step');
            if (stepNum < step) {
                $(this).removeClass('active').addClass('completed');
            } else if (stepNum > step) {
                $(this).removeClass('active completed');
            }
        });

        // Geri butonunu güncelle
        if (step === 1) {
            prevStepBtn.prop('disabled', true);
        } else {
            prevStepBtn.prop('disabled', false);
        }

        // İleri/Gönder butonunu güncelle
        if (step === totalSteps) {
            nextStepBtn.hide();
            submitFormBtn.show();
            updateSummary();
        } else {
            nextStepBtn.show();
            submitFormBtn.hide();
        }
         
        currentStep = step;
        updateProgressBar(); 
        $('html, body').animate({
            scrollTop: formWizard.offset().top - 100
        }, 500);
    }

    // Özet bilgilerini güncelle
    function updateSummary() {
        // Temel bilgiler
        $('#summary-project-code').text($('input[name="ProjectName"]').val() || '-');
        $('#summary-project-name').text($('input[name="ProjectHeader"]').val() || '-');
        $('#summary-customer').text($('select[name="CustomerName"] option:selected').text() || '-');

        // Sektör
        const sectorValue = $('input[name="ProjectSector"]:checked').val();
        if (sectorValue === undefined) {
            $('#summary-sector').text('-');
        } else if ($('#other-option').is(':checked')) {
            $('#summary-sector').text($('input[name="ProjectSectorInput"]').val() || 'Diğer');
        } else {
            $('#summary-sector').text($('input[name="ProjectSector"]:checked').closest('.form-wizard-checkbox-label').find('.form-wizard-checkbox-title').text() || '-');
        }

        // Metot ve Örneklem
        const methods = [];
        const sampleSizes = [];
        $('input[name="ProjectMethod"]:checked').each(function () {
            const methodValue = $(this).val();
            const methodName = $(this).closest('.form-wizard-checkbox-label').find('.form-wizard-checkbox-title').text();
            const sampleSize = $(`input[name="${methodValue}Size"]`).val();

            methods.push(methodName);
            if (sampleSize) {
                sampleSizes.push(`${methodName}: ${sampleSize}`);
            }
        });

        $('#summary-method').text(methods.length > 0 ? methods.join(', ') : '-');
        $('#summary-sample-size').text(sampleSizes.length > 0 ? sampleSizes.join(', ') : '-');

        // Saha Firması
        const sahaFirmalari = [];
        $('#sahaSelect option:selected').each(function () {
            sahaFirmalari.push($(this).text());
        });
        $('#summary-scm').text(sahaFirmalari.length > 0 ? sahaFirmalari.join(', ') : '-');

        // Tarihler
        $('#summary-saha-start').text($('#planlanan_saha_baslangic').val() || '-');
        $('#summary-saha-end').text($('#planlanan_saha_bitis').val() || '-');
        $('#summary-report-date').text($('#planlanan_rapor_teslim').val() || '-');

        // Ekip ve Durum
        $('#summary-manager').text($('#userManagerSelect option:selected').text() || '-');

        const ekipUyeleri = [];
        $('#userSelect option:selected').each(function () {
            ekipUyeleri.push($(this).text());
        });
        $('#summary-team').text(ekipUyeleri.length > 0 ? ekipUyeleri.join(', ') : '-');

        const projeStatus = $('select[name="ProjectStatus"] option:selected').text();
        $('#summary-status').text(projeStatus || '-');
    }

    // Form Doğrulama
    function validateStep(step) {
        let isValid = true;
        const currentPanel = $(`.form-wizard-panel[data-step="${step}"]`);

        // Her adım için özel doğrulama kuralları
        if (step === 1) {
            // Proje Bilgileri
            const projectCode = $('input[name="ProjectName"]').val();
            const projectName = $('input[name="ProjectHeader"]').val();
            const customer = $('select[name="CustomerName"]').val();

            if (!projectCode) {
                isValid = false;
                showError('input[name="ProjectName"]', 'Proje kodu zorunludur');
            } else {
                removeError('input[name="ProjectName"]');
            }

            if (!projectName) {
                isValid = false;
                showError('input[name="ProjectHeader"]', 'Proje adı zorunludur');
            } else {
                removeError('input[name="ProjectHeader"]');
            }

            if (!customer) {
                isValid = false;
                showError('select[name="CustomerName"]', 'Müşteri seçimi zorunludur');
            } else {
                removeError('select[name="CustomerName"]');
            }
        }
        else if (step === 2) {
            // Sektör Seçimi
            const sectorSelected = $('input[name="ProjectSector"]:checked').length > 0;

            if (!sectorSelected) {
                isValid = false;
                showStepError(2, 'Lütfen bir sektör seçiniz');
            } else {
                removeStepError(2);

                // Diğer seçeneği kontrol et
                if ($('#other-option').is(':checked')) {
                    const otherSector = $('input[name="ProjectSectorInput"]').val();
                    if (!otherSector) {
                        isValid = false;
                        showError('input[name="ProjectSectorInput"]', 'Sektör adı giriniz');
                    } else {
                        removeError('input[name="ProjectSectorInput"]');
                    }
                }
            }
        }
        else if (step === 3) {
            // Metot Seçimi
            const methodSelected = $('input[name="ProjectMethod"]:checked').length > 0;

            if (!methodSelected) {
                isValid = false;
                showStepError(3, 'Lütfen en az bir metot seçiniz');
            } else {
                removeStepError(3);
                 
                let allSampleSizesValid = true;
                $('input[name="ProjectMethod"]:checked').each(function () {
                    const methodValue = $(this).val();
                    const sampleSizeInput = $(`input[name="${methodValue}Size"]`);

                    if (sampleSizeInput.length && !sampleSizeInput.val()) {
                        allSampleSizesValid = false;
                        showError(sampleSizeInput, 'Örneklem büyüklüğü giriniz');
                    } else if (sampleSizeInput.length) {
                        removeError(sampleSizeInput);
                    }
                });

                if (!allSampleSizesValid) {
                    isValid = false;
                }
            }
        }
        else if (step === 4) {
            // Saha Firması
            const sahaFirmasi = $('#sahaSelect').val();

            if (!sahaFirmasi || sahaFirmasi.length === 0) {
                isValid = false;
                showError('#sahaSelect', 'En az bir saha firması seçiniz');
            } else {
                removeError('#sahaSelect');
            }
        }
        else if (step === 6) {
            // Proje Ekibi
            const projectManager = $('#userManagerSelect').val();
            const projectOfficers = $('#userSelect').val();
            const projectStatus = $('select[name="ProjectStatus"]').val();

            if (!projectManager) {
                isValid = false;
                showError('#userManagerSelect', 'Ekip yöneticisi seçiniz');
            } else {
                removeError('#userManagerSelect');
            }

            if (!projectOfficers || projectOfficers.length === 0) {
                isValid = false;
                showError('#userSelect', 'En az bir ekip üyesi seçiniz');
            } else {
                removeError('#userSelect');
            }

            if (!projectStatus) {
                isValid = false;
                showError('select[name="ProjectStatus"]', 'Proje durumunu seçiniz');
            } else {
                removeError('select[name="ProjectStatus"]');
            }
        }

        return isValid;
    }

    // Hata gösterme fonksiyonları
    function showError(selector, message) {
        const element = $(selector);
        element.addClass('is-invalid');

        // Hata mesajı göster
        if (!element.next('.invalid-feedback').length) {
            element.after(`<div class="invalid-feedback">${message}</div>`);
        } else {
            element.next('.invalid-feedback').text(message);
        }
    }

    function removeError(selector) {
        const element = $(selector);
        element.removeClass('is-invalid');
        element.next('.invalid-feedback').remove();
    }

    function showStepError(step, message) {
        const stepPanel = $(`.form-wizard-panel[data-step="${step}"]`);

        if (!stepPanel.find('.alert-danger').length) {
            stepPanel.prepend(`
                        <div class="alert alert-danger d-flex align-items-center p-5 mb-10">
                            <span class="svg-icon svg-icon-2hx svg-icon-danger me-4">
                                <i class="bi bi-exclamation-triangle-fill fs-2"></i>
                            </span>
                            <div class="d-flex flex-column">
                                <h5 class="mb-1 text-danger">Hata</h5>
                                <span>${message}</span>
                            </div>
                            <button type="button" class="position-absolute position-sm-relative m-2 m-sm-0 top-0 end-0 btn btn-icon ms-sm-auto" data-bs-dismiss="alert">
                                <i class="bi bi-x fs-2 text-danger"></i>
                            </button>
                        </div>
                    `);
        }
    }

    function removeStepError(step) {
        const stepPanel = $(`.form-wizard-panel[data-step="${step}"]`);
        stepPanel.find('.alert-danger').remove();
    }
     
    nextStepBtn.click(function () { 
        if (validateStep(currentStep)) {
            goToStep(currentStep + 1);
        }
    });
     
    prevStepBtn.click(function () {
        goToStep(currentStep - 1);
    });
     
    submitFormBtn.click(function () { 
        if (validateStep(currentStep)) { 
            const submitButton = $(this);
            submitButton.attr('data-kt-indicator', 'on');
            submitButton.prop('disabled', true);
             
            const formData = new FormData($('#kt_modal_create_app_form')[0]);
             
            let projectSizes = {};
            $('input[name="ProjectMethod"]:checked').each(function () {
                const methodValue = $(this).val();
                const sizeInput = $(`input[name="${methodValue}Size"]`);
                if (sizeInput.length) {
                    projectSizes[methodValue] = sizeInput.val();
                }
            });

            formData.append('ProjectSizes', JSON.stringify(projectSizes));

            // Diğer seçeneği için kontrol
            const otherOption = $('#other-option');
            if (otherOption.is(':checked')) {
                formData.delete('ProjectSector'); // Mevcut veriyi sil
                const inputValue = $('input[name="ProjectSectorInput"]').val();
                formData.append('ProjectSector', inputValue);
            }

            // AJAX ile form gönderimi
            $.ajax({
                url: '/proje-olustur',
                method: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    submitButton.removeAttr('data-kt-indicator');
                    submitButton.prop('disabled', false);
                     
                    Swal.fire({
                        text: "Proje başarıyla oluşturuldu.",
                        icon: "success",
                        buttonsStyling: false,
                        confirmButtonText: "Tamam",
                        customClass: {
                            confirmButton: "btn btn-primary"
                        }
                    }).then(function () { 
                        window.location.href = "project-list";
                    });
                },
                error: function (xhr, status, error) {
                    submitButton.removeAttr('data-kt-indicator');
                    submitButton.prop('disabled', false);

                    let errorMessage = 'Bir hata oluştu. Lütfen tekrar deneyin.';
                    if (xhr.responseJSON && xhr.responseJSON.Errors) {
                        errorMessage = xhr.responseJSON.Errors.join('<br>');
                    }

                    Swal.fire({
                        html: errorMessage,
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
     
    $('#other-option').change(function () {
        if ($(this).is(':checked')) {
            $('#other-text-field').removeClass('d-none');
        } else {
            $('#other-text-field').addClass('d-none');
        }
    });
     
    $('.form-wizard-checkbox-label input[type="checkbox"]').change(function () {
        if ($(this).is(':checked')) {
            $(this).closest('.form-wizard-checkbox-label').addClass('checked');
            $(this).closest('.form-wizard-checkbox-label').next('.form-wizard-sample-size').slideDown();
        } else {
            $(this).closest('.form-wizard-checkbox-label').removeClass('checked');
            $(this).closest('.form-wizard-checkbox-label').next('.form-wizard-sample-size').slideUp();
        }
    });
     
    $('.form-wizard-checkbox-label input[type="radio"]').change(function () {
        $('.form-wizard-checkbox-label input[type="radio"]').each(function () {
            if ($(this).is(':checked')) {
                $(this).closest('.form-wizard-checkbox-label').addClass('checked');
            } else {
                $(this).closest('.form-wizard-checkbox-label').removeClass('checked');
            }
        });
    });
     
    $(".flatpickr-input").flatpickr({
        locale: "tr",
        dateFormat: "d-m-Y",
        allowInput: false,
        altInput: true,
        altFormat: "d F Y",
        disableMobile: true,
        defaultDate: new Date(), // Bugünün tarihini varsayılan olarak kullan
        time_24hr: true, // 24 saat formatını kullan
        parseDate: (datestr, format) => {
            return flatpickr.parseDate(datestr, format);
        },
        formatDate: (date, format, locale) => {
            if (format === "d F Y") {
                const aylar = [
                    'Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran',
                    'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'
                ];
                const gun = date.getDate();
                const ay = aylar[date.getMonth()];
                const yil = date.getFullYear();
                return `${gun} ${ay} ${yil}`;
            }
            return flatpickr.formatDate(date, format, locale);
        }
    });
     
    $("#sahaSelect").select2({
        dropdownParent: $("#kt_modal_create_app_form")
    });

    $("#userSelect").select2({
        dropdownParent: $("#kt_modal_create_app_form")
    });

    $("#userManagerSelect").select2({
        dropdownParent: $("#kt_modal_create_app_form")
    });
     
    $.ajax({
        url: "/GetAllListSCM",
        method: "GET",
        success: function (data) { 
            data.forEach(function (firma) {
                $("#sahaSelect").append(new Option(firma.scmName, firma.id));
            });
        },
        error: function (xhr, status, error) {
            console.error("Firma verileri yüklenirken hata oluştu:", error);
        },
    });
     
    $.ajax({
        url: "/GetUserListGroupByDepartman",
        method: "GET",
        success: function (data) {
            data.forEach(function (group) {
                var optgroup = $('<optgroup>').attr('label', group.departmanName);
                group.users.forEach(function (user) {
                    optgroup.append(new Option(user.fullName, user.userId));
                });
                $('#userSelect').append(optgroup);
                $('#userManagerSelect').append(optgroup.clone());
            });
        },
        error: function (xhr, status, error) {
            console.error("Kullanıcı verileri yüklenirken hata oluştu:", error);
        },
    });
     
    $('#kt_modal_create_app_form :input').change(function () {
        $(this).addClass('form-touched');
    });
     
 
     
    $('.form-wizard-step').click(function () {
        const step = $(this).data('step');
         
        if ($(this).hasClass('completed') || parseInt(step) === currentStep) {
            if (validateStep(currentStep)) {
                goToStep(step);
            }
        }
    });
});