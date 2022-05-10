using System.Threading.Tasks;

namespace CA.Stub
{
    public interface IStubManager
    {
        Task<int> UpdateMapping(string condition, object result);

        Task RemoveMapping(int id);
        
        Task RemoveMapping(string condition);

        Task EditMapping(int id, object result);

        Task<object> Resolve(object input);
    }
}
