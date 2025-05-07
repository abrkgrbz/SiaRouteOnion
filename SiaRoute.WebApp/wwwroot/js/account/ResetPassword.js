"use strict";
var KTAccountSettingsSigninMethods = {
    init: function () {
        var t, e;
        ! function () {
            var t = document.getElementById("kt_signin_email"),
                o = document.getElementById("kt_signin_password_edit"),
                r = document.getElementById("kt_signin_password_button"),
                n = document.getElementById("kt_signin_password"),
                a = document.getElementById("kt_password_cancel");


            r.querySelector("button").addEventListener("click", (function () {
                d()
            })),
                a.addEventListener("click", (function () {
                    d()
                }));
            var l = function () {
                t.classList.toggle("d-none"), i.classList.toggle("d-none"), e.classList.toggle("d-none")
            },
                d = function () {
                    n.classList.toggle("d-none"), r.classList.toggle("d-none"), o.classList.toggle("d-none")
                }
        }(),
            function (t) {
                var e, n = document.getElementById("kt_signin_change_password");
                e = FormValidation.formValidation(n, {
                    fields: {
                        currentPassword: {
                            validators: {
                                notEmpty: {
                                    message: "Mevcut şifre alanı gereklidir"
                                }
                            }
                        },
                        password: {
                            validators: {
                                notEmpty: {
                                    message: "Yeni şifre alanı gereklidir"
                                }
                            }
                        },
                        confirmPassword: {
                            validators: {
                                notEmpty: {
                                    message: "Yeni şifre onay alanı gereklidir"
                                },
                                identical: {
                                    compare: function () {
                                        return n.querySelector('[name="password"]').value
                                    },
                                    message: "Şifreniz eşleşmiyor!"
                                }
                            }
                        }
                    },
                    plugins: {
                        trigger: new FormValidation.plugins.Trigger,
                        bootstrap: new FormValidation.plugins.Bootstrap5({
                            rowSelector: ".fv-row"
                        })
                    }
                });

                n.querySelector("#kt_password_submit").addEventListener("click", (function (t) {
                    t.preventDefault(),
                        e.validate().then((function (t) {
                            if (t == "Valid") {
                                var formData = {
                                    currentPassword: n.querySelector('[name="currentPassword"]').value,
                                    password: n.querySelector('[name="password"]').value,
                                    confirmPassword: n.querySelector('[name="confirmPassword"]').value
                                };
                                 
                                $.ajax({
                                    url: 'change-password',  
                                    type: 'POST',
                                    data: JSON.stringify(formData),
                                    contentType: 'application/json',
                                    success: function (response) {
                                        Swal.fire({
                                            text: "Şifreniz değiştirildi",
                                            icon: "success",
                                            buttonsStyling: !1,
                                            confirmButtonText: "Tamam",
                                            customClass: {
                                                confirmButton: "btn font-weight-bold btn-light-primary"
                                            }
                                        }).then(function (result) {
                                            if (result.isConfirmed) {
                                                n.reset(), e.resetForm();
                                            }
                                        });
                                    },
                                    error: function (xhr, status, error) {
                                        Swal.fire({
                                            text: "Şifre değişimi sırasında hata oluştu. Lütfen tekrar deneyiniz.",
                                            icon: "error",
                                            buttonsStyling: !1,
                                            confirmButtonText: "Tamam",
                                            customClass: {
                                                confirmButton: "btn font-weight-bold btn-light-primary"
                                            }
                                        });
                                    }
                                });
                            } else {
                                Swal.fire({
                                    text: "Lütfen gerekli alanları doldurunuz.",
                                    icon: "error",
                                    buttonsStyling: !1,
                                    confirmButtonText: "Tamam",
                                    customClass: {
                                        confirmButton: "btn font-weight-bold btn-light-primary"
                                    }
                                });
                            }
                        }));
                }));
            }()
    }
};

KTUtil.onDOMContentLoaded((function () {
    KTAccountSettingsSigninMethods.init()
}));
