using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class NewProduct
    {
        public string Name { get; set; }
        public float Energy { get; set; }
        public float? Fat { get; set; }
        public float? Saturates { get; set; }
        public float? Carbohydrate { get; set; }
        public float? Sugars { get; set; }
        public float? Fibre { get; set; }
        public float? Protein { get; set; }
        public float? Salt { get; set; }
    }
}
