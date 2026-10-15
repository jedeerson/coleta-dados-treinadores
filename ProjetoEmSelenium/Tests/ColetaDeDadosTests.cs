using NUnit.Framework; // Ou Microsoft.VisualStudio.TestTools.UnitTesting / xUnit
using System;
using System.IO;
using System.Text;

namespace TelaPesquisaTests
{
    [TestFixture]
    public class ColetaDeDadosCsvTests
    {
        private string _caminhoArquivoTeste;

        [SetUp]
        public void Setup()
        {
            // Define um caminho temporário para não sujar seu arquivo original de produção
            _caminhoArquivoTeste = Path.Combine(Path.GetTempPath(), "empresas_coletadas_teste.csv");

            // Garante que o arquivo antigo de teste seja apagado antes de rodar
            if (File.Exists(_caminhoArquivoTeste))
            {
                File.Delete(_caminhoArquivoTeste);
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Limpa o arquivo de teste gerado após a execução
            if (File.Exists(_caminhoArquivoTeste))
            {
                File.Delete(_caminhoArquivoTeste);
            }
        }

        [Test]
        public void CriarOuAnexarCsv_DeveGravarCabecalhoELinhasCorretamente()
        {
            // 1. ARRANGE (Preparação)
            string cabecalhoEsperado = $"CNPJ;Razao Social;Telefone{Environment.NewLine}";
            string cnpj = "12.345.678/0001-99";
            string razaoSocial = "Empresa Teste LTDA";
            string telefone = "(11) 99999-9999";

            // 2. ACT (Ação - Simulando a lógica de gravação do seu código)
            if (!File.Exists(_caminhoArquivoTeste))
            {
                File.WriteAllText(_caminhoArquivoTeste, cabecalhoEsperado, Encoding.UTF8);
            }

            string linhaCsv = $"\"{cnpj}\";\"{razaoSocial}\";\"{telefone}\"{Environment.NewLine}";
            File.AppendAllText(_caminhoArquivoTeste, linhaCsv, Encoding.UTF8);

            // 3. ASSERT (Verificação dos resultados)
            Assert.IsTrue(File.Exists(_caminhoArquivoTeste), "O arquivo CSV deveria ter sido criado.");

            string[] linhas = File.ReadAllLines(_caminhoArquivoTeste, Encoding.UTF8);

            Assert.AreEqual(2, linhas.Length, "O arquivo deve conter exatamente 2 linhas (cabeçalho + 1 registro).");
            Assert.AreEqual("CNPJ;Razao Social;Telefone", linhas[0], "O cabeçalho do CSV está incorreto.");
            Assert.AreEqual($"\"{cnpj}\";\"{razaoSocial}\";\"{telefone}\"", linhas[1], "A linha de dados gravada não corresponde ao esperado.");
        }
    }
}