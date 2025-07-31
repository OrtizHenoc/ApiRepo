public class GetTransactionDto
{
    public int IdTransaction { get; set; }
    public decimal Amount { get; set; }
    public Type TransactionType { get; set; }
    public decimal RemainingBalance { get; set; }
    public string AccountNumber { get; set; }
}
