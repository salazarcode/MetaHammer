using MetaHammer.Domain.Features.Classes;
using MetaHammer.Domain.ReadModels;

namespace MetaHammer.Domain.Interfaces.Repositories;

public interface IMetaTypeRepository : IRepository<MetaClass>
{
    Task<MetaClass?> GetByIdAsync(Guid guid);
    Task<List<MetaTypeReadModel>> GetAllAsync();
}
