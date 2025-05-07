"use strict";
var KTSignupGeneral = function () {
    var e, t, a, s, r = function () {
        return 100 === s.getScore()
    };
    return {
        init: function () {
            e = document.querySelector("#kt_sign_up_form"), t = document.querySelector("#kt_sign_up_submit"), s = KTPasswordMeter.getInstance(e.querySelector('[data-kt-password-meter="true"]')), a = FormValidation.formValidation(e, {
                fields: {
                    "first-name": {
                        validators: {
                            notEmpty: {
                                message: "Ad Alaný Zorunludur"
                            }
                        }
                    },
                    "last-name": {
                        validators: {
                            notEmpty: {
                                message: "Soyad Alaný Zorunludur"
                            }
                        }
                    } 
                    password: {
                        validators: {
                            notEmpty: {
                                message: "Þifre Alaný Zorunludur"
                            },
                            callback: {
                                message: "Lütfen Kurallara Uygun Bir Þifre Giriniz",
                                callback: function (e) {
                                    if (e.value.length > 0) return r()
                                }
                            }
                        }
                    },
                    "confirm-password": {
                        validators: {
                            notEmpty: {
                                message: "Þifre Onayý Zorunludur"
                            },
                            identical: {
                                compare: function () {
                                    return e.querySelector('[name="password"]').value
                                },
                                message: "Þifre ve Þifre Onayý ayný deðil"
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
                    })
                }
            }), t.addEventListener("click", (function (r) {
                r.preventDefault(), a.revalidateField("password"), a.validate().then((function (a) {
                    "Valid" == a ? (t.setAttribute("data-kt-indicator", "on"), t.disabled = !0, setTimeout((function () {
                        t.removeAttribute("data-kt-indicator"), t.disabled = !1, Swal.fire({
                            text: "Kullanýcý Baþarýyla Oluþturuldu! Giriþ Yapmak için lütfen Admin onayý bekleyiniz!",
                            icon: "success",
                            buttonsStyling: !1,
                            confirmButtonText: "Tamam!",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            }
                        }).then((function (t) {
                            t.isConfirmed && (e.reset(), s.reset())
                        }))
                    }), 1500)) : Swal.fire({
                        text: "Üzgünüz, bazý hatalar algýlandý, lütfen tekrar deneyin.",
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