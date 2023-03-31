using DNTCommon.Web.Core;
using eCommerceWebAPI.Domain.Addresses;
using eCommerceWebAPI.Domain.Catalogs;
using eCommerceWebAPI.Domain.Customers;
using eCommerceWebAPI.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerceWebAPI.Infrastructure.Databases
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IUnitOfWork _unitOfWork;

        public DbInitializer(IServiceScopeFactory scopeFactory,
            IUnitOfWork unitOfWork)
        {
            _scopeFactory = scopeFactory;
            _unitOfWork = unitOfWork;
        }

        public void SeedData()
        {
            //_scopeFactory.RunScopedService<CommonDataContext>(context =>
            //{
            //    context.Database.Migrate();
            //});

            //return;

            bool isDataMigratedAtFirstTime = _unitOfWork.Set<Country>().AsQueryable().AnyAsync().Result;
            if (!isDataMigratedAtFirstTime)
            {
                _scopeFactory.RunScopedService<IDbInitializer>(dbSeedData =>
                {
                    dbSeedData.SeedCountries().GetAwaiter().GetResult();
                    dbSeedData.SeedProvinces().GetAwaiter().GetResult();
                    dbSeedData.SeedCities().GetAwaiter().GetResult();
                    dbSeedData.SeedAddresses().GetAwaiter().GetResult();
                    dbSeedData.Customers().GetAwaiter().GetResult();
                    dbSeedData.SeedCategories().GetAwaiter().GetResult();
                    dbSeedData.SeedSpecifications().GetAwaiter().GetResult();
                    dbSeedData.SeedProducts().GetAwaiter().GetResult();
                });
            }
        }

        public async Task SeedCountries()
        {
            var query = _unitOfWork.Set<Country>().AsQueryable();
            var isThereAnyItem = await query.AnyAsync();
            if (!isThereAnyItem)
            {
                List<Country> items = new()
                {
                    new Country { Name = "Iran", DisplayOrder = 1 }
                };

                _unitOfWork.Set<Country>().AddRange(items);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task SeedProvinces()
        {
            var query = _unitOfWork.Set<Province>().AsQueryable();
            var isThereAnyItem = await query.AnyAsync();
            if (!isThereAnyItem)
            {
                var iranCountry = _unitOfWork.Set<Country>().AsQueryable().FirstOrDefault(c => c.Name == "Iran");
                if (iranCountry != null)
                {
                    List<Province> items = new()
                    {
                        new Province { Name = "Tehran", CountryId = iranCountry.Id, DisplayOrder = 1 },
                        new Province { Name = "Alborz", CountryId = iranCountry.Id, DisplayOrder = 2 },
                        new Province { Name = "Fars", CountryId = iranCountry.Id, DisplayOrder = 3 }
                    };

                    _unitOfWork.Set<Province>().AddRange(items);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }

        public async Task SeedCities()
        {
            var query = _unitOfWork.Set<City>().AsQueryable();
            var isThereAnyItem = await query.AnyAsync();
            if (!isThereAnyItem)
            {
                var iranCountry = _unitOfWork.Set<Country>().AsQueryable().FirstOrDefault(c => c.Name == "Iran");
                if (iranCountry != null)
                {
                    List<City> items = new();

                    var tehranProvince = _unitOfWork.Set<Province>().AsQueryable().FirstOrDefault(p => p.Name == "Tehran" && p.CountryId == iranCountry.Id);
                    if (tehranProvince != null)
                    {
                        items.Add(new City { Name = "Tehran", ProvinceId = tehranProvince.Id, DisplayOrder = 1 });
                        items.Add(new City { Name = "Shahriar", ProvinceId = tehranProvince.Id, DisplayOrder = 2 });
                        items.Add(new City { Name = "Qods", ProvinceId = tehranProvince.Id, DisplayOrder = 3 });
                        items.Add(new City { Name = "Islam Shahr", ProvinceId = tehranProvince.Id, DisplayOrder = 4 });
                    }

                    var alborzProvince = _unitOfWork.Set<Province>().AsQueryable().FirstOrDefault(p => p.Name == "Alborz" && p.CountryId == iranCountry.Id);
                    if (alborzProvince != null)
                    {
                        items.Add(new City { Name = "Karaj", ProvinceId = alborzProvince.Id, DisplayOrder = 1 });
                        items.Add(new City { Name = "hashtgerd", ProvinceId = alborzProvince.Id, DisplayOrder = 2 });
                        items.Add(new City { Name = "Mehestan", ProvinceId = alborzProvince.Id, DisplayOrder = 3 });
                        items.Add(new City { Name = "Taleqan", ProvinceId = alborzProvince.Id, DisplayOrder = 4 });
                        items.Add(new City { Name = "Mohammad Shahr", ProvinceId = alborzProvince.Id, DisplayOrder = 5 });
                    }

                    var farsProvince = _unitOfWork.Set<Province>().AsQueryable().FirstOrDefault(p => p.Name == "Fars" && p.CountryId == iranCountry.Id);
                    if (farsProvince != null)
                    {
                        items.Add(new City { Name = "Shiraz", ProvinceId = farsProvince.Id, DisplayOrder = 1 });
                        items.Add(new City { Name = "Jahrom", ProvinceId = farsProvince.Id, DisplayOrder = 2 });
                        items.Add(new City { Name = "Firuz Abad", ProvinceId = farsProvince.Id, DisplayOrder = 3 });
                        items.Add(new City { Name = "Abadeh", ProvinceId = farsProvince.Id, DisplayOrder = 4 });
                    }

                    _unitOfWork.Set<City>().AddRange(items);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }

        public async Task SeedAddresses()
        {
            var query = _unitOfWork.Set<Address>().AsQueryable();
            var isThereAnyItem = await query.AnyAsync();
            if (!isThereAnyItem)
            {
                List<Address> items = new()
                {
                    new Address { Title = "My home", FirstName = "Sina", LastName = "Bahmanpour", CountryName = "Iran", ProvinceName = "Tehran", CityName = "Tehran", StreetAddress = "Floor 2, No. 1, X alley, Y street", PostalCode = "123457890", PhoneNumber = "989196657560" }
                };

                _unitOfWork.Set<Address>().AddRange(items);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task Customers()
        {
            var query = _unitOfWork.Set<Customer>().AsQueryable();
            var isThereAnyItem = await query.AnyAsync();
            if (!isThereAnyItem)
            {
                var relatedAddress = _unitOfWork.Set<Address>().AsQueryable().FirstOrDefault(x => x.Title != null && x.Title == "My home" && x.FirstName == "Sina" && x.LastName == "Bahmanpour");

                var customerSinaBahmanpour = new Customer { FirstName = "Sina", LastName = "Bahmanpour", NationalCode = "1111100000" };
                if (relatedAddress != null) customerSinaBahmanpour.CustomerAddressMappings.Add(new() { AddressId = relatedAddress.Id });

                _unitOfWork.Set<Customer>().AddRange(customerSinaBahmanpour);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task SeedCategories()
        {
            var query = _unitOfWork.Set<Category>().AsQueryable();
            var isThereAnyItem = await query.AnyAsync();
            if (!isThereAnyItem)
            {
                var digitalCategory = new Category { Name = "Digital", DisplayOrder = 1 };
                var digitalLaptopSubcategory = new Category { Name = "Laptop", DisplayOrder = 1 };
                var digitalSmartPhoneSubcategory = new Category { Name = "Smart phone", DisplayOrder = 2 };
                var digitalAccessorySubcategory = new Category { Name = "Digital accessory", DisplayOrder = 3 };
                digitalCategory.ChildCategories.Add(digitalLaptopSubcategory);
                digitalCategory.ChildCategories.Add(digitalSmartPhoneSubcategory);
                digitalCategory.ChildCategories.Add(digitalAccessorySubcategory);

                var homeAppliancesCategory = new Category { Name = "HomeAppliances", DisplayOrder = 2 };
                var homeAppliancesRefrigeratorSubcategory = new Category { Name = "Refrigerator", DisplayOrder = 1 };
                var homeAppliancesWashingMachineSubcategory = new Category { Name = "Washing machine", DisplayOrder = 2 };
                var homeAppliancesHeaterSubcategory = new Category { Name = "Heater", DisplayOrder = 3 };
                var homeAppliancesCoolerSubcategory = new Category { Name = "Cooler", DisplayOrder = 4 };
                homeAppliancesCategory.ChildCategories.Add(homeAppliancesRefrigeratorSubcategory);
                homeAppliancesCategory.ChildCategories.Add(homeAppliancesWashingMachineSubcategory);
                homeAppliancesCategory.ChildCategories.Add(homeAppliancesHeaterSubcategory);
                homeAppliancesCategory.ChildCategories.Add(homeAppliancesCoolerSubcategory);

                var clothesCategory = new Category { Name = "Clothes", DisplayOrder = 3 };
                var clothesMenWearingSubcategory = new Category { Name = "Men wearing", DisplayOrder = 1 };
                var clothesJewelrySubcategory = new Category { Name = "Jewelry", DisplayOrder = 2 };
                clothesCategory.ChildCategories.Add(clothesMenWearingSubcategory);
                clothesCategory.ChildCategories.Add(clothesJewelrySubcategory);

                _unitOfWork.Set<Category>().AddRange(digitalCategory, homeAppliancesCategory, clothesCategory);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task SeedSpecifications()
        {
            var query = _unitOfWork.Set<Specification>().AsQueryable();
            var isThereAnyItem = await query.AnyAsync();
            if (!isThereAnyItem)
            {
                var colorSpecification = new Specification { Name = "Color", DisplayOrder = 1 };
                var colorSierraBlueSpecificationValue = new SpecificationValue { Name = "Sierra Blue", DisplayOrder = 1 };
                var colorWhiteSpecificationValue = new SpecificationValue { Name = "White", DisplayOrder = 2 };
                var colorSnowWhiteSpecificationValue = new SpecificationValue { Name = "Snow White", DisplayOrder = 3 };
                var colorBlackSpecificationValue = new SpecificationValue { Name = "Black", DisplayOrder = 4 };
                var colorRedSpecificationValue = new SpecificationValue { Name = "Red", DisplayOrder = 5 };
                colorSpecification.SpecificationValues.Add(colorSierraBlueSpecificationValue);
                colorSpecification.SpecificationValues.Add(colorWhiteSpecificationValue);
                colorSpecification.SpecificationValues.Add(colorSnowWhiteSpecificationValue);
                colorSpecification.SpecificationValues.Add(colorBlackSpecificationValue);
                colorSpecification.SpecificationValues.Add(colorRedSpecificationValue);

                var typeSpecification = new Specification { Name = "Type", DisplayOrder = 2 };
                var typeGamingSpecificationValue = new SpecificationValue { Name = "Gaming", DisplayOrder = 1 };
                var typeMultimediaSpecificationValue = new SpecificationValue { Name = "Multimedia", DisplayOrder = 2 };
                var typeCasualSpecificationValue = new SpecificationValue { Name = "Casual", DisplayOrder = 3 };
                typeSpecification.SpecificationValues.Add(typeGamingSpecificationValue);
                typeSpecification.SpecificationValues.Add(typeMultimediaSpecificationValue);
                typeSpecification.SpecificationValues.Add(typeCasualSpecificationValue);

                var brandSpecification = new Specification { Name = "Brand", DisplayOrder = 3 };
                var brandAppleSpecificationValue = new SpecificationValue { Name = "Apple", DisplayOrder = 1 };
                var brandSamsungSpecificationValue = new SpecificationValue { Name = "Samsung", DisplayOrder = 1 };
                var brandHPSpecificationValue = new SpecificationValue { Name = "HP", DisplayOrder = 1 };
                brandSpecification.SpecificationValues.Add(brandAppleSpecificationValue);
                brandSpecification.SpecificationValues.Add(brandSamsungSpecificationValue);
                brandSpecification.SpecificationValues.Add(brandHPSpecificationValue);

                var dimensionsSpecification = new Specification { Name = "Dimensions", DisplayOrder = 4 };
                var dimensionsSpecificationValue1 = new SpecificationValue { Name = "160.8 X 78.1 X 7.65 (mm)", DisplayOrder = 1 };
                var dimensionsSpecificationValue2 = new SpecificationValue { Name = "44.78 X 30.81 X 7.01 (mm)", DisplayOrder = 2 };
                dimensionsSpecification.SpecificationValues.Add(dimensionsSpecificationValue1);
                dimensionsSpecification.SpecificationValues.Add(dimensionsSpecificationValue2);

                var weightSpecification = new Specification { Name = "Weight", DisplayOrder = 5 };
                var weightSpecificationValue1 = new SpecificationValue { Name = "2,014 (gr)", DisplayOrder = 1 };
                var weightSpecificationValue2 = new SpecificationValue { Name = "240 (gr)", DisplayOrder = 2 };
                weightSpecification.SpecificationValues.Add(weightSpecificationValue1);
                weightSpecification.SpecificationValues.Add(weightSpecificationValue2);

                _unitOfWork.Set<Specification>().AddRange(colorSpecification, typeSpecification, brandSpecification, dimensionsSpecification, weightSpecification);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task SeedProducts()
        {
            var query = _unitOfWork.Set<Product>().AsQueryable();
            var isThereAnyItem = await query.AnyAsync();
            if (!isThereAnyItem)
            {
                var digitalCategory = _unitOfWork.Set<Category>().AsQueryable().FirstOrDefault(x => x.Name == "Digital");
                var digitalSmartPhoneCategory = _unitOfWork.Set<Category>().AsQueryable().FirstOrDefault(x => x.Name == "Smart phone");
                var digitalLaptopCategory = _unitOfWork.Set<Category>().AsQueryable().FirstOrDefault(x => x.Name == "Laptop");

                var sierraBlueColor = _unitOfWork.Set<SpecificationValue>().AsQueryable().FirstOrDefault(x => x.Name == "Sierra Blue");
                var snowWhiteColor = _unitOfWork.Set<SpecificationValue>().AsQueryable().FirstOrDefault(x => x.Name == "Snow White");
                var casualType = _unitOfWork.Set<SpecificationValue>().AsQueryable().FirstOrDefault(x => x.Name == "Casual");
                var appleBrand = _unitOfWork.Set<SpecificationValue>().AsQueryable().FirstOrDefault(x => x.Name == "Apple");
                var hpBrand = _unitOfWork.Set<SpecificationValue>().AsQueryable().FirstOrDefault(x => x.Name == "HP");
                var phoneDimensions = _unitOfWork.Set<SpecificationValue>().AsQueryable().FirstOrDefault(x => x.Name == "44.78 X 30.81 X 7.01 (mm)");
                var laptopDimensions = _unitOfWork.Set<SpecificationValue>().AsQueryable().FirstOrDefault(x => x.Name == "160.8 X 78.1 X 7.65 (mm)");
                var phoneWeight = _unitOfWork.Set<SpecificationValue>().AsQueryable().FirstOrDefault(x => x.Name == "240 (gr)");
                var laptopWeight = _unitOfWork.Set<SpecificationValue>().AsQueryable().FirstOrDefault(x => x.Name == "2,014 (gr)");

                var iPhone13ProMax = new Product
                {
                    Name = "Refurbished iPhone 13 Pro Max 1TB - Sierra Blue (Unlocked)",
                    Code = "FLL53LL/A",
                    ShortDescription = "A fast and powerful chip.\r\nThe lightning-fast A15 Bionic powers Cinematic mode, Photographic Styles, Live Text, and more. Its Secure Enclave locks down personal info like your Face ID data and contacts. And it’s more efficient, helping to deliver longer battery life.",
                    FullDescription = "Great battery life.\r\nDo more on a single charge thanks to the powerfully efficient A15 Bionic chip. Up to 28 hours of video playback on iPhone 13 Pro Max. Smart data mode switches from 5G to LTE when you don’t need 5G speeds, conserving battery life.\r\n\r\nA flawless flat-edge design.\r\nThe flat-edge design makes iPhone even more durable and allows us to take the display right to the very edge. iPhone 13 Pro Max has a Ceramic Shield front, Textured matte glass back and stainless steel design.\r\n\r\nPro 12MP camera system: Telephoto, Wide, and Ultra Wide cameras.\r\nA dramatically more powerful camera system. Our Pro camera system gets its biggest upgrade ever. With next-level hardware that captures so much more detail. Superintelligent software for new photo and filmmaking techniques. And a mind-blowingly fast chip that makes it all possible. It’ll change the way you shoot.\r\n\r\nSuperfast 5G.\r\nThe world is quickly moving to 5G. Streaming, downloading — everything happens so much faster. 5G is even fast enough for serious multiplayer gaming, sharing AR videos, and more.\r\n\r\nA remarkably durable Ceramic Shield front.\r\nCeramic Shield is tougher than any smartphone glass. It’s made by introducing nano-ceramic crystals - which are harder than most metals - into glass for far greater durability. Our dual-ion exchange process also protects against nicks, scratches, and everyday wear and tear.\r\n\r\nA bright, beautiful OLED display with ProMotion technology.\r\nThe new Super Retina XDR display with ProMotion can refresh from 10 to 120 times per second, and all kinds of frame rates in between. It intelligently ramps up when you need exceptional graphics performance, and ramps down to save power when you don’t. It even accelerates and decelerates naturally to match the speed of your finger as you scroll. You’ve never felt anything like it.",
                    Price = 78900000,
                    DiscountAmount = 1350000,
                    StockQuantity = 9,
                    Currency = "IRT"
                };
                if (digitalCategory != null) iPhone13ProMax.ProductCategoryMappings.Add(new() { CategoryId = digitalCategory.Id, DisplayOrder = 1 });
                if (digitalSmartPhoneCategory != null) iPhone13ProMax.ProductCategoryMappings.Add(new() { CategoryId = digitalSmartPhoneCategory.Id, DisplayOrder = 2 });
                if (sierraBlueColor != null) iPhone13ProMax.ProductSpecificationValueMappings.Add(new() { SpecificationValueId = sierraBlueColor.Id });
                if (appleBrand != null) iPhone13ProMax.ProductSpecificationValueMappings.Add(new() { SpecificationValueId = appleBrand.Id });
                if (phoneDimensions != null) iPhone13ProMax.ProductSpecificationValueMappings.Add(new() { SpecificationValueId = phoneDimensions.Id });
                if (phoneWeight != null) iPhone13ProMax.ProductSpecificationValueMappings.Add(new() { SpecificationValueId = phoneWeight.Id });

                var hpLaptopProMax = new Product
                {
                    Name = "2022 HP Stream 14inch HD Laptop",
                    Code = "32MTk2NTQ4NDMw5",
                    ShortDescription = "2022 HP Stream 14inch HD Laptop, Intel Celeron N4020 Dual-Core Processor, 4GB DDR4 Memory, 128GB Storage(64GB eMMC+64GB Card),WiFi,Webcam,Bluetooth,1-Year Microsoft 365,Win10 S,Snow White|TGCD Bundle",
                    FullDescription = "How to switch out of S mode in Windows\r\n1. On your PC running Windows 10 in S mode, open Settings > Update & Security > Activation. 2. In the Switch to Windows 10/11 Home or Switch to Windows 10 Pro section, select Go to the Store. (If you also see an \"Upgrade your edition of Windows\" section, be careful not to click the \"Go to the Store\" link that appears there.) 3. On the Switch out of S mode (or similar) page that appears in the Microsoft Store, select the Get button. After you see a confirmation message on the page, you'll be able to install apps from outside of the Microsoft Store.\r\n\r\n\r\nCapacity:Snow White/N4020/64GB eMMC/64GB memory card/14 inch\r\n\r\n✔Intel Celeron N4020 (1.1 GHz base frequency, up to 2.8 GHz burst frequency, 4 MB L2 cache, 2 cores),\r\n\r\n✔4 GB DDR4-2400 SDRAM (onboard), 64GB eMMC + 64GB Card Total 128GB Storage\r\n\r\n✔14\" HD (1366 x 768) SVA WLED-Backlit Display with BrightView Micro-edge Technology, Intel Integrated UHD Graphics 600\r\n\r\n✔1 USB 3.1 Gen 1 Type-C (Data transfer only, data 5 Gb/s signaling rate); 2 USB 3.1 Gen 1 (Data transfer only); 1 HDMI; 1 headphone/microphone combo; Realtek RTL8822CE 802.11a/b/g/n/ac (2x2) Wi-Fi and Bluetooth 5 combo; 1 media card reader; NO Optical Drive\r\n\r\n✔Office 365 for one year: Get full access to Microsoft Excel, Word, PowerPoint, OneNote, Access, and 1 TB of One Drive storage for 1 year\r\n\r\nUp to 14 hours and 15 minutes (video playback), up to 11 hours and 30 minutes (wireless streaming)\r\n\r\n✔Windows 10 S OS, Free upgrade to windows 11\r\n\r\n✔TGCD bundle: ADATA 64GB microSDXC memory card",
                    Price = 19850000,
                    DiscountAmount = 900000,
                    StockQuantity = 2,
                    Currency = "IRT"
                };
                if (digitalCategory != null) hpLaptopProMax.ProductCategoryMappings.Add(new() { CategoryId = digitalCategory.Id, DisplayOrder = 1 });
                if (digitalLaptopCategory != null) hpLaptopProMax.ProductCategoryMappings.Add(new() { CategoryId = digitalLaptopCategory.Id, DisplayOrder = 1 });
                if (snowWhiteColor != null) hpLaptopProMax.ProductSpecificationValueMappings.Add(new() { SpecificationValueId = snowWhiteColor.Id });
                if (casualType != null) hpLaptopProMax.ProductSpecificationValueMappings.Add(new() { SpecificationValueId = casualType.Id });
                if (hpBrand != null) hpLaptopProMax.ProductSpecificationValueMappings.Add(new() { SpecificationValueId = hpBrand.Id });
                if (laptopDimensions != null) hpLaptopProMax.ProductSpecificationValueMappings.Add(new() { SpecificationValueId = laptopDimensions.Id });
                if (laptopWeight != null) hpLaptopProMax.ProductSpecificationValueMappings.Add(new() { SpecificationValueId = laptopWeight.Id });

                _unitOfWork.Set<Product>().AddRange(iPhone13ProMax, hpLaptopProMax);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
