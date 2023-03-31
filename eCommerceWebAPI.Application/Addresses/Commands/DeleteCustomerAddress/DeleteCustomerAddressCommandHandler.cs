using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using System.Net;
using eCommerceWebAPI.Domain.Customers;
using eCommerceWebAPI.Domain.Addresses;

namespace eCommerceWebAPI.Application.Addresses.Commands.DeleteCustomerAddress
{
    public class DeleteCustomerAddressCommandHandler : ICommandHandler<DeleteCustomerAddressCommand, BaseResponse>
    {
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerAddressMapping> _customerAddressMappingRepository;

        public DeleteCustomerAddressCommandHandler(IRepository<Address> addressRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerAddressMapping> customerAddressMappingRepository)
        {
            _addressRepository = addressRepository;
            _customerRepository = customerRepository;
            _customerAddressMappingRepository = customerAddressMappingRepository;
        }

        public async Task<BaseResponse> Handle(DeleteCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            BaseResponse response = new();

            // Check editing address would be existed
            var address = await _addressRepository.Get(request.AddressId);
            if (address == null)
            {
                response.Errors.Add($"Address with ID of {request.AddressId} is not found!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Check customer would be existed
            Customer customer = await _customerRepository.Get(request.CustomerId);
            if (customer == null || customer.Deleted)
            {
                response.Errors.Add($"No customer found with ID of : {request.CustomerId}");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Check editing address would be related to the requested customer
            var relatedCustomerAddressMapping = _customerAddressMappingRepository
                    .AsQueryable()
                    .FirstOrDefault(cam => cam.CustomerId == request.CustomerId && cam.AddressId == request.AddressId);
            if (relatedCustomerAddressMapping == null)
            {
                response.Errors.Add($"Modifying address is not related to the requested customer.");
                response.HttpStatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            // Removing related customer address mapping from Db
            await _customerAddressMappingRepository.Delete(relatedCustomerAddressMapping);

            // Return the response
            response.HttpStatusCode = HttpStatusCode.OK;
            response.Message = "The customer's address has been removed successfully!";
            return response;
        }
    }
}
