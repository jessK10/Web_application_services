using Microsoft.AspNetCore.Mvc;
using project_1.Dtos;
using project_1.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace project_1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController(IBookService books) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll(CancellationToken ct = default) =>
            Ok(await books.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDto>> GetById(int id, CancellationToken ct = default)
            => (await books.GetByIdAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

        [HttpPost]
        public async Task<ActionResult<BookDto>> Create([FromBody] BookCreateDto input, CancellationToken ct = default)
        {
            try
            {
                var dto = await books.CreateAsync(input, ct);
                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message); // invalid AuthorId
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDto input, CancellationToken ct = default)
            => await books.UpdateAsync(id, input, ct) ? NoContent() : NotFound();

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
            => await books.DeleteAsync(id, ct) ? NoContent() : NotFound();
    }
}
