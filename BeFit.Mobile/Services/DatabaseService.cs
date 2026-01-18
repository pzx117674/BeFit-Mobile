using BeFit.Mobile.Models;
using SQLite;

namespace BeFit.Mobile.Services;

/// <summary>
/// Serwis bazodanowy SQLite do przechowywania danych lokalnie
/// </summary>
public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "befit.db3");
    }

    /// <summary>
    /// Inicjalizacja połączenia z bazą danych i tworzenie tabel
    /// </summary>
    private async Task InitAsync()
    {
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        
        await _database.CreateTableAsync<ExerciseType>();
        await _database.CreateTableAsync<TrainingSession>();
        await _database.CreateTableAsync<TrainingEntry>();
    }

    #region ExerciseType CRUD

    /// <summary>
    /// Pobiera wszystkie typy ćwiczeń
    /// </summary>
    public async Task<List<ExerciseType>> GetExerciseTypesAsync()
    {
        await InitAsync();
        return await _database!.Table<ExerciseType>().OrderBy(e => e.Name).ToListAsync();
    }

    /// <summary>
    /// Pobiera typ ćwiczenia po ID
    /// </summary>
    public async Task<ExerciseType?> GetExerciseTypeAsync(int id)
    {
        await InitAsync();
        return await _database!.Table<ExerciseType>().FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <summary>
    /// Zapisuje typ ćwiczenia (dodaje nowy lub aktualizuje istniejący)
    /// </summary>
    public async Task<int> SaveExerciseTypeAsync(ExerciseType exerciseType)
    {
        await InitAsync();
        if (exerciseType.Id != 0)
            return await _database!.UpdateAsync(exerciseType);
        else
            return await _database!.InsertAsync(exerciseType);
    }

    /// <summary>
    /// Usuwa typ ćwiczenia
    /// </summary>
    public async Task<int> DeleteExerciseTypeAsync(ExerciseType exerciseType)
    {
        await InitAsync();
        return await _database!.DeleteAsync(exerciseType);
    }

    /// <summary>
    /// Sprawdza czy typ ćwiczenia jest używany w jakimkolwiek wpisie
    /// </summary>
    public async Task<bool> IsExerciseTypeInUseAsync(int exerciseTypeId)
    {
        await InitAsync();
        var count = await _database!.Table<TrainingEntry>()
            .Where(e => e.ExerciseTypeId == exerciseTypeId)
            .CountAsync();
        return count > 0;
    }

    #endregion

    #region TrainingSession CRUD

    /// <summary>
    /// Pobiera wszystkie sesje treningowe
    /// </summary>
    public async Task<List<TrainingSession>> GetTrainingSessionsAsync()
    {
        await InitAsync();
        return await _database!.Table<TrainingSession>().OrderByDescending(s => s.StartedAt).ToListAsync();
    }

    /// <summary>
    /// Pobiera sesję treningową po ID
    /// </summary>
    public async Task<TrainingSession?> GetTrainingSessionAsync(int id)
    {
        await InitAsync();
        return await _database!.Table<TrainingSession>().FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Zapisuje sesję treningową (dodaje nową lub aktualizuje istniejącą)
    /// </summary>
    public async Task<int> SaveTrainingSessionAsync(TrainingSession session)
    {
        await InitAsync();
        if (session.Id != 0)
            return await _database!.UpdateAsync(session);
        else
            return await _database!.InsertAsync(session);
    }

    /// <summary>
    /// Usuwa sesję treningową wraz z powiązanymi wpisami
    /// </summary>
    public async Task<int> DeleteTrainingSessionAsync(TrainingSession session)
    {
        await InitAsync();
        
        // Usuń powiązane wpisy treningowe (cascade delete)
        await _database!.Table<TrainingEntry>()
            .Where(e => e.TrainingSessionId == session.Id)
            .DeleteAsync();
        
        return await _database!.DeleteAsync(session);
    }

    #endregion

    #region TrainingEntry CRUD

    /// <summary>
    /// Pobiera wszystkie wpisy treningowe z danymi powiązanymi
    /// </summary>
    public async Task<List<TrainingEntry>> GetTrainingEntriesAsync()
    {
        await InitAsync();
        var entries = await _database!.Table<TrainingEntry>().ToListAsync();
        
        // Ładowanie powiązanych danych
        var exerciseTypes = await GetExerciseTypesAsync();
        var sessions = await GetTrainingSessionsAsync();
        
        foreach (var entry in entries)
        {
            entry.ExerciseType = exerciseTypes.FirstOrDefault(e => e.Id == entry.ExerciseTypeId);
            entry.TrainingSession = sessions.FirstOrDefault(s => s.Id == entry.TrainingSessionId);
        }
        
        return entries.OrderByDescending(e => e.TrainingSession?.StartedAt).ToList();
    }

    /// <summary>
    /// Pobiera wpisy treningowe dla danej sesji
    /// </summary>
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

    /// <summary>
    /// Pobiera wpis treningowy po ID
    /// </summary>
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

    /// <summary>
    /// Zapisuje wpis treningowy (dodaje nowy lub aktualizuje istniejący)
    /// </summary>
    public async Task<int> SaveTrainingEntryAsync(TrainingEntry entry)
    {
        await InitAsync();
        if (entry.Id != 0)
            return await _database!.UpdateAsync(entry);
        else
            return await _database!.InsertAsync(entry);
    }

    /// <summary>
    /// Usuwa wpis treningowy
    /// </summary>
    public async Task<int> DeleteTrainingEntryAsync(TrainingEntry entry)
    {
        await InitAsync();
        return await _database!.DeleteAsync(entry);
    }

    #endregion
}
