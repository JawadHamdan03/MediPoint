# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["MediPoint.Api/MediPoint.Api.csproj", "MediPoint.Api/"]
COPY ["MediPoint.Application/MediPoint.Application.csproj", "MediPoint.Application/"]
COPY ["MediPoint.Infrastructure/MediPoint.Infrastructure.csproj", "MediPoint.Infrastructure/"]
COPY ["MediPoint.Domain/MediPoint.Domain.csproj", "MediPoint.Domain/"]

RUN dotnet restore "MediPoint.Api/MediPoint.Api.csproj"

COPY . .

WORKDIR "/src/MediPoint.Api"

RUN dotnet publish "MediPoint.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080


ENV ASPNETCORE_HTTP_PORTS=8080

ENV MediPoint__ConnectionString="mongodb://host.docker.internal:27017"
ENV MediPoint__DatabaseName="MediPoint"
ENV MediPoint__MedicalRecordsCollectionName="MedicalRecords"
ENV MediPoint__LabResultsCollectionName="LabResults"
ENV MediPoint__MedicineCollectionName="Medicine"

ENV ConnectionStrings__DefaultConnection="Server=host.docker.internal,1434;Database=MediPoint;User Id=sa;Password=123456;TrustServerCertificate=True;"

ENV JwtSettings__Issuer="localhost"
ENV JwtSettings__Audience="localhost"
ENV JwtSettings__TokenExpirationInMinutes="200"
ENV JwtSettings__SecretKey="ThisIsAVeryStrongSecretKeyForYourDemoApplication!@#$"

ENV OpenAI__ApiKey=""
ENV OpenAI__Model="gpt-4o-mini"

ENV SmtpSettings__Server="smtp.gmail.com"
ENV SmtpSettings__Port="587"
ENV SmtpSettings__SenderName="Jawad Hamdan"
ENV SmtpSettings__SenderEmail="HamdanJawad48@gmail.com"
ENV SmtpSettings__Username="HamdanJawad48@gmail.com"
ENV SmtpSettings__Password="zxti ilwl siin hxag"

EXPOSE 8080

ENTRYPOINT ["dotnet", "MediPoint.Api.dll"]