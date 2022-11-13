using System.Linq.Expressions;
using TMS.Core.Domain.Entities;
using TMS.Core.Domain.Enums;
using TMS.Shared.Responses;

namespace Infrastructure.Query
{
    public static class SelectExpressions
    {
        public static class Tenants
        {
            public static Expression<Func<Tenant, TenantResponse>> TenantResponse = s => new TenantResponse
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                IsActive = s.IsActive,
                ExpireDate = s.ExpireDate,
                Description = s.Description,
                DBProvider = s.DBProvider,
                ConnectionString = s.ConnectionString,
                Users = s.UserTenants.Select(u => new UserResponse
                {
                    Image = u.User.Image,
                    Id = u.User.Id.ToString(),
                    PhoneNumber = u.User.PhoneNumber,
                    FirstName = u.User.FirstName,
                    LastName = u.User.LastName,
                    Email = u.User.Email
                }).ToList()
            };
        }
        public static class Users
        {
            public static Expression<Func<User, UserResponse>> UserResponse = s => new UserResponse
            {
                Id = s.Id.ToString(),
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Description = s.Description,
                Image = s.Image,
                ProfileName = s.ProfileName,
                IsActive = s.IsActive,
                Settings = new UserSettingsResponse
                {
                    Id = s.Settings.Id.ToString(),
                    Culture = s.Settings.Culture,
                    DarkMode = s.Settings.DarkMode,
                    DefaultTenantId = s.Settings.DefaultTenantId.ToString(),
                    RightToLeft = s.Settings.RightToLeft,
                    Theme = s.Settings.Theme
                },
                Roles = s.UserRoles.Select(r => new RoleResponse
                {
                    Id = r.Role.Id.ToString(),
                    Name = r.Role.Name
                }).ToList(),
                Tenants = s.UserTenants.Select(d => new TenantResponse
                {
                    Id = d.Tenant.Id.ToString().ToLowerInvariant(),
                    Name = d.Tenant.Name,
                    ExpireDate = d.Tenant.ExpireDate,
                    IsActive = d.Tenant.IsActive,
                    Description = d.Tenant.Description,
                    DBProvider = d.Tenant.DBProvider,
                    ConnectionString = d.Tenant.ConnectionString,
                }).ToList(),
                Claims = s.UserClaims.Select(c => new UserClaimResponse
                {
                    ClaimType = c.ClaimType,
                    ClaimValue = c.ClaimValue,
                }).ToList(),
                ActiveSessions = s.UserSessions.Select(t => new UserSessionResponse
                {
                    Id = t.Id.ToString(),
                    Name = t.Name,
                    LoginProvider = t.LoginProvider,
                    BuildVersion = t.BuildVersion,
                }).ToList()
            };
        }
        public static class UserClaims
        {
            public static Expression<Func<UserClaim, UserClaimResponse>> UserClaimResponse = s => new UserClaimResponse
            {
                UserId = s.UserId.ToString(),
                ClaimType = s.ClaimType,
                ClaimValue = s.ClaimValue
            };
            public static Expression<Func<UserClaim, ClaimResponse>> ClaimResponse = s => new ClaimResponse
            {
                ClaimType = s.ClaimType,
                ClaimValue = s.ClaimValue,

            };

        }
        public static class UserTenants
        {
            public static Expression<Func<UserTenant, UserResponse>> UserResponse = s => new UserResponse
            {
                Id = s.User.Id.ToString(),
                Email = s.User.Email,
                PhoneNumber = s.User.PhoneNumber,
                FirstName = s.User.FirstName,
                LastName = s.User.LastName,
                Description = s.User.Description,
                Image = s.User.Image,
                ProfileName = s.User.ProfileName
            };
            public static Expression<Func<UserTenant, TenantResponse>> TenantResponse = s => new TenantResponse
            {
                Id = s.Tenant.Id.ToString(),
                Description = s.Tenant.Description,
                ExpireDate = s.Tenant.ExpireDate,
                ConnectionString = s.Tenant.ConnectionString,
                DBProvider = s.Tenant.DBProvider,
                IsActive = s.Tenant.IsActive,
                Name = s.Tenant.Name,
            };
        }
        public static class UserSessions
        {
            public static Expression<Func<UserSession, UserSessionResponse>> UserSessionResponse = s => new UserSessionResponse
            {
                Id = s.Id.ToString(),
                UserId = s.UserId.ToString(),
                Name = s.Name,
                LoginProvider = s.LoginProvider,
                StartDate = s.StartDate,
                BuildVersion = s.BuildVersion,
                SessionIpAddress = s.SessionIpAddress,
                RefreshToken = s.RefreshToken,
                RefreshTokenExpires = s.RefreshTokenExpires,
            };
        }

