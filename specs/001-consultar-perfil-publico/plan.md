# Plano de implementação: Consulta pública do perfil profissional

**Branch**: `001-consultar-perfil-publico` | **Data**: 2026-11-22 | **Spec**: [spec.md](spec.md)

**Estado**: planejamento elaborado para revisão do usuário; implementação não iniciada nem autorizada por este documento.

## Summary

Disponibilizar como único endpoint funcional `GET /api/perfil`, anônimo, retornando o único perfil persistido com HTTP 200 e seis campos explícitos, ou HTTP 404 sem corpo quando ausente. Utilizar as quatro camadas da Constitution, Minimal APIs e EF Core/SQLite. Preparar um perfil fictício somente na migração inicial, sem escrita durante consultas ou startup. Decisões e alternativas estão em [research.md](research.md).

## Technical Context

- **Language/Version**: .NET 10, C# padrão do SDK, nullable habilitado. SDK 10.0.400 disponível localmente; não há global.json.
- **Primary Dependencies**: ASP.NET Core Minimal APIs, System.Text.Json, DI e ILogger nativos; EF Core 10 SQLite; `Microsoft.AspNetCore.OpenApi` para geração OpenAPI e `Scalar.AspNetCore` para interface interativa, ambos restritos a Development; xUnit e suporte oficial de integração HTTP. Pacotes/ferramentas e versões exatas exigem aprovação antes de introdução; nenhum instalado neste planejamento.
- **Storage**: arquivo SQLite configurado externamente, sem banco versionado. Uma tabela, zero ou um perfil, chave técnica fixa e restrição de unicidade. Migração inicial com exemplo pontual; aplicação explícita e autorizada fora do processo HTTP.
- **Testing**: um projeto xUnit com unitários e integração WebApplicationFactory/SQLite isolado. Sem EF InMemory ou dependência de serviços externos.
- **Target Platform**: serviço ASP.NET Core em runtime .NET 10; desenvolvimento verificado em Windows. Hospedagem e deploy não definidos nem necessários a esta entrega.
- **Project Type**: serviço HTTP sem interface visual de produto; Scalar é a interface técnica de documentação somente em Development.
- **Performance Goals**: nenhuma meta quantitativa exigida; uma leitura assíncrona sem tracking por consulta, sem cache ou otimização antecipada.
- **Constraints**: pt-BR; leitura pública; opcionais explicitamente nulos; zero autenticação, edição, upload, links sociais ou dependência funcional externa.
- **Scale/Scope**: um recurso, uma entidade e um endpoint funcional; documentação técnica somente em Development; nenhum suporte a múltiplos profissionais.

## Constitution Check

Gate inicial avaliado antes do design e reavaliado após Fase 1:

| Princípio | Antes | Após design / evidência |
| --- | --- | --- |
| I — Stack e dependências | Conforme | .NET 10, Minimal API, SQLite/EF Core, xUnit, OpenAPI e Scalar conforme Constitution 1.1.0; pacotes propostos justificados, sem instalação; autenticação dispensada pelos FR-001/008 |
| II — Arquitetura | Conforme | Quatro projetos prescritos; porta específica de leitura evita persistência em Application/Api; endpoint somente traduz HTTP |
| III — Segurança | Conforme | Anônimo por requisito; dados sem segredos; foto não causa requisição externa; erros sem detalhes internos; configuração fora do código quando sensível |
| IV — Qualidade | Conforme | .editorconfig planejado, DI/ILogger nativos e testes de domínio, HTTP, SQLite e disponibilidade da documentação por ambiente |
| V — Desempenho | Conforme | Uma leitura sem rastreamento; sem metas inventadas, cache, infraestrutura ou escalabilidade antecipada |
| Governança e escopo | Conforme | Sem mudanças na Constitution/spec; planejamento não autoriza implementação, dependências, migrações ou deploy |

Nenhuma violação ou esclarecimento técnico bloqueante. Aprovação do planejamento e autorizações operacionais futuras continuam pendentes. `design-guidelines.md` trata da Home visual e não amplia esta spec de serviço; nenhum projeto Blazor será criado nesta fase.

## Project Structure

### Documentation (this feature)

```text
specs/001-consultar-perfil-publico/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/public-profile.md
└── checklists/requirements.md
```

`tasks.md` pertence à próxima etapa speckit-tasks e não é gerado aqui. O relatório fica em `output/001-consultar-perfil-publico.md`.

### Source Code (repository root — estrutura proposta, ainda inexistente)

