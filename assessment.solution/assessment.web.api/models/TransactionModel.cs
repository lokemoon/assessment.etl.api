namespace assessment.web.api.models
{
  public class TransactionModel
  {
    public int Id { get; set; }
    public string AccountNum { get; set; }
    public string CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime TransactionOn { get; set; }
    public DateTime LastModifyTime { get; set; }
  }
}