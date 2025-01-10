using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Snap.Hutao.Test.IncomingFeature;

[TestClass]
public sealed class GeniusInvokationDecoding
{
    public TestContext? TestContext { get; set; }

    /// <summary>
    /// https://www.bilibili.com/video/av278125720
    /// </summary>
    [TestMethod]
    public unsafe void GeniusInvokationShareCodeDecoding()
    {
        // 51 bytes obfuscated data
        byte[] bytes = Convert.FromBase64String("BCHBwxQNAYERyVANCJGBynkOCZER2pgOCrFx8poQChGR9bYQDEGB9rkQDFKRD7oRDeEB");

        // ---------------------------------------------
        // |   Data   | Caesar Cipher Key |
        // |----------|-------------------|
        // | 50 Bytes |       1 Byte      |
        // ---------------------------------------------
        // Data:
        // 00000100 00100001 11000001 11000011 00010100
        // 00001101 00000001 10000001 00010001 11001001
        // 01010000 00001101 00001000 10010001 10000001
        // 11001010 01111001 00001110 00001001 10010001
        // 00010001 11011010 10011000 00001110 00001010
        // 10110001 01110001 11110010 10011010 00010000
        // 00001010 00010001 10010001 11110101 10110110
        // 00010000 00001100 01000001 10000001 11110110
        // 10111001 00010000 00001100 01010010 10010001
        // 00001111 10111010 00010001 00001101 11100001
        // ---------------------------------------------
        // Caesar Cipher Key:
        // 00000001
        // ---------------------------------------------
        fixed (byte* ptr = bytes)
        {
            // Reinterpret as 50 byte actual data and 1 deobfuscate key byte
            EncryptedDataAndKey* data = (EncryptedDataAndKey*)ptr;
            byte* dataPtr = data->Data;

            // ----------------------------------------------------------
            // |   First   |  Second  | Padding |
            // |-----------|----------|---------|
            // |  25 Bytes | 25 Bytes |  1 Byte |
            // ----------------------------------------------------------
            // We are doing two things here:
            // 1. Retrieve actual data by subtracting key
            // 2. Store data into two halves by alternating between them
            // ----------------------------------------------------------
            // What we will get after this step:
            // ----------------------------------------------------------
            // First:
            // 00000011 11000000 00010011 00000000 00010000
            // 01001111 00000111 10000000 01111000 00001000
            // 00010000 10010111 00001001 01110000 10011001
            // 00001001 10010000 10110101 00001011 10000000
            // 10111000 00001011 10010000 10111001 00001100
            // ----------------------------------------------------------
            // Second:
            // 00100000 11000010 00001100 10000000 11001000
            // 00001100 10010000 11001001 00001101 10010000
            // 11011001 00001101 10110000 11110001 00001111
            // 00010000 11110100 00001111 01000000 11110101
            // 00001111 01010001 00001110 00010000 11100000
            // ----------------------------------------------------------
            RearrangeBuffer rearranged = default;
            byte* pFirst = rearranged.First;
            byte* pSecond = rearranged.Second;
            for (int i = 0; i < 50; i++)
            {
                // Determine which half are we going to insert
                byte** ppTarget = i % 2 == 0 ? &pFirst : &pSecond;

                // (actual data = data - key) and store it directly to the target half
                **ppTarget = unchecked((byte)(dataPtr[i] - data->Key));

                (*ppTarget)++;
            }

            // Prepare decoded data result storage
            DecryptedData decoded = default;
            ushort* pDecoded = decoded.Data;

            // ----------------------------------------------------------
            // |   Data   |
            // |----------| x 17 = 51 Bytes
            // |  3 Bytes |
            // ----------------------------------------------------------
            // Grouping each 3 bytes and read out as 2 ushort with
            // 12 bits each (Big Endian)
            // ----------------------------------------------------------
            // 00000011 1100·0000 00010011|
            // 00000000 0001·0000 01001111|
            // 00000111 1000·0000 01111000|
            // 00001000 0001·0000 10010111|
            // 00001001 0111·0000 10011001|
            // 00001001 1001·0000 10110101|
            // 00001011 1000·0000 10111000|
            // 00001011 1001·0000 10111001|
            // 00001100 0010·0000 11000010|
            // 00001100 1000·0000 11001000|
            // 00001100 1001·0000 11001001|
            // 00001101 1001·0000 11011001|
            // 00001101 1011·0000 11110001|
            // 00001111 0001·0000 11110100|
            // 00001111 0100·0000 11110101|
            // 00001111 0101·0001 00001110|
            // 00010000 1110·0000 -padding|[padding32]
            // ----------------------------------------------------------
            // reinterpret as DecodeGroupingHelper for each 3 bytes
            DecodeGroupingHelper* pGroup = (DecodeGroupingHelper*)&rearranged;
            for (int i = 0; i < 17; i++)
            {
                (ushort first, ushort second) = pGroup->GetData();

                *pDecoded = first;
                *(pDecoded + 1) = second;

                pDecoded += 2;
                pGroup++;
            }

            // Now we get
            //  60, 19,  1,
            //  79,120,120,
            // 129,151,151,
            // 153,153,181,
            // 184,184,185,
            // 185,194,194,
            // 200,200,201,
            // 201,217,217,
            // 219,241,241,
            // 244,244,245,
            // 245,270,270,
            StringBuilder stringBuilder = new();
            for (int i = 0; i < 33; i++)
            {
                stringBuilder
                    .AppendFormat(CultureInfo.InvariantCulture, "{0,3}", decoded.Data[i])
                    .Append(',');

                if (i % 11 == 10)
                {
                    stringBuilder.Append('\n');
                }
            }

            TestContext?.WriteLine(stringBuilder.ToString(0, stringBuilder.Length - 1));

            ushort[] resultArray = new ushort[33];
            Span<ushort> result = new((ushort*)&decoded, 33);
            result.CopyTo(resultArray);

            ushort[] testKnownResult =
            [
                060,
                019,
                001,
                079,
                120,
                120,
                129,
                151,
                151,
                153,
                153,
                181,
                184,
                184,
                185,
                185,
                194,
                194,
                200,
                200,
                201,
                201,
                217,
                217,
                219,
                241,
                241,
                244,
                244,
                245,
                245,
                270,
                270,
            ];

            CollectionAssert.AreEqual(resultArray, testKnownResult);
        }
    }

    [SuppressMessage("", "CS0649")]
    private struct EncryptedDataAndKey
    {
        public unsafe fixed byte Data[50];
        public byte Key;
    }

    private struct RearrangeBuffer
    {
        public unsafe fixed byte First[25];
        public unsafe fixed byte Second[25];

        // Make it 51 bytes
        // allow to be group as 17 DecodeGroupingHelper later
        public byte padding;

        // prevent accidently int32 cast access violation
        public byte paddingTo32;
    }

    private struct DecodeGroupingHelper
    {
        public unsafe fixed byte Data[3];

        public unsafe (ushort First, ushort Second) GetData()
        {
            fixed (byte* ptr = Data)
            {
                uint value = BinaryPrimitives.ReverseEndianness((*(uint*)ptr) & 0x00FFFFFF) >> 8; // keep low 24 bits only
                return ((ushort)((value >> 12) & 0x0FFF), (ushort)(value & 0x0FFF));
            }
        }
    }

    private struct DecryptedData
    {
        public unsafe fixed ushort Data[33];
    }
}