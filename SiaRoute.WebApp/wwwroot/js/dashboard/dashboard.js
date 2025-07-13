"use strict";

// Proje durumlarını gösteren AmCharts 5 grafiği
function initProjectStatusChart() {
    // Proje durumlarını API'den çek
    $.ajax({
        url: '/Dashboard/GetProjectStatusData',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log("API'den gelen veriler:", data);
            // Veri yapısını doğrula ve düzelt gerekirse
            data = normalizeData(data);
            // AmCharts 5 için verileri hazırla ve grafiği oluştur
            createStatusChart(data);
        },
        error: function (xhr, status, error) {
            console.error("Veri çekilirken hata oluştu:", error);
            if (typeof toastr !== 'undefined') {
                toastr.error("Proje durumları yüklenirken bir hata oluştu.");
            } else {
                alert("Proje durumları yüklenirken bir hata oluştu.");
            }
        }
    });
}

// Veri yapısını normalleştir (büyük/küçük harf sorunlarını çöz)
function normalizeData(data) {
    return data.map(item => {
        // Anahtar adlarını kontrol et ve normalleştir
        const normalizedItem = {};

        // Status veya status alanını normalleştir
        if (item.Status !== undefined) {
            normalizedItem.status = item.Status;
        } else if (item.status !== undefined) {
            normalizedItem.status = item.status;
        }

        // Count veya count alanını normalleştir
        if (item.Count !== undefined) {
            normalizedItem.count = item.Count;
        } else if (item.count !== undefined) {
            normalizedItem.count = item.count;
        }

        // Color veya color alanını normalleştir
        if (item.Color !== undefined) {
            normalizedItem.color = item.Color;
        } else if (item.color !== undefined) {
            normalizedItem.color = item.color;
        }

        // Varsa diğer alanları da ekle
        if (item.order !== undefined) normalizedItem.order = item.order;
        if (item.group !== undefined) normalizedItem.group = item.group;

        return normalizedItem;
    });
}

function createStatusChart(chartData) {
    console.log("Normalleştirilmiş veriler:", chartData);

    // Süreç grupları tanımlaması
    const processGroups = {
        "Başlangıç": ["OnayToplantisi", "SoruFormuPaylasimi", "ScriptYazimi"],
        "Hazırlık": ["ScriptOnayi", "KotaDosyasi", "SahaBaslangic"],
        "Saha": ["Sahada", "SahaBitis"],
        "Veri İşleme": ["Kodlama", "Tablolama"],
        "Analiz": ["MSAnalizleri", "EkIstekler", "Raporlama"]
    };

    // Her durumun hangi gruba ait olduğunu belirle
    let statusToGroup = {};
    Object.entries(processGroups).forEach(([group, statuses]) => {
        statuses.forEach(status => {
            statusToGroup[status] = group;
        });
    });

    // Her kayda grup bilgisi ekle - DÜZELTME: status alanı kullanılıyor
    chartData.forEach(item => {
        item.group = statusToGroup[item.status] || "Diğer";

        // Konsola bilgi yazdır
        console.log(`${item.status} => ${item.group}`);
    });

    // Grupları ID'lerine göre sırala - proje akışına göre doğru sıralama için
    const groupOrder = ["Başlangıç", "Hazırlık", "Saha", "Veri İşleme", "Analiz"];

    // Grup renklerini tanımla
    const groupColors = {
        "Başlangıç": "#8950FC",
        "Hazırlık": "#FFA800",
        "Saha": "#1BC5BD",
        "Veri İşleme": "#3699FF",
        "Analiz": "#F64E60"
    };

    // AmCharts 5 grafiğini oluştur
    am5.ready(function () {
        try {
            // Root element oluştur ve tema belirle
            let root = am5.Root.new("kt_amcharts_1");
            root.setThemes([am5themes_Animated.new(root)]);

            // Chart oluştur
            let chart = root.container.children.push(
                am5percent.PieChart.new(root, {
                    layout: root.verticalLayout,
                    innerRadius: am5.percent(40)
                })
            );

            // Verilerden donut grafiği oluştur
            let series = chart.series.push(
                am5percent.PieSeries.new(root, {
                    valueField: "count", // DÜZELTME: count alanı kullanılıyor
                    categoryField: "status", // DÜZELTME: status alanı kullanılıyor
                    alignLabels: false,
                    legendLabelText: "{category}",
                    legendValueText: "{value} [{value.percent.formatNumber('#.0')}%]"
                })
            );

            // Slices özelliklerini ayarla
            series.slices.template.setAll({
                strokeWidth: 2,
                stroke: am5.color(0xffffff),
                templateField: "sliceSettings"
            });

            // Hover state
            series.slices.template.states.create("hover", {
                scale: 1.05,
                fillOpacity: 0.9
            });

            // Label özelliklerini ayarla
            series.labels.template.setAll({
                textType: "adjusted",
                radius: 10,
                text: "{category}: {value}",
                fill: am5.color(0x000000),
                background: am5.RoundedRectangle.new(root, {
                    fill: am5.color(0xffffff),
                    fillOpacity: 0.7
                })
            });

            // Tooltip
            series.slices.template.set("tooltipText", "{category}: {value} [{value.percent.formatNumber('#.0')}%]");

            // Gruplandırılmış verileri oluştur
            let groupedData = [];
            let tempData = [...chartData]; // orijinal veriyi kopyala

            // Herbir grup için veri hazırla
            groupOrder.forEach(group => {
                // Bu gruba ait verileri bul
                let groupItems = tempData.filter(item => item.group === group);

                // Grup bilgisini konsola yazdır
                console.log(`${group} grubunda ${groupItems.length} öğe var`);

                // Herbir durum için slice özelliklerini belirle
                groupItems.forEach(item => {
                    item.sliceSettings = {
                        fill: am5.color(groupColors[item.group])
                    };
                });

                // Grup verilerini ekle
                groupedData.push(...groupItems);
            });

            // Verileri ata
            console.log("Grafik için hazırlanan veriler:", groupedData);
            series.data.setAll(groupedData);

            // Legend oluştur
            let legend = chart.children.push(
                am5.Legend.new(root, {
                    centerX: am5.p50,
                    x: am5.p50,
                    layout: root.verticalLayout,
                    height: am5.percent(30),
                    width: am5.percent(100),
                    verticalScrollbar: am5.Scrollbar.new(root, {
                        orientation: "vertical"
                    })
                })
            );

            // Legend için seri bağlantısı
            legend.data.setAll(series.dataItems);

            // Başlık ekle
            chart.children.unshift(
                am5.Label.new(root, {
                    text: "Proje Durumları",
                    fontSize: 22,
                    fontWeight: "500",
                    textAlign: "center",
                    x: am5.percent(50),
                    centerX: am5.percent(50),
                    paddingTop: 10,
                    paddingBottom: 10
                })
            );

            // Animasyon
            series.appear(1000, 100);

            // Dashboard sayaçlarını güncelle
            updateDashboardCounters(chartData);

        } catch (error) {
            console.error("Grafik oluşturulurken hata oluştu:", error);
        }
    });
}

