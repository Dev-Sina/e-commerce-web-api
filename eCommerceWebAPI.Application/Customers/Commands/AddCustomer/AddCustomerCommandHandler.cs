using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using System.Net;
using eCommerceWebAPI.Domain.Customers;

namespace eCommerceWebAPI.Application.Customers.Commands.AddCustomer
{
    public class AddCustomerCommandHandler : ICommandHandler<AddCustomerCommand, BaseResponse>
    {
        private readonly IRepository<Customer> _customerRepository;

        public AddCustomerCommandHandler(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<BaseResponse> Handle(AddCustomerCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var addCustomerDto = request.AddCustomerDto;
            ArgumentNullException.ThrowIfNull(nameof(addCustomerDto));

            BaseResponse response = new();

            // Bad request parameters
            if (string.IsNullOrEmpty(addCustomerDto.FirstName?.Trim())) response.Errors.Add("Customer first name could not be empty!");
            if (string.IsNullOrEmpty(addCustomerDto.LastName?.Trim())) response.Errors.Add("Customer last name could not be empty!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

            // Create customer entity
            Customer customer = new()
            {
                FirstName = addCustomerDto.FirstName!.Trim(),
                LastName = addCustomerDto.LastName!.Trim(),
                NationalCode = addCustomerDto.NationalCode,
                CreatedOnUtc = DateTime.UtcNow,
                Deleted = false
            };

            // Inserting address into the Db
            var insertedCustomer = await _customerRepository.Add(customer);

            // Check it's inserted or not
            if (insertedCustomer.Id <= 0)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Errors.Add("An error has been occured while registering customer!");
                return response;
            }

            // Return response
            response.HttpStatusCode = HttpStatusCode.Created;
            response.Message = "The customer has been registered successfully!";
            return response;
        }
    }
}
