using MediatR;
using MetaHammer.Application.DTOs;
using MetaHammer.Domain.Interfaces.Repositories;

namespace MetaHammer.Application.Features.Types;

public class AddMetaTypeProperties
{
    public record Command(Guid TypeGuid, AddMetaTypePropertiesRequest Request) : IRequest<bool>;

    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly IMetaTypeRepository _repository;

        public Handler(IMetaTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
        {
            var metaClass = await _repository.GetByIdAsync(command.TypeGuid);
            if (metaClass == null) return false;

            foreach (var propReq in command.Request.Properties)
            {
                var propType = await _repository.GetByIdAsync(propReq.TypeGuid);
                if (propType == null) continue;

                metaClass.AddProperty(propReq.Name, propType, propReq.IsCollection);
            }

            await _repository.SaveAsync(metaClass);
            return true;
        }
    }
}
