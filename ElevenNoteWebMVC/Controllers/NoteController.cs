using ElevenNote.Models;
using ElevenNote.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElevenNoteWebMVC.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private NoteService VerifyUserThenGetService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new NoteService(userId);
            return service;
        }

        private ActionResult GetNoteById(int id)
        {
            NoteService service = VerifyUserThenGetService();
            var model = service.GetNoteById(id);
            return View(model);
        }

        public ActionResult Create() => View();

        public ActionResult Index()
        {
            NoteService service = VerifyUserThenGetService();
            var model = service.GetNotes(); 
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NoteCreate model)
        {
            if (!ModelState.IsValid) return View(model);
            NoteService service = VerifyUserThenGetService();            
            
            if (service.CreateNote(model))
            {
                TempData["SaveResult"] = "Your note was created.";
                return RedirectToAction("Index");
            };

            ModelState.AddModelError("", "Note could not be created.");

            return View(model);
        }

        public ActionResult Details(int id)
        {
            return GetNoteById(id);
        }

        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            return GetNoteById(id);
        }


        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            var service = VerifyUserThenGetService();

            service.DeleteNote(id);

            TempData["SaveResult"] = "Your note was deleted";

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            NoteService service = VerifyUserThenGetService();
            var detail = service.GetNoteById(id);
            var model = new NoteEdit
            {
                NoteId = detail.NoteId,
                Title = detail.Title,
                Content = detail.Content
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, NoteEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.NoteId != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }
            var service = VerifyUserThenGetService();

            if (service.UpdateNote(model))
            {
                TempData["SaveResult"] = "Your note was updated.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your note could not be updated.");
            return View();
        }
    }
}