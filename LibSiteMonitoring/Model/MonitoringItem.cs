namespace LibSiteMonitoring.Model
{
  public class MonitoringItem
  {
    public string itemId { get; set; }
    public string itemTitle { get; set; }
    public string itemUrlPc { get; set; }
    public string itemUrlMobile { get; set; }
    public int itemPrice { get; set; }
    public System.DateTime itemDate { get; set; }
  }

  public class ExceptedItem
  {
    public MonitoringItem item { get; set; }
    public string exceptWord { get; set; }
  }
}
