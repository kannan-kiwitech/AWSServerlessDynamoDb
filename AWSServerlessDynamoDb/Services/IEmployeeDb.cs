using AWSServerlessDynamoDb.Models;

namespace AWSServerlessDynamoDb.Services
{
    public interface IEmployeeDb
    {
        Task<IEnumerable<EmployeeModel>> GetAllReporteesAsync(string empCode);

        Task<EmployeeModel> GetEmployeeAsync(string empCode);

        Task SaveAsync(EmployeeModel model);

        Task SaveBatchAsync(List<EmployeeModel> models);
    }
}