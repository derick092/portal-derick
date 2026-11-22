# Modelo de dados — Perfil profissional

## Entidade `ProfessionalProfile`

| Campo interno | Persistência | JSON | Regra |
| --- | --- | --- | --- |
| Id | INTEGER PK, CHECK Id = 1 | não exposto | Identidade técnica fixa; zero ou um registro |
| Name | TEXT NOT NULL | nome | String não nula, não vazia e não composta somente por espaços |
| Title | TEXT NOT NULL | titulo | Mesma regra de Name |
| Bio | TEXT NULL | bio | Opcional independente |
| Photo | TEXT NULL | foto | Referência textual de imagem; nenhum processamento remoto |
| Country | TEXT NULL | pais | Opcional independente de State |
| State | TEXT NULL | estado | Opcional independente de Country |

Tabela: `ProfessionalProfiles`. Sem relações ou outras entidades. O identificador existe exclusivamente para persistência; não há seleção de profissional pelo consumidor.

O domínio protege Name e Title na construção/materialização por caminho validado, incluindo teste para espaços em branco. O mapeamento EF deve preservar essa garantia, sem setters públicos que permitam invalidá-la. Não impor truncamento, tamanho máximo, enumeração geográfica ou formato de foto não exigidos. Valores opcionais presentes são preservados; ausência é representada por null, nunca convertida em texto vazio. Não inferir país/estado nem normalizar silenciosamente os dados lidos.

## Estados e ciclo de vida

- **Ausente:** tabela sem registro → 404, sem corpo.
- **Existente:** registro com nome/título válidos → 200; opcionais podem ser nulos.
- Preparação inicial autorizada aplica a migração uma vez e inclui o exemplo definido em `research.md`.
- GET e reinício da aplicação não fazem transições de estado. Alterações de fixtures são exclusivas de testes isolados; não existem operações HTTP de escrita.
- Falha técnica de persistência não equivale ao estado ausente.

## Validação planejada

Testar obrigatórios inválidos, 16 combinações de opcionais, tentativa de segundo registro e de Id diferente de 1, ausência e leitura repetida sem alteração. O banco garante cardinalidade; a aplicação garante as regras textuais de domínio.
