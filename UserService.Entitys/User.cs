using FreeSql.DataAnnotations;
using System.ComponentModel.DataAnnotations;


namespace UserService.Entities
{

    public class User
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }
    }
}
