namespace eCommerceWebAPI.Application.Customers.Commands.EditCustomer
{
    public class EditCustomerDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? NationalCode { get; set; }
    }
}
