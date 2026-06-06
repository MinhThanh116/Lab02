# Bước 1: Dùng SDK .NET 10.0 để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /App

# Copy các file và khôi phục thư viện (Restore)
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ code còn lại và build dự án
COPY . ./
RUN dotnet publish -c Release -o out

# Bước 2: Tạo runtime image .NET 10.0 nhẹ để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /App
COPY --from=build-env /App/out .

# Cấu hình cổng chạy trong Docker khớp với cổng http của bạn
ENV ASPNETCORE_URLS=http://+:5175
EXPOSE 5175

ENTRYPOINT ["dotnet", "DapperApi.dll"]