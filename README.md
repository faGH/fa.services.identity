# fa.services.identity
## Description
FrostAura identity-as-a-service platform.
## Status
| Project | Build | Test Coverage
| --- | --- | --- |
| FrostAura.Services.Identity | ![TravisCI](https://travis-ci.org/faGH/fa.services.identity.svg?branch=master) | PENDING |
## Supported Auth Flows
- API resources (Web services etc).
- Client resources (Native clients, HTML5 clients, etc).
- OpenId Connect / OAuth2 sign-in and concent flow.
## Database Migrations (EF Core)
### Overview
For migrations, we need to add them initially and update or re-add them each time the context changes. The actual execution of migrations happen on application start and is autonomous.
### Lessons Learnt
- In order to create migrations, the DB context has to either
    - Have a default constructor.
    - Have a IDesignTimeDbContextFactory implementation. - This allows for providing the args to the overloaded constructor during design-time as there is no DI available during that time. 

### Re-create Migrations (Package-Manager Console)
> dotnet tool install --global dotnet-ef

> dotnet ef migrations add InitialIdentityServerStoreDbMigration -c IdentityDbContext -o Migrations/IdentityServer/IdentityDb --project FrostAura.Services.Identity.Data

> dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Migrations/IdentityServer/PersistedGrantDb --project FrostAura.Services.Identity.Data

> dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Migrations/IdentityServer/ConfigurationDb --project FrostAura.Services.Identity.Data

## Docker Support
### Local
The project supports being run as a container and is in fact indended to. In order to run this service locally, simply run `docker-compose up` in the directory where the `docker-compose.yml` file resides. The service will now run on port 8083:HTTPS, 8082:HTTP.
### Docker Hub
Automated builds are set up for Docker Hub. To use this service without the source code running, use 
- `docker pull frostauraconsolidated/idenitity` or 
- Visit https://hub.docker.com/repository/docker/frostauraconsolidated/idenitity.

After the service is running, you can connect another application to it via openid or simply confirm that the service is running correctly by navigating to https://localhost:8083/.well-known/openid-configuration.

NOTE: For the container to work (HTTPS) by itself, we will require a valid SSL cert. You can simply create a dev self-signed cert using dotnet CLI, after which we can map it to the container. See below for local / development. For production you should have a valid cert. See https://docs.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-3.1 for more.

    dotnet dev-certs https --clean
    dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Password1234
    dotnet dev-certs https --trust
#### Docker Compose Example
    version: "3"
        services:
            db:
                image: "mcr.microsoft.com/mssql/server"
                environment:
                    - SA_PASSWORD=Password1234
                    - ACCEPT_EULA=Y
                ports:
                    - 1433:1433
            web:
                image: "frostauraconsolidated/idenitity"
                ports:
                    - 8082:80
                    - 8083:443
                depends_on:
                    - db
                environment:
                    - ASPNETCORE_URLS=https://+:443;http://+:80
                    - ASPNETCORE_Kestrel__Certificates__Default__Password=Password1234
                    - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
                volumes:
                    - ~/.aspnet/https:/https:ro

## How To
### Getting Familiar
To get context on how to consume the service, see the unit tests and integration tests for context on total usage.
### Customization
Customization is allowed on a per-client basis. These client credentials are then provided by the platform that connects to the identity service.

We can then use the context of the client to override some configuration. In order to use customization on the auth pages, the client credentials you are usign, should have the following claims assigned to them in the config db.
#### Custom Styling
| Claims Key | Example Value |
| --- | --- |
| FrostAura.Clients.CustomCssUrl | https://demo.com/custom.css |

#### Custom Icon (SVG only)
| Claims Key | Example Value |
| --- | --- |
| FrostAura.Clients.CustomLogoSvgUrl | https://demo.com/custom-icon.svg |

#### Custom App Name
| Claims Key | Example Value |
| --- | --- |
| FrostAura.Clients.Name | Demo Application Name |

## Contribute
In order to contribute, simply fork the repository, make changes and create a pull request.

## Support
For any queries, contact dean.martin@frostaura.net.
