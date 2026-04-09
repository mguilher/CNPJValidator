# novo-cnpj — Validador de CNPJ Alfanumérico em C#

Exemplo de como validar e tipar o novo **CNPJ Alfanumérico** da Receita Federal, implementado em C# puro com suporte ao formato legado (numérico) e ao novo formato alfanumérico previsto para entrar em vigor em **julho de 2026**.

---

## O que é o novo CNPJ Alfanumérico?

O **CNPJ (Cadastro Nacional da Pessoa Jurídica)** é o número de identificação das empresas e organizações junto à Receita Federal do Brasil. Até hoje ele é composto apenas por **14 dígitos numéricos** no formato `XX.XXX.XXX/XXXX-DD`.

Com o crescimento contínuo do número de empresas abertas no Brasil, a capacidade do formato puramente numérico estava chegando ao limite. Para resolver isso, a Receita Federal publicou a **Instrução Normativa RFB nº 2.229, de 15 de outubro de 2024**, criando o **CNPJ Alfanumérico**: um novo formato que combina letras maiúsculas (A–Z) e números (0–9) nas 12 primeiras posições.

> **Importante:** os CNPJs numéricos já existentes **não serão alterados**. O novo formato será atribuído apenas às **novas inscrições a partir de julho de 2026**.

### Cronograma oficial

| Data                  | Evento                                                  |
| --------------------- | ------------------------------------------------------- |
| 15 de outubro de 2024 | Publicação da Instrução Normativa RFB nº 2.229          |
| 25 de outubro de 2024 | Entrada em vigor da Instrução Normativa                 |
| Julho de 2026         | Início da emissão de CNPJs no novo formato alfanumérico |

---

## Estrutura do CNPJ Alfanumérico

Um CNPJ tem sempre **14 caracteres** divididos em três partes:

```
X X . X X X . X X X / X X X X - D D
└───────────────────┘ └──────┘ └──┘
    Raiz (8 chars)    Ordem   Dígitos
                     (4 chars) Verificadores
```

| Parte                     | Posições | Conteúdo                                       | Exemplo    |
| ------------------------- | -------- | ---------------------------------------------- | ---------- |
| **Raiz**                  | 1–8      | Identifica a empresa (pode ter letras)         | `12ABC345` |
| **Ordem**                 | 9–12     | Identifica o estabelecimento (pode ter letras) | `01AB`     |
| **Dígitos Verificadores** | 13–14    | Sempre numéricos (0–9)                         | `77`       |

No novo formato, as posições de 1 a 12 aceitam os caracteres `A–Z` e `0–9`. As posições 13 e 14 (dígitos verificadores) **continuam sendo sempre numéricas**.

A máscara de exibição é: `XX.XXX.XXX/XXXX-DD`

---

## Como os Dígitos Verificadores são calculados

O algoritmo segue o padrão **módulo 11** definido pela Receita Federal, adaptado para caracteres alfanuméricos.

### Conversão de caracteres

Cada caractere da base é convertido para um valor numérico subtraindo 48 do seu código ASCII:

- Dígitos `0–9` → valores `0` a `9`
- Letras maiúsculas `A–Z` → valores `17` a `42`
  - `'A'` = 65 − 48 = **17**
  - `'B'` = 66 − 48 = **18**
  - `'Z'` = 90 − 48 = **42**

### Cálculo do 1º dígito verificador

1. Tome os **12 primeiros caracteres** (Raiz + Ordem)
2. Multiplique cada caractere pelo seu peso, da esquerda para a direita:

   | Posição  | 1   | 2   | 3   | 4   | 5   | 6   | 7   | 8   | 9   | 10  | 11  | 12  |
   | -------- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
   | **Peso** | 5   | 4   | 3   | 2   | 9   | 8   | 7   | 6   | 5   | 4   | 3   | 2   |

3. Some todos os resultados
4. Calcule o resto da divisão por 11
5. Se o resto for `0` ou `1` → dígito = `0`; caso contrário → dígito = `11 − resto`

### Cálculo do 2º dígito verificador

