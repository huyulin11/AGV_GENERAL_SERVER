using System;

namespace AGV.init
{
    public enum USER_TYPE_T
    {
        USER_TYPE_MIN = 0,
        USER_TYPE_ADMIN = 1,
        USER_TYPE_GENERAL = 2,

        USER_TYPE_MAX
    }

    public class User
    {
        public int id; //用户id
        public String userName = null;  //用户名称
        public String userPasswd = null; //用户密码
        public USER_TYPE_T userType = 0; //用户类型 1 管理员用户 2 普通用户
    }
}
