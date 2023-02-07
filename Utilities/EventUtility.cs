namespace USPSAddressValidator.Helpers
{
    public class EventUtility
    {
        public EventUtility() { }

        public int Request
        {
            get
            {
                return RequestCount;
            }
            set
            {
                Interlocked.Increment(ref RequestCount);
            }
        }

        public void Success()
        {
            Interlocked.Increment(ref SuccessCount);
        }

        public void Exception()
        {
            Interlocked.Increment(ref ExceptionCount);
        }

        public void NotFound()
        {
            Interlocked.Increment(ref NotFoundCount);
        }

        public void BadRequest()
        {
            Interlocked.Increment(ref BadRequestCount);
        }

        private int RequestCount;
        private int SuccessCount;
        private int ExceptionCount;
        private int NotFoundCount;
        private int BadRequestCount;

        public void Clear()
        {
            RequestCount = 0;
            SuccessCount = 0;
            ExceptionCount = 0;
            NotFoundCount = 0;
            BadRequestCount = 0;
        }
    }
}
