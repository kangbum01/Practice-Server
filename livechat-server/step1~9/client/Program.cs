using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DummyTester // 더미 클라이언트용 별도 프로젝트
{
    class Program
    {
        private static string _myName = "UnKnown";

        // 게임 상태 및 보드 랜더링용 변수들
        private static bool _isPlaying = false;
        private static int _myPosX = -1, _myPosY = -1;
        private static int _oppPosX = -1, _oppPosY = -1;
        private static string _opponentName = "";

        // 최근 채팅 로그 리스트
        private static List<string> _chatLogs = new List<string>();

        static async Task Main(string[] args)
        {
            Console.WriteLine("더미 봇 1000마리 투입을 시작합니다...");

            var bots = new List<Task>();

            for (int i = 0; i < 1000; i++)
            {
                bots.Add(StartDummyClient(i));
            }

            await Task.WhenAll(bots);

            Console.WriteLine("모든 봇의 연결이 종료되었습니다.");
        }

        public static async Task StartDummyClient(int botId)
        {
            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 5000);

                var writer = new StreamWriter(client.GetStream())
                {
                    AutoFlush = true
                };

                // 접속하자마자 로그인 패킷 전송
                await writer.WriteLineAsync($"LOGIN|Bot_{botId}");

                await Task.Delay(100);

                // 1초에 1번씩 채팅 패킷 전송
                while (true)
                {
                    await writer.WriteLineAsync(
                        $"CHAT|Lobby|Bot_{botId}|부하테스트 중입니다!"
                    );

                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[Bot_{botId} Error] {ex.Message}"
                );
            }
        }
    }
}