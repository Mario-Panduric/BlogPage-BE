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
    public class PostsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PostsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Posts>>> GetPosts()
        {
            var posts = await _context.Posts.Where(c => c.isActive).ToListAsync();
            
            return this.Ok(_mapper.Map<List<GetPostDto>>(posts));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetPostWithCommentsDto>> GetPost(int id)
        {
            var post = await _context.Posts
                .Include(x => x.Comments)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(c => c.Id == id && c.isActive);
   
     
            if (post == null)
            {
                return NotFound();
            }

            return _mapper.Map<GetPostWithCommentsDto>(post);
        }

        [HttpPost]
        [Route("Post")]
        public async Task<ActionResult<Posts>> CreatePost(PostsDto newPost)
        {
            var post = _mapper.Map<Posts>(newPost);
            _context.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreatePost),
                    _mapper.Map<PostsDto>(post));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditPost(int id, PostsDto updatedPost)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == id && c.isActive);

            if (post == null)
            {
                return NotFound();
            }

            _mapper.Map(updatedPost, post);
            post.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == id && c.isActive);
            if (post == null)
            {
                return NotFound();
            }

            post.isActive = false;
            post.DeletedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
