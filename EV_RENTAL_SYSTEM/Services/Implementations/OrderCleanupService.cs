using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    /// <summary>
    /// Background service để tự động hủy đơn hàng chưa thanh toán sau thời gian quy định
    /// </summary>
    public class OrderCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderCleanupService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // Check mỗi 5 phút
        private readonly TimeSpan _autoCancelAfter = TimeSpan.FromMinutes(15); // Tự động hủy sau 15 phút

        public OrderCleanupService(IServiceProvider serviceProvider, ILogger<OrderCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderCleanupService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingOrders();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing pending orders");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("OrderCleanupService stopped");
        }

        private async Task ProcessPendingOrders()
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            try
            {
                // Lấy tất cả đơn hàng có status "Pending" hoặc "Pending Payment"
                var pendingOrders = await unitOfWork.Orders.GetPendingOrdersAsync();

                var cutoffTime = DateTime.Now.Subtract(_autoCancelAfter);
                var ordersToCancel = pendingOrders
                    .Where(o => o.OrderDate < cutoffTime)
                    .ToList();

                if (ordersToCancel.Any())
                {
                    _logger.LogInformation("Found {Count} orders to cancel due to timeout", ordersToCancel.Count);

                    foreach (var order in ordersToCancel)
                    {
                        await CancelOrderDueToTimeout(order, unitOfWork);
                    }

                    await unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pending orders");
            }
        }

        private async Task CancelOrderDueToTimeout(Models.Order order, IUnitOfWork unitOfWork)
        {
            try
            {
                _logger.LogInformation("Cancelling order {OrderId} due to timeout (created at {OrderDate})", 
                    order.OrderId, order.OrderDate);

                // Cập nhật status đơn hàng
                order.Status = "Cancelled";

                // Giải phóng biển số xe nếu có
                var orderLicensePlates = await unitOfWork.OrderLicensePlates.GetByOrderIdAsync(order.OrderId);
                foreach (var orderLicensePlate in orderLicensePlates)
                {
                    var licensePlate = await unitOfWork.LicensePlates.GetByIdAsync(orderLicensePlate.LicensePlateId);
                    if (licensePlate != null)
                    {
                        licensePlate.Status = "Available";
                        unitOfWork.LicensePlates.Update(licensePlate);
                        
                        _logger.LogInformation("Released license plate {LicensePlateId} back to Available status", 
                            licensePlate.LicensePlateId);
                    }
                }

                // Cập nhật contract status nếu có
                var contracts = await unitOfWork.Contracts.GetContractsByOrderIdAsync(order.OrderId);
                foreach (var contract in contracts)
                {
                    contract.Status = "Cancelled";
                    unitOfWork.Contracts.Update(contract);
                }

                unitOfWork.Orders.Update(order);

                _logger.LogInformation("Successfully cancelled order {OrderId} due to timeout", order.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId} due to timeout", order.OrderId);
            }
        }
    }
}

