using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Shared.Services
{
    public class ExcelService : IExcelService
    {
        private IPrintStudyService _projectCode;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        public ExcelService(IPrintStudyService projectCode)
        {
            _projectCode = projectCode;
        }

        #region Old

        //public async Task<(string filePath, string pCode,string fileName)> UploadFile(IFormFile file,string target)
        //{
        //    await _semaphore.WaitAsync();
        //    try
        //    {
        //        string filePath= string.Empty;
        //        string pCode = string.Empty; 
        //        if (!Directory.Exists(target))
        //        {
        //            Directory.CreateDirectory(target);
        //        }
        //        var strFileName = Path.GetFileNameWithoutExtension(file.FileName);
        //        var extension = Path.GetExtension(file.FileName);
        //        var strExtension = extension.ToLower().Trim();
        //        var fPath = Path.Combine(target, $"{strFileName}_{DateTime.Now:yyyyMMddHHmmss}{strExtension}");

        //        using (var fileStream = new FileStream(fPath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(fileStream);

        //        }

        //        using (var sr = new StreamReader(fPath))
        //        {
        //            var fContent = await sr.ReadToEndAsync();
        //            var rexp_study = new Regex(@"(?<=Study Name: )[a-zA-Z0-9]*(?=\r\n)");
        //            var projectCode = rexp_study.Match(fContent).ToString();

        //            if (!string.IsNullOrEmpty(projectCode))
        //            {
        //                _projectCode.SetProjectName(projectCode);
        //                _projectCode.SetPrintStudySetFpath(fPath);
        //                filePath = fPath;
        //                pCode = projectCode;
        //            }

        //            sr.Close();
        //        }
        //        return (filePath, pCode,strFileName);
        //    }
        //    finally
        //    {
        //        _semaphore.Release();
        //    }
        //}

        #endregion

        public async Task<(string filePath, string pCode, string fileName)> UploadFile(IFormFile file, string target)
        {
            await _semaphore.WaitAsync();
            try
            {
                string filePath = string.Empty;
                string pCode = string.Empty;

                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }

                var strFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                var strExtension = extension.ToLower().Trim();
                var fPath = Path.Combine(target, $"{strFileName}_{DateTime.Now:yyyyMMddHHmmss}{strExtension}");

                // Dosyayı geçici olarak hedefe yaz
                using (var fileStream = new FileStream(fPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Dosya içeriğini oku ve proje adını çıkar
                var fContent = await System.IO.File.ReadAllTextAsync(fPath);
                var rexp_study = new Regex(@"(?<=Study Name: )[a-zA-Z0-9]*(?=\r\n)");
                var projectCode = rexp_study.Match(fContent).ToString();

                if (!string.IsNullOrEmpty(projectCode))
                {
                    _projectCode.SetProjectName(projectCode);
                    var projectFolder = Path.Combine(target, projectCode);

                    // Proje adında klasör oluştur
                    if (!Directory.Exists(projectFolder))
                    {
                        Directory.CreateDirectory(projectFolder);
                    }

                    // Dosyayı yeni proje klasörüne taşı
                    var newFilePath = Path.Combine(projectFolder, $"{strFileName}_{DateTime.Now:yyyyMMddHHmmss}{strExtension}");
                    System.IO.File.Move(fPath, newFilePath);
                    if (System.IO.File.Exists(fPath))
                    {
                        System.IO.File.Delete(fPath);
                    }
                    _projectCode.SetPrintStudySetFpath(newFilePath);
                    filePath = newFilePath;
                    pCode = projectCode;
                }

                return (filePath, pCode, strFileName);
            }
            catch (Exception e)
            {
                throw new ApiException("Dosya yükleme sırasında hata meydana geldi!");
            }
            finally
            {
                _semaphore.Release();
            }

        }

        public async Task DownloadExcelFile(string projectCode)
        {
            throw new NotImplementedException();
        }
    }
}
