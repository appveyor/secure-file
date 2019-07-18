using System;
using System.Security.Cryptography;

namespace AppVeyor.Tools.SecureFile
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var context = ApplicationContext.Parse(args);
                ValidateApplicationContext(context);

                try
                {
                    Execute(context);
                }
                catch (CryptographicException)
                {
                    Console.WriteLine("Encountered a cryptographic error while processing the file.");
                    Environment.Exit(2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error processing file: " + ex.Message);
                    Environment.Exit(3);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine();

                WriteSyntaxToConsole();

                Environment.Exit(1);
            }
        }

        private static void ValidateApplicationContext(ApplicationContext context)
        {
            var validator = new ApplicationContextValidator();
            validator.Validate(context);
        }

        private static void Execute(ApplicationContext context)
        {
            var command = new CommandFactory().Create(context);
            command.Execute(context);
        }

        private static void WriteSyntaxToConsole()
        {
            Console.WriteLine(@"
USAGE:

Encrypting file:

    secure-file -encrypt <filename.ext> -secret <keyphrase> -out [filename.ext.enc]

Decrypting file:

    secure-file -decrypt <filename.ext.enc> -secret <keyphrase> -salt [value] -out [filename.ext]
");
        }
    }
}