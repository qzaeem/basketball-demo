using System.Threading;

namespace Basketball_Demo
{
    public static class TaskUtils
    {
        public static void CancelAndDisposeCTS(ref CancellationTokenSource cts)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }

        public static CancellationToken RenewCTS(ref CancellationTokenSource cts, params CancellationToken[] tokens)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            if (tokens.Length > 0)
            {
                cts = CancellationTokenSource.CreateLinkedTokenSource(tokens);
            }
            else
            {
                cts = new CancellationTokenSource();
            }

            return cts.Token;
        }
    }
}