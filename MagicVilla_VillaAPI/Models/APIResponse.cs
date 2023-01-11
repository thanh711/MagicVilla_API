using System.Net;

namespace MagicVilla_VillaAPI.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }
        private HttpStatusCode statusCode;
        public HttpStatusCode StatusCode
        {
            get => statusCode;
            set
            {
                if ((int)value >= 400)
                {
                    IsSuccess = false;
                }
                statusCode = value;
            }
        }
        public bool IsSuccess { get; set; }=true;
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }
    }
}
    