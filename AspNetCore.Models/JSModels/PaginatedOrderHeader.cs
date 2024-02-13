using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.JSModels
{
    public class PaginatedOrderHeader
    {
        public IEnumerable<OrderHeader> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
