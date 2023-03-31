using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using System.Net;
using eCommerceWebAPI.Domain.Customers;
using eCommerceWebAPI.Domain.Addresses;
using Microsoft.EntityFrameworkCore;

namespace eCommerceWebAPI.Application.Addresses.Commands.AddCustomerAddress
{
    public class AddCustomerAddressCommandHandler : ICommandHandler<AddCustomerAddressCommand, BaseResponse>
    {
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<City> _cityRepository;

        public AddCustomerAddressCommandHandler(IRepository<Address> addressRepository,
            IRepository<Customer> customerRepository,
            IRepository<City> cityRepository)
        {
            _addressRepository = addressRepository;
            _customerRepository = customerRepository;
            _cityRepository = cityRepository;
        }

        public async Task<BaseResponse> Handle(AddCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var addAddressDto = request.AddAddressDto;
            ArgumentNullException.ThrowIfNull(nameof(addAddressDto));

            BaseResponse response = new();

            // Bad request parameters
            if (string.IsNullOrEmpty(addAddressDto.FirstName?.Trim())) response.Errors.Add("Address first name could not be empty!");
            if (string.IsNullOrEmpty(addAddressDto.LastName?.Trim())) response.Errors.Add("Address last could not be empty!");
            if (string.IsNullOrEmpty(addAddressDto.StreetAddress?.Trim())) response.Errors.Add("Address's street address could not be empty!");
            if (string.IsNullOrEmpty(addAddressDto.PhoneNumber?.Trim())) response.Errors.Add("Address phone number could not be empty!");
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

            // Check chosen city would be existed
            var city = await _cityRepository
                .AsQueryable()
                .AsNoTracking()
                .Include(c => c.Province)
                .ThenInclude(p => p.Country)
                .FirstOrDefaultAsync(c => c.Id == addAddressDto.CityId);
            if (city == null)
            {
                response.Errors.Add($"No city found with ID of : {addAddressDto.CityId}");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Create address entity
            Address address = new()
            {
                Title = addAddressDto.Title?.Trim(),
                FirstName = addAddressDto.FirstName!.Trim(),
                LastName = addAddressDto.LastName!.Trim(),
                CountryName = city.Province?.Country?.Name ?? string.Empty,
                ProvinceName = city.Province?.Name ?? string.Empty,
                CityName = city.Name,
                StreetAddress = addAddressDto.StreetAddress!.Trim(),
                PostalCode = addAddressDto.PostalCode?.Trim(),
                PhoneNumber = addAddressDto.PhoneNumber!.Trim()
            };
            address.CustomerAddressMappings.Add(new CustomerAddressMapping { CustomerId = request.CustomerId });

            // Inserting address into the Db
            var insertedAddress = await _addressRepository.Add(address);

            // Check it's inserted or not
            if (insertedAddress.Id <= 0)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Errors.Add("An error has been occured while inserting customer address!");
                return response;
            }

            response.HttpStatusCode = HttpStatusCode.Created;
            response.Message = "The customer's address has been inserted successfully!";
            return response;
        }
    }
}
