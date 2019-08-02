using System;
using System.IO;
using System.Security.Cryptography;

namespace AppVeyor.Tools.SecureFile.Commands
{
    /// <summary>
    /// Provides an abstract command implementation for a symmetric algorithm.
    /// </summary>
    abstract class SymmetricAlgorithmCommand : ICommand
    {
        /// <summary>
        /// Identifies the default number of iterations for the PBKDF2 derivation function.
        /// </summary>
        public const int DefaultIterationsCount = 10000;

        public void Execute(ApplicationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            OnExecute(context);
            OnAfterExecute(context);
        }

        protected virtual void OnExecute(ApplicationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            using (var algorithm = CreateAlgorithm(context))
            using (Stream inStream = File.OpenRead(context.FileName), outStream = File.Create(context.OutputFileName))
            {
                using (var transform = CreateTransform(algorithm))
                using (var cryptoStream = new CryptoStream(outStream, transform, CryptoStreamMode.Write))
                {
                    inStream.CopyTo(cryptoStream);
                }
            }
        }

        protected virtual void OnAfterExecute(ApplicationContext context)
        {
        }

        protected abstract ICryptoTransform CreateTransform(SymmetricAlgorithm algorithm);

        protected virtual SymmetricAlgorithm CreateAlgorithm(ApplicationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            SymmetricAlgorithm alg = null;

            try
            {
                alg = CreateAlgorithmImpl(context);
                var salt = GetSaltBytes(context);

                using (var pbkdf2 = new Rfc2898DeriveBytes(context.Secret, salt, DefaultIterationsCount))
                {
                    alg.Key = pbkdf2.GetBytes(32);
                    alg.IV = pbkdf2.GetBytes(16);
                }

                return alg;
            }
            catch (Exception)
            {
                alg?.Dispose();
                throw;
            }
        }

        protected virtual SymmetricAlgorithm CreateAlgorithmImpl(ApplicationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.RequireFipsCompliance)
            {
                return Aes.Create();
            }

            return Rijndael.Create();
        }

        protected abstract byte[] GetSaltBytes(ApplicationContext context);
    }
}