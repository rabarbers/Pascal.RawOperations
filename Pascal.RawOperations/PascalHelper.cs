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
        public static string CreateTransaction(uint sender, string senderPrivateKey, uint senderNOperations, uint receiver, decimal amount, decimal fee,
            PayloadType payloadType = PayloadType.NonDeterministic, string payload = null, string password = null)
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

        public static string CreateChangeKeyOperation(uint signer, uint target, string signerPrivateKey, string newEncodedPublicKey, uint signerNOperations, decimal fee,
            PayloadType payloadType = PayloadType.NonDeterministic, string payload = null, string password = null)
        {
            using var stream = new MemoryStream();

            stream.Write(BitConverter.GetBytes((uint)1)); //single operation
            stream.Write(BitConverter.GetBytes((ushort)OperationType.ChangeKeySignedByAnotherAccount));
            stream.Write(BitConverter.GetBytes((ushort)5)); //protocol

            stream.Write(BitConverter.GetBytes(signer));
            stream.Write(BitConverter.GetBytes(target));
            stream.Write(BitConverter.GetBytes(signerNOperations + 1));
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

            stream.Write(new byte[] { 0, 0, 0, 0, 0, 0, 70, 0 }); //8 bytes - constant

            var (newPubKeyNid, newPubKeyX, newPubKeyY) = CryptoHelper.GetPublicKeyInfo(newEncodedPublicKey);
            var x = Convert.FromHexString(newPubKeyX);
            var y = Convert.FromHexString(newPubKeyY);
            stream.Write(BitConverter.GetBytes((ushort)newPubKeyNid));
            stream.Write(BitConverter.GetBytes((ushort)x.Length));
            stream.Write(x);
            stream.Write(BitConverter.GetBytes((ushort)y.Length));
            stream.Write(y);

            var unsignedOperation = CreateUnsignedChangeKeyOperation(signer, target, signerNOperations, fee, payloadType, payloadBytes, newPubKeyNid, x, y);

            var hash = CryptoHelper.GetHash(unsignedOperation);
            var (signerKeyType, signerKey) = CryptoHelper.GetPrivateKeyInfo(signerPrivateKey);
            var (r, s) = CryptoHelper.SignOperation(signerKeyType, signerKey, hash);
            stream.Write(BitConverter.GetBytes((ushort)r.Length));
            stream.Write(r);
            stream.Write(BitConverter.GetBytes((ushort)s.Length));
            stream.Write(s);

            return Convert.ToHexString(stream.ToArray());
        }

        public static string CreateChangePasaInfoOperation(uint signer, uint target, string signerPrivateKey, uint signerNOperations, decimal fee, string newEncodedPublicKey, string newPasaName,
            ushort newPasaType, byte[] newPasaData, PayloadType payloadType = PayloadType.NonDeterministic, string payload = null, string password = null)
        {
            using var stream = new MemoryStream();

            stream.Write(BitConverter.GetBytes((uint)1)); //single operation
            stream.Write(BitConverter.GetBytes((ushort)OperationType.ChangeAccountInfo));
            stream.Write(BitConverter.GetBytes((ushort)5)); //protocol

            stream.Write(BitConverter.GetBytes(signer));
            stream.Write(BitConverter.GetBytes(target));
            stream.Write(BitConverter.GetBytes(signerNOperations + 1));
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

            //TODO...
            //public key
            //changeType: byte
            //newAccountKey
            //newName
            //newType
            //newData

            //stream.Write(new byte[] { 0, 0, 0, 0, 0, 0, 70, 0 }); //8 bytes - constant

            //var (newPubKeyNid, newPubKeyX, newPubKeyY) = CryptoHelper.GetPublicKeyInfo(newEncodedPublicKey);
            //var x = Convert.FromHexString(newPubKeyX);
            //var y = Convert.FromHexString(newPubKeyY);
            //stream.Write(BitConverter.GetBytes((ushort)newPubKeyNid));
            //stream.Write(BitConverter.GetBytes((ushort)x.Length));
            //stream.Write(x);
            //stream.Write(BitConverter.GetBytes((ushort)y.Length));
            //stream.Write(y);

            //var unsignedOperation = CreateUnsignedChangeKeyOperation(signer, target, signerNOperations, fee, payloadType, payloadBytes, newPubKeyNid, x, y);

            //var hash = CryptoHelper.GetHash(unsignedOperation);
            //var (signerKeyType, signerKey) = CryptoHelper.GetPrivateKeyInfo(signerPrivateKey);
            //var (r, s) = CryptoHelper.SignOperation(signerKeyType, signerKey, hash);
            //stream.Write(BitConverter.GetBytes((ushort)r.Length));
            //stream.Write(r);
            //stream.Write(BitConverter.GetBytes((ushort)s.Length));
            //stream.Write(s);

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
            (byte[], byte[], byte[]) GetPublicKeyChangingOperationSignature(string singleRawOperation)
            {
                var signer = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(16, 8)));
                var target = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(24, 8)));
                var signerNOperations = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(32, 8)));
                var fee = BitConverter.ToUInt32(Convert.FromHexString(singleRawOperation.Substring(40, 16))) / 10000M;
                var payloadType = (PayloadType)Convert.FromHexString(singleRawOperation.Substring(56, 2))[0];
                var payloadLength = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(58, 4)));
                var payloadBytes = Convert.FromHexString(singleRawOperation.Substring(62, payloadLength * 2));
                const byte unknownConstant = 16; //8 bytes (16 Hexadecimal symbols) unknown constant
                var nid = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(62 + payloadLength * 2 + unknownConstant, 4)));
                var xLength = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(66 + payloadLength * 2 + unknownConstant, 4)));
                var x = Convert.FromHexString(singleRawOperation.Substring(70 + payloadLength * 2 + unknownConstant, xLength * 2));
                var yLength = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(70 + payloadLength * 2 + unknownConstant + xLength * 2, 4)));
                var y = Convert.FromHexString(singleRawOperation.Substring(74 + payloadLength * 2 + unknownConstant + xLength * 2, yLength * 2));
                var unsignedOperation = CreateUnsignedChangeKeyOperation(signer, target, signerNOperations - 1, fee, payloadType, payloadBytes, (EncryptionType)nid, x, y);
                var signatureRLength = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(74 + payloadLength * 2 + unknownConstant + xLength * 2 + yLength * 2, 4)));
                var signatureR = Convert.FromHexString(singleRawOperation.Substring(78 + payloadLength * 2 + unknownConstant + xLength * 2 + yLength * 2, signatureRLength * 2));
                var signatureSLength = BitConverter.ToUInt16(Convert.FromHexString(singleRawOperation.Substring(78 + payloadLength * 2 + unknownConstant + xLength * 2 + yLength * 2 + signatureRLength * 2, 4)));
                var signatureS = Convert.FromHexString(singleRawOperation.Substring(82 + payloadLength * 2 + unknownConstant + xLength * 2 + yLength * 2 + signatureRLength * 2, signatureSLength * 2));
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
                case OperationType.ChangeKeySignedByAnotherAccount:
                    (unsignedOperation, signatureR, signatureS) = GetPublicKeyChangingOperationSignature(singleRawOperation);
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
            stream.Write(BitConverter.GetBytes((ushort)0)); //constant
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

        private static byte[] CreateUnsignedChangeKeyOperation(uint signer, uint target, uint signerNOperations, decimal fee, PayloadType payloadType, byte[] payloadBytes, EncryptionType nid, byte[] newKeyX, byte[] newKeyY)
        {
            using var stream = new MemoryStream();
            stream.Write(BitConverter.GetBytes(signer));
            if(signer != target)
            {
                stream.Write(BitConverter.GetBytes(target));
            }
            stream.Write(BitConverter.GetBytes(signerNOperations + 1));
            stream.Write(BitConverter.GetBytes((ulong)(fee * 10000)));
            
            stream.WriteByte((byte)payloadType);
            stream.Write(payloadBytes);
            stream.Write(BitConverter.GetBytes((ushort)0)); //magic constant
            stream.Write(BitConverter.GetBytes((ushort)nid));
            stream.Write(BitConverter.GetBytes((ushort)newKeyX.Length));
            stream.Write(newKeyX);
            stream.Write(BitConverter.GetBytes((ushort)newKeyY.Length));
            stream.Write(newKeyY);
            stream.WriteByte((byte)OperationType.ChangeKeySignedByAnotherAccount);
            return stream.ToArray();

            //TODO...
        }
    }
}
