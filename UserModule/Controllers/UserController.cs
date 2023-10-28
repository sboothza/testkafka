using Microsoft.AspNetCore.Mvc;
using UserModule.Interfaces;
using UserModuleCommon;

namespace UserModule.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _repository;

    public UserController(ILogger<UserController> logger, IUserRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet(template: "")]
    //[HttpGet]
    //[Route("")]
    public IEnumerable<User> Get()
    {
        return _repository.GetAll();
    }

    [HttpGet(template: "{id}")]
    //[HttpGet]
    //[Route("{id}")]
    public User GetOne(Guid id)
    {
        return _repository.Get(id);
    }

    [HttpPost]
    public void Add(User user)
    {
        _repository.Add(user.Id, user.Username!, user.Name!);
    }
    
    [HttpPut]
    public void Update(User user)
    {
        _repository.Update(user.Id, user.Username!, user.Name!);
    }

    [HttpDelete]
    [Route("{id}")]
    public void Delete(Guid id)
    {
        _repository.Delete(id);
    }
}