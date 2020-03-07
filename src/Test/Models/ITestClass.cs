using System.Threading.Tasks;

namespace Test.Models
{
    public interface ITestClass//:ISingleton
    {
        Task<string> Test(string a);
        Task Test2();
        string Test3();
    }
}