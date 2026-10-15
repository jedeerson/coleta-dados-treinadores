using ColetaDeTreinadores.Core;
using ColetaDeTreinadores.Pages;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;

namespace ColetaDeTreinadores
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            string caminhoCsv = configuration["FilePaths:CollectedCompanies"] ?? string.Empty;
            string caminhoUrl = configuration["FilePaths:LastUrl"] ?? string.Empty;
            string urlInicial = "https://casadosdados.com.br/solucao/cnpj?q=Personal+trainer";

            WebDriverManager webDriverManager = new WebDriverManager();
            IWebDriver driver = webDriverManager.driver;

            try
            {
                HashSet<string> cnpjsExistentes = CarregarCnpjsExistentes(caminhoCsv);

                TelaPesquisaPage telaPesquisa = new TelaPesquisaPage(driver, caminhoUrl);
                ColetaDeDadosPage coletaDados = new ColetaDeDadosPage(driver);

                telaPesquisa.CarregarOuIniciarPesquisa(urlInicial);

                Console.WriteLine("\n=== INICIANDO COLETA DE DADOS ===\n");

                while (coletaDados.ColetadosTotal < ColetaDeDadosPage.LIMITE_POR_EXECUCAO)
                {
                    telaPesquisa.SalvarUrlAtual();

                    List<string> urlsDasEmpresas = telaPesquisa.ObterUrlsDasEmpresasNaPagina();

                    if (urlsDasEmpresas == null || urlsDasEmpresas.Count == 0)
                    {
                        Console.WriteLine("[AVISO] Nenhum card de empresa foi encontrado nesta página.");
                        break;
                    }

                    Console.WriteLine($"Total de cards encontrados na página: {urlsDasEmpresas.Count}");

                    bool atingiuMeta = coletaDados.AcessarPerfilDeTreinadores(urlsDasEmpresas, cnpjsExistentes, caminhoCsv);

                    if (atingiuMeta)
                    {
                        break;
                    }

                    Console.WriteLine("Avançando para a próxima página...");

                    bool temProximaPagina = telaPesquisa.IrParaProximaPagina();

                    if (!temProximaPagina)
                    {
                        Console.WriteLine("[FIM] Não há mais páginas disponíveis ou botão 'Próxima' não encontrado.");
                        break;
                    }
                }

                Console.WriteLine("\n=== PROCESSO FINALIZADO ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERRO NO PROGRAMA]: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Pressione qualquer tecla para encerrar...");
                Console.ReadKey();
                driver.Quit();
            }
        }

        private static HashSet<string> CarregarCnpjsExistentes(string caminhoArquivo)
        {
            HashSet<string> cnpjs = new HashSet<string>();

            if (File.Exists(caminhoArquivo))
            {
                var linhas = File.ReadAllLines(caminhoArquivo);
                foreach (var linha in linhas)
                {
                    var partes = linha.Split(';');
                    if (partes.Length > 0)
                    {
                        string cnpj = partes[0].Replace("\"", "").Trim();
                        if (!string.IsNullOrEmpty(cnpj) && cnpj != "CNPJ")
                        {
                            cnpjs.Add(cnpj);
                        }
                    }
                }
            }
            return cnpjs;
        }
    }
}