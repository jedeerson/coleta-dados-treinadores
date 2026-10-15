using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace ColetaDeTreinadores.Core
{
    public class WebDriverManager
    {
        public IWebDriver driver;

        public WebDriverManager()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            string userProfileDir = configuration["Selenium:UserProfileDir"] ?? string.Empty;

            ChromeOptions options = new ChromeOptions();
            options.AddArgument($"--user-data-dir={userProfileDir}");
            options.AddArgument("--remote-debugging-port=9222");
            options.AddArgument("--disable-blink-features=AutomationControlled");

            driver = new ChromeDriver(options);
        }
    }
}