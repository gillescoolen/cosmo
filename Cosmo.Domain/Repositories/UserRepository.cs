using Cosmo.Domain.Data;

public class UserRepository
{
    private readonly ApplicationDbContext context;
    public UserRepository(ApplicationDbContext context)
    {
        this.context = context;
    }
}