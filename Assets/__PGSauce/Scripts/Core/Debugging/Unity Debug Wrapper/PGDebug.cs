using UnityEngine;
using Object = UnityEngine.Object;

namespace PGSauce.Core.PGDebugging
{
	public static class PGDebug
	{
		private static PGDebugDirector pgDebugDirector;

		static PGDebug()
        {
			pgDebugDirector = new PGDebugDirector();
        }

        public static PGDebugDirector SetException(System.Exception exception)
        {
            pgDebugDirector.SetException(exception);
            return pgDebugDirector;
        }

        public static PGDebugDirector SetCondition(bool condition)
        {
            pgDebugDirector.SetCondition(condition);
            return pgDebugDirector;
        }

        public static PGDebugDirector SetContext(Object context)
        {
            pgDebugDirector.SetContext(context);
            return pgDebugDirector;
        }

        public static PGDebugDirector Message(string message)
        {
            pgDebugDirector.Message(message);
            return pgDebugDirector;
        }

        public static PGDebugDirector SetColor(Color c)
        {
            pgDebugDirector.SetColor(c);
            return pgDebugDirector;
        }

        public static PGDebugDirector SetBold()
        {
            pgDebugDirector.SetBold();
            return pgDebugDirector;
        }

        public static PGDebugDirector SetItalic()
        {
            pgDebugDirector.SetItalic();
            return pgDebugDirector;
        }

        public static PGDebugDirector SetSize(int size)
        {
            pgDebugDirector.SetSize(size);
            return pgDebugDirector;
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
        {
            pgDebugDirector.DrawLine(start, end, color, duration, depthTest);
        }

        public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
        {
            pgDebugDirector.DrawRay(start, dir, color, duration, depthTest);
        }

        public static void Break()
        {
            Pause();
        }

        public static void Pause()
        {
            pgDebugDirector.Pause();
        }

        public static void Log()
        {
            pgDebugDirector.Log();
        }

        public static void LogWarning()
        {
            pgDebugDirector.LogWarning();
        }

        public static void LogError()
        {
            pgDebugDirector.LogError();
        }

        public static void Assert()
        {
            pgDebugDirector.Assert();
        }

        public static void Clear()
        {
            pgDebugDirector.Clear();
        }

        public static void LogException()
        {
            pgDebugDirector.LogException();
        }
    }
}
