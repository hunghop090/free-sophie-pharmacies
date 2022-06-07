using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sophie.Areas.Admin.Shared
{
    public static class NavigationPages
    {
        // Empty
        public static string Empty => "Empty";
        public static string Empty_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Empty);

        // Dashboard
        public static string Dashboard => "Dashboard";
        public static string Dashboard_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Dashboard);

        // Profiles
        public static string Profiles => "Profiles";
        public static string Profiles_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Profiles);

        // Client Manager
        public static string ClientManager => "Client Manager";
        public static string ClientManager_NavClass(ViewContext viewContext) => PageNavClass(viewContext, ClientManager);

        // User Manage
        public static string UserManage => "User Manage";
        public static string UserManage_NavClass(ViewContext viewContext) => PageNavClass(viewContext, UserManage);

        // Development
        public static string Development => "Development";
        public static string Development_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Development);

        // SystemSetting
        public static string SystemSetting => "System Setting";
        public static string SystemSetting_NavClass(ViewContext viewContext) => PageNavClass(viewContext, SystemSetting);

        // Backup & Restore - DatabaseTools
        public static string PostgreSQL => "PostgreSQL";
        public static string PostgreSQL_NavClass(ViewContext viewContext) => PageNavClass(viewContext, PostgreSQL);

        public static string MongoDB => "MongoDB";
        public static string MongoDB_NavClass(ViewContext viewContext) => PageNavClass(viewContext, MongoDB);

        // Account
        public static string List_Account => "List Account";
        public static string List_Account_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_Account);
        public static string Create_Account => "Create Account";
        public static string Create_Account_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_Account);
        public static string Edit_Account => "Edit Account";
        public static string Edit_Account_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Edit_Account);

        // Doctor
        public static string List_Doctor => "List Doctor";
        public static string List_Doctor_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_Doctor);
        public static string Create_Doctor => "Create Doctor";
        public static string Create_Doctor_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_Doctor);
        public static string Edit_Doctor => "Edit Doctor";
        public static string Edit_Doctor_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Edit_Doctor);

        // Hospital
        public static string List_Hospital => "List Hospital";
        public static string List_Hospital_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_Hospital);
        public static string Create_Hospital => "Create Hospital";
        public static string Create_Hospital_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_Hospital);
        public static string Edit_Hospital => "Edit Hospital";
        public static string Edit_Hospital_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Edit_Hospital);

        // Relationship
        public static string List_Relationship => "List Relationship";
        public static string List_Relationship_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_Relationship);
        public static string Create_Relationship => "Create Relationship";
        public static string Create_Relationship_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_Relationship);
        public static string Edit_Relationship => "Edit Relationship";
        public static string Edit_Relationship_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Edit_Relationship);

        // Appointment
        public static string List_Appointment => "List Appointment";
        public static string List_Appointment_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_Appointment);
        public static string Create_Appointment => "Create Appointment";
        public static string Create_Appointment_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_Appointment);
        public static string Edit_Appointment => "Edit Appointment";
        public static string Edit_Appointment_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Edit_Appointment);

        // Shop
        public static string List_Shop => "List Shop";
        public static string List_Shop_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_Shop);
        public static string Create_Shop => "Create Shop";
        public static string Create_Shop_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_Shop);

        // Promotion
        public static string List_Promotion => "List Promotion";
        public static string List_Promotion_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_Promotion);
        public static string Create_Promotion => "Create Promotion";
        public static string Create_Promotion_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_Promotion);

        // TransportPromotion
        public static string List_TransportPromotion => "List TransportPromotion";
        public static string List_TransportPromotion_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_TransportPromotion);
        public static string Create_TransportPromotion => "Create TransportPromotion";
        public static string Create_TransportPromotion_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_TransportPromotion);

        // Order
        public static string List_Order => "List Order";
        public static string List_Order_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_Order);

        // DraftOrder
        public static string List_DraftOrder => "List DraftOrder";
        public static string List_DraftOrder_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_DraftOrder);
        public static string Create_DraftOrder => "Create DraftOrder";
        public static string Create_DraftOrder_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_DraftOrder);


        // Product - Category
        public static string List_Category_Product => "List Category Product";
        public static string List_Category_Product_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_Category_Product);
        public static string Create_Category_Product => "Create Category Product";
        public static string Create_Category_Product_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_Category_Product);
        public static string Edit_Category_Product => "Edit Category Product";
        public static string Edit_Category_Product_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Edit_Category_Product);

        public static string List_Product => "List Product";
        public static string List_Product_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_Product);
        public static string Create_Product => "Create Product";
        public static string Create_Product_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_Product);
        public static string Edit_Product => "Edit Product";
        public static string Edit_Product_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Edit_Product);


        // Notification
        public static string SendPush_Notification => "Send Push Notification";
        public static string SendPush_Notification_NavClass(ViewContext viewContext) => PageNavClass(viewContext, SendPush_Notification);
        public static string List_Notification => "List Notification";
        public static string List_Notification_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_Notification);
        public static string Create_Notification => "Create Notification";
        public static string Create_Notification_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_Notification);
        public static string Edit_Notification => "Edit Notification";
        public static string Edit_Notification_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Edit_Notification);

        // News
        public static string List_News => "List News";
        public static string List_News_NavClass(ViewContext viewContext) => PageNavClass(viewContext, List_News);
        public static string Create_News => "Create News";
        public static string Create_News_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Create_News);
        public static string Edit_News => "Edit News";
        public static string Edit_News_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Edit_News);

        // Setting
        public static string Setting => "Setting Manager";
        public static string Setting_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Setting);

        // Logging
        public static string MongoLogs => "MongoLogs";
        public static string MongoLogs_NavClass(ViewContext viewContext) => PageNavClass(viewContext, MongoLogs);

        public static string FileLogs => "FileLogs";
        public static string FileLogs_NavClass(ViewContext viewContext) => PageNavClass(viewContext, FileLogs);

        // Support
        public static string Support => " Support";
        public static string Support_NavClass(ViewContext viewContext) => PageNavClass(viewContext, Support);



        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["NameActivePage"] as string ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
