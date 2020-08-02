using System;
using System.Threading.Tasks;
using Ess3.Library.Interfaces;

namespace Ess3.Library.S3
{
    public static class S3Helpers
    {
        public static async Task<bool> ValidAccountAsync(IAccount account)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5d)).ConfigureAwait(false);

            return true;
        }
    }
}
