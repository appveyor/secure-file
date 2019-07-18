namespace AppVeyor.Tools.SecureFile
{
    /// <summary>
    /// Identifies a command which can be executed.
    /// </summary>
    interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="context">The application contextual information for the command to be executed.</param>
        void Execute(ApplicationContext context);
    }
}