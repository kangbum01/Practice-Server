using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DummyTester // 더미 클라이언트용 별도 프로젝트
{
    class Program
    {
        // 1. 실행의 진입점인 Main 함수 내부에 '제어 로직'을 넣어야 합니다.
        static async Task Main(string[] args)
        {
            Console.WriteLine("더미 봇 1000마리 투입을 시작합니다...");

            // ✅ var와 for문은 반드시 이렇게 함수(Main) '안쪽'에 있어야 합니다!
            var bots = new List<Task>();
            for(int i = 0; i < 1000; i++) 
            {
                bots.Add(StartDummyClient(i));
            }

            // 1000개의 비동기 작업이 모두 끝날 때까지 대기 (필수)
            await Task.WhenAll(bots); 

            Console.WriteLine("모든 봇의 연결이 종료되었습니다.");
        }

        // 2. StartDummyClient는 Main과 동급인 '메서드'이므로 클래스 바로 아래에 둡니다.
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

                // 1초에 1번씩 미친듯이 채팅 패킷 전송 (서버 부하 유발)
                while(true)
                {
                    await writer.WriteLineAsync($"CHAT|Lobby|Bot_{botId}|부하테스트 중입니다!");
                    await Task.Delay(1000); // 1초 대기 (더 하드코어하게 하려면 100으로 줄이세요)
                }
            }
            catch (Exception ex)
            {
                // 봇이 서버에서 튕기거나 연결에 실패했을 때의 방어 로직
                Console.WriteLine($"[Bot_{botId} Error] {ex.Message}");
            }
        }
    }
}