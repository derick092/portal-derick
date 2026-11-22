# Tarefas: Consulta pública do perfil profissional

**Entrada:** `specs/001-consultar-perfil-publico/` — spec.md, plan.md, research.md, data-model.md, contracts/public-profile.md e quickstart.md; Constitution 1.1.0.
**Estado:** decomposição para revisão; nenhuma tarefa abaixo foi executada ou aprovada. A geração deste arquivo não autoriza implementação.
**Organização:** histórias em ordem de prioridade; caminhos relativos à raiz. `[P]` identifica trabalho em arquivos distintos que pode ocorrer em paralelo após seus pré-requisitos. `[USn]` identifica a história correspondente.
**Testes:** incluídos conforme cenários da especificação e validação exigida pela Constitution, modelo e plano; usar xUnit, leitor simulado nos testes funcionais HTTP e SQLite real isolado nos testes de persistência, sem EF InMemory ou biblioteca de mocks. As 16 combinações de opcionais são verificadas no contrato HTTP, sem repetir essa matriz no SQLite.

## Fase 1: Preparação

**Objetivo:** preparar solução e convenções sem ampliar a arquitetura.

- [ ] T001 Levantar versões exatas compatíveis dos pacotes e ferramentas previstos em `specs/001-consultar-perfil-publico/plan.md` (EF Core SQLite/Design 10, dotnet-ef, OpenAPI, Scalar, xUnit, adaptador/SDK e Mvc.Testing), registrar justificativas nesse plano e obter autorização antes de incluir dependências ou alterar versões; permitir a restauração de dependências e ferramentas já aprovadas nas versões autorizadas, sem nova autorização.
- [ ] T002 Após T001 e autorização de implementação, criar `PortalDerick.slnx`, os quatro projetos em `src/PortalDerick.Domain/PortalDerick.Domain.csproj`, `src/PortalDerick.Application/PortalDerick.Application.csproj`, `src/PortalDerick.Infrastructure/PortalDerick.Infrastructure.csproj`, `src/PortalDerick.Api/PortalDerick.Api.csproj` e o projeto `tests/PortalDerick.Tests/PortalDerick.Tests.csproj`; usar net10.0, nullable e referências na direção do plano, adicionando somente pacotes com versões autorizadas e manifesto local `.config/dotnet-tools.json` para dotnet-ef.
- [ ] T003 [P] Concretizar convenções C#/.NET em `.editorconfig`, sem novas ferramentas, e ignorar bin/obj, arquivos SQLite e auxiliares em `.gitignore`, preservando regras existentes.
- [ ] T004 Restaurar ferramentas/pacotes, compilar e executar a suíte inicial de `PortalDerick.slnx` conforme quickstart; registrar comandos, resultados e eventuais falhas preexistentes em `output/001-consultar-perfil-publico.md` antes de avançar.

## Fase 2: Fundação compartilhada

**Objetivo:** domínio, esquema e infraestrutura de teste necessários às três histórias. Bloqueia as fases seguintes.

- [ ] T005 Criar `src/PortalDerick.Domain/ProfessionalProfile.cs` com as regras literais de `data-model.md`: Id — "Identidade técnica fixa; zero ou um registro"; Name — "String não nula, não vazia e não composta somente por espaços"; Title — "Mesma regra de Name"; Bio — "Opcional independente"; Photo — "Referência textual de imagem; nenhum processamento remoto"; Country — "Opcional independente de State"; State — "Opcional independente de Country". Proteger construção/materialização por caminho validado sem setters públicos invalidantes; preservar valores, sem inferências, limites de tamanho, enumerações ou normalização adicional.
- [ ] T006 Criar `src/PortalDerick.Infrastructure/Persistence/PortalDerickDbContext.cs` mapeando ProfessionalProfiles: Id "INTEGER PK, CHECK Id = 1", Name/Title "TEXT NOT NULL", Bio/Photo/Country/State "TEXT NULL", sem relações ou HasData; preservar invariantes de materialização de T005 e não aplicar migração ou seed.
- [ ] T007 Criar infraestrutura compartilhada em `tests/PortalDerick.Tests/Integration/ProfileApiFactory.cs` com WebApplicationFactory e ambiente parametrizável: usar leitor simulado para perfis completos/parciais/ausentes e falhas nos testes funcionais HTTP, sem acessar SQLite; criar suporte separado em `tests/PortalDerick.Tests/Integration/SqliteProfileFixture.cs` para testes de persistência e migração, com banco exclusivo por teste, preparação direta de dados e reinício do host preservando o banco. Permitir composição com o leitor real somente nos cenários de integração com persistência, sem duplicar a matriz funcional de opcionais. Não usar banco de demonstração nem endpoints auxiliares; aplicação de migrações nos testes depende de autorização específica.

