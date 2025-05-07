using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Shared.Services
{
    public class RegularExpressionService:IRegularExpressionService
    {
        public string rexp_h1(string input)
        {
            var regex = new Regex(@"(?<=\[Header 1]:)([^\]]*)(?=(\[Header 2]:|\[Question]:|\[Response Options]:|(===========================\r\n)))"); 
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        public string rexp_h2(string input)
        {
            var regex = new Regex(@"(?<=\[Header 2]:)([^\]]*)(?=(\[Question]:|\[Response Options]:|(===========================\r\n)))");
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        public string rexp_q(string input)
        {
            var regex = new Regex(@"(?<=\[Question]:)([^\]]*)(?=\[Response Options]:|(===========================\r\n))");
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        public string rexp_r(string input)
        {
            var regex = new Regex(@"(?<=\r\n)([0-9]+\t.*\r\n)+(?=\r\n)");
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        public string rexp_grd_d(string input)
        {
            var regex = new Regex(@"(?<=Question Direction: )Rows|Columns(?=\r\n)");
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        public string rexp_grd_h1(string input)
        {
            var regex = new Regex(@"(?<=\[Header 1]:)([^\]]*)(?=(\[Header 2]:|\[Corner Label]:|\[Grid Header]:|\[Row List]:))");
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        public string rexp_grd_h2(string input)
        {
            var regex = new Regex(@"(?<=\[Header 2]:)([^\]]*)(?=(\[Corner Label]:|\[Grid Header]:|\[Row List]:))");
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        public string rexp_grd_cr(string input)
        {
            var regex = new Regex(@"(?<=\[Corner Label]:)([^\]]*)(?=(\[Grid Header]:|\[Row List]:))");
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        public string rexp_grd_head(string input)
        {
            var regex = new Regex(@"(?<=\[Grid Header]:)([^\]]*)(?=(\[Row List]:))");
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        public List<string> rexp_grd_r(string input)
        {
            var regex = new Regex(@"(?<=\r\n)([0-9]+\t.*\r\n)+", RegexOptions.Multiline);
            var matches = regex.Matches(input);
            var results = new List<string>();

            foreach (Match match in matches)
            {
                results.Add(match.Value);
            }

            return results;
        }

        public string rexp_s_h1(string input)
        {
            var regex = new Regex(@"(?<=\[Header 1]:)([^\]]*)(?=(\[Header 2]:|\[Left-Side List]:))");
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        public string rexp_s_h2(string input)
        {
            var regex = new Regex(@"(?<=\[Header 2]:)([^\]]*)(?=(\[Left-Side List]:))");
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        public List<string> rexp_s_r(string input)
        {
            var regex = new Regex(@"(?<=\r\n)([0-9]+\t.*\r\n)+(?=\r\n)", RegexOptions.Multiline);
            var matches = regex.Matches(input);
            var results = new List<string>();

            foreach (Match match in matches)
            {
                results.Add(match.Value);
            }

            return results;
        }
    }
}
