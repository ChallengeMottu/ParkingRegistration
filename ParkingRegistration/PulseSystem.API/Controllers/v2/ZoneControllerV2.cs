using System.ComponentModel;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Services.interfaces;
using PulseSystem.Configuration;

namespace PulseSystem.Controllers.v2;

[Authorize(Roles = "GESTOR")]
[ApiExplorerSettings(GroupName = "v2")]
[DisplayName("ZoneControllerV2")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/zone")]
[ApiController]
public class ZoneControllerV2 : ControllerBase
{
    private readonly IZoneService _zoneService;
    private readonly HateoasConfig _hateoas;

    
    public ZoneControllerV2(IZoneService zoneService, HateoasConfig hateoas)
    {
        _zoneService = zoneService;
        _hateoas = hateoas;
    }

    /// <summary>
    /// Retorna todas as zonas com suporte à paginação.
    /// </summary>
    /// <param name="pageNumber">Número da página desejada. Valor padrão: 1.</param>
    /// <param name="pageSize">Quantidade de itens por página. Valor padrão: 10.</param>
    /// <returns>Lista paginada contendo zonas.</returns>
    /// <response code="200">Retorna a lista paginada de zonas.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<ZoneResponseDto>), 200)]
    public async Task<ActionResult<PaginatedResult<ZoneResponseDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _zoneService.GetAllAsync(pageNumber, pageSize);

        foreach (var dto in result.Items)
        {
            _hateoas.AddZoneLinks(dto, Url);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retorna uma zona específica com base no ID.
    /// </summary>
    /// <param name="id">ID da zona.</param>
    /// <returns>Objeto contendo os dados da zona.</returns>
    /// <response code="200">Retorna a zona encontrada.</response>
    /// <response code="404">Nenhuma zona foi encontrada com o ID informado.</response>
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
    /// Retorna todas as zonas pertencentes a um pátio específico.
    /// </summary>
    /// <param name="parkingId">ID do pátio.</param>
    /// <returns>Lista de zonas vinculadas ao pátio informado.</returns>
    /// <response code="200">Retorna a lista de zonas do pátio.</response>
    /// <response code="404">Nenhuma zona foi encontrada para o pátio informado.</response>
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
    /// <param name="dto">Dados necessários para a criação da zona.</param>
    /// <returns>A zona criada.</returns>
    /// <response code="201">Zona criada com sucesso.</response>
    /// <response code="400">Dados inválidos foram enviados.</response>
    /// <response code="404">O pátio informado não foi encontrado.</response>
    /// <response code="422">Erro de validação ao criar a zona.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ZoneResponseDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<ActionResult<ZoneResponseDto>> Create([FromBody] ZoneRequestDto dto)
    {
        var zone = await _zoneService.AddAsync(dto);

        _hateoas.AddZoneLinks(zone, Url);

        return CreatedAtAction(nameof(GetById), new { id = zone.Id }, zone);
    }

    /// <summary>
    /// Atualiza os dados de uma zona existente.
    /// </summary>
    /// <param name="id">ID da zona a ser atualizada.</param>
    /// <param name="dto">Dados atualizados da zona.</param>
    /// <returns>Zona atualizada.</returns>
    /// <response code="200">Zona atualizada com sucesso.</response>
    /// <response code="400">Dados inválidos foram enviados.</response>
    /// <response code="404">Nenhuma zona foi encontrada com o ID informado.</response>
    [HttpPut("{id:long}", Name = "UpdateZone")]
    [ProducesResponseType(typeof(ZoneResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ZoneResponseDto>> Update(long id, [FromBody] ZoneRequestDto dto)
    {
        var updated = await _zoneService.UpdateAsync(id, dto);

        _hateoas.AddZoneLinks(updated, Url);

        return Ok(updated);
    }

    /// <summary>
    /// Remove uma zona com base no ID informado.
    /// </summary>
    /// <param name="id">ID da zona que será removida.</param>
    /// <response code="204">Zona removida com sucesso.</response>
    /// <response code="404">Nenhuma zona foi encontrada com o ID informado.</response>
    [HttpDelete("{id:long}", Name = "DeleteZone")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(long id)
    {
        await _zoneService.RemoveAsync(id);
        return NoContent();
    }
}
