FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

COPY --from=build /app/out .

# 소켓 서버 포트 노출
EXPOSE 9000

# 여기서 원하는 포트로 변경 가능
ENV GAME_PORT=9000

ENTRYPOINT [ "dotnet", "SocketGameServer.dll" ]