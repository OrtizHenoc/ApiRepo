
using Microsoft.EntityFrameworkCore;

public class ClientService : IClientService
{
    private readonly AppDbContext _context;
    public ClientService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Client> CreateClient(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task<Client?> GetClient(int Id)
    {
        return await _context.Clients.FindAsync(Id);
    }

    public async Task<List<GetClientDto>> GetAll()
    {
        var clients = await _context.Clients.ToListAsync();
        return clients.Select(a => new GetClientDto
        {
            Name = a.Name,
            Birthdate = a.Birthdate,
            Sex = a.Sex,
            Income = a.Income
        }).ToList();
    }
}