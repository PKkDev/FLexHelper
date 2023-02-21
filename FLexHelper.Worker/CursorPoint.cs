﻿using System;
using System.Runtime.InteropServices;

namespace FLexHelper.Worker
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CursorPoint
    {
        public int x;
        public int y;

        public CursorPoint(int X, int Y)
        {
            x = X;
            y = Y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Input
    {
        public SendInputEventType type;
        public MouseKeybdhardwareInputUnion mkhi;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MouseKeybdhardwareInputUnion
    {
        [FieldOffset(0)]
        public MouseInputData mi;

        [FieldOffset(0)]
        public KEYBDINPUT ki;

        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    }
    public struct MouseInputData
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public MouseEventFlags dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
    [Flags]
    public enum MouseEventFlags : uint
    {
        MOUSEEVENTF_MOVE = 0x0001,
        MOUSEEVENTF_LEFTDOWN = 0x0002,
        MOUSEEVENTF_LEFTUP = 0x0004,
        MOUSEEVENTF_RIGHTDOWN = 0x0008,
        MOUSEEVENTF_RIGHTUP = 0x0010,
        MOUSEEVENTF_MIDDLEDOWN = 0x0020,
        MOUSEEVENTF_MIDDLEUP = 0x0040,
        MOUSEEVENTF_XDOWN = 0x0080,
        MOUSEEVENTF_XUP = 0x0100,
        MOUSEEVENTF_WHEEL = 0x0800,
        MOUSEEVENTF_VIRTUALDESK = 0x4000,
        MOUSEEVENTF_ABSOLUTE = 0x8000
    }
    public enum SendInputEventType : int
    {
        InputMouse,
        InputKeyboard,
        InputHardware
    }


    public enum SystemMetric
    {
        SM_CXSCREEN = 0,
        SM_CYSCREEN = 1,
    }
}
