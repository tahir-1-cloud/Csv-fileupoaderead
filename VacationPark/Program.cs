using Microsoft.Extensions.DependencyInjection;
using VacationPark.BusinesServices;
using VacationPark.DBConnection;
using VacationPark.Interface;
using VacationPark.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped(provider => new DatabaseContext(connectionString));

// Register Repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IFacilityRepository, FacilityRepository>();
builder.Services.AddScoped<IParkRepository, ParkRepository>();
builder.Services.AddScoped<IHouseRepository, HouseRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
