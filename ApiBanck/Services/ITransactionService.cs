public interface ITransactionService
{
    Task <GetTransactionDto> CreateTransaction(CreateTransactionDto transactionDto);
    Task<SummaryTransactionDto> GetTransactionByAccountNumber(string accountNumber);
}