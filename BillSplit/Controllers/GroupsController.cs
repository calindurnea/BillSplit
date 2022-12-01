using Microsoft.AspNetCore.Mvc;

namespace BillSplit.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupsController : ControllerBase
{
    // GET: api/<GroupsController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<GroupsController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<GroupsController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<GroupsController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<GroupsController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
