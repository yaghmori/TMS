namespace TMS.Shared.Constants
{

    public static class EndPoints
    {
        public static class Hub
        {
            //====================== Users ==================================
            public const string HubUrl = "Hubs/MainHub";
            //====================== Dashboard ==================================
            public const string SendUpdateDashboard = "UpdateDashboardAsync";
            public const string ReceiveUpdateDashboard = "UpdateDashboard";
            //====================== Token ==================================
            public const string SendRegenerateTokens = "RegenerateTokensAsync";
            public const string ReceiveRegenerateTokens = "RegenerateTokens";
            public const string SendUpdateAuthState = "UpdateAuthStateAsync";
            public const string ReceiveUpdateAuthState = "UpdateAuthState";
            public const string SendTerminateSession = "TerminateSessionAsync";
            public const string ReceiveTerminateSession = "TerminateSession";
            public const string SendUpdateUser = "UpdateUserAsync";
            public const string ReceiveUpdateUser = "UpdateUpdateUser";

            //====================== Chat ==================================
            public const string SendChatNotification = "ChatNotificationAsync";
            public const string ReceiveChatNotification = "ReceiveChatNotification";
            public const string ReceiveMessage = "ReceiveMessage";
            public const string SendMessage = "SendMessageAsync";

            //==================Culture==============================
            public const string SendUpdateCulture = "UpdateCultureAsync";
            public const string ReceiveUpdateCulture = "UpdateCulture";

        }
        public static class UserController
        {
            public const string GetUsers = "api/v1/server/users";
            public const string GetUserById = "api/v1/server/users/{0}";
            public const string AddNewUser = "api/v1/server/users/New";
            public const string AddNewUserWithPassword = "api/v1/server/users/NewUser";
            public const string UpdateUserById = "api/v1/server/users/{0}";
            public const string ChangePassword = "api/v1/server/users/{0}/change-password";
            public const string DeleteUserById = "api/v1/server/users/{0}";
            public const string AddRoleToUserByRoleName = "api/v1/server/users/{0}/Roles?roleName={1}";
            public const string AddRoleToUserByRoleId = "api/v1/server/users/{0}/Roles/{1}";
            public const string RemoveRoleFromUserByRoleName = "api/v1/server/users/Roles?roleName={1}";
            public const string RemoveRoleFromUserByRoleId = "api/v1/server/users/Roles/{1}";
            public const string GetUserRolesByUserIdAsync = "api/v1/server/users/{0}/Roles";
            public const string GetClaimsByUserId = "api/v1/server/users/{0}/Claims";
            public const string GetUserByJWT = "api/v1/server/users/Token";
            public const string GetUserClaimsByJWT = "api/v1/server/users/claims";
            public const string GetUserRolesByJWT = "api/v1/server/users/Roles";
            public const string GetUserTenantsByJWT = "api/v1/server/users/Tenants";
            public const string GetUserByRefreshToken = "api/v1/server/users/RefreshToken";
            public const string GetTenantsByUserId = "api/v1/server/users/{0}/Tenants";
            public const string ReplaceUserTenants = "api/v1/server/users/{0}/UpdateTenants";
            public const string ReplaceUserRoles = "api/v1/server/users/{0}/UpdateRoles";
            public const string ReplaceUserClaims = "api/v1/server/users/{0}/UpdateClaims";
            //===============================Session==================================
            public const string GetActiveSessions = "api/v1/server/users/{0}/sessions";
            public const string TerminateSession = "api/v1/server/users/sessions/{0}";

        }
        public static class AuthController
        {
            public const string RefreshToken = "api/v1/server/auth/token/refresh";
            public const string GetToken = "api/v1/server/auth/token";
            public const string Register = "api/v1/server/auth/Register";


            public const string RegisterByEmail = "api/v1/server/auth/RegisterByEmail";
            public const string RegisterByPhoneNumber = "api/v1/server/auth/RegisterByPhoneNumber";
            public const string IsDupplicateByUserName = "api/v1/server/auth/UserName";
            public const string IsDupplicateByEmail = "api/v1/server/auth/Email";
            public const string IsDupplicateByPhoneNumber = "api/v1/server/auth/PhoneNumber";
            public const string VerifyPhoneNumber = "api/v1/server/auth/Verify-PhoneNumber";
            public const string VerifyEmail = "api/v1/server/auth/Verify-email";
            public const string TwoFactorLoginByPhoneNumber = "api/v1/server/Auth/Login/TwoFactor/PhoneNumber";
            public const string TwoFactorLoginByEmail = "api/v1/server/Auth/Login/TwoFactor/Email?email={0}";
            public const string Logout = "api/v1/server/Auth/Logout";
            public const string ForgotPasswordByUserId = "api/v1/server/Auth/{0}/Forgot-Password/";
            public const string ForgotPasswordByPhoneNumber = "api/v1/server/Auth/Forgot-Password/PhoneNumber?phoneNumber={0}";
            public const string ForgotPasswordByUserName = "api/v1/server/Auth/Forgot-Password/UserName?userName={0}";
            public const string ForgotPasswordByEmail = "api/v1/server/Auth/Forgot-Password/Email?email={0}";
            public const string ResetPassword = "api/v1/server/Auth/{0}/Reset-Password";
            public const string SetPassword = "api/v1/server/Auth/{0}/set-Password";
            public const string UpdateSecurityStamp = "api/v1/server/Auth/UpdateSecurityStamp/{0}";
        }
        public static class TenantController
        {
            public const string TenantEndPoint = "api/v1/Tenants";

            public const string GetTenants = "api/v1/server/Tenants";
            public const string GetTenantById = "api/v1/server/Tenants/{0}";
            public const string GetUsersByTenantId = "api/v1/server/Tenants/{0}/users";
            public const string AddTenant = "api/v1/server/Tenants";
            public const string AddUserToTenant = "api/v1/server/Tenants/{0}/users/{1}";
            public const string RemoveUserFromTenant = "api/v1/server/Tenants/{0}/users/{1}";
            public const string DeleteUserTenantById = "api/v1/server/UserTenants/{0}";
            public const string DeleteTenantById = "api/v1/server/Tenants/{0}";
            public const string UpdateTenantById = "api/v1/server/Tenants/{0}";
            public const string ReplaceTenantUsers = "api/v1/server/Tenants/{0}/users";
            public const string CreateDataBase = "api/v1/server/Tenants/{0}/EnsureCreated";
            public const string DeleteDataBase = "api/v1/server/Tenants/{0}/EnsureDeleted";
            public const string MigrateDataBase = "api/v1/server/Tenants/{0}/Migrate";
        }
        public static class AppSettingController
        {
            public const string GetAppSettings = "api/v1/server/AppSettings";
            public const string GetAppSettingById = "api/v1/server/AppSettings/{0}";
            public const string GetAppSettingByKey = "api/v1/server/AppSettings/keys";
            public const string AddAppSetting = "api/v1/server/AppSettings";
            public const string DeleteAppSettingById = "api/v1/server/AppSettings/{0}";
            public const string DeleteAppSettingByKey = "api/v1/server/AppSettings/keys";
            public const string UpdateAppSettingById = "api/v1/server/AppSettings/{0}";
        }
        public static class UserSettingController
        {
            public const string GetUserSettings = "api/v1/server/UserSettings";
            public const string GetUserSettingById = "api/v1/server/UserSettings/{0}";
            public const string AddUserSetting = "api/v1/server/UserSettings";
            public const string GetUserSettingByUserId = "api/v1/server/UserSettings/users/{0}";
            public const string DeleteUserSettingById = "api/v1/server/UserSettings/{0}";
            public const string UpdateUserSettingById = "api/v1/server/UserSettings/{0}";
        }
        public static class RoleController
        {
            public const string GetRoles = "api/v1/server/roles";
            public const string GetRoleClaims = "api/v1/server/roles/{0}/claims";
            public const string GetRoleById = "api/v1/server/roles/{0}";
            public const string GetRoleByName = "api/v1/server/roles/{0}";
            public const string AddRole = "api/v1/server/roles/{0}";
            public const string DeleteRoleById = "api/v1/server/roles/{0}";
            public const string DeleteRoleByName = "api/v1/server/roles/{0}";
            public const string UpdateRoleById = "api/v1/server/roles/{0}";
            public const string GetUsersByRoleId = "api/v1/server/roles/{0}/users";
            public const string GetUsersByRoleName = "api/v1/server/roles/{0}/users";
            public const string GetRolesByUserId = "api/v1/server/roles/users";
            public const string ReplaceRoleUsers = "api/v1/server/roles/{0}/users";
            public const string UpdateRoleClaims = "api/v1/server/roles/{0}/claims";
        }
        public static class CultureController
        {
            public const string GetCultures = "api/v1/server/cultures";
            public const string GetCultureById = "api/v1/server/cultures/{0}";
            public const string AddCulture = "api/v1/server/cultures";
            public const string DeleteCultureById = "api/v1/server/cultures/{0}";
            public const string UpdateCultureById = "api/v1/server/cultures/{0}";
        }
        public static class ClaimController
        {
            public const string GetClaims = "api/v1/server/claims";
            public const string GetClaimById = "api/v1/server/claims/{0}";
            public const string AddCalim = "api/v1/server/claims";
            public const string DeleteClaimById = "api/v1/server/claims/{0}";
            public const string UpdateClaimById = "api/v1/server/claims/{0}";
        }
        public static class SiloItemController
        {
            public const string GetAllSiloItems = "api/v1/tenant/siloitems";
            public const string GetSiloItemById = "api/v1/tenant/siloitems/{0}";
            public const string GetByItemType = "api/v1/tenant/siloitems/ItemType";
            public const string GetLastIndex = "api/v1/tenant/siloitems/LastIndex";
            public const string GetLastAddress = "api/v1/tenant/siloitems/LastAddress";
            public const string GetAncestorsById = "api/v1/tenant/siloitems/{0}/Ancestors";
            public const string GetChildById = "api/v1/tenant/siloitems/{0}/Child";
            public const string GetParentById = "api/v1/tenant/siloitems/{0}/Parent";
            public const string AddSiloItem = "api/v1/tenant/siloitems";
            public const string DeleteSiloItemById = "api/v1/tenant/siloitems/{0}";
            public const string DeleteAllSiloItems = "api/v1/tenant/siloitems/Reset";
            public const string UpdateSiloItemById = "api/v1/tenant/siloitems/{0}";
            public const string GetHistoriesById = "api/v1/tenant/siloitems/{0}/History";
            //====================== SensorHistories ==============================
            public const string GetHistories = "api/v1/tenant/sensorhistories/SiloItems";

        }










    }
}
