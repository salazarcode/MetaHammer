using MediatR;
using Microsoft.AspNetCore.Mvc;
using MetaHammer.Application.Features.Instances;
using MetaHammer.Application.DTOs;

namespace MetaHammer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetaObjectController : ControllerBase
{
    private readonly IMediator _mediator;

    public MetaObjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMetaObjectRequest request)
    {
        var guid = await _mediator.Send(new CreateMetaObject.Command(request));
        return Ok(guid);
    }
}
