using System.Threading.Tasks;

namespace CrytonCore.Interfaces
{
    public interface IService
    {
        bool GetStatus();

        Task InitializeService(object obj);
    }
}
