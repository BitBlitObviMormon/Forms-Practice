namespace Forms_Control
{
    /************************************************************************************
     * COMMAND ERROR                                                                    *
     * An advanced enum for all of the possible errors the Command Manager might return *
     ************************************************************************************/
    public sealed class CommandError
    {
        /* The values for all of the errors the Command Manager might return */
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

        /* Stores the error code */
        private readonly int err;

        /*******************************
         * Constructor from error code *
         *******************************/
        private CommandError(int err) { this.err = err; }

        /********************
         * Copy Constructor *
         ********************/
        private CommandError(CommandError err) { this.err = err.err; }

        /***************************************************
         * Implicitly convert an integer to a CommandError *
         ***************************************************/
        public static implicit operator CommandError(int value) { return new CommandError(value); }

        /***************************************************
         * Implicitly convert a Command Error to an integer *
         ***************************************************/
        public static implicit operator int(CommandError err) { return err.err; }

        /***************************************************************************
         * Return the descriptive text of an error when it's converted to a string *
         ***************************************************************************/
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
