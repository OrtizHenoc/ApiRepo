using Xunit;
using Moq;
using ApiBanck.Services;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class TransactionServiceTests
{
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly TransactionService _transactionService;
    private readonly Mock<IAccountService> _mockAccountService; 

    public TransactionServiceTests()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockAccountService = new Mock<IAccountService>();
        _transactionService = new TransactionService(
            _mockAccountRepository.Object,
            _mockTransactionRepository.Object,
            _mockAccountService.Object 
        );
    }



    [Fact]
    public async Task Deposit_ShouldIncreaseBalanceAndAddTransaction()
    {

        var accountNumber = "ACC001";
        var initialBalance = 100m;
        var depositAmount = 50m;
        var account = new Account { Id = 1, AccountNumber = accountNumber, Balance = initialBalance };

        _mockAccountService.Setup(s => s.GetIdByNumberAccount(accountNumber)).ReturnsAsync(account.Id);
        _mockAccountRepository.Setup(repo => repo.GetById(account.Id)).ReturnsAsync(account);
        _mockAccountRepository.Setup(repo => repo.Update(It.IsAny<Account>())).Returns(Task.CompletedTask);
        _mockTransactionRepository.Setup(repo => repo.Add(It.IsAny<Transaction>())).Returns(Task.CompletedTask);

        var newBalance = await _transactionService.Deposit(accountNumber, depositAmount);

        Assert.Equal(initialBalance + depositAmount, newBalance);
        Assert.Equal(initialBalance + depositAmount, account.Balance); 
        _mockAccountRepository.Verify(repo => repo.Update(account), Times.Once());
        _mockTransactionRepository.Verify(repo => repo.Add(It.Is<Transaction>(t =>
            t.Amount == depositAmount &&
            t.TransactionType == "Deposito" && 
            t.RemainingBalance == newBalance &&
            t.IdAccount == account.Id
        )), Times.Once());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Deposit_ShouldThrowArgumentException_ForNonPositiveAmount(decimal amount)
    {
        
        var accountNumber = "ACC002";
    

        
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _transactionService.Deposit(accountNumber, amount));
        _mockAccountRepository.Verify(repo => repo.Update(It.IsAny<Account>()), Times.Never());
        _mockTransactionRepository.Verify(repo => repo.Add(It.IsAny<Transaction>()), Times.Never());
    }

    [Fact]
    public async Task Deposit_ShouldThrowException_WhenAccountNotFound()
    {
        
        var accountNumber = "NONEXISTENT";
        _mockAccountService.Setup(s => s.GetIdByNumberAccount(accountNumber)).ReturnsAsync((int?)null); // Simula cuenta no encontrada

        await Assert.ThrowsAsync<Exception>(() =>
            _transactionService.Deposit(accountNumber, 100m));
        _mockAccountRepository.Verify(repo => repo.Update(It.IsAny<Account>()), Times.Never());
        _mockTransactionRepository.Verify(repo => repo.Add(It.IsAny<Transaction>()), Times.Never());
    }


    [Fact]
    public async Task Withdraw_ShouldDecreaseBalanceAndAddTransaction()
    {
        var accountNumber = "ACC003";
        var initialBalance = 200m;
        var withdrawAmount = 75m;
        var account = new Account { Id = 2, AccountNumber = accountNumber, Balance = initialBalance };

        _mockAccountService.Setup(s => s.GetIdByNumberAccount(accountNumber)).ReturnsAsync(account.Id);
        _mockAccountRepository.Setup(repo => repo.GetById(account.Id)).ReturnsAsync(account);
        _mockAccountRepository.Setup(repo => repo.Update(It.IsAny<Account>())).Returns(Task.CompletedTask);
        _mockTransactionRepository.Setup(repo => repo.Add(It.IsAny<Transaction>())).Returns(Task.CompletedTask);

        var newBalance = await _transactionService.Withdraw(accountNumber, withdrawAmount);

        Assert.Equal(initialBalance - withdrawAmount, newBalance);
        Assert.Equal(initialBalance - withdrawAmount, account.Balance);
        _mockAccountRepository.Verify(repo => repo.Update(account), Times.Once());
        _mockTransactionRepository.Verify(repo => repo.Add(It.Is<Transaction>(t =>
            t.Amount == withdrawAmount &&
            t.RemainingBalance == newBalance &&
            t.IdAccount == account.Id
        )), Times.Once());
    }

    [Fact]
    public async Task Withdraw_ShouldThrowInvalidOperationException_WhenInsufficientFunds()
    {
        var accountNumber = "ACC004";
        var initialBalance = 50m;
        var account = new Account { Id = 3, AccountNumber = accountNumber, Balance = initialBalance };

        _mockAccountService.Setup(s => s.GetIdByNumberAccount(accountNumber)).ReturnsAsync(account.Id);
        _mockAccountRepository.Setup(repo => repo.GetById(account.Id)).ReturnsAsync(account);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _transactionService.Withdraw(accountNumber, withdrawAmount));
        _mockAccountRepository.Verify(repo => repo.Update(It.IsAny<Account>()), Times.Never());
        _mockTransactionRepository.Verify(repo => repo.Add(It.IsAny<Transaction>()), Times.Never());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Withdraw_ShouldThrowArgumentException_ForNonPositiveAmount(decimal amount)
    {
        var accountNumber = "ACC005";

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _transactionService.Withdraw(accountNumber, amount));
        _mockAccountRepository.Verify(repo => repo.Update(It.IsAny<Account>()), Times.Never());
        _mockTransactionRepository.Verify(repo => repo.Add(It.IsAny<Transaction>()), Times.Never());
    }


    [Fact]
    public async Task GetTransactionsAndFinalBalance_ShouldReturnCorrectSummary()
    {
        var accountNumber = "ACC007";
        var accountId = 4;
        var transactions = new List<Transaction>
        {
            new Transaction { Id = 1, IdAccount = accountId, Amount = 100m, TransactionType = "Deposito", RemainingBalance = 100m, TransactionDate = DateTime.Now.AddDays(-2) },
            new Transaction { Id = 2, IdAccount = accountId, Amount = 20m, TransactionType = "Retiro", RemainingBalance = 80m, TransactionDate = DateTime.Now.AddDays(-1) },
            new Transaction { Id = 3, IdAccount = accountId, Amount = 150m, TransactionType = "Deposito", RemainingBalance = 230m, TransactionDate = DateTime.Now }
        };

        _mockAccountService.Setup(s => s.GetIdByNumberAccount(accountNumber)).ReturnsAsync(accountId);
        _mockTransactionRepository.Setup(repo => repo.GetByAccountId(accountId)).ReturnsAsync(transactions);

        var summary = await _transactionService.GetTransactionsAndFinalBalance(accountNumber);

        Assert.NotNull(summary);
        Assert.Equal(3, summary.Transactions.Count);
    }

    [Fact]
    public async Task GetTransactionsAndFinalBalance_ShouldReturnEmptyListAndZeroBalance_WhenNoTransactions()
    {
        var accountNumber = "ACC008";
        var accountId = 5;

        _mockAccountService.Setup(s => s.GetIdByNumberAccount(accountNumber)).ReturnsAsync(accountId);

        var summary = await _transactionService.GetTransactionsAndFinalBalance(accountNumber);

        Assert.NotNull(summary);
        Assert.Empty(summary.Transactions);
        Assert.Equal(0m, summary.FinalBalance);
    }

}