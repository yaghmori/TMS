using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TMS.Core.Domain.Entities;

namespace TMS.Shared.Constants
{
    public static class ApplicationPermissions
    {
        [DisplayName("SensorHistories")]
        [Description("SensorHistorie Permissions")]
        public static class SensorHistory
        {
            public const string View = "SensorHistorie.View";
            public const string Create = "SensorHistorie.Create";
            public const string Edit = "SensorHistorie.Edit";
            public const string Delete = "SensorHistorie.Delete";
            public const string Export = "SensorHistorie.Export";
            public const string Search = "SensorHistorie.Search";
        }

        [DisplayName("ProjectSettings")]
        [Description("ProjectSettings Permissions")]
        public static class ProjectSetting
        {
            public const string View = "ProjectSetting.View";
            public const string Edit = "ProjectSetting.Edit";
        }

        [DisplayName("UserClaims")]
        [Description("UserClaims Permissions")]
        public static class UserClaims
        {
            public const string View = "UserClaim.View";
            public const string Edit = "UserClaim.Edit";
        }


        [DisplayName("Tenants")]
        [Description("Tenants Permissions")]
        public static class Tenants
        {
            public const string View = "Client.View";
            public const string Create = "Client.Create";
            public const string Edit = "Client.Edit";
            public const string Delete = "Client.Delete";
            public const string Search = "Client.Search";
            public const string AddOrRemoveUsers = "Client.AddOrRemoveUsers";
            public const string Configuration = "Client.Configuration";


        }


        [DisplayName("Users")]
        [Description("Users Permissions")]
        public static class User
        {
            public const string View = "User.View";
            public const string ViewSessions = "User.ViewSessions";
            public const string TerminateSession = "User.TerminateSession";
            public const string Create = "User.Create";
            public const string Edit = "User.Edit";
            public const string AddOrRemovePermissions = "User.AddOrRemovePermissions";
            public const string AddOrRemoveRoles = "User.AddOrRemoveRoles";
            public const string AddOrRemoveTenant = "User.AddOrRemoveTenant";
            public const string Delete = "User.Delete";
            public const string Export = "User.Export";
            public const string Search = "User.Search";
        }


        [DisplayName("Roles")]
        [Description("Role Permissions")]
        public static class Roles
        {
            public const string View = "Role.View";
            public const string AddOrRemoveUsers = "Role.AddOrRemoveUsers";
            public const string AddOrRemoveClaims = "Role.AddOrRemoveClaims";
            public const string Create = "Role.Create";
            public const string Edit = "Role.Edit";
            public const string Delete = "Role.Delete";
            public const string Search = "Role.Search";
        }

        [DisplayName("Role Claims")]
        [Description("Role Claim Permissions")]
        public static class RoleClaims
        {
            public const string View = "RoleClaim.View";
            public const string Create = "RoleClaim.Create";
            public const string Edit = "RoleClaim.Edit";
            public const string Delete = "RoleClaim.Delete";
            public const string Search = "RoleClaim.Search";
        }

        [DisplayName("Communication")]
        [Description("Communication Permissions")]
        public static class Communication
        {
            public const string SendMessage = "Communication.SendMessage";
            public const string Chat = "Communication.Chat";
        }

        [DisplayName("Preference")]
        [Description("Preferences Permissions")]
        public static class Preference
        {
            public const string ChangeLanguage = "Preference.ChangeLanguage";

            //TODO - add permissions
        }

        [DisplayName("Dashboard")]
        [Description("Dashboards Permissions")]
        public static class Dashboard
        {
            public const string View = "Dashboard.View";
        }

        [DisplayName("Dashboards")]
        [Description("Dashboards Permissions")]
        public static class AdminDashboard
        {
            public const string View = "AdminDashboard.View";
        }

        [DisplayName("Audit Trails")]
        [Description("Audit Trails Permissions")]
        public static class AuditTrail
        {
            public const string View = "AuditTrail.View";
            public const string Export = "AuditTrail.Export";
            public const string Search = "AuditTrail.Search";
        }

        [DisplayName("Identity Management")]
        [Description("Identity Management")]
        public static class IdentityManagement
        {
            public const string Menu = "IdentityManagement.Menu";
            public const string View = "IdentityManagement.View";
        }

        [DisplayName("User Profile")]
        [Description("User Profile Permissions")]
        public static class UserProfile
        {
            public const string View = "UserProfile.View";
            public const string Export = "UserProfile.Edit";
        }

        [DisplayName("App Setting")]
        [Description("Application Settings Permissions")]
        public static class AppSetting
        {
            public const string View = "AppSetting.View";
            public const string Create = "AppSetting.Create";
            public const string Edit = "AppSetting.Edit";
            public const string Delete = "AppSetting.Delete";
            public const string Search = "AppSetting.Search";
        }

        [DisplayName("User Setting")]
        [Description("User Settings Permissions")]
        public static class UserSetting
        {
            public const string View = "UserSetting.View";
            public const string Create = "UserSetting.Create";
            public const string Edit = "UserSetting.Edit";
            public const string Delete = "UserSetting.Delete";
            public const string Search = "UserSetting.Search";
        }

        /// <summary>
        /// Returns a list of Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllPermissions()
        {
            var permissions = new List<string>();
            foreach (var prop in typeof(ApplicationPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                    permissions.Add(propertyValue.ToString());
            }
            return permissions;
        }

        public static List<string> GetBasicPermissions()
        {
            var permissions = new List<string>();
            foreach (var prop in typeof(ApplicationPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                    permissions.Add(propertyValue.ToString());
            }
            return permissions;
        }

        /// <summary>
        /// Returns a list of Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRegisteredPermissions()
        {
            var permissions = new List<string>();
            foreach (var prop in typeof(ApplicationPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                    permissions.Add(propertyValue.ToString());
            }
            return permissions;
        }




    }
}