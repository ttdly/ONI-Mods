namespace PipStore {
    public class LogUtil {
        private const string PREFIX = "【皮皮商店|PipStore】";

        public static void Info(string message) {
            Debug.Log($"{PREFIX} {message}");
        }

        public static void Warning(string message) {
            Debug.LogWarning($"{PREFIX} {message}");
        }

        public static void Error(string message) {
            Debug.LogError($"{PREFIX} {message}");
        }
    }
}