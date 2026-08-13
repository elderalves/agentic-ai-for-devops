FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/OrderService/OrderService.csproj src/OrderService/
COPY src/SharedKernel/SharedKernel.csproj src/SharedKernel/
RUN dotnet restore src/OrderService/OrderService.csproj

COPY src/OrderService/ src/OrderService/
COPY src/SharedKernel/ src/SharedKernel/
RUN dotnet publish src/OrderService/OrderService.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "OrderService.dll"]
