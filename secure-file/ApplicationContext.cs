using System;

namespace AppVeyor.Tools.SecureFile
{
    /// <summary>
    /// Contains the contextual information for the application.
    /// </summary>
    class ApplicationContext
    {
        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        public OperationType Operation { get; private set; }

        /// <summary>
        /// Gets or sets the secret.
        /// </summary>
        public string Secret { get; private set; }

        /// <summary>
        /// Gets or sets the base64 encoded salt.
        /// </summary>
        public string Salt { get; set; }
        
        /// <summary>
        /// Gets or sets the input file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the output file name.
        /// </summary>
        public string OutputFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a FIPS compliant algorithm must be used.
        /// </summary>
        public bool RequireFipsCompliance { get; set; }

        public static ApplicationContext Parse(string[] args)
        {
            if (args.Length == 0)
            {
                throw new Exception("No arguments specified.");
            }

            var result = new ApplicationContext();

            int pos = 0;
            while (pos < args.Length)
            {
                var p = args[pos];
                if (p.Equals("-decrypt", StringComparison.OrdinalIgnoreCase))
                {
                    // is it last parameter?
                    if (pos == args.Length - 1)
                    {
                        throw new Exception("Input file name is missing.");
                    }
                    else
                    {
                        result.Operation = OperationType.Decrypt;
                        result.FileName = args[++pos];
                    }
                }
                else if (p.Equals("-encrypt", StringComparison.OrdinalIgnoreCase))
                {
                    // is it last parameter?
                    if (pos == args.Length - 1)
                    {
                        throw new Exception("Input file name is missing.");
                    }
                    else
                    {
                        result.Operation = OperationType.Encrypt;
                        result.FileName = args[++pos];
                    }
                }
                else if (p.Equals("-salt", StringComparison.OrdinalIgnoreCase))
                {
                    if (pos == args.Length - 1)
                    {
                        throw new Exception("The Base64 encoded salt value is missing.");
                    }
                    else
                    {
                        result.Salt = args[++pos];
                    }
                }
                else if (p.Equals("-secret", StringComparison.OrdinalIgnoreCase))
                {
                    // is it last parameter?
                    if (pos == args.Length - 1)
                    {
                        throw new Exception("Secret passphrase is missing.");
                    }
                    else
                    {
                        result.Secret = args[++pos];
                    }
                }
                else if (p.Equals("-fips", StringComparison.OrdinalIgnoreCase))
                {
                    result.RequireFipsCompliance = true;
                }
                else if (p.Equals("-out", StringComparison.OrdinalIgnoreCase))
                {
                    // is it last parameter?
                    if (pos == args.Length - 1)
                    {
                        throw new Exception("Out file name is missing.");
                    }
                    else
                    {
                        result.OutputFileName = args[++pos];
                    }
                }

                pos++;
            }

            return result;
        }
    }
}