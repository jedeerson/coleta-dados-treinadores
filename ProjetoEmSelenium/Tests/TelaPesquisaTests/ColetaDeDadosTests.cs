using Xunit;
using System;
using System.IO;
using System.Text;

namespace ColetaDeTreinadores.Tests
{
    public class ColetaDeDadosTests : IDisposable
    {
        private readonly string _caminhoArquivoTeste;

        public ColetaDeDadosTests()
        {
            // Apaga o arquivo antigo de teste antes de iniciar
            _caminhoArquivoTeste = Path.Combine(Path.GetTempPath(), "coleta_dados_teste.csv");

            if (File.Exists(_caminhoArquivoTeste))
            {
                File.Delete(_caminhoArquivoTeste);
            }
        }

        [Fact]
        public void TestarEscritaECriacaoDoArquivoCsv()
        {
            // 1. ARRANGE
            string cabecalhoEsperado = $"CNPJ;Razao Social;Telefone{Environment.NewLine}";
            string cnpj = "12.345.678/0001-99";
            string razaoSocial = "Empresa Teste LTDA";
            string telefone = "(11) 99999-9999";

            // 2. ACT
            if (!File.Exists(_caminhoArquivoTeste))
            {
                File.WriteAllText(_caminhoArquivoTeste, cabecalhoEsperado, Encoding.UTF8);
            }

            string linhaCsv = $"\"{cnpj}\";\"{razaoSocial}\";\"{telefone}\"{Environment.NewLine}";
            File.AppendAllText(_caminhoArquivoTeste, linhaCsv, Encoding.UTF8);

            // 3. ASSERT
            Assert.True(File.Exists(_caminhoArquivoTeste), "O arquivo CSV deveria ter sido criado.");

            string[] linhas = File.ReadAllLines(_caminhoArquivoTeste, Encoding.UTF8);

            Assert.Equal(2, linhas.Length);
            Assert.Equal("CNPJ;Razao Social;Telefone", linhas[0]);
            Assert.Equal($"\"{cnpj}\";\"{razaoSocial}\";\"{telefone}\"", linhas[1]);
        }

        public void Dispose()
        {
            // Limpa o arquivo de teste gerado após a execução
            if (File.Exists(_caminhoArquivoTeste))
            {
                File.Delete(_caminhoArquivoTeste);
            }
        }
    }
}