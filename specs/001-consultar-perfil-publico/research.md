# Pesquisa técnica — 001-consultar-perfil-publico

Data: 2026-11-22. Fontes de requisitos: `spec.md` e `.specify/memory/constitution.md`. O repositório possui somente documentação; não há convenções de código ou testes existentes a reutilizar. SDK local observado: 10.0.400. Não há questões técnicas em aberto que impeçam o design.

## 1. Arquitetura e consulta

- **Decisão:** quatro projetos prescritos pela Constitution. Domain contém `ProfessionalProfile`; Application contém `GetPublicProfile` e uma porta específica de leitura `IProfessionalProfileReader`; Infrastructure implementa essa porta com EF Core; Api expõe Minimal API. Dependências: Application → Domain; Infrastructure → Application e Domain; Api → Application e Infrastructure para composição.
- **Justificativa:** a porta isola a persistência sem expor DbContext/IQueryable à aplicação. O caso de uso retorna o perfil ou ausência; o endpoint apenas mapeia o resultado para HTTP e DTO.
- **Alternativas consideradas:** DbContext no endpoint viola a separação exigida; repositório genérico, MediatR, CQRS com infraestrutura própria, AutoMapper e classes-base não agregam valor a uma leitura.

## 2. Persistência e unicidade

- **Decisão:** SQLite/EF Core 10, tabela `ProfessionalProfiles`, chave inteira técnica fixa em 1, com chave primária e restrição `CHECK (Id = 1)`. Campos textuais, obrigatórios com validação de domínio e NOT NULL; opcionais anuláveis. Leitura assíncrona sem rastreamento e com cancelamento, retornando zero ou um registro.
- **Justificativa:** a restrição e a chave primária garantem no máximo um perfil, inclusive contra inserções externas. Ausência é zero registros, sem estado artificial ou soft delete.
- **Alternativas consideradas:** escolher arbitrariamente o primeiro registro esconderia violações; flag de perfil ativo permitiria múltiplos perfis; banco em memória na aplicação contrariaria a persistência definida.

## 3. Preparação inicial

- **Decisão:** uma migração inicial cria o esquema e inclui o registro fictício com nome `Nome do Dev`, título `Dev. de Software` e quatro opcionais nulos, por operação de dados explícita nessa migração. Não manter esse exemplo como dados gerenciados permanentes no modelo. Não executar migrações, seed ou escrita no startup ou GET.
- **Justificativa:** o histórico de migrações permite preparação única; reaplicar uma migração já aplicada não recria o registro removido. Um banco sem perfil continua produzindo 404 após reinício.
- **Alternativas consideradas:** seed sempre que a tabela estiver vazia mascararia a ausência; resposta fixa não consultaria a persistência; EnsureCreated não oferece o histórico de preparação escolhido. Preferiu-se operação explícita pontual a HasData para não vincular alterações futuras do exemplo ao modelo.
- **Limite operacional:** o planejamento não executa migrações. Aplicação futura exige autorização específica, inclusive em banco local; dados compartilhados não serão manipulados para testes.

## 4. Contrato e foto

- **Decisão:** `GET /api/perfil`, resposta JSON plana com `nome`, `titulo`, `bio`, `foto`, `pais`, `estado`; todos presentes. Foto é uma referência textual a uma imagem existente, sem download, validação remota, upload ou conversão pelo serviço. Ausência usa 404 sem corpo.
- **Justificativa:** seis propriedades evitam objeto de localização artificial e preservam a independência de país/estado. Referência textual dispensa binários e integração externa. A spec não impõe formato de URL, catálogo geográfico ou limites de tamanho; não serão inventadas essas validações. 404 sem corpo evita um contrato de erro sem necessidade.
- **Alternativas consideradas:** base64 aumenta payload e responsabilidades; recurso de mídia, identificadores públicos e negociação de idioma ampliariam o escopo.

## 5. Segurança e dependências

