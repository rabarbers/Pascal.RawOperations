// © 2021 Contributors to the Pascal.RawOperations
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace Pascal.RawOperations.Demo
{
    class Program
    {
        static async Task Main()
        {
            //Replace sender, receiver and other data as needed. Be careful and do not share your private keys with others! Private key provided for demo purposes, it does not contain real Pascals or Pascal Accounts!
            var nodeAddress = "192.168.88.240";
            var nodePort = 4004;
            var sender = 1141769U; //this account pays transaction fee
            var receiver = 796500U;
            var signerPrivateKey = "...";
            var amount = 0.0001M;
            var fee = 0.0001M;

            try
            {
                var accountInfo = await PascalNetwork.RequestAccountInfoAsync(nodeAddress, nodePort, sender);
                Console.WriteLine(accountInfo);
                
                var rawOperation = PascalHelper.CreateTransaction(sender, signerPrivateKey, accountInfo.NOperations, receiver, amount, fee);
                await PascalNetwork.SendOperationsAsync(nodeAddress, nodePort, rawOperation);
                Console.WriteLine($"\nSent operation: {rawOperation}\n");

                accountInfo = await PascalNetwork.RequestAccountInfoAsync(nodeAddress, nodePort, sender);
                Console.WriteLine(accountInfo);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
