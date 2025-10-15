using Microsoft.AspNetCore.Mvc;
using project_1.Dtos;
using project_1.Services;

namespace project_1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController(IGenreService svc) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreDto>>> Get(CancellationToken ct = default)
            => Ok(await svc.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GenreDto>> GetById(int id, CancellationToken ct = default)
            => (await svc.GetByIdAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

        [HttpPost]
        public async Task<ActionResult<GenreDto>> Create(GenreCreateDto input, CancellationToken ct = default)
        {
            var dto = await svc.CreateAsync(input, ct);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, GenreUpdateDto input, CancellationToken ct = default)
            => await svc.UpdateAsync(id, input, ct) ? NoContent() : NotFound();

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
            => await svc.DeleteAsync(id, ct) ? NoContent() : NotFound();
    }
}
