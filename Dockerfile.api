# Используем SDK-образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем csproj-файлы и восстанавливаем зависимости
COPY TemplateService.Domain/TemplateService.Domain.csproj TemplateService.Domain/
COPY TemplateService.Application/TemplateService.Application.csproj TemplateService.Application/
COPY TemplateService.Infrastructure/TemplateService.Infrastructure.csproj TemplateService.Infrastructure/
COPY TemplateService.API/TemplateService.API.csproj TemplateService.API/

RUN dotnet restore TemplateService.API/TemplateService.API.csproj

# Копируем остальной код
COPY . .

# Сборка
WORKDIR /app/TemplateService.API
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out ./

ENV ASPNETCORE_URLS=http://+:5124
EXPOSE 5124


ENTRYPOINT ["dotnet", "TemplateService.API.dll"]