**Checkpoint:** esquema mapeado e testes isoláveis; ainda sem preparação de banco de demonstração.

## Fase 3: US1 — Consultar o perfil profissional (P1) — MVP

**Objetivo:** consultar anonimamente o perfil persistido, preservando os seis campos e nulos explícitos.
**Critério independente:** com fixture conhecida, GET sem credenciais retorna 200 e os valores persistidos em todas as 16 combinações de opcionais; consultas repetidas não escrevem.

### Testes

Escrever os testes antes dos comportamentos correspondentes e confirmar falha significativa após os componentes compilarem.

- [ ] T008 [P] [US1] Criar `tests/PortalDerick.Tests/Unit/ProfessionalProfileTests.cs` cobrindo Name e Title nulos/vazios/somente espaços, preservação de valores e independência dos quatro opcionais, incluindo materialização validada conforme T005.
- [ ] T009 [P] [US1] Criar `tests/PortalDerick.Tests/Integration/PublicProfileContractTests.cs` usando leitor simulado, sem SQLite, para GET anônimo completo e 16 combinações de opcionais: 200 JSON com exatamente nome/titulo/bio/foto/pais/estado, quatro nulls explícitos quando ausentes, sem id, dados preservados e cabeçalhos de idioma sem tradução; incluir o caso básico de leitor retornando ausência com 404 de corpo vazio, sem repetir os cenários aprofundados da US2.
- [ ] T010 [P] [US1] Criar `tests/PortalDerick.Tests/Integration/ProfilePersistenceTests.cs` verificando rejeição de segundo registro e Id diferente de 1 no SQLite, materialização pelo leitor real, consulta repetida sem alteração/duplicação e leitura sem tracking, sem repetir as 16 combinações do contrato HTTP; fixtures não alteram arquivos compartilhados.

### Implementação

- [ ] T011 [US1] Criar `src/PortalDerick.Application/Profiles/IProfessionalProfileReader.cs` e `src/PortalDerick.Application/Profiles/GetPublicProfile.cs` com leitura assíncrona, CancellationToken e retorno de entidade ou ausência, sem DbContext/IQueryable, DTO HTTP ou repositório genérico.
- [ ] T012 [P] [US1] Implementar `src/PortalDerick.Infrastructure/Profiles/ProfessionalProfileReader.cs` após T011, consultando zero ou um registro de modo assíncrono sem tracking e propagando cancelamento e falhas técnicas, sem escrita ou escolha arbitrária do primeiro perfil.
- [ ] T013 [P] [US1] Criar `src/PortalDerick.Api/Profiles/PublicProfileResponse.cs` após T011, mapeando nome/titulo obrigatórios e bio/foto/pais/estado anuláveis, com as seis propriedades sempre serializadas e sem validação remota da foto.
- [ ] T014 [US1] Criar `src/PortalDerick.Api/Profiles/PublicProfileEndpoint.cs` e compor serviços em `src/PortalDerick.Api/Program.cs` após T012/T013; mapear somente GET /api/perfil anônimo com 200 e DTO quando existir perfil e 404 de corpo vazio quando o leitor retornar ausência, sem exemplo substituto, ler ConnectionStrings:PortalDerick externamente e não executar migração/seed no startup ou GET. Preparar acesso por WebApplicationFactory; endpoint delega ao caso de uso, sem acesso direto ao banco.
- [ ] T015 [US1] Adicionar metadados 200/404/500 e descrições/exemplo em pt-BR em `src/PortalDerick.Api/Profiles/PublicProfileEndpoint.cs`; configurar em `src/PortalDerick.Api/Program.cs` AddOpenApi somente em Development e MapOpenApi/MapScalarApiReference somente nesse ambiente, expondo /openapi/v1.json e /scalar em runtime, sem geração no build, publicação estática, autenticação ou consulta ao banco para documentar.
- [ ] T016 [US1] Executar build e testes US1 de `PortalDerick.slnx`, corrigir falhas relacionadas e registrar comandos/resultados em `output/001-consultar-perfil-publico.md`, incluindo evidência de 200 e das 16 combinações; submeter o incremento à revisão do usuário.

**Checkpoint:** MVP consultável com fixture e retorno básico de 404 implementado e testado; exemplo inicial e cenários aprofundados de ausência pertencem às próximas histórias.

