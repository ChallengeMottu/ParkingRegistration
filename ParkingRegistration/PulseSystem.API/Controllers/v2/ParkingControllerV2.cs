using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Services.Implementations;
using PulseSystem.Application.Services.Implementations.v2;
using PulseSystem.Application.Services.interfaces.v2;
using PulseSystem.Configuration;

namespace PulseSystem.Controllers.v2
{
    [Authorize(Roles = "GESTOR")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class ParkingControllerV2 : ControllerBase
    {
        private readonly IParkingServiceV2 _parkingService;
        private readonly HateoasConfig _hateoas;

        public ParkingControllerV2(IParkingServiceV2 parkingService)
        {
            _parkingService = parkingService;
            _hateoas = new HateoasConfig();
        }

        [HttpPost]
        [ProducesResponseType(typeof(ParkingSuggestionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<ParkingSuggestionDto>> Create([FromBody] ParkingRequestDto dto)
        {
            var parking = await _parkingService.AddAsync(dto);
            _hateoas.AddParkingLinks(parking, Url);
            return CreatedAtAction(nameof(GetById), new { id = parking.Id }, parking);
        }

        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(ParkingSuggestionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<ParkingSuggestionDto>> Update(long id, [FromBody] ParkingRequestDto dto)
        {
            var updated = await _parkingService.UpdateAsync(id, dto);
            _hateoas.AddParkingLinks(updated, Url);
            return Ok(updated);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ParkingResponseListDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ParkingResponseListDto>> GetById(long id)
        {
            var parking = await _parkingService.GetByIdAsync(id);
            _hateoas.AddParkingLinks(parking, Url);
            return Ok(parking);
        }
    }
}