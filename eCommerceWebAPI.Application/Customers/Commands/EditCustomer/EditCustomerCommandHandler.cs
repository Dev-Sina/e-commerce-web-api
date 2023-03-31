using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using System.Net;
using eCommerceWebAPI.Domain.Customers;

namespace eCommerceWebAPI.Application.Customers.Commands.EditCustomer
{
    public class EditCustomerCommandHandler : ICommandHandler<EditCustomerCommand, BaseResponse>
    {
        private readonly IRepository<Customer> _customerRepository;

        public EditCustomerCommandHandler(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<BaseResponse> Handle(EditCustomerCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var editCustomerDto = request.EditCustomerDto;
            ArgumentNullException.ThrowIfNull(nameof(editCustomerDto));

            BaseResponse response = new();

            // Check editing customer would be existed
            var customer = await _customerRepository.Get(request.CustomerId);
            if (customer == null || customer.Deleted)
            {
                response.Errors.Add($"Customer with ID of {request.CustomerId} is not found!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Bad request parameters
            if (string.IsNullOrEmpty(editCustomerDto.FirstName?.Trim())) response.Errors.Add("Customer first name could not be empty!");
            if (string.IsNullOrEmpty(editCustomerDto.LastName?.Trim())) response.Errors.Add("Customer last could not be empty!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

            // Create customer entity
            customer.FirstName = editCustomerDto.FirstName!.Trim();
            customer.LastName = editCustomerDto.LastName!.Trim();
            customer.NationalCode = editCustomerDto.NationalCode;

            // Updating customer into the Db
            await _customerRepository.Update(customer);

            // Return response
            response.HttpStatusCode = HttpStatusCode.OK;
            response.Message = "The customer info has been modified successfully!";
            return response;
        }
    }
}
