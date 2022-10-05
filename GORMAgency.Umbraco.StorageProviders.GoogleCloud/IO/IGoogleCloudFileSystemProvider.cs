using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GORMAgency.Umbraco.StorageProviders.GoogleCloud.IO
{
    /// <summary>
    /// The Google Cloud file system provider.
    /// </summary>
    public interface IGoogleCloudFileSystemProvider
    {
        /// <summary>
        /// Get the file system by its <paramref name="name" />.
        /// </summary>
        /// <param name="name">The name of the <see cref="IGoogleCloudFileSystem" />.</param>
        /// <returns>
        /// The <see cref="IGoogleCloudFileSystem" />.
        /// </returns>
        IGoogleCloudFileSystem GetFileSystem(string name);
    }
}
