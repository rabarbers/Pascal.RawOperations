// © 2021 Contributors to the Pascal.RawOperations
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation - PIP-0027: https://www.pascalcoin.org/development/pips/pip-0027

using System;

namespace Pascal.RawOperations
{
    [Flags]
    public enum PayloadType: byte
    {
        NonDeterministic = 0,
        Public = 1,
        RecipientKeyEncrypted = 2,
        SenderKeyEncrypted = 4,
        PasswordEncrypted = 8,
        AsciiFormatted = 16,
        HexFormatted = 32,
        Base58Formatted = 64,
        AddressedByName = 128
    }
}
