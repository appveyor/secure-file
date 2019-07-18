using System;
using AppVeyor.Tools.SecureFile.Commands;

namespace AppVeyor.Tools.SecureFile
{
    /// <summary>
    /// Provides a factory for creating commands.
    /// </summary>
    class CommandFactory
    {
        /// <summary>
        /// Creates a command.
        /// </summary>
        /// <param name="context">The contextual information for the factory to use when deciding on the command to create.</param>
        /// <returns>The command instance.</returns>
        /// <exception cref="NotSupportedException">The operation being created is not supported.</exception>
        public ICommand Create(ApplicationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            switch (context.Operation)
            {
                case OperationType.Decrypt:
                    return new DecryptCommand();

                case OperationType.Encrypt:
                    return new EncryptCommand();

                default:
                    throw new NotSupportedException($"The operation '{context.Operation}' is not supported.");
            }
        }
    }
}