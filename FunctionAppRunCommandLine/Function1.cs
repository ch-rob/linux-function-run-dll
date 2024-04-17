using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FunctionAppRunCommandLine
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            var dllPath = "/home/site/wwwroot/exe/HelloWorldConsoleApp.dll";
            var dotnetCmd = "/usr/bin/dotnet";
            string returnStr = String.Empty;

            try
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = dotnetCmd,
                        Arguments = dllPath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        WorkingDirectory = "/usr/bin/"
                    }
                };

                string line = String.Empty;
                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                    line = proc.StandardOutput.ReadLine();

                returnStr += $"The output of '{dotnetCmd} {dllPath}' is '{line}'";
            }
            catch (Exception ex)
            {
                returnStr = ex.ToString();
            }

            _logger.LogInformation($"Captured the following output: {returnStr}");

            return new OkObjectResult(returnStr);
        }
    }
}
