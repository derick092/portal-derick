# Feature Specification: Consulta pública do perfil profissional

**Feature Branch**: `main` (branch atual; nenhuma branch criada)

**Created**: 2026-11-22

**Status**: Draft — aguardando revisão do usuário

**Input**: User description: "Disponibilizar via serviço de forma pública os dados de um único perfil profissional (não existirão outros), contendo nome, título, bio (opcional), foto (opcional), localização (país e estado, também opcionais), respeitando os campos opcionais. Retornar os dados existentes ou HTTP 404 quando não houver perfil. Somente pt-BR, sem edição, autenticação, links sociais ou upload. Disponibilizar um perfil fictício para consulta, com nome e título preenchidos e campos opcionais nulos."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Consultar o perfil profissional (Priority: P1)

Como consumidor público do serviço, quero consultar o único perfil profissional sem autenticação para obter sua apresentação e os dados disponíveis.

**Why this priority**: A consulta pública é o objetivo central da funcionalidade.

**Independent Test**: Disponibilizar um perfil conhecido e consultá-lo sem credenciais, conferindo os dados retornados.

**Acceptance Scenarios**:

1. **Given** um perfil com todos os campos preenchidos, **When** houver uma consulta pública sem credenciais, **Then** o serviço retorna HTTP 200 com nome, título, bio, foto, país e estado iguais aos dados existentes.
2. **Given** um perfil com nome e título preenchidos e todos os opcionais ausentes, **When** houver uma consulta, **Then** o serviço retorna HTTP 200 com nome e título e valores `null` explícitos para bio, foto, país e estado.
3. **Given** um perfil com apenas parte dos opcionais preenchida, **When** houver uma consulta, **Then** os valores existentes são preservados e cada opcional ausente retorna `null`, inclusive quando apenas país ou apenas estado estiver preenchido.
4. **Given** o perfil disponível, **When** houver consultas repetidas sem indicar identificador de perfil, **Then** todas retornam o mesmo perfil, sem seleção entre profissionais e sem alterar seus dados.

---

### User Story 2 - Identificar a ausência de perfil (Priority: P1)

Como consumidor público do serviço, quero reconhecer quando não existe perfil para distinguir essa situação de um perfil com informações opcionais ausentes.

**Why this priority**: A ausência tem um resultado exigido explicitamente e diferente de uma consulta bem-sucedida.

**Independent Test**: Consultar o serviço em uma condição sem perfil e verificar o resultado.

**Acceptance Scenarios**:

1. **Given** nenhum perfil existente, **When** houver uma consulta pública, **Then** o serviço retorna HTTP 404 sem apresentar dados de perfil nem substituir a ausência por um perfil fictício.
2. **Given** um perfil existente sem dados opcionais, **When** houver uma consulta, **Then** o resultado é HTTP 200, e não HTTP 404.

---

### User Story 3 - Consultar um exemplo fictício disponível (Priority: P2)

Como consumidor do serviço, quero encontrar um perfil fictício na disponibilização inicial para experimentar a consulta com os dados mínimos exigidos.

**Why this priority**: Permite demonstrar e verificar o serviço sem depender de dados pessoais reais.

**Independent Test**: Consultar o serviço na disponibilização inicial e verificar que existe um único perfil fictício com nome e título preenchidos e todos os opcionais nulos, sem exigir textos específicos para nome e título.

**Acceptance Scenarios**:

1. **Given** a disponibilização inicial preparada para consulta, **When** houver uma consulta pública, **Then** o único perfil retornado é fictício, tem nome e título preenchidos e bio, foto, país e estado iguais a `null`.

### Edge Cases

- Todos os opcionais ausentes: o perfil continua disponível e cada opcional retorna `null`, sem omissão ou substituição por texto vazio.
- País preenchido e estado ausente, ou o inverso: cada valor é tratado independentemente, sem inferir informações.
- Nenhum perfil existente: retorna HTTP 404; o exemplo fictício não é uma resposta alternativa automática.
- Consultas sucessivas: não criam, modificam ou duplicam o perfil.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O serviço DEVE permitir consulta pública ao perfil profissional sem credenciais, login ou autenticação (cenário 1.1).
- **FR-002**: O serviço DEVE tratar o perfil como único, admitindo sua ausência, sem listagem, seleção por identificador ou suporte a outros perfis (cenários 1.4 e 2.1).
- **FR-003**: Quando houver perfil, o serviço DEVE retornar HTTP 200 e os dados existentes de nome, título, bio, foto, país e estado (cenários 1.1 a 1.3).
- **FR-004**: Nome e título DEVEM estar preenchidos no perfil, não podendo ser nulos, vazios ou compostos apenas por espaços (cenários 1.1 e 3.1).
- **FR-005**: Bio, foto, país e estado DEVEM ser opcionais individualmente; cada campo ausente DEVE retornar `null` explícito, preservando os valores dos demais campos e sem exigir o preenchimento de outro opcional (cenários 1.2 e 1.3).
- **FR-006**: Quando não houver perfil, o serviço DEVE retornar HTTP 404 sem dados de perfil e sem gerar automaticamente um substituto fictício (cenário 2.1).
- **FR-007**: O conteúdo disponibilizado e eventuais mensagens destinadas ao consumidor DEVEM estar somente em pt-BR, sem seleção ou negociação de idioma.
- **FR-008**: A funcionalidade DEVE se limitar à consulta: não disponibilizar criação, edição, exclusão, autenticação, links sociais ou upload. Consultas NÃO DEVEM alterar o perfil (cenário 1.4).
- **FR-009**: A disponibilização inicial DEVE conter exatamente um perfil fictício com nome e título preenchidos e todos os opcionais nulos, sem exigir valores textuais específicos para os campos obrigatórios (cenário 3.1).

### Key Entities *(include if feature involves data)*

- **Perfil profissional**: Único profissional apresentado pelo serviço. Contém nome e título obrigatórios, bio e foto opcionais e localização composta de país e estado, ambos opcionais de forma independente. A inexistência do perfil é diferente da ausência de seus campos opcionais.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em 100% dos cenários de consulta com perfil existente, o consumidor obtém os dados correspondentes sem fornecer credenciais ou escolher um profissional.
- **SC-002**: Em 100% das combinações de presença e ausência dos quatro campos opcionais, os valores preenchidos são preservados e os ausentes são explicitamente nulos.
- **SC-003**: Em 100% das consultas sem perfil, o consumidor consegue distinguir a ausência por HTTP 404; um perfil com opcionais ausentes permanece consultável com sucesso.
- **SC-004**: Na disponibilização inicial, uma única consulta pública permite obter o perfil fictício com os dois campos obrigatórios preenchidos e os quatro opcionais nulos.
- **SC-005**: Todo conteúdo destinado ao consumidor está em pt-BR e a consulta não exige etapas de cadastro ou edição.

## Assumptions

- A entrega compreende o serviço de consulta; não inclui interface visual.
- O exemplo fictício é um dado inicial para demonstração, e não uma resposta fixa que esconda a inexistência de perfil.
- Os valores textuais de nome e título do perfil fictício serão definidos no planejamento; não constituem requisitos de negócio.
- País e estado são informações independentes; não se presume país a partir de estado nem o contrário.
- A forma de representar a foto e de organizar os campos na resposta será definida no planejamento técnico, respeitando sua opcionalidade e a exclusão de upload.
- Não há dependência funcional de serviços externos, autenticação ou conteúdo em outros idiomas. A consulta depende apenas da disponibilidade dos dados do perfil, quando existentes.
- Não se estabelecem metas de latência ou capacidade sem necessidade demonstrada, conforme a Constitution.
