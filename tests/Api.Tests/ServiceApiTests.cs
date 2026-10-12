using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Application;
using Domain;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Api.Tests;

public sealed class ServiceApiTests : IDisposable
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    public ServiceApiTests()
    {
        connection.Open();
        factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<HotelDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<HotelDbContext>>();
                services.AddDbContext<HotelDbContext>(o => o.UseSqlite(connection));
            });
        });
        client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<HotelDbContext>().Database.EnsureCreated();
    }

    private static SaveServiceRequest Request(string name = "Airport transfer", decimal price = 250_000,
        string description = "Transfer", string status = "Active") =>
        new(name, price, description, status);

    [Fact]
    public async Task List_filter_and_crud_persist_across_requests()
    {
        var all = await client.GetFromJsonAsync<List<ServiceDetailsDto>>("/api/services");
        Assert.Equal(4, all!.Count);
        Assert.Equal(4, all.Select(s => s.Name).Distinct().Count());
        var active = (await client.GetFromJsonAsync<List<ServiceDetailsDto>>("/api/services?status=Active"))!;
        Assert.All(active, s => Assert.Equal("Active", s.Status));
        Assert.Equal(HttpStatusCode.BadRequest, await client.GetAsync("/api/services?status=unknown").ContinueWith(t => t.Result.StatusCode));

        var createdResponse = await client.PostAsJsonAsync("/api/services", Request());
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        var created = (await createdResponse.Content.ReadFromJsonAsync<ServiceDetailsDto>())!;
        Assert.Equal($"/api/services/{created.Id}", createdResponse.Headers.Location!.ToString());
        var url = createdResponse.Headers.Location.ToString();
        Assert.Equal(created, await client.GetFromJsonAsync<ServiceDetailsDto>(url));

        var updatedResponse = await client.PutAsJsonAsync(url, Request("Airport transfer updated", 300_000, "Updated", "Inactive"));
        Assert.Equal(HttpStatusCode.OK, updatedResponse.StatusCode);
        Assert.Equal("Airport transfer updated", (await updatedResponse.Content.ReadFromJsonAsync<ServiceDetailsDto>())!.Name);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync(url)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(url)).StatusCode);
    }

    [Fact]
    public async Task Invalid_input_missing_members_and_rejected_update_are_400()
    {
        foreach (var request in new[] { Request("", 1), Request("   ", 1), Request("valid", 0), Request("valid", -1), Request("valid", 1, status: "active"), Request("valid", 1, new string('x', 501)) })
            Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/services", request)).StatusCode);

        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/api/services", new StringContent("{}", Encoding.UTF8, "application/json"))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/api/services", new StringContent("{broken", Encoding.UTF8, "application/json"))).StatusCode);
        var before = await client.GetFromJsonAsync<ServiceDetailsDto>("/api/services/1");
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PutAsJsonAsync("/api/services/1", Request("changed", 0))).StatusCode);
        Assert.Equal(before, await client.GetFromJsonAsync<ServiceDetailsDto>("/api/services/1"));
    }

    [Fact]
    public async Task Duplicate_names_are_case_insensitive_but_own_name_is_allowed()
    {
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/services", Request("MINIBAR"))).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PutAsJsonAsync("/api/services/1", Request("minibar"))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync("/api/services/1", Request("Giặt là"))).StatusCode);
    }

    [Fact]
    public async Task Missing_service_is_404_for_get_update_and_delete()
    {
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/services/99999")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PutAsJsonAsync("/api/services/99999", Request())).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync("/api/services/99999")).StatusCode);
    }

    [Fact]
    public async Task Used_service_cannot_be_deleted_but_unused_service_can()
    {
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
            var booking = Booking.Create(1, 1, DateOnly.FromDateTime(DateTime.Today).AddDays(30), DateOnly.FromDateTime(DateTime.Today).AddDays(32), 1, 0, 1);
            db.Bookings.Add(booking);
            await db.SaveChangesAsync();
            db.ServiceUsages.Add(new ServiceUsage { BookingId = booking.Id, ServiceId = 1, AccountId = 1, Quantity = 1, UnitPrice = 50_000, UsedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();
        }

        Assert.Equal(HttpStatusCode.Conflict, (await client.DeleteAsync("/api/services/1")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/services/1")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync("/api/services/2")).StatusCode);
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
        connection.Dispose();
    }
}
