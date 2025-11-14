using Microsoft.Extensions.Logging;
using Moq;
using OrderService.API.Application.Dtos;
using OrderService.API.Application.Interfaces;
using Shared.Contracts.Events;
using OrderService.API.Domain.Entities;
using OrderService.API.Infrastructure.Messaging;

namespace OrderService.API.Tests
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task CreateOrderAsync_PersistsOrder_And_PublishesEvent()
        {
            // Arrange
            var repoMock = new Mock<IOrderRepository>();
            var publisherMock = new Mock<IRabbitMqPublisher>();
            var loggerMock = new Mock<ILogger<OrderService.API.Application.Services.OrderService>>();

            // make repo AddAsync just complete
            repoMock.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            publisherMock.Setup(p => p.PublishOrderCreatedAsync(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            var svc = new OrderService.API.Application.Services.OrderService(repoMock.Object, publisherMock.Object, loggerMock.Object);

            var dto = new CreateOrderDto(49.99m, "user@example.com");

            // Act
            var created = await svc.CreateOrderAsync(dto, CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.AddAsync(It.Is<Order>(o => o.OrderId == created.OrderId && o.Amount == dto.Amount), It.IsAny<CancellationToken>()), Times.Once);

            publisherMock.Verify(p => p.PublishOrderCreatedAsync(It.Is<OrderCreatedEvent>(e => e.OrderId == created.OrderId && e.Amount == dto.Amount && e.CustomerEmail == dto.CustomerEmail), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_InvalidPayload_ThrowsArgumentException()
        {
            var repoMock = new Mock<IOrderRepository>();
            var publisherMock = new Mock<IRabbitMqPublisher>();
            var loggerMock = new Mock<ILogger<OrderService.API.Application.Services.OrderService>>();

            var svc = new OrderService.API.Application.Services.OrderService(repoMock.Object, publisherMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<System.ArgumentException>(() => svc.CreateOrderAsync(new CreateOrderDto(0m, ""), CancellationToken.None));
        }
    }
}
