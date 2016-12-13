using Microsoft.AspNetCore.Mvc;

namespace OmniQuant.QuantWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var request = ControllerContext.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}/services/api";
            ViewData["BaseUrl"] = baseUrl;
            return View();
        }
    }

    namespace Api
    {
        [Route("services/api/project")]
        public class ProjectController : Controller
        {
            [Route("getlogs")]
            public string GetLogs()
            {
                return @"[{""$type11"":""<>f__AnonymousType2`4[[System.String, mscorlib],[System.String, mscorlib],[System.String, mscorlib],[System.DateTime, mscorlib]], StrategyService"",""name"":""log"",""solutionName"":"""",""projectName"":"""",""creationDate"":""0001-01-01T00:00:00""},{""$type"":""<>f__AnonymousType2`4[[System.String, mscorlib],[System.String, mscorlib],[System.String, mscorlib],[System.DateTime, mscorlib]], StrategyService"",""name"":""SMACrossover-Backtest-131242475965324452"",""solutionName"":""SMACrossover"",""projectName"":""Backtest"",""creationDate"":""2016-11-22T00:19:56.7824426""}]";
            }

            [Route("status")]
            public string GetStatus()
            {
                return @"[{""$type11"":""<>f__AnonymousType2`4[[System.String, mscorlib],[System.String, mscorlib],[System.String, mscorlib],[System.DateTime, mscorlib]], StrategyService"",""name"":""log"",""solutionName"":"""",""projectName"":"""",""creationDate"":""0001-01-01T00:00:00""},{""$type"":""<>f__AnonymousType2`4[[System.String, mscorlib],[System.String, mscorlib],[System.String, mscorlib],[System.DateTime, mscorlib]], StrategyService"",""name"":""SMACrossover-Backtest-131242475965324452"",""solutionName"":""SMACrossover"",""projectName"":""Backtest"",""creationDate"":""2016-11-22T00:19:56.7824426""}]";
            }

            [Route("stop")]
            public string Stop()
            {
                return @"[{""$type11"":""<>f__AnonymousType2`4[[System.String, mscorlib],[System.String, mscorlib],[System.String, mscorlib],[System.DateTime, mscorlib]], StrategyService"",""name"":""log"",""solutionName"":"""",""projectName"":"""",""creationDate"":""0001-01-01T00:00:00""},{""$type"":""<>f__AnonymousType2`4[[System.String, mscorlib],[System.String, mscorlib],[System.String, mscorlib],[System.DateTime, mscorlib]], StrategyService"",""name"":""SMACrossover-Backtest-131242475965324452"",""solutionName"":""SMACrossover"",""projectName"":""Backtest"",""creationDate"":""2016-11-22T00:19:56.7824426""}]";
            }

            [Route("clearlog")]
            public string clearLog()
            {
                return "LogSeries is cleared";
            }

            [Route("run")]
            public string Run()
            {
                return "Running";
            }
        }

        [Route("services/api/instruments")]
        public class InstrumentsController : Controller
        {
            [Route("")]
            public string GetInstruments()
            {
                return "[{\"$type\":\"SmartQuant.Instrument, SmartQuant\",\"id\":0,\"type\":0,\"symbol\":\"AAPL\",\"description\":\"\",\"exchange\":\"\",\"currencyId\":148,\"ccy1\":0,\"ccy2\":0,\"tickSize\":0.0,\"maturity\":\"0001-01-01T00:00:00\",\"factor\":0.0,\"strike\":0.0,\"putcall\":0,\"margin\":0.0,\"priceFormat\":\"F2\",\"formula\":\"\",\"altId\":[{\"providerId\":4,\"symbol\":\"AAPL|265598\",\"exchange\":\"SMART\",\"currencyId\":148}],\"legs\":[],\"trade\":null,\"ask\":null,\"bid\":null,\"bar\":null,\"parent\":null,\"isDeleted\":false,\"isPersistent\":true},{\"$type\":\"SmartQuant.Instrument, SmartQuant\",\"id\":1,\"type\":0,\"symbol\":\"AMD\",\"description\":\"\",\"exchange\":\"\",\"currencyId\":148,\"ccy1\":0,\"ccy2\":0,\"tickSize\":0.0,\"maturity\":\"0001-01-01T00:00:00\",\"factor\":0.0,\"strike\":0.0,\"putcall\":0,\"margin\":0.0,\"priceFormat\":\"F2\",\"formula\":\"\",\"altId\":[{\"providerId\":4,\"symbol\":\"AMD|4391\",\"exchange\":\"SMART\",\"currencyId\":148}],\"legs\":[],\"trade\":null,\"ask\":null,\"bid\":null,\"bar\":null,\"parent\":null,\"isDeleted\":false,\"isPersistent\":true},{\"$type\":\"SmartQuant.Instrument, SmartQuant\",\"id\":2,\"type\":9,\"symbol\":\"CSCO vs MSFT\",\"description\":\"\",\"exchange\":\"\",\"currencyId\":148,\"ccy1\":0,\"ccy2\":0,\"tickSize\":0.0,\"maturity\":\"0001-01-01T00:00:00\",\"factor\":0.0,\"strike\":0.0,\"putcall\":0,\"margin\":0.0,\"priceFormat\":\"F2\",\"formula\":\"\",\"altId\":[],\"legs\":[],\"trade\":null,\"ask\":null,\"bid\":null,\"bar\":null,\"parent\":null,\"isDeleted\":false,\"isPersistent\":true},{\"$type\":\"SmartQuant.Instrument, SmartQuant\",\"id\":3,\"type\":0,\"symbol\":\"CSCO\",\"description\":\"\",\"exchange\":\"\",\"currencyId\":148,\"ccy1\":0,\"ccy2\":0,\"tickSize\":0.0,\"maturity\":\"0001-01-01T00:00:00\",\"factor\":0.0,\"strike\":0.0,\"putcall\":0,\"margin\":0.0,\"priceFormat\":\"F2\",\"formula\":\"\",\"altId\":[{\"providerId\":4,\"symbol\":\"CSCO|268084\",\"exchange\":\"SMART\",\"currencyId\":148}],\"legs\":[],\"trade\":null,\"ask\":null,\"bid\":null,\"bar\":null,\"parent\":null,\"isDeleted\":false,\"isPersistent\":true},{\"$type\":\"SmartQuant.Instrument, SmartQuant\",\"id\":4,\"type\":0,\"symbol\":\"IBM\",\"description\":\"\",\"exchange\":\"\",\"currencyId\":148,\"ccy1\":0,\"ccy2\":0,\"tickSize\":0.0,\"maturity\":\"0001-01-01T00:00:00\",\"factor\":0.0,\"strike\":0.0,\"putcall\":0,\"margin\":0.0,\"priceFormat\":\"F2\",\"formula\":\"\",\"altId\":[{\"providerId\":4,\"symbol\":\"IBM|8314\",\"exchange\":\"SMART\",\"currencyId\":148}],\"legs\":[],\"trade\":null,\"ask\":null,\"bid\":null,\"bar\":null,\"parent\":null,\"isDeleted\":false,\"isPersistent\":true},{\"$type\":\"SmartQuant.Instrument, SmartQuant\",\"id\":5,\"type\":0,\"symbol\":\"MSFT\",\"description\":\"\",\"exchange\":\"\",\"currencyId\":148,\"ccy1\":0,\"ccy2\":0,\"tickSize\":0.0,\"maturity\":\"0001-01-01T00:00:00\",\"factor\":0.0,\"strike\":0.0,\"putcall\":0,\"margin\":0.0,\"priceFormat\":\"F2\",\"formula\":\"\",\"altId\":[{\"providerId\":4,\"symbol\":\"MSFT|272093\",\"exchange\":\"SMART\",\"currencyId\":148}],\"legs\":[],\"trade\":null,\"ask\":null,\"bid\":null,\"bar\":null,\"parent\":null,\"isDeleted\":false,\"isPersistent\":true},{\"$type\":\"SmartQuant.Instrument, SmartQuant\",\"id\":6,\"type\":9,\"symbol\":\"NQ\",\"description\":\"\",\"exchange\":\"\",\"currencyId\":148,\"ccy1\":0,\"ccy2\":0,\"tickSize\":0.0,\"maturity\":\"0001-01-01T00:00:00\",\"factor\":0.0,\"strike\":0.0,\"putcall\":0,\"margin\":0.0,\"priceFormat\":\"F2\",\"formula\":\"\",\"altId\":[],\"legs\":[],\"trade\":null,\"ask\":null,\"bid\":null,\"bar\":null,\"parent\":null,\"isDeleted\":false,\"isPersistent\":true},{\"$type\":\"SmartQuant.Instrument, SmartQuant\",\"id\":7,\"type\":1,\"symbol\":\"NQH4\",\"description\":\"\",\"exchange\":\"\",\"currencyId\":148,\"ccy1\":0,\"ccy2\":0,\"tickSize\":0.0,\"maturity\":\"0001-01-01T00:00:00\",\"factor\":0.0,\"strike\":0.0,\"putcall\":0,\"margin\":0.0,\"priceFormat\":\"F2\",\"formula\":\"\",\"altId\":[],\"legs\":[],\"trade\":null,\"ask\":null,\"bid\":null,\"bar\":null,\"parent\":null,\"isDeleted\":false,\"isPersistent\":true},{\"$type\":\"SmartQuant.Instrument, SmartQuant\",\"id\":8,\"type\":1,\"symbol\":\"NQZ3\",\"description\":\"\",\"exchange\":\"\",\"currencyId\":148,\"ccy1\":0,\"ccy2\":0,\"tickSize\":0.0,\"maturity\":\"0001-01-01T00:00:00\",\"factor\":0.0,\"strike\":0.0,\"putcall\":0,\"margin\":0.0,\"priceFormat\":\"F2\",\"formula\":\"\",\"altId\":[],\"legs\":[],\"trade\":null,\"ask\":null,\"bid\":null,\"bar\":null,\"parent\":null,\"isDeleted\":false,\"isPersistent\":true},{\"$type\":\"SmartQuant.Instrument, SmartQuant\",\"id\":9,\"type\":0,\"symbol\":\"AAPL vs MSFT\",\"description\":\"\",\"exchange\":\"\",\"currencyId\":148,\"ccy1\":0,\"ccy2\":0,\"tickSize\":0.0,\"maturity\":\"0001-01-01T00:00:00\",\"factor\":0.0,\"strike\":0.0,\"putcall\":0,\"margin\":0.0,\"priceFormat\":\"F2\",\"formula\":\"\",\"altId\":[],\"legs\":[],\"trade\":null,\"ask\":null,\"bid\":null,\"bar\":null,\"parent\":null,\"isDeleted\":false,\"isPersistent\":true}]";
            }
        }

        [Route("services/api/strategies")]
        public class StrategiesController : Controller
        {
            [HttpGet("{id:int}")]
            public string GetStrategy(int id)
            {
                return "{\"Id\":10,\"User\":null,\"WebUser\":\"javagg\",\"Timestamp\":\"2016-12-03T08:25:50.4403842\",\"Duration\":\"00:00:09.2341211\",\"Name\":\"BollingerBands\",\"Mode\":\"Backtest\",\"Status\":1,\"DurationString\":\"00:00:09\",\"Parameters\":[{\"Name\":\"Type\",\"Value\":\"InstrumentStrategy\",\"TypeName\":\"System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\",\"Attributes\":[{\"$type\":\"System.ComponentModel.CategoryAttribute, System\",\"Category\":\"Information\",\"TypeId\":\"System.ComponentModel.CategoryAttribute, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"},{\"$type\":\"System.ComponentModel.ReadOnlyAttribute, System\",\"IsReadOnly\":true,\"TypeId\":\"System.ComponentModel.ReadOnlyAttribute, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"}]},{\"Name\":\"Id\",\"Value\":102,\"TypeName\":\"System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\",\"Attributes\":[{\"$type\":\"System.ComponentModel.CategoryAttribute, System\",\"Category\":\"Information\",\"TypeId\":\"System.ComponentModel.CategoryAttribute, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"},{\"$type\":\"System.ComponentModel.ReadOnlyAttribute, System\",\"IsReadOnly\":true,\"TypeId\":\"System.ComponentModel.ReadOnlyAttribute, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"}]},{\"Name\":\"AllocationPerInstrument\",\"Value\":100000.0,\"TypeName\":\"System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\",\"Attributes\":[]},{\"Name\":\"Qty\",\"Value\":100.0,\"TypeName\":\"System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\",\"Attributes\":[]},{\"Name\":\"Length\",\"Value\":10,\"TypeName\":\"System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\",\"Attributes\":[]},{\"Name\":\"K\",\"Value\":2.0,\"TypeName\":\"System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\",\"Attributes\":[]}]}";
            }
        }

        [Route("services/api/solutions")]
        public class SolutionsController : Controller
        {
            [HttpGet("{solutionName}/logs")]
            public string GetLogs(string solutionName)
            {
                return "{\"ResultCode\":1,\"Errors\":[]}";
            }

            [Route("{solutionName}/build")]
            public string Build(string solutionName)
            {
                return "{\"ResultCode\":1,\"Errors\":[]}";
            }

            [Route("{solutionName}/projects")]
            public string GetProjects(string solutionName)
            {
                return "{}";
            }

            [Route("{solutionName}/projects/{projectName}/files")]
            public string GetProjectFiles(string solutionName, string projectName)
            {
                return "[{\"Name\":\"Program.cs\",\"Length\":1234,\"DateModified\":\"2016-11-22T00:29:23.9696988+00:00\"},{\"Name\":\"Scenario.cs\",\"Length\":1012,\"DateModified\":\"2016-09-06T12:02:37.8032609+00:00\"}]";
            }

            [Route("{solutionName}/projects/{projectName}/files/{fileName}")]
            public string GetProjectFile(string solutionName, string projectName, string fileName)
            {
                return "{\"Content\":\"using System;\\n\\nusing SmartQuant;\\nusing System.Net;\\nusing System.Net.Http;\\nusing System.Threading.Tasks;\\n\\nnamespace OpenQuant\\n{\\n    class Program\\n    {\\n        static void Main(string[] args)\\n        {\\n            var t = new Task(DownloadPageAsync);\\n\\t        t.Start().Wait();\\n\\t        Console.WriteLine(\\\"Downloading page...\\\");\\n            \\n            Scenario scenario = new Backtest(Framework.Current);\\n            scenario.Run();\\n\\n            Framework.Current.Dispose(10);\\n        }\\n        \\n        static async void DownloadPageAsync()\\n        {\\n            // ... Target page.\\n            string page = \\\"http://www.sohu.com/\\\";\\n\\n            // ... Use HttpClient.\\n            using (HttpClient client = new HttpClient())\\n            using (HttpResponseMessage response = await client.GetAsync(page))\\n            using (HttpContent content = response.Content)\\n            {\\n                // ... Read the string.\\n                string result = await content.ReadAsStringAsync();\\n\\n                // ... Display the result.\\n                if (result != null && result.Length >= 50)\\n                {\\n                     Console.WriteLine(result.Substring(0, 50) + \\\"...\\\");\\n                }\\n            }\\n        }\\n    }\\n}\\n\"}";
            }
        }

        [Route("services/api/solutions/{solutionName}/projects/{projectName}/files")]
        public class FilesController : Controller
        {
            [HttpGet("")]
            public string GetFiles(string solutionName, string projectName)
            {
                return "[{\"Name\":\"Program.cs\",\"Length\":1234,\"DateModified\":\"2016-11-22T00:29:23.9696988+00:00\"},{\"Name\":\"Scenario.cs\",\"Length\":1012,\"DateModified\":\"2016-09-06T12:02:37.8032609+00:00\"}]";
            }

            [HttpGet("{fileName}")]
            public string GetFile(string solutionName, string projectName, string fileName)
            {
                return "{\"Content\":\"using System;\\n\\nusing SmartQuant;\\nusing System.Net;\\nusing System.Net.Http;\\nusing System.Threading.Tasks;\\n\\nnamespace OpenQuant\\n{\\n    class Program\\n    {\\n        static void Main(string[] args)\\n        {\\n            var t = new Task(DownloadPageAsync);\\n\\t        t.Start().Wait();\\n\\t        Console.WriteLine(\\\"Downloading page...\\\");\\n            \\n            Scenario scenario = new Backtest(Framework.Current);\\n            scenario.Run();\\n\\n            Framework.Current.Dispose(10);\\n        }\\n        \\n        static async void DownloadPageAsync()\\n        {\\n            // ... Target page.\\n            string page = \\\"http://www.sohu.com/\\\";\\n\\n            // ... Use HttpClient.\\n            using (HttpClient client = new HttpClient())\\n            using (HttpResponseMessage response = await client.GetAsync(page))\\n            using (HttpContent content = response.Content)\\n            {\\n                // ... Read the string.\\n                string result = await content.ReadAsStringAsync();\\n\\n                // ... Display the result.\\n                if (result != null && result.Length >= 50)\\n                {\\n                     Console.WriteLine(result.Substring(0, 50) + \\\"...\\\");\\n                }\\n            }\\n        }\\n    }\\n}\\n\"}";
            }
            [HttpPut("{fileName}")]
            public IActionResult PutFile(string solutionName, string projectName, string fileName)
            {
                return Ok();
            }
        }
    }
}