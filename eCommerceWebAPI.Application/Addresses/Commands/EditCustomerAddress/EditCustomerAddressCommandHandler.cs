using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using System.Net;
using eCommerceWebAPI.Domain.Customers;
using eCommerceWebAPI.Domain.Addresses;
using Microsoft.EntityFrameworkCore;

namespace eCommerceWebAPI.Application.Addresses.Commands.EditCustomerAddress
{
    public class EditCustomerAddressCommandHandler : ICommandHandler<EditCustomerAddressCommand, BaseResponse>
    {
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerAddressMapping> _customerAddressMappingRepository;
        private readonly IRepository<City> _cityRepository;

        public EditCustomerAddressCommandHandler(IRepository<Address> addressRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerAddressMapping> customerAddressMappingRepository,
            IRepository<City> cityRepository)
        {
            _addressRepository = addressRepository;
            _customerRepository = customerRepository;
            _customerAddressMappingRepository = customerAddressMappingRepository;
            _cityRepository = cityRepository;
        }

        public async Task<BaseResponse> Handle(EditCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var editAddressDto = request.EditAddressDto;
            ArgumentNullException.ThrowIfNull(nameof(editAddressDto));

            BaseResponse response = new();

            // Check editing address would be existed
            var address = await _addressRepository.Get(editAddressDto.Id);
            if (address == null)
            {
                response.Errors.Add($"Address with ID of {editAddressDto.Id} is not found!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Bad request parameters
            if (string.IsNullOrEmpty(editAddressDto.FirstName?.Trim())) response.Errors.Add("Address first name could not be empty!");
            if (string.IsNullOrEmpty(editAddressDto.LastName?.Trim())) response.Errors.Add("Address last could not be empty!");
            if (string.IsNullOrEmpty(editAddressDto.StreetAddress?.Trim())) response.Errors.Add("Address's street address could not be empty!");
            if (string.IsNullOrEmpty(editAddressDto.PhoneNumber?.Trim())) response.Errors.Add("Address phone number could not be empty!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

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
                    .AsNoTracking()
                    .FirstOrDefault(cam => cam.CustomerId == request.CustomerId && cam.AddressId == editAddressDto.Id);
            if (relatedCustomerAddressMapping == null)
            {
                response.Errors.Add($"Modifying address is not related to the requested customer.");
                response.HttpStatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            // Check chosen city would be existed
            var city = await _cityRepository
                .AsQueryable()
                .AsNoTracking()
                .Include(c => c.Province)
                .ThenInclude(p => p.Country)
                .FirstOrDefaultAsync(c => c.Id == editAddressDto.CityId);
            if (city == null)
            {
                response.Errors.Add($"No city found with ID of : {editAddressDto.CityId}");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Modify address entity
            address.Title = editAddressDto.Title?.Trim();
            address.FirstName = editAddressDto.FirstName!.Trim();
            address.LastName = editAddressDto.LastName!.Trim();
            address.CountryName = city.Province?.Country?.Name ?? string.Empty;
            address.ProvinceName = city.Province?.Name ?? string.Empty;
            address.CityName = city.Name;
            address.StreetAddress = editAddressDto.StreetAddress!.Trim();
            address.PostalCode = editAddressDto.PostalCode?.Trim();
            address.PhoneNumber = editAddressDto.PhoneNumber!.Trim();

            // Updating address in the Db
            await _addressRepository.Update(address);

            // Return the response
            response.HttpStatusCode = HttpStatusCode.OK;
            response.Message = "The customer's address has been modified successfully!";
            return response;
        }
    }
}
