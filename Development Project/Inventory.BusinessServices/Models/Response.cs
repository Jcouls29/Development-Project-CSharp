namespace Inventory.BusinessServices
{
    public class Response
    {
        //Master data, separate table
       public int StatusCode { get; set; }
       public string StatusMessage { get; set; }
       public string StatusDescription { get; set; }

    }
}