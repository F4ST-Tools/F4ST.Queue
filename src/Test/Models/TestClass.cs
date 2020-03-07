using System;
using System.Threading.Tasks;

namespace Test.Models
{
    public class TestClass : ITestClass
    {
        public async Task<string> Test(string a)
        {
            return a + new Random().Next(1000).ToString();
        }

        public async Task Test2(int a)
        {
            
        }

        public string Test3()
        {
            return new Random().Next(1000).ToString();
        }

    }
}