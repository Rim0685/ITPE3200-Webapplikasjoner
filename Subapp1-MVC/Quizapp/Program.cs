using Microsoft.EntityFrameworkCore;
using Quizapp.DAL;

var builder = WebApplication.CreateBuilder(args);

// Registers MVC with controllers and views.
builder.Services.AddControllersWithViews();

// Registers the database and connects it to SQLite.
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("QuizDbContext")));

// Registers the repository so it can be used in controllers.
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

var app = builder.Build();

// Shows a detailed error page when the project runs in the development environment.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Makes files in wwwroot available, for example CSS, JavaScript and images.
app.UseStaticFiles();

// Sets up routing between URLs and controllers.
app.UseRouting();

// Sets up authorization in case it is added later.
app.UseAuthorization();

// Creates the database, runs migrations and adds seed data.
DBInit.Seed(app);

// Uses the default route: Controller/Action/Id.
app.MapDefaultControllerRoute();

app.Run();