﻿/***************************************************************************
 *   NativeMethods.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
#endregion

namespace UltimaXNA.Core.Windows
{
    public delegate IntPtr WndProcHandler(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    class NativeMethods
    {
        [DllImport("Kernel32")]
        private unsafe static extern int _lread(SafeFileHandle hFile, void* lpBuffer, int wBytes);

        internal static unsafe void ReadBuffer(SafeFileHandle ptr, void* buffer, int length)
        {
            _lread(ptr, buffer, length);
        }

        [DllImport("Imm32.dll")]
        internal static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("Imm32.dll")]
        internal static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("user32.dll")]
        internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        internal static extern int MultiByteToWideChar(int CodePage, int dwFlags, byte[] lpMultiByteStr, int cchMultiByte, char[] lpWideCharStr, int cchWideChar);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern short GetKeyState(int keyCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr SetWindowsHookEx(int hookType, WndProcHandler callback, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetMessage(out Message lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "TranslateMessage")]
        internal static extern bool TranslateMessage(ref Message m);

        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr window, IntPtr module);

        internal static int LOWORD(IntPtr val)
        {
            return (unchecked((int)(long)val)) & 0xFFFF;
        }

        internal static int MAKELCID(int languageID, int sortID)
        {
            return ((0xFFFF & languageID) | (((0x000F) & sortID) << 16));
        }

        internal static int MAKELANGID(int primaryLang, int subLang)
        {
            return ((((ushort)(subLang)) << 10) | (ushort)(primaryLang));
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int GetLocaleInfo(int locale, int lcType, out uint lpLCData, int cchData);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetKeyboardLayout(uint idThread);

        internal static uint GetCurrentCodePage()
        {
            // Get the keyboard layout for the current thread.
            IntPtr keybdLayout = GetKeyboardLayout(0);

            // Extract the language ID from it, contained in its low-order word.
            int langID = LOWORD(keybdLayout);

            // Call the GetLocaleInfo function to retrieve the default ANSI code page
            // associated with that language ID.
            uint codePage = 0;
            GetLocaleInfo(MAKELCID(langID, NativeConstants.SORT_DEFAULT),
                           NativeConstants.LOCALE_IDEFAULTANSICODEPAGE | NativeConstants.LOCALE_RETURN_NUMBER,
                           out codePage,
                           Marshal.SizeOf(codePage));
            return codePage;
        }
    }
}
