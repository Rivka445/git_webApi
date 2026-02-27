# Repository layer instructions (EventDressRental)

## Purpose
Repositories encapsulate EF Core data access for the dress rental domain. Each repository:
- Has an interface (`I*Repository`) in `Repositories/`
- Has a concrete implementation that uses `EventDressRentalContext`
- Exposes async CRUD/query methods used by services

## Patterns to follow
- **Async-first:** use EF Core async APIs (`ToListAsync`, `FirstOrDefaultAsync`, `CountAsync`, `ExecuteUpdateAsync`).
- **Interface + implementation:** add new methods to both the interface and the class.
- **Context injection:** constructor should accept `EventDressRentalContext` and store it in a private readonly field.
- **Include related data:** use `.Include()` / `.ThenInclude()` when service DTOs depend on navigation properties.
- **Soft deletes:** follow existing pattern (e.g., `Dresses` uses `IsActive` and `ExecuteUpdateAsync`).
- **Save changes only when mutating:** call `SaveChangesAsync` only for add/update/delete operations.

## Files to be careful with
- `Repositories/EventDressRentalContext.cs` and `Entities/*.cs` are database-first and contain auto-generated sections. Avoid hand-editing unless regenerating the model.

## Testing guidance
- Repository tests live in `Tests/TestRepository/UnitTest`.
- Use `Moq.EntityFrameworkCore` to mock `DbSet<T>`.
- Cover happy + empty/unhappy paths when adding new repository methods.

## Example shape
```csharp
public interface ICategoryRepository
{
    Task<List<Category>> GetCategories();
}

public class CategoryRepository : ICategoryRepository
{
    private readonly EventDressRentalContext _eventDressRentalContext;
    public CategoryRepository(EventDressRentalContext eventDressRentalContext)
    {
        _eventDressRentalContext = eventDressRentalContext;
    }

    public async Task<List<Category>> GetCategories()
    {
        return await _eventDressRentalContext.Categories.ToListAsync();
    }
}
```
