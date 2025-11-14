using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.API.Application.Interfaces;
using NotificationService.API.Domain.Entities;
using NotificationService.API.Infrastructure.Senders;
using Xunit;

namespace NotificationService.API.Tests.Application.Services
{
    public class NotificationServiceTests
    {
        [Fact]
        public async Task CreateAndSendAsync_SavesRecord_And_CallsSender()
        {
            // Arrange
            var repoMock = new Mock<INotificationRepository>();
            var senderMock = new Mock<INotificationSender>();
            var loggerMock = new Mock<ILogger<NotificationService.API.Application.Services.NotificationService>>();

            repoMock.Setup(r => r.AddAsync(It.IsAny<NotificationRecord>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            senderMock.Setup(s => s.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask);

            var svc = new NotificationService.API.Application.Services.NotificationService(repoMock.Object, senderMock.Object, loggerMock.Object);

            // Act
            var record = await svc.CreateAndSendAsync("order-1", "Payment succeeded", "user@example.com", CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.AddAsync(It.Is<NotificationRecord>(n => n.OrderId == "order-1"), It.IsAny<CancellationToken>()), Times.Once);
            senderMock.Verify(s => s.SendAsync("user@example.com", It.IsAny<string>(), "Payment succeeded", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAndSendAsync_InvalidArgs_ThrowsArgumentException()
        {
            var repoMock = new Mock<INotificationRepository>();
            var senderMock = new Mock<INotificationSender>();
            var loggerMock = new Mock<ILogger<NotificationService.API.Application.Services.NotificationService>>();

            var svc = new NotificationService.API.Application.Services.NotificationService(repoMock.Object, senderMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<System.ArgumentException>(() => svc.CreateAndSendAsync("", "msg", "to", CancellationToken.None));
            await Assert.ThrowsAsync<System.ArgumentException>(() => svc.CreateAndSendAsync("order-1", "msg", "", CancellationToken.None));
        }
    }
}
