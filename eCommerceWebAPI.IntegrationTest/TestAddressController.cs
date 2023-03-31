using AngleSharp.Common;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.Addresses;
using eCommerceWebAPI.Application.Addresses.Commands.AddCustomerAddress;
using eCommerceWebAPI.Application.Addresses.Commands.EditCustomerAddress;
using eCommerceWebAPI.IntegrationTest.InfraStructure;
using eCommerceWebAPI.IntegrationTest.MockObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Net;
using System.Web;

namespace eCommerceWebAPI.IntegrationTest
{
    [TestFixture]
    public class TestAddressController : BaseControllerTest
    {
        public TestAddressController()
        {
            BaseRoute = "address";
        }

        #region CreateCustomerAddress

        [Test]
        public void TestCreateCustomerAddress()
        {
            var countries = SampleTestEntities.GetSampleCountries().Result;
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            Database.Countries.AddRange(countries);
            Database.Customers.Add(customer);
            Database.SaveChanges();

            var rnd = new Random();
            var booleanList = new List<bool>(2) { false, true };

            var address = SampleTestEntities.GetSampleAddress().Result;
            var allCities = countries.SelectMany(c => c.Provinces).SelectMany(p => p.Cities).ToList();
            var city = allCities.GetItemByIndex(rnd.Next(0, allCities.Count));
            AddAddressDto addAddressDto = new()
            {
                Title = address.Title,
                FirstName = address.FirstName,
                LastName = address.LastName,
                CityId = city.Id,
                StreetAddress = address.StreetAddress,
                PostalCode = address.PostalCode,
                PhoneNumber = address.PhoneNumber
            };

            var response = Client.PostAsJsonAsync(GetBaseRoute($"customer/{customer.Id}"), addAddressDto).Result;
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var foundAddress = Database
                .CustomerAddressMappings
                .AsQueryable()
                .AsNoTracking()
                .Include(cam => cam.Address)
                .Where(cam => cam.CustomerId == customer.Id)
                .Select(cam => cam.Address)
                .FirstOrDefault();
            foundAddress.Should().NotBeNull();
            foundAddress.Id.Should().BeGreaterThanOrEqualTo(1);
            foundAddress.Title.Should().Be(address.Title);
            foundAddress.FirstName.Should().Be(address.FirstName);
            foundAddress.LastName.Should().Be(address.LastName);
            foundAddress.CountryName.Should().Be(city.Province.Country.Name);
            foundAddress.ProvinceName.Should().Be(city.Province.Name);
            foundAddress.CityName.Should().Be(city.Name);
            foundAddress.StreetAddress.Should().Be(address.StreetAddress);
            foundAddress.PostalCode.Should().Be(address.PostalCode);
            foundAddress.PhoneNumber.Should().Be(address.PhoneNumber);
        }

        #endregion

        #region EditCustomerAddress

        [Test]
        public void TestEditCustomerAddress()
        {
            var countries = SampleTestEntities.GetSampleCountries().Result;
            var addresses = SampleTestEntities.GetSampleAddresses(40).Result;
            Database.Countries.AddRange(countries);
            Database.Addresses.AddRange(addresses);
            Database.SaveChanges();

            var customers = SampleTestEntities.GetSampleCustomers(addresses: addresses).Result;
            Database.Customers.AddRange(customers);
            Database.SaveChanges();

            var rnd = new Random();

            var chosenCustomer = customers.GetItemByIndex(rnd.Next(0, customers.Count));
            var chosenCustomerAddressMappings = chosenCustomer.CustomerAddressMappings.ToList();
            var chosenAddressId = chosenCustomerAddressMappings.GetItemByIndex(rnd.Next(0, chosenCustomerAddressMappings.Count)).AddressId;
            var chosenAddress = addresses.FirstOrDefault(a => a.Id == chosenAddressId);
            chosenAddress.Should().NotBeNull();

            var updatingAddress = SampleTestEntities.GetSampleAddress().Result;
            var allCities = countries.SelectMany(c => c.Provinces).SelectMany(p => p.Cities).ToList();
            var city = allCities.GetItemByIndex(rnd.Next(0, allCities.Count));
            EditAddressDto editAddressDto = new()
            {
                Id = chosenAddressId,
                Title = updatingAddress.Title,
                FirstName = updatingAddress.FirstName,
                LastName = updatingAddress.LastName,
                CityId = city.Id,
                StreetAddress = updatingAddress.StreetAddress,
                PostalCode = updatingAddress.PostalCode,
                PhoneNumber = updatingAddress.PhoneNumber
            };

            var response = Client.PutAsJsonAsync(GetBaseRoute($"customer/{chosenCustomer.Id}"), editAddressDto).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var foundAddress = Database.Addresses.AsQueryable().AsNoTracking().FirstOrDefault(a => a.Id == chosenAddressId);
            foundAddress.Should().NotBeNull();
            foundAddress.Id.Should().BeGreaterThanOrEqualTo(1);
            foundAddress.Title.Should().Be(editAddressDto.Title);
            foundAddress.FirstName.Should().Be(editAddressDto.FirstName);
            foundAddress.LastName.Should().Be(editAddressDto.LastName);
            foundAddress.CountryName.Should().Be(city.Province.Country.Name);
            foundAddress.ProvinceName.Should().Be(city.Province.Name);
            foundAddress.CityName.Should().Be(city.Name);
            foundAddress.StreetAddress.Should().Be(editAddressDto.StreetAddress);
            foundAddress.PostalCode.Should().Be(editAddressDto.PostalCode);
            foundAddress.PhoneNumber.Should().Be(editAddressDto.PhoneNumber);
        }

