using System;
using System.Security.Cryptography;
using System.Text;

namespace AppVeyor.Tools.SecureFile.Commands
{
    /// <summary>
    /// Provides a command to decrypt a file.
    /// </summary>
    class DecryptCommand : SymmetricAlgorithmCommand
    {
        protected override ICryptoTransform CreateTransform(SymmetricAlgorithm algorithm)
        {
            if (algorithm == null)
            {
                throw new ArgumentNullException(nameof(algorithm));
            }

            return algorithm.CreateDecryptor();
        }

        protected override byte[] GetSaltBytes(ApplicationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(context.Salt))
            {
                Console.WriteLine("WARNING: You are using a hard-coded salt value! Please re-encrypt your data so a randomized salt can be generated (which you will also need to store).");

                // This is to support any files that were encrypted using the previous process.
                return Encoding.UTF8.GetBytes("{E4E66F59-CAF2-4C39-A7F8-46097B1C461B}");
            }

            return Convert.FromBase64String(context.Salt);
        }
    }
}