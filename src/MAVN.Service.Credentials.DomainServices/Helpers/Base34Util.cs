using System;
using System.Numerics;
using System.Text;
using Falcon.Common;
using MAVN.Service.Credentials.Domain.Helpers;

namespace MAVN.Service.Credentials.DomainServices.Helpers
{
    public class Base34Util : IBase34Util
    {
        // ReSharper disable StringLiteralTypo
        private readonly string alphabet = "123456789abcdefghijklmnpqrstuvwxyz";
        // ReSharper restore StringLiteralTypo

        public string GenerateBase(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            var bytes = Encoding.UTF8.GetBytes(input);

            return GetBase34String(bytes.ComputeSha256Hash());
        }

        private string GetBase34String(byte[] toConvert)
        {
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(toConvert);
            }

            var dividend = new BigInteger(toConvert);
            var builder = new StringBuilder();

            while (dividend != 0)
            {
                dividend = BigInteger.DivRem(dividend, alphabet.Length, out var remainder);
                builder.Insert(0, alphabet[Math.Abs(((int)remainder))]);
            }

            return builder.ToString();
        }
    }
}
