using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GORMAgency.Umbraco.StorageProviders.GoogleCloud.IO
{
    public sealed class GoogleCloudFileSystemOptions
    {
        /// <summary>
        /// The media filesystem name.
        /// </summary>
        public const string MediaFileSystemName = "Media";

        /// <summary>
        /// Gets or sets the bucket name.
        /// </summary>
        [Required]
        public string BucketName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the storage account connection string.
        /// </summary>
        [Required]
        public string ConnectionString { get; set; } = null!;

        /// <summary>
        /// Gets or sets the virtual path.
        /// </summary>
        [Required]
        public string VirtualPath { get; set; } = null!;
    }
}
