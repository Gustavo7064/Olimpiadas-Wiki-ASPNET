# Olimpíadas Wiki

> Sistema web para consulta e gerenciamento de informações históricas dos Jogos Olímpicos.

## Sobre o projeto

O **Olimpíadas Wiki** é uma aplicação web desenvolvida para organizar e consultar dados relacionados às edições dos Jogos Olímpicos. A plataforma reúne informações sobre modalidades, provas, atletas, cidades, estados, sedes olímpicas e resultados esportivos.

Além da experiência de consulta, o sistema possui uma área administrativa para cadastrar, editar, excluir e relacionar os dados da competição. Dessa forma, a aplicação funciona como uma base histórica estruturada, permitindo navegar desde uma modalidade esportiva até os resultados obtidos por cada atleta em determinada edição.

## Funcionalidades

| Área | Recursos disponíveis |
| --- | --- |
| **Consulta de atletas** | Listagem, cadastro, edição, exclusão e visualização detalhada dos atletas. |
| **Edições olímpicas** | Cadastro de ano e cidade-sede, consulta de detalhes e visualização dos atletas associados a cada edição. |
| **Modalidades** | Gerenciamento das modalidades esportivas cadastradas no sistema. |
| **Provas** | Cadastro e manutenção das provas relacionadas às modalidades. |
| **Resultados** | Registro de resultado e medalha por atleta, prova e edição olímpica. |
| **Localidades** | Cadastro de estados e cidades utilizados na organização dos dados dos atletas e das sedes. |
| **Administração** | Login, registro de usuários, edição, exclusão e controle de usuários administrativos. |
| **Segurança** | Sessão autenticada, autorização por perfil, redirecionamento para login e página de acesso negado. |

## Tecnologias utilizadas

| Tecnologia | Aplicação no projeto |
| --- | --- |
| **C#** | Linguagem principal da aplicação. |
| **ASP.NET Core MVC** | Framework web e organização baseada no padrão Model-View-Controller. |
| **.NET 8** | Plataforma de execução do projeto. |
| **Razor** | Construção das páginas dinâmicas da aplicação. |
| **HTML e CSS** | Estrutura e estilização das interfaces. |
| **JavaScript** | Comportamentos complementares da interface. |
| **MySQL** | Armazenamento dos dados olímpicos e dos usuários. |
| **MySql.Data** | Integração entre a aplicação e o banco de dados. |
| **BCrypt.Net-Next** | Proteção das senhas por meio de hash seguro. |

## Arquitetura

O projeto utiliza o padrão **MVC**, distribuindo as responsabilidades da aplicação de forma organizada:

- **Models:** representam as entidades do domínio, como atletas, edições, modalidades, provas, resultados, cidades, estados e usuários.

- **Views:** páginas Razor responsáveis pela apresentação das informações e pelos formulários de interação.

- **Controllers:** coordenam os fluxos da aplicação e executam as operações de consulta, cadastro, atualização e exclusão.

- **Data:** concentra a classe responsável pela abertura das conexões com o MySQL.

- **Filters:** implementa o controle de acesso baseado em sessão e perfil.

A base de dados também possui procedures para consultas específicas, como a busca de atletas por edição e a recuperação dos resultados de um atleta.

## Modelo de dados

O sistema organiza as principais relações olímpicas da seguinte forma:

```
Modalidade
    └── Provas

Edição Olímpica ─── Cidade-sede

Atleta ─── Cidade ─── Estado

Atleta + Prova + Edição
    └── Resultado e medalha
```

Esse modelo permite representar tanto o cadastro geral dos Jogos quanto o desempenho individual de cada atleta em uma prova e edição específica.

## Fluxo principal de uso

1. O usuário acessa a aplicação e realiza autenticação na área administrativa.

1. Após o login, pode consultar as entidades cadastradas no sistema.

1. O administrador cadastra modalidades e provas, relacionando cada prova à sua modalidade.

1. As edições olímpicas são registradas com ano e cidade-sede.

1. Os atletas são cadastrados com informações pessoais e localização de origem.

1. Os resultados são vinculados ao atleta, à prova e à edição correspondente.

1. A consulta detalhada permite visualizar a trajetória esportiva e as medalhas registradas para cada atleta.

## Pré-requisitos

Para executar o projeto localmente, instale e configure:

- .NET SDK 8.0 ou superior;

- MySQL Server;

