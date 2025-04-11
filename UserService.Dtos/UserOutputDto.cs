using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Dtos
{
    public class UserOutputDto: UserBaseDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int Id { get; set; }
    }
}
