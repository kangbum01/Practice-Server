#include <iostream>
#include <vector>
#include <WinSock2.h>
#include <WS2tcpip.h>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

bool SendAll(SOCKET socket, const char* data, int len)
{
    int totalSent = 0;

    while (totalSent < len)
    {
        int sent = send(socket , data + totalSent, len - totalSent,0);
        if (sent == SOCKET_ERROR)
        {
            return false;
        }

        totalSent += sent;
    }
    return true;
}

int main(void)
{
    WSADATA wsaData;
    int result = WSAStartup(MAKEWORD(2,2), &wsaData);
    if (result != 0)
    {
        cerr << "WSAStartup failed: " << result << endl;
        return 1;
    }
    SOCKET listenerSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (listenerSocket == INVALID_SOCKET)
    {
        cerr << "socket failed: " << WSAGetLastError() << endl;
        WSACleanup();
        return 1;
    }

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(9000);
    serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");

    result = bind(listenerSocket, (sockaddr*)&serverAddr, sizeof(serverAddr));
    if (result == SOCKET_ERROR)
    {
        cerr << "bind failed: " << WSAGetLastError() << endl;
        closesocket(listenerSocket);
        WSACleanup();
        return 1;
    }

    result = listen(listenerSocket, SOMAXCONN);
    if(result == SOCKET_ERROR)
    {
        cerr << "listen failed: " << WSAGetLastError() << endl;
        closesocket(listenerSocket);
        WSACleanup();
        return 1;
    }

    cout << "Server is listening on 127.0.0.1:9000 " << endl;

    // client소켓 벡터
    vector<SOCKET> clientSockets;
    while(true)
    {
        // fd_set : select()에 넘기는 소켓 집합
        fd_set readSet;
        FD_ZERO(&readSet);

        FD_SET(listenerSocket, &readSet);

        for (size_t i = 0; i < clientSockets.size(); i++)
        {
            FD_SET(clientSockets[i], &readSet);
        }

        int readyCount = select(0,&readSet, NULL,NULL,NULL);
        if(readyCount == SOCKET_ERROR)
        {
            cerr << "select failed: " << WSAGetLastError() << endl;
            break;
        }
        
        // FD에 읽을 내용이 있는지 확인
        if (FD_ISSET(listenerSocket, &readSet))
        {
            // 소켓 버퍼
            sockaddr_in clientAddr;
            int clientAddrSize = sizeof(clientAddr);
            
            SOCKET clientSocket = accept(listenerSocket, (sockaddr*)&clientAddr, &clientAddrSize);
            if(clientSocket == INVALID_SOCKET)
            {
                cerr << "accept failed: " << WSAGetLastError() << endl;
            }
            else
            {
                clientSockets.push_back(clientSocket);
                cout << "Client connected. current clients = " << clientSockets.size() << endl;
            }
        }
        for (size_t i = 0; i < clientSockets.size(); )
        {
            SOCKET clientSocket = clientSockets[i];
            // 읽을 소켓이 없다면 다음으로 넘어간다~
            if (!FD_ISSET(clientSocket, &readSet))
            {
                ++i;
                continue;
            }

            char buffer[1024];
            int recvLen = recv(clientSocket, buffer, sizeof(buffer), 0);
            // 논블로킹 소켓에서는 데이터가 없다? -> SOCKET_ERROR + WSAEWOULDBLOCK의 값이 나옴
            // 즉 0이면 연결 끝 >0이면 뭔가 연결중이긴 하다 <0 이건 오류
            if (recvLen == 0)
            {
                cout << "Client disconnected." << endl;
                closesocket(clientSocket);
                clientSockets.erase(clientSockets.begin() + i);
                continue;
            }

            buffer[recvLen] = '\0';
            cout << "Received: " << buffer << endl;

            // 에코 파트 클라에게 다시 전송
            bool echomessage = SendAll(clientSocket, buffer, recvLen);
            if(!echomessage)
            {
                cerr << "send failed: " << WSAGetLastError() << endl;
                closesocket(clientSocket);
                clientSockets.erase(clientSockets.begin() + i);
            }
            ++i;
        }

    }
    // 종료파트 반복문으로 소켓들 전부 제거
    for (size_t i = 0; i < clientSockets.size(); ++i)
    {
        closesocket(clientSockets[i]);
    }

    closesocket(listenerSocket);
    WSACleanup();
    return 0;
}