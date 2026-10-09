using Api.Presenters;
using Application;
using Infrastructure;
using Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddSingleton(TimeProvider.System);

// Gateway (Infrastructure)
builder.Services.AddSingleton<IRoomRepository, InMemoryRoomRepository>();
builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
builder.Services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();

// Use case (Input Boundary)
builder.Services.AddScoped<ISearchAvailableRoomsInputBoundary, SearchAvailableRoomsUseCase>();
builder.Services.AddScoped<ICreateBookingInputBoundary, CreateBookingUseCase>();
builder.Services.AddScoped<IGetBookingInputBoundary, GetBookingUseCase>();
builder.Services.AddScoped<ICancelBookingInputBoundary, CancelBookingUseCase>();

// Presenter (Output Boundary): cùng 1 instance cho use case và controller trong mỗi request
builder.Services.AddScoped<SearchRoomsPresenter>();
builder.Services.AddScoped<ISearchAvailableRoomsOutputBoundary>(sp => sp.GetRequiredService<SearchRoomsPresenter>());
builder.Services.AddScoped<CreateBookingPresenter>();
builder.Services.AddScoped<ICreateBookingOutputBoundary>(sp => sp.GetRequiredService<CreateBookingPresenter>());
builder.Services.AddScoped<GetBookingPresenter>();
builder.Services.AddScoped<IGetBookingOutputBoundary>(sp => sp.GetRequiredService<GetBookingPresenter>());
builder.Services.AddScoped<CancelBookingPresenter>();
builder.Services.AddScoped<ICancelBookingOutputBoundary>(sp => sp.GetRequiredService<CancelBookingPresenter>());

var app = builder.Build();
app.MapControllers();
app.Run();