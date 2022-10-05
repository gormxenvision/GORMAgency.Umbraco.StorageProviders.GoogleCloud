using Microsoft.Extensions.FileProviders;

namespace GORMAgency.Umbraco.StorageProviders.GoogleCloud
{
    /// <summary>
    /// Represents an Google Cloud Storage item.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.FileProviders.IFileInfo" />
    public sealed class GoogleCloudItemInfo : IFileInfo
    {
        private readonly Stream _output;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleCloudItemInfo" /> class.
        /// </summary>
        /// <param name="storageItem">The storage item.</param>
        /// <param name="output">The stream output.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="storageItem" /> is <c>null</c>.</exception>
        public GoogleCloudItemInfo(Google.Apis.Storage.v1.Data.Object storageItem, Stream output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));

            Name = ParseName("media/" + storageItem?.Name);
            LastModified = storageItem?.Updated != null ? storageItem.Updated.Value : new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero);
            Length = Convert.ToInt64(storageItem?.Size != null ? storageItem.Size.Value : 0);
        }

        /// <inheritdoc />
        public bool Exists => true;

        /// <inheritdoc />
        public bool IsDirectory => false;

        /// <inheritdoc />
        public DateTimeOffset LastModified { get; }

        /// <inheritdoc />
        public long Length { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string PhysicalPath => null!;

        /// <inheritdoc />
        public Stream CreateReadStream() => _output;

        /// <summary>
        /// Parses the name from the file path.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>
        /// The name.
        /// </returns>
        internal static string ParseName(string path) => path[(path.LastIndexOf('/') + 1)..];
    }
}
