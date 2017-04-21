using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            try
            {
                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} was not found");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception)
            {
                _logger.LogInformation($"Exception while getting poi for city with id {cityId}");
                return StatusCode(500, "Problem when handling request");
            }
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name ="GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == city.Id);

            if (pointOfInterest == null)
                return NotFound();

            return Ok(pointOfInterest);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();
            
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            //tuto purpose, will be improved in the future
            var maxPoiId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            var pointOfInterestToInsert = new PointOfInterestDto()
            {
                Id = ++maxPoiId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(pointOfInterestToInsert);

            return CreatedAtRoute("GetPointOfInterest", new
            { cityId = cityId, id = pointOfInterestToInsert.Id }, pointOfInterestToInsert);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == id);

            if (pointOfInterestFromStore == null)
                return NotFound();

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (patchDocument == null)
                return BadRequest();

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == id);

            if (pointOfInterestFromStore == null)
                return NotFound();

            var pointOfInterestToPatch =
                new PointOfInterestForUpdateDto()
                {
                    Name = pointOfInterestFromStore.Name,
                    Description = pointOfInterestFromStore.Description
                };

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
                return BadRequest();

            if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
                ModelState.AddModelError("Description", "Name and Description cannot be the same");

            TryValidateModel(pointOfInterestToPatch);


            if (!ModelState.IsValid)
                return BadRequest();

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/id")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == id);

            if (pointOfInterestFromStore == null)
                return NotFound();

            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            return NoContent();
        }
    }
}
