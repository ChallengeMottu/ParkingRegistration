using System.ComponentModel;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.ML.DTOs;
using PulseSystem.Application.Services.interfaces.v2;
using PulseSystem.Application.ML.Services;
using PulseSystem.Configuration;

namespace PulseSystem.Controllers.v2
{
    [Authorize(Roles = "GESTOR")]
    [ApiExplorerSettings(GroupName = "v2")]
    [DisplayName("ParkingControllerV2")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/parking")]
    [ApiController]
    public class ParkingControllerV2 : ControllerBase
    {
        private readonly IParkingServiceV2 _parkingService;
        private readonly IGatewayPredictionService _mlService;
        private readonly HateoasConfig _hateoas;

        public ParkingControllerV2(IParkingServiceV2 parkingService, IGatewayPredictionService mlService)
        {
            _parkingService = parkingService;
            _mlService = mlService;
            _hateoas = new HateoasConfig();
        }

        /// <summary>
        /// Cria um novo pátio.
        /// </summary>
        /// <param name="dto">Dados do pátio.</param>
        /// <returns>Pátio criado.</returns>
        /// <response code="201">Pátio criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        ///<response code="401">Usuário não logado, sem permissão para executar a funcionalidade</response>
        [HttpPost]
        [ProducesResponseType(typeof(ParkingSuggestionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ParkingSuggestionDto>> Create([FromBody] ParkingRequestDto dto)
        {
            var parking = await _parkingService.AddAsync(dto);
            _hateoas.AddParkingLinks(parking, Url);
            return CreatedAtAction(nameof(GetById), new { id = parking.Id }, parking);
        }

        /// <summary>
        /// Atualiza um pátio existente.
        /// </summary>
        /// <param name="id">ID do pátio.</param>
        /// <param name="dto">Dados atualizados.</param>
        /// <returns>Pátio atualizado.</returns>
        /// <response code="200">Pátio atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Pátio não encontrado.</response>
        /// <response code="401">Usuário não logado, sem permissão para executar a funcionalidade</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(ParkingSuggestionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ParkingSuggestionDto>> Update(long id, [FromBody] ParkingRequestDto dto)
        {
            var updated = await _parkingService.UpdateAsync(id, dto);
            _hateoas.AddParkingLinks(updated, Url);
            return Ok(updated);
        }

        /// <summary>
        /// Obtém um pátio pelo ID.
        /// </summary>
        /// <param name="id">ID do pátio.</param>
        /// <returns>Pátio correspondente.</returns>
        /// <response code="200">Pátio encontrado.</response>
        /// <response code="404">Pátio não encontrado.</response>
        /// <response code="401">Usuário não logado, sem permissão para executar a funcionalidade</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ParkingResponseListDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ParkingResponseListDto>> GetById(long id)
        {
            var parking = await _parkingService.GetByIdAsync(id);
            _hateoas.AddParkingLinks(parking, Url);
            return Ok(parking);
        }

        /// <summary>
        /// Sugere a quantidade ideal de gateways usando Machine Learning.
        /// </summary>
        /// <param name="id">ID do pátio.</param>
        /// <param name="query">Parâmetros opcionais para cálculo.</param>
        /// <returns>Resultado contendo parâmetros utilizados e sugestão gerada.</returns>
        /// <response code="200">Sugestão gerada com sucesso.</response>
        /// <response code="404">Pátio não encontrado.</response>
        /// <response code="401">Usuário não logado, sem permissão para executar a funcionalidade</response>
        [HttpPost("{id:long}/suggest-gateways")]
        [ProducesResponseType(typeof(ParkingGatewaySuggestionDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SuggestGateways(long id, [FromBody] ParkingGatewayQueryDto query)
        {
            var parking = await _parkingService.GetByIdAsync(id);
            if (parking == null)
                return NotFound();

            var area = query.AvailableArea ?? parking.AvailableArea;
            var capacity = query.Capacity ?? parking.Capacity;

            var suggested = _mlService.PredictGateways(
                area,
                capacity,
                query.IrregularityFactor,
                query.DistanceBetweenZones
            );

            var result = new ParkingGatewaySuggestionDto
            {
                ParkingId = id,
                Area = area,
                Capacity = capacity,
                IrregularityFactor = query.IrregularityFactor,
                DistanceBetweenZones = query.DistanceBetweenZones,
                SuggestedGateways = suggested
            };

            return Ok(result);
        }


        /// <summary>
        /// Obtém a planta baixa (SVG) do pátio.
        /// </summary>
        /// <param name="id">ID do pátio.</param>
        /// <response code="200">SVG retornado.</response>
        /// <response code="404">Pátio não encontrado.</response>
        /// <response code="401">Usuário não logado, sem permissão para executar a funcionalidade</response>
        [HttpGet("{id:long}/structure", Name = "GetStructurePlanByIdV2")]
        [Produces("image/svg+xml")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetStructurePlanByIdAsync(long id)
        {
            var parking = await _parkingService.GetStructurePlanByIdAsync(id);
            return Content(parking, "image/svg+xml");
        }

        /// <summary>
        /// Remove um pátio.
        /// </summary>
        /// <param name="id">ID do pátio.</param>
        /// <response code="204">Removido com sucesso.</response>
        /// <response code="404">Pátio não encontrado.</response>
        /// <response code="401">Usuário não logado, sem permissão para executar a funcionalidade</response>
        [HttpDelete("delete/{id:long}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Delete(long id)
        {
            await _parkingService.RemoveAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Lista pátios com paginação.
        /// </summary>
        /// <param name="pageNumber">Página (padrão 1).</param>
        /// <param name="pageSize">Tamanho da página (padrão 10).</param>
        /// <returns>Lista paginada.</returns>
        /// <response code="200">Lista retornada com sucesso.</response>
        /// <response code="401">Usuário não logado, sem permissão para executar a funcionalidade</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<ParkingResponseDto>), 200)]
        public async Task<ActionResult<PaginatedResult<ParkingResponseDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _parkingService.GetAllAsync(pageNumber, pageSize);

            foreach (var dto in result.Items)
                _hateoas.AddParkingLinks(dto, Url);

            return Ok(result);
        }
    }
}
