# Projeto E-commerce Microsserviços

Este repositório contém uma implementação de uma arquitetura de microsserviços para gerenciar estoque e vendas em um contexto de e-commerce. O projeto é otimizado para desenvolvimento com GitHub Codespaces e Docker.

## Visão Geral da Arquitetura

O sistema é composto pelos seguintes microsserviços:

-   **EstoqueService**: Gerencia o estoque de produtos.
-   **VendasService**: Gerencia os pedidos e vendas.
-   **PrecoService**: Gerencia a atualização de preços dos produtos.
-   **ApiGateway**: Atua como um ponto de entrada único para todas as requisições externas, roteando-as para os microsserviços apropriados e lidando com autenticação.
-   **RabbitMQ**: Utilizado para comunicação assíncrona entre os microsserviços.

## Tecnologias Utilizadas

-   **.NET 8 (ASP.NET Core)**: Para o desenvolvimento dos microsserviços e API Gateway.
-   **Docker**: Para conteinerização de todos os serviços.
-   **Docker Compose**: Para orquestração e fácil inicialização de todos os contêineres.
-   **SQL Server**: Como banco de dados relacional para cada microsserviço.
-   **RabbitMQ**: Para mensageria.
-   **Ocelot**: Para o API Gateway.
-   **JWT**: Para autenticação.

## Configuração do Ambiente de Desenvolvimento (GitHub Codespaces)

Este projeto é configurado para ser executado sem problemas no GitHub Codespaces.

1.  **Abrir no Codespaces**: Clique no botão "Code" no GitHub e selecione "Open with Codespaces".
2.  **Construção Automática**: O Codespaces irá automaticamente construir o ambiente, incluindo a instalação do .NET SDK, Docker e a inicialização de todos os serviços definidos no `docker-compose.yml`.

## Rodando o Projeto Localmente (Docker Compose)


## Acesso aos Serviços


## Microsserviços

### EstoqueService


### VendasService


### PrecoService


## Próximos Passos (Desenvolvimento)

-   Implementar a lógica de negócios e persistência para cada microsserviço.
-   Configurar a comunicação via RabbitMQ.
-   Implementar autenticação JWT no API Gateway e nos microsserviços.
-   Adicionar testes unitários.
-   Implementar logging e monitoramento.
