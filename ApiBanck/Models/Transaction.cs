public class Transaction
{
    public int Id { get; set; }
    public Type TransactionType { get; set; }
    public decimal Amount { get; set; }
    public decimal RemainingBalance { get; set; }

    public int IdAccount { get; set; }
    public Account Account{ get; set; }

}