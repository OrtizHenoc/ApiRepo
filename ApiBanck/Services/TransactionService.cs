using Microsoft.EntityFrameworkCore;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;
    private readonly IAccountService _accountService;
    public TransactionService(AppDbContext context, IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    public async Task <GetTransactionDto> CreateTransaction(CreateTransactionDto transactionDTO)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == transactionDTO.AccountNumber);

        bool success;

        if (account == null)
        {
            throw new Exception("No se encontro la cuenta"); 
        }

        if (transactionDTO.TransactionType == Type.Retiro) //Retiro de cuenta
        {
            success = await _accountService.Withdraw(account, transactionDTO.Amount);
            if (!success)
            {
                throw new Exception("Fondos insuficientes");
            }

        }
        else if (transactionDTO.TransactionType == Type.Deposito)
        {
            success = await _accountService.Deposit(account, transactionDTO.Amount);
        }
        else
        {
            throw new Exception("Tipo de transaccion no valido");
        }

        var transaction = new Transaction
        {
            TransactionType = transactionDTO.TransactionType,
            Amount = transactionDTO.Amount,
            IdAccount = account.Id,
            RemainingBalance = account.Balance
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return new GetTransactionDto
        {
            AccountNumber = account.AccountNumber,
            TransactionType = transaction.TransactionType,
            Amount = transaction.Amount,
            RemainingBalance = transaction.RemainingBalance
        };
    }

    public async Task<SummaryTransactionDto> GetTransactionByAccountNumber(string accountNumber)
    {
        var IdAccount = await _accountService.GetIdByNumberAccount(accountNumber);
        if (IdAccount == null)
        {
            throw new Exception("Account not found");
        }
        var transactionList = await _context.Transactions
            .Where(t => t.IdAccount == IdAccount)
            .ToListAsync();

        decimal calculatedFinalBalance = 0m;
        bool firstTransaction = false;

        if (transactionList.Any())
        {
            foreach (var transaction in transactionList)
            {
                if (!firstTransaction)
                {
                    calculatedFinalBalance = transaction.RemainingBalance;
                    firstTransaction = true;
                }
                else
                {
                    if (transaction.TransactionType == Type.Deposito)
                    {
                        calculatedFinalBalance += transaction.Amount;
                    }
                    else if (transaction.TransactionType == Type.Retiro)
                    {
                        calculatedFinalBalance -= transaction.Amount;
                    }
                }
            }
        }
        else //Si no hay transacciones leer el saldo de apertura que tiene la cuenta.
        {
            calculatedFinalBalance = await _accountService.CheckBalance(accountNumber);
        }

        var transaccionDto = transactionList.Select(t => new GetTransactionDto
        {
            IdTransaction = t.Id,
            Amount = t.Amount,
            TransactionType = t.TransactionType,
            RemainingBalance = t.RemainingBalance,
            AccountNumber = accountNumber
        }).ToList();

        return new SummaryTransactionDto
        {
            Transactions = transaccionDto,
            FinalBalance = calculatedFinalBalance
        };
    }
}