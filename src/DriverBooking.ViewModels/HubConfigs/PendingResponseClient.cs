using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverBooking.Core.HubConfigs
{
    public class PendingResponseClient
    {
        private ConcurrentDictionary<Guid, TaskCompletionSource<object>> _pendingResponse = new ConcurrentDictionary<Guid, TaskCompletionSource<object>>();

        // add a pending
        public void AddPending(Guid pendingId, TaskCompletionSource<object> value)
        {
            _pendingResponse[pendingId] = value;
        }

        // complete pending with T value response
        public void CompletePending(Guid pendingId, object responseValue)
        {
            if (_pendingResponse.TryRemove(pendingId, out var taskCompletionSource))
            {
                taskCompletionSource.SetResult(responseValue);
                // after this all await this task complete source will continue to run
            }
        }
    }
}