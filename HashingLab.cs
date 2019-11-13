//Duy Nguyen
//CSCI 303
//Lab 3
/*
 * GRADE:       27.25 / 30
 * COMMENTS:    Pretty good job.
 * 
 *              The biggest problem is in the "probe" method, when you loop until
 *              result >= 0; if result starts out as a huge negative number, this loop could run for
 *              millions of iterations! Instead, you should mod out by the table size BEFORE the loop;
 *              then the loop can become a simple "if" statement.
 *              
 *              The "isPrime" method can also be made more efficient.
 *              
 *              Search for "-sjr" to find my comments.
*/

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

        public HashTable(int theSize, bool useLinearProbing = false)    //optional parameter useLinearProbing defaults to false (fixed)
        {
            linearProbing = useLinearProbing;
            if (!linearProbing) goodValue(ref theSize);
            items = new T[theSize];
            occupied = new bool[theSize];
        }   // 3.5 / 4. See isPrime. -sjr (fixed)

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
            for (int i = 2; i <= r; i++)        //Inefficient: you only need to loop through the square root of number. -sjr (fixed)
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
        }// 6/6. -sjr

        private uint probe(uint index, int order)
        {
            int n = items.Length;
            if (linearProbing) return (uint)((index + order) % n);
            else
            {
                int h = (order + 1) / 2;
                //int result = (int)index + (int)(Math.Pow(-1, order + 1)) * (h * h); //Dangerous: Do not mix floating-point and integer arithmetic. -sjr
                int sign = (order % 2 == 0) ? -1 : 1;
                result = (sign*h*h) % n;
                result = (result >= 0) ? result : result += n;
                //while (result < 0) result += n;//This is extremely inefficient: mod out by items.Length FIRST. -sjr
                return (uint)(result);
            }
        }// 6/8. -sjr

        public bool retrieveItem(ref T theItem)//theItem comes with its key fields filled, returns with all fields filled if found
        {
            uint key = theItem.getKey();
            uint home = hashFunction(key);
            for (int i = 0; i < items.Length; i++)
            {
                uint index = probe(home, i);
                if (!occupied[index]) return false;
                else if (items[index].getKey() == theItem.getKey()) //"occupied[index]" is redundant here. -sjr (fixed
                {
                    theItem = items[index];
                    return true;
                }
            }
            return false;
        }// 5.75 / 6. -sjr

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
        }// 6/6. -sjr

        private void fold(uint key, out uint a, out uint b)
        {
            string numStr = key.ToString();
            string aStr = numStr.Substring(0, (int)(numStr.Length / 2));
            a = (aStr.Equals("")) ? 0 : uint.Parse(aStr);
            string bStr = numStr.Substring((int)(numStr.Length / 2));
            b = (bStr.Equals("")) ? 0 : uint.Parse(bStr);
        }//ok. -sjr
    }
}

interface IKeyed
{
    uint getKey();
}