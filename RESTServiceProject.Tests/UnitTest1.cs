using System.ComponentModel;

namespace RESTServiceProject.Tests
{
    public class Token
    {
        [JsonPropertyName("token")]
        public string TokenString { get; set; }
    }


    
    public class UserTests
    {
        HttpClient client;

        [SetUp]
        public void SetUp()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5000/api/");

            var tokenResult = client.GetAsync("token?email=email&password=password").Result;
            var tokenJson = tokenResult.Content.ReadAsStringAsync().Result;
            var token = JsonSerializer.Deserialize<Token>(tokenJson);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.TokenString);
        }

        [Test]
        public async Task AddNewUser()
        {
            var newUser = new User
            {
                email = "email",
                userPassword = "password"
            };

            var newJson = JsonSerializer.Serialize(newUser);

            var postContent = new StringContent(newJson,
                Encoding.UTF8, "application/json");

            var postResult = await client.PostAsync("users", postContent);

            Assert.AreEqual(HttpStatusCode.Created, postResult.StatusCode);

            postResult.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task TestGetAll()
        {
            var result = await client.GetAsync("users");

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task TestDelete_ValidUserId()
        {
            var postResult = await CreateUser("TestDelete_ValidUserEmail");

            var json = await postResult.Content.ReadAsStringAsync();

            var user = JsonSerializer.Deserialize<Contact>(json);

            var result = await client.DeleteAsync("users/" + user.Id);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task TestDelete_InvalidUserStringId()
        {
            var result = await client.DeleteAsync("users/invalid");

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task TestDelete_InvalidUserId()
        {
            var result = await client.DeleteAsync("users/1");

            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task TestGetSpecific_Good()
        {
            var postResult = await CreateUser("TestGetSpecific_Good");

            var json = await postResult.Content.ReadAsStringAsync();

            var user = JsonSerializer.Deserialize<User>(json);

            var result = await client.GetAsync("users/" + user.Id);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task TestGetSpecific_Bad()
        {
            var result = await client.GetAsync("users/111111");

            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task TestAddNewUser()
        {
            var postResult = await CreateUser("AddNewUserTest");

            Assert.AreEqual(HttpStatusCode.Created, postResult.StatusCode);

            postResult.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task TestAddNewUserEmailNull()
        {
            var postResult = await CreateUser(null);

            postResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task TestAddNewUserEmailEmpty()
        {
            var postResult = await CreateUser("");

            postResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        private async Task<HttpResponseMessage> CreateUser(string email)
        {
            var newUser = new User
            {
                email = email,
                userPassword = "password"
            };

            var newJson = JsonSerializer.Serialize(newUser);
            var postContent = new StringContent(newJson, Encoding.UTF8, "application/json");
            var postResult = await client.PostAsync("users", postContent);

            return postResult;
        }

      
    }