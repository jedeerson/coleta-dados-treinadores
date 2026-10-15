using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ColetaDeTreinadores.Pages
{
    public class TelaPesquisaPage
    {
        private IWebDriver driver;
        private readonly string caminhoArquivoUrl;

        public TelaPesquisaPage(IWebDriver driver, string caminhoArquivoUrl)
        {
            this.driver = driver;
            this.caminhoArquivoUrl = caminhoArquivoUrl;
        }

        public void CarregarOuIniciarPesquisa(string urlInicial)
        {
            // Verifica se tem URL salva para continuar de onde parou
            if (File.Exists(caminhoArquivoUrl))
            {
                string urlSalva = File.ReadAllText(caminhoArquivoUrl).Trim();
                if (!string.IsNullOrEmpty(urlSalva))
                {
                    Console.WriteLine($"[RETOMANDO] Carregando última URL salva:\n{urlSalva}");
                    driver.Navigate().GoToUrl(urlSalva);
                    Thread.Sleep(4000);
                    return;
                }
            }

            // Se for do zero, abre a URL passada no Program.cs
            Console.WriteLine("[INÍCIO] Acessando página de pesquisa inicial...");
            driver.Navigate().GoToUrl(urlInicial);
            Thread.Sleep(4000);
        }

        public void SalvarUrlAtual()
        {
            string urlAtual = driver.Url;
            File.WriteAllText(caminhoArquivoUrl, urlAtual);
            Console.WriteLine($"[PROGRESSO SALVO] Ponto de parada gravado: {urlAtual}");
        }

        public List<string> ObterUrlsDasEmpresasNaPagina()
        {
            var urls = new List<string>();

            var treinadoresAtivos = driver.FindElements(By.XPath("//article[contains(@class, 'media')]"));

            foreach (var el in treinadoresAtivos)
            {
                try
                {
                    var statusElemento = el.FindElement(By.XPath(".//small"));
                    string status = statusElemento.Text;

                    // Filtra apenas empresas ativas
                    if (status.Contains("Ativa"))
                    {
                        var linkElemento = el.FindElement(By.XPath(".//a[contains(@href, '/solucao/cnpj/')]"));
                        string urlEmpresa = linkElemento.GetAttribute("href") ?? "";

                        string urlCompleta = urlEmpresa.StartsWith("http")
                            ? urlEmpresa
                            : "https://casadosdados.com.br" + urlEmpresa;

                        if (!string.IsNullOrEmpty(urlEmpresa) && !urls.Contains(urlCompleta))
                        {
                            urls.Add(urlCompleta);
                        }
                    }
                }
                catch
                {
                    // Ignora card com layout fora do padrão
                }
            }

            return urls;
        }

        // Método atualizado usando seu seletor (Antigo AvancarProximaPagina)
        public bool IrParaProximaPagina()
        {
            try
            {
                var botaoProximo = driver.FindElement(By.XPath("//a[contains(@class, 'pagination-next')]"));

                if (botaoProximo != null && botaoProximo.Displayed && botaoProximo.Enabled)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", botaoProximo);

                    // Sua espera original até 10s para os novos cards de empresa estarem visíveis
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(d => d.FindElement(By.XPath("//article")).Displayed);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AVISO] Fim das páginas: {ex.Message}");
            }

            return false;
        }
    }
}