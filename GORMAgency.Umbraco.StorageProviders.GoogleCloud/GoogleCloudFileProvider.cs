using Google;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Net;
using GORMAgency.Umbraco.StorageProviders.GoogleCloud.IO;

namespace GORMAgency.Umbraco.StorageProviders.GoogleCloud
{
    public sealed class GoogleCloudFileProvider : IFileProvider
    {
        private readonly string? _bucketName;
        private readonly StorageClient _storageClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleCloudFileProvider" /> class.
        /// </summary>
        /// <param name="storageClient">The storage client.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="storageClient" /> is <c>null</c>.</exception>
        public GoogleCloudFileProvider(StorageClient storageClient, string? bucketName = null)
        {
            _storageClient = storageClient ?? throw new ArgumentNullException(nameof(storageClient));
            _bucketName = bucketName ?? throw new ArgumentNullException(nameof(bucketName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleCloudFileProvider" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="options" /> is <c>null</c>.</exception>
        public GoogleCloudFileProvider(GoogleCloudFileSystemOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            GoogleCredential googleCredential = GoogleCredential.FromFile(options.ConnectionString);
            _storageClient = StorageClient.Create(googleCredential);
        }

        /// <inheritdoc />
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IFileInfo GetFileInfo(string subpath)
        {
            var path = subpath.TrimStart('/');
            Stream output = new MemoryStream();
            Google.Apis.Storage.v1.Data.Object storageItem;

            try
            {
                storageItem = _storageClient.GetObject(_bucketName, path);
                _storageClient.DownloadObject(storageItem, output);

                // We should always reset the position to 0, so the reading starts from the begining
                output.Position = 0;
            }
            catch (GoogleApiException ex) when (((int)ex.HttpStatusCode) == (int)HttpStatusCode.NotFound)
            {
                return new NotFoundFileInfo(GoogleCloudItemInfo.ParseName(path));
            }

            return new GoogleCloudItemInfo(storageItem, output);
        }

        /// <inheritdoc />
        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }
}
