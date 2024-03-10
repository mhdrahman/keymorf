﻿using KeebSharp.Handlers;
using KeebSharp.Interop;
using KeebSharp.Logging;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace KeebSharp
{
    /// <summary>
    /// Contains the startup and run logic for KeyMorf.
    /// </summary>
    public static class Program
    {
        private static IntPtr _hookId = IntPtr.Zero;
        private static readonly ConsoleLogger _logger = new(LogLevel.Info);
        private static readonly Handler _handler = new(_logger);

        // Constant value which indicates that an event was handled.
        private static readonly IntPtr Handled = new(1);

        /// <summary>
        /// Entry point for the application.
        /// </summary>
        public static void Main()
        {
            _hookId = SetKeyboardHook();
            if (_hookId == IntPtr.Zero)
            {
                _logger.Error($"{new Win32Exception(Marshal.GetLastWin32Error())}");
                Environment.Exit(-1);
            }

            Console.CancelKeyPress += (_, _) =>
            {
                _logger.Info("KeebSharp is exiting. Please wait...");
                Exit(0);
            };

            _logger.Info("KeebSharp is running. Press <C-c> to exit.");

            // This pumps a message loop. It allows Windows to break into the thread and make the call back.
            // Passing IntPtr.Zero to GetMessage retrieves messages for any window that belongs to
            // the current thread, and any messages on the current thread's message queue. As there
            // are no windows, we should never actually recieve a message.
            var message = new User32.Message();
            while (User32.GetMessage(ref message, IntPtr.Zero, 0, 0))
            {
                _logger.Error($"Recieved a message unexpectedly: {JsonSerializer.Serialize(message)}. Exiting...");
                Exit(-1);
            }
        }

        /// <summary>
        /// Install the low-level keyboard hook.
        /// </summary>
        /// <returns>The installed hook id if it was installed successfully.
        /// Returns IntPtr.Zero if an error occured while installing the hook.</returns>
        private static IntPtr SetKeyboardHook()
        {
            using var process = Process.GetCurrentProcess();
            using var module = process.MainModule;

            if (module == null)
            {
                _logger.Error($"{nameof(module)} was null.");
                return IntPtr.Zero;
            }

            if (module.ModuleName == null)
            {
                _logger.Error($"{nameof(module.ModuleName)} was null.");
                return IntPtr.Zero;
            }

            var moduleHandle = Kernel32.GetModuleHandle(module.ModuleName);
            if (moduleHandle == IntPtr.Zero)
            {
                _logger.Error($"{new Win32Exception(Marshal.GetLastWin32Error())}");
                return IntPtr.Zero;
            }

            return User32.SetWindowsHookEx((int)Constants.WH_KEYBOARD_LL, KeyboardHook, Kernel32.GetModuleHandle(module.ModuleName), 0);
        }

        /// <summary>
        /// The keyboard hook to be installed.
        /// </summary>
        /// <param name="nCode">Indicates whether <paramref name="lParam"/> and <paramref name="wParam"/>
        /// contains valid data. A value of 0 indicates valid data.</param>
        /// <param name="wParam">Event type.</param>
        /// <param name="lParam">Virtual key code of the key associated with the event.</param>
        private static IntPtr KeyboardHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                // Never seen this happen before...
                _logger.Warn($"{nameof(nCode)} was less than zero");
                return User32.CallNextHookEx(_hookId, nCode, wParam, lParam);
            }

            // _handler won't be null here because it's set in the Start function
            var handled = _handler!.Handle(wParam, lParam);
            if (handled)
            {
                return Handled;
            }    

            // For any unhandled keys, let the key be processed as normal
            return User32.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        /// <summary>
        /// Exit the application with the specified <paramref name="exitCode"/>.
        /// </summary>
        /// <param name="exitCode">The exit code for the process.</param>
        private static void Exit(int exitCode)
        {
            User32.UnhookWindowsHookEx(_hookId);
            Environment.Exit(exitCode);
        }
    }
} 