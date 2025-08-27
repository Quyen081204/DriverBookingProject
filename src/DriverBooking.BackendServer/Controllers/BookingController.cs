using DriverBooking.API.Services.BookingServices.Interface;
using DriverBooking.Core.Models.Booking;
using DriverBooking.Core.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriverBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;

        private readonly IBookingService _bookingService;
        public BookingController(ILogger<BookingController> logger,
                                IBookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;
        }

        [HttpPost("init")]
        [Authorize(Roles = "Customer, Admin")]
        public async Task<ActionResult<ApiResponse<InitBookingResponse>>> InitBookingTrip([FromBody] InitBookingRequest initBookingRequest)
        {
            _logger.LogInformation("InitBookingTrip called with Depart: {Depart}, Dest: {Dest}, RequestVehicleCapacity: {RequestVehicleCapacity}", 
                initBookingRequest.Depart, initBookingRequest.Dest, initBookingRequest.RequestVehicleCapacity);

            var response = await _bookingService.InitBookingTrip(initBookingRequest);

            return Ok(response);
        }
    }
}
// Today: make BookingService, Use unit of work pattern

// Test controller with input and output 
// This is version use haversine formula to calculate the distance between two points   
