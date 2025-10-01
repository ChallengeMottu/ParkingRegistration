using Microsoft.AspNetCore.Mvc;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Services.interfaces;
using PulseSystem.Configuration; 

namespace PulseSystem.Controllers
{
    
    [Route("/zones")]
    [ApiController]
    public class ZoneController : ControllerBase
    {
        private readonly IZoneService _zoneService;
        private readonly HateoasConfig _hateoas; 

        /// <summary>
        /// Construtor da controller de zonas.
        /// </summary>
        /// <param name="zoneService">Serviço responsável pelas operações de zonas.</param>
        public ZoneController(IZoneService zoneService)
        {
            _zoneService = zoneService;
            _hateoas = new HateoasConfig();
        }

        /// <summary>
        /// Retorna todas as zonas com paginação.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<ZoneResponseDto>), 200)]
        public async Task<ActionResult<PaginatedResult<ZoneResponseDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _zoneService.GetAllAsync(pageNumber, pageSize);

            foreach (var dto in result.Items)
            {
                _hateoas.AddZoneLinks(dto, Url); 
            }

            return Ok(result);
        }

        /// <summary>
        /// Retorna uma zona pelo ID.
        /// </summary>
        [HttpGet("{id:long}", Name = "GetZoneById")]
        [ProducesResponseType(typeof(ZoneResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ZoneResponseDto>> GetById(long id)
        {
            var zone = await _zoneService.GetByIdAsync(id);
            _hateoas.AddZoneLinks(zone, Url); 
            return Ok(zone);
        }

        /// <summary>
        /// Retorna todas as zonas de um pátio específico.
        /// </summary>
        [HttpGet("parking/{parkingId:long}")]
        [ProducesResponseType(typeof(IEnumerable<ZoneResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<ZoneResponseDto>>> GetByParkingId(long parkingId)
        {
            var zones = await _zoneService.GetByParkingIdAsync(parkingId);

            foreach (var dto in zones)
            {
                _hateoas.AddZoneLinks(dto, Url);
            }

            return Ok(zones);
        }

        /// <summary>
        /// Cria uma nova zona em um pátio.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ZoneResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<ActionResult<ZoneResponseDto>> Create([FromBody] ZoneRequestDto dto)
        {
            var zone = await _zoneService.AddAsync(dto);
            _hateoas.AddZoneLinks(zone, Url); // 👈 adicionando links
            return CreatedAtAction(nameof(GetById), new { id = zone.Id }, zone);
        }

        /// <summary>
        /// Atualiza uma zona existente.
        /// </summary>
        [HttpPut("{id:long}", Name = "UpdateZone")]
        [ProducesResponseType(typeof(ZoneResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ZoneResponseDto>> Update(long id, [FromBody] ZoneRequestDto dto)
        {
            var updated = await _zoneService.UpdateAsync(id, dto);
            _hateoas.AddZoneLinks(updated, Url); // 👈 adicionando links
            return Ok(updated);
        }

        /// <summary>
        /// Remove uma zona existente pelo ID.
        /// </summary>
        [HttpDelete("{id:long}", Name = "DeleteZone")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(long id)
        {
            await _zoneService.RemoveAsync(id);
            return NoContent();
        }
    }
}
