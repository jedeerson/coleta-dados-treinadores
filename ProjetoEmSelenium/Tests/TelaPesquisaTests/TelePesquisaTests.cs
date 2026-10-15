using Xunit;
using OpenQA.Selenium;
using ColetaDeTreinadores.Core;
using ColetaDeTreinadores.Pages;
using System.IO;
using Microsoft.Extensions.Configuration; 

namespace ColetaDeTreinadores.Tests
{
    public class TelaPesquisaTests
    {
        [Fact]
        public void AbrirPagina_DeveNavegarParaUrlCorreta()
        {
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            string caminhoHistorico = configuration["FilePaths:LastUrl"] ?? string.Empty;

            
            if (File.Exists(caminhoHistorico))
            {
                File.Delete(caminhoHistorico);
            }

            WebDriverManager manager = new WebDriverManager();
            IWebDriver driver = manager.driver;

            try
            {
                TelaPesquisaPage pesquisaPage = new TelaPesquisaPage(driver, caminhoHistorico);

                string urlInicial = "https://casadosdados.com.br/solucao/cnpj?q=Personal+trainer";

                pesquisaPage.CarregarOuIniciarPesquisa(urlInicial);

                Assert.Contains("solucao/cnpj?q=Personal+trainer", driver.Url);
            }
            finally
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}