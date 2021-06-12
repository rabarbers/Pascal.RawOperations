// © 2021 Contributors to the Pascal.RawOperations
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.

namespace Pascal.RawOperations
{
    public static class Payload
    {
        public static PayloadType Public => PayloadType.Public ^ PayloadType.AsciiFormatted;
        public static PayloadType AesEncrypted => PayloadType.PasswordEncrypted ^ PayloadType.AsciiFormatted;
        public static PayloadType DestinationPublicKeyEncrypted => PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted;
        public static PayloadType SenderPublicKeyEncryptedPayload => PayloadType.SenderKeyEncrypted ^ PayloadType.AsciiFormatted;
    }
}