        public static class UserRoles
        {
            public static Expression<Func<UserRole, RoleResponse>> RoleResponse = s => new RoleResponse
            {
                Id = s.Role.Id.ToString(),
                Name = s.Role.Name
            };
            public static Expression<Func<UserRole, UserResponse>> UserResponse = s => new UserResponse
            {
                Id = s.User.Id.ToString(),
                Email = s.User.Email,
                FirstName = s.User.FirstName,
                LastName = s.User.LastName,
                Image = s.User.Image
            };
        }
        public static class SiloItems
        {
            public static Expression<Func<SiloItem, SiloItemResponse>> SiloItemResponse = t => new SiloItemResponse
            {
                Id = t.Id.ToString(),
                Name = t.Name,
                IsExpanded = t.IsExpanded,
                AirCondition = t.AirCondition,
                Feature = t.Feature,
                IsMapped = t.IsMapped,
                ItemType = t.ItemType,
                ParentId = t.ParentId.ToString(),
                Value = t.Value,
                WarningServiceType = t.WarningServiceType,
                Index = t.Index,
                Box = t.Box,
                Line = t.Line,
                Address = t.Address,
                FalseValueCount = t.FalseValueCount,
                IsReadOnly = t.IsReadOnly,
                FalseValue = t.FalseValue,
                ChildLimit = t.ChildLimit,
                Description = t.Description,
                IsActive = t.IsActive,
                HighTemp = t.HighTemp,
                LowTemp = t.LowTemp,
                Parent = null,
                Length = t.Length,
                SensorSpace = t.SensorSpace,
                Offset = t.Offset,
                ReadIndex = t.ReadIndex,
                ReadDateTime = t.ReadDateTime,
                LoopDiameter = t.LoopDiameter,
                SiloDiameter = t.SiloDiameter,
                TechNo = t.TechNo,
                SiloHeight = t.SiloHeight,
                Rom = t.Rom,
            };
            public static Expression<Func<SiloItem, SiloItemResponse>> FullSiloItemResponse = t => new SiloItemResponse
            {
                Id = t.Id.ToString(),
                Name = t.Name,
                IsExpanded = t.IsExpanded,
                AirCondition = t.AirCondition,
                Feature = t.Feature,
                IsMapped = t.IsMapped,
                ItemType = t.ItemType,
                ParentId = t.ParentId.ToString(),
                Value = t.Value,
                WarningServiceType = t.WarningServiceType,
                Index = t.Index,
                Box = t.Box,
                Line = t.Line,
                Address = t.Address,
                FalseValueCount = t.FalseValueCount,
                IsReadOnly = t.IsReadOnly,
                FalseValue = t.FalseValue,
                ChildLimit = t.ChildLimit,
                Description = t.Description,
                IsActive = t.IsActive,
                HighTemp = t.HighTemp,
                LowTemp = t.LowTemp,
                Parent = null,
                Length = t.Length,
                SensorSpace = t.SensorSpace,
                Offset = t.Offset,
                ReadIndex = t.ReadIndex,
                ReadDateTime = t.ReadDateTime,
                LoopDiameter = t.LoopDiameter,
                SiloDiameter = t.SiloDiameter,
                TechNo = t.TechNo,
                SiloHeight = t.SiloHeight,
                Rom = t.Rom,
                FeaturedSensors = t.SiloItems.Where(x => x.Feature != SensorFeatureEnum.None).Select(f => new SiloItemResponse
                {
                    Id = f.Id.ToString(),
                    Name = f.Name,
                    IsExpanded = f.IsExpanded,
                    AirCondition = f.AirCondition,
                    Feature = f.Feature,
                    IsMapped = f.IsMapped,
                    ItemType = f.ItemType,
                    ParentId = f.ParentId.ToString(),
                    Value = f.Value,
                    WarningServiceType = f.WarningServiceType,
                    Index = f.Index,
                    Box = f.Box,
                    Line = f.Line,
                    Address = f.Address,
                    FalseValueCount = f.FalseValueCount,
                    IsReadOnly = f.IsReadOnly,
                    FalseValue = f.FalseValue,
                    ChildLimit = f.ChildLimit,
                    Description = f.Description,
                    IsActive = f.IsActive,
                    HighTemp = f.HighTemp,
                    LowTemp = f.LowTemp,
                }).OrderBy(o => o.Index).ToList(),
                SiloItems = t.SiloItems.Where(x => x.Feature == SensorFeatureEnum.None).Select(l => new SiloItemResponse
                {
                    Id = l.Id.ToString(),
                    Name = l.Name,
                    IsExpanded = l.IsExpanded,
                    AirCondition = l.AirCondition,
                    Feature = l.Feature,
                    IsMapped = l.IsMapped,
                    ItemType = l.ItemType,
                    ParentId = l.ParentId.ToString(),
                    Value = l.Value,
                    WarningServiceType = l.WarningServiceType,
                    Index = l.Index,
                    Box = l.Box,
                    Line = l.Line,
                    Address = l.Address,
                    FalseValueCount = l.FalseValueCount,
                    IsReadOnly = l.IsReadOnly,
                    FalseValue = l.FalseValue,
                    ChildLimit = l.ChildLimit,
                    Description = l.Description,
                    IsActive = l.IsActive,
                    HighTemp = l.HighTemp,
                    LowTemp = l.LowTemp,
                    Length = l.Length,
                    SensorSpace = l.SensorSpace,
                    Offset = l.Offset,
                    ReadIndex = l.ReadIndex,
                    ReadDateTime = l.ReadDateTime,
                    LoopDiameter = l.LoopDiameter,
                    SiloDiameter = l.SiloDiameter,
                    TechNo = l.TechNo,
                    SiloHeight = l.SiloHeight,
                    Rom = l.Rom,
                    Parent = new SiloItemResponse
                    {
                        Id = t.Id.ToString(),
                        Name = t.Name,
                        IsExpanded = t.IsExpanded,
                        AirCondition = t.AirCondition,
                        Feature = t.Feature,
                        IsMapped = t.IsMapped,
                        ItemType = t.ItemType,
                        ParentId = t.ParentId.ToString(),
                        Value = t.Value,
                        WarningServiceType = t.WarningServiceType,
                        Index = t.Index,
                        Box = t.Box,
                        Line = t.Line,
                        Address = t.Address,
                        FalseValueCount = t.FalseValueCount,
                        IsReadOnly = t.IsReadOnly,
                        FalseValue = t.FalseValue,
                        ChildLimit = t.ChildLimit,
                        Description = t.Description,
                        IsActive = t.IsActive,
                        HighTemp = t.HighTemp,
                        LowTemp = t.LowTemp,
                        Length = t.Length,
                        SensorSpace = t.SensorSpace,
                        Offset = t.Offset,
                        ReadIndex = t.ReadIndex,
                        ReadDateTime = t.ReadDateTime,
                        LoopDiameter = t.LoopDiameter,
                        SiloDiameter = t.SiloDiameter,
                        TechNo = t.TechNo,
                        SiloHeight = t.SiloHeight,
                        Rom = t.Rom,
                    },
                    FeaturedSensors = l.SiloItems.Where(x => x.Feature != SensorFeatureEnum.None).Select(f => new SiloItemResponse
                    {
                        Id = f.Id.ToString(),
                        Name = f.Name,
                        IsExpanded = f.IsExpanded,
                        AirCondition = f.AirCondition,
                        Feature = f.Feature,
                        IsMapped = f.IsMapped,
                        ItemType = f.ItemType,
                        ParentId = f.ParentId.ToString(),
                        Value = f.Value,
                        WarningServiceType = f.WarningServiceType,
                        Index = f.Index,
                        Box = f.Box,
                        Line = f.Line,
                        Address = f.Address,
                        FalseValueCount = f.FalseValueCount,
                        IsReadOnly = f.IsReadOnly,
                        FalseValue = f.FalseValue,
                        ChildLimit = f.ChildLimit,
                        Description = f.Description,
                        IsActive = f.IsActive,
                        HighTemp = f.HighTemp,
                        LowTemp = f.LowTemp,
                    }).OrderBy(o => o.Index).ToList(),
                    SiloItems = l.SiloItems.Select(c => new SiloItemResponse
                    {
                        Id = c.Id.ToString(),
                        Name = c.Name,
                        IsExpanded = c.IsExpanded,
                        AirCondition = c.AirCondition,
                        Feature = c.Feature,
                        IsMapped = c.IsMapped,
                        ItemType = c.ItemType,
                        ParentId = c.ParentId.ToString(),
                        Value = c.Value,
                        WarningServiceType = c.WarningServiceType,
                        Index = c.Index,
                        Box = c.Box,
                        Line = c.Line,
                        Address = c.Address,
                        FalseValueCount = c.FalseValueCount,
                        IsReadOnly = c.IsReadOnly,
                        FalseValue = c.FalseValue,
                        ChildLimit = c.ChildLimit,
                        Description = c.Description,
                        IsActive = c.IsActive,
                        HighTemp = c.HighTemp,
                        LowTemp = c.LowTemp,
                        Length = c.Length,
                        SensorSpace = c.SensorSpace,
                        Offset = c.Offset,
                        ReadIndex = c.ReadIndex,
                        ReadDateTime = c.ReadDateTime,
                        LoopDiameter = c.LoopDiameter,
                        SiloDiameter = c.SiloDiameter,
                        TechNo = c.TechNo,
                        SiloHeight = c.SiloHeight,
                        Rom = c.Rom,
                        Parent = new SiloItemResponse
                        {
                            Id = l.Id.ToString(),
                            Name = l.Name,
                            IsExpanded = l.IsExpanded,
                            AirCondition = l.AirCondition,
                            Feature = l.Feature,
                            IsMapped = l.IsMapped,
                            ItemType = l.ItemType,
                            ParentId = l.ParentId.ToString(),
                            Value = l.Value,
                            WarningServiceType = l.WarningServiceType,
                            Index = l.Index,
                            Box = l.Box,
                            Line = l.Line,
                            Address = l.Address,
                            FalseValueCount = l.FalseValueCount,
                            IsReadOnly = l.IsReadOnly,
                            FalseValue = l.FalseValue,
                            ChildLimit = l.ChildLimit,
                            Description = l.Description,
                            IsActive = l.IsActive,
                            HighTemp = l.HighTemp,
                            LowTemp = l.LowTemp,
                            Length = l.Length,
                            SensorSpace = l.SensorSpace,
                            Offset = l.Offset,
                            ReadIndex = l.ReadIndex,
                            ReadDateTime = l.ReadDateTime,
                            LoopDiameter = l.LoopDiameter,
                            SiloDiameter = l.SiloDiameter,
                            TechNo = l.TechNo,
                            SiloHeight = l.SiloHeight,
                            Rom = l.Rom,
                        },

                        FeaturedSensors = c.SiloItems.Where(x => x.Feature != SensorFeatureEnum.None).Select(f => new SiloItemResponse
                        {
                            Id = f.Id.ToString(),
                            Name = f.Name,
                            IsExpanded = f.IsExpanded,
                            AirCondition = f.AirCondition,
                            Feature = f.Feature,
                            IsMapped = f.IsMapped,
                            ItemType = f.ItemType,
                            ParentId = f.ParentId.ToString(),
                            Value = f.Value,
                            WarningServiceType = f.WarningServiceType,
                            Index = f.Index,
                            Box = f.Box,
                            Line = f.Line,
                            Address = f.Address,
                            FalseValueCount = f.FalseValueCount,
                            IsReadOnly = f.IsReadOnly,
                            FalseValue = f.FalseValue,
                            ChildLimit = f.ChildLimit,
                            Description = f.Description,
                            IsActive = f.IsActive,
                            HighTemp = f.HighTemp,
                            LowTemp = f.LowTemp,
                            Length = f.Length,
                            SensorSpace = f.SensorSpace,
                            Offset = f.Offset,
                            ReadIndex = f.ReadIndex,
                            ReadDateTime = f.ReadDateTime,
                            LoopDiameter = f.LoopDiameter,
                            SiloDiameter = f.SiloDiameter,
                            TechNo = f.TechNo,
                            SiloHeight = f.SiloHeight,
                            Rom = f.Rom,
                        }).OrderBy(o => o.Index).ToList(),
                        SiloItems = c.SiloItems.Select(s => new SiloItemResponse
                        {
                            Id = s.Id.ToString(),
                            Name = s.Name,
                            IsExpanded = s.IsExpanded,
                            AirCondition = s.AirCondition,
                            Feature = s.Feature,
                            IsMapped = s.IsMapped,
                            ItemType = s.ItemType,
                            ParentId = s.ParentId.ToString(),
                            Value = s.Value,
                            WarningServiceType = s.WarningServiceType,
                            Index = s.Index,
                            Box = s.Box,
                            Line = s.Line,
                            Address = s.Address,
                            FalseValueCount = s.FalseValueCount,
                            IsReadOnly = s.IsReadOnly,
                            FalseValue = s.FalseValue,
                            ChildLimit = s.ChildLimit,
                            Description = s.Description,
                            IsActive = s.IsActive,
                            HighTemp = s.HighTemp,
                            LowTemp = s.LowTemp,
                            Length = s.Length,
                            SensorSpace = s.SensorSpace,
                            Offset = s.Offset,
                            ReadIndex = s.ReadIndex,
                            ReadDateTime = s.ReadDateTime,
                            LoopDiameter = s.LoopDiameter,
                            SiloDiameter = s.SiloDiameter,
                            TechNo = s.TechNo,
                            SiloHeight = s.SiloHeight,
                            Rom = s.Rom,
                            Parent = new SiloItemResponse
                            {
                                Id = c.Id.ToString(),
                                Name = c.Name,
                                IsExpanded = c.IsExpanded,
                                AirCondition = c.AirCondition,
                                Feature = c.Feature,
                                IsMapped = c.IsMapped,
                                ItemType = c.ItemType,
                                ParentId = c.ParentId.ToString(),
                                Value = c.Value,
                                WarningServiceType = c.WarningServiceType,
                                Index = c.Index,
                                Box = c.Box,
                                Line = c.Line,
                                Address = c.Address,
                                FalseValueCount = c.FalseValueCount,
                                IsReadOnly = c.IsReadOnly,
                                FalseValue = c.FalseValue,
                                ChildLimit = c.ChildLimit,
                                Description = c.Description,
                                IsActive = c.IsActive,
                                HighTemp = c.HighTemp,
                                LowTemp = c.LowTemp,
                                Length = c.Length,
                                SensorSpace = c.SensorSpace,
                                Offset = c.Offset,
                                ReadIndex = c.ReadIndex,
                                ReadDateTime = c.ReadDateTime,
                                LoopDiameter = c.LoopDiameter,
                                SiloDiameter = c.SiloDiameter,
                                TechNo = c.TechNo,
                                SiloHeight = c.SiloHeight,
                                Rom = c.Rom,
                            }
                        }).OrderBy(x => x.Index).ToList()
                    }).OrderBy(x => x.Index).ToList()
                }).OrderBy(x => x.Index).ToList()
            };
            public static Expression<Func<SiloItem, SiloItemResponse>> GetSiloItemResponse = t => new SiloItemResponse
            {
                Id = t.Id.ToString(),
                Name = t.Name,
                IsExpanded = t.IsExpanded,
                AirCondition = t.AirCondition,
                Feature = t.Feature,
                IsMapped = t.IsMapped,
                ItemType = t.ItemType,
                ParentId = t.ParentId.ToString(),
                Value = t.Value,
                WarningServiceType = t.WarningServiceType,
                Index = t.Index,
                Box = t.Box,
                Line = t.Line,
                Address = t.Address,
                FalseValueCount = t.FalseValueCount,
                IsReadOnly = t.IsReadOnly,
                FalseValue = t.FalseValue,
                ChildLimit = t.ChildLimit,
                Description = t.Description,
                IsActive = t.IsActive,
                HighTemp = t.HighTemp,
                LowTemp = t.LowTemp,
                Length = t.Length,
                SensorSpace = t.SensorSpace,
                Offset = t.Offset,
                ReadIndex = t.ReadIndex,
                ReadDateTime = t.ReadDateTime,
                LoopDiameter = t.LoopDiameter,
                SiloDiameter = t.SiloDiameter,
                TechNo = t.TechNo,
                SiloHeight = t.SiloHeight,
                Rom = t.Rom,
                FeaturedSensors = t.SiloItems.Where(x => x.Feature != SensorFeatureEnum.None).Select(f => new SiloItemResponse
                {
                    Id = f.Id.ToString(),
                    Name = f.Name,
                    IsExpanded = f.IsExpanded,
                    AirCondition = f.AirCondition,
                    Feature = f.Feature,
                    IsMapped = f.IsMapped,
                    ItemType = f.ItemType,
                    ParentId = f.ParentId.ToString(),
                    Value = f.Value,
                    WarningServiceType = f.WarningServiceType,
                    Index = f.Index,
                    Box = f.Box,
                    Line = f.Line,
                    Address = f.Address,
                    FalseValueCount = f.FalseValueCount,
                    IsReadOnly = f.IsReadOnly,
                    FalseValue = f.FalseValue,
                    ChildLimit = f.ChildLimit,
                    Description = f.Description,
                    IsActive = f.IsActive,
                    HighTemp = f.HighTemp,
                    LowTemp = f.LowTemp,
                    Length = f.Length,
                    SensorSpace = f.SensorSpace,
                    Offset = f.Offset,
                    ReadIndex = f.ReadIndex,
                    ReadDateTime = f.ReadDateTime,
                    LoopDiameter = f.LoopDiameter,
                    SiloDiameter = f.SiloDiameter,
                    TechNo = f.TechNo,
                    SiloHeight = f.SiloHeight,
                    Rom = f.Rom,
                }).OrderBy(o => o.Index).ToList(),
                SiloItems = t.SiloItems.Where(x => x.Feature == SensorFeatureEnum.None).Select(l => new SiloItemResponse
                {
                    Id = l.Id.ToString(),
                    Name = l.Name,
                    IsExpanded = l.IsExpanded,
                    AirCondition = l.AirCondition,
                    Feature = l.Feature,
                    IsMapped = l.IsMapped,
                    ItemType = l.ItemType,
                    ParentId = l.ParentId.ToString(),
                    Value = l.Value,
                    WarningServiceType = l.WarningServiceType,
                    Index = l.Index,
                    Box = l.Box,
                    Line = l.Line,
                    Address = l.Address,
                    FalseValueCount = l.FalseValueCount,
                    IsReadOnly = l.IsReadOnly,
                    FalseValue = l.FalseValue,
                    ChildLimit = l.ChildLimit,
                    Description = l.Description,
                    IsActive = l.IsActive,
                    HighTemp = l.HighTemp,
                    LowTemp = l.LowTemp,
                    Length = l.Length,
                    SensorSpace = l.SensorSpace,
                    Offset = l.Offset,
                    ReadIndex = l.ReadIndex,
                    ReadDateTime = l.ReadDateTime,
                    LoopDiameter = l.LoopDiameter,
                    SiloDiameter = l.SiloDiameter,
                    TechNo = l.TechNo,
                    SiloHeight = l.SiloHeight,
                    Rom = l.Rom,
                    FeaturedSensors = l.SiloItems.Where(x => x.Feature != SensorFeatureEnum.None).Select(f => new SiloItemResponse
                    {
                        Id = f.Id.ToString(),
                        Name = f.Name,
                        IsExpanded = f.IsExpanded,
                        AirCondition = f.AirCondition,
                        Feature = f.Feature,
                        IsMapped = f.IsMapped,
                        ItemType = f.ItemType,
                        ParentId = f.ParentId.ToString(),
                        Value = f.Value,
                        WarningServiceType = f.WarningServiceType,
                        Index = f.Index,
                        Box = f.Box,
                        Line = f.Line,
                        Address = f.Address,
                        FalseValueCount = f.FalseValueCount,
                        IsReadOnly = f.IsReadOnly,
                        FalseValue = f.FalseValue,
                        ChildLimit = f.ChildLimit,
                        Description = f.Description,
                        IsActive = f.IsActive,
                        HighTemp = f.HighTemp,
                        LowTemp = f.LowTemp,
                        Length = f.Length,
                        SensorSpace = f.SensorSpace,
                        Offset = f.Offset,
                        ReadIndex = f.ReadIndex,
                        ReadDateTime = f.ReadDateTime,
                        LoopDiameter = f.LoopDiameter,
                        SiloDiameter = f.SiloDiameter,
                        TechNo = f.TechNo,
                        SiloHeight = f.SiloHeight,
                        Rom = f.Rom,
                    }).OrderBy(o => o.Index).ToList(),
                    SiloItems = l.SiloItems.Select(c => new SiloItemResponse
                    {
                        Id = c.Id.ToString(),
                        Name = c.Name,
                        IsExpanded = c.IsExpanded,
                        AirCondition = c.AirCondition,
                        Feature = c.Feature,
                        IsMapped = c.IsMapped,
                        ItemType = c.ItemType,
                        ParentId = c.ParentId.ToString(),
                        Value = c.Value,
                        WarningServiceType = c.WarningServiceType,
                        Index = c.Index,
                        Box = c.Box,
                        Line = c.Line,
                        Address = c.Address,
                        FalseValueCount = c.FalseValueCount,
                        IsReadOnly = c.IsReadOnly,
                        FalseValue = c.FalseValue,
                        ChildLimit = c.ChildLimit,
                        Description = c.Description,
                        IsActive = c.IsActive,
                        HighTemp = c.HighTemp,
                        LowTemp = c.LowTemp,
                        Length = c.Length,
                        SensorSpace = c.SensorSpace,
                        Offset = c.Offset,
                        ReadIndex = c.ReadIndex,
                        ReadDateTime = c.ReadDateTime,
                        LoopDiameter = c.LoopDiameter,
                        SiloDiameter = c.SiloDiameter,
                        TechNo = c.TechNo,
                        SiloHeight = c.SiloHeight,
                        Rom = c.Rom,
                        FeaturedSensors = c.SiloItems.Where(x => x.Feature != SensorFeatureEnum.None).Select(f => new SiloItemResponse
                        {
                            Id = f.Id.ToString(),
                            Name = f.Name,
                            IsExpanded = f.IsExpanded,
                            AirCondition = f.AirCondition,
                            Feature = f.Feature,
                            IsMapped = f.IsMapped,
                            ItemType = f.ItemType,
                            ParentId = f.ParentId.ToString(),
                            Value = f.Value,
                            WarningServiceType = f.WarningServiceType,
                            Index = f.Index,
                            Box = f.Box,
                            Line = f.Line,
                            Address = f.Address,
                            FalseValueCount = f.FalseValueCount,
                            IsReadOnly = f.IsReadOnly,
                            FalseValue = f.FalseValue,
                            ChildLimit = f.ChildLimit,
                            Description = f.Description,
                            IsActive = f.IsActive,
                            HighTemp = f.HighTemp,
                            LowTemp = f.LowTemp,
                            Length = f.Length,
                            SensorSpace = f.SensorSpace,
                            Offset = f.Offset,
                            ReadIndex = f.ReadIndex,
                            ReadDateTime = f.ReadDateTime,
                            LoopDiameter = f.LoopDiameter,
                            SiloDiameter = f.SiloDiameter,
                            TechNo = f.TechNo,
                            SiloHeight = f.SiloHeight,
                            Rom = f.Rom,
                        }).OrderBy(o => o.Index).ToList(),
                        SiloItems = c.SiloItems.Select(s => new SiloItemResponse
                        {
                            Id = s.Id.ToString(),
                            Name = s.Name,
                            IsExpanded = s.IsExpanded,
                            AirCondition = s.AirCondition,
                            Feature = s.Feature,
                            IsMapped = s.IsMapped,
                            ItemType = s.ItemType,
                            ParentId = s.ParentId.ToString(),
                            Value = s.Value,
                            WarningServiceType = s.WarningServiceType,
                            Index = s.Index,
                            Box = s.Box,
                            Line = s.Line,
                            Address = s.Address,
                            FalseValueCount = s.FalseValueCount,
                            IsReadOnly = s.IsReadOnly,
                            FalseValue = s.FalseValue,
                            ChildLimit = s.ChildLimit,
                            Description = s.Description,
                            IsActive = s.IsActive,
                            HighTemp = s.HighTemp,
                            LowTemp = s.LowTemp,
                            Length = s.Length,
                            SensorSpace = s.SensorSpace,
                            Offset = s.Offset,
                            ReadIndex = s.ReadIndex,
                            ReadDateTime = s.ReadDateTime,
                            LoopDiameter = s.LoopDiameter,
                            SiloDiameter = s.SiloDiameter,
                            TechNo = s.TechNo,
                            SiloHeight = s.SiloHeight,
                            Rom = s.Rom,
                        }).OrderBy(x => x.Index).ToList()
                    }).OrderBy(x => x.Index).ToList()
                }).OrderBy(x => x.Index).ToList()
            };
        }
        public static class SensorHistories
        {
            public static Expression<Func<SensorHistory, SensorHistoryResponse>> SensorHistoryResponse = s => new SensorHistoryResponse
            {
                Id = s.Id,
                Value = s.Value,
                FalseValue = s.FalseValue,
                ConvertedValue = s.Sensor.Offset + s.Value,
                ReadDateTime = s.ReadDateTime,
                ReadIndex = s.ReadIndex,
                SensorId = s.SensorId,
            };
        }
        public static class Roles
        {
            public static Expression<Func<Role, RoleResponse>> DetailedRoleResponse = s => new RoleResponse
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Claims = s.RoleClaims.Select(x => new ClaimResponse
                {
                    ClaimType = x.ClaimType,
                    ClaimValue = x.ClaimValue,
                }).ToList(),
                Users = s.UserRoles.Select(x => new UserResponse
                {
                    Id = x.User.Id.ToString(),
                    Description = x.User.Description,
                    Email = x.User.Email,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Image = x.User.Image,
                    PhoneNumber = x.User.PhoneNumber,
                    ProfileName = x.User.ProfileName,
                }).ToList()
            };
            public static Expression<Func<Role, RoleResponse>> RoleResponse = s => new RoleResponse
            {
                Id = s.Id.ToString(),
                Name = s.Name,
            };


        }
        public static class RoleClaims
        {
            public static Expression<Func<RoleClaim, ClaimResponse>> ClaimResponse = s => new ClaimResponse
            {
                ClaimType = s.ClaimType,
                ClaimValue = s.ClaimValue,

            };

        }
        public static class AppSettings
        {
            public static Expression<Func<AppSetting, AppSettingsResponse>> AppSettingsResponse = s => new AppSettingsResponse
            {
                Id = s.Id.ToString(),
                Key = s.Key,
                Value = s.Value,
                Description = s.Description,
                IsReadOnly = s.IsReadOnly,

            };
        }
        public static class UserSettings
        {
            public static Expression<Func<UserSetting, UserSettingsResponse>> UserSettingsResponse = s => new UserSettingsResponse
            {
                Id = s.Id.ToString(),
                Culture = s.Culture,
                DarkMode = s.DarkMode,
                DefaultTenantId = s.DefaultTenantId.ToString(),
                RightToLeft = s.RightToLeft,
                Theme = s.Theme,
                UserEmail = s.User.Email,
                UserId = s.UserId.ToString(),
            };
        }

        public static class Cultures
        {
            public static Expression<Func<Culture, CultureResponse>> CultureResponse = x => new CultureResponse
            {
                Id = x.Id.ToString(),
                DisplayName = x.DisplayName,
                CultureName = x.CultureName,
                RightToLeft = x.RightToLeft,
                DateSeparator = x.DateSeparator,
                FullDateTimePattern = x.FullDateTimePattern,
                FirstDayOfWeek = DayOfWeek.Monday,
                IsDefault = x.IsDefault,
                LongDatePattern = x.LongDatePattern,
                LongTimePattern = x.LongTimePattern,
                MonthDayPattern = x.MonthDayPattern,
                ShortDatePattern = x.ShortDatePattern,
                ShortTimePattern = x.ShortTimePattern,
                TimeSeparator = x.TimeSeparator,
                YearMonthPattern = x.YearMonthPattern,
                Image = x.Image,
            };
        }

    }
}
