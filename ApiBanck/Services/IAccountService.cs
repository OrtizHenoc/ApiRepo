public interface IAccountService
{
    Task<Account> CreateAccountAsync(CreateAccountDto accountDto);
    Task<int> GetIdByNumberAccount(string accountNumber);
    Task<List<GetAccountDto>> GetAll();
    Task<decimal> CheckBalance(string accountNumber);
    Task<bool> Deposit(Account account, decimal amount);
    Task<bool> Withdraw(Account account, decimal amount);

}