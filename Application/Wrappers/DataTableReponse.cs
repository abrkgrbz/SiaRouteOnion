using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Wrappers
{
    public class DataTableReponse<T> : Response<T>
    {
        public int recordsFiltered { get; set; }
        public int recordTotal { get; set; }

        public DataTableReponse(T data, int recordsFiltered, int recordTotal) : base(data)
        {
            this.recordsFiltered = recordsFiltered;
            this.recordTotal = recordTotal;
            this.Succeeded = true;
            this.Data = data;
        }
    }
}
