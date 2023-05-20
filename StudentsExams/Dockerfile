FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["StudentsExamsWebAPI/StudentsExamsWebAPI.csproj", "StudentsExamsWebAPI/"]
COPY ["StudentsExamsLib/StudentsExamsLib.csproj", "StudentsExamsLib/"]
RUN dotnet restore "StudentsExamsWebAPI/StudentsExamsWebAPI.csproj"
COPY . .
WORKDIR "/src/StudentsExamsWebAPI"
RUN dotnet build "StudentsExamsWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StudentsExamsWebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentsExamsWebAPI.dll"]
