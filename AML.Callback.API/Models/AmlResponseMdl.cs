namespace AML.Callback.API.Models
{
    public class CustomerResponse
    {
        public string CustomerId { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class ApiResponse
    {
        public List<CustomerResponse> Responses { get; set; }
    }

    public class ApiRequest
    {
        public string customerId { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
    }
}
