using BizCover.Repository.Cars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizCover.Api.Cars.Model
{
    public class CalculationRq
    {
        public List<Car> Cars { get; set; }
    }
}
