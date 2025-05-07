using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.SSIServices
{
    public interface ISSIDataService
    { 
   
        Task<List<Dictionary<string, object>>> ConvertDataToDataTable(int projectId, string projectName);
        DataTable ConvertListToDataTable(List<Dictionary<string, object>> list);
        Task<List<Dictionary<string, int>>> ProjectStatusDetails(string projectName);
    }
}