        #endregion

        #region DeleteCustomerAddress

        [Test]
        public void TestDeleteCustomerAddress()
        {
            var countries = SampleTestEntities.GetSampleCountries().Result;
            var addresses = SampleTestEntities.GetSampleAddresses(40).Result;
            Database.Countries.AddRange(countries);
            Database.Addresses.AddRange(addresses);
            Database.SaveChanges();

            var customers = SampleTestEntities.GetSampleCustomers(addresses: addresses).Result;
            Database.Customers.AddRange(customers);
            Database.SaveChanges();

            var rnd = new Random();

            var chosenCustomer = customers.GetItemByIndex(rnd.Next(0, customers.Count));
            var chosenCustomerAddressMappings = chosenCustomer.CustomerAddressMappings.ToList();
            var chosenAddressId = chosenCustomerAddressMappings.GetItemByIndex(rnd.Next(0, chosenCustomerAddressMappings.Count)).AddressId;
            var chosenAddress = addresses.FirstOrDefault(a => a.Id == chosenAddressId);
            chosenAddress.Should().NotBeNull();

            var response = Client.DeleteAsJsonAsync(GetBaseRoute($"customer/{chosenCustomer.Id}"), chosenAddressId).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var foundAddress = Database
                .CustomerAddressMappings
                .AsQueryable()
                .AsNoTracking()
                .FirstOrDefault(a =>
                    a.CustomerId == chosenCustomer.Id &&
                    a.AddressId == chosenAddressId);
            foundAddress.Should().BeNull();
        }

        #endregion

        #region GetAllCustomerAddresses

        [Test]
        public void TestGetAllCustomerAddresses_WithoutQueryParameters()
        {
            var addresses = SampleTestEntities.GetSampleAddresses(40).Result;
            Database.Addresses.AddRange(addresses);
            Database.SaveChanges();

            var customers = SampleTestEntities.GetSampleCustomers(addresses: addresses).Result;
            Database.Customers.AddRange(customers);
            Database.SaveChanges();

            var rnd = new Random();
            var booleanList = new List<bool>(2) { false, true };

            var chosenCustomer = customers.GetItemByIndex(rnd.Next(0, customers.Count));

            var response = Client.GetAsync(GetBaseRoute($"customer/{chosenCustomer.Id}")).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<AddressDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var data = responseContent.Data;
            data.Should().NotBeNull();
            data.Should().HaveCountGreaterThanOrEqualTo(1);

            var chosenCustomerAddressIds = chosenCustomer
                .CustomerAddressMappings
                .Select(cam => cam.AddressId)
                .ToList();
            var chosenCustomerAddresses = addresses.Where(a => chosenCustomerAddressIds.Contains(a.Id)).ToList();
            chosenCustomerAddresses.Should().NotBeNullOrEmpty();

            var foundAddresses = Database
                .CustomerAddressMappings
                .AsQueryable()
                .AsNoTracking()
                .Include(cam => cam.Address)
                .Where(cam => cam.CustomerId == chosenCustomer.Id)
                .Select(cam => cam.Address)
                .ToList();
            foundAddresses.Should().NotBeNullOrEmpty();
            foundAddresses.Count.Should().Be(chosenCustomerAddresses.Count);

            foreach (var address in foundAddresses)
            {
                var relatedChosenCustomerAddress = chosenCustomerAddresses.FirstOrDefault(a => a.FirstName == address.FirstName);
                relatedChosenCustomerAddress.Should().NotBeNull();
                relatedChosenCustomerAddress.Id.Should().Be(address.Id);
                relatedChosenCustomerAddress.FirstName.Should().Be(address.FirstName);
                relatedChosenCustomerAddress.LastName.Should().Be(address.LastName);
                relatedChosenCustomerAddress.CountryName.Should().Be(address.CountryName);
                relatedChosenCustomerAddress.ProvinceName.Should().Be(address.ProvinceName);
                relatedChosenCustomerAddress.CityName.Should().Be(address.CityName);
                relatedChosenCustomerAddress.StreetAddress.Should().Be(address.StreetAddress);
                relatedChosenCustomerAddress.PostalCode.Should().Be(address.PostalCode);
                relatedChosenCustomerAddress.PhoneNumber.Should().Be(address.PhoneNumber);
            }
        }