```text
PortalDerick.slnx
.editorconfig
src/
├── PortalDerick.Domain/
│   └── ProfessionalProfile.cs
├── PortalDerick.Application/
│   └── Profiles/
│       ├── IProfessionalProfileReader.cs
│       └── GetPublicProfile.cs
├── PortalDerick.Infrastructure/
│   ├── Persistence/
│   │   ├── PortalDerickDbContext.cs
│   │   └── Migrations/
│   └── Profiles/ProfessionalProfileReader.cs
└── PortalDerick.Api/
    ├── Program.cs
    └── Profiles/
        ├── PublicProfileEndpoint.cs
        └── PublicProfileResponse.cs
tests/
└── PortalDerick.Tests/
    ├── Unit/
    └── Integration/
```

**Structure Decision**: separar produção nos quatro projetos obrigatórios; agrupar testes inicialmente em um projeto para evitar fragmentação. Domain não referencia outras camadas; Application referencia Domain; Infrastructure referencia Application/Domain; Api referencia Application/Infrastructure para composição. A interface de leitura retorna entidade ou ausência com CancellationToken, sem IQueryable/DbContext. DTO HTTP fica em Api, mapeado diretamente sem biblioteca. Não criar repositório genérico, framework de casos de uso, projeto Web ou camadas adicionais.

## Fase 0 — Pesquisa

Consolidada em [research.md](research.md), incluindo revisão independente por agente de pesquisa. Resolvidas: fronteiras das camadas, cardinalidade no banco, seed pontual, formato da foto, contrato, autenticação e estratégia de testes. Fontes oficiais consultadas sustentam uso de testes HTTP e migrações; não substituem requisitos.

## Fase 1 — Design e validação

- [data-model.md](data-model.md): campos, regras e ciclo de vida sem transições por GET.
- [contracts/public-profile.md](contracts/public-profile.md): rota, JSON, nulls, 404, métodos e rastreabilidade FR/SC.
- [quickstart.md](quickstart.md): preparação futura e cenários reproduzíveis de validação.

Ordem técnica para futura decomposição em tarefas: solução/convenções e dependências aprovadas; domínio; caso de uso/porta; persistência/migração; composição/endpoint e documentação OpenAPI/Scalar em Development; testes e execução autorizada da preparação inicial. Essa sequência não constitui tarefas implementadas.

## Complexity Tracking

Sem exceções à Constitution. A única abstração de persistência é a porta de leitura necessária à inversão de dependência. A restrição SQLite justifica-se pelo perfil único; OpenAPI e Scalar atendem à exigência da Constitution 1.1.0 sem acrescentar camadas ou geradores alternativos.

## Limitações e pontos de revisão

- Não há solução, código ou suíte executável hoje: build/test não são aplicáveis ao planejamento.
- A spec registra `main` como metadado histórico; o setup e Git identificam `001-consultar-perfil-publico`. Nenhuma branch foi criada ou trocada nesta execução.
- Textos fictícios, rota e nomes JSON são decisões técnicas propostas neste plano e dependem da revisão do usuário.
- Não se executaram instalações, migrações, alterações em dados, commits ou pushes.

## Documentação HTTP em desenvolvimento

Prever `Microsoft.AspNetCore.OpenApi` (linha compatível com .NET 10) e `Scalar.AspNetCore` (versão estável compatível) exclusivamente no projeto PortalDerick.Api. A escolha desses pacotes está solicitada para o planejamento; nenhuma dependência foi instalada. Fixar versões na implementação conforme as regras operacionais.

Em Program.cs, registrar AddOpenApi somente quando builder.Environment.IsDevelopment(); após construir a aplicação, mapear MapOpenApi e MapScalarApiReference somente quando app.Environment.IsDevelopment(). Gerar o documento em runtime por requisição, sem geração no build ou publicação estática. Em Development, expor `/openapi/v1.json` e a interface `/scalar`, consumindo esse documento. Em todos os demais ambientes, não registrar a geração nem mapear as rotas de documentação (incluindo rotas subordinadas da interface); responder 404 nesses caminhos. GET /api/perfil permanece disponível independentemente do ambiente.

Derivar a documentação dos metadados do endpoint/DTO: operação pública sem esquema de autenticação, resposta 200 com as seis propriedades obrigatórias e quatro anuláveis, 404 sem corpo e 500 sem detalhes internos. Usar título, descrições e exemplos próprios em pt-BR, mantendo Nome do Dev e Dev. de Software. A UI técnica Scalar não altera o contrato de conteúdo pt-BR do perfil. Não criar endpoint de negócio adicional ou consultar o banco para gerar a documentação.

Validar por integração em Development, Staging e Production: documento e interface disponíveis apenas em Development, esquema alinhado ao contrato e GET funcional preservado. Conferir manualmente o carregamento da interface e a execução da consulta por ela.