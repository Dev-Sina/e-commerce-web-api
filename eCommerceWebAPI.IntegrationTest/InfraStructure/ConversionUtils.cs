namespace eCommerceWebAPI.IntegrationTest.InfraStructure
{
    public static class ConversionUtils
    {
        public static async Task<T> Deserialize<T>(this HttpContent content)
        {
            var resultStringified = await content.ReadAsStringAsync();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(resultStringified);
            return result;
        }
    }
}
