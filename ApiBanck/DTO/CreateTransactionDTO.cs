public class CreateTransactionDto
{
    public Type TransactionType{ get; set; }
    public decimal Amount { get; set; }
    public string AccountNumber { get; set; }
}