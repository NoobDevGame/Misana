using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Misana.Network
{
    public static class BlockAllocator
    {
        public const int BlockSize = 64;
        public const int BlockStackCount = 16;
        public const int MaxSize = BlockSize * BlockStackCount;
        
        private static readonly Stack<IntPtr>[] BlockStacks = new Stack<IntPtr>[BlockStackCount];

        static BlockAllocator()
        {
            const int prefill = 16;

            for (int i = 0; i < BlockStackCount; i++)
            {
                BlockStacks[i] = new Stack<IntPtr>();

                for (int j = 0; j < prefill; j++)
                {
                    BlockStacks[i].Push(Marshal.AllocHGlobal((i + 1) * BlockSize));
                }
            }
        }

        public static IntPtr Alloc(int minSize, out int actualSize)
        {
            var idx = minSize / BlockSize - 1;
            if (minSize % BlockSize > 0)
                idx++;

            if (idx >= BlockStackCount)
            {
                actualSize = minSize;
                return Marshal.AllocHGlobal(minSize);
            }

            actualSize = (idx + 1) * BlockSize;

            var stack = BlockStacks[idx];

            if (stack.Count > 0)
            {
                lock (stack)
                {
                    if (stack.Count > 0)
                    {
                        return stack.Pop();
                    }
                }
            }

            return Marshal.AllocHGlobal(actualSize);
        }

        public static void Free(IntPtr ptr, int actualSize)
        {
            if (actualSize > MaxSize)
            {
                Marshal.FreeHGlobal(ptr);
                return;
            }
            
            var stack = actualSize <= BlockSize 
                ? BlockStacks[0]
                : BlockStacks[actualSize / BlockSize - 1];

            lock (stack)
            {
                stack.Push(ptr);
            }
        }
    }
}