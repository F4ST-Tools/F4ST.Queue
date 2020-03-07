using System.Threading.Tasks;

namespace Test.Models
{
    public interface ITestClass//:ISingleton
    {
        Task<string> Test(string a);
        Task Test2(int a);
        string Test3();
    }
}