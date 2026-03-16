namespace AML.Callback.API.Models
{
    public class AmlResponseMdl
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }
}

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