- **Decisão:** endpoint anônimo, sem mecanismo de autenticação, conforme FR-001/FR-008. Sem CORS adicional ou serviços externos. Logs nativos ILogger sem conteúdo do perfil e sem detalhes internos em erros. Falha de banco não deve ser traduzida em 404; resposta inesperada é 500 sem detalhes sensíveis e eventual mensagem em pt-BR.
- **Justificativa:** não existem recursos protegidos nesta funcionalidade. A foto é dado retornado, nunca destino de requisição do backend.
- **Dependências propostas:** Microsoft.EntityFrameworkCore.Sqlite e Microsoft.EntityFrameworkCore.Design/tool dotnet-ef compatíveis com EF Core 10; xUnit, adaptador/SDK de testes e Microsoft.AspNetCore.Mvc.Testing para validação; Microsoft.AspNetCore.OpenApi e Scalar.AspNetCore no projeto Api para documentação em Development. São ferramentas da stack ou de suporte direto, sem bibliotecas auxiliares de mocks, servidor SQL ou contêineres. Versões exatas devem ser fixadas e aprovadas antes de adicionar/restaurar dependências; nada foi instalado nesta fase.

## 6. Testes

- **Decisão:** xUnit para obrigatórios e independência dos opcionais; integração com WebApplicationFactory e SQLite isolado usando o mesmo provedor da aplicação. Um projeto de testes com pastas Unit e Integration é suficiente inicialmente.
- **Justificativa:** cobre HTTP, serialização, persistência e restrição de unicidade com poucas peças. Enumerar as 16 combinações dos quatro opcionais comprova SC-002. Testar migração em arquivo temporário e reaplicação após remoção controlada comprova que o exemplo não reaparece.
- **Alternativas consideradas:** EF InMemory não valida restrições SQLite; mocks do DbContext não comprovam comportamento persistido; testes apenas de endpoint com leitor falso não comprovam a integração.

## Referências primárias consultadas

- [Testes de Minimal APIs — Microsoft](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-10.0): suporte de WebApplicationFactory/TestServer via Microsoft.AspNetCore.Mvc.Testing.
- [Dados iniciais — EF Core](https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding): operações de dados em migrações permitem preparação explícita revisável.
- [Limitações do provedor SQLite](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations): considerar restrições do provedor na evolução das migrações.

As escolhas de rota, campos, arquitetura concreta e seed pontual são decisões deste planejamento, derivadas dos requisitos; não são imposições dessas referências.

## 7. OpenAPI e Scalar — alinhamento à Constitution 1.1.0

- **Decisão:** usar Microsoft.AspNetCore.OpenApi para gerar o documento a partir dos metadados da Minimal API e Scalar.AspNetCore para apresentá-lo. Condicionar registro AddOpenApi e mapeamentos MapOpenApi/MapScalarApiReference ao ambiente Development. Documento em `/openapi/v1.json`, interface em `/scalar`; ambos indisponíveis nos demais ambientes. Sem geração em build ou artefato estático publicado.
- **Justificativa:** o gerador oficial atende à stack .NET 10; Scalar atende à interface exigida pela Constitution. Configuração restrita ao projeto Api mantém domínio, aplicação e persistência independentes. A geração em runtime evita manter manualmente um segundo contrato executável e não depende de dados reais.
- **Alternativas consideradas:** Swashbuckle/NSwag e outra UI duplicariam ferramentas sem necessidade; documentação pública ou geração estática fora de Development contrariam a restrição solicitada. Não adicionar essas alternativas.
- **Validação planejada:** integração com ambientes Development, Staging e Production; conferir schema, 200/404/500, ausência de segurança e nulls; documentação retorna 404 fora de Development, enquanto a consulta continua funcional. Verificação manual do carregamento e execução de GET no Scalar.
- **Referências oficiais:** [geração OpenAPI ASP.NET Core 10](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-10.0) e [integração Scalar ASP.NET Core](https://scalar.com/products/api-references/integrations/aspnetcore/integration). O registro também restrito a Development é decisão deste plano para atender à solicitação de geração exclusiva nesse ambiente.