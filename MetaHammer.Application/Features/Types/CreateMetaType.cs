using MediatR;
using MetaHammer.Domain.Features.Classes;
using MetaHammer.Domain.Features.Classes.Enums;
using MetaHammer.Domain.Features.Organizations;
using MetaHammer.Domain.Features.Organizations.Identity;
using MetaHammer.Domain.Interfaces.Repositories;
using MetaHammer.Application.DTOs;

namespace MetaHammer.Application.Features.Types;

public class CreateMetaType
{
    public record Command(CreateMetaTypeRequest Request) : IRequest<Guid>;

    public class Handler : IRequestHandler<Command, Guid>
    {
        private readonly IMetaTypeRepository _repository;

        public Handler(IMetaTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(Command command, CancellationToken cancellationToken)
        {
            var req = command.Request;
            
            // Mocking Organization and User for now as we don't have full identity logic yet
            var org = new Organization(req.OrganizationId, "Default Org");
            var user = new User(org, req.UserId, "admin");

            var metaClass = new MetaClass(
                Guid.NewGuid(),
                req.Name,
                Enum.Parse<MetaNature>(req.Nature),
                org,
                user,
                req.IsNative
            );

            await _repository.SaveAsync(metaClass);
            return metaClass.Guid;
        }
    }
}
