#include <iostream>
#include <string>
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
		cerr << "WSAStartup faile: " << result << endl;
		return 1;
	}
	SOCKET clientSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (clientSocket == INVALID_SOCKET)
	{
		cerr << "socket failed: " << WSAGetLastError() << endl;
		WSACleanup();
		return 1;
	}

	sockaddr_in serverAddr;
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_port = htons(9000);
	serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");

	result = connect(clientSocket, (sockaddr*)&serverAddr, sizeof(serverAddr));
	if (result == SOCKET_ERROR)
	{
		cerr << "connect failed: " << WSAGetLastError() << endl;
		closesocket(clientSocket);
		WSACleanup();
		return 1;
	}

	cout << "Connected to server!" << endl;
	while (true)
	{
		string message;
		cout << "Enter message to send (/quit to exit): ";
		getline(cin, message);

		if (message == "/quit")
		{
			break;
		}
		int sendLen = send(clientSocket, message.c_str(), (int)message.size(), 0);
		if (sendLen == SOCKET_ERROR)
		{
			cerr << "send failed: " << WSAGetLastError() << endl;
			break;
		}
		char buffer[1024];
		int recvLen = recv(clientSocket, buffer, sizeof(buffer) -1, 0);
		if (recvLen == 0)
		{
			cout << "Server has disconnected." << endl;
			break;
		}
		if (recvLen == SOCKET_ERROR)
		{
			cerr << "recv failed: " << WSAGetLastError() << endl;
			break;
		}

		buffer[recvLen] = '\0';
		cout << "Message returned by server: " << buffer << endl;
	}
	closesocket(clientSocket);
	WSACleanup();

	return 0;
}
