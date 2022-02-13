[![.NET](https://github.com/c00kw1/cookwi-api-v2/actions/workflows/dotnet.yml/badge.svg)](https://github.com/c00kw1/cookwi-api-v2/actions/workflows/dotnet.yml)
[![Release homologation](https://github.com/c00kw1/cookwi-api-v2/actions/workflows/dotnet-rel-homolo.yml/badge.svg)](https://github.com/c00kw1/cookwi-api-v2/actions/workflows/dotnet-rel-homolo.yml)

# cookwi-api-v2

## Runtime

Those ENV variables must be set for the API to work correctly on a containerized environment.

| Variable               | Value                                         | What is it ?                                                      |
| ---------------------- | --------------------------------------------- | ----------------------------------------------------------------- |
| ASPNETCORE_ENVIRONMENT | `Development` / `Homologation` / `Production` | The current runtime env                                           |
| API_JWT_SECRET         | `string`                                      | Secret token used to sign jwt tokens                              |
| MAIL_SMTP_PWD          | `string`                                      | Password used for SMTP server (when the API needs to send emails) |
| DB_CONNECTION_STRING   | `string`                                      | Connection string used by npgsql                                  |
