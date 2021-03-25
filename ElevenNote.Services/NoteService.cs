using ElevenNote.Data;
using ElevenNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services
{
    public class NoteService
    {
        private readonly Guid _userId;
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public NoteService(Guid userId) => _userId = userId;
        

        public bool CreateNote(NoteCreate model)
        {
            var entity = new Note()
            {
                OwnerId =   _userId,
                Title =     model.Title,
                Content =   model.Content,
                CreatedUtc = DateTimeOffset.Now
            };

            _context.Notes.Add(entity);
            return _context.SaveChanges() == 1;
        }



        public IEnumerable<NoteListItem> GetNotes()
        {
            var query = _context.Notes.Where(x => x.OwnerId == _userId)
                                        .Select(x => new NoteListItem
                                        {
                                            NoteId = x.NoteId,
                                            Title = x.Title,
                                            CreatedUtc = x.CreatedUtc
                                        });
            return query.ToArray();
        }

        public NoteDetail GetNoteById(int id)
        {
            var entity = _context.Notes.Single(x => x.NoteId == id &&
                                                    x.OwnerId == _userId);
            return new NoteDetail
            {
                NoteId =        entity.NoteId,
                Title =         entity.Title,
                Content =       entity.Content,
                CreatedUtc =    entity.CreatedUtc,
                ModifiedUtc =   entity.ModifiedUtc
            };
        }

        public bool UpdateNote(NoteEdit model)
        {
            var entity = _context.Notes.Single(x=>  x.NoteId == model.NoteId && 
                                                    x.OwnerId == _userId);
            entity.Title =      model.Title;
            entity.Content =    model.Content;
            entity.ModifiedUtc = DateTimeOffset.UtcNow;

            return _context.SaveChanges() == 1;
        }
    }
}