- uma IDE compatível com .NET, como Visual Studio ou Visual Studio Code;

- Git, caso deseje clonar o repositório.

## Como executar

### 1. Clonar o repositório

```bash
git clone <URL-DO-REPOSITORIO>
cd Olimpiadas-Wiki-ASPNET-main
```

### 2. Criar o banco de dados

O projeto disponibiliza scripts SQL para criação e carga inicial do banco. Recomenda-se utilizar a versão corrigida:

```
banco_olimpicos_completo_CORRIGIDO.sql
```

No MySQL, execute:

```sql
SOURCE /caminho/para/banco_olimpicos_completo_CORRIGIDO.sql;
```

O script cria o banco `bdolimpicoJueGu`, suas tabelas, dados iniciais, usuários e procedures utilizadas pela aplicação.

### 3. Configurar a conexão

A conexão com o MySQL está centralizada em [`ProjetoOlimpicos/Data/Database.cs`](ProjetoOlimpicos/Data/Database.cs). Ajuste os valores conforme o seu ambiente local:

```csharp
private readonly string connectionString =
    "server=localhost;port=3306;database=bdolimpicoJueGu;user=SEU_USUARIO;password=SUA_SENHA;";
```

> **Importante:** nunca publique senhas reais no repositório. Para ambientes de produção, utilize variáveis de ambiente, User Secrets ou um gerenciador de segredos.

### 4. Restaurar dependências e iniciar a aplicação

```bash
dotnet restore
dotnet run --project ProjetoOlimpicos/ProjetoOlimpicos.csproj
```

Depois, acesse no navegador a URL informada pelo terminal. As portas podem variar conforme a configuração presente em `Properties/launchSettings.json`.

## Controle de acesso

A autenticação é baseada em sessão e utiliza um filtro personalizado para verificar se o usuário está autenticado e se possui o perfil necessário para acessar determinadas funcionalidades.

| Situação | Comportamento |
| --- | --- |
| Usuário não autenticado | É redirecionado para a tela de login. |
| Usuário sem permissão | É direcionado para a página de acesso negado. |
| Usuário autorizado | Pode acessar as operações liberadas para o seu perfil. |
| Requisição AJAX sem sessão | Recebe resposta HTTP de não autorizado ou proibido, conforme o caso. |

O cadastro de novos usuários deve ser utilizado conforme as regras definidas no projeto e no script SQL inicial.

## Estrutura resumida

```
Olimpiadas-Wiki-ASPNET-main/
├── banco_olimpicos_completo.sql
├── banco_olimpicos_completo_CORRIGIDO.sql
├── ProjetoOlimpicos.sln
├── ProjetoOlimpicos/
│   ├── Controllers/
│   ├── Data/
│   ├── Filters/
│   ├── Models/
│   ├── Views/
│   ├── wwwroot/
│   ├── Program.cs
│   └── ProjetoOlimpicos.csproj
└── README.md
```

## Aprendizados e objetivos técnicos

Este projeto foi desenvolvido para praticar a construção de uma aplicação web completa com **C# e ASP.NET Core MVC**, modelagem de dados relacionais, integração com MySQL, autenticação baseada em sessão e autorização por perfil.

O desenvolvimento também envolveu a criação de relacionamentos entre entidades, utilização de procedures, implementação de operações CRUD, organização de dados históricos e construção de telas administrativas para manutenção de uma base de informações esportivas.

## Melhorias futuras

Como possibilidades de evolução, o projeto pode receber uma API REST, filtros avançados por edição, país e medalha, gráficos de desempenho por modalidade, paginação nas listagens, upload de imagens dos atletas, testes automatizados, separação das regras de negócio em Services e Repository Pattern, além de publicação em ambiente cloud com pipeline de CI/CD.

## Referências do projeto

- [Arquivo de projeto e dependências](ProjetoOlimpicos/ProjetoOlimpicos.csproj)

- [Script SQL principal](banco_olimpicos_completo_CORRIGIDO.sql)

- [Classe de conexão com o banco](ProjetoOlimpicos/Data/Database.cs)

- [Filtro de autenticação e autorização](ProjetoOlimpicos/Filters/SessionAuthorizeAttribute.cs)

## Licença

Este projeto pode ser utilizado e adaptado.

---

Desenvolvido como projeto de estudo e prática em desenvolvimento web com **C#, ASP.NET Core MVC e MySQL**.
