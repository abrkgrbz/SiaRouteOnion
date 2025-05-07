using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProjectProcess:BaseEntity
    {

        public DateTime? PlanlananScriptTeslim { get; set; }
        public DateTime? PlanlananScriptKontrol { get; set; }
        public DateTime? PlanlananScriptRevizyon { get; set; }
        public DateTime? PlanlananSahaBaslangic { get; set; }
        public DateTime? PlanlananSahaBitis { get; set; }
        public DateTime? PlanlananKodlamaTeslim { get; set; }
        public DateTime? PlanlananTablolamaTeslim { get; set; }
        public DateTime? PlanlananSoruFormuTeslim { get; set; }
        public DateTime? PlanlananRaporTeslim { get; set; }
         
        public DateTime? GerceklesenScriptTeslim { get; set; }
        public DateTime? GerceklesenScriptKontrol { get; set; }
        public DateTime? GerceklesenScriptRevizyon { get; set; }
        public DateTime? GerceklesenSahaBaslangic { get; set; }
        public DateTime? GerceklesenSahaBitis { get; set; }
        public DateTime? GerceklesenKodlamaTeslim { get; set; }
        public DateTime? GerceklesenTablolamaTeslim { get; set; }
        public DateTime? GerceklesenSoruFormuTeslim { get; set; }
        public DateTime? GerceklesenRaporTeslim { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

    }
}
