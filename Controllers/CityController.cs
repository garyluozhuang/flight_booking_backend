using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;

namespace wpi.Controllers;

[ApiController]
[Route("api/flights")]
public class FlightsController(IDbConnection dbConnection) : ControllerBase
{
    private readonly IDbConnection _dbConnection = dbConnection;

    [HttpGet("one-way")]
    public async Task<IActionResult> GetOneWayFlights(
        string departAirport,
        string arriveAirport,
        string departDate,
        string outboundArriveStartTime,
        string outboundArriveEndTime,
        string outboundDepartStartTime,
        string outboundDepartEndTime)
    {
        string query = @"
            SELECT Id, DATE_FORMAT(DepartDateTime, '%Y-%m-%d %H:%i:%s') as DepartDateTime, DATE_FORMAT(ArriveDateTime, '%Y-%m-%d %H:%i:%s') as ArriveDateTime, DepartAirport, ArriveAirport, FlightNumber, 'delta' as Airline
            FROM deltas
            WHERE DepartAirport = @DepartAirport
                AND ArriveAirport = @ArriveAirport
                AND DepartDateTime between CAST(CONCAT(@DepartDate, ' ', @OutboundDepartStartTime) AS DATETIME) and CAST(CONCAT(@DepartDate, ' ', @OutboundDepartEndTime) AS DATETIME)
                AND TIME(ArriveDateTime) between CAST(@OutboundArriveStartTime AS TIME) and CAST(@OutboundArriveEndTime AS TIME)
            UNION
            SELECT Id, DATE_FORMAT(DepartDateTime, '%Y-%m-%d %H:%i:%s') as DepartDateTime, DATE_FORMAT(ArriveDateTime, '%Y-%m-%d %H:%i:%s') as ArriveDateTime, DepartAirport, ArriveAirport, FlightNumber, 'southwest' as Airline
            FROM southwests
            WHERE DepartAirport = @DepartAirport
                AND ArriveAirport = @ArriveAirport
                AND DepartDateTime between CAST(CONCAT(@DepartDate, ' ', @OutboundDepartStartTime) AS DATETIME) and CAST(CONCAT(@DepartDate, ' ', @OutboundDepartEndTime) AS DATETIME)
                AND TIME(ArriveDateTime) between CAST(@OutboundArriveStartTime AS TIME) and CAST(@OutboundArriveEndTime AS TIME);
        ";

        var flights = await _dbConnection.QueryAsync<Flight>(query, new
        {
            DepartAirport = departAirport,
            ArriveAirport = arriveAirport,
            DepartDate = departDate,
            OutboundArriveStartTime = outboundArriveStartTime,
            OutboundArriveEndTime = outboundArriveEndTime,
            OutboundDepartStartTime = outboundDepartStartTime,
            OutboundDepartEndTime = outboundDepartEndTime
        });

        return Ok(flights);
    }

