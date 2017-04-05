namespace Forms_Control
{
    /************************************************************
     * CONTROL SIGNAL TYPE                                      *
     * Contains the types of events for console control signals *
     ************************************************************/
    public enum CtrlSigType
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT = 1,
        CTRL_CLOSE_EVENT = 2,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT = 6
    }

    /*************************************************************
     * CONTROL SIGNAL EVENT ARGUMENTS                            *
     * Used for getting the type of signal when raising an event *
     *************************************************************/
    public class CtrlSigEventArgs : System.EventArgs
    {
        public CtrlSigType type { get; private set; }
        public CtrlSigEventArgs(CtrlSigType type) : base() { this.type = type; }
    }

    /*********************************************************
     * CONTROL SIGNAL EVENT                                  *
     * Used for (un)subscribing events to the signal handler *
     *********************************************************/
    public class CtrlSigEvent
    {
        /*************************************************************
         * Adds or removes a control signal handler from the console *
         *************************************************************/
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        /****************************************************
         * Used by the Console Ctrl Handler to raise events *
         ****************************************************/
        private delegate bool EventHandler(CtrlSigType sig);

        /***************
         * Constructor *
         ***************/
        public CtrlSigEvent() {}

        /*********************************
         * Constructor with subscription *
         *********************************/
        public CtrlSigEvent(System.Action<object, CtrlSigEventArgs> eventToSubscribe) : this()
        {
            // Subscribe a CtrlSigEvent
            setHandle(eventToSubscribe, true);
        }

        /***************************
         * += operator (subscribe) *
         ***************************/
        public static CtrlSigEvent operator +(CtrlSigEvent left, System.Action<object, CtrlSigEventArgs> right)
        {
            // Subscribe a CtrlSigEvent
            left.setHandle(right, true);

            return left;
        }

        /*****************************
         * -= operator (unsubscribe) *
         *****************************/
        public static CtrlSigEvent operator -(CtrlSigEvent left, System.Action<object, CtrlSigEventArgs> right)
        {
            // Unsubscribe a CtrlSigEvent
            left.setHandle(right, false);

            return left;
        }

        /********************************************************
         * (Un)Subscribes the event to the console ctrl handler *
         ********************************************************/
        private void setHandle(System.Action<object, CtrlSigEventArgs> func, bool subscribe)
        {
            EventHandler handler = new EventHandler((CtrlSigType type) => { func.Invoke(this, new CtrlSigEventArgs(type)); return false; });
            SetConsoleCtrlHandler(handler, subscribe);
        }
    }
}