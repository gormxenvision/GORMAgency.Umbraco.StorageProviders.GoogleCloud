using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.DependencyInjection;
using GORMAgency.Umbraco.StorageProviders.GoogleCloud.IO;

namespace GORMAgency.Umbraco.StorageProviders.GoogleCloud.DependencyInjection
{
    /// <summary>
    /// Extension methods to help registering Google Cloud Storage file systems for Umbraco media.
    /// </summary>
    public static class GoogleCloudMediaFileSystemExtensions
    {
        /// <summary>
        /// Registers an <see cref="IGoogleCloudFileSystem" /> and it's dependencies configured for media.
        /// </summary>
        /// <param name="builder">The <see cref="IUmbracoBuilder" />.</param>
        /// <param name="useGoogleCloudImageCache">If set to <c>true</c> also configures Google Cloud Storage for the image cache.</param>
        /// <returns>
        /// The <see cref="IUmbracoBuilder" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder" /> is <c>null</c>.</exception>
        public static IUmbracoBuilder AddGoogleCloudMediaFileSystem(this IUmbracoBuilder builder, bool useGoogleCloudImageCache = true)
            => builder.AddInternal(useGoogleCloudImageCache);

        /// <summary>
        /// Registers a <see cref="IGoogleCloudFileSystem" /> and it's dependencies configured for media.
        /// </summary>
        /// <param name="builder">The <see cref="IUmbracoBuilder" />.</param>
        /// <param name="configure">An action used to configure the <see cref="GoogleCloudFileSystemOptions" />.</param>
        /// <param name="useGoogleCloudImageCache">If set to <c>true</c> also configures Google Cloud Storage for the image cache.</param>
        /// <returns>
        /// The <see cref="IUmbracoBuilder" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder" /> is <c>null</c>.</exception>
        public static IUmbracoBuilder AddGoogleCloudMediaFileSystem(this IUmbracoBuilder builder, Action<GoogleCloudFileSystemOptions> configure, bool useGoogleCloudImageCache = true)
            => builder.AddInternal(useGoogleCloudImageCache, x => x.Configure(configure));

        /// <summary>
        /// Registers a <see cref="IGoogleCloudFileSystem" /> and it's dependencies configured for media.
        /// </summary>
        /// <param name="builder">The <see cref="IUmbracoBuilder" />.</param>
        /// <param name="configure">An action used to configure the <see cref="GoogleCloudFileSystemOptions" />.</param>
        /// <param name="useGoogleCloudImageCache">If set to <c>true</c> also configures Google Cloud Storage for the image cache.</param>
        /// <returns>
        /// The <see cref="IUmbracoBuilder" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder" /> is <c>null</c>.</exception>
        public static IUmbracoBuilder AddGoogleCloudFileSystem(this IUmbracoBuilder builder, Action<GoogleCloudFileSystemOptions, IServiceProvider> configure, bool useGoogleCloudImageCache = true)
            => builder.AddInternal(useGoogleCloudImageCache, x => x.Configure(configure));

        /// <summary>
        /// Registers a <see cref="IGoogleCloudFileSystem" /> and it's dependencies configured for media.
        /// </summary>
        /// <typeparam name="TDep">A dependency used by the configure action.</typeparam>
        /// <param name="builder">The <see cref="IUmbracoBuilder" />.</param>
        /// <param name="configure">An action used to configure the <see cref="GoogleCloudFileSystemOptions" />.</param>
        /// <param name="useGoogleCloudImageCache">If set to <c>true</c> also configures Google Cloud Storage for the image cache.</param>
        /// <returns>
        /// The <see cref="IUmbracoBuilder" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder" /> is <c>null</c>.</exception>
        public static IUmbracoBuilder AddGoogleCloudFileSystem<TDep>(this IUmbracoBuilder builder, Action<GoogleCloudFileSystemOptions, TDep> configure, bool useGoogleCloudImageCache = true)
            where TDep : class
            => builder.AddInternal(useGoogleCloudImageCache, x => x.Configure(configure));

        /// <summary>
        /// Registers a <see cref="IGoogleCloudFileSystem" /> and it's dependencies configured for media.
        /// </summary>
        /// <param name="builder">The <see cref="IUmbracoBuilder" />.</param>
        /// <param name="useGoogleCloudImageCache">If set to <c>true</c> also configures Google Cloud Storage for the image cache.</param>
        /// <param name="configure">An action used to configure the <see cref="GoogleCloudFileSystemOptions" />.</param>
        /// <returns>
        /// The <see cref="IUmbracoBuilder" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder" /> is <c>null</c>.</exception>
        internal static IUmbracoBuilder AddInternal(this IUmbracoBuilder builder, bool useGoogleCloudImageCache, Action<OptionsBuilder<GoogleCloudFileSystemOptions>>? configure = null)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.AddInternal(GoogleCloudFileSystemOptions.MediaFileSystemName, optionsBuilder =>
            {
                optionsBuilder.Configure<IOptions<GlobalSettings>>((options, globalSettings) => options.VirtualPath = globalSettings.Value.UmbracoMediaPath);
                configure?.Invoke(optionsBuilder);
            });

            builder.SetMediaFileSystem(provider => provider.GetRequiredService<IGoogleCloudFileSystemProvider>().GetFileSystem(GoogleCloudFileSystemOptions.MediaFileSystemName));

            return builder;
        }
    }
}
