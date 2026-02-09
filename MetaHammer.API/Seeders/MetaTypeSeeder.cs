using MediatR;
using MetaHammer.Application.DTOs;
using MetaHammer.Application.Features.Types;
using MetaHammer.Domain.Interfaces.Repositories;

namespace MetaHammer.API.Seeders;

public class MetaTypeSeeder
{
    private readonly IMediator _mediator;
    private readonly IMetaTypeRepository _repository;

    public MetaTypeSeeder(IMediator mediator, IMetaTypeRepository repository)
    {
        _mediator = mediator;
        _repository = repository;
    }

    public async Task SeedAsync()
    {
        int retries = 5;
        while (retries > 0)
        {
            try
            {
                var existingTypes = await _repository.GetAllAsync();
                var primitives = new[] { "String", "Int32", "Boolean", "DateTime", "Guid", "Decimal", "Double" };

                var systemOrgId = Guid.Parse("00000000-0000-0000-0000-000000000001");
                var systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

                foreach (var typeName in primitives)
                {
                    if (!existingTypes.Any(t => t.Name == typeName))
                    {
                        await _mediator.Send(new CreateMetaType.Command(new CreateMetaTypeRequest(
                            typeName,
                            "Primitive",
                            systemOrgId,
                            systemUserId,
                            IsNative: true
                        )));
                    }
                }
                break; // Success
            }
            catch (Exception ex)
            {
                retries--;
                if (retries == 0) throw;
                Console.WriteLine($"Seeding failed, retrying in 5s... ({ex.Message})");
                await Task.Delay(5000);
            }
        }
    }
}
