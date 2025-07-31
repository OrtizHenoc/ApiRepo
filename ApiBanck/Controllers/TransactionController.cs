using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService service)
    {
        _transactionService = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateTransaction(CreateTransactionDto transactionDTO)
    {
        try
        {
            var result = await _transactionService.CreateTransaction(transactionDTO);
            return Ok(result);

        }
        catch (Exception ex)
        {

            return BadRequest(new { ex.Message });
        }
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SummaryTransactionDto>>> GetTransactionByAccountNumber(string accountNumber)
    {
        try
        {
            var summaryTransaction = await _transactionService.GetTransactionByAccountNumber(accountNumber);
            return Ok(summaryTransaction);
        }
        catch (Exception ex)
        {
            return BadRequest(new { ex.Message });
        }
    }
}