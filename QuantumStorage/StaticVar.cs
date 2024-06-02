using QuantumStorage.Database;

namespace QuantumStorage {
  public class StaticVar {
    public static DatabaseQ database;

    public static void OnCleanUp() {
      if (database != null) database = null;
    }
  }
}