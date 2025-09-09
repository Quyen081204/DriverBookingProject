using System.Collections.Concurrent;
using DriverBooking.Core.Models.Common;
using DriverBooking.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DriverBooking.Core.HubConfigs
{
    [Authorize]
    public class BookingHub : Hub
    {
        // Map username to connectionId
        private readonly ConnectionMapping<string> _connections;

        // Sigleton
        private PendingResponseClient _pendingResponse;

        private readonly ManageCancellationToken _manageCancellationToken;

        private readonly ITripRepository _tripRepository;
        public BookingHub(PendingResponseClient pendingResponse, ConnectionMapping<string> connections,
                          ManageCancellationToken manageCancellationToken, ITripRepository tripRepository)
        {
            _pendingResponse = pendingResponse;
            _connections = connections;
            _manageCancellationToken = manageCancellationToken;
            _tripRepository = tripRepository;
        }

        public async Task TestConnectedBySendMessage(string clientName, string message)
        {
            await Clients.All.SendAsync("ReceiveConnectedMessage", $"hi {clientName}");
        }

        // call back on server to receive message from driver 
        // tam thoi de yes no co the minh se can gui dongy/khong hoac them thong tin ca nhan ....
        public Task ReceiveDriverResponse(Guid pendingId, bool yesNo)
        {
            _pendingResponse.CompletePending(pendingId, yesNo);
            return Task.CompletedTask;
        }

        public async Task SendMessToCustomerAboutFoundDriver(string customerUserName, object infoDriver)
        {
            await Clients.Client(_connections.GetConnection(customerUserName)).SendAsync("ReceiveDriverInfo", infoDriver);
        }

        // Consider write api or this method
        // Hub method dùng để cancel trip
        public async Task<ApiResponse<bool>> CancelTrip(Guid guid, string cancelReason)
        {
            var tripCancled = await _tripRepository.GetTripById(guid);
            if(tripCancled.CustomerId.ToString() == Context.User.FindFirst("profileId")?.Value)
            {
                _manageCancellationToken.CancelToken(guid);

                if (tripCancled?.DriverId != null)
                {
                    tripCancled.CancelReason = cancelReason;
                    tripCancled.RequestStatus = Domain.Entities.TripRequestStatus.CANCELED;
                    var driverUsername = tripCancled.Driver.DriverAccount.UserName;
                    await Clients.Client(_connections.GetConnection(driverUsername)).SendAsync("CustomerCancelTrip");
                    await _tripRepository.UpdateTrip(guid, tripCancled);
                }

                return ApiResponse<bool>.CreateSuccessResponse(true, "Cancel trip successfully");
            }

            return ApiResponse<bool>.CreateFailureResponseWithoutError("Fail cancel trip this is not trip belong with you");
        }

        public override async Task OnConnectedAsync()
        {
            // get username
            string username = Context.User.Identity.Name;
            // add current connectionId
            _connections.Add(username, Context.ConnectionId);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // get username
            string username = Context.User.Identity.Name;
            // remove current connectionId
            _connections.Remove(username);

            return base.OnDisconnectedAsync(exception);
        }
    }
}