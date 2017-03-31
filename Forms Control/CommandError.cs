namespace Forms_Control
{
    public sealed class CommandError
    {
        public static readonly CommandError Success            = new CommandError( 1);
        public static readonly CommandError Null               = new CommandError( 0);
        public static readonly CommandError InvalidCommand     = new CommandError(-1);
        public static readonly CommandError InvalidArgument    = new CommandError(-2);
        public static readonly CommandError VarDoesNotExist    = new CommandError(-3);
        public static readonly CommandError NotEnoughArguments = new CommandError(-4);
        public static readonly CommandError InvalidHandle      = new CommandError(-5);
        public static readonly CommandError NoCommandGiven     = new CommandError(-6);
        public static readonly CommandError PuppetNotCreated   = new CommandError(-7);
        public static readonly CommandError PuppetNotVisible   = new CommandError(-8);

        private readonly int err;
        private CommandError(int err) { this.err = err; }
        private CommandError(CommandError err) { this.err = err.err; }

        public static implicit operator CommandError(int value) { return new CommandError(value); }
        public static implicit operator int(CommandError err) { return err.err; }
        public override string ToString()
        {
            switch (err)
            {
                case 1:
                    return "Operation successful";
                case 0:
                    return "Return value is null";
                case -1:
                    return "Invalid command";
                case -2:
                    return "An invalid argument was given";
                case -3:
                    return "Variable does not exist";
                case -4:
                    return "Not enough arguments were passed";
                case -5:
                    return "An invalid window handle was passed";
                case -6:
                    return "No command was given";
                case -7:
                    return "Puppet form has not been created yet, use show command to create one";
                case -8:
                    return "Puppet form needs to be visible in order to run this command";
                default:
                    return "Invalid error code";
            }
        }
    }
}
