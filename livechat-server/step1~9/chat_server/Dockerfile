# 1. 빌드 용 이미지
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 2. 프로젝트 파일만 먼저 복사하고 패키지 복원 (캐시 활용 최적화)
COPY *.csproj ./
RUN dotnet restore

# 3. 나머시 모든 소스 코드를 복사하고 Release 모드로 빌드
COPY . ./
RUN dotnet publish -c Release -o out

# 4. 실제 실행용 이미지 (Runtime만 포함, 매우 가벼움)
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# 5. 빌드 단계에서 만들어진 결과물(out 폴더)만 실행용 이미지로 가져옴
COPY --from=build /app/out .

# 6. 외부와 통신할 포트 열어주기 (우리 서버가 5000번 포트를 쓴다면)
EXPOSE 5000

# 7. 컨테이너가 켜질 때 서버 실행 (Server.dll 부분은 실제 프로젝트 이름에 맞게 수정)
ENTRYPOINT ["dotnet", "Server.dll"]

