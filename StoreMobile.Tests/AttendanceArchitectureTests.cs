using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreMobile.Controllers;
using StoreMobile.Data;
using StoreMobile.Models;

namespace StoreMobile.Tests
{
    public class AttendanceArchitectureTests
    {
        /// <summary>
        /// Utility factory helper to generate an isolated, in-memory database context configuration block
        /// </summary>
        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"StoreMobile_AttendanceTestPool_{Guid.NewGuid()}")
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task ProcessScan_ShouldReturnNotFound_WhenUserIdentityDoesNotExist()
        {
            // Arrange: Initialize a completely empty database sandbox context
            using var context = CreateInMemoryDbContext();
            var controller = new AttendanceController(context);
            var invalidPayload = new PythonScanPayload { UsernameToken = "unknown_badge_holder" };

            // Act: Dispatch the payload packet through the target action method
            var result = await controller.ProcessScan(invalidPayload);

            // Assert: Confirm the engine returns a clean 404 NotFound object tracking security boundaries
            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Identity Validation Error", objectResult.Value?.ToString());
        }

        [Fact]
        public async Task ProcessScan_ShouldToggleToCheckIn_WhenNoPriorAttendanceHistoryExists()
        {
            // Arrange: Pre-populate the scaled database with one active merchant profile node
            using var context = CreateInMemoryDbContext();
            context.Users.Add(new User { Username = "active_merchant", Email = "merchant@store.com", PasswordHash = "key" });
            await context.SaveChangesAsync();

            var controller = new AttendanceController(context);
            var payload = new PythonScanPayload { UsernameToken = "active_merchant", NodeIdentifier = "TEST-NODE-01" };

            // Act: Execute the first badge scan process
            var result = await controller.ProcessScan(payload);

            // Assert: Confirm return data confirms a success state mapping down to a "CheckIn" entry type
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic data = okResult.Value!;
            
            // Accessing anonymous object return data properties via reflection wrappers
            string checkAction = data.GetType().GetProperty("Action").GetValue(data, null).ToString();
            Assert.Equal("CheckIn", checkAction);

            // Verify a row was written out down into the database persistence layer
            var savedRecord = await context.AttendanceRecords.FirstOrDefaultAsync(a => a.UserIdentity == "active_merchant");
            Assert.NotNull(savedRecord);
            Assert.Equal("CheckIn", savedRecord.ActionType);
            Assert.Equal("TEST-NODE-01", savedRecord.TerminalNode);
        }

        [Fact]
        public async Task ProcessScan_ShouldToggleToCheckOut_WhenPreviousRecordWasCheckIn()
        {
            // Arrange: Seed a user profile alongside an existing active CheckIn logging entry record block
            using var context = CreateInMemoryDbContext();
            context.Users.Add(new User { Username = "active_merchant", Email = "merchant@store.com", PasswordHash = "key" });
            context.AttendanceRecords.Add(new AttendanceRecord { UserIdentity = "active_merchant", ActionType = "CheckIn", Timestamp = DateTime.UtcNow.AddHours(-8) });
            await context.SaveChangesAsync();

            var controller = new AttendanceController(context);
            var payload = new PythonScanPayload { UsernameToken = "active_merchant" };

            // Act: Trigger the sequential scanning endpoint thread path
            var result = await controller.ProcessScan(payload);

            // Assert: Verify logic detects the past state and alternates the parameter type to a "CheckOut"
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic data = okResult.Value!;
            
            string checkAction = data.GetType().GetProperty("Action").GetValue(data, null).ToString();
            Assert.Equal("CheckOut", checkAction);

            // Double check data tracking states inside the database framework index
            var actualRecords = await context.AttendanceRecords.OrderByDescending(a => a.Timestamp).ToListAsync();
            Assert.Equal(2, actualRecords.Count);
            Assert.Equal("CheckOut", actualRecords[0].ActionType);
        }
    }
}
