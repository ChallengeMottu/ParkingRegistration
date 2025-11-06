using System.ComponentModel;
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
    [ApiExplorerSettings(GroupName = "v2")]
    [DisplayName("GatewayControllerV2")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/gateway")]
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

        /// <summary>
        /// Obtém um gateway pelo ID.
        /// </summary>
        /// <param name="id">ID do gateway.</param>
        /// <returns>Gateway correspondente.</returns>
        /// <response code="200">Gateway encontrado.</response>
        /// <response code="404">Gateway não encontrado.</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(GatewayResponseDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<GatewayResponseDto>> GetById(long id)
        {
            var gateway = await _gatewayService.GetByIdAsync(id);
            _hateoas.AddGatewayLinks(gateway, Url);
            return Ok(gateway);
        }

        /// <summary>
        /// Cria um novo gateway.
        /// </summary>
        /// <param name="dto">Dados do gateway.</param>
        /// <returns>Gateway criado.</returns>
        /// <response code="201">Criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Pátio associado não encontrado.</response>
        /// <response code="409">MAC Address já cadastrado.</response>
        [HttpPost]
        [ProducesResponseType(typeof(GatewayResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<GatewayResponseDto>> Create([FromBody] GatewayRequestDto dto)
        {
            var created = await _gatewayService.AddAsync(dto);
            _hateoas.AddGatewayLinks(created, Url);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Atualiza um gateway existente.
        /// </summary>
        /// <param name="id">ID do gateway.</param>
        /// <param name="dto">Dados atualizados.</param>
        /// <returns>Gateway atualizado.</returns>
        /// <response code="200">Atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Gateway não encontrado.</response>
        /// <response code="409">MAC Address já utilizado.</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(GatewayResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<GatewayResponseDto>> Update(long id, [FromBody] GatewayRequestDto dto)
        {
            var updated = await _gatewayService.UpdateAsync(id, dto);
            _hateoas.AddGatewayLinks(updated, Url);
            return Ok(updated);
        }

        /// <summary>
        /// Obtém um gateway pelo endereço MAC.
        /// </summary>
        /// <param name="macAddress">MAC Address.</param>
        /// <returns>Gateway correspondente.</returns>
        /// <response code="200">Gateway encontrado.</response>
        /// <response code="404">Gateway não encontrado.</response>
        [HttpGet("macAddress/{macAddress}")]
        [ProducesResponseType(typeof(GatewayResponseDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<GatewayResponseDto>> GetByMacAddress(string macAddress)
        {
            var gateway = await _gatewayService.GetByMacAddressAsync(macAddress);
            _hateoas.AddGatewayLinks(gateway, Url);
            return Ok(gateway);
        }

        /// <summary>
        /// Remove um gateway.
        /// </summary>
        /// <param name="id">ID do gateway.</param>
        /// <response code="204">Removido com sucesso.</response>
        /// <response code="404">Gateway não encontrado.</response>
        [HttpDelete("delete/{id:long}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Delete(long id)
        {
            await _gatewayService.RemoveAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Lista todos os gateways de um pátio específico.
        /// </summary>
        /// <param name="parkingId">ID do pátio.</param>
        /// <returns>Lista de gateways.</returns>
        /// <response code="200">Lista retornada.</response>
        /// <response code="404">Pátio não encontrado.</response>
        /// <response code="401">Usuário não logado, sem permissão para executar a funcionalidade</response>
        [HttpGet("parkingId/{parkingId:long}")]
        [ProducesResponseType(typeof(IEnumerable<GatewayResponseDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<GatewayResponseDto>>> GetByParkingId(long parkingId)
        {
            var gateways = await _gatewayService.GetAllByParkingId(parkingId);

            foreach (var dto in gateways)
                _hateoas.AddGatewayLinks(dto, Url);

            return Ok(gateways);
        }
    }
}
