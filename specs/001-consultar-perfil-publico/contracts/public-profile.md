# Contrato HTTP — Consulta pública

## Requisição

`GET /api/perfil`

Sem autenticação, corpo, identificador, paginação ou parâmetros de idioma. Um único recurso; não há rota de coleção nem operações de escrita. Cabeçalhos de idioma não alteram o conteúdo pt-BR.

## Perfil existente

HTTP 200, `Content-Type: application/json` (parâmetro charset permitido).

```json
{
  "nome": "Nome do Dev",
  "titulo": "Dev. de Software",
  "bio": null,
  "foto": null,
  "pais": null,
  "estado": null
}
```

As seis propriedades são obrigatórias na resposta. `nome` e `titulo` são strings preenchidas. As quatro restantes são string ou null, independentemente umas das outras. Foto preenchida contém somente a referência textual existente; o serviço não resolve seu conteúdo. Não há id, links sociais ou metadados adicionais. A serialização deve manter valores nulos explícitos. Os textos acima são dados iniciais escolhidos, não valores fixos de toda resposta.

## Perfil ausente

HTTP 404 com corpo vazio, sem dados de perfil ou exemplo substituto. Não habilitar middleware de páginas de status que gere corpo para esse resultado.

## Erros técnicos e limites

Falhas técnicas produzem HTTP 500 sem detalhes internos; não devem ser convertidas em 404. Não se estabelece esquema de erro público adicional. Eventuais mensagens ao consumidor devem ser pt-BR, inclusive no ambiente usado para validação do contrato; não expor página de exceção no endpoint.

Somente GET é mapeado para esse recurso. POST/PUT/PATCH/DELETE não executam operações e, na rota conhecida, devem ser rejeitados por HTTP 405. Não criar endpoints para preparar fixtures, inserir o exemplo ou apagar o perfil.

## Rastreabilidade

| Requisitos | Verificação |
| --- | --- |
| FR-001, FR-002, SC-001 | GET sem credenciais ou id, mesmo recurso em leituras sucessivas |
| FR-003 a FR-005, SC-002 | Igualdade dos valores e presença das seis chaves nas 16 combinações |
| FR-006, SC-003 | Fixture sem registro → 404 vazio; opcionais nulos → 200 |
| FR-007, SC-005 | Conteúdo pt-BR; cabeçalhos de idioma não negociam tradução |
| FR-008 | Nenhuma escrita por GET; métodos de alteração rejeitados |
| FR-009, SC-004 | Banco recém-preparado contém um exemplo com opcionais nulos |

## Documentação técnica por ambiente

Conforme Constitution 1.1.0, Microsoft.AspNetCore.OpenApi gera a descrição do endpoint e Scalar.AspNetCore apresenta a interface interativa exclusivamente em Development:

- `GET /openapi/v1.json`: documento gerado em runtime; descreve GET /api/perfil sem autenticação, 200 com as seis propriedades obrigatórias (quatro aceitam null), 404 sem conteúdo e 500 sem detalhes internos. Textos próprios e exemplo em pt-BR, com Nome do Dev e Dev. de Software.
- `/scalar`: interface interativa que consome o documento acima e permite executar a consulta.
- Fora de Development: geração não registrada, rotas OpenAPI/Scalar e subordinadas não mapeadas; requisições a elas retornam 404. Não publicar documento estático.

Essas rotas são de documentação técnica; não acrescentam operações sobre o perfil. O endpoint funcional mantém o mesmo contrato em todos os ambientes. Gerar a documentação não consulta nem modifica dados do perfil.