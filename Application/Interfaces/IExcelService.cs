using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;  

namespace Application.Interfaces
{
    public interface IExcelService
    {
        Task<(string filePath,string pCode,string fileName)> UploadFile(IFormFile file, string target);
        Task DownloadExcelFile(string projectCode);
    }
}
