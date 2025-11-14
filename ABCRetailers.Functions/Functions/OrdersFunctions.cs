using Azure.Data.Tables;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using ABCRetailers.Functions.Entities;
using ABCRetailers.Functions.Helpers;
using ABCRetailers.Functions.Models;
using System.Text.Json;


namespace ABCRetailers.Functions.Functions;

public class OrdersFunctions
{
    private readonly string _conn;
    private readonly string _table;
    private readonly string _queueOrderNotifications;
    private readonly string _queueStockUpdates;

    public OrdersFunctions(IConfiguration cfg)
    {
        _conn = cfg["STORAGE_CONNECTION"] ?? throw new InvalidOperationException("STORAGE_CONNECTION missing");
        _table = cfg["TABLE_ORDER"] ?? "Orders";
        _queueOrderNotifications = cfg["QUEUE_ORDER_NOTIFICATIONS"] ?? "order-notifications";
        _queueStockUpdates = cfg["QUEUE_STOCK_UPDATES"] ?? "stock-updates";
    }

    [Function("Orders_List")]
    public async Task<HttpResponseData> List(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders")] HttpRequestData req)
    {
        var table = new TableClient(_conn, _table);
        await table.CreateIfNotExistsAsync();

        var items = new List<OrderDto>();
        await foreach (var e in table.QueryAsync<OrderEntity>(x => x.PartitionKey == "Order"))
            items.Add(Map.ToDto(e));

        return HttpJson.Ok(req, items);
    }

    [Function("Orders_Get")]
    public async Task<HttpResponseData> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders/{id}")] HttpRequestData req, string id)
    {
        var table = new TableClient(_conn, _table);
        try
        {
            var e = await table.GetEntityAsync<OrderEntity>("Order", id);
            return HttpJson.Ok(req, Map.ToDto(e.Value));
        }
        catch
        {
            return HttpJson.NotFound(req, "Order not found");
        }
    }

    public record OrderCreate(string? CustomerId, string? ProductId, string? ProductName, int? Quantity, decimal? UnitPrice);

    [Function("Orders_Create")]
    public async Task<HttpResponseData> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders")] HttpRequestData req)
    {
        var input = await HttpJson.ReadAsync<OrderCreate>(req);
        if (input is null || string.IsNullOrWhiteSpace(input.CustomerId) || string.IsNullOrWhiteSpace(input.ProductId))
            return HttpJson.Bad(req, "CustomerId and ProductId are required");

        var table = new TableClient(_conn, _table);
        await table.CreateIfNotExistsAsync();

        var e = new OrderEntity
        {
            CustomerId = input.CustomerId!,
            ProductId = input.ProductId!,
            ProductName = input.ProductName ?? "",
            Quantity = input.Quantity ?? 1,
            UnitPrice = (double)(input.UnitPrice ?? 0m),
            OrderDateUtc = DateTimeOffset.UtcNow,
            Status = "Submitted"
        };
        await table.AddEntityAsync(e);

        // Send order notification to queue
        var queueClient = new QueueClient(_conn, _queueOrderNotifications);
        await queueClient.CreateIfNotExistsAsync();
        var message = JsonSerializer.Serialize(new
        {
            OrderId = e.RowKey,
            CustomerId = e.CustomerId,
            ProductId = e.ProductId,
            Quantity = e.Quantity,
            Timestamp = e.OrderDateUtc
        });
        await queueClient.SendMessageAsync(message);

        // Send stock update notification to queue
        var stockQueueClient = new QueueClient(_conn, _queueStockUpdates);
        await stockQueueClient.CreateIfNotExistsAsync();
        var stockMessage = JsonSerializer.Serialize(new
        {
            ProductId = e.ProductId,
            QuantityOrdered = e.Quantity,
            Timestamp = e.OrderDateUtc
        });
        await stockQueueClient.SendMessageAsync(stockMessage);

        return HttpJson.Created(req, Map.ToDto(e));
    }

    [Function("Orders_Delete")]
    public async Task<HttpResponseData> Delete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "orders/{id}")] HttpRequestData req, string id)
    {
        var table = new TableClient(_conn, _table);
        await table.DeleteEntityAsync("Order", id);
        return HttpJson.NoContent(req);
    }
}