1. Utilize os **12 primeiros caracteres + o 1º dígito verificador** (total de 13 caracteres)
2. Multiplique cada caractere pelo seu peso:

   | Posição  | 1   | 2   | 3   | 4   | 5   | 6   | 7   | 8   | 9   | 10  | 11  | 12  | 13  |
   | -------- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
   | **Peso** | 6   | 5   | 4   | 3   | 2   | 9   | 8   | 7   | 6   | 5   | 4   | 3   | 2   |

3. Some todos os resultados
4. Calcule o resto da divisão por 11
5. Mesma regra: se o resto for `0` ou `1` → dígito = `0`; caso contrário → dígito = `11 − resto`

---

## Caso queira testar no seu projeto

### Verificar se um CNPJ é válido — `IsValidCNPJ`

```csharp
using CNPJValidator;

// CNPJs numéricos legados continuam válidos
CnpjValidator.IsValidCNPJ("00.000.000/0001-91"); // true

// Novo formato alfanumérico (com máscara)
CnpjValidator.IsValidCNPJ("12.ABC.345/01AB-77"); // true

// Aceita letras minúsculas e caracteres extras na entrada — normaliza automaticamente
CnpjValidator.IsValidCNPJ("12.abc.345/01ab-77"); // true

// Retorna false para CNPJs inválidos
CnpjValidator.IsValidCNPJ("12.ABC.345/01AB-35"); // false (dígitos verificadores errados)
CnpjValidator.IsValidCNPJ("00000000000000"); // false (sequência uniforme)
CnpjValidator.IsValidCNPJ(null); // false (não é string)
```

### Criar um CNPJ tipado — `Create`

```csharp
using CNPJValidator;

// Retorna um Result<Cnpj> com sucesso se válido
var result = Cnpj.Create("12.abc.345/01ab-77");

if (result.IsSuccess)
{
    Console.WriteLine(result.Value.Value); // Valor do CNPJ
}

// Retorna um Result com falha se o CNPJ for inválido
var resultInvalido = Cnpj.Create("12.ABC.345/01AB-36");

if (resultInvalido.IsFailure)
{
    Console.WriteLine(resultInvalido.Error.Message); // Mensagem de erro
}

// Também retorna falha para valores nulos ou vazios
var resultVazio = Cnpj.Create("");
if (resultVazio.IsFailure)
{
    Console.WriteLine(resultVazio.Error.Message); // Erro: CNPJ vazio
}
```

---

## Rodando os testes

O projeto utiliza xUnit para testes unitários. Para executar os testes:

```bash
dotnet test
```

Ou execute os testes diretamente no Visual Studio através do Test Explorer.

Os testes cobrem:

- Validação de CNPJs alfanuméricos com máscara e variações de caixa
- Rejeição de dígitos verificadores mutados
- Rejeição de sequências uniformes e comprimentos inválidos
- Rejeição de valores nulos e vazios
- Validação usando Result Pattern no método `Create`

---

## Autor

Adaptado para C# por **Marcio Guilherme** ([@mguilher](https://github.com/mguilher)).

## Projeto Original

Esta implementação é baseada no excelente trabalho em TypeScript desenvolvido por **Gabriel Froes** e **Vanessa Weber** do [Código Fonte TV](https://www.youtube.com/@codigofontetv).

- 📦 Repositório original em TypeScript: [gabrielfroes/novo-cnpj](https://github.com/gabrielfroes/novo-cnpj/blob/main/README.md)
- 🎥 Vídeo explicativo sobre o novo CNPJ Alfanumérico:

[![Código Fonte TV — Novo CNPJ Alfanumérico](https://img.youtube.com/vi/PrlLdgwxpdo/hqdefault.jpg)](https://www.youtube.com/watch?v=PrlLdgwxpdo)

Esta versão adaptada para o ecossistema .NET linguagem C#.

---

## Referências oficiais

- [CNPJ Alfanumérico — Receita Federal](https://www.gov.br/receitafederal/pt-br/acesso-a-informacao/acoes-e-programas/programas-e-atividades/cnpj-alfanumerico)
- [Instrução Normativa RFB nº 2.229/2024](https://www.gov.br/receitafederal/pt-br/acesso-a-informacao/legislacao)
