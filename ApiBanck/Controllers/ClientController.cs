using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/controller")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;
    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientDto clientDto)
    {
        var client = new Client
        {
            Name = clientDto.Name,
            Birthdate = clientDto.Birthdate,
            Sex = clientDto.Sex,
            Income = clientDto.Income
        };

        var newClient = await _clientService.CreateClient(client);
        return CreatedAtAction(nameof(GetClient), new { id = newClient.Id }, newClient);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClient(int id)
    {
        var cliente = await _clientService.GetClient(id);
        if (cliente == null)
            return NotFound("Cliente no encontrado");
        return Ok(cliente);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetClientDto>>> GetAllClient()
    {
        var clientList = await _clientService.GetAll();
        return Ok(clientList);
    }
}