using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Data;
using System.Security.Claims;


[ApiController]
[Route("todos")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly AppDbContext _db;

    public TodoController(AppDbContext db)
    {
        _db = db;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public IActionResult Get() => Ok(_db.Todos.Where(t => t.UserId == GetUserId()).ToList());

    [HttpPost]
    public IActionResult Create(Todo todo)
    {
        todo.UserId = GetUserId();
        _db.Todos.Add(todo);
        _db.SaveChanges();
        return Ok(todo);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Todo updated)
    {
        var todo = _db.Todos.FirstOrDefault(t => t.Id == id && t.UserId == GetUserId());
        if (todo == null) return NotFound();

        todo.Title = updated.Title;
        todo.IsDone = updated.IsDone;
        _db.SaveChanges();
        return Ok(todo);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var todo = _db.Todos.FirstOrDefault(t => t.Id == id && t.UserId == GetUserId());
        if (todo == null) return NotFound();

        _db.Todos.Remove(todo);
        _db.SaveChanges();
        return Ok();
    }
}
