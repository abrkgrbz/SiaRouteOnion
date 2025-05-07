"use strict"; var KTSigninGeneral = function () {
    var t, e, i; return {
        init: function () {
            t =
                document.querySelector("#kt_sign_in_form"), e = document.querySelector("#kt_sign_in_submit"), i =
                FormValidation.formValidation(t, {
                    fields: {
                        Username: {
                            validators: {
                                notEmpty: { message: "Kullanici Adi Alani Zorunludur" },

                            }
                        }, Password: {
                            validators: {
                                notEmpty: {
                                    message:
                                        "Sifre Alani Zorunludur"
                                }
                            }
                        }
                    }, plugins: {
                        trigger: new FormValidation.plugins.Trigger, bootstrap: new
                            FormValidation.plugins.Bootstrap5({ rowSelector: ".fv-row" })
                    }
                }), e.addEventListener("click", (function (n) {
                    n.preventDefault(), i.validate().then((function (i) {
                        "Valid" == i ? (e.setAttribute("data-kt-indicator", "on"),
                            e.disabled = !0, setTimeout((function () {
                                     
                                    setTimeout(function () {
                                        $.ajax({
                                            url: '~/User/loginUser',
                                            type: 'POST',
                                            dataType: 'json',
                                            cache: false,
                                            data: new FormData(document.getElementById("kt_sign_in_form")),
                                            processData: false,
                                            contentType: false,
                                            success: function (data) { 
                                                e.removeAttribute("data-kt-indicator"), e.disabled = !1, 
                                                Swal.fire({
                                                    text:
                                                        "Başarıyla giriş yaptınız!", icon: "success", buttonsStyling: !1, confirmButtonText: "Tamam", customClass:
                                                        { confirmButton: "btn btn-primary" }
                                                }).then((function (e) {
                                                    if (e.isConfirmed) {
                                                        window.location.href = '/';
                                                        t.querySelector('[name="email"]').value = "", t.querySelector('[name="password"]').value = ""; var i =
                                                            t.getAttribute("data-kt-redirect-url"); i && (location.href = i)
                                                    }
                                                }))
                                            },
                                            error: function (data) {
                                                e.removeAttribute("data-kt-indicator"), e.disabled = !1, 
                                                Swal.fire({
                                                    text: "Üzgünüz, Kullanıcı Adı veya Şifre hatalı, lütfen tekrar deneyin.", icon: "error", buttonsStyling: !1, confirmButtonText: "Tamam",
                                                    customClass: { confirmButton: "btn btn-primary" }
                                                })
                                            }
                                        });
                                      
                                    })
                                 
                            }), 2e3)) : Swal.fire({
                                text: "Üzgünüz, bazı hatalar tespit edilmiş gibi görünüyor, lütfen tekrar deneyin.", icon: "error", buttonsStyling: !1, confirmButtonText: "Tamam",
                                customClass: { confirmButton: "btn btn-primary" }
                            })
                    }))
                }))
        }
    }
}(); KTUtil.onDOMContentLoaded((function () {
    KTSigninGeneral.init()
}));