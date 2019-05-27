using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Turing_Back_ED.Utilities
{
    public class SecretHasher
    {
        //get a hashed password only
        public static string GetHash(string password)
        {
            return Hash(password).hash;
        }

        //return a hashed password combined with its salt
        public static string GetHashWithSalt(string password)
        {
            //get a hashed password, combined with the salt used for hashing
            var hashnsalt = Hash(password);
            return $"{hashnsalt.hash}::{hashnsalt.salt}";
        }

        public static bool CompareHashes(string hashedPass, string rawPass)
        {
            //get and split the password(hashed) of the match that was found,
            //into the has, and the salt
            var splitPassword = hashedPass.Split("::");

            //hash the newly provided password with the salt of
            //the password of the matched user. Then compare the result
            //of the hashing to the hash of the matched user
            //and then return whether they match or not
            return splitPassword[0]
                .Equals(Hash(password: rawPass,// the entered password 
                saltString: splitPassword[1])//the salt of the found match's password hash
                .hash);

        }

        private static (string hash, string salt) Hash(string password, string saltString = null)
        {

            byte[] salt;
            if (string.IsNullOrWhiteSpace(saltString))
            {
                // generate a 128-bit salt using a secure PRNG
                salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
            }
            else
            {
                //get the existing salt from the base64 string 
                salt = Convert.FromBase64String(saltString);
            }

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return (hash: hashed, salt: Convert.ToBase64String(salt));
        }
    }

    /*
     * SAMPLE OUTPUT
     *
     * Enter a password: Xtw9NMgx
     * Salt: NZsP6NnmfBuYeJrrAKNuVQ==
     * Hashed: /OOoOer10+tGwTRDTrQSoeCxVTFr6dtYly7d0cPxIak=
     */
}
