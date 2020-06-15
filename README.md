# SolidSampleApplication

This Sample Application Features:
1. In-memory EF Core with sqllite version 3.1.4
2. Mediatr library by Jimmy Bogard
3. Usage of CQRS & ES pattern with a separate read model, and a separate reporting read model in an application
4. Command handler validation with fluent validation. This gets hooked it up to be triggered via mediatr pipeline rather than via controller.
5. Swagger definition
6. The use of DDD Aggregate root and domain service and domain event.
7. The application structure follows SOLID / Clean architecture application by Jimmy Bogard - (or Jason Taylor) with custom CQRS/ES implementation.
8. Uses Shouldly with xUnit for unit test for assertion
9. Uses MartinCostello.Logging.XUnit to pipe logging output to xUnit output
10. Uses Pressius for test values permutation
11. Uses MassTransit and RabbitMQ to serves as a bus and publishes the events