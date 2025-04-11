namespace EasyDapr.Core.Exceptions
{
    public class UserFriendlyException : Exception
    {
        public int StatusCode { get; set; }

        public UserFriendlyException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
