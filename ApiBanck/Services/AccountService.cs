
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;

    public AccountService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> CheckBalance(string accountNumber)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account == null)
        {
            throw new ArgumentException("La cuenta no existe");
        }

        return account.Balance;
    }

    public async Task<Account> CreateAccountAsync(CreateAccountDto accountDto)
    {
        var client = await _context.Clients.FindAsync(accountDto.IdClient);
        if (client == null)
            throw new Exception("Cliente no encontrado");

        var account = new Account
        {
            AccountNumber = Guid.NewGuid().ToString().Substring(0, 10),
            IdClient = accountDto.IdClient,
            Balance = accountDto.InitialBalance
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<List<GetAccountDto>> GetAll()
    {
        var accounts = await _context.Accounts.Include(a => a.Client).ToListAsync();
        return accounts.Select(a => new GetAccountDto
        {
            IdAccount = a.Id,
            AccountNumber = a.AccountNumber,
            Balance = a.Balance,
            ClientName = a.Client?.Name
        }).ToList();
    }

    public async Task<int> GetIdByNumberAccount(string accountNumber)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account == null)
        {
            throw new Exception("Account not found");
        }

        return account.Id;
    }

    public async Task<bool> Withdraw(Account account, decimal amount)
    {
        if (account.Balance < amount)
        {
            return false;
        }

        account.Balance -= amount;
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Deposit(Account account, decimal amount)
    {
        account.Balance += amount;
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
        return true;        
    }

}