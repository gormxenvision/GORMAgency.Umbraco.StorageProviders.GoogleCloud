using System;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;

namespace GORMAgency.Umbraco.StorageProviders.GoogleCloud.IO
{
    public sealed class GoogleCloudFileSystemProvider : IGoogleCloudFileSystemProvider
    {
        private readonly ConcurrentDictionary<string, IGoogleCloudFileSystem> _fileSystems = new();
        private readonly IOptionsMonitor<GoogleCloudFileSystemOptions> _optionsMonitor;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IIOHelper _ioHelper;
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleCloudFileSystemProvider"/> class.
        /// </summary>
        /// <param name="optionsMonitor">The options monitor.</param>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        /// <param name="ioHelper">The IO helper.</param>
        /// <exception cref="ArgumentNullException"><paramref name="optionsMonitor"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="hostingEnvironment"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="ioHelper"/> is <c>null</c>.</exception>
        public GoogleCloudFileSystemProvider(IOptionsMonitor<GoogleCloudFileSystemOptions> optionsMonitor, IHostingEnvironment hostingEnvironment, IIOHelper ioHelper)
        {
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _ioHelper = ioHelper ?? throw new ArgumentNullException(nameof(ioHelper));
            _fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();

            _optionsMonitor.OnChange((options, name) => _fileSystems.TryRemove(name, out _));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
        public IGoogleCloudFileSystem GetFileSystem(string name)
        {
            ArgumentNullException.ThrowIfNull(name);

            return _fileSystems.GetOrAdd(name, name =>
            {
                var options = _optionsMonitor?.Get(name);

                return new GoogleCloudFileSystem(options, _hostingEnvironment, _ioHelper, _fileExtensionContentTypeProvider);
            });
        }
    }
}
