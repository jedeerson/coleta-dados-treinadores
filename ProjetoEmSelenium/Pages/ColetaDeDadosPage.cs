using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ColetaDeTreinadores.Pages
{
    public class ColetaDeDadosPage
    {
        private IWebDriver driver;

        public int ColetadosTotal { get; private set; } = 0;
        public const int LIMITE_POR_EXECUCAO = 10;

        public ColetaDeDadosPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        private string ExtrairDados(string xpath)
        {
            try
            {
                var elemento = driver.FindElement(By.XPath(xpath));
                return elemento.Text;
            }
            catch
            {
                return "NÃO ENCONTRADO";
            }
        }

        public bool AcessarPerfilDeTreinadores(List<string> urlsDasEmpresas, HashSet<string> cnpjsExistentes, string caminhoArquivo)
        {
            if (!File.Exists(caminhoArquivo))
            {
                string cabecalho = $"CNPJ;Razao Social;Telefone{Environment.NewLine}";
                File.WriteAllText(caminhoArquivo, cabecalho, Encoding.UTF8);
            }

            foreach (var url in urlsDasEmpresas)
            {
                if (ColetadosTotal >= LIMITE_POR_EXECUCAO)
                {
                    Console.WriteLine("\n[SUCESSO] Meta global de 10 novos cadastros atingida!");
                    return true;
                }

                // Abre a URL em uma nova aba
                ((IJavaScriptExecutor)driver).ExecuteScript("window.open(arguments[0], '_blank');", url);
                var abas = driver.WindowHandles;
                driver.SwitchTo().Window(abas[abas.Count - 1]);

                try
                {
                    // Pausa necessária para a verificação de segurança/login estabilizar
                    Thread.Sleep(5000);

                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    IWebElement elementoCnpj = wait.Until(d => d.FindElement(By.XPath("//label[contains(text(), 'CNPJ')]/following-sibling::p")));

                    string cnpj = elementoCnpj.Text;
                    string razaoSocial = ExtrairDados("//label[contains(text(), 'Razão Social')]/following-sibling::p");
                    string telefone = ExtrairDados("//label[contains(text(), 'Telefone')]/following-sibling::p");

                    cnpj = cnpj.Replace("\n", " ").Replace("\r", "").Replace(";", ",").Trim();
                    razaoSocial = razaoSocial.Replace("\n", " ").Replace("\r", "").Replace(";", ",").Trim();
                    telefone = telefone.Replace("\n", " ").Replace("\r", "").Replace(";", ",").Trim();

                    if (cnpjsExistentes.Contains(cnpj) || string.IsNullOrEmpty(cnpj) || cnpj == "NÃO ENCONTRADO")
                    {
                        Console.WriteLine($"[IGNORADO] CNPJ {cnpj} já existe ou falhou.");
                    }
                    else
                    {
                        cnpjsExistentes.Add(cnpj);
                        ColetadosTotal++;

                        Console.WriteLine($"[{ColetadosTotal}/{LIMITE_POR_EXECUCAO}] [COLETADO] CNPJ: {cnpj} | Razão: {razaoSocial}");

                        string linhaCsv = $"\"{cnpj}\";\"{razaoSocial}\";\"{telefone}\"{Environment.NewLine}";
                        File.AppendAllText(caminhoArquivo, linhaCsv, Encoding.UTF8);
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    Console.WriteLine($"[TIMEOUT] O elemento não carregou em 10s na URL: {url}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERRO] Falha ao processar a página: {ex.Message}");
                }
                finally
                {
                    if (driver.WindowHandles.Count > 1)
                    {
                        driver.Close();
                        driver.SwitchTo().Window(driver.WindowHandles[0]);
                    }
                }
            }

            return ColetadosTotal >= LIMITE_POR_EXECUCAO;
        }
    }
}