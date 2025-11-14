using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventHubs;
using System.Text;
using System.Text.Json;


namespace ABCRetailers.Functions.Functions;

public class QueueProcessorFunctions
{
    [Function("OrderNotifications_Processor")]
    public void OrderNotificationsProcessor(
        [QueueTrigger("%QUEUE_ORDER_NOTIFICATIONS%", Connection = "STORAGE_CONNECTION")] string message,
        FunctionContext ctx)
    {
        var log = ctx.GetLogger("OrderNotifications_Processor");
        log.LogInformation($"Processing order notification: {message}");
        
        // Parse the message
        try
        {
            var orderData = JsonSerializer.Deserialize<JsonElement>(message);
            if (orderData.TryGetProperty("OrderId", out var orderId))
            {
                log.LogInformation($"Order {orderId} notification processed successfully");
                // Here you could:
                // - Send email notifications to customers
                // - Update order tracking systems
                // - Trigger shipping workflows
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error processing order notification: {ex.Message}");
        }
    }

    [Function("StockUpdates_Processor")]
    public void StockUpdatesProcessor(
        [QueueTrigger("%QUEUE_STOCK_UPDATES%", Connection = "STORAGE_CONNECTION")] string message,
        FunctionContext ctx)
    {
        var log = ctx.GetLogger("StockUpdates_Processor");
        log.LogInformation($"Processing stock update: {message}");
        
        // Parse the message
        try
        {
            var stockData = JsonSerializer.Deserialize<JsonElement>(message);
            if (stockData.TryGetProperty("ProductId", out var productId) && 
                stockData.TryGetProperty("QuantityOrdered", out var quantity))
            {
                log.LogInformation($"Stock update for Product {productId}: -{quantity} units");
                // Here you could:
                // - Update inventory management systems
                // - Trigger reorder alerts if stock is low
                // - Sync to reporting databases
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error processing stock update: {ex.Message}");
        }
    }
}





