using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PGSauce.Core.PGDebugging
{
	public class PGDebugDirector
	{
		private RichTextDebugBuilder debugBuilder;
        private List<MessageStructure> messages;
        private MessageStructure currentMessage;
        private Object _context;
        private bool _condition;
        private System.Exception _exception;

        private MessageStructure CurrentMessage { get {
                if(currentMessage == null)
                {
                    throw new UnityException("You haven't called Message(string) on your debug message !");
                }

                return currentMessage;
            }
            set => currentMessage = value; }

        public PGDebugDirector()
        {
            debugBuilder = new RichTextDebugBuilder();
            Reset();
        }

        public PGDebugDirector SetException(System.Exception exception)
        {
            _exception = exception;
            return this;
        }

        public PGDebugDirector SetCondition(bool condition)
        {
            _condition = condition;
            return this;
        }

        public PGDebugDirector SetContext(Object context)
        {
            _context = context;
            return this;
        }

        public PGDebugDirector Message(string message)
        {
            CurrentMessage = new MessageStructure(message, Color.magenta, false, false, 14);
            messages.Add(CurrentMessage);
            return this;
        }

        public PGDebugDirector SetColor(Color c)
        {
            CurrentMessage.color = c;
            return this;
        }

        public PGDebugDirector SetBold()
        {
            CurrentMessage.isBold = true;
            return this;
        }

        public PGDebugDirector SetItalic()
        {
            CurrentMessage.isItalic = true;
            return this;
        }

        public PGDebugDirector SetSize(int size)
        {
            CurrentMessage.size = size;
            return this;
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
        {
            if(_condition)
            {
                Debug.DrawLine(start, end, color, duration, depthTest);
            }
            Reset();
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
        {
            if (_condition)
            {
                Debug.DrawRay(start, dir, color, duration, depthTest);
            }
            Reset();
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public void Pause()
        {
            Debug.Break();
            Reset();
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public void Log()
        {
            if (_condition)
            {
                Debug.Log(GetFullMessage(), _context);
            }

            Reset();
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public void LogWarning()
        {
            if (_condition)
            {
                Debug.LogWarning(GetFullMessage(), _context);
            }

            Reset();
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public void LogError()
        {
            if (_condition)
            {
                Debug.LogError(GetFullMessage(), _context);
            }

            Reset();
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR"), Conditional("UNITY_ASSERTIONS")]
        public void Assert()
        {
            Debug.Assert(_condition, GetFullMessage(), _context);
            Reset();
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public void Clear()
        {
            if (_condition)
            {
                Debug.ClearDeveloperConsole();
            }

            Reset();
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public void LogException()
        {
            if (_condition)
            {
                Debug.LogException(_exception, _context);
            }

            Reset();
        }

        private string GetFullMessage()
        {
            BuildMessages();
            return debugBuilder.Build();
        }

        private void BuildMessages()
        {
            foreach (var message in messages)
            {
                BuildMessage(message);
            }
        }

        private void Reset()
        {
            messages = new List<MessageStructure>();
            _condition = true;
            _context = null;
            _exception = null;
            debugBuilder.Build();
        }

        private void BuildMessage(MessageStructure message)
        {
            debugBuilder
                                .BeginColor(message.color)
                                .BeginSize(message.size);

            if (message.isItalic)
            {
                debugBuilder.BeginItalic();
            }

            if (message.isBold)
            {
                debugBuilder.BeginBold();
            }

            debugBuilder
                .Message(message.message);

            if (message.isBold)
            {
                debugBuilder.EndBold();
            }

            if (message.isItalic)
            {
                debugBuilder.EndItalic();
            }

            debugBuilder
                .EndSize()
                .EndColor();
        }

        private class MessageStructure
        {
            public string message;
            public Color color;
            public bool isBold;
            public bool isItalic;
            public int size;

            public MessageStructure(string message, Color color, bool isBold, bool isItalic, int size)
            {
                this.message = message;
                this.color = color;
                this.isBold = isBold;
                this.isItalic = isItalic;
                this.size = size;
            }
        }
	
    }
}
