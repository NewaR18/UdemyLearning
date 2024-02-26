using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.JSModels
{
    public class DataTableAjaxModel
    {
        public string Status { get; set; } //Added Later for addition of Filter From 5 anchor buttons
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public List<ColumnFilterModel> Columns { get; set; }
    }
    public class ColumnFilterModel
    {
        public string Data { get; set; }
        public string Name { get; set; } // Optional
        public Search Search { get; set; }
    }
    public class Search
    {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }
}
