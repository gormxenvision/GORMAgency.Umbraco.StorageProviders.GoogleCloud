using Google;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;

namespace GORMAgency.Umbraco.StorageProviders.GoogleCloud.IO
{
    public sealed class GoogleCloudFileSystem : IGoogleCloudFileSystem, IFileProviderFactory
    {
        private readonly string _bucketName;
        private readonly string _rootUrl;
        private readonly StorageClient _storageClient;
        private readonly IIOHelper _ioHelper;
        private readonly IContentTypeProvider _contentTypeProvider;

        public GoogleCloudFileSystem(GoogleCloudFileSystemOptions options, IHostingEnvironment hostingEnvironment, IIOHelper ioHelper, IContentTypeProvider contentTypeProvider)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(hostingEnvironment);

            _bucketName = options.BucketName;
            _rootUrl = EnsureUrlSeparatorChar(hostingEnvironment.ToAbsolute(options.VirtualPath)).TrimEnd('/');
            GoogleCredential _googleCredential = GoogleCredential.FromFile(options.ConnectionString);
            _storageClient = StorageClient.Create(_googleCredential);
            _ioHelper = ioHelper ?? throw new ArgumentNullException(nameof(ioHelper));
            _contentTypeProvider = contentTypeProvider ?? throw new ArgumentNullException(nameof(contentTypeProvider));
        }

        public GoogleCloudFileSystem(string rootUrl, string bucketName, StorageClient storageClient, IIOHelper ioHelper, IContentTypeProvider contentTypeProvider)
        {
            ArgumentNullException.ThrowIfNull(rootUrl);

            _bucketName = bucketName ?? throw new ArgumentNullException(nameof(bucketName));
            _rootUrl = EnsureUrlSeparatorChar(rootUrl).TrimEnd('/');
            _storageClient = storageClient ?? throw new ArgumentNullException(nameof(storageClient));
            _ioHelper = ioHelper ?? throw new ArgumentNullException(nameof(ioHelper));
            _contentTypeProvider = contentTypeProvider ?? throw new ArgumentNullException(nameof(contentTypeProvider));
        }

        /// <inheritdoc />
        public bool CanAddPhysical => false;

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public IEnumerable<string> GetDirectories(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public void DeleteDirectory(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            DeleteDirectory(path, true);
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public void DeleteDirectory(string path, bool recursive)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public bool DirectoryExists(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream" /> is <c>null</c>.</exception>
        public void AddFile(string path, Stream stream)
        {
            ArgumentNullException.ThrowIfNull(path);
            ArgumentNullException.ThrowIfNull(stream);

            AddFile(path, stream, true);
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="stream" /> is <c>null</c>.</exception>
        public void AddFile(string path, Stream stream, bool overrideIfExists)
        {
            ArgumentNullException.ThrowIfNull(path);
            ArgumentNullException.ThrowIfNull(stream);

            if (!overrideIfExists && FileExists(path))
            {
                throw new InvalidOperationException($"A file at path '{path}' already exists");
            }

            _contentTypeProvider.TryGetContentType(path, out var contentType);

            _storageClient.UploadObject(_bucketName, path, contentType, stream);
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="physicalPath" /> is <c>null</c>.</exception>
        public void AddFile(string path, string physicalPath, bool overrideIfExists = true, bool copy = false)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public IEnumerable<string> GetFiles(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            return GetFiles(path, null);
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public IEnumerable<string> GetFiles(string path, string? filter)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public Stream OpenFile(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            Stream output = new MemoryStream();

            _storageClient.DownloadObject(_bucketName, path, output);

            // Always set position to 0, so the read starts from the begining
            output.Position = 0;

            return output;
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public void DeleteFile(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            _storageClient.DeleteObject(_bucketName, path);
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public bool FileExists(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            try {
                return _storageClient.GetObject(_bucketName, path) != null ? true : false;
            } catch (GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound) {
                return false;
            }
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="fullPathOrUrl" /> is <c>null</c>.</exception>
        public string GetRelativePath(string fullPathOrUrl)
        {
            ArgumentNullException.ThrowIfNull(fullPathOrUrl);

            var path = EnsureUrlSeparatorChar(fullPathOrUrl); // ensure url separator char

            // if it starts with the root url, strip it and trim the starting slash to make it relative
            // eg "/Media/1234/img.jpg" => "1234/img.jpg"
            if (_ioHelper.PathStartsWith(path, _rootUrl, '/'))
            {
                path = path[_rootUrl.Length..].TrimStart('/');
            }

            return path;
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public string GetFullPath(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            path = EnsureUrlSeparatorChar(path);

            return (_ioHelper.PathStartsWith(path, _rootUrl, '/') ? path : $"{_rootUrl}/{path}").Trim('/');
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public string GetUrl(string? path)
        {
            ArgumentNullException.ThrowIfNull(path);

            return $"{_rootUrl}/{EnsureUrlSeparatorChar(path).Trim('/')}";
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public DateTimeOffset GetLastModified(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            var _obj = _storageClient.GetObject(_bucketName, path);

            if (_obj == null)
            {
                return new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero);
            }
          
            return _obj.Updated != null ? _obj.Updated.Value : new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero);
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public DateTimeOffset GetCreated(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            var _obj = _storageClient.GetObject(_bucketName, path);

            if (_obj == null)
            {
                return new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero);
            }

            return _obj.TimeCreated != null ? _obj.TimeCreated.Value : new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero);
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        public long GetSize(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            var _obj = _storageClient.GetObject(_bucketName, path);

            if (_obj == null)
            {
                return 0;
            }

            ulong size = _obj.Size != null ? _obj.Size.Value : 0;

            return Convert.ToInt64(size);
        }

        /// <inheritdoc />
        public IFileProvider Create() => new GoogleCloudFileProvider(_storageClient, _bucketName);

        private static string EnsureUrlSeparatorChar(string path)
            => path.Replace("\\", "/", StringComparison.InvariantCultureIgnoreCase);

    }
}
