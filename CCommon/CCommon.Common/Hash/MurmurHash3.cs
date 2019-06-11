using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCommon.Common.Hash
{
    /// <summary>
    /// MurmurHash算法 Hash算法
    /// 高运算性能，低碰撞率，由Austin Appleby创建于2008年，现已应用到Hadoop、libstdc++、nginx、libmemcached等开源系统。2011年Appleby被Google雇佣，随后Google推出其变种的CityHash算法。
    /// 官方网站：https://sites.google.com/site/murmurhash/
    /// </summary>
    public class MurmurHash3
    {

        /** 128 bits of state */
        private static class LongPair
        {
            public static long val1;
            public static long val2;
        }

        private static uint fmix32(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }

        private static ulong fmix64(ulong k)
        {
            k ^= k >> 33;
            k *= 0xff51afd7ed558ccdL;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53L;
            k ^= k >> 33;
            return k;
        }

        /** Gets a long from a byte buffer in little endian byte order. */
        private static long getLongLittleEndian(byte[] buf, int offset)
        {
            return ((long)buf[offset + 7] << 56)   // no mask needed
                    | ((buf[offset + 6] & 0xffL) << 48)
                    | ((buf[offset + 5] & 0xffL) << 40)
                    | ((buf[offset + 4] & 0xffL) << 32)
                    | ((buf[offset + 3] & 0xffL) << 24)
                    | ((buf[offset + 2] & 0xffL) << 16)
                    | ((buf[offset + 1] & 0xffL) << 8)
                    | ((buf[offset] & 0xffL));        // no shift needed
        }


        //-----------------------------------------------------------------------------
        // Block read - if your platform needs to do endian-swapping or can only
        // handle aligned reads, do the conversion here
        private static uint ROTL32(uint x, SByte r)
        {
            return (x << r) | (x >> (32 - r));
        }

        private static ulong ROTL64(ulong x, SByte r)
        {
            return (x << r) | (x >> (64 - r));
        }

        private static uint fmix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;

            return h;
        }

        #region String
        public static uint GetHashCode_x86_32(string content)
        {
            return GetHashCode_x86_32(content, Encoding.UTF8);
        }

        public static uint[] GetHashCode_x86_128(string content)
        {
            return GetHashCode_x86_128(content, Encoding.UTF8);
        }

        public static ulong[] GetHashCode_x64_128(string content)
        {
            return GetHashCode_x64_128(content, Encoding.UTF8);
        }


        public static uint GetHashCode_x86_32(string content, Encoding encoding)
        {
            var bytes = encoding.GetBytes(content);
            return MurmurHash3.murmurhash3_x86_32(bytes, 0, bytes.Length, 42);
        }

        public static uint[] GetHashCode_x86_128(string content, Encoding encoding)
        {
            var bytes = encoding.GetBytes(content);
            return MurmurHash3.murmurhash3_x86_128(bytes, 0, bytes.Length, 42);
        }

        public static ulong[] GetHashCode_x64_128(string content, Encoding encoding)
        {
            var bytes = encoding.GetBytes(content);
            return MurmurHash3.murmurhash3_x64_128(bytes, 0, bytes.Length, 42);
        }
        #endregion

        #region Byte
        public static uint murmurhash3_x86_32(byte[] data, int offset, int len, uint seed)
        {
            int nblocks = len / 4;

            uint h1 = seed;

            uint c1 = 0xcc9e2d51;
            uint c2 = 0x1b873593;
            for (int i = 0; i < nblocks; i++)
            {
                uint k1 = BitConverter.ToUInt32(data, i * 4);
                k1 *= c1;
                k1 = ROTL32(k1, 15);
                k1 *= c2;

                h1 ^= k1;
                h1 = ROTL32(h1, 13);
                h1 = h1 * 5 + 0xe6546b64;
            }

            uint k1s = 0;

            switch (len & 3)
            {

                case 3: { k1s ^= (uint)data[len - (len & 3) + 2] << 16; } goto case 2;
                case 2: { k1s ^= (uint)data[len - (len & 3) + 1] << 8; } goto case 1;
                case 1:
                    {
                        k1s ^= data[len - (len & 3)];
                        k1s *= c1;
                        k1s = ROTL32(k1s, 15);
                        k1s *= c2;
                        h1 ^= k1s;
                    }

                    break;
            }

            h1 ^= (uint)len;

            h1 = fmix(h1);

            return h1;
        }

        public static uint[] murmurhash3_x86_128(byte[] data, int offset, int len, uint seed)
        {
            int nblocks = len / 16;

            uint h1 = seed;
            uint h2 = seed;
            uint h3 = seed;
            uint h4 = seed;
            uint c1 = 0x239b961b;
            uint c2 = 0xab0e9789;
            uint c3 = 0x38b34ae5;
            uint c4 = 0xa1e38b93;

            int lastIndex = nblocks * 16;



            for (int i = 0; i < nblocks; i++)
            {

                uint k1 = BitConverter.ToUInt32(data, i * 16);

                uint k2 = BitConverter.ToUInt32(data, i * 16 + 4);

                uint k3 = BitConverter.ToUInt32(data, i * 16 + 8);

                uint k4 = BitConverter.ToUInt32(data, i * 16 + 12);
                k1 *= c1; k1 = ROTL32(k1, 15); k1 *= c2; h1 ^= k1;

                h1 = ROTL32(h1, 19); h1 += h2; h1 = h1 * 5 + 0x561ccd1b;

                k2 *= c2; k2 = ROTL32(k2, 16); k2 *= c3; h2 ^= k2;

                h2 = ROTL32(h2, 17); h2 += h3; h2 = h2 * 5 + 0x0bcaa747;

                k3 *= c3; k3 = ROTL32(k3, 17); k3 *= c4; h3 ^= k3;

                h3 = ROTL32(h3, 15); h3 += h4; h3 = h3 * 5 + 0x96cd1c35;

                k4 *= c4; k4 = ROTL32(k4, 18); k4 *= c1; h4 ^= k4;

                h4 = ROTL32(h4, 13); h4 += h1; h4 = h4 * 5 + 0x32ac3b17;
            }

            uint kk1 = 0;
            uint kk2 = 0;
            uint kk3 = 0;
            uint kk4 = 0;

            switch (len & 15)
            {
                case 15: kk4 ^= (uint)data[len - (len & 15) + 14] << 16; goto case 14;
                case 14: kk4 ^= (uint)data[len - (len & 15) + 13] << 8; goto case 13;
                case 13:
                    kk4 ^= (uint)data[len - (len & 15) + 12] << 0;
                    kk4 *= c4; kk4 = ROTL32(kk4, 18); kk4 *= c1; h4 ^= kk4; goto case 12;

                case 12: kk3 ^= (uint)data[len - (len & 15) + 11] << 24; goto case 11;
                case 11: kk3 ^= (uint)data[len - (len & 15) + 10] << 16; goto case 10;
                case 10: kk3 ^= (uint)data[len - (len & 15) + 9] << 8; goto case 9;
                case 9:
                    kk3 ^= (uint)data[len - (len & 15) + 8] << 0;
                    kk3 *= c3; kk3 = ROTL32(kk3, 17); kk3 *= c4; h3 ^= kk3; goto case 8;

                case 8: kk2 ^= (uint)data[len - (len & 15) + 7] << 24; goto case 7;
                case 7: kk2 ^= (uint)data[len - (len & 15) + 6] << 16; goto case 6;
                case 6: kk2 ^= (uint)data[len - (len & 15) + 5] << 8; goto case 5;
                case 5:
                    kk2 ^= (uint)data[len - (len & 15) + 4] << 0;
                    kk2 *= c2; kk2 = ROTL32(kk2, 16); kk2 *= c3; h2 ^= kk2; goto case 4;

                case 4: kk1 ^= (uint)data[len - (len & 15) + 3] << 24; goto case 3;
                case 3: kk1 ^= (uint)data[len - (len & 15) + 2] << 16; goto case 2;
                case 2: kk1 ^= (uint)data[len - (len & 15) + 1] << 8; goto case 1;
                case 1:
                    kk1 ^= (uint)data[len - (len & 15)] << 0;
                    kk1 *= c1; kk1 = ROTL32(kk1, 15); kk1 *= c2; h1 ^= kk1;
                    break;
            }

            h1 ^= (uint)len; h2 ^= (uint)len; h3 ^= (uint)len; h4 ^= (uint)len;

            h1 += h2; h1 += h3; h1 += h4;
            h2 += h1; h3 += h1; h4 += h1;

            h1 = fmix(h1);
            h2 = fmix(h2);
            h3 = fmix(h3);
            h4 = fmix(h4);

            h1 += h2; h1 += h3; h1 += h4;
            h2 += h1; h3 += h1; h4 += h1;

            uint[] returnValue = new uint[4];
            returnValue[0] = h1;
            returnValue[1] = h2;
            returnValue[2] = h3;
            returnValue[3] = h4;
            return returnValue;
        }

        public static ulong[] murmurhash3_x64_128(byte[] data, int offset, int len, uint seed)
        {
            int nblocks = len / 16;

            ulong h1 = seed;
            ulong h2 = seed;

            ulong c1 = 0x87c37b91114253d5;
            ulong c2 = 0x4cf5ad432745937f;

            for (int i = 0; i < nblocks; i++)
            {
                ulong k1 = BitConverter.ToUInt64(data, i * 16);
                ulong k2 = BitConverter.ToUInt64(data, i * 16 + 8);

                k1 *= c1; k1 = ROTL64(k1, 31); k1 *= c2; h1 ^= k1;

                h1 = ROTL64(h1, 27); h1 += h2; h1 = h1 * 5 + 0x52dce729;

                k2 *= c2; k2 = ROTL64(k2, 33); k2 *= c1; h2 ^= k2;

                h2 = ROTL64(h2, 31); h2 += h1; h2 = h2 * 5 + 0x38495ab5;
            }

            ulong kk1 = 0;
            ulong kk2 = 0;

            switch (len & 15)
            {
                case 15: kk2 ^= (ulong)(uint)data[len - (len & 15) + 14] << 48; goto case 14;
                case 14: kk2 ^= (ulong)(uint)data[len - (len & 15) + 13] << 40; goto case 13;
                case 13: kk2 ^= (ulong)(uint)data[len - (len & 15) + 12] << 32; goto case 12;
                case 12: kk2 ^= (ulong)(uint)data[len - (len & 15) + 11] << 24; goto case 11;
                case 11: kk2 ^= (ulong)(uint)data[len - (len & 15) + 10] << 16; goto case 10;
                case 10: kk2 ^= (ulong)(uint)data[len - (len & 15) + 9] << 8; goto case 9;
                case 9:
                    kk2 ^= (ulong)(uint)data[len - (len & 15) + 8] << 0;
                    kk2 *= c2; kk2 = ROTL64(kk2, 33); kk2 *= c1; h2 ^= kk2; goto case 8;

                case 8: kk1 ^= (ulong)(uint)data[len - (len & 15) + 7] << 56; goto case 7;
                case 7: kk1 ^= (ulong)(uint)data[len - (len & 15) + 6] << 48; goto case 6;
                case 6: kk1 ^= (ulong)(uint)data[len - (len & 15) + 5] << 40; goto case 5;
                case 5: kk1 ^= (ulong)(uint)data[len - (len & 15) + 4] << 32; goto case 4;
                case 4: kk1 ^= (ulong)(uint)data[len - (len & 15) + 3] << 24; goto case 3;
                case 3: kk1 ^= (ulong)(uint)data[len - (len & 15) + 2] << 16; goto case 2;
                case 2: kk1 ^= (ulong)(uint)data[len - (len & 15) + 1] << 8; goto case 1;
                case 1:
                    kk1 ^= (ulong)(uint)data[len - (len & 15)] << 0;
                    kk1 *= c1; kk1 = ROTL64(kk1, 31); kk1 *= c2; h1 ^= kk1;
                    break;
            };


            h1 ^= (ulong)len; h2 ^= (ulong)len;

            h1 += h2;
            h2 += h1;

            h1 = fmix64(h1);
            h2 = fmix64(h2);

            h1 += h2;
            h2 += h1;

            ulong[] returnValue = new ulong[2];
            returnValue[0] = h1;
            returnValue[1] = h2;
            return returnValue;

        }
        #endregion
    }
}