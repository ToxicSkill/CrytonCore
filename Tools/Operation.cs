using System;
using System.Collections.Generic;

namespace CrytonCore.Ciphers
{
    public class Operation
    {
        public static byte[] AddByteToEndArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 0);
            newArray[newArray.Length - 1] = newByte;
            return newArray;
        }
        public static byte[] AddByteToBegArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }

        public static byte[] RemoveByteFromBegArray(byte[] bArray, int length)
        {
            byte[] newArray = new byte[bArray.Length - length];
            Array.Copy(bArray, length, newArray, 0, newArray.Length);
            return newArray;
        }
        public static byte[] RemoveByteFromEndArray(byte[] bArray, int length)
        {
            byte[] newArray = new byte[bArray.Length - length];
            Array.Copy(bArray, newArray, bArray.Length - length);
            return newArray;
        }
        public static IEnumerable<byte[]> ArraySplit(byte[] bArray, int intBufforLengt)
        {
            int bArrayLenght = bArray.Length;
            int i = 0;
            byte[] bReturn;

            for (; bArrayLenght > (i + 1) * intBufforLengt; i++)
            {
                bReturn = new byte[intBufforLengt];
                Array.Copy(bArray, i * intBufforLengt, bReturn, 0, intBufforLengt);
                yield return bReturn;
            }

            int intBufforLeft = bArrayLenght - i * intBufforLengt;
            if (intBufforLeft > 0)
            {
                bReturn = new byte[intBufforLeft];
                Array.Copy(bArray, i * intBufforLengt, bReturn, 0, intBufforLeft);
                yield return bReturn;
            }
        }
    }
}
