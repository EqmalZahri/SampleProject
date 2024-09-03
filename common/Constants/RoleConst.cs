namespace Rbac_IctJohor.common.Constants
{
    public static class RoleConst
    {
        public static string UCMP_Administrator = "UcmpAdministrator";
        public static string Catalogue_Administrator = "CatalogueAdministrator";
        public static string Cloud_Request_Approver = "CloudRequestApprover";
        public static string Special_Cloud_Request_Approver = "SpecialCloudRequestApprover";
        public static string Cloud_Administrator = "CloudAdministrator";
        public static string Cloud_Viewer = "CloudViewer";
        public static string Cloud_Editor = "CloudEditor";

        public static List<string> All()
        {
            return new List<string>
            {
                UCMP_Administrator,
                Catalogue_Administrator,
                Cloud_Request_Approver,
                Special_Cloud_Request_Approver,
                Cloud_Administrator,
                Cloud_Viewer,
                Cloud_Editor
            };
        }

        public static string Normalized(this string input)
        {
            return input.ToUpper();
        }
    }
}
