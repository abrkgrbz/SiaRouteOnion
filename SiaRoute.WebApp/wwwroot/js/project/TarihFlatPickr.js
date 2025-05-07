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

planlananElements.forEach((element) => {
    initDatePicker(element, "Lütfen bir tarih seçiniz");
});
 
