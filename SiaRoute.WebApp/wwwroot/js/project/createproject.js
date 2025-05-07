"use strict";
var KTCreateApp = (function () {

    var e,
        t,
        o,
        r,
        a,
        i,
        n = [];
    return {
        init: function () {
            const scmField = jQuery(kt_modal_create_app_form.querySelector('[name="ScmId"]'));
            const usersField = jQuery(kt_modal_create_app_form.querySelector('[name="ProjectOfficers"]'));
            const userManagerField = jQuery(kt_modal_create_app_form.querySelector('[name="ProjectManager"]'));
            (e = document.querySelector("#kt_modal_create_app")) &&
                (new bootstrap.Modal(e),
                    (t = document.querySelector("#kt_modal_create_app_stepper")),
                    (o = document.querySelector("#kt_modal_create_app_form")),
                    (r = t.querySelector('[data-kt-stepper-action="submit"]')),
                    (a = t.querySelector('[data-kt-stepper-action="next"]')),
                    (i = new KTStepper(t)).on("kt.stepper.changed", function (e) {
                        7 === i.getCurrentStepIndex()
                            ? (r.classList.remove("d-none"),
                                r.classList.add("d-inline-block"),
                                a.classList.add("d-none"))
                            : 7 === i.getCurrentStepIndex()
                                ? (r.classList.add("d-none"), a.classList.add("d-none"))
                                : (r.classList.remove("d-inline-block"),
                                    r.classList.remove("d-none"),
                                    a.classList.remove("d-none"));
                    }),
                    i.on("kt.stepper.next", function (e) {
                        console.log("stepper.next");
                        var t = n[e.getCurrentStepIndex() - 1];
                        t
                            ? t.validate().then(function (t) {
                                console.log("validated!"),
                                    "Valid" == t
                                        ? e.goNext()
                                        : Swal.fire({
                                            text: "Lütfen ilgili alanları doldurunuz.",
                                            icon: "error",
                                            buttonsStyling: !1,
                                            confirmButtonText: "Tamam",
                                            customClass: {
                                                confirmButton: "btn btn-light",
                                            },
                                        }).then(function () { });
                            })
                            : (e.goNext(), KTUtil.scrollTop());
                    }),
                    i.on("kt.stepper.previous", function (e) {
                        console.log("stepper.previous"), e.goPrevious(), KTUtil.scrollTop();
                    }),
                    r.addEventListener("click", function (e) {
                        n[3].validate().then(function (t) {
                            console.log("validated!"),
                                "Valid" == t
                                    ? (e.preventDefault(),
                                        (r.disabled = !0),
                                    r.setAttribute("data-kt-indicator", "on"), 
                                    setTimeout(function () {  
                                        var formElement = document.querySelector("#kt_modal_create_app_form");
                                        var formData = new FormData(formElement);
                                         
                                        let projectSizes = {};
                                        document.querySelectorAll('input[name="ProjectMethod"]:checked').forEach(el => {
                                            formData.append('ProjectMethod', el.value);
                                            const sizeInput = document.querySelector(`input[name="${el.value}Size"]`);
                                            if (sizeInput) {
                                                projectSizes[el.value] = sizeInput.value;
                                            }
                                        });
                                        const otherOption = document.getElementById('other-option');
                                         
                                        if (otherOption.checked) { 
                                            if (formData.has('ProjectSector')) {
                                                formData.delete('ProjectSector'); // Önce mevcut veriyi sil
                                            }
                                            const inputElement = document.querySelector('input[name="ProjectSectorInput"]'); 
                                            const inputValue = inputElement.value;
                                            formData.append('ProjectSector', inputValue);
                                        }  
                                        formData.append('ProjectSizes', JSON.stringify(projectSizes));
                                         
                                        for (var pair of formData.entries()) {
                                            console.log(pair[0] + ': ' + pair[1]);
                                        }
                                        $.ajax({
                                            url: 'project-list/proje-olustur',  
                                            method: 'POST',
                                            data: formData,
                                            contentType: false,
                                            processData: false,
                                            success: function (response) {
                                                Swal.fire({
                                                    text: "Form başarıyla gönderildi.",
                                                    icon: "success",
                                                    buttonsStyling: false,
                                                    confirmButtonText: "Tamam",
                                                    customClass: {
                                                        confirmButton: "btn btn-light",
                                                    },
                                                }).then(function () {
                                                    KTUtil.scrollTop();
                                                    i.goNext();
                                                    location.reload(true);
                                                });
                                            },
                                            error: function (xhr, status, error) {
                                                let errorMessage = '';
                                                xhr.responseJSON.Errors.forEach(function (err) {
                                                    errorMessage += err + '\n';
                                                });
                                                Swal.fire({
                                                    text: errorMessage,
                                                    icon: "error",
                                                    buttonsStyling: false,
                                                    confirmButtonText: "Tamam",
                                                    customClass: {
                                                        confirmButton: "btn btn-light",
                                                    },
                                                }).then(function () {
                                                    KTUtil.scrollTop();
                                                });
                                            },
                                            complete: function () {
                                                r.removeAttribute("data-kt-indicator"),
                                                    r.disabled = !1;
                                            }
                                        });
                                    }, 2000))
                                    : Swal.fire({
                                        text: "Üzgünüz, bazı hatalar algılandı, lütfen tekrar deneyin.",
                                        icon: "error",
                                        buttonsStyling: !1,
                                        confirmButtonText: "Tamam",
                                        customClass: {
                                            confirmButton: "btn btn-light",
                                        },
                                    }).then(function () {
                                        KTUtil.scrollTop();
                                    });
                        });
                    }), 
                    n.push(
                        FormValidation.formValidation(o, {
                            fields: {
                                ProjectName: {
                                    validators: {
                                        notEmpty: {
                                            message: "Proje Kodu alanı zorunludur",
                                        },
                                    },
                                },
                                ProjectHeader: {
                                    validators: {
                                        notEmpty: {
                                            message: "Proje Adı alanı zorunludur",
                                        },
                                    },
                                },
                                CustomerName: {
                                    validators: {
                                        notEmpty: {
                                            message: "Müşteri Adı alanı zorunludur",
                                        },
                                    },
                                },
                            },
                            plugins: {
                                trigger: new FormValidation.plugins.Trigger(),
                                bootstrap: new FormValidation.plugins.Bootstrap5({
                                    rowSelector: ".fv-row",
                                    eleInvalidClass: "",
                                    eleValidClass: "",
                                }),
                            },
                        })
                    ),
                    n.push(
                        FormValidation.formValidation(o, {
                            fields: {
                                ProjectSector: {
                                    validators: {
                                        notEmpty: {
                                            message: "Proje Sektörü zorunludur",
                                        },
                                    },
                                },
                            },
                            plugins: {
                                trigger: new FormValidation.plugins.Trigger(),
                                bootstrap: new FormValidation.plugins.Bootstrap5({
                                    rowSelector: ".fv-row",
                                    eleInvalidClass: "",
                                    eleValidClass: "",
                                }),
                            },
                        })
                    ),
                    n.push(
                        FormValidation.formValidation(o, {
                            fields: {
                                ProjectMethod: {
                                    validators: {
                                        notEmpty: {
                                            message: "Proje Metodu zorunludur",
                                        },
                                    },
                                },

                            },
                            plugins: {
                                trigger: new FormValidation.plugins.Trigger(),
                                bootstrap: new FormValidation.plugins.Bootstrap5({
                                    rowSelector: ".fv-row",
                                    eleInvalidClass: "",
                                    eleValidClass: "",
                                }),
                            },
                        })
                    ),
                    n.push(
                        FormValidation.formValidation(o, {
                            fields: {
                                ScmId: {
                                    validators: {
                                        callback: {
                                            message: 'Lütfen en az 1 Saha Firması seçiniz',
                                            callback: function (input) {
                                                const options = scmField.select2('data');
                                                return options != null && options.length >= 1;
                                            },
                                        },
                                    },
                                },

                            },
                            plugins: {
                                trigger: new FormValidation.plugins.Trigger(),
                                bootstrap: new FormValidation.plugins.Bootstrap5({
                                    rowSelector: ".fv-row",
                                    eleInvalidClass: "",
                                    eleValidClass: "",
                                }),
                            },
                        })
                    )),
                    n.push(
                    FormValidation.formValidation(o, {

                        plugins: {
                            trigger: new FormValidation.plugins.Trigger(),
                            bootstrap: new FormValidation.plugins.Bootstrap5({
                                rowSelector: ".fv-row",
                                eleInvalidClass: "",
                                eleValidClass: "",
                            }),
                        },
                    })
                ),
                    n.push(
                    FormValidation.formValidation(o, {
                        fields: {
                            ProjectManager: {
                                validators: {
                                    callback: {
                                        message: 'Lütfen en az 1 ekip yöneticisi seçiniz',
                                        callback: function (input) {
                                            const options = usersField.select2('data');
                                            return options != null && options.length >= 1;
                                        },
                                    },
                                },
                            },
                            ProjectOfficers: {
                                validators: {
                                    callback: {
                                        message: 'Lütfen en az 1 ekip üyesi seçiniz',
                                        callback: function (input) {
                                            const options = usersField.select2('data');
                                            return options != null && options.length >= 1;
                                        },
                                    },
                                },
                            },

                        },
                        plugins: {
                            trigger: new FormValidation.plugins.Trigger(),
                            bootstrap: new FormValidation.plugins.Bootstrap5({
                                rowSelector: ".fv-row",
                                eleInvalidClass: "",
                                eleValidClass: "",
                            }),
                        },
                    })
                );

        },
    };
})();
KTUtil.onDOMContentLoaded(function () {
    KTCreateApp.init();
});