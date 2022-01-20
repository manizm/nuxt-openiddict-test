﻿using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpeniddictServer.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpeniddictServer
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync(cancellationToken);

            await RegisterApplicationsAsync(scope.ServiceProvider);
            await RegisterScopesAsync(scope.ServiceProvider);

            static async Task RegisterApplicationsAsync(IServiceProvider provider)
            {
                var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

                // Angular UI client
                if (await manager.FindByClientIdAsync("nuxtclient") is null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "nuxtclient",
                        ConsentType = ConsentTypes.Implicit,
                        DisplayName = "nuxt client PKCE",
                        DisplayNames =
                        {
                            [CultureInfo.GetCultureInfo("fr-FR")] = "Application cliente MVC"
                        },
                        PostLogoutRedirectUris =
                        {
                            new Uri("http://localhost:3000"),
                            new Uri("http://localhost:3000/")
                        },
                        RedirectUris =
                        {
                            new Uri("http://localhost:3000"),
                            new Uri("http://localhost:3000/"),
                            new Uri("http://localhost:3000/login")
                        },
                        Permissions =
                        {
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Logout,
                            Permissions.Endpoints.Token,
                            Permissions.Endpoints.Revocation,
                            Permissions.GrantTypes.AuthorizationCode,
                            Permissions.GrantTypes.RefreshToken,
                            Permissions.ResponseTypes.Code,
                            Permissions.Scopes.Email,
                            Permissions.Scopes.Profile,
                            Permissions.Scopes.Roles,
                            Permissions.Prefixes.Scope + "dataEventRecords"
                        },
                        Requirements =
                        {
                            Requirements.Features.ProofKeyForCodeExchange
                        }
                    });
                }

                // API
                if (await manager.FindByClientIdAsync("rs_dataEventRecordsApi") == null)
                {
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = "rs_dataEventRecordsApi",
                        ClientSecret = "dataEventRecordsSecret",
                        Permissions =
                        {
                            Permissions.Endpoints.Introspection
                        }
                    };

                    await manager.CreateAsync(descriptor);
                }

                // Blazor Hosted
                if (await manager.FindByClientIdAsync("blazorcodeflowpkceclient") is null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "blazorcodeflowpkceclient",
                        ConsentType = ConsentTypes.Explicit,
                        DisplayName = "Blazor code PKCE",
                        DisplayNames =
                        {
                            [CultureInfo.GetCultureInfo("fr-FR")] = "Application cliente MVC"
                        },
                        PostLogoutRedirectUris =
                        {
                            new Uri("https://localhost:44348/signout-callback-oidc"),
                            new Uri("https://localhost:5001/signout-callback-oidc")
                        },
                        RedirectUris =
                        {
                            new Uri("https://localhost:44348/signin-oidc"),
                            new Uri("https://localhost:5001/signin-oidc")
                        },
                        ClientSecret = "codeflow_pkce_client_secret",
                        Permissions =
                        {
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Logout,
                            Permissions.Endpoints.Token,
                            Permissions.Endpoints.Revocation,
                            Permissions.GrantTypes.AuthorizationCode,
                            Permissions.GrantTypes.RefreshToken,
                            Permissions.ResponseTypes.Code,
                            Permissions.Scopes.Email,
                            Permissions.Scopes.Profile,
                            Permissions.Scopes.Roles,
                            Permissions.Prefixes.Scope + "dataEventRecords"
                        },
                        Requirements =
                        {
                            Requirements.Features.ProofKeyForCodeExchange
                        }
                    });
                }
            }

            static async Task RegisterScopesAsync(IServiceProvider provider)
            {
                var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

                if (await manager.FindByNameAsync("dataEventRecords") is null)
                {
                    await manager.CreateAsync(new OpenIddictScopeDescriptor
                    {
                        DisplayName = "dataEventRecords API access",
                        DisplayNames =
                        {
                            [CultureInfo.GetCultureInfo("fr-FR")] = "Accès à l'API de démo"
                        },
                        Name = "dataEventRecords",
                        Resources =
                        {
                            "rs_dataEventRecordsApi"
                        }
                    });
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
