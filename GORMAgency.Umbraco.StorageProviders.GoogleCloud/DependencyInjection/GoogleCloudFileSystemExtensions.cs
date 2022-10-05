using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.DependencyInjection;
using GORMAgency.Umbraco.StorageProviders.GoogleCloud.IO;

namespace GORMAgency.Umbraco.StorageProviders.GoogleCloud.DependencyInjection
{
    /// <summary>
    /// Extension methods to help registering Google Cloud Storage file systems.
    /// </summary>
    public static class GoogleCloudFileSystemExtensions
    {
        /// <summary>
        /// Registers a <see cref="IGoogleCloudFileSystem" /> in the <see cref="IServiceCollection" />, with it's configuration
        /// loaded from <c>Umbraco:Storage:GoogleCloud:{name}</c> where {name} is the value of the <paramref name="name" /> parameter.
        /// </summary>
        /// <param name="builder">The <see cref="IUmbracoBuilder" />.</param>
        /// <param name="name">The name of the file system.</param>
        /// <returns>
        /// The <see cref="IUmbracoBuilder" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder" /> is <c>null</c>.</exception>
        public static IUmbracoBuilder AddGoogleCloudFileSystem(this IUmbracoBuilder builder, string name)
            => builder.AddInternal(name);

        /// <summary>
        /// Registers a <see cref="IGoogleCloudFileSystem" /> in the <see cref="IServiceCollection" />, with it's configuration
        /// loaded from <c>Umbraco:Storage:GoogleCloud:{name}</c> where {name} is the value of the <paramref name="name" /> parameter.
        /// </summary>
        /// <param name="builder">The <see cref="IUmbracoBuilder" />.</param>
        /// <param name="name">The name of the file system.</param>
        /// <param name="configure">An action used to configure the <see cref="GoogleCloudFileSystemOptions" />.</param>
        /// <returns>
        /// The <see cref="IUmbracoBuilder" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        public static IUmbracoBuilder AddGoogleCloudFileSystem(this IUmbracoBuilder builder, string name, Action<GoogleCloudFileSystemOptions> configure)
            => builder.AddInternal(name, optionsBuilder => optionsBuilder.Configure(configure));

        /// <summary>
        /// Registers a <see cref="IGoogleCloudFileSystem" /> in the <see cref="IServiceCollection" />, with it's configuration
        /// loaded from <c>Umbraco:Storage:GoogleCloud:{name}</c> where {name} is the value of the <paramref name="name" /> parameter.
        /// </summary>
        /// <param name="builder">The <see cref="IUmbracoBuilder" />.</param>
        /// <param name="name">The name of the file system.</param>
        /// <param name="configure">An action used to configure the <see cref="GoogleCloudFileSystemOptions" />.</param>
        /// <returns>
        /// The <see cref="IUmbracoBuilder" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        public static IUmbracoBuilder AddGoogleCloudFileSystem(this IUmbracoBuilder builder, string name, Action<GoogleCloudFileSystemOptions, IServiceProvider> configure)
            => builder.AddInternal(name, optionsBuilder => optionsBuilder.Configure(configure));

        /// <summary>
        /// Registers a <see cref="IGoogleCloudFileSystem" /> in the <see cref="IServiceCollection" />, with it's configuration
        /// loaded from <c>Umbraco:Storage:GoogleCloud:{name}</c> where {name} is the value of the <paramref name="name" /> parameter.
        /// </summary>
        /// <typeparam name="TDep">A dependency used by the configure action.</typeparam>
        /// <param name="builder">The <see cref="IUmbracoBuilder" />.</param>
        /// <param name="name">The name of the file system.</param>
        /// <param name="configure">An action used to configure the <see cref="GoogleCloudFileSystemOptions" />.</param>
        /// <returns>
        /// The <see cref="IUmbracoBuilder" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        public static IUmbracoBuilder AddGoogleCloudFileSystem<TDep>(this IUmbracoBuilder builder, string name, Action<GoogleCloudFileSystemOptions, TDep> configure)
            where TDep : class
            => builder.AddInternal(name, optionsBuilder => optionsBuilder.Configure(configure));

        /// <summary>
        /// Registers a <see cref="IGoogleCloudFileSystem" /> in the <see cref="IServiceCollection" />, with it's configuration
        /// loaded from <c>Umbraco:Storage:GoogleCloud:{name}</c> where {name} is the value of the <paramref name="name" /> parameter.
        /// </summary>
        /// <param name="builder">The <see cref="IUmbracoBuilder" />.</param>
        /// <param name="name">The name of the file system.</param>
        /// <param name="configure">An action used to configure the <see cref="GoogleCloudFileSystemOptions" />.</param>
        /// <returns>
        /// The <see cref="IUmbracoBuilder" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        internal static IUmbracoBuilder AddInternal(this IUmbracoBuilder builder, string name, Action<OptionsBuilder<GoogleCloudFileSystemOptions>>? configure = null)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(name);

            builder.Services.TryAddSingleton<IGoogleCloudFileSystemProvider, GoogleCloudFileSystemProvider>();

            var optionsBuilder = builder.Services.AddOptions<GoogleCloudFileSystemOptions>(name)
                .BindConfiguration($"Umbraco:Storage:GoogleCloud:{name}")
                .ValidateDataAnnotations();

            configure?.Invoke(optionsBuilder);

            return builder;
        }
    }
}
