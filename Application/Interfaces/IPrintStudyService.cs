using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPrintStudyService
    {
        string ProjectName { get; set; }
        string PrintStudySetFpath { get; set; }
        void SetProjectName(string projectName);
        void SetPrintStudySetFpath(string path);
        (List<Question> questions, List<Response> responses) GetQuestionResponseList(string fpath);
 
    }
}
