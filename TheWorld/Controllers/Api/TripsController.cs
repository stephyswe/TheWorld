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
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
	[Authorize]
	[Route("api/trips")]
	public class TripsController : Controller
	{
		private IWorldRepository _respository;
		private ILogger<TripsController> _logger;

		public TripsController(IWorldRepository repository, ILogger<TripsController> logger)
		{
			_respository = repository;
			_logger = logger;

		}

		[HttpGet("")]
		public IActionResult Get()
		{
			try
			{
				var results = _respository.GetTripsByUsername(this.User.Identity.Name);
				return Ok(Mapper.Map<IEnumerable<TripViewModel>>(results));
			}
			catch (Exception ex)
			{
				// Logging
				_logger.LogError($"Failed to get All Trips: {ex}");
				return BadRequest("Error occurred");
			}
			
		}

		[HttpPost("")]
		public async Task<IActionResult> Post([FromBody]TripViewModel theTrip)
		{
			if (ModelState.IsValid)
			{
				// Save to the Database
				var newTrip = Mapper.Map<Trip>(theTrip);
				newTrip.UserName = User.Identity.Name;
				_respository.AddTrip(newTrip);

				if (await _respository.SaveChangesAsync())
				{
					//Sending the object as it looks after any database calls.
					return Created($"api/trips/{theTrip.Name}", Mapper.Map<TripViewModel>(newTrip));
				} 
			}
			return BadRequest("Failed to save the trip");
		}
	}
}
