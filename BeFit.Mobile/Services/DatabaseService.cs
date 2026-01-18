using BeFit.Mobile.Models;
using SQLite;

namespace BeFit.Mobile.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "befit.db3");
    }

    private async Task InitAsync()
    {
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        
        await _database.CreateTableAsync<ExerciseType>();
        await _database.CreateTableAsync<TrainingSession>();
        await _database.CreateTableAsync<TrainingEntry>();
    }

    public async Task<List<ExerciseType>> GetExerciseTypesAsync()
    {
        await InitAsync();
        return await _database!.Table<ExerciseType>().OrderBy(e => e.Name).ToListAsync();
    }

    public async Task<ExerciseType?> GetExerciseTypeAsync(int id)
    {
        await InitAsync();
        return await _database!.Table<ExerciseType>().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<int> SaveExerciseTypeAsync(ExerciseType exerciseType)
    {
        await InitAsync();
        if (exerciseType.Id != 0)
            return await _database!.UpdateAsync(exerciseType);
        else
            return await _database!.InsertAsync(exerciseType);
    }

    public async Task<int> DeleteExerciseTypeAsync(ExerciseType exerciseType)
    {
        await InitAsync();
        return await _database!.DeleteAsync(exerciseType);
    }

    public async Task<bool> IsExerciseTypeInUseAsync(int exerciseTypeId)
    {
        await InitAsync();
        var count = await _database!.Table<TrainingEntry>()
            .Where(e => e.ExerciseTypeId == exerciseTypeId)
            .CountAsync();
        return count > 0;
    }

    public async Task<List<TrainingSession>> GetTrainingSessionsAsync()
    {
        await InitAsync();
        return await _database!.Table<TrainingSession>().OrderByDescending(s => s.StartedAt).ToListAsync();
    }

    public async Task<TrainingSession?> GetTrainingSessionAsync(int id)
    {
        await InitAsync();
        return await _database!.Table<TrainingSession>().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<int> SaveTrainingSessionAsync(TrainingSession session)
    {
        await InitAsync();
        if (session.Id != 0)
            return await _database!.UpdateAsync(session);
        else
            return await _database!.InsertAsync(session);
    }

    public async Task<int> DeleteTrainingSessionAsync(TrainingSession session)
    {
        await InitAsync();
        
        await _database!.Table<TrainingEntry>()
            .Where(e => e.TrainingSessionId == session.Id)
            .DeleteAsync();
        
        return await _database!.DeleteAsync(session);
    }

    public async Task<List<TrainingEntry>> GetTrainingEntriesAsync()
    {
        await InitAsync();
        var entries = await _database!.Table<TrainingEntry>().ToListAsync();
        
        var exerciseTypes = await GetExerciseTypesAsync();
        var sessions = await GetTrainingSessionsAsync();
        
        foreach (var entry in entries)
        {
            entry.ExerciseType = exerciseTypes.FirstOrDefault(e => e.Id == entry.ExerciseTypeId);
            entry.TrainingSession = sessions.FirstOrDefault(s => s.Id == entry.TrainingSessionId);
        }
        
        return entries.OrderByDescending(e => e.TrainingSession?.StartedAt).ToList();
    }

    public async Task<List<TrainingEntry>> GetTrainingEntriesForSessionAsync(int sessionId)
    {
        await InitAsync();
        var entries = await _database!.Table<TrainingEntry>()
            .Where(e => e.TrainingSessionId == sessionId)
            .ToListAsync();
        
        var exerciseTypes = await GetExerciseTypesAsync();
        foreach (var entry in entries)
        {
            entry.ExerciseType = exerciseTypes.FirstOrDefault(e => e.Id == entry.ExerciseTypeId);
        }
        
        return entries;
    }

    public async Task<TrainingEntry?> GetTrainingEntryAsync(int id)
    {
        await InitAsync();
        var entry = await _database!.Table<TrainingEntry>().FirstOrDefaultAsync(e => e.Id == id);
        
        if (entry != null)
        {
            entry.ExerciseType = await GetExerciseTypeAsync(entry.ExerciseTypeId);
            entry.TrainingSession = await GetTrainingSessionAsync(entry.TrainingSessionId);
        }
        
        return entry;
    }

    public async Task<int> SaveTrainingEntryAsync(TrainingEntry entry)
    {
        await InitAsync();
        if (entry.Id != 0)
            return await _database!.UpdateAsync(entry);
        else
            return await _database!.InsertAsync(entry);
    }

    public async Task<int> DeleteTrainingEntryAsync(TrainingEntry entry)
    {
        await InitAsync();
        return await _database!.DeleteAsync(entry);
    }
}
