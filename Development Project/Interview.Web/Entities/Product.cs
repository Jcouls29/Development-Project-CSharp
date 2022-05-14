namespace Interview.Web.Entities
{
    //EVAL: I created this first just to be sure I could pull data from the database and see it in the browser.

    public class Product
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }        
    }
}
