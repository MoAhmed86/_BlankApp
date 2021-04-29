using System.Collections.Generic;
using System.Security.Claims;

namespace Core.DTOs.Identity
{
    public static class ClaimsStore
    {
        public static List<Claim> claims = new List<Claim>() {
            //Users
            new Claim("UserReadOnly","استعراض بيانات المستخدمين"),
            new Claim("CreateUser","إنشاء مستخدم"),
            new Claim("EditUser","تعديل مستخدم"),
            new Claim("DeleteUser","حذف مستخدم"),
            //Roles
            new Claim("RoleReadOnly","استعراض بيانات الأدوار"),
            new Claim("CreateRole","إنشاء دور"),
            new Claim("EditRole","تعديل دور"),
            new Claim("DeleteRole","حذف دور"),
            //Competitions
            new Claim("CompetitionReadOnly","استعراض بيانات المسابقة"),
            new Claim("CreateCompetition","إنشاء مسابقة"),
            new Claim("EditCompetition","تعديل مسابقة"),
            new Claim("DeleteCompetition","حذف مسابقة"),

            //Fields
            new Claim("FieldReadOnly","استعراض بيانات المجالات"),
            new Claim("AddField","إضافة مجال"),
            new Claim("EditField","تعديل مجال"),
            new Claim("DeleteField","حذف مجال"),
            //Participations
            new Claim("ParticipationReadOnly","استعراض بيانات المشاركة"),
            //Judging
            new Claim("ParticipationEvaluation","تقييم المشاركة"),
            new Claim("ManageOrganizers","إدارة الجهات")
        };
    }

    public static class RolesStore
    {
        public static string SuperAdmin = "مسئول النظام";
        public static string CompetitionOfficial = "مسئول المسابقة";
        public static string Judge = "محكم";
        public static string Reviewer = "مدقق";
    }
}
