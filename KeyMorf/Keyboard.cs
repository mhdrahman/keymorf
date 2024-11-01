﻿namespace KeyMorf
{
    /// <summary>
    /// Class containing logic for interacting with the keyboard, i.e. sending key presses.
    /// </summary>
    internal static class Keyboard
    {
        /// <summary>
        /// Press and release the <paramref name="key"/> along with any modifiers in <paramref name="mods"/>.
        /// </summary>
        /// <param name="key">The key to pressed and released.</param>
        /// <param name="mods">Any modifier keys to be pressed with the <paramref name="key"/>.</param>
        public static void SendKey(Keys key, params Keys[] mods)
        {
            foreach (var mod in mods)
            {
                Press(mod);
            }

            Press(key);
            Release(key);

            foreach (var mod in mods)
            {
                Release(mod);
            }
        }

        /// <summary>
        /// Press the <paramref name="key"/> down.
        /// </summary>
        /// <param name="key">The <see cref="Keys"/> to be pressed.</param>
        private static void Press(Keys key)
            => Win32.keybd_event((byte)key, 0, Win32.KEYEVENTF_KEYDOWN, 0);

        /// <summary>
        /// Release the <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The <see cref="Keys"/> to be released.</param>
        private static void Release(Keys key)
            => Win32.keybd_event((byte)key, 0, Win32.KEYEVENTF_KEYUP, 0);
    }

    /// <summary>
    /// Enum containing keycodes for various keys - not exhaustive, just has the keys I needed.
    /// </summary>
    internal enum Keys
    {
        // Numbers
        Zero = 48,
        One = 49,
        Two = 50,
        Three = 51,
        Four = 52,
        Five = 53,
        Six = 54,
        Seven = 55,
        Eight = 56,
        Nine = 57,

        // Letters
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,

        // Symbols
        Backtick = 223,
        Minus = 189,
        Equal = 187,
        LSquareBracket = 219,
        RSquareBracket = 221,
        Semicolon = 186,
        Apostrophe = 192,
        Hash = 222,
        Backslash = 220,
        Comma = 188,
        Fullstop = 190,
        ForwardSlash = 191,

        // Movement and editing
        Escape = 27,
        Delete = 46,
        Backspace = 8,
        PageUp = 33,
        Tab = 9,
        Enter = 13,
        PageDown = 34,
        Caps = 20,
        Home = 36,
        LShift = 160,
        RShift = 161,
        Up = 38,
        End = 35,
        LControl = 162,
        Win = 91,
        LAlt = 164,
        Space = 32,
        RAlt = 165,
        RControl = 163,
        Left = 37,
        Down = 40,
        Right = 39,
    }
}
