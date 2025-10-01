using Microsoft.AspNetCore.Mvc;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Services.interfaces;
using PulseSystem.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PulseSystem.Controllers
{
    
    [Route("/parkings")]
    [ApiController]
    public class ParkingController : ControllerBase
    {
        private readonly IParkingService _parkingService;
        private readonly HateoasConfig _hateoas;

        /// <summary>
        /// Construtor da controller de pátios.
        /// </summary>
        /// <param name="parkingService">Serviço responsável pelas operações de pátios.</param>
        public ParkingController(IParkingService parkingService)
        {
            _parkingService = parkingService;
            _hateoas = new HateoasConfig();
        }

        /// <summary>
        /// Retorna todos os pátios com paginação.
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão 1).</param>
        /// <param name="pageSize">Tamanho da página (padrão 10).</param>
        /// <returns>Lista paginada de pátios.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<ParkingResponseDto>), 200)]
        public async Task<ActionResult<PaginatedResult<ParkingResponseDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _parkingService.GetAllAsync(pageNumber, pageSize);

            foreach (var dto in result.Items)
            {
                _hateoas.AddParkingLinks(dto, Url);
            }

            return Ok(result);
        }

        /// <summary>
        /// Retorna um pátio pelo ID.
        /// </summary>
        /// <param name="id">ID do pátio.</param>
        /// <returns>Pátio correspondente.</returns>
        [HttpGet("{id:long}", Name = "GetParkingById")]
        [ProducesResponseType(typeof(ParkingResponseListDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ParkingResponseListDto>> GetById(long id)
        {
            var parking = await _parkingService.GetByIdAsync(id);
            _hateoas.AddParkingLinks(parking, Url);
            return Ok(parking);
        }

        /// <summary>
        /// Retorna um pátio pelo endereço (rua e complemento).
        /// </summary>
        /// <param name="street">Nome da rua do pátio.</param>
        /// <param name="complement">Complemento do endereço do pátio.</param>
        /// <returns>Pátio correspondente ao endereço informado.</returns>
        [HttpGet("location")]
        [ProducesResponseType(typeof(ParkingResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ParkingResponseDto>> GetByLocation([FromQuery] string street, [FromQuery] string complement)
        {
            var parking = await _parkingService.GetByLocationAsync(street, complement);
            _hateoas.AddParkingLinks(parking, Url);
            return Ok(parking);
        }

        /// <summary>
        /// Cria um novo pátio.
        /// </summary>
        /// <param name="dto">Dados do pátio a ser criado.</param>
        /// <returns>Pátio criado, com sugestões de zonas e gateways.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ParkingSuggestionDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ParkingSuggestionDto>> Create([FromBody] ParkingRequestDto dto)
        {
            var parking = await _parkingService.AddAsync(dto);
            _hateoas.AddParkingLinks(parking, Url);
            return CreatedAtAction(nameof(GetById), new { id = parking.Id }, parking);
        }

        /// <summary>
        /// Atualiza um pátio existente.
        /// </summary>
        /// <param name="id">ID do pátio a ser atualizado.</param>
        /// <param name="dto">Dados atualizados do pátio.</param>
        /// <returns>Pátio atualizado, com sugestões de zonas e gateways.</returns>
        [HttpPut("{id:long}", Name = "UpdateParking")]
        [ProducesResponseType(typeof(ParkingSuggestionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ParkingSuggestionDto>> Update(long id, [FromBody] ParkingRequestDto dto)
        {
            var updated = await _parkingService.UpdateAsync(id, dto);
            _hateoas.AddParkingLinks(updated, Url);
            return Ok(updated);
        }

        /// <summary>
        /// Remove um pátio existente pelo ID.
        /// </summary>
        /// <param name="id">ID do pátio a ser removido.</param>
        [HttpDelete("{id:long}", Name = "DeleteParking")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(long id)
        {
            await _parkingService.RemoveAsync(id);
            return NoContent();
        }
    }
}
