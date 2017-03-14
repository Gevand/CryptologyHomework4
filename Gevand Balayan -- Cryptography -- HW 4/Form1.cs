using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gevand_Balayan____Cryptography____HW_4
{

    public partial class Form1 : Form
    {
        Encoding enc = Encoding.GetEncoding("ISO-8859-1");
        Byte[] key = new Byte[16];
        List<Byte[]> ExpandedKeys;
        Byte[,] SBox = new Byte[16, 16];
        Byte[,] ISBox = new Byte[16, 16];
        Byte[,] RCon = new Byte[11, 4];
        public Form1()
        {
            InitializeComponent();

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            GenerateSBox();
            //GenerateRandomKey();
            key = Encoding.ASCII.GetBytes("nq596kECzmth2qIT");
            txtKey.Text = System.Text.Encoding.UTF8.GetString(key);
            ExpandedKeys = ExpandKey(key);
            if (txtInput.Text.Length == 0)
            {
                MessageBox.Show("Please enter an input in the text box");
                return;
            }
            if (radEnc.Checked)
            {
                txtOutput.Text = GenerateCypher(txtInput.Text);
            }
            else
                txtOutput.Text = DecryptCypher(txtInput.Text);
        }
        private string DecryptCypher(string input)
        {
            try
            {
                string returnString = "";
                //convert input to bytes
                Byte[] inputAsBytes = enc.GetBytes(input);
                //split it into size 16 arrays
                var chunks = Split<Byte>(inputAsBytes, 16);
                foreach (var chunk in chunks)
                {
                    Byte[] temp = chunk.ToArray();
                    //add 0s to the end of the array if its not 16
                    while (temp.Length != 16)
                    {
                        var list = temp.ToList();
                        list.Add(0);
                        temp = list.ToArray();
                    }
                    Byte[] currentKey = (Byte[])ExpandedKeys[ExpandedKeys.Count - 1].Clone();
                    for (int i = 0; i < temp.Length; i++)
                    {
                        //XOR with the chunk of data with the current key
                        temp[i] = (byte)((int)temp[i] ^ (int)currentKey[i]);
                    }

                    for (int step = 10; step > 0; step--)
                    {
                        //Sub with the inverse S-Box
                        if (step <= 10)
                        {
                            temp = SubWordInv(temp);
                            Byte[,] matrix = InvShiftRow(temp);
                            currentKey = (Byte[])ExpandedKeys[step - 1].Clone();
                            for (int i = 0; i < temp.Length; i++)
                            {
                                //XOR with the chunk of data with the current key
                                matrix[i / 4, i % 4] = (byte)((int)matrix[i / 4, i % 4] ^ (int)currentKey[i]);
                            }
                            if (step != 1)
                                matrix = InvMixedColumns(matrix);
                            //flatten the matrix and save it as an array
                            for (int i = 0; i < 4; i++)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    temp[i * 4 + j] = matrix[i, j];
                                }
                            }
                        }
                    }

                    returnString += System.Text.Encoding.ASCII.GetString(temp);
                }

                return returnString;
            }
            catch (Exception ex)
            {
                return "COULDN'T DECRYPT, THERE WAS AN ERROR";
            }
        }
        private string GenerateCypher(string input)
        {
            string returnString = "";
            //convert input to bytes
            Byte[] inputAsBytes = Encoding.ASCII.GetBytes(input);
            //split it into size 16 arrays
            var chunks = Split<Byte>(inputAsBytes, 16);
            foreach (var chunk in chunks)
            {
                Byte[] temp = chunk.ToArray();
                //add 0s to the end of the array if its not 16
                while (temp.Length != 16)
                {
                    var list = temp.ToList();
                    list.Add(0);
                    temp = list.ToArray();
                }
                for (int step = 0; step <= 10; step++)
                {
                    Byte[] currentKey = (Byte[])ExpandedKeys[step].Clone();
                    for (int i = 0; i < temp.Length; i++)
                    {
                        //XOR with the chunk of data with the current key
                        temp[i] = (byte)((int)temp[i] ^ (int)currentKey[i]);
                    }
                    //Sub with the S-Box
                    if (step < 10)
                    {
                        temp = SubWord(temp);
                        Byte[,] matrix = ShiftRow(temp);
                        if (step != 9)
                            matrix = MixedColumns(matrix);
                        //flatten the matrix and save it as an array
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                temp[i * 4 + j] = matrix[i, j];
                            }
                        }
                    }
                }
                returnString += enc.GetString(temp);
            }
            return returnString;
        }
        private Byte[,] MixedColumns(Byte[,] input)
        {
            Byte[,] returnedByte = (Byte[,])input.Clone();

            for (int c = 0; c < 4; ++c)
            {
                returnedByte[0, c] = (byte)((int)gfAESmultiplication02(input[0, c]) ^ (int)gfAESmultiplication03(input[1, c]) ^
                                           (int)gfAESmultiplication01(input[2, c]) ^ (int)gfAESmultiplication01(input[3, c]));
                returnedByte[1, c] = (byte)((int)gfAESmultiplication01(input[0, c]) ^ (int)gfAESmultiplication02(input[1, c]) ^
                                           (int)gfAESmultiplication03(input[2, c]) ^ (int)gfAESmultiplication01(input[3, c]));
                returnedByte[2, c] = (byte)((int)gfAESmultiplication01(input[0, c]) ^ (int)gfAESmultiplication01(input[1, c]) ^
                                           (int)gfAESmultiplication02(input[2, c]) ^ (int)gfAESmultiplication03(input[3, c]));
                returnedByte[3, c] = (byte)((int)gfAESmultiplication03(input[0, c]) ^ (int)gfAESmultiplication01(input[1, c]) ^
                                           (int)gfAESmultiplication01(input[2, c]) ^ (int)gfAESmultiplication02(input[3, c]));
            }
            return returnedByte;
        }
        private Byte[,] InvMixedColumns(Byte[,] input)
        {
            Byte[,] returnedByte = (Byte[,])input.Clone();

            for (int c = 0; c < 4; ++c)
            {
                returnedByte[0, c] = (byte)((int)gfAESmultiplication0e(input[0, c]) ^ (int)gfAESmultiplication0b(input[1, c]) ^
                                   (int)gfAESmultiplication0d(input[2, c]) ^ (int)gfAESmultiplication09(input[3, c]));
                returnedByte[1, c] = (byte)((int)gfAESmultiplication09(input[0, c]) ^ (int)gfAESmultiplication0e(input[1, c]) ^
                                           (int)gfAESmultiplication0b(input[2, c]) ^ (int)gfAESmultiplication0d(input[3, c]));
                returnedByte[2, c] = (byte)((int)gfAESmultiplication0d(input[0, c]) ^ (int)gfAESmultiplication09(input[1, c]) ^
                                           (int)gfAESmultiplication0e(input[2, c]) ^ (int)gfAESmultiplication0b(input[3, c]));
                returnedByte[3, c] = (byte)((int)gfAESmultiplication0b(input[0, c]) ^ (int)gfAESmultiplication0d(input[1, c]) ^
                                           (int)gfAESmultiplication09(input[2, c]) ^ (int)gfAESmultiplication0e(input[3, c]));
            }
            return returnedByte;
        }
        private Byte[,] InvShiftRow(Byte[] input)
        {
            Byte[,] returnedByte = new Byte[4, 4];
            Byte[,] temp = new Byte[4, 4];
            //turn flat array into 2d one
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    temp[i, j] = input[i * 4 + j];
                }
            }
            //copy the first row. it stays as is
            for (int i = 0; i < 4; i++)
                returnedByte[0, i] = input[i];
            //rotate each other row to the right
            for (int i = 1; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    returnedByte[i, (i + j) % 4] = temp[i, j];
                }
            }
            return returnedByte;
        }
        private Byte[,] ShiftRow(Byte[] input)
        {
            Byte[,] returnedByte = new Byte[4, 4];
            Byte[,] temp = new Byte[4, 4];
            //turn flat array into 2d one
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    temp[i, j] = input[i * 4 + j];
                }
            }
            //copy the first row. it stays as is
            for (int i = 0; i < 4; i++)
                returnedByte[0, i] = input[i];
            //rotate each other row to the left
            for (int i = 1; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    returnedByte[i, j] = temp[i, (i + j) % 4];
                }
            }
            return returnedByte;
        }
        private List<Byte[]> ExpandKey(Byte[] key)
        {

            List<Byte[]> returnedList = new List<byte[]>();
            returnedList.Add(key);

            for (int i = 4; i < 44; i++)
            {
                if (i % 4 == 0)
                {
                    Byte[] tempKey = new Byte[16];
                    Byte[] lastKey = returnedList[returnedList.Count - 1];
                    Byte[] lastWord = lastKey.Skip(12).Take(4).ToArray();
                    Byte[] tempWord = SubWord(RotateWord(lastWord));
                    tempWord[0] = (byte)((int)tempWord[0] ^ (int)RCon[i / 4, 0]);
                    for (int z = 0; z < 4; z++)
                    {
                        tempKey[z] = (byte)((int)tempWord[z] ^ (int)lastWord[z]);
                    }
                    returnedList.Add(tempKey);
                }
                else
                {
                    //get last key and its 4th word
                    Byte[] lastKey = returnedList[returnedList.Count - 2];
                    Byte[] lastSpecialWord = lastKey.Skip(12).Take(4).ToArray();
                    //get current key's last word
                    Byte[] currentKey = returnedList[returnedList.Count - 1];
                    Byte[] lastWord = currentKey.Skip((i % 4 - 1) * 4).Take(4).ToArray();
                    //or them together
                    Byte[] tempWord = new Byte[4];
                    for (int z = 0; z < 4; z++)
                    {
                        tempWord[z] = (byte)((int)lastWord[z] ^ (int)lastSpecialWord[z]);
                    }
                    for (int j = (i % 4) * 4; j < (i % 4 + 1) * 4; j++)
                    {
                        currentKey[j] = tempWord[j % 4];
                    }
                    returnedList[returnedList.Count - 1] = currentKey;
                }
            }
            return returnedList;
        }
        private Byte[] RotateWord(Byte[] input)
        {
            var temp = new Byte[input.Length];
            //shift every byte by 1 to the left [1] becomes [0], [2] becomes [1], etc
            for (int i = 0; i < input.Length - 1; i++)
            {
                temp[i] = input[i + 1];
            }
            //last byte is now what the first byte used to be
            temp[input.Length - 1] = input[0];
            return temp;
        }
        private Byte[] SubWord(Byte[] input)
        {
            var temp = new Byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                //get the right nibble with a mask
                int col = (byte)input[i] & 0x0F;
                //get the left nibble with a mask
                int row = (byte)((input[i] & 0xF0) >> 4);
                temp[i] = this.SBox[row, col];
            }
            return temp;
        }
        private Byte[] SubWordInv(Byte[] input)
        {
            var temp = new Byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                //get the right nibble with a mask
                int col = (byte)input[i] & 0x0F;
                //get the left nibble with a mask
                int row = (byte)((input[i] & 0xF0) >> 4);
                temp[i] = this.ISBox[row, col];
            }
            return temp;
        }
        private void GenerateRandomKey()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rnd = new Random();
            for (int i = 0; i < 16; i++)
            {
                key[i] = Convert.ToByte(chars[rnd.Next(chars.Length)]);
            }
        }
        private void GenerateSBox()
        {
            this.SBox = new byte[16, 16] {  
            /* 0     1     2     3     4     5     6     7     8     9     a     b     c     d     e     f */
    /*0*/  {0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76},
    /*1*/  {0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0},
    /*2*/  {0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15},
    /*3*/  {0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75},
    /*4*/  {0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84},
    /*5*/  {0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf},
    /*6*/  {0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8},
    /*7*/  {0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2},
    /*8*/  {0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73},
    /*9*/  {0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb},
    /*a*/  {0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79},
    /*b*/  {0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08},
    /*c*/  {0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a},
    /*d*/  {0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e},
    /*e*/  {0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf},
    /*f*/  {0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16} };

            this.ISBox = new byte[16, 16] {  
            /* 0     1     2     3     4     5     6     7     8     9     a     b     c     d     e     f */
    /*0*/  {0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb},
    /*1*/  {0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb},
    /*2*/  {0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e},
    /*3*/  {0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25},
    /*4*/  {0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92},
    /*5*/  {0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84},
    /*6*/  {0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06},
    /*7*/  {0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b},
    /*8*/  {0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73},
    /*9*/  {0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e},
    /*a*/  {0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b},
    /*b*/  {0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4},
    /*c*/  {0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f},
    /*d*/  {0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef},
    /*e*/  {0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61},
    /*f*/  {0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d} };

            this.RCon = new byte[11, 4] { {0x00, 0x00, 0x00, 0x00},  
                                   {0x01, 0x00, 0x00, 0x00},
                                   {0x02, 0x00, 0x00, 0x00},
                                   {0x04, 0x00, 0x00, 0x00},
                                   {0x08, 0x00, 0x00, 0x00},
                                   {0x10, 0x00, 0x00, 0x00},
                                   {0x20, 0x00, 0x00, 0x00},
                                   {0x40, 0x00, 0x00, 0x00},
                                   {0x80, 0x00, 0x00, 0x00},
                                   {0x1b, 0x00, 0x00, 0x00},
                                   {0x36, 0x00, 0x00, 0x00} };
        }
        private static IEnumerable<IEnumerable<T>> Split<T>(ICollection<T> self, int chunkSize)
        {
            var splitList = new List<List<T>>();
            var chunkCount = (int)Math.Ceiling((double)self.Count / (double)chunkSize);

            for (int c = 0; c < chunkCount; c++)
            {
                var skip = c * chunkSize;
                var take = skip + chunkSize;
                var chunk = new List<T>(chunkSize);

                for (int e = skip; e < take && e < self.Count; e++)
                {
                    chunk.Add(self.ElementAt(e));
                }

                splitList.Add(chunk);
            }

            return splitList;
        }
        //Found these on the internet that help with GF field math, had to modify them from C to C sharp.
        #region GF(2^8) MATH
        private static byte gfAESmultiplication01(byte b)
        {
            return b;
        }
        private static byte gfAESmultiplication02(byte b)
        {
            if (b < 0x80)
                return (byte)(int)(b << 1);
            else
                return (byte)((int)(b << 1) ^ (int)(0x1b));
        }
        private static byte gfAESmultiplication03(byte b)
        {
            return (byte)((int)gfAESmultiplication02(b) ^ (int)b);
        }
        private static byte gfAESmultiplication09(byte b)
        {
            return (byte)((int)gfAESmultiplication02(gfAESmultiplication02(gfAESmultiplication02(b))) ^
                           (int)b);
        }
        private static byte gfAESmultiplication0b(byte b)
        {
            return (byte)((int)gfAESmultiplication02(gfAESmultiplication02(gfAESmultiplication02(b))) ^
                           (int)gfAESmultiplication02(b) ^
                           (int)b);
        }
        private static byte gfAESmultiplication0d(byte b)
        {
            return (byte)((int)gfAESmultiplication02(gfAESmultiplication02(gfAESmultiplication02(b))) ^
                           (int)gfAESmultiplication02(gfAESmultiplication02(b)) ^
                           (int)(b));
        }
        private static byte gfAESmultiplication0e(byte b)
        {
            return (byte)((int)gfAESmultiplication02(gfAESmultiplication02(gfAESmultiplication02(b))) ^
                           (int)gfAESmultiplication02(gfAESmultiplication02(b)) ^
                           (int)gfAESmultiplication02(b));
        }
        #endregion
    }
}
