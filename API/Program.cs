/* IMPORTANT NOTES:
The "Program.cs" is the entry point to a dotnet application.
- The code here is read on startup.

- In order for the app to run, something has to host and sever the content inside our folder.
- Thats whats going on inside this class
*/


using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);
/*NOTES:
The "builder" variable is used to do a bunch of things. Such as
 - Create a kestrel server
    - Kestrel server:  Kestrel server is a lightweight / cross-platform web server that is used to host ASP.NET Core applications.
 - Read from the configuration files, any configs passed to it (the "args")
    - appsettings.Development.json (takes priority bc its env specific)
    - appsettings.json
*/


// Add services to the container.
/*NOTES: 
- Services ^ are things that can be used to give more functionality inside our applications logic.
    - We will be using Dependency Injection to inject these services to other classes inside our code

- Any services we add will go inside this section ^.
*/


builder.Services.AddControllers(); // Adds services on the controllers.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // Registers the API Explorer services in the applications dependency injection container
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(opt =>
{
  opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add CORS Service which contains our policy
builder.Services.AddCors(opt => {
    opt.AddPolicy("CorsPolicy", policy => {
        policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
    });
});

var app = builder.Build();



// MIDDLEWARE (BELOW) NOTE: Order Matters
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
// Cors Middleware to attach the policy to the response. Notes: The "CorsPolicy" string must match the Cors Service string above. We want this to be ran fairly early in the pipeline process.
app.UseCors("CorsPolicy");

/*NOTES:
- The HTTP Request pipeline is essentially middleware.

- Any middleware we add will go inside this section ^.
*/

// app.UseHttpsRedirection(); // not using https, so removed

app.UseAuthorization(); // used for authenticating the app. Not set up rn

app.MapControllers(); // when requests come in, points where they should be sent

// I need to get access to my datacontext service.
using var scope = app.Services.CreateScope(); 
// Notes: The using statement means that whatever is followed (its scope) is going to be disposed immediately.
// The point of creating this temporary scope is so when a request comes in, we can have access to the dbContext, and immediately dispose of it.
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
  logger.LogError(ex, "An error occured during migration");
}

app.Run(); // runs the application

// NOTES: If I run the app "dotnet run", I can see the default application at localhost:5000/swagger
