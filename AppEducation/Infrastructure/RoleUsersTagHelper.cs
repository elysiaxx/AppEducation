using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AppEducation.Models.Users;
namespace AppEducation.Infrastructure
{
    /*
        What are tag helpers ? 
        - Tag helper là 1 class thao tác với các phần tử HTML, hoặc thay đổi chúng theo một cách nào đó, để cung cấp chúng với nội dung bổ sung
        hoặc thay đổi chúng một cách hoàn toàn với nội dung mới

        Why are they userful?
        - Tag helper cho phép nội dung view được tạo ra hoặc chuyển đổi nhờ C# logic, việc đảm bảo rằng nội dung HTML gửi tới client phản ánh được 
        trạng thái của ứng dụng.

        How are they used
        - Những phần tử HTML trong đó có áp dụng tag helpers được lựa chọn dựa trên tên của class hoặc thông qua việc dùng thuộc tính HTMLTargetElement.
        Khi một View được gửi, các phần tử được chuyển đổi bởi tag helpers và được bao gồm trong HTML gửi tới client

        => Tag helper ở đây hoạt động trên thẻ 'td' với thuộc tính identity-role, cái mà được sử dụng để nhận tên của Role đang được xử lý.

    */

    [HtmlTargetElement("td", Attributes = "identity-role")]
    public class RoleUsersTagHelper : TagHelper
    {
        private UserManager<AppUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        public RoleUsersTagHelper(UserManager<AppUser> usermgr,
        RoleManager<IdentityRole> rolemgr)
        {
            userManager = usermgr;
            roleManager = rolemgr;
        }
        [HtmlAttributeName("identity-role")]
        public string Role { get; set; }
        public override async Task ProcessAsync(TagHelperContext context,
        TagHelperOutput output)
        {
            List<string> names = new List<string>();
            IdentityRole role = await roleManager.FindByIdAsync(Role);
            if (role != null)
            {
                foreach (var user in userManager.Users)
                {
                    if (user != null
                    && await userManager.IsInRoleAsync(user, role.Name))
                    {
                        names.Add(user.UserName);
                    }
                }
            }
            output.Content.SetContent(names.Count == 0 ?
            "No Users" : string.Join(", ", names));
        }
    }
}
