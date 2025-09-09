using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Triangulate;

namespace DriverBooking.Core.HubConfigs
{   
    // Class nay dung de quan ly cancellation token, co the dung no de huy cancellation token tuy y bat cu luc nao
    public class ManageCancellationToken
    {
        private ConcurrentDictionary<Guid, CancellationTokenSource> _cancellationTokenSources = new ConcurrentDictionary<Guid, CancellationTokenSource>();

        public void Add(Guid id, CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSources.TryAdd(id, cancellationTokenSource); 
        }

        public void CancelToken(Guid id)
        {
            if (_cancellationTokenSources.TryRemove(id, out var cancellationTokenSource))
            {
                cancellationTokenSource.Cancel();   
            }
            ;
        }
    }
}
