using ABCRetailers.Functions.Helpers;
using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;


namespace ABCRetailers.Functions.Functions;

public class UploadsFunctions
{
    private readonly string _conn;
    private readonly string _proofs;
    private readonly string _share;
    private readonly string _shareDir;

    public UploadsFunctions(IConfiguration cfg)
    {
        _conn = cfg["STORAGE_CONNECTION"] ?? throw new InvalidOperationException("STORAGE_CONNECTION missing");
        _proofs = cfg["BLOB_PAYMENT_PROOFS"] ?? "payment-proofs";
        _share = cfg["FILESHARE_CONTRACTS"] ?? "contracts";
        _shareDir = cfg["FILESHARE_DIR_PAYMENTS"] ?? "payments";
    }

    // Upload proof of payment to Blob Storage and metadata to Azure Files
    [Function("Uploads_ProofOfPayment")]
    public async Task<HttpResponseData> Proof(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uploads/proof-of-payment")] HttpRequestData req)
    {
        var contentType = req.Headers.TryGetValues("Content-Type", out var ct) ? ct.First() : "";
        if (!contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            return HttpJson.Bad(req, "Expected multipart/form-data");

        var form = await MultipartHelper.ParseAsync(req.Body, contentType);
        var file = form.Files.FirstOrDefault(f => f.FieldName == "ProofOfPayment");
        if (file is null || file.Data.Length == 0) return HttpJson.Bad(req, "ProofOfPayment file is required");

        var orderId = form.Text.GetValueOrDefault("OrderId");
        var customerName = form.Text.GetValueOrDefault("CustomerName");

        // Upload to Blob Storage
        var container = new BlobContainerClient(_conn, _proofs);
        await container.CreateIfNotExistsAsync();
        var blobName = $"{Guid.NewGuid():N}-{file.FileName}";
        var blob = container.GetBlobClient(blobName);
        await using (var s = file.Data) await blob.UploadAsync(s);

        // Store metadata in Azure Files
        var share = new ShareClient(_conn, _share);
        await share.CreateIfNotExistsAsync();
        var root = share.GetRootDirectoryClient();
        var dir = root.GetSubdirectoryClient(_shareDir);
        await dir.CreateIfNotExistsAsync();

        var fileClient = dir.GetFileClient(blobName + ".txt");
        var meta = $"UploadedAtUtc: {DateTimeOffset.UtcNow:O}\nOrderId: {orderId}\nCustomerName: {customerName}\nBlobUrl: {blob.Uri}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(meta);
        using var ms = new MemoryStream(bytes);
        await fileClient.CreateAsync(ms.Length);
        await fileClient.UploadAsync(ms);

        return HttpJson.Ok(req, new { fileName = blobName, blobUrl = blob.Uri.ToString() });
    }

    // Upload contract file directly to Azure Files
    [Function("Uploads_Contract")]
    public async Task<HttpResponseData> UploadContract(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uploads/contract")] HttpRequestData req)
    {
        var contentType = req.Headers.TryGetValues("Content-Type", out var ct) ? ct.First() : "";
        if (!contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            return HttpJson.Bad(req, "Expected multipart/form-data");

        var form = await MultipartHelper.ParseAsync(req.Body, contentType);
        var file = form.Files.FirstOrDefault(f => f.FieldName == "ContractFile");
        if (file is null || file.Data.Length == 0) return HttpJson.Bad(req, "ContractFile is required");

        var customerId = form.Text.GetValueOrDefault("CustomerId");
        var contractType = form.Text.GetValueOrDefault("ContractType") ?? "general";

        // Upload to Azure Files
        var share = new ShareClient(_conn, _share);
        await share.CreateIfNotExistsAsync();
        var root = share.GetRootDirectoryClient();

        // Create directory for contract type if it doesn't exist
        var dir = root.GetSubdirectoryClient(contractType);
        await dir.CreateIfNotExistsAsync();

        var fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{customerId}_{file.FileName}";
        var fileClient = dir.GetFileClient(fileName);

        await using (var s = file.Data)
        {
            await fileClient.CreateAsync(s.Length);
            await fileClient.UploadAsync(s);
        }

        return HttpJson.Ok(req, new 
        { 
            fileName = fileName, 
            filePath = $"{_share}/{contractType}/{fileName}",
            customerId = customerId,
            contractType = contractType
        });
    }

    // List files in Azure File Share
    [Function("Uploads_ListContracts")]
    public async Task<HttpResponseData> ListContracts(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "uploads/contracts")] HttpRequestData req)
    {
        var share = new ShareClient(_conn, _share);
        await share.CreateIfNotExistsAsync();
        var root = share.GetRootDirectoryClient();

        var files = new List<object>();
        
        await foreach (var item in root.GetFilesAndDirectoriesAsync())
        {
            if (item.IsDirectory)
            {
                var dir = root.GetSubdirectoryClient(item.Name);
                await foreach (var file in dir.GetFilesAndDirectoriesAsync())
                {
                    if (!file.IsDirectory)
                    {
                        files.Add(new
                        {
                            directory = item.Name,
                            fileName = file.Name,
                            filePath = $"{_share}/{item.Name}/{file.Name}"
                        });
                    }
                }
            }
            else
            {
                files.Add(new
                {
                    directory = "root",
                    fileName = item.Name,
                    filePath = $"{_share}/{item.Name}"
                });
            }
        }

        return HttpJson.Ok(req, files);
    }
}





