// Puedes crear este archivo en una carpeta como 'TuProyectoAPI/Dtos/AccountSummaryDto.cs'
public class SummaryTransactionDto
{
    public List<GetTransactionDto> Transactions { get; set; }
    public decimal FinalBalance { get; set; }
}