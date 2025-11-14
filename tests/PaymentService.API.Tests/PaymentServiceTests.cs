using Microsoft.Extensions.Logging;
using Moq;
using PaymentService.API.Application.Interfaces;
using PaymentService.API.Domain.Entities;
using PaymentService.API.Infrastructure.Messaging;
using Shared.Contracts.Events;
using Xunit;

namespace PaymentService.API.Tests.Application.Services
{
    public class PaymentServiceTests
    {
        [Fact]
        public async Task ProcessPaymentAsync_SavesPayment_And_PublishesEvent()
        {
            // Arrange
            var repoMock = new Mock<IPaymentRepository>();
            var publisherMock = new Mock<IRabbitMqPublisher>();
            var loggerMock = new Mock<ILogger<PaymentService.API.Application.Services.PaymentService>>();

            repoMock.Setup(r => r.AddAsync(It.IsAny<PaymentRecord>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            publisherMock.Setup(p => p.PublishPaymentSucceededAsync(It.IsAny<PaymentSucceededEvent>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            var svc = new PaymentService.API.Application.Services.PaymentService(repoMock.Object, publisherMock.Object, loggerMock.Object);

            // Act
            var record = await svc.ProcessPaymentAsync("order-1", 25.5m, CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.AddAsync(It.Is<PaymentRecord>(p => p.OrderId == "order-1" && p.Amount == 25.5m), It.IsAny<CancellationToken>()), Times.Once);

            publisherMock.Verify(p => p.PublishPaymentSucceededAsync(It.Is<PaymentSucceededEvent>(e => e.OrderId == "order-1" && e.Amount == 25.5m), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentAsync_NonPositiveAmount_ThrowsArgumentException()
        {
            var repoMock = new Mock<IPaymentRepository>();
            var publisherMock = new Mock<IRabbitMqPublisher>();
            var loggerMock = new Mock<ILogger<PaymentService.API.Application.Services.PaymentService>>();

            var svc = new PaymentService.API.Application.Services.PaymentService(repoMock.Object, publisherMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<System.ArgumentException>(() => svc.ProcessPaymentAsync("order-1", 0m, CancellationToken.None));
        }
    }
}
