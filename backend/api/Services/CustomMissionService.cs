﻿

using System.Text.Json;
using Api.Database.Models;
using Api.Options;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace Api.Services
{

    public interface ICustomMissionService
    {
        Task<Uri> UploadSource(List<MissionTask> tasks);
        List<MissionTask>? GetMissionTasksFromMissionId(string id);
        List<MissionTask>? GetMissionTasksFromURL(string url);
    }

    public class CustomMissionService : ICustomMissionService
    {
        private readonly IOptions<StorageOptions> _storageOptions;

        public CustomMissionService(IOptions<StorageOptions> storageOptions)
        {
            _storageOptions = storageOptions;
        }

        public async Task<bool> CreateContainer(string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_storageOptions.Value.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            return true;
        }

        public async Task<Uri> UploadFile(string fileName, Stream fileStream)
        {
            throw new NotImplementedException();
        }

        public Task<Uri> UploadSource(List<MissionTask> tasks)
        {
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(tasks)));

            string id = Guid.NewGuid().ToString();
            var taskUri = UploadFile(id, memoryStream);

            return taskUri;
        }

        public List<MissionTask>? GetMissionTasksFromMissionId(string id)
        {
            var blobServiceClient = new BlobServiceClient(_storageOptions.Value.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_storageOptions.Value.CustomMissionContainerName);
            containerClient.CreateIfNotExists();

            var blobClient = containerClient.GetBlobClient(id);

            List<MissionTask>? content;
            try
            {
                content = blobClient.DownloadContent().Value.Content.ToObjectFromJson<List<MissionTask>>();
            }
            catch (Exception)
            {
                return null;
            }

            return content;
        }

        public List<MissionTask>? GetMissionTasksFromURL(string url)
        {
            var blobClient = new BlobClient(new Uri(url));

            List<MissionTask>? content;
            try
            {
                content = blobClient.DownloadContent().Value.Content.ToObjectFromJson<List<MissionTask>>();
            }
            catch (Exception)
            {
                return null;
            }

            return content;
        }
    }
}