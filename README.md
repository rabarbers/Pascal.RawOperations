# Pascal.RawOperations
.NET library to create PascalCoin rawOperations
Based on the source code of [PascalCoin](https://github.com/PascalCoin/PascalCoin) and [NPascalCoin](https://github.com/Sphere10/NPascalCoin). 
RawOperations documentation: https://github.com/rabarbers/Pascal.RawOperations/wiki
## Nuget package
https://www.nuget.org/packages/Pascal.RawOperations
### RawOperations demo
RawOperations can be executed using [pascal.wallet.connector](https://github.com/rabarbers/pascal.wallet.connector) - .NET5 library to call Pascal full node Wallet JSON RPC API methods.
```c#
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
            var connector = new PascalConnector("127.0.0.1", 4203);

            //Replace sender, receiver and other data as needed. Be careful and do not share your private keys with others! Private key provided for demo purposes, it does not contain real Pascals or Pascal Accounts!
            var sender = 32330U;
            var receiver = 32332U;
            var senderPrivateKey = "CA022000DC3778C0EA88CEF38EBB2E9A9E990FC37A65DCA7B3E547A028B39CE1805FA10D";
            var password = "Password";
            var amount = 0.0036M;
            var fee = 0.0001M;
            var payloadType = Payload.AesEncrypted; //Recommended to use predefined payloadType templates
            var message = "Hello world!";
            uint nOperation = 37; //this should be the sender (or signer if the signer is used) current account nOperations value that is stored on SafeBox. NOperations is mechanism to avoid double spending.

            var rawOperation = PascalHelper.CreateTransaction(sender, senderPrivateKey, nOperation, receiver, amount, fee, payloadType, message, password);
            var response = await connector.ExecuteOperationsAsync(rawOperation);
            if(response.Result != null)
            {
                Console.WriteLine("Operation executed successfully!");
            }
            else
            {
                Console.WriteLine(response.Error.Message);
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
