# ColetaDeTreinadores

Sistema de automação e raspagem de dados desenvolvido em **C# (.NET)** com **Selenium WebDriver** e **xUnit**, projetado para auxiliar no estabelecimento de parcerias estratégicas entre nutricionistas e profissionais de educação física.

## Introdução

O **ColetaDeTreinadores** é uma ferramenta de automação web criada com o objetivo de otimizar a prospecção de parceiros de forma automatizada e inteligente. A aplicação navega em plataformas de dados corporativos (como o Casa dos Dados) para localizar treinadores ativos, extraindo informações comerciais essenciais (como nome, CNPJ e telefone de contato) e salvando os registros diretamente em arquivos CSV para posterior contato e envio de propostas de parcerias e descontos para alunos.

Para garantir eficiência e evitar retrabalho, o sistema conta com lógicas avançadas de controle de estado, salvando a última página processada e validando dados já coletados.

## Funcionalidades Principais

- **Automação de Navegação Web:** Utiliza Selenium WebDriver para interagir com páginas de listagem e dados corporativos de forma simulada a um usuário real.
- **Controle de Paginação Inteligente:** O sistema armazena a última página processada. Caso a execução seja interrompida (por exemplo, processando lotes diários), a próxima execução retoma exatamente de onde parou, evitando varreduras desnecessárias nas mesmas páginas.
- **Verificação de Duplicidade:** Realiza a leitura e validação dos dados já existentes no arquivo CSV para ignorar registros duplicados ou números de telefones já contatados.
- **Arquitetura Limpa (POM):** Estruturado utilizando o padrão de design *Page Object Model (POM)*, garantindo alta separação de responsabilidades e facilidade na manutenção dos seletores e páginas.
- **Testes Automatizados:** Suíte de testes estruturada com **xUnit** para garantir a integridade das rotinas de extração.

## Tecnologias Utilizadas

- **C# / .NET** (Linguagem e plataforma)
- **Selenium WebDriver** (Automação de navegador)
- **xUnit** (Framework de testes unitários)
- **Page Object Model - POM** (Padrão de arquitetura de telas)
- **Manipulação de CSV** (Persistência e controle local de dados)

## Estrutura do Projeto

- **Core/**: Gerenciador de instâncias do WebDriver e rotinas de suporte globais.
- **Pages/**: Implementação do padrão Page Object Model para o mapeamento de elementos e ações das telas.
- **Tests/**: Casos de teste e validações integradas com xUnit.
