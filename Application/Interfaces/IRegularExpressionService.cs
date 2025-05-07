using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRegularExpressionService
    {
        string rexp_h1(string input);
        string rexp_h2(string input);
        string rexp_q(string input);
        string rexp_r(string input);
        string rexp_grd_d(string input);
        string rexp_grd_h1(string input);
        string rexp_grd_h2(string input);
        string rexp_grd_cr(string input);
        string rexp_grd_head(string input);
        List<string> rexp_grd_r(string input);
        string rexp_s_h1(string input);
        string rexp_s_h2(string input);
        List<string> rexp_s_r(string input);
    }
}
