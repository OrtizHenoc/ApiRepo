public interface IClientService
{
    Task <Client> CreateClient(Client client);
    Task<Client?> GetClient(int Id); //get client or null value if no found the id.
    Task<List<GetClientDto>> GetAll();
}