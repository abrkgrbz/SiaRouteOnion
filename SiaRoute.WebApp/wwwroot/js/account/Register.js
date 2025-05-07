var KTSignupGeneral = function () {
    var e, t, a, s, r = function () {
        return 100 === s.getScore()
    };
    return {
        init: function () {
            e = document.querySelector("#kt_sign_up_form"), t = document.querySelector("#kt_sign_up_submit"), s = KTPasswordMeter.getInstance(e.querySelector('[data-kt-password-meter="true"]')), a = FormValidation.formValidation(e, {
                fields: {
                    "firstname": {
                        validators: {
                            notEmpty: {
                                message: "Ad Alanı Zorunludur"
                            }
                        }
                    },
                    "lastname": {
                        validators: {
                            notEmpty: {
                                message: "Soyad Alanı Zorunludur"
                            }
                        }
                    },
                    "username": {
                        validators: {
                            notEmpty: {
                                message: "Kullanıcı Adı Alanı Zorunludur"
                            }
                        }
                    },
                    "email": {
                        validators: {
                            emailAddress: {
                                message: 'Geçerli Bir Email Adresi Giriniz'
                            },
                            notEmpty: {
                                message: "Email Alanı Zorunludur"
                            }
                        }
                    },
                    "password": {
                        validators: {
                            notEmpty: {
                                message: "Şifre Alanı Zorunludur"
                            },
                            callback: {
                                message: "Lütfen Kurallara Uygun Bir Şifre Giriniz",
                                callback: function (e) {
                                    if (e.value.length > 0) return r()
                                }
                            }
                        }
                    },
                    "confirmpassword": {
                        validators: {
                            notEmpty: {
                                message: "Şifre Onayı Zorunludur"
                            },
                            identical: {
                                compare: function () {
                                    return e.querySelector('[name="password"]').value
                                },
                                message: "Şifre ve Şifre Onayı aynı değil"
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger({
                        event: {
                            password: !1
                        }
                    }),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: ".fv-row",
                        eleInvalidClass: "",
                        eleValidClass: ""
                    }),

                }
            }), t.addEventListener("click", (function (r) {
                r.preventDefault(), a.revalidateField("password"), a.validate().then((function (a) {
                    "Valid" == a ? (t.setAttribute("data-kt-indicator", "on"), t.disabled = !0, setTimeout((function () {

                        setTimeout(function () {
                            $.ajax({
                                url: 'kayit-olustur',
                                type: "POST",
                                dataType: "json",
                                cache: false,
                                data: new FormData(
                                    document.getElementById(
                                        "kt_sign_up_form"
                                    )
                                ),
                                processData: false,
                                contentType: false,
                                success: function (data) {
                                    if (data.result == 200) {
                                        t.removeAttribute("data-kt-indicator"), t.disabled = !1,
                                            Swal.fire({
                                                text: data.message,
                                                icon: "success",
                                                buttonsStyling: !1,
                                                confirmButtonText: "Tamam!",
                                                customClass: {
                                                    confirmButton: "btn btn-primary",
                                                },
                                            }).then(function (t) {
                                                window.location.href = '/User/Login'
                                            });
                                    } else {
                                        console.log(data)
                                    }
                                   
                                },
                                error: function (data) {
                                    console.log(data)
                                    Swal.fire({
                                        text: data,
                                        icon: "error",
                                        buttonsStyling: !1,
                                        confirmButtonText: "Tamam!",
                                        customClass: {
                                            confirmButton: "btn btn-primary",
                                        },
                                    }).then(function (ta) {
                                        t.removeAttribute("data-kt-indicator"), t.disabled = !1
                                    });
                                }
                            });
                        });



                    }), 1500)) : Swal.fire({
                        text: "Üzgünüz, bazı hatalar algılandı, lütfen tekrar deneyin.",
                        icon: "error",
                        buttonsStyling: !1,
                        confirmButtonText: "Tamam!",
                        customClass: {
                            confirmButton: "btn btn-primary"
                        }
                    })
                }))
            })), e.querySelector('input[name="password"]').addEventListener("input", (function () {
                this.value.length > 0 && a.updateFieldStatus("password", "NotValidated")
            }))
        }
    }
}();
KTUtil.onDOMContentLoaded((function () {
    KTSignupGeneral.init()
}));