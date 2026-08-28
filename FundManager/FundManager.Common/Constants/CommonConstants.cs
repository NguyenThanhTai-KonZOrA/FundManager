namespace FundManager.Common.Constants
{
    public class CommonConstants
    {
        #region Created by system
        public const string SystemUser = "System";
        #endregion
        public const string UnknowUser = "Anonymous";
        public const int UnknowUserId = 0;
        public const string AdminUserName = "admin";
        public const string AdminPassword = "admin@123";
        public const string AdminRole = "Administrator";
        public const string UserRole = "user";
        public const int DefaultMemberId = 1;
        public const string DefaultConnection = "DefaultConnection";
        public const string BreakFastCheckInConnection = "BreakFastCheckInConnection";
        public const string QualityControlPosition = "Software Development Quality Control Engineer";

        // Application Settings Keys
        public const string CacheExpirationMinutesKey = "CacheExpiration";
        public const string EnableCheckAdministratorKey = "EnableCheckAdministrator";
        public const string EnableCreateGitHubRepositoryKey = "EnableCreateGitHubRepository";
        public const string MaxDataLoadOnPageKey = "MaxDataLoadOnPage";
        public const string EnableSendEmailKey = "IsSentEmail";
        public const string ProjectManagementWebURLKey = "ProjectManagementWebURL";
        public const string InActiveDaysThresholdKey = "InActiveDaysThreshold";
        public const string ListEmailCCKey = "ListEmailCC";
        public const string ListEmailToKey = "ListEmailTo";

        // Permissions constants
        public const string CanViewDashboard = "CAN_VIEW_DASHBOARD";
        public const string CanCheckRoom = "CAN_CHECK_ROOM";
        public const string CanUncheck = "CAN_UNCHECK";
        public const string CanViewHistory = "CAN_VIEW_HISTORY";
        public const string CanViewReports = "CAN_VIEW_REPORTS";
        public const string CanViewAllReports = "CAN_VIEW_ALL_REPORTS";

        // Roles constants
        public const string OutletStaff = "OutletStaff";

        // Application Settings Data Types
        public const string String = "String";
        public const string Integer = "Integer";
        public const string Decimal = "Decimal";
        public const string Boolean = "Boolean";
        public const string Json = "Json";

        // Workflow
        public const string DefaultWorkflowName = "Default";

        // Language
        public const string DefaultLanguage = "en";
    }
}