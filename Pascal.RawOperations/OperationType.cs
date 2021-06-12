// © 2021 Contributors to the Pascal.RawOperations
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin

namespace Pascal.RawOperations
{
    public enum OperationType: byte
    {
        BlockchainReward,
        Transaction,
        ChangeKey,
        RecoverFounds,
        ListAccountForSale,
        DelistAccount,
        BuyAccount,
        ChangeKeySignedByAnotherAccount,
        ChangeAccountInfo,
        Multioperation,
        DataOperation
    }
}
