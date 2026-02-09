using MediatR;
using MetaHammer.Domain.Features.Objects;
using MetaHammer.Domain.Interfaces.Repositories;
using MetaHammer.Application.DTOs;

namespace MetaHammer.Application.Features.Instances;

public class CreateMetaObject
{
    public record Command(CreateMetaObjectRequest Request) : IRequest<Guid>;

    public class Handler : IRequestHandler<Command, Guid>
    {
        private readonly IMetaObjectRepository _objectRepository;
        private readonly IMetaTypeRepository _typeRepository;

        public Handler(IMetaObjectRepository objectRepository, IMetaTypeRepository typeRepository)
        {
            _objectRepository = objectRepository;
            _typeRepository = typeRepository;
        }

        public async Task<Guid> Handle(Command command, CancellationToken cancellationToken)
        {
            var req = command.Request;
            
            var metaClass = await _typeRepository.GetByIdAsync(req.ClassGuid);
            if (metaClass == null) throw new Exception("MetaClass not found");

            var metaObject = new MetaObject(
                metaClass,
                Guid.NewGuid(),
                req.TenantId,
                req.UserId
            );

            // Set initial properties if any (simplified for now)
            // In a real scenario, we'd validate against MetaClass properties
            
            await _objectRepository.SaveAsync(metaObject);
            return metaObject.Guid;
        }
    }
}
