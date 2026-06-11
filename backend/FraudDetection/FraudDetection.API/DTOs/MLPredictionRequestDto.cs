public class MLPredictionRequestDto
{
    public decimal Amount { get; set; }
    public int IsForeignTransaction { get; set; }
    public int IsNightTransaction { get; set; }
    public int Hour { get; set; }
    public int IsNonCadCurrency { get; set; }
}
