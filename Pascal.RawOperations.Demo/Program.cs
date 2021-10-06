// © 2021 Contributors to the Pascal.RawOperations
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Pascal.Wallet.Connector;

namespace Pascal.RawOperations.Demo
{
    class Program
    {
        static async Task Main()
        {
            //Port 4203 is for testnet, MainNet port: 4003.
            var connector = new PascalConnector("127.0.0.1", 4003);

            //Replace sender, receiver and other data as needed. Be careful and do not share your private keys with others! Private key provided for demo purposes, it does not contain real Pascals or Pascal Accounts!
            var signer = 32330U; //this account pays transaction fee
            var target = 32330U; //this account is being transferred (changed public key). Can be different than signer only when the signer and the target PASA have the same public-private key pair.
            var signerPrivateKey = "CA022000DC3778C0EA88CEF38EBB2E9A9E990FC37A65DCA7B3E547A028B39CE1805FA10D";
            var newPublicKey = "CA0220000AC3D12F0260A85D674AFD8604B1311AC8EE26C927324A9E63AD262C68E7483520004F43B9C60084C453C37ECF9BED975F91DF34635AA9B5DE340B77370382E0FE59"; //in encoded format.
            var fee = 0M;
            uint nOperation = 19; //this should be the signer account nOperations value that is stored on SafeBox. NOperations is mechanism to avoid double spending.

            var rawOperation = PascalHelper.CreateChangeKeyOperation(signer, target, signerPrivateKey, newPublicKey, nOperation, fee);
            var response = await connector.ExecuteOperationsAsync(rawOperation);
            if(response.Result != null)
            {
                if(response.Result[0].Errors != null)
                {
                    Console.WriteLine(response.Result[0].Errors);
                }
                else
                {
                    Console.WriteLine("Operation executed successfully!");
                }
            }
            else
            {
                Console.WriteLine(response.Error.Message);
            }
        }
    }
}
