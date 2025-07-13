using Application.Enums;
using Application.Features.Queries.Dashboard.GetProjectStatusAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace SiaRoute.WebApp.Controllers
{
    public class DashboardController : BaseController
    {
       
        [HttpGet]
        public async Task<IActionResult> GetProjectStatusData()
        {
            var query = new GetProjectStatusAnalysisQuery();
            var result = await Mediator.Send(query); 
            var orderedResult = result.OrderBy(x => GetStatusOrder((ProjectStatus)x.Status)).ToList();
             
            var chartData = orderedResult.Select(x => new
            {
                Status = ((ProjectStatus)x.Status).ToString(),
                Count = x.Adet,
                Color = GetStatusColor(x.Status),
                Order = GetStatusOrder((ProjectStatus)x.Status)
            }).ToList();

            return Json(chartData);
        }
         
        [HttpGet]
        public async Task<IActionResult> GetProjectStatusGroupData()
        {
            var query = new GetProjectStatusAnalysisQuery();
            var result = await Mediator.Send(query);

            // Proje aşamalarını gruplara ayır
            var groupedData = result.Select(x => new
            {
                Status = ((ProjectStatus)x.Status).ToString(),
                Count = x.Adet,
                Group = GetProjectGroup(x.Status),
                Color = GetStatusColor(x.Status),
                Order = GetGroupOrder(GetProjectGroup(x.Status))
            })
            .GroupBy(x => x.Group)
            .Select(g => new
            {
                Group = g.Key,
                Count = g.Sum(x => x.Count),
                Order = g.First().Order,
                Details = g.Select(x => new
                {
                    Status = x.Status,
                    Count = x.Count,
                    Color = x.Color
                }).ToList()
            })
            .OrderBy(g => g.Order)
            .ToList();

            return Json(groupedData);
        }
         
        [HttpGet]
        public async Task<IActionResult> GetProjectStatusStatistics()
        {
            var query = new GetProjectStatusAnalysisQuery();
            var result = await Mediator.Send(query);

            // Gruplandırma yap
            var groupData = result.Select(x => new
            {
                Status = ((ProjectStatus)x.Status).ToString(),
                Count = x.Adet,
                Group = GetProjectGroup(x.Status)
            })
            .GroupBy(x => x.Group)
            .Select(g => new
            {
                Group = g.Key,
                Count = g.Sum(x => x.Count),
                Percentage = 0.0 // Sonradan hesaplanacak
            })
            .ToList();

            // Toplam proje sayısını bul
            int totalProjects = groupData.Sum(g => g.Count);

            // Yüzdelikleri hesapla
            var statistics = groupData.Select(g => new
            {
                g.Group,
                g.Count,
                Percentage = (double)g.Count / totalProjects * 100
            }).ToList();

            // Dashboard için önemli istatistikler
            var dashboardStats = new
            {
                TotalProjects = totalProjects,
                ActiveProjects = result.Sum(x => x.Adet),
                FieldProjects = result.Where(x => (ProjectStatus)x.Status == ProjectStatus.Sahada).Sum(x => x.Adet),
                PlanningProjects = result.Where(x => (ProjectStatus)x.Status == ProjectStatus.OnayToplantisi
                                                 || (ProjectStatus)x.Status == ProjectStatus.SoruFormuPaylasimi
                                                 || (ProjectStatus)x.Status == ProjectStatus.ScriptYazimi
                                                 || (ProjectStatus)x.Status == ProjectStatus.ScriptOnayi
                                                 || (ProjectStatus)x.Status == ProjectStatus.KotaDosyasi
                                                 || (ProjectStatus)x.Status == ProjectStatus.SahaBaslangic)
                                       .Sum(x => x.Adet),
                CompletedProjects = 0 // Bu veri başka bir kaynaktan gelebilir
            };

            return Json(new
            {
                Statistics = statistics,
                DashboardStats = dashboardStats
            });
        }

        private string GetStatusColor(int status)
        { 
            switch ((ProjectStatus)status)
            {
                case ProjectStatus.OnayToplantisi:
                    return "#8950FC"; // Koyu Mor
                case ProjectStatus.SoruFormuPaylasimi:
                    return "#A169FD"; // Orta Mor
                case ProjectStatus.ScriptYazimi:
                    return "#B98AFE"; // Açık Mor

                case ProjectStatus.ScriptOnayi:
                    return "#FFA800"; // Koyu Turuncu
                case ProjectStatus.KotaDosyasi:
                    return "#FFB739"; // Orta Turuncu
                case ProjectStatus.SahaBaslangic:
                    return "#FFC772"; // Açık Turuncu

                case ProjectStatus.Sahada:
                    return "#1BC5BD"; // Yeşil
                case ProjectStatus.SahaBitis:
                    return "#40D9D2"; // Açık Yeşil

                case ProjectStatus.Kodlama:
                    return "#3699FF"; // Koyu Mavi
                case ProjectStatus.Tablolama:
                    return "#69B3FF"; // Açık Mavi

                case ProjectStatus.MSAnalizleri:
                    return "#F64E60"; // Koyu Kırmızı
                case ProjectStatus.EkIstekler:
                    return "#F77685"; // Orta Kırmızı
                case ProjectStatus.Raporlama:
                    return "#F99EAA"; // Açık Kırmızı

                default:
                    return "#B5B5C3"; // Gri - Diğer
            }
        }

        private string GetProjectGroup(int status)
        {
            
            switch ((ProjectStatus)status)
            {
                case ProjectStatus.OnayToplantisi:
                case ProjectStatus.SoruFormuPaylasimi:
                case ProjectStatus.ScriptYazimi:
                    return "Başlangıç";

                case ProjectStatus.ScriptOnayi:
                case ProjectStatus.KotaDosyasi:
                case ProjectStatus.SahaBaslangic:
                    return "Hazırlık";

                case ProjectStatus.Sahada:
                case ProjectStatus.SahaBitis:
                    return "Saha";

                case ProjectStatus.Kodlama:
                case ProjectStatus.Tablolama:
                    return "Veri İşleme";

                case ProjectStatus.MSAnalizleri:
                case ProjectStatus.EkIstekler:
                case ProjectStatus.Raporlama:
                    return "Analiz";

                default:
                    return "Diğer";
            }
        }

        private int GetStatusOrder(ProjectStatus status)
        {
           
            return (int)status;
        }

        private int GetGroupOrder(string group)
        {
         
            switch (group)
            {
                case "Başlangıç": return 1;
                case "Hazırlık": return 2;
                case "Saha": return 3;
                case "Veri İşleme": return 4;
                case "Analiz": return 5;
                default: return 99;
            }
        }
    }
}
