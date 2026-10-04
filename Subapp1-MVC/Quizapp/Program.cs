using Microsoft.EntityFrameworkCore;
using Quizapp.DAL;

var builder = WebApplication.CreateBuilder(args);

// Registrerer MVC med controllers og views.
builder.Services.AddControllersWithViews();

// Registrerer databasen og kobler den til SQLite.
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("QuizDbContext")));

// Registrerer repository slik at det kan brukes i controllers.
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

var app = builder.Build();

// Viser en detaljert feilside når prosjektet kjøres i utviklingsmiljø.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Gjør filer i wwwroot tilgjengelige, for eksempel CSS, JavaScript og bilder.
app.UseStaticFiles();

// Klargjør ruting mellom URL-er og controllers.
app.UseRouting();

// Klargjør autorisasjon dersom det legges til senere.
app.UseAuthorization();

// Oppretter databasen, kjører migrations og legger inn testdata.
DBInit.Seed(app);

// Bruker standardruten: Controller/Action/Id.
app.MapDefaultControllerRoute();

app.Run();