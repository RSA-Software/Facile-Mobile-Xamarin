using SQLite;

namespace Facile.Interfaces
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
        void RemoveDB();
    }
}

