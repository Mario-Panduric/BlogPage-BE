using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using AutoMapper;
using WebApplication5.DTOs;
using WebApplication5.Entities;

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CommentsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsersCommentsDto>>> GetComments()
        {
            var comments = await _context.UsersComments.Where(c => c.isActive).ToListAsync();
            return this.Ok(this._mapper.Map<List<UsersCommentsDto>>(comments));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsersCommentsDto>> GetComment(int id)
        {
            var comment = await _context.UsersComments.FirstOrDefaultAsync(c => c.Id == id && c.isActive);

            if (comment == null)
            {
                return NotFound();
            }

            return _mapper.Map<UsersCommentsDto>(comment);
        }

        [HttpPost]
        [Route("Comment")]
        public async Task<ActionResult<UsersComments>> CreateComment(SubmitCommentDto newComment)
        {
            var comment = _mapper.Map<UsersComments>(newComment);
            _context.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateComment),
                    _mapper.Map<SubmitCommentDto>(comment));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditComment(int id, UsersCommentsDto updatedComment)
        {
            var comment = await _context.UsersComments.FirstOrDefaultAsync(c => c.Id == id && c.isActive);

            if (comment == null)
            {
                return NotFound();
            }

            _mapper.Map(updatedComment, comment);
            comment.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.UsersComments.FirstOrDefaultAsync(c => c.Id == id && c.isActive);
            if (comment == null)
            {
                return NotFound();
            }

            comment.isActive = false;
            comment.DeletedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
