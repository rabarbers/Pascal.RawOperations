using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Pascal.RawOperations
{
    public static class PascalNetwork
    {
        public const int MagicNetIdentification = 0x0A043580; //for MainNet
        public const ushort ProtocolVersion = 12;
        public const ushort ProtocolAvailable = 13;
        public const ushort MagicRequest = 1;
        public const ushort MagicResponse = 2;
        public const ushort MagicAutoSend = 3;
        public const ushort NetOpHello = 0x01;
        public const ushort NetOpAddOperations = 0x20;
        public const ushort NetOpGetAccount = 0x31;
        private const ushort NoError = 0;
        private const uint RequestId = 0;
        private const int HeaderSize = 22; //bytes

        public static async Task SendOperationsAsync(string nodeAddress, int port, string rawOperations)
        {
            using var client = new TcpClient(nodeAddress, port);
            using var memStream = new MemoryStream();
            memStream.Write(BitConverter.GetBytes(MagicNetIdentification));
            memStream.Write(BitConverter.GetBytes(MagicAutoSend));
            memStream.Write(BitConverter.GetBytes(NetOpAddOperations));
            memStream.Write(BitConverter.GetBytes(NoError));
            memStream.Write(BitConverter.GetBytes(RequestId));
            memStream.Write(BitConverter.GetBytes(ProtocolVersion));
            memStream.Write(BitConverter.GetBytes(ProtocolAvailable));
            memStream.Write(BitConverter.GetBytes(rawOperations.Length / 2));
            memStream.Write(Convert.FromHexString(rawOperations));

            var data = new byte[(int)memStream.Length];
            memStream.Seek(0, SeekOrigin.Begin);
            memStream.Read(data, 0, data.Length);

            using var stream = client.GetStream();
            await stream.WriteAsync(data);
        }

        public static async Task<AccountInfo> RequestAccountInfoAsync(string nodeAddress, int port, uint account)
        {
            using var client = new TcpClient(nodeAddress, port);
            using var memStream = new MemoryStream();
            memStream.Write(BitConverter.GetBytes(MagicNetIdentification));
            memStream.Write(BitConverter.GetBytes(MagicRequest));
            memStream.Write(BitConverter.GetBytes(NetOpGetAccount));
            memStream.Write(BitConverter.GetBytes(NoError));
            memStream.Write(BitConverter.GetBytes(RequestId));
            memStream.Write(BitConverter.GetBytes(ProtocolVersion));
            memStream.Write(BitConverter.GetBytes(ProtocolAvailable));
            memStream.Write(BitConverter.GetBytes((uint)5));
            memStream.Write(new byte[] { 1 });
            memStream.Write(BitConverter.GetBytes(account));
            
            var data = new byte[(int)memStream.Length];
            memStream.Seek(0, SeekOrigin.Begin);
            memStream.Read(data, 0, data.Length);

            using var stream = client.GetStream();
            await stream.WriteAsync(data);

            var responseHeader = new byte[HeaderSize];
            var bytesRead = stream.Read(responseHeader, 0, HeaderSize);
            if(bytesRead != HeaderSize)
            {
                throw new Exception($"Invalid response data: {Convert.ToHexString(responseHeader)}");
            }

            var magicId = BitConverter.ToUInt32(responseHeader, 0);
            if (magicId != MagicNetIdentification)
            {
                throw new Exception($"Invalid response data: {Convert.ToHexString(responseHeader)}");
            }
            var messageType = BitConverter.ToUInt16(responseHeader, 4);
            var operationType = BitConverter.ToUInt16(responseHeader, 6);
            var requestId = BitConverter.ToUInt32(responseHeader, 10);
            var ver = BitConverter.ToUInt16(responseHeader, 14);
            var verA = BitConverter.ToUInt16(responseHeader, 16);
            var dataLength = BitConverter.ToUInt32(responseHeader, 18);

            var responseData = new byte[dataLength];
            bytesRead = await stream.ReadAsync(responseData, 0, responseData.Length);
            if (bytesRead != dataLength)
            {
                throw new Exception($"Expected response length: {dataLength} bytes, but received: {bytesRead} bytes.");
            }

            var blockNumber = BitConverter.ToUInt32(responseData, 0);
            var accountCount = BitConverter.ToUInt32(responseData, 4);
            var version = BitConverter.ToUInt16(responseData, 8);
            var accountNumber = BitConverter.ToUInt32(responseData, 10);
            var accountInfoSize = BitConverter.ToUInt16(responseData, 14);
            //var accountInfo = ... TODO...
            var balance = BitConverter.ToUInt32(responseData, 16 + accountInfoSize) / 10000M;
            var passiveUpdateBlock = BitConverter.ToUInt32(responseData, 24 + accountInfoSize);
            var activeUpdateBlock = BitConverter.ToUInt32(responseData, 28 + accountInfoSize);
            var nOperations = BitConverter.ToUInt32(responseData, 32 + accountInfoSize);
            var accountNameSize = BitConverter.ToUInt16(responseData, 36 + accountInfoSize);
            var accountName = Encoding.UTF8.GetString(responseData, 38 + accountInfoSize, accountNameSize);
            var accountType = BitConverter.ToUInt16(responseData, 38 + accountInfoSize + accountNameSize);
            var accountDataSize = BitConverter.ToUInt16(responseData, 40 + accountInfoSize + accountNameSize);
            //var accountData = TODO...
            var accountSealSize = BitConverter.ToUInt16(responseData, 42 + accountInfoSize + accountNameSize + accountDataSize);
            //var accountSeal = ...TODO...

            return new AccountInfo(blockNumber, accountNumber, balance, passiveUpdateBlock, activeUpdateBlock, nOperations, accountName, accountType, null, null);
        }
    }
}
