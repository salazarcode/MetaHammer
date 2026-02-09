using MediatR;
using Microsoft.AspNetCore.Mvc;
using MetaHammer.Application.Features.Types;
using MetaHammer.Application.DTOs;

namespace MetaHammer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetaTypeController : ControllerBase
{
    private readonly IMediator _mediator;

    public MetaTypeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMetaTypeRequest request)
    {
        var guid = await _mediator.Send(new CreateMetaType.Command(request));
        return Ok(guid);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _mediator.Send(new GetMetaTypes.Query());
        return Ok(list);
    }

    [HttpGet("{guid}")]
    public async Task<IActionResult> GetById(Guid guid)
    {
        var result = await _mediator.Send(new GetMetaTypeById.Query(guid));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost("{guid}/properties")]
    public async Task<IActionResult> AddProperties(Guid guid, [FromBody] AddMetaTypePropertiesRequest request)
    {
        var result = await _mediator.Send(new AddMetaTypeProperties.Command(guid, request));
        return result ? Ok() : NotFound();
    }
}
