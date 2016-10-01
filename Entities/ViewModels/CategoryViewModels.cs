using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class CategoryToDropDownGroup
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? CategoryId { get; set; }
        public long? CategoryTypeId { get; set; }
        public string CategoryTypeName { get; set; }
        public int? OrdinalNumber { get; set; }
    }
}
