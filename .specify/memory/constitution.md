# Portal Derick Constitution

## Core Principles

### I. Tecnologias e dependências

- Utilizar .NET 10 e a versão padrão correspondente do C#.
- Utilizar ASP.NET Core Minimal APIs para endpoints HTTP.
- Utilizar OpenAPI para documentar os endpoints HTTP e Scalar como interface interativa dessa
  documentação. Disponibilizar a interface interativa Scalar exclusivamente no ambiente de
  desenvolvimento.
- Utilizar Blazor Web App para a interface, quando essa etapa for iniciada.
- Utilizar SQLite com Entity Framework Core para persistência.
- Utilizar xUnit para testes automatizados.
- Justificar a escolha do mecanismo de autenticação no planejamento técnico, conforme os
  requisitos da especificação.
- Priorizar recursos nativos do .NET e bibliotecas oficiais da Microsoft quando atenderem
  adequadamente aos requisitos.
- Preferir soluções simples, consolidadas e de fácil manutenção, evitando dependências externas
  desnecessárias. Bibliotecas externas são aceitáveis quando seus benefícios justificarem os
  custos de manutenção, segurança, compatibilidade e complexidade, inclusive em comparação com
  uma implementação própria.
- Justificar tecnicamente novas dependências; sua introdução ou mudança de versão exige
  autorização conforme as regras operacionais do projeto. Serviços pagos e dependências
  comerciais exigem aprovação explícita.

### II. Arquitetura e simplicidade

- Adotar Clean Architecture como padrão arquitetural, organizando inicialmente a solução em
  projetos separados: PortalDerick.Domain, PortalDerick.Application, PortalDerick.Infrastructure
  e PortalDerick.Api.
- Respeitar a direção das dependências e manter as regras de negócio independentes de mecanismos
  de persistência e de interfaces externas.
- Manter regras de negócio fora dos endpoints HTTP; endpoints não devem acessar diretamente o
  banco de dados.
- Organizar comportamentos de aplicação em casos de uso quando isso tornar claras suas
  responsabilidades, sem exigir classes ou abstrações artificiais para operações triviais.
- Criar interfaces, repositórios e outras abstrações somente quando houver necessidade concreta.
- Priorizar soluções simples que atendam aos requisitos atuais; aumentar a complexidade somente
  diante de necessidade demonstrável.
- Adicionar outros projetos, como PortalDerick.Web, conforme a evolução aprovada do sistema,
  preservando a consistência arquitetural.

### III. Segurança

- Aplicar autenticação e autorização aos recursos protegidos, de acordo com os requisitos de
  cada funcionalidade.
- Validar entradas e tratar dados externos como não confiáveis; utilizar os mecanismos de
  proteção adequados às interfaces e operações expostas.
- Nunca armazenar segredos, credenciais, tokens ou chaves privadas diretamente no código-fonte
  ou em arquivos versionados.
- Utilizar .NET User Secrets no desenvolvimento e mecanismos adequados em produção, como
  variáveis de ambiente ou gerenciadores de segredos.
- Não registrar informações sensíveis em logs nem expô-las desnecessariamente em respostas de
  erro.
- Avaliar riscos de segurança relevantes no planejamento e na validação das funcionalidades,
  sem presumir controles não exigidos pelo contexto.

### IV. Convenções, qualidade e testes

- Seguir as convenções oficiais aplicáveis de nomenclatura, formatação e estilo de C# e .NET da
  Microsoft, concretizando as regras automatizáveis em um .editorconfig versionado e revisado
  pelo projeto.
- Manter consistência com as convenções aprovadas do repositório, desde que não contrariem esta
  Constitution.
- Utilizar recursos estáveis e compatíveis com a stack adotada, sem introduzir complexidade
  desnecessária.
- Implementar testes automatizados para regras de negócio, comportamentos relevantes e
  cenários críticos.
- Utilizar testes unitários e de integração conforme a necessidade, priorizando verificações
  confiáveis e significativas, sem percentual mínimo obrigatório de cobertura.
- Utilizar injeção de dependências para serviços com dependências, de maneira proporcional à
  solução.
- Implementar logs estruturados utilizando ILogger, evitando exposição de informações sensíveis.

### V. Desempenho e escalabilidade

- Projetar para as necessidades e o volume esperados do Portal Derick, sem introduzir
  infraestrutura ou mecanismos de escalabilidade antecipadamente.
- Evitar consultas, transferências de dados e processamento desnecessários, especialmente em
  caminhos de uso frequente.
- Medir ou demonstrar gargalos antes de introduzir otimizações complexas, caches ou novos
  componentes de infraestrutura.
- Estabelecer metas quantitativas de desempenho e capacidade apenas quando houver requisitos
  ou evidências que as justifiquem.

## Governance

- Esta Constitution orienta as decisões técnicas e prevalece sobre convenções locais e planos
  que a contradigam.
- Decisões específicas de funcionalidades pertencem às respectivas especificações e planos
  técnicos, respeitando estes princípios.
- Alterações desta Constitution exigem justificativa, avaliação de impacto e aprovação explícita
  do usuário.
- Exceções aos princípios devem ser identificadas, justificadas e aprovadas antes de sua adoção;
  quando representarem mudança permanente, devem resultar em emenda à Constitution.
- Divergências relevantes devem ser resolvidas antes da conclusão das tarefas afetadas.
- Versionar emendas conforme versionamento semântico: MAJOR para remoções ou redefinições
  incompatíveis de princípios ou governança; MINOR para novos princípios, seções ou ampliações
  materiais das diretrizes; PATCH para esclarecimentos e correções sem mudança de significado.
- Revisar a conformidade com esta Constitution durante o planejamento técnico e a validação das
  tarefas afetadas, registrando divergências e exceções para aprovação do usuário.

**Version**: 1.1.0 | **Ratified**: 2026-11-22 | **Last Amended**: 2026-11-22
