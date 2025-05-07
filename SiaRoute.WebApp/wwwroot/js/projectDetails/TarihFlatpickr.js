function initDatePicker(selector, placeholderText) {
    selector.flatpickr({
        enableTime: false,
        dateFormat: "d-m-Y",
        locale: "tr"
    });
    if (!selector.val()) {
        selector.attr("placeholder", placeholderText);
    }
}

const planlananElements = [
    $("#planlanan_scrpt_teslim"),
    $("#planlanan_scrpt_kontrol"),
    $("#planlanan_scrpt_revizyon"),
    $("#planlanan_saha_baslangic"),
    $("#planlanan_saha_bitis"),
    $("#planlanan_kodlama_teslim"), 
    $("#planlanan_tablolama_teslim"),
    $("#planlanan_soruformu_teslim"),
    $("#planlanan_rapor_teslim")
];

const gerceklesenElements = [
    $("#gerceklesen_scrpt_teslim"),
    $("#gerceklesen_scrpt_kontrol"),
    $("#gerceklesen_scrpt_revizyon"),
    $("#gerceklesen_saha_baslangic"),
    $("#gerceklesen_saha_bitis"),
    $("#gerceklesen_kodlama_teslim"),
    $("#gerceklesen_tablolama_teslim"),
    $("#gerceklesen_soruformu_teslim"),
    $("#gerceklesen_rapor_teslim")
];

planlananElements.forEach((element) => {
    initDatePicker(element, "Lütfen bir tarih seçiniz");
});

gerceklesenElements.forEach((element) => {
    initDatePicker(element, "Lütfen bir tarih seçiniz");
});