// Dashboard sayaçlarını güncelle
function updateDashboardCounters(data) {
    console.log(data)
    try {
        // Toplam proje sayısını hesapla
        const totalProjects = data.reduce((sum, item) => sum + item.count, 0);
       
        // Sahada olan proje sayısını hesapla
        const fieldProjects = data.filter(item => item.status === "Sahada").reduce((sum, item) => sum + item.count, 0);

        // Planlama aşamasındaki projelerin sayısını hesapla
        const planningStatuses = ["OnayToplantisi", "SoruFormuPaylasimi", "ScriptYazimi", "ScriptOnayi", "KotaDosyasi", "SahaBaslangic"];
        const planningProjects = data
            .filter(item => planningStatuses.includes(item.status))
            .reduce((sum, item) => sum + item.count, 0);

        // Tamamlanan projelerin sayısını hesapla
        const completedProjects = Math.round(data
            .filter(item => item.status === "Raporlama")
            .reduce((sum, item) => sum + item.count, 0) / 3); 
        $('#active-projects-counter').attr('data-kt-countup-value', totalProjects);
        $('#field-projects-counter').attr('data-kt-countup-value', fieldProjects);
        $('#planning-projects-counter').attr('data-kt-countup-value', planningProjects);
        $('#completed-projects-counter').attr('data-kt-countup-value', completedProjects);

        // Sonra sayaçları başlat
        initCounters();
    } catch (error) {
        console.error("Sayaçlar güncellenirken hata oluştu:", error);

        // Hata durumunda basit bir şekilde değerleri güncelle
        $('#active-projects-counter').text(totalProjects);
        $('#field-projects-counter').text(fieldProjects);
        $('#planning-projects-counter').text(planningProjects);
        $('#completed-projects-counter').text(completedProjects);
    }
}

// Metronic tema içindeki KTCountUp özelliğini kullanarak sayaç başlatma
function initCounters() {
    try {
        $('[data-kt-countup="true"]').each(function () {
            var element = $(this);
            var value = element.attr('data-kt-countup-value');
            console.log(value)
            element.text(value);
        });
    } catch (error) {
        console.error("Sayaçlar başlatılırken hata oluştu:", error);

        // Hata durumunda sayıları doğrudan göster
        $('[data-kt-countup="true"]').each(function () {
            var element = $(this);
            var value = element.attr('data-kt-countup-value');
            element.text(value);
        });
    }
}


// Sayfa yüklendiğinde çalıştır
$(document).ready(function () {
    console.log("Sayfa yüklendi, chart başlatılıyor...");

    // AmCharts'ı başlat
    initProjectStatusChart();

    // Rapor İndir butonu için event listener
    $("#download-chart-report").on("click", function () {
        // AmCharts 5'in export fonksiyonunu kullanarak grafiği indir
        am5.ready(function () {
            try {
                // Root element'i bul
                let chart = document.getElementById("kt_amcharts_1")._amcharts_root;
                if (chart) {
                    // Export menüsünü göster veya doğrudan PNG olarak indir
                    am5.exportingMenu(chart, {
                        filePrefix: "proje-durumlari-raporu",
                        formats: ["png", "jpg", "pdf", "xlsx"]
                    });
                } else {
                    console.error("Grafik henüz yüklenmedi veya oluşturulamadı.");
                    alert("Grafik yüklenemedi. Lütfen sayfayı yenileyip tekrar deneyin.");
                }
            } catch (error) {
                console.error("Export işlemi sırasında hata oluştu:", error);
                alert("Rapor oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
            }
        });
    });
});