        [Test]
        public void TestGetAllCustomerAddresses_WithQueryParameters()
        {
            var addresses = SampleTestEntities.GetSampleAddresses(40).Result;
            Database.Addresses.AddRange(addresses);
            Database.SaveChanges();

            var customers = SampleTestEntities.GetSampleCustomers(addresses: addresses).Result;
            Database.Customers.AddRange(customers);
            Database.SaveChanges();

            var rnd = new Random();
            var booleanList = new List<bool>(2) { false, true };

            var chosenCustomer = customers.GetItemByIndex(rnd.Next(0, customers.Count));

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("pageNumber", $"{1}");
            queryString.Add("pageSize", $"{1}");

            var response = Client.GetAsync(GetBaseRoute($"customer/{chosenCustomer.Id}" + "?" + queryString)).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<AddressDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var data = responseContent.Data;
            data.Should().NotBeNull();
            data.Count.Should().Be(1);

            var dataAddressDto = data.FirstOrDefault();
            dataAddressDto.Should().NotBeNull();
            dataAddressDto.Id.Should().BeGreaterThanOrEqualTo(1);

            var relatedAddress = addresses.FirstOrDefault(a => a.Id == dataAddressDto.Id);
            relatedAddress.Should().NotBeNull();
            dataAddressDto.FirstName.Should().Be(relatedAddress.FirstName);
            dataAddressDto.LastName.Should().Be(relatedAddress.LastName);
            dataAddressDto.CountryName.Should().Be(relatedAddress.CountryName);
            dataAddressDto.ProvinceName.Should().Be(relatedAddress.ProvinceName);
            dataAddressDto.CityName.Should().Be(relatedAddress.CityName);
            dataAddressDto.StreetAddress.Should().Be(relatedAddress.StreetAddress);
            dataAddressDto.PostalCode.Should().Be(relatedAddress.PostalCode);
            dataAddressDto.PhoneNumber.Should().Be(relatedAddress.PhoneNumber);
        }

        [Test]
        public void TestGetAllCustomerAddresses_WithoutQueryParameters_NoAddresses()
        {
            var customers = SampleTestEntities.GetSampleCustomers().Result;
            Database.Customers.AddRange(customers);
            Database.SaveChanges();

            var rnd = new Random();
            var booleanList = new List<bool>(2) { false, true };

            var chosenCustomer = customers.GetItemByIndex(rnd.Next(0, customers.Count));

            var response = Client.GetAsync(GetBaseRoute($"customer/{chosenCustomer.Id}")).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<AddressDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Failed");

            var data = responseContent.Data;
            data.Should().BeNullOrEmpty();
        }

        [Test]
        public void TestGetAllCustomerAddresses_WithQueryParameters_NoAddresses()
        {
            var customers = SampleTestEntities.GetSampleCustomers().Result;
            Database.Customers.AddRange(customers);
            Database.SaveChanges();

            var rnd = new Random();
            var booleanList = new List<bool>(2) { false, true };

            var chosenCustomer = customers.GetItemByIndex(rnd.Next(0, customers.Count));

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("pageNumber", $"{1}");
            queryString.Add("pageSize", $"{1}");

            var response = Client.GetAsync(GetBaseRoute($"customer/{chosenCustomer.Id}" + "?" + queryString)).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<AddressDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Failed");

            var data = responseContent.Data;
            data.Should().BeNullOrEmpty();
        }

        #endregion
    }
}