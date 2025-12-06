using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServerCore.PortalAPI.Services;

namespace ServerCore.PortalAPI.Controllers.Admin
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "admin")]
    [SwaggerTag("User Group Management APIs")]
    public class GroupController : ControllerBase
    {
        private readonly IAdminDbService _dbService;

        public GroupController(IAdminDbService dbService)
        {
            _dbService = dbService;
        }

        /// <summary>
        /// Get all user groups
        /// </summary>
        [HttpGet("Index")]
        [SwaggerOperation(Summary = "Get all user groups")]
        [SwaggerResponse(200, "Success", typeof(List<GroupInfo>))]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Placeholder for actual DB call when setup is fixed
                // var sql = "SELECT * FROM Groups WHERE IsActive = 1";
                // var groups = await _dbService.QueryAsync<GroupInfo>(sql);
                
                var groups = new List<GroupInfo>
                {
                    new GroupInfo { GroupID = 1, GroupName = "Admin", Description = "Administrators", CreatedDate = DateTime.Now },
                    new GroupInfo { GroupID = 2, GroupName = "Moderator", Description = "Content Moderators", CreatedDate = DateTime.Now }
                };

                return Ok(new { Response = 1, Data = groups, message = "Success" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }

        /// <summary>
        /// Add new user group
        /// </summary>
        [HttpPost("FunGroup_Add")]
        [SwaggerOperation(Summary = "Add new user group")]
        [SwaggerResponse(200, "Success")]
        public async Task<IActionResult> FunGroup_Add([FromForm] GroupAddRequest request)
        {
            try
            {
                // Placeholder logic
                return Ok(new { Response = 1, message = "Group added successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }

        /// <summary>
        /// Edit existing group
        /// </summary>
        [HttpPost("FunGroup_Edit")]
        [SwaggerOperation(Summary = "Edit existing group")]
        [SwaggerResponse(200, "Success")]
        public async Task<IActionResult> FunGroup_Edit([FromForm] GroupEditRequest request)
        {
            try
            {
                 // Placeholder logic
                return Ok(new { Response = 1, message = "Group updated successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }

        /// <summary>
        /// Toggle group status (soft delete)
        /// </summary>
        [HttpPost("Group_Delete")]
        [SwaggerOperation(Summary = "Delete/Toggle group")]
        [SwaggerResponse(200, "Success")]
        public async Task<IActionResult> Group_Delete([FromForm] GroupDeleteRequest request)
        {
            try
            {
                 // Placeholder logic
                return Ok(new { Response = 1, message = "Group deleted successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }
    }

    #region Models
    public class GroupInfo
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class GroupAddRequest
    {
        public string txtName { get; set; }
        public string txtDescription { get; set; }
    }

    public class GroupEditRequest
    {
        public int txtGroupID { get; set; }
        public string txtName { get; set; }
        public string txtDescription { get; set; }
    }

    public class GroupDeleteRequest
    {
        public int GroupID { get; set; }
    }
    #endregion
}
