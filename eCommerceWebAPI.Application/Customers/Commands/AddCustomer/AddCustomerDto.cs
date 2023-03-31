namespace eCommerceWebAPI.Application.Customers.Commands.AddCustomer
{
    public class AddCustomerDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? NationalCode { get; set; }
    }
}
