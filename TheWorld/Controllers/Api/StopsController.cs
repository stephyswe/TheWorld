using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
	[Authorize]
	[Route("/api/trips/{tripName}/stops")]
	public class StopsController : Controller
	{
		private IWorldRepository _respository;
		private ILogger<StopsController> _logger;
		private GeoCoordsService _coordsService;

		public StopsController(IWorldRepository repository, ILogger<StopsController> logger, GeoCoordsService coordsService)
		{
			_respository = repository;
			_logger = logger;
			_coordsService = coordsService;
		}

		[HttpGet("")]
		public IActionResult Get(string tripName) //Stops for specific Trips
		{
			try
			{
				var trip = _respository.GetUserTripByName(tripName, User.Identity.Name);
				return Ok(Mapper.Map<IEnumerable<StopViewModel>>(trip.Stops.OrderBy(s => s.Order).ToList()));
			}
			catch (Exception ex)
			{
				_logger.LogError($"Failed to get stops: {ex} ");
			}
			return BadRequest("Failed to get stops");
		}

		[HttpPost("")]
		public async Task<IActionResult> Post(string tripName, [FromBody]StopViewModel vm)
		{
			try
			{
				// If the VM is valid
				if (ModelState.IsValid)
				{
					var newStop = Mapper.Map<Stop>(vm);
					// Lookup the Geocodes
					var result = await _coordsService.GetCoordsAsync(newStop.Name);
					if (!result.Success)
					{
						_logger.LogError(result.Message);
					}
					else
					{
						newStop.Latitude = result.Latitude;
						newStop.Longitude = result.Longitude;
						//Save to the Database
						_respository.AddStop(tripName, newStop, User.Identity.Name);

						if (await _respository.SaveChangesAsync())
						{
							return Created($"/api/trips/{tripName}/stops/{newStop.Name}", //Result of Post when you save it
								Mapper.Map<StopViewModel>(newStop)); //Convert it back to a ViewModel. 
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Failed to save the new Stop: {ex}");
			}
			return BadRequest("Failed to save new stop");
		}
	}
}
