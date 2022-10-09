using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AWSServerlessDynamoDb.Models;
using Type = AWSServerlessDynamoDb.Models.Type;

namespace AWSServerlessDynamoDb.Services
{
    public class EmployeeDb : IEmployeeDb
    {
        private readonly AmazonDynamoDBConfig _clientConfig = new AmazonDynamoDBConfig();
        private readonly AmazonDynamoDBClient _client;
        private readonly DynamoDBContext _context;
        private readonly ILogger<EmployeeDb> _logger;

        public EmployeeDb(ILogger<EmployeeDb> logger, IWebHostEnvironment configuration)
        {
            //Comment out the below four line if you're not using the DynamoDb local instance.
            if (configuration.IsDevelopment())
            {
                _clientConfig.ServiceURL = "http://localhost:8000";
            }
            _client = new AmazonDynamoDBClient(_clientConfig);
            _context = new DynamoDBContext(_client);
            _logger = logger;
        }

        public async Task SaveAsync(EmployeeModel model)
        {
            await SaveInDbAsync(GetUserModelForSave(PrepareEmpModel(model)));
            await SaveInDbAsync(GetReporteeModelForSave(PrepareEmpModel(model)));
        }

        private async Task SaveInDbAsync(EmployeeModel model)
        {
            await _context.SaveAsync(model);
            _logger.LogInformation("Saved {} successfully!", model.EmployeeCode);
        }

        public async Task SaveBatchAsync(List<EmployeeModel> models)
        {
            var empBatch = _context.CreateBatchWrite<EmployeeModel>();
            empBatch.AddPutItems(models);
            await empBatch.ExecuteAsync();
        }

        public async Task<EmployeeModel> GetEmployeeAsync(string empCode)
        {
            var result = await _context.LoadAsync<EmployeeModel>(empCode.ToUpper(), empCode.ToUpper());
            if (result != null)
                result.ReportingManagerCode = ""; //ReportingManagerCode was same as EmployeeCode, so just remove it
            return result;
        }

        public async Task<IEnumerable<EmployeeModel>> GetAllReporteesAsync(string empCode)
        {
            var config = new DynamoDBOperationConfig
            {
                QueryFilter = new List<ScanCondition> {
                    new ScanCondition("Type", ScanOperator.Equal, "Reportee"),
                    new ScanCondition("LastWorkingDate", ScanOperator.IsNull)
                }
            };
            var result = await _context.QueryAsync<EmployeeModel>(empCode.ToUpper(), config).GetRemainingAsync();
            return PrepareReporteeReturnModel(result); //swap the EmployeeCode and ReportingManagerCode and return
        }

        private IEnumerable<EmployeeModel> PrepareReporteeReturnModel(List<EmployeeModel> models)
        {
            models.ForEach(a =>
            {
                var tmp = a.EmployeeCode;
                a.EmployeeCode = a.ReportingManagerCode;
                a.ReportingManagerCode = tmp;
            });
            return models;
        }

        private EmployeeModel PrepareEmpModel(EmployeeModel model)
        {
            model.EmployeeCode = model.EmployeeCode?.ToUpper();
            model.ReportingManagerCode = model.ReportingManagerCode?.ToUpper();
            return model;
        }

        private EmployeeModel GetUserModelForSave(EmployeeModel model)
        {
            var userModel = (EmployeeModel)model.Clone();
            userModel.Type = Type.User.ToString();
            userModel.ReportingManagerCode = model.EmployeeCode;
            return userModel;
        }

        private EmployeeModel GetReporteeModelForSave(EmployeeModel model)
        {
            var userModel = (EmployeeModel)model.Clone();
            userModel.Type = Type.Reportee.ToString();
            userModel.ReportingManagerCode = model.EmployeeCode;
            userModel.EmployeeCode = model.ReportingManagerCode;
            return userModel;
        }
    }
}