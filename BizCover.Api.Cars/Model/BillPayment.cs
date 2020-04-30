using BizCover.Repository.Cars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizCover.Api.Cars.Model
{
    public class BillPayment
    {
        public List<Car> Cars { get; set; }
        public decimal TotalPriceBeforeDisc { get; set; }
        public decimal TotalPriceAfterDisc { get; set; }
        public string Description { get; set; }
    }
}
