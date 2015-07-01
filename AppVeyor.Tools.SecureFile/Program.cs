using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppVeyor.Tools.SecureFile
{
    class Program
    {
        private static string Salt = "{E4E66F59-CAF2-4C39-A7F8-46097B1C461B}";

        static void Main(string[] args)
        {
            string operation = null;
            string fileName = null;
            string secret = null;
            string outFileName = null;

            try
            {
                #region parse parameters
                if (args.Length == 0)
                {
                    throw new Exception("No arguments specified.");
                }

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
                            operation = "decrypt";
                            fileName = args[++pos];
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
                            operation = "encrypt";
                            fileName = args[++pos];
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
                            secret = args[++pos];
                        }
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
                            outFileName = args[++pos];
                        }
                    }

                    pos++;
                }

                if(operation == null)
                {
                    throw new Exception("No operation specified. It should be either -encrypt or -decrypt.");
                }
                #endregion

                #region validate file names
                if (outFileName == null && operation == "encrypt")
                {
                    outFileName = fileName + ".enc";
                }
                else if (outFileName == null && operation == "decrypt")
                {
                    if (Path.GetExtension(fileName).Equals(".enc", StringComparison.OrdinalIgnoreCase))
                    {
                        outFileName = fileName.Substring(0, fileName.Length - 4); // trim .enc
                    }
                    else
                    {
                        outFileName = fileName + ".dec";
                    }
                }

                var basePath = Environment.CurrentDirectory;

                // convert relative paths to absolute
                if (!Path.IsPathRooted(fileName))
                {
                    fileName = Path.GetFullPath(Path.Combine(basePath, fileName));
                }

                if (!Path.IsPathRooted(outFileName))
                {
                    outFileName = Path.GetFullPath(Path.Combine(basePath, outFileName));
                }

                // in and out file names should not be the same
                if (fileName.Equals(outFileName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Input and output files cannot be the same.");
                }

                if (!File.Exists(fileName))
                {
                    throw new Exception("File not found: " + fileName);
                }
                #endregion
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                
                Console.WriteLine(@"
USAGE:

Encrypting file:

    secure-file -encrypt <filename.ext> -secret <keyphrase> -out [filename.ext.enc]

Decrypting file:

    secure-file -decrypt <filename.ext.enc> -secret <keyphrase> -out [filename.ext]
");
                Environment.Exit(1);
            }

            if(operation == "encrypt")
            {
                try
                {
                    Encrypt(fileName, outFileName, secret);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error encrypting file: " + ex.Message);
                }
            }
            else if (operation == "decrypt")
            {
                try
                {
                    Decrypt(fileName, outFileName, secret);
                }
                catch (CryptographicException)
                {
                    Console.WriteLine("Error decrypting file.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error decrypting file: " + ex.Message);
                }
            }
        }

        private static void Encrypt(string fileName, string outFileName, string secret)
        {
            var alg = GetRijndael(secret);

            using(var inStream = File.OpenRead(fileName))
            {
                using(var outStream = File.Create(outFileName))
                {
                    using (var cryptoStream = new CryptoStream(outStream, alg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        inStream.CopyTo(cryptoStream);
                    }
                }
            }
        }

        private static void Decrypt(string fileName, string outFileName, string secret)
        {
            var alg = GetRijndael(secret);

            using (var inStream = File.OpenRead(fileName))
            {
                using (var outStream = File.Create(outFileName))
                {
                    using (var cryptoStream = new CryptoStream(outStream, alg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        inStream.CopyTo(cryptoStream);
                    }
                }
            }
        }

        private static Rijndael GetRijndael(string secret)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(secret, Encoding.UTF8.GetBytes(Salt), 10000);

            Rijndael alg = Rijndael.Create();
            alg.Key = pbkdf2.GetBytes(32);
            alg.IV = pbkdf2.GetBytes(16);

            return alg;
        }
    }
}
