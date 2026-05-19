# Care Guide (Backend)

## Objetivo do Projeto

Esta solução tem como objetivo centralizar todas as informações relevantes ao tratamento de pacientes oncológicos em um único local. Entre essas informações estão os dados de médicos e especialistas, a lista de medicamentos em uso, exames realizados ou agendados, cirurgias e procedimentos médicos, além dos endereços e coordenadas de hospitais e clínicas frequentados. Também é possível acompanhar a evolução dos sintomas e detalhes dos tratamentos recebidos, proporcionando ao paciente uma visão completa e organizada de sua jornada de cuidados.

## Principais Funcionalidades

- **Gerenciar Usuários**: Cadastro de informações e login;
- **Gerenciar Pessoas**: Cadastro de todas as informações referentes a pessoa/usuário;
- **Gerenciar Médicos**: Cadastro de médicos;
- **Gerenciar Anotações**: Cadastro de anotações importantes;
- **Agenda/Calendário**: Cadastro de eventos em uma agenda/calendário (TO DO);

## Tecnologias Utilizadas

- .NET 10;
- Entity Framework;
- PostgreSQL;
- Docker;

## Diagrama Entidade-Relacionamento do Banco de Dados

- [Diagrama ER](https://miro.com/app/board/uXjVNh5qees=/)

## Estrutura

Para mais informações referente a estrutura da Solução, acesse a seguinte [documentação](docs/STRUCTURE.md).

## Execução

Para um passo a passo de como executar a aplicação, acesse a seguinte [documentação](docs/EXECUTION.md).

## Migrations

Para saber como criar e executar migrations no projeto, consulte a seguinte [documentação](docs/MIGRATIONS.md).

## Testes

Para informações sobre a estrutura de testes unitários e como executá-los, acesse a seguinte [documentação](docs/TESTS.md).

## Documentação da API (OpenAPI + Scalar)

A API é documentada utilizando o padrão **OpenAPI** e a interface interativa do **Scalar**.

Ao rodar a aplicação, a documentação estará disponível em:

- `http://localhost:<porta>/scalar`

Essa interface permite visualizar todos os endpoints, schemas de requisição/resposta, exemplos e realizar testes diretamente pelo navegador.

## Participantes do Projeto

- [Amanda Maschio](https://www.linkedin.com/in/amanda-maschio/)
- [Fernando Waldow Martens](https://www.linkedin.com/in/fernandomartens/)
