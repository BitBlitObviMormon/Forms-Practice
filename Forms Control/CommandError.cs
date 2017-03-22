
namespace Forms_Control
{
    public sealed class CommandError
    {
        public static readonly CommandError Success = new CommandError(1);
        public static readonly CommandError Null = new CommandError(0);
        public static readonly CommandError InvalidCommand = new CommandError(-1);
        public static readonly CommandError InvalidArgument = new CommandError(-2);
        public static readonly CommandError VarDoesNotExist = new CommandError(-3);
        public static readonly CommandError NotEnoughArguments = new CommandError(-4);

        private readonly int err;        
        private CommandError(int err) { this.err = err; }

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
                    return "Invalid argument";
                case -3:
                    return "Variable does not exist";
                case -4:
                    return "Not enough arguments were passed";
                default:
                    return "Invalid error code";
            }
        }
    }
}
