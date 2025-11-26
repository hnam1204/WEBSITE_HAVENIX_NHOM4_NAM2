using HV_NIX.Models;

namespace HV_NIX.Helpers
{
    public static class DbHelper
    {
        private static AppDbContext _db;
        public static AppDbContext Db => _db ?? (_db = new AppDbContext());
    }
}
