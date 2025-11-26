using HV_NIX.Models;
using System;

namespace HV_NIX.Helpers
{
    public static class LogHelper
    {
        public static void AddLog(AppDbContext db, int? userId, string action, string desc)
        {
            db.ActivityLogs.Add(new ActivityLog
            {
                UserID = userId,  // nullable -> KHÔNG LỖI
                Action = action,
                Description = desc,
                CreatedAt = DateTime.Now
            });

            db.SaveChanges();
        }
    }
}
