namespace KRFCommon.Extensions
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class HashExtensions
    {
        public static string GetSha1HashFromString( this string value )
        {
            return value.GetBytesFromString().GetSha1HashFromBytes();
        }

        public static string GetSha256HashFromString( this string value )
        {
            return value.GetBytesFromString().GetSha256HashFromBytes();
        }

        public static string GetSha512HashFromString( this string value )
        {
            return value.GetBytesFromString().GetSha512HashFromBytes();
        }

        public static string GetHashFromPassword( this string password )
        {
            using ( var hash512 = SHA512.Create() )
            {
                using ( var hash256 = SHA256.Create() )
                {
                    var part1 = hash256.ComputeHash( password.GetBytesFromString() ).GetStringFromHashBytes();
                    var part2 = hash256.ComputeHash( password.ToUpperInvariant().GetBytesFromString() ).GetStringFromHashBytes();

                    return hash512.ComputeHash( string.Concat( part1, part2 ).GetBytesFromString() ).GetStringFromHashBytes();
                }
            }
        }

        public static string GetSha1HashFromBytes( this byte[] bytes )
        {
            using ( var hash = SHA1.Create() )
            {
                return hash.ComputeHash( bytes ).GetStringFromHashBytes();
            }
        }

        public static string GetSha256HashFromBytes( this byte[] bytes )
        {
            using ( var hash = SHA256.Create() )
            {
                return hash.ComputeHash( bytes ).GetStringFromHashBytes();
            }
        }

        public static string GetSha512HashFromBytes( this byte[] bytes )
        {
            using ( var hash = SHA512.Create() )
            {
                return hash.ComputeHash( bytes ).GetStringFromHashBytes();
            }
        }

        private static byte[] GetBytesFromString( this string value )
        {
            return Encoding.UTF8.GetBytes( value );
        }

        private static string GetStringFromHashBytes( this byte[] bytes )
        {
            return BitConverter.ToString( bytes ).ToLowerInvariant().Replace( "-", "" );
        }
    }
}