## Fase 4: US2 — Identificar a ausência de perfil (P1)

**Objetivo:** distinguir ausência real, opcionais ausentes e erro técnico.
**Critério independente:** banco isolado com esquema e zero linhas retorna 404 vazio, inclusive após reinício; perfil com opcionais nulos retorna 200; falha técnica retorna 500 sem detalhes.

### Testes

- [ ] T017 [P] [US2] Criar `tests/PortalDerick.Tests/Integration/AbsentProfileTests.cs` cobrindo tabela vazia, 404 sem corpo/substituto, reinício mantendo ausência e contraste com perfil de opcionais nulos retornando 200; confirmar falha antes da implementação correspondente.
- [ ] T018 [P] [US2] Criar `tests/PortalDerick.Tests/Integration/ProfileFailureTests.cs` simulando falha do leitor em Development, Staging e Production: 500 sem dados internos nem página de exceção e nunca 404; qualquer mensagem pública deve estar em pt-BR.

### Implementação

- [ ] T019 [US2] Validar o retorno básico implementado em T014 por meio de `tests/PortalDerick.Tests/Integration/AbsentProfileTests.cs`, aprofundando os cenários com SQLite vazio, reinício sem recriação de perfil e contraste com opcionais nulos; ajustar `src/PortalDerick.Api/Profiles/PublicProfileEndpoint.cs` somente se esses cenários revelarem falhas, mantendo 404 vazio sem substituto e sem páginas de status que preencham a resposta. Não reimplementar o mapeamento básico de ausência da US1.
- [ ] T020 [US2] Configurar tratamento de falhas em `src/PortalDerick.Api/Program.cs` com 500 sem detalhes internos em todos os ambientes e ILogger estruturado sem dados do perfil/segredos; não converter falhas de persistência em ausência nem expor página de exceção no recurso.
- [ ] T021 [US2] Executar testes de US2 e regressão US1 de `PortalDerick.slnx` e registrar evidências de 404 vazio, 200 com opcionais nulos e 500 em `output/001-consultar-perfil-publico.md`; submeter à revisão do usuário.

## Fase 5: US3 — Consultar um exemplo fictício disponível (P2)

**Objetivo:** preparar exatamente um exemplo somente pela migração inicial explícita.
**Critério independente:** banco temporário novo migrado contém Nome do Dev / Dev. de Software e quatro opcionais nulos; reaplicar migrações após remover fixture não recria o registro e GET continua 404 após reinício.

### Testes

- [ ] T022 [US3] Criar `tests/PortalDerick.Tests/Integration/InitialProfileMigrationTests.cs` com arquivo temporário exclusivo, verificando migração inicial, exatamente um perfil e GET 200 com os dados fictícios, remoção controlada da fixture, reaplicação sem recriação e GET 404 após reinício; obter autorização específica antes de executar migrações, inclusive nos testes.

### Implementação

- [ ] T023 [US3] Gerar e revisar a migração inicial em `src/PortalDerick.Infrastructure/Persistence/Migrations/` e snapshot EF correspondente, criando esquema de T006 e inserção explícita pontual de Id 1, Name Nome do Dev, Title Dev. de Software, Bio/Photo/Country/State null; não usar HasData, seed permanente, EnsureCreated em produção ou aplicação automática. Gerar artefatos não autoriza executar a migração.
- [ ] T024 [US3] Após autorização específica, executar testes de migração T022 e preparar somente banco local descartável conforme `specs/001-consultar-perfil-publico/quickstart.md`, conferir consulta inicial e repetida e registrar comandos, autorização e resultados em `output/001-consultar-perfil-publico.md`; manter banco fora do versionamento e submeter à revisão do usuário. Sem autorização, registrar esta validação como bloqueada.

## Fase 6: Acabamento e validação transversal

