using System.Security.Claims;
using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotesController(NoteDbContext context) : ControllerBase
    {
        private readonly NoteDbContext _context = context;
        
        private Guid GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdString, out Guid paseGuid)) return paseGuid;
            throw new UnauthorizedAccessException("Invalid token.");
        }
        
        [HttpGet]
        public async Task<ActionResult<List<Note>>> GetNotes()
        {
            var userId = GetUserId();
            var userNotes = await _context.Notes.Where(n => n.userId == userId).ToListAsync();

            // return Ok(await _context.Notes.ToListAsync());
            return Ok(new
            {
                result = true,
                message = "Get all notes successfull",
                data = userNotes
            });
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Note>> GetNoteById(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            return note == null ? NotFound() : Ok(new
            {
                result = true,
                message = "Get note successfull",
                data = note
            });
        }

        [HttpPost]
        public async Task<ActionResult<Note>> CreateNote(Note newNote)
        {
            if (newNote is null) return BadRequest();

            newNote.userId = GetUserId();
            _context.Notes.Add(newNote);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNoteById), new { id = newNote.id }, new
            {
                result = true,
                message = "Note created successfull",
                data = newNote
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateNote(int id, Note updatedNote)
        {
            var note = await _context.Notes.FindAsync(id);

            if (updatedNote is null) return BadRequest();
            if (note is null) return NotFound();

            note.title = updatedNote.title;
            note.content = updatedNote.content;
            note.createdAt = note.createdAt;
            note.updatedAt = updatedNote.updatedAt;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                result = true,
                message = "Note updated successfull"
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note is null) return NotFound(new
            {
                result = false,
                message = "Note not found"
            });
            _context.Remove(note);
            await _context.SaveChangesAsync();
            
            return Ok(new
            {
                result = true,
                message = "Note deleted successfull"
            });
        }
    }

}
