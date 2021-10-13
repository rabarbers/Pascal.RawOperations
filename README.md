# Pascal.RawOperations
.NET library to create PascalCoin rawOperations
Based on the source code of [PascalCoin](https://github.com/PascalCoin/PascalCoin) and [NPascalCoin](https://github.com/Sphere10/NPascalCoin). 
RawOperations documentation: https://github.com/PascalCoinDev/PascalCoin/wiki/Create-operations
## Nuget package
https://www.nuget.org/packages/Pascal.RawOperations
### RawOperations demo
RawOperations can be executed using [pascal.wallet.connector](https://github.com/rabarbers/pascal.wallet.connector) - .NET5 library to call Pascal full node Wallet JSON RPC API methods.
```c#
using System;
using System.Threading.Tasks;

namespace Pascal.RawOperations.Demo
{
    class Program
    {
        static async Task Main()
        {
            //Replace sender, receiver and other data as needed. 
            var nodeAddress = "192.168.88.240"; //change to the address of your Pascal wallet
            var nodePort = 4004;
            var sender = 1141769U; //this account pays transaction fee
            var receiver = 796500U;
            var signerPrivateKey = "..."; //Be careful and do not share your private keys with others!
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
``` 

### Supported features
* RawOperation creation for transactions
* RawOperation creation for data operations
* Supported payload methods: public and AES encrypted

### For technical support contact Rabarbers and be polite

## Roadmap
* Support payload encryption using DestinationPublicKey and SenderPublicKey methods.
* Ability to create rawOperations for other kinds of Pascal operations.

## Feedback & Donations
pascal.wallet.connector account 834853-50.
