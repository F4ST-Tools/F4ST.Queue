using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using F4ST.Common.Containers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Test.Models;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITestClass _testClass;

        public HomeController(ILogger<HomeController> logger, ITestClass testClass)
        {
            _logger = logger;
            //_testClass = IoC.Resolve<ITestClass>();
            _testClass = testClass;
        }

        public async Task<IActionResult> Index()
        {
            var a = await _testClass.Test("test: ");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
