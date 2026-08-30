using System;
using System.Runtime.InteropServices;

namespace Server
{
    // 1. 패킷 식별자 PacketID ushort 설정은 enum의 기본 타입인 int보다 더 작은 크기의 ushort로도
    // 패킷의 종류를 충분히 넣을 수 있기 때문
    public enum PacketID : ushort
    {
        C2S_MOVE = 1001,
        S2C_MOVE = 1002,
        C2S_CHAT = 2001,
        S2C_CHAT = 2002
    }

    // 2. 공통 패킷 헤더 (메모리 정렬 고정)
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketHeader
    {
        public ushort Size; // 패킷 전체 크기 (Header + body)
        public ushort Id; // PacketID
    }

    // 3. 이동 요청 패킷 바디 
    // 클라 -> 서버
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct C2S_MOVEPacket
    {
        public int TargetX;
        public int TargetY;
    }

    // 4. 이동 결과 응답 패킷 바디 (닉네임 + 좌표)
    // C# 구조체 내 고정 크기 문자열/배열 처리를 위해 unsafe fixed buffer 활용
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct S2C_MovePacket
    {
        public fixed byte NickName[32]; // 닉네임은 32 바이트로 고정
        public int PosX;
        public int PosY;
    }

    // 5. Zero-Copy 직렬화 / 역직렬화 헬퍼 클래스
    public static class PacketSerializer
    {
        // [구조체 -> Span<byte>] 메모리 복사 없이 바이트 버퍼로 변환
        public static bool TrySerialize<T>(unshort packetId, ref T srcBody, Span<byte> destination, out int totalBytesWritten) where T : struct
        {
            int headerSize = Marshal.SizeOf<PacketHeader>();
            int bodySize = Marshal.SizeOf<T>();
            totalBytesWritten = headerSize + bodySize;

            if (destination.Length < totalBytesWritten)
                return false;
            
            // Header 작성
            PacketHeader header = new PacketHeader
            {
                Size = (ushort)totalBytesWritten,
                Id = packetId
            };

            MemoryMarshal.Write(destination.Slice(0, headerSize), ref header);

            // Body 작성
            MemoryMarshal.Write(destination.Slice(headerSize, bodySize), ref srcBody);

            return true;
        }

        public static T Deserialize<T>(ReadOnlySpan<byte> buffer) where T : struct
        {
            return MemoryMarshal.Read<T>(buffer);
        }
    }
}