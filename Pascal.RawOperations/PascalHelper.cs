// © 2021 Contributors to the Pascal.RawOperations
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of PascalCoin https://github.com/PascalCoin/PascalCoin
// Documentation: https://github.com/PascalCoin/PascalCoin/wiki/Create-operations

using System;
using System.IO;
using System.Text;

namespace Pascal.RawOperations
{
    public class PascalHelper
    {
        public static string CreateTransaction(uint sender, string senderPrivateKey, uint senderNOperations, uint receiver, decimal amount, decimal fee, PayloadType payloadType, string payload, string password = null)
        {
            using var stream = new MemoryStream();

            stream.Write(BitConverter.GetBytes((uint)1)); //single operation
            stream.Write(BitConverter.GetBytes((ushort)OperationType.Transaction));
            stream.Write(BitConverter.GetBytes((ushort)5)); //protocol

            stream.Write(BitConverter.GetBytes(sender));
            stream.Write(BitConverter.GetBytes(senderNOperations + 1));
            stream.Write(BitConverter.GetBytes(receiver));
            stream.Write(BitConverter.GetBytes((ulong)(amount * 10000)));
            stream.Write(BitConverter.GetBytes((ulong)(fee * 10000)));

            stream.WriteByte((byte)payloadType);
            byte[] payloadBytes;
            switch (payloadType)
            {
                case PayloadType.Public | PayloadType.AsciiFormatted:
                    payloadBytes = Encoding.UTF8.GetBytes(payload);
                    break;
                case PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted:
                    payloadBytes = CryptoHelper.AesEncrypt(payload, password);
                    break;
                case PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted:
                    throw new NotImplementedException();
                case PayloadType.SenderKeyEncrypted | PayloadType.AsciiFormatted:
                    throw new NotImplementedException();
                default:
                    payloadBytes = new byte[0];
                    break;
            }
            stream.Write(BitConverter.GetBytes((ushort)payloadBytes.Length));
            stream.Write(payloadBytes);

            stream.Write(new byte[] { 0, 0, 0, 0, 0, 0 }); //6 bytes - constant

            var unsignedOperation = CreateUnsignedTransaction(sender, senderNOperations, receiver, amount, fee, payloadType, payloadBytes);

            var hash = CryptoHelper.GetHash(unsignedOperation);

            var (senderKeyType, senderKey) = CryptoHelper.GetPrivateKeyInfo(senderPrivateKey);
            var (r, s) = CryptoHelper.SignOperation(senderKeyType, senderKey, hash);
            stream.Write(BitConverter.GetBytes((ushort)r.Length));
            stream.Write(r);
            stream.Write(BitConverter.GetBytes((ushort)s.Length));
            stream.Write(s);

            return Convert.ToHexString(stream.ToArray());
        }

        public static string CreateDataOperation(uint signer, uint sender, string senderPrivateKey, uint signerNOperations, uint receiver, decimal amount, decimal fee,
            PayloadType payloadType, string payload, Guid guid, ushort dataType, ushort sequence, string password = null)
        {
            using var stream = new MemoryStream();

            stream.Write(BitConverter.GetBytes((uint)1)); //single operation

            stream.Write(BitConverter.GetBytes((ushort)OperationType.DataOperation));
            stream.Write(BitConverter.GetBytes((ushort)5)); //protocol


            stream.Write(BitConverter.GetBytes(signer));
            stream.Write(BitConverter.GetBytes(sender));
            stream.Write(BitConverter.GetBytes(receiver));
            stream.Write(BitConverter.GetBytes(signerNOperations + 1));

            stream.Write(guid.ToByteArray());

            stream.Write(BitConverter.GetBytes(dataType));
            stream.Write(BitConverter.GetBytes(sequence));

            stream.Write(BitConverter.GetBytes((ulong)(amount * 10000)));
            stream.Write(BitConverter.GetBytes((ulong)(fee * 10000)));

            stream.WriteByte((byte)payloadType);
            byte[] payloadBytes;
            switch (payloadType)
            {
                case PayloadType.Public | PayloadType.AsciiFormatted:
                    payloadBytes = Encoding.UTF8.GetBytes(payload);
                    break;
                case PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted:
                    payloadBytes = CryptoHelper.AesEncrypt(payload, password);
                    break;
                case PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted:
                    throw new NotImplementedException();
                case PayloadType.SenderKeyEncrypted | PayloadType.AsciiFormatted:
                    throw new NotImplementedException();
                default:
                    payloadBytes = new byte[0];
                    break;
            }

            stream.Write(BitConverter.GetBytes((ushort)payloadBytes.Length));
            stream.Write(payloadBytes);

            var unsignedOperation = CreateUnsignedDataOperation(signer, signerNOperations, sender, receiver, guid.ToByteArray(), dataType, sequence, amount, fee, payloadType, payloadBytes);

            var test = Convert.ToHexString(unsignedOperation);

            var hash = CryptoHelper.GetHash(unsignedOperation);

            var (senderKeyType, senderKey) = CryptoHelper.GetPrivateKeyInfo(senderPrivateKey);
            var (r, s) = CryptoHelper.SignOperation(senderKeyType, senderKey, hash);
            stream.Write(BitConverter.GetBytes((ushort)r.Length));
            stream.Write(r);
            stream.Write(BitConverter.GetBytes((ushort)s.Length));
            stream.Write(s);

            return Convert.ToHexString(stream.ToArray());
        }

