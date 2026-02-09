using MediatR;
using MetaHammer.Application.DTOs;
using MetaHammer.Domain.Interfaces.Repositories;

namespace MetaHammer.Application.Features.Types;

public class GetMetaTypes
{
    public record Query : IRequest<List<MetaTypeDto>>;

    public class Handler : IRequestHandler<Query, List<MetaTypeDto>>
    {
        private readonly IMetaTypeRepository _repository;

        public Handler(IMetaTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MetaTypeDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var readModels = await _repository.GetAllAsync();
            return readModels.Select(rm => new MetaTypeDto(
                rm.Guid, 
                rm.Name, 
                rm.Nature, 
                rm.IsNative, 
                rm.ParentGuid,
                rm.Properties.Select(p => new MetaPropertyDto(p.Name, p.TypeGuid, p.TypeName, p.IsCollection)).ToList()
            )).ToList();
        }
    }
}
