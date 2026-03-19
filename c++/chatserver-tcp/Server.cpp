#include <iostream>
#include <WinSock2.h>
#include <WS2tcpip.h>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

int main()
{
	WSADATA wsaData;
	int result = WSAStartup(MAKEWORD(2, 2), &wsaData);
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

	// 소켓 생성
	sockaddr_in serverAddr;
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_port = htons(9000);
	serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");

	// 바인드
	result = bind(listenerSocket, (sockaddr*)&serverAddr, sizeof(serverAddr));
	if (result == SOCKET_ERROR)
	{
		cerr << "bind error: " << WSAGetLastError() << endl;
		closesocket(listenerSocket);
		WSACleanup();
		return 1;
	}

	// listen 소켓
	result = listen(listenerSocket, SOMAXCONN);
	if (result == SOCKET_ERROR)
	{
		cerr << "listen failed: " << WSAGetLastError() << endl;
		closesocket(listenerSocket);
		WSACleanup();
		return 1;
	}

	cout << "Server is listening on 127.0.0.1:9000 ..." << endl;

	sockaddr_in clientAddr;
	int clientAddrSize = sizeof(clientAddr);

	// client 소켓 생성
	SOCKET clientSocket = accept(listenerSocket, (sockaddr*)&clientAddr, &clientAddrSize);
	if (clientSocket == INVALID_SOCKET)
	{
		cerr << "accept failed: " << WSAGetLastError() << endl;
		closesocket(listenerSocket);
		WSACleanup();
		return 1;
	}

	cout << "Client connected successfully!" << endl;

	char buffer[1024];

	while (true)
	{
		int recvLen = recv(clientSocket, buffer, sizeof(buffer) - 1, 0);

		if (recvLen == 0)
		{
			// 전달할 메시지가 없다는거
			cout << "Client has disconnected." << endl;
			break;
		}

		if (recvLen == SOCKET_ERROR)
		{
			// 오류 발생
			cerr << "recv failed : " << WSAGetLastError() << endl;
			break;
		}

		buffer[recvLen] = '\0';
		cout << "Received message: " << buffer << endl;

		int sendLen = send(clientSocket, buffer, recvLen, 0);	
		if (sendLen == SOCKET_ERROR)
		{
			cerr << "send failed: " << WSAGetLastError() << endl;
			break;
		}

		cout << "Echo sent successfully: " << buffer << endl;
	}

	closesocket(clientSocket);
	closesocket(listenerSocket);
	WSACleanup();

	return 0;
}