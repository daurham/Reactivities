# Steps
1. In API remove fluff from launchSettings.json (only leave the http and schema lines)
2. In API remove https redirection in Program.cs
3. Read over Program and know what each line is roughly doing. At least the section.
4. Disable the Nullable property in all .csproj files.
5. In Domain remove the default Class.cs and create a new one named Activity.cs, where the Domain Entity will be
6. Add the Activity.cs properties (Make sure the Id has the name "Id" so entity framework knows how to ready it)
7. Now install microsoft.entityFrameworkCore.Sqlite via Nutget Gallery
   1. Make sure you install it for the Persistence.csproj, but be sure you match the .NET version
8. Create the "DataContext.cs" class in the Persistence project
9. Derive the class from the Microsoft.EnitityFrameworkCore.DbContext base class.
10. Create the DataContext Constructor
    1.  It will take options and return derive from a base
        1.      public DataContext(DbContextOptions options) : base(options){}
11. Create the DataContext Property "Activities", using the type DbSet<Activity> 
12. Connect this to our Program and add it as a service
    1.  builder.Services.AddDbContext<DataContext>(opt =>{ opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")); });
        1.  May need to restore dotnet to access from one project to another. In termianl, run "dotnet restore".
13. Add credentials to appsettings.Development.json
    1.  After "Logging" {}, add
        1.  "ConnectionStrings": { "DefaultConnection": "Data Source=reactivities.db" }
14. Make sure you have "dotnet ef" installed and are using the current version
    1.  https://www.nuget.org/packages/dotnet-ef/
    2.  copy the latest version that matches your .NET runtime
    3.  install or update:
    4.  dotnet tool install --global dotnet-ef --version 7.0.9
    5.  or
    6.  dotnet tool update --global dotnet-ef --version 7.0.9
15. Run: dotnet ef migrations add InitialCreate -s API -p Persistence
    1.  dotnet ef migrations add [migrationName] -s [localPathToStartUpProject] -p [localPathToProjectWhereDataContextResides]
    2.  dotnet ef migrations add InitialCreate -s API -p Persistence
16. Getting an error, so install "Microsoft.EntityFrameworkCore.Design" into the API project
    1.  If getting an error, make sure the API isnt running.
17. Added This to Program.cs, JUST above `app.Run()`
```cs
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
```

## Slowed down taking notes by Section2. Video12: "Creating the Database"

18. Hot reload creates issues, so use: dotnet watch --no-hot-reload
    1.  This will include a file watcher, that will restart when we make changes.

19. Added my base API controller
```cs
// BaseAPIController.cs
using Microsoft.AspNetCore.Mvc;
/* IMPORTANT NOTE:
    The purpose of this file is to avoid having to retype these attributes on every controller. 
    Therefore, I derive all new classes from this one.
*/

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseAPIController
    {
        
    }
}
```
20.  Added an activities API controller
```cs
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ActivitiesController : BaseAPIController
    {
        private readonly DataContext _context;
        public ActivitiesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet] // api/activities
        public async Task<ActionResult<List<Activity>>> GetActivities() {
            return await _context.Activities.ToListAsync();
        }

        [HttpGet("{id}")] // defining a route param: // api/activities/someIdAbcdefg
        public async Task<ActionResult<Activity>> GetActivity(Guid id) {
            return await _context.Activities.FindAsync(id);
        }
    }
}
```
21. Begin Source Control
    1.  git init
    2.  dotnet new gitignore
    3.  add "appsettings.json", "*.db-shm", "*.db-wal" to gitignore
    4.  git add .
    5.  git ci -m'initial commit'
    6.  add remote branch


22. Create a react app
    1.  npx create-react-app client-app --use-npm --template typescript
23. Install axios
24. on App.tsx, create a state for fetching activities and use a useEffect to fetch activities from your c# application:
```tsx
function App() {
  const [activities, setActivities] = useState([]);

  useEffect(() => {
    axios.get("http://localhost:5000/api/activities")
      .then(res => {
        console.log(res.data);
        setActivities(res.data);
      })
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <ul>
          {activities.length && activities.map((activity: any) => (
            <li key={activity.id}>
              {activity.title
              }</li>
          ))}
        </ul>
      </header>
    </div>
  );
}

export default App;
```
25. There will be an error due to a CORS Policy issue.
    1.  To bypass, I need to attach a header giving permissions:
        1.  In Program.cs, add the CORS as a service the middleware to attach the CORS to the response going out to our client.
```cs
...
...
// Add CORS Service which contains our policy
builder.Services.AddCors(opt => {
    opt.AddPolicy("CorsPolicy", policy => {
        policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
    });
});

var app = builder.Build();
...
```
```cs
...
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
// Cors Middleware to attach the policy to the response. Notes: The "CorsPolicy" string must match the Cors Service string above. We want this to be ran fairly early in the pipeline process.
app.UseCors("CorsPolicy");
...
```
- This will solve the CORSPolicy issue
26. Install semantic-ui for react and style the react app.
```tsx
function App() {
  const [activities, setActivities] = useState([]);

  useEffect(() => {
    axios.get("http://localhost:5000/api/activities")
      .then(res => {
        console.log(res.data);
        setActivities(res.data);
      })
  }, []);

  return (
    <div>
      <Header as='h2' icon='users' content='Reactivities' />
      <List>
        {activities.length && activities.map((activity: any) => (
          <List.Item key={activity.id}>
            {activity.title
            }</List.Item>
        ))}
      </List>
    </div>
  );
}
```
27. remove React.StrictMode on index.tsx
