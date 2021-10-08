using System;
using System.Text;

namespace Pascal.RawOperations
{
    public class AccountInfo
    {
        public uint CurrentBlockNumber { get; }
        public uint AccountNumber { get; }
        public decimal Balance { get; }
        public uint PassiveUpdateBlock { get; }
        public uint ActiveUpdateBlock { get; }
        public uint NOperations { get; }
        public string AccountName { get; }
        public ushort AccountType { get; }
        public byte[] AccountData { get; }
        public byte[] AccountSeal { get; }

        public AccountInfo(uint blockNumber, uint accountNumber, decimal balance, uint passiveUpdateBlock, uint activeUpdateBlock, uint nOperations, string accountName, ushort accountType, byte[] accountData, byte[] accountSeal)
        {
            CurrentBlockNumber = blockNumber;
            AccountNumber = accountNumber;
            Balance = balance;
            PassiveUpdateBlock = passiveUpdateBlock;
            ActiveUpdateBlock = activeUpdateBlock;
            NOperations = nOperations;
            AccountName = accountName;
            AccountType = accountType;
            AccountData = accountData;
            AccountSeal = accountSeal;
        }

        public override string ToString()
        {
            var readableAccountData = AccountData != null ? Encoding.UTF8.GetString(AccountData) : string.Empty;
            var readableAccountSeal = AccountSeal != null ? Convert.ToHexString(AccountSeal) : string.Empty;
            return $"Current block number: {CurrentBlockNumber}\nAccount number: {AccountNumber}\nBalance: {Balance}\nPassive update block: {PassiveUpdateBlock}\nActive update block: {ActiveUpdateBlock}\nN operations: {NOperations}\nAccount name: {AccountName}\nAccount type: {AccountType}\nAccount data: {readableAccountData}\nAccount seal: {readableAccountSeal}";
        }
    }
}
