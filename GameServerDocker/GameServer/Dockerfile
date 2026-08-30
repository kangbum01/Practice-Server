# .NET SDK 이미지를 베이스로 사용
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# 작업 디렉토리 설정
WORKDIR /app

# csproj 파일 복사 및 의존성 복원
COPY *.csproj ./
RUN dotnet restore

# 나머지 소스 코드 복사
COPY . ./

# 릴리스 모드로 빌드
RUN dotnet publish -c Release -o out

# Runtime 이미지로 변환
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# 빌드 결과물을 복사 
COPY --from=build /app/out/ .

# 서버가 사용할 포트
EXPOSE 8080

# 환경 변수 설정
ENV ASPNETCORE_URLS=http://+:8080

# 애플리케이션 실행
ENTRYPOINT [ "dotnet", "GameServer.dll" ]
