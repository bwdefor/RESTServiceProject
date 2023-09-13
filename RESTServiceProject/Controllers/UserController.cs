using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using RESTServiceProject.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RESTServiceProject.Controllers
{
    [Route("api/UserController")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static int currentId = 1;
        private static List<User> users = new List<User>();

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(users);
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = users.FirstOrDefault(t => t.id == id);

            if (user == null)
            {
                return NotFound(null);
            }
            else
            {
                return Ok(user);
            }
        }

        // POST api/<ValuesController>
        [Authenticator]
        [HttpPost]
        public IActionResult Post([FromBody] User value)
        {
            if (string.IsNullOrEmpty(value.email))
            {
                var errorResponse = new ErrorResponse
                {
                    DBCode = ErrorType.MissingField,
                    Message = "Null or Empty Email",
                    FieldName = "Email",
                };
                return BadRequest(errorResponse);
            }

            if (string.IsNullOrEmpty(value.password))
            {
                var errorResponse = new ErrorResponse
                {
                    DBCode = ErrorType.MissingField,
                    Message = "Null or Empty Password",
                    FieldName = "Password",
                };
                return BadRequest(errorResponse);
            }

            value.id = currentId++;
            value.dateAdded = DateTime.UtcNow;

            users.Add(value);

            return CreatedAtAction(nameof(Get), new { id = value.id }, value);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] User value)
        {
            var user = users.FirstOrDefault(t => t.id == id);

            if (user == null)
            {
                return NotFound(null);
            }

            user.email = value.email;
            user.password = value.password;

            return Ok(user);
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var rowsDeleted = users.RemoveAll(t => t.id == id);

            if (rowsDeleted == 0)
            {
                return NotFound(null);
            }
            else
            {
                return Ok();
            }
        }
    }
}
