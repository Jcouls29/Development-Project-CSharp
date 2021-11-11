namespace Interview.Web.Core.DomainModel
{
    public class ResponseMessage
    {
        /// <summary>
        /// HttpStatusCode for Error
        /// </summary>
        public int ReturnCode { get; set; }

        /// <summary>
        /// Error text
        /// </summary>
        public string ReturnText { get; set; }
    }
}
