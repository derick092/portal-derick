# Guia de validação — 001-consultar-perfil-publico

Este guia descreve a execução **após implementação autorizada**. Os projetos e testes indicados ainda não existem. Não interpretar os comandos abaixo como verificações já executadas.

## Pré-requisitos

- SDK .NET 10; solução e projetos propostos em plan.md implementados.
- Dependências e versões aprovadas, com ferramenta local dotnet-ef compatível e manifesto versionado preparado na implementação.
- Autorização específica antes de aplicar migrações. Usar somente arquivo SQLite local descartável para demonstração; testes usam seus próprios bancos isolados. Não usar dados compartilhados.
- A configuração ConnectionStrings:PortalDerick deve ser consumida pela implementação; banco e arquivos auxiliares não devem ser versionados.

## Build e testes

Na raiz do repositório, após aprovação das dependências:

```powershell
dotnet tool restore
dotnet restore PortalDerick.slnx
dotnet build PortalDerick.slnx --no-restore
dotnet test PortalDerick.slnx --no-build
```

Esperado: build e suíte integral sem falhas. Como a solução inicial inteira é afetada, executar toda a suíte. Testes de persistência/migração devem usar bancos temporários exclusivos e não aplicar mudanças ao arquivo de demonstração. Alterações de fixtures ficam no processo de teste, sem endpoints auxiliares.

## Preparação inicial e consulta manual

Depois de autorizada a aplicação da migração, escolher um arquivo novo em diretório local existente, fora dos arquivos versionados. Em PowerShell:

```powershell
$env:ConnectionStrings__PortalDerick = "Data Source=$env:TEMP/portal-derick-spec001-demo.db"
dotnet ef database update --project src/PortalDerick.Infrastructure --startup-project src/PortalDerick.Api
dotnet run --project src/PortalDerick.Api --no-launch-profile -- --urls http://localhost:5080
```

A migração inicial deverá criar o esquema e o único perfil fictício; startup e GET não executam migração nem seed. Não reutilizar um arquivo contendo dados relevantes. O comando database update modifica o banco e só pode ser executado com a autorização indicada.

Em outro terminal:

```powershell
$response = Invoke-WebRequest -Uri http://localhost:5080/api/perfil
$response.StatusCode
$response.Content
```

Esperado: 200, JSON conforme [contrato](contracts/public-profile.md), nome Nome do Dev, título Dev. de Software e quatro opcionais presentes com null. Repetir a consulta e conferir que não mudou o registro. Encerrar o processo com Ctrl+C.

## Cenários automatizados exigidos

| Cenário / preparação isolada | Resultado esperado |
| --- | --- |
| Perfil completo, GET sem credenciais | 200 e seis valores iguais aos persistidos |
| Cada uma das 16 combinações dos quatro opcionais | 200, seis chaves presentes; valor existente preservado e ausente null |
| Nenhuma linha, esquema existente | 404, corpo vazio, sem exemplo gerado |
| Reinício com banco sem perfil | Continua 404 |
| GET repetido | Mesmo perfil, sem alteração ou duplicação de registros |
| Nome/título nulo, vazio ou somente espaços no domínio | Construção inválida rejeitada |
| Segundo registro ou chave diferente de 1 no SQLite | Restrição rejeita a gravação |
| Arquivo novo com migração inicial | Exatamente um perfil fictício com obrigatórios preenchidos e opcionais nulos |
| Remover fixture em banco de teste e reaplicar migrações já aplicadas | Continua vazio; GET retorna 404 |
| Cabeçalhos de idioma variados | Conteúdo permanece pt-BR |
| POST/PUT/PATCH/DELETE em /api/perfil | 405 e nenhum dado alterado |
| Falha técnica de persistência simulada | 500 sem detalhes internos; não 404 |

A suíte deve preparar cenários com perfil completo, parcial e ausente diretamente nas fixtures. Não solicitar remoções manuais no banco de demonstração para provar 404. Referência de campos e invariantes: [modelo](data-model.md).

## Evidências a registrar na implementação

Registrar comandos realmente executados e códigos de saída, resultado da suíte completa, evidências de 200/404 e do exemplo inicial, e qualquer limitação em output/001-consultar-perfil-publico.md. Não declarar validação funcional quando houver falhas ou verificações não executadas.

## Validação OpenAPI e Scalar por ambiente

Após implementação, com dependências Microsoft.AspNetCore.OpenApi e Scalar.AspNetCore disponíveis e banco de demonstração já preparado conforme autorização, iniciar a aplicação explicitamente em Development (sem launch profile):

```powershell
$env:DOTNET_ENVIRONMENT = 'Development'
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project src/PortalDerick.Api --no-launch-profile -- --urls http://localhost:5080
```

Em outro terminal, consultar o documento:

```powershell
$document = Invoke-RestMethod http://localhost:5080/openapi/v1.json
$document.paths.'/api/perfil'.get
```

Esperado: documento válido contendo a operação pública, respostas 200/404/500 e schema conforme contracts/public-profile.md, incluindo campos obrigatórios e nulls. Abrir `http://localhost:5080/scalar` no navegador: interface deve carregar o documento e executar GET /api/perfil, mostrando o perfil fictício já definido. A documentação não pode depender da presença de um perfil no banco.

Encerrar o processo. Repetir a inicialização com DOTNET_ENVIRONMENT e ASPNETCORE_ENVIRONMENT definidos como Staging e depois Production, em processos separados. Em ambos, `/openapi/v1.json`, `/scalar` e caminhos subordinados da UI devem retornar 404, e `/api/perfil` deve continuar retornando 200 para o banco preparado. Não executar migrações novamente para essa verificação. Restaurar as variáveis ao estado anterior ao terminar.

Adicionar à suíte de integração os três ambientes, validando status e conteúdo OpenAPI/HTML em Development, indisponibilidade das rotas fora dele e manutenção do comportamento da consulta. Os comandos desta seção são roteiro futuro e não foram executados nesta revisão documental.