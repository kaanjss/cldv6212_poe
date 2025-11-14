using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using ABCRetailers.Functions.Helpers;


namespace ABCRetailers.Functions.Functions;

public class BlobFunctions
{
    private readonly string _conn;
    private readonly string _productImages;

    public BlobFunctions(IConfiguration cfg)
    {
        _conn = cfg["STORAGE_CONNECTION"] ?? throw new InvalidOperationException("STORAGE_CONNECTION missing");
        _productImages = cfg["BLOB_PRODUCT_IMAGES"] ?? "product-images";
    }

    // Blob Trigger - Logs when a product image is uploaded
    [Function("OnProductImageUploaded")]
    public void OnProductImageUploaded(
        [BlobTrigger("%BLOB_PRODUCT_IMAGES%/{name}", Connection = "STORAGE_CONNECTION")] Stream blob,
        string name,
        FunctionContext ctx)
    {
        var log = ctx.GetLogger("OnProductImageUploaded");
        log.LogInformation($"Product image uploaded: {name}, size={blob.Length} bytes");
    }

    // HTTP endpoint to upload product images
    [Function("Uploads_ProductImage")]
    public async Task<HttpResponseData> UploadProductImage(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uploads/product-image")] HttpRequestData req)
    {
        var contentType = req.Headers.TryGetValues("Content-Type", out var ct) ? ct.First() : "";
        if (!contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            return HttpJson.Bad(req, "Expected multipart/form-data");

        var form = await MultipartHelper.ParseAsync(req.Body, contentType);
        var file = form.Files.FirstOrDefault(f => f.FieldName == "ProductImage");
        if (file is null || file.Data.Length == 0) return HttpJson.Bad(req, "ProductImage file is required");

        var container = new BlobContainerClient(_conn, _productImages);
        await container.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.None);

        var blobName = $"{Guid.NewGuid():N}-{file.FileName}";
        var blob = container.GetBlobClient(blobName);
        
        await using (var s = file.Data) 
            await blob.UploadAsync(s);

        return HttpJson.Ok(req, new { fileName = blobName, blobUrl = blob.Uri.ToString() });
    }

    // HTTP endpoint to list all product images
    [Function("Uploads_ListProductImages")]
    public async Task<HttpResponseData> ListProductImages(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "uploads/product-images")] HttpRequestData req)
    {
        var container = new BlobContainerClient(_conn, _productImages);
        await container.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.None);

        var images = new List<object>();
        await foreach (var blobItem in container.GetBlobsAsync())
        {
            var blobClient = container.GetBlobClient(blobItem.Name);
            images.Add(new
            {
                name = blobItem.Name,
                url = blobClient.Uri.ToString(),
                size = blobItem.Properties.ContentLength,
                lastModified = blobItem.Properties.LastModified
            });
        }

        return HttpJson.Ok(req, images);
    }
}





