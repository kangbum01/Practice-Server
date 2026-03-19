#include <iostream>
#include <string>
#include <thread>
#include <atomic>
#include <chrono>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <functional>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

bool SendALL(SOCKET sock, const char* data, int len)
{
    // totalSent는 이미 보낸 부분을 의미한다.
    int totalSent = 0;
    // 그래서 여기 while문을 보면 buffer 파트에 data + totalSent를 해
    // 그럼 아직 보내지지 않은 파트가 마저 보내지는거다.
    // len - totalSent도 마찬가지 아직 보내지 않는 부분을 뜻함
    while(totalSent < len)
    {
        int sent = send(sock, data + totalSent, len - totalSent, 0);
        if (sent == SOCKET_ERROR)
        {
            int err = WSAGetLastError();
            if(err == WSAEWOULDBLOCK)
            {
                this_thread::sleep_for(chrono::milliseconds(10));
                continue;
            }

            return false;

        }

        totalSent += sent;
    }

    return true;
}

bool WaitForConnect(SOCKET sock, int timeoutSec)
{
    fd_set writeSet;
    fd_set exceptSet;

    FD_ZERO(&writeSet);
    FD_ZERO(&exceptSet);

    FD_SET(sock, &writeSet);
    FD_SET(sock, &exceptSet);

    timeval tv;
    tv.tv_sec = timeoutSec;
    tv.tv_usec = 0;

    int result = select(0, NULL, &writeSet, &exceptSet, &tv);
    if( result <= 0)
    {
        return false;
    }

    if (FD_ISSET(sock, &exceptSet))
    {
        return false;
    }

    int soError = 0;
    int len = sizeof(soError);
    getsockopt(sock, SOL_SOCKET, SO_ERROR, (char*)&soError, &len);

    return soError == 0;
}

// atomic: 멀티스레드 환경에서 데이터 경함(DATA Race)을 방지하고 원자적연산을
// 보장하는 템플릿 클래스
// 원자성: atomic으로 선언된 변수는 읽기-수정-쓰기 연산이 중간에 다른 스레드에
// 의해 방해받지 않고 하나의 쪼개지지 않는 단위로 처리됩니다.
void ReceiveLoop(SOCKET clientSocket, atomic<bool>& running)
{
    char buffer[1024];

    while(running)
    {
        int recvLen = recv(clientSocket, buffer, sizeof(buffer) - 1, 0);
        
        if (recvLen > 0)
        {
            buffer[recvLen] = '\0';
            cout << "\n[From Server] " << buffer << endl;
            cout << "Input: ";
            cout.flush();  // 출력 스트림의 내부 버퍼의 데이터 제거
        }
        else if (recvLen == 0)
        {
            cout << "\nServer closed the connection. " << endl;
            running = false;
            break;
        }
        else
        {
            int err = WSAGetLastError();

            if (err != WSAEWOULDBLOCK)
            {
                cerr << "\nrecv failed: " << err << endl;
                running = false;
                break;
            }
        }

        this_thread::sleep_for(chrono::milliseconds(10));
    }
}

int main(void)
{
    WSADATA wsaData;
    int result = WSAStartup(MAKEWORD(2,2), &wsaData);
    if(result != 0)
    {
        cerr << "WSAStarup failed: " << result << endl;
        return 1;
    }

    SOCKET clientSocket = socket(AF_INET, SOCK_STREAM,IPPROTO_TCP );
    if (clientSocket == INVALID_SOCKET)
    {
        cerr << "socket failed: " << WSAGetLastError() << endl;
        WSACleanup();
        return 1;
    }

    u_long mode = 1;    
    result = ioctlsocket(clientSocket, FIONBIO, &mode);
    if (result == SOCKET_ERROR)
    {
        cerr << "ioctlsocket failed: " << WSAGetLastError() << endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(9000);
    serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");

    result = connect(clientSocket, (sockaddr*)&serverAddr, sizeof(serverAddr));
    if(result == SOCKET_ERROR)
    {
        int err = WSAGetLastError();
        if(err != WSAEWOULDBLOCK)
        {
            cerr << "connected failed: " << err << endl;
            closesocket(clientSocket);
            WSACleanup();
            return 1;
        }
    }

    if (!WaitForConnect(clientSocket, 5))
    {
        cerr << "connect timeout or failed. " << endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }

    cout << "Connected to server. " << endl;

    atomic<bool> running(true);
    thread recvThread(ReceiveLoop, clientSocket, ref(running));

    while(running)
    {
        string message;
        cout << "Input: ";
        getline(cin, message);

        if(!running)
        {
            break;
        }

        if(message == "/quit")
        {
            running = false;
            break;
        }

        if(message.empty())
        {
            continue;
        }

        bool ok = SendALL(clientSocket, message.c_str(), (int)message.size());
        if(!ok)
        {
            cerr << "send failed: " << WSAGetLastError() << endl;
            running = false;
            break;
        }
    }

    shutdown(clientSocket, SD_BOTH);
    closesocket(clientSocket);

    if(recvThread.joinable())
    {
        recvThread.join();
    }

    WSACleanup();
    return 0;
}


