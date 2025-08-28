using System.Linq;

namespace PipStore {
    public class LogUtil {
        private const string PREFIX = "【皮皮商店|PipStore】 ";

        private static string GetMessage(object[] args) {
            return PREFIX + string.Join(" ", args.Select(arg => arg.ToString()));
        }

        public static void Info(params object[] args) {
            Debug.Log(GetMessage(args));
        }

        public static void Warning(params object[] args) {
            Debug.LogWarning(GetMessage(args));
        }

        public static void Error(params object[] args) {
            Debug.LogError(GetMessage(args));
        }
    }
}