    [HttpGet("round-trip")]
    public async Task<IActionResult> GetRoundTripFlights(
        string departAirport,
        string arriveAirport,
        string departDate,
        string returnDate,
        string outboundArriveStartTime,
        string outboundArriveEndTime,
        string outboundDepartStartTime,
        string outboundDepartEndTime,
        string returnArriveStartTime,
        string returnArriveEndTime,
        string returnDepartStartTime,
        string returnDepartEndTime)
    {
        string query = @"
            SELECT Id, DATE_FORMAT(DepartDateTime, '%Y-%m-%d %H:%i:%s') as DepartDateTime, DATE_FORMAT(ArriveDateTime, '%Y-%m-%d %H:%i:%s') as ArriveDateTime, DepartAirport, ArriveAirport, FlightNumber, 'delta' as Airline
            FROM deltas
            WHERE DepartAirport = @DepartAirport
                AND ArriveAirport = @ArriveAirport
                AND DepartDateTime between CAST(CONCAT(@DepartDate, ' ', @OutboundDepartStartTime) AS DATETIME) and CAST(CONCAT(@DepartDate, ' ', @OutboundDepartEndTime) AS DATETIME)
                AND TIME(ArriveDateTime) between CAST(@OutboundArriveStartTime AS TIME) and CAST(@OutboundArriveEndTime AS TIME)
            UNION
            SELECT Id, DATE_FORMAT(DepartDateTime, '%Y-%m-%d %H:%i:%s') as DepartDateTime, DATE_FORMAT(ArriveDateTime, '%Y-%m-%d %H:%i:%s') as ArriveDateTime, DepartAirport, ArriveAirport, FlightNumber, 'southwest' as Airline
            FROM southwests
            WHERE DepartAirport = @DepartAirport
                AND ArriveAirport = @ArriveAirport
                AND DepartDateTime between CAST(CONCAT(@DepartDate, ' ', @OutboundDepartStartTime) AS DATETIME) and CAST(CONCAT(@DepartDate, ' ', @OutboundDepartEndTime) AS DATETIME)
                AND TIME(ArriveDateTime) between CAST(@OutboundArriveStartTime AS TIME) and CAST(@OutboundArriveEndTime AS TIME)
            UNION
            SELECT Id, DATE_FORMAT(DepartDateTime, '%Y-%m-%d %H:%i:%s') as DepartDateTime, DATE_FORMAT(ArriveDateTime, '%Y-%m-%d %H:%i:%s') as ArriveDateTime, DepartAirport, ArriveAirport, FlightNumber, 'delta' as Airline
            FROM deltas
            WHERE DepartAirport = @ArriveAirport
                AND ArriveAirport = @DepartAirport
                AND DepartDateTime between CAST(CONCAT(@ReturnDate, ' ', @ReturnDepartStartTime) AS DATETIME) and CAST(CONCAT(@ReturnDate, ' ', @ReturnDepartEndTime) AS DATETIME)
                AND TIME(ArriveDateTime) between CAST(@ReturnArriveStartTime AS TIME) and CAST(@ReturnArriveEndTime AS TIME)
            UNION
            SELECT Id, DATE_FORMAT(DepartDateTime, '%Y-%m-%d %H:%i:%s') as DepartDateTime, DATE_FORMAT(ArriveDateTime, '%Y-%m-%d %H:%i:%s') as ArriveDateTime, DepartAirport, ArriveAirport, FlightNumber, 'southwest' as Airline
            FROM southwests
            WHERE DepartAirport = @ArriveAirport
                AND ArriveAirport = @DepartAirport
                AND DepartDateTime between CAST(CONCAT(@ReturnDate, ' ', @ReturnDepartStartTime) AS DATETIME) and CAST(CONCAT(@ReturnDate, ' ', @ReturnDepartEndTime) AS DATETIME)
                AND TIME(ArriveDateTime) between CAST(@ReturnArriveStartTime AS TIME) and CAST(@ReturnArriveEndTime AS TIME);
        ";

        var flights = await _dbConnection.QueryAsync<Flight>(query, new
        {
            DepartAirport = departAirport,
            ArriveAirport = arriveAirport,
            DepartDate = departDate,
            ReturnDate = returnDate,
            OutboundArriveStartTime = outboundArriveStartTime,
            OutboundArriveEndTime = outboundArriveEndTime,
            OutboundDepartStartTime = outboundDepartStartTime,
            OutboundDepartEndTime = outboundDepartEndTime,
            ReturnArriveStartTime = returnArriveStartTime,
            ReturnArriveEndTime = returnArriveEndTime,
            ReturnDepartStartTime = returnDepartStartTime,
            ReturnDepartEndTime = returnDepartEndTime
        });
        

        return Ok(flights);
    }

    [HttpPost("booking")]
    public async Task<IActionResult> BookFlight(
        int flightId, 
        string fullName, 
        string birthDate, 
        string phone, 
        string email,
        string flightCompany)
{
    string query = @"
        INSERT INTO Bookings (FullName, BirthDate, Phone, Email, FlightCompany, FlightId)
        VALUES (@FullName, @BirthDate, @Phone, @Email, @FlightCompany, @FlightId);
        SELECT LAST_INSERT_ID();
    ";

    int bookingId = await _dbConnection.QuerySingleAsync<int>(query, new
    {
        FlightId = flightId,
        FullName = fullName,
        BirthDate = birthDate,
        Phone = phone,
        Email = email,
        FlightCompany = flightCompany
    });

    return Ok(new { BookingId = bookingId });
}

}

public class Flight
{
    public int Id { get; set; }
    public string? DepartDateTime { get; set; }
    public string? ArriveDateTime { get; set; }
    public string? DepartAirport { get; set; }
    public string? ArriveAirport { get; set; }
    public string? FlightNumber { get; set; }
    public string? Airline { get; set; }
}

public class Booking
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public string? FullName { get; set; }
    public string? BirthDate { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? FlightCompany { get; set; }
}