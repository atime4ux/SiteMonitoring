namespace LibSiteMonitoring.Model
{
  public class MonitoringItem
  {
    public string ItemId { get; set; }
    public string ItemTitle { get; set; }
    public string ItemUrlPc { get; set; }
    public string ItemUrlMobile { get; set; }
    public int ItemPrice { get; set; }
    public System.DateTime ItemDate { get; set; }
  }

  public class ExceptedItem
  {
    public MonitoringItem Item { get; set; }
    public string ExceptWord { get; set; }
  }
}
