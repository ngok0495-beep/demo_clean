using Api.Presenters;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Gateway (Infrastructure)
builder.Services.AddSingleton<IRoomRepository, InMemoryRoomRepository>();
builder.Services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();

// Use case: Input Boundary -> cài đặt
builder.Services.AddScoped<ISearchAvailableRoomsInputBoundary, SearchAvailableRoomsUseCase>();
builder.Services.AddScoped<ICreateBookingInputBoundary, CreateBookingUseCase>();

// Presenter: Output Boundary -> cài đặt
// Đăng ký scoped và dùng CHUNG 1 instance cho controller lẫn use case trong cùng request
builder.Services.AddScoped<SearchRoomsPresenter>();
builder.Services.AddScoped<ISearchAvailableRoomsOutputBoundary>(sp => sp.GetRequiredService<SearchRoomsPresenter>());
builder.Services.AddScoped<CreateBookingPresenter>();
builder.Services.AddScoped<ICreateBookingOutputBoundary>(sp => sp.GetRequiredService<CreateBookingPresenter>());

var app = builder.Build();
app.MapControllers();
app.Run();