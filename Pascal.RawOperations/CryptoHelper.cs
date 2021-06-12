// © 2021 Contributors to the Pascal.RawOperations
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Signing and AES encryption based on source code of PascalCoin https://github.com/PascalCoin/PascalCoin and NPascalCoin: https://github.com/Sphere10/NPascalCoin projects.

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Pascal.RawOperations
{
    public static class CryptoHelper
    {
        private static Dictionary<EncryptionType, Tuple<X9ECParameters, ECDomainParameters>> _domainParameters = new Dictionary<EncryptionType, Tuple<X9ECParameters, ECDomainParameters>>();

        private static SecureRandom _random;
        public static SecureRandom Random
        {
            get
            {
                if (_random == null)
                {
                    _random = new SecureRandom();
                }
                return _random;
            }
        }

        public static (byte[], byte[]) SignOperation(EncryptionType encryptionType, string privateKey, byte[] message)
        {
            var (curve, domain) = GetDomainParameters(encryptionType);
            var signer = new ECDsaSigner();
            signer.Init(true, new ECPrivateKeyParameters("ECDSA", new BigInteger(privateKey, 16), domain));
            var signerResult = signer.GenerateSignature(message);
            return (signerResult[0].ToByteArray(), signerResult[1].ToByteArray());
        }

        public static bool Verify(string senderEncodedPublicKey, byte[] message, byte[] signatureR, byte[] signatureS)
        {
            var (encryptionType, x, y) = GetPublicKeyInfo(senderEncodedPublicKey);

            var (curve, domain) = GetDomainParameters(encryptionType);
            var ecPoint = curve.Curve.CreatePoint(new BigInteger(x, 16), new BigInteger(y, 16));
            var pubKeyParams = new ECPublicKeyParameters("ECDSA", ecPoint, domain);

            var ecDsaSigner = new ECDsaSigner();
            ecDsaSigner.Init(false, pubKeyParams);
            return ecDsaSigner.VerifySignature(GetHash(message), new BigInteger(signatureR), new BigInteger(signatureS));
        }

        public static byte[] GetHash(byte[] message)
        {
            var hasher = new SHA256Managed();
            return hasher.ComputeHash(message);
        }

        public static (EncryptionType, string) GetPrivateKeyInfo(string encodedPrivateKey)
        {
            if (encodedPrivateKey != null && encodedPrivateKey.Length > 8)
            {
                var keyType = (EncryptionType)BitConverter.ToUInt16(Convert.FromHexString(encodedPrivateKey.Substring(0, 4)));
                var keySize = BitConverter.ToUInt16(Convert.FromHexString(encodedPrivateKey.Substring(4, 4)));

                var key = encodedPrivateKey.Substring(8);
                if (key.Length == keySize * 2)
                {
                    return (keyType, key);
                }
            }
            return (0, string.Empty);
        }

        public static (EncryptionType, string, string) GetPublicKeyInfo(string encodedPublicKey)
        {
            if (encodedPublicKey != null && encodedPublicKey.Length > 12)
            {
                var keyType = (EncryptionType)BitConverter.ToUInt16(Convert.FromHexString(encodedPublicKey.Substring(0, 4)));
                var xSize = BitConverter.ToUInt16(Convert.FromHexString(encodedPublicKey.Substring(4, 4)));
                if (encodedPublicKey.Length > 12 + xSize * 2)
                {
                    var x = encodedPublicKey.Substring(8, xSize * 2);
                    var ySize = BitConverter.ToUInt16(Convert.FromHexString(encodedPublicKey.Substring(8 + xSize * 2, 4)));
                    var y = encodedPublicKey.Substring(12 + xSize * 2);
                    if (y.Length == ySize * 2)
                    {
                        return (keyType, x, y);
                    }
                }
            }
            return (0, string.Empty, string.Empty);
        }

        public static byte[] AesEncrypt(string message, string password)
        {
            const int PKCS5_SALT_LEN = 8;
            const string SALT_MAGIC = "Salted__";

            var saltBytes = new byte[PKCS5_SALT_LEN];
            Random.NextBytes(saltBytes);

            var (key, iv) = GetKeyIV(Encoding.UTF8.GetBytes(password), saltBytes);
            var encrypted = AesEncrypt(message, key, iv);

            using var stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes(SALT_MAGIC));
            stream.Write(saltBytes);
            stream.Write(encrypted);
            return stream.ToArray();
        }

        private static (X9ECParameters, ECDomainParameters) GetDomainParameters(EncryptionType encryptionType)
        {
            if (_domainParameters.TryGetValue(encryptionType, out var parameters))
            {
                return (parameters.Item1, parameters.Item2);
            }
            var curve = CustomNamedCurves.GetByName(encryptionType.ToString().ToUpper());
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            _domainParameters.Add(encryptionType, Tuple.Create(curve, domain));
            return (curve, domain);
        }

        private static (byte[], byte[]) GetKeyIV(byte[] password, byte[] salt)
        {
            var key = new byte[32];
            var iv = new byte[32];
            var digest = DigestUtilities.GetDigest("SHA-256");
            digest.BlockUpdate(password, 0, password.Length);
            digest.BlockUpdate(salt, 0, salt.Length);
            digest.DoFinal(key, 0);
            digest.Reset();
            digest.BlockUpdate(key, 0, key.Length);
            digest.BlockUpdate(password, 0, password.Length);
            digest.BlockUpdate(salt, 0, salt.Length);
            digest.DoFinal(iv, 0);
            Array.Resize(ref iv, 16);
            return (key, iv);
        }

        private static byte[] AesEncrypt(string plainText, byte[] key, byte[] iv)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
            return msEncrypt.ToArray();
        }

        private static string AesDecrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var msDecrypt = new MemoryStream(cipherText);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}
