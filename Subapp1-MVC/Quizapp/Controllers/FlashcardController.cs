using Microsoft.AspNetCore.Mvc;
using Quizapp.DAL;
using Quizapp.Models;

namespace Quizapp.Controllers
{
    // Denne controlleren håndterer alt som har med flashcards å gjøre:
    // vise, opprette, endre, slette og øve på kortene.
    public class FlashcardController : Controller
    {
        private readonly QuizDbContext _db;

        // Databasekonteksten blir gitt automatisk via dependency injection (registrert i Program.cs).
        public FlashcardController(QuizDbContext db)
        {
            _db=db;
        }

        // Viser en liste over alle flashcards.
        public IActionResult Index()
        {
            List<Flashcard> flashcards= _db.Flashcards.ToList();
            return View(flashcards);
        }

        // Viser et tomt skjema for å lage et nytt flashcard.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Tar imot skjemaet og lagrer det nye kortet i databasen.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Flashcard flashcard)
        {
            // Viser skjemaet på nytt med feilmeldinger hvis noe mangler eller er for langt.
            if (!ModelState.IsValid)
            {
                return View(flashcard);
            }

            _db.Flashcards.Add(flashcard);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Henter kortet som skal endres og viser skjemaet ferdig utfylt.
        [HttpGet]
        public IActionResult Update(int id)
        {
            var flashcard = _db.Flashcards.Find(id);

            // Gir 404 hvis det ikke finnes et kort med denne ID-en.
            if (flashcard == null)
            {
                return NotFound();
            }
            return View(flashcard);
        }

        // Lagrer endringene på et eksisterende kort.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Flashcard flashcard)
        {
            if (!ModelState.IsValid)
            {
                return View(flashcard);
            }

            _db.Flashcards.Update(flashcard);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Viser en bekreftelsesside før kortet slettes.
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var flashcard = _db.Flashcards.Find(id);
            if (flashcard == null)
            {
                return NotFound();
            }
            return View(flashcard);
        }

        // Sletter kortet etter at brukeren har bekreftet.
        // Heter DeleteConfirmed fordi Delete(int id) allerede finnes med samme parameter.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var flashcard = _db.Flashcards.Find(id);
            if (flashcard == null)
            {
                return NotFound();
            }

            _db.Flashcards.Remove(flashcard);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Øvemodus: viser ett kort om gangen. "index" bestemmer hvilket kort som vises (0 = første).
        [HttpGet]
        public IActionResult Study(int index = 0)
        {
            // Sorterer etter ID så rekkefølgen er lik hver gang.
            List<Flashcard> flashcards = _db.Flashcards
                .OrderBy(f => f.FlashcardId)
                .ToList();

            // Ingen kort å øve på, så brukeren sendes tilbake til listen.
            if (flashcards.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            // Starter på første kort igjen etter det siste (eller hvis index er ugyldig).
            if (index < 0 || index >= flashcards.Count)
            {
                index = 0;
            }

            // Sender posisjon og antall kort til viewet, så det kan vise "Card 2 of 5".
            ViewBag.Index = index;
            ViewBag.Total = flashcards.Count;
            return View(flashcards[index]);
        }
    }
}
