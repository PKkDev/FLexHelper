using FLexHelper.Worker.Model;
using System.Runtime.InteropServices;

namespace FLexHelper.Worker
{
    public class MouseEvent
    {
        public static void ClickLeftMouseButton(CursorPoint cursorPos)
        {
            Input[] inputs = new Input[]
            {
                new Input()
                {
                    type = SendInputEventType.InputMouse,
                    mkhi = new MouseKeybdhardwareInputUnion()
                    {
                        mi = new MouseInputData()
                        {
                            // dx = cursorPos.x,
                            // dy = cursorPos.y,
                            dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN,
                            dwExtraInfo = Win32.GetMessageExtraInfo(),
                            mouseData = 0
                        }
                    }
                },
                new Input()
                {
                    type =SendInputEventType.InputMouse,
                    mkhi = new MouseKeybdhardwareInputUnion()
                    {
                        mi = new MouseInputData()
                        {
                            dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP,
                            dwExtraInfo = Win32.GetMessageExtraInfo(),
                            mouseData = 0
                        }
                    }
                }
            };

            uint uSent1 = Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        public static void ClickRightMouseButton(CursorPoint cursorPos)
        {
            Input[] inputs = new Input[]
            {
                new Input()
                {
                    type = SendInputEventType.InputMouse,
                    mkhi = new MouseKeybdhardwareInputUnion()
                    {
                        mi = new MouseInputData()
                        {
                            // dx = cursorPos.x,
                            // dy = cursorPos.y,
                            dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTDOWN,
                            dwExtraInfo = Win32.GetMessageExtraInfo(),
                            mouseData = 0
                        }
                    }
                },
                new Input()
                {
                    type =SendInputEventType.InputMouse,
                    mkhi = new MouseKeybdhardwareInputUnion()
                    {
                        mi = new MouseInputData()
                        {
                            dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTUP,
                            dwExtraInfo = Win32.GetMessageExtraInfo(),
                            mouseData = 0
                        }
                    }
                }
            };

            uint uSent1 = Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }
    }
}
