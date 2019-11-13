//Duy Nguyen
//CSCI 303
//Lab 3

using System;
using System.Collections.Generic;
using System.Text;

namespace HashingLab
{
    class HashTable<T>
    where T : IKeyed
    {
        private T[] items;
        private bool linearProbing;
        private bool[] occupied;

        public HashTable(int theSize, bool useLinearProbing = false)
        {
            linearProbing = useLinearProbing;
            if (!linearProbing) goodValue(ref theSize);
            items = new T[theSize];
            occupied = new bool[theSize];
        }

        #region Check for good value in Size

        private void goodValue(ref int theSize)
        {
            while (!checkQPSize(theSize))
            {
                theSize++;
            }
        }

        private bool checkQPSize(int theSize)
        {
            if (theSize == 1 || theSize == 2) return true;
            else if (isPrime(theSize) && theSize % 4 == 3) return true;
            else if (theSize % 2 == 0 && isPrime(theSize / 2) && (theSize / 2) % 4 == 3) return true;
            else return false;
        }

        private bool isPrime(int number)
        {
            int r = Math.Sqrt(number);
            for (int i = 2; i <= r; i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        #endregion

        public void addItem(T theItem)
        {
            uint key = theItem.getKey();
            uint home = hashFunction(key);
            for (int i = 0; i < items.Length; i++)
            {
                uint index = probe(home, i);
                if (!occupied[index])
                {
                    items[index] = theItem;
                    occupied[index] = true;
                    return;
                }
            }
            throw new Exception("The Hash Table is full !!!");
        }

        private uint probe(uint index, int order)
        {
            int n = items.Length;
            if (linearProbing) return (uint)((index + order) % n);
            else
            {
                int h = (order + 1) / 2;
                int sign = (order % 2 == 0) ? -1 : 1;
                result = (sign*h*h) % n;
                result = (result >= 0) ? result : result += n;
                return (uint)(result);
            }
        }

        public bool retrieveItem(ref T theItem)
        {
            uint key = theItem.getKey();
            uint home = hashFunction(key);
            for (int i = 0; i < items.Length; i++)
            {
                uint index = probe(home, i);
                if (!occupied[index]) return false;
                else if (items[index].getKey() == theItem.getKey())
                {
                    theItem = items[index];
                    return true;
                }
            }
            return false;
        }

        private uint hashFunction(uint key)
        {
            uint a, b, hashValue = key;
            for (uint i = 0; i < 3; i++)
            {
                fold(hashValue, out a, out b);
                a = (a == 0) ? 1 : a;
                b = (b == 0) ? 1 : b;
                hashValue = (((a * b) + 8) % (uint)items.Length);
            }
            return hashValue;
        }

        private void fold(uint key, out uint a, out uint b)
        {
            string numStr = key.ToString();
            string aStr = numStr.Substring(0, (int)(numStr.Length / 2));
            a = (aStr.Equals("")) ? 0 : uint.Parse(aStr);
            string bStr = numStr.Substring((int)(numStr.Length / 2));
            b = (bStr.Equals("")) ? 0 : uint.Parse(bStr);
        }
    }
}

interface IKeyed
{
    uint getKey();
}
