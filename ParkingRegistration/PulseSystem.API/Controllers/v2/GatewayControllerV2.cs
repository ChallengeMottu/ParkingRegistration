using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Services.interfaces.v2;
using PulseSystem.Configuration;

namespace PulseSystem.Controllers.v2
{
    [Authorize(Roles = "GESTOR")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class GatewayControllerV2 : ControllerBase
    {
        private readonly IGatewayServiceV2 _gatewayService;
        private readonly HateoasConfig _hateoas;

        public GatewayControllerV2(IGatewayServiceV2 gatewayService)
        {
            _gatewayService = gatewayService;
            _hateoas = new HateoasConfig();
        }

        
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(GatewayResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<GatewayResponseDto>> GetById(long id)
        {
            var gateway = await _gatewayService.GetByIdAsync(id);
            _hateoas.AddGatewayLinks(gateway, Url);
            return Ok(gateway);
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(GatewayResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<GatewayResponseDto>> Create([FromBody] GatewayRequestDto dto)
        {
            var created = await _gatewayService.AddAsync(dto);
            _hateoas.AddGatewayLinks(created, Url);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(GatewayResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<GatewayResponseDto>> Update(long id, [FromBody] GatewayRequestDto dto)
        {
            var updated = await _gatewayService.UpdateAsync(id, dto);
            _hateoas.AddGatewayLinks(updated, Url);
            return Ok(updated);
        }

        
    }
}
