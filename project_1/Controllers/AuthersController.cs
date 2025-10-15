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
    public class AuthorsController(IAuthorService authors) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll(CancellationToken ct = default) =>
            Ok(await authors.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuthorDto>> GetById(int id, CancellationToken ct = default)
            => (await authors.GetByIdAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

        [HttpPost]
        public async Task<ActionResult<AuthorDto>> Create([FromBody] AuthorCreateDto input, CancellationToken ct = default)
        {
            var dto = await authors.CreateAsync(input, ct);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDto input, CancellationToken ct = default)
            => await authors.UpdateAsync(id, input, ct) ? NoContent() : NotFound();

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
            => await authors.DeleteAsync(id, ct) ? NoContent() : NotFound();
    }
}
