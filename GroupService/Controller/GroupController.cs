using Domain.DTO.Group;
using Microsoft.AspNetCore.Mvc;

namespace GroupService.Controller;

[ApiController]
[Route("group")]
public class GroupController : ControllerBase {

    /* NOT IN THE INITIAL REQUIREMENTS, MIGHT BE USED LATER
    [HttpGet("/{groupId}/members")]
    public IActionResult AllMembersFromGroup([FromRoute] int groupId) {
        if(groupId == 1) {
            return Ok("yo mama says hi");
        }
        return BadRequest("No bueno, to test this endpoint OK result insert 1 for group id");
    }
    */
    
    [HttpGet("{userId}/groups")] //group/1/groups
    public IActionResult GetAllGroupsOfUser([FromRoute] int userId) {
        if(userId == 1) {
            return Ok("yo mama says hi");
        }
        return BadRequest("No bueno, to test this endpoint OK result insert 1 for user id");
    }
    
    [HttpPost]
    public IActionResult Create([FromBody] PostGroup request) {
        if(request.Name == "admin") {
            return Ok("Group created");
        }
        return BadRequest("No bueno, to test this endpoint OK result insert 'admin' for name");
    }
    
    [HttpPost("join")]
    public IActionResult Join([FromBody] JoinGroupReq request) {
        if(request.GroupId == 1 && request.UserId == 1) {
            return Ok("User joined group");
        }
        return BadRequest("No bueno, to test this endpoint OK result insert 1 and 1 for both ids");
    }
}
