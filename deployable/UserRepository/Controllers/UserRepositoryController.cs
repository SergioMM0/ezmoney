using Messages.User.Request;
using Messages.User.Response;
using Microsoft.AspNetCore.Mvc;
using UserRepository.Service;

namespace UserRepository.Controllers; 

[ApiController]
[Route("[controller]")]
public class UserRepositoryController : ControllerBase{
    private readonly UserRepositoryService _userRepositoryService;
    public UserRepositoryController(UserRepositoryService userRepositoryService){
        _userRepositoryService = userRepositoryService;
    }
    
    [HttpGet("GetUserByPhoneNumber")]
    public ActionResult<UserResponse> GetUserByPhoneNumber(string phoneNumber){
        try{
            GetUserByPhone getUserByPhone = new GetUserByPhone(){
                PhoneNumber = phoneNumber
            };
            return Ok(_userRepositoryService.GetUserByPhoneNumber(getUserByPhone));
        } catch (Exception e){
            return BadRequest(e.Message);
        }
    }
    [HttpGet("GetAllUsers")]
    public ActionResult<List<UserResponse>> GetAllUsers(){
        try{
            return Ok(_userRepositoryService.GetAllUsers());
        } catch (Exception e){
            return BadRequest(e.Message);
        }
    }
    [HttpPost("AddUser")]
    public ActionResult<UserResponse> AddUser(CreateUserReq request){
        try{
            return Ok(_userRepositoryService.AddUser(request));
        } catch (Exception e){
            return BadRequest(e.Message);
        }
    }
}
