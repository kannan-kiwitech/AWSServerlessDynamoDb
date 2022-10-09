using Amazon.DynamoDBv2.DataModel;
using Newtonsoft.Json;

namespace AWSServerlessDynamoDb.Models
{
    [DynamoDBTable("employees")]
    public class EmployeeModel : ICloneable
    {
        [DynamoDBHashKey]
        [JsonProperty("EmployeeCode")]
        public string? EmployeeCode { get; set; }

        [DynamoDBProperty]
        [JsonProperty("EmailId")]
        public string? EmailId { get; set; }

        [DynamoDBProperty]
        [JsonProperty("FirstName")]
        public string? FirstName { get; set; }

        [DynamoDBProperty]
        [JsonProperty("LastName")]
        public string? LastName { get; set; }

        [DynamoDBProperty]
        [JsonProperty("Type")]
        public string? Type { get; set; }

        [DynamoDBRangeKey]
        [JsonProperty("ReportingManagerCode")]
        public string? ReportingManagerCode { get; set; }

        [DynamoDBProperty]
        [JsonProperty("LastWorkingDate")]
        public string? LastWorkingDate { get; set; }

        public object Clone()
        {
            var emp = new EmployeeModel
            {
                EmployeeCode = EmployeeCode,
                EmailId = EmailId,
                FirstName = FirstName,
                LastName = LastName,
                Type = Type,
                LastWorkingDate = LastWorkingDate,
                ReportingManagerCode = ReportingManagerCode
            };
            return emp;
        }
    }
}