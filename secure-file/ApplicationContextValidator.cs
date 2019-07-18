using System;
using System.IO;

namespace AppVeyor.Tools.SecureFile
{
    /// <summary>
    /// Validates the application context.
    /// </summary>
    class ApplicationContextValidator
    {
        /// <summary>
        /// Performs validation of the application context.
        /// </summary>
        /// <param name="context">The contextual information to validate.</param>
        public void Validate(ApplicationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            if (context.OutputFileName == null && context.Operation == OperationType.Encrypt)
            {
                context.OutputFileName = context.FileName + ".enc";
            }
            else if (context.OutputFileName == null && context.Operation == OperationType.Decrypt)
            {
                if (Path.GetExtension(context.FileName).Equals(".enc", StringComparison.OrdinalIgnoreCase))
                {
                    context.OutputFileName = context.FileName.Substring(0, context.FileName.Length - 4); // trim .enc
                }
                else
                {
                    context.OutputFileName = context.FileName + ".dec";
                }
            }

            var basePath = Environment.CurrentDirectory;

            // convert relative paths to absolute
            if (!Path.IsPathRooted(context.FileName))
            {
                context.FileName = Path.GetFullPath(Path.Combine(basePath, context.FileName));
            }

            if (!Path.IsPathRooted(context.OutputFileName))
            {
                context.OutputFileName = Path.GetFullPath(Path.Combine(basePath, context.OutputFileName));
            }

            // in and out file names should not be the same
            if (context.FileName.Equals(context.OutputFileName, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Input and output files cannot be the same.");
            }

            if (!File.Exists(context.FileName))
            {
                throw new Exception("File not found: " + context.FileName);
            }
        }
    }
}