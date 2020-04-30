using System;
using System.Collections.Generic;
using BizCover.Repository.Cars;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BizCover.Api.Cars.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;

namespace BizCover.Api.Cars.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    [Authorize(Roles ="Administrator")]
    public class CarsController : ControllerBase
    {
        // Below is just the sample code from the Visual Studio Web Api Template. 
        // Feel free to replace this with whatever implementation you feel is suitable and production ready for a web api.

        // The repository BizCover.Repository.Cars can be found in ../packages/BizCover.Repository.Cars.1.0.0/BizCover.Repository.Cars.dll. You can restructure this solution as you like.

        private readonly ICarRepository _carRepository;

        public CarsController(ICarRepository carRespository)
        {
            _carRepository = carRespository?? throw new ArgumentNullException(nameof(carRespository));
        }

        #region AddCar
        [HttpPost]
        [Route("AddCar")]
        public IActionResult AddCar(Car nCar)
        {
            try
            {
                var taskRs = _carRepository.Add(nCar);

                if (taskRs.Exception != null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, taskRs.Exception.InnerException.Message);
                }
                return Ok(nCar);
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        #endregion

        #region UpdateCar
        [HttpPost]
        [Route("UpdateCar")]
        public IActionResult UpdateCar(Car nCar)
        {
            try
            {
                if (isExist(nCar))
                {
                    var taskRs = _carRepository.Update(nCar);

                    if (taskRs.Exception != null)
                    {
                        return StatusCode((int)HttpStatusCode.BadRequest, taskRs.Exception.InnerException.Message);
                    }
                }
                else
                {
                    return NotFound();
                }
                return Ok(nCar);
            }catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        #endregion

        #region GetCar
        [HttpGet]
        [Route("Car/{id}")]
        public IActionResult GetCar(int id)
        {
            try
            {
                var car = _carRepository.GetAllCars().Result.Where(x=>x.Id==id).FirstOrDefault();
                if (car == null)
                {
                    return NotFound();
                }

                return Ok(car);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        #endregion

        #region GetAllCars
        [HttpGet]
        [Route("CarList")]
        public IActionResult GetAllCars()
        {
            try
            {
                var cars = _carRepository.GetAllCars().Result;
                if (cars.Count == 0)
                {
                    return NotFound();
                }

                return Ok(cars);
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        #endregion

        #region Calculation
        [HttpPost]
        [Route("Calculation")]
        public ActionResult<BillPayment> Calculation(CalculationRq calRq)
        {
            try
            {
                #region Sanity check
                if (calRq == null || calRq.Cars == null || calRq.Cars.Count==0) return StatusCode((int)HttpStatusCode.BadRequest,"Please supply a valid car. Its empty");
                #endregion

                //Populate data car
                List<int> carsId = calRq.Cars.Select(x=>x.Id).ToList();
                calRq.Cars = _carRepository.GetAllCars().Result.Where(x => carsId.Contains(x.Id)).ToList();
                                

                var rs = doCalculation(calRq);
                if (rs == null)
                {
                    return NotFound();
                }

                return rs;
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        #endregion

        #region Method
        public BillPayment doCalculation(CalculationRq calRq)
        {
            #region Local Variable
            int disc = 0;
            decimal totalPrice = 0;
            decimal finalPrice = 0;
            List<string> desc = null;
            int numberOldCard = 0;
            decimal exceeds = 100000;
            BillPayment result = null;
            #endregion

            try
            {
                result = new BillPayment();
                desc = new List<string>();
                List<Car> cars = calRq.Cars;

                #region 1. cars year is before 2000, apply 10% discount
                foreach (var car in cars)
                {
                    if (car.Year < 2000)
                    {
                        car.Price = car.Price - (car.Price * Convert.ToDecimal(0.1));
                        numberOldCard++;
                    }
                    totalPrice += car.Price;

                    result.TotalPriceBeforeDisc += car.Price;
                }
                if (numberOldCard > 0) desc.Add(String.Format("have {0} car production year before 20000, get disc 10%", numberOldCard));
                #endregion

                #region 2. Total cost exceeds $100,000 apply 5% discount
                if (totalPrice > exceeds)
                {
                    disc = 5;
                    desc.Add(String.Format("total cost ${0}, exceeds $100,000, get disc 5% ", totalPrice));
                }
                #endregion

                #region 3. Number of cars is more than 2, apply 3% discount
                if (cars.Count() > 2)
                {
                    disc += 3;

                    desc.Add(String.Format("number of cars is {0}, more than 2,  get disc 3% ", cars.Count()));
                }
                #endregion

                finalPrice = totalPrice - (totalPrice * disc / 100);

                result.TotalPriceAfterDisc = finalPrice;
                //result.Cars = cars;
                result.Description = string.Join("; ", desc);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        public bool isExist(Car nCar)
        {
            bool blnResult = false;
            
             var taskRs = _carRepository.GetAllCars();
            if (taskRs.Exception != null)
            {
                throw new Exception(taskRs.Exception.InnerException.Message);
            }

            if (taskRs.Result.Count > 0)
            {
                if (taskRs.Result.Where(x => x.Id == nCar.Id).Count() > 0)
                {
                    blnResult = true;
                }                
            }
            
            return blnResult;
        }
        #endregion
    }
}
