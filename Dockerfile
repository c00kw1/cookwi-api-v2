FROM mcr.microsoft.com/dotnet/runtime:6.0

ARG path_to_binaries="./cookwi-api-bin"
ARG environment="Homologation"

COPY $path_to_binaries/ /var/www/api/

RUN apt-get update && apt-get install -y libc6-dev libgdiplus libx11-dev

EXPOSE 5000

WORKDIR /var/www/api/
ENV ASPNETCORE_ENVIRONMENT $environment
ENV ASPNETCORE_URLS "http://*:5000"
ENTRYPOINT [ "dotnet", "Cookwi.Api.dll" ]
