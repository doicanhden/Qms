namespace Vido.Parking
{
  using Vido.Qms;

  public class Export : IExport
  {
    public string FirstImage { get; set; }
    public string SecondImage { get; set; }
    public double Amount { get; set; }
  }
}
