using System;
using System.Threading.Tasks;
using F4ST.Queue.Receivers;
using Test.Models;

namespace Test.Controllers
{
    public class RpcTestReceiver : RPCReceiver, ITestClass
    {

        public RpcTestReceiver()
        {
        }

        public async Task<string> Test(string a)
        {
            return a + new Random().Next(1000).ToString();
        }

        public async Task Test2()
        {
        }

        public string Test3()
        {
            return new Random().Next(1000).ToString();
        }
    }
}