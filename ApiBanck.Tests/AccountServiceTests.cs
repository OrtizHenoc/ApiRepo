using Xunit;
using Moq;
using ApiBanck.Services;
using ApiBanck.Models;
using System.Threading.Tasks;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _accountService = new AccountService(_mockAccountRepository.Object); 
    }

    [Fact]
    public async Task CreateAccount_ShouldCreateNewAccountAndCallRepositoryAdd()
    {

        var accountNumber = "ACC001";
        var initialBalance = 100m;
        var clientId = 1;

        _mockAccountRepository.Setup(repo => repo.Add(It.IsAny<Account>())).Returns(Task.CompletedTask);
        _mockAccountRepository.Setup(repo => repo.GetByAccountNumber(accountNumber)).ReturnsAsync((Account)null); 


        var createdAccount = await _accountService.CreateAccount(accountNumber, initialBalance, clientId);


        Assert.NotNull(createdAccount);
        Assert.Equal(accountNumber, createdAccount.AccountNumber);
        Assert.Equal(initialBalance, createdAccount.Balance);
        Assert.Equal(clientId, createdAccount.IdClient);
        _mockAccountRepository.Verify(repo => repo.Add(It.IsAny<Account>()), Times.Once());
    }

    [Fact]
    public async Task CreateAccount_ShouldThrowException_WhenAccountAlreadyExists()
    {
        
        var accountNumber = "ACC002";
        var existingAccount = new Account { Id = 1, AccountNumber = accountNumber, Balance = 500m };
        _mockAccountRepository.Setup(repo => repo.GetByAccountNumber(accountNumber)).ReturnsAsync(existingAccount);

        
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _accountService.CreateAccount(accountNumber, 100m, 1));
        Assert.Equal("Account with this number already exists.", exception.Message);
        _mockAccountRepository.Verify(repo => repo.Add(It.IsAny<Account>()), Times.Never());
    }

    [Theory]
    [InlineData("ACC003", -10.00, 1)] 
    [InlineData("ACC004", 50.00, 0)]  
    public async Task CreateAccount_ShouldThrowException_WhenInvalidInput(string accountNumber, decimal initialBalance, int clientId)
    {

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _accountService.CreateAccount(accountNumber, initialBalance, clientId));
        _mockAccountRepository.Verify(repo => repo.Add(It.IsAny<Account>()), Times.Never());
    }


    [Fact]
    public async Task ApplyInterest_ShouldIncreaseBalanceByInterestRate()
    {
        
        var accountId = 10;
        var initialBalance = 1000m;
        var interestRate = 0.05m; 
        var account = new Account { Id = accountId, AccountNumber = "ACC010", Balance = initialBalance };

        _mockAccountRepository.Setup(repo => repo.GetById(accountId)).ReturnsAsync(account);
        _mockAccountRepository.Setup(repo => repo.Update(It.IsAny<Account>())).Returns(Task.CompletedTask);

       
        await _accountService.ApplyInterest(accountId, interestRate);

        
        var expectedBalance = initialBalance * (1 + interestRate);
        Assert.Equal(expectedBalance, account.Balance);
        _mockAccountRepository.Verify(repo => repo.Update(account), Times.Once());
    }

    [Fact]
    public async Task ApplyInterest_ShouldThrowException_WhenAccountNotFound()
    {
        
        var accountId = 999;
        var interestRate = 0.01m;
        _mockAccountRepository.Setup(repo => repo.GetById(accountId)).ReturnsAsync((Account)null); 

        
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _accountService.ApplyInterest(accountId, interestRate));
        _mockAccountRepository.Verify(repo => repo.Update(It.IsAny<Account>()), Times.Never());
    }

    [Fact]
    public async Task ApplyInterest_ShouldNotChangeBalance_ForNegativeInterestRate()
    {
        
        var accountId = 11;
        var initialBalance = 500m;
        var interestRate = -0.02m;
        var account = new Account { Id = accountId, AccountNumber = "ACC011", Balance = initialBalance };

        _mockAccountRepository.Setup(repo => repo.GetById(accountId)).ReturnsAsync(account);
        _mockAccountRepository.Setup(repo => repo.Update(It.IsAny<Account>())).Returns(Task.CompletedTask);

        await _accountService.ApplyInterest(accountId, interestRate);

        Assert.Equal(initialBalance, account.Balance);
        _mockAccountRepository.Verify(repo => repo.Update(It.IsAny<Account>()), Times.Once());
    }
    

    [Fact]
    public async Task GetAccountBalance_ShouldReturnCorrectBalance()
    {
        
        var accountNumber = "ACC006";
        var expectedBalance = 750m;
        var account = new Account { Id = 1, AccountNumber = accountNumber, Balance = expectedBalance };

        _mockAccountRepository.Setup(repo => repo.GetByAccountNumber(accountNumber)).ReturnsAsync(account);

        
        var balance = await _accountService.GetAccountBalance(accountNumber);

        
        Assert.Equal(expectedBalance, balance);
    }

    [Fact]
    public async Task GetAccountBalance_ShouldThrowException_WhenAccountNotFound()
    {
        
        var accountNumber = "NONEXISTENT";
        _mockAccountRepository.Setup(repo => repo.GetByAccountNumber(accountNumber)).ReturnsAsync((Account)null);

        
        await Assert.ThrowsAsync<Exception>(() =>
            _accountService.GetAccountBalance(accountNumber));
    }
}