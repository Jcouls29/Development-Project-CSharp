namespace Interview.Web.Models
{
    /// <summary>
    /// class for base model
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// created Date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Updated Date
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Updated By
        /// </summary>
        public string UpdatedBy { get; set; }
    }
}
