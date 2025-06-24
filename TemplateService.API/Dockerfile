FROM digital-cr.hq.corp.mmk.chel.su/library/dotnet/aspnet:7.0-jammy
WORKDIR /app
EXPOSE 5124
EXPOSE 3119

COPY Src/Services/TemplateService/TemplateService.API/app/ . 
ENTRYPOINT ["dotnet", "TemplateService.API.dll"]