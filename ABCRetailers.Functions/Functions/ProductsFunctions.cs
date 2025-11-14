using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using ABCRetailers.Functions.Entities;
using ABCRetailers.Functions.Helpers;
using ABCRetailers.Functions.Models;


namespace ABCRetailers.Functions.Functions;

public class ProductsFunctions
{
    private readonly string _conn;
    private readonly string _table;

    public ProductsFunctions(IConfiguration cfg)
    {
        _conn = cfg["STORAGE_CONNECTION"] ?? throw new InvalidOperationException("STORAGE_CONNECTION missing");
        _table = cfg["TABLE_PRODUCT"] ?? "Products";
    }

    [Function("Products_List")]
    public async Task<HttpResponseData> List(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequestData req)
    {
        var table = new TableClient(_conn, _table);
        await table.CreateIfNotExistsAsync();

        var items = new List<ProductDto>();
        await foreach (var e in table.QueryAsync<ProductEntity>(x => x.PartitionKey == "Product"))
            items.Add(Map.ToDto(e));

        return HttpJson.Ok(req, items);
    }

    [Function("Products_Get")]
    public async Task<HttpResponseData> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id}")] HttpRequestData req, string id)
    {
        var table = new TableClient(_conn, _table);
        try
        {
            var e = await table.GetEntityAsync<ProductEntity>("Product", id);
            return HttpJson.Ok(req, Map.ToDto(e.Value));
        }
        catch
        {
            return HttpJson.NotFound(req, "Product not found");
        }
    }

    public record ProductCreateUpdate(string? ProductName, string? Description, decimal? Price, int? StockAvailable, string? ImageUrl);

    [Function("Products_Create")]
    public async Task<HttpResponseData> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequestData req)
    {
        var input = await HttpJson.ReadAsync<ProductCreateUpdate>(req);
        if (input is null || string.IsNullOrWhiteSpace(input.ProductName) || input.Price == null)
            return HttpJson.Bad(req, "ProductName and Price are required");

        var table = new TableClient(_conn, _table);
        await table.CreateIfNotExistsAsync();

        var e = new ProductEntity
        {
            ProductName = input.ProductName!,
            Description = input.Description ?? "",
            Price = (double)input.Price.Value,
            StockAvailable = input.StockAvailable ?? 0,
            ImageUrl = input.ImageUrl ?? ""
        };
        await table.AddEntityAsync(e);

        return HttpJson.Created(req, Map.ToDto(e));
    }

    [Function("Products_Update")]
    public async Task<HttpResponseData> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "products/{id}")] HttpRequestData req, string id)
    {
        var input = await HttpJson.ReadAsync<ProductCreateUpdate>(req);
        if (input is null) return HttpJson.Bad(req, "Invalid body");

        var table = new TableClient(_conn, _table);
        try
        {
            var resp = await table.GetEntityAsync<ProductEntity>("Product", id);
            var e = resp.Value;

            e.ProductName = input.ProductName ?? e.ProductName;
            e.Description = input.Description ?? e.Description;
            e.Price = input.Price.HasValue ? (double)input.Price.Value : e.Price;
            e.StockAvailable = input.StockAvailable ?? e.StockAvailable;
            e.ImageUrl = input.ImageUrl ?? e.ImageUrl;

            await table.UpdateEntityAsync(e, e.ETag, TableUpdateMode.Replace);
            return HttpJson.Ok(req, Map.ToDto(e));
        }
        catch
        {
            return HttpJson.NotFound(req, "Product not found");
        }
    }

    [Function("Products_Delete")]
    public async Task<HttpResponseData> Delete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "products/{id}")] HttpRequestData req, string id)
    {
        var table = new TableClient(_conn, _table);
        await table.DeleteEntityAsync("Product", id);
        return HttpJson.NoContent(req);
    }
}