- [ ] T025 [P] Criar `tests/PortalDerick.Tests/Integration/ApiDocumentationTests.cs` após T015/T020: em Development validar OpenAPI/HTML Scalar, operação pública, seis propriedades obrigatórias/quatro anuláveis, respostas 200/404/500 e textos próprios/exemplo pt-BR; testar documento sem perfil e com leitor que falharia se consultado. Em Staging/Production validar 404 em /openapi/v1.json, /scalar e caminhos subordinados reais da UI, preservando GET /api/perfil.
- [ ] T026 [P] Atualizar `specs/001-consultar-perfil-publico/quickstart.md` após implementação com versões autorizadas, comandos efetivos, configuração externa e roteiro de preparação/consulta e ambientes; preservar exigência de autorização para migrações e ausência de endpoints auxiliares.
- [ ] T027 Executar restore/build/suíte integral de `PortalDerick.slnx`, conforme quickstart e autorizações existentes, corrigindo falhas introduzidas; verificar manualmente carregamento e execução de GET no Scalar em Development e indisponibilidade fora dele com banco já autorizado, sem repetir preparação desnecessária; registrar resultados e limitações em `output/001-consultar-perfil-publico.md`.
- [ ] T028 Consolidar em `output/001-consultar-perfil-publico.md` cobertura FR-001–FR-009/SC-001–SC-005, arquivos alterados, decisões, evidências, pendências e estados implementado/validado/bloqueado/aguardando aprovação; solicitar revisão final sem declarar aprovação, realizar commits, pushes ou deploys.

## Dependências e ordem de execução

```text
T001 → T002 → T003 → T004 → T005 → T006 → T007
Fundação → US1 (T008–T016) → US2 (T017–T021) → US3 (T022–T024)
US1 + US2 → T025
Implementação das histórias → T026
US1 + US2 + US3 + T025 + T026 → T027 → T028
```

- T003 também pode ser preparado em paralelo a T002; T004 aguarda ambos.
- US2 reutiliza a consulta integrada da US1; US3 reutiliza a consulta e o resultado de ausência. Cada fase tem cenário isolado de verificação, mas não é uma implementação independente da base anterior.
- Dentro da US1: T008/T009/T010 podem ser escritos em paralelo; T011 precede T012/T013; ambos precedem T014; T015 sucede T014; T016 aguarda todos. Testes podem inicialmente falhar por componentes incompletos, mas a evidência relevante deve exercitar o comportamento depois que compilarem.
- US2: T017/T018 em paralelo; T019 aprofunda a validação de ausência após T017; T020 implementa o tratamento de falhas após T018; T021 aguarda todos.
- US3: T022 precede T023; execução em T024 exige autorização operacional. Não existe paralelismo útil dentro desta história: teste e migração compartilham dependência sequencial.
- T025/T026 podem ocorrer em paralelo, em arquivos diferentes; T027 aguarda todas as histórias e essas tarefas. Atualizar o relatório após cada tarefa executada, preservando histórico e serializando alterações nesse arquivo compartilhado.

## Exemplos de execução paralela por história

- **US1:** após a fundação, escrever T008 (domínio), T009 (contrato) e T010 (SQLite) em arquivos distintos; após T011, implementar T012 (leitor) e T013 (DTO) simultaneamente.
- **US2:** após US1, escrever T017 (ausência) e T018 (falha técnica) simultaneamente; depois executar T019 (validação aprofundada de ausência) e T020 (tratamento de falhas no host), coordenando eventual ajuste no endpoint.
- **US3:** executar T022 → T023 → T024 sequencialmente; não marcar artificialmente tarefas dependentes com `[P]`.

Esses exemplos descrevem oportunidades de trabalho; não solicitam agentes paralelos nem concedem autorizações operacionais.

## Estratégia de implementação

1. Confirmar autorização de implementação e versões de dependências antes da preparação; validar a base antes dos comportamentos.
2. Entregar primeiro fundação + US1 como MVP com perfil preparado nas fixtures; demonstrar os seis campos, opcionais independentes e consulta sem escrita.
3. Acrescentar US2 para comprovar ausência e erros; acrescentar US3 para preparação inicial explícita e idempotência do histórico de migrações.
4. Validar documentação por ambiente, suíte completa e roteiro manual antes da revisão final. A entrega integral exige as três histórias, ainda que US1 seja o primeiro incremento.
5. Interromper somente a operação que dependa de autorização não concedida e registrar o bloqueio. Não executar migrações, exclusões manuais, mudanças compartilhadas, commits, pushes ou deploys sem a autorização pertinente.

## Notas de rastreabilidade e limites

- US1: FR-001–FR-005, FR-007/008 e SC-001/002/005; US2: FR-006 e SC-003; US3: FR-009 e SC-004. T025/T027 cobrem documentação e regressão transversal.
- Não há interface visual de produto, projeto Blazor, autenticação, upload, links sociais ou edição. Scalar é documentação técnica em Development.
- O metadado histórico `main` em spec.md diverge do contexto ativo `001-consultar-perfil-publico`; isso não muda as tarefas nem autoriza alterar a especificação.
- Todas as tarefas permanecem abertas até execução e evidências; a aprovação final cabe ao usuário.
