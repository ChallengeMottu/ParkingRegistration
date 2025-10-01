using Microsoft.AspNetCore.Mvc;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Configuration;

namespace PulseSystem.Controllers
{
    
    [Route("/gateways")]
    [ApiController]
    public class GatewayController : ControllerBase
    {
        private readonly IGatewayService _gatewayService;
        private readonly HateoasConfig _hateoas;

        /// <summary>
        /// Construtor da controller de gateways.
        /// </summary>
        /// <param name="gatewayService">Serviço de gateways.</param>
        public GatewayController(IGatewayService gatewayService)
        {
            _gatewayService = gatewayService;
            _hateoas = new HateoasConfig();
        }
        
        /// <summary>
        /// Retorna todos os gateways com paginação.
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão 1).</param>
        /// <param name="pageSize">Tamanho da página (padrão 10).</param>
        /// <returns>Lista paginada de gateways.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<GatewayResponseDto>), 200)]
        public async Task<ActionResult<PaginatedResult<GatewayResponseDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _gatewayService.GetAllAsync(pageNumber, pageSize);

            foreach (var dto in result.Items)
            {
                _hateoas.AddGatewayLinks(dto, Url);
            }

            return Ok(result);
        }

        /// <summary>
        /// Retorna um gateway pelo ID.
        /// </summary>
        /// <param name="id">ID do gateway.</param>
        /// <returns>Gateway correspondente.</returns>
        [HttpGet("{id:long}", Name = "GetGatewayById")]
        [ProducesResponseType(typeof(GatewayResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<GatewayResponseDto>> GetById(long id)
        {
            var gateway = await _gatewayService.GetByIdAsync(id);
            _hateoas.AddGatewayLinks(gateway, Url);
            return Ok(gateway);
        }

        /// <summary>
        /// Retorna um gateway pelo endereço MAC.
        /// </summary>
        /// <param name="macAddress">Endereço MAC do gateway.</param>
        /// <returns>Gateway correspondente.</returns>
        [HttpGet("mac/{macAddress}")]
        [ProducesResponseType(typeof(GatewayResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<GatewayResponseDto>> GetByMacAddress(string macAddress)
        {
            var gateway = await _gatewayService.GetByMacAddressAsync(macAddress);
            _hateoas.AddGatewayLinks(gateway, Url);
            return Ok(gateway);
        }

        /// <summary>
        /// Retorna todos os gateways de um pátio específico.
        /// </summary>
        /// <param name="parkingId">ID do pátio.</param>
        /// <returns>Lista de gateways do pátio.</returns>
        [HttpGet("parking/{parkingId:long}")]
        [ProducesResponseType(typeof(IEnumerable<GatewayResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<GatewayResponseDto>>> GetByParkingId(long parkingId)
        {
            var gateways = await _gatewayService.GetAllByParkingId(parkingId);

            foreach (var dto in gateways)
            {
                _hateoas.AddGatewayLinks(dto, Url);
            }

            return Ok(gateways);
        }

        /// <summary>
        /// Cria um novo gateway.
        /// </summary>
        /// <param name="dto">Dados do gateway a ser criado.</param>
        /// <returns>Gateway criado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(GatewayResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<GatewayResponseDto>> Create([FromBody] GatewayRequestDto dto)
        {
            var gateway = await _gatewayService.AddAsync(dto);
            _hateoas.AddGatewayLinks(gateway, Url);
            return CreatedAtAction(nameof(GetById), new { id = gateway.Id }, gateway);
        }

        /// <summary>
        /// Atualiza um gateway existente.
        /// </summary>
        /// <param name="id">ID do gateway.</param>
        /// <param name="dto">Dados atualizados do gateway.</param>
        /// <returns>Gateway atualizado.</returns>
        [HttpPut("{id:long}", Name = "UpdateGateway")]
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

        /// <summary>
        /// Remove um gateway existente.
        /// </summary>
        /// <param name="id">ID do gateway.</param>
        [HttpDelete("{id:long}", Name = "DeleteGateway")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(long id)
        {
            await _gatewayService.RemoveAsync(id);
            return NoContent();
        }
    }
}
