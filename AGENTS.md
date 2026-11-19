# Instruções gerais para o Agente

## 1. Autonomia e permissões

- A solicitação explícita do usuário autoriza a execução da tarefa solicitada, dentro do escopo definido.
- A existência de especificações, planos ou tarefas aprovadas não constitui autorização automática para implementá-las.
- Antes de executar, verificar o escopo da solicitação e esclarecer ambiguidades relevantes quando necessário.
- Executar as atividades necessárias dentro do escopo autorizado, sem solicitar novamente aprovação para a mesma tarefa.
- Consultar o usuário antes de realizar alterações relevantes fora do escopo autorizado.
- Nunca executar commits ou pushes sem autorização explícita.
- Solicitar autorização específica antes de excluir arquivos, executar migrações de banco, alterar infraestrutura, realizar deploys ou modificar dados em ambientes compartilhados.

## 2. Gerenciamento de escopo

- Corrigir problemas encontrados que estejam relacionados ao escopo aprovado, respeitando os limites de autorização.
- Comunicar problemas sem relação com a tarefa para que sejam considerados posteriormente.
- Não alterar requisitos por conta própria.
- Informar impedimentos e propor alternativas, solicitando aprovação quando houver mudança de escopo.
- Utilizar como fontes de contexto a Constitution, as especificações, os planos, as tarefas e os demais documentos técnicos pertinentes.
- Não tratar relatórios de execução como fontes de requisitos ou decisões arquiteturais.

## 3. Qualidade e consistência arquitetural

### Princípios gerais

- Priorizar simplicidade, legibilidade e facilidade de manutenção.
- Evitar abstrações, funcionalidades e complexidade desnecessárias.
- Preservar alterações preexistentes do usuário.
- Não inserir segredos, credenciais ou dados sensíveis no código, nos logs ou em arquivos versionados.
- Utilizar dependências já aprovadas quando pertinentes à tarefa.
- Justificar e solicitar autorização antes de adicionar, atualizar ou substituir dependências e ferramentas.

### Consistência arquitetural e padrões do projeto

- Antes de implementar, identificar a organização, os padrões e as convenções relevantes à tarefa.
- Consultar a Constitution e os documentos técnicos aplicáveis para compreender as decisões aprovadas.
- Examinar implementações semelhantes no repositório e utilizá-las como referência quando estiverem alinhadas às diretrizes do projeto.
- Manter consistência com as responsabilidades das camadas, organização do código, nomenclatura e tratamento de erros existentes.
- Não introduzir novos padrões arquiteturais, camadas ou abstrações sem necessidade justificada e autorização.
- Não reproduzir práticas existentes que contrariem as diretrizes aprovadas apenas para manter consistência.
- Quando houver divergências entre documentação e implementação, comunicar o conflito antes de tomar decisões que afetem a arquitetura.

### Interfaces visuais

- Para tarefas envolvendo interfaces, layouts ou identidade visual, consultar `design-guidelines.md` na raiz do repositório, quando disponível e aprovado.
- Não presumir a existência desse documento nem criar diretrizes visuais sem solicitação.

## 4. Validação

### Antes da implementação

- Verificar o estado atual do projeto por meio de compilação e testes pertinentes, quando aplicáveis.
- Identificar falhas preexistentes e avaliar seu impacto na tarefa.
- Quando uma falha preexistente comprometer a implementação ou sua validação, interromper e comunicar o problema.
- Falhas independentes da tarefa não precisam impedir sua execução, desde que não comprometam a validação e sejam registradas.

### Após a implementação

- Compilar os projetos afetados e executar os testes relacionados às alterações, quando aplicáveis.
- Executar a suíte completa de testes quando houver alterações compartilhadas, risco relevante de regressão ou ao concluir a especificação.
- Corrigir todas as falhas introduzidas pela implementação antes de solicitar aprovação.
- Não declarar verificações como bem-sucedidas sem executá-las.
- Registrar comandos executados, resultados, falhas e limitações de validação.
- Não considerar uma implementação validada enquanto houver falhas nas verificações exigidas.
- A aprovação final de cada tarefa cabe exclusivamente ao usuário.

## 5. Relatórios de execução

### Organização

- Manter um único relatório Markdown por especificação em `output/<referencia-spec>.md`.
- Utilizar o identificador da especificação definido pelo Spec Kit.
- Atualizar o relatório após cada tarefa executada, preservando o histórico das tarefas anteriores.
- Gerar ou atualizar o relatório mesmo quando a execução estiver bloqueada ou apresentar falhas.
- Não considerar a geração ou atualização do relatório como aprovação da tarefa.

### Conteúdo

Cada relatório deve conter:

- **Estado geral:** situação da implementação da especificação.
- **Tarefas:** identificadores, resumo do trabalho realizado e estado de cada tarefa.
- **Implementação:** decisões técnicas relevantes e respectivas justificativas.
- **Arquivos:** arquivos criados, modificados ou excluídos.
- **Validação:** comandos executados, resultados e evidências das verificações.
- **Pendências:** falhas preexistentes, problemas não resolvidos, limitações e pontos que exigem atenção.

Para cada pendência, registrar sua origem, impacto e estado.

Destacar na comunicação final problemas que possam afetar outras especificações.

Manter os relatórios objetivos e proporcionais à complexidade das tarefas.

### Isolamento de contexto

- Os arquivos de `output/` destinam-se exclusivamente ao registro e à revisão das execuções.
- Não consultar, pesquisar ou carregar relatórios de outras especificações durante a preparação, planejamento ou implementação de tarefas.
- É permitido consultar o relatório da especificação atual quando necessário para atualizá-lo e preservar seu histórico.
- Consultar relatórios de outras especificações somente mediante autorização explícita do usuário.
- Não utilizar relatórios anteriores como substitutos da documentação técnica aprovada.

## 6. Comunicação

- Comunicar-se em português brasileiro, de forma clara e objetiva.
- Fornecer atualizações durante tarefas demoradas.
- Ao finalizar cada tarefa, resumir as alterações e informar os arquivos modificados.
- Apresentar evidências das verificações realizadas.
- Informar pendências, impedimentos e limitações.
- Distinguir tarefas implementadas, validadas, bloqueadas e pendentes de aprovação.
- Não declarar uma tarefa concluída ou aprovada sem a validação e aprovação do usuário.
