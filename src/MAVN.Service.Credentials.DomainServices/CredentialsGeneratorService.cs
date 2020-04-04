using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Common.Log;
using Falcon.Common;
using Lykke.Common.Log;
using MAVN.Service.Credentials.Domain.Services;

namespace MAVN.Service.Credentials.DomainServices
{
    public class CredentialsGeneratorService : ICredentialsGeneratorService
    {
        // ReSharper disable StringLiteralTypo
        private const string ClientIdBase = "0123456789abcdefghijklmnpoqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZO";
        private readonly string[] _passwordBase = {
            "0123456789",
            
            "abcdefghijklmnpqrstuvwxyzo",
            "ABCDEFGHIJKLMNPQRSTUVWXYZO",
            "!@#$%&"
        };
        // ReSharper restore StringLiteralTypo

        private readonly ILog _log;

        public CredentialsGeneratorService(ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
        }

        public string GenerateClientId(int length)
        {
            return GetRandomString(length, ClientIdBase);
        }

        public string GeneratePassword(int length)
        {
            if (length < _passwordBase.Length)
            {
                var exception = new InvalidOperationException($"Length cannot be less than {_passwordBase.Length} due to password symbols restriction.");
                _log.Error(exception);

                throw exception;
            }

            // length / PasswordBase.Length - 1 we are taking equal number of characters per password mask
            var passwordParts = _passwordBase.Select(p => 
                GetRandomString(length / _passwordBase.Length, p));

            return Shuffle(string.Join(string.Empty, passwordParts));
        }

        private string Shuffle(string str)
        {
            var array = str.ToCharArray();
            var rng = new Random();
            var n = array.Length;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }

        private string GetRandomString(int length, string mask)
        {
            var input = Guid.NewGuid().ToString();

            var bytes = Encoding.UTF8.GetBytes(input);

            var hash = bytes.ComputeSha256Hash();

            var baseString = GetBaseString(hash, mask);

            // In rare occasion the base string can be smaller than required length
            // due to the way bytes and sha computing works
            while (baseString.Length < length)
            {
                input = Guid.NewGuid().ToString();

                bytes = Encoding.UTF8.GetBytes(input);

                hash = bytes.ComputeSha256Hash();

                baseString += GetBaseString(hash, mask);
            }

            return baseString.Substring(0, length - 1);
        }

        private string GetBaseString(byte[] toConvert, string mask)
        {
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(toConvert);
            }

            var dividend = new BigInteger(toConvert);
            var builder = new StringBuilder();

            while (dividend != 0)
            {
                dividend = BigInteger.DivRem(dividend, mask.Length, out var remainder);
                builder.Insert(0, mask[Math.Abs((int)remainder)]);
            }

            return builder.ToString();
        }
    }
}