        public static bool VerifyRawOperation(string senderEncodedPublicKey, string singleRawOperation)
        {
            (byte[], byte[], byte[]) GetTransactionSignature(string singleRawOperation)
            {
                var sender = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(16, 8)));
                var signerNOperations = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(24, 8)));
                var receiver = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(32, 8)));
                var amount = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(40, 16))) / 10000M;
                var fee = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(56, 16))) / 10000M;
                var payloadType = (PayloadType)Convert.FromHexString(singleRawOperation.Substring(72, 2))[0];
                var payloadLength = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(74, 4)));
                var payloadBytes = Convert.FromHexString(singleRawOperation.Substring(78, payloadLength * 2));
                var unsignedOperation = CreateUnsignedTransaction(sender, signerNOperations - 1, receiver, amount, fee, payloadType, payloadBytes);
                var signatureRLength = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(90 + payloadLength * 2, 4)));
                var signatureR = Convert.FromHexString(singleRawOperation.Substring(94 + payloadLength * 2, signatureRLength * 2));
                var signatureSLength = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(94 + payloadLength * 2 + signatureRLength * 2, 4)));
                var signatureS = Convert.FromHexString(singleRawOperation.Substring(98 + payloadLength * 2 + signatureRLength * 2, signatureSLength * 2));
                return (unsignedOperation, signatureR, signatureS);
            }
            (byte[], byte[], byte[]) GetDataOperationSignature(string singleRawOperation)
            {
                var signer = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(16, 8)));
                var sender = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(24, 8)));
                var receiver = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(32, 8)));
                var signerNOperations = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(40, 8)));
                var guid = Convert.FromHexString(singleRawOperation.Substring(48, 32));
                var datatype = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(80, 4)));
                var sequence = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(84, 4)));
                var amount = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(88, 16))) / 10000M;
                var fee = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(104, 16))) / 10000M;
                var payloadType = (PayloadType)Convert.FromHexString(singleRawOperation.Substring(120, 2))[0];
                var payloadLength = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(122, 4)));
                var payloadBytes = Convert.FromHexString(singleRawOperation.Substring(126, payloadLength * 2));
                var unsignedOperation = CreateUnsignedDataOperation(signer, signerNOperations - 1, sender, receiver, guid, datatype, sequence, amount, fee, payloadType, payloadBytes);
                var signatureRLength = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(126 + payloadLength * 2, 4)));
                var signatureR = Convert.FromHexString(singleRawOperation.Substring(130 + payloadLength * 2, signatureRLength * 2));
                var signatureSLength = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(130 + payloadLength * 2 + signatureRLength * 2, 4)));
                var signatureS = Convert.FromHexString(singleRawOperation.Substring(134 + payloadLength * 2 + signatureRLength * 2, signatureSLength * 2));
                return (unsignedOperation, signatureR, signatureS);
            }


            byte[] unsignedOperation = null;
            byte[] signatureR = null;
            byte[] signatureS = null;

            var operationType = (OperationType)BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(8, 4)));
            switch (operationType)
            {
                case OperationType.Transaction:
                    (unsignedOperation, signatureR, signatureS) = GetTransactionSignature(singleRawOperation);
                    break;
                case OperationType.DataOperation:
                    (unsignedOperation, signatureR, signatureS) = GetDataOperationSignature(singleRawOperation);
                    break;
            }

            return CryptoHelper.Verify(senderEncodedPublicKey, unsignedOperation, signatureR, signatureS);
        }

        private static byte[] CreateUnsignedTransaction(uint sender, uint senderNOperations, uint receiver, decimal amount, decimal fee, PayloadType payloadType, byte[] payloadBytes)
        {
            using var stream = new MemoryStream();
            stream.Write(BitConverter.GetBytes(sender));
            stream.Write(BitConverter.GetBytes(senderNOperations + 1));
            stream.Write(BitConverter.GetBytes(receiver));
            stream.Write(BitConverter.GetBytes((ulong)(amount * 10000)));
            stream.Write(BitConverter.GetBytes((ulong)(fee * 10000)));
            stream.WriteByte((byte)payloadType);
            stream.Write(payloadBytes);
            stream.Write(BitConverter.GetBytes((ushort)0));
            stream.WriteByte((byte)OperationType.Transaction);
            return stream.ToArray();
        }

        private static byte[] CreateUnsignedDataOperation(uint signer, uint signerNOperations, uint sender, uint receiver, byte[] guid, ushort dataType, ushort sequence, decimal amount, decimal fee, PayloadType payloadType, byte[] payloadBytes)
        {
            using var stream = new MemoryStream();
            stream.Write(BitConverter.GetBytes(signer));
            stream.Write(BitConverter.GetBytes(sender));
            stream.Write(BitConverter.GetBytes(receiver));
            stream.Write(BitConverter.GetBytes(signerNOperations + 1));
            stream.Write(guid);
            stream.Write(BitConverter.GetBytes(dataType));
            stream.Write(BitConverter.GetBytes(sequence));
            stream.Write(BitConverter.GetBytes((ulong)(amount * 10000)));
            stream.Write(BitConverter.GetBytes((ulong)(fee * 10000)));
            stream.WriteByte((byte)payloadType);
            stream.Write(BitConverter.GetBytes((ushort)payloadBytes.Length));
            stream.Write(payloadBytes);
            stream.WriteByte((byte)OperationType.DataOperation);
            return stream.ToArray();
        }
    }
}
