using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto accountDto)
    {
        var account = await _accountService.CreateAccountAsync(accountDto);
        return Ok(account);
    }

    [HttpGet("balance/{accountNumber}")]
    public async Task<IActionResult> GetBalance(string accountNumber)
    {
        try
        {
            var balance = await _accountService.CheckBalance(accountNumber);
            return Ok(new { Balance = balance });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { ex.Message });
        }
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetAccountDto>>> GetAllAccounts()
    {
        var accountList = await _accountService.GetAll();
        return Ok(accountList);
    }
}