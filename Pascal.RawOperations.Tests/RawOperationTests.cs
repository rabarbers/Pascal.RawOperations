// © 2021 Contributors to the Pascal.RawOperations
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace Pascal.RawOperations.Tests
{
    public class RawOperationTests
    {
        [Fact]
        public void CreateTransactionSecp256k1()
        {
            var sender = 32330U;
            var receiver = 32332U;
            var senderPrivateKey = "CA022000DC3778C0EA88CEF38EBB2E9A9E990FC37A65DCA7B3E547A028B39CE1805FA10D";
            var senderPublicKey = "CA02200070ED16DC191D90E6153CF6BD622DBC68E0E135FC7D4DE4F92D2C192947237D732000E6FDBA6A09282BB189FABAE27028DBD85E4A86A7065DF7CFF87122B74A1C86D0";
            var amount = 1M;
            var fee = 0.0001M;
            var payloadType = Payload.Public | PayloadType.AsciiFormatted;
            var message = "test";
            uint nOperation = 16;

            var rawOperation = PascalHelper.CreateTransaction(sender, senderPrivateKey, nOperation, receiver, amount, fee, payloadType, message);

            Assert.True(PascalHelper.VerifyRawOperation(senderPublicKey, rawOperation));
        }

        [Fact]
        public void CreateTransactionSecp384r1()
        {
            var sender = 32330U;
            var receiver = 32332U;
            var senderPrivateKey = "CB0230003FF4E7A997EBA57B32AC0F01F14D607B1BB345268C3AC576D012F688AD84F1024F45045EC2E89128ED2AD6C4A9FB50E7";
            var senderPublicKey = "CB023000DE9EB043C044D31166B413D688E29D842F3C9974B0B4E5DF2F3A434EB724777531DFF2209C047C547FD0DCE6AB9E812530002229747432913302423DC26AA9B18E265A3329A9203E780772D89A67B68F44029AE93D93328D5A366ABF4A70EC248C47";
            var amount = 1M;
            var fee = 0.0001M;
            var payloadType = Payload.Public | PayloadType.AsciiFormatted;
            var message = "test";
            uint nOperation = 16;

            var rawOperation = PascalHelper.CreateTransaction(sender, senderPrivateKey, nOperation, receiver, amount, fee, payloadType, message);

            Assert.True(PascalHelper.VerifyRawOperation(senderPublicKey, rawOperation));
        }

        [Fact]
        public void CreateTransactionSecp521r1()
        {
            var sender = 32330U;
            var receiver = 32332U;
            var senderPrivateKey = "CC02420001D9B58F28CC5F5C728690C712FEF69B07FDEFF32AA8CD5177B4989FE21034352EE608AFEF272799F2A2D6AD0CC26FDD88B2DB92AC24291304004CCCD6AD0D84A2D8";
            var senderPublicKey = "CC0242000161E63760B153A5415B9377EA0F103CE63D61042ECFBDF7D41A8705A4F5AC650CC08D53BBE46DF2DA0EBD3CC114940584BA915BDDD34CF6AF23EDFFBF79BDF178FB4100CE50DD04F500EC81A706BDB442CF4E87821EA6FC4E532DC37C34ECDFADF6C59949BF7F4A7B679E755749D07EBEAE7630898FBDEA3AB5D2F69440AD31FF45D6FF71";
            var amount = 1M;
            var fee = 0.0001M;
            var payloadType = Payload.Public | PayloadType.AsciiFormatted;
            var message = "test";
            uint nOperation = 16;

            var rawOperation = PascalHelper.CreateTransaction(sender, senderPrivateKey, nOperation, receiver, amount, fee, payloadType, message);

            Assert.True(PascalHelper.VerifyRawOperation(senderPublicKey, rawOperation));
        }

        [Fact]
        public void CreateTransactionSect283k1()
        {
            var sender = 32330U;
            var receiver = 32332U;
            var senderPrivateKey = "D90223000EDA0BDDC8FAEE5CD40B3A89E0C666C7A3F9D55DEE585A0D95DE3A8586620ED4FBD968";
            var senderPublicKey = "D9022400031B60A3811C2A71EF3F5E36267C1EDA386A1CAE311309782CC718998521FB0E2FEB515A240003EA679C8064600BAF95DAC31467F892863FB078D10D7BD90DEE318423CDE02AB36B4126";
            var amount = 1M;
            var fee = 0.0001M;
            var payloadType = Payload.Public | PayloadType.AsciiFormatted;
            var message = "test";
            uint nOperation = 16;

            var rawOperation = PascalHelper.CreateTransaction(sender, senderPrivateKey, nOperation, receiver, amount, fee, payloadType, message);

            Assert.True(PascalHelper.VerifyRawOperation(senderPublicKey, rawOperation));
        }

        [Fact]
        public void CreateTransactionSecp256k1AesEncrypted()
        {
            var sender = 32330U;
            var receiver = 32332U;
            var senderPrivateKey = "CA022000DC3778C0EA88CEF38EBB2E9A9E990FC37A65DCA7B3E547A028B39CE1805FA10D";
            var senderPublicKey = "CA02200070ED16DC191D90E6153CF6BD622DBC68E0E135FC7D4DE4F92D2C192947237D732000E6FDBA6A09282BB189FABAE27028DBD85E4A86A7065DF7CFF87122B74A1C86D0";
            var password = "Password";
            var amount = 1M;
            var fee = 0.0001M;
            var payloadType = Payload.AesEncrypted | PayloadType.AsciiFormatted;
            var message = "test";
            uint nOperation = 16;

            var rawOperation = PascalHelper.CreateTransaction(sender, senderPrivateKey, nOperation, receiver, amount, fee, payloadType, message, password);

            Assert.True(PascalHelper.VerifyRawOperation(senderPublicKey, rawOperation));
        }

        [Fact]
        public void CreateDataOperationSecp256k1()
        {
            var signer = 32330U;
            var sender = 32331U;
            var receiver = 32332U;
            var senderPrivateKey = "CA022000DC3778C0EA88CEF38EBB2E9A9E990FC37A65DCA7B3E547A028B39CE1805FA10D";
            var senderPublicKey = "CA02200070ED16DC191D90E6153CF6BD622DBC68E0E135FC7D4DE4F92D2C192947237D732000E6FDBA6A09282BB189FABAE27028DBD85E4A86A7065DF7CFF87122B74A1C86D0";
            var amount = 1M;
            var fee = 0.0001M;
            var payloadType = Payload.Public | PayloadType.AsciiFormatted;
            var message = "test";
            uint nOperation = 16;
            var guid = Guid.Parse("f862b6a6-8024-420d-b492-f5ad5991ad0b");
            ushort dataType = 1;
            ushort sequence = 2;

            var rawOperation = PascalHelper.CreateDataOperation(signer, sender, senderPrivateKey, nOperation, receiver, amount, fee, payloadType, message, guid, dataType, sequence);

            Assert.True(PascalHelper.VerifyRawOperation(senderPublicKey, rawOperation));
        }

        [Fact]
        public void CreateDataOperationSecp384r1()
        {
            var signer = 32330U;
            var sender = 32331U;
            var receiver = 32332U;
            var senderPrivateKey = "CB0230003FF4E7A997EBA57B32AC0F01F14D607B1BB345268C3AC576D012F688AD84F1024F45045EC2E89128ED2AD6C4A9FB50E7";
            var senderPublicKey = "CB023000DE9EB043C044D31166B413D688E29D842F3C9974B0B4E5DF2F3A434EB724777531DFF2209C047C547FD0DCE6AB9E812530002229747432913302423DC26AA9B18E265A3329A9203E780772D89A67B68F44029AE93D93328D5A366ABF4A70EC248C47";
            var amount = 1M;
            var fee = 0.0001M;
            var payloadType = Payload.Public | PayloadType.AsciiFormatted;
            var message = "test";
            uint nOperation = 16;
            var guid = Guid.Parse("f862b6a6-8024-420d-b492-f5ad5991ad0b");
            ushort dataType = 1;
            ushort sequence = 2;

            var rawOperation = PascalHelper.CreateDataOperation(signer, sender, senderPrivateKey, nOperation, receiver, amount, fee, payloadType, message, guid, dataType, sequence);

            Assert.True(PascalHelper.VerifyRawOperation(senderPublicKey, rawOperation));
        }

        [Fact]
        public void CreateDataOperationSecp521r1()
        {
            var signer = 32330U;
            var sender = 32331U;
            var receiver = 32332U;
            var senderPrivateKey = "CC02420001D9B58F28CC5F5C728690C712FEF69B07FDEFF32AA8CD5177B4989FE21034352EE608AFEF272799F2A2D6AD0CC26FDD88B2DB92AC24291304004CCCD6AD0D84A2D8";
            var senderPublicKey = "CC0242000161E63760B153A5415B9377EA0F103CE63D61042ECFBDF7D41A8705A4F5AC650CC08D53BBE46DF2DA0EBD3CC114940584BA915BDDD34CF6AF23EDFFBF79BDF178FB4100CE50DD04F500EC81A706BDB442CF4E87821EA6FC4E532DC37C34ECDFADF6C59949BF7F4A7B679E755749D07EBEAE7630898FBDEA3AB5D2F69440AD31FF45D6FF71";
            var amount = 1M;
            var fee = 0.0001M;
            var payloadType = Payload.Public | PayloadType.AsciiFormatted;
            var message = "test";
            uint nOperation = 16;
            var guid = Guid.Parse("f862b6a6-8024-420d-b492-f5ad5991ad0b");
            ushort dataType = 1;
            ushort sequence = 2;

            var rawOperation = PascalHelper.CreateDataOperation(signer, sender, senderPrivateKey, nOperation, receiver, amount, fee, payloadType, message, guid, dataType, sequence);

            Assert.True(PascalHelper.VerifyRawOperation(senderPublicKey, rawOperation));
        }

        [Fact]
        public void CreateDataOperationSect283k1()
        {
            var signer = 32330U;
            var sender = 32331U;
            var receiver = 32332U;
            var senderPrivateKey = "D90223000EDA0BDDC8FAEE5CD40B3A89E0C666C7A3F9D55DEE585A0D95DE3A8586620ED4FBD968";
            var senderPublicKey = "D9022400031B60A3811C2A71EF3F5E36267C1EDA386A1CAE311309782CC718998521FB0E2FEB515A240003EA679C8064600BAF95DAC31467F892863FB078D10D7BD90DEE318423CDE02AB36B4126";
            var amount = 1M;
            var fee = 0.0001M;
            var payloadType = Payload.Public | PayloadType.AsciiFormatted;
            var message = "test";
            uint nOperation = 16;
            var guid = Guid.Parse("f862b6a6-8024-420d-b492-f5ad5991ad0b");
            ushort dataType = 1;
            ushort sequence = 2;

            var rawOperation = PascalHelper.CreateDataOperation(signer, sender, senderPrivateKey, nOperation, receiver, amount, fee, payloadType, message, guid, dataType, sequence);

            Assert.True(PascalHelper.VerifyRawOperation(senderPublicKey, rawOperation));
        }

        [Fact]
        public void CreateChangeKeyOperationSecp256k1()
        {
            var signer = 32330U;
            var target = 32332U;
            var signerPrivateKey = "CA022000DC3778C0EA88CEF38EBB2E9A9E990FC37A65DCA7B3E547A028B39CE1805FA10D";
            var signerPublicKey = "CA02200070ED16DC191D90E6153CF6BD622DBC68E0E135FC7D4DE4F92D2C192947237D732000E6FDBA6A09282BB189FABAE27028DBD85E4A86A7065DF7CFF87122B74A1C86D0";
            var newPublicKey = "CA0220000AC3D12F0260A85D674AFD8604B1311AC8EE26C927324A9E63AD262C68E7483520004F43B9C60084C453C37ECF9BED975F91DF34635AA9B5DE340B77370382E0FE59";
            var fee = 0.0001M;
            uint nOperation = 16;

            var rawOperation = PascalHelper.CreateChangeKeyOperation(signer, target, signerPrivateKey, newPublicKey, nOperation, fee);

            Assert.True(PascalHelper.VerifyRawOperation(signerPublicKey, rawOperation));
        }

        [Fact]
        public void CreateChangeKeyOperationSecp256k1WithPublicPayload()
        {
            var signer = 32330U;
            var target = 32332U;
            var signerPrivateKey = "CA022000DC3778C0EA88CEF38EBB2E9A9E990FC37A65DCA7B3E547A028B39CE1805FA10D";
            var signerPublicKey = "CA02200070ED16DC191D90E6153CF6BD622DBC68E0E135FC7D4DE4F92D2C192947237D732000E6FDBA6A09282BB189FABAE27028DBD85E4A86A7065DF7CFF87122B74A1C86D0";
            var newPublicKey = "CA0220000AC3D12F0260A85D674AFD8604B1311AC8EE26C927324A9E63AD262C68E7483520004F43B9C60084C453C37ECF9BED975F91DF34635AA9B5DE340B77370382E0FE59";
            var fee = 0.0001M;
            uint nOperation = 16;

            var rawOperation = PascalHelper.CreateChangeKeyOperation(signer, target, signerPrivateKey, newPublicKey, nOperation, fee, Payload.Public | PayloadType.AsciiFormatted, "test");

            Assert.True(PascalHelper.VerifyRawOperation(signerPublicKey, rawOperation));
        }
    }
}
