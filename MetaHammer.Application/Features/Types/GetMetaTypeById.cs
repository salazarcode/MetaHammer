using MediatR;
using MetaHammer.Application.DTOs;
using MetaHammer.Domain.Interfaces.Repositories;

namespace MetaHammer.Application.Features.Types;

public class GetMetaTypeById
{
    public record Query(Guid Guid) : IRequest<MetaTypeDto?>;

    public class Handler : IRequestHandler<Query, MetaTypeDto?>
    {
        private readonly IMetaTypeRepository _repository;

        public Handler(IMetaTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<MetaTypeDto?> Handle(Query request, CancellationToken cancellationToken)
        {
            var metaClass = await _repository.GetByIdAsync(request.Guid);
            if (metaClass == null) return null;

            return new MetaTypeDto(
                metaClass.Guid,
                metaClass.Name,
                metaClass.MetaNature.ToString(),
                metaClass.IsNative,
                metaClass.ParentClassGuid == Guid.Empty ? null : metaClass.ParentClassGuid,
                metaClass.Properties.Select(p => new MetaPropertyDto(p.Name, p.MetaClass.Guid, p.MetaClass.Name, p.IsCollection)).ToList()
            );
        }
    }
}
