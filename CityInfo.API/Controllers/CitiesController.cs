using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet()]
        public IActionResult GetCities()
        {
            var cities = _cityInfoRepository.GetCities();

            var results = cities.Select(c => new CityWithoutPointsOfInterestDto()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });
             
            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);
            if(city == null)
            {
                return NotFound();
            }
            
            if (includePointsOfInterest)
            {
                var cityResult = new CityDto()
                {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description
                };

                cityResult.PointsOfInterest = city.PointsOfInterest.Select(poi => new PointOfInterestDto()
                {
                    Id = poi.Id,
                    Name = poi.Name,
                    Description = poi.Description
                }).ToList();
                
                return Ok(cityResult);
            }

            var cityWithoutPointsOfInterestResult = new CityWithoutPointsOfInterestDto()
            {
                Id = city.Id,
                Name = city.Name,
                Description = city.Description
            };

            return Ok(cityWithoutPointsOfInterestResult);
        }
    }
}
